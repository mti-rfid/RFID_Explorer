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
 * $Id: SummaryControl.cs,v 1.3 2009/09/03 20:23:18 dshaheen Exp $
 * 
 * Description: 
 *
 *****************************************************************************
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace RFID_Explorer
{
	public partial class SummaryControl : UserControl
	{

		private enum DisplayObjectID
		{
			TitleLabel,
			SessionHeaderLabel,
			CommandHeaderLabel,
			CurrentHeaderLabel,
			DurationLabel,
			SessionDurationValue,
			CommandDurationValue,
			CurrentDurationValue,
			PacketsLabel,
			SessionPacketsValue,
			CommandPacketsValue, 
			CurrentPacketsValue,
			AntennaCyclesLabel,
			SessionAntennaCycleValue,
			CommandAntennaCycleValue,
			CurrentAntennaCycleValue,
			InventoryCyclesLabel,
			SessionInventoryCycleValue,
			CommandInventoryCycleValue,
			CurrentInventoryCycleValue,
			InventoryRoundLabel,
			SessionInventoryRoundValue,
			CommandInventoryRoundValue,
			CurrentInventoryRoundValue,
			TotalReadLabel,
			SessionTotalReadValue,
			CommandTotalReadValue,
			CurrentTotalReadValue,
			VR_SessionTotalReadValue,
			VR_CommandTotalReadValue,
			VR_CurrentTotalReadValue,
			UniqueTagsLabel,
			SessionUniqueTagsValue,
			CommandUniqueTagsValue,
			CurrentUniqueTagsValue,
			VR_Session_UniqueTagReads,
			VR_Command_UniqueTagReads,
			VR_Current_UniqueTagReads,
			PerSecLabelLabel,
			SessionPerSecLabelValue,
			CommandPerSecLabelValue,
			CurrentPerSecLabelValue,
			VR_Session_ReadsPerSecond,
			VR_Command_ReadsPerSecond,
			VR_Current_ReadsPerSecond,
			BadPacketsLabel,
			SessionBadPacketsValue,
			CommandBadPacketsValue,
			CurrentBadPacketsValue,
			LbtErrorsLabel,
			SessionLbtErrorsValue,
			CommandLbtErrorsValue,
			CurrentLbtErrorsValue,
			CrcErrorsLabel,
			SessionCrcErrorsValue,
			CommandCrcErrorsValue,
			CurrentCrcErrorsValue,			
}


		private abstract class DisplayObject
		{
			public abstract float Row { get;}
			public abstract float Col { get;}
			public abstract void Render(String[] data, Graphics graphics, RectangleF r, Brush bgBrush);
			
		}

		private class TextDisplayObject : DisplayObject
		{
			public static Font TitleFont = null;
			public static Font LabelFont = null;
			public static Font ValueFont = null;
			public static int ColWidth = 200;
			public static int WidthPadding = 2;
			public static int RowHeight = 30;
			public static int HeightPadding = 2;
			public static TextDisplayObject[] StaticLablesList = null;



			static TextDisplayObject()
			{
				FontFamily verdanaFontFamily = new FontFamily("Verdana");
				FontFamily arialFontFamily = new FontFamily("Arial");
				FontFamily sansSerifFontFamily = new FontFamily("Microsoft Sans Serif");


				TitleFont = new Font(verdanaFontFamily,		12.0f, FontStyle.Regular, GraphicsUnit.Point);
				LabelFont = new Font(arialFontFamily,		14.0f, FontStyle.Regular, GraphicsUnit.Pixel);
				ValueFont = new Font(sansSerifFontFamily,	8.25f, FontStyle.Regular, GraphicsUnit.Point);

				List<TextDisplayObject>  lst = new List<TextDisplayObject>(255);
				foreach (int id in Enum.GetValues(typeof(DisplayObjectID)))
				{
					DisplayObjectID ID = (DisplayObjectID)id;
					switch (ID)
					{
						case DisplayObjectID.TitleLabel:
							lst.Add(new TextDisplayObject(ID, "Summary View", TitleFont, 0, 0));
							break;

						case DisplayObjectID.SessionHeaderLabel:
							lst.Add(new TextDisplayObject(ID, "S E S S I O N", LabelFont, 1, 1));
							break;
					
						case DisplayObjectID.CommandHeaderLabel:
							lst.Add(new TextDisplayObject(ID, "C O M M A N D", LabelFont, 1, 2));
							break;

						case DisplayObjectID.CurrentHeaderLabel:
							lst.Add(new TextDisplayObject(ID, "C U R R E N T", LabelFont, 1, 3));
							break;

						case DisplayObjectID.DurationLabel:
							lst.Add(new TextDisplayObject(ID, "Duration", LabelFont, 2, 0, StringAlignment.Far));
							break;

						case DisplayObjectID.SessionDurationValue:
							lst.Add(new TextDisplayObject(ID, DataValues.Session_Duration, ValueFont, 2, 1));
							break;

						case DisplayObjectID.CommandDurationValue:
							lst.Add(new TextDisplayObject(ID, DataValues.Command_Duration, ValueFont, 2, 2));
							break;

						case DisplayObjectID.CurrentDurationValue:
							lst.Add(new TextDisplayObject(ID, DataValues.Current_Duration, ValueFont, 2, 3));
							break;

						case DisplayObjectID.PacketsLabel:
							lst.Add(new TextDisplayObject(ID, "Packets recieved", LabelFont, 3, 0, StringAlignment.Far));
							break;

						case DisplayObjectID.SessionPacketsValue:
							lst.Add(new TextDisplayObject(ID, DataValues.Session_Packets, ValueFont, 3, 1));
							break;

						case DisplayObjectID.CommandPacketsValue:
							lst.Add(new TextDisplayObject(ID, DataValues.Command_Packets, ValueFont, 3, 2));
							break;

						case DisplayObjectID.CurrentPacketsValue:
							lst.Add(new TextDisplayObject(ID, DataValues.Current_Packets, ValueFont, 3, 3));
							break;

						case DisplayObjectID.AntennaCyclesLabel:
							lst.Add(new TextDisplayObject(ID, "Antenna cycles", LabelFont, 4, 0, StringAlignment.Far));
							break;

						case DisplayObjectID.SessionAntennaCycleValue:
							lst.Add(new TextDisplayObject(ID, DataValues.Session_AntennaCycleCount, ValueFont, 4, 1));
							break;

						case DisplayObjectID.CommandAntennaCycleValue:
							lst.Add(new TextDisplayObject(ID, DataValues.Command_AntennaCycleCount, ValueFont, 4, 2));
							break;

						case DisplayObjectID.CurrentAntennaCycleValue:
							lst.Add(new TextDisplayObject(ID, DataValues.Current_AntennaCycleCount, ValueFont, 4, 3));
							break;

						case DisplayObjectID.InventoryCyclesLabel:
							lst.Add(new TextDisplayObject(ID, "Inventory cycles", LabelFont, 5, 0, StringAlignment.Far));
							break;

						case DisplayObjectID.SessionInventoryCycleValue:
							lst.Add(new TextDisplayObject(ID, DataValues.Session_InventoryCycleCount, ValueFont, 5, 1));
							break;

						case DisplayObjectID.CommandInventoryCycleValue:
							lst.Add(new TextDisplayObject(ID, DataValues.Command_InventoryCycleCount, ValueFont, 5, 2));
							break;

						case DisplayObjectID.CurrentInventoryCycleValue:
							lst.Add(new TextDisplayObject(ID, DataValues.Current_AntennaCycleCount, ValueFont, 5, 3));
							break;

						case DisplayObjectID.InventoryRoundLabel:
							lst.Add(new TextDisplayObject(ID, "Inventory rounds", LabelFont, 6, 0, StringAlignment.Far));
							break;

						case DisplayObjectID.SessionInventoryRoundValue:
							lst.Add(new TextDisplayObject(ID, DataValues.Session_RoundCount, ValueFont, 6, 1));
							break;

						case DisplayObjectID.CommandInventoryRoundValue:
							lst.Add(new TextDisplayObject(ID, DataValues.Command_RoundCount, ValueFont, 6, 2));
							break;

						case DisplayObjectID.CurrentInventoryRoundValue:
							lst.Add(new TextDisplayObject(ID, DataValues.Current_RoundCount, ValueFont, 6, 3));
							break;
			
						case DisplayObjectID.TotalReadLabel:
							lst.Add(new TextDisplayObject(ID, "Total singulations", LabelFont, 8, 0, StringAlignment.Far));
							break;
						
						case DisplayObjectID.SessionTotalReadValue:
							lst.Add(new TextDisplayObject(ID, DataValues.Session_TotalTagReads, ValueFont, 8, 1));
							break;

						case DisplayObjectID.CommandTotalReadValue:
							lst.Add(new TextDisplayObject(ID, DataValues.Command_TotalTagReads, ValueFont, 8, 2));
							break;

						case DisplayObjectID.CurrentTotalReadValue:
							lst.Add(new TextDisplayObject(ID, DataValues.Current_TotalTagReads, ValueFont, 8, 3));
							break;

						case DisplayObjectID.VR_SessionTotalReadValue:
								lst.Add(new TextDisplayObject(ID, DataValues.VR_SessionTotalReadValue, ValueFont, 8.5f, 1.2f));
							break;

						case DisplayObjectID.VR_CommandTotalReadValue:
								lst.Add(new TextDisplayObject(ID, DataValues.VR_CommandTotalReadValue, ValueFont, 8.5f, 2.2f));
							break;

						case DisplayObjectID.VR_CurrentTotalReadValue:
								lst.Add(new TextDisplayObject(ID, DataValues.VR_CurrentTotalReadValue, ValueFont, 8.5f, 3.2f));
							break;

						case DisplayObjectID.UniqueTagsLabel:
							lst.Add(new TextDisplayObject(ID, "Unique singulations", LabelFont, 9, 0, StringAlignment.Far));
							break;
						
						case DisplayObjectID.SessionUniqueTagsValue:
							lst.Add(new TextDisplayObject(ID, DataValues.Session_UniqueTagReads, ValueFont, 9, 1));
							break;

						case DisplayObjectID.CommandUniqueTagsValue:
							lst.Add(new TextDisplayObject(ID, DataValues.Command_UniqueTagReads, ValueFont, 9, 2));
							break;

						case DisplayObjectID.CurrentUniqueTagsValue:
							lst.Add(new TextDisplayObject(ID, DataValues.Current_UniqueTagReads, ValueFont, 9, 3));
							break;

						case DisplayObjectID.VR_Session_UniqueTagReads:
							lst.Add(new TextDisplayObject(ID, DataValues.VR_Session_UniqueTagReads, ValueFont, 9.5f, 1.2f));
							break;

						case DisplayObjectID.VR_Command_UniqueTagReads:
							lst.Add(new TextDisplayObject(ID, DataValues.VR_Command_UniqueTagReads, ValueFont, 9.5f, 2.2f));
							break;

						case DisplayObjectID.VR_Current_UniqueTagReads:
							lst.Add(new TextDisplayObject(ID, DataValues.VR_Current_UniqueTagReads, ValueFont, 9.5f, 3.2f));
							break;

						case DisplayObjectID.PerSecLabelLabel:
							lst.Add(new TextDisplayObject(ID, "Singulations / second", LabelFont, 11, 0, StringAlignment.Far));
							break;
						
						case DisplayObjectID.SessionPerSecLabelValue:
							lst.Add(new TextDisplayObject(ID, DataValues.Session_ReadsPerSecond, ValueFont, 11, 1));
							break;

						case DisplayObjectID.CommandPerSecLabelValue:
							lst.Add(new TextDisplayObject(ID, DataValues.Command_ReadsPerSecond, ValueFont, 11, 2));
							break;

						case DisplayObjectID.CurrentPerSecLabelValue:
							lst.Add(new TextDisplayObject(ID, DataValues.Current_ReadsPerSecond, ValueFont, 11, 3));
							break;

						case DisplayObjectID.VR_Session_ReadsPerSecond:
							lst.Add(new TextDisplayObject(ID, DataValues.VR_Session_ReadsPerSecond, ValueFont, 11.5f, 1.2f));
							break;

						case DisplayObjectID.VR_Command_ReadsPerSecond:
							lst.Add(new TextDisplayObject(ID, DataValues.VR_Command_ReadsPerSecond, ValueFont, 11.5f, 2.2f));
							break;

						case DisplayObjectID.VR_Current_ReadsPerSecond:
							lst.Add(new TextDisplayObject(ID, DataValues.VR_Current_ReadsPerSecond, ValueFont, 11.5f, 3.2f));
							break;

						case DisplayObjectID.BadPacketsLabel:
							lst.Add(new TextDisplayObject(ID, "Bad packets", LabelFont, 13, 0, StringAlignment.Far));
							break;

						case DisplayObjectID.SessionBadPacketsValue:
							lst.Add(new TextDisplayObject(ID, DataValues.Session_BadPackets, ValueFont, 13, 1));
							break;

						case DisplayObjectID.CommandBadPacketsValue:
							lst.Add(new TextDisplayObject(ID, DataValues.Command_BadPackets, ValueFont, 13, 2));
							break;

						case DisplayObjectID.CurrentBadPacketsValue:
							lst.Add(new TextDisplayObject(ID, DataValues.Current_BadPackets, ValueFont, 13, 3));
							break;
						
						case DisplayObjectID.LbtErrorsLabel:
							lst.Add(new TextDisplayObject(ID, "LBT errors", LabelFont, 14, 0, StringAlignment.Far));
							break;

						case DisplayObjectID.SessionLbtErrorsValue:
							break;
						
						case DisplayObjectID.CommandLbtErrorsValue:
							break;
						
						case DisplayObjectID.CurrentLbtErrorsValue:
							break;
						
						case DisplayObjectID.CrcErrorsLabel:
							lst.Add(new TextDisplayObject(ID, "CRC errors", LabelFont, 15, 0, StringAlignment.Far));
							break;

						case DisplayObjectID.SessionCrcErrorsValue:
							lst.Add(new TextDisplayObject(ID, DataValues.Session_CRCErrors, ValueFont, 15, 1));
							break;
						
						case DisplayObjectID.CommandCrcErrorsValue:
							lst.Add(new TextDisplayObject(ID, DataValues.Command_CRCErrors, ValueFont, 15, 2));
							break;
						
						case DisplayObjectID.CurrentCrcErrorsValue:
							lst.Add(new TextDisplayObject(ID, DataValues.Current_CRCErrors, ValueFont, 15, 3));
							break;

						default:
							System.Diagnostics.Debug.Assert(false);
							break;
					}

				}
				
				StaticLablesList = lst.ToArray();
			}


			public static void RenderAll(String[] values, Graphics graphics, Rectangle rect, Color fgColor, Color bgColor)
			{
				graphics.Clear(bgColor);

				using (Brush fgBrush = new SolidBrush(fgColor))
				{
					foreach (TextDisplayObject obj in StaticLablesList)
					{
						RectangleF r = new RectangleF((obj.Col * ColWidth) + WidthPadding, (obj.Row * RowHeight) + HeightPadding, ColWidth,  RowHeight);
						if (obj.Row > 0 && obj.Col == 0) r.Width = ColWidth * .8f;
						obj.Render(values, graphics, r, fgBrush);
					}
				}
			}

			private DisplayObjectID _id;
			private string _label;
			private int _index;
			private Font _font;
			private float _row;
			private float _col;
			private StringFormat _format = new StringFormat();

			private TextDisplayObject(DisplayObjectID ID, String label, Font font, float row, float col)
			{
				_id		= ID;
				_font	= font;
				_label	= label;
				_row	= row;
				_col	= col;
			}

			private TextDisplayObject(DisplayObjectID ID, String label, Font font, float row, float col, StringAlignment alignment)
			{
				_id = ID;
				_font = font;
				_label = label;
				_row = row;
				_col = col;
				_format.Alignment = alignment;
			}

			private TextDisplayObject(DisplayObjectID ID, int index, Font font, float row, float col)
			{
				_id		= ID;
				_font	= font;
				_index = index;
				_label = null;
				_row	= row;
				_col	= col;
			}

			public override string ToString()
			{
				return _label;
			}

			public override void Render(String[] data, Graphics buffer, RectangleF r, Brush bgBrush)
			{
				if (String.IsNullOrEmpty(_label))
				{
					buffer.DrawString(data[_index], _font, bgBrush, r, _format);
				}
				else
				{
					buffer.DrawString(_label, _font, bgBrush, r, _format);
				}
			}

			public override float Row
			{
				get { return _row; }
			}

			public override float Col
			{
				get { return _col; }
			}
		}


		private static class DataValues
		{
			public const int Session_Duration = 0;
			public const int Command_Duration = 1;
			public const int Current_Duration = 2;

			public const int Session_Packets = 3;
			public const int Command_Packets = 4;
			public const int Current_Packets = 5;

			public const int Session_AntennaCycleCount = 6;
			public const int Command_AntennaCycleCount = 7;
			public const int Current_AntennaCycleCount = 8;

			public const int Session_InventoryCycleCount = 9;
			public const int Command_InventoryCycleCount = 10;
			public const int Current_InventoryCycleCount = 11;

			public const int Session_RoundCount = 12;
			public const int Command_RoundCount = 13;
			public const int Current_RoundCount = 14;

			public const int Session_TotalTagReads = 15;
			public const int Command_TotalTagReads = 16;
			public const int Current_TotalTagReads = 17;

			public const int Session_UniqueTagReads = 18;
			public const int Command_UniqueTagReads = 19;
			public const int Current_UniqueTagReads = 20;

			public const int Session_ReadsPerSecond = 21;
			public const int Command_ReadsPerSecond = 22;
			public const int Current_ReadsPerSecond = 23;

			public const int Session_BadPackets = 24;
			public const int Command_BadPackets = 25;
			public const int Current_BadPackets = 26;

			public const int Session_CRCErrors = 27;
			public const int Command_CRCErrors = 28;
			public const int Current_CRCErrors = 29;

			public const int VR_SessionTotalReadValue = 30;
			public const int VR_CommandTotalReadValue = 31;
			public const int VR_CurrentTotalReadValue = 32;

			public const int VR_Session_UniqueTagReads = 33;
			public const int VR_Command_UniqueTagReads = 34;
			public const int VR_Current_UniqueTagReads = 35;

			public const int VR_Session_ReadsPerSecond = 36;
			public const int VR_Command_ReadsPerSecond = 37;
			public const int VR_Current_ReadsPerSecond = 38;

			public const int TotalValues = 39;
		}


		public SummaryControl()
		{
			Values = new String[DataValues.TotalValues];
			InitializeComponent();

			this.Dock = DockStyle.Fill;
		}

		
		private static Color			_DefaultFGColor = SystemColors.ControlText;
		private static Color			_DefaultBGColor = SystemColors.Control;

		private Rectangle				_drawingRect = Rectangle.Empty;
		private BufferedGraphics		_gxBuffer	= null;
		private BufferedGraphicsContext _gxContext	= null;
		private String[]				Values		= null;


		private Rectangle DrawingRectangle
		{
			get { return _drawingRect; }
			set { _drawingRect = value; }
		}

	

		public TimeSpan Session_Duration { set { Values[DataValues.Session_Duration] = String.Format(RFID.RFIDInterface.DurationFormat.Formatter, "{0:c}", value); } }
		public TimeSpan Command_Duration { set { Values[DataValues.Command_Duration] = String.Format(RFID.RFIDInterface.DurationFormat.Formatter, "{0:c}", value); } }
		public TimeSpan Current_Duration { set { Values[DataValues.Current_Duration] = String.Format(RFID.RFIDInterface.DurationFormat.Formatter, "{0:c}", value); } }

		public int Session_Packets { set { Values[DataValues.Session_Packets] = String.Format("{0:n0}", value); } }
		public int Command_Packets { set { Values[DataValues.Command_Packets] = String.Format("{0:n0}", value); } }
		public int Current_Packets { set { Values[DataValues.Current_Packets] = String.Format("{0:n0}", value); } }

		public int Session_AntennaCycleCount { set { Values[DataValues.Session_AntennaCycleCount] = String.Format("{0:n0}", value); } }
		public int Command_AntennaCycleCount { set { Values[DataValues.Command_AntennaCycleCount] = String.Format("{0:n0}", value); } }
		public int Current_AntennaCycleCount { set { Values[DataValues.Current_AntennaCycleCount] = String.Format("{0:n0}", value); } }

		public int Session_InventoryCycleCount { set { Values[DataValues.Session_InventoryCycleCount] = String.Format("{0:n0}", value); } }
		public int Command_InventoryCycleCount { set { Values[DataValues.Command_InventoryCycleCount] = String.Format("{0:n0}", value); } }
		public int Current_InventoryCycleCount { set { Values[DataValues.Current_InventoryCycleCount] = String.Format("{0:n0}", value); } }

		public int Session_RoundCount { set { Values[DataValues.Session_RoundCount] = String.Format("{0:n0}", value); } }
		public int Command_RoundCount { set { Values[DataValues.Command_RoundCount] = String.Format("{0:n0}", value); } }
		public int Current_RoundCount { set { Values[DataValues.Current_RoundCount] = String.Format("{0:n0}", value); } }

		public int Session_TotalTagReads { set { Values[DataValues.Session_TotalTagReads] = String.Format("{0:n0}", value); } }
		public int Command_TotalTagReads { set { Values[DataValues.Command_TotalTagReads] = String.Format("{0:n0}", value); } }
		public int Current_TotalTagReads { set { Values[DataValues.Current_TotalTagReads] = String.Format("{0:n0}", value); } }

		public int Session_UniqueTagReads { set { Values[DataValues.Session_UniqueTagReads] = String.Format("{0}", value); } }
		public int Command_UniqueTagReads { set { Values[DataValues.Command_UniqueTagReads] = String.Format("{0}", value); } }
		public int Current_UniqueTagReads { set { Values[DataValues.Current_UniqueTagReads] = String.Format("{0}", value); } }

		public float Session_ReadsPerSecond { set { Values[DataValues.Session_ReadsPerSecond] = String.Format("\u0305x\u0305  =  {0:n1}", value); } }
		public float Command_ReadsPerSecond { set { Values[DataValues.Command_ReadsPerSecond] = String.Format("{0:n1}", value); } }
		public float Current_ReadsPerSecond { set { Values[DataValues.Current_ReadsPerSecond] = String.Format("{0:n1}", value); } }

		public int Session_BadPackets { set { Values[DataValues.Session_BadPackets] = String.Format("{0}", value); } }
		public int Command_BadPackets { set { Values[DataValues.Command_BadPackets] = String.Format("{0}", value); } }
		public int Current_BadPackets { set { Values[DataValues.Current_BadPackets] = String.Format("{0}", value); } }

		public int Session_CRCErrors { set { Values[DataValues.Session_CRCErrors] = String.Format("{0}", value); } }
		public int Command_CRCErrors { set { Values[DataValues.Command_CRCErrors] = String.Format("{0}", value); } }
		public int Current_CRCErrors { set { Values[DataValues.Current_CRCErrors] = String.Format("{0}", value); } }

		public string VR_SessionTotalReadValue { set {Values[DataValues.VR_SessionTotalReadValue] = value;	} }
		public string VR_CommandTotalReadValue { set {Values[DataValues.VR_CommandTotalReadValue] = value;	} }
		public string VR_CurrentTotalReadValue { set { Values[DataValues.VR_CurrentTotalReadValue] = value; } }

		public string VR_Session_UniqueTagReads { set { Values[DataValues.VR_Session_UniqueTagReads] = value; } }
		public string VR_Command_UniqueTagReads { set { Values[DataValues.VR_Command_UniqueTagReads] = value; } }
		public string VR_Current_UniqueTagReads { set { Values[DataValues.VR_Current_UniqueTagReads] = value; } }

		public string VR_Session_ReadsPerSecond { set { Values[DataValues.VR_Session_ReadsPerSecond] = value; } }
		public string VR_Command_ReadsPerSecond { set { Values[DataValues.VR_Command_ReadsPerSecond] = value; } }
		public string VR_Current_ReadsPerSecond { set { Values[DataValues.VR_Current_ReadsPerSecond] = value; } }


		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			this.SetStyle(ControlStyles.OptimizedDoubleBuffer |
				ControlStyles.AllPaintingInWmPaint |
				ControlStyles.UserPaint, true);


			_gxContext = BufferedGraphicsManager.Current;
			Reset();

		}

		protected override void OnHandleDestroyed(EventArgs e)
		{
			if (_gxBuffer != null)
			{
				_gxBuffer.Dispose();
				_gxBuffer = null;
			}
			base.OnHandleDestroyed(e);
		}



		protected override void OnSizeChanged(EventArgs e)
		{
#if DEBUG
			System.Diagnostics.Debug.WriteLine(String.Format("{0}() ThreadID = {1}", System.Reflection.MethodInfo.GetCurrentMethod().Name, System.Threading.Thread.CurrentThread.ManagedThreadId));
#endif

			base.OnSizeChanged(e);

			Reset();

		}


		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);


			if (_gxBuffer == null)
			{
				e.Graphics.Clear(_DefaultBGColor);
			}
			else
			{
				RenderAll();
				_gxBuffer.Render(e.Graphics);
			}
			
		}

		private void RenderAll()
		{
			TextDisplayObject.RenderAll(Values, _gxBuffer.Graphics, DrawingRectangle, _DefaultFGColor, _DefaultBGColor);
		}

		private void Reset()
		{
			DrawingRectangle = ClientRectangle;

			if (_gxContext != null && DrawingRectangle.Width > 0)
			{
				_gxContext.MaximumBuffer = Rectangle.Inflate(DrawingRectangle, 1, 1).Size;
				if (_gxBuffer != null)
				{
					_gxBuffer.Dispose();
				}
				_gxBuffer = _gxContext.Allocate(this.CreateGraphics(), DrawingRectangle);
			}
			Invalidate();
		}

		public void Clear()
		{
			Values = new String[DataValues.TotalValues];
			Invalidate();
			return;

		}

        private void SummaryControl_Load( object sender, EventArgs e )
        {

        }


		
		
	}
}
