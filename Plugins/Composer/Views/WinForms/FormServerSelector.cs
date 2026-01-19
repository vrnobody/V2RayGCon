using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using Composer.Misc;
using Composer.Resources.Langs;
using Composer.Services;

namespace Composer.Views.WinForms
{
    public partial class FormServerSelector : Form
    {
        private readonly Settings settings;
        private readonly Models.ServerSelectorItem nodeFilterItem;
        private readonly VgcApis.Controllers.KeywordFilterAcm acm;

        public Models.ServerSelectorItem result = null;

        public FormServerSelector(Settings settings, Models.ServerSelectorItem nodeFilterItem)
        {
            InitializeComponent();
            this.settings = settings;
            this.nodeFilterItem = VgcApis.Misc.Utils.Clone(nodeFilterItem);

            VgcApis.Misc.UI.AutoSetFormIcon(this);
            InitControls();
            this.acm = new VgcApis.Controllers.KeywordFilterAcm(this.cboxFilterKw);
        }

        #region public methods
        public void DeleteServer(string uid)
        {
            this.nodeFilterItem.servInfos = this
                .nodeFilterItem.servInfos.Where(el => el.uid != uid)
                .ToList();
            RefreshServersPanel();
        }
        #endregion

        #region private methods
        void UpdateTotal()
        {
            var cFilter = 0;
            var kw = this.cboxFilterKw.Text;
            if (!string.IsNullOrEmpty(kw))
            {
                cFilter = settings.GetServers(kw).Count;
            }
            var cCustom = this.nodeFilterItem.servInfos.Count;
            var s = string.Format(I18N.TotalLabelText, cFilter, cCustom, cFilter + cCustom);
            lbTotal.Text = s;
            toolTip1.SetToolTip(lbTotal, s);
        }

        void InitControls()
        {
            this.tboxTag.Text = nodeFilterItem.tag;
            this.cboxFilterKw.Text = nodeFilterItem.filter;
            RefreshServersPanel();
        }

        void RefreshServersPanel()
        {
            this.nodeFilterItem.servInfos = settings.RemoveNonExistServers(
                this.nodeFilterItem.servInfos
            );
            UI.RefreshPanel(
                flyCustomServers,
                this.nodeFilterItem.servInfos,
                (el) => new UserControls.ServerInfoUC(this, el)
            );
            UpdateTotal();
        }

        Models.ServerSelectorItem CollectSettings()
        {
            var si = new List<Models.ServerInfoItem>();
            foreach (UserControls.ServerInfoUC servInfo in this.flyCustomServers.Controls)
            {
                si.Add(servInfo.GetServInfoItem());
            }

            var r = new Models.ServerSelectorItem()
            {
                filter = this.cboxFilterKw.Text,
                id = this.nodeFilterItem.id,
                tag = this.tboxTag.Text,
                servInfos = si,
            };
            r.SetIndex(this.nodeFilterItem.GetIndex());
            return r;
        }

        #endregion

        #region UI event handlers
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.tboxTag.Text))
            {
                VgcApis.Misc.UI.MsgBox(I18N.ErrTagIsEmpty);
                return;
            }
            this.result = CollectSettings();
            this.Close();
        }

        private void btnPullServers_Click(object sender, EventArgs e)
        {
            var servs = this.settings.GetServersSelected();
            var uids = this.nodeFilterItem.servInfos.Select(si => si.uid).ToList();
            var c = uids.Count;
            foreach (var coreServ in servs)
            {
                var uid = coreServ.GetCoreStates().GetUid();
                if (!uids.Contains(uid))
                {
                    var si = new Models.ServerInfoItem(coreServ);
                    si.SetIndex(++c);
                    this.nodeFilterItem.servInfos.Add(si);
                }
            }
            RefreshServersPanel();
        }

        private void flyCustomServers_DragEnter(object sender, DragEventArgs e)
        {
            if (UI.HasDropableSelectorControl(e))
            {
                e.Effect = DragDropEffects.Move;
            }
        }

        private void flyCustomServers_DragDrop(object sender, DragEventArgs e)
        {
            if (
                e.Data.GetData(typeof(UserControls.ServerInfoUC))
                is UserControls.ServerInfoUC servInfoUC
            )
            {
                if (!Misc.UI.SwapUserControls(this.flyCustomServers, servInfoUC, e))
                {
                    return;
                }
                Utils.ResetIndex(this.nodeFilterItem.servInfos);
            }
            else if (
                VgcApis.Misc.UI.TryGetIDropableControlFromDragDropEvent(
                    e,
                    VgcApis.Models.Consts.UI.VgcServUiName,
                    out var servUi
                )
            )
            {
                var uid = servUi.GetUid();
                var infos = this.nodeFilterItem.servInfos;
                if (infos.FirstOrDefault(el => el.uid == uid) != null)
                {
                    return;
                }
                var si = new Models.ServerInfoItem() { title = servUi.GetTitle(), uid = uid };
                si.SetIndex(this.nodeFilterItem.servInfos.Count + 1);
                this.nodeFilterItem.servInfos.Add(si);
            }
            else
            {
                return;
            }
            RefreshServersPanel();
        }

        private void btnRefreshTotal_Click(object sender, EventArgs e)
        {
            RefreshServersPanel();
        }

        private void tboxFilterKw_TextChanged(object sender, EventArgs e)
        {
            RefreshServersPanel();
        }

        private void FormServerSelector_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.acm?.Dispose();
        }
        #endregion
    }
}
