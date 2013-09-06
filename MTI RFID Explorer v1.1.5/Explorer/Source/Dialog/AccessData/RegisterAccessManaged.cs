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
 * $Id: RegisterAccessManaged.cs,v 1.9 2010/01/07 02:10:57 dshaheen Exp $
 * 
 * Description: 
 *
 *****************************************************************************
 */

using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Threading;

using rfid.Constants;
using rfid.Structures;
using RFID.RFIDInterface;

namespace RFID_Explorer
{
    public partial class FORM_RegisterAccess : Form
    {

        private bool GetMacError(out uint errorCode, out uint lastErrorCode)
        {
            rfid.Constants.Result result = Result.NOT_INITIALIZED;

            errorCode = uint.MaxValue;
            lastErrorCode = uint.MaxValue;

            /* MacGetError */
            try
            {
                result = Reader.MacGetError(out errorCode, out lastErrorCode);
            }
            catch (Exception)
            {
                StatusUpdate("GetMacError Exception", TextUpdateType.Error);
                return false;
            }

            if (result != rfid.Constants.Result.OK)
            {
                return false;
            }

            return true;
        }

        private bool ClearMacError()
        {
            rfid.Constants.Result result = Result.NOT_INITIALIZED;

            /* MacClearError */
            try
            {
                result = Reader.MacClearError();
            }
            catch (Exception)
            {
                StatusUpdate("MacClearError Exception", TextUpdateType.Error);
                return false;
            }

            if (result != rfid.Constants.Result.OK)
            {
                return false;
            }

            return true;
        }

        /*
         *****************************************************************************
         *
         * Register Access
         * 
         *****************************************************************************
         */

        private bool RegisterAccessRead(RegisterAccessType type, ushort address, ushort bank, uint count, bool print)
        {
            ushort registerAddress = 0;
            ushort endAddress      = 0;
            uint   data32          = 0;
            ushort data16          = 0;

            //clark 2011.8.17. If use count 0, show error message.
            if(count == 0)
                StatusUpdate(print, string.Format("Read Fail [{0:X4}]", address), TextUpdateType.Error);

            if (ushort.MaxValue >= (uint)(address + count - 1))
            {
                //If address is 0xFFFF and count is 1. endAddress = 0xFFFF + 1. => Over flow.
                //Let explorer read 0xFFFF, The condition of For Loop is "equal", not "greater".
                endAddress = (ushort)(address + count - 1);
            }
            else
            {
                endAddress = ushort.MaxValue;
            }

            //Let explorer read 0xFFFF, The condition of For Loop is "equal", not "greater".
            switch (type)
            {                 
                case RegisterAccessType.Mac:
                    for (registerAddress = address; registerAddress <= endAddress; registerAddress++)
                    {
                        if (!ReadMacRegsiter(registerAddress, out data32, print))
                        {
                            return false;
                        }
                    }
                    break;

                case RegisterAccessType.MacBank:
                    for (registerAddress = address; registerAddress <= endAddress; registerAddress++)
                    {
                        RegisterInfo registerInfo;

                        if (!GetMacRegsiterInfo(registerAddress, out registerInfo))
                        {
                            return false;
                        }

                        if (registerInfo.type == RegisterType.BANKED || (count == 1))
                        {
                            if (bank == ushort.MaxValue)
                            {
                                if (!ReadMacRegsiter(registerAddress, out data32, print))
                                {
                                    return false;
                                }

                                for (ushort index = 0; index < registerInfo.bankSize; index++)
                                {
                                    if (!ReadMacRegsiterBank(registerAddress, index, out data32, print))
                                    {
                                        return false;
                                    }
                                }
                            }
                            else
                            {
                                if (!ReadMacRegsiterBank(registerAddress, (ushort)bank, out data32, print))
                                {
                                    return false;
                                }
                            }
                        }
                        else
                        {
                            if (!ReadMacRegsiter(registerAddress, out data32, print))
                            {
                                return false;
                            }
                        }
                    }
                    break;

                case RegisterAccessType.Bypass:
                    for (registerAddress = address; registerAddress <= endAddress; registerAddress++)
                    {
                        if (!ReadBypassRegsiter(registerAddress, out data16, print))
                        {
                            return false;
                        }
                    }
                    break;

                case RegisterAccessType.OEM:
                    for (registerAddress = address; registerAddress <= endAddress; registerAddress++)
                    {
                        if (!ReadOemRegsiter(registerAddress, out data32, print))
                        {
                            return false;
                        }
                    }
                    break;

                default:
                    System.Diagnostics.Debug.Assert(false, "Register Access Type");
                    break;
            }

            return true;
        }

        private bool RegisterAccessWrite(RegisterAccessType type, ushort address, ushort bank, uint data, bool print)
        {
            switch (type)
            {
                case RegisterAccessType.Mac:
                    if (!WriteMacRegsiter(address, (uint)data, print))
                    {
                        return false;
                    }
                    break;

                case RegisterAccessType.MacBank:
                    if (!WriteMacRegsiterBank(address, bank, (uint)data, print))
                    {
                        return false;
                    }
                    break;

                case RegisterAccessType.Bypass:
                    if (!WriteBypassRegsiter(address, (ushort)data, print))
                    {
                        return false;
                    }
                    break;

                case RegisterAccessType.OEM:
                    if (!WriteOemRegsiter(address, (uint)data, print))
                    {
                        return false;
                    }

                    break;

                default:
                    System.Diagnostics.Debug.Assert(false, "Register Access Type");
                    break;
            }

            return true;
        }

        private bool ReadMacRegsiter(ushort address, out uint data)
        {
            rfid.Constants.Result result = Result.NOT_INITIALIZED;

            data = uint.MaxValue;

            /* Mac Register Read */
            try
            {
                result = Reader.MacReadRegister(address, out data);
            }
            catch (Exception)
            {
                StatusUpdate("Read Mac Register Exception", TextUpdateType.Error);
                return false;
            }

            if (result != rfid.Constants.Result.OK)
            {
                return false;
            }

            return true;
        }

        private bool ReadMacRegsiter(ushort address, out uint data, bool print)
        {
            rfid.Constants.Result result = Result.NOT_INITIALIZED;
            RegisterInfo registerInfo;
            string infoString = "Unknown";

            data = uint.MaxValue;

            if ( !GetMacRegsiterInfo(address, out registerInfo ) )
            {
                return false;
            }

            string accessTypeString;

            switch (registerInfo.accessType)
            {
                case RegisterProtectionType.READ_WRITE:
                    accessTypeString = "";
                    break;
                case RegisterProtectionType.READ_ONLY:
                    accessTypeString = " RO";
                    break;
                case RegisterProtectionType.WRITE_ONLY:
                    accessTypeString = " WO";
                    break;
                default:
                    accessTypeString = " NA";
                    break;
            }

            switch (registerInfo.type)
            {
                case RegisterType.NORMAL:
                    infoString = string.Format("Normal{0}", accessTypeString);
                    break;
                case RegisterType.BANKED:
                    infoString = string.Format("Banked{0} Sel:{1:X4}", accessTypeString, registerInfo.selectorAddress);
                    break;
                case RegisterType.SELECTOR:
                    infoString = string.Format("Selector{0} Size:{1}", accessTypeString, registerInfo.bankSize);
                    break;
                case RegisterType.RESERVED:
                    infoString = string.Format("Reserved");
                    break;
            }

            /* Mac Register Read */
            try
            {
                result = Reader.MacReadRegister(address, out data);
                if (result != Result.OK)
                {
                    data = 0;
                }
            }
            catch (Exception)
            {
                StatusUpdate("Read Mac Register Exception", TextUpdateType.Error);
                return false;
            }

            if (result == rfid.Constants.Result.OK)
            {
                if (registerInfo.type == RegisterType.BANKED)
                {
                    StatusUpdate(print, string.Format("Read Mac [{0:X4}][{1:X2}] = {2:X8} ({3})", address, registerInfo.currentSelector, data, infoString), TextUpdateType.Success);
                }
                else
                {
                    StatusUpdate(print, string.Format("Read Mac [{0:X4}] = {1:X8} ({2})", address, data, infoString), TextUpdateType.Success);
                }
            }
            else if (registerInfo.accessType == RegisterProtectionType.WRITE_ONLY)
            {
                StatusUpdate(print, string.Format("Read Mac [{0:X4}] ({1})", address, infoString), TextUpdateType.Success);
            }
            else if (registerInfo.type == RegisterType.RESERVED)
            {
                StatusUpdate(print, string.Format("Read Mac [{0:X4}] ({1})", address, infoString), TextUpdateType.Success);
            }
            else
            {
                StatusUpdate(print, string.Format("Read Mac Fail [{0}] [{1:X4}] ", result, address), TextUpdateType.Error);
                return false;
            }

            return true;
        }

        private bool ReadMacRegsiterBank(ushort address, ushort bank, out uint data, bool print)
        {
            rfid.Constants.Result result = Result.NOT_INITIALIZED;
            RegisterInfo registerInfo;

            data = uint.MaxValue;

            if ( !GetMacRegsiterInfo(address, out registerInfo ) )
            {
                return false;
            }

            if (registerInfo.type != RegisterType.BANKED)
            {
                StatusUpdate("Read Mac Register Bank (Not Banked)", TextUpdateType.Error);
                return false;
            }

            /* Mac Bank Register Read */
            try
            {
                result = Reader.MacReadBankedRegister(address, bank, out data);
            }
            catch (Exception)
            {
                StatusUpdate("Read Mac Register Bank Exception", TextUpdateType.Error);
                return false;
            }

            if (result == rfid.Constants.Result.OK)
            {
                StatusUpdate(print, string.Format("Read Mac Register Bank [{0:X4}][{1:X2}] = {2:X8}", address, bank, data), TextUpdateType.Success);
            }
            else
            {
                StatusUpdate(print, string.Format("Read Mac Register Bank Fail [{0}] [{1:X4}] ", result, address), TextUpdateType.Error);
                return false;
            }

            /* Restore Original Selector */
            if (!WriteMacRegsiter(registerInfo.selectorAddress, registerInfo.currentSelector, false))
            {
                StatusUpdate("Read Mac Register Bank (Write Original Selector)", TextUpdateType.Error);
                return false;
            }

            return true;
        }


        private bool GetMacRegsiterInfo(ushort address, out RegisterInfo registerInfo)
        {
            rfid.Constants.Result result = Result.NOT_INITIALIZED; 
            registerInfo = new RegisterInfo();

            /* Mac Register Info Read */
            try
            {
                result = Reader.MacReadRegisterInfo(address, out registerInfo);
            }
            catch (Exception)
            {
                StatusUpdate("Get Mac Register Info Exception", TextUpdateType.Error);
                return false;
            }

            return true;
        }


        private bool WriteMacRegsiter(ushort address, uint data, bool print)
        {
            rfid.Constants.Result result = Result.NOT_INITIALIZED;
            uint errorCode;
            uint lastErrorCode;

            if (!ClearMacError())
            {
                return false;
            }

            /* Mac Register Write */
            try
            {
                result = Reader.MacWriteRegister(address, data);
            }
            catch (Exception)
            {
                StatusUpdate("Write Mac Register Exception", TextUpdateType.Error);
                return false;
            }

            if (!GetMacError(out errorCode, out lastErrorCode))
            {
                return false;
            }

            if ( (result == rfid.Constants.Result.OK) && (errorCode == 0) )
            {
                StatusUpdate(print, string.Format("Write Mac [{0:X4}] = {1:X8}", address, data), TextUpdateType.Success);
            }
            else
            {
                StatusUpdate(print, string.Format("Write Mac Fail (MacErr:{0:X4}) [{1:X4}] = {2:X8}", errorCode, address, data), TextUpdateType.Error);
                if (!ClearMacError())
                {
                    return false;
                }
                return false;
            }

            return true;
        }



        private bool WriteMacRegsiterBank(ushort address, ushort bank, uint data, bool print)
        {
            rfid.Constants.Result result = Result.NOT_INITIALIZED;
            uint errorCode;
            uint lastErrorCode;

            if( !ClearMacError() )
            {
                return false;
            }

            /* Mac Register Bank Write */
            try
            {
                result = Reader.MacWriteBankedRegister(address, bank, data);
            }
            catch (Exception)
            {
                StatusUpdate("Write Mac Register Bank Exception", TextUpdateType.Error);
                return false;
            }

            if (!GetMacError(out errorCode, out lastErrorCode))
            {
                return false;
            }

            if ((result == rfid.Constants.Result.OK) && (errorCode == 0))
            {
                StatusUpdate(print, string.Format("Write Mac Register Bank [{0:X4}][{1:X2}] = {2:X8}", address, bank, data), TextUpdateType.Success);
            }
            else if (errorCode != 0)
            {
                StatusUpdate(print, string.Format("Write Mac Register Bank Fail (MacErr:{0:X4}) [{1:X4}][{2:X2}] = {3:X8}", errorCode, address, bank, data), TextUpdateType.Error);
                return false;
            }
            else
            {
                StatusUpdate(print, string.Format("Write Mac Register Bank Fail [{0}]", result), TextUpdateType.Error);
                return false;
            }

            return true;
        }

        private bool ReadBypassRegsiter(ushort address, out ushort data, bool print)
        {
            rfid.Constants.Result result = Result.NOT_INITIALIZED;

            data = ushort.MaxValue;

            /* Bypass Register Read */
            try
            {
                result = Reader.MacBypassReadRegister(address, out data);
            }
            catch (Exception)
            {
                StatusUpdate("Read Bypass Register Exception", TextUpdateType.Error);
                return false;
            }

            if (result == rfid.Constants.Result.OK)
            {
                StatusUpdate(print, string.Format("Read Bypass [{0:X4}] = {1:X4}", address, data), TextUpdateType.Success);
            }
            else
            {
                StatusUpdate(print, string.Format("Read Bypass Fail [{0}] [{1:X4}] ", result, address), TextUpdateType.Error);
                return false;
            }

            return true;
        }

        private bool WriteBypassRegsiter(ushort address, ushort data, bool print)
        {
            rfid.Constants.Result result = Result.NOT_INITIALIZED;

            /* Indy Register Write */
            try
            {
                result = Reader.MacBypassWriteRegister(address, data);
            }
            catch (Exception)
            {
                StatusUpdate("Write Bypass Register Exception", TextUpdateType.Error);
                return false;
            }

            if (result == rfid.Constants.Result.OK)
            {
                StatusUpdate(print, string.Format("Write Bypass [{0:X4}] = {1:X4}", address, data), TextUpdateType.Success);
            }
            else
            {
                StatusUpdate(print, string.Format("Write Bypass Fail [{0}]", result), TextUpdateType.Error);
                return false;
            }

            return true;
        }

        private bool ReadOemRegsiter(ushort address, out uint data, bool print)
        {
            rfid.Constants.Result result = Result.NOT_INITIALIZED;
 
            uint OemData = 0;

            data = uint.MaxValue;

            /* OEM Register Read */
            try
            {
                result = Reader.MacReadOemData(address, ref OemData);
            }
            catch (Exception)
            {
                StatusUpdate("Read OEM Register Exception", TextUpdateType.Error);
                return false;
            }

            if (result == rfid.Constants.Result.OK)
            {
                data = OemData;
                StatusUpdate(print, string.Format("Read OEM [{0:X4}] = {1:X8}", address, data), TextUpdateType.Success);
            }
            else
            {
                StatusUpdate(print, string.Format("Read OEM Fail [{0}] [{1:X4}]", result, address), TextUpdateType.Error);
                return false;
            }

            return true;
        }


        private bool ReadOemRegsiter(ushort address, out uint[] pData, uint count, out uint readCount, bool print)
        {
            rfid.Constants.Result result = Result.NOT_INITIALIZED;

            pData = new uint[count];
            uint requestedCount = count;
            readCount = 0;

            Array.Clear(pData, 0, pData.Length);

            /* OEM Register Read */
            try
            {
                //clark 2011.3.23. Only Transfer one data at one tme.
                for (int i = 0; i < count; i++)
                {
                    result = Reader.MacReadOemData(address, ref  pData[i]);

                    if (result != Result.OK)
                       break;
                }
            }
            catch (Exception)
            {
                StatusUpdate("Read OEM Register Exception", TextUpdateType.Error);
                return false;
            }

            if ((result == rfid.Constants.Result.INVALID_PARAMETER) && (count > 0))
            {
                result = rfid.Constants.Result.OK;
            }

            if (result == rfid.Constants.Result.OK)
            {
                readCount = count;
                StatusUpdate(print, string.Format("Read OEM Register Block [{0:X4}] ({1}/{2}) ", address, readCount, requestedCount), TextUpdateType.Success);
            }
            else
            {
                StatusUpdate(print, string.Format("Read OEM Register Block Fail [{0}] [{1:X4}]", result, address), TextUpdateType.Error);
                return false;
            }

            return true;
        }

        private bool WriteOemRegsiter(ushort address, uint data, bool print)
        {
            rfid.Constants.Result result = Result.NOT_INITIALIZED;

            /* Oem Register Write */
            try
            {
                result = Reader.MacWriteOemData(address, data);
            }
            catch (Exception)
            {
                StatusUpdate("Write OEM Register Exception", TextUpdateType.Error);
                return false;
            }

            if (result == rfid.Constants.Result.OK)
            {
                StatusUpdate(print, string.Format("Write OEM [{0:X4}] = {1:X8}", address, data), TextUpdateType.Success);
            }
            else
            {
                StatusUpdate(print, string.Format("Write OEM Fail [{0}]", result), TextUpdateType.Error);
                return false;
            }

            return true;
        }

        private bool WriteFlushOemRegsiter(ushort address, uint data, bool print)
        {
            uint readBack;

            if (!WriteOemRegsiter(address, data, print))
            {
                return false;
            }

            if (!ReadOemRegsiter(address, out readBack, print))
            {
                return false;
            }

            return true;
        }



        /*
         *****************************************************************************
         *
         * Threads
         * 
         *****************************************************************************
         */

        private void DumpMac_Click()
        {
            Thread dumpMac = new Thread(new ThreadStart(DumpMacThread));
            dumpMac.IsBackground = true;
            dumpMac.Start();
        }

        private void DumpMacThread()
        {
            for (ushort macBaseAddress = 0; macBaseAddress < 0xFFF; macBaseAddress += 0x100)
            {
                RegisterAccessRead(RegisterAccessType.MacBank, macBaseAddress, ushort.MaxValue, 0xFF, true);
            }
            StatusUpdate("Dump MAC Success", TextUpdateType.Success);
        }

    }
}