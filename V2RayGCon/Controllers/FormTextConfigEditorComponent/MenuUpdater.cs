using System.Windows.Forms;
using V2RayGCon.Resources.Resx;

namespace V2RayGCon.Controllers.FormTextConfigEditorComponent
{
    class DyMenuHelper : Comm.IDyMenuHelper
    {
        private readonly FormTextConfigEditorCtrl formCtrl;

        public DyMenuHelper(FormTextConfigEditorCtrl controller)
        {
            this.formCtrl = controller;
        }

        public void LoadConfigByUid(string uid)
        {
            if (formCtrl.IsConfigChanged() && !VgcApis.Misc.UI.Confirm(I18N.ConfirmLoadNewServer))
            {
                return;
            }
            formCtrl.LoadConfigByUid(uid);
        }

        public void ReplaceServer(string uid)
        {
            if (VgcApis.Misc.UI.Confirm(I18N.ReplaceServer))
            {
                formCtrl.ReplaceServer(uid);
            }
        }
    }

    class MenuUpdater : CompBase
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
            ToolStripMenuItem miRoot,
            ToolStripMenuItem miReplaceServer,
            ToolStripMenuItem miLoadServer
        )
        {
            helper = new DyMenuHelper(container);
            dmg = new Comm.DyMenuGenerator(helper, miRoot, miReplaceServer, miLoadServer);
        }
        #endregion

        #region protected method

        protected override void Cleanup()
        {
            dmg?.Dispose();
        }

        #endregion
    }
}
