using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

using RFID.RFIDInterface;

namespace RFID_Explorer
{
    //clark 2011.05.20 Let user to set comport in disconnected status.
    public partial class SetComport : Form
    {
        public SetComport()
        {
            InitializeComponent();

            //com port selection
            numComNum.Value = LakeChabotReader.uiLibSettingComPort;

            //Tip
            ToolTip toolTip = new ToolTip();

            // Set up the delays for the ToolTip.
            toolTip.AutoPopDelay = 5000;
            toolTip.InitialDelay = 500;
            toolTip.ReshowDelay  = 500;

            // Force the ToolTip text to be displayed whether or not the form is active.
            toolTip.ShowAlways = true;

            // Set up the ToolTip text for the Button.
            toolTip.SetToolTip(this.btn_Set, "Note!! If pressed Update button to set COM port, the application will be closed.");
        }



        private void btn_Update_Click(object sender, EventArgs e)
        {
            //com port
            UInt32 portNum = LakeChabotReader.uiLibSettingComPort;

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

                     
            if 
            (
                MessageBox.Show("If you change the setting, Explorer will be closed.\nAre you sure?",
                                "Reader - Reset",
                                 MessageBoxButtons.YesNo, 
                                 MessageBoxIcon.Question) 
                                 
                                 == 
                                 
                DialogResult.Yes
            )
            {
                LakeChabotReader.uiLibSettingComPort = portNum;

                // Force Application Close
                Application.Exit();
            }
        }

    }//class


}
