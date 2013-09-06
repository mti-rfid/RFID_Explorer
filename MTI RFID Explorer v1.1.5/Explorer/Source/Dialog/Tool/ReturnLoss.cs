using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using System.IO;
using System.Threading;
using System.Diagnostics;
using RFID.RFIDInterface;
using rfid.Constants;
using rfid.Structures;
using Global;



namespace RFID_Explorer
{
    
    public partial class ReturnLoss : Form
    {
        //Class Reference
        private static RFID_Explorer.mainForm m_mainForm  = null;
        private        LakeChabotReader       m_reader    = null;
        
        private List <Infor>   m_listInfor = new List<Infor>();
        private Infor          m_strcInfor;

        private volatile bool m_bAllChannel      = false;
        private volatile bool m_bRunFlag         = false;
        private byte          m_btChannelFlag    = 0;
        private byte          m_btPhysicalPort   = 0;
        private UInt16        m_usPowerLevel     = 0;
        private UInt32        m_uiExactFrequecny = 0;
        private UInt32        m_uiErrorCode      = 0;
        private MacRegion     m_macRegion        = rfid.Constants.MacRegion.UNKNOWN;
        private const int     PAINT_SHIFT        = 110;
        private const string  BMP_NAME           = "TmpPaint.bmp";
        
        
        
        //Graphic
        private int       m_OldX             = 0;
        private int       m_OldY             = 0;
        private Graphics  m_Graphics         = null;
        private Graphics  m_GraphicsBmp      = null;
        private Bitmap    m_PaintBmp         = null;
        private Image     m_PaintImage;

        //Thread
        private Thread                   m_thd      = null;
        private ParameterizedThreadStart m_thdParam = new ParameterizedThreadStart( DoThdWork );

        //Delegate
        private delegate void SET_COMBO_FREQ(ENUM_COMBO_FREQ_TYPE enumFreqType, int iCount);
        private SET_COMBO_FREQ dlgSetComboFreq = null;
        private delegate void SHOW_ERR_MSG(String strMsg);
        private SHOW_ERR_MSG dlgShowErrMsg = null;
        private delegate void SHOW_PAINT( double r_dbValue, Point[] point );
        private SHOW_PAINT dlgShowPaint = null;                                    
        private delegate void SET_CUR_VALUE_TO_TEXT( double r_dbValue );
        private SET_CUR_VALUE_TO_TEXT dlgSetCurValueToText = null;                                    
        private delegate void DRAW_STRING( String r_strVal, int x, int y, StringFormatFlags r_Type, Color r_color);
        private DRAW_STRING dlgDrawString = null; 

        enum ENUM_TEST_FUNCTION
        { 
            RF_ON,
            RF_OFF,
            INVENTORY,
            PULSE,
            ABORT,
        }

        enum ENUM_ANT_PORT
        {
            PORT_0,
            PORT_1,
            PORT_2,
            PORT_3,
            UNKNOWN
        }

        enum ENUM_RF_COMMON : uint
        { 
            CHANNEL_ALL = 0xFFFFFFFF,
        }

       
        enum ENUM_COMBO_FREQ_TYPE
        {
            RESET,
            ADD,
            SUB,
            LAST,
            CHOOSE,
            NO
        }

        private struct Infor
        { 
            public string  strFrequency;
            public double  dbReturnValue;
            public string  strText;
        }

        

        public ReturnLoss(RFID_Explorer.mainForm r_form, LakeChabotReader rm_reader)
        {
            InitializeComponent();

            string  strBmpPath = Path.Combine( Application.CommonAppDataPath, BMP_NAME);
            //string strBmpPath = Path.Combine( Environment.GetFolderPath( Environment.SpecialFolder.ApplicationData),
            //                                  Application.CommonAppDataPath );  //;  //Path.GetTempFileName\
           // string strBmpPath = Path.Combine( Application.CommonAppDataPath, BMP_NAME);

            m_mainForm = r_form;
            m_reader   = rm_reader;

            //Read Data from module
            m_reader.API_TestGetAntennaPortConfiguration(ref m_btPhysicalPort, ref m_usPowerLevel);
            m_reader.API_TestGetFrequencyConfiguration(ref m_btChannelFlag, ref m_uiExactFrequecny);

            //Thread function
            dlgSetComboFreq      = new SET_COMBO_FREQ( SetComboFreq );
            dlgShowErrMsg        = new SHOW_ERR_MSG( ShowErrMsg );
            dlgShowPaint         = new SHOW_PAINT( ShowPaint );
            dlgSetCurValueToText = new SET_CUR_VALUE_TO_TEXT( SetCurValueToText );
            dlgDrawString        = new DRAW_STRING(DrawString);

                        

            //Graphic
            PaintPanel.Controls.Add(picPaint);
            m_Graphics    = picPaint.CreateGraphics();                     //Link between Graphics and panel
            m_PaintBmp    = new Bitmap(picPaint.Width, picPaint.Height);   //Create BMP File
            m_PaintBmp.Save(strBmpPath);
            m_PaintBmp.Dispose();
            m_PaintImage  = Image.FromFile(strBmpPath);                      //Open BMP File
            m_GraphicsBmp = Graphics.FromImage(m_PaintImage);              //Link BMP File to Graphics Object
            //m_GraphicsBmp = Graphics.FromImage(m_PaintBmp);



            //regionComboBox
            try
            {
                m_macRegion = m_reader.RegulatoryRegion;

            }
            catch (rfidReaderException exp)
            {
                cmbBoxRegion.Text = exp.Message;
            }
            cmbBoxRegion.Items.Add(m_macRegion);
            cmbBoxRegion.SelectedIndex = 0;
            
            //Frequency
            switch(m_macRegion)
            {
                case MacRegion.US:
                    foreach ( ENUM_RF_US item in Enum.GetValues(typeof(ENUM_RF_US)) )
                    {
                        cmbBoxFreq.Items.Add( String.Format("{0:000.000}", (float)item/1000) );
                    }
                    cmbBoxFreq.Items.Add( ENUM_RF_COMMON.CHANNEL_ALL );
                    break;

                case MacRegion.EU:
                    foreach ( ENUM_RF_EU item in Enum.GetValues(typeof(ENUM_RF_EU)) )
                    {
                        cmbBoxFreq.Items.Add( String.Format("{0:000.000}", (float)item/1000) );
                    }
                    cmbBoxFreq.Items.Add( ENUM_RF_COMMON.CHANNEL_ALL );
                    break;

                case MacRegion.JP:
                    foreach ( ENUM_RF_JP item in Enum.GetValues(typeof(ENUM_RF_JP)) )
                    {
                        cmbBoxFreq.Items.Add( String.Format("{0:000.000}", (float)item/1000) );
                    }
                    cmbBoxFreq.Items.Add( ENUM_RF_COMMON.CHANNEL_ALL );
                    break;

                case MacRegion.EU2:
                    foreach ( ENUM_RF_EU2 item in Enum.GetValues(typeof(ENUM_RF_EU2)) )
                    {
                        cmbBoxFreq.Items.Add( String.Format("{0:000.000}", (float)item/1000) );
                    }
                    cmbBoxFreq.Items.Add( ENUM_RF_COMMON.CHANNEL_ALL );
                    break;

                case MacRegion.TW:
                    foreach ( ENUM_RF_TW item in Enum.GetValues(typeof(ENUM_RF_TW)) )
                    {
                        cmbBoxFreq.Items.Add( String.Format("{0:000.000}", (float)item/1000) );
                    }
                    cmbBoxFreq.Items.Add( ENUM_RF_COMMON.CHANNEL_ALL );
                    break;

                case MacRegion.CN:
                    foreach ( ENUM_RF_CN item in Enum.GetValues(typeof(ENUM_RF_CN)) )
                    {
                        cmbBoxFreq.Items.Add( String.Format("{0:000.000}", (float)item/1000) );
                    }
                    cmbBoxFreq.Items.Add( ENUM_RF_COMMON.CHANNEL_ALL ); 
                    break;

                case MacRegion.KR:
                    foreach ( ENUM_RF_KR item in Enum.GetValues(typeof(ENUM_RF_KR)) )
                    {
                       cmbBoxFreq.Items.Add( String.Format("{0:000.000}", (float)item/1000) );
                    }
                    cmbBoxFreq.Items.Add( ENUM_RF_COMMON.CHANNEL_ALL ); 
                    break;

                case MacRegion.AU:
                    foreach ( ENUM_RF_AU item in Enum.GetValues(typeof(ENUM_RF_AU)) )
                    {
                        cmbBoxFreq.Items.Add( String.Format("{0:000.000}", (float)item/1000) );
                    }
                    cmbBoxFreq.Items.Add( ENUM_RF_COMMON.CHANNEL_ALL );
                    break;

                case MacRegion.BR:
                    foreach ( ENUM_RF_BR item in Enum.GetValues(typeof(ENUM_RF_BR)) )
                    {
                        cmbBoxFreq.Items.Add( String.Format("{0:000.000}", (float)item/1000) );
                    }
                    cmbBoxFreq.Items.Add( ENUM_RF_COMMON.CHANNEL_ALL );
                    break;

                case MacRegion.HK:
                    foreach ( ENUM_RF_HK item in Enum.GetValues(typeof(ENUM_RF_HK)) )
                    {
                        cmbBoxFreq.Items.Add( String.Format("{0:000.000}", (float)item/1000) );
                    }
                    cmbBoxFreq.Items.Add( ENUM_RF_COMMON.CHANNEL_ALL );
                    break;

                case MacRegion.MY:
                    foreach ( ENUM_RF_MY item in Enum.GetValues(typeof(ENUM_RF_MY)) )
                    {
                        cmbBoxFreq.Items.Add( String.Format("{0:000.000}", (float)item/1000) );
                    }
                    cmbBoxFreq.Items.Add( ENUM_RF_COMMON.CHANNEL_ALL );
                    break;

                case MacRegion.SG:
                    foreach ( ENUM_RF_SG item in Enum.GetValues(typeof(ENUM_RF_SG)) )
                    {
                        cmbBoxFreq.Items.Add( String.Format("{0:000.000}", (float)item/1000) );
                    }
                    cmbBoxFreq.Items.Add( ENUM_RF_COMMON.CHANNEL_ALL );
                    break;

                case MacRegion.TH:
                    foreach ( ENUM_RF_TH item in Enum.GetValues(typeof(ENUM_RF_TH)) )
                    {
                        cmbBoxFreq.Items.Add( String.Format("{0:000.000}", (float)item/1000) );
                    }
                    cmbBoxFreq.Items.Add( ENUM_RF_COMMON.CHANNEL_ALL );
                    break;

                case MacRegion.IL:
                    foreach ( ENUM_RF_IL item in Enum.GetValues(typeof(ENUM_RF_IL)) )
                    {
                        cmbBoxFreq.Items.Add( String.Format("{0:000.000}", (float)item/1000) );
                    }
                    cmbBoxFreq.Items.Add( ENUM_RF_COMMON.CHANNEL_ALL );  
                    break;

                case MacRegion.RU:
                    foreach ( ENUM_RF_RU item in Enum.GetValues(typeof(ENUM_RF_RU)) )
                    {
                        cmbBoxFreq.Items.Add( String.Format("{0:000.000}", (float)item/1000) );
                    }
                    cmbBoxFreq.Items.Add( ENUM_RF_COMMON.CHANNEL_ALL );
                    break;

                case MacRegion.IN:
                    foreach ( ENUM_RF_IN item in Enum.GetValues(typeof(ENUM_RF_IN)) )
                    {
                        cmbBoxFreq.Items.Add( String.Format("{0:000.000}", (float)item/1000) );
                    }
                    cmbBoxFreq.Items.Add( ENUM_RF_COMMON.CHANNEL_ALL );   
                    break;

                case MacRegion.SA:
                    foreach ( ENUM_RF_SA item in Enum.GetValues(typeof(ENUM_RF_SA)) )
                    {
                        cmbBoxFreq.Items.Add( String.Format("{0:000.000}", (float)item/1000) );
                    }
                    cmbBoxFreq.Items.Add( ENUM_RF_COMMON.CHANNEL_ALL );  
                    break;

                case MacRegion.JO:
                    foreach ( ENUM_RF_JO item in Enum.GetValues(typeof(ENUM_RF_JO)) )
                    {
                        cmbBoxFreq.Items.Add( String.Format("{0:000.000}", (float)item/1000) );
                    }
                    cmbBoxFreq.Items.Add( ENUM_RF_COMMON.CHANNEL_ALL );
                    break;

                case MacRegion.MX:
                    foreach ( ENUM_RF_MX item in Enum.GetValues(typeof(ENUM_RF_MX)) )
                    {
                        cmbBoxFreq.Items.Add( String.Format("{0:000.000}", (float)item/1000) );
                    }
                    cmbBoxFreq.Items.Add( ENUM_RF_COMMON.CHANNEL_ALL );   
                    break;


                //clark 2011.9.13
                case MacRegion.CUSTOMER:
                    //Set cmbBoxFreq's type to Non-ReadOnly. 
                    cmbBoxFreq.DropDownStyle = ComboBoxStyle.DropDown;
                    cmbBoxFreq.Items.Add( "000.000" );

                    string strCustomerRegion = null; 
                    Result result = m_reader.API_MacGetCustomerRegion( ref strCustomerRegion );
                    
                    cmbBoxRegion.Items.Clear();
                    switch(result)
                    {
                        case Result.OK:                                                                                    
                            //Show customer config name
                            cmbBoxRegion.Items.Add(strCustomerRegion);                            
                            break;

                        case Result.NOT_SUPPORTED:
                            cmbBoxRegion.Items.Add("Not support customer");
                            btnRun.Enabled = false;  
                            break;


                       case Result.FAILURE:
                       default:
                            cmbBoxRegion.Items.Add("Get customer fail");
                            btnRun.Enabled = false;  
                            break;
                    }
                    cmbBoxRegion.SelectedIndex = 0;
                    break;


                case MacRegion.UNKNOWN:
                default:
                    cmbBoxFreq.Items.Add(MacRegion.UNKNOWN);
                    btnRun.Enabled      = false;
                    break;
            }


            //clark 2011.9.13
            if (m_macRegion != MacRegion.UNKNOWN)
            {
                //Customer doesn't support ENUM_RF_COMMON.CHANNEL_ALL
                if( m_macRegion == MacRegion.CUSTOMER)
                    cmbBoxFreq.SelectedIndex = 0;
                else
                    cmbBoxFreq.SelectedItem = ENUM_RF_COMMON.CHANNEL_ALL;
            }


            //Port Radio Button
            if (AddAntPort() == false)
            { 
                cmbBoxFreq.Items.Add(MacRegion.UNKNOWN);
                btnRun.Enabled      = false;  
                cmbAntPort.Items.Add(ENUM_ANT_PORT.UNKNOWN);

                cmbAntPort.SelectedIndex = cmbAntPort.Items.IndexOf(ENUM_ANT_PORT.UNKNOWN);
                return;
            }

        }


        private bool AddAntPort()
        {
            Byte              btPhysicalPort = 0;
            AntennaPortConfig config         = new AntennaPortConfig();
            
            //Get Logic port 0
            if( m_reader.API_AntennaPortGetConfiguration(0 , ref config) != Result.OK )
                return false;

            m_btPhysicalPort = config.physicalPort;
            m_usPowerLevel   = config.powerLevel;


            //set numPowerLevel value
            numPowerLevel.Value = (m_usPowerLevel > numPowerLevel.Maximum) ? 
                                        numPowerLevel.Maximum : m_usPowerLevel;

            //Detect Port
            foreach
            (
                ENUM_ANT_PORT port in Enum.GetValues(typeof(ENUM_ANT_PORT) )
            )
            {
                config.physicalPort = (byte)port;

                //Set all port to check which is supported.
                if (m_reader.API_AntennaPortSetConfiguration(0, config) == Result.OK)
                { 
                    cmbAntPort.Items.Add( (byte)port );
                }

                
                if( m_btPhysicalPort == (byte)port)
                    cmbAntPort.SelectedIndex = cmbAntPort.Items.IndexOf( (byte)port );
            }

            //Restore Port Setting
            config.physicalPort = btPhysicalPort;
            if( m_reader.API_AntennaPortSetConfiguration(0 , config) != Result.OK )
                return false;            

            return true;
        }


        //==========================Event=======================                    
        private void RFTest_FormClosing(object sender, FormClosingEventArgs e)
        {

            //If do something, can't close the window.
            if ( m_bRunFlag == true )
            {
                e.Cancel = true;
                return;
            }


            //Set Frequency to hopping
            m_btChannelFlag = 0;
            m_reader.API_TestSetFrequencyConfiguration(m_btChannelFlag, m_uiExactFrequecny);
            m_Graphics.Dispose();
            m_PaintImage.Dispose();
            m_GraphicsBmp.Dispose();

            //Delete Tmp File
            try
            {
                string  strBmpPath = Path.Combine( Application.CommonAppDataPath, BMP_NAME);

                if ( File.Exists(strBmpPath) )
                {
                    File.Delete(strBmpPath);
                }
            }
            catch (Exception exception)
            { 
                ;
            }


        }
    
    

        private void RfOnButton_Click(object sender, EventArgs e)
        {
            if (m_reader == null || m_mainForm == null)
                return;

            //If is in the ldle state, start to get return loss value
            if (m_bRunFlag == false)
            {
                //Reset value
                m_OldX = 0;
                m_OldY = 0;

                //Clear panel
                m_Graphics.Clear( Color.Wheat );
                m_GraphicsBmp.Clear( Color.Wheat );
                DrawFrame();


                //Draw standard line
                if( false ==  DrawStandardLine() )
                    return;


                //Set channel type
                if ( cmbBoxFreq.Text == "CHANNEL_ALL" )  
                {
                    //All RF channel
                    m_bAllChannel = true; 
                }
                else
                {
                    //Single RF channel
                    m_bAllChannel = false;
                }
        
                //Disable ComboBox
                cmbAntPort.Enabled     = false;
                numPowerLevel.Enabled  = false;
                cmbBoxFreq.Enabled     = false;
                btnExport.Enabled      = false;

                btnRun.Text = "Stop";
                m_bRunFlag  = true;

                //Create thread to handle
                m_thd = new Thread(m_thdParam);
                m_thd.Start(this);                                       
            }        
            else    
            {       
                //use this flag to stop the process
                m_bRunFlag = false;
            }

        }



        //===================Set Data=============================
        private void StoreInforToTxt( string Path )
        {

            FileStream   fileStream   = null;
            StreamWriter streamWriter = null;
            try
            {
                fileStream = new FileStream( Path, FileMode.Create, FileAccess.Write, FileShare.None, 512, FileOptions.None);
                
            }
            catch (Exception ex)
            { 
                MessageBox.Show( "Please select a valid path.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }   
           
            int iCount = 0;
            streamWriter = new StreamWriter(fileStream);
            foreach( Infor strcData in m_listInfor )
            {   
                if(iCount>=10 && iCount%10 == 0)
                   streamWriter.WriteLine(String.Format("============================================================ {0}",iCount) );            
            
                if(strcData.dbReturnValue >= 0)
                    streamWriter.WriteLine(String.Format("{0:000}     {1}     {2:00.00}     {3}", iCount+1, strcData.strFrequency, strcData.dbReturnValue, strcData.strText));            
                else
                    streamWriter.WriteLine(String.Format("{0:000}     {1}    {2:00.00}     {3}", iCount+1, strcData.strFrequency, strcData.dbReturnValue, strcData.strText));            
                
                iCount++;
            }

            streamWriter.Flush();
            streamWriter.Close();


            Process notePad = new Process();
            // FileName 是要執行的檔案
            notePad.StartInfo.FileName = Path;
            notePad.Start();
        }

        private bool SetFreq()
        {
            if (m_reader == null || m_mainForm == null)
                return false;

            try
            {
                GetFreqValue();
            }
            catch (Exception exception)
            { 
                ShowErrMsg("Please enter valid value!");
                return false;            
            }          

            if (Result.OK != m_reader.API_TestSetFrequencyConfiguration(m_btChannelFlag, m_uiExactFrequecny))
            {
                ShowErrMsg("Set channel and frequecny unsuccessfully!");
                return false;
            }

            return true;
        }


        //Set Logic-Antenna Port and Test-Antenna Port
        private bool SetPort()
        {
            if (m_reader == null || m_mainForm == null)
                return false;

            
            AntennaPortConfig config = new AntennaPortConfig();
            AntennaPortState  State  = AntennaPortState.DISABLED;

            //Close Logic antenna port 1~15. Only use Logic antenna port 0            
            for (byte Port = 1; Port <= Source_Antenna.LOGICAL_MAXIMUM; Port++)
            {
                if (Result.OK != m_reader.API_AntennaPortSetState(Port, State))
                {
                    ShowErrMsg("Set logic antenna port State unsuccessfully!");
                    return false;
                }
            }


            GetPortValue();
            if (Result.OK != m_reader.API_TestSetAntennaPortConfiguration(m_btPhysicalPort, m_usPowerLevel))
                return false;

            //Get Logic Port 0
            if (m_reader.API_AntennaPortGetConfiguration(0, ref config) != Result.OK)
            {
                ShowErrMsg("Get antenna port0 unsuccessfully!");
                return false;
            }

            config.powerLevel   = m_usPowerLevel;
            config.physicalPort = m_btPhysicalPort;

            //Set Logic Port 0
            if (m_reader.API_AntennaPortSetConfiguration(0, config) != Result.OK)
            {
                ShowErrMsg("Set antenna port0 unsuccessfully!");
                return false;
            }
       

            return true;
        }



        private bool SetConfig()
        {
            if (m_reader == null || m_mainForm == null)
                return false;

            //clear Information
            m_strcInfor.strFrequency   = "000.000";
            m_strcInfor.dbReturnValue  = 0;
            m_strcInfor.strText        = "unsuccessfully";
            m_uiErrorCode              = 0;

            //Set Frequency Configuration
            if ( SetFreq() == false )
                return false;

            //Set Antenna test/logic Port
            if( SetPort() == false)
                return false;
 
            return true;
        }


        private bool StartRF_On( )
        {           
            if (m_reader == null || m_mainForm == null)
                return false;
           
            uint uiLastError = 0;

            
            if (Result.OK != m_reader.API_TestTurnCarrierWaveOn())
            {
                String strMsg;
                if (Result.OK != m_reader.MacGetError(out m_uiErrorCode, out uiLastError))
                {
                    strMsg = "RF On unsuccessfully";
                }
                else
                {
                    //Only allow error 0x309 and keep running the process.                  
                    if(m_uiErrorCode == 0x309)
                        return true;

                    strMsg = String.Format("Error Code: 0x{0:X3}", m_uiErrorCode);

                }


                ShowErrMsg(strMsg);
                return false;
            }

            return true;
        }



        private bool StopRF_On( )
        {           
            if (m_reader == null || m_mainForm == null)
                return false;

            uint uiLastError = 0;


           if (Result.OK != m_reader.API_TestTurnCarrierWaveOff())
            {
                String strMsg;
                if (Result.OK != m_reader.MacGetError(out m_uiErrorCode, out uiLastError))
                {
                    strMsg = "RF Off unsuccessfully";
                }
                else
                {
                    //Only allow error 0x309 and keep running the process.
                    if(m_uiErrorCode == 0x309)
                        return true;

                    strMsg = String.Format("Error Code: 0x{0:X3}", m_uiErrorCode);
                }


                ShowErrMsg(strMsg);
                return false;
            }

            return true;
        }



        //=========================Read All Channel============================
        private bool ReadSingleChannel()
        { 
            do
            {
                if( SetConfig() == false )
                    break;

                if( StartRF_On() == false )
                    break;                

                System.Threading.Thread.Sleep(500);

                HandlePaint();                  
                
               if( StopRF_On( ) == false )
                   break;

               //Only allow error 0x309.
               if (m_uiErrorCode == 0x309)
               {
                   m_strcInfor.strText = String.Format("Error Code: 0x{0:X3}", m_uiErrorCode);
               }
               else
               { 
                   m_strcInfor.strText = "Successfully";               
               }

                m_listInfor.Add(m_strcInfor);

                return true;

            }while(false);


           if (m_uiErrorCode != 0)
           {
               m_strcInfor.strText = String.Format("Error Code: {0:X3}", m_uiErrorCode);
           }

            m_listInfor.Add(m_strcInfor);            
            return false;
        }
    


        private bool ReadAllChannel()
        {

            //Set cmbBoxFreq to first frequency channel.
            SetComboFreq(ENUM_COMBO_FREQ_TYPE.RESET, 0);

            if( SetPort() == false )
                return false;
    
            //Frequency
            switch(m_macRegion)
            {
                case MacRegion.US:
                    foreach ( ENUM_RF_US item in Enum.GetValues(typeof(ENUM_RF_US)) )
                    {                        
                        if(m_bRunFlag == false)
                        {
                            SetComboFreq(ENUM_COMBO_FREQ_TYPE.LAST, 1);
                            return true;
                        }

                        if(ReadSingleChannel() == false)
                            return false;

                        SetComboFreq(ENUM_COMBO_FREQ_TYPE.ADD, 1);
                    }

                    break;

                case MacRegion.EU:
                    foreach ( ENUM_RF_EU item in Enum.GetValues(typeof(ENUM_RF_EU)) )
                    {                        
                        if(m_bRunFlag == false)
                        {
                            SetComboFreq(ENUM_COMBO_FREQ_TYPE.LAST, 1);
                            return true;
                        }

                        if(ReadSingleChannel() == false)
                            return false;

                        SetComboFreq(ENUM_COMBO_FREQ_TYPE.ADD, 1);
                    }
                    break;

                case MacRegion.JP:
                    foreach ( ENUM_RF_JP item in Enum.GetValues(typeof(ENUM_RF_JP)) )
                    {                 
                        if(m_bRunFlag == false)
                        {
                            SetComboFreq(ENUM_COMBO_FREQ_TYPE.LAST, 1);
                            return true;
                        }
                
                        if(ReadSingleChannel() == false)
                            return false;

                        SetComboFreq(ENUM_COMBO_FREQ_TYPE.ADD, 1);
                    }
 
                    break;

                case MacRegion.EU2:
                    foreach ( ENUM_RF_EU2 item in Enum.GetValues(typeof(ENUM_RF_EU2)) )
                    {                  
                        if(m_bRunFlag == false)
                        {
                            SetComboFreq(ENUM_COMBO_FREQ_TYPE.LAST, 1);
                            return true;
                        }
               
                        if(ReadSingleChannel() == false)
                            return false;

                        SetComboFreq(ENUM_COMBO_FREQ_TYPE.ADD, 1);
                    }
                    break;

                case MacRegion.TW:
                    foreach ( ENUM_RF_TW item in Enum.GetValues(typeof(ENUM_RF_TW)) )
                    {                    
                        if(m_bRunFlag == false)
                        {
                            SetComboFreq(ENUM_COMBO_FREQ_TYPE.LAST, 1);
                            return true;
                        }
             
                        if(ReadSingleChannel() == false)
                            return false;

                        SetComboFreq(ENUM_COMBO_FREQ_TYPE.ADD, 1);
                    }    
                    break;

                case MacRegion.CN:
                    foreach ( ENUM_RF_CN item in Enum.GetValues(typeof(ENUM_RF_CN)) )
                    {                   
                        if(m_bRunFlag == false)
                        {
                            SetComboFreq(ENUM_COMBO_FREQ_TYPE.LAST, 1);
                            return true;
                        }
              
                        if(ReadSingleChannel() == false)
                            return false;

                        SetComboFreq(ENUM_COMBO_FREQ_TYPE.ADD, 1);
                    }    
                    break;

                case MacRegion.KR:
                    foreach ( ENUM_RF_KR item in Enum.GetValues(typeof(ENUM_RF_KR)) )
                    {                  
                        if(m_bRunFlag == false)
                        {
                            SetComboFreq(ENUM_COMBO_FREQ_TYPE.LAST, 1);
                            return true;
                        }
               
                        if(ReadSingleChannel() == false)
                            return false;

                        SetComboFreq(ENUM_COMBO_FREQ_TYPE.ADD, 1);
                    }  
                    break;

                case MacRegion.AU:
                    foreach ( ENUM_RF_AU item in Enum.GetValues(typeof(ENUM_RF_AU)) )
                    {                   
                        if(m_bRunFlag == false)
                        {
                            SetComboFreq(ENUM_COMBO_FREQ_TYPE.LAST, 1);
                            return true;
                        }
             
                        if(ReadSingleChannel() == false)
                            return false;

                        SetComboFreq(ENUM_COMBO_FREQ_TYPE.ADD, 1);
                    }    
                    break;

                case MacRegion.BR:
                    foreach ( ENUM_RF_BR item in Enum.GetValues(typeof(ENUM_RF_BR)) )
                    {                  
                        if(m_bRunFlag == false)
                        {
                            SetComboFreq(ENUM_COMBO_FREQ_TYPE.LAST, 1);
                            return true;
                        }
               
                        if(ReadSingleChannel() == false)
                            return false;

                        SetComboFreq(ENUM_COMBO_FREQ_TYPE.ADD, 1);
                    }
                    break;

                case MacRegion.HK:
                    foreach ( ENUM_RF_HK item in Enum.GetValues(typeof(ENUM_RF_HK)) )
                    {                   
                        if(m_bRunFlag == false)
                        {
                            SetComboFreq(ENUM_COMBO_FREQ_TYPE.LAST, 1);
                            return true;
                        }
              
                        if(ReadSingleChannel() == false)
                            return false;

                        SetComboFreq(ENUM_COMBO_FREQ_TYPE.ADD, 1);
                    }
                    break;

                case MacRegion.MY:
                    foreach ( ENUM_RF_MY item in Enum.GetValues(typeof(ENUM_RF_MY)) )
                    {                
                        if(m_bRunFlag == false)
                        {
                            SetComboFreq(ENUM_COMBO_FREQ_TYPE.LAST, 1);
                            return true;
                        }
                 
                        if(ReadSingleChannel() == false)
                            return false;

                        SetComboFreq(ENUM_COMBO_FREQ_TYPE.ADD, 1);
                    }  
                    break;

                case MacRegion.SG:
                    foreach ( ENUM_RF_SG item in Enum.GetValues(typeof(ENUM_RF_SG)) )
                    {                  
                        if(m_bRunFlag == false)
                        {
                            SetComboFreq(ENUM_COMBO_FREQ_TYPE.LAST, 1);
                            return true;
                        }
               
                        if(ReadSingleChannel() == false)
                            return false;

                        SetComboFreq(ENUM_COMBO_FREQ_TYPE.ADD, 1);
                    } 
                    break;

                case MacRegion.TH:
                    foreach ( ENUM_RF_TH item in Enum.GetValues(typeof(ENUM_RF_TH)) )
                    {                  
                        if(m_bRunFlag == false)
                        {
                            SetComboFreq(ENUM_COMBO_FREQ_TYPE.LAST, 1);
                            return true;
                        }
               
                        if(ReadSingleChannel() == false)
                            return false;

                        SetComboFreq(ENUM_COMBO_FREQ_TYPE.ADD, 1);
                    }   
                    break;

                case MacRegion.IL:
                    foreach ( ENUM_RF_IL item in Enum.GetValues(typeof(ENUM_RF_IL)) )
                    {                    
                        if(m_bRunFlag == false)
                        {
                            SetComboFreq(ENUM_COMBO_FREQ_TYPE.LAST, 1);
                            return true;
                        }
             
                        if(ReadSingleChannel() == false)
                            return false;

                        SetComboFreq(ENUM_COMBO_FREQ_TYPE.ADD, 1);
                    }     
                    break;

                case MacRegion.RU:
                    foreach ( ENUM_RF_RU item in Enum.GetValues(typeof(ENUM_RF_RU)) )
                    {                    
                        if(m_bRunFlag == false)
                        {
                            SetComboFreq(ENUM_COMBO_FREQ_TYPE.LAST, 1);
                            return true;
                        }
             
                        if(ReadSingleChannel() == false)
                            return false;

                        SetComboFreq(ENUM_COMBO_FREQ_TYPE.ADD, 1);
                    }   
                    break;

                case MacRegion.IN:
                    foreach ( ENUM_RF_IN item in Enum.GetValues(typeof(ENUM_RF_IN)) )
                    {                 
                        if(m_bRunFlag == false)
                        {
                             SetComboFreq(ENUM_COMBO_FREQ_TYPE.LAST, 1);
                            return true;
                        }
   
                        if (ReadSingleChannel() == false)
                             return false;

                        SetComboFreq(ENUM_COMBO_FREQ_TYPE.ADD, 1);
                    }   
                    break;

                case MacRegion.SA:
                    foreach ( ENUM_RF_SA item in Enum.GetValues(typeof(ENUM_RF_SA)) )
                    {                  
                        if(m_bRunFlag == false)
                        {
                            SetComboFreq(ENUM_COMBO_FREQ_TYPE.LAST, 1);
                            return true;
                        }
               
                        if(ReadSingleChannel() == false)
                            return false;

                        SetComboFreq(ENUM_COMBO_FREQ_TYPE.ADD, 1);
                    }   
                    break;

                case MacRegion.JO:
                    foreach ( ENUM_RF_JO item in Enum.GetValues(typeof(ENUM_RF_JO)) )
                    {                  
                        if(m_bRunFlag == false)
                        {
                            SetComboFreq(ENUM_COMBO_FREQ_TYPE.LAST, 1);
                            return true;
                        }
               
                        if(ReadSingleChannel() == false)
                            return false;

                        SetComboFreq(ENUM_COMBO_FREQ_TYPE.ADD, 1);
                    } 
                    break;

                case MacRegion.MX:
                    foreach ( ENUM_RF_MX item in Enum.GetValues(typeof(ENUM_RF_MX)) )
                    {                  
                        if(m_bRunFlag == false)
                        {
                            SetComboFreq(ENUM_COMBO_FREQ_TYPE.LAST, 1);
                            return true;
                        }
               
                        if(ReadSingleChannel() == false)
                            return false;

                        SetComboFreq(ENUM_COMBO_FREQ_TYPE.ADD, 1);
                    }       
                    break;


                case MacRegion.UNKNOWN:
                default:
                    ShowErrMsg("Doesn't support this region");
                    return false;
            }

            return true;
        }



        public static void DoThdWork(object r_Cls)
        {
            ReturnLoss thisCls = r_Cls as ReturnLoss;
            if (thisCls == null) return;

            //Clear information
            thisCls.m_listInfor.Clear();

            if (thisCls.m_bAllChannel == true)
            {
                //Start to read all channel
                thisCls.ReadAllChannel();
            }
            else
            { 
                //Start to read single channel
                thisCls.ReadSingleChannel();            
            }            

            //Enable all compontent
            thisCls.EnableForm();

            return;
        }




        //=========================Delegate、Invoke============================
        private void SetComboFreq( ENUM_COMBO_FREQ_TYPE enumFreqType, int iCount )
        {

            if ( InvokeRequired )
            {
                Invoke( dlgSetComboFreq, new object[]{ enumFreqType, iCount}  );
                return;
            }


            switch (enumFreqType)
            { 
                case ENUM_COMBO_FREQ_TYPE.RESET:
                    cmbBoxFreq.SelectedIndex = 0;
                    break;

                case ENUM_COMBO_FREQ_TYPE.LAST:
                    cmbBoxFreq.SelectedItem = ENUM_RF_COMMON.CHANNEL_ALL;
                    break;   

                case ENUM_COMBO_FREQ_TYPE.ADD:
                    cmbBoxFreq.SelectedIndex += iCount;
                    break;

                case ENUM_COMBO_FREQ_TYPE.SUB:
                    cmbBoxFreq.SelectedIndex -= iCount;
                    break;
 
                case ENUM_COMBO_FREQ_TYPE.CHOOSE:
                    cmbBoxFreq.SelectedIndex = iCount;
                    break;        
            }

            enumFreqType = ENUM_COMBO_FREQ_TYPE.NO;
        }





        void GetPortValue()
        {
            if ( InvokeRequired )
            {
                Invoke( new MethodInvoker( GetPortValue )  );
                return;
            }    


            //Set Test Antenna Port
            m_usPowerLevel   = (UInt16)numPowerLevel.Value;
            m_btPhysicalPort = (byte)cmbAntPort.SelectedItem;    
        }



        void GetFreqValue()
        {
            if ( InvokeRequired )
            {
                Invoke( new MethodInvoker( GetFreqValue )  );
                return;
            }    

            //Set Frequency Configuration
            m_btChannelFlag    = 1; //Single
          
            if (cmbBoxFreq.Text == "CHANNEL_ALL")
                m_uiExactFrequecny = 0;
            else
                m_uiExactFrequecny = (uint)(Convert.ToDouble(cmbBoxFreq.Text) * 1000);   

            m_strcInfor.strFrequency = cmbBoxFreq.Text;  
        }
       

        public void EnableForm()
        { 
            if ( InvokeRequired )
            {
                Invoke( new MethodInvoker( EnableForm ) );
                return;
            }

            //Enable ComboBox
            cmbAntPort.Enabled     = true;
            numPowerLevel.Enabled  = true;
            cmbBoxFreq.Enabled     = true;
            btnRun.Enabled         = true;
            btnExport.Enabled      = true;  

            m_bRunFlag    = false;
            btnRun.Text  = "Run";  

            return;
        }


                
        private void ShowErrMsg(String strMsg)
        { 
            if ( InvokeRequired )
            {
                Invoke( dlgShowErrMsg, new object[]{strMsg} );
                return;
            }

            m_strcInfor.strText = strMsg;

            MessageBox.Show( strMsg,
                             "Return Loss",
                             MessageBoxButtons.OK, 
                             MessageBoxIcon.Error    );    
        }



        private void ShowPaint( double r_dbValue,  Point[] r_point )
        {
        
            if ( InvokeRequired )
            {
                Invoke( dlgShowPaint, new object[]{ r_dbValue, r_point} );
                return;
            }
        

            //Lines
            m_Graphics.DrawLines( new Pen(Color.Black), r_point );
            m_GraphicsBmp.DrawLines( new Pen(Color.Black), r_point );

            //Point
            DrawPieRectangle( r_point[1].X ,  r_point[1].Y, Color.Purple );
            
            //String return loss value
            DrawString( String.Format("{0}",  r_dbValue.ToString()) , 
                        r_point[1].X-6 ,  r_point[1].Y + 15,
                        StringFormatFlags.LineLimit,
                        6,
                        Color.Purple                                       );

            //String                       
            DrawString( String.Format("{0:000.000}", (float)m_uiExactFrequecny/1000), 
                        r_point[1].X-7 ,  410, 
                        StringFormatFlags.DirectionVertical,
                        Color.Red                                                       );


            //Show current return loss value to TextBox
            SetCurValueToText(r_dbValue);
            //m_PaintBmp.Save("TmpPaint.bmp");
        }


        public void SetCurValueToText( double r_dbValue )
        {
            if ( InvokeRequired )
            {
                Invoke( dlgSetCurValueToText, new object[]{ r_dbValue} );
                return;
            }
                 
            textCurValue.Text = r_dbValue.ToString();
        }



        private void ShowPic()
        {
            if ( InvokeRequired )
            {
                Invoke( new MethodInvoker( ShowPic )  );
                return;
            }      

            //picPaint.Image = m_PaintBmp;    
            picPaint.Image = m_PaintImage;  
        }

        private void splitContainer1_Panel2_Resize(object sender, EventArgs e)
        {
            ShowPic();
        }
        

        private void DrawFrame()
        {
            if ( InvokeRequired )
            {
                Invoke( new MethodInvoker( DrawFrame )  );
                return;
            }  

            Point []point;

            //Colum
            for( int i = 0 ; i <= 50; i++)
            {                
                point = new Point [] { new Point(i*20 + PAINT_SHIFT, 0), new Point(i*20 + PAINT_SHIFT, 400) };
                m_Graphics.DrawLines( new Pen(Color.Snow), point );
                m_GraphicsBmp.DrawLines( new Pen(Color.Snow), point );          
            }
            DrawString( String.Format( "{0}", "Frequency\n(MHz)") , PAINT_SHIFT + 1020, 400, StringFormatFlags.LineLimit, Color.Red );



            //Row
            for( int i = 0 ; i <= 20; i++)
            {
               
                point = new Point [] { new Point(0 + PAINT_SHIFT, i*20), new Point(1000 + PAINT_SHIFT, i*20) };
                m_Graphics.DrawLines( new Pen(Color.Snow), point );
                m_GraphicsBmp.DrawLines( new Pen(Color.Snow), point );   

                
                DrawString( String.Format( i==0? " {0}":"-{0}", i*2) , PAINT_SHIFT - 25,  i*20-5, StringFormatFlags.LineLimit, Color.Red);
            }            
            DrawString( String.Format( "{0}", "Return Loss\n(dB)") ,  PAINT_SHIFT - 100 , 5, StringFormatFlags.LineLimit, Color.Red );
        }



        public void DrawString(String r_strVal, int x, int y, StringFormatFlags r_Type, Color r_color)
        {
            
            if ( InvokeRequired )
            {
                Invoke( dlgDrawString, new object[]{ r_strVal, x, y, r_Type}, r_color );
                return;
            }

            // Create string to draw.
            String drawString = r_strVal;
                     
            // Create font and brush.
            Font drawFont = new Font("Arial", 8);
            SolidBrush drawBrush = new SolidBrush(r_color);                     
                     
            // Set format of string.
            StringFormat drawFormat = new StringFormat();
            drawFormat.FormatFlags = r_Type;
                     
            // Draw string to screen.
            m_Graphics.DrawString(drawString, drawFont, drawBrush, x, y , drawFormat);
            m_GraphicsBmp.DrawString(drawString, drawFont, drawBrush, x, y , drawFormat);
        }


        public void DrawString(String r_strVal, int x, int y, StringFormatFlags r_Type, int r_size, Color r_color)
        {
            
            if ( InvokeRequired )
            {
                Invoke( dlgDrawString, new object[]{ r_strVal, x, y, r_Type}, r_color );
                return;
            }

            // Create string to draw.
            String drawString = r_strVal;
                     
            // Create font and brush.
            Font drawFont = new Font("Arial", r_size);
            SolidBrush drawBrush = new SolidBrush(r_color);                     
                     
            // Set format of string.
            StringFormat drawFormat = new StringFormat();
            drawFormat.FormatFlags = r_Type;
                     
            // Draw string to screen.
            m_Graphics.DrawString(drawString, drawFont, drawBrush, x, y , drawFormat);
            m_GraphicsBmp.DrawString(drawString, drawFont, drawBrush, x, y , drawFormat);
        }

        //=======================Paint=============================
        private void HandlePaint()
        { 
            int    iRevPwr     = 0;
            int    iPaPwr      = 0;
            uint   uiOemRevPwr = 0;
            uint   uiOemPaPwr  = 0;
            int    iValue      = 0;  
            int    m_NewX      = 0;
            int    m_NewY      = PaintPanel.Height;          
            double dbValue     = 0;  

            //Read return loss data
            if ( m_reader.MacReadRegister(0xB04, out uiOemRevPwr) != Result.OK ||
                 m_reader.MacReadRegister(0xB00, out uiOemPaPwr)  != Result.OK      )
            { 
                ShowErrMsg("Get return loss fail");
                return;
            }

            iRevPwr = (int)uiOemRevPwr;
            iPaPwr  = (int)uiOemPaPwr;

            //calculate return loss value 
            dbValue = iValue  = iRevPwr - iPaPwr;
            dbValue = (dbValue != 0) ? dbValue / 10 : 0;
            
            //calculate point  
            m_NewX = m_NewX+ PAINT_SHIFT;
            //??return loss??//
            //m_NewY = Math.Abs( iValue );
            m_NewY = (iValue > 0) ? m_NewY = 0 : m_NewY = Math.Abs(iValue);/*Add by Rick for return loss,2013-01-29*/

            //Push Data to Queue
            m_strcInfor.dbReturnValue = dbValue;           
                       
            //First time, don't move the point
            if (m_OldX == 0 && m_OldY == 0)
            {
                m_OldX = m_NewX;
                m_OldY = m_NewY;
            }
            else
            { 
                //Every time, add 20 shift.
                m_NewX = m_OldX + 20;                     
            }

            //push point to variable
            Point[] ps = new Point[]{ new Point(m_OldX, m_OldY),
                                      new Point(m_NewX, m_NewY)  };
            
            //show point in the window
            ShowPaint( dbValue, ps );

            //Store point
            m_OldX = m_NewX;
            m_OldY = m_NewY;
        }
        
        private void splitContainer1_Panel2_Scroll(object sender, ScrollEventArgs e)
        { 
            ShowPic();
        }


        public void DrawPieRectangle(int x, int y, Color r_color )
        {
                     
            // Create pen.
            Pen pen = new Pen(r_color, 3);
                     
            // Create rectangle for ellipse.
            Rectangle rect = new Rectangle(x, y-1, 3, 3);
                     
            // Create start and sweep angles.
            float startAngle =   0.0F;
            float sweepAngle = 360.0F;
                     
            // Draw pie to screen.
            m_Graphics.DrawPie(pen, rect, startAngle, sweepAngle);
            m_GraphicsBmp.DrawPie(pen, rect, startAngle, sweepAngle);
        }
        


        private bool DrawStandardLine()
        { 
            //if ( InvokeRequired )
            //{
            //    Invoke( DrawStandardLine, new object[]{ r_dbValue, r_point} );
            //    return;
            //}

            uint uiThreshold = 0;

            if (Result.OK != m_reader.MacReadOemData((ushort)enumOEM_ADDR.THRESHOLD, ref uiThreshold))
            { 
                ShowErrMsg("Get threshold unsuccessfully!");
                return false;
            }
                  
            int threshold =  (int)uiThreshold ;

            Point []point = new Point [] { new Point(PAINT_SHIFT, threshold+1), 
                                           new Point(1000 + PAINT_SHIFT, threshold+1) }; 

            //Line
            m_Graphics.DrawLines( new Pen(Color.OrangeRed, 2), point );
            m_GraphicsBmp.DrawLines( new Pen(Color.OrangeRed, 2), point );

            //String                                   
            DrawString( String.Format("Threshold :   {0}",  -( (float)threshold/10 ) ), 
                        point[1].X+4,  point[1].Y-7, 
                        StringFormatFlags.LineLimit,
                        Color.OrangeRed                                                         );

            return true;
        }



        private void btnExport_Click(object sender, EventArgs e)
        {

            if (m_reader == null || m_mainForm == null)
                return;

            //If do something, can't close the window.
            if ( m_bRunFlag == true )
                return;

            if (m_listInfor.Count <= 0)
            { 
                MessageBox.Show( "No Data.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;            
            }


            //btnExport
            const string DEF_FILE_NAME = "RFID Return Loss Data.txt";
            const string DEF_FILE_PATH = "C:\\";
            
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter   = "TXT (*.txt)|*.txt|ALL Files (*.*)|*.*";
            saveFileDialog1.Title    = "Save a File";
            saveFileDialog1.FileName = Path.Combine( DEF_FILE_PATH, DEF_FILE_NAME );

            try
            {
                if (saveFileDialog1.ShowDialog() != DialogResult.OK)
                    return;
            }
            catch (Exception ex)
            { 
                MessageBox.Show( "Please select a valid path.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }    

            StoreInforToTxt(saveFileDialog1.FileName);
        }


    }
}
