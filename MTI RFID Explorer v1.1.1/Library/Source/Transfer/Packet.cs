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


        private static StringBuilder m_str    = new StringBuilder(256);
        private static Mutex         m_Mutex  = new Mutex();
        private static TRANS_RESULT  m_Result = TRANS_RESULT.FAILURE;

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


    }  // Functions class END

} // rfid namespace END


