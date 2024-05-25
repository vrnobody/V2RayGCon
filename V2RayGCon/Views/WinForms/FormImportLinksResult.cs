using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace V2RayGCon.Views.WinForms
{
    public partial class FormImportLinksResult : Form
    {
        public static void ShowResult(IEnumerable<string[]> importResults) =>
            VgcApis.Misc.UI.Invoke(() => new FormImportLinksResult(importResults).Show());

        readonly IEnumerable<string[]> results;

        FormImportLinksResult(IEnumerable<string[]> importResults)
        {
            InitializeComponent();
            results = importResults;
            VgcApis.Misc.UI.AutoSetFormIcon(this);
            VgcApis.Misc.UI.AddTagToFormTitle(this);
        }

        private void FormImportLinksResult_Shown(object sender, EventArgs e)
        {
            lvResult.Items.Clear();
            lvResult.SuspendLayout();

            var total = results.Count();
            if (total > 1000)
            {
                pbLoading.Visible = true;
            }

            var max = pbLoading.Maximum;
            var min = pbLoading.Minimum;
            var t = Math.Max(total, 1);

            Task.Run(() =>
            {
                ResultLoader(t, min, max);
            });
        }

        #region loader
        private void ResultLoader(int total, int min, int max)
        {
            int count = 0;
            int seg = 1000;

            var it = results.GetEnumerator();

            bool stop = false;
            while (!stop)
            {
                Invoke(
                    (MethodInvoker)
                        delegate
                        {
                            var p = Math.Max(min, Math.Min(count * max / total, max));
                            pbLoading.Value = p;
                            for (int i = 0; i < seg; i++)
                            {
                                if (!it.MoveNext())
                                {
                                    stop = true;
                                    break;
                                }
                                var result = it.Current;
                                result[0] = (++count).ToString();
                                var item = new ListViewItem(result);
                                lvResult.Items.Add(item);
                            }
                        }
                );
                VgcApis.Misc.Utils.Sleep(100);
            }

            Invoke(
                (MethodInvoker)
                    delegate
                    {
                        lvResult.ResumeLayout();
                        lvResult.Items[lvResult.Items.Count - 1].EnsureVisible();
                        pbLoading.Visible = false;
                    }
            );
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
