using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Windows.Forms;

namespace V2RayGCon.Controllers.ConfigerComponet
{
    class EnvVar : ConfigerComponentController
    {
        ComboBox cboxImportAlias, cboxEnvName;
        TextBox tboxImportUrl, tboxEnvValue;
        Button btnInsertImport, btnInsertEnv;

        Dictionary<string, string> importTable;


        public EnvVar(
            ComboBox cboxImportAlias,
            TextBox tboxImportUrl,
            Button btnInsertImport,
            ComboBox cboxEnvName,
            TextBox tboxEnvValue,
            Button btnInsertEnv)
        {
            this.cboxEnvName = cboxEnvName;
            this.cboxImportAlias = cboxImportAlias;
            this.tboxEnvValue = tboxEnvValue;
            this.tboxImportUrl = tboxImportUrl;
            this.btnInsertImport = btnInsertImport;
            this.btnInsertEnv = btnInsertEnv;

            Init();
            BindEvent();

        }


        #region private method
        void BindEvent()
        {
            cboxImportAlias.SelectedIndexChanged += (s, a) =>
            {
                var alias = cboxImportAlias.Text;
                if (importTable.ContainsKey(alias))
                {
                    tboxImportUrl.Text = importTable[alias];
                }
            };

            btnInsertImport.Click += (s, a) =>
            {
                var json = Misc.Utils.CreateJObject("v2raygcon.import");
                json["v2raygcon"]["import"][tboxImportUrl.Text] = cboxImportAlias.Text;
                container.InjectConfigHelper(() =>
                    Misc.Utils.MergeJson(ref container.config, json));
            };

            btnInsertEnv.Click += (s, a) =>
            {
                var json = Misc.Utils.CreateJObject("v2raygcon.env");
                json["v2raygcon"]["env"][cboxEnvName.Text] = tboxEnvValue.Text;
                container.InjectConfigHelper(
                    () => Misc.Utils.MergeJson(ref container.config, json));
            };
        }

        void Init()
        {
            importTable = new Dictionary<string, string>();

            var importItems = Services.Settings.Instance.GetGlobalImportItems();

            cboxImportAlias.Items.Clear();
            foreach (var item in importItems)
            {
                if (!importTable.ContainsKey(item.alias))
                {
                    importTable.Add(item.alias, item.url);
                    cboxImportAlias.Items.Add(item.alias);
                }
                else
                {
                    importTable[item.alias] = item.url;
                }
            }

            cboxEnvName.Items.Clear();
            foreach (var name in Models.Datas.Table.EnviromentVariablesName)
            {
                cboxEnvName.Items.Add(name);
            }

            Misc.UI.ResetComboBoxDropdownMenuWidth(cboxImportAlias);
            Misc.UI.ResetComboBoxDropdownMenuWidth(cboxEnvName);
        }

        #endregion

        #region public method
        public override void Update(JObject config)
        {
            // do nothing
        }
        #endregion
    }
}
