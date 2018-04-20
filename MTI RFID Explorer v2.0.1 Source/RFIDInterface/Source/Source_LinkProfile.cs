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
 * $Id: Source_LinkProfile.cs,v 1.4 2009/11/12 19:23:20 dciampi Exp $
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

    public class Source_LinkProfile
    {

        protected rfid.Structures.RadioLinkProfile linkProfile;


        public Source_LinkProfile
        (
            rfid.Structures.RadioLinkProfile linkProfile
        )
        {
            // Currently just reference copy ~ change to deep 
            // copy later or ?

            this.linkProfile = linkProfile;
        }


        public Boolean Enabled
        {
            get { return ( this.linkProfile.enabled == 0 ) ? false : true; }

            set { this.linkProfile.enabled = ( UInt32 ) ( ( value == false ) ? 0 : 1 ); }
        }

        public UInt64 ProfileId
        {
            get { return this.linkProfile.profileId; }

            // NO IMPL ON RADIO 
            // set { this.linkProfile.profileId = value; }
        }

        public UInt32 ProfileVersion
        {
            get { return this.linkProfile.profileVersion; }

            // NO IMPL ON RADIO 
            // set { this.linkProfile.profileVersion = value; }
        }

        public String ProfileUniqueId
        {
            get
            {
                // Generated 'field' where profile unique identifier
                // is documented as the profileId + profileVersion

                return String.Format( "{0}:{1}", ProfileId, ProfileVersion );
            }
        }

        public rfid.Constants.RadioProtocol ProfileProtocol
        {
            get { return this.linkProfile.profileProtocol; }

            // NO IMPL ON RADIO 
            // set { this.linkProfile.profileProtocol = value; }
        }

        public Boolean DenseReaderMode
        {
            get { return ( this.linkProfile.denseReaderMode == 0 ) ? false : true; }

            // NO IMPL ON RADIO 
            // set { this.linkProfile.denseReaderMode = ( UInt32 ) ( ( value == false ) ? 0 : 1 ); }
        }

        public UInt32 WidebandRssiSamples
        {
            get { return this.linkProfile.widebandRssiSamples; }

            // NO IMPL ON RADIO 
            // set { this.linkProfile.widebandRssiSamples = value; }
        }

        public UInt32 NarrowbandRssiSamples
        {
            get { return this.linkProfile.narrowbandRssiSamples; }

            // NO IMPL ON RADIO 
            // set { this.linkProfile.narrowbandRssiSamples = value; }
        }

        public Boolean RealtimeRssiEnabled
        {
            get { return ( this.linkProfile.realtimeRssiEnabled == 0 ) ? false : true; }

            // NO IMPL ON RADIO 
            // set { this.linkProfile.realtimeRssiEnabled = ( UInt32 ) ( ( value == false ) ? 0 : 1 ); }
        }

        public UInt32 RealtimeWidebandRssiSamples
        {
            get { return this.linkProfile.realtimeWidebandRssiSamples; }

            // NO IMPL ON RADIO 
            // set { this.linkProfile.realtimeWidebandRssiSamples = value; }
        }

        public UInt32 RealtimeNarrowbandRssiSamples
        {
            get { return this.linkProfile.realtimeNarrowbandRssiSamples; }

            // NO IMPL ON RADIO 
            // set { this.linkProfile.realtimeNarrowbandRssiSamples = value; }
        }


        // TODO: Wrap the union classes so can be visually represented

        public rfid.Structures.RadioLinkProfileConfig_ISO18K6C LinkProfileConfig
        {
            get { return ( rfid.Structures.RadioLinkProfileConfig_ISO18K6C ) this.linkProfile.profileConfig; }

            // NO IMPL ON RADIO 
            // set { this.linkProfile.profileConfig = value; }
        }

        public string Name
        {
            // Since only 1 possibility in current config union, hard
            // coding the casting to it... TODO: make dynamic

            get
            {
                if (((rfid.Structures.RadioLinkProfileConfig_ISO18K6C)this.linkProfile.profileConfig).modulationType == 0 &&
                    ((rfid.Structures.RadioLinkProfileConfig_ISO18K6C)this.linkProfile.profileConfig).millerNumber == 0 &&
                    ((rfid.Structures.RadioLinkProfileConfig_ISO18K6C)this.linkProfile.profileConfig).trLinkFrequency == 0 )
                {
                    return "Profile Unitialized";
                }
                else
                {
                    return string.Format
                        (
                            "{0} / M{1} / {2} khz",
                            ((rfid.Structures.RadioLinkProfileConfig_ISO18K6C)this.linkProfile.profileConfig).modulationType,
                            (UInt32)((rfid.Structures.RadioLinkProfileConfig_ISO18K6C)this.linkProfile.profileConfig).millerNumber,
                            (int)Math.Round
                                (
                                    ((double)((rfid.Structures.RadioLinkProfileConfig_ISO18K6C)this.linkProfile.profileConfig).trLinkFrequency) / 1000.0, 0
                                )
                        );
                }
            }
        }

        public string Description
        {
            get
            {
                return Name;
            }
        }

        public override string ToString( )
        {
            return Name;
        }


    } // End class Source_LinkProfile


} // End namespace RFID.RFIDInterface
