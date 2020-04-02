using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;

namespace Luna.Views.WinForms
{
    public partial class FormDataGrid : Form
    {
        readonly int MAX_TITLE_LEN = 60;
        readonly int UPDATE_INTERVAL = 500;

        private readonly string title;
        private readonly DataTable dataSource;
        private readonly int defColumn;
        private string filterKeyword = string.Empty;

        public List<List<string>> results = new List<List<string>>();
        VgcApis.Libs.Tasks.LazyGuy uiUpdater;

        public FormDataGrid(string title, DataTable dataSource, int defColumn)
        {
            InitializeComponent();
            this.title = title;
            this.dataSource = dataSource;
            this.defColumn = defColumn;
            VgcApis.Misc.UI.AutoSetFormIcon(this);
            Disposed += (s, a) => Cleanup();
        }

        private void FormDataGrid_Load(object sender, EventArgs e)
        {
            InitControls();
            uiUpdater = new VgcApis.Libs.Tasks.LazyGuy(UpdateUiLater, UPDATE_INTERVAL);
            UpdateUiLater();
        }

        #region private methods
        void Cleanup()
        {
            uiUpdater?.Quit();
        }

        VgcApis.Libs.Tasks.Bar updating = new VgcApis.Libs.Tasks.Bar();
        void UpdateUiLater()
        {
            if (!updating.Install())
            {
                uiUpdater?.DoItLater();
                return;
            }

            UpdateUiThen(() => updating.Remove());
        }

        DataTable GetFilteredDataTable()
        {
            var ds = dataSource;
            var r = new DataTable();
            var idx = Math.Max(0, cboxColumnIdx.SelectedIndex);

            foreach (DataColumn column in ds.Columns)
            {
                r.Columns.Add(column.ToString());
            }

            foreach (DataRow row in ds.Rows)
            {
                var text = row[idx].ToString();
                if (VgcApis.Misc.Utils.MeasureSimilarityCi(text, filterKeyword) <= 0)
                {
                    continue;
                }

                var vs = new List<string>();
                foreach (string v in row.ItemArray)
                {
                    vs.Add(v);
                }
                r.Rows.Add(vs.ToArray());
            }

            return r;
        }

        void UpdateUiThen(Action next)
        {
            VgcApis.Misc.UI.RunInUiThreadIgnoreErrorThen(dgvData, () =>
            {
                var ds = GetFilteredDataTable();
                dgvData.DataSource = ds;
            }, next);
        }

        void InitControls()
        {
            lbTitle.Text = VgcApis.Misc.Utils.AutoEllipsis(title, MAX_TITLE_LEN);
            toolTip1.SetToolTip(lbTitle, title);
            InitColumnsBox(dataSource);
        }

        void InitColumnsBox(DataTable dataSource)
        {
            var items = cboxColumnIdx.Items;
            items.Clear();
            foreach (DataColumn column in dataSource.Columns)
            {
                items.Add(column.ToString());
            }

            VgcApis.Misc.UI.ResetComboBoxDropdownMenuWidth(cboxColumnIdx);

            var count = cboxColumnIdx.Items.Count;
            if (count > 0)
            {
                // lua index starts from 1
                cboxColumnIdx.SelectedIndex = VgcApis.Misc.Utils.Clamp(
                    defColumn - 1, 0, count);
            }
        }

        void SetResult()
        {
            results.Clear();
            var dt = dgvData.DataSource as DataTable;
            foreach (DataRow row in dt.Rows)
            {
                var lr = new List<string>();
                foreach (string value in row.ItemArray)
                {
                    lr.Add(value);
                }
                results.Add(lr);
            }
        }
        #endregion

        #region UI events

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            SetResult();
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void tboxFilter_TextChanged(object sender, EventArgs e)
        {
            filterKeyword = tboxFilter.Text;
            uiUpdater?.DoItLater();
        }

        private void cboxColumnIdx_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateUiLater();
        }
        #endregion


    }
}
