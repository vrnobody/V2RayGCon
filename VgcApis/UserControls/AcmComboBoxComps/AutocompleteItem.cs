using System;
using System.Drawing;

namespace VgcApis.UserControls.AcmComboBoxComps
{
    /// <summary>
    /// Item of autocomplete menu
    /// </summary>
    public class AutocompleteItem
    {
        public object Tag;
        string toolTipTitle;
        string toolTipText;
        Color? toolTipBackColor = null;
        Color? toolTipForeColor = null;
        string menuText;

        /// <summary>
        /// Parent AutocompleteMenu
        /// </summary>
        public AutocompleteMenu Parent { get; internal set; }

        /// <summary>
        /// Text for inserting into textbox
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Image index for this item
        /// </summary>
        public int ImageIndex { get; set; }

        /// <summary>
        /// Alignment. The direction the text shows.
        /// </summary>
        public StringAlignment Alignment { get; set; }

        /// <summary>
        /// Title for tooltip.
        /// </summary>
        /// <remarks>Return null for disable tooltip for this item</remarks>
        public virtual string ToolTipTitle
        {
            get { return toolTipTitle; }
            set { toolTipTitle = value; }
        }

        /// <summary>
        /// Tooltip text.
        /// </summary>
        /// <remarks>For display tooltip text, ToolTipTitle must be not null</remarks>
        public virtual string ToolTipText
        {
            get { return toolTipText; }
            set { toolTipText = value; }
        }

        /// <summary>
        /// Tooltip Backcolor.
        /// </summary>
        /// <remarks>For display tooltip backcolor, ToolTipTitle must be not null</remarks>
        public virtual Color? ToolTipBackColor
        {
            get { return toolTipBackColor; }
            set { toolTipBackColor = value; }
        }

        /// <summary>
        /// Tooltip ForeColor.
        /// </summary>
        /// <remarks>For display tooltip forecolor, ToolTipTitle must be not null</remarks>
        public virtual Color? ToolTipForeColor
        {
            get { return toolTipForeColor; }
            set { toolTipForeColor = value; }
        }

        /// <summary>
        /// Menu text. This text is displayed in the drop-down menu.
        /// </summary>
        public virtual string MenuText
        {
            get { return menuText; }
            set { menuText = value; }
        }

        public AutocompleteItem()
        {
            ImageIndex = -1;
        }

        public AutocompleteItem(string text)
            : this()
        {
            Text = text;
            Alignment = StringAlignment.Near;
        }

        public AutocompleteItem(string text, int imageIndex)
            : this(text)
        {
            this.ImageIndex = imageIndex;
        }

        public AutocompleteItem(string text, int imageIndex, string menuText)
            : this(text, imageIndex)
        {
            this.menuText = menuText;
        }

        public AutocompleteItem(
            string text,
            int imageIndex,
            string menuText,
            string toolTipTitle,
            string toolTipText
        )
            : this(text, imageIndex, menuText)
        {
            this.toolTipTitle = toolTipTitle;
            this.toolTipText = toolTipText;
        }

        /// <summary>
        /// Returns text for inserting into Textbox
        /// </summary>
        public virtual string GetTextForReplace()
        {
            return Text;
        }

        /// <summary>
        /// Compares fragment text with this item
        /// </summary>
        public virtual CompareResult Compare(string fragmentText)
        {
            if (
                Text.StartsWith(fragmentText, StringComparison.InvariantCultureIgnoreCase)
                && Text != fragmentText
            )
                return CompareResult.VisibleAndSelected;

            return CompareResult.Hidden;
        }

        /// <summary>
        /// Returns text for display into popup menu
        /// </summary>
        public override string ToString()
        {
            return menuText ?? Text;
        }

        /// <summary>
        /// This method is called after item was inserted into text
        /// </summary>
        public virtual void OnSelected(SelectedEventArgs e) { }

        public virtual void OnPaint(PaintItemEventArgs e)
        {
            using (
                var brush = new SolidBrush(
                    e.IsSelected ? e.Colors.SelectedForeColor : e.Colors.ForeColor
                )
            )
                e.Graphics.DrawString(
                    ToString(),
                    e.Font,
                    brush,
                    e.TextRect,
                    new StringFormat() { Alignment = Alignment }
                );
        }
    }

    public enum CompareResult
    {
        /// <summary>
        /// Item do not appears
        /// </summary>
        Hidden,

        /// <summary>
        /// Item appears
        /// </summary>
        Visible,

        /// <summary>
        /// Item appears and will selected
        /// </summary>
        VisibleAndSelected,
    }
}
