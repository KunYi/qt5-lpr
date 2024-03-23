namespace CsRGBshow
{
    partial class Form1
    {
        /// <summary>
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置受控資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 設計工具產生的程式碼

        /// <summary>
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器修改
        /// 這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.SaveImageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.OpenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.AlignToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.RotateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.NormalizeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.CorrectAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.AddDashToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.ContextMenuStrip = this.contextMenuStrip1;
            this.pictureBox1.Location = new System.Drawing.Point(0, 28);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(640, 400);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox1.TabIndex = 5;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PictureBox1_MouseDown);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.SaveImageToolStripMenuItem});
            this.contextMenuStrip1.Name = "ContextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(141, 26);
            // 
            // SaveImageToolStripMenuItem
            // 
            this.SaveImageToolStripMenuItem.Name = "SaveImageToolStripMenuItem";
            this.SaveImageToolStripMenuItem.Size = new System.Drawing.Size(140, 22);
            this.SaveImageToolStripMenuItem.Text = "Save Image";
            this.SaveImageToolStripMenuItem.Click += new System.EventHandler(this.SaveImageToolStripMenuItem_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.OpenToolStripMenuItem,
            this.AlignToolStripMenuItem,
            this.RotateToolStripMenuItem,
            this.NormalizeToolStripMenuItem,
            this.CorrectAllToolStripMenuItem,
            this.AddDashToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(640, 24);
            this.menuStrip1.TabIndex = 4;
            this.menuStrip1.Text = "MenuStrip1";
            // 
            // OpenToolStripMenuItem
            // 
            this.OpenToolStripMenuItem.Name = "OpenToolStripMenuItem";
            this.OpenToolStripMenuItem.Size = new System.Drawing.Size(51, 20);
            this.OpenToolStripMenuItem.Text = "Open";
            this.OpenToolStripMenuItem.Click += new System.EventHandler(this.OpenToolStripMenuItem_Click);
            // 
            // AlignToolStripMenuItem
            // 
            this.AlignToolStripMenuItem.Name = "AlignToolStripMenuItem";
            this.AlignToolStripMenuItem.Size = new System.Drawing.Size(48, 20);
            this.AlignToolStripMenuItem.Text = "Align";
            this.AlignToolStripMenuItem.Click += new System.EventHandler(this.AlignToolStripMenuItem_Click);
            // 
            // RotateToolStripMenuItem
            // 
            this.RotateToolStripMenuItem.Name = "RotateToolStripMenuItem";
            this.RotateToolStripMenuItem.Size = new System.Drawing.Size(57, 20);
            this.RotateToolStripMenuItem.Text = "Rotate";
            this.RotateToolStripMenuItem.Click += new System.EventHandler(this.RotateToolStripMenuItem_Click);
            // 
            // NormalizeToolStripMenuItem
            // 
            this.NormalizeToolStripMenuItem.Name = "NormalizeToolStripMenuItem";
            this.NormalizeToolStripMenuItem.Size = new System.Drawing.Size(78, 20);
            this.NormalizeToolStripMenuItem.Text = "Normalize";
            this.NormalizeToolStripMenuItem.Click += new System.EventHandler(this.NormalizeToolStripMenuItem_Click);
            // 
            // CorrectAllToolStripMenuItem
            // 
            this.CorrectAllToolStripMenuItem.Name = "CorrectAllToolStripMenuItem";
            this.CorrectAllToolStripMenuItem.Size = new System.Drawing.Size(77, 20);
            this.CorrectAllToolStripMenuItem.Text = "Correct All";
            this.CorrectAllToolStripMenuItem.Click += new System.EventHandler(this.CorrectAllToolStripMenuItem_Click);
            // 
            // AddDashToolStripMenuItem
            // 
            this.AddDashToolStripMenuItem.Name = "AddDashToolStripMenuItem";
            this.AddDashToolStripMenuItem.Size = new System.Drawing.Size(74, 20);
            this.AddDashToolStripMenuItem.Text = "Add Dash";
            this.AddDashToolStripMenuItem.Click += new System.EventHandler(this.AddDashToolStripMenuItem_Click);
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.Filter = "*.png|*.png";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(640, 428);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "Target Normalize";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        internal System.Windows.Forms.PictureBox pictureBox1;
        internal System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        internal System.Windows.Forms.ToolStripMenuItem SaveImageToolStripMenuItem;
        internal System.Windows.Forms.MenuStrip menuStrip1;
        internal System.Windows.Forms.ToolStripMenuItem OpenToolStripMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem AlignToolStripMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem RotateToolStripMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem NormalizeToolStripMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem CorrectAllToolStripMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem AddDashToolStripMenuItem;
        internal System.Windows.Forms.OpenFileDialog openFileDialog1;
        internal System.Windows.Forms.SaveFileDialog saveFileDialog1;
    }
}

