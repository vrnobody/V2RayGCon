﻿using System;
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
            settings = Services.Settings.Instance;
            servers = Services.Servers.Instance;

            InitializeComponent();

            VgcApis.Misc.UI.AutoSetFormIcon(this);
        }

        private void FormBatchModifyServerInfo_Shown(object sender, EventArgs e)
        {
            this.cboxMark.Items.Clear();
            cboxMark.Items.AddRange(servers.GetMarkList());
            InitCboxCustomCore();

            var firstCtrl = servers.GetSelectedServers().OrderBy(s => s).FirstOrDefault();

            if (firstCtrl == null)
            {
                return;
            }

            var first = firstCtrl.GetCoreStates().GetAllRawCoreInfo();

            this.cboxInMode.SelectedIndex = first.customInbType;
            this.tboxInIP.Text = first.inbIp;
            this.tboxInPort.Text = first.inbPort.ToString();
            this.cboxMark.Text = first.customMark;
            this.tboxRemark.Text = first.customRemark;
            SelectCoreName(first.customCoreName);
            this.cboxAutorun.SelectedIndex = first.isAutoRun ? 0 : 1;
            this.cboxImport.SelectedIndex = first.isInjectImport ? 0 : 1;
            this.cboxIsInjectSkipCNSite.SelectedIndex = first.isInjectSkipCNSite ? 0 : 1;
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

        string GetCustomCoreName()
        {
            if (chkCustomCore.Checked)
            {
                if (cboxCustomCoreName.SelectedIndex < 1)
                {
                    return string.Empty;
                }
                return cboxCustomCoreName.Text;
            }
            return null;
        }

        private void btnModify_Click(object sender, EventArgs e)
        {
            var list = servers.GetSelectedServers();

            var newMode = chkInMode.Checked ? cboxInMode.SelectedIndex : -1;
            var newIP = chkInIP.Checked ? tboxInIP.Text : null;
            var newPort = chkInPort.Checked ? VgcApis.Misc.Utils.Str2Int(tboxInPort.Text) : -1;

            var newMark = chkMark.Checked ? cboxMark.Text : null;
            var newRemark = chkRemark.Checked ? tboxRemark.Text : null;
            var newCoreName = GetCustomCoreName();

            var newAutorun = chkAutorun.Checked ? cboxAutorun.SelectedIndex : -1;
            var newImport = chkImport.Checked ? cboxImport.SelectedIndex : -1;
            var newSkipCN = chkIsInjectSkipCNSite.Checked
                ? cboxIsInjectSkipCNSite.SelectedIndex
                : -1;
            var isPortAutoIncrease = chkIncrement.Checked;

            ModifyServersSetting(
                list,
                newMode,
                newIP,
                newPort,
                isPortAutoIncrease,
                newMark,
                newRemark,
                newCoreName,
                newAutorun,
                newImport,
                newSkipCN
            );
        }

        #endregion

        #region private method
        void SelectCoreName(string def)
        {
            if (string.IsNullOrEmpty(def))
            {
                cboxCustomCoreName.SelectedIndex = 0;
                return;
            }

            var items = cboxCustomCoreName.Items;
            for (int i = 1; i < items.Count; i++)
            {
                if (items[i].ToString() == def)
                {
                    cboxCustomCoreName.SelectedIndex = i;
                    return;
                }
            }
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
            List<VgcApis.Interfaces.ICoreServCtrl> list,
            int newMode,
            string newIP,
            int newPort,
            bool isPortAutoIncrease,
            string newMark,
            string newRemark,
            string newCoreName,
            int newAutorun,
            int newImport,
            int newSkipCN
        )
        {
            Action<int, Action> worker = (index, next) =>
            {
                var portNumber = isPortAutoIncrease ? newPort + index : newPort;

                var server = list[index];
                if (!server.GetCoreCtrl().IsCoreRunning())
                {
                    ModifyServerSetting(
                        ref server,
                        newMode,
                        newIP,
                        portNumber,
                        newMark,
                        newRemark,
                        newCoreName,
                        newAutorun,
                        newImport,
                        newSkipCN
                    );
                    server.InvokeEventOnPropertyChange();
                    next();
                    return;
                }

                server
                    .GetCoreCtrl()
                    .StopCoreThen(() =>
                    {
                        ModifyServerSetting(
                            ref server,
                            newMode,
                            newIP,
                            portNumber,
                            newMark,
                            newRemark,
                            newCoreName,
                            newAutorun,
                            newImport,
                            newSkipCN
                        );
                        server.GetCoreCtrl().RestartCoreThen();
                        next();
                    });
            };

            var that = this;
            Action done = () =>
            {
                servers.UpdateMarkList();
                VgcApis.Misc.UI.Invoke(() => that.Close());
            };

            Misc.Utils.ChainActionHelperAsync(list.Count, worker, done);
        }

        void ModifyServerSetting(
            ref VgcApis.Interfaces.ICoreServCtrl serverCtrl,
            int newMode,
            string newIP,
            int newPort,
            string newMark,
            string newRemark,
            string newCoreName,
            int newAutorun,
            int newImport,
            int newSkipCN
        )
        {
            var server = serverCtrl.GetCoreStates().GetAllRawCoreInfo();

            if (newCoreName != null)
            {
                server.customCoreName = newCoreName;
            }

            if (newSkipCN >= 0)
            {
                server.isInjectSkipCNSite = newSkipCN == 0;
            }

            if (newAutorun >= 0)
            {
                server.isAutoRun = newAutorun == 0;
            }

            if (newImport >= 0)
            {
                server.isInjectImport = newImport == 0;
            }

            if (newMode >= 0)
            {
                server.customInbType = newMode;
            }

            if (newIP != null)
            {
                server.inbIp = newIP;
            }

            if (newPort >= 0)
            {
                server.inbPort = newPort;
            }

            if (newMark != null)
            {
                server.customMark = newMark;
            }

            if (!string.IsNullOrEmpty(newRemark))
            {
                server.customRemark = newRemark;
            }
        }

        #endregion
    }
}
