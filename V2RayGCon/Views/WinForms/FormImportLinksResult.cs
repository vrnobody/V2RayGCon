using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using V2RayGCon.Resources.Resx;

namespace V2RayGCon.Views.WinForms
{
    public partial class FormImportLinksResult : Form
    {
        public static void ShowResult(VgcApis.Libs.Infr.ImportResultRecorder recorder) =>
            VgcApis.Misc.UI.Invoke(() => new FormImportLinksResult(recorder).Show());

        readonly VgcApis.Libs.Infr.ImportResultRecorder recorder;
        readonly List<string[]> results;

        FormImportLinksResult(VgcApis.Libs.Infr.ImportResultRecorder recorder)
        {
            InitializeComponent();

            this.recorder = recorder;
            this.results = recorder.GetResults();
            VgcApis.Misc.UI.AutoSetFormIcon(this);
            VgcApis.Misc.UI.AddTagToFormTitle(this);
        }

        private void FormImportLinksResult_Shown(object sender, EventArgs e)
        {
            lvResult.Items.Clear();
            var total = recorder.GetTotalCount();
            if (total > 1000)
            {
                pbLoading.Visible = true;
            }

            var max = pbLoading.Maximum;
            var min = pbLoading.Minimum;
            var t = Math.Max(total, 1);

            Task.Run(() =>
            {
                try
                {
                    ResultLoader(t, min, max);
                }
                catch
                {
                    // incase form is closed while loading results
                }
            });
        }

        #region loader
        private void ResultLoader(int total, int min, int max)
        {
            var len = this.results.Count;
            var idx = 0;
            while (idx < len)
            {
                Invoke(
                    (MethodInvoker)
                        delegate
                        {
                            var pg = Math.Max(min, Math.Min(idx * max / total, max));
                            pbLoading.Value = pg;
                            var prev = DateTime.Now;
                            lvResult.SuspendLayout();
                            while (idx < len)
                            {
                                for (int i = 0; i < 200 && idx < len; i++, idx++)
                                {
                                    LoadOneRow(idx);
                                }
                                if ((DateTime.Now - prev).TotalMilliseconds > 500)
                                {
                                    break;
                                }
                            }
                            lvResult.ResumeLayout();
                            UpdateTotal(len);
                        }
                );
                VgcApis.Misc.Utils.Sleep(100);
            }

            Invoke(
                (MethodInvoker)
                    delegate
                    {
                        pbLoading.Visible = false;
                    }
            );
        }

        private void UpdateTotal(int len)
        {
            var s = this.recorder.GetReasonsSubTotal();
            var text = $"{I18N.Total}: {len}, {s}";
            lbResult.Text = text;
            toolTip1.SetToolTip(lbResult, text);
        }

        void LoadOneRow(int idx)
        {
            var result = this.results[idx];
            result[0] = (idx + 1).ToString();
            var item = new ListViewItem(result);
            lvResult.Items.Add(item);
        }

        #endregion

        #region UI events

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            var links = new List<string>();

            foreach (ListViewItem item in lvResult.SelectedItems)
            {
                links.Add(ConvertListViewItemToString(item));
            }
            CopyToClipboard(links);
        }

        string ConvertListViewItemToString(ListViewItem item)
        {
            var count = item.SubItems.Count;
            var list = new List<string>();
            for (var i = 0; i < count; i++)
            {
                list.Add(item.SubItems[i].Text);
            }
            return string.Join(",", list);
        }

        void CopyToClipboard(List<string> links) =>
            Misc.Utils.CopyToClipboardAndPrompt(
                @"index,link,mark,success,message"
                    + Environment.NewLine
                    + string.Join(Environment.NewLine, links)
            );

        private void btnCopyAll_Click(object sender, EventArgs e)
        {
            var links = new List<string>();
            foreach (ListViewItem item in lvResult.Items)
            {
                links.Add(ConvertListViewItemToString(item));
            }

            CopyToClipboard(links);
        }
        #endregion
    }
}
