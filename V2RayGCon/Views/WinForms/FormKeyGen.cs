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

        public FormKeyGen()
        {
            InitializeComponent();

            VgcApis.Misc.UI.AutoSetFormIcon(this);
            VgcApis.Misc.UI.AddTagToFormTitle(this);
        }

        #region helpers
        string GetCoreFullPath()
        {
            var exe = @"xray.exe";
            var folder = VgcApis.Misc.Utils.GetCoreFolderFullPath();
            var path = Path.Combine(folder, exe);
            return path;
        }

        void TryExecCore(string args)
        {
            var exe = GetCoreFullPath();
            if (!File.Exists(exe))
            {
                VgcApis.Misc.UI.MsgBox($"{I18N.ExeNotFound}{Environment.NewLine}{exe}");
                return;
            }
            var result = VgcApis.Misc.Utils.ExecuteAndGetStdOut(exe, args, 5000, null);
            rtBoxOutput.Text = result;
        }
        #endregion

        #region UI event handler
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void uUIDV4ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var ids = new List<string>();
            for (int i = 0; i < 5; i++)
            {
                var id = Guid.NewGuid().ToString();
                ids.Add(id);
            }
            this.rtBoxOutput.Text = string.Join(Environment.NewLine, ids);
        }

        private void tLSECHToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TryExecCore("tls ech");
        }

        private void tLSCertToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TryExecCore("tls cert");
        }

        private void x25519ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TryExecCore("x25519");
        }

        private void wireGuardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TryExecCore("wg");
        }

        private void mLDSA65ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TryExecCore("mldsa65");
        }
        #endregion
    }
}
