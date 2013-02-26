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
 * $Id: GridControl.cs,v 1.6 2010/06/17 01:03:58 dciampi Exp $
 * 
 * Description: 
 *
 *****************************************************************************
 */

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using RFID.RFIDInterface;
using System.Collections;


namespace RFID_Explorer
{
    public partial class GridControl : UserControl
    {
        public static Color[ ] GoodColors =
			{
				Color.FromArgb(255,255,255),	//0
				Color.FromArgb(204,237,220),	//1
				Color.FromArgb(187,231,208),	//2
				Color.FromArgb(165,224,193),	//3
				Color.FromArgb(133,213,172),	//4
				Color.FromArgb(103,202,151),	//5
				Color.FromArgb(86,196,140),		//6
				Color.FromArgb(68,190,127),		//7
				Color.FromArgb(48,183,113),		//8
				Color.FromArgb(25,175,98),		//9
			};

        public static Color[ ] BadColors =
			{
				Color.FromArgb(251,208,211),	//0
				Color.FromArgb(250,193,196),	//1
				Color.FromArgb(248,174,177),	//2
				Color.FromArgb(246,146,150),	//3
				Color.FromArgb(244,119,124),	//4
				Color.FromArgb(243,104,110),	//5
				Color.FromArgb(242,88,94),		//6
				Color.FromArgb(241,70,76),		//7
			};


        public static int goodAverageMidpoint = 2;

        public static Color GetColorForAverage( double average )
        {
            double alpha = 1.0 / ( double ) BadColors.Length;
            double beta = ( goodAverageMidpoint * 8.0 ) / ( double ) GoodColors.Length;

            if ( average >= 1.0 ) // Grean Zone
            {
                if ( average < beta * 1 )
                    return GoodColors[ 0 ];
                else if ( average < beta * 2 )
                    return GoodColors[ 1 ];
                else if ( average < beta * 3 )
                    return GoodColors[ 2 ];
                else if ( average < beta * 4 )
                    return GoodColors[ 3 ];
                else if ( average < beta * 5 )
                    return GoodColors[ 4 ];
                else if ( average < beta * 6 )
                    return GoodColors[ 5 ];
                else if ( average < beta * 7 )
                    return GoodColors[ 6 ];
                else if ( average < beta * 8 )
                    return GoodColors[ 7 ];
                else return GoodColors[ 8 ];
            }
            else
                if ( average > alpha * 7 )
                    return BadColors[ 0 ];
                else if ( average > alpha * 6 )
                    return BadColors[ 1 ];
                else if ( average > alpha * 5 )
                    return BadColors[ 2 ];
                else if ( average > alpha * 4 )
                    return BadColors[ 3 ];
                else if ( average > alpha * 3 )
                    return BadColors[ 4 ];
                else if ( average > alpha * 2 )
                    return BadColors[ 5 ];
                else if ( average > alpha * 1 )
                    return BadColors[ 6 ];
                else
                    return BadColors[ 7 ];

        }


        public enum StandardRule
        {
            ReaderID,
            PacketTime,
            PacketSequence,
            ElapsedTime,
            TagCount,
            UniqueTagCount,
            SingulationRate,
            UniqueSingulationRate,
            TimeUTC,
        }


        public class DisplayRule
        {

            public int DisplayIndex;
            public int Width;
            public bool Visible;
            public bool Frozen;
            public string Format;
            public string HeaderText;
            public string ToolTipText;
            public string DataPropertyName;

            public DataGridViewContentAlignment Alignment;

            public DisplayRule( string DataPropertyName, int DisplayIndex, bool Visible, bool Frozen, int Width, string Format, string HeaderText, string ToolTipText,
                                    DataGridViewContentAlignment Alignment )
            {

                this.DisplayIndex = DisplayIndex;
                this.Width = Width;
                this.Visible = Visible;
                this.Frozen = Frozen;
                this.Format = Format;
                this.HeaderText = HeaderText;
                this.ToolTipText = ToolTipText;
                this.DataPropertyName = DataPropertyName;
                this.Alignment = Alignment;
            }

            public static DisplayRule GetStandardRule( StandardRule rule, int DisplayIndex )
            {
                switch ( rule )
                {
                    case StandardRule.ReaderID:
                        return new DisplayRule("ReaderID", DisplayIndex, true, false, 45, "g", "Reader", "Unique reader device number (assigned by Explorer).", DataGridViewContentAlignment.MiddleCenter);

                    case StandardRule.PacketTime:
                        return new DisplayRule( "PacketTime", DisplayIndex, true, false, 80, "HH:mm:ss.fff", "Packet Time", "Time packet was received by application.", DataGridViewContentAlignment.MiddleCenter );

                    case StandardRule.PacketSequence:
                        return new DisplayRule( "PacketSequence", DisplayIndex, false, false, 80, "g", "Packet Number", "Packet number that caused the record to be created.", DataGridViewContentAlignment.MiddleCenter );

                    case StandardRule.ElapsedTime:
                        return new DisplayRule( "ElapsedTime", DisplayIndex, true, false, 80, "g", "Elapsed Ms", "Elapsed time in milliseconds.", DataGridViewContentAlignment.MiddleCenter );

                    case StandardRule.TagCount:
                        return new DisplayRule( "TagCount", DisplayIndex, true, false, 80, "g", "Tag Count", "Total number of tag signulations received.", DataGridViewContentAlignment.MiddleCenter );

                    case StandardRule.UniqueTagCount:
                        return new DisplayRule( "UniqueTagCount", DisplayIndex, true, false, 80, "g", "Unique Tags", "Unique tag signulations received.", DataGridViewContentAlignment.MiddleCenter );

                    case StandardRule.SingulationRate:
                        return new DisplayRule( "SingulationRate", DisplayIndex, true, false, 80, "f2", "Singulation Rate", "Rate at which tags were inventoried (tags per second).", DataGridViewContentAlignment.MiddleCenter );

                    case StandardRule.UniqueSingulationRate:
                        return new DisplayRule( "UniqueSingulationRate", DisplayIndex, true, false, 80, "f2", "Unique Rate", "Rate at which unique tags were inventoried (unique tags per second).", DataGridViewContentAlignment.MiddleCenter );

                    case StandardRule.TimeUTC: // hack in for Tom
                        return new DisplayRule( "Timestamp", DisplayIndex, true, false, 80, "HH:mm:ss.fff", "App First Seen UTC", "Time packet was received by application.", DataGridViewContentAlignment.MiddleCenter );

                    default:
                        throw new Exception( "Unknown/unsupported standard display rule in GridControl." );
                }
            }
        }

        public enum GridType
        {
            StandardView,

            ReaderRequests,

            RawPackets,

            ReaderCommands,

            ReaderAntennaCycles,

            AntennaPacket,

            InventoryCycle,

            InventoryRounds,

            InventoryParameters,

            BadPackets,

            InventoryCycleDiag,

            InventoryRoundDiag,

            TagAccess,

            TagDataDiagnostics,

            ReadRate,

        }



        public class GridClass
        {
            public GridType Grid;
            public String Caption;
            public String DataMember;
            public List<DisplayRule> DisplayRules;

            public GridClass( GridType grid, string caption, string dataMember )
            {
                Grid = grid;
                Caption = caption;
                DataMember = dataMember;
                DisplayRules = new List<DisplayRule>( );
            }
        }

        public class GridTypeCollection : KeyedCollection<GridType, GridClass>
        {
            public static GridTypeCollection GridCollection = BuildGridTypeCollection( );


            public static GridTypeCollection BuildGridTypeCollection( )
            {
                bool bIsShow;


        #if (DISPLAY_ALL_FUNCTION)
                bIsShow = true;
        #else
                bIsShow = false;
        #endif
                   

                GridTypeCollection rslt = new GridTypeCollection( );

                rslt.Add( new GridClass( GridType.StandardView, "Standard View", "TagInventory" ) );
                rslt.Add( new GridClass( GridType.ReaderRequests, "Issued Commands", "ReaderRequest" ) );
                rslt.Add( new GridClass( GridType.RawPackets, "All Packets (Raw Format)", "PacketStream" ) );
                rslt.Add( new GridClass( GridType.ReaderCommands, "Command Summary", "ReaderCommand" ) );
                rslt.Add( new GridClass( GridType.ReaderAntennaCycles, "Antenna Cycle Summary", "ReaderAntennaCycle" ) );
                rslt.Add( new GridClass( GridType.AntennaPacket, "Antenna Data", "AntennaPacket" ) );
                rslt.Add( new GridClass( GridType.InventoryCycle, "Inventory Cycle Summary", "InventoryCycle" ) );
                rslt.Add( new GridClass( GridType.InventoryRounds, "Inventory Round Summary", "InventoryRound" ) );
                rslt.Add( new GridClass( GridType.InventoryParameters, "Inventory Parameters", "InventoryRound" ) );
                rslt.Add( new GridClass( GridType.BadPackets, "Invalid / Unrecognized Packets", "BadPacket" ) );
                rslt.Add( new GridClass( GridType.InventoryCycleDiag, "Inventory Cycle Diagnostics", "InventoryCycle" ) );
                rslt.Add( new GridClass( GridType.InventoryRoundDiag, "Inventory Round Diagnostics", "InventoryRound" ) );
                rslt.Add( new GridClass( GridType.TagAccess, "Tag Access", "TagRead" ) );
                rslt.Add( new GridClass( GridType.TagDataDiagnostics, "Singulation Diagnostics", "TagRead" ) );
                rslt.Add( new GridClass( GridType.ReadRate, "Read Rate Data", "ReadRate" ) );


                //Tag Inventory (Normal View)   //clark 2011.2.14   Copied from R1000 Tracer
                rslt[ GridType.StandardView ].DisplayRules.AddRange( new DisplayRule[ ]
				{
					//DataPropertyName,							DisplayIndex,	Visible	    Frozen	Width	Format			HeaderText					ToolTipText										Alignment
					new DisplayRule("TagIdData",					0,			true,	    true,	300,	"g",			"ISO 18000-6C Inventory (PC - EPC - CRC)", "ISO 18000-6C Inventory Response consisting of the PC (Protocol Control) field, EPC or other code, and CRC16.", DataGridViewContentAlignment.MiddleCenter),
					new DisplayRule("ReadCount",					1,			true,	    false,	100,	"g",			"Read Count",				"Number of times the tag has been read (in the current session).",	DataGridViewContentAlignment.MiddleCenter),	
					new DisplayRule("CycleReadCount",				2,			true,	    false,	100,	"g",			"Current Cycle Read Count",	"Number of distinct reads (inventory singulations) in the current (or most recent) read cycle.",	DataGridViewContentAlignment.MiddleCenter),
					new DisplayRule("TotalCycleCount",				3,			bIsShow,	false,	100,	"g",			"Cycle Count",				"Count of the total number of distinct cycles in which the tag could have been read / inventoried (in the current session).",	DataGridViewContentAlignment.MiddleCenter),	
					new DisplayRule("ActualCycleCount",				4,			bIsShow,	false,	100,	"g",			"Cycles with Reads",		"Count of the number of distinct cycles in which the tag was actually read / inventoried (in the current session).",		DataGridViewContentAlignment.MiddleCenter),	
					new DisplayRule("SimpleAverage",				5,			bIsShow,	false,	100,	"f2",			"Mean Reads/Cycle",			"Session average reads per cycle.",				DataGridViewContentAlignment.MiddleCenter),	
					new DisplayRule("AverageReadsPerCycle",			6,			bIsShow,	false,	100,	"f2",			"EMA Reads/Cycle",			"Exponentially weighted moving average of number of reads per cycle.",		DataGridViewContentAlignment.MiddleCenter),	

					new DisplayRule("FirstCommand",					7,			bIsShow,	false,	100,	"g",			"First Read Command ID",	"Command number (zero relative) when tag was first read.",	DataGridViewContentAlignment.MiddleCenter),	
					new DisplayRule("FirstReadTime",				8,			bIsShow,	false,	100,	"g",			"First Read Timestamp",		"Device timestamp of first read.",				DataGridViewContentAlignment.MiddleCenter),	
					new DisplayRule("LastCommand",					9,			bIsShow,	false,	100,	"g",			"Last Read Command ID",		"Command number (zero relative) when tag was last read",	DataGridViewContentAlignment.MiddleCenter),	
					new DisplayRule("LastReadTime",					10,			bIsShow,	false,	100,	"g",			"Last Read Timestamp",		"Device timestamp of most recent read.",		DataGridViewContentAlignment.MiddleCenter),	
					
					//clark 2011.7.15
                    //wait for firmware to support 4 port.
                    new DisplayRule("Port0reads",					11,			true,	    false,	100,	"g",			"Antenna 0",				"Number of times the tag was read with antenna 0.",		DataGridViewContentAlignment.MiddleCenter),	
                    new DisplayRule("Port1reads",					12,			true,	    false,	100,	"g",			"Antenna 1",				"Number of times the tag was read with antenna 1.",		DataGridViewContentAlignment.MiddleCenter),	
					new DisplayRule("Port2reads",					13,			true,   	false,	100,	"g",			"Antenna 2",				"Number of times the tag was read with antenna 2.",		DataGridViewContentAlignment.MiddleCenter),	
					new DisplayRule("Port3reads",					14,			true,   	false,	100,	"g",			"Antenna 3",				"Number of times the tag was read with antenna 3.",		DataGridViewContentAlignment.MiddleCenter),	
					new DisplayRule("Port4reads",					15,			true,	    false,	100,	"g",			"Antenna 4",				"Number of times the tag was read with antenna 4.",		DataGridViewContentAlignment.MiddleCenter),	
					new DisplayRule("Port5reads",					16,			true,	    false,	100,	"g",			"Antenna 5",				"Number of times the tag was read with antenna 5.",		DataGridViewContentAlignment.MiddleCenter),	
					new DisplayRule("Port6reads",					17,			true,	    false,	100,	"g",			"Antenna 6",				"Number of times the tag was read with antenna 6.",		DataGridViewContentAlignment.MiddleCenter),	
					new DisplayRule("Port7reads",					18,			true,	    false,	100,	"g",			"Antenna 7",				"Number of times the tag was read with antenna 7.",		DataGridViewContentAlignment.MiddleCenter),	
					new DisplayRule("Port8reads",					19,			true,	    false,	100,	"g",			"Antenna 8",				"Number of times the tag was read with antenna 8.",		DataGridViewContentAlignment.MiddleCenter),	
					new DisplayRule("Port9reads",					20,			true,	    false,	100,	"g",			"Antenna 9",				"Number of times the tag was read with antenna 9.",		DataGridViewContentAlignment.MiddleCenter),	
					new DisplayRule("Port10reads",					21,			true,   	false,	100,	"g",			"Antenna 10",				"Number of times the tag was read with antenna 10.",	DataGridViewContentAlignment.MiddleCenter),	
					new DisplayRule("Port11reads",					22,			true,   	false,	100,	"g",			"Antenna 11",				"Number of times the tag was read with antenna 11.",	DataGridViewContentAlignment.MiddleCenter),	
					new DisplayRule("Port12reads",					23,			true,	    false,	100,	"g",			"Antenna 12",				"Number of times the tag was read with antenna 12.",	DataGridViewContentAlignment.MiddleCenter),	
					new DisplayRule("Port13reads",					24,			true,	    false,	100,	"g",			"Antenna 13",				"Number of times the tag was read with antenna 13.",	DataGridViewContentAlignment.MiddleCenter),	
					new DisplayRule("Port14reads",					25,			true,	    false,	100,	"g",			"Antenna 14",				"Number of times the tag was read with antenna 14.",	DataGridViewContentAlignment.MiddleCenter),	
					new DisplayRule("Port15reads",					26,			true,	    false,	100,	"g",			"Antenna 15",				"Number of times the tag was read with antenna 15.",	DataGridViewContentAlignment.MiddleCenter),	
	            
                    new DisplayRule("DataRead",			     		27,			true,	    false,	100,	"g",			"Data",				         "Data",	                                            DataGridViewContentAlignment.MiddleCenter),	
                    //DisplayRule.GetStandardRule(StandardRule.TimeUTC,			27),
				} );


                //Reader Requests
                rslt[ GridType.ReaderRequests ].DisplayRules.AddRange( new DisplayRule[ ]
				{
					//DataPropertyName,									DisplayIndex,	Visible	Frozen	Width	Format			HeaderText					ToolTipText										Alignment
					new DisplayRule("RequestSequence",							0,			true,	true,	50,		"g",			"#",						"Packet sequence number.",						DataGridViewContentAlignment.MiddleCenter),
					new DisplayRule("StartTime",								1,			true,	false,	100,	"HH:mm:ss.fff",	"Start Time",				"Time (UTC) request was started.",				DataGridViewContentAlignment.MiddleCenter),	
					new DisplayRule("RequestName",								2,			true,	false,	100,	"g",			"Request",					"Type of request",								DataGridViewContentAlignment.MiddleCenter),					
					new DisplayRule("PacketCount",								3,			true,	false,	100,	"g",			"Packet Count",				"Number of packets received in processing the request.",			DataGridViewContentAlignment.MiddleCenter),	
					DisplayRule.GetStandardRule(StandardRule.TagCount,			4),
					DisplayRule.GetStandardRule(StandardRule.ElapsedTime,		5),
					DisplayRule.GetStandardRule(StandardRule.SingulationRate,	6),					
//					DisplayRule.GetStandardRule(StandardRule.UniqueTagCount,	7),
				} );

                //Packet Data
                rslt[ GridType.RawPackets ].DisplayRules.AddRange( new DisplayRule[ ]
				{
					//DataPropertyName,									DisplayIndex,	Visible	Frozen	Width	Format			HeaderText					ToolTipText										Alignment
					DisplayRule.GetStandardRule(StandardRule.ReaderID,			0),
					new DisplayRule("PacketSequence",							1,			true,	true,	50,		"g",			"#",						"Packet sequence number.",						DataGridViewContentAlignment.MiddleCenter),		
					DisplayRule.GetStandardRule(StandardRule.PacketTime,		2),
					new DisplayRule("PacketType",								3,			true,	false,	100,	"g",			"Packet Type",				"Type of packet (from packet preamble).",		DataGridViewContentAlignment.MiddleCenter),	
					new DisplayRule("PacketData",								4,			true,	false,	450,	"g",			"Raw Packet Data",			"Packet dataFile received from reader.",			DataGridViewContentAlignment.MiddleLeft),	
				} );

                // reader Commands												
                rslt[ GridType.ReaderCommands ].DisplayRules.AddRange( new DisplayRule[ ]	
				{																
					//DataPropertyName,									DisplayIndex,	Visible	Frozen	Width	Format			HeaderText					ToolTipText										Alignment
					DisplayRule.GetStandardRule(StandardRule.ReaderID,			0),
					new DisplayRule("CommandSequence",							1,			true,	true,	50,		"g",			"#",						"Command sequence number.",						DataGridViewContentAlignment.MiddleCenter),
					DisplayRule.GetStandardRule(StandardRule.PacketTime,		2),
					DisplayRule.GetStandardRule(StandardRule.PacketSequence,	3),
					DisplayRule.GetStandardRule(StandardRule.ElapsedTime,		4),
					new DisplayRule("CommandType",								5,			true,	false,	80,		"g",			"Command Type",				"Type of command.",								DataGridViewContentAlignment.MiddleCenter),
					new DisplayRule("CommandResult",							6,			true,	false,	60,		"g",			"Result",					"Comand result.",								DataGridViewContentAlignment.MiddleCenter),
					new DisplayRule("ContinuousModeFlag",						7,			true,	false,	80,		"g",			"Continuous Mode",			"Mode of operation in which the command executed.", DataGridViewContentAlignment.MiddleCenter),
					DisplayRule.GetStandardRule(StandardRule.TagCount,			8),
					DisplayRule.GetStandardRule(StandardRule.SingulationRate,	9),
					DisplayRule.GetStandardRule(StandardRule.UniqueTagCount,	10),
					DisplayRule.GetStandardRule(StandardRule.UniqueSingulationRate, 11),
//					new DisplayRule("UniqueSingulationRate",					9,			true,	false,	50,		"g",			"Unique Sing. Rate",		"Timestamp on device when command started.",	DataGridViewContentAlignment.MiddleCenter),
//					new DisplayRule("Cmd_endTime",								10,			true,	false,	50,		"g",			"End Timestamp",			"Timestamp on device when command ended.",		DataGridViewContentAlignment.MiddleCenter),
//					new DisplayRule("Cmd_delta",								11,			true,	false,	50,		"g",			"Device Time (ms).",		"Device processing time (milliseconds).",		DataGridViewContentAlignment.MiddleCenter),
				} );

                // Reader Antenna Cycles
                rslt[ GridType.ReaderAntennaCycles ].DisplayRules.AddRange( new DisplayRule[ ]
				{
					//DataPropertyName,									DisplayIndex,	Visible	Frozen	Width	Format			HeaderText					ToolTipText										Alignment
					DisplayRule.GetStandardRule(StandardRule.ReaderID,			0),
					new DisplayRule("CycleSequence",							1,			true,	true,	50,		"g",			"#",						"Packet sequence number.",						DataGridViewContentAlignment.MiddleCenter),
					DisplayRule.GetStandardRule(StandardRule.PacketTime,		2),
					DisplayRule.GetStandardRule(StandardRule.PacketSequence,	3),
					DisplayRule.GetStandardRule(StandardRule.ElapsedTime,		4),
					new DisplayRule("AntennaCount",								5,			true,	false,	80,		"g",			"Antenna Count",			"Number of logical antennas included int the reader antenna cycle.", DataGridViewContentAlignment.MiddleCenter),
					DisplayRule.GetStandardRule(StandardRule.TagCount,			6),
					DisplayRule.GetStandardRule(StandardRule.SingulationRate,	7),
					DisplayRule.GetStandardRule(StandardRule.UniqueTagCount,	8),
					DisplayRule.GetStandardRule(StandardRule.UniqueSingulationRate, 9),
					new DisplayRule("InventoryCycleCount",						10,			true,	false,	80,		"g",			"Inventory Cycles",			"Total number of inventory cycles included in the antenna cycle", DataGridViewContentAlignment.MiddleCenter),
					new DisplayRule("InventoryRoundCount",						11,			true,	false,	80,		"g",			"Inventory Rounds",			"Total number of inventory rounds included in the antenna cycle", DataGridViewContentAlignment.MiddleCenter),
//					new DisplayRule("Cyc_begTime",								10,			true,	false,	100,	"g",			"Start Timestamp",			"Timestamp on device when cycle started.",		DataGridViewContentAlignment.MiddleCenter),	
//					new DisplayRule("Cyc_endTime",								4,			true,	false,	100,	"g",			"End Timestamp",			"Timestamp on device when cycle ended.",		DataGridViewContentAlignment.MiddleLeft),	
//					new DisplayRule("Cyc_delta",								5,			true,	false,  75,		"g",			"Device Time (ms)",			"Device processing time (milliseconds).",		DataGridViewContentAlignment.MiddleCenter),	
				} );

                // Antenna Cycle								
                rslt[ GridType.AntennaPacket ].DisplayRules.AddRange( new DisplayRule[ ]	
				{																
					//DataPropertyName,									DisplayIndex,	Visible	Frozen	Width	Format			HeaderText					ToolTipText											Alignment
					DisplayRule.GetStandardRule(StandardRule.ReaderID,			0),
					new DisplayRule("AntennaSequence",							1,			true,	true,	50,		"g",			"#",						"Command sequence number.",							DataGridViewContentAlignment.MiddleCenter),		
					DisplayRule.GetStandardRule(StandardRule.PacketTime,		2),
					new DisplayRule("AntennaNumber",							3,			true,	false,	100,	"g",			"Antenna",					"Antenna (port) number.",							DataGridViewContentAlignment.MiddleCenter),	
					new DisplayRule("SenseResistorValue",						4,			true,	false,	75,		"g",			"Antenna Sense",			"Antenna sense resistor value.",					DataGridViewContentAlignment.MiddleCenter),
					DisplayRule.GetStandardRule(StandardRule.TagCount,			5),
					DisplayRule.GetStandardRule(StandardRule.ElapsedTime,		6),
					DisplayRule.GetStandardRule(StandardRule.SingulationRate,	7),
					DisplayRule.GetStandardRule(StandardRule.UniqueTagCount,	8),
				} );

                // Inventory Cycle
                rslt[ GridType.InventoryCycle ].DisplayRules.AddRange( new DisplayRule[ ]	
				{																
					//DataPropertyName,									DisplayIndex,	Visible	Frozen	Width	Format			HeaderText					ToolTipText											Alignment
					DisplayRule.GetStandardRule(StandardRule.ReaderID,			0),
					new DisplayRule("CycleSequence",							1,			true,	true,	50,		"g",			"#",						"Command sequence number.",							DataGridViewContentAlignment.MiddleCenter),
					DisplayRule.GetStandardRule(StandardRule.PacketTime,		2),
					DisplayRule.GetStandardRule(StandardRule.PacketSequence,	3),
					new DisplayRule("AntennaNumber",							4,			true,	false,	75,		"g",			"Antenna",					"Antenna (port) number.",							DataGridViewContentAlignment.MiddleCenter),	
					DisplayRule.GetStandardRule(StandardRule.TagCount,			5),
					DisplayRule.GetStandardRule(StandardRule.ElapsedTime,		6),
					DisplayRule.GetStandardRule(StandardRule.SingulationRate,	7),
					DisplayRule.GetStandardRule(StandardRule.UniqueTagCount,	8),
					DisplayRule.GetStandardRule(StandardRule.UniqueSingulationRate, 9),
				} );

                // Inventory Rounds
                rslt[ GridType.InventoryRounds ].DisplayRules.AddRange( new DisplayRule[ ]	
				{																
					//DataPropertyName,									DisplayIndex,	Visible	Frozen	Width	Format			HeaderText					ToolTipText											Alignment
					DisplayRule.GetStandardRule(StandardRule.ReaderID,			0),
					new DisplayRule("RoundSequence",							1,			true,	true,	50,		"g",			"#",						"Command sequence number.",							DataGridViewContentAlignment.MiddleCenter),
					DisplayRule.GetStandardRule(StandardRule.PacketTime,		2),
					DisplayRule.GetStandardRule(StandardRule.PacketSequence,	3),
					new DisplayRule("AntennaNumber",							4,			true,	false,	75,		"g",			"Antenna",					"Antenna (port) number.",							DataGridViewContentAlignment.MiddleCenter),	
					new DisplayRule("SingulationParametersString",				5,			true,	false,	75,		"g",			"Parameters",				"Singulation parameters used for inventory round.",						DataGridViewContentAlignment.MiddleCenter),
					DisplayRule.GetStandardRule(StandardRule.TagCount,			6),
					DisplayRule.GetStandardRule(StandardRule.ElapsedTime,		7),
					DisplayRule.GetStandardRule(StandardRule.SingulationRate,	8),
				} );

                // Inventory Parameters
                rslt[ GridType.InventoryParameters ].DisplayRules.AddRange( new DisplayRule[ ]	
				{																
					//DataPropertyName,										DisplayIndex,	Visible	Frozen	Width	Format			HeaderText					ToolTipText										Alignment
					DisplayRule.GetStandardRule(StandardRule.ReaderID,			0),
					new DisplayRule("RoundSequence",							1,			true,	true,	50,		"g",			"#",						"Command sequence number.",						DataGridViewContentAlignment.MiddleCenter),
					DisplayRule.GetStandardRule(StandardRule.PacketTime,		2),
					DisplayRule.GetStandardRule(StandardRule.PacketSequence,	3),
					new DisplayRule("AntennaNumber",							4,			true,	false,	75,		"g",			"Antenna",					"Antenna (port) number.",						DataGridViewContentAlignment.MiddleCenter),
					new DisplayRule("CurrentQValue",							5,			true,	false,	75,		"g",			"\"Q\" Value",				"Current Q value.",								DataGridViewContentAlignment.MiddleCenter),
					new DisplayRule("InventoriedFlag",							6,			true,	false,	75,		"g",			"Flag",						"A/B inventory flag.",							DataGridViewContentAlignment.MiddleCenter),
					new DisplayRule("CurrentSlot",								7,			true,	false,	75,		"g",			"Current Slot",				"Current slot.",								DataGridViewContentAlignment.MiddleCenter),
					new DisplayRule("CurrentRetry",								8,			true,	false,	75,		"g",			"Retry",					"Current retry count.",							DataGridViewContentAlignment.MiddleCenter),
					new DisplayRule("SingulationParameters",					9,			false,	false,	75,		"x",			"Parameter (hex)",				"Singulation Parameters.",						DataGridViewContentAlignment.MiddleCenter),
				} );

                // Bad Packets
                rslt[ GridType.BadPackets ].DisplayRules.AddRange( new DisplayRule[ ]
				{
					DisplayRule.GetStandardRule(StandardRule.ReaderID,			0),
					new DisplayRule("PacketSequence",							1,			true,	true,	50,		"g",			"#",						"Packet sequence number.",						DataGridViewContentAlignment.MiddleCenter),		
					DisplayRule.GetStandardRule(StandardRule.PacketTime,		2),
					new DisplayRule("PacketData",								3,			true,	false,	225,	"g",			"Raw Packet Data",			"Packet dataFile received from reader.",		DataGridViewContentAlignment.MiddleLeft),	
					new DisplayRule("ErrorMessage",								4,			true,	false,	225,	"g",			"Parsing Error",			"Error message from the parser.",				DataGridViewContentAlignment.MiddleLeft),	
				} );

                // Inventory Cycle Diagnostics
                rslt[ GridType.InventoryCycleDiag ].DisplayRules.AddRange( new DisplayRule[ ]	
				{																
					//DataPropertyName,									DisplayIndex,	Visible	Frozen	Width	Format			HeaderText					ToolTipText										Alignment
					DisplayRule.GetStandardRule(StandardRule.ReaderID,			0),
					new DisplayRule("CycleSequence",							1,			true,	true,	50,		"g",			"#",						"Command sequence number.",						DataGridViewContentAlignment.MiddleCenter),
					DisplayRule.GetStandardRule(StandardRule.PacketTime,		2),
					DisplayRule.GetStandardRule(StandardRule.PacketSequence,	3),
					new DisplayRule("Querys",									4,			true,	false,	100,	"g",			"Queries",					"Number of queries issued.",						DataGridViewContentAlignment.MiddleCenter),	
					new DisplayRule("Rn16rcv",									5,			true,	false,	100,	"g",			"Rn16 Recieved",			"Number of RN16s received.",						DataGridViewContentAlignment.MiddleCenter),	
					new DisplayRule("Rn16Timeouts",								6,			true,	false,	100,	"g",			"Rn16 Timeouts",			"Number of RN16 timeouts (empty slots).",			DataGridViewContentAlignment.MiddleCenter),	
					new DisplayRule("EpcTimeouts",								7,			true,	false,	100,	"g",			"EPC Timeouts",				"Number of EPC timeoutss (No response to rn16 Ack).",	DataGridViewContentAlignment.MiddleCenter),	
					new DisplayRule("GoodReads",								8,			true,	false,	100,	"g",			"Good Reads",				"Number of good EPC reads .",						DataGridViewContentAlignment.MiddleCenter),	
					new DisplayRule("CrcFailures",								9,			true,	false,	100,	"g",			"CRC Failures",				"Number of CRC failures.",							DataGridViewContentAlignment.MiddleCenter),
				} );


                // Inventory Round Diagnostics
                rslt[ GridType.InventoryRoundDiag ].DisplayRules.AddRange( new DisplayRule[ ]	
				{																
					//DataPropertyName,									DisplayIndex,	Visible	Frozen	Width	Format			HeaderText					ToolTipText										Alignment
					DisplayRule.GetStandardRule(StandardRule.ReaderID,			0),
					new DisplayRule("RoundSequence",							1,			true,	true,	50,		"g",			"#",						"Command sequence number.",						DataGridViewContentAlignment.MiddleCenter),
					DisplayRule.GetStandardRule(StandardRule.PacketTime,		2),
					DisplayRule.GetStandardRule(StandardRule.PacketSequence,	3),
					new DisplayRule("Querys",									4,			true,	false,	100,	"g",			"Queries",					"Number of queries issued.",						DataGridViewContentAlignment.MiddleCenter),	
					new DisplayRule("Rn16rcv",									5,			true,	false,	100,	"g",			"Rn16 Recieved",			"Number of RN16s received.",						DataGridViewContentAlignment.MiddleCenter),	
					new DisplayRule("Rn16Timeouts",								6,			true,	false,	100,	"g",			"Rn16 Timeouts",			"Number of RN16 timeouts (empty slots).",			DataGridViewContentAlignment.MiddleCenter),	
					new DisplayRule("EpcTimeouts",								7,			true,	false,	100,	"g",			"EPC Timeouts",				"Number of EPC timeoutss (No response to rn16 Ack).",	DataGridViewContentAlignment.MiddleCenter),	
					new DisplayRule("GoodReads",								8,			true,	false,	100,	"g",			"Good Reads",				"Number of good EPC reads .",						DataGridViewContentAlignment.MiddleCenter),	
					new DisplayRule("CrcFailures",								9,			true,	false,	100,	"g",			"CRC Failures",				"Number of CRC failures.",							DataGridViewContentAlignment.MiddleCenter),
				} );


                // Tag Access
                rslt[ GridType.TagAccess ].DisplayRules.AddRange( new DisplayRule[ ]		
				{																
									//DataPropertyName,					DisplayIndex,	Visible	Frozen	Width	Format			HeaderText					ToolTipText										Alignment
					DisplayRule.GetStandardRule(StandardRule.ReaderID,			0),
					new DisplayRule("ReadSequence",								1,			true,	true,	50,		"g",			"#",						"Read sequence number.",						DataGridViewContentAlignment.MiddleCenter),	
					DisplayRule.GetStandardRule(StandardRule.PacketTime,		2),
					DisplayRule.GetStandardRule(StandardRule.PacketSequence,	3),
					new DisplayRule("TagId",									4,			true,	true,	300,	"g",			"EPC Data (PC - EPC - CRC)","EPC Global Tag ID.",							DataGridViewContentAlignment.MiddleCenter),
					new DisplayRule("AntennaNumber",							5,			true,	false,	100,	"g",			"Antenna",					"Antenna (port) number.",						DataGridViewContentAlignment.MiddleCenter),	
					new DisplayRule("InventoryTime",							6,			true,	false,	100,	"g",			"Device Time",				"Time on device when tag was read.",			DataGridViewContentAlignment.MiddleCenter),	
					new DisplayRule("ResultType",								7,			true,	false,	100,	"g",			"Error Flag",				"Tag access error.",							DataGridViewContentAlignment.MiddleCenter),
					new DisplayRule("AccessType",								8,			true,	false,	100,	"g",			"Access",					"Tag access type (read, write, etc.)",			DataGridViewContentAlignment.MiddleCenter),
					new DisplayRule("Parameter",								9,			true,	false,	100,	"g",			"Parameter",				"Tag access parameter.",						DataGridViewContentAlignment.MiddleCenter),
					new DisplayRule("TagData",									10,			true,	false,	100,	"g",			"Tag Data",					"Data from tag access operation.",				DataGridViewContentAlignment.MiddleCenter),

				} );

                // INVENTORY Data	Disgnostics
                rslt[ GridType.TagDataDiagnostics ].DisplayRules.AddRange( new DisplayRule[ ]		
				{																
									//DataPropertyName,					DisplayIndex,	Visible	Frozen	Width	Format			HeaderText					ToolTipText										Alignment
					new DisplayRule("ReadSequence",								0,			true,	true,	50,		"g",			"#",						"Read sequence number.",						DataGridViewContentAlignment.MiddleCenter),	
					DisplayRule.GetStandardRule(StandardRule.PacketTime,		1),
					DisplayRule.GetStandardRule(StandardRule.PacketSequence,	2),
					new DisplayRule("AntennaNumber",							3,			true,	false,	100,	"g",			"Antenna",					"Antenna (port) number.",						DataGridViewContentAlignment.MiddleCenter),	
					new DisplayRule("TagId",									4,			true,	false,	300,	"g",			"EPC Data (PC - EPC - CRC)","EPC Global Tag ID.",							DataGridViewContentAlignment.MiddleCenter),
					new DisplayRule("CrcResult",								5,			true,	false,	100,	"g",			"CRC Result",				"Result of CRC check.",							DataGridViewContentAlignment.MiddleCenter),
					new DisplayRule("NB_RSSI",									6,			true,	false,	100,	"g",			"NB RSSI",					"NB Receive Signal Strength Indicator (backscattered tag signal amplitude).", DataGridViewContentAlignment.MiddleCenter),	
					new DisplayRule("WB_RSSI",									7,			true,	false,	100,	"g",			"WB RSSI",					"WB Receive Signal Strength Indicator (backscattered tag signal amplitude).", DataGridViewContentAlignment.MiddleCenter),	
					new DisplayRule("RSSI",								     	8,			true,	false,	100,	"g",			"RSSI",					    "Receive Signal Strength Indicator.", DataGridViewContentAlignment.MiddleCenter),	
					new DisplayRule("ProtocolParameters",						9,			true,	false,	100,	"g",			"Parameter",				"Protocol parameter.",							DataGridViewContentAlignment.MiddleCenter),	
				} );

                // Read Rate
                rslt[ GridType.ReadRate ].DisplayRules.AddRange( new DisplayRule[ ]
				{
									//DataPropertyName,					DisplayIndex,	Visible	Frozen	Width	Format			HeaderText					ToolTipText										Alignment
					DisplayRule.GetStandardRule(StandardRule.ReaderID,			0),
					new DisplayRule("ReadSequence",								1,			true,	true,	50,		"g",			"#",					"Read sequence number.",					DataGridViewContentAlignment.MiddleCenter),	
					new DisplayRule("ClientReadTime",							2,			true,	true,	50,		"g",			"Client Time",			"Client Read Time.",						DataGridViewContentAlignment.MiddleCenter),	
					new DisplayRule("DeviceReadTime",							3,			true,	true,	50,		"g",			"Device Time",			"Device Read Time.",						DataGridViewContentAlignment.MiddleCenter),	
				} );
                return rslt;
            }


            public GridTypeCollection( ) : base( null, -1 ) { }

            protected override GridType GetKeyForItem( GridClass item )
            {
                return item.Grid;
            }
        }


        public class RowCountChangedEventArgs : System.EventArgs
        {
            public int NewRowCount;

            public RowCountChangedEventArgs( int rows )
            {
                NewRowCount = rows;
            }
        }




        //		public delegate void RowCountChangedHandler(object sender, RowCountChangedEventArgs e);
        //		public event RowCountChangedHandler RowCountChanged;



        //private DataSet					_dataset	= null;
        private GridClass _grid = null;
        private Object _dataFile = null;
        private IGridControlPager _pager = null;

        static GridControl( )
        {
            ValidateViews( );

            foreach ( GridClass grid in GridTypeCollection.GridCollection )
            {
                if ( grid.DisplayRules.Count == 0 )
                {
                    String msg = String.Format( ">>>>>ERROR: No display rules were provided for gridType {0:g}.<<<<<", grid.Grid );
                    System.Diagnostics.Debug.WriteLine( msg );
                    System.Diagnostics.Debug.Assert( false, msg );
                }
            }

        }

        public GridControl( GridType grid )
        {
            InitializeComponent( );
            this.Dock = DockStyle.Fill;

            if ( GridTypeCollection.GridCollection.Contains( grid ) )
            {
                captionLabel.Text = GridTypeCollection.GridCollection[ grid ].Caption;
                BuildGrid( grid );

                Type t = GridControlPager<DatabaseRowTemplate>.GetPagerForClass( grid );
                Pager = ( IGridControlPager ) t.GetConstructors( )[ 0 ].Invoke( new object[ ] { this } );
            }
            else
            {
                String msg = String.Format( ">>>>>ERROR: GridType \"{0:g}\" ({0:d}) is not in the GridCollection. See BuildGridTypeCollection().<<<<<", grid );
                System.Diagnostics.Debug.WriteLine( msg );
                System.Diagnostics.Debug.Assert( false, msg );
            }
        }


        public GridClass Grid
        {
            get { return _grid; }
            private set { _grid = value; }
        }

        private IGridControlPager Pager
        {
            get { return _pager; }
            set { _pager = value; }
        }

        public int ColumnCount
        {
            get { return Grid.DisplayRules.Count; }
        }

        public bool HasSelection
        {
            get { return theGrid.GetCellCount( DataGridViewElementStates.Selected ) > 0; }
        }



        public DictionaryEntry[ ] DataSources
        {
            set
            {
                //				System.Diagnostics.Debug.WriteLine(String.Format("{0}() ThreadID = {1}", System.Reflection.MethodInfo.GetCurrentMethod().Name, System.Threading.Thread.CurrentThread.ManagedThreadId));

                _dataFile = null;
                if ( value == null )
                    return;
                foreach ( DictionaryEntry entry in value )
                {
                    if ( ( string ) entry.Key == Grid.DataMember )
                    {
                        _dataFile = entry.Value;
                        break;
                    }
                }
                if ( _dataFile == null )
                {
                    throw new Exception( String.Format( "Unable to find required data member, {0} in data sources.", Grid.DataMember ) );
                }
            }
        }



        public void SelectAll( )
        {
            theGrid.SelectAll( );
        }

        public string Copy( )
        {
            DataGridViewSelectedCellCollection selection = theGrid.SelectedCells;

            DataObject data = theGrid.GetClipboardContent( );


            if ( theGrid.GetCellCount( DataGridViewElementStates.Selected ) > 0 )
            {
                Clipboard.SetDataObject( theGrid.GetClipboardContent( ) );
                return string.Format( "Copied {0} rows to the clipboard.", theGrid.SelectedRows.Count );
            }
            return string.Format( "Nothing to copy, no rows have been selected.", theGrid.SelectedRows.Count );
        }



        public string CopyAll( )
        {
            DataGridViewSelectedCellCollection selection = theGrid.SelectedCells;
            theGrid.SelectAll( );
            Clipboard.SetDataObject( theGrid.GetClipboardContent( ) );
            string rslt = String.Format( "Copied {0} rows to the clipboard.", theGrid.SelectedRows.Count );
            theGrid.ClearSelection( );
            foreach ( DataGridViewCell cell in selection )
                cell.Selected = true;
            return rslt;
        }



        public void UpdateNow( )
        {
            //			System.Diagnostics.Debug.WriteLine(String.Format("{0}() ThreadID = {1}", System.Reflection.MethodInfo.GetCurrentMethod().Name, System.Threading.Thread.CurrentThread.ManagedThreadId));

            if ( this.Disposing )
                return;

            if ( _dataFile == null )
                return;

            Pager.DataFile = _dataFile;

            theGrid.DataSource = Pager.GetData( 0 );

            navigator.BindingSource = new BindingSource( _pager.GetPages( ), "" );
            navigator.BindingSource.PositionChanged += new EventHandler( BindingSource_PositionChanged );
            navigator.Refresh( );
            theGrid.Refresh( );
        }

        void BindingSource_PositionChanged( object sender, EventArgs e )
        {
            theGrid.DataSource = _pager.GetData( navigator.BindingSource.Position );
        }

        public void Clear( )
        {
            theGrid.DataSource = null;
            navigator.BindingSource = null;
            theGrid.Refresh( );
        }

        private void theGrid_DataError( object sender, DataGridViewDataErrorEventArgs e )
        {
            System.Diagnostics.Debug.WriteLine( String.Format( "DATA ERROR: {0} on row {1}, col {2}.", e.Exception.Message, e.RowIndex, e.ColumnIndex ) );
        }

        private static void ValidateViews( )
        {
            bool errors = false;
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetAssembly( typeof( RFID.RFIDInterface.IReader ) );
            foreach ( GridClass gridClass in GridTypeCollection.GridCollection )
            {
                String typeTarget = string.Format( "RFID.RFIDInterface.{0}", gridClass.DataMember );

                Type dataSourceType = assembly.GetType( typeTarget, false );
                if ( dataSourceType == null )
                {
                    System.Diagnostics.Debug.Assert( false, "Bad type name in DataGrid views" );
                    throw new Exception( String.Format( "Error validating DataGrid views. {0} does not exist in LakeChabotReader.", typeTarget ) );
                }
                System.Diagnostics.Debug.Assert( dataSourceType.IsSubclassOf( typeof( DatabaseRowTemplate ) ) );
                int[ ] rowOrderCounter = new int[ gridClass.DisplayRules.Count ];
                foreach ( DisplayRule ruleList in gridClass.DisplayRules )
                {
                    int ndx = ruleList.DisplayIndex;
                    if ( ndx < 0 || ndx >= gridClass.DisplayRules.Count )
                    {
                        errors = true;
                        System.Diagnostics.Debug.WriteLine( String.Format( ">>>> {0} has invalid display index. <<<<", gridClass.Grid ) );
                    }
                    else
                    {
                        ++rowOrderCounter[ ndx ];
                    }
                    
                    // Console.WriteLine( dataSourceType + " ----> " + ruleList.DataPropertyName );

                    if ( dataSourceType.GetProperty( ruleList.DataPropertyName ) == null )
                    {
                        // Console.WriteLine("    was missing...");

                        errors = true;
                        System.Diagnostics.Debug.WriteLine( String.Format( ">>>> {0} is not in {1}<<<<", ruleList.DataPropertyName, dataSourceType.Name ) );
                    }
                }

                for ( int i = 0; i < rowOrderCounter.Length; i++ )
                {
                    if ( rowOrderCounter[ i ] != 1 )
                    {
                        errors = true;
                        System.Diagnostics.Debug.WriteLine( String.Format( ">>>> {0} has duplicat or missing display index values. <<<<", gridClass.Grid ) );
                    }
                }
            }
            System.Diagnostics.Debug.Assert( errors == false, ">>>>> ERROR: One ore more gridClass rules reference invalid data Properities. See debug output." );
        }



        private void BuildGrid( GridType gridType )
        {
            if ( !GridTypeCollection.GridCollection.Contains( gridType ) )
                throw new ApplicationException( String.Format( "GridControl.BuidGrid() was passed an invalid GridType. GridType \"{0:g}\" ({0:d}) is not in the GridCollection. See BuildGridTypeCollection()", gridType ) );

            Grid = GridTypeCollection.GridCollection[ gridType ];

            theGrid.Dock = DockStyle.Fill;
            theGrid.ReadOnly = true;
            theGrid.AllowUserToAddRows = false;
            theGrid.AllowUserToDeleteRows = false;
            theGrid.AllowUserToResizeRows = false;
            theGrid.AutoGenerateColumns = false;
            theGrid.RowHeadersWidth = 12;
            theGrid.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            theGrid.RowHeadersVisible = false;
            theGrid.BackgroundColor = SystemColors.Window;
            theGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            theGrid.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText;
            theGrid.EnableHeadersVisualStyles = true;

            theGrid.ScrollBars = ScrollBars.Horizontal;
            theGrid.Resize += new EventHandler( theGrid_Resize );

            // Uncomment the next 2 lines to see all the columns in default format;
            //theGrid.AutoGenerateColumns = true;
            //return;

            theGrid.ColumnCount = ColumnCount;
            theGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            for ( int i = 0; i < ColumnCount; i++ )
            {
                DataGridViewColumn col = theGrid.Columns[ i ];
                DisplayRule rule = Grid.DisplayRules[ i ];
                col.Name = rule.DataPropertyName;
                col.DataPropertyName = rule.DataPropertyName;
                col.DisplayIndex = rule.DisplayIndex;
                //				col.Width				= rule.Width;
                col.MinimumWidth = rule.Width;
                col.DefaultCellStyle.Format = rule.Format;
                col.HeaderText = rule.HeaderText;
                col.ToolTipText = rule.ToolTipText;
                col.Visible = rule.Visible;
                col.Frozen = rule.Frozen;
                col.DefaultCellStyle.Alignment = rule.Alignment;
                if ( rule.HeaderText == "#" )
                {
                    col.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                    col.Width = rule.Width;
                }
            }


            if ( gridType == GridType.StandardView )
            {
                theGrid.CellFormatting += new DataGridViewCellFormattingEventHandler( Grid_CellFormatting );
            }

            theGrid.Update( );
        }


        void Grid_CellFormatting( object sender, DataGridViewCellFormattingEventArgs e )
        {

            //			e.CellStyle.BackColor = SystemColors.ButtonFace;
            //			e.CellStyle.ForeColor = SystemColors.GrayText;
            Double val = 1;
            try
            {
                if ( theGrid[ "AverageReadsPerCycle", e.RowIndex ].Value != null && theGrid[ "AverageReadsPerCycle", e.RowIndex ].Value.GetType( ) == typeof( Double ) )
                {
                    val = ( Double ) theGrid[ "AverageReadsPerCycle", e.RowIndex ].Value;
                }
                else
                {
                    if ( theGrid[ "SimpleAverage", e.RowIndex ].Value != null )
                    {
                        val = ( Double ) theGrid[ "SimpleAverage", e.RowIndex ].Value;
                    }
                }
            }
            catch ( Exception e2 )
            {
                System.Diagnostics.Debug.Assert( false, e2.Message );
            }
            Color c = GetColorForAverage( val );
            e.CellStyle.SelectionBackColor = c;
            e.CellStyle.SelectionForeColor = Color.Black;
            e.CellStyle.BackColor = c;
            e.CellStyle.ForeColor = Color.Black;
        }


        void theGrid_Resize( object sender, EventArgs e )
        {
            UpdateNow( );
        }


        public string Label
        {
            set
            {
                captionLabel.Text = value;
            }
        }


    } // public partial class GridControl : UserControl


    interface IGridControlPager
    {
        object DataFile { get; set; }
        object GetData( int forPage );
        object GetPages( );
    }

    public class GridControlPager<T> : BindingList<T>, IGridControlPager where T : DatabaseRowTemplate, new( )
    {

        public static Type GetPagerForClass( GridControl.GridType grid )
        {
            switch ( grid )
            {
                case GridControl.GridType.StandardView:
                    return typeof( GridControlPager<TagInventory> );

                case GridControl.GridType.ReaderRequests:
                    return typeof( GridControlPager<ReaderRequest> );

                case GridControl.GridType.RawPackets:
                    return typeof( GridControlPager<PacketStream> );

                case GridControl.GridType.ReaderCommands:
                    return typeof( GridControlPager<ReaderCommand> );

                case GridControl.GridType.ReaderAntennaCycles:
                    return typeof( GridControlPager<ReaderAntennaCycle> );

                case GridControl.GridType.AntennaPacket:
                    return typeof( GridControlPager<AntennaPacket> );

                case GridControl.GridType.InventoryCycle:
                    return typeof( GridControlPager<InventoryCycle> );

                case GridControl.GridType.InventoryRounds:
                    return typeof( GridControlPager<InventoryRound> );

                case GridControl.GridType.InventoryParameters:
                    return typeof( GridControlPager<InventoryRound> );

                case GridControl.GridType.BadPackets:
                    return typeof( GridControlPager<BadPacket> );

                case GridControl.GridType.InventoryCycleDiag:
                    return typeof( GridControlPager<InventoryCycle> );

                case GridControl.GridType.InventoryRoundDiag:
                    return typeof( GridControlPager<InventoryRound> );

                case GridControl.GridType.TagAccess:
                    return typeof( GridControlPager<TagRead> );

                case GridControl.GridType.TagDataDiagnostics:
                    return typeof( GridControlPager<TagRead> );

                case GridControl.GridType.ReadRate:
                    return typeof( GridControlPager<ReadRate> );

                default:
                    throw new InvalidEnumArgumentException( String.Format( "GridControl.GridType grid={0:f}", grid ), ( int ) grid, grid.GetType( ) );
            }

        }


        //		private SequentialDataFile<T> _dataFile;
        private Object _dataFile;
        private GridControl _gridControl;


        public Object DataFile
        {
            get { return _dataFile; }
            set
            {
                if ( value is SequentialDataFile<T> )
                    _dataFile = value as SequentialDataFile<T>;
                else
                    if ( value is DataFile<T> )
                        _dataFile = value as DataFile<T>;
            }
        }


        public GridControl GridControl
        {
            get { return _gridControl; }
            private set { _gridControl = value; }
        }

        private int DisplayRowCount
        {
            get
            {
                if ( GridControl == null ||
                    GridControl.IsDisposed ||
                    GridControl.Parent == null ||
                    GridControl.Parent.Parent == null )
                    return 0;

                int h = GridControl.Parent.Parent.Height;
                int val = ( int ) Math.Truncate( ( ( Double ) GridControl.Parent.Parent.Height ) / 22.0 ) - 5;
                //GridControl.Label = String.Format("Height={0}, Rows={1}", h, val);
                return Math.Max( 0, val );
            }
        }

        int RowCount
        {
            get
            {
                int dataCount = 0;

                if ( _dataFile is SequentialDataFile<T> )
                    dataCount = ( ( SequentialDataFile<T> ) _dataFile ).Count;

                if ( _dataFile is DataFile<T> )
                    dataCount = ( ( DataFile<T> ) _dataFile ).Count;

                return Math.Min( dataCount, DisplayRowCount );
            }
        }


        public int PageCount
        {
            get
            {
                if ( _dataFile == null || RowCount == 0 )
                    return 0;

                if ( _dataFile is SequentialDataFile<T> )
                {
                    if ( ( ( SequentialDataFile<T> ) _dataFile ).Count == 0 )
                        return 0;

                    return ( int ) ( Math.Ceiling( ( double ) ( ( SequentialDataFile<T> ) _dataFile ).Count / ( double ) RowCount ) );
                }

                if ( _dataFile is DataFile<T> )
                {
                    if ( ( ( DataFile<T> ) _dataFile ).Count == 0 )
                        return 0;

                    return ( int ) ( Math.Ceiling( ( double ) ( ( DataFile<T> ) _dataFile ).Count / ( double ) RowCount ) );
                }
                return 0;
            }
        }

        public object GetData( int forPage )
        {
            Clear( );
            if ( _dataFile != null )
            {
                int dataCount = 0;

                if ( _dataFile is SequentialDataFile<T> )
                {
                    if ( ( ( SequentialDataFile<T> ) _dataFile ).IsDisposing )
                        return null;
                    dataCount = ( ( SequentialDataFile<T> ) _dataFile ).Count;
                }

                if ( _dataFile is DataFile<T> )
                {
                    if ( ( ( DataFile<T> ) _dataFile ).IsDisposing )
                        return null;
                    dataCount = ( ( DataFile<T> ) _dataFile ).Count;
                }

                int start = forPage * RowCount;
                int end = Math.Min( start + RowCount, dataCount );
                for ( int i = start; i < end; i++ )
                {
                    //T record = new T();
                    if ( _dataFile is SequentialDataFile<T> )
                        Add( ( T ) ( ( SequentialDataFile<T> ) _dataFile )[ i ] );
                    else
                        if ( _dataFile is DataFile<T> )
                            Add( ( T ) ( ( DataFile<T> ) _dataFile )[ i ] );
                }
            }
            return this;
        }

        public object GetPages( )
        {
            return
                _dataFile == null ||
                ( _dataFile is SequentialDataFile<T> && ( ( SequentialDataFile<T> ) _dataFile ).Count == 0 ) ||
                ( _dataFile is DataFile<T> && ( ( DataFile<T> ) _dataFile ).Count == 0 )
                ? null : new int[ PageCount ];
        }



        public GridControlPager( GridControl gridControl )
        {
            GridControl = gridControl;
        }

    }

}
