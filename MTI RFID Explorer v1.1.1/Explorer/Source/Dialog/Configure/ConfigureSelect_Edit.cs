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
 * $Id: ConfigureSelect_Edit.cs,v 1.3 2009/09/03 20:23:18 dshaheen Exp $
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
    public partial class ConfigureSelect_Edit : Form
    {
        public ConfigureSelect_Edit( LakeChabotReader reader, SelectCriteria criteria )
        {
            InitializeComponent( );

            configureSelect_Display.setReader( reader );

            configureSelect_Display.setSource( criteria );

            configureSelect_Display.Mode = ConfigureSelect_Display.EDIT_MODE; // edit on
        }

    }
}