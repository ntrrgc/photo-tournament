﻿namespace CompetititiveCullingAlgorithm
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
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.imgPhotoA = new System.Windows.Forms.PictureBox();
            this.imgPhotoB = new System.Windows.Forms.PictureBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.btnChooseA = new System.Windows.Forms.Button();
            this.btnChooseB = new System.Windows.Forms.Button();
            this.statusStrip1.SuspendLayout();
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
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(922, 25);
            this.toolStrip1.TabIndex = 2;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // imgPhotoA
            // 
            this.imgPhotoA.Dock = System.Windows.Forms.DockStyle.Fill;
            this.imgPhotoA.Location = new System.Drawing.Point(3, 23);
            this.imgPhotoA.Name = "imgPhotoA";
            this.imgPhotoA.Size = new System.Drawing.Size(455, 519);
            this.imgPhotoA.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.imgPhotoA.TabIndex = 3;
            this.imgPhotoA.TabStop = false;
            // 
            // imgPhotoB
            // 
            this.imgPhotoB.Dock = System.Windows.Forms.DockStyle.Fill;
            this.imgPhotoB.Location = new System.Drawing.Point(464, 23);
            this.imgPhotoB.Name = "imgPhotoB";
            this.imgPhotoB.Size = new System.Drawing.Size(455, 519);
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
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnChooseA, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.btnChooseB, 1, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 25);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(922, 585);
            this.tableLayoutPanel1.TabIndex = 5;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.label1, 2);
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(916, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "Which picture fits better in the album?";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnChooseA
            // 
            this.btnChooseA.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnChooseA.Location = new System.Drawing.Point(3, 548);
            this.btnChooseA.Name = "btnChooseA";
            this.btnChooseA.Size = new System.Drawing.Size(455, 34);
            this.btnChooseA.TabIndex = 6;
            this.btnChooseA.TabStop = false;
            this.btnChooseA.Text = "The picture on the &left is better";
            this.btnChooseA.UseVisualStyleBackColor = true;
            this.btnChooseA.Click += new System.EventHandler(this.btnChooseA_Click);
            // 
            // btnChooseB
            // 
            this.btnChooseB.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnChooseB.Location = new System.Drawing.Point(464, 548);
            this.btnChooseB.Name = "btnChooseB";
            this.btnChooseB.Size = new System.Drawing.Size(455, 34);
            this.btnChooseB.TabIndex = 7;
            this.btnChooseB.TabStop = false;
            this.btnChooseB.Text = "The picture on the &right is better";
            this.btnChooseB.UseVisualStyleBackColor = true;
            this.btnChooseB.Click += new System.EventHandler(this.btnChooseB_Click);
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(922, 632);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.statusStrip1);
            this.KeyPreview = true;
            this.Name = "MainWindow";
            this.Text = "Photo Tournament";
            this.Load += new System.EventHandler(this.MainWindow_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MainWindow_KeyDown);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
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
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnChooseA;
        private System.Windows.Forms.Button btnChooseB;
    }
}