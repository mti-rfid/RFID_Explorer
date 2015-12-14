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
 * $Id: TracerControl.cs,v 1.10 2011/01/05 06:22:15 dciampi Exp $
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
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using RFID.RFIDInterface;

namespace RFID_Explorer
{
    public partial class TracerControl : UserControl
    {
//        private const int MAX_PACKTES         = 1024;
        private const int INTERNAL_LEFT_MARGIN  = 5;
        private const int INTERNAL_RIGHT_MARGIN = 15;
        private const int PACKET_RECT_HEIGHT    = 45;
        private const int ROW_SPACING           = 2;

        private static ContextDictionary Context = new ContextDictionary();

        private class ContextDictionary : Dictionary<String, TracerControlContext>
        {
            public Object ContextLock = new Object();

            public void Add(String name, Type type)
            {
                base.Add(name, new TracerControlContext(type));
            }
        }

        private class TracerControlContext
        {
            public int               _basePacketNumber = -1;
            public bool              _dataPending      = false;
            public PacketArrayList   _PacketList       = null;
            public PacketArrayList   _SavePacketList   = null;
            public ControlStateValue _controlState     = ControlStateValue.Review;

            public TracerControlContext(Type type)
            {
                _PacketList = new PacketArrayList(type);
            }
        }

        private String					_currentContextName	= null;
        public bool						_ignoreCheckChanged = false;
        private Timer					_timer				= new Timer();
        private Color					_DefaultFGColor		= SystemColors.ControlText;
        private Color					_DefaultBGColor		= SystemColors.Control;
        private Rectangle				_drawingRect		= Rectangle.Empty;
        private Control					_activeLabel		= null;
        private CheckBox				_captionsCheckbox	= null;
        private CheckBox				_freezeCheckbox		= null;
        private VScrollBar				_scrollBar			= null;
        private BufferedGraphics		_gxBuffer			= null;
        private BufferedGraphicsContext	_gxContext			= null;

        public enum ControlStateValue
        {
            Active,
            FrozenActive,
            Review,
            FrozenReview,
        }

        public TracerControl()
        {
            InitializeComponent();
            BackColor = SystemColors.Control;
        }

        public static Color InvertColor(Color c)
        {
            return ColorTranslator.FromWin32(~ColorTranslator.ToWin32(c));
        }

        private string CurrentContext
        {
            get { return _currentContextName; }
            set { _currentContextName = value; }
        }

        private Rectangle DrawingRectangle
        {
            get { return _drawingRect; }
            set { _drawingRect = value; }
        }

        public int PacketRectHeight
        {
            get { return ShowCaption ? PACKET_RECT_HEIGHT : (int)Math.Floor(PACKET_RECT_HEIGHT / 2.0); }
        }

        private Rectangle GetOuterRowRectangle()
        {
            return new Rectangle(INTERNAL_LEFT_MARGIN, 0, DrawingRectangle.Width - INTERNAL_RIGHT_MARGIN, PacketRectHeight);
        }

        public int VisibleRowCount
        {
            get { return DrawingRectangle.Height / PacketRectHeight; }
        }

        private Rectangle OffsetToRowPosition(Rectangle r, int row)
        {
            int location = row * PacketRectHeight;
            r.Offset(0, location);
            return r;
        }

        private Rectangle GetDrawingRowRectangle(Rectangle rowRectangle)
        {
            return Rectangle.Inflate(rowRectangle, -1, ShowCaption ? -ROW_SPACING : 0);
        }

        public bool ShowCaption
        {
            get { return _captionsCheckbox == null ? true : _captionsCheckbox.Checked; }
            set { if (_captionsCheckbox != null) _captionsCheckbox.Checked = !value; }
        }

        public Control ActiveLabel
        {
            get { return _activeLabel; }
            set { _activeLabel = value; }
        }

        public CheckBox CaptionsCheckbox
        {
            get { return _captionsCheckbox; }
            set
            {
                if (_captionsCheckbox != null)
                {
                    _captionsCheckbox.CheckedChanged -= CaptionsCheckbox_CheckedChanged;
                    _captionsCheckbox = null;
                }
                if (value != null)
                {
                    value.Checked = true;
                    value.CheckedChanged +=new EventHandler(CaptionsCheckbox_CheckedChanged);
                }
                _captionsCheckbox = value; 
            }
        }

        public CheckBox FreezeCheckbox
        {
            get { return _freezeCheckbox; }
            set
            {
                if (_freezeCheckbox != null)
                {
                    _freezeCheckbox.CheckedChanged -= FreezeCheckbox_CheckedChanged;
                    _freezeCheckbox = null;
                }
                if (value != null)
                {
                    _DefaultFGColor = value.ForeColor;
                    _DefaultBGColor = value.BackColor;
                    value.Checked = false;
                    value.CheckedChanged +=new EventHandler(FreezeCheckbox_CheckedChanged);
                }
                _freezeCheckbox = value; 
            }
        }

        public VScrollBar ScrollBar
        {
            get { return _scrollBar;}
            set 
            {
                if (_scrollBar != null)
                {
                    _scrollBar.Scroll -= ScrollBar_Scroll;
                    _scrollBar = null;
                }
                if (value != null)
                {
                    value.Scroll +=new ScrollEventHandler(ScrollBar_Scroll);
                    value.LargeChange = VisibleRowCount;
                    value.Minimum = 0;
                    //value.Maximum = Math.Max(0, GetPacketList(CurrentContext).Count - 1);
                }
                _scrollBar = value;
            }
        }

        public ControlStateValue GetControlState(String contextName)
        {
            if (!Context.ContainsKey(contextName))
            {
                throw new Exception(String.Format("{0} is not a known context.", contextName));
            }
            return Context[contextName]._controlState;
        }

        public void SetControlState(String contextName, ControlStateValue value)
        {
            if (!Context.ContainsKey(contextName))
            {
                throw new Exception(String.Format("{0} is not a known context.", contextName));
            }
            Context[contextName]._controlState = value;
        }

        private int GetBasePacketNumber(string contextName)
        {
            if (!Context.ContainsKey(contextName))
            {
                throw new Exception(String.Format("{0} is not a known context.", contextName));
            }
            return Context[contextName]._basePacketNumber;
        }

        private void SetBasePacketNumber(string contextName, int value)
        {
            if (!Context.ContainsKey(contextName))
            {
                throw new Exception(String.Format("{0} is not a known context.", contextName));
            }
            Context[contextName]._basePacketNumber = value;
        }

        private bool GetDataPending(string contextName)
        {
            if (!Context.ContainsKey(contextName))
            {
                throw new Exception(String.Format("{0} is not a known context.", contextName));
            }
            return Context[contextName]._dataPending;
        }

        private void SetDataPending(string contextName, bool value)
        {
            if (!Context.ContainsKey(contextName))
            {
                throw new Exception(String.Format("{0} is not a known context.", contextName));
            }
            Context[contextName]._dataPending = value;
        }

        private PacketArrayList GetPacketList(string contextName)
        {
            if (!Context.ContainsKey(contextName))
            {
                throw new Exception(String.Format("{0} is not a known context.", contextName));
            }
            return Context[contextName]._PacketList;
        }

        private void SetPacketList(string contextName, PacketArrayList value)
        {
            if (!Context.ContainsKey(contextName))
            {
                throw new Exception(String.Format("{0} is not a known context.", contextName));
            }
            Context[contextName]._PacketList = value;
        }

        private PacketArrayList GetSavedPacketList(string contextName)
        {
            if (!Context.ContainsKey(contextName))
            {
                throw new Exception(String.Format("{0} is not a known context.", contextName));
            }
            return Context[contextName]._SavePacketList;
        }

        private void SetSavedPacketList(string contextName, PacketArrayList value)
        {
            if (!Context.ContainsKey(contextName))
            {
                throw new Exception(String.Format("{0} is not a known context.", contextName));
            }
            Context[contextName]._SavePacketList = value;
        }

        void CaptionsCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (ScrollBar != null)
            ScrollBar.LargeChange = VisibleRowCount;
            Invalidate(DrawingRectangle);
        }

        void FreezeCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (_ignoreCheckChanged)
                return;

            if (CurrentContext == null)
                return;

            StatusChange(CurrentContext, FreezeCheckbox.Checked ? ProtocolControl.ControlEvent.Freeze : ProtocolControl.ControlEvent.Thaw, GetPacketList(CurrentContext));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// The value of a scroll bar cannot reach its maximum value through user interaction at run time. 
        /// The maximum value that can be reached is equal to the Maximum property value minus the LargeChange property value plus 1. 
        /// The maximum value can only be reached programmatically.
        /// </remarks>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
//            if (e.Type != ScrollEventType.EndScroll)
//            System.Diagnostics.Debug.Write(String.Format("Vis={0} N={1} Old={2} New={3} {4} Val={5} Max={6} Large={7} Magic={8}\n", VisibleRowCount, PacketList.Count, e.OldValue, e.NewValue, e.Type, ScrollBar._value, ScrollBar.Maximum, ScrollBar.LargeChange, ScrollBar.Maximum - ScrollBar.LargeChange + 1));

            if (GetPacketList(CurrentContext).Count == 0)
                return;

            if (GetControlState(CurrentContext) == ControlStateValue.Active)
            {
                //SetControlState(CurrentContext, ControlStateValue.FrozenActive);
                StatusChange(CurrentContext, ProtocolControl.ControlEvent.Freeze, GetPacketList(CurrentContext));
            }

            int val = e.NewValue + VisibleRowCount - 1;

            if (val > GetPacketList(CurrentContext).Count - 1)
            {
                val = GetPacketList(CurrentContext).Count - 1;
                ScrollBar.Value = Math.Max(0, GetPacketList(CurrentContext).Count - 1 - VisibleRowCount);
            }

            SetBasePacketNumber(CurrentContext, val);
            
            Invalidate(DrawingRectangle);
        }

        public void Clear()
        {
            if (CurrentContext == null)
            {
                return;
            }
            StatusChange(CurrentContext, ProtocolControl.ControlEvent.Clear, GetPacketList(CurrentContext));
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            this.SetStyle(ControlStyles.OptimizedDoubleBuffer |
                            ControlStyles.AllPaintingInWmPaint | 
                            ControlStyles.UserPaint, true);

            _gxContext = BufferedGraphicsManager.Current;
            _timer.Interval = 500;
            _timer.Tick +=new EventHandler(_Timer_Tick);
            _timer.Start();
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            if (_gxBuffer != null)
                _gxBuffer.Dispose();

            base.OnHandleDestroyed(e);
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);

            if (GetPacketList(CurrentContext).Count == 0)
                return;

            //LiveDisplay = false;
            if (GetControlState(CurrentContext) == ControlStateValue.Active)
            {
                SetControlState(CurrentContext, ControlStateValue.FrozenActive);
            }

            int val = e.Delta < 0 ? GetBasePacketNumber(CurrentContext) + 1 : GetBasePacketNumber(CurrentContext) - 1;
            if (val < 1)
                val = 1;

            if (val >= GetPacketList(CurrentContext).Count)
                val = GetPacketList(CurrentContext).Count - 1;

            ScrollBar.Maximum = Math.Max(0, GetPacketList(CurrentContext).Count - 1);
            ScrollBar.Value = Math.Max(0, val - VisibleRowCount);

            SetBasePacketNumber(CurrentContext, val);

            Invalidate(DrawingRectangle);
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

            if (_gxBuffer == null || Context.Count == 0 || CurrentContext == null)
            {
                e.Graphics.Clear(ColorScheme.BG);

            }
            else
            {
                if (e.ClipRectangle.Size.Height < DrawingRectangle.Size.Height && 
                    e.ClipRectangle.Size.Width  < DrawingRectangle.Size.Width)
                {
                    //_gxBuffer.Graphics.SetClip(e.ClipRectangle);
                    _gxBuffer.Render(e.Graphics);
                    return;
                }
                _gxBuffer.Graphics.Clear(ColorScheme.BG);
                
                if (GetPacketList(CurrentContext).Count == 0)
                {
                    using (Brush bgBrush = new SolidBrush(ColorScheme.BG),
                                 fgBrush = new SolidBrush(Color.FromArgb(128, InvertColor(ColorScheme.BG)))) 
                    {
                        string msg = " No Data ";
//                        _gxBuffer.Graphics.DrawRectangle(Pens.Magenta, Rectangle.Inflate(DrawingRectangle, -1, -1));
//                        _gxBuffer.Graphics.DrawString(DrawingRectangle.ToString(), SystemFonts.DialogFont, brush, DrawingRectangle);
                        using (Pen p = new Pen(fgBrush))
                        {
                            _gxBuffer.Graphics.DrawLine(Pens.DarkGray, 20, 40, DrawingRectangle.Right - 20, 40);
                        }
                        SizeF size = _gxBuffer.Graphics.MeasureString(msg, TextScheme.F1);
                        RectangleF textRect = new RectangleF(new PointF(DrawingRectangle.X + ((DrawingRectangle.Width - size.Width) / 2),
                                                                DrawingRectangle.Y + 40 - (size.Height / 2)), size);
                        _gxBuffer.Graphics.FillRectangle(bgBrush, textRect);
                        _gxBuffer.Graphics.DrawString(msg, TextScheme.F1, fgBrush, textRect);
                        _gxBuffer.Render();
                    }
                }
                else
                {
                    RenderObjects(_gxBuffer.Graphics);
                    _gxBuffer.Render();
                }
                _gxBuffer.Render(e.Graphics);
            }
        }

        private void RenderObjects(Graphics graphics)
        {
            int visibleRows = VisibleRowCount;

            if (visibleRows == 0 || GetPacketList(CurrentContext).Count == 0) 
                return;

            int row0Packet = Math.Max(0, 1 + GetBasePacketNumber(CurrentContext) - visibleRows);

            for (int row = 0; row < Math.Min(visibleRows, GetPacketList(CurrentContext).Count); row++)
            {
                DrawPacket(graphics, OffsetToRowPosition(GetOuterRowRectangle(), row), row0Packet + row);
            }
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
            if (CurrentContext != null && ScrollBar != null && VisibleRowCount > 0)
            {
                ScrollBar.LargeChange = VisibleRowCount;
                ScrollBar.Maximum = Math.Max(0, GetPacketList(CurrentContext).Count - 1);
                SetBasePacketNumber(CurrentContext, ScrollBar.Value);
                ScrollBar.Value = Math.Max(0, GetBasePacketNumber(CurrentContext) + 1 - ScrollBar.LargeChange);
                System.Diagnostics.Debug.WriteLine(String.Format("Setting value = {0}, BasePacketNumber = {1}", ScrollBar.Value, GetBasePacketNumber(CurrentContext)));
            }
            Invalidate();
        }

        internal void StatusChange(string contextName, ProtocolControl.ControlEvent evnt, PacketArrayList packetList)
        {
            if (String.IsNullOrEmpty(contextName))
            {
                System.Diagnostics.Debug.Assert(false, "contextName is null");
                throw new ArgumentNullException("contextName", "TracerControl.StatusName() was passed a null contextName");
            }

            if (packetList == null)
            {
                System.Diagnostics.Debug.Assert(false, "packetList is null");
                throw new ArgumentNullException("packetList", "TracerControl.StatusName() was passed a null packetList");
            }

            lock (Context.ContextLock)
            {
                if (!Context.ContainsKey(contextName))
                {
                    Context.Add(contextName, packetList.DataType);
                }

                //AddPacketBatch(contextName, packetList);

                switch (GetControlState(contextName))
                {
                case TracerControl.ControlStateValue.Active:
                    switch (evnt)
                    {
                    case ProtocolControl.ControlEvent.DataUpdate:
                        //System.Diagnostics.Debugger.Break();
                        SetPacketList(contextName, packetList);
                        SetSavedPacketList(contextName, packetList);
                        ConfigureScrollBar(contextName);

                        Invalidate();
                        break;

                    case ProtocolControl.ControlEvent.CaptureMode:
                        //System.Diagnostics.Debugger.Break();
                        SetPacketList(contextName, packetList);
                        SetSavedPacketList(contextName, packetList);
                        Invalidate();
                        break;

                    case ProtocolControl.ControlEvent.ReivewMode:
                        //System.Diagnostics.Debugger.Break();
                        SetPacketList(contextName, packetList);
                        SetSavedPacketList(contextName, packetList);
                        SetControlState(contextName, ControlStateValue.Review);
                        ConfigureActiveLabel(contextName);
                        ConfigureFreezeCheckbox(contextName);
                        ConfigureScrollBar(contextName);
                        Invalidate();
                        break;

                    case ProtocolControl.ControlEvent.ContextChange:
                        //System.Diagnostics.Debugger.Break();
                        SwitchContext(contextName);
                        SetPacketList(contextName, packetList);
                        SetSavedPacketList(contextName, packetList);
                        Invalidate();
                        break;

                    case ProtocolControl.ControlEvent.Closing:
                        System.Diagnostics.Debug.Assert(false, "Unexpected Closing Event.");
                        break;

                    case ProtocolControl.ControlEvent.Freeze:
                        //System.Diagnostics.Debugger.Break();
                        // Make a frozen copy of the packetlist
                        PacketArrayList freezeList = new PacketArrayList(packetList.DataType);
                        freezeList.AddRange(packetList);
                        SetSavedPacketList(contextName, packetList);
                        SetPacketList(contextName, freezeList);
                        SetDataPending(contextName, false);
                        SetControlState(contextName, ControlStateValue.FrozenActive);
                        ConfigureFreezeCheckbox(contextName);
                        ConfigureActiveLabel(contextName);
                        break;

                    case ProtocolControl.ControlEvent.Thaw:
                        System.Diagnostics.Debug.Assert(false, "Unexpected Thaw Event");
                        break;

                    case ProtocolControl.ControlEvent.Clear:
                        SetControlState(contextName, ControlStateValue.Review);
                        ConfigureActiveLabel(contextName);
                        ConfigureFreezeCheckbox(contextName);
                        ConfigureScrollBar(contextName);
                        Invalidate();
                        break;

                    case ProtocolControl.ControlEvent.Load:
                        System.Diagnostics.Debug.Assert(false, "Unexpected Load Event");
                        break;

                    default:
                        System.Diagnostics.Debug.Assert(false, "Unknown/unhandled event");
                        break;
                    }
                    break;

                case TracerControl.ControlStateValue.FrozenActive:
                    switch (evnt)
                    {
                    case ProtocolControl.ControlEvent.DataUpdate:
                //        System.Diagnostics.Debugger.Break();
                        SetSavedPacketList(contextName, packetList);
                        SetDataPending(contextName, true);
                        break;

                    case ProtocolControl.ControlEvent.CaptureMode:
                        //System.Diagnostics.Debugger.Break();
                        SetSavedPacketList(contextName, packetList);
                        SetDataPending(contextName, true);
                        break;

                    case ProtocolControl.ControlEvent.ReivewMode:
                        //System.Diagnostics.Debugger.Break();
                        SetSavedPacketList(contextName, packetList);
                        SetDataPending(contextName, true);
                        SetControlState(contextName, ControlStateValue.FrozenReview);
                        ConfigureActiveLabel(contextName);
                        break;

                    case ProtocolControl.ControlEvent.ContextChange:
                        //System.Diagnostics.Debugger.Break();
                        SwitchContext(contextName);
                        SetSavedPacketList(contextName, packetList);
                        Invalidate();
                        break;

                    case ProtocolControl.ControlEvent.Closing:
                        System.Diagnostics.Debug.Assert(false, "Unexpected Closing Event");
                        break;

                    case ProtocolControl.ControlEvent.Freeze:
                        System.Diagnostics.Debug.Assert(false, "Unexpected Freeze Event");
                        break;

                    case ProtocolControl.ControlEvent.Thaw:
                        //System.Diagnostics.Debugger.Break();
                        {
                            PacketArrayList saved = GetSavedPacketList(contextName);
                            PacketArrayList frozen = GetPacketList(contextName);
                            if (saved == null)
                            {
                                System.Diagnostics.Debug.Assert(false, "SavedPacketList is null");
                                return;
                            }
                            System.Diagnostics.Debug.Assert(saved.GetType() == typeof(PacketArrayListGlue));
                            System.Diagnostics.Debug.Assert(!saved.Equals(frozen));
                            if (!saved.Equals(frozen))
                            {
                                SetPacketList(contextName, saved);
                                SetSavedPacketList(contextName, saved);
                                SetBasePacketNumber(contextName, saved.Count - 1);
                                SetControlState(contextName, ControlStateValue.Active);
                                ConfigureActiveLabel(contextName);
                                ConfigureFreezeCheckbox(contextName);
                                ConfigureScrollBar(contextName);
                                frozen.Clear();
                                frozen.TrimToSize();
                            }
                            Invalidate();
                        }
                        break;

                    case ProtocolControl.ControlEvent.Clear:
                        {
                            PacketArrayList saved = GetSavedPacketList(contextName);
                            PacketArrayList frozen = GetPacketList(contextName);
                            if (saved == null)
                            {
                                System.Diagnostics.Debug.Assert(false, "SavedPacketList is null");
                                return;
                            }
                            System.Diagnostics.Debug.Assert(saved.GetType() == typeof(PacketArrayListGlue));
                            System.Diagnostics.Debug.Assert(!saved.Equals(frozen));
                            SetPacketList(contextName, saved);
                            SetSavedPacketList(contextName, saved);
                            SetBasePacketNumber(contextName, saved.Count - 1);
                            SetControlState(contextName, ControlStateValue.Review);
                            ConfigureActiveLabel(contextName);
                            ConfigureFreezeCheckbox(contextName);
                            ConfigureScrollBar(contextName);
                            frozen.Clear();
                            frozen.TrimToSize();
                            Invalidate();
                        }
                        break;

                    case ProtocolControl.ControlEvent.Load:
                        System.Diagnostics.Debug.Assert(false, "Unexpected Load Event");
                        break;

                    default:
                        System.Diagnostics.Debug.Assert(false, "Unknown/unhandled event");
                        break;
                    }
                    break;

                case TracerControl.ControlStateValue.Review:
                    switch (evnt)
                    {
                    case ProtocolControl.ControlEvent.DataUpdate:
                        // Should not happen
                        System.Diagnostics.Debug.Assert(false, "Unexpected DataUpdate Event");
                        break;

                    case ProtocolControl.ControlEvent.CaptureMode:
                        //System.Diagnostics.Debugger.Break();
                        SetPacketList(contextName, packetList);
                        SetSavedPacketList(contextName, packetList);
                        SetControlState(contextName, ControlStateValue.Active);
                        ConfigureActiveLabel(contextName);
                        ConfigureFreezeCheckbox(contextName);
                        ConfigureScrollBar(contextName);
                        Invalidate();
                        break;

                    case ProtocolControl.ControlEvent.ReivewMode:
                        // Should not happen
                        System.Diagnostics.Debug.Assert(false, "Unexpected ReviewMode Event");
                        break;

                    case ProtocolControl.ControlEvent.ContextChange:
                        //System.Diagnostics.Debugger.Break();
                        SetPacketList(contextName, packetList);
                        SetSavedPacketList(contextName, packetList);
                        SwitchContext(contextName);
//                        ConfigureScrollBar();
                        Invalidate();
                        break;

                    case ProtocolControl.ControlEvent.Closing:
                        //System.Diagnostics.Debugger.Break();
                        GetPacketList(contextName).Clear();
                        GetPacketList(contextName).TrimToSize();
                        Context.Remove(contextName);
                        break;

                    case ProtocolControl.ControlEvent.Freeze:
                        System.Diagnostics.Debug.Assert(false, "Unexpected Freeze Event");
                        break;

                    case ProtocolControl.ControlEvent.Thaw:
                        System.Diagnostics.Debug.Assert(false, "Unexpected Thaw Event");
                        break;

                    case ProtocolControl.ControlEvent.Clear:
                        ConfigureScrollBar(contextName);
                        Invalidate();
                        break;

                    case ProtocolControl.ControlEvent.Load:
                        {
                            ConfigureScrollBar(contextName);
                            Invalidate();
                        }
                        break;

                    default:
                        System.Diagnostics.Debug.Assert(false, "Unknown/unhandled event");
                        break;
                    }
                    break;

                case TracerControl.ControlStateValue.FrozenReview:
                    switch (evnt)
                    {
                    case ProtocolControl.ControlEvent.DataUpdate:
                        System.Diagnostics.Debug.Assert(false, "Unexpected DataUpdate Event");
                        break;

                    case ProtocolControl.ControlEvent.CaptureMode:
                        //System.Diagnostics.Debugger.Break();
                        SetSavedPacketList(contextName, packetList);
                        SetControlState(contextName, ControlStateValue.FrozenActive);
                        ConfigureActiveLabel(contextName);
                        ConfigureFreezeCheckbox(contextName);
                        ConfigureScrollBar(contextName);
                        Invalidate();
                        break;

                    case ProtocolControl.ControlEvent.ReivewMode:
                        //System.Diagnostics.Debugger.Break();
                        break;

                    case ProtocolControl.ControlEvent.ContextChange:
                        //System.Diagnostics.Debugger.Break();
                        SwitchContext(contextName);
                        SetSavedPacketList(contextName, packetList);
                        Invalidate();
                        break;

                    case ProtocolControl.ControlEvent.Closing:
                        System.Diagnostics.Debug.Assert(false, "Unexpected Closing Event");
                        break;

                    case ProtocolControl.ControlEvent.Freeze:
                        System.Diagnostics.Debug.Assert(false, "Unexpected Freeze Event");
                        break;

                    case ProtocolControl.ControlEvent.Thaw:
                        //System.Diagnostics.Debugger.Break();
                        {
                            PacketArrayList saved = GetSavedPacketList(contextName);
                            PacketArrayList frozen = GetPacketList(contextName);
                            if (saved == null)
                            {
                                System.Diagnostics.Debug.Assert(false, "SavedPacketList is null");
                                return;
                            }
                            SetPacketList(contextName, saved);
                            SetSavedPacketList(contextName, saved);
                            SetBasePacketNumber(contextName, saved.Count - 1);
                            SetControlState(contextName, ControlStateValue.Review);
                            ConfigureActiveLabel(contextName);
                            ConfigureFreezeCheckbox(contextName);
                            ConfigureScrollBar(contextName);
                            frozen.Clear();
                            frozen.TrimToSize();
                            Invalidate();
                        }
                        break;

                    case ProtocolControl.ControlEvent.Clear:
                        {
                            PacketArrayList saved = GetSavedPacketList(contextName);
                            PacketArrayList frozen = GetPacketList(contextName);
                            if (saved == null)
                            {
                                System.Diagnostics.Debug.Assert(false, "SavedPacketList is null");
                                return;
                            }
                            SetPacketList(contextName, saved);
                            SetSavedPacketList(contextName, saved);
                            SetBasePacketNumber(contextName, saved.Count - 1);
                            SetControlState(contextName, ControlStateValue.Review);
                            ConfigureActiveLabel(contextName);
                            ConfigureFreezeCheckbox(contextName);
                            ConfigureScrollBar(contextName);
                            frozen.Clear();
                            frozen.TrimToSize();
                            Invalidate();
                        }
                        break;

                    case ProtocolControl.ControlEvent.Load:
                        {
                            ConfigureScrollBar(contextName);
                            Invalidate();
                        }
                        break;

                    default:
                        System.Diagnostics.Debug.Assert(false, "Unknown/unhandled event");
                        break;
                    }
                    break;

                default:
                    System.Diagnostics.Debug.Assert(false, "Unknown/unhandled control state.");
                    break;
                }
            }
        }

        private void ConfigureActiveLabel(string contextName)
        {
            if (ActiveLabel != null && contextName == CurrentContext)
            {
                switch (GetControlState(CurrentContext))
                {
                case ControlStateValue.Active:
                    ActiveLabel.Visible = true;
                    ActiveLabel.Enabled = true;
                    break;

                case ControlStateValue.FrozenActive:
                    ActiveLabel.Visible = true;
                    ActiveLabel.Enabled = false;
                    break;

                case ControlStateValue.Review:
                    ActiveLabel.Visible = false;
                    break;

                case ControlStateValue.FrozenReview:
                    ActiveLabel.Visible = false;
                    break;

                default:
                    break;
                }
            }
        }

        private void ConfigureFreezeCheckbox(string contextName)
        {
            try
            {
                _ignoreCheckChanged = true;
                if (FreezeCheckbox != null && contextName == CurrentContext)
                {
                    switch (GetControlState(CurrentContext))
                    {
                    case ControlStateValue.Active:
                        FreezeCheckbox.Enabled = true;
                        FreezeCheckbox.Checked = false;
                        FreezeCheckbox.ForeColor = _DefaultFGColor;
                        FreezeCheckbox.BackColor = _DefaultBGColor;
                        break;

                    case ControlStateValue.FrozenActive:
                        FreezeCheckbox.Enabled = true;
                        FreezeCheckbox.Checked = true;
                        break;

                    case ControlStateValue.Review:
                        FreezeCheckbox.ForeColor = _DefaultFGColor;
                        FreezeCheckbox.BackColor = _DefaultBGColor;
                        FreezeCheckbox.Enabled = false;
                        break;

                    case ControlStateValue.FrozenReview:
                        FreezeCheckbox.Enabled = true;
                        FreezeCheckbox.Checked = true;
                        break;

                    default:
                        break;
                    }
                }
            }
            finally
            {
                _ignoreCheckChanged = false;
            }
        }

        private void ConfigureScrollBar(string contextName)
        {
            
            if (ScrollBar != null && contextName == CurrentContext && VisibleRowCount > 0)
            {
                switch (GetControlState(CurrentContext))
                {
                case ControlStateValue.Active:
                    {
                        int cnt = GetPacketList(CurrentContext).Count;
                        SetBasePacketNumber(CurrentContext, cnt - 1);
                        ScrollBar.Maximum = Math.Max(0, cnt - 1);
                        ScrollBar.Value = Math.Max(0, cnt - VisibleRowCount);
//                        System.Diagnostics.Debug.WriteLine(String.Format("Setting Scrollbar value = {0}, BasePacketNumber = {1}", ScrollBar.Value, GetBasePacketNumber(CurrentContext)));
                    }
                    break;

                case ControlStateValue.FrozenActive:
                    break;

                case ControlStateValue.Review:
                    {
                        int cnt = GetPacketList(CurrentContext).Count;
                        SetBasePacketNumber(CurrentContext, cnt - 1);
                        ScrollBar.Maximum = Math.Max(0, cnt - 1);
                        ScrollBar.Value = Math.Max(0, cnt - VisibleRowCount);
//                        System.Diagnostics.Debug.WriteLine(String.Format("Setting Scrollbar value = {0}, BasePacketNumber = {1}", ScrollBar.Value, GetBasePacketNumber(CurrentContext)));
                        //ScrollBar.LargeChange = VisibleRowCount;
                        //ScrollBar.Maximum = Math.Max(0, GetPacketList(CurrentContext).Count - 1);
                        //SetBasePacketNumber(CurrentContext, ScrollBar._value);
                        //ScrollBar._value = Math.Max(0, GetBasePacketNumber(CurrentContext) + 1 - ScrollBar.LargeChange);
                    }
                    break;

                case ControlStateValue.FrozenReview:
                    break;

                default:
                    break;
                }
            }
        }

        private void SwitchContext(string newContextName)
        {
            lock (Context.ContextLock)
            {
                // Save Current Context
                if (!String.IsNullOrEmpty(CurrentContext))
                {
//                    Context[_displayContextName]._freezeEnabled	= FreezeCheckbox.Enabled;
//                    Context[_displayContextName]._freezeOn		= FreezeCheckbox.Checked;
                }

                if (!Context.ContainsKey(newContextName))
                {
                    throw new Exception(String.Format("{0} is not a known context.", newContextName));
                }

                CurrentContext = newContextName;
                switch (GetControlState(newContextName))
                {

                case ControlStateValue.Active:
                    ConfigureActiveLabel(CurrentContext);
                    ConfigureFreezeCheckbox(CurrentContext);
                    ConfigureScrollBar(CurrentContext);
                    break;

                case ControlStateValue.FrozenActive:
                    ConfigureActiveLabel(CurrentContext);
                    ConfigureFreezeCheckbox(CurrentContext);
                    break;

                case ControlStateValue.Review:
                    ConfigureActiveLabel(CurrentContext);
                    ConfigureFreezeCheckbox(CurrentContext);
                    break;

                case ControlStateValue.FrozenReview:
                    ConfigureActiveLabel(CurrentContext);
                    ConfigureFreezeCheckbox(CurrentContext);
                    break;

                default:
                    break;
                }

                if (FreezeCheckbox != null)
                {
//                    FreezeCheckbox.Enabled = Context[CurrentContext]._freezeEnabled;
//                    FreezeCheckbox.Checked = Context[CurrentContext]._freezeOn;
                }
            }
        }

        public void DrawPacket(Graphics graphics, Rectangle containingRectangle, int packetIndex)
        {
            PacketData.PacketWrapper packetEnvelope = null;

            if (packetIndex >= GetPacketList(CurrentContext).Count)
            {
                System.Diagnostics.Debug.WriteLine("DrawPacket() packet index out of range");
                return;
            }

            // new
            PacketArrayList pal = GetPacketList(CurrentContext);

			if (typeof(LakeChabotReader) == pal.DataType || typeof(VirtualReader) == pal.DataType)
            {
                packetEnvelope = new PacketData.PacketWrapper((PacketData.PacketWrapper)pal[packetIndex]);
            }
            else
            {
            }
            System.Diagnostics.Debug.Assert(packetEnvelope != null);

            Rectangle r = GetDrawingRowRectangle(containingRectangle);

            if (packetEnvelope != null)
            {
                DrawPacketInfo(graphics, r, packetEnvelope);
            }
            else
            {
            }
        }

        private static class ColorScheme
        {
            public static readonly Color BG =	Color.FromArgb(255, Color.FromArgb(218, 218, 218));	// Used for background
            public static readonly Color C0 =	Color.FromArgb(255, Color.FromArgb(255, 251, 236));	// Used for value background
            public static readonly Color C0a =	Color.FromArgb(128, Color.FromArgb(245, 255, 210)); 
            public static readonly Color C1 =	Color.FromArgb(255, Color.FromArgb(199, 195, 162)); // Used for first captions
            public static readonly Color C1a =	Color.FromArgb(244, Color.FromArgb(239, 229, 167)); // Used without captions
            public static readonly Color C2 =	Color.FromArgb(255, Color.FromArgb(167, 195, 178));
            public static readonly Color C2a =	Color.FromArgb(255, Color.FromArgb(219, 255, 207));
            public static readonly Color C3 =	Color.FromArgb(255, Color.FromArgb(209, 178, 186));
            public static readonly Color C3a =	Color.FromArgb(255, Color.FromArgb(247, 199, 187));
            public static readonly Color C4 =	Color.FromArgb(255, Color.FromArgb(157, 187, 208));
            public static readonly Color C4a =	Color.FromArgb(255, Color.FromArgb(193, 223, 228));
            public static readonly Color C4Err = Color.FromArgb(255, Color.FromArgb(255, 201, 0));
            public static readonly Color C4Erra = Color.FromArgb(255, Color.FromArgb(255, 201, 0));
            public static readonly Color CBad =	Color.FromArgb(255, Color.FromArgb(237, 76, 82));
            public static readonly Color C6	=	Color.FromArgb(255, Color.FromArgb(224, 224, 224));
            public static readonly Color C6a =	Color.FromArgb(255, Color.FromArgb(224, 224, 224));
//            public static readonly Color C7 = Color.FromArgb(255, Color.FromArgb(157, 187, 208));
        }

        private static class TextScheme
        {
            public static readonly Color F1Color = Color.Black;
            public static readonly Color F1Colora = Color.Black;
            public static readonly Font F1 = new Font("Tahoma", 8.0f);
        }

        private class DisplayField
        {
            public string	Name;
            public string	Value;
            public int		Width;
            public Color	CaptionBgColor;
            public Color	ValueBgColor;

            public DisplayField(string fieldName, int fieldWidth, Color captionColor, Color valueColor)
            {
                Name = fieldName;
                Width = fieldWidth;
                CaptionBgColor = captionColor;
                ValueBgColor = valueColor;
            }

            public DisplayField(string fieldName, int fieldWidth, Color captionColor, Color valueColor, string value)
            {
                Name			= fieldName;
                Value			= value;
                Width			= fieldWidth;
                CaptionBgColor	= captionColor;
                ValueBgColor	= valueColor;
            }
        }

        public void DrawPacketInfo(Graphics graphics, Rectangle drawingRect, PacketData.PacketWrapper envelope)
        {
            if (graphics == null)
            {
                System.Diagnostics.Debug.Assert(false, "Null graphics passed to DrawPacketInfo");
                Exception exp = new ArgumentNullException("graphics", "RFID_Explorer.TracerControl.DrawPacketInfo() was passed a null graphics object.");
                SysLogger.LogError(exp);
                return;
            }

            if (envelope == null)
            {
                System.Diagnostics.Debug.Assert(false, "Null envelope passed to DrawPacketInfo");
                Exception exp = new ArgumentNullException("envelope", "RFID_Explorer.TracerControl.DrawPacketInfo() was passed a null envelope.");
                SysLogger.LogError(exp);
                return;
            }

            //#	Packet Time	Packet Type	Raw Packet Data	
            //Sequence# TimeStamp Pseudo Reader Index Type In/Out
            // pkt_ver; flags; 	pkt_type;		pkt_len;	

            PacketData.PacketBase packet = envelope.Packet;
            PacketData.PacketType type = envelope.PacketType;
            DisplayField[] recordlayout = null;

            if (envelope.IsPseudoPacket)
            {
                PacketData.CommandPsuedoPacket psuedo = envelope.Packet as PacketData.CommandPsuedoPacket;
                if (psuedo != null)
                {
                    switch (psuedo.RequestName)
                    {
                    case "Inventory":

                        ReaderRequest readerRequest = new ReaderRequest();

                        using (System.IO.MemoryStream data = new System.IO.MemoryStream(psuedo.DataValues))
                        {
                            readerRequest.ReadFrom(data);
                        }
                        break;

                    case "BadPacket":
                        {
                            BadPacket badPacket = new BadPacket();
                            using (System.IO.MemoryStream data = new System.IO.MemoryStream(psuedo.DataValues))
                            {
                                badPacket.ReadFrom(data);
                            }

                            string packetData = badPacket.RawPacketData == null ? null : BitConverter.ToString(badPacket.RawPacketData);
                            if (packetData.Length > 80) packetData = packetData.Substring(0, 80) + "\u2026";

                            DisplayField[] badPkLayout =
                        {
                            new DisplayField("reader",			55, ColorScheme.C1, ColorScheme.C1a, envelope.ReaderIndex.ToString()),
                            new DisplayField("packet",			55, ColorScheme.CBad, ColorScheme.CBad, envelope.PacketNumber.ToString()),
                            new DisplayField("timestamp",		69, ColorScheme.CBad, ColorScheme.CBad, envelope.Timestamp.ToString("mm:ss.fff")),
                            new DisplayField("ver",				21, ColorScheme.CBad, ColorScheme.CBad),
                            new DisplayField("flg",				31, ColorScheme.CBad, ColorScheme.CBad),
                            new DisplayField("len",				31, ColorScheme.CBad, ColorScheme.CBad),
                            new DisplayField("type",			94, ColorScheme.CBad, ColorScheme.CBad),
                            new DisplayField("invalid packet - raw buffer dump",
                                                                515, ColorScheme.CBad, ColorScheme.CBad, packetData),
                        };

                            recordlayout = badPkLayout;
                        }
                        break;

                    default:
                        System.Diagnostics.Debug.Assert(false);
                        break;
                    }
                }
            }
            else
            {
                switch (type)
                {
                case PacketData.PacketType.CMD_BEGIN:
                    {
                        PacketData.cmd_beg cmd_beg = packet as PacketData.cmd_beg;
                        System.Diagnostics.Debug.Assert(cmd_beg != null);
                        BitVector32 flags = new BitVector32(cmd_beg.flags);
                        DisplayField[] cmdBegLayout =
                    {
                        new DisplayField("reader",			55, ColorScheme.C1, ColorScheme.C1a, envelope.ReaderIndex.ToString()),
                        new DisplayField("packet",			55, ColorScheme.C1, ColorScheme.C1a, envelope.PacketNumber.ToString()),
                        new DisplayField("timestamp",		69, ColorScheme.C1, ColorScheme.C1a, envelope.Timestamp.ToString("mm:ss.fff")),
                        new DisplayField("ver",				21, ColorScheme.C2, ColorScheme.C2a, cmd_beg.pkt_ver.ToString()),
                        new DisplayField("flg",				31, ColorScheme.C2, ColorScheme.C2a, String.Format("0x{0:x}", cmd_beg.flags)),
                        new DisplayField("len",				31, ColorScheme.C2, ColorScheme.C2a, cmd_beg.pkt_len.ToString()),
                        new DisplayField("type",			94, ColorScheme.C3, ColorScheme.C3a, envelope.PacketTypeName),
                        new DisplayField("ms_ctr",			70,	ColorScheme.C4, ColorScheme.C4a, cmd_beg.ms_ctr.ToString()),
                        new DisplayField("command",			80, ColorScheme.C4, ColorScheme.C4a, PacketData.PacketBase.GetCommandName(cmd_beg.command)),
                        new DisplayField("continuous mode",	100, ColorScheme.C4, ColorScheme.C4a, flags[PacketData.PacketBase.continuousMode] == (int)PacketData.PacketBase.ContinuousModeFlag.InContinuousMode ? "Yes" : "No"),
                    };

                        recordlayout = cmdBegLayout;
                    }
                    break;

                case PacketData.PacketType.CMD_END:
                    {
                        PacketData.cmd_end cmd_end = packet as PacketData.cmd_end;
                        System.Diagnostics.Debug.Assert(cmd_end != null);
                        DisplayField[] cmdEndLayout =
                    {
                        new DisplayField("reader",			55, ColorScheme.C1, ColorScheme.C1a, envelope.ReaderIndex.ToString()),
                        new DisplayField("packet",			55, ColorScheme.C1, ColorScheme.C1a, envelope.PacketNumber.ToString()),
                        new DisplayField("timestamp",		69, ColorScheme.C1, ColorScheme.C1a, envelope.Timestamp.ToString("mm:ss.fff")),
                        new DisplayField("ver",				21, ColorScheme.C2, ColorScheme.C2a, cmd_end.pkt_ver.ToString()),
                        new DisplayField("flg",				31, ColorScheme.C2, ColorScheme.C2a, String.Format("0x{0:x}", cmd_end.flags)),
                        new DisplayField("len",				31, ColorScheme.C2, ColorScheme.C2a, cmd_end.pkt_len.ToString()),
                        new DisplayField("type",			94, ColorScheme.C3, ColorScheme.C3a, envelope.PacketTypeName),
                        new DisplayField("ms_ctr",			70,	ColorScheme.C4, ColorScheme.C4a, cmd_end.ms_ctr.ToString()),
                        new DisplayField("status",			80, cmd_end.Result == 0 ? ColorScheme.C4  : ColorScheme.C4Err, 
                                                                cmd_end.Result == 0 ? ColorScheme.C4a : ColorScheme.C4Err, PacketData.PacketBase.GetResultName(cmd_end.Result)),
                    };
                        recordlayout = cmdEndLayout;
                    }
                    break;

                case PacketData.PacketType.COMMAND_ACTIVE:
                    {
                        PacketData.command_active command_active = packet as PacketData.command_active;
                        System.Diagnostics.Debug.Assert(command_active != null);
                        DisplayField[] commandActiveLayout =
                    {
                        new DisplayField("reader",			55, ColorScheme.C1, ColorScheme.C1a, envelope.ReaderIndex.ToString()),
                        new DisplayField("packet",			55, ColorScheme.C1, ColorScheme.C1a, envelope.PacketNumber.ToString()),
                        new DisplayField("timestamp",		69, ColorScheme.C1, ColorScheme.C1a, envelope.Timestamp.ToString("mm:ss.fff")),
                        new DisplayField("ver",				21, ColorScheme.C2, ColorScheme.C2a, command_active.pkt_ver.ToString()),
                        new DisplayField("flg",				31, ColorScheme.C2, ColorScheme.C2a, String.Format("0x{0:x}", command_active.flags)),
                        new DisplayField("len",				31, ColorScheme.C2, ColorScheme.C2a, command_active.pkt_len.ToString()),
                        new DisplayField("type",			94, ColorScheme.C3, ColorScheme.C3a, envelope.PacketTypeName),
                        new DisplayField("ms_ctr",			70,	ColorScheme.C4, ColorScheme.C4a, command_active.ms_ctr.ToString()),
                    };
                        recordlayout = commandActiveLayout;
                    }
                    break;

                case PacketData.PacketType.ANTENNA_CYCLE_BEGIN:
                    {
                        PacketData.ant_cyc_beg cyc_beg = packet as PacketData.ant_cyc_beg;
                        System.Diagnostics.Debug.Assert(cyc_beg != null);
                        DisplayField[] cyclBegLayout =
                    {
                        new DisplayField("reader",			55, ColorScheme.C1, ColorScheme.C1a, envelope.ReaderIndex.ToString()),
                        new DisplayField("packet",			55, ColorScheme.C1, ColorScheme.C1a, envelope.PacketNumber.ToString()),
                        new DisplayField("timestamp",		69, ColorScheme.C1, ColorScheme.C1a, envelope.Timestamp.ToString("mm:ss.fff")),
                        new DisplayField("ver",				21, ColorScheme.C2, ColorScheme.C2a, cyc_beg.pkt_ver.ToString()),
                        new DisplayField("flg",				31, ColorScheme.C2, ColorScheme.C2a, String.Format("0x{0:x}", cyc_beg.flags)),
                        new DisplayField("len",				31, ColorScheme.C2, ColorScheme.C2a, cyc_beg.pkt_len.ToString()),
                        new DisplayField("type",			94, ColorScheme.C3, ColorScheme.C3a, envelope.PacketTypeName),
                    };
                        recordlayout = cyclBegLayout;
                    }
                    break;

                case PacketData.PacketType.ANTENNA_CYCLE_BEGIN_DIAG:
                    {
                        PacketData.ant_cyc_beg_diag cyc_beg_diag = packet as PacketData.ant_cyc_beg_diag;
                        System.Diagnostics.Debug.Assert(cyc_beg_diag != null);
                        DisplayField[] cyclBegLayout =
                    {
                        new DisplayField("reader",			55, ColorScheme.C1, ColorScheme.C1a, envelope.ReaderIndex.ToString()),
                        new DisplayField("packet",			55, ColorScheme.C1, ColorScheme.C1a, envelope.PacketNumber.ToString()),
                        new DisplayField("timestamp",		69, ColorScheme.C1, ColorScheme.C1a, envelope.Timestamp.ToString("mm:ss.fff")),
                        new DisplayField("ver",				21, ColorScheme.C2, ColorScheme.C2a, cyc_beg_diag.pkt_ver.ToString()),
                        new DisplayField("flg",				31, ColorScheme.C2, ColorScheme.C2a, String.Format("0x{0:x}", cyc_beg_diag.flags)),
                        new DisplayField("len",				31, ColorScheme.C2, ColorScheme.C2a, cyc_beg_diag.pkt_len.ToString()),
                        new DisplayField("type",			94, ColorScheme.C3, ColorScheme.C3a, envelope.PacketTypeName),
                        new DisplayField("ms_ctr",			70,	ColorScheme.C6, ColorScheme.C6a, cyc_beg_diag.ms_ctr.ToString()),
                    };
                        recordlayout = cyclBegLayout;
                    }
                    break;

                case PacketData.PacketType.ANTENNA_CYCLE_END:
                    PacketData.ant_cyc_end cyc_end = packet as PacketData.ant_cyc_end;
                    System.Diagnostics.Debug.Assert(cyc_end != null);
                    DisplayField[] cycEndLayout =
                    {
                        new DisplayField("reader",			55, ColorScheme.C1, ColorScheme.C1a, envelope.ReaderIndex.ToString()),
                        new DisplayField("packet",			55, ColorScheme.C1, ColorScheme.C1a, envelope.PacketNumber.ToString()),
                        new DisplayField("timestamp",		69, ColorScheme.C1, ColorScheme.C1a, envelope.Timestamp.ToString("mm:ss.fff")),
                        new DisplayField("ver",				21, ColorScheme.C2, ColorScheme.C2a, cyc_end.pkt_ver.ToString()),
                        new DisplayField("flg",				31, ColorScheme.C2, ColorScheme.C2a, String.Format("0x{0:x}", cyc_end.flags)),
                        new DisplayField("len",				31, ColorScheme.C2, ColorScheme.C2a, cyc_end.pkt_len.ToString()),
                        new DisplayField("type",			94, ColorScheme.C3, ColorScheme.C3a, envelope.PacketTypeName),
                    };
                    recordlayout = cycEndLayout;
                    break;

                case PacketData.PacketType.ANTENNA_CYCLE_END_DIAG:
                    {
                        PacketData.ant_cyc_end_diag cyc_end_diag = packet as PacketData.ant_cyc_end_diag;
                        System.Diagnostics.Debug.Assert(cyc_end_diag != null);
                        DisplayField[] cycEndDiagLayout =
                    {
                        new DisplayField("reader",			55, ColorScheme.C1, ColorScheme.C1a, envelope.ReaderIndex.ToString()),
                        new DisplayField("packet",			55, ColorScheme.C1, ColorScheme.C1a, envelope.PacketNumber.ToString()),
                        new DisplayField("timestamp",		69, ColorScheme.C1, ColorScheme.C1a, envelope.Timestamp.ToString("mm:ss.fff")),
                        new DisplayField("ver",				21, ColorScheme.C2, ColorScheme.C2a, cyc_end_diag.pkt_ver.ToString()),
                        new DisplayField("flg",				31, ColorScheme.C2, ColorScheme.C2a, String.Format("0x{0:x}", cyc_end_diag.flags)),
                        new DisplayField("len",				31, ColorScheme.C2, ColorScheme.C2a, cyc_end_diag.pkt_len.ToString()),
                        new DisplayField("type",			94, ColorScheme.C3, ColorScheme.C3a, envelope.PacketTypeName),
                        new DisplayField("ms_ctr",			70,	ColorScheme.C6, ColorScheme.C6a, cyc_end_diag.ms_ctr.ToString()),
                    };
                        recordlayout = cycEndDiagLayout;
                    }
                    break;

                case PacketData.PacketType.ANTENNA_BEGIN:
                    {
                        PacketData.ant_beg ant_beg = packet as PacketData.ant_beg;
                        System.Diagnostics.Debug.Assert(ant_beg != null);
                        DisplayField[] antBegLayout =
                    {
                        new DisplayField("reader",			55, ColorScheme.C1, ColorScheme.C1a, envelope.ReaderIndex.ToString()),
                        new DisplayField("packet",			55, ColorScheme.C1, ColorScheme.C1a, envelope.PacketNumber.ToString()),
                        new DisplayField("timestamp",		69, ColorScheme.C1, ColorScheme.C1a, envelope.Timestamp.ToString("mm:ss.fff")),
                        new DisplayField("ver",				21, ColorScheme.C2, ColorScheme.C2a, ant_beg.pkt_ver.ToString()),
                        new DisplayField("flg",				31, ColorScheme.C2, ColorScheme.C2a, String.Format("0x{0:x}", ant_beg.flags)),
                        new DisplayField("len",				31, ColorScheme.C2, ColorScheme.C2a, ant_beg.pkt_len.ToString()),
                        new DisplayField("type",			94, ColorScheme.C3, ColorScheme.C3a, envelope.PacketTypeName),
                        new DisplayField("antenna",			70,	ColorScheme.C4, ColorScheme.C4a, ant_beg.antenna.ToString()),
                    };
                        recordlayout = antBegLayout;
                    }
                    break;

                case PacketData.PacketType.ANTENNA_BEGIN_DIAG:
                    {
                        PacketData.ant_beg_diag ant_beg_diag	= packet as PacketData.ant_beg_diag;
                        System.Diagnostics.Debug.Assert(ant_beg_diag != null);
                        DisplayField[] antBegDiagLayout =
                    {
                        new DisplayField("reader",			55, ColorScheme.C1, ColorScheme.C1a, envelope.ReaderIndex.ToString()),
                        new DisplayField("packet",			55, ColorScheme.C1, ColorScheme.C1a, envelope.PacketNumber.ToString()),
                        new DisplayField("timestamp",		69, ColorScheme.C1, ColorScheme.C1a, envelope.Timestamp.ToString("mm:ss.fff")),
                        new DisplayField("ver",				21, ColorScheme.C2, ColorScheme.C2a, ant_beg_diag.pkt_ver.ToString()),
                        new DisplayField("flg",				31, ColorScheme.C2, ColorScheme.C2a, String.Format("0x{0:x}", ant_beg_diag.flags)),
                        new DisplayField("len",				31, ColorScheme.C2, ColorScheme.C2a, ant_beg_diag.pkt_len.ToString()),
                        new DisplayField("type",			94, ColorScheme.C3, ColorScheme.C3a, envelope.PacketTypeName),
                        new DisplayField("ms_ctr",			70,	ColorScheme.C6, ColorScheme.C6a, ant_beg_diag.ms_ctr.ToString()),
                        new DisplayField("sense_res",		80,	ColorScheme.C6, ColorScheme.C6a, ant_beg_diag.sense_res.ToString()),
                    };
                        recordlayout = antBegDiagLayout;
                    }
                    break;

                case PacketData.PacketType.ANTENNA_END:
                    {
                        PacketData.ant_end ant_end	= packet as PacketData.ant_end;
                        System.Diagnostics.Debug.Assert(ant_end != null);
                        DisplayField[] antEndLayout =
                    {
                        new DisplayField("reader",			55, ColorScheme.C1, ColorScheme.C1a, envelope.ReaderIndex.ToString()),
                        new DisplayField("packet",			55, ColorScheme.C1, ColorScheme.C1a, envelope.PacketNumber.ToString()),
                        new DisplayField("timestamp",		69, ColorScheme.C1, ColorScheme.C1a, envelope.Timestamp.ToString("mm:ss.fff")),
                        new DisplayField("ver",				21, ColorScheme.C2, ColorScheme.C2a, ant_end.pkt_ver.ToString()),
                        new DisplayField("flg",				31, ColorScheme.C2, ColorScheme.C2a, String.Format("0x{0:x}", ant_end.flags)),
                        new DisplayField("len",				31, ColorScheme.C2, ColorScheme.C2a, ant_end.pkt_len.ToString()),
                        new DisplayField("type",			94, ColorScheme.C3, ColorScheme.C3a, envelope.PacketTypeName),
                    };
                        recordlayout = antEndLayout;
                    }
                    break;

                case PacketData.PacketType.ANTENNA_END_DIAG:
                    {
                        PacketData.ant_end_diag ant_end_diag = packet as PacketData.ant_end_diag;
                        System.Diagnostics.Debug.Assert(ant_end_diag != null);
                        DisplayField[] antEndDiagLayout =
                    {
                        new DisplayField("reader",			55, ColorScheme.C1, ColorScheme.C1a, envelope.ReaderIndex.ToString()),
                        new DisplayField("packet",			55, ColorScheme.C1, ColorScheme.C1a, envelope.PacketNumber.ToString()),
                        new DisplayField("timestamp",		69, ColorScheme.C1, ColorScheme.C1a, envelope.Timestamp.ToString("mm:ss.fff")),
                        new DisplayField("ver",				21, ColorScheme.C2, ColorScheme.C2a, ant_end_diag.pkt_ver.ToString()),
                        new DisplayField("flg",				31, ColorScheme.C2, ColorScheme.C2a, String.Format("0x{0:x}", ant_end_diag.flags)),
                        new DisplayField("len",				31, ColorScheme.C2, ColorScheme.C2a, ant_end_diag.pkt_len.ToString()),
                        new DisplayField("type",			94, ColorScheme.C3, ColorScheme.C3a, envelope.PacketTypeName),
                        new DisplayField("ms_ctr",			70,	ColorScheme.C6, ColorScheme.C6a, ant_end_diag.ms_ctr.ToString()),
                        
                    };
                        recordlayout = antEndDiagLayout;
                    }
                    break;

                case PacketData.PacketType.ISO18K6C_INVENTORY_ROUND_BEGIN:
                    {
                        PacketData.inv_rnd_beg inv_rnd_beg = packet as PacketData.inv_rnd_beg;
                        System.Diagnostics.Debug.Assert(inv_rnd_beg != null);
                        DisplayField[] invBegLayout =
                    {
                        new DisplayField("reader",			55, ColorScheme.C1, ColorScheme.C1a, envelope.ReaderIndex.ToString()),
                        new DisplayField("packet",			55, ColorScheme.C1, ColorScheme.C1a, envelope.PacketNumber.ToString()),
                        new DisplayField("timestamp",		69, ColorScheme.C1, ColorScheme.C1a, envelope.Timestamp.ToString("mm:ss.fff")),
                        new DisplayField("ver",				21, ColorScheme.C2, ColorScheme.C2a, inv_rnd_beg.pkt_ver.ToString()),
                        new DisplayField("flg",				31, ColorScheme.C2, ColorScheme.C2a, String.Format("0x{0:x}", inv_rnd_beg.flags)),
                        new DisplayField("len",				31, ColorScheme.C2, ColorScheme.C2a, inv_rnd_beg.pkt_len.ToString()),
                        new DisplayField("type",			94, ColorScheme.C3, ColorScheme.C3a, envelope.PacketTypeName),
                    };
                        recordlayout = invBegLayout;
                    }
                    break;

                case PacketData.PacketType.ISO18K6C_INVENTORY_ROUND_BEGIN_DIAG:
                    {
                        PacketData.inv_rnd_beg_diag inv_rnd_beg_diag = packet as PacketData.inv_rnd_beg_diag;
                        System.Diagnostics.Debug.Assert(inv_rnd_beg_diag != null);

                        BitVector32 singulationParams = new BitVector32( ( int ) inv_rnd_beg_diag.sing_params);
                         
                        DisplayField[] invBegDiagLayout =
                    {
                        new DisplayField("reader",			55, ColorScheme.C1, ColorScheme.C1a, envelope.ReaderIndex.ToString()),
                        new DisplayField("packet",			55, ColorScheme.C1, ColorScheme.C1a, envelope.PacketNumber.ToString()),
                        new DisplayField("timestamp",		69, ColorScheme.C1, ColorScheme.C1a, envelope.Timestamp.ToString("mm:ss.fff")),
                        new DisplayField("ver",				21, ColorScheme.C2, ColorScheme.C2a, inv_rnd_beg_diag.pkt_ver.ToString()),
                        new DisplayField("flg",				31, ColorScheme.C2, ColorScheme.C2a, String.Format("0x{0:x}", inv_rnd_beg_diag.flags)),
                        new DisplayField("len",				31, ColorScheme.C2, ColorScheme.C2a, inv_rnd_beg_diag.pkt_len.ToString()),
                        new DisplayField("type",			94, ColorScheme.C3, ColorScheme.C3a, envelope.PacketTypeName),
                        new DisplayField("ms_ctr",			70,	ColorScheme.C6, ColorScheme.C6a, inv_rnd_beg_diag.ms_ctr.ToString()),
                        new DisplayField("current Q",		60, ColorScheme.C6, ColorScheme.C6a, singulationParams[PacketData.PacketBase.CurrentQValue].ToString()),
//                        new DisplayField("session",			49, ColorScheme.C6, ColorScheme.C6a, PacketData.PacketBase.InventorySessionName(singulationParams[PacketData.PacketBase.Session])),
                        new DisplayField("inv flg",			60, ColorScheme.C6, ColorScheme.C6a, PacketData.PacketBase.InventoriedFlagName(singulationParams[PacketData.PacketBase.InventoriedFlag])),
                        new DisplayField("slot",			81, ColorScheme.C6, ColorScheme.C6a, PacketData.PacketBase.GetCurrentSlot(singulationParams).ToString()),
                        new DisplayField("retry cnt",		81, ColorScheme.C6, ColorScheme.C6a, singulationParams[PacketData.PacketBase.RetryCount].ToString()),
                        new DisplayField("raw param",		163, ColorScheme.C6, ColorScheme.C6a, String.Format("0x{0:x}", singulationParams.Data)),
//                        new DisplayField("state",			49, ColorScheme.C6, ColorScheme.C6a, PacketData.PacketBase.SelectedStateName(singulationParams[PacketData.PacketBase.SelectedState])),
//                        new DisplayField("\u00f7 ratio",	49, ColorScheme.C6, ColorScheme.C6a, PacketData.PacketBase.DivideRatioName(singulationParams[PacketData.PacketBase.DivideRatio])), 
//                        new DisplayField("M",				48, ColorScheme.C6, ColorScheme.C6a, PacketData.PacketBase.SubcarrierCyclesNames(singulationParams[PacketData.PacketBase.SubcarrierCycles])), 
//                        new DisplayField("pilot tone",		81, ColorScheme.C6, ColorScheme.C6a, PacketData.PacketBase.PilotToneNames(singulationParams[PacketData.PacketBase.PilotTone])), 
//                        new DisplayField("sing_params",		70, ColorScheme.C6, ColorScheme.C6a, String.Format("0x{0:x}", inv_rnd_beg_diag.sing_params)),
                    };
                        recordlayout = invBegDiagLayout;
                    }
                    break;

                case PacketData.PacketType.ISO18K6C_INVENTORY:
                    {
                        PacketData.inventory inventory = packet as PacketData.inventory;
                        System.Diagnostics.Debug.Assert(inventory != null);
                        BitVector32 flags = new BitVector32(inventory.flags);
                        string tagData	= BitConverter.ToString(inventory.inventory_data, 0, inventory.inventory_data.Length - flags[PacketData.PacketBase.paddingBytes]);
                        if (tagData.Length > 59) tagData = tagData.Substring(0, 56) + "\u2026";
                        DisplayField[] inventoryLayout =
                    {
                        new DisplayField("reader",			55, ColorScheme.C1, ColorScheme.C1a, envelope.ReaderIndex.ToString()),
                        new DisplayField("packet",			55, ColorScheme.C1, ColorScheme.C1a, envelope.PacketNumber.ToString()),          
                        new DisplayField("timestamp",		69, ColorScheme.C1, ColorScheme.C1a, envelope.Timestamp.ToString("mm:ss.fff")),
                        new DisplayField("ver",				21, ColorScheme.C2, ColorScheme.C2a, inventory.pkt_ver.ToString()),
                        new DisplayField("flg",				31, ColorScheme.C2, ColorScheme.C2a, String.Format("0x{0:x}", inventory.flags)),
                        new DisplayField("len",				31, ColorScheme.C2, ColorScheme.C2a, inventory.pkt_len.ToString()),
                        new DisplayField("type",			94, ColorScheme.C3, ColorScheme.C3a, envelope.PacketTypeName),
                        new DisplayField("ms_ctr",			70,	ColorScheme.C4, ColorScheme.C4a, inventory.ms_ctr.ToString()),
                        new DisplayField("crc",				30, flags[PacketData.PacketBase.crcResult] == (int)PacketData.PacketBase.CrcResultValues.Good ? ColorScheme.C4 : ColorScheme.C4Err, 
                                                                flags[PacketData.PacketBase.crcResult] == (int)PacketData.PacketBase.CrcResultValues.Good ? ColorScheme.C4a : ColorScheme.C4Err,
                                                                flags[PacketData.PacketBase.crcResult] == (int)PacketData.PacketBase.CrcResultValues.Good ? "ok" : "BAD"),
                        new DisplayField("pad",				30, ColorScheme.C4, ColorScheme.C4a, flags[PacketData.PacketBase.paddingBytes].ToString()),
                        new DisplayField("nb_rssi",			60, ColorScheme.C4, ColorScheme.C4a, String.Format("0x{0:X}", inventory.nb_rssi)),
                        new DisplayField("wb_rssi",			60, ColorScheme.C4, ColorScheme.C4a, String.Format("0x{0:X}", inventory.wb_rssi)),
                        new DisplayField("ana_ctrl1",	    60, ColorScheme.C4, ColorScheme.C4a, String.Format("0x{0:X}", inventory.ana_ctrl1)),
                        new DisplayField("rssi",	        60, ColorScheme.C4, ColorScheme.C4a, String.Format("{0:0.0} dBm", (decimal)((short)inventory.rssi)/10)),
                        new DisplayField("inventory_data",	325, ColorScheme.C4, ColorScheme.C4a, tagData),
                    };
                        recordlayout = inventoryLayout;
                    }
                    break;

                case PacketData.PacketType.ISO18K6C_INVENTORY_ROUND_END:
                    {
                        PacketData.inv_rnd_end inv_rnd_end = packet as PacketData.inv_rnd_end;
                        System.Diagnostics.Debug.Assert(inv_rnd_end != null);
                        DisplayField[] invEndLayout =
                    {
                        new DisplayField("reader",			55, ColorScheme.C1, ColorScheme.C1a, envelope.ReaderIndex.ToString()),
                        new DisplayField("packet",			55, ColorScheme.C1, ColorScheme.C1a, envelope.PacketNumber.ToString()),
                        new DisplayField("timestamp",		69, ColorScheme.C1, ColorScheme.C1a, envelope.Timestamp.ToString("mm:ss.fff")),
                        new DisplayField("ver",				21, ColorScheme.C2, ColorScheme.C2a, inv_rnd_end.pkt_ver.ToString()),
                        new DisplayField("flg",				31, ColorScheme.C2, ColorScheme.C2a, String.Format("0x{0:x}", inv_rnd_end.flags)),
                        new DisplayField("len",				31, ColorScheme.C2, ColorScheme.C2a, inv_rnd_end.pkt_len.ToString()),
                        new DisplayField("type",			94, ColorScheme.C3, ColorScheme.C3a, envelope.PacketTypeName),
                    };
                        recordlayout = invEndLayout;
                    }
                    break;

                case PacketData.PacketType.ISO18K6C_INVENTORY_ROUND_END_DIAG:
                    {
                        PacketData.inv_rnd_end_diag inv_rnd_end_diag = packet as PacketData.inv_rnd_end_diag;
                        System.Diagnostics.Debug.Assert(inv_rnd_end_diag != null);
                        DisplayField[] invEndDiagLayout =
                    {
                        new DisplayField("reader",			55, ColorScheme.C1, ColorScheme.C1a, envelope.ReaderIndex.ToString()),
                        new DisplayField("packet",			55, ColorScheme.C1, ColorScheme.C1a, envelope.PacketNumber.ToString()),
                        new DisplayField("timestamp",		69, ColorScheme.C1, ColorScheme.C1a, envelope.Timestamp.ToString("mm:ss.fff")),
                        new DisplayField("ver",				21, ColorScheme.C2, ColorScheme.C2a, inv_rnd_end_diag.pkt_ver.ToString()),
                        new DisplayField("flg",				31, ColorScheme.C2, ColorScheme.C2a, String.Format("0x{0:x}", inv_rnd_end_diag.flags)),
                        new DisplayField("len",				31, ColorScheme.C2, ColorScheme.C2a, inv_rnd_end_diag.pkt_len.ToString()),
                        new DisplayField("type",			94, ColorScheme.C3, ColorScheme.C3a, envelope.PacketTypeName),
                        new DisplayField("ms_ctr",			70,	ColorScheme.C6, ColorScheme.C6a, inv_rnd_end_diag.ms_ctr.ToString()),
                        new DisplayField("queries",			60,	ColorScheme.C6, ColorScheme.C6a, inv_rnd_end_diag.querys.ToString()),
                        new DisplayField("rn16 rcv",		60,	ColorScheme.C6, ColorScheme.C6a, inv_rnd_end_diag.rn16rcv.ToString()),
                        new DisplayField("rn16 to",			81,	ColorScheme.C6, ColorScheme.C6a, inv_rnd_end_diag.rn16to.ToString()),
                        new DisplayField("epc to",			81,	ColorScheme.C6, ColorScheme.C6a, inv_rnd_end_diag.epcto.ToString()),
                        new DisplayField("good reads",		82,	ColorScheme.C6, ColorScheme.C6a, inv_rnd_end_diag.good_reads.ToString()),
                        new DisplayField("bad crc",			81,	ColorScheme.C6, ColorScheme.C6a, inv_rnd_end_diag.crc_failures.ToString()),
                    };
                        recordlayout = invEndDiagLayout;
                    }
                    break;

                case PacketData.PacketType.ISO18K6C_INVENTORY_DIAG:
                    {
                        PacketData.inventory_diag inventory_diag = packet as PacketData.inventory_diag;
                        System.Diagnostics.Debug.Assert(inventory_diag != null);
                        DisplayField[] inventoryDiagLayout =
                    {
                        new DisplayField("reader",			55, ColorScheme.C1, ColorScheme.C1a, envelope.ReaderIndex.ToString()),
                        new DisplayField("packet",			55, ColorScheme.C1, ColorScheme.C1a, envelope.PacketNumber.ToString()),
                        new DisplayField("timestamp",		69, ColorScheme.C1, ColorScheme.C1a, envelope.Timestamp.ToString("mm:ss.fff")),
                        new DisplayField("ver",				21, ColorScheme.C2, ColorScheme.C2a, inventory_diag.pkt_ver.ToString()),
                        new DisplayField("flg",				31, ColorScheme.C2, ColorScheme.C2a, String.Format("0x{0:x}", inventory_diag.flags)),
                        new DisplayField("len",				31, ColorScheme.C2, ColorScheme.C2a, inventory_diag.pkt_len.ToString()),
                        new DisplayField("type",			94, ColorScheme.C3, ColorScheme.C3a, envelope.PacketTypeName),
                        new DisplayField("prot_parms",		70, ColorScheme.C6, ColorScheme.C6a, inventory_diag.prot_parms.ToString()),
                    };
                        recordlayout = inventoryDiagLayout;
                    }
                    break;

                    case PacketData.PacketType.INVENTORY_CYCLE_BEGIN:
                        {
                            PacketData.inv_cyc_beg inv_cyc_beg = packet as PacketData.inv_cyc_beg;
                            System.Diagnostics.Debug.Assert(inv_cyc_beg != null);
                            DisplayField[] layout =
                                {
                                    new DisplayField("reader",			55, ColorScheme.C1, ColorScheme.C1a, envelope.ReaderIndex.ToString()),
                                    new DisplayField("packet",			55, ColorScheme.C1, ColorScheme.C1a, envelope.PacketNumber.ToString()),
                                    new DisplayField("timestamp",		69, ColorScheme.C1, ColorScheme.C1a, envelope.Timestamp.ToString("mm:ss.fff")),
                                    new DisplayField("ver",				21, ColorScheme.C2, ColorScheme.C2a, inv_cyc_beg.pkt_ver.ToString()),
                                    new DisplayField("flg",				31, ColorScheme.C2, ColorScheme.C2a, String.Format("0x{0:x}", inv_cyc_beg.flags)),
                                    new DisplayField("len",				31, ColorScheme.C2, ColorScheme.C2a, inv_cyc_beg.pkt_len.ToString()),
                                    new DisplayField("type",			94, ColorScheme.C3, ColorScheme.C3a, envelope.PacketTypeName),
                                    new DisplayField("ms_ctr",			70,	ColorScheme.C6, ColorScheme.C6a, inv_cyc_beg.ms_ctr.ToString()),
                                };
                            recordlayout = layout;
                        }
                        break;

                    case PacketData.PacketType.INVENTORY_CYCLE_END:
                        {
                            PacketData.inv_cyc_end inv_cyc_end = packet as PacketData.inv_cyc_end;
                            System.Diagnostics.Debug.Assert(inv_cyc_end != null);
                            DisplayField[] layout =
                                {
                                    new DisplayField("reader",			55, ColorScheme.C1, ColorScheme.C1a, envelope.ReaderIndex.ToString()),
                                    new DisplayField("packet",			55, ColorScheme.C1, ColorScheme.C1a, envelope.PacketNumber.ToString()),
                                    new DisplayField("timestamp",		69, ColorScheme.C1, ColorScheme.C1a, envelope.Timestamp.ToString("mm:ss.fff")),
                                    new DisplayField("ver",				21, ColorScheme.C2, ColorScheme.C2a, inv_cyc_end.pkt_ver.ToString()),
                                    new DisplayField("flg",				31, ColorScheme.C2, ColorScheme.C2a, String.Format("0x{0:x}", inv_cyc_end.flags)),
                                    new DisplayField("len",				31, ColorScheme.C2, ColorScheme.C2a, inv_cyc_end.pkt_len.ToString()),
                                    new DisplayField("type",			94, ColorScheme.C3, ColorScheme.C3a, envelope.PacketTypeName),
                                    new DisplayField("ms_ctr",			70,	ColorScheme.C6, ColorScheme.C6a, inv_cyc_end.ms_ctr.ToString()),
                                };
                            recordlayout = layout;
                        }
                        break;

                    case PacketData.PacketType.CARRIER_INFO:
                        {
                            PacketData.carrier_info carrier_info = packet as PacketData.carrier_info;
                            System.Diagnostics.Debug.Assert(carrier_info != null);
                            Double calc_freq = ( 24000.0 / ( 4 * ( ( carrier_info.plldivmult >> 0x10 ) & 0x0FF) ) ) * ( carrier_info.plldivmult & 0x0FFFF ) / 1000;
                            DisplayField[] layout =
                                {
                                    new DisplayField("reader",			55, ColorScheme.C1, ColorScheme.C1a, envelope.ReaderIndex.ToString()),
                                    new DisplayField("packet",			55, ColorScheme.C1, ColorScheme.C1a, envelope.PacketNumber.ToString()),
                                    new DisplayField("timestamp",		69, ColorScheme.C1, ColorScheme.C1a, envelope.Timestamp.ToString("mm:ss.fff")),
                                    new DisplayField("ver",				21, ColorScheme.C2, ColorScheme.C2a, carrier_info.pkt_ver.ToString()),
                                    new DisplayField("flg",				31, ColorScheme.C2, ColorScheme.C2a, String.Format("0x{0:x}", carrier_info.flags)),
                                    new DisplayField("len",				31, ColorScheme.C2, ColorScheme.C2a, carrier_info.pkt_len.ToString()),
                                    new DisplayField("type",			94, ColorScheme.C3, ColorScheme.C3a, envelope.PacketTypeName),
                                    new DisplayField("ms_ctr",			70,	ColorScheme.C6, ColorScheme.C6a, carrier_info.ms_ctr.ToString()),
                                    new DisplayField("channel",			60,	ColorScheme.C6, ColorScheme.C6a, carrier_info.chan.ToString()),
                                    new DisplayField("carrier",			60,	ColorScheme.C6, ColorScheme.C6a, carrier_info.cw_flags == (int)PacketData.PacketBase.carrierResult.carrier_on ?  "on" : "off"),
                                    new DisplayField("plldivmult",	   130,	ColorScheme.C6, ColorScheme.C6a, String.Format("0x{0:X} ({1:F2} MHz)", carrier_info.plldivmult, calc_freq)),
                               };
                            recordlayout = layout;
                        }
                        break;
                    case PacketData.PacketType.INVENTORY_CYCLE_END_DIAGS:
                        {
                            PacketData.inv_cyc_end_diag inv_cyc_end_diag = packet as PacketData.inv_cyc_end_diag;
                            System.Diagnostics.Debug.Assert(inv_cyc_end_diag != null);
                            DisplayField[] layout =
                                {
                                    new DisplayField("reader",			55, ColorScheme.C1, ColorScheme.C1a, envelope.ReaderIndex.ToString()),
                                    new DisplayField("packet",			55, ColorScheme.C1, ColorScheme.C1a, envelope.PacketNumber.ToString()),
                                    new DisplayField("timestamp",		69, ColorScheme.C1, ColorScheme.C1a, envelope.Timestamp.ToString("mm:ss.fff")),
                                    new DisplayField("ver",				21, ColorScheme.C2, ColorScheme.C2a, inv_cyc_end_diag.pkt_ver.ToString()),
                                    new DisplayField("flg",				31, ColorScheme.C2, ColorScheme.C2a, String.Format("0x{0:x}", inv_cyc_end_diag.flags)),
                                    new DisplayField("len",				31, ColorScheme.C2, ColorScheme.C2a, inv_cyc_end_diag.pkt_len.ToString()),
                                    new DisplayField("type",			94, ColorScheme.C3, ColorScheme.C3a, envelope.PacketTypeName),
                                    new DisplayField("queries",			60,	ColorScheme.C6, ColorScheme.C6a, inv_cyc_end_diag.querys.ToString()),
                                    new DisplayField("rn16 rcv",		60,	ColorScheme.C6, ColorScheme.C6a, inv_cyc_end_diag.rn16rcv.ToString()),
                                    new DisplayField("rn16 to",			81,	ColorScheme.C6, ColorScheme.C6a, inv_cyc_end_diag.rn16to.ToString()),
                                    new DisplayField("epc to",			81,	ColorScheme.C6, ColorScheme.C6a, inv_cyc_end_diag.epcto.ToString()),
                                    new DisplayField("good reads",		82,	ColorScheme.C6, ColorScheme.C6a, inv_cyc_end_diag.good_reads.ToString()),
                                    new DisplayField("bad crc",			81,	ColorScheme.C6, ColorScheme.C6a, inv_cyc_end_diag.crc_failures.ToString()),
                                };
                            recordlayout = layout;
                        }
                        break;

                    case PacketData.PacketType.ISO18K6C_TAG_ACCESS:
                    {
                        PacketData.Iso18k6c_access Iso18k6c_access = packet as PacketData.Iso18k6c_access;
                        System.Diagnostics.Debug.Assert(Iso18k6c_access != null);
                        BitVector32 flags = new BitVector32(Iso18k6c_access.flags);
                        bool bsError = flags[PacketData.PacketBase.accessTagErrFlag] == (int)PacketData.PacketBase.ISO_18000_6C_TagErrorFlag.BackscatterError;
                        bool success = flags[PacketData.PacketBase.accessErrorFlag] == (int)PacketData.PacketBase.ISO_18000_6C_ErrorFlag.AccessSucceeded;
                        string tagData = BitConverter.ToString(Iso18k6c_access.data, 0, Iso18k6c_access.data.Length - flags[PacketData.PacketBase.paddingBytes]);
                        if (tagData.Length > 30) tagData = tagData.Substring(0, 27) + "\u2026";

                        DisplayField[] Iso18k6c_accessLayout =
                        {
                        new DisplayField("reader",			55, ColorScheme.C1, ColorScheme.C1a, envelope.ReaderIndex.ToString()),
                        new DisplayField("packet",			55, ColorScheme.C1, ColorScheme.C1a, envelope.PacketNumber.ToString()),
                        new DisplayField("timestamp",		69, ColorScheme.C1, ColorScheme.C1a, envelope.Timestamp.ToString("mm:ss.fff")),
                        new DisplayField("ver",				21, ColorScheme.C2, ColorScheme.C2a, Iso18k6c_access.pkt_ver.ToString()),
                        new DisplayField("flg",				31, ColorScheme.C2, ColorScheme.C2a, String.Format("0x{0:x}", Iso18k6c_access.flags)),
                        new DisplayField("len",				31, ColorScheme.C2, ColorScheme.C2a, Iso18k6c_access.pkt_len.ToString()),
                        new DisplayField("type",			94, ColorScheme.C3, ColorScheme.C3a, envelope.PacketTypeName),
                        new DisplayField("crc",				30, flags[PacketData.PacketBase.accessCRCFlag] == (int)PacketData.PacketBase.CrcResultValues.Good ? ColorScheme.C4 : ColorScheme.C4Err, 
                                                                flags[PacketData.PacketBase.accessCRCFlag] == (int)PacketData.PacketBase.CrcResultValues.Good ? ColorScheme.C4a : ColorScheme.C4a, flags[PacketData.PacketBase.accessCRCFlag] == 	(int)PacketData.PacketBase.CrcResultValues.Good ? "ok" : "BAD"),
                        new DisplayField("pad",				30, ColorScheme.C4, ColorScheme.C4a, flags[PacketData.PacketBase.accessPadding].ToString()),
                        new DisplayField("ack",				30, ColorScheme.C4, ColorScheme.C4a, flags[PacketData.PacketBase.accessAckTOFlag] == (int)PacketData.PacketBase.ISO_18000_6C_AckTimeoutFlag.AckReceived ? "ack" : "NO"),
                        new DisplayField("success",			50, !success ? ColorScheme.C4Err : ColorScheme.C4, 
                                                                !success ? ColorScheme.C4Err : ColorScheme.C4a, success ? "yes" : "no"),
                        new DisplayField("tag error",		60, bsError ? ColorScheme.C4Err : ColorScheme.C4, 
                                                                bsError ? ColorScheme.C4Erra : ColorScheme.C4a, PacketData.PacketBase.GetTagErrorName(bsError, Iso18k6c_access.tag_error_code)),
                        new DisplayField("prot error",		60, (Iso18k6c_access.prot_error_type > 0 ) ? ColorScheme.C4Err : ColorScheme.C4, 
                                                                (Iso18k6c_access.prot_error_type > 0 ) ? ColorScheme.C4Erra : ColorScheme.C4a, PacketData.PacketBase.GetProtErrorName(Iso18k6c_access.prot_error_type)),
                        new DisplayField("command",			60, ColorScheme.C4, ColorScheme.C4a, PacketData.PacketBase.GetTagAccessTypeName(Iso18k6c_access.command)),
                        new DisplayField("write count",		90, ColorScheme.C4, ColorScheme.C4a, Iso18k6c_access.write_word_count.ToString()),
                        new DisplayField("data",			165, ColorScheme.C4, ColorScheme.C4a, tagData),
                        };
                        recordlayout = Iso18k6c_accessLayout;
                    }
                    break;

                case PacketData.PacketType.FREQUENCY_HOP_DIAG:
                    break;

                case PacketData.PacketType.NONCRITICAL_FAULT:
                    break;

                case PacketData.PacketType.DEBUG:
                    {
                        PacketData.debug debugPacket = packet as PacketData.debug;
                        System.Diagnostics.Debug.Assert(debugPacket != null);
                        BitVector32 flags = new BitVector32(debugPacket.flags);
                        string data = BitConverter.ToString(debugPacket.data, 0, debugPacket.data.Length - flags[PacketData.PacketBase.paddingBytes]);
                        if (data.Length > 59) data = data.Substring(0, 56) + "\u2026";
                        DisplayField[] debugLayout =
                    {
                        new DisplayField("reader",			55, ColorScheme.C1, ColorScheme.C1a, envelope.ReaderIndex.ToString()),
                        new DisplayField("packet",			55, ColorScheme.C1, ColorScheme.C1a, envelope.PacketNumber.ToString()),
                        new DisplayField("timestamp",		69, ColorScheme.C1, ColorScheme.C1a, envelope.Timestamp.ToString("mm:ss.fff")),
                        new DisplayField("ver",				21, ColorScheme.C2, ColorScheme.C2a, debugPacket.pkt_ver.ToString()),
                        new DisplayField("flg",				31, ColorScheme.C2, ColorScheme.C2a, String.Format("0x{0:x}", debugPacket.flags)),
                        new DisplayField("len",				31, ColorScheme.C2, ColorScheme.C2a, debugPacket.pkt_len.ToString()),
                        new DisplayField("type",			94, ColorScheme.C3, ColorScheme.C3a, envelope.PacketTypeName),
                        new DisplayField("ms_ctr",			70,	ColorScheme.C4, ColorScheme.C4a, debugPacket.ms_ctr.ToString()),
                        new DisplayField("id",				30, ColorScheme.C4, ColorScheme.C4a, debugPacket.id.ToString()),
                        new DisplayField("counter",			60, ColorScheme.C4, ColorScheme.C4a, debugPacket.counter.ToString()),
                        new DisplayField("data",	325, ColorScheme.C4, ColorScheme.C4a, data),
                    };
                        recordlayout = debugLayout;
                    }
                    break;


                case PacketData.PacketType.U_N_D_F_I_N_E_D:
                default:
                    {
                        DisplayField[] unknownLayout =
                        {
                            new DisplayField("reader",			55, ColorScheme.C1, ColorScheme.C1a, envelope.ReaderIndex.ToString()),
                            new DisplayField("packet",			55, ColorScheme.CBad, ColorScheme.CBad, envelope.PacketNumber.ToString()),
                            new DisplayField("timestamp",		69, ColorScheme.CBad, ColorScheme.CBad, envelope.Timestamp.ToString("mm:ss.fff")),
                            new DisplayField("ver",				21, ColorScheme.CBad, ColorScheme.CBad),
                            new DisplayField("flg",				31, ColorScheme.CBad, ColorScheme.CBad),
                            new DisplayField("len",				31, ColorScheme.CBad, ColorScheme.CBad),
                            new DisplayField("type",			94, ColorScheme.CBad, ColorScheme.CBad),
                            new DisplayField("invalid packet - raw buffer dump",
                                                                515, ColorScheme.CBad, ColorScheme.CBad, BitConverter.ToString(envelope.RawPacket)),
                        };
                        recordlayout = unknownLayout;
                    }
                    break;
                }
            }
            if (recordlayout != null)
                LayoutRecord(graphics, drawingRect, recordlayout);
        }

        private void LayoutRecord(Graphics graphics, Rectangle drawingRect, DisplayField[] fields)
        {
            if (graphics == null)
            {
                System.Diagnostics.Debug.Assert(false, "graphics is null");
                throw new ArgumentNullException("graphics");
            }

            if (drawingRect.IsEmpty)
            {
                System.Diagnostics.Debug.Assert(false, "drawingRect is empty.");
                throw new ArgumentException("drawingRect is empty.", "drawingRect");
            }
            if (fields == null)
            {
                System.Diagnostics.Debug.Assert(false, "fields is null");
                throw new ArgumentNullException("fields");
            }

            int topRowHeight = ShowCaption ? (int)Math.Round((double)drawingRect.Height / 2.0) : 2;
            int bottomRowHeight = drawingRect.Height - topRowHeight - 1;
            int currentOffset = 0;

            foreach (DisplayField field in fields)
            {
                Rectangle captionRect	= new Rectangle(drawingRect.X + currentOffset, drawingRect.Y, field.Width, topRowHeight);
                Rectangle valueRect		= new Rectangle(drawingRect.X + currentOffset, drawingRect.Y + topRowHeight, field.Width, bottomRowHeight);

                SizeF CaptionTextSize = graphics.MeasureString(field.Name, TextScheme.F1);
                System.Diagnostics.Debug.WriteLineIf(ShowCaption && topRowHeight < CaptionTextSize.Height, "Caption row is too short.", String.Format("Required height is {0}, but actual height is {1}.", CaptionTextSize.Height, topRowHeight));
                System.Diagnostics.Debug.WriteLineIf(ShowCaption && field.Width < CaptionTextSize.Width, String.Format("Field \"{0}\" is too small to hold caption.", field.Name), String.Format("Required width is {0}, but actual width is {1}.", CaptionTextSize.Width, field.Width));
                int CaptionTextVertOffset = (int)Math.Ceiling((topRowHeight - CaptionTextSize.Height) / 2.0);
                int CaptionTextHorzOffset = (int)Math.Ceiling((field.Width - CaptionTextSize.Width) / 2.0);

                SizeF ValueTextSize = graphics.MeasureString(field.Value, TextScheme.F1);
                System.Diagnostics.Debug.WriteLineIf(bottomRowHeight < ValueTextSize.Height, "Value row is too short.", String.Format("Required height is {0}, but actual height is {1}.", ValueTextSize.Height, bottomRowHeight));
                System.Diagnostics.Debug.WriteLineIf(field.Width < ValueTextSize.Width, String.Format("Field \"{0}\" is too small to hold value.", field.Name), String.Format("Required width is {0}, but actual width is {1}.", ValueTextSize.Width, field.Width));
                int ValueTextVertOffset = (int)Math.Ceiling((bottomRowHeight - ValueTextSize.Height) / 2.0);
                int ValueTextHorzOffset = (int)Math.Ceiling((field.Width - ValueTextSize.Width) / 2.0);

                Brush captionBrush = new SolidBrush(field.CaptionBgColor);
                Brush valueBrush = ShowCaption ? new SolidBrush(ColorScheme.C0) : new SolidBrush(field.ValueBgColor);

                if (ShowCaption)
                {
                    graphics.FillRectangle(captionBrush, captionRect);
                    graphics.FillRectangle(valueBrush, valueRect);
                    graphics.DrawRectangle(Pens.Black, captionRect);
                    graphics.DrawRectangle(Pens.Black, valueRect);
                    graphics.DrawString(field.Name, TextScheme.F1, Brushes.Black, captionRect.X + CaptionTextHorzOffset, captionRect.Y + CaptionTextVertOffset);
                }
                else
                {
                    graphics.FillRectangle(valueBrush, valueRect);
                    graphics.DrawRectangle(Pens.DarkGray, valueRect);
                }

                graphics.DrawString(field.Value, TextScheme.F1, Brushes.Black, valueRect.X + ValueTextHorzOffset, valueRect.Y + ValueTextVertOffset);

                captionBrush.Dispose();
                valueBrush.Dispose();

                currentOffset += field.Width;
            }
        }

        void _Timer_Tick(object sender, EventArgs e)
        {
            if( Context.Count > 0 && CurrentContext != null &&
                FreezeCheckbox                      != null &&
                GetDataPending(CurrentContext)              &&
                (GetControlState(CurrentContext) == ControlStateValue.FrozenActive ||
                GetControlState(CurrentContext) == ControlStateValue.FrozenReview) )
            {
                FreezeCheckbox.BackColor = InvertColor(FreezeCheckbox.BackColor);
                FreezeCheckbox.ForeColor = InvertColor(FreezeCheckbox.ForeColor);
            }
        }
    }
}
