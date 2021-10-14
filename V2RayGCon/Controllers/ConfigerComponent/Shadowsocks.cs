using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Windows.Forms;

namespace V2RayGCon.Controllers.ConfigerComponet
{
    class Shadowsocks : ConfigerComponentController
    {
        Services.Cache cache;
        public Shadowsocks(
            RadioButton rbtnInboundMode,
            CheckBox chkV4Mode,
            TextBox tboxAddress,
            TextBox tboxPassword,
            CheckBox chkIsShowPassword,
            ComboBox cboxMethods,
            CheckBox chkIsUseOTA,
            Button btnInsert)
        {
            cache = Services.Cache.Instance;
            isServerMode = false;
            isV4Mode = false;

            Misc.UI.FillComboBox(cboxMethods, Models.Datas.Table.ssMethods);
            Misc.UI.ResetComboBoxDropdownMenuWidth(cboxMethods);

            AttachEvents(tboxPassword, chkIsShowPassword, btnInsert, rbtnInboundMode, chkV4Mode);
            DataBinding(tboxAddress, tboxPassword, cboxMethods, chkIsUseOTA);
        }


        #region properties
        private bool isServerMode { get; set; }
        private bool isV4Mode { get; set; }

        private int _methodTypeIndex;
        public int methodTypeIndex
        {
            get { return _methodTypeIndex; }
            set { SetField(ref _methodTypeIndex, value); }
        }

        private string _address;
        public string address
        {
            get { return _address; }
            set { SetField(ref _address, value); }
        }

        private string _password;
        public string password
        {
            get { return _password; }
            set { SetField(ref _password, value); }
        }

        bool _isUseOTA;
        public bool isUseOTA
        {
            get { return _isUseOTA; }
            set { SetField(ref _isUseOTA, value); }
        }
        #endregion

        #region public method
        public override void Update(JObject config)
        {
            var GetStr = Misc.Utils.GetStringByPrefixAndKeyHelper(config);

            var root = Misc.Utils.GetConfigRoot(isServerMode, isV4Mode);

            var protocol = Misc.Utils.GetValue<string>(config, root, "protocol");
            if (protocol.ToLower() != "shadowsocks")
            {
                EmptyAllControl();
                return;
            }

            var prefix = root + ".settings" + (isServerMode ? "" : ".servers.0");

            this.isUseOTA = Misc.Utils.GetValue<bool>(config, prefix, "ota");
            this.password = GetStr(prefix, "password");
            this.address = isServerMode ?
                Misc.Utils.GetAddr(config, root, "listen", "port") :
                Misc.Utils.GetAddr(config, prefix, "address", "port");
            SetMethodTypeIndex(GetStr(prefix, "method"));
        }
        #endregion

        #region private method
        private void EmptyAllControl()
        {
            this.address = string.Empty;
            this.methodTypeIndex = -1;
            this.password = string.Empty;
            this.isUseOTA = false;
        }

        void SetMethodTypeIndex(string selectedMethod)
        {
            this.methodTypeIndex = Misc.Utils.GetIndexIgnoreCase(
                Models.Datas.Table.ssMethods,
                selectedMethod);
        }

        private void DataBinding(TextBox tboxAddress, TextBox tboxPassword, ComboBox cboxMethods, CheckBox chkIsUseOTA)
        {
            var bs = new BindingSource();
            bs.DataSource = this;

            tboxAddress.DataBindings.Add("Text", bs, nameof(this.address));
            tboxPassword.DataBindings.Add("Text", bs, nameof(this.password));

            chkIsUseOTA.DataBindings.Add(
                nameof(CheckBox.Checked),
                bs,
                nameof(this.isUseOTA),
                true,
                DataSourceUpdateMode.OnPropertyChanged);

            cboxMethods.DataBindings.Add(
                nameof(ComboBox.SelectedIndex),
                bs,
                nameof(this.methodTypeIndex),
                true,
                DataSourceUpdateMode.OnPropertyChanged);
        }

        private void AttachEvents(
            TextBox tboxPassword,
            CheckBox chkIsShowPassword,
            Button btnInsert,
            RadioButton rbtnIsServerMode,
            CheckBox chkV4Mode)
        {
            chkV4Mode.CheckedChanged += (s, a) =>
            {
                this.isV4Mode = chkV4Mode.Checked;
                this.Update(container.config);
            };

            rbtnIsServerMode.CheckedChanged += (s, a) =>
            {
                this.isServerMode = rbtnIsServerMode.Checked;
                this.Update(container.config);
            };

            chkIsShowPassword.CheckedChanged += (s, a) =>
            {
                tboxPassword.PasswordChar = chkIsShowPassword.Checked ? '\0' : '*';
            };

            btnInsert.Click += (s, a) =>
            {
                container.InjectConfigHelper(() =>
                {
                    Inject(ref container.config);
                });
            };
        }

        void Inject(ref JObject config)
        {
            var root = Misc.Utils.GetConfigRoot(isServerMode, isV4Mode);
            var ss = Misc.Utils.CreateJObject(root, GetSettings());
            try
            {
                Misc.Utils.RemoveKeyFromJObject(config, root + ".settings");
            }
            catch (KeyNotFoundException) { }
            Misc.Utils.MergeJson(config, ss);
        }

        JToken GetSettings()
        {
            var ssMethods = Models.Datas.Table.ssMethods;
            var tpl = cache.tpl.LoadTemplate(isServerMode ? "ssServer" : "ssClient");
            VgcApis.Misc.Utils.TryParseAddress(this.address, out string ip, out int port);
            var index = this.methodTypeIndex;
            var methodName = index < 0 ? ssMethods[0] : ssMethods[index];

            if (isServerMode)
            {
                tpl["port"] = port;
                tpl["listen"] = ip;
                tpl["settings"]["method"] = methodName;
                tpl["settings"]["password"] = this.password;
                tpl["settings"]["ota"] = this.isUseOTA;
            }
            else
            {
                tpl["settings"]["servers"][0]["address"] = ip;
                tpl["settings"]["servers"][0]["port"] = port;
                tpl["settings"]["servers"][0]["method"] = methodName;
                tpl["settings"]["servers"][0]["password"] = this.password;
                tpl["settings"]["servers"][0]["ota"] = this.isUseOTA;
            }

            return tpl;
        }

        #endregion

    }

}
