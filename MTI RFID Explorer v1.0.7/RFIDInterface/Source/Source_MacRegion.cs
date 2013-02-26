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
 * $Id: Source_MacRegion.cs,v 1.4 2009/11/25 05:36:39 dciampi Exp $
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

    public class Source_MacRegion
        :
        Object
    {
        protected rfid.Constants.MacRegion macRegion        = rfid.Constants.MacRegion.UNKNOWN;
        protected UInt32                   macRegionSupport = 0;

        public Source_MacRegion
        (

        )
            :
            this( rfid.Constants.MacRegion.UNKNOWN )
        {
            // NOP
        }

        public Source_MacRegion
        (
            rfid.Constants.MacRegion r_macRegion
        )
            :
            base()
        {
            this.macRegion = r_macRegion;
        }


        // Constructor ( Copy style )

        public Source_MacRegion
        (
            Source_MacRegion source
        )
            :
            base()
        {
            this.Copy(source);
        }


        public void Copy(Source_MacRegion from)
        {
            this.macRegion = from.macRegion;
        }


        public override bool Equals(System.Object obj)
        {
            if (null == obj)
            {
                return false;
            }

            Source_AntennaResult rhs = obj as Source_AntennaResult;

            if (null == (System.Object)rhs)
            {
                return false;
            }

            return this.Equals(rhs);
        }


        public bool Equals(Source_MacRegion rhs)
        {
            if (null == (System.Object)rhs)
            {
                return false;
            }

            return this.macRegion == rhs.macRegion;
        }


        // TODO: provide real hash return value

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }



        public rfid.Constants.Result load
        (
            rfid.Linkage transport,
            UInt32       readerHandle
        )
        {
            return transport.API_MacGetRegion(ref this.macRegion, ref this.macRegionSupport);
        }


        public rfid.Constants.Result store
        (
            rfid.Linkage transport,
            UInt32       readerHandle
        )
        {

            return transport.API_MacSetRegion(this.macRegion);
        }



        public rfid.Constants.MacRegion MacRegion
        {
            get
            {
                return this.macRegion;
            }
            set
            {
                this.macRegion = value;
            }
        }


        public UInt32 MacRegionSupport
        {
            get
            {
                return this.macRegionSupport;
            }
            set
            {
                this.macRegionSupport = value;
            }
        }


    } // End class Source_MacRegion



} // End namespace RFID.RFIDInterface
