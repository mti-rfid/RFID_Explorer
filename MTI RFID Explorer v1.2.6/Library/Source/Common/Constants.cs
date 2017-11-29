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
 * $Id: Constants.cs,v 1.14 2010/11/11 17:41:32 dshaheen Exp $
 * 
 * Description:
 *     This is the RFID Library header file that specifies the enumeration
 *     and various other constants.
 *     
 *
 *****************************************************************************
 */


using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;


namespace rfid
{

    namespace Constants
    {

        /**
         * See native RFID_FLAG_LIBRARY_ ( func: RFID_Startup )
        **/

        public enum LibraryMode : uint
        {
            DEFAULT   = 0x0,
            EMULATION = 0x1,
            UNKNOWN   = 0xFFFF
        };

        /**
         * See native RFID_FLAG_MAC_ ( func: RFID_RadioOpen )
        **/

        public enum MacMode : uint
        {
            DEFAULT   = 0x0,
            EMULATION = 0x1,
            UNKNOWN   = 0xFFFF
        };

        /**
         * See native RFID_RADIO_OPERATION_MODE_*
        **/

        public enum RadioOperationMode : byte
        {
            CONTINUOUS,
            NONCONTINUOUS,
            UNKNOWN = 0xFF
        };

        /**
         * See native RFID_RADIO_POWER_STATE_*
        **/

        public enum RadioPowerState : byte
        {
            FULL,
            STANDBY,
            UNKNOWN = 0xFF
        };

        /**
         * See native RFID_RADIO_PORT_STATE_*
        **/

        public enum AntennaPortState : byte
        {
            DISABLED,
            ENABLED,
            UNKNOWN = 0xFF
        };

        /**
         * See native RFID_18K6C_MODULATION_TYPE_*
        **/

        public enum ModulationType : uint
        {
            DSB_ASK,
            SSB_ASK,
            PR_ASK,
            UNKNOWN = 0xFFF
        };

        /**
         * See native RFID_18K6C_DATA_0_1_DIFFERENCE_*
        **/

        public enum DataDifference : uint
        {
            HALF_TARI,
            ONE_TARI,
            UNKNOWN = 0xFFFF
        };

        /**
         * See native RFID_18K6C_DIVIDE_RATIO_*
        **/

        public enum DivideRatio : uint
        {
            RATIO_8,
            RATIO_64DIV3,
            UNKNOWN = 0xFFFF
        };

        /**
         * See native RFID_18K6C_MILLER_NUMBER_*
        **/

        public enum MillerNumber : uint
        {
            NUMBER_FM0,
            NUMBER_2,
            NUMBER_4,
            NUMBER_8,
            UNKNOWN = 0xFFFF
        };

        /**
         * See native RFID_RADIO_PROTOCOL_*
        **/

        public enum RadioProtocol : uint
        {
            ISO18K6C,
            UNKNOWN = 0xFFFF
        };

        /**
         * See native RFID_18K6C_MEMORY_BANK_*
        **/

        public enum MemoryBank : byte
        {
            RESERVED,
            EPC,
            TID,
            USER,
            UNKNOWN = 0xFF
        };

        /**
         * See native RFID_18K6C_TARGET_*
        **/

        public enum Target : byte
        {
            S0,
            S1,
            S2,
            S3,
            SELECTED,
            UNKNOWN = 0xFF
        };

        /**
         * See native RFID_18K6C_ACTION_*
        **/

        public enum Action : byte
        {
            ASLINVA_DSLINVB,
            ASLINVA_NOTHING,
            NOTHING_DSLINVB,
            NSLINVS_NOTHING,
            DSLINVB_ASLINVA,
            DSLINVB_NOTHING,
            NOTHING_ASLINVA,
            NOTHING_NSLINVS,
            UNKNOWN = 0xFF
        };


        /**
         * See native RFID_18K6C_ACTIVE_ENABLE*
        **/

        public enum AvtiveEnable : byte
        {
            DISENABLE,
            ENABLE,
            UNKNOWN = 0xFF
        };


        /**
         * See native RFID_18K6C_SELECTED_*
        **/

        public enum Selected : byte
        {
            SELECT_ALL,
            RESERVED,
            SELECT_DEASSERTED,
            SELECT_ASSERTED,
            UNKNOWN = 0xFF
        };

        /**
         * See native RFID_18K6C_INVENTORY_SESSION_*
        **/

        public enum Session : byte
        {
            S0,
            S1,
            S2,
            S3,
            UNKNOWN = 0xFF
        };

        /**
         * See native RFID_18K6C_INVENTORY_SESSION_TARGET_*
        **/

        public enum SessionTarget : byte
        {
            A,
            B,
            UNKNOWN = 0xFF
        };

        /**
         * See native RFID_18K6C_SINGULATION_ALGORITHM_*
        **/

        public enum SingulationAlgorithm : byte
        {
            FIXEDQ,
            DYNAMICQ,
            UNKNOWN = 0xFF
        };

        /**
         * See native RFID_18K6C_WRITE_TYPE_*
        **/

        public enum WriteType : uint
        {
            SEQUENTIAL,
            RANDOM,
            UNKNOWN = 0xFFFF
        };

        /**
         * See native RFID_18K6C_TAG_PWD_PERM_*
        **/

        public enum PasswordPermission : byte
        {
            ACCESSIBLE,
            ALWAYS_ACCESSIBLE,
            SECURED_ACCESSIBLE,
            ALWAYS_NOT_ACCESSIBLE,
            NO_CHANGE,
            UNKNOWN                = 0xFF
        };

        /**
         * See native RFID_18K6C_TAG_MEM_PERM_*
        **/

        public enum MemoryPermission : uint
        {
            WRITEABLE,
            ALWAYS_WRITEABLE,
            SECURED_WRITEABLE,
            ALWAYS_NOT_WRITEABLE,
            NO_CHANGE,
            UNKNOWN              = 0xFFFF
        };


        /**
         * See native RFID_18K6C_QT_OPT_CMD_TYPE*
        **/

        public enum OptType : uint
        {
            OPT_NONE,
            OPT_READ,
            OPT_WRITE_TYPE_SEQUENTIAL,
            OPT_WRITE_TYPE_RANDOM,
            UNKNOWN = 0xFFFF
        };


        /**
         * See native RFID_18K6C_QT_CTRL_TYPE*
        **/

        public enum QTCtrlType : uint
        {
            READ,
            WRITE,
            UNKNOWN = 0xFFFF
        };

        /**
         * See native RFID_18K6C_QT_PERSISTENCE_TYPE*
        **/

        public enum QTPersistenceType : uint
        {
            TEMPORARY,
            PERMANENT,
            UNKNOWN = 0xFFFF
        };


        /**
         * See native RFID_18K6C_QT_SR_TYPE*
        **/

        public enum QTShortRangeType : uint
        {
            DISABLE,
            ENABLE,
            UNKNOWN = 0xFFFF
        };

        /**
         * See native RFID_18K6C_QT_MEMMAP_TYPE*
        **/

        public enum QTMemMapType : uint
        {
            PRIVATE,
            PUBLIC,
            UNKNOWN = 0xFFFF
        };

        /**
         * See native RFID_RESPONSE_TYPE_*
        **/

        public enum ResponseType : uint
        {
            DATA    = 0xFFFFFFFF,
            UNKNOWN = 0xFFFF
        };

        /**
         * See native RFID_RESPONSE_MODE_*
        **/

        public enum ResponseMode : uint
        {
            COMPACT  = 0x00000001,
            NORMAL   = 0x00000003,
            EXTENDED = 0x00000007,
            UNKNOWN  = 0xFFFF
        };

		//Add by Chingsheng for display extention data, 2015-01-14
        /**
         * See native RFID_RESPONSE_PACKET_FORMAT *
        **/

        public enum ResponseResponseFormat : byte
        {
            DISABLED = 0x00,
            ENABLED = 0x01,
        };
		//End by Chingsheng for display extention data, 2015-01-14
        /**
         * See native RFID_MAC_RESET_TYPE_*
        **/

        public enum MacResetType : uint
        {
            SOFT,
            TO_BOOTLOADER,
            UNKNOWN = 0xFFFF
        };

        //Add by FJ for enhanced region and frequency selection function, 2017-02-10
        public enum RegionConstant : int
        {
            //Customer First Frequency OEM Address 0x00002237(HEX) = 8759(Decimal)
            CustomerFreqAddress = 8759,

            //The US Region Country Name OEM Address : 0x00001BEF(HEX) = 7151(Decimal), US is the first one of regions
            USCountryAddress = 7151,

            //TotalRegion :US!BEU!BEU2!BTW!BCN!BKR!BAU/NZ!BBR!BIL!BIN!BCustomer!BJP
            TotalRegion = 12,

            //1-Frequency Diable  3- Frequency Enable
            FreqEnable = 3,

            //Region Customer Flag
            RegionCustomer = 11,

            //Interval of Region OEM Address
            RegionOffset = 176,

            //Offset between first Frequency and Country Name of Region OEM Address
            RegionFreqOffset = 152,

            //Amount of Frequency in Every Region
            RegionFreqCount = 50,

            //Offset of next Frequency if current Frequency Not Support
            EnableFreqOffset = 3,

            //Offset of next Frequency if current Frequency Support
            DiableFreqOffset = 2
        };
        //End by FJ for enhanced region and frequency selection function, 2017-02-10

        /**
         * See native RFID_MAC_REGION_*
        **/

        public enum MacRegion : uint
        {
            US,
            EU,           
            EU2,
            TW,
            CN,
            KR,
            AU,
            BR,
            IL,
            IN,
            CUSTOMER,

            //clark not sure. wait for firmware team to support
            JP,
            HK,
            MY,
            SG,
            TH,
            RU,
            SA,
            JO,
            MX,
            UNKNOWN = 0xFF
        };

        /**
         * See native RFID_RADIO_GPIO_*
        **/

        public enum GpioPin : byte
        {
            PIN_0  = ( byte ) 0x1 << 0,
            PIN_1 =  ( byte ) 0x1 << 1,
            PIN_2  = ( byte ) 0x1 << 2,
            PIN_3  = ( byte ) 0x1 << 3

            //clark 2011.2.16   Only support 4 pings
            /*
            PIN_4  = ( uint ) 0x1 << 4,
            PIN_5  = ( uint ) 0x1 << 5,
            PIN_6  = ( uint ) 0x1 << 6,
            PIN_7  = ( uint ) 0x1 << 7,
            PIN_8  = ( uint ) 0x1 << 8,
            PIN_9  = ( uint ) 0x1 << 9,
            PIN_10 = ( uint ) 0x1 << 10,
            PIN_11 = ( uint ) 0x1 << 11,
            PIN_12 = ( uint ) 0x1 << 12,
            PIN_13 = ( uint ) 0x1 << 13,
            PIN_14 = ( uint ) 0x1 << 14,
            PIN_15 = ( uint ) 0x1 << 15,
            PIN_16 = ( uint ) 0x1 << 16,
            PIN_17 = ( uint ) 0x1 << 17,
            PIN_18 = ( uint ) 0x1 << 18,
            PIN_19 = ( uint ) 0x1 << 19,
            PIN_20 = ( uint ) 0x1 << 20,
            PIN_21 = ( uint ) 0x1 << 21,
            PIN_22 = ( uint ) 0x1 << 22,
            PIN_23 = ( uint ) 0x1 << 23,
            PIN_24 = ( uint ) 0x1 << 24,
            PIN_25 = ( uint ) 0x1 << 25,
            PIN_26 = ( uint ) 0x1 << 26,
            PIN_27 = ( uint ) 0x1 << 27,
            PIN_28 = ( uint ) 0x1 << 28,
            PIN_29 = ( uint ) 0x1 << 29,
            PIN_30 = ( uint ) 0x1 << 30,
            PIN_31 = ( uint ) 0x1 << 31
            */
        };



        /**
         * See native rfid_error.h file ( function result value definitions )
        **/

        public enum Result : int
        {
            /* Success                                                               **/

            OK           =  0,

            /* Attempted to open a radio that is already open                        **/

            ALREADY_OPEN = -9999,

            /* Buffer supplied is too small                                          **/

            BUFFER_TOO_SMALL,

            /* General failure                                                       **/

            FAILURE,

            /* Failed to load radio bus driver                                       **/

            DRIVER_LOAD,

            /* Library cannot use version of radio bus driver present on system      **/

            DRIVER_MISMATCH,

            /* This error code is no longer used, maintain slot in enum             **/

            RESERVED_01,

            /* Antenna number is invalid                                             **/

            INVALID_ANTENNA,

            /* Radio handle provided is invalid                                      **/

            INVALID_HANDLE,

            /* One of the parameters to the function is invalid                      **/

            INVALID_PARAMETER,

            /* Attempted to open a non-existent radio                                **/

            NO_SUCH_RADIO,

            /* Library has not been successfully initialized                         **/

            NOT_INITIALIZED,

            /* Function not supported                                                **/

            NOT_SUPPORTED,

            /* Op cancelled by cancel op func, close radio, or library shutdown      **/

            OPERATION_CANCELLED,

            /* Library encountered an error allocating memory                        **/

            OUT_OF_MEMORY,

            /* The operation cannot be performed because the radio is currently busy **/

            RADIO_BUSY,

            /* The underlying radio module encountered an error                      **/

            RADIO_FAILURE,

            /* The radio has been detached from the system                           **/

            RADIO_NOT_PRESENT,

            /* The RFID library function is not allowed at this time.                **/

            CURRENTLY_NOT_ALLOWED,

            /* The radio module's MAC firmware is not responding to requests.        **/

            RADIO_NOT_RESPONDING,

            /* The MAC firmware encountered an error while initiating the nonvolatile**/
            /* memory update.  The MAC firmware will return to its normal idle state **/
            /* without resetting the radio module.                                   **/

            NONVOLATILE_INIT_FAILED,

            /* An attempt was made to write data to an address that is not in the    **/
            /* valid range of radio module nonvolatile memory addresses.             **/

            NONVOLATILE_OUT_OF_BOUNDS,

            /* The MAC firmware encountered an error while trying to write to the    **/
            /* radio module's nonvolatile memory region.                             **/

            NONVOLATILE_WRITE_FAILED,

            /* The underlying transport layer detected that there was an overflow    **/
            /* error resulting in one or more bytes of the incoming data being       **/
            /* dropped.  The operation was aborted and all data in the pipeline was  **/
            /* flushed.                                                              **/

            RECEIVE_OVERFLOW,

            /* An unexpected value was returned to this function by the MAC firmware  **/
            UNEXPECTED_VALUE,

            /* The MAC firmware encountered CRC errors while trying to                **/
            /* write to the radio module's nonvolatile memory region.                 **/

            NONVOLATILE_CRC_FAILED,

            /* The MAC firmware encountered unexpected values in the packet header    **/

            NONVOLATILE_PACKET_HEADER,

            /* The MAC firmware received more than the specified maximum packet size  **/

            NONVOLATILE_MAX_PACKET_LENGTH,


            //==============MTI BL Mode=========================================
            FWUPD_WR_FAIL,
            FWUPD_INVALID_PARAM,
            FWUPD_CMD_IGN,
            FWUPD_BNDS,
            FWUPD_MAGIC,
            FWUPD_DATA_LEN ,            
            FWUPD_EXIT_ERR,
            FWUPD_EXIT_SUCCESS,
            FWUPD_EXIT_NOWRITES,
            FWUPD_GEN_RXPKT_ERR,
            FWUPD_INT_MEM_BNDS,
            FWUPD_ENTER_OK,            
            FWUPD_RX_TO,
            FWUPD_CRC_ERR,
            FWUPD_SYS_ERR,
            //Add by Wayne for improve FW upgrade feature for integrate M03X model, 2015-03-31
            FWUPD_STAT_EXIT_MN_ERR,
            FWUPD_STAT_EXIT_UPDKEY_ERR,
            //End by Wayne for improve FW upgrade feature for integrate M03X model, 2015-03-31
        };

        /**
         * See native RFID_REGISTER_TYPE
        **/

        public enum RegisterType : byte
        {
            NORMAL,
            BANKED,
            SELECTOR,
            RESERVED,
            UNKNOWN = 0xFF
        };

        /**
         * See native RFID_REGISTER_ACCESS_TYPE
         * Note:  RegisterAccessType is already defined Explorer app, so need
         *        to avoid name collision, using RegisterProtectionType instead
        **/

        public enum RegisterProtectionType : byte
        {
            READ_WRITE,
            WRITE_ONLY,
            READ_ONLY, 
            RESERVED,
            UNKNOWN = 0xFF
        };


        /**
         * See native RFID_BLOCKWRITE_MODE_*
        **/

        public enum BlockWriteMode : uint
        {
            AUTO,
            FORCE_ONE,
            FORCE_TWO,
            UNKNOWN = 0xFFFF
        };

        /**
         * See native RFID_TAG_SUPPRESSION_*
        **/

        public enum TagSupression : uint
        {
            SUPPRESSION_DISABLED,
            SUPPRESSION_ENABLED,
            UNKNOWN = 0xFFFF
        };

        /**
         * See native RFID_SERIALIZED_TID_*
        **/

        public enum SerializedTid : uint
        {
            SERIALIZED_TID_DISABLED,
            SERIALIZED_TID_ENABLED,
            UNKNOWN = 0xFFFF
        };


        /**
         * See API_MacGetError*
        **/

        public enum ENUM_ERROR_TYPE : byte
        {
            CURRENT_ERROR,
            LAST_ERROR,
            UNKNOWN = 0xFF
        };

    } // Constants end

} // rfid_csharp end

