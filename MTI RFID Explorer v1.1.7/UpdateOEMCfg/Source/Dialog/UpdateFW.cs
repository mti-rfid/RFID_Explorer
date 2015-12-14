using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using System.IO;

using rfid.Structures;
using rfid.Constants;
using UpdateOEMCfgTool.Global;
using System.Runtime.InteropServices;

using Global;
using RFID.RFIDInterface;
using System.ComponentModel;

namespace UpdateOEMCfgTool
{

    public partial class Update : Form
    {
        string sLine = ""; string ReaderOEMNumber;
        string addLine, dataLine;
        string[] OEMLine;

        Interface           m_clsInterface   = new Interface();
        BackgroundWorker    m_clsBackThread  = new BackgroundWorker();
        OpenFileDialog      m_clsOpenFileDlg = new OpenFileDialog(); 
        ClsRegisterHidEvent m_clsRegHidEvent = new ClsRegisterHidEvent();   
        DEV_MODE            m_Mode           = DEV_MODE.NO;
        UInt16 OEMAddress; UInt32 OEMData, checkOEMData;
        uint uiCurError = 0;        uint uiLastError = 0;
        bool UpdateOEMCfgCompleteFlag = false, OEMPercentageFlag=false;
        int iRealReadCountOEM = 0, iRealReadCountOEMTopHalf=0, iRealReadCountOEMBtnHalf=0;
        double OEMPercentage = 0.0;
        int spaceLine = 0;



        //File
        FileStream m_fileStream  = null;
        long       m_lFileSize   = 0;
        uint       m_uiFileCount = 0;
        bool       m_bTestMode   = false;
        string     m_strFilePath;


        //Delegate
        private CONTROL_ITEM dlgControlItem= null;

        //Define
        const int DEFINE_UPDATE_DATA_SIZE    = 32;
        const int DEFINE_UPDATE_DATA_ADDRESS = 0x00104000;


        public Update()
        {
            InitializeComponent();  
          
            //Delegate
            dlgControlItem    = new CONTROL_ITEM(ControlItem);

            //Register HID Event for receiving DBT_DEVICE_ARRIVAL and DBT_DEVICE_REMOVE_COMPLETE event puring PNP          
            m_clsRegHidEvent.RegisterHidEvent(this.Handle);
        }



//==========================Other=========================================
        /*private DEV_MODE AskMode()
        {
            Result result = Result.FAILURE;

            do
            {
                //Check BL Mode
                result = GetVersion(DEV_MODE.BL);

                if ( Result.OK == result )
                {
                    m_Mode = DEV_MODE.BL;
                    break;
                } 
                else if(  Result.FWUPD_CMD_IGN == result  )
                {
                    m_Mode = DEV_MODE.BL_ENTER_UPDATE;
                    break;
                } 

                //Check AP mode
                if ( Result.OK == GetVersion(DEV_MODE.AP) )
                {
                    m_Mode = DEV_MODE.AP;
                    break;
                }  

                m_Mode = DEV_MODE.NO;

            }while(false);                     

            return m_Mode;
        }*/

        private void GetOEMVersion()
        {
            OEMCfgVersion Oversion = new OEMCfgVersion();
            OEMCfgUpdateNumber OversionUpdateNumber = new OEMCfgUpdateNumber();
            //取得OEM版號
            Result result = Result.FAILURE;
            result = m_clsInterface.API_MacGetOEMCfgVersion(ref Oversion);
            result = m_clsInterface.API_MacGetOEMCfgUpdateNumber(ref OversionUpdateNumber);
            /*ControlItem(ENUM_ITEM_TYPE.TEXT_VER, String.Format("[OEM] {0}.{1}.{2}", 
                (char)Oversion.major, 
                (char)Oversion.minor, 
                (char)Oversion.release));*/
            /*textBoxOEMVersion.Text=String.Format("[OEM] {0}.{1}{2}-up{3}{4}",
                (char)Oversion.major,
                (char)Oversion.minor,
                (char)Oversion.release,
                (char)OversionUpdateNumber.major,
                (char)OversionUpdateNumber.minor);*/
            ReaderOEMNumber = String.Format("{0}.{1}{2}",
                (char)Oversion.major,
                (char)Oversion.minor,
                (char)Oversion.release);
            ControlItem(ENUM_ITEM_TYPE.TEXT_OEMVER, String.Format("[OEM]{0}.{1}{2}up{3}{4}", 
                (char)Oversion.major, (char)Oversion.minor, (char)Oversion.release, 
                (char)OversionUpdateNumber.major, (char)OversionUpdateNumber.minor));
            //Show device type
            ControlItem(ENUM_ITEM_TYPE.TEXT_DEVICE, rfid.clsPacket.TRANS_API_AskDevType().ToString());

            //GetModelName
            do
            {
                UInt32 uiModelNameMajor = 0;
                UInt32 uiModelNameSub = 0;
                string strModule = string.Empty;
                string strModuleSub = string.Empty;
                rfid.Constants.Result resultM = rfid.Constants.Result.OK;

                //Get Model Name
                resultM = m_clsInterface.API_MacReadOemData((ushort)((int)enumOEM_ADDR.MODEL_NAME_MAIN), ref uiModelNameMajor);
                resultM = m_clsInterface.API_MacReadOemData((ushort)((int)enumOEM_ADDR.MODEL_NAME_SUB), ref uiModelNameSub);
                /*
                textBoxModelName.Text = String.Format("RU-{0}{1}{2}-{3}{4}{5}{6}",
                                                   (char)((uiModelNameMajor >> 16) & 0xFF),
                                                   (char)((uiModelNameMajor >> 8) & 0xFF),
                                                   (char)(uiModelNameMajor & 0xFF),
                                                   (char)((uiModelNameSub >> 16) & 0xFF),
                                                   (char)((uiModelNameSub >> 8) & 0xFF),
                                                   (char)(uiModelNameSub & 0xFF),
                                                   (char)((uiModelNameSub >> 24) & 0xFF));*/
                
                 if (uiModelNameMajor == 0x4D303258)//0x4D303258==M02X
                {
                    textBoxModelName.Text = String.Format("RU00-{0}{1}{2}-{3}{4}{5}{6}",
                                                   (char)((uiModelNameMajor >> 24) & 0xFF),
                                                   (char)((uiModelNameMajor >> 16) & 0xFF),
                                                   (char)((uiModelNameMajor >> 8) & 0xFF),
                                                   (char)(uiModelNameMajor & 0xFF),
                                                   (char)((uiModelNameSub >> 16) & 0xFF),
                                                   (char)((uiModelNameSub >> 8) & 0xFF),
                                                   (char)(uiModelNameSub & 0xFF));
                }
                else
                {
                    textBoxModelName.Text = String.Format("RU-{0}{1}{2}-{3}{4}{5}{6}",
                                                       (char)((uiModelNameMajor >> 16) & 0xFF),
                                                       (char)((uiModelNameMajor >> 8) & 0xFF),
                                                       (char)(uiModelNameMajor & 0xFF),
                                                       (char)((uiModelNameSub >> 16) & 0xFF),
                                                       (char)((uiModelNameSub >> 8) & 0xFF),
                                                       (char)(uiModelNameSub & 0xFF),
                                                       (char)((uiModelNameSub >> 24) & 0xFF));
                } 
               
                 

            } while (false);
           

        }



        //private Result GetVersion( DEV_MODE r_mode )
        private Result GetVersion()
        {
            MacBootLoaderVersion Version = new MacBootLoaderVersion();
            Result               result  = Result.FAILURE;
            
            m_Mode  = DEV_MODE.NO;
            FirmwareVersion ver = new FirmwareVersion();
            result = m_clsInterface.API_MacGetFirmwareVersion(ref ver);
            ControlItem(ENUM_ITEM_TYPE.TEXT_VER,    String.Format("[FW] {0}.{1}.{2}",ver.major,ver.minor,ver.release) );
            //ControlItem(ENUM_ITEM_TYPE.TEXT_STATUS, String.Format("[FW] {0}.{1}.{2}",ver.major,ver.minor,ver.release) );
 
            //Show device type
            ControlItem( ENUM_ITEM_TYPE.TEXT_DEVICE, rfid.clsPacket.TRANS_API_AskDevType().ToString() );

            return Result.OK;
             
        }
                
        private bool OpenDevice( )
        {
            if (true == m_clsInterface.API_OpenDevice())
            {
                /*
                if ( Result.OK != GetVersion(AskMode()) )
                {
                    m_clsInterface.API_Shutdown();
                    return false;
                }*/
                ControlItem(ENUM_ITEM_TYPE.TEXT_STATUS, "Connected");
                GetVersion();
                GetOEMVersion();      
                return true;
            }
            else            
            { 
                ControlItem(ENUM_ITEM_TYPE.TEXT_STATUS, "Disconnected");
                return false;
            }
        }


        private void Connect( )
        {

#if _TRACE_OUT_PUT
           m_clsInterface.PrintMagToTxt("======UpdateFW Connect======");
#endif 

            //clear
            ControlItem(ENUM_ITEM_TYPE.TEXT_VER,    "");
            ControlItem(ENUM_ITEM_TYPE.TEXT_STATUS, "");
            ControlItem(ENUM_ITEM_TYPE.TEXT_DEVICE, "No device");
            m_Mode = DEV_MODE.NO;

            //close the device
            m_clsInterface.API_RadioClose();

            //Disable Item
            //btnUpdate.Enabled = false;
            ControlItem(ENUM_ITEM_TYPE.MENU_SET_COM_PORT,     false);
            ControlItem(ENUM_ITEM_TYPE.MENU_DEVICE_INTERFACE, false);

            //open the device            
            if (true == OpenDevice())
            { 
                //Enable Item
                btnUpdate.Enabled = true;    
                
                //If update progress is running, hide "device Interface" item.
                if(m_clsBackThread.IsBusy == false)
                    ControlItem(ENUM_ITEM_TYPE.MENU_DEVICE_INTERFACE, true);           
            }
            
            ////If update progress is running, hide "Set Com Port" item.
            //if (m_clsBackThread.IsBusy == false)            
            //{
            //    //always enable "set com port" item to let user set uless update progress is running.
            //    SetComPortToolStripMenuItem.Enabled = true;
            //}
           ControlItem(ENUM_ITEM_TYPE.MENU_SET_COM_PORT,  true);
        }

  
        private void Disconnect( )
        {

#if _TRACE_OUT_PUT
            m_clsInterface.PrintMagToTxt("====== UpdateFW Disconnect======");
#endif 

            //clear
            ControlItem(ENUM_ITEM_TYPE.TEXT_VER,    "Disconnected");
            ControlItem(ENUM_ITEM_TYPE.TEXT_STATUS, "Disconnected");
            ControlItem(ENUM_ITEM_TYPE.TEXT_DEVICE, "No device");
            m_Mode = DEV_MODE.NO;

            //close the device
            m_clsInterface.API_RadioClose();

            //Disable Item            
            ControlItem(ENUM_ITEM_TYPE.MENU_DEVICE_INTERFACE, false);
        }
 
           
//==========================Event=========================================    
        
        private void UpdateFW_Load(object sender, EventArgs e)
        {
            //Disable Item
            ControlItem(ENUM_ITEM_TYPE.BTN_UPDATE,            false);      
            ControlItem(ENUM_ITEM_TYPE.MENU_DEVICE_INTERFACE, false);

            //open the device            
            if (true == OpenDevice())
            {
                //Enable Item
                ControlItem(ENUM_ITEM_TYPE.BTN_UPDATE, true);
                ControlItem(ENUM_ITEM_TYPE.MENU_DEVICE_INTERFACE, true);
                GetOEMVersion();
                GetVersion();
            }
        }



        private void UpdateFW_FormClosing(object sender, FormClosingEventArgs e)
        {
            //if is running update progress, give up it.
            if ( m_clsBackThread.IsBusy == true )
            {
                m_clsBackThread.CancelAsync();
                return;
            }

            if ( null != m_fileStream )
            {
                m_fileStream.Close();
                m_fileStream = null;
            }


            if ( true == m_clsRegHidEvent.IsReg() )
            {
                m_clsRegHidEvent.UnRegisterHidEvent();
            }


            m_clsInterface.API_RadioClose();
        }




        private void btnUpdate_Click(object sender, EventArgs e)
        {
            FileStream fileStream = null;
            uint       FileCount  = 0;


            //Check fireware file
            if( m_clsOpenFileDlg.FileName == null )
            {
                ControlItem(ENUM_ITEM_TYPE.TEXT_STATUS, "Please select a file.");   
                return;
            }

            if ( false == File.Exists(m_clsOpenFileDlg.FileName) )
            { 
                ControlItem(ENUM_ITEM_TYPE.TEXT_STATUS, "The file doesn't exist.");   
                return;            
            }


            //if is running update progress, cancel it.
            if (m_clsBackThread.IsBusy == true)
            {
                m_clsBackThread.CancelAsync();
                return;
            }

            //Check file size and OEMFile VersionNumber
            string[] aVersion;
            if(fileStream == null)
                fileStream  = new FileStream( m_clsOpenFileDlg.FileName, FileMode.Open, FileAccess.Read, FileShare.None, 512, FileOptions.None);
            StreamReader OEMReader = new StreamReader(fileStream);
            for (int cnt = 0; cnt < 4; cnt++)
            {
                sLine = OEMReader.ReadLine();
                if (sLine.Contains("OEMCfg Manufacture Version Number"))
                {
                    aVersion = sLine.Split(':');
                    sLine = aVersion[1].Trim();
                    fileStream.Position = 0;
                    break;
                }
            }

            if (fileStream.Length <= 0)
            {
                fileStream.Close();
                return;
            }

            //initial setting
            m_fileStream  = fileStream;
            m_lFileSize   = m_fileStream.Length;
            m_uiFileCount = (FileCount == 0) ? 1 : FileCount;
            m_strFilePath = m_clsOpenFileDlg.FileName;
            m_bTestMode   = testModeToolStripMenuItem.Checked;

            

            //Disabel Item
            ControlItem(ENUM_ITEM_TYPE.MENU_SET_COM_PORT,     false);
            ControlItem(ENUM_ITEM_TYPE.MENU_DEVICE_INTERFACE, false);
            ControlItem(ENUM_ITEM_TYPE.BTN_RELINK,            false);
            ControlItem(ENUM_ITEM_TYPE.BTN_UPDATE_TEXT,       "Stop");
            
            //Create thread
            m_clsBackThread = new BackgroundWorker( );
            m_clsBackThread.WorkerReportsProgress = true;
            m_clsBackThread.WorkerSupportsCancellation = true;
            m_clsBackThread.DoWork += new DoWorkEventHandler( MonitorUpdate );
            m_clsBackThread.ProgressChanged += new ProgressChangedEventHandler( BackgroundProgress );
            m_clsBackThread.RunWorkerCompleted += new RunWorkerCompletedEventHandler( BackgroundTaskCompleted );
            m_clsBackThread.RunWorkerAsync( !m_bTestMode );        
        }



        private void btnBrowser_Click(object sender, EventArgs e)
        {
            //btnExport                        
            //m_clsOpenFileDlg.Filter   = "BIN (*.bin)|*.bin|ALL Files (*.*)|*.*";
            m_clsOpenFileDlg.Filter   = "DAT (*.dat)|*.dat|ALL Files (*.*)|*.*";
            //m_clsOpenFileDlg.Filter = "ALL Files (*.*)|*.*";
            m_clsOpenFileDlg.Title    = "Open a File";

            try
            {
                if (m_clsOpenFileDlg.ShowDialog() != DialogResult.OK)
                    return;


                ControlItem(ENUM_ITEM_TYPE.TEXT_FILE_PATH, m_clsOpenFileDlg.FileName);
            }
            catch (Exception ex)
            { 
                MessageBox.Show( "Please select a valid path.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }    
        }




        private void btnRelink_Click(object sender, EventArgs e)
        {
            Connect();    
        }



        private void SetComPortToolStripMenuItem_Click(object sender, EventArgs e)
        {
                   
            //Check status
            if (m_clsBackThread.IsBusy == true)
            {
                ControlItem(ENUM_ITEM_TYPE.MENU_SET_COM_PORT, false);
                return;
            }


            //Open dialog
            using ( SetComport frm = new SetComport(m_clsInterface) )
            {
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    return;
                }
            }

        }


        private void deviceInterfaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (m_clsBackThread.IsBusy == true)
            { 
                 MessageBox.Show("Error: It's busy. Please finish or cancel update progress to return to AP Mode.",
                                 "Configuration - Busy",
                                  MessageBoxButtons.OK,
                                  MessageBoxIcon.Warning              );
                return;            
            }
            
           
            //Open dialog
            using ( DeviceInterface frm = new DeviceInterface( m_clsInterface, dlgControlItem ) )
            {
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    return;
                }
            }
        }



        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Open dialog
            using ( AboutBox b = new AboutBox( ) )
            {
                b.ShowDialog( );
            }
        }


        private bool HidArribval( int r_iEvent, char[] r_cDevName)
        {
            string strDllDevName   = rfid.clsPacket.TRANS_API_AskDevPath();               
            string strEventDevName = new string(r_cDevName);

            strEventDevName = strEventDevName.ToUpper();
            int iVid = Convert.ToInt32( strEventDevName.Substring( strEventDevName.IndexOf("VID_") + 4, 4 ), 16 );
            int iPid = Convert.ToInt32( strEventDevName.Substring( strEventDevName.IndexOf("PID_") + 4, 4 ), 16 );

            //Check whether the vendor is MTI or not
            if(iVid != (int)ENUM_DEVICE_INFO.MTI_VID)
                return false;


            do
            {
                //No device
                if ( 0 == strDllDevName.Length )
                {
                    //Check the Pid from the device whether match MTI Pid or not
                    foreach ( ENUM_DEVICE_INFO ennumDevPid in Enum.GetValues(typeof(ENUM_DEVICE_INFO)) )
                    { 
                        //Check PID
                        if( iPid != (int)ennumDevPid )
                            continue;

                       
                        if (r_iEvent == m_clsRegHidEvent.DBT_DEVICE_ARRIVAL)     //Insert HID
                        {
                            //If is in the update progress, "Insert" event may broke the progress.      
                            //For example, the applicarion is sending update data. At this moment, 
                            //received "Insert" Event. The application will relink and stop the 
                            //update progress.
                            if( m_clsBackThread.IsBusy == true )
                                return false;

                            Connect();
                            return true;
                        }
                        else if (r_iEvent == m_clsRegHidEvent.DBT_DEVICE_REMOVE_COMPLETE) //Remove HID
                        {
                            Disconnect();
                            return true;
                        }                        
                    }
                    
                    break;
                }     
                    

                //already have Device                       
                if ( string.Compare( strEventDevName, 0, 
                                     strDllDevName,   0,
                                     strDllDevName.Length,
                                     true                                    ) == 0 )
                {
                    if (r_iEvent == m_clsRegHidEvent.DBT_DEVICE_ARRIVAL)
                    {

                        //If is in the update progress, "Insert" event may broke the progress.      
                        //For example, the applicarion is sending update data. At this moment, 
                        //received "Insert" Event. The application will relink and stop the 
                        //update progress.
                        if( m_clsBackThread.IsBusy == true )
                            return false;

                        Connect();
                        return true;
                    }
                    else if (r_iEvent == m_clsRegHidEvent.DBT_DEVICE_REMOVE_COMPLETE)
                    {
                        Disconnect();
                        return true;
                    }
                }

            }while(false);

                    

            return false;
        }


        protected override void WndProc(ref Message m)
        {
            try
            {
                if (m.Msg == m_clsRegHidEvent.WM_DEVICECHANGE)
                { 
                    if (m.WParam.ToInt32() == m_clsRegHidEvent.DBT_DEVICE_ARRIVAL ||
                        m.WParam.ToInt32() == m_clsRegHidEvent.DBT_DEVICE_REMOVE_COMPLETE )
                    { 
                        DEV_BROADCAST_DEVICE_INTERFACE_2 devBroadcastDeviceInterface = new DEV_BROADCAST_DEVICE_INTERFACE_2();
                        DEV_BROADCAST_HDR devBroadcastHeader = new DEV_BROADCAST_HDR();        
                        devBroadcastDeviceInterface.dbcc_classguid = new Byte[1];
                        devBroadcastDeviceInterface.dbcc_name = new Char[1];
                        Marshal.PtrToStructure( m.LParam, devBroadcastHeader );
                        if (devBroadcastHeader.dbcc_deviceType == m_clsRegHidEvent.DBT_DEVTYP_DEVICE_INTERFACE)
                        { 
                            Int32 stringSize = Convert.ToInt32( (devBroadcastHeader.dbcc_size-32)/2 );
                            Array.Resize(ref devBroadcastDeviceInterface.dbcc_name, stringSize);
                            Marshal.PtrToStructure(m.LParam, devBroadcastDeviceInterface);
                            HidArribval( m.WParam.ToInt32(), devBroadcastDeviceInterface.dbcc_name);
                        }
                    }
                }

                base.WndProc(ref m);
            }//try
            catch (Exception ex)
            {
                base.WndProc(ref m);
                MessageBox.Show(ex.Message);
            }                            
        }
    
    

//===========================Back Ground==========================================
        private void MonitorUpdate(object sender, DoWorkEventArgs e)
        {
            MacBootLoaderVersion Version        = new MacBootLoaderVersion();
            string               strFilePath    = textFilePath.Text;
            
            Result               result         = Result.FAILURE;
            Updatedata           strcData;
            int       iLast = 0;
            int       i;


#if _TRACE_OUT_PUT
                m_clsInterface.PrintMagToTxt("--MonitorUpdate---");
#endif 
            
            //ControlItem(ENUM_ITEM_TYPE.TEXT_VER, "");            

            //Clear buffer
            rfid.clsPacket.TRANS_API_ClearBuffer();

            //Compare OEMFile VersionNumber with Reader OEMVersionNumber
            if (sLine != ReaderOEMNumber)
            {
                ControlItem(ENUM_ITEM_TYPE.TEXT_STATUS, string.Format(" Write OEMCfg file failure."));
                //m_clsBackThread.ReportProgress(0);
                ControlItem(ENUM_ITEM_TYPE.PROG_SET_MAX, (int)m_lFileSize);
                ControlItem(ENUM_ITEM_TYPE.PROG_CLEAR, null);
                ControlItem(ENUM_ITEM_TYPE.LABEL_PERCENT_TEXT, string.Format("{0}%", 0));

                MessageBox.Show(string.Format("The browser file is invalid for this product.", result.ToString()),
                                    " ",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                return;
            }
            else//Write OEM File to the device
            {
                //ControlItem(ENUM_ITEM_TYPE.PROG_SET_MAX, (int)m_lFileSize);
                ControlItem(ENUM_ITEM_TYPE.PROG_SET_MAX, 100);
                ControlItem(ENUM_ITEM_TYPE.PROG_CLEAR, null);
                ControlItem(ENUM_ITEM_TYPE.LABEL_PERCENT_TEXT, string.Format("{0}%", 0));
                rfid.Constants.Result status;
                OEMPercentageFlag = false;
                StreamReader OEMReader = new StreamReader(m_fileStream);
                iRealReadCountOEM=(int)m_lFileSize;
                while ((addLine = OEMReader.ReadLine()) != null)
                {
                    iRealReadCountOEM = iRealReadCountOEM - (addLine.Length+2);
                    if(addLine.Contains("0x"))
                    {
                        if(OEMPercentageFlag==false)
                        {
                            iRealReadCountOEMBtnHalf = iRealReadCountOEM+23;//BtnHalf=Total-TopHalf
                            iRealReadCountOEMBtnHalf = iRealReadCountOEMBtnHalf / 23;
                            OEMPercentage = Convert.ToInt16(Math.Ceiling((100.0 / iRealReadCountOEMBtnHalf)));
                            OEMPercentageFlag = true;
                        }

                        iLast = iLast + (int)OEMPercentage;
                        if (iLast > 100)
                        {
                            i = 100 - (iLast - (int)OEMPercentage);
                            if(i > 0)
                                OEMPercentage = i;
                        }

                        OEMLine = addLine.Split(',');
                        addLine = OEMLine[0].Trim();
                        dataLine = OEMLine[1].Trim();
                        string[] SusaddLine = addLine.Split('x');
                        addLine = SusaddLine[1].Trim();
                        OEMAddress = UInt16.Parse(addLine, System.Globalization.NumberStyles.HexNumber);                        
                        string[] SusdataLine = dataLine.Split('x');
                        dataLine = SusdataLine[1].Trim();
                        OEMData = UInt32.Parse(dataLine, System.Globalization.NumberStyles.HexNumber);
                        switch (m_clsInterface.API_MacWriteOemData(OEMAddress, OEMData))
                        { 
                            case rfid.Constants.Result.OK:
                                switch (m_clsInterface.API_MacReadOemData(OEMAddress, ref checkOEMData))
                                {
                                    case rfid.Constants.Result.OK:
                                        if (OEMData != checkOEMData)//Compare Error
                                        {
                                            MessageBox.Show("Write OEMData Failure.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                            ControlItem(ENUM_ITEM_TYPE.PROG_CLEAR, null);
                                            ControlItem(ENUM_ITEM_TYPE.LABEL_PERCENT_TEXT, string.Format("{0}%", 0));
                                            return;
                                        }
                                        m_clsBackThread.ReportProgress((int)OEMPercentage);
                                        UpdateOEMCfgCompleteFlag = true;
                                        break;
                                    case rfid.Constants.Result.INVALID_PARAMETER:
                                        UpdateOEMCfgCompleteFlag = false;
                                        MessageBox.Show("Invalid Parameter.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                        ControlItem(ENUM_ITEM_TYPE.PROG_CLEAR, null);
                                        ControlItem(ENUM_ITEM_TYPE.LABEL_PERCENT_TEXT, string.Format("{0}%", 0));
                                        return;
                                    case rfid.Constants.Result.FAILURE:
                                        //Get error
                                        result = m_clsInterface.API_MacGetError(ref uiCurError, ref uiLastError);
                                        if (result == rfid.Constants.Result.OK)
                                        {
                                            MessageBox.Show(string.Format("Error Code: 0x{0:X3}", uiCurError), " ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                            UpdateOEMCfgCompleteFlag = false;
                                            ControlItem(ENUM_ITEM_TYPE.PROG_CLEAR, null);
                                            ControlItem(ENUM_ITEM_TYPE.LABEL_PERCENT_TEXT, string.Format("{0}%", 0));
                                            return;
                                        }
                                        UpdateOEMCfgCompleteFlag = false;
                                        ControlItem(ENUM_ITEM_TYPE.PROG_CLEAR, null);
                                        ControlItem(ENUM_ITEM_TYPE.LABEL_PERCENT_TEXT, string.Format("{0}%", 0));
                                        break;
                                }  
                                break;

                            case rfid.Constants.Result.INVALID_PARAMETER:
                                MessageBox.Show(string.Format("Write OEM Address(h): 0x{0} Fail [INVALID_PARAMETER]", addLine), " ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                UpdateOEMCfgCompleteFlag = false;
                                ControlItem(ENUM_ITEM_TYPE.PROG_CLEAR, null);
                                ControlItem(ENUM_ITEM_TYPE.LABEL_PERCENT_TEXT, string.Format("{0}%", 0));
                                 return;

                            case rfid.Constants.Result.FAILURE:
                                //Get error
                                result = m_clsInterface.API_MacGetError(ref uiCurError, ref uiLastError);
                                if (result == rfid.Constants.Result.OK)
                                {
                                    MessageBox.Show(string.Format("Error Code: 0x{0:X3}", uiCurError), " ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    UpdateOEMCfgCompleteFlag = false;
                                    ControlItem(ENUM_ITEM_TYPE.PROG_CLEAR, null);
                                    ControlItem(ENUM_ITEM_TYPE.LABEL_PERCENT_TEXT, string.Format("{0}%", 0));
                                    return;
                                }
                                UpdateOEMCfgCompleteFlag = false;
                                ControlItem(ENUM_ITEM_TYPE.PROG_CLEAR, null);
                                ControlItem(ENUM_ITEM_TYPE.LABEL_PERCENT_TEXT, string.Format("{0}%", 0));
                                break;
                        }
                    }
                }

                //m_lFileSize = m_lFileSize - 40;
                //m_clsBackThread.ReportProgress(40);
                //for (int filesize = 0; filesize<(int)m_lFileSize; filesize = filesize + 40)
                //{
                    //Increase progress bar
                    //m_clsBackThread.ReportProgress((int)m_lFileSize);
                    //m_clsBackThread.ReportProgress(filesize);
                //}
                //System.Threading.Thread.Sleep(10);
            }
            //m_clsBackThread.ReportProgress(100);
            ControlItem(ENUM_ITEM_TYPE.TEXT_STATUS, "Update progress finished.");
            MessageBox.Show("Update OEMCfg successfully.");            
            //initial Progress Bar
            ControlItem(ENUM_ITEM_TYPE.PROG_SET_MAX, 100);
            ControlItem(ENUM_ITEM_TYPE.PROG_CLEAR, null);
            ControlItem(ENUM_ITEM_TYPE.LABEL_PERCENT_TEXT, string.Format("{0}%", 0));
            m_clsInterface.API_ControlSoftReset();
//            OpenDevice(); 
//            Connect();
//            GetOEMVersion();
        }



        private void BackgroundProgress( object sender, ProgressChangedEventArgs e )        
        {
            //improve progress bar step
            ControlItem(ENUM_ITEM_TYPE.PROG_ADD_VALUE, e.ProgressPercentage);  

            //show progress step
            ControlItem(ENUM_ITEM_TYPE.LABEL_PERCENT_TEXT, string.Format("{0}%", (uint)(progressBar.Value*100/progressBar.Maximum)) );  
        }


        private void BackgroundTaskCompleted(object sender, RunWorkerCompletedEventArgs e)
        {         
         
#if _TRACE_OUT_PUT
                m_clsInterface.PrintMagToTxt("--BackgroundTaskCompleted---");
#endif      

            //close file
            m_fileStream.Close();
            //Enable "deviceInterface" item while only connect device.
            ControlItem(ENUM_ITEM_TYPE.MENU_DEVICE_INTERFACE, true);
            //Enable Item
            ControlItem(ENUM_ITEM_TYPE.MENU_SET_COM_PORT, true);
            ControlItem(ENUM_ITEM_TYPE.BTN_RELINK,        true);
            ControlItem(ENUM_ITEM_TYPE.BTN_UPDATE_TEXT,   "Update");
            if (UpdateOEMCfgCompleteFlag == true)
            {
                Connect();
                UpdateOEMCfgCompleteFlag = false;
            }
        }



//======================Delegate========================================         

        private void ControlItem( ENUM_ITEM_TYPE r_enumItem, object r_obj)
        {

            if ( InvokeRequired )
            {
                Invoke( dlgControlItem, new object[]{r_enumItem, r_obj} );
                return;
            }

            try
            {

                switch (r_enumItem)
                {
                    //======Text=============
                    case ENUM_ITEM_TYPE.TEXT_VER:
                        textBootLoader.Text = (string)r_obj;
                        break;

                    case ENUM_ITEM_TYPE.TEXT_FILE_PATH:
                        textFilePath.Text   = (string)r_obj;
                        break;

                    case ENUM_ITEM_TYPE.TEXT_STATUS:
                        statusLabel.Text    = (string)r_obj;

#if _TRACE_OUT_PUT
                m_clsInterface.PrintMagToTxt(statusLabel.Text);
#endif 
                        break;

                    case ENUM_ITEM_TYPE.TEXT_DEVICE:
                        textBoxDevice.Text  = (string)r_obj;
                        break;

                    case ENUM_ITEM_TYPE.TEXT_OEMVER:
                        textBoxOEMVersion.Text = (string)r_obj;
                        break;

                    //======Label=============
                    case ENUM_ITEM_TYPE.LABEL_PERCENT_TEXT:
                        labelPercent.Text  = (string)r_obj;
                        break;



                    //=====Button================
                    case ENUM_ITEM_TYPE.BTN_RELINK:
                        btnRelink.Enabled = (bool)r_obj;
                        break;

                    case ENUM_ITEM_TYPE.BTN_BROWSER:
                        btnBrowser.Enabled = (bool)r_obj;
                        break;

                    case ENUM_ITEM_TYPE.BTN_UPDATE:
                        btnUpdate.Enabled  = (bool)r_obj;
                        break;

                    case ENUM_ITEM_TYPE.BTN_UPDATE_TEXT:
                        btnUpdate.Text      = (string)r_obj;
                        break;


                    //===========Progress==========
                    case ENUM_ITEM_TYPE.PROG_SET_MAX:
                        progressBar.Maximum = (int)r_obj;
                        progressBar.Minimum = 0;
                        break;

                    case ENUM_ITEM_TYPE.PROG_ADD_VALUE:
                        progressBar.Value += (int)r_obj;
                        break;

                    case ENUM_ITEM_TYPE.PROG_CLEAR:
                        progressBar.Value = 0;
                        break;


                    //=====MenuItem===============
                    case ENUM_ITEM_TYPE.MENU_SET_COM_PORT:
                        SetComPortToolStripMenuItem.Enabled = (bool)r_obj;
                        break;

                    case ENUM_ITEM_TYPE.MENU_DEVICE_INTERFACE:
                        deviceInterfaceToolStripMenuItem.Enabled = (bool)r_obj;
                        break;
                    
                    
                    
                    //=====Function===============
                    case ENUM_ITEM_TYPE.FUNC_CONNECT:
                        Connect();
                        break;

                    case ENUM_ITEM_TYPE.FUNC_DISCONNECT:
                        Disconnect();
                        break;
                }

            }
            catch(Exception e)
            {
                ;
            }
        }
    
    }
}
