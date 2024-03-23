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

namespace CsSegm
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        byte[,] c; //監視區圓周標記點
        byte[,] d; //浮萍目標點
        TgInfo RTg; //邊界標記之目標物件
        FastPixel f = new FastPixel(); //宣告快速繪圖物件
        //儲存影像
        private void SaveImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.Image.Save(saveFileDialog1.FileName);
            }
        }
        //浮萍二值化
        private void LeafToolStripMenuItem_Click(object sender, EventArgs e)
        {
            d = new byte[f.nx, f.ny]; //浮萍目標點陣列
            for (int i = 1; i < f.nx - 1; i++)
            {
                for (int j = 1; j < f.ny - 1; j++)
                {
                    if (f.Gv[i, j] > 70 && (int)f.Gv[i, j] - f.Bv[i, j] > 50) d[i, j] = 1;
                }
            }
            pictureBox1.Image = f.BWImg(d); //建立浮萍二值化圖
        }
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
        //水面圓周邊界二值化
        private void RimToolStripMenuItem_Click(object sender, EventArgs e)
        {
            c = new byte[f.nx, f.ny]; //浮萍目標點陣列
            for (int i = 1; i < f.nx - 1; i++)
            {
                for (int j = 1; j < f.ny - 1; j++)
                {
                    if (f.Gv[i, j] > 40 && f.Rv[i, j] > f.Gv[i, j] && f.Rv[i, j] > f.Bv[i, j])
                    {
                        c[i, j] = 1;
                    }
                }
            }
            pictureBox1.Image = f.BWImg(c); //建立浮萍二值化圖
        }
        //取得邊界(紅色圓圈)目標
        private void RimTgToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TgInfo T = getMaxTg(c); //取得最大紅色區塊目標
            Bitmap bmp = new Bitmap(f.nx, f.ny);
            for (int m = 0; m < T.P.Count; m++)
            {
                Point p = (Point)T.P[m];
                bmp.SetPixel(p.X, p.Y, Color.Black);
            }
            pictureBox1.Image = bmp;
        }
        //取得最大目標
        private TgInfo getMaxTg(byte[,] q)
        {
            int mx = 0; //最多的目標點數
            RTg = new TgInfo(); //最大的圓周目標
            byte[,] b = (byte[,])q.Clone(); //建立目標點陣列副本
            for (int i = 1; i < f.nx - 1; i++)
            {
                for (int j = 1; j < f.ny - 1; j++)
                {
                    if (b[i, j] == 0) continue;
                    TgInfo G = new TgInfo();
                    G.xmn = i; G.xmx = i; G.ymn = j; G.ymx = j; G.P = new ArrayList();
                    ArrayList nc = new ArrayList();
                    nc.Add(new Point(i, j)); G.P.Add(new Point(i, j));
                    b[i, j] = 0;
                    do
                    {
                        ArrayList nb = (ArrayList)nc.Clone();
                        nc = new ArrayList();
                        for (int m = 0; m < nb.Count; m++)
                        {
                            Point p = (Point)nb[m];
                            for (int ii = p.X - 1; ii <= p.X + 1; ii++)
                            {
                                for (int jj = p.Y - 1; jj <= p.Y + 1; jj++)
                                {
                                    if (b[ii, jj] == 0) continue;
                                    Point k = new Point(ii, jj);
                                    nc.Add(k); G.P.Add(k); G.np += 1;
                                    if (ii < G.xmn) G.xmn = ii;
                                    if (ii > G.xmx) G.xmx = ii;
                                    if (jj < G.ymn) G.ymn = jj;
                                    if (jj > G.ymx) G.ymx = jj;
                                    b[ii, jj] = 0;
                                }
                            }
                        }
                    } while (nc.Count > 0);
                    if (q[i - 1, j] == 1) continue; //排除白色區塊的負目標，起點左邊是黑點
                    if (G.np > mx)
                    {
                        mx = G.np;
                        G.width = G.xmx - G.xmn + 1;
                        G.height = G.ymx - G.ymn + 1;
                        G.cx = (G.xmn + G.xmx) / 2; G.cy = (G.ymn + G.ymx) / 2;
                        RTg = G;
                    }
                }
            }
            return RTg; //回傳目標
        }
        //計算與繪製結果→圓周內之浮萍面積
        private void LeafRimToolStripMenuItem_Click(object sender, EventArgs e)
        {
            double r = 0; //邊界圓圈之半徑
            for (int k = 0; k < RTg.P.Count; k++)
            {
                Point p = (Point)RTg.P[k];
                double x = (double)(p.X - RTg.cx), y = (double)(p.Y - RTg.cy);
                double rr = Math.Sqrt(x * x + y * y);
                r += rr;
            }
            r /= RTg.np; //平均半徑
            int nLf = 0;
            byte[,] A = new byte[f.nx, f.ny];
            for (int i = RTg.xmn; i <= RTg.xmx; i++) {
                for (int j = RTg.ymn; j <= RTg.ymx; j++) {
                    if (d[i, j] == 0) continue;
                    double x = (double)(i - RTg.cx), y = (double)(j - RTg.cy);
                    double rr = Math.Sqrt(x * x + y * y); //浮萍目標到中心距離
                    if (rr > r) continue; //目標點在監視區外(玻璃杯倒影)
                    A[i, j] = 1; nLf += 1; //監視範圍內→計量
                }
            }
            Bitmap bmp = f.BWImg(A); //建立監視區內浮萍二值化圖
            Graphics G = Graphics.FromImage(bmp);
            G.DrawEllipse(Pens.Red, RTg.xmn, RTg.ymn, RTg.width, RTg.height); //繪製監視區邊界圓周
            pictureBox1.Image = bmp;
            double Area = Math.PI * r * r;//監視總面積→畫素單位
            double pc = nLf * 100 / Area;//浮萍面積百分比
            MessageBox.Show(pc.ToString());
        }
    }
}
