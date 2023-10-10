using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Mime;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using V2RayGCon.Resources.Resx;

namespace V2RayGCon.Views.Updater
{
    public partial class FormDownloader : Form
    {
        readonly Services.Settings settings = Services.Settings.Instance;
        readonly Services.Servers servers = Services.Servers.Instance;

        Models.Datas.UpdateInfo updateInfo = null;
        MyWebClient webClient = null;
        string tempFile = null;
        string downloadUrl = null;

        public FormDownloader()
        {
            InitializeComponent();
            VgcApis.Misc.UI.AutoSetFormIcon(this);
        }

        #region public methods
        internal void Init(Models.Datas.UpdateInfo info)
        {
            this.updateInfo = info;
            this.Show();

            this.downloadUrl = string.Format(
                VgcApis.Models.Consts.Webs.ReleaseDownloadUrlTpl,
                Misc.Utils.TrimVersionString(info.version)
            );

            lbSource.Text = downloadUrl;
            tempFile = Path.GetTempFileName();

            webClient = CreateWebClient();

            webClient.DownloadProgressChanged += OnDownloadProgressChanged;
            webClient.DownloadFileCompleted += OnDownloadFileCompleted;
            webClient.DownloadFileAsync(new Uri(downloadUrl), tempFile);
        }

        #endregion

        #region UI event handlers
        DateTime startTimeStamp;

        private void OnDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            if (startTimeStamp == default)
            {
                startTimeStamp = DateTime.Now;
            }
            else
            {
                TimeSpan timeSpan = DateTime.Now - startTimeStamp;
                var totalSeconds = (long)timeSpan.TotalSeconds;
                if (totalSeconds > 0)
                {
                    long bytesPerSecond = e.BytesReceived / totalSeconds;
                    lbSpeed.Text = string.Format(
                        I18N.DownloadSpeedMessage,
                        BytesToString(bytesPerSecond)
                    );
                }
            }

            lbSize.Text =
                $"{BytesToString(e.BytesReceived)} / {BytesToString(e.TotalBytesToReceive)}";
            pgBar.Value = e.ProgressPercentage;
        }

        private void OnDownloadFileCompleted(
            object sender,
            AsyncCompletedEventArgs asyncCompletedEventArgs
        )
        {
            if (asyncCompletedEventArgs.Cancelled)
            {
                DialogResult = DialogResult.Cancel;
                Close();
                return;
            }

            try
            {
                DialogResult = DialogResult.OK;
                if (asyncCompletedEventArgs.Error != null)
                {
                    throw asyncCompletedEventArgs.Error;
                }

                if (!Checksum())
                {
                    throw new WebException(I18N.CheckMd5SumFailed);
                }

                var tempFolder = Path.GetTempPath();
                // Try to parse the content disposition header if it exists.
                var zipFilePath = MoveTempDownloadFileToTempFolder(tempFolder);
                var installerPath = ExtractZipExtractor(tempFolder);
                var procInfo = CreateUpdateProcessInfo(zipFilePath, installerPath);

                try
                {
                    Process.Start(procInfo);
                }
                catch (Win32Exception exception)
                {
                    if (exception.NativeErrorCode == 1223)
                    {
                        webClient = null;
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            catch (Exception e)
            {
                VgcApis.Misc.UI.MsgBox(e.Message);
                this.DialogResult = DialogResult.Cancel;
            }
            finally
            {
                Close();
            }
        }

        void btnCancel_Click(object sender, EventArgs e)
        {
            if (VgcApis.Misc.UI.Confirm(I18N.CancelDownload))
            {
                VgcApis.Misc.Utils.CancelWebClientAsync(webClient);
            }
        }
        #endregion

        #region private methods
        ProcessStartInfo CreateUpdateProcessInfo(string zipFilePath, string installerPath)
        {
            string currentExe = Process.GetCurrentProcess().MainModule?.FileName;
            string extractionPath = Path.GetDirectoryName(currentExe);

            var pInfo = new ProcessStartInfo { FileName = installerPath, UseShellExecute = true };

            var arguments = new Collection<string>
            {
                "--input",
                zipFilePath,
                "--output",
                extractionPath,
                "--current-exe",
                currentExe,
            };

            pInfo.Arguments = BuildArguments(arguments);
            pInfo.Verb = "runas";

            return pInfo;
        }

        string ExtractZipExtractor(string tempPath)
        {
            string exe = Properties.Resources.UpdaterName;
            string dest = Path.Combine(tempPath, exe);
            string src = Path.Combine(Properties.Resources.BinsFolder, exe);
            File.Copy(src, dest, true);
            return dest;
        }

        string BuildArguments(Collection<string> argumentList)
        {
            var arguments = new StringBuilder();
            if (argumentList.Count <= 0)
            {
                return string.Empty;
            }

            foreach (string argument in argumentList)
            {
                PasteArguments.AppendArgument(ref arguments, argument);
            }
            return arguments.ToString();
        }

        string MoveTempDownloadFileToTempFolder(string tempFolder)
        {
            ContentDisposition contentDisposition = null;
            if (!string.IsNullOrWhiteSpace(webClient.ResponseHeaders?["Content-Disposition"]))
            {
                try
                {
                    contentDisposition = new ContentDisposition(
                        webClient.ResponseHeaders["Content-Disposition"]
                    );
                }
                catch (FormatException)
                {
                    // Ignore content disposition header if it is wrongly formatted.
                    contentDisposition = null;
                }
            }

            string fileName = string.IsNullOrEmpty(contentDisposition?.FileName)
                ? Path.GetFileName(webClient.ResponseUri.LocalPath)
                : contentDisposition.FileName;

            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new WebException(I18N.UnableToDetermineFilenameMessage);
            }

            var tempPath = Path.Combine(tempFolder, fileName);

            if (File.Exists(tempPath))
            {
                File.Delete(tempPath);
            }

            File.Move(tempFile, tempPath);
            return tempPath;
        }

        bool Checksum()
        {
            using (var hashAlgorithm = HashAlgorithm.Create("MD5"))
            {
                using (FileStream stream = File.OpenRead(tempFile))
                {
                    byte[] hash = hashAlgorithm.ComputeHash(stream);
                    string fileChecksum = BitConverter
                        .ToString(hash)
                        .Replace("-", string.Empty)
                        .ToLowerInvariant();
                    if (fileChecksum == updateInfo.md5.ToLowerInvariant())
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        string BytesToString(long byteCount)
        {
            string[] suf = { "B", "KiB", "MiB", "GiB", "TiB", "PiB", "EiB" };
            if (byteCount == 0)
            {
                return "0" + suf[0];
            }

            long bytes = Math.Abs(byteCount);
            var place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
            double num = Math.Round(bytes / Math.Pow(1024, place), 1);
            return $"{(Math.Sign(byteCount) * num).ToString(System.Globalization.CultureInfo.InvariantCulture)} {suf[place]}";
        }

        MyWebClient CreateWebClient()
        {
            var port = -1;
            if (settings.isUpdateUseProxy)
            {
                port = servers.GetAvailableHttpProxyPort();
            }

            MyWebClient wc = new MyWebClient { Encoding = Encoding.UTF8, };

            wc.Headers.Add(VgcApis.Models.Consts.Webs.UserAgent);

            if (port > 0 && port < 65536)
            {
                wc.Proxy = new WebProxy(VgcApis.Models.Consts.Webs.LoopBackIP, port);
            }

            return wc;
        }

        #endregion
    }
}
