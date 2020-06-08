using Newtonsoft.Json.Linq;
using System;
using System.Windows.Forms;
using V2RayGCon.Resources.Resx;

namespace V2RayGCon.Controllers
{
    class FormConfigerCtrl : BaseClasses.FormController
    {
        Services.Servers servers;

        public JObject config;
        string originalConfig, originalFile;
        ConfigerComponet.Editor editor;

        public FormConfigerCtrl(string originalConfig)
        {
            servers = Services.Servers.Instance;

            this.originalFile = string.Empty;
            this.originalConfig = string.Empty;

            LoadConfig(originalConfig);
        }

        #region public method


        public void Cleanup()
        {
            GetComponent<ConfigerComponet.MenuUpdater>()?.Cleanup();
            GetComponent<ConfigerComponet.ExpandGlobalImports>()?.Cleanup();
            GetComponent<ConfigerComponet.Editor>()?.Cleanup();
        }

        public void Prepare()
        {
            editor = GetComponent<ConfigerComponet.Editor>();
            editor.Prepare();
            Update();
        }

        public bool IsConfigSaved()
        {
            if (editor.IsChanged())
            {
                return false;
            }

            if (string.IsNullOrEmpty(originalConfig)
                && string.IsNullOrEmpty(originalFile))
            {
                return false;
            }

            if (string.IsNullOrEmpty(originalFile))
            {
                JObject orgConfig = JObject.Parse(originalConfig);
                return JObject.DeepEquals(orgConfig, config);
            }

            JObject orgFile = JObject.Parse(originalFile);
            return JObject.DeepEquals(orgFile, config);
        }

        public string GetAlias()
        {
            return Misc.Utils.GetValue<string>(config, "v2raygcon.alias");
        }

        public void Update()
        {
            foreach (var component in this.GetAllComponents())
            {
                (component.Value as Controllers.ConfigerComponet.ConfigerComponentController)
                    .Update(config);
            }
        }

        public bool SaveServer()
        {
            return ReplaceServer(originalConfig);
        }

        public bool ReplaceServer(string originalConfig)
        {
            if (!editor.Flush())
            {
                return false;
            }
            Update();

            var newConfig = Misc.Utils.Config2String(config);
            if (originalConfig == newConfig
                || servers.IsServerExist(newConfig))
            {
                MessageBox.Show(I18N.DuplicateServer);
                return false;
            }

            if (servers.ReplaceServerConfig(originalConfig, newConfig))
            {
                MarkOriginalConfig();
            }
            else
            {
                MessageBox.Show(I18N.OrgServNotFound);
                return false;
            }

            return true;
        }

        public void AddNewServer()
        {
            if (!editor.Flush())
            {
                return;
            }

            Update();

            if (servers.AddServer(Misc.Utils.Config2String(config), ""))
            {
                MarkOriginalConfig();
            }
            else
            {
                MessageBox.Show(I18N.DuplicateServer);
            }
        }

        public void MarkOriginalConfig()
        {
            originalFile = string.Empty;
            originalConfig = Misc.Utils.Config2String(config);
        }

        public void MarkOriginalFile()
        {
            originalConfig = string.Empty;
            originalFile = GetConfigFormated();
        }

        public string GetConfigFormated()
        {
            return config.ToString(Newtonsoft.Json.Formatting.Indented);
        }

        public bool LoadJsonFromFile(string content)
        {
            if (string.IsNullOrEmpty(content))
            {
                return false;
            }

            try
            {
                var o = JObject.Parse(content);
                if (o == null)
                {
                    return false;
                }
                config = o;
                Update();
                MarkOriginalFile();
                editor.ReloadSection();
                return true;
            }
            catch { }
            return false;
        }

        public void LoadServer(string configString)
        {
            editor.DiscardChanges();
            editor.ShowEntireConfig();
            LoadConfig(configString);
            Update();
            editor.ReloadSection();
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
        void LoadConfig(string originalConfig)
        {
            JObject o = null;

            if (!string.IsNullOrEmpty(originalConfig))
            {
                o = JObject.Parse(originalConfig);
            }

            if (o == null)
            {
                o = Services.Cache.Instance.tpl.LoadMinConfig();
                VgcApis.Misc.UI.MsgBoxAsync(I18N.EditorCannotLoadServerConfig);
            }

            config = o;
            MarkOriginalConfig();
        }
        #endregion
    }
}
