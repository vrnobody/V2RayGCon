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

        public MenuUpdater(
            Views.WinForms.FormConfiger formConfiger,
            ToolStripMenuItem miReplaceServer,
            ToolStripMenuItem miLoadServer)
        {
            servers = Services.Servers.Instance;
            this.formConfiger = formConfiger;
            this.miReplaceServer = miReplaceServer;
            this.miLoadServer = miLoadServer;

            VgcApis.Misc.Utils.RunInBackground(() =>
            {
                VgcApis.Misc.Utils.Sleep(2000);
                ServerMenuItemsUpdateWorker();
            });

        }

        #region properties
        #endregion

        #region private method
        List<string[]> CollectServNameAndConfig()
        {
            var serverList = servers.GetAllServersOrderByIndex();
            var servInfos = new List<string[]>();
            for (int i = 0; i < serverList.Count; i++)
            {
                var coreServ = serverList[i];
                var coreState = coreServ.GetCoreStates();

                var name = string.Format(
                    "{0}.{1}",
                    coreState.GetIndex(),
                    coreState.GetLongName());

                var org = coreServ.GetConfiger().GetConfig();
                servInfos.Add(new string[] { name, org });
            }
            return servInfos;
        }

        void ServerMenuItemsUpdateWorker()
        {
            // step 1 bg
            var servInfos = CollectServNameAndConfig();

            // step 2 ui
            VgcApis.Misc.UI.Invoke(() =>
            {
                var miRootReplace = miReplaceServer.DropDownItems;
                var miRootLoad = miLoadServer.DropDownItems;
                miRootReplace.Clear();
                miRootLoad.Clear();
            });

            if (servInfos.Count <= 0)
            {
                VgcApis.Misc.UI.Invoke(() =>
                {
                    miReplaceServer.Enabled = false;
                    miLoadServer.Enabled = false;
                });
                return;
            }

            // step 3 ui
            var loadServMiList = new List<ToolStripMenuItem>();
            var replaceServMiList = new List<ToolStripMenuItem>();
            VgcApis.Misc.UI.Invoke(() =>
            {
                foreach (var si in servInfos)
                {
                    loadServMiList.Add(GenMenuItemLoad(si[0], si[1]));
                    replaceServMiList.Add(GenMenuItemReplace(si[0], si[1]));
                }
            });

            // step 4 bg
            var groupSize = VgcApis.Models.Consts.Config.MenuItemGroupSize;
            var groupedMiLoad = VgcApis.Misc.UI.AutoGroupMenuItems(loadServMiList, groupSize);
            var groupedMiReplace = VgcApis.Misc.UI.AutoGroupMenuItems(replaceServMiList, groupSize);

            // step 5 ui
            VgcApis.Misc.UI.Invoke(() =>
            {
                var miRootReplace = miReplaceServer.DropDownItems;
                var miRootLoad = miLoadServer.DropDownItems;
                miRootLoad.AddRange(groupedMiLoad.ToArray());
                miRootReplace.AddRange(groupedMiReplace.ToArray());
                miLoadServer.Enabled = true;
                miReplaceServer.Enabled = true;
            });
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

        }

        public override void Update(JObject config) { }
        #endregion
    }
}
