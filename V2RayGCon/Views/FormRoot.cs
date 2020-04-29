using System;
using System.Collections.Concurrent;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using V2RayGCon.Resources.Resx;

namespace V2RayGCon.Views
{
    public partial class FormRoot : Form
    {
        Services.Settings setting;
        Services.Notifier notifier;
        Services.Launcher launcher;

        string appName = @"";

        public NotifyIcon notifyIcon => this.ni;
        // VgcApis.WinForms.HotKeyWindow hkWindow;

        public FormRoot()
        {
            InitializeComponent();

            VgcApis.Misc.UI.AutoSetFormIcon(this);

            CreateHandle();

            setting = Services.Settings.Instance;
            notifier = Services.Notifier.Instance;
            notifier.InitNotifyIconFor(this);

            launcher = new Services.Launcher(setting, notifier);

            appName = Misc.Utils.GetAppNameAndVer();
            VgcApis.Libs.Sys.FileLogger.Raw("\n");
            VgcApis.Libs.Sys.FileLogger.Info($"{appName} start");
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            BindEvents();

            if (!launcher.Warmup())
            {
                setting.ShutdownReason = VgcApis.Models.Datas.Enums.ShutdownReasons.CloseByUser;
                VgcApis.Misc.UI.Invoke(this, () => Application.Exit());
                return;
            }

            launcher.Run();
        }



        #region exit 
        void Cleanup()
        {
            VgcApis.Libs.Sys.FileLogger.Info("");
            RemoveAllHotKeys();
            launcher?.Dispose();
            VgcApis.Libs.Sys.FileLogger.Info($"{appName} end");
        }

        void BindEvents()
        {
            Microsoft.Win32.SystemEvents.PowerModeChanged += (s, a) =>
            {
                switch (a.Mode)
                {
                    case Microsoft.Win32.PowerModes.Suspend:
                        setting.SetScreenLockingState(true);
                        break;
                    default:
                        break;
                }
            };

            Microsoft.Win32.SystemEvents.SessionSwitch += (s, a) =>
            {
                switch (a.Reason)
                {
                    case Microsoft.Win32.SessionSwitchReason.SessionLock:
                        setting.SetScreenLockingState(true);
                        break;
                    case Microsoft.Win32.SessionSwitchReason.SessionUnlock:
                        setting.SetScreenLockingState(false);
                        break;
                    default:
                        break;
                }
            };

            Application.ApplicationExit += (s, a) => Cleanup();

            Microsoft.Win32.SystemEvents.SessionEnding += (s, a) =>
            {
                setting.ShutdownReason = VgcApis.Models.Datas.Enums.ShutdownReasons.Poweroff;
                Close();
            };

            Application.ThreadException += (s, a) => ShowExceptionDetailAndExit(a.Exception);
            AppDomain.CurrentDomain.UnhandledException += (s, a) => ShowExceptionDetailAndExit(a.ExceptionObject as Exception);
        }

        void ShowExceptionDetailAndExit(Exception exception)
        {
            VgcApis.Libs.Sys.FileLogger.Error($"unhandled exception:\n{exception}");

            if (setting.ShutdownReason != VgcApis.Models.Datas.Enums.ShutdownReasons.Poweroff)
            {
                ShowExceptionDetails(exception);
            }
            VgcApis.Misc.UI.Invoke(this, () => Application.Exit());
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

        #region WinAPI
        const int WM_HOTKEY = 0x0312;


        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);
        #endregion

        #region hotkey window
        ConcurrentDictionary<long, Action> handlers = new ConcurrentDictionary<long, Action>();
        ConcurrentDictionary<string, Tuple<int, long>> contexts = new ConcurrentDictionary<string, Tuple<int, long>>();
        int currentEvCode = 0;

        string RegisterHotKeyWorker(
            Action hotKeyHandler,
            string keyName, bool hasAlt, bool hasCtrl, bool hasShift)
        {

            var evCode = currentEvCode++;

            if (!VgcApis.Misc.Utils.TryParseKeyMesssage(keyName, hasAlt, hasCtrl, hasShift,
                out uint modifier, out uint key))
            {
                return null;
            }

            long hkMsg = (key << 16) | modifier;
            var handlerKeys = handlers.Keys;
            if (handlerKeys.Contains(hkMsg) || !RegisterHotKey(Handle, evCode, modifier, key))
            {
                return null;
            }

            for (int failsafe = 0; failsafe < 1000; failsafe++)
            {
                var hkHandle = Guid.NewGuid().ToString();
                var keys = contexts.Keys;
                if (!keys.Contains(hkHandle))
                {
                    var hkParma = new Tuple<int, long>(evCode, hkMsg);
                    contexts.TryAdd(hkHandle, hkParma);
                    handlers.TryAdd(hkMsg, hotKeyHandler);
                    return hkHandle;
                }
            }

            return null;
        }

        void RemoveAllHotKeys()
        {
            VgcApis.Libs.Sys.FileLogger.Info("HotKey window remove all hotkey begin");
            var handles = contexts.Keys;
            foreach (var handle in handles)
            {
                UnregisterHotKeyWorker(handle);
            }
            VgcApis.Libs.Sys.FileLogger.Info("HotKey window remove all hotkey end");
        }

        bool UnregisterHotKeyWorker(string hotKeyHandle)
        {
            try
            {
                if (!string.IsNullOrEmpty(hotKeyHandle)
                    && contexts.TryRemove(hotKeyHandle, out var context))
                {
                    var evCode = context.Item1;
                    var keyMsg = context.Item2;
                    var ok = UnregisterHotKey(this.Handle, evCode);
                    handlers.TryRemove(keyMsg, out _);
                    return ok;
                }
            }
            catch { }
            return false;
        }

        bool HandleHotKey(uint keyCode)
        {
            long key = keyCode & 0xffffffff;
            try
            {
                if (handlers.TryGetValue(key, out var handler))
                {
                    VgcApis.Misc.Utils.RunInBackground(() =>
                    {
                        try
                        {
                            handler?.Invoke();
                        }
                        catch (Exception e)
                        {
                            VgcApis.Libs.Sys.FileLogger.Error($"Handle hotkey event error\n key code:{key} \n {e}");
                        }
                    });
                    return true;
                }
            }
            catch (Exception e)
            {
                VgcApis.Libs.Sys.FileLogger.Error($"Handle wnd event error\n key code:{key} \n {e}");
            }
            return false;
        }

        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        protected override void WndProc(ref Message m)
        {
            // check if we got a hot key pressed.
            if (m.Msg == WM_HOTKEY
                && HandleHotKey((uint)m.LParam))
            {
                return;
            }

            base.WndProc(ref m);
        }

        public string RegisterHotKey(
             Action hotKeyHandler,
             string keyName, bool hasAlt, bool hasCtrl, bool hasShift)
        {
            string handle = null;
            VgcApis.Misc.UI.Invoke(this, () =>
            {
                try
                {
                    handle = RegisterHotKeyWorker(hotKeyHandler, keyName, hasAlt, hasCtrl, hasShift);
                }
                catch { }
            });
            return handle;
        }
        public bool UnregisterHotKey(string hotKeyHandle)
        {

            var r = false;
            VgcApis.Misc.UI.Invoke(this, () =>
            {
                try
                {
                    r = UnregisterHotKeyWorker(hotKeyHandle);
                }
                catch { }
            });
            return r;
        }

        #endregion

        #region private method


        #endregion

        #region UI event handler
        private void FormRoot_Shown(object sender, EventArgs e)
        {
            Hide();
        }
        #endregion
    }
}
