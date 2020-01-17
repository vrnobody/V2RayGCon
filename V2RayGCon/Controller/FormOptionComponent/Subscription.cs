using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using V2RayGCon.Resource.Resx;

namespace V2RayGCon.Controller.OptionComponent
{
    public class Subscription : OptionComponentController
    {
        readonly FlowLayoutPanel flyPanel;
        readonly Button btnAdd, btnUpdate, btnUseAll, btnInvertSelection;
        readonly CheckBox chkSubsIsUseProxy;

        readonly Service.Setting setting;
        readonly Service.Servers servers;
        readonly Service.ShareLinkMgr slinkMgr;

        string oldOptions;

        public Subscription(
            FlowLayoutPanel flyPanel,
            Button btnAdd,
            Button btnUpdate,
            CheckBox chkSubsIsUseProxy,
            Button btnUseAll,
            Button btnInvertSelection)
        {
            this.setting = Service.Setting.Instance;
            this.servers = Service.Servers.Instance;
            this.slinkMgr = Service.ShareLinkMgr.Instance;

            this.flyPanel = flyPanel;
            this.btnAdd = btnAdd;
            this.btnUpdate = btnUpdate;
            this.chkSubsIsUseProxy = chkSubsIsUseProxy;
            this.btnUseAll = btnUseAll;
            this.btnInvertSelection = btnInvertSelection;

            chkSubsIsUseProxy.Checked = setting.isUpdateUseProxy;

            InitPanel();
            BindEvent();

            MarkDuplicatedSubsInfo();
        }

        #region public method
        public override bool SaveOptions()
        {
            string curOptions = GetCurOptions();

            if (curOptions != oldOptions)
            {
                setting.SaveSubscriptionItems(curOptions);
                oldOptions = curOptions;
                return true;
            }
            return false;
        }

        public override bool IsOptionsChanged()
        {
            return GetCurOptions() != oldOptions;
        }

        public void Merge(string rawSetting)
        {
            var mergedSettings = MergeIntoCurSubsItems(rawSetting);
            setting.SaveSubscriptionItems(mergedSettings);
            Lib.UI.ClearFlowLayoutPanel(this.flyPanel);
            InitPanel();
        }

        public void MarkDuplicatedSubsInfo()
        {
            VgcApis.Libs.UI.RunInUiThreadIgnoreError(flyPanel, MarkDuplicatedSubsInfoWorker);
        }
        #endregion

        #region private method


        void MarkDuplicatedSubsInfoWorker()
        {
            var subsUis = flyPanel.Controls.OfType<Views.UserControls.SubscriptionUI>().ToList();
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
            var subs = new List<Model.Data.SubscriptionItem>();
            try
            {
                var items = JsonConvert.DeserializeObject<List<Model.Data.SubscriptionItem>>(rawSubsSetting);
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
            return JsonConvert.SerializeObject(
                CollectSubscriptionItems());
        }

        List<Model.Data.SubscriptionItem> CollectSubscriptionItems()
        {
            var itemList = new List<Model.Data.SubscriptionItem>();
            var urlCache = new List<string>();
            foreach (Views.UserControls.SubscriptionUI item in this.flyPanel.Controls)
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

            this.oldOptions = JsonConvert.SerializeObject(subItemList);

            if (subItemList.Count <= 0)
            {
                subItemList.Add(new Model.Data.SubscriptionItem());
            }

            foreach (var item in subItemList)
            {
                AddSubsUiItem(item);
            }

            UpdatePanelItemsIndex();
        }

        void AddSubsUiItem(Model.Data.SubscriptionItem data)
        {
            var subsUi = new Views.UserControls.SubscriptionUI(this, data);
            subsUi.OnDelete += UpdatePanelItemsIndex;
            flyPanel.Controls.Add(subsUi);
        }

        void BindEventBtnAddClick()
        {
            this.btnAdd.Click += (s, a) =>
            {
                AddSubsUiItem(new Model.Data.SubscriptionItem());
                UpdatePanelItemsIndex();
            };
        }

        void BindEventBtnSelections()
        {
            this.btnUseAll.Click += (s, a) =>
            {
                foreach (Views.UserControls.SubscriptionUI subUi in this.flyPanel.Controls)
                {
                    subUi.SetIsUse(true);
                }
            };

            this.btnInvertSelection.Click += (s, a) =>
            {
                foreach (Views.UserControls.SubscriptionUI subUi in this.flyPanel.Controls)
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
                    MessageBox.Show(I18N.NoSubsUrlAvailable);
                    return;
                }

                VgcApis.Libs.Utils.RunInBackground(() =>
                {
                    var links = Lib.Utils.FetchLinksFromSubcriptions(
                        subs, GetAvailableHttpProxyPort());

                    LogDownloadFails(links
                        .Where(l => string.IsNullOrEmpty(l[0]))
                        .Select(l => l[1]));

                    slinkMgr.ImportLinkWithOutV2cfgLinksBatchMode(
                        links.Where(l => !string.IsNullOrEmpty(l[0])).ToList());

                    EnableBtnUpdate();
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
            setting.SendLog(string.Join(
                Environment.NewLine + I18N.DownloadFail + @" ",
                downloadFailUrls));
        }

        private List<Model.Data.SubscriptionItem> GetSubsIsInUse()
        {
            var subs = new List<Model.Data.SubscriptionItem>();
            var urlCache = new List<string>();

            foreach (Views.UserControls.SubscriptionUI subUi in this.flyPanel.Controls)
            {
                var subItem = subUi.GetValue();
                if (!subItem.isUse
                    || urlCache.Contains(subItem.url))
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

                var data = a.Data.GetData(typeof(Views.UserControls.SubscriptionUI))
                    as Views.UserControls.SubscriptionUI;

                var dest = s as FlowLayoutPanel;
                Point p = dest.PointToClient(new Point(a.X, a.Y));
                var item = dest.GetChildAtPoint(p);
                int index = dest.Controls.GetChildIndex(item, false);
                dest.Controls.SetChildIndex(data, index);
                dest.Invalidate();
            };
        }

        void BindEvent()
        {
            BindEventBtnAddClick();
            BindEventBtnUpdateClick();
            BindEventBtnSelections();
            BindEventFlyPanelDragDrop();

            this.flyPanel.DragEnter += (s, a) =>
            {
                a.Effect = DragDropEffects.Move;
            };
        }

        void UpdatePanelItemsIndex()
        {
            var index = 1;
            foreach (Views.UserControls.SubscriptionUI item in this.flyPanel.Controls)
            {
                item.SetIndex(index++);
            }
            MarkDuplicatedSubsInfo();
        }

        int GetAvailableHttpProxyPort()
        {
            if (!chkSubsIsUseProxy.Checked)
            {
                return -1;
            }

            var port = servers.GetAvailableHttpProxyPort();
            if (port > 0)
            {
                return port;
            }

            VgcApis.Libs.Utils.RunInBackground(
                () => MessageBox.Show(
                    I18N.NoQualifyProxyServer));

            return -1;
        }

        private void EnableBtnUpdate()
        {
            try
            {
                VgcApis.Libs.UI.RunInUiThread(btnUpdate, () =>
                {
                    this.btnUpdate.Enabled = true;
                });
            }
            catch { }
        }
        #endregion
    }
}
