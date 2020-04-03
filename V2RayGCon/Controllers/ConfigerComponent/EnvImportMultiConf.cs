using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Windows.Forms;

namespace V2RayGCon.Controllers.ConfigerComponet
{
    class EnvImportMultiConf : ConfigerComponentController
    {
        readonly ComboBox cboxImportAlias, cboxEnvName, cboxMultiConfAlias;
        readonly TextBox tboxImportUrl, tboxEnvValue, tboxMultiConfPath;
        readonly Button btnInsertImport, btnInsertEnv, btnInsertMultiConf;

        Dictionary<string, string> importTable, multiConfTable;


        public EnvImportMultiConf(
            ComboBox cboxMultiConfAlias,
            TextBox tboxMultiConfPath,
            Button btnInsertMultiConf,

            ComboBox cboxImportAlias,
            TextBox tboxImportUrl,
            Button btnInsertImport,

            ComboBox cboxEnvName,
            TextBox tboxEnvValue,
            Button btnInsertEnv)
        {
            this.cboxMultiConfAlias = cboxMultiConfAlias;
            this.tboxMultiConfPath = tboxMultiConfPath;
            this.btnInsertMultiConf = btnInsertMultiConf;

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
            cboxMultiConfAlias.SelectedIndexChanged += (s, a) =>
            {
                var alias = cboxMultiConfAlias.Text;
                if (multiConfTable.ContainsKey(alias))
                {
                    tboxMultiConfPath.Text = multiConfTable[alias];
                }
            };

            btnInsertMultiConf.Click += (s, a) =>
            {
                var json = Misc.Utils.CreateJObject("v2raygcon.configs");
                json["v2raygcon"]["configs"][tboxMultiConfPath.Text] = cboxMultiConfAlias.Text;
                container.InjectConfigHelper(() =>
                    Misc.Utils.MergeJson(ref container.config, json));
            };

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
            InitMultiConfControls();
            InitGlobalImportControls();
            InitEnvVarControls();
        }

        private void InitEnvVarControls()
        {
            cboxEnvName.Items.Clear();
            foreach (var name in Models.Datas.Table.EnviromentVariablesName)
            {
                cboxEnvName.Items.Add(name);
            }
            Misc.UI.ResetComboBoxDropdownMenuWidth(cboxEnvName);
        }

        private void InitMultiConfControls()
        {
            var stdinKey = @"Standard Input";
            multiConfTable = new Dictionary<string, string> {
                {stdinKey, VgcApis.Models.Consts.Core.StdIn                },
            };

            var multiConfItems = Services.Settings.Instance.GetMultiConfItems();

            var items = cboxMultiConfAlias.Items;
            items.Clear();
            items.Add(stdinKey);

            foreach (var item in multiConfItems)
            {
                var alias = item.alias;
                var path = VgcApis.Misc.Utils.RelativePath2FullPath(item.path);

                if (!multiConfTable.ContainsKey(alias))
                {
                    multiConfTable.Add(alias, path);
                    items.Add(alias);
                }
                else
                {
                    importTable[alias] = path;
                }
            }

            Misc.UI.ResetComboBoxDropdownMenuWidth(cboxMultiConfAlias);
        }


        private void InitGlobalImportControls()
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

            Misc.UI.ResetComboBoxDropdownMenuWidth(cboxImportAlias);
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
