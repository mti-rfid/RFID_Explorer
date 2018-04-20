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
 * $Id: Source_GPIO.cs,v 1.3 2009/09/03 20:23:24 dshaheen Exp $
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

    [System.ComponentModel.TypeConverter( typeof( Source_GPIO_TypeConverter ) )]
    public class Source_GPIO
        :
        Object
    {

        public enum OpAccess
        {
            GET     = 0,
            SET     = 1,
        };

        public enum OpState
        {
            LO          = 0,
            HI          = 1,
            FAILURE     = 2,  // guard in gui so this val only programmatically set
            UNSUPPORTED = 3,
        };

        public enum OpResult
        {
            SUCCESS     = 1,
            FAILURE     = 2,  // guard in gui so this val only programmatically set
            UNSUPPORTED = 3,
        };



        internal rfid.Constants.GpioPin nativePin;

        private OpAccess access;
        private OpState  state;
        private OpResult status;
        private string _name; //Add by FJ for change caption of "Antenna Ports" and "GPIO" GUI for HP SiP, 2015-01-22

        public Source_GPIO
        (
            rfid.Constants.GpioPin nativePin,
            OpAccess               access
        )
            :
            base( )
        {
            this.nativePin = nativePin;
            this.access    = access;
            this.state     = OpState.FAILURE;
            this.status    = OpResult.FAILURE;
        }


        public Source_GPIO
        (
            rfid.Constants.GpioPin nativePin,
            OpState                state
        )
            :
            base( )
        {
            this.nativePin = nativePin;
            this.access    = OpAccess.SET;
            this.state     = state;
            this.status    = OpResult.FAILURE;
        }

        // Constructor ( Copy style )

        public Source_GPIO
        (
            Source_GPIO source
        )
            :
            base( )
        {
            this.Copy( source );
        }
        

        public void Copy( Source_GPIO from )
        {
            this.nativePin = from.nativePin;
            this.access    = from.access;
            this.state     = from.state;
            this.status    = from.status;
        }


        public override bool Equals( System.Object obj )
        {
            if ( null == obj )
            {
                return false;
            }

            Source_GPIO rhs = obj as Source_GPIO;

            if ( null == ( System.Object ) rhs )
            {
                return false;
            }

            return this.Equals( rhs );
        }


        public bool Equals( Source_GPIO rhs )
        {
            if ( null == ( System.Object ) rhs )
            {
                return false;
            }

            return
                   this.nativePin == rhs.nativePin
                && this.access    == rhs.access
                && this.state     == rhs.state  
                && this.status    == rhs.status;
        }


        // TODO: provide real hash return value

        public override int GetHashCode( )
        {
            return base.GetHashCode( );
        }




        public rfid.Constants.Result load
        (
            rfid.Linkage transport,
            UInt32       readerHandle
        )
        {
            // TODO : validate that when doin store the given pin has
            //        access flag in GET mode (?)

            // Configure pin to set mode

            rfid.Constants.Result result    = rfid.Constants.Result.OK;
            rfid.Constants.Result resultErr = rfid.Constants.Result.OK;
            uint uiCurError  = 0;
            uint uiLastError = 0;
            byte getValue    = 0;


            //Set access to "Get"
            result = transport.API_GpioSetPinsConfiguration( (byte)this.nativePin,
                                                          0                     );
            if (rfid.Constants.Result.OK != result)
            {

                //Get error
                resultErr = transport.API_MacGetError(ref uiCurError, ref uiLastError);

                //Can't get error
                if (resultErr != rfid.Constants.Result.OK)
                {
                    this.status = OpResult.FAILURE;
                    return resultErr;
                }


                //If error = 0x2B, it means doesn't support this pin
                if (uiCurError == 0x2B)
                {
                    this.status = OpResult.UNSUPPORTED;
                    return result;
                }
                else
                {
                    this.status = OpResult.FAILURE;
                    return result;
                }

            }                


            //Get GPIO Status
            result = transport.API_GpioReadPins( (byte)this.nativePin,
                                                  ref  getValue        );

            if ( rfid.Constants.Result.OK == result )
            {
                this.state  = ( ( UInt32 ) this.nativePin & getValue ) == 0 ? OpState.LO : OpState.HI;
                this.status = OpResult.SUCCESS;
            }
            else
            {
                this.state  = OpState.FAILURE;
                this.status = OpResult.FAILURE;
            }

            return result;
        }



        public rfid.Constants.Result store
        (
            rfid.Linkage transport,
            UInt32       readerHandle
        )
        {
            // TODO : validate that when doin store the given pin has
            //        access flag in SET mode (?)

            // Configure pin to set mode

            rfid.Constants.Result result = rfid.Constants.Result.OK;

            result = transport.API_GpioSetPinsConfiguration( (byte)this.nativePin,
                                                             (byte)this.nativePin  );

            if ( rfid.Constants.Result.OK != result )
            {
                this.status = OpResult.FAILURE;

                return result;
            }


            //2011.12.30 check state
            if(this.state != OpState.HI && this.state != OpState.LO)
            {
                this.status = OpResult.FAILURE;

                return rfid.Constants.Result.INVALID_ANTENNA;
            }

            //Set state
            result = transport.API_GpioWritePins
                                ( (byte)this.nativePin,
                                  (byte)(OpState.LO == this.state ? 0 : this.nativePin) );


            if ( rfid.Constants.Result.OK != result )
            {
                this.status = OpResult.FAILURE;
            }
            else
            {
                this.status = OpResult.SUCCESS;
            }

            return result;
        }




        public String Pin
        {
            get
            {
            //Mod by FJ for change caption of "Antenna Ports" and "GPIO" GUI for HP SiP, 2015-01-22
				 //return this.nativePin.ToString();
                 return _name;
            }
            set
            {
                this._name = value;
			//End by FJ for change caption of "Antenna Ports" and "GPIO" GUI for HP SiP, 2015-01-22
            }
        }

        public OpAccess Access
        {
            get
            {
                return this.access;
            }
            set
            {
                this.access = value;
            }
        }

        public OpState State
        {
            get
            {
                return this.state;
            }
            set
            {
                this.state = value;
            }
        }

        public OpResult Status
        {
            get
            {
                return this.status;
            }
            set
            {
                this.status = value;
            }
        }


    } // END class Source_GPIO


} // END namespace RFID.RFIDInterface
