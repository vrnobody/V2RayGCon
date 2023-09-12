using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Windows.Forms;
using V2RayGCon.Resources.Resx;

namespace V2RayGCon.Controllers
{
    class FormJsonConfigEditorCtrl : BaseClasses.FormController
    {
        public event EventHandler OnChanged;

        Services.Servers servers;

        public JObject config;
        string uid,
            orgCfg;
        FormJsonConfigEditorComponet.Editor editor;

        public FormJsonConfigEditorCtrl()
        {
            servers = Services.Servers.Instance;
            config = ParseConfigString(null);
        }

        #region public method

        public void InvokeOnChanged()
        {
            try
            {
                OnChanged?.Invoke(this, EventArgs.Empty);
            }
            catch { }
        }

        public void LoadConfigByUid(string uid)
        {
            var cfg = GetConfigByUid(uid);
            LoadConfigString(cfg);
            this.uid = uid;
        }

        public void LoadConfigString(string cfg)
        {
            uid = null;
            CacheOriginalConfig(cfg);
            Update();
            editor.ReloadSection();
        }

        public void Cleanup()
        {
            GetComponent<FormJsonConfigEditorComponet.MenuUpdater>()?.Cleanup();
            GetComponent<FormJsonConfigEditorComponet.ExpandGlobalImports>()?.Cleanup();
            GetComponent<FormJsonConfigEditorComponet.Editor>()?.Cleanup();
        }

        public void Prepare()
        {
            editor = GetComponent<FormJsonConfigEditorComponet.Editor>();
            editor.Prepare();
        }

        public bool IsConfigSaved()
        {
            if (editor.IsChanged())
            {
                return false;
            }

            if (string.IsNullOrEmpty(uid) && string.IsNullOrEmpty(orgCfg))
            {
                return false;
            }

            var cfg = orgCfg;
            if (!string.IsNullOrEmpty(uid))
            {
                cfg = GetConfigByUid(uid);
            }

            JObject o = ParseConfigString(cfg);
            return JObject.DeepEquals(o, config);
        }

        public string GetAlias()
        {
            return Misc.Utils.GetValue<string>(config, "v2raygcon.alias");
        }

        public void Update()
        {
            foreach (var component in this.GetAllComponents())
            {
                (
                    component.Value as FormJsonConfigEditorComponet.ConfigerComponentController
                ).Update(config);
            }
        }

        public string SaveToFile()
        {
            InjectConfigHelper(null);

            var cfg = FormatWithIndent();
            var r = VgcApis.Misc.UI.ShowSaveFileDialog(
                VgcApis.Models.Consts.Files.JsonExt,
                cfg,
                out string filename
            );

            switch (r)
            {
                case VgcApis.Models.Datas.Enums.SaveFileErrorCode.Success:
                    uid = null;
                    this.orgCfg = cfg;
                    MessageBox.Show(I18N.Done);
                    return filename;
                case VgcApis.Models.Datas.Enums.SaveFileErrorCode.Fail:
                    MessageBox.Show(I18N.WriteFileFail);
                    break;
                case VgcApis.Models.Datas.Enums.SaveFileErrorCode.Cancel:
                    // do nothing
                    break;
            }
            return null;
        }

        public bool SaveCurServer()
        {
            return ReplaceServer(uid);
        }

        public bool ReplaceServer(string uid)
        {
            if (string.IsNullOrEmpty(uid))
            {
                MessageBox.Show(I18N.OrgServNotFound);
                return false;
            }

            if (!Flush())
            {
                return false;
            }

            var originalConfig = GetConfigByUid(uid);
            var newConfig = VgcApis.Misc.Utils.FormatConfig(config);

            if (originalConfig == newConfig || servers.IsServerExist(newConfig))
            {
                MessageBox.Show(I18N.DuplicateServer);
                return false;
            }

            if (!servers.ReplaceServerConfig(originalConfig, newConfig))
            {
                MessageBox.Show(I18N.OrgServNotFound);
                return false;
            }

            LoadConfigByUid(uid);
            return true;
        }

        public void AddNewServer()
        {
            if (!Flush())
            {
                return;
            }

            if (!servers.AddServer(config.ToString(), ""))
            {
                MessageBox.Show(I18N.DuplicateServer);
            }

            var uid = servers
                .GetAllServersOrderByIndex()
                ?.FirstOrDefault(s =>
                {
                    try
                    {
                        var o = ParseConfigString(s.GetConfiger()?.GetConfig());
                        return JObject.DeepEquals(config, o);
                    }
                    catch { }
                    return false;
                })
                ?.GetCoreStates()
                ?.GetUid();

            LoadConfigByUid(uid);
        }

        public string FormatWithIndent()
        {
            return config.ToString(Newtonsoft.Json.Formatting.Indented);
        }

        public void InjectConfigHelper(Action lambda)
        {
            if (!editor.Flush())
            {
                return;
            }

            lambda?.Invoke();

            Update();
            editor.ReloadSection();
        }

        #endregion

        #region private method
        bool Flush()
        {
            if (!editor.Flush())
            {
                return false;
            }
            Update();
            return true;
        }

        string GetConfigByUid(string uid) =>
            servers.GetServerByUid(uid)?.GetConfiger()?.GetConfig();

        void CacheOriginalConfig(string cfg)
        {
            this.orgCfg = cfg;
            config = ParseConfigString(cfg);
        }

        JObject ParseConfigString(string cfgStr)
        {
            var empty = JObject.Parse(@"{}");

            if (string.IsNullOrEmpty(cfgStr))
            {
                return empty;
            }

            try
            {
                return JObject.Parse(cfgStr);
            }
            catch
            {
                VgcApis.Misc.UI.MsgBox(I18N.ConfigerSupportsJsonOnly);
            }
            return empty;
        }
        #endregion
    }
}
