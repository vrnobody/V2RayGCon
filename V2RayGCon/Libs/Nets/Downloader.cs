using System;
using System.IO;
using System.Net;
using V2RayGCon.Resources.Resx;

namespace V2RayGCon.Libs.Nets
{
    internal sealed class Downloader
    {
        public event EventHandler OnDownloadCompleted, OnDownloadCancelled, OnDownloadFail;
        public event EventHandler<VgcApis.Models.Datas.IntEvent> OnProgress;

        public enum CoreTypes
        {
            Xray,
            V2Ray,
        }

        public CoreTypes coreType = CoreTypes.V2Ray;
        string _packageName;
        string _version = @"v4.27.0";
        string _source = VgcApis.Models.Consts.Core.GetSourceUrlByIndex(0);
        readonly object waitForDigest = new object();

        public int proxyPort { get; set; } = -1;
        WebClient webClient;

        Services.Settings setting;

        public Downloader(Services.Settings setting)
        {
            this.setting = setting;
            SetArchitecture(false);
            webClient = null;
        }

        #region public method
        public void SetSource(int index)
        {
            _source = VgcApis.Models.Consts.Core.GetSourceUrlByIndex(index);
        }

        public void SetArchitecture(bool win64 = false)
        {
            var arch = win64 ? "64" : "32";
            _packageName = coreType == CoreTypes.Xray ?
                $"Xray-windows-{arch}.zip" :
                $"v2ray-windows-{arch}.zip";
        }

        public void SetVersion(string version)
        {
            _version = version;
        }

        public string GetPackageName()
        {
            return _packageName;
        }

        public void DownloadV2RayCore()
        {
            // debug
            /*
            {
                setting.SendLog("Debug: assume download completed");
                DownloadCompleted(false);
                return;
            }
            */

            Download();
        }

        public bool UnzipPackage()
        {
            var path = GetLocalFolderPath();
            var filename = GetLocalFilename();
            if (string.IsNullOrEmpty(path) || string.IsNullOrEmpty(filename))
            {
                setting.SendLog(I18N.LocateTargetFolderFail);
                return false;
            }

            VgcApis.Misc.Utils.Sleep(1000);
            try
            {
                RemoveOldExe(path);
                Misc.Utils.ZipFileDecompress(filename, path);
            }
            catch (Exception ex)
            {
                setting.SendLog(I18N.DecompressFileFail + Environment.NewLine + ex.ToString());
                return false;
            }
            return true;
        }

        public void Cleanup()
        {
            Cancel();
            webClient?.Dispose();
        }

        public void Cancel()
        {
            webClient?.CancelAsync();
        }
        #endregion

        #region private method
        void RemoveOldExe(string path)
        {
            string[] exes =
                new string[] {
                    // remove all cores to support switching between v2ray and xray
                    VgcApis.Models.Consts.Core.XrayCoreExeFileName,
                    VgcApis.Models.Consts.Core.V2RayCoreExeFileName,
                    VgcApis.Models.Consts.Core.V2RayCtlExeFileName,
                };

            string prefix = "bak";

            foreach (var exe in exes)
            {
                var newFn = Path.Combine(path, $"{prefix}.{exe}");
                if (File.Exists(newFn))
                {
                    try
                    {
                        File.Delete(newFn);
                    }
                    catch
                    {
                        var now = DateTime.Now.ToString("yyyy-MM-dd.HHmmss.ffff");
                        newFn = Path.Combine(path, $"{prefix}.{now}.{exe}");
                    }
                }

                var orgFn = Path.Combine(path, exe);
                if (File.Exists(orgFn))
                {
                    File.Move(orgFn, newFn);
                }
            }
        }

        void SendProgress(int percentage)
        {
            try
            {
                OnProgress?.Invoke(this,
                    new VgcApis.Models.Datas.IntEvent(Math.Max(1, percentage)));
            }
            catch { }
        }

        void NotifyDownloadResults(bool status)
        {
            try
            {
                if (status)
                {
                    OnDownloadCompleted?.Invoke(this, EventArgs.Empty);
                }
                else
                {
                    OnDownloadFail?.Invoke(this, EventArgs.Empty);
                }
            }
            catch { }
        }

        void UpdateCore()
        {
            var servers = Services.Servers.Instance;

            // var pluginServ = Services.PluginsServer.Instance;
            // pluginServ.StopAllPlugins();

            VgcApis.Misc.Utils.Sleep(1000);

            var activeServerList = servers.GetRunningServers();
            servers.StopAllServersThen(() =>
            {
                var status = UnzipPackage();
                NotifyDownloadResults(status);

                // pluginServ.RestartAllPlugins();

                if (activeServerList.Count > 0)
                {
                    servers.RestartServersThen(activeServerList);
                }
            });
        }

        void DownloadCompleted(bool cancelled)
        {
            webClient?.Dispose();
            webClient = null;

            if (cancelled)
            {
                try
                {
                    OnDownloadCancelled?.Invoke(this, EventArgs.Empty);
                }
                catch { }
                return;
            }

            setting.SendLog(string.Format("{0}", I18N.DownloadCompleted));
            UpdateCore();
        }

        string GetLocalFolderPath()
        {
            var path = setting.isPortable ?
                VgcApis.Misc.Utils.GetCoreFolderFullPath() :
                Misc.Utils.GetSysAppDataFolder();

            if (!Directory.Exists(path))
            {
                try
                {
                    Directory.CreateDirectory(path);
                }
                catch
                {
                    VgcApis.Misc.UI.MsgBoxAsync(I18N.CreateFolderFail);
                    return null;
                }
            }
            return path;
        }

        string GetLocalFilename()
        {
            var path = GetLocalFolderPath();
            return string.IsNullOrEmpty(path) ? null : Path.Combine(path, _packageName);
        }

        string GenReleaseUrl()
        {
            // tail =  "/releases/download/{0}/{1}";
            string tpl = _source + @"/download/{0}/{1}";
            return string.Format(tpl, _version, _packageName);
        }

        void Download()
        {
            string url = GenReleaseUrl();

            var filename = GetLocalFilename();
            if (string.IsNullOrEmpty(filename))
            {
                return;
            }

            webClient = new WebClient();
            webClient.Headers.Add(VgcApis.Models.Consts.Webs.UserAgent);

            if (proxyPort > 0)
            {
                webClient.Proxy = new WebProxy(
                    VgcApis.Models.Consts.Webs.LoopBackIP, proxyPort);
            }

            webClient.DownloadProgressChanged += (s, a) =>
            {
                SendProgress(a.ProgressPercentage);
            };

            webClient.DownloadFileCompleted += (s, a) =>
            {
                DownloadCompleted(a.Cancelled);
            };

            setting.SendLog(string.Format("{0}:{1}", I18N.Download, url));
            webClient.DownloadFileAsync(new Uri(url), filename);
        }

        #endregion
    }
}
