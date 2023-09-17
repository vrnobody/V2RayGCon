using ScintillaNET;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using V2RayGCon.Resources.Resx;

namespace V2RayGCon.Misc
{
    public static class UI
    {
        #region winform

        public static IEnumerable<TResult> GetAllControls<TResult>(Control control)
            where TResult : Control
        {
            var controls = control.Controls.Cast<Control>();

            return controls
                .SelectMany(ctrl => GetAllControls<TResult>(ctrl))
                .Concat(controls)
                .Where(c => c is TResult)
                .Select(c => c as TResult);
        }

        static double GetScalingFactorFromGraphic(Graphics graphic, int step)
        {
            var scale = Math.Max(graphic.DpiX, graphic.DpiY) / 96.0;
            return ((int)Math.Floor(scale * 100) / step * step) / 100.0;
        }

        public static double GetScreenScalingFactor()
        {
            // https://stackoverflow.com/questions/14385838/draw-on-screen-without-form
            IntPtr desktopPtr = Libs.Sys.SafeNativeMethods.GetDC(IntPtr.Zero);
            Graphics g = Graphics.FromHdc(desktopPtr);
            var result = GetScalingFactorFromGraphic(g, 25);
            g.Dispose();
            Libs.Sys.SafeNativeMethods.ReleaseDC(IntPtr.Zero, desktopPtr);
            return result;
        }

        public static double GetFormScalingFactor(Form form)
        {
            // https://www.medo64.com/2014/01/scaling-toolstrip-with-dpi/
            using (Graphics g = form.CreateGraphics())
            {
                return GetScalingFactorFromGraphic(g, 25);
            }
        }

        // if baseLine is set, use baseLine instead of orginal image scaling size
        public static void AutoScaleToolStripControls(Form form, int baseLine = -1)
        {
            // https://www.medo64.com/2014/01/scaling-toolstrip-with-dpi/
            var factor = GetFormScalingFactor(form);
            if (factor < 1)
            {
                factor = 1;
            }

            var menuList = GetAllControls<ToolStrip>(form);
            foreach (var menu in menuList)
            {
                menu.ImageScalingSize = new Size(
                    (int)((baseLine < 0 ? menu.ImageScalingSize.Width : baseLine) * factor),
                    (int)((baseLine < 0 ? menu.ImageScalingSize.Height : baseLine) * factor)
                );

                foreach (var cbox in GetAllControls<ComboBox>(menu))
                {
                    cbox.Width = (int)(cbox.Width * factor);
                }
            }
        }

        public static void ShowMessageBoxDoneAsync()
        {
            VgcApis.Misc.Utils.RunInBackground(() => MessageBox.Show(I18N.Done));
        }

        public static bool UpdateControlOnDemand(Control control, int value)
        {
            switch (control)
            {
                case ComboBox cbox:
                    if (cbox.SelectedIndex == value)
                    {
                        return false;
                    }
                    cbox.SelectedIndex = value;
                    return true;
            }

            throw new ArgumentException("Unsupported control type");
        }

        public static bool UpdateControlOnDemand(ToolStripMenuItem menuItem, bool value)
        {
            if (menuItem.Checked == value)
            {
                return false;
            }

            menuItem.Checked = value;
            return true;
        }

        public static bool UpdateControlOnDemand(Control control, bool value)
        {
            switch (control)
            {
                case CheckBox chk:
                    if (chk.Checked == value)
                    {
                        return false;
                    }
                    chk.Checked = value;
                    return true;
            }
            throw new ArgumentException("Unsupported control type");
        }

        public static bool UpdateControlOnDemand(Control control, string value)
        {
            if (
                !(control is TextBox)
                && !(control is Label)
                && !(control is RichTextBox)
                && !(control is ComboBox)
            )
            {
                throw new ArgumentException("Unsupported control type");
            }

            if (control.Text == value)
            {
                return false;
            }

            control.Text = value;
            return true;
        }

        public static void ResetComboBoxDropdownMenuWidth(ToolStripComboBox cbox)
        {
            int maxWidth = 0,
                tempWidth = 0;
            var font = cbox.Font;

            foreach (var item in cbox.Items)
            {
                tempWidth = TextRenderer.MeasureText(item.ToString(), font).Width;
                if (tempWidth > maxWidth)
                {
                    maxWidth = tempWidth;
                }
            }
            cbox.DropDownWidth = Math.Max(
                cbox.Width,
                maxWidth + SystemInformation.VerticalScrollBarWidth
            );
        }

        public static void ResetComboBoxDropdownMenuWidth(ComboBox cbox)
        {
            int maxWidth = 0,
                tempWidth = 0;
            var font = cbox.Font;

            foreach (var item in cbox.Items)
            {
                tempWidth = TextRenderer.MeasureText(item.ToString(), font).Width;
                if (tempWidth > maxWidth)
                {
                    maxWidth = tempWidth;
                }
            }
            cbox.DropDownWidth = Math.Max(
                cbox.Width,
                maxWidth + SystemInformation.VerticalScrollBarWidth
            );
        }

        public static void ClearFlowLayoutPanel(FlowLayoutPanel panel)
        {
            List<Control> listControls = new List<Control>();

            foreach (Control control in panel.Controls)
            {
                listControls.Add(control);
            }

            foreach (Control control in listControls)
            {
                panel.Controls.Remove(control);
                control.Dispose();
            }
        }

        public static Cursor CreateCursorIconFromUserControl(Control control)
        {
            Bitmap bmp = new Bitmap(control.Size.Width, control.Size.Height);
            control.DrawToBitmap(bmp, new Rectangle(Point.Empty, bmp.Size));
            return new Cursor(bmp.GetHicon());
        }

        public static Scintilla CreateScintilla(Panel container, bool readOnlyMode = false)
        {
            var scintilla = new Scintilla();

            container.Controls.Add(scintilla);

            // scintilla.Dock = DockStyle.Fill;
            scintilla.Dock = DockStyle.Fill;
            scintilla.WrapMode = WrapMode.None;
            scintilla.IndentationGuides = IndentView.LookBoth;

            // Configure the JSON lexer styles
            scintilla.StyleResetDefault();
            scintilla.Styles[Style.Default].Font = "Consolas";
            scintilla.Styles[Style.Default].Size = 11;
            scintilla.StyleClearAll();

            if (readOnlyMode)
            {
                var bgColor = Color.FromArgb(240, 240, 240);
                scintilla.Styles[Style.Default].BackColor = bgColor;
                scintilla.Styles[Style.Json.BlockComment].BackColor = bgColor;
                scintilla.Styles[Style.Json.Default].BackColor = bgColor;
                scintilla.Styles[Style.Json.Error].BackColor = bgColor;
                scintilla.Styles[Style.Json.EscapeSequence].BackColor = bgColor;
                scintilla.Styles[Style.Json.Keyword].BackColor = bgColor;
                scintilla.Styles[Style.Json.LineComment].BackColor = bgColor;
                scintilla.Styles[Style.Json.Number].BackColor = bgColor;
                scintilla.Styles[Style.Json.Operator].BackColor = bgColor;
                scintilla.Styles[Style.Json.PropertyName].BackColor = bgColor;
                scintilla.Styles[Style.Json.String].BackColor = bgColor;
                scintilla.Styles[Style.Json.CompactIRI].BackColor = bgColor;
                scintilla.ReadOnly = true;
            }

            scintilla.Styles[Style.Json.Default].ForeColor = Color.Silver;
            scintilla.Styles[Style.Json.BlockComment].ForeColor = Color.FromArgb(0, 128, 0); // Green
            scintilla.Styles[Style.Json.LineComment].ForeColor = Color.FromArgb(0, 128, 0); // Green
            scintilla.Styles[Style.Json.Number].ForeColor = Color.Olive;
            scintilla.Styles[Style.Json.PropertyName].ForeColor = Color.Blue;
            scintilla.Styles[Style.Json.String].ForeColor = Color.FromArgb(163, 21, 21); // Red
            scintilla.Styles[Style.Json.StringEol].BackColor = Color.Pink;
            scintilla.Styles[Style.Json.Operator].ForeColor = Color.Purple;
            scintilla.Lexer = Lexer.Json;

            // folding
            // Instruct the lexer to calculate folding
            scintilla.SetProperty("fold", "1");
            scintilla.SetProperty("fold.compact", "1");

            // Configure a margin to display folding symbols
            scintilla.Margins[2].Type = MarginType.Symbol;
            scintilla.Margins[2].Mask = Marker.MaskFolders;
            scintilla.Margins[2].Sensitive = true;
            scintilla.Margins[2].Width = 20;

            // Set colors for all folding markers
            for (int i = 25; i <= 31; i++)
            {
                scintilla.Markers[i].SetForeColor(SystemColors.ControlLightLight);
                scintilla.Markers[i].SetBackColor(SystemColors.ControlDark);
            }

            // Configure folding markers with respective symbols
            scintilla.Markers[Marker.Folder].Symbol = MarkerSymbol.BoxPlus;
            scintilla.Markers[Marker.FolderOpen].Symbol = MarkerSymbol.BoxMinus;
            scintilla.Markers[Marker.FolderEnd].Symbol = MarkerSymbol.BoxPlusConnected;
            scintilla.Markers[Marker.FolderMidTail].Symbol = MarkerSymbol.TCorner;
            scintilla.Markers[Marker.FolderOpenMid].Symbol = MarkerSymbol.BoxMinusConnected;
            scintilla.Markers[Marker.FolderSub].Symbol = MarkerSymbol.VLine;
            scintilla.Markers[Marker.FolderTail].Symbol = MarkerSymbol.LCorner;

            // Enable automatic folding
            scintilla.AutomaticFold = (
                AutomaticFold.Show | AutomaticFold.Click | AutomaticFold.Change
            );

            VgcApis.Misc.Utils.ClearControlKeys(scintilla, null);

            // Configure a margin to display line number
            if (!readOnlyMode)
            {
                scintilla.Margins[0].Type = MarginType.Number;
                scintilla.Margins[0].Width = 16;
                scintilla.Styles[Style.LineNumber].ForeColor = Color.DarkGray;
            }

            VgcApis.Misc.UI.SetSearchIndicator(scintilla);

            return scintilla;
        }

        public static void FillComboBox(ComboBox cbox, Dictionary<int, string> table)
        {
            cbox.Items.Clear();
            foreach (var item in table)
            {
                cbox.Items.Add(item.Value);
            }
            cbox.SelectedIndex = table.Count > 0 ? 0 : -1;
        }

        public static void FillComboBox(ComboBox element, List<string> itemList)
        {
            element.Items.Clear();

            if (itemList == null || itemList.Count <= 0)
            {
                element.SelectedIndex = -1;
                return;
            }

            foreach (var item in itemList)
            {
                element.Items.Add(item);
            }
            element.SelectedIndex = 0;
        }
        #endregion

        #region popup

        public static bool Confirm(string content)
        {
            var confirm = MessageBox.Show(
                content,
                I18N.Confirm,
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button2
            );

            return confirm == DialogResult.Yes;
        }

        public static void VisitUrl(string msg, string url)
        {
            var text = string.Format("{0}\n{1}", msg, url);
            if (Confirm(text))
            {
                VgcApis.Misc.Utils.RunInBgSlim(() => Process.Start(url));
            }
        }

        #endregion

        #region DEBUG

        [Conditional("DEBUG")]
        public static void SetFormLocation<T>(T form, Models.Datas.Enums.FormLocations location)
            where T : Form
        {
            var width = Screen.PrimaryScreen.WorkingArea.Width;
            var height = Screen.PrimaryScreen.WorkingArea.Height;

            form.StartPosition = FormStartPosition.Manual;
            form.Size = new Size(width / 2, height / 2);
            form.Left = 0;
            form.Top = 0;

            switch (location)
            {
                case Models.Datas.Enums.FormLocations.TopRight:
                    form.Left = width / 2;
                    break;
                case Models.Datas.Enums.FormLocations.BottomLeft:
                    form.Top = height / 2;
                    break;
                case Models.Datas.Enums.FormLocations.BottomRight:
                    form.Top = height / 2;
                    form.Left = width / 2;
                    break;
            }
        }

        #endregion
    }
}
