using Luna.Resources.Langs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Luna.Controllers
{
    public class TabGeneralCtrl
    {
        Button btnStopAll, btnKillAll, btnDelAll, btnImport, btnExport;
        FlowLayoutPanel flyLuaUiPanel;

        public TabGeneralCtrl(
            FlowLayoutPanel flyLuaUiPanel,
            Button btnStopAll,
            Button btnKillAll,
            Button btnDelAll,
            Button btnImport,
            Button btnExport)
        {
            BindControls(flyLuaUiPanel, btnStopAll, btnKillAll, btnDelAll, btnImport, btnExport);
        }

        #region public methods
        Services.LuaServer luaServer;
        Services.Settings settings;
        public void Run(
            Services.Settings settings,
            Services.LuaServer luaServer)
        {
            this.settings = settings;
            this.luaServer = luaServer;
            BindEvents(luaServer);
            RefreshFlyPanel();
            luaServer.OnLuaCoreCtrlListChange += OnLuaCoreCtrlListChangeHandler;
        }

        private void BindEvents(Services.LuaServer luaServer)
        {
            btnDelAll.Click += (s, a) =>
            {
                if (!VgcApis.Libs.UI.Confirm(I18N.ConfirmDeleteAllScripts))
                {
                    return;
                }
                luaServer.RemoveAllScripts();
            };

            btnImport.Click += (s, a) =>
            {
                try
                {
                    var serializedScripts = VgcApis.Libs.UI.ReadFileContentFromDialog(
                        VgcApis.Models.Consts.Files.TxtExt);

                    // cancelle by user
                    if (serializedScripts == null)
                    {
                        return;
                    }

                    var scripts = JsonConvert.DeserializeObject<List<string[]>>(serializedScripts);
                    if (scripts != null && scripts.Count > 0)
                    {
                        luaServer.ImportScripts(scripts);
                        MessageBox.Show(I18N.ImportScriptsSuccess);
                        return;
                    }
                }
                catch { }
                MessageBox.Show(I18N.ImportScriptsFail);
            };

            btnExport.Click += (s, a) =>
            {
                try
                {
                    var scripts = luaServer.GetAllScripts();
                    var serializedScripts = JsonConvert.SerializeObject(scripts);
                    VgcApis.Libs.UI.SaveToFile(VgcApis.Models.Consts.Files.TxtExt, serializedScripts);
                }
                catch
                {
                    MessageBox.Show(I18N.ExportScriptsFail);
                }
            };

            btnStopAll.Click += (s, a) =>
            {
                var ctrls = luaServer.GetAllLuaCoreCtrls();
                foreach (var c in ctrls)
                {
                    c.Stop();
                }
            };

            btnKillAll.Click += (s, a) =>
            {
                var ctrls = luaServer.GetAllLuaCoreCtrls();
                foreach (var c in ctrls)
                {
                    c.Kill();
                }
            };
        }

        public void Cleanup()
        {
            luaServer.OnLuaCoreCtrlListChange -= OnLuaCoreCtrlListChangeHandler;
            RunInUiThread(() =>
            {
                ClearFlyPanel();
            });
        }
        #endregion

        #region private methods
        void OnLuaCoreCtrlListChangeHandler(object sender, EventArgs args)
        {
            RefreshFlyPanel();
        }

        void RunInUiThread(Action updater)
        {
            VgcApis.Libs.UI.RunInUiThread(flyLuaUiPanel, () =>
            {
                updater();
            });
        }

        void RefreshFlyPanel()
        {
            RunInUiThread(() =>
            {
                ClearFlyPanel();
                AddLuaCoreCtrlToPanel();
            });
        }

        void AddLuaCoreCtrlToPanel()
        {
            var ctrls = luaServer.GetAllLuaCoreCtrls();
            foreach (var c in ctrls)
            {
                var ui = new Views.UserControls.LuaUI(
                    luaServer, c);
                flyLuaUiPanel.Controls.Add(ui);
            }
        }

        void ClearFlyPanel()
        {
            var list = flyLuaUiPanel.Controls;
            foreach (Views.UserControls.LuaUI c in list)
            {
                c.Cleanup();
            }
            flyLuaUiPanel.Controls.Clear();
        }

        private void BindControls(
            FlowLayoutPanel flyLuaUiPanel,
            Button btnStopAll,
            Button btnKillAll,
            Button btnDelAll,
            Button btnImport,
            Button btnExport)
        {
            this.btnStopAll = btnStopAll;
            this.btnKillAll = btnKillAll;
            this.flyLuaUiPanel = flyLuaUiPanel;
            this.btnDelAll = btnDelAll;
            this.btnImport = btnImport;
            this.btnExport = btnExport;

        }

        #endregion
    }
}
