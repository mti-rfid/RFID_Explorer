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
 * $Id: LCPacketParser.cs,v 1.14 2011/01/05 06:22:14 dciampi Exp $
 * 
 * Description: 
 *
 *****************************************************************************
 */

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml;
using INT8U=System.Byte;
using INT16U=System.UInt16;
using INT32U=System.UInt32;
using System.Runtime.Serialization;



namespace RFID.RFIDInterface
{

	public class PacketData
	{

		public const int PACKET_VERSION			= 1;
		public const int PREAMBLE_SIZE			= 8;

		private const int PACKET_TYPE_OFFSET	= 2;
		private const int PACKET_SIZE_OFFSET	= 4;

		public enum PacketType : ushort
		{				
			U_N_D_F_I_N_E_D					= 0xffff,
			CMD_BEGIN						= 0x0000,
			CMD_END							= 0x0001,
			ANTENNA_CYCLE_BEGIN				= 0x0002,
			ANTENNA_BEGIN					= 0x0003,
			ISO18K6C_INVENTORY_ROUND_BEGIN	= 0x0004,
			ISO18K6C_INVENTORY				= 0x0005,
			ISO18K6C_TAG_ACCESS				= 0x0006,
			ANTENNA_CYCLE_END				= 0x0007,
			ANTENNA_END						= 0x0008,
			ISO18K6C_INVENTORY_ROUND_END	= 0x0009,
			INVENTORY_CYCLE_BEGIN			= 0x000A,
			INVENTORY_CYCLE_END				= 0x000B,
			CARRIER_INFO					= 0x000C,
            COMMAND_ACTIVE                  = 0x000E,
			ANTENNA_CYCLE_BEGIN_DIAG		= 0x1000,
			ANTENNA_CYCLE_END_DIAG			= 0x1001,
			ANTENNA_BEGIN_DIAG				= 0x1002,
			ANTENNA_END_DIAG				= 0x1003,
			ISO18K6C_INVENTORY_ROUND_BEGIN_DIAG	= 0x1004,
			ISO18K6C_INVENTORY_ROUND_END_DIAG	= 0x1005,
			ISO18K6C_INVENTORY_DIAG			= 0x1006,
			FREQUENCY_HOP_DIAG				= 0x1007,
			INVENTORY_CYCLE_END_DIAGS		= 0x1008,
			NONCRITICAL_FAULT				= 0x2000,
			FIRMWARE_UPDATE_COFIGURATION    = 0x300B,
            DEBUG                     = 0x4000,
		}

		[Serializable]
		public class PacketWrapper : ISerializable
		{
			private bool			_isPseudoPacket		= false;
			private int				_packetNumber		= 0;
			private DateTime		_timestamp			= DateTime.UtcNow;
			private long			_elapsedMs			= 0;
			private int?			_readerIndex		= null;
			private string			_readerName			= null;
			private PacketType?		_packetType			= null;
			private PacketBase		_packet				= null;
			private byte[]			_rawPacket			= null;
			private int				_requestNumber		= 0;
			private string			_readRequest		= null;

			public PacketWrapper()
			{ }

			public PacketWrapper(PacketWrapper envelope)
			{
				if (envelope == null)
					return;

				_isPseudoPacket		= envelope._isPseudoPacket;
				_packetNumber		= envelope._packetNumber;
				_timestamp			= envelope._timestamp;
				_elapsedMs			= envelope._elapsedMs;
				_readerIndex		= envelope._readerIndex;
				_readerName			= envelope._readerName;
				_packetType			= envelope._packetType;
				_packet				= envelope._packet;
				_rawPacket			= (byte[])(envelope._rawPacket == null ? null : envelope._rawPacket.Clone());
				_requestNumber		= envelope._requestNumber;
			}

			public PacketWrapper(PacketData.PacketBase Packet, PacketType type)
			{
				_packet = Packet;
				_packetType = type;
			}

			// Constructor for PacketCallBackFromReader
			public PacketWrapper(PacketData.PacketBase Packet, PacketType type, byte[] packetData, int requestNumber, long elapsedMs, int readerIndex, string readerName)
			{
				_packet			= Packet;
				_packetType		= type;
				_requestNumber	= requestNumber;
				_elapsedMs		= elapsedMs;
				_readerIndex	= readerIndex;
				_readerName		= readerName;
				_rawPacket		= packetData.Clone() as byte[];
				_isPseudoPacket = false;
			}


			public PacketWrapper(SerializationInfo information, StreamingContext context)
			{
				_isPseudoPacket = information.GetBoolean("isPseudoPacket");
				_packetNumber	= information.GetInt32("packetNumber");
				_requestNumber	= information.GetInt32("requestNumber");
				_timestamp		= (DateTime)information.GetValue("timestamp", typeof(DateTime));
				_readerIndex	= (int?)information.GetValue("readerIndex", typeof(int?));
				_readerName		= information.GetString("readerName");
				_packetType		= (PacketType?)information.GetValue("packetType", typeof(PacketType?));
				_elapsedMs		= information.GetInt64("elapsedMs");
				_readRequest	= information.GetString("readRequest");

				if (_isPseudoPacket)
				{
					// _packet = new CommandPsuedoPacket(information.GetString("RequestName"), (object [])information.GetValue("DataValues", typeof(object [])));
					_packet = new CommandPsuedoPacket(information.GetString("RequestName"), (byte [])information.GetValue("DataValues", typeof(byte[])));
				}
				else
				{
					Type theType;

					theType			= GetTypeFromPacketType((PacketType)_packetType);
					_packet			= (PacketBase)information.GetValue("packet", theType);
					_rawPacket		= (byte[])information.GetValue("rawPacket", typeof(byte[]));
//					_commandPacketNumber= (int?)information.GetValue("commandPacketNumber", typeof(int?));
				}
			}

			

			public int PacketNumber
			{
				get { return _packetNumber; }
				set { _packetNumber = value; }
			}

			public int CommandNumber
			{
				get { return _requestNumber; }
				set { _requestNumber = value; }
			}

			public DateTime Timestamp
			{
				get { return _timestamp; }
				set { _timestamp = value; }
			}

			public long ElapsedTimeMs
			{
				get { return _elapsedMs; }
				set { _elapsedMs = value; }
			}

			public void ResetTimestamp()
			{
				_timestamp = DateTime.UtcNow;
			}

			public byte[] RawPacket
			{
				get { return _rawPacket; }
				set { _rawPacket = value; }
			}

			public PacketData.PacketBase Packet
			{
				get { return _packet; }
				set { _packet = value; }
			}

			public PacketData.PacketType PacketType
			{
				get { return _packetType == null ? (PacketData.PacketType)(_packetType = PacketData.GetPacketType(_packet)) : (PacketData.PacketType)_packetType; }
			}

			public string PacketTypeName
			{
				get 
				{
					if (_packetType == null)				return "Unknown";

				switch (_packetType)
				{
					
					case PacketType.CMD_BEGIN:					return "Cmd Begin";
					case PacketType.CMD_END:					return "Cmd End";
					case PacketType.ANTENNA_CYCLE_BEGIN:		return "Ant Cyc Begin";
					case PacketType.ANTENNA_CYCLE_END:			return "Ant Cyc End";
					case PacketType.ANTENNA_CYCLE_BEGIN_DIAG:	return "Ant Cyc Begin Diag";
					case PacketType.ANTENNA_CYCLE_END_DIAG:		return "Ant Cyc End Diag";
					case PacketType.ANTENNA_BEGIN:				return "Ant Begin";
					case PacketType.ANTENNA_END:				return "Ant End";
					case PacketType.ANTENNA_BEGIN_DIAG:			return "Ant Diag";
					case PacketType.ANTENNA_END_DIAG:			return "Ant End Diag";
					case PacketType.ISO18K6C_INVENTORY_ROUND_BEGIN:			return "Inv Round Begin";
					case PacketType.ISO18K6C_INVENTORY_ROUND_END:			return "Inv Round End";
					case PacketType.INVENTORY_CYCLE_BEGIN:					return "Inv Cycle Begin";
					case PacketType.INVENTORY_CYCLE_END:					return "Inv Cycle End";
					case PacketType.CARRIER_INFO:							return "Carrier Info";
                    case PacketType.COMMAND_ACTIVE:                         return "Command Active";
					case PacketType.ISO18K6C_INVENTORY_ROUND_BEGIN_DIAG:	return "Round Begin Diag";
					case PacketType.ISO18K6C_INVENTORY_ROUND_END_DIAG:		return "Round End Diag";
					case PacketType.ISO18K6C_INVENTORY:			return "Inventory";
					case PacketType.ISO18K6C_INVENTORY_DIAG:	return "Inventory Diag";
					case PacketType.ISO18K6C_TAG_ACCESS:		return "Tag Access";
					case PacketType.FREQUENCY_HOP_DIAG:			return "Freq Hop Diag";
					case PacketType.INVENTORY_CYCLE_END_DIAGS:	return "Inv Cyc End Diag";
					case PacketType.NONCRITICAL_FAULT:			return "Noncritial Fault";
                    case PacketType.DEBUG:   return "Debug";

					case PacketType.U_N_D_F_I_N_E_D:
					default:									return "Unknown";
					}
				}
			
			}
			public bool IsPseudoPacket
			{
				get { return _isPseudoPacket; }
				set { _isPseudoPacket = value; }
			}

			public int? ReaderIndex
			{
				get { return _readerIndex; }
				set { _readerIndex = value; }
			}


			public string ReaderName
			{
				get { return _readerName; }
				set { _readerName = value; }
			}

			public string ReadRequest
			{
				get { return _readRequest; }
				set { _readRequest = value; }
			}

			public void RecordReadRequest(int bank, int start, int count, uint password)
			{
				_readRequest = String.Format("bank:{0} loc:0x{1}", bank, start, count, password);
			}

			#region ISerializable Members

			public void GetObjectData(SerializationInfo info, StreamingContext context)
			{
				info.AddValue("isPseudoPacket", _isPseudoPacket);
				info.AddValue("packetNumber",	_packetNumber);
				info.AddValue("requestNumber",	_requestNumber);
				info.AddValue("timestamp",		_timestamp);
				info.AddValue("readerIndex",	_readerIndex);
				info.AddValue("readerName",		_readerName);
				info.AddValue("packetType",		_packetType);
				info.AddValue("elapsedMs",		_elapsedMs);
				info.AddValue("readRequest",	_readRequest);

				if (_isPseudoPacket)
				{
					CommandPsuedoPacket pkt = this.Packet as CommandPsuedoPacket;
					info.AddValue("RequestName", pkt.RequestName);
					info.AddValue("DataValues", pkt.DataValues, pkt.DataValues.GetType());
				}
				else
				{					
					info.AddValue("packet", _packet, _packet.GetType());
					info.AddValue("rawPacket", _rawPacket);
				}

			}

			#endregion
		}




		/// <summary>
		/// The base class from which all Packet Format Layout Classes are derived.
		/// </summary>
		[Serializable]
		[StructLayout(LayoutKind.Sequential, CharSet=CharSet.Ansi)]
		public class PacketBase
		{
			// Bit=mapped data for flags field of the BegCmd packet
			public static readonly BitVector32.Section continuousMode	= BitVector32.CreateSection(0x01);

			/// Bit-mapped data for flags field of INVENTORY packet
			public static readonly BitVector32.Section crcResult		= BitVector32.CreateSection(0x01);
			// Bit 0	CRC valid flag:
			// 0 ?CRC was valid
			// 1 ?CRC was invalid.  No INVENTORY data will be included in packet.
			public enum CrcResultValues
			{
				Good=0,
				Bad=1
			}

			public static readonly BitVector32.Section reserved			= BitVector32.CreateSection(31, crcResult);
			// bit 1-5	Reserved.  Read as zero.

			public static readonly BitVector32.Section paddingBytes		= BitVector32.CreateSection(3, reserved);
			// bits 6 and 7 number of padding bytes added to end of INVENTORY data 


			/// Bit-mapped data for flags field of ISO_18000_6C Tag Access packet
			public static readonly BitVector32.Section accessErrorFlag	= BitVector32.CreateSection(0x01);
			public static readonly BitVector32.Section accessTagErrFlag	= BitVector32.CreateSection(0x01, accessErrorFlag);
			public static readonly BitVector32.Section accessAckTOFlag	= BitVector32.CreateSection(0x01, accessTagErrFlag);
			public static readonly BitVector32.Section accessCRCFlag	= BitVector32.CreateSection(0x01, accessAckTOFlag);
			public static readonly BitVector32.Section accessReserved	= BitVector32.CreateSection(0x03, accessCRCFlag);
			public static readonly BitVector32.Section accessPadding	= BitVector32.CreateSection(0x03, accessReserved);

			public enum ISO_18000_6C_ErrorFlag
			{
				AccessSucceeded = 0,
				AccessError = 1,
			}

			public enum ISO_18000_6C_TagErrorFlag
			{
				NoBackscatterError = 0,
				BackscatterError = 1,
			}

			public enum ISO_18000_6C_AckTimeoutFlag
			{
				AckReceived = 0,
				AckTimeout  = 1,
			}


			public enum ISO_18000_6C_TagAccessType
			{
				TAG_ACCESS_READ		  = 0xC2,
				TAG_ACCESS_WRITE	  = 0xC3,
				TAG_ACCESS_KILL		  = 0xC4,
				TAG_ACCESS_LOCK	      = 0xC5,
                TAG_ACCESS_ACCESS     = 0xC6,
                TAG_ACCESS_BLOCKWRITE = 0xC7,
                TAG_ACCESS_BLOCKERASE = 0xC8,
                TAG_ACCESS_QT         = 0xE0,
                TAG_ACCESS_LARGEREAD = 0xE8,//Add LargeRead command
            }


			public static string GetTagAccessTypeName(int access)
			{
				switch ((ISO_18000_6C_TagAccessType)access)
				{
					case ISO_18000_6C_TagAccessType.TAG_ACCESS_READ:
						return "read";

					case ISO_18000_6C_TagAccessType.TAG_ACCESS_WRITE:
						return "write";

					case ISO_18000_6C_TagAccessType.TAG_ACCESS_KILL:
						return "kill";

					case ISO_18000_6C_TagAccessType.TAG_ACCESS_LOCK:
						return "lock";

					case ISO_18000_6C_TagAccessType.TAG_ACCESS_ACCESS:
						return "access";

                    case ISO_18000_6C_TagAccessType.TAG_ACCESS_BLOCKWRITE:
                        return "block write";

                    case ISO_18000_6C_TagAccessType.TAG_ACCESS_BLOCKERASE:
                        return "block erase";

                    case ISO_18000_6C_TagAccessType.TAG_ACCESS_QT:
                        return "qt";

                    //Add LargeRead command
                    case ISO_18000_6C_TagAccessType.TAG_ACCESS_LARGEREAD:
                        return "LargeRead";

                    default:
						return "unknown";
			
				}
			}


			public enum ISO_18000_6C_TagErrors
			{
				GeneralError		= 0x00,
				LocationError		= 0x03,
				LockedError			= 0x04,
				PowerError			= 0x0B,
				ErrorNotSupported	= 0x0F
			}


			public static string GetTagErrorName(bool tagSentError, int errorCode)
			{
				if (tagSentError)
				{
				    switch ((ISO_18000_6C_TagErrors)errorCode)
				    {
				    	case ISO_18000_6C_TagErrors.GeneralError:
						    //return "general";
                          return "0x00";

				    	case ISO_18000_6C_TagErrors.LocationError:
					    	//return "location";
                          return "0x03";

				    	case ISO_18000_6C_TagErrors.LockedError:
				    		//return "locked";
                           return "0x04";

				    	case ISO_18000_6C_TagErrors.PowerError:
					    	//return "power";
                         return "0x0B";

				    	case ISO_18000_6C_TagErrors.ErrorNotSupported:
				    		//return "n/a";
                           return "0x0F";
                    
					    default:
					    	return String.Format("err 0x{0:x}", errorCode);
			    	}
				}
				else
				{
					return "none";
				}
			}


            public enum ISO_18000_6C_ProtErrors
            {
                None = 0,
                HandleMismatch = 1,
                BadCrc = 2,
                NoReply = 3,
                InvalidPassword = 4,
                ZeroKillPassword = 5,
                TagLost = 6,
                CmdFormat = 7,
                ReadCountInvalid = 8,
                RetryCountExceeded = 9
            }


            public static string GetProtErrorName(int errorCode)
            {
                switch ((ISO_18000_6C_ProtErrors)errorCode)
                {
                    case ISO_18000_6C_ProtErrors.None:
                        return "none";

                    case ISO_18000_6C_ProtErrors.HandleMismatch:
                        return "handle";

                    case ISO_18000_6C_ProtErrors.BadCrc:
                        return "crc";

                    case ISO_18000_6C_ProtErrors.NoReply:
                        return "no reply";

                    case ISO_18000_6C_ProtErrors.InvalidPassword:
                        return "inv pwd";

                    case ISO_18000_6C_ProtErrors.ZeroKillPassword:
                        return "zero kill pwd";

                    case ISO_18000_6C_ProtErrors.TagLost:
                        return "lost";

                    case ISO_18000_6C_ProtErrors.CmdFormat:
                        return "cmd format";

                    case ISO_18000_6C_ProtErrors.ReadCountInvalid:
                        return "read cnt";

                    case ISO_18000_6C_ProtErrors.RetryCountExceeded:
                        return "retry cnt";

                    default:
                        return String.Format("err 0x{0:x}", errorCode);
                }
            }


			// Bit-mapped data for sing_params field of inv_rnd_beg_diag packet
			public static readonly BitVector32.Section CurrentQValue	= BitVector32.CreateSection(0x0f);
			public static readonly BitVector32.Section CurrentSlot1		= BitVector32.CreateSection(0xff, CurrentQValue);
			public static readonly BitVector32.Section CurrentSlot2		= BitVector32.CreateSection(0x1ff, CurrentSlot1);
			public static readonly BitVector32.Section InventoriedFlag	= BitVector32.CreateSection(0x01, CurrentSlot2);
			public static readonly BitVector32.Section RetryCount		= BitVector32.CreateSection(0x01ff, InventoriedFlag);
			//Bit	0	Inventoried flag value: 
			
			public static int GetCurrentSlot(BitVector32 singulationParams)
			{
				return (singulationParams[CurrentSlot2] << 16) | singulationParams[CurrentSlot1];
			}

			public enum InventoriedFlagState
			{
				A = 0,	// 0 ?Singulate tags with inventoried flag set to A, 
				B = 1,	// 1 ?Singulate tags with inventoried flag set to B
			}

			public static string InventoriedFlagName(int val)
			{
				if (Enum.IsDefined(typeof(InventoriedFlagState), val))
				{
					switch ((InventoriedFlagState)val)
					{
						case InventoriedFlagState.A:
							return "A";

						case InventoriedFlagState.B:
							return "B";
					}
				}
				return "?";
			}
/*
			public static readonly BitVector32.Section Session			= BitVector32.CreateSection(0x03, InventoriedFlag);
			//2:1	Inventory session:

			public enum InventorySession
			{
				S0 = 0x00,	//00 ?Use session S0 for inventory round
				S1 = 0x01,	//01 ?Use session S1 for inventory round
				S2 = 0x02,	//10 ?Use session S2 for inventory round
				S3 = 0x03,	//11 ?Use session S3 for inventory round
			}

			public static string InventorySessionName(int val)
			{
				if (Enum.IsDefined(typeof(InventorySession), val))
				{
					switch ((InventorySession)val)
					{
						case InventorySession.S0:
							return "S0";

						case InventorySession.S1:
							return "S1";

						case InventorySession.S2:
							return "S2";

						case InventorySession.S3:
							return "S3";
					}
				}
				return "?";
			}


			public static readonly BitVector32.Section SelectedState	= BitVector32.CreateSection(0x03, Session);
			//4:3	Selected flag value:

			public enum SelectedStateValues
			{
				Either	= 0x00,	//00 ?Tags with selected flag in either state chosen for inventory round
				Set		= 0x02,	//10 ?Tags with selected flag set chosen for inventory round
				Unset	= 0x03,	//11 ?Tags with selected flag unset chosen for inventory round
			}

			public static string SelectedStateName(int val)
			{
				if (Enum.IsDefined(typeof(SelectedStateValues), val))
				{
					switch ((SelectedStateValues)val)
					{
						case SelectedStateValues.Either:
							return "Any";

						case SelectedStateValues.Set:
							return "Set";

						case SelectedStateValues.Unset:
							return "Unset";
					}
				}
				return "Unknown";
			}
			public static readonly BitVector32.Section DivideRatio		= BitVector32.CreateSection(0x01, SelectedState);
			//5	ISO 18000-6C tag-to-radio calibration divide ratio used for inventory round:

			public enum DivideRatioValues
			{
				Eight = 0,			//0 ?Divide ratio = 8
				SixtyfourOverThree = 1, //1 ?Divide ratio = 64/3
			}

			public static string DivideRatioName(int val)
			{
				if (Enum.IsDefined(typeof(DivideRatioValues), val))
				{
					switch ((DivideRatioValues)val)
					{
						case DivideRatioValues.Eight:
							return "8";

						case DivideRatioValues.SixtyfourOverThree:
							return "64/3";
					}
				}
				return "?";
			}

			public static readonly BitVector32.Section SubcarrierCycles = BitVector32.CreateSection(0x03, DivideRatio);
			//7:6	ISO 18000-6C number of subcarrier cycles per bin in Miller sequence:

			public enum SubcarrierCyclesValues
			{
				FMO = 0x00,	//00 ?1 bit (FMO)
				M2  = 0x01,	//01 ?2 bits (M=2)
				M4  = 0x02,	//10 ?4 bits (M=4)
				M8  = 0x03, //11 ?8 bits (M=8}
			}

			public static string SubcarrierCyclesNames(int val)
			{
				if (Enum.IsDefined(typeof(SubcarrierCyclesValues), val))
				{
					switch ((SubcarrierCyclesValues)val)
					{
						case SubcarrierCyclesValues.FMO:
							return "FM0";

						case SubcarrierCyclesValues.M2:
							return "M=2";

						case SubcarrierCyclesValues.M4:
							return "M=4";

						case SubcarrierCyclesValues.M8:
							return "M=8";
					}
				}
				return "?";
			}


			public static readonly BitVector32.Section PilotTone = BitVector32.CreateSection(0x01, SubcarrierCycles);
			//8	ISO 18000-6C tag-to-radio preamble flag:

			public enum PilotToneValues
			{
				NoPilotTone = 0,	 //0 ?No pilot tone prepended to tag-to-radio preamble
				PrependPilotTone = 1,	//1 ?Pilot tone prepended to tag-to-radio preamble
			}

			public static string PilotToneNames(int val)
			{
				if (Enum.IsDefined(typeof(PilotToneValues), val))
				{
					switch ((PilotToneValues)val)
					{
						case PilotToneValues.NoPilotTone:
							return "Omit";

						case PilotToneValues.PrependPilotTone:
							return "Add";
					}
				}
				return "Unknown";
			}

			public static readonly BitVector32.Section InitalQValue		= BitVector32.CreateSection(0x0f, PilotTone);
			//12:9	Initial Q value

			//15:13	Reserved.  Read as zero.
*/


			// Bit-mapped data for sing_stats field of inv_rnd_end_diag packet
			public static readonly BitVector32.Section singulationCollisions = BitVector32.CreateSection(0x7fff);
			// 15:0	Number of tag singulation collisions during inventory round.

			public static readonly BitVector32.Section hack = BitVector32.CreateSection(0x01, singulationCollisions);

			public static readonly BitVector32.Section minimumQValue = BitVector32.CreateSection(0x0f, hack);
			// 19:16	Minimum Q value during inventory round.

			public static readonly BitVector32.Section maximumQValue = BitVector32.CreateSection(0x0f, minimumQValue);
			//23:20	Maximum Q value during inventory round.

			//31:24	Reserved.  Read as zero.

			public enum commandtype
			{
				ISO_18000_6C_Inventory	  = 0x0000000F,
				ISO_18000_6C_Read		  = 0x00000010,
				ISO_18000_6C_Write		  = 0x00000011,
				ISO_18000_6C_Lock		  = 0x00000012,
				ISO_18000_6C_Kill		  = 0x00000013,
                ISO_18000_6C_BlockWrite   = 0x0000001F,
                Test_Transmit_Random_Data = 0x00000022,   //clark 2011.08.04. Add test pulse for firmware.
                //Add LargeRead command
                ISO_18000_6C_LargeRead = 0x00000034,
			}

			public static string GetCommandName(uint value)
			{
				if (Enum.IsDefined(typeof(commandtype), (int)value))
				{
					commandtype c = (commandtype)value;
					switch (c)
					{
					case commandtype.ISO_18000_6C_Inventory:
						return "Inventory";

					case commandtype.ISO_18000_6C_Read:
						return "Read";

					case commandtype.ISO_18000_6C_Write:
						return "Write";

					case commandtype.ISO_18000_6C_Lock:
						return "Lock";

					case commandtype.ISO_18000_6C_Kill:
						return "Kill";

                    case commandtype.ISO_18000_6C_BlockWrite:
                        return "Block Write";

                    case commandtype.Test_Transmit_Random_Data:
                        return "Test Pulse";

                    //Add LargeRead command
                    case commandtype.ISO_18000_6C_LargeRead:
                        return "LargeRead";

					default:
						return "Unknown";
					}
				}
				return "Unknown";
			}


			public enum ContinuousModeFlag
			{
				NotInContinuousMode=0,
				InContinuousMode=1,
			}



			public enum commandResult
			{
				success = 0,
			}

			public static string GetResultName(uint value)
			{
				if (Enum.IsDefined(typeof(commandResult), (int)value))
				{
					commandResult cs = (commandResult)value;
					switch (cs)
					{
					case commandResult.success:
						return "Success";

					default:
						return "Unknown";
					}
				}
				return String.Format("Err 0x{0:X}", value);
			}

			public enum carrierResult : int
			{
				carrier_off = 0x0000, 
				carrier_on = 0x0001,
			}

			public override string ToString()
			{
				StringBuilder sb = new StringBuilder();
				foreach (System.Reflection.FieldInfo info in this.GetType().GetFields())
				{
					if (info.FieldType.IsArray)
					{
						sb.AppendFormat("{0}: (array) ", info.Name);
					}
					else
					{
						sb.AppendFormat("{0}: {1} ", info.Name, info.GetValue(this).ToString());
					}
				}
				return sb.ToString();
			}
		}



		[Serializable]
		public class CommandPsuedoPacket : PacketBase
		{
			private string _requestName;
			//private object[] _data;
			private byte[] _data;

			//public CommandPsuedoPacket(string requestName, Object[] data)
			public CommandPsuedoPacket(string requestName, byte[] data)
			{
				_requestName = requestName;
				_data = data;
			}

			public string RequestName
			{
				get { return _requestName; }
			}

			//public object[] DataValues
			public byte[] DataValues
			{
				get { return _data; }
			}
		}



		/**************
		 * 
		 * 	Packet Format Layout Classes
		 * 
		 * ************/

		//[StructLayout(LayoutKind.Sequential, CharSet=CharSet.Ansi)]
		//public class Preamble : PacketBase
		//{
		//    public INT8U pkt_ver;						// packet specific version number
		//    public INT8U flags;						// packet specific flags 
		//    public INT16U pkt_type;					// packet type identifier 
		//    public INT16U pkt_len;					// packet length indicator - number of 32bit words that follow THIS struct 
		//    public readonly INT16U res0		= 0;	// reserved for future use 
		//}

        [Serializable]
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public class debug : PacketBase
        {
            // Common Preamble
            public readonly INT8U pkt_ver = 1;	// packet specific version number
            public INT8U flags;				// packet specific flags 
            public readonly INT16U pkt_type = (INT16U)PacketType.DEBUG;	// packet type identifier
            public INT16U pkt_len;	// packet length indicator - number of 32bit words that follow preamble 
            public readonly INT16U res0 = 0;	// reserved for future use (must be zero)
            // End of Common Preamble					

            public INT32U ms_ctr;					// current millisecond timer/counter
            public INT16U counter;
            public INT16U id;
            public INT8U[] data = null;	// variable length data

        }


		[Serializable]
		[StructLayout(LayoutKind.Sequential, CharSet=CharSet.Ansi)]
		public class cmd_beg : PacketBase
		{
			// Common Preamble
			public readonly INT8U pkt_ver		= 1;	// packet specific version number
			public			INT8U flags;				// packet specific flags 
			public readonly INT16U pkt_type		= (INT16U)PacketType.CMD_BEGIN;	// packet type identifier
			public readonly INT16U pkt_len		= 2;	// packet length indicator - number of 32bit words that follow preamble 
			public          INT16U res0			= 0;	// reserved for future use (must be zero)
			// End of Common Preamble					

			public INT32U command				= 0;	//The command which initiated the packet sequence:
														//	0x0000000F - ISO 18000-6C Inventory
														//	0x00000010 - ISO 18000-6C Read
														//	0x00000011 - ISO 18000-6C Write
														//	0x00000012 - ISO 18000-6C Lock
														//	0x00000013 - ISO 18000-6C Kill

			public INT32U ms_ctr;						// current millisecond timer/counter
		}

		[Serializable]
		[StructLayout(LayoutKind.Sequential, CharSet=CharSet.Ansi)]
		public class cmd_end : PacketBase
		{
			// Common Preamble
			public readonly INT8U pkt_ver		= 1;
			public          INT8U flags			= 0;
			public readonly INT16U pkt_type		= (INT16U)PacketType.CMD_END;
			public readonly INT16U pkt_len		= 2;
			public          INT16U res0			= 0;
			// End of Common Preamble

			public INT32U ms_ctr;						// current millisecond timer/counter
			public INT32U Result;						// command Result indicator - usually macerrno
		}


		[Serializable]
		[StructLayout(LayoutKind.Sequential, CharSet=CharSet.Ansi)]
		public class ant_cyc_beg : PacketBase
		{
			// Common Preamble
			public readonly INT8U pkt_ver	= 1;
			public readonly INT8U flags		= 0;
			public readonly INT16U pkt_type	= (INT16U)PacketType.ANTENNA_CYCLE_BEGIN;
			public readonly INT16U pkt_len	= 0;
			public readonly INT16U res0		= 0;
			// End of Common Preamble

			/* no other packet specific fields */
		}



		[Serializable]
		[StructLayout(LayoutKind.Sequential, CharSet=CharSet.Ansi)]
		public class ant_cyc_end : PacketBase
		{
			// Common Preamble
			public readonly INT8U pkt_ver	= 1;
			public readonly INT8U flags		= 0;
			public readonly INT16U pkt_type	= (INT16U)PacketType.ANTENNA_CYCLE_END;
			public readonly INT16U pkt_len	= 0;
			public readonly INT16U res0		= 0;
			// End of Common Preamble

			/* no other packet specific fields */
		}

		[Serializable]
		[StructLayout(LayoutKind.Sequential, CharSet=CharSet.Ansi)]
		public class ant_cyc_beg_diag : PacketBase
		{
			// Common Preamble
			public readonly INT8U pkt_ver	= 1;
			public readonly INT8U flags		= 0;
			public readonly INT16U pkt_type	= (INT16U)PacketType.ANTENNA_CYCLE_BEGIN_DIAG;
			public readonly INT16U pkt_len	= 1;
			public readonly INT16U res0 = 0;
			// End of Common Preamble

			public INT32U ms_ctr;						// current millisecond timer/counter
		}


		[Serializable]
		[StructLayout(LayoutKind.Sequential, CharSet=CharSet.Ansi)]
		public class ant_cyc_end_diag : PacketBase
		{
			// Common Preamble
			public readonly INT8U pkt_ver	= 1;
			public readonly INT8U flags		= 0;
			public readonly INT16U pkt_type	= (INT16U)PacketType.ANTENNA_CYCLE_END_DIAG;
			public readonly INT16U pkt_len	= 1;
			public readonly INT16U res0		= 0;
			// End of Common Preamble

			public INT32U ms_ctr;						// current millisecond timer/counter
		}

		[Serializable]
		[StructLayout(LayoutKind.Sequential, CharSet=CharSet.Ansi)]
		public class ant_beg : PacketBase
		{
			// Common Preamble
			public readonly INT8U pkt_ver	= 1;
			public readonly INT8U flags		= 0;
			public readonly INT16U pkt_type	= (INT16U)PacketType.ANTENNA_BEGIN;
			public readonly INT16U pkt_len	= 1;
			public readonly INT16U res0		= 0;
			// End of Common Preamble

			public INT32U antenna;						// antenna number
		}

		[Serializable]
		[StructLayout(LayoutKind.Sequential, CharSet=CharSet.Ansi)]
		public class ant_end : PacketBase
		{
			// Common Preamble
			public readonly INT8U pkt_ver	= 1;
			public readonly INT8U flags		= 0;
			public readonly INT16U pkt_type	= (INT16U)PacketType.ANTENNA_END;
			public readonly INT16U pkt_len	= 0;
			public readonly INT16U res0		= 0;
			// End of Common Preamble

			/* no other packet specific fields */
		}


		[Serializable]
		[StructLayout(LayoutKind.Sequential, CharSet=CharSet.Ansi)]
		public class ant_beg_diag : PacketBase
		{
			// Common Preamble
			public readonly INT8U pkt_ver	= 1;
			public readonly	INT8U flags		= 0;
			public readonly INT16U pkt_type	= (INT16U)PacketType.ANTENNA_BEGIN_DIAG;
			public readonly INT16U pkt_len	= 2;
			public readonly INT16U res0		= 0;
			// End of Common Preamble
			public INT32U ms_ctr;						// current millisecond timer/counter
			public INT32U sense_res;					// The antenna sense resistor value for the logical antenna’s physical transmit antenna port.  TBD ?units of measure
		}

		[Serializable]
		[StructLayout(LayoutKind.Sequential, CharSet=CharSet.Ansi)]
		public class ant_end_diag : PacketBase
		{
			// Common Preamble
			public readonly INT8U pkt_ver	= 1;
			public readonly INT8U flags		= 0;
			public readonly INT16U pkt_type	= (INT16U)PacketType.ANTENNA_END_DIAG;
			public readonly INT16U pkt_len	= 1;
			public readonly INT16U res0		= 0;
			// End of Common Preamble

			public INT32U ms_ctr;						// current millisecond timer/counter

		}


		[Serializable]
		[StructLayout(LayoutKind.Sequential, CharSet=CharSet.Ansi)]
		public class inv_rnd_beg : PacketBase
		{
			// Common Preamble
			public readonly INT8U pkt_ver	= 1;
			public readonly INT8U flags		= 0;
			public readonly INT16U pkt_type	= (INT16U)PacketType.ISO18K6C_INVENTORY_ROUND_BEGIN;
			public readonly INT16U pkt_len	= 0;
			public			INT16U res0		= 0;
			// End of Common Preamble

			// no other packet specific fields
		}

		[Serializable]
		[StructLayout(LayoutKind.Sequential, CharSet=CharSet.Ansi)]
		public class inv_rnd_end : PacketBase
		{
			// Common Preamble
			public readonly INT8U pkt_ver	= 1;
			public readonly INT8U flags		= 0;
			public readonly INT16U pkt_type	= (INT16U)PacketType.ISO18K6C_INVENTORY_ROUND_END;
			public readonly INT16U pkt_len	= 0;
			public			INT16U res0		= 0;
			// End of Common Preamble

			// no other packet specific fields
		}

		[Serializable]
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
		public class inv_cyc_beg : PacketBase
		{
			// Common Preamble
			public readonly INT8U pkt_ver = 1;
			public readonly INT8U flags = 0;
			public readonly INT16U pkt_type = (INT16U)PacketType.INVENTORY_CYCLE_BEGIN;
			public readonly INT16U pkt_len = 1;
			public readonly INT16U res0 = 0;
			// End of Common Preamble

			public INT32U ms_ctr;						// current millisecond timer/counter
		}

		[Serializable]
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
		public class inv_cyc_end : PacketBase
		{
			// Common Preamble
			public readonly INT8U pkt_ver = 1;
			public readonly INT8U flags = 0;
			public readonly INT16U pkt_type = (INT16U)PacketType.INVENTORY_CYCLE_END;
			public readonly INT16U pkt_len = 1;
			public readonly INT16U res0 = 0;
			// End of Common Preamble

			public INT32U ms_ctr;						// current millisecond timer/counter
		}


		[Serializable]
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
		public class carrier_info : PacketBase
		{
			// Common Preamble
			public readonly INT8U pkt_ver = 1;
			public readonly INT8U flags = 0;
			public readonly INT16U pkt_type = (INT16U)PacketType.CARRIER_INFO;
			public readonly INT16U pkt_len = 3;
			public readonly INT16U res0 = 0;
			// End of Common Preamble

			public INT32U ms_ctr;				// current millisecond timer/counter
			public INT32U plldivmult;			// current plldivmult setting
			public INT16U chan;					// channel
			public INT16U cw_flags;				// carrier flags
		}

        [Serializable]
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public class command_active : PacketBase
        {
            // Common Preamble
            public readonly INT8U pkt_ver = 1;
            public readonly INT8U flags = 0;
            public readonly INT16U pkt_type = (INT16U)PacketType.COMMAND_ACTIVE;
            public readonly INT16U pkt_len = 1;
            public readonly INT16U res0 = 0;
            // End of Common Preamble

            public INT32U ms_ctr;				// current millisecond timer/counter
        }



		[Serializable]
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
		public class inv_cyc_end_diag : PacketBase
		{
			// Common Preamble
			public readonly INT8U pkt_ver   = 1;
			public readonly INT8U flags     = 0;
			public readonly INT16U pkt_type = (INT16U)PacketType.INVENTORY_CYCLE_END_DIAGS;
			public readonly INT16U pkt_len  = 6;
			public			INT16U res0     = 0;
			// End of Common Preamble

			public INT32U querys;					// number of query's issued
			public INT32U rn16rcv;					// number of RN16's received
			public INT32U rn16to;					// rn16 timeouts (empty slots)
			public INT32U epcto;					// epc timeouts (No response to rn16 Ack)
			public INT32U good_reads;				// number of good EPC reads 
			public INT32U crc_failures;				// Number of CRC failures                       

		}



		[Serializable]
		[StructLayout(LayoutKind.Sequential, CharSet=CharSet.Ansi)]
		public class inv_rnd_beg_diag : PacketBase
		{
			// Common Preamble
			public readonly INT8U pkt_ver	= 1;
			public readonly INT8U flags		= 0;
			public readonly INT16U pkt_type	= (INT16U)PacketType.ISO18K6C_INVENTORY_ROUND_BEGIN_DIAG;
			public readonly INT16U pkt_len	= 2;
			public readonly INT16U res0		= 0;
			// End of Common Preamble

			public INT32U ms_ctr;					// current millisecond timer/counter
			public INT32U sing_params;				// Starting singulation parameters
		}


		[Serializable]
		[StructLayout(LayoutKind.Sequential, CharSet=CharSet.Ansi)]
		public class inv_rnd_end_diag : PacketBase
		{
			// Common Preamble
			public readonly INT8U pkt_ver	= 1;
			public readonly INT8U flags		= 0;
			public readonly INT16U pkt_type	= (INT16U)PacketType.ISO18K6C_INVENTORY_ROUND_END_DIAG;
			public readonly INT16U pkt_len	= 7;
			public			INT16U res0		= 0;
			// End of Common Preamble
			public INT32U ms_ctr;					// current millisecond timer/counter
			public INT32U querys;					// number of query's issued
			public INT32U rn16rcv;					// number of RN16's received
			public INT32U rn16to;					// rn16 timeouts (empty slots)
			public INT32U epcto;					// epc timeouts (No response to rn16 Ack)
			public INT32U good_reads;				// number of good EPC reads 
			public INT32U crc_failures;				// Number of CRC failures                       
		}


		[Serializable]
		[StructLayout(LayoutKind.Sequential, CharSet=CharSet.Ansi)]
		public class inventory : PacketBase
		{
			// Common Preamble
			public readonly INT8U  pkt_ver	= 1;
			public			INT8U  flags;
			public readonly INT16U pkt_type	= (INT16U)PacketType.ISO18K6C_INVENTORY;
			public			INT16U pkt_len;
			public			INT16U res		= 0;
			// End of Common Preamble

			public INT32U  ms_ctr;					// current millisecond timer/counter
            public INT8U   nb_rssi;					// Narrow-Band Receive Signal Strength Indicator - backscattered tab signal amplitude.
            public INT8U   wb_rssi;					// Wide-Band Receive Signal Strength Indicator - backscattered tab signal amplitude.
            public INT16U  ana_ctrl1;               // 
            public INT16U  rssi           = 0;      // RSSI. Tenths of dBm.
            public INT16U  logic_ant      = 0;      // Inventory form which Logic Port is.
            public INT8U[] inventory_data = null;	// variable length data
		}


		[Serializable]
		[StructLayout(LayoutKind.Sequential, CharSet=CharSet.Ansi)]
		public class inventory_diag : PacketBase
		{
			// Common Preamble
			public readonly INT8U pkt_ver	= 1;
			public readonly INT8U flags		= 0;
			public readonly INT16U pkt_type	= (INT16U)PacketType.ISO18K6C_INVENTORY_DIAG;
			public readonly INT16U pkt_len	= 1;
			public readonly INT16U res0		= 0;
			// End of Common Preamble

			public INT32U prot_parms;				// Protocol parameters 
		}


		[Serializable]
		[StructLayout(LayoutKind.Sequential, CharSet=CharSet.Ansi)]
		public class Iso18k6c_access : PacketBase
		{
			// Common Preamble
			public readonly INT8U pkt_ver	= 1;
			public INT8U flags;
			public readonly INT16U pkt_type		= (INT16U)PacketType.ISO18K6C_TAG_ACCESS;
			public			INT16U pkt_len;
			public          INT16U res0;
			// End of Common Preamble
			public INT32U ms_ctr;				// current millisecond timer/counter
			public INT8U command;				// ISO 18000-6C access command (0xC2 ?Read, 0xC3 ?Write, 0xC4 ?Kill, 0xC5 ?Lock, 0xC6 ?Erase)
			public INT8U tag_error_code;		// Error backscattered by tag
            public INT16U prot_error_type;		// Protocol attributed error code from tag access                                
            public INT16U write_word_count;     // The number of words successfully written; Write and BlockWrite only
            public INT16U res1;                 // reserved
            public INT8U[] data = null;		    // variable length data
		}


		[Serializable]
		[StructLayout(LayoutKind.Sequential, CharSet=CharSet.Ansi)]
		public class freq_hop_diag : PacketBase
		{
			// Common Preamble
			public readonly INT8U pkt_ver	= 1;
			public readonly INT8U flags		= 0;
			public readonly INT16U pkt_type	= (INT16U)PacketType.FREQUENCY_HOP_DIAG;
			public readonly INT16U pkt_len	= 6;
			public readonly INT16U res0		= 0;
			// End of Common Preamble

			public INT32U ms_ctr;				// current millisecond timer/counter 
			public INT16U channel;				// The channel index for the new frequency.
			public INT16U lbt_collisions;		// The number of Listen Before Talk (LBT) collisions encountered 
												// before either was able to use channel or determine channel was unusable.
			public INT32U pa_tx_pwr;			// PA transmit channel forward power measurement.  Value is expressed in 0.1 (i.e., 1/10th) dBm.
			public INT32U pa_rv_pwr;			// PA transmit channel reverse power measurement.  Value is expressed in 0.1 (i.e., 1/10th) dBm.
			public INT16U pa_temp;				// PA temperature.  Value is expressed in °C.
			public INT16U amb_temp;				// Ambient temperature.  Value is expressed in °C.
			public INT16U xcvr_temp;			// Transceiver temperature.  Value is expressed in °C.
			public INT16U reserved1;				//	Reserved.  Read as zero.
		}

		[Serializable]
		[StructLayout(LayoutKind.Sequential, CharSet=CharSet.Ansi)]
		public class noncrit_fault : PacketBase
		{
			// Common Preamble
			public readonly INT8U pkt_ver	= 1;
			public readonly INT8U flags		= 0;
			public readonly INT16U pkt_type	= (INT16U)PacketType.NONCRITICAL_FAULT;
			public readonly INT16U pkt_len	= 3;
			public readonly INT16U res0		= 0;
			// End of Common Preamble
			public INT32U ms_ctr;				// current millisecond timer/counter
			public INT16U fault_type;			// Fault type 
			public INT16U fault_subtype;		// Fault subtype 
			public INT32U context;				// context specific data for fault 

		}

		//01 - 01 - 05-00 -03-00-DC-65-C3-3B-02-00-10-00-00-00-00-00-EB-6B

		[Serializable]
		[StructLayout(LayoutKind.Sequential, CharSet=CharSet.Ansi)]
		public class unknown : PacketBase
		{
			// Common Preamble
			public			INT8U pkt_ver	= 1;
			public			INT8U flags		= 0;
			public			INT16U pkt_type	= 0;
			public			INT16U pkt_len	= 3;
			public			INT16U res0		= 0;
			// End of Common Preamble
			public INT8U[] data			= null;		// variable length data
		}




		/**********
		* 
		* PacketBase Public Static Methods
		* 
		* ********/
		public static INT16U pkt_len(int totalBytes)
		{
			return (INT16U)((totalBytes - PREAMBLE_SIZE) / 4);
		}
		/// <summary>
		/// Caculate the size of a packet (including preamble) in bytes
		/// </summary>
		/// <param name="pkt_len">value from pkt_len field of preamble</param>
		/// <returns>Packet size in bytes</returns>
		public static int FullPacketBytes(uint pkt_len)
		{
			return PREAMBLE_SIZE + (4 * (int)pkt_len);
		}

		public static int FullPacketBytes(Byte[] Buffer, int PacketStartOffset)
		{
			return FullPacketBytes(BitConverter.ToUInt16(Buffer, PacketStartOffset + PACKET_SIZE_OFFSET));
		}

		public static PacketType GetPacketTypeFromBuffer(byte[] buffer, int PacketStartOffset)
		{
			return PacketData.GetSafePacketType(BitConverter.ToUInt16(buffer, PacketStartOffset + PACKET_TYPE_OFFSET));
		}



		/*************
		 * 
		 * PacketBase Methods
		 * 
		 * ***********/

		/// <summary>
		/// XmlSerialize
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="stream"></param>
		/// <returns>The number of bytes written to the stream</returns>
		public virtual XmlElement AppendToNode(XmlElement elm)
		{
			foreach (System.Reflection.FieldInfo info in this.GetType().GetFields())
			{
				//					System.Diagnostics.Debug.WriteLine("ToXml:: " + 
				//						info.FieldType.Name + ": " + info.Name + " = " + info.GetValue(this).ToString() , "TRACE");

				if (info.FieldType.IsArray)
				{
					elm.SetAttribute(info.Name, BitConverter.ToString((byte[])info.GetValue(this), 0));
				}
				else
				{
					elm.SetAttribute(info.Name, info.GetValue(this).ToString());
				}
			}
			return elm;
		}


		public static Type GetTypeFromPacketType(PacketType packetType)
		{
			switch (packetType)
			{

			case PacketType.CMD_BEGIN:
				return typeof(cmd_beg);

			case PacketType.CMD_END:
				return typeof(cmd_end);

//			case PacketType.ENGTSTPAT_TYPE_ZZS:
//				return typeof(engtstpatZZS);

//			case PacketType.ENGTSTPAT_TYPE_FFS:
//				return typeof(engtstpatFFS);

//			case PacketType.ENGTSTPAT_TYPE_W1S:
//				return typeof(engtstpatW1S);

//			case PacketType.ENGTSTPAT_TYPE_W0S:
//				return typeof(engtstpatW0S);

//			case PacketType.ENGTSTPAT_TYPE_BND:
//				return typeof(engtstpatBND);

			case PacketType.ANTENNA_CYCLE_BEGIN:
				return typeof(ant_cyc_beg);

			case PacketType.ANTENNA_CYCLE_END:
				return typeof(ant_cyc_end);

			case PacketType.ANTENNA_CYCLE_BEGIN_DIAG:
				return typeof(ant_cyc_beg_diag);

			case PacketType.ANTENNA_CYCLE_END_DIAG:
				return typeof(ant_cyc_end_diag);

			case PacketType.ANTENNA_BEGIN:
				return typeof(ant_beg);

			case PacketType.ANTENNA_END:
				return typeof(ant_end);

			case PacketType.ANTENNA_BEGIN_DIAG:
				return typeof(ant_beg_diag);

			case PacketType.ANTENNA_END_DIAG:
				return typeof(ant_end_diag);

			case PacketType.ISO18K6C_INVENTORY_ROUND_BEGIN:
				return typeof(inv_rnd_beg);

			case PacketType.ISO18K6C_INVENTORY_ROUND_END:
				return typeof(inv_rnd_end);

			case PacketType.INVENTORY_CYCLE_BEGIN:
				return typeof(inv_cyc_beg);

			case PacketType.INVENTORY_CYCLE_END:
				return typeof(inv_cyc_end);

			case PacketType.CARRIER_INFO:
				return typeof(carrier_info);

            case PacketType.COMMAND_ACTIVE:
                return typeof(command_active);

			case PacketType.INVENTORY_CYCLE_END_DIAGS:
				return typeof(inv_cyc_end_diag);

			case PacketType.ISO18K6C_INVENTORY_ROUND_BEGIN_DIAG:
				return typeof(inv_rnd_beg_diag);

			case PacketType.ISO18K6C_INVENTORY_ROUND_END_DIAG:
				return typeof(inv_rnd_end_diag);

			case PacketType.ISO18K6C_INVENTORY:
				return typeof(inventory);

			case PacketType.ISO18K6C_INVENTORY_DIAG:
				return typeof(inventory_diag);

			case PacketType.ISO18K6C_TAG_ACCESS:
				return typeof(Iso18k6c_access);

			case PacketType.FREQUENCY_HOP_DIAG:
				return typeof(freq_hop_diag);

			case PacketType.NONCRITICAL_FAULT:
				return typeof(noncrit_fault);

            case PacketType.DEBUG:
                return typeof(debug);

			case PacketType.U_N_D_F_I_N_E_D:
			default:
				return typeof(unknown);
			}

		}

		/// <summary>
		/// Returns an uninitialized instance of the indicated packetType.
		/// </summary>
		/// <param name="packetType">Packet type to create.</param>
		/// <returns></returns>
		public static PacketBase CreatePacket(PacketType packetType)
		{
			switch (packetType)
			{

				case PacketType.CMD_BEGIN:
					return new cmd_beg();

				case PacketType.CMD_END:
					return new cmd_end();

				//			case PacketType.ENGTSTPAT_TYPE_ZZS:
				//				return new engtstpatZZS();

				//			case PacketType.ENGTSTPAT_TYPE_FFS:
				//				return new engtstpatFFS();

				//			case PacketType.ENGTSTPAT_TYPE_W1S:
				//				return new engtstpatW1S();

				//			case PacketType.ENGTSTPAT_TYPE_W0S:
				//				return new engtstpatW0S();

				//			case PacketType.ENGTSTPAT_TYPE_BND:
				//				return new engtstpatBND();

				case PacketType.ANTENNA_CYCLE_BEGIN:
					return new ant_cyc_beg();

				case PacketType.ANTENNA_CYCLE_END:
					return new ant_cyc_end();

				case PacketType.ANTENNA_CYCLE_BEGIN_DIAG:
					return new ant_cyc_beg_diag();

				case PacketType.ANTENNA_CYCLE_END_DIAG:
					return new ant_cyc_end_diag();

				case PacketType.ANTENNA_BEGIN:
					return new ant_beg();

				case PacketType.ANTENNA_END:
					return new ant_end();

				case PacketType.ANTENNA_BEGIN_DIAG:
					return new ant_beg_diag();

				case PacketType.ANTENNA_END_DIAG:
					return new ant_end_diag();

				case PacketType.ISO18K6C_INVENTORY_ROUND_BEGIN:
					return new inv_rnd_beg();

				case PacketType.ISO18K6C_INVENTORY_ROUND_END:
					return new inv_rnd_end();

				case PacketType.ISO18K6C_INVENTORY_ROUND_BEGIN_DIAG:
					return new inv_rnd_beg_diag();

				case PacketType.ISO18K6C_INVENTORY_ROUND_END_DIAG:
					return new inv_rnd_end_diag();

				case PacketType.INVENTORY_CYCLE_BEGIN:
					return new inv_cyc_beg();

				case PacketType.INVENTORY_CYCLE_END:
					return new inv_cyc_end();

				case PacketType.CARRIER_INFO:
					return new carrier_info();

                case PacketType.COMMAND_ACTIVE:
                    return new command_active();

				case PacketType.INVENTORY_CYCLE_END_DIAGS:
					return new inv_cyc_end_diag();

				case PacketType.ISO18K6C_INVENTORY:
					return new inventory();

				case PacketType.ISO18K6C_INVENTORY_DIAG:
					return new inventory_diag();

				case PacketType.ISO18K6C_TAG_ACCESS:
					return new Iso18k6c_access();

				case PacketType.FREQUENCY_HOP_DIAG:
					return new freq_hop_diag();

				case PacketType.NONCRITICAL_FAULT:
					return new noncrit_fault();

                case PacketType.DEBUG:
                    return new debug();

				case PacketType.U_N_D_F_I_N_E_D:
				default:
					return new unknown();
			}
		}	//public static PacketBase CreatePacket(packetType packetType)


		public static PacketType GetSafePacketType(UInt16 pkt_type)
		{
			return Enum.IsDefined(typeof(PacketType), pkt_type) ? (PacketType)pkt_type :PacketType.U_N_D_F_I_N_E_D;
		}


		/// <summary>
		/// Returns the packetType for a instance of one of the packet layouts classes.
		/// </summary>
		/// <param name="packet">The packet instance</param>
		/// <returns>Returns the Packet Type or return packetType.U_N_D_F_I_N_E_D if the packet is unknown or  has no type.</returns>
		public static PacketType GetPacketType(PacketData.PacketBase packet)
		{
			if (packet == null) 
			{
				System.Diagnostics.Debug.Assert(false, "null packet");
				throw new ArgumentNullException("packet", "Null packet pass to RFIDInterface.PacketData.GetPacketType()");
			}
			switch (packet.GetType().Name)
			{
			case "Preamble":
				return PacketType.U_N_D_F_I_N_E_D;

			case "cmd_beg":
				return PacketType.CMD_BEGIN;

			case "cmd_end":
				return PacketType.CMD_END;

            case "cmd_active":
                return PacketType.COMMAND_ACTIVE;

			case "ant_cyc_beg":
				return PacketType.ANTENNA_CYCLE_BEGIN;

			case "ant_cyc_end":
				return PacketType.ANTENNA_CYCLE_END;

			case "ant_cyc_beg_diag":
				return PacketType.ANTENNA_CYCLE_BEGIN_DIAG;

			case "ant_cyc_end_diag":
				return PacketType.ANTENNA_CYCLE_END_DIAG;

			case "ant_beg":
				return PacketType.ANTENNA_BEGIN;

			case "ant_end":
				return PacketType.ANTENNA_END;

			case "ant_beg_diag":
				return PacketType.ANTENNA_BEGIN_DIAG;

			case "ant_end_diag":
				return PacketType.ANTENNA_END_DIAG;

			case "inv_rnd_beg":
				return PacketType.ISO18K6C_INVENTORY_ROUND_BEGIN;

			case "inv_rnd_end":
				return PacketType.ISO18K6C_INVENTORY_ROUND_END;

			case "inv_rnd_beg_diag":
				return PacketType.ISO18K6C_INVENTORY_ROUND_BEGIN_DIAG;

			case "inv_rnd_end_diag":
				return PacketType.ISO18K6C_INVENTORY_ROUND_END_DIAG;

			case "inventory":
				return PacketType.ISO18K6C_INVENTORY;

			case "inventory_diag":
				return PacketType.ISO18K6C_INVENTORY_DIAG;

			case "Iso18k6c_access":
				return PacketType.ISO18K6C_TAG_ACCESS;

			case "freq_hop_diag":
				return PacketType.FREQUENCY_HOP_DIAG;

            case "noncrit_fault":
                return PacketType.NONCRITICAL_FAULT;

            case "debug":
                return PacketType.DEBUG;

            default:
				return PacketType.U_N_D_F_I_N_E_D;
			}
		}



		/// <summary>
		/// Parses a buffer that hold a packet data and updates the packet structure with data from the buffer.
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="buffer"></param>
		/// <param name="offset"></param>
		/// <returns>New offset after parsing the buffer</returns>
		/// 01-01-05-00-03-00-DC-65-C3-3B-02-00-10-00-00-00-00-00-EB-6B

		private static int Parse_Fields(PacketBase obj, byte[] buffer, int offset, int size)
		{
			int offsetLimit = offset + size;

			if (offsetLimit > buffer.Length)
				throw new rfidInvalidPacketException(rfidErrorCode.PacketDataTooSmall, buffer, 0);

			foreach (System.Reflection.FieldInfo info in obj.GetType().GetFields())
			{
				#region Parse Tracing code
#if false
				if (info.FieldType.IsArray)
				{
					System.Diagnostics.Debug.WriteLine(String.Format("ParseMessage:: {0}: {1}", info.FieldType.Name, info.Name), "TRACE");
				}
				else
				{
					System.Diagnostics.Debug.WriteLine(String.Format("ParseMessage:: {0} {1}: {2} = {3}", info.IsInitOnly ? "RO" : "", info.FieldType.Name, info.Name, info.GetValue(obj).ToString()), "TRACE");
				}
#endif
				#endregion

				switch (info.FieldType.Name)
				{

				case "Byte[]":
					int arraySize  = 0;
					if (info.GetValue(obj) != null) // This is for fixed length byte arrays (none right now)
					{
						arraySize = ((byte[])info.GetValue(obj)).Length;
						if (offset + arraySize > offsetLimit)
							return -1;
						byte[] newArray = new byte[arraySize];
						for (int i = 0; i < arraySize; i++)
							newArray[i] = buffer[offset+i];

						// Reverse the array if it has tag data
						if (info.Name == "inventory_data")
							Array.Reverse(newArray);

						info.SetValue(obj, newArray);
					}
					else
					{
						// Variable length array (assume the rest of the packet is the data)
						arraySize  = buffer.Length - offset;
						byte[] newArray = new byte[arraySize];
						for (int i = 0; i < arraySize; i++)
							newArray[i] = buffer[offset+i];

						info.SetValue(obj, newArray);
					}
					offset += arraySize;
					//				binaryWriter.Write((byte[])info.GetValue(obj), 0, arraySize);
					break;

				case "Byte":
					if (offset + 1 > offsetLimit)
						return -1;

					if (info.IsInitOnly)
					{
						if ((byte)info.GetValue(obj) != (byte)buffer[offset])
							throw new rfidInvalidPacketFieldException(info.Name, info.GetValue(obj).ToString(), buffer[offset].ToString());
					}
					else
					{
						info.SetValue(obj, buffer[offset]);
					}
					offset += 1;
					break;

				case "UInt16":
					if (offset + 2 > offsetLimit)
						return -1;

					if (info.IsInitOnly)
					{
						if ((UInt16)info.GetValue(obj) != Parse_UInt16(buffer, offset))
							throw new rfidInvalidPacketFieldException(info.Name, info.GetValue(obj).ToString(), Parse_UInt16(buffer, offset).ToString());
					}
					else
					{
						info.SetValue(obj, Parse_UInt16(buffer, offset));
					}
					offset += 2;
					break;

				case "UInt32":
					if (offset + 4 > offsetLimit)
						return -1;

					if (info.IsInitOnly)
					{
						if ((UInt32)info.GetValue(obj) != Parse_UInt32(buffer, offset))
							throw new rfidInvalidPacketFieldException(info.Name, info.GetValue(obj).ToString(), Parse_UInt32(buffer, offset).ToString());
					}
					else
					{
						info.SetValue(obj, Parse_UInt32(buffer, offset));
					}
					offset += 4;
					break;

				case "String":
					int strlen = Find_String_Length(obj, info);
					if (offset + 12 > offsetLimit)
						return -1;
					info.SetValue(obj, Parse_String(buffer, offset, strlen));
					offset += strlen;
					break;

				//					case "Char[]":
				//						binaryWriter.Write((char[])info.GetValue(obj));
				//						break;

				default:
					throw new ApplicationException("Parse_Fields Error " + info.Name + "(" + info.FieldType.Name + ") is an unsupported type.");
				}
			}
			return offset;
		}


		/// <summary>
		/// ParsePacket
		/// </summary>
		/// <param name="buffer">packet data to be parsed.</param>
		/// <param name="header">output parameter to hold the message header.</param>
		/// <param name="errorMessage">output parameter to hold any error message.</param>
		/// <returns>The packet type of the packet. 
		/// </returns>
		public static PacketType ParsePacket(byte[] buffer, out PacketBase packet, out string errorMessage)
		{
			// Initalize output paramters
			packet = null;
			errorMessage = null;

			// Check parameters
			if (buffer == null)
			{
				errorMessage = SysLogger.LogError(new ArgumentNullException("buffer", "Attempted to parse an empty packet buffer."));
				return PacketType.U_N_D_F_I_N_E_D;
			}

			if (buffer.Length < PREAMBLE_SIZE)
			{
				errorMessage = SysLogger.LogError(new rfidInvalidPacketException(rfidErrorCode.PacketDataTooSmall, buffer, 0));
				return PacketType.U_N_D_F_I_N_E_D;
			}


			// Initialize locals

			PacketType rslt = GetPacketTypeFromBuffer(buffer, 0);
			if (rslt == PacketType.U_N_D_F_I_N_E_D)
			{
				errorMessage = SysLogger.LogError(new rfidInvalidPacketException(rfidErrorCode.UnknownPacketType, buffer, 0));
				return PacketType.U_N_D_F_I_N_E_D;
			}


			packet = PacketData.CreatePacket(rslt);
			try
			{
				Parse_Fields(packet, buffer, 0, FullPacketBytes(buffer, 0));

			} catch (rfidInvalidPacketException exp)
			{
				errorMessage =SysLogger.LogError(exp);
				packet = null;
				return PacketType.U_N_D_F_I_N_E_D;
			}
			return rslt;
		}


		private static UInt16 Parse_UInt16(byte[] buffer, int offset)
		{
			UInt16 result = BitConverter.ToUInt16(buffer, offset);
			//			result = (ushort)System.Net.IPAddress.NetworkToHostOrder((short)result);
			return result;
		}

		private static UInt32 Parse_UInt32(byte[] buffer, int offset)
		{
			UInt32 result = BitConverter.ToUInt32(buffer, offset);
			//			result = (uint)System.Net.IPAddress.NetworkToHostOrder((int)result);
			return result;
		}


		private static int Find_String_Length(PacketBase body, System.Reflection.FieldInfo info)
		{
			switch (info.DeclaringType.Name)
			{
			case "rfidError":
				switch (info.Name)
				{
				//				case "reason":
				//					return ((Greenwater.rfidError)body).reasonlen;

				default:
					throw new rfidInvalidPacketException(String.Format("Cannot find the string length of {0}.{1}.", info.DeclaringType.Name, info.Name));
				}

			default:
				throw new rfidInvalidPacketException(String.Format("Cannot find the length of strings in {0}.", info.DeclaringType.Name));
			}
		}



		private static string Parse_String(byte[] buffer, int offset, int strlen)
		{
			char[] asciiChars = new char[Encoding.ASCII.GetCharCount(buffer, offset, strlen)];
			Encoding.ASCII.GetChars(buffer, offset, strlen, asciiChars, 0);
			return new string(asciiChars);
		}

	}

	public class RegisterData
	{
		public const ushort MAC_REG_MACVER		= 0x0000;
		public const ushort MAC_REG_MACINFO		= 0x0001;
		public const ushort MAC_REG_RFTRANSINFO = 0x0002;

        public const ushort MAC_REG_ERROR       = 0x0005;
        public const ushort MAC_REG_LAST_ERROR  = 0x0006;
        public const ushort HST_18K6C_QUERY_CFG = 0x0900;
		public const ushort HST_INV_CFG			= 0x0901;
		public const ushort HST_INV_SEL			= 0x0902;
		public const ushort HST_INV_ALG_PARM_0	= 0x0903;
		public const ushort HST_INV_ALG_PARM_1	= 0x0904;
		public const ushort HST_INV_ALG_PARM_2	= 0x0905;
		public const ushort HST_INV_ALG_PARM_3	= 0x0906;
        public const ushort HST_INV_EPC_MATCH_CFG = 0x911;



		public enum InventoryAlgorithmValue
		{
			Unknown = -1,
			FixedQ = 0,
			VariableQ = 1,
		}

		public static string GetAlgorithmName(InventoryAlgorithmValue algorithm)
		{
			switch (algorithm)
			{
				case InventoryAlgorithmValue.FixedQ:
					return "Algorithm 0 - Fixed Q";

				case InventoryAlgorithmValue.VariableQ:
					return "Algorithm 1 - Variable Q";

				case InventoryAlgorithmValue.Unknown:
				default:
					return "Unknown";
			}
		}

		public static string GetAlgorithmShortName(InventoryAlgorithmValue algorithm)
		{
			switch (algorithm)
			{
				case InventoryAlgorithmValue.FixedQ:
					return "Fixed Q (0)";

				case InventoryAlgorithmValue.VariableQ:
					return "Variable Q (1)";

				case InventoryAlgorithmValue.Unknown:
				default:
					return "Unknown";
			}
		}


		public class QueryConfigRegister
		{
			private BitVector32 _value;
			public static readonly BitVector32.Section reserved1	= BitVector32.CreateSection(0xF);
			public static readonly BitVector32.Section target		= BitVector32.CreateSection(1, reserved1);
			public static readonly BitVector32.Section session		= BitVector32.CreateSection(3, target);
			public static readonly BitVector32.Section select		= BitVector32.CreateSection(3, session);

			public QueryConfigRegister()
			{
				_value = new BitVector32(~0);
			}

			public QueryConfigRegister(uint value)
			{
				_value = new BitVector32((int)value);
			}

			public uint Value
			{
				get { unchecked { return (uint)_value.Data; } }
			}

            public rfid.Constants.SessionTarget Target
			{
				get
				{
					int val = _value[target];

                    return Enum.IsDefined( typeof( rfid.Constants.SessionTarget ), val ) ?
                        ( rfid.Constants.SessionTarget ) val : rfid.Constants.SessionTarget.UNKNOWN;
				}

				set
				{
					_value[target] = (int)value;
				}
			}

            public rfid.Constants.Session Session
			{
				get
				{
					int val = _value[session];

                    return Enum.IsDefined( typeof( rfid.Constants.Session ), val ) ?
                        ( rfid.Constants.Session ) val : 
                        rfid.Constants.Session.UNKNOWN;
				}

				set
				{
					_value[session] = (int)value;
				}
			}

			public LakeChabotReader.SelectedStateValue SelectedState
			{
				get
				{
					int val = _value[select];

					return Enum.IsDefined(typeof(LakeChabotReader.SelectedStateValue), val) ?
						(LakeChabotReader.SelectedStateValue)val : LakeChabotReader.SelectedStateValue.Unknown;
				}

				set
				{
					_value[select] = (int)value;
				}
			}
			
		}

		public class InventoryConfigRegister
		{
			private BitVector32 _value;
			public static readonly BitVector32.Section inv_algo		= BitVector32.CreateSection(63);
			public static readonly BitVector32.Section match_rep	= BitVector32.CreateSection(255, inv_algo);
			public static readonly BitVector32.Section auto_sel		= BitVector32.CreateSection(1, match_rep);


			public InventoryConfigRegister()
			{
				_value = new BitVector32(~0);
			}
		
			public InventoryConfigRegister(uint value)
			{
				_value = new BitVector32((int)value);
			}


			public uint Value 
			{
				get { unchecked { return (uint)_value.Data; } }
			}

			public InventoryAlgorithmValue Algorithm
			{
				get
				{
					int val = _value[inv_algo];

					return Enum.IsDefined(typeof(InventoryAlgorithmValue), val) ?
						(InventoryAlgorithmValue)val : InventoryAlgorithmValue.Unknown;
				}

				set
				{
					_value[inv_algo] = (int)value;
				}
			}

			public byte MatchRep
			{
				get
				{
					return (byte)_value[match_rep];
				}
				set
				{
					_value[match_rep] = value;
				}
			}

			public bool AutoSelect
			{
				get
				{
					return _value[auto_sel] == 1;
				}
				set
				{
					_value[auto_sel] = value ? 1 : 0;
				}
			}

		}


		public class FixedQInventoryParam0Register
		{
			private BitVector32 _value;
			public static readonly BitVector32.Section qValue = BitVector32.CreateSection(0xf);

			public FixedQInventoryParam0Register(uint value)
			{
				_value = new BitVector32((int)value);
			}


			public uint Value
			{
				get { unchecked { return (uint)_value.Data; } }
			}

			public byte Q
			{
				get	{	return (byte)_value[qValue];	}
				set	{	_value[qValue] = value;			}
			}

		} //public class FixedQInventoryParam0Register



		public class FixedQInventoryParam1Register
		{
			private BitVector32 _value;
			public static readonly BitVector32.Section retry = BitVector32.CreateSection(0xff);

			public FixedQInventoryParam1Register(uint value)
			{
				_value = new BitVector32((int)value);
			}


			public uint Value
			{
				get { unchecked { return (uint)_value.Data; } }
			}

			public byte Retry
			{
				get { return (byte)_value[retry]; }
				set { _value[retry] = value; }
			}

		} //public class FixedQInventoryParam1Register


		public class FixedQInventoryParam2Register
		{
			private BitVector32 _value;
			public static readonly BitVector32.Section aBFlip		= BitVector32.CreateSection(0x1);
			public static readonly BitVector32.Section runTillZero	= BitVector32.CreateSection(0x1, aBFlip);

			public FixedQInventoryParam2Register(uint value)
			{
				_value = new BitVector32((int)value);
			}


			public uint Value
			{
				get { unchecked { return (uint)_value.Data; } }
			}

			public bool ABFlip
			{
				get { return _value[aBFlip] == 1; }
				set { _value[aBFlip] = value ? 1 : 0; }
			}

			public bool RunTillZero
			{
				get { return _value[runTillZero] == 1; }
				set { _value[runTillZero] = value ? 1 : 0; }
			}

		} //public class FixedQInventoryParam2Register


		public class VariableQInventoryParam0Register
		{
			private BitVector32 _value;
			public static readonly BitVector32.Section startQ	= BitVector32.CreateSection(0xf);
			public static readonly BitVector32.Section maxQ		= BitVector32.CreateSection(0xf, startQ);
			public static readonly BitVector32.Section minQ		= BitVector32.CreateSection(0xf, maxQ);
			public static readonly BitVector32.Section maxReps	= BitVector32.CreateSection(0xff, minQ);

			public VariableQInventoryParam0Register(uint value)
			{
				_value = new BitVector32((int)value);
			}


			public uint Value
			{
				get { unchecked { return (uint)_value.Data; } }
			}

			public byte StartQ
			{
				get	{	return (byte)_value[startQ];	}
				set	{	_value[startQ] = value;			}
			}

			public byte MaxQ
			{
				get { return (byte)_value[maxQ]; }
				set { _value[maxQ] = value; }
			}

			public byte MinQ
			{
				get { return (byte)_value[minQ]; }
				set { _value[minQ] = value; }
			}

			public byte MaxQueryReps
			{
				get { return (byte)_value[maxReps]; }
				set { _value[maxReps] = value; }
			}

		} //public class VariableQInventoryParam0Register


		public class VariableQInventoryParam2Register
		{
			private BitVector32 _value;
			public static readonly BitVector32.Section aBFlip = BitVector32.CreateSection(0x1);

			public VariableQInventoryParam2Register(uint value)
			{
				_value = new BitVector32((int)value);
			}


			public uint Value
			{
				get { unchecked { return (uint)_value.Data; } }
			}

			public bool ABFlip
			{
				get { return _value[aBFlip] == 1; }
				set { _value[aBFlip] = value ? 1 : 0; }
			}

			
		} //public class VariableQInventoryParam2Register


	}
} 
