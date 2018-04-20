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
 * $Id: Source_LinkProfileList.cs,v 1.5 2009/12/03 03:56:55 dciampi Exp $
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


    public class Source_LinkProfileList
        :
        List< Source_LinkProfile >
    {
        /**************************************************
         * WARNING -
         * 
         * While link profiles are exposed as a mutable
         * list via this class once loaded from the radio
         * only the active profile should be modified but
         * the held profiles should NEVER BE MODIFIED !!!
         **************************************************/

        protected rfid.Linkage transport;
        protected UInt32                             readerHandle;

        protected Int32 activeProfileIndex;


        // Force hiding base constructor ~ should never be used

        private Source_LinkProfileList( )
            :
            base( )
        {

        }


        // Provided just to allow easy copying of existing
        // link profile list source(s)

        public Source_LinkProfileList( IEnumerable< Source_LinkProfile > enumerable )
            :
            base( enumerable )
        {
            this.transport    = null;
            this.readerHandle = 0;    // 0 == invalid radio handle

            for ( activeProfileIndex = 0; activeProfileIndex < this.Count; ++activeProfileIndex )
            {
                if ( this[ activeProfileIndex ].Enabled )
                {
                    break;
                }
            }
        }


        // Force hiding base constructor ~ should never be used

        private Source_LinkProfileList( Int32 capacity )
            :
            base( capacity )
        {

        }


        // Primary constructor, provide a transport & handle and load
        // the list from current link profile information

        public Source_LinkProfileList( rfid.Linkage transport, UInt32 readerHandle )
            :
            base( )
        {
            this.transport    = transport;
            this.readerHandle = readerHandle;

            // Ignore error at this time ~ rely on console debug msg

            this.load( );   
        }


        // Attempt to load all link profiles currently on the radio
        // keeping track of the profile marked active

        public rfid.Constants.Result load( )
        {
            this.Clear( );

            byte CurProfile = 0;
            this.transport.API_ConfigGetCurrentLinkProfile(ref CurProfile);

            for (UInt32 profileIndex = 0; profileIndex < RFID.RFIDInterface.Properties.Settings.Default.MaxAllowedProfiles; profileIndex++)
            {
                rfid.Structures.RadioLinkProfile profile =
                    new rfid.Structures.RadioLinkProfile( );

                //MTI protocol doesn't support API_ConfigGetLinkProfile
                rfid.Constants.Result Result = this.transport.API_ConfigGetLinkProfile(profileIndex, profile);
                
                if ( rfid.Constants.Result.OK == Result )
                {
                    this.Add( new Source_LinkProfile( profile ) );

                    //clark 2011.4.19 because MTI protocol doesn't support API_ConfigGetLinkProfile,
                    //we don't know enable value. Use API_ConfigSetCurrentLinkProfile to get current profile.
                    //if ( 0 != profile.enabled )
                    //{
                    //    this.activeProfileIndex = ( Int32 ) profileIndex;                        
                    //}      

                    //Use CurrentLinkProfile number to find and enable flag.
                    if (CurProfile == profileIndex)
                    {
                        //Set current profile by myself
                        profile.enabled = 1;

                        this.activeProfileIndex = (Int32)profileIndex;
                    }                
                }
                else if ( rfid.Constants.Result.INVALID_PARAMETER == Result )
                {
                    break; // this rcv when profileIndex > profile count on radio
                }
                else
                {
                    Console.WriteLine( "Error while reading radio link profiles" );

                    return Result; // this rcv all other errors
                }
            }

            return rfid.Constants.Result.OK;
        }


        // Attempt to save all link profiles currently on the radio
        // and mark the active one.

        public rfid.Constants.Result store( )
        {
            // In reality we can only set the active profile index at this
            // time so devolution to a single call of:

            rfid.Constants.Result Result =
                this.transport.API_ConfigSetCurrentLinkProfile((byte)this.activeProfileIndex);

            return Result;
        }


        // Attempt to locate matching link profile where uniqueId consists of
        // the string [profileId]:[profileVersion]

        public Source_LinkProfile FindByUniqueId( String uniqueIdString )
        {
            if ( null == uniqueIdString )
            {
                return null;
            }

            string[ ] parts = uniqueIdString.Split( ':' );

            if ( null == parts || 2 != parts.Length )
            {
                return null;
            }

            UInt64 profileId;
            UInt32 profileVersion;

            if ( ! ( UInt64.TryParse( parts[ 0 ], out profileId ) && UInt32.TryParse( parts[ 1 ], out profileVersion ) ) )
            {
                return null;
            }

            return this.FindByUniqueId( profileId, profileVersion );
        }


        // Attempt to locate and reture a link profile with a matching
        // unique id ( profileId & profileVersion )

        public Source_LinkProfile FindByUniqueId( UInt64 profileId, UInt32 profileVersion )
        {
            Source_LinkProfile result = this.Find
                (
                    delegate( Source_LinkProfile p )
                    {
                        return p.ProfileId == profileId && p.ProfileVersion == profileVersion;
                    }
                );

            return result;
        }


        // Retrieve active profile ( object ) from cached info ~ if guaranteed up to
        // date info required, perform a load( ) operation immediately prior

        public Source_LinkProfile getActiveProfile( )
        {
            return this[ this.activeProfileIndex ];
        }


        // Retrieve active profile index from cached info ~ if guaranteed up to
        // date info required, perform a load( ) operation immediately prior

        public UInt32 getActiveProfileIndex( )
        {
            return ( UInt32 ) this.activeProfileIndex;
        }


        public void setActiveProfileIndex( int index )
        {
            this[ this.activeProfileIndex ].Enabled = false;
            this.activeProfileIndex = index;
            this[ this.activeProfileIndex ].Enabled = true;
        }


    } // End class Source_LinkProfileList


} // End namespace RFID.RFIDInterface
