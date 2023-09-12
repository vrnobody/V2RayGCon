using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using V2RayGCon.Resources.Resx;
using V2RayGCon.Views.WinForms;

namespace V2RayGCon.Controllers.FormJsonConfigEditorComponet
{
    class DyMenuHelper : Comm.IDyMenuHelper
    {
        private readonly FormJsonConfigEditor formConfiger;
        private readonly FormJsonConfigEditorCtrl configer;

        public DyMenuHelper(FormJsonConfigEditor formConfiger, FormJsonConfigEditorCtrl controller)
        {
            this.formConfiger = formConfiger;
            this.configer = controller;
        }

        public void LoadConfigByUid(string uid)
        {
            if (!configer.IsConfigSaved() && !Misc.UI.Confirm(I18N.ConfirmLoadNewServer))
            {
                return;
            }
            configer.LoadConfigByUid(uid);
            formConfiger.SetTitle(configer.GetAlias());
        }

        public void ReplaceServer(string uid)
        {
            if (Misc.UI.Confirm(I18N.ReplaceServer))
            {
                if (configer.ReplaceServer(uid))
                {
                    formConfiger.SetTitle(configer.GetAlias());
                }
            }
        }
    }

    class MenuUpdater : ConfigerComponentController
    {
        DyMenuHelper helper;
        Comm.DyMenuGenerator dmg = null;

        public MenuUpdater() { }

        #region properties
        #endregion

        #region private method
        #endregion

        #region public method
        public void Init(
            FormJsonConfigEditor formConfiger,
            ToolStripMenuItem miRoot,
            ToolStripMenuItem miReplaceServer,
            ToolStripMenuItem miLoadServer
        )
        {
            helper = new DyMenuHelper(formConfiger, container);
            dmg = new Comm.DyMenuGenerator(helper, miRoot, miReplaceServer, miLoadServer);
        }

        public void Cleanup()
        {
            dmg?.Dispose();
        }

        public override void Update(JObject config) { }
        #endregion
    }
}
