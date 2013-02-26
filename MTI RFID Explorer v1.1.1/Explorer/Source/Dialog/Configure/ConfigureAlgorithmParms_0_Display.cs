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
 * $Id: ConfigureAlgorithmParms_0_Display.cs,v 1.3 2009/09/03 20:23:18 dshaheen Exp $
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

using RFID.RFIDInterface;

using rfid.Structures;
using rfid.Constants;

using System.Collections;



namespace RFID_Explorer
{

    public partial class ConfigureAlgorithmParms_0_Display : UserControl
    {

        Boolean masterEnabled;
        

        public ConfigureAlgorithmParms_0_Display( )
        {
            InitializeComponent( );

            this.toggleTarget.Items.Add( "Disable" );
            this.toggleTarget.Items.Add( "Enable" );
            this.toggleTarget.MaxDropDownItems = 2;

            this.repeatUntilNoTags.Items.Add( "Disable" );
            this.repeatUntilNoTags.Items.Add( "Enable" );
            this.repeatUntilNoTags.MaxDropDownItems = 2;
        }


        public void setSource( Source_SingulationParametersFixedQ parms )
        {
            this.qValue.DataBindings.Clear( );
            this.qValue.DataBindings.Add( "Value", parms, "QValue" );

            this.retryCount.DataBindings.Clear( );
            this.retryCount.DataBindings.Add( "Value", parms, "RetryCount" );

            this.toggleTarget.DataBindings.Clear( );
            this.toggleTarget.DataBindings.Add( "SelectedIndex", parms, "ToggleTarget" );

            this.repeatUntilNoTags.DataBindings.Clear( );
            this.repeatUntilNoTags.DataBindings.Add( "SelectedIndex", parms, "RepeatUntilNoTags" );
        }



        public Boolean MasterEnabled
        {
            get
            {
                return this.masterEnabled;
            }

            set
            {
                this.masterEnabled        = value;

                qValue.Enabled            = this.MasterEnabled;
                retryCount.Enabled        = this.MasterEnabled;
                toggleTarget.Enabled      = this.MasterEnabled;
                repeatUntilNoTags.Enabled = this.MasterEnabled;

                // no refresh ~ parent will do...
            }
        }


        private void AlgorithmParms_0_Display_Load( object sender, EventArgs e )
        {
            // NOP - placeholder for now
        }

        private void qValue_ValueChanged( object sender, EventArgs e )
        {
            // NOP - placeholder for now
        }

        private void retryCount_ValueChanged( object sender, EventArgs e )
        {
            // NOP - placeholder for now
        }

        private void toggleTarget_SelectedIndexChanged( object sender, EventArgs e )
        {
            // NOP - placeholder for now
        }

        private void repeatUntilNoTags_SelectedIndexChanged( object sender, EventArgs e )
        {
            // NOP - placeholder for now
        }


    }


}
