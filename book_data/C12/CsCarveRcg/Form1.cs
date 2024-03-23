using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CsCarveRcg
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        byte[,] B; //灰階陣列
        byte[,] Z; //二值化陣列
        ArrayList C; //目標物件集合
        int brt; //全圖平均亮度
        int Ytop, Ybot; //字元列上下切線之Y值

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
        //搜尋鎖定字元列的上下切線
        private void YBoundToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int[] Yb = new int[f.ny]; //各Y值橫列的亮度總和
            for (int j = 0; j < f.ny; j++)
            {
                for (int i = 0; i < f.nx; i++)
                {
                    Yb[j] += B[i, j];
                }
            }
            //用相鄰Y值的亮度差最大位置鎖定字元列上下邊界
            int tmx = 0, bmx = 0; //最大亮度差值
            Ytop = 0; Ybot = 0;
            for (int j = 0; j < f.ny - 2; j++)
            {
                if (Yb[j] == 0 && Yb[j + 1] == 0) continue; //無資料略過
                if (j < f.ny / 2)
                {
                    int dy = Yb[j] - Yb[j + 1];
                    if (dy > tmx)
                    {
                        tmx = dy; Ytop = j;
                    }
                }
                else
                {
                    int dy = Yb[j + 1] - Yb[j];
                    if (dy > bmx)
                    {
                        bmx = dy; Ybot = j;
                    }
                }
            }
            //繪製上下邊界
            Bitmap bmp = f.GrayImg(B);
            Graphics G = Graphics.FromImage(bmp);
            G.DrawLine(Pens.Red, 0, Ytop, f.nx - 1, Ytop);
            G.DrawLine(Pens.Red, 0, Ytop + 1, f.nx - 1, Ytop);
            G.DrawLine(Pens.Red, 0, Ybot, f.nx - 1, Ybot);
            G.DrawLine(Pens.Red, 0, Ybot - 1, f.nx - 1, Ybot);
            pictureBox1.Image = bmp;
        }
        //二值化
        private void BinaryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int[] his = new int[255]; //字元區的亮度分佈直方圖
            int n = 0;
            for (int j = Ytop; j <= Ybot; j++)
            {
                for (int i = 0; i < f.nx; i++)
                {
                    his[B[i, j]] += 1; n += 1;
                }
            }
            //計算二值化門檻值
            brt = 0;
            int ac = his[0];
            while (ac < (int)(n * 0.4))
            {
                brt += 1;
                ac += his[brt];
            }
            Z = new byte[f.nx, f.ny];
            for (int j = Ytop; j <= Ybot; j++)
            {
                for (int i = 1; i < f.nx - 1; i++)
                {
                    if (B[i, j] < brt) Z[i, j] = 1;
                }
            }
            pictureBox1.Image = f.BWImg(Z); //建立二值化圖
        }
        //建立目標物件
        private void TargetsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            C = getTargets(Z); //建立目標物件集合
            //繪製目標框線
            Bitmap bmp = f.BWImg(Z);
            Graphics G = Graphics.FromImage(bmp);
            for (int k = 0; k < C.Count; k++)
            {
                TgInfo T = (TgInfo)C[k];
                G.DrawRectangle(Pens.Red, T.xmn, T.ymn, T.width, T.height);
            }
            pictureBox1.Image = bmp;
        }
        //建立目標
        private ArrayList getTargets(byte[,] q)
        {
            int minwidth = 20, minheight = 20; //最小有效目標寬度寬高度
            ArrayList A = new ArrayList();
            byte[,] b = (byte[,])q.Clone(); //建立輪廓點陣列副本
            for (int i = 1; i < f.nx - 1; i++)
            {
                for (int j = 1; j < f.ny - 1; j++)
                {
                    if (b[i, j] == 0) continue;
                    TgInfo G = new TgInfo();
                    G.xmn = i; G.xmx = i; G.ymn = j; G.ymx = j; G.P = new ArrayList();
                    ArrayList nc = new ArrayList(); //每一輪搜尋的起點集合
                    nc.Add(new Point(i, j)); G.P.Add(new Point(i, j)); //搜尋起點
                    b[i, j] = 0; //清除此起點之輪廓點標記
                    do
                    {
                        ArrayList nb = (ArrayList)nc.Clone(); //複製此輪之搜尋起點集合
                        nc = new ArrayList(); //清除準備蒐集下一輪搜尋起點之集合
                        for (int m = 0; m < nb.Count; m++)
                        {
                            Point p = (Point)nb[m]; //搜尋起點
                            //在此點周邊3X3區域內找目標點
                            for (int ii = p.X - 1; ii <= p.X + 1; ii++)
                            {
                                if (ii < 0 || ii > f.nx - 1) continue; //避免搜尋越界
                                for (int jj = p.Y - 1; jj <= p.Y + 1; jj++)
                                {
                                    if (jj < 0 || jj > f.ny - 1) continue; //避免搜尋越界
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
                    } while (nc.Count > 0); //此輪搜尋有新發現輪廓點時繼續搜尋
                    if (Z[i - 1, j] == 1) continue; //排除白色區塊的負目標
                    G.width = G.xmx - G.xmn + 1;
                    G.height = G.ymx - G.ymn + 1;
                    //以寬高大小篩選目標
                    if (G.height < minheight) continue;
                    if (G.width < minwidth) continue;
                    A.Add(G);
                }
            }
            return A; //回傳目標物件集合
        }
        //融合交疊目標
        private void MergeTgsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool[] merged = new bool[C.Count];
            for (int k = 0; k < C.Count - 1; k++)
            {
                if (merged[k]) continue;
                TgInfo T = (TgInfo)C[k];
                for (int m = k + 1; m < C.Count; m++)
                {
                    if (merged[m]) continue;
                    TgInfo G = (TgInfo)C[m];
                    if (T.xmx > G.xmn) //目標交疊進行融合
                    {
                        T.xmn = Math.Min(T.xmn, G.xmn);
                        T.xmx = Math.Max(T.xmx, G.xmx);
                        T.ymn = Math.Min(T.ymn, G.ymn);
                        T.ymx = Math.Max(T.ymx, G.ymx);
                        T.width = T.xmx - T.xmn + 1;
                        T.height = T.ymx - T.ymn + 1;
                        for (int i = 0; i < G.P.Count; i++)
                        {
                            T.P.Add(G.P[i]);
                        }
                        merged[m] = true; //標記已被融合目標
                        C[k] = T; //回置更新目標物件
                    }
                }
            }
            //排除已被融合目標，建立新目標物件集合
            ArrayList A = new ArrayList();
            for (int k = 0; k < C.Count; k++)
            {
                if (merged[k]) continue;
                A.Add(C[k]);
            }
            C = A; //回置更新目標集合
            //分割過大的沾連目標
            int bh = Ybot - Ytop + 1; //理想字元高度
            int bw = (int)(bh * 0.6); //理想字元寬度
            TgInfo q = new TgInfo();
            A = new ArrayList();
            for (int k = 0; k < C.Count; k++)
            {
                TgInfo T = (TgInfo)C[k];
                if (T.width > bw * 2)
                {
                    TgInfo G = q.clone(T); //複製T
                    int midx = (T.xmn + T.xmx) / 2;
                    T.xmx = midx;
                    T.width = T.xmx - T.xmn + 1;
                    G.xmn = midx;
                    G.width = G.xmx - G.xmn + 1;
                    A.Add(T); A.Add(G);
                }
                else
                {
                    A.Add(T);
                }
            }
            C = A; //回置更新目標集合
            //繪製已融合處理之目標範圍框架
            Bitmap bmp = f.BWImg(Z);
            Graphics Gr = Graphics.FromImage(bmp);
            for (int k = 0; k < C.Count; k++)
            {
                TgInfo T = (TgInfo)C[k];
                Gr.DrawRectangle(Pens.Red, T.xmn, T.ymn, T.width, T.height);
            }
            pictureBox1.Image = bmp;
        }
        //鎖定最佳目標區塊
        private void BestFitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int bh = Ybot - Ytop + 1; //理想字元高度
            int bw = (int)(bh * 0.6); //理想字元寬度
            //建立X方向的每行強度總和變化陣列
            int[] Xb = new int[f.nx];
            for (int i = 0; i < f.nx; i++)
            {
                for (int j = Ytop; j <= Ybot; j++)
                {
                    Xb[i] += Z[i, j];
                }
            }
            //假設目標為理想寬度，搜尋目標點密度最高之X位置
            for (int k = 0; k < C.Count; k++)
            {
                TgInfo T = (TgInfo)C[k];
                int dw = Math.Abs(bw - T.width); //目標寬度與理想寬度之差
                int x1 = T.xmn, x2 = T.xmn; //左右搜尋之X範圍
                if (T.width < bw) x1 -= dw; //目標窄於理想寬度時，搜尋起點左移
                if (T.width > bw) x2 += dw; //目標寬於理想寬度時，搜尋終點右移
                int mx = 0, mi = T.xmn; //目標內之最大目標點總量與X位置
                for (int i = x1; i <= x2; i++)
                {
                    int v = 0; //此估計目標內之目標點累計值
                    for (int x = 0; x <= bw; x++)
                    {
                        if (i + x > f.nx - 1) continue;
                        v += Xb[i + x];
                    }
                    if (v > mx)
                    {
                        mx = v; mi = i;
                    }
                }
                T.width = bw; //修改目標點寬度為理想寬度
                T.xmn = mi; T.xmx = mi + bw - 1; //修改目標左右邊界
                //重建目標物件內之目標點集合內容
                T.P = new ArrayList();
                for (int i = T.xmn; i <= T.xmx; i++)
                {
                    for (int j = T.ymn; j <= T.ymx; j++)
                    {
                        if (Z[i, j] == 1) T.P.Add(new Point(i, j));
                    }
                }
                C[k] = T;
            }
            //繪製最佳目標位置與範圍
            Bitmap bmp = f.BWImg(Z);
            Graphics Gr = Graphics.FromImage(bmp);
            for (int k = 0; k < C.Count; k++)
            {
                TgInfo T = (TgInfo)C[k];
                Gr.DrawRectangle(Pens.Red, T.xmn, T.ymn, T.width, T.height);
            }
            pictureBox1.Image = bmp;
        }

        //蝕刻或浮雕字影像的影像前處理→空間亮度偏差值積分
        private void IntegralToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //計算全圖平均亮度
            brt = 0;
            for (int i = 0; i < f.nx; i++)
            {
                for (int j = 0; j < f.ny; j++)
                {
                    brt += f.Gv[i, j];
                }
            }
            brt = (int)((double)brt / f.nx / f.ny);
            //計算空間亮度偏差值的局部積分
            int[,] T = new int[f.nx, f.ny];
            int mx = 0;
            for (int i = 3; i < f.nx - 3; i++)
            {
                for (int j = 3; j < f.ny - 3; j++)
                {
                    int d = 0;
                    for (int x = -3; x < 4; x++)
                    {
                        for (int y = -3; y < 4; y++)
                        {
                            d += Math.Abs(f.Gv[i + x, j + y] - brt); //與平均亮度的偏差值
                        }
                    }
                    T[i, j] = d; //此點積分值
                    if (d > mx) mx = d; //最大積分值
                }
            }
            //積分陣列轉換為灰階
            B = new byte[f.nx, f.ny];
            double z = (double)mx / 255 / 2; //積分值投射為灰階值之比例(X2倍)
            for (int i = 3; i < f.nx - 3; i++)
            {
                for (int j = 3; j < f.ny - 3; j++)
                {
                    int u = (int)(T[i, j] / z);
                    if (u > 255) u = 255;
                    B[i, j] = (byte)(255 - u); //灰階，目標為深色
                }
            }
            pictureBox1.Image = f.GrayImg(B); //灰階圖
        }
    }
}
