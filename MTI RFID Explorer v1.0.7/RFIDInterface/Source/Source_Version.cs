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
 * $Id: Source_Version.cs,v 1.4 2009/11/09 19:14:10 dshaheen Exp $
 * 
 * Description: 
 *
 *****************************************************************************
 */



using System;
using System.Collections.Generic;
using System.Text;



namespace RFID.RFIDInterface
{
    class Source_Version
    {
        protected rfid.Structures.Version version;


        public Source_Version
        (
            rfid.Structures.Version version
        )
        {
            // Currently just reference copy ~ change to deep 
            // copy later or ?

            this.version = version;
        }

        public UInt32 Major
        {
            get
            {
                return this.version.major;
            }
        }

        public UInt32 Minor
        {
            get
            {
                return this.version.minor;
            }
        }


        public UInt32 Release
        {
            get
            {
                return this.version.release;
            }
        }
    } // End class Source_Version


} // End namespace RFID.RFIDInterface
