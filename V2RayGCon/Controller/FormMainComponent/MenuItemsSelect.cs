using System;
using System.Linq;
using System.Windows.Forms;

namespace V2RayGCon.Controller.FormMainComponent
{
    class MenuItemsSelect : FormMainComponentController
    {
        Service.Servers servers;
        public MenuItemsSelect(
            ToolStripMenuItem selectAllCurPage,
            ToolStripMenuItem invertSelectionCurPage,
            ToolStripMenuItem selectNoneCurPage,

            ToolStripMenuItem selectAllAllPages,
            ToolStripMenuItem invertSelectionAllPages,
            ToolStripMenuItem selectNoneAllPages,

            ToolStripMenuItem selectAutorunAllPages,
            ToolStripMenuItem selectNoMarkAllPages,
            ToolStripMenuItem selectNoSpeedTestAllPages,
            ToolStripMenuItem selectRunningAllPages,
            ToolStripMenuItem selectTimeoutAllPages,
            ToolStripMenuItem selectUntrackAllPages,

            ToolStripMenuItem selectAllAllServers,
            ToolStripMenuItem invertSelectionAllServers,
            ToolStripMenuItem selectNoneAllServers,

            ToolStripMenuItem selectAutorunAllServers,
            ToolStripMenuItem selectNoMarkAllServers,
            ToolStripMenuItem selectNoSpeedTestAllServers,
            ToolStripMenuItem selectRunningAllServers,
            ToolStripMenuItem selectTimeoutAllServers,
            ToolStripMenuItem selectUntrackAllServers)


        {
            servers = Service.Servers.Instance;

            InitAllPagesSelectors(selectNoneAllPages, invertSelectionAllPages, selectAllAllPages, selectAutorunAllPages, selectRunningAllPages, selectTimeoutAllPages, selectNoSpeedTestAllPages, selectNoMarkAllPages, selectUntrackAllPages);
            InitAllServersSelector(selectNoneAllServers, invertSelectionAllServers, selectAllAllServers, selectAutorunAllServers, selectRunningAllServers, selectTimeoutAllServers, selectNoSpeedTestAllServers, selectNoMarkAllServers, selectUntrackAllServers);
            InitCurPageSelectors(selectAllCurPage, selectNoneCurPage, invertSelectionCurPage);
        }


        #region public method
        public override bool RefreshUI()
        {
            return false;
        }

        public override void Cleanup()
        {
        }
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
            ToolStripMenuItem selectUntrackAllPages)
        {
            selectAllAllPages.Click +=
                            (s, a) => SelectAllPagesWhere(el => true);

            selectNoneAllPages.Click +=
                (s, a) => SelectAllPagesWhere(el => false);

            invertSelectionAllPages.Click +=
                (s, a) => SelectAllPagesWhere(el => !el.GetCoreStates().IsSelected());

            selectAutorunAllPages.Click +=
                (s, a) => SelectAllPagesWhere(el => el.GetCoreStates().IsAutoRun());

            selectRunningAllPages.Click +=
                 (s, a) => SelectAllPagesWhere(el => el.GetCoreCtrl().IsCoreRunning());

            selectTimeoutAllPages.Click +=
                (s, a) => SelectAllPagesWhere(el => el.GetCoreStates().GetSpeedTestResult() == long.MaxValue);

            selectNoSpeedTestAllPages.Click +=
                (s, a) => SelectAllPagesWhere(el => el.GetCoreStates().GetSpeedTestResult() < 0);

            selectNoMarkAllPages.Click +=
                (s, a) => SelectAllPagesWhere(
                    el => string.IsNullOrEmpty(el.GetCoreStates().GetMark()));

            selectUntrackAllPages.Click +=
                (s, a) => SelectAllPagesWhere(el => el.GetCoreStates().IsUntrack());

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
            ToolStripMenuItem selectUntrackAllServers)
        {
            selectAllAllServers.Click +=
                (s, a) => SelectAllServersWhere(el => true);

            invertSelectionAllServers.Click +=
                (s, a) => SelectAllServersWhere(el => !el.GetCoreStates().IsSelected());

            selectNoneAllServers.Click +=
                (s, a) => SelectAllServersWhere(el => false);

            selectNoMarkAllServers.Click +=
                (s, a) => SelectAllServersWhere(
                    el => string.IsNullOrEmpty(el.GetCoreStates().GetMark()));

            selectNoSpeedTestAllServers.Click +=
                (s, a) => SelectAllServersWhere(el => el.GetCoreStates().GetSpeedTestResult() < 0);

            selectTimeoutAllServers.Click +=
                (s, a) => SelectAllServersWhere(el => el.GetCoreStates().GetSpeedTestResult() == long.MaxValue);

            selectRunningAllServers.Click +=
                (s, a) => SelectAllServersWhere(el => el.GetCoreCtrl().IsCoreRunning());

            selectAutorunAllServers.Click +=
                (s, a) => SelectAllServersWhere(el => el.GetCoreStates().IsAutoRun());

            selectUntrackAllServers.Click +=
                (s, a) => SelectAllServersWhere(el => el.GetCoreStates().IsUntrack());
        }

        private void InitCurPageSelectors(ToolStripMenuItem selectAllCurPage, ToolStripMenuItem selectNoneCurPage, ToolStripMenuItem invertSelectionCurPage)
        {
            selectAllCurPage.Click +=
                (s, a) => SelectCurPageWhere(el => true);

            selectNoneCurPage.Click +=
                (s, a) => SelectCurPageWhere(el => false);

            invertSelectionCurPage.Click +=
                (s, a) => SelectCurPageWhere(el => !el.isSelected);
        }

        void SelectCurPageWhere(Func<Views.UserControls.ServerUI, bool> condiction)
        {
            var panel = GetFlyPanel();
            var configs = panel.GetFilteredList().Select(s => s.GetConfiger().GetConfig());

            // clear all not in current filtered list
            servers.GetAllServersOrderByIndex()
                .Where(s => !configs.Contains(s.GetConfiger().GetConfig()))
                .Select(s =>
                {
                    s.GetCoreStates().SetIsSelected(false);
                    return true;
                })
                .ToList();

            panel.LoopThroughAllServerUI(s =>
            {
                s.SetSelected(condiction(s));
            });
        }

        void SelectAllPagesWhere(
            Func<VgcApis.Models.Interfaces.ICoreServCtrl, bool>
            condiction)
        {
            var configs = GetFlyPanel().GetFilteredList()
                .Select(s => s.GetConfiger().GetConfig())
                .ToList();

            servers.GetAllServersOrderByIndex()
                .Select(s =>
                {
                    if (!configs.Contains(s.GetConfiger().GetConfig()))
                    {
                        s.GetCoreStates().SetIsSelected(false);
                        return false;
                    }
                    s.GetCoreStates().SetIsSelected(condiction(s));
                    return true;
                })
                .ToList();
        }


        void SelectAllServersWhere(
            Func<VgcApis.Models.Interfaces.ICoreServCtrl, bool>
            condiction)
        {
            servers.GetAllServersOrderByIndex()
                .Select(s =>
                {
                    s.GetCoreStates().SetIsSelected(condiction(s));
                    return true;
                })
                .ToList();
        }

        Controller.FormMainComponent.FlyServer GetFlyPanel()
        {
            return this.GetContainer()
                .GetComponent<Controller.FormMainComponent.FlyServer>();
        }
        #endregion
    }
}
