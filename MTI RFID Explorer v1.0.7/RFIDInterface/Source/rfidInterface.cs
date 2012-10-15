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
 * $Id: rfidInterface.cs,v 1.9 2010/06/17 01:03:42 dciampi Exp $
 * 
 * Description: 
 *
 *****************************************************************************
 */

using System;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Text;


namespace RFID.RFIDInterface
{

	public enum TableState
	{
		Ready,
		Building,
		BuildRequired,
		NotAvailable,
	}

	public interface IReader
	{
		bool	IsClosed												{ get;			}
		string	Name													{ get;			}

		TableState TableResult											{ get;			}
		FunctionControl FunctionController								{ get;			}
		PacketArrayList RecentPacketList								{ get;			}
		rfidReader.OperationMode Mode									{ get;			}
        rfid.Structures.Version         HardwareVersion                 { get;			}
        rfid.Structures.FirmwareVersion FirmwareVersion                 { get;          }
        rfid.Structures.LibraryVersion  LibraryVersion                  { get;			}
        rfid.Structures.MacBootLoaderVersion BootLoaderVersion          { get;          }


		DataFile<PropertyBag> PropertyBagData							{ get;			}
		SequentialDataFile<PacketStream> PacketStreamData				{ get;			}
		DataFile<TagInventory> TagInventoryData							{ get;			}
		SequentialDataFile<ReaderRequest> ReaderRequestData				{ get;			}
		SequentialDataFile<ReaderCommand> ReaderCommandData				{ get;			}
		SequentialDataFile<ReaderAntennaCycle> ReaderAntennaCycleData	{ get;			}
		SequentialDataFile<AntennaPacket> AntennaCycleData				{ get;			}
		SequentialDataFile<InventoryCycle> InventoryCycleData			{ get;			}
		SequentialDataFile<InventoryRound> InventoryRoundData			{ get;			}
		SequentialDataFile<TagRead> TagReadData							{ get;			}
		SequentialDataFile<ReadRate> ReadRateData						{ get;			}
		SequentialDataFile<BadPacket> BadPacketData						{ get;			}


		void SetProperty(string name, object value);
		string GetPropertyAsString(string name);
	}


	[System.Runtime.InteropServices.GuidAttribute("BA4E691D-6495-48df-8A09-3FF4F52C5A21")]
	public class PropertyBag : DatabaseRowTemplate
	{
		[DBFieldInfo(0, AllowDuplicate = false, AllowNull = false, PrimaryKey = true)]
		public string propName;
		public string PropName { get { return propName; } }


		[DBFieldInfo(1, AllowDuplicate = true, AllowNull = false)]
		public Object value;
		public Object Value { get { return value; } }

		[DBFieldInfo(2, Timestamp= true, AllowDuplicate = true, AllowNull = false)]
		public DateTime timestamp = DateTime.UtcNow;

	}


	[System.Runtime.InteropServices.GuidAttribute("7B9A74DC-0EEC-4e5f-A669-A34BE5B59874")]
	public class BadPacket : DatabaseRowTemplate
	{
		[DBFieldInfo(0, AllowDuplicate = false, AllowNull = false)]
		public Int32 packetSequence;
		public Int32 PacketSequence { get { return packetSequence; } }

		[DBFieldInfo(1, AllowDuplicate = false, AllowNull = false, PrimaryKey = true)]
		public Int32 badPacketSequence;
		public Int32 BadPacketSequence { get { return badPacketSequence; } }

		[DBFieldInfo(2, "RequestSequence", AllowDuplicate = false, AllowNull = false)]
		public Int32 requestSequence;
		public Int32 RequestSequence { get { return requestSequence; } }

		[DBFieldInfo(3, AllowDuplicate = true, AllowNull = false)]
		public int? readerID;
		public int? ReaderID { get { return readerID; } }

		[DBFieldInfo(4, Timestamp = true, AllowDuplicate = true, AllowNull = false)]
		public DateTime packetTime;
		public DateTime PacketTime { get { return packetTime; } }

		[DBFieldInfo(5, "RawPacketData", AllowDuplicate = true, AllowNull = false)]
		public byte[] rawPacketData;
		public byte[] RawPacketData { get { return rawPacketData; } }

		[DBFieldInfo(6, "PacketData", AllowDuplicate = true, AllowNull = false)]
		public string packetData;
		public string PacketData { get { return packetData; } }

		[DBFieldInfo(7, "ErrorMessage", AllowDuplicate = true, AllowNull = false)]
		public string errorMessage;
		public string ErrorMessage { get { return errorMessage; } }
	}



	[System.Runtime.InteropServices.GuidAttribute("51F7444A-70BF-4628-91CD-493486BF7776")]
	public class ReaderRequest : DatabaseRowTemplate
	{
		[DBFieldInfo(0, AllowDuplicate = true, AllowNull = false)]
		public string reader;
		public string Reader { get { return reader; } }

		[DBFieldInfo(1, AllowDuplicate = false, AllowNull = false, PrimaryKey = true)]
		public Int32 requestSequence;
		public Int32 RequestSequence { get { return requestSequence; } }

		[DBFieldInfo(2, AllowDuplicate = true, AllowNull = false)]
		public string requestName;
		public string RequestName { get { return requestName; } }

		[DBFieldInfo(3, Timestamp=true, AllowDuplicate = true, AllowNull = false)]
		public DateTime startTime;
		public DateTime StartTime { get { return startTime; } }

		[DBFieldInfo(4, AllowDuplicate = true, AllowNull = true)]
		public DateTime? endTime = null;
		public DateTime? EndTime { get { return endTime; } }

		[DBFieldInfo(5, AllowDuplicate = true, AllowNull = false)]
		public Int64 requestStartTime;
		public Int64 RequestStartTime { get { return requestStartTime; } }

		[DBFieldInfo(6, AllowDuplicate = true, AllowNull = true)]
		public Int64? requestEndTime = null;
		public Int64? RequestEndTime { get { return requestEndTime; } }

		[DBFieldInfo(7, AllowDuplicate = true, AllowNull = true)]
		public Int64? elapsedTime = null;
		public Int64? ElapsedTime { get { return elapsedTime; } }

		[DBFieldInfo(8, AllowDuplicate = true, AllowNull = true)]
		public Int32? packetCount = null;
		public Int32? PacketCount { get { return packetCount; } }

		[DBFieldInfo(9, AllowDuplicate = true, AllowNull = true)]
		public Int32? tagCount = null;
		public Int32? TagCount { get { return tagCount; } }

		[DBFieldInfo(10, AllowDuplicate = true, AllowNull = true)]
		public Double? singulationRate = null;
		public Double? SingulationRate { get { return singulationRate; } }

		[DBFieldInfo(11, AllowDuplicate = true, AllowNull = true)]
		public Double? uniqueSingulationRate = null;
		public Double? UniqueSingulationRate { get { return uniqueSingulationRate; } }

		[DBFieldInfo(12, AllowDuplicate = true, AllowNull = false)]
		public Int32 startingPacketCount;

		[DBFieldInfo(13, AllowDuplicate = true, AllowNull = false)]
		public Int32 startingTagCount;
	}


	[System.Runtime.InteropServices.GuidAttribute("63515753-1F1B-4f95-A883-1268797E6AD3")]
	public class PacketStream : DatabaseRowTemplate
	{
		[DBFieldInfo(0, AllowDuplicate = false, AllowNull = false, PrimaryKey = true)]
		public Int32 packetSequence;
		public Int32 PacketSequence { get { return packetSequence; } }

		[DBFieldInfo(1, AllowDuplicate = true, AllowNull = false)]
		public int? readerID;
		public int? ReaderID { get { return readerID; } }

		[DBFieldInfo(2, AllowDuplicate = true, AllowNull = false)]
		public string reader;
		public string Reader { get { return reader; } }

		[DBFieldInfo(3, Timestamp = true, AllowDuplicate = true, AllowNull = false)]
		public DateTime packetTime;
		public DateTime PacketTime { get { return packetTime; } }

		[DBFieldInfo(4, AllowDuplicate = true, AllowNull = false)]
		public string packetType;
		public string PacketType { get { return packetType; } }

		[DBFieldInfo(5, AllowDuplicate = true, AllowNull = false)]
		public Int64 elapsedTimeMs;
		public Int64 ElapsedTimeMs { get { return elapsedTimeMs; } }

		[DBFieldInfo(6, AllowDuplicate = true, AllowNull = false)]
		public Int32 requestNumber;
		public Int32 RequestNumber { get { return requestNumber; } }

		[DBFieldInfo(7, AllowDuplicate = true, AllowNull = false)]
		public Int32? readerIndex = null;
		public Int32? ReaderIndex { get { return readerIndex; } }

		[DBFieldInfo(8, "RawPacketData", AllowDuplicate = true, AllowNull = false)]
		public byte[] rawPacketData;
		public byte[] RawPacketData { get { return rawPacketData; } }

		[DBFieldInfo(9, "PacketData", AllowDuplicate = true, AllowNull = false)]
		public string packetData;
		public string PacketData { get { return packetData; } }

		[DBFieldInfo(10, "IsPseudoPacket", AllowDuplicate = true, AllowNull = false)]
		public bool isPseudoPacket;
		public bool IsPseudoPacket { get { return isPseudoPacket; } }


	}




	[System.Runtime.InteropServices.GuidAttribute("F427185A-4916-47f6-90BD-6C1638E197E9")]
	public class ReaderCommand : DatabaseRowTemplate
	{
		[DBFieldInfo(0, AllowDuplicate = false, AllowNull = false)]
		public Int32 packetSequence;
		public Int32 PacketSequence { get { return packetSequence; } }


		[DBFieldInfo(1, AllowDuplicate = true, AllowNull = false)]
		public int? readerID;
		public int? ReaderID { get { return readerID; } }


		[DBFieldInfo(2, Timestamp = true, AllowDuplicate = true, AllowNull = false)]
		public DateTime packetTime;
		public DateTime PacketTime { get { return packetTime; } }


		[DBFieldInfo(3, AllowDuplicate = false, AllowNull = false, PrimaryKey = true)]
		public Int32 commandSequence;
		public Int32 CommandSequence { get { return commandSequence; } }


		[DBFieldInfo(4, AllowDuplicate = true, AllowNull = false)]
		public Int64 commandStartTime;
		public Int64 CommandStartTime { get { return commandStartTime; } }


		[DBFieldInfo(5, AllowDuplicate = true, AllowNull = true)]
		public Int64? commandEndTime = null;
		public Int64? CommandEndTime { get { return commandEndTime; } }


		[DBFieldInfo(6, AllowDuplicate = true, AllowNull = true)]
		public Int32? tagCount = null;
		public Int32? TagCount { get { return tagCount; } }


		[DBFieldInfo(7, AllowDuplicate = true, AllowNull = true)]
		public Int32? uniqueTagCount = null;
		public Int32? UniqueTagCount { get { return uniqueTagCount; } }


		[DBFieldInfo(8, AllowDuplicate = true, AllowNull = true)]
		public Int64? elapsedTime = null;
		public Int64? ElapsedTime { get { return elapsedTime; } }


		[DBFieldInfo(9, AllowDuplicate = false, AllowNull = false)]
		public UInt32 cmd_begTime;
		public UInt32 Cmd_begTime { get { return cmd_begTime; } }


		[DBFieldInfo(10, AllowDuplicate = true, AllowNull = true)]
		public UInt32? cmd_endTime = null;
		public UInt32? Cmd_endTime { get { return cmd_endTime; } }


		[DBFieldInfo(11, AllowDuplicate = true, AllowNull = true)]
		public UInt32? cmd_delta = null;
		public UInt32? Cmd_delta { get { return cmd_delta; } }


		[DBFieldInfo(12, AllowDuplicate = true, AllowNull = false)]
		public string commandType = null;
		public string CommandType { get { return commandType; } }


		[DBFieldInfo(13, AllowDuplicate = true, AllowNull = false)]
		public string commandResult = null;
		public string CommandResult { get { return commandResult; } }


		[DBFieldInfo(14, AllowDuplicate = true, AllowNull = true)]
		public string continuousModeFlag = null;
		public string ContinuousModeFlag { get { return continuousModeFlag; } }


		[DBFieldInfo(15, AllowDuplicate = true, AllowNull = true)]
		public Double? singulationRate = null;
		public Double? SingulationRate { get { return singulationRate; } }


		[DBFieldInfo(16, AllowDuplicate = true, AllowNull = false)]
		public Int32 startingTagCount;


		public Double? UniqueSingulationRate 
		{ 
			get 
			{
				if (UniqueTagCount.HasValue &&
					ElapsedTime.HasValue	&&
					UniqueTagCount	> 0		&& 
					ElapsedTime		> 0		)
					return UniqueTagCount / (ElapsedTime / 1000f);
				else
					return null;
			}
		}


	}



	[System.Runtime.InteropServices.GuidAttribute("20A2D8F5-7BCE-4869-95A8-FA586B8D6AC3")]
	public class ReaderAntennaCycle : DatabaseRowTemplate
	{
		[DBFieldInfo(0, AllowDuplicate = false, AllowNull = false)]
		public Int32 packetSequence;
		public Int32 PacketSequence { get { return packetSequence; } }


		[DBFieldInfo(1, AllowDuplicate = true, AllowNull = false)]
		public int? readerID;
		public int? ReaderID { get { return readerID; } }


		[DBFieldInfo(2, Timestamp = true, AllowDuplicate = true, AllowNull = false)]
		public DateTime packetTime;
		public DateTime PacketTime { get { return packetTime; } }


		[DBFieldInfo(3, AllowDuplicate = true, AllowNull = false)]
		public Int32 commandSequence;
		public Int32 CommandSequence { get { return commandSequence; } }


		[DBFieldInfo(4, AllowDuplicate = false, AllowNull = false, PrimaryKey = true)]
		public Int32 cycleSequence;
		public Int32 CycleSequence { get { return cycleSequence; } }


		[DBFieldInfo(5, AllowDuplicate = true, AllowNull = true, DBType = typeof(Int64))]
		public Int64? cycleStartTime = null;
		public Int64? CycleStartTime { get { return cycleStartTime; } }


		[DBFieldInfo(6, AllowDuplicate = true, AllowNull = true, DBType = typeof(Int64))]
		public Int64? cycleEndTime = null;
		public Int64? CycleEndTime { get { return cycleEndTime; } }


		[DBFieldInfo(7, AllowDuplicate = true, AllowNull = true)]
		public Int32? uniqueTagCount = null;
		public Int32? UniqueTagCount { get { return uniqueTagCount; } }


		[DBFieldInfo(8, AllowDuplicate = true, AllowNull = true, DBType = typeof(UInt32))]
		public UInt32? cyc_begTime = null;
		public UInt32? Cyc_begTime { get { return cyc_begTime; } }


		[DBFieldInfo(9, AllowDuplicate = true, AllowNull = true, DBType = typeof(UInt32))]
		public UInt32? cyc_endTime = null;
		public UInt32? Cyc_endTime { get { return cyc_endTime; } }


		[DBFieldInfo(10, AllowDuplicate = true, AllowNull = false)]
		public Int32 startingTagCount = -1;
		public Int32? StartingTagCount { get { return startingTagCount == -1 ? null : (Int32?)startingTagCount; } }


		[DBFieldInfo(11, AllowDuplicate = true, AllowNull = false)]
		public Int32 endingTagCount = -1;
		public Int32? EndingTagCount { get { return endingTagCount == -1 ? null : (Int32?)endingTagCount; } }


		[DBFieldInfo(12, AllowDuplicate = true, AllowNull = false)]
		public Int32 startingInventoryCycleCount = -1;
		public Int32? StartingInventoryCycleCount { get { return startingInventoryCycleCount == -1 ? null : (Int32?)startingInventoryCycleCount; } }


		[DBFieldInfo(13, AllowDuplicate = true, AllowNull = false)]
		public Int32 endingInventoryCycleCount = -1;
		public Int32? EndingInventoryCycleCount { get { return endingInventoryCycleCount == -1 ? null : (Int32?)endingInventoryCycleCount; } }

		[DBFieldInfo(14, AllowDuplicate = true, AllowNull = false)]
		public Int32 startingRoundCount = -1;
		public Int32? StartingRoundCount { get { return startingRoundCount == -1 ? null : (Int32?)startingRoundCount; } }


		[DBFieldInfo(15, AllowDuplicate = true, AllowNull = false)]
		public Int32 endingRoundCount = -1;
		public Int32? EndingRoundCount { get { return endingRoundCount == -1 ? null : (Int32?)endingRoundCount; } }


		[DBFieldInfo(16, AllowDuplicate = true, AllowNull = false)]
		public Int32 startingAntennaCount = -1;
		public Int32? StartingAntennaCount { get { return startingAntennaCount == -1 ? null : (Int32?)startingAntennaCount; } }
		

		[DBFieldInfo(17, AllowDuplicate = true, AllowNull = false)]
		public Int32 endingAntennaCount = -1;
		public Int32? EndingAntennaCount { get { return endingAntennaCount == -1 ? null : (Int32?)endingAntennaCount; } }


		public UInt32? Cyc_delta
		{
			get
			{
				if (cyc_endTime.HasValue && cyc_begTime.HasValue)
					return cyc_endTime - cyc_begTime;
				else
					return null;
			}
		}

		public Int32? TagCount 
		{ 
			get 
			{
				if (StartingTagCount.HasValue && EndingTagCount.HasValue)
					return EndingTagCount - StartingTagCount;
				else
					return null;
			} 
		}

		public Int32? InventoryCycleCount
		{
			get
			{
				if (StartingInventoryCycleCount.HasValue && EndingInventoryCycleCount.HasValue)
					return EndingInventoryCycleCount - StartingInventoryCycleCount;
				else
					return null;
			}
		}

		public Int32? InventoryRoundCount
		{
			get
			{
				if (StartingRoundCount.HasValue && EndingRoundCount.HasValue)
					return EndingRoundCount - StartingRoundCount;
				else
					return null;
			}
		}

		public Int32? AntennaCount
		{
			get
			{
				if (StartingAntennaCount.HasValue && EndingAntennaCount.HasValue)
					return EndingAntennaCount - StartingAntennaCount;
				else
					return null;
			}
		}

		public Int64? ElapsedTime
		{
			get 
			{
				if (CycleStartTime	!= null && 
					CycleEndTime	!= null)
					return CycleEndTime - CycleStartTime;
				else
					return null;
			}
		}
		
		public Double? SingulationRate
		{
			get 
			{
				if (TagCount.HasValue		&&
					TagCount > 0			&&
					ElapsedTime.HasValue	&&
					ElapsedTime > 0)
					return TagCount / (ElapsedTime / 1000f);
				else
					return null;
			}
		}

		public Double? UniqueSingulationRate 
		{ 
			get 
			{
				if (UniqueTagCount.HasValue &&
					ElapsedTime.HasValue	&&
					UniqueTagCount	> 0		&& 
					ElapsedTime		> 0		)
					return UniqueTagCount / (ElapsedTime / 1000f);
				else
					return null;
			}
		}

	}




	[System.Runtime.InteropServices.GuidAttribute("3629814B-0D1F-42bf-BAED-BB6836179AA9")]
	public class AntennaPacket : DatabaseRowTemplate
	{
		[DBFieldInfo(0, AllowDuplicate = true, AllowNull = false)]
		public int? readerID;
		public int? ReaderID { get { return readerID; } }

		[DBFieldInfo(1, Timestamp = true, AllowDuplicate = true, AllowNull = false)]
		public DateTime packetTime;
		public DateTime PacketTime { get { return packetTime; } }

		[DBFieldInfo(2, AllowDuplicate = true, AllowNull = false)]
		public Int32 commandSequence;
		public Int32 CommandSequence { get { return commandSequence; } }

		[DBFieldInfo(3, AllowDuplicate = true, AllowNull = false)]
		public Int32 cycleSequence;
		public Int32 CycleSequence { get { return cycleSequence; } }

		[DBFieldInfo(4, AllowDuplicate = false, AllowNull = false, PrimaryKey = true)]
		public Int32 antennaSequence;
		public Int32 AntennaSequence { get { return antennaSequence; } }

		[DBFieldInfo(5, AllowDuplicate = true, AllowNull = true)]
		public UInt32 antennaNumber;
		public UInt32 AntennaNumber { get { return antennaNumber; } }

		[DBFieldInfo(6, AllowDuplicate = true, AllowNull = true, DBType = typeof(Int64))]
		public Int64? antennaStartTime = null;
		public Int64? AntennaStartTime { get { return antennaStartTime; } }

		[DBFieldInfo(7, AllowDuplicate = true, AllowNull = true, DBType = typeof(Int64))]
		public Int64? antennaEndTime = null;
		public Int64? AntennaEndTime { get { return antennaEndTime; } }

		[DBFieldInfo(8, AllowDuplicate = true, AllowNull = false)]
		public Int32? tagCount = null;
		public Int32? TagCount { get { return tagCount; } }

		[DBFieldInfo(9, AllowDuplicate = true, AllowNull = true)]
		public Int32? uniqueTagCount = null;
		public Int32? UniqueTagCount { get { return uniqueTagCount; } }

		[DBFieldInfo(10, AllowDuplicate = true, AllowNull = true, DBType = typeof(Int64))]
		public Int64? elapsedTime = null;
		public Int64? ElapsedTime { get { return elapsedTime; } }

		[DBFieldInfo(11, AllowDuplicate = true, AllowNull = true, DBType = typeof(UInt32))]
		public UInt32? ant_begTime = null;
		public UInt32? Ant_begTime { get { return ant_begTime; } }

		[DBFieldInfo(12, AllowDuplicate = true, AllowNull = true, DBType = typeof(UInt32))]
		public UInt32? ant_endTime = null;
		public UInt32? Ant_endTime { get { return ant_endTime; } }

		[DBFieldInfo(13, AllowDuplicate = true, AllowNull = true, DBType = typeof(UInt32))]
		public UInt32? ant_delta = null;
		public UInt32? Ant_delta { get { return ant_delta; } }

		[DBFieldInfo(14, AllowDuplicate = true, AllowNull = true, DBType = typeof(UInt32))]
		public UInt32? senseResistorValue = null;
		public UInt32? SenseResistorValue { get { return senseResistorValue; } }

		[DBFieldInfo(15, AllowDuplicate = true, AllowNull = true, DBType = typeof(Double))]
		public Double? singulationRate = null;
		public Double? SingulationRate { get { return singulationRate; } }

		[DBFieldInfo(16, AllowDuplicate = true, AllowNull = true, DBType = typeof(Double))]
		public Double? uniqueSingulationRate = null;
		public Double? UniqueSingulationRate { get { return uniqueSingulationRate; } }

		[DBFieldInfo(17, AllowDuplicate = true, AllowNull = false)]
		public Int32 startingTagCount;

	}

	[System.Runtime.InteropServices.GuidAttribute("DB1EA203-DC78-45fa-B098-DC2BBDD0F444")]
	public class InventoryCycle : DatabaseRowTemplate
	{
		[DBFieldInfo(0, AllowDuplicate = false, AllowNull = false)]
		public Int32 packetSequence;
		public Int32 PacketSequence { get { return packetSequence; } }

		[DBFieldInfo(1, AllowDuplicate = true, AllowNull = false)]
		public int? readerID;
		public int? ReaderID { get { return readerID; } }

		[DBFieldInfo(2, Timestamp = true, AllowDuplicate = true, AllowNull = false)]
		public DateTime packetTime;
		public DateTime PacketTime { get { return packetTime; } }

		[DBFieldInfo(3, AllowDuplicate = true, AllowNull = false)]
		public Int32 commandSequence;
		public Int32 CommandSequence { get { return commandSequence; } }

		[DBFieldInfo(4, AllowDuplicate = false, AllowNull = false, PrimaryKey = true)]
		public Int32 cycleSequence;
		public Int32 CycleSequence { get { return cycleSequence; } }

		[DBFieldInfo(5, AllowDuplicate = true, AllowNull = true, DBType = typeof(Int64))]
		public Int64? cycleStartTime = null;
		public Int64? CycleStartTime { get { return cycleStartTime; } }

		[DBFieldInfo(6, AllowDuplicate = true, AllowNull = true, DBType = typeof(Int64))]
		public Int64? cycleEndTime = null;
		public Int64? CycleEndTime { get { return cycleEndTime; } }

		[DBFieldInfo(7, AllowDuplicate = true, AllowNull = true)]
		public Int32? uniqueTagCount = null;
		public Int32? UniqueTagCount { get { return uniqueTagCount; } }

		[DBFieldInfo(8, AllowDuplicate = true, AllowNull = true, DBType = typeof(UInt32))]
		public UInt32? inv_cyc_begTime = null;
		public UInt32? Inv_cyc_begTime { get { return inv_cyc_begTime; } }

		[DBFieldInfo(9, AllowDuplicate = true, AllowNull = true, DBType = typeof(UInt32))]
		public UInt32? inv_cyc_endTime = null;
		public UInt32? Inv_cyc_endTime { get { return inv_cyc_endTime; } }


		[DBFieldInfo(10, AllowDuplicate = true, AllowNull = false)]
		public Int32 startingTagCount = -1;
		public Int32? StartingTagCount { get { return startingTagCount == -1 ? null : (Int32?)startingTagCount; } }


		[DBFieldInfo(11, AllowDuplicate = true, AllowNull = false)]
		public Int32 endingTagCount = -1;
		public Int32? EndingTagCount { get { return endingTagCount == -1 ? null : (Int32?)endingTagCount; } }


		[DBFieldInfo(12, AllowDuplicate = true, AllowNull = false)]
		public Int32 startingRoundCount = -1;
		public Int32? StartingRoundCount { get { return startingRoundCount == -1 ? null : (Int32?)startingRoundCount; } }


		[DBFieldInfo(13, AllowDuplicate = true, AllowNull = false)]
		public Int32 endingRoundCount = -1;
		public Int32? EndingRoundCount { get { return endingRoundCount == -1 ? null : (Int32?)endingRoundCount; } }


		[DBFieldInfo(14, AllowDuplicate = true, AllowNull = true, DBType = typeof(UInt32))]
		public UInt32? antennaNumber = null;
		public UInt32? AntennaNumber { get { return antennaNumber; } }


		[DBFieldInfo(15, AllowDuplicate = true, AllowNull = true, DBType = typeof(UInt32))]
		public UInt32? querys = null;					// number of query's issued
		public UInt32? Querys { get { return querys; } }


		[DBFieldInfo(16, AllowDuplicate = true, AllowNull = true, DBType = typeof(UInt32))]
		public UInt32? rn16rcv = null;					// number of RN16's received
		public UInt32? Rn16rcv { get { return rn16rcv; } }


		[DBFieldInfo(17, AllowDuplicate = true, AllowNull = true, DBType = typeof(UInt32))]
		public UInt32? rn16Timeouts = null;				// number of empty slots (rn16 timeouts)
		public UInt32? Rn16Timeouts { get { return rn16Timeouts; } }


		[DBFieldInfo(18, AllowDuplicate = true, AllowNull = true, DBType = typeof(UInt32))]
		public UInt32? epcTimeouts = null;					// number of bad RN16's (No response to rn16 Ack)
		public UInt32? EpcTimeouts { get { return epcTimeouts; } }

		[DBFieldInfo(19, AllowDuplicate = true, AllowNull = true, DBType = typeof(UInt32))]
		public UInt32? goodReads = null;				// number of good EPC reads 
		public UInt32? GoodReads { get { return goodReads; } }


		[DBFieldInfo(20, AllowDuplicate = true, AllowNull = true, DBType = typeof(UInt32))]
		public UInt32? crcFailures = null;
		public UInt32? CrcFailures { get { return crcFailures; } }



		public UInt32? inv_cyc_delta
		{
			get
			{
				if (inv_cyc_endTime.HasValue && inv_cyc_begTime.HasValue)
					return inv_cyc_endTime - inv_cyc_begTime;
				else
					return null;
			}
		}

		public Int32? TagCount 
		{ 
			get 
			{
				if (StartingTagCount.HasValue && EndingTagCount.HasValue)
					return EndingTagCount - StartingTagCount;
				else
					return null;
			} 
		}


		public Int32? InventoryRoundCount
		{
			get
			{
				if (StartingRoundCount.HasValue && EndingRoundCount.HasValue)
					return EndingRoundCount - StartingRoundCount;
				else
					return null;
			}
		}


		public Int64? ElapsedTime
		{
			get 
			{
				if (CycleStartTime	!= null && 
					CycleEndTime	!= null)
					return CycleEndTime - CycleStartTime;
				else
					return null;
			}
		}
		
		public Double? SingulationRate
		{
			get 
			{
				if (TagCount.HasValue		&&
					TagCount > 0			&&
					ElapsedTime.HasValue	&&
					ElapsedTime > 0)
					return TagCount / (ElapsedTime / 1000f);
				else
					return null;
			}
		}

		public Double? UniqueSingulationRate 
		{ 
			get 
			{
				if (UniqueTagCount.HasValue &&
					ElapsedTime.HasValue	&&
					UniqueTagCount	> 0		&& 
					ElapsedTime		> 0		)
					return UniqueTagCount / (ElapsedTime / 1000f);
				else
					return null;
			}
		}

	}





	[System.Runtime.InteropServices.GuidAttribute("AA63F8B0-2C20-4502-B5A2-75EB04E96B1E")]
	public class InventoryRound : DatabaseRowTemplate
	{
		[DBFieldInfo(0, AllowDuplicate = false, AllowNull = false)]
		public Int32 packetSequence;
		public Int32 PacketSequence { get { return packetSequence; } }

		[DBFieldInfo(1, AllowDuplicate = true, AllowNull = false)]
		public int? readerID;
		public int? ReaderID { get { return readerID; } }

		[DBFieldInfo(2, Timestamp = true, AllowDuplicate = true, AllowNull = false)]
		public DateTime packetTime;
		public DateTime PacketTime { get { return packetTime; } }

		[DBFieldInfo(3, AllowDuplicate = true, AllowNull = false)]
		public Int32 commandSequence;
		public Int32 CommandSequence { get { return commandSequence; } }

		[DBFieldInfo(4, AllowDuplicate = true, AllowNull = false)]
		public Int32 cycleSequence;
		public Int32 CycleSequence { get { return cycleSequence; } }

		[DBFieldInfo(5, AllowDuplicate = true, AllowNull = false)]
		public Int32 antennaSequence;
		public Int32 AntennaSequence { get { return antennaSequence; } }

		[DBFieldInfo(6, AllowDuplicate = false, AllowNull = false, PrimaryKey = true)]
		public Int32 roundSequence;
		public Int32 RoundSequence { get { return roundSequence; } }

		[DBFieldInfo(7, AllowDuplicate = true, AllowNull = true, DBType = typeof(Int64))]
		public Int64 roundStartTime = -1;
		public Int64? RoundStartTime { get { return roundStartTime == -1 ? null : (Int64?)roundStartTime; } }

		[DBFieldInfo(8, AllowDuplicate = true, AllowNull = true, DBType = typeof(Int64))]
		public Int64 roundEndTime = -1;
		public Int64? RoundEndTime { get { return roundEndTime == -1 ? null : (Int64?)roundEndTime; } }

		[DBFieldInfo(9, AllowDuplicate = true, AllowNull = false)]
		public Int32 startingTagCount = -1;
		public Int32? StartingTagCount { get { return startingTagCount == -1 ? null : (Int32?)startingTagCount; } }

		[DBFieldInfo(10, AllowDuplicate = true, AllowNull = false)]
		public Int32 endingTagCount = -1;
		public Int32? EndingTagCount { get { return endingTagCount == -1 ? null : (Int32?)endingTagCount; } }

		[DBFieldInfo(11, AllowDuplicate = true, AllowNull = true)]
		public Int32 uniqueTagCount = -1;
		public Int32? UniqueTagCount { get { return uniqueTagCount == -1 ? null : (Int32?)uniqueTagCount; } }


		[DBFieldInfo(12, AllowDuplicate = true, AllowNull = true, DBType = typeof(UInt32))]
		public UInt32 inv_rnd_begTime = 0;
		public UInt32? Inv_rnd_begTime { get { return inv_rnd_begTime == 0 ? null : (UInt32?)inv_rnd_begTime; } }


		[DBFieldInfo(13, AllowDuplicate = true, AllowNull = true, DBType = typeof(UInt32))]
		public UInt32 inv_rnd_endTime = 0;
		public UInt32? Inv_rnd_endTime { get { return inv_rnd_endTime == 0 ? null : (UInt32?)inv_rnd_endTime; } }


		[DBFieldInfo(14, AllowDuplicate = true, AllowNull = true, DBType = typeof(UInt32))]
		public UInt32? singulationParameters = null;
		public UInt32? SingulationParameters { get { return singulationParameters; } }
		public string SingulationParametersString { get { return singulationParameters.HasValue ? String.Format("0x{0:x}", singulationParameters) : null; } }


		[DBFieldInfo(15, AllowDuplicate = true, AllowNull = true, DBType = typeof(UInt32))]
		public UInt32? querys = null;					// number of query's issued
		public UInt32? Querys { get { return querys; } }


		[DBFieldInfo(16, AllowDuplicate = true, AllowNull = true, DBType = typeof(UInt32))]
		public UInt32? rn16rcv = null;					// number of RN16's received
		public UInt32? Rn16rcv { get { return rn16rcv; } }


		[DBFieldInfo(17, AllowDuplicate = true, AllowNull = true, DBType = typeof(UInt32))]
		public UInt32? rn16Timeouts = null;				// number of empty slots (rn16 timeouts)
		public UInt32? Rn16Timeouts { get { return rn16Timeouts; } }

		[DBFieldInfo(18, AllowDuplicate = true, AllowNull = true, DBType = typeof(UInt32))]
		public UInt32? epcTimeouts = null;					// number of bad RN16's (No response to rn16 Ack)
		public UInt32? EpcTimeouts { get { return epcTimeouts; } }

		[DBFieldInfo(19, AllowDuplicate = true, AllowNull = true, DBType = typeof(UInt32))]
		public UInt32? goodReads = null;				// number of good EPC reads 
		public UInt32? GoodReads { get { return goodReads; } }

		[DBFieldInfo(20, AllowDuplicate = true, AllowNull = true, DBType = typeof(UInt32))]
		public UInt32? crcFailures = null;
		public UInt32? CrcFailures { get { return crcFailures; } }

		[DBFieldInfo(21, AllowDuplicate = true, AllowNull = true, DBType = typeof(UInt32))]
		public UInt32? antennaNumber = null;
		public UInt32? AntennaNumber { get { return antennaNumber; } }

/*
		public string Session 
		{
			get 
			{
				if (!SingulationParameters.HasValue) return null;
				BitVector32 singulationParams = new BitVector32(SingulationParameters.Value);
				return PacketData.PacketBase.InventorySessionName(singulationParams[PacketData.PacketBase.Session]);
			} 
		}
*/
		public string InventoriedFlag 
		{			
			get 
			{	
				if (!SingulationParameters.HasValue) return null;
				BitVector32 singulationParams = new BitVector32( ( int ) SingulationParameters.Value);
				return PacketData.PacketBase.InventoriedFlagName(singulationParams[PacketData.PacketBase.InventoriedFlag]); 
			}
		}

		public string CurrentQValue
		{			
			get 
			{	
				if (!SingulationParameters.HasValue) return null;
				BitVector32 singulationParams = new BitVector32( ( int ) SingulationParameters.Value);
				return singulationParams[PacketData.PacketBase.CurrentQValue].ToString();
			}
		}

		public string CurrentSlot
		{
			get
			{
				if (!SingulationParameters.HasValue) return null;
				BitVector32 singulationParams = new BitVector32( ( int ) SingulationParameters.Value);
				return PacketData.PacketBase.GetCurrentSlot(singulationParams).ToString();
			}
		}
		public string CurrentRetry
		{
			get
			{
				if (!SingulationParameters.HasValue) return null;
				BitVector32 singulationParams = new BitVector32( ( int ) SingulationParameters.Value);
				return singulationParams[PacketData.PacketBase.RetryCount].ToString();
			}
		}

		
		public UInt32? Rnd_delta
		{
			get
			{
				if (Inv_rnd_endTime.HasValue && Inv_rnd_begTime.HasValue)
					return inv_rnd_endTime - inv_rnd_begTime;
				else
					return null;
			}
		}


		public Int32? TagCount
		{
			get
			{
				if (StartingTagCount.HasValue && EndingTagCount.HasValue)
					return EndingTagCount - StartingTagCount;
				else
					return null;
			}
		}


		public Int64? ElapsedTime
		{
			get
			{
				if (RoundStartTime != null &&
					RoundEndTime != null)
					return RoundEndTime - RoundStartTime;
				else
					return null;
			}
		}

		public Double? SingulationRate
		{
			get
			{
				if (TagCount.HasValue &&
					TagCount > 0 &&
					ElapsedTime.HasValue &&
					ElapsedTime > 0)
					return TagCount / (ElapsedTime / 1000f);
				else
					return null;
			}
		}

		public Double? UniqueSingulationRate
		{
			get
			{
				if (UniqueTagCount.HasValue &&
					ElapsedTime.HasValue &&
					UniqueTagCount > 0 &&
					ElapsedTime > 0)
					return UniqueTagCount / (ElapsedTime / 1000f);
				else
					return null;
			}
		}

		

	}


	[System.Runtime.InteropServices.GuidAttribute("6B5D9EDD-79AD-4f21-8B8A-8DAC6EE4019F")]
	public class TagRead : DatabaseRowTemplate
	{
		[DBFieldInfo(0, AllowDuplicate = false, AllowNull = false)]
		public Int32 packetSequence;
		public Int32 PacketSequence { get { return packetSequence; } }

		[DBFieldInfo(1, AllowDuplicate = true, AllowNull = true)]
		public int? readerID;
		public int? ReaderID { get { return readerID; } }


		[DBFieldInfo(2, Timestamp = true, AllowDuplicate = true, AllowNull = false)]
		public DateTime packetTime;
		public DateTime PacketTime { get { return packetTime; } }


		[DBFieldInfo(3, AllowDuplicate = true, AllowNull = false)]
		public Int64 readTime;
		public Int64 ReadTime { get { return readTime; } }


		[DBFieldInfo(4, AllowDuplicate = true, AllowNull = false)]
		public Int32 commandSequence;
		public Int32 CommandSequence { get { return commandSequence; } }


		[DBFieldInfo(5, AllowDuplicate = true, AllowNull = false)]
		public Int32 cycleSequence;
		public Int32 CycleSequence { get { return cycleSequence; } }


		[DBFieldInfo(6, AllowDuplicate = true, AllowNull = false)]
		public Int32 antennaSequence;
		public Int32 AntennaSequence { get { return antennaSequence; } }


		[DBFieldInfo(7, AllowDuplicate = true, AllowNull = false)]
		public Int32 roundSequence;
		public Int32 RoundSequence { get { return roundSequence; } }


		[DBFieldInfo(8, AllowDuplicate = false, AllowNull = false, PrimaryKey = true)]
		public Int32 readSequence;
		public Int32 ReadSequence { get { return readSequence; } }


		[DBFieldInfo(9, AllowDuplicate = true, AllowNull = false)]
		public string crcResult;						// valid or invalid
		public string CrcResult { get { return crcResult; } }
		//Bit 0 of preable flags field - CRC valid flag:
		//1 ?CRC was invalid.  No INVENTORY data will be included in packet.
		//0 ?CRC was valid

		[DBFieldInfo(10, AllowDuplicate = true, AllowNull = false)]
		public string tagId;
		public string TagId { get { return tagId; } }

		[DBFieldInfo(11, AllowDuplicate = true, AllowNull = false)]
		public string resultType;
		public string ResultType { get { return resultType; } }

		[DBFieldInfo(12, AllowDuplicate = true, AllowNull = false)]
		public string accessType;
		public string AccessType { get { return accessType; } }

		[DBFieldInfo(13, AllowDuplicate = true, AllowNull = false)]
		public string parameter;
		public string Parameter { get { return parameter; } }

		[DBFieldInfo(14, AllowDuplicate = true, AllowNull = false)]
		public string tagData;
		public string TagData { get { return tagData; } }

		[DBFieldInfo(15, AllowDuplicate = true, AllowNull = false)]
		public UInt32 inventoryTime;
		public UInt32 InventoryTime { get { return inventoryTime; } }

        [DBFieldInfo(16, AllowDuplicate = true, AllowNull = true, DBType = typeof(UInt32))]
        public UInt32? nb_rssi = null;
        public UInt32? NB_RSSI { get { return nb_rssi; } }

        [DBFieldInfo(17, AllowDuplicate = true, AllowNull = true, DBType = typeof(UInt32))]
        public UInt32? wb_rssi = null;
        public UInt32? WB_RSSI { get { return wb_rssi; } }

        [DBFieldInfo(18, AllowDuplicate = true, AllowNull = true, DBType = typeof(UInt32))]
        public UInt32? ana_ctrl1 = null;				// Receive Signal Strength Indicator - Backscattered tag signal amplitude ?
        public UInt32? ANA_CTRL { get { return ana_ctrl1; } }

        [DBFieldInfo(19, AllowDuplicate = true, AllowNull = true, DBType = typeof(UInt32))]
        public UInt32? rssi = null;				// Receive Signal Strength Indicator - Backscattered tag signal amplitude ?
        public UInt32? RSSI { get { return rssi; } }

        [DBFieldInfo(20, AllowDuplicate = true, AllowNull = true, DBType = typeof(UInt32))]
		public UInt32? protocolParameters = null;
		public UInt32? ProtocolParameters { get { return protocolParameters; } }


		[DBFieldInfo(21, AllowDuplicate = true, AllowNull = true, DBType = typeof(UInt32))]
		public UInt32? antennaNumber = null;
		public UInt32? AntennaNumber { get { return antennaNumber; } }
	}



	/// <summary>
	/// Used to support the "Standard View" which is displayed live. All records are held in memory.
	/// </summary>
	[System.Runtime.InteropServices.GuidAttribute("EA528487-CA1D-49b3-9A56-8242ACB2FCD6")]
	public class TagInventory : DatabaseRowTemplate
	{
		[DBFieldInfo(0, ColumnName = "TagIdData", AllowDuplicate = false, AllowNull = false, PrimaryKey = true)]
		public string tagIdData;
		public string TagIdData { get { return tagIdData; } }

		/// <summary>
		/// Number of times the tag has been read in session.
		/// </summary>
		[DBFieldInfo(1, AllowDuplicate = true, AllowNull = false)]
		public UInt32 readCount;
		public UInt32 ReadCount { get { return readCount; } }

		/// <summary>
		/// Time tag was first read in session in Device timestamp ms 
		/// </summary>
		[DBFieldInfo(2, AllowDuplicate = true, AllowNull = false)]
		public UInt32 firstReadTime;
		public UInt32 FirstReadTime { get { return firstReadTime; } }

		/// <summary>
		/// Time of most recent read in Device timestamp ms
		/// </summary>
		[DBFieldInfo(3, AllowDuplicate = true, AllowNull = false)]
		public UInt32 lastReadTime;
		public UInt32 LastReadTime { get { return lastReadTime; } }

		/// <summary>
		/// Command ID of the first time the tag was read (in the current session)
		/// </summary>
		[DBFieldInfo(4, AllowDuplicate = true, AllowNull = false)]
		public Int32 firstCommand;
		public Int32 FirstCommand { get { return firstCommand; } }

		/// <summary>
		/// Command ID of the most recent time the tag was actually read (in the current session)
		/// </summary>
		[DBFieldInfo(5, AllowDuplicate = true, AllowNull = false)]
		public Int32 lastCommand;
		public Int32 LastCommand { get { return lastCommand; } }


		/// <summary>
		/// Count of the number of distinct cycles in which the tag was actually read / inventoried (in the current session)
		/// </summary>
		[DBFieldInfo(6, AllowDuplicate = true, AllowNull = false)]
		public Int32 actualCycleCount;
		public Int32 ActualCycleCount { get { return actualCycleCount; } }

		/// <summary>
		/// Count of the total number of distinct cycle in which the tag could have been read / inventoried (in the current session)
		/// </summary>
		[DBFieldInfo(7, AllowDuplicate = true, AllowNull = false)]
		public Int32 totalCycleCount;
		public Int32 TotalCycleCount { get { return totalCycleCount; } }

		/// <summary>
		/// Count of the number of distinct reads (inventory singulation) in the current (most recent) cycle
		/// </summary>
		[DBFieldInfo(8, AllowDuplicate = true, AllowNull = false)]
		public Int32 cycleReadCount;
		public Int32 CycleReadCount { get { return cycleReadCount; } }


		[DBFieldInfo(9, AllowDuplicate = true, AllowNull = false)]
		public Int32 sumOfCommandReads;
		public Int32 SumOfCommandReads { get { return sumOfCommandReads; } }

		[DBFieldInfo(10, AllowDuplicate = true, AllowNull = false)]
		public Double averageReadsPerCycle = -1;
		public Double? AverageReadsPerCycle
		{
			get
			{
				return averageReadsPerCycle == -1 ? null : (Double?)averageReadsPerCycle;
			}
		}

		public Double? SimpleAverage { get { return totalCycleCount == 0 ? null : (Double?)((double)readCount / (double)totalCycleCount); } }
		//		[DBFieldInfo(7, AllowDuplicate=true, AllowNull=false)]
		//		public UInt32 antennaCycleCount;
		//		public UInt32 AntennaCycleCount { get { return antennaCycleCount; } }

		//		[DBFieldInfo(8, AllowDuplicate=true, AllowNull=false)]
		//		public Int32 firstReaderCycle;
		//		public Int32 FirstReaderCycle { get { return firstReaderCycle; } }

		[DBFieldInfo(11, AllowDuplicate = true, AllowNull = false)]
		public Int32 lastReaderCycle;
		public Int32 LastReaderCycle { get { return lastReaderCycle; } }

		[DBFieldInfo(12, AllowDuplicate = true, AllowNull = false)]
		public UInt32 antennaCycleCount;
		public UInt32 AntennaCycleCount { get { return antennaCycleCount; } }

		[DBFieldInfo(13, AllowDuplicate = true, AllowNull = false)]
		public Int32 firstAntennaCycle;
		public Int32 FirstAntennaCycle { get { return firstAntennaCycle; } }

		[DBFieldInfo(14, AllowDuplicate = true, AllowNull = false)]
		public Int32 lastAntennaCycle;
		public Int32 LastAntennaCycle { get { return lastAntennaCycle; } }

		[DBFieldInfo(15, AllowDuplicate = true, AllowNull = false, DBType = typeof(Int32))]
		public Int32? port0reads;
		public Int32? Port0reads { get { return port0reads; } }

		[DBFieldInfo(16, AllowDuplicate = true, AllowNull = false, DBType = typeof(Int32))]
		public Int32? port1reads;
		public Int32? Port1reads { get { return port1reads; } }

		[DBFieldInfo(17, AllowDuplicate = true, AllowNull = false, DBType = typeof(Int32))]
		public Int32? port2reads;
		public Int32? Port2reads { get { return port2reads; } }

		[DBFieldInfo(18, AllowDuplicate = true, AllowNull = false, DBType = typeof(Int32))]
		public Int32? port3reads;
		public Int32? Port3reads { get { return port3reads; } }

		[DBFieldInfo(19, AllowDuplicate = true, AllowNull = false, DBType = typeof(Int32))]
		public Int32? port4reads;
		public Int32? Port4reads { get { return port4reads; } }

		[DBFieldInfo(20, AllowDuplicate = true, AllowNull = false, DBType = typeof(Int32))]
		public Int32? port5reads;
		public Int32? Port5reads { get { return port5reads; } }

		[DBFieldInfo(21, AllowDuplicate = true, AllowNull = false, DBType = typeof(Int32))]
		public Int32? port6reads;
		public Int32? Port6reads { get { return port6reads; } }

		[DBFieldInfo(22, AllowDuplicate = true, AllowNull = false, DBType = typeof(Int32))]
		public Int32? port7reads;
		public Int32? Port7reads { get { return port7reads; } }

		[DBFieldInfo(23, AllowDuplicate = true, AllowNull = false, DBType = typeof(Int32))]
		public Int32? port8reads;
		public Int32? Port8reads { get { return port8reads; } }

		[DBFieldInfo(24, AllowDuplicate = true, AllowNull = false, DBType = typeof(Int32))]
		public Int32? port9reads;
		public Int32? Port9reads { get { return port9reads; } }

		[DBFieldInfo(25, AllowDuplicate = true, AllowNull = false, DBType = typeof(Int32))]
		public Int32? port10reads;
		public Int32? Port10reads { get { return port10reads; } }

		[DBFieldInfo(26, AllowDuplicate = true, AllowNull = false, DBType = typeof(Int32))]
		public Int32? port11reads;
		public Int32? Port11reads { get { return port11reads; } }

		[DBFieldInfo(27, AllowDuplicate = true, AllowNull = false, DBType = typeof(Int32))]
		public Int32? port12reads;
		public Int32? Port12reads { get { return port12reads; } }

		[DBFieldInfo(28, AllowDuplicate = true, AllowNull = false, DBType = typeof(Int32))]
		public Int32? port13reads;
		public Int32? Port13reads { get { return port13reads; } }

		[DBFieldInfo(29, AllowDuplicate = true, AllowNull = false, DBType = typeof(Int32))]
		public Int32? port14reads;
		public Int32? Port14reads { get { return port14reads; } }

		[DBFieldInfo(30, AllowDuplicate = true, AllowNull = false, DBType = typeof(Int32))]
		public Int32? port15reads;
		public Int32? Port15reads { get { return port15reads; } }

		[DBFieldInfo(31, AllowDuplicate = true, AllowNull = false)]
		public UInt32 inventoryRoundCount;
		public UInt32 InventoryRoundCount { get { return inventoryRoundCount; } }

		[DBFieldInfo(32, AllowDuplicate = true, AllowNull = false)]
		public Int32 firstinventoryRound;
		public Int32 FirstinventoryRound { get { return firstinventoryRound; } }

		[DBFieldInfo(33, AllowDuplicate = true, AllowNull = false)]
		public Int32 lastinventoryRound;
		public Int32 LastinventoryRound { get { return lastinventoryRound; } }

		[DBFieldInfo(34, Timestamp = true, AllowDuplicate = true, AllowNull = false)]
        public DateTime timeStamp = DateTime.UtcNow;
		public DateTime Timestamp { get { return timeStamp; } }

        //clark 2011.2.14 copied from R1000 Tracer
        [DBFieldInfo(35, AllowDuplicate = true, AllowNull = false)]
        public string dataRead;
        public string DataRead { get { return dataRead; } }
	}

	[System.Runtime.InteropServices.GuidAttribute("BD2E2549-9930-43e2-B683-BA2C872D1EE1")]
	public class ReadRate : DatabaseRowTemplate
	{
		[DBFieldInfo(0, AllowDuplicate = true, AllowNull = false)]
		public int? readerID;
		public int? ReaderID { get { return readerID; } }

		[DBFieldInfo(1, AllowDuplicate = false, AllowNull = false, PrimaryKey = true)]
		public Int32 readSequence;
		public Int32 ReadSequence { get { return readSequence; } }


		// Like ReadTime
		[DBFieldInfo(2, AllowDuplicate = true, AllowNull = false)]
		public Int64 clientReadTime;
		public Int64 ClientReadTime { get { return clientReadTime; } }


		// Like inventoryTime 
		[DBFieldInfo(3, AllowDuplicate = true, AllowNull = false)]
		public UInt32 deviceReadTime;
		public UInt32 DeviceReadTime { get { return deviceReadTime; } }

		[DBFieldInfo(4, Timestamp = true, AllowDuplicate = true, AllowNull = false)]
		public DateTime Timestamp = DateTime.UtcNow;

	}


	public class rfidRFIDReaderClass
	{
		public static StringCollection ReaderList
		{
			get
			{
				RFID.RFIDInterface.Properties.Settings settings = new RFID.RFIDInterface.Properties.Settings();
				return settings["SupportedReaders"] as StringCollection;
			}
		}
		 
	} // public class rfidRFIDReaderClass


	public class rfidReaderID
	{
		public enum ReaderType
		{
			Unknown = -1,
			MTI	=  0
		}
		
		public enum LocationTypeID
		{
			Unknown,
			IPAddressPortNumber,
			HostName,
			ComPort,
			LocalDevice
		}

		public  UInt32			_handle = 0;
		private ReaderType		_readerType = ReaderType.Unknown;
		private string			_readerName = null;
		private string			_description = null;
		private string			_location = null;
		private LocationTypeID	_locationType = LocationTypeID.Unknown;

		private rfidReaderID() { }

		/// <summary>
		/// Constructor for client applications to request reader access at a given location
		/// </summary>
		/// <param name="readerLocation"></param>
		/// <param name="locationType"></param>
		public rfidReaderID(ReaderType type, string readerLocation, LocationTypeID locationType)
		{
			_readerType		= type;
			_location		= readerLocation;
			_locationType	= locationType;
		}


		/// <summary>
		/// Constructor for rfidInterface to provide the client with a list of readers (and handles).
		/// </summary>
		/// <param name="handle">Object passed to BindReader to create an interface to a specific physical reader.</param>
		/// <param name="readerName">Human-readable name for reader.</param>
		/// <param name="description">reader description.</param>
		/// <param name="location">reader location.</param>
		/// <param name="locationType">Type of dataFile stored in location.</param>
		public rfidReaderID(ReaderType type, UInt32 handle, string readerName, string description, string location, LocationTypeID locationType)
		{
			_readerType		= type;
			_handle			= handle;
			_readerName		= readerName;
			_description	= description;
			_location		= location;
			_locationType	= locationType;
		}


		/// <summary>
		/// Constructor for rfidInterface to provide the client a list of readers (and handles) based on a location.
		/// </summary>
		/// <param name="handle"></param>
		/// <param name="hostLocation"></param>
		public rfidReaderID(ReaderType type, UInt32 handle, rfidReaderID readerID)
		{
			_readerType		= type;
			_handle			= handle;
			_readerName		= readerID.Name;
			_description	= readerID.Description;
			_location		= readerID.Location;
			_locationType	= readerID.LocationType;	
		}

		/// <summary>
		/// Constructor for use when reading a file of packets from disk.
		/// </summary>
		/// <param name="ReaderName"></param>
		public rfidReaderID(ReaderType type, string ReaderName)
		{
			_readerType = type;
			_readerName = ReaderName;
		}

		public ReaderType Type
		{
			get { return _readerType; }
		}

		public UInt32 Handle
		{
			get { return _handle; }
		}

		public string Name
		{
			get { return _readerName; }
			set { _readerName = value; }
		}

		public string Description
		{
			get { return _description; }
			set { _description = value; }
		}

		public string Location
		{
			get { return _location; }
			set { _location = value; }
		}

		public LocationTypeID LocationType
		{
			get { return _locationType; }
			set { _locationType = value; }
		}


	} //public class rfidReaderID


	

	enum rfidReaderType
	{
		Embedded,
		Attached,
		Remote
	}

	/// <summary>
	/// Idetifies an RIFD Tag Class (Air Protocol)
	/// </summary>
	public class rfidTagClass
	{
		public enum TagClassID
		{
			ISO_18000_6a,
			ISO_18000_6b,
			ISO_18000_6c,
			Other
		}


		public readonly TagClassID ID;
		public readonly string Name;
		public readonly string ShortName;
		public rfidTagClass(TagClassID tagID)
		{
			ID = tagID;
			switch (tagID)
			{
			case TagClassID.ISO_18000_6a:
				Name = "ISO 18000 6a";
				ShortName = "ISO 18000 6a";
				break;

			case TagClassID.ISO_18000_6b:
				Name = "ISO 18000 6b";
				ShortName = "ISO 18000 6b";
				break;

			case TagClassID.ISO_18000_6c:
				Name = "ISO 18000 6c";
				ShortName = "ISO 18000 6c";
				break;

			default:
				ShortName = Enum.GetName(typeof(TagClassID), tagID);
				Name = "Tag Class: " + Enum.GetName(typeof(TagClassID), tagID);
				break;
			}
		}
	}//public class rfidTagClass


	public class rfidTagClassList { public List<rfidTagClass> TagClasses = new List<rfidTagClass>(); 	}


	public class rfidProperty<T>
	{
		private T		_id;
		private Object	_value			= null;
		private Type	_type			= null;
		private Type	_enum			= typeof(T);
		private string	_name			= null;
		private string	_display		= null;
		private string	_helptext		= null;
		
		private bool	_readOnly		= false;
		private bool	_required		= false;
		private bool	_hasValue		= false;
		private bool	_standard		= false;

		public rfidProperty(T name, Type type)
		{
			_id = name;
			_type = type;
			_name = Enum.GetName(typeof(T), name);
			_standard = true;
			_display = _name;
		}

		public rfidProperty(string Name, bool Required)
		{
			if (IsStandardName(Name)) throw new rfidException(rfidErrorCode.StandardNameUsedAsCustom, String.Format("The standard name {0}, cannot be a custom property name.", Name));

			_name = Name;
			_required = Required;
		}

		public static string[] StandardNames
		{
			get
			{
				return Enum.GetNames(typeof(T));
			}
		}

		public override string ToString()
		{
			string name = string.Format("{0} : ", Name.PadRight(20).Substring(0, 20));
			string type = string.Format("[{0}]", PropertyType.Name).PadRight(20).Substring(0, 20);
//			string value = string.Format("{0}", PropertyType.IsValueType ? (HasValue ? Value.ToString() : " - ") : PropertyType.Name == "String" ? Value.ToString() : "");
			string value = "";
			if (HasValue)
			{
				if (PropertyType.IsValueType)
				{
					value = Value.ToString();
				}
				else
				{
					if (PropertyType.Name == "String")
						value = (string)Value;
					else
						value = "<object>";
				}
			}
			else
			{
				value = "value not set";
			}
			
			return string.Format("{0}\t{1}\t{2}", name, type, value);
		}

		
		public T Property
		{
			get { return _id; }
		}

		public string Name
		{
			get { return _name; }
			private set { _name = value; }
		}

		public Object Value
		{
			get { return _value; }
			set 
			{
				if (value.GetType() != _type)
					throw new ArgumentException(String.Format("Cannot set rfidProperty[{0}] to a object of type {1}", Name, value.GetType().Name));

				_hasValue = true;
				_value = value; 
			}
		}

		public Type PropertyType
		{
			get { return _type; }
		}

		public bool HasValue
		{
			get { return _hasValue; }
		}

		public bool IsRequiredPoperty
		{
			get { return _required; }
		}

		public bool IsReadOnlyProperty
		{
			get { return _readOnly; }
			set { _readOnly = value; }
		}

		public bool IsStandard
		{
			get { return _standard; }
			set { _standard = value; }
		}

		public static bool IsStandardName(string name)
		{
			return Array.IndexOf<string>(StandardNames, name) != -1;
		}

		public string DisplayText
		{
			get { return _display; }
			set { _display = value; }
		}

		public string HelpText
		{
			get { return _helptext; }
			set { _helptext = value; }
		}


	}

	/// <summary>
	/// 
	/// </summary>
	//public class rfidPropertyList<T> : Dictionary<string, rfidProperty<T>>
	//{
	//    public rfidPropertyList()
	//    {
	//        // Add standard properties
	//        foreach (T prop in Enum.GetValues(typeof(T)))
	//        {
	//            string key = rfidStandardPropertyLoader.StandardKey(prop);
	//            rfidHelperClass item = rfidStandardPropertyLoader.StandardProperties[key];
	//            Add(item.Name, new rfidProperty<T>(prop, item.PropertyType));
	//        }
	//    }	
	//}



	/// <summary>
	/// 
	/// </summary>
	public class rfidReader
	{
		/*
		public enum StandardProperties
		{
			AntennaList,
			CurrentReadMode,
			CurrentTestMode,
			EnabledTagClasses,
			Manufacturer,
			Model,
			ReaderType,
			SupportedCommandSet,
			SupportedReadModes,
			SupportedStatistics,
			SupportedTagClasses,
			SupportedTestModes,
			Version
		}
		*/
		public enum OperationMode
		{
			Static,
			Unbound,
			BoundToReader,
		}

		public enum ReaderRequestType
		{
			GetInventory,
            TagAccess,
		}

		public static string GetNameForRequest(ReaderRequestType t)
		{
			switch (t)
			{
				case ReaderRequestType.GetInventory:
					return "Inventory";

				case ReaderRequestType.TagAccess:
					return "Tag Access";

				default:
					return "Unknown";
			}
		}

	}


	/// <summary>
	/// 
	/// </summary>
	//public class rfidAntenna
	//{
	//    public enum StandardAntennaProperty
	//    {
	//        AntennaId,
	//        Enabled,
	//        AntennaPower,
	//        AntennaPowerRange,
	//    }

	//    public rfidPropertyList<StandardAntennaProperty> Properties;

	//    public rfidAntenna()
	//    {
	//        Properties = new rfidPropertyList<StandardAntennaProperty>();
	//    }
	//}

	//public class rfidAntennaList { public List<rfidAntenna> Antennas = new List<rfidAntenna>(); 	}



	/// <summary>
	/// 
	/// </summary>
	public enum OperationOutcome
	{
		Unknown,
		Success,
		SuccessWithUserCancel,
		SuccessWithInformation,
		FailByContext,
		FailByReaderError,
		FailByApplicationError,
		FailByUserAbort
	}



	public abstract class ReportBase
	{
		public abstract Object Context					{ get; }
		public abstract TimeSpan Duration				{ get; }
		public abstract OperationOutcome Outcome		{ get; }
		public abstract Exception OperationException	{ get; }
	}


	/// <summary>
	/// 
	/// </summary>
	public class rfidSimpleReport : ReportBase, IFormattable
	{
		private Object _context;
		private OperationOutcome _outcome;
		private Exception _exception;
		
		private string _message;
		private long _startTimeMS;
		private long _endTimeMS;

		private Object _newReader;


		public rfidSimpleReport(Object context, long msStartTime)
		{
			_context = context;
			_startTimeMS = _endTimeMS = msStartTime;
		}

		public rfidSimpleReport(object context, OperationOutcome outcome, Exception exception)
		{
			_context	= context;
			_outcome	= outcome;
			_exception	= exception;
		}

		public override object Context
		{
			get { return _context; }
		}

		public override TimeSpan Duration
		{
			get { 	return new TimeSpan(TimeSpan.TicksPerMillisecond * (_endTimeMS - _startTimeMS)); }
		}

		public override OperationOutcome Outcome
		{
			get { return _outcome; }
		}

		public override Exception OperationException
		{
			get { return _exception; }
		}

		public Object NewReader
		{
			get { return _newReader; }
			set { _newReader = value; }
		}

		public rfidSimpleResultReport GetProgressReport(long asOfTimeMS)
		{
			_message = null;
			_endTimeMS = asOfTimeMS;
			return new rfidSimpleResultReport(Context, "", Duration);
		}

		public rfidSimpleResultReport GetProgressReport(string ResultMessage, long asOfTimeMS)
		{
			_message = ResultMessage;
			_endTimeMS = asOfTimeMS;
			return new rfidSimpleResultReport(Context, ResultMessage, Duration);
		}

		public void OperationCompleted(OperationOutcome outcome, long asOfTimeMS)
		{
			_outcome = outcome;
			_message = null;
			_endTimeMS = asOfTimeMS;
		}

		public void OperationCompleted(OperationOutcome outcome, string ResultMessage, long asOfTimeMS)
		{
			_outcome = outcome;
			_message = ResultMessage;
			_endTimeMS = asOfTimeMS;
		}

		public override string ToString()
		{
			switch (Outcome)
			{

				case OperationOutcome.Success:
					return String.Format(DurationFormat.Formatter, "{0} ({1:c})", String.IsNullOrEmpty(_message) ? "Successful" : _message, Duration);

				case OperationOutcome.SuccessWithUserCancel:
					return String.Format(DurationFormat.Formatter, "{0} ({1:c})", String.IsNullOrEmpty(_message) ? "Canceled" : _message, Duration);

				case OperationOutcome.SuccessWithInformation:
					return String.Format(DurationFormat.Formatter, "{0} ({1:c})", String.IsNullOrEmpty(_message) ? "Successful *" : _message, Duration);

				case OperationOutcome.FailByContext:
					return String.Format(DurationFormat.Formatter, "{0} ({1:c})", String.IsNullOrEmpty(_message) ? "Failed, invalid context" : _message, Duration);

				case OperationOutcome.FailByReaderError:
					return String.Format(DurationFormat.Formatter, "{0} ({1:c})", String.IsNullOrEmpty(_message) ? "Failed, reader error" : _message, Duration);

				case OperationOutcome.FailByApplicationError:
					return String.Format(DurationFormat.Formatter, "{0} ({1:c})", String.IsNullOrEmpty(_message) ? "Failed, application error" : _message, Duration);

				case OperationOutcome.FailByUserAbort:
					return String.Format(DurationFormat.Formatter, "{0} ({1:c})", String.IsNullOrEmpty(_message) ? "Aborted" : _message, Duration);
				
				case OperationOutcome.Unknown:
				default:
					return String.Format(DurationFormat.Formatter, "{0} ({1:c})", String.IsNullOrEmpty(_message) ? "Unknown" : _message, Duration);
					
			}
		}

		#region IFormattable Members

		public string ToString(string format, IFormatProvider formatProvider)
		{
			if (format == null || format.Equals("") || format.Equals("g") || format.Equals("G"))
			{
				return ToString();
			}
			else
			{
				return Convert.ToString(this, formatProvider);
			}
		}

		#endregion

	}

	/// <summary>
	/// 
	/// </summary>
	public class rfidSimpleResultReport : ReportBase, IFormattable
	{
		private Object _context;
		private string _ResultMessage;
		private TimeSpan _duration;


		public rfidSimpleResultReport(Object context, string ResultMessage, TimeSpan duration)
		{
			_context = context;
			_ResultMessage = ResultMessage;
			_duration = duration;
		}


		public override object Context
		{
			get { return _context; }
		}

		public override TimeSpan Duration
		{
			get { return _duration; }
		}

		public override OperationOutcome Outcome
		{
			get { return OperationOutcome.Unknown; }
		}

		public override Exception OperationException
		{
			get { return null; }
		}


		public override string ToString()
		{
			return String.Format(DurationFormat.Formatter, "{0} ({1:c})", _ResultMessage, Duration);
		}

		#region IFormattable Members

		public string ToString(string format, IFormatProvider formatProvider)
		{
			if (format == null || format.Equals("") || format.Equals("g") || format.Equals("G"))
			{
				return ToString();
			}
			else
			{
				return Convert.ToString(this, formatProvider);
			}
		}

		#endregion

	}


	/// <summary>
	/// 
	/// </summary>
	public abstract class rfidReportBase : ReportBase
	{
//		public abstract TimeSpan Duration			   { get; }
        public abstract TimeSpan SessionDuration       { get; }
		public abstract TimeSpan TotalDuration		   { get; }
        
		public abstract int RequestCount			   { get; }
        public abstract int SessionRequestCount        { get; }
		public abstract int TotalRequestCount		   { get; }
		

		public abstract int AntennaCycleCount		   { get; }
        public abstract int SessionAntennaCycleCount   { get; }
		public abstract int TotalAntennaCycleCount	   { get; }
		

		public abstract int InventoryCycleCount		   { get; }
        public abstract int SessionInventoryCycleCount { get; }
		public abstract int TotalInventoryCycleCount   { get; }


        public abstract int PacketCount                { get; }
		public abstract int SessionPacketCount		   { get; }
		public abstract int TotalPacketCount		   { get; }
		

        public abstract int BadPacketCount             { get; }		
		public abstract int SessionBadPacketCount	   { get; }
        public abstract int TotalBadPacketCount        { get; }

        public abstract int RoundCount                 { get; }
		public abstract int SessionRoundCount		   { get; }
		public abstract int TotalRoundCount			   { get; }

        public abstract int TagCount                   { get; }
		public abstract int SessionTagCount			   { get; }
		public abstract int TotalTagCount			   { get; }
		

		public abstract int SessionUniqueTags		   { get; }
		public abstract int RequestUniqueTags		   { get; }
		public abstract int CurrentUniqueTags		   { get; }

		public abstract float TotalRate				   { get; }
		public abstract float Rate					   { get; }

		public abstract int CrcErrorCount			   { get; }
        public abstract int SessionCrcErrorCount       { get; }
		public abstract int TotalCrcErrorCount		   { get; }
		
	}



	public class rfidProgressReport : rfidReportBase, IFormattable
	{
		
		private Object _context;

        private TimeSpan _duration;
        private TimeSpan _sessionDuration;
        private TimeSpan _totalDuration;        

		private int _requestCount;
        private int _sessionRequestCount;
		private int _totalRequestCount;		

		private int _antennaCycleCount;
        private int _sessionAntennaCycleCount;
		private int _totalAntennaCycleCount;		

		private int _inventoryCycleCount;
        private int _sessionInventoryCycleCount;
		private int _totalInventoryCycleCount;		

		private int _badPacketCount;
        private int _sessionBadPacketCount;
		private int _totalBadPacketCount;
		
		private int _crcErrorCount;
        private int _sessionCrcErrorCount;
		private int _totalCrcErrorCount;		

		private int _packetCount;
        private int _sessionPacketCount;
		private int _totalPacketCount;		

		private int _roundCount;
        private int _sessionRoundCount;
		private int _totalRoundCount;		

		private int _tagCount;
        private int _sessionTagCount;
		private int _totalTagCount;


		private int _sessionUniqueTags;
		private int _requestUniqueTags;
		private int _currentUniqueTags;



		public rfidProgressReport(
									Object context,
									int requestCount,	
									int totalRequestCount,
									int sessionRequestCount,

									int antennaCycleCount,
									int totalAntennaCycleCount,
									int sessionAntennaCycleCount,

									int inventoryCycleCount,
									int totalInventoryCycleCount,
									int sessionInventoryCycleCount,

									int badPacketCount,
									int totalBadPacketCount,
									int sessionBadPacketCount,
									
									int crcErrorCount,
									int totalCrcErrorCount,
									int sessionCrcErrorCount,

									int packetCount,	
									int totalPacketCount,
									int sessionPacketCount,
									
									int roundCount,		
									int totalRoundCount,
									int sessionRoundCount,
									
									int tagCount,		
									int totalTagCount,
									int sessionTagCount,
									
									TimeSpan duration,	
									TimeSpan totalDuration,
									TimeSpan sessionDuration,
									
									int sessionUniqueTags,
									int requestUniqueTags,
									int currentUniqueTags
									)
		{
			_context					= context;
			_requestCount				= requestCount;
			_antennaCycleCount			= antennaCycleCount;
			_inventoryCycleCount		= inventoryCycleCount;
			_badPacketCount				= badPacketCount;
			_crcErrorCount				= crcErrorCount;
			_packetCount				= packetCount;
			_roundCount					= roundCount;
			_tagCount					= tagCount;
			_duration					= duration;

			_sessionRequestCount		= sessionRequestCount;
			_sessionAntennaCycleCount	= sessionAntennaCycleCount;
			_sessionInventoryCycleCount = sessionInventoryCycleCount;
			_sessionBadPacketCount		= sessionBadPacketCount;
			_sessionCrcErrorCount		= sessionCrcErrorCount;
			_sessionPacketCount			= sessionPacketCount;
			_sessionRoundCount			= sessionRoundCount;
			_sessionTagCount			= sessionTagCount;

			_totalRequestCount			= totalRequestCount;
			_totalAntennaCycleCount		= totalAntennaCycleCount;
			_totalInventoryCycleCount	= totalInventoryCycleCount;
			_totalBadPacketCount		= totalBadPacketCount;
			_totalCrcErrorCount			= totalCrcErrorCount;
			_totalPacketCount			= totalPacketCount;
			_totalRoundCount			= totalRoundCount;
			_totalTagCount				= totalTagCount;
			_totalDuration				= totalDuration;
			_sessionDuration			= sessionDuration;

			_sessionUniqueTags			= sessionUniqueTags;
			_requestUniqueTags			= requestUniqueTags;
			_currentUniqueTags			= currentUniqueTags;

		}

		public override int RequestCount
		{
			get { return _requestCount; }
		}

		public override int TotalRequestCount
		{
			get { return _totalRequestCount; }
		}

		public override int SessionRequestCount
		{
			get { return _sessionRequestCount; }
		}

		public override int AntennaCycleCount
		{
			get { return _antennaCycleCount; }
		}

		public override int TotalAntennaCycleCount
		{
			get { return _totalAntennaCycleCount; }
		}

		public override int SessionAntennaCycleCount
		{
			get { return _sessionAntennaCycleCount; }
		}

		public override int InventoryCycleCount
		{
			get { return _inventoryCycleCount; }
		}

		public override int TotalInventoryCycleCount
		{
			get { return _totalInventoryCycleCount; }
		}

		public override int SessionInventoryCycleCount
		{
			get { return _sessionInventoryCycleCount; }
		}

		public override int BadPacketCount
		{
			get { return _badPacketCount; }
		}

		public override int TotalBadPacketCount
		{
			get { return _totalBadPacketCount; }
		}

		public override int SessionBadPacketCount
		{
			get { return _sessionBadPacketCount; }
		}

		public override int CrcErrorCount
		{
			get { return _crcErrorCount; }
		}

		public override int TotalCrcErrorCount
		{
			get { return _totalCrcErrorCount; }
		}

		public override int SessionCrcErrorCount
		{
			get { return _sessionCrcErrorCount; }
		}

		public override int PacketCount
		{
			get { return _packetCount; }
		}

		public override int TotalPacketCount
		{
			get { return _totalPacketCount; }
		}

		public override int SessionPacketCount
		{
			get { return _sessionPacketCount; }
		}

		public override int RoundCount
		{
			get { return _roundCount; }
		}

		public override int TotalRoundCount
		{
			get { return _totalRoundCount; }
		}

		public override int SessionRoundCount
		{
			get { return _sessionRoundCount; }
		}

		public override int TagCount
		{
			get { return _tagCount; }
		}

		public override int TotalTagCount
		{
			get { return _totalTagCount; }
		}

		public override int SessionTagCount
		{
			get { return _sessionTagCount; }
		}

		public override TimeSpan SessionDuration
		{
			get { return _sessionDuration; }
		}

		public override TimeSpan Duration
		{
			get { return _duration; }
		}

		public override TimeSpan TotalDuration
		{
			get { return _totalDuration; }
		}

		public override int SessionUniqueTags
		{
			get { return _sessionUniqueTags; }
		}

		public override int RequestUniqueTags
		{
			get { return _requestUniqueTags; }
		}

		public override int CurrentUniqueTags
		{
			get { return _currentUniqueTags; }
		}

		public override float Rate
		{
			get { return (float)((double)TagCount / Duration.TotalSeconds); }
		}

		public override float TotalRate 
		{
			get { return (float)((double)TotalTagCount / TotalDuration.TotalSeconds); }
		}

		public override string ToString()
		{
			return String.Format(DurationFormat.Formatter, "{0:n0} tags in {1:c} ({2:F1} tags/second)", TotalTagCount, TotalDuration, TotalRate);
		}

		#region IFormattable Members

		public string ToString(string format, IFormatProvider formatProvider)
		{
			if (format == null || format.Equals("") || format.Equals("g") || format.Equals("G"))
			{
				return ToString();
			}
			else
			{
				return Convert.ToString(this, formatProvider);
			}
		}

		#endregion

		public override object Context
		{
			get { return _context; }
		}

		public override OperationOutcome Outcome
		{
			get { return OperationOutcome.Unknown; }
		}

		public override Exception OperationException
		{
			get { return null; }
		}

	}


	public class rfidOperationReport : rfidReportBase, IFormattable
	{
		private Object _context;
		private OperationOutcome _outcome = OperationOutcome.Unknown;
		private Exception _exception = null;


		public long StartTimeMS;
		public long	PriorTimeMS;
		public long	EndTimeMS;

		public int StartRequestCount;
		public int PriorRequestCount;
		public int EndRequestCount;

		public int StartAntennaCycleCount;
		public int PriorAntennaCycleCount;
		public int EndAntennaCycleCount;

		public int StartInventoryCycleCount;
		public int PriorInventoryCycleCount;
		public int EndInventoryCycleCount;

		public int StartBadPacketCount;
		public int PriorBadPacketCount;
		public int EndBadPacketCount;

		public int StartCrcErrorCount;
		public int PriorCrcErrorCount;
		public int EndCrcErrorCount;

		public int StartPacketCount;
		public int PriorPacketCount;
		public int EndPacketCount;

		public int StartRoundCount;
		public int PriorRoundCount;
		public int EndRoundCount;

		public int StartTagCount;
		public int PriorTagCount;
		public int EndTagCount;

		private int _sessionUniqueTags;
		private int _requestUniqueTags;
		private int _currentUniqueTags;
		
		private TimeSpan _sessionDuration;


//		private String	_additionalInfo;
//		private String _errorMessage;

		
		public rfidOperationReport(object context)
		{
			_context = context;
		}

		public rfidOperationReport(
										object context, 
										long msStartTime, 
										int requestCount, 
										int antennaCycleCount,
										int inventoryCycleCount,
										int badPacketCount,
										int crcErrorCount,
										int packetCount, 
										int roundCount, 
										int tagCount, 
										int sessionUniqueTags, 
										int requestUniqueTags,
										int currentUniqueTags,
										TimeSpan sessionDuration
									)
		{
			_context			= context;

			EndTimeMS				= PriorTimeMS				= StartTimeMS				= msStartTime;
			EndRequestCount			= PriorRequestCount			= StartRequestCount			= requestCount;
			EndAntennaCycleCount	= PriorAntennaCycleCount	= StartAntennaCycleCount	= antennaCycleCount;
			EndInventoryCycleCount	= PriorInventoryCycleCount	= StartInventoryCycleCount	= inventoryCycleCount;
			EndBadPacketCount		= PriorBadPacketCount		= StartBadPacketCount		= badPacketCount;
			EndCrcErrorCount		= PriorCrcErrorCount		= StartCrcErrorCount		= crcErrorCount;
			EndPacketCount			= PriorPacketCount			= StartPacketCount			= packetCount;
			EndRoundCount			= PriorRoundCount			= StartRoundCount			= roundCount;
			EndTagCount				= PriorTagCount				= StartTagCount				= tagCount;

			_sessionUniqueTags	= sessionUniqueTags;
			_requestUniqueTags	= requestUniqueTags;
			_currentUniqueTags	= currentUniqueTags;
			_sessionDuration	= sessionDuration;
		}


		/*
		public String AdditionalInfo
		{
			get { return _additionalInfo; }
			set { _additionalInfo = value; }
		}
		
		public String ErrorMessage
		{
			get { return _errorMessage; }
			set { _errorMessage = value; }
		}
		*/
		public override int SessionRequestCount
		{
			get { return EndRequestCount; }
		}

		public override int SessionAntennaCycleCount
		{
			get { return EndAntennaCycleCount; }
		}

		public override int SessionInventoryCycleCount
		{
			get { return EndInventoryCycleCount; }
		}

		public override int SessionBadPacketCount
		{
			get { return EndBadPacketCount; }
		}

		public override int SessionCrcErrorCount
		{
			get { return EndCrcErrorCount; }
		}

		public override int SessionUniqueTags
		{
			get { return _sessionUniqueTags; }
		}

		public override int RequestUniqueTags
		{
			get { return _requestUniqueTags; }
		}

		public override int CurrentUniqueTags
		{
			get { return _currentUniqueTags; }
		}

		public override int TotalRequestCount
		{
			get { return EndRequestCount - StartRequestCount; }
		}

		public override int RequestCount
		{
			get { return EndRequestCount - PriorRequestCount; }
		}


		public override int  TotalAntennaCycleCount
		{
			get { return EndAntennaCycleCount - StartAntennaCycleCount; }
		}
		
		public override int AntennaCycleCount
		{
			get { return EndAntennaCycleCount - PriorAntennaCycleCount; }
		}

		public override int TotalInventoryCycleCount
		{
			get { return EndInventoryCycleCount - StartInventoryCycleCount; }
		}

		public override int InventoryCycleCount
		{
			get { return EndInventoryCycleCount - PriorInventoryCycleCount; }
		}

		public override int TotalBadPacketCount
		{
			get { return EndBadPacketCount - StartBadPacketCount; }
		}

		public override int BadPacketCount
		{
			get { return EndBadPacketCount - PriorBadPacketCount; }
		}

		public override int TotalCrcErrorCount
		{
			get { return EndCrcErrorCount - StartCrcErrorCount; }
		}

		public override int CrcErrorCount
		{
			get { return EndCrcErrorCount - PriorCrcErrorCount; }
		}


		public override int SessionPacketCount
		{
			get { return EndPacketCount; }
		}

		public override int TotalPacketCount
		{
			get { return EndPacketCount - StartPacketCount;	}
		}

		public override int PacketCount
		{
			get { return EndPacketCount - PriorPacketCount; }
		}


		public override int SessionRoundCount
		{
			get { return EndRoundCount; }
		}

		public override int TotalRoundCount
		{
			get { return EndRoundCount - StartRoundCount; }
		}

		public override int RoundCount
		{
			get { return EndRoundCount - PriorRoundCount; }
		}

		public override int SessionTagCount
		{
			get { return EndTagCount; }
		}

		public override int TotalTagCount
		{
			get { return EndTagCount - StartTagCount;			}
		}

		public override int TagCount
		{
			get { return EndTagCount - PriorTagCount; }
		}

		public override TimeSpan SessionDuration
		{
			get { return _sessionDuration; }
		}
		
		public override TimeSpan Duration
		{
			get { 	return new TimeSpan(TimeSpan.TicksPerMillisecond * (EndTimeMS - PriorTimeMS)); }
		}

		public override TimeSpan TotalDuration
		{
			get 
            {
                return new TimeSpan(TimeSpan.TicksPerMillisecond * (EndTimeMS - StartTimeMS));
            }
		}


		public override float Rate
		{
			get { return (float)(((double)TagCount) / Duration.TotalSeconds); }
		}

		public override float TotalRate
		{
			get
            {
                return (float)(((double)TotalTagCount) / TotalDuration.TotalSeconds); 
            }
		}

		public void OperationCompleted(OperationOutcome outcome)
		{
			_outcome = outcome;
		}

		public void OperationCompleted(
											OperationOutcome outcome,
											long	endTimeMS,
											int		endRequestCount,
											int		endAntennaCycleCount,
											int		endInventoryCycleCount,
											int		endBadPacketCount,
											int		endCrcErrorCount,
											int		endPacketCount,
											int		endRoundCount,	 
											int		endTagCount,	
											int		sessionUniqueTags,
											int		commandUniqueTags,
											int		currentUniqueTags,
											TimeSpan sessionDuration
										)
		{
			_outcome				= outcome;
            EndTimeMS               = endTimeMS;
			EndRequestCount			= endRequestCount;
			EndAntennaCycleCount	= endAntennaCycleCount;
			EndInventoryCycleCount	= endInventoryCycleCount;
			EndBadPacketCount		= endBadPacketCount;
			EndCrcErrorCount		= endCrcErrorCount;
			EndPacketCount			= endPacketCount;
			EndRoundCount			= endRoundCount;
			EndTagCount				= endTagCount;	 
			_sessionUniqueTags		= sessionUniqueTags;
			_requestUniqueTags		= commandUniqueTags;
			_currentUniqueTags		= currentUniqueTags;
			_sessionDuration		= sessionDuration;
		}


		public rfidProgressReport GetProgressReport(
														long asOfTimeMS, 
														int requestCount,
														int antennaCycleCount,
														int inventoryCycleCount,
														int badPacketCount,
														int crcErrorCount,
														int packetCount, 
														int roundCount, 
														int tagCount, 
														int sessionUniqueTags,
														int commandUniqueTags,
														int currentUniqueTags,
														TimeSpan sessionDuration
													)
		{
            //Debug.WriteLine( this.GetHashCode() + " called GetProgressReport" );
			PriorTimeMS					= EndTimeMS;
            //Debug.WriteLine( this.GetHashCode( ) + " PriorTimeMS            : " + PriorTimeMS );
			PriorRequestCount			= EndRequestCount;
            //Debug.WriteLine( this.GetHashCode( ) + " PriorRequestCount      : " + PriorRequestCount );
			PriorAntennaCycleCount		= EndAntennaCycleCount;
            //Debug.WriteLine( this.GetHashCode( ) + " PriorAntennaCycleCount : " + PriorAntennaCycleCount );
			PriorInventoryCycleCount	= EndInventoryCycleCount;
            //Debug.WriteLine( this.GetHashCode( ) + " PriorTimeMS            : " + PriorInventoryCycleCount );
			PriorBadPacketCount			= EndBadPacketCount;
            //Debug.WriteLine( this.GetHashCode( ) + " PriorBadPacketCount    : " + PriorBadPacketCount );
			PriorCrcErrorCount			= EndCrcErrorCount;
            //Debug.WriteLine( this.GetHashCode( ) + " PriorCrcErrorCount     : " + PriorCrcErrorCount );
			PriorPacketCount			= EndPacketCount;
            //Debug.WriteLine( this.GetHashCode( ) + " PriorPacketCount       : " + PriorPacketCount );
			PriorRoundCount				= EndRoundCount;
            //Debug.WriteLine( this.GetHashCode( ) + " PriorRoundCount        : " + PriorRoundCount );
			PriorTagCount				= EndTagCount;
            //Debug.WriteLine( this.GetHashCode( ) + " PriorTagCount          : " + PriorTagCount );
			
			EndTimeMS					= asOfTimeMS;
			EndRequestCount				= requestCount;
			EndAntennaCycleCount		= antennaCycleCount;
			EndInventoryCycleCount		= inventoryCycleCount;
			EndBadPacketCount			= badPacketCount;
			EndCrcErrorCount			= crcErrorCount;
			EndPacketCount				= packetCount;
			EndRoundCount				= roundCount;
			EndTagCount					= tagCount;
			
			_sessionUniqueTags	= sessionUniqueTags;
			_requestUniqueTags	= commandUniqueTags;
			_currentUniqueTags	= currentUniqueTags;

			_sessionDuration	= sessionDuration;

			return new rfidProgressReport(
												Context, 
												RequestCount,
												TotalRequestCount,
												SessionRequestCount,
												AntennaCycleCount,
												TotalAntennaCycleCount,
												SessionAntennaCycleCount,
												InventoryCycleCount,
												TotalInventoryCycleCount,
												SessionInventoryCycleCount,
												BadPacketCount,
												TotalBadPacketCount,
												SessionBadPacketCount,
												CrcErrorCount,
												TotalCrcErrorCount,
												SessionCrcErrorCount,
												PacketCount,	
												TotalPacketCount,
												SessionPacketCount,
												RoundCount,		
												TotalRoundCount,
												SessionRoundCount,
												TagCount,		
												TotalTagCount, 
												SessionTagCount,
												Duration,		
												TotalDuration,
												SessionDuration,
												SessionUniqueTags,
												RequestUniqueTags,
												CurrentUniqueTags
											);
		}

		public override string ToString()
		{
			switch (Outcome)
			{
			
			
			case OperationOutcome.Success:
				return String.Format(DurationFormat.Formatter, "{0:n0} tags", TotalTagCount);

			case OperationOutcome.SuccessWithUserCancel:
				return String.Format(DurationFormat.Formatter, "{0:n0} tags", TotalTagCount);

//			case OperationOutcome.SuccessWithInformation:
//				return String.Format(DurationFormat.Formatter, "{0}", AdditionalInfo);

			case OperationOutcome.FailByContext:
				return String.Format("Operation failed due to invalid context");

			case OperationOutcome.FailByReaderError:
				return String.Format(DurationFormat.Formatter, "reader error after {0:n0} tag reads", TotalTagCount);

			case OperationOutcome.FailByApplicationError:
				return String.Format(DurationFormat.Formatter, "application error after {0:n0} tag", TotalTagCount);

			case OperationOutcome.FailByUserAbort:
				return String.Format(DurationFormat.Formatter, "user abort after {0:n0} tag", TotalTagCount);

			case OperationOutcome.Unknown:
			default:
				return String.Format("Result is unknown");
			}
		}

		#region IFormattable Members

		public string ToString(string format, IFormatProvider formatProvider)
		{
			if (format == null || format.Equals("") || format.Equals("g") || format.Equals("G"))
			{
				return ToString();
			}
			else
			{
				return Convert.ToString(this, formatProvider);
			}
		}

		#endregion

		public override object Context
		{
			get { return _context; }
		}

		public override OperationOutcome Outcome
		{
			get { return _outcome; }
		}

		public override Exception OperationException
		{
			get { return _exception; }
		}


	}							  
								  

	/// <summary>
	/// 
	/// </summary>
	/*
	public abstract class rfidInterface : IDisposable
	{
//		public abstract rfidPropertyList<rfidReader.StandardProperties> Properties { get; }
		public abstract int TagCount { get; }
		public abstract bool NoTempFileAccess { get; set; }
		public abstract bool EnableLoging { get; set; }
		public abstract string LogPath { get; set; }

		public abstract void SettingsChanged();
		public abstract void ClearSession();
		public abstract List<rfidReaderID> FindReaders();
		public abstract List<rfidReaderID> FindReaders(rfidReaderID whereToLook);
		public abstract rfidInterface BindReader(rfidReaderID Reader);
		public abstract void SaveDataToFile(System.IO.Stream stream);
		public abstract void CloseReader();
		public abstract rfidOperationReport ReadInventory(Object context, BackgroundWorker bw, int refreshRateMS);
		public abstract rfidOperationReport MonitorInventory(Object context, BackgroundWorker bw, int refreshRateMS);
//		public abstract void AssemblyClosing();
		


		#region IDisposable Members

		void IDisposable.Dispose()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		#endregion
	}

	public class rfidHelperClass
	{
		public Object Property;
		public Type PropertyType;
		public bool ReadOnly;
		public bool Required;
		public string Name;
		public string DisplayText;
		public string Helptext;

		public rfidHelperClass(Object property, Type propertyType, bool required, bool readOnly, string display, string helptext)
		{
			Property		= property;
			Name			= Enum.GetName(property.GetType(), property);
			PropertyType	= propertyType;
			ReadOnly		= readOnly;
			Required		= required;
			DisplayText		= display;
			Helptext		= helptext;
		}
	}



	public class rfidStandardProperties : KeyedCollection<string, rfidHelperClass>
	{
		protected override string GetKeyForItem(rfidHelperClass item)
		{
			string key = rfidStandardPropertyLoader.StandardKey(item.Property);
			return key;
		}
	}



	public class rfidStandardPropertyLoader
	{
		public static rfidStandardProperties StandardProperties;


		static rfidStandardPropertyLoader()
		{
			rfidHelperClass[] AllStandardProperties =
				{
					new rfidHelperClass(rfidReader.StandardProperties.AntennaList,					typeof(rfidAntennaList),	true,	false,	"Antenna List", ""),
					new rfidHelperClass(rfidReader.StandardProperties.CurrentReadMode,				typeof(String),				true,	false,	"Read Mode", ""),
					new rfidHelperClass(rfidReader.StandardProperties.CurrentTestMode,				typeof(String),				true,	false,	"Test Mode", ""),
					new rfidHelperClass(rfidReader.StandardProperties.EnabledTagClasses,			typeof(String),				true,	false,	"Enabled Tag Classes", ""),
					new rfidHelperClass(rfidReader.StandardProperties.Manufacturer,					typeof(String),				true,	false,	"Manufacturer", ""),
					new rfidHelperClass(rfidReader.StandardProperties.Model,						typeof(String),				true,	false,	"Model", ""),
					new rfidHelperClass(rfidReader.StandardProperties.ReaderType,					typeof(rfidReaderType),		true,	false,	"reader Type", ""),
					new rfidHelperClass(rfidReader.StandardProperties.SupportedCommandSet,			typeof(String),				true,	false,	"Supported Command Set", ""),
					new rfidHelperClass(rfidReader.StandardProperties.SupportedReadModes,			typeof(String),				true,	false,	"Supported Read Modes", ""),
					new rfidHelperClass(rfidReader.StandardProperties.SupportedStatistics,			typeof(String),				true,	false,	"Supported Statistics", ""),
					new rfidHelperClass(rfidReader.StandardProperties.SupportedTagClasses,			typeof(rfidTagClassList),	true,	false,	"Supported Tag Classes", ""),
					new rfidHelperClass(rfidReader.StandardProperties.SupportedTestModes,			typeof(String),				true,	false,	"Supported Test Modes", ""),
					new rfidHelperClass(rfidReader.StandardProperties.Version,						typeof(String),				true,	false,	"Version", "reader version information"),

					new rfidHelperClass(rfidAntenna.StandardAntennaProperty.AntennaId,				typeof(Int32),				true,	true,	"Antenna ID", "Unique identifier or handle for a specific antenna."),
					new rfidHelperClass(rfidAntenna.StandardAntennaProperty.Enabled,				typeof(Boolean),			true,	false,	"Antenna Enabled", "Enabled/Disabled antenna state."),
					new rfidHelperClass(rfidAntenna.StandardAntennaProperty.AntennaPower,			typeof(String),				true,	false,	"Antenna Power", "Power to apply to antenna when transmitting."),
					new rfidHelperClass(rfidAntenna.StandardAntennaProperty.AntennaPowerRange,		typeof(rfidAntenna.StandardAntennaProperty),  true, true, "Antenna power range", "Range of values that can be applied to the antenna."),

				};

			StandardProperties = new rfidStandardProperties();
			foreach (rfidHelperClass item in AllStandardProperties)
			{
				StandardProperties.Add(item);
			}
		}

		public static string StandardKey(Object item)
		{
			return String.Format("{0}.{1}", item.GetType().FullName, Enum.GetName(item.GetType(), item));
		}



	} //public class rfidStandardPropertyLoader

    */

	public class rfidTag : IComparable<rfidTag>
	{
		public readonly byte[] Value;
		public rfidTag(byte[] data)
		{
			Value = (byte[])data.Clone();
		}

		#region IComparable<rfidTag> Members

		public int CompareTo(rfidTag other)
		{
			if (Value.Length != other.Value.Length)
				return Value.Length - other.Value.Length;

			if (this.Value.Length  == 0)
			{
				return other.Value.Length;
			}
			int i = this.Value.Length - 1;
			while (this.Value[i] == other.Value[i])
			{
				if (--i <  0)
					return 0;
			}
			return this.Value[i] - other.Value[i];
		}

		#endregion
	}

	
	
	public class rfidTagList : SortedDictionary<rfidTag, int>
	{

		/// <summary>
		/// 
		/// </summary>
		/// <param name="tag"></param>
		/// <returns>The updated tag count i.e. 1 to n time the tag was seen including this on</returns>
		public int AddOrIncrementTagCount(rfidTag tag)
		{
			
			if (ContainsKey(tag))
				return ++this[tag];
			
			Add(tag, 1);
			return 1;
		}
		
	}



	/// <summary>
	/// Functions as an ArrayList but with a few changes in behavior, to support the protocol/tracer control.
	/// This ArrayList will not grow beyond its starting capacity, instead the older items are deleted.
	/// Data access is always synchonized
	/// </summary>
	public class PacketArrayList : ArrayList
	{
		private const int MaxCapacity = 4 * 1024;

		private Object _DataLock			= null;
		private Type   _DataType			= null;
		public PacketArrayList(Type type)
			: base(MaxCapacity)
		{
			_DataLock = new Object();
			_DataType = type;
		}

		private PacketArrayList(ICollection c, Type type)
			: base(c)
		{
			_DataType = type; 
		}

		public virtual Type DataType
		{
			get { return _DataType; }
		}

		/// <summary>
		/// Can only set to zero to free memory.
		/// </summary>
		public override int Capacity 
		{ 
			get { return MaxCapacity; } 
			set {
				if (value == 0)
				{
					base.Capacity = 0;
				}
				else
				{
					throw new ArgumentOutOfRangeException("Capacity", "Cannot set the capacity of a PacketArrayList to any value other than zero.", String.Format("Attempted to set the capacity to {0}.", value));
				}
			} 
		}

		public override int Count
		{
			get
			{
				lock (_DataLock)
				{
					return base.Count;
				}
			}
		}

		public override bool IsFixedSize { get { return true; } }
		public override bool IsReadOnly { get { return false; } }
		public override bool IsSynchronized { get { return true; } }

		/// <summary>
		/// Not Supported.
		/// </summary>
		public override object SyncRoot { get { throw new NotSupportedException(); } }


		//		public virtual object this[int index] { get; set; }
		public new virtual PacketData.PacketWrapper this[int index]
		{
			get
			{
				lock (_DataLock)
				{
					return base[index] as PacketData.PacketWrapper;
				}
			}
		}



		/// <summary>
		/// Not Supported.
		/// </summary>
		public override int Add(object value) { throw new NotSupportedException(); }


		public int Add(PacketData.PacketWrapper envelope)
		{
			int result;

			lock (_DataLock)
			{
				while (base.Count >= MaxCapacity)
				{
					base.RemoveAt(0);
				}
				result = base.Add(envelope);
			}

			return result;
		}



		public override void AddRange(ICollection c)
		{
			lock (_DataLock)
			{
				base.AddRange(c);
			}
		}

		/// <summary>
		/// Not Supported.
		/// </summary>
		public override int BinarySearch(object value) { throw new NotSupportedException(); }

		/// <summary>
		/// Not Supported.
		/// </summary>
		public override int BinarySearch(object value, IComparer comparer) { throw new NotSupportedException(); }

		/// <summary>
		/// Not Supported.
		/// </summary>
		public override int BinarySearch(int index, int count, object value, IComparer comparer) { throw new NotSupportedException(); }


		public override void Clear()
		{
			lock (_DataLock)
			{
				base.Clear();
			}
		}

		public override object Clone()
		{
			lock (_DataLock)
			{
				return new PacketArrayList(this, _DataType);
			}
		}

		public override bool Contains(object item)
		{
			lock (_DataLock)
			{
				return base.Contains(item);
			}
		}

		public override void CopyTo(Array array)
		{
			lock (_DataLock)
			{
				base.CopyTo(array);
			}
		}

		public override void CopyTo(Array array, int arrayIndex)
		{
			lock (_DataLock)
			{
				base.CopyTo(array, arrayIndex);
			}
		}

		public override void CopyTo(int index, Array array, int arrayIndex, int count)
		{
			lock (_DataLock)
			{
				base.CopyTo(index, array, arrayIndex, count);
			}
		}

		public override IEnumerator GetEnumerator()
		{
			lock (_DataLock)
			{
				return base.GetEnumerator();
			}
		}


		public override IEnumerator GetEnumerator(int index, int count)
		{
			lock (_DataLock)
			{
				return base.GetEnumerator(index, count);
			}
		}


		public override ArrayList GetRange(int index, int count)
		{
			lock (_DataLock)
			{
				return base.GetRange(index, count);
			}
		}

		public override int IndexOf(object value)
		{
			lock (_DataLock)
			{
				return base.IndexOf(value);
			}
		}

		public override int IndexOf(object value, int startIndex)
		{
			lock (_DataLock)
			{
				return base.IndexOf(value, startIndex);
			}
		}

		/// <summary>
		/// Not Supported.
		/// </summary>
		public override void Insert(int index, object value) { throw new NotSupportedException(); }

		/// <summary>
		/// Not Supported.
		/// </summary>
		public override void InsertRange(int index, ICollection c) 
		{
			lock (_DataLock)
			{
				base.InsertRange(index, c);
			}
		}

		public override int LastIndexOf(object value)
		{
			lock (_DataLock)
			{
				return base.LastIndexOf(value);
			}
		}

		public override int LastIndexOf(object value, int startIndex)
		{
			lock (_DataLock)
			{
				return base.LastIndexOf(value, startIndex);
			}
		}

		public override int LastIndexOf(object value, int startIndex, int count)
		{
			lock (_DataLock)
			{
				return base.LastIndexOf(value, startIndex, count);
			}
		}

		/// <summary>
		/// Not Supported.
		/// </summary>
		public override void Remove(object obj) { throw new NotSupportedException(); }

		/// <summary>
		/// Not Supported.
		/// </summary>
		public override void RemoveAt(int index) { throw new NotSupportedException(); }

		/// <summary>
		/// Not Supported.
		/// </summary>
		public override void RemoveRange(int index, int count) 
		{ 
			throw new NotSupportedException(); 
		}

		/// <summary>
		/// Not Supported.
		/// </summary>
		public override void Reverse() 
		{ 
			throw new NotSupportedException(); 
		}

		/// <summary>
		/// Not Supported.
		/// </summary>
		public override void Reverse(int index, int count) 
		{ 
			throw new NotSupportedException(); 
		}

		/// <summary>
		/// Not Supported.
		/// </summary>
		public override void SetRange(int index, ICollection c) 
		{ 
			throw new NotSupportedException(); 
		}

		/// <summary>
		/// Not Supported.
		/// </summary>
		public override void Sort() 
		{ 
			throw new NotSupportedException(); 
		}

		/// <summary>
		/// Not Supported.
		/// </summary>
		public override void Sort(IComparer comparer) 
		{ 
			throw new NotSupportedException(); 
		}

		/// <summary>
		/// Not Supported.
		/// </summary>
		public override void Sort(int index, int count, IComparer comparer) 
		{ 
			throw new NotSupportedException(); 
		}


		public new PacketData.PacketWrapper[] ToArray()
		{
			lock (_DataLock)
			{
				return base.ToArray() as PacketData.PacketWrapper[];
			}
		}

		public override Array ToArray(Type type)
		{
			lock (_DataLock)
			{
				return base.ToArray(type);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public override void TrimToSize() 
		{
			lock (_DataLock)
			{
				base.TrimToSize();
			}
		}


	} //public class PacketArrayList : ArrayList


	public class PacketArrayListGlue : PacketArrayList
	{
		private IReader _reader;

		public PacketArrayListGlue(IReader reader)
			: base(reader.GetType())
		{
			_reader = reader;
		}

		public override int Count
		{
			get
			{
				int baseCount = base.Count;
				int PacketStreamCount = _reader.PacketStreamData.Count;
				if (PacketStreamCount >= baseCount && 
					_reader.TableResult == TableState.Ready)
				{
					return PacketStreamCount;
				}
				else
				{
					return baseCount;
				}
			}
		}



		public override PacketData.PacketWrapper this[int index]
		{
			get
			{
				if (_reader.TableResult == TableState.Ready)
				{
					string errorMessage;
					PacketData.PacketBase packet				= null;
					PacketStream packetData	= new PacketStream();

					packetData = _reader.PacketStreamData[index];
					PacketData.PacketWrapper envelope;
					if (packetData.IsPseudoPacket)
					{
						BadPacket badPacket 		= new BadPacket();
						badPacket.packetTime		= packetData.PacketTime;
						badPacket.rawPacketData		= packetData.RawPacketData.Clone() as byte[];
						//badPacket.errorMessage		= errorMessage;
						using (System.IO.MemoryStream data = new System.IO.MemoryStream(256))
						{
							badPacket.WriteTo(data);
							envelope = new PacketData.PacketWrapper(new PacketData.CommandPsuedoPacket("BadPacket", data.ToArray()), PacketData.PacketType.U_N_D_F_I_N_E_D);
						}
						envelope.IsPseudoPacket		= true;
						envelope.PacketNumber		= packetData.PacketSequence;
						envelope.Timestamp			= packetData.PacketTime;
						envelope.ReaderIndex		= packetData.ReaderIndex;
						envelope.ReaderName			= packetData.Name;
						envelope.CommandNumber		= packetData.RequestNumber;
						envelope.ElapsedTimeMs		= packetData.ElapsedTimeMs;

					}
					else
					{
						PacketData.PacketType type	 				= PacketData.ParsePacket(packetData.RawPacketData, out packet, out errorMessage);


						envelope = new PacketData.PacketWrapper(packet,	
																	type,
																	packetData.RawPacketData,
																	packetData.RequestNumber,
																	packetData.ElapsedTimeMs,
																	packetData.ReaderIndex.GetValueOrDefault(0),
																	packetData.Reader);
						envelope.PacketNumber = packetData.PacketSequence;
						envelope.Timestamp		= packetData.PacketTime;
						//					envelope.ReaderName		= packetData.Reader;
						//					envelope.PacketTypeName;packetStream.packetType		
						//					envelope.ElapsedTimeMs;	packetStream.elapsedTimeMs
						//					envelope.CommandNumber;	packetStream.requestNumber
						//					envelope.ReaderIndex;	packetStream.readerIndex	
						//					envelope.RawPacket;		packetStream.rawPacketData
						//	BitConverter.ToString(envelope.RawPacketpacketStream.packetData		);
					}					
					return envelope;
				}
				else
				{
					return base[index] as PacketData.PacketWrapper;
				}
			}
		}

	} // public class PacketArrayListGlue : PacketArrayList


} //namespace RFID.rfidInterface
