using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Windows.Forms;

namespace V2RayGCon.Controllers.ConfigerComponet
{
    class StreamSettings : ConfigerComponentController
    {
        Services.Cache cache;

        public StreamSettings(
            ComboBox type,
            ComboBox param,
            RadioButton inbound,
            CheckBox v4mode,
            Button insert,
            CheckBox tls,
            CheckBox sockopt
        )
        {
            cache = Services.Cache.Instance;
            isServer = false;
            isV4Mode = false;
            DataBinding(type, param, tls, sockopt);
            Connect(type, param);
            AttachEvent(inbound, v4mode, insert);
            InitComboBox(type);
        }

        #region properties
        bool isServer { get; set; }
        bool isV4Mode { get; set; }

        private int _streamType;
        public int streamType
        {
            get { return _streamType; }
            set { SetField(ref _streamType, value); }
        }

        private string _streamParamText;
        public string streamParamText
        {
            get { return _streamParamText; }
            set { SetField(ref _streamParamText, value); }
        }

        bool _isUseTls;
        public bool isUseTls
        {
            get { return _isUseTls; }
            set { SetField(ref _isUseTls, value); }
        }

        bool _isUseSockopt;
        public bool isUseSockopt
        {
            get { return _isUseSockopt; }
            set { SetField(ref _isUseSockopt, value); }
        }

        #endregion

        #region public method
        public override void Update(JObject config)
        {
            var GetStr = Misc.Utils.GetStringByPrefixAndKeyHelper(config);

            var root = Misc.Utils.GetConfigRoot(isServer, isV4Mode);

            string prefix = root + ".streamSettings";

            var index = GetIndexByNetwork(GetStr(prefix, "network"));
            streamType = index;
            streamParamText =
                index < 0
                    ? string.Empty
                    : GetStr(prefix, Models.Datas.Table.streamSettings[index].paths[0]).ToLower();

            isUseSockopt = Misc.Utils.GetKey(config, prefix + ".sockopt") != null;
            isUseTls = GetStr(prefix, "security") == "tls";
        }
        #endregion

        #region private method
        void InitComboBox(ComboBox cboxType)
        {
            var streamType = new Dictionary<int, string>();
            foreach (var type in Models.Datas.Table.streamSettings)
            {
                streamType.Add(type.Key, type.Value.name);
            }
            Misc.UI.FillComboBox(cboxType, streamType);
        }

        void Connect(ComboBox type, ComboBox param)
        {
            type.SelectedIndexChanged += (sender, arg) =>
            {
                var index = type.SelectedIndex;

                if (index < 0)
                {
                    param.SelectedIndex = -1;
                    param.Items.Clear();
                    return;
                }

                var s = Models.Datas.Table.streamSettings[index];

                param.Items.Clear();

                if (!s.dropDownStyle)
                {
                    param.DropDownStyle = ComboBoxStyle.DropDown;
                    return;
                }

                param.DropDownStyle = ComboBoxStyle.DropDownList;
                foreach (var option in s.options)
                {
                    param.Items.Add(option.Key);
                }
            };
        }

        void AttachEvent(RadioButton inbound, CheckBox v4mode, Button insert)
        {
            v4mode.CheckedChanged += (s, a) =>
            {
                this.isV4Mode = v4mode.Checked;
                this.Update(container.config);
            };

            inbound.CheckedChanged += (s, a) =>
            {
                this.isServer = inbound.Checked;
                this.Update(container.config);
            };

            insert.Click += (s, a) =>
            {
                container.InjectConfigHelper(() =>
                {
                    Inject(ref container.config);
                });
            };
        }

        void DataBinding(ComboBox type, ComboBox param, CheckBox tls, CheckBox sockopt)
        {
            var bs = new BindingSource();
            bs.DataSource = this;

            tls.DataBindings.Add(
                nameof(CheckBox.Checked),
                bs,
                nameof(this.isUseTls),
                true,
                DataSourceUpdateMode.OnPropertyChanged
            );

            sockopt.DataBindings.Add(
                nameof(CheckBox.Checked),
                bs,
                nameof(this.isUseSockopt),
                true,
                DataSourceUpdateMode.OnPropertyChanged
            );

            type.DataBindings.Add(
                nameof(ComboBox.SelectedIndex),
                bs,
                nameof(this.streamType),
                true,
                DataSourceUpdateMode.OnValidation
            );

            param.DataBindings.Add(
                nameof(ComboBox.Text),
                bs,
                nameof(this.streamParamText),
                true,
                DataSourceUpdateMode.OnPropertyChanged
            );
        }

        void Inject(ref JObject config)
        {
            var root = Misc.Utils.GetConfigRoot(isServer, isV4Mode);
            var path = root + ".streamSettings";
            var stream = Misc.Utils.CreateJObject(path, GetSettings());

            try
            {
                Misc.Utils.RemoveKeyFromJObject(config, path);
            }
            catch (KeyNotFoundException) { }

            Misc.Utils.MergeJson(config, stream);
        }

        JToken GetSettings()
        {
            streamType = Misc.Utils.Clamp(streamType, 0, Models.Datas.Table.streamSettings.Count);

            var key = "none";
            var s = Models.Datas.Table.streamSettings[streamType];
            if (s.dropDownStyle && s.options.ContainsKey(streamParamText))
            {
                key = streamParamText;
            }

            var tpl = cache.tpl.LoadTemplate(s.options[key]) as JObject;
            if (!s.dropDownStyle)
            {
                Misc.Utils.TrySetValue<string>(tpl, s.paths[0], streamParamText);
            }

            InsertTLSSettings(tpl);
            InsertSocksMark(tpl);
            return tpl;
        }

        int GetIndexByNetwork(string network)
        {
            if (string.IsNullOrEmpty(network))
            {
                return -1;
            }

            foreach (var item in Models.Datas.Table.streamSettings)
            {
                if (item.Value.network == network)
                {
                    return item.Key;
                }
            }

            return -1;
        }

        void InsertSocksMark(JToken streamSettings)
        {
            if (isUseSockopt)
            {
                streamSettings["sockopt"] = JToken.Parse(@"{mark:0,tcpFastOpen:true,tproxy:'off'}");
                return;
            }
        }

        void InsertTLSSettings(JToken streamSettings)
        {
            var tlsTpl = cache.tpl.LoadTemplate("tls");
            if (isUseTls)
            {
                streamSettings["security"] = "tls";
                streamSettings["tlsSettings"] = tlsTpl;
            }
            else
            {
                streamSettings["security"] = "none";
            }
        }
        #endregion
    }
}
