using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace V2RayGCon.Controller.ConfigerComponet
{
    class Vmess : ConfigerComponentController
    {
        // textbox [id, level, aid, ipaddr]
        public Vmess(
            TextBox id,
            TextBox level,
            TextBox alterID,
            TextBox ipAddr,
            RadioButton inbound,
            CheckBox v4,
            Button genID,
            Button insert)
        {
            serverMode = false;
            v4Mode = false;
            DataBinding(id, level, alterID, ipAddr);
            AttachEvent(inbound, v4, genID, insert);
        }

        #region properties
        public bool serverMode, v4Mode;

        private string _ID;
        public string ID
        {
            get { return _ID; }
            set { SetField(ref _ID, value); }
        }

        private string _level;
        public string level
        {
            get { return _level; }
            set { SetField(ref _level, value); }
        }

        private string _altID;
        public string altID
        {
            get { return _altID; }
            set { SetField(ref _altID, value); }
        }

        private string _address;
        public string address
        {
            get { return _address; }
            set
            {
                SetField(ref _address, value);
            }
        }
        #endregion

        #region private method
        void AttachEvent(
            RadioButton inbound,
            CheckBox v4,
            Button genID,
            Button insert)
        {
            v4.CheckedChanged += (s, a) =>
            {
                this.v4Mode = v4.Checked;
                this.Update(container.config);
            };

            inbound.CheckedChanged += (s, a) =>
            {
                this.serverMode = inbound.Checked;
                this.Update(container.config);
            };

            genID.Click += (s, a) =>
            {
                this.ID = Guid.NewGuid().ToString();
            };

            insert.Click += (s, a) =>
            {
                container.InjectConfigHelper(() =>
                {
                    Inject(ref container.config);
                });
            };
        }

        void DataBinding(TextBox id, TextBox level, TextBox alterID, TextBox ipAddr)
        {
            var bs = new BindingSource();
            bs.DataSource = this;

            id.DataBindings.Add("Text", bs, nameof(this.ID));
            level.DataBindings.Add("Text", bs, nameof(this.level));
            alterID.DataBindings.Add("Text", bs, nameof(this.altID));
            ipAddr.DataBindings.Add("Text", bs, nameof(this.address));
        }

        JToken GetSettings()
        {
            JToken vmess = Service.Cache.Instance.
                tpl.LoadTemplate(serverMode ?
                "vmessServer" : "vmessClient");

            VgcApis.Libs.Utils.TryParseIPAddr(this.address, out string ip, out int port);

            if (serverMode)
            {
                vmess["port"] = port;
                vmess["listen"] = ip;
                vmess["settings"]["clients"][0]["id"] = ID;
                vmess["settings"]["clients"][0]["level"] = Lib.Utils.Str2Int(level);
                vmess["settings"]["clients"][0]["alterId"] = Lib.Utils.Str2Int(altID);
            }
            else
            {
                vmess["settings"]["vnext"][0]["address"] = ip;
                vmess["settings"]["vnext"][0]["port"] = port;
                vmess["settings"]["vnext"][0]["users"][0]["id"] = ID;
                vmess["settings"]["vnext"][0]["users"][0]["alterId"] = Lib.Utils.Str2Int(altID);
                vmess["settings"]["vnext"][0]["users"][0]["level"] = Lib.Utils.Str2Int(level);
            }

            return vmess;
        }

        void Inject(ref JObject config)
        {
            var root = Lib.Utils.GetConfigRoot(serverMode, v4Mode);
            var vmess = Lib.Utils.CreateJObject(root, GetSettings());
            try
            {
                Lib.Utils.RemoveKeyFromJObject(config, root + ".settings");
            }
            catch (KeyNotFoundException) { }

            Lib.Utils.MergeJson(ref config, vmess);
        }

        void EmptyAllControl()
        {
            this.ID = string.Empty;
            this.level = string.Empty;
            this.altID = string.Empty;
            this.address = string.Empty;

        }
        #endregion

        #region public method
        public override void Update(JObject config)
        {
            var root = Lib.Utils.GetConfigRoot(serverMode, v4Mode);

            var protocol = Lib.Utils.GetValue<string>(config, root, "protocol");

            if (protocol.ToLower() != "vmess")
            {
                EmptyAllControl();
                return;
            }

            var prefix = root + ".settings."
                + (serverMode ? "clients.0" : "vnext.0.users.0");

            ID = Lib.Utils.GetValue<string>(config, prefix, "id");
            level = Lib.Utils.GetValue<int>(config, prefix, "level").ToString();
            altID = Lib.Utils.GetValue<int>(config, prefix, "alterId").ToString();

            address = serverMode ?
                Lib.Utils.GetAddr(config, root, "listen", "port") :
                Lib.Utils.GetAddr(
                    config,
                    root + ".settings.vnext.0",
                    "address", "port");
        }
        #endregion
    }
}
