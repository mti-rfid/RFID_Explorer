/*
 *****************************************************************************
 *                                                                           *
 *                 IMPINJ CONFIDENTIAL AND PROPRIETARY                       *
 *                                                                           *
 * This source code is the sole property of Impinj, Inc.  Reproduction or    *
 * utilization of this source code in whole or in part is forbidden without  *
 * the prior written consent of Impinj, Inc.                                 *
 *                                                                           *
 * (c) Copyright Impinj, Inc. 2009. All rights reserved.                     *
 *                                                                           *
 *****************************************************************************
 */

/*
 *****************************************************************************
 *
 * $Id: Native.cs,v 1.19 2010/11/09 23:05:38 dshaheen Exp $
 * 
 * Description: 
 *
 *****************************************************************************
 */



using System;
using System.Runtime;
using System.Runtime.InteropServices;



using rfid.Constants;
using rfid.Structures;



namespace rfid
{

    // Declare, instantiate and utilize for callbacks
    // originating from inventory, read, write and
    // similar operations...

    public delegate Int32 CallbackDelegate
    (
        [In]      Int32 handle,
        [In]      UInt32 bufferLength,
        [In]      IntPtr pBuffer,
        [In, Out] IntPtr context
    );


    public class Native
    {

        [DllImport( "rfid.dll" )]
        public static extern Result RFID_Startup
        (
            [In, Out] LibraryVersion pVersion,
            [In]      LibraryMode    mode
        );

        [DllImport( "rfid.dll" )]
        public static extern Result RFID_Shutdown();

        [DllImport( "rfid.dll" )]
        public static extern Result RFID_RetrieveAttachedRadiosList
        (
            [In] IntPtr pRadioEnum,
            [In] UInt32 flags
        );

        [DllImport( "rfid.dll" )]
        public static extern Result RFID_RadioOpen
        (
            [In]          UInt32  cookie,
            [In, Out] ref Int32   pHandle,
            [In]          MacMode mode
        );

        [DllImport( "rfid.dll" )]
        public static extern Result RFID_RadioClose
        (
            [In] Int32 handle
        );

        [DllImport( "rfid.dll" )]
        public static extern Result RFID_MacWriteRegister
        (
            [In] Int32  handle,
            [In] UInt16 addr,
            [In] UInt32 val
        );

        [DllImport( "rfid.dll" )]
        public static extern Result RFID_MacReadRegister
        (
            [In]          Int32  handle,
            [In]          UInt16 addr,
            [In, Out] ref UInt32 val
        );

        [DllImport( "rfid.dll" )]
        public static extern Result RFID_MacReadRegisterInfo
        (
            [In]          Int32                      handle,
            [In]          UInt16                     address,
            [In, Out]     RegisterInfo               pInfo
        );

        [DllImport( "rfid.dll" )]
        public static extern Result RFID_MacReadBankedRegister
        (
            [In]          Int32  handle,
            [In]          UInt16 addr,
            [In]          UInt16 bankSelector,
            [In, Out] ref UInt32 val
        );

        [DllImport( "rfid.dll" )]
        public static extern Result RFID_MacWriteBankedRegister
        (
            [In] Int32  handle,
            [In] UInt16 addr,
            [In] UInt16 bankSelector,
            [In] UInt32 val
        );


        [DllImport( "rfid.dll" )]
        public static extern Result RFID_RadioSetOperationMode
        (
            [In] Int32              handle,
            [In] RadioOperationMode mode
        );

        [DllImport( "rfid.dll" )]
        public static extern Result RFID_RadioGetOperationMode
        (
            [In]          Int32              handle,
            [In, Out] ref RadioOperationMode mode
        );

        [DllImport( "rfid.dll" )]
        public static extern Result RFID_RadioSetPowerState
        (
            [In] Int32           handle,
            [In] RadioPowerState state
        );

        [DllImport( "rfid.dll" )]
        public static extern Result RFID_RadioGetPowerState
        (
            [In]          Int32           handle,
            [In, Out] ref RadioPowerState state
        );

        [DllImport( "rfid.dll" )]
        public static extern Result RFID_RadioSetCurrentLinkProfile
        (
            [In] Int32  handle,
            [In] UInt32 profile
        );

        [DllImport( "rfid.dll" )]
        public static extern Result RFID_RadioGetCurrentLinkProfile
        (
            [In]          Int32  handle,
            [In, Out] ref UInt32 profile
        );

        [DllImport( "rfid.dll" )]
        public static extern Result RFID_RadioGetLinkProfile
        (
            [In]      Int32  handle, 
            [In]      UInt32 num, 
            [In, Out] IntPtr profile
        );

        [DllImport( "rfid.dll" )]
        public static extern Result RFID_RadioWriteLinkProfileRegister
        (
            [In] Int32  handle,
            [In] UInt32 profile,
            [In] UInt16 address,
            [In] UInt16 value
        );

        [DllImport( "rfid.dll" )]
        public static extern Result RFID_RadioReadLinkProfileRegister
        (
            [In]          Int32  handle,
            [In]          UInt32 profile,
            [In]          UInt16 address,
            [In, Out] ref UInt16 value
        );

        [DllImport( "rfid.dll" )]
        public static extern Result RFID_AntennaPortGetStatus
        (
            [In]      Int32             handle, 
            [In]      UInt32            port, 
            [In, Out] AntennaPortStatus status
        );

        [DllImport( "rfid.dll" )]
        public static extern Result RFID_AntennaPortSetState
        (
            [In] Int32            handle, 
            [In] UInt32           port, 
            [In] AntennaPortState state
        );

        [DllImport( "rfid.dll" )]
        public static extern Result RFID_AntennaPortSetConfiguration
        (
            [In] Int32             handle, 
            [In] UInt32            port, 
            [In] AntennaPortConfig config
        );

        [DllImport( "rfid.dll" )]
        public static extern Result RFID_AntennaPortGetConfiguration
        (
            [In]      Int32             handle, 
            [In]      UInt32            port, 
            [In, Out] AntennaPortConfig config
        );

        [DllImport( "rfid.dll" )]
        public static extern Result RFID_18K6CSetSelectCriteria
        (
            [In] Int32  handle,
            [In] IntPtr pCriteria,
            [In] UInt32 flags
        );

        [DllImport( "rfid.dll" )]
        public static extern Result RFID_18K6CGetSelectCriteria
        (
            [In]      Int32  handle,
            [In, Out] IntPtr pCriteria
        );

        [DllImport( "rfid.dll" )]
        public static extern Result RFID_18K6CSetPostMatchCriteria
        (
            [In] Int32  handle, 
            [In] IntPtr pCriteria, 
            [In] UInt32 flags
        );

        [DllImport( "rfid.dll" )]
        public static extern Result RFID_18K6CGetPostMatchCriteria
        (
            [In]      Int32  handle,
            [In, Out] IntPtr pCriteria
        );

        [DllImport( "rfid.dll" )]
        public static extern Result RFID_18K6CSetQueryTagGroup
        (
           [In] Int32    handle,
           [In] TagGroup group
        );

        [DllImport( "rfid.dll" )]
        public static extern Result RFID_18K6CGetQueryTagGroup
        (
            [In]      Int32    handle,
            [In, Out] TagGroup pGroup
        );

        [DllImport( "rfid.dll" )]
        public static extern Result RFID_18K6CSetCurrentSingulationAlgorithm
        (
           [In] Int32                handle,
           [In] SingulationAlgorithm algorithm
        );

        [DllImport( "rfid.dll" )]
        public static extern Result RFID_18K6CGetCurrentSingulationAlgorithm
        (
           [In]          Int32                handle,
           [In, Out] ref SingulationAlgorithm algorithm
        );

        [DllImport( "rfid.dll" )]
        public static extern Result RFID_18K6CSetSingulationAlgorithmParameters
        (
           [In] Int32                handle,
           [In] SingulationAlgorithm algorithm,
           [In] IntPtr               pParms
        );

        [DllImport( "rfid.dll" )]
        public static extern Result RFID_18K6CGetSingulationAlgorithmParameters
        (
           [In]      Int32                handle,
           [In]      SingulationAlgorithm algorithm,
           [In, Out] IntPtr               pParms
        );


        [DllImport("rfid.dll" )]
        public static extern Result RFID_18K6CTagInventory
        (
            [In] Int32  handle,
            [In] IntPtr parms, 
            [In] UInt32 flags
        );

        [DllImport("rfid.dll" )]
        public static extern Result RFID_18K6CTagRead
        (
            [In] Int32  handle, 
            [In] IntPtr parms,      
            [In] UInt32 flags
        );

        [DllImport( "rfid.dll" )]
        public static extern Result RFID_18K6CTagWrite
        (
            [In] Int32  handle,
            [In] IntPtr parms,
            [In] UInt32 flags
        );

        [DllImport("rfid.dll" )]
        public static extern Result RFID_18K6CTagKill
        (
            [In] Int32  handle,
            [In] IntPtr parms,
            [In] UInt32 flags
        );

        [DllImport("rfid.dll" )]
        public static extern Result RFID_18K6CTagLock
        (
            [In] Int32  handle,
            [In] IntPtr parms,
            [In] UInt32 flags
        );

        [DllImport("rfid.dll" )]
        public static extern Result RFID_18K6CTagBlockWrite
        (
            [In] Int32  handle,
            [In] IntPtr parms,
            [In] UInt32 flags
        );

        [DllImport("rfid.dll")]
        public static extern Result RFID_18K6CTagQT
        (
            [In] Int32 handle,
            [In] IntPtr parms,
            [In] UInt32 flags
        );

        [DllImport("rfid.dll")]
        public static extern Result RFID_18K6CTagBlockErase
        (
            [In] Int32 handle,
            [In] IntPtr parms,
            [In] UInt32 flags
        );

        [DllImport("rfid.dll" )]
        public static extern Result RFID_RadioCancelOperation
        (
            [In] Int32  handle,
            [In] UInt32 flags
        );

        [DllImport("rfid.dll" )]
        public static extern Result RFID_RadioAbortOperation
        (
            [In] Int32  handle,
            [In] UInt32 flags
        );

        [DllImport( "rfid.dll" )]
        public static extern Result RFID_RadioSetResponseDataMode
        (
            [In] Int32        handle,
            [In] ResponseType responseType,
            [In] ResponseMode responseMode
        );

        [DllImport( "rfid.dll" )]
        public static extern Result RFID_RadioGetResponseDataMode
        (
            [In]          Int32        handle,
            [In]          ResponseType responseType,
            [In, Out] ref ResponseMode responseMode
        );

        [DllImport( "rfid.dll" )]
        public static extern Result RFID_MacUpdateNonvolatileMemory
        (
            [In] Int32  handle,
            [In] UInt32 countBlocks,
            [In] IntPtr pBlocks,
            [In] UInt32 flags
        );

        [DllImport( "rfid.dll" )]
        public static extern Result RFID_MacGetVersion
        (
            [In]      Int32      handle,
            [In, Out] MacVersion pVersion
        );

        [DllImport( "rfid.dll" )]
        public static extern Result RFID_MacGetBootLoaderVersion
        (
            [In]      Int32                handle,
            [In, Out] MacBootLoaderVersion pVersion
        );

        [DllImport( "rfid.dll" )]
        public static extern Result RFID_MacReadOemData
        (
            [In]          Int32  handle,
            [In]          UInt32 address,
            [In, Out] ref UInt32 pCount,
            [In]          IntPtr data
        );

        [DllImport( "rfid.dll" )]
        public static extern Result RFID_MacWriteOemData
        (
            [In]          Int32  handle,
            [In]          UInt32 address,
            [In, Out] ref UInt32 pCount,
            [In]          IntPtr data
        );

        [DllImport( "rfid.dll" )]
        public static extern Result RFID_MacReset
        (
            [In] Int32        handle,
            [In] MacResetType resetType
        );

        [DllImport( "rfid.dll" )]
        public static extern Result RFID_MacClearError
        (
            [In] Int32 handle
        );


        [DllImport( "rfid.dll" )]
        public static extern Result RFID_MacGetError
        (
            [In]          Int32  handle,
            [In, Out] ref UInt32 error,
            [In, Out] ref UInt32 lastError
        );

        [DllImport( "rfid.dll" )]
        public static extern Result RFID_MacBypassWriteRegister
        (
            [In] Int32  handle,
            [In] UInt16 address,
            [In] UInt16 value
        );

        [DllImport( "rfid.dll" )]
        public static extern Result RFID_MacBypassReadRegister
        (
            [In]     Int32  handle,
            [In]     UInt16 address,
            [In] ref UInt16 value
        );

        [DllImport( "rfid.dll" )]
        public static extern Result RFID_MacGetRegion
        (
            [In]     Int32 handle,
            [In] ref MacRegion region,
            [In]     IntPtr regionConfig
        );

        [DllImport( "rfid.dll" )]
        public static extern Result RFID_RadioSetGpioPinsConfiguration
        (
            [In] Int32  handle,
            [In] UInt32 mask,
            [In] UInt32 configuration
        );

        [DllImport( "rfid.dll" )]
        public static extern Result RFID_RadioGetGpioPinsConfiguration
        (
            [In]     Int32  handle,
            [In] ref UInt32 configuration
        );

        [DllImport( "rfid.dll" )]
        public static extern Result RFID_RadioReadGpioPins
        (
            [In]     Int32  handle,
            [In]     UInt32 mask,
            [In] ref UInt32 value
        );

        [DllImport( "rfid.dll" )]
        public static extern Result RFID_RadioWriteGpioPins
        (
            [In] Int32  handle,
            [In] UInt32 mask,
            [In] UInt32 value
        );


        [DllImport( "rfid.dll" )]
        public static extern Result RFID_RadioTurnCarrierWaveOn
        (
            [In] Int32  handle
        );

        [DllImport( "rfid.dll" )]
        public static extern Result RFID_RadioTurnCarrierWaveOff
        (
            [In] Int32  handle
        );

        [DllImport( "rfid.dll" )]
        public static extern Result RFID_RadioIssueCommand
        (
            [In] Int32  handle,
            [In] IntPtr parms 
        );

        [DllImport( "rfid.dll" )]
        public static extern Result RFID_RadioTurnCarrierWaveOnRandom
        (
            [In] Int32  handle,
            [In] IntPtr parms
        );

        [DllImport( "rfid.dll" )]
        public static extern Result RFID_RadioSetImpinjExtensions
        (
           [In] Int32            handle,
           [In] ImpinjExtensions extensions
        );

        [DllImport( "rfid.dll" )]
        public static extern Result RFID_RadioGetImpinjExtensions
        (
            [In]      Int32            handle,
            [In, Out] ImpinjExtensions pExtensions
        );


    }  // Functions class END

} // rfid namespace END
