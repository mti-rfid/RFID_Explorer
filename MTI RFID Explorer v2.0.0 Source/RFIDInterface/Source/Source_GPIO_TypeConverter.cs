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
 * $Id: Source_GPIO_TypeConverter.cs,v 1.3 2009/09/03 20:23:24 dshaheen Exp $
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


    public class Source_GPIO_TypeConverter
        :
        System.ComponentModel.TypeConverter
    {

        public override bool CanConvertFrom
        (
            System.ComponentModel.ITypeDescriptorContext context,
            Type                                         sourceType
        )
        {
            return
                   typeof( string ) == sourceType
                || base.CanConvertFrom( context, sourceType );
        }

        public override object ConvertFrom
        (
            System.ComponentModel.ITypeDescriptorContext context,
            System.Globalization.CultureInfo             culture,
            Object                                       value
        )
        {
            if ( String.IsNullOrEmpty( value as string ) )
            {
                return null; // TODO : supply err msg
            }

            String[ ] gpioData = ( value as String ).Split( new Char[ ] { ',' } );

            if ( null == gpioData )
            {
                return null; // TODO : supply err msg ~ improper arg 
            }

            if ( 2 > gpioData.Length || 3 < gpioData.Length )
            {
                return null; // TODO : supply err msg ~ improper arg count
            }

            try
            {
                rfid.Constants.GpioPin nativePin =
                    ( rfid.Constants.GpioPin ) Enum.Parse
                    (
                        typeof( Source_GPIO.OpAccess ),
                        gpioData[ 0 ]
                    );

                Source_GPIO.OpAccess access =
                    ( Source_GPIO.OpAccess ) Enum.Parse
                    (
                        typeof( Source_GPIO.OpAccess ),
                        gpioData[ 1 ]
                    );

                Source_GPIO.OpState state = Source_GPIO.OpState.FAILURE;

                if ( 3 == gpioData.Length )
                {
                    state = ( Source_GPIO.OpState ) Enum.Parse
                    (
                        typeof( Source_GPIO.OpState ),
                        gpioData[ 2 ]
                    );
                }

                Source_GPIO gpio = new Source_GPIO( nativePin, access );

                gpio.State  = state;
                gpio.Status = Source_GPIO.OpResult.FAILURE;

                return gpio;
            }
            catch ( Exception )
            {
                // TODO : supply err msg ~ bad arg

                return null;
            }
        }


        public override object ConvertTo
        (
            System.ComponentModel.ITypeDescriptorContext context,
            System.Globalization.CultureInfo             culture,
            object                                       value,
            Type                                         destinationType
        )
        {
            if ( typeof( string ) == destinationType )
            {
                Source_GPIO gpio = value as Source_GPIO;

                if ( null == gpio )
                {
                    throw new ArgumentException( "Expected a Source_GPIO", "value" );
                }

                StringBuilder sb = new StringBuilder( );

                // Assume is GET access then state shouldn't be stored i.e. radio state
                // retrieval should be done.  If SET access, then keep state...

                if ( Source_GPIO.OpAccess.GET == gpio.Access )
                {
                    sb.AppendFormat( "{0},{1}", ( Int32 ) gpio.nativePin, ( Int32 ) gpio.Access );
                }
                else
                {
                    sb.AppendFormat( "{0},{1},{2}", ( Int32 ) gpio.nativePin, ( Int32 ) gpio.Access, ( Int32 ) gpio.State );
                }

                return sb.ToString( );
            }

            return base.ConvertTo( context, culture, value, destinationType );
        }


    } // END class Source_GPIO_TypeConverter


} // END namespace RFID.RFIDInterface
