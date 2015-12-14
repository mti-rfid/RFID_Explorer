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
 * $Id: LakeChabot.cs,v 1.32 2011/01/05 06:22:14 dciampi Exp $
 * 
 * Description: 
 *
 *****************************************************************************
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Xml;

using rfid.Constants;
using rfid.Structures;
using Global;

namespace RFID.RFIDInterface
{
    public enum TagAccessType { Read, Write, Kill, Lock, BlockWrite, BlockErase, LargeRead, QT_Read, QT_Write, QT_None };

    public struct TagAccessReads//Add LargeRead command
    {
        public int ReadWords;
        public string ReadWords_text;
        public int TotalReadWords;
        public string TotalReadWords_text;
    }

    public struct TagAccessData
    {
        public bool initialized;
        public TagAccessType type;
        public MemoryBank bank;
        public ushort offset;
        public string offset_text;
        public ushort value1;
        public string value1_text;
        public ushort value2;
        public string value2_text;
        public byte   count;
        public uint accessPassword;
        public string accessPassword_text;
        public uint killPassword;
        public string killPassword_text;
        public PasswordPermission killPasswordPermissions;
        public PasswordPermission accessPasswordPermissions;
        public MemoryPermission epcMemoryBankPermissions;
        public MemoryPermission tidMemoryBankPermissions;
        public MemoryPermission userMemoryBankPermissions;
        public QTCtrlType qtReadWrite;
        public QTPersistenceType qtPersistence;
        public QTShortRangeType qtShortRange;
        public QTMemMapType qtMemoryMap;
        public TagAccessFlag strcTagFlag; //clark 2011.4.22
    }


    public class LakeChabotReader : IDisposable, IReader
    {
        [DllImport("kernel32.dll")]
        public static extern IntPtr LoadLibrary(String lpFileName);

        private const int MIN_SLEEP_TIME_MS = 20;		// Min time callback thread should sleep
        private const int SLEEP_ADD_SUBTRACT_MS = 10;		// Amount to bump the sleep time when processor bound
        private const int TARGET_MAX_USAGE_PERCENT = 80;		// Target percent processor usage
        private const int INITIAL_QUEUE_SIZE = 100;
        private const int TARGET_QUEUE_SIZE = 256;
        private const int MAX_QUEUE_SIZE = 512;
        private const int QUEUE_SLEEP_MS = 10;
        private const int MIN_REFRESH_MS = 100;
        private const int MAX_REFRESH_MS = 1000;
        internal const int EMA_PERIOD = 8;

        public class PacketArrivalEventArgs : EventArgs
        {
            public PacketArrayList PacketBatch = null;
            public PacketArrivalEventArgs( PacketArrayList packetBatch )
            {
                PacketBatch = packetBatch;
            }
        }

        // public delegate void PacketArrivalHandler(Object source, PacketArrivalEventArgs e);
        // public event PacketArrivalHandler PacketArrival;

        internal static class PacketLogger
        {
            private const string _fileNameRoot = "rfid";
            private const string _fileNameExt = "log";
            private static string _pathName = null;
            private static StreamWriter _fileStream = null;

            private static void OpenFile( )
            {
                if ( _pathName == null || !Directory.Exists( _pathName ) )
                {
                    _fileStream = StreamWriter.Null;
                    return;
                }
                int i = 1;
                string path = Path.Combine( _pathName, String.Format( "{0}{1:d03}.{2}", _fileNameRoot, i, _fileNameExt ) );
                while ( File.Exists( path ) )
                {
                    path = Path.Combine( _pathName, String.Format( "{0}{1:d03}.{2}", _fileNameRoot, ++i, _fileNameExt ) );
                }
                _fileStream = new StreamWriter( File.Open( path, FileMode.Create, FileAccess.Write, FileShare.Read ) );
            }

            public static string PathName
            {
                get { return _pathName; }
                set
                {
                    _pathName = value;
                }
            }

            public static void Comment( string message )
            {
                if ( _fileStream == null )
                    OpenFile( );

                _fileStream.WriteLine( String.Format( "{0} {1}", new String( '*', 23 ), message ) );
            }

            public static void Log( PacketData.PacketWrapper envelope )
            {
                if ( _fileStream == null )
                    OpenFile( );

                if ( envelope.IsPseudoPacket )
                {
                    PacketData.CommandPsuedoPacket psuedo = envelope.Packet as PacketData.CommandPsuedoPacket;
                    if ( psuedo != null )
                    {
                        switch ( psuedo.RequestName )
                        {
                            case "Inventory":

                                ReaderRequest readerRequest = new ReaderRequest( );
                                using ( System.IO.MemoryStream data = new System.IO.MemoryStream( psuedo.DataValues ) )
                                {
                                    readerRequest.ReadFrom( data );
                                }
                                _fileStream.WriteLine( String.Format( "{0:d02}\t{1}\t\"{2}\"\t{3}", envelope.ReaderIndex, envelope.ElapsedTimeMs, "Request", readerRequest.RequestName ) );
                                break;


                            case "BadPacket":
                                BadPacket badPacket = new BadPacket( );
                                using ( System.IO.MemoryStream data = new System.IO.MemoryStream( psuedo.DataValues ) )
                                {
                                    badPacket.ReadFrom( data );
                                }
                                _fileStream.WriteLine( String.Format( "{0:d02}\t{1}\t\"{2}\"\t{3}", envelope.ReaderIndex, envelope.ElapsedTimeMs, "Bad Packet", badPacket.RawPacketData == null ? "No Data" : BitConverter.ToString( badPacket.RawPacketData ) ) );
                                break;

                            default:
                                System.Diagnostics.Debug.Assert( false );
                                break;
                        }
                    }
                }
                else
                {
                    _fileStream.WriteLine( String.Format( "{0:d02}\t{1}\t\"{2}\"\t{3}", envelope.ReaderIndex, envelope.ElapsedTimeMs, envelope.PacketTypeName, BitConverter.ToString( envelope.RawPacket ) ) );
                }
            }


            public static void Flush( )
            {
                if ( _fileStream != null )
                {
                    _fileStream.Flush( );
                }
            }

            public static void Clear( )
            {
                if ( _fileStream != null )
                {
                    _fileStream.Flush( );
                    _fileStream.Close( );
                    _fileStream = null;
                }
            }
        }

        public enum TagMemoryBank
        {
            Unknown = -1,
            Reserved = 0,
            EPC = 1,
            TID = 2,
            User = 3,
        }

        private class FileHandlerClass : IDisposable
        {
            private const int FIXED_HEADER_SIZE = 64;
            private const int FILE_BLOCKING_SIZE = 1024 * 4;
            private const int MAX_MEMORY_STREAMS = 2;
            private const int MAX_MEMORY_PACKETS = 5;

            public byte[ ] sig = { 0x3e, 0x42, 0xba, 0x82, 0x95, 0xa4, 0x48, 0xc5, 0x8d, 0x6f, 0x41, 0xeb, 0xeb, 0x5a, 0x34, 0x38 };
            public byte[ ] Id;
            public DateTime Timestamp = default( DateTime );

            private bool _disposed = false;
            private bool _noTempFile = false;
            private int _packets = 0;
            private int _activeStream = 0;
            private long _lastPosition = 0;

            private byte[ ] _timestamp = null;
            private MemoryStream _headerStream = null;
            private MemoryStream[ ] _memoryStreams = null;
            private FileStream _fileStream = null;
            private IFormatter _formatter = null;

            public bool HasData
            {
                get { return _packets > 0; }
            }

            public int TotalPacketCount
            {
                get { return _packets; }
            }

            public bool NoTempFile
            {
                get { return _noTempFile; }
                set
                {
                    CloseStream( );

                    _noTempFile = value;

                    InitializeStream( );
                }
            }

            public void InitializeStream( )
            {
                if ( NoTempFile )
                {
                    _memoryStreams = new MemoryStream[ ] { new MemoryStream( FILE_BLOCKING_SIZE ), null };
                }
                else
                {
                    //clark not sure    2011.08.31
                    //Use Path.GetTempFileName( ) first time after restart the computer spend more time.
                    //_fileStream = new FileStream( Path.GetTempFileName( ), FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None, FILE_BLOCKING_SIZE, FileOptions.DeleteOnClose );
                    //_fileStream = new FileStream( "tmpFile.tmp", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None, FILE_BLOCKING_SIZE, FileOptions.DeleteOnClose );
                    string tmpPath = Path.Combine(System.Windows.Forms.Application.CommonAppDataPath, "tmpStream");
                    _fileStream = new FileStream( tmpPath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None, FILE_BLOCKING_SIZE, FileOptions.DeleteOnClose );                    
                }
            }

            public void CloseStream( )
            {
                if ( _fileStream != null )
                {
                    _fileStream.Close( );
                    _fileStream = null;
                }

                if ( _memoryStreams != null )
                {
                    foreach ( MemoryStream s in _memoryStreams )
                    {
                        s.Close( );
                    }
                    _memoryStreams = null;
                }
            }

            public MemoryStream ActiveMemoryStream
            {
                get
                {
                    // Caculate current index 
                    int index = ( ( int ) Math.Truncate( ( double ) ( _packets / MAX_MEMORY_PACKETS ) ) ) % MAX_MEMORY_STREAMS;
                    // allocate new stream if index is new
                    if ( index != _activeStream )
                    {
                        _memoryStreams[ index ] = new MemoryStream( FILE_BLOCKING_SIZE );
                        _activeStream = index;
                    }
                    return _memoryStreams[ _activeStream ];
                }
            }

            public Stream HeaderWriteStream
            {
                get { return NoTempFile ? ( Stream ) ( _headerStream = new MemoryStream( FIXED_HEADER_SIZE ) ) : ( Stream ) _fileStream; }
            }

            public Stream HeaderReadStream
            {
                get { return NoTempFile ? ( Stream ) _headerStream : ( Stream ) _fileStream; }
            }

            public Stream WriteableStream
            {
                get { return NoTempFile ? ( Stream ) ActiveMemoryStream : ( Stream ) _fileStream; }
            }

            public Stream ReadableStream
            {
                get { return Stream.Null; }
            }

            public PacketData.PacketWrapper GetPendingPacket( )
            {
                PacketData.PacketWrapper result = null;
                if ( NoTempFile )
                    return result;

                System.Diagnostics.Debug.Assert( _fileStream != null );

                long oldPosition;
                long newPosistion;

                if ( _fileStream != null &&
                     ( oldPosition = _fileStream.Position ) >
                     ( newPosistion = _lastPosition > FIXED_HEADER_SIZE ? _lastPosition : FIXED_HEADER_SIZE ) )
                {
                    _fileStream.Position = newPosistion;
                    result = _formatter.Deserialize( _fileStream ) as PacketData.PacketWrapper;
                    _lastPosition = _fileStream.Position;
                    _fileStream.Position = oldPosition;
                }
                return result;
            }

            public IEnumerable<PacketData.PacketWrapper> PendingPackets
            {
                get
                {
                    if ( NoTempFile )
                    {
                        yield break;
                    }
                    else
                    {
                        System.Diagnostics.Debug.Assert( _fileStream != null );
                        if ( _fileStream != null )
                        {
                            long pos = _fileStream.Position;
                            _fileStream.Position = _lastPosition > FIXED_HEADER_SIZE ? _lastPosition : FIXED_HEADER_SIZE;
                            while ( _fileStream.Position < pos )
                            {
                                yield return _formatter.Deserialize( _fileStream ) as PacketData.PacketWrapper;
                            }
                            _lastPosition = _fileStream.Position;
                            _fileStream.Position = pos;
                        }
                    }
                }
            }

            public FileHandlerClass( bool noTempFile )
            {
                Id = new Guid().ToByteArray();
                Timestamp = DateTime.Now;
                NoTempFile = noTempFile;
                _timestamp = BitConverter.GetBytes(Timestamp.ToBinary());
                _formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                WriteHeader();
            }

            public FileHandlerClass( string FileName )
            {
                NoTempFile = false;

                // SetFileStream(new FileStream(FileName, FileMode.Open, FileAccess.Read, FileShare.Read));

                byte[ ] header = ReaderHeader( );
                for ( int i = 0; i < sig.Length; i++ )
                {
                    if ( header[ i ] != sig[ i ] )
                    {
                        throw new rfidException( rfidErrorCode.InvalidPacketFile, String.Format( "{0} is not a valid data file.", FileName ) );
                    }
                }

                Id = new byte[ sig.Length ];			// _sig and ID are the same size.
                Array.Copy( header, sig.Length, Id, 0, Id.Length );
            }

            private void WriteHeader( )
            {
                byte[ ] header = new byte[ FIXED_HEADER_SIZE ];

                sig.CopyTo( header, 0 );
                Id.CopyTo( header, sig.Length );
                _timestamp.CopyTo( header, sig.Length + Id.Length );

                HeaderWriteStream.Write( header, 0, FIXED_HEADER_SIZE );
            }

            private byte[ ] ReaderHeader( )
            {
                byte[ ] header = new byte[ FIXED_HEADER_SIZE ];
                HeaderReadStream.Read( header, 0, FIXED_HEADER_SIZE );
                return header;
            }

            public void WritePacket( PacketData.PacketWrapper envelope )
            {
                _packets++;
                _formatter.Serialize( WriteableStream, envelope );
            }

            public PacketData.PacketWrapper ReadPacket( Stream packetStream )
            {
                return _formatter.Deserialize( packetStream ) as PacketData.PacketWrapper;
            }

            #region IDisposable Members

            public void Dispose( )
            {
                Dispose( true );
                GC.SuppressFinalize( this );
            }

            #endregion

            private void Dispose( bool disposing )
            {
                if ( !this._disposed )
                {
                    if ( disposing )
                    {
                        CloseStream( );
                    }
                }
                _disposed = true;
            }

            ~FileHandlerClass( )
            {
                Dispose( false );
            }

            public void Close( )
            {
                Dispose( true );
            }
        } // private class FileHandler : IDisposable

        // TODO : sub-class out different components according to functionality
        //        and param requirements e.g. read, write, lock, kill tags...

        private class ReaderInterfaceThreadClass
        {
            public string Result = null;
            private RadioOperationMode _OpMode = RadioOperationMode.NONCONTINUOUS;
            private int _handle;
            private bool _singlePackets;
            private bool _stopRequest;
            public LakeChabotReader _reader;
            public TagAccessFlag _strcTagFlag = new TagAccessFlag();
            public AutoResetEvent StartEvent  = new AutoResetEvent(false);

            private rfid.Linkage _access = null;

            /// <summary>
            /// Constructor for Inventory
            /// </summary>
            /// <param name="ManagedAccess"></param>
            /// <param name="Handle"></param>
            /// <param name="WantSinglePackets"></param>
            /// <param name="runCount"></param>
            public ReaderInterfaceThreadClass(rfid.Linkage ManagedAccess, int Handle, bool WantSinglePackets, RadioOperationMode r_OpMode, TagAccessFlag r_strcTagFlag)
            {
                if (r_OpMode == RadioOperationMode.UNKNOWN)
                {
                    throw new ArgumentOutOfRangeException("OpMode", "OpMode must be CONTINUE or NON_CONTINUE");
                }

                _strcTagFlag = r_strcTagFlag;
                _OpMode = r_OpMode;
                _access = ManagedAccess;
                _handle = Handle;
                _singlePackets = WantSinglePackets;
                _stopRequest = false;
            }

            private TagAccessData _tagAccessData;
            private TagAccessReads _tagAccessReadSet;//??把计??//Add LargeRead command
            /// <summary>
            /// 
            /// </summary>
            /// <param name="ManagedAccess"></param>
            /// <param name="Handle"></param>
            /// <param name="MemoryBank"></param>
            /// <param name="?"></param>
            public ReaderInterfaceThreadClass(rfid.Linkage ManagedAccess, int Handle, TagAccessData tagAccessData, TagAccessReads tagAccessDataRead)//??把计??//Add LargeRead command
            {
                _access = ManagedAccess;
                _handle = Handle;
                _tagAccessData = tagAccessData;
                _tagAccessReadSet = tagAccessDataRead;//把计??
            }

            public bool Stop
            {
                get { return _stopRequest; }
                set { _stopRequest = value; }
            }
            /// <summary>
            /// 
            /// </summary>
            public void InventoryThreadProc()
            {
                StartEvent.WaitOne();

                rfid.Structures.InventoryParms inventoryParms = new rfid.Structures.InventoryParms();

                inventoryParms.common.tagStopCount = 0;
                inventoryParms.common.callback     = new rfid.CallbackDelegate(this._reader.MyCallback);
                inventoryParms.common.context      = IntPtr.Zero;
                inventoryParms.common.callbackCode = IntPtr.Zero;

                //clark 2011.4.25 Set tag access flag to inventory structure
                inventoryParms.common.strcTagFlag.RetryCount        = _strcTagFlag.RetryCount;
                inventoryParms.common.strcTagFlag.PostMatchFlag     = _strcTagFlag.PostMatchFlag;
                inventoryParms.common.strcTagFlag.SelectOpsFlag     = _strcTagFlag.SelectOpsFlag;
                inventoryParms.common.strcTagFlag.bErrorKeepRunning = _strcTagFlag.bErrorKeepRunning;
                inventoryParms.common.OpMode                        = _OpMode;
                
             
                byte flags = 0;

                ///===============================================clark not sure
                //Set "Continue"Mode(Inventory) or "Non-Continue" Mode(Inventory Once)
                Result = LakeChabotReader.MANAGED_ACCESS.API_ConfigSetOperationMode(_OpMode).ToString();
                if (0 != String.Compare("OK", Result)) return;

                do
                {
                    Result = LakeChabotReader.MANAGED_ACCESS.API_l8K6CTagInventory
                        (
                            inventoryParms,
                            flags
                        ).ToString();

                    if(Stop) break;

                }while(_strcTagFlag.bErrorKeepRunning);

            }


            public void PulseThreadProc()
            {
                StartEvent.WaitOne();

                rfid.Structures.CommonParms Params = new rfid.Structures.CommonParms();

                Params.tagStopCount = 0;
                Params.callback     = new rfid.CallbackDelegate(this._reader.MyCallback);
                Params.context      = IntPtr.Zero;
                Params.callbackCode = IntPtr.Zero;

                Result result = LakeChabotReader.MANAGED_ACCESS.API_TestTransmitRandomData(Params, 1, 0 );
            }


            private void BuildParams_ReadCmd(ref ReadCmdParms parameters, ref UInt32 accessPassword)
            {
                parameters.bank   = _tagAccessData.bank;
                parameters.offset = _tagAccessData.offset;
                parameters.count  = _tagAccessData.count;
                accessPassword    = _tagAccessData.accessPassword;
            }

            private void BuildParams_WriteCmd(ref WriteSequentialParms parameters, ref UInt32 accessPassword)
            {
                parameters.bank     = _tagAccessData.bank;
                parameters.offset   = _tagAccessData.offset;
                parameters.pData    = new ushort[2];
                parameters.pData[0] = _tagAccessData.value1;
                parameters.pData[1] = _tagAccessData.value2;
                parameters.count    = _tagAccessData.count;
                accessPassword      = _tagAccessData.accessPassword;
            }

            private void BuildParams_BlockWriteCmd(ref BlockWriteCmdParms parameters, ref UInt32 accessPassword)
            {
                parameters.bank     = _tagAccessData.bank;
                parameters.offset   = _tagAccessData.offset;
                parameters.pData    = new ushort[2];
                parameters.pData[0] = _tagAccessData.value1;
                parameters.pData[1] = _tagAccessData.value2;
                parameters.count    = _tagAccessData.count;
                accessPassword      = _tagAccessData.accessPassword;
            }

            private void BuildParams_BlockEraseCmd(ref BlockEraseCmdParms parameters, ref UInt32 accessPassword)
            {
                parameters.bank   = _tagAccessData.bank;
                parameters.offset = _tagAccessData.offset;
                parameters.count  = _tagAccessData.count;
                accessPassword    = _tagAccessData.accessPassword;
            }

            private void BuildParams_LockCmd(ref LockCmdParms parameters, ref UInt32 accessPassword)
            {
                parameters.permissions.killPasswordPermissions   = _tagAccessData.killPasswordPermissions;
                parameters.permissions.accessPasswordPermissions = _tagAccessData.accessPasswordPermissions;
                parameters.permissions.epcMemoryBankPermissions  = _tagAccessData.epcMemoryBankPermissions;
                parameters.permissions.tidMemoryBankPermissions  = _tagAccessData.tidMemoryBankPermissions;
                parameters.permissions.userMemoryBankPermissions = _tagAccessData.userMemoryBankPermissions; ;
                accessPassword = _tagAccessData.accessPassword;
            }

            private void BuildParams_KillCmd(ref KillCmdParms parameters, ref UInt32 accessPassword)
            {
                parameters.killPassword = _tagAccessData.killPassword;
                accessPassword = _tagAccessData.accessPassword;
            }

            private void BuildParams_QT(ref QTParms parameters, ref UInt32 accessPassword)
            {
                parameters = new QTParms();

                switch (_tagAccessData.type)
                {
                    case TagAccessType.QT_Read:
                        ReadCmdParms readParameters = new ReadCmdParms();
                        BuildParams_ReadCmd(ref readParameters, ref parameters.accessPassword);
                        parameters.accessParms = readParameters;
                        parameters.optCmdType = OptType.OPT_READ;
                        break;
                    case TagAccessType.QT_Write:
                        WriteSequentialParms writeParameters = new WriteSequentialParms();
                        BuildParams_WriteCmd(ref writeParameters, ref parameters.accessPassword);
                        parameters.accessParms = writeParameters;
                        parameters.optCmdType = OptType.OPT_WRITE_TYPE_SEQUENTIAL;
                        break;
                    case TagAccessType.QT_None:
                        parameters.optCmdType = OptType.OPT_NONE;
                        break;
                    default:
                        System.Diagnostics.Debug.Assert(false, "Tag Acces Type"); 
                        break;
                }

                parameters.qtCmdParms.qtReadWrite   = _tagAccessData.qtReadWrite;
                parameters.qtCmdParms.qtPersistence = _tagAccessData.qtPersistence;
                parameters.qtCmdParms.qtShortRange  = _tagAccessData.qtShortRange;
                parameters.qtCmdParms.qtMemoryMap   = _tagAccessData.qtMemoryMap;

                accessPassword = _tagAccessData.accessPassword;
            }


            public void AccessThreadProc()
            {
                rfid.Constants.Result result = rfid.Constants.Result.NOT_INITIALIZED;

                byte flags = 0;

                StartEvent.WaitOne();

                switch (_tagAccessData.type)
                {
                    case TagAccessType.Read:
                        {
                            ReadParms parameters = new ReadParms();
                            BuildParams_ReadCmd(ref parameters.readCmdParms, ref parameters.accessPassword);
                            parameters.common.callback = new rfid.CallbackDelegate(this._reader.MyCallback);

                            //clark 2011.4.25 Set tag access flag to inventory structure
                            parameters.common.strcTagFlag.RetryCount    = _tagAccessData.strcTagFlag.RetryCount;
                            parameters.common.strcTagFlag.PostMatchFlag = _tagAccessData.strcTagFlag.PostMatchFlag;
                            parameters.common.strcTagFlag.SelectOpsFlag = _tagAccessData.strcTagFlag.SelectOpsFlag;
                            parameters.common.OpMode                    = RadioOperationMode.NONCONTINUOUS;
                           
                            result = LakeChabotReader.MANAGED_ACCESS.API_l8K6CTagRead(parameters, flags);
                        }
                        break;
                    //Add LargeRead
                    case TagAccessType.LargeRead:
                        {
                            ReadParms parameters = new ReadParms();
                            BuildParams_ReadCmd(ref parameters.readCmdParms, ref parameters.accessPassword);
                            parameters.common.callback = new rfid.CallbackDelegate(this._reader.MyCallback);

                            //clark 2011.4.25 Set tag access flag to inventory structure
                            parameters.common.strcTagFlag.RetryCount = _tagAccessData.strcTagFlag.RetryCount;
                            parameters.common.strcTagFlag.PostMatchFlag = _tagAccessData.strcTagFlag.PostMatchFlag;
                            parameters.common.strcTagFlag.SelectOpsFlag = _tagAccessData.strcTagFlag.SelectOpsFlag;
                            parameters.common.OpMode = RadioOperationMode.NONCONTINUOUS;
                            parameters.readCmdParms.count = (byte)(_tagAccessReadSet.ReadWords);//籠筁count
                            //parameters.readCmdParms.offset = 0;
                            //□add loop
                            for (int ccnt = 0; ccnt < (_tagAccessReadSet.TotalReadWords /_tagAccessReadSet.ReadWords); ccnt++) // TotalReadWords/ReadWords
                            {
                                UInt16 choff = (UInt16)(_tagAccessReadSet.ReadWords);// readwords
                                result = LakeChabotReader.MANAGED_ACCESS.API_l8K6CTagRead(parameters, flags);
                                parameters.readCmdParms.offset = (UInt16)(parameters.readCmdParms.offset + choff);
                                if (Stop) break;
                            }
                            //◆add loop  
                            //result = LakeChabotReader.MANAGED_ACCESS.API_l8K6CTagRead(parameters, flags);
                        }
                        break;
                    case TagAccessType.Write:
                        {
                            WriteParms parameters = new WriteParms();
                            WriteSequentialParms writeParameters = new WriteSequentialParms();
                            BuildParams_WriteCmd(ref writeParameters, ref parameters.accessPassword);
                            parameters.writeParms = writeParameters;
                            parameters.writeType = WriteType.SEQUENTIAL;
                            parameters.common.callback = new rfid.CallbackDelegate(this._reader.MyCallback);

                            //clark 2011.4.25 Set tag access flag to inventory structure
                            parameters.common.strcTagFlag.RetryCount    = _tagAccessData.strcTagFlag.RetryCount;
                            parameters.common.strcTagFlag.PostMatchFlag = _tagAccessData.strcTagFlag.PostMatchFlag;
                            parameters.common.strcTagFlag.SelectOpsFlag = _tagAccessData.strcTagFlag.SelectOpsFlag;
                            parameters.common.OpMode                    = RadioOperationMode.NONCONTINUOUS;

                            result = LakeChabotReader.MANAGED_ACCESS.API_l8K6CTagWrite(parameters, flags);
                        }
                        break;
                    case TagAccessType.BlockWrite:
                        {
                            BlockWriteParms parameters = new BlockWriteParms();
                            BuildParams_BlockWriteCmd(ref parameters.blockWriteCmdParms, ref parameters.accessPassword);
                            parameters.common.callback = new rfid.CallbackDelegate(this._reader.MyCallback);

                            //clark 2011.4.25 Set tag access flag to inventory structure
                            parameters.common.strcTagFlag.RetryCount    = _tagAccessData.strcTagFlag.RetryCount;
                            parameters.common.strcTagFlag.PostMatchFlag = _tagAccessData.strcTagFlag.PostMatchFlag;
                            parameters.common.strcTagFlag.SelectOpsFlag = _tagAccessData.strcTagFlag.SelectOpsFlag;
                            parameters.common.OpMode                    = RadioOperationMode.NONCONTINUOUS;
                            
                            result = LakeChabotReader.MANAGED_ACCESS.API_l8K6CTagBlockWrite(parameters, flags);
                        }
                        break;
                    case TagAccessType.BlockErase:
                        {
                            BlockEraseParms parameters = new BlockEraseParms();
                            BuildParams_BlockEraseCmd(ref parameters.blockEraseCmdParms, ref parameters.accessPassword);
                            parameters.common.callback = new rfid.CallbackDelegate(this._reader.MyCallback);

                            //clark 2011.4.25 Set tag access flag to inventory structure
                            parameters.common.strcTagFlag.RetryCount    = _tagAccessData.strcTagFlag.RetryCount;
                            parameters.common.strcTagFlag.PostMatchFlag = _tagAccessData.strcTagFlag.PostMatchFlag;
                            parameters.common.strcTagFlag.SelectOpsFlag = _tagAccessData.strcTagFlag.SelectOpsFlag;
                            parameters.common.OpMode                    = RadioOperationMode.NONCONTINUOUS;
                            
                            result = LakeChabotReader.MANAGED_ACCESS.API_l8K6CTagBlockErase(parameters, flags);
                        }
                        break;
                    case TagAccessType.Lock:
                        {
                            LockParms parameters = new LockParms();
                            BuildParams_LockCmd(ref parameters.lockCmdParms, ref parameters.accessPassword);
                            parameters.common.callback = new rfid.CallbackDelegate(this._reader.MyCallback);

                            //clark 2011.4.25 Set tag access flag to inventory structure
                            parameters.common.strcTagFlag.RetryCount    = _tagAccessData.strcTagFlag.RetryCount;
                            parameters.common.strcTagFlag.PostMatchFlag = _tagAccessData.strcTagFlag.PostMatchFlag;
                            parameters.common.strcTagFlag.SelectOpsFlag = _tagAccessData.strcTagFlag.SelectOpsFlag;
                            parameters.common.OpMode                    = RadioOperationMode.NONCONTINUOUS;
                            
                            result = LakeChabotReader.MANAGED_ACCESS.API_l8K6CTagLock(parameters, flags);
                        }
                        break;
                    case TagAccessType.Kill:
                        {
                            KillParms parameters = new KillParms();
                            BuildParams_KillCmd(ref parameters.killCmdParms, ref parameters.accessPassword);
                            parameters.common.callback = new rfid.CallbackDelegate(this._reader.MyCallback);

                            //clark 2011.4.25 Set tag access flag to inventory structure
                            parameters.common.strcTagFlag.RetryCount    = _tagAccessData.strcTagFlag.RetryCount;
                            parameters.common.strcTagFlag.PostMatchFlag = _tagAccessData.strcTagFlag.PostMatchFlag;
                            parameters.common.strcTagFlag.SelectOpsFlag = _tagAccessData.strcTagFlag.SelectOpsFlag;
                            parameters.common.OpMode                    = RadioOperationMode.NONCONTINUOUS;
                           
                            result = LakeChabotReader.MANAGED_ACCESS.API_l8K6CTagKill(parameters, flags);
                        }
                        break;
                    case TagAccessType.QT_None:
                    case TagAccessType.QT_Read:
                    case TagAccessType.QT_Write:
                        {
                            QTParms parameters = new QTParms();
                            BuildParams_QT(ref parameters, ref parameters.accessPassword);
                            parameters.common.callback = new rfid.CallbackDelegate(this._reader.MyCallback);

                            //clark 2011.4.25 Set tag access flag to inventory structure
                            parameters.common.strcTagFlag.RetryCount    = _tagAccessData.strcTagFlag.RetryCount;
                            parameters.common.strcTagFlag.PostMatchFlag = _tagAccessData.strcTagFlag.PostMatchFlag;
                            parameters.common.strcTagFlag.SelectOpsFlag = _tagAccessData.strcTagFlag.SelectOpsFlag;
                            parameters.common.OpMode                    = RadioOperationMode.NONCONTINUOUS;

                            result = LakeChabotReader.MANAGED_ACCESS.API_l8K6CTagQT(parameters, flags);
                        }
                        break;
                    default:
                        System.Diagnostics.Debug.Assert(false, "Tag Acces Type");
                        break;
                }
            }
        }

        public enum EmulationMode
        {
            None = 0,
            MAC = 1,
            Library = 2
        }

        public enum CarrierWaveValue
        {
            Unknown = -1,
            Off = 0,
            On = 1,
        }

        public enum SelectedStateValue
        {
            Unknown = -1,
            All = 0,
            On = 2,
            Off = 3,
        }

        private static int _packetSleepMS = MIN_SLEEP_TIME_MS;
        private static bool _noTempFileAccess = false;
        private static bool _enableLogging = false;

        private rfidReader.OperationMode _myMode;		// Initialized in instance constructor 
        private rfidReaderID _theReaderID = null;
        private rfid.Linkage _managedAccess = null;
        private Queue<PacketData.PacketWrapper> PacketQueue = null;
        public Queue<PacketData.PacketWrapper> VirtualReaderQueue = null;

        private long _sessionStartMS = 0;
        private DateTime _sessionStart = DateTime.MinValue;
        private ManualResetEvent QueueEvent = new ManualResetEvent( false );

        private PacketArrayListGlue _recentPacketList = null;
        private TagCycleMatrix _inventoryMatrix = null;

        private DataFile<PropertyBag> _propertyBagData = null;
        private DataFile<TagInventory> _tagInventoryData = null;

        private SequentialDataFile<ReaderRequest> _readerRequestData = null;
        private SequentialDataFile<PacketStream> _packetStreamData = null;
        private SequentialDataFile<ReaderCommand> _readerCommandData = null;
        private SequentialDataFile<ReaderAntennaCycle> _readerAntennaCycleData = null;
        private SequentialDataFile<AntennaPacket> _antennaCycleData = null;
        private SequentialDataFile<InventoryCycle> _inventoryCycleData = null;
        private SequentialDataFile<InventoryRound> _inventoryRoundData = null;
        private SequentialDataFile<TagRead> _tagReadData = null;
        private SequentialDataFile<ReadRate> _readRateData = null;
        private SequentialDataFile<BadPacket> _badPacketData = null;

        private FunctionControl _control = null;
        private BackgroundWorker _bgdWorker = null;
        private FileHandlerClass _fileHandler = null;

        private rfidTagList _sessionTagList = null;
        private rfidTagList _requestTagList = null;
        private rfidTagList _periodTagList = null;
        private rfidTagList _commandTagList = null;
        private rfidTagList _antennaCycleTagList = null;
        private rfidTagList _antennaTagList = null;
        private rfidTagList _inventoryCycleTagList = null;
        private rfidTagList _inventoryRoundTagList = null;

        private string _staticReaderDir = null;

        // common counts
        private volatile int _iTagAccessReqCount = 0;
        private volatile int _iTagAccessReqCountRead = 0;//把计
        private volatile int _queueCount = 0;
        private volatile int _commonAccessCount = 0;
        private volatile int _commonRequestIndex = -1;
        private volatile int _commonBadIndex = -1;
        private volatile int _cRCErrors = 0;
        private volatile int _cmdEndErrors = 0;

        // raw (from the reader) counts
        private volatile int _rawPacketCount = 0;
        private volatile int _rawCommandCount = 0;
        private volatile int _rawAntennaCycleCount = 0;
        private volatile int _rawAntennaCount = 0;
        private volatile int _rawInventoryCycleCount = 0;
        private volatile int _rawRoundCount = 0;
        private volatile int _rawInventoryCount = 0;

        // processed (from disk) count

        private volatile int _processedPacketIndex = -1;
        private volatile int _processedCommandIndex = -1;
        private volatile int _processedAntennaCycleIndex = -1;
        private volatile int _processedAntennaIndex = -1;
        private volatile int _processedInventoryCycleIndex = -1;
        private volatile int _processedRoundIndex = -1;
        private volatile int _processedInventoryIndex = -1;
        private volatile int _processedCmdReadCount = 0;

        private int _isDisposed = 0;
        private int _refreshRateMS = 0;
        private int _maxQueueSize = 0;

        private UInt32? _lastUsedAntenna = null;
        private UInt32? _lastCmdResult = null;
        private TagAccessData  _lastTagAccessDataSet;
        private TagAccessFlag  _strcTagFlag;
        private TagAccessReads _lastTagAccessReadSet;//??把计??//Add LargeRead command

        private long _lastCmdClientTime = 0;
        private uint _lastCmdDeviceTime = 0;

        private bool IsDisposed { get { return Interlocked.Equals( _isDisposed, 1 ); } }
        public bool IsClosed { get { return IsDisposed; } }

        public rfidReader.OperationMode Mode
        {
            get { return _myMode; }
            set { _myMode = value; }
        }

        private FileHandlerClass FileHandler
        {
            get
            {
                if ( _fileHandler == null )
                    _fileHandler = new FileHandlerClass( NoTempFileAccess );
                return _fileHandler;
            }
        }

        public DateTime SessionStartTime
        {
            get { return _sessionStart; }
        }

        public int SessionUniqueTags
        {
            get { return _sessionTagList.Count; }
        }

        public int RequestUniqueTags
        {
            get { return _requestTagList.Count; }
        }

        public int CurrentUniqueTags
        {
            get { return _periodTagList.Count; }
        }

        public int CommandUniqueTags
        {
            get { return _commandTagList.Count; }
        }

        public int AntennaCycleUniqueTags
        {
            get { return _antennaCycleTagList.Count; }
        }

        public int AntennaUniqueTags
        {
            get { return _antennaTagList.Count; }
        }

        public int InventoryCycleUniqueTags
        {
            get { return _inventoryCycleTagList.Count; }
        }

        public int InventoryRoundUniqueTags
        {
            get { return _inventoryRoundTagList.Count; }
        }

        public long ElapsedMilliseconds
        {
            get { return HighResolutionTimer.Milliseconds - _sessionStartMS; }
        }

        public TimeSpan SessionDuration
        {
            get { return new TimeSpan( TimeSpan.TicksPerMillisecond * ElapsedMilliseconds ); }
        }

        public TimeSpan GetSessionRelativeSessionDuration( long elapsedMilliseconds )
        {
            return new TimeSpan( TimeSpan.TicksPerMillisecond * elapsedMilliseconds );
        }

        public DateTime GetSessionRelativeDateTime( long pointInTimeMS )
        {
            return new DateTime( SessionStartTime.Ticks + ( pointInTimeMS * TimeSpan.TicksPerMillisecond ), DateTimeKind.Utc );
        }

        public UInt32 ReaderHandle
        {
            get
            {
                //if ( Mode == rfidReader.OperationMode.BoundToReader )
                //{
                    return _theReaderID.Handle;
                //}
                //return 0; // ???? should be checking mode before this call
                //      should be throwing exception (?)
            }
        }

        public string Name { get { return _theReaderID == null ? null : _theReaderID.Name; } }

        public FunctionControl FunctionController
        { get { return _control; } }

        public TableState TableResult
        {
            get
            {
                if ( Mode == rfidReader.OperationMode.Static )
                    return TableState.Ready;

                if ( _noTempFileAccess )
                    return TableState.NotAvailable;

                if ( _bgdWorker != null && _bgdWorker.IsBusy )
                    return TableState.Building;

                //if (ProcessedPacketCount + ProcessedCommandCount + BadPacketCount < FileHandler.TotalPacketCount)
                if ( ProcessedPacketCount + _commonRequestIndex + BadPacketCount < FileHandler.TotalPacketCount - 1 )
                    return TableState.BuildRequired;

                return TableState.Ready;
            }
        }

        //clark 2011.4.25 Tag Access flag
        public TagAccessData TagAccessDataSet
        {
            get { return _lastTagAccessDataSet; }
            set { _lastTagAccessDataSet = value; }
        }

        public TagAccessFlag strcTagFlag
        {
            get { return _strcTagFlag; }
            set { _strcTagFlag = value; }
        }

        public TagAccessReads TagAccessReadSet//??把计??//Add LargeRead command
        {
            get { return _lastTagAccessReadSet; }
            set { _lastTagAccessReadSet = value; }
        }

        // All statics near constructor will be used ( later ) in a
        // radio discover(er) & communicator class - for now using 
        // setup similar to previous...

        public static rfid.Linkage MANAGED_ACCESS;

        static rfid.Structures.LibraryVersion LIBRARY_VERSION = new LibraryVersion( ); 
        static rfid.Constants.LibraryMode LIBRARY_MODE = LibraryMode.DEFAULT;
        static rfid.Constants.Result LIBRARY_Result = Result.OK;

        static Dictionary<UInt32, rfid.Structures.RadioInformation> LOCATED_READERS;
        static Dictionary<UInt32, rfid.Structures.RadioInformation> OPENED_READERS;

        // TODO : Move to locator & communicator class

        static LakeChabotReader( )
        {
            try
            {
                LakeChabotReader.MANAGED_ACCESS = new rfid.Linkage( );
            }
            catch ( Exception e )
            {
                throw e;  // TODO: more resolution of error e.g. missing dll
            }

            if (rfid.Constants.Result.OK != LakeChabotReader.MANAGED_ACCESS.API_Startup(LakeChabotReader.LIBRARY_VERSION, LakeChabotReader.LIBRARY_MODE))
            {
                throw new rfidException( rfidErrorCode.LibraryFailedToInitialize );
            }

            LakeChabotReader.LIBRARY_Result = rfid.Constants.Result.OK;

            // Track keyed by cookie ~ allow ( later ) to open and close radios
            // dynamically during application execution - currently ALL OPEN

            rfid.Structures.RadioEnumeration radioEnum = new rfid.Structures.RadioEnumeration( );


            //clark 2011.05.20 Show Tracer no matter the device is disconneced.
            //                 Let user to set comport.  
            LakeChabotReader.MANAGED_ACCESS.API_RetrieveAttachedRadiosList(radioEnum, 0);
            //if
            //(
            //    rfid.Constants.Result.OK !=
            //    LakeChabotReader.MANAGED_ACCESS.API_RetrieveAttachedRadiosList(radioEnum, 0)
            //)
            //{
            //    throw new rfidException( rfidErrorCode.LibraryFailedToInitialize );
            //}

            LakeChabotReader.LOCATED_READERS =
                new Dictionary<UInt32, rfid.Structures.RadioInformation>( );

            foreach ( rfid.Structures.RadioInformation radioInfo in radioEnum.radioInfo )
            {
                //clark 2011.3.23 not sure
                uint uiUniqueId = BitConverter.ToUInt32( radioInfo.uniqueId, 0);
                LakeChabotReader.LOCATED_READERS.Add(uiUniqueId, radioInfo);
                //LakeChabotReader.LOCATED_READERS.Add(radioInfo.cookie, radioInfo);
            }

            LakeChabotReader.OPENED_READERS =
                new Dictionary<UInt32, rfid.Structures.RadioInformation>( );

            try
            {
                new PerformanceCounter( "Processor", "% Processor Time", "_Total", "." );
            }
            catch ( Exception ) { }
        }

        // TODO : Move to locator & communicator class

        public static List<rfidReaderID> FindReaders( )
        {
            return EnumerateReaders( new rfidReaderID( rfidReaderID.ReaderType.MTI, ".", rfidReaderID.LocationTypeID.LocalDevice ) );
        }

        // TODO : Move to locator & communicator class

        public static List<rfidReaderID> FindReaders( rfidReaderID whereToLook )
        {
            return EnumerateReaders( whereToLook );
        }

        // TODO : Move to locator & communicator class
        // TODO : Modify to active query for radios each call

        private static List<rfidReaderID> EnumerateReaders( rfidReaderID whereToLook )
        {
            if ( rfid.Constants.Result.OK != LakeChabotReader.LIBRARY_Result )
            {
                throw new rfidException( rfidErrorCode.LibraryFailedToInitialize );
            }

            //if ( Mode == rfidReader.OperationMode.Static )
            //{
            //    throw new rfidException( rfidErrorCode.CannotBindToStaticReader );
            //}

            if ( whereToLook == null )
            {
                throw new ArgumentNullException( "whereToLook" );
            }

            if ( whereToLook.LocationType != rfidReaderID.LocationTypeID.LocalDevice )
            {
                throw new rfidException( rfidErrorCode.LocationTypeNotSupported, String.Format( "reader does not support {0} location type", Enum.GetName( typeof( rfidReaderID.LocationTypeID ), whereToLook.LocationType ) ) );
            }

            List<rfidReaderID> readers = new List<rfidReaderID>( );

            foreach ( KeyValuePair<UInt32, rfid.Structures.RadioInformation> pair in LakeChabotReader.LOCATED_READERS )
            {
                readers.Add
                (
                    new rfidReaderID
                        (
                            rfidReaderID.ReaderType.MTI,
                            0,  // bad handle ~ unknown handle
                            System.Text.Encoding.ASCII.GetString( pair.Value.uniqueId, 0, pair.Value.uniqueId.Length ),
                            "RFID reader ",
                            "local attached reader",
                            rfidReaderID.LocationTypeID.LocalDevice
                         )
                 );
            }
            return readers;
        }

        /// <summary>
        /// Create an unbound reader
        /// </summary>
        public LakeChabotReader( )
        {
            Mode = rfidReader.OperationMode.Unbound;
            InitReader( );
            PacketQueue = new Queue<PacketData.PacketWrapper>( INITIAL_QUEUE_SIZE );
            this._queueCount = 0;
        }
        
        /// <summary>
        /// Create a bound reader
        /// </summary>
        /// <param name="ReaderToBind"></param>
        public LakeChabotReader( rfidReaderID ReaderToBind )
        {                       
            InitReader();
            
            PacketQueue = new Queue<PacketData.PacketWrapper>(INITIAL_QUEUE_SIZE);

            this._queueCount = 0;

            // Ugly but will switch rfidReaderID to provide cookie n lookup that way
            // when code is completed...

            rfid.Structures.RadioInformation radioInfo = null;

            foreach (KeyValuePair<UInt32, rfid.Structures.RadioInformation> pair in LakeChabotReader.LOCATED_READERS)
            {
                if
                (
                    System.Text.Encoding.ASCII.GetString(pair.Value.uniqueId, 0, pair.Value.uniqueId.Length).Equals
                    (
                        ReaderToBind.Name
                    )
                )
                {
                    radioInfo = pair.Value;
                    break;
                }
            }

           
            Mode = rfidReader.OperationMode.Unbound; // ???? ~ bound set auto ~ but needs unset for BindReader ????

            BindReader(ReaderToBind);

            LakeChabotReader.MANAGED_ACCESS.API_MacGetFirmwareVersion(ref firmwareVersion);
            LakeChabotReader.MANAGED_ACCESS.API_MacGetOEMCfgVersion(ref OemCfgVersion);
            LakeChabotReader.MANAGED_ACCESS.API_MacGetOEMCfgUpdateNumber(ref OemCfgUpdateNumber);
            LakeChabotReader.MANAGED_ACCESS.API_MacGetBootLoaderVersion(ref bootLoaderVersion);

            UInt32 version = 0;

            LakeChabotReader.MANAGED_ACCESS.API_ConfigReadRegister(0x0002, ref version);

            this.hardwareVersion.major = (version >> 16) & 0xFFFF;  // control block
            this.hardwareVersion.minor = (version >> 0) & 0xFFFF;   // chip rev
            this.hardwareVersion.release = 0;         

        }

        private LakeChabotReader( bool noStartup )
        {
            Mode = rfidReader.OperationMode.Static;
            InitReader( );
        }

        ~LakeChabotReader( )
        {
            Dispose( false );
        }

        public static bool NoTempFileAccess
        {
            get { return _noTempFileAccess; }
            set { _noTempFileAccess = value; }
        }

        public static bool EnableLogging
        {
            get { return _enableLogging; }
            set { _enableLogging = value; }
        }

        /*
        public static string ProfileListFilename
        {
            get { return _profileListFilename; }
            set { _profileListFilename = value; }
        }
        */

        public static string LogPath
        {
            get { return PacketLogger.PathName; }
            set { PacketLogger.PathName = value; }
        }

        public PacketArrayList RecentPacketList
        {
            get { return _recentPacketList; }
            // set { _recentPacketList = value; }
        }

        // public rfidPropertyList<rfidReader.StandardProperties> Properties
        // {
        //     get { return _properties; }
        // }

        //clark 2011.2.18 Record that user request data length
        public int TagAccessReqCount
        {
            set { _iTagAccessReqCount = value; }
            get { return _iTagAccessReqCount; }
        }

        //把计         
        public int TagAccessReqCountRead
        {
            set { _iTagAccessReqCountRead = value; }
            get { return _iTagAccessReqCountRead; }
        }

        public int RequestCount
        {
            get { return _commonRequestIndex + 1; }
        }

        public int AccessCount
        {
            get { return _commonAccessCount; }
        }

        public int BadPacketCount
        {
            get { return _commonBadIndex + 1; }
        }

        public int CRCErrorCount
        {
            get { return _cRCErrors; }
        }

        public int CommandErrors
        {
            get { return _cmdEndErrors; }
            set { _cmdEndErrors = value; }
        }

        public int RawPacketCount
        {
            get { return _rawPacketCount; }
        }

        public int RawCommandCount
        {
            get { return _rawCommandCount; }
        }

        public int RawAntennaCycleCount
        {
            get { return _rawAntennaCycleCount; }
        }

        public int RawAntennaCount
        {
            get { return _rawAntennaCount; }
        }

        public int RawInventoryCycleCount
        {
            get { return _rawInventoryCycleCount; }
        }

        public int RawRoundCount
        {
            get { return _rawRoundCount; }
        }

        public int RawInventoryCount
        {
            get { return _rawInventoryCount; }
        }

        public UInt32? LastUsedAntenna
        {
            get { return _lastUsedAntenna; }
            private set { _lastUsedAntenna = value; }
        }

        public UInt32? LastCommandResult
        {
            get { return _lastCmdResult; }
            set { _lastCmdResult = value; }
        }

        public int ProcessedPacketCount
        {
            get { return _processedPacketIndex + 1; }
        }

        public int ProcessedCommandCount
        {
            get { return _processedCommandIndex + 1; }
        }

        public int ProcessedAntennaCycleCount
        {
            get { return _processedAntennaCycleIndex + 1; }
        }

        public int ProcessedAntennaCount
        {
            get { return _processedAntennaIndex + 1; }
        }

        public int ProcessedInventoryCycleCount
        {
            get { return _processedInventoryCycleIndex + 1; }
        }

        public int ProcessedRoundCount
        {
            get { return _processedRoundIndex + 1; }
        }

        public int ProcessedInventoryCount
        {
            get { return _processedInventoryIndex + 1; }
        }

        public int ProcessedCmdReadCount
        {
            get { return _processedCmdReadCount; }
        }

        /// <summary>
        /// Called after the public properties have been changed.
        /// </summary>
        public void SettingsChanged( )
        {
            //TODO implement SettingsChanged
            throw new NotImplementedException( "SettingsChanged()" );
        }

        public void SetProperty( string name, object value )
        {
            KeyList<PropertyBag> key = new KeyList<PropertyBag>( "propName", name );

            PropertyBag myBag = new PropertyBag( );

            if ( PropertyBagData.Contains( key ) )
            {
                myBag = ( PropertyBag ) PropertyBagData[ key ];
                myBag.value = value;
                PropertyBagData[ key ] = myBag;
            }
            else
            {
                myBag.propName = name;
                myBag.value = value;
                PropertyBagData.Add( myBag );
            }
        }

        public string GetPropertyAsString( string name )
        {
            KeyList<PropertyBag> key = new KeyList<PropertyBag>( "propName", name );

            PropertyBag myBag = new PropertyBag( );

            if ( PropertyBagData.Contains( key ) )
            {
                myBag = ( PropertyBag ) PropertyBagData[ key ];
                if ( myBag == null || myBag.Value == null )
                    return String.Empty;

                return myBag.value.ToString( );
            }
            return String.Empty;
        }

        public rfid.Constants.Result MacReset( )
        {
            return LakeChabotReader.MANAGED_ACCESS.API_ControlSoftReset();
        }

        public rfid.Constants.Result MacGetError(out uint errorCode, out uint lastErrorCode)
        {
            errorCode = uint.MaxValue;
            lastErrorCode = uint.MaxValue;
            return LakeChabotReader.MANAGED_ACCESS.API_MacGetError(ref errorCode, ref lastErrorCode);
        }

        public rfid.Constants.Result MacClearError()
        {
            return LakeChabotReader.MANAGED_ACCESS.API_MacClearError();
        }

        public void ClearSession( )
        {
            PacketLogger.Clear( );

#pragma warning disable 420 // reference to a volatile field is valid for Interlocked call
            System.Threading.Interlocked.Exchange( ref _commonRequestIndex, -1 );
            System.Threading.Interlocked.Exchange( ref _commonBadIndex, -1 );

            System.Threading.Interlocked.Exchange( ref _rawPacketCount, 0 );
            System.Threading.Interlocked.Exchange( ref _rawCommandCount, 0 );
            System.Threading.Interlocked.Exchange( ref _rawAntennaCycleCount, 0 );
            System.Threading.Interlocked.Exchange( ref _rawAntennaCount, 0 );
            System.Threading.Interlocked.Exchange( ref _rawInventoryCycleCount, 0 );
            System.Threading.Interlocked.Exchange( ref _rawRoundCount, 0 );
            System.Threading.Interlocked.Exchange( ref _rawInventoryCount, 0 );
            System.Threading.Interlocked.Exchange( ref _commonAccessCount, 0 );
            System.Threading.Interlocked.Exchange( ref _cRCErrors, 0 );

            System.Threading.Interlocked.Exchange( ref _processedPacketIndex, -1 );
            System.Threading.Interlocked.Exchange( ref _processedCommandIndex, -1 );
            System.Threading.Interlocked.Exchange( ref _processedAntennaCycleIndex, -1 );
            System.Threading.Interlocked.Exchange( ref _processedAntennaIndex, -1 );
            System.Threading.Interlocked.Exchange( ref _processedInventoryCycleIndex, -1 );
            System.Threading.Interlocked.Exchange( ref _processedRoundIndex, -1 );
            // System.Threading.Interlocked.Exchange(ref _processedEndRoundIndex, -1);

            System.Threading.Interlocked.Exchange( ref _processedInventoryIndex, -1 );
            System.Threading.Interlocked.Exchange( ref _processedCmdReadCount, 0 );
            System.Threading.Interlocked.Exchange( ref _maxQueueSize, -1 );
#pragma warning restore 420

            // _stopwatch.Reset();
            // _stopwatch.Start();
            DateTime temp = DateTime.UtcNow;
            _sessionStartMS = HighResolutionTimer.Milliseconds;
            // _sessionStart = new DateTime( temp.Year, temp.Month,  temp.Day,
            //                               temp.Hour, temp.Minute, temp.Second,
            //                               (int)_stopwatch.ElapsedMilliseconds,
            //                               DateTimeKind.Utc );

            _sessionStart = new DateTime( temp.Year, temp.Month, temp.Day,
                                          temp.Hour, temp.Minute, temp.Second,
                                          Math.Min( 999, ( int ) ElapsedMilliseconds ),
                                          DateTimeKind.Utc );

            _inventoryMatrix.Clear( );

            _sessionTagList.Clear( );
            _requestTagList.Clear( );
            _periodTagList.Clear( );
            _commandTagList.Clear( );
            _antennaCycleTagList.Clear( );
            _antennaTagList.Clear( );
            _inventoryCycleTagList.Clear( );
            _inventoryRoundTagList.Clear( );

            LastUsedAntenna = null;
            LastCommandResult = null;

            RecentPacketList.Clear( );

            PropertyBagData.Clear( );
            ReaderRequestData.Clear( );
            PacketStreamData.Clear( );
            ReaderCommandData.Clear( );
            ReaderAntennaCycleData.Clear( );
            AntennaCycleData.Clear( );
            TagInventoryData.Clear( );
            InventoryRoundData.Clear( );
            InventoryCycleData.Clear( );
            TagReadData.Clear( );
            ReadRateData.Clear( );
            BadPacketData.Clear( );

            if ( _fileHandler != null ) _fileHandler.Close( );
            _fileHandler = null;
        }

        public LakeChabotReader BindReader( rfidReaderID Reader )
        {
            if ( IsDisposed )
            {
                throw new ObjectDisposedException( "LakeChabotReader" );
            }

            if ( rfid.Constants.Result.OK != LakeChabotReader.LIBRARY_Result )
            {
                throw new rfidException( rfidErrorCode.LibraryFailedToInitialize );
            }

            if ( Mode == rfidReader.OperationMode.Static )
            {
                throw new rfidException( rfidErrorCode.CannotBindToStaticReader );
            }

            if ( Mode == rfidReader.OperationMode.BoundToReader )
            {
                throw new rfidException( rfidErrorCode.AlreadyBoundToAReader );
            }

            if ( Reader == null )
            {
                throw new ArgumentNullException( "Reader" );
            }

            //clark 2011.3.23 既留旅
            //if ( Reader.Handle == 0 )
            //{
            //    throw new rfidException( rfidErrorCode.InvalidRfidReaderID );
            //}

            _theReaderID = Reader;

            if ( Result.OK != this.initAntennaList( ) )
            {
                Console.WriteLine( "Error initializing antenna information from radio" );
            }

            CreateDataSet( );

            FunctionController.Name = Reader.Name;

            Mode = rfidReader.OperationMode.BoundToReader;

            _sessionTagList = new rfidTagList( );
            _requestTagList = new rfidTagList( );
            _periodTagList = new rfidTagList( );
            _commandTagList = new rfidTagList( );
            _antennaCycleTagList = new rfidTagList( );
            _antennaTagList = new rfidTagList( );
            _inventoryCycleTagList = new rfidTagList( );
            _inventoryRoundTagList = new rfidTagList( );

            DateTime temp = DateTime.UtcNow;
            _sessionStartMS = HighResolutionTimer.Milliseconds;
            _sessionStart = new DateTime( temp.Year, temp.Month,  temp.Day,
                                          temp.Hour, temp.Minute, temp.Second,
                                          Math.Min( 999, ( int ) ElapsedMilliseconds ),
                                          DateTimeKind.Utc );
            return this;
        }

        private void CreateDataSet( )
        {
            if ( IsDisposed )
                throw new ObjectDisposedException( "LakeChabotReader" );

            new ReaderRequest( ).ValidateOrdinalValues( );
            new PacketStream( ).ValidateOrdinalValues( );
            new ReaderCommand( ).ValidateOrdinalValues( );
            new ReaderAntennaCycle( ).ValidateOrdinalValues( );
            new AntennaPacket( ).ValidateOrdinalValues( );
            new InventoryCycle( ).ValidateOrdinalValues( );
            new InventoryRound( ).ValidateOrdinalValues( );
            new TagRead( ).ValidateOrdinalValues( );
            new TagInventory( ).ValidateOrdinalValues( );
            new BadPacket( ).ValidateOrdinalValues( );
            new ReadRate( ).ValidateOrdinalValues( );

            _recentPacketList = new PacketArrayListGlue( this );
            _inventoryMatrix = new TagCycleMatrix( EMA_PERIOD );

            _propertyBagData = new DataFile<PropertyBag>( RFID.RFIDInterface.Properties.Settings.Default.PropertyBagPageSize );
            _tagInventoryData = new DataFile<TagInventory>( RFID.RFIDInterface.Properties.Settings.Default.TagInventoryPageSize );

            _readerRequestData = new SequentialDataFile<ReaderRequest>( RFID.RFIDInterface.Properties.Settings.Default.ReaderRequestPageSize );
            _packetStreamData = new SequentialDataFile<PacketStream>( RFID.RFIDInterface.Properties.Settings.Default.PacketStreamPageSize );
            _readerCommandData = new SequentialDataFile<ReaderCommand>( RFID.RFIDInterface.Properties.Settings.Default.ReaderCommandPageSize );
            _readerAntennaCycleData = new SequentialDataFile<ReaderAntennaCycle>( RFID.RFIDInterface.Properties.Settings.Default.ReaderCyclePageSize );
            _antennaCycleData = new SequentialDataFile<AntennaPacket>( RFID.RFIDInterface.Properties.Settings.Default.AntennaCyclePageSize );
            _inventoryCycleData = new SequentialDataFile<InventoryCycle>( RFID.RFIDInterface.Properties.Settings.Default.InventoryCyclePageSize );
            _inventoryRoundData = new SequentialDataFile<InventoryRound>( RFID.RFIDInterface.Properties.Settings.Default.InventoryRoundPageSize );
            _tagReadData = new SequentialDataFile<TagRead>( RFID.RFIDInterface.Properties.Settings.Default.TagReadPageSize );
            _readRateData = new SequentialDataFile<ReadRate>( RFID.RFIDInterface.Properties.Settings.Default.ReadRatePageSize );
            _badPacketData = new SequentialDataFile<BadPacket>( RFID.RFIDInterface.Properties.Settings.Default.BadPacketPageSize );
        }

        public static rfid.Constants.LibraryMode LibraryMode
        {
            get
            {
                //!!!! saving mode as settings - check program flow... short circuit for now
                return rfid.Constants.LibraryMode.DEFAULT;
                //return ( rfid.Constants.LibraryMode ) Enum.ToObject
                //    (
                //        typeof( rfid.Constants.LibraryMode ),
                //        RFID.RFIDInterface.Properties.Settings.Default.LibraryMode
                //    );
            }
            set
            {
                RFID.RFIDInterface.Properties.Settings.Default.LibraryMode = ( int ) value;
                RFID.RFIDInterface.Properties.Settings.Default.Save( );
            }
        }

        public static rfid.Constants.RadioOperationMode RadioOperationMode
        {
            get
            {
                //!!!! saving mode as settings - check program flow... short circuit for now
                return rfid.Constants.RadioOperationMode.NONCONTINUOUS;
                //return ( rfid.Constants.RadioOperationMode ) Enum.ToObject
                //    (
                //        typeof( rfid.Constants.RadioOperationMode ),
                //        RFID.RFIDInterface.Properties.Settings.Default.LibraryMode
                //    );
            }
            set
            {
                RFID.RFIDInterface.Properties.Settings.Default.LibraryMode = ( int ) value;
                RFID.RFIDInterface.Properties.Settings.Default.Save( );
            }
        }

        /*
        private void UpdatePropertiesForBoundReader(Object context)
        {
            if (IsDisposed)
                throw new ObjectDisposedException("LakeChabotReader");

            ManualResetEvent evnt = context as ManualResetEvent;
            if (evnt == null)
                throw new ArgumentException("UpdatePropertiesForBoundReader() must be passed a ManualResetEvent", "context");

            foreach (rfidProperty<rfidReader.StandardProperties> property in Properties.Values)
            {
                if (property.IsStandard)
                {
                    switch (property.Property)
                    {
                    case rfidReader.StandardProperties.AntennaList:
                        rfidAntennaList antList = new rfidAntennaList();
                        antList.Antennas.Add(new rfidAntenna());
                        for (int i = 0; i < antList.Antennas.Count; i++)
                        {
                            rfidAntenna A = antList.Antennas[i];
                            foreach (rfidProperty<rfidAntenna.StandardAntennaProperty> prop in A.Properties.Values)
                            {
                                if (prop.IsStandard)
                                {
                                    switch (prop.Property)
                                    {
                                    case rfidAntenna.StandardAntennaProperty.AntennaId:
                                        prop.Value = i;
                                        break;

                                    case rfidAntenna.StandardAntennaProperty.Enabled:
                                        prop.Value = true;
                                        break;

                                    case rfidAntenna.StandardAntennaProperty.AntennaPower:
                                        prop.Value = "50 %";
                                        break;

                                    case rfidAntenna.StandardAntennaProperty.AntennaPowerRange:
                                        break;

                                    default:
                                        break;
                                    }
                                }
                            }
                        }
                        property.Value = antList;
                        break;
                    case rfidReader.StandardProperties.CurrentReadMode:
                        break;
                    case rfidReader.StandardProperties.CurrentTestMode:
                        break;
                    case rfidReader.StandardProperties.EnabledTagClasses:
                        break;
                    case rfidReader.StandardProperties.Manufacturer:
                        break;
                    case rfidReader.StandardProperties.Model:
                        break;
                    case rfidReader.StandardProperties.ReaderType:
                        break;
                    case rfidReader.StandardProperties.SupportedCommandSet:
                        break;
                    case rfidReader.StandardProperties.SupportedReadModes:
                        break;
                    case rfidReader.StandardProperties.SupportedStatistics:
                        break;
                    case rfidReader.StandardProperties.SupportedTagClasses:
                        break;
                    case rfidReader.StandardProperties.SupportedTestModes:
                        break;
                    case rfidReader.StandardProperties.Version:
                        break;
                    default:
                        break;
                    }
                }
            }

            // notify complete
            evnt.Set();
        }
        */

        public void CloseReader( )
        {
            Dispose( );
        }

        public ReportBase SaveDataToFile( Object context, BackgroundWorker worker, int refreshMS, string Filename )
        {
            if ( IsDisposed )
                throw new ObjectDisposedException( "LakeChabotReader" );

            if ( TableResult != TableState.Ready )
                //return new rfidOperationReport(context, OperationOutcome.FailByContext, new rfidException(rfidErrorCode.TablesAreNotReady, "You must build the post-capture views before saving."));
                return new rfidSimpleReport( context, OperationOutcome.FailByContext, new rfidException( rfidErrorCode.TablesAreNotReady, "You must build the post-capture views before saving." ) );

            try
            {
                rfidSimpleReport report = new rfidSimpleReport( context, HighResolutionTimer.Milliseconds );

                string tempDirectory = Directory.CreateDirectory( Path.Combine( Path.GetTempPath( ), Path.GetFileNameWithoutExtension( Path.GetRandomFileName( ) ) ) ).FullName;

                string[ ] fileNameArray =
                {
                    Path.Combine(tempDirectory, "PropertyBagData.df"),
                    Path.Combine(tempDirectory,	"AntennaCycleData.df"),
                    Path.Combine(tempDirectory, "InventoryRoundData.df"),
                    Path.Combine(tempDirectory,	"PacketStreamData.df"),
                    Path.Combine(tempDirectory, "ReaderCommandData.df"),
                    Path.Combine(tempDirectory,	"ReaderCycleData.df"),
                    Path.Combine(tempDirectory, "ReaderRequestData.df"),
                    Path.Combine(tempDirectory,	"TagInventoryData.df"),
                    Path.Combine(tempDirectory,	"TagReadData.df"),
                    Path.Combine(tempDirectory, "ReadRateData.df"),
                    Path.Combine(tempDirectory,	"BadPacketData.df"),
                    Path.Combine(tempDirectory, "InventoryCycleData.df"),
                };

                PropertyBagData.CommitPageAndCopyFile( fileNameArray[ 0 ] );

                AntennaCycleData.CommitPageAndCopyFile( fileNameArray[ 1 ] );
                worker.ReportProgress( 10, report.GetProgressReport( HighResolutionTimer.Milliseconds ) );

                InventoryRoundData.CommitPageAndCopyFile( fileNameArray[ 2 ] );
                worker.ReportProgress( 15, report.GetProgressReport( HighResolutionTimer.Milliseconds ) );
                Thread.Sleep( 20 );

                PacketStreamData.CommitPageAndCopyFile( fileNameArray[ 3 ] );
                worker.ReportProgress( 20, report.GetProgressReport( HighResolutionTimer.Milliseconds ) );
                Thread.Sleep( 20 );

                ReaderCommandData.CommitPageAndCopyFile( fileNameArray[ 4 ] );
                worker.ReportProgress( 20, report.GetProgressReport( HighResolutionTimer.Milliseconds ) );
                Thread.Sleep( 20 );

                ReaderAntennaCycleData.CommitPageAndCopyFile( fileNameArray[ 5 ] );
                worker.ReportProgress( 30, report.GetProgressReport( HighResolutionTimer.Milliseconds ) );

                ReaderRequestData.CommitPageAndCopyFile( fileNameArray[ 6 ] );
                worker.ReportProgress( 30, report.GetProgressReport( HighResolutionTimer.Milliseconds ) );
                Thread.Sleep( 20 );

                TagInventoryData.CommitPageAndCopyFile( fileNameArray[ 7 ] );
                worker.ReportProgress( 40, report.GetProgressReport( HighResolutionTimer.Milliseconds ) );
                Thread.Sleep( 20 );

                TagReadData.CommitPageAndCopyFile( fileNameArray[ 8 ] );
                worker.ReportProgress( 40, report.GetProgressReport( HighResolutionTimer.Milliseconds ) );
                Thread.Sleep( 20 );

                ReadRateData.CommitPageAndCopyFile( fileNameArray[ 9 ] );
                worker.ReportProgress( 40, report.GetProgressReport( HighResolutionTimer.Milliseconds ) );
                Thread.Sleep( 20 );

                BadPacketData.CommitPageAndCopyFile( fileNameArray[ 10 ] );
                worker.ReportProgress( 50, report.GetProgressReport( HighResolutionTimer.Milliseconds ) );

                InventoryCycleData.CommitPageAndCopyFile( fileNameArray[ 11 ] );
                worker.ReportProgress( 60, report.GetProgressReport( HighResolutionTimer.Milliseconds ) );
                Thread.Sleep( 20 );

                String zipFileName = Path.GetTempFileName( );
                File.Delete( zipFileName );
                zipFileName = Path.ChangeExtension( zipFileName, ".rfi" );

                FileCompressor.Compress( zipFileName, true, fileNameArray );

                worker.ReportProgress( 80, report.GetProgressReport( HighResolutionTimer.Milliseconds ) );

                File.Copy( zipFileName, Filename, true );

                worker.ReportProgress( 90, report.GetProgressReport( HighResolutionTimer.Milliseconds ) );

                File.Delete( zipFileName );
                Directory.Delete( tempDirectory );

                report.OperationCompleted( OperationOutcome.Success,
                                           String.Format( "Successfuly opened {0}", Filename ),
                                           HighResolutionTimer.Milliseconds );
                return report;
            }
            catch ( Exception e )
            {
                return new rfidSimpleReport( context, OperationOutcome.FailByApplicationError, e );
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public ReportBase BuildTables( Object context, BackgroundWorker worker, int refreshMS )
        {
            Debug.WriteLine( String.Format( "{0}() ThreadID = {1}", System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Thread.CurrentThread.ManagedThreadId ) );

            if ( IsDisposed )
                throw new ObjectDisposedException( "LakeChabotReader" );

            if ( Mode != rfidReader.OperationMode.BoundToReader )
                return new rfidSimpleReport( context, OperationOutcome.FailByContext,
                    new rfidException( rfidErrorCode.ReaderIsNotBound, "Reader must be bound before invoking BuildTables." ) );

            if ( _control.State != FunctionControl.FunctionState.Idle )
                return new rfidSimpleReport( context, OperationOutcome.FailByContext, new Exception( "Cannot build tables, the prior task has not completed" ) );

            if ( null == worker )
                return new rfidSimpleReport( context, OperationOutcome.FailByApplicationError, new ArgumentNullException( "worker", "BackgroundWorker is required" ) );

            if ( refreshMS < MIN_REFRESH_MS || refreshMS > MAX_REFRESH_MS )
                return new rfidSimpleReport( context, OperationOutcome.FailByApplicationError, new ArgumentOutOfRangeException( "refreshMS", refreshMS, string.Format( "Value must be between {0} and {1}", MIN_REFRESH_MS, MAX_REFRESH_MS ) ) );

            _refreshRateMS = refreshMS;
            _bgdWorker = worker;
            _periodTagList.Clear( );
            _requestTagList.Clear( );
            rfidOperationReport report
                = new rfidOperationReport( context,
                                           ElapsedMilliseconds,
                                           RequestCount,
                                           ProcessedAntennaCycleCount,
                                           ProcessedInventoryCycleCount,
                                           BadPacketCount,
                                           CRCErrorCount,
                                           ProcessedPacketCount,
                                           ProcessedRoundCount,
                                           ProcessedInventoryCount,
                                           SessionUniqueTags,
                                           RequestUniqueTags,
                                           CurrentUniqueTags,
                                           SessionDuration );

            DateTime ReportDue = DateTime.Now.AddMilliseconds( refreshMS );
            OperationOutcome outcome = OperationOutcome.Success;
            _control.StartAction( );

            int totalPacketsToProcess = FileHandler.TotalPacketCount - ProcessedPacketCount;
            int localPacketCount = 0;

            PacketData.PacketWrapper envelope;

            while ( ( envelope = FileHandler.GetPendingPacket( ) ) != null )
            {
                localPacketCount++;
                ProcessPacket( envelope );
                DateTime now = DateTime.Now;
                if ( ReportDue.Ticks <= now.Ticks )
                {
                    _bgdWorker.ReportProgress(
                        ( int ) ( 100 * ( ( float ) localPacketCount / ( float ) totalPacketsToProcess ) ),
                        report.GetProgressReport( ElapsedMilliseconds,
                                                  RequestCount,
                                                  ProcessedAntennaCycleCount,
                                                  ProcessedInventoryCycleCount,
                                                  BadPacketCount,
                                                  CRCErrorCount,
                                                  ProcessedPacketCount,
                                                  ProcessedRoundCount,
                                                  ProcessedInventoryCount,
                                                  SessionUniqueTags,
                                                  RequestUniqueTags,
                                                  CurrentUniqueTags,
                                                  SessionDuration ) );
                    _periodTagList.Clear( );
                    ReportDue = now.AddMilliseconds( refreshMS );
                }
                FunctionControl.RequestedAction action = FunctionController.GetActionRequest( );
                if ( action != FunctionControl.RequestedAction.Continue )
                {
                    break;
                }
            }

            switch ( _control.State )
            {
                case FunctionControl.FunctionState.Stopping:
                    outcome = OperationOutcome.SuccessWithUserCancel;
                    break;

                case FunctionControl.FunctionState.Aborting:
                    outcome = OperationOutcome.FailByUserAbort;
                    break;

                case FunctionControl.FunctionState.Running:
                    outcome = OperationOutcome.Success;
                    break;

                case FunctionControl.FunctionState.Idle:
                case FunctionControl.FunctionState.Paused:
                case FunctionControl.FunctionState.Unknown:
                default:
                    outcome = OperationOutcome.Unknown;
                    break;
            }

            _control.Finished( );

            report.OperationCompleted( outcome,
                                       ElapsedMilliseconds,
                                       RequestCount,
                                       RawAntennaCycleCount,
                                       RawInventoryCycleCount,
                                       BadPacketCount,
                                       CRCErrorCount,
                                       RawPacketCount,
                                       RawRoundCount,
                                       RawInventoryCount,
                                       SessionUniqueTags,
                                       RequestUniqueTags,
                                       CurrentUniqueTags,
                                       SessionDuration );
            return report;
        }

        /// <summary>
        /// 
        /// </summary>
        public static ReportBase LoadFileIntoStaticReader( Object context, BackgroundWorker worker, int refreshMS, string zipFileName )
        {
            Debug.WriteLine( String.Format( "{0}() ThreadID = {1}", System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Thread.CurrentThread.ManagedThreadId ) );

            if ( null == worker )
                return new rfidSimpleReport( context, OperationOutcome.FailByApplicationError, new ArgumentNullException( "worker", "BackgroundWorker is required" ) );

            if ( refreshMS < MIN_REFRESH_MS || refreshMS > MAX_REFRESH_MS )
                return new rfidSimpleReport( context, OperationOutcome.FailByApplicationError, new ArgumentOutOfRangeException( "refreshMS", refreshMS, string.Format( "Value must be between {0} and {1}", MIN_REFRESH_MS, MAX_REFRESH_MS ) ) );

            if ( zipFileName == null )
                return new rfidSimpleReport( context, OperationOutcome.FailByApplicationError, new ArgumentNullException( "zipFileName" ) );

            if ( !File.Exists( zipFileName ) )
                return new rfidSimpleReport( context, OperationOutcome.FailByApplicationError, new ArgumentOutOfRangeException( "ziFileName", zipFileName, "File does not exit." ) );

            try
            {
                rfidSimpleReport report = new rfidSimpleReport( context, 0 );

                LakeChabotReader reader = new LakeChabotReader( false );

                reader._staticReaderDir = Directory.CreateDirectory( Path.Combine( Path.GetTempPath( ), Path.GetFileNameWithoutExtension( Path.GetRandomFileName( ) ) ) ).FullName;

                worker.ReportProgress( 10, report.GetProgressReport( HighResolutionTimer.Milliseconds ) );

                FileCompressor.Decompress( zipFileName, reader._staticReaderDir );

                worker.ReportProgress( 20, report.GetProgressReport( HighResolutionTimer.Milliseconds ) );

                reader._propertyBagData = new DataFile<PropertyBag>( RFID.RFIDInterface.Properties.Settings.Default.PropertyBagPageSize, Path.Combine( reader._staticReaderDir, "PropertyBagData.df" ) );

                reader._antennaCycleData = new SequentialDataFile<AntennaPacket>( RFID.RFIDInterface.Properties.Settings.Default.AntennaCyclePageSize, Path.Combine( reader._staticReaderDir, "AntennaCycleData.df" ) );

                worker.ReportProgress( 30, report.GetProgressReport( HighResolutionTimer.Milliseconds ) );

                reader._inventoryRoundData = new SequentialDataFile<InventoryRound>( RFID.RFIDInterface.Properties.Settings.Default.InventoryRoundPageSize, Path.Combine( reader._staticReaderDir, "InventoryRoundData.df" ) );

                worker.ReportProgress( 40, report.GetProgressReport( HighResolutionTimer.Milliseconds ) );

                reader._packetStreamData = new SequentialDataFile<PacketStream>( RFID.RFIDInterface.Properties.Settings.Default.PacketStreamPageSize, Path.Combine( reader._staticReaderDir, "PacketStreamData.df" ) );

                worker.ReportProgress( 50, report.GetProgressReport( HighResolutionTimer.Milliseconds ) );

                reader._readerCommandData = new SequentialDataFile<ReaderCommand>( RFID.RFIDInterface.Properties.Settings.Default.ReaderCommandPageSize, Path.Combine( reader._staticReaderDir, "ReaderCommandData.df" ) );

                reader._readerAntennaCycleData = new SequentialDataFile<ReaderAntennaCycle>( RFID.RFIDInterface.Properties.Settings.Default.ReaderCyclePageSize, Path.Combine( reader._staticReaderDir, "ReaderCycleData.df" ) );

                worker.ReportProgress( 60, report.GetProgressReport( HighResolutionTimer.Milliseconds ) );

                reader._readerRequestData = new SequentialDataFile<ReaderRequest>( RFID.RFIDInterface.Properties.Settings.Default.ReaderRequestPageSize, Path.Combine( reader._staticReaderDir, "ReaderRequestData.df" ) );

                worker.ReportProgress( 70, report.GetProgressReport( HighResolutionTimer.Milliseconds ) );

                reader._tagInventoryData = new DataFile<TagInventory>( RFID.RFIDInterface.Properties.Settings.Default.TagInventoryPageSize, Path.Combine( reader._staticReaderDir, "TagInventoryData.df" ) );

                reader._tagReadData = new SequentialDataFile<TagRead>( RFID.RFIDInterface.Properties.Settings.Default.TagReadPageSize, Path.Combine( reader._staticReaderDir, "TagReadData.df" ) );

                worker.ReportProgress( 80, report.GetProgressReport( HighResolutionTimer.Milliseconds ) );

                reader._readRateData = new SequentialDataFile<ReadRate>( RFID.RFIDInterface.Properties.Settings.Default.ReadRatePageSize, Path.Combine( reader._staticReaderDir, "ReadRateData.df" ) );

                reader._badPacketData = new SequentialDataFile<BadPacket>( RFID.RFIDInterface.Properties.Settings.Default.BadPacketPageSize, Path.Combine( reader._staticReaderDir, "BadPacketData.df" ) );

                reader._inventoryCycleData = new SequentialDataFile<InventoryCycle>( RFID.RFIDInterface.Properties.Settings.Default.InventoryCyclePageSize, Path.Combine( reader._staticReaderDir, "InventoryCycleData.df" ) );

                worker.ReportProgress( 90, report.GetProgressReport( HighResolutionTimer.Milliseconds ) );

                reader._theReaderID = new rfidReaderID(rfidReaderID.ReaderType.MTI, zipFileName);

                reader._recentPacketList = new PacketArrayListGlue( reader );

                report.NewReader = reader;

                report.OperationCompleted( OperationOutcome.Success,
                                           HighResolutionTimer.Milliseconds );
                return report;
            }
            catch ( Exception e )
            {
                return new rfidSimpleReport( context, OperationOutcome.FailByApplicationError, e );
            }
        }

        /// <summary>
        /// 
        /// </summary>
        // Inventory Once
        public ReportBase ReadInventory( Object context, BackgroundWorker worker, int refreshMS )
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine( String.Format( "{0}() ThreadID = {1}", System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Thread.CurrentThread.ManagedThreadId ) );
#endif

            if ( IsDisposed )
            {
                throw new ObjectDisposedException( "LakeChabotReader" );
            }

            if ( Mode != rfidReader.OperationMode.BoundToReader )
            {
                return new rfidSimpleReport( context, OperationOutcome.FailByContext,
                    new rfidException( rfidErrorCode.ReaderIsNotBound, "Reader must be bound before inventory can be read." ) );
            }

            if ( _control.State != FunctionControl.FunctionState.Idle )
            {
                return new rfidSimpleReport( context, OperationOutcome.FailByContext, new Exception( "Cannot read the inventory, the prior task has not completed" ) );
            }

            if ( null == worker )
            {
                return new rfidSimpleReport( context, OperationOutcome.FailByApplicationError, new ArgumentNullException( "worker", "BackgroundWorker is required" ) );
            }

            if ( refreshMS < MIN_REFRESH_MS || refreshMS > MAX_REFRESH_MS )
                return new rfidSimpleReport( context, OperationOutcome.FailByApplicationError, new ArgumentOutOfRangeException( "refreshMS", refreshMS, string.Format( "Value must be between {0} and {1}", MIN_REFRESH_MS, MAX_REFRESH_MS ) ) );


            _refreshRateMS = refreshMS;
            _bgdWorker = worker;
            _requestTagList.Clear( );
            _periodTagList.Clear( );
            rfidOperationReport report
                = new rfidOperationReport( context,
                                           ElapsedMilliseconds,
                                           RequestCount,
                                           RawAntennaCycleCount,
                                           RawInventoryCycleCount,
                                           BadPacketCount,
                                           CRCErrorCount,
                                           RawPacketCount,
                                           RawRoundCount,
                                           RawInventoryCount,
                                           SessionUniqueTags,
                                           RequestUniqueTags,
                                           CurrentUniqueTags,
                                           SessionDuration );

            _control.StartAction( );

            PerformanceCounter processorUtilizationCounter = null;

            long t1 = HighResolutionTimer.Microseconds;
            try
            {
                processorUtilizationCounter = new PerformanceCounter( "Processor", "% Processor Time", "_Total", "." );
            }
            catch ( Exception )
            {
                Debug.WriteLine( "Unable to start processorUtilizationCounter" );
            }

            t1 = HighResolutionTimer.Microseconds - t1;

            Debug.WriteLine( String.Format( "processorUtilizationCounter startup time {0} us", t1 ) );

            //clark 2011.05.27  Inventory Once uses Non-Conitnue mode
            ReaderInterfaceThreadClass threadClass = new ReaderInterfaceThreadClass(_managedAccess, (int)ReaderHandle, true, RadioOperationMode.NONCONTINUOUS, strcTagFlag);
            threadClass._reader = this; // HACK
            Thread runnerThread = new Thread( new ThreadStart( threadClass.InventoryThreadProc ) );
            runnerThread.Name = "ReadInventory";
            runnerThread.IsBackground = true;
            runnerThread.Priority = ThreadPriority.BelowNormal;
            runnerThread.Start( );

            int notused = FileHandler.TotalPacketCount; // Make sure the file is created;

            // Wait for thread to be in ready stat
            int counter = 500;
            while ( counter-- > 0 && ( int ) ( runnerThread.ThreadState & System.Threading.ThreadState.WaitSleepJoin ) == 1 )
            {
                Thread.Sleep( 10 );
            }
            if ( !runnerThread.IsAlive )
            {
                return new rfidSimpleReport( context, OperationOutcome.FailByApplicationError, new ApplicationException( "BackgroundWorker thread could not be started." ) );
            }

            ReaderRequest readerRequest = new ReaderRequest( );

            readerRequest.reader = _theReaderID.Name;
#pragma warning disable 420 // reference to a volatile field is valid for Interlocked call
            readerRequest.requestSequence = System.Threading.Interlocked.Increment( ref _commonRequestIndex );
#pragma warning restore 420
            readerRequest.requestName = rfidReader.GetNameForRequest( rfidReader.ReaderRequestType.GetInventory );
            readerRequest.startTime = GetSessionRelativeDateTime( report.StartTimeMS );
            readerRequest.requestStartTime = report.StartTimeMS;
            readerRequest.startingPacketCount = RawPacketCount;
            readerRequest.startingTagCount = RawInventoryCount;

            MemoryStream data = new MemoryStream( );
            readerRequest.WriteTo( data );
            PacketData.PacketWrapper pseudoPacket = new PacketData.PacketWrapper( new PacketData.CommandPsuedoPacket( readerRequest.requestName, data.ToArray( ) ), PacketData.PacketType.U_N_D_F_I_N_E_D );
            pseudoPacket.IsPseudoPacket = true;
            pseudoPacket.ReaderName = _theReaderID.Name;
            pseudoPacket.ReaderIndex = ( int ) _theReaderID.Handle;
            FileHandler.WritePacket( pseudoPacket );

            DateTime ReportDue = DateTime.Now.AddMilliseconds( refreshMS );
            threadClass.StartEvent.Set( );

            while ( runnerThread.IsAlive )
            {
                CounterSample sample = CounterSample.Empty;
                if ( processorUtilizationCounter != null )
                {
                    try
                    {
                        sample = processorUtilizationCounter.NextSample( );
                    }
                    catch ( Exception ) { }
                }
                ProcessQueuedPackets( );

                QueueEvent.WaitOne( 30, true );
                QueueEvent.Reset( );

                DateTime now = DateTime.Now;

                if ( ReportDue.Ticks <= now.Ticks )
                {
                    // Debug.WriteLine(String.Format("Reporing Progress Now (Elapsed Milliseconds {0})", ElapsedMilliseconds));
                    _bgdWorker.ReportProgress(
                        0,
                        report.GetProgressReport(
                            ElapsedMilliseconds,
                            RequestCount,
                            RawAntennaCycleCount,
                            RawInventoryCycleCount,
                            BadPacketCount,
                            CRCErrorCount,
                            RawPacketCount,
                            RawRoundCount,
                            RawInventoryCount,
                            SessionUniqueTags,
                            RequestUniqueTags,
                            CurrentUniqueTags,
                            SessionDuration ) );
                    _periodTagList.Clear( );
                    ReportDue = now.AddMilliseconds( refreshMS );
                }

                if ( !runnerThread.IsAlive )
                    break;

                if ( FunctionController.GetActionRequest( ) == FunctionControl.RequestedAction.Abort )
                {
                    threadClass.Stop = true;
                    Result abortError = Result.OK;
                    try
                    {
                        abortError = LakeChabotReader.MANAGED_ACCESS.API_ControlAbort();
                    }
                    catch ( Exception e )
                    {
                        Debug.WriteLine( String.Format( "Error attempting to abort: {0}", e.Message ) );
                    }
                    break;
                }

                if ( FunctionController.GetActionRequest( ) == FunctionControl.RequestedAction.Stop )
                {
                    threadClass.Stop = true;
                    Result stopError = Result.OK;
                    try
                    {
                        stopError = LakeChabotReader.MANAGED_ACCESS.API_ControlCancel();
                    }
                    catch ( Exception e )
                    {
                        Debug.WriteLine( String.Format( "Error attempting to stop: {0}", e.Message ) );
                    }
                    if ( Result.OK != stopError )
                    {
                        // Try to abort
                    }
                    break;
                }

                if ( FunctionController.State == FunctionControl.FunctionState.Paused )
                {
                    do
                    {
                        Thread.Sleep( refreshMS );
                    } while ( FunctionController.State == FunctionControl.FunctionState.Paused );
                }

                if ( !runnerThread.IsAlive )
                    break;

                float processorUtilization = 0;

                if ( processorUtilizationCounter != null && sample == CounterSample.Empty )
                {
                    try
                    {
                        processorUtilization = CounterSample.Calculate( sample, processorUtilizationCounter.NextSample( ) );
                    }
                    catch ( Exception ) { }
                }
                // Debug.WriteLine(String.Format("Processor Util: {0}", processorUtilization));

                if ( processorUtilization > ( float ) TARGET_MAX_USAGE_PERCENT )
                    _packetSleepMS += SLEEP_ADD_SUBTRACT_MS;
                else
                    _packetSleepMS -= _packetSleepMS <= MIN_SLEEP_TIME_MS ? 0 : SLEEP_ADD_SUBTRACT_MS;
            }

            runnerThread.Join( );

            // Get any leftover packets
            ProcessQueuedPackets( );

            if ( LastCommandResult != 0 ) MacClearError( );

            OperationOutcome outcome = OperationOutcome.Success;
            string result = threadClass.Result;

            if ( !( result == null || result == "" || result == "OK" ) )
            {
                switch ( _control.State )
                {
                    case FunctionControl.FunctionState.Stopping:
                        outcome = OperationOutcome.SuccessWithUserCancel;
                        break;

                    case FunctionControl.FunctionState.Aborting:
                        outcome = OperationOutcome.FailByUserAbort;
                        break;

                    case FunctionControl.FunctionState.Running:
                    // outcome = OperationOutcome.FailByReaderError;
                    // report.ErrorMessage = result;
                    // break;

                    case FunctionControl.FunctionState.Idle:
                    case FunctionControl.FunctionState.Paused:
                    case FunctionControl.FunctionState.Unknown:
                    default:
                        outcome = OperationOutcome.Unknown;
                        break;
                }
            }

            _control.Finished( );

            report.OperationCompleted( outcome,
                                       ElapsedMilliseconds,
                                       RequestCount,
                                       RawAntennaCycleCount,
                                       RawInventoryCycleCount,
                                       BadPacketCount,
                                       CRCErrorCount,
                                       RawPacketCount,
                                       RawRoundCount,
                                       RawInventoryCount,
                                       SessionUniqueTags,
                                       RequestUniqueTags,
                                       CurrentUniqueTags,
                                       SessionDuration );

            if ( EnableLogging )
            {
                //Push data from buffer to file. And clear buffer.
                PacketLogger.Flush(); 
            }

            return report;
        }



        public ReportBase MonitorPulse(Object context, BackgroundWorker worker, int refreshMS)
        {
            Debug.WriteLine( String.Format( "{0}() ThreadID = {1}", System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Thread.CurrentThread.ManagedThreadId ) );

            if ( IsDisposed )
                throw new ObjectDisposedException( "LakeChabotReader" );

            if ( Mode != rfidReader.OperationMode.BoundToReader )
                return new rfidSimpleReport( context, OperationOutcome.FailByContext,
                    new rfidException( rfidErrorCode.ReaderIsNotBound, "Reader must be bound before pulse.") );

            if ( _control.State != FunctionControl.FunctionState.Idle )
                return new rfidSimpleReport( context, OperationOutcome.FailByContext, new Exception("Cannot pulse, the prior task has not completed") );

            if ( null == worker )
                return new rfidSimpleReport( context, OperationOutcome.FailByApplicationError, new ArgumentNullException( "worker", "BackgroundWorker is required" ) );

            if ( refreshMS < MIN_REFRESH_MS || refreshMS > MAX_REFRESH_MS )
                return new rfidSimpleReport( context, OperationOutcome.FailByApplicationError, new ArgumentOutOfRangeException( "refreshMS", refreshMS, string.Format( "Value must be between {0} and {1}", MIN_REFRESH_MS, MAX_REFRESH_MS ) ) );

            _refreshRateMS = refreshMS;
            _bgdWorker     = worker;
            _requestTagList.Clear( );
            _periodTagList.Clear( );

            long asofTimeInElapsedMS = ElapsedMilliseconds;

            rfidOperationReport report
                = new rfidOperationReport( context,
                                           asofTimeInElapsedMS,
                                           RequestCount,
                                           RawAntennaCycleCount,
                                           RawInventoryCycleCount,
                                           BadPacketCount,
                                           CRCErrorCount,
                                           RawPacketCount,
                                           RawRoundCount,
                                           RawInventoryCount,
                                           SessionUniqueTags,
                                           RequestUniqueTags,
                                           CurrentUniqueTags,
                                           GetSessionRelativeSessionDuration(asofTimeInElapsedMS) );

            _control.StartAction( );

            //@@@@_managedAccess.Callback = PacketCallBackFromReader;

            PerformanceCounter processorUtilizationCounter = null;
            try
            {
                processorUtilizationCounter = new PerformanceCounter( "Processor", "% Processor Time", "_Total", "." );
            }
            catch ( Exception ) { }

            //clark 2011.05.27  Inventory uses Conitnue mode
            ReaderInterfaceThreadClass threadClass = new ReaderInterfaceThreadClass(_managedAccess, (int)ReaderHandle, true, RadioOperationMode.CONTINUOUS, strcTagFlag);
            threadClass._reader       = this; // HACK
            Thread runnerThread       = new Thread( new ThreadStart(threadClass.PulseThreadProc) );
            runnerThread.Name         = "MonitorPulse";
            runnerThread.IsBackground = true;
            runnerThread.Priority     = ThreadPriority.BelowNormal;
            runnerThread.Start( );

            int notused = FileHandler.TotalPacketCount; // Make sure the file is created;


            // Wait for thread to be in ready stat
            int counter = 500;
            while ( counter-- > 0 && ( int ) ( runnerThread.ThreadState & System.Threading.ThreadState.WaitSleepJoin ) == 1 )
            {
                Thread.Sleep( 10 );
            }
            if ( !runnerThread.IsAlive )
            {
                return new rfidSimpleReport( context, OperationOutcome.FailByApplicationError, new ApplicationException( "BackgroundWorker thread could not be started." ) );
            }


            ReaderRequest readerRequest           = new ReaderRequest();
            readerRequest.reader                  = _theReaderID.Name;
#pragma warning disable 420 // reference to a volatile field is valid for Interlocked call
            readerRequest.requestSequence         = System.Threading.Interlocked.Increment(ref _commonRequestIndex);
#pragma	warning restore 420
            readerRequest.requestName             = rfidReader.GetNameForRequest(rfidReader.ReaderRequestType.GetInventory);
            readerRequest.startTime               = GetSessionRelativeDateTime(report.StartTimeMS);
            readerRequest.startingPacketCount     = RawPacketCount;
            readerRequest.startingTagCount        = RawInventoryCount;
            MemoryStream data                     = new MemoryStream();
            readerRequest.WriteTo(data);
            PacketData.PacketWrapper pseudoPacket = new PacketData.PacketWrapper(new PacketData.CommandPsuedoPacket(readerRequest.requestName, data.ToArray()), PacketData.PacketType.U_N_D_F_I_N_E_D);
            data.Dispose();
            pseudoPacket.IsPseudoPacket           = true;
            pseudoPacket.ReaderName               = _theReaderID.Name;
            pseudoPacket.ReaderIndex              = (int)_theReaderID.Handle;
            FileHandler.WritePacket(pseudoPacket);

            DateTime ReportDue                    = DateTime.Now.AddMilliseconds(refreshMS);
            threadClass.StartEvent.Set( );

            while ( runnerThread.IsAlive )
            {
                CounterSample sample = CounterSample.Empty;
                if ( processorUtilizationCounter != null )
                {
                    try
                    {
                        sample = processorUtilizationCounter.NextSample( );
                    }
                    catch ( Exception ) { }
                }

                ProcessQueuedPackets( );

                QueueEvent.WaitOne( 30, true );
                QueueEvent.Reset( );

                DateTime now = DateTime.Now;

                if (ReportDue.Ticks <= now.Ticks)
                {
                    _bgdWorker.ReportProgress(
                        0,
                        report.GetProgressReport(
                            ElapsedMilliseconds,
                            RequestCount,
                            RawAntennaCycleCount,
                            RawInventoryCycleCount,
                            BadPacketCount,
                            CRCErrorCount,
                            RawPacketCount,
                            RawRoundCount,
                            RawInventoryCount,
                            SessionUniqueTags,
                            RequestUniqueTags,
                            CurrentUniqueTags,
                            SessionDuration));
                    _periodTagList.Clear();
                    ReportDue = now.AddMilliseconds(refreshMS);
                }

                if ( !runnerThread.IsAlive )
                    break;

                if ( FunctionController.GetActionRequest( ) == FunctionControl.RequestedAction.Abort )
                {
                    threadClass.Stop = true;
                    Result abortError = Result.OK;
                    try
                    {
                        abortError = LakeChabotReader.MANAGED_ACCESS.API_ControlAbort();
                    }
                    catch ( Exception e )
                    {
                        Debug.WriteLine( String.Format( "Error attempting to abort: {0}", e.Message ) );
                    }
                    break;
                }

                if ( FunctionController.GetActionRequest( ) == FunctionControl.RequestedAction.Stop )
                {
                    threadClass.Stop = true;
                    Result stopError = Result.OK;
                    try
                    {
                        stopError = LakeChabotReader.MANAGED_ACCESS.API_ControlCancel();
                    }
                    catch ( Exception e )
                    {
                        Debug.WriteLine( String.Format( "Error attempting to stop: {0}", e.Message ) );
                    }
                    if ( Result.OK != stopError )
                    {
                        // Try to abort
                    }
                    break;
                }

                if ( FunctionController.State == FunctionControl.FunctionState.Paused )
                {
                    do
                    {
                        Thread.Sleep( refreshMS );
                    } while ( FunctionController.State == FunctionControl.FunctionState.Paused );
                }

                if ( !runnerThread.IsAlive )
                    break;
                float processorUtilization = 0;
                if ( processorUtilizationCounter != null && sample != CounterSample.Empty )
                {
                    processorUtilization = CounterSample.Calculate( sample, processorUtilizationCounter.NextSample( ) );
                }
                // Debug.WriteLine(String.Format("Processor Util: {0}", processorUtilization));

                if ( processorUtilization > ( float ) TARGET_MAX_USAGE_PERCENT )
                    _packetSleepMS += SLEEP_ADD_SUBTRACT_MS;
                else
                    _packetSleepMS -= _packetSleepMS <= MIN_SLEEP_TIME_MS ? 0 : SLEEP_ADD_SUBTRACT_MS;
            }

            runnerThread.Join( ); // wait for thread to end

            // Get any leftover packets
            ProcessQueuedPackets( );

            if ( LastCommandResult != 0 ) MacClearError( );

            OperationOutcome outcome = OperationOutcome.Success;
            string result = threadClass.Result;

            if ( !( result == null || result == "" || result == "OK" ) )
            {
                switch ( _control.State )
                {
                    case FunctionControl.FunctionState.Stopping:
                        outcome = OperationOutcome.SuccessWithUserCancel;
                        break;

                    case FunctionControl.FunctionState.Aborting:
                        outcome = OperationOutcome.FailByUserAbort;
                        break;

                    // case FunctionControl.FunctionState.Running:
                    //     outcome = OperationOutcome.FailByReaderError;
                    //     report.ErrorMessage = result;
                    //     break;

                    case FunctionControl.FunctionState.Idle:
                    case FunctionControl.FunctionState.Paused:
                    case FunctionControl.FunctionState.Unknown:
                    default:
                        Debug.Assert( false );
                        outcome = OperationOutcome.Unknown;
                        break;
                }
            }

            report.OperationCompleted( outcome,
                                       ElapsedMilliseconds,
                                       RequestCount,
                                       RawAntennaCycleCount,
                                       RawInventoryCycleCount,
                                       BadPacketCount,
                                       CRCErrorCount,
                                       RawPacketCount,
                                       RawRoundCount,
                                       RawInventoryCount,
                                       SessionUniqueTags,
                                       RequestUniqueTags,
                                       CurrentUniqueTags,
                                       SessionDuration );
            _control.Finished( );

           if ( EnableLogging )
            {
                //Push data from buffer to file. And clear buffer.
                PacketLogger.Flush(); 
            }
           

            return report;

        }// MonitorPulse


        public ReportBase MonitorInventory(Object context, BackgroundWorker worker, int refreshMS)
        {
            Debug.WriteLine( String.Format( "{0}() ThreadID = {1}", System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Thread.CurrentThread.ManagedThreadId ) );

            if ( IsDisposed )
                throw new ObjectDisposedException( "LakeChabotReader" );

            if ( Mode != rfidReader.OperationMode.BoundToReader )
                return new rfidSimpleReport( context, OperationOutcome.FailByContext,
                    new rfidException( rfidErrorCode.ReaderIsNotBound, "Reader must be bound before inventory can be read." ) );

            if ( _control.State != FunctionControl.FunctionState.Idle )
                return new rfidSimpleReport( context, OperationOutcome.FailByContext, new Exception( "Cannot read the inventory, the prior task has not completed" ) );

            if ( null == worker )
                return new rfidSimpleReport( context, OperationOutcome.FailByApplicationError, new ArgumentNullException( "worker", "BackgroundWorker is required" ) );

            if ( refreshMS < MIN_REFRESH_MS || refreshMS > MAX_REFRESH_MS )
                return new rfidSimpleReport( context, OperationOutcome.FailByApplicationError, new ArgumentOutOfRangeException( "refreshMS", refreshMS, string.Format( "Value must be between {0} and {1}", MIN_REFRESH_MS, MAX_REFRESH_MS ) ) );
  
            _refreshRateMS = refreshMS;
            _bgdWorker     = worker;
            _requestTagList.Clear( );
            _periodTagList.Clear( );

            long asofTimeInElapsedMS = ElapsedMilliseconds;

            rfidOperationReport report
                = new rfidOperationReport( context,
                                           asofTimeInElapsedMS,
                                           RequestCount,
                                           RawAntennaCycleCount,
                                           RawInventoryCycleCount,
                                           BadPacketCount,
                                           CRCErrorCount,
                                           RawPacketCount,
                                           RawRoundCount,
                                           RawInventoryCount,
                                           SessionUniqueTags,
                                           RequestUniqueTags,
                                           CurrentUniqueTags,
                                           GetSessionRelativeSessionDuration(asofTimeInElapsedMS) );

            _control.StartAction( );

            //@@@@_managedAccess.Callback = PacketCallBackFromReader;

            PerformanceCounter processorUtilizationCounter = null;
            try
            {
                processorUtilizationCounter = new PerformanceCounter( "Processor", "% Processor Time", "_Total", "." );
            }
            catch ( Exception ) { }

            //clark 2011.05.27  Inventory uses Conitnue mode
            ReaderInterfaceThreadClass threadClass = new ReaderInterfaceThreadClass(_managedAccess, (int)ReaderHandle, true, RadioOperationMode.CONTINUOUS, strcTagFlag);
            threadClass._reader       = this; // HACK
            Thread runnerThread       = new Thread( new ThreadStart( threadClass.InventoryThreadProc ) );
            runnerThread.Name         = "MonitorInventory";
            runnerThread.IsBackground = true;
            runnerThread.Priority     = ThreadPriority.BelowNormal;
            runnerThread.Start( );

            int notused = FileHandler.TotalPacketCount; // Make sure the file is created;

            try
            {
                rfid.Constants.RadioOperationMode continuousMode = ReaderOperationMode;
            }
            catch ( Exception e )
            {
                SysLogger.LogError( e );
                return new rfidSimpleReport( context, OperationOutcome.FailByReaderError, new Exception( "Unable to access reader operation mode.", e ) );
            }

            // Wait for thread to be in ready stat
            int counter = 500;
            while ( counter-- > 0 && ( int ) ( runnerThread.ThreadState & System.Threading.ThreadState.WaitSleepJoin ) == 1 )
            {
                Thread.Sleep( 10 );
            }

            if ( !runnerThread.IsAlive )
            {
                return new rfidSimpleReport( context, OperationOutcome.FailByApplicationError, new ApplicationException( "BackgroundWorker thread could not be started." ) );
            }

            ReaderRequest readerRequest = new ReaderRequest( );
            readerRequest.reader = _theReaderID.Name;
#pragma warning disable 420 // reference to a volatile field is valid for Interlocked call
            readerRequest.requestSequence = System.Threading.Interlocked.Increment( ref _commonRequestIndex );
#pragma	warning restore 420
            readerRequest.requestName = rfidReader.GetNameForRequest( rfidReader.ReaderRequestType.GetInventory );
            readerRequest.startTime = GetSessionRelativeDateTime( report.StartTimeMS );
            readerRequest.startingPacketCount = RawPacketCount;
            readerRequest.startingTagCount = RawInventoryCount;
            MemoryStream data = new MemoryStream( );
            readerRequest.WriteTo( data );
            PacketData.PacketWrapper pseudoPacket = new PacketData.PacketWrapper( new PacketData.CommandPsuedoPacket( readerRequest.requestName, data.ToArray( ) ), PacketData.PacketType.U_N_D_F_I_N_E_D );
            data.Dispose( );
            pseudoPacket.IsPseudoPacket = true;
            pseudoPacket.ReaderName = _theReaderID.Name;
            pseudoPacket.ReaderIndex = ( int ) _theReaderID.Handle;
            FileHandler.WritePacket( pseudoPacket );

            DateTime ReportDue = DateTime.Now.AddMilliseconds( refreshMS );
            threadClass.StartEvent.Set( );

            while ( runnerThread.IsAlive )
            {
                CounterSample sample = CounterSample.Empty;
                if ( processorUtilizationCounter != null )
                {
                    try
                    {
                        sample = processorUtilizationCounter.NextSample( );
                    }
                    catch ( Exception ) { }
                }

                ProcessQueuedPackets( );

                QueueEvent.WaitOne( 30, true );                
                QueueEvent.Reset( );     

                DateTime now = DateTime.Now;

                if ( ReportDue.Ticks <= now.Ticks )
                {
                    _bgdWorker.ReportProgress(
                        0,
                        report.GetProgressReport(
                            ElapsedMilliseconds,
                            RequestCount,
                            RawAntennaCycleCount,
                            RawInventoryCycleCount,
                            BadPacketCount,
                            CRCErrorCount,
                            RawPacketCount,
                            RawRoundCount,
                            RawInventoryCount,
                            SessionUniqueTags,
                            RequestUniqueTags,
                            CurrentUniqueTags,
                            SessionDuration ) );
                    _periodTagList.Clear( );
                    ReportDue = now.AddMilliseconds( refreshMS );
                }

                if ( !runnerThread.IsAlive )
                    break;
                
                if ( FunctionController.GetActionRequest( ) == FunctionControl.RequestedAction.Abort )
                {
                    threadClass.Stop = true;
                    Result abortError = Result.OK;
                    try
                    {
                        abortError = LakeChabotReader.MANAGED_ACCESS.API_ControlAbort();
                    }
                    catch ( Exception e )
                    {
                        Debug.WriteLine( String.Format( "Error attempting to abort: {0}", e.Message ) );
                    }
                    break;
                }

                if ( FunctionController.GetActionRequest( ) == FunctionControl.RequestedAction.Stop )
                {
                    threadClass.Stop = true;
                    Result stopError = Result.OK;
                    try
                    {
                        stopError = LakeChabotReader.MANAGED_ACCESS.API_ControlCancel();
                    }
                    catch ( Exception e )
                    {
                        Debug.WriteLine( String.Format( "Error attempting to stop: {0}", e.Message ) );
                    }
                    if ( Result.OK != stopError )
                    {
                        // Try to abort
                    }
                    break;
                }

                if ( FunctionController.State == FunctionControl.FunctionState.Paused )
                {
                    do
                    {
                        Thread.Sleep( refreshMS );
                    } while ( FunctionController.State == FunctionControl.FunctionState.Paused );
                }

                if ( !runnerThread.IsAlive )
                    break;
                float processorUtilization = 0;
                if ( processorUtilizationCounter != null && sample != CounterSample.Empty )
                {
                    processorUtilization = CounterSample.Calculate( sample, processorUtilizationCounter.NextSample( ) );
                }
                // Debug.WriteLine(String.Format("Processor Util: {0}", processorUtilization));

                if ( processorUtilization > ( float ) TARGET_MAX_USAGE_PERCENT )
                    _packetSleepMS += SLEEP_ADD_SUBTRACT_MS;
                else
                    _packetSleepMS -= _packetSleepMS <= MIN_SLEEP_TIME_MS ? 0 : SLEEP_ADD_SUBTRACT_MS;
            }

            runnerThread.Join( ); // wait for thread to end

            // Get any leftover packets
            ProcessQueuedPackets( );

            if ( LastCommandResult != 0 ) MacClearError( );

            OperationOutcome outcome = OperationOutcome.Success;
            string result = threadClass.Result;

            if ( !( result == null || result == "" || result == "OK" ) )
            {
                switch ( _control.State )
                {
                    case FunctionControl.FunctionState.Stopping:
                        outcome = OperationOutcome.SuccessWithUserCancel;
                        break;

                    case FunctionControl.FunctionState.Aborting:
                        outcome = OperationOutcome.FailByUserAbort;
                        break;

                    // case FunctionControl.FunctionState.Running:
                    //     outcome = OperationOutcome.FailByReaderError;
                    //     report.ErrorMessage = result;
                    //     break;

                    case FunctionControl.FunctionState.Idle:
                    case FunctionControl.FunctionState.Paused:
                    case FunctionControl.FunctionState.Unknown:
                    default:
                        Debug.Assert( false );
                        outcome = OperationOutcome.Unknown;
                        break;
                }
            }

            report.OperationCompleted( outcome,
                                       ElapsedMilliseconds,
                                       RequestCount,
                                       RawAntennaCycleCount,
                                       RawInventoryCycleCount,
                                       BadPacketCount,
                                       CRCErrorCount,
                                       RawPacketCount,
                                       RawRoundCount,
                                       RawInventoryCount,
                                       SessionUniqueTags,
                                       RequestUniqueTags,
                                       CurrentUniqueTags,
                                       SessionDuration );
            _control.Finished( );

            if ( EnableLogging )
            {
                //Push data from buffer to file. And clear buffer.
                PacketLogger.Flush(); 
            }
           

            return report;
        } // MonitorInventory(Object context, BackgroundWorker worker, int refreshMS, int singulationLimit, int readerCycleLimit)

        /// <summary>
        /// Read Tag memory
        /// </summary>
        /// <param name="context"></param>
        /// <param name="worker"></param>
        /// <param name="refreshMS"></param>
        /// <returns></returns>
        //public ReportBase TagAccess(Object context, BackgroundWorker worker, int refreshMS, int r_iTagAccessReqCount)//??把计??//Add LargeRead command
        public ReportBase TagAccess(Object context, BackgroundWorker worker, int refreshMS, int r_iTagAccessReqCount, int r_iTagAccessReqCountRead)//??把计??//Add LargeRead command
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine( String.Format( "{0}() ThreadID = {1}", System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Thread.CurrentThread.ManagedThreadId ) );
#endif
            if ( IsDisposed )
                throw new ObjectDisposedException( "LakeChabotReader" );

            if ( Mode != rfidReader.OperationMode.BoundToReader )
                return new rfidSimpleReport( context, OperationOutcome.FailByContext,
                    new rfidException( rfidErrorCode.ReaderIsNotBound, "Reader must be bound before tag can be read." ) );

            if ( _control.State != FunctionControl.FunctionState.Idle )
                return new rfidSimpleReport( context, OperationOutcome.FailByContext, new Exception( "Cannot read the tag, the prior task has not completed" ) );

            if ( null == worker )
                return new rfidSimpleReport( context, OperationOutcome.FailByApplicationError, new ArgumentNullException( "worker", "BackgroundWorker is required" ) );

            if ( refreshMS < MIN_REFRESH_MS || refreshMS > MAX_REFRESH_MS )
                return new rfidSimpleReport( context, OperationOutcome.FailByApplicationError, new ArgumentOutOfRangeException( "refreshMS", refreshMS, string.Format( "Value must be between {0} and {1}", MIN_REFRESH_MS, MAX_REFRESH_MS ) ) );

            // Tag Read is never in continuous mode
            rfid.Constants.RadioOperationMode priorMode = ReaderOperationMode;
            if ( priorMode != rfid.Constants.RadioOperationMode.NONCONTINUOUS )
            {
                ReaderOperationMode = rfid.Constants.RadioOperationMode.NONCONTINUOUS;
            }

            _refreshRateMS = refreshMS;
            _bgdWorker = worker;
            _requestTagList.Clear( );
            _periodTagList.Clear( );
#pragma warning disable 420 // reference to a volatile field is valid for Interlocked call
            System.Threading.Interlocked.Exchange( ref _commonAccessCount, 0 );
#pragma warning restore 420
            rfidOperationReport report  = new rfidOperationReport( context,
                                                                   ElapsedMilliseconds,
                                                                   RequestCount,
                                                                   RawAntennaCycleCount,
                                                                   RawInventoryCycleCount,
                                                                   BadPacketCount,
                                                                   CRCErrorCount,
                                                                   RawPacketCount,
                                                                   RawRoundCount,
                                                                   RawInventoryCount,
                                                                   SessionUniqueTags,
                                                                   RequestUniqueTags,
                                                                   CurrentUniqueTags,
                                                                   SessionDuration );

            _control.StartAction( );

            //_managedAccess.Callback = PacketCallBackFromReader;

            PerformanceCounter processorUtilizationCounter = null;

            try
            {
                processorUtilizationCounter = new PerformanceCounter( "Processor", "% Processor Time", "_Total", "." );
            }
            catch ( Exception ) { }

            //ReaderInterfaceThreadClass threadClass = new ReaderInterfaceThreadClass(_managedAccess, (int)ReaderHandle, TagAccessDataSet);//??把计??//Add LargeRead command
            ReaderInterfaceThreadClass threadClass = new ReaderInterfaceThreadClass(_managedAccess, (int)ReaderHandle, TagAccessDataSet, TagAccessReadSet);//??把计??//Add LargeRead command
            threadClass._reader = this; // HACK
            Thread runnerThread = new Thread(new ThreadStart(threadClass.AccessThreadProc));
            runnerThread.Name = "AccessTag";
            runnerThread.IsBackground = true;
            runnerThread.Priority = ThreadPriority.BelowNormal;
            runnerThread.Start( );

            int notused = FileHandler.TotalPacketCount; // Make sure the file is created;

            // Wait for thread to be in ready state
            int counter = 500;
            while ( counter-- > 0 && ( int ) ( runnerThread.ThreadState & System.Threading.ThreadState.WaitSleepJoin ) == 1 )
            {
                Thread.Sleep( 10 );
            }
            if ( !runnerThread.IsAlive )
            {
                return new rfidSimpleReport( context, OperationOutcome.FailByApplicationError, new ApplicationException( "BackgroundWorker thread could not be started." ) );
            }

            ReaderRequest readerRequest = new ReaderRequest( );
            readerRequest.reader = _theReaderID.Name;
#pragma warning disable 420 // reference to a volatile field is valid for Interlocked call
            readerRequest.requestSequence = System.Threading.Interlocked.Increment( ref _commonRequestIndex );
#pragma warning restore 420
            readerRequest.requestName = rfidReader.GetNameForRequest( rfidReader.ReaderRequestType.TagAccess );
            readerRequest.startTime = GetSessionRelativeDateTime( report.StartTimeMS );
            readerRequest.requestStartTime = report.StartTimeMS;
            readerRequest.startingPacketCount = RawPacketCount;
            readerRequest.startingTagCount = RawInventoryCount;

            MemoryStream data = new MemoryStream( );
            readerRequest.WriteTo( data );
            PacketData.PacketWrapper pseudoPacket = new PacketData.PacketWrapper( new PacketData.CommandPsuedoPacket( readerRequest.requestName, data.ToArray( ) ), PacketData.PacketType.U_N_D_F_I_N_E_D );
            pseudoPacket.IsPseudoPacket = true;
            pseudoPacket.ReaderName = _theReaderID.Name;
            pseudoPacket.ReaderIndex = ( int ) _theReaderID.Handle;
            FileHandler.WritePacket( pseudoPacket );

            //clark 2011.2.18 Record that user request data length
            TagAccessReqCount = r_iTagAccessReqCount;
            TagAccessReqCountRead = r_iTagAccessReqCountRead;//把计??

            DateTime ReportDue = DateTime.Now.AddMilliseconds( refreshMS );
            threadClass.StartEvent.Set( );

            while ( runnerThread.IsAlive )
            {
                CounterSample sample = CounterSample.Empty;
                if ( processorUtilizationCounter != null )
                {
                    try
                    {
                        sample = processorUtilizationCounter.NextSample( );
                    }
                    catch ( Exception ) { }
                }
                ProcessQueuedPackets( );

                QueueEvent.WaitOne( 30, true );
                QueueEvent.Reset( );

                DateTime now = DateTime.Now;

                if ( ReportDue.Ticks <= now.Ticks )
                {
                    //Debug.WriteLine(String.Format("Reporing Progress Now (Elapsed Milliseconds {0})", ElapsedMilliseconds));
                    _bgdWorker.ReportProgress(
                        0,
                        report.GetProgressReport( ElapsedMilliseconds,
                                                  RequestCount,
                                                  RawAntennaCycleCount,
                                                  RawInventoryCycleCount,
                                                  BadPacketCount,
                                                  CRCErrorCount,
                                                  RawPacketCount,
                                                  RawRoundCount,
                                                  RawInventoryCount,
                                                  SessionUniqueTags,
                                                  RequestUniqueTags,
                                                  CurrentUniqueTags,
                                                  SessionDuration ) );
                    _periodTagList.Clear( );
                    ReportDue = now.AddMilliseconds( refreshMS );
                }

                if ( !runnerThread.IsAlive )
                    break;

                if ( FunctionController.GetActionRequest( ) == FunctionControl.RequestedAction.Abort )
                {
                    threadClass.Stop = true;
                    Result abortError = Result.OK;
                    try
                    {
                        abortError = LakeChabotReader.MANAGED_ACCESS.API_ControlAbort();
                    }
                    catch ( Exception e )
                    {
                        Debug.WriteLine( String.Format( "Error attempting to abort: {0}", e.Message ) );
                    }
                    break;
                }

                if ( FunctionController.GetActionRequest( ) == FunctionControl.RequestedAction.Stop )
                {
                    threadClass.Stop = true;
                    Result stopError = Result.OK;
                    try
                    {
                        stopError = LakeChabotReader.MANAGED_ACCESS.API_ControlCancel();
                    }
                    catch ( Exception e )
                    {
                        Debug.WriteLine( String.Format( "Error attempting to stop: {0}", e.Message ) );
                    }
                    if ( Result.OK != stopError )
                    {
                        // Try to abort
                    }
                    break;
                }

                if ( FunctionController.State == FunctionControl.FunctionState.Paused )
                {
                    do
                    {
                        Thread.Sleep( refreshMS );
                    } while ( FunctionController.State == FunctionControl.FunctionState.Paused );
                }

                if ( !runnerThread.IsAlive )
                    break;

                float processorUtilization = 0;
                if ( processorUtilizationCounter != null && sample != CounterSample.Empty )
                {
                    try
                    {
                        CounterSample.Calculate( sample, processorUtilizationCounter.NextSample( ) );
                    }
                    catch ( Exception ) { }
                }
                // Debug.WriteLine(String.Format("Processor Util: {0}", processorUtilization));

                if ( processorUtilization > ( float ) TARGET_MAX_USAGE_PERCENT )
                    _packetSleepMS += SLEEP_ADD_SUBTRACT_MS;
                else
                    _packetSleepMS -= _packetSleepMS <= MIN_SLEEP_TIME_MS ? 0 : SLEEP_ADD_SUBTRACT_MS;
            }

            runnerThread.Join( );

            // Get any leftover packets
            ProcessQueuedPackets( );

            if ( LastCommandResult != 0 ) MacClearError( );

            OperationOutcome outcome = OperationOutcome.Success;
            string result = threadClass.Result;
            if ( !( result == null || result == "" || result == "OK" ) )
            {
                switch ( _control.State )
                {
                    case FunctionControl.FunctionState.Stopping:
                        outcome = OperationOutcome.SuccessWithUserCancel;
                        break;

                    case FunctionControl.FunctionState.Aborting:
                        outcome = OperationOutcome.FailByUserAbort;
                        break;

                    // case FunctionControl.FunctionState.Running:
                    //     outcome = OperationOutcome.FailByReaderError;
                    //     report.ErrorMessage = result;
                    //     break;

                    case FunctionControl.FunctionState.Idle:
                    case FunctionControl.FunctionState.Paused:
                    case FunctionControl.FunctionState.Unknown:
                    default:
                        outcome = OperationOutcome.Unknown;
                        break;
                }
            }

            _control.Finished( );

            report.OperationCompleted( outcome,
                                       ElapsedMilliseconds,
                                       RequestCount,
                                       RawAntennaCycleCount,
                                       RawInventoryCycleCount,
                                       BadPacketCount,
                                       CRCErrorCount,
                                       RawPacketCount,
                                       RawRoundCount,
                                       RawInventoryCount,
                                       SessionUniqueTags,
                                       RequestUniqueTags,
                                       CurrentUniqueTags,
                                       SessionDuration );

            // reset the mode
            ReaderOperationMode = priorMode;

            if ( EnableLogging )
            {
                //Push data from buffer to file. And clear buffer.
                PacketLogger.Flush(); 
            }

            return report;
        }

        public void UpdateInventoryStats( )
        {
            if ( RawAntennaCycleCount > _inventoryMatrix.Periods )
            {
                lock ( _tagInventoryData )
                {
                    foreach ( DatabaseRowTemplate row in TagInventoryData )
                    {
                        TagInventory tag = row as TagInventory;
                        if ( tag != null )
                        {
                            tag.averageReadsPerCycle = -1;
                            TagCycleEMA data = TagCycleEMA.Empty;
                            try
                            {
                                data = _inventoryMatrix.GetEMAReadsPerCycle( tag.TagIdData, RawAntennaCycleCount - 1 );

                                if ( data != TagCycleEMA.Empty && data.HasEMA )
                                {
                                    tag.averageReadsPerCycle = data.EMA.Value;
                                }
                                // PacketLogger.Comment(String.Format("TagCycleEMA {0}", data));
                            }
                            catch ( Exception e )
                            {
                                SysLogger.LogWarning( "Unable to get EMAReadesPerCycle: " + e.Message );
                                System.Diagnostics.Debug.Assert( false, e.Message );
                            }
                        }
                        // tag.sumReadDelta = 0xb0b;
                    }
                }
            }
        }






        public UInt32 ReadRegister( UInt16 address, out string errorMessage )
        {
            Result result = Result.OK;
            UInt32 val = 0;

            if ( Mode == rfidReader.OperationMode.BoundToReader )
            {
                if (Result.OK == (result = LakeChabotReader.MANAGED_ACCESS.API_ConfigReadRegister
                    (  address, ref val ) ) )
                {
                    errorMessage = null;
                }
                else
                {
                    errorMessage = "general : failure";
                }
            }
            else
            {
                result       = Result.NOT_INITIALIZED;
                errorMessage = "unbound : failure";
            }
            return val;
        }

        public UInt32 WriteRegister( UInt16 address, UInt32 value, out string errorMessage )
        {
            Result result = Result.OK;

            if ( Mode == rfidReader.OperationMode.BoundToReader )
            {
                if (Result.OK == (result = LakeChabotReader.MANAGED_ACCESS.API_ConfigWriteRegister
                    (  address, value ) ) )
                {
                    errorMessage = null;
                }
                else
                {
                    errorMessage = "general : failure";
                }
            }
            else
            {
                errorMessage = "unbound : failure";
                result = Result.NOT_INITIALIZED;
            }
            return ( UInt32 ) result;
        }

        public static void AssemblyClosing( )
        {
            LakeChabotReader.MANAGED_ACCESS.API_Shutdown();
        }

        #region IDisposable Members

        public void Dispose( )
        {
            Dispose( true );
            // Take yourself off the Finalization queue to prevent finalization code for this object from executing a second time.
            GC.SuppressFinalize( this );
        }

        #endregion

        /// <summary>
        /// Dispose(bool disposing) executes in two distinct scenarios.
        /// If disposing equals true, the method has been called directly
        /// or indirectly by a user's code. Managed and unmanaged resources
        /// can be disposed.
        /// If disposing equals false, the method has been called by the 
        /// runtime from inside the finalizer and you should not reference 
        /// other objects. Only unmanaged resources can be disposed.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose( bool disposing )
        {
            // Check to see if Dispose has already been called.
            if ( Interlocked.CompareExchange( ref _isDisposed, 1, 0 ) == 0 )
            {
                // If disposing equals true, dispose all managed and unmanaged resources.
                if ( disposing )
                {
                    // Dispose managed resources.
                    PacketLogger.Clear( );

                    //if ( _managedAccess != null ) _managedAccess.Dispose( );

                    if ( _fileHandler != null ) _fileHandler.Dispose( );

                    if ( _propertyBagData != null ) _propertyBagData.Dispose( );
                    if ( _antennaCycleData != null ) _antennaCycleData.Dispose( );
                    if ( _inventoryRoundData != null ) _inventoryRoundData.Dispose( );
                    if ( _packetStreamData != null ) _packetStreamData.Dispose( );
                    if ( _readerCommandData != null ) _readerCommandData.Dispose( );
                    if ( _readerAntennaCycleData != null ) _readerAntennaCycleData.Dispose( );
                    if ( _readerRequestData != null ) _readerRequestData.Dispose( );
                    if ( _tagInventoryData != null ) _tagInventoryData.Dispose( );
                    if ( _tagReadData != null ) _tagReadData.Dispose( );
                    if ( _readRateData != null ) _readRateData.Dispose( );
                    if ( _badPacketData != null ) _badPacketData.Dispose( );
                    if ( _inventoryCycleData != null ) _inventoryCycleData.Dispose( );

                    if ( _staticReaderDir != null && Directory.Exists( _staticReaderDir ) )
                        Directory.Delete( _staticReaderDir );
                }
                // Release unmanaged resources. If disposing is false, only the following code is executed.
                // nothing yet...
            }
        }

        public DictionaryEntry[ ] DataSources
        {
            get
            {
                return new DictionaryEntry[ ]
                {
                    new DictionaryEntry("PropertyBag",        PropertyBagData),
                    new DictionaryEntry("ReaderRequest",      ReaderRequestData),
                    new DictionaryEntry("PacketStream",       PacketStreamData),
                    new DictionaryEntry("ReaderCommand",      ReaderCommandData),
                    new DictionaryEntry("ReaderAntennaCycle", ReaderAntennaCycleData),
                    new DictionaryEntry("AntennaPacket",      AntennaCycleData),
                    new DictionaryEntry("InventoryCycle",     InventoryCycleData),
                    new DictionaryEntry("InventoryRound",     InventoryRoundData),
                    new DictionaryEntry("TagRead",            TagReadData),
                    new DictionaryEntry("TagInventory",       TagInventoryData),
                    new DictionaryEntry("BadPacket",          BadPacketData),
                    new DictionaryEntry("ReadRate",           ReadRateData),
                };
            }
        }


        public rfid.Constants.Result MacReadRegister(ushort address, out uint data)
        {
            data = 0;
            return LakeChabotReader.MANAGED_ACCESS.API_ConfigReadRegister(address, ref data);
        }

        //******//
        public rfid.Constants.Result MacReadRegisterGetTempature(ushort address, out int data)
        {
            data = 0;
            return LakeChabotReader.MANAGED_ACCESS.API_ConfigReadRegisterGetTemperature(address, ref data);
        }
        //******//

        public rfid.Constants.Result MacReadBankedRegister(ushort address, ushort bank, out uint data)
        {
            data = 0;
            return LakeChabotReader.MANAGED_ACCESS.API_ConfigReadBankedRegister(address, bank, ref data);
        }

        public rfid.Constants.Result MacWriteRegister(UInt16 address, uint data)
        {
            return LakeChabotReader.MANAGED_ACCESS.API_ConfigWriteRegister(address, data);
        }

        public rfid.Constants.Result MacWriteBankedRegister(ushort address, ushort bank, uint data)
        {
            return LakeChabotReader.MANAGED_ACCESS.API_ConfigWriteBankedRegister(address, bank, data);
        }

        public rfid.Constants.Result MacReadRegisterInfo(ushort address, out RegisterInfo info)
        {            
            return LakeChabotReader.MANAGED_ACCESS.API_ConfigReadRegisterInfo(address, out info);
        }

        public rfid.Constants.Result MacBypassReadRegister(ushort address, out ushort data)
        {
            data = 0;
            return LakeChabotReader.MANAGED_ACCESS.API_MacBypassReadRegister(address, ref data);
        }

        public rfid.Constants.Result MacBypassWriteRegister(ushort address, ushort data)
        {
            return LakeChabotReader.MANAGED_ACCESS.API_MacBypassWriteRegister(address, data);
        }

        public rfid.Constants.Result MacReadOemData(ushort address, ref uint Data)
        {
            return LakeChabotReader.MANAGED_ACCESS.API_MacReadOemData(address, ref Data);
        }

        public rfid.Constants.Result MacWriteOemData(ushort address, uint Data)
        {
            return LakeChabotReader.MANAGED_ACCESS.API_MacWriteOemData(address, Data);
        }


        public rfid.Structures.LibraryVersion LibraryVersion
        {
            // This looks strange but since the radio detection is static
            // and performed at start-up, we just grap that lib version...
            get
            {
                return LakeChabotReader.LIBRARY_VERSION;
            }
        }

        rfid.Structures.FirmwareVersion firmwareVersion
            = new rfid.Structures.FirmwareVersion();

        public rfid.Structures.FirmwareVersion FirmwareVersion
        {
            get
            {
                return this.firmwareVersion;
            }
        }


        rfid.Structures.Version hardwareVersion
            = new rfid.Structures.Version( );

        public rfid.Structures.Version HardwareVersion
        {
            get
            {
                return this.hardwareVersion;
            }
        }


        rfid.Structures.OEMCfgVersion OemCfgVersion
            = new rfid.Structures.OEMCfgVersion();

        public rfid.Structures.OEMCfgVersion OEMCfgVersion
        {
            get
            {
                return this.OemCfgVersion;
            }
        }


        rfid.Structures.OEMCfgUpdateNumber OemCfgUpdateNumber
            = new rfid.Structures.OEMCfgUpdateNumber();

        public rfid.Structures.OEMCfgUpdateNumber OEMCfgUpdateNumber
        {
            get
            {
                return this.OemCfgUpdateNumber;
            }
        }



        rfid.Structures.MacBootLoaderVersion bootLoaderVersion
             = new rfid.Structures.MacBootLoaderVersion();

        public rfid.Structures.MacBootLoaderVersion BootLoaderVersion
        {
            get
            {
                return this.bootLoaderVersion;
            }
        }





        //clark 2011.5.10 Doesn't recommand to get data from OEM directly.
        //public UInt32 FirmwareErrorCode
        //{
        //    get
        //    {
        //        string error = null;
        //        UInt32 ec = ReadRegister( RegisterData.MAC_REG_ERROR, out error );
        //        if ( !String.IsNullOrEmpty( error ) )
        //        {
        //            throw new rfidReaderException( error );
        //        }
        //        return ec;
        //    }
        //}

        //public UInt32 LastFirmwareErrorCode
        //{
        //    get
        //    {
        //        string error = null;
        //        UInt32 ec = ReadRegister(RegisterData.MAC_REG_LAST_ERROR, out error);
        //        if (!String.IsNullOrEmpty(error))
        //        {
        //            throw new rfidReaderException(error);
        //        }
        //        return ec;
        //    }
        //}


        public UInt32 OEMAreaCode
        {
            get
            {
                string error = null;
                UInt32 ac = ReadRegister(RegisterData.MAC_REG_MACINFO, out error);
                if ( !String.IsNullOrEmpty( error ) )
                {
                    throw new rfidReaderException( error );
                }
                return ac;
            }
        }

        //clark 2011.4.26 cancel that read region from enum
        public MacRegion RegulatoryRegion
        {
            get
            {
                MacRegion actualRegion  = MacRegion.UNKNOWN;
                UInt32    regionSupport = 0;

                Result Result = LakeChabotReader.MANAGED_ACCESS.API_MacGetRegion( ref actualRegion,
                                                                                  ref regionSupport );

                if (Result.OK != Result)
                {
                    throw new rfidReaderException(Result.ToString());
                }

                return actualRegion;
            }
            set
            {
            }

        }

        public RadioOperationMode ReaderOperationMode
        {
            get
            {
                RadioOperationMode opMode = RadioOperationMode.UNKNOWN;

                Result Result = LakeChabotReader.MANAGED_ACCESS.API_ConfigGetOperationMode(ref opMode);
                if (Result.OK != Result)
                {
                    throw new rfidReaderException(Result.ToString());
                }
                return opMode;
            }
            set
            {
                if ( RadioOperationMode.UNKNOWN == value )
                {
                    throw new ArgumentOutOfRangeException( value.ToString( ) );
                }

                Result Result = LakeChabotReader.MANAGED_ACCESS.API_ConfigSetOperationMode(value);

                if ( rfid.Constants.Result.OK != Result )
                {
                    throw new rfidReaderException( Result.ToString( ) );
                }
            }
        }



        public RadioPowerState PowerState
        {
            get
            {
                RadioPowerState powerState = RadioPowerState.UNKNOWN;

                Result Result = LakeChabotReader.MANAGED_ACCESS.API_ControlGetPowerState(ref powerState);

                if ( rfid.Constants.Result.OK != Result )
                {
                    throw new rfidReaderException( Result.ToString( ) );
                }
                return powerState;
            }
            set
            {
                if ( RadioPowerState.UNKNOWN == value )
                {
                    throw new ArgumentOutOfRangeException( value.ToString( ) );
                }

                Result Result = LakeChabotReader.MANAGED_ACCESS.API_ControlSetPowerState(value);

                if ( rfid.Constants.Result.OK != Result )
                {
                    throw new rfidReaderException( Result.ToString( ) );
                }
            }
        }


        //Settint Com Port Number from Library
        static public uint uiLibSettingComPort
        {
            get
            {
                return LakeChabotReader.MANAGED_ACCESS.uiLibSettingComPort;
            }

            set
            {
                LakeChabotReader.MANAGED_ACCESS.uiLibSettingComPort = value;
            }
        }


        /*
        public CarrierWaveValue CarrierWave
        {
            get
            {
                bool waveIsOn;
                String error = _managedAccess.GetCarrierWaveState((int)( int ) readerHandle, out waveIsOn);
                if (!String.IsNullOrEmpty(error))
                {
                    throw new rfidReaderException(error);
                }
                return waveIsOn ? CarrierWaveValue.On : CarrierWaveValue.Off;
            }

            set
            {
                if (value == CarrierWaveValue.Unknown) throw new ArgumentOutOfRangeException("value", String.Format("CarrierWaveValue can not be {0}", value));

                string error = _managedAccess.SetCarrierWaveState((int)( int ) readerHandle, value == CarrierWaveValue.On);
                if (!String.IsNullOrEmpty(error))
                {
                    throw new rfidReaderException(error);
                }
            }
        }
*/

        public DataFile<PropertyBag> PropertyBagData
        {
            get { return _propertyBagData; }
        }

        public SequentialDataFile<PacketStream> PacketStreamData
        {
            get { return _packetStreamData; }
        }

        public DataFile<TagInventory> TagInventoryData
        {
            get { return _tagInventoryData; }
        }

        public SequentialDataFile<ReaderRequest> ReaderRequestData
        {
            get { return _readerRequestData; }
        }

        public SequentialDataFile<ReaderCommand> ReaderCommandData
        {
            get { return _readerCommandData; }
        }

        public SequentialDataFile<ReaderAntennaCycle> ReaderAntennaCycleData
        {
            get { return _readerAntennaCycleData; }
        }

        public SequentialDataFile<AntennaPacket> AntennaCycleData
        {
            get { return _antennaCycleData; }
        }

        public SequentialDataFile<InventoryCycle> InventoryCycleData
        {
            get { return _inventoryCycleData; }
        }

        public SequentialDataFile<InventoryRound> InventoryRoundData
        {
            get { return _inventoryRoundData; }
        }

        public SequentialDataFile<TagRead> TagReadData
        {
            get { return _tagReadData; }
        }

        public SequentialDataFile<ReadRate> ReadRateData
        {
            get { return _readRateData; }
        }

        public SequentialDataFile<BadPacket> BadPacketData
        {
            get { return _badPacketData; }
        }

        private Result InitReader( )
        {
            this._control = new FunctionControl( FunctionControl.SupportedActions.StopAbortPause );

            return Result.OK;
        }

        private Source_AntennaList antennaList;

        private Result initAntennaList( )
        {
            this.antennaList = new Source_AntennaList( );

            return Result.OK;
        }

        public Source_AntennaList AntennaList
        {
            get
            {
                return this.antennaList;
            }

            set
            {
                this.antennaList = value;
            }
        }

        private static void WaitForFileReady( string newFilePath )
        {
            bool goodOpen = false;
            int totalTries = 100;
            while ( !goodOpen )
            {
                try
                {
                    FileStream s = File.OpenRead( newFilePath );
                    goodOpen = s.Length > 0;
                    s.Close( );
                    s.Dispose( );
                }
                catch ( Exception )
                {
                    if ( totalTries-- == 0 )
                        throw;
                    Thread.Sleep( 50 );
                }
            }
        }

        public Int32 MyCallback
        (
            UInt32 bufferLength,
            IntPtr pBuffer,
            IntPtr context
        )
        {
            Byte[ ] packetBuffer = new Byte[ bufferLength ];

            Marshal.Copy( pBuffer, packetBuffer, 0, ( Int32 ) bufferLength );

            String errorMessage = null;
            PacketData.PacketBase packet = null;

            PacketData.PacketType type = PacketData.ParsePacket
            (
                packetBuffer,
                out packet,
                out errorMessage
            );

            SysLogger.WriteToLog = true;

            PacketData.PacketWrapper envelope = null;
            long elapsedSesionTime = ElapsedMilliseconds;

            if ( packet == null || type == PacketData.PacketType.U_N_D_F_I_N_E_D || errorMessage != null )
            {
                BadPacket badPacket = new BadPacket( );
#pragma warning disable 420 // reference to a volatile field is valid for Interlocked call
                badPacket.badPacketSequence = System.Threading.Interlocked.Increment( ref _commonBadIndex );
#pragma warning restore 420
                badPacket.packetTime = DateTime.UtcNow;

                badPacket.rawPacketData = packetBuffer.Clone( ) as byte[ ];
                badPacket.errorMessage = errorMessage;

                using ( MemoryStream data = new MemoryStream( 256 ) )
                {
                    badPacket.WriteTo( data );
                    envelope = new PacketData.PacketWrapper( new PacketData.CommandPsuedoPacket( "BadPacket", data.ToArray( ) ), PacketData.PacketType.U_N_D_F_I_N_E_D );
                }

                envelope.IsPseudoPacket = true;
                envelope.PacketNumber = badPacket.PacketSequence;
                envelope.Timestamp = badPacket.packetTime;
                envelope.ReaderIndex = ( int ) _theReaderID.Handle;
                envelope.ReaderName = _theReaderID.Name;
                envelope.CommandNumber = _processedInventoryIndex;
                envelope.ElapsedTimeMs = elapsedSesionTime;
            }
            else
            {
                envelope = new PacketData.PacketWrapper
                (
                    packet,
                    type,
                    packetBuffer,
                    _commonRequestIndex,
                    elapsedSesionTime,
                    ( int ) this.ReaderHandle,
                    Name
                );
                Debug.Assert( envelope.PacketType == type );
            }

            if ( VirtualReaderQueue != null )
            {
                lock ( VirtualReaderQueue )
                {
                    VirtualReaderQueue.Enqueue( envelope );
                }
            }

            lock ( PacketQueue )
            {
                PacketQueue.Enqueue( envelope );
            }

#pragma warning disable 420 // reference to a volatile field is valid for Interlocked call
            int queueSize = Interlocked.Increment( ref _queueCount );
#pragma warning restore 420

           
            _maxQueueSize = queueSize > _maxQueueSize ? queueSize : _maxQueueSize;
            if (queueSize > MAX_QUEUE_SIZE)
            {
                int loopCount = 0;
                while (queueSize > TARGET_QUEUE_SIZE && loopCount < 1000)
                {
                    loopCount++;
                    QueueEvent.Set();

                    Thread.Sleep(QUEUE_SLEEP_MS);
                    queueSize = _queueCount;
                }
                // Write an informational entry to the event log.    
                SysLogger.LogMessage(String.Format("Queue Size = {0}\nMax Queue Size = {1}\nSleep Count = {2}\nPacket Count = {3}", queueSize, _maxQueueSize, loopCount, ProcessedPacketCount));
            }
            return FunctionController.GetActionRequest( ) != FunctionControl.RequestedAction.Abort ? 0 : 1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="PacketBuffer"></param>

        private bool PacketCallBackFromReader( int readerIndex, Byte[ ] PacketBuffer )
        {
            uint myHandle = this._theReaderID._handle;
            Debug.Assert( readerIndex == myHandle );

            String errorMessage = null;
            PacketData.PacketBase packet = null;

            //!! TODO: fix output SysLogger.WriteToLog = ation != Library;

            PacketData.PacketType type = PacketData.ParsePacket( PacketBuffer, out packet, out errorMessage );
            SysLogger.WriteToLog = true;

            PacketData.PacketWrapper envelope = null;
            long elapsedSesionTime = ElapsedMilliseconds;

            if ( packet == null || type == PacketData.PacketType.U_N_D_F_I_N_E_D || errorMessage != null )
            {
                BadPacket badPacket = new BadPacket( );
#pragma warning disable 420 // reference to a volatile field is valid for Interlocked call
                badPacket.badPacketSequence = System.Threading.Interlocked.Increment( ref _commonBadIndex );
#pragma warning restore 420
                badPacket.packetTime = DateTime.UtcNow;

                badPacket.rawPacketData = PacketBuffer.Clone( ) as byte[ ];
                badPacket.errorMessage = errorMessage;
                using ( MemoryStream data = new MemoryStream( 256 ) )
                {
                    badPacket.WriteTo( data );
                    envelope = new PacketData.PacketWrapper( new PacketData.CommandPsuedoPacket( "BadPacket", data.ToArray( ) ), PacketData.PacketType.U_N_D_F_I_N_E_D );
                }
                envelope.IsPseudoPacket = true;
                envelope.PacketNumber = badPacket.PacketSequence;
                envelope.Timestamp = badPacket.packetTime;
                envelope.ReaderIndex = ( int ) _theReaderID.Handle;
                envelope.ReaderName = _theReaderID.Name;
                envelope.CommandNumber = _processedInventoryIndex;
                envelope.ElapsedTimeMs = elapsedSesionTime;
            }
            else
            {
                envelope = new PacketData.PacketWrapper( packet, type, PacketBuffer, _commonRequestIndex, elapsedSesionTime, readerIndex, Name );
                Debug.Assert( envelope.PacketType == type );
            }

            lock ( PacketQueue )
            {
                PacketQueue.Enqueue( envelope );
            }

#pragma warning disable 420 // reference to a volatile field is valid for Interlocked call
            int queueSize = Interlocked.Increment( ref _queueCount );
#pragma warning restore 420

            _maxQueueSize = queueSize > _maxQueueSize ? queueSize : _maxQueueSize;
            //if ( queueSize > MAX_QUEUE_SIZE )
            //{
            //    int loopCount = 0;
            //    while ( queueSize > TARGET_QUEUE_SIZE && loopCount < 1000 )
            //    {
            //        loopCount++;
            //        QueueEvent.Set( );

            //        Thread.Sleep( QUEUE_SLEEP_MS );
            //        queueSize = _queueCount;
            //    }
            //    // Write an informational entry to the event log.    
            //    SysLogger.LogMessage( String.Format( "Queue Size = {0}\nMax Queue Size = {1}\nSleep Count = {2}\nPacket Count = {3}", queueSize, _maxQueueSize, loopCount, ProcessedPacketCount ) );
            //}
            return FunctionController.GetActionRequest( ) != FunctionControl.RequestedAction.Abort;
        }

        /// <summary>
        /// 
        /// </summary>
        private int ProcessQueuedPackets( )
        {
            #region For debugging thread problems
#if false 
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("[0x{0:x}]{1}()\n", Thread.CurrentThread.ManagedThreadId, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            foreach (ProcessThread pt in Process.GetCurrentProcess().Threads)
            {
                sb.AppendFormat("[0x{0:x}] State: {1}\n", pt.Id, pt.ThreadState.ToString());
            }
            Debug.WriteLine(sb.ToString());
            Debug.Flush();
#endif
            #endregion

            int cnt = 0;
            while ( cnt < 1000 )
            {
                PacketData.PacketWrapper envelope = null;
                lock ( PacketQueue )
                {
                    if ( PacketQueue.Count > 0 )
                        envelope = PacketQueue.Dequeue( );
                }
                if ( envelope == null )
                    break;

#pragma warning disable 420 // reference to a volatile field is valid for Interlocked call
                Interlocked.Decrement( ref _queueCount );
#pragma warning restore 420

#pragma warning disable 420 // reference to a volatile field is valid for Interlocked call
                envelope.PacketNumber = System.Threading.Interlocked.Increment( ref _rawPacketCount ) - 1;
#pragma warning restore 420

                // if (PacketArrival != null)
                // {
                //     PacketArrival(this, new PacketArrivalEventArgs(envelope));
                // }
    
                SavePacket( envelope );
                cnt++;
            }
            return cnt;
        }

        /// <summary>
        /// Caculate basic stats and save the packet to a stream
        /// </summary>
        /// <param name="envelope"></param>
        private void SavePacket( PacketData.PacketWrapper envelope )
        {
            // Debug.WriteLineIf((int)envelope.RadioID != (int)_theReaderID.Handle, String.Format("Expected: {0}, Actual: {1}", (int)_theReaderID.Handle, (int)envelope.RadioID), "ERROR");
            Debug.Assert( envelope.ReaderName == Name );

            PacketData.PacketBase packet = envelope.Packet; 
            PacketData.PacketType type = envelope.PacketType;

            switch ( type )
            {
                case PacketData.PacketType.CMD_BEGIN:
                    {
                        LastUsedAntenna = null;
                        LastCommandResult = null;
                        PacketData.cmd_beg cmd_beg = packet as PacketData.cmd_beg;
                        Debug.Assert( cmd_beg != null );
#pragma warning disable 420 // reference to a volatile field is valid for Interlocked call
                        System.Threading.Interlocked.Increment( ref _rawCommandCount );
#pragma warning restore 420
                    }
                    break;

                case PacketData.PacketType.CMD_END:
                    {
                        PacketData.cmd_end cmd_end = packet as PacketData.cmd_end;
                        Debug.Assert( cmd_end != null );
                        LastCommandResult = cmd_end.Result;
                        if ( cmd_end.Result != 0 ) CommandErrors++;
                    }
                    break;

                case PacketData.PacketType.ANTENNA_CYCLE_BEGIN:
                    {
                        PacketData.ant_cyc_beg cyc_beg = packet as PacketData.ant_cyc_beg;
                        Debug.Assert( cyc_beg != null );

#pragma warning disable 420 // reference to a volatile field is valid for Interlocked call
                        System.Threading.Interlocked.Increment( ref _rawAntennaCycleCount );
#pragma warning restore 420

                        // PacketLogger.Comment(String.Format("Start of Cycle {0} Command={1}", RawCycleCount - 1, RawCommandCount - 1));

                        lock ( _tagInventoryData )
                        {
                            foreach ( TagInventory ti in TagInventoryData )
                            {
                                ti.cycleReadCount = 0;
                                ti.totalCycleCount = RawAntennaCycleCount;
                            }
                        }
                    }
                    break;

                case PacketData.PacketType.ANTENNA_CYCLE_BEGIN_DIAG:
                    {
                        PacketData.ant_cyc_beg_diag cyc_beg_diag = packet as PacketData.ant_cyc_beg_diag;
                        Debug.Assert( cyc_beg_diag != null );
                    }
                    break;

                case PacketData.PacketType.ANTENNA_CYCLE_END:
                    {
                        PacketData.ant_cyc_end cyc_end = packet as PacketData.ant_cyc_end;
                        Debug.Assert( cyc_end != null );

                        // PacketLogger.Comment(String.Format("End of Cycle {0}", RawCycleCount - 1)); 
                    }
                    break;

                case PacketData.PacketType.ANTENNA_CYCLE_END_DIAG:
                    {
                        PacketData.ant_cyc_end_diag cyc_end_diag = packet as PacketData.ant_cyc_end_diag;
                        Debug.Assert( cyc_end_diag != null );
                    }
                    break;

                case PacketData.PacketType.ANTENNA_BEGIN:
                    {
                        PacketData.ant_beg ant_beg = packet as PacketData.ant_beg;
                        Debug.Assert( ant_beg != null );

#pragma warning disable 420 // reference to a volatile field is valid for Interlocked call
                        System.Threading.Interlocked.Increment( ref _rawAntennaCount );
#pragma warning restore 420

                        LastUsedAntenna = ant_beg.antenna;
                    }
                    break;

                case PacketData.PacketType.ANTENNA_END:
                    {
                        PacketData.ant_end ant_end = packet as PacketData.ant_end;
                        Debug.Assert( ant_end != null );
                        LastUsedAntenna = null;
                    }
                    break;

                case PacketData.PacketType.ANTENNA_BEGIN_DIAG:
                    {
                        PacketData.ant_beg_diag ant_beg_diag = packet as PacketData.ant_beg_diag;
                        Debug.Assert( ant_beg_diag != null );
                    }
                    break;

                case PacketData.PacketType.ANTENNA_END_DIAG:
                    {
                        PacketData.ant_end_diag ant_end_diag = packet as PacketData.ant_end_diag;
                        Debug.Assert( ant_end_diag != null );
                    }
                    break;

                case PacketData.PacketType.INVENTORY_CYCLE_BEGIN:
                    {
                        PacketData.inv_cyc_beg inv_cyc_beg = packet as PacketData.inv_cyc_beg;
                        Debug.Assert( inv_cyc_beg != null );

#pragma warning disable 420 // reference to a volatile field is valid for Interlocked call
                        System.Threading.Interlocked.Increment( ref _rawInventoryCycleCount );
#pragma warning restore 420

                        // PacketLogger.Comment(String.Format("Start of Cycle {0} Command={1}", RawCycleCount - 1, RawCommandCount - 1));
                    }
                    break;

                case PacketData.PacketType.INVENTORY_CYCLE_END:
                    {
                        PacketData.inv_cyc_end inv_cyc_end = packet as PacketData.inv_cyc_end;
                        Debug.Assert( inv_cyc_end != null );
                    }
                    break;

                case PacketData.PacketType.CARRIER_INFO:
                    {
                        PacketData.carrier_info carrier_info = packet as PacketData.carrier_info;
                        Debug.Assert( carrier_info != null );
                    }
                    break;

                case PacketData.PacketType.INVENTORY_CYCLE_END_DIAGS:
                    {
                        PacketData.inv_cyc_end_diag inv_cyc_end_diag = packet as PacketData.inv_cyc_end_diag;
                        Debug.Assert( inv_cyc_end_diag != null );
                    }
                    break;

                case PacketData.PacketType.ISO18K6C_INVENTORY_ROUND_BEGIN:
                    {
                        PacketData.inv_rnd_beg inv_rnd_beg = packet as PacketData.inv_rnd_beg;
                        Debug.Assert( inv_rnd_beg != null );

#pragma warning disable 420 // reference to a volatile field is valid for Interlocked call
                        System.Threading.Interlocked.Increment( ref _rawRoundCount );
#pragma warning restore 420 // reference to a volatile field is valid for Interlocked call
                    }
                    break;

                case PacketData.PacketType.ISO18K6C_INVENTORY_ROUND_BEGIN_DIAG:
                    {
                        PacketData.inv_rnd_beg_diag inv_rnd_beg_diag = packet as PacketData.inv_rnd_beg_diag;
                        Debug.Assert( inv_rnd_beg_diag != null );
                    }
                    break;

                case PacketData.PacketType.ISO18K6C_INVENTORY_ROUND_END_DIAG:
                    {
                        PacketData.inv_rnd_end_diag inv_rnd_end_diag = packet as PacketData.inv_rnd_end_diag;
                        Debug.Assert( inv_rnd_end_diag != null );
                    }
                    break;

                case PacketData.PacketType.ISO18K6C_INVENTORY:
                    {
                        PacketData.inventory epc = packet as PacketData.inventory;
                        Debug.Assert( epc != null );

                        BitVector32 flags = new BitVector32( epc.flags );
                        bool badCrc = flags[ PacketData.PacketBase.crcResult ] != 0;
                        if ( badCrc )
                        {
#pragma warning disable 420 // reference to a volatile field is valid for Interlocked call
                            System.Threading.Interlocked.Increment( ref _cRCErrors );
#pragma warning restore 420
                        }
                        else
                        {
#pragma warning disable 420 // reference to a volatile field is valid for Interlocked call
                            System.Threading.Interlocked.Increment( ref _rawInventoryCount );
#pragma warning restore 420

                            rfidTag key = new rfidTag( epc.inventory_data );
                            if ( _sessionTagList.AddOrIncrementTagCount( key ) == 1 )
                            {
                                _requestTagList.Add( key, 1 );
                                _periodTagList.Add( key, 1 );
                            }
                            else
                            {
                                if ( _requestTagList.AddOrIncrementTagCount( key ) == 1 )
                                {
                                    _periodTagList.Add( key, 1 );
                                }
                                else
                                {
                                    _periodTagList.AddOrIncrementTagCount( key );
                                }
                            }

                            string epcData = BitConverter.ToString( epc.inventory_data, 0, epc.inventory_data.Length - flags[ PacketData.PacketBase.paddingBytes ] );
                            int CycleSlotNumber = RawAntennaCycleCount == 0 ? 0 : RawAntennaCycleCount - 1;

                            KeyList<TagInventory> epcDataKey = new KeyList<TagInventory>( "tagIdData", epcData );
                            _inventoryMatrix.AddTagCycle( epcData, CycleSlotNumber );

                            TagInventory tagInventory = new TagInventory( );

                            lock ( _tagInventoryData )
                            {
                                LastUsedAntenna = epc.logic_ant;

                                if ( !TagInventoryData.Contains( epcDataKey ) ) // new tag ID
                                {
                                    // PacketLogger.Comment(String.Format("First Read of Tag {0} Command={1}, Cycle={2}", epcData, RawCommandCount - 1, RawCycleCount - 1));

                                    tagInventory.tagIdData         = epcData;
                                    tagInventory.readCount         = 1;
                                    tagInventory.cycleReadCount    = 1;
                                    tagInventory.firstReadTime     = epc.ms_ctr;
                                    tagInventory.lastReadTime      = epc.ms_ctr;
                                    tagInventory.firstCommand      = RawCommandCount - 1;
                                    tagInventory.lastCommand       = RawCommandCount - 1;
                                    tagInventory.actualCycleCount  = 1;
                                    tagInventory.totalCycleCount   = RawAntennaCycleCount;
                                    tagInventory.cycleReadCount    = 1;
                                    tagInventory.sumOfCommandReads = 0;
                                    //tagInventory.readRate = 0;
                                    //tagInventory.readerCycleCount;
                                    //tagInventory.firstReaderCycle;
                                    //tagInventory.lastReaderCycle;
                                    //tagInventory.antennaCycleCount;
                                    //tagInventory.firstAntennaCycle;
                                    //tagInventory.lastAntennaCycle;
                                    tagInventory.port0reads = 0;
                                    tagInventory.port1reads = 0;
                                    tagInventory.port2reads = 0;
                                    tagInventory.port3reads = 0;
                                    tagInventory.port4reads = 0;
                                    tagInventory.port5reads = 0;
                                    tagInventory.port6reads = 0;
                                    tagInventory.port7reads = 0;
                                    tagInventory.port8reads = 0;
                                    tagInventory.port9reads = 0;
                                    tagInventory.port10reads = 0;
                                    tagInventory.port11reads = 0;
                                    tagInventory.port12reads = 0;
                                    tagInventory.port13reads = 0;
                                    tagInventory.port14reads = 0;
                                    tagInventory.port15reads = 0;

                                    

                                    if ( LastUsedAntenna != null )
                                    {
                                        switch ( LastUsedAntenna )
                                        {
                                            case 0:
                                                tagInventory.port0reads = 1;
                                                break;

                                            case 1:
                                                tagInventory.port1reads = 1;
                                                break;

                                            case 2:
                                                tagInventory.port2reads = 1;
                                                break;

                                            case 3:
                                                tagInventory.port3reads = 1;
                                                break;

                                            case 4:
                                                tagInventory.port4reads = 1;
                                                break;

                                            case 5:
                                                tagInventory.port5reads = 1;
                                                break;

                                            case 6:
                                                tagInventory.port6reads = 1;
                                                break;

                                            case 7:
                                                tagInventory.port7reads = 1;
                                                break;

                                            case 8:
                                                tagInventory.port8reads = 1;

                                                break;
                                            case 9:
                                                tagInventory.port9reads = 1;
                                                break;

                                            case 10:
                                                tagInventory.port10reads = 1;
                                                break;

                                            case 11:
                                                tagInventory.port11reads = 1;
                                                break;

                                            case 12:
                                                tagInventory.port12reads = 1;
                                                break;

                                            case 13:
                                                tagInventory.port13reads = 1;
                                                break;

                                            case 14:
                                                tagInventory.port14reads = 1;
                                                break;

                                            case 15:
                                                tagInventory.port15reads = 1;
                                                break;

                                            default:
                                                break;
                                        }
                                    }
                                    // tagInventory.inventoryRoundCount;
                                    // tagInventory.firstinventoryRound;
                                    // tagInventory.lastinventoryRound;
                                    TagInventoryData.Add( tagInventory );
                                }
                                else
                                {
                                    tagInventory = ( TagInventory ) TagInventoryData[ epcDataKey ];

                                    tagInventory.readCount++;
                                    if ( tagInventory.lastCommand == CycleSlotNumber )
                                    {
                                        tagInventory.cycleReadCount += 1;
                                    }
                                    else
                                    {
                                        tagInventory.cycleReadCount = 1;
                                        tagInventory.actualCycleCount++;
                                        tagInventory.lastCommand = CycleSlotNumber;
                                    }

                                    tagInventory.totalCycleCount = RawAntennaCycleCount;

                                    tagInventory.lastReadTime = epc.ms_ctr;
                                    // if (tagInventory.lastReadTime > tagInventory.firstReadTime)
                                    //{
                                    //    tagInventory.readRate = (float)(tagInventory.ReadCount) / (float)((tagInventory.LastReadTime - tagInventory.FirstReadTime) / 100);
                                    //}

                                    //tagInventory.commandCount = 1;
                                    //tagInventory.firstCommand;
                                    //tagInventory.lastCommand;
                                    //tagInventory.readerCycleCount;
                                    //tagInventory.firstReaderCycle;
                                    //tagInventory.lastReaderCycle;
                                    //tagInventory.antennaCycleCount;
                                    //tagInventory.firstAntennaCycle;
                                    //tagInventory.lastAntennaCycle;
                                    if ( LastUsedAntenna != null )
                                    {
                                        switch ( LastUsedAntenna )
                                        {
                                            case 0:
                                                tagInventory.port0reads += 1;
                                                break;

                                            case 1:
                                                tagInventory.port1reads += 1;
                                                break;

                                            case 2:
                                                tagInventory.port2reads += 1;
                                                break;

                                            case 3:
                                                tagInventory.port3reads += 1;
                                                break;

                                            case 4:
                                                tagInventory.port4reads += 1;
                                                break;

                                            case 5:
                                                tagInventory.port5reads += 1;
                                                break;

                                            case 6:
                                                tagInventory.port6reads += 1;
                                                break;

                                            case 7:
                                                tagInventory.port7reads += 1;
                                                break;

                                            case 8:
                                                tagInventory.port8reads += 1;
                                                break;

                                            case 9:
                                                tagInventory.port9reads += 1;
                                                break;

                                            case 10:
                                                tagInventory.port10reads += 1;
                                                break;

                                            case 11:
                                                tagInventory.port11reads += 1;
                                                break;

                                            case 12:
                                                tagInventory.port12reads += 1;
                                                break;

                                            case 13:
                                                tagInventory.port13reads += 1;
                                                break;

                                            case 14:
                                                tagInventory.port14reads += 1;
                                                break;

                                            case 15:
                                                tagInventory.port15reads += 1;
                                                break;

                                            default:
                                                break;
                                        }
                                    }

                                    //tagInventory.inventoryRoundCount	;
                                    //tagInventory.firstinventoryRound	;
                                    //tagInventory.lastinventoryRound	;
                                    tagInventory.dataRead = "";    //clark 2011.4. copied from R1000 Tracer
                                    TagInventoryData[epcDataKey] = tagInventory;

                                }
                            }
                        }
                    }
                    break;

                case PacketData.PacketType.ISO18K6C_INVENTORY_DIAG:
                    break;

                case PacketData.PacketType.ISO18K6C_TAG_ACCESS:
                    {
                        PacketData.Iso18k6c_access access = packet as PacketData.Iso18k6c_access;
                        Debug.Assert( access != null );

                        BitVector32 flags = new BitVector32( access.flags );
                        bool badCrc = flags[ PacketData.PacketBase.accessCRCFlag ] == ( int ) PacketData.PacketBase.CrcResultValues.Bad;
                        if ( badCrc )
                        {
#pragma warning disable 420 // reference to a volatile field is valid for Interlocked call
                            System.Threading.Interlocked.Increment( ref _cRCErrors );
#pragma warning restore 420
                        }
                        else
                        {
                            if ( flags[ PacketData.PacketBase.accessErrorFlag ] == ( int ) PacketData.PacketBase.ISO_18000_6C_ErrorFlag.AccessSucceeded )
                            {
#pragma warning disable 420 // reference to a volatile field is valid for Interlocked call
                                System.Threading.Interlocked.Increment( ref _commonAccessCount );
#pragma warning restore 420
                            }
                           // Update Requests Here - todo
                           // envelope.RecordReadRequest( ( int ) TagBank, TagMemoryLocation, TagMemoryLength, TagPassword );

                            
                            //Clark 2011.2.18 Copied from R1000 Tracer
                            PacketData.PacketWrapper[] RecentPacketArray = new PacketData.PacketWrapper[RecentPacketList.Count];
                            RecentPacketList.CopyTo(RecentPacketArray);

                            for (int i = (RecentPacketArray.Length) - 1; i >= 0; i--)
                            {
                                if (RecentPacketArray[i].PacketType == PacketData.PacketType.ISO18K6C_INVENTORY)
                                {
                                    PacketData.PacketBase pa = RecentPacketArray[i].Packet;
                                    PacketData.inventory epc = (PacketData.inventory)pa;

                                    string epcData = BitConverter.ToString(epc.inventory_data, 0, epc.inventory_data.Length);// - flags[PacketData.PacketBase.paddingBytes]);                                    

                                    KeyList<TagInventory> epcDataKey = new KeyList<TagInventory>("tagIdData", epcData);

                                    //Console.WriteLine(epcData);
                                    TagInventory tagInventory = new TagInventory();
                                    lock (_tagInventoryData)
                                    {
                                        if (TagInventoryData.Contains(epcDataKey))
                                        {
                                            tagInventory = (TagInventory)TagInventoryData[epcDataKey];
                                            BitVector32 accessflags = new BitVector32(access.flags);
                                            if (access.data.Length >= 2)
                                            {
                                                if (accessflags[PacketData.PacketBase.accessErrorFlag] == (int)PacketData.PacketBase.ISO_18000_6C_ErrorFlag.AccessSucceeded)
                                                {
                                                    //int Count = TagAccessReqCount * 2;
                                                    //if (TagAccessReqCount > 8)
                                                    //{
                                                    //    Count = 16;
                                                    //    TagAccessReqCount -= 8;
                                                    //}

                                                    //tagInventory.dataRead += BitConverter.ToString(access.data, 0, Count);// TagAccessReqCount * 2);//access.data.GetLength(0));  
                                                    tagInventory.dataRead += BitConverter.ToString(access.data, 0, TagAccessReqCount * 2);//access.data.GetLength(0));                                                
                                                }
                                                else
                                                    tagInventory.dataRead = "";
                                                TagInventoryData[epcDataKey] = tagInventory;
                                            }
                                            //Console.WriteLine(BitConverter.ToString(access.data, 0, access.data.Length));
                                        }
                                    }

                                    break;
                                }
                            }                        
                        }
                    }
                    break;

                case PacketData.PacketType.FREQUENCY_HOP_DIAG:
                    break;

                case PacketData.PacketType.DEBUG:
                    {
                        PacketData.debug debugPacket = packet as PacketData.debug;
                        Debug.Assert(debugPacket != null);
                    }
                    break;

                case PacketData.PacketType.COMMAND_ACTIVE:
                    {
                        PacketData.command_active commandActive = packet as PacketData.command_active;
                        Debug.Assert(commandActive != null);
                    }
                    break;


                case PacketData.PacketType.NONCRITICAL_FAULT:
                    break;

                case PacketData.PacketType.U_N_D_F_I_N_E_D:
                default:
                    break;
            }

            // Save the packet
            FileHandler.WritePacket( envelope );

            RecentPacketList.Add( new PacketData.PacketWrapper( envelope ) );

            if ( EnableLogging )
            {
                //Write Log
                PacketLogger.Log( envelope );
            }
        } //private void SavePacket(PacketData.PacketWrapper envelope)

        /// <summary>
        /// 
        /// </summary>
        /// <param name="envelope"></param>
        private void ProcessPacket( PacketData.PacketWrapper envelope )
        {
            Debug.Assert( envelope.ReaderName == Name );

            PacketData.PacketBase packet = envelope.Packet;
            PacketData.PacketType type = envelope.PacketType;

            if ( envelope.IsPseudoPacket )
            {
                PacketData.CommandPsuedoPacket psuedo = envelope.Packet as PacketData.CommandPsuedoPacket;
                Debug.Assert( psuedo != null );
                if ( psuedo != null )
                {
                    switch ( psuedo.RequestName )
                    {
                        case "Inventory":
                        case "Tag Access":

                            ReaderRequest readerRequest = new ReaderRequest( );

                            using ( MemoryStream data = new MemoryStream( psuedo.DataValues ) )
                            {
                                readerRequest.ReadFrom( data );
                            }
                            ReaderRequestData.Add( readerRequest );

#pragma warning disable 420 // reference to a volatile field is valid for Interlocked call
                            System.Threading.Interlocked.Exchange( ref _processedCmdReadCount, ProcessedInventoryCount );
#pragma warning restore 420
                            break;

                        case "BadPacket":
                            BadPacket badPacket = new BadPacket( );
                            using ( MemoryStream data = new MemoryStream( psuedo.DataValues ) )
                            {
                                badPacket.ReadFrom( data );
                            }
                            PacketStream packetStream = new PacketStream( );
#pragma warning disable 420 // reference to a volatile field is valid for Interlocked call
                            packetStream.packetSequence = System.Threading.Interlocked.Increment( ref _processedPacketIndex );
#pragma warning restore 420
                            packetStream.readerID = envelope.ReaderIndex;
                            packetStream.reader = envelope.ReaderName;
                            packetStream.packetTime = envelope.Timestamp;
                            packetStream.packetType = "bad packet";
                            packetStream.elapsedTimeMs = envelope.ElapsedTimeMs;
                            packetStream.requestNumber = envelope.CommandNumber;
                            packetStream.readerIndex = envelope.ReaderIndex;
                            packetStream.rawPacketData = badPacket.RawPacketData;
                            packetStream.packetData = badPacket.RawPacketData == null ? null : BitConverter.ToString( badPacket.RawPacketData );
                            packetStream.isPseudoPacket = true;
                            PacketStreamData.Add( packetStream );

                            badPacket.readerID = envelope.ReaderIndex;
                            badPacket.packetData = packetStream.packetData;
                            badPacket.packetSequence = packetStream.packetSequence;
                            badPacket.requestSequence = envelope.CommandNumber;
                            BadPacketData.Add( badPacket );
                            break;

                        default:
                            Debug.Assert( false );
                            break;

                    }
                }
            }
            else
            {
                PacketStream packetStream = new PacketStream( );
#pragma warning disable 420 // reference to a volatile field is valid for Interlocked call
                packetStream.packetSequence = System.Threading.Interlocked.Increment( ref _processedPacketIndex );
#pragma warning restore 420
                packetStream.readerID = envelope.ReaderIndex;
                packetStream.reader = envelope.ReaderName;
                packetStream.packetTime = envelope.Timestamp;
                packetStream.packetType = envelope.PacketTypeName;
                packetStream.elapsedTimeMs = envelope.ElapsedTimeMs;
                packetStream.requestNumber = envelope.CommandNumber;
                packetStream.readerIndex = envelope.ReaderIndex;
                packetStream.rawPacketData = envelope.RawPacket;
                packetStream.packetData = BitConverter.ToString( envelope.RawPacket );
                packetStream.isPseudoPacket = false;
                PacketStreamData.Add( packetStream );

                switch ( type )
                {
                    case PacketData.PacketType.CMD_BEGIN:
                        {
                            _commandTagList.Clear( );
                            _antennaCycleTagList.Clear( );
                            _antennaTagList.Clear( );
                            _inventoryCycleTagList.Clear( );
                            _inventoryRoundTagList.Clear( );

                            LastUsedAntenna = null;
                            PacketData.cmd_beg cmd_beg = packet as PacketData.cmd_beg;
                            Debug.Assert( cmd_beg != null );
                            BitVector32 flags = new BitVector32( cmd_beg.flags );

                            ReaderCommand readerCommand = new ReaderCommand( );
                            readerCommand.packetSequence = envelope.PacketNumber;
                            readerCommand.readerID = envelope.ReaderIndex;
                            readerCommand.packetTime = envelope.Timestamp;
                            readerCommand.commandStartTime = envelope.ElapsedTimeMs;
                            readerCommand.startingTagCount = ProcessedInventoryCount;
#pragma warning disable 420 // reference to a volatile field is valid for Interlocked call
                            readerCommand.commandSequence = System.Threading.Interlocked.Increment( ref _processedCommandIndex );
#pragma warning restore 420
                            readerCommand.continuousModeFlag = flags[ PacketData.PacketBase.continuousMode ] == ( int ) PacketData.PacketBase.ContinuousModeFlag.InContinuousMode ? "Yes" : "No";
                            readerCommand.commandType = PacketData.PacketBase.GetCommandName( cmd_beg.command );
                            readerCommand.cmd_begTime = cmd_beg.ms_ctr;
                            ReaderCommandData.Add( readerCommand );
                        }
                        break;

                    case PacketData.PacketType.CMD_END:
                        {
                            PacketData.cmd_end cmd_end = packet as PacketData.cmd_end;
                            Debug.Assert( cmd_end != null );
                            // TODO: Remove Debug Code
                            // cmd_end.ms_ctr = FakeTime;
                            if ( ReaderCommandData.Contains( _processedCommandIndex ) )
                            {
                                ReaderCommand readerCommand = ( ReaderCommand ) ReaderCommandData[ _processedCommandIndex ];
                                readerCommand.commandEndTime = envelope.ElapsedTimeMs;
                                readerCommand.tagCount = ProcessedInventoryCount - readerCommand.startingTagCount;
                                readerCommand.elapsedTime = readerCommand.CommandEndTime - readerCommand.CommandStartTime;
                                readerCommand.cmd_endTime = cmd_end.ms_ctr;
                                readerCommand.cmd_delta = readerCommand.cmd_endTime - readerCommand.cmd_begTime;
                                readerCommand.commandResult = PacketData.PacketBase.GetResultName( cmd_end.Result );

                                if ( readerCommand.TagCount > 0 && readerCommand.elapsedTime > 0 )
                                    readerCommand.singulationRate = readerCommand.TagCount / ( readerCommand.elapsedTime / 1000f );

                                readerCommand.uniqueTagCount = CommandUniqueTags;
                                ReaderCommandData[ _processedCommandIndex ] = readerCommand;
                            }
                            else
                            {
                                Debug.Assert( false, "Unable to find ReaderCommandData for " + _processedCommandIndex.ToString( ) );
                            }

                            if ( ReaderRequestData.Contains( envelope.CommandNumber ) )
                            {
                                ReaderRequest readerRequest = ( ReaderRequest ) ReaderRequestData[ envelope.CommandNumber ];
                                readerRequest.endTime = envelope.Timestamp;
                                readerRequest.requestEndTime = envelope.ElapsedTimeMs;
                                readerRequest.elapsedTime = readerRequest.RequestEndTime - readerRequest.RequestStartTime;
                                readerRequest.packetCount = ProcessedPacketCount - readerRequest.startingPacketCount;
                                readerRequest.tagCount = ProcessedInventoryCount - readerRequest.startingTagCount;

                                if ( readerRequest.TagCount > 0 && readerRequest.elapsedTime > 0 )
                                    readerRequest.singulationRate = readerRequest.TagCount / ( readerRequest.elapsedTime / 1000f );

                                ReaderRequestData[ envelope.CommandNumber ] = readerRequest;
                            }
                            else
                            {
                                Debug.Assert( false, "Unable to find ReaderRequestData for " + envelope.CommandNumber.ToString( ) );
                            }
                        }
                        break;

                    case PacketData.PacketType.ANTENNA_CYCLE_BEGIN:
                        {
                            _antennaCycleTagList.Clear( );
                            _antennaTagList.Clear( );
                            _inventoryCycleTagList.Clear( );
                            _inventoryRoundTagList.Clear( );

                            PacketData.ant_cyc_beg cyc_beg = packet as PacketData.ant_cyc_beg;
                            Debug.Assert( cyc_beg != null );
                            ReaderAntennaCycle readerCycle = new ReaderAntennaCycle( );
                            readerCycle.packetSequence = envelope.PacketNumber;
                            readerCycle.readerID = envelope.ReaderIndex;
                            readerCycle.packetTime = envelope.Timestamp;
                            readerCycle.startingAntennaCount = ProcessedAntennaCount;
                            readerCycle.startingTagCount = ProcessedInventoryCount;
                            readerCycle.cycleStartTime = envelope.ElapsedTimeMs;
                            readerCycle.commandSequence = _processedCommandIndex;
                            readerCycle.startingInventoryCycleCount = ProcessedInventoryCycleCount;
                            readerCycle.startingRoundCount = ProcessedRoundCount;
#pragma warning disable 420 // reference to a volatile field is valid for Interlocked call
                            readerCycle.cycleSequence = System.Threading.Interlocked.Increment( ref _processedAntennaCycleIndex );
#pragma warning restore 420
                            ReaderAntennaCycleData.Add( readerCycle );
                        }
                        break;

                    case PacketData.PacketType.ANTENNA_CYCLE_BEGIN_DIAG:
                        {
                            PacketData.ant_cyc_beg_diag cyc_beg_diag = packet as PacketData.ant_cyc_beg_diag;
                            Debug.Assert( cyc_beg_diag != null );

                            if ( ReaderAntennaCycleData.Contains( _processedAntennaCycleIndex ) )
                            {
                                ReaderAntennaCycle readerCycle = ( ReaderAntennaCycle ) ReaderAntennaCycleData[ _processedAntennaCycleIndex ];
                                readerCycle.cyc_begTime = cyc_beg_diag.ms_ctr;
                                ReaderAntennaCycleData[ _processedAntennaCycleIndex ] = readerCycle;
                            }
                            else
                            {
                                Debug.Assert( false, "Unable to find ReaderCycleData for " + _processedAntennaCycleIndex.ToString( ) );
                            }
                        }
                        break;

                    case PacketData.PacketType.ANTENNA_CYCLE_END:
                        {
                            PacketData.ant_cyc_end cyc_end = packet as PacketData.ant_cyc_end;
                            Debug.Assert( cyc_end != null );
                            if ( ReaderAntennaCycleData.Contains( _processedAntennaCycleIndex ) )
                            {
                                ReaderAntennaCycle readerCycle = ( ReaderAntennaCycle ) ReaderAntennaCycleData[ _processedAntennaCycleIndex ];
                                readerCycle.cycleEndTime = envelope.ElapsedTimeMs;
                                readerCycle.endingAntennaCount = ProcessedAntennaCount;
                                readerCycle.endingTagCount = ProcessedInventoryCount;
                                readerCycle.uniqueTagCount = AntennaCycleUniqueTags;
                                readerCycle.endingInventoryCycleCount = ProcessedInventoryCycleCount;
                                readerCycle.endingRoundCount = ProcessedRoundCount;
                                ReaderAntennaCycleData[ _processedAntennaCycleIndex ] = readerCycle;
                            }
                            else
                            {
                                Debug.Assert( false, "Unable to find ReaderCycleData for " + _processedAntennaCycleIndex.ToString( ) );
                            }
                        }
                        break;

                    case PacketData.PacketType.ANTENNA_CYCLE_END_DIAG:
                        {
                            PacketData.ant_cyc_end_diag cyc_end_diag = packet as PacketData.ant_cyc_end_diag;
                            Debug.Assert( cyc_end_diag != null );

                            if ( ReaderAntennaCycleData.Contains( _processedAntennaCycleIndex ) )
                            {
                                ReaderAntennaCycle readerCycle = ( ReaderAntennaCycle ) ReaderAntennaCycleData[ _processedAntennaCycleIndex ];
                                readerCycle.cyc_endTime = cyc_end_diag.ms_ctr;
                                ReaderAntennaCycleData[ _processedAntennaCycleIndex ] = readerCycle;
                            }
                            else
                            {
                                Debug.Assert( false, "Unable to find ReaderCycleData for " + _processedAntennaCycleIndex.ToString( ) );
                            }
                        }
                        break;

                    case PacketData.PacketType.ANTENNA_BEGIN:
                        {
                            _antennaTagList.Clear( );
                            _inventoryCycleTagList.Clear( );
                            _inventoryRoundTagList.Clear( );

                            PacketData.ant_beg ant_beg = packet as PacketData.ant_beg;
                            Debug.Assert( ant_beg != null );
                            AntennaPacket antennaCycle = new AntennaPacket( );
                            antennaCycle.readerID = envelope.ReaderIndex;
                            antennaCycle.packetTime = envelope.Timestamp;
                            antennaCycle.antennaStartTime = envelope.ElapsedTimeMs;
                            antennaCycle.startingTagCount = ProcessedInventoryCount;
                            antennaCycle.commandSequence = _processedCommandIndex;
                            antennaCycle.cycleSequence = _processedAntennaCycleIndex;
#pragma warning disable 420 // reference to a volatile field is valid for Interlocked call
                            antennaCycle.antennaSequence = System.Threading.Interlocked.Increment( ref _processedAntennaIndex );
#pragma warning restore 420
                            antennaCycle.antennaNumber = ant_beg.antenna;
                            AntennaCycleData.Add( antennaCycle );

                            LastUsedAntenna = ant_beg.antenna;
                        }
                        break;

                    case PacketData.PacketType.ANTENNA_END:
                        {
                            PacketData.ant_end ant_end = packet as PacketData.ant_end;
                            Debug.Assert( ant_end != null );
                            if ( AntennaCycleData.Contains( _processedAntennaIndex ) )
                            {
                                AntennaPacket antennaCycle = ( AntennaPacket ) AntennaCycleData[ _processedAntennaIndex ];
                                antennaCycle.antennaEndTime = envelope.ElapsedTimeMs;
                                if ( antennaCycle.antennaEndTime != null && antennaCycle.antennaStartTime != null )
                                {
                                    antennaCycle.elapsedTime = antennaCycle.antennaEndTime - antennaCycle.antennaStartTime;
                                }
                                antennaCycle.tagCount = ProcessedInventoryCount - antennaCycle.startingTagCount;

                                if ( antennaCycle.TagCount > 0 && antennaCycle.elapsedTime > 0 )
                                    antennaCycle.singulationRate = antennaCycle.TagCount / ( antennaCycle.elapsedTime / 1000f );

                                antennaCycle.uniqueTagCount = AntennaUniqueTags;

                                AntennaCycleData[ _processedAntennaIndex ] = antennaCycle;
                            }
                            else
                            {
                                Debug.Assert( false, "Unable to find AntennaCycleData for " + _processedAntennaIndex.ToString( ) );
                            }
                        }
                        break;

                    case PacketData.PacketType.ANTENNA_BEGIN_DIAG:
                        {
                            PacketData.ant_beg_diag ant_beg_diag = packet as PacketData.ant_beg_diag;
                            Debug.Assert( ant_beg_diag != null );
                            if ( AntennaCycleData.Contains( _processedAntennaIndex ) )
                            {
                                AntennaPacket antennaCycle = ( AntennaPacket ) AntennaCycleData[ _processedAntennaIndex ];
                                antennaCycle.ant_begTime = ant_beg_diag.ms_ctr;
                                antennaCycle.senseResistorValue = ant_beg_diag.sense_res;
                                AntennaCycleData[ _processedAntennaIndex ] = antennaCycle;
                            }
                            else
                            {
                                Debug.Assert( false, "Unable to find AntennaCycleData for " + _processedAntennaIndex.ToString( ) );
                            }
                        }
                        break;

                    case PacketData.PacketType.ANTENNA_END_DIAG:
                        {
                            PacketData.ant_end_diag ant_end_diag = packet as PacketData.ant_end_diag;
                            Debug.Assert( ant_end_diag != null );

                            //TODO: Remove Debug Code
                            //ant_end_diag.ms_ctr = FakeTime;

                            if ( AntennaCycleData.Contains( _processedAntennaIndex ) )
                            {
                                AntennaPacket antennaCycle = ( AntennaPacket ) AntennaCycleData[ _processedAntennaIndex ];
                                antennaCycle.ant_endTime = ant_end_diag.ms_ctr;
                                if ( antennaCycle.ant_endTime != null && antennaCycle.ant_begTime != null )
                                {
                                    antennaCycle.ant_delta = antennaCycle.ant_endTime - antennaCycle.ant_begTime;
                                }

                                AntennaCycleData[ _processedAntennaIndex ] = antennaCycle;
                            }
                            else
                            {
                                Debug.Assert( false, "Unable to find AntennaCycleData for " + _processedAntennaIndex.ToString( ) );
                            }
                        }
                        break;

                    case PacketData.PacketType.INVENTORY_CYCLE_BEGIN:
                        {
                            _inventoryCycleTagList.Clear( );
                            _inventoryRoundTagList.Clear( );

                            PacketData.inv_cyc_beg inv_cyc_beg = packet as PacketData.inv_cyc_beg;
                            Debug.Assert( inv_cyc_beg != null );

                            InventoryCycle inventoryCycle = new InventoryCycle( );
                            inventoryCycle.packetTime = envelope.Timestamp;
                            inventoryCycle.readerID = envelope.ReaderIndex;
                            inventoryCycle.packetSequence = envelope.PacketNumber;
                            inventoryCycle.commandSequence = _processedCommandIndex;
#pragma warning disable 420 // reference to a volatile field is valid for Interlocked call
                            inventoryCycle.cycleSequence = System.Threading.Interlocked.Increment( ref _processedInventoryCycleIndex );
#pragma warning restore 420
                            inventoryCycle.cycleStartTime = envelope.ElapsedTimeMs;
                            inventoryCycle.startingTagCount = ProcessedInventoryCount;
                            inventoryCycle.startingRoundCount = ProcessedInventoryCount;
                            inventoryCycle.inv_cyc_begTime = inv_cyc_beg.ms_ctr;
                            inventoryCycle.antennaNumber = LastUsedAntenna;
                            int index = InventoryCycleData.Add( inventoryCycle );
                            Debug.Assert( index == inventoryCycle.cycleSequence );
                        }
                        break;

                    case PacketData.PacketType.INVENTORY_CYCLE_END:
                        {
                            PacketData.inv_cyc_end inv_cyc_end = packet as PacketData.inv_cyc_end;
                            Debug.Assert( inv_cyc_end != null );

                            if ( InventoryCycleData.Contains( _processedInventoryCycleIndex ) )
                            {
                                InventoryCycle inventoryCycle = ( InventoryCycle ) InventoryCycleData[ _processedInventoryCycleIndex ];
                                inventoryCycle.cycleEndTime = envelope.ElapsedTimeMs;
                                inventoryCycle.endingTagCount = ProcessedInventoryCount;
                                inventoryCycle.uniqueTagCount = InventoryCycleUniqueTags;
                                inventoryCycle.endingRoundCount = ProcessedInventoryCount;
                                inventoryCycle.endingRoundCount = ProcessedRoundCount;
                                inventoryCycle.inv_cyc_endTime = inv_cyc_end.ms_ctr;
                                InventoryCycleData[ _processedInventoryCycleIndex ] = inventoryCycle;
                            }
                            else
                            {
                                Debug.Assert( false, "Unable to find InventoryCycleData for " + _processedInventoryCycleIndex.ToString( ) );
                            }
                        }
                        break;

                    case PacketData.PacketType.CARRIER_INFO:
                        {
                            PacketData.carrier_info carrier_info = packet as PacketData.carrier_info;
                            Debug.Assert( carrier_info != null );
                        }
                        break;

                    case PacketData.PacketType.INVENTORY_CYCLE_END_DIAGS:
                        {
                            PacketData.inv_cyc_end_diag inv_cyc_end_diag = packet as PacketData.inv_cyc_end_diag;
                            Debug.Assert( inv_cyc_end_diag != null );
                            if ( InventoryCycleData.Contains( _processedInventoryCycleIndex ) )
                            {
                                InventoryCycle inventoryCycle = ( InventoryCycle ) InventoryCycleData[ _processedInventoryCycleIndex ];
                                inventoryCycle.querys = inv_cyc_end_diag.querys;
                                inventoryCycle.rn16rcv = inv_cyc_end_diag.rn16rcv;
                                inventoryCycle.rn16Timeouts = inv_cyc_end_diag.rn16to;
                                inventoryCycle.epcTimeouts = inv_cyc_end_diag.epcto;
                                inventoryCycle.goodReads = inv_cyc_end_diag.good_reads;
                                inventoryCycle.crcFailures = inv_cyc_end_diag.crc_failures;

                                InventoryCycleData[ _processedInventoryCycleIndex ] = inventoryCycle;
                            }
                            else
                            {
                                Debug.Assert( false, "Unable to find InventoryCycleData for " + _processedInventoryCycleIndex.ToString( ) );
                            }
                        }
                        break;

                    case PacketData.PacketType.ISO18K6C_INVENTORY_ROUND_BEGIN:
                        {
                            _inventoryRoundTagList.Clear( );

                            PacketData.inv_rnd_beg inv_rnd_beg = packet as PacketData.inv_rnd_beg;
                            Debug.Assert( inv_rnd_beg != null );

                            InventoryRound inventoryRound   = new InventoryRound( );
                            inventoryRound.packetSequence   = envelope.PacketNumber;
                            inventoryRound.readerID         = envelope.ReaderIndex;
                            inventoryRound.packetTime       = envelope.Timestamp;
                            inventoryRound.commandSequence  = _processedCommandIndex;
                            inventoryRound.cycleSequence    = _processedAntennaCycleIndex;
                            inventoryRound.antennaSequence  = _processedAntennaIndex;
#pragma warning disable 420 // reference to a volatile field is valid for Interlocked call
                            inventoryRound.roundSequence    = System.Threading.Interlocked.Increment( ref _processedRoundIndex );
#pragma warning restore 420
                            inventoryRound.roundStartTime   = envelope.ElapsedTimeMs;
                            inventoryRound.startingTagCount = ProcessedInventoryCount;

                            inventoryRound.antennaNumber    = LastUsedAntenna;

                            int index = InventoryRoundData.Add( inventoryRound );
                            Debug.Assert( index == inventoryRound.roundSequence );
                        }
                        break;

                    case PacketData.PacketType.ISO18K6C_INVENTORY_ROUND_BEGIN_DIAG:
                        {
                            PacketData.inv_rnd_beg_diag inv_rnd_beg_diag = packet as PacketData.inv_rnd_beg_diag;
                            Debug.Assert( inv_rnd_beg_diag != null );

                            if ( InventoryRoundData.Contains( _processedRoundIndex ) )
                            {
                                InventoryRound inventoryRound  = ( InventoryRound ) InventoryRoundData[ _processedRoundIndex ];
                                inventoryRound.inv_rnd_begTime = inv_rnd_beg_diag.ms_ctr;
                                inventoryRound.singulationParameters = inv_rnd_beg_diag.sing_params;

                                BitVector32 parameters = new BitVector32( ( int ) inv_rnd_beg_diag.sing_params );

                                InventoryRoundData[ _processedRoundIndex ] = inventoryRound;
                            }
                            else
                            {
                                Debug.Assert( false, "Unable to find InventoryRoundData for " + _processedRoundIndex.ToString( ) );
                            }
                        }
                        break;

                    case PacketData.PacketType.ISO18K6C_INVENTORY_ROUND_END:
                        {
                            PacketData.inv_rnd_end inv_rnd_end = packet as PacketData.inv_rnd_end;
                            Debug.Assert( inv_rnd_end != null );
                            if ( InventoryRoundData.Contains( _processedRoundIndex ) )
                            {
                                InventoryRound inventoryRound = ( InventoryRound ) InventoryRoundData[ _processedRoundIndex ];
                                inventoryRound.roundEndTime   = envelope.ElapsedTimeMs;
                                inventoryRound.endingTagCount = ProcessedInventoryCount;
                                inventoryRound.uniqueTagCount = InventoryRoundUniqueTags;

                                InventoryRoundData[ _processedRoundIndex ] = inventoryRound;
                            }
                            else
                            {
                                Debug.Assert( false, "Unable to find InventoryRoundData for " + _processedRoundIndex.ToString( ) );
                            }
                        }
                        break;

                    case PacketData.PacketType.ISO18K6C_INVENTORY_ROUND_END_DIAG:
                        {
                            PacketData.inv_rnd_end_diag inv_rnd_end_diag = packet as PacketData.inv_rnd_end_diag;
                            Debug.Assert( inv_rnd_end_diag != null );
                            if ( InventoryRoundData.Contains( _processedRoundIndex ) )
                            {
                                InventoryRound inventoryRound  = ( InventoryRound ) InventoryRoundData[ _processedRoundIndex ];
                                inventoryRound.inv_rnd_endTime = inv_rnd_end_diag.ms_ctr;
                                inventoryRound.querys          = inv_rnd_end_diag.querys;
                                inventoryRound.rn16rcv         = inv_rnd_end_diag.rn16rcv;
                                inventoryRound.rn16Timeouts    = inv_rnd_end_diag.rn16to;
                                inventoryRound.epcTimeouts     = inv_rnd_end_diag.epcto;
                                inventoryRound.goodReads       = inv_rnd_end_diag.good_reads;
                                inventoryRound.crcFailures     = inv_rnd_end_diag.crc_failures;

                                //if (inventoryRound.inv_rnd_begTime != null && inventoryRound.inv_rnd_endTime != null)
                                //{
                                //    inventoryRound.inv_rnd_delta = inventoryRound.inv_rnd_endTime - inventoryRound.inv_rnd_begTime;
                                //}
                                InventoryRoundData[ _processedRoundIndex ] = inventoryRound;
                            }
                            else
                            {
                                Debug.Assert( false, "Unable to find InventoryRoundData for " + _processedRoundIndex.ToString( ) );
                            }
                        }
                        break;

                    case PacketData.PacketType.ISO18K6C_INVENTORY:
                        {
                            PacketData.inventory inventory = packet as PacketData.inventory;
                            Debug.Assert( inventory != null );
                            BitVector32 flags = new BitVector32( inventory.flags );
                            bool badCrc = flags[ PacketData.PacketBase.crcResult ] == ( int ) PacketData.PacketBase.CrcResultValues.Bad;
                            if ( badCrc )
                            {
                            }
                            else
                            {
                                TagRead tagRead = new TagRead( );
                                tagRead.packetSequence = envelope.PacketNumber;
                                //tagRead.reader			= envelope.ReaderName;
                                tagRead.readerID        = envelope.ReaderIndex;
                                tagRead.packetTime      = envelope.Timestamp;
                                tagRead.readTime        = envelope.ElapsedTimeMs;
                                tagRead.commandSequence = _processedCommandIndex;
                                tagRead.cycleSequence   = _processedAntennaCycleIndex;
                                tagRead.antennaSequence = _processedAntennaIndex;
                                tagRead.antennaNumber   = LastUsedAntenna;
                                tagRead.roundSequence   = _processedRoundIndex;
                                tagRead.crcResult       = "valid";
#pragma warning disable 420 // reference to a volatile field is valid for Interlocked call
                                tagRead.readSequence = System.Threading.Interlocked.Increment( ref _processedInventoryIndex );
#pragma warning restore 420
                                tagRead.nb_rssi   = inventory.nb_rssi;
                                tagRead.wb_rssi   = inventory.wb_rssi;
                                tagRead.ana_ctrl1 = inventory.ana_ctrl1;
                                tagRead.rssi = inventory.rssi;
                                //							tagRead.tagType			= flags[PacketData.PacketBase.tagType] == 0xC ? "ISO 18000-6C" : "unknown";
                                string tagData = BitConverter.ToString( inventory.inventory_data, 0, inventory.inventory_data.Length - flags[ PacketData.PacketBase.paddingBytes ] );
                                tagRead.accessType = "inventory";
                                tagRead.tagId = tagData;
                                tagRead.inventoryTime = inventory.ms_ctr;
                                TagReadData.Add( tagRead );

                                ReadRate readRate = new ReadRate( );

                                if ( 0 == ( readRate.readSequence = _processedInventoryIndex - _processedCmdReadCount ) ) // note _processedInventoryIndex is incremented above
                                {
                                    _lastCmdClientTime = envelope.ElapsedTimeMs;
                                    _lastCmdDeviceTime = inventory.ms_ctr;
                                }
                                readRate.readerID = envelope.ReaderIndex;
                                readRate.clientReadTime = envelope.ElapsedTimeMs - _lastCmdClientTime;
                                readRate.deviceReadTime = inventory.ms_ctr - _lastCmdDeviceTime;
                                ReadRateData.Add( readRate );

                                rfidTag tag = new rfidTag( inventory.inventory_data );
                                _commandTagList.AddOrIncrementTagCount( tag );
                                _antennaCycleTagList.AddOrIncrementTagCount( tag );
                                _antennaTagList.AddOrIncrementTagCount( tag );
                                _inventoryCycleTagList.AddOrIncrementTagCount( tag );
                                _inventoryRoundTagList.AddOrIncrementTagCount( tag );
                            }
                        }
                        break;

                    case PacketData.PacketType.ISO18K6C_INVENTORY_DIAG:
                        {
                            PacketData.inventory_diag epc_diag = packet as PacketData.inventory_diag;
                            Debug.Assert( epc_diag != null );
                            if ( TagReadData.Contains( _processedInventoryIndex ) )
                            {
                                TagRead tagRead = ( TagRead ) TagReadData[ _processedInventoryIndex ];
                                tagRead.protocolParameters = epc_diag.prot_parms;
                                TagReadData[ _processedInventoryIndex ] = tagRead;
                            }
                            else
                            {
                                Debug.Assert( false, "Unable to find TagReadData for " + _processedInventoryIndex.ToString( ) );
                            }
                        }
                        break;

                    case PacketData.PacketType.ISO18K6C_TAG_ACCESS:
                        {
                            PacketData.Iso18k6c_access access = packet as PacketData.Iso18k6c_access;
                            Debug.Assert( access != null );
                            BitVector32 flags = new BitVector32( access.flags );
                            bool badCrc = flags[ PacketData.PacketBase.accessCRCFlag ] == ( int ) PacketData.PacketBase.CrcResultValues.Bad;
                            if ( badCrc )
                            {
                            }
                            else
                            {
                                string tagData = BitConverter.ToString( access.data, 0, access.data.Length - flags[ PacketData.PacketBase.accessPadding ] );
                                if ( TagReadData.Contains( _processedInventoryIndex ) )
                                {
                                    TagRead tagRead = ( TagRead ) TagReadData[ _processedInventoryIndex ];
                                    tagRead.resultType = flags[ PacketData.PacketBase.accessErrorFlag ] == ( int ) PacketData.PacketBase.ISO_18000_6C_ErrorFlag.AccessError ? "Error" : "";
                                    tagRead.accessType = PacketData.PacketBase.GetTagAccessTypeName( access.command );
                                    tagRead.parameter = envelope.ReadRequest;
                                    tagRead.tagData = tagData;
                                    TagReadData[ _processedInventoryIndex ] = tagRead;
                                }
                                else
                                {
                                    // Check for Read Data - todo
                                    // Debug.Assert( false, "Unable to find TagReadData for " + _processedInventoryIndex.ToString( ) );
                                }
                            }
                        }
                        break;

                    case PacketData.PacketType.FREQUENCY_HOP_DIAG:
                        Debug.Assert( false, "New Packet" );
                        break;

                    case PacketData.PacketType.NONCRITICAL_FAULT:
                        Debug.Assert( false, "New Packet" );
                        break;

                    case PacketData.PacketType.COMMAND_ACTIVE:
                        break;

                    case PacketData.PacketType.DEBUG:
                        break;

                    case PacketData.PacketType.U_N_D_F_I_N_E_D:
                    default:
                        Debug.Assert( false, "Unexpected Packet" );
                        break;
                }
            }
        }


        //Library API
        public Result API_AntennaPortGetSenseThreshold(ref UInt32 AntennaThreshold)
        {
            return LakeChabotReader.MANAGED_ACCESS.API_AntennaPortGetSenseThreshold(ref AntennaThreshold);
        }

        public Result API_AntennaPortSetSenseThreshold(UInt32 AntennaThreshold)
        {
            return LakeChabotReader.MANAGED_ACCESS.API_AntennaPortSetSenseThreshold(AntennaThreshold);
        }

        public Result API_l8K6CTagInventory()
        {
            InventoryParms strcParms = new InventoryParms();

            return LakeChabotReader.MANAGED_ACCESS.API_l8K6CTagInventory(strcParms,0);
        }

        public Result API_ControlSoftReset()
        {
            return LakeChabotReader.MANAGED_ACCESS.API_ControlSoftReset();
        }



        //Antenna
        public Result API_AntennaPortSetState
        (
             byte             r_Port,
             AntennaPortState r_State
        )
        {
           return LakeChabotReader.MANAGED_ACCESS.API_AntennaPortSetState(r_Port, r_State);
        }



        public Result API_AntennaPortSetConfiguration
        (
            byte              r_Port,
            AntennaPortConfig r_Config
        )
        {
           return LakeChabotReader.MANAGED_ACCESS.API_AntennaPortSetConfiguration(r_Port, r_Config);
        }


        public Result API_AntennaPortGetConfiguration
        (
                byte              r_Port,
            ref AntennaPortConfig r_Config
        )
        {
           return LakeChabotReader.MANAGED_ACCESS.API_AntennaPortGetConfiguration(r_Port, ref r_Config);
        }


        //ISO 18000-6C Tag Access Parameter=========================================
        public Result API_l8K6CSetTagAccessPassword
        (
            UInt32 AccessPassword
        )
        {         
            return LakeChabotReader.MANAGED_ACCESS.API_l8K6CSetTagAccessPassword(AccessPassword);
        }


        public Result API_l8K6CTagGetAccessPassword
        (
            ref UInt32 AccessPassword
        )
        {
            return LakeChabotReader.MANAGED_ACCESS.API_l8K6CTagGetAccessPassword(ref AccessPassword);
        }



        //Mac======================================================================
        public Result API_MacGetCustomerRegion
        (
            ref string r_CustomerRegion
        )
        {
            return LakeChabotReader.MANAGED_ACCESS.API_MacGetCustomerRegion( ref r_CustomerRegion );  
        }


        //GPIO========================================================================
        public Result API_GpioReadPins
        (
                byte r_Mask,
            ref byte r_Value
        )
        { 
            return LakeChabotReader.MANAGED_ACCESS.API_GpioReadPins
                        (
                                r_Mask,
                            ref r_Value
                        );          
        
        }



        //Test API=====================================================================
        public Result API_TestSetAntennaPortConfiguration
        (
            byte   r_btPhysicalPort,
            UInt16 r_usPowerLevel
        )
        {
            return LakeChabotReader.MANAGED_ACCESS.API_TestSetAntennaPortConfiguration
                        (
                            r_btPhysicalPort,
                            r_usPowerLevel
                        );  
        }


        public Result API_TestGetAntennaPortConfiguration
        (
             ref byte   r_btPhysicalPort,
             ref UInt16 r_usPowerLevel
        )
        {
            return LakeChabotReader.MANAGED_ACCESS.API_TestGetAntennaPortConfiguration
                        (
                            ref r_btPhysicalPort,
                            ref r_usPowerLevel
                        );  
        }



        public Result API_TestSetFrequencyConfiguration
        (
            byte   r_btChannelFlag,
            UInt32 r_uiExactFrequecny
        )
        {
            return LakeChabotReader.MANAGED_ACCESS.API_TestSetFrequencyConfiguration
                        (
                            r_btChannelFlag,
                            r_uiExactFrequecny
                        );  
        }


        public Result API_TestGetFrequencyConfiguration
        (
            ref byte   r_btChannelFlag,
            ref UInt32 r_uiExactFrequecny
        )
        {
            return LakeChabotReader.MANAGED_ACCESS.API_TestGetFrequencyConfiguration
                        (
                            ref r_btChannelFlag,
                            ref r_uiExactFrequecny
                        );  
        }



        public Result API_TestSetRandomDataPulseTime
        (
            UInt16 r_usOnTime,
            UInt16 r_usOffTime
        )
        {
            return LakeChabotReader.MANAGED_ACCESS.API_TestSetRandomDataPulseTime
                        (
                            r_usOnTime,
                            r_usOffTime
                        );  
        }


        public Result API_TestGetRandomDataPulseTime
        (
            ref UInt16 r_usOnTime,
            ref UInt16 r_usOffTime
        )
        {
            return LakeChabotReader.MANAGED_ACCESS.API_TestGetRandomDataPulseTime
                        (
                            ref r_usOnTime,
                            ref r_usOffTime
                        );  
        }


        public Result API_TestTurnCarrierWaveOn()
        {
            return LakeChabotReader.MANAGED_ACCESS.API_TestTurnCarrierWaveOn();
        }

        public Result API_TestTurnCarrierWaveOff()
        {
            return LakeChabotReader.MANAGED_ACCESS.API_TestTurnCarrierWaveOff();
        } 
        //===========================================================================
       
    } //class LakeChabotReader : rfidInterface
} //namespace RFID.rfidInterface
