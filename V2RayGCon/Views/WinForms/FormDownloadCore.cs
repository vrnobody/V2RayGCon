using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;
using V2RayGCon.Resource.Resx;

namespace V2RayGCon.Views.WinForms
{
    public partial class FormDownloadCore : Form
    {
        #region Sigleton
        static FormDownloadCore _instant;
        public static FormDownloadCore GetForm()
        {
            if (_instant == null || _instant.IsDisposed)
            {
                _instant = new FormDownloadCore();
            }
            return _instant;
        }
        #endregion

        Lib.Nets.Downloader downloader;
        Service.Setting setting;
        Service.Servers servers;

        FormDownloadCore()
        {
            setting = Service.Setting.Instance;
            servers = Service.Servers.Instance;

            InitializeComponent();
            InitUI();

            this.FormClosed += (s, e) =>
            {
                downloader?.Cleanup();
                setting.LazyGC();
            };

            VgcApis.Libs.UI.AutoSetFormIcon(this);

            this.Show();
        }

        private void FormDownloadCore_Shown(object sender, System.EventArgs e)
        {
            RefreshLocalV2RayCoreVersion();
            chkUseProxy.Checked = setting.isUpdateUseProxy;
        }

        #region private method
        void RefreshLocalV2RayCoreVersion()
        {
            var el = labelCoreVersion;

            VgcApis.Libs.Utils.RunInBackground(() =>
            {
                var core = new V2RayGCon.Lib.V2Ray.Core(setting);
                var version = core.GetCoreVersion();
                var msg = string.IsNullOrEmpty(version) ?
                    I18N.GetCoreVerFail :
                    string.Format(I18N.CurrentCoreVerIs, version);
                try
                {
                    VgcApis.Libs.UI.RunInUiThread(
                        el, () => { el.Text = msg; });
                }
                catch { }
            });
        }

        void UpdateProgressBar(int percentage)
        {
            // window may closed before this function is called
            try
            {
                VgcApis.Libs.UI.RunInUiThread(pgBarDownload, () =>
                {
                    pgBarDownload.Value = Lib.Utils.Clamp(percentage, 0, 101);
                });
            }
            catch { }
        }

        void EnableBtnDownload()
        {
            try
            {
                VgcApis.Libs.UI.RunInUiThread(btnDownload, () =>
                {
                    btnDownload.Enabled = true;
                });
            }
            catch { }
        }

        void DownloadV2RayCore(int proxyPort)
        {
            downloader = new Lib.Nets.Downloader(setting);
            downloader.SetArchitecture(cboxArch.SelectedIndex == 1);
            downloader.SetVersion(cboxVer.Text);
            downloader.proxyPort = proxyPort;

            downloader.OnProgress += (s, a) =>
            {
                UpdateProgressBar(a.Data);
            };

            downloader.OnDownloadCompleted += (s, a) =>
            {
                ResetUI(100);
                VgcApis.Libs.Utils.RunInBackground(
                    () => MessageBox.Show(I18N.DownloadCompleted));
            };

            downloader.OnDownloadCancelled += (s, a) =>
            {
                ResetUI(0);
                VgcApis.Libs.Utils.RunInBackground(
                    () => MessageBox.Show(I18N.DownloadCancelled));
            };

            downloader.OnDownloadFail += (s, a) =>
            {
                ResetUI(0);
                VgcApis.Libs.Utils.RunInBackground(
                    () => MessageBox.Show(I18N.TryManualDownload));
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
            cboxArch.SelectedIndex =
                setting.isDownloadWin32V2RayCore ? 0 : 1;

            var verList = setting.GetV2RayCoreVersionList();
            Lib.UI.FillComboBox(cboxVer, new List<string>(verList));
            pgBarDownload.Value = 0;
        }

        private void BtnExit_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }

        private void BtnRefreshVer_Click(object sender, System.EventArgs e)
        {
            var elRefresh = btnRefreshVer;
            var elCboxVer = cboxVer;

            elRefresh.Enabled = false;

            VgcApis.Libs.Utils.RunInBackground(() =>
            {
                int proxyPort = -1;
                if (chkUseProxy.Checked)
                {
                    proxyPort = servers.GetAvailableHttpProxyPort();
                }
                var versions = Lib.Utils.GetOnlineV2RayCoreVersionList(proxyPort);
                if (versions != null && versions.Count > 0)
                {
                    setting.SaveV2RayCoreVersionList(versions);
                }
                try
                {
                    VgcApis.Libs.UI.RunInUiThread(elRefresh, () =>
                    {
                        elRefresh.Enabled = true;
                    });

                    VgcApis.Libs.UI.RunInUiThread(elCboxVer, () =>
                    {
                        if (versions.Count > 0)
                        {
                            Lib.UI.FillComboBox(elCboxVer, versions);
                        }
                        else
                        {
                            MessageBox.Show(I18N.GetVersionListFail);
                        }
                    });
                }
                catch { }
            });
        }

        private void BtnUpdate_Click(object sender, System.EventArgs e)
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
                    VgcApis.Libs.Utils.RunInBackground(
                        () => MessageBox.Show(
                            I18N.NoQualifyProxyServer));
                }
            }

            btnDownload.Enabled = false;
            DownloadV2RayCore(proxyPort);
        }

        void BtnCancel_Click(object sender, System.EventArgs e)
        {
            if (downloader != null && Lib.UI.Confirm(I18N.CancelDownload))
            {
                downloader?.Cancel();
            }
        }

        private void BtnCheckVersion_Click(object sender, System.EventArgs e)
        {
            RefreshLocalV2RayCoreVersion();
        }

        #endregion

        private void cboxArch_SelectedIndexChanged(object sender, System.EventArgs e)
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
    }
}
