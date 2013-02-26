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
 * $Id: Source_AntennaList.cs,v 1.6 2009/12/03 03:54:30 dciampi Exp $
 * 
 * Description: 
 *
 *****************************************************************************
 */



using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;



namespace RFID.RFIDInterface
{

    [System.ComponentModel.TypeConverter( typeof( Source_AntennaList_TypeConverter ) )]
    public class Source_AntennaList
        :
        List< Source_Antenna >
    {
        //clark 2011.2.10 Copied from R100 Tracer
        public static Source_AntennaList DEFAULT_ANTENNA_LIST
        {
            get
            {
                Object obj = TypeDescriptor.GetConverter(typeof(Source_AntennaList)).ConvertFromString
                (
                    RFID.RFIDInterface.Properties.Settings.Default.DefaultAntennaSettings
                );

                if (null == obj)
                {
                    // SHOULD NEVER OCCUR
                }

                return obj as Source_AntennaList;
            }
        }

        // Create an empty antenna list

        public Source_AntennaList( )
            :
            base( )
        {
            // NOP
        }


        // Create an empty antenna list with initial capacity

        public Source_AntennaList( Int32 capacity )
            :
            base( capacity )
        {
            // NOP
        }


        // Copy an antenna list ~ no checks for dup ports

        public Source_AntennaList( IEnumerable< Source_Antenna > enumerable )
            :
            base( enumerable )
        {
            // NOP
        }


        // Copy an antenna list ~ performing a DEEP copy of
        // antennas if indicated

        public Source_AntennaList( IEnumerable<Source_Antenna> enumerable, Boolean deepCopy )
            :
            base( )
        {
            if ( ! deepCopy )
            {
                this.AddRange( enumerable );
            }
            else
            {
                this.Copy( enumerable );
            }
        }



        public void Copy( IEnumerable< Source_Antenna > from )
        {
            this.Clear( );

            foreach ( Source_Antenna antenna in from )
            {
                this.Add( new Source_Antenna( antenna ) );
            }
        }



        // Attempt to load info for all known antennas

        public rfid.Constants.Result load
        (
            rfid.Linkage transport,
            UInt32       readerHandle
        )
        {
            this.Clear( );

            for (byte port = 0; port < RFID.RFIDInterface.Properties.Settings.Default.MaxVirtualAntennas; port++)
            {
                Source_Antenna antenna = new Source_Antenna( port );

                rfid.Constants.Result Result = antenna.load( transport, readerHandle );

                if ( rfid.Constants.Result.OK == Result )
                {
                    this.Add( antenna );
                }
                else if ( rfid.Constants.Result.INVALID_PARAMETER == Result )
                {
                    break; // this rcv when portIndex > logical antenna count on radio
                }
                else
                {
                    Console.WriteLine( "Error while reading antenna information" );

                    return Result; // this rcv all other errors
                }
            }

            return rfid.Constants.Result.OK;
        }


        // Attempt to save all link profiles currently on the radio
        // and mark the active one.

        public rfid.Constants.Result store
        (
            rfid.Linkage transport,
            UInt32                             readerHandle
        )
        {
            foreach( Source_Antenna antenna in this )
            {
                rfid.Constants.Result Result = antenna.store( transport, readerHandle );

                if ( rfid.Constants.Result.OK != Result )
                {
                    return Result;
                }
            }

            return rfid.Constants.Result.OK;
        }


        // Attempt to locate antenna with a matching
        // logical port

        public Source_Antenna FindByPort( UInt32 port )
        {
            Source_Antenna result = this.Find
                (
                    delegate( Source_Antenna antenna )
                    {
                        return antenna.Port == port;
                    }
                );

            return result;
        }




        // Attempt to locate the first antenna with
        // matching physical rx port

        public Source_Antenna FindByPhysicalPort(UInt32 port)
        {
            Source_Antenna result = this.Find
                (
                    delegate( Source_Antenna antenna )
                    {
                        return antenna.PhysicalPort == port;
                    }
                );

            return result;
        }


    } // END class Source_AntennaList


} // END namespace RFID.RFIDInterface