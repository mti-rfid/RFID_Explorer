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
 * $Id: RFChannelEdit.cs,v 1.4 2009/12/08 08:08:07 dciampi Exp $
 * 
 * Description: 
 *
 *****************************************************************************
 */



using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using RFID.RFIDInterface;



namespace RFID_Explorer
{

	public partial class RFChannelEditForm : Form
	{


		private LakeChabotReader     reader;

		private Source_FrequencyBand channelMaster;
        private Source_FrequencyBand channelActive;


        public RFChannelEditForm( LakeChabotReader reader, Source_FrequencyBand channel )
        {
            this.reader        = reader;

            this.channelMaster = channel;


            this.channelActive = new Source_FrequencyBand( channel );

            InitializeComponent( );

            channelSlotNumber.Text = String.Format( "RF Channel Slot {0}", this.channelActive.Band );

            foreach (Source_FrequencyBand.BandState item in Enum.GetValues(typeof(Source_FrequencyBand.BandState)))
            {
                this.state.Items.Add(item);
            }
            this.state.Items.Remove(Source_FrequencyBand.BandState.UNKNOWN);

            state.SelectedIndex =
                channelActive.State == Source_FrequencyBand.BandState.DISABLED
                ? 0 : 1;

            state.SelectedValueChanged += state_SelectedValueChanged;
            
            this.multiplyRatio.DataBindings.Add( "Value", this.channelActive, "MultiplyRatio" );
            this.divideRatio.DataBindings.Add( "Value", this.channelActive, "DivideRatio" );
            this.minimumDAC.DataBindings.Add( "Value", this.channelActive, "MinimumDACBand" );
            this.affinityBand.DataBindings.Add( "Value", this.channelActive, "AffinityBand" );
            this.maximumDAC.DataBindings.Add( "Value", this.channelActive, "MaximumDACBand" );
            this.guardBand.DataBindings.Add( "Value", this.channelActive, "GuardBand" );
        }



        private void state_SelectedValueChanged( object sender, EventArgs e )
        {
            this.channelActive.State = ( Source_FrequencyBand.BandState ) state.SelectedIndex;
            this.updateControls( );
        }

        private void updateControls( )
        {
            if ( this.state.SelectedIndex == 1 ) // Enable
            {
                this.multiplyRatio.Value   = this.channelActive.MultiplyRatio;
                this.multiplyRatio.Enabled = true;

                this.divideRatio.Value     = this.channelActive.DivideRatio;
                this.divideRatio.Enabled   = true;

                this.minimumDAC.Value      = this.channelActive.MinimumDACBand;
                this.minimumDAC.Enabled    = true;

                this.maximumDAC.Value      = this.channelActive.MaximumDACBand;
                this.maximumDAC.Enabled    = true;

                this.affinityBand.Value    = this.channelActive.AffinityBand;
                this.affinityBand.Enabled  = true;

                this.guardBand.Value       = this.channelActive.GuardBand;
                this.guardBand.Enabled     = true;

            }
            else
            {
                this.multiplyRatio.Enabled = false;
                this.divideRatio.Enabled   = false;
                this.minimumDAC.Enabled    = false;
                this.affinityBand.Enabled  = false;
                this.maximumDAC.Enabled    = false;
                this.guardBand.Enabled     = false;
            }
		}

        private void mulitplyFactor_ValueChanged( object sender, EventArgs e )
        {
            this.channelActive.MultiplyRatio = ( UInt16 ) this.multiplyRatio.Value;

            this.frequency.Text = String.Format( "{0:F2} MHz", this.channelActive.Frequency );
            this.frequency.Refresh( );
        }

        private void divideRatio_ValueChanged( object sender, EventArgs e )
        {
            this.channelActive.DivideRatio = ( UInt16 ) this.divideRatio.Value;

            this.frequency.Text = String.Format( "{0:F2} MHz", this.channelActive.Frequency );
            this.frequency.Refresh( );
        }


        private void minimumDAC_ValueChanged( object sender, EventArgs e )
        {
            this.affinityBand.Minimum = this.minimumDAC.Value;
            this.maximumDAC.Minimum   = this.minimumDAC.Value;
        }

        private void affinityBand_ValueChanged( object sender, EventArgs e )
        {
            // NOP at this time
        }

        private void maximumDACBand_ValueChanged( object sender, EventArgs e )
        {
            this.affinityBand.Maximum = this.maximumDAC.Value;
            this.minimumDAC.Maximum   = this.maximumDAC.Value;
        }

        private void guardBand_ValueChanged( object sender, EventArgs e )
        {
            // NOP at this time
        }



        private void okButton_Click( object sender, EventArgs e )
        {
            rfid.Constants.Result result =
                rfid.Constants.Result.OK;

            try
            {
                result = this.channelActive.store( LakeChabotReader.MANAGED_ACCESS, this.reader.ReaderHandle );
            }
            catch ( Exception exp )
            {
                MessageBox.Show( "Reader Error.\n\nAn error occurred while updating the frequency channel settings.\n\nThe follow error occurred: " + exp.Message, "RF Frequency Band Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
                return;
            }

            if ( rfid.Constants.Result.OK != result )
            {
                MessageBox.Show( "Reader Error.\n\nThe Reader was unable to configure the specified frequency channel settings.\n\nThe follow error occurred: " + result.ToString( ), "RF Frequency Band Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
                return;
            }

            // Need to retrieve the affinityBand value since potentially
            // modified by the radio during set operation(s)

            Source_FrequencyBand channelUpdated = new Source_FrequencyBand( channelActive.Band );

            try
            {
                 result = channelUpdated.load( LakeChabotReader.MANAGED_ACCESS, this.reader.ReaderHandle );
            }
            catch ( Exception exp )
            {
                MessageBox.Show( "Reader Error.\n\nAn error occurred while retrieving updated frequency channel information.\n\nThe follow error occurred: " + exp.Message, "RF Frequency Band Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
                return;
            }

            if ( rfid.Constants.Result.OK != result )
            {
                MessageBox.Show( "Reader Error.\n\nThe Reader was unable to configure the specified frequency channel settings.\n\nThe follow error occurred: " + result.ToString( ), "RF Frequency Band Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
                return;
            }

            this.channelMaster.Copy( this.channelActive );

            DialogResult = DialogResult.OK;
        }


    } // END partial class RFChannelEditForm


} // END namespace RFID_Explorer