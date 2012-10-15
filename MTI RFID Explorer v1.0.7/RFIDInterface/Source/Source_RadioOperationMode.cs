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
 * $Id: Source_RadioOperationMode.cs,v 1.3 2009/09/03 20:23:25 dshaheen Exp $
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

    public class Source_RadioOperationMode
        :
        Object
    {
        protected rfid.Constants.RadioOperationMode radioOperationMode;


        public Source_RadioOperationMode
        (
            
        )
            :
            this( rfid.Constants.RadioOperationMode.UNKNOWN )
        {
            // NOP
        }

        public Source_RadioOperationMode
        (
            rfid.Constants.RadioOperationMode radioOperationMode  
        )
            :
            base( )
        {
            this.radioOperationMode = radioOperationMode;
        }


        // Constructor ( Copy style )

        public Source_RadioOperationMode
        (
            Source_RadioOperationMode source
        )
            :
            base( )
        {
            this.Copy( source );
        }


        public void Copy( Source_RadioOperationMode from )
        {
            this.radioOperationMode = from.radioOperationMode;
        }


        public override bool Equals( System.Object obj )
        {
            if ( null == obj )
            {
                return false;
            }

            Source_AntennaResult rhs = obj as Source_AntennaResult;

            if ( null == ( System.Object ) rhs )
            {
                return false;
            }

            return this.Equals( rhs );
        }


        public bool Equals( Source_RadioOperationMode rhs )
        {
            if ( null == ( System.Object ) rhs )
            {
                return false;
            }

            return this.radioOperationMode == rhs.radioOperationMode;
        }


        // TODO: provide real hash return value

        public override int GetHashCode( )
        {
            return base.GetHashCode( );
        }



        public rfid.Constants.Result load
        (
            rfid.Linkage transport,
            UInt32                             readerHandle
        )
        {
            rfid.Constants.Result Result = transport.API_ConfigGetOperationMode(ref this.radioOperationMode);

            return Result;
        }


        public rfid.Constants.Result store
        (
            rfid.Linkage transport,
            UInt32                             readerHandle
        )
        {

            rfid.Constants.Result Result = transport.API_ConfigSetOperationMode(this.radioOperationMode);

            return Result;
        }



        public rfid.Constants.RadioOperationMode RadioOperationMode
        {
            get
            {
                return this.radioOperationMode;
            }
            set
            {
                this.radioOperationMode = value;
            }
        }



    } // End class Source_RadioOperationMode



} // End namespace RFID.RFIDInterface
