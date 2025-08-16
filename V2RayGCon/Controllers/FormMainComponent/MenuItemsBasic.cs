using System;
using System.IO;
using System.Windows.Forms;
using V2RayGCon.Resources.Resx;

namespace V2RayGCon.Controllers.FormMainComponent
{
    class MenuItemsBasic : FormMainComponentController
    {
        readonly Services.Servers servers;
        readonly Services.ShareLinkMgr slinkMgr;
        readonly Services.Updater updater;
        readonly Services.PluginsServer pluginServ;

        ToolStripMenuItem pluginToolStrip;

        public MenuItemsBasic(
            ToolStripMenuItem pluginToolStrip,
            ToolStripMenuItem miImportLinkFromClipboard,
            ToolStripMenuItem miAbout,
            ToolStripMenuItem miHelp,
            ToolStripMenuItem miFormTextEditor,
            ToolStripMenuItem miFormKeyGen,
            ToolStripMenuItem miFormLog,
            ToolStripMenuItem miFormOptions,
            ToolStripMenuItem miDownloadV2rayCore,
            ToolStripMenuItem miCleanupProgramData,
            ToolStripMenuItem miCheckVgcUpdate
        )
        {
            servers = Services.Servers.Instance;
            slinkMgr = Services.ShareLinkMgr.Instance;
            updater = Services.Updater.Instance;
            pluginServ = Services.PluginsServer.Instance;

            InitMenuPlugin(pluginToolStrip);

            InitMenuFile(miImportLinkFromClipboard);
            InitMenuWindows(miFormTextEditor, miFormKeyGen, miFormLog, miFormOptions);

            InitMenuAbout(
                miAbout,
                miHelp,
                miDownloadV2rayCore,
                miCleanupProgramData,
                miCheckVgcUpdate
            );
        }

        #region public method


        public override void Cleanup()
        {
            pluginServ.OnRequireMenuUpdate -= OnRequireMenuUpdateHandler;
        }
        #endregion

        #region private method
        void UpdatePluginMenu()
        {
            VgcApis.Misc.UI.Invoke(() =>
            {
                var plugins = pluginServ.GetAllEnabledPlugins();
                pluginToolStrip.DropDownItems.Clear();
                pluginToolStrip.DropDown.PerformLayout();

                if (plugins.Count <= 0)
                {
                    pluginToolStrip.Visible = false;
                    return;
                }

                foreach (var plugin in plugins)
                {
                    var mi = new ToolStripMenuItem(
                        plugin.Name,
                        plugin.Icon,
                        (s, a) => plugin.ShowMainForm()
                    );
                    pluginToolStrip.DropDownItems.Add(mi);
                    mi.ToolTipText = plugin.Description;
                }
                pluginToolStrip.Visible = true;
            });
        }

        void OnRequireMenuUpdateHandler(object sender, EventArgs evs)
        {
            VgcApis.Misc.UI.Invoke(UpdatePluginMenu);
        }

        void InitMenuPlugin(ToolStripMenuItem pluginToolStrip)
        {
            this.pluginToolStrip = pluginToolStrip;
            OnRequireMenuUpdateHandler(this, EventArgs.Empty);
            pluginServ.OnRequireMenuUpdate += OnRequireMenuUpdateHandler;
        }

        private void InitMenuAbout(
            ToolStripMenuItem aboutVGC,
            ToolStripMenuItem help,
            ToolStripMenuItem downloadV2rayCore,
            ToolStripMenuItem cleanupProgramData,
            ToolStripMenuItem miCheckVgcUpdate
        )
        {
            // menu about
            downloadV2rayCore.Click += (s, a) => Views.WinForms.FormDownloadCore.ShowForm();

            cleanupProgramData.Click += (s, a) => CleanupProgramDataFolder();

            aboutVGC.Click += (s, a) =>
                VgcApis.Misc.UI.VisitUrl(I18N.VistProjectPage, Properties.Resources.ProjectLink);

            help.Click += (s, a) =>
                VgcApis.Misc.UI.VisitUrl(I18N.VistWikiPage, Properties.Resources.WikiLink);

            miCheckVgcUpdate.Click += (s, a) => updater.CheckForUpdate(true);
        }

        private void InitMenuFile(ToolStripMenuItem importLinkFromClipboard)
        {
            // menu file
            importLinkFromClipboard.Click += (s, a) =>
            {
                string text = VgcApis.Misc.Utils.ReadFromClipboard();
                slinkMgr.ImportLinkWithV2cfgLinks(text);
            };
        }

        private static void InitMenuWindows(
            ToolStripMenuItem miFormTextEditor,
            ToolStripMenuItem miFormKeyGen,
            ToolStripMenuItem miFormLog,
            ToolStripMenuItem miFormOptions
        )
        {
            miFormTextEditor.Click += (s, a) =>
                Views.WinForms.FormTextConfigEditor.ShowEmptyConfig();

            miFormKeyGen.Click += (s, a) => Views.WinForms.FormKeyGen.ShowForm();

            miFormLog.Click += (s, a) => Views.WinForms.FormLog.ShowForm();

            miFormOptions.Click += (s, a) => Views.WinForms.FormOption.ShowForm();
        }

        private void CleanupProgramDataFolder()
        {
            if (!VgcApis.Misc.UI.Confirm(I18N.ConfirmRemoveProgramDataFolder))
            {
                return;
            }

            if (!Directory.Exists(Misc.Utils.GetSysAppDataFolder()))
            {
                VgcApis.Misc.UI.MsgBox(I18N.Done);
                return;
            }

            servers.StopAllServersThen(() =>
            {
                try
                {
                    Misc.Utils.DeleteAppDataFolder();
                }
                catch (IOException)
                {
                    VgcApis.Misc.UI.MsgBox(I18N.FileInUse);
                    return;
                }
                VgcApis.Misc.UI.MsgBox(I18N.Done);
            });
        }
        #endregion
    }
}
