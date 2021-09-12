using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace V2RayGCon.Views.WinForms
{
    public partial class FormImportLinksResult : Form
    {
        public static void ShowResult(IEnumerable<string[]> importResults) =>
            VgcApis.Misc.UI.Invoke(
                () => new FormImportLinksResult(importResults).Show());


        IEnumerable<string[]> results;

        FormImportLinksResult(IEnumerable<string[]> importResults)
        {
            InitializeComponent();
            results = importResults;
            VgcApis.Misc.UI.AutoSetFormIcon(this);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void FormImportLinksResult_Shown(object sender, EventArgs e)
        {
            lvResult.Items.Clear();
            var total = results.Count();
            if (total > 1000)
            {
                pbLoading.Visible = true;
            }

            var count = 0;
            var step = Math.Max(total / 50, 1);
            var max = pbLoading.Maximum;
            var min = pbLoading.Minimum;
            var t = Math.Max(total, 1);

            lvResult.SuspendLayout();
            foreach (var result in results)
            {
                if (count % step == 0)
                {
                    var p = Math.Max(min, Math.Min(count * max / t, max));
                    pbLoading.Value = p;
                }
                result[0] = (++count).ToString();
                var item = new ListViewItem(result);
                lvResult.Items.Add(item);
            }
            lvResult.ResumeLayout();

            pbLoading.Visible = false;
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
                    @"index,link,mark,success,message" +
                    Environment.NewLine +
                    string.Join(Environment.NewLine, links));

        private void btnCopyAll_Click(object sender, EventArgs e)
        {
            var links = new List<string>();
            foreach (ListViewItem item in lvResult.Items)
            {
                links.Add(ConvertListViewItemToString(item));
            }

            CopyToClipboard(links);
        }
    }
}
