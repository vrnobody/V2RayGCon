using System;
using System.IO;
using System.Net;
using V2RayGCon.Resources.Resx;

namespace V2RayGCon.Libs.Nets
{
    internal sealed class Downloader
    {
        public event EventHandler OnDownloadCompleted,
            OnDownloadCancelled,
            OnDownloadFail;
        public event Action<int> OnProgress;

        public enum CoreTypes
        {
            Xray,
            V2Ray,
        }

        public string sourceUrl = VgcApis.Models.Consts.Core.GetSourceUrlByIndex(2);
        public string version = @"v1.8.18";

        readonly CoreTypes coreType = CoreTypes.Xray;
        string _packageName;

        public int proxyPort { get; set; } = -1;
        public bool isSocks5 { get; set; } = false;

        WebClient webClient;
        readonly Services.Settings setting;

        public Downloader(Services.Settings setting, CoreTypes coreType, bool is64bit)
        {
            this.setting = setting;
            this.coreType = coreType;
            UpdatePackageName(is64bit);
        }

        #region public method

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
            var dir = GetLocalFolderPath();
            var src = GetLocalFilename();
            if (string.IsNullOrEmpty(dir) || string.IsNullOrEmpty(src))
            {
                VgcApis.Misc.Logger.Log(I18N.LocateTargetFolderFail);
                return false;
            }

            VgcApis.Misc.Utils.Sleep(1000);
            try
            {
                var ok = Misc.Utils.VerifyZipFile(src);
                if (ok)
                {
                    RemoveOldExe(dir);
                    Misc.Utils.DecompressZipFile(src, dir);
                }
            }
            catch (Exception ex)
            {
                VgcApis.Misc.Logger.Log(
                    I18N.DecompressFileFail + Environment.NewLine + ex.ToString()
                );
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
            VgcApis.Misc.Utils.CancelWebClientAsync(webClient);
        }
        #endregion

        #region private method
        void UpdatePackageName(bool is64bit = false)
        {
            var arch = is64bit ? "64" : "32";
            var sys = setting.isDownloadWin7XrayCore ? "win7" : "windows";
            var core = coreType == CoreTypes.Xray ? "Xray" : "v2ray";
            _packageName = $"{core}-{sys}-{arch}.zip";
        }

        void RemoveOldExe(string path)
        {
            string[] exes = new string[]
            {
                // remove all cores to support switching between v2ray and xray
                VgcApis.Models.Consts.Core.XrayCoreExeFileName,
                VgcApis.Models.Consts.Core.V2RayCoreExeFileName,

                // statistic require v2ctl.exe
                // VgcApis.Models.Consts.Core.V2RayCtlExeFileName,
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
                OnProgress?.Invoke(percentage);
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
            VgcApis.Misc.Utils.Sleep(1000);

            var activeServerList = servers.GetRunningServers();
            servers.StopAllServersThen(() =>
            {
                var status = UnzipPackage();
                NotifyDownloadResults(status);
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

            VgcApis.Misc.Logger.Log(string.Format("{0}", I18N.DownloadCompleted));
            UpdateCore();
        }

        string GetLocalFolderPath()
        {
            var path = setting.isPortable
                ? VgcApis.Misc.Utils.GetCoreFolderFullPath()
                : Misc.Utils.GetSysAppDataFolder();

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
            string tpl = sourceUrl + @"/download/{0}/{1}";
            return string.Format(tpl, version, _packageName);
        }

        void Download()
        {
            string url = GenReleaseUrl();

            var filename = GetLocalFilename();
            if (string.IsNullOrEmpty(filename))
            {
                return;
            }

            var wc = VgcApis.Misc.Utils.CreateWebClient(
                url,
                isSocks5,
                VgcApis.Models.Consts.Webs.LoopBackIP,
                proxyPort,
                null,
                null
            );

            var maxPercentage = 0;
            wc.DownloadProgressChanged += (s, a) =>
            {
                var p = VgcApis.Misc.Utils.Clamp(a.ProgressPercentage, 1, 101);
                if (p > maxPercentage)
                {
                    maxPercentage = p;
                    SendProgress(p);
                }
            };

            wc.DownloadFileCompleted += (s, a) =>
            {
                DownloadCompleted(a.Cancelled);
            };

            webClient = wc;
            VgcApis.Misc.Logger.Log(string.Format("{0}:{1}", I18N.Download, url));
            wc.DownloadFileAsync(new Uri(url), filename);
        }

        #endregion
    }
}
