using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using V2RayGCon.Resources.Resx;
using V2RayGCon.Services;

namespace V2RayGCon.Views.WinForms
{
    public partial class FormBatchModifyServerSetting : Form
    {
        #region Sigleton
        static FormBatchModifyServerSetting _instant;

        public static FormBatchModifyServerSetting GetForm()
        {
            VgcApis.Misc.UI.Invoke(() =>
            {
                if (_instant == null || _instant.IsDisposed)
                {
                    _instant = new FormBatchModifyServerSetting();
                    _instant.FormClosed += (s, a) => _instant = null;
                    _instant.Show();
                }
                else
                {
                    _instant.Activate();
                }
            });

            return _instant;
        }

        #endregion
        private readonly Settings settings;
        private readonly Servers servers;

        FormBatchModifyServerSetting()
        {
            settings = Settings.Instance;
            servers = Servers.Instance;

            InitializeComponent();

            VgcApis.Misc.UI.AutoSetFormIcon(this);
            FormClosed += (s, a) => formTemplateNames?.Close();
        }

        private void FormBatchModifyServerInfo_Shown(object sender, EventArgs e)
        {
            this.cboxMark.Items.Clear();
            cboxMark.Items.AddRange(servers.GetMarkList());
            InitCboxCustomCore();
            InitCboxCustomInboundName();

            var firstCtrl = servers.GetSelectedServers().OrderBy(s => s).FirstOrDefault();

            if (firstCtrl == null)
            {
                return;
            }

            var first = firstCtrl.GetCoreStates().GetAllRawCoreInfo();

            VgcApis.Misc.UI.SelectComboxByText(cboxInName, first.inbName);
            this.tboxInIP.Text = first.inbIp;
            this.tboxInPort.Text = first.inbPort.ToString();
            this.cboxMark.Text = first.customMark;
            this.tboxRemark.Text = first.customRemark;
            this.tboxTag1.Text = first.tag1;
            this.tboxTag2.Text = first.tag2;
            this.tboxTag3.Text = first.tag3;
            VgcApis.Misc.UI.SelectComboxByText(cboxCustomCoreName, first.customCoreName);
            this.cboxAutorun.SelectedIndex = first.isAutoRun ? 0 : 1;
            this.cboxUntrack.SelectedIndex = first.isUntrack ? 0 : 1;
            this.cboxInject.SelectedIndex = first.isAcceptInjection ? 0 : 1;
            this.cboxSendThrough.SelectedIndex = first.ignoreSendThrough ? 1 : 0;
            this.tboxTemplates.Text = first.templates;
        }

        #region UI event
        private void chkShareOverLAN_CheckedChanged(object sender, EventArgs e)
        {
            var isChecked = chkShareOverLAN.Checked;
            if (isChecked)
            {
                tboxInIP.Text = "0.0.0.0";
            }
            else
            {
                tboxInIP.Text = VgcApis.Models.Consts.Webs.LoopBackIP;
            }
            tboxInIP.Enabled = !isChecked;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        FormTemplateNameSelector formTemplateNames = null;

        private void btnTemplates_Click(object sender, EventArgs e)
        {
            if (formTemplateNames == null)
            {
                formTemplateNames = new FormTemplateNameSelector(tboxTemplates.Text);
                formTemplateNames.FormClosed += (s, a) =>
                {
                    if (formTemplateNames.DialogResult == DialogResult.OK)
                    {
                        tboxTemplates.Text = formTemplateNames.result;
                    }
                    formTemplateNames = null;
                };
                formTemplateNames.Show();
            }
            formTemplateNames.Activate();
        }

        private void btnModify_Click(object sender, EventArgs e)
        {
            btnModify.Enabled = false;
            btnCancel.Enabled = false;

            var coreServs = servers.GetSelectedServers().OrderBy(s => s).ToList();

            var bs = new BatchSettings()
            {
                mInbName = chkInMode.Checked,
                inboundName = cboxInName.Text,

                mIp = chkInIP.Checked,
                ip = tboxInIP.Text,

                mPort = chkInPort.Checked,
                port = VgcApis.Misc.Utils.Str2Int(tboxInPort.Text),
                isPortAutoIncrease = chkIncrement.Checked,

                mMark = chkMark.Checked,
                mark = cboxMark.Text,

                mRemark = chkRemark.Checked,
                remark = tboxRemark.Text,

                mTag1 = chkTag1.Checked,
                tag1 = tboxTag1.Text,

                mTag2 = chkTag2.Checked,
                tag2 = tboxTag2.Text,

                mTag3 = chkTag3.Checked,
                tag3 = tboxTag3.Text,

                mCoreName = chkCustomCore.Checked,
                coreName = cboxCustomCoreName.SelectedIndex < 1 ? "" : cboxCustomCoreName.Text,

                mTemplates = chkTemplates.Checked,
                templates = tboxTemplates.Text,

                mAutorun = chkAutorun.Checked,
                autorun = cboxAutorun.SelectedIndex == 0,

                mUntrack = chkUntrack.Checked,
                untrack = cboxUntrack.SelectedIndex == 0,

                mInject = chkInject.Checked,
                inject = cboxInject.SelectedIndex == 0,

                mSendThrough = chkSendThough.Checked,
                sendThrough = cboxSendThrough.SelectedIndex == 0,
            };

            ModifyServersSetting(coreServs, bs);
        }

        #endregion

        #region private method


        void InitCboxCustomInboundName()
        {
            var items = cboxInName.Items;
            items.Clear();
            var names = settings
                .GetCustomConfigTemplates()
                .Where(inb => !inb.isInject)
                .Select(inb => inb.name)
                .ToArray();
            items.AddRange(names);
            VgcApis.Misc.UI.ResetComboBoxDropdownMenuWidth(cboxCustomCoreName);
        }

        void InitCboxCustomCore()
        {
            var items = cboxCustomCoreName.Items;
            items.Clear();
            items.Add(I18N.Default);
            var names = settings.GetCustomCoresSetting().Select(cs => cs.name).ToArray();
            items.AddRange(names);
            VgcApis.Misc.UI.ResetComboBoxDropdownMenuWidth(cboxCustomCoreName);
        }

        void ModifyServersSetting(
            List<VgcApis.Interfaces.ICoreServCtrl> coreServs,
            BatchSettings bs
        )
        {
            void worker(int index, Action next)
            {
                var server = coreServs[index];
                if (!server.GetCoreCtrl().IsCoreRunning())
                {
                    ModifyServerSetting(ref server, index, bs);
                    server.InvokeEventOnPropertyChange();
                    next();
                    return;
                }

                server
                    .GetCoreCtrl()
                    .StopCoreThen(() =>
                    {
                        ModifyServerSetting(ref server, index, bs);
                        server.GetCoreCtrl().RestartCoreThen();
                        next();
                    });
            }

            var that = this;
            void done()
            {
                servers.UpdateMarkList();
                VgcApis.Misc.UI.Invoke(() => that.Close());
            }

            VgcApis.Misc.Utils.InvokeChainActionsAsync(coreServs.Count, worker, done);
        }

        void ModifyServerSetting(
            ref VgcApis.Interfaces.ICoreServCtrl serverCtrl,
            int index,
            BatchSettings bs
        )
        {
            var server = serverCtrl.GetCoreStates().GetAllRawCoreInfo();

            if (bs.mTemplates)
            {
                server.templates = bs.templates;
            }

            if (bs.mSendThrough)
            {
                server.ignoreSendThrough = !bs.sendThrough;
            }

            if (bs.mInject)
            {
                server.isAcceptInjection = bs.inject;
            }

            if (bs.mAutorun)
            {
                server.isAutoRun = bs.autorun;
            }

            if (bs.mUntrack)
            {
                server.isUntrack = bs.untrack;
            }

            if (bs.mInbName)
            {
                server.inbName = bs.inboundName;
            }

            if (bs.mIp)
            {
                server.inbIp = bs.ip;
            }

            if (bs.mPort)
            {
                server.inbPort = bs.isPortAutoIncrease ? bs.port + index : bs.port;
            }

            if (bs.mMark)
            {
                server.customMark = bs.mark;
            }

            if (bs.mRemark)
            {
                server.customRemark = bs.remark;
            }

            if (bs.mTag1)
            {
                server.tag1 = bs.tag1;
            }
            if (bs.mTag2)
            {
                server.tag2 = bs.tag2;
            }
            if (bs.mTag3)
            {
                server.tag3 = bs.tag3;
            }

            // 需要放最后，因为会InvokeEventOnPropertyChange()
            if (bs.mCoreName)
            {
                serverCtrl.GetCoreCtrl().SetCustomCoreName(bs.coreName);
            }

            serverCtrl.GetConfiger().UpdateSummary();
        }

        #endregion

        #region batch settins

        class BatchSettings
        {
            public bool mInbName;
            public string inboundName;

            public bool mIp;
            public string ip;

            public bool mPort;
            public int port;
            public bool isPortAutoIncrease;

            public bool mMark;
            public string mark;

            public bool mRemark;
            public string remark;

            public bool mTag1;
            public string tag1;

            public bool mTag2;
            public string tag2;

            public bool mTag3;
            public string tag3;

            public bool mCoreName;
            public string coreName;

            public bool mTemplates;
            public string templates;

            public bool mAutorun;
            public bool autorun;

            public bool mUntrack;
            public bool untrack;

            public bool mInject;
            public bool inject;

            public bool mSendThrough;
            public bool sendThrough;
        }

        #endregion
    }
}
