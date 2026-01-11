using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using V2RayGCon.Resources.Resx;

namespace V2RayGCon.Services
{
    class Launcher : VgcApis.BaseClasses.Disposable
    {
        public ApplicationContext context { get; private set; }

        readonly Settings setting;

        Notifier notifier;
        Servers servers;
        Updater updater;

        bool isDisposing = false;
        List<IDisposable> services = new List<IDisposable>();
        readonly string appName = Misc.Utils.GetAppNameAndVer();

        public Launcher()
        {
            VgcApis.Libs.Sys.FileLogger.Raw("\n");
            VgcApis.Libs.Sys.FileLogger.Info($"{appName} start");

            this.context = new ApplicationContext();
            VgcApis.Libs.Sys.FileLogger.Info($"Init Settings service");
            this.setting = Settings.Instance;
        }

        #region public method
        public bool Warmup()
        {
            switch (setting.GetShutdownReason())
            {
                case VgcApis.Models.Datas.Enums.ShutdownReasons.ShowHelpInfo:
                case VgcApis.Models.Datas.Enums.ShutdownReasons.FileLocked:
                    return false;
                case VgcApis.Models.Datas.Enums.ShutdownReasons.Abort:
                    setting.DisposeFileMutex();
                    return false;
            }

            Misc.Utils.EnableTls13Support();
            notifier = Notifier.Instance;
            SetCulture(setting.culture);
            return true;
        }

        public void Run()
        {
            BindApplicationGlobalEvents();
            InitAllServices();
            BootUp();

            var nv = Misc.Utils.GetAppNameAndVer();
            var dir = VgcApis.Misc.Utils.GetAppDir(); // init VgcApi.dll
            VgcApis.Misc.Logger.Log($"{nv} started.");
            VgcApis.Misc.Logger.Log($"{dir}");

            This_Function_Is_Used_For_Debugging();
        }

        #endregion

        #region debug
#pragma warning disable IDE0051 // 删除未使用的私有成员
        void ShowPlugin(string name)
#pragma warning restore IDE0051 // 删除未使用的私有成员
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

        [System.Diagnostics.Conditional("DEBUG")]
        void This_Function_Is_Used_For_Debugging()
        {
            ShowPlugin(@"Composer");
            // ShowPlugin(@"Commander");
            // ShowPlugin(@"NeoLuna");
            // ShowPlugin(@"ProxySetter");

            // Views.WinForms.FormLog.ShowForm();
            // Views.WinForms.FormMain.ShowForm();
            // Views.WinForms.FormToolbox.ShowForm();
            // Views.WinForms.FormTextConfigEditor.ShowEmptyConfig();

            //notifier.InjectDebugMenuItem(new ToolStripMenuItem(
            //    "Debug",
            //    null,
            //    (s, a) =>
            //    {
            //        servers.DbgFastRestartTest(100);
            //    }));

            // Views.WinForms.FormOption.GetForm();
            // setting.WakeupAutorunServer();
            // Views.WinForms.FormDownloadCore.GetForm();
        }

        #endregion

        #region private method
        bool isExceptionDetailShown = false;

        void ShowExceptionDetailAndExit(Exception exception)
        {
            if (isExceptionDetailShown)
            {
                return;
            }
            isExceptionDetailShown = true;

            VgcApis.Libs.Sys.FileLogger.Error($"unhandled exception:\n{exception}");

            if (!setting.IsPowerOff())
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
                log += nl + nl + VgcApis.Misc.Logger.GetContent();
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

            CheckLastBootTimestampBg();
            CheckForUpdateBg(setting.isCheckV2RayCoreUpdateWhenAppStart, V2RayCoreUpdater);
            CheckForUpdateBg(
                setting.isCheckVgcUpdateWhenAppStart,
                () => updater.CheckForUpdate(false)
            );
        }

        void CheckLastBootTimestampBg()
        {
            var utcTick = setting.LastBootTimestamp;
            var now = DateTime.UtcNow.Ticks;
            setting.LastBootTimestamp = now;

            var exp = 42 - 1;
            if (VgcApis.Misc.Utils.CheckTimestamp(utcTick, exp))
            {
                return;
            }
            if (!VgcApis.Misc.Utils.CheckTimestamp(now, exp))
            {
                return;
            }
            VgcApis.Misc.Utils.RunInBackground(() =>
            {
                VgcApis.Misc.UI.ConfirmBg(I18N.WarnTrialEnds);
                VgcApis.Misc.UI.MsgBoxBg("", I18N.A1Message);
            });
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
            string curVerStr;
            using (var core = new Libs.V2Ray.Core(setting))
            {
                curVerStr = core.GetV2RayCoreVersion();
            }
            if (string.IsNullOrEmpty(curVerStr))
            {
                return;
            }

            var src = setting.v2rayCoreDownloadSource;
            var port = -1;
            var isSocks5 = false;
            if (setting.isUpdateUseProxy)
            {
                servers.GetAvailableProxyInfo(out isSocks5, out port);
            }
            var vers = Misc.Utils.GetOnlineV2RayCoreVersionList(isSocks5, port, src);
            if (vers.Count < 1)
            {
                return;
            }
            setting.SaveV2RayCoreVersionList(vers);

            var first = vers.First();
            var msg = string.Format(I18N.ConfirmUpgradeV2rayCore, first);

            if (
                VgcApis.Misc.Utils.TryParseVersionString(first, out var vRemote)
                && VgcApis.Misc.Utils.TryParseVersionString(curVerStr, out var vLocal)
                && vRemote > vLocal
                && VgcApis.Misc.UI.Confirm(msg)
            )
            {
                Views.WinForms.FormDownloadCore.ShowForm();
            }
        }

        void BindApplicationGlobalEvents()
        {
            Application.ApplicationExit += (s, a) =>
            {
                VgcApis.Libs.Sys.FileLogger.Raw("");
                VgcApis.Libs.Sys.FileLogger.Warn($"detect application exit event.");
                context.ExitThread();
            };

            Microsoft.Win32.SystemEvents.SessionSwitch += (s, a) =>
            {
                VgcApis.Libs.Sys.FileLogger.Raw("");
                VgcApis.Libs.Sys.FileLogger.Warn($"detect session switch event: {a.Reason}");
            };

            Microsoft.Win32.SystemEvents.SessionEnding += (s, a) =>
            {
                VgcApis.Libs.Sys.FileLogger.Raw("");
                VgcApis.Libs.Sys.FileLogger.Warn($"detect session ending event: {a.Reason}");
                setting.SetShutdownReason(VgcApis.Models.Datas.Enums.ShutdownReasons.PowerOff);

                SetStateClosing();
                servers?.SaveServersSettingNow();
                setting.SaveUserSettingsNow();

                // for win7 logoff
                context.ExitThread();
            };

            Application.ThreadException += (s, a) => ShowExceptionDetailAndExit(a.Exception);
            AppDomain.CurrentDomain.UnhandledException += (s, a) =>
                ShowExceptionDetailAndExit(a.ExceptionObject as Exception);
            TaskScheduler.UnobservedTaskException += (s, a) =>
                ShowExceptionDetailAndExit(a.Exception);
        }

        void InitAllServices()
        {
            // Do not set value to this.servers before Services.Servers.Run().
            // Incase computer shutdown during init.
            var servers = Servers.Instance;
            var updater = Updater.Instance;

            // warn-up
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
            configMgr.Run(setting);
            servers.Run(setting, configMgr);
            this.servers = servers;

            updater.Run(setting, servers);
            this.updater = updater;

            slinkMgr.Run(setting, servers);
            notifier.Run(setting, servers, slinkMgr, updater);
            pluginsServ.Run(setting, servers, configMgr, slinkMgr, notifier);
        }

        void SetStateClosing()
        {
            setting.SetIsClosing(true);
            setting.isSpeedtestCancelled = true;
        }

        readonly object disposeLocker = new object();

        void DisposeAllServices()
        {
            // throw new NullReferenceException("for debugging");
            VgcApis.Libs.Sys.FileLogger.Info("Luancher.DisposeAllServices() begin");
            VgcApis.Libs.Sys.FileLogger.Info($"App exit reason: {setting.GetShutdownReason()}");

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

            SetStateClosing();

            foreach (var service in services)
            {
                service.Dispose();
            }

            if (
                setting.GetShutdownReason()
                == VgcApis.Models.Datas.Enums.ShutdownReasons.CloseByUser
            )
            {
                VgcApis.Libs.Sys.FileLogger.Info(
                    "Luancher.DisposeAllServices() call servers.StopAllServers()"
                );
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

            Thread
                .CurrentThread.CurrentCulture.GetType()
                .GetProperty("DefaultThreadCurrentCulture")
                .SetValue(Thread.CurrentThread.CurrentCulture, ci, null);

            Thread
                .CurrentThread.CurrentCulture.GetType()
                .GetProperty("DefaultThreadCurrentUICulture")
                .SetValue(Thread.CurrentThread.CurrentCulture, ci, null);
        }
        #endregion

        #region protected override
        protected override void Cleanup()
        {
            VgcApis.Libs.Sys.FileLogger.Info("Launcher.Cleanup() begin");
            DisposeAllServices();
            VgcApis.Libs.Sys.FileLogger.Info("Launcher.Cleanup() done");
            VgcApis.Libs.Sys.FileLogger.Info($"{appName} exited");
        }
        #endregion
    }
}
