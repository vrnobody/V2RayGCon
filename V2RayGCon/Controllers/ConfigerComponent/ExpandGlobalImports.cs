using Newtonsoft.Json.Linq;
using ScintillaNET;
using System;
using System.IO;
using System.Windows.Forms;
using V2RayGCon.Resources.Resx;


namespace V2RayGCon.Controllers.ConfigerComponet
{
    class ExpandGlobalImports : ConfigerComponentController
    {
        Services.Settings setting;
        Services.ConfigMgr configMgr;

        Scintilla editor;
        CheckBox cboxGlobalImport;

        public ExpandGlobalImports(
            Panel container,
            CheckBox globalImport,
            Button btnExpand,
            Button btnClearCache,
            Button btnCopy,
            Button btnSaveAs)
        {
            this.setting = Services.Settings.Instance;
            this.configMgr = Services.ConfigMgr.Instance;

            this.editor = Misc.UI.CreateScintilla(container, true);
            this.cboxGlobalImport = globalImport;
            DataBinding();
            AttachEvent(btnExpand, btnClearCache, btnCopy, btnSaveAs);
        }

        #region properties
        private string _content;

        public string content
        {
            get { return _content; }
            set
            {
                editor.ReadOnly = false;
                SetField(ref _content, value);
                editor.ReadOnly = true;
            }
        }
        #endregion

        #region private method
        void SaveCurrentContentToFile()
        {
            VgcApis.Misc.UI.SaveToFile(
                VgcApis.Models.Consts.Files.JsonExt,
                editor.Text);
        }

        void AttachEvent(
            Button btnExpand,
            Button btnClearCache,
            Button btnCopy,
            Button btnSaveAs)
        {
            btnSaveAs.Click += (s, a) => SaveCurrentContentToFile();

            btnExpand.Click += (s, a) =>
            {
                container.InjectConfigHelper(null);
            };

            btnClearCache.Click += (s, a) =>
            {
                container.InjectConfigHelper(() =>
                {
                    Services.Cache.Instance.html.Clear();
                });
            };

            btnCopy.Click += (s, a) =>
            {
                Misc.Utils.CopyToClipboardAndPrompt(editor.Text);
            };
        }

        void DataBinding()
        {
            var bs = new BindingSource();
            bs.DataSource = this;

            editor.DataBindings.Add(
                "Text",
                bs,
                nameof(this.content),
                true,
                DataSourceUpdateMode.OnPropertyChanged);
        }

        #endregion

        #region public method
        public void Cleanup()
        {
            editor?.Dispose();
        }

        public override void Update(JObject config)
        {
            content = I18N.AnalysingImport;

            var plainText = config.ToString();
            if (cboxGlobalImport.Checked)
            {
                var configWithGlobalImports =
                    Misc.Utils.ImportItemList2JObject(
                        setting.GetGlobalImportItems(), false, true, false);

                Misc.Utils.MergeJson(ref configWithGlobalImports, config);
                plainText = configWithGlobalImports.ToString();
            }

            VgcApis.Misc.Utils.RunInBackground(() =>
            {
                string result = ParseConfig(plainText);
                setting.LazyGC();

                VgcApis.Misc.UI.RunInUiThreadIgnoreError(editor, () => content = result);
            });
        }

        private string ParseConfig(string plainText)
        {
            try
            {
                return configMgr.ParseImport(plainText).ToString();
            }
            catch (FileNotFoundException)
            {
                return string.Format("{0}{1}{2}",
                        I18N.DecodeImportFail,
                        Environment.NewLine,
                        I18N.FileNotFound);
            }
            catch (FormatException)
            {
                return I18N.DecodeImportFail;
            }
            catch (System.Net.WebException)
            {
                return string.Format(
                        "{0}{1}{2}",
                        I18N.DecodeImportFail,
                        Environment.NewLine,
                        I18N.NetworkTimeout);
            }
            catch (Newtonsoft.Json.JsonReaderException)
            {
                return I18N.DecodeImportFail;
            }
        }
        #endregion
    }
}
