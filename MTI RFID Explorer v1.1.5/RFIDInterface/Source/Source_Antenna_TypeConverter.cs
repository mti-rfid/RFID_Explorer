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
 * $Id: Source_Antenna_TypeConverter.cs,v 1.3 2009/09/03 20:23:24 dshaheen Exp $
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


    public class Source_Antenna_TypeConverter
        :
        System.ComponentModel.TypeConverter
    {

        public override bool CanConvertFrom
        (
            System.ComponentModel.ITypeDescriptorContext context,
            Type sourceType
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

            String[ ] antennaData = ( value as String ).Split( new Char[ ] { ',' } );

            if ( null == antennaData )
            {
                return null; // TODO : supply err msg ~ improper arg 
            }

            if ( 7 != antennaData.Length ) // 8 with threshold value
            {
                return null; // TODO : supply err msg ~ improper arg count
            }

            try
            {
                // TODO : split out parsing ? to better define which parms bad...

                Source_Antenna antenna = new Source_Antenna( byte.Parse( antennaData[ 0 ] ) );

                rfid.Constants.AntennaPortState state =
                    ( rfid.Constants.AntennaPortState ) Enum.Parse
                    (
                        typeof( rfid.Constants.AntennaPortState ),
                        antennaData[ 1 ].ToUpper( )
                    );

                antenna.State                 = state;
                antenna.PowerLevel            = UInt16.Parse(antennaData[2]);
                antenna.DwellTime             = UInt16.Parse( antennaData[ 3 ] );
                antenna.NumberInventoryCycles = UInt16.Parse( antennaData[ 4 ] );
                antenna.PhysicalPort          = byte.Parse( antennaData[ 5 ] );

                // Currently Rx is explicitly tied to Tx so cannot be set - ignore val
                // antenna.PhysicalRxPort        = UInt32.Parse( antennaData[ 6 ] );

                // now using global threshold not per antenna...
                // antenna.AntennaSenseThreshold = UInt32.Parse( antennaData[ 7 ] );
                antenna.AntennaSenseThreshold = 0; // if using Source_Antenna to store, uses glob var

                return antenna;
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
                Source_Antenna antenna = value as Source_Antenna;

                if ( null == antenna )
                {
                    throw new ArgumentException( "Expected a Source_Antenna", "value" );
                }

                StringBuilder sb = new StringBuilder( );

                sb.AppendFormat
                (
                    "{0},{1},{2},{3},{4},{5}", // ,{7}", if with threshold value
                    antenna.Port,
                    antenna.State,
                    antenna.PowerLevel,
                    antenna.DwellTime,
                    antenna.NumberInventoryCycles,
                    antenna.PhysicalPort
                    //antenna.AntennaSenseThreshold
                );

                return sb.ToString( );
            }

            return base.ConvertTo( context, culture, value, destinationType );
        }


    } // END class Source_Antenna_TypeConverter


} // END namespace RFID.RFIDInterface
