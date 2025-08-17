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
using V2RayGCon.Controllers.FormTextConfigEditorComponent;
using V2RayGCon.Resources.Resx;

namespace V2RayGCon.Views.WinForms
{
    public partial class FormKeyGen : Form
    {
        #region Sigleton
        static readonly VgcApis.BaseClasses.AuxSiWinForm<FormKeyGen> auxSiForm =
            new VgcApis.BaseClasses.AuxSiWinForm<FormKeyGen>();

        public static FormKeyGen GetForm() => auxSiForm.GetForm();

        public static void ShowForm() => auxSiForm.ShowForm();
        #endregion

        readonly string title;

        public FormKeyGen()
        {
            InitializeComponent();

            this.title = this.Text;

            VgcApis.Misc.UI.AutoSetFormIcon(this);
            VgcApis.Misc.UI.AddTagToFormTitle(this);
        }

        #region helpers
        void DoFiveTimes(string keyType, Func<string> gen)
        {
            var r = new List<string>();
            for (int i = 0; i < 5; i++)
            {
                r.Add(gen());
            }
            rtBoxOutput.Text = string.Join(Environment.NewLine, r);
            SetTitleKeyType(keyType);
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
            SetTitleKeyType(keyType);
        }
        #endregion

        #region UI event handler



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

        private void uUIDV4ToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            DoFiveTimes("UUID v4", () => Guid.NewGuid().ToString());
        }

        void SetTitleKeyType(string keyType)
        {
            this.Text = $"{title} - {keyType}";
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var content = rtBoxOutput.Text;
            Misc.Utils.CopyToClipboardAndPrompt(content);
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

        private void fragmentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            rtBoxOutput.Text = Misc.Caches.Jsons.LoadExample("frag01");
            SetTitleKeyType("Fragment - #01");
        }

        private void cNDirectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            rtBoxOutput.Text = Misc.Caches.Jsons.LoadExample("routeCnDirect01");
            SetTitleKeyType("Routing - CN direct");
        }
        #endregion
    }
}
