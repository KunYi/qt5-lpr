using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Eventing.Reader;
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
                B = f.Gv; //以綠光為灰階
                pictureBox1.Image = bmp;
            }
        }
        //輪廓圖
        private void OutlineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Z = DoBinary(B);
            Q = Outline(Z);
            pictureBox1.Image = f.BWImg(Q);
        }
        //二值化
        private byte[,] DoBinary(byte[,] b)
        {
            Th = ThresholdBuild(b);
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
            return Z;
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
        //建立合格目標集合
        private void TargetsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            C = getTargets(Q);
            //繪製有效目標
            Bitmap bmp = new Bitmap(f.nx, f.ny);
            for (int k = 0; k <= 10; k++)
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
        //選取特定目標顯示資訊
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
                    string S = "ID=" + T.ID.ToString();
                    S += "\n\r" + "Center=(" + T.cx.ToString() + "," + T.cy.ToString() + ")";
                    S += "\n\r" + "Width=" + T.width.ToString();
                    S += "\n\r" + "Height=" + T.height.ToString();
                    S += "\n\r" + "Contrast=" + T.pm.ToString();
                    S += "\n\r" + "Point=" + T.np.ToString();
                    MessageBox.Show(S);
                }
            }
        }
        //找車牌字元目標群組
        private void AlignToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ArrayList R = AlignTgs(C); //找到最多七個的字元目標
            Bitmap bmp = (Bitmap)Mb.Clone();
            for (int k = 0; k < R.Count; k++)
            {
                TgInfo T = (TgInfo)R[k];
                if (k == 0)//搜尋的中心目標畫成實心
                {
                    for (int i = T.xmn; i <= T.xmx; i++)
                    {
                        for (int j = T.ymn; j <= T.ymx; j++)
                        {
                            if (Z[i, j] == 1) bmp.SetPixel(i, j, Color.Red);
                        }
                    }
                }
                else//畫輪廓
                {
                    for (int m = 0; m < T.P.Count; m++)
                    {
                        Point p = (Point)T.P[m];
                        bmp.SetPixel(p.X, p.Y, Color.Red);
                    }
                }
            }
            Graphics Gr = Graphics.FromImage(bmp);//繪製搜尋區
            Gr.DrawRectangle(Pens.Lime, rec);
            pictureBox1.Image = bmp;
        }
        //儲存影像
        private void SaveImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.Image.Save(saveFileDialog1.FileName);
            }
        }

        //以輪廓點建立目標陣列，排除負目標
        int minHeight = 20, maxHeight = 80;//有效目標高度範圍
        int minWidth = 2, maxWidth = 80;//有效目標寬度範圍
        int Tgmax = 20;//進入決選範圍的最明顯目標上限
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
                    nc.Add(new Point(i, j)); //輸入之搜尋起點
                    G.P.Add(new Point(i, j));
                    b[i, j] = 0;//清除此起點之輪廓點標記
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
                                    G.P.Add(k);//點集合
                                    G.np += 1;
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
                    //以寬高大小篩選目標
                    if (G.height < minHeight) continue;
                    if (G.height > maxHeight) continue;
                    if (G.width < minWidth) continue;
                    if (G.width > maxWidth) continue;
                    G.cx = (G.xmn + G.xmx) / 2; G.cy = (G.ymn + G.ymx) / 2; //中心點
                    //計算目標的對比度
                    for (int m = 0; m < G.P.Count; m++)
                    {
                        int pm = PointPm((Point)G.P[m]);
                        if (pm > G.pm) G.pm = pm;//最高對比度的輪廓點
                    }
                    A.Add(G);//加入有效目標集合
                }
            }
            //以對比度排序
            for (int i = 0; i <= Tgmax; i++)
            {
                if (i > A.Count - 1) break;
                for (int j = i + 1; j < A.Count; j++)
                {
                    TgInfo T = (TgInfo)A[i], G = (TgInfo)A[j];
                    if (T.pm < G.pm)//互換位置，高對比目標在前
                    {
                        A[i] = G; A[j] = T;
                    }
                }
            }
            //取得Tgmax個最明顯的目標輸出
            C = new ArrayList();
            for (int i = 0; i < Tgmax; i++)
            {
                if (i > A.Count - 1) break;//超過總目標數
                TgInfo T = (TgInfo)A[i]; T.ID = i;//建立以對比度排序的序號
                C.Add(T);
            }
            return C; //回傳目標物件集合
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
        //找車牌字元目標群組
        Rectangle rec; //收集目標範圍框
        private ArrayList AlignTgs(ArrayList C)
        {
            ArrayList R = new ArrayList(); int pmx = 0;//最佳目標組合與最佳度比度
            for (int i = 0; i < C.Count; i++)
            {
                TgInfo T = (TgInfo)C[i];//核心目標
                ArrayList D = new ArrayList(); int Dm = 0;//此輪搜尋的目標集合
                D.Add(T); Dm = T.pm;//加入搜尋起點目標
                //搜尋X範圍
                int x1 = (int)(T.cx - T.height * 2.5);
                int x2 = (int)(T.cx + T.height * 2.5);
                //搜尋Y範圍
                int y1 = (int)(T.cy - T.height * 1.5);
                int y2 = (int)(T.cy + T.height * 1.5);
                for (int j = 0; j < C.Count; j++)
                {
                    if (i == j) continue;//與起點重複略過
                    TgInfo G = (TgInfo)C[j];
                    if (G.cx < x1) continue;
                    if (G.cx > x2) continue;
                    if (G.cy < y1) continue;
                    if (G.cy > y2) continue;
                    if (G.width > T.height) continue;//目標寬度太大略過
                    if (G.height > T.height * 1.5) continue;//目標高度太大略過
                    D.Add(G); Dm += G.pm;//合格目標加入集合
                    if (D.Count >= 7) break;//目標蒐集個數已滿跳離迴圈
                }
                if (Dm > pmx)//對比度高於之前的目標集合
                {
                    pmx = Dm; R = D;
                    rec = new Rectangle(x1, y1, x2 - x1 + 1, y2 - y1 + 1);//搜尋範圍
                }
            }
            return R;
        }
    }
}
