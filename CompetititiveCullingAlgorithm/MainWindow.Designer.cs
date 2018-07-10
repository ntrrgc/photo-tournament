namespace TournamentSort
{
    partial class MainWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnNewTournament = new System.Windows.Forms.ToolStripButton();
            this.btnLoad = new System.Windows.Forms.ToolStripButton();
            this.btnSave = new System.Windows.Forms.ToolStripButton();
            this.btnSaveAs = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.btnUndo = new System.Windows.Forms.ToolStripButton();
            this.btnRedo = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.btnExportByRank = new System.Windows.Forms.ToolStripButton();
            this.btnExportUnsorted = new System.Windows.Forms.ToolStripButton();
            this.imgPhotoA = new System.Windows.Forms.PictureBox();
            this.imgPhotoB = new System.Windows.Forms.PictureBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.lblHint = new System.Windows.Forms.Label();
            this.btnChooseA = new System.Windows.Forms.Button();
            this.btnChooseB = new System.Windows.Forms.Button();
            this.pgrNextWinner = new System.Windows.Forms.ProgressBar();
            this.pgrGlobal = new System.Windows.Forms.ProgressBar();
            this.statusStrip1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.imgPhotoA)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgPhotoB)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblStatus});
            this.statusStrip1.Location = new System.Drawing.Point(0, 610);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(922, 22);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // lblStatus
            // 
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(118, 17);
            this.lblStatus.Text = "toolStripStatusLabel1";
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnNewTournament,
            this.btnLoad,
            this.btnSave,
            this.btnSaveAs,
            this.toolStripSeparator1,
            this.btnUndo,
            this.btnRedo,
            this.toolStripSeparator2,
            this.btnExportByRank,
            this.btnExportUnsorted});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(922, 25);
            this.toolStrip1.TabIndex = 2;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // btnNewTournament
            // 
            this.btnNewTournament.Image = ((System.Drawing.Image)(resources.GetObject("btnNewTournament.Image")));
            this.btnNewTournament.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnNewTournament.Name = "btnNewTournament";
            this.btnNewTournament.Size = new System.Drawing.Size(117, 22);
            this.btnNewTournament.Text = "New tournament";
            this.btnNewTournament.Click += new System.EventHandler(this.btnNewTournament_Click_1);
            // 
            // btnLoad
            // 
            this.btnLoad.Image = ((System.Drawing.Image)(resources.GetObject("btnLoad.Image")));
            this.btnLoad.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(119, 22);
            this.btnLoad.Text = "Load tournament";
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // btnSave
            // 
            this.btnSave.Image = ((System.Drawing.Image)(resources.GetObject("btnSave.Image")));
            this.btnSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(117, 22);
            this.btnSave.Text = "Save tournament";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnSaveAs
            // 
            this.btnSaveAs.Image = ((System.Drawing.Image)(resources.GetObject("btnSaveAs.Image")));
            this.btnSaveAs.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSaveAs.Name = "btnSaveAs";
            this.btnSaveAs.Size = new System.Drawing.Size(65, 22);
            this.btnSaveAs.Text = "Save as";
            this.btnSaveAs.Click += new System.EventHandler(this.btnSaveAs_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // btnUndo
            // 
            this.btnUndo.Image = ((System.Drawing.Image)(resources.GetObject("btnUndo.Image")));
            this.btnUndo.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnUndo.Name = "btnUndo";
            this.btnUndo.Size = new System.Drawing.Size(56, 22);
            this.btnUndo.Text = "Undo";
            this.btnUndo.Click += new System.EventHandler(this.btnUndo_Click);
            // 
            // btnRedo
            // 
            this.btnRedo.Image = ((System.Drawing.Image)(resources.GetObject("btnRedo.Image")));
            this.btnRedo.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnRedo.Name = "btnRedo";
            this.btnRedo.Size = new System.Drawing.Size(54, 22);
            this.btnRedo.Text = "Redo";
            this.btnRedo.Click += new System.EventHandler(this.btnRedo_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // btnExportByRank
            // 
            this.btnExportByRank.Image = ((System.Drawing.Image)(resources.GetObject("btnExportByRank.Image")));
            this.btnExportByRank.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnExportByRank.Name = "btnExportByRank";
            this.btnExportByRank.Size = new System.Drawing.Size(138, 22);
            this.btnExportByRank.Text = "Export sorted by rank";
            this.btnExportByRank.Click += new System.EventHandler(this.btnExportByRank_Click);
            // 
            // btnExportUnsorted
            // 
            this.btnExportUnsorted.Image = ((System.Drawing.Image)(resources.GetObject("btnExportUnsorted.Image")));
            this.btnExportUnsorted.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnExportUnsorted.Name = "btnExportUnsorted";
            this.btnExportUnsorted.Size = new System.Drawing.Size(110, 22);
            this.btnExportUnsorted.Text = "Export unsorted";
            this.btnExportUnsorted.Click += new System.EventHandler(this.btnExportUnsorted_Click);
            // 
            // imgPhotoA
            // 
            this.imgPhotoA.Dock = System.Windows.Forms.DockStyle.Fill;
            this.imgPhotoA.Location = new System.Drawing.Point(3, 23);
            this.imgPhotoA.Name = "imgPhotoA";
            this.imgPhotoA.Size = new System.Drawing.Size(455, 479);
            this.imgPhotoA.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.imgPhotoA.TabIndex = 3;
            this.imgPhotoA.TabStop = false;
            // 
            // imgPhotoB
            // 
            this.imgPhotoB.Dock = System.Windows.Forms.DockStyle.Fill;
            this.imgPhotoB.Location = new System.Drawing.Point(464, 23);
            this.imgPhotoB.Name = "imgPhotoB";
            this.imgPhotoB.Size = new System.Drawing.Size(455, 479);
            this.imgPhotoB.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.imgPhotoB.TabIndex = 4;
            this.imgPhotoB.TabStop = false;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.imgPhotoA, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.imgPhotoB, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.lblHint, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnChooseA, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.btnChooseB, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.pgrNextWinner, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.pgrGlobal, 0, 4);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 25);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 5;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(922, 585);
            this.tableLayoutPanel1.TabIndex = 5;
            // 
            // lblHint
            // 
            this.lblHint.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.lblHint, 2);
            this.lblHint.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblHint.Location = new System.Drawing.Point(3, 0);
            this.lblHint.Name = "lblHint";
            this.lblHint.Size = new System.Drawing.Size(916, 20);
            this.lblHint.TabIndex = 0;
            this.lblHint.Text = "Which picture fits better in the album?";
            this.lblHint.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnChooseA
            // 
            this.btnChooseA.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnChooseA.Location = new System.Drawing.Point(3, 508);
            this.btnChooseA.Name = "btnChooseA";
            this.btnChooseA.Size = new System.Drawing.Size(455, 34);
            this.btnChooseA.TabIndex = 6;
            this.btnChooseA.TabStop = false;
            this.btnChooseA.Text = "This picture is better (1)";
            this.btnChooseA.UseVisualStyleBackColor = true;
            this.btnChooseA.Click += new System.EventHandler(this.btnChooseA_Click);
            // 
            // btnChooseB
            // 
            this.btnChooseB.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnChooseB.Location = new System.Drawing.Point(464, 508);
            this.btnChooseB.Name = "btnChooseB";
            this.btnChooseB.Size = new System.Drawing.Size(455, 34);
            this.btnChooseB.TabIndex = 7;
            this.btnChooseB.TabStop = false;
            this.btnChooseB.Text = "This picture is better (2)";
            this.btnChooseB.UseVisualStyleBackColor = true;
            this.btnChooseB.Click += new System.EventHandler(this.btnChooseB_Click);
            // 
            // pgrNextWinner
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.pgrNextWinner, 2);
            this.pgrNextWinner.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pgrNextWinner.Location = new System.Drawing.Point(3, 548);
            this.pgrNextWinner.Name = "pgrNextWinner";
            this.pgrNextWinner.Size = new System.Drawing.Size(916, 14);
            this.pgrNextWinner.TabIndex = 8;
            // 
            // pgrGlobal
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.pgrGlobal, 2);
            this.pgrGlobal.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pgrGlobal.Location = new System.Drawing.Point(3, 568);
            this.pgrGlobal.Name = "pgrGlobal";
            this.pgrGlobal.Size = new System.Drawing.Size(916, 14);
            this.pgrGlobal.TabIndex = 9;
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(922, 632);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.statusStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Name = "MainWindow";
            this.Text = "Photo Tournament";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainWindow_FormClosing);
            this.Load += new System.EventHandler(this.MainWindow_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MainWindow_KeyDown);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.imgPhotoA)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgPhotoB)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel lblStatus;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.PictureBox imgPhotoA;
        private System.Windows.Forms.PictureBox imgPhotoB;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label lblHint;
        private System.Windows.Forms.Button btnChooseA;
        private System.Windows.Forms.Button btnChooseB;
        private System.Windows.Forms.ToolStripButton btnLoad;
        private System.Windows.Forms.ToolStripButton btnSave;
        private System.Windows.Forms.ProgressBar pgrNextWinner;
        private System.Windows.Forms.ProgressBar pgrGlobal;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton btnUndo;
        private System.Windows.Forms.ToolStripButton btnRedo;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton btnExportByRank;
        private System.Windows.Forms.ToolStripButton btnExportUnsorted;
        private System.Windows.Forms.ToolStripButton btnNewTournament;
        private System.Windows.Forms.ToolStripButton btnSaveAs;
    }
}