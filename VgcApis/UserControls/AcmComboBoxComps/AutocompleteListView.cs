using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace VgcApis.UserControls.AcmComboBoxComps
{
    [System.ComponentModel.ToolboxItem(false)]
    public class AutocompleteListView : UserControl, IAutocompleteListView
    {
        private readonly ToolTip toolTip = new ToolTip();
        public int HighlightedItemIndex { get; set; }
        private int oldItemCount;
        private int selectedItemIndex = -1;
        private IList<AutocompleteItem> visibleItems;

        /// <summary>
        /// Duration (ms) of tooltip showing
        /// </summary>
        public int ToolTipDuration { get; set; }

        /// <summary>
        /// Occurs when user selected item for inserting into text
        /// </summary>
        public event EventHandler ItemSelected;

        /// <summary>
        /// Occurs when current hovered item is changing
        /// </summary>
        public event EventHandler<HoveredEventArgs> ItemHovered;

        /// <summary>
        /// Colors
        /// </summary>
        public Colors Colors { get; set; }

        internal AutocompleteListView()
        {
            SetStyle(
                ControlStyles.AllPaintingInWmPaint
                    | ControlStyles.OptimizedDoubleBuffer
                    | ControlStyles.UserPaint,
                true
            );
            base.Font = new Font(FontFamily.GenericSansSerif, 9);
            ItemHeight = Font.Height + 2;
            VerticalScroll.SmallChange = ItemHeight;
            BackColor = Color.White;
            LeftPadding = 18;
            ToolTipDuration = 3000;
            Colors = new Colors();

            toolTip.OwnerDraw = true;
            toolTip.Draw += ToolTip_Draw;
        }

        private void ToolTip_Draw(object sender, DrawToolTipEventArgs e)
        {
            e.DrawBackground();
            e.DrawBorder();
            e.DrawText();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                toolTip.Dispose();
            }
            base.Dispose(disposing);
        }

        private int itemHeight;

        public int ItemHeight
        {
            get { return itemHeight; }
            set
            {
                itemHeight = value;
                VerticalScroll.SmallChange = value;
                oldItemCount = -1;
                AdjustScroll();
            }
        }

        public override Font Font
        {
            get { return base.Font; }
            set
            {
                base.Font = value;
                ItemHeight = Font.Height + 2;
            }
        }

        public int LeftPadding { get; set; }

        public ImageList ImageList { get; set; }

        public string[] ColumnsTitle { get; set; } = null;
        public int[] ColumnsWidth { get; set; } = null;

        public IList<AutocompleteItem> VisibleItems
        {
            get { return visibleItems; }
            set
            {
                visibleItems = value;
                SelectedItemIndex = -1;
                AdjustScroll();
                Invalidate();
            }
        }

        public int SelectedItemIndex
        {
            get { return selectedItemIndex; }
            set
            {
                AutocompleteItem item = null;
                if (value >= 0 && value < VisibleItems.Count)
                    item = VisibleItems[value];

                selectedItemIndex = value;

                OnItemHovered(new HoveredEventArgs() { Item = item });

                if (item != null)
                {
                    ShowToolTip(item, this.Parent);
                    ScrollToSelected();
                }

                Invalidate();
            }
        }

        private void OnItemHovered(HoveredEventArgs e)
        {
            if (ItemHovered != null)
                ItemHovered(this, e);
        }

        private void AdjustScroll()
        {
            if (VisibleItems == null)
                return;
            if (oldItemCount == VisibleItems.Count)
                return;

            int needHeight = ItemHeight * VisibleItems.Count + 1;
            if (HasColumn)
                needHeight += ItemHeight;
            Height = Math.Min(needHeight, MaximumSize.Height);
            AutoScrollMinSize = new Size(0, needHeight);
            oldItemCount = VisibleItems.Count;
        }

        private void ScrollToSelected()
        {
            int y;
            if (HasColumn)
                y = (SelectedItemIndex + 1) * ItemHeight - VerticalScroll.Value;
            else
                y = SelectedItemIndex * ItemHeight - VerticalScroll.Value;
            //if (HasColumn) y += ItemHeight;

            if (y < 0)
                if (HasColumn)
                    VerticalScroll.Value = (SelectedItemIndex + 1) * ItemHeight;
                else
                    VerticalScroll.Value = SelectedItemIndex * ItemHeight;
            if (y > ClientSize.Height - ItemHeight)
                VerticalScroll.Value = Math.Min(
                    VerticalScroll.Maximum,
                    (
                        HasColumn
                            ? (SelectedItemIndex * ItemHeight) + ItemHeight
                            : SelectedItemIndex * ItemHeight
                    )
                        - ClientSize.Height
                        + ItemHeight
                );

            //Show headers if index 0 is selected.
            if (HasColumn && SelectedItemIndex == 0)
                VerticalScroll.Value = 0;

            //some magic for update scrolls
            AutoScrollMinSize -= new Size(1, 0);
            AutoScrollMinSize += new Size(1, 0);
        }

        public Rectangle GetItemRectangle(int itemIndex)
        {
            var y = itemIndex * ItemHeight - VerticalScroll.Value;
            return new Rectangle(0, y, ClientSize.Width - 1, ItemHeight - 1);
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            e.Graphics.Clear(Colors.BackColor);
        }

        private bool HasColumn
        {
            get { return ColumnsTitle != null && ColumnsTitle.Length != 0; }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            bool rtl = RightToLeft == RightToLeft.Yes;
            AdjustScroll();
            int startI = VerticalScroll.Value / ItemHeight - 1;
            int finishI = (VerticalScroll.Value + ClientSize.Height) / ItemHeight + 1;
            int y = 0;

            var sf = new StringFormat();
            if (rtl)
                sf.FormatFlags = StringFormatFlags.DirectionRightToLeft;

            if (HasColumn && VerticalScroll.Value == 0)
            {
                var textRect = new Rectangle(
                    LeftPadding,
                    y,
                    ClientSize.Width - 1 - LeftPadding,
                    ItemHeight - 1
                );
                if (rtl)
                    textRect = new Rectangle(
                        1,
                        y,
                        ClientSize.Width - 1 - LeftPadding,
                        ItemHeight - 1
                    );

                int[] columnWidth = ColumnsWidth;
                if (columnWidth == null)
                {
                    columnWidth = new int[ColumnsTitle.Length];
                    float step = textRect.Width / ColumnsTitle.Length;
                    for (int i = 0; i < ColumnsTitle.Length; i++)
                        columnWidth[i] = (int)step;
                }

                float x = textRect.X;
                sf.Alignment = StringAlignment.Center;
                for (int i = 0; i < ColumnsTitle.Length; i++)
                {
                    using (var brush = new SolidBrush(Colors.ForeColor))
                    {
                        var width = columnWidth[i];
                        var rect = new RectangleF(x, textRect.Top, width, textRect.Height);
                        e.Graphics.DrawLine(
                            Pens.Silver,
                            new PointF(x, textRect.Top),
                            new PointF(x, textRect.Bottom)
                        );
                        e.Graphics.DrawString(ColumnsTitle[i], Font, brush, rect, sf);
                        x += width;
                    }
                }
                if (rtl)
                    sf.FormatFlags = StringFormatFlags.DirectionRightToLeft;
                else
                    sf = new StringFormat();
            }

            startI = Math.Max(startI, 0);
            finishI = Math.Min(finishI, VisibleItems.Count);

            for (int i = startI; i < finishI; i++)
            {
                if (HasColumn)
                    y = (i * ItemHeight - VerticalScroll.Value) + ItemHeight;
                else
                    y = i * ItemHeight - VerticalScroll.Value;

                if (ImageList != null && VisibleItems[i].ImageIndex >= 0)
                    if (rtl)
                        e.Graphics.DrawImage(
                            ImageList.Images[VisibleItems[i].ImageIndex],
                            Width - 1 - LeftPadding,
                            y
                        );
                    else
                        e.Graphics.DrawImage(ImageList.Images[VisibleItems[i].ImageIndex], 1, y);

                var textRect = new Rectangle(
                    LeftPadding,
                    y,
                    ClientSize.Width - 1 - LeftPadding,
                    ItemHeight - 1
                );
                if (rtl)
                    textRect = new Rectangle(
                        1,
                        y,
                        ClientSize.Width - 1 - LeftPadding,
                        ItemHeight - 1
                    );

                if (i == SelectedItemIndex)
                {
                    Brush selectedBrush = new LinearGradientBrush(
                        new Point(0, y - 3),
                        new Point(0, y + ItemHeight),
                        Colors.SelectedBackColor2,
                        Colors.SelectedBackColor
                    );
                    e.Graphics.FillRectangle(selectedBrush, textRect);
                    using (var pen = new Pen(Colors.SelectedBackColor2))
                        e.Graphics.DrawRectangle(pen, textRect);
                }

                if (i == HighlightedItemIndex)
                    using (var pen = new Pen(Colors.HighlightingColor))
                        e.Graphics.DrawRectangle(pen, textRect);

                var args = new PaintItemEventArgs(e.Graphics, e.ClipRectangle)
                {
                    Font = Font,
                    TextRect = new RectangleF(textRect.Location, textRect.Size),
                    StringFormat = sf,
                    IsSelected = i == SelectedItemIndex,
                    IsHovered = i == HighlightedItemIndex,
                    Colors = Colors,
                };
                //call drawing
                VisibleItems[i].OnPaint(args);
            }
        }

        protected override void OnScroll(ScrollEventArgs se)
        {
            base.OnScroll(se);
            Invalidate(true);
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            int i = PointToItemIndex(e.Location);
            if (i >= 0)
            {
                base.OnMouseClick(e);

                if (e.Button == MouseButtons.Left)
                {
                    SelectedItemIndex = i;
                    ScrollToSelected();
                    Invalidate();
                }
            }
        }

        private Point mouseEnterPoint;

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            mouseEnterPoint = Control.MousePosition;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (mouseEnterPoint != Control.MousePosition)
            {
                int i = PointToItemIndex(e.Location);
                if (i >= 0)
                {
                    HighlightedItemIndex = i;
                    Invalidate();
                }
            }
        }

        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            int i = PointToItemIndex(e.Location);
            if (i >= 0)
            {
                base.OnMouseDoubleClick(e);
                SelectedItemIndex = i;
                Invalidate();
                OnItemSelected();
            }
        }

        private void OnItemSelected()
        {
            if (ItemSelected != null)
                ItemSelected(this, EventArgs.Empty);
        }

        private int PointToItemIndex(Point p)
        {
            var t = (p.Y + VerticalScroll.Value);
            if (HasColumn)
                t -= ItemHeight;
            return (float)t / ItemHeight < 0 ? -1 : t / ItemHeight;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            var host = Parent as AutocompleteMenuHost;
            if (host != null)
                if (host.Menu.ProcessKey((char)keyData, Keys.None))
                    return true;

            return base.ProcessCmdKey(ref msg, keyData);
        }

        public void SelectItem(int itemIndex)
        {
            SelectedItemIndex = itemIndex;
            ScrollToSelected();
            Invalidate();
        }

        public void SetItems(List<AutocompleteItem> items)
        {
            VisibleItems = items;
            SelectedItemIndex = -1;
            AdjustScroll();
            Invalidate();
        }

        public void ShowToolTip(AutocompleteItem autocompleteItem, Control control = null)
        {
            string title = autocompleteItem.ToolTipTitle;
            string text = autocompleteItem.ToolTipText;
            Color? backColor = autocompleteItem.ToolTipBackColor;
            Color? foreColor = autocompleteItem.ToolTipForeColor;

            if (control == null)
                control = this;

            if (string.IsNullOrEmpty(title))
            {
                toolTip.SetToolTip(control, null);
                return;
            }

            if (backColor != null)
            {
                toolTip.BackColor = (Color)backColor;
            }

            if (foreColor != null)
            {
                toolTip.ForeColor = (Color)foreColor;
            }

            if (string.IsNullOrEmpty(text))
            {
                toolTip.Show(title, control, Width + 3, 0, ToolTipDuration);
            }
            else
            {
                toolTip.Show(text, control, Width + 3, 0, ToolTipDuration);
            }
        }

        public void HideToolTip(Control control)
        {
            toolTip.Hide(control);
        }

        public Control GetParentControl()
        {
            return this.Parent;
        }
    }
}
