namespace PhotoTournament
{
    partial class NewTournamentDialog
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.txtSourceDir = new System.Windows.Forms.TextBox();
            this.btnBrowseSourceDir = new System.Windows.Forms.Button();
            this.spnPicks = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.spnPicks)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(84, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Source &directory";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(11, 47);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(84, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Number of &picks";
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(324, 71);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 3;
            this.btnOK.Text = "&OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(405, 71);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(80, 23);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "&Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // txtSourceDir
            // 
            this.txtSourceDir.Location = new System.Drawing.Point(105, 12);
            this.txtSourceDir.Name = "txtSourceDir";
            this.txtSourceDir.Size = new System.Drawing.Size(294, 20);
            this.txtSourceDir.TabIndex = 0;
            // 
            // btnBrowseSourceDir
            // 
            this.btnBrowseSourceDir.Location = new System.Drawing.Point(405, 11);
            this.btnBrowseSourceDir.Name = "btnBrowseSourceDir";
            this.btnBrowseSourceDir.Size = new System.Drawing.Size(80, 21);
            this.btnBrowseSourceDir.TabIndex = 1;
            this.btnBrowseSourceDir.Text = "&Browse";
            this.btnBrowseSourceDir.UseVisualStyleBackColor = true;
            this.btnBrowseSourceDir.Click += new System.EventHandler(this.btnBrowseSourceDir_Click);
            // 
            // spnPicks
            // 
            this.spnPicks.Location = new System.Drawing.Point(105, 45);
            this.spnPicks.Name = "spnPicks";
            this.spnPicks.Size = new System.Drawing.Size(73, 20);
            this.spnPicks.TabIndex = 2;
            this.spnPicks.Value = new decimal(new int[] {
            24,
            0,
            0,
            0});
            // 
            // NewTournamentDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(497, 109);
            this.Controls.Add(this.spnPicks);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnBrowseSourceDir);
            this.Controls.Add(this.txtSourceDir);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "NewTournamentDialog";
            this.Text = "New tournament";
            this.Shown += new System.EventHandler(this.NewTournamentDialog_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.spnPicks)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.TextBox txtSourceDir;
        private System.Windows.Forms.Button btnBrowseSourceDir;
        private System.Windows.Forms.NumericUpDown spnPicks;
    }
}