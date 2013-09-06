using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using Global;
using UpdateOEMCfgTool.Global;

namespace UpdateOEMCfgTool
{
   
    public partial class DeviceInterface : Form
    {
        private Interface m_clsInterface  = null;

        //Delegate        
        private CONTROL_ITEM m_dlgControlItem= null;


        public DeviceInterface( Interface r_clsInterface, CONTROL_ITEM r_dlgControlItem)
        {
            InitializeComponent();

            m_clsInterface   = r_clsInterface;
            m_dlgControlItem = r_dlgControlItem;
        }



        private void DeviceInterface_Load(object sender, EventArgs e)
        {
                
            do
            {                
                UInt32                uiModelNameMajor  = 0;
                string                strModule         = string.Empty;
                string                strModuleSub  = string.Empty;
                rfid.Constants.Result result            = rfid.Constants.Result.OK;
            
                //Get Model Name
                result  = m_clsInterface.API_MacReadOemData((ushort)((int)enumOEM_ADDR.MODEL_NAME_MAIN), ref uiModelNameMajor);
                //result = m_clsInterface.API_MacReadOemData((ushort)((int)enumOEM_ADDR.MODEL_NAME_SUB), ref uiModelNameMajor);

                if (rfid.Constants.Result.OK != result)
                {
                    btn_Update.Enabled = false;
                    break;
                }
                
                strModule= String.Format( "RU-{0}{1}{2}",
                                          (char)((uiModelNameMajor >> 16) & 0xFF),
                                          (char)((uiModelNameMajor >>  8) & 0xFF),
                                          (char)( uiModelNameMajor        & 0xFF)   );

                if (strModule == "RU-861")
                {
                    rBtn_USB.Checked = true;
                    rBtn_UART.Checked = true;
                    btn_Update.Enabled = true;
                    break;
                }

                if (strModule == "RU-824")
                { 
                    rBtn_USB.Checked   = true;
                    btn_Update.Enabled = false;                    
                    rBtn_UART.Enabled  = false;
                    break;
                }

                UInt32 oemData = 0;
                result = m_clsInterface.API_MacReadOemData( (ushort) enumOEM_ADDR.HOST_IF_SEL, ref oemData);
                if (rfid.Constants.Result.OK != result)
                {
                    btn_Update.Enabled = false;
                    break;
                }

                if (oemData == (uint)enumPORT.ENUM_PORT_USB)
                {
                    rBtn_USB.Checked = true;
                    rBtn_UART.Checked = false;
                }
                else
                {
                    rBtn_USB.Checked = false;
                    rBtn_UART.Checked = true;
                }


            }while(false);
        }



        private void btn_Update_Click(object sender, EventArgs e)
        {
            if( null == m_clsInterface )
            {
                 MessageBox.Show("Error: Interface class is null.",
                                 "Configuration - Error",
                                  MessageBoxButtons.OK,
                                  MessageBoxIcon.Error              );
                return;
            }



            if
            (
                 MessageBox.Show("If you change the setting, The device will be restarted.\nAre you sure?",
                                 "Configuration - Set communication port",
                                  MessageBoxButtons.YesNo,
                                  MessageBoxIcon.Question)

                                  ==

                 DialogResult.Yes
            )
            {

                UInt32 oemData = 0;
                oemData =
                    (rBtn_USB.Checked == true) ? (uint)enumPORT.ENUM_PORT_USB : (uint)enumPORT.ENUM_PORT_UART;

                rfid.Constants.Result status = m_clsInterface.API_MacWriteOemData((ushort)enumOEM_ADDR.HOST_IF_SEL,
                                                                         oemData);

                if (rfid.Constants.Result.OK != status)
                {
                    MessageBox.Show( "Set communication port unsuccessfully",
                                     "Configuration - Set communication port",
                                     MessageBoxButtons.OK,
                                     MessageBoxIcon.Error                     );
                    return;
                }



                if ( rfid.Constants.Result.OK != m_clsInterface.API_ControlSoftReset() )
                {
                    MessageBox.Show( "Reset reader unsuccessfully",
                                     "Configuration - Reset reader",
                                     MessageBoxButtons.OK,
                                     MessageBoxIcon.Error                     );
                    return;
                }   
 
            
                m_dlgControlItem(ENUM_ITEM_TYPE.FUNC_DISCONNECT, null);

                this.Close();            

            }//if 
        }


    }
}
