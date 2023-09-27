using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using V2RayGCon.Resources.Resx;

namespace V2RayGCon.Views.WinForms
{
    public partial class FormDownloadCore : Form
    {
        #region Sigleton
        static FormDownloadCore _instant;

        public static FormDownloadCore ShowForm()
        {
            if (_instant == null || _instant.IsDisposed)
            {
                VgcApis.Misc.UI.Invoke(() =>
                {
                    _instant = new FormDownloadCore();
                    _instant.FormClosed += (s, a) => _instant = null;
                });
            }
            VgcApis.Misc.UI.Invoke(() => _instant.Show());
            return _instant;
        }
        #endregion

        Libs.Nets.Downloader downloader;
        readonly Services.Settings setting;
        readonly Services.Servers servers;

        FormDownloadCore()
        {
            setting = Services.Settings.Instance;
            servers = Services.Servers.Instance;

            InitializeComponent();
            InitUI();

            this.FormClosed += (s, e) =>
            {
                downloader?.Cleanup();
            };

            VgcApis.Misc.UI.AutoSetFormIcon(this);
        }

        private void FormDownloadCore_Shown(object sender, EventArgs e)
        {
            RefreshV2RayCoreSourceUrls();
            RefreshLocalV2RayCoreVersion();
            chkUseProxy.Checked = setting.isUpdateUseProxy;
        }

        #region private methods


        void RefreshV2RayCoreSourceUrls()
        {
            var urls = VgcApis.Models.Consts.Core.SourceUrls;
            var items = cboxDownloadSource.Items;
            items.Clear();
            items.AddRange(urls);
            VgcApis.Misc.UI.ResetComboBoxDropdownMenuWidth(cboxDownloadSource);
            var url = setting.v2rayCoreDownloadSource;
            var index = VgcApis.Models.Consts.Core.GetIndexBySourceUrl(url);
            cboxDownloadSource.SelectedIndex = index;
        }

        void RefreshLocalV2RayCoreVersion()
        {
            var el = labelCoreVersion;

            VgcApis.Misc.Utils.RunInBgSlim(() =>
            {
                var core = new Libs.V2Ray.Core(setting);
                var version = core.GetV2RayCoreVersion();
                var msg = string.IsNullOrEmpty(version)
                    ? I18N.GetCoreVerFail
                    : string.Format(I18N.CurrentCoreVerIs, version);

                VgcApis.Misc.UI.Invoke(() => el.Text = msg);
            });
        }

        void UpdateProgressBar(int percentage)
        {
            var v = Misc.Utils.Clamp(percentage, 1, 101);
            VgcApis.Misc.UI.Invoke(() => pgBarDownload.Value = v);
        }

        void EnableBtnDownload()
        {
            VgcApis.Misc.UI.Invoke(() => btnDownload.Enabled = true);
        }

        void DownloadV2RayCore(int proxyPort)
        {
            var idx = cboxDownloadSource.SelectedIndex;

            downloader = new Libs.Nets.Downloader(setting)
            {
                coreType =
                    idx == 2
                        ? Libs.Nets.Downloader.CoreTypes.Xray
                        : Libs.Nets.Downloader.CoreTypes.V2Ray
            };
            downloader.SetSource(idx);
            downloader.SetArchitecture(cboxArch.SelectedIndex == 1);
            downloader.SetVersion(cboxVer.Text);
            downloader.proxyPort = proxyPort;

            downloader.OnProgress += (p) => UpdateProgressBar(p);

            downloader.OnDownloadCompleted += (s, a) =>
            {
                ResetUI(100);
                VgcApis.Misc.UI.MsgBoxAsync(I18N.DownloadCompleted);
                VgcApis.Misc.Utils.Sleep(1000);
                VgcApis.Misc.UI.Invoke(() => btnCheckVersion.PerformClick());
            };

            downloader.OnDownloadCancelled += (s, a) =>
            {
                ResetUI(0);
                VgcApis.Misc.UI.MsgBoxAsync(I18N.DownloadCancelled);
            };

            downloader.OnDownloadFail += (s, a) =>
            {
                ResetUI(0);
                VgcApis.Misc.UI.MsgBoxAsync(I18N.TryManualDownload);
            };

            downloader.DownloadV2RayCore();
            UpdateProgressBar(1);
        }

        #endregion

        #region UI
        void ResetUI(int progress)
        {
            UpdateProgressBar(progress);
            downloader = null;
            EnableBtnDownload();
        }

        void InitUI()
        {
            cboxArch.SelectedIndex = setting.isDownloadWin32V2RayCore ? 0 : 1;

            var verList = setting.GetV2RayCoreVersionList();
            Misc.UI.FillComboBox(cboxVer, new List<string>(verList));
            pgBarDownload.Value = 0;
        }

        private void BtnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void BtnRefreshVer_Click(object sender, EventArgs e)
        {
            btnRefreshVer.Enabled = false;

            var sourceUrl = VgcApis.Models.Consts.Core.GetSourceUrlByIndex(
                cboxDownloadSource.SelectedIndex
            );
            int proxyPort = chkUseProxy.Checked ? servers.GetAvailableHttpProxyPort() : -1;
            var isV2fly = cboxDownloadSource.SelectedIndex == 1;

            void done(List<string> versions)
            {
                btnRefreshVer.Enabled = true;
                if (versions.Count > 0)
                {
                    Misc.UI.FillComboBox(cboxVer, versions);
                }
                else
                {
                    MessageBox.Show(I18N.GetVersionListFail);
                }
            }

            void worker()
            {
                var versions = Misc.Utils.GetOnlineV2RayCoreVersionList(proxyPort, sourceUrl);

                if (isV2fly)
                {
                    versions = versions.Where(v => v.StartsWith("v4.")).ToList();
                }

                if (versions != null && versions.Count > 0)
                {
                    setting.SaveV2RayCoreVersionList(versions);
                }
                VgcApis.Misc.UI.Invoke(() => done(versions));
            }

            VgcApis.Misc.Utils.RunInBgSlim(worker);
        }

        private void BtnUpdate_Click(object sender, EventArgs e)
        {
            if (downloader != null)
            {
                MessageBox.Show(I18N.Downloading);
                return;
            }

            int proxyPort = -1;
            if (chkUseProxy.Checked)
            {
                proxyPort = servers.GetAvailableHttpProxyPort();
                if (proxyPort <= 0)
                {
                    VgcApis.Misc.UI.MsgBoxAsync(I18N.NoQualifyProxyServer);
                }
            }

            btnDownload.Enabled = false;
            DownloadV2RayCore(proxyPort);
        }

        void BtnCancel_Click(object sender, EventArgs e)
        {
            if (downloader != null && Misc.UI.Confirm(I18N.CancelDownload))
            {
                downloader?.Cancel();
            }
        }

        private void BtnCheckVersion_Click(object sender, EventArgs e)
        {
            RefreshLocalV2RayCoreVersion();
        }

        private void cboxArch_SelectedIndexChanged(object sender, EventArgs e)
        {
            var index = cboxArch.SelectedIndex;
            if (index < 0 || index > 1)
            {
                return;
            }

            var isWin32 = index == 0;
            if (isWin32 == setting.isDownloadWin32V2RayCore)
            {
                return;
            }

            setting.isDownloadWin32V2RayCore = isWin32;
        }

        private void cboxDownloadSource_SelectedIndexChanged(object sender, EventArgs e)
        {
            var index = cboxDownloadSource.SelectedIndex;
            var url = VgcApis.Models.Consts.Core.GetSourceUrlByIndex(index);
            setting.v2rayCoreDownloadSource = url;
        }
        #endregion
    }
}
