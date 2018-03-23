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
 * $Id: Structures.cs,v 1.20 2010/11/09 23:05:39 dshaheen Exp $
 * 
 * Description:
 *     This is the RFID Library header file that specifies csharp 'clones' of the
 *     structures defined in the native lib.
 *     
 *
 *****************************************************************************
 */


using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;


using rfid; //.Constants;
using rfid.Constants;
using Global;


namespace rfid
{

    namespace Structures
    {

        // Auto-marshal ok

        [StructLayout( LayoutKind.Sequential )]
        public class Version
        {
            public UInt32 major = 0;
            public UInt32 minor = 0;
            public UInt32 release = 0;

            public Version( )
            {
                // NOP
            }
        }


        // Auto-marshaling ok

        [StructLayout( LayoutKind.Sequential )]
        public class LibraryVersion
            :
            Version
        {
            public LibraryVersion( )
            {
                // NOP
            }
        }


        // Auto-marshaling ok

        [StructLayout( LayoutKind.Sequential )]
        public class FirmwareVersion
            :
            Version
        {
            public FirmwareVersion()
            {
                // NOP
            }
        }

        // Auto-marshaling ok

        [StructLayout( LayoutKind.Sequential )]
        public class MacBootLoaderVersion
            :
            Version
        {
            public MacBootLoaderVersion( )
            {
                // NOP
            }
        }


        // Auto-marshaling ok

        [StructLayout( LayoutKind.Sequential )]
        public class OEMCfgVersion
            :
            Version
        {
            public OEMCfgVersion()
            {
                // NOP
            }
        }


        // Auto-marshaling ok

        [StructLayout( LayoutKind.Sequential )]
        public class OEMCfgUpdateNumber
            :
            Version
        {
            public OEMCfgUpdateNumber()
            {
                // NOP
            }
        }



        // No auto-marshaling due to variable size array

        [StructLayout( LayoutKind.Sequential )]
        public class RadioInformation
        {
            public UInt32        length        = 0;
            public UInt32        idLength      = 0;
            public Byte[ ]       uniqueId      = new Byte[ 0 ];

            public RadioInformation( )
            {
                // NOP
            }
        }


        // No auto-marshaling due to variable size array

        [StructLayout( LayoutKind.Sequential )]
        public class RadioEnumeration
        {
            protected UInt32              length_     = ( UInt32 ) ( 12 + IntPtr.Size );
            public    UInt32              countRadios = 0;
            public    RadioInformation[ ] radioInfo   = new RadioInformation[ 0 ];

            public RadioEnumeration( )
            {
                // NOP
            }

            public UInt32 length
            {
                get { return this.length_; }
            }
        }


        // Internal class - not represented in native lib other 
        // than as anon union profileConfig in RadioLinkProfile

        [StructLayout( LayoutKind.Sequential )]
        public class ProfileConfig
        {
            protected UInt32 length_ = 4;

            public ProfileConfig( )
            {
                // NOP - length_ MUST be set by child classes
            }

            public UInt32 length
            {
                get { return this.length_; }
            }
        }


        // Auto-marshaling ok

        [StructLayout( LayoutKind.Sequential )]
        public class RadioLinkProfileConfig_ISO18K6C
            :
            ProfileConfig
        {
            public    ModulationType modulationType     = ModulationType.UNKNOWN;
            public    UInt32         tari               = 0;
            public    DataDifference data01Difference   = DataDifference.UNKNOWN;
            public    UInt32         pulseWidth         = 0;
            public    UInt32         rtCalibration      = 0;
            public    UInt32         trCalibration      = 0;
            public    DivideRatio    divideRatio        = DivideRatio.UNKNOWN;
            public    MillerNumber   millerNumber       = MillerNumber.UNKNOWN;
            public    UInt32         trLinkFrequency    = 0;
            public    UInt32         varT2Delay         = 0;
            public    UInt32         rxDelay            = 0;
            public    UInt32         minT2Delay         = 0;
            public    UInt32         txPropagationDelay = 0;

            public RadioLinkProfileConfig_ISO18K6C( )
            {
                base.length_ = ( UInt32 ) 56;
            }
        };


        // No auto-marshaling due to base class ProfileConfig use...

        [StructLayout( LayoutKind.Sequential )]
        public class RadioLinkProfile
        {
            protected UInt32        length_                       = ( UInt32 ) ( 48 + 56 );
            public    UInt32        enabled                       = 0;
            public    UInt64        profileId                     = 0;
            public    UInt32        profileVersion                = 0;
            protected RadioProtocol profileProtocol_              = RadioProtocol.ISO18K6C;
            public    UInt32        denseReaderMode               = 0;
            public    UInt32        widebandRssiSamples           = 0;
            public    UInt32        narrowbandRssiSamples         = 0;  
            public    UInt32        realtimeRssiEnabled           = 0;
            public    UInt32        realtimeWidebandRssiSamples   = 0;
            public    UInt32        realtimeNarrowbandRssiSamples = 0;
            protected ProfileConfig profileConfig_                = new RadioLinkProfileConfig_ISO18K6C( );

            public RadioLinkProfile( )
            {
                // NOP
            }

            public UInt32 length
            {
                get { return this.length_; }
            }

            public RadioProtocol profileProtocol
            {
                get { return this.profileProtocol_; }

                set
                {
                    if ( RadioProtocol.UNKNOWN == value )
                    {
                        this.profileProtocol_ = value;
                        this.profileConfig_   = new ProfileConfig( );
                    }
                    else if ( RadioProtocol.ISO18K6C == value )
                    {
                        this.profileProtocol_ = value;
                        this.profileConfig_   = new RadioLinkProfileConfig_ISO18K6C( );
                    }
                }
            }

            public ProfileConfig profileConfig
            {
                get { return this.profileConfig_; }

                set
                {
                    Type configType = value.GetType( );

                    if ( configType == typeof( rfid.Structures.ProfileConfig ) )
                    {
                        this.profileProtocol_ = RadioProtocol.UNKNOWN;
                        this.profileConfig_   = value;
                    }
                    else if ( configType == typeof( rfid.Structures.RadioLinkProfileConfig_ISO18K6C ) )
                    {
                        this.profileProtocol_ = RadioProtocol.ISO18K6C;
                        this.profileConfig_   = value;
                    }
                }
            }
        };



        // Auto-marshaling ok

        [StructLayout( LayoutKind.Sequential )]
        public class AntennaPortStatus
        {
            public    AntennaPortState state             = AntennaPortState.UNKNOWN;
            public    UInt32           antennaSenseValue = 0;

            public AntennaPortStatus( )
            {
                // NOP
            }
        };


        // Auto-marshaling ok

        [StructLayout( LayoutKind.Sequential )]
        public class AntennaPortConfig
        {
            public    UInt16 powerLevel            = 0;
            public    UInt16 dwellTime             = 0;
            public    UInt16 numberInventoryCycles = 0;
            public    byte   physicalPort          = 0;
            public    UInt32 antennaSenseThreshold = 0;

            public AntennaPortConfig( )
            {
                // NOP
            }
        };


        // Auto-marshaling ok since fixed size array

        [StructLayout( LayoutKind.Sequential )]
        public class SelectMask
        {
            public MemoryBank bank      = MemoryBank.UNKNOWN;
            public UInt16     offset    = 0;
            public byte       count     = 0;
            [MarshalAs( UnmanagedType.ByValArray, SizeConst = 32 )]
            public Byte[ ]    mask      = new Byte[ 32 ];

            public SelectMask( )
            {
                // NOP
            }
        };


        // Auto-marshaling ok

        [StructLayout( LayoutKind.Sequential )]
        public class SelectAction
        {
            public Target target         = Target.UNKNOWN;
			//Mod by FJ for NXP authentication function, 2018-02-02
            public rfid.Constants.Action action = rfid.Constants.Action.UNKNOWN; 
			//public Action action         = Action.UNKNOWN;
			//End by FJ for NXP authentication function, 2018-02-02
			public byte   enableTruncate = 0;

            public SelectAction( )
            {
                // NOP
            }
        };


        // Auto-marshaling ok

        [StructLayout( LayoutKind.Sequential )]
        public class SelectCriterion
        {
            public SelectMask   mask   = new SelectMask( );
            public SelectAction action = new SelectAction( );

            public SelectCriterion( )
            {
                // NOP
            }
        };


        // No auto-marshaling due to variable size array - we could
        // constrain to size 8 (?) since max MAC allows...

        [StructLayout( LayoutKind.Sequential )]
        public class SelectCriteria
        {
            public byte               countCriteria = 0;
            public SelectCriterion[ ] pCriteria     = new SelectCriterion[ 0 ];

            public SelectCriteria( )
            {
                // The count criteria field must be set manually since the user
                // is allowed to have a pCriteria array of len N and count less
                // than N...
            }
        };



        // Auto-marshaling ok since fixed size array

        [StructLayout( LayoutKind.Sequential )]
        public class SingulationMask
        {
            public UInt32  offset   = 0;
            public UInt32  count    = 0;
            [MarshalAs( UnmanagedType.ByValArray, SizeConst = 62 )]
            public Byte[ ] mask     = new Byte[ 62 ];

            public SingulationMask( )
            {
                // NOP
            }
        };



        // Auto-marshaling ok

        [StructLayout( LayoutKind.Sequential )]
        public class SingulationCriterion
        {
            public byte            match = 0;
            public SingulationMask mask  = new SingulationMask( );

            public SingulationCriterion( )
            {
                // NOP
            }
        };



        // No auto-marshaling due to variable size array - we could
        // constrain to size 8 (?) since max MAC allows...

        [StructLayout( LayoutKind.Sequential )]
        public class SingulationCriteria
        {
            public UInt32                  countCriteria = 0;
            public SingulationCriterion[ ] pCriteria     = new SingulationCriterion[ 0 ];

            public SingulationCriteria( )
            {
                // NOP
            }
        };




        // Auto-marshaling ok

        [StructLayout( LayoutKind.Sequential )]
        public class TagGroup
        {
            public Selected      selected = Selected.UNKNOWN;
            public Session       session  = Session.UNKNOWN;
            public SessionTarget target   = SessionTarget.UNKNOWN;

            public TagGroup( )
            {
                // NOP
            }
        };



        [StructLayout( LayoutKind.Sequential )]
        public class SingulationAlgorithmParms
        {
            protected UInt32 length_ = 4;

            public SingulationAlgorithmParms( )
            {
                // NOP - child classes MUST set length_ to 
                //       an appropriate value...
            }

            public UInt32 length
            {
                get { return this.length_; }
            }
        }



        // Auto-marshaling ok 

        [StructLayout( LayoutKind.Sequential )]
        public class FixedQParms
            :
            SingulationAlgorithmParms
        {
            public    byte   qValue          = 0;
            public    byte retryCount        = 0;
            public    byte toggleTarget      = 0;
            public    byte repeatUntilNoTags = 0;

            public FixedQParms( )
            {
                base.length_ = ( base.length_ + 16 );
            }
        }


        // Auto-marshaling ok 

        [StructLayout( LayoutKind.Sequential )]
        public class DynamicQParms
            :
            SingulationAlgorithmParms
        {
            public    byte startQValue         = 0;
            public    byte minQValue           = 0;
            public    byte maxQValue           = 0;
            public    byte retryCount          = 0;
            public    byte toggleTarget        = 0;
            public    byte thresholdMultiplier = 0;

            public DynamicQParms( )
            {
                base.length_ = ( base.length_ + 24 );
            }
        }





        // Auto-marshaling not ok ( delegate err in CE ) 

        [StructLayout( LayoutKind.Sequential )]
        public class CommonParms
        {
            public UInt32           tagStopCount = 0;
            public CallbackDelegate callback     = null;
            public IntPtr           context      = IntPtr.Zero;
            public IntPtr           callbackCode = IntPtr.Zero;
            public TagAccessFlag    strcTagFlag;
            public RadioOperationMode OpMode = RadioOperationMode.UNKNOWN;
            //Add by FJ for NXP authentication function, 2018-02-02
            public byte[] bufMTII = new byte[64];
            public byte[] bufMTIA = new byte[64];
            public byte[] bufSecondeMTIA = new byte[64];
            public bool tagFlag = false;
            //Add by FJ for NXP authentication function, 2018-02-02
        }


        // Auto-marshaling not ok ( see common parms ) 

        [StructLayout( LayoutKind.Sequential )]
        public class InventoryParms
        {
            protected UInt32      length_ = ( UInt32 ) ( 4 + ( 4 + IntPtr.Size * 3 + 2) );
            public    CommonParms common  = new CommonParms( );

            public InventoryParms( )
            {
                // NOP
            }

            public UInt32 length
            {
                get { return this.length_; }
            }
        }


        // Used internally only to represent the access command parameter(s)
        // that may make up part of the anonymous union(s) in the native 
        // RFID_18K6C_WRITE_PARMS and RFID_18K6C_QT_PARMS structs
        
        [StructLayout( LayoutKind.Sequential )]
        public class AccessParmsBase
        {
            protected UInt32 length_ = 4;

            public AccessParmsBase( )
            {
                // NOP - child classes MUST set length field to proper value!
            }

            public UInt32 length
            {
                get { return this.length_; }
            }
        }

        // Auto-marshaling not ok ( see common parms ) 

        [StructLayout( LayoutKind.Sequential )]
        public class ReadCmdParms
            : 
            AccessParmsBase
        {
            public    MemoryBank  bank           = MemoryBank.UNKNOWN;
            public    UInt16      offset         = 0;
            public    byte        count          = 0;

            public ReadCmdParms( )
            {
                base.length_ = ( UInt32 ) ( base.length_ + 1 + 2 + 1 );
            }

        }

        // Auto-marshaling not ok ( see common parms ) 

        [StructLayout( LayoutKind.Sequential )]
        public class ReadParms
        {
            protected UInt32       length_        = (UInt32)(4 + (4 + IntPtr.Size * 3 + 2) + (4 + 4 + 2 + 2) + 4);
            public    CommonParms  common         = new CommonParms( );
            public    ReadCmdParms readCmdParms   = new ReadCmdParms( );
            public    UInt32       accessPassword = 0;

            public ReadParms( )
            {
                // NOP
            }

            public UInt32 length
            {
                get { return this.length_; }
            }
        }



        // No auto-marshaling ~ contains variable sized array

        [StructLayout( LayoutKind.Sequential )]
        public class WriteSequentialParms 
            : 
            AccessParmsBase
        {
            public MemoryBank bank   = MemoryBank.UNKNOWN;
            public byte       count  = 0;
            public UInt16     offset = 0;
            public UInt16[ ]  pData  = new UInt16[ 0 ];

            public WriteSequentialParms( )
            {
                base.length_ = ( UInt32 ) ( base.length_ + 4 + 2 + 2  + IntPtr.Size );
            }               
        }



        // No auto-marshaling ~ contains variable sized array(s)

        [StructLayout( LayoutKind.Sequential )]
        public class WriteRandomParms 
            : 
            AccessParmsBase
        {
            public MemoryBank bank     = MemoryBank.UNKNOWN;
            public byte       count    = 0;
            public UInt16     reserved = 0;
            public UInt16[ ]  pOffset  = new UInt16[ 0 ];
            public UInt16[ ]  pData    = new UInt16[ 0 ];

            public WriteRandomParms( )
            {
                base.length_ = ( UInt32 ) ( base.length_ + 4 + 2 + 2 + IntPtr.Size * 2 );
            } 
        }



        // No auto-marshaling ~ contains class w/ base representing
        // a union ( in the native lib )

        [StructLayout( LayoutKind.Sequential )]
        public class WriteParms
        {
            protected UInt32          length_          = (UInt32)(4 + (4 + IntPtr.Size * 3 + 2) + 4 + 4); // modified in constructor
            public    CommonParms     common           = new CommonParms( );
            public    WriteType       writeType        = WriteType.UNKNOWN;
            public    AccessParmsBase writeParms       = new AccessParmsBase( );
            public    UInt32          accessPassword   = 0;

            public WriteParms( )
            {
               WriteRandomParms     tempRandom     = new WriteRandomParms();
               WriteSequentialParms tempSequential = new WriteSequentialParms();
               UInt32               max            = 0;

               // arbitrarily default to one of the lengths
               max = tempSequential.length;

               if (tempRandom.length > max)
               {
                   max = tempRandom.length;
               }

               this.length_ = this.length_ + max;
            }

            public UInt32 length
            {
                get { return this.length_; }
            }
        }


        [StructLayout( LayoutKind.Sequential )]
        public class BlockWriteCmdParms
        {
            protected UInt32         length_          = ( UInt32 ) ( 4 + 1 + 1 + 2 + IntPtr.Size);
            public    MemoryBank     bank             = MemoryBank.UNKNOWN;
            public    byte           count            = 0;
            public    UInt16         offset           = 0;            
            public    UInt16[ ]      pData            = new UInt16[ 0 ];

            public BlockWriteCmdParms( )
            {
                // NOP
            }

            public UInt32 length
            {
                get { return this.length_; }
            }
        }

        // No auto-marshaling ~ contains variable sized array(s)

        [StructLayout( LayoutKind.Sequential )]
        public class BlockWriteParms
        {
            protected UInt32              length_            = (UInt32)(4 + (4 + IntPtr.Size * 3 + 2) + (4 + 4 + 2 + 2 + IntPtr.Size) + 4);
            public    CommonParms         common             = new CommonParms( );
            public    BlockWriteCmdParms  blockWriteCmdParms = new BlockWriteCmdParms( );
            public    UInt32              accessPassword     = 0;

            public BlockWriteParms( )
            {
                // NOP
            }

            public UInt32 length
            {
                get { return this.length_; }
            }
        }

        [StructLayout( LayoutKind.Sequential )]
        public class KillCmdParms
        {
            protected UInt32      length_        = ( UInt32 ) ( 4 +  4);
            public    UInt32      killPassword   = 0;

            public KillCmdParms( )
            {
                // NOP
            }

            public UInt32 length
            {
                get { return this.length_; }
            }
        }

        [StructLayout( LayoutKind.Sequential )]
        public class KillParms
        {
            protected UInt32       length_        = (UInt32)(4 + (4 + IntPtr.Size * 3 + 2) + (4 + 4) + 4);
            public    CommonParms  common         = new CommonParms(  );
            public    KillCmdParms killCmdParms   = new KillCmdParms();
            public    UInt32       accessPassword = 0;

            public KillParms( )
            {
                // NOP
            }

            public UInt32 length
            {
                get { return this.length_; }
            }
        }


        // Auto-marshaling ok

        [StructLayout( LayoutKind.Sequential )]
        public class TagPerm
        {
            public PasswordPermission killPasswordPermissions   = PasswordPermission.NO_CHANGE;
            public PasswordPermission accessPasswordPermissions = PasswordPermission.NO_CHANGE;
            public MemoryPermission   epcMemoryBankPermissions  = MemoryPermission.NO_CHANGE;
            public MemoryPermission   tidMemoryBankPermissions  = MemoryPermission.NO_CHANGE;
            public MemoryPermission   userMemoryBankPermissions = MemoryPermission.NO_CHANGE;

            public TagPerm( )
            {
                // NOP 
            }
        }


        [StructLayout( LayoutKind.Sequential )]
        public class LockCmdParms
        {
            protected UInt32      length_        = ( UInt32 ) ( 4 + ( 20 ));
            public    TagPerm     permissions    = new TagPerm( );

            public LockCmdParms( )
            {
                // NOP
            }

            public UInt32 length
            {
                get { return this.length_; }
            }
        }


        [StructLayout( LayoutKind.Sequential )]
        public class LockParms
        {
            protected UInt32       length_        = (UInt32)(4 + (4 + IntPtr.Size * 3 + 2) + (4 + (20)) + 4);
            public    CommonParms  common         = new CommonParms( );
            public    LockCmdParms lockCmdParms   = new LockCmdParms( );
            public    UInt32      accessPassword = 0;

            public LockParms( )
            {
                // NOP
            }

            public UInt32 length
            {
                get { return this.length_; }
            }
        }


        [StructLayout( LayoutKind.Sequential )]
        public class QTCmdParms
        {
            protected UInt32             length_        = ( UInt32 ) ( 4 + 4 + 4 + 4 + 4);
            public    QTCtrlType         qtReadWrite    = QTCtrlType.UNKNOWN;
            public    QTPersistenceType  qtPersistence  = QTPersistenceType.UNKNOWN;
            public    QTShortRangeType   qtShortRange   = QTShortRangeType.UNKNOWN;
            public    QTMemMapType       qtMemoryMap    = QTMemMapType.UNKNOWN;

            public QTCmdParms( )
            {
                // NOP
            }

            public UInt32 length
            {
                get { return this.length_; }
            }
        }

        [StructLayout( LayoutKind.Sequential )]
        public class QTParms
        {
            protected UInt32          length_        = (UInt32)(4 + (4 + IntPtr.Size * 3 + 2) + 4 +
                                                               ( 4 + 4 + 4 + 4 + 4) + 4 );  // modified in constructor
            public    CommonParms     common         = new CommonParms( );
            public    OptType         optCmdType     = OptType.UNKNOWN;
            public    AccessParmsBase accessParms    = new AccessParmsBase( );
            public    QTCmdParms      qtCmdParms     = new QTCmdParms( );
            public    UInt32          accessPassword = 0;

            public QTParms( )
            {
                WriteRandomParms     tempRandom     = new WriteRandomParms();
                WriteSequentialParms tempSequential = new WriteSequentialParms();
                ReadCmdParms         tempRead       = new ReadCmdParms();
                UInt32               max            = 0;

                // arbitrarily default to one of the lengths
                max = tempSequential.length;

                // account for randomParms being larger
                if (tempRandom.length > max)
                {
                    max = tempRandom.length;
                }

                // account for readParms being larger
                if (tempRead.length > max)
                {
                    max = tempRead.length;
                }

                this.length_ = this.length_ + max;
            }

            public UInt32 length
            {
                get { return this.length_; }
            }
        }


        // Auto-marshaling not ok ( see common parms ) 

        [StructLayout( LayoutKind.Sequential )]
        public class BlockEraseCmdParms
        {
            protected UInt32      length_        = (UInt32)(4 + 2 + 1 + 1);
            public    UInt16      offset         = 0;
            public    MemoryBank  bank           = MemoryBank.UNKNOWN;
            public    byte        count          = 0;

            public BlockEraseCmdParms( )
            {
               // NOP
            }

            public UInt32 length
            {
                get { return this.length_; }
            }

        }

        // Auto-marshaling not ok ( see common parms ) 

        [StructLayout( LayoutKind.Sequential )]
        public class BlockEraseParms
        {
            protected UInt32             length_            = (UInt32)(4 + (4 + IntPtr.Size * 3 + 2) + (4 + 2 + 1 + 1) + 4);
            public    CommonParms        common             = new CommonParms( );
            public    BlockEraseCmdParms blockEraseCmdParms = new BlockEraseCmdParms( );
            public    UInt32             accessPassword = 0;

            public BlockEraseParms( )
            {
                // NOP
            }

            public UInt32 length
            {
                get { return this.length_; }
            }
        }


        [StructLayout( LayoutKind.Sequential )]
        public class NonVolatileMemoryBlock
        {
            public UInt32   address = 0;
            public UInt32   length  = 0;
            public Byte[ ]  pData   = new Byte[ 0 ];
            public UInt32   flags   = 0;

            public NonVolatileMemoryBlock( )
            {
                // NOP
            }
        }


        // Auto-marshaling not ok 

        [StructLayout( LayoutKind.Sequential )]
        public class IssueCommandParms
        {
            protected UInt32           length_        = (UInt32)(4 + 4 + (IntPtr.Size * 3 + 2));
            public    UInt32           command        = 0;
            public    CallbackDelegate callback       = null;
            public    IntPtr           context        = IntPtr.Zero;
            public    IntPtr           callbackCode   = IntPtr.Zero;

            public IssueCommandParms( )
            {
                // NOP
            }

            public UInt32 length
            {
                get { return this.length_; }
            }
        }

        // Auto-marshaling not ok 

        [StructLayout( LayoutKind.Sequential )]
        public class RandomCwParms
        {
            protected UInt32           length_        = ( UInt32 ) ( 4 + 4 + (IntPtr.Size * 3));
            public    UInt32           duration       = 0;
            public    CallbackDelegate callback       = null;
            public    IntPtr           context        = IntPtr.Zero;
            public    IntPtr           callbackCode   = IntPtr.Zero;

            public RandomCwParms( )
            {
                // NOP
            }

            public UInt32 length
            {
                get { return this.length_; }
            }
        }


        // Auto-marshaling ok

        [StructLayout( LayoutKind.Sequential )]
        public class RegisterInfo
        {
            protected UInt32                     length_          = 13;
            public    UInt16                     selectorAddress  = 0;
            public    UInt16                     currentSelector  = 0;
            public    UInt16                     reserved         = 0;
            public    byte                       bankSize         = 0;
            public    RegisterType               type             = RegisterType.UNKNOWN;
            public    RegisterProtectionType     accessType       = RegisterProtectionType.UNKNOWN;

            public RegisterInfo( )
            {
                // NOP
            }

            public UInt32 length
            {
                get { return this.length_; }
            }
        };


        // Auto-marshaling ok
        [StructLayout( LayoutKind.Sequential )]
        public class ImpinjExtensions
        {
            public BlockWriteMode      blockWriteMode  = BlockWriteMode.UNKNOWN;
            public TagSupression       tagSuppression  = TagSupression.UNKNOWN;
            public SerializedTid       serializedTid   = SerializedTid.UNKNOWN;

            public ImpinjExtensions( )
            {
                // NOP
            }
        };


        [StructLayout(LayoutKind.Sequential)]
        public class HostPkt
        {
            /* Packet specific version number                                         */
            byte   pkt_ver  = 0;
            /* Packet specific flags*/
            byte   flags    = 0;
            /* Packet type identifier                                                 */
            UInt16 pkt_type = 0;
            /* Packet length indicator - number of 32-bit words that follow the common*/
            /* packet preamble (i.e., this struct)                                    */
            UInt16 pkt_len  = 0;
            /* Reserved for future use                                                */
            UInt16 res0     = 0;
        };

        //Add by FJ for NXP authentication function, 2018-02-02
        [StructLayout(LayoutKind.Sequential)]
        public class NXPChangeConfig
        {
            protected UInt32 length_ = (UInt32)(2 + 1 + 1 + 4);
            public UInt16 configWord = 0;
            public byte retryCount = 0;
            public byte performOption = 0;
            public UInt32 accessPassword = 0;

            public NXPChangeConfig()
            {
                // NOP
            }

            public UInt32 length
            {
                get { return this.length_; }
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public class NXPTAM1Authenticate
        {
            protected UInt32 length_ = (UInt32)(1 + 1 + 1 + 1 + 4);
            public byte replySetting = 0;
            public byte keyID = 0;
            public byte retryCount = 0;
            public byte performOption = 0;
            public UInt32 accessPassword = 0;

            public NXPTAM1Authenticate()
            {
                // NOP
            }

            public UInt32 length
            {
                get { return this.length_; }
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public class NXPTAM2Authenticate
        {
            protected UInt32 length_ = (UInt32)(1 + 1 + 2 + 1 + 1 + 1 + 4);
            public byte replySetting = 0;
            public byte memoryProfile = 0;
            public UInt16 offset = 0;
            public byte blockCount = 0;
            public byte retryCount = 0;
            public byte performOption = 0;
            public UInt32 accessPassword = 0;

            public NXPTAM2Authenticate()
            {
                // NOP
            }

            public UInt32 length
            {
                get { return this.length_; }
            }
        }

        //End by FJ for NXP authentication function, 2018-02-02

        //Add by FJ for CB-2 Tag Access function, 2018-03-20
        [StructLayout(LayoutKind.Sequential)]
        public class CB2_Data
        {
            protected UInt32 length_ = (UInt32)(11 + 4);
            public string Tag_PC_EPC_CRC  = "";
            public string Tag_TID = "";
            public string Tag_TID_XOR = "";
            public string Tag_RESERVED_MAPPED_DAT1 = "";   //0A
            public string Tag_RESERVED_MAPPED_DAT0 = "";   //04
            public string Tag_USER_KEYS = "";
            public string Tag_USER_VER = "";
            public string Tag_USER_KEY = "";
            public string Tag_USER_BAT = "";
            public string Tag_USER_ANT = "";
            public string Tag_USER_LED = "";
            public bool Tag_ACCESS_FLAG = false;
            public UInt32 Tag_Error = 0;

            public CB2_Data()
            {
                // NOP
            }

            public UInt32 length
            {
                get { return this.length_; }
            }
        }
        //End by FJ for CB-2 Tag Access function, 2018-03-20


    } // Structures end


} // rfid_csharp end

