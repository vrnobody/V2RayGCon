using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using V2RayGCon.Resource.Resx;

namespace V2RayGCon.Controller.ConfigerComponet
{
    class MenuUpdater : ConfigerComponentController
    {
        Service.Servers servers;

        Views.WinForms.FormConfiger formConfiger;
        ToolStripMenuItem miReplaceServer, miLoadServer;

        VgcApis.Libs.Tasks.LazyGuy menuUpdater;

        public MenuUpdater(
            Views.WinForms.FormConfiger formConfiger,
            ToolStripMenuItem miReplaceServer,
            ToolStripMenuItem miLoadServer)
        {
            servers = Service.Servers.Instance;

            this.formConfiger = formConfiger;
            this.miReplaceServer = miReplaceServer;
            this.miLoadServer = miLoadServer;

            menuUpdater = new VgcApis.Libs.Tasks.LazyGuy(
               () =>
               {
                   try
                   {
                       VgcApis.Libs.Utils.RunInBackground(
                           UpdateServerMenus);
                   }
                   catch
                   {
                       // Do not hurt me.
                   }
               },
               VgcApis.Models.Consts.Intervals.FormConfigerMenuUpdateDelay);
        }

        #region properties
        #endregion

        #region private method
        void UpdateServerMenus()
        {
            var serverList = servers.GetAllServersOrderByIndex();

            var loadServMiList = new List<ToolStripMenuItem>();
            var replaceServMiList = new List<ToolStripMenuItem>();

            for (int i = 0; i < serverList.Count; i++)
            {
                var coreServ = serverList[i];
                var coreState = coreServ.GetCoreStates();

                var name = string.Format(
                    "{0}.{1}",
                    coreState.GetIndex(),
                    coreState.GetName());


                var org = coreServ.GetConfiger().GetConfig();
                loadServMiList.Add(GenMenuItemLoad(name, org));
                replaceServMiList.Add(GenMenuItemReplace(name, org));
            }

            VgcApis.Libs.UI.RunInUiThread(
                formConfiger,
                () =>
                {
                    try
                    {
                        ReplaceOldMenus(loadServMiList, replaceServMiList);
                    }
                    catch
                    {
                        // Do not hurt me.
                    }
                });
        }

        private void ReplaceOldMenus(
            List<ToolStripMenuItem> loadServMiList,
            List<ToolStripMenuItem> replaceServMiList)
        {
            var miRootReplace = miReplaceServer.DropDownItems;
            var miRootLoad = miLoadServer.DropDownItems;

            miRootReplace.Clear();
            miRootLoad.Clear();

            if (loadServMiList.Count <= 0)
            {
                miReplaceServer.Enabled = false;
                miLoadServer.Enabled = false;
                return;
            }

            miLoadServer.Enabled = true;
            miReplaceServer.Enabled = true;

            var miPerSubmenu = VgcApis.Models.Consts.Config.ConfigEditorServerMenuGroupSize;
            var isUseSubMenu = loadServMiList.Count > miPerSubmenu;
            if (!isUseSubMenu)
            {
                miRootLoad.AddRange(loadServMiList.ToArray());
                miRootReplace.AddRange(replaceServMiList.ToArray());
                return;
            }

            int submenuCount = (int)(Math.Ceiling(1.0 * loadServMiList.Count / miPerSubmenu));
            for (int i = 0; i < submenuCount; i++)
            {
                var skip = i * miPerSubmenu;
                var submenuTitle = string.Format(
                    "{0,3} - {1,3}", skip + 1, skip + miPerSubmenu);

                var submenuLoad = new ToolStripMenuItem(submenuTitle);
                var submenuReplace = new ToolStripMenuItem(submenuTitle);
                var partialMisLoad = loadServMiList.Skip(skip).Take(miPerSubmenu).ToArray();
                var partialMisReplace = replaceServMiList.Skip(skip).Take(miPerSubmenu).ToArray();
                submenuLoad.DropDownItems.AddRange(partialMisLoad);
                submenuReplace.DropDownItems.AddRange(partialMisReplace);
                miRootLoad.Add(submenuLoad);
                miRootReplace.Add(submenuReplace);
            }
        }

        private ToolStripMenuItem GenMenuItemLoad(string name, string orgConfig)
        {
            var configer = container;
            var config = orgConfig;

            return new ToolStripMenuItem(name, null, (s, a) =>
            {
                if (!configer.IsConfigSaved()
                && !Lib.UI.Confirm(I18N.ConfirmLoadNewServer))
                {
                    return;
                }
                configer.LoadServer(config);
                formConfiger.SetTitle(configer.GetAlias());
            });
        }

        private ToolStripMenuItem GenMenuItemReplace(string name, string orgConfig)
        {
            var configer = container;
            var config = orgConfig;

            return new ToolStripMenuItem(name, null, (s, a) =>
            {
                if (Lib.UI.Confirm(I18N.ReplaceServer))
                {
                    if (configer.ReplaceServer(config))
                    {
                        formConfiger.SetTitle(configer.GetAlias());
                    }
                }
            });
        }
        #endregion

        #region public method
        public void Cleanup()
        {
            menuUpdater?.ForgetIt();
            menuUpdater?.Quit();
        }

        public void UpdateMenusLater() =>
            menuUpdater?.DoItLater();

        public override void Update(JObject config) { }
        #endregion
    }
}
