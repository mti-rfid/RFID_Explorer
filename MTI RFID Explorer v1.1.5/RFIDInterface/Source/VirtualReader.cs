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
 * $Id: VirtualReader.cs,v 1.9 2009/12/09 21:24:55 dciampi Exp $
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
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Xml;




namespace RFID.RFIDInterface
{

	public delegate string StopDependentReadersDelegate(bool abort);

	public class VirtualReader : IDisposable, IReader
	{
		private const int MIN_SLEEP_TIME_MS			= 20;		// Min time callback thread should sleep
		private const int SLEEP_ADD_SUBTRACT_MS		= 10;		// Amount to bump the sleep time when processor bound
		private const int TARGET_MAX_USAGE_PERCENT	= 80;		// Target percent processor usage
		private const int INITIAL_QUEUE_SIZE		= 4 * 1045;
		private const int TARGET_QUEUE_SIZE			= 256;
		private const int MAX_QUEUE_SIZE			= 512;
		private const int QUEUE_SLEEP_MS			= 10;
		private const int MIN_REFRESH_MS			= 100;
		private const int MAX_REFRESH_MS			= 1000;
		internal const int EMA_PERIOD				= 8;

		


	

		private class FileHandlerClass : IDisposable
		{
			private const int FIXED_HEADER_SIZE		= 64;
			private const int FILE_BLOCKING_SIZE	= 1024 * 4;
			private const int MAX_MEMORY_STREAMS	= 2;
			private const int MAX_MEMORY_PACKETS	= 5;

			public byte[] sig			= { 0x3e, 0x42, 0xba, 0x82, 0x95, 0xa4, 0x48, 0xc5, 0x8d, 0x6f, 0x41, 0xeb, 0xeb, 0x5a, 0x34, 0x38 };
			public byte[] Id;
			public DateTime Timestamp	= default(DateTime);


			private bool				_disposed		= false;
			private bool				_noTempFile		= false;
			private int					_packets		= 0;
			private int					_activeStream	= 0;
			private long				_lastPosition	= 0;

			private byte[]				_timestamp		= null;
			private MemoryStream		_headerStream	= null;
			private MemoryStream[]		_memoryStreams	= null;
			private FileStream			_fileStream		= null;
			private IFormatter			_formatter		= null;


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
					CloseStream();

					_noTempFile = value;

					InitializeStream();
				}
			}


			public void InitializeStream()
			{
				if (NoTempFile)
				{
					_memoryStreams =  new MemoryStream[] { new MemoryStream(FILE_BLOCKING_SIZE), null };
				}
				else
				{
					_fileStream = new FileStream(Path.GetTempFileName(), FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None, FILE_BLOCKING_SIZE, FileOptions.DeleteOnClose);

				}
			}


			public void CloseStream()
			{
				if (_fileStream != null)
				{
					_fileStream.Close();
					_fileStream = null;
				}

				if (_memoryStreams != null)
				{
					foreach (MemoryStream s in _memoryStreams)
					{
						s.Close();
					}
					_memoryStreams = null;
				}
			}



			public MemoryStream ActiveMemoryStream
			{
				get
				{
					// Caculate current index 
					int index = ((int)Math.Truncate((double)(_packets / MAX_MEMORY_PACKETS))) % MAX_MEMORY_STREAMS;
					// allocate new stream if index is new
					if (index != _activeStream)
					{
						_memoryStreams[index] = new MemoryStream(FILE_BLOCKING_SIZE);
						_activeStream = index;
					}
					return _memoryStreams[_activeStream];
				}
			}

			public Stream HeaderWriteStream
			{
				get { return NoTempFile ? (Stream)(_headerStream = new MemoryStream(FIXED_HEADER_SIZE)) : (Stream)_fileStream; }
			}

			public Stream HeaderReadStream
			{
				get { return NoTempFile ? (Stream)_headerStream : (Stream)_fileStream; }
			}

			public Stream WriteableStream
			{
				get { return NoTempFile ? (Stream)ActiveMemoryStream : (Stream)_fileStream;		}
			}

			public Stream ReadableStream
			{
				get { return Stream.Null; }
			}


			public PacketData.PacketWrapper GetPendingPacket()
			{
				PacketData.PacketWrapper result = null;
				if (NoTempFile)
					return result;

				System.Diagnostics.Debug.Assert(_fileStream != null);

				long oldPosition;
				long newPosistion;
				
				if (_fileStream != null && 
					 (oldPosition = _fileStream.Position) > 
					 (newPosistion = _lastPosition > FIXED_HEADER_SIZE ? _lastPosition : FIXED_HEADER_SIZE))
				{
					_fileStream.Position = newPosistion;
					result = _formatter.Deserialize(_fileStream) as PacketData.PacketWrapper;
					_lastPosition = _fileStream.Position;
					_fileStream.Position = oldPosition;
				}
				return result;
			}

			public IEnumerable<PacketData.PacketWrapper> PendingPackets
			{
				get
				{
					
					if (NoTempFile)
					{

						yield break;
					}
					else
					{
						System.Diagnostics.Debug.Assert(_fileStream != null);
						if (_fileStream != null)
						{
							long pos = _fileStream.Position;
							_fileStream.Position = _lastPosition > FIXED_HEADER_SIZE ? _lastPosition : FIXED_HEADER_SIZE;
							while (_fileStream.Position < pos)
							{
								yield return _formatter.Deserialize(_fileStream) as PacketData.PacketWrapper;
							}
							_lastPosition = _fileStream.Position;
							_fileStream.Position = pos;
						}
					}
				}
			}

			

			public FileHandlerClass(bool noTempFile)
			{
				Id			= new Guid().ToByteArray();
				Timestamp	= DateTime.Now;
				NoTempFile	= noTempFile;
				_timestamp = BitConverter.GetBytes(Timestamp.ToBinary());
				_formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
				WriteHeader();
			}

			public FileHandlerClass(string FileName)
			{
				NoTempFile = false;

//				SetFileStream(new FileStream(FileName, FileMode.Open, FileAccess.Read, FileShare.Read));

				byte[] header = ReaderHeader();
				for (int i = 0; i < sig.Length; i++)
				{
					if (header[i] != sig[i])
					{
						throw new rfidException(rfidErrorCode.InvalidPacketFile, String.Format("{0} is not a valid data file.", FileName));
					}
				}

				Id = new byte[sig.Length];			// _sig and ID are the same size.
				Array.Copy(header, sig.Length, Id, 0, Id.Length);
			}


			private void WriteHeader()
			{
				byte[] header = new byte[FIXED_HEADER_SIZE];

				sig.CopyTo(header, 0);
				Id.CopyTo(header, sig.Length);
				_timestamp.CopyTo(header, sig.Length + Id.Length);

				HeaderWriteStream.Write(header, 0, FIXED_HEADER_SIZE);
			}


			private byte[] ReaderHeader()
			{
				byte[] header = new byte[FIXED_HEADER_SIZE];
				HeaderReadStream.Read(header, 0, FIXED_HEADER_SIZE);
				return header;
			}

			public void WritePacket(PacketData.PacketWrapper envelope)
			{
				_packets++;
				_formatter.Serialize(WriteableStream, envelope);
			}


			public PacketData.PacketWrapper ReadPacket(Stream packetStream)
			{
				return _formatter.Deserialize(packetStream) as PacketData.PacketWrapper;
			}

			#region IDisposable Members

			public void Dispose()
			{
				Dispose(true);
				GC.SuppressFinalize(this);
			}

			#endregion

			private void Dispose(bool disposing)
			{
				if (!this._disposed)
				{
					if (disposing)
					{
						CloseStream();
					}

				}
				_disposed = true;
			}

			~FileHandlerClass()
			{
				Dispose(false);
			}

			public void Close()
			{
				Dispose(true);
			}


		} // private class FileHandler : IDisposable



		
		
		


		private class ReaderInterfaceThreadClass
		{
			public static AutoResetEvent StartEvent = new AutoResetEvent(false);
			public string	Result = null;
			private int		_stopCountDown;
			private int		_handle;
			private bool	_singlePackets;
			private bool	_stopRequest;

			private rfid.Linkage _access = null;

			/// <summary>
			/// Constructor for Inventory
			/// </summary>
			/// <param name="ManagedAccess"></param>
			/// <param name="Handle"></param>
			/// <param name="WantSinglePackets"></param>
			/// <param name="runCount"></param>
			public ReaderInterfaceThreadClass(rfid.Linkage ManagedAccess, int Handle, bool WantSinglePackets, int runCount)
			{
				if (runCount == 0 || runCount < -1)
					throw new ArgumentOutOfRangeException("runCount", "runCount must be > 0 or equal to -1");

				_stopCountDown = runCount;
				_access = ManagedAccess;
				_handle = Handle;
				_singlePackets = WantSinglePackets;
				_stopRequest = false;
			}

			public int NumberOfRuns
			{
				set { _stopCountDown = value; }
			}
			public bool Stop
			{
				get { return _stopRequest;	}
				set { _stopRequest = value; }
			}
			/// <summary>
			/// 
			/// </summary>
			public void InventoryThreadProc()
			{
				StartEvent.WaitOne();
				do
				{
					//&&&&Result = _access.ReadInventory(_handle, _singlePackets);
					
					if (!String.IsNullOrEmpty(Result)) break;

					if (Stop) break;

				} while (_stopCountDown == -1 || // Run forever
						(Interlocked.Decrement(ref _stopCountDown) > 0)); // count down completed
			}

			/// <summary>
			/// 
			/// </summary>
			public void ReadThreadProc()
			{
				StartEvent.WaitOne();
				do
				{
					//&&&&Result = _access.ReadTagMemory(_handle, _memBank, _startLoc, _wordCount, _password);

					if (!String.IsNullOrEmpty(Result)) break;

					if (Stop) break;

				} while (_stopCountDown == -1 || // Run forever
						(Interlocked.Decrement(ref _stopCountDown) > 0)); // count down completed
			}

			/// <summary>
			/// 
			/// </summary>
			public void WriteThreadProc()
			{
				StartEvent.WaitOne();
				do
				{
					//&&&&Result = _access.WriteTagMemory(_handle, _memBank, _startLoc, _wordCount, _dataWord, _password);

					if (!String.IsNullOrEmpty(Result)) break;

					if (Stop) break;

				} while (_stopCountDown == -1 || // Run forever
						(Interlocked.Decrement(ref _stopCountDown) > 0)); // count down completed
			}

			/// <summary>
			/// 
			/// </summary>
			public void KillThreadProc()
			{
				StartEvent.WaitOne();
				do
				{
					//&&&&Result = _access.KillTag(_handle, _password, _killPassword);

					if (!String.IsNullOrEmpty(Result)) break;

					if (Stop) break;

				} while (_stopCountDown == -1 || // Run forever
						(Interlocked.Decrement(ref _stopCountDown) > 0)); // count down completed
			}

		}

		private enum LibraryMode
		{
			Initialized,
			LibraryFailedToInitialize,
		}





		private static int										_packetSleepMS		= MIN_SLEEP_TIME_MS;
		private static bool										_noTempFileAccess	= false;
//		private static bool										_enableLogging		= false;
//		private static string									_gpioConfigFilename = null;
		

		private rfidReaderID									_theReaderID		= null;
		private List<LakeChabotReader>							_readerList			= null;
		public Queue<PacketData.PacketWrapper>					PacketQueue			= null;
//		private LakeChabotReader.LinkProfileList				_profileList = null;

		private long											_sessionStartMS		= 0;
		private DateTime										_sessionStart		= DateTime.MinValue;

		private ManualResetEvent								QueueEvent			= new ManualResetEvent(false);

		private PacketArrayListGlue								_recentPacketList	= null;
		private TagCycleMatrix									_inventoryMatrix	= null;

		private DataFile<PropertyBag>							_propertyBagData	= null;
		private DataFile<TagInventory>							_tagInventoryData	= null;
		

		private SequentialDataFile<ReaderRequest>				_readerRequestData	= null;
		private SequentialDataFile<PacketStream>				_packetStreamData	= null;
		private SequentialDataFile<ReaderCommand> 				_readerCommandData	= null;
		private SequentialDataFile<ReaderAntennaCycle>			_readerAntennaCycleData	= null;
		private SequentialDataFile<AntennaPacket>				_antennaCycleData	= null;
		private SequentialDataFile<InventoryCycle>				_inventoryCycleData	= null;
		private SequentialDataFile<InventoryRound> 				_inventoryRoundData = null;
		private SequentialDataFile<TagRead>						_tagReadData		= null;
		private SequentialDataFile<ReadRate>					_readRateData		= null;
		private SequentialDataFile<BadPacket>					_badPacketData		= null;

		

		private FunctionControl									_control			= null;
		private BackgroundWorker								_bgdWorker			= null;
		private FileHandlerClass								_fileHandler		= null;

		private rfidTagList										_sessionTagList			= null;
		private rfidTagList										_requestTagList			= null;
		private rfidTagList										_periodTagList			= null;
		private rfidTagList										_commandTagList			= null;
		private rfidTagList										_antennaCycleTagList	= null;
		private rfidTagList										_antennaTagList			= null;
		private rfidTagList										_inventoryCycleTagList	= null;
		private rfidTagList										_inventoryRoundTagList	= null;


		private string			_staticReaderDir	= null;

		// common counts
		private volatile int _activeReaderCount		= 0;
		private volatile int _queueCount			= 0;
		private volatile int _commonAccessCount		= 0;
		private volatile int _commonRequestIndex	= -1;
		private volatile int _commonBadIndex		= -1;
		private volatile int _cRCErrors				= 0;
		private	volatile int _cmdEndErrors			= 0;
		
		// raw (from the reader) counts
		private volatile int _rawPacketCount		= 0;
		private volatile int _rawCommandCount		= 0;
		private volatile int _rawAntennaCycleCount	= 0;
		private volatile int _rawAntennaCount		= 0;
		private volatile int _rawInventoryCycleCount = 0;
		private volatile int _rawRoundCount			= 0;
		private volatile int _rawInventoryCount		= 0;
		
		
		
		
		// processed (from disk) count
		
		private volatile int _processedPacketIndex			= -1;
		private volatile int _processedCommandIndex			= -1;
		private volatile int _processedAntennaCycleIndex	= -1;
		private volatile int _processedAntennaIndex			= -1;
		private volatile int _processedInventoryCycleIndex	= -1;
		private volatile int _processedRoundIndex			= -1;
		private volatile int _processedInventoryIndex		= -1;

		private volatile int _processedCmdReadCount = 0;


		private int			_isDisposed				= 0;
		private int			_refreshRateMS			= 0;
		private int			_maxQueueSize			= 0;
		
		private UInt32?		_lastUsedAntenna		= null;
		private UInt32?		_lastCmdResult			= null;
		private int			_lastTagLocation		= 0;
		private int			_lastTagLength			= 1;
		private UInt16		_lastTagData			= 0;
		private uint		_lastPassword			= 0;
		private uint		_killPassword			= 0;

		private LakeChabotReader.TagMemoryBank _lastTagBank = LakeChabotReader.TagMemoryBank.EPC;

			
		private bool IsDisposed		{ get { return Interlocked.Equals(_isDisposed, 1);					} }
		public bool IsClosed		{ get { return IsDisposed;											} }

		public rfidReader.OperationMode Mode		
		{
			get { return rfidReader.OperationMode.BoundToReader; } 
		}

		
		private FileHandlerClass FileHandler
		{
			get
			{
				if (_fileHandler == null)
					_fileHandler = new FileHandlerClass(NoTempFileAccess);
				return _fileHandler;
			}
		}

		public DateTime SessionStartTime
		{
			get { return _sessionStart;		 }
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
			get { return new TimeSpan(TimeSpan.TicksPerMillisecond * ElapsedMilliseconds); }
		}

		public TimeSpan GetSessionRelativeSessionDuration(long elapsedMilliseconds)
		{
			return new TimeSpan(TimeSpan.TicksPerMillisecond * elapsedMilliseconds); 
		}

		public DateTime GetSessionRelativeDateTime(long pointInTimeMS)
		{
			return new DateTime(SessionStartTime.Ticks + (pointInTimeMS * TimeSpan.TicksPerMillisecond), DateTimeKind.Utc);
		}



		public UInt32 ReaderHandle
		{ 
			get 
			{
				if (Mode == rfidReader.OperationMode.BoundToReader)
                {
                    return _theReaderID.Handle;
                }

				return 0;
			}
		}

		public string Name				{ get { return _theReaderID == null ? null : _theReaderID.Name;		} }

		public FunctionControl FunctionController
										{ get { return _control;											} }



		public TableState TableResult
		{
			get
			{
				if (Mode == rfidReader.OperationMode.Static)
					return TableState.Ready;

				if (_noTempFileAccess) 
					return TableState.NotAvailable;

				if (_bgdWorker != null && _bgdWorker.IsBusy) 
					return TableState.Building;

				if (ProcessedPacketCount + _commonRequestIndex + BadPacketCount < FileHandler.TotalPacketCount - 1)
					return TableState.BuildRequired;
				
				return TableState.Ready;
			}
		}

		public int TagMemoryLocation
		{
			get { return _lastTagLocation; }
			set { _lastTagLocation = value; }
		}


		public int TagMemoryLength
		{
			get { return _lastTagLength; }
			set { _lastTagLength = value; }
		}

		public LakeChabotReader.TagMemoryBank TagBank
		{
			get { return _lastTagBank; }
			set { _lastTagBank = value; }
		}


		public UInt16 TagData
		{
			get { return _lastTagData; }
			set { _lastTagData = value; }
		}


		public uint TagPassword
		{
			get { return _lastPassword; }
			set { _lastPassword = value; }
		}

		

		public uint TagKillPassword
		{
			get { return _killPassword; }
			set { _killPassword = value; }
		}


		/// <summary>
		/// Create a bound reader
		/// </summary>
		/// <param name="ReaderToBind"></param>
		public VirtualReader(List<LakeChabotReader>  ReadersToBind)
		{

			InitReader();
			

			PacketQueue = new Queue<PacketData.PacketWrapper>(INITIAL_QUEUE_SIZE);
			_queueCount = 0;
			_activeReaderCount = 0;
			BindReaders(ReadersToBind);
		}


		private VirtualReader(bool noStartup)
		{
			
			InitReader();
			
		}


		~VirtualReader()
		{
			Dispose(false);
		}


		public static bool NoTempFileAccess
		{
			get { return _noTempFileAccess; }
			set { _noTempFileAccess = value; }
		}


//		public static bool EnableLogging
//		{
//			get { return _enableLogging; }
//			set { _enableLogging = value; }
//		}

		

		//public static string GpioConfigFilename
		//{
		//    get { return _gpioConfigFilename; }
		//    set { _gpioConfigFilename = value; }
		//}

		/*
		public static string ProfileListFilename
		{
			get { return _profileListFilename; }
			set { _profileListFilename = value; }
		}
		*/		
		

		public PacketArrayList RecentPacketList
		{
			get { return _recentPacketList; }
		}


		public int ActiveReaders
		{
			get { return _activeReaderCount; }
		}

		public void AddActiveReader()
		{
#pragma warning disable 420 // reference to a volatile field is valid for Interlocked call
			System.Threading.Interlocked.Increment(ref _activeReaderCount);
#pragma warning restore 420

		}
		
		public void RemoveActiveReader()
		{
#pragma warning disable 420 // reference to a volatile field is valid for Interlocked call
			System.Threading.Interlocked.Decrement(ref _activeReaderCount);
			if (_activeReaderCount < 0)
				System.Threading.Interlocked.Exchange(ref _activeReaderCount, 0);
#pragma warning restore 420
			
		}


		public void ClearActiveReaders()
		{
#pragma warning disable 420 // reference to a volatile field is valid for Interlocked call
			System.Threading.Interlocked.Exchange(ref _activeReaderCount, 0);
#pragma warning restore 420
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
			private set { _commonBadIndex = value - 1; }
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
			private set { _processedPacketIndex = value - 1; }
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
		public  void SettingsChanged()
		{
			//TODO implement SettingsChanged
			throw new NotImplementedException("SettingsChanged()");
		}

		public void SetProperty(string name, object value)
		{
			KeyList<PropertyBag> key = new KeyList<PropertyBag>("propName", name);
		
			PropertyBag myBag = new PropertyBag();

			if (PropertyBagData.Contains(key))
			{
				myBag = (PropertyBag)PropertyBagData[key];
				myBag.value = value;
				PropertyBagData[key] = myBag;
			}
			else
			{
				myBag.propName	= name;
				myBag.value		= value;
				PropertyBagData.Add(myBag);
			}

		}

		public string GetPropertyAsString(string name)
		{
			KeyList<PropertyBag> key = new KeyList<PropertyBag>("propName", name);

			PropertyBag myBag = new PropertyBag();

			if (PropertyBagData.Contains(key))
			{
				myBag = (PropertyBag)PropertyBagData[key];
				if (myBag == null || myBag.Value == null)
					return String.Empty;

				return myBag.value.ToString();
			}
			return String.Empty;
		}


		public string ResetMac()
		{
			throw new NotImplementedException("Virtual Reader");
		}

		public string ClearError()
		{
			return null;
		}

		
		public void ClearSession()
		{
			LakeChabotReader.PacketLogger.Clear();

			#pragma warning disable 420 // reference to a volatile field is valid for Interlocked call
			System.Threading.Interlocked.Exchange(ref _commonRequestIndex, -1);
			System.Threading.Interlocked.Exchange(ref _commonBadIndex, -1);
			System.Threading.Interlocked.Exchange(ref _activeReaderCount, 0);
			System.Threading.Interlocked.Exchange(ref _rawPacketCount, 0);
			System.Threading.Interlocked.Exchange(ref _rawCommandCount, 0);
			System.Threading.Interlocked.Exchange(ref _rawAntennaCycleCount, 0);
			System.Threading.Interlocked.Exchange(ref _rawAntennaCount, 0);
			System.Threading.Interlocked.Exchange(ref _rawInventoryCycleCount, 0);
			System.Threading.Interlocked.Exchange(ref _rawRoundCount, 0);
			System.Threading.Interlocked.Exchange(ref _rawInventoryCount, 0);
			System.Threading.Interlocked.Exchange(ref _commonAccessCount, 0);
			System.Threading.Interlocked.Exchange(ref _cRCErrors, 0);

			System.Threading.Interlocked.Exchange(ref _processedPacketIndex, -1);
			System.Threading.Interlocked.Exchange(ref _processedCommandIndex, -1);
			System.Threading.Interlocked.Exchange(ref _processedAntennaCycleIndex, -1);
			System.Threading.Interlocked.Exchange(ref _processedAntennaIndex, -1);
			System.Threading.Interlocked.Exchange(ref _processedInventoryCycleIndex, -1);
			System.Threading.Interlocked.Exchange(ref _processedRoundIndex, -1);
//			System.Threading.Interlocked.Exchange(ref _processedEndRoundIndex, -1);
		
			System.Threading.Interlocked.Exchange(ref _processedInventoryIndex, -1);
			System.Threading.Interlocked.Exchange(ref _processedCmdReadCount, 0);
			System.Threading.Interlocked.Exchange(ref _maxQueueSize, -1);
			#pragma warning restore 420

//			_stopwatch.Reset();
//			_stopwatch.Start();
			DateTime temp = DateTime.UtcNow;
			_sessionStartMS = HighResolutionTimer.Milliseconds;
//			_sessionStart = new DateTime(temp.Year, temp.Month, temp.Day, temp.Hour, temp.Minute, temp.Second, (int)_stopwatch.ElapsedMilliseconds, DateTimeKind.Utc);
			_sessionStart = new DateTime(temp.Year, temp.Month, temp.Day, temp.Hour, temp.Minute, temp.Second, Math.Min(999, (int)ElapsedMilliseconds), DateTimeKind.Utc);

			_inventoryMatrix.Clear();

			_sessionTagList.Clear();
			_requestTagList.Clear();
			_periodTagList.Clear();
			_commandTagList.Clear();
			_antennaCycleTagList.Clear();
			_antennaTagList.Clear();
			_inventoryCycleTagList.Clear();
			_inventoryRoundTagList.Clear();


			LastUsedAntenna = null;
			LastCommandResult = null;

			RecentPacketList.Clear();

			PropertyBagData.Clear();
			ReaderRequestData.Clear();
			PacketStreamData.Clear();
			ReaderCommandData.Clear();
			ReaderAntennaCycleData.Clear();
			AntennaCycleData.Clear();
			TagInventoryData.Clear();
			InventoryRoundData.Clear();
			InventoryCycleData.Clear();
			TagReadData.Clear();
			ReadRateData.Clear();
			BadPacketData.Clear();
			

			if (_fileHandler != null) _fileHandler.Close();
			_fileHandler = null;
		}



        public VirtualReader BindReaders( List<LakeChabotReader> Readers )
        {
            foreach ( LakeChabotReader reader in Readers )
            {
                Console.WriteLine( "VR adding Reader " + reader.GetHashCode( ) );
            }

            if ( IsDisposed )
            {
                throw new ObjectDisposedException( "VirtualReader" );
            }

            if ( Mode == rfidReader.OperationMode.Static )
            {
                throw new rfidException( rfidErrorCode.CannotBindToStaticReader );
            }

            if ( Readers == null )
            {
                throw new ArgumentNullException( "Readers" );
            }

            if ( Readers.Count < 2 )
            {
                throw new ArgumentOutOfRangeException( "Readers", "Virtual Reader Requires at least two physical readers." );
            }


            CreateDataSet( );

            _readerList = Readers;
            _theReaderID = new rfidReaderID(rfidReaderID.ReaderType.MTI, 0xFFFF, "Virtual Reader", "Virtual Reader Device", "Virtual", rfidReaderID.LocationTypeID.LocalDevice);
            FunctionController.Name = Name;



            _sessionTagList        = new rfidTagList( );
            _requestTagList        = new rfidTagList( );
            _periodTagList         = new rfidTagList( );
            _commandTagList        = new rfidTagList( );
            _antennaCycleTagList   = new rfidTagList( );
            _antennaTagList        = new rfidTagList( );
            _inventoryCycleTagList = new rfidTagList( );
            _inventoryRoundTagList = new rfidTagList( );


            DateTime temp = DateTime.UtcNow;

            _sessionStartMS = HighResolutionTimer.Milliseconds;
            _sessionStart = new DateTime( temp.Year, temp.Month, temp.Day, temp.Hour, temp.Minute, temp.Second, Math.Min( 999, ( int ) ElapsedMilliseconds ), DateTimeKind.Utc );

            return this;
		}



		private void CreateDataSet()
		{
			if (IsDisposed)
				throw new ObjectDisposedException("VirtualReader");

			new ReaderRequest().ValidateOrdinalValues();
			new PacketStream().ValidateOrdinalValues();
			new ReaderCommand().ValidateOrdinalValues();
			new ReaderAntennaCycle().ValidateOrdinalValues();
			new AntennaPacket().ValidateOrdinalValues();
			new InventoryCycle().ValidateOrdinalValues();
			new InventoryRound().ValidateOrdinalValues();
			new TagRead().ValidateOrdinalValues();
			new TagInventory().ValidateOrdinalValues();
			new BadPacket().ValidateOrdinalValues();
			new ReadRate().ValidateOrdinalValues();

			_recentPacketList		= new PacketArrayListGlue(this);
			_inventoryMatrix		= new TagCycleMatrix(EMA_PERIOD);

			_propertyBagData		= new DataFile<PropertyBag>(RFID.RFIDInterface.Properties.Settings.Default.PropertyBagPageSize);
			_tagInventoryData		= new DataFile<TagInventory>(RFID.RFIDInterface.Properties.Settings.Default.TagInventoryPageSize);
			
			_readerRequestData		= new SequentialDataFile<ReaderRequest>(RFID.RFIDInterface.Properties.Settings.Default.ReaderRequestPageSize);
			_packetStreamData		= new SequentialDataFile<PacketStream>(RFID.RFIDInterface.Properties.Settings.Default.PacketStreamPageSize);
			_readerCommandData		= new SequentialDataFile<ReaderCommand>(RFID.RFIDInterface.Properties.Settings.Default.ReaderCommandPageSize);
			_readerAntennaCycleData	= new SequentialDataFile<ReaderAntennaCycle>(RFID.RFIDInterface.Properties.Settings.Default.ReaderCyclePageSize);
			_antennaCycleData		= new SequentialDataFile<AntennaPacket>(RFID.RFIDInterface.Properties.Settings.Default.AntennaCyclePageSize);
			_inventoryCycleData		= new SequentialDataFile<InventoryCycle>(RFID.RFIDInterface.Properties.Settings.Default.InventoryCyclePageSize);
			_inventoryRoundData		= new SequentialDataFile<InventoryRound>(RFID.RFIDInterface.Properties.Settings.Default.InventoryRoundPageSize);
			_tagReadData			= new SequentialDataFile<TagRead>(RFID.RFIDInterface.Properties.Settings.Default.TagReadPageSize);
			_readRateData			= new SequentialDataFile<ReadRate>(RFID.RFIDInterface.Properties.Settings.Default.ReadRatePageSize);
			_badPacketData			= new SequentialDataFile<BadPacket>(RFID.RFIDInterface.Properties.Settings.Default.BadPacketPageSize);


		}





		public void CloseReader()
		{
			Dispose();
		}

		public ReportBase SaveDataToFile(Object context, BackgroundWorker worker, int refreshMS, string Filename)
		{
			if (IsDisposed)
				throw new ObjectDisposedException("VirtualReader");

			if (TableResult != TableState.Ready)
				//return new rfidOperationReport(context, OperationOutcome.FailByContext, new rfidException(rfidErrorCode.TablesAreNotReady, "You must build the post-capture views before saving."));
				return new rfidSimpleReport(context, OperationOutcome.FailByContext, new rfidException(rfidErrorCode.TablesAreNotReady, "You must build the post-capture views before saving."));
				

			try
			{

				rfidSimpleReport report = new rfidSimpleReport(context, HighResolutionTimer.Milliseconds);

				string tempDirectory  = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), Path.GetFileNameWithoutExtension(Path.GetRandomFileName()))).FullName;

				string[] fileNameArray =
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

				PropertyBagData.CommitPageAndCopyFile(fileNameArray[0]);


				AntennaCycleData.CommitPageAndCopyFile(fileNameArray[1]);
				worker.ReportProgress(10, report.GetProgressReport(HighResolutionTimer.Milliseconds));


				InventoryRoundData.CommitPageAndCopyFile(fileNameArray[2]);
				worker.ReportProgress(15, report.GetProgressReport(HighResolutionTimer.Milliseconds));
				Thread.Sleep(20);

				PacketStreamData.CommitPageAndCopyFile(fileNameArray[3]);
				worker.ReportProgress(20, report.GetProgressReport(HighResolutionTimer.Milliseconds));
				Thread.Sleep(20);

				ReaderCommandData.CommitPageAndCopyFile(fileNameArray[4]);
				worker.ReportProgress(20, report.GetProgressReport(HighResolutionTimer.Milliseconds));
				Thread.Sleep(20);

				ReaderAntennaCycleData.CommitPageAndCopyFile(fileNameArray[5]);
				worker.ReportProgress(30, report.GetProgressReport(HighResolutionTimer.Milliseconds));

				ReaderRequestData.CommitPageAndCopyFile(fileNameArray[6]);
				worker.ReportProgress(30, report.GetProgressReport(HighResolutionTimer.Milliseconds));
				Thread.Sleep(20);

				TagInventoryData.CommitPageAndCopyFile(fileNameArray[7]);
				worker.ReportProgress(40, report.GetProgressReport(HighResolutionTimer.Milliseconds));
				Thread.Sleep(20);

				TagReadData.CommitPageAndCopyFile(fileNameArray[8]);
				worker.ReportProgress(40, report.GetProgressReport(HighResolutionTimer.Milliseconds));
				Thread.Sleep(20);

				ReadRateData.CommitPageAndCopyFile(fileNameArray[9]);
				worker.ReportProgress(40, report.GetProgressReport(HighResolutionTimer.Milliseconds));
				Thread.Sleep(20);

				BadPacketData.CommitPageAndCopyFile(fileNameArray[10]);
				worker.ReportProgress(50, report.GetProgressReport(HighResolutionTimer.Milliseconds));

				InventoryCycleData.CommitPageAndCopyFile(fileNameArray[11]);
				worker.ReportProgress(60, report.GetProgressReport(HighResolutionTimer.Milliseconds));
				Thread.Sleep(20);



				String zipFileName = Path.GetTempFileName();
				File.Delete(zipFileName);
				zipFileName = Path.ChangeExtension(zipFileName, ".rfi");

				FileCompressor.Compress(zipFileName, true, fileNameArray);

				worker.ReportProgress(80, report.GetProgressReport(HighResolutionTimer.Milliseconds));

				File.Copy(zipFileName, Filename, true);

				worker.ReportProgress(90, report.GetProgressReport(HighResolutionTimer.Milliseconds));
				
				File.Delete(zipFileName);
				Directory.Delete(tempDirectory);

				report.OperationCompleted(OperationOutcome.Success, String.Format("Successfuly opened {0}", Filename), HighResolutionTimer.Milliseconds);

				return report;

			}
			catch (Exception e)
			{
				return new rfidSimpleReport(context, OperationOutcome.FailByApplicationError, e);
			}
		}

	

		/// <summary>
		/// 
		/// </summary>
		public ReportBase BuildTables(Object context, BackgroundWorker worker, int refreshMS, StopDependentReadersDelegate stopDelegate, List<DictionaryEntry[]> DataSourceList)
		{
			Debug.WriteLine(String.Format("{0}() ThreadID = {1}", System.Reflection.MethodInfo.GetCurrentMethod().Name, Thread.CurrentThread.ManagedThreadId));

			if (IsDisposed)
				throw new ObjectDisposedException("VirtualReader");

			if (Mode != rfidReader.OperationMode.BoundToReader)
				return new rfidSimpleReport(context, OperationOutcome.FailByContext,
					new rfidException(rfidErrorCode.ReaderIsNotBound, "Reader must be bound before invoking BuildTables."));

			if (_control.State != FunctionControl.FunctionState.Idle)
				return new rfidSimpleReport(context, OperationOutcome.FailByContext, new Exception("Cannot build tables, the prior task has not completed"));

			if (null == worker)
				return new rfidSimpleReport(context, OperationOutcome.FailByApplicationError, new ArgumentNullException("worker", "BackgroundWorker is required"));

			if (refreshMS < MIN_REFRESH_MS || refreshMS > MAX_REFRESH_MS)
				return new rfidSimpleReport(context, OperationOutcome.FailByApplicationError, new ArgumentOutOfRangeException("refreshMS", refreshMS, string.Format("Value must be between {0} and {1}", MIN_REFRESH_MS, MAX_REFRESH_MS)));

			_refreshRateMS = refreshMS;
			_bgdWorker = worker;
			_periodTagList.Clear();
			_requestTagList.Clear();
			rfidOperationReport report = new rfidOperationReport(
																	context, 
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
																	SessionDuration
																);


			DateTime ReportDue = DateTime.Now.AddMilliseconds(refreshMS);
			OperationOutcome outcome = OperationOutcome.Success;
			_control.StartAction();
			DateTime now = DateTime.Now;

			while (ActiveReaders > 0)
			{

				Thread.Sleep(refreshMS);
				now = DateTime.Now;

				if (ReportDue.Ticks <= now.Ticks)
				{
                    //!!
					Debug.WriteLine(String.Format("Reporing Progress Now (Elapsed Milliseconds {0})", ElapsedMilliseconds));
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
																			SessionDuration
																		)
											);
					ReportDue = now.AddMilliseconds(refreshMS);
				}
				if (FunctionController.GetActionRequest() == FunctionControl.RequestedAction.Abort)
				{
					string abortError = null;
					try
					{
						//abortError = _managedAccess.AbortOperation((int)ReaderHandle);
						abortError = stopDelegate(true);
						outcome = OperationOutcome.FailByUserAbort;
					}
					catch (Exception e)
					{
						abortError = String.Format("Error attempting to abort: {0}", e.Message);
						Debug.WriteLine(abortError);
						Debug.Assert(abortError == null);
					}
					break;
				}

				if (FunctionController.GetActionRequest() == FunctionControl.RequestedAction.Stop)
				{
					string stopError = null;
					try
					{
						stopError = stopDelegate(false);
						outcome = OperationOutcome.SuccessWithUserCancel;
					}
					catch (Exception e)
					{
						stopError = String.Format("Error attempting to stop: {0}", e.Message);
						Debug.WriteLine(stopError);
						Debug.Assert(stopError == null);
					}
					if (stopError != null)
					{
						// Try to abort
					}
					break;
				}

				if (FunctionController.State == FunctionControl.FunctionState.Paused)
				{
					do
					{
						Thread.Sleep(refreshMS);
					} while (FunctionController.State == FunctionControl.FunctionState.Paused);
				}
			}



			/*
			int totalPacketsToProcess = FileHandler.TotalPacketCount - ProcessedPacketCount;
			int localPacketCount = 0;

			PacketData.PacketWrapper envelope;
			
			while ((envelope = FileHandler.GetPendingPacket()) != null)
			{
				localPacketCount++;
				ProcessPacket(envelope);
				DateTime now = DateTime.Now;
				if (ReportDue.Ticks <= now.Ticks)
				{
					_bgdWorker.ReportProgress(
												(int)(100 * ((float)localPacketCount / (float)totalPacketsToProcess)), 
												report.GetProgressReport(
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
																			SessionDuration
																		)
											);
					_periodTagList.Clear();
					ReportDue = now.AddMilliseconds(refreshMS);
				}
				FunctionControl.RequestedAction action = FunctionController.GetActionRequest();
				if (action != FunctionControl.RequestedAction.Continue)
				{
					break;
				}
			}
			*/


			if (_control.State == FunctionControl.FunctionState.Running)
			{
				foreach (DictionaryEntry entry in DataSources)
				{
					if (ReportDue.Ticks <= now.Ticks)
					{
						//Debug.WriteLine(String.Format("Reporing Progress Now (Elapsed Milliseconds {0})", ElapsedMilliseconds));
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
																				SessionDuration
																			)
												);
						ReportDue = now.AddMilliseconds(refreshMS);
					}

					switch (entry.Key as string)
					{
						case "PropertyBag":
							break;

						case "ReaderRequest":
							MergeSequentialDataFiles(ReaderRequestData, DataSourceList);
							break;

						case "PacketStream":
							MergeSequentialDataFiles<PacketStream>(PacketStreamData, DataSourceList);
							break;

						case "ReaderCommand":
							MergeSequentialDataFiles<ReaderCommand>(ReaderCommandData, DataSourceList);
							break;

						case "ReaderAntennaCycle":
							MergeSequentialDataFiles<ReaderAntennaCycle>(ReaderAntennaCycleData, DataSourceList);
							break;

						case "AntennaPacket":
							MergeSequentialDataFiles<AntennaPacket>(AntennaCycleData, DataSourceList);
							break;

						case "InventoryCycle":
							MergeSequentialDataFiles<InventoryCycle>(InventoryCycleData, DataSourceList);
							break;

						case "InventoryRound":
							MergeSequentialDataFiles<InventoryRound>(InventoryRoundData, DataSourceList);
							break;

						case "TagRead":
							MergeSequentialDataFiles<TagRead>(TagReadData, DataSourceList);
							break;


						case "TagInventory":
							break;

						case "BadPacket":
							MergeSequentialDataFiles<BadPacket>(BadPacketData, DataSourceList);
							break;

						case "ReadRate":
							MergeSequentialDataFiles<ReadRate>(ReadRateData, DataSourceList);
							break;

						default:
							break;
					}
				}
			}

			// Update Counts
			foreach (DictionaryEntry[] var in DataSourceList)
			{
				foreach (DictionaryEntry entry in var)
				{
					switch ((string)entry.Key)
					{
						case "PacketStream":
							ProcessedPacketCount += ((SequentialDataFile<PacketStream>)entry.Value).Count;
							break;

						case "BadPacket":
							BadPacketCount += ((SequentialDataFile<BadPacket>)entry.Value).Count;
							break;

						default:
							break;
					}
				}
			}
			switch (_control.State)
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


			_control.Finished();

			report.OperationCompleted(outcome, 
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
										SessionDuration);

			
			return report;
		}

		private static void MergeSequentialDataFiles<T>(SequentialDataFile<T> mergedData, List<DictionaryEntry[]> DataSourceList) where T : DatabaseRowTemplate, new()
		{
			mergedData.Clear();
			T getName = new T();
			List<SequentialDataFile<T>> dataFileList = new List<SequentialDataFile<T>>();
			foreach (DictionaryEntry[] DictArray in DataSourceList)
			{
				foreach (DictionaryEntry e in DictArray)
				{
					if ((string)e.Key == getName.Name)
					{
						dataFileList.Add(e.Value as SequentialDataFile<T>);
						break;
					}
				}
			}
			int count = dataFileList.Count;
			System.Diagnostics.Debug.Assert(count > 1);
			
			int[] indexArray = new int[count];
			T[] dataArray = new T[count];

			while (true)
			{
				for (int i = 0; i < count; i++)
				{
					if (indexArray[i] < dataFileList[i].Count)
					{
						dataArray[i] = dataFileList[i][indexArray[i]];
					}
					else
					{
						dataArray[i] = null;
					}
				}

				int lowboy = -1;
				// find first non null
				for (int i = 0; i < count; i++)
				{
					if (dataArray[i] != null)
					{
						lowboy = i;
						break;
					}
				}
				if (lowboy == -1)
					return;

				for (int i = lowboy + 1; i < count; i++)
				{
					if (dataArray[i] != null)
					{
						if (dataArray[lowboy].GetTimestamp() > dataArray[i].GetTimestamp())
						{
						    lowboy = i;
						}
					}
				}

				mergedData.Add(dataArray[lowboy]);
				indexArray[lowboy]++;
			}
		}



		/// <summary>
		/// 
		/// </summary>
		public static ReportBase LoadFileIntoStaticReader(Object context, BackgroundWorker worker, int refreshMS, string zipFileName)
		{
			Debug.WriteLine(String.Format("{0}() ThreadID = {1}", System.Reflection.MethodInfo.GetCurrentMethod().Name, Thread.CurrentThread.ManagedThreadId));

			
			if (null == worker)
				return new rfidSimpleReport(context, OperationOutcome.FailByApplicationError, new ArgumentNullException("worker", "BackgroundWorker is required"));

			if (refreshMS < MIN_REFRESH_MS || refreshMS > MAX_REFRESH_MS)
				return new rfidSimpleReport(context, OperationOutcome.FailByApplicationError, new ArgumentOutOfRangeException("refreshMS", refreshMS, string.Format("Value must be between {0} and {1}", MIN_REFRESH_MS, MAX_REFRESH_MS)));

			if (zipFileName == null)
				return new rfidSimpleReport(context, OperationOutcome.FailByApplicationError, new ArgumentNullException("zipFileName"));

			if (!File.Exists(zipFileName))
				return new rfidSimpleReport(context, OperationOutcome.FailByApplicationError, new ArgumentOutOfRangeException("ziFileName", zipFileName, "File does not exit."));

			try
			{

				rfidSimpleReport report = new rfidSimpleReport(context, 0);

				VirtualReader reader = new VirtualReader(false);

				reader._staticReaderDir  = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), Path.GetFileNameWithoutExtension(Path.GetRandomFileName()))).FullName;

				worker.ReportProgress(10, report.GetProgressReport(HighResolutionTimer.Milliseconds));

				FileCompressor.Decompress(zipFileName, reader._staticReaderDir);

				worker.ReportProgress(20, report.GetProgressReport(HighResolutionTimer.Milliseconds));

				reader._propertyBagData	= new DataFile<PropertyBag>(RFID.RFIDInterface.Properties.Settings.Default.PropertyBagPageSize, Path.Combine(reader._staticReaderDir, "PropertyBagData.df"));

				reader._antennaCycleData = new SequentialDataFile<AntennaPacket>(RFID.RFIDInterface.Properties.Settings.Default.AntennaCyclePageSize, Path.Combine(reader._staticReaderDir, "AntennaCycleData.df"));

				worker.ReportProgress(30, report.GetProgressReport(HighResolutionTimer.Milliseconds));

				reader._inventoryRoundData	= new SequentialDataFile<InventoryRound>(RFID.RFIDInterface.Properties.Settings.Default.InventoryRoundPageSize, Path.Combine(reader._staticReaderDir, "InventoryRoundData.df"));

				worker.ReportProgress(40, report.GetProgressReport(HighResolutionTimer.Milliseconds));

				reader._packetStreamData = new SequentialDataFile<PacketStream>(RFID.RFIDInterface.Properties.Settings.Default.PacketStreamPageSize, Path.Combine(reader._staticReaderDir, "PacketStreamData.df"));

				worker.ReportProgress(50, report.GetProgressReport(HighResolutionTimer.Milliseconds));

				reader._readerCommandData = new SequentialDataFile<ReaderCommand>(RFID.RFIDInterface.Properties.Settings.Default.ReaderCommandPageSize, Path.Combine(reader._staticReaderDir, "ReaderCommandData.df"));

				reader._readerAntennaCycleData = new SequentialDataFile<ReaderAntennaCycle>(RFID.RFIDInterface.Properties.Settings.Default.ReaderCyclePageSize, Path.Combine(reader._staticReaderDir, "ReaderCycleData.df"));

				worker.ReportProgress(60, report.GetProgressReport(HighResolutionTimer.Milliseconds));

				reader._readerRequestData = new SequentialDataFile<ReaderRequest>(RFID.RFIDInterface.Properties.Settings.Default.ReaderRequestPageSize, Path.Combine(reader._staticReaderDir, "ReaderRequestData.df"));

				worker.ReportProgress(70, report.GetProgressReport(HighResolutionTimer.Milliseconds));

				reader._tagInventoryData = new DataFile<TagInventory>(RFID.RFIDInterface.Properties.Settings.Default.TagInventoryPageSize, Path.Combine(reader._staticReaderDir, "TagInventoryData.df"));

				reader._tagReadData	= new SequentialDataFile<TagRead>(RFID.RFIDInterface.Properties.Settings.Default.TagReadPageSize, Path.Combine(reader._staticReaderDir, "TagReadData.df"));

				worker.ReportProgress(80, report.GetProgressReport(HighResolutionTimer.Milliseconds));

				reader._readRateData = new SequentialDataFile<ReadRate>(RFID.RFIDInterface.Properties.Settings.Default.ReadRatePageSize, Path.Combine(reader._staticReaderDir, "ReadRateData.df"));

				reader._badPacketData = new SequentialDataFile<BadPacket>(RFID.RFIDInterface.Properties.Settings.Default.BadPacketPageSize, Path.Combine(reader._staticReaderDir, "BadPacketData.df"));

				reader._inventoryCycleData = new SequentialDataFile<InventoryCycle>(RFID.RFIDInterface.Properties.Settings.Default.InventoryCyclePageSize, Path.Combine(reader._staticReaderDir, "InventoryCycleData.df"));
				
				worker.ReportProgress(90, report.GetProgressReport(HighResolutionTimer.Milliseconds));


                reader._theReaderID = new rfidReaderID(rfidReaderID.ReaderType.MTI, zipFileName);
				reader._recentPacketList = new PacketArrayListGlue(reader);
				
				report.NewReader = reader;

				report.OperationCompleted(OperationOutcome.Success, HighResolutionTimer.Milliseconds);

				return report;
			}
			catch (Exception e)
			{
				return new rfidSimpleReport(context, OperationOutcome.FailByApplicationError, e);			
			}
		}


		/// <summary>
		/// 
		/// </summary>
		public ReportBase ReadInventory(Object context, BackgroundWorker worker, int refreshMS, StopDependentReadersDelegate stopDelegate)
		{
			#if DEBUG
			System.Diagnostics.Debug.WriteLine(String.Format("{0}() ThreadID = {1}", System.Reflection.MethodInfo.GetCurrentMethod().Name, Thread.CurrentThread.ManagedThreadId));
			#endif
			
			if (IsDisposed)
				throw new ObjectDisposedException("VirtualReader");

			if (Mode != rfidReader.OperationMode.BoundToReader)
				return new rfidSimpleReport(context, OperationOutcome.FailByContext,
					new rfidException(rfidErrorCode.ReaderIsNotBound, "Reader must be bound before inventory can be read."));

			if (_control.State != FunctionControl.FunctionState.Idle)
				return new rfidSimpleReport(context, OperationOutcome.FailByContext, new Exception("Cannot read the inventory, the prior task has not completed"));

			if (null == worker)
				return new rfidSimpleReport(context, OperationOutcome.FailByApplicationError, new ArgumentNullException("worker", "BackgroundWorker is required"));

			if (refreshMS < MIN_REFRESH_MS || refreshMS > MAX_REFRESH_MS)
				return new rfidSimpleReport(context, OperationOutcome.FailByApplicationError, new ArgumentOutOfRangeException("refreshMS", refreshMS, string.Format("Value must be between {0} and {1}", MIN_REFRESH_MS, MAX_REFRESH_MS)));


			// Single Inventory is never in continuous mode
//			LakeChabotReader.ReaderOperationModeValue priorMode = ReaderOperationMode;
//			if (priorMode != LakeChabotReader.ReaderOperationModeValue.Discontinuous)
//			{
//				ReaderOperationMode = LakeChabotReader.ReaderOperationModeValue.Discontinuous;
//			}
			
			_refreshRateMS = refreshMS;
			_bgdWorker = worker;
			_requestTagList.Clear();
			_periodTagList.Clear();
			rfidOperationReport report = new rfidOperationReport(
																	context, 
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
																	SessionDuration
																);
			
			_control.StartAction();

			//_managedAccess.Callback = PacketCallBackFromReader;

			PerformanceCounter processorUtilizationCounter = null;

			long t1 = HighResolutionTimer.Microseconds;
			try
			{
				processorUtilizationCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total", ".");
			}
			catch (Exception) 
			{
				Debug.WriteLine("Unable to start processorUtilizationCounter");
			}
			t1 = HighResolutionTimer.Microseconds - t1;
			Debug.WriteLine(String.Format("processorUtilizationCounter startup time {0} us", t1));


			int notused = FileHandler.TotalPacketCount; // Make sure the file is created;


			ReaderRequest readerRequest = new ReaderRequest();
			readerRequest.reader				= _theReaderID.Name;

			#pragma warning disable 420 // reference to a volatile field is valid for Interlocked call
			readerRequest.requestSequence		= System.Threading.Interlocked.Increment(ref _commonRequestIndex);
			#pragma warning restore 420

			readerRequest.requestName			= rfidReader.GetNameForRequest(rfidReader.ReaderRequestType.GetInventory);
			readerRequest.startTime				= GetSessionRelativeDateTime(report.StartTimeMS);
			readerRequest.requestStartTime		= report.StartTimeMS;
			readerRequest.startingPacketCount	= RawPacketCount;
			readerRequest.startingTagCount		= RawInventoryCount;

			MemoryStream data = new MemoryStream();
			readerRequest.WriteTo(data);
			PacketData.PacketWrapper pseudoPacket = new PacketData.PacketWrapper(new PacketData.CommandPsuedoPacket(readerRequest.requestName, data.ToArray()), PacketData.PacketType.U_N_D_F_I_N_E_D);
			pseudoPacket.IsPseudoPacket = true;
			pseudoPacket.ReaderName = _theReaderID.Name;
			pseudoPacket.ReaderIndex = (int)_theReaderID.Handle;
			FileHandler.WritePacket(pseudoPacket);

			DateTime ReportDue = DateTime.Now.AddMilliseconds(refreshMS);
			ReaderInterfaceThreadClass.StartEvent.Set();
			OperationOutcome outcome = OperationOutcome.Success;


			while (ActiveReaders > 0)
			{
				CounterSample sample = CounterSample.Empty;
				if (processorUtilizationCounter != null)
				{
					try
					{
						sample = processorUtilizationCounter.NextSample();
					}
					catch (Exception) { }
				}				
				ProcessQueuedPackets();

				QueueEvent.WaitOne(30, true);
				QueueEvent.Reset();

				DateTime now = DateTime.Now;

				if (ReportDue.Ticks <= now.Ticks)
				{
					//Debug.WriteLine(String.Format("Reporing Progress Now (Elapsed Milliseconds {0})", ElapsedMilliseconds));
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
																			SessionDuration
																		)
											);
					_periodTagList.Clear();
					ReportDue = now.AddMilliseconds(refreshMS);
				}


				if (FunctionController.GetActionRequest() == FunctionControl.RequestedAction.Abort)
				{
					string abortError = null;
					try
					{
						//abortError = _managedAccess.AbortOperation((int)ReaderHandle);
						abortError = stopDelegate(true);
						outcome = OperationOutcome.FailByUserAbort;
					}
					catch (Exception e)
					{
						abortError = String.Format("Error attempting to abort: {0}", e.Message);
						Debug.WriteLine(abortError);
						Debug.Assert(abortError == null);
					}
					break;
				}

				if (FunctionController.GetActionRequest() == FunctionControl.RequestedAction.Stop)
				{
					string stopError = null;
					try
					{
						stopError = stopDelegate(false);
						outcome = OperationOutcome.SuccessWithUserCancel;
					}
					catch (Exception e)
					{
						stopError = String.Format("Error attempting to stop: {0}", e.Message);
						Debug.WriteLine(stopError);
						Debug.Assert(stopError == null);
					}
					if (stopError != null)
					{
						// Try to abort
					}
					break;
				}

				if (FunctionController.State == FunctionControl.FunctionState.Paused)
				{
					do
					{
						Thread.Sleep(refreshMS);
					} while (FunctionController.State == FunctionControl.FunctionState.Paused);
				}


				float processorUtilization = 0;

				if (processorUtilizationCounter != null && sample == CounterSample.Empty)
				{
					try
					{
						processorUtilization = CounterSample.Calculate(sample, processorUtilizationCounter.NextSample());
					}
					catch (Exception) { }
				}
//				Debug.WriteLine(String.Format("Processor Util: {0}", processorUtilization));

				if (processorUtilization > (float)TARGET_MAX_USAGE_PERCENT)
					_packetSleepMS += SLEEP_ADD_SUBTRACT_MS;
				else
					_packetSleepMS -= _packetSleepMS <= MIN_SLEEP_TIME_MS ? 0 : SLEEP_ADD_SUBTRACT_MS;
			}


			// Get any leftover packets
			ProcessQueuedPackets();

			if (LastCommandResult != 0) ClearError();


//            string result = threadClass.Result;
//            if (!(result == null || result == ""))
//            {
//                switch (_control.State)
//                {
//                case FunctionControl.FunctionState.Stopping:
//                    outcome = OperationOutcome.SuccessWithUserCancel;
//                    break;

//                case FunctionControl.FunctionState.Aborting:
//                    outcome = OperationOutcome.FailByUserAbort;
//                    break;

//                case FunctionControl.FunctionState.Running:
//                    outcome = OperationOutcome.FailByReaderError;
//                    report.ErrorMessage = result;
//                    break;

//                case FunctionControl.FunctionState.Idle:
//                case FunctionControl.FunctionState.Paused:
//                case FunctionControl.FunctionState.Unknown:
//                default:
//                    outcome = OperationOutcome.Unknown;
//                    break;
//                }
//			}


			_control.Finished();

			report.OperationCompleted(
										outcome,
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
										SessionDuration
									);


			// reset the mode

			return report;
		}




		public ReportBase MonitorPulse(Object context, BackgroundWorker worker, int refreshMS, StopDependentReadersDelegate stopDelegate)
		{
			Debug.WriteLine(String.Format("{0}() ThreadID = {1}", System.Reflection.MethodInfo.GetCurrentMethod().Name, Thread.CurrentThread.ManagedThreadId));

			if (IsDisposed)
				throw new ObjectDisposedException("LakeChabotReader");

			if (Mode != rfidReader.OperationMode.BoundToReader)
				return new rfidSimpleReport(context, OperationOutcome.FailByContext,
                    new rfidException(rfidErrorCode.ReaderIsNotBound, "Reader must be bound before Pulse."));

			if (_control.State != FunctionControl.FunctionState.Idle)
                return new rfidSimpleReport(context, OperationOutcome.FailByContext, new Exception("Cannot read the Pulse, the prior task has not completed"));

			if (null == worker)
				return new rfidSimpleReport(context, OperationOutcome.FailByApplicationError, new ArgumentNullException("worker", "BackgroundWorker is required"));

			if (refreshMS < MIN_REFRESH_MS || refreshMS > MAX_REFRESH_MS)
				return new rfidSimpleReport(context, OperationOutcome.FailByApplicationError, new ArgumentOutOfRangeException("refreshMS", refreshMS, string.Format("Value must be between {0} and {1}", MIN_REFRESH_MS, MAX_REFRESH_MS)));


			_refreshRateMS = refreshMS;
			_bgdWorker     = worker;
			_requestTagList.Clear();
			_periodTagList.Clear();

			long asofTimeInElapsedMS = ElapsedMilliseconds;

			rfidOperationReport report = new rfidOperationReport(
																	context,
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
																	GetSessionRelativeSessionDuration(asofTimeInElapsedMS)
																);


			_control.StartAction();

			PerformanceCounter processorUtilizationCounter = null;

			int notused = FileHandler.TotalPacketCount; // Make sure the file is created;


			ReaderRequest readerRequest         = new ReaderRequest();
			readerRequest.reader				= _theReaderID.Name;
			
			#pragma warning disable 420 // reference to a volatile field is valid for Interlocked call
			readerRequest.requestSequence		= System.Threading.Interlocked.Increment(ref _commonRequestIndex);
			#pragma	warning restore 420
			
			readerRequest.requestName			= rfidReader.GetNameForRequest(rfidReader.ReaderRequestType.GetInventory);
			readerRequest.startTime				= GetSessionRelativeDateTime(report.StartTimeMS);
			readerRequest.startingPacketCount	= RawPacketCount;
			readerRequest.startingTagCount		= RawInventoryCount;
			MemoryStream data = new MemoryStream();
			readerRequest.WriteTo(data);
			PacketData.PacketWrapper pseudoPacket = new PacketData.PacketWrapper(new PacketData.CommandPsuedoPacket(readerRequest.requestName, data.ToArray()), PacketData.PacketType.U_N_D_F_I_N_E_D);
			data.Dispose();
			pseudoPacket.IsPseudoPacket = true;
			pseudoPacket.ReaderName = _theReaderID.Name;
			pseudoPacket.ReaderIndex = (int)_theReaderID.Handle;
			FileHandler.WritePacket(pseudoPacket);

			
			DateTime ReportDue = DateTime.Now.AddMilliseconds(refreshMS);
			ReaderInterfaceThreadClass.StartEvent.Set();
			OperationOutcome outcome = OperationOutcome.Success;

			while (ActiveReaders > 0)
			{
				CounterSample sample = CounterSample.Empty;
				if (processorUtilizationCounter != null)
				{
					try
					{
						sample = processorUtilizationCounter.NextSample();
					}
					catch (Exception)	{	}
				}

				ProcessQueuedPackets();

				QueueEvent.WaitOne(30, true);
				QueueEvent.Reset();
				
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
																			SessionDuration
																		)
											);
					_periodTagList.Clear();
					ReportDue = now.AddMilliseconds(refreshMS);
				}


				if (FunctionController.GetActionRequest() == FunctionControl.RequestedAction.Abort)
				{
					string abortError = null;
					try
					{
						//abortError = _managedAccess.AbortOperation((int)ReaderHandle);
						abortError = stopDelegate(true);
						outcome = OperationOutcome.FailByUserAbort;
					}
					catch (Exception e)
					{
						abortError = String.Format("Error attempting to abort: {0}", e.Message);
						Debug.WriteLine(abortError);
						Debug.Assert(abortError == null);
					}
					break;
				}

				if (FunctionController.GetActionRequest() == FunctionControl.RequestedAction.Stop)
				{
					string stopError = null;
					try
					{
						//stopError = LakeChabotReader.MANAGED_ACCESS.CancelOperation( ( int ) readerHandle, 0);
						stopError = stopDelegate(false);
						outcome = OperationOutcome.SuccessWithUserCancel;
					}
					catch (Exception e)
					{
						stopError = String.Format("Error attempting to stop: {0}", e.Message);
						Debug.WriteLine(stopError);
						Debug.Assert(stopError == null);
					}
					if (stopError != null)
					{
						// Try to abort
					}
					break;
				}


				if (FunctionController.State == FunctionControl.FunctionState.Paused)
				{
					do
					{
						Thread.Sleep(refreshMS);
					} while (FunctionController.State == FunctionControl.FunctionState.Paused);
				}

				float processorUtilization = 0;
				if (processorUtilizationCounter != null && sample != CounterSample.Empty)
				{
					processorUtilization = CounterSample.Calculate(sample, processorUtilizationCounter.NextSample());
				}
//				Debug.WriteLine(String.Format("Processor Util: {0}", processorUtilization));

				if (processorUtilization > (float)TARGET_MAX_USAGE_PERCENT)
					_packetSleepMS += SLEEP_ADD_SUBTRACT_MS;
				else
					_packetSleepMS -= _packetSleepMS <= MIN_SLEEP_TIME_MS ? 0 : SLEEP_ADD_SUBTRACT_MS;
			}


			// Get any leftover packets
			ProcessQueuedPackets();

			report.OperationCompleted(
										outcome,
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
										SessionDuration
										);

			
			_control.Finished();
			return report;

		} // MonitorInventory(Object context, BackgroundWorker worker, int refreshMS, int singulationLimit, int readerCycleLimit)





		public ReportBase MonitorInventory(Object context, BackgroundWorker worker, int refreshMS, StopDependentReadersDelegate stopDelegate)
		{
			Debug.WriteLine(String.Format("{0}() ThreadID = {1}", System.Reflection.MethodInfo.GetCurrentMethod().Name, Thread.CurrentThread.ManagedThreadId));

			if (IsDisposed)
				throw new ObjectDisposedException("LakeChabotReader");

			if (Mode != rfidReader.OperationMode.BoundToReader)
				return new rfidSimpleReport(context, OperationOutcome.FailByContext,
					new rfidException(rfidErrorCode.ReaderIsNotBound, "Reader must be bound before inventory can be read."));

			if (_control.State != FunctionControl.FunctionState.Idle)
				return new rfidSimpleReport(context, OperationOutcome.FailByContext, new Exception("Cannot read the inventory, the prior task has not completed"));

			if (null == worker)
				return new rfidSimpleReport(context, OperationOutcome.FailByApplicationError, new ArgumentNullException("worker", "BackgroundWorker is required"));

			if (refreshMS < MIN_REFRESH_MS || refreshMS > MAX_REFRESH_MS)
				return new rfidSimpleReport(context, OperationOutcome.FailByApplicationError, new ArgumentOutOfRangeException("refreshMS", refreshMS, string.Format("Value must be between {0} and {1}", MIN_REFRESH_MS, MAX_REFRESH_MS)));


			_refreshRateMS = refreshMS;
			_bgdWorker = worker;
			_requestTagList.Clear();
			_periodTagList.Clear();

			long asofTimeInElapsedMS = ElapsedMilliseconds;

			rfidOperationReport report = new rfidOperationReport(
																	context,
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
																	GetSessionRelativeSessionDuration(asofTimeInElapsedMS)
																);


			_control.StartAction();

			PerformanceCounter processorUtilizationCounter = null;

			int notused = FileHandler.TotalPacketCount; // Make sure the file is created;


			ReaderRequest readerRequest = new ReaderRequest();
			readerRequest.reader				= _theReaderID.Name;
			
			#pragma warning disable 420 // reference to a volatile field is valid for Interlocked call
			readerRequest.requestSequence		= System.Threading.Interlocked.Increment(ref _commonRequestIndex);
			#pragma	warning restore 420
			
			readerRequest.requestName			= rfidReader.GetNameForRequest(rfidReader.ReaderRequestType.GetInventory);
			readerRequest.startTime				= GetSessionRelativeDateTime(report.StartTimeMS);
			readerRequest.startingPacketCount	= RawPacketCount;
			readerRequest.startingTagCount		= RawInventoryCount;
			MemoryStream data = new MemoryStream();
			readerRequest.WriteTo(data);
			PacketData.PacketWrapper pseudoPacket = new PacketData.PacketWrapper(new PacketData.CommandPsuedoPacket(readerRequest.requestName, data.ToArray()), PacketData.PacketType.U_N_D_F_I_N_E_D);
			data.Dispose();
			pseudoPacket.IsPseudoPacket = true;
			pseudoPacket.ReaderName = _theReaderID.Name;
			pseudoPacket.ReaderIndex = (int)_theReaderID.Handle;
			FileHandler.WritePacket(pseudoPacket);

			
			DateTime ReportDue = DateTime.Now.AddMilliseconds(refreshMS);
			ReaderInterfaceThreadClass.StartEvent.Set();
			OperationOutcome outcome = OperationOutcome.Success;

			while (ActiveReaders > 0)
			{
				CounterSample sample = CounterSample.Empty;
				if (processorUtilizationCounter != null)
				{
					try
					{
						sample = processorUtilizationCounter.NextSample();
					}
					catch (Exception)	{	}
				}

				ProcessQueuedPackets();

				QueueEvent.WaitOne(30, true);
				QueueEvent.Reset();
				
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
																			SessionDuration
																		)
											);
					_periodTagList.Clear();
					ReportDue = now.AddMilliseconds(refreshMS);
				}


				if (FunctionController.GetActionRequest() == FunctionControl.RequestedAction.Abort)
				{
					string abortError = null;
					try
					{
						//abortError = _managedAccess.AbortOperation((int)ReaderHandle);
						abortError = stopDelegate(true);
						outcome = OperationOutcome.FailByUserAbort;
					}
					catch (Exception e)
					{
						abortError = String.Format("Error attempting to abort: {0}", e.Message);
						Debug.WriteLine(abortError);
						Debug.Assert(abortError == null);
					}
					break;
				}

				if (FunctionController.GetActionRequest() == FunctionControl.RequestedAction.Stop)
				{
					string stopError = null;
					try
					{
						//stopError = LakeChabotReader.MANAGED_ACCESS.CancelOperation( ( int ) readerHandle, 0);
						stopError = stopDelegate(false);
						outcome = OperationOutcome.SuccessWithUserCancel;
					}
					catch (Exception e)
					{
						stopError = String.Format("Error attempting to stop: {0}", e.Message);
						Debug.WriteLine(stopError);
						Debug.Assert(stopError == null);
					}
					if (stopError != null)
					{
						// Try to abort
					}
					break;
				}


				if (FunctionController.State == FunctionControl.FunctionState.Paused)
				{
					do
					{
						Thread.Sleep(refreshMS);
					} while (FunctionController.State == FunctionControl.FunctionState.Paused);
				}

				float processorUtilization = 0;
				if (processorUtilizationCounter != null && sample != CounterSample.Empty)
				{
					processorUtilization = CounterSample.Calculate(sample, processorUtilizationCounter.NextSample());
				}
//				Debug.WriteLine(String.Format("Processor Util: {0}", processorUtilization));

				if (processorUtilization > (float)TARGET_MAX_USAGE_PERCENT)
					_packetSleepMS += SLEEP_ADD_SUBTRACT_MS;
				else
					_packetSleepMS -= _packetSleepMS <= MIN_SLEEP_TIME_MS ? 0 : SLEEP_ADD_SUBTRACT_MS;
			}


			// Get any leftover packets
			ProcessQueuedPackets();

			report.OperationCompleted(
										outcome,
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
										SessionDuration
										);

			
			_control.Finished();
			return report;

		} // MonitorInventory(Object context, BackgroundWorker worker, int refreshMS, int singulationLimit, int readerCycleLimit)





		public void UpdateInventoryStats()
		{
			if (RawAntennaCycleCount > _inventoryMatrix.Periods)
			{
				lock (_tagInventoryData)
				{
					foreach (DatabaseRowTemplate row in TagInventoryData)
					{
						TagInventory tag = row as TagInventory;
						if (tag != null)
						{
							tag.averageReadsPerCycle = -1;
							TagCycleEMA data = TagCycleEMA.Empty;
							try
							{
								data = _inventoryMatrix.GetEMAReadsPerCycle(tag.TagIdData, RawAntennaCycleCount - 1);
							
								if (data != TagCycleEMA.Empty && data.HasEMA)
								{
									tag.averageReadsPerCycle = data.EMA.Value;
								}
//								PacketLogger.Comment(String.Format("TagCycleEMA {0}", data));
							}
							catch (Exception e)
							{
								SysLogger.LogWarning("Unable to get EMAReadesPerCycle: " + e.Message);
								System.Diagnostics.Debug.Assert(false, e.Message);
							}
						}
//						tag.sumReadDelta = 0xb0b;
					}
				}
			}
		}




		public UInt32 ReadRegister(UInt16 address, out string errorMessage)
		{
			UInt32 result = 0;
			if (Mode == rfidReader.OperationMode.BoundToReader)
			{
				errorMessage = "Not Supported for Virtual Reader";
			}
			else
			{
				errorMessage = "Reader is not bound.";
			}

			return result;
		}

		public UInt32 WriteRegister(UInt16 address, UInt32 value, out string errorMessage)
		{
			UInt32 result = 0;

			errorMessage = "Not Supported for Virtual Reader";

			return result;
		}


		public UInt16 ReadTildenRegister(UInt16 address, out string errorMessage)
		{
			UInt16 result = 0;
			if (Mode == rfidReader.OperationMode.BoundToReader)
			{
				errorMessage = "Not Supported for Virtual Reader";
			}
			else
			{
				errorMessage = "Reader is not bound.";
			}

			return result;
		}

		public UInt16 WriteTildenRegister(UInt16 address, UInt16 value, out string errorMessage)
		{
			UInt16 result = 0;

            errorMessage = "Not Supported for Virtual Reader";

			return result;
		}

		public static void AssemblyClosing()
		{

			// ???? @@@@ rfid.Linkage();
		}



		#region IDisposable Members

		public void Dispose()
		{
			Dispose(true);
			// Take yourself off the Finalization queue to prevent finalization code for this object from executing a second time.
			GC.SuppressFinalize(this);
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
		protected virtual void Dispose(bool disposing)
		{
			// Check to see if Dispose has already been called.
			if (Interlocked.CompareExchange(ref _isDisposed, 1, 0) == 0)
			{
				// If disposing equals true, dispose all managed and unmanaged resources.
				if (disposing)
				{

					// Dispose managed resources.
					LakeChabotReader.PacketLogger.Clear();

					if (_fileHandler != null)			_fileHandler.Dispose();

					if (_propertyBagData != null)		_propertyBagData.Dispose();
					if (_antennaCycleData != null)		_antennaCycleData.Dispose();
					if (_inventoryRoundData != null)	_inventoryRoundData.Dispose();
					if (_packetStreamData != null)		_packetStreamData .Dispose();
					if (_readerCommandData != null)		_readerCommandData.Dispose();
					if (_readerAntennaCycleData != null)_readerAntennaCycleData.Dispose();
					if (_readerRequestData != null)		_readerRequestData.Dispose();
					if (_tagInventoryData != null)		_tagInventoryData.Dispose();
					if (_tagReadData != null)			_tagReadData.Dispose();
					if (_readRateData != null)			_readRateData.Dispose();
					if (_badPacketData != null)			_badPacketData.Dispose();
					if (_inventoryCycleData != null)	_inventoryCycleData.Dispose();

					if (_staticReaderDir != null && Directory.Exists(_staticReaderDir))
						Directory.Delete(_staticReaderDir);
					
				}
				// Release unmanaged resources. If disposing is false, only the following code is executed.
				// nothing yet...
			}
		}


		public DictionaryEntry[] DataSources
		{
			get {

				return new DictionaryEntry[] 
					{ 
						new DictionaryEntry("PropertyBag",			PropertyBagData),
						new DictionaryEntry("ReaderRequest",		ReaderRequestData),
						new DictionaryEntry("PacketStream",			PacketStreamData),
						new DictionaryEntry("ReaderCommand",		ReaderCommandData),
						new DictionaryEntry("ReaderAntennaCycle",	ReaderAntennaCycleData),
						new DictionaryEntry("AntennaPacket",		AntennaCycleData),
						new DictionaryEntry("InventoryCycle",		InventoryCycleData),
						new DictionaryEntry("InventoryRound",		InventoryRoundData),
						new DictionaryEntry("TagRead",				TagReadData),
						new DictionaryEntry("TagInventory",			TagInventoryData),
						new DictionaryEntry("BadPacket",			BadPacketData),
						new DictionaryEntry("ReadRate",				ReadRateData),
					};
			}
		}




        public rfid.Structures.LibraryVersion LibraryVersion
		{
			get
			{
                return new rfid.Structures.LibraryVersion( ); 
			}
		}

        public rfid.Structures.FirmwareVersion FirmwareVersion
		{
			get
			{
                return new rfid.Structures.FirmwareVersion(); 
			}
		}


        public rfid.Structures.MacBootLoaderVersion BootLoaderVersion
		{
			get
			{
                return new rfid.Structures.MacBootLoaderVersion( ); 
			}
		}

        public rfid.Structures.Version HardwareVersion
        {
            get
            {
                return new rfid.Structures.Version();
            }
        }

        //clark 2011.5.10 Doesn't recommend to get data from OEM directly.
        //public UInt32 FirmwareErrorCode
        //{
        //    get
        //    {
        //        string error = null;
        //        UInt32 ec = ReadRegister(RegisterData.MAC_REG_ERROR, out error);
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
				if (!String.IsNullOrEmpty(error))
				{
					throw new rfidReaderException(error);
				}
				return ac;
			}
		}



		public rfid.Constants.MacRegion RegulatoryRegion
		{
			get
			{
				throw new NotImplementedException("Not Supported for Virtual Reader");
			}
			set
			{
				throw new NotImplementedException("Not Supported for Virtual Reader");
			}
		}





		public rfid.Constants.RadioOperationMode ReaderOperationMode
		{
			get
			{
				throw new NotImplementedException("Not Supported for Virtual Reader");
			}
			set
			{
				throw new NotImplementedException("Not Supported for Virtual Reader");
			}
		}


		public rfid.Constants.Result ReaderDataFormat
		{
			get
			{
				throw new NotImplementedException("Not Supported for Virtual Reader");
			}
			set
			{
				throw new NotImplementedException("Not Supported for Virtual Reader");
			}
		}

		public rfid.Constants.RadioPowerState PowerState
		{
			get
			{
				throw new NotImplementedException("Not Supported for Virtual Reader");
			}
			set
			{
				throw new NotImplementedException("Not Supported for Virtual Reader");
			}
		}


		public RegisterData.InventoryAlgorithmValue InventoryAlgorithm
		{

			get
			{
				throw new NotImplementedException("Not Supported for Virtual Reader");
			}
			set
			{
				throw new NotImplementedException("Not Supported for Virtual Reader");
			}
		}


		public int StopCount
		{
			get
			{
				throw new NotImplementedException("Not Supported for Virtual Reader");
			}
			set
			{
				throw new NotImplementedException("Not Supported for Virtual Reader");
			}
		}

		public bool AutomaticSelect
		{

			get
			{
				throw new NotImplementedException("Not Supported for Virtual Reader");
			}
			set
			{
				throw new NotImplementedException("Not Supported for Virtual Reader");
			}
		}

        public rfid.Constants.Session InventorySession
		{

			get
			{
				throw new NotImplementedException("Not Supported for Virtual Reader");
			}
			set
			{
				throw new NotImplementedException("Not Supported for Virtual Reader");
			}
		}

        public rfid.Constants.SessionTarget InventoryTarget
		{

			get
			{
				throw new NotImplementedException("Not Supported for Virtual Reader");
			}
			set
			{
				throw new NotImplementedException("Not Supported for Virtual Reader");
			}
		}


		public LakeChabotReader.SelectedStateValue InventorySelectedState
		{

			get
			{
				throw new NotImplementedException("Not Supported for Virtual Reader");
			}
			set
			{
				throw new NotImplementedException("Not Supported for Virtual Reader");
			}
		}

		public byte FixedQValue
		{
			get
			{
				throw new NotImplementedException("Not Supported for Virtual Reader");
			}
			set
			{
				throw new NotImplementedException("Not Supported for Virtual Reader");
			}
		}


		public byte FixedInventoryRetry
		{

			get
			{
				throw new NotImplementedException("Not Supported for Virtual Reader");
			}
			set
			{
				throw new NotImplementedException("Not Supported for Virtual Reader");
			}
		}



		public bool FixedQABFlipping
		{
			get
			{
				throw new NotImplementedException("Not Supported for Virtual Reader");
			}
			set
			{
				throw new NotImplementedException("Not Supported for Virtual Reader");
			}
		}


		public bool FixedQRunToZero
		{
			get
			{
				throw new NotImplementedException("Not Supported for Virtual Reader");
			}
			set
			{
				throw new NotImplementedException("Not Supported for Virtual Reader");
			}
		}

		public byte StartingQValue
		{
			get
			{
				throw new NotImplementedException("Not Supported for Virtual Reader");
			}
			set
			{
				throw new NotImplementedException("Not Supported for Virtual Reader");
			}
		}

		public byte MaximumQValue
		{
			get
			{
				throw new NotImplementedException("Not Supported for Virtual Reader");
			}
			set
			{
				throw new NotImplementedException("Not Supported for Virtual Reader");
			}
		}

		public byte MinimumQValue
		{
			get
			{
				throw new NotImplementedException("Not Supported for Virtual Reader");
			}
			set
			{
				throw new NotImplementedException("Not Supported for Virtual Reader");
			}
		}

		public byte MaximumQueryReps
		{
			get
			{
				throw new NotImplementedException("Not Supported for Virtual Reader");

			}
			set
			{
				throw new NotImplementedException("Not Supported for Virtual Reader");
			}
		}

		public bool VariableQABFlipping
		{
			get
			{
				throw new NotImplementedException("Not Supported for Virtual Reader");
			}
			set
			{
				throw new NotImplementedException("Not Supported for Virtual Reader");
			}
		}


		
		public Source_GPIOList AntennaConfiguration
		{
			get
			{
				throw new NotImplementedException("Not Supported for Virtual Reader");
			}

			set
			{
				throw new NotImplementedException("Not Supported for Virtual Reader");
			}
		}


		public Source_Antenna GetAntennaData(uint portIndex)
		{
			throw new NotImplementedException("Not Supported for Virtual Reader");
		}

		public Source_Antenna SetAntennaData(Source_Antenna data)
		{
			throw new NotImplementedException("Not Supported for Virtual Reader");
		}


		public Source_GPIOList GpioConfiguration
		{
			get 
			{
				throw new NotImplementedException("Not Supported for Virtual Reader");
			}
			set 
			{
				throw new NotImplementedException("Not Supported for Virtual Reader");
			}
		}



        public Source_GPIOList GpioData
		{
			get 
			{
				throw new NotImplementedException("Not Supported for Virtual Reader");
			}
			set 
			{
				throw new NotImplementedException("Not Supported for Virtual Reader");
			}
		}



        public Source_FrequencyBandList RFConfiguration
        {
            get
            {
                throw new NotImplementedException( "Not Supported for Virtual Reader" );
            }

            set
            {
                throw new NotImplementedException( "Not Supported for Virtual Reader" );
            }
        }


        public Source_FrequencyBand GetFrequencyBand( uint band )
        {
            throw new NotImplementedException( "Not Supported for Virtual Reader" );
        }

        public Source_FrequencyBand SetFrequencyBand( Source_FrequencyBand data )
        {
            throw new NotImplementedException( "Not Supported for Virtual Reader" );
        }
 

        /* err raise now if using Source_OEMData & try to load using specified
         * reader 
        public LakeChabotReader.OEM_Data OEMData
		{
			get 
			{
				throw new NotImplementedException("Not Supported for Virtual Reader");
			}
			set 
			{
				throw new NotImplementedException("Not Supported for Virtual Reader");
			}
		}
        */

        /*
		public Source_LinkProfileList Profiles
		{
			get
			{
				throw new NotImplementedException("Not Supported for Virtual Reader");
			}
		}


		public Source_LinkProfile GetLinkProfileFromProfileIDString(string profileString)
		{
			if (profileString == null) return null;

			string[] parts = profileString.Split(':');

			if (parts == null || parts.Length != 2) return null;

			UInt64 profileID;
			UInt32 version;

			if (!(UInt64.TryParse(parts[0], out profileID) && UInt32.TryParse(parts[1], out version)))
				return null;

			LakeChabotReader.LinkProfile result = Profiles.Find(delegate(LakeChabotReader.LinkProfile p) { return p.ProfileId == profileID && p.ProfileVersion == version; });

			return result;
		}

		public LakeChabotReader.LinkProfile ActiveProfile
		{
			get
			{
				throw new NotImplementedException("Not Supported for Virtual Reader");
			}
			set
			{
				throw new NotImplementedException("Not Supported for Virtual Reader");
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
			get { return _readerAntennaCycleData;	}	
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



		private void InitReader()
		{
			if (IsDisposed)
				throw new ObjectDisposedException("VirtualReader");

			_control = new FunctionControl(FunctionControl.SupportedActions.StopAbortPause);
		}




	
		
		private static void WaitForFileReady(string newFilePath)
		{
			bool goodOpen = false;
			int totalTries = 100;
			while (!goodOpen)
			{
				try
				{
					FileStream s = File.OpenRead(newFilePath);
					goodOpen =  s.Length > 0;
					s.Close();
					s.Dispose();
				}
				catch (Exception)
				{
					if (totalTries-- == 0)
						throw;
					Thread.Sleep(50);
				}
			}
		}


		private Source_GPIOList GetAntennaDataList()
		{
            /* //!!!!
			LakeChabotReader.AntennaDataList result = new LakeChabotReader.AntennaDataList();
			int max = RFID.RFIDInterface.Properties.Settings.Default.MaxVirtualAntennas;
			for (int i = 0; i < max; i++)
			{
				LakeChabotReader.AntennaData data = new LakeChabotReader.AntennaData();
				data.Port = (uint)i;
				result.Add(data);
			}
             */
			return null ; //result;
		}

        /* //???? virtual reader was returning default chabot reader channels ????
		private LakeChabotReader.FrequencyBandList GetFrequencyBandList()
		{
			LakeChabotReader.FrequencyBandList result = new LakeChabotReader.FrequencyBandList();
			int max = RFID.RFIDInterface.Properties.Settings.Default.MaxFrequencyBands;
			for (int i = 0; i < max; i++)
			{
				LakeChabotReader.FrequencyBand data = new LakeChabotReader.FrequencyBand();
				data.Band = (uint)i;
				result.Add(data);
			}
			return result;
		}
        */




		/// <summary>
		/// 
		/// </summary>
		/// <param name="PacketBuffer"></param>
		
		private bool PacketCallBackFromReader(int readerIndex, Byte[] PacketBuffer)
		{
			uint myHandle = this._theReaderID._handle;
			Debug.Assert(readerIndex == myHandle);

			#region For debugging thread problems
			#if false
			Debug.WriteLine(String.Format("{0}() ThreadID = {1}", System.Reflection.MethodInfo.GetCurrentMethod().Name, Thread.CurrentThread.ManagedThreadId));
			Console.WriteLine("\nRaw Packet Data:\n{0}", BitConverter.ToString(PacketBuffer));
			StringBuilder sb = new StringBuilder();
			sb.AppendFormat("[0x{0:x}]{1}() ", Thread.CurrentThread.ManagedThreadId, System.Reflection.MethodInfo.GetCurrentMethod().Name);
			sb.AppendFormat("readerIndex = {0}, PacketBuffer = {1}\n", readerIndex, BitConverter.ToString(PacketBuffer));
			foreach (ProcessThread pt in Process.GetCurrentProcess().Threads)
			{
				sb.AppendFormat("[0x{0:x}] State: {1}\n", pt.Id, pt.ThreadState.ToString());
			}
			Debug.WriteLine(sb.ToString());
			Debug.Flush();
			#endif
			#endregion


			String errorMessage					= null;
			PacketData.PacketBase packet		= null;

			// TODO SysLogger.WriteToLog = LakeChabotReader.Emulation != LakeChabotReader.EmulationMode.Library;
			PacketData.PacketType type			= PacketData.ParsePacket(PacketBuffer, out packet, out errorMessage);
			SysLogger.WriteToLog = true;

			PacketData.PacketWrapper envelope	= null;
			long elapsedSesionTime				= ElapsedMilliseconds;

			if (packet == null || type == PacketData.PacketType.U_N_D_F_I_N_E_D || errorMessage != null)
			{
				BadPacket badPacket			= new BadPacket();
				
				#pragma warning disable 420 // reference to a volatile field is valid for Interlocked call
				badPacket.badPacketSequence	= System.Threading.Interlocked.Increment(ref _commonBadIndex);
				#pragma warning restore 420

				badPacket.packetTime		= DateTime.UtcNow;
				
				badPacket.rawPacketData		= PacketBuffer.Clone() as byte[];
				badPacket.errorMessage		= errorMessage;
				using (MemoryStream data = new MemoryStream(256))
				{
					badPacket.WriteTo(data);
					envelope = new PacketData.PacketWrapper(new PacketData.CommandPsuedoPacket("BadPacket", data.ToArray()), PacketData.PacketType.U_N_D_F_I_N_E_D);
				}
				envelope.IsPseudoPacket		= true;
				envelope.PacketNumber		= badPacket.PacketSequence;
				envelope.Timestamp			= badPacket.packetTime;
				envelope.ReaderIndex		= (int)_theReaderID.Handle;
				envelope.ReaderName			= _theReaderID.Name;
				envelope.CommandNumber		= _processedInventoryIndex;
				envelope.ElapsedTimeMs		= elapsedSesionTime;
			}
			else
			{
				envelope = new PacketData.PacketWrapper(packet, type, PacketBuffer, _commonRequestIndex, elapsedSesionTime, readerIndex, Name);
				Debug.Assert(envelope.PacketType == type);
			}
			
			//Console.WriteLine("*** {0} Packet ***\n{1}", Enum.GetName(typeof(packetData.packetType), type), packet.ToString());
			//Console.WriteLine(packet.ToString());

			
			lock (PacketQueue)
			{
				PacketQueue.Enqueue(envelope);
			}

			#pragma warning disable 420 // reference to a volatile field is valid for Interlocked call
			int queueSize = Interlocked.Increment(ref _queueCount);
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


			return FunctionController.GetActionRequest() != FunctionControl.RequestedAction.Abort;
		}

		
		/// <summary>
		/// 
		/// </summary>
		private int ProcessQueuedPackets()
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
			while (cnt < 1000)
			{
				PacketData.PacketWrapper envelope = null;
				lock (PacketQueue)
				{
					if (PacketQueue.Count > 0)
						envelope = PacketQueue.Dequeue();
				}
				if (envelope == null)
					break;

				#pragma warning disable 420 // reference to a volatile field is valid for Interlocked call
				Interlocked.Decrement(ref _queueCount);
				#pragma warning restore 420 

				#pragma warning disable 420 // reference to a volatile field is valid for Interlocked call
				envelope.PacketNumber = System.Threading.Interlocked.Increment(ref _rawPacketCount) - 1;
				#pragma warning restore 420 

//				if (PacketArrival != null)
//				{
//					PacketArrival(this, new PacketArrivalEventArgs(envelope));
//				}

				SavePacket(envelope);
				cnt++;
			}
			return cnt;
		}

		

		/// <summary>
		/// Caculate basic stats and save the packet to a stream
		/// </summary>
		/// <param name="envelope"></param>
		private void SavePacket(PacketData.PacketWrapper envelope)
		{
//				Debug.WriteLineIf((int)envelope.RadioID != (int)_theReaderID.Handle, String.Format("Expected: {0}, Actual: {1}", (int)_theReaderID.Handle, (int)envelope.RadioID), "ERROR");
		//	Debug.Assert(envelope.ReaderName == Name);

			PacketData.PacketBase packet = envelope.Packet;
			PacketData.PacketType type = envelope.PacketType;

			switch (type)
			{

			case PacketData.PacketType.CMD_BEGIN:
				{
					LastUsedAntenna = null;
					LastCommandResult = null;
					PacketData.cmd_beg cmd_beg = packet as PacketData.cmd_beg;
					Debug.Assert(cmd_beg != null);

					#pragma warning disable 420 // reference to a volatile field is valid for Interlocked call
					System.Threading.Interlocked.Increment(ref _rawCommandCount);
					#pragma warning restore 420

				}
				break;

			case PacketData.PacketType.CMD_END:
				{
					PacketData.cmd_end cmd_end = packet as PacketData.cmd_end;
					Debug.Assert(cmd_end != null);
					LastCommandResult = cmd_end.Result;
					if (cmd_end.Result != 0) CommandErrors++;
				}
				break;


			case PacketData.PacketType.ANTENNA_CYCLE_BEGIN:
				{
					PacketData.ant_cyc_beg cyc_beg = packet as PacketData.ant_cyc_beg;
					Debug.Assert(cyc_beg != null);

					#pragma warning disable 420 // reference to a volatile field is valid for Interlocked call
					System.Threading.Interlocked.Increment(ref _rawAntennaCycleCount);
					#pragma warning restore 420 

//					PacketLogger.Comment(String.Format("Start of Cycle {0} Command={1}", RawCycleCount - 1, RawCommandCount - 1));

					lock (_tagInventoryData)
					{
						foreach (TagInventory ti in TagInventoryData)
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
					Debug.Assert(cyc_beg_diag != null);
				}
				break;

			case PacketData.PacketType.ANTENNA_CYCLE_END:
				{
					PacketData.ant_cyc_end cyc_end = packet as PacketData.ant_cyc_end;
					Debug.Assert(cyc_end != null);

//					PacketLogger.Comment(String.Format("End of Cycle {0}", RawCycleCount - 1)); 
					
				}
				break;

			case PacketData.PacketType.ANTENNA_CYCLE_END_DIAG:
				{
					PacketData.ant_cyc_end_diag cyc_end_diag = packet as PacketData.ant_cyc_end_diag;
					Debug.Assert(cyc_end_diag != null);

				}
				break;

			case PacketData.PacketType.ANTENNA_BEGIN:
				{
					PacketData.ant_beg ant_beg = packet as PacketData.ant_beg;
					Debug.Assert(ant_beg != null);

					#pragma warning disable 420 // reference to a volatile field is valid for Interlocked call
					System.Threading.Interlocked.Increment(ref _rawAntennaCount);
					#pragma warning restore 420 

					LastUsedAntenna = ant_beg.antenna;
				}
				break;

			case PacketData.PacketType.ANTENNA_END:
				{
					PacketData.ant_end ant_end = packet as PacketData.ant_end;
					Debug.Assert(ant_end != null);
					LastUsedAntenna = null;
				}
				break;

			case PacketData.PacketType.ANTENNA_BEGIN_DIAG:
				{
					PacketData.ant_beg_diag ant_beg_diag = packet as PacketData.ant_beg_diag;
					Debug.Assert(ant_beg_diag != null);
				}
				break;

			case PacketData.PacketType.ANTENNA_END_DIAG:
				{
					PacketData.ant_end_diag ant_end_diag = packet as PacketData.ant_end_diag;
					Debug.Assert(ant_end_diag != null);
				}
				break;

			case PacketData.PacketType.INVENTORY_CYCLE_BEGIN:
				{
					PacketData.inv_cyc_beg inv_cyc_beg = packet as PacketData.inv_cyc_beg;
					Debug.Assert(inv_cyc_beg != null);

#pragma warning disable 420 // reference to a volatile field is valid for Interlocked call
					System.Threading.Interlocked.Increment(ref _rawInventoryCycleCount);
#pragma warning restore 420

					//		PacketLogger.Comment(String.Format("Start of Cycle {0} Command={1}", RawCycleCount - 1, RawCommandCount - 1));
				}
				break;

				case PacketData.PacketType.INVENTORY_CYCLE_END:
					{
						PacketData.inv_cyc_end  inv_cyc_end = packet as PacketData.inv_cyc_end;
						Debug.Assert(inv_cyc_end != null);
					}
					break;


				case PacketData.PacketType.CARRIER_INFO:
					{
						PacketData.carrier_info carrier_info = packet as PacketData.carrier_info;
						Debug.Assert(carrier_info != null);
					}
					break;


				case PacketData.PacketType.INVENTORY_CYCLE_END_DIAGS:
					{
						PacketData.inv_cyc_end_diag inv_cyc_end_diag = packet as PacketData.inv_cyc_end_diag;
						Debug.Assert(inv_cyc_end_diag != null);
					}
					break;

			case PacketData.PacketType.ISO18K6C_INVENTORY_ROUND_BEGIN:
				{
					PacketData.inv_rnd_beg inv_rnd_beg = packet as PacketData.inv_rnd_beg;
					Debug.Assert(inv_rnd_beg != null);

					#pragma warning disable 420 // reference to a volatile field is valid for Interlocked call
					System.Threading.Interlocked.Increment(ref _rawRoundCount);
					#pragma warning restore 420 // reference to a volatile field is valid for Interlocked call
				}
				break;

			case PacketData.PacketType.ISO18K6C_INVENTORY_ROUND_BEGIN_DIAG:
				{
					PacketData.inv_rnd_beg_diag inv_rnd_beg_diag = packet as PacketData.inv_rnd_beg_diag;
					Debug.Assert(inv_rnd_beg_diag != null);
				}
				break;

			case PacketData.PacketType.ISO18K6C_INVENTORY_ROUND_END_DIAG:
				{
					PacketData.inv_rnd_end_diag inv_rnd_end_diag = packet as PacketData.inv_rnd_end_diag;
					Debug.Assert(inv_rnd_end_diag != null);
				}
				break;

			case PacketData.PacketType.ISO18K6C_INVENTORY:
				{
					PacketData.inventory epc = packet as PacketData.inventory;
					Debug.Assert(epc != null);

					BitVector32 flags	= new BitVector32(epc.flags);
					bool badCrc = flags[PacketData.PacketBase.crcResult] != 0;
					if (badCrc)
					{
						#pragma warning disable 420 // reference to a volatile field is valid for Interlocked call
						System.Threading.Interlocked.Increment(ref _cRCErrors);
						#pragma warning restore 420
					}
					else
					{
						#pragma warning disable 420 // reference to a volatile field is valid for Interlocked call
						System.Threading.Interlocked.Increment(ref _rawInventoryCount);
						#pragma warning restore 420

						rfidTag key = new rfidTag(epc.inventory_data);
						if (_sessionTagList.AddOrIncrementTagCount(key) == 1)
						{
							_requestTagList.Add(key, 1);
							_periodTagList.Add(key, 1);
						}
						else
						{
							if (_requestTagList.AddOrIncrementTagCount(key) == 1)
							{
								_periodTagList.Add(key, 1);
							}
							else
							{
								_periodTagList.AddOrIncrementTagCount(key);
							}
						}


						string epcData		= BitConverter.ToString(epc.inventory_data, 0, epc.inventory_data.Length - flags[PacketData.PacketBase.paddingBytes]);
						int CycleSlotNumber = RawAntennaCycleCount == 0 ? 0 : RawAntennaCycleCount - 1;

						KeyList<TagInventory> epcDataKey = new KeyList<TagInventory>("tagIdData", epcData);
						_inventoryMatrix.AddTagCycle(epcData, CycleSlotNumber);

						TagInventory tagInventory = new TagInventory();

						lock (_tagInventoryData)
						{
							if (!TagInventoryData.Contains(epcDataKey)) // new tag ID
							{
//								PacketLogger.Comment(String.Format("First Read of Tag {0} Command={1}, Cycle={2}", epcData, RawCommandCount - 1, RawCycleCount - 1));

								tagInventory.tagIdData = epcData;
								tagInventory.readCount = 1;
								tagInventory.cycleReadCount = 1;
								tagInventory.firstReadTime = epc.ms_ctr;
								tagInventory.lastReadTime = epc.ms_ctr;
								tagInventory.firstCommand = RawCommandCount - 1;
								tagInventory.lastCommand = RawCommandCount - 1;
								tagInventory.actualCycleCount = 1;
								tagInventory.totalCycleCount = RawAntennaCycleCount;
								tagInventory.cycleReadCount = 1;
								tagInventory.sumOfCommandReads = 0;
//								tagInventory.readRate = 0;

								//tagInventory.readerCycleCount		;
								//tagInventory.firstReaderCycle		;
								//tagInventory.lastReaderCycle		;
								//tagInventory.antennaCycleCount	;
								//tagInventory.firstAntennaCycle	;
								//tagInventory.lastAntennaCycle		;
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
								if (LastUsedAntenna != null)
								{
									switch (LastUsedAntenna)
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
								//tagInventory.inventoryRoundCount	;
								//tagInventory.firstinventoryRound	;
								//tagInventory.lastinventoryRound		;
								TagInventoryData.Add(tagInventory);
							}
							else
							{
								tagInventory = (TagInventory)TagInventoryData[epcDataKey];

								tagInventory.readCount++;
								if (tagInventory.lastCommand == CycleSlotNumber)
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
//								if (tagInventory.lastReadTime > tagInventory.firstReadTime)
//								{
//									tagInventory.readRate = (float)(tagInventory.ReadCount) / (float)((tagInventory.LastReadTime - tagInventory.FirstReadTime) / 100);
//								}

								//tagInventory.commandCount			= 1;

								//tagInventory.firstCommand			;
								//tagInventory.lastCommand			;
								//tagInventory.readerCycleCount		;
								//tagInventory.firstReaderCycle		;
								//tagInventory.lastReaderCycle		;
								//tagInventory.antennaCycleCount	;
								//tagInventory.firstAntennaCycle	;
								//tagInventory.lastAntennaCycle		;
								if (LastUsedAntenna != null)
								{
									switch (LastUsedAntenna)
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
								//tagInventory.lastinventoryRound		;
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
					Debug.Assert(access != null);

					BitVector32 flags = new BitVector32(access.flags);
					bool badCrc = flags[PacketData.PacketBase.accessCRCFlag] == (int)PacketData.PacketBase.CrcResultValues.Bad;
					if (badCrc)
					{
#pragma warning disable 420 // reference to a volatile field is valid for Interlocked call
						System.Threading.Interlocked.Increment(ref _cRCErrors);
#pragma warning restore 420
					}
					else
					{
						if (flags[PacketData.PacketBase.accessErrorFlag] == (int)PacketData.PacketBase.ISO_18000_6C_ErrorFlag.AccessSucceeded)
						{
#pragma warning disable 420 // reference to a volatile field is valid for Interlocked call
							System.Threading.Interlocked.Increment(ref _commonAccessCount);
#pragma warning restore 420
						}
						envelope.RecordReadRequest((int)TagBank, TagMemoryLocation, TagMemoryLength, TagPassword);
					}

				}
				break;

			case PacketData.PacketType.FREQUENCY_HOP_DIAG:
				break;

			case PacketData.PacketType.NONCRITICAL_FAULT:
				break;

			case PacketData.PacketType.U_N_D_F_I_N_E_D:
			default:
				break;
			}

			// Save the packet
			FileHandler.WritePacket(envelope);

			RecentPacketList.Add(new PacketData.PacketWrapper(envelope));
			
		} //private void SavePacket(PacketData.PacketWrapper envelope)


    }	//class VirtualReader : rfidInterface


}	//namespace RFID.rfidInterface
