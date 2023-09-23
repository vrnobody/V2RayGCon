﻿using System;
using System.Windows.Forms;
using V2RayGCon.Models.Datas;

namespace V2RayGCon.Views.WinForms
{
    public partial class FormCustomInboundSettings : Form
    {
        public CustomInboundSettings inbS;

        public FormCustomInboundSettings()
            : this(new CustomInboundSettings()) { }

        public FormCustomInboundSettings(CustomInboundSettings inboundSettings)
        {
            InitializeComponent();

            VgcApis.Misc.UI.AutoSetFormIcon(this);

            this.inbS = inboundSettings;
            this.DialogResult = DialogResult.Cancel;
        }

        private void FormCustomCoreSettings_Load(object sender, EventArgs e)
        {
            InitControls();
        }

        #region private method
        CustomInboundSettings GatherCoreSettings()
        {
            var cs = new CustomInboundSettings
            {
                name = tboxName.Text,
                template = rtboxTemplate.Text,
            };
            return cs;
        }

        void InitControls()
        {
            tboxName.Text = inbS.name;
            rtboxTemplate.Text = inbS.template;
        }

        #endregion

        #region public methods
        #endregion

        #region UI event handler
        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            this.inbS = GatherCoreSettings();
            this.DialogResult = DialogResult.OK;
            Close();
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            var inbS = GatherCoreSettings();
            var tpl = inbS.GetFormatedTemplate("127.0.0.1", 1080);
            VgcApis.Misc.UI.MsgBoxAsync(tpl);
        }
        #endregion
    }
}