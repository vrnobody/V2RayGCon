namespace V2RayGCon.Views.WinForms
{
    partial class FormSimAddVmessClient
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormSimAddVmessClient));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnGenUserID = new System.Windows.Forms.Button();
            this.tboxAID = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.tboxPort = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.tboxUID = new System.Windows.Forms.TextBox();
            this.tboxHost = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.chkboxTLS = new System.Windows.Forms.CheckBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.cboxKCP = new System.Windows.Forms.ComboBox();
            this.rbtnKCP = new System.Windows.Forms.RadioButton();
            this.rbtnTCP = new System.Windows.Forms.RadioButton();
            this.tboxWSPath = new System.Windows.Forms.TextBox();
            this.rbtnWS = new System.Windows.Forms.RadioButton();
            this.rbtnNoStream = new System.Windows.Forms.RadioButton();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.tboxAlias = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.linkConfigEditor = new System.Windows.Forms.LinkLabel();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnGenUserID);
            this.groupBox1.Controls.Add(this.tboxAID);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.tboxPort);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.tboxUID);
            this.groupBox1.Controls.Add(this.tboxHost);
            this.groupBox1.Controls.Add(this.label3);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // btnGenUserID
            // 
            resources.ApplyResources(this.btnGenUserID, "btnGenUserID");
            this.btnGenUserID.Name = "btnGenUserID";
            this.btnGenUserID.UseVisualStyleBackColor = true;
            this.btnGenUserID.Click += new System.EventHandler(this.btnGenUserID_Click);
            // 
            // tboxAID
            // 
            resources.ApplyResources(this.tboxAID, "tboxAID");
            this.tboxAID.Name = "tboxAID";
            // 
            // label8
            // 
            resources.ApplyResources(this.label8, "label8");
            this.label8.Name = "label8";
            // 
            // tboxPort
            // 
            resources.ApplyResources(this.tboxPort, "tboxPort");
            this.tboxPort.Name = "tboxPort";
            this.tboxPort.TextChanged += new System.EventHandler(this.tboxPort_TextChanged);
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // tboxUID
            // 
            resources.ApplyResources(this.tboxUID, "tboxUID");
            this.tboxUID.Name = "tboxUID";
            this.toolTip1.SetToolTip(this.tboxUID, resources.GetString("tboxUID.ToolTip"));
            this.tboxUID.TextChanged += new System.EventHandler(this.tboxUID_TextChanged);
            // 
            // tboxHost
            // 
            resources.ApplyResources(this.tboxHost, "tboxHost");
            this.tboxHost.Name = "tboxHost";
            this.toolTip1.SetToolTip(this.tboxHost, resources.GetString("tboxHost.ToolTip"));
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.chkboxTLS);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.cboxKCP);
            this.groupBox2.Controls.Add(this.rbtnKCP);
            this.groupBox2.Controls.Add(this.rbtnTCP);
            this.groupBox2.Controls.Add(this.tboxWSPath);
            this.groupBox2.Controls.Add(this.rbtnWS);
            this.groupBox2.Controls.Add(this.rbtnNoStream);
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // chkboxTLS
            // 
            resources.ApplyResources(this.chkboxTLS, "chkboxTLS");
            this.chkboxTLS.Name = "chkboxTLS";
            this.chkboxTLS.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // cboxKCP
            // 
            this.cboxKCP.FormattingEnabled = true;
            resources.ApplyResources(this.cboxKCP, "cboxKCP");
            this.cboxKCP.Name = "cboxKCP";
            // 
            // rbtnKCP
            // 
            resources.ApplyResources(this.rbtnKCP, "rbtnKCP");
            this.rbtnKCP.Name = "rbtnKCP";
            this.rbtnKCP.UseVisualStyleBackColor = true;
            // 
            // rbtnTCP
            // 
            resources.ApplyResources(this.rbtnTCP, "rbtnTCP");
            this.rbtnTCP.Name = "rbtnTCP";
            this.rbtnTCP.UseVisualStyleBackColor = true;
            // 
            // tboxWSPath
            // 
            resources.ApplyResources(this.tboxWSPath, "tboxWSPath");
            this.tboxWSPath.Name = "tboxWSPath";
            this.toolTip1.SetToolTip(this.tboxWSPath, resources.GetString("tboxWSPath.ToolTip"));
            // 
            // rbtnWS
            // 
            resources.ApplyResources(this.rbtnWS, "rbtnWS");
            this.rbtnWS.Name = "rbtnWS";
            this.rbtnWS.UseVisualStyleBackColor = true;
            // 
            // rbtnNoStream
            // 
            resources.ApplyResources(this.rbtnNoStream, "rbtnNoStream");
            this.rbtnNoStream.Checked = true;
            this.rbtnNoStream.Name = "rbtnNoStream";
            this.rbtnNoStream.TabStop = true;
            this.rbtnNoStream.UseVisualStyleBackColor = true;
            // 
            // tboxAlias
            // 
            resources.ApplyResources(this.tboxAlias, "tboxAlias");
            this.tboxAlias.Name = "tboxAlias";
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
            // 
            // btnOK
            // 
            resources.ApplyResources(this.btnOK, "btnOK");
            this.btnOK.Name = "btnOK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnExit
            // 
            resources.ApplyResources(this.btnExit, "btnExit");
            this.btnExit.Name = "btnExit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // linkConfigEditor
            // 
            resources.ApplyResources(this.linkConfigEditor, "linkConfigEditor");
            this.linkConfigEditor.Name = "linkConfigEditor";
            this.linkConfigEditor.TabStop = true;
            this.linkConfigEditor.UseCompatibleTextRendering = true;
            this.linkConfigEditor.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkConfigEditor_LinkClicked);
            // 
            // FormSimAddVmessClient
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.linkConfigEditor);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.tboxAlias);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormSimAddVmessClient";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox tboxUID;
        private System.Windows.Forms.TextBox tboxHost;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.ComboBox cboxKCP;
        private System.Windows.Forms.RadioButton rbtnKCP;
        private System.Windows.Forms.RadioButton rbtnTCP;
        private System.Windows.Forms.TextBox tboxWSPath;
        private System.Windows.Forms.RadioButton rbtnWS;
        private System.Windows.Forms.RadioButton rbtnNoStream;
        private System.Windows.Forms.CheckBox chkboxTLS;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox tboxAlias;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.TextBox tboxAID;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox tboxPort;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.LinkLabel linkConfigEditor;
        private System.Windows.Forms.Button btnGenUserID;
    }
}