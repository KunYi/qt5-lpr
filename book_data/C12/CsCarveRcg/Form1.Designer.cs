namespace CsCarveRcg
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
            this.IntegralToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.YBoundToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.BinaryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.TargetsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.MergeTgsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.BestFitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
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
            this.IntegralToolStripMenuItem,
            this.YBoundToolStripMenuItem,
            this.BinaryToolStripMenuItem,
            this.TargetsToolStripMenuItem,
            this.MergeTgsToolStripMenuItem,
            this.BestFitToolStripMenuItem});
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
            // IntegralToolStripMenuItem
            // 
            this.IntegralToolStripMenuItem.Name = "IntegralToolStripMenuItem";
            this.IntegralToolStripMenuItem.Size = new System.Drawing.Size(62, 20);
            this.IntegralToolStripMenuItem.Text = "Integral";
            this.IntegralToolStripMenuItem.Click += new System.EventHandler(this.IntegralToolStripMenuItem_Click);
            // 
            // YBoundToolStripMenuItem
            // 
            this.YBoundToolStripMenuItem.Name = "YBoundToolStripMenuItem";
            this.YBoundToolStripMenuItem.Size = new System.Drawing.Size(67, 20);
            this.YBoundToolStripMenuItem.Text = "Y bound";
            this.YBoundToolStripMenuItem.Click += new System.EventHandler(this.YBoundToolStripMenuItem_Click);
            // 
            // BinaryToolStripMenuItem
            // 
            this.BinaryToolStripMenuItem.Name = "BinaryToolStripMenuItem";
            this.BinaryToolStripMenuItem.Size = new System.Drawing.Size(53, 20);
            this.BinaryToolStripMenuItem.Text = "Binary";
            this.BinaryToolStripMenuItem.Click += new System.EventHandler(this.BinaryToolStripMenuItem_Click);
            // 
            // TargetsToolStripMenuItem
            // 
            this.TargetsToolStripMenuItem.Name = "TargetsToolStripMenuItem";
            this.TargetsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.TargetsToolStripMenuItem.Text = "Targets";
            this.TargetsToolStripMenuItem.Click += new System.EventHandler(this.TargetsToolStripMenuItem_Click);
            // 
            // MergeTgsToolStripMenuItem
            // 
            this.MergeTgsToolStripMenuItem.Name = "MergeTgsToolStripMenuItem";
            this.MergeTgsToolStripMenuItem.Size = new System.Drawing.Size(80, 20);
            this.MergeTgsToolStripMenuItem.Text = "Merge Tgs";
            this.MergeTgsToolStripMenuItem.Click += new System.EventHandler(this.MergeTgsToolStripMenuItem_Click);
            // 
            // BestFitToolStripMenuItem
            // 
            this.BestFitToolStripMenuItem.Name = "BestFitToolStripMenuItem";
            this.BestFitToolStripMenuItem.Size = new System.Drawing.Size(58, 20);
            this.BestFitToolStripMenuItem.Text = "Best Fit";
            this.BestFitToolStripMenuItem.Click += new System.EventHandler(this.BestFitToolStripMenuItem_Click);
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
            this.Text = "Carved Character Recognization";
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
        internal System.Windows.Forms.ToolStripMenuItem IntegralToolStripMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem YBoundToolStripMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem BinaryToolStripMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem TargetsToolStripMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem MergeTgsToolStripMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem BestFitToolStripMenuItem;
        internal System.Windows.Forms.OpenFileDialog openFileDialog1;
        internal System.Windows.Forms.SaveFileDialog saveFileDialog1;
    }
}

