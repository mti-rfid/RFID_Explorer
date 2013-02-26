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
 * $Id: Source_GPIOList.cs,v 1.4 2009/12/08 07:44:11 dciampi Exp $
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

    [TypeConverter( typeof( Source_GPIOList_TypeConverter ) )]
    public class Source_GPIOList
        :
        List< Source_GPIO >
    {
        // Create an empty gpio list

        public Source_GPIOList( )
            :
            base( )
        {
            // NOP
        }


        // Create an empty gpio list with initial capacity

        public Source_GPIOList( Int32 capacity )
            :
            base( capacity )
        {
            // NOP
        }


        // Copy an gpio list ~ no checks for dup ports

        public Source_GPIOList( IEnumerable< Source_GPIO > enumerable )
            :
            base( enumerable )
        {
            // NOP
        }


        // Copy an gpio list ~ performing a DEEP copy of
        // antennas if indicated

        public Source_GPIOList( IEnumerable< Source_GPIO > enumerable, Boolean deepCopy )
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



        public void Copy( IEnumerable< Source_GPIO > from )
        {
            this.Clear( );

            foreach ( Source_GPIO gpio in from )
            {
                this.Add( new Source_GPIO( gpio ) );
            }
        }


        // Attempt to get the state of all GPIO pins and place
        // results into the list

        public rfid.Constants.Result load
        (
            rfid.Linkage transport,
            UInt32                             readerHandle
        )
        {
            this.Clear( );

            // We do each pin ( though could be done by group ) just so we
            // can tell if individual pin op fails ~ slower but safer...

            foreach
            (
                rfid.Constants.GpioPin nativePin in
                Enum.GetValues( typeof( rfid.Constants.GpioPin ) )
            )
            {
                Source_GPIO gpio = new Source_GPIO( nativePin, Source_GPIO.OpAccess.GET );

                // Errs set in individual gpio objs if they occur
                gpio.load(transport, readerHandle);

                this.Add(gpio);               
            }

            // For now always consider success... user can look thru list
            // to check what pin failures may have occurred...

            return rfid.Constants.Result.OK;
        }


        // Turn all known gpio pins into set mode and attempt to set them
        // to their given ( according to property ) state

        public rfid.Constants.Result store
        (
            rfid.Linkage transport,
            UInt32       readerHandle
        )
        {
            foreach ( Source_GPIO gpio in this )
            {
                gpio.Access = Source_GPIO.OpAccess.SET;

                gpio.store( transport, readerHandle );
            }

            // For now always consider success... user can look thru list
            // to check what pin failures may have occurred...

            return rfid.Constants.Result.OK;
        }


        // Attempt to locate gpio with matching ( native ) pin

        public Source_GPIO FindByPin( rfid.Constants.GpioPin pin )
        {
            Source_GPIO result = this.Find
                (
                    delegate( Source_GPIO gpio )
                    {
                        return gpio.nativePin == pin;
                    }
                );

            return result;
        }



    } // End class Source_GPIOList


} // END namespace RFID.RFIDInterface



