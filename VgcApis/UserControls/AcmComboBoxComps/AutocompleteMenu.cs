//
//  THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
//  KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
//  IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
//  PURPOSE.
//
//  License: GNU Lesser General Public License (LGPLv3)
//
//  Email: p_torgashov@ukr.net.
//
//  Copyright (C) Pavel Torgashov, 2012-2015.

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Windows.Forms;

namespace VgcApis.UserControls.AcmComboBoxComps
{
    [ProvideProperty("AutocompleteMenu", typeof(Control))]
    public class AutocompleteMenu : Component
    {
        private static readonly Dictionary<Control, AutocompleteMenu> AutocompleteMenuByControls =
            new Dictionary<Control, AutocompleteMenu>();
        private static readonly Dictionary<Control, ITextBoxWrapper> WrapperByControls =
            new Dictionary<Control, ITextBoxWrapper>();

        private ITextBoxWrapper targetControlWrapper;
        private readonly Timer timer = new Timer();

        private IEnumerable<AutocompleteItem> sourceItems = new List<AutocompleteItem>();

        [Browsable(false)]
        public IList<AutocompleteItem> VisibleItems
        {
            get { return Host.ListView.VisibleItems; }
            private set { Host.ListView.VisibleItems = value; }
        }
        private Size maximumSize;

        /// <summary>
        /// Duration (ms) of tooltip showing
        /// </summary>
        [Description("Duration (ms) of tooltip showing")]
        [DefaultValue(3000)]
        public int ToolTipDuration
        {
            get { return Host.ListView.ToolTipDuration; }
            set { Host.ListView.ToolTipDuration = value; }
        }

        public AutocompleteMenu()
        {
            Host = new AutocompleteMenuHost(this);
            Host.ListView.ItemSelected += new EventHandler(ListView_ItemSelected);
            Host.ListView.ItemHovered += new EventHandler<HoveredEventArgs>(ListView_ItemHovered);
            VisibleItems = new List<AutocompleteItem>();
            Enabled = true;
            AppearInterval = 500;
            timer.Tick += timer_Tick;
            MaximumSize = new Size(180, 200);
            AutoPopup = true;

            MinFragmentLength = 2;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                timer.Dispose();
                Host.Dispose();
            }
            base.Dispose(disposing);
        }

        void ListView_ItemSelected(object sender, EventArgs e)
        {
            OnSelecting();
        }

        void ListView_ItemHovered(object sender, HoveredEventArgs e) { }

        [Browsable(false)]
        public int SelectedItemIndex
        {
            get { return Host.ListView.SelectedItemIndex; }
            internal set { Host.ListView.SelectedItemIndex = value; }
        }

        internal AutocompleteMenuHost Host { get; set; }

        /// <summary>
        /// Current target control wrapper
        /// </summary>
        [Browsable(false)]
        public ITextBoxWrapper TargetControlWrapper
        {
            get { return targetControlWrapper; }
            set
            {
                targetControlWrapper = value;
                if (value != null && !WrapperByControls.ContainsKey(value.TargetControl))
                {
                    WrapperByControls[value.TargetControl] = value;
                    SetAutocompleteMenu(value.TargetControl, this);
                }
            }
        }

        /// <summary>
        /// Maximum size of popup menu
        /// </summary>
        [DefaultValue(typeof(Size), "180, 200")]
        [Description("Maximum size of popup menu")]
        public Size MaximumSize
        {
            get { return maximumSize; }
            set
            {
                maximumSize = value;
                (Host.ListView as Control).MaximumSize = maximumSize;
                (Host.ListView as Control).Size = maximumSize;
                Host.CalcSize();
            }
        }

        /// <summary>
        /// Font
        /// </summary>
        public Font Font
        {
            get { return (Host.ListView as Control).Font; }
            set { (Host.ListView as Control).Font = value; }
        }

        /// <summary>
        /// Left padding of text
        /// </summary>
        [DefaultValue(18)]
        [Description("Left padding of text")]
        public int LeftPadding
        {
            get
            {
                if (Host.ListView is AutocompleteListView)
                    return (Host.ListView as AutocompleteListView).LeftPadding;
                else
                    return 0;
            }
            set
            {
                if (Host.ListView is AutocompleteListView)
                    (Host.ListView as AutocompleteListView).LeftPadding = value;
            }
        }

        /// <summary>
        /// Colors of foreground and background
        /// </summary>
        [Browsable(true)]
        [Description("Colors of foreground and background.")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public Colors Colors
        {
            get { return (Host.ListView as IAutocompleteListView).Colors; }
            set { (Host.ListView as IAutocompleteListView).Colors = value; }
        }

        /// <summary>
        /// AutocompleteMenu will popup automatically (when user writes text). Otherwise it will popup only programmatically or by Ctrl-Space.
        /// </summary>
        [DefaultValue(true)]
        [Description(
            "AutocompleteMenu will popup automatically (when user writes text). Otherwise it will popup only programmatically or by Ctrl-Space."
        )]
        public bool AutoPopup { get; set; }

        /// <summary>
        /// AutocompleteMenu will capture focus when opening.
        /// </summary>
        [DefaultValue(false)]
        [Description("AutocompleteMenu will capture focus when opening.")]
        public bool CaptureFocus { get; set; }

        /// <summary>
        /// Indicates whether the component should draw right-to-left for RTL languages.
        /// </summary>
        [DefaultValue(typeof(RightToLeft), "No")]
        [Description(
            "Indicates whether the component should draw right-to-left for RTL languages."
        )]
        public RightToLeft RightToLeft
        {
            get { return Host.RightToLeft; }
            set { Host.RightToLeft = value; }
        }

        /// <summary>
        /// Image list
        /// </summary>
        public ImageList ImageList
        {
            get { return Host.ListView.ImageList; }
            set { Host.ListView.ImageList = value; }
        }

        /// <summary>
        /// Fragment
        /// </summary>
        [Browsable(false)]
        public Range Fragment { get; internal set; }

        /// <summary>
        /// Minimum fragment length for popup
        /// </summary>
        [Description("Minimum fragment length for popup")]
        [DefaultValue(2)]
        public int MinFragmentLength { get; set; }

        /// <summary>
        /// Allows TAB for select menu item
        /// </summary>
        [Description("Allows TAB for select menu item")]
        [DefaultValue(false)]
        public bool AllowsTabKey { get; set; }

        /// <summary>
        /// Interval of menu appear (ms)
        /// </summary>
        [Description("Interval of menu appear (ms)")]
        [DefaultValue(500)]
        public int AppearInterval { get; set; }

        /// <summary>
        /// If true, the menu's width will be adjusted depending on visible items.
        /// </summary>
        [Description(
            "If true, the menu's width will be adjusted depending on visible items. Bounded by MaximumSize"
        )]
        [DefaultValue(false)]
        public bool AutoWidth { get; set; }

        [DefaultValue(null)]
        public string[] Items
        {
            get
            {
                if (sourceItems == null)
                    return null;
                var list = new List<string>();
                foreach (AutocompleteItem item in sourceItems)
                    list.Add(item.ToString());
                return list.ToArray();
            }
            set { SetAutocompleteItems(value); }
        }

        /// <summary>
        /// The control for menu displaying.
        /// Set to null for restore default ListView (AutocompleteListView).
        /// </summary>
        [Browsable(false)]
        public IAutocompleteListView ListView
        {
            get { return Host.ListView; }
            set
            {
                if (ListView != null)
                {
                    var ctrl = value as Control;
                    value.ImageList = ImageList;
                    ctrl.RightToLeft = RightToLeft;
                    ctrl.Font = Font;
                    ctrl.MaximumSize = MaximumSize;
                }
                Host.ListView = value;
                Host.ListView.ItemSelected += new EventHandler(ListView_ItemSelected);
                Host.ListView.ItemHovered += new EventHandler<HoveredEventArgs>(
                    ListView_ItemHovered
                );
            }
        }

        [DefaultValue(true)]
        public bool Enabled { get; set; }

        /// <summary>
        /// Updates size of the menu
        /// </summary>
        public void Update()
        {
            if (AutoWidth)
                UpdateSizeBasedOnVisible();
            else
                Host.CalcSize();
        }

        /// <summary>
        /// Returns rectangle of item
        /// </summary>
        public Rectangle GetItemRectangle(int itemIndex)
        {
            return Host.ListView.GetItemRectangle(itemIndex);
        }

        #region IExtenderProvider Members

        public void SetAutocompleteMenu(Control control, AutocompleteMenu menu)
        {
            if (menu == null)
            {
                Close();
                AutocompleteMenuByControls.TryGetValue(control, out menu);
                AutocompleteMenuByControls.Remove(control);
                if (menu != null && WrapperByControls.TryGetValue(control, out var w) && w != null)
                {
                    w.LostFocus -= menu.control_LostFocus;
                    w.KeyDown -= menu.control_KeyDown;
                    w.MouseDown -= menu.control_MouseDown;
                }
                WrapperByControls.Remove(control);
                return;
            }

            if (WrapperByControls.TryGetValue(control, out var wrapper) && wrapper != null)
            {
                AutocompleteMenuByControls[control] = menu;
                wrapper.LostFocus += menu.control_LostFocus;
                wrapper.KeyDown += menu.control_KeyDown;
                wrapper.MouseDown += menu.control_MouseDown;
                return;
            }

            throw new KeyNotFoundException("control not found!");
        }

        #endregion



        private void timer_Tick(object sender, EventArgs e)
        {
            timer.Stop();
            if (TargetControlWrapper != null)
                ShowAutocomplete(false);
        }

        private void control_MouseDown(object sender, MouseEventArgs e)
        {
            Close();
        }

        ITextBoxWrapper FindWrapper(object sender)
        {
            while (sender != null)
            {
                switch (sender)
                {
                    case Control control:
                        if (WrapperByControls.ContainsKey(control))
                            return WrapperByControls[control];
                        sender = control.Parent;
                        break;
                    case AcmComboBox cbox:
                        sender = cbox.GetCurrentParent();
                        break;
                    default:
                        return null;
                }
            }
            return null;
        }

        private void control_KeyDown(object sender, KeyEventArgs e)
        {
            TargetControlWrapper = FindWrapper(sender);

            bool backspaceORdel = e.KeyCode == Keys.Back || e.KeyCode == Keys.Delete;

            if (Host.Visible)
            {
                if (ProcessKey((char)e.KeyCode, Control.ModifierKeys))
                    e.SuppressKeyPress = true;
                else if (!backspaceORdel)
                    ResetTimer(1);
                else
                    ResetTimer();

                return;
            }

            if (!Host.Visible)
            {
                switch (e.KeyCode)
                {
                    case Keys.Up:
                    case Keys.Down:
                    case Keys.PageUp:
                    case Keys.PageDown:
                    case Keys.Left:
                    case Keys.Right:
                    case Keys.End:
                    case Keys.Home:
                    case Keys.ControlKey:
                        timer.Stop();
                        return;
                    case Keys.Escape:
                        timer.Stop();
                        TargetControlWrapper.InvokeKeyDownCallback(Keys.Escape);
                        e.SuppressKeyPress = true;
                        return;
                    case Keys.Enter:
                        TargetControlWrapper.InvokeKeyDownCallback(Keys.Enter);
                        e.SuppressKeyPress = true;
                        return;
                }
            }

            ResetTimer();
        }

        void ResetTimer()
        {
            ResetTimer(-1);
        }

        void ResetTimer(int interval)
        {
            if (interval <= 0)
                timer.Interval = AppearInterval;
            else
                timer.Interval = interval;
            timer.Stop();
            timer.Start();
        }

        private void control_LostFocus(object sender, EventArgs e)
        {
            if (!Host.Focused)
                Close();
        }

        bool forcedOpened = false;

        internal void ShowAutocomplete(bool forced)
        {
            if (forced)
                forcedOpened = true;

            if (TargetControlWrapper != null && TargetControlWrapper.Readonly)
            {
                Close();
                return;
            }

            if (!Enabled)
            {
                Close();
                return;
            }

            if (!forcedOpened && !AutoPopup)
            {
                Close();
                return;
            }

            //build list
            int selIndex = BuildAutocompleteList(forcedOpened);

            //show popup menu
            if (VisibleItems.Count > 0)
            {
                if (forced && VisibleItems.Count == 1 && Host.ListView.SelectedItemIndex == 0)
                {
                    //do autocomplete if menu contains only one line and user press CTRL-SPACE
                    OnSelecting();
                    Close();
                }
                else
                {
                    ShowMenu();

                    Host.ListView.SelectedItemIndex = selIndex;
                    Host.ListView.HighlightedItemIndex = -1;
                }
            }
            else
                Close();
        }

        private void ShowMenu()
        {
            if (!Host.Visible)
            {
                var args = new CancelEventArgs();
                if (!args.Cancel)
                {
                    //calc screen point for popup menu
                    Point point = TargetControlWrapper.TargetControl.Location;
                    point.Offset(2, TargetControlWrapper.TargetControl.Height + 2);
                    //
                    Host.Show(TargetControlWrapper.TargetControl, point);
                    if (CaptureFocus)
                    {
                        (Host.ListView as Control).Focus();
                        //ProcessKey((char) Keys.Down, Keys.None);
                    }
                }
            }
            else
                (Host.ListView as Control).Invalidate();
        }

        private int BuildAutocompleteList(bool forced)
        {
            var visibleItems = new List<AutocompleteItem>();

            bool foundSelected = false;
            int selectedIndex = -1;
            //get fragment around caret
            Range fragment = GetFragment();
            string text = fragment.Text;
            //
            if (sourceItems != null)
                if (
                    forced
                    || (
                        text.Length >= MinFragmentLength /* && tb.Selection.Start == tb.Selection.End*/
                    )
                )
                {
                    Fragment = fragment;
                    //build popup menu
                    foreach (AutocompleteItem item in sourceItems)
                    {
                        item.Parent = this;
                        CompareResult res = item.Compare(text);
                        if (res != CompareResult.Hidden)
                            visibleItems.Add(item);
                        if (res == CompareResult.VisibleAndSelected && !foundSelected)
                        {
                            foundSelected = true;
                            selectedIndex = visibleItems.Count - 1;
                        }
                    }
                }

            VisibleItems = visibleItems;

            /* Calculate Width Depending on Largest Value */
            if (AutoWidth)
            {
                UpdateSizeBasedOnVisible();
            }
            else
            {
                Host.CalcSize();
            }

            return selectedIndex;
        }

        private void UpdateSizeBasedOnVisible()
        {
            int mxWidth = 0;
            for (int i = 0; i < VisibleItems.Count; i++)
            {
                int width = TextRenderer
                    .MeasureText(Host.CreateGraphics(), VisibleItems[i].ToString(), Font)
                    .Width;
                if (width > mxWidth)
                    mxWidth = width;
            }

            mxWidth += LeftPadding;
            Host.CalcSize(Math.Min(MaximumSize.Width, mxWidth));
        }

        private Range GetFragment()
        {
            var tb = TargetControlWrapper;
            string text = tb.Text;
            var result = new Range(tb);
            if (string.IsNullOrEmpty(text))
            {
                return result;
            }

            result.End = tb.SelectionStart;
            for (int i = tb.SelectionStart; i <= text.Length; i++)
            {
                result.End = i;
                if (i >= text.Length)
                {
                    break;
                }
                var c = text[i];
                if (!char.IsLetterOrDigit(c))
                {
                    break;
                }
            }
            result.Start = 0;
            for (int i = result.End - 1; i >= 0; i--)
            {
                result.Start = i;
                if (text[i] == '#')
                    break;
            }
            return result;
        }

        public void Close()
        {
            Host.ListView.HideToolTip(Host.ListView.GetParentControl());
            Host.Close();
            forcedOpened = false;
        }

        public void SetAutocompleteItems(IEnumerable<string> items)
        {
            var list = new List<AutocompleteItem>();
            if (items == null)
            {
                sourceItems = null;
                return;
            }
            foreach (string item in items)
                list.Add(new AutocompleteItem(item));
            SetAutocompleteItems(list);
        }

        public void SetAutocompleteItems(IEnumerable<AutocompleteItem> items)
        {
            sourceItems = items;
        }

        public void AddItem(string item)
        {
            AddItem(new AutocompleteItem(item));
        }

        public void AddItem(AutocompleteItem item)
        {
            if (sourceItems == null)
                sourceItems = new List<AutocompleteItem>();

            if (sourceItems is IList)
                (sourceItems as IList).Add(item);
            else
                throw new Exception("Current autocomplete items does not support adding");
        }

        internal virtual void OnSelecting()
        {
            if (SelectedItemIndex < 0 || SelectedItemIndex >= VisibleItems.Count)
                return;

            AutocompleteItem item = VisibleItems[SelectedItemIndex];
            var args = new SelectingEventArgs { Item = item, SelectedIndex = SelectedItemIndex };

            if (args.Cancel)
            {
                SelectedItemIndex = args.SelectedIndex;
                (Host.ListView as Control).Invalidate(true);
                return;
            }

            if (!args.Handled)
            {
                Range fragment = Fragment;
                ApplyAutocomplete(item, fragment);
            }

            Close();
        }

        private void ApplyAutocomplete(AutocompleteItem item, Range fragment)
        {
            string newText = item.GetTextForReplace();
            //replace text of fragment
            fragment.Text = newText;
        }

        public void SetColumns(string[] columns, int[] columnsWidth = null)
        {
            ListView.ColumnsTitle = columns;
            ListView.ColumnsWidth = columnsWidth;
        }

        public void SelectNext(int shift)
        {
            SelectedItemIndex = Math.Max(
                0,
                Math.Min(SelectedItemIndex + shift, VisibleItems.Count - 1)
            );
            //
            (Host.ListView as Control).Invalidate();
        }

        public bool ProcessKey(char c, Keys keyModifiers)
        {
            var page = Host.Height / (Font.Height + 4);
            if (keyModifiers == Keys.None)
                switch ((Keys)c)
                {
                    case Keys.Down:
                        SelectNext(+1);
                        return true;
                    case Keys.PageDown:
                        SelectNext(+page);
                        return true;
                    case Keys.Up:
                        SelectNext(-1);
                        return true;
                    case Keys.PageUp:
                        SelectNext(-page);
                        return true;
                    case Keys.Enter:
                        if (SelectedItemIndex >= 0 && SelectedItemIndex < VisibleItems.Count)
                        {
                            OnSelecting();
                        }
                        else
                        {
                            Close();
                            TargetControlWrapper?.InvokeKeyDownCallback(Keys.Enter);
                        }
                        return true;
                    case Keys.Tab:
                        if (!AllowsTabKey)
                            break;
                        OnSelecting();
                        return true;
                    case Keys.Left:
                    case Keys.Right:
                        Close();
                        return false;
                    case Keys.Escape:
                        Close();
                        return true;
                }

            return false;
        }

        /// <summary>
        /// Menu is visible
        /// </summary>
        public bool Visible
        {
            get { return Host != null && Host.Visible; }
        }
    }
}
