using System;
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
        Services.Servers servers;

        public FormModifyServerSettings()
        {
            InitializeComponent();
            VgcApis.Misc.UI.AutoSetFormIcon(this);
            servers = Services.Servers.Instance;
        }

        private void FormModifyServerSettings_Load(object sender, System.EventArgs e)
        {
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
        }

        #region private methods

        VgcApis.Models.Datas.CoreServSettings GetterSettings()
        {
            var result = new VgcApis.Models.Datas.CoreServSettings();
            result.index = VgcApis.Misc.Utils.Str2Int(tboxServIndex.Text);
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
            tboxServIndex.Text = s.index.ToString();
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

        private void btnClearStat_Click(object sender, EventArgs e)
        {
            if (VgcApis.Misc.UI.Confirm(I18N.ConfirmClearStat))
            {
                var cst = coreServ.GetCoreStates();
                cst.SetDownlinkTotal(0);
                cst.SetUplinkTotal(0);
            }
        }
        #endregion
    }
}
