using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using V2RayGCon.Resources.Resx;

namespace V2RayGCon.Services
{
    class Launcher : VgcApis.BaseClasses.Disposable
    {
        private readonly Settings setting;

        public ApplicationContext context { get; private set; }

        Servers servers;
        Updater updater;
        Notifier notifier;

        bool isDisposing = false;
        List<IDisposable> services = new List<IDisposable>();
        string appName;

        public Launcher()
        {
            this.context = new ApplicationContext();
            this.setting = Settings.Instance;

            appName = Misc.Utils.GetAppNameAndVer();
            VgcApis.Libs.Sys.FileLogger.Raw("\n");
            VgcApis.Libs.Sys.FileLogger.Info($"{appName} start");

            notifier = Notifier.Instance;
        }

        #region public method
        public bool Warmup()
        {
            Misc.Utils.EnableTls13Support();
            if (setting.GetShutdownReason() == VgcApis.Models.Datas.Enums.ShutdownReasons.Abort)
            {
                return false;
            }
            SetCulture(setting.culture);
            return true;
        }

        public void Run()
        {
            BindAppExitEvents();
            InitAllServices();
            BootUp();

            var nv = Misc.Utils.GetAppNameAndVer();
            var dir = VgcApis.Misc.Utils.GetAppDir(); // init VgcApi.dll
            setting.SendLog($"{nv} started.");
            setting.SendLog($"{dir}");

#if DEBUG
            This_Function_Is_Used_For_Debugging();
#endif
        }

        #endregion

        #region debug
        void ShowPlugin(string name)
        {
            var pluginsServ = PluginsServer.Instance;
            var plugins = pluginsServ.GetAllEnabledPlugins();

            foreach (var plugin in plugins)
            {
                if (name == plugin.Name)
                {
                    VgcApis.Misc.UI.Invoke(() => plugin.ShowMainForm());
                }
            }
        }

#if DEBUG
        void This_Function_Is_Used_For_Debugging()
        {
            // ShowPlugin(@"NeoLuna");

            // Views.WinForms.FormLog.ShowForm();
            Views.WinForms.FormMain.ShowForm();

            //notifier.InjectDebugMenuItem(new ToolStripMenuItem(
            //    "Debug",
            //    null,
            //    (s, a) =>
            //    {
            //        servers.DbgFastRestartTest(100);
            //    }));

            // new Views.WinForms.FormConfiger(@"{}");
            // new Views.WinForms.FormConfigTester();
            // Views.WinForms.FormOption.GetForm();
            // Views.WinForms.FormMain.ShowForm();
            // Views.WinForms.FormLog.ShowForm();
            // setting.WakeupAutorunServer();
            // Views.WinForms.FormSimpleEditor.GetForm();
            // Views.WinForms.FormDownloadCore.GetForm();
        }
#endif
        #endregion

        #region private method

        void ShowExceptionDetailAndExit(Exception exception)
        {
            VgcApis.Libs.Sys.FileLogger.Error($"unhandled exception:\n{exception}");

            if (setting.GetShutdownReason() != VgcApis.Models.Datas.Enums.ShutdownReasons.Poweroff)
            {
                ShowExceptionDetails(exception);
            }
            context.ExitThread();
        }

        private void ShowExceptionDetails(Exception exception)
        {
            var nl = Environment.NewLine;
            var verInfo = Misc.Utils.GetAppNameAndVer();
            var issue = Properties.Resources.IssueLink;
            var log = $"{I18N.LooksLikeABug}{nl}{issue}{nl}{nl}{verInfo}";

            try
            {
                log += nl + nl + exception.ToString();
                log += nl + nl + setting.GetLogContent();
            }
            catch { }

            VgcApis.Libs.Sys.NotepadHelper.ShowMessage(log, "V2RayGCon bug report");
        }

        private void BootUp()
        {
            PluginsServer.Instance.RestartAllPlugins();

            if (servers.IsEmpty())
            {
                Views.WinForms.FormMain.ShowForm();
            }
            else
            {
                servers.WakeupServersInBootList();
            }

            CheckForUpdateBg(setting.isCheckV2RayCoreUpdateWhenAppStart, V2RayCoreUpdater);
            CheckForUpdateBg(
                setting.isCheckVgcUpdateWhenAppStart,
                () => updater.CheckForUpdate(false)
            );
        }

        void CheckForUpdateBg(bool flag, Action worker)
        {
            if (!flag)
            {
                return;
            }

            VgcApis.Misc.Utils.RunInBackground(() =>
            {
#if DEBUG
                VgcApis.Misc.Utils.Sleep(5000);
#else
                VgcApis.Misc.Utils.Sleep(VgcApis.Models.Consts.Webs.CheckForUpdateDelay);
#endif
                try
                {
                    worker?.Invoke();
                }
                catch { }
                ;
            });
        }

        void V2RayCoreUpdater()
        {
            var core = new Libs.V2Ray.Core(setting);
            var curVerStr = core.GetCoreVersion();
            if (string.IsNullOrEmpty(curVerStr))
            {
                return;
            }

            var src = setting.v2rayCoreDownloadSource;
            var port = -1;
            if (setting.isUpdateUseProxy)
            {
                port = servers.GetAvailableHttpProxyPort();
            }

            var vers = Misc.Utils.GetOnlineV2RayCoreVersionList(port, src);
            if (vers.Count < 1)
            {
                return;
            }
            setting.SaveV2RayCoreVersionList(vers);

            var first = vers.First();
            var current = new Version(curVerStr);
            var latest = new Version(first.Substring(1));

            var msg = string.Format(I18N.ConfirmUpgradeV2rayCore, first);
            if (latest > current && VgcApis.Misc.UI.Confirm(msg))
            {
                Views.WinForms.FormDownloadCore.ShowForm();
            }
        }

        void BindAppExitEvents()
        {
            Application.ApplicationExit += (s, a) =>
            {
                VgcApis.Libs.Sys.FileLogger.Warn($"detect application exit event.");
                context.ExitThread();
            };

            Microsoft.Win32.SystemEvents.SessionSwitch += (s, a) =>
            {
                VgcApis.Libs.Sys.FileLogger.Warn($"detect session switch event: {a.Reason}");
            };

            Microsoft.Win32.SystemEvents.SessionEnding += (s, a) =>
            {
                VgcApis.Libs.Sys.FileLogger.Warn($"receive session ending event.");
                setting.SetShutdownReason(VgcApis.Models.Datas.Enums.ShutdownReasons.Poweroff);
                setting.SaveUserSettingsNow();

                // for win7 logoff
                context.ExitThread();
            };

            Application.ThreadException += (s, a) => ShowExceptionDetailAndExit(a.Exception);
            AppDomain.CurrentDomain.UnhandledException += (s, a) =>
                ShowExceptionDetailAndExit(a.ExceptionObject as Exception);
        }

        void InitAllServices()
        {
            servers = Servers.Instance;
            updater = Updater.Instance;

            // warn-up
            var cache = Cache.Instance;
            var configMgr = ConfigMgr.Instance;
            var slinkMgr = ShareLinkMgr.Instance;
            var pluginsServ = PluginsServer.Instance;

            // by dispose order
            services = new List<IDisposable>
            {
                updater,
                pluginsServ,
                notifier,
                slinkMgr,
                servers,
                configMgr,
                setting,
                PostOffice.Instance,
            };

            // dependency injection
            cache.Run(setting);
            configMgr.Run(setting, cache);
            servers.Run(setting, cache, configMgr, notifier);
            updater.Run(setting, servers);
            slinkMgr.Run(setting, servers, cache);
            notifier.Run(setting, servers, slinkMgr, updater);
            pluginsServ.Run(setting, servers, configMgr, slinkMgr, notifier);
        }

        readonly object disposeLocker = new object();

        void DisposeAllServices()
        {
            // throw new NullReferenceException("for debugging");
            VgcApis.Libs.Sys.FileLogger.Info("Luancher.DisposeAllServices() begin");
            VgcApis.Libs.Sys.FileLogger.Info($"Close reason: {setting.GetShutdownReason()}");

            if (setting.GetShutdownReason() == VgcApis.Models.Datas.Enums.ShutdownReasons.Abort)
            {
                VgcApis.Libs.Sys.FileLogger.Info("Luancher.DisposeAllServices() abort");
                return;
            }

            lock (disposeLocker)
            {
                if (isDisposing)
                {
                    return;
                }
                isDisposing = true;
            }

            setting.SetIsClosing(true);
            setting.isSpeedtestCancelled = true;

            foreach (var service in services)
            {
                service.Dispose();
            }

            if (
                setting.GetShutdownReason()
                == VgcApis.Models.Datas.Enums.ShutdownReasons.CloseByUser
            )
            {
                servers.StopAllServers();
            }

            VgcApis.Libs.Sys.FileLogger.Info("Luancher.DisposeAllServices() done");
        }

        void SetCulture(Models.Datas.Enums.Cultures culture)
        {
            string cultureString = null;

            switch (culture)
            {
                case Models.Datas.Enums.Cultures.enUS:
                    cultureString = "";
                    break;
                case Models.Datas.Enums.Cultures.zhCN:
                    cultureString = "zh-CN";
                    break;
                case Models.Datas.Enums.Cultures.auto:
                    return;
            }

            var ci = new CultureInfo(cultureString);

            Thread.CurrentThread.CurrentCulture
                .GetType()
                .GetProperty("DefaultThreadCurrentCulture")
                .SetValue(Thread.CurrentThread.CurrentCulture, ci, null);

            Thread.CurrentThread.CurrentCulture
                .GetType()
                .GetProperty("DefaultThreadCurrentUICulture")
                .SetValue(Thread.CurrentThread.CurrentCulture, ci, null);
        }
        #endregion

        #region protected override
        protected override void Cleanup()
        {
            VgcApis.Libs.Sys.FileLogger.Raw("");
            DisposeAllServices();
            VgcApis.Libs.Sys.FileLogger.Info($"{appName} exited");
        }
        #endregion
    }
}
