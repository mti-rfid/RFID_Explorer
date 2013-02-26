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
using UpdateFWTool.Global;
using System.Runtime.InteropServices;

using Global;

namespace UpdateFWTool
{

    public partial class UpdateFW : Form
    {

        Interface           m_clsInterface   = new Interface();
        BackgroundWorker    m_clsBackThread  = new BackgroundWorker();
        OpenFileDialog      m_clsOpenFileDlg = new OpenFileDialog(); 
        ClsRegisterHidEvent m_clsRegHidEvent = new ClsRegisterHidEvent();   
        DEV_MODE            m_Mode           = DEV_MODE.NO;


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


        public UpdateFW()
        {
            InitializeComponent();  
          
            //Delegate
            dlgControlItem    = new CONTROL_ITEM(ControlItem);

            //Register HID Event for receiving DBT_DEVICE_ARRIVAL and DBT_DEVICE_REMOVE_COMPLETE event puring PNP          
            m_clsRegHidEvent.RegisterHidEvent(this.Handle);
        }



//==========================Other=========================================
        private DEV_MODE AskMode()
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
        }




        private Result GetVersion( DEV_MODE r_mode )
        {
            MacBootLoaderVersion Version = new MacBootLoaderVersion();
            Result               result  = Result.FAILURE;
            
            m_Mode  = DEV_MODE.NO;

            switch (r_mode)
            { 
                case DEV_MODE.AP:

#if _TRACE_OUT_PUT
                m_clsInterface.PrintMagToTxt("====== Get FW AP Version======");
#endif 

                    FirmwareVersion ver = new FirmwareVersion();
                    result = m_clsInterface.API_MacGetFirmwareVersion(ref ver);
 
                    if (Result.OK != result)
                    {
                        //ControlItem(ENUM_ITEM_TYPE.STATUS, "Can't get AP version");
                        return result;
                    }
                    ControlItem(ENUM_ITEM_TYPE.TEXT_VER,    String.Format("[AP] {0}.{1}.{2}",ver.major,ver.minor,ver.release) );
                    ControlItem(ENUM_ITEM_TYPE.TEXT_STATUS, String.Format("[AP] {0}.{1}.{2}",ver.major,ver.minor,ver.release) );
                    
                    m_Mode = DEV_MODE.AP;
                    break;

                case DEV_MODE.BL:  

#if _TRACE_OUT_PUT
                m_clsInterface.PrintMagToTxt("====== Get BL AP Version======");
#endif 


                    result = m_clsInterface.API_UpdataGetVersion(ref Version);                
                    if (Result.OK != result)
                    {
                        //ControlItem(ENUM_ITEM_TYPE.STATUS, "Can't get bootloader version");
                        return result;
                    }
                    ControlItem(ENUM_ITEM_TYPE.TEXT_VER,    String.Format("[BL] {0}.{1}.{2}",Version.major,Version.minor,Version.release) );
                    ControlItem(ENUM_ITEM_TYPE.TEXT_STATUS, String.Format("[BL] {0}.{1}.{2}",Version.major,Version.minor,Version.release) );
                    
                    m_Mode = DEV_MODE.BL;
                    break;

                case DEV_MODE.BL_ENTER_UPDATE:
                    
#if _TRACE_OUT_PUT
                m_clsInterface.PrintMagToTxt("====== Get BL_ENTER_UPDATE Version======");
#endif 

                    //result = m_clsInterface.API_UpdataGetVersion(ref Version);
                    //if (result != Result.OK && result != Result.FWUPD_CMD_IGN)
                    //{ 
                    //    //ControlItem(ENUM_ITEM_TYPE.STATUS, "Can't get bootloader version");
                    //    return result;                    
                    //}

                    ControlItem(ENUM_ITEM_TYPE.TEXT_VER,    "[BL] Update Mode." );
                    ControlItem(ENUM_ITEM_TYPE.TEXT_STATUS, "Enter update Mode." );

                    m_Mode = DEV_MODE.BL_ENTER_UPDATE;
                    break;

                default:
                    m_Mode = DEV_MODE.NO;
                    return Result.FAILURE;
                    break;
            }

            //Show device type
            ControlItem( ENUM_ITEM_TYPE.TEXT_DEVICE, rfid.clsPacket.TRANS_API_AskDevType().ToString() );

            return Result.OK;
        }



        private bool OpenDevice( )
        {
            if (true == m_clsInterface.API_OpenDevice())
            {

                if ( Result.OK != GetVersion(AskMode()) )
                {
                    m_clsInterface.API_Shutdown();
                    return false;
                }

                ControlItem(ENUM_ITEM_TYPE.TEXT_STATUS, "Connected");
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
    
            
            //ControlItem(ENUM_ITEM_TYPE.MENU_SET_COM_PORT, true);

            ////If update progress is running, doesn't hide "Updtae" button which shows "Stop".
            //if (m_clsBackThread.IsBusy == false)
            //{                
            //    btnUpdate.Enabled = false;
            //}

        }
 
           
//==========================Event=========================================    
        
        private void UpdateFW_Load(object sender, EventArgs e)
        {
            //Disable Item
            ControlItem(ENUM_ITEM_TYPE.BTN_UPDATE,            false);      
            ControlItem(ENUM_ITEM_TYPE.MENU_DEVICE_INTERFACE, false);

            //open the device            
            if (true == OpenDevice() )
            { 
                //Enable Item
                ControlItem(ENUM_ITEM_TYPE.BTN_UPDATE,            true);
                ControlItem(ENUM_ITEM_TYPE.MENU_DEVICE_INTERFACE, true);
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


            //Check status
            if ( DEV_MODE.NO == AskMode() )
            {
                ControlItem(ENUM_ITEM_TYPE.TEXT_STATUS, "Disconnected. Can't update.");   
                return;
            }


            //Check file size
            if(fileStream == null)
                fileStream  = new FileStream( m_clsOpenFileDlg.FileName, FileMode.Open, FileAccess.Read, FileShare.None, 512, FileOptions.None);           
            if (fileStream.Length <= 0)                       
            { 
                fileStream.Close();
                return;
            }


            //Get version
            if (Result.OK != GetVersion(AskMode()))
            {
                fileStream.Close();
                return;
            }


            //if is in ap mode , change to BL mode
            if (m_Mode == DEV_MODE.AP)
            { 
                 if(  Result.OK != m_clsInterface.API_ControlResetToBootLoader() )
                 {
                    ControlItem(ENUM_ITEM_TYPE.TEXT_STATUS, "Turn to BL Mode unsuccessfully");
                    fileStream.Close();
                    return;            
                 }
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
            m_clsOpenFileDlg.Filter   = "BIN (*.bin)|*.bin|ALL Files (*.*)|*.*";
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

            if ( DEV_MODE.AP != AskMode() )
            { 

                 MessageBox.Show("Error: It's not in AP Mode. Please finish update progress to return to AP Mode.",
                                 "Configuration - Error",
                                  MessageBoxButtons.OK,
                                  MessageBoxIcon.Error              );
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
            int                  iRealReadCount = 0;
            Result               result         = Result.FAILURE;
            Updatedata           strcData;


#if _TRACE_OUT_PUT
                m_clsInterface.PrintMagToTxt("--MonitorUpdate---");
#endif 
            
            ControlItem(ENUM_ITEM_TYPE.TEXT_VER, "");            

            //Clear buffer
            rfid.clsPacket.TRANS_API_ClearBuffer();


            //Get Version
            while (true)
            {
                if (m_clsBackThread.CancellationPending == true)
                { 
                    e.Cancel = true;
                    return;
                }

                ControlItem(ENUM_ITEM_TYPE.MENU_SET_COM_PORT, false);

                //Check whether connected or not.
                if( Result.OK == GetVersion( AskMode() ) )
                    break;

                System.Threading.Thread.Sleep(10);                
            }


            //start to enter update mode
            switch ( m_Mode )
            { 
                case DEV_MODE.AP:
                    ControlItem(ENUM_ITEM_TYPE.TEXT_STATUS, "In the AP mode now, please press \"Update\" button again.");
                    
                    MessageBox.Show( "In the AP mode now, please press \"Update\" button again.", 
                                     "Update firmware unsuccessfully.",
                                     MessageBoxButtons.OK,
                                     MessageBoxIcon.Error                                           );   
                    return;
                    break;

               case DEV_MODE.BL:
                    ControlItem(ENUM_ITEM_TYPE.TEXT_STATUS, "Enter to update mode.");

                    result = m_clsInterface.API_UpdataEnterUpdateMode();
                    if ( Result.OK !=  result)
                    { 
                        ControlItem(ENUM_ITEM_TYPE.TEXT_STATUS, string.Format("Error:{0}. Can't enter update mode.", result.ToString()) );
                        m_clsInterface.API_UpdataComplete();

                        MessageBox.Show( string.Format("Error:{0}. Can't enter update mode.", result.ToString()), 
                                     "Update firmware unsuccessfully.",
                                     MessageBoxButtons.OK,
                                     MessageBoxIcon.Error                                                     );    
                       return;
                    }
                    ControlItem(ENUM_ITEM_TYPE.TEXT_STATUS, "In update mode, start to update.");
                    break;               
                
                case DEV_MODE.BL_ENTER_UPDATE:
                    ControlItem(ENUM_ITEM_TYPE.TEXT_STATUS, "In update mode, start to update.");
                    break;

                case DEV_MODE.NO:
                    ControlItem(ENUM_ITEM_TYPE.TEXT_STATUS, "ERROR: Can't connect the device.");
                    return;
                    break;            
            }


            //Write FW File to the device           
            strcData.bRealRunFlag = (bool)e.Argument;
            strcData.btData       = new Byte[DEFINE_UPDATE_DATA_SIZE];
            Array.Clear(strcData.btData, 0, strcData.btData.Length);

            //initial Progress Bar
            ControlItem(ENUM_ITEM_TYPE.PROG_SET_MAX, (int)m_lFileSize);
            ControlItem(ENUM_ITEM_TYPE.PROG_CLEAR, null);
            ControlItem(ENUM_ITEM_TYPE.LABEL_PERCENT_TEXT, string.Format("{0}%",0) );
                
            //Start to update
            while (m_lFileSize >0)
            {   
                if (m_clsBackThread.CancellationPending == true)
                { 
                    e.Cancel = true;
                    return;
                }

                strcData.uiOffset = (uint)m_fileStream.Position + DEFINE_UPDATE_DATA_ADDRESS;
                strcData.uiCRC    = 0;
                Array.Clear( strcData.btData, 0, strcData.btData.Length);

                //Read data from file
                iRealReadCount = m_fileStream.Read(strcData.btData, 0, DEFINE_UPDATE_DATA_SIZE);

                //check file whether is end or not.
                if( 0 >= iRealReadCount)
                    break;
                
                strcData.usSize = (ushort)( (iRealReadCount/4) + (iRealReadCount%4 >0 ? 1 : 0) + 3 );

                result = m_clsInterface.API_UpdataSendUpdateData( strcData);
                if (Result.OK != result )
                {
                    ControlItem(ENUM_ITEM_TYPE.TEXT_STATUS, string.Format("Error:{0}. Write firmware file unsuccessfully.", result.ToString()) );
                    
                    MessageBox.Show( string.Format("Error:{0}. Write firmware file unsuccessfully.", result.ToString()), 
                                     "Update firmware unsuccessfully.",
                                     MessageBoxButtons.OK,
                                     MessageBoxIcon.Error                                           );   
                    return;
                }     
      
                m_lFileSize -= (uint)iRealReadCount;

                //Increase progress bar
                m_clsBackThread.ReportProgress(iRealReadCount);

            }      


#if _TRACE_OUT_PUT
                m_clsInterface.PrintMagToTxt("--Write Data Complete---");
#endif 

            //exit update mode and turn to AP mode.
            result = m_clsInterface.API_UpdataComplete();

            do
            {
                if ( Result.OK == result )            
                    break;

                //In the test mode, update the file and send complete command, then no error occur.                
                //It will return Result.FWUPD_EXIT_NOWRITES. Means doesn't actually write update data to 
                //the device
                if ( m_bTestMode == true                     &&
                     Result.FWUPD_EXIT_NOWRITES == result         )
                { 
                    break;
                }


                //Show error message
                ControlItem(ENUM_ITEM_TYPE.TEXT_STATUS, string.Format("Error:{0}. Exit BL mode unsuccessfully. Please check file.", result.ToString()) );
                m_clsInterface.API_UpdataReset();   
   
                MessageBox.Show( string.Format("Error:{0}. Exit BL mode unsuccessfully. Please check file.", result.ToString()), 
                                 "Update firmware unsuccessfully.",
                                 MessageBoxButtons.OK,
                                 MessageBoxIcon.Error                                           );   
 
                return;

            }while(false);

           
            ControlItem(ENUM_ITEM_TYPE.TEXT_STATUS, "Update progress finished. Try to return to AP Mode.");          


           //Get Version
            while (true)
            {
                if (m_clsBackThread.CancellationPending == true)
                { 
                    e.Cancel = true;
                    return;
                }

                //Check whether connected or not.
                if( Result.OK == GetVersion( AskMode() ) )
                    break;

                System.Threading.Thread.Sleep(10);                
            }


            //If return to AP mode, means update progress successfully
            if( m_Mode == DEV_MODE.AP )
                MessageBox.Show("Update firmware successfully.");           
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

            switch ( AskMode() )
            { 
                case DEV_MODE.BL:
                case DEV_MODE.BL_ENTER_UPDATE:
                    if( m_clsInterface.API_UpdataReset() != Result.OK)
                         ControlItem(ENUM_ITEM_TYPE.TEXT_STATUS, "Error: can't return to AP mode.");  

                    ControlItem(ENUM_ITEM_TYPE.TEXT_STATUS, "Update firmware unsuccessfully.");  
                    break;

                case DEV_MODE.NO:
                    break;
            }


            //Get version
            if (Result.OK == GetVersion(m_Mode))
            {
                //Enable "deviceInterface" item while only connect device.
                ControlItem(ENUM_ITEM_TYPE.MENU_DEVICE_INTERFACE, true);
            }
            else
            {
                ControlItem(ENUM_ITEM_TYPE.TEXT_STATUS, "Error: can't get the version.");
            }


            //Enable Item
            ControlItem(ENUM_ITEM_TYPE.MENU_SET_COM_PORT, true);
            ControlItem(ENUM_ITEM_TYPE.BTN_RELINK,        true);
            ControlItem(ENUM_ITEM_TYPE.BTN_UPDATE_TEXT,   "Update");
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
