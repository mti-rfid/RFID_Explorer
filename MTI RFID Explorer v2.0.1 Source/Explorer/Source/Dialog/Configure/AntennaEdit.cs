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
 * $Id: AntennaEdit.cs,v 1.6 2009/12/03 19:45:46 dciampi Exp $
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

	public partial class AntennaEditForm : Form
	{		        

		private LakeChabotReader  reader;

		private Source_Antenna	  antennaMaster;
        private Source_Antenna    antennaActive;


        public AntennaEditForm( LakeChabotReader reader, Source_Antenna antenna )
        {
            this.reader     = reader;

            this.antennaMaster = antenna;
            this.antennaActive = new Source_Antenna( antenna );

            InitializeComponent( );


            //Logic Port
            antennaNumberLabel.Text = String.Format( "Antenna {0}", antennaActive.Port );

            //Status
            foreach (rfid.Constants.AntennaPortState item in Enum.GetValues( typeof( rfid.Constants.AntennaPortState ) ))
            {
                state.Items.Add(item);
            }
            state.Items.Remove(rfid.Constants.AntennaPortState.UNKNOWN);

			state.SelectedIndex = 
                antennaActive.State == rfid.Constants.AntennaPortState.DISABLED
                ? 0 : 1;

            state.SelectedValueChanged += state_SelectedValueChanged;

            //Mod by FJ for change caption of "Antenna Ports" and "GPIO" GUI for HP SiP, 2015-01-22
            //clark. Set the limit of port number  Aotomatically
            //Mod by FJ for change the error message appear, 2015-01-30
			//rfid.Constants.Result m_result = rfid.Constants.Result.OK;
            if (rfid.Constants.Result.OK != reader.result_major)
            {
                throw new Exception(reader.result_major.ToString());
                //throw new Exception(result.ToString());
            //End by FJ for change the error message appear, 2015-01-30
            }
            //Mod by FJ for revert physical port display in M03 module, 2016-11-03
            //Mod by FJ for antenna port only set one port in M06 module, 2016-10-28
            if (reader.uiModelNameMAJOR == 0x4D303658)//0x4D303658 = M06X
            {
                PhysicalPort.Minimum = Source_Antenna.PHY_MINIMUM;
                PhysicalPort.Maximum = Source_Antenna.PHY_MINIMUM;
            }
            //if (reader.uiModelNameMAJOR == 0x4D303358)//0x4D303358==M03X
            //{
            //    PhysicalPort.Minimum = Source_Antenna.PHY_MINIMUM + 1;
            //    PhysicalPort.Maximum = Source_Antenna.PHY_MAXIMUM + 1;
            //}
            //else if (reader.uiModelNameMAJOR == 0x4D303658)//0x4D303658 = M06X
            //{
            //    PhysicalPort.Minimum = Source_Antenna.PHY_MINIMUM;
            //    PhysicalPort.Maximum = Source_Antenna.PHY_MINIMUM;
            //}
            else
            {
                PhysicalPort.Minimum = Source_Antenna.PHY_MINIMUM;
                PhysicalPort.Maximum = Source_Antenna.PHY_MAXIMUM;
            }
            /*
            if (reader.uiModelNameMAJOR != 0x4D303358)//0x4D303358==M03X
            {
                PhysicalPort.Minimum = Source_Antenna.PHY_MINIMUM;
                PhysicalPort.Maximum = Source_Antenna.PHY_MAXIMUM;
            }
            else {
                PhysicalPort.Minimum = Source_Antenna.PHY_MINIMUM + 1;
                PhysicalPort.Maximum = Source_Antenna.PHY_MAXIMUM + 1;
            }
            */
            //End by FJ for antenna port only set one port in M06 module, 2016-10-28
            //End by FJ for revert physical port display in M03 module, 2016-11-03
            PhysicalPort.DataBindings.Add("Value", this.antennaActive, "PhysicalPort");

            /*
            //clark. Set the limit of port number  Aotomatically
            PhysicalPort.Minimum = Source_Antenna.PHY_MINIMUM;
            PhysicalPort.Maximum = Source_Antenna.PHY_MAXIMUM;
            PhysicalPort.DataBindings.Add( "Value", this.antennaActive, "PhysicalPort" );
            */
            //End by FJ for change caption of "Antenna Ports" and "GPIO" GUI for HP SiP, 2015-01-22
        
            dwellTime.Minimum = 0;
            dwellTime.Maximum = 1000000;
            dwellTime.DataBindings.Add( "Value", this.antennaActive, "DwellTime" );

            inventoryCycles.Minimum = 0;
            inventoryCycles.Maximum = 1000000;
            inventoryCycles.DataBindings.Add( "Value", this.antennaActive, "NumberInventoryCycles" );


            //Clark 2011.2.21 Cpoied from R1000 Tracer
            //Mod by FJ for power level set 0~30dbm in M06 module, 2016-10-28
            if (reader.uiModelNameMAJOR == 0x4D303658)//0x4D303658 = M06X
            {
                powerLevel.Minimum = Source_Antenna.POWER_MINIMUM;
                powerLevel.Maximum = 300;
            }
            else
            {
                powerLevel.Minimum = Source_Antenna.POWER_MINIMUM;
                powerLevel.Maximum = Source_Antenna.POWER_MAXIMUM;
            }
            //powerLevel.Minimum = Source_Antenna.POWER_MINIMUM;
            //powerLevel.Maximum = Source_Antenna.POWER_MAXIMUM;
            powerLevel.DataBindings.Add( "Value", this.antennaActive, "PowerLevel" );
            //End by FJ for power level set 0~30dbm in M06 module, 2016-10-28
        }


		private void okButton_Click(object sender, EventArgs e)
		{
            if ( ! this.antennaActive.Equals( this.antennaMaster ) )
            {
                rfid.Constants.Result status =
                    rfid.Constants.Result.OK;

                //20090410 MTI Set to 0.1 dB per step   Clark copied from R1000 Tracer
                //this.antennaActive.PowerLevel = (uint)powerLevel.Value * 10;
                this.antennaActive.PowerLevel = (UInt16)powerLevel.Value;


			    try
			    {
                    status = 
                        this.antennaActive.store
                        ( 
                            LakeChabotReader.MANAGED_ACCESS,
                            reader.ReaderHandle
                        );
			    }
			    catch ( Exception )
			    {
                    status = rfid.Constants.Result.RADIO_FAILURE;
			    }

                if ( rfid.Constants.Result.OK != status )
                {
                    MessageBox.Show
                    (
                        "Reader Error.\n\n" + 
                        "An error occurred while updating the antenna settings.\n\n" +
                        "The follow error occurred: " + status,
                        "Antenna Settings Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );

                    return;
                }
                else
                {
                    // Sync master copy so initial display shows correct data

                    this.antennaMaster.Copy( this.antennaActive );
                }

            }

			DialogResult = DialogResult.OK;
		}


        private void cancelButton_Click( object sender, EventArgs e )
        {
            DialogResult = DialogResult.Cancel;
        }


        private void state_SelectedValueChanged( object sender, EventArgs e )
        {
            this.antennaActive.State = 
                ( rfid.Constants.AntennaPortState ) state.SelectedIndex;
        }

        private void PhysicalPort_ValueChanged( object sender, EventArgs e )
        {
            PhysicalPort.Validate( );

            //clark not sure
            //PhysicalPort.Text = ( ( UInt32 ) PhysicalPort.Value ).ToString( );
            //PhysicalPort.Refresh( );
        }



    } // END partial class AntennaEditForm



} // END namespace RFID_Explorer
