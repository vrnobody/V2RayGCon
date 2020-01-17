using System;
using System.Drawing;
using System.Windows.Forms;
using V2RayGCon.Resource.Resx;


namespace V2RayGCon.Views.WinForms
{
    #region auto hide tools panel
    struct ToolsPanelController
    {
        public int span;
        public int tabWidth;

        public Rectangle panel;
        public Rectangle page;

        public Lib.Sys.CancelableTimeout timerHide, timerShow;

        public void Dispose()
        {
            timerHide?.Release();
            timerShow?.Release();
        }
    };
    #endregion

    public partial class FormConfiger : Form
    {
        Controller.FormConfigerCtrl configer;
        Service.Setting setting;
        Service.Servers servers;

        VgcApis.WinForms.FormSearch formSearch;
        ToolsPanelController toolsPanelController;
        string originalConfigString;
        string formTitle;
        bool isShowPanel;

        ScintillaNET.Scintilla editor;

        public FormConfiger(string originalConfigString = null)
        {
            setting = Service.Setting.Instance;
            servers = Service.Servers.Instance;

            isShowPanel = setting.isShowConfigerToolsPanel;
            formSearch = null;
            InitializeComponent();
            formTitle = this.Text;
            this.originalConfigString = originalConfigString;

            VgcApis.Libs.UI.AutoSetFormIcon(this);
            this.Show();
        }

        private void FormConfiger_Shown(object sender, EventArgs e)
        {
            setting.RestoreFormRect(this);

            InitToolsPanel();
            this.configer = InitConfiger();

            SetTitle(configer.GetAlias());
            ToggleToolsPanel(isShowPanel);

            chkIsV4.Checked = setting.isUseV4;

            editor = configer
                .GetComponent<Controller.ConfigerComponet.Editor>()
                .GetEditor();

            editor.Click += OnMouseLeaveToolsPanel;
            BindServerEvents();

            this.FormClosing += (s, a) =>
            {
                if (!configer.IsConfigSaved())
                {
                    a.Cancel = !Lib.UI.Confirm(I18N.ConfirmCloseWinWithoutSave);
                }
            };

            this.FormClosed += (s, a) =>
            {
                formSearch?.Close();
                editor.Click -= OnMouseLeaveToolsPanel;
                toolsPanelController.Dispose();
                ReleaseServerEvents();
                configer.Cleanup();
                editor?.Dispose();
                setting.SaveFormRect(this);
                setting.LazyGC();
            };

            configer.UpdateServerMenusLater();
        }



        #region UI event handler
        private void ShowLeftPanelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToggleToolsPanel(true);
        }

        private void HideLeftPanelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToggleToolsPanel(false);
        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void SaveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            configer.InjectConfigHelper(null);

            switch (VgcApis.Libs.UI.ShowSaveFileDialog(
                VgcApis.Models.Consts.Files.JsonExt,
                configer.GetConfigFormated(),
                out string filename))
            {
                case VgcApis.Models.Datas.Enum.SaveFileErrorCode.Success:
                    SetTitle(filename);
                    configer.MarkOriginalFile();
                    MessageBox.Show(I18N.Done);
                    break;
                case VgcApis.Models.Datas.Enum.SaveFileErrorCode.Fail:
                    MessageBox.Show(I18N.WriteFileFail);
                    break;
                case VgcApis.Models.Datas.Enum.SaveFileErrorCode.Cancel:
                    // do nothing
                    break;
            }
        }

        private void AddNewServerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Lib.UI.Confirm(I18N.AddNewServer))
            {
                configer.AddNewServer();
                SetTitle(configer.GetAlias());
            }
        }

        private void LoadJsonToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!configer.IsConfigSaved()
                && !Lib.UI.Confirm(I18N.ConfirmLoadNewServer))
            {
                return;
            }

            var tuple = VgcApis.Libs.UI.ReadFileFromDialog(
                VgcApis.Models.Consts.Files.JsonExt);

            var json = tuple.Item1;
            var filename = tuple.Item2;

            // user cancelled.
            if (json == null)
            {
                return;
            }

            if (configer.LoadJsonFromFile(json))
            {
                cboxConfigSection.SelectedIndex = 0;
                SetTitle(filename);
            }
            else
            {
                MessageBox.Show(I18N.LoadJsonFail);
            }
        }

        private void NewWinToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            new FormConfiger();
        }

        private void SearchBoxToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowSearchBox();
        }

        private void SaveConfigStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Lib.UI.Confirm(I18N.ConfirmSaveCurConfig))
            {
                if (configer.SaveServer())
                {
                    SetTitle(configer.GetAlias());
                }
            }
        }

        private void TabCtrlToolPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (isShowPanel)
            {
                return;
            }

            for (int i = 0; i < tabCtrlToolPanel.TabCount; i++)
            {
                if (tabCtrlToolPanel.GetTabRect(i).Contains(e.Location))
                {
                    if (tabCtrlToolPanel.SelectedIndex != i)
                    {
                        tabCtrlToolPanel.SelectTab(i);
                    }
                    break;
                }
            }
        }

        private void TabCtrlToolPanel_MouseLeave(object sender, EventArgs e)
        {
            toolsPanelController.timerShow.Cancel();
        }
        #endregion

        #region bind hotkey
        protected override bool ProcessCmdKey(ref Message msg, Keys keyCode)
        {
            switch (keyCode)
            {
                case (Keys.Control | Keys.P):
                    ToggleToolsPanel(!isShowPanel);
                    break;
                case (Keys.Control | Keys.F):
                    ShowSearchBox();
                    break;
                case (Keys.Control | Keys.S):
                    configer.InjectConfigHelper(null);
                    break;
            }
            return base.ProcessCmdKey(ref msg, keyCode);
        }
        #endregion

        #region init
        Controller.FormConfigerCtrl InitConfiger()
        {
            var configer = new Controller.FormConfigerCtrl(this.originalConfigString);

            configer
                .Plug(new Controller.ConfigerComponet.EnvVar(
                    cboxImportAlias,
                    tboxImportURL,
                    btnInsertImport,
                    cboxEnvName,
                    tboxEnvValue,
                    btnInsertEnv))

                .Plug(new Controller.ConfigerComponet.Editor(
                    panelScintilla,
                    cboxConfigSection,
                    cboxExamples,
                    btnFormat,
                    btnClearModify))

                .Plug(new Controller.ConfigerComponet.Vmess(
                    tboxVMessID,
                    tboxVMessLevel,
                    tboxVMessAid,
                    tboxVMessIPaddr,
                    rbtnIsServerMode,
                    chkIsV4,
                    btnVMessGenUUID,
                    btnVMessInsertClient))

                .Plug(new Controller.ConfigerComponet.VGC(
                    tboxVGCAlias,
                    tboxVGCDesc,
                    btnInsertVGC))

                .Plug(new Controller.ConfigerComponet.StreamSettings(
                    cboxStreamType,
                    cboxStreamParam,
                    rbtnIsServerMode,
                    chkIsV4,
                    btnInsertStream,
                    chkStreamUseTls,
                    chkStreamUseSockopt))

                .Plug(new Controller.ConfigerComponet.Shadowsocks(
                    rbtnIsServerMode,
                    chkIsV4,
                    tboxSSAddr,
                    tboxSSPassword,
                    chkSSIsShowPassword,
                    cboxSSMethod,
                    chkSSIsUseOTA,
                    btnInsertSSSettings))

                .Plug(new Controller.ConfigerComponet.Import(
                    panelExpandConfig,
                    cboxGlobalImport,
                    btnExpandImport,
                    btnImportClearCache,
                    btnCopyExpansedConfig,
                    btnSaveExpansedConfigToFile))

                .Plug(new Controller.ConfigerComponet.Quick(
                    btnQConSkipCN,
                    btnQConMTProto,
                    chkIsV4))

                .Plug(new Controller.ConfigerComponet.MenuUpdater(
                    this,
                    replaceExistServerToolStripMenuItem,
                    loadServerToolStripMenuItem));

            configer.Prepare();
            return configer;
        }

        void ExpanseToolsPanel()
        {
            var width = toolsPanelController.panel.Width;
            if (pnlTools.Width != width)
            {
                VgcApis.Libs.UI.RunInUiThread(pnlTools, () =>
                {
                    pnlTools.Width = width;
                });
            }
        }

        void InitToolsPanel()
        {
            toolsPanelController.panel = new Rectangle(pnlTools.Location, pnlTools.Size);
            var span = pnlTools.Margin.Left;
            toolsPanelController.span = span;
            toolsPanelController.tabWidth = tabCtrlToolPanel.Left + tabCtrlToolPanel.ItemSize.Width;

            toolsPanelController.timerHide = new Lib.Sys.CancelableTimeout(FoldToolsPanel, 800);
            toolsPanelController.timerShow = new Lib.Sys.CancelableTimeout(ExpanseToolsPanel, 500);

            var page = tabCtrlToolPanel.TabPages[0];
            toolsPanelController.page = new Rectangle(
                pnlTools.Location.X + page.Left,
                pnlTools.Location.Y + page.Top,
                page.Width,
                page.Height);

            for (int i = 0; i < tabCtrlToolPanel.TabCount; i++)
            {
                tabCtrlToolPanel.TabPages[i].MouseEnter += OnMouseEnterToolsPanel;
                tabCtrlToolPanel.TabPages[i].MouseLeave += (s, e) =>
                {
                    var rect = toolsPanelController.page;
                    rect.Height = tabCtrlToolPanel.TabPages[0].Height;
                    if (!rect.Contains(PointToClient(Cursor.Position)))
                    {
                        OnMouseLeaveToolsPanel(s, e);
                    }
                };
            }

            tabCtrlToolPanel.MouseLeave += OnMouseLeaveToolsPanel;
            tabCtrlToolPanel.MouseEnter += OnMouseEnterToolsPanel;
        }
        #endregion

        #region private method
        private void ReleaseServerEvents()
        {
            servers.OnServerCountChange -= MenuUpdateHandler;
            servers.OnServerPropertyChange -= MenuUpdateHandler;
        }

        private void BindServerEvents()
        {
            servers.OnServerCountChange += MenuUpdateHandler;
            servers.OnServerPropertyChange += MenuUpdateHandler;
        }

        public void SetTitle(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                this.Text = formTitle;
            }
            else
            {
                this.Text = string.Format("{0} - {1}", formTitle, name);
            }
        }

        void ToggleToolsPanel(bool visible)
        {
            var pnlCtrl = toolsPanelController;
            var formSize = this.ClientSize;
            if (formSize.Width < pnlCtrl.panel.Width + pnlCtrl.span * 2)
            {
                visible = false;
            }

            pnlTools.Visible = false;
            pnlEditor.Visible = false;

            if (visible)
            {
                pnlTools.Width = pnlCtrl.panel.Width;
                pnlEditor.Left = pnlTools.Left + pnlTools.Width + pnlCtrl.span;
            }
            else
            {
                pnlTools.Width = toolsPanelController.tabWidth;
                pnlEditor.Left = pnlTools.Left + pnlTools.Width + pnlCtrl.span;
            }
            pnlEditor.Width = this.ClientSize.Width - pnlEditor.Left - pnlCtrl.span;

            pnlTools.Visible = true;
            pnlEditor.Visible = true;

            showLeftPanelToolStripMenuItem.Checked = visible;
            hideLeftPanelToolStripMenuItem.Checked = !visible;

            setting.isShowConfigerToolsPanel = visible;
            isShowPanel = visible;
        }

        void MenuUpdateHandler(object sender, EventArgs args)
        {
            configer.UpdateServerMenusLater();
        }

        void OnMouseEnterToolsPanel(object sender, EventArgs e)
        {
            toolsPanelController.timerHide.Cancel();
            toolsPanelController.timerShow.Start();
        }

        void FoldToolsPanel()
        {
            var visible = isShowPanel;
            var width = toolsPanelController.panel.Width;

            if (!visible)
            {
                width = toolsPanelController.tabWidth;
            }

            if (pnlTools.Width != width)
            {
                VgcApis.Libs.UI.RunInUiThread(pnlTools, () =>
                {
                    pnlTools.Width = width;
                });
            }
        }

        void OnMouseLeaveToolsPanel(object sender, EventArgs e)
        {
            toolsPanelController.timerShow.Cancel();
            toolsPanelController.timerHide.Start();
        }

        void ShowSearchBox()
        {
            if (formSearch != null)
            {
                return;
            }
            var editor = configer.GetComponent<Controller.ConfigerComponet.Editor>();
            formSearch = new VgcApis.WinForms.FormSearch(editor.GetEditor());
            formSearch.FormClosed += (s, a) => formSearch = null;
        }
        #endregion
    }
}
