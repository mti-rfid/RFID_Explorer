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
 * $Id: ProtocolControl.cs,v 1.3 2009/09/03 20:23:17 dshaheen Exp $
 * 
 * Description: 
 *
 *****************************************************************************
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data;
using System.Text;
using System.Windows.Forms;

using RFID.RFIDInterface;
using System.Collections;

namespace RFID_Explorer
{
	public partial class ProtocolControl : UserControl
	{
/*

		private class RenderObject
		{
			private PacketData.PacketWrapper _packet;
//			private int  _startTime;
//			private long _endTime;
			private RectangleF outlineRectangle;
			public Pen outlinePen = Pens.Red;
			public Brush bgBrush = Brushes.White;
			public Brush fillBrush = Brushes.PaleGoldenrod;
			public float lastDrawTime;
			public Region objectRegion;
			public Form frm;
			public RenderObject(PacketData.PacketWrapper envelope, RectangleF rect, Form form)
			{
				frm = form;
				_packet = envelope;
				outlineRectangle = rect;
				objectRegion = new Region(rect);
			}

			public void erase(Graphics graphics)
			{
				graphics.FillRectangle(bgBrush, outlineRectangle);
				
			}

			public void draw(Graphics graphics, float asOfTime)
			{
				lastDrawTime = asOfTime;
//				erase(graphics);
//				RectangleF rect = new RectangleF(outlineRectangle.X, outlineRectangle.Y, outlineRectangle.Width, outlineRectangle.Height);
				RectangleF rect = outlineRectangle;
				rect.Offset(asOfTime * pixelsPerSecond, 0f);

				graphics.FillRectangle(fillBrush, rect);
//				graphics.DrawRectangle(outlinePen, outlineRectangle);
				using (Pen topPen = new Pen(SystemColors.ControlDark), 
					bottomPen = new Pen(SystemColors.ControlDarkDark))
				
				{
//					graphics.DrawRectangle(bottomPen, outlineRectangle);
					graphics.DrawLine(topPen,		rect.Left, rect.Top,	rect.Right, rect.Top);
					graphics.DrawLine(topPen,		rect.Left, rect.Top,	rect.Left,	rect.Bottom);
					graphics.DrawLine(bottomPen,	rect.Left, rect.Bottom, rect.Right, rect.Bottom);
					graphics.DrawLine(bottomPen,	rect.Right, rect.Bottom,rect.Right, rect.Top);
					graphics.DrawLine(Pens.Black, rect.Left, rect.Top + rect.Height / 2, rect.Right, rect.Top + rect.Height /2);

				}
			}

			

		}

		private class RenderObjectList : List<RenderObject>
		{
			public static RenderObjectList ObjectList = new RenderObjectList();

			public static int ObjectCount
			{
				get { return ObjectList.Count; }
			}

			public static void DrawAll(Graphics graphics, Rectangle rect, float asOfTime)
			{
				foreach (RenderObject o in ObjectList)
					o.draw(graphics, asOfTime);
			}			

		}

		private static class Background
		{


			private static void DrawOutline(Graphics graphics, Rectangle drawingRect)
			{
				graphics.DrawRectangle(Pens.Black, Rectangle.Inflate(drawingRect, -3, -3));
			}

			private static void DrawXAxis(Graphics graphics, Rectangle drawingRect)
			{
				if (drawingRect == Rectangle.Empty)
				{
					System.Diagnostics.Debug.Assert(false);
					return;
				}

				Rectangle baselineRect = drawingRect;
				baselineRect.Offset(80, 28);
				baselineRect.Inflate(-80, -28);
			
//				graphics.FillRectangle(Brushes.Thistle, baselineRect);

				//graphics.DrawLine(Pens.BlueViolet, 3, baselineRect.Top, baselineRect.Width + baselineRect.Left - 3, baselineRect.Top);
				using (Pen baselinePen = new Pen(Color.DarkBlue, 2),
					tickPen = new Pen(Color.DarkBlue, 1))
				{
					graphics.DrawLine(baselinePen, 80, 40, bitmapWidth - 3, 40);
					for (int i = 80; i < bitmapWidth - 3; i+= pixelsPerSecond)
					{
						graphics.DrawLine(tickPen, i, 40, i, 35);
					}
				}
				
			}


			private static void DrawYAxis(Graphics graphics, Rectangle drawingRect)
			{
				graphics.DrawLine(Pens.DarkBlue, 80, 3, 80, drawingRect.Bottom - 3);
				Rectangle r = new Rectangle(7, 47, 70, 50);
//				graphics.DrawRectangle(Pens.Crimson, r);
//				graphics.DrawString("CMD", SystemFonts.SmallCaptionFont, Brushes.Black, r);
			}

			public static void Draw(Graphics graphics, Rectangle rect)
			{
				DrawOutline(graphics, rect);
				//DrawXAxis(graphics, rect);
				DrawYAxis(graphics, rect);
			}


		}
		*/

		public enum ControlEvent
		{
			DataUpdate,
			CaptureMode,
			ReivewMode,
			ContextChange,
			Closing,
			Freeze,
			Thaw,
			Clear,
			Load,
		}

		public ProtocolControl()
		{
			InitializeComponent();
			this.tracer.ActiveLabel = this.liveUpdateLabel;
			this.tracer.CaptionsCheckbox = this.captionsCheckBox;
			this.tracer.ScrollBar = this.tracerScrollBar;
			this.tracer.FreezeCheckbox = this.freezeCheckBox;

		}



		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			this.Parent.Dock = DockStyle.Fill;
			this.Dock = DockStyle.Fill;
			this.tracerScrollBar.Dock = DockStyle.Right;
			this.topPanel.Dock = DockStyle.Top;
			this.dataPanel.Dock = DockStyle.Fill;
			
		}



		public void Clear()
		{
			tracer.Clear();
		}

//		public void AddPacketBatch(String name, PacketArrayList packetList)
//		{
//			tracer.AddPacketBatch(name, packetList);
//		}

		public void StatusChange(String name, ControlEvent controlEvent, PacketArrayList packetList)
		{
			tracer.StatusChange(name, controlEvent, packetList);
		}

//		public void AddPacket(PacketData.PacketWrapper envelope)
//		{
//			tracer.AddPacket(envelope);
//		}
	


	}
}
