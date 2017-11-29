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
 * $Id: Source_Antenna.cs,v 1.3 2009/09/03 20:23:24 dshaheen Exp $
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

    // This impl is just so that the current application layout can
    // be easily mirrored ( it utilized inidividual fields from both
    // of the input sources )

    [System.ComponentModel.TypeConverter( typeof( Source_Antenna_TypeConverter ) )]
    public class Source_Antenna
        :
        Object
    {

        // According to MAC EDS ~ phy port number allocated 2 bits only

        public static readonly UInt32 PHY_MINIMUM = 0;
        public static readonly UInt32 PHY_MAXIMUM = 3;

        // According to MAC EDS ~ caveat RX logical must == TX logical
        // for the current version...


        public static readonly UInt32 LOGICAL_MINIMUM = 0;
        public static readonly UInt32 LOGICAL_MAXIMUM = 15;

        // According to previous app default settings
        //Clark 2011.2.21 Cpoied from R1000 Tracer
        public static readonly UInt32 POWER_MINIMUM = 0;  
        public static readonly UInt32 POWER_MAXIMUM = 330;



        protected UInt32               port;

        protected Source_AntennaResult antennaResult;
        protected Source_AntennaConfig antennaConfig;



        public Source_Antenna
        (
            byte port
        )
            :
            base( )
        {
            this.port          = port;
            this.antennaResult = new Source_AntennaResult( port );
            this.antennaConfig = new Source_AntennaConfig( port );
        }



        public Source_Antenna
        (
            byte                                          port,
            rfid.Constants.AntennaPortState state,
            UInt16                                          powerLevel,
            UInt16                                          dwellTime,
            UInt16                                          numberInventoryCycles,
            byte                                            physicalPort,
            UInt32                                          antennaSenseThreshold
        )
            :
            base( )
        {
            this.port = port;
            this.antennaResult = new Source_AntennaResult( port );
            this.antennaConfig = new Source_AntennaConfig( port );

            this.State                 = state;
            this.PowerLevel            = powerLevel;
            this.DwellTime             = dwellTime;
            this.NumberInventoryCycles = numberInventoryCycles;
            this.PhysicalPort          = physicalPort;
            this.AntennaSenseThreshold = antennaSenseThreshold;
        }



        public Source_Antenna
        (
            Source_Antenna antenna
        )
            :
            this( 0 )
        {
            this.Copy( antenna );   
        }



        public void Copy( Source_Antenna from )
        {
            this.port = from.Port;

            this.antennaResult.Copy( from.antennaResult );
            this.antennaConfig.Copy( from.antennaConfig );
        }


        public override bool Equals( System.Object obj )
        {
            if ( null == obj )
            {
                return false;
            }

            Source_Antenna rhs = obj as Source_Antenna;

            if ( null == ( System.Object ) rhs )
            {
                return false;
            }

            return this.Equals( rhs );
        }


        public bool Equals( Source_Antenna rhs )
        {
            if ( null == ( System.Object ) rhs )
            {
                return false;
            }

            return
                   this.port == rhs.port
                && this.antennaResult.Equals( rhs.antennaResult )
                && this.antennaConfig.Equals( rhs.antennaResult );
        }


        // TODO: provide real hash return value

        public override int GetHashCode( )
        {
            return base.GetHashCode( );
        }




        // Load antenna port info, actually does 2x calls - one for the
        // port Result info and one for port config info

        public rfid.Constants.Result load
        (
            rfid.Linkage transport,
            UInt32                             readerHandle
        )
        {
            // Err msgs emitted thru antennaResult & antennaConfig load( )

            rfid.Constants.Result Result = this.antennaResult.load( transport, readerHandle );

            if ( rfid.Constants.Result.OK != Result )
            {
                return Result;
            }

            return this.antennaConfig.load( transport, readerHandle );
        }

        public rfid.Constants.Result store
        (
            rfid.Linkage transport,
            UInt32                             readerHandle
        )
        {
            // Err msgs emitted thru antennaResult & antennaConfig store( )

            rfid.Constants.Result Result = this.antennaResult.store( transport, readerHandle );

            if ( rfid.Constants.Result.OK != Result )
            {
                return Result;
            }

            return this.antennaConfig.store( transport, readerHandle );
        }



        public UInt32 Port
        {
            get
            {
                return this.port;
            }
        }

        public rfid.Constants.AntennaPortState State
        {
            get
            {
                return this.antennaResult.State;
            }
            set
            {
                this.antennaResult.State = value;
            }
        }

        public UInt16 PowerLevel
        {
            get
            {
                return this.antennaConfig.PowerLevel;
            }
            set
            {
                this.antennaConfig.PowerLevel = value;
            }
        }

        public UInt16 DwellTime
        {
            get
            {
                return this.antennaConfig.DwellTime;
            }
            set
            {
                this.antennaConfig.DwellTime = value;
            }
        }

        public UInt16 NumberInventoryCycles
        {
            get
            {
                return this.antennaConfig.NumberInventoryCycles;
            }
            set
            {
                this.antennaConfig.NumberInventoryCycles = value;
            }
        }

        // At this time Rx and Tx ports MUST be equal - see
        // also file Source_AntennaConfig

        public byte PhysicalPort
        {
            //Mod by FJ for change caption of "Antenna Ports" and "GPIO" GUI for HP SiP, 2015-01-22
            get
            {
                //Mod by FJ for fix model name judgement bug, 2015-02-02
				//rfid.Constants.Result m_result = rfid.Constants.Result.OK;
                LakeChabotReader reader = new LakeChabotReader();
                if (rfid.Constants.Result.OK != reader.result_major)
                {
                    throw new Exception(reader.result_major.ToString());
                    //throw new Exception(result.ToString());
                //End by for FJ fix model name judgement bug, 2015-02-02
                }
                //Mod by FJ for revert physical port display in M03 module, 2016-11-03
                return this.antennaConfig.PhysicalPort;
                //if (reader.uiModelNameMAJOR != 0x4D303358)//0x4D303358==M03X
                //{
                //    return this.antennaConfig.PhysicalPort;                   
                //}
                //else {
                //    return (byte)(this.antennaConfig.PhysicalPort + 1);
                //}
                //End by FJ for revert physical port display in M03 module, 2016-11-03
            }
            set
            {
                //Mod by FJ for fix model name judgement bug, 2015-02-02
				//rfid.Constants.Result m_result = rfid.Constants.Result.OK;
                LakeChabotReader reader = new LakeChabotReader();
                if (rfid.Constants.Result.OK != reader.result_major)
                {
                    throw new Exception(reader.result_major.ToString());
                    //throw new Exception(result.ToString());
                //End by for FJ fix model name judgement bug, 2015-02-02
                }
                //Mod by FJ for revert physical port display in M03 module, 2016-11-03
                this.antennaConfig.PhysicalPort = value;
                //if (reader.uiModelNameMAJOR != 0x4D303358)//0x4D303358==M03X
                //{
                //    this.antennaConfig.PhysicalPort = value;
                //}
                //else
                //{
                //    this.antennaConfig.PhysicalPort = (byte)(value - 1);
                //}
                //End by FJ for revert physical port display in M03 module, 2016-11-03
            }

            /*
            get
            {
                
                return (byte)(this.antennaConfig.PhysicalPort+0x01);
                //return this.antennaConfig.PhysicalPort;
            }
            set
            {
                this.antennaConfig.PhysicalPort = (byte)(value - 1);
                //this.antennaConfig.PhysicalPort = value;
            }
            */
            //End by FJ for change caption of "Antenna Ports" and "GPIO" GUI for HP SiP, 2015-01-22
        }


        public UInt32 AntennaSenseThreshold
        {
            get
            {
                return this.antennaConfig.AntennaSenseThreshold;
            }
            set
            {
                this.antennaConfig.AntennaSenseThreshold = value;
            }
        }

        public UInt32 AntennaSenseValue
        {
            get
            {
                return this.antennaResult.AntennaSenseValue;
            }
        }


    } // End class Source_Antenna


} // End namespace RFID.RFIDInterface
