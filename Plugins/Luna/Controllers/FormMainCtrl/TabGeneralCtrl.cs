using Luna.Resources.Langs;
using Luna.Views.UserControls;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Luna.Controllers.FormMainCtrl
{
    internal class TabGeneralCtrl
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

        public void Run(Services.LuaServer luaServer)
        {
            this.luaServer = luaServer;
            BindEvents(luaServer);
            BindDragDropEvent();

            RefreshFlyPanel();
            luaServer.OnRequireFlyPanelUpdate += OnLuaCoreCtrlListChangeHandler;
        }

        void BindDragDropEvent()
        {
            flyLuaUiPanel.DragEnter += (s, a) =>
            {
                a.Effect = DragDropEffects.Move;
            };

            flyLuaUiPanel.DragDrop += (s, a) =>
            {
                var data = a.Data;
                if (data.GetDataPresent(typeof(LuaUI)))
                {
                    SwapFlyControls(s, a);
                }
                else if (data.GetDataPresent(DataFormats.FileDrop))
                {
                    HandleFileDrop(a);
                }
            };
        }

        void HandleFileDrop(DragEventArgs args)
        {
            var filenames = args.Data.GetData(DataFormats.FileDrop) as string[];
            if (filenames == null)
            {
                return;
            }

            foreach (var filename in filenames)
            {
                if (!File.Exists(filename))
                {
                    continue;
                }

                string content;
                string scriptName;

                try
                {
                    content = File.ReadAllText(filename);
                    scriptName = Path.GetFileName(filename);
                }
                catch
                {
                    continue;
                }

                if (!string.IsNullOrWhiteSpace(content) && !string.IsNullOrWhiteSpace(scriptName))
                {
                    luaServer.AddOrReplaceScript(scriptName, content);
                }
            }
        }

        private void SwapFlyControls(object sender, DragEventArgs args)
        {
            // https://www.codeproject.com/Articles/48411/Using-the-FlowLayoutPanel-and-Reordering-with-Drag
            var panel = sender as FlowLayoutPanel;
            var curItem = args.Data.GetData(typeof(LuaUI)) as LuaUI;
            Point p = panel.PointToClient(new Point(args.X, args.Y));
            var destItem = panel.GetChildAtPoint(p) as Views.UserControls.LuaUI;
            if (curItem == null || destItem == null || curItem == destItem)
            {
                return;
            }

            // swap index
            var destIdx = destItem.GetIndex() + 0.5;
            var curIdx = (curItem.GetIndex() > destIdx) ? destIdx - 0.1 : destIdx + 0.1;
            destItem.SetIndex(destIdx);
            curItem.SetIndex(curIdx);
            luaServer.ResetIndex();  // this will invoke menu update event

            // refresh panel
            var destPos = panel.Controls.GetChildIndex(destItem, false);
            panel.Controls.SetChildIndex(curItem, destPos);
            panel.Invalidate();
        }

        private void BindEvents(Services.LuaServer luaServer)
        {
            btnDelAll.Click += (s, a) =>
            {
                if (!VgcApis.Misc.UI.Confirm(I18N.ConfirmDeleteAllScripts))
                {
                    return;
                }
                luaServer.RemoveAllScripts();
            };

            btnImport.Click += (s, a) =>
            {
                try
                {
                    var serializedScripts = VgcApis.Misc.UI.ReadFileContentFromDialog(
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
                    VgcApis.Misc.UI.SaveToFile(VgcApis.Models.Consts.Files.TxtExt, serializedScripts);
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
                    c.Abort();
                }
            };
        }

        public void Cleanup()
        {
            luaServer.OnRequireFlyPanelUpdate -= OnLuaCoreCtrlListChangeHandler;
            var list = flyLuaUiPanel.Controls;
            foreach (Views.UserControls.LuaUI c in list)
            {
                c.Cleanup();
            }
        }
        #endregion

        #region private methods
        void OnLuaCoreCtrlListChangeHandler(object sender, EventArgs args)
        {
            RefreshFlyPanel();
        }

        void RefreshFlyPanel()
        {
            VgcApis.Misc.UI.Invoke(() =>
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
