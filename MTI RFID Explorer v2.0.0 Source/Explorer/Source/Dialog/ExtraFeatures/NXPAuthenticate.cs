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
 * $Id: NXPAuthentication.cs,v 1.0 2017/12/18 14:05:18 Willy(FJ) Exp $
 * 
 * Description: 
 *
 *****************************************************************************
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using System.Threading;
using System.Runtime.InteropServices;
using System.Text;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.IO;
using System.Linq;



using RFID.RFIDInterface;
using Global;
using rfid;
using rfid.Structures;
using rfid.Constants;


namespace RFID_Explorer
{
    partial class NXPAuthentication : Form
    {

        private static RFID_Explorer.mainForm m_mainForm = null;
        private LakeChabotReader m_reader = null;
        private bool customerKey0 = false;
        private bool customerKey1 = false;
        private byte retryCount = 0x00;
        private UInt32 accessPassword = 0x00;
        private int tagErrorCode = 19, moduleErrorCode1 = 20, moduleErrorCode2 = 21; //Reference the "MTI RFID Module Command Reference Manual-Table 3.10 - ISO 18000-6C Tag-Access Packet Fields"


        public NXPAuthentication(RFID_Explorer.mainForm r_form, LakeChabotReader rm_reader)
        {
            InitializeComponent();
            
            m_mainForm = r_form;
            m_reader = rm_reader;
            int i;
            
            comboBoxMemBank.Items.Add("EPC");
            comboBoxMemBank.Items.Add("TID");
            comboBoxMemBank.Items.Add("USER");
            comboBoxMemBank.SelectedIndex = 0;

            comboBoxBlockCount.Items.Add("One 64-bit Block");
            comboBoxBlockCount.Items.Add("Two 64-bit Block");
            comboBoxBlockCount.SelectedIndex = 0;

            retryCount = r_form.GetRetryCount();
            
            btnInsertKey.Enabled = false;
            btnActivateKey.Enabled = false;
        }

        private void btnGetKey_Click(object sender, EventArgs e)
        {
            int i;
            int dataEPCStart = 28, dataEPCEnd = 39; //Reference the "MTI RFID Module Command Reference Manual-Table 3.9 - ISO 18000-6C Inventory-Response Packet Fields"
            int dataKeyStart = 26, dataKeyEnd = 41; //Reference the "MTI RFID Module Command Reference Manual-Table 3.10 - ISO 18000-6C Tag-Access Packet Fields"
            string tempKeyEPC = "", tempKey0 = "", tempKey1 = "";
            bool getKey0Flag = false, getKey1Flag = false;
            byte selectOpsFlag = 0, postMatchFlag = 0;

            textBoxEPC.Text = "";
            textBoxInserKey0.Text = "";
            textBoxInserKey1.Text = "";
            btnInsertKey.Enabled = false;
            btnActivateKey.Enabled = false;
            selectOpsFlag = (chkPerformSelectOps.Checked == true) ? (byte)1 : (byte)0;
            postMatchFlag = (chkPerformPostMatch.Checked == true) ? (byte)1 : (byte)0;
           
            //Read key0 value
            ReadParms parameters = new ReadParms();
            parameters.readCmdParms.bank = MemoryBank.USER;
            parameters.readCmdParms.offset = 192; //C0
            parameters.readCmdParms.count = 8;
            parameters.common.strcTagFlag.RetryCount = retryCount;
            parameters.common.strcTagFlag.SelectOpsFlag = selectOpsFlag;
            parameters.common.strcTagFlag.PostMatchFlag = postMatchFlag;
            parameters.accessPassword = Convert.ToUInt32(TEXTBOX_TagAccessAccessPassword.Text, 16);
            parameters.common.callback = new rfid.CallbackDelegate(this.m_reader.MyCallback);
            parameters.common.OpMode = RadioOperationMode.NONCONTINUOUS;

            parameters.common.tagFlag = false;
            Array.Clear(parameters.common.bufMTII, 0, parameters.common.bufMTII.Length);
            Array.Clear(parameters.common.bufMTIA, 0, parameters.common.bufMTIA.Length);

            if (Result.OK != LakeChabotReader.MANAGED_ACCESS.API_l8K6CTagRead(parameters, 0))
            {
                MessageDisplay("Read key data command failed", "Key0 Information", MessageBoxIcon.Error);
            }
            else if (parameters.common.tagFlag != true)
            {
                MessageDisplay("No Tag", "Key0 Information", MessageBoxIcon.Warning);
            }
            else
            {
                if (parameters.common.bufMTIA[tagErrorCode] != 0x00)
                {
                    MessageDisplay("Tag Error Code : 0x" + parameters.common.bufMTIA[19].ToString("X2"), 
                                    "Key0 Information", MessageBoxIcon.Error);
                }
                else if (parameters.common.bufMTIA[moduleErrorCode1] != 0x00 || parameters.common.bufMTIA[moduleErrorCode2] != 0x00)
                {
                    MessageDisplay("Module Error Code : 0x" + parameters.common.bufMTIA[moduleErrorCode1].ToString("X2") + parameters.common.bufMTIA[moduleErrorCode2].ToString("X2"),
                                    "Key0 Information", MessageBoxIcon.Error);
                }
                else
                {
                    getKey0Flag = true;
                    tempKeyEPC = parameters.common.bufMTII[dataEPCStart].ToString("X2");
                    for (i = dataEPCStart + 1; i < dataEPCEnd + 1 ; i++)
                        tempKeyEPC += "-" + parameters.common.bufMTII[i].ToString("X2");

                    for (i = dataKeyStart; i < dataKeyEnd + 1 ; i++)
                        tempKey0 += parameters.common.bufMTIA[i].ToString("X2");
                }
            }


            //Read key1 value
            parameters.readCmdParms.bank = MemoryBank.USER;
            parameters.readCmdParms.offset = 208; //D0
            parameters.readCmdParms.count = 8;

            parameters.common.tagFlag = false;
            Array.Clear(parameters.common.bufMTII, 0, parameters.common.bufMTII.Length);
            Array.Clear(parameters.common.bufMTIA, 0, parameters.common.bufMTIA.Length);

            if ( Result.OK != LakeChabotReader.MANAGED_ACCESS.API_l8K6CTagRead(parameters, 0) )
            {
                MessageDisplay("Read key data command failed", "Key1 Information", MessageBoxIcon.Error);
            }
            else if (parameters.common.tagFlag != true)
            {
                MessageDisplay("No Tag", "Key1 Information", MessageBoxIcon.Warning);
            }
            else
            {
                if( parameters.common.bufMTIA[tagErrorCode] != 0x00 )
                {
                    MessageDisplay("Tag Error Code : 0x" + parameters.common.bufMTIA[19].ToString("X2"), 
                                    "Key1 Information", MessageBoxIcon.Error);
                }
                else if (parameters.common.bufMTIA[moduleErrorCode1] != 0x00 || parameters.common.bufMTIA[moduleErrorCode2] != 0x00 )
                {
                    MessageDisplay("Module Error Code : 0x" + parameters.common.bufMTIA[moduleErrorCode1].ToString("X2") + parameters.common.bufMTIA[moduleErrorCode2].ToString("X2"),
                                    "Key1 Information", MessageBoxIcon.Error);
                }
                else
                {
                    getKey1Flag = true;
                    for (i = dataKeyStart; i < dataKeyEnd + 1 ; i++)
                        tempKey1 += parameters.common.bufMTIA[i].ToString("X2");
                }
            }

            if ( getKey0Flag != true || getKey1Flag != true )
            {
                MessageDisplay("Get Keys Fail", "Keys Information", MessageBoxIcon.Error);
            }
            else
            {
                textBoxEPC.Text = tempKeyEPC;
                textBoxInserKey0.Text = tempKey0;
                textBoxInserKey1.Text = tempKey1;
                btnInsertKey.Enabled = true;
                btnActivateKey.Enabled = true;

                MessageDisplay("Get Keys Success", "Keys Information", MessageBoxIcon.Information);   
            }

        }

        private void btnInsertKey_Click(object sender, EventArgs e)
        {
            int i, key0Length, key1Length;
            byte flag = 0, selectOpsFlag = 0, postMatchFlag = 0;
            bool writeKey0Flag = false, writeKey1Flag = false;
            
            if ( (textBoxInserKey0.Text.Length / 2) < 16 )
            {
                MessageDisplay("Please enter 16 bytes hexadecimal value for Key0", "Key0 Information", MessageBoxIcon.Error);  
            }
            else if ( (textBoxInserKey1.Text.Length / 2) < 16 )
            {
                MessageDisplay("Please enter 16 bytes hexadecimal value for Key1", "Key1 Information", MessageBoxIcon.Error);
            }
            else if ( CheckKeyType(textBoxInserKey0.Text) != true )
            {
                MessageDisplay("Key0 is invalid hexadecimal value", "Key0 Information", MessageBoxIcon.Error);
            }
            else if ( CheckKeyType(textBoxInserKey1.Text) != true )
            {
                MessageDisplay("Key1 is invalid hexadecimal value", "Key1 Information", MessageBoxIcon.Error);
            }
            else
            {
                selectOpsFlag = (chkPerformSelectOps.Checked == true) ? (byte)1 : (byte)0;
                postMatchFlag = (chkPerformPostMatch.Checked == true) ? (byte)1 : (byte)0;

                key0Length = textBoxInserKey0.Text.Length;
                short[] key0 = new short[key0Length / 4];

                for (i = 0; i < key0Length; i += 4)
                    key0[i / 4] = Convert.ToInt16(textBoxInserKey0.Text.Substring(i, 4), 16);

                //Write key0 value
                WriteParms paramKey0 = new WriteParms();
                WriteSequentialParms writeparamKey0 = new WriteSequentialParms();
                writeparamKey0.bank = MemoryBank.USER;
                writeparamKey0.count = 8;
                writeparamKey0.offset = 192; //C0
                writeparamKey0.pData = new ushort[8];
                writeparamKey0.pData[0] = (ushort)key0[0];
                writeparamKey0.pData[1] = (ushort)key0[1];
                writeparamKey0.pData[2] = (ushort)key0[2];
                writeparamKey0.pData[3] = (ushort)key0[3];
                writeparamKey0.pData[4] = (ushort)key0[4];
                writeparamKey0.pData[5] = (ushort)key0[5];
                writeparamKey0.pData[6] = (ushort)key0[6];
                writeparamKey0.pData[7] = (ushort)key0[7];
                paramKey0.accessPassword = Convert.ToUInt32(TEXTBOX_TagAccessAccessPassword.Text, 16);
                paramKey0.writeParms = writeparamKey0;
                paramKey0.writeType = WriteType.SEQUENTIAL;
                paramKey0.common.callback = new rfid.CallbackDelegate(this.m_reader.MyCallback);

                paramKey0.common.strcTagFlag.RetryCount = retryCount;
                paramKey0.common.strcTagFlag.SelectOpsFlag = selectOpsFlag;
                paramKey0.common.strcTagFlag.PostMatchFlag = postMatchFlag;
                paramKey0.common.OpMode = RadioOperationMode.NONCONTINUOUS;

                Array.Clear(paramKey0.common.bufMTIA, 0, paramKey0.common.bufMTIA.Length);
                paramKey0.common.tagFlag = false;

                writeKey0Flag = true;
                if (Result.OK != LakeChabotReader.MANAGED_ACCESS.API_l8K6CTagWrite(paramKey0, flag))
                {
                    writeKey0Flag = false;
                    MessageDisplay("Write Key0 data command failed", "Key0 Information", MessageBoxIcon.Error);
                }
                else if (paramKey0.common.tagFlag != true)
                {
                    writeKey0Flag = false;
                    MessageDisplay("No Tag", "Key0 Information", MessageBoxIcon.Warning);
                }
                else if (paramKey0.common.bufMTIA[tagErrorCode] != 0x00)
                {
                    writeKey0Flag = false;
                    MessageDisplay("Tag Error Code : 0x" + paramKey0.common.bufMTIA[19].ToString("X2"),
                                    "Key0 Information", MessageBoxIcon.Error);
                }
                else if (paramKey0.common.bufMTIA[moduleErrorCode1] != 0x00 || paramKey0.common.bufMTIA[moduleErrorCode2] != 0x00)
                {
                    writeKey0Flag = false;
                    MessageDisplay("Module Error Code : 0x" + paramKey0.common.bufMTIA[moduleErrorCode1].ToString("X2") + paramKey0.common.bufMTIA[moduleErrorCode2].ToString("X2"),
                                    "Key0 Information", MessageBoxIcon.Error);
                }


                key1Length = textBoxInserKey1.Text.Length;
                short[] key1 = new short[key1Length / 4];

                for (i = 0; i < key0Length; i += 4)
                    key1[i / 4] = Convert.ToInt16(textBoxInserKey1.Text.Substring(i, 4), 16);

                //Write key1 value
                WriteParms paramKey1 = new WriteParms();
                WriteSequentialParms writeparamKey1 = new WriteSequentialParms();
                writeparamKey1.bank = MemoryBank.USER;
                writeparamKey1.count = 8;
                writeparamKey1.offset = 208; //D0
                writeparamKey1.pData = new ushort[8];
                writeparamKey1.pData[0] = (ushort)key1[0];
                writeparamKey1.pData[1] = (ushort)key1[1];
                writeparamKey1.pData[2] = (ushort)key1[2];
                writeparamKey1.pData[3] = (ushort)key1[3];
                writeparamKey1.pData[4] = (ushort)key1[4];
                writeparamKey1.pData[5] = (ushort)key1[5];
                writeparamKey1.pData[6] = (ushort)key1[6];
                writeparamKey1.pData[7] = (ushort)key1[7];
                paramKey1.accessPassword = Convert.ToUInt32(TEXTBOX_TagAccessAccessPassword.Text, 16);
                paramKey1.writeParms = writeparamKey1;
                paramKey1.writeType = WriteType.SEQUENTIAL;
                paramKey1.common.callback = new rfid.CallbackDelegate(this.m_reader.MyCallback);

                paramKey1.common.strcTagFlag.RetryCount = retryCount;
                paramKey1.common.strcTagFlag.SelectOpsFlag = selectOpsFlag;
                paramKey1.common.strcTagFlag.PostMatchFlag = postMatchFlag;
                paramKey1.common.OpMode = RadioOperationMode.NONCONTINUOUS;

                Array.Clear(paramKey1.common.bufMTIA, 0, paramKey1.common.bufMTIA.Length);
                paramKey1.common.tagFlag = false;

                writeKey1Flag = true;
                if (Result.OK != LakeChabotReader.MANAGED_ACCESS.API_l8K6CTagWrite(paramKey1, flag))
                {
                    writeKey1Flag = false;
                    MessageDisplay("Write Key1 data command failed", "Key1 Information", MessageBoxIcon.Error);
                }
                else if (paramKey1.common.tagFlag != true)
                {
                    writeKey1Flag = false;
                    MessageDisplay("No Tag", "Key1 Information", MessageBoxIcon.Warning);
                }
                else if (paramKey1.common.bufMTIA[tagErrorCode] != 0x00)
                {
                    writeKey1Flag = false;
                    MessageDisplay("Tag Error Code : 0x" + paramKey1.common.bufMTIA[19].ToString("X2"),
                                    "Key1 Information", MessageBoxIcon.Error);
                }
                else if (paramKey1.common.bufMTIA[moduleErrorCode1] != 0x00 || paramKey1.common.bufMTIA[moduleErrorCode2] != 0x00)
                {
                    writeKey1Flag = false;
                    MessageDisplay("Module Error Code : 0x" + paramKey1.common.bufMTIA[moduleErrorCode1].ToString("X2") + paramKey1.common.bufMTIA[moduleErrorCode2].ToString("X2"),
                                    "Key1 Information", MessageBoxIcon.Error);
                }


                if (writeKey0Flag != true || writeKey1Flag != true)
                {
                    textBoxEPC.Text = "";
                    textBoxInserKey0.Text = "";
                    textBoxInserKey1.Text = "";
                    btnInsertKey.Enabled = false;
                    btnActivateKey.Enabled = false;
                    MessageDisplay("Insert Keys Fail", "Keys Information", MessageBoxIcon.Error);
                }
                else
                {
                    MessageDisplay("Insert Keys Success", "Keys Information", MessageBoxIcon.Information);
                }
            }
        }

        private void btnActivateKey_Click(object sender, EventArgs e)
        {
            byte flag = 0, selectOpsFlag = 0, postMatchFlag = 0;
            bool writeParamActivateKey0Flag = false, writeParamActivateKey1Flag = false;
            
            if ((textBoxInserKey0.Text.Length / 2) < 16)
            {
                MessageDisplay("Please enter 16 bytes hexadecimal value for Key0", "Key0 Information", MessageBoxIcon.Error);
            }
            else if ((textBoxInserKey1.Text.Length / 2) < 16)
            {
                MessageDisplay("Please enter 16 bytes hexadecimal value for Key1", "Key1 Information", MessageBoxIcon.Error);
            }
            else if ( CheckKeyType(textBoxInserKey0.Text ) != true)
            {
                MessageDisplay("Key0 is invalid hexadecimal value", "Key0 Information", MessageBoxIcon.Error);
            }
            else if ( CheckKeyType(textBoxInserKey1.Text ) != true)
            {
                MessageDisplay("Key1 is invalid hexadecimal value", "Key1 Information", MessageBoxIcon.Error);
            }
            else 
            {
                selectOpsFlag = (chkPerformSelectOps.Checked == true) ? (byte)1 : (byte)0;
                postMatchFlag = (chkPerformPostMatch.Checked == true) ? (byte)1 : (byte)0;

                //Write key0 trigger value to activate the key0
                WriteParms paramActivateKey = new WriteParms();
                WriteSequentialParms writeParamActivateKey = new WriteSequentialParms();
                writeParamActivateKey.bank = MemoryBank.USER;
                writeParamActivateKey.count = 1;
                writeParamActivateKey.offset = 200; //C8 : Key0 header
                writeParamActivateKey.pData = new ushort[1];
                writeParamActivateKey.pData[0] = 0xE200;
                paramActivateKey.accessPassword = Convert.ToUInt32(TEXTBOX_TagAccessAccessPassword.Text, 16);
                paramActivateKey.writeParms = writeParamActivateKey;
                paramActivateKey.writeType = WriteType.SEQUENTIAL;
                paramActivateKey.common.callback = new rfid.CallbackDelegate(this.m_reader.MyCallback);

                paramActivateKey.common.strcTagFlag.RetryCount = retryCount;
                paramActivateKey.common.strcTagFlag.SelectOpsFlag = selectOpsFlag;
                paramActivateKey.common.strcTagFlag.PostMatchFlag = postMatchFlag;
                paramActivateKey.common.OpMode = RadioOperationMode.NONCONTINUOUS;

                Array.Clear(paramActivateKey.common.bufMTIA, 0, paramActivateKey.common.bufMTIA.Length);
                paramActivateKey.common.tagFlag = false;

                writeParamActivateKey0Flag = true;
                if (Result.OK != LakeChabotReader.MANAGED_ACCESS.API_l8K6CTagWrite(paramActivateKey, flag))
                {
                    writeParamActivateKey0Flag = false;
                    MessageDisplay("Activate the Key0 command failed.", "Key0 Information", MessageBoxIcon.Error);
                }
                else if (paramActivateKey.common.tagFlag != true)
                {
                    writeParamActivateKey0Flag = false;
                    MessageDisplay("No Tag", "Key0 Information", MessageBoxIcon.Warning);
                }
                else if (paramActivateKey.common.bufMTIA[tagErrorCode] != 0x00)
                {
                    writeParamActivateKey0Flag = false;
                    MessageDisplay("Tag Error Code : 0x" + paramActivateKey.common.bufMTIA[19].ToString("X2"),
                                    "Key0 Information", MessageBoxIcon.Error);
                }
                else if (paramActivateKey.common.bufMTIA[moduleErrorCode1] != 0x00 || paramActivateKey.common.bufMTIA[moduleErrorCode2] != 0x00)
                {
                    writeParamActivateKey0Flag = false;
                    MessageDisplay("Module Error Code : 0x" + paramActivateKey.common.bufMTIA[moduleErrorCode1].ToString("X2") + paramActivateKey.common.bufMTIA[moduleErrorCode2].ToString("X2"),
                                    "Key0 Information", MessageBoxIcon.Error);
                }



                //Write key1 trigger value to activate the key1
                writeParamActivateKey.bank = MemoryBank.USER;
                writeParamActivateKey.count = 1;
                writeParamActivateKey.offset = 216; //D8 : Key1 header
                writeParamActivateKey.pData = new ushort[1];
                writeParamActivateKey.pData[0] = 0xE200;
                paramActivateKey.accessPassword = Convert.ToUInt32(TEXTBOX_TagAccessAccessPassword.Text, 16);
                paramActivateKey.writeParms = writeParamActivateKey;
                paramActivateKey.writeType = WriteType.SEQUENTIAL;
                paramActivateKey.common.callback = new rfid.CallbackDelegate(this.m_reader.MyCallback);

                paramActivateKey.common.strcTagFlag.RetryCount = retryCount;
                paramActivateKey.common.strcTagFlag.SelectOpsFlag = selectOpsFlag;
                paramActivateKey.common.strcTagFlag.PostMatchFlag = postMatchFlag;
                paramActivateKey.common.OpMode = RadioOperationMode.NONCONTINUOUS;

                Array.Clear(paramActivateKey.common.bufMTIA, 0, paramActivateKey.common.bufMTIA.Length);
                paramActivateKey.common.tagFlag = false;

                writeParamActivateKey1Flag = true;
                if (Result.OK != LakeChabotReader.MANAGED_ACCESS.API_l8K6CTagWrite(paramActivateKey, flag))
                {
                    writeParamActivateKey1Flag = false;
                    MessageDisplay("Activate the Key1 command failed.", "Key1 Information", MessageBoxIcon.Error);
                }
                else if (paramActivateKey.common.tagFlag != true)
                {
                    writeParamActivateKey1Flag = false;
                    MessageDisplay("No Tag", "Key1 Information", MessageBoxIcon.Warning);
                }
                else if (paramActivateKey.common.bufMTIA[tagErrorCode] != 0x00)
                {
                    writeParamActivateKey1Flag = false;
                    MessageDisplay("Tag Error Code : 0x" + paramActivateKey.common.bufMTIA[19].ToString("X2"),
                                    "Key1 Information", MessageBoxIcon.Error);
                }
                else if (paramActivateKey.common.bufMTIA[moduleErrorCode1] != 0x00 || paramActivateKey.common.bufMTIA[moduleErrorCode2] != 0x00)
                {
                    writeParamActivateKey1Flag = false;
                    MessageDisplay("Module Error Code : 0x" + paramActivateKey.common.bufMTIA[moduleErrorCode1].ToString("X2") + paramActivateKey.common.bufMTIA[moduleErrorCode2].ToString("X2"),
                                    "Key1 Information", MessageBoxIcon.Error);
                }
                
                if (writeParamActivateKey0Flag != true || writeParamActivateKey1Flag != true)
                {
                    textBoxEPC.Text = "";
                    textBoxInserKey0.Text = "";
                    textBoxInserKey1.Text = "";
                    btnInsertKey.Enabled = false;
                    btnActivateKey.Enabled = false;
                    MessageDisplay("Activate Keys Fail", "Keys Information", MessageBoxIcon.Error);
                }
                else
                {
                    MessageDisplay("Activate Keys Success", "Keys Information", MessageBoxIcon.Information);
                }
            }
        }

        private void btnDefaultKey0_Click(object sender, EventArgs e)
        {
            //MTI default Key0
            textBoxVerificationKey0.Text = "0123456789ABCDEF0123456789ABCDEF";
        }

        private void btnDefaultKey1_Click(object sender, EventArgs e)
        {
            //MTI default Key1
            textBoxVerificationKey1.Text = "96910E5C3B1D4B16E19C6B62654067A7";
        }   

        private void btnTAM1Authenticate_Click(object sender, EventArgs e)
        {
            byte flag = 0, performOptionFlag = 0;
            int dataEPCStart = 28, dataEPCEnd = 39; //Reference the "MTI RFID Module Command Reference Manual-Table 3.9 - ISO 18000-6C Inventory-Response Packet Fields"
            int dataMessageStart = 26, dataMessageStop = 35, dataCipherStart = 36, dataCipherStop = 51; //Reference the "MTI RFID Module Command Reference Manual-Table 3.10 - ISO 18000-6C Tag-Access Packet Fields"
            int i;
            string message = "", key = "", ciphertext = "", randomChallenge = "";

            labelTagMemoryData.Text = "";
            
            if ( (textBoxVerificationKey0.Text.Length / 2) < 16 )
            {
                MessageDisplay("Please enter 16 bytes hexadecimal value for verification Key0",
                                "TAM1 Authentication", MessageBoxIcon.Error);
            }
            else if ( CheckKeyType(textBoxVerificationKey0.Text) != true )
            {
                MessageDisplay("Verification Key0 is invalid hexadecimal value", "TAM1 Authentication", MessageBoxIcon.Error);
            }
            else
            {
                if (chkPerformSelectOps.Checked == false && chkPerformPostMatch.Checked == false)
                    performOptionFlag = 0;
                else if (chkPerformSelectOps.Checked == true && chkPerformPostMatch.Checked == false)
                    performOptionFlag = 1;
                else if (chkPerformSelectOps.Checked == false && chkPerformPostMatch.Checked == true)
                    performOptionFlag = 2;
                else
                    performOptionFlag = 3;

                NXPChangeConfig NXPConfigParms = new NXPChangeConfig();
                NXPTAM1Authenticate NXPTAM1Parms = new NXPTAM1Authenticate();
                NXPTAM2Authenticate NXPTAM2Parms = new NXPTAM2Authenticate();
                ReadParms parameters = new ReadParms();

                NXPTAM1Parms.replySetting = (byte)1;
                NXPTAM1Parms.keyID = (byte)0;
                NXPTAM1Parms.retryCount = retryCount;
                NXPTAM1Parms.performOption = performOptionFlag;

                parameters.accessPassword = Convert.ToUInt32(TEXTBOX_TagAccessAccessPassword.Text, 16);
                parameters.common.tagFlag = false;
                Array.Clear(parameters.common.bufMTII, 0, parameters.common.bufMTII.Length);
                Array.Clear(parameters.common.bufMTIA, 0, parameters.common.bufMTIA.Length);
                
                if (Result.OK != LakeChabotReader.MANAGED_ACCESS.API_l8K6CTagNXPSpecific(NXPSpecificFunID.TAM1Authenticate, NXPConfigParms, NXPTAM1Parms, NXPTAM2Parms, parameters, flag))
                {
                    labelRandomChallenge.Text = labelDecryptedChallenge.Text = "";
                    MessageDisplay("TAM1 authenticate command failed", "TAM1 Authentication", MessageBoxIcon.Error);
                }
                else if (parameters.common.tagFlag != true)
                {
                    labelRandomChallenge.Text = labelDecryptedChallenge.Text = "";
                    MessageDisplay("No Tag", "TAM1 Authentication", MessageBoxIcon.Warning);
                }
                else if (parameters.common.bufMTIA[tagErrorCode] != 0x00)
                {
                    labelRandomChallenge.Text = labelDecryptedChallenge.Text = "";
                    MessageDisplay("Tag Error Code : 0x" + parameters.common.bufMTIA[19].ToString("X2"),
                                    "TAM1 Authentication", MessageBoxIcon.Error);
                }
                else if (parameters.common.bufMTIA[moduleErrorCode1] != 0x00 || parameters.common.bufMTIA[moduleErrorCode2] != 0x00)
                {
                    labelRandomChallenge.Text = labelDecryptedChallenge.Text = "";
                    MessageDisplay("Module Error Code : 0x" + parameters.common.bufMTIA[moduleErrorCode1].ToString("X2") + parameters.common.bufMTIA[moduleErrorCode2].ToString("X2"),
                                    "TAM1 Authentication", MessageBoxIcon.Error);
                }
                else
                {
                    textBoxEPC.Text = parameters.common.bufMTII[dataEPCStart].ToString("X2");
                    for (i = dataEPCStart + 1; i < dataEPCEnd + 1 ; i++)
                        textBoxEPC.Text += "-" + parameters.common.bufMTII[i].ToString("X2");

                    message = parameters.common.bufMTIA[dataMessageStart].ToString("X2");
                    labelRandomChallenge.Text = parameters.common.bufMTIA[dataMessageStart].ToString("X2");
                    for (i = dataMessageStart + 1; i < dataMessageStop + 1 ; i++)
                    {
                        message += parameters.common.bufMTIA[i].ToString("X2");
                        labelRandomChallenge.Text += " " + parameters.common.bufMTIA[i].ToString("X2");
                    }

                    for (i = dataCipherStart; i < dataCipherStop + 1 ; i++)
                        ciphertext += parameters.common.bufMTIA[i].ToString("X2");

                    key = textBoxVerificationKey0.Text;

                    randomChallenge = AESECBDecrypt(ciphertext, key);

                    labelDecryptedChallenge.Text = randomChallenge.Substring(0, 2);
                    for (i = 2; i < randomChallenge.Length; i += 2)
                        labelDecryptedChallenge.Text += " " + randomChallenge.Substring(i, 2);

                    if (String.Compare(randomChallenge, message) == 0)
                        MessageDisplay("Authenticate Tag Success", "TAM1 Authentication", MessageBoxIcon.Information);
                    else

                        MessageDisplay("Authenticate Tag Fail; Confirm Verification Key0 is Correct.", "TAM1 Authentication", MessageBoxIcon.Error);
                }
            }
        }

        public static string AESECBDecrypt(string ciphertext, string key)
        {
            string decryptedString = "";
            try
            {
                string iv = "00000000000000000000000000000000";
                int randomChallengeStart = 12, randomChallengeLength = 20;

                byte[] aeskey = HexStringToByteArray(key);
                byte[] aesIV = HexStringToByteArray(iv);
                decryptedString = DecryptHexStrings(ciphertext, aeskey, aesIV);

                decryptedString = decryptedString.Substring(randomChallengeStart, randomChallengeLength);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: {0}", ex.Message);
            }

            return decryptedString;
        }

       
        private void btnTAM2Authenticate_Click(object sender, EventArgs e)
        {
            byte flag = 0, performOptionFlag = 0;
            ushort offsetValue;
            bool result;
            int dataEPCStart = 28, dataEPCEnd = 39; //Reference the "MTI RFID Module Command Reference Manual-Table 3.9 - ISO 18000-6C Inventory-Response Packet Fields"
            int dataMessageStart = 26, dataMessageEnd = 35; //Reference the "MTI RFID Module Command Reference Manual-Table 3.10 - ISO 18000-6C Tag-Access Packet Fields"
            int dataCipher1Start = 36, dataCipher1End = 51, dataCipher2Start = 52, dataCipher2End = 61; //Reference the "MTI RFID Module Command Reference Manual-Table 3.10 - ISO 18000-6C Tag-Access Packet Fields"
            int i;
            string message = "", key = "", ciphertext1 = "", ciphertext2 = "", randomChallenge = "", returnedData = "";
            
            if ( (textBoxVerificationKey1.Text.Length / 2) < 16 )
            {                   
                MessageDisplay("Please enter 16 bytes hexadecimal value for verification Key1",
                                "TAM2 Authentication", MessageBoxIcon.Error);
            }
            else if ( CheckKeyType(textBoxVerificationKey1.Text) != true )
            {
                MessageDisplay("Verification Key1 is invalid hexadecimal value", "TAM2 Authentication", MessageBoxIcon.Error);
            }
            else if( Int16.Parse( textBoxOffset.Text ) > 4095 )
            {
                MessageDisplay("The offset value must be littler than 4095",
                                "TAM2 Authentication", MessageBoxIcon.Error);
            }
            else 
            {
                result = ushort.TryParse(textBoxOffset.Text, out offsetValue);

                if (chkPerformSelectOps.Checked == false && chkPerformPostMatch.Checked == false)
                    performOptionFlag = 0;
                else if (chkPerformSelectOps.Checked == true && chkPerformPostMatch.Checked == false)
                    performOptionFlag = 1;
                else if (chkPerformSelectOps.Checked == false && chkPerformPostMatch.Checked == true)
                    performOptionFlag = 2;
                else
                    performOptionFlag = 3;

                NXPChangeConfig NXPConfigParms = new NXPChangeConfig();
                NXPTAM1Authenticate NXPTAM1Parms = new NXPTAM1Authenticate();
                NXPTAM2Authenticate NXPTAM2Parms = new NXPTAM2Authenticate();
                ReadParms parameters = new ReadParms();

                NXPTAM2Parms.replySetting = (byte)1;
                NXPTAM2Parms.memoryProfile = (byte)comboBoxMemBank.SelectedIndex;
                NXPTAM2Parms.offset = offsetValue;
                NXPTAM2Parms.blockCount = (byte)comboBoxBlockCount.SelectedIndex;
                NXPTAM2Parms.retryCount = retryCount;
                NXPTAM2Parms.performOption = performOptionFlag;

                parameters.accessPassword = Convert.ToUInt32(TEXTBOX_TagAccessAccessPassword.Text, 16);
                parameters.common.tagFlag = false;
                Array.Clear(parameters.common.bufMTII, 0, parameters.common.bufMTII.Length);
                Array.Clear(parameters.common.bufMTIA, 0, parameters.common.bufMTIA.Length);
                Array.Clear(parameters.common.bufSecondeMTIA, 0, parameters.common.bufSecondeMTIA.Length);

                if (Result.OK != LakeChabotReader.MANAGED_ACCESS.API_l8K6CTagNXPSpecific(NXPSpecificFunID.TAM2Authenticate, NXPConfigParms, NXPTAM1Parms, NXPTAM2Parms, parameters, flag))
                {
                    labelRandomChallenge.Text = labelDecryptedChallenge.Text = labelTagMemoryData.Text = "";
                    MessageDisplay("TAM2 authenticate command failed", "TAM2 Authentication", MessageBoxIcon.Error);
                }
                else if (parameters.common.tagFlag != true)
                {
                    labelRandomChallenge.Text = labelDecryptedChallenge.Text = labelTagMemoryData.Text = "";
                    MessageDisplay("No Tag", "TAM2 Authentication", MessageBoxIcon.Warning);
                }
                else if (parameters.common.bufMTIA[tagErrorCode] != 0x00)
                {
                    labelRandomChallenge.Text = labelDecryptedChallenge.Text = labelTagMemoryData.Text = "";
                    MessageDisplay("Tag Error Code : 0x" + parameters.common.bufMTIA[19].ToString("X2"),
                                    "TAM2 Authentication", MessageBoxIcon.Error);
                }
                else if (parameters.common.bufMTIA[moduleErrorCode1] != 0x00 || parameters.common.bufMTIA[moduleErrorCode2] != 0x00)
                {
                    labelRandomChallenge.Text = labelDecryptedChallenge.Text = labelTagMemoryData.Text = "";
                    MessageDisplay("Module Error Code : 0x" + parameters.common.bufMTIA[moduleErrorCode1].ToString("X2") + parameters.common.bufMTIA[moduleErrorCode2].ToString("X2"),
                                    "TAM2 Authentication", MessageBoxIcon.Error);
                }
                else
                {
                    textBoxEPC.Text = parameters.common.bufMTII[dataEPCStart].ToString("X2");
                    for (i = dataEPCStart + 1; i < dataEPCEnd + 1 ; i++)
                        textBoxEPC.Text += "-" + parameters.common.bufMTII[i].ToString("X2");

                    message = parameters.common.bufMTIA[dataMessageStart].ToString("X2");
                    labelRandomChallenge.Text = parameters.common.bufMTIA[dataMessageStart].ToString("X2");
                    for (i = dataMessageStart + 1; i < dataMessageEnd + 1 ; i++)
                    {
                        message += parameters.common.bufMTIA[i].ToString("X2");
                        labelRandomChallenge.Text += " " + parameters.common.bufMTIA[i].ToString("X2");
                    }

                    for (i = dataCipher1Start; i < dataCipher1End + 1 ; i++)
                        ciphertext1 += parameters.common.bufMTIA[i].ToString("X2");

                    for (i = dataCipher2Start; i < dataCipher2End + 1 ; i++)
                        ciphertext2 += parameters.common.bufMTIA[i].ToString("X2");

                    dataCipher2Start = 14; //The second part of ciphertext2 start address  
                    dataCipher2End = 19; //The second part of ciphertext2 end address

                    for (i = dataCipher2Start; i < dataCipher2End + 1 ; i++)
                        ciphertext2 += parameters.common.bufSecondeMTIA[i].ToString("X2");

                    key = textBoxVerificationKey1.Text;

                    randomChallenge = AESECBDecrypt(ciphertext1, key);

                    returnedData = AESCBCDecrypt(ciphertext2, ciphertext1, key);

                    labelDecryptedChallenge.Text = randomChallenge.Substring(0, 2);
                    for (i = 2; i < randomChallenge.Length; i += 2)
                        labelDecryptedChallenge.Text += " " + randomChallenge.Substring(i, 2);

                    labelTagMemoryData.Text = returnedData.Substring(0, 2);
                    for (i = 2; i < returnedData.Length; i += 2)
                        labelTagMemoryData.Text += " " + returnedData.Substring(i, 2);

                    if (String.Compare(randomChallenge, message) == 0)
                        MessageDisplay("Authenticate Tag Success", "TAM2 Authentication", MessageBoxIcon.Information);
                    else
                        MessageDisplay("Authenticate Tag Fail; Confirm Verification Key1 is Correct.", "TAM2 Authentication", MessageBoxIcon.Error);

                }              
            }
        }

        public static string AESCBCDecrypt(string ciphertext, string iv, string key)
        {
            string decryptedCBCString = "";

                try
                {
                    string decryptMessage_2 = ciphertext;
                    byte[] aeskey;
                    byte[] aesIV;

                    aeskey = HexStringToByteArray(key);
                    aesIV = HexStringToByteArray(iv);
                    decryptedCBCString = CBCDecryptHexStrings(decryptMessage_2, aeskey, aesIV);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: {0}", ex.Message);
                }

                return decryptedCBCString;
        }

        public static byte[] HexStringToByteArray(string s)
        {
            byte[] ret = new byte[s.Length / 2];
            for (int i = 0; i < s.Length; i += 2)
            {
                ret[i / 2] = Convert.ToByte(s.Substring(i, 2), 16);
            }
            return ret;
        }

        public static string EncryptHexStrings(string message, byte[] aesKey, byte[] aesIV)
        {
            byte[] bytes = HexStringToByteArray(message);
            byte[] encryptBytes = EncryptBytes(bytes, aesKey, aesIV);
            return ByteArrayToHexString(encryptBytes);
        }

        public static string CBCEncryptHexStrings(string message, byte[] aesKey, byte[] aesIV)
        {
            byte[] bytes = HexStringToByteArray(message);
            byte[] encryptBytes = CBCEncryptBytes(bytes, aesKey, aesIV);
            return ByteArrayToHexString(encryptBytes);
        }

        public static byte[] EncryptBytes(byte[] encryptMessage, byte[] aesKey, byte[] aesIV)
        {
            Aes aes = new AesCryptoServiceProvider();
            aes.Padding = PaddingMode.None;
            aes.Mode = CipherMode.ECB;
            aes.Key = aesKey;
            aes.IV = aesIV;
            var enc = aes.CreateEncryptor(aes.Key, aes.IV);
            return enc.TransformFinalBlock(encryptMessage, 0, encryptMessage.Length);
        }

        public static byte[] CBCEncryptBytes(byte[] encryptMessage, byte[] aesKey, byte[] aesIV)
        {
            Aes aes = new AesCryptoServiceProvider();
            aes.Padding = PaddingMode.None;
            aes.Mode = CipherMode.CBC;
            aes.Key = aesKey;
            aes.IV = aesIV;
            var enc = aes.CreateEncryptor(aes.Key, aes.IV);
            return enc.TransformFinalBlock(encryptMessage, 0, encryptMessage.Length);
        }

        public static string DecryptHexStrings(string message, byte[] aesKey, byte[] aesIV)
        {
            byte[] bytes = HexStringToByteArray(message);
            byte[] decryptBytes = DecryptBytes(bytes, aesKey, aesIV);
            return ByteArrayToHexString(decryptBytes);
        }

        public static string CBCDecryptHexStrings(string message, byte[] aesKey, byte[] aesIV)
        {
            byte[] bytes = HexStringToByteArray(message);
            byte[] decryptBytes = CBCDecryptBytes(bytes, aesKey, aesIV);
            return ByteArrayToHexString(decryptBytes);
        }  

        public static byte[] DecryptBytes(byte[] decryptMessage, byte[] aesKey, byte[] aesIV)
        {
            Aes aes = new AesCryptoServiceProvider();
            aes.Padding = PaddingMode.None;
            aes.Mode = CipherMode.ECB;
            aes.Key = aesKey;
            aes.IV = aesIV;
            var dec = aes.CreateDecryptor(aes.Key, aes.IV);
            return dec.TransformFinalBlock(decryptMessage, 0, decryptMessage.Length);
        }

        public static byte[] CBCDecryptBytes(byte[] decryptMessage, byte[] aesKey, byte[] aesIV)
        {
            Aes aes = new AesCryptoServiceProvider();
            aes.Padding = PaddingMode.None;
            aes.Mode = CipherMode.CBC;
            aes.Key = aesKey;
            aes.IV = aesIV;
            var dec = aes.CreateDecryptor(aes.Key, aes.IV);
            return dec.TransformFinalBlock(decryptMessage, 0, decryptMessage.Length);
        }

        public static string ByteArrayToHexString(byte[] bytes)
        {
            var sb = new StringBuilder();
            foreach (byte b in bytes)
                sb.AppendFormat("{0:X2}", b);
            return sb.ToString();
        }

        public void MessageDisplay(string message, string messageType, MessageBoxIcon messageLevel)
        {
            MessageBox.Show(message,
                messageType, MessageBoxButtons.OK,
                messageLevel);
        }

        public static bool CheckKeyType(string key)
        {
            char keyChar;
            bool flag = false;

            for (int i = 0; i < key.Length; i++)
            {
                keyChar = Convert.ToChar(key.Substring(i, 1));
                if ((keyChar >= '0' && keyChar <= '9')  //0-9
                 || (keyChar >= 'A' && keyChar <= 'F')  //A-F
                 || (keyChar >= 'a' && keyChar <= 'f')  //a-f
                 || (keyChar == 0x20))                  //Space
                {
                    flag = true;
                }
                else
                {
                    return false;
                }
            }
            return flag;
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
    }
}
