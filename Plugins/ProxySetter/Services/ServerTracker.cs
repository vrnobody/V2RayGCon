using ProxySetter.Resources.Langs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using VgcApis.Libs.Sys;

namespace ProxySetter.Services
{
    class ServerTracker
    {
        PsSettings setting;
        PacServer pacServer;

        VgcApis.Models.IServices.IServersService servers;
        VgcApis.Models.IServices.INotifierService notifier;

        public event EventHandler OnSysProxyChanged;
        bool isTracking { get; set; }
        KeyboardHook kbHook = null;

        public ServerTracker()
        {
            isTracking = false;
        }

        #region public method
        public void Run(
            PsSettings setting,
            PacServer pacServer,
            VgcApis.Models.IServices.IServersService servers,
            VgcApis.Models.IServices.INotifierService notifier)
        {
            this.setting = setting;
            this.pacServer = pacServer;
            this.servers = servers;
            this.notifier = notifier;

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
                    Lib.Sys.ProxySetter.SetGlobalProxy(bs.proxyPort);
                    break;
                case Model.Data.Enum.SystemProxyModes.PAC:
                    isStartPacServer = true;
                    Lib.Sys.ProxySetter.SetPacProxy(pacServer.GetPacUrl());
                    break;
                case Model.Data.Enum.SystemProxyModes.Direct:
                    isAutoMode = false;
                    Lib.Sys.ProxySetter.ClearSysProxy();
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

            RegistHotKey();
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

        // https://stackoverflow.com/questions/2450373/set-global-hotkeys-using-c-sharp
        void ClearHotKey()
        {
            kbHook?.Dispose();
            kbHook = null;
        }

        void RegistHotKey()
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
                VgcApis.Libs.UI.MsgBoxAsync(I18N.ParseKeyCodeFail);
                return;
            }

            kbHook = new KeyboardHook();
            kbHook.KeyPressed += new EventHandler<KeyPressedEventArgs>(HotkeyHandler);
            ModifierKeys modifier = ModifierKeys.Control;
            if (bs.isUseAlt)
            {
                modifier |= ModifierKeys.Alt;
            }
            if (bs.isUseShift)
            {
                modifier |= ModifierKeys.Shift;
            }

            try
            {
                kbHook.RegisterHotKey((uint)modifier, (uint)hotkey);
                return;
            }
            catch { }

            setting.SendLog(I18N.RegistHotkeyFail);
            VgcApis.Libs.UI.MsgBoxAsync(I18N.RegistHotkeyFail);
        }

        void HotkeyHandler(object sender, KeyPressedEventArgs e)
        {
            var bs = setting.GetBasicSetting();
            bs.sysProxyMode = (bs.sysProxyMode % 3) + 1;
            setting.SaveBasicSetting(bs);
            Restart();
        }

        #endregion

        #region private method
        void InvokeOnSysProxyChange()
        {
            try
            {
                OnSysProxyChanged?.Invoke(null, EventArgs.Empty);
                notifier?.RefreshNotifyIcon();
            }
            catch { }
        }

        Lib.Sys.CancelableTimeout lazyProxyUpdateTimer = null;
        void WakeupLazyProxyUpdater()
        {
            if (lazyProxyUpdateTimer == null)
            {
                lazyProxyUpdateTimer =
                    new Lib.Sys.CancelableTimeout(
                        LazyProxyUpdater, 2000);
            }
            lazyProxyUpdateTimer.Start();
        }

        void SearchForAvailableProxyServer(
            bool isGlobal,
            List<VgcApis.Models.Interfaces.ICoreServCtrl> serverList)
        {
            foreach (var serv in serverList)
            {
                if (serv.GetConfiger().IsSuitableToBeUsedAsSysProxy(
                   isGlobal, out bool isSocks, out int port))
                {
                    UpdateSysProxySetting(
                        serv.GetCoreStates().GetTitle(),
                        isSocks,
                        port);
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
                var curPacProtoIsSocks = (bs.pacProtocol == (int)Model.Data.Enum.PacProtocols.SOCKS);
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
            bs.proxyPort = VgcApis.Libs.Utils.Clamp(port, 0, 65536);
            if (bs.sysProxyMode == (int)Model.Data.Enum.SystemProxyModes.Global)
            {
                Lib.Sys.ProxySetter.SetGlobalProxy(port);
            }
            else
            {
                bs.pacProtocol =
                    (int)(isSocks ?
                    Model.Data.Enum.PacProtocols.SOCKS :
                    Model.Data.Enum.PacProtocols.HTTP);
                Lib.Sys.ProxySetter.SetPacProxy(pacServer.GetPacUrl());
            }

            setting.SaveBasicSetting(bs);
            setting.SendLog(I18N.SystemProxySettingUpdated);
            InvokeOnSysProxyChange();
        }

        void LazyProxyUpdater()
        {
            var serverList = servers.GetTrackableServerList();
            var isGlobal =
                    setting.GetBasicSetting().sysProxyMode ==
                    (int)Model.Data.Enum.SystemProxyModes.Global;

            var curServ = serverList.FirstOrDefault(s => s.GetConfiger().GetConfig() == curServerConfig);
            if (curServ != null)
            {
                if (curServ.GetConfiger().IsSuitableToBeUsedAsSysProxy(
                    isGlobal, out bool isSocks, out int port))
                {
                    UpdateSysProxySetting(
                        curServ.GetCoreStates().GetTitle(),
                        isSocks,
                        port);
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
                var coreCtrl = sender as VgcApis.Models.Interfaces.ICoreServCtrl;
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
