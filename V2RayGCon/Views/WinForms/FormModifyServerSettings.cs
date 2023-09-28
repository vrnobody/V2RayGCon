using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
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
            FormModifyServerSettings form = null;

            if (_instant == null || _instant.IsDisposed)
            {
                VgcApis.Misc.UI.Invoke(() =>
                {
                    form = new FormModifyServerSettings();
                });
            }

            lock (formInstanLocker)
            {
                if (_instant == null || _instant.IsDisposed)
                {
                    _instant = form;
                    form.FormClosed += (s, a) => _instant = null;
                    form = null;
                }
            }

            VgcApis.Misc.UI.Invoke(() =>
            {
                form?.Close();
                var inst = _instant;
                if (inst != null)
                {
                    inst.InitControls(coreServ);
                    inst.Show();
                    inst.Activate();
                }
            });
        }
        #endregion

        private ICoreServCtrl coreServ;
        VgcApis.Models.Datas.CoreServSettings orgCoreServSettings;
        readonly Services.Servers servers;
        readonly Services.Settings settings;

        public FormModifyServerSettings()
        {
            InitializeComponent();
            VgcApis.Misc.UI.AutoSetFormIcon(this);
            servers = Services.Servers.Instance;
            settings = Services.Settings.Instance;
        }

        private void FormModifyServerSettings_Load(object sender, EventArgs e)
        {
            cboxZoomMode.SelectedIndex = 0;
        }

        void InitControls(ICoreServCtrl coreServ)
        {
            this.coreServ = coreServ;
            orgCoreServSettings = new VgcApis.Models.Datas.CoreServSettings(coreServ);
            var marks = servers.GetMarkList();
            lbServerTitle.Text = coreServ.GetCoreStates().GetTitle();

            cboxMark.Items.AddRange(marks);
            VgcApis.Misc.UI.ResetComboBoxDropdownMenuWidth(cboxMark);

            var coreNames = settings.GetCustomCoresSetting().Select(cs => cs.name).ToArray();
            cboxCoreName.Items.AddRange(coreNames);
            VgcApis.Misc.UI.ResetComboBoxDropdownMenuWidth(cboxCoreName);

            var inbNames = settings.GetCustomInboundsSetting().Select(inb => inb.name).ToArray();
            cboxInboundName.Items.AddRange(inbNames);
            VgcApis.Misc.UI.ResetComboBoxDropdownMenuWidth(cboxInboundName);

            UpdateControls(orgCoreServSettings);
            AutoSelectShareLinkType();
            UpdateShareLink();
        }

        #region private methods
        void AutoSelectShareLinkType()
        {
            var slinkMgr = Services.ShareLinkMgr.Instance;
            var config = coreServ.GetConfiger().GetConfig();
            var name = coreServ.GetCoreStates().GetName();
            var ts = new List<VgcApis.Models.Datas.Enums.LinkTypes>
            {
                VgcApis.Models.Datas.Enums.LinkTypes.vmess,
                VgcApis.Models.Datas.Enums.LinkTypes.vless,
                VgcApis.Models.Datas.Enums.LinkTypes.trojan,
                VgcApis.Models.Datas.Enums.LinkTypes.ss,
            };

            for (int i = 0; i < ts.Count; i++)
            {
                if (!string.IsNullOrEmpty(slinkMgr.EncodeConfigToShareLink(name, config, ts[i])))
                {
                    cboxShareLinkType.SelectedIndex = i;
                    return;
                }
            }
            cboxShareLinkType.SelectedIndex = 0;
        }

        VgcApis.Models.Datas.CoreServSettings GatherSettings()
        {
            var result = new VgcApis.Models.Datas.CoreServSettings
            {
                index = VgcApis.Misc.Utils.Str2Int(tboxServIndex.Text),
                serverName = tboxServerName.Text,
                inboundName = cboxInboundName.Text,
                inboundAddress = cboxInboundAddress.Text,
                mark = cboxMark.Text,
                remark = tboxRemark.Text,
                customCoreName = cboxCoreName.SelectedIndex < 1 ? string.Empty : cboxCoreName.Text,
                isAutorun = chkAutoRun.Checked,
                isUntrack = chkUntrack.Checked
            };
            return result;
        }

        void UpdateControls(VgcApis.Models.Datas.CoreServSettings coreServSettings)
        {
            var s = coreServSettings;
            tboxServIndex.Text = s.index.ToString();
            tboxServerName.Text = s.serverName;
            cboxInboundAddress.Text = s.inboundAddress;
            cboxMark.Text = s.mark;
            tboxRemark.Text = s.remark;

            VgcApis.Misc.UI.SelectComboxByText(cboxCoreName, s.customCoreName);
            VgcApis.Misc.UI.SelectComboxByText(cboxInboundName, s.inboundName);

            chkAutoRun.Checked = s.isAutorun;
            chkUntrack.Checked = s.isUntrack;
        }

        void UpdateShareLink()
        {
            var slinkMgr = Services.ShareLinkMgr.Instance;
            var name = coreServ.GetCoreStates().GetName();
            var config = coreServ.GetConfiger().GetConfig();
            var ty = VgcApis.Models.Datas.Enums.LinkTypes.ss;
            switch (cboxShareLinkType.Text.ToLower())
            {
                case "vmess":
                    ty = VgcApis.Models.Datas.Enums.LinkTypes.vmess;
                    break;
                case "vless":
                    ty = VgcApis.Models.Datas.Enums.LinkTypes.vless;
                    break;
                case "trojan":
                    ty = VgcApis.Models.Datas.Enums.LinkTypes.trojan;
                    break;
                default:
                    break;
            }
            var link = slinkMgr.EncodeConfigToShareLink(name, config, ty);
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
        private void cboxInboundAddress_TextChanged(object sender, EventArgs e)
        {
            VgcApis.Misc.UI.MarkInvalidAddressWithColorRed(cboxInboundAddress);
        }

        private void tboxShareLink_TextChanged(object sender, EventArgs e)
        {
            var text = tboxShareLink.Text;
            if (string.IsNullOrEmpty(text))
            {
                SetQRCodeImage(null);
                return;
            }

            Tuple<Bitmap, Libs.QRCode.QRCode.WriteErrors> r = Libs.QRCode.QRCode.GenQRCode(
                text,
                320
            );

            switch (r.Item2)
            {
                case Libs.QRCode.QRCode.WriteErrors.Success:
                    SetQRCodeImage(r.Item1);
                    break;
                case Libs.QRCode.QRCode.WriteErrors.DataEmpty:
                    SetQRCodeImage(null);
                    VgcApis.Misc.UI.MsgBox(I18N.EmptyLink);
                    break;
                case Libs.QRCode.QRCode.WriteErrors.DataTooBig:
                    SetQRCodeImage(null);
                    VgcApis.Misc.UI.MsgBox(I18N.DataTooBig);
                    break;
            }
        }

        private void cboxShareLinkType_SelectedValueChanged(object sender, EventArgs e)
        {
            UpdateShareLink();
        }

        private void cboxZoomMode_SelectedValueChanged(object sender, EventArgs e)
        {
            pboxQrcode.SizeMode =
                cboxZoomMode.Text.ToLower() == "none"
                    ? PictureBoxSizeMode.CenterImage
                    : PictureBoxSizeMode.Zoom;
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

        private void btnOK_Click(object sender, EventArgs e)
        {
            var curSettings = GatherSettings();
            if (!curSettings.Equals(orgCoreServSettings))
            {
                coreServ.UpdateCoreSettings(curSettings);
                servers.UpdateMarkList();
            }
            Close();
        }

        #endregion
    }
}
