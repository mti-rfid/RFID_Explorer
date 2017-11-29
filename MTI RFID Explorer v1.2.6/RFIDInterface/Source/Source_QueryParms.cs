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
 * $Id: Source_QueryParms.cs,v 1.7 2010/01/21 23:03:15 dciampi Exp $
 * 
 * Description: 
 *
 *****************************************************************************
 */



using System;
using System.Collections.Generic;
using System.Text;


using rfid.Structures;
using rfid.Constants;


namespace RFID.RFIDInterface
{

    // TODO: Impl type converter if necessary
    //
    // [System.ComponentModel.TypeConverter( typeof( Source_QueryParms ) )]

    public class Source_QueryParms
        :
        Object // !! cannot derive from base native value type
    {

        // The native object we are exposing...

        TagGroup nativeTagGroup;
        SingulationAlgorithmParms nativeSingulationParms;

        // The source for union type held in the native ref struct
        // nativeQueryParms.

        Source_SingulationParameters sourceParameters;


        // A Copy( ), load( ), loadForAlgorithm( ) operation MUST BE
        // performed after construction but prior to first use !!!

        public Source_QueryParms( )
            :
            base( )
        {
            this.nativeTagGroup = new TagGroup();
            this.nativeSingulationParms = new SingulationAlgorithmParms();
        }


        public void Copy( Source_QueryParms from )
        {
            // Val parm

            this.TagGroupSelected = from.TagGroupSelected;
            this.TagGroupSession  = from.TagGroupSession;
            this.TagGroupTarget   = from.TagGroupTarget;

            if (from.SingulationAlgorithm == SingulationAlgorithm.FIXEDQ)
            {
                this.nativeSingulationParms = new rfid.Structures.FixedQParms( );

                this.sourceParameters = new Source_SingulationParametersFixedQ
                    ( 
                        ( rfid.Structures.FixedQParms ) this.nativeSingulationParms
                    );

                    ( ( Source_SingulationParametersFixedQ ) this.sourceParameters ).Copy
                        (
                            ( Source_SingulationParametersFixedQ ) from.SingulationAlgorithmParameters
                        );
            }
            else if (from.SingulationAlgorithm == SingulationAlgorithm.DYNAMICQ)
            {
                this.nativeSingulationParms = new rfid.Structures.DynamicQParms( );

                this.sourceParameters = new Source_SingulationParametersDynamicQ
                    (
                        ( rfid.Structures.DynamicQParms ) this.nativeSingulationParms
                    );

                    ( ( Source_SingulationParametersDynamicQ ) this.sourceParameters ).Copy
                        (
                            ( Source_SingulationParametersDynamicQ ) from.SingulationAlgorithmParameters
                        );
            }
            else
            {
                Console.WriteLine( "ERR : Algorithm.Copy( Source_QueryParms from )" );
            }
     
        }


        // This load for basic grab current fields on board

        public rfid.Constants.Result load
        (
            rfid.Linkage transport,
            UInt32       readerHandle
        )
        {
            Result result;

            result = transport.API_l8K6CGetQueryTagGroup( ref this.nativeTagGroup);

            if (Result.OK != result)
            {
                return result;
            }

            SingulationAlgorithm algorithm = SingulationAlgorithm.UNKNOWN;

            result = transport.API_l8K6CGetCurrentSingulationAlgorithm(ref algorithm);

            if (Result.OK != result)
            {
                return result;
            }

            switch (algorithm)
            {
                case SingulationAlgorithm.FIXEDQ:
                    {
                        this.nativeSingulationParms = new FixedQParms();
                    }
                    break;
                case SingulationAlgorithm.DYNAMICQ:
                    {
                        this.nativeSingulationParms = new DynamicQParms();
                    }
                    break;
                //case SingulationAlgorithm.DYNAMICQ_ADJUST:
                //    {      
                 
                //    }
                //    break;
                //case SingulationAlgorithm.DYNAMICQ_THRESHOLD:
                //    {
                        
                //    }
                //    break;

                default:
                    {
                        return Result.DRIVER_MISMATCH;
                    }
            }

            result = transport.API_l8K6CGetSingulationAlgorithmParameters
                (
                    algorithm,
                    ref this.nativeSingulationParms
                );

            if ( Result.OK == result )
            {
                
                Type algoType = this.nativeSingulationParms.GetType();

                if (algoType == typeof(rfid.Structures.FixedQParms))
                {
                    this.sourceParameters = new Source_SingulationParametersFixedQ
                        (
                            ( rfid.Structures.FixedQParms ) this.nativeSingulationParms
                        );
                }
                else if (algoType == typeof(rfid.Structures.DynamicQParms))
                {
                    this.sourceParameters = new Source_SingulationParametersDynamicQ
                        (
                            ( rfid.Structures.DynamicQParms ) this.nativeSingulationParms
                        );
                }
                 else
                 {
                    System.Windows.Forms.MessageBox.Show( "ERR : Algorithm.Copy( Source_QueryParms from )" );
                    Console.WriteLine( "ERR : Algorithm.Copy( Source_QueryParms from )" );  
                 }
            }

            return result;
        }



        // Grabs the values on board for the specified algorithm but leaves
        // board in original state - warning - MANY POINTS OF FAILURE - you
        // have been warned.

        public rfid.Constants.Result loadForAlgorithm
        (
            rfid.Linkage                        transport,
            UInt32                              readerHandle,
            rfid.Constants.SingulationAlgorithm algorithm
        )
        {
            // Need to do lower level register manipulation for this one...

            UInt32 reg_0x0901 = 0;

            Result result = Result.OK;

            try
            {
                // Set the algo on the board ~ maintaining all other fields

                result = transport.API_ConfigReadRegister
                (
                    ( UInt16 ) 0x0901, ref reg_0x0901
                );

                if ( rfid.Constants.Result.OK != result )
                {
                    throw new Exception( result.ToString( ) );
                }

                result = transport.API_ConfigWriteRegister
                (
                    ( UInt16 ) 0x0901, ( reg_0x0901 & 0xFFFFFFC0 ) | ( UInt32 ) algorithm
                );

                if ( rfid.Constants.Result.OK != result )
                {
                    throw new Exception( result.ToString( ) );
                }

                // Set for the algo register bank on the board

                result = transport.API_ConfigWriteRegister
                (
                    ( UInt16 ) 0x0902, ( UInt32 ) algorithm
                );

                if ( rfid.Constants.Result.OK != result )
                {
                    throw new Exception( result.ToString( ) );
                }

                result = this.load( transport, readerHandle );

                if ( rfid.Constants.Result.OK != result )
                {
                    throw new Exception( result.ToString( ) );
                }

                // Restore algo bank reg to original value

                result = transport.API_ConfigWriteRegister
                (
                    ( UInt16 ) 0x0901, reg_0x0901
                );

                if ( rfid.Constants.Result.OK != result )
                {
                    throw new Exception( result.ToString( ) );
                }

            }
            catch ( Exception )
            {
                // NOP - let fall thru to result check & msg display
            }

            // If we did ok, our source for the underlying singulation
            // parameters must also now be changed here AND grabbed at
            // the gui level ... 

            if ( Result.OK == result )
            {
                switch( algorithm )
                {
                    case SingulationAlgorithm.FIXEDQ:
                    {
                        sourceParameters = new Source_SingulationParametersFixedQ
                            ( 
                                ( rfid.Structures.FixedQParms ) this.nativeSingulationParms
                            );
                    }
                    break;

                    case SingulationAlgorithm.DYNAMICQ:
                    {
                        sourceParameters = new Source_SingulationParametersDynamicQ
                            (
                                ( rfid.Structures.DynamicQParms ) this.nativeSingulationParms
                            );
                    }
                    break;

                    default:
                        Console.WriteLine( "ERR : Algorithm.Copy( Source_QueryParms from )" );

                    break;
                };
            }

            return result;
        }


        public rfid.Constants.Result store
        (
            rfid.Linkage transport,
            UInt32                             readerHandle
        )
        {
            Result result;

            result = transport.API_l8K6CSetQueryTagGroup(this.nativeTagGroup);

            if (Result.OK != result)
            {
                return result;
            }

            SingulationAlgorithm algorithm;

            Type algoType = this.nativeSingulationParms.GetType();

            if (algoType == typeof(FixedQParms))
                algorithm = SingulationAlgorithm.FIXEDQ;
            else if (algoType == typeof(DynamicQParms))
                algorithm = SingulationAlgorithm.DYNAMICQ;
            else
                return Result.INVALID_PARAMETER;

            result = transport.API_l8K6CSetCurrentSingulationAlgorithm(algorithm);

            if (Result.OK != result)
            {
                return result;
            }

            result = transport.API_l8K6CSetSingulationAlgorithmParameters( algorithm,
                                                                           this.nativeSingulationParms );

            return result;
        }



        public rfid.Constants.Selected TagGroupSelected
        {
            get { return this.nativeTagGroup.selected; }

            set { this.nativeTagGroup.selected = value; }
        }

        public rfid.Constants.Session TagGroupSession
        {
            get { return this.nativeTagGroup.session; }

            set { this.nativeTagGroup.session = value; }
        }

        public rfid.Constants.SessionTarget TagGroupTarget
        {
            get { return this.nativeTagGroup.target; }

            set { this.nativeTagGroup.target = value; }
        }

        public rfid.Constants.SingulationAlgorithm SingulationAlgorithm
        {
            get
            {
                Type algoType = this.nativeSingulationParms.GetType( );

                if (algoType == typeof(rfid.Structures.FixedQParms))
                    return rfid.Constants.SingulationAlgorithm.FIXEDQ;
                else if (algoType == typeof(rfid.Structures.DynamicQParms))
                    return rfid.Constants.SingulationAlgorithm.DYNAMICQ;
                else
                    return rfid.Constants.SingulationAlgorithm.UNKNOWN;
            }

            // set op via loadForAlgorithm( ) method...
        }

        public Source_SingulationParameters SingulationAlgorithmParameters
        {
            get { return this.sourceParameters; }
        }


    } // End class Source_QueryParms


} // End namespace RFID.RFIDInterface


