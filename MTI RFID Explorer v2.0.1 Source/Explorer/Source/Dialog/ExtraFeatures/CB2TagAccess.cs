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
 * $Id: TagAccess.cs,v 1.6 2009/12/16 23:38:22 dciampi Exp $
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
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Threading;

using rfid;
using rfid.Constants;
using rfid.Structures;
using RFID.RFIDInterface;
using Global;

namespace RFID_Explorer
{
    public partial class cB2TagAccess : Form
    {
        private static TagAccessData _tagAccessData;
        private static TagAccessReads _tagAccessReads;

        //Add by FJ for Initial value
        rfid.Structures.AntennaPortConfig antennaPortConfig = new rfid.Structures.AntennaPortConfig();
        public AutoResetEvent StartEvent = new AutoResetEvent(false);
        CB2_Data Tag_Infor = new CB2_Data();
        public rfid.Constants.Result CB2_result_Major;
        public rfid.Constants.Result CB2_result_Sub;
        public UInt32 CB2_uiModelNameMAJOR = 0;
        public UInt32 CB2_uiModelNameSUB = 0;
        public UInt16 Original_Power_Level = 0;
        public UInt16 Original_Physical_Port = 0;

        //Add by FJ for implement recovery mechanism for CB-2 function, 2017/12/07
        public UInt16 Original_InventoryCycles = 0;
        public UInt16 Original_dwellTime = 0;

        //Tag Group
        public rfid.Constants.Selected Original_Selected = Selected.SELECT_ALL;
        public rfid.Constants.Session Original_Session = Session.S2;
        public rfid.Constants.SessionTarget Original_Target = SessionTarget.A;
        public SingulationAlgorithm algorithm = SingulationAlgorithm.UNKNOWN;

        //DynamicQ
        public byte Original_DQStartQValue = (byte)4;
        public byte Original_DQMinQValue = (byte)0;
        public byte Original_DQMaxQValue = (byte)15;
        public byte Original_DQRetryCount = (byte)0;
        public byte Original_DQToggleTarget = (byte)1;
        public byte Original_DQThresholdMultiplier = (byte)4;

        //FixedQ
        public byte Original_FQQValue = 0;
        public byte Original_FQRetryCount = 0;
        public byte Original_FQToggleTarget = 0;
        public byte Original_FQRepeatUntilNoTags = 0;
        //End by FJ for implement recovery mechanism for CB-2 function, 2017/12/07

        //Add by FJ for getting button to read status of CB-2 tag control function, 2017/12/07
        public DateTime ds1, ds2;
        public TimeSpan ts1, ts2;
        public bool LED_Flag, Battery_Flag, Antenna_Flag, Version_Flag;
        //End by FJ for getting button to read status of CB-2 tag control function, 2017/12/07

        int dataEPCStart = 26, dataEPCEnd = 42; //Reference the "MTI RFID Module Command Reference Manual-Table 3.9 - ISO 18000-6C Inventory-Response Packet Fields"
        int dataTIDStart = 26, dataTIDEnd = 32; //Reference the "MTI RFID Module Command Reference Manual-Table 3.10 - ISO 18000-6C Tag-Access Packet Fields"
        int dataCB2Start = 26, dataCB2End = 28;//Reference the "MTI RFID Module Command Reference Manual-Table 3.10 - ISO 18000-6C Tag-Access Packet Fields"
        int rptFlags = 7; //Reference the "MTI RFID Module Command Reference Manual-Table 3.10 - ISO 18000-6C Tag-Access Packet Fields"
        public byte retryCount = 0x00;

        private LakeChabotReader _reader = null;
        public TagAccessReads TagAccessReadSet//??加參數??//Add LargeRead command
        {
            get { return _tagAccessReads; }
            set { _tagAccessReads = value; }
        }

        public TagAccessData TagAccessDataSet
        {
            get { return _tagAccessData; }
            set { _tagAccessData = value; }
        }

        //Add by FJ for set select criteria rule
        public int CRITERIA_MAXIM
        {
            get { return global::RFID_Explorer.Properties.Settings.Default.MaximumPreSingulation; }
        }

        private SelectCriteria selectCriteria;

        public void Init(LakeChabotReader reader)
        {
            InitializeComponent();
            this.FormClosing += Form1_FormClosing;

            //clark 2011.5.9 Get this point to get Access Password in module's buffer.
            _reader = reader;

            //Add by FJ for API result
            Result Result = Result.OK;

            //Add by FJ for initial value
            COMBOBOX_DAT_MSK.Items.Add("0");
            COMBOBOX_DAT_MSK.Items.Add("1");
            COMBOBOX_DAT_MSK.SelectedIndex = 1;

            COMBOBOX_COLOR.Items.Add("Red");
            COMBOBOX_COLOR.Items.Add("Green");
            COMBOBOX_COLOR.Items.Add("Blue");
            COMBOBOX_COLOR.SelectedIndex = 0;

            COMBOBOX_BATTERY_ASSIST.Items.Add("Disabled");
            COMBOBOX_BATTERY_ASSIST.Items.Add("Enabled");
            COMBOBOX_BATTERY_ASSIST.SelectedIndex = 0;

            COMBOBOX_ANTENNA_SELECT.Items.Add("NF + FF");
            COMBOBOX_ANTENNA_SELECT.Items.Add("NF");
            COMBOBOX_ANTENNA_SELECT.Items.Add("FF");
            COMBOBOX_ANTENNA_SELECT.SelectedIndex = 0;

            //Add by FJ for set default disable status for access memory 
            //DAT1 & DAT0
            PANEL_DAT1.Enabled = false;
            PANEL_DAT0.Enabled = false;
            COMBOBOX_DAT_MSK.Enabled = false;
            BUTTON_ACCESS_MEMORY_GET.Enabled = false;
            BUTTON_ACCESS_MEMORY_SET.Enabled = false;
            //CONTROL
            GROUPBOX_LED.Enabled = false;
            GROUPBOX_BATTERY.Enabled = false;
            GROUPBOX_ANTENNA.Enabled = false;

            //Add by FJ for adjust the layout
            PANEL_DAT1.Location = PANEL_DAT0.Location = new System.Drawing.Point(82, 168);
            this.ClientSize = new System.Drawing.Size(620, 450);
            tabControl1.Location = new System.Drawing.Point(12, 12);
            tabControl1.Size = new System.Drawing.Size(600, 430);

            //Add by FJ for get Model name
            CB2_result_Major = rfid.Constants.Result.OK;
            CB2_result_Major = _reader.MacReadOemData((ushort)((int)enumOEM_ADDR.MODEL_NAME_MAIN), ref CB2_uiModelNameMAJOR);
            CB2_result_Sub = rfid.Constants.Result.OK;
            CB2_result_Sub = _reader.MacReadOemData((ushort)((int)enumOEM_ADDR.MODEL_NAME_SUB), ref CB2_uiModelNameSUB);

            //Add by FJ for set Antenna setting
            AntennaPortConfig PortConfig = new AntennaPortConfig();
            Result = LakeChabotReader.MANAGED_ACCESS.API_AntennaPortGetConfiguration(0, ref PortConfig);

            Original_Power_Level = PortConfig.powerLevel;
            Original_Physical_Port = PortConfig.physicalPort;
            //Add by FJ for implement recovery mechanism for CB-2 function, 2017/12/07
            Original_InventoryCycles = PortConfig.numberInventoryCycles;
            Original_dwellTime = PortConfig.dwellTime;
            //End by FJ for implement recovery mechanism for CB-2 function, 2017/12/07

            if (Result.OK != Result)
                MessageBox.Show("Read Reader Antenna Status Failed.");
            else
            {
                NUMERICUPDOWN_PHYSICAL_PORT.Value = PortConfig.physicalPort;
                if (CB2_uiModelNameMAJOR == 0x4D303658)//0x4D303658 = M06X
                {
                    NUMERICUPDOWN_POWER_LEVEL.Minimum = Source_Antenna.POWER_MINIMUM;
                    NUMERICUPDOWN_POWER_LEVEL.Maximum = 300;
                    NUMERICUPDOWN_POWER_LEVEL.Value = (PortConfig.powerLevel >= NUMERICUPDOWN_POWER_LEVEL.Maximum) ? NUMERICUPDOWN_POWER_LEVEL.Maximum : PortConfig.powerLevel;
                }
                else
                {
                    NUMERICUPDOWN_POWER_LEVEL.Minimum = Source_Antenna.POWER_MINIMUM;
                    NUMERICUPDOWN_POWER_LEVEL.Maximum = Source_Antenna.POWER_MAXIMUM;
                    NUMERICUPDOWN_POWER_LEVEL.Value = PortConfig.powerLevel;
                }
            }

            PortConfig.numberInventoryCycles = (UInt16)1;
            PortConfig.dwellTime = (UInt16)0;
            Result = LakeChabotReader.MANAGED_ACCESS.API_AntennaPortSetConfiguration(0, PortConfig);
            if (Result.OK != Result)
                MessageBox.Show("Set Antenna DwellTime/InventoryCycles Failed.");

            //Add by FJ for set Select Criteria rule
            rfid.Structures.SelectCriteria retrievedCriteria = new rfid.Structures.SelectCriteria();
            this.selectCriteria = new SelectCriteria();
            this.selectCriteria.pCriteria = new SelectCriterion[CRITERIA_MAXIM];
            Result = LakeChabotReader.MANAGED_ACCESS.API_l8K6CGetSelectCriteria(ref retrievedCriteria);
            if (Result.OK != Result)
                MessageBox.Show("Read Select Criteria Rule Failed.");
            else
            {
                // Copy over incoming criteria...
                this.selectCriteria.countCriteria = retrievedCriteria.countCriteria;

                // Ref copy since safe creation in managed lib now...
                for (int index = 0; index < retrievedCriteria.countCriteria; ++index)
                {
                    this.selectCriteria.pCriteria[index] = retrievedCriteria.pCriteria[index];
                }

                // Fill in ( or zero out if call > 1 ) remaining criteria
                for (uint index = selectCriteria.countCriteria; index < CRITERIA_MAXIM; ++index)
                {
                    this.selectCriteria.pCriteria[index] = new rfid.Structures.SelectCriterion();
                }
            }

            this.selectCriteria.countCriteria = (byte)1;
            this.selectCriteria.pCriteria[0].mask.offset = 48;
            this.selectCriteria.pCriteria[0].mask.bank = MemoryBank.TID;
            this.selectCriteria.pCriteria[0].mask.count = (byte)0;
            this.selectCriteria.pCriteria[0].action.target = Target.S2;
            this.selectCriteria.pCriteria[0].action.enableTruncate = (byte)0;
            this.selectCriteria.pCriteria[0].action.action = rfid.Constants.Action.DSLINVB_ASLINVA;
            Result = LakeChabotReader.MANAGED_ACCESS.API_l8K6CSetSelectCriteria(selectCriteria);
            if (Result.OK != Result)
                MessageBox.Show("Set Select Criteria Rule Failed.");

            //Mod by FJ for implement recovery mechanism for CB-2 function, 2017/12/07
            Result = LakeChabotReader.MANAGED_ACCESS.API_l8K6CGetCurrentSingulationAlgorithm(ref algorithm);
            if (Result.OK != Result)
                MessageBox.Show("Get Algorithm Failed.");

            switch (algorithm)
            {
                case SingulationAlgorithm.DYNAMICQ:
                    Source_QueryParms parmsMasterDQ = new Source_QueryParms();
                    Result = parmsMasterDQ.loadForAlgorithm
                        (
                            LakeChabotReader.MANAGED_ACCESS,
                            (uint)_reader.ReaderHandle,
                            SingulationAlgorithm.DYNAMICQ
                        );
                    if (Result.OK != Result)
                        MessageBox.Show("Read Algorithm Rule Failed.");

                    Source_SingulationParametersDynamicQ parmsDynamicQ = (Source_SingulationParametersDynamicQ)parmsMasterDQ.SingulationAlgorithmParameters;

                    Original_Selected = parmsMasterDQ.TagGroupSelected;
                    Original_Session = parmsMasterDQ.TagGroupSession;
                    Original_Target = parmsMasterDQ.TagGroupTarget;
                    Original_DQStartQValue = parmsDynamicQ.StartQValue;
                    Original_DQMinQValue = parmsDynamicQ.MinQValue;
                    Original_DQMaxQValue = parmsDynamicQ.MaxQValue;
                    Original_DQRetryCount = parmsDynamicQ.RetryCount;
                    Original_DQToggleTarget = parmsDynamicQ.ToggleTarget;
                    Original_DQThresholdMultiplier = parmsDynamicQ.ThresholdMultiplier;

                    break;

                case SingulationAlgorithm.FIXEDQ:
                    Source_QueryParms parmsMasterFQ = new Source_QueryParms();
                    Result = parmsMasterFQ.loadForAlgorithm
                    (
                        LakeChabotReader.MANAGED_ACCESS,
                        (uint)reader.ReaderHandle,
                        SingulationAlgorithm.FIXEDQ
                    );
                    if (Result.OK != Result)
                        MessageBox.Show("Read Algorithm Rule Failed.");

                    Source_SingulationParametersFixedQ parmsFixedQ = (Source_SingulationParametersFixedQ)parmsMasterFQ.SingulationAlgorithmParameters;

                    Original_Selected = parmsMasterFQ.TagGroupSelected;
                    Original_Session = parmsMasterFQ.TagGroupSession;
                    Original_Target = parmsMasterFQ.TagGroupTarget;
                    Original_FQQValue = parmsFixedQ.QValue;
                    Original_FQRetryCount = parmsFixedQ.RetryCount;
                    Original_FQToggleTarget = parmsFixedQ.ToggleTarget;
                    Original_FQRepeatUntilNoTags = parmsFixedQ.RepeatUntilNoTags;

                    break;
            }

            //Add by FJ for set Algorithm rule
            Source_QueryParms parmsMasterCB2 = new Source_QueryParms();
            Result = parmsMasterCB2.loadForAlgorithm
                                (
                                    LakeChabotReader.MANAGED_ACCESS,
                                    (uint)reader.ReaderHandle,
                                    SingulationAlgorithm.FIXEDQ
                                );
            if (Result.OK != Result)
                MessageBox.Show("Read Algorithm Rule Failed.");

            Source_SingulationParametersFixedQ parmsFixedQCB2 = (Source_SingulationParametersFixedQ)parmsMasterCB2.SingulationAlgorithmParameters;

            parmsMasterCB2.TagGroupSelected = Selected.SELECT_ALL;
            parmsMasterCB2.TagGroupSession = Session.S2;
            parmsMasterCB2.TagGroupTarget = SessionTarget.B;
            parmsFixedQCB2.QValue = (byte)0;
            parmsFixedQCB2.RetryCount = (byte)0;
            parmsFixedQCB2.ToggleTarget = (byte)0;
            parmsFixedQCB2.RepeatUntilNoTags = (byte)0;

            Result = parmsMasterCB2.store
                     (
                         LakeChabotReader.MANAGED_ACCESS,
                         reader.ReaderHandle
                     );
            if (Result.OK != Result)
                MessageBox.Show("Set Algorithm Rule Failed.");
        }


        public cB2TagAccess(RFID_Explorer.mainForm r_form, LakeChabotReader reader, TagAccessData r_tagAccessData)
        {
            Init(reader);

            _tagAccessData = r_tagAccessData;

            //retryCount = r_form.GetRetryCount();

            //Get Access Password in modeule's buffer.
            UInt32 Password = 0;
            if (Result.OK == reader.API_l8K6CTagGetAccessPassword(ref Password))
            {
                _tagAccessData.accessPassword = Password;
            }

        }


        public cB2TagAccess(LakeChabotReader reader)
        {
            Init(reader);
        }

        private void ValidateHexInput(object sender, KeyEventArgs e)
        {
            ValidateHexInput(sender);
        }

        private void ValidateHexInput(object sender, MouseEventArgs e)
        {
            ValidateHexInput(sender);
        }

        private void ValidateHexInput(object sender)
        {
            if (typeof(TextBox) == sender.GetType())
            {
                TextBox textBox = (TextBox)sender;
                string originalText = textBox.Text;
                int originalSelection = textBox.SelectionStart;
                Regex regEx = new Regex("[^0-9A-Fa-f]");
                string newString = regEx.Replace(textBox.Text, "").ToUpper();
                textBox.Text = newString;
                if (newString != originalText)
                {
                    textBox.SelectionStart = originalSelection;
                    textBox.ScrollToCaret();
                }
            }
            else if (typeof(ComboBox) == sender.GetType())
            {
                ComboBox comboBox = (ComboBox)sender;
                string originalText = comboBox.Text;
                int originalSelection = comboBox.SelectionStart;
                Regex regEx = new Regex("[^0-9A-Fa-f]");
                string newString = regEx.Replace(comboBox.Text, "").ToUpper();
                comboBox.Text = newString;
                if (newString != originalText)
                {
                    comboBox.SelectionStart = originalSelection;
                }
            }
        }

        private bool ValidateHex_uint(string input, string name, out uint value)
        {
            if (!uint.TryParse(input, NumberStyles.AllowHexSpecifier, null, out value))
            {
                MessageBox.Show(string.Format("{0} [{1}]", name, input),
                                "Tag Access Input Invalid",
                                MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }
            return true;
        }

        private bool ValidateHex_ushort(string input, string name, out ushort value)
        {
            if (!ushort.TryParse(input, NumberStyles.AllowHexSpecifier, null, out value))
            {
                MessageBox.Show(string.Format("{0} [{1}]", name, input),
                                "Tag Access Input Invalid",
                                MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }
            return true;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
        }

        //Add by FJ for go back initial value when change tab
        private void TabControl1_Click(object sender, EventArgs e)
        {
            switch (tabControl1.SelectedIndex)
            {
                case 1:
                    TEXTBOX_CONFIGURE_PC_EPC_CRC.Text = "";
                    TEXTBOX_CONFIGURE_TID_SN.Text = "";
                    Clean_CFG_DAT1_Text();
                    Clean_CFG_DAT0_Text();
                    COMBOBOX_DAT_MSK.SelectedIndex = 1;
                    PANEL_DAT1.Enabled = false;
                    PANEL_DAT0.Enabled = false;
                    PANEL_DAT0.Visible = false;
                    PANEL_DAT1.Visible = true;
                    COMBOBOX_DAT_MSK.Enabled = false;
                    BUTTON_ACCESS_MEMORY_GET.Enabled = false;
                    BUTTON_ACCESS_MEMORY_SET.Enabled = false;
                    TEXTBOX_CONFIGURE_STATUS.Text = "";
                    break;

                case 2:
                    TEXTBOX_CONTROL_PC_EPC_CRC.Text = "";
                    TEXTBOX_CONTROL_TID_SN.Text = "";
                    COMBOBOX_COLOR.SelectedIndex = 0;
                    COMBOBOX_BATTERY_ASSIST.SelectedIndex = 0;
                    COMBOBOX_ANTENNA_SELECT.SelectedIndex = 0;
                    TEXTBOX_VERSION_NUMBER.Text = "";
                    GROUPBOX_LED.Enabled = false;
                    GROUPBOX_BATTERY.Enabled = false;
                    GROUPBOX_ANTENNA.Enabled = false;
                    TEXTBOX_VERSION_NUMBER.Text = "";
                    TEXTBOX_CONTROL_STATUS.Text = "";
                    break;
            }
        }

        /*---------------ANTENNA SETTING TAB---------------*/
        //Add by FJ set antenna power & port
        private void Button_Antenna_Setting_Click(object sender, EventArgs e)
        {

            UInt16 Antenna_Power, Antenna_Port;
            AntennaPortConfig PortConfig = new AntennaPortConfig();
            Result Result = Result.OK;

            Antenna_Power = (UInt16)NUMERICUPDOWN_POWER_LEVEL.Value;
            Antenna_Port = (UInt16)NUMERICUPDOWN_PHYSICAL_PORT.Value;


            Result = LakeChabotReader.MANAGED_ACCESS.API_AntennaPortGetConfiguration(0, ref PortConfig);

            PortConfig.powerLevel = Antenna_Power;
            PortConfig.physicalPort = (byte)Antenna_Port;
            Result = LakeChabotReader.MANAGED_ACCESS.API_AntennaPortSetConfiguration(0, PortConfig);
            if (Result.OK != Result)
                MessageBox.Show("Set Failed.");
            else
                MessageBox.Show("Set Success.");

        }

        private void analysis_TagAccessPkt(ReadParms r_Parms, byte r_PerformFlags, byte[] bufMTII, byte[] bufMTIA)
        {
            int i, j;
            byte[] TagData = new Byte[128];
            byte[] Tag_EPC = new Byte[16];
            byte[] Tag_TID = new Byte[6];
            byte[] Tag_TID_XOR = new Byte[2];
            UInt32 Tag_First_XOR = 0;
            UInt32 Tag_Secnod_XOR = 0;
            UInt32 Tag_RESERVED_DAT = 0;
            UInt32 Tag_USER_VER = 0;
            UInt32 Tag_USER_KEY = 0;
            UInt32 Tag_USER_BAT = 0;
            UInt32 Tag_USER_ANT = 0;
            UInt32 Tag_USER_LED = 0;
            Tag_Infor.Tag_ACCESS_FLAG = false;
            Tag_Infor.Tag_PC_EPC_CRC = "";
            Tag_Infor.Tag_USER_KEY = "";
            Tag_Infor.Tag_RESERVED_MAPPED_DAT1 = "";
            Tag_Infor.Tag_RESERVED_MAPPED_DAT0 = "";
            Tag_Infor.Tag_USER_LED = "";
            Tag_Infor.Tag_USER_BAT = "";
            Tag_Infor.Tag_USER_ANT = "";
            Tag_Infor.Tag_USER_VER = "";
            Tag_Infor.Tag_Error = 0;

            Tag_Infor.Tag_ACCESS_FLAG = r_Parms.common.tagFlag;

            switch (r_Parms.readCmdParms.bank)
            {
                case MemoryBank.EPC:
                     j = 0;
                     for (i = dataEPCStart; i < dataEPCEnd; i++)
                     {
                         Tag_EPC[j] = bufMTII[i];
                         j++;
                     }
                     Tag_Infor.Tag_PC_EPC_CRC = BitConverter.ToString(Tag_EPC);
                break;

                case MemoryBank.TID:
                     Tag_Infor.Tag_Error = (UInt32)bufMTIA[rptFlags];
                     if (Tag_Infor.Tag_ACCESS_FLAG == true)
                     {
                         //EPC
                         j = 0;
                         for (i = dataEPCStart ; i < dataEPCEnd ; i++)
                         {
                             Tag_EPC[j] = bufMTII[i];
                             j++;
                         }
                         Tag_Infor.Tag_PC_EPC_CRC = BitConverter.ToString(Tag_EPC);

                         //TID
                         j = 0;
                         for (i = dataTIDStart; i < dataTIDEnd; i++)
                         {
                             Tag_TID[j] = bufMTIA[i];
                             if (j % 2 == 0)
                             {
                                 Tag_First_XOR = Tag_First_XOR ^ Tag_TID[j];
                             }
                             else
                             {
                                 Tag_Secnod_XOR = Tag_Secnod_XOR ^ Tag_TID[j];
                             }
                             j++;
                         }
                         Tag_Infor.Tag_TID = BitConverter.ToString(Tag_TID);
                         Tag_Infor.Tag_TID_XOR = System.Convert.ToString(Tag_First_XOR, 16) + "-" + System.Convert.ToString(Tag_Secnod_XOR, 16);
                         Tag_Infor.Tag_USER_KEYS = System.Convert.ToString(Tag_First_XOR, 16) + System.Convert.ToString(Tag_Secnod_XOR, 16);
                     }
                break;

                case MemoryBank.RESERVED:
                     Tag_Infor.Tag_Error = (UInt32)bufMTIA[rptFlags];
                     if (Tag_Infor.Tag_ACCESS_FLAG == true)
                     {
                         for (i = dataCB2Start ; i < dataCB2End ; i++)
                         {
                             Tag_RESERVED_DAT = bufMTIA[i];
                             if (r_PerformFlags == 0) // 0:CFG_DAT1、1:CFG_DAT0
                             {
                                 Tag_Infor.Tag_RESERVED_MAPPED_DAT1 += Convert.ToString(Tag_RESERVED_DAT, 2).PadLeft(8, '0');
                             }
                             else
                             {
                                 Tag_Infor.Tag_RESERVED_MAPPED_DAT0 += Convert.ToString(Tag_RESERVED_DAT, 2).PadLeft(8, '0');
                             }
                         }
                     }
                break;

                case MemoryBank.USER:
                     Tag_Infor.Tag_Error = (UInt32)bufMTIA[rptFlags];
                     if (Tag_Infor.Tag_ACCESS_FLAG == true)
                     {
                         for (i = dataCB2Start ; i < dataCB2End ; i++)
                         {
                             if (r_PerformFlags == 1)//1：Get "USER_KEY"、2：Get "USER_LED"、3：Get "USER_BAT"、4：Get "USER_ANT"、5：Get "USER_VER"
                             {
                                 Tag_USER_KEY = bufMTIA[i];
                                 Tag_Infor.Tag_USER_KEY += Convert.ToString(Tag_USER_KEY, 2).PadLeft(8, '0');
                             }
                             else if (r_PerformFlags == 2)
                             {
                                 Tag_USER_LED = bufMTIA[i];
                                 Tag_Infor.Tag_USER_LED += Convert.ToString(Tag_USER_LED, 2).PadLeft(8, '0');
                             }
                             else if (r_PerformFlags == 3)
                             {
                                 Tag_USER_BAT = bufMTIA[i];
                                 Tag_Infor.Tag_USER_BAT += Convert.ToString(Tag_USER_BAT, 2).PadLeft(8, '0');
                             }
                             else if (r_PerformFlags == 4)
                             {
                                 Tag_USER_ANT = bufMTIA[i];
                                 Tag_Infor.Tag_USER_ANT += Convert.ToString(Tag_USER_ANT, 2).PadLeft(8, '0');
                             }
                             else 
                             {
                                 Tag_USER_VER = bufMTIA[i];
                                 Tag_Infor.Tag_USER_VER += Convert.ToString(Tag_USER_VER, 2).PadLeft(8, '0');
                             }
                         }
                     }
                break;
            }
        }


        /*---------------CONFIGURE FUNCTION TAB---------------*/
        //Add by FJ for inventory function when click inventory button
        private void Button_Configure_Inventory_Click(object sender, EventArgs e)
        {
            byte flags = 0;
            BUTTON_CONFIGURE_INVENTORY.Enabled = false;

            //reset
            Clean_Select_Critrtia_Mask();
            TEXTBOX_CONFIGURE_PC_EPC_CRC.Text = "";
            TEXTBOX_CONFIGURE_TID_SN.Text = "";
            Clear_Access_Memory_Status();

            ReadParms parameters = new ReadParms();
            //Banl = 2
            parameters.readCmdParms.bank = MemoryBank.TID;
            //offset = 3
            parameters.readCmdParms.offset = 3;
            //Count = 3
            parameters.readCmdParms.count = 3;
            //RetryCount = 0
            parameters.common.strcTagFlag.RetryCount = retryCount;
            //PerformSelect = 1
            parameters.common.strcTagFlag.SelectOpsFlag = 1;
            //PerformPostMatch = 0
            parameters.common.strcTagFlag.PostMatchFlag = 0;

            parameters.accessPassword = _tagAccessData.accessPassword;
            parameters.common.callback = new rfid.CallbackDelegate(this._reader.MyCallback);
            parameters.common.OpMode = RadioOperationMode.NONCONTINUOUS;

            parameters.common.tagFlag = false;
            Array.Clear(parameters.common.bufMTII, 0, parameters.common.bufMTII.Length);
            Array.Clear(parameters.common.bufMTIA, 0, parameters.common.bufMTIA.Length);
            
            if (Result.OK != LakeChabotReader.MANAGED_ACCESS.API_l8K6CTagRead(parameters, flags))
            {
                Textbox_Configure_Status_Prinf("Inventory Command Failed.");
            }
            else
            {
                analysis_TagAccessPkt(parameters, flags, parameters.common.bufMTII, parameters.common.bufMTIA);
                if (Tag_Infor.Tag_ACCESS_FLAG == true)
                {
                    Textbox_Configure_Status_Prinf("Get EPC/TID Data Success.");
                    TEXTBOX_CONFIGURE_PC_EPC_CRC.Text = Tag_Infor.Tag_PC_EPC_CRC;
                    TEXTBOX_CONFIGURE_TID_SN.Text = Tag_Infor.Tag_TID;
                    Button_CFG_DAT_Get_Click(sender, e);
                    BUTTON_CONFIGURE_INVENTORY.Enabled = true;
                }
                else
                {
                    Textbox_Configure_Status_Prinf("Get EPC/TID Data Failed.");
                    Clear_Access_Memory_Status();
                    BUTTON_CONFIGURE_INVENTORY.Enabled = true;
                }
            }
            this._reader.CB2_ProcessQueuedPackets(); //Add by FJ for fixing packet parsing mechanism for operating CB-2, 2017/12/07
        }

        //Add by FJ for get CFG_DAT when click get button
        private void Button_CFG_DAT_Get_Click(object sender, EventArgs e)
        {
            byte flags;
            string TempHexValue = "";
            BUTTON_ACCESS_MEMORY_GET.Enabled = false;

            //Clear_Access_Memory_Status();
            Edit_Select_Critrtia_Mask(Tag_Infor.Tag_TID);

            ReadParms parameters = new ReadParms();
            //Bank = 0
            parameters.readCmdParms.bank = MemoryBank.RESERVED;
            //Count = 1
            parameters.readCmdParms.count = 1;
            //RetryCount
            parameters.common.strcTagFlag.RetryCount = retryCount;
            //PerformSelect
            parameters.common.strcTagFlag.SelectOpsFlag = 1;
            //PerformPosMatch
            parameters.common.strcTagFlag.PostMatchFlag = 0;

            parameters.accessPassword = _tagAccessData.accessPassword;
            parameters.common.callback = new rfid.CallbackDelegate(this._reader.MyCallback);
            parameters.common.OpMode = RadioOperationMode.NONCONTINUOUS;

            parameters.common.tagFlag = false;
            Array.Clear(parameters.common.bufMTII, 0, parameters.common.bufMTII.Length);
            Array.Clear(parameters.common.bufMTIA, 0, parameters.common.bufMTIA.Length);

            switch (COMBOBOX_DAT_MSK.SelectedIndex)
            {
                case 1:    //CFG_DAT1
                    //Offset = 10,0x0A
                    parameters.readCmdParms.offset = 10;
                    flags = 0;

                    if (Result.OK != LakeChabotReader.MANAGED_ACCESS.API_l8K6CTagRead(parameters, flags))
                    {
                        Textbox_Configure_Status_Prinf("Read 'CFG_DAT1' Data Command Failed.");
                    }
                    else
                    {
                        analysis_TagAccessPkt(parameters, flags, parameters.common.bufMTII, parameters.common.bufMTIA);
                        if (Tag_Infor.Tag_Error == 1 || Tag_Infor.Tag_Error == 2 || Tag_Infor.Tag_Error == 3)
                        {
                            Textbox_Configure_Status_Prinf("Get 'CFG_DAT1' Data Failed - Non CB-2 Tag.");
                            Tag_Infor.Tag_ACCESS_FLAG = false;
                            Clear_Access_Memory_Status();
                        }
                        else
                        {
                            if (Tag_Infor.Tag_ACCESS_FLAG == true)
                            {
                                Textbox_Configure_Status_Prinf("Get 'CFG_DAT1' Data Success.");
                                Clear_Access_Memory_Status();
                                char[] TempArray = Tag_Infor.Tag_RESERVED_MAPPED_DAT1.ToCharArray();
                                for (int i = 0; i < TempArray.Length; i++)
                                {
                                    TempHexValue += TempArray[i];
                                    switch (i)
                                    {
                                        case 9:
                                            TEXTBOX_WWU.Text = System.Convert.ToString(TempArray[i]);
                                            TEXTBOX_WWU.Enabled = false;
                                            break;

                                        case 10:
                                            TEXTBOX_BPL_EN.Text = System.Convert.ToString(TempArray[i]);
                                            TEXTBOX_BPL_EN.Enabled = false;
                                            break;

                                        case 11:
                                            TEXTBOX_QT_SR.Text = System.Convert.ToString(TempArray[i]);
                                            TEXTBOX_QT_SR.Enabled = false;
                                            break;

                                        case 12:
                                            TEXTBOX_QT_MEM.Text = System.Convert.ToString(TempArray[i]);
                                            TEXTBOX_QT_MEM.Enabled = false;
                                            break;

                                        case 13:
                                            TEXTBOX_DCI_RF_EN.Text = System.Convert.ToString(TempArray[i]);
                                            TEXTBOX_DCI_RF_EN.Enabled = false;
                                            break;

                                        case 14:
                                            TEXTBOX_RF2_DIS.Text = System.Convert.ToString(TempArray[i]);
                                            TEXTBOX_RF2_DIS.Enabled = false;
                                            break;

                                        case 15:
                                            TEXTBOX_RF1_DIS.Text = System.Convert.ToString(TempArray[i]);
                                            TEXTBOX_RF1_DIS.Enabled = false;
                                            break;

                                        default:
                                            TEXTBOX_RFU.Text += System.Convert.ToString(TempArray[i]);
                                            break;
                                    }
                                }
                                BUTTON_ACCESS_MEMORY_GET.Enabled = true;
                                PANEL_DAT0.Visible = false;
                                PANEL_DAT1.Visible = true;
                            }
                            else
                            {
                                Textbox_Configure_Status_Prinf("Get 'CFG_DAT1' Data Failed.");
                                TEXTBOX_CONFIGURE_PC_EPC_CRC.Text = "";
                                TEXTBOX_CONFIGURE_TID_SN.Text = "";
                                Clear_Access_Memory_Status();
                            }
                        }
                    }
                    break;

                case 0:     //CFG_DAT0
                    //Offset = 4,0x04
                    parameters.readCmdParms.offset = 4;
                    flags = 1;

                    if (Result.OK != LakeChabotReader.MANAGED_ACCESS.API_l8K6CTagRead(parameters, flags))
                    {
                        Textbox_Configure_Status_Prinf("Read 'CFG_DAT0' Data Command Failed.");
                    }
                    else
                    {
                        analysis_TagAccessPkt(parameters, flags, parameters.common.bufMTII, parameters.common.bufMTIA);
                        if (Tag_Infor.Tag_Error == 1 || Tag_Infor.Tag_Error == 2 || Tag_Infor.Tag_Error == 3)
                        {
                            Textbox_Configure_Status_Prinf("Get 'CFG_DAT0' Data Failed - Non CB-2 Tag.");
                            Tag_Infor.Tag_ACCESS_FLAG = false;
                            Clear_Access_Memory_Status();
                        }
                        else
                        {
                            if (Tag_Infor.Tag_ACCESS_FLAG == true)
                            {
                                Textbox_Configure_Status_Prinf("Get 'CFG_DAT0' Data Success.");
                                Clear_Access_Memory_Status();
                                char[] TempArray = Tag_Infor.Tag_RESERVED_MAPPED_DAT0.ToCharArray();
                                for (int i = 0; i < TempArray.Length; i++)
                                {
                                    TempHexValue += TempArray[i];
                                    switch (i)
                                    {
                                        case 0:
                                        case 1:
                                            TEXTBOX_LOCK_KILL.Text += System.Convert.ToString(TempArray[i]);
                                            TEXTBOX_LOCK_KILL.Enabled = false;
                                            break;

                                        case 2:
                                        case 3:
                                            TEXTBOX_LOCK_ACCE.Text += System.Convert.ToString(TempArray[i]);
                                            TEXTBOX_LOCK_ACCE.Enabled = false;
                                            break;

                                        case 4:
                                        case 5:
                                            TEXTBOX_LOCK_EPC.Text += System.Convert.ToString(TempArray[i]);
                                            TEXTBOX_LOCK_EPC.Enabled = false;
                                            break;

                                        case 6:
                                        case 7:
                                            TEXTBOX_LOCK_USER.Text += System.Convert.ToString(TempArray[i]);
                                            TEXTBOX_LOCK_USER.Enabled = false;
                                            break;

                                        case 8:
                                        case 9:
                                        case 10:
                                        case 11:
                                        case 12:
                                            TEXTBOX_BLOCK_PERMALOCK.Text += System.Convert.ToString(TempArray[i]);
                                            TEXTBOX_BLOCK_PERMALOCK.Enabled = false;
                                            break;

                                        case 13:
                                            TEXTBOX_KILL.Text = System.Convert.ToString(TempArray[i]);
                                            TEXTBOX_KILL.Enabled = false;
                                            break;

                                        case 14:
                                        case 15:
                                            TEXTBOX_CONFIG.Text += System.Convert.ToString(TempArray[i]);
                                            break;
                                    }
                                }
                                BUTTON_ACCESS_MEMORY_GET.Enabled = true;
                                PANEL_DAT1.Visible = false;
                                PANEL_DAT0.Visible = true;
                            }
                            else
                            {
                                Textbox_Configure_Status_Prinf("Get 'CFG_DAT0' Data Failed.");
                                TEXTBOX_CONFIGURE_PC_EPC_CRC.Text = "";
                                TEXTBOX_CONFIGURE_TID_SN.Text = "";
                                Clear_Access_Memory_Status();
                            }
                        }
                    }
                    break;
            }
            Clean_Select_Critrtia_Mask();
            this._reader.CB2_ProcessQueuedPackets(); //Add FJ for fixing packet parsing mechanism for operating CB-2, 2017/12/07
        }

        //Add by FJ for set CFG_DAT whem click set button
        private void Button_CFG_DAT_Set_Click(object sender, EventArgs e)
        {
            string DAT1_TempValue, DAT0_TempValue;
            string MSK1_TempValue, MSK0_TempValue;
            Result Result = Result.OK;

            //Get CFG_KEYS
            byte flags = 0;
            string TempHexValue = "";

            BUTTON_ACCESS_MEMORY_SET.Enabled = false;
            Edit_Select_Critrtia_Mask(Tag_Infor.Tag_TID);

            switch (COMBOBOX_DAT_MSK.SelectedIndex)
            {
                case 1:    //CFG_DAT1

                    if (string.IsNullOrEmpty(TEXTBOX_WWU.Text) ||
                        string.IsNullOrEmpty(TEXTBOX_BPL_EN.Text) ||
                        string.IsNullOrEmpty(TEXTBOX_QT_SR.Text) ||
                        string.IsNullOrEmpty(TEXTBOX_QT_MEM.Text) ||
                        string.IsNullOrEmpty(TEXTBOX_DCI_RF_EN.Text) ||
                        string.IsNullOrEmpty(TEXTBOX_RF2_DIS.Text) ||
                        string.IsNullOrEmpty(TEXTBOX_RF1_DIS.Text))
                    {
                        MessageBox.Show("Please Enter Value.");
                    }
                    else
                    {
                        DAT1_TempValue = TEXTBOX_RFU.Text;
                        DAT1_TempValue += TEXTBOX_WWU.Text;
                        DAT1_TempValue += TEXTBOX_BPL_EN.Text;
                        DAT1_TempValue += TEXTBOX_QT_SR.Text;
                        DAT1_TempValue += TEXTBOX_QT_MEM.Text;
                        DAT1_TempValue += TEXTBOX_DCI_RF_EN.Text;
                        DAT1_TempValue += TEXTBOX_RF2_DIS.Text;
                        DAT1_TempValue += TEXTBOX_RF1_DIS.Text;
                        TempHexValue = DAT1_TempValue;

                        MSK1_TempValue = "000000000";
                        MSK1_TempValue += (CHECKBOX_WWU.Checked == true) ? "1" : "0";
                        MSK1_TempValue += (CHECKBOX_BPL_EN.Checked == true) ? "1" : "0";
                        MSK1_TempValue += (CHECKBOX_QT_SR.Checked == true) ? "1" : "0";
                        MSK1_TempValue += (CHECKBOX_QT_MEM.Checked == true) ? "1" : "0";
                        MSK1_TempValue += (CHECKBOX_DCI_RF_EN.Checked == true) ? "1" : "0";
                        MSK1_TempValue += (CHECKBOX_RF2_DIS.Checked == true) ? "1" : "0";
                        MSK1_TempValue += (CHECKBOX_RF1_DIS.Checked == true) ? "1" : "0";

                        //Write Data
                        WriteParms parameters_DAT1_MSK1 = new WriteParms();
                        WriteSequentialParms writeParameters_DAT1_MSK1 = new WriteSequentialParms();
                        writeParameters_DAT1_MSK1.bank = MemoryBank.USER;
                        writeParameters_DAT1_MSK1.pData = new ushort[2];
                        writeParameters_DAT1_MSK1.pData[1] = 0;
                        writeParameters_DAT1_MSK1.count = 1;
                        parameters_DAT1_MSK1.accessPassword = 0;
                        parameters_DAT1_MSK1.writeParms = writeParameters_DAT1_MSK1;
                        parameters_DAT1_MSK1.writeType = WriteType.SEQUENTIAL;
                        parameters_DAT1_MSK1.common.callback = new rfid.CallbackDelegate(this._reader.MyCallback);

                        //clark 2011.4.25 Set tag access flag to inventory structure
                        parameters_DAT1_MSK1.common.strcTagFlag.RetryCount = retryCount;
                        parameters_DAT1_MSK1.common.strcTagFlag.PostMatchFlag = 0;
                        parameters_DAT1_MSK1.common.strcTagFlag.SelectOpsFlag = 1;
                        parameters_DAT1_MSK1.common.OpMode = RadioOperationMode.NONCONTINUOUS;

                        parameters_DAT1_MSK1.common.tagFlag = false;
                        Array.Clear(parameters_DAT1_MSK1.common.bufMTII, 0, parameters_DAT1_MSK1.common.bufMTII.Length);
                        Array.Clear(parameters_DAT1_MSK1.common.bufMTIA, 0, parameters_DAT1_MSK1.common.bufMTIA.Length);

                        //CFG_DAT1
                        writeParameters_DAT1_MSK1.offset = 134;  //86h
                        writeParameters_DAT1_MSK1.pData[0] = Convert.ToUInt16(Convert.ToInt32(DAT1_TempValue, 2));

                        Result = LakeChabotReader.MANAGED_ACCESS.API_l8K6CTagWrite(parameters_DAT1_MSK1, flags);
                        Tag_Infor.Tag_ACCESS_FLAG = parameters_DAT1_MSK1.common.tagFlag;

                        //if (Result.OK != LakeChabotReader.MANAGED_ACCESS.API_CB2_TAG_Write(parameters_DAT1_MSK1, flags, Tag_Infor))
                        if (Result.OK != Result)
                            Textbox_Configure_Status_Prinf("Set 'DAT1/MSK1' Data Command Failed - CFG_DAT1.");
                        if (Tag_Infor.Tag_ACCESS_FLAG != true)
                        {
                            Textbox_Configure_Status_Prinf("Set 'DAT1/MSK1' Data Failed - CFG_DAT1.");
                            TEXTBOX_CONFIGURE_PC_EPC_CRC.Text = "";
                            TEXTBOX_CONFIGURE_TID_SN.Text = "";
                            Clear_Access_Memory_Status();
                            break;
                        }

                        //CFG_MSK1
                        parameters_DAT1_MSK1.common.tagFlag = false;
                        Array.Clear(parameters_DAT1_MSK1.common.bufMTII, 0, parameters_DAT1_MSK1.common.bufMTII.Length);
                        Array.Clear(parameters_DAT1_MSK1.common.bufMTIA, 0, parameters_DAT1_MSK1.common.bufMTIA.Length);

                        writeParameters_DAT1_MSK1.offset = 133;  //85h
                        writeParameters_DAT1_MSK1.pData[0] = Convert.ToUInt16(Convert.ToInt32(MSK1_TempValue, 2));

                        Result = LakeChabotReader.MANAGED_ACCESS.API_l8K6CTagWrite(parameters_DAT1_MSK1, flags);
                        Tag_Infor.Tag_ACCESS_FLAG = parameters_DAT1_MSK1.common.tagFlag;

                        //if (Result.OK != LakeChabotReader.MANAGED_ACCESS.API_CB2_TAG_Write(parameters_DAT1_MSK1, flags, Tag_Infor))
                        if (Result.OK != Result)
                            Textbox_Configure_Status_Prinf("Set 'DAT1/MSK1' Data Command Failed - CFG_MSK1.");
                        if (Tag_Infor.Tag_ACCESS_FLAG != true)
                        {
                            Textbox_Configure_Status_Prinf("Set 'DAT1/MSK1' Data Failed - CFG_MSK1.");
                            TEXTBOX_CONFIGURE_PC_EPC_CRC.Text = "";
                            TEXTBOX_CONFIGURE_TID_SN.Text = "";
                            Clear_Access_Memory_Status();
                            break;
                        }

                        //CFG_KEYS
                        if (Convert.ToInt32(Tag_Infor.Tag_USER_KEYS.ToUpper(), 16) == 0)  //0x0000
                            Tag_Infor.Tag_USER_KEYS = "5a5a";
                        if (Convert.ToInt32(Tag_Infor.Tag_USER_KEYS.ToUpper(), 16) == 65535)  //0xFFFF
                            Tag_Infor.Tag_USER_KEYS = "a5a5";

                        parameters_DAT1_MSK1.common.tagFlag = false;
                        Array.Clear(parameters_DAT1_MSK1.common.bufMTII, 0, parameters_DAT1_MSK1.common.bufMTII.Length);
                        Array.Clear(parameters_DAT1_MSK1.common.bufMTIA, 0, parameters_DAT1_MSK1.common.bufMTIA.Length);

                        writeParameters_DAT1_MSK1.offset = 135;  //87h
                        writeParameters_DAT1_MSK1.pData[0] = Convert.ToUInt16(Convert.ToInt32(Tag_Infor.Tag_USER_KEYS.ToUpper(), 16));

                        Result = LakeChabotReader.MANAGED_ACCESS.API_l8K6CTagWrite(parameters_DAT1_MSK1, flags);
                        Tag_Infor.Tag_ACCESS_FLAG = parameters_DAT1_MSK1.common.tagFlag;

                        //if (Result.OK != LakeChabotReader.MANAGED_ACCESS.API_CB2_TAG_Write(parameters_DAT1_MSK1, flags, Tag_Infor))
                        if (Result.OK != Result)
                            Textbox_Configure_Status_Prinf("Set 'DAT1/MSK1' Data Command Failed - CFG_KEYS.");
                        if (Tag_Infor.Tag_ACCESS_FLAG != true)
                        {
                            Textbox_Configure_Status_Prinf("Set 'DAT1/MSK1' Data Failed - CFG_KEYS.");
                            TEXTBOX_CONFIGURE_PC_EPC_CRC.Text = "";
                            TEXTBOX_CONFIGURE_TID_SN.Text = "";
                            Clear_Access_Memory_Status();
                            break;
                        }

                        //Chesk USER_CFG_KEYS，Value = XOR-TID(in progress)、0x0000(Successful)、0xFFFF(Failed)
                        Thread.Sleep(2000);
                        flags = 1; //1: Get "USER_KEY"
                        ReadParms parameters_Check = new ReadParms();
                        parameters_Check.readCmdParms.bank = MemoryBank.USER;
                        parameters_Check.readCmdParms.offset = 135;  //87h
                        parameters_Check.readCmdParms.count = 1;
                        parameters_Check.common.strcTagFlag.RetryCount = retryCount;
                        parameters_Check.common.strcTagFlag.SelectOpsFlag = 1;
                        parameters_Check.common.strcTagFlag.PostMatchFlag = 0;
                        parameters_Check.accessPassword = _tagAccessData.accessPassword;
                        parameters_Check.common.callback = new rfid.CallbackDelegate(this._reader.MyCallback);
                        parameters_Check.common.OpMode = RadioOperationMode.NONCONTINUOUS;

                        parameters_Check.common.tagFlag = false;
                        Array.Clear(parameters_Check.common.bufMTII, 0, parameters_Check.common.bufMTII.Length);
                        Array.Clear(parameters_Check.common.bufMTIA, 0, parameters_Check.common.bufMTIA.Length);

                        Result = LakeChabotReader.MANAGED_ACCESS.API_l8K6CTagRead(parameters_Check, flags);

                        //if (Result.OK != LakeChabotReader.MANAGED_ACCESS.CB2_TAG_READ(parameters_Check, flags, Tag_Infor))
                        if (Result.OK != Result)
                            Textbox_Configure_Status_Prinf("Read Check Value Command Failed.");

                        analysis_TagAccessPkt(parameters_Check, flags, parameters_Check.common.bufMTII, parameters_Check.common.bufMTIA);

                        if (Tag_Infor.Tag_ACCESS_FLAG == true)
                        {
                            if (Convert.ToInt32(Tag_Infor.Tag_USER_KEY, 2) == 0)
                                Textbox_Configure_Status_Prinf("Set 'CFG_DAT1' Data Success.");
                            else if (Convert.ToInt32(Tag_Infor.Tag_USER_KEY, 2) == 65535)
                                Textbox_Configure_Status_Prinf("Set 'CFG_DAT1' Data Failed.");
                            else
                                Textbox_Configure_Status_Prinf("Set 'CFG_DAT1' Data In Progress.");

                            BUTTON_ACCESS_MEMORY_SET.Enabled = true;
                        }
                        else
                        {
                            Textbox_Configure_Status_Prinf("Set 'CFG_DAT1' Data Completed, But Check Status Failed.");
                            BUTTON_ACCESS_MEMORY_SET.Enabled = true;
                        }
                    }

                    break;

                case 0:     //CFG_DAT0

                    if (string.IsNullOrEmpty(TEXTBOX_LOCK_KILL.Text) ||
                        string.IsNullOrEmpty(TEXTBOX_LOCK_ACCE.Text) ||
                        string.IsNullOrEmpty(TEXTBOX_LOCK_EPC.Text) ||
                        string.IsNullOrEmpty(TEXTBOX_LOCK_USER.Text) ||
                        string.IsNullOrEmpty(TEXTBOX_BLOCK_PERMALOCK.Text) ||
                        string.IsNullOrEmpty(TEXTBOX_KILL.Text))
                    {
                        MessageBox.Show("Please Enter Value.");
                    }
                    else
                    {
                        DAT0_TempValue = TEXTBOX_LOCK_KILL.Text;
                        DAT0_TempValue += TEXTBOX_LOCK_ACCE.Text;
                        DAT0_TempValue += TEXTBOX_LOCK_EPC.Text;
                        DAT0_TempValue += TEXTBOX_LOCK_USER.Text;
                        DAT0_TempValue += TEXTBOX_BLOCK_PERMALOCK.Text;
                        DAT0_TempValue += TEXTBOX_KILL.Text;
                        DAT0_TempValue += TEXTBOX_CONFIG.Text;
                        TempHexValue = DAT0_TempValue;

                        MSK0_TempValue = (CHECKBOX_LOCK_KILL.Checked == true) ? "11" : "00";
                        MSK0_TempValue += (CHECKBOX_LOCK_ACCE.Checked == true) ? "11" : "00";
                        MSK0_TempValue += (CHECKBOX_LOCK_EPC.Checked == true) ? "11" : "00";
                        MSK0_TempValue += (CHECKBOX_LOCK_USER.Checked == true) ? "11" : "00";
                        MSK0_TempValue += (CHECKBOX_BLOCK_PERMALOCK.Checked == true) ? "11111" : "00000";
                        MSK0_TempValue += (CHECKBOX_KILL.Checked == true) ? "1" : "0";
                        MSK0_TempValue += "00";

                        //Write Data
                        WriteParms parameters_DAT0_MSK0 = new WriteParms();
                        WriteSequentialParms writeParameters_DAT0_MSK0 = new WriteSequentialParms();
                        writeParameters_DAT0_MSK0.bank = MemoryBank.USER;
                        writeParameters_DAT0_MSK0.pData = new ushort[2];
                        writeParameters_DAT0_MSK0.pData[1] = 0;
                        writeParameters_DAT0_MSK0.count = 1;
                        parameters_DAT0_MSK0.accessPassword = 0;
                        parameters_DAT0_MSK0.writeParms = writeParameters_DAT0_MSK0;
                        parameters_DAT0_MSK0.writeType = WriteType.SEQUENTIAL;
                        parameters_DAT0_MSK0.common.callback = new rfid.CallbackDelegate(this._reader.MyCallback);

                        //clark 2011.4.25 Set tag access flag to inventory structure
                        parameters_DAT0_MSK0.common.strcTagFlag.RetryCount = retryCount;
                        parameters_DAT0_MSK0.common.strcTagFlag.PostMatchFlag = 0;
                        parameters_DAT0_MSK0.common.strcTagFlag.SelectOpsFlag = 1;
                        parameters_DAT0_MSK0.common.OpMode = RadioOperationMode.NONCONTINUOUS;

                        parameters_DAT0_MSK0.common.tagFlag = false;
                        Array.Clear(parameters_DAT0_MSK0.common.bufMTII, 0, parameters_DAT0_MSK0.common.bufMTII.Length);
                        Array.Clear(parameters_DAT0_MSK0.common.bufMTIA, 0, parameters_DAT0_MSK0.common.bufMTIA.Length);

                        //CFG_DAT0
                        writeParameters_DAT0_MSK0.offset = 132;  //84h
                        writeParameters_DAT0_MSK0.pData[0] = Convert.ToUInt16(Convert.ToInt32(DAT0_TempValue, 2));

                        Result = LakeChabotReader.MANAGED_ACCESS.API_l8K6CTagWrite(parameters_DAT0_MSK0, flags);
                        Tag_Infor.Tag_ACCESS_FLAG = parameters_DAT0_MSK0.common.tagFlag;

                        //if (Result.OK != LakeChabotReader.MANAGED_ACCESS.API_CB2_TAG_Write(parameters_DAT0_MSK0, flags, Tag_Infor))
                        if (Result.OK != Result)
                            Textbox_Configure_Status_Prinf("Set 'DAT0/MSK0' Data Command Failed - CFG_DAT0.");
                        if (Tag_Infor.Tag_ACCESS_FLAG != true)
                        {
                            Textbox_Configure_Status_Prinf("Set 'DAT0/MSK0' Data Failed - CFG_DAT0.");
                            TEXTBOX_CONFIGURE_PC_EPC_CRC.Text = "";
                            TEXTBOX_CONFIGURE_TID_SN.Text = "";
                            Clear_Access_Memory_Status();
                            break;
                        }

                        //CFG_MSK0
                        parameters_DAT0_MSK0.common.tagFlag = false;
                        Array.Clear(parameters_DAT0_MSK0.common.bufMTII, 0, parameters_DAT0_MSK0.common.bufMTII.Length);
                        Array.Clear(parameters_DAT0_MSK0.common.bufMTIA, 0, parameters_DAT0_MSK0.common.bufMTIA.Length);

                        writeParameters_DAT0_MSK0.offset = 131;  //83h
                        writeParameters_DAT0_MSK0.pData[0] = Convert.ToUInt16(Convert.ToInt32(MSK0_TempValue, 2));
                        
                        Result = LakeChabotReader.MANAGED_ACCESS.API_l8K6CTagWrite(parameters_DAT0_MSK0, flags);
                        Tag_Infor.Tag_ACCESS_FLAG = parameters_DAT0_MSK0.common.tagFlag;

                        //if (Result.OK != LakeChabotReader.MANAGED_ACCESS.API_CB2_TAG_Write(parameters_DAT0_MSK0, flags, Tag_Infor))
                        if (Result.OK != Result)
                            Textbox_Configure_Status_Prinf("Set 'DAT0/MSK0' Data Command Failed - CFG_MSK0.");
                        if (Tag_Infor.Tag_ACCESS_FLAG != true)
                        {
                            Textbox_Configure_Status_Prinf("Set 'DAT0/MSK0' Data Failed - CFG_MSK0.");
                            TEXTBOX_CONFIGURE_PC_EPC_CRC.Text = "";
                            TEXTBOX_CONFIGURE_TID_SN.Text = "";
                            Clear_Access_Memory_Status();
                            break;
                        }

                        //CFG_KEYS
                        if (Convert.ToInt32(Tag_Infor.Tag_USER_KEYS.ToUpper(), 16) == 0)  //0x0000
                            Tag_Infor.Tag_USER_KEYS = "5a5a";
                        if (Convert.ToInt32(Tag_Infor.Tag_USER_KEYS.ToUpper(), 16) == 65535)  //0xFFFF
                            Tag_Infor.Tag_USER_KEYS = "a5a5";

                        parameters_DAT0_MSK0.common.tagFlag = false;
                        Array.Clear(parameters_DAT0_MSK0.common.bufMTII, 0, parameters_DAT0_MSK0.common.bufMTII.Length);
                        Array.Clear(parameters_DAT0_MSK0.common.bufMTIA, 0, parameters_DAT0_MSK0.common.bufMTIA.Length);

                        writeParameters_DAT0_MSK0.offset = 135;  //87h
                        writeParameters_DAT0_MSK0.pData[0] = Convert.ToUInt16(Convert.ToInt32(Tag_Infor.Tag_USER_KEYS.ToUpper(), 16));

                        Result = LakeChabotReader.MANAGED_ACCESS.API_l8K6CTagWrite(parameters_DAT0_MSK0, flags);
                        Tag_Infor.Tag_ACCESS_FLAG = parameters_DAT0_MSK0.common.tagFlag;

                        //if (Result.OK != LakeChabotReader.MANAGED_ACCESS.API_CB2_TAG_Write(parameters_DAT0_MSK0, flags, Tag_Infor))
                        if (Result.OK != Result)
                            Textbox_Configure_Status_Prinf("Set 'DAT0/MSK0' Data Command Failed - CFG_KEYS.");
                        if (Tag_Infor.Tag_ACCESS_FLAG != true)
                        {
                            Textbox_Configure_Status_Prinf("Set 'DAT0/MSK0' Data Failed - CFG_KEYS.");
                            TEXTBOX_CONFIGURE_PC_EPC_CRC.Text = "";
                            TEXTBOX_CONFIGURE_TID_SN.Text = "";
                            Clear_Access_Memory_Status();
                            break;
                        }

                        //Chesk USER_CFG_KEYS，Value = XOR-TID(in progress)、0x0000(Successful)、0xFFFF(Failed)
                        Thread.Sleep(2000);
                        flags = 1; //1: Get "USER_KEY"
                        ReadParms parameters_Check = new ReadParms();
                        parameters_Check.readCmdParms.bank = MemoryBank.USER;
                        parameters_Check.readCmdParms.offset = 135;  //87h
                        parameters_Check.readCmdParms.count = 1;
                        parameters_Check.common.strcTagFlag.RetryCount = retryCount;
                        parameters_Check.common.strcTagFlag.SelectOpsFlag = 1;
                        parameters_Check.common.strcTagFlag.PostMatchFlag = 0;
                        parameters_Check.accessPassword = _tagAccessData.accessPassword;
                        parameters_Check.common.callback = new rfid.CallbackDelegate(this._reader.MyCallback);
                        parameters_Check.common.OpMode = RadioOperationMode.NONCONTINUOUS;

                        parameters_Check.common.tagFlag = false;
                        Array.Clear(parameters_Check.common.bufMTII, 0, parameters_Check.common.bufMTII.Length);
                        Array.Clear(parameters_Check.common.bufMTIA, 0, parameters_Check.common.bufMTIA.Length);

                        Result = LakeChabotReader.MANAGED_ACCESS.API_l8K6CTagRead(parameters_Check, flags);

                        //if (Result.OK != LakeChabotReader.MANAGED_ACCESS.CB2_TAG_READ(parameters_Check, flags, Tag_Infor))
                        if (Result.OK != Result)
                            Textbox_Configure_Status_Prinf("Read Check Value Command Failed.");

                        analysis_TagAccessPkt(parameters_Check, flags, parameters_Check.common.bufMTII, parameters_Check.common.bufMTIA);

                        if (Tag_Infor.Tag_ACCESS_FLAG == true)
                        {
                            if (Convert.ToInt32(Tag_Infor.Tag_USER_KEY, 2) == 0)
                                Textbox_Configure_Status_Prinf("Set 'CFG_DAT0' Data Success.");
                            else if (Convert.ToInt32(Tag_Infor.Tag_USER_KEY, 2) == 65535)
                                Textbox_Configure_Status_Prinf("Set 'CFG_DAT0' Data Failed.");
                            else
                                Textbox_Configure_Status_Prinf("Set 'CFG_DAT0' Data In Progress.");

                            BUTTON_ACCESS_MEMORY_SET.Enabled = true;
                        }
                        else
                        {
                            Textbox_Configure_Status_Prinf("Set 'CFG_DAT0' Data Completed, But Check Status Failed.");
                            BUTTON_ACCESS_MEMORY_SET.Enabled = true;
                        }
                    }

                    break;
            }
            Clean_Select_Critrtia_Mask();
            this._reader.CB2_ProcessQueuedPackets(); //Add FJ for fixing packet parsing mechanism for operating CB-2, 2017/12/07
        }

        //Add bu FJ for display the access memory value when CFG_DAT1 change to CFG_DAT0(CFG_DAT0 change to CFG_DAT1)
        private void COMBOBOX_DAT_MSK_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (COMBOBOX_DAT_MSK.SelectedIndex)
            {
                case 1:
                    Clean_CFG_DAT1_Text();
                    Button_CFG_DAT_Get_Click(sender, e);

                    //For set CFG_DATA1 & CFG_DATA0 whether display
                    //PANEL_DAT0.Visible = false;
                    //PANEL_DAT1.Visible = true;

                    TEXTBOX_RFU.Enabled = false;
                    TEXTBOX_WWU.Enabled = false;
                    TEXTBOX_BPL_EN.Enabled = false;
                    TEXTBOX_QT_SR.Enabled = false;
                    TEXTBOX_QT_MEM.Enabled = false;
                    TEXTBOX_DCI_RF_EN.Enabled = false;
                    TEXTBOX_RF2_DIS.Enabled = false;
                    TEXTBOX_RF1_DIS.Enabled = false;

                    break;

                case 0:
                    Clean_CFG_DAT0_Text();
                    Button_CFG_DAT_Get_Click(sender, e);

                    //For set CFG_DATA1 & CFG_DATA0 whether display
                    //PANEL_DAT1.Visible = false;
                    //PANEL_DAT0.Visible = true;

                    TEXTBOX_LOCK_KILL.Enabled = false;
                    TEXTBOX_LOCK_ACCE.Enabled = false;
                    TEXTBOX_LOCK_EPC.Enabled = false;
                    TEXTBOX_LOCK_USER.Enabled = false;
                    TEXTBOX_BLOCK_PERMALOCK.Enabled = false;
                    TEXTBOX_KILL.Enabled = false;
                    TEXTBOX_CONFIG.Enabled = false;

                    break;
                default:
                    break;
            }
        }

        //Add by FJ clean the access memory area value when click inventory & get & set function failed
        private void Clear_Access_Memory_Status()
        {
            BUTTON_ACCESS_MEMORY_SET.Enabled = false;
            Clean_CFG_DAT1_Text();
            Clean_CFG_DAT0_Text();
            if (Tag_Infor.Tag_ACCESS_FLAG == true)
            {
                //DAT1
                PANEL_DAT1.Enabled = true;
                TEXTBOX_RFU.Enabled = false;
                TEXTBOX_WWU.Enabled = false;
                TEXTBOX_BPL_EN.Enabled = false;
                TEXTBOX_QT_SR.Enabled = false;
                TEXTBOX_QT_MEM.Enabled = false;
                TEXTBOX_DCI_RF_EN.Enabled = false;
                TEXTBOX_RF2_DIS.Enabled = false;
                TEXTBOX_RF1_DIS.Enabled = false;

                //DAT0
                PANEL_DAT0.Enabled = true;
                TEXTBOX_LOCK_KILL.Enabled = false;
                TEXTBOX_LOCK_ACCE.Enabled = false;
                TEXTBOX_LOCK_EPC.Enabled = false;
                TEXTBOX_LOCK_USER.Enabled = false;
                TEXTBOX_BLOCK_PERMALOCK.Enabled = false;
                TEXTBOX_KILL.Enabled = false;
                TEXTBOX_CONFIG.Enabled = false;

                COMBOBOX_DAT_MSK.Enabled = true;
                BUTTON_ACCESS_MEMORY_GET.Enabled = true;
            }
            else
            {
                PANEL_DAT1.Enabled = false;
                PANEL_DAT0.Enabled = false;
                COMBOBOX_DAT_MSK.Enabled = false;
                BUTTON_ACCESS_MEMORY_GET.Enabled = false;
            }
        }

        //Add by FJ for WWU value can be entered when click WWU checkbox
        private void Checkbox_WWU_Click(object sender, EventArgs e)
        {
            TEXTBOX_WWU.Enabled = CHECKBOX_WWU.Checked;
            Check_Dat1_Mask_Click_For_Set();
        }

        //Add by FJ for BPL_EN value can be entered when click BPL_EN checkbox
        private void Checkbox_BPL_EN_Click(object sender, EventArgs e)
        {
            TEXTBOX_BPL_EN.Enabled = CHECKBOX_BPL_EN.Checked;
            Check_Dat1_Mask_Click_For_Set();
        }

        //Add by FJ for QT_SR value can be entered when click QT_SR checkbox
        private void Checkbox_QT_SR_Click(object sender, EventArgs e)
        {
            TEXTBOX_QT_SR.Enabled = CHECKBOX_QT_SR.Checked;
            Check_Dat1_Mask_Click_For_Set();
        }

        //Add by FJ for QT_MEM value can be entered when click QT_MEM checkbox
        private void Checkbox_QT_MEM_Click(object sender, EventArgs e)
        {
            TEXTBOX_QT_MEM.Enabled = CHECKBOX_QT_MEM.Checked;
            Check_Dat1_Mask_Click_For_Set();
        }

        //Add by FJ for DCI_RF_EN value can be entered when click DCI_RF_EN checkbox
        private void Checkbox_DCI_RF_EN_Click(object sender, EventArgs e)
        {
            TEXTBOX_DCI_RF_EN.Enabled = CHECKBOX_DCI_RF_EN.Checked;
            Check_Dat1_Mask_Click_For_Set();
        }

        //Add by FJ for RF2_DIS value can be entered when click RF2_DIS checkbox
        private void Checkbox_RF2_DIS_Click(object sender, EventArgs e)
        {
            TEXTBOX_RF2_DIS.Enabled = CHECKBOX_RF2_DIS.Checked;
            Check_Dat1_Mask_Click_For_Set();
        }

        //Add by FJ for RF1_DIS value can be entered when click RF1_DIS checkbox
        private void Checkbox_RF1_DIS_Click(object sender, EventArgs e)
        {
            TEXTBOX_RF1_DIS.Enabled = CHECKBOX_RF1_DIS.Checked;
            Check_Dat1_Mask_Click_For_Set();
        }

        //Add by FJ for set button can be click when any checkbox clicked
        private void Check_Dat1_Mask_Click_For_Set()
        {
            if (CHECKBOX_WWU.Checked == true ||
                 CHECKBOX_BPL_EN.Checked == true ||
                 CHECKBOX_QT_SR.Checked == true ||
                 CHECKBOX_QT_MEM.Checked == true ||
                 CHECKBOX_DCI_RF_EN.Checked == true ||
                 CHECKBOX_RF2_DIS.Checked == true ||
                 CHECKBOX_RF1_DIS.Checked == true)
            {
                BUTTON_ACCESS_MEMORY_SET.Enabled = true;
            }
            else
            {
                BUTTON_ACCESS_MEMORY_SET.Enabled = false;
            }
        }

        //Add by FJ for LOCK_KILL value can be entered when click LOCK_KILL checkbox
        private void Checkbox_LOCK_KILL_Click(object sender, EventArgs e)
        {
            TEXTBOX_LOCK_KILL.Enabled = CHECKBOX_LOCK_KILL.Checked;
            Check_Dat2_Mask_Click_For_Set();
        }

        //Add by FJ for LOCK_ACCE value can be entered when click LOCK_ACCE checkbox
        private void Checkbox_LOCK_ACCE_Click(object sender, EventArgs e)
        {
            TEXTBOX_LOCK_ACCE.Enabled = CHECKBOX_LOCK_ACCE.Checked;
            Check_Dat2_Mask_Click_For_Set();
        }

        //Add by FJ for LOCK_EPC value can be entered when click LOCK_EPC checkbox
        private void Checkbox_LOCK_EPC_Click(object sender, EventArgs e)
        {
            TEXTBOX_LOCK_EPC.Enabled = CHECKBOX_LOCK_EPC.Checked;
            Check_Dat2_Mask_Click_For_Set();
        }

        //Add by FJ for LOCK_USER value can be entered when click LOCK_USER checkbox
        private void Checkbox_LOCK_USER_Click(object sender, EventArgs e)
        {
            TEXTBOX_LOCK_USER.Enabled = CHECKBOX_LOCK_USER.Checked;
            Check_Dat2_Mask_Click_For_Set();
        }

        //Add by FJ for BLOCK_PERMALOCK value can be entered when click BLOCK_PERMALOCK checkbox
        private void Checkbox_BLOCK_PERMALOCK_Click(object sender, EventArgs e)
        {
            TEXTBOX_BLOCK_PERMALOCK.Enabled = CHECKBOX_BLOCK_PERMALOCK.Checked;
            Check_Dat2_Mask_Click_For_Set();
        }

        //Add by FJ for KILL value can be entered when click KILL checkbox
        private void Checkbox_KILL_Click(object sender, EventArgs e)
        {
            TEXTBOX_KILL.Enabled = CHECKBOX_KILL.Checked;
            Check_Dat2_Mask_Click_For_Set();
        }

        //Add by FJ for set button can be click when any checkbox clicked
        private void Check_Dat2_Mask_Click_For_Set()
        {
            if (CHECKBOX_LOCK_KILL.Checked == true ||
                 CHECKBOX_LOCK_ACCE.Checked == true ||
                 CHECKBOX_LOCK_EPC.Checked == true ||
                 CHECKBOX_LOCK_USER.Checked == true ||
                 CHECKBOX_BLOCK_PERMALOCK.Checked == true ||
                 CHECKBOX_KILL.Checked == true)
            {
                BUTTON_ACCESS_MEMORY_SET.Enabled = true;
            }
            else
            {
                BUTTON_ACCESS_MEMORY_SET.Enabled = false;
            }
        }

        //Add by FJ for go back initial CFG_DAT1 value  
        private void Clean_CFG_DAT1_Text()
        {
            TEXTBOX_RFU.Text = "";
            TEXTBOX_WWU.Text = "";
            TEXTBOX_BPL_EN.Text = "";
            TEXTBOX_QT_SR.Text = "";
            TEXTBOX_QT_MEM.Text = "";
            TEXTBOX_DCI_RF_EN.Text = "";
            TEXTBOX_RF2_DIS.Text = "";
            TEXTBOX_RF1_DIS.Text = "";

            CHECKBOX_WWU.Checked = false;
            CHECKBOX_BPL_EN.Checked = false;
            CHECKBOX_QT_SR.Checked = false;
            CHECKBOX_QT_MEM.Checked = false;
            CHECKBOX_DCI_RF_EN.Checked = false;
            CHECKBOX_RF2_DIS.Checked = false;
            CHECKBOX_RF1_DIS.Checked = false;
        }

        //Add by FJ for go back initial CFG_DAT0 value  
        private void Clean_CFG_DAT0_Text()
        {
            TEXTBOX_LOCK_KILL.Text = "";
            TEXTBOX_LOCK_ACCE.Text = "";
            TEXTBOX_LOCK_EPC.Text = "";
            TEXTBOX_LOCK_USER.Text = "";
            TEXTBOX_BLOCK_PERMALOCK.Text = "";
            TEXTBOX_KILL.Text = "";
            TEXTBOX_CONFIG.Text = "";

            CHECKBOX_LOCK_KILL.Checked = false;
            CHECKBOX_LOCK_ACCE.Checked = false;
            CHECKBOX_LOCK_EPC.Checked = false;
            CHECKBOX_LOCK_USER.Checked = false;
            CHECKBOX_BLOCK_PERMALOCK.Checked = false;
            CHECKBOX_KILL.Checked = false;
        }

        //Add by FJ for disply the configure status message
        private void Textbox_Configure_Status_Prinf(String str)
        {
            TEXTBOX_CONFIGURE_STATUS.Text = TEXTBOX_CONFIGURE_STATUS.Text + str + Environment.NewLine;
            Configure_TextBox_Refresh();
        }

        //Add by FJ for refresh the configure status message
        private void Configure_TextBox_Refresh()
        {
            TEXTBOX_CONFIGURE_STATUS.SelectionStart = TEXTBOX_CONFIGURE_STATUS.Text.Length;
            TEXTBOX_CONFIGURE_STATUS.ScrollToCaret();
            TEXTBOX_CONFIGURE_STATUS.Refresh();
        }

        //Add by FJ for clean the configure status message
        private void Button_Configure_Status_Clean_Click(object sender, EventArgs e)
        {
            TEXTBOX_CONFIGURE_STATUS.Text = "";
        }



        /*---------------CONTROL FUNCTION TAB---------------*/
        //Add by FJ for inventory function when click inventory button
        private void Button_Control_Inventory_Click(object sender, EventArgs e)
        {
            byte flags = 0;
            BUTTON_CONTROL_INVENTORY.Enabled = false;

            //reset
            Clean_Select_Critrtia_Mask();
            TEXTBOX_CONTROL_PC_EPC_CRC.Text = "";
            TEXTBOX_CONTROL_TID_SN.Text = "";

            ReadParms parameters = new ReadParms();
            //Banl = 2
            parameters.readCmdParms.bank = MemoryBank.TID;
            //offset = 3
            parameters.readCmdParms.offset = 3;
            //Count = 3
            parameters.readCmdParms.count = 3;
            //RetryCount = 0
            parameters.common.strcTagFlag.RetryCount = retryCount;
            //PerformSelect = 1
            parameters.common.strcTagFlag.SelectOpsFlag = 0;
            //PerformPostMatch = 0
            parameters.common.strcTagFlag.PostMatchFlag = 0;

            parameters.accessPassword = _tagAccessData.accessPassword;
            parameters.common.callback = new rfid.CallbackDelegate(this._reader.MyCallback);
            parameters.common.OpMode = RadioOperationMode.NONCONTINUOUS;

            parameters.common.tagFlag = false;
            Array.Clear(parameters.common.bufMTII, 0, parameters.common.bufMTII.Length);
            Array.Clear(parameters.common.bufMTIA, 0, parameters.common.bufMTIA.Length);

            if (Result.OK != LakeChabotReader.MANAGED_ACCESS.API_l8K6CTagRead(parameters, flags))
            {
                Textbox_Control_Status_Prinf("Inventory Command Failed.");
            }
            else
            {
                analysis_TagAccessPkt(parameters, flags, parameters.common.bufMTII, parameters.common.bufMTIA);
                if (Tag_Infor.Tag_ACCESS_FLAG == true)
                {
                    Textbox_Control_Status_Prinf("Get EPC/TID Data Success.");
                    TEXTBOX_CONTROL_PC_EPC_CRC.Text = Tag_Infor.Tag_PC_EPC_CRC;
                    TEXTBOX_CONTROL_TID_SN.Text = Tag_Infor.Tag_TID;
                    Get_Control_Function_Default_Value();
                    BUTTON_CONTROL_INVENTORY.Enabled = true;
                }
                else
                {
                    Textbox_Control_Status_Prinf("Get EPC/TID Data Failed.");
                    TEXTBOX_CONTROL_PC_EPC_CRC.Text = "";
                    TEXTBOX_CONTROL_TID_SN.Text = "";
                    Change_Contorl_Functions_Status();
                    BUTTON_CONTROL_INVENTORY.Enabled = true;
                }
            }
            this._reader.CB2_ProcessQueuedPackets(); //Add FJ for fixing packet parsing mechanism for operating CB-2, 2017/12/07
        }

        //Add by FJ for get led & battery & antenna & version default value
        private void Get_Control_Function_Default_Value()
        {
            string TempValue;
            byte flags;
            Result Result = Result.OK;
            Edit_Select_Critrtia_Mask(Tag_Infor.Tag_TID);

            //Mod by FJ for getting button to read status of CB-2 tag control function, 2017/12/07
            //Control LED
            //Should to set wrong value, then repeat read LED bank to get current value.	
            flags = 0;
            TempValue = "1000000000000011";

            WriteParms currvalueParameters = new WriteParms();
            WriteSequentialParms writeParameters = new WriteSequentialParms();
            writeParameters.bank = MemoryBank.USER;
            writeParameters.offset = 130;  //82h
            writeParameters.pData = new ushort[2];
            writeParameters.pData[0] = Convert.ToUInt16(Convert.ToInt32(TempValue, 2));
            writeParameters.pData[1] = 0;
            writeParameters.count = 1;
            currvalueParameters.accessPassword = 0;
            currvalueParameters.writeParms = writeParameters;
            currvalueParameters.writeType = WriteType.SEQUENTIAL;
            currvalueParameters.common.callback = new rfid.CallbackDelegate(this._reader.MyCallback);

            currvalueParameters.common.strcTagFlag.RetryCount = retryCount;
            currvalueParameters.common.strcTagFlag.PostMatchFlag = 0;
            currvalueParameters.common.strcTagFlag.SelectOpsFlag = 1;
            currvalueParameters.common.OpMode = RadioOperationMode.NONCONTINUOUS;

            currvalueParameters.common.tagFlag = false;
            Array.Clear(currvalueParameters.common.bufMTII, 0, currvalueParameters.common.bufMTII.Length);
            Array.Clear(currvalueParameters.common.bufMTIA, 0, currvalueParameters.common.bufMTIA.Length);

            Result = LakeChabotReader.MANAGED_ACCESS.API_l8K6CTagWrite(currvalueParameters, flags);
            Tag_Infor.Tag_ACCESS_FLAG = currvalueParameters.common.tagFlag;

            if (Result.OK != Result)
                Textbox_Control_Status_Prinf("Set LED Wrong Value Command Failed.");

            flags = 2;  //2: Get "USER_LED"
            LED_Flag = false;
            ReadParms parameters = new ReadParms();
            parameters.readCmdParms.bank = MemoryBank.USER;
            parameters.readCmdParms.offset = 130;  //82h
            parameters.readCmdParms.count = 1;
            parameters.common.strcTagFlag.RetryCount = retryCount;
            parameters.common.strcTagFlag.SelectOpsFlag = 1;
            parameters.common.strcTagFlag.PostMatchFlag = 0;
            parameters.accessPassword = _tagAccessData.accessPassword;
            parameters.common.callback = new rfid.CallbackDelegate(this._reader.MyCallback);
            parameters.common.OpMode = RadioOperationMode.NONCONTINUOUS;
            Tag_Infor.Tag_USER_LED = "";

            System.Threading.Thread.Sleep(1000);

            parameters.common.tagFlag = false;
            Array.Clear(parameters.common.bufMTII, 0, parameters.common.bufMTII.Length);
            Array.Clear(parameters.common.bufMTIA, 0, parameters.common.bufMTIA.Length);

            if (Tag_Infor.Tag_ACCESS_FLAG == true)
            {
                ds1 = DateTime.Now;
                ts1 = new TimeSpan(ds1.Ticks);

                while (true)
                {
                    Result = LakeChabotReader.MANAGED_ACCESS.API_l8K6CTagRead(parameters, flags);

                    if (Result.OK != Result)
                    {
                        Textbox_Control_Status_Prinf("Read LED Value Command Failed.");
                        Tag_Infor.Tag_ACCESS_FLAG = false;
                        TEXTBOX_CONTROL_PC_EPC_CRC.Text = "";
                        TEXTBOX_CONTROL_TID_SN.Text = "";
                        Change_Contorl_Functions_Status();
                        Close_Contorl_Functions_Button();
                        break;
                    }
                    else
                    {
                        analysis_TagAccessPkt(parameters, flags, parameters.common.bufMTII, parameters.common.bufMTIA);
                        if (Tag_Infor.Tag_Error == 1 || Tag_Infor.Tag_Error == 2 || Tag_Infor.Tag_Error == 3)
                        {
                            Textbox_Control_Status_Prinf("Get LED Value Failed - Read Tag Error.");
                            Tag_Infor.Tag_ACCESS_FLAG = false;
                            TEXTBOX_CONTROL_PC_EPC_CRC.Text = "";
                            TEXTBOX_CONTROL_TID_SN.Text = "";
                            Change_Contorl_Functions_Status();
                            Close_Contorl_Functions_Button();
                            break;
                        }
                        else
                        {
                            if (Tag_Infor.Tag_ACCESS_FLAG == true)
                            {
                                char[] TempArray = Tag_Infor.Tag_USER_LED.ToCharArray();
                                string TempLED_Color = "", TempLED_Blink = "", Check_Status = "";
                                Tag_Infor.Tag_USER_LED = "";
                                for (int i = 0; i < TempArray.Length; i++)
                                {
                                    switch (i)
                                    {
                                        case 0:
                                            Check_Status = System.Convert.ToString(TempArray[i]);
                                            break;

                                        case 14:
                                        case 15:
                                            TempLED_Color += System.Convert.ToString(TempArray[i]);
                                            break;

                                        default:
                                            TempLED_Blink += System.Convert.ToString(TempArray[i]);
                                            break;
                                    }
                                }
                                if (Convert.ToInt32(Check_Status, 2) == 0)
                                {
                                    if (Convert.ToInt32(TempLED_Color, 2) >= 3)
                                    {
                                        MessageDisplay("LED is in unknown status.", "Error", MessageBoxIcon.Error);
                                        LED_Flag = true;
                                        COMBOBOX_COLOR.SelectedIndex = 0;
                                        NUMERICUPDOWN_LED_BLINK.Value = 0;
                                    }
                                    else
                                    {
                                        Textbox_Control_Status_Prinf("Get LED Value Success.");
                                        LED_Flag = true;
                                        COMBOBOX_COLOR.SelectedIndex = Convert.ToInt32(TempLED_Color, 2);
                                        NUMERICUPDOWN_LED_BLINK.Value = Convert.ToInt32(TempLED_Blink, 2);
                                    }
                                    break;
                                }
                                else
                                {
                                    ds2 = DateTime.Now;
                                    ts2 = new TimeSpan(ds2.Ticks);
                                    TimeSpan ts = ts1.Subtract(ts2).Duration();
                                    if (ts.Seconds >= 1)
                                    {
                                        Textbox_Control_Status_Prinf("Get LED Value Timeout.");
                                        Tag_Infor.Tag_ACCESS_FLAG = false;
                                        TEXTBOX_CONTROL_PC_EPC_CRC.Text = "";
                                        TEXTBOX_CONTROL_TID_SN.Text = "";
                                        Change_Contorl_Functions_Status();
                                        Close_Contorl_Functions_Button();
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                Textbox_Control_Status_Prinf("Get LED Value Failed.");
                                TEXTBOX_CONTROL_PC_EPC_CRC.Text = "";
                                TEXTBOX_CONTROL_TID_SN.Text = "";
                                Change_Contorl_Functions_Status();
                                Close_Contorl_Functions_Button();
                                break;
                            }
                        }
                    }
                }
            }
            else
            {
                Textbox_Control_Status_Prinf("Get Data Buf Packet Failed - LED.");
                TEXTBOX_CONTROL_PC_EPC_CRC.Text = "";
                TEXTBOX_CONTROL_TID_SN.Text = "";
                Change_Contorl_Functions_Status();
                Close_Contorl_Functions_Button();
            }

            /*
            //Control LED
            byte flags = 2;  //2: Get "USER_LED"
            ReadParms parameters = new ReadParms();
            parameters.readCmdParms.bank = MemoryBank.USER;
            parameters.readCmdParms.offset = 130;  //82h
            parameters.readCmdParms.count = 1;
            parameters.common.strcTagFlag.RetryCount = 0;
            parameters.common.strcTagFlag.SelectOpsFlag = 1;
            parameters.common.strcTagFlag.PostMatchFlag = 0;
            parameters.accessPassword = _tagAccessData.accessPassword;
            parameters.common.callback = new rfid.CallbackDelegate(this._reader.MyCallback);
            parameters.common.OpMode = RadioOperationMode.NONCONTINUOUS;

            Result = LakeChabotReader.MANAGED_ACCESS.CB2_TAG_READ(parameters, flags, Tag_Infor);

            if (Result.OK != Result)
            {
                Textbox_Control_Status_Prinf("Read LED Value Command Failed.");
            }
            else
            {
                if (Tag_Infor.Tag_Error == 1 || Tag_Infor.Tag_Error == 2 || Tag_Infor.Tag_Error == 3)
                {
                    Textbox_Control_Status_Prinf("Get LED Value Failed - Non CB-2 Tag.");
                    Tag_Infor.Tag_ACCESS_FLAG = false;
                    Change_Contorl_Functions_Status();
                    return;
                }
                else
                {
                    if (Tag_Infor.Tag_ACCESS_FLAG == true)
                    {
                        Change_Contorl_Functions_Status();
                        char[] TempArray = Tag_Infor.Tag_USER_LED.ToCharArray();
                        string TempLED_Color = "", TempLED_Blink = "";
                        Tag_Infor.Tag_USER_LED = "";
                        for (int i = 0; i < TempArray.Length; i++)
                        {
                            switch (i)
                            {
                                case 0:
                                    break;

                                case 14:
                                case 15:
                                    TempLED_Color += System.Convert.ToString(TempArray[i]);
                                    break;

                                default:
                                    TempLED_Blink += System.Convert.ToString(TempArray[i]);
                                    break;
                            }
                        }
                        COMBOBOX_COLOR.SelectedIndex = Convert.ToInt32(TempLED_Color, 2);
                        if (Convert.ToInt32(TempLED_Blink, 2) == 0)
                            NUMERICUPDOWN_LED_BLINK.Value = 1;
                        else
                            NUMERICUPDOWN_LED_BLINK.Value = Convert.ToInt32(TempLED_Blink, 2);
                    }
                    else
                    {
                        Textbox_Control_Status_Prinf("Get LED Value Failed.");
                        TEXTBOX_CONTROL_PC_EPC_CRC.Text = "";
                        TEXTBOX_CONTROL_TID_SN.Text = "";
                        Change_Contorl_Functions_Status();
                    }
                }
            }
            */

            //Control Battery
            //Should to set wrong value, then repeat read Battery bank to get current value.
            flags = 0;
            TempValue = "1000000000000010";
            Battery_Flag = false;

            writeParameters.bank = MemoryBank.USER;
            writeParameters.offset = 129;  //81h
            writeParameters.pData = new ushort[2];
            writeParameters.pData[0] = Convert.ToUInt16(Convert.ToInt32(TempValue, 2));
            writeParameters.pData[1] = 0;
            writeParameters.count = 1;
            currvalueParameters.writeParms = writeParameters;

            currvalueParameters.common.tagFlag = false;
            Array.Clear(currvalueParameters.common.bufMTII, 0, currvalueParameters.common.bufMTII.Length);
            Array.Clear(currvalueParameters.common.bufMTIA, 0, currvalueParameters.common.bufMTIA.Length);

            Result = LakeChabotReader.MANAGED_ACCESS.API_l8K6CTagWrite(currvalueParameters, flags);
            Tag_Infor.Tag_ACCESS_FLAG = currvalueParameters.common.tagFlag;

            if (Result.OK != Result)
                Textbox_Control_Status_Prinf("Set Battery Wrong Value Command Failed.");

            System.Threading.Thread.Sleep(1000);

            parameters.common.tagFlag = false;
            Array.Clear(parameters.common.bufMTII, 0, parameters.common.bufMTII.Length);
            Array.Clear(parameters.common.bufMTIA, 0, parameters.common.bufMTIA.Length);

            if (Tag_Infor.Tag_ACCESS_FLAG == true)
            {
                flags = 3;  //3: Get "USER_BAT"
                parameters.readCmdParms.offset = 129;  //81h

                ds1 = DateTime.Now;
                ts1 = new TimeSpan(ds1.Ticks);

                while (true)
                {
                    Result = LakeChabotReader.MANAGED_ACCESS.API_l8K6CTagRead(parameters, flags);

                    if (Result.OK != Result)
                    {
                        Textbox_Control_Status_Prinf("Read Battery Value Command Failed.");
                        Tag_Infor.Tag_ACCESS_FLAG = false;
                        TEXTBOX_CONTROL_PC_EPC_CRC.Text = "";
                        TEXTBOX_CONTROL_TID_SN.Text = "";
                        Change_Contorl_Functions_Status();
                        Close_Contorl_Functions_Button();
                        break;
                    }
                    else
                    {
                        analysis_TagAccessPkt(parameters, flags, parameters.common.bufMTII, parameters.common.bufMTIA);
                        if (Tag_Infor.Tag_Error == 1 || Tag_Infor.Tag_Error == 2 || Tag_Infor.Tag_Error == 3)
                        {
                            Textbox_Control_Status_Prinf("Get Battery Value Failed - Read Tag Error.");
                            Tag_Infor.Tag_ACCESS_FLAG = false;
                            TEXTBOX_CONTROL_PC_EPC_CRC.Text = "";
                            TEXTBOX_CONTROL_TID_SN.Text = "";
                            Change_Contorl_Functions_Status();
                            Close_Contorl_Functions_Button();
                            break;
                        }
                        else
                        {
                            if (Tag_Infor.Tag_ACCESS_FLAG == true)
                            {
                                char[] TempArray = Tag_Infor.Tag_USER_BAT.ToCharArray();
                                string TempBAT = "", Check_Status = "";
                                Tag_Infor.Tag_USER_BAT = "";
                                for (int i = 0; i < TempArray.Length; i++)
                                {
                                    switch (i)
                                    {
                                        case 0:
                                            Check_Status = System.Convert.ToString(TempArray[i]);
                                            break;

                                        case 15:
                                            TempBAT = System.Convert.ToString(TempArray[i]);
                                            break;
                                    }
                                }
                                if (Convert.ToInt32(Check_Status, 2) == 0)
                                {
                                    Textbox_Control_Status_Prinf("Get Battery Value Success.");
                                    Battery_Flag = true;
                                    COMBOBOX_BATTERY_ASSIST.SelectedIndex = Convert.ToInt32(TempBAT, 2);
                                    break;
                                }
                                else
                                {
                                    ds2 = DateTime.Now;
                                    ts2 = new TimeSpan(ds2.Ticks);
                                    TimeSpan ts = ts1.Subtract(ts2).Duration();
                                    if (ts.Seconds >= 1)
                                    {
                                        Textbox_Control_Status_Prinf("Get Battery Value Timeout.");
                                        Tag_Infor.Tag_ACCESS_FLAG = false;
                                        TEXTBOX_CONTROL_PC_EPC_CRC.Text = "";
                                        TEXTBOX_CONTROL_TID_SN.Text = "";
                                        Change_Contorl_Functions_Status();
                                        Close_Contorl_Functions_Button();
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                Textbox_Control_Status_Prinf("Get Battery Value Failed.");
                                TEXTBOX_CONTROL_PC_EPC_CRC.Text = "";
                                TEXTBOX_CONTROL_TID_SN.Text = "";
                                Change_Contorl_Functions_Status();
                                Close_Contorl_Functions_Button();
                                break;
                            }
                        }
                    }
                }
            }
            else
            {
                Textbox_Control_Status_Prinf("Get Data Buf Packet Failed - Battery.");
                TEXTBOX_CONTROL_PC_EPC_CRC.Text = "";
                TEXTBOX_CONTROL_TID_SN.Text = "";
                Change_Contorl_Functions_Status();
                Close_Contorl_Functions_Button();
            }

            /*
            //Control Battery
            flags = 3;  //3: Get "USER_BAT"
            parameters.readCmdParms.offset = 129;  //81h

            Result = LakeChabotReader.MANAGED_ACCESS.CB2_TAG_READ(parameters, flags, Tag_Infor);

            if (Result.OK != Result)
            {
                Textbox_Control_Status_Prinf("Read Battery Value Command Failed.");
            }
            else
            {
                if (Tag_Infor.Tag_Error == 1 || Tag_Infor.Tag_Error == 2 || Tag_Infor.Tag_Error == 3)
                {
                    Textbox_Control_Status_Prinf("Get Battery Value Failed - Non CB-2 Tag.");
                    Tag_Infor.Tag_ACCESS_FLAG = false;
                    Change_Contorl_Functions_Status();
                    return;
                }
                else
                {
                    if (Tag_Infor.Tag_ACCESS_FLAG == true)
                    {
                        Change_Contorl_Functions_Status();
                        char[] TempArray = Tag_Infor.Tag_USER_BAT.ToCharArray();
                        string TempBAT = "";
                        Tag_Infor.Tag_USER_BAT = "";
                        for (int i = 0; i < TempArray.Length; i++)
                        {
                            switch (i)
                            {
                                case 15:
                                    TempBAT = System.Convert.ToString(TempArray[i]);
                                    COMBOBOX_BATTERY_ASSIST.SelectedIndex = Convert.ToInt32(TempBAT, 2);
                                    break;
                            }
                        }
                    }
                    else
                    {
                        Textbox_Control_Status_Prinf("Get Battery Value Failed.");
                        TEXTBOX_CONTROL_PC_EPC_CRC.Text = "";
                        TEXTBOX_CONTROL_TID_SN.Text = "";
                        Change_Contorl_Functions_Status();
                    }
                }
            }
            */

            //Control Antenna
            //Should to set wrong value, then repeat read Antenna bank to get current value.
            flags = 0;
            TempValue = "1000000000000011";
            Antenna_Flag = false;

            writeParameters.bank = MemoryBank.USER;
            writeParameters.offset = 128;  //80h
            writeParameters.pData = new ushort[2];
            writeParameters.pData[0] = Convert.ToUInt16(Convert.ToInt32(TempValue, 2));
            writeParameters.pData[1] = 0;
            writeParameters.count = 1;
            currvalueParameters.writeParms = writeParameters;

            currvalueParameters.common.tagFlag = false;
            Array.Clear(currvalueParameters.common.bufMTII, 0, currvalueParameters.common.bufMTII.Length);
            Array.Clear(currvalueParameters.common.bufMTIA, 0, currvalueParameters.common.bufMTIA.Length);

            Result = LakeChabotReader.MANAGED_ACCESS.API_l8K6CTagWrite(currvalueParameters, flags);
            Tag_Infor.Tag_ACCESS_FLAG = currvalueParameters.common.tagFlag;

            if (Result.OK != Result)
                Textbox_Control_Status_Prinf("Set Antenna Wrong Value Command Failed.");

            System.Threading.Thread.Sleep(1000);

            parameters.common.tagFlag = false;
            Array.Clear(parameters.common.bufMTII, 0, parameters.common.bufMTII.Length);
            Array.Clear(parameters.common.bufMTIA, 0, parameters.common.bufMTIA.Length);

            if (Tag_Infor.Tag_ACCESS_FLAG == true)
            {
                flags = 4;  //4: Get "USER_ANT"
                parameters.readCmdParms.offset = 128;  //80h

                ds1 = DateTime.Now;
                ts1 = new TimeSpan(ds1.Ticks);

                while (true)
                {
                    Result = LakeChabotReader.MANAGED_ACCESS.API_l8K6CTagRead(parameters, flags);

                    if (Result.OK != Result)
                    {
                        Textbox_Control_Status_Prinf("Read Antenna Value Command Failed.");
                        Tag_Infor.Tag_ACCESS_FLAG = false;
                        TEXTBOX_CONTROL_PC_EPC_CRC.Text = "";
                        TEXTBOX_CONTROL_TID_SN.Text = "";
                        Change_Contorl_Functions_Status();
                        Close_Contorl_Functions_Button();
                        break;
                    }
                    else
                    {
                        analysis_TagAccessPkt(parameters, flags, parameters.common.bufMTII, parameters.common.bufMTIA);
                        if (Tag_Infor.Tag_Error == 1 || Tag_Infor.Tag_Error == 2 || Tag_Infor.Tag_Error == 3)
                        {
                            Textbox_Control_Status_Prinf("Get Antenna Value Failed - Read Tag Error.");
                            Tag_Infor.Tag_ACCESS_FLAG = false;
                            TEXTBOX_CONTROL_PC_EPC_CRC.Text = "";
                            TEXTBOX_CONTROL_TID_SN.Text = "";
                            Change_Contorl_Functions_Status();
                            Close_Contorl_Functions_Button();
                            break;
                        }
                        else
                        {
                            if (Tag_Infor.Tag_ACCESS_FLAG == true)
                            {
                                char[] TempArray = Tag_Infor.Tag_USER_ANT.ToCharArray();
                                string TempANT = "", Check_Status = "";
                                Tag_Infor.Tag_USER_ANT = "";
                                for (int i = 0; i < TempArray.Length; i++)
                                {
                                    switch (i)
                                    {
                                        case 0:
                                            Check_Status = System.Convert.ToString(TempArray[i]);
                                            break;

                                        case 14:
                                        case 15:
                                            TempANT += System.Convert.ToString(TempArray[i]);
                                            break;
                                    }
                                }
                                if (Convert.ToInt32(Check_Status, 2) == 0)
                                {
                                    Textbox_Control_Status_Prinf("Get Antenna Value Success.");
                                    Antenna_Flag = true;
                                    COMBOBOX_ANTENNA_SELECT.SelectedIndex = Convert.ToInt32(TempANT, 2);
                                    break;
                                }
                                else
                                {
                                    ds2 = DateTime.Now;
                                    ts2 = new TimeSpan(ds2.Ticks);
                                    TimeSpan ts = ts1.Subtract(ts2).Duration();
                                    if (ts.Seconds >= 1)
                                    {
                                        Textbox_Control_Status_Prinf("Get Antenna Value Timeout.");
                                        Tag_Infor.Tag_ACCESS_FLAG = false;
                                        TEXTBOX_CONTROL_PC_EPC_CRC.Text = "";
                                        TEXTBOX_CONTROL_TID_SN.Text = "";
                                        Change_Contorl_Functions_Status();
                                        Close_Contorl_Functions_Button();
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                Textbox_Control_Status_Prinf("Get Antenna Value Failed.");
                                TEXTBOX_CONTROL_PC_EPC_CRC.Text = "";
                                TEXTBOX_CONTROL_TID_SN.Text = "";
                                Change_Contorl_Functions_Status();
                                Close_Contorl_Functions_Button();
                                break;
                            }
                        }
                    }
                }
            }
            else
            {
                Textbox_Control_Status_Prinf("Get Data Buf Packet Failed - Antenna.");
                TEXTBOX_CONTROL_PC_EPC_CRC.Text = "";
                TEXTBOX_CONTROL_TID_SN.Text = "";
                Change_Contorl_Functions_Status();
                Close_Contorl_Functions_Button();
            }

            /*
            //Control Antenna
            flags = 4;  //4: Get "USER_ANT"
            parameters.readCmdParms.offset = 128;  //80h

            Result = LakeChabotReader.MANAGED_ACCESS.CB2_TAG_READ(parameters, flags, Tag_Infor);

            if (Result.OK != Result)
            {
                Textbox_Control_Status_Prinf("Read Antenna Value Command Failed.");
            }
            else
            {
                if (Tag_Infor.Tag_Error == 1 || Tag_Infor.Tag_Error == 2 || Tag_Infor.Tag_Error == 3)
                {
                    Textbox_Control_Status_Prinf("Get Antenna Value Failed - Non CB-2 Tag.");
                    Tag_Infor.Tag_ACCESS_FLAG = false;
                    Change_Contorl_Functions_Status();
                    return;
                }
                else
                {
                    if (Tag_Infor.Tag_ACCESS_FLAG == true)
                    {
                        Change_Contorl_Functions_Status();
                        char[] TempArray = Tag_Infor.Tag_USER_ANT.ToCharArray();
                        string TempANT = "";
                        Tag_Infor.Tag_USER_ANT = "";
                        for (int i = 0; i < TempArray.Length; i++)
                        {
                            switch (i)
                            {
                                case 14:
                                case 15:
                                    TempANT += System.Convert.ToString(TempArray[i]);
                                    COMBOBOX_ANTENNA_SELECT.SelectedIndex = Convert.ToInt32(TempANT, 2);
                                    break;
                            }
                        }
                    }
                    else
                    {
                        Textbox_Control_Status_Prinf("Get Antenna Value Failed.");
                        TEXTBOX_CONTROL_PC_EPC_CRC.Text = "";
                        TEXTBOX_CONTROL_TID_SN.Text = "";
                        Change_Contorl_Functions_Status();
                    }
                }
            }
            */

            //Control Version
            flags = 5;  //5: Get "USER_VER"
            Version_Flag = false;
            parameters.readCmdParms.offset = 127;  //7Fh

            parameters.common.tagFlag = false;
            Array.Clear(parameters.common.bufMTII, 0, parameters.common.bufMTII.Length);
            Array.Clear(parameters.common.bufMTIA, 0, parameters.common.bufMTIA.Length);

            Result = LakeChabotReader.MANAGED_ACCESS.API_l8K6CTagRead(parameters, flags);

            if (Result.OK != Result)
            {
                Textbox_Control_Status_Prinf("Read Version Number Command Failed.");
                Tag_Infor.Tag_ACCESS_FLAG = false;
                TEXTBOX_CONTROL_PC_EPC_CRC.Text = "";
                TEXTBOX_CONTROL_TID_SN.Text = "";
                Change_Contorl_Functions_Status();
                Close_Contorl_Functions_Button();
                return;
            }
            else
            {
                analysis_TagAccessPkt(parameters, flags, parameters.common.bufMTII, parameters.common.bufMTIA);
                if (Tag_Infor.Tag_Error == 1 || Tag_Infor.Tag_Error == 2 || Tag_Infor.Tag_Error == 3)
                {
                    Textbox_Control_Status_Prinf("Get Version Number Failed - Read Tag Error.");
                    Tag_Infor.Tag_ACCESS_FLAG = false;
                    TEXTBOX_CONTROL_PC_EPC_CRC.Text = "";
                    TEXTBOX_CONTROL_TID_SN.Text = "";
                    Change_Contorl_Functions_Status();
                    Close_Contorl_Functions_Button();
                    return;
                }
                else
                {
                    if (Tag_Infor.Tag_ACCESS_FLAG == true)
                    {
                        Textbox_Control_Status_Prinf("Get Version Number Success.");
                        Version_Flag = true;
                        char[] TempArray = Tag_Infor.Tag_USER_VER.ToCharArray();
                        string TempPatch = "", TempMinor = "", TempMajor = "";
                        Tag_Infor.Tag_USER_VER = "";
                        for (int i = 0; i < TempArray.Length; i++)
                        {
                            switch (i)
                            {
                                case 4:
                                case 5:
                                case 6:
                                case 7:
                                    TempMajor += System.Convert.ToString(TempArray[i]);
                                    break;

                                case 8:
                                case 9:
                                case 10:
                                case 11:
                                    TempMinor += System.Convert.ToString(TempArray[i]);
                                    break;

                                case 12:
                                case 13:
                                case 14:
                                case 15:
                                    TempPatch += System.Convert.ToString(TempArray[i]);
                                    break;
                            }
                        }
                        TEXTBOX_VERSION_NUMBER.Text = System.Convert.ToString(Convert.ToInt32(TempMajor, 2)) + "." +
                                                      System.Convert.ToString(Convert.ToInt32(TempMinor, 2)) + "." +
                                                      System.Convert.ToString(Convert.ToInt32(TempPatch, 2));
                    }
                    else
                    {
                        Textbox_Control_Status_Prinf("Get Version Number Failed.");
                        TEXTBOX_CONTROL_PC_EPC_CRC.Text = "";
                        TEXTBOX_CONTROL_TID_SN.Text = "";
                        Change_Contorl_Functions_Status();
                        Close_Contorl_Functions_Button();
                        return;
                    }
                }
            }

            if (LED_Flag == true && Battery_Flag == true && Antenna_Flag == true && Version_Flag == true)
            {
                Tag_Infor.Tag_ACCESS_FLAG = true;
                Change_Contorl_Functions_Status();
            }
            else
            {
                Tag_Infor.Tag_ACCESS_FLAG = false;
                Change_Contorl_Functions_Status();
            }


            /*
            //Control Version
            flags = 5;  //5: Get "USER_VER"
            parameters.readCmdParms.offset = 127;  //7Fh

            Result = LakeChabotReader.MANAGED_ACCESS.CB2_TAG_READ(parameters, flags, Tag_Infor);

            if (Result.OK != Result)
            {
                Textbox_Control_Status_Prinf("Read Version Number Command Failed.");
            }
            else
            {
                if (Tag_Infor.Tag_Error == 1 || Tag_Infor.Tag_Error == 2 || Tag_Infor.Tag_Error == 3)
                {
                    Textbox_Control_Status_Prinf("Get Version Number Failed - Non CB-2 Tag.");
                    Tag_Infor.Tag_ACCESS_FLAG = false;
                    Change_Contorl_Functions_Status();
                    return;
                }
                else
                {
                    if (Tag_Infor.Tag_ACCESS_FLAG == true)
                    {
                        Change_Contorl_Functions_Status();
                        char[] TempArray = Tag_Infor.Tag_USER_VER.ToCharArray();
                        string TempPatch = "", TempMinor = "", TempMajor = "";
                        Tag_Infor.Tag_USER_VER = "";
                        for (int i = 0; i < TempArray.Length; i++)
                        {
                            switch (i)
                            {
                                case 4:
                                case 5:
                                case 6:
                                case 7:
                                    TempMajor += System.Convert.ToString(TempArray[i]);
                                    break;

                                case 8:
                                case 9:
                                case 10:
                                case 11:
                                    TempMinor += System.Convert.ToString(TempArray[i]);
                                    break;

                                case 12:
                                case 13:
                                case 14:
                                case 15:
                                    TempPatch += System.Convert.ToString(TempArray[i]);
                                    break;
                            }
                        }
                        TEXTBOX_VERSION_NUMBER.Text = System.Convert.ToString(Convert.ToInt32(TempMajor, 2)) + "." +
                                                      System.Convert.ToString(Convert.ToInt32(TempMinor, 2)) + "." +
                                                      System.Convert.ToString(Convert.ToInt32(TempPatch, 2));
                    }
                    else
                    {
                        Textbox_Control_Status_Prinf("Get Version Number Failed.");
                        TEXTBOX_CONTROL_PC_EPC_CRC.Text = "";
                        TEXTBOX_CONTROL_TID_SN.Text = "";
                        Change_Contorl_Functions_Status();
                    }
                }
            }
            */
            //End by FJ for getting button to read status of CB-2 tag control function, 2017/12/07
            Clean_Select_Critrtia_Mask();
        }

        //Add bu FJ for set led function when click set led button
        private void Button_Led_Set_Click(object sender, EventArgs e)
        {
            string TempValue;
            byte flags = 0;
            Result Result = Result.OK;

            Edit_Select_Critrtia_Mask(Tag_Infor.Tag_TID);

            TempValue = "1";
            TempValue += Convert.ToString(Convert.ToInt32(NUMERICUPDOWN_LED_BLINK.Value), 2).PadLeft(13, '0');
            TempValue += Convert.ToString(COMBOBOX_COLOR.SelectedIndex, 2).PadLeft(2, '0');

            WriteParms parameters = new WriteParms();
            WriteSequentialParms writeParameters = new WriteSequentialParms();
            writeParameters.bank = MemoryBank.USER;
            writeParameters.offset = 130;  //82h
            writeParameters.pData = new ushort[2];
            writeParameters.pData[0] = Convert.ToUInt16(Convert.ToInt32(TempValue, 2));
            writeParameters.pData[1] = 0;
            writeParameters.count = 1;
            parameters.accessPassword = 0;
            parameters.writeParms = writeParameters;
            parameters.writeType = WriteType.SEQUENTIAL;
            parameters.common.callback = new rfid.CallbackDelegate(this._reader.MyCallback);

            parameters.common.strcTagFlag.RetryCount = retryCount;
            parameters.common.strcTagFlag.PostMatchFlag = 0;
            parameters.common.strcTagFlag.SelectOpsFlag = 1;
            parameters.common.OpMode = RadioOperationMode.NONCONTINUOUS;

            parameters.common.tagFlag = false;
            Array.Clear(parameters.common.bufMTII, 0, parameters.common.bufMTII.Length);
            Array.Clear(parameters.common.bufMTIA, 0, parameters.common.bufMTIA.Length);

            Result = LakeChabotReader.MANAGED_ACCESS.API_l8K6CTagWrite(parameters, flags);
            Tag_Infor.Tag_ACCESS_FLAG = parameters.common.tagFlag;

            if (Result.OK != Result)
            {
                Textbox_Control_Status_Prinf("Set LED Data Command Failed.");
                //Add by FJ for getting button to read status of CB-2 tag control function, 2017/12/07
                Tag_Infor.Tag_ACCESS_FLAG = false;
                Change_Contorl_Functions_Status();
                Close_Contorl_Functions_Button();
                //End by FJ for getting button to read status of CB-2 tag control function, 2017/12/07
            }

            if (Tag_Infor.Tag_ACCESS_FLAG == true)
            {
                //Chesk USER_CTRL_LED，Value = 0(Complete)、1(In Progress)
                Thread.Sleep(1000);
                flags = 2; //2: Get "USER_LED"
                ReadParms parameters_Check = new ReadParms();
                parameters_Check.readCmdParms.bank = MemoryBank.USER;
                parameters_Check.readCmdParms.offset = 130;  //82h
                parameters_Check.readCmdParms.count = 1;
                parameters_Check.common.strcTagFlag.RetryCount = retryCount;
                parameters_Check.common.strcTagFlag.SelectOpsFlag = 1;
                parameters_Check.common.strcTagFlag.PostMatchFlag = 0;
                parameters_Check.accessPassword = _tagAccessData.accessPassword;
                parameters_Check.common.callback = new rfid.CallbackDelegate(this._reader.MyCallback);
                parameters_Check.common.OpMode = RadioOperationMode.NONCONTINUOUS;

                parameters_Check.common.tagFlag = false;
                Array.Clear(parameters_Check.common.bufMTII, 0, parameters_Check.common.bufMTII.Length);
                Array.Clear(parameters_Check.common.bufMTIA, 0, parameters_Check.common.bufMTIA.Length);

                if (Result.OK != LakeChabotReader.MANAGED_ACCESS.API_l8K6CTagRead(parameters_Check, flags))
                {
                    Textbox_Control_Status_Prinf("Read Check Value Command Failed.");
                    //Add by FJ for getting button to read status of CB-2 tag control function, 2017/12/07
                    Tag_Infor.Tag_ACCESS_FLAG = false;
                    Change_Contorl_Functions_Status();
                    Close_Contorl_Functions_Button();
                    //End by FJ for getting button to read status of CB-2 tag control function, 2017/12/07
                }

                analysis_TagAccessPkt(parameters_Check, flags, parameters_Check.common.bufMTII, parameters_Check.common.bufMTIA);

                char[] TempArray = Tag_Infor.Tag_USER_LED.ToCharArray();
                string TempLED = "";
                Tag_Infor.Tag_USER_LED = "";
                TempLED = System.Convert.ToString(TempArray[0]);

                if (Tag_Infor.Tag_ACCESS_FLAG == true)
                {
                    if (Convert.ToInt32(TempLED, 2) == 0)
                        Textbox_Control_Status_Prinf("Set LED Data Success.");
                    else
                        Textbox_Control_Status_Prinf("Set LED Data In Progress.");
                }
                else
                {
                    Textbox_Control_Status_Prinf("Set LED Data Completed, But Check Status Failed.");
                }
            }
            else
            {
                Textbox_Control_Status_Prinf("Set LED Data Failed.");
                TEXTBOX_CONTROL_PC_EPC_CRC.Text = "";
                TEXTBOX_CONTROL_TID_SN.Text = "";
                Change_Contorl_Functions_Status();
                Close_Contorl_Functions_Button(); //Add by FJ for getting button to read status of CB-2 tag control function, 2017/12/07
            }

            Clean_Select_Critrtia_Mask();
            this._reader.CB2_ProcessQueuedPackets(); //Add by FJ for fixing packet parsing mechanism for operating CB-2, 2017/12/07
        }

        //Add by FJ for getting button to read status of CB-2 tag control function, 2017/12/07
        private void Button_Led_Get_Click(object sender, EventArgs e)
        {
            string TempValue;
            byte flags;
            Result Result = Result.OK;
            Edit_Select_Critrtia_Mask(Tag_Infor.Tag_TID);

            //Control LED
            //Should to set wrong value, then repeat read LED bank to get current value.	
            flags = 0;
            TempValue = "1000000000000011";

            WriteParms currvalueParameters = new WriteParms();
            WriteSequentialParms writeParameters = new WriteSequentialParms();
            writeParameters.bank = MemoryBank.USER;
            writeParameters.offset = 130;  //82h
            writeParameters.pData = new ushort[2];
            writeParameters.pData[0] = Convert.ToUInt16(Convert.ToInt32(TempValue, 2));
            writeParameters.pData[1] = 0;
            writeParameters.count = 1;
            currvalueParameters.accessPassword = 0;
            currvalueParameters.writeParms = writeParameters;
            currvalueParameters.writeType = WriteType.SEQUENTIAL;
            currvalueParameters.common.callback = new rfid.CallbackDelegate(this._reader.MyCallback);

            currvalueParameters.common.strcTagFlag.RetryCount = retryCount;
            currvalueParameters.common.strcTagFlag.PostMatchFlag = 0;
            currvalueParameters.common.strcTagFlag.SelectOpsFlag = 1;
            currvalueParameters.common.OpMode = RadioOperationMode.NONCONTINUOUS;

            currvalueParameters.common.tagFlag = false;
            Array.Clear(currvalueParameters.common.bufMTII, 0, currvalueParameters.common.bufMTII.Length);
            Array.Clear(currvalueParameters.common.bufMTIA, 0, currvalueParameters.common.bufMTIA.Length);

            Result = LakeChabotReader.MANAGED_ACCESS.API_l8K6CTagWrite(currvalueParameters, flags);
            Tag_Infor.Tag_ACCESS_FLAG = currvalueParameters.common.tagFlag;

            if (Result.OK != Result)
                Textbox_Control_Status_Prinf("Set LED Value Wrong Command Failed.");

            flags = 2;  //2: Get "USER_LED"
            ReadParms parameters = new ReadParms();
            parameters.readCmdParms.bank = MemoryBank.USER;
            parameters.readCmdParms.offset = 130;  //82h
            parameters.readCmdParms.count = 1;
            parameters.common.strcTagFlag.RetryCount = retryCount;
            parameters.common.strcTagFlag.SelectOpsFlag = 1;
            parameters.common.strcTagFlag.PostMatchFlag = 0;
            parameters.accessPassword = _tagAccessData.accessPassword;
            parameters.common.callback = new rfid.CallbackDelegate(this._reader.MyCallback);
            parameters.common.OpMode = RadioOperationMode.NONCONTINUOUS;
            Tag_Infor.Tag_USER_LED = "";

            System.Threading.Thread.Sleep(1000);

            parameters.common.tagFlag = false;
            Array.Clear(parameters.common.bufMTII, 0, parameters.common.bufMTII.Length);
            Array.Clear(parameters.common.bufMTIA, 0, parameters.common.bufMTIA.Length);

            if (Tag_Infor.Tag_ACCESS_FLAG == true)
            {
                ds1 = DateTime.Now;
                ts1 = new TimeSpan(ds1.Ticks);

                while (true)
                {
                    Result = LakeChabotReader.MANAGED_ACCESS.API_l8K6CTagRead(parameters, flags);

                    if (Result.OK != Result)
                    {
                        Textbox_Control_Status_Prinf("Read LED Value Command Failed.");
                        break;
                    }
                    else
                    {
                        analysis_TagAccessPkt(parameters, flags, parameters.common.bufMTII, parameters.common.bufMTIA);
                        if (Tag_Infor.Tag_Error == 1 || Tag_Infor.Tag_Error == 2 || Tag_Infor.Tag_Error == 3)
                        {
                            Textbox_Control_Status_Prinf("Get LED Value Failed - Read Tag Error.");
                            break;
                        }
                        else
                        {
                            if (Tag_Infor.Tag_ACCESS_FLAG == true)
                            {
                                char[] TempArray = Tag_Infor.Tag_USER_LED.ToCharArray();
                                string TempLED_Color = "", TempLED_Blink = "", Check_Status = "";
                                Tag_Infor.Tag_USER_LED = "";
                                for (int i = 0; i < TempArray.Length; i++)
                                {
                                    switch (i)
                                    {
                                        case 0:
                                            Check_Status = System.Convert.ToString(TempArray[i]);
                                            break;

                                        case 14:
                                        case 15:
                                            TempLED_Color += System.Convert.ToString(TempArray[i]);
                                            break;

                                        default:
                                            TempLED_Blink += System.Convert.ToString(TempArray[i]);
                                            break;
                                    }
                                }
                                if (Convert.ToInt32(Check_Status, 2) == 0)
                                {
                                    Textbox_Control_Status_Prinf("Get LED Value Success.");
                                    COMBOBOX_COLOR.SelectedIndex = Convert.ToInt32(TempLED_Color, 2);
                                    NUMERICUPDOWN_LED_BLINK.Value = Convert.ToInt32(TempLED_Blink, 2);
                                    break;
                                }
                                else
                                {
                                    ds2 = DateTime.Now;
                                    ts2 = new TimeSpan(ds2.Ticks);
                                    TimeSpan ts = ts1.Subtract(ts2).Duration();
                                    if (ts.Seconds >= 1)
                                    {
                                        Textbox_Control_Status_Prinf("Get LED Value Timeout.");
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                Textbox_Control_Status_Prinf("Get LED Value Failed.");
                                break;
                            }
                        }
                    }
                }
            }

            Clean_Select_Critrtia_Mask();
            this._reader.CB2_ProcessQueuedPackets(); //Add by FJ for fixing packet parsing mechanism for operating CB-2, 2017/12/07
        }
        //End by FJ for getting button to read status of CB-2 tag control function, 2017/12/07

        //Add bu FJ for set battery function when click set battery button
        private void Button_Battery_Set_Click(object sender, EventArgs e)
        {
            string TempValue;
            byte flags = 0;
            Result Result = Result.OK;

            Edit_Select_Critrtia_Mask(Tag_Infor.Tag_TID);

            TempValue = "1";
            TempValue += "00000000000000";
            TempValue += COMBOBOX_BATTERY_ASSIST.SelectedIndex;

            WriteParms parameters = new WriteParms();
            WriteSequentialParms writeParameters = new WriteSequentialParms();
            writeParameters.bank = MemoryBank.USER;
            writeParameters.offset = 129;  //81h
            writeParameters.pData = new ushort[2];
            writeParameters.pData[0] = Convert.ToUInt16(Convert.ToInt32(TempValue, 2));
            writeParameters.pData[1] = 0;
            writeParameters.count = 1;
            parameters.accessPassword = 0;
            parameters.writeParms = writeParameters;
            parameters.writeType = WriteType.SEQUENTIAL;
            parameters.common.callback = new rfid.CallbackDelegate(this._reader.MyCallback);

            parameters.common.strcTagFlag.RetryCount = retryCount;
            parameters.common.strcTagFlag.PostMatchFlag = 0;
            parameters.common.strcTagFlag.SelectOpsFlag = 1;
            parameters.common.OpMode = RadioOperationMode.NONCONTINUOUS;

            parameters.common.tagFlag = false;
            Array.Clear(parameters.common.bufMTII, 0, parameters.common.bufMTII.Length);
            Array.Clear(parameters.common.bufMTIA, 0, parameters.common.bufMTIA.Length);

            Result = LakeChabotReader.MANAGED_ACCESS.API_l8K6CTagWrite(parameters, flags);
            Tag_Infor.Tag_ACCESS_FLAG = parameters.common.tagFlag;

            if (Result.OK != Result)
            {
                Textbox_Control_Status_Prinf("Set Battery Data Command Failed.");
                //Add by FJ for getting button to read status of CB-2 tag control function, 2017/12/07
                Tag_Infor.Tag_ACCESS_FLAG = false;
                Change_Contorl_Functions_Status();
                Close_Contorl_Functions_Button();
                //End by FJ for getting button to read status of CB-2 tag control function, 2017/12/07
            }

            if (Tag_Infor.Tag_ACCESS_FLAG == true)
            {
                //Chesk USER_CTRL_BAT，Value = 0(Complete)、1(In Progress)
                Thread.Sleep(1000);
                flags = 3; //3: Get "USER_BAT"
                ReadParms parameters_Check = new ReadParms();
                parameters_Check.readCmdParms.bank = MemoryBank.USER;
                parameters_Check.readCmdParms.offset = 129;  //81h
                parameters_Check.readCmdParms.count = 1;
                parameters_Check.common.strcTagFlag.RetryCount = retryCount;
                parameters_Check.common.strcTagFlag.SelectOpsFlag = 1;
                parameters_Check.common.strcTagFlag.PostMatchFlag = 0;
                parameters_Check.accessPassword = _tagAccessData.accessPassword;
                parameters_Check.common.callback = new rfid.CallbackDelegate(this._reader.MyCallback);
                parameters_Check.common.OpMode = RadioOperationMode.NONCONTINUOUS;

                parameters_Check.common.tagFlag = false;
                Array.Clear(parameters_Check.common.bufMTII, 0, parameters_Check.common.bufMTII.Length);
                Array.Clear(parameters_Check.common.bufMTIA, 0, parameters_Check.common.bufMTIA.Length);

                if (Result.OK != LakeChabotReader.MANAGED_ACCESS.API_l8K6CTagRead(parameters_Check, flags))
                {
                    Textbox_Control_Status_Prinf("Read Check Value Command Failed.");
                    //Add by FJ for getting button to read status of CB-2 tag control function, 2017/12/07
                    Tag_Infor.Tag_ACCESS_FLAG = false;
                    Change_Contorl_Functions_Status();
                    Close_Contorl_Functions_Button();
                    //End by FJ for getting button to read status of CB-2 tag control function, 2017/12/07
                }

                analysis_TagAccessPkt(parameters_Check, flags, parameters_Check.common.bufMTII, parameters_Check.common.bufMTIA);

                char[] TempArray = Tag_Infor.Tag_USER_BAT.ToCharArray();
                string TempBAT = "";
                Tag_Infor.Tag_USER_BAT = "";
                TempBAT = System.Convert.ToString(TempArray[0]);

                if (Tag_Infor.Tag_ACCESS_FLAG == true)
                {
                    if (Convert.ToInt32(TempBAT, 2) == 0)
                        Textbox_Control_Status_Prinf("Set Battery Data Success.");
                    else
                        Textbox_Control_Status_Prinf("Set Battery Data In Progress.");
                }
                else
                {
                    Textbox_Control_Status_Prinf("Set Battery Data Completed, But Check Status Failed.");
                }
            }
            else
            {
                Textbox_Control_Status_Prinf("Set Battery Data Failed.");
                TEXTBOX_CONTROL_PC_EPC_CRC.Text = "";
                TEXTBOX_CONTROL_TID_SN.Text = "";
                Change_Contorl_Functions_Status();
                Close_Contorl_Functions_Button(); //Add by FJ for getting button to read status of CB-2 tag control function, 2017/12/07
            }

            Clean_Select_Critrtia_Mask();
            this._reader.CB2_ProcessQueuedPackets(); //Add by FJ for fixing packet parsing mechanism for operating CB-2, 2017/12/07

        }

        //Add by FJ for getting button to read status of CB-2 tag control function, 2017/12/07
        private void Button_Battery_Get_Click(object sender, EventArgs e)
        {
            string TempValue;
            byte flags;
            Result Result = Result.OK;
            Edit_Select_Critrtia_Mask(Tag_Infor.Tag_TID);

            //Control Battery
            //Should to set wrong value, then repeat read Battery bank to get current value.
            flags = 0;
            TempValue = "1000000000000010";

            WriteParms currvalueParameters = new WriteParms();
            WriteSequentialParms writeParameters = new WriteSequentialParms();
            writeParameters.bank = MemoryBank.USER;
            writeParameters.offset = 129;  //81h
            writeParameters.pData = new ushort[2];
            writeParameters.pData[0] = Convert.ToUInt16(Convert.ToInt32(TempValue, 2));
            writeParameters.pData[1] = 0;
            writeParameters.count = 1;
            currvalueParameters.accessPassword = 0;
            currvalueParameters.writeParms = writeParameters;
            currvalueParameters.writeType = WriteType.SEQUENTIAL;
            currvalueParameters.common.callback = new rfid.CallbackDelegate(this._reader.MyCallback);

            currvalueParameters.common.strcTagFlag.RetryCount = retryCount;
            currvalueParameters.common.strcTagFlag.PostMatchFlag = 0;
            currvalueParameters.common.strcTagFlag.SelectOpsFlag = 1;
            currvalueParameters.common.OpMode = RadioOperationMode.NONCONTINUOUS;

            currvalueParameters.common.tagFlag = false;
            Array.Clear(currvalueParameters.common.bufMTII, 0, currvalueParameters.common.bufMTII.Length);
            Array.Clear(currvalueParameters.common.bufMTIA, 0, currvalueParameters.common.bufMTIA.Length);

            Result = LakeChabotReader.MANAGED_ACCESS.API_l8K6CTagWrite(currvalueParameters, flags);
            Tag_Infor.Tag_ACCESS_FLAG = currvalueParameters.common.tagFlag;

            if (Result.OK != Result)
                Textbox_Control_Status_Prinf("Set Battery Wrong Value Command Failed.");

            flags = 3;  //3: Get "USER_BAT"
            ReadParms parameters = new ReadParms();
            parameters.readCmdParms.bank = MemoryBank.USER;
            parameters.readCmdParms.offset = 129;  //81h
            parameters.readCmdParms.count = 1;
            parameters.common.strcTagFlag.RetryCount = retryCount;
            parameters.common.strcTagFlag.SelectOpsFlag = 1;
            parameters.common.strcTagFlag.PostMatchFlag = 0;
            parameters.accessPassword = _tagAccessData.accessPassword;
            parameters.common.callback = new rfid.CallbackDelegate(this._reader.MyCallback);
            parameters.common.OpMode = RadioOperationMode.NONCONTINUOUS;

            System.Threading.Thread.Sleep(1000);

            parameters.common.tagFlag = false;
            Array.Clear(parameters.common.bufMTII, 0, parameters.common.bufMTII.Length);
            Array.Clear(parameters.common.bufMTIA, 0, parameters.common.bufMTIA.Length);

            if (Tag_Infor.Tag_ACCESS_FLAG == true)
            {
                ds1 = DateTime.Now;
                ts1 = new TimeSpan(ds1.Ticks);

                while (true)
                {
                    Result = LakeChabotReader.MANAGED_ACCESS.API_l8K6CTagRead(parameters, flags);

                    if (Result.OK != Result)
                    {
                        Textbox_Control_Status_Prinf("Read Battery Value Command Failed.");
                        break;
                    }
                    else
                    {
                        analysis_TagAccessPkt(parameters, flags, parameters.common.bufMTII, parameters.common.bufMTIA);
                        if (Tag_Infor.Tag_Error == 1 || Tag_Infor.Tag_Error == 2 || Tag_Infor.Tag_Error == 3)
                        {
                            Textbox_Control_Status_Prinf("Get Battery Value Failed - Read Tag Error.");
                            break;
                        }
                        else
                        {
                            if (Tag_Infor.Tag_ACCESS_FLAG == true)
                            {
                                char[] TempArray = Tag_Infor.Tag_USER_BAT.ToCharArray();
                                string TempBAT = "", Check_Status = "";
                                Tag_Infor.Tag_USER_BAT = "";
                                for (int i = 0; i < TempArray.Length; i++)
                                {
                                    switch (i)
                                    {
                                        case 0:
                                            Check_Status = System.Convert.ToString(TempArray[i]);
                                            break;

                                        case 15:
                                            TempBAT = System.Convert.ToString(TempArray[i]);
                                            break;
                                    }
                                }
                                if (Convert.ToInt32(Check_Status, 2) == 0)
                                {
                                    Textbox_Control_Status_Prinf("Get Battery Value Success.");
                                    COMBOBOX_BATTERY_ASSIST.SelectedIndex = Convert.ToInt32(TempBAT, 2);
                                    break;
                                }
                                else
                                {
                                    ds2 = DateTime.Now;
                                    ts2 = new TimeSpan(ds2.Ticks);
                                    TimeSpan ts = ts1.Subtract(ts2).Duration();
                                    if (ts.Seconds >= 1)
                                    {
                                        Textbox_Control_Status_Prinf("Get Battery Value Timeout.");
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                Textbox_Control_Status_Prinf("Get Battery Value Failed.");
                                break;
                            }
                        }
                    }
                }
            }

            Clean_Select_Critrtia_Mask();
            this._reader.CB2_ProcessQueuedPackets(); //Add by FJ for fixing packet parsing mechanism for operating CB-2, 2017/12/07
        }
        //End by FJ for getting button to read status of CB-2 tag control function, 2017/12/07

        //Add by FJ for set antenna function when click set antenna button
        private void Button_Antenna_Set_Click(object sender, EventArgs e)
        {
            string TempValue;
            byte flags = 0;
            Result Result = Result.OK;

            Edit_Select_Critrtia_Mask(Tag_Infor.Tag_TID);

            TempValue = "1";
            TempValue += "0000000000000";
            TempValue += Convert.ToString(COMBOBOX_ANTENNA_SELECT.SelectedIndex, 2).PadLeft(2, '0');

            WriteParms parameters = new WriteParms();
            WriteSequentialParms writeParameters = new WriteSequentialParms();
            writeParameters.bank = MemoryBank.USER;
            writeParameters.offset = 128;  //80h
            writeParameters.pData = new ushort[2];
            writeParameters.pData[0] = Convert.ToUInt16(Convert.ToInt32(TempValue, 2));
            writeParameters.pData[1] = 0;
            writeParameters.count = 1;
            parameters.accessPassword = 0;
            parameters.writeParms = writeParameters;
            parameters.writeType = WriteType.SEQUENTIAL;
            parameters.common.callback = new rfid.CallbackDelegate(this._reader.MyCallback);

            parameters.common.strcTagFlag.RetryCount = retryCount;
            parameters.common.strcTagFlag.PostMatchFlag = 0;
            parameters.common.strcTagFlag.SelectOpsFlag = 1;
            parameters.common.OpMode = RadioOperationMode.NONCONTINUOUS;

            parameters.common.tagFlag = false;
            Array.Clear(parameters.common.bufMTII, 0, parameters.common.bufMTII.Length);
            Array.Clear(parameters.common.bufMTIA, 0, parameters.common.bufMTIA.Length);

            Result = LakeChabotReader.MANAGED_ACCESS.API_l8K6CTagWrite(parameters, flags);
            Tag_Infor.Tag_ACCESS_FLAG = parameters.common.tagFlag;

            if (Result.OK != Result)
            {
                Textbox_Control_Status_Prinf("Set Antenna Data Command Failed.");
                //Add by FJ for getting button to read status of CB-2 tag control function, 2017/12/07
                Tag_Infor.Tag_ACCESS_FLAG = false;
                Change_Contorl_Functions_Status();
                Close_Contorl_Functions_Button();
                //End by FJ for getting button to read status of CB-2 tag control function, 2017/12/07
            }

            if (Tag_Infor.Tag_ACCESS_FLAG == true)
            {
                //Chesk USER_CTRL_ANT，Value = 0(Complete)、1(In Progress)
                Thread.Sleep(1000);
                flags = 4; //4: Get "USER_ANT"
                ReadParms parameters_Check = new ReadParms();
                parameters_Check.readCmdParms.bank = MemoryBank.USER;
                parameters_Check.readCmdParms.offset = 128;  //80h
                parameters_Check.readCmdParms.count = 1;
                parameters_Check.common.strcTagFlag.RetryCount = retryCount;
                parameters_Check.common.strcTagFlag.SelectOpsFlag = 1;
                parameters_Check.common.strcTagFlag.PostMatchFlag = 0;
                parameters_Check.accessPassword = _tagAccessData.accessPassword;
                parameters_Check.common.callback = new rfid.CallbackDelegate(this._reader.MyCallback);
                parameters_Check.common.OpMode = RadioOperationMode.NONCONTINUOUS;

                parameters_Check.common.tagFlag = false;
                Array.Clear(parameters_Check.common.bufMTII, 0, parameters_Check.common.bufMTII.Length);
                Array.Clear(parameters_Check.common.bufMTIA, 0, parameters_Check.common.bufMTIA.Length);

                if (Result.OK != LakeChabotReader.MANAGED_ACCESS.API_l8K6CTagRead(parameters_Check, flags))
                {
                    Textbox_Control_Status_Prinf("Read Check Value Command Failed.");
                    //Add by FJ for getting button to read status of CB-2 tag control function, 2017/12/07
                    Tag_Infor.Tag_ACCESS_FLAG = false;
                    Change_Contorl_Functions_Status();
                    Close_Contorl_Functions_Button();
                    //End by FJ for getting button to read status of CB-2 tag control function, 2017/12/07
                }

                analysis_TagAccessPkt(parameters_Check, flags, parameters_Check.common.bufMTII, parameters_Check.common.bufMTIA);

                char[] TempArray = Tag_Infor.Tag_USER_ANT.ToCharArray();
                string TempANT = "";
                Tag_Infor.Tag_USER_ANT = "";
                TempANT = System.Convert.ToString(TempArray[0]);

                if (Tag_Infor.Tag_ACCESS_FLAG == true)
                {
                    if (Convert.ToInt32(TempANT, 2) == 0)
                        Textbox_Control_Status_Prinf("Set Antenna Data Success.");
                    else
                        Textbox_Control_Status_Prinf("Set Antenna Data In Progress.");
                }
                else
                {
                    Textbox_Control_Status_Prinf("Set Antenna Data Completed, But Check Status Failed.");
                }
            }
            else
            {
                Textbox_Control_Status_Prinf("Set Antenna Data Failed.");
                TEXTBOX_CONTROL_PC_EPC_CRC.Text = "";
                TEXTBOX_CONTROL_TID_SN.Text = "";
                Change_Contorl_Functions_Status();
                Close_Contorl_Functions_Button(); //Add by FJ for getting button to read status of CB-2 tag control function, 2017/12/07
            }

            Clean_Select_Critrtia_Mask();
            this._reader.CB2_ProcessQueuedPackets(); //Add by FJ for fixing packet parsing mechanism for operating CB-2, 2017/12/07

        }

        //Add by FJ for getting button to read status of CB-2 tag control function, 2017/12/07
        private void Button_Antenna_Get_Click(object sender, EventArgs e)
        {
            string TempValue;
            byte flags;
            Result Result = Result.OK;
            Edit_Select_Critrtia_Mask(Tag_Infor.Tag_TID);

            //Control Antenna
            //Should to set wrong value, then repeat read Antenna bank to get current value.
            flags = 0;
            TempValue = "1000000000000011";

            WriteParms currvalueParameters = new WriteParms();
            WriteSequentialParms writeParameters = new WriteSequentialParms();
            writeParameters.bank = MemoryBank.USER;
            writeParameters.offset = 128;  //80h
            writeParameters.pData = new ushort[2];
            writeParameters.pData[0] = Convert.ToUInt16(Convert.ToInt32(TempValue, 2));
            writeParameters.pData[1] = 0;
            writeParameters.count = 1;
            currvalueParameters.accessPassword = 0;
            currvalueParameters.writeParms = writeParameters;
            currvalueParameters.writeType = WriteType.SEQUENTIAL;
            currvalueParameters.common.callback = new rfid.CallbackDelegate(this._reader.MyCallback);

            currvalueParameters.common.strcTagFlag.RetryCount = retryCount;
            currvalueParameters.common.strcTagFlag.PostMatchFlag = 0;
            currvalueParameters.common.strcTagFlag.SelectOpsFlag = 1;
            currvalueParameters.common.OpMode = RadioOperationMode.NONCONTINUOUS;

            currvalueParameters.common.tagFlag = false;
            Array.Clear(currvalueParameters.common.bufMTII, 0, currvalueParameters.common.bufMTII.Length);
            Array.Clear(currvalueParameters.common.bufMTIA, 0, currvalueParameters.common.bufMTIA.Length);

            Result = LakeChabotReader.MANAGED_ACCESS.API_l8K6CTagWrite(currvalueParameters, flags);
            Tag_Infor.Tag_ACCESS_FLAG = currvalueParameters.common.tagFlag;

            if (Result.OK != Result)
                Textbox_Control_Status_Prinf("Set Antenna Wrong Value Command Failed.");

            flags = 4;  //4: Get "USER_ANT"
            ReadParms parameters = new ReadParms();
            parameters.readCmdParms.bank = MemoryBank.USER;
            parameters.readCmdParms.offset = 128;  //80h
            parameters.readCmdParms.count = 1;
            parameters.common.strcTagFlag.RetryCount = retryCount;
            parameters.common.strcTagFlag.SelectOpsFlag = 1;
            parameters.common.strcTagFlag.PostMatchFlag = 0;
            parameters.accessPassword = _tagAccessData.accessPassword;
            parameters.common.callback = new rfid.CallbackDelegate(this._reader.MyCallback);
            parameters.common.OpMode = RadioOperationMode.NONCONTINUOUS;

            System.Threading.Thread.Sleep(1000);

            parameters.common.tagFlag = false;
            Array.Clear(parameters.common.bufMTII, 0, parameters.common.bufMTII.Length);
            Array.Clear(parameters.common.bufMTIA, 0, parameters.common.bufMTIA.Length);

            if (Tag_Infor.Tag_ACCESS_FLAG == true)
            {

                ds1 = DateTime.Now;
                ts1 = new TimeSpan(ds1.Ticks);

                while (true)
                {
                    Result = LakeChabotReader.MANAGED_ACCESS.API_l8K6CTagRead(parameters, flags);

                    if (Result.OK != Result)
                    {
                        Textbox_Control_Status_Prinf("Read Antenna Value Command Failed.");
                        break;
                    }
                    else
                    {
                        analysis_TagAccessPkt(parameters, flags, parameters.common.bufMTII, parameters.common.bufMTIA);
                        if (Tag_Infor.Tag_Error == 1 || Tag_Infor.Tag_Error == 2 || Tag_Infor.Tag_Error == 3)
                        {
                            Textbox_Control_Status_Prinf("Get Antenna Value Failed - Read Tag Error.");
                            break;
                        }
                        else
                        {
                            if (Tag_Infor.Tag_ACCESS_FLAG == true)
                            {
                                char[] TempArray = Tag_Infor.Tag_USER_ANT.ToCharArray();
                                string TempANT = "", Check_Status = "";
                                Tag_Infor.Tag_USER_ANT = "";
                                for (int i = 0; i < TempArray.Length; i++)
                                {
                                    switch (i)
                                    {
                                        case 0:
                                            Check_Status = System.Convert.ToString(TempArray[i]);
                                            break;

                                        case 14:
                                        case 15:
                                            TempANT += System.Convert.ToString(TempArray[i]);
                                            break;
                                    }
                                }

                                if (Convert.ToInt32(Check_Status, 2) == 0)
                                {
                                    Textbox_Control_Status_Prinf("Get Antenna Value Success.");
                                    COMBOBOX_ANTENNA_SELECT.SelectedIndex = Convert.ToInt32(TempANT, 2);
                                    break;
                                }
                                else
                                {
                                    ds2 = DateTime.Now;
                                    ts2 = new TimeSpan(ds2.Ticks);
                                    TimeSpan ts = ts1.Subtract(ts2).Duration();
                                    if (ts.Seconds >= 1)
                                    {
                                        Textbox_Control_Status_Prinf("Get Antenna Value Timeout.");
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                Textbox_Control_Status_Prinf("Get Antenna Value Failed.");
                                break;
                            }
                        }
                    }
                }
            }

            Clean_Select_Critrtia_Mask();
            this._reader.CB2_ProcessQueuedPackets(); //Add by FJ for fixing packet parsing mechanism for operating CB-2, 2017/12/07
        }
        //End by FJ for getting button to read status of CB-2 tag control function, 2017/12/07

        //Add by FJ for getting button to read status of CB-2 tag control function, 2017/12/07
        private void Close_Contorl_Functions_Button()
        {
            BUTTON_LED_SET.Enabled = false;
            BUTTON_LED_GET.Enabled = false;
            BUTTON_BATTERY_SET.Enabled = false;
            BUTTON_BATTERY_GET.Enabled = false;
            BUTTON_ANTENNA_SET.Enabled = false;
            BUTTON_ANTENNA_GET.Enabled = false;
        }
        //End by FJ for getting button to read status of CB-2 tag control function, 2017/12/07

        //Add by FJ go back led & battery & antenna & version initial value when inventory or set function faiked
        private void Change_Contorl_Functions_Status()
        {
            if (Tag_Infor.Tag_ACCESS_FLAG == true)
            {
                GROUPBOX_LED.Enabled = true;
                GROUPBOX_BATTERY.Enabled = true;
                GROUPBOX_ANTENNA.Enabled = true;
                BUTTON_LED_SET.Enabled = true;
                BUTTON_LED_GET.Enabled = true;
                BUTTON_BATTERY_SET.Enabled = true;
                BUTTON_BATTERY_GET.Enabled = true;
                BUTTON_ANTENNA_SET.Enabled = true;
                BUTTON_ANTENNA_GET.Enabled = true;
            }
            else
            {
                COMBOBOX_COLOR.SelectedIndex = 0;
                COMBOBOX_BATTERY_ASSIST.SelectedIndex = 0;
                COMBOBOX_ANTENNA_SELECT.SelectedIndex = 0;
                NUMERICUPDOWN_LED_BLINK.Value = 1;
                TEXTBOX_VERSION_NUMBER.Text = "";
                GROUPBOX_LED.Enabled = false;
                GROUPBOX_BATTERY.Enabled = false;
                GROUPBOX_ANTENNA.Enabled = false;
            }
        }

        //Add by FJ for disply the control status message
        private void Textbox_Control_Status_Prinf(String str)
        {
            TEXTBOX_CONTROL_STATUS.Text = TEXTBOX_CONTROL_STATUS.Text + str + Environment.NewLine;
            Control_TextBox_Refresh();
        }

        //Add by FJ for refresh the control status message
        private void Control_TextBox_Refresh()
        {
            TEXTBOX_CONTROL_STATUS.SelectionStart = TEXTBOX_CONTROL_STATUS.Text.Length;
            TEXTBOX_CONTROL_STATUS.ScrollToCaret();
            TEXTBOX_CONTROL_STATUS.Refresh();
        }

        //Add by FJ for clean the control status message
        private void Button_Control_Status_Clean(object sender, EventArgs e)
        {
            TEXTBOX_CONTROL_STATUS.Text = "";
        }

        //Add by FJ for edit the select critrtia mask value 
        private void Edit_Select_Critrtia_Mask(string Select_TID_Mask)
        {
            //Add by FJ for set Select Criteria rule
            Result Result = Result.OK;
            rfid.Structures.SelectCriteria retrievedCriteria = new rfid.Structures.SelectCriteria();
            this.selectCriteria = new SelectCriteria();
            this.selectCriteria.pCriteria = new SelectCriterion[CRITERIA_MAXIM];
            Result = LakeChabotReader.MANAGED_ACCESS.API_l8K6CGetSelectCriteria(ref retrievedCriteria);
            if (Result.OK != Result)
                MessageBox.Show("Read Select Criteria Rule Status Failed.");
            else
            {
                // Copy over incoming criteria...
                this.selectCriteria.countCriteria = retrievedCriteria.countCriteria;

                // Ref copy since safe creation in managed lib now...
                for (int index = 0; index < retrievedCriteria.countCriteria; ++index)
                {
                    this.selectCriteria.pCriteria[index] = retrievedCriteria.pCriteria[index];
                }

                // Fill in ( or zero out if call > 1 ) remaining criteria
                for (uint index = selectCriteria.countCriteria; index < CRITERIA_MAXIM; ++index)
                {
                    this.selectCriteria.pCriteria[index] = new rfid.Structures.SelectCriterion();
                }
            }

            if (Tag_Infor.Tag_ACCESS_FLAG == true)
            {
                string[] Select_Mask = Select_TID_Mask.Split('-');
                this.selectCriteria.countCriteria = (byte)1;
                this.selectCriteria.pCriteria[0].mask.offset = 48;
                this.selectCriteria.pCriteria[0].mask.bank = MemoryBank.TID;
                this.selectCriteria.pCriteria[0].mask.count = (byte)48;
                this.selectCriteria.pCriteria[0].mask.mask[0] = (byte)Convert.ToInt32(Select_Mask[0], 16);
                this.selectCriteria.pCriteria[0].mask.mask[1] = (byte)Convert.ToInt32(Select_Mask[1], 16);
                this.selectCriteria.pCriteria[0].mask.mask[2] = (byte)Convert.ToInt32(Select_Mask[2], 16);
                this.selectCriteria.pCriteria[0].mask.mask[3] = (byte)Convert.ToInt32(Select_Mask[3], 16);
                this.selectCriteria.pCriteria[0].mask.mask[4] = (byte)Convert.ToInt32(Select_Mask[4], 16);
                this.selectCriteria.pCriteria[0].mask.mask[5] = (byte)Convert.ToInt32(Select_Mask[5], 16);
                this.selectCriteria.pCriteria[0].action.target = Target.S2;
                this.selectCriteria.pCriteria[0].action.enableTruncate = (byte)0;
                this.selectCriteria.pCriteria[0].action.action = rfid.Constants.Action.DSLINVB_ASLINVA;
                Result = LakeChabotReader.MANAGED_ACCESS.API_l8K6CSetSelectCriteria(selectCriteria);
                if (Result.OK != Result)
                    MessageBox.Show("Set Select Criteria Rule Failed.");
            }
        }

        //Add by FJ for clean the select critrtia mask value
        private void Clean_Select_Critrtia_Mask()
        {
            //Add by FJ for set Select Criteria rule
            Result Result = Result.OK;
            rfid.Structures.SelectCriteria retrievedCriteria = new rfid.Structures.SelectCriteria();
            this.selectCriteria = new SelectCriteria();
            this.selectCriteria.pCriteria = new SelectCriterion[CRITERIA_MAXIM];
            Result = LakeChabotReader.MANAGED_ACCESS.API_l8K6CGetSelectCriteria(ref retrievedCriteria);
            if (Result.OK != Result)
                MessageBox.Show("Read Select Criteria Rule Status Failed.");
            else
            {
                // Copy over incoming criteria...
                this.selectCriteria.countCriteria = retrievedCriteria.countCriteria;

                // Ref copy since safe creation in managed lib now...
                for (int index = 0; index < retrievedCriteria.countCriteria; ++index)
                {
                    this.selectCriteria.pCriteria[index] = retrievedCriteria.pCriteria[index];
                }

                // Fill in ( or zero out if call > 1 ) remaining criteria
                for (uint index = selectCriteria.countCriteria; index < CRITERIA_MAXIM; ++index)
                {
                    this.selectCriteria.pCriteria[index] = new rfid.Structures.SelectCriterion();
                }
            }

            this.selectCriteria.countCriteria = (byte)1;
            this.selectCriteria.pCriteria[0].mask.offset = 48;
            this.selectCriteria.pCriteria[0].mask.bank = MemoryBank.TID;
            this.selectCriteria.pCriteria[0].mask.count = (byte)0;
            this.selectCriteria.pCriteria[0].mask.mask[0] = (byte)0;
            this.selectCriteria.pCriteria[0].mask.mask[1] = (byte)0;
            this.selectCriteria.pCriteria[0].mask.mask[2] = (byte)0;
            this.selectCriteria.pCriteria[0].mask.mask[3] = (byte)0;
            this.selectCriteria.pCriteria[0].mask.mask[4] = (byte)0;
            this.selectCriteria.pCriteria[0].mask.mask[5] = (byte)0;
            this.selectCriteria.pCriteria[0].action.target = Target.S2;
            this.selectCriteria.pCriteria[0].action.enableTruncate = (byte)0;
            this.selectCriteria.pCriteria[0].action.action = rfid.Constants.Action.DSLINVB_ASLINVA;
            Result = LakeChabotReader.MANAGED_ACCESS.API_l8K6CSetSelectCriteria(selectCriteria);
            if (Result.OK != Result)
                MessageBox.Show("Set Select Criteria Rule Failed.");
        }

        //Add by FJ go back initial value when close the tag access tool
        private void Form1_FormClosing(Object sender, FormClosingEventArgs e)
        {
            //In case windows is trying to shut down, don't hold the process up
            if (e.CloseReason == CloseReason.WindowsShutDown) return;

            if (this.DialogResult == DialogResult.Cancel)
            {
                // Assume that X has been clicked and act accordingly.
                // Confirm user wants to close

                //Add by FJ go back initial value of antenna
                AntennaPortConfig PortConfig = new AntennaPortConfig();
                Result Result = Result.OK;

                Result = LakeChabotReader.MANAGED_ACCESS.API_AntennaPortGetConfiguration(0, ref PortConfig);

                PortConfig.powerLevel = Original_Power_Level;
                PortConfig.physicalPort = (byte)Original_Physical_Port;
                //Add by FJ for implement recovery mechanism for CB-2 function, 2017/12/07
                PortConfig.numberInventoryCycles = Original_InventoryCycles;
                PortConfig.dwellTime = Original_dwellTime;
                //End by FJ for implement recovery mechanism for CB-2 function, 2017/12/07
                Result = LakeChabotReader.MANAGED_ACCESS.API_AntennaPortSetConfiguration(0, PortConfig);

                //Add by FJ go back initial value of Select Criteria rule
                rfid.Structures.SelectCriteria retrievedCriteria = new rfid.Structures.SelectCriteria();
                this.selectCriteria = new SelectCriteria();
                this.selectCriteria.pCriteria = new SelectCriterion[CRITERIA_MAXIM];
                Result = LakeChabotReader.MANAGED_ACCESS.API_l8K6CGetSelectCriteria(ref retrievedCriteria);
                if (Result.OK != Result)
                    MessageBox.Show("Read Select Criteria Rule Failed.");
                else
                {
                    // Copy over incoming criteria...
                    this.selectCriteria.countCriteria = retrievedCriteria.countCriteria;

                    // Ref copy since safe creation in managed lib now...
                    for (int index = 0; index < retrievedCriteria.countCriteria; ++index)
                    {
                        this.selectCriteria.pCriteria[index] = retrievedCriteria.pCriteria[index];
                    }

                    // Fill in ( or zero out if call > 1 ) remaining criteria
                    for (uint index = selectCriteria.countCriteria; index < CRITERIA_MAXIM; ++index)
                    {
                        this.selectCriteria.pCriteria[index] = new rfid.Structures.SelectCriterion();
                    }
                }

                this.selectCriteria.countCriteria = (byte)0;
                Result = LakeChabotReader.MANAGED_ACCESS.API_l8K6CSetSelectCriteria(selectCriteria);
                if (Result.OK != Result)
                    MessageBox.Show("Set Select Criteria Rule Failed.");

                //Add by FJ for implement recovery mechanism for CB-2 function, 2017/12/07
                //Recover initial value for algorithm
                switch (algorithm)
                {
                    case SingulationAlgorithm.DYNAMICQ:
                        Source_QueryParms parmsMasterDQ = new Source_QueryParms();
                        Result = parmsMasterDQ.loadForAlgorithm
                            (
                                LakeChabotReader.MANAGED_ACCESS,
                                (uint)_reader.ReaderHandle,
                                SingulationAlgorithm.DYNAMICQ
                            );
                        if (Result.OK != Result)
                            MessageBox.Show("Read Algorithm Rule Failed.");

                        Source_SingulationParametersDynamicQ parmsDynamicQ = (Source_SingulationParametersDynamicQ)parmsMasterDQ.SingulationAlgorithmParameters;

                        parmsMasterDQ.TagGroupSelected = Original_Selected;
                        parmsMasterDQ.TagGroupSession = Original_Session;
                        parmsMasterDQ.TagGroupTarget = Original_Target;
                        parmsDynamicQ.StartQValue = Original_DQStartQValue;
                        parmsDynamicQ.MinQValue = Original_DQMinQValue;
                        parmsDynamicQ.MaxQValue = Original_DQMaxQValue;
                        parmsDynamicQ.RetryCount = Original_DQRetryCount;
                        parmsDynamicQ.ToggleTarget = Original_DQToggleTarget;
                        parmsDynamicQ.ThresholdMultiplier = Original_DQThresholdMultiplier;

                        Result = parmsMasterDQ.store
                                (
                                    LakeChabotReader.MANAGED_ACCESS,
                                     _reader.ReaderHandle
                                );

                        if (Result.OK != Result)
                            MessageBox.Show("Set Algorithm Rule Failed.");

                        break;

                    case SingulationAlgorithm.FIXEDQ:
                        Source_QueryParms parmsMasterFQ = new Source_QueryParms();
                        Result = parmsMasterFQ.loadForAlgorithm
                        (
                            LakeChabotReader.MANAGED_ACCESS,
                            (uint)_reader.ReaderHandle,
                            SingulationAlgorithm.FIXEDQ
                        );
                        if (Result.OK != Result)
                            MessageBox.Show("Read Algorithm Rule Failed.");

                        Source_SingulationParametersFixedQ parmsFixedQ = (Source_SingulationParametersFixedQ)parmsMasterFQ.SingulationAlgorithmParameters;

                        parmsMasterFQ.TagGroupSelected = Original_Selected;
                        parmsMasterFQ.TagGroupSession = Original_Session;
                        parmsMasterFQ.TagGroupTarget = Original_Target;
                        parmsFixedQ.QValue = Original_FQQValue;
                        parmsFixedQ.RetryCount = Original_FQRetryCount;
                        parmsFixedQ.ToggleTarget = Original_FQToggleTarget;
                        parmsFixedQ.RepeatUntilNoTags = Original_FQRepeatUntilNoTags;

                        Result = parmsMasterFQ.store
                                (
                                    LakeChabotReader.MANAGED_ACCESS,
                                    _reader.ReaderHandle
                                );

                        if (Result.OK != Result)
                            MessageBox.Show("Set Algorithm Rule Failed.");

                        break;
                }
                //End by FJ for implement recovery mechanism for CB-2 function, 2017/12/07
            }
        }

        //Add by FJ go back initial value when close the tag access tool
        private void Reduce_Initial_Value()
        {
            //Add by FJ go back initial value of antenna
            AntennaPortConfig PortConfig = new AntennaPortConfig();
            Result Result = Result.OK;

            Result = LakeChabotReader.MANAGED_ACCESS.API_AntennaPortGetConfiguration(0, ref PortConfig);

            PortConfig.powerLevel = Original_Power_Level;
            PortConfig.physicalPort = (byte)Original_Physical_Port;
            //Add by FJ for implement recovery mechanism for CB-2 function, 2017/12/07
            PortConfig.numberInventoryCycles = Original_InventoryCycles;
            PortConfig.dwellTime = Original_dwellTime;
            //End by FJ for implement recovery mechanism for CB-2 function, 2017/12/07
            Result = LakeChabotReader.MANAGED_ACCESS.API_AntennaPortSetConfiguration(0, PortConfig);

            //Add by FJ go back initial value of Select Criteria rule
            rfid.Structures.SelectCriteria retrievedCriteria = new rfid.Structures.SelectCriteria();
            this.selectCriteria = new SelectCriteria();
            this.selectCriteria.pCriteria = new SelectCriterion[CRITERIA_MAXIM];
            Result = LakeChabotReader.MANAGED_ACCESS.API_l8K6CGetSelectCriteria(ref retrievedCriteria);
            if (Result.OK != Result)
                MessageBox.Show("Read Select Criteria Rule Failed.");
            else
            {
                // Copy over incoming criteria...
                this.selectCriteria.countCriteria = retrievedCriteria.countCriteria;

                // Ref copy since safe creation in managed lib now...
                for (int index = 0; index < retrievedCriteria.countCriteria; ++index)
                {
                    this.selectCriteria.pCriteria[index] = retrievedCriteria.pCriteria[index];
                }

                // Fill in ( or zero out if call > 1 ) remaining criteria
                for (uint index = selectCriteria.countCriteria; index < CRITERIA_MAXIM; ++index)
                {
                    this.selectCriteria.pCriteria[index] = new rfid.Structures.SelectCriterion();
                }
            }

            this.selectCriteria.countCriteria = (byte)0;
            Result = LakeChabotReader.MANAGED_ACCESS.API_l8K6CSetSelectCriteria(selectCriteria);
            if (Result.OK != Result)
                MessageBox.Show("Set Select Criteria Rule Failed.");


            //Add by FJ for implement recovery mechanism for CB-2 function, 2017/12/07
            //Recover initial value for algorithm
            switch (algorithm)
            {
                case SingulationAlgorithm.DYNAMICQ:
                    Source_QueryParms parmsMasterDQ = new Source_QueryParms();
                    Result = parmsMasterDQ.loadForAlgorithm
                        (
                            LakeChabotReader.MANAGED_ACCESS,
                            (uint)_reader.ReaderHandle,
                            SingulationAlgorithm.DYNAMICQ
                        );
                    if (Result.OK != Result)
                        MessageBox.Show("Read Algorithm Rule Failed.");

                    Source_SingulationParametersDynamicQ parmsDynamicQ = (Source_SingulationParametersDynamicQ)parmsMasterDQ.SingulationAlgorithmParameters;

                    parmsMasterDQ.TagGroupSelected = Original_Selected;
                    parmsMasterDQ.TagGroupSession = Original_Session;
                    parmsMasterDQ.TagGroupTarget = Original_Target;
                    parmsDynamicQ.StartQValue = Original_DQStartQValue;
                    parmsDynamicQ.MinQValue = Original_DQMinQValue;
                    parmsDynamicQ.MaxQValue = Original_DQMaxQValue;
                    parmsDynamicQ.RetryCount = Original_DQRetryCount;
                    parmsDynamicQ.ToggleTarget = Original_DQToggleTarget;
                    parmsDynamicQ.ThresholdMultiplier = Original_DQThresholdMultiplier;

                    Result = parmsMasterDQ.store
                            (
                                LakeChabotReader.MANAGED_ACCESS,
                                 _reader.ReaderHandle
                            );

                    if (Result.OK != Result)
                        MessageBox.Show("Set Algorithm Rule Failed.");

                    break;

                case SingulationAlgorithm.FIXEDQ:
                    Source_QueryParms parmsMasterFQ = new Source_QueryParms();
                    Result = parmsMasterFQ.loadForAlgorithm
                    (
                        LakeChabotReader.MANAGED_ACCESS,
                        (uint)_reader.ReaderHandle,
                        SingulationAlgorithm.FIXEDQ
                    );
                    if (Result.OK != Result)
                        MessageBox.Show("Read Algorithm Rule Failed.");

                    Source_SingulationParametersFixedQ parmsFixedQ = (Source_SingulationParametersFixedQ)parmsMasterFQ.SingulationAlgorithmParameters;

                    parmsMasterFQ.TagGroupSelected = Original_Selected;
                    parmsMasterFQ.TagGroupSession = Original_Session;
                    parmsMasterFQ.TagGroupTarget = Original_Target;
                    parmsFixedQ.QValue = Original_FQQValue;
                    parmsFixedQ.RetryCount = Original_FQRetryCount;
                    parmsFixedQ.ToggleTarget = Original_FQToggleTarget;
                    parmsFixedQ.RepeatUntilNoTags = Original_FQRepeatUntilNoTags;

                    Result = parmsMasterFQ.store
                            (
                                LakeChabotReader.MANAGED_ACCESS,
                                _reader.ReaderHandle
                            );

                    if (Result.OK != Result)
                        MessageBox.Show("Set Algorithm Rule Failed.");

                    break;
            }
            //End by FJ for implement recovery mechanism for CB-2 function, 2017/12/07
        }

        public void MessageDisplay(string message, string messageType, MessageBoxIcon messageLevel)
        {
            MessageBox.Show(message,
                messageType, MessageBoxButtons.OK,
                messageLevel);
        }
    }
}
