﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using V2RayGCon.Resources.Resx;
using VgcApis.Interfaces;

namespace V2RayGCon.Controllers.FormMainComponent
{
    class FlyServer : FormMainComponentController
    {
        readonly int statusBarUpdateInterval = 1000;
        readonly int flyPanelUpdateInterval = 500;

        readonly Form formMain;
        readonly FlowLayoutPanel flyPanel;
        readonly Services.Servers servers;
        readonly Services.Settings setting;
        readonly VgcApis.UserControls.AcmComboBox cboxKeyword;
        readonly ToolStripStatusLabel tslbTotal,
            tslbPrePage,
            tslbNextPage;
        readonly ToolStripDropDownButton tsdbtnPager;

        readonly VgcApis.Libs.Tasks.LazyGuy lazyStatusBarUpdater,
            lazyFlyPanelUpdater;

        readonly Views.UserControls.WelcomeUI welcomeItem = null;

        int curPageNumber = 0;
        int totalPageNumber = 1;
        bool isFocusOnFormMain;

        readonly Acm acm;

        public FlyServer(
            Form formMain,
            FlowLayoutPanel panel,
            ToolStripLabel lbClearKeyword,
            ToolStripComboBox cboxKeyword,
            ToolStripStatusLabel tslbTotal,
            ToolStripDropDownButton tsdbtnPager,
            ToolStripStatusLabel tslbPrePage,
            ToolStripStatusLabel tslbNextPage,
            ToolStripMenuItem miResizeFormMain
        )
        {
            servers = Services.Servers.Instance;
            setting = Services.Settings.Instance;

            this.formMain = formMain;
            this.flyPanel = panel;
            this.cboxKeyword = cboxKeyword as VgcApis.UserControls.AcmComboBox;
            this.tsdbtnPager = tsdbtnPager;
            this.tslbTotal = tslbTotal;
            this.tslbPrePage = tslbPrePage;
            this.tslbNextPage = tslbNextPage;

            this.welcomeItem = new Views.UserControls.WelcomeUI();

            lazyFlyPanelUpdater = new VgcApis.Libs.Tasks.LazyGuy(
                RefreshFlyPanelWorker,
                flyPanelUpdateInterval,
                2000
            )
            {
                Name = "FormMain.RefreshFlyPanelWorker()",
            };

            lazyStatusBarUpdater = new VgcApis.Libs.Tasks.LazyGuy(
                UpdateStatusBarWorker,
                statusBarUpdateInterval,
                3000
            )
            {
                Name = "FormMain.UpdateStatusBarWorker()", // disable debug logging
            };

            InitFormControls(lbClearKeyword, miResizeFormMain);
            BindDragDropEvent();
            this.acm = new Acm(this.cboxKeyword);
            WatchServers();
            RefreshFlyPanelNow();
            DoSearchOptimization();
        }

        #region public method
        public void ResetServUiBorders()
        {
            var panel = flyPanel;
            var isFullBorder = IsMultiColumn(panel);
            foreach (var ctrls in panel.Controls)
            {
                if (ctrls is Views.UserControls.ServerUI sui)
                {
                    sui.SetBorderStyle(isFullBorder);
                }
            }
        }

        public List<ICoreServCtrl> GetFilteredList()
        {
            var r = new List<ICoreServCtrl>();
            var kwsr = kwSearcher;

            var f = kwsr.GetFilter();
            if (f != null)
            {
                var servs = servers.GetAllServersOrderByIndex();
                r = f.Filter(servs);
            }

            matchCountCache = r.Count;
            if (kwsr.TryGetIndex(out int index))
            {
                index = (index < 1 || index > r.Count) ? r.Count : index;
                curPageNumber = (index - 1) / setting.serverPanelPageSize;
            }
            return r;
        }

        public void LoopThroughAllServerUI(Action<Views.UserControls.ServerUI> operation)
        {
            var controls = GetAllServerControls();
            foreach (var control in controls)
            {
                VgcApis.Misc.UI.Invoke(() => operation?.Invoke(control));
            }
        }

        public override void Cleanup()
        {
            UnwatchServers();

            tsdbtnPager.DropDownOpening -= StatusBarPagerDropdownMenuOpeningHandler;

            lazyFlyPanelUpdater?.Dispose();
            lazyStatusBarUpdater?.Dispose();

            this.acm?.Dispose();

            RemoveAllServersConrol();

            Views.UserControls.ServerUI.currentServerUI = null;
        }

        public void RemoveAllServersConrol()
        {
            void worker()
            {
                var controlList = GetAllServerControls();
                flyPanel.SuspendLayout();
                flyPanel.Controls.Clear();
                flyPanel.ResumeLayout();
                flyPanel.PerformLayout();
                VgcApis.Misc.Utils.RunInBackground(() => DisposeFlyPanelControlByList(controlList));
            }
            VgcApis.Misc.UI.Invoke(worker);
        }

        void UpdateStatusBarWorker(Action done)
        {
            var start = DateTime.Now.Millisecond;
            int filteredListCount = matchCountCache;
            int allServersCount = servers.Count();
            int serverControlCount = GetAllServerControls().Count();

            // may cause dead lock in UI thread
            int selectedServersCount = servers.CountSelected();

            HighlightSearchKeywords();

            void worker()
            {
                UpdateStatusBarText(
                    filteredListCount,
                    allServersCount,
                    selectedServersCount,
                    serverControlCount
                );
                UpdateStatusBarPagerButtons();

                // prevent formain lost focus after click next page
                if (isFocusOnFormMain)
                {
                    formMain.Focus();
                    isFocusOnFormMain = false;
                }
            }

            void next()
            {
                var relex = statusBarUpdateInterval - (DateTime.Now.Millisecond - start);
                VgcApis.Misc.Utils.Sleep(Math.Max(0, relex));
                done?.Invoke();
            }

            VgcApis.Misc.UI.InvokeThen(worker, next);
        }

        public void RefreshFlyPanelLater()
        {
            lazyFlyPanelUpdater?.Deadline();
        }

        #endregion

        #region private method
        void DoSearchOptimization()
        {
            VgcApis.Misc.Utils.PreJITMethodsOnce(typeof(FlyServer));
        }

        void ScrollIntoView()
        {
            var kws = kwSearcher;
            if (!kws.TryGetIndex(out var index))
            {
                return;
            }

            List<Views.UserControls.ServerUI> controls = GetAllServerControls();
            Views.UserControls.ServerUI p = null;
            foreach (var c in controls)
            {
                p = c;
                if ((int)c.GetIndex() == index)
                {
                    break;
                }
            }
            if (p != null)
            {
                flyPanel.ScrollControlIntoView(p);
            }
        }

        void RefreshFlyPanelWorker(Action done)
        {
            var start = DateTime.Now.Millisecond;
            var flatList = GetFilteredList();
            var pagedList = GenPagedServerList(flatList);
            var showWelcome = servers.Count() == 0;
            List<Views.UserControls.ServerUI> removed = new List<Views.UserControls.ServerUI>();

            void next()
            {
                DisposeFlyPanelControlByList(removed);
                lazyStatusBarUpdater?.Deadline();
                var relex = flyPanelUpdateInterval - (DateTime.Now.Millisecond - start);
                VgcApis.Misc.Utils.Sleep(Math.Max(0, relex));
                done();
            }

            void worker()
            {
                if (flyPanel == null || flyPanel.IsDisposed)
                {
                    return;
                }

                if (showWelcome)
                {
                    removed = GetAllServerControls();
                    flyPanel.Controls.Clear();
                    flyPanel.PerformLayout();
                    flyPanel.Controls.Add(welcomeItem);
                    return;
                }

                flyPanel.SuspendLayout();
                flyPanel.Controls.Remove(welcomeItem);
                var rmServUis = VgcApis.Misc.UI.DoHouseKeeping<Views.UserControls.ServerUI>(
                    flyPanel,
                    pagedList.Count
                );
                removed.AddRange(rmServUis);
                var servUis = GetAllServerControls();
                BindServUiToCoreServCtrl(servUis, pagedList);
                flyPanel.ResumeLayout();
                ResetServUiBorders();
                ScrollIntoView();
            }

            VgcApis.Misc.UI.InvokeThen(worker, next);
        }

        bool IsMultiColumn(FlowLayoutPanel panel)
        {
            HashSet<int> ws = new HashSet<int>();
            foreach (Control ctrl in panel.Controls)
            {
                ws.Add(ctrl.Left);
            }
            return ws.Count > 1;
        }

        private void BindServUiToCoreServCtrl(
            List<Views.UserControls.ServerUI> servUis,
            List<ICoreServCtrl> coreServs
        )
        {
            if (servUis.Count != coreServs.Count)
            {
                throw new Exception("ServUi.Count != cs.Count");
            }

            for (int i = 0; i < servUis.Count; i++)
            {
                servUis[i].Rebind(coreServs[i]);
            }
        }

        private void WatchServers()
        {
            servers.OnRequireFlyPanelReload += OnRequireFlyPanelReloadHandler;
            servers.OnServerPropertyChange += OnServerPropertyChangeHandler;
        }

        private void UnwatchServers()
        {
            servers.OnRequireFlyPanelReload -= OnRequireFlyPanelReloadHandler;
            servers.OnServerPropertyChange -= OnServerPropertyChangeHandler;
        }

        List<ICoreServCtrl> GenPagedServerList(List<ICoreServCtrl> serverList)
        {
            var count = serverList.Count;
            var pageSize = setting.serverPanelPageSize;
            totalPageNumber = (int)Math.Ceiling(1.0 * count / pageSize);
            curPageNumber = VgcApis.Misc.Utils.Clamp(curPageNumber, 0, totalPageNumber);

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

        void HighlightSearchKeywords()
        {
            var hi = kwSearcher.GetHighlighter();
            // bug
            var controls = GetAllServerControls();
            VgcApis.Misc.Utils.RunInBackground(() =>
            {
                controls.ForEach(c => c.ChangeHighlighter(hi));
            });
        }

        private void UpdateStatusBarText(
            int filteredListCount,
            int allServersCount,
            int selectedServersCount,
            int serverControlCount
        )
        {
            var text = string.Format(
                $"{I18N.StatusBarMessageTpl}",
                filteredListCount,
                allServersCount,
                selectedServersCount,
                serverControlCount
            );

            if (text != tslbTotal.Text)
            {
                tslbTotal.Text = text;
            }
        }

        private void StatusBarPagerDropdownMenuOpeningHandler(object sender, EventArgs args)
        {
            List<ICoreServCtrl> flist = GetFilteredList();
            var cpn = VgcApis.Misc.Utils.Clamp(curPageNumber, 0, totalPageNumber);
            var groupSize = VgcApis.Models.Consts.Config.MenuItemGroupSize;
            var pageSize = setting.serverPanelPageSize;

            List<ToolStripMenuItem> menu;
            if (flist.Count < 1000)
            {
                var items = CreateBasicMenuItems(flist, cpn, 0, totalPageNumber, pageSize);
                menu = AutoGroupMenuItems(flist, items, groupSize);
            }
            else
            {
                menu = CreateDynamicPagingMenu(flist, pageSize, cpn, groupSize, 0, totalPageNumber);
            }
            tsdbtnPager.DropDownItems.Clear();
            tsdbtnPager.DropDown.PerformLayout();
            tsdbtnPager.DropDownItems.AddRange(menu.ToArray());
        }

        List<ToolStripMenuItem> CreateDynamicPagingMenu(
            List<ICoreServCtrl> filteredList,
            int pageSize,
            int currentPageNumber,
            int groupSize,
            int start,
            int end
        )
        {
            var ps = pageSize;
            var n = end - start;
            var step = 1;
            while (n > groupSize)
            {
                n /= groupSize;
                step *= groupSize;
            }

            if (step == 1)
            {
                List<ToolStripMenuItem> mis = CreateBasicMenuItems(
                    filteredList,
                    currentPageNumber,
                    start,
                    end,
                    ps
                );
                return mis;
            }

            var gmis = new List<ToolStripMenuItem>();
            for (int i = start; i < end; i += step)
            {
                var s = i;
                var e = Math.Min(i + step, end);
                var pageRange = string.Format("{0,5}-{1,5}", s + 1, e);
                var text = string.Format(
                    I18N.StatusBarPagerMenuItemTpl,
                    pageRange,
                    GetIndex(filteredList, s * pageSize),
                    GetIndex(filteredList, e * pageSize - 1)
                );

                var mi = new ToolStripMenuItem(text, null);
                mi.DropDownOpening += (o, a) =>
                {
                    var root = mi.DropDownItems;
                    if (root.Count < 1)
                    {
                        var dm = CreateDynamicPagingMenu(
                            filteredList,
                            pageSize,
                            currentPageNumber,
                            groupSize,
                            s,
                            e
                        );
                        root.AddRange(dm.ToArray());
                    }
                };
                gmis.Add(mi);
            }

            return gmis;
        }

        void RefreshFlyPanelNow()
        {
            lazyFlyPanelUpdater?.Throttle();
        }

        private List<ToolStripMenuItem> CreateBasicMenuItems(
            List<ICoreServCtrl> filteredList,
            int currentPageNumber,
            int start,
            int end,
            int pageSize
        )
        {
            var mis = new List<ToolStripMenuItem>();

            for (int i = start; i < end; i++)
            {
                var pn = i;
                var title = string.Format(
                    I18N.StatusBarPagerMenuItemTpl,
                    pn + 1,
                    GetIndex(filteredList, pn * pageSize),
                    GetIndex(filteredList, pn * pageSize + pageSize - 1)
                );

                void onClick(object s, EventArgs a)
                {
                    curPageNumber = pn;
                    ClearIndexSearchKeyword();
                    isFocusOnFormMain = true;
                    RefreshFlyPanelNow();
                }

                var item = new ToolStripMenuItem(title, null, onClick);
                item.Disposed += (s, a) => item.Click -= onClick;
                item.Checked = pn == currentPageNumber;
                mis.Add(item);
            }

            return mis;
        }

        private void UpdateStatusBarPagerButtons()
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
        }

        public List<ToolStripMenuItem> AutoGroupMenuItems(
            List<ICoreServCtrl> filteredList,
            List<ToolStripMenuItem> menuItems,
            int groupSize
        )
        {
            var mi = menuItems;
            var menuSpan = groupSize;
            var max = menuItems.Count;

            while (mi.Count() > groupSize)
            {
                mi = GroupPagerItemsWorker(filteredList, mi, max, groupSize, menuSpan);
                menuSpan *= groupSize;
            }
            return mi;
        }

        List<ToolStripMenuItem> GroupPagerItemsWorker(
            List<ICoreServCtrl> filteredList,
            IEnumerable<ToolStripMenuItem> menuItems,
            int maxIdx,
            int groupSize,
            int menuSpan
        )
        {
            var count = menuItems.Count();
            if (count <= groupSize)
            {
                return menuItems.ToList();
            }

            // grouping
            var pageSize = setting.serverPanelPageSize;
            var groups = new List<ToolStripMenuItem>();
            var servIdx = 0;
            var pageIdx = 0;
            while (servIdx < count)
            {
                var take = Math.Min(groupSize, count - servIdx);
                var last = Math.Min(pageIdx + menuSpan, maxIdx);
                var pageRange = string.Format("{0,5}-{1,5}", pageIdx + 1, last);
                var text = string.Format(
                    I18N.StatusBarPagerMenuItemTpl,
                    pageRange,
                    GetIndex(filteredList, pageIdx * pageSize),
                    GetIndex(filteredList, last * pageSize - 1)
                );
                var mis = menuItems.Skip(servIdx).Take(take).ToArray();
                var mi = new ToolStripMenuItem(text, null, mis);
                groups.Add(mi);
                servIdx += groupSize;
                pageIdx += menuSpan;
            }
            return groups;
        }

        int GetIndex(List<ICoreServCtrl> list, int index)
        {
            var i = Math.Max(0, Math.Min(list.Count - 1, index));
            var c = list[i];
            var r = c.GetCoreStates().GetIndex();
            return (int)r;
        }

        VgcApis.Libs.Infr.KeywordFilter kwSearcher = new VgcApis.Libs.Infr.KeywordFilter("");
        int matchCountCache = 0;

        private void InitFormControls(
            ToolStripLabel lbClearKeyword,
            ToolStripMenuItem miResizeFormMain
        )
        {
            InitComboBoxKeyword();

            tsdbtnPager.DropDownOpening += StatusBarPagerDropdownMenuOpeningHandler;

            tslbPrePage.Click += (s, a) =>
            {
                ClearIndexSearchKeyword();
                curPageNumber--;
                isFocusOnFormMain = true;
                RefreshFlyPanelNow();
            };

            tslbNextPage.Click += (s, a) =>
            {
                ClearIndexSearchKeyword();
                curPageNumber++;
                isFocusOnFormMain = true;
                RefreshFlyPanelNow();
            };

            lbClearKeyword.Click += (s, a) =>
            {
                cboxKeyword.Text = string.Empty;
                flyPanel.Focus();
                PerformSearch();
            };

            miResizeFormMain.Click += (s, a) => ResizeFormMain();
        }

        void ClearIndexSearchKeyword()
        {
            if (kwSearcher.TryGetIndex(out _))
            {
                cboxKeyword.Text = "";
                kwSearcher = new VgcApis.Libs.Infr.KeywordFilter("");
            }
        }

        private void ResizeFormMain()
        {
            var num = setting.serverPanelPageSize;
            if (num < 1)
            {
                return;
            }

            var height = 0;
            var width = 0;

            var controls = flyPanel.Controls.OfType<Views.UserControls.ServerUI>().ToList();
            foreach (var c in controls)
            {
                height += c.Height + c.Margin.Vertical;
                if (width == 0)
                {
                    width = c.Width + c.Margin.Horizontal;
                }
            }

            if (width == 0)
            {
                return;
            }

            height += flyPanel.Padding.Vertical + 2;
            width += flyPanel.Padding.Horizontal + 2;

            formMain.WindowState = FormWindowState.Normal;
            if (num <= 12)
            {
                formMain.Height += height - flyPanel.Height;
            }

            formMain.Width += width - flyPanel.ClientSize.Width;
        }

        private void InitComboBoxKeyword()
        {
            UpdateMarkFilterItems(cboxKeyword);

            cboxKeyword.DropDown += (s, e) =>
            {
                UpdateMarkFilterItems(cboxKeyword);
                VgcApis.Misc.UI.ResetComboBoxDropdownMenuWidth(cboxKeyword);
            };

            cboxKeyword.SelectedIndexChanged += (s, e) =>
            {
                PerformSearch();
            };

            cboxKeyword.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    PerformSearch();
                }
                else if (e.KeyCode == Keys.Escape)
                {
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    flyPanel.Focus();
                }
            };
        }

        void PerformSearch()
        {
            kwSearcher = new VgcApis.Libs.Infr.KeywordFilter(cboxKeyword.Text);
            this.tslbTotal.Text = I18N.Searching;
            RefreshFlyPanelNow();
        }

        void UpdateMarkFilterItems(ToolStripComboBox marker)
        {
            var marks = servers.GetMarkList().Select(mk => $"#mark like {mk}").ToArray();
            marker.Items.Clear();
            marker.Items.AddRange(marks);
        }

        void DisposeFlyPanelControlByList(List<Views.UserControls.ServerUI> controlList)
        {
            if (controlList == null)
            {
                return;
            }

            foreach (var control in controlList)
            {
                control.Cleanup();
            }

            VgcApis.Misc.UI.Invoke(() =>
            {
                foreach (var control in controlList)
                {
                    control.Dispose();
                }
            });
        }

        readonly object flyCtrlsLocker = new object();

        List<Views.UserControls.ServerUI> GetAllServerControls()
        {
            var result = new List<Views.UserControls.ServerUI>();
            lock (flyCtrlsLocker)
            {
                result = flyPanel.Controls.OfType<Views.UserControls.ServerUI>().ToList();
            }
            return result;
        }

        void OnRequireFlyPanelReloadHandler(object sender, EventArgs args) =>
            RefreshFlyPanelLater();

        private void BindDragDropEvent()
        {
            flyPanel.DragEnter += (s, a) =>
            {
                a.Effect = DragDropEffects.Move;
            };

            flyPanel.DragDrop += (s, a) =>
            {
                // https://www.codeproject.com/Articles/48411/Using-the-FlowLayoutPanel-and-Reordering-with-Drag
                if (
                    !(
                        a.Data.GetData(typeof(Views.UserControls.ServerUI))
                        is Views.UserControls.ServerUI curItem
                    )
                )
                {
                    return;
                }

                var panel = s as FlowLayoutPanel;
                Point cursor = panel.PointToClient(new Point(a.X, a.Y));
                if (!(panel.GetChildAtPoint(cursor) is Views.UserControls.ServerUI destItem))
                {
                    return;
                }

                var destIdx = destItem.GetIndex();
                var curIdx = curItem.GetIndex();
                destIdx -= 0.5;
                curIdx = curIdx < destIdx ? destIdx + 0.2 : destIdx - 0.2;
                destItem.SetIndex(destIdx);
                curItem.SetIndex(curIdx);
                servers.ResetIndex();

                // !must! reset item index!!
                var destPos = panel.Controls.GetChildIndex(destItem, false);
                panel.Controls.SetChildIndex(curItem, destPos);
                panel.Invalidate();
            };
        }
        #endregion
    }
}
