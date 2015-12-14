using System;
using System.Collections.Generic;
using System.Text;

using rfid.Structures;
using rfid.Constants;
using Global;
using rfid;

namespace UpdateOEMCfgTool
{
      
    public struct Updatedata
    { 
        public uint   uiOffset;
        public uint   uiCRC;
        public byte[] btData; 
        public ushort usSize;       
        public bool   bRealRunFlag;
    }


    public class Interface : rfid.Linkage
    {

        //Define
        const uint   DEFINE_CRC32_POLY = 0x4C11DB7;
              uint[] uiCrc32_table     = new uint[256];


        public Interface()
        {
            //Create CRC32 Table
            calc_crctable( DEFINE_CRC32_POLY, sizeof(uint) );

        }



        public bool API_OpenDevice()
        {
            bool bResult = false;

            bResult = this.LinkDevice();

            //Set retry count
            defTryCount = 1;

            return bResult;
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
            clsPacket.TRANS_API_SetOverlapTime( 200, 200 );

            //Clear inventory's infomation buffer
            clsPacket.TRANS_API_ClearBuffer();

            return true;
        }


//==========================Command===================================
        public Result API_UpdataGetVersion
        (
            ref MacBootLoaderVersion r_Version
        )
        {

#if _TRACE_OUT_PUT
            PrintMagToTxt("API_UpdataGetVersion");
#endif   

            Result   result = Result.OK;
            ENUM_CMD TxCmd  = ENUM_CMD.NOTHING;
            byte[]   buf    = new byte[57];
            Array.Clear(buf, 0, buf.Length);

            TxCmd  = ENUM_CMD.UPDAUE_GET_VERSION;
            buf[0] = 0x0D; //Magic
            buf[1] = 0xF0; //Magic
            buf[2] = (byte)( (int)TxCmd & 0xF );//command
            buf[3] = 0; //command
            buf[4] = 0; //Length
            buf[5] = 0; //Length
            buf[6] = 0; //Reserved
            buf[7] = 0; //Reserved


            if ( false == this.SendData(buf) )
                return Result.FAILURE;

            Array.Clear(buf, 0, buf.Length);

            //Check receive data
            if ( false == this.ReceiveData( TxCmd, ref buf ) )
                return Result.FAILURE;

            //Check result
            result = ConvertResult(buf[4]);
            if (Result.OK != result)
                return result;

            r_Version.major   = buf[11];
            r_Version.minor   = buf[10];
            r_Version.release = buf[09];

            return Result.OK;
        }



        public Result API_UpdataEnterUpdateMode
        (

        )
        {

#if _TRACE_OUT_PUT
            PrintMagToTxt("API_UpdataEnterUpdateMode");
#endif   

            Result      result = Result.OK;
            ENUM_CMD    TxCmd  = ENUM_CMD.NOTHING;
            CommonParms common = new CommonParms();
            byte[]      buf    = new byte[57];
            Array.Clear(buf, 0, buf.Length);

            TxCmd  = ENUM_CMD.UPDAUE_ENTER_UPDATE_MODE;
            buf[0] = 0x0D; //Magic
            buf[1] = 0xF0; //Magic
            buf[2] = (byte)( (int)TxCmd & 0xF );//command
            buf[3] = 0; //command
            buf[4] = 1; //Length
            buf[5] = 0; //Length
            buf[6] = 0; //Reserved
            buf[7] = 0; //Reserved
            buf[8] = 0; //0=>Enter update mode 

            if ( false == this.SendData(buf) )
                return Result.FAILURE;         

            //Run report packet progress
            common.callback     = null;
            common.callbackCode = IntPtr.Zero;
            common.context      = IntPtr.Zero;
            if ( CheckTagAccessPkt(common) == false )
                return Result.FAILURE;



            Array.Clear(buf, 0, buf.Length);

            //Check receive data
            if ( false == this.ReceiveData( TxCmd, ref buf ) )
                return Result.FAILURE;

            //Check result
            result = ConvertResult(buf[4]);
            if (Result.OK != result)
                return result;   


            return Result.OK;
        }



        public Result API_UpdataReset
        (
            
        )
        {

#if _TRACE_OUT_PUT
            PrintMagToTxt("API_UpdataReset");
#endif   

            Result   result = Result.OK;
            ENUM_CMD TxCmd  = ENUM_CMD.NOTHING;
            byte[]   buf    = new byte[57];
            Array.Clear(buf, 0, buf.Length);

            TxCmd  = ENUM_CMD.UPDAUE_RESET;
            buf[0] = 0x0D; //Magic
            buf[1] = 0xF0; //Magic
            buf[2] = (byte)( (int)TxCmd & 0xF );//command
            buf[3] = 0; //command
            buf[4] = 0; //Length
            buf[5] = 0; //Length
            buf[6] = 0; //Reserved
            buf[7] = 0; //Reserved
            Array.Clear(buf, 0, buf.Length);

            //Check receive data


            if ( false == this.SendData(buf) )
                return Result.FAILURE;

            if ( false == this.ReceiveData( TxCmd, ref buf ) )
                return Result.FAILURE;

            //Check result
            result = ConvertResult(buf[4]);
            if (Result.OK != result)
                return result;

            return Result.OK;
        }



        public Result API_UpdataComplete
        (
            
        )
        {

#if _TRACE_OUT_PUT
            PrintMagToTxt("API_UpdataReset");
#endif   

            Result   result = Result.OK;
            ENUM_CMD TxCmd  = ENUM_CMD.NOTHING;
            byte[]   buf    = new byte[57];
            Array.Clear(buf, 0, buf.Length);

            TxCmd  = ENUM_CMD.UPDAUE_COMPLETE;
            buf[0] = 0x0D; //Magic
            buf[1] = 0xF0; //Magic
            buf[2] = (byte)( (int)TxCmd & 0xF );//command;
            buf[3] = 0; //command
            buf[4] = 0; //Length
            buf[5] = 0; //Length
            buf[6] = 0; //Reserved
            buf[7] = 0; //Reserved


            if ( false == this.SendData(buf) )
                return Result.FAILURE;

            Array.Clear(buf, 0, buf.Length);

            //Check receive data
            if ( false == this.ReceiveData( TxCmd, ref buf ) )
                return Result.FAILURE;

            //Check result
            result = ConvertResult(buf[4]);
            if (Result.OK != result)
                return result;

            return Result.OK;
        }



        private ushort UpdatedataCRC
        (
            byte[] r_buf,
            ushort r_size
        )        
        {                
            byte[] CrcBuf = new Byte[64];
            Array.Clear( CrcBuf, 0, CrcBuf.Length );

            //Magic
            CrcBuf[0] = 0x0D;
            CrcBuf[1] = 0xF0;

            //Command ~ data-addr
            Array.Copy( r_buf,    0,
                        CrcBuf,   2,
                        10           );

            //data-Flag ~ data-data
            Array.Copy( r_buf,    14,
                        CrcBuf,   12,
                        (r_size+1)*4  );

            long uiCRC = 0;
            //uiCRC = (ushort)API_OtherCrc32( (ulong)uiCRC, CrcBuf, (ulong)((12+r_size*4) * 8) );
            
            return (ushort)uiCRC;
        }



        public Result API_UpdataSendUpdateData
        (
            Updatedata r_strcData
        )
        {

#if _TRACE_OUT_PUT
            PrintMagToTxt("API_UpdataSendUpdateData");
#endif   

            Result   result = Result.OK;
            ENUM_CMD TxCmd  = ENUM_CMD.NOTHING;
            byte[]   buf    = new byte[57];
            Array.Clear(buf, 0, buf.Length);

            TxCmd  = ENUM_CMD.UPDAUE_SEND_UPDATE_DATA;    
            buf[0] = 0x0D; //Magic
            buf[1] = 0xF0; //Magic     
            buf[2] = (byte)( (int)TxCmd & 0xF );//command;
            buf[3] = 0; //command
            buf[4] = (byte)(  r_strcData.usSize        & 0xFF); //Length
            buf[5] = (byte)( (r_strcData.usSize >> 8 ) & 0xFF); //Length
            buf[6] = 0; //Reserved
            buf[7] = 0; //Reserved

            //Data========
            //Addr
            buf[8]  = (byte)(  r_strcData.uiOffset        & 0xFF ); 
            buf[9]  = (byte)( (r_strcData.uiOffset >>  8) & 0xFF );
            buf[10] = (byte)( (r_strcData.uiOffset >> 16) & 0xFF ); 
            buf[11] = (byte)( (r_strcData.uiOffset >> 24) & 0xFF );

            //Flag 
            buf[16] = (byte)( r_strcData.bRealRunFlag == true ? 0 : 1 ); //Flag = 1 => Test mode

            //Data
            Array.Copy( r_strcData.btData,  0,
                        buf,                20,
                        r_strcData.btData.Length );

            //CRC
            uint uiCrc = 0;
            
            
            uiCrc = API_OtherCrc32( API_OtherCrc32( uiCrc, buf, 0, 12 ), buf, 16, (uint)(r_strcData.usSize-2)*4 );
            buf[12] = (byte)(  uiCrc        & 0xFF ); 
            buf[13] = (byte)( (uiCrc >>  8) & 0xFF );
            buf[14] = (byte)( (uiCrc >> 16) & 0xFF );
            buf[15] = (byte)( (uiCrc >> 24) & 0xFF );

            if ( false == this.SendData(buf) )
                return Result.FAILURE;

            Array.Clear(buf, 0, buf.Length);

            //Check receive data
            if ( false == this.ReceiveData( TxCmd, ref buf ) )
                return Result.FAILURE;

            //Check result
            result = ConvertResult(buf[4]);
            if (Result.OK != result)
                return result;

            return Result.OK;
        }
//=============================================================
        protected bool SendData
        (
            byte[] btData
        )
        {
            ushort usCRC    = 0;
            byte[] wtBuf    = new byte[(int)(ENMU_UPDATE_INDEX.END)];
            ushort usCrcLen = (ushort)(ENMU_UPDATE_INDEX.PKT_CRC - ENMU_UPDATE_INDEX.PKT_HEADER);

            Array.Clear(wtBuf, 0, wtBuf.Length);
            wtBuf[0] = 0x55;
            wtBuf[1] = 0x49;
            wtBuf[2] = 0x54;
            wtBuf[3] = 0x4D;
            wtBuf[4] = 0xFF;  

            Array.Copy( btData, 0,
                        wtBuf, (int)ENMU_UPDATE_INDEX.PKT_MAGIC,
                        btData.Length);
 
            usCRC = CrcCul( wtBuf,
                            0,
                            (ushort)(usCrcLen * 8) );

            wtBuf[(int)(ENMU_UPDATE_INDEX.PKT_CRC)]   = (byte)(~usCRC      & 0xFF);
            wtBuf[(int)(ENMU_UPDATE_INDEX.PKT_CRC+1)] = (byte)(~usCRC >> 8 & 0xFF);

            //return clsPacket.TRANS_API_Write(wtBuf, (UInt32)wtBuf.Length);

           if (clsPacket.TRANS_API_Write(wtBuf, (UInt32)wtBuf.Length) != true)
            {
                //Try to connect the device again
                clsPacket.TRANS_API_Close();

                //wait for device
                TimeBeginPeriod(1);
                System.Threading.Thread.Sleep( 100 );
                TimeEndPeriod(1);

                //Link the device again
                if ( this.LinkDevice() == false )
                    return false;                

                //Write again
                return clsPacket.TRANS_API_Write(wtBuf, (UInt32)wtBuf.Length);

            }
            else
            {                
                m_btTryLinkCount = 0;
                return true;
            }


        }


        protected bool ReceiveData
        (
                ENUM_CMD r_TxCmd,
            ref byte[]   r_btData
        )
        {

            ENUM_CMD result = ENUM_CMD.NOTHING;
            byte []  rdBuf  = new byte[128];
            Array.Clear(rdBuf, 0, rdBuf.Length);

            //Reset Receive flow
            ResetRxFlow(ENMU_PKT_TYPE.UPDATE_COMMON);

            while (result != r_TxCmd)
            {
                result = this.Regular(  ref rdBuf, true);

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
                    this.LinkDevice();

                    return false;
                }

                TimeBeginPeriod(1);
                System.Threading.Thread.Sleep( 5 );
                TimeEndPeriod(1);
            }


            //Data ~ Data over  //only copy Data
            int Length = ENMU_UPDATE_INDEX.PKT_CRC - ENMU_UPDATE_INDEX.PKT_DATA;


            Array.Copy( rdBuf, (int)ENMU_UPDATE_INDEX.PKT_DATA,
                        r_btData, 0,
                        Length);

            return true;
        }



        public ushort API_OtherCrc16
        (
            byte[] buf,
            uint   uiOffset,
            ushort bit_length
        )
        {
            return CrcCul(buf, uiOffset, bit_length);
        }


        private uint API_OtherCrc32
        (
            uint   r_uiSum,
            byte[] r_buf,
            uint   r_uiOffset,
            uint   r_uiLen
        )
        {   
            int i = 0;

            while( (r_uiLen--) > 0)
            {
                r_uiSum = uiCrc32_table[ (uint)(r_uiSum>>24) ^ r_buf[r_uiOffset+i] ] ^ (uint)( r_uiSum << 8);
                i++;
            }

            return r_uiSum;
        }

             
        private void calc_crctable(uint r_poly, int r_size)
        {         
            uint  uiMask  = (uint)( 1 << (r_size * 8 - 1));
            int   iLeft   = (r_size - 1) * 8;

            Array.Clear(uiCrc32_table, 0, uiCrc32_table.Length);

            for (uint  ulIndex = 0; ulIndex < 256; ++ulIndex)
            { 
                uint r = ulIndex << iLeft;

                for( uint i = 0; i < 8; ++i)
                {
                    
                    if ( (uint)(r & uiMask) > 0 )      
                        r = (r<<1) ^ r_poly;
                    else
                        r <<= 1;

                    uiCrc32_table[ulIndex] = r;
                
                }
            
            }
        
        }


    }
}
