using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NeoLuna.Misc;
using ScintillaNET;
using V2RayGCon.Controllers.FormTextConfigEditorComponent;
using V2RayGCon.Properties;
using V2RayGCon.Resources.Resx;
using static ScintillaNET.Style;

namespace V2RayGCon.Views.WinForms
{
    public partial class FormToolbox : Form
    {
        #region Sigleton
        static readonly VgcApis.BaseClasses.AuxSiWinForm<FormToolbox> auxSiForm =
            new VgcApis.BaseClasses.AuxSiWinForm<FormToolbox>();

        public static FormToolbox GetForm() => auxSiForm.GetForm();

        public static void ShowForm() => auxSiForm.ShowForm();
        #endregion


        readonly string formTitle;

        public FormToolbox()
        {
            InitializeComponent();

            VgcApis.Misc.UI.AutoSetFormIcon(this);
            VgcApis.Misc.UI.AddTagToFormTitle(this);

            this.formTitle = this.Text;
            LoadExamples();
        }

        #region config examples
        string appRoot = VgcApis.Misc.Utils.GetAppDir();

        void LoadExamples()
        {
            var dirRoot = Path.Combine(appRoot, "3rd", "examples");
            var items = GenMenuForFolder(dirRoot);
            var menuRoot = configToolStripMenuItem.DropDownItems;
            menuRoot.AddRange(items.ToArray());
        }

        List<ToolStripMenuItem> GenMenuForFolder(string root)
        {
            var r = new List<ToolStripMenuItem>();
            var dirs = Directory.GetDirectories(root, "*", SearchOption.TopDirectoryOnly);
            foreach (var dir in dirs)
            {
                var name = new DirectoryInfo(dir).Name;
                var mi = new ToolStripMenuItem(name);
                var subs = GenMenuForFolder(dir);
                if (subs.Count > 0)
                {
                    mi.DropDownItems.AddRange(subs.ToArray());
                    r.Add(mi);
                }
            }

            var allowedExtensions = new[] { ".txt", ".json", ".jsonc", ".md" };
            var files = Directory
                .GetFiles(root, "*", SearchOption.TopDirectoryOnly)
                .Where(file => allowedExtensions.Any(file.ToLower().EndsWith))
                .ToList();

            foreach (var file in files)
            {
                var name = new FileInfo(file).Name;
                var mi = new ToolStripMenuItem(name);

                mi.Click += (s, a) =>
                {
                    try
                    {
                        var content = File.ReadAllText(file);
                        var title =
                            file.Length > appRoot.Length + 1
                                ? file.Substring(appRoot.Length + 1)
                                : file;
                        SetResult($"Config - {title}", content);
                    }
                    catch (Exception ex)
                    {
                        VgcApis.Misc.UI.MsgBox(ex.Message);
                    }
                };
                r.Add(mi);
            }
            return r;
        }

        #endregion

        #region helpers

        void SetTitle(string keyType)
        {
            this.Text = $"{formTitle} - {keyType}";
        }

        void SetResult(string title, string result)
        {
            rtBoxOutput.Text = result;
            SetTitle(title);
        }

        void SetResults(string title, IEnumerable<string> results)
        {
            rtBoxOutput.Text = string.Join(Environment.NewLine, results);
            SetTitle(title);
        }

        void DoFiveTimes(string keyType, Func<string> gen)
        {
            var r = new List<string>();
            for (int i = 0; i < 5; i++)
            {
                r.Add(gen());
            }
            SetResults(keyType, r);
        }

        string GetCoreFullPath()
        {
            var folder = VgcApis.Misc.Utils.GetCoreFolderFullPath();
            var path = Path.Combine(folder, @"xray.exe");
            return path;
        }

        void TryExecCoreCmd(string keyType, string args)
        {
            var exe = GetCoreFullPath();
            if (!File.Exists(exe))
            {
                VgcApis.Misc.UI.MsgBox($"{I18N.ExeNotFound}{Environment.NewLine}{exe}");
                return;
            }
            var result = VgcApis.Misc.Utils.ExecuteAndGetStdOut(exe, args, 10000, null);
            if (string.IsNullOrEmpty(result))
            {
                return;
            }
            rtBoxOutput.Text = result;
            SetTitle(keyType);
        }
        #endregion

        #region key gen.



        private void tLSECHToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TryExecCoreCmd("TLS ECH", "tls ech");
        }

        private void tLSCertToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TryExecCoreCmd("TLS Certificate", "tls cert");
        }

        private void x25519ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TryExecCoreCmd("x25519", "x25519");
        }

        private void wireGuardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TryExecCoreCmd("WireGuard", "wg");
        }

        private void mLDSA65ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TryExecCoreCmd("ML-DSA-65", "mldsa65");
        }

        #endregion


        #region file menu

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var content = rtBoxOutput.Text;
            Misc.Utils.CopyToClipboardAndPrompt(content);
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var content = VgcApis.Misc.Utils.ReadFromClipboard();
            SetResult("Paste", content);
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var content = rtBoxOutput.Text;
            VgcApis.Misc.UI.SaveToFileAndPrompt(VgcApis.Models.Consts.Files.TxtExt, content);
        }

        private void closeToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion

        #region password
        private void uUIDV4ToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            DoFiveTimes("UUID v4", () => Guid.NewGuid().ToString());
        }

        private void numberToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DoFiveTimes("Number", () => VgcApis.Misc.Utils.PickRandomChars("1234567890", 32));
        }

        private void alphabetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DoFiveTimes(
                "Number",
                () => VgcApis.Misc.Utils.PickRandomChars("abcdefghijklmnopqrstuvwxyz", 32)
            );
        }

        private void numAlphabetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DoFiveTimes(
                "Number + Alphabet",
                () => VgcApis.Misc.Utils.PickRandomChars("1234567890abcdefghijklmnopqrstuvwxyz", 32)
            );
        }

        private void numAlphabetSymbolToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DoFiveTimes(
                "Number + Alphabet + Symbol",
                () =>
                    VgcApis.Misc.Utils.PickRandomChars(
                        "1234567890abcdefghijklmnopqrstuvwxyz/!&#^?,(*'<[.~$:_\"{`%;)|+>=-\\}@]",
                        32
                    )
            );
        }

        private void hexToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DoFiveTimes("Number", () => VgcApis.Misc.Utils.RandomHex(32));
        }

        #endregion



        #region menu tool
        private void scanQRCodeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            void ok(string result)
            {
                VgcApis.Misc.UI.Invoke(() =>
                {
                    rtBoxOutput.Text = result;
                    SetTitle("Scan QR code");
                });
            }

            void fail()
            {
                VgcApis.Misc.UI.MsgBox(I18N.NoQRCode);
            }

            Libs.QRCode.QRCode.ScanQRCode(ok, fail);
        }

        private void decodeBase64ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // vmess://abc... -> vmess:::abc...
            var content = (rtBoxOutput.Text ?? "").Replace("://", ":::");
            var b64s = VgcApis.Misc.Utils.ExtractBase64Strings(content, 8);
            var r = new List<string>();
            foreach (var b64 in b64s)
            {
                var text = VgcApis.Misc.Utils.Base64DecodeToString(b64);
                if (!string.IsNullOrEmpty(text))
                {
                    r.Add(text);
                }
            }
            SetResults("Decode - Base64", r);
        }

        private void decodeUnicodeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var content = rtBoxOutput.Text ?? "";
            var r = VgcApis.Misc.Utils.UnescapeUnicode(content);
            SetResult("Decode - Unicode", r);
        }

        private void decodeUriToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var content = rtBoxOutput.Text ?? "";
            var r = VgcApis.Misc.Utils.UriDecode(content);
            SetResult("Decode - URI", r);
        }

        private void encodeBase64ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var content = rtBoxOutput.Text ?? "";
            try
            {
                var b64 = VgcApis.Misc.Utils.Base64EncodeString(content);
                SetResult("Encode - Base64", b64);
            }
            catch (Exception ex)
            {
                VgcApis.Misc.UI.MsgBox(ex.Message);
            }
        }

        private void encodeUnicodeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var content = rtBoxOutput.Text ?? "";
            var r = VgcApis.Misc.Utils.EscapeUnicode(content);
            SetResult("Encode - Unicode", r);
        }

        private void encodeUriToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var content = rtBoxOutput.Text ?? "";
            var r = VgcApis.Misc.Utils.UriEncode(content);
            SetResult("Encode - URI", r);
        }
        #endregion
    }
}
