using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using V2RayGCon.Resources.Resx;

namespace V2RayGCon.Services
{
    class Launcher
    {
        Settings setting;
        Servers servers;
        Updater updater;
        Notifier notifier;

        bool isDisposing = false;
        List<IDisposable> services = new List<IDisposable>();

        public Launcher() { }

        #region public method
        public bool Warmup()
        {
            Misc.Utils.SupportProtocolTLS12();

            setting = Settings.Instance;
            if (setting.ShutdownReason == VgcApis.Models.Datas.Enums.ShutdownReasons.Abort)
            {
                return false;
            }
            SetCulture(setting.culture);
            return true;
        }

        public void Run()
        {
            notifier = Notifier.Instance;
            servers = Servers.Instance;
            updater = Updater.Instance;

            var nm = notifier.niMenu;
            nm.CreateControl();
            nm.Show();
            nm.Close();

            InitAllServices();
            BindEvents();
            Boot();

#if DEBUG
            This_Function_Is_Used_For_Debugging();
#endif
        }

        private void Boot()
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

            if (setting.isCheckUpdateWhenAppStart)
            {
                Task.Run(() =>
                {
#if DEBUG
#else
                    Task.Delay(VgcApis.Models.Consts.Webs.CheckForUpdateDelay).Wait();
#endif
                    updater.CheckForUpdate(false);
                }).ConfigureAwait(false);
            }
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
                    plugin.Show();
                }
            }

        }

#if DEBUG
        void This_Function_Is_Used_For_Debugging()
        {
            ShowPlugin(@"Luna");

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
            // Views.WinForms.FormSimAddVmessClient.GetForm();
            // Views.WinForms.FormDownloadCore.GetForm();
        }
#endif
        #endregion

        #region private method

        void InitAllServices()
        {
            // warn-up
            var cache = Cache.Instance;
            var configMgr = ConfigMgr.Instance;
            var slinkMgr = ShareLinkMgr.Instance;
            var pluginsServ = PluginsServer.Instance;

            // by dispose order
            services = new List<IDisposable> {
                updater,
                pluginsServ,
                notifier,
                slinkMgr,
                servers,
                configMgr,
                setting,
            };

            // dependency injection
            cache.Run(setting);
            configMgr.Run(setting, cache);
            servers.Run(setting, cache, configMgr);
            updater.Run(setting, servers);
            slinkMgr.Run(setting, servers, cache);
            notifier.Run(setting, servers, slinkMgr, updater);
            pluginsServ.Run(setting, servers, configMgr, slinkMgr, notifier);
        }

        void BindEvents()
        {
            Application.ApplicationExit += (s, a) => DisposeAllServices();

            Microsoft.Win32.SystemEvents.SessionEnding += (s, a) =>
            {
                setting.ShutdownReason = VgcApis.Models.Datas.Enums.ShutdownReasons.Poweroff;
                DisposeAllServices();
            };

            Application.ThreadException += (s, a) => ShowExceptionDetailAndExit(a.Exception);
            AppDomain.CurrentDomain.UnhandledException += (s, a) => ShowExceptionDetailAndExit(a.ExceptionObject as Exception);
        }

        readonly object disposeLocker = new object();
        void DisposeAllServices()
        {
            // throw new NullReferenceException("for debugging");

            if (setting.ShutdownReason == VgcApis.Models.Datas.Enums.ShutdownReasons.Abort)
            {
                // shutdown directly
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

            setting.SetIsShutdown(true);
            foreach (var service in services)
            {
                service.Dispose();
            }

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

            Thread.CurrentThread.CurrentCulture.GetType()
                .GetProperty("DefaultThreadCurrentCulture")
                .SetValue(Thread.CurrentThread.CurrentCulture, ci, null);

            Thread.CurrentThread.CurrentCulture.GetType()
                .GetProperty("DefaultThreadCurrentUICulture")
                .SetValue(Thread.CurrentThread.CurrentCulture, ci, null);
        }
        #endregion

        #region unhandle exception
        void ShowExceptionDetailAndExit(Exception exception)
        {
            if (setting.ShutdownReason != VgcApis.Models.Datas.Enums.ShutdownReasons.Poweroff)
            {
                ShowExceptionDetails(exception);
            }
            Application.Exit();
        }

        private void ShowExceptionDetails(Exception exception)
        {
            var nl = Environment.NewLine;
            var verInfo = Misc.Utils.GetAppNameAndVer();
            var log = $"{I18N.LooksLikeABug}{nl}{nl}{verInfo}";

            try
            {

                log += nl + nl + exception.ToString();
                log += nl + nl + setting.GetLogContent();
            }
            catch { }

            VgcApis.Libs.Sys.NotepadHelper.ShowMessage(log, "V2RayGCon bug report");
        }

        #endregion
    }
}
