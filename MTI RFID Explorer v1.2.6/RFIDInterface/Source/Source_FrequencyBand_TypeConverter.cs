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
 * $Id: Source_FrequencyBand_TypeConverter.cs,v 1.3 2009/09/03 20:23:24 dshaheen Exp $
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


    class Source_FrequencyBand_TypeConverter
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

            String[ ] channelData = ( value as String ).Split( new Char[ ] { ',' } );

            if ( null == channelData )
            {
                return null; // TODO : supply err msg ~ improper arg 
            }

            if ( 8 != channelData.Length )
            {
                return null; // TODO : supply err msg ~ improper arg count
            }

            try
            {
                UInt32 band = UInt32.Parse( channelData[ 0 ] );

                Source_FrequencyBand.BandState state =
                    ( Source_FrequencyBand.BandState ) Enum.Parse
                    (
                        typeof( Source_FrequencyBand.BandState ),
                        channelData[ 1 ]
                    );

                UInt16 multiplier   = UInt16.Parse( channelData[ 2 ] );
                UInt16 divider      = UInt16.Parse( channelData[ 3 ] );
                UInt16 guardBand    = UInt16.Parse( channelData[ 4 ] );
                UInt16 maxDACBand   = UInt16.Parse( channelData[ 5 ] );
                UInt16 affinityBand = UInt16.Parse( channelData[ 6 ] );
                UInt16 minDACBand   = UInt16.Parse( channelData[ 7 ] );
                

                Source_FrequencyBand channel = new Source_FrequencyBand
                    ( 
                        band,
                        state,
                        multiplier,
                        divider,
                        minDACBand,
                        affinityBand,
                        maxDACBand,
                        guardBand
                    );

                return channel;
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
                Source_FrequencyBand channel = value as Source_FrequencyBand;

                if ( null == channel )
                {
                    throw new ArgumentException( "Expected a Source_FrequencyBand", "value" );
                }

                StringBuilder sb = new StringBuilder( );


                sb.AppendFormat
                    (
                        "{0},{1},{2},{3},{4},{5},{6},{7}",
                        ( UInt32 ) channel.band,
                        channel.state.ToString( ),
                        ( UInt16 ) channel.multiplier,
                        ( UInt16 ) channel.divider,
                        ( UInt16 ) channel.guardBand,
                        ( UInt16 ) channel.maxDACBand,
                        ( UInt16 ) channel.affinityBand,
                        ( UInt16 ) channel.minDACBand                        
                    );

                return sb.ToString( );
            }

            return base.ConvertTo( context, culture, value, destinationType );
        }



    } // END class Source_FrequencyBand_TypeConverter



} // END namespace RFID.RFIDInterface
