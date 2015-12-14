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
 * $Id: AntennaSenseThresholdEdit.cs,v 1.4 2010/01/07 02:10:57 dshaheen Exp $
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
using Global;


namespace RFID_Explorer
{

    public partial class AntennaSenseThresholdEdit : Form
    {
        private LakeChabotReader reader;

        private uint activeThresholdValue;


        public AntennaSenseThresholdEdit( LakeChabotReader reader, uint activeThresholdValue )
        {
            this.reader               = reader;
            this.activeThresholdValue = activeThresholdValue;

            InitializeComponent( );

            activeThreshold.Text    = activeThresholdValue.ToString( );
            activeThreshold.Enabled = false;

            newThreshold.Minimum = 0;
            newThreshold.Maximum = 0x000FFFFF;
            newThreshold.Value   = activeThresholdValue;
        }





        private void okButton_Click( object sender, EventArgs e )
        {
            if ( activeThresholdValue != newThreshold.Value )
            {
                rfid.Constants.Result status = rfid.Constants.Result.OK;

                try
                {
                    status = reader.API_AntennaPortSetSenseThreshold( (uint)newThreshold.Value );
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
                        "An error occurred while updating the antenna threshold value.\n\n" +
                        "The follow error occurred: " + status,
                        "Antenna Threshold Setting Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );

                    return;
                }
            }

            DialogResult = DialogResult.OK;
        }

        private void cancelButton_Click( object sender, EventArgs e )
        {
            DialogResult = DialogResult.Cancel;
        }



    } // End class AntennaSenseThresholdEdit


} // End namespace RFID_Explorer
