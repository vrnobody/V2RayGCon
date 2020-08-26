using System;
using System.Collections.Generic;
using System.Windows.Forms;
using V2RayGCon.Resources.Resx;

namespace V2RayGCon.Views.UserControls
{
    public partial class VeeConfigerUI : UserControl
    {
        public VeeConfigerUI()
        {
            InitializeComponent();
            InitControls();
        }

        private void VeeImporter_Load(object sender, EventArgs e)
        {
        }

        public string ToVeeShareLink()
        {
            var sc = new Models.Datas.VeeConfigs();
            sc.name = tboxName.Text;
            sc.description = tboxDescription.Text;
            sc.proto = cboxProtocol.Text;
            sc.host = tboxHost.Text;
            sc.port = VgcApis.Misc.Utils.Str2Int(tboxPort.Text);
            sc.auth1 = tboxAuth1.Text;
            sc.method = cboxMethod.Text;
            sc.useOta = chkOTA.Checked;
            sc.streamType = cboxStreamType.Text;
            sc.useTls = chkStreamUseTls.Checked;
            sc.useSelfSignCert = chkStreamUseSelfSignCert.Checked;
            sc.streamParam1 = cboxStreamParma1.Text;
            sc.streamParam2 = tboxStreamParam2.Text;
            sc.streamParam3 = tboxStreamParam3.Text;

            return sc.ToVeeShareLink();
        }

        public void FromCoreConfig(string config)
        {
            var sc = new Models.Datas.VeeConfigs(config);
            tboxName.Text = sc.name;
            tboxDescription.Text = sc.description;

            SelectByText(cboxProtocol, sc.proto);

            tboxHost.Text = sc.host;
            tboxPort.Text = sc.port.ToString();
            tboxAuth1.Text = sc.auth1;

            SelectByText(cboxMethod, sc.method);

            chkOTA.Checked = sc.useOta;

            var st = string.IsNullOrEmpty(sc.streamType) ? StreamTypeNone : sc.streamType;
            SelectByText(cboxStreamType, st);

            chkStreamUseTls.Checked = sc.useTls;
            chkStreamUseSelfSignCert.Checked = sc.useSelfSignCert;
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
        }
        #region public methods

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
            Misc.UI.FillComboBox(cboxMethod, Models.Datas.Table.ssMethods);

            var streamType = new List<string> { StreamTypeNone };
            foreach (var type in Models.Datas.Table.streamSettings)
            {
                var network = type.Value.network;
                if (network == "domainsocket")
                {
                    continue;
                }
                streamType.Add(type.Value.network);
            }
            Misc.UI.FillComboBox(cboxStreamType, streamType);
            cboxProtocol.SelectedIndex = 4;
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

        #endregion

        #region UI events


        private void cboxStreamType_SelectedValueChanged(object sender, EventArgs e)
        {
            var network = cboxStreamType.Text;
            var opts = GetStreamComponet(network);

            var enableStreamParams = opts != null;
            cboxStreamParma1.Enabled = enableStreamParams;
            chkStreamUseTls.Enabled = enableStreamParams;
            chkStreamUseSelfSignCert.Enabled = enableStreamParams;
            tboxStreamParam2.Enabled = enableStreamParams;
            tboxStreamParam3.Enabled = enableStreamParams;

            if (!enableStreamParams)
            {
                return;
            }

            cboxStreamParma1.Items.Clear();
            cboxStreamParma1.DropDownStyle = opts.dropDownStyle ?
                ComboBoxStyle.DropDownList : ComboBoxStyle.DropDown;
            if (opts.dropDownStyle && opts.options.Count > 0)
            {
                foreach (var kv in opts.options)
                {
                    cboxStreamParma1.Items.Add(kv.Key);
                }
            }

            if (string.IsNullOrEmpty(opts.option2Name))
            {
                tboxStreamParam2.Enabled = false;
            }
            else
            {
                lbStreamParam2.Text = opts.option2Name;
                tboxStreamParam2.Enabled = true;
            }

            if (string.IsNullOrEmpty(opts.option3Name))
            {
                tboxStreamParam3.Enabled = false;
            }
            else
            {
                lbStreamParam3.Text = opts.option3Name;
                tboxStreamParam3.Enabled = true;
            }

        }

        private void cboxProtocol_SelectedValueChanged(object sender, EventArgs e)
        {
            var t = cboxProtocol.Text.ToLower();

            switch (t)
            {
                case @"vmess":
                case @"vless":
                    lbAuth2.Visible = false;
                    cboxMethod.Visible = false;
                    chkOTA.Visible = false;
                    tboxAuth2.Visible = false;
                    lbAuth1.Text = @"UUID";
                    break;
                case @"socks":
                case @"http":
                    lbAuth1.Text = I18N.User;
                    lbAuth2.Text = I18N.Password;
                    lbAuth2.Visible = true;
                    cboxMethod.Visible = false;
                    chkOTA.Visible = false;
                    tboxAuth2.Visible = true;
                    break;
                case @"shadowsocks":
                    lbAuth1.Text = I18N.Password;
                    lbAuth2.Text = @"Method";
                    lbAuth2.Visible = true;
                    cboxMethod.Visible = true;
                    chkOTA.Visible = true;
                    tboxAuth2.Visible = false;
                    break;
                default:
                    break;
            }
        }

        private void lbAuth1_Click(object sender, EventArgs e)
        {
            tboxAuth1.Text = Guid.NewGuid().ToString();
        }


        private void cboxStreamParma1_SelectedValueChanged(object sender, EventArgs e)
        {
            var disable = cboxStreamType.Text == "tcp" && cboxStreamParma1.Text == "none";
            tboxStreamParam2.Enabled = !disable;
            tboxStreamParam3.Enabled = !disable;
        }
        #endregion

    }
}
