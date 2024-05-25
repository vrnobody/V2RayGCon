namespace NeoLuna.Views.WinForms
{
    partial class FormEditor
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormEditor));
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.lbStatusBarMsg = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripSpring = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusCodeAnalyze = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusClrLib = new System.Windows.Forms.ToolStripStatusLabel();
            this.splitContainerTabEditor = new System.Windows.Forms.SplitContainer();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.panel3 = new System.Windows.Forms.Panel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panelScriptName = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.cboxScriptName = new System.Windows.Forms.ComboBox();
            this.btnKillScript = new System.Windows.Forms.Button();
            this.btnClearOutput = new System.Windows.Forms.Button();
            this.btnNewScript = new System.Windows.Forms.Button();
            this.btnStopScript = new System.Windows.Forms.Button();
            this.btnSaveScript = new System.Windows.Forms.Button();
            this.btnRunScript = new System.Windows.Forms.Button();
            this.pnlScriptEditor = new System.Windows.Forms.Panel();
            this.panelScriptDebugTools = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.tboxQuickSearch = new System.Windows.Forms.TextBox();
            this.btnGotoLine = new System.Windows.Forms.Button();
            this.btnShowFormSearch = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.panelSelectMethods = new System.Windows.Forms.Panel();
            this.panel4 = new System.Windows.Forms.Panel();
            this.cboxFunctionList = new System.Windows.Forms.ComboBox();
            this.panelSelectVariable = new System.Windows.Forms.Panel();
            this.panel5 = new System.Windows.Forms.Panel();
            this.cboxVarList = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rtBoxOutput = new VgcApis.UserControls.ExRichTextBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newWindowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showScriptManagerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.loadFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadCLRLibraryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.enableCodeAnalyzeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.outputPanelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.toolStripContainer1.BottomToolStripPanel.SuspendLayout();
            this.toolStripContainer1.ContentPanel.SuspendLayout();
            this.toolStripContainer1.TopToolStripPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerTabEditor)).BeginInit();
            this.splitContainerTabEditor.Panel1.SuspendLayout();
            this.splitContainerTabEditor.Panel2.SuspendLayout();
            this.splitContainerTabEditor.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.panelScriptName.SuspendLayout();
            this.panelScriptDebugTools.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.panelSelectMethods.SuspendLayout();
            this.panelSelectVariable.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStripContainer1
            // 
            // 
            // toolStripContainer1.BottomToolStripPanel
            // 
            this.toolStripContainer1.BottomToolStripPanel.Controls.Add(this.statusStrip1);
            // 
            // toolStripContainer1.ContentPanel
            // 
            this.toolStripContainer1.ContentPanel.Controls.Add(this.splitContainerTabEditor);
            resources.ApplyResources(this.toolStripContainer1.ContentPanel, "toolStripContainer1.ContentPanel");
            resources.ApplyResources(this.toolStripContainer1, "toolStripContainer1");
            this.toolStripContainer1.Name = "toolStripContainer1";
            // 
            // toolStripContainer1.TopToolStripPanel
            // 
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.menuStrip1);
            // 
            // statusStrip1
            // 
            resources.ApplyResources(this.statusStrip1, "statusStrip1");
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lbStatusBarMsg,
            this.toolStripSpring,
            this.toolStripStatusCodeAnalyze,
            this.toolStripStatusClrLib});
            this.statusStrip1.Name = "statusStrip1";
            // 
            // lbStatusBarMsg
            // 
            this.lbStatusBarMsg.Name = "lbStatusBarMsg";
            resources.ApplyResources(this.lbStatusBarMsg, "lbStatusBarMsg");
            // 
            // toolStripSpring
            // 
            this.toolStripSpring.Name = "toolStripSpring";
            resources.ApplyResources(this.toolStripSpring, "toolStripSpring");
            this.toolStripSpring.Spring = true;
            // 
            // toolStripStatusCodeAnalyze
            // 
            this.toolStripStatusCodeAnalyze.Name = "toolStripStatusCodeAnalyze";
            resources.ApplyResources(this.toolStripStatusCodeAnalyze, "toolStripStatusCodeAnalyze");
            // 
            // toolStripStatusClrLib
            // 
            this.toolStripStatusClrLib.Name = "toolStripStatusClrLib";
            resources.ApplyResources(this.toolStripStatusClrLib, "toolStripStatusClrLib");
            // 
            // splitContainerTabEditor
            // 
            resources.ApplyResources(this.splitContainerTabEditor, "splitContainerTabEditor");
            this.splitContainerTabEditor.Name = "splitContainerTabEditor";
            // 
            // splitContainerTabEditor.Panel1
            // 
            this.splitContainerTabEditor.Panel1.Controls.Add(this.groupBox2);
            // 
            // splitContainerTabEditor.Panel2
            // 
            this.splitContainerTabEditor.Panel2.Controls.Add(this.groupBox1);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.panel3);
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.tableLayoutPanel1);
            resources.ApplyResources(this.panel3, "panel3");
            this.panel3.Name = "panel3";
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.panelScriptName, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.pnlScriptEditor, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.panelScriptDebugTools, 0, 1);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // panelScriptName
            // 
            this.panelScriptName.Controls.Add(this.label1);
            this.panelScriptName.Controls.Add(this.cboxScriptName);
            this.panelScriptName.Controls.Add(this.btnKillScript);
            this.panelScriptName.Controls.Add(this.btnClearOutput);
            this.panelScriptName.Controls.Add(this.btnNewScript);
            this.panelScriptName.Controls.Add(this.btnStopScript);
            this.panelScriptName.Controls.Add(this.btnSaveScript);
            this.panelScriptName.Controls.Add(this.btnRunScript);
            resources.ApplyResources(this.panelScriptName, "panelScriptName");
            this.panelScriptName.Name = "panelScriptName";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            this.toolTip1.SetToolTip(this.label1, resources.GetString("label1.ToolTip"));
            // 
            // cboxScriptName
            // 
            resources.ApplyResources(this.cboxScriptName, "cboxScriptName");
            this.cboxScriptName.FormattingEnabled = true;
            this.cboxScriptName.Name = "cboxScriptName";
            this.toolTip1.SetToolTip(this.cboxScriptName, resources.GetString("cboxScriptName.ToolTip"));
            // 
            // btnKillScript
            // 
            resources.ApplyResources(this.btnKillScript, "btnKillScript");
            this.btnKillScript.Name = "btnKillScript";
            this.toolTip1.SetToolTip(this.btnKillScript, resources.GetString("btnKillScript.ToolTip"));
            this.btnKillScript.UseVisualStyleBackColor = true;
            // 
            // btnClearOutput
            // 
            resources.ApplyResources(this.btnClearOutput, "btnClearOutput");
            this.btnClearOutput.Name = "btnClearOutput";
            this.toolTip1.SetToolTip(this.btnClearOutput, resources.GetString("btnClearOutput.ToolTip"));
            this.btnClearOutput.UseVisualStyleBackColor = true;
            // 
            // btnNewScript
            // 
            resources.ApplyResources(this.btnNewScript, "btnNewScript");
            this.btnNewScript.Name = "btnNewScript";
            this.toolTip1.SetToolTip(this.btnNewScript, resources.GetString("btnNewScript.ToolTip"));
            this.btnNewScript.UseVisualStyleBackColor = true;
            // 
            // btnStopScript
            // 
            resources.ApplyResources(this.btnStopScript, "btnStopScript");
            this.btnStopScript.Name = "btnStopScript";
            this.toolTip1.SetToolTip(this.btnStopScript, resources.GetString("btnStopScript.ToolTip"));
            this.btnStopScript.UseVisualStyleBackColor = true;
            // 
            // btnSaveScript
            // 
            resources.ApplyResources(this.btnSaveScript, "btnSaveScript");
            this.btnSaveScript.Name = "btnSaveScript";
            this.toolTip1.SetToolTip(this.btnSaveScript, resources.GetString("btnSaveScript.ToolTip"));
            this.btnSaveScript.UseVisualStyleBackColor = true;
            // 
            // btnRunScript
            // 
            resources.ApplyResources(this.btnRunScript, "btnRunScript");
            this.btnRunScript.Name = "btnRunScript";
            this.toolTip1.SetToolTip(this.btnRunScript, resources.GetString("btnRunScript.ToolTip"));
            this.btnRunScript.UseVisualStyleBackColor = true;
            // 
            // pnlScriptEditor
            // 
            resources.ApplyResources(this.pnlScriptEditor, "pnlScriptEditor");
            this.pnlScriptEditor.Name = "pnlScriptEditor";
            // 
            // panelScriptDebugTools
            // 
            this.panelScriptDebugTools.Controls.Add(this.panel2);
            this.panelScriptDebugTools.Controls.Add(this.panel1);
            this.panelScriptDebugTools.Controls.Add(this.label2);
            resources.ApplyResources(this.panelScriptDebugTools, "panelScriptDebugTools");
            this.panelScriptDebugTools.Name = "panelScriptDebugTools";
            // 
            // panel2
            // 
            resources.ApplyResources(this.panel2, "panel2");
            this.panel2.Controls.Add(this.tboxQuickSearch);
            this.panel2.Controls.Add(this.btnGotoLine);
            this.panel2.Controls.Add(this.btnShowFormSearch);
            this.panel2.Name = "panel2";
            // 
            // tboxQuickSearch
            // 
            resources.ApplyResources(this.tboxQuickSearch, "tboxQuickSearch");
            this.tboxQuickSearch.Name = "tboxQuickSearch";
            this.toolTip1.SetToolTip(this.tboxQuickSearch, resources.GetString("tboxQuickSearch.ToolTip"));
            // 
            // btnGotoLine
            // 
            resources.ApplyResources(this.btnGotoLine, "btnGotoLine");
            this.btnGotoLine.Name = "btnGotoLine";
            this.toolTip1.SetToolTip(this.btnGotoLine, resources.GetString("btnGotoLine.ToolTip"));
            this.btnGotoLine.UseVisualStyleBackColor = true;
            // 
            // btnShowFormSearch
            // 
            resources.ApplyResources(this.btnShowFormSearch, "btnShowFormSearch");
            this.btnShowFormSearch.Name = "btnShowFormSearch";
            this.toolTip1.SetToolTip(this.btnShowFormSearch, resources.GetString("btnShowFormSearch.ToolTip"));
            this.btnShowFormSearch.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Controls.Add(this.tableLayoutPanel2);
            this.panel1.Name = "panel1";
            // 
            // tableLayoutPanel2
            // 
            resources.ApplyResources(this.tableLayoutPanel2, "tableLayoutPanel2");
            this.tableLayoutPanel2.Controls.Add(this.panelSelectMethods, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.panelSelectVariable, 0, 0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            // 
            // panelSelectMethods
            // 
            this.panelSelectMethods.Controls.Add(this.panel4);
            this.panelSelectMethods.Controls.Add(this.cboxFunctionList);
            resources.ApplyResources(this.panelSelectMethods, "panelSelectMethods");
            this.panelSelectMethods.Name = "panelSelectMethods";
            // 
            // panel4
            // 
            resources.ApplyResources(this.panel4, "panel4");
            this.panel4.Name = "panel4";
            this.toolTip1.SetToolTip(this.panel4, resources.GetString("panel4.ToolTip"));
            // 
            // cboxFunctionList
            // 
            resources.ApplyResources(this.cboxFunctionList, "cboxFunctionList");
            this.cboxFunctionList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboxFunctionList.FormattingEnabled = true;
            this.cboxFunctionList.Name = "cboxFunctionList";
            this.toolTip1.SetToolTip(this.cboxFunctionList, resources.GetString("cboxFunctionList.ToolTip"));
            // 
            // panelSelectVariable
            // 
            this.panelSelectVariable.Controls.Add(this.panel5);
            this.panelSelectVariable.Controls.Add(this.cboxVarList);
            resources.ApplyResources(this.panelSelectVariable, "panelSelectVariable");
            this.panelSelectVariable.Name = "panelSelectVariable";
            // 
            // panel5
            // 
            resources.ApplyResources(this.panel5, "panel5");
            this.panel5.Name = "panel5";
            this.toolTip1.SetToolTip(this.panel5, resources.GetString("panel5.ToolTip"));
            // 
            // cboxVarList
            // 
            resources.ApplyResources(this.cboxVarList, "cboxVarList");
            this.cboxVarList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboxVarList.FormattingEnabled = true;
            this.cboxVarList.Name = "cboxVarList";
            this.toolTip1.SetToolTip(this.cboxVarList, resources.GetString("cboxVarList.ToolTip"));
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            this.toolTip1.SetToolTip(this.label2, resources.GetString("label2.ToolTip"));
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rtBoxOutput);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // rtBoxOutput
            // 
            this.rtBoxOutput.BackColor = System.Drawing.SystemColors.Control;
            this.rtBoxOutput.DetectUrls = false;
            resources.ApplyResources(this.rtBoxOutput, "rtBoxOutput");
            this.rtBoxOutput.Name = "rtBoxOutput";
            this.rtBoxOutput.ReadOnly = true;
            // 
            // menuStrip1
            // 
            resources.ApplyResources(this.menuStrip1, "menuStrip1");
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.optionToolStripMenuItem});
            this.menuStrip1.Name = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newWindowToolStripMenuItem,
            this.showScriptManagerToolStripMenuItem,
            this.toolStripSeparator1,
            this.loadFileToolStripMenuItem,
            this.saveAsToolStripMenuItem,
            this.toolStripMenuItem1,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            resources.ApplyResources(this.fileToolStripMenuItem, "fileToolStripMenuItem");
            // 
            // newWindowToolStripMenuItem
            // 
            this.newWindowToolStripMenuItem.Name = "newWindowToolStripMenuItem";
            resources.ApplyResources(this.newWindowToolStripMenuItem, "newWindowToolStripMenuItem");
            // 
            // showScriptManagerToolStripMenuItem
            // 
            this.showScriptManagerToolStripMenuItem.Name = "showScriptManagerToolStripMenuItem";
            resources.ApplyResources(this.showScriptManagerToolStripMenuItem, "showScriptManagerToolStripMenuItem");
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            // 
            // loadFileToolStripMenuItem
            // 
            this.loadFileToolStripMenuItem.Name = "loadFileToolStripMenuItem";
            resources.ApplyResources(this.loadFileToolStripMenuItem, "loadFileToolStripMenuItem");
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            resources.ApplyResources(this.saveAsToolStripMenuItem, "saveAsToolStripMenuItem");
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            resources.ApplyResources(this.toolStripMenuItem1, "toolStripMenuItem1");
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            resources.ApplyResources(this.exitToolStripMenuItem, "exitToolStripMenuItem");
            // 
            // optionToolStripMenuItem
            // 
            this.optionToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadCLRLibraryToolStripMenuItem,
            this.enableCodeAnalyzeToolStripMenuItem,
            this.outputPanelToolStripMenuItem});
            this.optionToolStripMenuItem.Name = "optionToolStripMenuItem";
            resources.ApplyResources(this.optionToolStripMenuItem, "optionToolStripMenuItem");
            // 
            // loadCLRLibraryToolStripMenuItem
            // 
            this.loadCLRLibraryToolStripMenuItem.Name = "loadCLRLibraryToolStripMenuItem";
            resources.ApplyResources(this.loadCLRLibraryToolStripMenuItem, "loadCLRLibraryToolStripMenuItem");
            // 
            // enableCodeAnalyzeToolStripMenuItem
            // 
            this.enableCodeAnalyzeToolStripMenuItem.Name = "enableCodeAnalyzeToolStripMenuItem";
            resources.ApplyResources(this.enableCodeAnalyzeToolStripMenuItem, "enableCodeAnalyzeToolStripMenuItem");
            // 
            // outputPanelToolStripMenuItem
            // 
            this.outputPanelToolStripMenuItem.Name = "outputPanelToolStripMenuItem";
            resources.ApplyResources(this.outputPanelToolStripMenuItem, "outputPanelToolStripMenuItem");
            this.outputPanelToolStripMenuItem.Click += new System.EventHandler(this.outputPanelToolStripMenuItem_Click);
            // 
            // FormEditor
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.toolStripContainer1);
            this.KeyPreview = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "FormEditor";
            this.Load += new System.EventHandler(this.FormEditor_Load);
            this.toolStripContainer1.BottomToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.BottomToolStripPanel.PerformLayout();
            this.toolStripContainer1.ContentPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.PerformLayout();
            this.toolStripContainer1.ResumeLayout(false);
            this.toolStripContainer1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.splitContainerTabEditor.Panel1.ResumeLayout(false);
            this.splitContainerTabEditor.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerTabEditor)).EndInit();
            this.splitContainerTabEditor.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.panelScriptName.ResumeLayout(false);
            this.panelScriptName.PerformLayout();
            this.panelScriptDebugTools.ResumeLayout(false);
            this.panelScriptDebugTools.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.panelSelectMethods.ResumeLayout(false);
            this.panelSelectVariable.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel lbStatusBarMsg;
        private System.Windows.Forms.ToolStripContainer toolStripContainer1;
        private System.Windows.Forms.SplitContainer splitContainerTabEditor;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel panelScriptName;
        private System.Windows.Forms.ComboBox cboxScriptName;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Button btnKillScript;
        private System.Windows.Forms.Button btnStopScript;
        private System.Windows.Forms.Button btnRunScript;
        private System.Windows.Forms.Button btnSaveScript;
        private System.Windows.Forms.Panel pnlScriptEditor;
        private System.Windows.Forms.GroupBox groupBox1;
        private VgcApis.UserControls.ExRichTextBox rtBoxOutput;
        private System.Windows.Forms.Button btnClearOutput;
        private System.Windows.Forms.Button btnNewScript;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newWindowToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panelScriptDebugTools;
        private System.Windows.Forms.ComboBox cboxFunctionList;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cboxVarList;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.ToolStripMenuItem optionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadCLRLibraryToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem enableCodeAnalyzeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showScriptManagerToolStripMenuItem;
        private System.Windows.Forms.ToolStripStatusLabel toolStripSpring;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusCodeAnalyze;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusClrLib;
        private System.Windows.Forms.ToolStripMenuItem outputPanelToolStripMenuItem;
        private System.Windows.Forms.TextBox tboxQuickSearch;
        private System.Windows.Forms.Button btnGotoLine;
        private System.Windows.Forms.Button btnShowFormSearch;
        private System.Windows.Forms.Panel panelSelectMethods;
        private System.Windows.Forms.Panel panelSelectVariable;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Panel panel5;
    }
}
