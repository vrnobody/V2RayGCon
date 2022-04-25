using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using V2RayGCon.Resources.Resx;

namespace V2RayGCon.Controllers.ConfigerComponet
{
    class MenuUpdater : ConfigerComponentController
    {
        Services.Servers servers;

        Views.WinForms.FormConfiger formConfiger;
        ToolStripMenuItem miRoot, miReplaceServer, miLoadServer;

        public MenuUpdater(
            Views.WinForms.FormConfiger formConfiger,
            ToolStripMenuItem miRoot,
            ToolStripMenuItem miReplaceServer,
            ToolStripMenuItem miLoadServer)
        {
            servers = Services.Servers.Instance;
            this.formConfiger = formConfiger;
            this.miReplaceServer = miReplaceServer;
            this.miLoadServer = miLoadServer;
            this.miRoot = miRoot;

            this.miRoot.DropDownOpening += ServerMenuOpeningHandler;
        }

        #region properties
        #endregion

        #region private method
        void ServerMenuOpeningHandler(object sender, EventArgs args)
        {
            VgcApis.Misc.Utils.RunInBackground(() =>
            {
                ReloadMenu();
            });
        }

        List<string[]> CollectServNameAndUid()
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

                var uid = coreServ.GetCoreStates().GetUid();
                servInfos.Add(new string[] { name, uid });
            }
            return servInfos;
        }

        void ReloadMenu()
        {
            // step 1 bg
            var servInfos = CollectServNameAndUid();

            // step 2 ui
            VgcApis.Misc.UI.Invoke(() =>
            {
                miReplaceServer.DropDownItems.Clear();
                miReplaceServer.DropDown.PerformLayout();
                miLoadServer.DropDownItems.Clear();
                miLoadServer.DropDown.PerformLayout();
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

            // step 5 ui
            VgcApis.Misc.UI.Invoke(() =>
            {
                if (miRoot?.IsDisposed != false)
                {
                    return;
                }

                List<ToolStripMenuItem> misLoadServ, misReplaceServ;
                GenGroupedMenuItemList(servInfos, out misLoadServ, out misReplaceServ);
                var miRootReplace = miReplaceServer.DropDownItems;
                var miRootLoad = miLoadServer.DropDownItems;
                miRootLoad.AddRange(misLoadServ.ToArray());
                miRootReplace.AddRange(misReplaceServ.ToArray());
                miLoadServer.Enabled = true;
                miReplaceServer.Enabled = true;
            });
        }

        private void GenGroupedMenuItemList(
            List<string[]> servInfos,
            out List<ToolStripMenuItem> groupedMiLoad,
            out List<ToolStripMenuItem> groupedMiReplace)
        {
            var groupSize = VgcApis.Models.Consts.Config.MenuItemGroupSize;
            var minDynMenuSize = VgcApis.Models.Consts.Config.MinDynamicMenuSize;

            if (servInfos.Count < minDynMenuSize)
            {
                GenStaticServerMenuItems(servInfos, groupSize, out groupedMiLoad, out groupedMiReplace);
                return;
            }

            var c = servInfos.Count;
            groupedMiLoad = CreateDynamicServerMenus(servInfos, GenMenuItemLoad, groupSize, 0, c);
            groupedMiReplace = CreateDynamicServerMenus(servInfos, GenMenuItemReplace, groupSize, 0, c);
        }

        private void GenStaticServerMenuItems(
            List<string[]> servInfos,
            int groupSize,
            out List<ToolStripMenuItem> groupedMiLoad,
            out List<ToolStripMenuItem> groupedMiReplace)
        {
            var loadServMiList = new List<ToolStripMenuItem>();
            var replaceServMiList = new List<ToolStripMenuItem>();

            foreach (var si in servInfos)
            {
                loadServMiList.Add(GenMenuItemLoad(si[0], si[1]));
                replaceServMiList.Add(GenMenuItemReplace(si[0], si[1]));
            }

            groupedMiLoad = VgcApis.Misc.UI.AutoGroupMenuItems(loadServMiList, groupSize);
            groupedMiReplace = VgcApis.Misc.UI.AutoGroupMenuItems(replaceServMiList, groupSize);
        }

        List<ToolStripMenuItem> CreateDynamicServerMenus(
            List<string[]> servInofs,
            Func<string, string, ToolStripMenuItem> handlerGenerator,
            int groupSize, int start, int end)
        {
            var n = end - start;
            var step = 1;
            while (n > groupSize)
            {
                n = n / groupSize;
                step = step * groupSize;
            }

            if (step == 1)
            {
                var mis = new List<ToolStripMenuItem>();

                for (int i = start; i < servInofs.Count && i < end; i++)
                {
                    var si = servInofs[i];
                    mis.Add(handlerGenerator.Invoke(si[0], si[1]));
                }
                return mis;
            }

            var gmis = new List<ToolStripMenuItem>();
            for (int i = start; i < end; i += step)
            {
                var s = i;
                var e = Math.Min(i + step, end);
                var text = string.Format("{0,5:D5} - {1,5:D5}", s + 1, e);
                var mi = new ToolStripMenuItem(text, null);
                mi.DropDownOpening += (o, a) =>
                {
                    var root = mi.DropDownItems;
                    if (root.Count < 1)
                    {
                        var dm = CreateDynamicServerMenus(
                            servInofs, handlerGenerator,
                            groupSize, s, e);
                        root.AddRange(dm.ToArray());
                    }
                };
                gmis.Add(mi);
            }

            return gmis;
        }

        private ToolStripMenuItem GenMenuItemLoad(string name, string orgUid)
        {
            var configer = container;
            var uid = orgUid;

            return new ToolStripMenuItem(name, null, (s, a) =>
            {
                if (!configer.IsConfigSaved()
                && !Misc.UI.Confirm(I18N.ConfirmLoadNewServer))
                {
                    return;
                }
                configer.LoadConfigByUid(uid);
                formConfiger.SetTitle(configer.GetAlias());
            });
        }

        private ToolStripMenuItem GenMenuItemReplace(string name, string orgUid)
        {
            var configer = container;
            var uid = orgUid;

            return new ToolStripMenuItem(name, null, (s, a) =>
            {
                if (Misc.UI.Confirm(I18N.ReplaceServer))
                {
                    if (configer.ReplaceServer(uid))
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
            this.miRoot.DropDownOpening -= ServerMenuOpeningHandler;
            this.formConfiger = null;
        }

        public override void Update(JObject config) { }
        #endregion
    }
}
