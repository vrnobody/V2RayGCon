using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using V2RayGCon.Resource.Resx;

namespace V2RayGCon.Views.WinForms
{
    public partial class FormQRCode : Form
    {
        #region Sigleton
        static readonly VgcApis.Models.BaseClasses.AuxSiWinForm<FormQRCode> auxSiForm =
            new VgcApis.Models.BaseClasses.AuxSiWinForm<FormQRCode>();
        static public FormQRCode GetForm() => auxSiForm.GetForm();
        static public void ShowForm() => auxSiForm.ShowForm();
        #endregion

        Service.Servers servers;
        Service.ShareLinkMgr slinkMgr;

        int servIndex;
        VgcApis.Models.Datas.Enum.LinkTypes linkType;
        List<string> serverList;

        VgcApis.Libs.Tasks.LazyGuy servListUpdater;

        public FormQRCode()
        {
            servers = Service.Servers.Instance;
            slinkMgr = Service.ShareLinkMgr.Instance;

            servIndex = -1;
            linkType = VgcApis.Models.Datas.Enum.LinkTypes.vmess;

            InitializeComponent();

            VgcApis.Libs.UI.AutoSetFormIcon(this);
        }

        private void FormQRCode_Shown(object sender, EventArgs e)
        {
            ClearServerList();

            cboxLinkType.SelectedIndex =
                LinkTypeToComboBoxSelectedIndex(linkType);

            picQRCode.InitialImage = null;
            SetPicZoomMode();

            this.FormClosed += (s, a) =>
            {
                ReleaseServerEvents();
                servListUpdater?.ForgetIt();
                servListUpdater?.Quit();
            };

            servListUpdater = new VgcApis.Libs.Tasks.LazyGuy(
                () =>
                {
                    try
                    {
                        VgcApis.Libs.Utils.RunInBackground(RefreshServerList);
                    }
                    catch { }
                },
                VgcApis.Models.Consts.Intervals.FormQrcodeMenuUpdateDelay);

            BindServerEvents();

            servListUpdater.DoItLater();
        }

        #region private methods
        private void ReleaseServerEvents()
        {
            servers.OnServerCountChange -= OnSettingChangeHandler;
            servers.OnServerPropertyChange -= OnSettingChangeHandler;
        }

        private void BindServerEvents()
        {
            servers.OnServerCountChange += OnSettingChangeHandler;
            servers.OnServerPropertyChange += OnSettingChangeHandler;
        }

        VgcApis.Models.Datas.Enum.LinkTypes ComboBoxSelectedIndexToLinkType(
            int index)
        {
            switch (index)
            {
                case 0:
                    return VgcApis.Models.Datas.Enum.LinkTypes.vmess;
                case 1:
                    return VgcApis.Models.Datas.Enum.LinkTypes.v;

                default:
                    return VgcApis.Models.Datas.Enum.LinkTypes.unknow;
            }
        }

        int LinkTypeToComboBoxSelectedIndex(
            VgcApis.Models.Datas.Enum.LinkTypes linkType)
        {
            switch (linkType)
            {
                case VgcApis.Models.Datas.Enum.LinkTypes.vmess:
                    return 0;
                case VgcApis.Models.Datas.Enum.LinkTypes.v:
                    return 1;
                default:
                    return -1;
            }
        }

        void ClearServerList()
        {
            serverList = new List<string>();
        }

        void SetPicZoomMode()
        {
            picQRCode.SizeMode = rbtnIsCenterImage.Checked ?
                PictureBoxSizeMode.CenterImage :
                PictureBoxSizeMode.Zoom;
        }

        void OnSettingChangeHandler(object sender, EventArgs args) =>
            servListUpdater?.DoItLater();

        void RefreshServerList()
        {
            ClearServerList();
            var summaryList = new List<string>();
            var allServers = servers.GetAllServersOrderByIndex();
            foreach (var serv in allServers)
            {
                var summary = serv.GetCoreStates().GetTitle();
                var config = serv.GetConfiger().GetConfig();
                summaryList.Add(summary);
                this.serverList.Add(config);
            }

            VgcApis.Libs.UI.RunInUiThread(cboxServList,
                () =>
                {
                    cboxServList.Items.Clear();
                    cboxServList.Items.AddRange(summaryList.ToArray());
                    Lib.UI.ResetComboBoxDropdownMenuWidth(cboxServList);

                    if (summaryList.Count <= 0)
                    {
                        cboxServList.SelectedIndex = -1;
                        return;
                    }

                    var oldIndex = servIndex;
                    servIndex = -1;
                    cboxServList.SelectedIndex = VgcApis.Libs.Utils.Clamp(
                        oldIndex, 0, summaryList.Count);
                });
        }

        void UpdateTboxLink()
        {
            var config = string.Empty;

            if (servIndex >= 0
                && serverList != null
                && servIndex < serverList.Count)
            {
                config = serverList[servIndex];
            }

            if (string.IsNullOrEmpty(config))
            {
                tboxLink.Text = string.Empty;
                return;
            }

            string link = slinkMgr.EncodeConfigToShareLink(config, linkType);
            tboxLink.Text = link ?? string.Empty;
        }

        void SetQRCodeImage(Image img)
        {
            var oldImage = picQRCode.Image;

            picQRCode.Image = img;

            if (oldImage != img)
            {
                oldImage?.Dispose();
            }
        }

        void ShowQRCode()
        {
            var link = tboxLink.Text;

            if (string.IsNullOrEmpty(link))
            {
                SetQRCodeImage(null);
                return;
            }

            Tuple<Bitmap, Lib.QRCode.QRCode.WriteErrors> pair =
                Lib.QRCode.QRCode.GenQRCode(link, 320);

            switch (pair.Item2)
            {
                case Lib.QRCode.QRCode.WriteErrors.Success:
                    SetQRCodeImage(pair.Item1);
                    break;
                case Lib.QRCode.QRCode.WriteErrors.DataEmpty:
                    SetQRCodeImage(null);
                    MessageBox.Show(I18N.EmptyLink);
                    break;
                case Lib.QRCode.QRCode.WriteErrors.DataTooBig:
                    SetQRCodeImage(null);
                    MessageBox.Show(I18N.DataTooBig);
                    break;
            }
        }
        #endregion

        #region UI event handler
        private void cboxLinkType_SelectedIndexChanged(object sender, EventArgs e)
        {
            linkType = ComboBoxSelectedIndexToLinkType(cboxLinkType.SelectedIndex);
            UpdateTboxLink();
        }

        private void btnSavePic_Click(object sender, EventArgs e)
        {
            Stream myStream;
            SaveFileDialog saveFileDialog1 = new SaveFileDialog
            {
                Filter = StrConst.ExtPng,
                FilterIndex = 1,
                RestoreDirectory = true,
            };

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if ((myStream = saveFileDialog1.OpenFile()) != null)
                {
                    picQRCode.Image.Save(myStream, System.Drawing.Imaging.ImageFormat.Png);
                    myStream.Close();
                }
            }
        }

        private void tboxLink_TextChanged(object sender, EventArgs e)
        {
            ShowQRCode();
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            Lib.Utils.CopyToClipboardAndPrompt(tboxLink.Text);
        }

        private void rbtnIsCenterImage_CheckedChanged(object sender, EventArgs e)
        {
            SetPicZoomMode();
        }

        private void cboxServList_SelectedIndexChanged(object sender, EventArgs e)
        {
            var newIndex = cboxServList.SelectedIndex;
            if (servIndex == newIndex)
            {
                return;
            }
            servIndex = newIndex;
            UpdateTboxLink();
        }
        #endregion
    }
}
