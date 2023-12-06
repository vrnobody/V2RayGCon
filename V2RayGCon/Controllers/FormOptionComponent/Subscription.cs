using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using V2RayGCon.Resources.Resx;

namespace V2RayGCon.Controllers.OptionComponent
{
    public class Subscription : OptionComponentController
    {
        readonly FlowLayoutPanel flyPanel;
        readonly Button btnAdd,
            btnUpdate,
            btnUseAll,
            btnInvertSelection;
        readonly CheckBox chkSubsIsUseProxy;
        private readonly CheckBox chkSubsIsAutoPatch;
        readonly Services.Settings setting;
        readonly Services.Servers servers;
        readonly Services.ShareLinkMgr slinkMgr;
        readonly VgcApis.Libs.Tasks.LazyGuy lazyCounter;

        public Subscription(
            FlowLayoutPanel flyPanel,
            Button btnAdd,
            Button btnUpdate,
            CheckBox chkSubsIsUseProxy,
            CheckBox chkSubsIsAutoPatch,
            Button btnUseAll,
            Button btnInvertSelection
        )
        {
            this.setting = Services.Settings.Instance;
            this.servers = Services.Servers.Instance;
            this.slinkMgr = Services.ShareLinkMgr.Instance;

            this.flyPanel = flyPanel;
            this.btnAdd = btnAdd;
            this.btnUpdate = btnUpdate;
            this.chkSubsIsUseProxy = chkSubsIsUseProxy;
            this.chkSubsIsAutoPatch = chkSubsIsAutoPatch;
            this.btnUseAll = btnUseAll;
            this.btnInvertSelection = btnInvertSelection;

            chkSubsIsUseProxy.Checked = setting.isUpdateUseProxy;

            lazyCounter = new VgcApis.Libs.Tasks.LazyGuy(UpdateServUiTotalWorker, 1000, 2000)
            {
                Name = "SubsCtrl.CountTotal()",
            };

            InitPanel();
            BindEvent();

            MarkDuplicatedSubsInfo();

            UpdateServUiTotal(this, EventArgs.Empty);
        }

        #region public method
        public void UpdateServUiTotal(object sender, EventArgs args) => lazyCounter?.Deadline();

        public override void Cleanup()
        {
            ReleaseEvent();
            lazyCounter?.ForgetIt();
            lazyCounter?.Dispose();
        }

        public override bool SaveOptions()
        {
            string curOptions = GetCurOptions();
            string oldOptions = setting.GetSubscriptionConfig();

            if (curOptions != oldOptions)
            {
                setting.SetSubscriptionConfig(curOptions);
                return true;
            }
            return false;
        }

        public override bool IsOptionsChanged()
        {
            var oldOptions = setting.GetSubscriptionConfig();
            return GetCurOptions() != oldOptions;
        }

        public void Merge(string rawSetting)
        {
            var mergedSettings = MergeIntoCurSubsItems(rawSetting);
            setting.SetSubscriptionConfig(mergedSettings);
            Misc.UI.ClearFlowLayoutPanel(this.flyPanel);
            InitPanel();
        }

        public void MarkDuplicatedSubsInfo()
        {
            VgcApis.Misc.UI.Invoke(MarkDuplicatedSubsInfoWorker);
        }

        public void RemoveSubsUi(Views.UserControls.SubscriptionUI subsUi)
        {
            var subs = GetAllSubsUi();
            foreach (var sub in subs)
            {
                if (sub == subsUi)
                {
                    VgcApis.Misc.UI.Invoke(() => Remove(subsUi));
                }
            }

            UpdatePanelItemsIndex();
        }

        public void AutoAddEmptyUi()
        {
            var controls = GetAllSubsUi();
            foreach (Views.UserControls.SubscriptionUI ctrl in controls)
            {
                if (ctrl.IsEmpty())
                {
                    return;
                }
            }

            AddSubsUiItem(new Models.Datas.SubscriptionItem());
            UpdatePanelItemsIndex();
        }
        #endregion

        #region private method

        void UpdateServUiTotalWorker(Action done)
        {
            try
            {
                var markCounts = CountServMarks();
                var servUis = GetAllSubsUi();
                foreach (var servUi in servUis)
                {
                    var key = servUi.GetAlias();
                    var total = markCounts.ContainsKey(key) ? markCounts[key] : 0;
                    servUi.SetTotal(total);
                }
            }
            catch { }
            done?.Invoke();
        }

        Dictionary<string, int> CountServMarks()
        {
            var r = new Dictionary<string, int>();

            var servs = servers.GetAllServersOrderByIndex();
            var marks = servs.Select(serv => serv.GetCoreStates().GetMark()).ToList();

            foreach (var mark in marks)
            {
                if (string.IsNullOrEmpty(mark))
                {
                    continue;
                }

                if (!r.ContainsKey(mark))
                {
                    r[mark] = 0;
                }

                r[mark]++;
            }
            return r;
        }

        List<Views.UserControls.SubscriptionUI> GetAllSubsUi() =>
            flyPanel.Controls.OfType<Views.UserControls.SubscriptionUI>().ToList();

        void RemoveEmptyUi()
        {
            var controls = GetAllSubsUi();
            foreach (var control in controls)
            {
                if (control.IsEmpty())
                {
                    flyPanel.Controls.Remove(control);
                }
            }
        }

        void Remove(Views.UserControls.SubscriptionUI subsUi)
        {
            flyPanel.Controls.Remove(subsUi);
            AutoAddEmptyUi();
        }

        void MarkDuplicatedSubsInfoWorker()
        {
            var subsUis = GetAllSubsUi();
            var subs = subsUis.Select(ctrl => ctrl.GetValue()).ToList();

            var urls = subs.Select(item => item.url).ToList();
            var alias = subs.Select(item => item.alias).ToList();

            foreach (var subsUi in subsUis)
            {
                subsUi.UpdateTextBoxColor(alias, urls);
            }
        }

        string MergeIntoCurSubsItems(string rawSubsSetting)
        {
            var subs = new List<Models.Datas.SubscriptionItem>();
            try
            {
                var items = JsonConvert.DeserializeObject<List<Models.Datas.SubscriptionItem>>(
                    rawSubsSetting
                );
                if (items != null)
                {
                    subs = items;
                }
            }
            catch { }

            var curSubs = setting.GetSubscriptionItems();
            var urls = curSubs.Select(s => s.url).ToList();
            curSubs.AddRange(subs.Where(s => !urls.Contains(s.url)));
            var sorted = curSubs.OrderBy(s => s.alias).ToList();
            return JsonConvert.SerializeObject(sorted);
        }

        string GetCurOptions()
        {
            return JsonConvert.SerializeObject(CollectSubscriptionItems());
        }

        List<Models.Datas.SubscriptionItem> CollectSubscriptionItems()
        {
            var itemList = new List<Models.Datas.SubscriptionItem>();
            var urlCache = new List<string>();
            var subsUi = GetAllSubsUi();
            foreach (var item in subsUi)
            {
                var v = item.GetValue(); // capture

                if (urlCache.Contains(v.url))
                {
                    continue;
                }

                if (!string.IsNullOrEmpty(v.alias) || !string.IsNullOrEmpty(v.url))
                {
                    itemList.Add(v);
                    urlCache.Add(v.url);
                }
            }
            return itemList;
        }

        void InitPanel()
        {
            var subItemList = setting.GetSubscriptionItems();
            chkSubsIsAutoPatch.Checked = setting.isAutoPatchSubsInfo;

            if (subItemList.Count <= 0)
            {
                subItemList.Add(new Models.Datas.SubscriptionItem());
            }

            foreach (var item in subItemList)
            {
                AddSubsUiItem(item);
            }

            // 因tboxAlias.Changed事件引起一个空的subsUi在顶部,所以要先删除
            RemoveEmptyUi();
            AutoAddEmptyUi();
        }

        void AddSubsUiItem(Models.Datas.SubscriptionItem data)
        {
            var subsUi = new Views.UserControls.SubscriptionUI(this, data);
            flyPanel.Controls.Add(subsUi);
            flyPanel.ScrollControlIntoView(subsUi);
            UpdateServUiTotal(this, EventArgs.Empty);
        }

        void BindEventBtnAddClick()
        {
            this.btnAdd.Click += (s, a) =>
            {
                AddSubsUiItem(new Models.Datas.SubscriptionItem());
                UpdatePanelItemsIndex();
            };
        }

        void BindEventBtnSelections()
        {
            this.btnUseAll.Click += (s, a) =>
            {
                var subsUi = GetAllSubsUi();
                foreach (var subUi in subsUi)
                {
                    subUi.SetIsUse(true);
                }
            };

            this.btnInvertSelection.Click += (s, a) =>
            {
                var subsUi = GetAllSubsUi();
                foreach (var subUi in subsUi)
                {
                    var selected = subUi.IsUse();
                    subUi.SetIsUse(!selected);
                }
            };
        }

        void BindEventBtnUpdateClick()
        {
            this.btnUpdate.Click += (s, a) =>
            {
                this.btnUpdate.Enabled = false;
                var subs = GetSubsIsInUse();

                if (subs.Count <= 0)
                {
                    this.btnUpdate.Enabled = true;
                    VgcApis.Misc.UI.MsgBox(I18N.NoSubsUrlAvailable);
                    return;
                }

                VgcApis.Misc.Utils.RunInBackground(() =>
                {
                    GetAvailableProxyInfo(out var isSocks5, out var proxyPort);
                    var links = Misc.Utils.FetchLinksFromSubcriptions(subs, isSocks5, proxyPort);

                    LogDownloadFails(
                        links.Where(l => string.IsNullOrEmpty(l[0])).Select(l => l[1])
                    );

                    slinkMgr.ImportLinkWithOutV2cfgLinksBatchModeSync(
                        links.Where(l => !string.IsNullOrEmpty(l[0])).ToList()
                    );

                    UpdateServUiTotal(this, EventArgs.Empty);

                    VgcApis.Misc.UI.Invoke(() => this.btnUpdate.Enabled = true);
                });
            };
        }

        private void LogDownloadFails(IEnumerable<string> links)
        {
            var downloadFailUrls = links.ToList();
            if (downloadFailUrls.Count() <= 0)
            {
                return;
            }

            downloadFailUrls.Insert(0, "");
            setting.SendLog(
                string.Join(Environment.NewLine + I18N.DownloadFail + @" ", downloadFailUrls)
            );
        }

        private List<Models.Datas.SubscriptionItem> GetSubsIsInUse()
        {
            var subs = new List<Models.Datas.SubscriptionItem>();
            var urlCache = new List<string>();
            var subsUi = GetAllSubsUi();

            foreach (var subUi in subsUi)
            {
                var subItem = subUi.GetValue();
                if (!subItem.isUse || urlCache.Contains(subItem.url))
                {
                    continue;
                }

                urlCache.Add(subItem.url);
                subs.Add(subItem);
            }

            return subs;
        }

        void BindEventFlyPanelDragDrop()
        {
            this.flyPanel.DragDrop += (s, a) =>
            {
                // https://www.codeproject.com/Articles/48411/Using-the-FlowLayoutPanel-and-Reordering-with-Drag

                var subject =
                    a.Data.GetData(typeof(Views.UserControls.SubscriptionUI))
                    as Views.UserControls.SubscriptionUI;

                var container = s as FlowLayoutPanel;
                Point p = container.PointToClient(new Point(a.X, a.Y));
                var dest = container.GetChildAtPoint(p);
                int idxDest = container.Controls.GetChildIndex(dest, false);
                container.Controls.SetChildIndex(subject, idxDest);
                container.Invalidate();
            };
        }

        void BindEvent()
        {
            BindEventBtnAddClick();
            BindEventBtnUpdateClick();
            BindEventBtnSelections();
            BindEventFlyPanelDragDrop();

            //servers.OnServerCountChange += UpdateServUiTotal;
            //servers.OnServerPropertyChange += UpdateServUiTotal;

            chkSubsIsAutoPatch.CheckedChanged += (s, a) =>
                setting.isAutoPatchSubsInfo = chkSubsIsAutoPatch.Checked;

            this.flyPanel.DragEnter += (s, a) =>
            {
                a.Effect = DragDropEffects.Move;
            };
        }

        void ReleaseEvent()
        {
            //servers.OnServerCountChange -= UpdateServUiTotal;
            //servers.OnServerPropertyChange -= UpdateServUiTotal;
        }

        void UpdatePanelItemsIndex()
        {
            var index = 1;
            var subsUi = GetAllSubsUi();
            foreach (var item in subsUi)
            {
                item.SetIndex(index++);
            }
            MarkDuplicatedSubsInfo();
        }

        void GetAvailableProxyInfo(out bool isSocks5, out int port)
        {
            isSocks5 = false;
            port = -1;

            if (!chkSubsIsUseProxy.Checked || servers.GetAvailableProxyInfo(out isSocks5, out port))
            {
                return;
            }

            VgcApis.Misc.UI.MsgBoxAsync(I18N.NoQualifyProxyServer);
        }
        #endregion
    }
}
