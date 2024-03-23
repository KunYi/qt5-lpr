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
        TgInfo Tsel; //擇處理之目標
        byte[,] Tbin; //擇處理之二值化陣列
        static int Pw = 25, Ph = 50; //標準字模影像之寬與高
        int mw = 0, mh = 0; //標準字元目標寬高
        Array[] MC; //正規化完成後的字元二值化陣列
        double Inc = 0; //車牌傾斜角度(>0為順時針傾斜)

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
        //找車牌字元目標群組
        private void AlignToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Z = DoBinary(B); //二值化
            Q = Outline(Z); //建立輪廓點陣列
            C = getTargets(Q); //建立目標物件集合
            C = AlignTgs(C); //找到最多七個的字元目標群組
            //末字中心點與首字中心點的偏移量，斜率計算參數
            int n = C.Count;
            int dx = ((TgInfo)C[n - 1]).cx - ((TgInfo)C[0]).cx;
            int dy = ((TgInfo)C[n - 1]).cy - ((TgInfo)C[0]).cy;
            Inc = Math.Atan2((double)dy, (double)dx); //字元排列傾角
            //繪製有效目標
            Bitmap bmp = new Bitmap(f.nx, f.ny);
            for (int k = 0; k < C.Count; k++)
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
        //旋轉目標
        private void RotateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Tbin = RotateTg(Tbin, ref Tsel, Inc); //旋轉目標二值化影像
            pictureBox1.Image = f.BWImg(Tbin); //繪製轉正後之影像
        }
        //將單一目標轉正
        private byte[,] RotateTg(byte[,] b, ref TgInfo T, double A)
        {
            if (A == 0) return b; //無傾斜不須旋轉
            if (A > 0) A = -A; //順或逆時針傾斜時需要旋轉方向相反，經過推導A應該永遠為負值
            double[,] R = new double[2, 2]; //旋轉矩陣
            R[0, 0] = Math.Cos(A); R[0, 1] = Math.Sin(A);
            R[1, 0] = -R[0, 1]; R[1, 1] = R[0, 0];
            int x0 = T.xmn, y0 = T.ymx; //左下角座標
            //旋轉後之目標範圍
            int xmn = f.nx, xmx = 0, ymn = f.ny, ymx = 0;
            for (int i = T.xmn; i <= T.xmx; i++)
            {
                for (int j = T.ymn; j <= T.ymx; j++)
                {
                    if (b[i, j] == 0) continue; //空點無須旋轉
                    int x = i - x0, y = y0 - j; //轉換螢幕座標為直角座標
                    int xx = (int)(x * R[0, 0] + y * R[0, 1] + x0); //旋轉後X座標
                    if (xx < 1 || xx > f.nx - 2) continue; //邊界淨空
                    int yy = (int)(y0 - (x * R[1, 0] + y * R[1, 1])); //旋轉後Y座標
                    if (yy < 1 || yy > f.ny - 2) continue; //邊界淨空
                    b[i, j] = 0; b[xx, yy] = 1;
                    //旋轉後目標的範圍偵測
                    if (xx < xmn) xmn = xx;
                    if (xx > xmx) xmx = xx;
                    if (yy < ymn) ymn = yy;
                    if (yy > ymx) ymx = yy;
                }
            }
            //重設目標屬性
            T.xmn = xmn; T.xmx = xmx; T.ymn = ymn; T.ymx = ymx;
            T.width = T.xmx - T.xmn + 1; T.height = T.ymx - T.ymn + 1;
            T.cx = (T.xmx + T.xmn) / 2; T.cy = (T.ymx + T.ymn) / 2;
            //補足因為旋轉運算實產生的數位化誤差造成的資料空點
            for (int i = T.xmn; i <= T.xmx; i++)
            {
                for (int j = T.ymn; j <= T.ymx; j++)
                {
                    if (b[i, j] == 1) continue;
                    if (b[i - 1, j] + b[i + 1, j] + b[i, j - 1] + b[i, j + 1] >= 3) b[i, j] = 1;
                }
            }
            return b;
        }
        //點選目標
        private void PictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
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
                    m = k; break;//被點選目標
                }
                if (m >= 0)//有選取目標時
                {
                    Tsel = (TgInfo)C[m];//點選之目標
                    Tbin = new byte[f.nx, f.ny];//選取目標的二值化陣列
                    for (int n = 0; n < Tsel.P.Count; n++)
                    {
                        Point p = (Point)Tsel.P[n];
                        Tbin[p.X, p.Y] = 1;//起點
                        //向右連通成實心影像
                        int i = p.X + 1;
                        while (Z[i, p.Y] == 1)
                        {
                            Tbin[i, p.Y] = 1; i += 1;
                        }
                        //向左連通成實心影像
                        i = p.X - 1;
                        while (Z[i, p.Y] == 1)
                        {
                            Tbin[i, p.Y] = 1; i -= 1;
                        }
                    }
                    pictureBox1.Image = f.BWImg(Tbin);//繪製二值化圖
                }
            }
        }
        //字元目標正規化到字模寬高
        private void NormalizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            double fx = (double)Tsel.width / Pw, fy = (double)Tsel.height / Ph;
            byte[,] V = new byte[Pw, Ph];
            for (int i = 0; i < Pw; i++)
            {
                int x = Tsel.xmn + (int)(i * fx);
                for (int j = 0; j < Ph; j++)
                {
                    int y = Tsel.ymn + (int)(j * fy);
                    V[i, j] = Tbin[x, y];
                }
            }
            pictureBox1.Image = f.BWImg(V);
        }
        //完整辨識處理
        private void CorrectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int n = C.Count; //目標總數
            //旋轉所有目標
            TgInfo[] T = new TgInfo[n]; Array[] M = new Array[n];
            int[] w = new int[n]; int[] h = new int[n];
            for (int k = 0; k < n; k++)
            {
                TgInfo G = (TgInfo)C[k];
                M[k] = Tg2Bin(G); //建立單一目標的二值化矩陣
                TgInfo GG = new TgInfo().Clone(G);
                M[k] = RotateTg((byte[,])M[k], ref GG, Inc); //旋轉目標
                T[k] = GG; //儲存旋轉後的目標物件
                w[k] = GG.width; //寬度陣列
                h[k] = GG.height; //高度陣列
            }
            Array.Sort(w); Array.Sort(h); //寬高度排序，小到大
            mw = w[n - 2]; mh = h[n - 2]; //取第二寬或高的目標為標準，避開意外沾連的極端目標
            //車牌全圖矩陣，字元間隔4畫素
            byte[,] R = new byte[(Pw + 4) * n, Ph];
            MC = new Array[n];
            for (int k = 0; k < n; k++)
            {
                MC[k] = NmBin(T[k], (byte[,])M[k], mw, mh); //個別字元正規化矩陣
                int xs = (Pw + 4) * k;//X偏移量
                for (int i = 0; i < Pw; i++)
                {
                    for (int j = 0; j < Ph; j++)
                    {
                        R[xs + i, j] = ((byte[,])MC[k])[i, j];
                    }
                }
            }
            pictureBox1.Image = f.BWImg(R); //顯示正規化之後的車牌
        }
        //建立單一目標的二值化矩陣
        private byte[,] Tg2Bin(TgInfo T)
        {
            byte[,] b = new byte[f.nx, f.ny];//二值化陣列
            for (int n = 0; n < T.P.Count; n++)
            {
                Point p = (Point)T.P[n];
                b[p.X, p.Y] = 1;//起點
                //向右連通成實心影像
                int i = p.X + 1;
                while (Z[i, p.Y] == 1)
                {
                    b[i, p.Y] = 1; i += 1;
                }
                //向左連通成實心影像
                i = p.X - 1;
                while (Z[i, p.Y] == 1)
                {
                    b[i, p.Y] = 1; i -= 1;
                }
            }
            return b;
        }
        //建立正規化目標二值化陣列
        private byte[,] NmBin(TgInfo T, byte[,] M, int mw, int mh)
        {
            double fx = (double)mw / Pw, fy = (double)mh / Ph;
            byte[,] V = new byte[Pw, Ph];
            for (int i = 0; i < Pw; i++)
            {
                int sx = 0; //過窄字元的平移量，預設不平移
                if (T.width / mw < 0.75) //過窄字元，可能為1或I
                {
                    sx = (mw - T.width) / 2; //平移寬度差之一半
                }
                int x = (int)(T.xmn + i * fx - sx);
                if (x < 0 || x > f.nx - 1) continue;
                for (int j = 0; j < Ph; j++)
                {
                    int y = T.ymn + (int)(j * fy);
                    V[i, j] = M[x, y];
                }
            }
            return V;
        }
        //加隔線
        private void AddDashToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //計算最大字元間距
            int n = C.Count, dmx = 0, mi = 0;
            for (int i = 0; i < n - 1; i++)
            {
                int d1 = ((TgInfo)C[i + 1]).cx - ((TgInfo)C[i]).cx;
                int d2 = ((TgInfo)C[i + 1]).cy - ((TgInfo)C[i]).cy;
                int d = (d1 * d1) + (d2 * d2);
                if (d > dmx)
                {
                    dmx = d; mi = i;
                }
            }
            //繪製含隔線車牌
            //車牌全圖矩陣，字元間隔4畫素
            byte[,] R = new byte[(Pw + 4) * n + 20, Ph];
            for (int k = 0; k < n; k++)
            {
                int xs = (Pw + 4) * k;
                if (k > mi) xs += 20;
                for (int i = 0; i < Pw; i++)
                {
                    for (int j = 0; j < Ph; j++)
                    {
                        R[xs + i, j] = ((byte[,])MC[k])[i, j];
                    }
                }
                if (k == mi) //繪製隔線
                {
                    xs += Pw + 2;
                    for (int i = 5; i < 15; i++)
                    {
                        for (int j = 23; j < 28; j++)
                        {
                            R[xs + i, j] = 1;
                        }
                    }
                }
            }
            pictureBox1.Image = f.BWImg(R);
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
        int minHeight = 20, maxHeight = 80, minWidth = 2, maxWidth = 80, Tgmax = 20;
        private ArrayList getTargets(byte[,] q)
        {
            ArrayList A = new ArrayList();
            byte[,] b = (byte[,])q.Clone();
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
                    if (Z[i - 1, j] == 1) continue;
                    G.width = G.xmx - G.xmn + 1;
                    G.height = G.ymx - G.ymn + 1;
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
                        if (pm > G.pm) G.pm = pm;
                    }
                    A.Add(G);
                }
            }
            //以對比度排序
            for (int i = 0; i <= Tgmax; i++)
            {
                if (i > A.Count - 1) break;
                for (int j = i + 1; j < A.Count; j++)
                {
                    TgInfo T = (TgInfo)A[i], G = (TgInfo)A[j];
                    if (T.pm < G.pm)
                    {
                        A[i] = G; A[j] = T;
                    }
                }
            }
            //取得Tgmax個最明顯的目標輸出
            C = new ArrayList();
            for (int i = 0; i < Tgmax; i++)
            {
                if (i > A.Count - 1) break;
                TgInfo T = (TgInfo)A[i]; T.ID = i;
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
        private ArrayList AlignTgs(ArrayList C)
        {
            ArrayList R = new ArrayList();//最佳目標組合與最佳度比度
            int pmx = 0;
            for (int i = 0; i < C.Count; i++)
            {
                TgInfo T = (TgInfo)C[i];//核心目標
                ArrayList D = new ArrayList(); int Dm = 0;//此輪搜尋的目標集合
                D.Add(T); Dm = T.pm;//加入搜尋起點目標
                int x1 = (int)(T.cx - T.height * 2.5);//搜尋X範圍
                int x2 = (int)(T.cx + T.height * 2.5);
                int y1 = (int)(T.cy - T.height * 1.5);//搜尋Y範圍
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
                }
            }
            //目標群位置左右排序
            if (R.Count > 1)
            {
                int n = R.Count;
                for (int i = 0; i < n - 1; i++)
                {
                    for (int j = i + 1; j < n; j++)
                    {
                        TgInfo Ti = (TgInfo)R[i], Tj = (TgInfo)R[j];
                        if (Ti.cx > Tj.cx)
                        {
                            R[i] = Tj; R[j] = Ti;
                        }
                    }
                }
            }
            return R;
        }
    }
}
