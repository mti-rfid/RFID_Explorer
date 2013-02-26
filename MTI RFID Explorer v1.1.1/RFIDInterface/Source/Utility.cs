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
 * $Id: Utility.cs,v 1.3 2009/09/03 20:23:24 dshaheen Exp $
 * 
 * Description: 
 *
 *****************************************************************************
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Diagnostics;

namespace RFID.RFIDInterface
{

	public class DurationFormat : IFormatProvider, ICustomFormatter
	{
		public static DurationFormat Formatter = new DurationFormat();

		#region IFormatProvider Members

		public object GetFormat(Type formatType)
		{
			if (formatType == typeof(ICustomFormatter))
			{
				return this;
			}
			else
			{
				return null;
			}
		}

		#endregion


		#region ICustomFormatter Members

		public string Format(string format, object arg, IFormatProvider formatProvider)
		{
			if (format == null)	
				return String.Format ("{0}", arg);

			if (arg == null)
				return "";

			if (arg.GetType() == typeof(TimeSpan) && (format == "C" || format == "c")) // Custom Code
			{
				TimeSpan t = ((TimeSpan)arg).Duration();
				double seconds = t.TotalSeconds;
				if (seconds < 1)
					return String.Format("{0:F0} {1} ", t.TotalMilliseconds, format == "c" ? "milliseconds" : "Milliseconds");

				if (seconds <= 90)
					return String.Format("{0:F2} {1}", seconds, format == "c" ? "seconds" : "Seconds");

				double minutes = t.TotalMinutes;
				if (minutes <= 90)
					return String.Format("{0:F2} {1}", minutes, format == "c" ? "minutes" : "Minutes");


				return String.Format("{0:F2} {1}", t.TotalHours, format == "c" ? "hours" : "Hours");
			}
			
			//If the object to be formatted supports the IFormattable interface, pass on to ToString
			if (arg is IFormattable)
				return ((IFormattable)arg).ToString(format, formatProvider);

			return arg.ToString();
		}

		#endregion
	}


	public static class SysLogger
	{

		private static System.Diagnostics.EventLog _log;
		private static bool _writeToLog = true;

		public static bool WriteToLog
		{
			get { return _writeToLog; }
			set { _writeToLog = value; }
		}

		static SysLogger()
		{
			_log = new System.Diagnostics.EventLog("Application", ".", System.Reflection.Assembly.GetExecutingAssembly().GetName().Name);
		}

		/// <summary>
		/// Writes a formatted message to the System Application log and to the debug output (if debugging).
		/// Catches and ignores any logging errors such log full, etc.
		/// </summary>
		/// <param name="exception">Exception that is to be logged.</param>
		/// <returns>The message that was logged so it can be shown to the user if desired</returns>
		public static string LogError(Exception exception)
		{
			if (exception == null)
			{
				exception = new ArgumentNullException("exception", "Null exception passed to Logger.LogError()");
			}

			string message;
			RFID.RFIDInterface.rfidException rfid = exception as RFID.RFIDInterface.rfidException;
			if (rfid != null)
			{
				message = FormatEventMessage(rfid);
				if (message.Length > 32000) message = message.Substring(0, 32000);
				try
				{
					if (WriteToLog)	_log.WriteEntry(message, System.Diagnostics.EventLogEntryType.Error, (int)rfid.ErrorCode.ErrorCode);
				}
				catch (Exception)	{	}
			}
			else
			{
				message = FormatEventMessage(exception);
				if (message.Length > 32000)
					message = message.Substring(0, 32000);
				try
				{
					if (WriteToLog) _log.WriteEntry(message, System.Diagnostics.EventLogEntryType.Error);
				}
				catch (Exception)	{ }
				
			}

			System.Diagnostics.Debug.WriteLine("ERROR:: " + message);
			return message;
		}

		
		public static void LogWarning(string Message)
		{
			if (Message.Length > 32000)
				Message = Message.Substring(0, 32000);
			try
			{
				if (WriteToLog) _log.WriteEntry(Message, System.Diagnostics.EventLogEntryType.Warning);
			}
			catch (Exception)	{		}

			System.Diagnostics.Debug.WriteLine("Warning:: " + Message);
		}

		public static void LogMessage(string Message)
		{
			if (Message.Length > 32000)
				Message = Message.Substring(0, 32000);
			try
			{
				if (WriteToLog) _log.WriteEntry(Message, System.Diagnostics.EventLogEntryType.Information);
			}
			catch (Exception) {	}
			System.Diagnostics.Debug.WriteLine("Message:: " + Message);
		}



		private static string FormatEventMessage(Exception e)
		{
			return e == null ? "None." : 
				String.Format(
					"{0}\n\n" +
					"Source Name:...{1}\n" +
					"Thread Name:...{2}\n" +
					"Stack Trace:\n{3}\n\n" +
					"InnerException: {4}",
					e.Message,
					e.Source,
					System.Threading.Thread.CurrentThread.Name,
					e.StackTrace,
					e.InnerException);
		}

	} //public static class SysLogger





	[AttributeUsage(AttributeTargets.Field)]
	public class DBFieldInfoAttribute : System.Attribute
	{
		private int		_ordinal		= 0;
		private bool	_allowNull		= false;
		private bool	_allowDuplicate	= true;
		private bool	_primaryKey		= false;
		private bool	_timestamp		= false;
		private int		_keyOrder		= 0;
		private string	_columnName		= null;
		private string	_expression		= null;
		private Type	_databaseType	= null;


		public DBFieldInfoAttribute(int Ordinal)
		{
			_ordinal = Ordinal;
		}

		public DBFieldInfoAttribute(int Ordinal, string ColumnName)
		{
			_ordinal = Ordinal;
			_columnName = ColumnName;
		}

		public bool AllowNull
		{
			get { return _allowNull; }
			set { _allowNull = value; }
		}

		public bool AllowDuplicate
		{
			get { return _allowDuplicate; }
			set { _allowDuplicate = value; }
		}

		
		public bool PrimaryKey
		{
			get { return _primaryKey; }
			set { _primaryKey = value; }
		}
		
		public int KeyOrder
		{
			get { return _keyOrder; }
			set { _keyOrder = value; }
		}

		public string ColumnName
		{
			get { return _columnName; }
			set { _columnName = value; }
		}

		public int Ordinal
		{
			get { return _ordinal; }
			set { _ordinal = value; }
		}

		public string Expression
		{
			get { return _expression; }
			set { _expression = value; }
		}

		public Type DBType
		{
			get { return _databaseType; }
			set { _databaseType = value; }
		}
	
		public bool Timestamp
		{
			get { return _timestamp; }
			set { _timestamp = value; }
		}

	}






	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class KeyList<T> : Dictionary<String, Object>, IEqualityComparer<KeyList<T>> where T : DatabaseRowTemplate 
	{
		static List<String> KeyNames;
		static List<Type>	KeyTypes;
		static KeyList()
		{
			SortedList<int, string> keys = new SortedList<int,string>(5);
			SortedList<int, Type> types = new SortedList<int, Type>(5);
			foreach (FieldInfo field in typeof(T).GetFields())
			{
				Object [] attrs = field.GetCustomAttributes(typeof(DBFieldInfoAttribute), false);
				DBFieldInfoAttribute info = null;
				if (attrs.Length > 0)
					info = attrs[0] as DBFieldInfoAttribute;
				if (info != null && info.PrimaryKey)
				{
					int index = info.KeyOrder;
					while (keys.ContainsKey(index))
					{
						index++;
					}
					keys.Add(index, field.Name);
					types.Add(index, field.FieldType);
				}
			}
			KeyNames = new List<string>(keys.Count);
			KeyNames.AddRange(keys.Values);
			KeyTypes = new List<Type>(keys.Count);
			KeyTypes.AddRange(types.Values);
		}


		public KeyList()
		{
		}

		public KeyList(String keyName, Object value)
		{
			Add(keyName, value);
		}


		public KeyList(T dbRow)
		{
			foreach (string keyName in KeyNames)
			{
				Add(keyName, dbRow.GetType().GetField(keyName).GetValue(dbRow));
			}
		}


		public static string TableName
		{
			get { return typeof(T).Name; }
		}

		#region IEqualityComparer<KeyList> Members

		public bool Equals(KeyList<T> x, KeyList<T> y)
		{
			if (Object.Equals(x, null))
			{
				if (Object.Equals(y, null))
				{
					return true;
				}
				else 
				{
					return false;
				}
			}
			if (Object.Equals(y, null))
			{
				return false;
			}


			if (x.Count != y.Count)
				throw new ArgumentException("KeyLists must be of the same length to compare for equality.");

			foreach (string key in KeyNames)
			{
				Object xValue;
				Object yValue;

				if (!x.TryGetValue(key, out xValue))
				{
					throw new Exception(String.Format("First KeyList is missing key \"{0}\".", key));
				}
				if (!y.TryGetValue(key, out yValue))
				{
					throw new Exception(String.Format("First KeyList is missing key \"{0}\".", key));
				}

				if (!Object.Equals(xValue, yValue))	// the easy way
					return false;
				/*
				if (Object.Equals(xValue, null) || Object.Equals(yValue, null))
					return false;

				Type xType = xValue.GetType();
				Type yType = yValue.GetType();

				if (xType != yType)
				{
					throw new Exception(String.Format("KeyLists must hold keys with the same type but key \"{0}\" has different types.", key));
				}
				IComparable comparer = xValue as IComparable;
				if (comparer != null)
				{
					if (comparer.CompareTo(yValue) != 0)
						return false;
				}
				 * */
			}
			return true;
		}

		public override string  ToString()
		{
 			StringBuilder sb = new StringBuilder();
			sb.Append(String.Format("{0}:: ", TableName));
			foreach (string keyName in KeyNames)
			{
				object val;
				if (this.TryGetValue(keyName, out val))
				{
					sb.AppendFormat("{0} = {1} ", keyName, val);
				}
				else
				{
					sb.AppendFormat("{0} = 'null' ", keyName);
				}
			}
			return sb.ToString();
		}


		public new void Add(String keyName, Object value)
		{
			int index = KeyNames.FindIndex(delegate(string key) {  return key == keyName; });

			if (index == -1)
				throw new Exception(String.Format("The key \"{0}\" is not a valid key for {1}.", keyName, TableName));

//			if (value == null)
//				throw new Exception(String.Format("The value for key \"{0}\" of {1} cannot be null.", keyName, TableName));

			if (value != null && value.GetType() != KeyTypes[index])
				throw new Exception(String.Format("The value for key \"{0}\" is the wrong type for {1}. Expected type is {2}. Actual type passed is {3}.", keyName, TableName, KeyTypes[index].Name, value.GetType().Name));

			base.Add(keyName, value);
			
				
		}
		
		public int GetHashCode(KeyList<T> obj)
		{
			if (obj == null)
				throw new ArgumentNullException("obj");

			int rslt = 0;
			foreach (Object o in obj.Values)
				rslt ^= (o == null ? 0 : o.GetHashCode());

			return rslt;
		}

		#endregion

	}



	
	public class SequentialDataFile<T> : IDisposable where T : DatabaseRowTemplate, new()
	{
		private const int FILE_BLOCKING_SIZE	= 4 * 1024;

		public static string TableName
		{
			get { return typeof(T).Name; }
		}

		
		static SequentialDataFile()
		{
			Object[] attrbs = typeof(T).GetCustomAttributes(typeof(System.Runtime.InteropServices.GuidAttribute), false);
			if (attrbs.Length > 0)
			{
				FileHeader.Signature = new Guid(((System.Runtime.InteropServices.GuidAttribute)attrbs[0]).Value).ToByteArray();
			}
			else
			{
				throw new Exception(String.Format("{0} does not have the GuidAttribute associated with it.", TableName));
			}
		}




		/// <summary>
		/// A static class that holds the file header which is located at the top of each data file.
		/// </summary>
		private static class FileHeader
		{
			public const int		FILE_HEADER_SIZE	= 64;
			public static byte[]	Signature			= null;
			private static Guid		_ID					= Guid.Empty;
			private static DateTime	_timestamp			= DateTime.MinValue;

			private static byte[]	_idArray			= null;
			private static byte[]	_timestampArray		= null;


			public static Guid ID
			{
			  get { return FileHeader._ID; }
			  set { FileHeader._ID = value; _idArray = value.ToByteArray();}
			}

			public static DateTime Timestamp
			{
				get { return FileHeader._timestamp; }
				set { FileHeader._timestamp = value; _timestampArray = BitConverter.GetBytes(_timestamp.ToBinary()); }
			}


			public static byte[] Buffer
			{
				get
				{
					byte[] header = new byte[FILE_HEADER_SIZE];
					Signature.CopyTo(header, 0);

					_idArray.CopyTo(header, Signature.Length);

					_timestampArray.CopyTo(header, Signature.Length + _idArray.Length);

					return header;
				}
				set
				{
					for (int i = 0; i < Signature.Length; i++)
					{
						if (Signature[i] != value[i])
							throw new ApplicationException("Invalid file signature.");
					}
					int location = Signature.Length;

					if (_idArray == null) _idArray = new byte[16];
					Array.Copy(value, location, _idArray, 0, _idArray.Length);
					ID = new Guid(_idArray);
					location += _idArray.Length;

					if (_timestampArray == null) 
						_timestampArray = BitConverter.GetBytes(_timestamp.ToBinary());

					Array.Copy(value, location, _timestampArray, 0, _timestampArray.Length);
					
					_timestamp = DateTime.FromBinary(BitConverter.ToInt64(_timestampArray, 0));

				}
			}
		}

		/// <summary>
		/// A static class that holds the block header data which is at the start of each block of the file
		/// </summary>
		private static class PageHeader
		{
			public const			int PAGE_HEADER_SIZE	= 64;
			public static readonly	byte[] Signature		= null;
			public static			int PageNumber			= 0;
			public static			int StartingIndex		= 0;
			public static			int ItemCount			= 0;
			public static			long NextPagePosition	= 0;
			
			
			static PageHeader()
			{
				PageHeader.Signature = new Guid("B66948EB-99B4-4646-BC04-616A8B0358DC").ToByteArray();
			}

			public static void SetHeaderValues(PageLocation data, long dataLength)
			{
				PageNumber			= data.PageNumber;
				StartingIndex		= data.StartingIndex;
				ItemCount			= data.ItemCount;
				NextPagePosition	= data.Position + dataLength;
			}

			public static byte[] Buffer
			{
				get
				{
					byte[] header = new byte[PAGE_HEADER_SIZE];
					header.Initialize();
					int location = 0;

					Signature.CopyTo(header, location);
					location+= Signature.Length;

					BitConverter.GetBytes(PageNumber).CopyTo(header, location);
					location+= sizeof(int);

					BitConverter.GetBytes(StartingIndex).CopyTo(header, location);
					location+= sizeof(int);

					BitConverter.GetBytes(ItemCount).CopyTo(header, location);
					location+= sizeof(int);

					BitConverter.GetBytes(NextPagePosition).CopyTo(header, location);
					
					return header;
				}
				set
				{
					for (int i = 0; i < Signature.Length; i++)
					{
						if (Signature[i] != value[i])
							throw new ApplicationException("Unabled to access the correct page header");
					}
					int location = Signature.Length;
					PageNumber = BitConverter.ToInt32(value, location);
					location+= sizeof(int);

					StartingIndex= BitConverter.ToInt32(value, location);
					location+= sizeof(int);

					ItemCount= BitConverter.ToInt32(value, location);
					location+= sizeof(int);

					NextPagePosition = BitConverter.ToInt64(value, location);

				}
			}

		}

		/// <summary>
		/// A support class to hold a block related information including the block number, block starting position in file.
		/// </summary>
		private class PageLocation
		{
			public readonly int		PageNumber;
			public readonly int		StartingIndex;
			public readonly long	Position;
			public			int		ItemCount;

			public PageLocation(int page, int startingIndex, long position, int count)
			{
				PageNumber		= page;
				StartingIndex	= startingIndex;
				Position		= position;
				ItemCount		= count;
			}
		}

		

		private bool			_disposed		= false;
		private int				_maxRowsInMem	= 0;
		private int				_currentPage	= 0;
		private string			_fileName		= null;
		private FileStream		_fileStream		= null;
		
		private Object			DataLockObject	= new Object();

		private List<PageLocation>					PageList;
		private List<T>								NewCollection;

		/// <summary>
		/// SequentialDataFile Constructor
		/// </summary>
		public SequentialDataFile(int maxRowsInMemory)
//			: base(new KeyList<T>(), 0)
		{
			FileHeader.ID = Guid.NewGuid();
			FileHeader.Timestamp = DateTime.Now;
			_maxRowsInMem = maxRowsInMemory;


			NewCollection = new List<T>(maxRowsInMemory);

			PageList = new List<PageLocation>(1024);
			PageList.Add(new PageLocation(0, 0, FileHeader.FILE_HEADER_SIZE, 0));
			CurrentPageIndex = 0;

			
		}

		
		public SequentialDataFile(int maxRowsInMemory, string FileName)
//			: base(new KeyList<T>(), 32)
		{
			_maxRowsInMem = maxRowsInMemory;
			_fileName = FileName;


			NewCollection = new List<T>(maxRowsInMemory);

			PageList = new List<PageLocation>(1024);

			LoadPage(0);
			PageWalk();
		}


		~SequentialDataFile()
		{
			Dispose(false);
		}


		#region IDisposable Members and Dispose()

		public bool IsDisposing
		{
			get { return this._disposed; }
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		
		protected virtual void Dispose(bool disposing)
		{
			if (!this._disposed)
			{
				if (disposing)
				{
					if (_fileStream != null)
					{
						_fileStream.Close();
						_fileStream.Dispose();
						_fileStream = null;

						if (_fileName != null)
							File.Delete(_fileName);
					}
				}
				// Release unmanaged resources. If disposing is false, 
				// only the following code is executed.
			}
			_disposed = true;
		}

#endregion


		public Guid ID
		{
			get { return FileHeader.ID; }
		}

		private int CurrentPageIndex
		{
			set { _currentPage = value; }
		}

		private PageLocation CurrentPage
		{
			get { return PageList.Count == 0 ? null : PageList[_currentPage];	}
		}

		private PageLocation LastPage
		{
			get { return PageList.Count == 0 ? null : PageList[PageList.Count - 1]; }
		}

		public int PageCount
		{
			get { return PageList.Count; }
		}

		public bool IsLastPage
		{
			get { return PageList.Count == 0 ? true : LastPage.PageNumber == _currentPage; }
		}

		public bool HasPendingData
		{
			get { return _fileStream.CanWrite && IsLastPage && NewCollection.Count > 0; }
		}


		public string Name
		{
			get { return new T().Name; }
		}

		
		public string FileName
		{
			get 
			{
				if (_fileName == null) { bool notUsed = FileWriteStream.CanRead; };
				return _fileName; 
			}
			private set { _fileName = value; }
		}

		public FileStream FileWriteStream
		{
			get 
			{
				if (this._disposed)
					throw new ObjectDisposedException(String.Format("{0}<{1}>", this.GetType().Name, typeof(T).Name));

				if (_fileStream == null)
				{
					
					_fileStream = new FileStream(Path.GetTempFileName(), FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None, FILE_BLOCKING_SIZE, FileOptions.None);
					//Replace with above line with the following line to prevent temp files from being deleted
					//_fileStream = new FileStream(Path.GetTempFileName(), FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None, FILE_BLOCKING_SIZE, FileOptions.None);
					_fileName = _fileStream.Name;
					WriteFileHeader();
				}
				return _fileStream; 
			}
		}

		public FileStream FileReadStream
		{
			get
			{
				if (this._disposed)
					throw new ObjectDisposedException(String.Format("{0}<{1}>", this.GetType().Name, typeof(T).Name));

				if (_fileStream == null)
				{
					if (_fileName == null)
					{
						throw new NullReferenceException("File cannot be opened with a null _fileName.");
					}

					if (!File.Exists(_fileName))
					{
						throw new ApplicationException(_fileName + " does not exist.");
					}
					_fileStream = new FileStream(_fileName, FileMode.Open, FileAccess.Read, FileShare.Read, FILE_BLOCKING_SIZE, FileOptions.RandomAccess);
					ReadFileHeader();
					RebuildPageList();
				}
				return _fileStream;
			}
		}

		private void RebuildPageList()
		{
			if (this._disposed)
				throw new ObjectDisposedException(String.Format("{0}<{1}>", this.GetType().Name, typeof(T).Name));

			int cnt = 0;
			long position = FileHeader.FILE_HEADER_SIZE;
			_fileStream.Position = position;
			while (_fileStream.Position < _fileStream.Length - PageHeader.PAGE_HEADER_SIZE)
			{
				byte[] header = new byte[PageHeader.PAGE_HEADER_SIZE];
				_fileStream.Read(header, 0, PageHeader.PAGE_HEADER_SIZE);

				PageHeader.Buffer = header;

				System.Diagnostics.Debug.Assert(cnt++ == PageHeader.PageNumber);

				PageList.Add(new PageLocation(PageHeader.PageNumber, PageHeader.StartingIndex, position, PageHeader.ItemCount));

				position = PageHeader.NextPagePosition;

				if (position == 0)
					break;

				_fileStream.Position = position;
			}
		}

		/// <summary>
		/// /// <summary>
		/// Write the header for a new dataFile file.
		/// <remarks>File Position is left at the end of the header.</remarks>
		/// </summary>
		private void WriteFileHeader()
		{
			if (this._disposed)
				throw new ObjectDisposedException(String.Format("{0}<{1}>", this.GetType().Name, typeof(T).Name));


			lock (DataLockObject)
			{
				_fileStream.Position = 0;
				_fileStream.Write(FileHeader.Buffer, 0, FileHeader.FILE_HEADER_SIZE);
				_fileStream.Flush();
			}
		}

		
		private byte[] ReadFileHeader()
		{
			if (this._disposed)
				throw new ObjectDisposedException(String.Format("{0}<{1}>", this.GetType().Name, typeof(T).Name));

			byte[] header = new byte[FileHeader.FILE_HEADER_SIZE];

			lock (DataLockObject)
			{

				FileWriteStream.Position = 0;
				FileWriteStream.Read(header, 0, FileHeader.FILE_HEADER_SIZE);

				FileHeader.Buffer = header;
			}
			return header;
		}


		private void WritePageHeader(long pageDataLength)
		{
			if (this._disposed)
				throw new ObjectDisposedException(String.Format("{0}<{1}>", this.GetType().Name, typeof(T).Name));

			lock (DataLockObject)
			{
				FileWriteStream.Position = CurrentPage.Position;
				PageHeader.SetHeaderValues(CurrentPage, pageDataLength + PageHeader.PAGE_HEADER_SIZE);
				FileWriteStream.Write(PageHeader.Buffer, 0, PageHeader.PAGE_HEADER_SIZE);
			}
		}

		
		


		public void WriteBlock(byte[] data)
		{
			if (this._disposed)
				throw new ObjectDisposedException(String.Format("{0}<{1}>", this.GetType().Name, typeof(T).Name));


			FileWriteStream.Write(data, 0, data.Length);
		}

	

		public void CreateNewPage()
		{
			if (this._disposed)
				throw new ObjectDisposedException(String.Format("{0}<{1}>", this.GetType().Name, typeof(T).Name));
			

			lock (DataLockObject)
			{
				FileWriteStream.Position = FileWriteStream.Length;

				PageLocation lastPage = PageList[PageCount - 1];

				int startingIndex = lastPage.StartingIndex + lastPage.ItemCount;

				PageLocation pg = new PageLocation(PageCount,  startingIndex, FileWriteStream.Position, 0);

				PageList.Add(pg);

				CurrentPageIndex = pg.PageNumber;
			}
		}


		public void PageWalk()
		{
			if (this._disposed)
				throw new ObjectDisposedException(String.Format("{0}<{1}>", this.GetType().Name, typeof(T).Name));

			for (int i = 0; i < PageCount; i++)
				LoadPage(i);

		}


		private PageLocation FindPageLocation(int index)
		{
			if (this._disposed)
				throw new ObjectDisposedException(String.Format("{0}<{1}>", this.GetType().Name, typeof(T).Name));

			foreach (PageLocation pl in PageList)
			{
				if (index >= pl.StartingIndex && index < pl.StartingIndex + pl.ItemCount)
					return pl;
			}
			throw new System.Collections.Generic.KeyNotFoundException(String.Format("An element with the index \"{0}\" does not exist in the collection.", index));
		}


		public void LoadPage(int page)
		{
			if (this._disposed)
            {
				throw new ObjectDisposedException(String.Format("{0}<{1}>", this.GetType().Name, typeof(T).Name));
            }


			NewCollection = new List<T>(_maxRowsInMem);
			CurrentPageIndex = page;

			// note the reference to FileReadStream caused the block header to get loaded and the PageList to be populated.
			if (PageList == null || FileReadStream == null)
			{
				System.Diagnostics.Debug.Assert(false, "PageList == null");
				return;
			}

			if (PageList.Count == 0)
				return;

			FileReadStream.Position = PageList[page].Position + PageHeader.PAGE_HEADER_SIZE;

			for (int i = 0; i < CurrentPage.ItemCount; i++)
			{
				T t = new T();
//				long position = FileReadStream.Position;
				t.ReadFrom(FileReadStream);

				NewCollection.Add(t);

			}
		}

		



		/// <summary>
		/// Write the block to disk
		/// </summary>
		/// <returns></returns>
		public void CommitCurrentPage()
		{
			if (this._disposed)
				throw new ObjectDisposedException(String.Format("{0}<{1}>", this.GetType().Name, typeof(T).Name));

			int cnt = 0;
			using (MemoryStream pageStream = new MemoryStream())
			{
				lock (DataLockObject)
				{
					if (Count == 0)
						return;

					foreach (T t in NewCollection)
					{
						t.WriteTo(pageStream);
						cnt++;
					}

					//TODO remove debug	
					System.Diagnostics.Debug.Assert(cnt == CurrentPage.ItemCount);

					if (cnt != CurrentPage.ItemCount)
						throw new Exception(String.Format("Invalid Page Count Page={0}, Expected Count={1}, Actual Count{2}.", CurrentPage.PageNumber, CurrentPage.ItemCount, cnt));
					
					WritePageHeader(pageStream.Length);
					FileWriteStream.Write(pageStream.ToArray(), 0, (int)pageStream.Length);
					FileWriteStream.Flush();
				}
			}
		}


		public int Count
		{
			get
			{
				if (this._disposed)
					throw new ObjectDisposedException(String.Format("{0}<{1}>", this.GetType().Name, typeof(T).Name));

				if (PageList.Count == 0)
					return 0;

				PageLocation lastPage = PageList[PageList.Count - 1];
				if (_currentPage == lastPage.PageNumber)
				{
					return lastPage.StartingIndex + NewCollection.Count;
				}
				return lastPage.StartingIndex + lastPage.ItemCount;
			}
		}



		public T this[int index]
		{
			get
			{
				if (this._disposed)
					throw new ObjectDisposedException(String.Format("{0}<{1}>", this.GetType().Name, typeof(T).Name));


				if (index < 0)
					throw new ArgumentOutOfRangeException("index", "Index cannot be less than zero.");

				if (index >= Count)
					throw new ArgumentOutOfRangeException("index", "Index excced size of collection.");


				// is the index in the current page return the value from here
				if (index >= CurrentPage.StartingIndex && index < CurrentPage.StartingIndex + NewCollection.Count)
					return NewCollection[index - CurrentPage.StartingIndex];


				// if we got this far, we are going to have a do a page load from disk
				PageLocation location = FindPageLocation(index);

				if (HasPendingData)
				{
					CommitCurrentPage();
				}

				LoadPage(location.PageNumber);

				return NewCollection[index - location.StartingIndex];
				
			}
			set
			{
				if (this._disposed)
					throw new ObjectDisposedException(String.Format("{0}<{1}>", this.GetType().Name, typeof(T).Name));


				if (!Contains(index))
				{
					throw new System.Collections.Generic.KeyNotFoundException(String.Format("An element with the index \"{0}\" does not exist in the collection.", index));
				}


				PageLocation location = FindPageLocation(index);
				if (location == CurrentPage)
				{
					NewCollection[index - location.StartingIndex] = value;
					if (location != LastPage)
					{
						CommitCurrentPage();
					}
				}
				else
				{
					{
						if (HasPendingData)
						{
							CommitCurrentPage();
						}
						LoadPage(location.PageNumber);
						NewCollection[index - location.StartingIndex] = value;
						CommitCurrentPage();
					}
				}
			}
		}


		public bool Contains(int index)
		{
			int myCount = this.Count;
			return index >= 0 && index < myCount;
		}


		public int Add(T item)
		{
			if (this._disposed)
			{
				throw new ObjectDisposedException(String.Format("{0}<{1}>", this.GetType().Name, typeof(T).Name));
			}

			if (item == null)
				throw new ArgumentNullException("item");

			if (item.GetType() != typeof(T))
				throw new ArgumentException(String.Format("DataFile.Add expected a {0} but was passed a {1}", TableName, item.GetType().Name), "item");

			int index = Count;


			if (PageList.Count > 1 && !IsLastPage)
			{
				LoadPage(LastPage.PageNumber);
			}

			lock (DataLockObject)
			{
				if (_maxRowsInMem > 0 && NewCollection.Count >= _maxRowsInMem)
				{
					CommitCurrentPage();
					NewCollection = new List<T>(_maxRowsInMem);
					CreateNewPage();
					
				}
				CurrentPage.ItemCount++;
				NewCollection.Add(item);
			}
			return index;
		}


		/// <summary>
		/// Clears the entire SequentialDataFile structure
		/// </summary>
//		public new void Clear()
		public void Clear()
		{
			if (this._disposed)
			{
				throw new ObjectDisposedException(String.Format("{0}<{1}>", this.GetType().Name, typeof(T).Name));
			}

			lock (DataLockObject)
			{
				if (PageList.Count > 0)
				{

					if (CurrentPage.ItemCount > 0)
						CommitCurrentPage();

					NewCollection.Clear();

					PageList.Clear();
					PageList = new List<PageLocation>(1024);
					PageList.Add(new PageLocation(0, 0, FileHeader.FILE_HEADER_SIZE, 0));
					CurrentPageIndex = 0;
				}
				//TODO More to add here
				if (_fileStream != null)
				{
					_fileStream.Close();
					_fileStream.Dispose();
					_fileStream = null;
				}
				FileHeader.ID = Guid.NewGuid();
			}
		}


		public void CommitPageAndCopyFile(string newFilePath)
		{
			long fileSize;
			lock (DataLockObject)
			{
				CommitCurrentPage();
				long savePosition = FileWriteStream.Position;

				FileWriteStream.Flush();
				FileWriteStream.Close();

				using (FileStream temp = File.Open(FileName, FileMode.Open, FileAccess.Read, FileShare.Read))
				{
					fileSize = temp.Length;
					temp.Close();
				}

				File.Copy(FileName, newFilePath, true);

				_fileStream = new FileStream(FileName, FileMode.Open, FileAccess.ReadWrite, FileShare.None, FILE_BLOCKING_SIZE, FileOptions.None);
					
				_fileStream.Position = savePosition;
			}

			bool goodOpen = false;
			int totalTries = 100;
			while (!goodOpen)
			{
				try
				{
					using (FileStream s = File.OpenRead(newFilePath))
					{
						goodOpen =  s.Length == fileSize;
						s.Close();
					}
				}
				catch (Exception)
				{
					if (totalTries-- == 0)
						throw;
					Thread.Sleep(50);
				}
			}
			
		}




		public override int GetHashCode()
		{
			return ID.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return (obj is SequentialDataFile<T>) ? this == (SequentialDataFile<T>)obj : false;
		}
		
		public static bool operator==(SequentialDataFile<T> lhs, SequentialDataFile<T> rhs)
		{
			if (Object.Equals(lhs, null))
				if (Object.Equals(rhs, null))
					return true;
				else
					return false;
			else
				if (Object.Equals(rhs, null))
					return false;
				else
					return lhs.ID == rhs.ID;
		}

		public static bool operator!=(SequentialDataFile<T> lhs, SequentialDataFile<T> rhs)
		{
			return !(lhs==rhs);
		}




	} // public class SequentialDataFile : KeyedCollection<KeyList, PacketWrapper>, ISerializable



	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class DataFile<T> : KeyedCollection<KeyList<T>, DatabaseRowTemplate>, IDisposable where T : DatabaseRowTemplate, new()
	{
		private const int FILE_BLOCKING_SIZE	= 4 * 1024;

		public static string TableName
		{
			get { return typeof(T).Name; }
		}


		static DataFile()
		{
			Object[] attrbs = typeof(T).GetCustomAttributes(typeof(System.Runtime.InteropServices.GuidAttribute), false);
			if (attrbs.Length > 0)
			{
				FileHeader.Signature = new Guid(((System.Runtime.InteropServices.GuidAttribute)attrbs[0]).Value).ToByteArray();
			}
			else
			{
				throw new Exception(String.Format("{0} does not have the GuidAttribute associated with it.", TableName));
			}
		}


		/// <summary>
		/// A static class that holds the file header which is located at the top of each data file.
		/// </summary>
		private static class FileHeader
		{
			public const int FILE_HEADER_SIZE	= 64;
			public static byte[] Signature			= null;
			private static Guid _ID					= Guid.Empty;
			private static DateTime _timestamp			= DateTime.MinValue;

			private static byte[] _idArray			= null;
			private static byte[] _timestampArray		= null;


			public static Guid ID
			{
				get { return FileHeader._ID; }
				set { FileHeader._ID = value; _idArray = value.ToByteArray(); }
			}

			public static DateTime Timestamp
			{
				get { return FileHeader._timestamp; }
				set { FileHeader._timestamp = value; _timestampArray = BitConverter.GetBytes(_timestamp.ToBinary()); }
			}


			public static byte[] Buffer
			{
				get
				{
					byte[] header = new byte[FILE_HEADER_SIZE];
					Signature.CopyTo(header, 0);

					_idArray.CopyTo(header, Signature.Length);

					_timestampArray.CopyTo(header, Signature.Length + _idArray.Length);

					return header;
				}
				set
				{
					for (int i = 0; i < Signature.Length; i++)
					{
						if (Signature[i] != value[i])
							throw new ApplicationException("Invalid file signature.");
					}
					int location = Signature.Length;

					if (_idArray == null)
						_idArray = new byte[16];
					Array.Copy(value, location, _idArray, 0, _idArray.Length);
					ID = new Guid(_idArray);
					location += _idArray.Length;

					if (_timestampArray == null)
						_timestampArray = BitConverter.GetBytes(_timestamp.ToBinary());

					Array.Copy(value, location, _timestampArray, 0, _timestampArray.Length);

					_timestamp = DateTime.FromBinary(BitConverter.ToInt64(_timestampArray, 0));

				}
			}
		}

		/// <summary>
		/// A static class that holds the block header data which is at the start of each block of the file
		/// </summary>
		private static class PageHeader
		{
			public const int PAGE_HEADER_SIZE	= 64;
			public static readonly byte[] Signature		= null;
			public static int PageNumber			= 0;
			public static int StartingIndex		= 0;
			public static int ItemCount			= 0;
			public static long NextPagePosition	= 0;


			static PageHeader()
			{
				PageHeader.Signature = new Guid("B66948EB-99B4-4646-BC04-616A8B0358DC").ToByteArray();
			}

			public static void SetHeaderValues(PageLocation data, long dataLength)
			{
				PageNumber			= data.PageNumber;
				StartingIndex		= data.StartingIndex;
				ItemCount			= data.ItemCount;
				NextPagePosition	= data.Position + dataLength;
			}

			public static byte[] Buffer
			{
				get
				{
					byte[] header = new byte[PAGE_HEADER_SIZE];
					header.Initialize();
					int location = 0;

					Signature.CopyTo(header, location);
					location+= Signature.Length;

					BitConverter.GetBytes(PageNumber).CopyTo(header, location);
					location+= sizeof(int);

					BitConverter.GetBytes(StartingIndex).CopyTo(header, location);
					location+= sizeof(int);

					BitConverter.GetBytes(ItemCount).CopyTo(header, location);
					location+= sizeof(int);

					BitConverter.GetBytes(NextPagePosition).CopyTo(header, location);

					return header;
				}
				set
				{
					for (int i = 0; i < Signature.Length; i++)
					{
						if (Signature[i] != value[i])
							throw new ApplicationException("Unabled to access the correct page header");
					}
					int location = Signature.Length;
					PageNumber = BitConverter.ToInt32(value, location);
					location+= sizeof(int);

					StartingIndex= BitConverter.ToInt32(value, location);
					location+= sizeof(int);

					ItemCount= BitConverter.ToInt32(value, location);
					location+= sizeof(int);

					NextPagePosition = BitConverter.ToInt64(value, location);

				}
			}

		}

		/// <summary>
		/// A support class to hold a block related information including the block number, block starting position in file.
		/// </summary>
		private class PageLocation
		{
			public readonly int PageNumber;
			public readonly int StartingIndex;
			public readonly long Position;
			public int ItemCount;

			public PageLocation(int page, int startingIndex, long position, int count)
			{
				PageNumber		= page;
				StartingIndex	= startingIndex;
				Position		= position;
				ItemCount		= count;
			}
		}

		/// <summary>
		/// A support class to locate a specific key in the data file.
		/// </summary>
		private class KeyLocation
		{
			public readonly PageLocation Page;
			public readonly int Index;
			public long Position = 0;

			public KeyLocation(PageLocation page, int index)
			{
				Page = page;
				Index = index;
			}

			public KeyLocation(PageLocation page, int index, long position)
			{
				Page = page;
				Index = index;
				Position = position;
			}
		}

		private bool _disposed		= false;
		private int _maxRowsInMem	= 0;
		private int _currentPage	= 0;
		private string _fileName		= null;
		private FileStream _fileStream		= null;

		private Object DataLockObject	= new Object();

		private List<PageLocation> PageList;
		private Dictionary<KeyList<T>, KeyLocation> KeyLookupIndex;



		/// <summary>
		/// DataFile Constructor
		/// </summary>
		public DataFile(int maxRowsInMemory)
			: base(new KeyList<T>(), 0)
		{
			FileHeader.ID = Guid.NewGuid();
			FileHeader.Timestamp = DateTime.Now;
			_maxRowsInMem = maxRowsInMemory;
			KeyLookupIndex = new Dictionary<KeyList<T>, KeyLocation>(1024, new KeyList<T>());
			PageList = new List<PageLocation>(1024);
			PageList.Add(new PageLocation(0, 0, FileHeader.FILE_HEADER_SIZE, 0));
			CurrentPageIndex = 0;


		}


		public DataFile(int maxRowsInMemory, string FileName)
			: base(new KeyList<T>(), 32)
		{
			_maxRowsInMem = maxRowsInMemory;
			_fileName = FileName;

			KeyLookupIndex = new Dictionary<KeyList<T>, KeyLocation>(1024, new KeyList<T>());
			PageList = new List<PageLocation>(1024);

			LoadPage(0);
			PageWalk();
		}


		~DataFile()
		{
			Dispose(false);
		}


		#region IDisposable Members and Dispose()

		public bool IsDisposing
		{
			get { return this._disposed; }
		}


		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}


		protected virtual void Dispose(bool disposing)
		{
			if (!this._disposed)
			{
				if (disposing)
				{
					if (_fileStream != null)
					{
						_fileStream.Close();
						_fileStream.Dispose();
						_fileStream = null;

						if (_fileName != null)
							File.Delete(_fileName);
					}
				}
				// Release unmanaged resources. If disposing is false, 
				// only the following code is executed.
			}
			_disposed = true;
		}

		#endregion


		public Guid ID
		{
			get { return FileHeader.ID; }
		}

		private int CurrentPageIndex
		{
			set { _currentPage = value; }
		}

		private PageLocation CurrentPage
		{
			get { return PageList.Count == 0 ? null : PageList[_currentPage]; }
		}

		private PageLocation LastPage
		{
			get { return PageList.Count == 0 ? null : PageList[PageList.Count - 1]; }
		}

		public int PageCount
		{
			get { return PageList.Count; }
		}

		public bool IsLastPage
		{
			get { return PageList.Count == 0 ? true : LastPage.PageNumber == _currentPage; }
		}

		public bool HasPendingData
		{
			get { return _fileStream.CanWrite && IsLastPage && base.Count > 0; }
		}


		public string Name
		{
			get { return new T().Name; }
		}


		public string FileName
		{
			get
			{
				if (_fileName == null) { bool notUsed = FileWriteStream.CanRead; };
				return _fileName;
			}
			private set { _fileName = value; }
		}

		public FileStream FileWriteStream
		{
			get
			{
				if (this._disposed)
					throw new ObjectDisposedException(String.Format("{0}<{1}>", this.GetType().Name, typeof(T).Name));

				if (_fileStream == null)
				{

					_fileStream = new FileStream(Path.GetTempFileName(), FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None, FILE_BLOCKING_SIZE, FileOptions.None);
					//Replace with above line with the following line to prevent temp files from being deleted
					//_fileStream = new FileStream(Path.GetTempFileName(), FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None, FILE_BLOCKING_SIZE, FileOptions.None);
					_fileName = _fileStream.Name;
					WriteFileHeader();
				}
				return _fileStream;
			}
		}

		public FileStream FileReadStream
		{
			get
			{
				if (this._disposed)
					throw new ObjectDisposedException(String.Format("{0}<{1}>", this.GetType().Name, typeof(T).Name));

				if (_fileStream == null)
				{
					if (_fileName == null)
					{
						throw new NullReferenceException("File cannot be opened with a null _fileName.");
					}

					if (!File.Exists(_fileName))
					{
						throw new ApplicationException(_fileName + " does not exist.");
					}
					_fileStream = new FileStream(_fileName, FileMode.Open, FileAccess.Read, FileShare.Read, FILE_BLOCKING_SIZE, FileOptions.RandomAccess);
					ReadFileHeader();
					RebuildPageList();
				}
				return _fileStream;
			}
		}

		private void RebuildPageList()
		{
			if (this._disposed)
				throw new ObjectDisposedException(String.Format("{0}<{1}>", this.GetType().Name, typeof(T).Name));

			int cnt = 0;
			long position = FileHeader.FILE_HEADER_SIZE;
			_fileStream.Position = position;
			while (_fileStream.Position < _fileStream.Length - PageHeader.PAGE_HEADER_SIZE)
			{
				byte[] header = new byte[PageHeader.PAGE_HEADER_SIZE];
				_fileStream.Read(header, 0, PageHeader.PAGE_HEADER_SIZE);

				PageHeader.Buffer = header;

				System.Diagnostics.Debug.Assert(cnt++ == PageHeader.PageNumber);

				PageList.Add(new PageLocation(PageHeader.PageNumber, PageHeader.StartingIndex, position, PageHeader.ItemCount));

				position = PageHeader.NextPagePosition;

				if (position == 0)
					break;

				_fileStream.Position = position;
			}
		}

		/// <summary>
		/// /// <summary>
		/// Write the header for a new dataFile file.
		/// <remarks>File Position is left at the end of the header.</remarks>
		/// </summary>
		private void WriteFileHeader()
		{
			if (this._disposed)
				throw new ObjectDisposedException(String.Format("{0}<{1}>", this.GetType().Name, typeof(T).Name));


			lock (DataLockObject)
			{
				_fileStream.Position = 0;
				_fileStream.Write(FileHeader.Buffer, 0, FileHeader.FILE_HEADER_SIZE);
				_fileStream.Flush();
			}
		}


		private byte[] ReadFileHeader()
		{
			if (this._disposed)
				throw new ObjectDisposedException(String.Format("{0}<{1}>", this.GetType().Name, typeof(T).Name));

			byte[] header = new byte[FileHeader.FILE_HEADER_SIZE];

			lock (DataLockObject)
			{

				FileWriteStream.Position = 0;
				FileWriteStream.Read(header, 0, FileHeader.FILE_HEADER_SIZE);

				FileHeader.Buffer = header;
			}
			return header;
		}


		private void WritePageHeader(long pageDataLength)
		{
			if (this._disposed)
				throw new ObjectDisposedException(String.Format("{0}<{1}>", this.GetType().Name, typeof(T).Name));

			lock (DataLockObject)
			{
				FileWriteStream.Position = CurrentPage.Position;
				PageHeader.SetHeaderValues(CurrentPage, pageDataLength + PageHeader.PAGE_HEADER_SIZE);
				FileWriteStream.Write(PageHeader.Buffer, 0, PageHeader.PAGE_HEADER_SIZE);
			}
		}





		public void WriteBlock(byte[] data)
		{
			if (this._disposed)
				throw new ObjectDisposedException(String.Format("{0}<{1}>", this.GetType().Name, typeof(T).Name));


			FileWriteStream.Write(data, 0, data.Length);
		}



		public void CreateNewPage()
		{
			if (this._disposed)
				throw new ObjectDisposedException(String.Format("{0}<{1}>", this.GetType().Name, typeof(T).Name));


			lock (DataLockObject)
			{
				FileWriteStream.Position = FileWriteStream.Length;

				PageLocation lastPage = PageList[PageCount - 1];

				int startingIndex = lastPage.StartingIndex + lastPage.ItemCount;

				PageLocation pg = new PageLocation(PageCount, startingIndex, FileWriteStream.Position, 0);

				PageList.Add(pg);

				CurrentPageIndex = pg.PageNumber;
			}
		}


		public void PageWalk()
		{
			if (this._disposed)
				throw new ObjectDisposedException(String.Format("{0}<{1}>", this.GetType().Name, typeof(T).Name));

			for (int i = 0; i < PageCount; i++)
				LoadPage(i);

		}


		public void LoadPage(int page)
		{
			if (this._disposed)
				throw new ObjectDisposedException(String.Format("{0}<{1}>", this.GetType().Name, typeof(T).Name));


			ClearItems();
			CurrentPageIndex = page;

			// note the reference to FileReadStream caused the block header to get loaded and the PageList to be populated.
			if (PageList == null || FileReadStream == null)
			{
				System.Diagnostics.Debug.Assert(false, "PageList == null");
				return;
			}

			if (PageList.Count == 0)
				return;

			FileReadStream.Position = PageList[page].Position + PageHeader.PAGE_HEADER_SIZE;

			for (int i = 0; i < CurrentPage.ItemCount; i++)
			{
				T t = new T();
				long position = FileReadStream.Position;
				t.ReadFrom(FileReadStream);
				KeyList<T> key = new KeyList<T>(t);
				KeyLocation location;

				if (KeyLookupIndex.TryGetValue(key, out location))
				{
					location.Position = position;
				}
				else
				{
					KeyLookupIndex.Add(key, new KeyLocation(PageList[page], KeyLookupIndex.Count + 1, position));
				}
				base.Add(t);
			}
		}


		private int FindPageForIndex(int itemIndex)
		{
			if (this._disposed)
				throw new ObjectDisposedException(String.Format("{0}<{1}>", this.GetType().Name, typeof(T).Name));


			//	return PageList.FindIndex(indexsearch);
			return PageList.FindIndex(delegate(PageLocation p)
				{
					if (itemIndex >= p.StartingIndex && itemIndex < p.StartingIndex + p.ItemCount)
						return true;
					else
						return false;
				});
		}



		/// <summary>
		/// Write the block to disk
		/// </summary>
		/// <returns></returns>
		public void CommitCurrentPage()
		{
			if (this._disposed)
				throw new ObjectDisposedException(String.Format("{0}<{1}>", this.GetType().Name, typeof(T).Name));

			int cnt = 0;
			using (MemoryStream pageStream = new MemoryStream())
			{
				lock (DataLockObject)
				{
					if (Count == 0)
						return;

					foreach (T t in Items)
					{
						t.WriteTo(pageStream);
						cnt++;
					}
					//TODO remove debug	
					System.Diagnostics.Debug.Assert(cnt == CurrentPage.ItemCount);
					//byte[] debug = pageStream.ToArray() ;


					WritePageHeader(pageStream.Length);
					FileWriteStream.Write(pageStream.ToArray(), 0, (int)pageStream.Length);
					FileWriteStream.Flush();
				}
			}
		}



		public new int Count
		{
			get
			{
				if (this._disposed)
					throw new ObjectDisposedException(String.Format("{0}<{1}>", this.GetType().Name, typeof(T).Name));

				if (PageList.Count == 0)
					return 0;

				PageLocation lastPage = PageList[PageList.Count - 1];
				if (_currentPage == lastPage.PageNumber)
				{
					return lastPage.StartingIndex + base.Count;
				}
				return lastPage.StartingIndex + lastPage.ItemCount;
			}
		}


		public new DatabaseRowTemplate this[KeyList<T> key]
		{
			get
			{
				if (this._disposed)
					throw new ObjectDisposedException(String.Format("{0}<{1}>", this.GetType().Name, typeof(T).Name));

				if (key == null)
					throw new ArgumentNullException("key", "DataFile key cannot be null");

				KeyLocation location = null;
				KeyLookupIndex.TryGetValue(key, out location);
				if (location == null)
				{
					throw new System.Collections.Generic.KeyNotFoundException(String.Format("An element with the key \"{0}\" does not exist in the collection.", key));
				}
				if (location.Page != CurrentPage)
				{
					// This block was already index, just jump right the the needed data, build the object and restore the file position.
					if (location.Position != 0)
					{
						long savePosition = FileWriteStream.Position;
						T t = new T();
						FileWriteStream.Position = location.Position;
						t.ReadFrom(FileWriteStream);
						FileWriteStream.Position = savePosition;
						return t;
					}
					if (HasPendingData)
					{
						CommitCurrentPage();
					}
					LoadPage(location.Page.PageNumber);
				}
				return base[key];
			}
			set
			{
				if (this._disposed)
					throw new ObjectDisposedException(String.Format("{0}<{1}>", this.GetType().Name, typeof(T).Name));


				if (key == null)
					throw new ArgumentNullException("key", "DataFile key cannot be null");

				//if (!KeyLookupIndex.ContainsKey(key))
				//{
				//    throw new System.Collections.Generic.KeyNotFoundException(String.Format("An element with the key \"{0}\" does not exist in the collection.", key));
				//}


				KeyLocation location = null;
				KeyLookupIndex.TryGetValue(key, out location);
				if (location == null)
				{
					throw new System.Collections.Generic.KeyNotFoundException(String.Format("An element with the key \"{0}\" does not exist in the collection.", key));
				}
				if (location.Page == CurrentPage)
				{
					//base.SetItem(KeyLookupIndex[key].Index, value);
					base.SetItem(location.Index - location.Page.StartingIndex, value);
					if (location.Page != LastPage)
					{
						CommitCurrentPage();
					}
				}
				else
				{
					// This block was already index, just jump right the the needed data, build the object and restore the file position.
					if (location.Position != 0)
					{
						long savePosition = FileWriteStream.Position;
						T t = new T();
						FileWriteStream.Position = location.Position;
						long dataSize = t.ReadFrom(FileWriteStream);

						MemoryStream newData = new MemoryStream((int)dataSize);
						if (dataSize != value.WriteTo(newData))
							throw new ApplicationException("Cannot complete data updated becasue it would change the total data size of the record.");

						FileWriteStream.Position = location.Position;

						FileWriteStream.Write(newData.ToArray(), 0, (int)dataSize);

						FileWriteStream.Position = savePosition;

					}
					else
					{
						if (HasPendingData)
						{
							CommitCurrentPage();
						}
						LoadPage(location.Page.PageNumber);
						base.SetItem(KeyLookupIndex[key].Index, value);
						CommitCurrentPage();
					}
				}
			}
		}

		public new DatabaseRowTemplate this[int index]
		{
			get
			{
				if (this._disposed)
					throw new ObjectDisposedException(String.Format("{0}<{1}>", this.GetType().Name, typeof(T).Name));


				if (index < 0)
					throw new ArgumentOutOfRangeException("index", "Index cannot be less than zero.");

				if (index >= Count)
					throw new ArgumentOutOfRangeException("index", "Index excced size of collection.");


				// is the index in the current block return the value from here
				if (index >= CurrentPage.StartingIndex && index < CurrentPage.StartingIndex + base.Count)
					return base[index - CurrentPage.StartingIndex];

				// if we got this far, we are going to have a do a block load from disk
				int pageIndex = FindPageForIndex(index);
				if (pageIndex == -1)
					throw new ApplicationException("Unable to locate page for index");

				if (HasPendingData)
				{
					CommitCurrentPage();
				}

				LoadPage(pageIndex);

				return base[index - CurrentPage.StartingIndex];

			}
		}

		public new bool Contains(KeyList<T> key)
		{
			return KeyLookupIndex.ContainsKey(key);
		}


		public new void Add(DatabaseRowTemplate item)
		{
			if (this._disposed)
			{
				throw new ObjectDisposedException(String.Format("{0}<{1}>", this.GetType().Name, typeof(T).Name));
			}

			if (item == null)
				throw new ArgumentNullException("item");

			if (item.GetType() != typeof(T))
				throw new ArgumentException(String.Format("DataFile.Add expected a {0} but was passed a {1}", TableName, item.GetType().Name), "item");

			KeyList<T> key = new KeyList<T>((T)item);

			if (KeyLookupIndex.ContainsKey(key))
				throw new ArgumentException(String.Format("Key [{0}] already exsists in collection", key));


			if (PageList.Count > 1 && !IsLastPage)
			{
				LoadPage(LastPage.PageNumber);
			}

			lock (DataLockObject)
			{
				if (_maxRowsInMem > 0 && base.Count >= _maxRowsInMem)
				{
					CommitCurrentPage();
					ClearItems();


					CreateNewPage();

				}
				CurrentPage.ItemCount++;
				KeyLookupIndex.Add(key, new KeyLocation(CurrentPage, KeyLookupIndex.Count));
				base.Add(item);
			}
		}


		/// <summary>
		/// Clears the entire DataFile structure
		/// </summary>
		public new void Clear()
		{
			if (this._disposed)
			{
				throw new ObjectDisposedException(String.Format("{0}<{1}>", this.GetType().Name, typeof(T).Name));
			}

			lock (DataLockObject)
			{
				if (PageList.Count > 0)
				{

					if (CurrentPage.ItemCount > 0)
						CommitCurrentPage();

					PageList.Clear();
					KeyLookupIndex.Clear();
					ClearItems();

					KeyLookupIndex = new Dictionary<KeyList<T>, KeyLocation>(1024, new KeyList<T>());
					PageList = new List<PageLocation>(1024);
					PageList.Add(new PageLocation(0, 0, FileHeader.FILE_HEADER_SIZE, 0));
					CurrentPageIndex = 0;
				}
				//TODO More to add here
				if (_fileStream != null)
				{
					_fileStream.Close();
					_fileStream.Dispose();
					_fileStream = null;
				}
				FileHeader.ID = Guid.NewGuid();
			}
		}


		public void CommitPageAndCopyFile(string newFilePath)
		{
			long fileSize;
			lock (DataLockObject)
			{
				CommitCurrentPage();
				long savePosition = FileWriteStream.Position;

				FileWriteStream.Flush();
				FileWriteStream.Close();

				using (FileStream temp = File.Open(FileName, FileMode.Open, FileAccess.Read, FileShare.Read))
				{
					fileSize = temp.Length;
					temp.Close();
				}

				File.Copy(FileName, newFilePath, true);

				_fileStream = new FileStream(FileName, FileMode.Open, FileAccess.ReadWrite, FileShare.None, FILE_BLOCKING_SIZE, FileOptions.None);

				_fileStream.Position = savePosition;
			}

			bool goodOpen = false;
			int totalTries = 100;
			while (!goodOpen)
			{
				try
				{
					using (FileStream s = File.OpenRead(newFilePath))
					{
						goodOpen =  s.Length == fileSize;
						s.Close();
					}
				}
				catch (Exception)
				{
					if (totalTries-- == 0)
						throw;
					Thread.Sleep(50);
				}
			}

		}

		protected override void InsertItem(int index, DatabaseRowTemplate item)
		{
			base.InsertItem(index, item);
		}


		protected new void ChangeItemKey(DatabaseRowTemplate w, KeyList<T> key)
		{
			throw new NotSupportedException("ChangeItemKey() is not supported by DataFile.");
		}


		protected override void SetItem(int index, DatabaseRowTemplate item)
		{
			//base.SetItem(index, item);
			throw new NotSupportedException("SetItem() is not supported by DataFile.");
		}


		protected override void RemoveItem(int index)
		{
			//throw new NotSupportedException("RemoveItem() is not supported by DataFile.");
			base.RemoveItem(index);
		}



		protected override KeyList<T> GetKeyForItem(DatabaseRowTemplate row)
		{
			return new KeyList<T>((T)row);
		}



		protected override void ClearItems()
		{
			lock (DataLockObject)
			{
				base.ClearItems();
			}
		}

		public override int GetHashCode()
		{
			return ID.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return (obj is DataFile<T>) ? this == (DataFile<T>)obj : false;
		}

		public static bool operator==(DataFile<T> lhs, DataFile<T> rhs)
		{
			if (Object.Equals(lhs, null))
				if (Object.Equals(rhs, null))
					return true;
				else
					return false;
			else
				if (Object.Equals(rhs, null))
					return false;
				else
					return lhs.ID == rhs.ID;
		}

		public static bool operator!=(DataFile<T> lhs, DataFile<T> rhs)
		{
			return !(lhs==rhs);
		}


	} //public class DataFile<T> : KeyedCollection<KeyList<T>, DatabaseRowTemplate>, IDisposable where T : DatabaseRowTemplate, new()



	internal static class SortedColumns<T>
	{
//		public static Dictionary<String, int>	ColumnOrderList	= null;
		public static FieldInfo[]				SortedFieldInfo	= null;
		public static FieldInfo					TimestampFieldInfo = null;
		static SortedColumns()
		{
			SortedFieldInfo = typeof(T).GetFields();
			Array.Sort<FieldInfo>(SortedFieldInfo, delegate(FieldInfo x, FieldInfo y)
				{
					return DatabaseRowTemplate.GetOrdinalValue(x) - DatabaseRowTemplate.GetOrdinalValue(y);
				}
			);

			foreach (FieldInfo info in SortedFieldInfo)
			{
				if (DatabaseRowTemplate.GetTimestampValue(info))
				{
					if (info.FieldType != typeof(DateTime))
						throw new ApplicationException(String.Format("A timestamp field must be of type DateTime field {0} is type {1}", info.Name, info.FieldType.Name)); 
					
					if (TimestampFieldInfo != null)
						throw new ApplicationException(String.Format("Only one timestamp filed is allowed, {0} has two or more.", typeof(T).Name));

					TimestampFieldInfo = info;
				}
			}

			if (TimestampFieldInfo == null && typeof(T) != typeof(DatabaseRowTemplate))
				throw new ApplicationException(String.Format("Timestamp field is missing from type {0}", typeof(T).Name));
		}


		public static FieldInfo[] GetOrderForType(DatabaseRowTemplate recordType)
		{
			System.Diagnostics.Debug.Assert(recordType.Name == recordType.GetType().Name);
			switch (recordType.Name)
			{
			case "PropertyBag":
				return SortedColumns<PropertyBag>.SortedFieldInfo;

			case "ReaderCommand":
				return SortedColumns<ReaderCommand>.SortedFieldInfo;

			case "TagInventory":
				return SortedColumns<TagInventory>.SortedFieldInfo;

			case "InventoryCycle":
				return SortedColumns<InventoryCycle>.SortedFieldInfo;

			case "InventoryRound":
				return SortedColumns<InventoryRound>.SortedFieldInfo;

			case "ReaderRequest":
				return SortedColumns<ReaderRequest>.SortedFieldInfo;

			case "PacketStream":
				return SortedColumns<PacketStream>.SortedFieldInfo;

			case "ReaderAntennaCycle":
				return SortedColumns<ReaderAntennaCycle>.SortedFieldInfo;

			case "AntennaPacket":
				return SortedColumns<AntennaPacket>.SortedFieldInfo;

			case "TagRead":
				return SortedColumns<TagRead>.SortedFieldInfo;

			case "BadPacket":
				return SortedColumns<BadPacket>.SortedFieldInfo;

			case "ReadRate":
				return SortedColumns<ReadRate>.SortedFieldInfo;

			default:
				throw new ApplicationException(String.Format("Unknown/Unsupported type \"{0}\" in GetOrderForType().", recordType.GetType().Name));
			}

		}

		public static FieldInfo GetTimestampInfoForType(DatabaseRowTemplate recordType)
		{
			switch (recordType.Name)
			{
				case "PropertyBag":
					return SortedColumns<PropertyBag>.TimestampFieldInfo;

				case "ReaderCommand":
					return SortedColumns<ReaderCommand>.TimestampFieldInfo;

				case "TagInventory":
					return SortedColumns<TagInventory>.TimestampFieldInfo;

				case "InventoryCycle":
					return SortedColumns<InventoryCycle>.TimestampFieldInfo;

				case "InventoryRound":
					return SortedColumns<InventoryRound>.TimestampFieldInfo;

				case "ReaderRequest":
					return SortedColumns<ReaderRequest>.TimestampFieldInfo;

				case "PacketStream":
					return SortedColumns<PacketStream>.TimestampFieldInfo;

				case "ReaderAntennaCycle":
					return SortedColumns<ReaderAntennaCycle>.TimestampFieldInfo;

				case "AntennaPacket":
					return SortedColumns<AntennaPacket>.TimestampFieldInfo;

				case "TagRead":
					return SortedColumns<TagRead>.TimestampFieldInfo;

				case "BadPacket":
					return SortedColumns<BadPacket>.TimestampFieldInfo;

				case "ReadRate":
					return SortedColumns<ReadRate>.TimestampFieldInfo;

				default:
					throw new ApplicationException(String.Format("Unknown/Unsupported type \"{0}\" in GetTimestampNameForType().", recordType.GetType().Name));
			}

		}

	}


	/// <summary>
	/// 
	/// </summary>
	public  class DatabaseRowTemplate
	{
		private const BindingFlags flags =	BindingFlags.InvokeMethod	|
											BindingFlags.Instance		|
											BindingFlags.Public			|
											BindingFlags.Static;


		private const int MAX_STRING_SIZE = 5 * 1024;

		public static int GetOrdinalValue(FieldInfo info)
		{
			object[] attrs = info.GetCustomAttributes(typeof(DBFieldInfoAttribute), false);
			if (attrs == null || attrs.Length != 1)
				throw new Exception(String.Format("DBFieldInfoAttribure is missing from field {0}.", info.Name));

			return ((DBFieldInfoAttribute)attrs[0]).Ordinal;
		}

		public static bool GetTimestampValue(FieldInfo info)
		{
			object[] attrs = info.GetCustomAttributes(typeof(DBFieldInfoAttribute), false);
			if (attrs == null || attrs.Length != 1)
				throw new Exception(String.Format("DBFieldInfoAttribure is missing from field {0}.", info.Name));

			return ((DBFieldInfoAttribute)attrs[0]).Timestamp;
		}



		private FieldInfo[] GetSortedFieldInfo()
		{
			return SortedColumns<DatabaseRowTemplate>.GetOrderForType(this);
		}

		public DateTime GetTimestamp()
		{
			return (DateTime)SortedColumns<DatabaseRowTemplate>.GetTimestampInfoForType(this).GetValue(this);
		}

		public string Name
		{
			get { return this.GetType().Name; }
		}


		private Object[] ItemArray
		{
			get
			{
				FieldInfo[] fields = this.GetType().GetFields();
				Object[] result = new Object[fields.Length];
				for (int i = 0; i < fields.Length; i++)
				{
					int index = GetOrdinalValue(fields[i]);
					result[index] = (Object)fields[i].GetValue(this);
				}
				return result;
			}
			set
			{
				FieldInfo[] fields = this.GetType().GetFields();
				for (int i = 0; i < fields.Length; i++)
				{
					int index = GetOrdinalValue(fields[i]);
					fields[index].SetValue(this, (value[i] == null || value[i].GetType() == typeof(System.DBNull)) ? null : value[i]);
				}
			}
		}

		

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		
		public override bool Equals(object obj)
		{
			return (obj is DatabaseRowTemplate) ? this == (DatabaseRowTemplate)obj : false;
		}

		public static bool operator==(DatabaseRowTemplate lhs, DatabaseRowTemplate rhs)
		{
			if (Object.Equals(lhs, null))
			{
				if (Object.Equals(rhs, null))
				{
					return true;
				}
				else
				{
					return false;
				}
			}
			else
			{
				if (Object.Equals(rhs, null))
				{
					return false;
				}
				else
				{
					Type lhsType = lhs.GetType();
					Type rhsType = rhs.GetType();
					if (lhsType != rhsType)
					{
						return false;
					}
					else
					{
						MemoryStream lhsData = new MemoryStream();
						long lhsSize = lhs.WriteTo(lhsData);
						lhsData.Position = 0;

						MemoryStream rhsData = new MemoryStream();
						long rhsSize = rhs.WriteTo(rhsData);
						rhsData.Position = 0;
												
						if (lhsSize != rhsSize)
							return false;

						for (int i = 0; i < lhsData.Length; i++)
						{
							if (lhsData.ReadByte() != rhsData.ReadByte())
								return false;
						}
					}
				}
			}
			return true;
		}



		public static bool operator!=(DatabaseRowTemplate lhs, DatabaseRowTemplate rhs)
		{
			return !(lhs==rhs);
		}




		public long WriteTo(Stream stream)
		{
			long startLength = stream.Length;
			
			BinaryWriter binaryWriter = new BinaryWriter(stream);

			foreach (FieldInfo field in GetSortedFieldInfo())
			{
				Object obj = field.GetValue(this);
				Type t = field.FieldType;

				if (t == typeof(Object))
				{
					t = obj.GetType();
					binaryWriter.Write(t.Name);
				}

				if (t.IsArray)
				{
					if (obj == null)
					{
						binaryWriter.Write(0);
						continue;
					}
					int arraylength = ((Array)obj).Length;
					binaryWriter.Write(arraylength);

					Type elementType = t.GetElementType();
					switch (elementType.Name)
					{
					case "Byte":
					case "Char":
						binaryWriter.GetType().InvokeMember("Write", flags, null, binaryWriter, new object[] { obj });
						break;

					default:
						throw new ApplicationException(String.Format("DatabaseRowTemplate.WriteTo() encountered an unsupported array type \"{0}\" in {1}.", t.Name, this.GetType().FullName));

					}
				}	
				else
				{
					switch (t.Name)
					{
					case "String":
						string val = (string)obj;
						if (val == null)
						{
							binaryWriter.Write(0);
						}
						else
						{
							if (val.Length > MAX_STRING_SIZE)
							{
							//	throw new Exception(String.Format("Invalid string size of {0} found in {1}.", val.Length, this.GetType().FullName));
								System.Diagnostics.Debug.WriteLine(String.Format("Invalid string size of {0} found in {1}.", val.Length, this.GetType().FullName));
								val = val.Substring(0, MAX_STRING_SIZE - 5) + "...";
							}
							binaryWriter.Write(val.Length);
							binaryWriter.Write(val.ToCharArray());
						}
						break;

					case "DateTime":
						binaryWriter.Write(((DateTime)obj).ToBinary());
						break;

					case "TimeSpan":
						binaryWriter.Write(((TimeSpan)obj).Ticks);
						break;

					default:
						if (t.IsPrimitive)
						{
							binaryWriter.GetType().InvokeMember("Write", flags, null, binaryWriter, new object[] {obj});
						}
						else
						{
							Type underlyingType = Nullable.GetUnderlyingType(t);
							if (underlyingType != null)
							{
								if (Object.Equals(obj, null))
								{
									binaryWriter.Write((Int32)0);
								}
								else
								{
									switch (underlyingType.Name)
									{
									case "Boolean":
										binaryWriter.Write(sizeof(Boolean));
										binaryWriter.Write((bool)obj);
										break;

									case "Byte":
										binaryWriter.Write(sizeof(Byte));
										binaryWriter.Write((Byte)obj);
										break;

									case "SByte":
										binaryWriter.Write(sizeof(SByte));
										binaryWriter.Write((SByte)obj);
										break;

									case "Int16":
										binaryWriter.Write(sizeof(Int16));
										binaryWriter.Write((Int16)obj);
										break;

									case "UInt16":
										binaryWriter.Write(sizeof(UInt16));
										binaryWriter.Write((UInt16)obj);
										break;

									case "Int32":
										binaryWriter.Write(sizeof(Int32));
										binaryWriter.Write((Int32)obj);
										break;

									case "UInt32":
										binaryWriter.Write(sizeof(UInt32));
										binaryWriter.Write((UInt32)obj);
										break;
									
									case "Int64":
										binaryWriter.Write(sizeof(Int64));
										binaryWriter.Write((Int64)obj);
										break;

									case "UInt64":
										binaryWriter.Write(sizeof(UInt64));
										binaryWriter.Write((UInt64)obj);
										break;

									case "Char":
										binaryWriter.Write(sizeof(Char));
										binaryWriter.Write((Char)obj);
										break;

									case "Double":
										binaryWriter.Write(sizeof(Double));
										binaryWriter.Write((Double)obj);
										break;

									case "Single":
										binaryWriter.Write(sizeof(Single));
										binaryWriter.Write((Single)obj);
										break;

									case "DateTime":
										binaryWriter.Write(sizeof(long));
										binaryWriter.Write(((DateTime)obj).ToBinary());
										break;
									
									default:
										//if (underlyingType.IsPrimitive)
										//{
										//    binaryWriter.Write(System.Runtime.InteropServices.Marshal.SizeOf(underlyingType));
										//    binaryWriter.GetType().InvokeMember("Write", flags, null, binaryWriter, new object[] { obj });
										//}

										throw new ApplicationException(String.Format("DatabaseRowTemplate.WriteTo() encountered an unsupported nullable field type: {0}? ({1}))", underlyingType.Name, this.GetType().FullName));
									
									}
								}
							}
							else
							{
								throw new ApplicationException(String.Format("DatabaseRowTemplate.WriteTo() encountered an unsupported field type \"{0}\" ({1}))", t.Name, this.GetType().FullName));
							}
						}
						break;
					}
				}
			}			
			return stream.Length - startLength;
		}
		
		
		public long ReadFrom(Stream stream)
		{
			long startPosition = stream.Position;
			
//			List<Object> collection = new List<object>();

			BinaryReader binaryReader = new BinaryReader(stream);

			foreach (FieldInfo field in GetSortedFieldInfo())
			{
				Type t = field.FieldType;
				if (t == typeof(Object))
				{
					string typeName = binaryReader.ReadString();
					switch (typeName)
					{
					case "String":
						t = typeof(String);
						break;

					case "DateTime":
						t = typeof(DateTime);
						break;

					case "TimeSpan":
						t = typeof(TimeSpan);
						break;

					case "Boolean":
						t = typeof(Boolean);
						break;

					case "Byte":
						t = typeof(Byte);
						break;

					case "Char":
						t = typeof(Char);
						break;

					case "Double":
						t = typeof(Double);
						break;

					case "Int16":
						t = typeof(Int16);
						break;

					case "Int32":
						t = typeof(Int32);
						break;

					case "Int64":
						t = typeof(Int64);
						break;

					case "Single":
					case "Float":
						t = typeof(float);
						break;

					case "UInt16":
						t = typeof(UInt16);
						break;

					case "UInt32":
						t = typeof(UInt32);
						break;

					case "UInt64":
						t = typeof(UInt64);
						break;

					default:
						throw new Exception(String.Format("Encountered an unsupported object type \"{0}\" in {1}.", typeName, this.GetType().FullName));
					}
				}


				if (t.IsArray)
				{
					int arraylength = binaryReader.ReadInt32();
					if (arraylength == 0)
					{
						field.SetValue(this, null);
						continue;
					}
					Type elementType = t.GetElementType();
					switch (elementType.Name)
					{
					case "Byte":
						//collection.Add(binaryReader.ReadBytes(arraylength));
						field.SetValue(this, binaryReader.ReadBytes(arraylength));
						break;

					case "Char":
						//collection.Add(binaryReader.ReadChars(arraylength));
						field.SetValue(this, binaryReader.ReadChars(arraylength));
						break;

					default:
						throw new ApplicationException(String.Format("Encountered an unsupported array type \"{0}\" in {1}.", t.Name, this.GetType().FullName));
					}
				}
				else
				{
					switch (t.Name)
					{
					case "String":
						int stringLength = binaryReader.ReadInt32();

						if (stringLength > MAX_STRING_SIZE || stringLength < 0)
							throw new Exception(String.Format("String size of {0} is invalid, file corruption is likely in {1}.", stringLength, this.GetType().FullName));

						if (stringLength == 0)
						{
							field.SetValue(this, null);
						}
						else
						{
							//collection.Add(new String(binaryReader.ReadChars(stringLength)));
							field.SetValue(this, new String(binaryReader.ReadChars(stringLength)));
						}
						break;

					case "DateTime":
						field.SetValue(this, DateTime.FromBinary(binaryReader.ReadInt64()));
						break;

					case "TimeSpan":
						field.SetValue(this, TimeSpan.FromTicks(binaryReader.ReadInt64()));
						break;

					case "Boolean":
						field.SetValue(this, binaryReader.ReadBoolean());
						break;

					case "Byte":
						field.SetValue(this, binaryReader.ReadByte());
						break;

					case "Char":
						field.SetValue(this, binaryReader.ReadChar());
						break;

					case "Double":
						field.SetValue(this, binaryReader.ReadDouble());
						break;

					case "Int16":
						field.SetValue(this, binaryReader.ReadInt16());
						break;

					case "Int32":
						field.SetValue(this, binaryReader.ReadInt32());
						break;

					case "Int64":
						field.SetValue(this, binaryReader.ReadInt64());
						break;

					case "Single":
					case "Float":
						field.SetValue(this, binaryReader.ReadSingle());
						break;

					case "UInt16":
						field.SetValue(this, binaryReader.ReadUInt16());
						break;

					case "UInt32":
						field.SetValue(this, binaryReader.ReadUInt32());
						break;

					case "UInt64":
						field.SetValue(this, binaryReader.ReadUInt64());
						break;

					default:
						Type underlyingType = Nullable.GetUnderlyingType(t);
						if (underlyingType != null)
						{
							int size = binaryReader.ReadInt32();
							if (size == 0)
							{
								field.SetValue(this, null);
							}
							else
							{
								switch (underlyingType.Name)
								{
								case "Boolean":
									field.SetValue(this, binaryReader.ReadBoolean());
									break;

								case "Byte":
									field.SetValue(this, binaryReader.ReadByte());
									break;

								case "Char":
									field.SetValue(this, binaryReader.ReadChar());
									break;

								case "Double":
									field.SetValue(this, binaryReader.ReadDouble());
									break;

								case "Int16":
									field.SetValue(this, binaryReader.ReadInt16());
									break;

								case "Int32":
									field.SetValue(this, binaryReader.ReadInt32());
									break;

								case "Int64":
									field.SetValue(this, binaryReader.ReadInt64());
									break;

								case "Float":
									field.SetValue(this, binaryReader.ReadSingle());
									break;

								case "UInt16":
									field.SetValue(this, binaryReader.ReadUInt16());
									break;

								case "UInt32":
									field.SetValue(this, binaryReader.ReadUInt32());
									break;

								case "UInt64":
									field.SetValue(this, binaryReader.ReadUInt64());
									break;

								case "DateTime":
									field.SetValue(this, DateTime.FromBinary(binaryReader.ReadInt64()));
									break;

								default:
									throw new ApplicationException(String.Format("DatabaseRowTemplate.ReadFrom() encountered an unsupported nullable field type \"{0}\" ({1}))", t.Name, this.GetType().FullName));
								}
							}
						}
						else
						{
							throw new ApplicationException(String.Format("DatabaseRowTemplate.ReadFrom() encountered an unsupported field type \"{0}\" ({1}))", t.Name, this.GetType().FullName));
						}
						break;
				
					}
				}
			}
					
			//ItemArray = collection.ToArray();
			
			return stream.Position - startPosition;
		}

		public void ValidateOrdinalValues()
		{
			//System.Diagnostics.Debug.Assert(recordType.Name == recordType.GetType().Name);
			if (Name != GetType().Name)
				throw new ApplicationException(String.Format("DatabaseRowTemplate name for {0} is not correct.", this.GetType().Name));

			FieldInfo[] fields = GetSortedFieldInfo();

			for (int i = 0; i< fields.Length; i++)
			{
				if (GetOrdinalValue(fields[i]) != i)
					throw new ApplicationException(String.Format("Ordinal values for {0} are not correct.", this.GetType().Name));
			}
		}

	} // public  class DatabaseRowTemplate







	public class DataTools
	{
		public static DataTable DataTableBuilder(Type T)
		{

			DataTable table = new DataTable();
			List<DataColumn> primaryKeyList = new List<DataColumn>();
			table.TableName = T.Name;
			table.ExtendedProperties.Add("RowTemplate", T);
			foreach (FieldInfo info in T.GetFields())
			{
				DataColumn col;
				Type dataType		= info.FieldType;
				string columnName	= info.Name;
				DBFieldInfoAttribute myfieldInfo		= 
					info.IsDefined(typeof(DBFieldInfoAttribute), false) ? (DBFieldInfoAttribute)(info.GetCustomAttributes(typeof(DBFieldInfoAttribute), true)[0]) : null;

				bool allowDBNull	= false;
				bool unique			= false;
				string expression	= null;
				if (myfieldInfo != null)
				{
					columnName = myfieldInfo.ColumnName == null ? columnName : myfieldInfo.ColumnName;
					dataType = myfieldInfo.DBType == null ? dataType : myfieldInfo.DBType;
					allowDBNull = myfieldInfo.AllowNull;
					unique = !myfieldInfo.AllowDuplicate;
					expression = myfieldInfo.Expression;
				}

				if (expression == null)
				{
					col = new DataColumn(columnName, dataType);
					col.AllowDBNull = allowDBNull;
					col.Unique		= unique;
				}
				else
				{
					col = new DataColumn(columnName, dataType, expression);
				}
				table.Columns.Add(col);
				if (myfieldInfo != null && myfieldInfo.PrimaryKey) primaryKeyList.Add(col);
			}

			if (primaryKeyList.Count > 0)
				table.PrimaryKey = primaryKeyList.ToArray();

			return table;
		}


	} // public class DataTools




	/// <summary>
	/// Class for managing the state of reader functions
	/// </summary>
	public class FunctionControl
	{
		[Flags]
		public enum FunctionState
		{
			Unknown		= 0x00,

			Idle		= 0x01,

			Running		= 0x02,

			Paused		= 0x04,

			Stopping	= 0x08,

			Aborting	= 0x16,
		}

		public enum RequestedAction
		{
			Continue,

			Pause,

			Stop,

			Abort
		}

		[Flags]
		public enum SupportedActions
		{
			Stop			= 0x0,

			StopAbort		= 0x1,

			StopPause		= 0x2,

			StopAbortPause	= 0x3,
		}

		private  String _name		= "";
		private readonly SupportedActions _actions	= SupportedActions.Stop;

		public event EventHandler FunctionStateChanged;


		private FunctionState		_state		= FunctionState.Idle;
		private RequestedAction		_request	= RequestedAction.Continue;
		private ReaderWriterLock	_lock		= new ReaderWriterLock();
//		private AutoResetEvent		_continue	= new AutoResetEvent(false);
		private ManualResetEvent	_continue = new ManualResetEvent(false);
		private int					_timeout	= 5000;

		
		
		public FunctionControl() { }
		public FunctionControl(SupportedActions actions)
		{
			_actions = actions;
		}

		public FunctionControl(SupportedActions actions, string name)
		{
			_actions = actions;
			_name = name;
		}


		public String Name
		{
			get { return _name; }
			set { _name = value; }	
		}

		public SupportedActions Actions
		{
			get { return _actions; }
		}

		public FunctionState State
		{
			get 
			{
				_lock.AcquireReaderLock(_timeout);
				try
				{
					return _state;
				}
				finally
				{
					_lock.ReleaseReaderLock();
				}
			}
		}

		private RequestedAction Request
		{
			get
			{
				_lock.AcquireReaderLock(_timeout);
				try
				{
					return _request;
				}
				finally
				{
					_lock.ReleaseReaderLock();
				}
			}
		}

		public RequestedAction GetActionRequest()
		{
			RequestedAction result = Request;
			switch (result)
			{
			case RequestedAction.Pause:
				_lock.AcquireWriterLock(_timeout);
				try
				{
					_state = FunctionState.Paused;
					_continue.Reset();
				}
				finally
				{
					_lock.ReleaseWriterLock();
				}

				
				if (FunctionStateChanged != null)
				{
					FunctionStateChanged(this, EventArgs.Empty);
				}

				_continue.WaitOne();

				_lock.AcquireWriterLock(_timeout);
				try
				{
					_state = FunctionState.Running;
					result = _request = RequestedAction.Continue;
				}
				finally
				{
					_lock.ReleaseWriterLock();
				}
				break;

			case RequestedAction.Continue:
				break;

			case RequestedAction.Stop:
				_lock.AcquireWriterLock(_timeout);
				try
				{
					_state = FunctionState.Stopping;
				}
				finally
				{
					_lock.ReleaseWriterLock();
				}
				break;


			case RequestedAction.Abort:
				_lock.AcquireWriterLock(_timeout);
				try
				{
					_state = FunctionState.Aborting;
				}
				finally
				{
					_lock.ReleaseWriterLock();
				}
				break;


			default:
				break;
			}



			if (FunctionStateChanged != null)
			{
				System.Diagnostics.Debug.Assert(FunctionStateChanged.GetInvocationList().Length == 1);
				FunctionStateChanged(this, EventArgs.Empty);
			}

			return result;
		}


		public void Finished()
		{
			_lock.AcquireWriterLock(_timeout);
			try
			{
				_state = FunctionState.Idle;
			}
			finally
			{
				_lock.ReleaseWriterLock();
			}

			EventHandler theInvoker = FunctionStateChanged;
			if (theInvoker != null)
			{
				theInvoker(this, EventArgs.Empty);
			}
		}

		public FunctionState RequestStop()
		{
			if (State == FunctionState.Idle) return State;

			_lock.AcquireWriterLock(_timeout);
			try
			{
				_request = RequestedAction.Stop;
			}
			finally
			{
				_lock.ReleaseWriterLock();
			}
			return State;
		}

		public FunctionState RequestAbort()
		{
			if (State == FunctionState.Idle) return State;

			_lock.AcquireWriterLock(_timeout);
			try
			{
				if ((_actions & SupportedActions.StopAbort) != SupportedActions.StopAbort)
					throw new NotSupportedException("RequestAbort is not supported by this instance of FunctionControl");

				_request = RequestedAction.Abort;
			}
			finally
			{
				_lock.ReleaseWriterLock();
			}
			return State;
		}

		public FunctionState RequestPause()
		{
			if (State == FunctionState.Idle) return State;


			_lock.AcquireWriterLock(_timeout);
			try
			{
				if ((_actions & SupportedActions.StopPause) != SupportedActions.StopPause)
					throw new NotSupportedException("RequestPause is not supported by this instance of FunctionControl");

				_request = RequestedAction.Pause;
			}
			finally
			{
				_lock.ReleaseWriterLock();
			}

			return State;
		}

		public FunctionState Continue()
		{
			if (State  == FunctionState.Idle)
				return State;

			_lock.AcquireWriterLock(_timeout);
			try
			{
				_request = RequestedAction.Continue;
				_continue.Set();
			}
			finally
			{
				_lock.ReleaseWriterLock();
			}
			return State;
		}


		public void StartAction()
		{
			_lock.AcquireWriterLock(_timeout);
			try
			{
				_state = FunctionState.Running;
				_request = RequestedAction.Continue;
			}
			finally
			{
				_lock.ReleaseWriterLock();
			}

			EventHandler theInvoker = FunctionStateChanged;
			if (theInvoker != null)
			{
				theInvoker(this, EventArgs.Empty);
			}
		}

	} // public class FunctionControl





	public static class ExponentialMovingAverage
	{
		public const int n = 4;
		const double alpha			= ((double)2) / (double)(n + 1);
//		const double oneMinusAlpha	= (((double)1) - alpha);
		public static double Calculate(double priorValue, double nextMeasurment)
		{
//			return (alpha * priorValue) + (oneMinusAlpha * nextMeasurment);
			return (alpha * (nextMeasurment - priorValue)) + priorValue;
		}
	}

	/// <summary>
	/// struct to hold ReadCycle so it it a unique type (i.e. not just another integer)
	/// </summary>
	internal struct ReadCycle
	{
		private const int INVALID_CYCLE = -1;

		public static ReadCycle Missing = new ReadCycle(INVALID_CYCLE);

		private int _cycle;

		public ReadCycle(int cycle)
		{
			_cycle = cycle;
		}

		public ReadCycle(ReadCycle rc)
		{
			this._cycle = rc.Value;
		}

		
		public int Value
		{
			get { return _cycle; }
		}

		public override string ToString()
		{
			if (Value == INVALID_CYCLE)
				return "Missing ReadCycle";

			return String.Format("{0}: Cycle={1:d3}", "ReadCycle", Value);
		}

		public override int GetHashCode()
		{
			return Value.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (obj == null) return false;

			if (obj.GetType() != typeof(ReadCycle))
				return false;

			ReadCycle rc = (ReadCycle)obj;

			return this.Value == rc.Value;
		}

		public static ReadCycle operator-(ReadCycle rc, int n)
		{
			return new ReadCycle(rc.Value - n);
		}

		public static ReadCycle operator+(ReadCycle rc, int n)
		{
			return new ReadCycle(rc.Value + n);
		}


		public static bool operator ==(ReadCycle p1, ReadCycle p2)
		{
			return (p1.Equals(p2));
		}

		public static bool operator !=(ReadCycle p1, ReadCycle p2)
		{
			return !(p1 == p2);
		}


	}

	/// <summary>
	/// Caculates the Exponential Moving Average 
	/// </summary>
	public class EMA
	{
		public static EMA Empty = null;
		public static int PeriodFromAlpha(Double alpha)	{ return (int)Math.Round((2.0 / alpha) - 1); 	}
		public static double AlphaFromPeriod(int period) { return ((double)2) / (double)(period + 1);	}

		private double	_value = double.NaN;
		private double	_alpha = double.NaN;
		private double	_priorEMA = double.NaN;
		private double	_measurement = double.NaN;
		private int		_periods = int.MinValue;

	

		public EMA(double alpha, double priorEmaValue, double newMeasurement)
		{
			if (alpha <= 0 || alpha >= 1)
				throw new ArgumentOutOfRangeException("alpha", alpha, "Alpha must be > 0 and < 1");

			_alpha = alpha;
			_priorEMA = priorEmaValue;
			_measurement = newMeasurement;
		}

		public EMA(EMA priorEMA, double measurement)
		{
			_alpha = priorEMA.Alpha;
			_priorEMA = priorEMA.Value;
			_measurement = measurement;
		}

		public double Alpha
		{
			get { return _alpha; }
		}

		public int Periods
		{
			get 
			{
				try
				{

					return _periods != int.MinValue ? _periods :
						(_periods = (int)Math.Round((2.0 / _alpha) - 1)); 
				}
				catch (Exception e)
				{
					throw new Exception("Unable to caculate period for " + _alpha.ToString(), e);
				}

			}
		}

		public double Value
		{
			get 
			{
				if (!Double.IsNaN(_value)) return _value;

				if (Double.IsNaN(_alpha) || Double.IsNaN(_priorEMA) || Double.IsNaN(_measurement)) 
					throw new Exception(String.Format("Unable to caculate EMA alpha={0}, priorEMA={1}, newMeasurement={2}.", _alpha, _priorEMA, _measurement));

				try
				{
					_value = (_alpha * (_measurement - _priorEMA)) + _priorEMA;
				}
				catch (Exception e)
				{
					_value = Double.NaN;
					throw new Exception(String.Format("Unable to caculate EMA alpha={0}, priorEMA={1}, newMeasurement={2}.", _alpha, _priorEMA, _measurement), e);
					
				}
				return _value;
			}
		}

	

		public static bool HasValue(EMA ema)
		{
			return (ema != Empty						&&
					ema._alpha != Double.NaN			&&
					ema._priorEMA != Double.NaN			&&
					ema._measurement != Double.NaN	) 	? true : false;
		}


		public override string ToString()
		{
			return String.Format("EMA({0}) = {1}", Periods, Value);
		}

		public override int GetHashCode()
		{
			return _alpha.GetHashCode() * _priorEMA.GetHashCode() * _measurement.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (obj == null) return false;

			if (obj.GetType() != typeof(EMA))
				return false;

			EMA ema = (EMA)obj;

			return this.Alpha == ema.Alpha && this.Value == ema.Value;
		}


		public static bool operator ==(EMA p1, EMA p2)
		{
			if (Object.Equals(p1, null))
				return (Object.Equals(p2, null));
			return (p1.Equals(p2));
		}

		public static bool operator !=(EMA p1, EMA p2)
		{
			return !(p1 == p2);
		}


	}//public class EMA

	/// <summary>
	/// Holds the triple composed of a tag id, a read cycle and the Exponential Moving Average of the tags's Reads / Cycle
	/// </summary>
	public class TagCycleEMA
	{
		public static TagCycleEMA Empty = new TagCycleEMA();

		public int Reads = 0;
		
		public string		Tag			= null;
		private ReadCycle	_cycle		= ReadCycle.Missing;
		private EMA			_EMA		= EMA.Empty;
		
		public int?			_priorReads = null;


		private TagCycleEMA()
		{ }

		internal TagCycleEMA(string tag, ReadCycle cycle)
		{
			Tag = tag;
			_cycle = cycle;
		}

		// Copy Constructor
		internal TagCycleEMA(TagCycleEMA source)
		{
			this.Reads	= source.Reads;
			this.Tag	= source.Tag;
			this._cycle = source._cycle;
			this._EMA	= source.EMA;
			this._priorReads = source._priorReads;
		}


		public int TotalReadsForTag
		{
			get 
			{
				if (Cycle.Value == 0) return Reads;
				
				if (_priorReads.HasValue) return _priorReads.Value + Reads; 

				throw new Exception(String.Format("The object ({0}) does not have a value for total reads.", this.ToString())); 
			}
		}

		public Double Mean
		{
			get 
			{
				if (Cycle.Value == 0) return Reads;

				if (_priorReads.HasValue) return (Double)(_priorReads.Value + Reads) / (Double)(Cycle.Value + 1);

				throw new Exception(String.Format("The object ({0}) does not have a mean value.", this.ToString()));
			}
		}

		internal ReadCycle Cycle
		{
			get { return _cycle; }
		}

		public EMA EMA
		{
			get { if (EMA.HasValue(_EMA)) return _EMA; else throw new Exception(String.Format("The object ({0}) does not have an EMA value.", this.ToString())); }
			set { _EMA = value; }
		}

		public int PriorReads
		{
			set { _priorReads = value; }
		}

		public bool HasEMA			{	get { return EMA.HasValue(_EMA);									}	}
		public bool HasTotalReads	{	get { return Cycle.Value == 0 ? true : _priorReads.HasValue;		}	}
		public bool HasMean			{	get { return HasTotalReads;											}	}

		/// <summary>
		/// Sets the value for the sum of all reads of the tag prior to this cycle
		/// </summary>
		/// <param name="prior"></param>
		/// <returns></returns>
		public TagCycleEMA SetPriorReads(TagCycleEMA prior)
		{
			if (!prior.HasTotalReads)
				throw new ArgumentException(String.Format("{0} does not have a value for total reads.", prior), "prior");

			// catch the EMA if it happens to be a available.
			if (prior.HasEMA) SetPriorEma(prior);

			_priorReads = prior.TotalReadsForTag;
			
			return this;
		}

		public TagCycleEMA SetPriorEma(TagCycleEMA prior)
		{
			if (!prior.HasEMA)
				throw new ArgumentException(String.Format("{0} does not have a value for EMA.", prior), "prior");

			// catch the TotalReads if it happens to be available.
			if (prior.HasTotalReads) _priorReads = prior.TotalReadsForTag;

			EMA = new EMA(prior.EMA, Reads);

			return this;
		}


		public void ClearStats() 
		{
			_EMA = EMA.Empty;
			_priorReads = null;
		}


		
		/// <summary>
		/// Add a tag read to the cycle 
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public static TagCycleEMA operator ++(TagCycleEMA item)
		{
			item.Reads++;
			return item;
		}

		public override string ToString()
		{
			if (String.IsNullOrEmpty(Tag))
				return "The Empty Tag-Cycle-EMA";

			return String.Format("{0}: Cycle={1}, Reads={2} Mean={3} EMA={4}", Tag.Substring(Tag.Length - 10), Cycle, Reads, HasMean ? Mean : Double.NaN, HasEMA ? EMA.Value : Double.NaN);
		}

		public override int GetHashCode()
		{
			return Tag.GetHashCode() * Cycle.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (obj == null) return false;

			if (obj.GetType() != typeof(TagCycleEMA))
				return false;

			TagCycleEMA tce = (TagCycleEMA)obj;
			
			if (String.IsNullOrEmpty(this.Tag) && String.IsNullOrEmpty(tce.Tag))
				return true;

			return this.Tag == tce.Tag && this.Cycle == tce.Cycle;
		}


		public static bool operator ==(TagCycleEMA p1, TagCycleEMA p2)
		{
			return (p1.Equals(p2));
		}

		public static bool operator !=(TagCycleEMA p1, TagCycleEMA p2)
		{
			return !(p1 == p2);
		}


	}


	


	/// <summary>
	/// 
	/// </summary>
	public class TagCycleMatrix  
	{
		private class CycleList : List<TagCycleEMA>
		{
			private string	_tag;
			private int _emaPeriods;
			public string Tag
			{
				get { return _tag; }
				set { _tag = value; }
			}

			public CycleList(string tag, int emaPeriods)
				: base(emaPeriods * 3)
			{
				_tag = tag;
				_emaPeriods = emaPeriods;
			}


			public  TagCycleEMA this[ReadCycle readCycle]
			{
				get
				{
					if (readCycle == ReadCycle.Missing)
						throw new ArgumentOutOfRangeException("Cannot take the index with a value of ReadCycle.Missing.");


					int index = base.FindLastIndex(delegate(TagCycleEMA p){	return p.Cycle == readCycle;	});

					if (index == -1)
					{
						
						if (base.Count == 0)
						{
							base.Add(new TagCycleEMA(Tag, new ReadCycle(0)));
						}

						TagCycleEMA lastKnownValue = this[base.Count - 1];

						while (lastKnownValue.Cycle.Value < readCycle.Value)
						{
							if (base.Count >= base.Capacity) 
							{
								TagCycleEMA one = base[1]; // The element one will soon have an index of zero
								if (one.Cycle.Value > _emaPeriods && !one.HasEMA)
								{
									if (base[0].HasEMA)
									{
										one.SetPriorEma(base[0]);
									}
									else
									{
										one.EMA = new EMA(EMA.AlphaFromPeriod(_emaPeriods), one.Reads, one.Reads);
									}
								}
								if (!one.HasTotalReads)
								{
									if (base[0].HasTotalReads)
									{
										one.SetPriorReads(base[0]);
									}
									else
									{
										one.PriorReads = 0;
									}
								}
								base.RemoveAt(0);
							}
							base.Add(new TagCycleEMA(Tag, new ReadCycle(lastKnownValue.Cycle + 1)));
							lastKnownValue = this[base.Count - 1];
						}
						System.Diagnostics.Debug.Assert(lastKnownValue.Cycle == readCycle);

						return lastKnownValue;
					}

					TagCycleEMA result = base[index];
					System.Diagnostics.Debug.Assert(result.Cycle == readCycle);
					return result;
				}
			}
	
	
			public new void Add(TagCycleEMA item)
			{
				
				if (item == TagCycleEMA.Empty)
					throw new ArgumentOutOfRangeException("item", "TagCycleEMA.Empty cannot be added to a CycleList.");

				if (item.Tag != Tag)
					throw new ArgumentOutOfRangeException("item", item, String.Format("Must have a tag = {0} to add to this CycleList", Tag));

				base.Add(item);
			}


		} // private class CycleList : List<TagCycleEMA>


		/// <summary>
		/// Holds a KeyedCollection of Tag-Cycles (indexed by tag id)
		/// </summary>
		private class TagCycleList : KeyedCollection<string, CycleList>
		{
			private int _emaPeriods;
			public TagCycleList(int emaPeriods)
			{
				_emaPeriods = emaPeriods;
			}

			public ReadCycle GetReadCycle(string tagID, int cycleNumber)
			{
				return new ReadCycle(cycleNumber);
			}


			public TagCycleEMA AddTagCycle(string tagID, ReadCycle cycle)
			{
				if (String.IsNullOrEmpty(tagID))
					throw new ArgumentNullException("tagID", "A null or empty tagID cannot be added to a TagCycleList.");

				if (cycle == ReadCycle.Missing)
					throw new ArgumentOutOfRangeException("cycle", "Cannot add a ReadCycle.Missing value to a TagCycleList.");


				if (!Contains(tagID))
				{
					Add(new CycleList(tagID, _emaPeriods));
				}

				CycleList lst = this[tagID];
				if (lst.Count > 0)
				{
					for (int i = lst.FindLastIndex(delegate(TagCycleEMA p) { return p.Cycle == cycle; }); i != -1 && i < lst.Count; i++)
					{
						lst[i].ClearStats();
					}
				}

				TagCycleEMA result = this[tagID][cycle];


				result++;

				return result;
			}

			public TagCycleEMA GetMeanReadsPerCycle(string tagID, ReadCycle cycle)
			{
				if (String.IsNullOrEmpty(tagID))
					throw new ArgumentNullException("tagID", "The tagID cannot be null or empty.");

				if (cycle == ReadCycle.Missing)
					throw new ArgumentOutOfRangeException("cycle", "ReadCycle.Missing cannot be passed to GetMeanReadsPerCycle.");

				if (cycle.Value < 0)
					throw new ArgumentOutOfRangeException("cycle", cycle, "The cycle cannot be a negative number.");

//				if (cycle < _base)
//					throw new ArgumentOutOfRangeException("cycle", cycle, String.Format("The cycle cannot be less than the base cycle ({0}).", _base));

				if (!Contains(tagID))
				{
					Add(new CycleList(tagID, _emaPeriods));
				}

				//CycleList lst = this[tagID];

				TagCycleEMA result = this[tagID][cycle];



				if (result.HasMean)
					return result;

				return result.SetPriorReads(GetMeanReadsPerCycle(tagID, cycle - 1));
			}


			/// <summary>
			/// 
			/// </summary>
			/// <param name="tagID"></param>
			/// <param name="cycle"></param>
			/// <param name="alpha"></param>
			/// <returns></returns>
			public TagCycleEMA GetEMAReadsPerCycle(string tagID, ReadCycle cycle, double alpha)
			{

				if (String.IsNullOrEmpty(tagID))
					throw new ArgumentNullException("tagID", "The tagID cannot be null or empty.");

				if (cycle.Value < 0)
					throw new ArgumentOutOfRangeException("cycle", cycle, "The cycle cannot be a negative number.");

//				if (cycle < _base)
//					throw new ArgumentOutOfRangeException("cycle", cycle, String.Format("The cycle cannot be less than the base cycle ({0}).", _base));


				if (alpha <= 0 || alpha >= 1)
					throw new ArgumentOutOfRangeException("alpha", alpha, "Alpha must be > 0 and < 1");

				if (cycle.Value < EMA.PeriodFromAlpha(alpha))
				{
					throw new ArgumentOutOfRangeException("cycle", cycle, String.Format("Cycle must be < period of EMA ({0}).", EMA.PeriodFromAlpha(alpha)));
				}

				if (!Contains(tagID))
				{
					Add(new CycleList(tagID, _emaPeriods));
				}

//				CycleList lst = this[tagID];


				TagCycleEMA result = this[tagID][cycle];

				if (result.HasEMA && result.EMA.Alpha == alpha)
					return result;

				if (cycle.Value == EMA.PeriodFromAlpha(alpha))
				{
					// Seed the first value of EMA with the average of the prior n (period) cycles
					TagCycleEMA mean = GetMeanReadsPerCycle(tagID, cycle - 1);
					if (!mean.HasMean)
						throw new Exception(String.Format("Cannot seed EMA because {0} does not have a mean.", mean));

					TagCycleEMA temp = new TagCycleEMA(tagID, cycle - 1);
					temp.EMA = new EMA(alpha, mean.Mean, mean.Mean);
				
					return result.SetPriorEma(temp);
				}


				return result.SetPriorEma(GetEMAReadsPerCycle(tagID, cycle - 1, alpha));
			}



			/// <summary>
			/// 
			/// </summary>
			/// <param name="item"></param>
			/// <returns></returns>
			protected override string GetKeyForItem(CycleList item)
			{
				return item.Tag;
			}
		} // private class TagCycleList : KeyedCollection<string, TagCycleEMA>



		private double alpha;
		
		private TagCycleList TheMatrix = null;
		
		/// <summary>
		/// Returns the number of periods (cycles) in the EMA.
		/// </summary>
		public int Periods
		{
			get { return (int)Math.Round((2.0 / alpha) - 1); }
		}

		public TagCycleMatrix(int periodsInEMA)
		{
			alpha = ((double)2) / (double)(periodsInEMA + 1);
			TheMatrix = new TagCycleList(Periods);
		}


		public TagCycleMatrix(float weightingFactor)
		{
			alpha = weightingFactor;
			TheMatrix = new TagCycleList(Periods);
		}

		public void Clear()
		{
			if (TheMatrix != null) TheMatrix.Clear();
			TheMatrix = new TagCycleList(Periods);
		}

		internal ReadCycle GetReadCycle(string tagID, int cycle)
		{
			return TheMatrix.GetReadCycle(tagID, cycle);
		}

		public TagCycleEMA AddTagCycle(string tagID, int cycle)
		{
			ReadCycle readCycle = GetReadCycle(tagID, cycle);
			return TheMatrix.AddTagCycle(tagID, readCycle);
		}

		public TagCycleEMA GetMeanReadsPerCycle(string tagID, int cycle)
		{
			ReadCycle readCycle = GetReadCycle(tagID, cycle);
			return TheMatrix.GetMeanReadsPerCycle(tagID, readCycle);
		}

		public TagCycleEMA GetEMAReadsPerCycle(string tagID, int cycle)
		{
			ReadCycle readCycle = GetReadCycle(tagID, cycle);
			return TheMatrix.GetEMAReadsPerCycle(tagID, readCycle, alpha);
		}

		public void DumpData(System.Action<TagCycleEMA> action)
		{
			foreach (CycleList lst in TheMatrix)
			{
				lst.ForEach(action);				
			}
		}

	}


	public static class FileCompressor
	{
		private const long MAX_BLOCK_SIZE = 1024 * 1024; // 1MB

		private static class BlockHeader
		{
			public const int BLOCK_HEADER_SIZE			= 512;
			public static readonly byte[] Signature		= null;
			public static int BlockNumber				= 0;
			public static int BlockSize					= 0;
			public static string FileName				= null;

			static BlockHeader()
			{
				BlockHeader.Signature = new Guid("5D90E16D-3656-4a2c-A659-94F565C95D15").ToByteArray();
			}

			public static void SetHeaderValues(int block, int dataLength, string filename)
			{
				BlockNumber			= block;
				BlockSize			= dataLength;
				FileName			= filename;
			}

			public static byte[] Buffer
			{
				get
				{
					byte[] header = new byte[BLOCK_HEADER_SIZE];
					header.Initialize();
					int location = 0;

					Signature.CopyTo(header, location);
					location+= Signature.Length;

					BitConverter.GetBytes(BlockNumber).CopyTo(header, location);
					location+= sizeof(int);

					BitConverter.GetBytes(BlockSize).CopyTo(header, location);
					location+= sizeof(int);

					BitConverter.GetBytes(FileName.Length).CopyTo(header, location);
					location+= sizeof(int);

					foreach (char c in FileName.ToCharArray())
						header[location++] = (byte)c;

					return header;
				}
				set
				{
					for (int i = 0; i < Signature.Length; i++)
					{
						if (Signature[i] != value[i])
							throw new ApplicationException("Unabled to access the correct page header");
					}
					int location = Signature.Length;

					BlockNumber = BitConverter.ToInt32(value, location);
					location+= sizeof(int);

					BlockSize = BitConverter.ToInt32(value, location);
					location+= sizeof(int);

					int nameSize = BitConverter.ToInt32(value, location);
					location+= sizeof(int);

					char[] name = new char[nameSize];

					for (int i = 0; i < nameSize; i++)
						name[i] = (char)value[location + i];

					FileName = new string(name);
					
				}
			}
		}

		public static void Test()
		{
			
			List<string> filelist = new List<string>(10);
			for (int i = 0; i < 10; i++) filelist.Add(Path.GetTempFileName());
			Random rnd = new Random();

			foreach (string name in filelist)
			{
				byte[] buffer = new byte[rnd.Next(1025, 10*1024)];
				rnd.NextBytes(buffer);
				//for (int i = 0; i < buffer.Length; i++)
				//	buffer[i] = (byte)'C';

				File.WriteAllBytes(name, buffer);
			} 

			string compressedName = Path.GetTempFileName();
			Compress(compressedName, false, filelist.ToArray());
			Decompress(compressedName, Path.GetTempPath());
		}

		public static void test2()
		{
			string compressedName = Path.GetTempFileName();
			Compress(compressedName, false, @"C:\Documents and Settings\rlmendoz\Local Settings\Temp\New Folder\test1A1");
			Decompress(compressedName, Path.GetTempPath());
		}

		public static void Compress(string outputfile, bool delete, params string[] filenames)
		{

			FileStream outStream = File.Open(outputfile, FileMode.Create, FileAccess.Write, FileShare.None);
			foreach (string filename in filenames)
			{
				using (FileStream inFile = File.Open(filename, FileMode.Open, FileAccess.Read))
				{
					int block = 0;
					while (inFile.Position < inFile.Length)
					{
						int len = (int)Math.Min(MAX_BLOCK_SIZE, inFile.Length - inFile.Position);
						using (MemoryStream rawStream = new MemoryStream(len), 
											compressedStream = new MemoryStream())
						{
							System.IO.Compression.GZipStream zipStream = new System.IO.Compression.GZipStream(compressedStream, System.IO.Compression.CompressionMode.Compress, true);

							//byte[] data = new byte[len];
							for (int i = 0; i < len; i++)
							{
								//data[i] = (byte)inFile.ReadByte();
								rawStream.WriteByte((byte)inFile.ReadByte());
							}

							zipStream.Write(rawStream.ToArray(), 0, len);
							zipStream.Close();
							BlockHeader.SetHeaderValues(block++, (int)compressedStream.Length, filename);
							outStream.Write(BlockHeader.Buffer, 0, BlockHeader.BLOCK_HEADER_SIZE);
							//						outStream.Write(data, 0, data.Length);
							outStream.Write(compressedStream.ToArray(), 0, (int)compressedStream.Length);
							compressedStream.Close();
						}
					}
					inFile.Close();
				}
				if (delete)
					File.Delete(filename);
			}
			outStream.Close();
		}

		public static void Decompress(string filename, string outputDirectory)
		{

			FileStream inStream = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
			byte[] header = new byte[BlockHeader.BLOCK_HEADER_SIZE];
			while (inStream.Position < inStream.Length)
			{
				inStream.Read(header, 0, BlockHeader.BLOCK_HEADER_SIZE);
				BlockHeader.Buffer = header;
				byte[] data = new byte[BlockHeader.BlockSize];
				inStream.Read(data, 0, BlockHeader.BlockSize);
				using (MemoryStream compressedStream = new MemoryStream(data),
									rawStream = new MemoryStream())
				{
					System.IO.Compression.GZipStream zipStream = new System.IO.Compression.GZipStream(compressedStream, System.IO.Compression.CompressionMode.Decompress, true);
					int bytesRead;
					byte[] outData = new byte[1024];
					while ((bytesRead = zipStream.Read(outData, 0, outData.Length)) > 0)
					{
						rawStream.Write(outData, 0, bytesRead);
					}
					zipStream.Close();
	
					if (BlockHeader.BlockNumber == 0)
					{	//First block in file
						File.WriteAllBytes(Path.Combine(outputDirectory, Path.GetFileName(BlockHeader.FileName)), rawStream.ToArray());
					}
					else
					{	// Additional block
						using (FileStream outStream = File.OpenWrite(Path.Combine(outputDirectory, Path.GetFileName(BlockHeader.FileName))))
						{
							outStream.Position = outStream.Length;
							outStream.Write(rawStream.ToArray(), 0, (int)rawStream.Length);
							outStream.Close();
						}
					}
				}
			}
			inStream.Close();
		}
	} // public static class FileCompressor


	public static class HighResolutionTimer
	{
		public static Stopwatch Timer;
		public static long TicksPerMicrosecond;
		public static void NoOp()
		{ }

		static HighResolutionTimer()
		{
			Timer = new Stopwatch();
			Timer.Start();
			TicksPerMicrosecond = Stopwatch.Frequency / (1000L * 1000L);
		}

		public static long Microseconds
		{
			get { return Timer.ElapsedTicks / TicksPerMicrosecond; }
		}

		public static long Milliseconds
		{
			get { return Timer.ElapsedMilliseconds; }
		}

	}
}
