using ProxySetter.Resources.Langs;
using System;
using System.Windows.Forms;
using VgcApis.Libs.Sys;

namespace ProxySetter.Services
{
    class PsLuncher
    {
        PsSettings setting;
        PacServer pacServer;
        ServerTracker serverTracker;

        VgcApis.Models.IServices.IApiService vgcApi;
        Model.Data.ProxySettings orgSysProxySetting;
        Views.WinForms.FormMain formMain;
        public PsLuncher() { }

        #region public methods

        public void Run(VgcApis.Models.IServices.IApiService api)
        {
            orgSysProxySetting = Lib.Sys.ProxySetter.GetProxySetting();
            FileLogger.Info("ProxySetter: save sys proxy settings");

            this.vgcApi = api;

            var vgcSetting = api.GetSettingService();
            var vgcServer = api.GetServersService();
            var vgcNotifier = api.GetNotifierService();

            pacServer = new PacServer();
            setting = new PsSettings();
            serverTracker = new ServerTracker();

            // dependency injection
            setting.Run(vgcSetting);
            pacServer.Run(setting);

            serverTracker.OnSysProxyChanged += UpdateMenuItemCheckedStatHandler;
            serverTracker.Run(setting, pacServer, vgcServer, vgcNotifier);
        }

        public void Show()
        {
            if (formMain != null)
            {
                return;
            }

            formMain = new Views.WinForms.FormMain(
                setting,
                pacServer,
                serverTracker);
            formMain.FormClosed += (s, a) => formMain = null;
            formMain.Show();
        }

        public void Cleanup()
        {
            setting.DebugLog("call Luncher.cleanup");
            setting.isCleaning = true;

            serverTracker.OnSysProxyChanged -= UpdateMenuItemCheckedStatHandler;
            formMain?.Close();
            serverTracker.Cleanup();
            pacServer.Cleanup();
            setting.Cleanup();
            Lib.Sys.ProxySetter.UpdateProxySettingOnDemand(orgSysProxySetting);
            FileLogger.Info("ProxySetter: restore sys proxy settings");

        }

        public ToolStripMenuItem[] GetSubMenu() => GenSubMenu();
        #endregion

        #region private methods
        ToolStripMenuItem miProxyModeDirect = null;
        ToolStripMenuItem miProxyModeGlobal = null;
        ToolStripMenuItem miProxyModePac = null;
        ToolStripMenuItem miProxyModeNone = null;

        ToolStripMenuItem[] GenSubMenu()
        {
            miProxyModeNone = new ToolStripMenuItem(
               I18N.MiNone, null, (s, a) => SetProxyMode(Model.Data.Enum.SystemProxyModes.None));

            miProxyModePac = new ToolStripMenuItem(
                I18N.MiPAC, null, (s, a) => SetProxyMode(Model.Data.Enum.SystemProxyModes.PAC));
            miProxyModeDirect = new ToolStripMenuItem(
                I18N.MiDirect, null, (s, a) => SetProxyMode(Model.Data.Enum.SystemProxyModes.Direct));
            miProxyModeGlobal = new ToolStripMenuItem(
                I18N.MiGlobal, null, (s, a) => SetProxyMode(Model.Data.Enum.SystemProxyModes.Global));

            var menu = new ToolStripMenuItem[] {
                miProxyModeNone,
                miProxyModeDirect,
                miProxyModeGlobal,
                miProxyModePac,
            };

            UpdateMenuItemCheckedStatHandler(this, EventArgs.Empty);

            return menu;
        }

        void UpdateMenuItemCheckedStatHandler(object sender, EventArgs events)
        {
            var bs = setting.GetBasicSetting();
            var pm = bs.sysProxyMode;

            if (miProxyModeNone != null)
            {
                miProxyModeNone.Checked =
                    (int)Model.Data.Enum.SystemProxyModes.None == pm;
            }

            if (miProxyModeDirect != null)
            {
                miProxyModeDirect.Checked =
                    (int)Model.Data.Enum.SystemProxyModes.Direct == pm;
            }

            if (miProxyModeGlobal != null)
            {
                miProxyModeGlobal.Checked =
                    (int)Model.Data.Enum.SystemProxyModes.Global == pm;
            }

            if (miProxyModePac != null)
            {
                miProxyModePac.Checked =
                    (int)Model.Data.Enum.SystemProxyModes.PAC == pm;
            }
        }

        void SetProxyMode(Model.Data.Enum.SystemProxyModes proxyMode)
        {
            var bs = setting.GetBasicSetting();
            bs.sysProxyMode = (int)proxyMode;
            setting.SaveBasicSetting(bs);
            serverTracker?.Restart();
        }

        #endregion
    }

}
