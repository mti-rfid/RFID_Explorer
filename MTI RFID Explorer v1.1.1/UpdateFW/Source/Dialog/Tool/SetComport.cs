using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;


namespace UpdateFWTool
{
    //clark 2011.05.20 Let user to set comport in disconnected status.
    public partial class SetComport : Form
    {
        Interface m_clsInterface = null;

        public SetComport(Interface r_clsInterface)
        {
            InitializeComponent();

            m_clsInterface = r_clsInterface;

            //com port selection
            numComNum.Value = m_clsInterface.uiLibSettingComPort;
        }



        private void btn_Update_Click(object sender, EventArgs e)
        {

            if (null == m_clsInterface)
            {
                MessageBox.Show("Error: Interface class is null.",
                                "Configuration - Error",
                                 MessageBoxButtons.OK,
                                 MessageBoxIcon.Error               );

                return;
            }


            //com port
            UInt32 portNum = m_clsInterface.uiLibSettingComPort;

            try
            {
                portNum = Convert.ToUInt32( numComNum.Value );

            }
            catch (Exception exception)
            {
                MessageBox.Show( "Please enter number",
                                 "Invalid number",
                                 MessageBoxButtons.OK, 
                                 MessageBoxIcon.Exclamation );
                return;
            }

                     
             m_clsInterface.uiLibSettingComPort = portNum;

        }

    }//class


}
