using Newtonsoft.Json;
using System;
using System.Text;
using System.Windows.Forms;
using V2RayGCon.Resources.Resx;

namespace V2RayGCon.Services
{
    public sealed class Updater : BaseClasses.SingletonService<Updater>
    {
        Settings setting;
        Servers servers;
        readonly VgcApis.Libs.Tasks.Bar updateBar = new VgcApis.Libs.Tasks.Bar();

        Updater() { }

        #region public methods
        public void CheckForUpdate(bool isShowErrorUsingMessageBox)
        {
            if (!updateBar.Install())
            {
                VgcApis.Misc.UI.MsgBoxAsync(I18N.UpdatingPleaseWait);
                return;
            }

            VgcApis.Misc.Utils.RunInBgSlim(() => Update(!isShowErrorUsingMessageBox));
        }

        public void Run(Settings setting, Servers servers)
        {
            this.setting = setting;
            this.servers = servers;
        }
        #endregion

        #region private methods
        void Update(bool isQuiet)
        {
            var port = GetAvailableProxyPort(isQuiet);
            var source = Properties.Resources.LatestVersionInfoUrl;
            var info = GetUpdateInfoFrom(source, port);
            UpdateWorker(info, isQuiet);
        }

        Models.Datas.UpdateInfo GetUpdateInfoFrom(string url, int port)
        {
            var timeout = VgcApis.Models.Consts.Intervals.DefaultFetchTimeout;
            var str = Misc.Utils.Fetch(url, port, timeout);
            if (string.IsNullOrEmpty(str))
            {
                return null;
            }
            try
            {
                return JsonConvert.DeserializeObject<Models.Datas.UpdateInfo>(str);
            }
            catch { }
            return null;
        }

        bool CheckUpdateInfo(Models.Datas.UpdateInfo infos, bool isQuiet)
        {
            if (infos == null)
            {
                if (!isQuiet)
                {
                    MessageBox.Show(
                        I18N.FetchUpdateInfoFail,
                        I18N.Error,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                }
                return false;
            }

            var curVer = System.Reflection.Assembly.GetEntryAssembly().GetName().Version;
            var newVer = new Version(infos.version);
            if (newVer > curVer)
            {
                return true;
            }

            if (isQuiet)
            {
                setting.SendLog(infos == null ? I18N.FetchUpdateInfoFail : I18N.NoUpdateTryLater);
            }
            else
            {
                VgcApis.Misc.UI.MsgBox(I18N.NoUpdateTryLater);
            }
            return false;
        }

        void UpdateWorker(Models.Datas.UpdateInfo info, bool isQuiet)
        {
            if (!CheckUpdateInfo(info, isQuiet) || !ConfirmUpdate(info))
            {
                // must confirm first
                updateBar.Remove();
                return;
            }

            // if download file failed or cancelled, user can try again
            updateBar.Remove();

            try
            {
                VgcApis.Misc.UI.Invoke(() =>
                {
                    var form = new Views.Updater.FormDownloader();
                    form.FormClosed += (s, e) =>
                    {
                        if (form.DialogResult == DialogResult.OK)
                        {
                            setting.SetShutdownReason(
                                VgcApis.Models.Datas.Enums.ShutdownReasons.CloseByUser
                            );
                            Application.Exit();
                        }
                    };
                    form.Init(info);
                });
            }
            catch (Exception exception)
            {
                MessageBox.Show(
                    exception.Message,
                    exception.GetType().ToString(),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        bool ConfirmUpdate(Models.Datas.UpdateInfo info)
        {
            if (info == null)
            {
                return false;
            }

            var tag = Misc.Utils.TrimVersionString(info.version);
            StringBuilder msg = new StringBuilder(string.Format(I18N.ConfirmUpgradeVgc, tag));

            var warnings = info.warnings;
            var changes = info.changes;
            var nl = Environment.NewLine;

            if (warnings != null && warnings.Count > 0)
            {
                msg.Append(nl + nl + I18N.WarningColon + nl);
                msg.Append(string.Join(nl, warnings));
            }

            if (changes != null && changes.Count > 0)
            {
                msg.Append(nl + nl + I18N.ChangesColon + nl);
                msg.Append(string.Join(nl, changes));
            }

            return VgcApis.Misc.UI.Confirm(msg.ToString());
        }

        int GetAvailableProxyPort(bool isQuiet)
        {
            if (setting.isUpdateUseProxy)
            {
                var port = servers.GetAvailableHttpProxyPort();
                if (port > 0)
                {
                    return port;
                }
            }

            if (isQuiet)
            {
                setting.SendLog(I18N.NoQualifyProxyServer);
            }
            else
            {
                VgcApis.Misc.UI.MsgBox(I18N.NoQualifyProxyServer);
            }

            return -1;
        }

        #endregion

        #region protected methods
        protected override void Cleanup() { }
        #endregion
    }
}
