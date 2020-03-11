using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using V2RayGCon.Resources.Resx;

namespace V2RayGCon.Controllers.FormMainComponent
{
    class FlyServer : FormMainComponentController
    {
        readonly Form formMain;
        readonly FlowLayoutPanel flyPanel;
        readonly Services.Servers servers;
        readonly Services.Settings setting;
        readonly ToolStripComboBox cboxMarkFilter;
        readonly ToolStripStatusLabel tslbTotal, tslbPrePage, tslbNextPage;
        readonly ToolStripDropDownButton tsdbtnPager;

        readonly VgcApis.Libs.Tasks.LazyGuy lazyStatusBarUpdater, lazySearchResultDisplayer, lazyUiRefresher;

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

            lazyStatusBarUpdater = new VgcApis.Libs.Tasks.LazyGuy(UpdateStatusBarLater, 300);
            lazySearchResultDisplayer = new VgcApis.Libs.Tasks.LazyGuy(ShowSearchResultNow, 1000);
            lazyUiRefresher = new VgcApis.Libs.Tasks.LazyGuy(() => RefreshUI(), 300);

            InitFormControls(lbMarkFilter, miResizeFormMain);
            BindDragDropEvent();
            RefreshUI();
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
                VgcApis.Misc.UI.RunInUiThread(formMain, () =>
                {
                    operation(e);
                });
                return true;
            })
            .ToList();  // make sure operation is executed
        }

        public override void Cleanup()
        {
            UnwatchServers();

            lazyUiRefresher?.Quit();
            lazyStatusBarUpdater?.Quit();
            lazySearchResultDisplayer?.Quit();

            RemoveAllServersConrol(true);
        }

        public void RemoveAllServersConrol(bool blocking = false)
        {
            var controlList = GetAllServerControls();

            VgcApis.Misc.UI.RunInUiThread(formMain, () =>
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
                VgcApis.Misc.Utils.RunInBackground(
                    () => DisposeFlyPanelControlByList(
                        controlList));
            }
        }

        readonly VgcApis.Libs.Tasks.Bar updateStatusBarLock = new VgcApis.Libs.Tasks.Bar();
        public void UpdateStatusBarLater()
        {
            if (!updateStatusBarLock.Install())
            {
                lazyStatusBarUpdater?.DoItLater();
                return;
            }

            _ = Task.Run(() =>
            {
                HighLightSearchKeywordsNow();
                UpdateStatusBarThen(() => updateStatusBarLock.Remove());
            });
        }

        readonly VgcApis.Libs.Tasks.Bar refreshUiLock = new VgcApis.Libs.Tasks.Bar();
        public override bool RefreshUI()
        {
            if (!refreshUiLock.Install())
            {
                lazyUiRefresher?.DoItLater();
                return false;
            }

            _ = Task.Run(() =>
            {
                RefreshServersUiThen(() =>
                {
                    refreshUiLock.Remove();
                    UpdateStatusBarLater();
                });
            });

            return true;
        }
        #endregion

        #region private method

        private void RefreshServersUiThen(Action next)
        {
            servers.ResetIndex();
            var flatList = this.GetFilteredList();
            var pagedList = GenPagedServerList(flatList);

            VgcApis.Misc.UI.RunInUiThreadIgnoreErrorThen(formMain, () =>
            {
                ClearFlyPanel(pagedList);
                FillFlyPanelWith(ref pagedList);
            }, next);
        }

        private void FillFlyPanelWith(ref List<VgcApis.Interfaces.ICoreServCtrl> pagedList)
        {
            if (pagedList.Count <= 0 && string.IsNullOrWhiteSpace(this.searchKeywords))
            {
                LoadWelcomeItem();
            }
            else
            {
                RemoveDeletedServerItems(ref pagedList);
                AddNewServerItems(pagedList);
            }
        }

        private void ClearFlyPanel(List<VgcApis.Interfaces.ICoreServCtrl> pagedList)
        {
            if (pagedList.Count > 0)
            {
                RemoveWelcomeItem();
            }
            else
            {
                RemoveAllServersConrol();
            }
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
            UpdateStatusBarLater();
        }

        void UpdateStatusBarThen(Action next)
        {
            int filteredListCount = GetFilteredList().Count;
            int allServersCount = servers.CountAllServers();
            int selectedServersCount = servers.CountSelectedServers(); // may cause dead lock in UI thread
            int serverControlCount = GetAllServerControls().Count();

            VgcApis.Misc.UI.RunInUiThreadIgnoreErrorThen(formMain, () =>
            {
                UpdateStatusBarText(
                    filteredListCount,
                    allServersCount,
                    selectedServersCount,
                    serverControlCount);

                UpdateStatusBarPageSelectorMenuItemsOndemand();
                UpdateStatusBarPagingButtons();

                // prevent formain lost focus after click next page
                if (isFocusOnFormMain)
                {
                    formMain.Focus();
                    isFocusOnFormMain = false;
                }
            }, next);
        }



        void HighLightSearchKeywordsNow()
        {
            // bug
            var controls = GetAllServerControls();
            controls.ForEach(c => c.SetKeywords(searchKeywords));
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

            if (tsdbtnPager.Visible != showPager)
            {
                tsdbtnPager.Visible = showPager;
                tslbNextPage.Visible = showPager;
                tslbPrePage.Visible = showPager;
            }

            tslbPrePage.Enabled = curPageNumber != 0;
            tslbNextPage.Enabled = totalPageNumber > 1 && curPageNumber != totalPageNumber - 1;

            tsdbtnPager.Text = string.Format(
                    I18N.StatusBarPagerInfoTpl,
                    curPageNumber + 1,
                    totalPageNumber);

            for (int i = 0; i < pagerMenuItemCache.Count; i++)
            {
                pagerMenuItemCache[i].Checked = curPageNumber == i;
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
                        RefreshUI();
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

            RefreshUI();
        }

        void ShowSearchResultLater() => lazySearchResultDisplayer?.DoItLater();

        private void InitFormControls(
            ToolStripLabel lbMarkFilter,
            ToolStripMenuItem miResizeFormMain)
        {
            InitComboBoxMarkFilter();
            tslbPrePage.Click += (s, a) =>
            {
                curPageNumber--;
                isFocusOnFormMain = true;
                RefreshUI();
            };

            tslbNextPage.Click += (s, a) =>
            {
                curPageNumber++;
                isFocusOnFormMain = true;
                RefreshUI();
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
                // cboxMarkFilter has no Invoke method.
                VgcApis.Misc.UI.RunInUiThread(formMain, () =>
                {
                    UpdateMarkFilterItemList(cboxMarkFilter);
                    Misc.UI.ResetComboBoxDropdownMenuWidth(cboxMarkFilter);
                });
            };

            cboxMarkFilter.TextChanged += (s, a) =>
            {
                this.searchKeywords = cboxMarkFilter.Text;
                ShowSearchResultLater();
            };
        }

        void UpdateMarkFilterItemList(ToolStripComboBox marker)
        {
            marker.Items.Clear();
            marker.Items.AddRange(
                servers.GetMarkList().ToArray());
        }

        void AddNewServerItems(List<VgcApis.Interfaces.ICoreServCtrl> serverList)
        {
            flyPanel.Controls.AddRange(
                serverList
                    .Select(s => new Views.UserControls.ServerUI(s))
                    .ToArray());
        }

        void DisposeFlyPanelControlByList(List<Views.UserControls.ServerUI> controlList)
        {
            foreach (var control in controlList)
            {
                control.Cleanup();
            }
            VgcApis.Misc.UI.RunInUiThread(formMain, () =>
            {
                foreach (var control in controlList)
                {
                    control.Dispose();
                }
            });
        }

        void RemoveDeletedServerItems(
            ref List<VgcApis.Interfaces.ICoreServCtrl> serverList)
        {
            var deletedControlList = GetDeletedControlList(serverList);

            flyPanel.SuspendLayout();
            foreach (var control in deletedControlList)
            {
                flyPanel.Controls.Remove(control);
            }

            flyPanel.ResumeLayout();
            VgcApis.Misc.Utils.RunInBackground(() => DisposeFlyPanelControlByList(deletedControlList));
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
            var list = flyPanel.Controls
                .OfType<Views.UserControls.WelcomeUI>()
                .Select(e =>
                {
                    flyPanel.Controls.Remove(e);
                    return true;
                })
                .ToList();
        }

        List<Views.UserControls.ServerUI> GetAllServerControls() => flyPanel.Controls
                .OfType<Views.UserControls.ServerUI>()
                .ToList();


        void OnRequireFlyPanelReloadHandler(object sender, EventArgs args) =>
            ReloadFlyPanel();
        void ReloadFlyPanel()
        {
            RemoveAllServersConrol();
            RefreshUI();
        }

        void OnRequireFlyPanelUpdateHandler(object sender, EventArgs args)
        {
            RefreshUI();
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

                if (!(a.Data.GetData(typeof(Views.UserControls.ServerUI)) is Views.UserControls.ServerUI serverItemMoving))
                {
                    return;
                }

                var panel = s as FlowLayoutPanel;
                Point p = panel.PointToClient(new Point(a.X, a.Y));
                var controlDest = panel.GetChildAtPoint(p);
                int index = panel.Controls.GetChildIndex(controlDest, false);
                panel.Controls.SetChildIndex(serverItemMoving, index);
                var serverItemDest = controlDest as Views.UserControls.ServerUI;
                MoveServerListItem(ref serverItemMoving, ref serverItemDest);
            };
        }

        void MoveServerListItem(ref Views.UserControls.ServerUI moving, ref Views.UserControls.ServerUI destination)
        {
            if (moving == null || destination == null)
            {
                ReloadFlyPanel();
                return;
            }

            var indexDest = destination.GetIndex();
            var indexMoving = moving.GetIndex();
            if (indexDest == indexMoving)
            {
                return;
            }

            moving.SetIndex(
                indexDest < indexMoving ?
                indexDest - 0.5 :
                indexDest + 0.5);
            RefreshUI();
        }
        #endregion
    }
}
