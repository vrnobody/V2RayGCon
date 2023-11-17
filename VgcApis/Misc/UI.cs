using ScintillaNET;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using VgcApis.Resources.Langs;

namespace VgcApis.Misc
{
    public static class UI
    {
        #region Controls
        public static List<T> DoHouseKeeping<T>(
            FlowLayoutPanel flyPanel,
            int num,
            bool isOneColumn = false
        )
            where T : Control, new()
        {
            var removed = new List<T>();
            if (flyPanel == null || flyPanel.IsDisposed)
            {
                return removed;
            }

            var ctrls = flyPanel.Controls.OfType<T>().ToList();
            var numAdd = num - ctrls.Count;
            for (int i = 0; i < numAdd; i++)
            {
                var ctrl = new T();
                flyPanel.Controls.Add(ctrl);
                if (isOneColumn)
                {
                    flyPanel.SetFlowBreak(ctrl, true);
                }
            }

            var numRemove = ctrls.Count - num;
            ctrls.Reverse();
            for (int i = 0; i < numRemove; i++)
            {
                var c = ctrls[i];
                removed.Add(c);
                flyPanel.Controls.Remove(c);
            }
            return removed;
        }

        public static void SetSearchIndicator(Scintilla scintilla)
        {
            // indicator for search
            const int INDICATOR_NUM = 8;

            // Remove all uses of our indicator
            scintilla.IndicatorCurrent = INDICATOR_NUM;

            // Update indicator appearance
            scintilla.Indicators[INDICATOR_NUM].Style = IndicatorStyle.StraightBox;
            scintilla.Indicators[INDICATOR_NUM].Under = true;
            scintilla.Indicators[INDICATOR_NUM].ForeColor = Color.Yellow;
            scintilla.Indicators[INDICATOR_NUM].OutlineAlpha = 220;
            scintilla.Indicators[INDICATOR_NUM].Alpha = 180;
        }

        public static void SelectComboxByText(ComboBox cbox, string text)
        {
            if (!string.IsNullOrEmpty(text))
            {
                var items = cbox.Items;
                for (int i = 0; i < items.Count; i++)
                {
                    var item = items[i];
                    if (item.ToString() == text)
                    {
                        cbox.SelectedIndex = i;
                        return;
                    }
                }
            }

            cbox.SelectedIndex = cbox.Items.Count > 0 ? 0 : -1;
            return;
        }

        public static void ResetComboBoxDropdownMenuWidth(ToolStripComboBox cbox)
        {
            int maxWidth = 0;
            int tempWidth;
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
            int maxWidth = 0;
            int tempWidth;
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

        public static void AddContextMenu(RichTextBox rtb)
        {
            // https://stackoverflow.com/questions/18966407/enable-copy-cut-past-window-in-a-rich-text-box/44064791

            if (rtb.ContextMenuStrip != null)
            {
                return;
            }

            ContextMenuStrip cms = new ContextMenuStrip() { ShowImageMargin = false };

            var tsmiUndo = new ToolStripMenuItem(I18N.Undo, null, (sender, e) => rtb.Undo());

            var tsmiRedo = new ToolStripMenuItem(I18N.Redo, null, (sender, e) => rtb.Redo());

            var tsmiCut = new ToolStripMenuItem(I18N.Cut, null, (sender, e) => rtb.Cut());

            var tsmiCopy = new ToolStripMenuItem(I18N.Copy, null, (sender, e) => rtb.Copy());

            var tsmiPaste = new ToolStripMenuItem(I18N.Paste, null, (sender, e) => rtb.Paste());

            var tsmiDelete = new ToolStripMenuItem(
                I18N.Delete,
                null,
                (sender, e) => rtb.SelectedText = ""
            );

            var tsmiSelectAll = new ToolStripMenuItem(
                I18N.SelectAll,
                null,
                (sender, e) => rtb.SelectAll()
            );

            ToolStripSeparator nsp() => new ToolStripSeparator();
            cms.Items.AddRange(
                new ToolStripItem[]
                {
                    tsmiSelectAll,
                    nsp(),
                    tsmiCut,
                    tsmiCopy,
                    tsmiPaste,
                    nsp(),
                    tsmiUndo,
                    tsmiRedo,
                    tsmiDelete,
                }
            );

            bool isClipboardHasText()
            {
                var r = Clipboard.ContainsText();
                return r;
            }

            cms.Opening += (sender, e) =>
            {
                tsmiUndo.Enabled = !rtb.ReadOnly && rtb.CanUndo;
                tsmiRedo.Enabled = !rtb.ReadOnly && rtb.CanRedo;
                tsmiCut.Enabled = !rtb.ReadOnly && rtb.SelectionLength > 0;
                tsmiCopy.Enabled = rtb.SelectionLength > 0;
                tsmiPaste.Enabled = !rtb.ReadOnly && isClipboardHasText();
                tsmiDelete.Enabled = !rtb.ReadOnly && rtb.SelectionLength > 0;
                tsmiSelectAll.Enabled = rtb.TextLength > 0 && rtb.SelectionLength < rtb.TextLength;
            };

            rtb.ContextMenuStrip = cms;
        }

        static void SetControlFontColor(Control control, bool isValid)
        {
            var color = isValid ? Color.Black : Color.Red;
            if (control.ForeColor != color)
            {
                control.ForeColor = color;
            }
        }

        public static void MarkInvalidGuidWithColorRed(Control control)
        {
            var isValid = false;
            try
            {
                var _ = new Guid(control.Text);
                isValid = true;
            }
            catch { }
            SetControlFontColor(control, isValid);
        }

        public static void MarkInvalidPortWithColorRed(Control control)
        {
            var port = Utils.Str2Int(control.Text);
            SetControlFontColor(control, port > 0 && port < 65536);
        }

        public static void MarkInvalidAddressWithColorRed(Control control)
        {
            var isValid = Utils.TryParseAddress(control.Text, out _, out _);
            SetControlFontColor(control, isValid);
        }
        #endregion

        #region update ui
        public static void CloseFormIgnoreError(Form form) => Invoke(() => form?.Close());

        public static bool IsInUiThread()
        {
            return Thread.CurrentThread.Name == Models.Consts.Libs.UiThreadName;
        }

        public static Action<Action> Invoke;

        public static Action<Action, Action> InvokeThen;

        // https://stackoverflow.com/questions/87795/how-to-prevent-flickering-in-listview-when-updating-a-single-listviewitems-text
        public static void DoubleBuffered(this Control control, bool enable)
        {
            var doubleBufferPropertyInfo = control
                .GetType()
                .GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            doubleBufferPropertyInfo.SetValue(control, enable, null);
        }

        public static void UpdateRichTextBox(RichTextBox box, string content)
        {
            Invoke?.Invoke(() =>
            {
                if (box == null || box.IsDisposed)
                {
                    return;
                }

                DisableRedraw(box);
                box.Text = content;
                box.SelectionStart = box.Text.Length;
                ScrollToBottom(box);
                EnableRedraw(box);
            });
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(
            IntPtr hWnd,
            int wMsg,
            IntPtr wParam,
            IntPtr lParam
        );

        const int WM_VSCROLL = 277;
        const int SB_PAGEBOTTOM = 7;
        const int WM_SETREDRAW = 0x0b;

        public static void ScrollToBottom(RichTextBox box)
        {
            SendMessage(box.Handle, WM_VSCROLL, (IntPtr)SB_PAGEBOTTOM, IntPtr.Zero);
        }

        public static void DisableRedraw(Control control)
        {
            if (control == null)
            {
                return;
            }
            // Stop redrawing:
            SendMessage(control.Handle, WM_SETREDRAW, IntPtr.Zero, IntPtr.Zero);
        }

        public static void EnableRedraw(Control control)
        {
            if (control == null)
            {
                return;
            }
            // turn on redrawing
            SendMessage(control.Handle, WM_SETREDRAW, (IntPtr)1, IntPtr.Zero);
            control.Invalidate();
        }

        #endregion

        #region file
        static public string ReadFileContentFromDialog(string extension)
        {
            var tuple = ReadFileFromDialog(extension);
            return tuple.Item1;
        }

        /// <summary>
        /// <para>return(content, filename)</para>
        /// <para>[content] Null: cancelled String.Empty: file is empty or read fail.</para>
        /// </summary>
        /// <param name="extension"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        static public Tuple<string, string> ReadFileFromDialog(string extension)
        {
            Tuple<string, string> r = null;
            Invoke(() =>
            {
                r = ReadFileFromDialogWorker(extension);
            });
            return r;
        }

        static Tuple<string, string> ReadFileFromDialogWorker(string extension)
        {
            OpenFileDialog readFileDialog = new OpenFileDialog
            {
                Filter = extension,
                RestoreDirectory = true,
                CheckFileExists = true,
                CheckPathExists = true,
                ShowHelp = true,
            };

            var fileName = string.Empty;

            if (readFileDialog.ShowDialog() != DialogResult.OK)
            {
                return new Tuple<string, string>(null, fileName);
            }

            fileName = readFileDialog.FileName;
            var content = string.Empty;
            try
            {
                content = File.ReadAllText(fileName);
            }
            catch { }
            return new Tuple<string, string>(content, fileName);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="content"></param>
        /// <param name="extentions"></param>
        /// <returns>file name</returns>
        static public string SaveToFile(string extentions, string content)
        {
            var err = ShowSaveFileDialog(extentions, content, out string filename);

            switch (err)
            {
                case Models.Datas.Enums.SaveFileErrorCode.Success:
                    VgcApis.Misc.UI.MsgBox(I18N.Done);
                    break;
                case Models.Datas.Enums.SaveFileErrorCode.Fail:
                    VgcApis.Misc.UI.MsgBox(I18N.WriteFileFail);
                    break;
                case Models.Datas.Enums.SaveFileErrorCode.Cancel:
                    // do nothing
                    break;
            }
            return filename;
        }

        public static Models.Datas.Enums.SaveFileErrorCode ShowSaveFileDialog(
            string extension,
            string content,
            out string fileName
        )
        {
            Models.Datas.Enums.SaveFileErrorCode r = Models.Datas.Enums.SaveFileErrorCode.Cancel;
            string fn = null;
            Invoke(() =>
            {
                r = ShowSaveFileDialogWorker(extension, content, out fn);
            });
            fileName = fn;
            return r;
        }

        static Models.Datas.Enums.SaveFileErrorCode ShowSaveFileDialogWorker(
            string extension,
            string content,
            out string fileName
        )
        {
            using (
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = extension,
                    RestoreDirectory = true,
                    Title = I18N.SaveAs,
                    ShowHelp = true,
                }
            )
            {
                saveFileDialog.ShowDialog();

                fileName = saveFileDialog.FileName;
                if (string.IsNullOrEmpty(fileName))
                {
                    return Models.Datas.Enums.SaveFileErrorCode.Cancel;
                }

                try
                {
                    File.WriteAllText(fileName, content);
                    return Models.Datas.Enums.SaveFileErrorCode.Success;
                }
                catch { }
                return Models.Datas.Enums.SaveFileErrorCode.Fail;
            }
        }

        public static string ShowSelectFolderDialog()
        {
            string r = null;
            Invoke(() =>
            {
                r = ShowSelectFolderDialogWorker();
            });
            return r;
        }

        static string ShowSelectFolderDialogWorker()
        {
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    return fbd.SelectedPath;
                }
            }

            return null;
        }

        /// <summary>
        /// Return file name.
        /// </summary>
        /// <param name="extension"></param>
        /// <returns></returns>
        public static string ShowSelectFileDialog(string extension)
        {
            string r = null;
            Invoke(() =>
            {
                r = ShowSelectFileDialogWorker(extension);
            });
            return r;
        }

        static string ShowSelectFileDialogWorker(string extension)
        {
            using (
                OpenFileDialog readFileDialog = new OpenFileDialog
                {
                    InitialDirectory = "c:\\",
                    Filter = extension,
                    RestoreDirectory = true,
                    CheckFileExists = true,
                    CheckPathExists = true,
                    ShowHelp = true,
                }
            )
            {
                if (readFileDialog.ShowDialog() != DialogResult.OK)
                {
                    return null;
                }

                return readFileDialog.FileName;
            }
        }

        #endregion

        #region popup
        public static void VisitUrl(string msg, string url)
        {
            var text = string.Format("{0}\n{1}", msg, url);
            if (Confirm(text))
            {
                Utils.RunInBgSlim(() => System.Diagnostics.Process.Start(url));
            }
        }

        public static void MsgBox(string content)
        {
            MsgBox(Properties.Resources.AppName, content);
        }

        public static void MsgBox(string title, string content)
        {
            Invoke(
                () =>
                    MessageBox.Show(
                        content ?? string.Empty,
                        title ?? string.Empty,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.None,
                        MessageBoxDefaultButton.Button1
                    )
            );
        }

        public static void MsgBoxAsync(string content)
        {
            Utils.RunInBgSlim(() => MsgBox(content));
        }

        public static void MsgBoxAsync(string title, string content)
        {
            Utils.RunInBgSlim(() => MsgBox(title, content));
        }

        public static bool Confirm(string content)
        {
            DialogResult ok = DialogResult.No;
            Invoke(() =>
            {
                ok = MessageBox.Show(
                    content,
                    $"{Properties.Resources.AppName} - {I18N.Confirm}",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button2
                );
            });
            return ok == DialogResult.Yes;
        }

        #endregion

        #region winform
        public static void GetUserInput(string title, Action<string> onOk, Action onCancel = null)
        {
            var form = new WinForms.FormInput(title);
            form.Show();
            form.FormClosed += (_, __) =>
            {
                if (form.DialogResult == DialogResult.OK)
                {
                    onOk?.Invoke(form.Content);
                }
                else
                {
                    onCancel?.Invoke();
                }
            };
        }

        static readonly List<Color> colorTable = new List<Color>
        {
            Color.AntiqueWhite,
            Color.Aqua,
            Color.Aquamarine,
            Color.Bisque,
            Color.BlueViolet,
            Color.Brown,
            Color.BurlyWood,
            Color.CadetBlue,
            Color.Chartreuse,
            Color.Chocolate,
            Color.Coral,
            Color.CornflowerBlue,
            Color.Crimson,
            Color.Cyan,
            Color.DarkCyan,
            Color.DarkGoldenrod,
            Color.DarkGray,
            Color.DarkGreen,
            Color.DarkKhaki,
            Color.DarkMagenta,
            Color.DarkOrange,
            Color.DarkOrchid,
            Color.DarkSalmon,
            Color.DarkSeaGreen,
            Color.DarkSlateBlue,
            Color.DarkSlateGray,
            Color.DarkTurquoise,
            Color.DarkViolet,
            Color.DeepPink,
            Color.DeepSkyBlue,
            Color.DodgerBlue,
            Color.Firebrick,
            Color.ForestGreen,
            Color.Fuchsia,
            Color.Gold,
            Color.Goldenrod,
            Color.Gray,
            Color.GreenYellow,
            Color.HotPink,
            Color.IndianRed,
            Color.Khaki,
            Color.LawnGreen,
            Color.LightBlue,
            Color.LightCoral,
            Color.LightGray,
            Color.LightGreen,
            Color.LightPink,
            Color.LightSalmon,
            Color.LightSeaGreen,
            Color.LightSkyBlue,
            Color.LightSlateGray,
            Color.LightSteelBlue,
            Color.Lime,
            Color.LimeGreen,
            Color.Magenta,
            Color.MediumAquamarine,
            Color.MediumOrchid,
            Color.MediumPurple,
            Color.MediumSeaGreen,
            Color.MediumSlateBlue,
            Color.MediumSpringGreen,
            Color.MediumTurquoise,
            Color.MediumVioletRed,
            Color.Moccasin,
            Color.Olive,
            Color.OliveDrab,
            Color.Orange,
            Color.OrangeRed,
            Color.Orchid,
            Color.PaleGreen,
            Color.PaleTurquoise,
            Color.PaleVioletRed,
            Color.PeachPuff,
            Color.Peru,
            Color.Pink,
            Color.Plum,
            Color.PowderBlue,
            Color.RosyBrown,
            Color.RoyalBlue,
            Color.Salmon,
            Color.SandyBrown,
            Color.SeaGreen,
            Color.Sienna,
            Color.SkyBlue,
            Color.SlateBlue,
            Color.SpringGreen,
            Color.SteelBlue,
            Color.Tan,
            Color.Teal,
            Color.Thistle,
            Color.Tomato,
            Color.Turquoise,
            Color.Violet,
            Color.Wheat,
            Color.YellowGreen,
            Color.Yellow,
        };

        public static Color String2Color(string text)
        {
            var hashed = Utils.Md5Hash(text);
            var v = BitConverter.ToInt32(hashed, 0);
            var maxIdx = colorTable.Count();
            var idx = ((v % maxIdx) + maxIdx) % maxIdx;
            return colorTable[idx];
        }

        public static List<ToolStripMenuItem> AutoGroupMenuItems(
            List<ToolStripMenuItem> menuItems,
            int groupSize
        )
        {
            var mi = menuItems;
            var menuSpan = groupSize;
            var max = menuItems.Count;

            while (mi.Count() > groupSize)
            {
                mi = AutoGroupMenuItemsWorker(mi, max, groupSize, menuSpan);
                menuSpan *= groupSize;
            }
            return mi;
        }

        static List<ToolStripMenuItem> AutoGroupMenuItemsWorker(
            IEnumerable<ToolStripMenuItem> menuItems,
            int maxIdx,
            int groupSize,
            int menuSpan
        )
        {
            var count = menuItems.Count();
            if (count <= groupSize)
            {
                return menuItems.ToList();
            }

            // grouping
            var groups = new List<ToolStripMenuItem>();
            var servIdx = 0;
            var pageIdx = 0;
            while (servIdx < count)
            {
                var take = Math.Min(groupSize, count - servIdx);
                var last = Math.Min(pageIdx + menuSpan, maxIdx);
                var text = string.Format("{0,5} - {1,5}", pageIdx + 1, last);
                var mis = menuItems.Skip(servIdx).Take(take).ToArray();
                var mi = new ToolStripMenuItem(text, null, mis);
                groups.Add(mi);
                servIdx += groupSize;
                pageIdx += menuSpan;
            }
            return groups;
        }

        public static void AutoSetFormIcon(Form form)
        {
#if DEBUG
            form.Icon = Properties.Resources.icon_light;
#else
            form.Icon = Properties.Resources.icon_dark;
#endif
        }

        static Bitmap tunIconCache = null;

        public static Bitmap GetTunModeIconCache()
        {
            if (tunIconCache == null)
            {
                tunIconCache = Properties.Resources.icon_tun_mode.ToBitmap();
            }
            return tunIconCache;
        }

        public static Icon GetAppIcon()
        {
#if DEBUG
            return Properties.Resources.icon_light;
#else
            return Properties.Resources.icon_dark;
#endif
        }
        #endregion
    }
}
