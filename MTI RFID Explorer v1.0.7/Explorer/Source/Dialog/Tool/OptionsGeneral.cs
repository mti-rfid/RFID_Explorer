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
 * $Id: OptionsGeneral.cs,v 1.5 2009/11/13 02:01:45 dciampi Exp $
 * 
 * Description: 
 *
 *****************************************************************************
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using RFID_Explorer.Properties;

namespace RFID_Explorer
{
    public partial class OptionsGeneralControl : UserControl
    {
        public OptionsGeneralControl(string[] viewArray)
        {
            InitializeComponent();

            defaultViewComboBox.Items.AddRange(viewArray);
            string defView = Properties.Settings.Default.defaultView;
            for (int i = 0; i < defaultViewComboBox.Items.Count; i++)
            {
                string val = defaultViewComboBox.Items[i].ToString();
                if (val == defView)
                {
                    defaultViewComboBox.SelectedIndex = i;
                    break;
                }
            }

            maximizeCheckBox.Checked = Properties.Settings.Default.maximizeOnStartUp;
            automaticallyIndexCheckBox.Checked = Properties.Settings.Default.automaticIndex;
            confirmExitCheckBox.Checked = Properties.Settings.Default.confirmExit;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            this.defaultViewComboBox.SelectedIndexChanged += new System.EventHandler(this.defaultViewComboBox_SelectedIndexChanged);

            OptionsForm parent = this.TopLevelControl as OptionsForm;
            if (parent == null)
            {
                System.Diagnostics.Debug.Assert(false, String.Format("Unknown parent form: {0}", this.TopLevelControl.GetType()));
            }
            else
            {
                parent.DialogClose +=new DialogCloseDelegate(DialogClose);
            }
        }

        void DialogClose(object sender, DialogCloseEventArgs e)
        {
        }

        private void maximizeCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.maximizeOnStartUp = maximizeCheckBox.Checked;
            Properties.Settings.Default.Save( );
        }

        private void automaticallyIndexCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.automaticIndex = automaticallyIndexCheckBox.Checked;
            Properties.Settings.Default.Save( );
        }

        private void confirmExitCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.confirmExit = confirmExitCheckBox.Checked;
            Properties.Settings.Default.Save( );
        }

        private void defaultViewComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            string viewName = (string)defaultViewComboBox.SelectedItem;
            if (!String.IsNullOrEmpty(viewName))
            {
                RFID_Explorer.Properties.Settings.Default.defaultView = viewName;
                Properties.Settings.Default.Save( );
            }
        }
    }
}
