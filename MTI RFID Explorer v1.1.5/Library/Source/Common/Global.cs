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


using System;
using System.Collections.Generic;
using System.Text;


namespace Global
{

    public enum ENUM_RFID_INFO : uint
    {
        // Version information
        RFID_MAJOR_VERSION   = 1,
        RFID_MINOR_VERSION   = 1,
        RFID_RELEASE_VERSION = 5,

        // Private bits for the library startup flag
        RFID_FLAG_LOG_FILE = 0x80000000,   // Flag for logging to file
        RFID_FLAG_LOG_CONS = 0x40000000,   // Flag for logging to console
        RFID_FLAG_LOG_LEVEL_MASK = 0x0F000000,   // Flag bits for logging level
        RFID_FLAG_LOG_LEVEL_SHIFT = 24            // Number of bits to shift to
        // get bits into low-order bits
    };



    //In Tag Access function(Inventory、Read、Write....), 
    //need to set Post Match flag and Select value.
    public struct TagAccessFlag      //clark 2011.4.25
    {
        public byte RetryCount;
        public byte SelectOpsFlag;
        public byte PostMatchFlag;
        public bool bErrorKeepRunning;
    };


    //OEM Address
    public enum enumOEM_ADDR : uint
    {
        SERIAL_NUM           = 0x000005F,
        HOST_IF_SEL          = 0x000000A0,
        MODEL_NAME_MAIN      = 0x0000000A,
        MODEL_NAME_SUB       = 0x0000000B,
        THRESHOLD            = 0x00001B20, 
        CUSTOMER_REGION_NAME = 0x000022CF,
    }

//===============================Communication=============================
    public enum enumPORT
    {
        ENUM_PORT_USB,
        ENUM_PORT_UART,
    }


    public enum ENUM_DEVICE_INFO: uint
    {
        MTI_VID     = 0x24E9,
        MTI_DEF_PID = 0x1000,
        MTI_PID_824 = 0x0824,
        MTI_PID_861 = 0x0861,
    };


    public enum ENUM_BAUD_RATE : uint
    {
        CBR_110    = 110,
        CBR_300    = 300,
        CBR_600    = 600,
        CBR_1200   = 1200,
        CBR_2400   = 2400,
        CBR_4800   = 4800,
        CBR_9600   = 9600,
        CBR_14400  = 14400,
        CBR_19200  = 19200,
        CBR_38400  = 38400,
        CBR_57600  = 57600,
        CBR_115200 = 115200,
        CBR_128000 = 128000,
        CBR_256000 = 256000
    }

    public enum ENUM_STOP_BIT
    {
        ONESTOPBIT,
        ONE5STOPBITS,
        TWOSTOPBITS
    }


    public enum TRANS_DEV_TYPE : uint
    {
        NO_DEVICE,
        SERIAL,
        USB,
    }

    public enum TRANS_MODE
    {
        INTERRUPT_WITH_OVERLAP,
        INTERRUPT_WITHOUT_OVERLAP,
        REPORT,
    }


    public enum TRANS_RESULT : uint
    {
        FAILURE,
        OK,
        INVALID_HANDLE,
        INVALID_PARAMETER,
        USB_REPORT_COUNT_NOT_MATCH,
        BUFFER_OVERFLOW,
    }



   public enum ENUM_FLOW_CONTROL : byte
    {
        PAUSE,
        RESUME,
        ABORT,
        CANCEL,
        NOTHING
    }



//===============================Protocol=============================
    public enum ENUM_CMD : byte
    {
        //Redio Module Configuration
        CONFIG_SET_DEVICE_ID = 0x0,
        CONFIG_GET_DEVICE_ID,
        CONFIG_SET_OPERATOR_MODE,
        CONFIG_GET_OPERATOR_MODE,
        CONFIG_SET_CURRENT_LINK_PROFILE,
        CONFIG_GET_CURRENT_LINK_PROFILE,
        CONFIG_WRITE_REGISTER,
        CONFIG_READ_REGISTER,
        CONFIG_WRITE_BANKED_REGISTER,
        CONFIG_READ_BANKED_REGISTER,
        CONFIG_READ_REGISTER_INFO,

        //Antenna Port Configuration
        ANTENNA_PORT_SET_STATE = 0x10,
        ANTENNA_PORT_GET_STATE,
        ANTENNA_PORT_SET_CONFIGURATION,
        ANTENNA_PORT_GET_CONFIGURATION,
        ANTENNA_PORT_SET_SENSE_THRESHOLD,
        ANTENNA_PORT_GET_SENSE_THRESHOLD,

        //ISO 18000-6C Tag Select Operation
        l8K6C_SET_ACTIVE_SELECT_CRITERIA = 0x20,
        l8K6C_GET_ACTIVE_SELECT_CRITERIA,
        l8K6C_SET_SELECT_CRITERIA,
        l8K6C_GET_SELECT_CRITERIA,
        l8K6C_SET_SELECT_MASK_DATA,
        l8K6C_GET_SELECT_MASK_DATA,
        l8K6C_SET_POST_MATCH_CRITERIA, 
        l8K6C_GET_POST_MATCH_CRITERIA,
        l8K6C_SET_POST_MATCH_MASK_DATA,
        l8K6C_GET_POST_MATCH_MASK_DATA,

        //ISO 18000-6C Tag Access Parameter
        l8K6C_SET_QUERY_TAG_GROUP = 0x30,
        l8K6C_GET_QUERY_TAG_GROUP,
        l8K6C_SET_CURRENT_SINGULATION_ALGORITHM,
        l8K6C_GET_CURRENT_SINGULATION_ALGORITHM,
        l8K6C_SET_SINGULATION_ALGORITHM_PARAMETERS,
        l8K6C_GET_SINGULATION_ALGORITHM_PARAMETERS,
        l8K6C_SET_TAG_ACCESS_PASSWORD,
        l8K6C_GET_TAG_ACCESS_PASSWORD,   
        l8K6C_TAG_WRITE_DATA_BUFFER,
        l8K6C_TAG_READ_DATA_BUFFER,

        //ISO 18000-6C Tag Access Operation
        l8K6C_TAG_INVENTORY = 0x40,
        l8K6C_TAG_READ,
        l8K6C_TAG_WRITE,
        l8K6C_TAG_KILL,
        l8K6C_TAG_LOCK,
        l8K6C_TAG_MULTIPLE_WRITE,
        l8K6C_TAG_BLOCK_WRITE,
        l8K6C_TAG_BLOCK_ERASE,
        l8K6C_TAG_LARGEREAD,//Add LargeRead command
  
        //Control Operation
        CONTROL_CANCEL = 0x50,
        CONTROL_ABORT,
        CONTROL_PAUSE,
        CONTROL_RESUME,
        CONTROL_SOFT_RESET,
        CONTROL_RESET_TO_BOOT_LOADER,
        CONTROL_SET_POWER_STATE,
        CONTROL_GET_POWER_STATE,

        //Mac; Firmware Access
        MAC_GET_FIRMWARE_VERSION = 0x60,
        MAC_GET_DEBUG,
        MAC_CLEAR_ERROR,
        MAC_GET_ERROR,
        MAC_GET_BOOT_LOADER_VERSION,
        MAC_RESERVED,
        MAC_WRITE_OME_DATA,
        MAC_READ_OME_DATA,
        MAC_BY_PASS_WRITE_REGISTER,
        MAC_BY_PASS_READ_REGISTER,
        MAC_SET_REGION,        
        MAC_GET_REGION,
        MAC_GET_OEM_CFG_VERSION,
        MAC_GET_OEM_CFG_UPDATE_NUMBER,


        //GPIO Access
        GPIO_SET_PINS_CONFIGURATION = 0x70,
        GPIO_GET_PINS_CONFIGURATION,
        GPIO_WRITE_PINS,
        GPIO_READ_PINS,

        //Test support
        TEST_SET_ANTENNA_PORT_CONFIGURATION = 0x80,
        TEST_GET_ANTENNA_PORT_CONFIGURATION,
        TEST_SET_FREQUENCY_CONFIGURATION,
        TEST_GET_FREQUENCY_CONFIGURATION,
        TEST_SET_RANDOM_DATA_PULSE_TIME,
        TEST_GET_RANDOM_DATA_PULSE_TIME,
        TEST_SET_INVENTORY_CONFIGURATION,
        TEST_GET_INVENTORY_CONFIGURATION,
        TEST_TURN_CARRIER_WAVE_ON,
        TEST_TURN_CARRIER_WAVE_OFF,
        TEST_INJECT_RANDOM_DATA,
        TEST_TRANSMIT_RANDOM_DATA,


        //Tag Access Data
        UPDAUE_GET_VERSION = 0xE1,
        UPDAUE_ENTER_UPDATE_MODE,
        UPDAUE_SEND_UPDATE_DATA,
        UPDAUE_COMPLETE,
        UPDAUE_RESET,

        //Tag Access Data
        BEGIN       = 0xF0,
        END         = 0xF1,
        INVENTORY   = 0xF5,
        TAG_ACCESS  = 0xF6,

        //Define by MTI
        TIME_OUT = 0xFE,
        NOTHING  = 0xFF
    }



    public enum ENUM_CMD_RESULT : byte
    {
        //AP Mode
        OK            = 0,
        INVALID_PARAM = 0xF0,
        FAILURE       = 0xFF,

        //BL Mode
        FWUPD_WR_FAIL = 0x02,
        FWUPD_INVALID_PARAM,
        FWUPD_CMD_IGN,
        FWUPD_BNDS,
        FWUPD_MAGIC,
        FWUPD_DATA_LEN ,            
        FWUPD_EXIT_ERR,
        FWUPD_EXIT_SUCCESS,
        FWUPD_EXIT_NOWRITES,
        FWUPD_GEN_RXPKT_ERR,
        FWUPD_INT_MEM_BNDS = 0x0D,
        FWUPD_ENTER_OK,            
        FWUPD_RX_TO = 0x10,
        FWUPD_CRC_ERR,
        FWUPD_SYS_ERR,            
    }


    //packet type
    public enum ENMU_PKT_TYPE : byte
    {
        COMMON,
        TAG_ACCESS,
        UPDATE_ENTER,
        UPDATE_COMMON,
        NO = 0xFF,
    }

    public enum ENMU_PKT_INDEX : byte  //Common packet
    {
        PKT_HEADER     = 0,
        PKT_DEVIDE_ID  = 4,
        PKT_COMMAND_ID = 5,
        PKT_DATA       = 6,
        PKT_CRC        = 14,
        END            = 16,    //Length
        NO             = 0xFF,
    }


    public enum ENMU_UPDATE_INDEX : byte  //Common packet
    {
        PKT_HEADER      = 0,
        PKT_DEVIDE_ID   = 4,
        PKT_MAGIC       = 5,
        PKT_COMMAND_ID  = 7,
        PKT_DATA_LENGTH = 9,
        PKT_RESERVE     = 11,
        PKT_DATA        = 13,
        PKT_DATA_RECMD  = 13,
        PKT_DATA_STATUS = 17,
        PKT_DATA_INFO   = 21,
        PKT_CRC         = 62,
        END             = 64,    //Length
        NO              = 0xFF,
    }


    public enum ENMU_TAG_PKT_INDEX : byte
    {
        PKT_HEADER      =  0,
        PKT_DEVIDE_NUM  =  4,
        PKT_DEVIDE_SEQ  =  5,
        INFO_VER        =  6,
        INFO_FLAG       =  7,
        INFO_TYPE       =  8,
        INFO_LENGTH     = 10,
        INFO_SEQ        = 12,
        INFO_DATA       = 14,
        PKT_CRC         = 62,
        END             = 64,           
        NO              = 0xFF,
    }


    public enum ENMU_PKT_FLOW : byte 
    {
        STEP_HEADER_1,
        STEP_HEADER_2,
        STEP_HEADER_3,
        STEP_HEADER_4,
        STEP_HANDLE,
        NO = 0xFF,
    }


    public enum ENMU_TAG_PKT_TYPE : ushort
    {
        BEGIN      = 0x0000,
        END        = 0x0001, 
        INVENTORY  = 0x0005,
        TAG_ACCESS = 0x0006, 
        TIME_OUT,
        NO         = 0xFFFF,
    }

    public enum ENMU_BEGIN_CMD_TYPE : uint
    {
        ENTER_UPDATE_MODE = 0x00000002,
        l8K6C_INVENTORY   = 0x0000000F,
        l8K6C_READ        = 0x00000010,
        l8K6C_WRITE       = 0x00000011,
        l8K6C_LOCK        = 0x00000012,
        l8K6C_KILL        = 0x00000013,
        l8K6C_BLOCK_ERASE = 0x0000001E,
        l8K6C_BLOCK_WRITE = 0x0000001F,
        l8K6C_QT          = 0x00000023,
        l8K6C_LargeRead = 0x00000034,//Add LargeRead command
    }

    public enum ENMU_TAG_ACCESS : byte
    {
        READ = 0xC2,
        WRITE,
        KILL,
        LOCK,
        ACCESS,
        BLOCK_WRITE,
        BLOCK_ERASE,
        QT,
        //Add LargeRead command
        LARGEREAD = 0xE8,
    }


    public enum ENMU_REV_STATE : byte
    {
        WAIT_DATA,
        REVEICE_DONE,
        RECEIVE_FAULT,
        NO = 0xFF,
    }


//=======================Protocol  value limit==========================
    public class ValueLimit : Object
    {
        //Device ID
        public static readonly byte   CONFIG_DEVICE_ID_MAX = 0xFE;

        //CurrentLinkProfile
        public static readonly byte   CONFIG_LINK_PROFILE_MAX = 0x05;


        //AntennaPortGetState
        public static readonly byte   ANTENNA_STATUS_MIN = 0;
        public static readonly byte   ANTENNA_STATUS_MAX = 1;

        public static readonly UInt32 ANTENNA_SENSE_VALUE_MIN = 0;
        public static readonly UInt32 ANTENNA_SENSE_VALUE_MAX = 0x000FFFFF;
        
        //AntennaPortGetConfiguration
        public static readonly byte   ANTENNA_PHY_PORT_MIN = 0;
        public static readonly byte   ANTENNA_PHY_PORT_MAX = 3;

        public static readonly byte   ANTENNA_LOGICAL_PORT_MIN = 0;
        public static readonly byte   ANTENNA_LOGICAL_PORT_MAX = 15;


        public static readonly UInt16 ANTENNA_POWER_LEVEL_MIN = 0;
        public static readonly UInt16 ANTENNA_POWER_LEVEL_MAX = 330;    

        public static readonly UInt16 ANTENNA_DWELL_TIME_MIN = 0;
        public static readonly UInt16 ANTENNA_DWELL_TIME_MAX = 0xFFFF;  
 

        public static readonly UInt16 ANTENNA_NUMBER_INVENT_OR_CYCLES_MIN = 0;
        public static readonly UInt16 ANTENNA_NUMBER_INVENT_OR_CYCLES_MAX = 0xFFFF;

        //AntennaPortGetSenseThreshold
        public static readonly UInt32 ANTENNA_SENSE_THRESHOLD_MIN = 0;
        public static readonly UInt32 ANTENNA_SENSE_THRESHOLD_MAX = 0x000FFFFF;  


        //Tag Data Buffer
        public static readonly byte   TAG_DATA_BUF_LENTH = 0xFF;
        public static readonly byte   TAG_MULTI_WRITE_DATA_BUF_LENTH = 0x20;

        //l8K6CPostMatchCriteria
        public static readonly UInt16 l8K6C_POST_MATCH_CRITERIA_OFFSET_MAX = 0x01FF;
    }


    public class CGlobalFunc
    {

        public static String uint32ArrayToString
        (
            UInt32[] source,
            UInt32   length
        )
        {
            StringBuilder sb = new StringBuilder();

            sb.Append((Char)((source[0] & 0xFFFF0000) >> 0x10));

            for (int index = 1; index < length; index++)
            {
                sb.Append
                (
                    (Char)(source[index] & 0x0000FFFF)
                );


                sb.Append
                (
                    (Char)((source[index] & 0xFFFF0000) >> 16)
                );

            }

            return sb.ToString();
        }


    }

 

}