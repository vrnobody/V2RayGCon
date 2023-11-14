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

        public TunaServer() { }

        public void Run(VgcApis.Interfaces.Services.IApiService vgcApi, PsSettings settings)
        {
            this.settings = settings;
            vgcSettings = vgcApi.GetSettingService();
            vgcServers = vgcApi.GetServersService();
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
                settings.SendLog(_isRunning ? I18N.TunaStarts : I18N.TunaStopped);
                try
                {
                    onChanged?.Invoke(_isRunning);
                }
                catch { }
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
            vgcSettings.SetSendThrough("");
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

                if (vgcSettings.SetSendThrough(ts.nicIp))
                {
                    var servs = vgcServers
                        .GetAllServersOrderByIndex()
                        .Where(s => s.GetCoreCtrl().IsCoreRunning())
                        .ToList();
                    foreach (var serv in servs)
                    {
                        serv.GetCoreCtrl().RestartCore();
                    }
                }
            }
            catch (Exception ex)
            {
                settings.SendLog(ex.ToString());
            }
        }

        public bool UpdateTunaSettings(Model.Data.TunaSettings ts)
        {
            var first = GetFirstRoute(ts.tunIp);

            if (first == null)
            {
                return false;
            }
            UpdateTunaSettingCore(ts, first[3], first[4]);
            return true;
        }

        #endregion

        #region private methods
        string GetProxyInfo()
        {
            var def = "socks5://127.0.0.1:1080";

            var first = vgcServers
                .GetAllServersOrderByIndex()
                .FirstOrDefault(s => s.GetCoreCtrl().IsCoreRunning());

            if (first == null)
            {
                return def;
            }

            var inbs = first.GetConfiger().GetAllInboundsInfo();
            var info = inbs.FirstOrDefault(inb => inb.protocol == "socks");
            if (info == null)
            {
                info = inbs.FirstOrDefault(inb => inb.protocol == "http");
            }

            if (info == null)
            {
                return def;
            }

            var proto = info.protocol == "socks" ? "socks5" : info.protocol;
            var host = VgcApis.Misc.Utils.FormatHost(info.host);
            return $"{proto}://{host}:{info.port}";
        }

        string[] GetFirstRoute(string tunIp)
        {
            var s = VgcApis.Misc.Utils.ExecuteAndGetStdOut(
                "route",
                "print -4",
                5000,
                System.Text.Encoding.Default
            );

            var first = s?.Replace("\r", "")
                .Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(
                    line => line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                )
                .Where(
                    parts =>
                        parts.Count() == 5
                        && parts[0] == parts[1]
                        && parts[0] == "0.0.0.0"
                        && parts[3] != tunIp
                )
                .OrderBy(parts => VgcApis.Misc.Utils.Str2Int(parts[4]))
                .FirstOrDefault();

            return first;
        }

        void UpdateTunaSettingCore(Model.Data.TunaSettings ts, string nicIp, string metric)
        {
            ts.nicIp = nicIp;
            var m = Math.Max(1, VgcApis.Misc.Utils.Str2Int(metric) / 2);

            if (string.IsNullOrEmpty(ts.tunIp))
            {
                var parts = ts.nicIp?.Split('.');
                if (parts != null && parts.Count() == 4)
                {
                    var subnet = (VgcApis.Misc.Utils.Str2Int(parts[2]) + 20) % 256;
                    parts[2] = subnet.ToString();
                    ts.tunIp = string.Join(".", parts);
                }
            }

            if (string.IsNullOrEmpty(ts.proxy))
            {
                ts.proxy = GetProxyInfo();
            }

            var setTunIp =
                $"netsh interface ipv4 set address name={ts.tunName} source=static addr={ts.tunIp} mask=255.255.255.0 gateway=none";

            var setTunDns =
                $"netsh interface ipv4 set dns name={ts.tunName} source=static {ts.dns} primary";

            var setRouteTable =
                $"netsh interface ipv4 add route 0.0.0.0/0 {ts.tunName} {ts.tunIp} metric={m} store=active";

            ts.startupScript =
                $"-device=\"{ts.tunName}\" \n"
                + $"-proxy=\"{ts.proxy}\" \n"
                + $"-tun-post-up=\"cmd /c \n"
                + $" {setTunIp}\n"
                + (string.IsNullOrEmpty(ts.dns) ? "" : $" && {setTunDns}\n")
                + $" && {setRouteTable}\"";
        }

        Process CreateTunProcess(Model.Data.TunaSettings tunaSettings)
        {
            var args = tunaSettings.startupScript?.Replace("\n", "");
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
}
