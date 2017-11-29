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
 * $Id: clsPacket.cs,v 1.18 2010/11/09 23:05:38 dshaheen Exp $
 * 
 * Description: 
 *
 *****************************************************************************
 */

using System;
using System.IO;
using System.Reflection;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Collections.Specialized;

using System.Threading;
using System.Text;

using rfid.Constants;
using rfid.Structures;
using Global;
using FTD2XX_NET; //Add by Wayne for implement high baud rate function, 2015-06-05

namespace rfid
{

    // Declare, instantiate and utilize for callbacks
    // originating from inventory, read, write and
    // similar operations...

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate Int32 CallbackDelegate
    (
        [In]      UInt32 bufferLength,
        [In]      IntPtr pBuffer,
        [In, Out] IntPtr context
    );


    [StructLayout(LayoutKind.Sequential)]
    public struct DCB
    {
        public uint DCBLength;
        public uint BaudRate;
        public BitVector32 Flags;
        private short wReserved;
        public short XonLim;
        public short XoffLim;
        public byte ByteSize;
        public byte Parity;
        public byte StopBits;
        public byte XonChar;
        public byte XoffChar;
        public byte ErrorChar;
        public byte EofChar;
        public byte EvtChar;
        public short wReserved1;

        private static readonly int fBinary;
        private static readonly int fParity;
        private static readonly int fOutxCtsFlow;
        private static readonly int fOutxDsrFlow;
        private static readonly BitVector32.Section fDtrControl;
        private static readonly int fDsrSensitivity;
        private static readonly int fTXContinueOnXoff;
        private static readonly int fOutX;
        private static readonly int fInX;
        private static readonly int fErrorChar;
        private static readonly int fNull;
        private static readonly BitVector32.Section fRtsControl;
        private static readonly int fAbortOnError;
    }



    public class clsPacket
    {
        //Add by Wayne for implement high baud rate function, 2015-06-05
        static IntPtr ftHandle = IntPtr.Zero;
        static bool Use_FTDI_Dll = false;
		//End by Wayne for implement high baud rate function, 2015-06-05

        [DllImport("Transfer.dll", EntryPoint = "API_USB_Open",       CallingConvention = CallingConvention.StdCall)]
        private static extern TRANS_RESULT dllUsbOpen( uint uiPID, uint uiVID );
       
        [DllImport("Transfer.dll", EntryPoint = "API_Serial_Open", CallingConvention = CallingConvention.StdCall)]
        private static extern TRANS_RESULT dllSerialOpen(uint uiComPort, ref DCB r_dcb);
        
        [DllImport("Transfer.dll", EntryPoint = "API_Write",          CallingConvention = CallingConvention.StdCall)]
        private static extern TRANS_RESULT dllWrite(uint uiMode, byte[] cData, uint iLength);
        
        [DllImport("Transfer.dll", EntryPoint = "API_Read",           CallingConvention = CallingConvention.StdCall)]
        private static extern TRANS_RESULT dllRead(uint uiMode, byte[] cData, ref uint iLength);
        
        [DllImport("Transfer.dll", EntryPoint = "API_Close",          CallingConvention = CallingConvention.StdCall)]
        private static extern TRANS_RESULT dllClose();
        
        [DllImport("Transfer.dll", EntryPoint = "API_SetOverlapTime", CallingConvention = CallingConvention.StdCall)]
        private static extern TRANS_RESULT dllSetOverlapTime(UInt32 uiWtOverlapTime, UInt32 r_uiRdOverlapTime);
        
        [DllImport("Transfer.dll", EntryPoint = "API_ClearBuffer", CallingConvention = CallingConvention.StdCall)]
        private static extern TRANS_RESULT dllClearBuffer();

        [DllImport("Transfer.dll", EntryPoint = "API_AskRevCount", CallingConvention = CallingConvention.StdCall)]
        private static extern TRANS_RESULT dllAskRevCount(ref uint r_uiRxCount);

        [DllImport("Transfer.dll", EntryPoint = "API_AskDevType",  CallingConvention = CallingConvention.StdCall)]
        private static extern TRANS_RESULT dllAskDevType(ref uint r_uiTxCount);

        [DllImport("Transfer.dll", EntryPoint = "API_AskVersion", CallingConvention = CallingConvention.StdCall)]
        private static extern IntPtr dllAskVersion();        
        
        [DllImport("Transfer.dll", EntryPoint = "API_AskDevPath", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        private static extern IntPtr dllAskDevPath();
        //Add by Wayne for implement high baud rate function, 2015-06-05
        [DllImportAttribute("ftd2xx.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern FTDI.FT_STATUS FT_Open(int iDevices, ref IntPtr ftHandle);

        [DllImportAttribute("ftd2xx.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern FTDI.FT_STATUS FT_OpenEx(string pvArg1, int dwFlags, ref IntPtr ftHandle);

        [DllImportAttribute("ftd2xx.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern FTDI.FT_STATUS FT_Close(IntPtr ftHandle);

        [DllImportAttribute("ftd2xx.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern FTDI.FT_STATUS FT_SetBaudRate(IntPtr ftHandle, int dwBaudRate);

        [DllImportAttribute("ftd2xx.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern FTDI.FT_STATUS FT_SetDataCharacteristics(IntPtr ftHandle, byte uWordLength, byte uStopBits, byte uParity);

        [DllImportAttribute("ftd2xx.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern FTDI.FT_STATUS FT_SetFlowControl(IntPtr ftHandle, ushort usFlowControl, byte uXon, byte uXoff);

        [DllImportAttribute("ftd2xx.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern FTDI.FT_STATUS FT_SetTimeouts(IntPtr ftHandle, int dwReadTimeout, int dwWriteTimeout);

        [DllImportAttribute("ftd2xx.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern FTDI.FT_STATUS FT_GetQueueStatus(IntPtr ftHandle, ref int lpdwAmountInRxQueue);

        [DllImportAttribute("ftd2xx.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern FTDI.FT_STATUS FT_Purge(IntPtr ftHandle, byte uEventCh);

        [DllImportAttribute("ftd2xx.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern FTDI.FT_STATUS FT_Read(IntPtr ftHandle, byte[] lpBuffer, int dwBytesToRead, ref int lpdwBytesReturned);

        [DllImportAttribute("ftd2xx.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern FTDI.FT_STATUS FT_Write(IntPtr ftHandle, byte[] lpBuffer, int dwBytesToWrite, ref int lpdwBytesWritten);

        [DllImportAttribute("ftd2xx.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern FTDI.FT_STATUS FT_SetDeadmanTimeout(IntPtr ftHandle, int dwDeadmanTimeout);
        
		//Not Used        
		//[DllImportAttribute("ftd2xx.dll", CallingConvention = CallingConvention.Cdecl)]
        //static extern FTDI.FT_STATUS FT_SetDtr(IntPtr ftHandle);

        //[DllImportAttribute("ftd2xx.dll", CallingConvention = CallingConvention.Cdecl)]
        //static extern FTDI.FT_STATUS FT_ClrDtr(IntPtr ftHandle);

        //[DllImportAttribute("ftd2xx.dll", CallingConvention = CallingConvention.Cdecl)]
        //static extern FTDI.FT_STATUS FT_SetRts(IntPtr ftHandle);

        //[DllImportAttribute("ftd2xx.dll", CallingConvention = CallingConvention.Cdecl)]
        //static extern FTDI.FT_STATUS FT_ClrRts(IntPtr ftHandle);

        //[DllImportAttribute("ftd2xx.dll", CallingConvention = CallingConvention.Cdecl)]
        //static extern FTDI.FT_STATUS FT_GetModemStatus(IntPtr ftHandle, ref byte ModemStatus);
        //End by Wayne for implement high baud rate function, 2015-06-05


        private static StringBuilder m_str    = new StringBuilder(256);
        private static Mutex         m_Mutex  = new Mutex();
        private static TRANS_RESULT  m_Result = TRANS_RESULT.FAILURE;
        private static FTDI.FT_STATUS ftStatus = FTDI.FT_STATUS.FT_OK; //Add by Wayne for implement high baud rate function, 2015-06-05

        public static TRANS_RESULT TRANS_API_USB_Open(uint uiPID, uint uiVID)
        {
            m_Mutex.WaitOne();

            try
            {
                m_Result = dllUsbOpen(uiPID, uiVID);
                
                m_Mutex.ReleaseMutex();

                return m_Result;
            }
            catch (Exception e)
            { 
                dllClose();
                m_Mutex.ReleaseMutex();

                return TRANS_RESULT.FAILURE;
            }

            
        }

        public static TRANS_RESULT TRANS_API_Serial_Open(uint uiComPort, ref DCB r_pDcb)
        {
            m_Mutex.WaitOne(); 
            m_Result = dllSerialOpen(uiComPort, ref r_pDcb);
            m_Mutex.ReleaseMutex();

            return m_Result;
        }

        public static bool TRANS_API_Write(byte[] cData, uint iLength)
        {
            m_Mutex.WaitOne();
            m_Result = dllWrite((uint)TRANS_MODE.INTERRUPT_WITH_OVERLAP, cData, iLength);
            m_Mutex.ReleaseMutex();
            
            if ( m_Result == TRANS_RESULT.OK)
                return true;
            else
                return false;
        }

        static uint tmpLen = 0;
        public static uint TRANS_API_Read(byte[] cData, uint iLength)
        {
            m_Mutex.WaitOne();
            tmpLen   = iLength;
            m_Result = dllRead((uint)TRANS_MODE.INTERRUPT_WITH_OVERLAP, cData, ref tmpLen);
            m_Mutex.ReleaseMutex();

            if ( m_Result != TRANS_RESULT.OK )
                return 0;

            return tmpLen;
        }

        public static void TRANS_API_Close()
        {
            m_Mutex.WaitOne();
            m_Result = dllClose();
            m_Mutex.ReleaseMutex();
        }



        public static TRANS_RESULT TRANS_API_ClearBuffer()
        {
            m_Mutex.WaitOne();
            m_Result =  dllClearBuffer();
            m_Mutex.ReleaseMutex();

            return m_Result;
        }


        public static TRANS_RESULT TRANS_API_SetOverlapTime(UInt32 r_uiWtOverlapTime, UInt32 r_uiRdOverlapTime)
        {
            m_Mutex.WaitOne();
            m_Result = dllSetOverlapTime(r_uiWtOverlapTime, r_uiRdOverlapTime);
            m_Mutex.ReleaseMutex();

            return m_Result;
        }
 


        public static uint TRANS_API_AskRevCount()
        {
            uint iCount = 0;

            m_Mutex.WaitOne();
            dllAskRevCount(ref iCount);
            m_Mutex.ReleaseMutex();

            return iCount;
        }



       public static TRANS_DEV_TYPE TRANS_API_AskDevType()
        {
            TRANS_DEV_TYPE enumDev = TRANS_DEV_TYPE.NO_DEVICE;
            uint    devType = 0;

            //Add by Wayne for implement high baud rate function, 2015-06-05
            if (Use_FTDI_Dll == true)
            {
				//Mod by FJ for Explorer can not execute in high baudrate, 2016-12-23
				enumDev = TRANS_DEV_TYPE.SERIAL;
                //enumDev = TRANS_DEV_TYPE.USB;
				//End by FJ for Explorer can not execute in high baudrate, 2016-12-23
                return enumDev;
            }
            //End by Wayne for implement high baud rate function, 2015-06-05

            m_Mutex.WaitOne();           
            dllAskDevType(ref devType);
            m_Mutex.ReleaseMutex();

            switch (devType)
            {                      
                case 1:
                    enumDev = TRANS_DEV_TYPE.SERIAL;
                    break;

                case 2:
                    enumDev = TRANS_DEV_TYPE.USB;
                    break;

                default:
                    enumDev = TRANS_DEV_TYPE.NO_DEVICE;
                    break;
            }

            return enumDev;
        }


        //clark 2011.05.17  transfer dll version
        public static string TRANS_API_AskVersion( )
        {
            string strVer = null;

            m_Mutex.WaitOne();

            do
            {
                if( IntPtr.Zero ==  dllAskVersion() )
                    break;
                else
                    strVer = Marshal.PtrToStringAnsi( dllAskVersion() );
            
            }while(false);
            
            m_Mutex.ReleaseMutex();

            return strVer;
        }



 
        public static string TRANS_API_AskDevPath()
        {
            string strName = null;

            m_Mutex.WaitOne();

            do
            {
                if( IntPtr.Zero ==  dllAskDevPath() )
                    break;
                else
                    strName = Marshal.PtrToStringAnsi( dllAskDevPath() );
            
            }while(false);
            
            m_Mutex.ReleaseMutex();

            return strName;
        }
       //Add by Wayne for implement high baud rate function, 2015-06-05
        public static FTDI.FT_STATUS FTDI_API_Serial_Open(uint Baudrate, ref IntPtr ftHandle)
        {
            m_Mutex.WaitOne();
            ftStatus = FT_Open(0, ref ftHandle);
            if (ftStatus == FTDI.FT_STATUS.FT_OK)
                if ((ftStatus = FT_SetBaudRate(ftHandle, (int)Baudrate)) == FTDI.FT_STATUS.FT_OK)
                    if ((ftStatus = FT_SetDataCharacteristics(ftHandle, 8, 0, 0)) == FTDI.FT_STATUS.FT_OK)
                    {
                        ftStatus = FT_SetFlowControl(ftHandle, 0x0000, 0x11, 0x13);
                        if (ftStatus == FTDI.FT_STATUS.FT_OK)
                        {
                            ftStatus = FT_SetTimeouts(ftHandle, 500, 500);
                        }
                    }
            m_Mutex.ReleaseMutex();

            return ftStatus;
        }
        public static bool FTDI_API_Write(byte[] cData, uint iLength)
        {
            int WrittenBytes = 0;
            bool result;

            m_Mutex.WaitOne();
            //write data to buffer
            if (FT_Write(ftHandle, cData, (int)iLength, ref WrittenBytes) != FTDI.FT_STATUS.FT_OK)
            {
                result = false;
            }
            else
                result = true;
            m_Mutex.ReleaseMutex();

            return result;
        }


        public static int FTDI_API_Read(byte[] RxBuf, uint ReadByte)
        {
            int actualReadByte = 0;
            
            m_Mutex.WaitOne();
            if (FT_Read(ftHandle, RxBuf, (int)ReadByte, ref actualReadByte) != FTDI.FT_STATUS.FT_OK)
            {
                actualReadByte = 0;
            }
            m_Mutex.ReleaseMutex();

            return actualReadByte;
        }

        public static int FTDI_API_AskRevCount()
        {
            int ReceivedCount = 0;

            m_Mutex.WaitOne();
            FT_GetQueueStatus(ftHandle, ref ReceivedCount);
            m_Mutex.ReleaseMutex();

            return ReceivedCount;
        }

        public static void FTDI_API_Close()
        {
            FTDI.FT_STATUS result;

            m_Mutex.WaitOne();
            result = FT_Close(ftHandle);
            m_Mutex.ReleaseMutex();

            return;
        }

        public static void FTDI_API_ClearBuffer()
        {
            m_Mutex.WaitOne();
            FT_Purge(ftHandle, (byte)ENUM_PURGE_HW_BUF.FT_PURGE_RX);
            m_Mutex.ReleaseMutex();

            return;
        }

        public static bool FTDI_API_SetOverlapTime(int ReadTimeout, int WriteTimeout)
        {
            bool result = true;

            m_Mutex.WaitOne();
            if (FT_SetTimeouts(ftHandle, ReadTimeout, WriteTimeout) != FTDI.FT_STATUS.FT_OK)
                result = false;
            m_Mutex.ReleaseMutex();

            return result;
        }

        public static bool Mid_API_Serial_Open(uint ComPort)
        {
            //Mod by FJ for fix linking to incorrect Dll API issue, 2016-12-23
            bool result = false;
            //bool result = true;
            rfid.Linkage lk = new rfid.Linkage();

            uint[] DetectBaudRate = new uint[] { (uint)ENUM_BAUD_RATE.CBR_115200, (uint)ENUM_BAUD_RATE.CBR_230400, (uint)ENUM_BAUD_RATE.CBR_460800, (uint)ENUM_BAUD_RATE.CBR_921600 };
            uint errcount = 0;

            for (int i = 0; i < DetectBaudRate.Length; i++)
            {
                if (FTDI_API_Serial_Open(DetectBaudRate[i], ref ftHandle) == FTDI.FT_STATUS.FT_OK)
                {
                    Use_FTDI_Dll = true; 
                    if (lk.API_MacgGetDebug(ref errcount) != Result.OK)
                    {
                        result = false;
                        continue;
                    }
                    else
                    {
                        result = true;
                        return result;
                    }
                }
                else
                {
                    result = false;
                }
            }

            if (result == false)
            {
                Use_FTDI_Dll = false;
                rfid.DCB pDcb = new rfid.DCB();
                pDcb.ByteSize = 8;
                pDcb.Parity = 0;
                pDcb.StopBits = (byte)ENUM_STOP_BIT.ONESTOPBIT;
                pDcb.BaudRate = (uint)ENUM_BAUD_RATE.CBR_115200;
                if (clsPacket.TRANS_API_Serial_Open(ComPort, ref pDcb) != TRANS_RESULT.OK)
                {
                    result = false;
                }
                else
                {
                    result = true;
                }
            }

            ////Open COM port by WDK DLL
            ////Try to open real COM port by baudrate 115200
            //Use_FTDI_Dll = false;
            //rfid.DCB pDcb = new rfid.DCB();
            //pDcb.ByteSize = 8;
            //pDcb.Parity = 0;
            //pDcb.StopBits = (byte)ENUM_STOP_BIT.ONESTOPBIT;
            //pDcb.BaudRate = (uint)ENUM_BAUD_RATE.CBR_115200;
            //if (clsPacket.TRANS_API_Serial_Open(ComPort, ref pDcb) != TRANS_RESULT.OK)
            //{
            //    result = false;
            //}

            ////Open COM port by FTDI DLL
            //if (result == false) //Try to open virtual COM port by high speed baudrate
            //{
            //    uint[] DetectBaudRate = new uint[] { (uint)ENUM_BAUD_RATE.CBR_115200, (uint)ENUM_BAUD_RATE.CBR_230400, (uint)ENUM_BAUD_RATE.CBR_460800, (uint)ENUM_BAUD_RATE.CBR_921600 };
            //    uint errcount = 0;
            //    //Use_FTDI_Dll = true; //Del by FJ for the FWUpdate failure in USB connection, 2016-09-08


            //    for (int i = 0; i < DetectBaudRate.Length; i++)
            //    {
            //        if (FTDI_API_Serial_Open(DetectBaudRate[i], ref ftHandle) == FTDI.FT_STATUS.FT_OK)
            //        {
            //            if (lk.API_MacgGetDebug(ref errcount) != Result.OK)
            //            {
            //                result = false;
            //                continue;
            //            }
            //            else
            //            {
            //                Use_FTDI_Dll = true; //Add by FJ for the FWUpdate failure in USB connection, 2016-09-08
            //                result = true;
            //                break;
            //            }
            //        }
            //        else
            //        {
            //            result = false;
            //        }
            //    }
            //}
            //End by FJ for fix linking to incorrect Dll API issue, 2016-12-23

            return result;
        }

        public static bool Mid_API_Write(byte[] cData, uint iLength)
        {
            bool result = true;

            if (Use_FTDI_Dll == false)
            {
                if (TRANS_API_Write(cData, iLength) == true)
                    result = true;
                else
                    result = false;
            }
            else
            {
                if (FTDI_API_Write(cData, iLength) == true)
                    result = true;
                else
                    result = false;
            }

            return result;
        }

        public static uint Mid_API_Read(byte[] RxBuf, uint ReadByte)
        {
            uint ActualReadByte = 0;

            if (Use_FTDI_Dll == false)
            {
                ActualReadByte = TRANS_API_Read(RxBuf, ReadByte);
            }
            else
            {
                ActualReadByte = (uint)FTDI_API_Read(RxBuf, ReadByte);
            }

            return ActualReadByte;
        }

        public static uint Mid_API_AskRevCount()
        {
            uint ReceivedByte = 0;

            if (Use_FTDI_Dll == false)
            {
                ReceivedByte = TRANS_API_AskRevCount();
            }
            else
            {
                ReceivedByte = (uint)FTDI_API_AskRevCount();
            }

            return ReceivedByte;
        }

        public static void Mid_API_Close()
        {
            if (Use_FTDI_Dll == false)
            {
                TRANS_API_Close();
            }
            else
            {
                FTDI_API_Close();
            }

            return;
        }

        public static void Mid_API_ClearBuffer()
        {
            if (Use_FTDI_Dll == false)
            {
                TRANS_API_ClearBuffer();
            }
            else
            {
                FTDI_API_ClearBuffer();
            }

            return;
        }

        public static bool Mid_API_SetOverlapTime(UInt32 r_uiWtOverlapTime, UInt32 r_uiRdOverlapTime)
        {
            bool result = true;

            if (Use_FTDI_Dll == false)
            {
                if (TRANS_API_SetOverlapTime(r_uiWtOverlapTime, r_uiRdOverlapTime) != TRANS_RESULT.OK)
                    result = false;
            }
            else
            {
                if (FTDI_API_SetOverlapTime(Convert.ToInt32(r_uiRdOverlapTime), Convert.ToInt32(r_uiWtOverlapTime)) != true)
                    result = false;
            }
            return result;
        }
        //End by Wayne for implement high baud rate function, 2015-06-05

    }  // Functions class END

} // rfid namespace END

