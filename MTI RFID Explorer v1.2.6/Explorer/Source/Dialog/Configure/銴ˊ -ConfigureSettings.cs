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
 * $Id: ConfigureSettings.cs,v 1.7 2010/01/07 02:10:57 dshaheen Exp $
 * 
 * Description: 
 *
 *****************************************************************************
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.IO;

using RFID.RFIDInterface;
using Global;


namespace RFID_Explorer
{

	public partial class ConfigureSettingsControl : UserControl
	{

        private bool initCall = true;
        private LakeChabotReader reader = null;
        private Timer _timer = null;


        private static Color OK_COLOR    = Color.Green;
        private static Color WARN_COLOR  = Color.Purple;
        private static Color ERROR_COLOR = Color.Red;


        private Panel   errorPanel   = new Panel( );
        private TextBox errorTextBox = new TextBox( );

        private UInt32                    RegionCount = 0;
        private Source_MacRegion          macRegion;
        private Source_LinkProfileList    profileList;

        private string strCustomerRegion = "";

        enum ENUM_REGION_RESULT
        { 
            OK,
            FAIL,
            NOT_MATCH,
            CUSTOMER_OK,
            CUSTOMER_FAIL,
            CUSTOMER_NON_SUPPORTED,
        }

        public ConfigureSettingsControl( LakeChabotReader reader )
        {
            if ( reader == null )
            {
                throw new ArgumentNullException( "reader", "Null reader passed to ConfigureGeneral CTOR()" );
            }

            if ( reader.Mode != rfidReader.OperationMode.BoundToReader )
            {
                throw new ArgumentOutOfRangeException( "reader", "Unbound reader passed to ConfigureGeneral()" );
            }

            InitializeComponent( );

            this.reader = reader;

            _timer           = new Timer( );
            _timer.Interval  = 5000;
            _timer.Tick     += new EventHandler( timer_Tick );

			string startupPowerState      = Properties.Settings.Default.startupPowerState;
			string startupOpMode          = Properties.Settings.Default.startupOperationalMode;
			int    startupAlgorithmNumber = Properties.Settings.Default.startupInventoryAlgorithm;

            rfid.Constants.Result result = rfid.Constants.Result.OK;


            //Interface radiobutton=============================================================
            UInt32 oemData = 0;
            rfid.Constants.Result status = reader.MacReadOemData( (ushort) enumOEM_ADDR.HOST_IF_SEL,
                                                                   ref     oemData                                );

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


            // regionComboBox=============================================================
            ENUM_REGION_RESULT enumMatch = ENUM_REGION_RESULT.FAIL;

            this.macRegion = new Source_MacRegion();            
            result = macRegion.load(LakeChabotReader.MANAGED_ACCESS, this.reader.ReaderHandle);


            do
            {
                if (rfid.Constants.Result.OK != result)
                {
                    enumMatch = ENUM_REGION_RESULT.FAIL;
                    break;
                }      
  

                UInt32 shift = 1;

                foreach (rfid.Constants.MacRegion item in Enum.GetValues(typeof(rfid.Constants.MacRegion)))
                {                   

                    do
                    { 
                        if ((this.macRegion.MacRegionSupport & shift) <= 0)
                            break;  
                
                        //Add support region to regionComboBox.                        
                        if (item == rfid.Constants.MacRegion.CUSTOMER)//Customer region uses string.
                        {                           
                            result = reader.API_MacGetCustomerRegion( ref strCustomerRegion );

                            switch (result)
                            {
                                case rfid.Constants.Result.OK:
                                    regionComboBox.Items.Add(strCustomerRegion);

                                    if (item == this.macRegion.MacRegion)
                                    {                 
                                        enumMatch = ENUM_REGION_RESULT.CUSTOMER_OK;
                                    }
                                    break;

                                case rfid.Constants.Result.NOT_SUPPORTED:
                                    //Only hide the option.
                                    if (item == this.macRegion.MacRegion)
                                    {
                                        enumMatch = ENUM_REGION_RESULT.CUSTOMER_NON_SUPPORTED;
                                    }
                                    break;

                                case rfid.Constants.Result.FAILURE:
                                default:
                                    enumMatch = ENUM_REGION_RESULT.CUSTOMER_FAIL;
                                    break;                               
                            }
                  
                        }
                        else
                        {
                            //Other region uses enum.
                            regionComboBox.Items.Add(item);

                            //Check match region between support and current region setting.
                            if (item == this.macRegion.MacRegion)
                            {
                                enumMatch = ENUM_REGION_RESULT.OK;
                            }
                        }
                    
                    }while(false);

                    shift <<= 0x01;
                }
           
            }while(false);

            switch (enumMatch)
            { 
                case ENUM_REGION_RESULT.OK:                                   
                    regionComboBox.SelectedIndex = regionComboBox.Items.IndexOf(this.macRegion.MacRegion);    
                    break;

                case ENUM_REGION_RESULT.FAIL:
                    RegionError("Read region unsuccessfully");
                    break;   

                case ENUM_REGION_RESULT.CUSTOMER_OK:
                    //Customer region uses string.                                   
                    regionComboBox.SelectedIndex = regionComboBox.Items.IndexOf(strCustomerRegion);    
                    break;

                case ENUM_REGION_RESULT.CUSTOMER_NON_SUPPORTED:
                    RegionError("Not support customer region");
                    break;

                case ENUM_REGION_RESULT.CUSTOMER_FAIL:
                    RegionError("Get customer region fail");
                    break;

                case ENUM_REGION_RESULT.NOT_MATCH:
                    RegionError("Region deosn't match \"RegionSupport\".");
                    break;          
            }                    
            


            // profileComboBox=============================================================
            this.profileList = new Source_LinkProfileList ( LakeChabotReader.MANAGED_ACCESS,
                                                            this.reader.ReaderHandle );

            this.profileList.load( );

            int count = 0;

            foreach ( Source_LinkProfile linkProfile in profileList )
            {
                profileComboBox.Items.Add( count + " : " + linkProfile.ToString( ) );

                ++ count;
            }

            profileComboBox.SelectedIndex = ( int ) profileList.getActiveProfileIndex( );

            this.profileComboBox.SelectedIndexChanged += new System.EventHandler( this.profileComboBox_SelectedIndexChanged );
            


            // Currently out of sync with 'new' model ~ no explicit read done
            // here or source provided ~ done via reader call...
            foreach ( rfid.Constants.SingulationAlgorithm item in Enum.GetValues( typeof( rfid.Constants.SingulationAlgorithm ) ) )
            {
                algorithmComboBox.Items.Add( item );
            }
            algorithmComboBox.Items.Remove(rfid.Constants.SingulationAlgorithm.UNKNOWN);

            // skipping err checking on these shortcut methods...

            Source_QueryParms queryParms = new Source_QueryParms( );

            queryParms.load( LakeChabotReader.MANAGED_ACCESS, reader.ReaderHandle );

            algorithmComboBox.SelectedIndex = algorithmComboBox.Items.IndexOf
                (
                    queryParms.SingulationAlgorithm
                );

            algorithmComboBox.SelectedIndexChanged += new System.EventHandler( this.algorithmComboBox_SelectedIndexChanged );

		}

        
        void RegionError( string strMsg )
        {
             this.macRegion.MacRegion = rfid.Constants.MacRegion.UNKNOWN;
            regionComboBox.Items.Add(this.macRegion.MacRegion);

            //Show error message
            RegionMatchLabel.Text    = strMsg;
            RegionMatchLabel.Visible = true;

            //Disable
            regionComboBox.Enabled = false;
            btn_SetRegion.Enabled  = false;

            regionComboBox.SelectedIndex = regionComboBox.Items.IndexOf(rfid.Constants.MacRegion.UNKNOWN);
        }


        void timer_Tick( object sender, EventArgs e )
        {
            _timer.Stop( );
            statusTextBox.Visible = false;
        }


        // Quick fix for now ~ sync between this 'quick' algorithm flipping
        // and display on main query parameters page...

        Boolean externalQueryParamsMod = false;

        protected override void OnLoad( EventArgs e )
        {
            base.OnLoad( e );
        }



        protected override void OnVisibleChanged( EventArgs e )
        {
            base.OnVisibleChanged( e );

            if ( this.Visible )
            {
                statusTextBox.Text = "";

                this.initCall = true;

                //Region
                if
                ( rfid.Constants.Result.OK != 
                  macRegion.load( LakeChabotReader.MANAGED_ACCESS, this.reader.ReaderHandle )
                )
                {
                    this.macRegion.MacRegion = rfid.Constants.MacRegion.UNKNOWN;
                }

                //clark 2011.4.26  Check range.
                if (macRegion.MacRegion > 0 && (uint)macRegion.MacRegion < RegionCount)
                {
                    regionComboBox.SelectedIndex = (int)this.macRegion.MacRegion;
                }

 
                this.initCall = false;


                //Query Parms
                Source_QueryParms queryParms = new Source_QueryParms( );

                queryParms.load( LakeChabotReader.MANAGED_ACCESS, reader.ReaderHandle );

                externalQueryParamsMod = true;

                algorithmComboBox.SelectedIndex = algorithmComboBox.Items.IndexOf
                (
                    queryParms.SingulationAlgorithm
                );

                externalQueryParamsMod = false;
            }
        }
		


		public LakeChabotReader Reader
		{
			get { return this.reader; }
		}




//============================================Event================================================
		private void algorithmComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
            if ( this.initCall || externalQueryParamsMod ) return;             
            
            // WARN: hard-coded to current max algo value at this time
            //       requires hand mod as new algos impld & exposed

			if( algorithmComboBox.SelectedIndex > 3 )
            {
                algorithmComboBox.SelectedIndex = 0;

                return;
            }

			statusTextBox.Text = "";
			statusTextBox.Visible = false;
			_timer.Stop();


            UInt32 reg_0x0901 = 0;
            UInt32 reg_0x0902 = 0;
  

            rfid.Constants.Result result = rfid.Constants.Result.OK;

            try
            {
                result = LakeChabotReader.MANAGED_ACCESS.API_ConfigReadRegister
                (
                    ( UInt16 ) 0x0901, ref reg_0x0901
                );

                if ( rfid.Constants.Result.OK != result )
                {
                    throw new Exception( result.ToString( ) );
                }

                // Set new algorithm preserving all bits other than algo ident

                reg_0x0901 &= 0xFFFFFFC0;
                reg_0x0901 |= ( UInt32 ) ( rfid.Constants.SingulationAlgorithm ) algorithmComboBox.SelectedItem;

                result = LakeChabotReader.MANAGED_ACCESS.API_ConfigWriteRegister
                (
                    ( UInt16 ) 0x0901, reg_0x0901
                );

                if ( rfid.Constants.Result.OK != result )
                {
                    throw new Exception( result.ToString( ) );
                }

                // Select proper bank for remaining registers... this is currently done
                // by the rfid lib each time a get is performed so here as safety net ...

                reg_0x0902 = ( UInt32 ) ( rfid.Constants.SingulationAlgorithm ) algorithmComboBox.SelectedItem;

                result = LakeChabotReader.MANAGED_ACCESS.API_ConfigWriteRegister
                (
                    ( UInt16 ) 0x0902, reg_0x0902
                );

                if ( rfid.Constants.Result.OK != result )
                {
                    throw new Exception( result.ToString( ) );
                }

                // Now we have to assure that when the query params tab is
                // shown ( becomes visible ) we ALWAYS force a load...

            }
            catch ( Exception )
            {
                // NOP - let fall thru to result check & msg display
            }

            if ( rfid.Constants.Result.OK == result )
            {
                this.statusTextBox.ForeColor = OK_COLOR;
                this.statusTextBox.Text = "Save Criteria Success";
                this.statusTextBox.Visible = true;

                this._timer.Start( );
            }
            else
            {
                this.statusTextBox.ForeColor = ERROR_COLOR;
                this.statusTextBox.Text = "Save Criteria Error: " + result;
                this.statusTextBox.Visible = true;
            }

            this.statusTextBox.Refresh( );
		}



 


		private void profileComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
            statusTextBox.Text = "";
            statusTextBox.Visible = false;
            _timer.Stop( );


            profileList.setActiveProfileIndex( profileComboBox.SelectedIndex );

            rfid.Constants.Result result = profileList.store( );

            if ( rfid.Constants.Result.OK == result )
            {
                statusTextBox.ForeColor = System.Drawing.Color.Green;
                statusTextBox.Text = "Link Profile Changed";
                statusTextBox.Visible = true;
                statusTextBox.Refresh( );
                _timer.Start( );
            }
            else
            {
                statusTextBox.ForeColor = Color.Red;
                statusTextBox.Text = "Link Profile Error : " + result.ToString( );
                statusTextBox.Visible = true;
                statusTextBox.Refresh( );
                _timer.Start( );
            }
		}




        private void ConfigureSettingsControl_Load( object sender, EventArgs e )
        {

        }



        //clark 2011.2.14 Copied from R1000 Tracer
        private void btn_Update_Click(object sender, EventArgs e)
        {

            if
            (
                 MessageBox.Show("If you change the setting, Explorer will be closed.\nAre you sure?",
                                 "Reader - Set communication port",
                                  MessageBoxButtons.YesNo,
                                  MessageBoxIcon.Question)

                                  ==

                 DialogResult.Yes
            )
            {

                UInt32 oemData = 0;
                oemData =
                    (rBtn_USB.Checked == true) ? (uint)enumPORT.ENUM_PORT_USB : (uint)enumPORT.ENUM_PORT_UART;

                rfid.Constants.Result status = reader.MacWriteOemData((ushort)enumOEM_ADDR.HOST_IF_SEL,
                                                                         oemData);

                if (rfid.Constants.Result.OK != status)
                {
                    MessageBox.Show( "Set communication port unsuccessfully",
                                     "Reader - Set communication port",
                                     MessageBoxButtons.OK,
                                     MessageBoxIcon.Error                     );
                    return;
                }



                if ( rfid.Constants.Result.OK != reader.API_ControlSoftReset() )
                {
                    MessageBox.Show( "Reset reader unsuccessfully",
                                     "Reader - Reset reader",
                                     MessageBoxButtons.OK,
                                     MessageBoxIcon.Error                     );
                    return;
                }   
 
                Application.Exit();              

            }//if
  
        }



        private void btn_SetRegion_Click(object sender, EventArgs e)
        {
            rfid.Constants.MacRegion SelectMacRegion = rfid.Constants.MacRegion.UNKNOWN;

            if (strCustomerRegion != null && regionComboBox.Text == strCustomerRegion)
                SelectMacRegion = rfid.Constants.MacRegion.CUSTOMER;
            else
            {
                if( Enum.IsDefined( typeof( rfid.Constants.MacRegion), regionComboBox.SelectedItem) )
                    SelectMacRegion = (rfid.Constants.MacRegion)regionComboBox.SelectedItem;
                else
                    SelectMacRegion = rfid.Constants.MacRegion.UNKNOWN;
            }



           //Check
           if
            (
                SelectMacRegion == this.macRegion.MacRegion   //Choose the same region
                ||
                SelectMacRegion == rfid.Constants.MacRegion.UNKNOWN   
            )
            {
                return;
            }


           //if
           // (
           //     (rfid.Constants.MacRegion)regionComboBox.SelectedItem ==
           //     rfid.Constants.MacRegion.UNKNOWN

           // )
           // {
           //     regionComboBox.SelectedIndex = regionComboBox.Items.IndexOf(this.macRegion.MacRegion);

           //     return;
           // }



           if
            (
                 MessageBox.Show("If you change the setting, Explorer will be closed.\nAre you sure?",
                                 "Reader - Set Region",
                                  MessageBoxButtons.YesNo,
                                  MessageBoxIcon.Question)

                                  ==

                 DialogResult.Yes
            )
            {

                this.macRegion.MacRegion = SelectMacRegion;

                rfid.Constants.Result result =
                    this.macRegion.store(LakeChabotReader.MANAGED_ACCESS, this.reader.ReaderHandle);

                if (rfid.Constants.Result.OK != result)
                {
                    MessageBox.Show( "Set region unsuccessfully",
                                     "Reader - Set region",
                                     MessageBoxButtons.OK,
                                     MessageBoxIcon.Error                     );
                    return;
                }


                if (rfid.Constants.Result.OK != reader.API_ControlSoftReset())
                {
                    MessageBox.Show( "Reset reader unsuccessfully",
                                     "Reader - Reset reader",
                                     MessageBoxButtons.OK,
                                     MessageBoxIcon.Error                     );
                    return;
                }    

                Application.Exit();              

            }//if

        }







    }

}


