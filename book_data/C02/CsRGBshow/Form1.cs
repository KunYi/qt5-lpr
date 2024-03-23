using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CsRGBshow
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        FastPixel f = new FastPixel(); //宣告快速繪圖物件

        //開啟檔案
        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Bitmap bmp = new Bitmap(openFileDialog1.FileName);
                f.Bmp2RGB(bmp); //讀取RGB亮度陣列
                pictureBox1.Image = bmp;
            }
        }

        //以紅光為灰階
        private void RedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pictureBox1.Image = f.GrayImg(f.Rv);
        }

        //以綠光為灰階
        private void GreenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pictureBox1.Image = f.GrayImg(f.Gv);
        }

        //以藍光為灰階
        private void BlueToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pictureBox1.Image = f.GrayImg(f.Bv);
        }

        //以RGB整合亮度為灰階
        private void RGBToolStripMenuItem_Click(object sender, EventArgs e)
        {
            byte[,] A = new byte[f.nx, f.ny];
            for (int j = 0; j < f.ny; j++)
            {
                for (int i = 0; i < f.nx; i++)
                {
                    byte gray = (byte)(f.Rv[i, j] * 0.299 + f.Gv[i, j] * 0.587 + f.Bv[i, j] * 0.114);
                    A[i, j] = gray;
                }
            }
            pictureBox1.Image = f.GrayImg(A);
        }

        //選擇紅綠光之較暗亮度為灰階
        private void RGLowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            byte[,] A = new byte[f.nx, f.ny];
            for (int j = 0; j < f.ny; j++)
            {
                for (int i = 0; i < f.nx; i++)
                {
                    if (f.Rv[i, j] > f.Gv[i, j])
                    {
                        A[i, j] = f.Gv[i, j];
                    }
                    else
                    {
                        A[i, j] = f.Rv[i, j];
                    }
                }
            }
            pictureBox1.Image = f.GrayImg(A);
        }

        //二值化
        private void BinaryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            byte[,] A = new byte[f.nx, f.ny];
            for (int j = 0; j < f.ny; j++)
            {
                for (int i = 0; i < f.nx; i++)
                {
                    if (f.Gv[i, j] < 128)
                    {
                        A[i, j] = 1;
                    }
                }
            }
            pictureBox1.Image = f.BWImg(A);
        }

        //負片
        private void NegativeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            byte[,] A = new byte[f.nx, f.ny];
            for (int j = 0; j < f.ny; j++)
            {
                for (int i = 0; i < f.nx; i++)
                {
                    A[i, j] = (byte)(255 - f.Gv[i, j]);
                }
            }
            pictureBox1.Image = f.GrayImg(A);
        }

        //儲存現狀影像
        private void SaveImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.Image.Save(saveFileDialog1.FileName);
            }
        }
    }
}
