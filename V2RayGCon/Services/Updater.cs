using AutoUpdaterDotNET;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Text;
using System.Windows.Forms;
using V2RayGCon.Resources.Resx;

namespace V2RayGCon.Services
{
    internal sealed class Updater :
        BaseClasses.SingletonService<Updater>
    {
        Settings setting;
        Servers servers;

        VgcApis.Libs.Tasks.Bar updateBar = new VgcApis.Libs.Tasks.Bar();
        readonly string LoopBackIP = VgcApis.Models.Consts.Webs.LoopBackIP;
        Models.Datas.UpdateInfo rawUpdateInfo = null;

        Updater() { }

        #region public methods
        public void CheckForUpdate(bool isShowErrorWithMsgbox)
        {
            if (!updateBar.Install())
            {
                VgcApis.Misc.UI.MsgBoxAsync(I18N.UpdatingPleaseWait);
                return;
            }

            flagShowErrorWithMsgbox = isShowErrorWithMsgbox;
            AutoSetUpdaterProxy();
            AutoUpdater.Start(Properties.Resources.LatestVersionInfoUrl);
        }

        public void Run(
            Settings setting,
            Servers servers)
        {
            this.setting = setting;
            this.servers = servers;
            InitAutoUpdater();
        }
        #endregion

        #region private methods
        bool flagShowErrorWithMsgbox = true;
        bool CheckUpdateInfoArgs(UpdateInfoEventArgs args)
        {
            if (args != null && args.IsUpdateAvailable)
            {
                return true;
            }

            if (!flagShowErrorWithMsgbox)
            {
                setting.SendLog(
                    args == null ?
                    I18N.FetchUpdateInfoFail :
                    I18N.NoUpdateTryLater);
                return false;
            }

            if (args == null)
            {
                MessageBox.Show(
                   I18N.FetchUpdateInfoFail,
                   I18N.Error,
                   MessageBoxButtons.OK,
                   MessageBoxIcon.Error);
            }
            else
            {
                MessageBox.Show(I18N.NoUpdateTryLater);
            }
            return false;
        }

        void AutoUpdaterOnCheckForUpdateEvent(UpdateInfoEventArgs args)
        {
            if (!CheckUpdateInfoArgs(args)
                || !ConfirmUpdate())
            {
                updateBar.Remove();
                return;
            }

            try
            {
                if (AutoUpdater.DownloadUpdate())
                {
                    Application.Exit();
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(
                    exception.Message,
                    exception.GetType().ToString(),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        bool ConfirmUpdate()
        {
            if (rawUpdateInfo == null)
            {
                return false;
            }

            var tag = Misc.Utils.TrimVersionString(rawUpdateInfo.version);
            StringBuilder msg = new StringBuilder(
                string.Format(I18N.ConfirmUpgradeVgc, tag));

            var warnings = rawUpdateInfo.warnings;
            var changes = rawUpdateInfo.changes;
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

        void AutoUpdaterOnParseUpdateInfoEvent(ParseUpdateInfoEventArgs args)
        {
            rawUpdateInfo = JsonConvert
                .DeserializeObject<Models.Datas.UpdateInfo>(
                    args.RemoteData);

            var url = string.Format(
                VgcApis.Models.Consts.Webs.ReleaseDownloadUrlTpl,
                Misc.Utils.TrimVersionString(rawUpdateInfo.version));

            args.UpdateInfo = new UpdateInfoEventArgs
            {
                CurrentVersion = new Version(rawUpdateInfo.version),
                Mandatory = false,
                DownloadURL = url,
                HashingAlgorithm = "MD5",
                Checksum = rawUpdateInfo.md5,
            };
        }

        void AutoSetUpdaterProxy()
        {
            if (!setting.isUpdateUseProxy)
            {
                return;
            }

            var port = servers.GetAvailableHttpProxyPort();
            if (port > 0)
            {
                var proxy = new WebProxy($"{LoopBackIP}:{port}", true);
                AutoUpdater.Proxy = proxy;
                return;
            }

            if (flagShowErrorWithMsgbox)
            {
                MessageBox.Show(I18N.NoQualifyProxyServer);
            }
            else
            {
                setting.SendLog(I18N.NoQualifyProxyServer);
            }
        }

        void InitAutoUpdater()
        {
            AutoUpdater.ReportErrors = true;
            AutoUpdater.ParseUpdateInfoEvent +=
                AutoUpdaterOnParseUpdateInfoEvent;
            AutoUpdater.CheckForUpdateEvent +=
                AutoUpdaterOnCheckForUpdateEvent;
        }
        #endregion

        #region protected methods
        protected override void Cleanup()
        {
            AutoUpdater.ParseUpdateInfoEvent -=
                AutoUpdaterOnParseUpdateInfoEvent;
            AutoUpdater.CheckForUpdateEvent -=
                AutoUpdaterOnCheckForUpdateEvent;
        }
        #endregion
    }
}
