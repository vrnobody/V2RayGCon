using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Moq;
using V2RayGCon.Resources.Resx;

namespace V2RayGCon.Views.WinForms
{
    public partial class FormImportLinksResult : Form
    {
        public static void ShowResult(IEnumerable<string[]> importResults) =>
            VgcApis.Misc.UI.Invoke(() => new FormImportLinksResult(importResults).Show());

        readonly List<string[]> results;

        FormImportLinksResult(IEnumerable<string[]> importResults)
        {
            InitializeComponent();
            this.results = CopyResults(importResults);
            VgcApis.Misc.UI.AutoSetFormIcon(this);
            VgcApis.Misc.UI.AddTagToFormTitle(this);
        }

        private void FormImportLinksResult_Shown(object sender, EventArgs e)
        {
            lvResult.Items.Clear();
            var total = results.Count;
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

        List<string[]> CopyResults(IEnumerable<string[]> importResults)
        {
            var r = new List<string[]>();
            var it = importResults.GetEnumerator();
            try
            {
                while (it.MoveNext())
                {
                    r.Add(it.Current);
                }
            }
            finally
            {
                it.Dispose();
            }
            return r;
        }

        private void ResultLoader(int total, int min, int max)
        {
            var reasons = new Dictionary<string, int>();
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
                                    LoadOneRow(reasons, idx);
                                }
                                if ((DateTime.Now - prev).TotalMilliseconds > 500)
                                {
                                    break;
                                }
                            }
                            lvResult.ResumeLayout();
                            UpdateTotal(reasons, len);
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

        private void UpdateTotal(Dictionary<string, int> reasons, int len)
        {
            var s = string.Join(", ", reasons.Select(kv => $"{kv.Key}: {kv.Value}"));
            var text = $"{I18N.Total}: {len}, {s}";
            lbResult.Text = text;
            toolTip1.SetToolTip(lbResult, text);
        }

        void LoadOneRow(Dictionary<string, int> reasons, int idx)
        {
            var result = this.results[idx];
            result[0] = (idx + 1).ToString();
            var item = new ListViewItem(result);
            lvResult.Items.Add(item);

            var reason = result[4];
            if (reasons.ContainsKey(reason))
            {
                reasons[reason]++;
            }
            else
            {
                reasons[reason] = 1;
            }
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
