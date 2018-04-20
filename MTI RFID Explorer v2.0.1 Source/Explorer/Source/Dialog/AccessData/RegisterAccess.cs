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
 * $Id: RegisterAccess.cs,v 1.11 2010/06/24 10:01:54 dciampi Exp $
 * 
 * Description: 
 *
 *****************************************************************************
 */

using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Threading;

using rfid.Constants;
using rfid.Structures;
using RFID.RFIDInterface;

namespace RFID_Explorer
{
    public partial class FORM_RegisterAccess : Form
	{
        enum RegisterAccessType { Mac, MacBank, Bypass, OEM };

        private LakeChabotReader _reader;

        public LakeChabotReader Reader
        {
            get { return _reader; }
        }

        public FORM_RegisterAccess(LakeChabotReader Reader)
		{
            InitializeComponent();
            _reader = Reader;
            InitializeComponent_RegisterAccess();
        }

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
		}

        private void BUTTON_Close_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void BUTTON_ReadRegister_Click(object sender, EventArgs e)
        {
            RegisterAccessRead_Click();
        }

        private void BUTTON_WriteRegister_Click(object sender, EventArgs e)
        {
            RegsiterAccessWrite_Click();
        }

        private void BUTTON_RegisterAccessBatch_Click(object sender, EventArgs e)
        {
            RegisterAccessBatch_Click();
        }

        private void BUTTON_RegisterAccessBatchHelp_Click(object sender, EventArgs e)
        {
            RegisterAccessBatchHelp_Click();
        }

        private void COMBOBOX_RegisterAccess_SelectedIndexChanged(object sender, EventArgs e)
        {
            RegisterAccessType_Change();
        }

        private void BUTTON_ClearStatus_Click(object sender, EventArgs e)
        {
            StatusClear();
        }

        private void BUTTON_SaveStatus_Click(object sender, EventArgs e)
        {
            StatusSave();
        }

        private void BUTTON_DumpMac_Click(object sender, EventArgs e)
        {
            DumpMac_Click();
        }

        // COMMON CODE WITH INDYTOOL - START HERE

        private void InitializeComponent_RegisterAccess()
        {
            this.COMBOBOX_RegisterAccessType.DataSource = System.Enum.GetValues(typeof(RegisterAccessType));
        }

        private void RegisterAccessType_Change()
        {
            TEXTBOX_RegisterAccessWriteRegisterData.Text = "0";

            LABEL_RegisterAccessProfile.Visible = false;
            NUMERICUPDOWN_RegisterAccessProfile.Visible = false;

            LABEL_RegisterAccessWriteRegisterBank.Visible = false;
            TEXTBOX_RegisterAccessWriteRegisterBank.Visible = false;

            switch ((RegisterAccessType)COMBOBOX_RegisterAccessType.SelectedIndex)
            {
                case RegisterAccessType.Mac:
                    TEXTBOX_RegisterAccessWriteRegisterData.MaxLength = 8;
                    break;
                case RegisterAccessType.MacBank:
                    TEXTBOX_RegisterAccessWriteRegisterData.MaxLength = 8;
                    LABEL_RegisterAccessWriteRegisterBank.Visible = true;
                    TEXTBOX_RegisterAccessWriteRegisterBank.Visible = true;
                    break;
                case RegisterAccessType.Bypass:
                    TEXTBOX_RegisterAccessWriteRegisterData.MaxLength = 4;
                    break;
                case RegisterAccessType.OEM:
                    TEXTBOX_RegisterAccessWriteRegisterData.MaxLength = 8;
                    break;

                default:
                    throw new System.Exception(MethodBase.GetCurrentMethod().Name);
            }
        }

        private void RegisterAccessRead_Click()
        {
            RegisterAccessType type;
            ushort address = 0;
            uint count = 0;
            ushort bank = 0;

            StatusUpdate(string.Format("Read Register [{0}]", COMBOBOX_RegisterAccessType.SelectedItem), TextUpdateType.Begin);

            type = (RegisterAccessType)COMBOBOX_RegisterAccessType.SelectedIndex;
            
            if (!ValidateHex_ushort(TEXTBOX_RegisterAccessReadAddress.Text, "Address", out address))
            {
                return;
            }

            count = (uint)NUMERICUPDOWN_RegisterAccessReadRegisterLength.Value;

            if (type == RegisterAccessType.MacBank)
            {
                bank = ushort.MaxValue;
            }

            RegisterAccessRead(type, address, bank, count, true);
        }

        private void RegsiterAccessWrite_Click()
        {
            RegisterAccessType type;
            uint address = 0;
            uint data = 0;
            ushort bank = 0;

            StatusUpdate(string.Format("Write Register [{0}]", COMBOBOX_RegisterAccessType.SelectedItem), TextUpdateType.Begin);

            type = (RegisterAccessType)COMBOBOX_RegisterAccessType.SelectedIndex;

            if (!ValidateHex_uint(TEXTBOX_RegisterAccessWriteRegisterAddress.Text, "Address", out address))
            {
                return;
            }

            if (!ValidateHex_uint(TEXTBOX_RegisterAccessWriteRegisterData.Text, "Data", out data))
            {
                return;
            }

            if (type == RegisterAccessType.MacBank)
            {
                if (!ValidateHex_ushort(TEXTBOX_RegisterAccessWriteRegisterBank.Text, "Bank", out bank))
                {
                    return;
                }
            }
  
            RegisterAccessWrite(type, (ushort)address, bank, data, true);
        }
        
        private void RegisterAccessBatch_Click()
        {
            StatusUpdate("Load Register Access Batch File", TextUpdateType.Begin);

            try
            {
                OpenFileDialog openFileDiaglog = new OpenFileDialog();
                openFileDiaglog.Filter = "All (*)|*";
                openFileDiaglog.RestoreDirectory = true;
                openFileDiaglog.Title = "Load Regsiter Access Batch File";
                DialogResult dialogResult = openFileDiaglog.ShowDialog();

                if (dialogResult == DialogResult.OK)
                {
                    int lineNumber = 0;
                    uint commandCount = 0;
                    uint errorCount = 0;
                    bool endedEarly = false;
                    FileStream inStream = File.OpenRead(openFileDiaglog.FileName);

                    StreamReader stream = new StreamReader(inStream);

                    StatusUpdate(string.Format("File Name: {0}", openFileDiaglog.FileName), TextUpdateType.Normal);

                    while (!stream.EndOfStream)
                    {
                        string lineString = "";
                        string commandString = "";
                        string commentString = "";

                        string[] items;
                        string target = "";
                        string action = "";
                        bool valid = true;

                        RegisterAccessType type;
                        uint address;
                        uint count;
                        uint bank;
                        uint data;

                        lineString = stream.ReadLine();
                        lineNumber++;

                        int commentIndex = lineString.IndexOf(@"#");

                        if (commentIndex == -1)
                        {
                            commandString = lineString;
                        }
                        else if (commentIndex == 0)
                        {
                            commentString = lineString;
                        }
                        else
                        {
                            commandString = lineString.Substring(0, commentIndex).Trim();
                            commentString = lineString.Substring(commentIndex).Trim();
                        }

                        Regex regExNonAscii = new Regex("[^\x00-\x7f]");

                        if (regExNonAscii.IsMatch(commandString) || regExNonAscii.IsMatch(commentString))
                        {
                            StatusUpdate("Non Ascii Detected - Verify File Type", TextUpdateType.Error);
                            endedEarly = true;
                            break;
                        }

                        StatusUpdate((commentString != ""), string.Format("({0}) {1}", lineNumber, commentString), TextUpdateType.Warning);
                        StatusUpdate((commandString != ""), string.Format("({0}) {1}", lineNumber, commandString), TextUpdateType.Info);

                        if (commandString == "")
                        {
                            continue;
                        }

                        /* OEM_Tool Backward Compatibility */
                        commandString = Regex.Replace(commandString, "single_entry", "oem,write");

                        items = commandString.Split(',');

                        for (int index = 0; index < items.Length; index++)
                        {
                            items[index] = items[index].Trim();
                        }

                        if (items.Length >= 2)
                        {
                            target = items[0].ToLower();
                            action = items[1].ToLower();
                            if (action != "read" && action != "write")
                            {
                                StatusUpdate(string.Format("({0}) Invalid Action [{1}]", lineNumber, commandString), TextUpdateType.Error);
                                errorCount++;
                                if (CHECKBOX_RegisterAccesssBatchStopOnError.Checked)
                                {
                                    endedEarly = true;
                                    break;
                                }
                                continue;
                            }
                        }
                        else
                        {
                            StatusUpdate(string.Format("({0}) Invalid Line [{1}]", lineNumber, commandString), TextUpdateType.Error);
                            errorCount++;
                            if (CHECKBOX_RegisterAccesssBatchStopOnError.Checked)
                            {
                                endedEarly = true;
                                break;
                            }
                            continue;
                        }

                        uint[] parameters = new uint[items.Length - 2];

                        Regex regExHex = new Regex("^0x");

                        for (int index = 0; index < parameters.Length; index++)
                        {
                            if (regExHex.IsMatch(items[index + 2]))
                            {
                                if (!uint.TryParse(items[index + 2].Substring(2), NumberStyles.AllowHexSpecifier, null, out parameters[index]))
                                {
                                    valid = false;
                                }
                            }
                            else
                            {
                                if (!uint.TryParse(items[index + 2], out parameters[index]))
                                {
                                    valid = false;
                                }
                            }

                            if ((target == "macbank") && (index == 1) && (items[3] == "-1"))
                            {
                                parameters[index] = uint.MaxValue;
                                valid = true;
                            }

                            if (!valid)
                            {
                                StatusUpdate(string.Format("({0}) Invalid Item Index {1} [{2}]", lineNumber, index + 2, items[index + 2]), TextUpdateType.Error);
                                errorCount++;
                                if (CHECKBOX_RegisterAccesssBatchStopOnError.Checked)
                                {
                                    endedEarly = true;
                                    break;
                                }
                                continue;
                            }
                        }

                        type = RegisterAccessType.Mac;
                        address = 0;
                        count = 1;
                        bank = 0;
                        data = 0;

                        if (action == "read")
                        {
                            switch (target)
                            {
                                case "mac":
                                    type = RegisterAccessType.Mac;
                                    if (parameters.Length == 1) /* mac,read,*address */
                                    {
                                        address = parameters[0];
                                    }
                                    else if (parameters.Length == 2) /* mac,read,*address,*count */
                                    {
                                        address = parameters[0];
                                        count = parameters[1];
                                    }
                                    else
                                    {
                                        valid = false;
                                    }
                                    break;
                                case "macbank":
                                    type = RegisterAccessType.MacBank;
                                    if (parameters.Length == 2) /* macbank,read,*address,*bank */
                                    {
                                        address = parameters[0];
                                        bank = (parameters[1] == uint.MaxValue) ? ushort.MaxValue : parameters[1];
                                    }
                                    else if (parameters.Length == 3) /* macbank,read,*address,*bank,*count */
                                    {
                                        address = parameters[0];
                                        bank = (parameters[1] == uint.MaxValue) ? ushort.MaxValue : parameters[1];
                                        count = parameters[2];
                                    }
                                    else
                                    {
                                        valid = false;
                                    }
                                    break;
                                case "bypass":
                                    type = RegisterAccessType.Bypass;
                                    if (parameters.Length == 1) /* bypass,read,*address */
                                    {
                                        address = parameters[0];
                                    }
                                    else if (parameters.Length == 2) /* bypass,read,*address,*count */
                                    {
                                        address = parameters[0];
                                        count = parameters[1];
                                    }
                                    else
                                    {
                                        valid = false;
                                    }
                                    break;
                                case "oem":
                                    type = RegisterAccessType.OEM;
                                    if (parameters.Length == 1) /* oem,read,*address */
                                    {
                                        address = parameters[0];
                                    }
                                    else if (parameters.Length == 2) /* oem,read,*address,*count */
                                    {
                                        address = parameters[0];
                                        count = parameters[1];
                                    }
                                    else
                                    {
                                        valid = false;
                                    }
                                    break;
 
                                 default:
                                    break;
                            }

                            if (address > ushort.MaxValue)
                            {
                                valid = false;
                            }

                            if (valid)
                            {
                                commandCount++;
                                if (!RegisterAccessRead(type, (ushort)address, (ushort)bank, count, true))
                                {
                                    errorCount++;
                                    if (CHECKBOX_RegisterAccesssBatchStopOnError.Checked)
                                    {
                                        endedEarly = true;
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                StatusUpdate(string.Format("({0}) Invalid Parameters [{1}]", lineNumber, lineString), TextUpdateType.Error);
                                errorCount++;
                                if (CHECKBOX_RegisterAccesssBatchStopOnError.Checked)
                                {
                                    endedEarly = true;
                                    break;
                                }
                                continue;
                            }
                        }
                        else if (action == "write")
                        {
                            switch (target)
                            {
                                case "mac":
                                    type = RegisterAccessType.Mac;
                                    if (parameters.Length == 2) /* mac,write,*address,*data */
                                    {
                                        address = parameters[0];
                                        data = parameters[1];
                                    }
                                    else
                                    {
                                        valid = false;
                                    }
                                    break;
                                case "macbank":
                                    type = RegisterAccessType.MacBank;
                                    if (parameters.Length == 3) /* macbank,write,*address,*bank,*data */
                                    {
                                        address = parameters[0];
                                        bank = (parameters[1] == uint.MaxValue) ? uint.MaxValue : parameters[1];
                                        data = parameters[2];
                                    }
                                    else
                                    {
                                        valid = false;
                                    }
                                    break;
                                case "bypass":
                                    type = RegisterAccessType.Bypass;
                                    if (parameters.Length == 2) /* bypass,write,*address,*data */
                                    {
                                        address = parameters[0];
                                        data = parameters[1];
                                    }
                                    else
                                    {
                                        valid = false;
                                    }
                                    break;
                                case "oem":
                                    type = RegisterAccessType.OEM;
                                    if (parameters.Length == 2) /* oem,write,*address,*data */
                                    {
                                        address = parameters[0];
                                        data = parameters[1];
                                    }
                                    else
                                    {
                                        valid = false;
                                    }
                                    break;

                                default:
                                    break;
                            }

                            if (address > ushort.MaxValue)
                            {
                                valid = false;
                            }

                            if (type == RegisterAccessType.Bypass)
                            {
                                if (data > ushort.MaxValue)
                                {
                                    valid = false;
                                }
                            }

                            if (valid)
                            {
                                commandCount++;
                                if (!RegisterAccessWrite(type, (ushort)address, (ushort)bank, data, true))
                                {
                                    errorCount++;
                                    if (CHECKBOX_RegisterAccesssBatchStopOnError.Checked)
                                    {
                                        endedEarly = true;
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                StatusUpdate(string.Format("({0}) Invalid Parameters [{1}]", lineNumber, lineString), TextUpdateType.Error);
                                errorCount++;
                                if (CHECKBOX_RegisterAccesssBatchStopOnError.Checked)
                                {
                                    endedEarly = true;
                                    break;
                                }
                                continue;
                            }
                        }

                    }

                    inStream.Close();

                    StatusUpdate("", TextUpdateType.Normal);

                    if (!endedEarly)
                    {
                        StatusUpdate(string.Format("Load Regsiter Access Batch Success [Commands:{0} Errors:{1}]", commandCount, errorCount), TextUpdateType.Success);
                    }
                    else
                    {
                        StatusUpdate(string.Format("Load Regsiter Access Batch Fail [Commands:{0} Errors:{1}]", commandCount, errorCount), TextUpdateType.Error);
                    }
                }
                else if (dialogResult == DialogResult.Cancel)
                {
                    StatusUpdate("Load Regsiter Access Batch Cancelled", TextUpdateType.Error);
                }

                openFileDiaglog.Dispose();
            }
            catch
            {
                System.Diagnostics.Debug.Assert(false, "Load Batch File Exception"); 
            }

        }

        private void RegisterAccessBatchHelp_Click()
        {
            StatusUpdate("Regsiter Access Batch Help", TextUpdateType.Begin);

            StatusUpdate("Legend", TextUpdateType.Info);
            StatusUpdate("* Decimal value or Hex Value (0x_)", TextUpdateType.Normal);
            StatusUpdate("** Same as *, but in addition -1 for all banks", TextUpdateType.Normal);
            StatusUpdate("# is the comment character", TextUpdateType.Normal);
            StatusUpdate("", TextUpdateType.Normal);

            StatusUpdate("Commands", TextUpdateType.Info);
            StatusUpdate("mac,read,*address", TextUpdateType.Normal);
            StatusUpdate("mac,read,*address,*count", TextUpdateType.Normal);
            StatusUpdate("mac,write,*address,*data", TextUpdateType.Normal);
            StatusUpdate("", TextUpdateType.Normal);
            StatusUpdate("macbank,read,*address,**bank", TextUpdateType.Normal);
            StatusUpdate("macbank,read,*address,**bank,*count", TextUpdateType.Normal);
            StatusUpdate("macbank,write,*address,*bank,*data", TextUpdateType.Normal);
            StatusUpdate("", TextUpdateType.Normal);
            StatusUpdate("bypass,read,*address", TextUpdateType.Normal);
            StatusUpdate("bypass,read,*address,*count", TextUpdateType.Normal);
            StatusUpdate("bypass,write,*address,*data", TextUpdateType.Normal);
            StatusUpdate("", TextUpdateType.Normal);
            StatusUpdate("oem,read,*address", TextUpdateType.Normal);
            StatusUpdate("oem,read,*address,*count", TextUpdateType.Normal);
            StatusUpdate("oem,write,*address,*data", TextUpdateType.Normal);
            StatusUpdate("single_entry,*address,*data", TextUpdateType.Normal);
            StatusUpdate("", TextUpdateType.Normal);
        }

        // STATUS CODE

        private Object thisLock = new Object();

        enum TextUpdateType { Normal, Success, Error, Warning, Info, Begin };

        private void StatusUpdate(bool print, string text, TextUpdateType type)
        {
            if (print)
            {
                StatusUpdate(text, type);
            }
        }

        private void StatusUpdate(string text, TextUpdateType type)
        {
            Color color = Color.Black;
            FontStyle fontStyle = FontStyle.Regular;
            bool append = true;

            switch (type)
            {
                case TextUpdateType.Normal:
                    break;
                case TextUpdateType.Success:
                    color = Color.Green;
                    fontStyle = FontStyle.Bold;
                    break;
                case TextUpdateType.Info:
                    color = Color.Brown;
                    fontStyle = FontStyle.Bold;
                    break;
                case TextUpdateType.Warning:
                    color = Color.Orange;
                    fontStyle = FontStyle.Bold;
                    break;
                case TextUpdateType.Error:
                    color = Color.Red;
                    fontStyle = FontStyle.Bold;
                    break;
                case TextUpdateType.Begin:
                    fontStyle = FontStyle.Bold;
                    append = false;
                    // Append History Override
                    if (CHECKBOX_AppendHistory.Checked)
                    {
                        append = true;
                        text = Environment.NewLine + text;
                    }
                    break;
                default:
                    break;
            }

            StatusUpdate(text, color, fontStyle, append);
        }

        delegate void StatusUpdate_Delegate(string text, Color color, FontStyle fontStyle, bool append);
        private void StatusUpdate(string text, Color color, FontStyle fontStyle, bool append)
        {
            if (TEXTBOX_MainStatus.InvokeRequired)
            {
                StatusUpdate_Delegate del = new StatusUpdate_Delegate(StatusUpdate);
                TEXTBOX_MainStatus.Invoke(del, new object[] { text, color, fontStyle, append });
            }
            else
            {
                if (!append)
                {
                    TEXTBOX_MainStatus.Text = "";
                }

               lock (thisLock)
                {
                    TEXTBOX_MainStatus.Select(2147483647, 0);
                    TEXTBOX_MainStatus.SelectionFont = new Font("Verdana", 8, fontStyle);
                    TEXTBOX_MainStatus.SelectionColor = color;
                    TEXTBOX_MainStatus.SelectedText += text + Environment.NewLine;
                    TEXTBOX_MainStatus.ScrollToCaret();
                }

            }
        }
        private void StatusClear()
        {
            StatusUpdate("", Color.Black, FontStyle.Regular, false);
        }

        private void StatusSave()
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.DefaultExt = "txt";
                saveFileDialog.Filter = "Text file (*.txt)|*.txt";
                saveFileDialog.AddExtension = true;
                saveFileDialog.RestoreDirectory = true;
                saveFileDialog.Title = "Save Status";
                saveFileDialog.FileName = "status.txt";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    StreamWriter logFile = new System.IO.StreamWriter(saveFileDialog.FileName);

                    logFile.WriteLine(this.Text);
                    logFile.WriteLine(string.Format("Status Dump [{0}]" + Environment.NewLine, DateTime.Now));

                    for (int i = 0; i < TEXTBOX_MainStatus.Lines.Length; i++)
                    {
                        logFile.WriteLine(TEXTBOX_MainStatus.Lines[i]);
                    }

                    logFile.Close();

                    StatusUpdate("", TextUpdateType.Normal);
                    StatusUpdate(string.Format("File Saved To [{0}]", saveFileDialog.FileName), TextUpdateType.Success);
                }

                saveFileDialog.Dispose();
                saveFileDialog = null;
            }
            catch
            {
                System.Diagnostics.Debug.Assert(false, "Save Status Exception");
            }
        }


        // VALIDATE CODE

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
                StatusUpdate(string.Format("Invalid {0} [{1}]", name, input), TextUpdateType.Error);
                return false;
            }
            return true;
        }

        private bool ValidateHex_ushort(string input, string name, out ushort value)
        {
            if (!ushort.TryParse(input, NumberStyles.AllowHexSpecifier, null, out value))
            {
                StatusUpdate(string.Format("Invalid {0} [{1}]", name, input), TextUpdateType.Error);
                return false;
            }
            return true;
        }
    }
}