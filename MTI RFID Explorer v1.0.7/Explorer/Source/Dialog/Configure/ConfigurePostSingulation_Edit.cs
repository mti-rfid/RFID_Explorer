/*
 *****************************************************************************
 *                                                                           *
 *                 IMPINJ CONFIDENTIAL AND PROPRIETARY                       *
 *                                                                           *
 * This source code is the sole property of Impinj, Inc.  Reproduction or    *
 * utilization of this source code in whole or in part is forbidden without  *
 * the prior written consent of Impinj, Inc.                                 *
 *                                                                           *
 * (c) Copyright Impinj, Inc. 2009. All rights reserved.                     *
 *                                                                           *
 *****************************************************************************
 */

/*
 *****************************************************************************
 *
 * $Id: ConfigurePostSingulation_Edit.cs,v 1.3 2009/09/03 20:23:18 dshaheen Exp $
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
    public partial class ConfigurePostSingulation_Edit : Form
    {

        public ConfigurePostSingulation_Edit( LakeChabotReader reader, SingulationCriteria criteria )
        {
            InitializeComponent( );

            configurePostSingulation_Display.setReader( reader );

            configurePostSingulation_Display.setSource( criteria );

            configurePostSingulation_Display.Mode = ConfigurePostSingulation_Display.EDIT_MODE; // edit on
        }
    
    
    } // End class ConfigurePostSingulation_Edit


} // End namespace RFID_Explorer