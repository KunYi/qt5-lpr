using CsRGBshow;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CsMotionDetect
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        byte[,] B1, B2; //連續影像的灰階陣列
        int Type = 0; //顯示模式：0→原圖，1→灰階，2→二值化
        Form2 F2 = new Form2(); //監看範圍設定框
        FastPixel f = new FastPixel(); //快速繪圖物件

        //啟動程式顯示監看範圍設定框
        private void Form1_Load(object sender, EventArgs e)
        {
            F2.Show();
        }
        //啟動監看
        private void CopyScreentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            F2.Hide();
            timer1.Start();
        }
        //灰階差異
        private void DifferenceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Type = 1;
        }
        //灰階動態圖
        private Bitmap GrayDiff()
        {
            byte[,] D = new byte[f.nx, f.ny];
            for (int i = 0; i < f.nx; i++)
            {
                for (int j = 0; j < f.ny; j++)
                {
                    D[i, j] = (byte)(255-Math.Abs((int)B1[i, j] - B2[i, j]));
                }
            }
            return f.GrayImg(D);
        }
        //高差異點二值化
        private void BinaryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Type = 2;
        }
        //二值化動態圖
        private Bitmap BinDiff()
        {
            int Th = 10;
            byte[,] D = new byte[f.nx, f.ny];
            for (int i = 0; i < f.nx; i++)
            {
                for (int j = 0; j < f.ny; j++)
                {
                    int dd=Math.Abs((int)B1[i,j]-B2[i,j]);
                    if(dd>Th) D[i, j] = 1;
                }
            }
            return f.BWImg(D);
        }
        //動態監看迴圈
        private void Timer1_Tick(object sender, EventArgs e)
        {
            Bitmap bmp = new Bitmap(F2.Width, F2.Height);
            Graphics G = Graphics.FromImage(bmp);
            G.CopyFromScreen(F2.Location, new Point(0, 0), F2.Size);
            f.Bmp2RGB(bmp);
            if (B1 == null)
            {
                B1 = f.Gv; return;
            }
            else
            {
                B1 = B2;
            }
            B2 = f.Gv;
            switch (Type)
            {
                case 0:
                    pictureBox1.Image = bmp; //原圖顯示
                    break;
                case 1:
                    pictureBox1.Image = GrayDiff(); //差異灰階顯示
                    break;
                case 2:
                    pictureBox1.Image = BinDiff(); //差異二值化顯示
                    break;
            }
        }
    }
}
