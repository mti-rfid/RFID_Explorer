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
 * $Id: ConfigureAlgorithm_Edit.cs,v 1.3 2009/09/03 20:23:18 dshaheen Exp $
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
    public partial class ConfigureAlgorithm_Edit : Form
    {
        public ConfigureAlgorithm_Edit( LakeChabotReader reader, Source_QueryParms parms )
        {
            InitializeComponent( );

            // The algorithmDisplay is the obj that needs the zero arg constructor
            // so it works via the gui builder in VS... and so needs secondary
            // calls to set the reader and parms...

            algorithmDisplay.setReader( reader );
            algorithmDisplay.setSource( parms );
            algorithmDisplay.MasterEnabled = true; // edit on

            algorithmDisplay.displayData( );
        }
    }
}