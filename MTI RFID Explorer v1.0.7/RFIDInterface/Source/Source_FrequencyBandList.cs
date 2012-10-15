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
 * $Id: Source_FrequencyBandList.cs,v 1.5 2009/12/03 03:56:28 dciampi Exp $
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

    [TypeConverter( typeof( Source_FrequencyBandList_TypeConverter ) )]
    public class Source_FrequencyBandList
        :
        List< Source_FrequencyBand >
    {

        // Create an empty frequency band list

        public Source_FrequencyBandList( )
            :
            base( )
        {
            // NOP
        }


        // Create an empty antenna list with initial capacity

        public Source_FrequencyBandList( Int32 capacity )
            :
            base( capacity )
        {
            // NOP
        }


        // Copy an frequency band list ~ no checks for dup ports

        public Source_FrequencyBandList( IEnumerable< Source_FrequencyBand > enumerable )
            :
            base( enumerable )
        {
            // NOP
        }


        // Copy an frequency band list ~ performing a DEEP
        // copy of all band objects if indicated

        public Source_FrequencyBandList( IEnumerable< Source_FrequencyBand > enumerable, Boolean deepCopy )
            :
            base( )
        {
            if ( !deepCopy )
            {
                this.AddRange( enumerable );
            }
            else
            {
                this.Copy( enumerable );
            }
        }



        public void Copy( IEnumerable< Source_FrequencyBand > from )
        {
            this.Clear( );

            foreach ( Source_FrequencyBand freqBand in from )
            {
                this.Add( new Source_FrequencyBand( freqBand ) );
            }
        }



        // Attempt to load info for all known antennas

        public rfid.Constants.Result load
        (
            rfid.Linkage transport,
            UInt32                             readerHandle
        )
        {            
            this.Clear( );

            for ( UInt32 band = 0; band < RFID.RFIDInterface.Properties.Settings.Default.MaxFrequencyBands ; band++ )
            {
                Source_FrequencyBand freqBand = new Source_FrequencyBand( band );

                rfid.Constants.Result Result = freqBand.load( transport, readerHandle );

                if ( rfid.Constants.Result.OK == Result )
                {
                    this.Add( freqBand );
                }
                else if ( rfid.Constants.Result.INVALID_PARAMETER == Result )
                {
                    break; // in case max bands gets lowered in future e.g. european version
                }
                else
                {
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
            UInt32 readerHandle
        )
        {
            foreach ( Source_FrequencyBand freqBand in this )
            {
                rfid.Constants.Result Result = freqBand.store( transport, readerHandle );

                if ( rfid.Constants.Result.OK != Result )
                {
                    return Result;
                }
            }

            return rfid.Constants.Result.OK;
        }


        // Attempt to locate channel / frequency band with matching band #

        public Source_FrequencyBand FindByBand( UInt32 band )
        {
            Source_FrequencyBand result = this.Find
                (
                    delegate( Source_FrequencyBand frequencyBand )
                    {
                        return frequencyBand.band == band;
                    }
                );

            return result;
        }



    } // END class Source_FrequencyBandList


} // END namespace RFID.RFIDInterface

