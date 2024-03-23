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
        byte[,] B; //灰階陣列
        byte[,] Z; //二值化陣列
        byte[,] Q; //輪廓線陣列
        int Gdim = 40; //計算區域亮度區塊的寬與高
        int[,] Th; //每一區塊的平均亮度，二值化門檻值
        ArrayList C; //目標物件集合
        Bitmap Mb; //底圖副本
        FastPixel f = new FastPixel(); //宣告快速繪圖物件

        //開啟影像
        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Bitmap bmp = new Bitmap(openFileDialog1.FileName);
                f.Bmp2RGB(bmp); //讀取RGB亮度陣列
                B = f.Gv;
                pictureBox1.Image = bmp;
            }
        }
        //二值化
        private void BinaryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Th = ThresholdBuild(B); //門檻值陣列建立
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
        //門檻值陣列建立
        private int[,] ThresholdBuild(byte[,] b)
        {
            int kx = f.nx / Gdim, ky = f.ny / Gdim;
            Th = new int[kx, ky];
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
            for (int i = 0; i < kx; i++)
            {
                for (int j = 0; j < ky; j++)
                {
                    Th[i, j] /= Gdim * Gdim;
                }
            }
            return Th;
        }
        //輪廓線
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
        //建立目標物件
        private void TargetsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            C = getTargets(Q); //建立目標物件集合
            //繪製目標輪廓點
            Bitmap bmp = new Bitmap(f.nx, f.ny);
            for (int k = 0; k < C.Count - 1; k++)
            {
                TgInfo T = (TgInfo)C[k];
                for (int m = 0; m < T.P.Count; m++)
                {
                    Point p = (Point)T.P[m];
                    bmp.SetPixel(p.X, p.Y, Color.Black);
                }
            }
            pictureBox1.Image = bmp;
        }
        //以輪廓點建立目標陣列，排除負目標
        private ArrayList getTargets(byte[,] q)
        {
            ArrayList A = new ArrayList();
            byte[,] b = (byte[,])q.Clone();//建立輪廓點陣列副本
            for (int i = 1; i < f.nx - 1; i++)
            {
                for (int j = 1; j < f.ny - 1; j++)
                {
                    if (b[i, j] == 0) continue;
                    TgInfo G = new TgInfo();
                    G.xmn = i; G.xmx = i; G.ymn = j; G.ymx = j; G.P = new ArrayList();
                    ArrayList nc = new ArrayList();//每一輪搜尋的起點集合
                    nc.Add(new Point(i, j));//輸入之搜尋起點
                    G.P.Add(new Point(i, j));
                    b[i, j] = 0;//清除此起點之輪廓點標記
                    do
                    {
                        ArrayList nb = (ArrayList)nc.Clone();//複製此輪之搜尋起點集合
                        nc = new ArrayList();// 清除準備蒐集下一輪搜尋起點之集合
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
                                    G.P.Add(k);
                                    G.np += 1;//點數累計
                                    if (ii < G.xmn) G.xmn = ii;
                                    if (ii > G.xmx) G.xmx = ii;
                                    if (jj < G.ymn) G.ymn = jj;
                                    if (jj > G.ymx) G.ymx = jj;
                                    b[ii, jj] = 0;//清除輪廓點點標記
                                }
                            }
                        }
                    } while (nc.Count > 0);//此輪搜尋有新發現輪廓點時繼續搜尋
                    if (Z[i - 1, j] == 1) continue;//排除白色區塊的負目標，起點左邊是黑點
                    G.width = G.xmx - G.xmn + 1;//寬度計算
                    G.height = G.ymx - G.ymn + 1;//高度計算
                    A.Add(G);//加入有效目標集合
                }
            }
            return A; //回傳目標物件集合
        }
        //點選目標顯示位置與屬性
        private void PictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (Mb == null) return;
            if (e.Button == MouseButtons.Left)
            {
                int m = -1;
                for (int k = 0; k < C.Count; k++)
                {
                    TgInfo T = (TgInfo)C[k];
                    if (e.X < T.xmn) continue;
                    if (e.X > T.xmx) continue;
                    if (e.Y < T.ymn) continue;
                    if (e.Y > T.ymx) continue;
                    m = k; break;
                }
                if (m >= 0)
                {
                    Bitmap bmp = (Bitmap)Mb.Clone();
                    TgInfo T = (TgInfo)C[m];
                    for (int n = 0; n < T.P.Count; n++)
                    {
                        Point p = (Point)T.P[n];
                        bmp.SetPixel(p.X, p.Y, Color.Red);
                    }
                    pictureBox1.Image = bmp;
                    //指定目標的資訊
                    string S = "Width=" + T.width.ToString();
                    S += "\n\r" + "Height=" + T.height.ToString();
                    S += "\n\r" + "Contrast=" + T.pm.ToString();
                    S += "\n\r" + "Point=" + T.np.ToString();
                    MessageBox.Show(S);
                }
            }
        }
        //依據對比度排序前10大目標
        private void SortToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //建立目標物件的對比度屬性
            for (int k = 0; k < C.Count; k++)
            {
                TgInfo T = (TgInfo)C[k];
                for (int m = 0; m < T.P.Count; m++)
                {
                    int pm = PointPm((Point)T.P[m]);
                    if (pm > T.pm) T.pm = pm;
                }
                C[k] = T;
            }
            //依對比度排序
            for (int i = 0; i < 10; i++)
            {
                for (int j = i + 1; j < C.Count; j++)
                {
                    TgInfo T = (TgInfo)C[i], G = (TgInfo)C[j];
                    if (T.pm < G.pm)
                    {
                        C[i] = G; C[j] = T;
                    }
                }
            }
            //繪製有效目標
            Bitmap bmp = new Bitmap(f.nx, f.ny);
            for (int k = 0; k < 10; k++)
            {
                TgInfo T = (TgInfo)C[k];
                for (int m = 0; m < T.P.Count; m++)
                {
                    Point p = (Point)T.P[m];
                    bmp.SetPixel(p.X, p.Y, Color.Black);
                }
            }
            pictureBox1.Image = bmp;
            Mb = (Bitmap)bmp.Clone();
        }
        //輪廓點與背景的對比度
        private int PointPm(Point p)
        {
            int x = p.X, y = p.Y, mx = B[x, y];
            if (mx < B[x - 1, y]) mx = B[x - 1, y];
            if (mx < B[x + 1, y]) mx = B[x + 1, y];
            if (mx < B[x, y - 1]) mx = B[x, y - 1];
            if (mx < B[x, y + 1]) mx = B[x, y + 1];
            return mx - B[x, y];
        }
        //儲存影像
        private void SaveImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.Image.Save(saveFileDialog1.FileName);
            }
        }
        //依據目標大小篩選目標
        int minHeight = 16, maxHeight = 100, minWidth = 2, maxWidth = 100;
        private void FilterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ArrayList D = new ArrayList();
            for (int k = 0; k < C.Count; k++)
            {
                TgInfo T = (TgInfo)C[k];
                if (T.height < minHeight) continue;
                if (T.height > maxHeight) continue;
                if (T.width < minWidth) continue;
                if (T.width > maxWidth) continue;
                D.Add(T);
            }
            C = D;
            //繪製有效目標
            Bitmap bmp = new Bitmap(f.nx, f.ny);
            for (int k = 0; k < C.Count - 1; k++)
            {
                TgInfo T = (TgInfo)C[k];
                for (int m = 0; m < T.P.Count; m++)
                {
                    Point p = (Point)T.P[m];
                    bmp.SetPixel(p.X, p.Y, Color.Black);
                }
            }
            pictureBox1.Image = bmp;
            Mb = (Bitmap)bmp.Clone();
        }
    }
}
