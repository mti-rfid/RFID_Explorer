using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using RFID.RFIDInterface;
using rfid.Constants;
using rfid.Structures;
using Global;
using System.Text.RegularExpressions;

namespace RFID_Explorer
{

    public partial class RFTest : Form
    {
        private static RFID_Explorer.mainForm m_mainForm  = null;
        private        LakeChabotReader     m_reader    = null;

        private bool      m_bInRfOn          = false;
        private byte      m_btChannelFlag    = 0;
        private byte      m_btPhysicalPort   = 0;
        private UInt16    m_uiPulseOnTime    = 0;
        private UInt16    m_uiPulseOffTime   = 0;
        private UInt16    m_usPowerLevel     = 0;
        private UInt32    m_uiExactFrequecny = 0;
        private MacRegion m_macRegion        = rfid.Constants.MacRegion.UNKNOWN;


        private const uint PULSE_TIME_MAX = 0xFFFF;
        private const uint PULSE_TIME_MIN = 0;
  

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

       
  
        public RFTest(RFID_Explorer.mainForm r_form, LakeChabotReader rm_reader)
        {
            InitializeComponent();

            m_mainForm = r_form;
            m_reader   = rm_reader;

            //Initial value
            ckboxErrorKeepRunning.Checked = m_mainForm.bErrorKeepRunning;


            //Read Data from module
            m_reader.API_TestGetAntennaPortConfiguration(ref m_btPhysicalPort, ref m_usPowerLevel);
            m_reader.API_TestGetFrequencyConfiguration(ref m_btChannelFlag, ref m_uiExactFrequecny);
            m_reader.API_TestGetRandomDataPulseTime(ref m_uiPulseOnTime, ref m_uiPulseOffTime);


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
                    break;

                case MacRegion.EU:
                    foreach ( ENUM_RF_EU item in Enum.GetValues(typeof(ENUM_RF_EU)) )
                    {
                        cmbBoxFreq.Items.Add( String.Format("{0:000.000}", (float)item/1000) );
                    }   
                    break;

                case MacRegion.JP:
                    foreach ( ENUM_RF_JP item in Enum.GetValues(typeof(ENUM_RF_JP)) )
                    {
                        cmbBoxFreq.Items.Add( String.Format("{0:000.000}", (float)item/1000) );
                    }     
                    break;

                case MacRegion.EU2:
                    foreach ( ENUM_RF_EU2 item in Enum.GetValues(typeof(ENUM_RF_EU2)) )
                    {
                        cmbBoxFreq.Items.Add( String.Format("{0:000.000}", (float)item/1000) );
                    }   
                    break;

                case MacRegion.TW:
                    foreach ( ENUM_RF_TW item in Enum.GetValues(typeof(ENUM_RF_TW)) )
                    {
                        cmbBoxFreq.Items.Add( String.Format("{0:000.000}", (float)item/1000) );
                    }      
                    break;

                case MacRegion.CN:
                    foreach ( ENUM_RF_CN item in Enum.GetValues(typeof(ENUM_RF_CN)) )
                    {
                        cmbBoxFreq.Items.Add( String.Format("{0:000.000}", (float)item/1000) );
                    }      
                    break;

                case MacRegion.KR:
                    foreach ( ENUM_RF_KR item in Enum.GetValues(typeof(ENUM_RF_KR)) )
                    {
                       cmbBoxFreq.Items.Add( String.Format("{0:000.000}", (float)item/1000) );
                    }    
                    break;

                case MacRegion.AU:
                    foreach ( ENUM_RF_AU item in Enum.GetValues(typeof(ENUM_RF_AU)) )
                    {
                        cmbBoxFreq.Items.Add( String.Format("{0:000.000}", (float)item/1000) );
                    }      
                    break;

                case MacRegion.BR:
                    foreach ( ENUM_RF_BR item in Enum.GetValues(typeof(ENUM_RF_BR)) )
                    {
                        cmbBoxFreq.Items.Add( String.Format("{0:000.000}", (float)item/1000) );
                    }  
                    break;

                case MacRegion.HK:
                    foreach ( ENUM_RF_HK item in Enum.GetValues(typeof(ENUM_RF_HK)) )
                    {
                        cmbBoxFreq.Items.Add( String.Format("{0:000.000}", (float)item/1000) );
                    } 
                    break;

                case MacRegion.MY:
                    foreach ( ENUM_RF_MY item in Enum.GetValues(typeof(ENUM_RF_MY)) )
                    {
                        cmbBoxFreq.Items.Add( String.Format("{0:000.000}", (float)item/1000) );
                    }     
                    break;

                case MacRegion.SG:
                    foreach ( ENUM_RF_SG item in Enum.GetValues(typeof(ENUM_RF_SG)) )
                    {
                        cmbBoxFreq.Items.Add( String.Format("{0:000.000}", (float)item/1000) );
                    }     
                    break;

                case MacRegion.TH:
                    foreach ( ENUM_RF_TH item in Enum.GetValues(typeof(ENUM_RF_TH)) )
                    {
                        cmbBoxFreq.Items.Add( String.Format("{0:000.000}", (float)item/1000) );
                    }     
                    break;

                case MacRegion.IL:
                    foreach ( ENUM_RF_IL item in Enum.GetValues(typeof(ENUM_RF_IL)) )
                    {
                        cmbBoxFreq.Items.Add( String.Format("{0:000.000}", (float)item/1000) );
                    }      
                    break;

                case MacRegion.RU:
                    foreach ( ENUM_RF_RU item in Enum.GetValues(typeof(ENUM_RF_RU)) )
                    {
                        cmbBoxFreq.Items.Add( String.Format("{0:000.000}", (float)item/1000) );
                    }      
                    break;

                case MacRegion.IN:
                    foreach ( ENUM_RF_IN item in Enum.GetValues(typeof(ENUM_RF_IN)) )
                    {
                        cmbBoxFreq.Items.Add( String.Format("{0:000.000}", (float)item/1000) );
                    }       
                    break;

                case MacRegion.SA:
                    foreach ( ENUM_RF_SA item in Enum.GetValues(typeof(ENUM_RF_SA)) )
                    {
                        cmbBoxFreq.Items.Add( String.Format("{0:000.000}", (float)item/1000) );
                    }       
                    break;

                case MacRegion.JO:
                    foreach ( ENUM_RF_JO item in Enum.GetValues(typeof(ENUM_RF_JO)) )
                    {
                        cmbBoxFreq.Items.Add( String.Format("{0:000.000}", (float)item/1000) );
                    }     
                    break;

                case MacRegion.MX:
                    foreach ( ENUM_RF_MX item in Enum.GetValues(typeof(ENUM_RF_MX)) )
                    {
                        cmbBoxFreq.Items.Add( String.Format("{0:000.000}", (float)item/1000) );
                    }         
                    break;

                //clark 2011.9.13
                case MacRegion.CUSTOMER:
                    //Set cmbBoxFreq's type to Non-ReadOnly. 
                    cmbBoxFreq.DropDownStyle = ComboBoxStyle.DropDown;
                    cmbBoxFreq.Items.Add( "000.000" );

                    string strCustomerRegion = "";    
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
                            btnRfOn.Enabled      = false;
                            btnRfOff.Enabled     = false;
                            btnRF.Enabled = false;//btnRF Combine
                            btnInventory.Enabled = false;
                            btnPulse.Enabled     = false;            
                            btnClear.Enabled     = false;
                            break;


                       case Result.FAILURE:
                       default:
                            cmbBoxRegion.Items.Add("Get customer fail");
                            btnRfOn.Enabled      = false;
                            btnRfOff.Enabled     = false;
                            btnRF.Enabled = false;//btnRF Combine
                            btnInventory.Enabled = false;
                            btnPulse.Enabled     = false;            
                            btnClear.Enabled     = false;
                            break;
                    }
                    cmbBoxRegion.SelectedIndex = 0;
                    break;

                case MacRegion.UNKNOWN:
                default:
                    cmbBoxFreq.Items.Add(MacRegion.UNKNOWN);
                    btnRfOn.Enabled      = false;
                    btnRfOff.Enabled     = false;
                    btnRF.Enabled = false;//btnRF Combine
                    btnInventory.Enabled = false;
                    btnPulse.Enabled     = false;            
                    btnClear.Enabled     = false;
                    break;
            }
            cmbBoxFreq.SelectedIndex   = 0;



            //RF Channel Radio Button
            if (m_btChannelFlag == 0)
                rdoBtnMultiChannel.Checked = true;
            else
                rdoBtnSingleChannel.Checked = true;



            //Port Radio Button
            if (AddAntPort() == false)
            { 
                cmbBoxFreq.Items.Add(MacRegion.UNKNOWN);
                btnRfOn.Enabled      = false;
                btnRfOff.Enabled     = false;
                btnRF.Enabled = false;//btnRF Combine
                btnInventory.Enabled = false;
                btnPulse.Enabled     = false;    
                cmbAntPort.Items.Add(ENUM_ANT_PORT.UNKNOWN);

                cmbAntPort.SelectedIndex = cmbAntPort.Items.IndexOf(ENUM_ANT_PORT.UNKNOWN);
                return;
            }



            //Pulse Time
            numPulseOnTime.Maximum  = PULSE_TIME_MAX/1000;
            numPulseOnTime.Minimum  = PULSE_TIME_MIN/1000;
            numPulseOffTime.Maximum = PULSE_TIME_MAX/1000;
            numPulseOffTime.Minimum = PULSE_TIME_MIN/1000;
            numPulseOnTime.Value    = (m_uiPulseOnTime > 0)  ? m_uiPulseOnTime/ 1000  : numPulseOnTime.Minimum;     //us to ms
            numPulseOffTime.Value   = (m_uiPulseOffTime > 0) ? m_uiPulseOffTime/ 1000 : numPulseOffTime.Minimum;   //us to ms


            //Set Event
            m_mainForm.CurrentContextChanged += new EventHandler(CurrentContextChanged);
            m_mainForm.BindAllFunctionControlers(FunctionStateChanged, true);
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


      

        private bool SetConfig()
        {
            if (m_reader == null || m_mainForm == null)
                return false;


            //Set Frequency Configuration
            m_btChannelFlag    = (rdoBtnMultiChannel.Checked == true) ? (byte)0 : (byte)1;

            //Get value from cmbBoxFreq
            try
            {
                m_uiExactFrequecny = (uint)(Convert.ToDouble(cmbBoxFreq.Text) * 1000);     
            }
            catch(Exception exception)
            { 
                ShowErrMsg("Please enter valid value!");
                return false;            
            }
            //m_uiExactFrequecny = (uint)(Convert.ToDouble(cmbBoxFreq.Text) * 1000); 
            if (Result.OK != m_reader.API_TestSetFrequencyConfiguration(m_btChannelFlag, m_uiExactFrequecny))
            {
                ShowErrMsg("Set channel and frequecny unsuccessfully!");
                return false;
            }




            //Set Logic-Antenna Port and Test-Antenna Port
            {
                AntennaPortConfig config = new AntennaPortConfig();
                AntennaPortState  State  = AntennaPortState.DISABLED;

                for (byte Port = 0; Port <= Source_Antenna.LOGICAL_MAXIMUM; Port++)
                {
                    if (Result.OK != m_reader.API_AntennaPortGetConfiguration(Port, ref config))
                    {
                        ShowErrMsg("Get logic antenna port State unsuccessfully!");
                        return false;
                    }
                }
                
                /*
                //Close Logic antenna port 1~15. Only use Logic antenna port 0            
                for (byte Port = 1; Port <= Source_Antenna.LOGICAL_MAXIMUM; Port++)
                {
                    if (Result.OK != m_reader.API_AntennaPortSetState(Port, State))
                    {
                        ShowErrMsg("Set logic antenna port State unsuccessfully!");
                         return false;
                    }
                }
                 */


                //Set Test Antenna Port
                m_usPowerLevel   = (UInt16)numPowerLevel.Value;
                m_btPhysicalPort = (byte)cmbAntPort.SelectedItem;
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
            }


            //Set Pulse Time
            m_uiPulseOnTime  = (UInt16)(numPulseOnTime.Value * 1000);     //ms to us
            m_uiPulseOffTime = (UInt16)(numPulseOffTime.Value * 1000);
            if (Result.OK != m_reader.API_TestSetRandomDataPulseTime(m_uiPulseOnTime, m_uiPulseOffTime))
            {
                ShowErrMsg("Set pulse time unsuccessfully!");
                return false;
            }

            return true;
        }


//===============================Event==============================================                            
        private void RFTest_FormClosing(object sender, FormClosingEventArgs e)
        {

            //If do something, can't close the window.
            FunctionControl.FunctionState newState =
                 m_mainForm.ActiveControler == null ? FunctionControl.FunctionState.Unknown : m_mainForm.ActiveControler.State;

            if ( newState == FunctionControl.FunctionState.Unknown || 
                 newState != FunctionControl.FunctionState.Idle    ||
                 m_bInRfOn == true)
            {
                e.Cancel = true;
                return;
            }

            //Set Frequency to hopping
            m_btChannelFlag = 0;
            m_reader.API_TestSetFrequencyConfiguration(m_btChannelFlag, m_uiExactFrequecny);

            //Remove Event
            m_mainForm.BindAllFunctionControlers(FunctionStateChanged, false);
        }




        //Only RF ON/OFF doesn't use "Tag Access flow".
        private void RfOnButton_Click(object sender, EventArgs e)
        {
            if (m_reader == null || m_mainForm == null)
                return;

            //Set configuration
            if ( false == SetConfig() )           
                return;


            uint uiError     = 0;
            uint uiLastError = 0;

            if( Result.OK != m_reader.API_TestTurnCarrierWaveOn() )
            {
                String strMsg;
                if (Result.OK != m_reader.MacGetError(out uiError, out uiLastError))
                {
                    strMsg = "RF On unsuccessfully";
                }
                else
                { 
                    strMsg = String.Format("Error Code: 0x{0:X3}",  uiError);

                }


                ShowErrMsg(strMsg);
                return;
            }

            m_bInRfOn = true;
                  
            //Disable Button
            btnInventory.Enabled = false;
            btnPulse.Enabled     = false;
            btnClear.Enabled     = false;    
        
            //Disable
            numPulseOnTime.Enabled        = false;
            numPulseOffTime.Enabled       = false;
            cmbAntPort.Enabled            = false;
            numPowerLevel.Enabled         = false;
            cmbBoxFreq.Enabled            = false;               
            rdoBtnMultiChannel.Enabled    = false;
            rdoBtnSingleChannel.Enabled   = false;
            ckboxErrorKeepRunning.Enabled = false;
            this.ControlBox               = false;
            rdoBtnContinuous.Enabled = false;
            rdoBtnNonContinuous.Enabled = false;
 
        }


        
        private void btnRfOff_Click(object sender, EventArgs e)
        {
            if (m_reader == null || m_mainForm == null)
                return;
            
            uint uiError     = 0;
            uint uiLastError = 0;
      

            if( Result.OK != m_reader.API_TestTurnCarrierWaveOff() )
            {
                String strMsg;
                if (Result.OK != m_reader.MacGetError(out uiError, out uiLastError))
                {
                    strMsg = "RF Off unsuccessfully";
                }
                else
                { 
                    strMsg = String.Format("Error Code: {0:X3}",  uiError);

                }

                ShowErrMsg(strMsg);   
                return;
            }

            m_bInRfOn = false;
                  
            //Enable Button
            btnInventory.Enabled = true;
            btnPulse.Enabled     = true;
            btnClear.Enabled     = true;

            //Enable
            numPulseOnTime.Enabled        = true;
            numPulseOffTime.Enabled       = true;
            cmbAntPort.Enabled            = true;
            numPowerLevel.Enabled         = true;              
            rdoBtnMultiChannel.Enabled    = true;
            rdoBtnSingleChannel.Enabled   = true;
            ckboxErrorKeepRunning.Enabled = true;
            this.ControlBox               = true;
            rdoBtnContinuous.Enabled = true;
            rdoBtnNonContinuous.Enabled = true;
            TemperatureBTN.Enabled = true;
            textBoxTemperature.Enabled = true;
            cmbBoxRegion.Enabled = true;


            //If set "hopping config", only support "inventory" command.
            CheckChannel();       
        }

        private void btnRF_Click(object sender, EventArgs e)
        {
            if (m_reader == null || m_mainForm == null)
                return;

            uint uiError = 0;
            uint uiLastError = 0;

            if (btnRF.Text == "RF On")
            {
                //Set configuration
                if (false == SetConfig())
                    return;

                if (Result.OK != m_reader.API_TestTurnCarrierWaveOn())
                {
                    String strMsg;
                    if (Result.OK != m_reader.MacGetError(out uiError, out uiLastError))
                    {
                        strMsg = "RF On unsuccessfully";
                    }
                    else
                    {
                        strMsg = String.Format("Error Code: 0x{0:X3}", uiError);

                    }
                    ShowErrMsg(strMsg);
                    return;
                }

                m_bInRfOn = true;

                //Disable Button
                btnInventory.Enabled = false;
                btnPulse.Enabled = false;
                btnClear.Enabled = false;

                //Disable
                numPulseOnTime.Enabled = false;
                numPulseOffTime.Enabled = false;
                cmbAntPort.Enabled = false;
                numPowerLevel.Enabled = false;
                cmbBoxFreq.Enabled = false;
                rdoBtnMultiChannel.Enabled = false;
                rdoBtnSingleChannel.Enabled = false;
                ckboxErrorKeepRunning.Enabled = false;
                this.ControlBox = false;
                rdoBtnContinuous.Enabled = false;
                rdoBtnNonContinuous.Enabled = false;
                TemperatureBTN.Enabled = false;
                textBoxTemperature.Enabled = false;
                btnRF.Text = "RF Off";
                cmbBoxRegion.Enabled = false;
            }
            else
            {
                if (Result.OK != m_reader.API_TestTurnCarrierWaveOff())
                {
                    String strMsg;
                    if (Result.OK != m_reader.MacGetError(out uiError, out uiLastError))
                    {
                        strMsg = "RF Off unsuccessfully";
                    }
                    else
                    {
                        strMsg = String.Format("Error Code: {0:X3}", uiError);
                    }
                    ShowErrMsg(strMsg);
                    return;
                }
                m_bInRfOn = false;

                //Enable Button
                btnInventory.Enabled = true;
                btnPulse.Enabled = true;
                btnClear.Enabled = true;

                //Enable
                numPulseOnTime.Enabled = true;
                numPulseOffTime.Enabled = true;
                cmbAntPort.Enabled = true;
                numPowerLevel.Enabled = true;
                rdoBtnMultiChannel.Enabled = true;
                rdoBtnSingleChannel.Enabled = true;
                ckboxErrorKeepRunning.Enabled = true;
                this.ControlBox = true;
                rdoBtnContinuous.Enabled = true;
                rdoBtnNonContinuous.Enabled = true;
                TemperatureBTN.Enabled = true;
                textBoxTemperature.Enabled = true;
                btnRF.Text = "RF On";
                cmbBoxRegion.Enabled = true;


                //If set "hopping config", only support "inventory" command.
                CheckChannel();       
            }
        }


        //Use "Tag Access flow".
        private void InventoryButton_Click(object sender, EventArgs e)
        {
            if (m_reader == null || m_mainForm == null)
                return;

                   
            if (btnInventory.Text == "Inventory On")
            {     
                //Set configuration
                if ( false == SetConfig() )
                    return;

                //Disable Button
                btnRfOn.Enabled      = false;
                btnRfOff.Enabled     = false;
                btnRF.Enabled = false;//btnRF Combine
                btnPulse.Enabled     = false;
                btnClear.Enabled     = false;

                //Disable
                numPulseOnTime.Enabled        = false;
                numPulseOffTime.Enabled       = false;
                cmbAntPort.Enabled            = false;
                numPowerLevel.Enabled         = false;
                cmbBoxFreq.Enabled            = false;                
                rdoBtnMultiChannel.Enabled    = false;
                rdoBtnSingleChannel.Enabled   = false;
                ckboxErrorKeepRunning.Enabled = false;
                this.ControlBox               = false;
                rdoBtnContinuous.Enabled = false;
                rdoBtnNonContinuous.Enabled = false;
                TemperatureBTN.Enabled = false;
                textBoxTemperature.Enabled = false;
                cmbBoxRegion.Enabled = false;

                btnInventory.Text = "Inventory Off";

                Global.TagAccessFlag strcTagFlag;
                strcTagFlag.PostMatchFlag = 0;
                strcTagFlag.SelectOpsFlag = 0;
                strcTagFlag.RetryCount = 0;
                
                
                //Set Inventory Rule
                if (rdoBtnContinuous.Checked == true)
                {
                    strcTagFlag.bErrorKeepRunning = m_mainForm.bErrorKeepRunning;
                    m_mainForm.ActiveReader.strcTagFlag = strcTagFlag;
                    m_mainForm.StartMonitorInventory();     //Continuous
                }
                else
                {
                    strcTagFlag.bErrorKeepRunning = false;
                    m_mainForm.ActiveReader.strcTagFlag = strcTagFlag;
                    m_mainForm.StartInventoryOnce();     //non-Continuous
                }
            }
            else
            {
                //Abort
                if (m_mainForm.ActiveControler != null)
                    //m_mainForm.ActiveControler.RequestAbort();//??modified for abort??//
                    m_mainForm.ActiveControler.RequestStop();/*Add by Rick for abort,2013-1-29*/
            }
        }


        //Use "Tag Access flow".
        private void PulseButton_Click(object sender, EventArgs e)
        {
            if (m_reader == null || m_mainForm == null)
                return;

       
            if (btnPulse.Text == "Pulse On")
            {     
 
                //Set configuration
                if ( false == SetConfig() )
                    return;


                //Disable Button
                btnRfOn.Enabled      = false;
                btnRfOff.Enabled     = false;
                btnRF.Enabled = false;
                btnInventory.Enabled = false;
                btnClear.Enabled     = false;

                //Disable
                numPulseOnTime.Enabled        = false;
                numPulseOffTime.Enabled       = false;
                cmbAntPort.Enabled            = false;
                numPowerLevel.Enabled         = false;
                cmbBoxFreq.Enabled            = false;                 
                rdoBtnMultiChannel.Enabled    = false;
                rdoBtnSingleChannel.Enabled   = false;
                ckboxErrorKeepRunning.Enabled = false;
                this.ControlBox               = false;
                rdoBtnContinuous.Enabled = false;
                rdoBtnNonContinuous.Enabled = false;
                TemperatureBTN.Enabled = false;
                textBoxTemperature.Enabled = false;
                cmbBoxRegion.Enabled = false;


                //Start Pulse
                m_mainForm.StartMonitorPulse();   

                btnPulse.Text = "Pulse Off";
            }
            else
            {
                //Abort
                if (m_mainForm.ActiveControler != null)
                    //m_mainForm.ActiveControler.RequestAbort();//??modified for abort??//
                    m_mainForm.ActiveControler.RequestStop();/*Add by Rick for abort, 2013-01-29*/
            }

        }


        private void btnClear_Click(object sender, EventArgs e)
        {
            if (m_reader == null || m_mainForm == null)
                return;

            m_mainForm.ClearSessionData();            
            
            //avoid that can't exit the window
            this.ControlBox = true;
        }



        void CurrentContextChanged(object sender, EventArgs e)
        {

        }


        void FunctionStateChanged(object sender, EventArgs e)
        {
            SetButtonState();
        }


        //"Pulse" and "Inventory" use "Tag Access flow". So they can enter this event function.
        //"RF ON/OFF" doesn't use "Tag Access flow". It doesn't't enter this flow.
        private void SetButtonState()
        {
            try
            {
                if (this.InvokeRequired)
                {
                    Invoke(new MethodInvoker(SetButtonState));
                    return;
                }
            }
            catch (Exception e)
            {
                return;
            }

            FunctionControl.FunctionState newState =
                m_mainForm.ActiveControler == null ? FunctionControl.FunctionState.Unknown : m_mainForm.ActiveControler.State;

            //Completed inventory or pulse. Reset the dialog.
            if (newState == FunctionControl.FunctionState.Idle)
            {
                //If set "hopping config", only support "inventory" command.
                CheckChannel();

                //Enable Button
                btnInventory.Enabled = true;              
                btnClear.Enabled     = true;  

                btnInventory.Text = "Inventory On";
                btnPulse.Text     = "Pulse On";


                //Enable
                numPulseOnTime.Enabled        = true;
                numPulseOffTime.Enabled       = true;
                cmbAntPort.Enabled            = true;
                numPowerLevel.Enabled         = true;
                rdoBtnMultiChannel.Enabled    = true;
                rdoBtnSingleChannel.Enabled   = true;
                ckboxErrorKeepRunning.Enabled = true;
                this.ControlBox               = true;
                rdoBtnContinuous.Enabled = true;
                rdoBtnNonContinuous.Enabled = true;
                TemperatureBTN.Enabled = true;
                textBoxTemperature.Enabled = true;
                cmbBoxRegion.Enabled = true;
            }
        }


        //If set "hopping config", only support "inventory" command.
        private void MultiChannelRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            FunctionControl.FunctionState State =
                m_mainForm.ActiveControler == null ? FunctionControl.FunctionState.Unknown : m_mainForm.ActiveControler.State;


            //If start testing, can't change button.
            if ( State == FunctionControl.FunctionState.Idle &&
                 m_bInRfOn == false                               )
            {
                CheckChannel();
            }
        }


        //If set "single config", support full command.
        private void SingleChannelRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            FunctionControl.FunctionState State =
                m_mainForm.ActiveControler == null ? FunctionControl.FunctionState.Unknown : m_mainForm.ActiveControler.State;

            
            //If start testing, can't change button.
            if ( State == FunctionControl.FunctionState.Idle &&
                 m_bInRfOn == false                               )
            {
                CheckChannel();
            }
                       
        }


        //Save bErrorKeepRunning enable config
        private void ckboxErrorKeepRunning_CheckedChanged(object sender, EventArgs e)
        {
            m_mainForm.bErrorKeepRunning = ckboxErrorKeepRunning.Checked;
        }

//========================================================================================

        private void CheckChannel()
        {
   
            if (rdoBtnSingleChannel.Checked == true)
            {
                //If set "single config", support full command.
                btnRfOn.Enabled    = true;
                btnRfOff.Enabled   = true;
                btnRF.Enabled = true;//btnRF Combine
                btnPulse.Enabled   = true;
                cmbBoxFreq.Enabled = true;
            }
            else
            { 
                //If set "hopping config", only support "inventory" command.
                btnRfOn.Enabled    = false;
                btnRfOff.Enabled   = false;
                btnRF.Enabled = false;//btnRF Combine
                btnPulse.Enabled   = false;
                cmbBoxFreq.Enabled = false;            
            }
        }


        
        private void ShowErrMsg(String strMsg)
        { 
            MessageBox.Show( strMsg,
                     "RF Test",
                     MessageBoxButtons.OK, 
                     MessageBoxIcon.Error    );    
        }

        private void TemperatureBTN_Click(object sender, EventArgs e)
        {
            //uint data = uint.MaxValue;
            int data = int.MaxValue;
            //data = Convert.ToUInt16(textBoxTemperature.Text);
            
            rfid.Constants.Result result = Result.NOT_INITIALIZED;

            result = LakeChabotReader.MANAGED_ACCESS.API_ControlSetPowerState(0);
            if (result != rfid.Constants.Result.OK)
            {
                ShowErrMsg("Control Set Power State unsuccessfully!");
                return;
            }

            //LakeChabotReader.MANAGED_ACCESS.API_TestTurnCarrierWaveOff();
            result = LakeChabotReader.MANAGED_ACCESS.API_TestTurnCarrierWaveOff();
            if (result != rfid.Constants.Result.OK)
            {
                ShowErrMsg("Test Turn Carrier Wave Off unsuccessfully!");
                return;
            }
            
            //result = m_reader.MacReadRegister(0x0b0a, out data);
            result = m_reader.MacReadRegisterGetTempature(0x0b0a, out data);
            if (result != rfid.Constants.Result.OK)
            {
                ShowErrMsg("Get Temperature unsuccessfully!");
                return;
            }

            /*
            if (data < 0)
            {
                //textBoxTemperature.Text = "-"+Convert.ToString(data) + " ℃"; //not sure
                textBoxTemperature.Text = Convert.ToString(data+1) + " ℃";

            }
            else
            {
                textBoxTemperature.Text = Convert.ToString(data) + " ℃";
            }
            */

            textBoxTemperature.Text = Convert.ToString(data) + " ℃";
            //string val = Convert.ToString(data, 16);
            //int num = Convert.ToInt32(val, 16);

            //textBoxTemperature.Text = Convert.ToString(data)+" ℃"; 


                                    
            
        }        



    }
}
