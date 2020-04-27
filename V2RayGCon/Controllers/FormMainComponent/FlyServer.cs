using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using V2RayGCon.Resources.Resx;

namespace V2RayGCon.Controllers.FormMainComponent
{
    class FlyServer : FormMainComponentController
    {
        int statusBarUpdateInterval = 300;
        int flyPanelUpdateInterval = 300;

        readonly Form formMain;
        readonly FlowLayoutPanel flyPanel;
        readonly Services.Servers servers;
        readonly Services.Settings setting;
        readonly ToolStripComboBox cboxMarkFilter;
        readonly ToolStripStatusLabel tslbTotal, tslbPrePage, tslbNextPage;
        readonly ToolStripDropDownButton tsdbtnPager;

        readonly VgcApis.Libs.Tasks.LazyGuy
            lazyStatusBarUpdater,
            lazySearchResultDisplayer,
            lazyFlyPanelUpdater;

        readonly Views.UserControls.WelcomeUI welcomeItem = null;

        int curPageNumber = 0;
        int totalPageNumber = 1;
        bool isFocusOnFormMain;

        public FlyServer(
            Form formMain,
            FlowLayoutPanel panel,
            ToolStripLabel lbMarkFilter,
            ToolStripComboBox cboxMarkeFilter,
            ToolStripStatusLabel tslbTotal,
            ToolStripDropDownButton tsdbtnPager,
            ToolStripStatusLabel tslbPrePage,
            ToolStripStatusLabel tslbNextPage,
            ToolStripMenuItem miResizeFormMain)
        {
            servers = Services.Servers.Instance;
            setting = Services.Settings.Instance;

            this.formMain = formMain;
            this.flyPanel = panel;
            this.cboxMarkFilter = cboxMarkeFilter;
            this.tsdbtnPager = tsdbtnPager;
            this.tslbTotal = tslbTotal;
            this.tslbPrePage = tslbPrePage;
            this.tslbNextPage = tslbNextPage;

            this.welcomeItem = new Views.UserControls.WelcomeUI();

            lazyFlyPanelUpdater = new VgcApis.Libs.Tasks.LazyGuy(RefreshFlyPanelWorker, flyPanelUpdateInterval)
            {
                Name = "Vgc.Panel.Refreasher",
            };

            lazyStatusBarUpdater = new VgcApis.Libs.Tasks.LazyGuy(UpdateStatusBarWorker, statusBarUpdateInterval)
            {
                Name = "", // disable debug logging
            };

            lazySearchResultDisplayer = new VgcApis.Libs.Tasks.LazyGuy(ShowSearchResultNow, 1300)
            {
                Name = "Vgc.Search",
            };

            InitFormControls(lbMarkFilter, miResizeFormMain);
            BindDragDropEvent();
            RefreshFlyPanelLater();
            WatchServers();
        }

        #region public method

        public List<VgcApis.Interfaces.ICoreServCtrl> GetFilteredList()
        {
            var list = servers.GetAllServersOrderByIndex();
            var keyword = searchKeywords?.Replace(@" ", "");

            if (string.IsNullOrEmpty(keyword))
            {
                return list.ToList();
            }

            return list
                .Where(serv => serv.GetCoreStates().GetterInfoForSearch(
                    infos => infos.Any(
                        info => VgcApis.Misc.Utils.PartialMatchCi(info, keyword))))
                .ToList();
        }

        public void LoopThroughAllServerUI(Action<Views.UserControls.ServerUI> operation)
        {
            var controls = GetAllServerControls();

            controls.Select(e =>
            {
                VgcApis.Misc.UI.RunInUiThreadIgnoreError(formMain, () => operation(e));
                return true;
            })
            .ToList();  // make sure operation is executed
        }

        public override void Cleanup()
        {
            UnwatchServers();

            lazyFlyPanelUpdater?.Dispose();
            lazyStatusBarUpdater?.Dispose();
            lazySearchResultDisplayer?.Dispose();

            RemoveAllServersConrol(true);
        }

        public void RemoveAllServersConrol(bool blocking = false)
        {
            var controlList = GetAllServerControls();

            VgcApis.Misc.UI.RunInUiThreadIgnoreError(formMain, () =>
            {
                flyPanel.SuspendLayout();
                flyPanel.Controls.Clear();
                flyPanel.ResumeLayout();
            });

            if (blocking)
            {
                DisposeFlyPanelControlByList(controlList);
            }
            else
            {
                VgcApis.Misc.Utils.RunInBackground(() => DisposeFlyPanelControlByList(controlList));
            }
        }

        void UpdateStatusBarWorker()
        {
            SetSearchKeywords();

            var start = DateTime.Now.Millisecond;
            int filteredListCount = GetFilteredList().Count;
            int allServersCount = servers.CountAllServers();
            int serverControlCount = GetAllServerControls().Count();

            // may cause dead lock in UI thread
            int selectedServersCount = servers.CountSelectedServers();

            VgcApis.Misc.UI.RunInUiThreadIgnoreError(formMain, () =>
            {
                UpdateStatusBarText(filteredListCount, allServersCount, selectedServersCount, serverControlCount);
                UpdateStatusBarPageSelectorMenuItemsOndemand();
                UpdateStatusBarPagingButtons();

                // prevent formain lost focus after click next page
                if (isFocusOnFormMain)
                {
                    formMain.Focus();
                    isFocusOnFormMain = false;
                }
            });

            // throttle
            var relex = statusBarUpdateInterval - (DateTime.Now.Millisecond - start);
            VgcApis.Misc.Utils.Sleep(Math.Max(0, relex));
        }

        public void RefreshFlyPanelLater() => lazyFlyPanelUpdater?.Deadline();

        #endregion

        #region private method
        void RefreshFlyPanelWorker()
        {
            var start = DateTime.Now.Millisecond;
            servers.ResetIndex();
            var flatList = GetFilteredList();
            var pagedList = GenPagedServerList(flatList);
            var showWelcome = servers.CountAllServers() == 0;

            VgcApis.Misc.UI.RunInUiThreadIgnoreError(formMain, () =>
            {
                RemoveAllServersConrol();
                if (showWelcome)
                {
                    // add on element no need to suspend
                    LoadWelcomeItem();
                    return;
                }

                flyPanel.SuspendLayout();
                foreach (var serv in pagedList)
                {
                    var ui = new Views.UserControls.ServerUI(serv);
                    flyPanel.Controls.Add(ui);
                }
                flyPanel.ResumeLayout();
            });

            lazyStatusBarUpdater?.Postpone();
            var relex = flyPanelUpdateInterval - (DateTime.Now.Millisecond - start);
            VgcApis.Misc.Utils.Sleep(Math.Max(0, relex));
        }

        private void WatchServers()
        {
            servers.OnRequireFlyPanelUpdate += OnRequireFlyPanelUpdateHandler;
            servers.OnRequireFlyPanelReload += OnRequireFlyPanelReloadHandler;
            servers.OnServerPropertyChange += OnServerPropertyChangeHandler;
        }

        private void UnwatchServers()
        {
            servers.OnRequireFlyPanelReload -= OnRequireFlyPanelReloadHandler;
            servers.OnRequireFlyPanelUpdate -= OnRequireFlyPanelUpdateHandler;
            servers.OnServerPropertyChange -= OnServerPropertyChangeHandler;
        }

        List<VgcApis.Interfaces.ICoreServCtrl> GenPagedServerList(
            List<VgcApis.Interfaces.ICoreServCtrl> serverList)
        {
            var count = serverList.Count;
            var pageSize = setting.serverPanelPageSize;
            totalPageNumber = (int)Math.Ceiling(1.0 * count / pageSize);
            curPageNumber = Misc.Utils.Clamp(curPageNumber, 0, totalPageNumber);

            if (serverList.Count <= 0)
            {
                return serverList;
            }

            var begin = curPageNumber * pageSize;
            var num = Math.Min(pageSize, count - begin);
            return serverList.GetRange(begin, num);
        }

        void OnServerPropertyChangeHandler(object sender, EventArgs args)
        {
            lazyStatusBarUpdater?.Deadline();
        }

        void SetSearchKeywords()
        {
            // bug
            var controls = GetAllServerControls();
            VgcApis.Misc.Utils.RunInBackground(() =>
            {
                controls.ForEach(c => c.SetKeywords(searchKeywords));
            });
        }

        private void UpdateStatusBarText(
            int filteredListCount,
            int allServersCount,
            int selectedServersCount,
            int serverControlCount)
        {
            var text = string.Format(
                $"{I18N.StatusBarMessageTpl}",
                filteredListCount,
                allServersCount,
                selectedServersCount,
                serverControlCount);

            if (text != tslbTotal.Text)
            {
                tslbTotal.Text = text;
            }
        }

        private void UpdateStatusBarPageSelectorMenuItemsOndemand()
        {
            if (totalPageNumber == pagerMenuItemCache.Count())
            {
                return;
            }

            var pagerMenu = tsdbtnPager.DropDownItems;
            pagerMenu.Clear();
            UpdateStatusBarPagerMenuCache();
            var groupedMenu = VgcApis.Misc.UI.AutoGroupMenuItems(
                pagerMenuItemCache,
                VgcApis.Models.Consts.Config.MenuItemGroupSize);
            pagerMenu.AddRange(groupedMenu.ToArray());
        }

        private void UpdateStatusBarPagingButtons()
        {
            var showPager = totalPageNumber > 1;
            var cpn = VgcApis.Misc.Utils.Clamp(curPageNumber, 0, totalPageNumber);

            if (tsdbtnPager.Visible != showPager)
            {
                tsdbtnPager.Visible = showPager;
                tslbNextPage.Visible = showPager;
                tslbPrePage.Visible = showPager;
            }

            tslbPrePage.Enabled = cpn > 0;
            tslbNextPage.Enabled = totalPageNumber > 1 && cpn < totalPageNumber - 1;
            tsdbtnPager.Text = string.Format(I18N.StatusBarPagerInfoTpl, cpn + 1, totalPageNumber);

            for (int i = 0; i < pagerMenuItemCache.Count; i++)
            {
                pagerMenuItemCache[i].Checked = cpn == i;
            }
        }

        readonly List<ToolStripMenuItem> pagerMenuItemCache = new List<ToolStripMenuItem>();
        private void UpdateStatusBarPagerMenuCache()
        {
            var ps = setting.serverPanelPageSize;
            pagerMenuItemCache.Clear();
            for (int i = 0; i < totalPageNumber; i++)
            {
                var pn = i;
                var title = string.Format(
                        I18N.StatusBarPagerMenuItemTpl,
                        pn + 1,
                        pn * ps + 1,
                        pn * ps + ps);
                var item = new ToolStripMenuItem(
                    title, null,
                    (s, a) =>
                    {
                        curPageNumber = pn;
                        isFocusOnFormMain = true;
                        RefreshFlyPanelLater();
                    });
                pagerMenuItemCache.Add(item);
            }
        }

        string searchKeywords = "";

        void ShowSearchResultNow()
        {
            // 如果不RemoveAll会乱序
            RemoveAllServersConrol();

            // 修改搜索项时应该清除选择,否则会有可显示列表外的选中项
            servers.SetAllServerIsSelected(false);

            lazyFlyPanelUpdater?.Throttle();
        }

        void ShowSearchResultLater() => lazySearchResultDisplayer?.Postpone();

        private void InitFormControls(
            ToolStripLabel lbMarkFilter,
            ToolStripMenuItem miResizeFormMain)
        {
            InitComboBoxMarkFilter();
            tslbPrePage.Click += (s, a) =>
            {
                curPageNumber--;
                isFocusOnFormMain = true;
                lazyFlyPanelUpdater?.Throttle();
            };

            tslbNextPage.Click += (s, a) =>
            {
                curPageNumber++;
                isFocusOnFormMain = true;
                lazyFlyPanelUpdater?.Throttle();
            };

            lbMarkFilter.Click += (s, a) => this.cboxMarkFilter.Text = string.Empty;
            miResizeFormMain.Click += (s, a) => ResizeFormMain();
        }

        private void ResizeFormMain()
        {
            var num = setting.serverPanelPageSize;
            if (num < 1 || num > 16)
            {
                return;
            }

            var height = 0;
            var width = 0;
            var first = flyPanel.Controls
                .OfType<Views.UserControls.ServerUI>()
                .Select(c =>
                {
                    height += c.Height + c.Margin.Vertical;
                    width = c.Width + c.Margin.Horizontal;
                    return width;
                })
                .ToList();

            if (width == 0)
            {
                return;
            }

            height += flyPanel.Padding.Vertical + 2;
            width += flyPanel.Padding.Horizontal + 2;

            formMain.Height += height - flyPanel.Height;
            formMain.Width += width - flyPanel.Width;
        }

        private void InitComboBoxMarkFilter()
        {
            UpdateMarkFilterItemList(cboxMarkFilter);

            cboxMarkFilter.DropDown += (s, e) =>
            {
                UpdateMarkFilterItemList(cboxMarkFilter);
                Misc.UI.ResetComboBoxDropdownMenuWidth(cboxMarkFilter);
            };

            cboxMarkFilter.TextChanged += (s, a) =>
            {
                searchKeywords = cboxMarkFilter.Text;
                ShowSearchResultLater();
            };
        }

        void UpdateMarkFilterItemList(ToolStripComboBox marker)
        {
            marker.Items.Clear();
            marker.Items.AddRange(servers.GetMarkList().ToArray());
        }

        void DisposeFlyPanelControlByList(List<Views.UserControls.ServerUI> controlList)
        {
            foreach (var control in controlList)
            {
                control.Cleanup();
            }

            /* let runtime handle dispose
            VgcApis.Misc.UI.RunInUiThread(formMain, () =>
            {
                foreach (var control in controlList)
                {
                    control.Dispose();
                }
            });
            */
        }

        List<Views.UserControls.ServerUI> GetDeletedControlList(
            List<VgcApis.Interfaces.ICoreServCtrl> serverList)
        {
            var result = new List<Views.UserControls.ServerUI>();

            foreach (var control in GetAllServerControls())
            {
                var config = control.GetConfig();
                if (serverList.Where(s => s.GetConfiger().GetConfig() == config)
                    .FirstOrDefault() == null)
                {
                    result.Add(control);
                }
                serverList.RemoveAll(s => s.GetConfiger().GetConfig() == config);
            }

            return result;
        }

        void RemoveWelcomeItem()
        {
            var controls = flyPanel.Controls.OfType<Views.UserControls.WelcomeUI>();
            foreach (var item in controls)
            {
                flyPanel.Controls.Remove(item);
            }
        }

        List<Views.UserControls.ServerUI> GetAllServerControls() => flyPanel.Controls
                .OfType<Views.UserControls.ServerUI>()
                .ToList();


        void OnRequireFlyPanelReloadHandler(object sender, EventArgs args) =>
            ReloadFlyPanel();

        void ReloadFlyPanel()
        {
            RemoveAllServersConrol();
            RefreshFlyPanelLater();
        }

        void OnRequireFlyPanelUpdateHandler(object sender, EventArgs args)
        {
            lazyFlyPanelUpdater?.Deadline();
        }

        private void LoadWelcomeItem()
        {
            var welcome = flyPanel.Controls
                .OfType<Views.UserControls.WelcomeUI>()
                .FirstOrDefault();

            if (welcome == null)
            {
                flyPanel.Controls.Add(welcomeItem);
            }
        }

        private void BindDragDropEvent()
        {
            flyPanel.DragEnter += (s, a) =>
            {
                a.Effect = DragDropEffects.Move;
            };

            flyPanel.DragDrop += (s, a) =>
            {
                // https://www.codeproject.com/Articles/48411/Using-the-FlowLayoutPanel-and-Reordering-with-Drag
                if (!(a.Data.GetData(typeof(Views.UserControls.ServerUI)) is Views.UserControls.ServerUI curItem))
                {
                    return;
                }

                var panel = s as FlowLayoutPanel;
                Point cursor = panel.PointToClient(new Point(a.X, a.Y));
                var destItem = panel.GetChildAtPoint(cursor) as Views.UserControls.ServerUI;
                if (destItem == null)
                {
                    return;
                }

                var destIdx = destItem.GetIndex();
                var curIdx = curItem.GetIndex();
                destIdx = destIdx - 0.5;
                curIdx = curIdx < destIdx ? destIdx + 0.2 : destIdx - 0.2;
                destItem.SetIndex(destIdx);
                curItem.SetIndex(curIdx);
                servers.ResetIndex();

                // refresh panel
                var destPos = panel.Controls.GetChildIndex(destItem, false);
                panel.Controls.SetChildIndex(curItem, destPos);
                panel.Invalidate();
            };
        }
        #endregion
    }
}
