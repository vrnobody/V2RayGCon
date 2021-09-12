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
            var count = 0;
            var items = results.Select(r =>
            {
                r[0] = (++count).ToString();
                return new ListViewItem(r);
            }).ToArray();
            lvResult.SuspendLayout();
            lvResult.Items.Clear();
            lvResult.Items.AddRange(items);
            lvResult.ResumeLayout();
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
