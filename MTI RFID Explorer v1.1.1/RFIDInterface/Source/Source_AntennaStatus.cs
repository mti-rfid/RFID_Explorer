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
 * $Id: Source_AntennaStatus.cs,v 1.3 2009/09/03 20:23:24 dshaheen Exp $
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

    public class Source_AntennaResult
        :
        Object
    {
        protected byte                              port;
        protected rfid.Structures.AntennaPortStatus antennaPortStatus;


        // Constructor ~ empty except for transport & handle
        // designed to init for loading from radio

        public Source_AntennaResult
        (
            byte port
        )
            :
            base( )
        {
            this.port              = port;
            this.antennaPortStatus = new rfid.Structures.AntennaPortStatus( );
        }


        // Constructor ( Copy style )

        public Source_AntennaResult
        (
            Source_AntennaResult source
        )
            :
            base( )
        {
            this.Copy( source );
        }
        

        public void Copy( Source_AntennaResult from )
        {
            this.port                                = from.Port;
            this.antennaPortStatus.state             = from.State;
            this.antennaPortStatus.antennaSenseValue = from.AntennaSenseValue;
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


        public bool Equals( Source_AntennaResult rhs )
        {
            if ( null == ( System.Object ) rhs )
            {
                return false;
            }

            return
                   this.port                                == rhs.port
                && this.antennaPortStatus.state             == rhs.antennaPortStatus.state
                && this.antennaPortStatus.antennaSenseValue == rhs.antennaPortStatus.antennaSenseValue;
        }


        // TODO: provide real hash return value

        public override int GetHashCode( )
        {
            return base.GetHashCode( );
        }



        public rfid.Constants.Result load
        (
            rfid.Linkage transport,
            UInt32       readerHandle
        )
        {
            rfid.Constants.Result Result = transport.API_AntennaPortGetState( this.port,
                                                                              ref this.antennaPortStatus );

            return Result;
        }


        public rfid.Constants.Result store
        (
            rfid.Linkage transport,
            UInt32 readerHandle
        )
        {
            // There is no save for the antennaPortStatus in lower library but
            // there is a set state so use that here!

            rfid.Constants.Result Result = transport.API_AntennaPortSetState(this.port,
                                                                             this.antennaPortStatus.state );

            if ( rfid.Constants.Result.OK != Result )
            {
                Console.WriteLine( "Error while saving antennaPortStatus.state" );
            }

            return Result;
        }



        public byte Port
        {
            get
            {
                return this.port;
            }
        }

        public rfid.Constants.AntennaPortState State
        {
            get
            {
                return this.antennaPortStatus.state;
            }
            set
            {
                this.antennaPortStatus.state = value;
            }
        }

        public UInt32 AntennaSenseValue
        {
            get
            {
                return this.antennaPortStatus.antennaSenseValue;
            }
        }


    } // End class Source_AntennaStatus


} // End namespace RFID.RFIDInterface
