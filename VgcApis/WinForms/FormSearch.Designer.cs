namespace VgcApis.WinForms
{
    partial class FormSearch
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormSearch));
            this.btnNext = new System.Windows.Forms.Button();
            this.btnPrevious = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.btnReplaceOne = new System.Windows.Forms.Button();
            this.btnReplaceAll = new System.Windows.Forms.Button();
            this.cboxReplaceKeyword = new System.Windows.Forms.ComboBox();
            this.cboxSearchKeyword = new System.Windows.Forms.ComboBox();
            this.groupBoxOptions = new System.Windows.Forms.GroupBox();
            this.chkOptionWordStart = new System.Windows.Forms.CheckBox();
            this.chkOptionMatchCase = new System.Windows.Forms.CheckBox();
            this.chkOptionWholeWord = new System.Windows.Forms.CheckBox();
            this.chkOptionRegex = new System.Windows.Forms.CheckBox();
            this.btnNewSearch = new System.Windows.Forms.Button();
            this.groupBoxOptions.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnNext
            // 
            resources.ApplyResources(this.btnNext, "btnNext");
            this.btnNext.Name = "btnNext";
            this.btnNext.UseVisualStyleBackColor = true;
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // btnPrevious
            // 
            resources.ApplyResources(this.btnPrevious, "btnPrevious");
            this.btnPrevious.Name = "btnPrevious";
            this.btnPrevious.UseVisualStyleBackColor = true;
            this.btnPrevious.Click += new System.EventHandler(this.btnPrevious_Click);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // btnReplaceOne
            // 
            resources.ApplyResources(this.btnReplaceOne, "btnReplaceOne");
            this.btnReplaceOne.Name = "btnReplaceOne";
            this.btnReplaceOne.UseVisualStyleBackColor = true;
            this.btnReplaceOne.Click += new System.EventHandler(this.btnReplaceOne_Click);
            // 
            // btnReplaceAll
            // 
            resources.ApplyResources(this.btnReplaceAll, "btnReplaceAll");
            this.btnReplaceAll.Name = "btnReplaceAll";
            this.btnReplaceAll.UseVisualStyleBackColor = true;
            this.btnReplaceAll.Click += new System.EventHandler(this.btnReplaceAll_Click);
            // 
            // cboxReplaceKeyword
            // 
            resources.ApplyResources(this.cboxReplaceKeyword, "cboxReplaceKeyword");
            this.cboxReplaceKeyword.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Append;
            this.cboxReplaceKeyword.FormattingEnabled = true;
            this.cboxReplaceKeyword.Name = "cboxReplaceKeyword";
            // 
            // cboxSearchKeyword
            // 
            resources.ApplyResources(this.cboxSearchKeyword, "cboxSearchKeyword");
            this.cboxSearchKeyword.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cboxSearchKeyword.FormattingEnabled = true;
            this.cboxSearchKeyword.Name = "cboxSearchKeyword";
            this.cboxSearchKeyword.TextChanged += new System.EventHandler(this.cboxSearchKeyword_TextChanged);
            // 
            // groupBoxOptions
            // 
            resources.ApplyResources(this.groupBoxOptions, "groupBoxOptions");
            this.groupBoxOptions.Controls.Add(this.chkOptionWordStart);
            this.groupBoxOptions.Controls.Add(this.chkOptionMatchCase);
            this.groupBoxOptions.Controls.Add(this.chkOptionWholeWord);
            this.groupBoxOptions.Controls.Add(this.chkOptionRegex);
            this.groupBoxOptions.Name = "groupBoxOptions";
            this.groupBoxOptions.TabStop = false;
            // 
            // chkOptionWordStart
            // 
            resources.ApplyResources(this.chkOptionWordStart, "chkOptionWordStart");
            this.chkOptionWordStart.Name = "chkOptionWordStart";
            this.chkOptionWordStart.UseVisualStyleBackColor = true;
            this.chkOptionWordStart.CheckedChanged += new System.EventHandler(this.chkOptionWordStart_CheckedChanged);
            // 
            // chkOptionMatchCase
            // 
            resources.ApplyResources(this.chkOptionMatchCase, "chkOptionMatchCase");
            this.chkOptionMatchCase.Name = "chkOptionMatchCase";
            this.chkOptionMatchCase.UseVisualStyleBackColor = true;
            this.chkOptionMatchCase.CheckedChanged += new System.EventHandler(this.chkOptionMatchCase_CheckedChanged);
            // 
            // chkOptionWholeWord
            // 
            resources.ApplyResources(this.chkOptionWholeWord, "chkOptionWholeWord");
            this.chkOptionWholeWord.Name = "chkOptionWholeWord";
            this.chkOptionWholeWord.UseVisualStyleBackColor = true;
            this.chkOptionWholeWord.CheckedChanged += new System.EventHandler(this.chkOptionWholeWord_CheckedChanged);
            // 
            // chkOptionRegex
            // 
            resources.ApplyResources(this.chkOptionRegex, "chkOptionRegex");
            this.chkOptionRegex.Name = "chkOptionRegex";
            this.chkOptionRegex.UseVisualStyleBackColor = true;
            this.chkOptionRegex.CheckedChanged += new System.EventHandler(this.chkOptionRegex_CheckedChanged);
            // 
            // btnNewSearch
            // 
            resources.ApplyResources(this.btnNewSearch, "btnNewSearch");
            this.btnNewSearch.Name = "btnNewSearch";
            this.btnNewSearch.UseVisualStyleBackColor = true;
            this.btnNewSearch.Click += new System.EventHandler(this.btnNewSearch_Click);
            // 
            // FormSearch
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBoxOptions);
            this.Controls.Add(this.cboxSearchKeyword);
            this.Controls.Add(this.cboxReplaceKeyword);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnPrevious);
            this.Controls.Add(this.btnReplaceAll);
            this.Controls.Add(this.btnReplaceOne);
            this.Controls.Add(this.btnNewSearch);
            this.Controls.Add(this.btnNext);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormSearch";
            this.groupBoxOptions.ResumeLayout(false);
            this.groupBoxOptions.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btnNext;
        private System.Windows.Forms.Button btnPrevious;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnReplaceOne;
        private System.Windows.Forms.Button btnReplaceAll;
        private System.Windows.Forms.ComboBox cboxReplaceKeyword;
        private System.Windows.Forms.ComboBox cboxSearchKeyword;
        private System.Windows.Forms.GroupBox groupBoxOptions;
        private System.Windows.Forms.Button btnNewSearch;
        private System.Windows.Forms.CheckBox chkOptionWordStart;
        private System.Windows.Forms.CheckBox chkOptionMatchCase;
        private System.Windows.Forms.CheckBox chkOptionWholeWord;
        private System.Windows.Forms.CheckBox chkOptionRegex;
    }
}
