namespace CsMotionDetect
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.CopyScreentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.DifferenceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.BinaryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.CopyScreentToolStripMenuItem,
            this.DifferenceToolStripMenuItem,
            this.BinaryToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(640, 24);
            this.menuStrip1.TabIndex = 5;
            this.menuStrip1.Text = "MenuStrip1";
            // 
            // CopyScreentToolStripMenuItem
            // 
            this.CopyScreentToolStripMenuItem.Name = "CopyScreentToolStripMenuItem";
            this.CopyScreentToolStripMenuItem.Size = new System.Drawing.Size(87, 20);
            this.CopyScreentToolStripMenuItem.Text = "CopyScreen";
            this.CopyScreentToolStripMenuItem.Click += new System.EventHandler(this.CopyScreentToolStripMenuItem_Click);
            // 
            // DifferenceToolStripMenuItem
            // 
            this.DifferenceToolStripMenuItem.Name = "DifferenceToolStripMenuItem";
            this.DifferenceToolStripMenuItem.Size = new System.Drawing.Size(77, 20);
            this.DifferenceToolStripMenuItem.Text = "Difference";
            this.DifferenceToolStripMenuItem.Click += new System.EventHandler(this.DifferenceToolStripMenuItem_Click);
            // 
            // BinaryToolStripMenuItem
            // 
            this.BinaryToolStripMenuItem.Name = "BinaryToolStripMenuItem";
            this.BinaryToolStripMenuItem.Size = new System.Drawing.Size(53, 20);
            this.BinaryToolStripMenuItem.Text = "Binary";
            this.BinaryToolStripMenuItem.Click += new System.EventHandler(this.BinaryToolStripMenuItem_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(0, 28);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(640, 400);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 6;
            this.pictureBox1.TabStop = false;
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.Timer1_Tick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(640, 428);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.pictureBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "Motion Detection";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        internal System.Windows.Forms.MenuStrip menuStrip1;
        internal System.Windows.Forms.ToolStripMenuItem CopyScreentToolStripMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem DifferenceToolStripMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem BinaryToolStripMenuItem;
        internal System.Windows.Forms.PictureBox pictureBox1;
        internal System.Windows.Forms.Timer timer1;
    }
}

