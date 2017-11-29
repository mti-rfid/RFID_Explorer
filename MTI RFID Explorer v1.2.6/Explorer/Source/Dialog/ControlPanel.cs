/*
 *****************************************************************************
 *                                                                           *
 *                   MTI CONFIDENTIAL AND PROPRIETARY                        *
 *                                                                           *
 * This source code is the sole property of MTI, Inc.  Reproduction or       *
 * utilization of this source code in whole or in part is forbidden without  *
 * the prior written consent of MTI, Inc.                                    *
 *                                                                           *
 * (c) Copyright MTI, Inc. 2011. All rights reserved.                        *
 *                                                                           *
 *****************************************************************************
 */

/*
 *****************************************************************************
 *
 * $Id: ControlPanel.cs,v 1.12 2009/12/23 00:41:13 dciampi Exp $
 * 
 * Description: 
 *
 *****************************************************************************
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Windows.Forms;

using RFID.RFIDInterface;


namespace RFID_Explorer
{
    public partial class ControlPanelForm : Form
    {
        private static Thread				_controlPanelThread = null;
        private static ControlPanelForm		_controlPanel		= null;
        private static RFID_Explorer.mainForm _mainForm			= null;

        public static ControlPanelForm ControlPanel
        {
            get { return ControlPanelForm._controlPanel; }
        }

        public static Thread ControlPanelThread
        {
            get { return ControlPanelForm._controlPanelThread; }
        }

        public static Thread LaunchControlPanel(RFID_Explorer.mainForm form)
        {
            if (form == null)
                throw new ArgumentNullException("form");

            if (ControlPanelThread != null) return ControlPanelThread;

            _mainForm = form;

            ControlPanelForm._controlPanelThread = new Thread(ControlPanelThreadProc);
            ControlPanelThread.Name = "ControlPanel";
            ControlPanelThread.Priority = ThreadPriority.Highest;
            ControlPanelThread.IsBackground = false;
            ControlPanelThread.Start();
            return ControlPanelThread;
        }


        public static void CloseControlPanel()
        {
            try
            {
                if (_controlPanelThread != null && _controlPanelThread.IsAlive)
                {
                    _controlPanel.Invoke(new MethodInvoker(_controlPanel.Close));
                    _controlPanelThread.Join();
                    _controlPanelThread = null;
                }
            }
            catch (Exception) { }
        }

        public static void SetTopMost(bool topMost)
        {
            if (_controlPanelThread != null && _controlPanelThread.IsAlive && ControlPanel != null && ControlPanel.Created)
            {
                if (topMost)
                    _controlPanel.Invoke(new MethodInvoker(_controlPanel.SetAsTopMost));
                else
                    _controlPanel.Invoke(new MethodInvoker(_controlPanel.ResetAsTopMost));
            }
        }

        public static void ShowControlPanel(bool show)
        {
            if (_controlPanelThread != null && _controlPanelThread.IsAlive && ControlPanel != null && ControlPanel.Created)
            {
                if (show)
                    _controlPanel.Invoke(new MethodInvoker(_controlPanel.ShowControlPanel));
                else
                    _controlPanel.Invoke(new MethodInvoker(_controlPanel.HideControlPanel));
            }
        }

        [STAThread]
        static void ControlPanelThreadProc()
        {
            System.Diagnostics.Debug.WriteLine(String.Format("{0} is threadID {1}", System.Threading.Thread.CurrentThread.Name, System.Threading.Thread.CurrentThread.ManagedThreadId));
            _controlPanel = new ControlPanelForm();
            
            _controlPanel.StartPosition = FormStartPosition.Manual;
            _controlPanel.ShowInTaskbar = false;
            //_controlPanel.TopLevel = false;
            //_controlPanel.Owner = MainForm; // doesn't work
            //_controlPanel.Show(MainForm); // doesn't work
            Application.Run(_controlPanel);
            System.Diagnostics.Debug.WriteLine("ControlPanel Thread is exiting");
        }



        public ControlPanelForm()
        {
            InitializeComponent();
            
            foreach (ToolStripItem item in toolStrip1.Items)
            {
                ToolStripButton button = item as ToolStripButton;
                if (button != null)
                {
                    switch (button.Name)
                    {
                    case "inventoryButton":
                    case "inventoryOnceButton":
                    case "clearButton":
                    case "BUTTON_TagAccess":
                    case "BUTTON_RegisterAccess":
                    case "BUTTON_ConfigureReader":
                        button.Tag = FunctionControl.FunctionState.Idle;
                        break;

                    case "stopButton":
                        button.Tag = FunctionControl.FunctionState.Running;
                        break;

                    case "pauseButton":
                        button.Tag = FunctionControl.FunctionState.Running | FunctionControl.FunctionState.Paused;
                        break;

                    case "abortButton":
                        button.Tag = FunctionControl.FunctionState.Running | FunctionControl.FunctionState.Stopping;
                        break;

                    default:
                        button.Tag = FunctionControl.FunctionState.Unknown;
                        break;
                    }
                }
            }
            SetButtonState();
        }


        private void ControlPanelForm_Load(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.Assert(MainForm != null);
            

            Point pt = MainForm.Location;
            pt.Offset(MainForm.Width, (RectangleToScreen(MainForm.ClientRectangle).Height / 2) - Width / 2);
            Rectangle r = Screen.GetBounds(this.DesktopBounds);
            if (pt.X + Width > r.Width)
                pt.X = r.Right - Width;

            this.Location = pt;

            MainForm.CurrentContextChanged +=new EventHandler(CurrentContextChanged);
            MainForm.BindAllFunctionControlers(FunctionStateChanged, true);
        }

        private void SetAsTopMost()
        {
            if (this.InvokeRequired)
            {
                Invoke(new MethodInvoker(SetAsTopMost));
                return;
            }
            this.TopMost = true;
        }

        private void ResetAsTopMost()
        {
            if (this.InvokeRequired)
            {
                Invoke(new MethodInvoker(SetAsTopMost));
                return;
            }
            this.TopMost = false;
        }

        private void ShowControlPanel()
        {
            if (this.InvokeRequired)
            {
                Invoke(new MethodInvoker(ShowControlPanel));
                return;
            }
            this.Visible = true;
        }

        private void HideControlPanel()
        {
            if (this.InvokeRequired)
            {
                Invoke(new MethodInvoker(HideControlPanel));
                return;
            }
            this.Visible = false;
        }

        public static RFID_Explorer.mainForm MainForm
        {
            get { return ControlPanelForm._mainForm; }
            set { ControlPanelForm._mainForm = value; }
        }


        private void SetButtonState()
        {
            if (this.InvokeRequired)
            {
                Invoke(new MethodInvoker(SetButtonState));
                return;
            }
            String ctrlname  = MainForm.ActiveControler == null ? "not active" : MainForm.ActiveControler.Name;

            FunctionControl.FunctionState newState =  
                MainForm.ActiveControler == null ? FunctionControl.FunctionState.Unknown : MainForm.ActiveControler.State;

            pauseButton.Checked = newState == FunctionControl.FunctionState.Paused;
            
            foreach (ToolStripItem item in toolStrip1.Items)
            {
                ToolStripButton b = item as ToolStripButton;
                if (b != null)
                {
//                    string s = b.Text;
                    bool newEnabledState = 
                        newState != FunctionControl.FunctionState.Unknown &&
                        (int)(((FunctionControl.FunctionState)b.Tag) & newState) != 0;

					if (newEnabledState && MainForm.IsVirtualReaderActiveContext)
                    {
                        if (!(  b == inventoryButton        ||
                                /*
                                b == inventoryOnceButton    ||           //clark 2011.2.8 copied from R1000 Tracer
                                */
                                b == stopButton             ||
                                b == pauseButton            ||
                                b == abortButton            ||
                                b == clearButton            ))
                        {
                            newEnabledState = false;
                        }
                    }
                    if (b.Enabled != newEnabledState)
                        b.Enabled = newEnabledState;

                }
            }

        }


        void CurrentContextChanged(object sender, EventArgs e)
        {
            SetButtonState();
        }


        void FunctionStateChanged(object sender, EventArgs e)
        {
            SetButtonState();
        }


        public EventHandler FunctionStateChangeHandler
        {
            get { return FunctionStateChanged; }
        }

        private void ControlPanelForm_MouseEnter(object sender, EventArgs e)
        {
            Select();
        }


        private void pauseButton_Click(object sender, EventArgs e)
        {
            if (MainForm.ActiveControler != null)
            {
                if (pauseButton.Checked)
                {
                    pauseButton.Text = "Resume";
                    MainForm.PauseCurrentOperation();
                    MainForm.ActiveControler.RequestPause();
                }
                else
                {
                    pauseButton.Text = "Pause";
                    MainForm.ContinueAfterPause();
                    MainForm.ActiveControler.Continue();
                }
            }
        }


        private void stopButton_Click(object sender, EventArgs e)
        {
            if (MainForm.ActiveControler != null)
                MainForm.ActiveControler.RequestStop();
        }

        private void abortToolStripButton_Click(object sender, EventArgs e)
        {
            if (MainForm.ActiveControler != null)
                MainForm.ActiveControler.RequestAbort();
        }


        private void inventoryOnceButton_Click(object sender, EventArgs e)
        {
            LakeChabotReader reader = MainForm.ActiveReader;

            //clark 2011.4.25 Set tag access flag to inventory structure
            Global.TagAccessFlag strcTagFlag;
            strcTagFlag.PostMatchFlag = MainForm.IsPostSingulationEnable;
            strcTagFlag.SelectOpsFlag = MainForm.IsSelectCriteriaEnable;
            strcTagFlag.RetryCount    = MainForm.GetRetryCount();
            strcTagFlag.bErrorKeepRunning = false;

            reader.strcTagFlag = strcTagFlag;

            MainForm.StartInventoryOnce();
        }

        private void inventoryButton_Click(object sender, EventArgs e)
        {
            LakeChabotReader reader = MainForm.ActiveReader;

            //clark 2011.4.25 Set tag access flag to inventory structure
            Global.TagAccessFlag strcTagFlag;
            strcTagFlag.PostMatchFlag = MainForm.IsPostSingulationEnable;
            strcTagFlag.SelectOpsFlag = MainForm.IsSelectCriteriaEnable;
            strcTagFlag.RetryCount    = MainForm.GetRetryCount();
            strcTagFlag.bErrorKeepRunning = MainForm.bErrorKeepRunning;


            reader.strcTagFlag = strcTagFlag;

            MainForm.StartMonitorInventory();
        }

        private void clearButton_Click(object sender, EventArgs e)
        {
            MainForm.ClearSessionData();
        }

        private void BUTTON_TagAccess_Click( object sender, EventArgs e )
        {
            LakeChabotReader reader = MainForm.ActiveReader;
            if ( reader != null )
            {
                using (FORM_TagAccess dlg = new FORM_TagAccess(reader, reader.TagAccessDataSet))
                {
                    if ( DialogResult.OK == dlg.ShowDialog( ) )
                    {
                        reader.TagAccessDataSet = dlg.TagAccessDataSet;
                        MainForm.StartTagAccess();
                    }
                    else
                    {
                        return;
                    }
                }
            }
        }

        private void BUTTON_RegsiterAccess_Click(object sender, EventArgs e)
        {
            LakeChabotReader reader = MainForm.ActiveReader;
            if (reader != null)
            {
                using (FORM_RegisterAccess dlg = new FORM_RegisterAccess(reader))
                {
                    dlg.ShowDialog();
                }
            }
        }

        private void BUTTON_ConfigureReader_Click(object sender, EventArgs e)
        {
            LakeChabotReader reader = MainForm.ActiveReader;
            if (reader != null)
            {
                using (ConfigureForm frm = new ConfigureForm(reader))
                {
                    frm.ShowDialog();
                }
            }
        }


    }

    
    public class Win32User
    {
        [System.Runtime.InteropServices.DllImport("User32")]
        public static extern UInt32 BringWindowToTop(IntPtr hwnd);

        [System.Runtime.InteropServices.DllImport("User32")]
        public static extern UInt32 PostMessage(IntPtr hWnd, UInt32 Msg, UInt32 wParam, UInt32 lParam);

    }


}