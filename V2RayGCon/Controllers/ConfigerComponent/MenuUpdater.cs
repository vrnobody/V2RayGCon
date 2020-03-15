using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Windows.Forms;
using V2RayGCon.Resources.Resx;

namespace V2RayGCon.Controllers.ConfigerComponet
{
    class MenuUpdater : ConfigerComponentController
    {
        Services.Servers servers;

        Views.WinForms.FormConfiger formConfiger;
        ToolStripMenuItem miReplaceServer, miLoadServer;

        VgcApis.Libs.Tasks.LazyGuy menuUpdater;

        public MenuUpdater(
            Views.WinForms.FormConfiger formConfiger,
            ToolStripMenuItem miReplaceServer,
            ToolStripMenuItem miLoadServer)
        {
            servers = Services.Servers.Instance;

            this.formConfiger = formConfiger;
            this.miReplaceServer = miReplaceServer;
            this.miLoadServer = miLoadServer;

            menuUpdater = new VgcApis.Libs.Tasks.LazyGuy(
               () =>
               {
                   try
                   {
                       VgcApis.Misc.Utils.RunInBackground(
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
                    coreState.GetLongName());


                var org = coreServ.GetConfiger().GetConfig();
                loadServMiList.Add(GenMenuItemLoad(name, org));
                replaceServMiList.Add(GenMenuItemReplace(name, org));
            }

            VgcApis.Misc.UI.RunInUiThread(
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

            var groupSize = VgcApis.Models.Consts.Config.MenuItemGroupSize;
            var groupedMiLoad = VgcApis.Misc.UI.AutoGroupMenuItems(
                loadServMiList, groupSize);
            var groupedMiReplace = VgcApis.Misc.UI.AutoGroupMenuItems(
                replaceServMiList, groupSize);

            miRootLoad.AddRange(groupedMiLoad.ToArray());
            miRootReplace.AddRange(groupedMiReplace.ToArray());

            miLoadServer.Enabled = true;
            miReplaceServer.Enabled = true;
        }

        private ToolStripMenuItem GenMenuItemLoad(string name, string orgConfig)
        {
            var configer = container;
            var config = orgConfig;

            return new ToolStripMenuItem(name, null, (s, a) =>
            {
                if (!configer.IsConfigSaved()
                && !Misc.UI.Confirm(I18N.ConfirmLoadNewServer))
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
                if (Misc.UI.Confirm(I18N.ReplaceServer))
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
