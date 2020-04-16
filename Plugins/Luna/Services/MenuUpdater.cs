using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;

namespace Luna.Services
{
    public class MenuUpdater :
        VgcApis.BaseClasses.Disposable
    {
        LuaServer luaServer;
        ToolStripMenuItem miRoot, miShowWindow;
        VgcApis.Interfaces.Services.INotifierService vgcNotifierService;
        VgcApis.Libs.Tasks.LazyGuy lazyMenuUpdater;
        AutoResetEvent menuUpdaterLock = new AutoResetEvent(true);

        public MenuUpdater(VgcApis.Interfaces.Services.INotifierService vgcNotifierService)
        {
            this.vgcNotifierService = vgcNotifierService;
        }

        public void Run(
            LuaServer luaServer,
            ToolStripMenuItem miRoot,
            ToolStripMenuItem miShowWindow)
        {
            this.luaServer = luaServer;

            this.miRoot = miRoot;
            this.miShowWindow = miShowWindow;

            lazyMenuUpdater = new VgcApis.Libs.Tasks.LazyGuy(UpdateMenuLater, 1000);

            BindEvents();

            UpdateMenuLater();
        }

        #region public methods

        #endregion

        #region private methods

        void UpdateMenuLater()
        {
            if (!menuUpdaterLock.WaitOne(0))
            {
                lazyMenuUpdater?.DoItLater();
                return;
            }

            Action next = () => menuUpdaterLock.Set();
            UpdateMenuNow(next);
        }

        void UpdateMenuNow(Action next)
        {
            vgcNotifierService.RunInUiThreadIgnoreErrorThen(() =>
            {
                var mis = GenSubMenuItems();
                var root = miRoot.DropDownItems;
                root.Clear();
                root.Add(miShowWindow);
                if (mis.Count > 0)
                {
                    root.Add(new ToolStripSeparator());
                    root.AddRange(mis.ToArray());
                }
            }, next);
        }

        List<ToolStripMenuItem> GenSubMenuItems()
        {
            var mis = new List<ToolStripMenuItem>();
            var luaCtrls = luaServer.GetVisibleCoreCtrls();
            foreach (var luaCtrl in luaCtrls)
            {
                var ctrl = luaCtrl; // capture
                var mi = new ToolStripMenuItem(ctrl.name, null, (s, a) => ctrl.Start());
                mis.Add(mi);
            }

            int gs = Constants.Numbers.MenuItemGroupSize;
            return mis.Count <= gs ? mis : VgcApis.Misc.UI.AutoGroupMenuItems(mis, gs);
        }

        void LuaCoreCtrlListChangeHandler(object sender, EventArgs args) =>
            UpdateMenuLater();

        void BindEvents()
        {
            luaServer.OnLuaCoreCtrlListChanged += LuaCoreCtrlListChangeHandler;
            luaServer.OnLuaCoreCtrlHiddenStateChanged += LuaCoreCtrlListChangeHandler;
        }

        void ReleaseEvents()
        {
            luaServer.OnLuaCoreCtrlListChanged -= LuaCoreCtrlListChangeHandler;
            luaServer.OnLuaCoreCtrlHiddenStateChanged -= LuaCoreCtrlListChangeHandler;
        }
        #endregion

        #region protected methods
        protected override void Cleanup()
        {
            ReleaseEvents();
        }
        #endregion


    }
}
