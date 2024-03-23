namespace CsSegm
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
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.SaveImageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.OpenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.LeafToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.RimToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.RimTgToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.LeafRimToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.contextMenuStrip1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
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
            this.LeafToolStripMenuItem,
            this.RimToolStripMenuItem,
            this.RimTgToolStripMenuItem,
            this.LeafRimToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(600, 24);
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
            // LeafToolStripMenuItem
            // 
            this.LeafToolStripMenuItem.Name = "LeafToolStripMenuItem";
            this.LeafToolStripMenuItem.Size = new System.Drawing.Size(43, 20);
            this.LeafToolStripMenuItem.Text = "Leaf";
            this.LeafToolStripMenuItem.Click += new System.EventHandler(this.LeafToolStripMenuItem_Click);
            // 
            // RimToolStripMenuItem
            // 
            this.RimToolStripMenuItem.Name = "RimToolStripMenuItem";
            this.RimToolStripMenuItem.Size = new System.Drawing.Size(41, 20);
            this.RimToolStripMenuItem.Text = "Rim";
            this.RimToolStripMenuItem.Click += new System.EventHandler(this.RimToolStripMenuItem_Click);
            // 
            // RimTgToolStripMenuItem
            // 
            this.RimTgToolStripMenuItem.Name = "RimTgToolStripMenuItem";
            this.RimTgToolStripMenuItem.Size = new System.Drawing.Size(59, 20);
            this.RimTgToolStripMenuItem.Text = "Rim Tg";
            this.RimTgToolStripMenuItem.Click += new System.EventHandler(this.RimTgToolStripMenuItem_Click);
            // 
            // LeafRimToolStripMenuItem
            // 
            this.LeafRimToolStripMenuItem.Name = "LeafRimToolStripMenuItem";
            this.LeafRimToolStripMenuItem.Size = new System.Drawing.Size(70, 20);
            this.LeafRimToolStripMenuItem.Text = "Leaf_Rim";
            this.LeafRimToolStripMenuItem.Click += new System.EventHandler(this.LeafRimToolStripMenuItem_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.ContextMenuStrip = this.contextMenuStrip1;
            this.pictureBox1.Location = new System.Drawing.Point(0, 28);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(600, 400);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox1.TabIndex = 5;
            this.pictureBox1.TabStop = false;
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.Filter = "*.png|*.png";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(600, 428);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.pictureBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "Leaf Area in Rim";
            this.contextMenuStrip1.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        internal System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        internal System.Windows.Forms.ToolStripMenuItem SaveImageToolStripMenuItem;
        internal System.Windows.Forms.MenuStrip menuStrip1;
        internal System.Windows.Forms.ToolStripMenuItem OpenToolStripMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem LeafToolStripMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem RimToolStripMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem RimTgToolStripMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem LeafRimToolStripMenuItem;
        internal System.Windows.Forms.PictureBox pictureBox1;
        internal System.Windows.Forms.OpenFileDialog openFileDialog1;
        internal System.Windows.Forms.SaveFileDialog saveFileDialog1;
    }
}

