using ScintillaNET;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using VgcApis.Resources.Langs;

namespace VgcApis.Misc
{
    public static class UI
    {
        #region Controls
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

        public static void ResetComboBoxDropdownMenuWidth(ComboBox cbox)
        {
            int maxWidth = 0, tempWidth = 0;
            var font = cbox.Font;

            foreach (var item in cbox.Items)
            {
                tempWidth = TextRenderer.MeasureText(item.ToString(), font).Width;
                if (tempWidth > maxWidth)
                {
                    maxWidth = tempWidth;
                }
            }
            cbox.DropDownWidth = Math.Max(cbox.Width, maxWidth + SystemInformation.VerticalScrollBarWidth);
        }

        public static void AddContextMenu(RichTextBox rtb)
        {
            // https://stackoverflow.com/questions/18966407/enable-copy-cut-past-window-in-a-rich-text-box/44064791

            if (rtb.ContextMenuStrip != null)
            {
                return;
            }

            ContextMenuStrip cms = new ContextMenuStrip()
            {
                ShowImageMargin = false
            };

            var tsmiUndo = new ToolStripMenuItem(
                I18N.Undo, null, (sender, e) => rtb.Undo());

            var tsmiRedo = new ToolStripMenuItem(
                I18N.Redo, null, (sender, e) => rtb.Redo());

            var tsmiCut = new ToolStripMenuItem(
                I18N.Cut, null, (sender, e) => rtb.Cut());

            var tsmiCopy = new ToolStripMenuItem(
                I18N.Copy, null, (sender, e) => rtb.Copy());

            var tsmiPaste = new ToolStripMenuItem(
                I18N.Paste, null, (sender, e) => rtb.Paste());

            var tsmiDelete = new ToolStripMenuItem(
                I18N.Delete, null, (sender, e) => rtb.SelectedText = "");

            var tsmiSelectAll = new ToolStripMenuItem(
                I18N.SelectAll, null, (sender, e) => rtb.SelectAll());

            ToolStripSeparator nsp() => new ToolStripSeparator();
            cms.Items.AddRange(new ToolStripItem[] {
                tsmiSelectAll,
                nsp(),
                tsmiCut,
                tsmiCopy,
                tsmiPaste,
                nsp(),
                tsmiUndo,
                tsmiRedo,
                tsmiDelete,
            });

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
        public static void CloseFormIgnoreError(Form form) =>
            Invoke(() => form?.Close());

        public static bool IsInUiThread()
        {
            return Thread.CurrentThread.Name == Models.Consts.Libs.UiThreadName;
        }

        public static Action<Action> Invoke;

        public static Action<Action, Action> InvokeThen;

        // https://stackoverflow.com/questions/87795/how-to-prevent-flickering-in-listview-when-updating-a-single-listviewitems-text
        public static void DoubleBuffered(this Control control, bool enable)
        {
            var doubleBufferPropertyInfo = control.GetType().GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            doubleBufferPropertyInfo.SetValue(control, enable, null);
        }

        public static void ScrollToBottom(RichTextBox control)
        {
            control.SelectionStart = control.Text.Length;
            control.ScrollToCaret();
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
            var err = ShowSaveFileDialog(
                    extentions,
                    content,
                    out string filename);

            switch (err)
            {
                case Models.Datas.Enums.SaveFileErrorCode.Success:
                    MessageBox.Show(I18N.Done);
                    break;
                case Models.Datas.Enums.SaveFileErrorCode.Fail:
                    MessageBox.Show(I18N.WriteFileFail);
                    break;
                case Models.Datas.Enums.SaveFileErrorCode.Cancel:
                    // do nothing
                    break;
            }
            return filename;
        }

        public static Models.Datas.Enums.SaveFileErrorCode ShowSaveFileDialog(
            string extension, string content, out string fileName)
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
            string extension, string content, out string fileName)
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = extension,
                RestoreDirectory = true,
                Title = I18N.SaveAs,
                ShowHelp = true,
            })
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
            using (OpenFileDialog readFileDialog = new OpenFileDialog
            {
                InitialDirectory = "c:\\",
                Filter = extension,
                RestoreDirectory = true,
                CheckFileExists = true,
                CheckPathExists = true,
                ShowHelp = true,
            })
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
                Utils.RunInBackground(() => System.Diagnostics.Process.Start(url));
            }
        }

        public static void MsgBox(string content) =>
            MsgBox("", content);

        public static void MsgBox(string title, string content) =>
            MessageBox.Show(content ?? string.Empty, title ?? string.Empty);

        public static void MsgBoxAsync(string content) =>
            Utils.RunInBackground(() => MsgBox("", content));

        public static void MsgBoxAsync(string title, string content) =>
            Utils.RunInBackground(() => MsgBox(title, content));

        public static bool Confirm(string content)
        {
            var confirm = MessageBox.Show(
                content,
                I18N.Confirm,
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button2);

            return confirm == DialogResult.Yes;
        }

        #endregion

        #region winform

        static List<Color> colorTable = new List<Color> {
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

        static List<Color> ColorStructToList(IEnumerable<string> filterList)
        {
            var colors = typeof(Color).GetProperties(BindingFlags.Static | BindingFlags.DeclaredOnly | BindingFlags.Public)
                .Select(c => (Color)c.GetValue(null, null))
                .Where(c => !filterList.Where(fl => c.Name.Contains(fl)).Any())
                .ToList();
            return colors;
        }

        public static Color String2Color(string text)
        {
            MD5 md5Hasher = MD5.Create();
            var hashed = md5Hasher.ComputeHash(Encoding.UTF8.GetBytes(text));
            var v = BitConverter.ToInt32(hashed, 0);
            var maxIdx = colorTable.Count();
            var idx = ((v % maxIdx) + maxIdx) % maxIdx;
            return colorTable[idx];
        }

        public static List<ToolStripMenuItem> AutoGroupMenuItems(
            List<ToolStripMenuItem> menuItems, int groupSize)
        {
            var mi = menuItems;
            var menuSpan = groupSize;
            while (mi.Count() > groupSize)
            {
                mi = AutoGroupMenuItemsWorker(mi, groupSize, menuSpan);
                menuSpan *= groupSize;
            }
            return mi;
        }

        static List<ToolStripMenuItem> AutoGroupMenuItemsWorker(
            IEnumerable<ToolStripMenuItem> menuItems, int groupSize, int menuSpan)
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
                var text = string.Format("{0,4} - {1,4}", pageIdx + 1, pageIdx + menuSpan);
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

        public static System.Drawing.Icon GetAppIcon()
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
