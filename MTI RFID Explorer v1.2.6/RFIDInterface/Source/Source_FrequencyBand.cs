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
 * $Id: Source_FrequencyBand.cs,v 1.4 2010/01/07 02:10:57 dshaheen Exp $
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

    // This class does NOT represent a wrapper to a library structure
    // but is its own stand alone unit utilized by the gui

    [TypeConverter( typeof( Source_FrequencyBand_TypeConverter ) )]
    public class Source_FrequencyBand
    {

        public enum BandState
        {
            DISABLED    = 0,
            ENABLED     = 1,
            UNKNOWN     = 2
        }

        
        public static readonly Double ClockKHz = 24000.0;


        // See MAC EDS for these values... utilized by load & store funcs

        private static readonly UInt16 SELECTOR_ADDRESS = 0xC01;
        private static readonly UInt16 CONFIG_ADDRESS   = 0xC02;
        private static readonly UInt16 MULTDIV_ADDRESS  = 0xC03;
        private static readonly UInt16 PLLCC_ADDRESS    = 0xC04;


        public UInt32    band;
        public BandState state;
        public UInt16    multiplier;
        public UInt16    divider;
        public UInt16    minDACBand;
        public UInt16    affinityBand;
        public UInt16    maxDACBand;
        public UInt16    guardBand;



        public Source_FrequencyBand
        (
            UInt32 band
        )
            :
            this( band, 0, 0, 1, 0, 0, 0, 0 )
        {
            // NOP ~ empty designed for immediate load op
        }


        public Source_FrequencyBand
        (
            UInt32    band,
            BandState state,
            UInt16    multiplier,
            UInt16    divider,
            UInt16    minDACBand,
            UInt16    affinityBand,
            UInt16    maxDACBand,
            UInt16    guardBand
        )
            :
            base( )
        {
            this.band           = band;
            this.state          = state;
            this.multiplier     = multiplier;
            this.divider        = divider;
            this.minDACBand     = minDACBand;
            this.affinityBand   = affinityBand;
            this.maxDACBand     = maxDACBand;
            this.guardBand      = guardBand;
        }



        public Source_FrequencyBand
        (
            Source_FrequencyBand frequencyBand
        )
            :
            base( )
        {
            this.Copy( frequencyBand );   
        }


        public void Copy( Source_FrequencyBand from )
        {
            this.band           = from.band;
            this.state          = from.state;
            this.multiplier     = from.multiplier;
            this.divider        = from.divider;
            this.minDACBand     = from.minDACBand;
            this.affinityBand   = from.affinityBand;
            this.maxDACBand     = from.maxDACBand;
            this.guardBand      = from.guardBand;
        }





        public override bool Equals( System.Object obj )
        {
            if ( null == obj )
            {
                return false;
            }

            Source_FrequencyBand rhs = obj as Source_FrequencyBand;

            if ( null == ( System.Object ) rhs )
            {
                return false;
            }

            return this.Equals( rhs );
        }


        public bool Equals( Source_FrequencyBand rhs )
        {
            if ( null == ( System.Object ) rhs )
            {
                return false;
            }

            return
                    this.band           == rhs.band
                &&  this.state          == rhs.state
                &&  this.multiplier     == rhs.multiplier
                &&  this.divider        == rhs.divider
                &&  this.minDACBand     == rhs.minDACBand
                &&  this.affinityBand   == rhs.affinityBand
                &&  this.maxDACBand     == rhs.maxDACBand
                &&  this.guardBand      == rhs.guardBand;
        }


        // TODO: provide real hash return value

        public override int GetHashCode( )
        {
            return base.GetHashCode( );
        }




        // Load frequency info, actually does Nx calls - any
        // of which may fail and cause an error return

        public rfid.Constants.Result load
        (
            rfid.Linkage transport,
            UInt32       readerHandle
        )
        {		
            UInt32 config            = 0;
            UInt32 multdiv           = 0;
            UInt32 pllcc             = 0;

            rfid.Constants.Result result = transport.API_ConfigWriteRegister
            ( 
                SELECTOR_ADDRESS, 
                this.band 
            );

            if ( rfid.Constants.Result.OK != result )
            {
                return result;
            }

            result = transport.API_ConfigReadRegister
            ( 
                CONFIG_ADDRESS, 
                ref config
            );

            if ( rfid.Constants.Result.OK != result )
            {
                return result;
            }

            result = transport.API_ConfigReadRegister
            ( 
                MULTDIV_ADDRESS, 
                ref multdiv
            );

            if ( rfid.Constants.Result.OK != result )
            {
                return result;
            }

            result = transport.API_ConfigReadRegister
            ( 
                PLLCC_ADDRESS, 
                ref pllcc
            );

            if ( rfid.Constants.Result.OK != result )
            {
                return result;
            }

            try
            {
                this.state = ( BandState ) ( config == 0 ? 0 : 1 );
            }
            catch ( Exception )
            {
                this.state = BandState.UNKNOWN;
            }

            this.multiplier   = ( UInt16 ) ( ( multdiv >> 0  ) & 0xffff );
            this.divider      = ( UInt16 ) ( ( multdiv >> 16 ) & 0xff   );

            this.minDACBand   = ( UInt16 ) ( ( pllcc >> 0  ) & 0xff );
            this.affinityBand = ( UInt16 ) ( ( pllcc >> 8  ) & 0xff );
            this.maxDACBand   = ( UInt16 ) ( ( pllcc >> 16 ) & 0xff );
            this.guardBand    = ( UInt16 ) ( ( pllcc >> 24 ) & 0xff );

            return rfid.Constants.Result.OK;
        }




        public rfid.Constants.Result store
        (
            rfid.Linkage transport,
            UInt32                             readerHandle
        )
        {
            UInt32 config    = ( UInt32 ) state;
            UInt32 multdiv   = ( UInt32 ) ( this.divider      << 16 ) | ( UInt32 ) this.multiplier;
            UInt32 pllcc     = ( UInt32 ) ( this.guardBand    << 24 ) |
                               ( UInt32 ) ( this.maxDACBand   << 16 ) |
                               ( UInt32 ) ( this.affinityBand << 8  ) |
                               ( UInt32 ) ( this.minDACBand         ) ;

            rfid.Constants.Result result = transport.API_ConfigWriteRegister
            ( 
                SELECTOR_ADDRESS, 
                this.band 
            );

            if ( rfid.Constants.Result.OK != result )
            {
                return result;
            }

            result = transport.API_ConfigWriteRegister
            ( 
                CONFIG_ADDRESS, 
                config 
            );

            if ( rfid.Constants.Result.OK != result )
            {
                return result;
            }

            result = transport.API_ConfigReadRegister
            (  
                CONFIG_ADDRESS, 
                ref config 
            );

            if ( rfid.Constants.Result.OK != result )
            {
                return result;
            }


		    if ( BandState.ENABLED == ( BandState ) config )
		    {
                result = transport.API_ConfigWriteRegister
                (  
                    MULTDIV_ADDRESS, 
                    multdiv 
                );

                if ( rfid.Constants.Result.OK != result )
                {
                    return result;
                }

                result = transport.API_ConfigReadRegister
                ( 
                    MULTDIV_ADDRESS, 
                    ref multdiv 
                );

                if ( rfid.Constants.Result.OK != result )
                {
                    return result;
                }

                result = transport.API_ConfigWriteRegister
                (  
                    PLLCC_ADDRESS, 
                    pllcc 
                );

                if ( rfid.Constants.Result.OK != result )
                {
                    return result;
                }

                result = transport.API_ConfigReadRegister
                ( 
                    PLLCC_ADDRESS, 
                    ref pllcc
                );

                if ( rfid.Constants.Result.OK != result )
                {
                    return result;
                }

            }

            this.multiplier   = ( UInt16 ) ( ( multdiv >>  0 ) & 0xffff );
            this.divider      = ( UInt16 ) ( ( multdiv >> 16 ) & 0xff   );

            this.minDACBand   = ( UInt16 ) ( ( pllcc >>  0 ) & 0xff );
            this.affinityBand = ( UInt16 ) ( ( pllcc >>  8 ) & 0xff );
            this.maxDACBand   = ( UInt16 ) ( ( pllcc >> 16 ) & 0xff );
            this.guardBand    = ( UInt16 ) ( ( pllcc >> 24 ) & 0xff );

            return rfid.Constants.Result.OK;
        }




        public UInt32 Band
        {
            get { return this.band; }
            set { this.band = value; }
        }

        public BandState State
        {
            get { return this.state; }
            set { this.state = value; }
        }

        public UInt16 MultiplyRatio
        {
            get { return this.multiplier; }
            set { this.multiplier = value; }
        }

        public UInt16 DivideRatio
        {
            get { return this.divider; }
            set { this.divider = value; }
        }

        public UInt16 MinimumDACBand
        {
            get { return this.minDACBand; }
            set { this.minDACBand = value; }
        }

        public UInt16 AffinityBand
        {
            get { return this.affinityBand; }
            set { this.affinityBand = value; }
        }

        public UInt16 MaximumDACBand
        {
            get { return this.maxDACBand; }
            set { this.maxDACBand = value; }
        }

        public UInt16 GuardBand
        {
            get { return this.guardBand; }
            set { this.guardBand = value; }
        }

        public Double Frequency
        {
            get { return ( ClockKHz / ( 4 * DivideRatio ) ) * MultiplyRatio / 1000; }
        }



    } // End class Source_FrequencyBand


} // End namespace RFID.RFIDInterface
