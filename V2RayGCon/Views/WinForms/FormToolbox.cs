using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using V2RayGCon.Resources.Resx;

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
        CancellationTokenSource clipboardWatcherCts;
        bool formClosed = false;

        public FormToolbox()
        {
            InitializeComponent();

            VgcApis.Misc.UI.AutoSetFormIcon(this);
            VgcApis.Misc.UI.AddTagToFormTitle(this);

            this.formTitle = this.Text;
        }

        private void FormToolbox_FormClosing(object sender, FormClosingEventArgs e)
        {
            formClosed = true;
            clipboardWatcherCts?.Cancel();
        }

        #region helpers
        void StartClipboardWatcher(CancellationToken token)
        {
            var contents = new List<string>();
            var lastText = "";
            while (!token.IsCancellationRequested)
            {
                string s = "";
                VgcApis.Misc.UI.Invoke(() => s = VgcApis.Misc.Utils.ReadFromClipboard());
                if (!string.IsNullOrEmpty(s) && s != lastText && !contents.Contains(s))
                {
                    lastText = s;
                    contents.Add(s);
                    var text = string.Join("\n", contents);
                    VgcApis.Misc.UI.Invoke(() =>
                    {
                        SetResult("Clipboard", text);
                        VgcApis.Misc.UI.ScrollToBottom(rtboxOutput);
                    });
                }
                VgcApis.Misc.Utils.Sleep(500);
            }
        }

        string GetContent()
        {
            return rtboxOutput.Text ?? "";
        }

        void SetResult(string title, string result)
        {
            if (formClosed)
            {
                return;
            }
            this.Text = $"{formTitle} - {title}";
            rtboxOutput.Text = result;
        }

        void SetResults(string title, IEnumerable<string> results)
        {
            SetResult(title, string.Join("\n", results));
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
            SetResult(keyType, result);
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

        private void mLKEM768ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TryExecCoreCmd("ML-KEM-768", "mlkem768");
        }

        #endregion

        #region file menu

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var content = GetContent();
            Misc.Utils.CopyToClipboardAndPrompt(content);
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var content = VgcApis.Misc.Utils.ReadFromClipboard();
            SetResult("Paste", content);
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var content = GetContent();
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

        readonly string numbers = @"1234567890";
        readonly string alphabets = @"abcdefghijklmnopqrstuvwxyz";
        readonly string symbols = "/!&#^?,(*'<[.~$:_\"{`%;)|+>=-\\}@]";

        private void numberToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DoFiveTimes("Number", () => VgcApis.Misc.Utils.PickRandomChars(numbers, 64));
        }

        private void numAlphabetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var chars = $"{numbers}{alphabets}";
            DoFiveTimes("Number + Alphabet", () => VgcApis.Misc.Utils.PickRandomChars(chars, 64));
        }

        private void numAlphabetSymbolToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var a = alphabets;
            var n = numbers;
            var chars = $"{symbols}{n}{n}{a}{a}{a}{a}{a}";

            var password = VgcApis.Misc.Utils.PickRandomChars(chars, 1024);
            SetResult("Number + Alphabet + Symbol", password);
        }

        private void hexToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DoFiveTimes("Hex", () => VgcApis.Misc.Utils.RandomHex(64));
        }

        #endregion

        #region menu translate

        private void decodeBase64ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var content = GetContent();
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

        private void vmessToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // vmess://abc... -> vmess:::abc...
            var content = GetContent().Replace("://", ":::");
            var b64s = VgcApis.Misc.Utils.ExtractBase64Strings(content, 64);
            var r = new List<string>();
            foreach (var b64 in b64s)
            {
                var config = VgcApis.Misc.Utils.Base64DecodeToString(b64);
                try
                {
                    var s = VgcApis.Misc.Utils.FormatConfig(config);
                    if (!string.IsNullOrEmpty(s))
                    {
                        r.Add(s);
                        continue;
                    }
                }
                catch { }
                if (!string.IsNullOrEmpty(config))
                {
                    r.Add(config);
                }
            }
            SetResults("Decode - vmess://...", r);
        }

        private void decodeUnicodeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var content = GetContent();
            var r = VgcApis.Misc.Utils.UnescapeUnicode(content);
            SetResult("Decode - Unicode", r);
        }

        private void decodeUriToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var content = GetContent();
            var r = VgcApis.Misc.Utils.UriDecode(content);
            SetResult("Decode - URI", r);
        }

        private void encodeBase64ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var content = GetContent();
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
            var content = GetContent();
            var r = VgcApis.Misc.Utils.EscapeUnicode(content);
            SetResult("Encode - Unicode", r);
        }

        private void encodeUriToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var content = GetContent();
            var r = VgcApis.Misc.Utils.UriEncode(content);
            SetResult("Encode - URI", r);
        }

        private void upperCaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var c = GetContent();
            SetResult("Convert - Upper case", c.ToUpper());
        }

        private void lowerCaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var c = GetContent();
            SetResult("Convert - Lower case", c.ToLower());
        }

        private void mixedCaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var content = GetContent();
            var sb = new StringBuilder();
            foreach (var c in content)
            {
                if (!char.IsLetter(c))
                {
                    sb.Append(c);
                    continue;
                }
                if (VgcApis.Libs.Infr.PseudoRandom.Next(2) == 1)
                {
                    sb.Append(char.ToUpper(c));
                }
                else
                {
                    sb.Append(char.ToLower(c));
                }
            }
            SetResult("Convert - Mixed case", sb.ToString());
        }
        #endregion

        #region menu tool
        private void scanQRCodeToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            void ok(string result)
            {
                VgcApis.Misc.UI.Invoke(() => SetResult("Scan QR code", result));
            }

            void fail()
            {
                VgcApis.Misc.UI.MsgBox(I18N.NoQRCode);
            }

            Libs.QRCode.QRCode.ScanQRCode(ok, fail);
        }

        private void watchClipboardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var isChecked = watchClipboardToolStripMenuItem.Checked;
            watchClipboardToolStripMenuItem.Checked = !isChecked;

            if (isChecked)
            {
                clipboardWatcherCts?.Cancel();
                return;
            }

            var cts = new CancellationTokenSource();
            clipboardWatcherCts = cts;
            SetResult("Clipboard", "");
            VgcApis.Misc.Utils.RunInBackground(() => StartClipboardWatcher(cts.Token));
        }
        #endregion
    }
}
