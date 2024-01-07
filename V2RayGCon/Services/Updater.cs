using System;
using System.Text;
using System.Windows.Forms;
using Newtonsoft.Json;
using V2RayGCon.Resources.Resx;
using VgcApis.Libs.Tasks;

namespace V2RayGCon.Services
{
    public sealed class Updater : BaseClasses.SingletonService<Updater>
    {
        Settings setting;
        Servers servers;
        readonly Bar bar = new Bar();

        Updater() { }

        #region public methods
        public void CheckForUpdate(bool isShowErrorUsingMessageBox)
        {
            if (!bar.Install())
            {
                VgcApis.Misc.UI.MsgBoxAsync(I18N.UpdatingPleaseWait);
                return;
            }

            VgcApis.Misc.Utils.RunInBackground(() => Update(!isShowErrorUsingMessageBox));
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
            if (!GetAvailableProxyInfo(out var isSocks5, out var port))
            {
                if (isQuiet)
                {
                    setting.SendLog(I18N.NoQualifyProxyServer);
                }
                else
                {
                    VgcApis.Misc.UI.MsgBox(I18N.NoQualifyProxyServer);
                }
            }

            var source = Properties.Resources.LatestVersionInfoUrl;
            var info = GetUpdateInfoFrom(source, isSocks5, port);
            UpdateWorker(info, isQuiet);
        }

        Models.Datas.UpdateInfo GetUpdateInfoFrom(string url, bool isSocks5, int port)
        {
            var timeout = VgcApis.Models.Consts.Intervals.DefaultFetchTimeout;
            var str = Misc.Utils.FetchWorker(
                isSocks5,
                url,
                VgcApis.Models.Consts.Webs.LoopBackIP,
                port,
                timeout,
                null,
                null
            );
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
                bar.Remove();
                return;
            }

            // if download file failed or cancelled, user can try again
            bar.Remove();

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

        bool GetAvailableProxyInfo(out bool isSocks5, out int port)
        {
            if (setting.isUpdateUseProxy)
            {
                return servers.GetAvailableProxyInfo(out isSocks5, out port);
            }

            isSocks5 = false;
            port = -1;
            return false;
        }

        #endregion

        #region protected methods
        protected override void Cleanup()
        {
            bar.Dispose();
        }
        #endregion
    }
}
