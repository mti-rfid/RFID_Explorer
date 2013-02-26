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
 * $Id: Linkage.cs,v 1.33 2010/11/09 23:05:38 dshaheen Exp $
 * 
 * Description: 
 *
 *****************************************************************************
 */



using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;

using System.Runtime.InteropServices;

using System.Collections;
using Global;
using rfid.Constants;
using rfid.Structures;
using Linkage.Properties;



namespace rfid
{


    public partial class Linkage
    {
        [DllImport("winmm.dll", EntryPoint="timeBeginPeriod", SetLastError=true)]
        protected static extern uint TimeBeginPeriod(uint uMilliseconds);

        [DllImport("winmm.dll", EntryPoint="timeEndPeriod", SetLastError=true)]
        protected static extern uint TimeEndPeriod(uint uMilliseconds);


        //Sync        
        private volatile bool              bCheckTimeOut        = false;
        private volatile ENUM_FLOW_CONTROL enumINVENTORY_STATUS = ENUM_FLOW_CONTROL.NOTHING;
        
        //Const define
        private   Byte     DEFINE_TRY_COUNT  = 3;
        
        protected Byte     m_btTryLinkCount  = 0;
        private   Mutex    mutex             = new Mutex();        
        private   DateTime m_timeBefAccess   = default( DateTime );     
        private   DateTime m_timeBefReg      = default( DateTime );     


      
        protected Byte defTryCount
        {
            set
            {
                DEFINE_TRY_COUNT = value;
            }
        }


        //COM Port Number
        public uint uiLibSettingComPort
        {
            get
            {
                return Settings.Default.uiComPort;
            }

            set
            {
                Settings.Default.uiComPort = value;
                Settings.Default.Save();
            }
        }


        public void PrintMagToTxt(String strMsg)
        {

#if _TRACE_OUT_PUT
            mutex.WaitOne();
            System.DateTime currentTime = new System.DateTime();
            currentTime = System.DateTime.Now;

            String strBuf = String.Format("{0:00}:{1:00}:{2:00}:{3:000}  =>  ", currentTime.Hour,
                                                                                currentTime.Minute,
                                                                                currentTime.Second,
                                                                                currentTime.Millisecond);

            strBuf += strMsg;

            Console.WriteLine(strBuf);

#if _TRACE_TO_LOG
            StreamWriter strmWt = File.AppendText(@"API Process.txt");

            strmWt.WriteLine(strBuf);
            strmWt.Flush();
            strmWt.Close();
#endif

            mutex.ReleaseMutex();
#endif
        }


        public Result API_Startup
        (
            [In, Out] LibraryVersion pVersion,
            [In]      LibraryMode    mode
        )
        {

#if _TRACE_OUT_PUT
            PrintMagToTxt("API_Startup");
#endif   

            pVersion.major       = (uint)ENUM_RFID_INFO.RFID_MAJOR_VERSION;
            pVersion.minor       = (uint)ENUM_RFID_INFO.RFID_MINOR_VERSION;
            pVersion.release     = (uint)ENUM_RFID_INFO.RFID_RELEASE_VERSION;

            return Result.OK;
        }



        public Result API_Shutdown
        (
        )
        {

#if _TRACE_OUT_PUT
            PrintMagToTxt("API_Shutdown");
#endif   

            clsPacket.TRANS_API_Close();
            return Result.OK; 
        }

      
        public bool LinkDevice
        (
 
        )
        {

#if _TRACE_OUT_PUT
            PrintMagToTxt("LinkDevice");
#endif   

            bool bGetUSB = false;

            do
            {
                //open USB
                foreach (ENUM_DEVICE_INFO device in Enum.GetValues(typeof(ENUM_DEVICE_INFO)))
                {
                    if
                    (
                        clsPacket.TRANS_API_USB_Open((uint)device, (uint)ENUM_DEVICE_INFO.MTI_VID)
                        ==
                        TRANS_RESULT.OK
                    )
                    {
                       
                        bGetUSB = true;                        
                        break;
                    }
                }


                if (bGetUSB)
                {

    #if _TRACE_OUT_PUT
                    PrintMagToTxt("--Open USB successfully---");
    #endif   
                    break;
                }
                else
                {

    #if _TRACE_OUT_PUT
                    PrintMagToTxt("--Open USB fail---");
    #endif   
                }
               
                  
                //open Serial
                rfid.DCB pDcb = new rfid.DCB();
                pDcb.BaudRate = (uint)ENUM_BAUD_RATE.CBR_115200;
                pDcb.ByteSize = 8;
                pDcb.Parity   = 0;
                pDcb.StopBits = (byte)ENUM_STOP_BIT.ONESTOPBIT;

                if (clsPacket.TRANS_API_Serial_Open(uiLibSettingComPort, ref pDcb) != TRANS_RESULT.OK)
                {

    #if _TRACE_OUT_PUT
                    PrintMagToTxt("--Open Serial fail---");
    #endif   
                    return false;
                }
                else
                {

    #if _TRACE_OUT_PUT
                    PrintMagToTxt("--Open Serial successfully---");
    #endif   
                    break;
                }

            }while( false );


            //Set USB/RS232 Overlap time
            clsPacket.TRANS_API_SetOverlapTime( 400, 400 );


            // clark 2011.6.8
            //If the device is in inventory status, abort it.   
            //Maybe receive many inventory's infomation.
            if( ControlAbort() != Result.OK )
                return false;
            
            System.Threading.Thread.Sleep(500);

            //Clear inventory's infomation buffer
            clsPacket.TRANS_API_ClearBuffer();

            return true;
        }


        public Result API_RetrieveAttachedRadiosList
        (
            [In, Out] RadioEnumeration pRadioEnum,
            [In]      UInt32           flags
        )
        {

#if _TRACE_OUT_PUT
            PrintMagToTxt("API_RetrieveAttachedRadiosList");
#endif   

            
            try
            {

                while (true)
                {
                    pRadioEnum.countRadios = 0;

                    //Connect to the device
                    if (LinkDevice() == false)                     
                        break;   
                    
              
                    String SerialNum = ShowOemData( (ushort)enumOEM_ADDR.SERIAL_NUM );
                    if(SerialNum != null)
                    {
                        pRadioEnum.countRadios = 1;  

        #if _TRACE_OUT_PUT 
                        PrintMagToTxt("---Attach OK---");
        #endif   
                    }
                    else
                    {
                        clsPacket.TRANS_API_Close();

        #if _TRACE_OUT_PUT
                        PrintMagToTxt("---Attach Failure---");
        #endif   
                        break;
                    }
                    
                    ////Use command "Get ID" to comform whether link or not.
                    //if (API_ConfigGetDeviceID(ref deviceID) == Result.OK)
                    //{
                    //    pRadioEnum.countRadios = 1;   
                    //    PrintMagToTxt("---Attach OK---");
                    //}
                    //else
                    //{
                    //    clsPacket.TRANS_API_Close();
                    //    PrintMagToTxt("---Attach Failure---");
                    //    break;
                    //}

                    //Assign ID to structure.
                    pRadioEnum.radioInfo = new RadioInformation[pRadioEnum.countRadios];

                    for (int index = 0; index < pRadioEnum.countRadios; ++index)
                    {

                        pRadioEnum.radioInfo[index] = new RadioInformation();


                        pRadioEnum.radioInfo[index].length   = 21;
                        pRadioEnum.radioInfo[index].idLength = (uint)(4 + SerialNum.Length);

                        pRadioEnum.radioInfo[index].uniqueId = new Byte[pRadioEnum.radioInfo[index].idLength];
                        //Assign initial value
                        for( int i = 0; i< pRadioEnum.radioInfo[index].idLength; i++)
                            pRadioEnum.radioInfo[index].uniqueId[i] = (byte)' ';


                        TRANS_DEV_TYPE enumDev = clsPacket.TRANS_API_AskDevType();
                        if( enumDev == TRANS_DEV_TYPE.NO_DEVICE)
                        {                       
                            break;
                        }
                        else if( enumDev == TRANS_DEV_TYPE.USB)
                        {                        
                            pRadioEnum.radioInfo[index].uniqueId[0] = (byte)'U';
                            pRadioEnum.radioInfo[index].uniqueId[1] = (byte)'S'; 
                            pRadioEnum.radioInfo[index].uniqueId[2] = (byte)'B'; 
                            pRadioEnum.radioInfo[index].uniqueId[3] = (byte)'-';

                            Array.Copy( Encoding.Default.GetBytes( SerialNum.ToCharArray() ),  0,
                                        pRadioEnum.radioInfo[index].uniqueId,                  4,
                                        SerialNum.Length);                               

                        }
                        else if (enumDev == TRANS_DEV_TYPE.SERIAL)
                        {
                            pRadioEnum.radioInfo[index].uniqueId[0] = (byte)'S';
                            pRadioEnum.radioInfo[index].uniqueId[1] = (byte)'E';
                            pRadioEnum.radioInfo[index].uniqueId[2] = (byte)'R';
                            pRadioEnum.radioInfo[index].uniqueId[3] = (byte)'-';

                            Array.Copy( Encoding.Default.GetBytes( SerialNum.ToCharArray() ),  0,
                                        pRadioEnum.radioInfo[index].uniqueId,                  4,
                                        SerialNum.Length);    
                        }
                    }

                    return Result.OK;  
                             
                }//while

            }
            catch (OutOfMemoryException)
            {
                return Result.OUT_OF_MEMORY;
            }

            return Result.FAILURE;
        }




        public Result API_RadioOpenSerial
        (
            [In]     uint    uiComPort,
            [In] ref DCB     r_pDcb
        )
        {

#if _TRACE_OUT_PUT
            PrintMagToTxt("API_RadioOpenSerial");
#endif   

            if ( clsPacket.TRANS_API_Serial_Open(uiComPort, ref r_pDcb) == TRANS_RESULT.OK )
                return Result.OK; 
            else
                return Result.FAILURE; 
        }


        public Result API_RadioOpenUSB
        (
            [In]          uint uiPID,
            [In]          uint uiVID
        )
        {

#if _TRACE_OUT_PUT
            PrintMagToTxt("API_RadioOpenUSB");
#endif   

            if( clsPacket.TRANS_API_USB_Open(uiPID, uiVID) == TRANS_RESULT.OK )
                return Result.OK; 
            else
                return Result.FAILURE; 
        }


        public Result API_RadioClose
        (

        )
        {

#if _TRACE_OUT_PUT
            PrintMagToTxt("API_RadioClose");
#endif   

            clsPacket.TRANS_API_Close();
            return Result.OK;
        }


        public Result API_RadioEnterConfigMenu
        (

        )
        {

#if _TRACE_OUT_PUT
            PrintMagToTxt("API_RadioEnterConfigMenu");
#endif   

            //Clear Memory data. Avoid many invenory or other to full in memory and can't 
            //catch corrent packet.
            clsPacket.TRANS_API_ClearBuffer();
            return Result.OK;
        }


//==============================Redio Module Configuration==============================

        public Result API_ConfigSetDeviceID
        (
            [In] byte r_DeviceID
        )
        {

#if _TRACE_OUT_PUT
            PrintMagToTxt("API_ConfigSetDeviceID");
#endif   

            //Check range. 0xFF is board channel.
            if (r_DeviceID > ValueLimit.CONFIG_DEVICE_ID_MAX)
                return Result.INVALID_PARAMETER;

            Result   result = Result.OK;
            ENUM_CMD TxCmd  = ENUM_CMD.NOTHING;
            byte[]   buf    = new byte[10];
            Array.Clear(buf, 0, buf.Length);

            TxCmd  = ENUM_CMD.CONFIG_SET_DEVICE_ID;
            buf[0] = (byte)TxCmd;
            buf[1] = r_DeviceID;

            if ( false == SendData(buf) )
                return Result.FAILURE;

            Array.Clear(buf, 0, buf.Length);

            //Check receive data
            if ( false == ReceiveData( TxCmd, ref buf ) )
                return Result.FAILURE;

            //Check result
            result = ConvertResult(buf[1]);
            if (Result.OK != result)
                return result;


            return Result.OK;
        }



        public Result API_ConfigGetDeviceID
        (
            [In, Out] ref byte r_DeviceID
        )
        {

#if _TRACE_OUT_PUT
            PrintMagToTxt("API_ConfigGetDeviceID");
#endif   

            Result   result = Result.OK;
            ENUM_CMD TxCmd  = ENUM_CMD.NOTHING;
            byte[]   buf    = new byte[10];
            Array.Clear(buf, 0, buf.Length);

            TxCmd  = ENUM_CMD.CONFIG_GET_DEVICE_ID;
            buf[0] = (byte)TxCmd;

            if ( false == SendData(buf) )
                return Result.FAILURE;

            Array.Clear(buf, 0, buf.Length);

            //Check receive data
            if ( false == ReceiveData( TxCmd, ref buf ) )
                return Result.FAILURE;

            //Check result
            result = ConvertResult(buf[1]);
            if (Result.OK != result)
                return result;

            r_DeviceID = buf[2];

            return Result.OK;
        }



        public Result API_ConfigSetOperationMode
        (
            [In] RadioOperationMode r_Mode
        )
        {

#if _TRACE_OUT_PUT
            PrintMagToTxt("API_ConfigSetOperationMode");
#endif   

            Result   result = Result.OK;
            ENUM_CMD TxCmd  = ENUM_CMD.NOTHING;
            byte[]   buf    = new byte[10];
            Array.Clear(buf, 0, buf.Length);

            TxCmd  = ENUM_CMD.CONFIG_SET_OPERATOR_MODE;
            buf[0] = (byte)TxCmd;
            buf[1] = (byte)r_Mode;

            if ( false == SendData(buf) )
                return Result.FAILURE;

            Array.Clear(buf, 0, buf.Length);

            //Check receive data
            if ( false == ReceiveData( TxCmd, ref buf ) )
                return Result.FAILURE;

            //Check result
            result = ConvertResult(buf[1]);
            if (Result.OK != result)
                return result;

            return Result.OK;
        }




        public Result API_ConfigGetOperationMode
        (
            [In, Out] ref RadioOperationMode r_Mode
        )
        {

#if _TRACE_OUT_PUT
            PrintMagToTxt("API_ConfigGetOperationMode");
#endif   

            Result   result = Result.OK;
            ENUM_CMD TxCmd  = ENUM_CMD.NOTHING;
            byte[]   buf    = new byte[10];
            Array.Clear(buf, 0, buf.Length);

            r_Mode = RadioOperationMode.UNKNOWN;

            TxCmd  = ENUM_CMD.CONFIG_GET_OPERATOR_MODE;
            buf[0] = (byte)TxCmd;

            if ( false == SendData(buf) )
                return Result.FAILURE;

            Array.Clear(buf, 0, buf.Length);

            //Check receive data
            if ( false == ReceiveData( TxCmd, ref buf ) )
                return Result.FAILURE;

            //Check result
            result = ConvertResult(buf[1]);
            if (Result.OK != result)
                return result;

            r_Mode = (RadioOperationMode)buf[2];

            return Result.OK;
        }




        public Result API_ConfigSetCurrentLinkProfile
        (
            [In] byte r_Profile
        )
        {

#if _TRACE_OUT_PUT
            PrintMagToTxt("API_ConfigSetCurrentLinkProfile");
#endif   

            Result   result = Result.OK;
            ENUM_CMD TxCmd  = ENUM_CMD.NOTHING;
            byte[]           buf   = new byte[10];
            Array.Clear(buf, 0, buf.Length);

            TxCmd  = ENUM_CMD.CONFIG_SET_CURRENT_LINK_PROFILE;
            buf[0] = (byte)TxCmd;
            buf[1] = r_Profile;

            if ( false == SendData(buf) )
                return Result.FAILURE;

            Array.Clear(buf, 0, buf.Length);

            //Check receive data
            if ( false == ReceiveData( TxCmd, ref buf ) )
                return Result.FAILURE;

            //Check result
            result = ConvertResult(buf[1]);
            if (Result.OK != result)
                return result;

            return Result.OK;
        }




        public Result API_ConfigGetCurrentLinkProfile
        (
            [In, Out] ref byte r_Profile
        )
        {

#if _TRACE_OUT_PUT
            PrintMagToTxt("API_ConfigGetCurrentLinkProfile");
#endif   

            Result   result = Result.OK;
            ENUM_CMD TxCmd  = ENUM_CMD.NOTHING;
            byte[]   buf    = new byte[10];
            Array.Clear(buf, 0, buf.Length);

            TxCmd  = ENUM_CMD.CONFIG_GET_CURRENT_LINK_PROFILE;
            buf[0] = (byte)TxCmd;

            if ( false == SendData(buf) )
                return Result.FAILURE;

            Array.Clear(buf, 0, buf.Length);

            //Check receive data
            if ( false == ReceiveData( TxCmd, ref buf ) )
                return Result.FAILURE;

            //Check result
            result = ConvertResult(buf[1]);
            if (Result.OK != result)
                return result;

            r_Profile = buf[2];

            //check return valu
            if ( r_Profile > ValueLimit.CONFIG_LINK_PROFILE_MAX )
            {
                r_Profile = 0;
                return Result.UNEXPECTED_VALUE;
            }

            return Result.OK;
        }



        public Result API_ConfigGetLinkProfile
        (
            [In]      UInt32           profileIdx,
            [In, Out] RadioLinkProfile profileInfo
        )
        {

#if _TRACE_OUT_PUT
            PrintMagToTxt("API_ConfigGetLinkProfile");
#endif   

            // regionComboBox
            IniFile clsIni   = null;
            string  strIndex = profileIdx.ToString();

            clsIni = new IniFile(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase
                                        + "LinkProfile.ini");

            if (null == clsIni)     return Result.FAILURE;           


            profileInfo.enabled               = clsIni.ReadInteger(strIndex, "Enabled", 0); 
            profileInfo.profileId             = clsIni.ReadInteger64(strIndex, "ProfileId", 0);
            profileInfo.profileVersion        = clsIni.ReadInteger(strIndex, "ProfileVersion", 0);

            profileInfo.denseReaderMode       = clsIni.ReadInteger(strIndex, "DenseReaderMode", 0);
            profileInfo.widebandRssiSamples   = clsIni.ReadInteger(strIndex, "WidebandRssiSamples", 0);
            profileInfo.narrowbandRssiSamples = clsIni.ReadInteger(strIndex, "NarrowbandRssiSamples", 0);

            profileInfo.realtimeRssiEnabled           = clsIni.ReadInteger(strIndex, "RealtimeRssiEnabled", 0);
            profileInfo.realtimeWidebandRssiSamples   = clsIni.ReadInteger(strIndex, "RealtimeWidebandRssiSamples", 0);
            profileInfo.realtimeNarrowbandRssiSamples = clsIni.ReadInteger(strIndex, "RealtimeNarrowbandRssiSamples", 0); 


            if (RadioProtocol.ISO18K6C == profileInfo.profileProtocol)
            {
                RadioLinkProfileConfig_ISO18K6C profileConfig =
                    new RadioLinkProfileConfig_ISO18K6C();

                profileConfig.data01Difference   = (DataDifference)clsIni.ReadInteger(strIndex, "Data01Difference", 0);
                profileConfig.divideRatio        = (DivideRatio)clsIni.ReadInteger(strIndex, "DivideRatio", 0);
                profileConfig.millerNumber       = (MillerNumber)clsIni.ReadInteger(strIndex, "MillerNumber", 0);
                profileConfig.minT2Delay         = clsIni.ReadInteger(strIndex, "MinT2Delay", 0);
                profileConfig.modulationType     = (ModulationType)clsIni.ReadInteger(strIndex, "ModulationType", 0);
                profileConfig.pulseWidth         = clsIni.ReadInteger(strIndex, "PulseWidth", 0);
                profileConfig.rtCalibration      = clsIni.ReadInteger(strIndex, "RtCalibration", 0);
                profileConfig.rxDelay            = clsIni.ReadInteger(strIndex, "RxDelay", 0);
                profileConfig.tari               = clsIni.ReadInteger(strIndex, "Tari", 0);
                profileConfig.trCalibration      = clsIni.ReadInteger(strIndex, "TrCalibration", 0);
                profileConfig.trLinkFrequency    = clsIni.ReadInteger(strIndex, "TrLinkFrequency", 0);
                profileConfig.txPropagationDelay = clsIni.ReadInteger(strIndex, "TxPropagationDelay", 0);
                profileConfig.varT2Delay         = clsIni.ReadInteger(strIndex, "VarT2Delay", 0); 

                profileInfo.profileConfig = profileConfig;
            }
            else
            {
                // This will set the value of profile.profileProtocol field
                // to be set to UNKNOWN - i.e. library / linkage mis-match

                profileInfo.profileConfig = new ProfileConfig();
            }

            return Result.OK;
        }



        public Result API_ConfigWriteRegister
        (
            [In] UInt16 r_MacVirtualRegAddr,
            [In] UInt32 r_Value
        )
        {

#if _TRACE_OUT_PUT
            PrintMagToTxt("API_ConfigWriteRegister");
#endif   

            Result   result = Result.OK;
            ENUM_CMD TxCmd  = ENUM_CMD.NOTHING;
            byte[]   buf    = new byte[10];
            Array.Clear(buf, 0, buf.Length);

            TxCmd  = ENUM_CMD.CONFIG_WRITE_REGISTER;
            buf[0] = (byte)TxCmd;
            buf[1] = (byte)(  r_MacVirtualRegAddr       & 0xFF);
            buf[2] = (byte)( (r_MacVirtualRegAddr >> 8) & 0xFF);
            buf[3] = (byte)(  r_Value        & 0xFF );
            buf[4] = (byte)( (r_Value >>  8) & 0xFF );
            buf[5] = (byte)( (r_Value >> 16) & 0xFF );
            buf[6] = (byte)( (r_Value >> 24) & 0xFF );

            if ( false == SendData(buf) )
                return Result.FAILURE;

            Array.Clear(buf, 0, buf.Length);

            //Check receive data
            if ( false == ReceiveData( TxCmd, ref buf ) )
                return Result.FAILURE;

            //Check result
            result = ConvertResult(buf[1]);
            if (Result.OK != result)
                return result;

            return Result.OK;
        }



        public Result API_ConfigReadRegister
        (
            [In]          UInt16 r_MacVirtualRegAddr,
            [In, Out] ref UInt32 r_Value
        )
        {

#if _TRACE_OUT_PUT
            PrintMagToTxt("API_ConfigReadRegister");
#endif   

            Result   result = Result.OK;
            ENUM_CMD TxCmd  = ENUM_CMD.NOTHING;
            byte[]   buf    = new byte[10];
            Array.Clear(buf, 0, buf.Length);

            TxCmd  = ENUM_CMD.CONFIG_READ_REGISTER;
            buf[0] = (byte)TxCmd;
            buf[1] = (byte)(  r_MacVirtualRegAddr       & 0xFF );
            buf[2] = (byte)( (r_MacVirtualRegAddr >> 8) & 0xFF );

            if ( false == SendData(buf) )
                return Result.FAILURE;

            Array.Clear(buf, 0, buf.Length);

            //Check receive data
            if ( false == ReceiveData( TxCmd, ref buf ) )
                return Result.FAILURE;

            //Check result
            result = ConvertResult(buf[1]);
            if (Result.OK != result)
                return result;

            r_Value = BitConverter.ToUInt32(buf, 2);

            return Result.OK;
        }




        public Result API_ConfigWriteBankedRegister
        (
            [In] UInt16 r_Addr,
            [In] UInt16 r_BankSelector,
            [In] UInt32 r_Value
        )
        {

#if _TRACE_OUT_PUT
            PrintMagToTxt("API_ConfigWriteBankedRegister");
#endif   

            Result   result = Result.OK;
            ENUM_CMD TxCmd  = ENUM_CMD.NOTHING;
            byte[]   buf    = new byte[10];
            Array.Clear(buf, 0, buf.Length);

            TxCmd  = ENUM_CMD.CONFIG_WRITE_BANKED_REGISTER;
            buf[0] = (byte)TxCmd;
            buf[1] = (byte)(  r_Addr       & 0xFF );
            buf[2] = (byte)( (r_Addr >> 8) & 0xFF );

            buf[3] = (byte)(  r_BankSelector       & 0xFF );
            buf[4] = (byte)( (r_BankSelector >> 8) & 0xFF );

            buf[5] = (byte)( (r_Value      ) & 0xFF );
            buf[6] = (byte)( (r_Value >>  8) & 0xFF );
            buf[7] = (byte)( (r_Value >> 16) & 0xFF );
            buf[8] = (byte)( (r_Value >> 24) & 0xFF );

            if ( false == SendData(buf) )
                return Result.FAILURE;

            Array.Clear(buf, 0, buf.Length);

            //Check receive data
            if ( false == ReceiveData( TxCmd, ref buf ) )
                return Result.FAILURE;

            //Check result
            result = ConvertResult(buf[1]);
            if (Result.OK != result)
                return result;

            return Result.OK;
        }




        public Result API_ConfigReadBankedRegister
        (
            [In]          UInt16 r_Addr,
            [In]          UInt16 r_BankSelector,
            [In, Out] ref UInt32 r_Value
        )
        {

#if _TRACE_OUT_PUT
            PrintMagToTxt("API_ConfigReadBankedRegister");
#endif   

            Result   result = Result.OK;
            ENUM_CMD TxCmd  = ENUM_CMD.NOTHING;
            byte[]   buf    = new byte[10];
            Array.Clear(buf, 0, buf.Length);

            TxCmd  = ENUM_CMD.CONFIG_READ_BANKED_REGISTER;
            buf[0] = (byte)TxCmd;
            buf[1] = (byte)(  r_Addr       & 0xFF );
            buf[2] = (byte)( (r_Addr >> 8) & 0xFF );

            buf[3] = (byte)(  r_BankSelector       & 0xFF );
            buf[4] = (byte)( (r_BankSelector >> 8) & 0xFF );

            if ( false == SendData(buf) )
                return Result.FAILURE;

            Array.Clear(buf, 0, buf.Length);

            //Check receive data
            if ( false == ReceiveData( TxCmd, ref buf ) )
                return Result.FAILURE;

            //Check result
            result = ConvertResult(buf[1]);
            if (Result.OK != result)
                return result;

            r_Value = BitConverter.ToUInt32(buf, 2);

            return Result.OK;
        }


        public Result API_ConfigReadRegisterInfo
        (
            [In]      UInt16       r_Addr,
            [Out] out RegisterInfo r_pInfo
        )
        {

#if _TRACE_OUT_PUT
            PrintMagToTxt("API_ConfigReadRegisterInfo");
#endif   

            //initial
            r_pInfo = new RegisterInfo();
            
            Result   result = Result.OK;
            ENUM_CMD TxCmd  = ENUM_CMD.NOTHING;
            byte[]   buf    = new byte[10];
            Array.Clear(buf, 0, buf.Length);


            TxCmd  = ENUM_CMD.CONFIG_READ_REGISTER_INFO;
            buf[0] = (byte)TxCmd;
            buf[1] = (byte)(  r_Addr       & 0xFF );
            buf[2] = (byte)( (r_Addr >> 8) & 0xFF );

            if ( false == SendData(buf) )
                return Result.FAILURE;

            Array.Clear(buf, 0, buf.Length);

            //Check receive data
            if ( false == ReceiveData( TxCmd, ref buf ) )
                return Result.FAILURE;

            //Check result
            result = ConvertResult(buf[1]);
            if (Result.OK != result)
                return result;


            r_pInfo.type            = (RegisterType)buf[2];
            r_pInfo.accessType      = (RegisterProtectionType)buf[3];
            r_pInfo.bankSize        = buf[4];
            r_pInfo.selectorAddress = BitConverter.ToUInt16(buf, 5);
            r_pInfo.currentSelector = BitConverter.ToUInt16(buf, 7);
                
            return Result.OK;
        }



//=========================Antenna Port Configuration============================

        public Result API_AntennaPortSetState
        (
            [In] byte             r_Port,
            [In] AntennaPortState r_State
        )
        {

#if _TRACE_OUT_PUT
            PrintMagToTxt("API_AntennaPortSetState");
#endif   

            Result   result = Result.OK;
            ENUM_CMD TxCmd  = ENUM_CMD.NOTHING;
            byte[]   buf    = new byte[10];
            Array.Clear(buf, 0, buf.Length);

            TxCmd  = ENUM_CMD.ANTENNA_PORT_SET_STATE;
            buf[0] = (byte)TxCmd;
            buf[1] = r_Port;
            buf[2] = (byte)r_State;

            if ( false == SendData(buf) )
                return Result.FAILURE;

            Array.Clear(buf, 0, buf.Length);

            //Check receive data
            if ( false == ReceiveData( TxCmd, ref buf ) )
                return Result.FAILURE;

            //Check result
            result = ConvertResult(buf[1]);
            if (Result.OK != result)
                return result;

            return Result.OK;
        }


        public Result API_AntennaPortGetState
        (
            [In]          byte              r_Port,
            [In, Out] ref AntennaPortStatus r_strcStatus
        )
        {

#if _TRACE_OUT_PUT
            PrintMagToTxt("API_AntennaPortGetState");
#endif   

            //initial
            r_strcStatus.state             = AntennaPortState.UNKNOWN;
            r_strcStatus.antennaSenseValue = 0;
            
            
            Result   result = Result.OK;
            ENUM_CMD TxCmd  = ENUM_CMD.NOTHING;
            byte[]   buf    = new byte[10];
            Array.Clear(buf, 0, buf.Length);           


            TxCmd  = ENUM_CMD.ANTENNA_PORT_GET_STATE;
            buf[0] = (byte)TxCmd;
            buf[1] = r_Port;

            if ( false == SendData(buf) )
                return Result.FAILURE;

            Array.Clear(buf, 0, buf.Length);

            //Check receive data
            if ( false == ReceiveData( TxCmd, ref buf ) )
                return Result.FAILURE;

            //Check result
            result = ConvertResult(buf[1]);
            if (Result.OK != result)
                return result;


            r_strcStatus.state             = (AntennaPortState)buf[2];
            r_strcStatus.antennaSenseValue = BitConverter.ToUInt32(buf, 3);


            //Check value limit
            if ( r_strcStatus.state > (AntennaPortState)Global.ValueLimit.ANTENNA_STATUS_MAX ||
                 r_strcStatus.state < (AntennaPortState)Global.ValueLimit.ANTENNA_STATUS_MIN    )
            {
                r_strcStatus.state = AntennaPortState.UNKNOWN;
                result = Result.UNEXPECTED_VALUE;
            }

            if (r_strcStatus.antennaSenseValue > Global.ValueLimit.ANTENNA_SENSE_VALUE_MAX)
            {
                r_strcStatus.antennaSenseValue = Global.ValueLimit.ANTENNA_SENSE_VALUE_MAX;
                result = Result.UNEXPECTED_VALUE;
            }
                
           if (r_strcStatus.antennaSenseValue < Global.ValueLimit.ANTENNA_SENSE_VALUE_MIN)
            {
                r_strcStatus.antennaSenseValue = Global.ValueLimit.ANTENNA_SENSE_VALUE_MIN;
                result = Result.UNEXPECTED_VALUE;
            }

           return result;
        }



        public Result API_AntennaPortSetConfiguration
        ( 
            [In] byte              r_Port,
            [In] AntennaPortConfig r_Config
        )
        {

#if _TRACE_OUT_PUT
            PrintMagToTxt("API_AntennaPortSetConfiguration");
#endif   

            Result   result = Result.OK;
            ENUM_CMD TxCmd  = ENUM_CMD.NOTHING;
            byte[]   buf    = new byte[10];
            Array.Clear(buf, 0, buf.Length);

            TxCmd  = ENUM_CMD.ANTENNA_PORT_SET_CONFIGURATION;
            buf[0] = (byte)TxCmd;
            buf[1] = r_Port;

            buf[2] = (byte)(  r_Config.powerLevel       & 0xFF );
            buf[3] = (byte)( (r_Config.powerLevel >>8 ) & 0xFF );

            buf[4] = (byte)(  r_Config.dwellTime       & 0xFF );
            buf[5] = (byte)( (r_Config.dwellTime >> 8) & 0xFF );

            buf[6] = (byte)(  r_Config.numberInventoryCycles       & 0xFF );
            buf[7] = (byte)( (r_Config.numberInventoryCycles >> 8) & 0xFF );

            buf[8] = r_Config.physicalPort;

            if ( false == SendData(buf) )
                return Result.FAILURE;

            Array.Clear(buf, 0, buf.Length);

            //Check receive data
            if ( false == ReceiveData( TxCmd, ref buf ) )
                return Result.FAILURE;

            //Check result
            result = ConvertResult(buf[1]);
            if (Result.OK != result)
                return result;         

            return Result.OK;
        }




        public Result API_AntennaPortGetConfiguration
        (
            [In]          byte              r_Port,
            [In, Out] ref AntennaPortConfig r_Config
        )
        {

#if _TRACE_OUT_PUT
            PrintMagToTxt("AntennaPortGetConfiguration");
#endif   

            Result   result = Result.OK;
            ENUM_CMD TxCmd  = ENUM_CMD.NOTHING;
            byte[]   buf    = new byte[10];
            Array.Clear(buf, 0, buf.Length);

            TxCmd  = ENUM_CMD.ANTENNA_PORT_GET_CONFIGURATION;
            buf[0] = (byte)TxCmd;
            buf[1] = r_Port;

            if ( false == SendData(buf) )
                return Result.FAILURE;

            Array.Clear(buf, 0, buf.Length);

            //Check receive data
            if ( false == ReceiveData( TxCmd, ref buf ) )
                return Result.FAILURE;

            //Check result
            result = ConvertResult(buf[1]);
            if (Result.OK != result)
                return result;


            r_Config.powerLevel             = BitConverter.ToUInt16(buf, 2);
            r_Config.dwellTime              = BitConverter.ToUInt16(buf, 4);
            r_Config.numberInventoryCycles  = BitConverter.ToUInt16(buf, 6);
            r_Config.physicalPort           = buf[8];

            //Check value limit
            if ( r_Config.powerLevel > Global.ValueLimit.ANTENNA_POWER_LEVEL_MAX )
            {
                r_Config.powerLevel = Global.ValueLimit.ANTENNA_POWER_LEVEL_MAX;
                result              = Result.UNEXPECTED_VALUE;
            }

            if ( r_Config.powerLevel < Global.ValueLimit.ANTENNA_POWER_LEVEL_MIN )
            {
                r_Config.powerLevel = Global.ValueLimit.ANTENNA_POWER_LEVEL_MIN;
                result              = Result.UNEXPECTED_VALUE;
            }
                
            if ( r_Config.dwellTime > Global.ValueLimit.ANTENNA_DWELL_TIME_MAX )
            {
                r_Config.dwellTime = Global.ValueLimit.ANTENNA_DWELL_TIME_MAX;
                result             = Result.UNEXPECTED_VALUE;
            }

            if ( r_Config.dwellTime < Global.ValueLimit.ANTENNA_DWELL_TIME_MIN )
            {
                r_Config.dwellTime = Global.ValueLimit.ANTENNA_DWELL_TIME_MIN;
                result             = Result.UNEXPECTED_VALUE;
            }

            if ( r_Config.numberInventoryCycles > Global.ValueLimit.ANTENNA_NUMBER_INVENT_OR_CYCLES_MAX )
            {
                r_Config.numberInventoryCycles = Global.ValueLimit.ANTENNA_NUMBER_INVENT_OR_CYCLES_MAX;
                result                         = Result.UNEXPECTED_VALUE;
            }

            if ( r_Config.numberInventoryCycles < Global.ValueLimit.ANTENNA_NUMBER_INVENT_OR_CYCLES_MIN )
            {
                r_Config.numberInventoryCycles = Global.ValueLimit.ANTENNA_NUMBER_INVENT_OR_CYCLES_MIN;
                result                         = Result.UNEXPECTED_VALUE;
            }

            if ( r_Config.physicalPort > Global.ValueLimit.ANTENNA_PHY_PORT_MAX )
            {
                r_Config.physicalPort = Global.ValueLimit.ANTENNA_PHY_PORT_MAX;
                result                  = Result.UNEXPECTED_VALUE;
            }

            if ( r_Config.physicalPort < Global.ValueLimit.ANTENNA_PHY_PORT_MIN )
            {
                r_Config.physicalPort = Global.ValueLimit.ANTENNA_PHY_PORT_MIN;
                result                  = Result.UNEXPECTED_VALUE;
            }

            return result;
        }



        public Result API_AntennaPortSetSenseThreshold
        (
            [In] UInt32 r_AntennaSenseThreshold
        )
        {

#if _TRACE_OUT_PUT
            PrintMagToTxt("AntennaPortSetSenseThreshold");
#endif   

            Result   result = Result.OK;
            ENUM_CMD TxCmd  = ENUM_CMD.NOTHING;
            byte[]   buf    = new byte[10];
            Array.Clear(buf, 0, buf.Length);

            TxCmd  = ENUM_CMD.ANTENNA_PORT_SET_SENSE_THRESHOLD;
            buf[0] = (byte)TxCmd;
            buf[1] = (byte)(  r_AntennaSenseThreshold        & 0xFF );
            buf[2] = (byte)( (r_AntennaSenseThreshold >>  8) & 0xFF );
            buf[3] = (byte)( (r_AntennaSenseThreshold >> 16) & 0xFF );
            buf[4] = (byte)( (r_AntennaSenseThreshold >> 24) & 0xFF );

            if ( false == SendData(buf) )
                return Result.FAILURE;

            Array.Clear(buf, 0, buf.Length);

            //Check receive data
            if ( false == ReceiveData( TxCmd, ref buf ) )
                return Result.FAILURE;

            //Check result
            result = ConvertResult(buf[1]);
            if (Result.OK != result)
                return result;

            return Result.OK;
        }


        public Result API_AntennaPortGetSenseThreshold
        (
            [In, Out] ref UInt32 r_AntennaSenseThreshold
        )
        {

#if _TRACE_OUT_PUT
            PrintMagToTxt("AntennaPortGetSenseThreshold");
#endif   

            Result   result = Result.OK;
            ENUM_CMD TxCmd  = ENUM_CMD.NOTHING;
            byte[]   buf    = new byte[10];
            Array.Clear(buf, 0, buf.Length);

            TxCmd  = ENUM_CMD.ANTENNA_PORT_GET_SENSE_THRESHOLD;
            buf[0] = (byte)TxCmd;

            if ( false == SendData(buf) )
                return Result.FAILURE;

            Array.Clear(buf, 0, buf.Length);

            //Check receive data
            if ( false == ReceiveData( TxCmd, ref buf ) )
                return Result.FAILURE;

            //Check result
            result = ConvertResult(buf[1]);
            if (Result.OK != result)
                return result;

            r_AntennaSenseThreshold = BitConverter.ToUInt32(buf, 2);

            //Check value limit
            if (r_AntennaSenseThreshold > Global.ValueLimit.ANTENNA_SENSE_THRESHOLD_MAX)
            {
                r_AntennaSenseThreshold = Global.ValueLimit.ANTENNA_SENSE_THRESHOLD_MAX;
                return Result.UNEXPECTED_VALUE;
            }

            if (r_AntennaSenseThreshold < Global.ValueLimit.ANTENNA_SENSE_THRESHOLD_MIN)
            {
                r_AntennaSenseThreshold = Global.ValueLimit.ANTENNA_SENSE_THRESHOLD_MIN;
                return Result.UNEXPECTED_VALUE;
            }

            return Result.OK;
        }



//===========================ISO 18000-6C Tag Access================================ 

        public Result API_l8K6CSetSelectCriteria
        (
            [In] SelectCriteria strcData
        )
        {

#if _TRACE_OUT_PUT
            PrintMagToTxt("API_l8K6CSetSelectCriteria");
#endif   

            //Clear active enable flag
            for (byte index = 0; index < 8; index++)
            {
                if ( Result.OK != API_l8K6CSetActiveSelectCriteria(index, (byte)AvtiveEnable.DISENABLE) )
                    return Result.FAILURE;
            }


            for (byte index = 0; index < strcData.countCriteria; index++)
            {
                //Set Mask Data
                if ( Result.OK != API_l8K6CSetSelectMaskData(index, strcData) )
                    return Result.FAILURE;

                //Set Select Criteria Parameter
                if ( Result.OK != l8K6CSetSelectCriteria(index, strcData) )
                    return Result.FAILURE;

                //Enable active enable flag
                if ( Result.OK != API_l8K6CSetActiveSelectCriteria(index, (byte)AvtiveEnable.ENABLE) )
                    return Result.FAILURE;
                
            }//end for  

            return Result.OK;
        }


        public Result l8K6CSetSelectCriteria
        (
            [In] byte           btIndex,
            [In] SelectCriteria strcData
        )
        {

#if _TRACE_OUT_PUT
            PrintMagToTxt("l8K6CSetSelectCriteria");
#endif   
            
            Result   result = Result.OK;
            ENUM_CMD TxCmd  = ENUM_CMD.NOTHING;
            byte[]   buf    = new byte[10];

            TxCmd  = ENUM_CMD.l8K6C_SET_SELECT_CRITERIA;
            buf[0] = (byte)TxCmd;
            buf[1] = btIndex;
            buf[2] = (byte)strcData.pCriteria[btIndex].mask.bank;
            buf[3] = (byte)(  strcData.pCriteria[btIndex].mask.offset       & 0xFF);
            buf[4] = (byte)( (strcData.pCriteria[btIndex].mask.offset >> 8) & 0xFF);
            buf[5] = strcData.pCriteria[btIndex].mask.count;
            buf[6] = (byte)strcData.pCriteria[btIndex].action.target;
            buf[7] = (byte)strcData.pCriteria[btIndex].action.action;
            buf[8] = strcData.pCriteria[btIndex].action.enableTruncate;

            if (false == SendData(buf))
                return Result.FAILURE;

            Array.Clear(buf, 0, buf.Length);

            //Check receive data
            if ( false == ReceiveData(TxCmd, ref buf) )
                return Result.FAILURE;

            //Check result
            result = ConvertResult(buf[1]);
            if (Result.OK != result)
                return result;

            return Result.OK;
        }


        public Result API_l8K6CSetActiveSelectCriteria
        (
            [In] byte r_btIndex,
            [In] byte r_btEnableFlag
        )
        {

#if _TRACE_OUT_PUT
            PrintMagToTxt("l8K6CSetActiveSelectCriteria");
#endif   

            Result   result = Result.OK;
            ENUM_CMD TxCmd  = ENUM_CMD.NOTHING;
            byte[]   buf    = new byte[10];
            Array.Clear(buf, 0, buf.Length);

            TxCmd  = ENUM_CMD.l8K6C_SET_ACTIVE_SELECT_CRITERIA;
            buf[0] = (byte)TxCmd;
            buf[1] = r_btIndex;
            buf[2] = r_btEnableFlag;

            if ( false == SendData(buf) )
                return Result.FAILURE;

            Array.Clear(buf, 0, buf.Length);

            //Check receive data
            if ( false == ReceiveData( TxCmd, ref buf ) )
                return Result.FAILURE;

            //Check result
            result = ConvertResult(buf[1]);
            if (Result.OK != result)
                return result;

            return Result.OK;
        }


        public Result API_l8K6CSetSelectMaskData
        (
            [In] byte           r_btSelectCriteria,
            [In] SelectCriteria strcData
        )
        {

#if _TRACE_OUT_PUT
            PrintMagToTxt("API_l8K6CSetSelectMaskData");
#endif   

            Result result = Result.OK;

            //Every criteria has 32 bytes mask data. One time set 4 bytes,so need to set 8 times.
            for (byte btIndex = 0; btIndex < 8; btIndex++)
            {
                result = l8K6CSetSelectMaskData(r_btSelectCriteria, btIndex, strcData);

                if (Result.OK != result)
                    return result;
            }

            return Result.OK;
        }



        public Result l8K6CSetSelectMaskData
        (
            [In] byte           r_btSelectCriteria,
            [In] byte           r_btIndex,
            [In] SelectCriteria strcData            
        )
        {

#if _TRACE_OUT_PUT
            PrintMagToTxt("l8K6CSetSelectMaskData");
#endif   

            int             Offset = r_btIndex << 2;
            Result   result = Result.OK;
            ENUM_CMD TxCmd  = ENUM_CMD.NOTHING;
            byte[]   buf    = new byte[10];
            Array.Clear(buf, 0, buf.Length);

           
            TxCmd  = ENUM_CMD.l8K6C_SET_SELECT_MASK_DATA;
            buf[0] = (byte)TxCmd;
            buf[1] = r_btSelectCriteria;
            buf[2] = r_btIndex;
            buf[3] = strcData.pCriteria[r_btSelectCriteria].mask.mask[ Offset ];
            buf[4] = strcData.pCriteria[r_btSelectCriteria].mask.mask[ Offset + 1 ];
            buf[5] = strcData.pCriteria[r_btSelectCriteria].mask.mask[ Offset + 2 ];
            buf[6] = strcData.pCriteria[r_btSelectCriteria].mask.mask[ Offset + 3 ];

            if ( false == SendData(buf) )
                return Result.FAILURE;
              
            Array.Clear(buf, 0, buf.Length);

            //Check receive data
            if ( false == ReceiveData( TxCmd, ref buf ) )
                return Result.FAILURE;

            //Check result
            result = ConvertResult(buf[1]);
            if (Result.OK != result)
                return result;

            return Result.OK;
        }



        public Result API_l8K6CGetSelectCriteria
        (
              [In, Out] ref SelectCriteria strcData
        )
        {

#if _TRACE_OUT_PUT
            PrintMagToTxt("API_l8K6CGetSelectCriteria");
#endif   

            byte btEnableFlag = 0;

            //Reset Count
            strcData.countCriteria = 0;

            //Serach Active Count
            for (byte index = 0; index < 8; index++)
            {
                if (Result.OK != API_l8K6CGetActiveSelectCriteria(index, ref btEnableFlag))
                    return Result.FAILURE;

                if (btEnableFlag != (byte)AvtiveEnable.ENABLE)
                    break;

                strcData.countCriteria++;
            }


            strcData.pCriteria = new SelectCriterion[strcData.countCriteria];

            for (byte index = 0; index < strcData.countCriteria; ++index)
            {
                strcData.pCriteria[index] = new SelectCriterion();

                //Get Mask Data
                if (Result.OK != API_l8K6CGetSelectMaskData(index, ref strcData))
                    return Result.FAILURE;

                //Get Select Criteria Parameter
                if (Result.OK != l8K6CGetSelectCriteria(index, ref strcData))
                    return Result.FAILURE;

            }//end for                

            return Result.OK;
        }




        public Result l8K6CGetSelectCriteria
        (
            [In]          byte           btIndex,
            [In, Out] ref SelectCriteria strcData
        )
        {

#if _TRACE_OUT_PUT
            PrintMagToTxt("l8K6CGetSelectCriteria");
#endif

            Result   result = Result.OK;
            ENUM_CMD TxCmd  = ENUM_CMD.NOTHING;
            byte[]   buf    = new byte[10];
            Array.Clear(buf, 0, buf.Length);

            TxCmd  = ENUM_CMD.l8K6C_GET_SELECT_CRITERIA;
            buf[0] = (byte)TxCmd;
            buf[1] = btIndex;

            if ( false == SendData(buf) )
                return Result.FAILURE;

            Array.Clear(buf, 0, buf.Length);

            //Check receive data
            if ( false == ReceiveData( TxCmd, ref buf ) )
                return Result.FAILURE;

            //Check result
            result = ConvertResult(buf[1]);
            if (Result.OK != result)
                return result;

            //Get read data to structure
            strcData.pCriteria[btIndex].mask.bank             = (MemoryBank)buf[2];
            strcData.pCriteria[btIndex].mask.offset           = BitConverter.ToUInt16(buf, 3);
            strcData.pCriteria[btIndex].mask.count            = buf[5];
            strcData.pCriteria[btIndex].action.target         = (Target)buf[6];
            strcData.pCriteria[btIndex].action.action         = (Action)buf[7];
            strcData.pCriteria[btIndex].action.enableTruncate = buf[8];

            return Result.OK;
        }



        public Result API_l8K6CGetActiveSelectCriteria
        (
            [In]          byte r_btIndex,
            [In, Out] ref byte r_btEnableFlag
        )
        {

#if _TRACE_OUT_PUT
            PrintMagToTxt("l8K6CGetActiveSelectCriteria");
#endif

            Result   result = Result.OK;
            ENUM_CMD TxCmd  = ENUM_CMD.NOTHING;
            byte[]   buf    = new byte[10];
            Array.Clear(buf, 0, 8);

            TxCmd  = ENUM_CMD.l8K6C_GET_ACTIVE_SELECT_CRITERIA;
            buf[0] = (byte)TxCmd;
            buf[1] = r_btIndex;

            if ( false == SendData(buf) )
                return Result.FAILURE;

            Array.Clear(buf, 0, buf.Length);

            //Check receive data
            if ( false == ReceiveData( TxCmd, ref buf ) )
                return Result.FAILURE;

            //Check result
            result = ConvertResult(buf[1]);
            if (Result.OK != result)
                return result;

            r_btEnableFlag = buf[2];

            return Result.OK;
        }




        public Result API_l8K6CGetSelectMaskData
        (
            [In]          byte           r_btSelectCriteria,
            [In, Out] ref SelectCriteria strcData
        )
        {

#if _TRACE_OUT_PUT
            PrintMagToTxt("API_l8K6CGetSelectMaskData");
#endif

            Result result = Result.OK;          

            //Every criteria has 32 bytes mask data. One time get 4 bytes,so need to get 8 times.
            for (byte r_btIndex = 0; r_btIndex < 8; r_btIndex++)
            {
                result = l8K6CGetSelectMaskData(r_btSelectCriteria, r_btIndex, ref strcData);

                if (Result.OK != result)
                    return result;
            }

            return Result.OK;
        }




        public Result l8K6CGetSelectMaskData
        (
            [In]          byte           r_btSelectCriteria,
            [In]          byte           r_btIndex,
            [In, Out] ref SelectCriteria strcData
        )
        {

#if _TRACE_OUT_PUT
            PrintMagToTxt("l8K6CGetSelectMaskData");
#endif

            int             Offset = r_btIndex << 2;
            Result   result = Result.OK;
            ENUM_CMD TxCmd  = ENUM_CMD.NOTHING;
             byte[]         buf    = new byte[10];
            Array.Clear(buf, 0, buf.Length);

            TxCmd  = ENUM_CMD.l8K6C_GET_SELECT_MASK_DATA;
            buf[0] = (byte)TxCmd;
            buf[1] = r_btSelectCriteria;
            buf[2] = r_btIndex;            

            if ( false == SendData(buf) )
                return Result.FAILURE;
              
            Array.Clear(buf, 0, buf.Length);

            //Check receive data
            if ( false == ReceiveData( TxCmd, ref buf ) )
                return Result.FAILURE;

            //Check result
            result = ConvertResult(buf[1]);
            if (Result.OK != result)
                return result;

            strcData.pCriteria[r_btSelectCriteria].mask.mask[ Offset ]     = buf[2];
            strcData.pCriteria[r_btSelectCriteria].mask.mask[ Offset + 1 ] = buf[3];
            strcData.pCriteria[r_btSelectCriteria].mask.mask[ Offset + 2 ] = buf[4];
            strcData.pCriteria[r_btSelectCriteria].mask.mask[ Offset + 3 ] = buf[5]; 

            return Result.OK;
        }





        public Result API_l8K6CSetPostMatchCriteria
        (
            [In] SingulationCriteria r_strcData,
            [In] UInt32              r_flags
        )
        {

#if _TRACE_OUT_PUT
            PrintMagToTxt("API_l8K6CSetPostMatchCriteria");
#endif

            //Check size
            if (r_strcData.countCriteria > r_strcData.pCriteria.Length)
            {
                return Result.INVALID_PARAMETER;
            }


            for (byte index = 0; index < r_strcData.countCriteria; index++)
            {
                //SetMaskData
                if ( Result.OK != API_l8K6CSetPostMatchMaskData(index, r_strcData) )
                    return Result.FAILURE;

                //SetPostMatchCriteria
                if ( Result.OK != l8K6CSetPostMatchCriteria(index, r_strcData, r_flags) )
                    return Result.FAILURE;

            }//end for                
                

            return Result.OK;
        }




        public Result l8K6CSetPostMatchCriteria
        (
            [In] byte                r_btIndex,
            [In] SingulationCriteria r_strcData,
            [In] UInt32              r_flags
        )
        {

#if _TRACE_OUT_PUT
            PrintMagToTxt("l8K6CSetPostMatchCriteria");
#endif

            Result result         = Result.OK;
            ENUM_CMD TxCmd = ENUM_CMD.NOTHING;
            byte[]          buf   = new byte[10];
            Array.Clear(buf, 0, buf.Length);

            TxCmd = ENUM_CMD.l8K6C_SET_POST_MATCH_CRITERIA;
            buf[0] = (byte)TxCmd;
            buf[1] = r_strcData.pCriteria[r_btIndex].match;
            buf[2] = (byte)(  r_strcData.pCriteria[r_btIndex].mask.offset       & 0xFF );
            buf[3] = (byte)( (r_strcData.pCriteria[r_btIndex].mask.offset >> 8) & 0xFF );
            buf[4] = (byte)(  r_strcData.pCriteria[r_btIndex].mask.count        & 0xFF );
            buf[5] = (byte)( (r_strcData.pCriteria[r_btIndex].mask.count  >> 8) & 0xFF );

            if ( false == SendData(buf) )
                return Result.FAILURE;

            Array.Clear(buf, 0, buf.Length);

            //Check receive data
            if ( false == ReceiveData( TxCmd, ref buf ) )
                return Result.FAILURE;

            //Check result
            result = ConvertResult(buf[1]);
            if (Result.OK != result)
                return result;

            return Result.OK;
        }


        public Result API_l8K6CSetPostMatchMaskData
        (
            [In] byte                r_btSelectCriteria,
            [In] SingulationCriteria strcData
        )
        {

#if _TRACE_OUT_PUT
            PrintMagToTxt("API_l8K6CSetPostMatchMaskData");
#endif

            Result result = Result.OK;

            //Every criteria has 64 bytes mask data. One time set 4 bytes,so need to set 16 times.
            for (byte btIndex = 0; btIndex < 16; btIndex++)
            {
                result = l8K6CSetPostMatchMaskData(r_btSelectCriteria, btIndex, strcData);

                if (Result.OK != result)
                    return result;
            }

            return Result.OK;
        }



        public Result l8K6CSetPostMatchMaskData
        (
            [In] byte                r_btSelectCriteria,
            [In] byte                r_btMaskIndex,
            [In] SingulationCriteria strcData
        )
        {

#if _TRACE_OUT_PUT
            PrintMagToTxt("l8K6CSetPostMatchMaskData");
#endif

            int             Offset = r_btMaskIndex << 2;
            Result   result = Result.OK;
            ENUM_CMD TxCmd  = ENUM_CMD.NOTHING;
            byte[]   buf    = new byte[10];
            Array.Clear(buf, 0, buf.Length);


            TxCmd  = ENUM_CMD.l8K6C_SET_POST_MATCH_MASK_DATA;
            buf[0] = (byte)TxCmd;
            buf[1] = r_btMaskIndex;
            buf[2] = strcData.pCriteria[r_btSelectCriteria].mask.mask[Offset];
            buf[3] = strcData.pCriteria[r_btSelectCriteria].mask.mask[Offset + 1];

            //only support 62byte. last 2 byte doesn't support.
            if (r_btMaskIndex < 15)
            {
                buf[4] = strcData.pCriteria[r_btSelectCriteria].mask.mask[Offset + 2];
                buf[5] = strcData.pCriteria[r_btSelectCriteria].mask.mask[Offset + 3];
            }

            if ( false == SendData(buf) )
                return Result.FAILURE;

            Array.Clear(buf, 0, buf.Length);

            //Check receive data
            if ( false == ReceiveData( TxCmd, ref buf ) )
                return Result.FAILURE;

            //Check result
            result = ConvertResult(buf[1]);
            if (Result.OK != result)
                return result;

            return Result.OK;
        }



        public Result API_l8K6CGetPostMatchCriteria
        (
            [In, Out] ref SingulationCriteria r_strcData
        )
        {

#if _TRACE_OUT_PUT
            PrintMagToTxt("API_l8K6CGetPostMatchCriteria");
#endif

            //Reset Count
            r_strcData.countCriteria = 1;
            r_strcData.pCriteria     = new SingulationCriterion[r_strcData.countCriteria];

            for (byte index = 0; index < r_strcData.countCriteria; ++index)
            {
                r_strcData.pCriteria[index] = new SingulationCriterion();
                r_strcData.pCriteria[index].mask.count  = 0;
                r_strcData.pCriteria[index].mask.offset = 0;


                //Get Mask Data
                if ( Result.OK != API_l8K6CGetPostMatchMaskData(index, ref r_strcData) )
                    return Result.FAILURE;


                if ( Result.OK != l8K6CGetPostMatchCriteria(index, ref r_strcData) )
                    return Result.FAILURE; 


            }//end for                

            return Result.OK;
        }



        public Result l8K6CGetPostMatchCriteria
        (
            [In]          byte                r_btSelectCriteria,
            [In, Out] ref SingulationCriteria r_strcData
        )
        {

#if _TRACE_OUT_PUT
            PrintMagToTxt("l8K6CGetPostMatchCriteria");
#endif
            
            Result   result = Result.OK;
            ENUM_CMD TxCmd  = ENUM_CMD.NOTHING;
            byte[]   buf    = new byte[10];
            Array.Clear(buf, 0, buf.Length);

            TxCmd  = ENUM_CMD.l8K6C_GET_POST_MATCH_CRITERIA;
            buf[0] = (byte)TxCmd;


            if ( false == SendData(buf) )
                return Result.FAILURE;

            Array.Clear(buf, 0, buf.Length);

            //Check receive data
            if ( false == ReceiveData( TxCmd, ref buf ) )
                return Result.FAILURE;

            //Check result
            result = ConvertResult(buf[1]);
            if (Result.OK != result)
                return result;


            //Get read data to structure
            r_strcData.pCriteria[r_btSelectCriteria].match       = buf[2];
            r_strcData.pCriteria[r_btSelectCriteria].mask.offset = BitConverter.ToUInt16(buf, 3);
            r_strcData.pCriteria[r_btSelectCriteria].mask.count  = BitConverter.ToUInt16(buf, 5);

            if( ValueLimit.l8K6C_POST_MATCH_CRITERIA_OFFSET_MAX < r_strcData.pCriteria[r_btSelectCriteria].mask.count )
                return Result.UNEXPECTED_VALUE;

            return Result.OK;
        }




        public Result API_l8K6CGetPostMatchMaskData
        (
            [In]          byte                r_btSelectCriteria,
            [In, Out] ref SingulationCriteria strcData
        )
        {

#if _TRACE_OUT_PUT
            PrintMagToTxt("API_l8K6CGetPostMatchMaskData");
#endif

            Result result = Result.OK;

            //Every criteria has 64 bytes mask data. One time get 4 bytes,so need to get 16 times.
            for (byte r_btIndex = 0; r_btIndex < 16; r_btIndex++)
            {
                result = l8K6CGetPostMatchMaskData(r_btSelectCriteria, r_btIndex, ref strcData);

                if (Result.OK != result)
                    return result;
            }

            return Result.OK;
        }



        public Result l8K6CGetPostMatchMaskData
        (
            [In]          byte r_btSelectCriteria,
            [In]          byte r_btIndex,
            [In, Out] ref SingulationCriteria strcData
        )
        {

#if _TRACE_OUT_PUT
            PrintMagToTxt("l8K6CGetPostMatchMaskData");
#endif

            int             Offset = r_btIndex << 2;
            Result   result = Result.OK;
            ENUM_CMD TxCmd  = ENUM_CMD.NOTHING;
            byte[]   buf    = new byte[10];
            Array.Clear(buf, 0, buf.Length);

            TxCmd  = ENUM_CMD.l8K6C_GET_POST_MATCH_MASK_DATA;
            buf[0] = (byte)TxCmd;
            buf[1] = r_btIndex;

            if ( false == SendData(buf) )
                return Result.FAILURE;

            Array.Clear(buf, 0, buf.Length);

            //Check receive data
            if ( false == ReceiveData( TxCmd, ref buf ) )
                return Result.FAILURE;

            //Check result
            result = ConvertResult(buf[1]);
            if (Result.OK != result)
                return result;

            strcData.pCriteria[r_btSelectCriteria].mask.mask[Offset]     = buf[2];
            strcData.pCriteria[r_btSelectCriteria].mask.mask[Offset + 1] = buf[3];

            //only support 62byte. last 2 byte doesn't support.
            if (r_btIndex < 15)
            {
                strcData.pCriteria[r_btSelectCriteria].mask.mask[Offset + 2] = buf[4];
                strcData.pCriteria[r_btSelectCriteria].mask.mask[Offset + 3] = buf[5];
            }

            return Result.OK;
        }



//===========================ISO 18000-6C Tag Access Parameter==========================
        public Result API_l8K6CSetQueryTagGroup
        (
            [In] TagGroup r_strcGroup
        )
        {

#if _TRACE_OUT_PUT
            PrintMagToTxt("API_l8K6CSetQueryTagGroup");
#endif

            Result   result = Result.OK;
            ENUM_CMD TxCmd  = ENUM_CMD.NOTHING;
            byte[]   buf    = new byte[10];
            Array.Clear(buf, 0, buf.Length);

            TxCmd  = ENUM_CMD.l8K6C_SET_QUERY_TAG_GROUP;
            buf[0] = (byte)TxCmd;
            buf[1] = (byte)r_strcGroup.selected;
            buf[2] = (byte)r_strcGroup.session;
            buf[3] = (byte)r_strcGroup.target;

            if ( false == SendData(buf) )
                return Result.FAILURE;

            Array.Clear(buf, 0, buf.Length);

            //Check receive data
            if ( false == ReceiveData( TxCmd, ref buf ) )
                return Result.FAILURE;

            //Check result
            result = ConvertResult(buf[1]);
            if (Result.OK != result)
                return result;

            return Result.OK;
        }




        public Result API_l8K6CGetQueryTagGroup
        (
            [In, Out] ref TagGroup r_strcGroup
        )
        {

#if _TRACE_OUT_PUT
            PrintMagToTxt("API_l8K6CGetQueryTagGroup");
#endif

            Result result = Result.OK;
            if (null == r_strcGroup)
            {
                r_strcGroup = new TagGroup();
            }

            r_strcGroup.selected = Selected.UNKNOWN;
            r_strcGroup.session  = Session.UNKNOWN;
            r_strcGroup.target   = SessionTarget.UNKNOWN;

            ENUM_CMD TxCmd = ENUM_CMD.NOTHING;
            byte[] buf = new byte[10];
            Array.Clear(buf, 0, buf.Length);

            TxCmd = ENUM_CMD.l8K6C_GET_QUERY_TAG_GROUP;
            buf[0] = (byte)TxCmd;

            if ( false == SendData(buf) )
                return Result.FAILURE;

            Array.Clear(buf, 0, buf.Length);

            //Check receive data
            if ( false == ReceiveData( TxCmd, ref buf ) )
                return Result.FAILURE;

            //Check result
            result = ConvertResult(buf[1]);
            if (Result.OK != result)
                return result;


            r_strcGroup.selected = (Selected)buf[2];
            r_strcGroup.session  = (Session) buf[3];
            r_strcGroup.target   = (SessionTarget)buf[4];

            return Result.OK;
            
        }




        public Result API_l8K6CSetCurrentSingulationAlgorithm
        (
            [In] SingulationAlgorithm algorithm
        )
        {

#if _TRACE_OUT_PUT
            PrintMagToTxt("API_l8K6CSetCurrentSingulationAlgorithm");
#endif

            Result   result = Result.OK;
            ENUM_CMD TxCmd  = ENUM_CMD.NOTHING;
            byte[]   buf    = new byte[10];
            Array.Clear(buf, 0, buf.Length);

            TxCmd  = ENUM_CMD.l8K6C_SET_CURRENT_SINGULATION_ALGORITHM;
            buf[0] = (byte)TxCmd;
            buf[1] = (byte)algorithm;

            if ( false == SendData(buf) )
                return Result.FAILURE;

            Array.Clear(buf, 0, buf.Length);

            //Check receive data
            if ( false == ReceiveData( TxCmd, ref buf ) )
                return Result.FAILURE;

            //Check result
            result = ConvertResult(buf[1]);
            if (Result.OK != result)
                return result;

            return Result.OK;
        }




        public Result API_l8K6CGetCurrentSingulationAlgorithm
        (
            [In, Out] ref SingulationAlgorithm algorithm
        )
        {

#if _TRACE_OUT_PUT
            PrintMagToTxt("l8K6CGetCurrentSingulationAlgorithm");
#endif

            algorithm              = SingulationAlgorithm.UNKNOWN;
            Result   result = Result.OK;
            ENUM_CMD TxCmd  = ENUM_CMD.NOTHING;
            byte[]   buf    = new byte[10];
            Array.Clear(buf, 0, buf.Length);

            TxCmd  = ENUM_CMD.l8K6C_GET_CURRENT_SINGULATION_ALGORITHM;
            buf[0] = (byte)TxCmd;

            if ( false == SendData(buf) )
                return Result.FAILURE;

            Array.Clear(buf, 0, buf.Length);

            //Check receive data
            if ( false == ReceiveData( TxCmd, ref buf ) )
                return Result.FAILURE;

            //Check result
            result = ConvertResult(buf[1]);
            if (Result.OK != result)
                return result;


            algorithm = (SingulationAlgorithm)buf[2];

            return Result.OK;
            
        }



        public Result API_l8K6CSetSingulationAlgorithmParameters
        (
            [In] SingulationAlgorithm      r_Algorithm,
            [In] SingulationAlgorithmParms r_Parms
        )
        {

#if _TRACE_OUT_PUT
            PrintMagToTxt("API_l8K6CSetSingulationAlgorithmParameters");
#endif

            if (null == r_Parms)
            {   
                return Result.INVALID_PARAMETER;
            }

            // Instead of the following, could simply make the native call
            // with given fields and catch the resulting marshal exception
            // when trying to do ptr to structure

            Type algoType = r_Parms.GetType( );

            if
            (
                ( SingulationAlgorithm.UNKNOWN   == r_Algorithm )
             || ( SingulationAlgorithm.FIXEDQ    == r_Algorithm && r_Parms.GetType( ) != typeof( FixedQParms ) )
             || ( SingulationAlgorithm.DYNAMICQ  == r_Algorithm && r_Parms.GetType( ) != typeof( DynamicQParms ) )
            )
            {
                return Result.INVALID_PARAMETER;
            }


            Result          result    = Result.OK;
            ENUM_CMD TxCmd     = ENUM_CMD.NOTHING;
            byte[]          buf       = new byte[10];
            Array.Clear(buf, 0, buf.Length);

            TxCmd  = ENUM_CMD.l8K6C_SET_SINGULATION_ALGORITHM_PARAMETERS;
            buf[0] = (byte)TxCmd;
            buf[1] = (byte)r_Algorithm;

            switch(r_Algorithm)
            {
                case SingulationAlgorithm.FIXEDQ:
                    {
                        FixedQParms fixParams = (FixedQParms)r_Parms;

                        buf[2] = fixParams.qValue;
                        buf[3] = fixParams.retryCount;
                        buf[4] = fixParams.toggleTarget;
                        buf[5] = fixParams.repeatUntilNoTags;
                    } 
                    break;

                case SingulationAlgorithm.DYNAMICQ:
                    {
                        DynamicQParms dynamicParams = (DynamicQParms)r_Parms;

                        buf[2] = dynamicParams.startQValue;
                        buf[3] = dynamicParams.minQValue;
                        buf[4] = dynamicParams.maxQValue;
                        buf[5] = dynamicParams.retryCount;
                        buf[6] = dynamicParams.toggleTarget;
                        buf[7] = dynamicParams.thresholdMultiplier;
                    }
                    break;

                default:
                    return Result.INVALID_PARAMETER;                   
            }//end switch


            if ( false == SendData(buf) )
                return Result.FAILURE;

            Array.Clear(buf, 0, buf.Length);

            //Check receive data
            if ( false == ReceiveData( TxCmd, ref buf ) )
                return Result.FAILURE;

            //Check result
            result = ConvertResult(buf[1]);
            if (Result.OK != result)
                return result;


            return Result.OK; 
        }



        public Result API_l8K6CGetSingulationAlgorithmParameters
        (
            [In]          SingulationAlgorithm      r_Algorithm,
            [In, Out] ref SingulationAlgorithmParms r_Parms
        )
        {

#if _TRACE_OUT_PUT
            PrintMagToTxt("API_l8K6CGetSingulationAlgorithmParameters");
#endif

            if (null == r_Parms)
            {
                return Result.INVALID_PARAMETER;
            }

            // Instead of the following, could simply make the native call
            // with given fields and catch the resulting marshal exception
            // when trying to do ptr to structure

            Type algoType = r_Parms.GetType();

            if
            (
                (SingulationAlgorithm.UNKNOWN  == r_Algorithm)
             || (SingulationAlgorithm.FIXEDQ   == r_Algorithm && r_Parms.GetType() != typeof(FixedQParms))
             || (SingulationAlgorithm.DYNAMICQ == r_Algorithm && r_Parms.GetType() != typeof(DynamicQParms))
            )
            {
                return Result.INVALID_PARAMETER;
            }


            Result          result    = Result.OK;
            ENUM_CMD TxCmd     = ENUM_CMD.NOTHING;
            object          objParams = r_Parms;
            byte[]          buf       = new byte[10];
            Array.Clear(buf, 0, buf.Length);

            TxCmd  = ENUM_CMD.l8K6C_GET_SINGULATION_ALGORITHM_PARAMETERS;
            buf[0] = (byte)TxCmd;
            buf[1] = (byte)r_Algorithm;

            if ( false == SendData(buf) )
                return Result.FAILURE;

            Array.Clear(buf, 0, buf.Length);

            //Check receive data
            if ( false == ReceiveData( TxCmd, ref buf ) )
                return Result.FAILURE;

            //Check result
            result = ConvertResult(buf[1]);
            if (Result.OK != result)
                return result;


            switch (r_Algorithm)
            {
                case SingulationAlgorithm.FIXEDQ:
                    {

                        FixedQParms fixParams = new FixedQParms();

                        fixParams.qValue            = buf[2];
                        fixParams.retryCount        = buf[3];
                        fixParams.toggleTarget      = buf[4];
                        fixParams.repeatUntilNoTags = buf[5];

                        objParams = fixParams;
                    }
                    break;

                case SingulationAlgorithm.DYNAMICQ:
                    {
                        DynamicQParms dynamicParams = new DynamicQParms();

                        dynamicParams.startQValue         = buf[2];
                        dynamicParams.minQValue           = buf[3];
                        dynamicParams.maxQValue           = buf[4];
                        dynamicParams.retryCount          = buf[5];
                        dynamicParams.toggleTarget        = buf[6];
                        dynamicParams.thresholdMultiplier = buf[7];

                        objParams = dynamicParams;
                    }
                    break;

                default:
                    return Result.INVALID_PARAMETER;
            }//end switch


            //Copy data to the reference structure
            IntPtr pstrcData = IntPtr.Zero;
            Int32  Size      = 0;

            Size      = Marshal.SizeOf(objParams);
            pstrcData = Marshal.AllocHGlobal(Size);

            if (0 >= Size || pstrcData == IntPtr.Zero)
                return Result.INVALID_PARAMETER;

            Marshal.StructureToPtr(objParams, pstrcData, true);
            Marshal.PtrToStructure(pstrcData, r_Parms);


            return Result.OK;
        }




        public Result API_l8K6CSetTagAccessPassword
        (
            [In] UInt32 AccessPassword
        )
        {

#if _TRACE_OUT_PUT
            PrintMagToTxt("API_l8K6CSetTagAccessPassword");
#endif

            Result   result = Result.OK;
            ENUM_CMD TxCmd  = ENUM_CMD.NOTHING;
            byte[]   buf    = new byte[10];
            Array.Clear(buf, 0, buf.Length);

            TxCmd  = ENUM_CMD.l8K6C_SET_TAG_ACCESS_PASSWORD;
            buf[0] = (byte)TxCmd;
            buf[1] = (byte)(  AccessPassword        & 0xFF );
            buf[2] = (byte)( (AccessPassword >>  8) & 0xFF );
            buf[3] = (byte)( (AccessPassword >> 16) & 0xFF );
            buf[4] = (byte)( (AccessPassword >> 24) & 0xFF );

            if ( false == SendData(buf) )
                return Result.FAILURE;

            Array.Clear(buf, 0, buf.Length);

            //Check receive data
            if ( false == ReceiveData( TxCmd, ref buf ) )
                return Result.FAILURE;

            //Check result
            result = ConvertResult(buf[1]);
            if (Result.OK != result)
                return result;

            return Result.OK;
        }




        public Result API_l8K6CTagGetAccessPassword
        (
            [In,Out] ref UInt32 AccessPassword
        )
        {

#if _TRACE_OUT_PUT
            PrintMagToTxt("API_l8K6CTagGetAccessPassword");
#endif

            Result   result = Result.OK;
            ENUM_CMD TxCmd  = ENUM_CMD.NOTHING;
            byte[]   buf    = new byte[10];
            Array.Clear(buf, 0, buf.Length);

            TxCmd  = ENUM_CMD.l8K6C_GET_TAG_ACCESS_PASSWORD;
            buf[0] = (byte)TxCmd;

            if ( false == SendData(buf) )
                return Result.FAILURE;

            Array.Clear(buf, 0, buf.Length);

            //Check receive data
            if ( false == ReceiveData( TxCmd, ref buf ) )
                return Result.FAILURE;

            //Check result
            result = ConvertResult(buf[1]);
            if (Result.OK != result)
                return result;

            AccessPassword = BitConverter.ToUInt32(buf, 2);

            return Result.OK;
        }




        public Result API_l8K6CTagWriteDataBuffer
        (
            [In] byte   r_Index,
            [In] UInt16 r_Data,
            [In] byte   r_OffsetType,
            [In] UInt16 r_DataOffset
        )
        {

#if _TRACE_OUT_PUT
            PrintMagToTxt("API_l8K6CTagSetWriteDataBuffer");
#endif

            Result   result = Result.OK;
            ENUM_CMD TxCmd  = ENUM_CMD.NOTHING;
            byte[]   buf    = new byte[10];
            Array.Clear(buf, 0, buf.Length);

            TxCmd  = ENUM_CMD.l8K6C_TAG_WRITE_DATA_BUFFER;
            buf[0] = (byte)TxCmd;
            buf[1] = r_Index;
            buf[2] = (byte)(  r_Data       & 0xFF );
            buf[3] = (byte)( (r_Data >> 8) & 0xFF );
            buf[4] = r_OffsetType;
            buf[5] = (byte)(  r_DataOffset       & 0xFF );
            buf[6] = (byte)( (r_DataOffset >> 8) & 0xFF );

            if ( false == SendData(buf) )
                return Result.FAILURE;

            Array.Clear(buf, 0, buf.Length);

            //Check receive data
            if ( false == ReceiveData( TxCmd, ref buf ) )
                return Result.FAILURE;

            //Check result
            result = ConvertResult(buf[1]);
            if (Result.OK != result)
                return result;       

            return Result.OK;
        }




        public Result API_l8K6CTagReadDataBuffer
        (
            [In]          byte   r_Index,
            [In, Out] ref UInt16 r_Data,
            [In, Out] ref UInt16 r_DataOffset
        )
        {

#if _TRACE_OUT_PUT
            PrintMagToTxt("API_l8K6CTagReadDataBuffer");
#endif

            Result   result = Result.OK;
            ENUM_CMD TxCmd  = ENUM_CMD.NOTHING;
            byte[]   buf    = new byte[10];
            Array.Clear(buf, 0, buf.Length);

            TxCmd  = ENUM_CMD.l8K6C_TAG_READ_DATA_BUFFER;
            buf[0] = (byte)TxCmd;
            buf[1] = r_Index;

            if ( false == SendData(buf) )
                return Result.FAILURE;

            Array.Clear(buf, 0, buf.Length);

            //Check receive data
            if ( false == ReceiveData( TxCmd, ref buf ) )
                return Result.FAILURE;

            //Check result
            result = ConvertResult(buf[1]);
            if (Result.OK != result)
                return result;

            r_Data       = BitConverter.ToUInt16(buf, 2);
            r_DataOffset = BitConverter.ToUInt16(buf, 4);
        
            return Result.OK;
        }




//===========================ISO 18000-6C Tag Access Operation============================
        public Result API_l8K6CTagInventory
        (
            [In] InventoryParms r_Parms,
            [In] byte           r_PerformFlags
        )
        {

#if _TRACE_OUT_PUT
            PrintMagToTxt("API_l8K6CTagInventory");
#endif

            Result   result = Result.OK;
            ENUM_CMD TxCmd  = ENUM_CMD.NOTHING;
            byte[]   buf    = new byte[10];
            Array.Clear(buf, 0, buf.Length);

            TxCmd  = ENUM_CMD.l8K6C_TAG_INVENTORY;
            buf[0] = (byte)TxCmd;
            buf[1] = r_Parms.common.strcTagFlag.SelectOpsFlag;
            buf[2] = r_Parms.common.strcTagFlag.PostMatchFlag;

            if ( false == SendData(buf) )
                return Result.FAILURE;

            Array.Clear(buf, 0, buf.Length);

            //Check receive data
            if ( false == ReceiveData( TxCmd, ref buf ) )
                return Result.FAILURE;
           
            //Check result
            result = ConvertResult(buf[1]);
            if (Result.OK != result)
                return result;
            
            //Run report packet process
            if ( CheckTagAccessPkt(r_Parms.common) == false )
                return Result.FAILURE;
            
            return Result.OK;         
        }



        
        public Result API_l8K6CTagRead
        (
            [In] ReadParms r_Parms,
            [In] byte      r_PerformFlags
        )
        {

#if _TRACE_OUT_PUT
            PrintMagToTxt("API_l8K6CTagRead");
#endif

            //Set Access Password. Tag Read/Write/Kill/Lock need to set Access Password
            if (Result.OK != API_l8K6CSetTagAccessPassword(r_Parms.accessPassword))
                return Result.FAILURE;

            Result   result = Result.OK;
            ENUM_CMD TxCmd  = ENUM_CMD.NOTHING;
            byte[]   buf    = new byte[10];
            Array.Clear(buf, 0, buf.Length);

            TxCmd  = ENUM_CMD.l8K6C_TAG_READ;
            buf[0] = (byte)TxCmd;
            buf[1] = (byte)r_Parms.readCmdParms.bank;
            buf[2] = (byte)(  r_Parms.readCmdParms.offset       & 0xFF);
            buf[3] = (byte)( (r_Parms.readCmdParms.offset >> 8) & 0xFF);
            buf[4] = r_Parms.readCmdParms.count;
            buf[5] = r_Parms.common.strcTagFlag.RetryCount;
            buf[6] = r_Parms.common.strcTagFlag.SelectOpsFlag;
            buf[7] = r_Parms.common.strcTagFlag.PostMatchFlag;

            if ( false == SendData(buf) )
                return Result.FAILURE;

            Array.Clear(buf, 0, buf.Length);

            //Check receive data
            if ( false == ReceiveData( TxCmd, ref buf ) )
                return Result.FAILURE;

            //Check result
            result = ConvertResult(buf[1]);
            if (Result.OK != result)
                return result;
            
            //Run report packet process
            if ( CheckTagAccessPkt(r_Parms.common ) == false)
                return Result.FAILURE;

            return Result.OK;   
        }




        //clark 2011.3.25 not sure
        public Result API_l8K6CTagWrite
        (
            [In] WriteParms r_Parms,
            [In] byte       r_PerformFlags
        )
        {

#if _TRACE_OUT_PUT
            PrintMagToTxt("API_l8K6CTagWrite");
#endif

            Result result = Result.OK;

            switch (r_Parms.writeType)
            {
                case WriteType.SEQUENTIAL:
                    {
                        WriteSequentialParms seqParms = (WriteSequentialParms)r_Parms.writeParms;

                        //Check length and buffer overflow
                        if (seqParms.pData.Length < seqParms.count)
                            return Result.INVALID_PARAMETER;

                        if (seqParms.count > 1)
                        {
                            //l8K6CTagWrite doesn't send data to write buffer, and
                            //it only send l8K6CTagWrite command. It is faster.
                            result = API_l8K6CTagMultipleWrite(r_Parms, r_PerformFlags);
                        }
                        else
                        {
                            //l8K6CTagMultipleWrite need to send every data to wite buffer,
                            //then send l8K6CTagMultipleWrite command.
                            result = l8K6CTagWrite(r_Parms, r_PerformFlags);
                        }

                        break;
                    }


                case WriteType.RANDOM:
                    {
                        WriteRandomParms randParms = (WriteRandomParms)r_Parms.writeParms;

                        //Check length and buffer overflow
                        if (randParms.pData.Length < randParms.count)
                            return Result.INVALID_PARAMETER;

                        if (randParms.count > 1)
                        {
                            //l8K6CTagWrite doesn't send data to write buffer, and
                            //it only send l8K6CTagWrite command. It is faster.
                            result = API_l8K6CTagMultipleWrite(r_Parms, r_PerformFlags);
                        }
                        else
                        {
                            //l8K6CTagMultipleWrite need to send every data to wite buffer,
                            //then send l8K6CTagMultipleWrite command.
                            result = l8K6CTagWrite(r_Parms, r_PerformFlags);
                        }

                        break;
                    }

                default:
                    result = Result.INVALID_PARAMETER;
                    break;
            }

            return result;
        }





        //clark 2011.3.25 not sure
        public Result l8K6CTagWrite
        (
            [In] WriteParms r_Parms,
            [In] byte       r_PerformFlags
        )
        {

#if _TRACE_OUT_PUT
            PrintMagToTxt("l8K6CTagWrite");
#endif

            //Set Access Password. Tag Read/Write/Kill/Lock need to set Access Password
            if (Result.OK != API_l8K6CSetTagAccessPassword(r_Parms.accessPassword))
                return Result.FAILURE;

            Result   result = Result.OK;
            ENUM_CMD TxCmd  = ENUM_CMD.NOTHING;
            byte[]   buf    = new byte[10];
            Array.Clear(buf, 0, buf.Length);

            TxCmd  = ENUM_CMD.l8K6C_TAG_WRITE;
            buf[0] = (byte)TxCmd;
            buf[6] = r_Parms.common.strcTagFlag.RetryCount;
            buf[7] = r_Parms.common.strcTagFlag.SelectOpsFlag;
            buf[8] = r_Parms.common.strcTagFlag.PostMatchFlag;

            if ( WriteType.SEQUENTIAL  == r_Parms.writeType )
            {
                WriteSequentialParms seqParms = ( WriteSequentialParms ) r_Parms.writeParms;
                
                buf[1] = (byte)seqParms.bank;
                buf[2] = (byte)(  seqParms.offset         & 0xFF );
                buf[3] = (byte)( (seqParms.offset   >> 8) & 0xFF );
                buf[4] = (byte)(  seqParms.pData[0]       & 0xFF );
                buf[5] = (byte)( (seqParms.pData[0] >> 8) & 0xFF );
     
             }     
             else
             {
                WriteRandomParms randParms = ( WriteRandomParms ) r_Parms.writeParms;
                
                buf[1] = (byte)randParms.bank;
                buf[2] = (byte)(  randParms.pOffset[0]       & 0xFF );
                buf[3] = (byte)(( randParms.pOffset[0] >> 8) & 0xFF );
                buf[4] = (byte)(  randParms.pData[0]         & 0xFF );
                buf[5] = (byte)( (randParms.pData[0]   >> 8) & 0xFF );
             }

            if ( false == SendData(buf) )
                return Result.FAILURE;

            Array.Clear(buf, 0, buf.Length);

            //Check receive data
            if ( false == ReceiveData( TxCmd, ref buf ) )
                return Result.FAILURE;

            //Check result
            result = ConvertResult(buf[1]);
            if (Result.OK != result)
                return result;

            //Run report packet process
            if ( CheckTagAccessPkt(r_Parms.common) == false )
                return Result.FAILURE;

            return Result.OK;  
        }




        //clark not sure
        public Result API_l8K6CTagMultipleWrite
        (
            [In] WriteParms r_Parms,
            [In] byte r_PerformFlags
        )
        {
            
#if _TRACE_OUT_PUT
            PrintMagToTxt("API_l8K6CTagMultipleWrite");
#endif

            //Set Access Password. Tag Read/Write/Kill/Lock need to set Access Password
            if (Result.OK != API_l8K6CSetTagAccessPassword(r_Parms.accessPassword))
                return Result.FAILURE;

            Result result = Result.OK;

            //Send data to buffer
            switch (r_Parms.writeType)
            { 
                case WriteType.SEQUENTIAL:
                    WriteSequentialParms seqParms = (WriteSequentialParms)r_Parms.writeParms;

                    if (seqParms.count > ValueLimit.TAG_MULTI_WRITE_DATA_BUF_LENTH)
                        return Result.INVALID_PARAMETER;

                    for (byte i = 0; i < seqParms.count; i++)
                    {
                        result = API_l8K6CTagWriteDataBuffer( i,
                                                              seqParms.pData[i],
                                                              0,
                                                              0);

                        if (Result.OK != result)
                            return result;

                    }
                    break;

                case WriteType.RANDOM:
                    WriteRandomParms randParms = (WriteRandomParms)r_Parms.writeParms;

                    if (randParms.count > ValueLimit.TAG_MULTI_WRITE_DATA_BUF_LENTH)
                        return Result.INVALID_PARAMETER;

                    for (byte i = 0; i < randParms.count; i++)
                    {
                        result = API_l8K6CTagWriteDataBuffer( i,
                                                              randParms.pData[i],
                                                              0,
                                                              0);

                        if (Result.OK != result)
                            return result;
                    }
                    break;


                default:
                    return Result.INVALID_PARAMETER;            
            }



            //Start to update data              
            return l8K6CTagMultipleWrite(r_Parms, r_PerformFlags);

        }



        //clark not sure
        public Result l8K6CTagMultipleWrite
        (
            [In] WriteParms r_Parms,
            [In] byte r_PerformFlags
        )
        {
            
#if _TRACE_OUT_PUT
            PrintMagToTxt("l8K6CTagMultipleWrite");
#endif

            Result result         = Result.OK;
            ENUM_CMD TxCmd = ENUM_CMD.NOTHING;
            byte[] buf            = new byte[10];
            Array.Clear(buf, 0, buf.Length);

            TxCmd = ENUM_CMD.l8K6C_TAG_MULTIPLE_WRITE;
            buf[0] = (byte)TxCmd;
            buf[5] = 0; //Resered
            buf[6] = r_Parms.common.strcTagFlag.RetryCount;
            buf[7] = r_Parms.common.strcTagFlag.SelectOpsFlag;
            buf[8] = r_Parms.common.strcTagFlag.PostMatchFlag;

            //push dtat to buffer
            switch (r_Parms.writeType)
            { 
                case WriteType.SEQUENTIAL:
                    WriteSequentialParms seqParms = (WriteSequentialParms)r_Parms.writeParms;

                    buf[1] = (byte)seqParms.bank;
                    buf[2] = (byte)(  seqParms.offset       & 0xFF );
                    buf[3] = (byte)( (seqParms.offset >> 8) & 0xFF );
                    buf[4] = seqParms.count;
                    break;

                case WriteType.RANDOM:
                    WriteRandomParms randParms = (WriteRandomParms)r_Parms.writeParms;

                    buf[1] = (byte)randParms.bank;
                    buf[2] = (byte)(  randParms.pOffset[0]       & 0xFF );
                    buf[3] = (byte)( (randParms.pOffset[0] >> 8) & 0xFF );
                    buf[4] = randParms.count;
                    break;

                default:
                    return Result.INVALID_PARAMETER;            
            }
  

            if ( false == SendData(buf) )
                return Result.FAILURE;

            Array.Clear(buf, 0, buf.Length);

            //Check receive data
            if ( false == ReceiveData( TxCmd, ref buf ) )
                return Result.FAILURE;

            //Check result
            result = ConvertResult(buf[1]);
            if (Result.OK != result)
                return result;

            //Run report packet process
            if ( CheckTagAccessPkt(r_Parms.common) == false )
                return Result.FAILURE;

            return Result.OK;
        }





        public Result API_l8K6CTagKill
        (        
            [In] KillParms r_Parms,
            [In] byte      r_PerformFlags
        )
        {
                       
#if _TRACE_OUT_PUT
            PrintMagToTxt("API_l8K6CTagKill");
#endif

            //Set Access Password. Tag Read/Write/Kill/Lock need to set Access Password
            if (Result.OK != API_l8K6CSetTagAccessPassword(r_Parms.accessPassword))
                return Result.FAILURE;


            Result   result = Result.OK;
            ENUM_CMD TxCmd  = ENUM_CMD.NOTHING;
            byte[]   buf    = new byte[10];
            Array.Clear(buf, 0, buf.Length);

            TxCmd  = ENUM_CMD.l8K6C_TAG_KILL;
            buf[0] = (byte)TxCmd;
            buf[1] = (byte)(  r_Parms.killCmdParms.killPassword        & 0xFF );
            buf[2] = (byte)( (r_Parms.killCmdParms.killPassword >>  8) & 0xFF );
            buf[3] = (byte)( (r_Parms.killCmdParms.killPassword >> 16) & 0xFF );
            buf[4] = (byte)( (r_Parms.killCmdParms.killPassword >> 24) & 0xFF );
            buf[5] = r_Parms.common.strcTagFlag.RetryCount;
            buf[6] = r_Parms.common.strcTagFlag.SelectOpsFlag;
            buf[7] = r_Parms.common.strcTagFlag.PostMatchFlag;


            if ( false == SendData(buf) )
                return Result.FAILURE;

            Array.Clear(buf, 0, buf.Length);

            //Check receive data
            if ( false == ReceiveData( TxCmd, ref buf ) )
                return Result.FAILURE;

            //Check result
            result = ConvertResult(buf[1]);
            if (Result.OK != result)
                return result;

            //Run report packet process
            if ( CheckTagAccessPkt(r_Parms.common) == false )
                return Result.FAILURE;

            return Result.OK;  

        }



        public Result API_l8K6CTagLock
        (
            [In] LockParms r_Parms,
            [In] byte      r_PerformFlags
        )
        {
            
#if _TRACE_OUT_PUT
            PrintMagToTxt("API_Tag18K6CLock");
#endif

            //Set Access Password. Tag Read/Write/Kill/Lock need to set Access Password
            if (Result.OK != API_l8K6CSetTagAccessPassword(r_Parms.accessPassword))
                return Result.FAILURE;

            Result   result = Result.OK;
            ENUM_CMD TxCmd  = ENUM_CMD.NOTHING;
            byte[]   buf    = new byte[10];
            Array.Clear(buf, 0, buf.Length);

            TagPerm lockParms = (TagPerm)r_Parms.lockCmdParms.permissions;
            
            TxCmd  = ENUM_CMD.l8K6C_TAG_LOCK;
            buf[0] = (byte)TxCmd;
            buf[1] = (byte)lockParms.killPasswordPermissions;
            buf[2] = (byte)lockParms.accessPasswordPermissions;
            buf[3] = (byte)lockParms.epcMemoryBankPermissions;
            buf[4] = (byte)lockParms.tidMemoryBankPermissions;
            buf[5] = (byte)lockParms.userMemoryBankPermissions;
            buf[6] = r_Parms.common.strcTagFlag.RetryCount;
            buf[7] = r_Parms.common.strcTagFlag.SelectOpsFlag;
            buf[8] = r_Parms.common.strcTagFlag.PostMatchFlag;

            if ( false == SendData(buf) )
                return Result.FAILURE;

            Array.Clear(buf, 0, buf.Length);

            //Check receive data
            if ( false == ReceiveData( TxCmd, ref buf ) )
                return Result.FAILURE;

            //Check result
            result = ConvertResult(buf[1]);
            if (Result.OK != result)
                return result;

            //Run report packet process
            if ( CheckTagAccessPkt(r_Parms.common ) == false )
                return Result.FAILURE;

            return Result.OK;  
        }





        //clark not sure
        public Result API_l8K6CTagBlockWrite
        (
            [In] BlockWriteParms r_Parms,
            [In] UInt32          r_PerformFlags
        )
        {
            
#if _TRACE_OUT_PUT
            PrintMagToTxt("API_l8K6CTagBlockWrite");
#endif

            //Set Access Password. Tag Read/Write/Kill/Lock need to set Access Password
            if (Result.OK != API_l8K6CSetTagAccessPassword(r_Parms.accessPassword))
                return Result.FAILURE;


            if (r_Parms.blockWriteCmdParms.count > ValueLimit.TAG_DATA_BUF_LENTH)
                return Result.INVALID_PARAMETER;


            //Send data to buffer
            for (byte i = 0; i < r_Parms.blockWriteCmdParms.count; i++)
            {
                API_l8K6CTagWriteDataBuffer( i, 
                                             r_Parms.blockWriteCmdParms.pData[i],
                                             0,
                                             0                                      );
                
            }

            //Start to update data
            return l8K6CTagBlockWrite( r_Parms, r_PerformFlags );
        }





        //clark not sure
        public Result l8K6CTagBlockWrite
        (
            [In] BlockWriteParms r_Parms,
            [In] UInt32          r_PerformFlags
        )
        {
            
#if _TRACE_OUT_PUT
            PrintMagToTxt("l8K6CTagBlockWrite");
#endif

            Result   result = Result.OK;
            ENUM_CMD TxCmd  = ENUM_CMD.NOTHING;
            byte[]   buf    = new byte[10];
            Array.Clear(buf, 0, buf.Length);

            TxCmd  = ENUM_CMD.l8K6C_TAG_BLOCK_WRITE;
            buf[0] = (byte)TxCmd;
            buf[1] = (byte)r_Parms.blockWriteCmdParms.bank;
            buf[2] = (byte)(  r_Parms.blockWriteCmdParms.offset       & 0xFF );
            buf[3] = (byte)( (r_Parms.blockWriteCmdParms.offset >> 8) & 0xFF );
            buf[4] = r_Parms.blockWriteCmdParms.count;
            buf[5] = 0; //Reserved
            buf[6] = r_Parms.common.strcTagFlag.RetryCount;
            buf[7] = r_Parms.common.strcTagFlag.SelectOpsFlag;
            buf[8] = r_Parms.common.strcTagFlag.PostMatchFlag;

         if ( false == SendData(buf) )
                return Result.FAILURE;

            Array.Clear(buf, 0, buf.Length);

            //Check receive data
            if ( false == ReceiveData( TxCmd, ref buf ) )
                return Result.FAILURE;

            //Check result
            result = ConvertResult(buf[1]);
            if (Result.OK != result)
                return result;

            //Run report packet process
            if ( CheckTagAccessPkt(r_Parms.common ) == false )
                return Result.FAILURE;

            return Result.OK;
        }



        public Result API_l8K6CTagBlockErase
        (
            [In] BlockEraseParms r_Parms,
            [In] UInt32          r_PerformFlags
        )
        {
            
#if _TRACE_OUT_PUT
            PrintMagToTxt("API_l8K6CTagBlockErase");
#endif

            //Set Access Password. Tag Read/Write/Kill/Lock need to set Access Password
            if (Result.OK != API_l8K6CSetTagAccessPassword(r_Parms.accessPassword))
                return Result.FAILURE;

            Result   result = Result.OK;
            ENUM_CMD TxCmd  = ENUM_CMD.NOTHING;
            byte[]   buf    = new byte[10];
            Array.Clear(buf, 0, buf.Length);

            TxCmd  = ENUM_CMD.l8K6C_TAG_BLOCK_ERASE;
            buf[0] = (byte)TxCmd;
            buf[1] = (byte)r_Parms.blockEraseCmdParms.bank;
            buf[2] = (byte)(  r_Parms.blockEraseCmdParms.offset       & 0xFF );
            buf[3] = (byte)( (r_Parms.blockEraseCmdParms.offset >> 8) & 0xFF );
            buf[4] = r_Parms.blockEraseCmdParms.count;
            buf[5] = 0; //Reserved
            buf[6] = r_Parms.common.strcTagFlag.RetryCount;  
            buf[7] = r_Parms.common.strcTagFlag.SelectOpsFlag;
            buf[8] = r_Parms.common.strcTagFlag.PostMatchFlag;

         if ( false == SendData(buf) )
                return Result.FAILURE;

            Array.Clear(buf, 0, buf.Length);

            //Check receive data
            if ( false == ReceiveData( TxCmd, ref buf ) )
                return Result.FAILURE;

            //Check result
            result = ConvertResult(buf[1]);
            if (Result.OK != result)
                return result;

            //Run report packet process
            if ( CheckTagAccessPkt(r_Parms.common ) == false )
                return Result.FAILURE;

            return Result.OK;  
        }




        //Reserved for future
        public Result API_l8K6CTagQT
        (
            [In] QTParms r_Parms,
            [In] UInt32  r_PerformFlags
        )
        {

#if _TRACE_OUT_PUT
            PrintMagToTxt("API_l8K6CTagQT");
#endif
            return Result.OK;  
        }




//============================Control Operation=================================== 

        public Result API_ControlCancel
        (

        )
        {

#if _TRACE_OUT_PUT
            PrintMagToTxt("API_ControlCancel");
#endif

            enumINVENTORY_STATUS = ENUM_FLOW_CONTROL.CANCEL; 

            //this command deesn't response
            return Result.OK;
        }

        public Result ControlCancel
        (

        )
        {

#if _TRACE_OUT_PUT
            PrintMagToTxt("ControlCancel");
#endif            

            byte[]   buf    = new byte[10];
            Array.Clear(buf, 0, buf.Length);

            buf[0] = (byte)ENUM_CMD.CONTROL_CANCEL;

            if ( false == SendData(buf) )
                return Result.FAILURE;

            //this command deesn't response
            return Result.OK;
        }



        public Result API_ControlAbort
        (

        )
        {

#if _TRACE_OUT_PUT
            PrintMagToTxt("API_ControlAbort");
#endif   

            //Ask CheckTagAccessPkt function stop.
            enumINVENTORY_STATUS = ENUM_FLOW_CONTROL.ABORT;

            //Result   result = Result.OK;
            //ENUM_CMD TxCmd  = ENUM_CMD.NOTHING;
            //byte[]   buf    = new byte[10];
            //Array.Clear(buf, 0, buf.Length);

            //TxCmd  = ENUM_CMD.CONTROL_ABORT;
            //buf[0] = (byte)TxCmd;

            //if ( false == SendData(buf) )
            //    return Result.FAILURE;

            //Array.Clear(buf, 0, buf.Length);

            ////Check receive data
            //if ( false == ReceiveData( TxCmd, ref buf ) )
            //    return Result.FAILURE;

            ////Check result
            //result = ConvertResult(buf[1]);
            //if (Result.OK != result)
            //    return result;

            return Result.OK;
        }


        public Result ControlAbort
         (

         )
        {

#if _TRACE_OUT_PUT
            PrintMagToTxt("ControlAbort");
#endif   

            ENUM_CMD TxCmd  = ENUM_CMD.NOTHING;
            byte[]   buf    = new byte[10];
            Array.Clear(buf, 0, buf.Length);

            TxCmd  = ENUM_CMD.CONTROL_ABORT;
            buf[0] = (byte)TxCmd;

            if (false == SendData(buf))
                return Result.FAILURE;

            //Array.Clear(buf, 0, buf.Length);

            ////Check receive data
            //if (false == ReceiveData(TxCmd, ref buf))
            //    return Result.FAILURE;

            ////Check result
            //result = ConvertResult(buf[1]);
            //if (Result.OK != result)
            //    return result;

            return Result.OK;
        }

 

        public Result API_ControlPause
        (

        )
        {

#if _TRACE_OUT_PUT
            PrintMagToTxt("API_ControlPause");
#endif   

            //Require CheckTagAccessPkt function pause.
            enumINVENTORY_STATUS = ENUM_FLOW_CONTROL.PAUSE;

            //this command deesn't response
             return Result.OK;
        }


        public Result ControlPause
        (

        )
        {

#if _TRACE_OUT_PUT
            PrintMagToTxt("ControlPause");
#endif   

            byte[] buf = new byte[10];
            Array.Clear(buf, 0, buf.Length);

            buf[0] = (byte)ENUM_CMD.CONTROL_PAUSE;

            if ( false == SendData(buf) )
                return Result.FAILURE;

            //this command deesn't response
             return Result.OK;
        }



         public Result API_ControlResume
        (

        )
        {

#if _TRACE_OUT_PUT
            PrintMagToTxt("API_ControlResume");
#endif   

            //Require CheckTagAccessPkt function resume.
            enumINVENTORY_STATUS = ENUM_FLOW_CONTROL.RESUME;


            //this command deesn't response
            return Result.OK;
        }


        public Result ControlResume
        (

        )
        {

#if _TRACE_OUT_PUT
            PrintMagToTxt("ControlResume");
#endif   

            byte[] buf = new byte[10];
            Array.Clear(buf, 0, buf.Length);

            buf[0] = (byte)ENUM_CMD.CONTROL_RESUME;

            if ( false == SendData(buf) )
                return Result.FAILURE;

            //this command deesn't response
            return Result.OK;
        }



       public Result API_ControlSoftReset
        (

        )
        {

#if _TRACE_OUT_PUT
            PrintMagToTxt("API_ControlSoftReset");
#endif   

            ENUM_CMD TxCmd = ENUM_CMD.NOTHING;
            byte[]          buf   = new byte[10];
            Array.Clear(buf, 0, buf.Length);

            TxCmd  = ENUM_CMD.CONTROL_SOFT_RESET;
            buf[0] = (byte)ENUM_CMD.CONTROL_SOFT_RESET;

            if ( false == SendData(buf) )
                return Result.FAILURE;

            Array.Clear(buf, 0, buf.Length);

            //clark 2011.04.08 If reset successfully, the device doesn't respones.
            if ( false == ReceiveData( TxCmd, ref buf ) )
                return Result.OK;

            //If reset unsuccessfully, the device only return 0xFF.
            if ( ENUM_CMD_RESULT.FAILURE != (ENUM_CMD_RESULT)buf[1] )
                return Result.UNEXPECTED_VALUE;

            return Result.FAILURE;
        }



        //clark not sure
        public Result API_ControlResetToBootLoader
        (

        )
        {

#if _TRACE_OUT_PUT
            PrintMagToTxt("API_ControlResetToBootLoader");
#endif   

            ENUM_CMD TxCmd  = ENUM_CMD.NOTHING;
            byte[]   buf    = new byte[10];
            Array.Clear(buf, 0, buf.Length);

            TxCmd  = ENUM_CMD.CONTROL_RESET_TO_BOOT_LOADER;
            buf[0] = (byte)TxCmd;

            if ( false == SendData(buf) )
                return Result.FAILURE;

            //Don't receive the data. After send command, the reader close AP Mode and turn
            //to BL Mode. It would disconnect with Host, so we can't get response.

            //Array.Clear(buf, 0, buf.Length);


            ////Check receive data
            //if ( false == ReceiveData( TxCmd, ref buf ) )
            //    return Result.FAILURE;

            ////Check result
            //result = ConvertResult(buf[1]);
            //if (Result.OK != result)
            //    return result;

            return Result.OK;
        }




        public Result API_ControlSetPowerState
        (
            [In] RadioPowerState PowerState
        )
        {

#if _TRACE_OUT_PUT
            PrintMagToTxt("API_ControlPowerState");
#endif   

            Result   result = Result.OK;
            ENUM_CMD TxCmd  = ENUM_CMD.NOTHING;
            byte[]   buf    = new byte[10];
            Array.Clear(buf, 0, buf.Length);

            TxCmd  = ENUM_CMD.CONTROL_SET_POWER_STATE;
            buf[0] = (byte)TxCmd;
            buf[1] = (byte)PowerState;

            if ( false == SendData(buf) )
                return Result.FAILURE;

            Array.Clear(buf, 0, buf.Length);

            //Check receive data
            if ( false == ReceiveData( TxCmd, ref buf ) )
                return Result.FAILURE;

            //Check result
            result = ConvertResult(buf[1]);
            if (Result.OK != result)
                return result;

            return Result.OK;
        }


     
        public Result API_ControlGetPowerState
        (
            [In] ref RadioPowerState PowerState
        )
        {

#if _TRACE_OUT_PUT
            PrintMagToTxt("API_ControlPowerState");
#endif   

            Result   result = Result.OK;
            ENUM_CMD TxCmd  = ENUM_CMD.NOTHING;
            byte[]   buf    = new byte[10];
            Array.Clear(buf, 0, buf.Length);

            TxCmd  = ENUM_CMD.CONTROL_GET_POWER_STATE;
            buf[0] = (byte)TxCmd;

            if ( false == SendData(buf) )
                return Result.FAILURE;

            Array.Clear(buf, 0, buf.Length);

            //Check receive data
            if ( false == ReceiveData( TxCmd, ref buf ) )
                return Result.FAILURE;

            //Check result
            result = ConvertResult(buf[1]);
            if (Result.OK != result)
                return result;

            PowerState = (RadioPowerState)buf[2];

            return Result.OK;
        }
   




//==============================Mac; Firmware Access=====================================


        public Result API_MacGetFirmwareVersion
        (
              [In, Out] ref FirmwareVersion r_Version
        )
        {

#if _TRACE_OUT_PUT
            PrintMagToTxt("MacGetFirmwareVersion");
#endif   

            Result   result = Result.OK;
            ENUM_CMD TxCmd  = ENUM_CMD.NOTHING;
            byte[]   buf    = new byte[10];
            Array.Clear(buf, 0, buf.Length);

            TxCmd  = ENUM_CMD.MAC_GET_FIRMWARE_VERSION;
            buf[0] = (byte)TxCmd;

            if ( false == SendData(buf) )
                return Result.FAILURE;

            Array.Clear(buf, 0, buf.Length);

            //Check receive data
            if ( false == ReceiveData( TxCmd, ref buf)  )
                return Result.FAILURE;

            //Check result
            result = ConvertResult(buf[1]);
            if (Result.OK != result)
                return result;

            r_Version.major   = buf[2];
            r_Version.minor   = buf[3];
            r_Version.release = buf[4];

            return Result.OK;
        }


        public Result API_MacgGetDebug
        (
            [In,Out] ref UInt32 r_DebugValue
        )
        {

#if _TRACE_OUT_PUT
            PrintMagToTxt("API_MacgGetDebug");
#endif   

            Result   result = Result.OK;
            ENUM_CMD TxCmd  = ENUM_CMD.NOTHING;
            byte[]   buf    = new byte[10];
            Array.Clear(buf, 0, buf.Length);

            TxCmd  = ENUM_CMD.MAC_GET_DEBUG;
            buf[0] = (byte)TxCmd;

            if ( false == SendData(buf) )
                return Result.FAILURE;

            Array.Clear(buf, 0, buf.Length);

            //Check receive data
            if ( false == ReceiveData( TxCmd, ref buf ) )
                return Result.FAILURE;

            //Check result
            result = ConvertResult(buf[1]);
            if (Result.OK != result)
                return result;

            r_DebugValue = BitConverter.ToUInt32( buf, 2 );

            return Result.OK;
        }




        public Result API_MacClearError
        (

        )
        {

#if _TRACE_OUT_PUT
            PrintMagToTxt("API_MacClearError");
#endif   

            Result   result = Result.OK;
            ENUM_CMD TxCmd  = ENUM_CMD.NOTHING;
            byte[]   buf    = new byte[10];
            Array.Clear(buf, 0, buf.Length);

            TxCmd  = ENUM_CMD.MAC_CLEAR_ERROR;
            buf[0] = (byte)TxCmd;

            if ( false == SendData(buf) )
                return Result.FAILURE;

            Array.Clear(buf, 0, buf.Length);

            //Check receive data
            if ( false == ReceiveData( TxCmd, ref buf ) )
                return Result.FAILURE;

            //Check result
            result = ConvertResult(buf[1]);
            if (Result.OK != result)
                return result;

            return Result.OK;
        }



      
        public Result API_MacGetError
        (
            [In, Out] ref UInt32 CurrentError,
            [In, Out] ref UInt32 lastError
        )
        {

#if _TRACE_OUT_PUT
            PrintMagToTxt("API_MacGetError");
#endif   

            Result result = Result.OK;

            result = MacGetError(ENUM_ERROR_TYPE.CURRENT_ERROR, ref CurrentError);

            if (Result.OK != result)
                return result;


            result = MacGetError(ENUM_ERROR_TYPE.LAST_ERROR, ref lastError);

            if (Result.OK != result)
                return result;

            return Result.OK;        
        }



        public Result MacGetError
        (
            [In]         ENUM_ERROR_TYPE ErrorType,
            [In,Out] ref UInt32          Error
        )
        {

#if _TRACE_OUT_PUT
            PrintMagToTxt("MacGetError");    
#endif   
       
            Result   result = Result.OK;
            ENUM_CMD TxCmd  = ENUM_CMD.NOTHING;
            byte[]   buf    = new byte[10];
            Array.Clear(buf, 0, buf.Length);

            TxCmd  = ENUM_CMD.MAC_GET_ERROR;
            buf[0] = (byte)TxCmd;
            buf[1] = (byte)ErrorType;

            if ( false == SendData(buf) )
                return Result.FAILURE;

            Array.Clear(buf, 0, buf.Length);

            //Check receive data
            if ( false == ReceiveData( TxCmd, ref buf ) )
                return Result.FAILURE;

            //Check result
            result = ConvertResult(buf[1]);
            if (Result.OK != result)
                return result;

            Error = BitConverter.ToUInt32( buf, 2 );

            return Result.OK;        
        }




        public Result API_MacGetBootLoaderVersion
        (
            [In, Out] ref MacBootLoaderVersion r_Version
        )
        {

#if _TRACE_OUT_PUT
            PrintMagToTxt("API_MacGetBootLoaderVersion"); 
#endif   
            
            Result   result = Result.OK;
            ENUM_CMD TxCmd  = ENUM_CMD.NOTHING;
            byte[]   buf    = new byte[10];
            Array.Clear(buf, 0, buf.Length);

            TxCmd  = ENUM_CMD.MAC_GET_BOOT_LOADER_VERSION;
            buf[0] = (byte)TxCmd;

            if ( false == SendData(buf) )
                return Result.FAILURE;

            Array.Clear(buf, 0, buf.Length);

            //Check receive data
            if ( false == ReceiveData( TxCmd, ref buf)  )
                return Result.FAILURE;

            //Check result
            result = ConvertResult(buf[1]);
            if (Result.OK != result)
                return result;

            r_Version.major   = buf[2];
            r_Version.minor   = buf[3];
            r_Version.release = buf[4];

            return Result.OK;
        }





        public Result API_MacWriteOemData
        (
            [In] UInt16 addr,
            [In] UInt32 uiData
        )
        {

#if _TRACE_OUT_PUT
            PrintMagToTxt("API_MacWriteOemData");
#endif   

            Result   result = Result.OK;
            ENUM_CMD TxCmd  = ENUM_CMD.NOTHING;
            byte[]   buf    = new byte[10];
            Array.Clear(buf, 0, buf.Length);

            TxCmd  = ENUM_CMD.MAC_WRITE_OME_DATA;
            buf[0] = (byte)TxCmd;

            buf[1] = (byte)(  addr       & 0xff );
            buf[2] = (byte)( (addr >> 8) & 0xff );

            buf[3] = (byte)(  uiData        & 0xff );
            buf[4] = (byte)( (uiData >>  8) & 0xff );
            buf[5] = (byte)( (uiData >> 16) & 0xff );
            buf[6] = (byte)( (uiData >> 24) & 0xff );


            if ( false == SendData(buf) )
                return Result.FAILURE;

            Array.Clear(buf, 0, buf.Length);

            //Check receive data
            if ( false == ReceiveData( TxCmd, ref buf ) )
                return Result.FAILURE;

            //Check result
            result = ConvertResult(buf[1]);
            if (Result.OK != result)
                return result;

            return Result.OK;
        }




        public Result API_MacReadOemData
        (
            [In]           UInt16 addr,
            [In, Out]  ref UInt32 uiData
        )
        {

#if _TRACE_OUT_PUT
            PrintMagToTxt("API_MacReadOemData");
#endif   

            Result   result = Result.OK;
            ENUM_CMD TxCmd  = ENUM_CMD.NOTHING;
            byte[]   buf    = new byte[10];
            Array.Clear(buf, 0, buf.Length);

            TxCmd  = ENUM_CMD.MAC_READ_OME_DATA;
            buf[0] = (byte)TxCmd;

            buf[1] = (byte)(  addr       & 0xff );
            buf[2] = (byte)( (addr >> 8) & 0xff );


            if ( false == SendData(buf) )
                return Result.FAILURE;

            Array.Clear(buf, 0, buf.Length);

            //Check receive data
            if ( false == ReceiveData( TxCmd, ref buf ) )
                return Result.FAILURE;

            //Check result
            result = ConvertResult(buf[1]);
            if (Result.OK != result)
                return result;

            uiData = BitConverter.ToUInt32(buf, 2);

            return Result.OK;
        }



        public Result API_MacBypassWriteRegister
        (
            [In] UInt16 r_addr,
            [In] UInt16 r_val
        )
        {

#if _TRACE_OUT_PUT
            PrintMagToTxt("API_MacBypassWriteRegister");
#endif   

            Result   result = Result.OK;
            ENUM_CMD TxCmd  = ENUM_CMD.NOTHING;
            byte[]   buf    = new byte[10];
            Array.Clear(buf, 0, buf.Length);

            TxCmd  = ENUM_CMD.MAC_BY_PASS_WRITE_REGISTER;
            buf[0] = (byte)TxCmd;
            buf[1] = (byte)(  r_addr        & 0xff );
            buf[2] = (byte)( (r_addr >> 8)  & 0xff );
            buf[3] = (byte)(  r_val         & 0xff );
            buf[4] = (byte)( (r_val >> 8)   & 0xff );

            if ( false == SendData(buf) )
                return Result.FAILURE;

            Array.Clear(buf, 0, buf.Length);

            //Check receive data
            if ( false == ReceiveData( TxCmd, ref buf ) )
                return Result.FAILURE;

            //Check result
            result = ConvertResult(buf[1]);
            if (Result.OK != result)
                return result;

            return Result.OK;
        }



        public Result API_MacBypassReadRegister
        (
            [In]          UInt16 r_addr,
            [In, Out] ref UInt16 r_val
        )
        {

#if _TRACE_OUT_PUT
            PrintMagToTxt("API_MacBypassReadRegister");
#endif   

            Result   result = Result.OK;
            ENUM_CMD TxCmd  = ENUM_CMD.NOTHING;
            byte[]   buf    = new byte[10];
            Array.Clear(buf, 0, buf.Length);

            TxCmd  = ENUM_CMD.MAC_BY_PASS_READ_REGISTER;
            buf[0] = (byte)TxCmd;
            buf[1] = (byte)(  r_addr       & 0xff );
            buf[2] = (byte)( (r_addr >> 8) & 0xff );

            if ( false == SendData(buf) )
                return Result.FAILURE;

            Array.Clear(buf, 0, buf.Length);

            //Check receive data
            if ( false == ReceiveData( TxCmd, ref buf ) )
                return Result.FAILURE;

            //Check result
            result = ConvertResult(buf[1]);
            if (Result.OK != result)
                return result;

            r_val = BitConverter.ToUInt16(buf, 2);

            return Result.OK;
        }



        public Result API_MacSetRegion
        (
            [In, Out] MacRegion r_Region
        )
        {

#if _TRACE_OUT_PUT
            PrintMagToTxt("API_MacSetRegion");
#endif   

            Result   result = Result.OK;
            ENUM_CMD TxCmd  = ENUM_CMD.NOTHING;
            byte[]   buf    = new byte[10];
            Array.Clear(buf, 0, buf.Length);

            TxCmd  = ENUM_CMD.MAC_SET_REGION;
            buf[0] = (byte)ENUM_CMD.MAC_SET_REGION;
            buf[1] = (byte)r_Region;

            if ( false == SendData(buf) )
                return Result.FAILURE;

            Array.Clear(buf, 0, buf.Length);

            //Check receive data
            if ( false == ReceiveData( TxCmd, ref buf ) )
                return Result.FAILURE;

            //Check result
            result = ConvertResult(buf[1]);
            if (Result.OK != result)
                return result;

            return Result.OK;
        }



        public Result API_MacGetRegion
        (
            [In, Out] ref MacRegion r_Region,
            [In, Out] ref UInt32    r_macRegionSupport
        )
        {  

#if _TRACE_OUT_PUT
            PrintMagToTxt("API_MacGetRegion");
#endif   
           
            Result   result = Result.OK;
            ENUM_CMD TxCmd  = ENUM_CMD.NOTHING;
            byte[]   buf    = new byte[10];
            Array.Clear(buf, 0, buf.Length);

            r_Region = MacRegion.UNKNOWN;

            TxCmd  = ENUM_CMD.MAC_GET_REGION;
            buf[0] = (byte)ENUM_CMD.MAC_GET_REGION;

            if ( false == SendData(buf) )
                return Result.FAILURE;

            Array.Clear(buf, 0, buf.Length);

            //Check receive data
            if ( false == ReceiveData( TxCmd, ref buf ) )
                return Result.FAILURE;

            //Check result
            result = ConvertResult(buf[1]);
            if (Result.OK != result)
                return result;
  
            r_Region           = (MacRegion)buf[2];
            r_macRegionSupport = BitConverter.ToUInt32(buf, 3);
           
            return Result.OK;
        }


        public Result API_MacGetCustomerRegion
        (
            [In, Out] ref string r_CustomerRegion
        )
        {  

#if _TRACE_OUT_PUT
            PrintMagToTxt("API_MacGetCustomerRegion");
#endif   

            uint   uiData = 0;
            Result result = Result.OK;  
            byte   bTmp   = 0;
             

            result = API_MacReadOemData((ushort)enumOEM_ADDR.CUSTOMER_REGION_NAME, ref uiData);

            if(result != Result.OK)
                return result;

            r_CustomerRegion = "";

            for (int i = 0; i < 4; i++)
            { 
                bTmp = (byte)((uiData >> i*8) & 0xFF);

                if(bTmp == 0)
                    break;

                r_CustomerRegion += String.Format("{0}",  (char)bTmp); 
            }

            if( "CUST" == r_CustomerRegion.ToUpper() )
               return Result.NOT_SUPPORTED;

            return Result.OK;
        }



        public Result API_MacGetOEMCfgVersion
        (
              [In, Out] ref OEMCfgVersion r_Version
        )
        {

#if _TRACE_OUT_PUT
            PrintMagToTxt("API_MacGetOEMCfgVersion");
#endif   

            Result   result = Result.OK;
            ENUM_CMD TxCmd  = ENUM_CMD.NOTHING;
            byte[]   buf    = new byte[10];
            Array.Clear(buf, 0, buf.Length);

            TxCmd  = ENUM_CMD.MAC_GET_OEM_CFG_VERSION;
            buf[0] = (byte)TxCmd;

            if ( false == SendData(buf) )
                return Result.FAILURE;

            Array.Clear(buf, 0, buf.Length);

            //Check receive data
            if ( false == ReceiveData( TxCmd, ref buf ) )
                return Result.FAILURE;

            //Check result
            result = ConvertResult(buf[1]);
            if (Result.OK != result)
                return result;

            r_Version.major   = buf[2];
            r_Version.minor   = buf[3];
            r_Version.release = buf[4];

            return Result.OK;
        }


              
        public Result API_MacGetOEMCfgUpdateNumber
        (
            [In, Out] ref OEMCfgUpdateNumber r_Version
        )
        {

#if _TRACE_OUT_PUT
            PrintMagToTxt("API_MacGetOEMCfgUpdateNumber");
#endif   

            Result   result = Result.OK;
            ENUM_CMD TxCmd  = ENUM_CMD.NOTHING;
            byte[]   buf    = new byte[10];
            Array.Clear(buf, 0, buf.Length);

            TxCmd  = ENUM_CMD.MAC_GET_OEM_CFG_UPDATE_NUMBER;
            buf[0] = (byte)TxCmd;

            if ( false == SendData(buf) )
                return Result.FAILURE;

            Array.Clear(buf, 0, buf.Length);

            //Check receive data
            if ( false == ReceiveData( TxCmd, ref buf ) )
                return Result.FAILURE;

            //Check result
            result = ConvertResult(buf[1]);
            if (Result.OK != result)
                return result;

            r_Version.major   = buf[2];
            r_Version.minor   = buf[3];

            return Result.OK;
        }



//======================================GPIO===========================================
      
        public Result API_GpioSetPinsConfiguration
        (
            [In] byte r_Mask,
            [In] byte r_Config
        )
        {

#if _TRACE_OUT_PUT
            PrintMagToTxt("API_GpioSetPinsConfiguration");
#endif   

            Result   result = Result.OK;
            ENUM_CMD TxCmd  = ENUM_CMD.NOTHING;
            byte[]   buf    = new byte[10];
            Array.Clear(buf, 0, buf.Length);

            TxCmd  = ENUM_CMD.GPIO_SET_PINS_CONFIGURATION;
            buf[0] = (byte)TxCmd;
            buf[1] = r_Mask;
            buf[2] = r_Config;

            if ( false == SendData(buf) )
                return Result.FAILURE;

            Array.Clear(buf, 0, buf.Length);

            //Check receive data
            if ( false == ReceiveData( TxCmd, ref buf ) )
                return Result.FAILURE;

            //Check result
            result = ConvertResult(buf[1]);
            if (Result.OK != result)
                return result;

            return Result.OK;
        }




        public Result API_GpioGetPinsConfiguration
        (
            [In, Out] ref byte r_Config
        )
        {

#if _TRACE_OUT_PUT
            PrintMagToTxt("API_GpioGetPinsConfiguration");
#endif   

            Result   result = Result.OK;
            ENUM_CMD TxCmd  = ENUM_CMD.NOTHING;
            byte[]   buf    = new byte[10];
            Array.Clear(buf, 0, buf.Length);

            TxCmd  = ENUM_CMD.GPIO_GET_PINS_CONFIGURATION;
            buf[0] = (byte)TxCmd;

            if ( false == SendData(buf) )
                return Result.FAILURE;

            Array.Clear(buf, 0, buf.Length);

            //Check receive data
            if ( false == ReceiveData( TxCmd, ref buf ) )
                return Result.FAILURE;

            //Check result
            result = ConvertResult(buf[1]);
            if (Result.OK != result)
                return result;

            r_Config = buf[2];

            return Result.OK;
        }




        public Result API_GpioWritePins
        (
            [In] byte r_Mask,
            [In] byte r_Value
        )
        {

#if _TRACE_OUT_PUT
            PrintMagToTxt("API_GpioWritePins");
#endif   

            Result   result = Result.OK;
            ENUM_CMD TxCmd  = ENUM_CMD.NOTHING;
            byte[]   buf    = new byte[10];
            Array.Clear(buf, 0, buf.Length);

            TxCmd  = ENUM_CMD.GPIO_WRITE_PINS;
            buf[0] = (byte)TxCmd;
            buf[1] = r_Mask;
            buf[2] = r_Value;

            if ( false == SendData(buf) )
                return Result.FAILURE;

            Array.Clear(buf, 0, buf.Length);

            //Check receive data
            if ( false == ReceiveData( TxCmd, ref buf ) )
                return Result.FAILURE;

            //Check result
            result = ConvertResult(buf[1]);
            if (Result.OK != result)
                return result;

            return Result.OK;
        }




        public Result API_GpioReadPins
        (
            [In]          byte r_Mask,
            [In, Out] ref byte r_Value
        )
        {

#if _TRACE_OUT_PUT
            PrintMagToTxt("API_GpioReadPins");
#endif   

            Result   result = Result.OK;
            ENUM_CMD TxCmd  = ENUM_CMD.NOTHING;
            byte[]   buf    = new byte[10];
            Array.Clear(buf, 0, buf.Length);

            TxCmd  = ENUM_CMD.GPIO_READ_PINS;
            buf[0] = (byte)TxCmd;
            buf[1] = r_Mask;

            if ( false == SendData(buf) )
                return Result.FAILURE;

            Array.Clear(buf, 0, buf.Length);

            //Check receive data
            if ( false == ReceiveData( TxCmd, ref buf ) )
                return Result.FAILURE;

            //Check result
            result = ConvertResult(buf[1]);
            if (Result.OK != result)
                return result;

            r_Value = buf[2];

            return Result.OK;
        }







//======================================TEST===========================================
   
        public Result API_TestSetAntennaPortConfiguration
        (
            [In] byte   r_btPhysicalPort,
            [In] UInt16 r_usPowerLevel
        )
        {

#if _TRACE_OUT_PUT
            PrintMagToTxt("API_TestSetAntennaPortConfiguration");
#endif   

            Result   result = Result.OK;
            ENUM_CMD TxCmd  = ENUM_CMD.NOTHING;
            byte[]   buf    = new byte[10];
            Array.Clear(buf, 0, buf.Length);

            TxCmd  = ENUM_CMD.TEST_SET_ANTENNA_PORT_CONFIGURATION;
            buf[0] = (byte)TxCmd;
            buf[1] = r_btPhysicalPort;
            buf[2] = (byte)(  r_usPowerLevel       & 0xFF );
            buf[3] = (byte)( (r_usPowerLevel >> 8) & 0xFF );

            if ( false == SendData(buf) )
                return Result.FAILURE;

            Array.Clear(buf, 0, buf.Length);

            //Check receive data
            if ( false == ReceiveData( TxCmd, ref buf ) )
                return Result.FAILURE;

            //Check result
            result = ConvertResult(buf[1]);
            if (Result.OK != result)
                return result;

            return Result.OK;
        }     

        
        public Result API_TestGetAntennaPortConfiguration
        (
            [In] ref byte   r_btPhysicalPort,
            [In] ref UInt16 r_usPowerLevel
        )
        {

#if _TRACE_OUT_PUT
            PrintMagToTxt("API_TestGetAntennaPortConfiguration");
#endif   

            Result   result = Result.OK;
            ENUM_CMD TxCmd  = ENUM_CMD.NOTHING;
            byte[]   buf    = new byte[10];
            Array.Clear(buf, 0, buf.Length);

            TxCmd  = ENUM_CMD.TEST_GET_ANTENNA_PORT_CONFIGURATION;
            buf[0] = (byte)TxCmd;

            if ( false == SendData(buf) )
                return Result.FAILURE;

            Array.Clear(buf, 0, buf.Length);

            //Check receive data
            if ( false == ReceiveData( TxCmd, ref buf ) )
                return Result.FAILURE;

            //Check result
            result = ConvertResult(buf[1]);
            if (Result.OK != result)
                return result;
            
            r_btPhysicalPort = buf[2];
            r_usPowerLevel   = BitConverter.ToUInt16(buf, 3);

            return Result.OK;
        }



        public Result API_TestSetFrequencyConfiguration
        (
            [In] byte   r_btChannelFlag,
            [In] UInt32 r_uiExactFrequecny
        )
        {

#if _TRACE_OUT_PUT
            PrintMagToTxt("API_TestSetFrequencyConfiguration");
#endif   

            Result   result = Result.OK;
            ENUM_CMD TxCmd  = ENUM_CMD.NOTHING;
            byte[]   buf    = new byte[10];
            Array.Clear(buf, 0, buf.Length);

            TxCmd  = ENUM_CMD.TEST_SET_FREQUENCY_CONFIGURATION;
            buf[0] = (byte)TxCmd;
            buf[1] = r_btChannelFlag;
            buf[2] = (byte)(  r_uiExactFrequecny        & 0xFF ) ;
            buf[3] = (byte)( (r_uiExactFrequecny >>  8) & 0xFF );
            buf[4] = (byte)( (r_uiExactFrequecny >> 16) & 0xFF );
            buf[5] = (byte)( (r_uiExactFrequecny >> 24) & 0xFF );

            if ( false == SendData(buf) )
                return Result.FAILURE;

            Array.Clear(buf, 0, buf.Length);

            //Check receive data
            if ( false == ReceiveData( TxCmd, ref buf ) )
                return Result.FAILURE;

            //Check result
            result = ConvertResult(buf[1]);
            if (Result.OK != result)
                return result;

            return Result.OK;
        }     

        
        public Result API_TestGetFrequencyConfiguration
        (
            [In] ref byte   r_btChannelFlag,
            [In] ref UInt32 r_uiExactFrequecn
        )
        {

#if _TRACE_OUT_PUT
            PrintMagToTxt("API_TestGetFrequencyConfiguration");
#endif   

            Result   result = Result.OK;
            ENUM_CMD TxCmd  = ENUM_CMD.NOTHING;
            byte[]   buf    = new byte[10];
            Array.Clear(buf, 0, buf.Length);

            TxCmd  = ENUM_CMD.TEST_GET_FREQUENCY_CONFIGURATION;
            buf[0] = (byte)TxCmd;

            if ( false == SendData(buf) )
                return Result.FAILURE;

            Array.Clear(buf, 0, buf.Length);

            //Check receive data
            if ( false == ReceiveData( TxCmd, ref buf ) )
                return Result.FAILURE;

            //Check result
            result = ConvertResult(buf[1]);
            if (Result.OK != result)
                return result;
            
            r_btChannelFlag   = buf[2];
            r_uiExactFrequecn = BitConverter.ToUInt32(buf, 3);

            return Result.OK;
        }

       

        public Result API_TestSetRandomDataPulseTime
        (
            [In] UInt16 r_usOnTime,
            [In] UInt16 r_usOffTime
        )
        {

#if _TRACE_OUT_PUT
            PrintMagToTxt("API_TestSetRandomDataPulseTime");
#endif   

            Result   result = Result.OK;
            ENUM_CMD TxCmd  = ENUM_CMD.NOTHING;
            byte[]   buf    = new byte[10];
            Array.Clear(buf, 0, buf.Length);

            TxCmd  = ENUM_CMD.TEST_SET_RANDOM_DATA_PULSE_TIME;
            buf[0] = (byte)TxCmd;
            buf[1] = (byte)(  r_usOnTime         & 0xFF );
            buf[2] = (byte)( (r_usOnTime  >>  8) & 0xFF );
            buf[3] = (byte)(  r_usOffTime        & 0xFF );
            buf[4] = (byte)( (r_usOffTime >>  8) & 0xFF );

            if ( false == SendData(buf) )
                return Result.FAILURE;

            Array.Clear(buf, 0, buf.Length);

            //Check receive data
            if ( false == ReceiveData( TxCmd, ref buf ) )
                return Result.FAILURE;

            //Check result
            result = ConvertResult(buf[1]);
            if (Result.OK != result)
                return result;

            return Result.OK;
        }     

        
        public Result API_TestGetRandomDataPulseTime
        (
            [In] ref UInt16 r_usOnTime,
            [In] ref UInt16 r_usOffTime
        )
        {

#if _TRACE_OUT_PUT
            PrintMagToTxt("API_TestGetRandomDataPulseTime");
#endif   

            Result   result = Result.OK;
            ENUM_CMD TxCmd  = ENUM_CMD.NOTHING;
            byte[]   buf    = new byte[10];
            Array.Clear(buf, 0, buf.Length);

            TxCmd  = ENUM_CMD.TEST_GET_RANDOM_DATA_PULSE_TIME;
            buf[0] = (byte)TxCmd;

            if ( false == SendData(buf) )
                return Result.FAILURE;

            Array.Clear(buf, 0, buf.Length);

            //Check receive data
            if ( false == ReceiveData( TxCmd, ref buf ) )
                return Result.FAILURE;

            //Check result
            result = ConvertResult(buf[1]);
            if (Result.OK != result)
                return result;
            
            r_usOnTime  = BitConverter.ToUInt16(buf, 2);
            r_usOffTime = BitConverter.ToUInt16(buf, 4);

            return Result.OK;
        }

     
        public Result API_TestSetInventoryConfiguration
        (
            [In] byte r_btContinuousOperation
        )
        {

#if _TRACE_OUT_PUT
            PrintMagToTxt("API_TestSetInventoryConfiguration");
#endif   

            Result   result = Result.OK;
            ENUM_CMD TxCmd  = ENUM_CMD.NOTHING;
            byte[]   buf    = new byte[10];
            Array.Clear(buf, 0, buf.Length);

            TxCmd  = ENUM_CMD.TEST_SET_INVENTORY_CONFIGURATION;
            buf[0] = (byte)TxCmd;
            buf[1] = r_btContinuousOperation;

            if ( false == SendData(buf) )
                return Result.FAILURE;

            Array.Clear(buf, 0, buf.Length);

            //Check receive data
            if ( false == ReceiveData( TxCmd, ref buf ) )
                return Result.FAILURE;

            //Check result
            result = ConvertResult(buf[1]);
            if (Result.OK != result)
                return result;

            return Result.OK;
        }     

        
        public Result API_TestGetInventoryConfiguration
        (
            [In] ref byte r_btContinuousOperation
        )
        {

#if _TRACE_OUT_PUT
            PrintMagToTxt("API_TestGetInventoryConfiguration");
#endif   

            Result   result = Result.OK;
            ENUM_CMD TxCmd  = ENUM_CMD.NOTHING;
            byte[]   buf    = new byte[10];
            Array.Clear(buf, 0, buf.Length);

            TxCmd  = ENUM_CMD.TEST_GET_INVENTORY_CONFIGURATION;
            buf[0] = (byte)TxCmd;

            if ( false == SendData(buf) )
                return Result.FAILURE;

            Array.Clear(buf, 0, buf.Length);

            //Check receive data
            if ( false == ReceiveData( TxCmd, ref buf ) )
                return Result.FAILURE;

            //Check result
            result = ConvertResult(buf[1]);
            if (Result.OK != result)
                return result;
            
            r_btContinuousOperation = buf[2];

            return Result.OK;
        }



        public Result API_TestTurnCarrierWaveOn
        (

        )
        {

#if _TRACE_OUT_PUT
            PrintMagToTxt("API_TestTurnCarrierWaveOn");
#endif   

            Result   result = Result.OK;
            ENUM_CMD TxCmd  = ENUM_CMD.NOTHING;
            byte[]   buf    = new byte[10];
            Array.Clear(buf, 0, buf.Length);

            TxCmd  = ENUM_CMD.TEST_TURN_CARRIER_WAVE_ON;
            buf[0] = (byte)TxCmd;

            if ( false == SendData(buf) )
                return Result.FAILURE;

            Array.Clear(buf, 0, buf.Length);

            //Check receive data
            if ( false == ReceiveData( TxCmd, ref buf ) )
                return Result.FAILURE;

            //Check result
            result = ConvertResult(buf[1]);
            if (Result.OK != result)
                return result;

            return Result.OK;
        }


        public Result API_TestTurnCarrierWaveOff
        (

        )
        {

#if _TRACE_OUT_PUT
            PrintMagToTxt("API_TestTurnCarrierWaveOff");
#endif   

            Result   result = Result.OK;
            ENUM_CMD TxCmd  = ENUM_CMD.NOTHING;
            byte[]   buf    = new byte[10];
            Array.Clear(buf, 0, buf.Length);

            TxCmd  = ENUM_CMD.TEST_TURN_CARRIER_WAVE_OFF;
            buf[0] = (byte)TxCmd;

            if ( false == SendData(buf) )
                return Result.FAILURE;

            Array.Clear(buf, 0, buf.Length);

            //Check receive data
            if ( false == ReceiveData( TxCmd, ref buf ) )
                return Result.FAILURE;

            //Check result
            result = ConvertResult(buf[1]);
            if (Result.OK != result)
                return result;

            return Result.OK;
        }


        public Result API_TestInjectRandomData
        (
            [In] UInt32 r_btCount
        )
        {

#if _TRACE_OUT_PUT
            PrintMagToTxt("API_TestInjectRandomData");
#endif   

            Result   result = Result.OK;
            ENUM_CMD TxCmd  = ENUM_CMD.NOTHING;
            byte[]   buf    = new byte[10];
            Array.Clear(buf, 0, buf.Length);

            TxCmd  = ENUM_CMD.TEST_INJECT_RANDOM_DATA;
            buf[0] = (byte)TxCmd;
            buf[1] = (byte)(  r_btCount        & 0xFF );
            buf[2] = (byte)( (r_btCount >>  8) & 0xFF );
            buf[3] = (byte)( (r_btCount >> 16) & 0xFF );
            buf[4] = (byte)( (r_btCount >> 24) & 0xFF );


            if ( false == SendData(buf) )
                return Result.FAILURE;

            Array.Clear(buf, 0, buf.Length);

            //Check receive data
            if ( false == ReceiveData( TxCmd, ref buf ) )
                return Result.FAILURE;

            //Check result
            result = ConvertResult(buf[1]);
            if (Result.OK != result)
                return result;

            return Result.OK;
        }
   
             
        public Result API_TestTransmitRandomData
        (
            [In] CommonParms r_Params,
            [In] byte        r_btControl,
            [In] UInt32      r_uiDuration
        )
        {

#if _TRACE_OUT_PUT
            PrintMagToTxt("API_TestTransmitRandomData");
#endif   

            Result   result = Result.OK;
            ENUM_CMD TxCmd  = ENUM_CMD.NOTHING;
            byte[]   buf    = new byte[10];
            Array.Clear(buf, 0, buf.Length);

            TxCmd  = ENUM_CMD.TEST_TRANSMIT_RANDOM_DATA;
            buf[0] = (byte)TxCmd;
            buf[1] = r_btControl;
            buf[2] = (byte)(  r_uiDuration        & 0xFF );
            buf[3] = (byte)( (r_uiDuration >>  8) & 0xFF );
            buf[4] = (byte)( (r_uiDuration >> 16) & 0xFF );
            buf[5] = (byte)( (r_uiDuration >> 24) & 0xFF );

            if ( false == SendData(buf) )
                return Result.FAILURE;

            Array.Clear(buf, 0, buf.Length);

            //Check receive data
            if ( false == ReceiveData( TxCmd, ref buf ) )
                return Result.FAILURE;

            //Check result
            result = ConvertResult(buf[1]);
            if (Result.OK != result)
                return result;


            //Run report packet process
            if ( CheckTagAccessPkt(r_Params) == false )
                return Result.FAILURE;


            return Result.OK;
        }

   

//==========================================================================

        protected bool SendData
        (
            byte[] btData
        )
        {
            ushort usCRC = 0;
            byte[] wtBuf = new byte[16];
            ushort usCrcLen = (ushort)(ENMU_PKT_INDEX.PKT_CRC - ENMU_PKT_INDEX.PKT_HEADER);

            Array.Clear(wtBuf, 0, wtBuf.Length);
            wtBuf[0] = 0x43;
            wtBuf[1] = 0x49;
            wtBuf[2] = 0x54;
            wtBuf[3] = 0x4D;
            wtBuf[4] = 0xFF;

            Array.Copy(btData, 0,
                        wtBuf, (int)ENMU_PKT_INDEX.PKT_COMMAND_ID,
                        btData.Length);

            usCRC = CrcCul(wtBuf,
                            0,
                            (ushort)(usCrcLen * 8));

            wtBuf[14] = (byte)(~usCRC & 0xFF);
            wtBuf[15] = (byte)(~usCRC >> 8 & 0xFF);

            if (clsPacket.TRANS_API_Write(wtBuf, (UInt32)wtBuf.Length) != true)
            {
                //Try to connect the device again
                clsPacket.TRANS_API_Close();

                //Check Try-Link count. Avoid to  fall into link loop.
                if (m_btTryLinkCount > DEFINE_TRY_COUNT)
                {
                    m_btTryLinkCount = 0;
                    return false;
                }
                else
                { 
                    m_btTryLinkCount++;
                }


                //wait for device
                TimeBeginPeriod(1);
                System.Threading.Thread.Sleep( 100 );
                TimeEndPeriod(1);

                //Link the device again
                if ( LinkDevice() == false )
                    return false;                
               
                //Write again
                if (clsPacket.TRANS_API_Write(wtBuf, (UInt32)wtBuf.Length) == true)
                {
                    //Reset the flag
                    m_btTryLinkCount = 0;
                    return true;
                }
                else
                { 
                    return false;
                }

            }
            else
            {
                
                m_btTryLinkCount = 0;
                return true;
            }

        }


        protected bool ReceiveData
        (
            [In]          ENUM_CMD r_TxCmd,
            [In, Out] ref byte[]   r_btData
        )
        {

            ENUM_CMD result = ENUM_CMD.NOTHING;
            byte []  rdBuf  = new byte[128];
            Array.Clear(rdBuf, 0, rdBuf.Length);

            //Reset Receive flow
            ResetRxFlow(ENMU_PKT_TYPE.COMMON);

            while (result != r_TxCmd)
            {
                result = Regular(  ref rdBuf, true);

#if _TRACE_OUT_PUT
                PrintMagToTxt("ReceiveData");
#endif   

                if (result == ENUM_CMD.TIME_OUT) //Timeout
                {

#if _TRACE_OUT_PUT
                    PrintMagToTxt("TIME OUT ReceiveData");
#endif   

                    //Try to connect the device again
                    clsPacket.TRANS_API_Close();
                    LinkDevice();

                    return false;
                }

                TimeBeginPeriod(1);
                System.Threading.Thread.Sleep( (Int32)Settings.Default.uiInterval );
                TimeEndPeriod(1);
            }


            //Command ID ~ Data over
            int Length = ENMU_PKT_INDEX.PKT_CRC - ENMU_PKT_INDEX.PKT_COMMAND_ID;


            Array.Copy(rdBuf, (int)ENMU_PKT_INDEX.PKT_COMMAND_ID,
                        r_btData, 0,
                        Length);


            return true;
        }



        protected ushort CrcCul
        (
            [In, Out] byte[] buf,
            [In]      uint   uiOffset,
            [In]      ushort bit_length
        )
        {
            ushort    shift = 0;
            ushort    data  = 0;
            ushort    val   = 0;
            const int POLY  = 0x1021;

            shift = 0xFFFF;

            for (int i = 0, j = 0; i < bit_length; i++)
            {
                if ( (i % 8) == 0 )
                {                    
                    data = (ushort)( buf[uiOffset + j] << 8 );
                    j++;
                }

                val   = (ushort)(  shift ^ data );
                shift = (ushort)( (shift << 1) & 0xFFFF );
                data  = (ushort)( (data  << 1) & 0xFFFF );
               
                if ( (val & 0x8000) > 0 ? true : false )
                    shift = (ushort)( shift ^ POLY );
                
            }
            
            return (ushort)(shift & 0xFFFF);
        }



        //Reset receive flow
        protected void ResetRxFlow(ENMU_PKT_TYPE r_Type)
        {
            RxPktTpye    = r_Type;
            RxStep       = ENMU_PKT_FLOW.STEP_HEADER_1;
            m_timeBefReg = DateTime.Now;

            //Reset flag while restart tag access/inventory
            if (RxPktTpye == ENMU_PKT_TYPE.TAG_ACCESS)
            {
                bCheckTimeOut        = false;
                enumINVENTORY_STATUS = ENUM_FLOW_CONTROL.NOTHING;
            }
        }


        //Convert firmware result to MTI result
        protected Result ConvertResult
        (
           [In]          byte MTIresult
        )
        {
            Result result = Result.FAILURE;

            switch (MTIresult)
            {
                case (byte)Global.ENUM_CMD_RESULT.OK:
                case (byte)Global.ENUM_CMD_RESULT.FWUPD_ENTER_OK:
                case (byte)Global.ENUM_CMD_RESULT.FWUPD_EXIT_SUCCESS:
                    result = Result.OK;
                    break;

                case (byte)Global.ENUM_CMD_RESULT.INVALID_PARAM:
                    result = Result.INVALID_PARAMETER;
                    break;

                case (byte)Global.ENUM_CMD_RESULT.FAILURE:
                    result = Result.FAILURE;
                    break;

                case (byte)Global.ENUM_CMD_RESULT.FWUPD_WR_FAIL:
                    result = Result.FWUPD_WR_FAIL;
                    break;

                case (byte)Global.ENUM_CMD_RESULT.FWUPD_INVALID_PARAM:
                    result = Result.FWUPD_INVALID_PARAM;
                    break;

                case (byte)Global.ENUM_CMD_RESULT.FWUPD_CMD_IGN:
                    result = Result.FWUPD_CMD_IGN;
                    break;
 
                case (byte)Global.ENUM_CMD_RESULT.FWUPD_BNDS:
                    result = Result.FWUPD_BNDS;
                    break;

                case (byte)Global.ENUM_CMD_RESULT.FWUPD_MAGIC:
                    result = Result.FWUPD_MAGIC;
                    break;

                case (byte)Global.ENUM_CMD_RESULT.FWUPD_DATA_LEN:
                    result = Result.FWUPD_DATA_LEN;
                    break;

                case (byte)Global.ENUM_CMD_RESULT.FWUPD_EXIT_ERR:
                    result = Result.FWUPD_EXIT_ERR;
                    break;

                case (byte)Global.ENUM_CMD_RESULT.FWUPD_EXIT_NOWRITES:
                    result = Result.FWUPD_EXIT_NOWRITES;
                    break;

                case (byte)Global.ENUM_CMD_RESULT.FWUPD_GEN_RXPKT_ERR:
                    result = Result.FWUPD_GEN_RXPKT_ERR;
                    break;


                case (byte)Global.ENUM_CMD_RESULT.FWUPD_INT_MEM_BNDS:
                    result = Result.FWUPD_INT_MEM_BNDS;
                    break;

                case (byte)Global.ENUM_CMD_RESULT.FWUPD_RX_TO:
                    result = Result.FWUPD_RX_TO;
                    break;


                case (byte)Global.ENUM_CMD_RESULT.FWUPD_CRC_ERR:
                    result = Result.FWUPD_CRC_ERR;
                    break;

                case (byte)Global.ENUM_CMD_RESULT.FWUPD_SYS_ERR:
                    result = Result.FWUPD_SYS_ERR;
                    break;
            }

            return result;
        }



        protected bool CheckPktEnd
        (
            [In] byte[] buf
        )
        {
            return (usDivideSeq) == usDivideNum ? true : false;
        }



        protected bool AddDataToMultiBuf
        (
            [In, Out] ref ArrayList MultiBuf,
            [In]          byte[]    SingleBuf
        )
        {
            if ( null == SingleBuf )
                return false;

            int usByteLen = usDataSingleLenDw * 4;
            byte[] tmpBuf = new byte[usByteLen];
            Array.Clear(tmpBuf, 0, tmpBuf.Length);

            try
            {
                //only copy data to buffer
                Array.Copy(SingleBuf, (int)ENMU_TAG_PKT_INDEX.INFO_DATA,
                            tmpBuf, 0,
                            usByteLen);

                //combine data
                MultiBuf.AddRange(tmpBuf);

                //Reset total data length
                MultiBuf[(int)ENMU_TAG_PKT_INDEX.INFO_LENGTH]
                                    = (byte)(usDataTotalLenDw & 0xFF);
                MultiBuf[(int)ENMU_TAG_PKT_INDEX.INFO_LENGTH + 1]
                                    = (byte)((usDataTotalLenDw >> 8) & 0xFF);
            }
            catch (Exception e)
            {
                return false;
            }

            return true;
        }



        protected bool CheckTagAccessPkt
        (
            [In] CommonParms strcParms
        )
        {
            ENUM_CMD result = ENUM_CMD.NOTHING;

            ArrayList MultiBuf    = new ArrayList();
            byte[]    SingleBuf   = new Byte[64];
            Array.Clear(SingleBuf, 0, SingleBuf.Length);


            //Reset Receive flow
            ResetRxFlow(ENMU_PKT_TYPE.TAG_ACCESS);

            try
            {
                while (true) 
                {

    #if _TRACE_OUT_PUT
                    PrintMagToTxt("CheckTagAccessPkt");
    #endif   

                    //Start Time Out flow. (Not command timeout. It's process timeout)
                    if(bCheckTimeOut)
                    {
                        TimeSpan ts = new TimeSpan(DateTime.Now.Ticks - m_timeBefAccess.Ticks);
                        if (ts.TotalSeconds > 3)
                        {

            #if _TRACE_OUT_PUT
                            PrintMagToTxt("CheckTagAccessPkt: TIME_OUT");
            #endif   

                            //Send Abort to check that skip inventory status.
                            this.ControlAbort();
                            clsPacket.TRANS_API_ClearBuffer();
                            
                            return true;
                        }
                    }


                    
                    switch (enumINVENTORY_STATUS)
                    {
                        case ENUM_FLOW_CONTROL.PAUSE:
            #if _TRACE_OUT_PUT
                            PrintMagToTxt("CheckTagAccessPkt: PAUSE");
            #endif   
                            enumINVENTORY_STATUS = ENUM_FLOW_CONTROL.NOTHING;
                            ControlPause();
                            break;

                        case ENUM_FLOW_CONTROL.RESUME:
            #if _TRACE_OUT_PUT
                            PrintMagToTxt("CheckTagAccessPkt: RESUME");
            #endif   
                            enumINVENTORY_STATUS = ENUM_FLOW_CONTROL.NOTHING;
                            ControlResume();
                            break;

                        case ENUM_FLOW_CONTROL.ABORT:
            #if _TRACE_OUT_PUT
                            PrintMagToTxt("CheckTagAccessPkt: ABORT");
            #endif   
                            enumINVENTORY_STATUS = ENUM_FLOW_CONTROL.NOTHING;

                            //Abort all inventory data from memory.
                            clsPacket.TRANS_API_ClearBuffer();
                            ControlAbort();
                            bCheckTimeOut   = true;
                            m_timeBefAccess = DateTime.Now;
                            break;

                        case ENUM_FLOW_CONTROL.CANCEL:
            #if _TRACE_OUT_PUT
                            PrintMagToTxt("CheckTagAccessPkt: CANCEL");
            #endif   
                            enumINVENTORY_STATUS = ENUM_FLOW_CONTROL.NOTHING;
                            ControlCancel();
                            bCheckTimeOut   = true;                            
                            m_timeBefAccess = DateTime.Now;
                            break;

                    }


                    //Only wait End Packet or Abort Common Packet to stop the process.
                    //don't need command timeout.
                    result = Regular( ref SingleBuf, false );
                    
                    switch (result)
                    {
                        //case ENUM_CMD.TIME_OUT:                         
                        //    if (strcParms.OpMode == RadioOperationMode.CONTINUOUS ||
                        //        strcParms.OpMode == RadioOperationMode.NONCONTINUOUS   )
                        //        break;

                        //    PrintMagToTxt("CheckTagAccessPkt: TIME_OUT");
                        //    clsPacket.TRANS_API_ClearBuffer();
                        //    return false;

                        case ENUM_CMD.CONTROL_ABORT: 
            #if _TRACE_OUT_PUT
                            PrintMagToTxt("CheckTagAccessPkt: bAbort");
            #endif                              
                            return true;
                            break;


                        case ENUM_CMD.BEGIN:
            #if _TRACE_OUT_PUT
                            PrintMagToTxt("CheckTagAccessPkt: BEGIN");
            #endif    

                            //If get first packet, reset the flow.
                            if (usDivideSeq == 1)
                            {
                                usDataTotalLenDw = usDataSingleLenDw;

                                if (0 != MultiBuf.Count)
                                    MultiBuf.RemoveRange(0, MultiBuf.Count);
                            }
                            else
                            {
                                //The packet sequence is more then 1, but buffer doesn't have data.
                                //It means lossing some packets. Ignore this inventory or tag access.
                                if (0 == MultiBuf.Count)
                                    break;

                                usDataTotalLenDw += usDataSingleLenDw;
                            }

                            HandleCallBack(strcParms, SingleBuf);

                            Array.Clear(SingleBuf, 0, SingleBuf.Length);
                            usDataTotalLenDw = 0;

                            if (0 != MultiBuf.Count)
                                MultiBuf.RemoveRange(0, MultiBuf.Count);

                            break;


                        case ENUM_CMD.END:
            #if _TRACE_OUT_PUT
                            PrintMagToTxt("CheckTagAccessPkt: END");
            #endif    

                            //If get first packet, reset the flow.
                            if (usDivideSeq == 1)
                            {
                                usDataTotalLenDw = usDataSingleLenDw;

                                if (0 != MultiBuf.Count)
                                    MultiBuf.RemoveRange(0, MultiBuf.Count);
                            }
                            else
                            {
                                //The packet sequence is more then 1, but buffer doesn't have data.
                                //It means lossing some packets. Ignore this inventory or tag access.
                                if (0 == MultiBuf.Count)
                                    break;

                                usDataTotalLenDw += usDataSingleLenDw;
                            }

                            HandleCallBack(strcParms, SingleBuf);

                            Array.Clear(SingleBuf, 0, SingleBuf.Length);
                            usDataTotalLenDw = 0;

                            if (0 != MultiBuf.Count)
                                MultiBuf.RemoveRange(0, MultiBuf.Count);

                            return true;
                            break;


                        case ENUM_CMD.UPDAUE_ENTER_UPDATE_MODE:
            #if _TRACE_OUT_PUT
                            PrintMagToTxt("CheckTagAccessPkt: UPDAUE_ENTER_UPDATE_MODE");
            #endif    

                            //If get first packet, reset the flow.
                            if (usDivideSeq == 1)
                            {
                                usDataTotalLenDw = usDataSingleLenDw;

                                if (0 != MultiBuf.Count)
                                    MultiBuf.RemoveRange(0, MultiBuf.Count);
                            }
                            else
                            {
                                //The packet sequence is more then 1, but buffer doesn't have data.
                                //It means lossing some packets. Ignore this inventory or tag access.
                                if (0 == MultiBuf.Count)
                                    break;

                                usDataTotalLenDw += usDataSingleLenDw;
                            }

                            HandleCallBack(strcParms, SingleBuf);

                            Array.Clear(SingleBuf, 0, SingleBuf.Length);
                            usDataTotalLenDw = 0;

                            if (0 != MultiBuf.Count)
                                MultiBuf.RemoveRange(0, MultiBuf.Count);
                            break;


                        case ENUM_CMD.INVENTORY:
                        case ENUM_CMD.TAG_ACCESS:
            #if _TRACE_OUT_PUT
                            PrintMagToTxt("CheckTagAccessPkt: INVENTORY/TAG_ACCESS");
            #endif    
                         
                            //If get first packet, reset the flow.
                            if (usDivideSeq == 1)
                            {
                                usDataTotalLenDw = usDataSingleLenDw;

                                if (0 != MultiBuf.Count)
                                    MultiBuf.RemoveRange(0, MultiBuf.Count);
                            }
                            else
                            {

                                //The packet sequence is more then 1, but buffer doesn't have data.
                                //It means lossing some packets. Ignore this inventory or tag access.
                                if (0 == MultiBuf.Count)
                                    break;

                                usDataTotalLenDw += usDataSingleLenDw;
                            }
                            

                            if ( true == CheckPktEnd(SingleBuf) )   //report end
                            {
                                //Check multi-buffer size
                                if (0 == MultiBuf.Count)   //Single packet
                                {
                                    HandleCallBack(strcParms, SingleBuf);
                                }
                                else    //Multi packet
                                {
                                    //combine data
                                    if (false == AddDataToMultiBuf(ref MultiBuf, SingleBuf))
                                    {
                                        if (0 != MultiBuf.Count)
                                            MultiBuf.RemoveRange(0, MultiBuf.Count);

                                        break;
                                    }


                                    //Copy ArrayList bufeer to byte [] buffer
                                    byte[] tmpBuf = MultiBuf.ToArray(typeof(byte)) as byte[];
                                    if (null == tmpBuf)     
                                    {
                                        if (0 != MultiBuf.Count)
                                            MultiBuf.RemoveRange(0, MultiBuf.Count);

                                        break;
                                    }                              

                                    HandleCallBack(strcParms, tmpBuf);

                                    MultiBuf.RemoveRange(0, MultiBuf.Count);
                                }

                                usDataTotalLenDw = 0;
                            }
                            else   //report not end. Continue.
                            {

                                //Check multi-buffer size
                                if (0 == MultiBuf.Count)   //at first packet
                                {
                                    //Push data to multi buffer
                                    MultiBuf.AddRange(SingleBuf);

                                    //Remove CRC (last 2 bytes). Don't need CRC.
                                    //Because want to combine data form different 
                                    //packet, the data in next packet will replace
                                    //CRC address.
                                    MultiBuf.RemoveRange(MultiBuf.Count - 3, 2);
                                }
                                else   //Combine packets
                                {
                                    //combine data
                                    if (false == AddDataToMultiBuf(ref MultiBuf, SingleBuf))
                                    {
                                        if (0 != MultiBuf.Count)
                                            MultiBuf.RemoveRange(0, MultiBuf.Count);

                                        break;
                                    }
                                }
                            }

                            //Clear single buffer to receive next packet
                            Array.Clear(SingleBuf, 0, SingleBuf.Length);
                            break;

                        default:
                            break;
                    }


                    TimeBeginPeriod(1);
                    System.Threading.Thread.Sleep( (int)Settings.Default.uiInterval );
                    TimeEndPeriod(1);

                }//while

            }//try
            catch (Exception e)
            {
                clsPacket.TRANS_API_ClearBuffer();
                ControlAbort();

                return false;
            }

            return true;
        }


        private void HandleCallBack
        (
            [In]      CommonParms Params,
            [In, Out] byte[]      RxBuf
        )
        {
            if (null != Params.callback)
            {
                //usDataSingleLenDw is DWORD. 1 DWORD = 4 bytes
                //DataLength = Data(usDataTotalLenDw) + 
                //             (Info Ver + Info Flag + Info Tyte + Pkt Data Single Len + Pkt Seq )                
                UInt32 DataLength = (UInt32)(usDataTotalLenDw * 4) + 
                                    (ENMU_TAG_PKT_INDEX.INFO_DATA - ENMU_TAG_PKT_INDEX.INFO_VER);

                IntPtr bufPtr  = Marshal.AllocHGlobal( (Int32)DataLength );
                byte[] PktData = new byte[DataLength];
                Array.Clear(PktData, 0, PktData.Length);

                //Explorer only need bytes from version to data.
                Array.Copy( RxBuf,      (int)ENMU_TAG_PKT_INDEX.INFO_VER,
                            PktData,    0,
                            DataLength                                     );

                // Copy the array to unmanaged memory.
                Marshal.Copy( PktData, 0,
                              bufPtr,
                              PktData.Length );

                // If the application callback returned a non-zero value, then it
                // doesn't care to receive any more packets...that includes the
                // command-end packet.
                Int32 status = Params.callback(DataLength, bufPtr, Params.context);

                if (status != 0)
                {
                    if (IntPtr.Zero != Params.callbackCode)
                    {
                        Params.callbackCode = new IntPtr(status);
                    }
                }

                Marshal.FreeHGlobal(bufPtr);
            }

        }




        protected ENMU_PKT_FLOW  RxStep    = ENMU_PKT_FLOW.STEP_HEADER_1;
        protected ENMU_PKT_TYPE  RxPktTpye = ENMU_PKT_TYPE.NO;
        protected ENUM_CMD       RxCmd     = ENUM_CMD.NOTHING;

        protected byte[] RxByte            = new byte[1];
        //1 report may devide into manny packets. 
        protected UInt16 usDataTotalLenDw  = 0;  //How manny data in one Report.(DWORD)
        protected UInt16 usDataSingleLenDw = 0;  //How manny data in one packet.(DWORD)
        protected UInt16 usPktSingleLenBt  = 0;  //one packet size.(byte)
        protected UInt16 usRxCRC           = 0;
        protected byte   usDivideNum       = 0;  //How manny packets in one report.
        protected byte   usDivideSeq       = 0;  //the packets sequence.
        protected int    RxCnt             = 0;

        protected ENUM_CMD Regular
        (
            [In, Out] ref byte[] RxBuf,
            [In]          bool   bCheckTimeOut
        )
        {
            ENUM_CMD         result = ENUM_CMD.NOTHING;
            TRANS_DEV_TYPE   enumDev = clsPacket.TRANS_API_AskDevType();


            RxCmd = ENUM_CMD.NOTHING;

            //Check time out
            if (bCheckTimeOut == true)
            {               
                TimeSpan ts = new TimeSpan(DateTime.Now.Ticks - m_timeBefReg.Ticks);
                if (ts.TotalMilliseconds > 1500)
                {
    #if _TRACE_OUT_PUT
                    PrintMagToTxt("Regular: TimeOut");
    #endif    

                    RxStep = ENMU_PKT_FLOW.STEP_HEADER_1;
                    return ENUM_CMD.TIME_OUT;
                }                
            }

            
#if _TRACE_OUT_PUT
            PrintMagToTxt("Regular");
#endif    
            UInt32 RevCount = clsPacket.TRANS_API_AskRevCount();


            //USB receives one report at one time. Don't need to check every byte.
            if (enumDev == TRANS_DEV_TYPE.USB)
            {
                if ( 0 == clsPacket.TRANS_API_Read(RxBuf, RevCount) )
                    return result;


                //USB receives one report at one time. Don't need to find Pkt ID and Header.                
                RxStep = ENMU_PKT_FLOW.STEP_HEADER_1;

                if (PutPktToData(enumDev, RxBuf) == ENMU_REV_STATE.REVEICE_DONE)
                {
                    result = RxCmd;                     
                }

                return result;
            }


            for (int i = 0; i < RevCount; i++)
            {
                switch (RxStep)
                {
                    case ENMU_PKT_FLOW.STEP_HEADER_1:
                        RxCnt             = 0;
                        usDataSingleLenDw = 0;
                        usPktSingleLenBt  = 0;
                        usDivideNum       = 0;
                        usDivideSeq       = 0;
                        usRxCRC           = 0;
                        RxCmd             = ENUM_CMD.NOTHING;
                        result            = ENUM_CMD.NOTHING;
                        Array.Clear(RxBuf, 0, RxBuf.Length);


                        //RS232 need to check every byte.
                        clsPacket.TRANS_API_Read(RxByte, 1);
                        RxBuf[RxCnt] = RxByte[0];


                        //Check Header
                        switch (RxByte[0])
                        {
                            case (byte)'B': //Begin
                            case (byte)'E': //End
                            case (byte)'I': //Inventory
                            case (byte)'A': //Tag Access
                            case (byte)'R': //Common Response
                            case (byte)'S': //Update Common Response
                            case (byte)'F': //Update Enter Firmware configuration
                                RxCnt++;
                                RxStep++;
                                break;

                            default:
                                break;
                        }
                        break;


                    case ENMU_PKT_FLOW.STEP_HEADER_2:  //Header
                        //RS232 need to check every byte.
                        clsPacket.TRANS_API_Read(RxByte, 1);
                        RxBuf[RxCnt] = RxByte[0];


                        if (RxByte[0] == 'I')
                        {
                            RxCnt++;
                            RxStep++;
                        }
                        else
                        {
                            RxStep = ENMU_PKT_FLOW.STEP_HEADER_1;
                        }

                        break;


                    case ENMU_PKT_FLOW.STEP_HEADER_3:  //Header
                        //RS232 need to check every byte.
                        clsPacket.TRANS_API_Read(RxByte, 1);
                        RxBuf[RxCnt] = RxByte[0];
                        

                        if (RxByte[0] == 'T')
                        {
                            RxCnt++;
                            RxStep++;
                        }
                        else
                        {
                            RxStep = ENMU_PKT_FLOW.STEP_HEADER_1;
                        }

                        break;

                    case ENMU_PKT_FLOW.STEP_HEADER_4:  //Header
                        //RS232 need to check every byte.
                        clsPacket.TRANS_API_Read(RxByte, 1);
                        RxBuf[RxCnt] = RxByte[0];


                        if (RxByte[0] == 'M')
                        {
                            RxCnt++;
                            RxStep++;
                        }
                        else
                        {
                            RxStep = ENMU_PKT_FLOW.STEP_HEADER_1;
                        }

                        break;



                    case ENMU_PKT_FLOW.STEP_HANDLE:
                        if (PutPktToData(enumDev, RxBuf) == ENMU_REV_STATE.REVEICE_DONE)
                        {
                            result = RxCmd;
                            return result;
                        }
                        break;

                    default:
                        RxStep = ENMU_PKT_FLOW.STEP_HEADER_1;
                        break;

                }//end switch

            }//end for

            return result;

        }


        protected ENMU_REV_STATE PutPktToData
        (
            [In] TRANS_DEV_TYPE enumDevice,
            [In] byte[]         RxBuf
        )
        {
            int Length = 0;
            int CrcLen = 0;
            UInt16 usCRC = 0;
            
#if _TRACE_OUT_PUT
            PrintMagToTxt("PutPktToData");
#endif    

            try
            {
                //Get Per Packet size and remaining bytes
                switch (RxBuf[0])
                {
                    //Tag Access=====================
                    case (byte)'B': //Begin
                    case (byte)'E': //End
                        RxPktTpye = ENMU_PKT_TYPE.TAG_ACCESS;
                        usPktSingleLenBt = 24;
                        Length = usPktSingleLenBt - (int)ENMU_TAG_PKT_INDEX.PKT_DEVIDE_NUM;
                        break;


                    case (byte)'I': //Inventory
                    case (byte)'A': //Tag Access
                        RxPktTpye = ENMU_PKT_TYPE.TAG_ACCESS;
                        usPktSingleLenBt = 64;
                        Length = usPktSingleLenBt - (int)ENMU_TAG_PKT_INDEX.PKT_DEVIDE_NUM;
                        break;


                    //Common==========================
                    case (byte)'R': //Common
                        RxPktTpye = ENMU_PKT_TYPE.COMMON;
                        usPktSingleLenBt = 16;
                        Length = (int)ENMU_PKT_INDEX.END - (int)ENMU_PKT_INDEX.PKT_DEVIDE_ID;
                        break;


                    //Update==========================
                    case (byte)'F': //Update firmware mode
                        RxPktTpye = ENMU_PKT_TYPE.UPDATE_ENTER;
                        usPktSingleLenBt = 24;
                        Length = usPktSingleLenBt - (int)ENMU_TAG_PKT_INDEX.PKT_DEVIDE_NUM;
                        break;


                    case (byte)'S': //Update Common
                        RxPktTpye = ENMU_PKT_TYPE.UPDATE_COMMON;
                        usPktSingleLenBt = 64;
                        Length = usPktSingleLenBt - (int)ENMU_UPDATE_INDEX.PKT_DEVIDE_ID;
                        break;


                    default:
                        RxStep = ENMU_PKT_FLOW.STEP_HEADER_1;
                        return ENMU_REV_STATE.RECEIVE_FAULT;
                        break;
                }


                //Receive data
                if (enumDevice == TRANS_DEV_TYPE.SERIAL)
                {

                    //Data is not ready. Break and wait for next loop to get.
                    if (clsPacket.TRANS_API_AskRevCount() < Length)
                    {
                        System.Threading.Thread.Sleep(5);
                        return ENMU_REV_STATE.WAIT_DATA;
                    }

                    byte[] tmp = new byte[Length];
                    Array.Clear(tmp, 0, tmp.Length);

                    //Receive data fail.
                    if (Length != clsPacket.TRANS_API_Read(tmp, (uint)Length))
                    {
                        RxStep = ENMU_PKT_FLOW.STEP_HEADER_1;
                        return ENMU_REV_STATE.RECEIVE_FAULT;
                    }


                    Array.Copy(tmp, 0,
                                RxBuf, RxCnt,
                                tmp.Length);
                }


                RxCnt += Length;
                RxStep = ENMU_PKT_FLOW.STEP_HEADER_1;


                //Handle Data
                switch (RxPktTpye)
                {
                    case ENMU_PKT_TYPE.COMMON:
                        {
                            RxCmd   = (ENUM_CMD)RxBuf[(int)ENMU_PKT_INDEX.PKT_COMMAND_ID];
                            usRxCRC = BitConverter.ToUInt16(RxBuf, (int)ENMU_PKT_INDEX.PKT_CRC);
                            CrcLen  = (int)(ENMU_PKT_INDEX.PKT_CRC - ENMU_PKT_INDEX.PKT_HEADER);
                            break;
                        }

                    case ENMU_PKT_TYPE.TAG_ACCESS:
                        {
                            usDivideNum = RxBuf[(int)ENMU_TAG_PKT_INDEX.PKT_DEVIDE_NUM];
                            usDivideSeq = RxBuf[(int)ENMU_TAG_PKT_INDEX.PKT_DEVIDE_SEQ];
                            RxCmd = (ENUM_CMD)(BitConverter.ToInt16
                                                          (RxBuf,
                                                            (int)ENMU_TAG_PKT_INDEX.INFO_TYPE) | 0xF0);
                            usDataSingleLenDw = BitConverter.ToUInt16
                                                            (RxBuf,
                                                              (int)ENMU_TAG_PKT_INDEX.INFO_LENGTH);

                            usRxCRC = BitConverter.ToUInt16(RxBuf, usPktSingleLenBt - 2);
                            CrcLen  = (ushort)(usPktSingleLenBt - 2);
                            break;
                        }


                    case ENMU_PKT_TYPE.UPDATE_ENTER:
                        {
                            usDivideNum = RxBuf[(int)ENMU_TAG_PKT_INDEX.PKT_DEVIDE_NUM];
                            usDivideSeq = RxBuf[(int)ENMU_TAG_PKT_INDEX.PKT_DEVIDE_SEQ];

                            //It uses TAG_ACCESS Packet, but reture 0x300B. Redefine to ENUM_CMD.
                            RxCmd   = ENUM_CMD.UPDAUE_ENTER_UPDATE_MODE;

                            usDataSingleLenDw = BitConverter.ToUInt16
                                                            (RxBuf,
                                                              (int)ENMU_TAG_PKT_INDEX.INFO_LENGTH);

                            usRxCRC = BitConverter.ToUInt16(RxBuf, usPktSingleLenBt - 2);
                            CrcLen  = (ushort)(usPktSingleLenBt - 2);

                            break;
                        }


                    case ENMU_PKT_TYPE.UPDATE_COMMON:
                        {
                            RxCmd   = (ENUM_CMD)(RxBuf[(int)ENMU_UPDATE_INDEX.PKT_DATA_RECMD] | 0xE0);
                            usRxCRC = BitConverter.ToUInt16(RxBuf, usPktSingleLenBt - 2);
                            CrcLen  = (ushort)(usPktSingleLenBt - 2);

                            break;
                        }

                    default:
                        break;
                }


                //Check CRC. CRC Legnth doesn't include CRC(2 bytes)
                usCRC = CrcCul(RxBuf, 0, (ushort)(CrcLen * 8)); //8 bits
                usCRC = (UInt16)(~usCRC & 0xFFFF);

            }
            catch (Exception e)
            {
#if _TRACE_OUT_PUT
                this.PrintMagToTxt("Exception Error : PutPktToData");
#endif    
                clsPacket.TRANS_API_ClearBuffer();
                return ENMU_REV_STATE.RECEIVE_FAULT;
            }

            if (usCRC == usRxCRC)
            {
                return ENMU_REV_STATE.REVEICE_DONE;
            }
            else
            {
                return ENMU_REV_STATE.RECEIVE_FAULT;
            }
        }




//=============================================Other====================================

        protected string ShowOemData	
        (
            UInt16 Offset
        )
        {
            UInt32 OemData = 0;
            UInt32 [] pOemData  = null;
            UInt32 uiLength = 0;
            rfid.Constants.Result result = rfid.Constants.Result.OK;

            if( rfid.Constants.Result.OK != API_MacReadOemData( Offset, ref OemData) )
                return null;

            uiLength =  (0 == (OemData & 0xFF) / 4) ? 1 : (OemData & 0xFF) / 4;
            uiLength += (uint)((0 == (OemData & 0xFF) % 4) ? 0 : 1);

            pOemData = new UInt32[uiLength];
            Array.Clear(pOemData, 0, pOemData.Length);

            pOemData[0] = OemData;
            //Offset++; //Point to  data address
            for (int i = 1; i < uiLength; i++)
            {
                result = API_MacReadOemData( (UInt16)(Offset + i), ref pOemData[i] );

                if( rfid.Constants.Result.OK != result )
                    return null;
            }

            return CGlobalFunc.uint32ArrayToString(pOemData, uiLength);
        }

    }  // Linkage class END


} // rfidnamespace END
