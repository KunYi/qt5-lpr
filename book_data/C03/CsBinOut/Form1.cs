using System;
using System.Collections;
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

        //開啟影像
        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Bitmap bmp = new Bitmap(openFileDialog1.FileName);
                f.Bmp2RGB(bmp); //讀取RGB亮度陣列
                pictureBox1.Image = bmp;
            }
        }
        //儲存影像
        private void SaveImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.Image.Save(saveFileDialog1.FileName);
            }
        }
        //灰階
        private void GrayToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pictureBox1.Image = f.GrayImg(f.Gv);
        }
        //平均亮度方塊圖
        int Gdim = 40; //計算區域亮度區塊的寬與高
        int[,] Th; //每一區塊的平均亮度，二值化門檻值
        private void AveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int kx = f.nx / Gdim, ky = f.ny / Gdim;
            Th = new int[kx, ky]; //區塊陣列
            //累計各區塊亮度值總和
            for (int i = 0; i < f.nx; i++)
            {
                int x = i / Gdim;
                for (int j = 0; j < f.ny; j++)
                {
                    int y = j / Gdim;
                    Th[x, y] += f.Gv[i, j];
                }
            }
            //建立亮度塊狀圖
            byte[,] A = new byte[f.nx, f.ny];
            for (int i = 0; i < kx; i++)
            {
                for (int j = 0; j < ky; j++)
                {
                    Th[i, j] /= Gdim * Gdim;
                    for (int ii = 0; ii < Gdim; ii++)
                    {
                        for (int jj = 0; jj < Gdim; jj++)
                        {
                            A[i * Gdim + ii, j * Gdim + jj] = (byte)Th[i, j];
                        }
                    }
                }
            }
            pictureBox1.Image = f.GrayImg(A);
        }
        //二值化
        byte[,] Z;
        private void BinaryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Z = new byte[f.nx, f.ny];
            for (int i = 1; i < f.nx - 1; i++)
            {
                int x = i / Gdim;
                for (int j = 1; j < f.ny - 1; j++)
                {
                    int y = j / Gdim;
                    if (f.Gv[i, j] < Th[x, y])
                    {
                        Z[i, j] = 1;
                    }
                }
            }
            pictureBox1.Image = f.BWImg(Z);
        }
        //輪廓線
        byte[,] Q;
        private void OutlineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Q = Outline(Z);
            pictureBox1.Image = f.BWImg(Q);
        }
        //建立輪廓點陣列
        private byte[,] Outline(byte[,] b)
        {
            byte[,] Q = new byte[f.nx, f.ny];
            for (int i = 1; i < f.nx - 1; i++)
            {
                for (int j = 1; j < f.ny - 1; j++)
                {
                    if (b[i, j] == 0) continue;
                    if (b[i - 1, j] == 0) { Q[i, j] = 1; continue; }
                    if (b[i + 1, j] == 0) { Q[i, j] = 1; continue; }
                    if (b[i, j - 1] == 0) { Q[i, j] = 1; continue; }
                    if (b[i, j + 1] == 0) { Q[i, j] = 1; }
                }
            }
            return Q;
        }
        //選擇顯示某目標之輪廓線
        private void PictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (Q == null) return;
                int x = e.X;
                int y = e.Y;
                //尋找左方最近之輪廓點
                while (Q[x, y] == 0 && x > 0)
                {
                    x--;
                }
                ArrayList A = getGrp(Q, x, y);//搜尋此目標所有輪廓點
                Bitmap bmp = f.BWImg(Q);//建立輪廓圖
                for (int k = 0; k < A.Count; k++)
                {
                    Point p = (Point)A[k];
                    bmp.SetPixel(p.X, p.Y, Color.Red);
                }
                pictureBox1.Image = bmp;
            }
        }
        //氾濫式演算法取得某目標之輪廓點
        private ArrayList getGrp(byte[,] q, int i, int j)
        {
            if (q[i, j] == 0) return new ArrayList();
            byte[,] b = (byte[,])q.Clone();//建立輪廓點陣列副本
            ArrayList nc = new ArrayList();//每一輪搜尋的起點集合
            nc.Add(new Point(i, j));//輸入之搜尋起點
            b[i, j] = 0;//清除此起點之輪廓點標記
            ArrayList A = nc;//此目標中所有目標點的集合
            do
            {
                ArrayList nb = (ArrayList)nc.Clone();//複製此輪之搜尋起點集合
                nc = new ArrayList();//清除準備蒐集下一輪搜尋起點之集合
                for (int m = 0; m < nb.Count; m++)
                {
                    Point p = (Point)nb[m];//搜尋起點
                    //在此點周邊3X3區域內找輪廓點
                    for (int ii = p.X - 1; ii <= p.X + 1; ii++)
                    {
                        for (int jj = p.Y - 1; jj <= p.Y + 1; jj++)
                        {
                            if (b[ii, jj] == 0) continue;//非輪廓點忽略
                            Point k = new Point(ii, jj);//建立點物件
                            nc.Add(k);//本輪搜尋新增的輪廓點
                            A.Add(k);//加入所有已蒐集到的目標點集合
                            b[ii, jj] = 0;//清除輪廓點點標記
                        }
                    }
                }
            } while (nc.Count > 0);//此輪搜尋有新發現輪廓點時繼續搜尋
            return A; //回傳目標物件集合
        }
    }
}

