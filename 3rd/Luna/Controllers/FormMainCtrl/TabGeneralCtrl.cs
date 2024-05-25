using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Luna.Resources.Langs;
using Luna.Views.UserControls;
using Newtonsoft.Json;

namespace Luna.Controllers.FormMainCtrl
{
    internal class TabGeneralCtrl
    {
        readonly Button btnStopAll,
            btnKillAll,
            btnDelAll,
            btnImport,
            btnExport;
        readonly FlowLayoutPanel flyLuaUiPanel;

        public TabGeneralCtrl(
            FlowLayoutPanel flyLuaUiPanel,
            Button btnStopAll,
            Button btnKillAll,
            Button btnDelAll,
            Button btnImport,
            Button btnExport
        )
        {
            this.btnStopAll = btnStopAll;
            this.btnKillAll = btnKillAll;
            this.flyLuaUiPanel = flyLuaUiPanel;
            this.btnDelAll = btnDelAll;
            this.btnImport = btnImport;
            this.btnExport = btnExport;
        }

        #region public methods
        Services.LuaServer luaServer;
        Services.FormMgrSvc formMgrSvc;

        public void Run(Services.LuaServer luaServer, Services.FormMgrSvc formMgrSvc)
        {
            this.luaServer = luaServer;
            this.formMgrSvc = formMgrSvc;

            BindControlsEvent(luaServer);
            BindFlyPanelDragDropEvent();

            RefreshFlyPanel();
            luaServer.OnRequireFlyPanelUpdate += OnLuaCoreCtrlListChangeHandler;
        }

        public void Cleanup()
        {
            luaServer.OnRequireFlyPanelUpdate -= OnLuaCoreCtrlListChangeHandler;
            var list = flyLuaUiPanel.Controls;
            foreach (LuaUI c in list)
            {
                c.Cleanup();
            }
        }
        #endregion

        #region private methods
        void BindFlyPanelDragDropEvent()
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
            if (!(args.Data.GetData(DataFormats.FileDrop) is string[] filenames))
            {
                return;
            }

            var names = luaServer.GetAllScripts().Select(s => s[0]).ToList();
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

                if (string.IsNullOrWhiteSpace(content) || string.IsNullOrWhiteSpace(scriptName))
                {
                    continue;
                }

                if (
                    names.Contains(scriptName)
                    && !VgcApis.Misc.UI.Confirm($"{I18N.ReplaceScript} {scriptName}")
                )
                {
                    continue;
                }
                luaServer.AddOrReplaceScript(scriptName, content);
            }
        }

        private void SwapFlyControls(object sender, DragEventArgs args)
        {
            // https://www.codeproject.com/Articles/48411/Using-the-FlowLayoutPanel-and-Reordering-with-Drag
            var panel = sender as FlowLayoutPanel;
            Point p = panel.PointToClient(new Point(args.X, args.Y));
            if (
                !(args.Data.GetData(typeof(LuaUI)) is LuaUI curItem)
                || !(panel.GetChildAtPoint(p) is LuaUI destItem)
                || curItem == destItem
            )
            {
                return;
            }

            // swap index
            var destIdx = destItem.GetIndex() + 0.5;
            var curIdx = (curItem.GetIndex() > destIdx) ? destIdx - 0.1 : destIdx + 0.1;
            destItem.SetIndex(destIdx);
            curItem.SetIndex(curIdx);
            luaServer.ResetIndex(); // this will invoke menu update event

            // refresh panel
            var destPos = panel.Controls.GetChildIndex(destItem, false);
            panel.Controls.SetChildIndex(curItem, destPos);
            panel.Invalidate();
        }

        private void BindControlsEvent(Services.LuaServer luaServer)
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
                        VgcApis.Models.Consts.Files.TxtExt
                    );

                    // cancelle by user
                    if (serializedScripts == null)
                    {
                        return;
                    }

                    var scripts = JsonConvert.DeserializeObject<List<string[]>>(serializedScripts);
                    if (scripts != null && scripts.Count > 0)
                    {
                        luaServer.ImportScripts(scripts);
                        VgcApis.Misc.UI.MsgBox(I18N.ImportScriptsSuccess);
                        return;
                    }
                }
                catch { }
                VgcApis.Misc.UI.MsgBox(I18N.ImportScriptsFail);
            };

            btnExport.Click += (s, a) =>
            {
                try
                {
                    var scripts = luaServer.GetAllScripts();
                    var serializedScripts = JsonConvert.SerializeObject(scripts);
                    VgcApis.Misc.UI.SaveToFile(
                        VgcApis.Models.Consts.Files.TxtExt,
                        serializedScripts
                    );
                }
                catch
                {
                    VgcApis.Misc.UI.MsgBox(I18N.ExportScriptsFail);
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

        void OnLuaCoreCtrlListChangeHandler(object sender, EventArgs args)
        {
            RefreshFlyPanel();
        }

        void RefreshFlyPanel()
        {
            VgcApis.Misc.UI.Invoke(() =>
            {
                var ctrls = luaServer.GetAllLuaCoreCtrls();
                var removed = VgcApis.Misc.UI.DoHouseKeeping<LuaUI>(flyLuaUiPanel, ctrls.Count);
                ReloadAllCtrls(ctrls);
                DisposeRemovedCtrls(removed);
            });
        }

        void DisposeRemovedCtrls(List<LuaUI> rmCtrls)
        {
            foreach (var ctrl in rmCtrls)
            {
                ctrl.Cleanup();
            }
        }

        void ReloadAllCtrls(List<LuaCoreCtrl> ctrls)
        {
            var uis = flyLuaUiPanel.Controls.OfType<LuaUI>().ToList();
            if (uis.Count() != ctrls.Count())
            {
                return;
            }

            for (int i = 0; i < uis.Count(); i++)
            {
                uis[i].Reload(luaServer, formMgrSvc, ctrls[i]);
            }
        }
        #endregion
    }
}
