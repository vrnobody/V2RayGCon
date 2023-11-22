using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using V2RayGCon.Resources.Resx;

namespace V2RayGCon.Views.UserControls
{
    public partial class SimpleConfigerUI : UserControl
    {
        public SimpleConfigerUI()
        {
            InitializeComponent();
            InitControls();
        }

        #region public methods
        public void SetTitle(string title)
        {
            lbName.Text = string.IsNullOrEmpty(title) ? I18N.NewServer : title;
            toolTip1.SetToolTip(lbName, title);
            tboxName.Visible = false;
        }

        public string ToShareLink()
        {
            var sc = new Models.Datas.SharelinkMetadata
            {
                name = tboxName.Text,
                proto = cboxProtocol.Text,
                host = tboxHost.Text,
                port = VgcApis.Misc.Utils.Str2Int(tboxPort.Text),
                auth1 = tboxAuth1.Text,
                auth2 = cboxAuth2.Text,
                streamType = cboxStreamType.Text,
                streamParam1 = cboxStreamParma1.Text,
                streamParam2 = tboxStreamParam2.Text,
                streamParam3 = tboxStreamParam3.Text,
                tlsType = cboxTlsType.Text,
                tlsParam1 = tboxTlsPublicKey.Text,
                tlsParam2 = tboxTlsShortId.Text,
                tlsParam3 = tboxTlsSpiderX.Text,
                tlsServName = tboxTlsServName.Text,
                useSelfSignCert = chkTlsCertSelfSign.Checked,
                tlsAlpn = tboxTlsAlpn.Text,
                tlsFingerPrint = cboxTlsFingerprint.Text,
            };

            return sc.ToShareLink();
        }

        public void FromCoreConfig(string config)
        {
            if (!Models.Datas.SharelinkMetadata.TryParseConfig(config, out var sc) || sc == null)
            {
                return;
            }

            tboxName.Text = sc.name;

            SelectByText(cboxProtocol, sc.proto);

            tboxHost.Text = sc.host;
            tboxPort.Text = sc.port.ToString();

            tboxAuth1.Text = sc.auth1;
            cboxAuth2.Text = sc.auth2;

            var st = string.IsNullOrEmpty(sc.streamType) ? StreamTypeNone : sc.streamType;
            SelectByText(cboxStreamType, st);

            if (cboxStreamParma1.DropDownStyle == ComboBoxStyle.DropDownList)
            {
                SelectByText(cboxStreamParma1, sc.streamParam1);
            }
            else
            {
                cboxStreamParma1.Text = sc.streamParam1;
            }
            tboxStreamParam2.Text = sc.streamParam2;
            tboxStreamParam3.Text = sc.streamParam3;

            var tt = string.IsNullOrEmpty(sc.tlsType) ? StreamTypeNone : sc.tlsType;
            SelectByText(cboxTlsType, tt);
            tboxTlsServName.Text = sc.tlsServName;
            chkTlsCertSelfSign.Checked = sc.useSelfSignCert;

            tboxTlsAlpn.Text = sc.tlsAlpn;
            cboxTlsFingerprint.Text = sc.tlsFingerPrint;
            tboxTlsPublicKey.Text = sc.tlsParam1;
            tboxTlsShortId.Text = sc.tlsParam2;
            tboxTlsSpiderX.Text = sc.tlsParam3;
        }
        #endregion

        #region private methods

        void SelectByText(ComboBox cbox, string content)
        {
            var idx = -1;
            foreach (var item in cbox.Items)
            {
                idx++;
                if (item.ToString() == content)
                {
                    cbox.SelectedIndex = idx;
                    return;
                }
            }
            cbox.SelectedIndex = -1;
        }

        const string StreamTypeNone = @"none";

        void InitControls()
        {
            var protocols = new List<string> { "vless", "trojan", "vmess", "shadowsocks", "socks" };

            Misc.UI.FillComboBox(cboxProtocol, protocols);
            var streamType = new List<string> { StreamTypeNone };
            foreach (var type in Models.Datas.Table.streamSettings)
            {
                streamType.Add(type.Value.network);
            }
            Misc.UI.FillComboBox(cboxStreamType, streamType);

            cboxProtocol.SelectedIndex = 0;
            cboxProtocol_SelectedValueChanged(this, EventArgs.Empty);
        }

        Models.Datas.StreamComponent GetStreamComponet(string network)
        {
            foreach (var kv in Models.Datas.Table.streamSettings)
            {
                var c = kv.Value;
                if (c.network == network)
                {
                    return c;
                }
            }
            return null;
        }

        void DisableStreamParamControlsWhenStreamTypeIsNone(bool isNone)
        {
            var enable = !isNone;

            cboxStreamParma1.Enabled = enable;
            cboxTlsType.Enabled = enable;
            if (!enable)
            {
                SelectByText(cboxTlsType, "none");
            }
            tboxStreamParam2.Enabled = enable;
            tboxStreamParam3.Enabled = enable;
        }

        private void ToggleVisibilityOfControls()
        {
            var t = cboxProtocol.Text.ToLower();

            switch (t)
            {
                case @"vmess":
                case @"vless":
                    lbAuth2.Visible = t == @"vless";
                    cboxAuth2.Visible = t == @"vless";
                    lbAuth1.Text = @"UUID";
                    lbAuth2.Text = @"flow";
                    break;
                case @"socks":
                case @"http":
                    lbAuth1.Text = I18N.User;
                    lbAuth2.Text = I18N.Password;
                    lbAuth2.Visible = true;
                    cboxAuth2.Visible = true;
                    break;
                case @"shadowsocks":
                    lbAuth1.Text = I18N.Password;
                    lbAuth2.Text = @"Method";
                    lbAuth2.Visible = true;
                    cboxAuth2.Visible = true;
                    break;
                case @"trojan":
                    lbAuth1.Text = I18N.Password;
                    lbAuth2.Visible = true;
                    cboxAuth2.Visible = true;
                    break;
                default:
                    break;
            }
        }

        void FillCboxAuth2()
        {
            var items = cboxAuth2.Items;

            var t = cboxProtocol.Text.ToLower();
            items.Clear();
            switch (t)
            {
                case @"vless":
                case @"trojan":
                    items.AddRange(
                        new string[]
                        {
                            "xtls-rprx-vision",
                            "xtls-rprx-vision-udp443",
                            "",
                            "xtls-rprx-splice",
                            "xtls-rprx-splice-udp443",
                        }
                    );
                    break;

                case @"shadowsocks":
                    items.AddRange(
                        new string[]
                        {
                            "none",
                            "aes-256-gcm",
                            "aes-128-gcm",
                            "chacha20-poly1305",
                            "chacha20-ietf-poly1305",
                        }
                    );
                    break;

                default:
                    break;
            }
            cboxAuth2.SelectedIndex = -1;
            cboxAuth2.Text = @"";
        }

        #endregion

        #region UI events

        private void cboxStreamType_SelectedValueChanged(object sender, EventArgs e)
        {
            var network = cboxStreamType.Text;
            var opts = GetStreamComponet(network);

            // stream = none or ...
            DisableStreamParamControlsWhenStreamTypeIsNone(opts == null);
            if (opts == null)
            {
                return;
            }

            cboxStreamParma1.Items.Clear();
            cboxStreamParma1.DropDownStyle = opts.dropDownStyle
                ? ComboBoxStyle.DropDownList
                : ComboBoxStyle.DropDown;
            if (opts.dropDownStyle && opts.options.Count > 0)
            {
                foreach (var kv in opts.options)
                {
                    cboxStreamParma1.Items.Add(kv.Key);
                }
            }

            var lbs = new List<Control> { lbStreamParam1, lbStreamParam2, lbStreamParam3 };
            var tboxes = new List<Control> { cboxStreamParma1, tboxStreamParam2, tboxStreamParam3 };
            var paths = opts.paths;
            for (int i = 0; i < lbs.Count; i++)
            {
                var visable = i < paths.Count;
                lbs[i].Visible = visable;
                tboxes[i].Visible = visable;
                if (visable)
                {
                    var name = paths[i].Split('.').Last();
                    lbs[i].Text = name;
                }
            }
        }

        private void cboxProtocol_SelectedValueChanged(object sender, EventArgs e)
        {
            ToggleVisibilityOfControls();
            FillCboxAuth2();
            ValidateAuth1();
        }

        private void lbAuth1_Click(object sender, EventArgs e)
        {
            tboxAuth1.Text = Guid.NewGuid().ToString();
        }

        private void cboxStreamParma1_SelectedValueChanged(object sender, EventArgs e)
        {
            if (cboxStreamType.Text == "tcp")
            {
                var enable = cboxStreamParma1.Text != "none";
                tboxStreamParam2.Enabled = enable;
                tboxStreamParam3.Enabled = enable;
            }
        }

        void ValidateAuth1()
        {
            var tag = lbAuth1.Text;
            var c = Color.Black;
            if (tag?.ToLower() == "uuid" && !Guid.TryParse(tboxAuth1.Text, out _))
            {
                c = Color.Red;
            }

            if (tboxAuth1.ForeColor != c)
            {
                tboxAuth1.ForeColor = c;
            }
        }

        private void tboxAuth1_TextChanged(object sender, EventArgs e)
        {
            ValidateAuth1();
        }

        private void cboxTlsType_SelectedValueChanged(object sender, EventArgs e)
        {
            bool tlsDisabled = cboxTlsType.Text == "none";
            bool realityEnabled = cboxTlsType.Text == "reality";

            chkTlsCertSelfSign.Enabled = !tlsDisabled;
            tboxTlsServName.Enabled = !tlsDisabled;
            cboxTlsFingerprint.Enabled = !tlsDisabled;
            tboxTlsAlpn.Enabled = !tlsDisabled;
            tboxTlsPublicKey.Enabled = !tlsDisabled && realityEnabled;
            tboxTlsShortId.Enabled = !tlsDisabled && realityEnabled;
            tboxTlsSpiderX.Enabled = !tlsDisabled && realityEnabled;
        }

        #endregion
    }
}
