using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using V2RayGCon.Resources.Resx;
using VgcApis.Interfaces;

namespace V2RayGCon.Views.WinForms
{
    public partial class FormModifyServerSettings : Form
    {
        #region Sigleton
        static FormModifyServerSettings _instant;
        static readonly object formInstanLocker = new object();
        public static void ShowForm(ICoreServCtrl coreServ)
        {
            lock (formInstanLocker)
            {
                if (_instant == null || _instant.IsDisposed)
                {
                    VgcApis.Misc.UI.Invoke(() =>
                    {
                        _instant = new FormModifyServerSettings();
                    });
                }
                VgcApis.Misc.UI.Invoke(() =>
                {
                    _instant.InitControls(coreServ);
                    _instant.Show();
                    _instant.Activate();
                });
            }
        }
        #endregion

        private ICoreServCtrl coreServ;
        VgcApis.Models.Datas.CoreServSettings orgCoreServSettings;
        Services.Servers servers;

        public FormModifyServerSettings()
        {
            InitializeComponent();
            VgcApis.Misc.UI.AutoSetFormIcon(this);
            servers = Services.Servers.Instance;
        }

        private void FormModifyServerSettings_Load(object sender, System.EventArgs e)
        {
            InitCboxStreamType();
            Misc.UI.FillComboBox(cboxOutbMethod, Models.Datas.Table.ssMethods);
            cboxShareLinkType.SelectedIndex = 1;
            cboxZoomMode.SelectedIndex = 0;
        }

        void InitControls(ICoreServCtrl coreServ)
        {
            this.coreServ = coreServ;
            orgCoreServSettings = new VgcApis.Models.Datas.CoreServSettings(coreServ);
            var marks = servers.GetMarkList();
            lbServerTitle.Text = coreServ.GetCoreStates().GetTitle();
            cboxMark.Items.Clear();
            cboxMark.Items.AddRange(marks);
            Misc.UI.ResetComboBoxDropdownMenuWidth(cboxMark);
            UpdateControls(orgCoreServSettings);
            UpdateShareLink();
            InitOutbControls();
        }

        #region private methods

        void InitOutbControls()
        {
            var config = coreServ.GetConfiger().GetConfig();
            var sc = new Models.Datas.ServerConfigs(config);
            SelectByText(cboxOutbProto, sc.proto);
            tboxOutbAddr.Text = sc.addr;
            tboxOutbAuth1.Text = sc.auth1;
            tboxOutbAuth2.Text = sc.auth2;
            SelectByText(cboxOutbMethod, sc.method);
            chkOutbOTA.Checked = sc.useOta;
            chkOutbStreamUseTls.Checked = sc.useTls;
            SelectByText(cboxOutbStreamType, sc.streamType);
            cboxOutbStreamParma.Text = sc.streamParam;
        }

        void SelectByText(ComboBox cbox, string text)
        {
            var idx = -1;
            foreach (string item in cbox.Items)
            {
                idx++;
                if (item.ToLower() == text)
                {
                    break;
                }
            }
            cbox.SelectedIndex = idx;
        }

        VgcApis.Models.Datas.CoreServSettings GetterSettings()
        {
            var result = new VgcApis.Models.Datas.CoreServSettings();
            result.serverName = tboxServerName.Text;
            result.serverDescription = tboxDescription.Text;
            result.inboundMode = cboxInboundMode.SelectedIndex;
            result.inboundAddress = cboxInboundAddress.Text;
            result.mark = cboxMark.Text;
            result.remark = tboxRemark.Text;
            result.isAutorun = chkAutoRun.Checked;
            result.isBypassCnSite = chkBypassCnSite.Checked;
            result.isGlobalImport = chkGlobalImport.Checked;
            result.isUntrack = chkUntrack.Checked;
            return result;
        }

        void UpdateControls(VgcApis.Models.Datas.CoreServSettings coreServSettings)
        {
            var s = coreServSettings;
            tboxServerName.Text = s.serverName;
            tboxDescription.Text = s.serverDescription;
            cboxInboundMode.SelectedIndex = s.inboundMode;
            cboxInboundAddress.Text = s.inboundAddress;
            cboxMark.Text = s.mark;
            tboxRemark.Text = s.remark;
            chkAutoRun.Checked = s.isAutorun;
            chkBypassCnSite.Checked = s.isBypassCnSite;
            chkGlobalImport.Checked = s.isGlobalImport;
            chkUntrack.Checked = s.isUntrack;
        }

        void InitCboxStreamType()
        {
            var streamType = new Dictionary<int, string>();
            foreach (var type in Models.Datas.Table.streamSettings)
            {
                streamType.Add(type.Key, type.Value.name);
            }
            Misc.UI.FillComboBox(cboxOutbStreamType, streamType);
        }

        void UpdateShareLink()
        {
            var slinkMgr = Services.ShareLinkMgr.Instance;
            var config = coreServ.GetConfiger().GetConfig();
            var ty = cboxShareLinkType.Text.ToLower() == "vee" ?
                VgcApis.Models.Datas.Enums.LinkTypes.v :
                VgcApis.Models.Datas.Enums.LinkTypes.vmess;
            var link = slinkMgr.EncodeConfigToShareLink(config, ty);
            tboxShareLink.Text = link;
        }

        void SetQRCodeImage(Image img)
        {
            var oldImage = pboxQrcode.Image;

            pboxQrcode.Image = img;

            if (oldImage != img)
            {
                oldImage?.Dispose();
            }
        }

        #endregion

        #region UI events
        private void cboxInboundAddress_TextChanged(object sender, System.EventArgs e)
        {
            VgcApis.Misc.UI.MarkInvalidAddressWithColorRed(cboxInboundAddress);
        }

        private void cboxInboundMode_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            var idx = cboxInboundMode.SelectedIndex;
            cboxInboundAddress.Enabled = idx == 1 || idx == 2;
        }

        private void cboxOutbProto_SelectedValueChanged(object sender, System.EventArgs e)
        {
            var t = cboxOutbProto.Text.ToLower();

            switch (t)
            {
                case @"vmess":
                case @"vless":
                    lbOutbAuth2.Visible = false;
                    cboxOutbMethod.Visible = false;
                    chkOutbOTA.Visible = false;
                    tboxOutbAuth2.Visible = false;
                    lbOutbAuth1.Text = @"UUID";
                    break;
                case @"socks":
                case @"http":
                    lbOutbAuth1.Text = I18N.User;
                    lbOutbAuth2.Text = I18N.Password;
                    lbOutbAuth2.Visible = true;
                    cboxOutbMethod.Visible = false;
                    chkOutbOTA.Visible = false;
                    tboxOutbAuth2.Visible = true;
                    break;
                case @"shadowsocks":
                    lbOutbAuth1.Text = I18N.Password;
                    lbOutbAuth2.Text = @"Method";
                    lbOutbAuth2.Visible = true;
                    cboxOutbMethod.Visible = true;
                    chkOutbOTA.Visible = true;
                    tboxOutbAuth2.Visible = false;
                    break;
                default:
                    break;
            }
        }

        private void cboxOutbStreamType_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            var index = cboxOutbStreamType.SelectedIndex;

            if (index < 0)
            {
                cboxOutbStreamParma.SelectedIndex = -1;
                cboxOutbStreamParma.Items.Clear();
                return;
            }

            var s = Models.Datas.Table.streamSettings[index];

            cboxOutbStreamParma.Items.Clear();

            if (!s.dropDownStyle)
            {
                cboxOutbStreamParma.DropDownStyle = ComboBoxStyle.DropDown;
                return;
            }

            cboxOutbStreamParma.DropDownStyle = ComboBoxStyle.DropDownList;
            foreach (var option in s.options)
            {
                cboxOutbStreamParma.Items.Add(option.Key);
            }
        }

        private void tboxShareLink_TextChanged(object sender, System.EventArgs e)
        {
            var text = tboxShareLink.Text;
            if (string.IsNullOrEmpty(text))
            {
                SetQRCodeImage(null);
                return;
            }

            Tuple<Bitmap, Libs.QRCode.QRCode.WriteErrors> r =
               Libs.QRCode.QRCode.GenQRCode(text, 320);

            switch (r.Item2)
            {
                case Libs.QRCode.QRCode.WriteErrors.Success:
                    SetQRCodeImage(r.Item1);
                    break;
                case Libs.QRCode.QRCode.WriteErrors.DataEmpty:
                    SetQRCodeImage(null);
                    MessageBox.Show(I18N.EmptyLink);
                    break;
                case Libs.QRCode.QRCode.WriteErrors.DataTooBig:
                    SetQRCodeImage(null);
                    MessageBox.Show(I18N.DataTooBig);
                    break;
            }

        }

        private void cboxShareLinkType_SelectedValueChanged(object sender, System.EventArgs e)
        {
            UpdateShareLink();
        }

        private void cboxZoomMode_SelectedValueChanged(object sender, System.EventArgs e)
        {
            pboxQrcode.SizeMode = cboxZoomMode.Text.ToLower() == "none" ?
                PictureBoxSizeMode.CenterImage :
                PictureBoxSizeMode.Zoom;
        }

        private void btnCopyShareLink_Click(object sender, EventArgs e)
        {
            Misc.Utils.CopyToClipboardAndPrompt(tboxShareLink.Text);
        }

        private void btnSaveQrcode_Click(object sender, EventArgs e)
        {
            Stream myStream;
            SaveFileDialog saveFileDialog1 = new SaveFileDialog
            {
                Filter = VgcApis.Models.Consts.Files.PngExt,
                FilterIndex = 1,
                RestoreDirectory = true,
            };

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if ((myStream = saveFileDialog1.OpenFile()) != null)
                {
                    pboxQrcode.Image.Save(myStream, System.Drawing.Imaging.ImageFormat.Png);
                    myStream.Close();
                }
            }
        }

        private void tboxOutbAddr_TextChanged(object sender, EventArgs e)
        {
            var t = tboxOutbAddr.Text;
            var ok = VgcApis.Misc.Utils.TryParseAddress(t, out string ip, out int port);
            var color = ok ? Color.Black : Color.Red;
            if (tboxOutbAddr.ForeColor != color)
            {
                tboxOutbAddr.ForeColor = color;
            }
        }

        private void lbOutbAuth1_Click(object sender, EventArgs e)
        {
            // tboxOutbAuth1.Text = Guid.NewGuid().ToString();
            var msg = VgcApis.Misc.Utils.CopyToClipboard(tboxOutbAuth1.Text) ?
                 I18N.CopySuccess : I18N.CopyFail;
            VgcApis.Misc.UI.MsgBoxAsync(msg);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            var curSettings = GetterSettings();
            if (!curSettings.Equals(orgCoreServSettings))
            {
                coreServ.UpdateCoreSettings(curSettings);
                servers.UpdateMarkList();
            }
            Close();
        }

        #endregion

        private void btnOpenInEditor_Click(object sender, EventArgs e)
        {
            var config = coreServ.GetConfiger().GetConfig();
            WinForms.FormConfiger.ShowConfig(config);
        }
    }
}
