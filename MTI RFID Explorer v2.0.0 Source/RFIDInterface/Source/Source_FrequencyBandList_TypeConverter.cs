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
 * $Id: Source_FrequencyBandList_TypeConverter.cs,v 1.3 2009/09/03 20:23:24 dshaheen Exp $
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


    public class Source_FrequencyBandList_TypeConverter
        :
        TypeConverter
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
            Source_FrequencyBandList channelList = new Source_FrequencyBandList( );

            if ( String.IsNullOrEmpty( value as string ) )
            {
                return channelList;
            }

            String[ ] channelStrings = ( value as String ).Split( new Char[ ] { ';' } );

            if ( null == channelStrings )
            {
                return channelList;
            }

            foreach ( String s in channelStrings )
            {
                Object obj = TypeDescriptor.GetConverter( typeof( Source_FrequencyBand ) ).ConvertFromString( s );

                if ( null == obj )
                {
                    // TODO : supply err msg || rely on Source_FrequencyBand converter for msg
                }
                else
                {
                    channelList.Add( obj as Source_FrequencyBand );
                }
            }

            return channelList;
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
                Source_FrequencyBandList channelList = value as Source_FrequencyBandList;

                if ( null == channelList )
                {
                    throw new ArgumentException( "Expected a Source_FrequencyBandList", "value" );
                }

                StringBuilder sb = new StringBuilder( );

                foreach ( Source_FrequencyBand channel in channelList )
                {
                    Object obj = TypeDescriptor.GetConverter( typeof( Source_FrequencyBand ) ).ConvertToString( channel );

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



    } // END class Source_FrequencyBandList_TypeConverter


} // END namespace RFID.RFIDInterface
