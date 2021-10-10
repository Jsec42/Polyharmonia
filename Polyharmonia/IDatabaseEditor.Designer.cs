namespace Polyharmonia
{
    partial class IDatabaseEditor
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnMoveUp = new System.Windows.Forms.Button();
            this.btnMoveDown = new System.Windows.Forms.Button();
            this.lstInstruments = new System.Windows.Forms.ListBox();
            this.txtIName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.numIIndex = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.lstBankSelect = new System.Windows.Forms.ListBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtBName = new System.Windows.Forms.TextBox();
            this.numBIndex = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.btnAddBank = new System.Windows.Forms.Button();
            this.btnAddInst = new System.Windows.Forms.Button();
            this.btnBMoveUp = new System.Windows.Forms.Button();
            this.btnBMoveDown = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.btnDeleteInstrument = new System.Windows.Forms.Button();
            this.btnDeleteBank = new System.Windows.Forms.Button();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numIIndex)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numBIndex)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(486, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.openToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.newToolStripMenuItem.Text = "New";
            this.newToolStripMenuItem.Click += new System.EventHandler(this.newToolStripMenuItem_Click);
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.openToolStripMenuItem.Text = "Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            // 
            // btnMoveUp
            // 
            this.btnMoveUp.Location = new System.Drawing.Point(12, 311);
            this.btnMoveUp.Name = "btnMoveUp";
            this.btnMoveUp.Size = new System.Drawing.Size(70, 27);
            this.btnMoveUp.TabIndex = 1;
            this.btnMoveUp.TabStop = false;
            this.btnMoveUp.Text = "Move ↑";
            this.btnMoveUp.UseVisualStyleBackColor = true;
            this.btnMoveUp.Click += new System.EventHandler(this.btnMoveUp_Click);
            // 
            // btnMoveDown
            // 
            this.btnMoveDown.Location = new System.Drawing.Point(162, 311);
            this.btnMoveDown.Name = "btnMoveDown";
            this.btnMoveDown.Size = new System.Drawing.Size(70, 27);
            this.btnMoveDown.TabIndex = 2;
            this.btnMoveDown.TabStop = false;
            this.btnMoveDown.Text = "Move ↓";
            this.btnMoveDown.UseVisualStyleBackColor = true;
            this.btnMoveDown.Click += new System.EventHandler(this.btnMoveDown_Click);
            // 
            // lstInstruments
            // 
            this.lstInstruments.FormattingEnabled = true;
            this.lstInstruments.Items.AddRange(new object[] {
            "(new)"});
            this.lstInstruments.Location = new System.Drawing.Point(12, 37);
            this.lstInstruments.Name = "lstInstruments";
            this.lstInstruments.Size = new System.Drawing.Size(220, 264);
            this.lstInstruments.TabIndex = 0;
            this.lstInstruments.SelectedIndexChanged += new System.EventHandler(this.lstInstruments_SelectedIndexChanged);
            this.lstInstruments.KeyDown += new System.Windows.Forms.KeyEventHandler(this.lstInstruments_KeyDown);
            this.lstInstruments.KeyUp += new System.Windows.Forms.KeyEventHandler(this.lstInstruments_KeyUp);
            this.lstInstruments.Leave += new System.EventHandler(this.lstInstruments_Leave);
            // 
            // txtIName
            // 
            this.txtIName.Location = new System.Drawing.Point(238, 53);
            this.txtIName.Name = "txtIName";
            this.txtIName.Size = new System.Drawing.Size(173, 20);
            this.txtIName.TabIndex = 1;
            this.txtIName.Click += new System.EventHandler(this.txtIName_Click);
            this.txtIName.Enter += new System.EventHandler(this.txtIName_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(235, 37);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(87, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Instrument Name";
            // 
            // numIIndex
            // 
            this.numIIndex.Location = new System.Drawing.Point(417, 53);
            this.numIIndex.Maximum = new decimal(new int[] {
            127,
            0,
            0,
            0});
            this.numIIndex.Name = "numIIndex";
            this.numIIndex.Size = new System.Drawing.Size(57, 20);
            this.numIIndex.TabIndex = 2;
            this.numIIndex.Enter += new System.EventHandler(this.txtIName_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(414, 37);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(33, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Index";
            // 
            // lstBankSelect
            // 
            this.lstBankSelect.FormattingEnabled = true;
            this.lstBankSelect.Items.AddRange(new object[] {
            "(new)"});
            this.lstBankSelect.Location = new System.Drawing.Point(238, 92);
            this.lstBankSelect.Name = "lstBankSelect";
            this.lstBankSelect.Size = new System.Drawing.Size(236, 134);
            this.lstBankSelect.TabIndex = 3;
            this.lstBankSelect.SelectedIndexChanged += new System.EventHandler(this.lstBankSelect_SelectedIndexChanged);
            this.lstBankSelect.KeyDown += new System.Windows.Forms.KeyEventHandler(this.lstBankSelect_KeyDown);
            this.lstBankSelect.KeyUp += new System.Windows.Forms.KeyEventHandler(this.lstBankSelect_KeyUp);
            this.lstBankSelect.Leave += new System.EventHandler(this.lstBankSelect_Leave);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(238, 76);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(32, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "Bank";
            // 
            // txtBName
            // 
            this.txtBName.Location = new System.Drawing.Point(238, 284);
            this.txtBName.Name = "txtBName";
            this.txtBName.Size = new System.Drawing.Size(173, 20);
            this.txtBName.TabIndex = 4;
            this.txtBName.Enter += new System.EventHandler(this.txtBName_Click);
            // 
            // numBIndex
            // 
            this.numBIndex.Location = new System.Drawing.Point(417, 285);
            this.numBIndex.Maximum = new decimal(new int[] {
            127,
            0,
            0,
            0});
            this.numBIndex.Name = "numBIndex";
            this.numBIndex.Size = new System.Drawing.Size(57, 20);
            this.numBIndex.TabIndex = 5;
            this.numBIndex.Enter += new System.EventHandler(this.txtBName_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(238, 268);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(63, 13);
            this.label4.TabIndex = 12;
            this.label4.Text = "Bank Name";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(414, 268);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(33, 13);
            this.label5.TabIndex = 13;
            this.label5.Text = "Index";
            // 
            // btnAddBank
            // 
            this.btnAddBank.Location = new System.Drawing.Point(238, 311);
            this.btnAddBank.Name = "btnAddBank";
            this.btnAddBank.Size = new System.Drawing.Size(99, 27);
            this.btnAddBank.TabIndex = 6;
            this.btnAddBank.Text = "Add Bank";
            this.btnAddBank.UseVisualStyleBackColor = true;
            this.btnAddBank.Click += new System.EventHandler(this.btnAddBank_Click);
            // 
            // btnAddInst
            // 
            this.btnAddInst.Location = new System.Drawing.Point(375, 311);
            this.btnAddInst.Name = "btnAddInst";
            this.btnAddInst.Size = new System.Drawing.Size(99, 27);
            this.btnAddInst.TabIndex = 7;
            this.btnAddInst.Text = "Add Instrument";
            this.btnAddInst.UseVisualStyleBackColor = true;
            this.btnAddInst.Click += new System.EventHandler(this.btnAddInst_Click);
            // 
            // btnBMoveUp
            // 
            this.btnBMoveUp.Location = new System.Drawing.Point(238, 238);
            this.btnBMoveUp.Name = "btnBMoveUp";
            this.btnBMoveUp.Size = new System.Drawing.Size(70, 27);
            this.btnBMoveUp.TabIndex = 16;
            this.btnBMoveUp.TabStop = false;
            this.btnBMoveUp.Text = "Move ↑";
            this.btnBMoveUp.UseVisualStyleBackColor = true;
            this.btnBMoveUp.Click += new System.EventHandler(this.btnBMoveUp_Click);
            // 
            // btnBMoveDown
            // 
            this.btnBMoveDown.Location = new System.Drawing.Point(404, 238);
            this.btnBMoveDown.Name = "btnBMoveDown";
            this.btnBMoveDown.Size = new System.Drawing.Size(70, 27);
            this.btnBMoveDown.TabIndex = 17;
            this.btnBMoveDown.TabStop = false;
            this.btnBMoveDown.Text = "Move ↓";
            this.btnBMoveDown.UseVisualStyleBackColor = true;
            this.btnBMoveDown.Click += new System.EventHandler(this.btnBMoveDown_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.Filter = "JSON Object|*.json|All Files|*.*";
            this.openFileDialog1.FileOk += new System.ComponentModel.CancelEventHandler(this.openFileDialog1_FileOk);
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.Filter = "JSON Object|*.json|All Files|*.*";
            this.saveFileDialog1.FileOk += new System.ComponentModel.CancelEventHandler(this.saveFileDialog1_FileOk);
            // 
            // btnDeleteInstrument
            // 
            this.btnDeleteInstrument.Location = new System.Drawing.Point(88, 311);
            this.btnDeleteInstrument.Name = "btnDeleteInstrument";
            this.btnDeleteInstrument.Size = new System.Drawing.Size(68, 26);
            this.btnDeleteInstrument.TabIndex = 18;
            this.btnDeleteInstrument.TabStop = false;
            this.btnDeleteInstrument.Text = "Delete";
            this.btnDeleteInstrument.UseVisualStyleBackColor = true;
            this.btnDeleteInstrument.Click += new System.EventHandler(this.btnDeleteInstrument_Click);
            // 
            // btnDeleteBank
            // 
            this.btnDeleteBank.Location = new System.Drawing.Point(314, 239);
            this.btnDeleteBank.Name = "btnDeleteBank";
            this.btnDeleteBank.Size = new System.Drawing.Size(84, 26);
            this.btnDeleteBank.TabIndex = 19;
            this.btnDeleteBank.TabStop = false;
            this.btnDeleteBank.Text = "Delete";
            this.btnDeleteBank.UseVisualStyleBackColor = true;
            this.btnDeleteBank.Click += new System.EventHandler(this.btnDeleteBank_Click);
            // 
            // IDatabaseEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(486, 349);
            this.Controls.Add(this.btnDeleteBank);
            this.Controls.Add(this.btnDeleteInstrument);
            this.Controls.Add(this.btnBMoveDown);
            this.Controls.Add(this.btnBMoveUp);
            this.Controls.Add(this.btnAddInst);
            this.Controls.Add(this.btnAddBank);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.numBIndex);
            this.Controls.Add(this.txtBName);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.lstBankSelect);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.numIIndex);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtIName);
            this.Controls.Add(this.lstInstruments);
            this.Controls.Add(this.btnMoveDown);
            this.Controls.Add(this.btnMoveUp);
            this.Controls.Add(this.menuStrip1);
            this.KeyPreview = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "IDatabaseEditor";
            this.Text = "Instrument Database";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.IDatabaseEditor_FormClosing);
            this.Load += new System.EventHandler(this.IDatabaseEditor_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numIIndex)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numBIndex)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.Button btnMoveUp;
        private System.Windows.Forms.Button btnMoveDown;
        private System.Windows.Forms.ListBox lstInstruments;
        private System.Windows.Forms.TextBox txtIName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown numIIndex;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ListBox lstBankSelect;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtBName;
        private System.Windows.Forms.NumericUpDown numBIndex;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btnAddBank;
        private System.Windows.Forms.Button btnAddInst;
        private System.Windows.Forms.Button btnBMoveUp;
        private System.Windows.Forms.Button btnBMoveDown;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.Button btnDeleteInstrument;
        private System.Windows.Forms.Button btnDeleteBank;
    }
}