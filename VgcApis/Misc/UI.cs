using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using VgcApis.Resources.Langs;

namespace VgcApis.Misc
{
    public static class UI
    {

        #region Controls
        public static bool TryParseControlTextToIpAndPort(Control control, out string ip, out int port)
        {
            var success = Utils.TryParseIPAddr(control.Text, out ip, out port);
            var color = success ? Color.Black : Color.Red;

            // UI operation is expansive
            if (control.ForeColor != color)
            {
                control.ForeColor = color;
            }

            return success;
        }
        #endregion

        #region update ui
        public static void CloseFormIgnoreError(Form form)
        {
            try
            {
                form?.Invoke((MethodInvoker)delegate
                {
                    try
                    {
                        form?.Close();
                    }
                    catch { }
                });
            }
            catch { }
        }

        /// <summary>
        /// If control==null return;
        /// </summary>
        /// <param name="control">invokeable control</param>
        /// <param name="updateUi">UI updater</param>
        public static void RunInUiThread(Control control, Action updateUi)
        {
            if (control == null || control.IsDisposed)
            {
                return;
            }

            if (control.InvokeRequired)
            {
                control.Invoke((MethodInvoker)delegate
                {
                    updateUi();
                });
            }
            else
            {
                updateUi();
            }
        }

        public static void RunInUiThreadIgnoreError(Control control, Action updateUi)
        {
            if (control == null || control.IsDisposed)
            {
                return;
            }

            if (control.InvokeRequired)
            {
                try
                {
                    control.Invoke((MethodInvoker)delegate
                    {
                        try
                        {
                            updateUi();
                        }
                        catch { }
                    });
                }
                catch { }
            }
            else
            {
                try
                {
                    updateUi();
                }
                catch { }
            }
        }

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
            OpenFileDialog readFileDialog = new OpenFileDialog
            {
                InitialDirectory = "c:\\",
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
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                InitialDirectory = "c:\\",
                Filter = extension,
                RestoreDirectory = true,
                Title = I18N.SaveAs,
                ShowHelp = true,
            };

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

        /// <summary>
        /// Return file name.
        /// </summary>
        /// <param name="extension"></param>
        /// <returns></returns>
        public static string ShowSelectFileDialog(string extension)
        {
            OpenFileDialog readFileDialog = new OpenFileDialog
            {
                InitialDirectory = "c:\\",
                Filter = extension,
                RestoreDirectory = true,
                CheckFileExists = true,
                CheckPathExists = true,
                ShowHelp = true,
            };

            var fileName = string.Empty;

            if (readFileDialog.ShowDialog() != DialogResult.OK)
            {
                return null;
            }

            return readFileDialog.FileName;
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
            IEnumerable<ToolStripMenuItem> menuItems, int groupSize)
        {
            var count = menuItems.Count();
            if (count <= groupSize)
            {
                return menuItems.ToList();
            }

            // grouping
            var groups = new List<ToolStripMenuItem>();
            var index = 0;
            while (index < count)
            {
                var take = Math.Min(groupSize, count - index);
                groups.Add(new ToolStripMenuItem(
                     string.Format("{0,4} - {1,4}", index + 1, index + groupSize),
                     null,
                     menuItems.Skip(index).Take(take).ToArray()));
                index += groupSize;
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
