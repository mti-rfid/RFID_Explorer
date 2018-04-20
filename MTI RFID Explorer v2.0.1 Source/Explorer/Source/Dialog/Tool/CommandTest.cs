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
 * $Id: CommandTest.cs,v 1.3 2015/01/XX 20:23:18 Chingsheng Exp $
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

using System.Runtime.InteropServices;

using RFID.RFIDInterface;
using Global;
using rfid;

namespace RFID_Explorer
{
    public partial class CommandTest : Form
    {
        private static RFID_Explorer.mainForm m_mainForm = null;
        private LakeChabotReader m_reader = null;
        private int errCount = 0;
        private int errSize = 0;
        private int MAX_ERROR = 3;
        //Add by FJ for implement the script command test function, 2017-01-13
        private int CommandCount = 0, RemainingCount = 0;
        private List<string> CommandList = new List<string>();
        private List<string> ResponseCommand = new List<string>();
        private bool DelayTimeError, CommandError, CommandIDError;
        //End by FJ for implement the script command test function, 2017-01-13

        public CommandTest(RFID_Explorer.mainForm r_form, LakeChabotReader rm_reader)
        {
            InitializeComponent();

            backgroundWorker1.WorkerReportsProgress = true;
            pictureBox.Visible = false;
            //Add by FJ for implement the script command test function, 2017-01-13
            backgroundWorker2.WorkerReportsProgress = true;            
            pictureBox1.Visible = false;
            ButtonExecute.Enabled = false;
            //End by FJ for implement the script command test function, 2017-01-13
            m_mainForm = r_form;
            m_reader = rm_reader;
            errCount = 0;
            errSize = 0;
            textBoxDeviceID.Select();
        }


        /// <summary>  
        /// Ban the command id checks : Report Packets (Command-Begin, etc.)
        /// 0x40 - 0x4F
        /// 0x8B
        /// </summary>  
        /// <returns></returns>  
        private bool checkBanCommand(byte commandID)
        {
            if ((commandID >= 0x40 && commandID <= 0x4F)
              || commandID == 0x8B)
                return true;
            return false;
        }

        /// <summary>  
        /// Paste the data format checks  
        /// </summary>  
        /// <returns></returns>  
        private bool checkPaste()
        {
            try
            {
                char[] PasteChar = Clipboard.GetDataObject().GetData(DataFormats.Text).ToString().ToCharArray();

                foreach (char data in PasteChar)
                {
                    if (!((data >= '0' && data <= '9')//0-9
                       || (data >= 'A' && data <= 'F')//A-F
                       || (data >= 'a' && data <= 'f')//a-f
                       || data == 0x20))//Space
                    {
                        MessageBox.Show("Invalid Command!\nMust contain only numbers 0-9, capital letters A-F, lowercase letters a-f and spaces!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        /// <summary>  
        /// Keyin the data format checks  
        /// </summary>  
        /// <returns></returns>
        private bool checkTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar >= '0' && e.KeyChar <= '9')  //0-9
             || (e.KeyChar >= 'A' && e.KeyChar <= 'F')  //A-F
             || (e.KeyChar >= 'a' && e.KeyChar <= 'f')  //a-f
             || (e.KeyChar == 0x20)                     //Space
             || (e.KeyChar == 0x08)                     //Backpace
             || (e.KeyChar == 0x03)                     //Ctrl + C
             || (e.KeyChar == 0x18))                    //Ctrl + X
            {
                return true;
            }
            else if (e.KeyChar == 0x16)//Ctrl + V
            { //Paste the data format checks
                return checkPaste();
            }

            return false;
        }

        /// <summary>  
        /// String to byte array  
        /// </summary>  
        /// <returns></returns>
        public static byte[] StringToByteArray(String hex)
        {

            if ((hex.Length % 2) == 1)
            {
                hex = hex + "0";
            }

            int NumberChars = hex.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }


        private void genCommand()
        {
            ushort usCRC = 0;
            byte[] sendCommandBuf = new byte[16];
            byte[] parametersBuf = new byte[8];
            ushort usCrcLen = (ushort)(ENMU_PKT_INDEX.PKT_CRC - ENMU_PKT_INDEX.PKT_HEADER);
            Array.Clear(sendCommandBuf, 0, sendCommandBuf.Length);

            sendCommandBuf[0] = 0x43;
            sendCommandBuf[1] = 0x49;
            sendCommandBuf[2] = 0x54;
            sendCommandBuf[3] = 0x4D;
            sendCommandBuf[4] = Convert.ToByte(textBoxDeviceID.Text, 16);
            sendCommandBuf[5] = Convert.ToByte(textBoxCommandID.Text, 16);

            parametersBuf = StringToByteArray(textBoxParameters.Text.Replace(" ", ""));
            Array.Copy(parametersBuf, 0, sendCommandBuf, (int)ENMU_PKT_INDEX.PKT_DATA, parametersBuf.Length);

            usCRC = rfid.Linkage.CrcCul(sendCommandBuf, 0, (ushort)(usCrcLen * 8));
            sendCommandBuf[14] = (byte)(~usCRC & 0xFF);
            sendCommandBuf[15] = (byte)(~usCRC >> 8 & 0xFF);

            textBoxSendCommand.Text = BitConverter.ToString(sendCommandBuf, 0, sendCommandBuf.Length).Replace("-", " ");
            labelMessage.Text = "";
            textBoxResponse.Text = "";
        }

        private void executeCommand()
        {
            if (backgroundWorker1.IsBusy != true)
            {
                byte[] sendCommandBuf = StringToByteArray(textBoxSendCommand.Text.Replace(" ", ""));
                textBoxSendCommand.Text = BitConverter.ToString(sendCommandBuf, 0, sendCommandBuf.Length).Replace("-", " ");

                if (textBoxSendCommand.Text == "" || textBoxSendCommand.Text.Replace(" ", "").Length != 32)
                {
                    labelMessage.Text = "*Invalid Command! The length must be 16 bytes!";
                    textBoxResponse.Text = "";
                    errSize++;
                    if (errSize > MAX_ERROR)
                    {
                        MessageBox.Show("The length must be 16 bytes!\nPlease refer to RFID Module Command Reference Manual!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    return;
                }

                if (checkBanCommand(sendCommandBuf[(int)ENMU_PKT_INDEX.PKT_COMMAND_ID]))
                {
                    labelMessage.Text = "*Does not support Command ID '0x40' ~ '0x4F' or '0x8B'!";
                    textBoxResponse.Text = "";
                    errCount++;
                    if (errCount > MAX_ERROR)
                    {
                        MessageBox.Show("Does not support Command ID '0x40' ~ '0x4F' or '0x8B'!\nPlease refer to RFID Module Command Reference Manual!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    return;
                }
                errCount = 0;
                errSize = 0;
                labelMessage.Text = "";
                pictureBox.Visible = true;
                // Start the asynchronous operation.
                backgroundWorker1.RunWorkerAsync();
            }

        }

        private void btnExecuteCommand_Click(object sender, EventArgs e)
        {
            executeCommand();
        }

        private void btnGenCommand_Click(object sender, EventArgs e)
        {
            genCommand();
        }

        private void textBoxDeviceID_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (checkTextBox_KeyPress(sender, e))
            {
                e.Handled = false;
                return;
            }
            else if (e.KeyChar == 0x0D) // Enter
            {
                textBoxCommandID.Select();
            }

            e.Handled = true;
        }

        private void textBoxCommandID_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (checkTextBox_KeyPress(sender, e))
            {
                e.Handled = false;
                return;
            }
            else if (e.KeyChar == 0x0D) // Enter
            {
                textBoxParameters.Select();
            }

            e.Handled = true;
        }

        private void textBoxParameters_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (checkTextBox_KeyPress(sender, e))
            {
                e.Handled = false;
                return;
            }
            else if (e.KeyChar == 0x0D) // Enter
            {
                textBoxSendCommand.Select();
                genCommand();
                textBoxSendCommand.Select(textBoxSendCommand.Text.Length, 0);
            }

            e.Handled = true;
        }

        private void textBoxSendCommand_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (checkTextBox_KeyPress(sender, e))
            {
                e.Handled = false;
                return;
            }
            else if (e.KeyChar == 0x0D) // Enter
            {
                executeCommand();
            }

            e.Handled = true;
        }

        private void textBoxResponse_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 0x03) //Ctrl + C (Copy)
            {
                e.Handled = false;
                return;
            }

            e.Handled = true;
        }


        private void textBoxDeviceID_Leave(object sender, EventArgs e)
        {
            if (textBoxDeviceID.Text.Length == 0)
            {
                textBoxDeviceID.Text = String.Format("{0:X2}", 0);
            }
            else if (textBoxDeviceID.Text.Length != 2)
            {
                textBoxDeviceID.Text = String.Format("{0:X2}", Convert.ToByte(textBoxDeviceID.Text, 16));
            }
        }


        private void textBoxCommandID_Leave(object sender, EventArgs e)
        {
            if (textBoxCommandID.Text.Length == 0)
            {
                textBoxCommandID.Text = String.Format("{0:X2}", 0);
            }
            else if (textBoxCommandID.Text.Length != 2)
            {
                textBoxCommandID.Text = String.Format("{0:X2}", Convert.ToByte(textBoxCommandID.Text, 16));
            }
            if (checkBanCommand(Convert.ToByte(textBoxCommandID.Text, 16)))
            {
                errCount++;
                if (errCount > MAX_ERROR)
                {
                    MessageBox.Show("Does not support Command ID '0x40' ~ '0x4F' or '0x8B'!\nPlease refer to RFID Module Command Reference Manual!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //textBoxCommandID.Text = "";
                }
                else
                {
                    MessageBox.Show("Does not support Command ID '0x40' ~ '0x4F' or '0x8B'!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                textBoxCommandID.Select();
                textBoxCommandID.Select(0, textBoxCommandID.Text.Length);
            }
            else
            {
                errCount = 0;
            }
            
        }

        private void textBoxParameters_Leave(object sender, EventArgs e)
        {
            String str = textBoxParameters.Text.Replace(" ", "");
            if (str.Length != 16)
            {
                int len = str.Length;

                if (len > 16)
                    str = str.Substring(0, 16);
                else
                {
                    while (len < 16)
                    {
                        str = str + "0";
                        len++;
                    }
                }
            }

            byte[] parametersBuf = StringToByteArray(str);
            textBoxParameters.Text = BitConverter.ToString(parametersBuf, 0, parametersBuf.Length).Replace("-", " ");
        }


        // This event handler is where the time-consuming work is done.
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            byte[] sendCommandBuf = StringToByteArray(textBoxSendCommand.Text.Replace(" ", ""));
            byte[] responseBuf = new byte[16];
            worker.ReportProgress(0, "");
            Global.ENUM_CMD result = LakeChabotReader.MANAGED_ACCESS.Command_Test(sendCommandBuf, ref responseBuf);

            if (Global.ENUM_CMD.TIME_OUT == result)
            {// Timeout
                worker.ReportProgress(0, "No Response!");
                return;
            }
            else if (Global.ENUM_CMD.COMMAND_TEST != result)
            {// Result Error
                worker.ReportProgress(0, "No Response!");
                return;
            }

            worker.ReportProgress(0, BitConverter.ToString(responseBuf, 0, responseBuf.Length).Replace("-", " "));
        }

        // This event handler updates the progress.
        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            textBoxResponse.Text = (String)e.UserState;
        }

        // This event handler deals with the results of the background operation.
        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            pictureBox.Visible = false;
        }

        //Add by FJ for implement the script command test function, 2017-01-13
        //Go back initial value when change tab
        private void TabControl1_Click(object sender, EventArgs e)
        {
            switch (tabControl1.SelectedIndex)
            {
                case 0:
                    textBoxSendCommand.Text = "";
                    textBoxResponse.Text = "";
                    textBoxCommandID.Text = "00";
                    break;

                case 1:
                    TextBoxCommandScriptDes.Text = "";
                    TextBoxCommandScript.Text = "";
                    TextBoxScriptResponse.Text = "";
                    LabelCommandCount.Text = "0";
                    LabelRemainingCount.Text = "0";
                    ButtonExecute.Enabled = false;
                    break;
            }
        }
        
        //Load script file
        private void ButtonLoadScript_Click(object sender, EventArgs e)
        {
            string ScriptFileLine, DelayStr, CommandDes;
            string[] DelayStrArray;
            int ScriptFileLine1 = 0, Result, CheckInteger;
            bool Error = false;
            ushort usCRC = 0, usCrcLen;
            byte[] CommandBuf = new byte[16];
            byte[] ScriptCommand = new byte[14];
			
            CommandCount = 0;
            TextBoxCommandScriptDes.Text = "";
            TextBoxCommandScript.Text = "";
            TextBoxScriptResponse.Text = "";
            LabelCommandCount.Text = "0";
            ButtonExecute.Enabled = false;
            DelayTimeError = false;
            CommandError = false;
            CommandIDError = false;
            CommandList.Clear();

            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.InitialDirectory = "C:\\";
            openFileDialog1.Filter = "Text Files (.txt)|*.txt";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                System.IO.StreamReader ScriptFile = new System.IO.StreamReader(openFileDialog1.FileName);
                while ((ScriptFileLine = ScriptFile.ReadLine()) != null)
                {
                    ScriptFileLine1++;
                    if (ScriptFileLine1 == 1)
                    {
                        //Check the RFID script file.
                        Result = String.Compare(ScriptFileLine, "#MTI RFID Command Script", true);
                        if (Result != 0)
                        {
                            MessageBox.Show("This is not command script file！");
                            Error = true;
                            break;
                        }
                    }
                    else
                    {
                        if (ScriptFileLine.Replace(" ", "").Trim().Length != 0)
                        {
                            //Example of script content

                            //#This is a command script sample.
                            //#This is a test file.
                            //43 49 54 4D FF 11 00 00 00 00 00 00 00 00 # comment 1 
                            //delay(500)
                            //43 49 54 4D FF 11 01 00 00 00 00 00 00 00 # comment 2 
							
                            //Filter the command script description
                            if (String.Compare(ScriptFileLine[0].ToString(), "#", true) == 0)
                            {
                                TextBoxCommandScriptDes_Refresh(ScriptFileLine.Trim('#'));
                            }
                            else 
                            {
                                //Filter the command script "delay" function
                                if (ScriptFileLine.Replace(" ", "").Contains("delay("))
                                {
                                    DelayStr = ScriptFileLine.Trim();
                                    TextBoxCommandScript_Refresh(DelayStr);
                                    
                                    //Check first char
                                    if (DelayStr[0] != 'd')
                                    {
                                        DelayTimeError = true;
                                        ErrorMes(ScriptFileLine);
                                        Error = true;
                                        break;
                                    }

                                    //Check delay(delaytime) format
                                    DelayStrArray = DelayStr.Replace("delay(", "*").Replace(")", "*").Split('*');
                                    if (DelayStrArray.Length > 3)
                                    {
                                        DelayTimeError = true;
                                        ErrorMes(ScriptFileLine);
                                        Error = true;
                                        break;
                                    }
                                    if (DelayStrArray[0].Length > 0 || DelayStrArray[2].Length > 0)
                                    {
                                        DelayTimeError = true;
                                        ErrorMes(ScriptFileLine);
                                        Error = true;
                                        break;
                                    }

                                    //Check delaytime format
                                    CheckInteger = 0;
                                    if (int.TryParse(DelayStrArray[1], out CheckInteger) == false)
                                    {
                                        DelayTimeError = true;
                                        ErrorMes(ScriptFileLine);
                                        Error = true;
                                        break;
                                    }

                                    CommandList.Add("d" + DelayStrArray[1].ToString());
                                }
                                //Filter the command script "COMMAND" function
                                else if (ScriptFileLine.Replace(" ", "").Contains("4349544D"))
                                {
                                    if (CheckCommandFormat(ScriptFileLine))
                                    {
                                        //Compute the command CRC
                                        usCRC = 0;
                                        CommandDes = "";
                                        CommandBuf = new byte[16];
                                        ScriptCommand = new byte[14];
                                        Array.Clear(CommandBuf, 0, CommandBuf.Length);
                                        usCrcLen = (ushort)(ENMU_PKT_INDEX.PKT_CRC - ENMU_PKT_INDEX.PKT_HEADER);

                                        ScriptCommand = StringToByteArray(ScriptFileLine.Substring(0, 28).Replace(" ", ""));
                                        Array.Copy(ScriptCommand, 0, CommandBuf, 0, ScriptCommand.Length);

                                        usCRC = rfid.Linkage.CrcCul(CommandBuf, 0, (ushort)(usCrcLen * 8));
                                        CommandBuf[14] = (byte)(~usCRC & 0xFF);
                                        CommandBuf[15] = (byte)(~usCRC >> 8 & 0xFF);

                                        CommandList.Add(BitConverter.ToString(CommandBuf, 0, CommandBuf.Length).Replace("-", ""));

                                        if (ScriptFileLine.Contains("#"))
                                            CommandDes = ScriptFileLine.Substring(42, ScriptFileLine.Length - 42);

                                        TextBoxCommandScript_Refresh(BitConverter.ToString(CommandBuf, 0, CommandBuf.Length).Replace("-", " ") + " " + CommandDes);
                                        ResponseCommand.Add(BitConverter.ToString(CommandBuf, 0, CommandBuf.Length).Replace("-", " ") + " " + CommandDes);
                                        CommandCount++;
                                    }
                                    else
                                    {
                                        TextBoxCommandScript_Refresh(ScriptFileLine);
                                        ErrorMes(ScriptFileLine);
                                        Error = true;
                                        break;
                                    }
                                }
                                else
                                {
                                    TextBoxCommandScript_Refresh(ScriptFileLine);
                                    ErrorMes(ScriptFileLine);
                                    Error = true;
                                    break;
                                }
                            }
                        }
                        if (Error == true)
                            break;
                    }
                }
                ScriptFile.Close();
                if (Error == false)
                {
                    ButtonExecute.Enabled = true;
                    LabelCommandCount.Text = CommandCount.ToString();
                }
            } 
        }

        private void ErrorMes(String str)
        {
            
            if(DelayTimeError == true)
            {
                MessageBox.Show("The command \"" + str + "\" format error！\nThe correct format is \"delay([Delaytime])\", and the parameter \"[Delaytime]\" must be an integer！");
            }
            else if (CommandError == true)
            {
                MessageBox.Show("The command \"" + str + "\" format error！\nThe correct format \"43 49 54 4D FF [CommandID] [Parameters]\"！\n" +
                                "The \"[CommandID]\" must contain only numbers 0-9, capital letters A-F, lowercase letters a-f, then 2 bits length！\n" +
                                "The \"[Parameters]\" must contain only numbers 0-9, capital letters A-F, lowercase letters a-f, then 16 bits length！\n"
                               );
            }
            else if (CommandIDError == true)
            {
                MessageBox.Show("The command \"" + str + "\" format error！\nThe correct format \"43 49 54 4D FF [CommandID] [Parameters]\"！\n" +
                                "The \"[CommandID]\" does not support '0x40' ~ '0x4F' or '0x8B'!\n");
            }
            else
            {
                MessageBox.Show("The \"" + str + "\" is invalid command format！");
            }

            TextBoxCommandScriptDes.Text = "";
            TextBoxCommandScript.Text = "";
            CommandCount = 0;
        }


        private bool CheckCommandFormat(String str)
        {
            string CheckCommand, CommandStr;
     
            CheckCommand = str.Replace(" ", "").Trim();

            //Check "43 49 54 4D" header
            CommandStr = CheckCommand.Replace("4349544D", "*");
            if (CommandStr[0] != '*')
            {
                CommandError = true;
                return false;
            }

            //Check command length whethere to less
            if (CheckCommand.Length < 28)
            {
                CommandError = true;
                return false;
            }

            //Check command length whethere to long or Command description error
            CommandStr = CheckCommand.Substring(28, CheckCommand.Length - 28);
            if (CommandStr.Length > 0 && CommandStr[0] != '#')
            {
                CommandError = true;
                return false;
            }

            //Check command format whethere include description information
            foreach (char data in CheckCommand.Substring(0, 28))
            {
                if (!((data >= '0' && data <= '9')//0-9
                  || (data >= 'A' && data <= 'F')//A-F
                  || (data >= 'a' && data <= 'f')))//a-f
                {
                    CommandError = true;
                    return false;
                }
            }

            //Check Command ID '0x40' ~ '0x4F' or '0x8B'
            CommandStr = CheckCommand.Substring(10, 2);
            if (checkBanCommand(Convert.ToByte(CommandStr, 16)))
            {
                CommandIDError = true;
                return false;
            }

            return true;
        }

        //The command script description text content
        private void TextBoxCommandScriptDes_Refresh(String str)
        {
            TextBoxCommandScriptDes.Text = TextBoxCommandScriptDes.Text + str + Environment.NewLine;
            TextBoxCommandScriptDes.SelectionStart = TextBoxCommandScriptDes.Text.Length;
            TextBoxCommandScriptDes.ScrollToCaret();
            TextBoxCommandScriptDes.Refresh();
        }

        //The command script text content
        private void TextBoxCommandScript_Refresh(String str)
        {
            TextBoxCommandScript.Text = TextBoxCommandScript.Text + str + Environment.NewLine;
            TextBoxCommandScript.SelectionStart = TextBoxCommandScript.Text.Length;
            TextBoxCommandScript.ScrollToCaret();
            TextBoxCommandScript.Refresh();
        }

        //The script response text content 
        private void TextBoxScriptResponse_Refresh(String str)
        {
            TextBoxScriptResponse.Text = TextBoxScriptResponse.Text + str + Environment.NewLine;
            TextBoxScriptResponse.SelectionStart = TextBoxScriptResponse.Text.Length;
            TextBoxScriptResponse.ScrollToCaret();
            TextBoxScriptResponse.Refresh();
        }

        //Execute the "Command Script" content 
        private void ButtonExecute_Click(object sender, EventArgs e)
        {
            TextBoxScriptResponse.Text = "";
            //Use back ground worker
            if (backgroundWorker2.IsBusy != true)
            {
                backgroundWorker2.RunWorkerAsync();
                pictureBox1.Visible = true;
            }
        }

        //Back ground work-DoWork, execute command
        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            int i, j;
            string DelayTime;
            byte[] sendCommandBuf, responseBuf = new byte[16];
            RemainingCount = CommandCount;
            i = 0;
            j = 0;

            while (i < CommandList.Count)
            {
                //Execute delay function
                if (CommandList[i].Contains("d"))
                {
                    DelayTime = CommandList[i].Trim('d');
                    System.Threading.Thread.Sleep(Int32.Parse(DelayTime));
                    backgroundWorker2.ReportProgress(0, "-> delay " + DelayTime + "ms\r\n");
                }
                //Execute command function
                else
                {
                    sendCommandBuf = StringToByteArray(CommandList[i]);
                    responseBuf = new byte[16];
                    backgroundWorker2.ReportProgress(0, ResponseCommand[j]);

                    Global.ENUM_CMD result = LakeChabotReader.MANAGED_ACCESS.Command_Test(sendCommandBuf, ref responseBuf);
                    RemainingCount--;

                    // Timeout
                    if (Global.ENUM_CMD.TIME_OUT == result)
                    {
                        backgroundWorker2.ReportProgress(0, "->" + "Time Out！" + "\r\n");
                        return;
                    }
                    // Result Error
                    else if (Global.ENUM_CMD.COMMAND_TEST != result)
                    {
                        backgroundWorker2.ReportProgress(0, "->" + "No Response！" + "\r\n");
                        return;
                    }
                    //Conpute the remaining command count 
                    backgroundWorker2.ReportProgress(0, "->" + BitConverter.ToString(responseBuf, 0, responseBuf.Length).Replace("-", " ") + "\r\n");
                    j++;
                }
                i++;
            }            
        }

        //Back ground work-ProgressChanged, response command
        private void backgroundWorker2_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            LabelRemainingCount.Text = RemainingCount.ToString();
            TextBoxScriptResponse_Refresh((String)e.UserState);
        }

        //Back ground work-RunWorkerCompleted,completed back ground worker
        private void backgroundWorker2_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            pictureBox1.Visible = false;
        }
        //End by FJ for implement the script command test function, 2017-01-13
    }
}
