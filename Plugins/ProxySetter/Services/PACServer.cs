using ProxySetter.Resources.Langs;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProxySetter.Services
{
    class PacServer
    {
        Services.PsSettings setting;
        Lib.Nets.PacGenerator pacGenerator;

        public event EventHandler OnPACServerStateChanged;

        string customPacFileCache = string.Empty;
        FileSystemWatcher customPacFileWatcher = null;

        readonly object webServerLock = new object();
        HttpListener webListener = null;
        Lib.Sys.CancelableTask webResponser = null;

        public PacServer() { }

        public void Run(PsSettings setting)
        {
            this.setting = setting;

            this.pacGenerator = new Lib.Nets.PacGenerator();
            this.pacGenerator.Run();

            // let serverTracker handle this
            // Reload();
        }

        #region properties
        bool _isRunning = false;
        public bool isRunning
        {
            get
            {
                return _isRunning;
            }
            private set
            {
                if (_isRunning == value)
                {
                    return;
                }
                _isRunning = value;
                LazyInvokeOnPacServerStateChange();
            }
        }
        #endregion

        #region public method
        public void Reload()
        {
            setting.DebugLog("Reload pac server");
            StopPacServer();
            ClearDefaultPacCache();
            RestartPacServer();
        }

        public void StartPacServer()
        {
            lock (webServerLock)
            {
                if (isRunning)
                {
                    return;
                }
                RestartPacServer();
            }
        }

        public void RestartPacServer()
        {
            var bs = setting.GetBasicSetting();
            var prefix = GenPrefix(bs.pacServPort);

            lock (webServerLock)
            {
                if (isRunning)
                {
                    StopPacServer();
                }

                if (!StartListining(prefix))
                {
                    isRunning = false;
                    return;
                }
                isRunning = true;
            }

            StopCustomPacFileWatcher();
            if (bs.isUseCustomPac)
            {
                LazyCustomPacFileCacheUpdate();
                StartFileWatcher(bs.customPacFileName);
            }
        }

        public string GetCurPacFileContent()
        {
            var bs = setting.GetBasicSetting();
            var url = VgcApis.Models.Consts.Webs.FakeRequestUrl;
            var reqParams = GenReqParamFromUrl(url, bs);

            var pacResp = pacGenerator.GenPacFileResponse(
                bs.isUseCustomPac,
                reqParams,
                setting.GetCustomPacSetting(),
                customPacFileCache);

            return pacResp.Item1;
        }

        public void Cleanup()
        {
            StopPacServer();
            lazyStateChangeTimer?.Release();
            lazyCustomPacFileCacheUpdateTimer?.Release();
        }

        public string GetPacUrl()
        {
            var bs = setting.GetBasicSetting();
            return GenPrefix(bs.pacServPort);
        }

        public void StopPacServer()
        {
            lock (webServerLock)
            {
                if (!isRunning)
                {
                    return;
                }

                StopListening();
                ClearDefaultPacCache();
                StopCustomPacFileWatcher();
                isRunning = false;
            }
        }

        private void StopListening()
        {
            try
            {
                webResponser?.Stop();
            }
            catch { }
            webResponser = null;

            try
            {
                webListener?.Abort();
            }
            catch { }
            webListener = null;
        }
        #endregion

        #region private method
        void StartFileWatcher(string relativeFileName)
        {

            if (!File.Exists(relativeFileName))
            {
                return;
            }

            var path = Path.GetDirectoryName(relativeFileName);

            customPacFileWatcher = new FileSystemWatcher
            {
                Path = (string.IsNullOrEmpty(path) ? VgcApis.Libs.Utils.GetAppDir() : path),
                Filter = Path.GetFileName(relativeFileName),
            };

            customPacFileWatcher.Changed += (s, a) => LazyCustomPacFileCacheUpdate();
            customPacFileWatcher.Created += (s, a) => LazyCustomPacFileCacheUpdate();
            customPacFileWatcher.Deleted += (s, a) => LazyCustomPacFileCacheUpdate();

            customPacFileWatcher.EnableRaisingEvents = true;
        }

        Lib.Sys.CancelableTimeout lazyCustomPacFileCacheUpdateTimer = null;
        void LazyCustomPacFileCacheUpdate()
        {
            if (lazyCustomPacFileCacheUpdateTimer == null)
            {
                lazyCustomPacFileCacheUpdateTimer =
                    new Lib.Sys.CancelableTimeout(
                        UpdateCustomPacCache, 2000);
            }

            lazyCustomPacFileCacheUpdateTimer.Start();
        }

        void UpdateCustomPacCache()
        {
            var bs = setting.GetBasicSetting();
            var fileName = bs.customPacFileName;

            customPacFileCache = string.Empty;
            if (File.Exists(fileName))
            {
                try
                {
                    var content = File.ReadAllText(fileName);
                    customPacFileCache = content ?? string.Empty;
                }
                catch { }
            }

            Lib.Sys.ProxySetter.SetPacProxy(GetPacUrl());
            setting.SendLog(I18N.SystemProxySettingUpdated);
        }


        bool StartListining(string prefix)
        {
            webListener = new HttpListener
            {
                IgnoreWriteExceptions = true,
                Prefixes = { prefix }
            };

            try
            {
                webListener.Start();
            }
            catch
            {
                VgcApis.Libs.Utils.RunInBackground(
                    () => MessageBox.Show(I18N.StartPacServFail));
                return false;
            }

            webResponser =
                new Lib.Sys.CancelableTask(
                    WebResponseWorker);
            webResponser.Start();
            return true;
        }

        void WebResponseWorker()
        {
            while (true)
            {
                try
                {
                    WebRequestDispatcher(
                        webListener.GetContext());
                }
                catch (System.Threading.ThreadAbortException)
                {
                    break;
                }
                catch (ObjectDisposedException)
                {
                    break;
                }
                catch
                {
                    // ignore other exceptions
                }
            }
        }

        void Write(HttpListenerResponse response, string html, string mime)
        {
            if (!string.IsNullOrEmpty(mime))
            {
                response.ContentType = mime;
            }

            response.ContentLength64 = Encoding.UTF8.GetByteCount(html ?? "error");
            using (var writer = new StreamWriter(response.OutputStream))
            {
                writer.Write(html);
            }
        }

        void StopCustomPacFileWatcher()
        {
            if (customPacFileWatcher == null)
            {
                return;
            }
            customPacFileWatcher.EnableRaisingEvents = false;
            customPacFileWatcher.Dispose();
            customPacFileWatcher = null;
        }

        Lib.Sys.CancelableTimeout lazyStateChangeTimer = null;
        void LazyInvokeOnPacServerStateChange()
        {
            // Create on demand.
            if (lazyStateChangeTimer == null)
            {
                lazyStateChangeTimer = new Lib.Sys.CancelableTimeout(
                    () =>
                    {
                        try
                        {
                            OnPACServerStateChanged?.Invoke(this, EventArgs.Empty);
                        }
                        catch { }
                    },
                    1000);
            }
            lazyStateChangeTimer.Start();
        }

        static void ClearDefaultPacCache()
        {
            Lib.Nets.PacGenerator.ClearCache();
        }

        static string GenPrefix(int port)
        {
            return string.Format("http://localhost:{0}/pac/", port);
        }

        void WebRequestDispatcher(HttpListenerContext context)
        {
            var bs = setting.GetBasicSetting();
            var request = context.Request;
            var reqParams = GenReqParamFromUrl(request.Url.AbsoluteUri, bs);

            var pacResp = pacGenerator.GenPacFileResponse(
                    bs.isUseCustomPac,
                    reqParams,
                    setting.GetCustomPacSetting(),
                    customPacFileCache);

            var response = context.Response;
            if (!reqParams.isDebug)
            {
                Write(response, pacResp.Item1, pacResp.Item2);
                return;
            }

            var debugResp = GenPacDebuggerResponse(
                (reqParams.isWhiteList ? "white" : "black"),
                GenPrefix(bs.pacServPort),
                pacResp.Item1);

            Write(response, debugResp.Item1, debugResp.Item2);
        }

        private static Model.Data.PacUrlParams GenReqParamFromUrl(
            string reqUrl,
            Model.Data.BasicSettings bs)
        {
            // e.g. http://localhost:3000/pac/?&port=5678&ip=1.2.3.4&proto=socks&type=whitelist&key=rnd
            var queryParams = Lib.Utils.GetQureryParamsFrom(reqUrl);
            var defValues = new Model.Data.QueryParams(bs);
            queryParams.ReplaceNullValueWith(defValues);
            var pacParams = queryParams.ToPacUrlParams();
            return pacParams;
        }

        static Tuple<string, string> GenPacDebuggerResponse(
            string mode, string url, string pacContent)
        {

            var html = StrConst.PacDebuggerTpl
                .Replace("__PacMode__", mode)
                .Replace("__PacServerUrl__", url)
                .Replace("__PACFileContent__", pacContent);
            var mime = "text/html; charset=UTF-8";
            return new Tuple<string, string>(html, mime);
        }
        #endregion
    }
}
