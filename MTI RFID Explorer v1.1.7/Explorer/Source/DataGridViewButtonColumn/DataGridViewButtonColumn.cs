using System;
using System.Collections.Generic;
using System.Text;


using System.Drawing;
using System.Windows.Forms;

namespace my_DataGridViewButtonColumn
{
    //Button
    public class DataGridViewDisableButtonColumn : DataGridViewButtonColumn
    {
        public DataGridViewDisableButtonColumn()
        {
            this.CellTemplate = new DataGridViewDisableButtonCell();
        }
    }

    public class DataGridViewDisableButtonCell : DataGridViewButtonCell
    {
        private bool enabledValue;
        public bool Enabled
        {
            get
            {
                return enabledValue;
            }
            set
            {
                enabledValue = value;
            }
        }

        public override object Clone()
        {
            DataGridViewDisableButtonCell cell = (DataGridViewDisableButtonCell)base.Clone();
            cell.Enabled = this.Enabled;
            return cell;
        }

        public DataGridViewDisableButtonCell()
        {
            this.enabledValue = true;
        }

        protected override void Paint
        (
            Graphics                        graphics,
            Rectangle                       clipBounds, 
            Rectangle                       cellBounds, 
            int                             rowIndex,
            DataGridViewElementStates       elementState, 
            object                          value,
            object                          formattedValue, 
            string                          errorText,
            DataGridViewCellStyle           cellStyle,
            DataGridViewAdvancedBorderStyle advancedBorderStyle,
            DataGridViewPaintParts          paintParts
        )
        {
            if (!this.enabledValue)
            {
                
                if 
                (
                    (paintParts & DataGridViewPaintParts.Background) 
                    ==
                    DataGridViewPaintParts.Background
                )
                {
                    SolidBrush cellBackground = new SolidBrush(cellStyle.BackColor);
                    graphics.FillRectangle(cellBackground, cellBounds);
                    cellBackground.Dispose();
                }

                if 
                (
                    (paintParts & DataGridViewPaintParts.Border) 
                    ==
                    DataGridViewPaintParts.Border
                )
                {
                    PaintBorder(graphics, clipBounds, cellBounds, cellStyle,advancedBorderStyle);
                }

                Rectangle buttonArea       = cellBounds;
                Rectangle buttonAdjustment = this.BorderWidths(advancedBorderStyle);
                buttonArea.X += buttonAdjustment.X;
                buttonArea.Y += buttonAdjustment.Y;
                buttonArea.Height -= buttonAdjustment.Height;
                buttonArea.Width  -= buttonAdjustment.Width;

                ButtonRenderer.DrawButton
                              ( graphics, 
                                buttonArea,
                                System.Windows.Forms.VisualStyles.PushButtonState.Disabled);

                if (this.FormattedValue is String)
                {
                    TextRenderer.DrawText( graphics,
                                           (string)this.FormattedValue,
                                           this.DataGridView.Font,
                                           buttonArea, 
                                           SystemColors.GrayText);
                }
            }
            else
            {
                base.Paint(graphics, clipBounds, cellBounds, rowIndex,
                           elementState, value, formattedValue, errorText,
                           cellStyle, advancedBorderStyle, paintParts);
            }//if outside
        }



    }
}
