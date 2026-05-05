using System;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using V2RayGCon.Resources.Resx;

namespace V2RayGCon.Controllers.FormMainComponent
{
    class MenuItemsServer : FormMainComponentController
    {
        readonly Services.Settings settings;
        readonly Services.Servers servers;
        readonly Services.ShareLinkMgr slinkMgr;

        public MenuItemsServer(
            // misc
            ToolStripMenuItem refreshSummary,
            ToolStripMenuItem deleteAllServers,
            ToolStripMenuItem deleteSelected,
            // copy
            ToolStripMenuItem copyAsV2cfgLinks,
            ToolStripMenuItem copyAsVmixLinks,
            ToolStripMenuItem copyAsMobLinks,
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
            ToolStripMenuItem moveToCustomIndex,
            ToolStripMenuItem reverseByIndex,
            ToolStripMenuItem sortBySpeed,
            ToolStripMenuItem sortByDate,
            ToolStripMenuItem sortBySummary,
            ToolStripMenuItem sortByDownloadTotal,
            ToolStripMenuItem sortByUploadTotal
        )
        {
            servers = Services.Servers.Instance;
            slinkMgr = Services.ShareLinkMgr.Instance;
            settings = Services.Settings.Instance;

            InitCtrlSorting(
                reverseByIndex,
                sortBySpeed,
                sortByDate,
                sortBySummary,
                sortByDownloadTotal,
                sortByUploadTotal
            );

            InitCtrlView(moveToTop, moveToBottom, moveToCustomIndex);

            InitCtrlCopyToClipboard(copyAsV2cfgLinks, copyAsVmixLinks, copyAsMobLinks);

            InitCtrlMisc(refreshSummary, deleteSelected, deleteAllServers);

            InitCtrlBatchOperation(
                stopSelected,
                restartSelected,
                runBatchSpeedtest,
                stopBatchSpeedtest,
                clearSpeedtestResults,
                clearStatisticRecord,
                modifySelected
            );
        }

        #region public method
        public override void Cleanup() { }
        #endregion

        #region private method
        void ClearSelectedServersStatRecordBg()
        {
            VgcApis.Misc.Utils.RunInBackground(() =>
            {
                var servs = servers.GetSelectedServers();

                foreach (var serv in servs)
                {
                    var cst = serv.GetCoreStates();
                    cst.SetDownlinkTotal(0);
                    cst.SetUplinkTotal(0);
                }
            });
        }

        void ClearSelectedServersSpeedTestResultsBg()
        {
            VgcApis.Misc.Utils.RunInBackground(() =>
            {
                var servs = servers.GetSelectedServers();

                foreach (var serv in servs)
                {
                    var cst = serv.GetCoreStates();
                    cst.SetSpeedTestResult(0);
                }
            });
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
            ToolStripMenuItem modifySelected
        )
        {
            clearStatisticsRecord.Click += RunWhenSelectionIsNotEmptyHandler(() =>
            {
                if (VgcApis.Misc.UI.Confirm(I18N.ConfirmClearStat))
                {
                    ClearSelectedServersStatRecordBg();
                }
            });

            clearSpeedtestResults.Click += RunWhenSelectionIsNotEmptyHandler(() =>
            {
                if (VgcApis.Misc.UI.Confirm(I18N.ConfirmClearSpeedTestResults))
                {
                    ClearSelectedServersSpeedTestResultsBg();
                }
            });

            modifySelected.Click += RunWhenSelectionIsNotEmptyHandler(() =>
                Views.WinForms.FormBatchModifyServerSetting.GetForm()
            );

            runBatchSpeedtest.Click += RunWhenSelectionIsNotEmptyHandler(() =>
            {
                if (!VgcApis.Misc.UI.Confirm(I18N.TestWillTakeALongTime))
                {
                    return;
                }

                servers.RunSpeedTestOnSelectedServersBg();
            });

            stopBatchSpeedtest.Click += (s, a) =>
            {
                VgcApis.Misc.Logger.Log(I18N.StoppingSpeedtest);
                settings.isSpeedtestCancelled = true;
            };

            stopSelected.Click += RunWhenSelectionIsNotEmptyHandler(() =>
            {
                if (VgcApis.Misc.UI.Confirm(I18N.ConfirmStopAllSelectedServers))
                {
                    servers.StopSelectedServersThen();
                }
            });

            restartSelected.Click += RunWhenSelectionIsNotEmptyHandler(() =>
            {
                if (VgcApis.Misc.UI.Confirm(I18N.ConfirmRestartAllSelectedServers))
                {
                    servers.RestartSelectedServersThen();
                }
            });
        }

        private void InitCtrlMisc(
            ToolStripMenuItem refreshSummary,
            ToolStripMenuItem deleteSelected,
            ToolStripMenuItem deleteAllItems
        )
        {
            refreshSummary.Click += (s, a) =>
            {
                VgcApis.Misc.Utils.RunInBackground(servers.UpdateAllServersSummary);
            };

            deleteAllItems.Click += (s, a) =>
            {
                if (!VgcApis.Misc.UI.Confirm(I18N.ConfirmDeleteAllServers))
                {
                    return;
                }
                VgcApis.Misc.Utils.RunInBackground(() =>
                {
                    Services.Servers.Instance.DeleteAllServers();
                });
            };

            deleteSelected.Click += RunWhenSelectionIsNotEmptyHandler(() =>
            {
                if (!VgcApis.Misc.UI.Confirm(I18N.ConfirmDeleteSelectedServers))
                {
                    return;
                }
                VgcApis.Misc.Utils.RunInBackground(() =>
                {
                    servers.DeleteSelectedServers();
                });
            });
        }

        private void InitCtrlCopyToClipboard(
            ToolStripMenuItem copyAsV2cfgLinks,
            ToolStripMenuItem copyAsVmixLinks,
            ToolStripMenuItem copyAsMobLinks
        )
        {
            copyAsMobLinks.Click += RunWhenSelectionIsNotEmptyHandler(() =>
            {
                CopySelectedAsShareLinkBg(VgcApis.Models.Datas.Enums.LinkTypes.mob);
            });

            copyAsV2cfgLinks.Click += RunWhenSelectionIsNotEmptyHandler(() =>
            {
                CopySelectedAsShareLinkBg(VgcApis.Models.Datas.Enums.LinkTypes.v2cfg);
            });

            copyAsVmixLinks.Click += RunWhenSelectionIsNotEmptyHandler(() =>
            {
                CopySelectedAsVmixShareLinksBg();
            });
        }

        private void InitCtrlView(
            ToolStripMenuItem moveToTop,
            ToolStripMenuItem moveToBottom,
            ToolStripMenuItem moveToCustomIndex
        )
        {
            moveToTop.Click += RunWhenSelectionIsNotEmptyHandler(() =>
            {
                VgcApis.Misc.Utils.RunInBackground(() =>
                {
                    var selected = servers.GetSelectedServers();
                    var uids = selected.Select(s => s.GetCoreStates().GetUid()).ToList();
                    servers.MoveTo(uids, 1);
                });
            });

            moveToBottom.Click += RunWhenSelectionIsNotEmptyHandler(() =>
            {
                VgcApis.Misc.Utils.RunInBackground(() =>
                {
                    var selected = servers.GetSelectedServers();
                    var uids = selected.Select(s => s.GetCoreStates().GetUid()).ToList();
                    servers.MoveTo(uids, servers.Count() + 1);
                });
            });

            moveToCustomIndex.Click += RunWhenSelectionIsNotEmptyHandler(() =>
            {
                VgcApis.Misc.UI.GetUserInput(
                    I18N.DestIndex,
                    str =>
                    {
                        if (double.TryParse(str, out var index))
                        {
                            VgcApis.Misc.Utils.RunInBackground(() =>
                            {
                                var selected = servers.GetSelectedServers();
                                var uids = selected
                                    .Select(s => s.GetCoreStates().GetUid())
                                    .ToList();
                                servers.MoveTo(uids, index);
                            });
                        }
                        else
                        {
                            VgcApis.Misc.UI.MsgBox(I18N.ParseNumberFailed);
                        }
                    }
                );
            });
        }

        private void InitCtrlSorting(
            ToolStripMenuItem reverseByIndex,
            ToolStripMenuItem sortBySpeed,
            ToolStripMenuItem sortByDate,
            ToolStripMenuItem sortBySummary,
            ToolStripMenuItem sortByDownloadTotal,
            ToolStripMenuItem sortByUploadTotal
        )
        {
            sortByDownloadTotal.Click += RunWhenSelectionIsNotEmptyHandler(() =>
                VgcApis.Misc.Utils.RunInBackground(() => servers.SortSelectedByDownloadTotal())
            );

            sortByUploadTotal.Click += RunWhenSelectionIsNotEmptyHandler(() =>
                VgcApis.Misc.Utils.RunInBackground(() => servers.SortSelectedByUploadTotal())
            );

            reverseByIndex.Click += RunWhenSelectionIsNotEmptyHandler(() =>
                VgcApis.Misc.Utils.RunInBackground(() => servers.ReverseSelectedByIndex())
            );

            sortByDate.Click += RunWhenSelectionIsNotEmptyHandler(() =>
                VgcApis.Misc.Utils.RunInBackground(() => servers.SortSelectedByLastModifiedDate())
            );

            sortBySummary.Click += RunWhenSelectionIsNotEmptyHandler(() =>
                VgcApis.Misc.Utils.RunInBackground(() => servers.SortSelectedBySummary())
            );

            sortBySpeed.Click += RunWhenSelectionIsNotEmptyHandler(() =>
                VgcApis.Misc.Utils.RunInBackground(() => servers.SortSelectedBySpeedTest())
            );
        }

        void CopySelectedAsVmixShareLinksBg()
        {
            VgcApis.Misc.Utils.RunInBackground(() =>
            {
                var serverList = servers.GetAllServersOrderByIndex();

                StringBuilder result = new StringBuilder("");

                foreach (var server in serverList)
                {
                    if (!server.GetCoreStates().IsSelected())
                    {
                        continue;
                    }

                    var shareLink = server.GetConfiger().GetShareLink();

                    if (!string.IsNullOrEmpty(shareLink))
                    {
                        result.Append(shareLink).Append(Environment.NewLine);
                    }
                }

                var links = result.ToString();
                VgcApis.Misc.UI.Invoke(() => Misc.Utils.CopyToClipboardAndPrompt(links));
            });
        }

        void CopySelectedAsShareLinkBg(VgcApis.Models.Datas.Enums.LinkTypes linkType)
        {
            VgcApis.Misc.Utils.RunInBackground(() =>
            {
                var serverList = servers.GetAllServersOrderByIndex();

                StringBuilder result = new StringBuilder("");

                foreach (var server in serverList)
                {
                    if (!server.GetCoreStates().IsSelected())
                    {
                        continue;
                    }

                    var name = server.GetCoreStates().GetName();
                    var configString = server.GetConfiger().GetConfig();
                    var shareLink = slinkMgr.EncodeConfigToShareLink(name, configString, linkType);

                    if (!string.IsNullOrEmpty(shareLink))
                    {
                        result.Append(shareLink).Append(Environment.NewLine);
                    }
                }

                var links = result.ToString();
                VgcApis.Misc.UI.Invoke(() => Misc.Utils.CopyToClipboardAndPrompt(links));
            });
        }
        #endregion
    }
}
