using ProxySetter.Resources.Langs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace ProxySetter.Services
{
    internal class TunaServer : IDisposable
    {
        public event Action<bool> onChanged;

        Process proc = null;

        PsSettings settings;
        VgcApis.Interfaces.Services.ISettingsService vgcSettings;
        VgcApis.Interfaces.Services.IServersService vgcServers;
        VgcApis.Interfaces.Services.INotifierService vgcNotifier;

        public TunaServer() { }

        public void Run(VgcApis.Interfaces.Services.IApiService vgcApi, PsSettings settings)
        {
            this.settings = settings;
            vgcSettings = vgcApi.GetSettingService();
            vgcServers = vgcApi.GetServersService();
            vgcNotifier = vgcApi.GetNotifierService();
        }

        #region IDisposable
        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)
                    Kill(1000);
                }

                // TODO: 释放未托管的资源(未托管的对象)并重写终结器
                // TODO: 将大型字段设置为 null
                disposedValue = true;
            }
        }

        // // TODO: 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
        // ~TunaServer()
        // {
        //     // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion

        #region properties
        bool _isRunning = false;
        public bool isRunning
        {
            get => _isRunning;
            set
            {
                if (_isRunning == value)
                {
                    return;
                }
                _isRunning = value;
                VgcApis.Misc.UI.Invoke(() => menu.Checked = _isRunning);
                vgcSettings.isTunMode = _isRunning;
                settings.SendLog(_isRunning ? I18N.TunaStarts : I18N.TunaStopped);
                try
                {
                    onChanged?.Invoke(_isRunning);
                }
                catch { }
                vgcNotifier.RefreshNotifyIconLater();
            }
        }
        #endregion

        #region public methods
        ToolStripMenuItem menu = null;

        public ToolStripMenuItem GetMenu()
        {
            if (menu == null)
            {
                menu = new ToolStripMenuItem(
                    "Tuna",
                    null,
                    (s, a) =>
                    {
                        if (isRunning)
                        {
                            Stop();
                        }
                        else
                        {
                            Start();
                        }
                    }
                );
            }
            menu.Checked = isRunning;
            return menu;
        }

        public void Stop()
        {
            if (!isRunning)
            {
                return;
            }

            Kill(3000);
            vgcSettings.SetSendThroughIpv4("");
        }

        public void Start()
        {
            if (isRunning)
            {
                return;
            }

            var ts = settings.GetTunaSettings();
            if (!File.Exists(ts.exe))
            {
                var msg = string.Format(I18N.ExeNotFound, ts.exe);
                VgcApis.Misc.UI.MsgBox(msg);
                return;
            }

            if (string.IsNullOrEmpty(ts.startupScript))
            {
                VgcApis.Misc.UI.MsgBox(I18N.PleaseConfigTunaFirst);
                return;
            }

            try
            {
                if (ts.autoGenArgs && UpdateTunaSettings(ts))
                {
                    settings.SaveTunaSettings(ts);
                }
                proc = CreateTunProcess(ts);
                proc.Start();
                isRunning = true;

                if (ts.isModifySendThrough)
                {
                    ModifySendThrough(ts);
                }
            }
            catch (Exception ex)
            {
                settings.SendLog(ex.ToString());
            }
        }

        public bool UpdateTunaSettings(Model.Data.TunaSettings ts)
        {
            var nic = GetNicInfoFromRoutes(ts.tunIpv4);
            if (nic == null)
            {
                return false;
            }

            ts.nicIpv4 = nic.IPv4;

            if (string.IsNullOrEmpty(ts.proxy))
            {
                ts.proxy = GetProxyInfo();
            }

            var parts = ts.nicIpv4?.Split('.');
            if (parts != null && parts.Count() == 4)
            {
                var subnet = (VgcApis.Misc.Utils.Str2Int(parts[2]) + 20) % 256;
                parts[2] = subnet.ToString();
                if (string.IsNullOrEmpty(ts.tunIpv4))
                {
                    ts.tunIpv4 = string.Join(".", parts);
                }
                if (string.IsNullOrEmpty(ts.tunIpv6))
                {
                    ts.tunIpv6 = $"fcba:{subnet}::{parts[3]}";
                }
            }

            var metric = Math.Max(1, nic.metric / 2);
            ts.startupScript = GenStartupScript(ts, metric);
            return true;
        }

        #endregion

        #region private methods
        void ModifySendThrough(Model.Data.TunaSettings ts)
        {
            var changed = vgcSettings.SetSendThroughIpv4(ts.nicIpv4);
            if (!changed)
            {
                return;
            }

            var servs = vgcServers
                .GetAllServersOrderByIndex()
                .Where(s => s.GetCoreCtrl().IsCoreRunning())
                .ToList();
            foreach (var serv in servs)
            {
                serv.GetCoreCtrl().RestartCore();
            }
        }

        string GetProxyInfo()
        {
            var def = "socks5://127.0.0.1:1080";

            var inbs = vgcServers
                .GetAllServersOrderByIndex()
                .Where(s => s.GetCoreCtrl().IsCoreRunning())
                .SelectMany(cs => cs.GetConfiger().GetAllInboundsInfo())
                .ToList();

            var info =
                inbs.FirstOrDefault(inb => inb.protocol == "socks")
                ?? inbs.FirstOrDefault(inb => inb.protocol == "http");

            if (info == null)
            {
                return def;
            }

            var proto = info.protocol == "socks" ? "socks5" : info.protocol;
            var host = VgcApis.Misc.Utils.FormatHost(info.host);
            return $"{proto}://{host}:{info.port}";
        }

        NicInfos GetNicInfoFromRoutes(string tunIpv4)
        {
            var ipv4 = VgcApis.Misc.Utils
                .ExecuteAndGetStdOut("route", "print -4", 5000, System.Text.Encoding.Default)
                ?.Replace("\r", "")
                .Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(
                    line => line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                )
                .Where(
                    parts =>
                        parts.Count() == 5
                        && parts[0] == "0.0.0.0"
                        && parts[1] == parts[0]
                        && parts[3] != tunIpv4
                )
                .OrderBy(parts => VgcApis.Misc.Utils.Str2Int(parts[4]))
                .FirstOrDefault();

            if (ipv4 == null)
            {
                return null;
            }

            return new NicInfos { IPv4 = ipv4[3], metric = VgcApis.Misc.Utils.Str2Int(ipv4[4]) };
        }

        string GenStartupScript(Model.Data.TunaSettings ts, int metric)
        {
            var setTunIpv4 =
                $"netsh interface ipv4 set address name={ts.tunName} source=static addr={ts.tunIpv4} mask=255.255.255.0 gateway=none";

            var setTunIpv6 = $"netsh interface ipv6 add address name={ts.tunName} {ts.tunIpv6}";

            var setRoute4 =
                $"netsh interface ipv4 add route 0.0.0.0/0 {ts.tunName} {ts.tunIpv4} metric={metric} store=active";

            var setRoute6 =
                $"netsh interface ipv6 add route ::/0 interface={ts.tunName} {ts.tunIpv6} metric={metric} store=active";

            var dnses =
                ts.dns
                    ?.Replace("\r", "")
                    .Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries)
                ?? new string[0];

            var list = new List<string>();
            var tpl =
                $"&& netsh interface {{0}} set dnsservers name={ts.tunName} source=static {{1}} primary";
            foreach (var dns in dnses)
            {
                var isIpv6 = VgcApis.Misc.Utils.IsIpv6(dns);
                if (!isIpv6 || ts.isEnableIpv6)
                {
                    var ipType = isIpv6 ? "ipv6" : "ipv4";
                    list.Add(string.Format(tpl, ipType, dns));
                }
            }
            var setTunDns = string.Join("\n", list);

            var cmds = new List<string>()
            {
                $"-device=\"{ts.tunName}\"",
                $"-proxy=\"{ts.proxy}\"",
                $"-tun-post-up=\"cmd /c",
                $"{setTunIpv4}",
            };
            if (ts.isEnableIpv6)
            {
                cmds.Add($"&& {setTunIpv6}");
            }
            if (!string.IsNullOrEmpty(setTunDns))
            {
                cmds.Add($"{setTunDns}");
            }
            cmds.Add($"&& {setRoute4}");
            if (ts.isEnableIpv6)
            {
                cmds.Add($"&& {setRoute6}");
            }
            return string.Join("\n", cmds) + "\"";
        }

        Process CreateTunProcess(Model.Data.TunaSettings tunaSettings)
        {
            var args = tunaSettings.startupScript.Replace("\r", "").Replace("\n", " ");
            var path = Path.GetFullPath(tunaSettings.exe);
            var dir = Path.GetDirectoryName(path);

            var p = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    Verb = "runas",
                    FileName = path,
                    Arguments = args,
                    CreateNoWindow = !tunaSettings.isDebug,
                    UseShellExecute = true,
                    WorkingDirectory = dir,
                },
                EnableRaisingEvents = true,
            };

            if (!tunaSettings.isDebug)
            {
                p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            }

            p.Exited += (s, a) => isRunning = false;

            return p;
        }

        void Kill(int ms)
        {
            var p = proc;
            if (p == null)
            {
                return;
            }
            try
            {
                VgcApis.Misc.Utils.SendStopSignal(p);
                p.WaitForExit(ms);
                if (!p.HasExited)
                {
                    p.Kill();
                }
            }
            catch { }
            proc = null;
        }

        #endregion

        #region protected methods

        #endregion
    }

    class NicInfos
    {
        public string IPv4 = "";
        public int metric = 20;
    }
}
