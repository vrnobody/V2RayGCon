using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using V2RayGCon.Resources.Resx;

namespace V2RayGCon.Controllers
{
    public class FormOptionCtrl : BaseClasses.FormController
    {
        readonly Services.Settings setting;
        readonly Services.PluginsServer pluginServ;

        static readonly string BAK_IMPORT = @"import";
        static readonly string BAK_SUBSCRIPTION = @"subscription";
        static readonly string BAK_SERVERS = @"servers";
        static readonly string BAK_PLUGINS = @"plugins";

        public FormOptionCtrl()
        {
            this.setting = Services.Settings.Instance;
            pluginServ = Services.PluginsServer.Instance;
        }

        public bool IsOptionsSaved()
        {
            foreach (var component in GetAllComponents())
            {
                var optCtrl = component.Value as OptionComponent.OptionComponentController;
                if (optCtrl.IsOptionsChanged())
                {
                    return false;
                }
            }
            return true;
        }

        public bool SaveAllOptions()
        {
            var changed = false;
            foreach (var kvPair in GetAllComponents())
            {
                var component = kvPair.Value as OptionComponent.OptionComponentController;
                if (component.SaveOptions())
                {
                    changed = true;
                }
            }
            return changed;
        }

        public void BackupOptions()
        {
            if (!IsOptionsSaved())
            {
                MessageBox.Show(I18N.SaveChangeFirst);
                return;
            }

            var serverString = string.Empty;
            foreach (var server in Services.Servers.Instance.GetAllServersOrderByIndex())
            {
                // insert a space in the front for regex matching
                serverString += @" "
                    + VgcApis.Models.Datas.Enum.LinkTypes.v2cfg.ToString()
                    + @"://"
                    + Misc.Utils.Base64Encode(server.GetConfiger().GetConfig())
                    + Environment.NewLine;
            }

            var data = new Dictionary<string, string> {
                { BAK_IMPORT, JsonConvert.SerializeObject(setting.GetGlobalImportItems())},
                { BAK_SUBSCRIPTION, JsonConvert.SerializeObject(setting.GetSubscriptionItems()) },
                { BAK_SERVERS, serverString},
                { BAK_PLUGINS, setting.AllPluginsSetting},
            };

            VgcApis.Misc.UI.SaveToFile(
                VgcApis.Models.Consts.Files.TxtExt,
                JsonConvert.SerializeObject(data));
        }

        public void RestoreOptions()
        {
            string backup = VgcApis.Misc.UI.ReadFileContentFromDialog(
                VgcApis.Models.Consts.Files.TxtExt);

            if (backup == null)
            {
                return;
            }

            if (!Misc.UI.Confirm(I18N.ConfirmOriginalSettingWillBeReset))
            {
                return;
            }

            Dictionary<string, string> options;
            try
            {
                options = JsonConvert.DeserializeObject<Dictionary<string, string>>(backup);
            }
            catch
            {
                MessageBox.Show(I18N.DecodeFail);
                return;
            }

            if (options.ContainsKey(BAK_IMPORT)
                && Misc.UI.Confirm(I18N.ConfirmRestoreGlobalImportSettings))
            {
                GetComponent<OptionComponent.Import>().Reload(options[BAK_IMPORT]);
            }

            if (options.ContainsKey(BAK_SUBSCRIPTION)
                && Misc.UI.Confirm(I18N.ConfirmRestoreSubscriptionSettings))
            {
                GetComponent<OptionComponent.Subscription>().Merge(options[BAK_SUBSCRIPTION]);
            }

            if (options.ContainsKey(BAK_PLUGINS)
                && Misc.UI.Confirm(I18N.ConfirmRestorePluginsSetting))
            {
                pluginServ.StopAllPlugins();
                setting.AllPluginsSetting = options[BAK_PLUGINS];
                pluginServ.RestartAllPlugins();
            }

            if (options.ContainsKey(BAK_SERVERS)
                && Misc.UI.Confirm(I18N.ConfirmImportServers))
            {
                Services.ShareLinkMgr.Instance
                    .ImportLinkWithV2cfgLinks(options[BAK_SERVERS]);
            }
            else
            {
                // import link will popup a import-results window
                MessageBox.Show(I18N.Done);
            }
        }

    }
}
