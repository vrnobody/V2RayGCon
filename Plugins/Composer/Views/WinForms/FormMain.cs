using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;
using Composer.Misc;
using Composer.Resources.Langs;
using Composer.Services;

namespace Composer.Views.WinForms
{
    public partial class FormMain : Form
    {
        private readonly Settings settings;
        List<Models.PackageItem> packageItems;

        public FormMain(Settings settings)
        {
            InitializeComponent();

            VgcApis.Misc.UI.AutoSetFormIcon(this);
            this.settings = settings;
            this.packageItems = settings.GetPackageItems();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            InitControls();
            cboxNodeInsertPos.SelectedIndex = 1;
            RefreshPackageNamePanel();
            var name = settings.GetCurPackageName();
            var pkg = this.packageItems.FirstOrDefault(el => el.name == name);
            LoadPackageItemToEditor(pkg);
        }

        #region skelecton templates

        Dictionary<string, byte[]> SK_TPLS = new Dictionary<string, byte[]>()
        {
            { "Routing CN", Properties.Resources.Routing_CN },
            { "Balancer Random", Properties.Resources.Balancer_Random },
            { "Balancer Round Robin", Properties.Resources.Balancer_Round_Robin },
            { "Balancer Least Ping", Properties.Resources.Balancer_Least_Ping },
            { "Balancer Least Load", Properties.Resources.Balancer_Least_Load },
        };

        #endregion


        #region public methods
        Form curNodeFilterForm = null;

        public void ShowModifyNodeFilterForm(Models.ServerSelectorItem nodeFilterItem)
        {
            var cur = curNodeFilterForm;
            if (cur != null)
            {
                cur.Activate();
                VgcApis.Misc.UI.MsgBox(I18N.ErrCanNotOpenMulFormServSelector);
                return;
            }

            void onSave(Models.ServerSelectorItem r)
            {
                curNodeFilterForm = null;
                if (r == null)
                {
                    return;
                }

                var filters = this.curPkgItem.selectors;
                var filter = filters.FirstOrDefault(el => el.id == r.id);
                if (filter == null)
                {
                    r.index = filters.Count + 1;
                    filters.Add(r);
                }
                else
                {
                    filter.Copy(r);
                }
                RefreshNodeFilterPanel(filters);
            }

            var form = new FormServerSelector(this.settings, nodeFilterItem);
            this.curNodeFilterForm = form;
            form.FormClosed += (s, a) => onSave(form.result);
            form.Show();
            form.Activate();
        }

        public void DeleteNodeFilterItem(string nodeFilterId)
        {
            curPkgItem.selectors = curPkgItem.selectors.Where(el => el.id != nodeFilterId).ToList();
            Utils.ResetIndex(curPkgItem.selectors);
            RefreshNodeFilterPanel(curPkgItem.selectors);
        }

        Models.PackageItem curPkgItem = new Models.PackageItem();

        public void LoadPackageItemToEditor(Models.PackageItem pkgItem)
        {
            if (pkgItem == null)
            {
                return;
            }

            curPkgItem = VgcApis.Misc.Utils.Clone(pkgItem);
            settings.SaveCurPackageName(curPkgItem.name);

            this.tboxPkgName.Text = curPkgItem.name;
            this.rtboxSkelecton.Text = curPkgItem.skelecton;
            RefreshNodeFilterPanel(curPkgItem.selectors);
            this.cboxNodeInsertPos.SelectedIndex = curPkgItem.isAppend ? 1 : 0;
        }

        public void DeletePackageItem(string pkgName)
        {
            this.packageItems = this.packageItems.Where(pkg => pkg.name != pkgName).ToList();
            Utils.ResetIndex(this.packageItems);
            SavePackageNameItems();
            RefreshPackageNamePanel();
        }
        #endregion

        #region bind hotkey
        protected override bool ProcessCmdKey(ref Message msg, Keys keyCode)
        {
            switch (keyCode)
            {
                case (Keys.Control | Keys.K):
                    btnSkFormat.PerformClick();
                    break;
                default:
                    return base.ProcessCmdKey(ref msg, keyCode);
            }
            return true;
        }
        #endregion

        #region private methods
        void SavePackageNameItems()
        {
            this.settings.SavePackageItems(this.packageItems);
        }

        void RefreshNodeFilterPanel(List<Models.ServerSelectorItem> nodeFilterItems)
        {
            UI.RefreshPanel(
                flySelectors,
                nodeFilterItems,
                (el) => new UserControls.ServerSelectorUC(this, el)
            );
        }

        void RefreshPackageNamePanel()
        {
            UI.RefreshPanel(
                flyPkgNames,
                this.packageItems,
                (el) => new UserControls.PkgNameUC(this, el)
            );
        }

        void InitControls()
        {
            foreach (var kv in SK_TPLS)
            {
                cboxSkelectonTpl.Items.Add(kv.Key);
            }
            cboxSkelectonTpl.SelectedIndex = 0;
        }

        Models.PackageItem CollectCurrentSettings()
        {
            var filters = new List<Models.ServerSelectorItem>();
            foreach (UserControls.ServerSelectorUC nf in flySelectors.Controls)
            {
                filters.Add(nf.GetNodeFilterItem());
            }
            var r = new Models.PackageItem()
            {
                name = tboxPkgName.Text,
                uid = curPkgItem.uid,
                skelecton = rtboxSkelecton.Text,
                isAppend = cboxNodeInsertPos.SelectedIndex == 1,
                selectors = VgcApis.Misc.Utils.Clone(filters),
            };

            settings.SaveCurPackageName(r.name);
            return r;
        }

        #endregion

        #region UI event handlers


        private void btnLoadSkTpl_Click(object sender, EventArgs e)
        {
            var name = this.cboxSkelectonTpl.Text;
            if (!SK_TPLS.ContainsKey(name))
            {
                VgcApis.Misc.UI.MsgBox($"{I18N.UnkownTemplateName} ${name}");
                return;
            }
            rtboxSkelecton.Text = VgcApis.Misc.Utils.GetUtf8StringWithoutBom(SK_TPLS[name]);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            var name = tboxPkgName.Text;
            if (string.IsNullOrEmpty(name))
            {
                VgcApis.Misc.UI.MsgBox(I18N.ErrPkgNameIsEmpty);
                return;
            }

            var pkgItem = CollectCurrentSettings();
            var p = this.packageItems.FirstOrDefault(el => el.name == name);
            if (p == null)
            {
                pkgItem.SetIndex(this.packageItems.Count + 1);
                this.packageItems.Add(pkgItem);
            }
            else
            {
                p.Copy(pkgItem);
            }
            SavePackageNameItems();
            RefreshPackageNamePanel();
        }

        private void btnNewNodeFilter_Click(object sender, EventArgs e)
        {
            ShowModifyNodeFilterForm(new Models.ServerSelectorItem());
        }

        private void flyPkgNames_DragEnter(object sender, DragEventArgs e)
        {
            if (UI.HasDropablePackageNameControl(e))
            {
                e.Effect = DragDropEffects.Move;
            }
        }

        private void flySelectors_DragEnter(object sender, DragEventArgs e)
        {
            if (Misc.UI.HasDropableSelectorControl(e))
            {
                e.Effect = DragDropEffects.Move;
            }
        }

        private void flyPkgNames_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetData(typeof(UserControls.PkgNameUC)) is UserControls.PkgNameUC pkg)
            {
                if (!UI.SwapUserControls(flyPkgNames, pkg, e))
                {
                    return;
                }
                Utils.ResetIndex(this.packageItems);
                SavePackageNameItems();
                RefreshPackageNamePanel();
            }
        }

        private void flySelectors_DragDrop(object sender, DragEventArgs e)
        {
            if (
                VgcApis.Misc.UI.TryGetControlFromDragDropEvent(
                    e,
                    out UserControls.ServerSelectorUC nodeFilter
                )
            )
            {
                if (!UI.SwapUserControls(flySelectors, nodeFilter, e))
                {
                    return;
                }
            }
            else if (
                VgcApis.Misc.UI.TryGetIDropableControlFromDragDropEvent(
                    e,
                    VgcApis.Models.Consts.UI.VgcServUiName,
                    out var servUi
                )
            )
            {
                var tag = "node";
                var selector = this.curPkgItem.selectors.FirstOrDefault(el => el.tag == tag);
                if (selector == null)
                {
                    selector = new Models.ServerSelectorItem()
                    {
                        tag = tag,
                        index = this.curPkgItem.selectors.Count + 1,
                    };
                    this.curPkgItem.selectors.Add(selector);
                }
                var uid = servUi.GetUid();
                if (selector.servInfos.FirstOrDefault(el => el.uid == uid) != null)
                {
                    return;
                }
                var si = new Models.ServerInfoItem()
                {
                    index = selector.servInfos.Count + 1,
                    title = servUi.GetTitle(),
                    uid = uid,
                };
                selector.servInfos.Add(si);
            }
            else
            {
                return;
            }

            Utils.ResetIndex(this.curPkgItem.selectors);
            RefreshNodeFilterPanel(this.curPkgItem.selectors);
        }

        private void btnSkFormat_Click(object sender, EventArgs e)
        {
            var txt = rtboxSkelecton.Text;
            try
            {
                txt = VgcApis.Misc.Utils.FormatConfig(txt);
                rtboxSkelecton.Text = txt;
            }
            catch (Exception ex)
            {
                VgcApis.Misc.UI.MsgBox(ex.Message);
            }
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.curNodeFilterForm?.Close();
        }

        private void btnPack_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(tboxPkgName.Text))
            {
                VgcApis.Misc.UI.MsgBox(I18N.ErrPkgNameIsEmpty);
                return;
            }

            var pkgItem = CollectCurrentSettings();
            var uid = settings.ComposeServers(pkgItem);
            if (!string.IsNullOrEmpty(uid))
            {
                this.curPkgItem.uid = uid;
            }
        }
        #endregion
    }
}
