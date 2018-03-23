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
 * $Id: Source_GPIOList_TypeConverter.cs,v 1.3 2009/09/03 20:23:24 dshaheen Exp $
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


    public class Source_GPIOList_TypeConverter
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
            Source_GPIOList gpioList = new Source_GPIOList( );

            if ( String.IsNullOrEmpty( value as string ) )
            {
                return gpioList;
            }

            String[ ] gpioPinStrings = ( value as String ).Split( new Char[ ] { ';' } );

            if ( null == gpioPinStrings )
            {
                return gpioList;
            }

            foreach ( String s in gpioPinStrings )
            {
                Object obj = TypeDescriptor.GetConverter( typeof( Source_GPIO ) ).ConvertFromString( s );

                if ( null == obj )
                {
                    // TODO : supply err msg || rely on Source_GPIO converter for msg
                }
                else
                {
                    gpioList.Add( obj as Source_GPIO );
                }
            }

            return gpioList;
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
                Source_GPIOList gpioList = value as Source_GPIOList;

                if ( null == gpioList )
                {
                    throw new ArgumentException( "Expected a Source_GPIOList", "value" );
                }

                StringBuilder sb = new StringBuilder( );

                foreach ( Source_GPIO gpio in gpioList )
                {
                    Object obj = TypeDescriptor.GetConverter( typeof( Source_GPIO ) ).ConvertToString( gpio );

                    if ( null == obj )
                    {
                        // Should NOT be possible ~ should get exception for bad arg
                        // before seeing a null == obj return value
                    }
                    else
                    {
                        sb.Append( obj as String );
                        sb.Append( ';' );
                    }
                }

                return sb.ToString( );
            }

            return base.ConvertTo( context, culture, value, destinationType );
        }



    } // END class Source_GPIOList_TypeConverter



} // END namespace RFID.RFIDInterface

