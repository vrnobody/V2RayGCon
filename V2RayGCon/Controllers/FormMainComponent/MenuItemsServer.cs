using System;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using V2RayGCon.Resources.Resx;

namespace V2RayGCon.Controllers.FormMainComponent
{
    class MenuItemsServer : FormMainComponentController
    {
        Services.Cache cache;
        Services.Settings settings;
        Services.Servers servers;
        Services.ShareLinkMgr slinkMgr;

        public MenuItemsServer(

            // misc
            ToolStripMenuItem refreshSummary,
            ToolStripMenuItem deleteAllServers,
            ToolStripMenuItem deleteSelected,

            // copy
            ToolStripMenuItem copyAsV2cfgLinks,
            ToolStripMenuItem copyAsVmixLinks,
            ToolStripMenuItem copyAsVeeLinks,
            ToolStripMenuItem copyAsVmessSubscriptions,
            ToolStripMenuItem copyAsVeeSubscriptions,

            // batch op
            ToolStripMenuItem stopBatchSpeedtest,
            ToolStripMenuItem runBatchSpeedtest,
            ToolStripMenuItem clearSpeedtestResults,
            ToolStripMenuItem clearStatisticRecord,

            ToolStripMenuItem modifySelected,
            ToolStripMenuItem stopSelected,
            ToolStripMenuItem restartSelected,

            // view
            ToolStripMenuItem moveToTop,
            ToolStripMenuItem moveToBottom,

            ToolStripMenuItem reverseByIndex,
            ToolStripMenuItem sortBySpeed,
            ToolStripMenuItem sortByDate,
            ToolStripMenuItem sortBySummary,
          ToolStripMenuItem sortByDownloadTotal,
             ToolStripMenuItem sortByUploadTotal)
        {
            cache = Services.Cache.Instance;
            servers = Services.Servers.Instance;
            slinkMgr = Services.ShareLinkMgr.Instance;
            settings = Services.Settings.Instance;

            InitCtrlSorting(
                reverseByIndex,
                sortBySpeed, sortByDate, sortBySummary,
                sortByDownloadTotal, sortByUploadTotal);

            InitCtrlView(moveToTop, moveToBottom);

            InitCtrlCopyToClipboard(
                copyAsV2cfgLinks,
                copyAsVmixLinks,
                copyAsVeeLinks,
                copyAsVmessSubscriptions,
                copyAsVeeSubscriptions);

            InitCtrlMisc(
                refreshSummary,
                deleteSelected,
                deleteAllServers);

            InitCtrlBatchOperation(
                stopSelected,
                restartSelected,

                runBatchSpeedtest,
                stopBatchSpeedtest,
                clearSpeedtestResults,
                clearStatisticRecord,

                modifySelected);
        }

        #region public method
        public override void Cleanup()
        {
        }
        #endregion

        #region private method
        void ClearSelectedServersStatRecord()
        {
            var servs = servers
               .GetAllServersOrderByIndex()
               .Where(s => s.GetCoreStates().IsSelected())
               .ToList();

            foreach (var serv in servs)
            {
                var cst = serv.GetCoreStates();
                cst.SetDownlinkTotal(0);
                cst.SetUplinkTotal(0);
            }
        }
        void ClearSelectedServersSpeedTestResults()
        {
            var servs = servers
                .GetAllServersOrderByIndex()
                .Where(s => s.GetCoreStates().IsSelected())
                .ToList();

            foreach (var serv in servs)
            {
                var cst = serv.GetCoreStates();
                cst.SetSpeedTestResult(0);
            }
        }

        EventHandler RunWhenSelectionIsNotEmptyHandler(Action action)
        {
            return (s, a) =>
            {
                if (!servers.IsSelecteAnyServer())
                {
                    VgcApis.Misc.UI.MsgBoxAsync(I18N.SelectServerFirst);
                    return;
                }
                action();
            };
        }

        private void InitCtrlBatchOperation(
            ToolStripMenuItem stopSelected,
            ToolStripMenuItem restartSelected,

            ToolStripMenuItem runBatchSpeedtest,
            ToolStripMenuItem stopBatchSpeedtest,
            ToolStripMenuItem clearSpeedtestResults,
            ToolStripMenuItem clearStatisticsRecord,

            ToolStripMenuItem modifySelected)
        {
            clearStatisticsRecord.Click += RunWhenSelectionIsNotEmptyHandler(() =>
            {
                if (Misc.UI.Confirm(I18N.ConfirmClearStat))
                {
                    ClearSelectedServersStatRecord();
                }
            });

            clearSpeedtestResults.Click += RunWhenSelectionIsNotEmptyHandler(() =>
            {
                if (Misc.UI.Confirm(I18N.ConfirmClearSpeedTestResults))
                {
                    ClearSelectedServersSpeedTestResults();
                }
            });

            modifySelected.Click += RunWhenSelectionIsNotEmptyHandler(
                () => Views.WinForms.FormBatchModifyServerSetting.GetForm());

            runBatchSpeedtest.Click += RunWhenSelectionIsNotEmptyHandler(() =>
            {
                if (!Misc.UI.Confirm(I18N.TestWillTakeALongTime))
                {
                    return;
                }

                servers.RunSpeedTestOnSelectedServersBg();
            });

            stopBatchSpeedtest.Click += (s, a) =>
            {
                settings.SendLog(I18N.StoppingSpeedtest);
                settings.isSpeedtestCancelled = true;
            };

            stopSelected.Click += RunWhenSelectionIsNotEmptyHandler(() =>
            {
                if (Misc.UI.Confirm(I18N.ConfirmStopAllSelectedServers))
                {
                    servers.StopSelectedServersThen();
                }
            });

            restartSelected.Click += RunWhenSelectionIsNotEmptyHandler(() =>
            {
                if (Misc.UI.Confirm(I18N.ConfirmRestartAllSelectedServers))
                {
                    servers.RestartSelectedServersThen();
                }
            });
        }

        private void InitCtrlMisc(
            ToolStripMenuItem refreshSummary,
            ToolStripMenuItem deleteSelected,
            ToolStripMenuItem deleteAllItems)
        {
            refreshSummary.Click += (s, a) =>
            {
                cache.html.Clear();
                VgcApis.Misc.Utils.RunInBackground(
                    servers.UpdateAllServersSummary);
            };

            deleteAllItems.Click += (s, a) =>
            {
                if (!Misc.UI.Confirm(I18N.ConfirmDeleteAllServers))
                {
                    return;
                }
                Services.Servers.Instance.DeleteAllServersThen();
                Services.Cache.Instance.core.Clear();
            };

            deleteSelected.Click += RunWhenSelectionIsNotEmptyHandler(() =>
            {
                if (!Misc.UI.Confirm(I18N.ConfirmDeleteSelectedServers))
                {
                    return;
                }
                servers.DeleteSelectedServersThen();
            });
        }

        private void InitCtrlCopyToClipboard(
            ToolStripMenuItem copyAsV2cfgLinks,
            ToolStripMenuItem copyAsVmixLinks,
            ToolStripMenuItem copyAsVeeLinks,
            ToolStripMenuItem copyAsVmessSubscriptions,
            ToolStripMenuItem copyAsVeeSubscriptions)
        {
            copyAsVeeSubscriptions.Click += RunWhenSelectionIsNotEmptyHandler(() =>
            {
                var links = EncodeSelectedServersIntoShareLinks(
                    VgcApis.Models.Datas.Enums.LinkTypes.v);
                var b64Links = Misc.Utils.Base64Encode(links);
                Misc.Utils.CopyToClipboardAndPrompt(b64Links);
            });

            copyAsVmessSubscriptions.Click += RunWhenSelectionIsNotEmptyHandler(() =>
            {
                var links = EncodeSelectedServersIntoShareLinks(
                    VgcApis.Models.Datas.Enums.LinkTypes.vmess);
                var b64Links = Misc.Utils.Base64Encode(links);
                Misc.Utils.CopyToClipboardAndPrompt(b64Links);
            });

            copyAsV2cfgLinks.Click += RunWhenSelectionIsNotEmptyHandler(() =>
            {
                var links = EncodeSelectedServersIntoShareLinks(
                    VgcApis.Models.Datas.Enums.LinkTypes.v2cfg);
                Misc.Utils.CopyToClipboardAndPrompt(links);
            });

            copyAsVmixLinks.Click += RunWhenSelectionIsNotEmptyHandler(() =>
            {
                var vmesses = EncodeSelectedServersIntoShareLinks(
                    VgcApis.Models.Datas.Enums.LinkTypes.vmess);
                var vlesses = EncodeSelectedServersIntoShareLinks(
                    VgcApis.Models.Datas.Enums.LinkTypes.vless);
                Misc.Utils.CopyToClipboardAndPrompt(
                    vlesses + Environment.NewLine + vmesses);
            });

            copyAsVeeLinks.Click += RunWhenSelectionIsNotEmptyHandler(() =>
            {
                var links = EncodeSelectedServersIntoShareLinks(
                    VgcApis.Models.Datas.Enums.LinkTypes.v);
                Misc.Utils.CopyToClipboardAndPrompt(links);
            });
        }

        private void InitCtrlView(
            ToolStripMenuItem moveToTop,
            ToolStripMenuItem moveToBottom)
        {
            moveToTop.Click += RunWhenSelectionIsNotEmptyHandler(() =>
            {
                SetServerItemsIndex(0);
            });

            moveToBottom.Click += RunWhenSelectionIsNotEmptyHandler(() =>
            {
                SetServerItemsIndex(double.MaxValue);
            });
        }

        private void InitCtrlSorting(
            ToolStripMenuItem reverseByIndex,
            ToolStripMenuItem sortBySpeed,
            ToolStripMenuItem sortByDate,
            ToolStripMenuItem sortBySummary,
             ToolStripMenuItem sortByDownloadTotal,
             ToolStripMenuItem sortByUploadTotal)
        {
            sortByDownloadTotal.Click += RunWhenSelectionIsNotEmptyHandler(
               () => servers.SortSelectedByDownloadTotal());

            sortByUploadTotal.Click += RunWhenSelectionIsNotEmptyHandler(
               () => servers.SortSelectedByUploadTotal());

            reverseByIndex.Click += RunWhenSelectionIsNotEmptyHandler(
                () => servers.ReverseSelectedByIndex());

            sortByDate.Click += RunWhenSelectionIsNotEmptyHandler(
                () => servers.SortSelectedByLastModifiedDate());

            sortBySummary.Click += RunWhenSelectionIsNotEmptyHandler(
                () => servers.SortSelectedBySummary());

            sortBySpeed.Click += RunWhenSelectionIsNotEmptyHandler(
                () => servers.SortSelectedBySpeedTest());
        }

        void RemoveAllControlsAndRefreshFlyPanel()
        {
            var panel = GetFlyPanel();
            panel.RemoveAllServersConrol();
            panel.RefreshFlyPanelLater();
        }

        void SetServerItemsIndex(double index)
        {
            servers.GetAllServersOrderByIndex()
                .Where(s => s.GetCoreStates().IsSelected())
                .Select(s =>
                {
                    s.GetCoreStates().SetIndex(index);
                    return true;
                })
                .ToList(); // force linq to execute

            RemoveAllControlsAndRefreshFlyPanel();
        }

        string EncodeSelectedServersIntoShareLinks(
            VgcApis.Models.Datas.Enums.LinkTypes linkType)
        {
            var serverList = servers.GetAllServersOrderByIndex();

            StringBuilder result = new StringBuilder("");

            foreach (var server in serverList)
            {
                if (!server.GetCoreStates().IsSelected())
                {
                    continue;
                }

                var configString = server.GetConfiger().GetConfig();
                var shareLink = slinkMgr.EncodeConfigToShareLink(
                    configString, linkType);

                if (!string.IsNullOrEmpty(shareLink))
                {
                    result
                        .Append(shareLink)
                        .Append(Environment.NewLine);
                }
            }

            return result.ToString();
        }

        FlyServer GetFlyPanel()
        {
            return this.GetContainer()
                .GetComponent<FlyServer>();
        }
        #endregion
    }
}
