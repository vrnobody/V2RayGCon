using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Luna.Services
{
    internal class MenuUpdater : VgcApis.BaseClasses.Disposable
    {
        LuaServer luaServer;
        ToolStripMenuItem miRoot,
            miShowMgr,
            miShowEditor;
        VgcApis.Libs.Tasks.LazyGuy lazyMenuUpdater;

        public MenuUpdater() { }

        public void Run(
            LuaServer luaServer,
            ToolStripMenuItem miRoot,
            ToolStripMenuItem miShowMgr,
            ToolStripMenuItem miShowEditor
        )
        {
            this.luaServer = luaServer;

            this.miRoot = miRoot;
            this.miShowMgr = miShowMgr;
            this.miShowEditor = miShowEditor;

            lazyMenuUpdater = new VgcApis.Libs.Tasks.LazyGuy(UpdateMenuWorker, 500, 3000)
            {
                Name = "Luna.MenuUpdater",
            };

            BindEvents();

            UpdateMenuLater();
        }

        #region public methods

        #endregion

        #region private methods
        void UpdateMenuWorker(Action done)
        {
            VgcApis.Misc.UI.InvokeThen(
                () =>
                {
                    var mis = GenSubMenuItems();
                    var root = miRoot.DropDownItems;
                    root.Clear();
                    miRoot.DropDown.PerformLayout();
                    root.Add(miShowMgr);
                    root.Add(miShowEditor);
                    if (mis.Count > 0)
                    {
                        root.Add(new ToolStripSeparator());
                        root.AddRange(mis.ToArray());
                    }
                },
                done
            );
        }

        void UpdateMenuLater() => lazyMenuUpdater?.Postpone();

        List<ToolStripMenuItem> GenSubMenuItems()
        {
            var mis = new List<ToolStripMenuItem>();
            var luaCtrls = luaServer.GetVisibleCoreCtrls();
            foreach (var luaCtrl in luaCtrls)
            {
                var ctrl = luaCtrl; // capture
                void onClick()
                {
                    if (ctrl.isRunning)
                    {
                        ctrl.Stop();
                    }
                    else
                    {
                        ctrl.Start();
                    }
                }

                var mi = new ToolStripMenuItem(ctrl.name, null, (s, a) => onClick())
                {
                    Checked = luaCtrl.isRunning
                };
                mis.Add(mi);
            }

            int gs = Constants.Numbers.MenuItemGroupSize;
            return mis.Count <= gs ? mis : VgcApis.Misc.UI.AutoGroupMenuItems(mis, gs);
        }

        void LuaCoreCtrlListChangeHandler(object sender, EventArgs args) => UpdateMenuLater();

        void BindEvents()
        {
            luaServer.OnRequireMenuUpdate += LuaCoreCtrlListChangeHandler;
        }

        void ReleaseEvents()
        {
            luaServer.OnRequireMenuUpdate -= LuaCoreCtrlListChangeHandler;
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
