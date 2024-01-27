using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using ProxySetter.Resources.Langs;

namespace ProxySetter.Services
{
    class ServerTracker
    {
        PsSettings setting;
        PacServer pacServer;

        VgcApis.Interfaces.Services.IServersService servers;
        VgcApis.Interfaces.Services.INotifierService notifier;

        public event EventHandler OnSysProxyChanged;
        bool isTracking { get; set; }

        string hotKeyHandle = null;

        public ServerTracker()
        {
            isTracking = false;
        }

        #region public method
        public void Run(
            PsSettings setting,
            PacServer pacServer,
            VgcApis.Interfaces.Services.IServersService servers,
            VgcApis.Interfaces.Services.INotifierService notifier
        )
        {
            this.setting = setting;
            this.pacServer = pacServer;
            this.servers = servers;
            this.notifier = notifier;

            UpdateHotkey();
            Restart();
        }

        public void Restart()
        {
            var bs = setting.GetBasicSetting();
            var isStartPacServer = bs.isAlwaysStartPacServ;
            var isAutoMode = bs.isAutoUpdateSysProxy;

            switch ((Model.Data.Enum.SystemProxyModes)bs.sysProxyMode)
            {
                case Model.Data.Enum.SystemProxyModes.Global:
                    Libs.Sys.ProxySetter.SetGlobalProxy(bs.proxyPort);
                    break;
                case Model.Data.Enum.SystemProxyModes.PAC:
                    isStartPacServer = true;
                    Libs.Sys.ProxySetter.SetPacProxy(pacServer.GetPacUrl());
                    break;
                case Model.Data.Enum.SystemProxyModes.Direct:
                    isAutoMode = false;
                    Libs.Sys.ProxySetter.ClearSysProxy();
                    break;
                default:
                    isAutoMode = false;
                    break;
            }

            if (isStartPacServer)
            {
                pacServer.Reload();
            }
            else
            {
                pacServer.StopPacServer();
            }

            if (isAutoMode)
            {
                //in case user not set any proxysetter settings yet
                OnCoreRunningStatChangeHandler(null, EventArgs.Empty);

                StartTracking();
            }
            else
            {
                StopTracking();
            }

            InvokeOnSysProxyChange();
        }

        public void Cleanup()
        {
            ClearHotKey();
            lazyProxyUpdateTimer?.Release();
            StopTracking();
        }

        #endregion

        #region hotkey
        public void UpdateHotkey()
        {
            ClearHotKey();

            var bs = setting.GetBasicSetting();
            if (!bs.isUseHotkey)
            {
                return;
            }

            if (!Enum.TryParse(bs.hotkeyStr, out Keys hotkey))
            {
                setting.SendLog(I18N.ParseKeyCodeFail);
                VgcApis.Misc.UI.MsgBoxAsync(I18N.ParseKeyCodeFail);
                return;
            }

            void handler()
            {
                bs.sysProxyMode = (bs.sysProxyMode % 3) + 1;
                setting.SaveBasicSetting(bs);
                Restart();
            }

            hotKeyHandle = notifier.RegisterHotKey(
                handler,
                bs.hotkeyStr,
                bs.isUseAlt,
                true,
                bs.isUseShift
            );

            if (string.IsNullOrEmpty(hotKeyHandle))
            {
                setting.SendLog(I18N.RegistHotkeyFail);
                VgcApis.Misc.UI.MsgBoxAsync(I18N.RegistHotkeyFail);
            }
            else
            {
                setting.SendLog(I18N.RegHotKeySuccess);
            }
        }

        // https://stackoverflow.com/questions/2450373/set-global-hotkeys-using-c-sharp
        public void ClearHotKey()
        {
            if (!string.IsNullOrEmpty(hotKeyHandle))
            {
                var ok = notifier.UnregisterHotKey(hotKeyHandle);
                hotKeyHandle = null;
                setting.SendLog(string.Format(I18N.UnregisterHotKey, ok ? I18N.Done : I18N.Failed));
            }
        }
        #endregion

        #region private method
        void InvokeOnSysProxyChange()
        {
            try
            {
                OnSysProxyChanged?.Invoke(null, EventArgs.Empty);
                notifier?.RefreshNotifyIconLater();
            }
            catch { }
        }

        VgcApis.Libs.Tasks.CancelableTimeout lazyProxyUpdateTimer = null;

        void WakeupLazyProxyUpdater()
        {
            if (lazyProxyUpdateTimer == null)
            {
                lazyProxyUpdateTimer = new VgcApis.Libs.Tasks.CancelableTimeout(
                    LazyProxyUpdater,
                    2000
                );
            }
            lazyProxyUpdateTimer.Start();
        }

        void SearchForAvailableProxyServer(
            bool isGlobal,
            List<VgcApis.Interfaces.ICoreServCtrl> serverList
        )
        {
            foreach (var serv in serverList)
            {
                if (
                    serv.GetConfiger()
                        .IsSuitableToBeUsedAsSysProxy(isGlobal, out bool isSocks, out int port)
                )
                {
                    UpdateSysProxySetting(serv.GetCoreStates().GetTitle(), isSocks, port);
                    return;
                }
            }
            setting.SendLog(I18N.NoServerCapableOfSysProxy);
        }

        bool IsProxySettingChanged(bool isSocks, int port)
        {
            var bs = setting.GetBasicSetting();
            var isGlobal = bs.sysProxyMode == (int)Model.Data.Enum.SystemProxyModes.Global;

            if (!isGlobal)
            {
                var curPacProtoIsSocks = (
                    bs.pacProtocol == (int)Model.Data.Enum.PacProtocols.SOCKS
                );
                if (isSocks != curPacProtoIsSocks)
                {
                    return true;
                }
            }

            if (port != bs.proxyPort)
            {
                return true;
            }
            return false;
        }

        void UpdateSysProxySetting(string servTitle, bool isSocks, int port)
        {
            setting.SendLog(I18N.SysProxyChangeTo + " " + servTitle);
            if (!IsProxySettingChanged(isSocks, port))
            {
                setting.SendLog(I18N.SystemProxySettingRemain);
                return;
            }

            var bs = setting.GetBasicSetting();
            bs.proxyPort = VgcApis.Misc.Utils.Clamp(port, 0, 65536);
            if (bs.sysProxyMode == (int)Model.Data.Enum.SystemProxyModes.Global)
            {
                Libs.Sys.ProxySetter.SetGlobalProxy(port);
            }
            else
            {
                bs.pacProtocol = (int)(
                    isSocks ? Model.Data.Enum.PacProtocols.SOCKS : Model.Data.Enum.PacProtocols.HTTP
                );
                Libs.Sys.ProxySetter.SetPacProxy(pacServer.GetPacUrl());
            }

            setting.SaveBasicSetting(bs);
            setting.SendLog(I18N.SystemProxySettingUpdated);
            InvokeOnSysProxyChange();
        }

        void LazyProxyUpdater()
        {
            var serverList = servers.GetTrackableServerList();
            var isGlobal =
                setting.GetBasicSetting().sysProxyMode
                == (int)Model.Data.Enum.SystemProxyModes.Global;

            var curServ = serverList.FirstOrDefault(
                s => s.GetConfiger().GetConfig() == curServerConfig
            );
            if (curServ != null)
            {
                if (
                    curServ
                        .GetConfiger()
                        .IsSuitableToBeUsedAsSysProxy(isGlobal, out bool isSocks, out int port)
                )
                {
                    UpdateSysProxySetting(curServ.GetCoreStates().GetTitle(), isSocks, port);
                    return;
                }
            }
            SearchForAvailableProxyServer(isGlobal, serverList.ToList());
        }

        string curServerConfig;

        void OnCoreRunningStatChangeHandler(object sender, EventArgs args)
        {
            if (sender != null)
            {
                var coreCtrl = sender as VgcApis.Interfaces.ICoreServCtrl;
                curServerConfig = coreCtrl.GetConfiger().GetConfig();
            }
            else
            {
                curServerConfig = @"";
            }
            WakeupLazyProxyUpdater();
        }

        readonly object trackingLocker = new object();

        void StartTracking()
        {
            lock (trackingLocker)
            {
                if (isTracking)
                {
                    return;
                }

                servers.OnCoreStop += OnCoreRunningStatChangeHandler;
                servers.OnCoreStart += OnCoreRunningStatChangeHandler;
                isTracking = true;
            }
            setting.DebugLog("Start tracking.");
        }

        void StopTracking()
        {
            lock (trackingLocker)
            {
                if (!isTracking)
                {
                    return;
                }

                servers.OnCoreStop -= OnCoreRunningStatChangeHandler;
                servers.OnCoreStart -= OnCoreRunningStatChangeHandler;

                isTracking = false;
            }
            setting.DebugLog("Stop tracking.");
        }
        #endregion
    }
}
