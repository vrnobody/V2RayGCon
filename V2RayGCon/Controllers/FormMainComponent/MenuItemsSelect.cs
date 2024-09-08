using System;
using System.Windows.Forms;
using VgcApis.Interfaces;

namespace V2RayGCon.Controllers.FormMainComponent
{
    class MenuItemsSelect : FormMainComponentController
    {
        static readonly long SpeedtestTimeout = VgcApis.Models.Consts.Core.SpeedtestTimeout;
        readonly Services.Servers servers;

        public MenuItemsSelect(
            // current page
            ToolStripMenuItem selectAllCurPage,
            ToolStripMenuItem invertSelectionCurPage,
            ToolStripMenuItem selectNoneCurPage,
            ToolStripMenuItem selectAutorunCurPage,
            ToolStripMenuItem selectRunningCurPage,
            ToolStripMenuItem selectTimeoutCurPage,
            ToolStripMenuItem selectNoSpeedTestCurPage,
            ToolStripMenuItem selectNoMarkCurPage,
            ToolStripMenuItem selectUntrackCurPage,
            //all pages
            ToolStripMenuItem selectAllAllPages,
            ToolStripMenuItem invertSelectionAllPages,
            ToolStripMenuItem selectNoneAllPages,
            ToolStripMenuItem selectAutorunAllPages,
            ToolStripMenuItem selectNoMarkAllPages,
            ToolStripMenuItem selectNoSpeedTestAllPages,
            ToolStripMenuItem selectRunningAllPages,
            ToolStripMenuItem selectTimeoutAllPages,
            ToolStripMenuItem selectUntrackAllPages,
            // all servers
            ToolStripMenuItem selectAllAllServers,
            ToolStripMenuItem invertSelectionAllServers,
            ToolStripMenuItem selectNoneAllServers,
            ToolStripMenuItem selectAutorunAllServers,
            ToolStripMenuItem selectNoMarkAllServers,
            ToolStripMenuItem selectNoSpeedTestAllServers,
            ToolStripMenuItem selectRunningAllServers,
            ToolStripMenuItem selectTimeoutAllServers,
            ToolStripMenuItem selectUntrackAllServers
        )
        {
            servers = Services.Servers.Instance;

            InitAllPagesSelectors(
                selectNoneAllPages,
                invertSelectionAllPages,
                selectAllAllPages,
                selectAutorunAllPages,
                selectRunningAllPages,
                selectTimeoutAllPages,
                selectNoSpeedTestAllPages,
                selectNoMarkAllPages,
                selectUntrackAllPages
            );
            InitAllServersSelector(
                selectNoneAllServers,
                invertSelectionAllServers,
                selectAllAllServers,
                selectAutorunAllServers,
                selectRunningAllServers,
                selectTimeoutAllServers,
                selectNoSpeedTestAllServers,
                selectNoMarkAllServers,
                selectUntrackAllServers
            );
            InitCurPageSelectors(
                selectAllCurPage,
                selectNoneCurPage,
                invertSelectionCurPage,
                selectAutorunCurPage,
                selectRunningCurPage,
                selectTimeoutCurPage,
                selectNoSpeedTestCurPage,
                selectNoMarkCurPage,
                selectUntrackCurPage
            );
        }

        #region public method

        public override void Cleanup() { }
        #endregion

        #region private method
        private void InitAllPagesSelectors(
            ToolStripMenuItem selectNoneAllPages,
            ToolStripMenuItem invertSelectionAllPages,
            ToolStripMenuItem selectAllAllPages,
            ToolStripMenuItem selectAutorunAllPages,
            ToolStripMenuItem selectRunningAllPages,
            ToolStripMenuItem selectTimeoutAllPages,
            ToolStripMenuItem selectNoSpeedTestAllPages,
            ToolStripMenuItem selectNoMarkAllPages,
            ToolStripMenuItem selectUntrackAllPages
        )
        {
            selectAllAllPages.Click += (s, a) => SelectAllPagesWhere(el => true);

            selectNoneAllPages.Click += (s, a) => SelectAllPagesWhere(el => false);

            invertSelectionAllPages.Click += (s, a) =>
                SelectAllPagesWhere(el => !el.GetCoreStates().IsSelected());

            selectAutorunAllPages.Click += (s, a) =>
                SelectAllPagesWhere(el => el.GetCoreStates().IsAutoRun());

            selectRunningAllPages.Click += (s, a) =>
                SelectAllPagesWhere(el => el.GetCoreCtrl().IsCoreRunning());

            selectTimeoutAllPages.Click += (s, a) =>
                SelectAllPagesWhere(el =>
                    el.GetCoreStates().GetSpeedTestResult() == SpeedtestTimeout
                );

            selectNoSpeedTestAllPages.Click += (s, a) =>
                SelectAllPagesWhere(el => el.GetCoreStates().GetSpeedTestResult() <= 0);

            selectNoMarkAllPages.Click += (s, a) =>
                SelectAllPagesWhere(el => string.IsNullOrEmpty(el.GetCoreStates().GetMark()));

            selectUntrackAllPages.Click += (s, a) =>
                SelectAllPagesWhere(el => el.GetCoreStates().IsUntrack());
        }

        private void InitAllServersSelector(
            ToolStripMenuItem selectNoneAllServers,
            ToolStripMenuItem invertSelectionAllServers,
            ToolStripMenuItem selectAllAllServers,
            ToolStripMenuItem selectAutorunAllServers,
            ToolStripMenuItem selectRunningAllServers,
            ToolStripMenuItem selectTimeoutAllServers,
            ToolStripMenuItem selectNoSpeedTestAllServers,
            ToolStripMenuItem selectNoMarkAllServers,
            ToolStripMenuItem selectUntrackAllServers
        )
        {
            selectAllAllServers.Click += (s, a) => SelectAllServersWhere(el => true);

            invertSelectionAllServers.Click += (s, a) =>
                SelectAllServersWhere(el => !el.GetCoreStates().IsSelected());

            selectNoneAllServers.Click += (s, a) => SelectAllServersWhere(el => false);

            selectNoMarkAllServers.Click += (s, a) =>
                SelectAllServersWhere(el => string.IsNullOrEmpty(el.GetCoreStates().GetMark()));

            selectNoSpeedTestAllServers.Click += (s, a) =>
                SelectAllServersWhere(el => el.GetCoreStates().GetSpeedTestResult() <= 0);

            selectTimeoutAllServers.Click += (s, a) =>
                SelectAllServersWhere(el =>
                    el.GetCoreStates().GetSpeedTestResult() == SpeedtestTimeout
                );

            selectRunningAllServers.Click += (s, a) =>
                SelectAllServersWhere(el => el.GetCoreCtrl().IsCoreRunning());

            selectAutorunAllServers.Click += (s, a) =>
                SelectAllServersWhere(el => el.GetCoreStates().IsAutoRun());

            selectUntrackAllServers.Click += (s, a) =>
                SelectAllServersWhere(el => el.GetCoreStates().IsUntrack());
        }

        private void InitCurPageSelectors(
            ToolStripMenuItem selectAllCurPage,
            ToolStripMenuItem selectNoneCurPage,
            ToolStripMenuItem invertSelectionCurPage,
            ToolStripMenuItem selectAutorunCurPage,
            ToolStripMenuItem selectRunningCurPage,
            ToolStripMenuItem selectTimeoutCurPage,
            ToolStripMenuItem selectNoSpeedTestCurPage,
            ToolStripMenuItem selectNoMarkCurPage,
            ToolStripMenuItem selectUntrackCurPage
        )
        {
            selectAllCurPage.Click += (s, a) => SelectCurPageWhere(_ => true);

            selectNoneCurPage.Click += (s, a) => SelectCurPageWhere(_ => false);

            invertSelectionCurPage.Click += (s, a) =>
                SelectCurPageWhere(coreServ => !coreServ.GetCoreStates().IsSelected());

            selectAutorunCurPage.Click += (s, a) =>
                SelectCurPageWhere(coreServ => coreServ.GetCoreStates().IsAutoRun());
            selectRunningCurPage.Click += (s, a) =>
                SelectCurPageWhere(coreServ => coreServ.GetCoreCtrl().IsCoreRunning());
            selectTimeoutCurPage.Click += (s, a) =>
                SelectCurPageWhere(coreServ =>
                    coreServ.GetCoreStates().GetSpeedTestResult() == SpeedtestTimeout
                );
            selectNoSpeedTestCurPage.Click += (s, a) =>
                SelectCurPageWhere(coreServ => coreServ.GetCoreStates().GetSpeedTestResult() <= 0);
            selectNoMarkCurPage.Click += (s, a) =>
                SelectCurPageWhere(coreServ =>
                    string.IsNullOrEmpty(coreServ.GetCoreStates().GetMark())
                );
            selectUntrackCurPage.Click += (s, a) =>
                SelectCurPageWhere(coreServ => coreServ.GetCoreStates().IsUntrack());
        }

        void SelectCurPageWhere(Func<ICoreServCtrl, bool> condiction)
        {
            GetFlyPanel()
                .LoopThroughAllServerUI(s =>
                {
                    s.GetCoreStates().SetIsSelected(condiction(s));
                });
        }

        void SelectAllPagesWhere(Func<VgcApis.Interfaces.ICoreServCtrl, bool> condiction)
        {
            GetFlyPanel()
                .GetFilteredList()
                .ForEach(s => s.GetCoreStates().SetIsSelected(condiction(s)));
        }

        void SelectAllServersWhere(Func<VgcApis.Interfaces.ICoreServCtrl, bool> condiction)
        {
            var servs = servers.GetAllServersOrderByIndex();
            foreach (var serv in servs)
            {
                var isSelected = condiction(serv);
                serv.GetCoreStates().SetIsSelected(isSelected);
            }
        }

        FlyServer GetFlyPanel()
        {
            return this.GetContainer().GetComponent<FlyServer>();
        }
        #endregion
    }
}
