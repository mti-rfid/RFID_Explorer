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
 * $Id: ConfigureAlgorithmParms_1_Display.cs,v 1.5 2009/11/12 17:58:08 dciampi Exp $
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

    public partial class ConfigureAlgorithmParms_1_Display : UserControl
    {

        Boolean masterEnabled;


        public ConfigureAlgorithmParms_1_Display( )
        {
            InitializeComponent( );

            this.toggleTarget.Items.Add( "Disable" );
            this.toggleTarget.Items.Add( "Enable" );
            this.toggleTarget.MaxDropDownItems = 2;
        }


        public void setSource( Source_SingulationParametersDynamicQ parms )
        {
            this.startQValue.DataBindings.Clear( );
            this.startQValue.DataBindings.Add( "Value", parms, "StartQValue" );

            this.minQValue.DataBindings.Clear( );
            this.minQValue.DataBindings.Add( "Value", parms, "MinQValue" );

            this.maxQValue.DataBindings.Clear( );
            this.maxQValue.DataBindings.Add( "Value", parms, "MaxQValue" );

            this.retryCount.DataBindings.Clear( );
            this.retryCount.DataBindings.Add( "Value", parms, "RetryCount" );

            this.toggleTarget.DataBindings.Clear( );
            this.toggleTarget.DataBindings.Add( "SelectedIndex", parms, "ToggleTarget" );

            this.thresholdMultiplier.DataBindings.Clear( );
            this.thresholdMultiplier.DataBindings.Add( "Value", parms, "ThresholdMultiplier" );
        }



        public Boolean MasterEnabled
        {
            get
            {
                return this.masterEnabled;
            }

            set
            {
                this.masterEnabled = value;

                startQValue.Enabled  = this.MasterEnabled;
                minQValue.Enabled    = this.MasterEnabled;
                maxQValue.Enabled    = this.MasterEnabled;
                retryCount.Enabled   = this.MasterEnabled;                
                toggleTarget.Enabled = this.MasterEnabled;
                thresholdMultiplier.Enabled = this.MasterEnabled;

                // no refresh ~ parent will do...
            }
        }


        private void AlgorithmParms_1_Display_Load( object sender, EventArgs e )
        {
            // NOP - placeholder for now
        }

        private void startQValue_ValueChanged( object sender, EventArgs e )
        {
            // NOP - placeholder for now
        }

        private void minQValue_ValueChanged( object sender, EventArgs e )
        {
            // NOP - placeholder for now
        }

        private void maxQValue_ValueChanged( object sender, EventArgs e )
        {
            // NOP - placeholder for now
        }

        private void retryCount_ValueChanged( object sender, EventArgs e )
        {
            // NOP - placeholder for now
        }

        private void maxQueryReps_ValueChanged( object sender, EventArgs e )
        {
            // NOP - placeholder for now
        }

        private void toggleTarget_SelectedIndexChanged( object sender, EventArgs e )
        {
            // NOP - placeholder for now
        }


    }

}
