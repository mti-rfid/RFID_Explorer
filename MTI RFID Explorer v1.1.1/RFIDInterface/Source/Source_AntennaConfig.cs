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
 * $Id: Source_AntennaConfig.cs,v 1.4 2010/01/07 02:10:57 dshaheen Exp $
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

    public class Source_AntennaConfig
        :
        Object
    {
        protected byte                              port;
        protected rfid.Structures.AntennaPortConfig antennaPortConfig;


        public Source_AntennaConfig
        (
            byte port
        )
            :
            base( )
        {
            this.port              = port;
            this.antennaPortConfig = new rfid.Structures.AntennaPortConfig( );
        }


        public Source_AntennaConfig
        (
            Source_AntennaConfig source
        )
            :
            base( )
        {
            this.Copy( source );
        }


        public void Copy( Source_AntennaConfig from )
        {
            this.port                                    = from.port;

            this.antennaPortConfig.powerLevel            = from.antennaPortConfig.powerLevel;
            this.antennaPortConfig.dwellTime             = from.antennaPortConfig.dwellTime;
            this.antennaPortConfig.numberInventoryCycles = from.antennaPortConfig.numberInventoryCycles;
            this.antennaPortConfig.physicalPort          = from.antennaPortConfig.physicalPort;
            this.antennaPortConfig.antennaSenseThreshold = from.antennaPortConfig.antennaSenseThreshold;
        }


        public override bool Equals( System.Object obj )
        {
            if ( null == obj )
            {
                return false;
            }

            Source_AntennaConfig rhs = obj as Source_AntennaConfig;

            if ( null == ( System.Object ) rhs )
            {
                return false;
            }

            return this.Equals( rhs );
        }

        public bool Equals( Source_AntennaConfig rhs )
        {
            if ( null == ( System.Object ) rhs )
            {
                return false;
            }

            return
                   this.port                                    == rhs.port
                && this.antennaPortConfig.powerLevel            == rhs.antennaPortConfig.powerLevel
                && this.antennaPortConfig.dwellTime             == rhs.antennaPortConfig.dwellTime
                && this.antennaPortConfig.numberInventoryCycles == rhs.antennaPortConfig.numberInventoryCycles
                && this.antennaPortConfig.physicalPort          == rhs.antennaPortConfig.physicalPort
                && this.antennaPortConfig.antennaSenseThreshold == rhs.antennaPortConfig.antennaSenseThreshold;
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
            rfid.Constants.Result Result = transport.API_AntennaPortGetConfiguration
                (
                        port,
                    ref this.antennaPortConfig
                );

            if (Result != rfid.Constants.Result.OK)
            {
                Console.WriteLine("Error while retrieving global antenna sense threshold");
                return Result;
            }


            Result = transport.API_AntennaPortGetSenseThreshold
                (
                     ref this.antennaPortConfig.antennaSenseThreshold
                );

            return Result;
        }

        public rfid.Constants.Result store
        (
            rfid.Linkage transport,
            UInt32                             readerHandle
        )
        {
            rfid.Constants.Result Result = transport.API_AntennaPortSetSenseThreshold
            (
                this.antennaPortConfig.antennaSenseThreshold
            );

            if ( rfid.Constants.Result.OK != Result )
            {
                Console.WriteLine("Error while storing global antenna sense threshold");
            }

            Result = transport.API_AntennaPortSetConfiguration
                (
                    port,
                    this.antennaPortConfig
                );


            if ( rfid.Constants.Result.OK != Result )
            {
                //Console.WriteLine( "Error while storing AntennaPortConfig" );  //clark 2011.2.16 Copied from R1000 Tracer
                Console.WriteLine("Error while storing AntennaPortConfig port:" + port);
            }

            return Result;
        }



        public UInt32 Port
        {
            get
            {
                return this.port;
            }
        }

        public UInt16 PowerLevel
        {
            get
            {
                return this.antennaPortConfig.powerLevel;
            }
            set
            {
                this.antennaPortConfig.powerLevel = value;
            }
        }

        public UInt16 DwellTime
        {
            get
            {
                return this.antennaPortConfig.dwellTime;
            }
            set
            {
                this.antennaPortConfig.dwellTime = value;
            }
        }

        public UInt16 NumberInventoryCycles
        {
            get
            {
                return this.antennaPortConfig.numberInventoryCycles;
            }
            set
            {
                this.antennaPortConfig.numberInventoryCycles = value;
            }
        }

        // At this time Rx and Tx ports MUST be equal

        public byte PhysicalPort
        {
            get
            {
                return this.antennaPortConfig.physicalPort;
            }
            set
            {
                this.antennaPortConfig.physicalPort = value;
                this.antennaPortConfig.physicalPort = value;
            }
        }


        public UInt32 AntennaSenseThreshold
        {
            get
            {
                return this.antennaPortConfig.antennaSenseThreshold;
            }
            set
            {
                this.antennaPortConfig.antennaSenseThreshold = value;
            }
        }


    } // End class Source_AntennaConfig


} // End namespace RFID.RFIDInterface
