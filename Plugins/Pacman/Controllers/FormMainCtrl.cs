using Pacman.Resources.Langs;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Pacman.Controllers
{
    class FormMainCtrl
    {
        readonly Services.Settings settings;
        readonly Button btnSave,
            btnDelete,
            btnPull,
            btnPack,
            btnChain;
        readonly TextBox tboxName;
        readonly FlowLayoutPanel flyContent;
        readonly ListBox lstBoxPackages;
        List<Models.Data.Bean> beanList;
        readonly ComboBox cboxBalancerStrategy,
            cboxObsInterval,
            cboxObsUrl;

        public FormMainCtrl(
            Services.Settings settings,
            TextBox tboxName,
            FlowLayoutPanel flyContent,
            ListBox lstBoxPackages,
            Button btnSave,
            Button btnDelete,
            Button btnChain,
            Button btnPack,
            ComboBox cboxBalancerStrategy,
            ComboBox cboxObsInterval,
            ComboBox cboxObsUrl,
            Button btnPull,
            Button btnSelectAll,
            Button btnSelectInvert,
            Button btnSelectNone,
            Button btnDeleteSelected,
            Button btnRefreshSelected
        )
        {
            this.settings = settings;

            this.tboxName = tboxName;
            this.flyContent = flyContent;
            this.lstBoxPackages = lstBoxPackages;
            this.btnDelete = btnDelete;
            this.btnChain = btnChain;
            this.btnPack = btnPack;
            this.btnPull = btnPull;
            this.btnSave = btnSave;
            this.cboxBalancerStrategy = cboxBalancerStrategy;
            this.cboxObsInterval = cboxObsInterval;
            this.cboxObsUrl = cboxObsUrl;

            BindEvent(
                btnSelectAll,
                btnSelectInvert,
                btnSelectNone,
                btnDeleteSelected,
                btnRefreshSelected
            );
        }

        #region drag and drop
        readonly List<string> dropableObjectsTypeString = new List<string>
        {
            "Pacman.Views.UserControls.BeanUI",
            "V2RayGCon.Views.UserControls.ServerUI",
        };

        bool IsDropableObject(DragEventArgs args)
        {
            foreach (string type in args.Data.GetFormats())
            {
                if (dropableObjectsTypeString.Contains(type))
                {
                    return true;
                }
            }
            return false;
        }

        void ResetFlyContentItemsIndex()
        {
            var controlList = flyContent.Controls;
            var count = 1;
            foreach (Views.UserControls.BeanUI control in controlList)
            {
                control.index = count++;
            }
        }

        void BindDragDropEvent()
        {
            flyContent.DragEnter += (s, a) =>
            {
                if (!IsDropableObject(a))
                {
                    return;
                }
                a.Effect = DragDropEffects.All;
            };

            flyContent.DragDrop += (s, a) =>
            {
                Views.UserControls.BeanUI beanUI = null;
                // https://www.codeproject.com/Articles/48411/Using-the-FlowLayoutPanel-and-Reordering-with-Drag

                var objName = "V2RayGCon.Views.UserControls.ServerUI";

                if (a.Data.GetDataPresent(objName))
                {
                    var item = (VgcApis.Interfaces.IDropableControl)a.Data.GetData(objName);
                    var bean = new Models.Data.Bean
                    {
                        title = item.GetTitle(),
                        uid = item.GetUid(),
                        status = item.GetStatus(),
                    };

                    foreach (Views.UserControls.BeanUI control in flyContent.Controls)
                    {
                        if (control.GetBean().uid == bean.uid)
                        {
                            // update title
                            control.SetTitle(bean.title);
                            control.SetStatus(bean.status);
                            return;
                        }
                    }

                    beanUI = new Views.UserControls.BeanUI();
                    flyContent.Controls.Add(beanUI);
                    beanUI.Reload(bean);
                }

                if (beanUI == null && a.Data.GetDataPresent(typeof(Views.UserControls.BeanUI)))
                {
                    beanUI = (Views.UserControls.BeanUI)
                        a.Data.GetData(typeof(Views.UserControls.BeanUI));
                }

                if (beanUI == null)
                {
                    return;
                }

                var destIndex = flyContent.Controls.GetChildIndex(
                    flyContent.GetChildAtPoint(flyContent.PointToClient(new Point(a.X, a.Y))),
                    false
                );
                flyContent.Controls.SetChildIndex(beanUI, destIndex);

                ResetFlyContentItemsIndex();
            };
        }

        #endregion

        #region public methods
        public void Run()
        {
            RefreshPackageListBox();
            RefreshFlyContent();
            BindControlsEvent();
            BindDragDropEvent();
        }

        private void BindControlsEvent()
        {
            btnSave.Click += (s, a) => SaveCurPackageSetting();
            btnDelete.Click += (s, a) => DeletePackage();
            lstBoxPackages.SelectedIndexChanged += (s, a) => PackageListSelectedIndexChanged();
            btnPull.Click += (s, a) => PullSelectedServerFromMainWindow();
            btnPack.Click += (s, a) => Pack();
            btnChain.Click += (s, a) => Chain();
        }
        #endregion

        #region private methods
        private void BindEvent(
            Button btnSelectAll,
            Button btnSelectInvert,
            Button btnSelectNone,
            Button btnDeleteSelected,
            Button btnRefreshSelected
        )
        {
            btnSelectAll.Click += (s, a) => LoopThroughFlyContentItems((b) => b.Select(true));
            btnSelectNone.Click += (s, a) => LoopThroughFlyContentItems((b) => b.Select(false));
            btnSelectInvert.Click += (s, a) =>
                LoopThroughFlyContentItems((b) => b.InvertSelection());
            btnDeleteSelected.Click += (s, a) =>
                LoopThroughFlyContentItems(
                    (b) =>
                    {
                        if (b.isSelected)
                        {
                            flyContent.Controls.Remove(b);
                        }
                    }
                );
            btnRefreshSelected.Click += (s, a) =>
            {
                var list = settings.GetAllServersList();
                LoopThroughFlyContentItems(b =>
                {
                    var bean = b.GetBean();
                    var c = list.FirstOrDefault(t => t.GetCoreStates().GetUid() == bean.uid);
                    if (c == null)
                    {
                        flyContent.Controls.Remove(b);
                        return;
                    }

                    var coreStates = c.GetCoreStates();
                    b.SetStatus(coreStates.GetStatus());
                    b.SetTitle(coreStates.GetTitle());
                });
            };
        }

        void LoopThroughFlyContentItems(Action<Views.UserControls.BeanUI> action)
        {
            flyContent.SuspendLayout();
            var controls = flyContent.Controls.OfType<Views.UserControls.BeanUI>().ToList();

            foreach (Views.UserControls.BeanUI control in controls)
            {
                action(control);
            }
            flyContent.ResumeLayout();
        }

        void Pack()
        {
            var uidList = GetFlyContentBeanList()
                .Where(b => b.isSelected)
                .Select(b => b.uid)
                .ToList();

            var servList = settings
                .GetAllServersList()
                .Where(s => uidList.Contains(s.GetCoreStates().GetUid()))
                .ToList();

            var package = settings.GetPackageList().FirstOrDefault(p => p.name == tboxName.Text);

            var strategy = (VgcApis.Models.Datas.Enums.BalancerStrategies)
                cboxBalancerStrategy.SelectedIndex;
            var interval = cboxObsInterval.Text;
            var url = cboxObsUrl.Text;

            var newUid = settings.Pack(
                servList,
                package?.uid,
                tboxName.Text,
                interval,
                url,
                strategy
            );
            if (package != null && !string.IsNullOrEmpty(newUid))
            {
                package.uid = newUid;
                settings.SavePackage(package);
            }
        }

        void Chain()
        {
            var uidList = GetFlyContentBeanList()
                .Where(b => b.isSelected)
                .Select(b => b.uid)
                .ToList();

            var servList = settings
                .GetAllServersList()
                .Where(s => uidList.Contains(s.GetCoreStates().GetUid()))
                .ToList();

            var package = settings.GetPackageList().FirstOrDefault(p => p.name == tboxName.Text);

            var newUid = settings.Chain(servList, package?.uid, tboxName.Text);
            if (package != null && !string.IsNullOrEmpty(newUid))
            {
                package.uid = newUid;
                settings.SavePackage(package);
            }
        }

        List<Models.Data.Bean> GetFlyContentBeanList()
        {
            var result = new List<Models.Data.Bean>();
            foreach (Views.UserControls.BeanUI beanUI in flyContent.Controls)
            {
                result.Add(beanUI.GetBean().Clone());
            }
            return result;
        }

        void PullSelectedServerFromMainWindow()
        {
            var curList = GetFlyContentBeanList();
            var selectedServerList = settings
                .GetAllServersList()
                .Where(s => s.GetCoreStates().IsSelected())
                .ToList();

            foreach (var serverCtrl in selectedServerList)
            {
                var states = serverCtrl.GetCoreStates();

                var found = curList.FirstOrDefault(
                    b => b.uid == serverCtrl.GetCoreStates().GetUid()
                );
                if (found != null)
                {
                    found.title = states.GetTitle();
                    found.status = states.GetStatus();
                    continue;
                }
                curList.Add(
                    new Models.Data.Bean
                    {
                        title = states.GetTitle(),
                        uid = serverCtrl.GetCoreStates().GetUid(),
                        status = states.GetStatus(),
                    }
                );
            }
            this.beanList = curList;
            RefreshFlyContent();
        }

        void DoHouseKeeping(int exp)
        {
            flyContent.SuspendLayout();
            var ctrls = flyContent.Controls.OfType<Views.UserControls.BeanUI>().ToList();
            var cur = ctrls.Count;
            for (int i = cur - 1; i >= exp; i--)
            {
                flyContent.Controls.Remove(ctrls[i]);
            }

            var beans = new List<Views.UserControls.BeanUI>();
            for (int i = cur; i < exp; i++)
            {
                beans.Add(new Views.UserControls.BeanUI());
            }
            flyContent.Controls.AddRange(beans.ToArray());
            flyContent.ResumeLayout();
        }

        void RefreshFlyContent()
        {
            var clone = beanList?.ToList();

            DoHouseKeeping(clone?.Count ?? 0);

            if (clone == null)
            {
                return;
            }

            var ctrls = flyContent.Controls.OfType<Views.UserControls.BeanUI>().ToList();
            if (ctrls.Count != clone.Count)
            {
                return;
            }

            for (int i = 0; i < ctrls.Count; i++)
            {
                ctrls[i].Reload(clone[i]);
            }
        }

        void PackageListSelectedIndexChanged()
        {
            var index = lstBoxPackages.SelectedIndex;
            var package = settings.GetPackageByIndex(index);
            ShowPackage(package);
        }

        void ShowPackage(Models.Data.Package package)
        {
            tboxName.Text = package.name;
            cboxBalancerStrategy.SelectedIndex = package.strategy;
            cboxObsInterval.Text = package.interval;
            cboxObsUrl.Text = package.url;
            beanList = package.beans.Select(b => b.Clone()).ToList();
            RefreshFlyContent();
        }

        void DeletePackage()
        {
            var name = tboxName.Text;

            if (string.IsNullOrEmpty(name))
            {
                return;
            }

            if (!VgcApis.Misc.UI.Confirm(string.Format(I18N.DeleteTpl, name)))
            {
                return;
            }

            settings.RemovePackageByName(name);
            RefreshPackageListBox();
        }

        void SaveCurPackageSetting()
        {
            var name = tboxName.Text;
            if (string.IsNullOrEmpty(name))
            {
                Libs.UI.MsgBoxAsync(I18N.NameCanNotBeNull);
                return;
            }

            var package = new Models.Data.Package
            {
                name = name,
                strategy = cboxBalancerStrategy.SelectedIndex,
                interval = cboxObsInterval.Text,
                url = cboxObsUrl.Text,
                beans = GetFlyContentBeanList()
            };

            Libs.UI.MsgBoxAsync(settings.SavePackage(package) ? I18N.Done : I18N.NullParam);

            RefreshPackageListBox();
        }

        private void RefreshPackageListBox()
        {
            var packages = settings.GetPackageList();
            this.lstBoxPackages.Items.Clear();
            foreach (var package in packages)
            {
                lstBoxPackages.Items.Add(package.name);
            }
        }
        #endregion

        #region UI

        #endregion
    }
}
