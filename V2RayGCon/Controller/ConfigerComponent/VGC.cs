using Newtonsoft.Json.Linq;
using System.Windows.Forms;

namespace V2RayGCon.Controller.ConfigerComponet
{
    class VGC : ConfigerComponentController
    {
        public VGC(TextBox alias, TextBox description, Button insert)
        {
            var bs = new BindingSource();
            bs.DataSource = this;
            alias.DataBindings.Add("Text", bs, nameof(this.alias));
            description.DataBindings.Add("Text", bs, nameof(this.description));

            insert.Click += (s, a) =>
            {
                container.InjectConfigHelper(() =>
                {
                    var key = "v2raygcon";
                    var mixin = Lib.Utils.CreateJObject(key);
                    mixin[key] = GetSettings();
                    Lib.Utils.MergeJson(ref container.config, mixin);
                });
            };
        }

        #region properties
        private string _alias;
        private string _description;

        public string alias
        {
            get { return _alias; }
            set { SetField(ref _alias, value); }
        }

        public string description
        {
            get { return _description; }
            set { SetField(ref _description, value); }
        }

        #endregion

        #region private method
        JToken GetSettings()
        {
            JToken vgc = Service.Cache.Instance.
                tpl.LoadTemplate("vgc");

            vgc["alias"] = alias;
            vgc["description"] = description;

            return vgc;
        }
        #endregion

        #region public method
        public override void Update(JObject config)
        {
            var prefix = "v2raygcon";

            alias = Lib.Utils.GetValue<string>(config, prefix, "alias");
            description = Lib.Utils.GetValue<string>(config, prefix, "description");
        }
        #endregion
    }
}
