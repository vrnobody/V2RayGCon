using System.Windows.Forms;
using V2RayGCon.Resources.Resx;
using V2RayGCon.Views.WinForms;

namespace V2RayGCon.Controllers
{
    internal class FormTextConfigEditorCtrl : BaseClasses.FormController
    {
        FormTextConfigEditorComponent.Editor editor;
        readonly Services.Servers servers;
        string prevConfig = string.Empty;
        string uid = string.Empty;

        public FormTextConfigEditorCtrl(FormTextConfigEditor parent)
        {
            servers = Services.Servers.Instance;
            this.parent = parent;
        }

        #region properties

        private readonly FormTextConfigEditor parent;

        #endregion

        #region public methods
        public void AddNewServer()
        {
            var config = editor.content;
            VgcApis.Misc.UI.GetUserInput(
                I18N.NewServerName,
                (name) =>
                {
                    var uid = servers.AddServer(name, config, "", false);
                    if (string.IsNullOrEmpty(uid))
                    {
                        VgcApis.Misc.UI.MsgBox(I18N.DuplicateServer);
                    }
                    uid = servers.GetServerByConfig(config)?.GetCoreStates()?.GetUid();
                    LoadConfigByUid(uid);
                }
            );
        }

        public string SaveToFile()
        {
            var cfg = editor.content;

            var r = VgcApis.Misc.UI.ShowSaveFileDialog(
                VgcApis.Models.Consts.Files.AllExt,
                cfg,
                out string filename
            );

            switch (r)
            {
                case VgcApis.Models.Datas.Enums.SaveFileErrorCode.Success:
                    this.prevConfig = cfg;
                    this.uid = string.Empty;
                    VgcApis.Misc.UI.MsgBox(I18N.Done);
                    return filename;
                case VgcApis.Models.Datas.Enums.SaveFileErrorCode.Fail:
                    VgcApis.Misc.UI.MsgBox(I18N.WriteFileFail);
                    break;
                case VgcApis.Models.Datas.Enums.SaveFileErrorCode.Cancel:
                    // do nothing
                    break;
            }
            return null;
        }

        public void Init()
        {
            editor = GetComponent<FormTextConfigEditorComponent.Editor>();
        }

        public FormTextConfigEditorComponent.Editor GetEditor() => editor;

        public void Cleanup()
        {
            GetComponent<FormTextConfigEditorComponent.MenuUpdater>()?.Dispose();
            editor?.Dispose();
        }

        public void OverwriteCurServer()
        {
            ReplaceServer(this.uid);
        }

        public void ReplaceServer(string uid)
        {
            var coreServ = servers.GetServerByUid(uid);
            if (coreServ == null)
            {
                VgcApis.Misc.UI.MsgBox(I18N.OrgServNotFound);
                return;
            }

            var oldConfig = coreServ.GetConfiger().GetConfig();
            var newConfig = editor.content;

            if (oldConfig == newConfig || servers.IsServerExist(newConfig))
            {
                VgcApis.Misc.UI.MsgBox(I18N.DuplicateServer);
                return;
            }

            if (!servers.ReplaceServerConfig(oldConfig, newConfig))
            {
                VgcApis.Misc.UI.MsgBox(I18N.OrgServNotFound);
                return;
            }

            LoadConfigByUid(uid);
        }

        public void LoadConfigByUid(string uid)
        {
            var coreServ = servers.GetServerByUid(uid);
            if (coreServ == null)
            {
                return;
            }
            var config = coreServ.GetConfiger().GetConfig();
            var title = coreServ.GetCoreStates().GetTitle();
            parent.SetTitle(title);
            LoadConfig(config);
            this.uid = uid;
        }

        public void LoadConfig(string config)
        {
            prevConfig = config;
            editor.content = config;
            this.uid = string.Empty;
        }

        public bool IsConfigChanged()
        {
            var config = editor.content;
            return prevConfig != config;
        }

        #endregion

        #region private methods

        #endregion

        #region protected methods

        #endregion
    }
}
