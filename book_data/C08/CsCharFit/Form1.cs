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
        static int Pw = 25, Ph = 50; //標準字模影像之寬與高
        byte[,,,] P = new byte[2, 36, Pw, Ph]; //六七碼車牌所有英數字二值化陣列
        byte[,,] P69 = new byte[2, Pw, Ph]; //變形的6與9
        Array[] MC; //正規化完成後的字元二值化陣列
        int mw = 0, mh = 0; //標準字元目標寬高
        string LP = ""; //車牌號碼
        double Inc = 0; //車牌傾斜角度(>0為順時針傾斜)
        //字元對照表
        //0-9→0-9
        //10→A，11→B，12→C，13→D，14→E，15→F，16→G，17→H，18→I，19→J
        //20→K，21→L，22→M，23→N，24→O，25→P，26→Q，27→R，28→S，29→T
        //30→U，31→V，32→W，33→X，34→Y，35→Z
        char[] Ch = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
            'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N',
            'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };
        FastPixel f = new FastPixel(); //宣告快速繪圖物件

        //儲存影像
        private void SaveImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.Image.Save(saveFileDialog1.FileName);
            }
        }
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
                M[k] = RotateTg((byte[,])M[k], ref G, Inc); //旋轉目標
                T[k] = G; //儲存旋轉後的目標物件
                w[k] = G.width; //寬度陣列
                h[k] = G.height; //高度陣列
            }
            Array.Sort(w); Array.Sort(h); //寬高度排序，小到大
            mw = w[n - 2]; mh = h[n - 2]; //取第二寬或高的目標為標準，避開意外沾連的極端目標
            //車牌全圖矩陣，字元間隔4畫素
            byte[,] R = new byte[(Pw + 4) * n, Ph];
            MC = new Array[n];
            for (int k = 0; k < n; k++)
            {
                MC[k] = NmBin(T[k], (byte[,])M[k], mw, mh); //個別字元正規化矩陣
                int xs = (Pw + 4) * k;
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
            byte[,] b = new byte[f.nx, f.ny];
            for (int n = 0; n < T.P.Count; n++)
            {
                Point p = (Point)T.P[n];
                b[p.X, p.Y] = 1;
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
        //辨識整個車牌
        private void RecognizeAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //計算最大字元間距與位置
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
            LP = ""; //車牌字串
            int sc = 0; //符合度
            for (int i = 0; i < MC.Length; i++)
            {
                ChInfo k = BestC((byte[,])MC[i]);
                LP += k.Ch;
                sc += k.ft;
                if (i == mi) LP += '-';
            }
            sc /= MC.Length;
            LP = ChkLP(LP, sc);
            LP = ChkED(LP);
            MessageBox.Show(LP + "," + sc.ToString());
        }
        //檢驗車牌是否正確的程式
        private string ChkLP(string S, int V)
        {
            if (V < 500) return ""; //符合度低於及格分數
            if (S.Length < 5) return ""; //包含分隔線在內字數小於5
            int m = S.IndexOf('-'); //格線位置
            if (m == 1) return ""; //沒有1-x 的字數區段格式
            return S; //合格車牌
        }
        //嘗試依據英數字規範修改車牌答案
        private string ChkED(string S)
        {
            char[] C = S.ToCharArray(); //字串轉成字元陣列
            int n1 = S.IndexOf('-'); //第一區段長度
            int n2 = C.Length - n1 - 1; //第二區段長度
            int d1 = 0, d2 = 0; //數字區的起終點
            if (n1 > n2) { d1 = 0; d2 = n1 - 1; } //第一區段較長
            if (n2 > n1) { d1 = n1 + 1; d2 = C.Length - 1; } //第二區段較長
            if (d2 == 0) return S; //無法判定純數字區段(2-2或3-3)
            //嘗試將純數字區段的英文字改成數字
            for (int i = d1; i <= d2; i++)
            {
                C[i] = E2D(C[i]);
            }
            //如果是七碼車牌，強制將前三碼中的數字改成英文
            if (n1 == 3 && n2 == 4)
            {
                for (int i = 0; i <= 2; i++)
                {
                    C[i] = D2E(C[i]);
                }
            }
            //重組字串
            S = "";
            for (int i = 0; i < C.Length; i++)
            {
                S += C[i];
            }
            return S;
        }
        //嘗試將英文字母變成相似的數字
        private char E2D(char C)
        {
            if (C == 'B') return '8';
            if (C == 'D') return '0';
            if (C == 'O') return '0';
            return C;
        }
        //嘗試將英文字母變成相似的數字
        private char D2E(char C)
        {
            if (C == '8') return 'B';
            if (C == '0') return 'D';
            return C;
        }
        //最佳字元
        private ChInfo BestC(byte[,] A)
        {
            ChInfo C = new ChInfo();
            //六七碼正常字形比對
            for (int m = 0; m < 2; m++)
            {
                for (int k = 0; k < 36; k++)
                {
                    int n0 = 0; //字模黑點數
                    int nf = 0; //符合的黑點數
                    for (int i = 0; i < Pw; i++)
                    {
                        for (int j = 0; j < Ph; j++)
                        {
                            if (P[m, k, i, j] == 0)
                            {
                                if (A[i, j] == 1) nf -= 1; //目標與字模不符合點數
                            }
                            else
                            {
                                n0 += 1; //字模黑點數累計
                                if (A[i, j] == 1) nf += 1; //目標與字模符合點數
                            }
                        }
                    }
                    int v = nf * 1000 / n0; //符合點數千分比
                    if (v > C.ft)
                    {
                        C.ft = v;
                        C.Ch = Ch[k];
                        C.kind = m;
                    }
                }
            }
            //變形6與9比對
            for (int k = 0; k < 2; k++)
            {
                int n0 = 0; //字模黑點數
                int nf = 0; //符合的黑點數
                for (int i = 0; i < Pw; i++)
                {
                    for (int j = 0; j < Ph; j++)
                    {
                        if (P69[k, i, j] == 0)
                        {
                            if (A[i, j] == 1) nf -= 1; //目標與字模不符合點數
                        }
                        else
                        {
                            n0 += 1; //字模黑點數累計
                            if (A[i, j] == 1) nf += 1; //目標與字模符合點數
                        }
                    }
                }
                int v = nf * 1000 / n0; //符合點數千分比
                if (v > C.ft)
                {
                    C.ft = v;
                    if (k == 0) { C.Ch = '6'; } else { C.Ch = '9'; }
                }
            }
            return C;
        }
        //啟動程式載入字模
        private void Form1_Load(object sender, EventArgs e)
        {
            //MessageBox.Show(ChkED("A8C-01D6"));
            FontLoad();
        }
        //載入字模
        private void FontLoad()
        {
            byte[] q = CsCharFit.Properties.Resources.font;
            int n = 0;
            for (int m = 0; m < 2; m++)
            {
                for (int k = 0; k < 36; k++)
                {
                    for (int j = 0; j < Ph; j++)
                    {
                        for (int i = 0; i < Pw; i++)
                        {
                            P[m, k, i, j] = q[n]; n += 1;
                        }
                    }
                }
            }
            for (int k = 0; k < 2; k++)
            {
                for (int j = 0; j < Ph; j++)
                {
                    for (int i = 0; i < Pw; i++)
                    {
                        P69[k, i, j] = q[n]; n += 1;
                    }
                }
            }
        }
        //左方外插一字
        private void LeftExtToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //字元間的最小間距
            int xd = f.nx, yd = f.ny;
            for (int i = 0; i < C.Count - 1; i++)
            {
                int dx = ((TgInfo)C[i + 1]).cx - ((TgInfo)C[i]).cx;
                if (dx < xd) xd = dx;
                int dy = ((TgInfo)C[i + 1]).cy - ((TgInfo)C[i]).cy;
                if (dy < yd) yd = dy;
            }
            TgInfo G = (TgInfo)C[0]; //複製一個外插的目標
            //目標位置移動
            G.cx -= xd; G.cy -= yd;
            G.xmn = G.cx - mw / 2; G.xmx = G.cx + mw / 2;
            G.ymn = G.cy - mh / 2; G.ymx = G.cy + mh / 2;
            //外插目標二值化陣列
            byte[,] A = new byte[f.nx, f.ny];
            for (int i = G.xmn; i <= G.xmx; i++)
            {
                for (int j = G.ymn; j <= G.ymx; j++)
                {
                    A[i, j] = Z[i, j];
                }
            }
            A = RotateTg(A, ref G, Inc); //旋轉目標
            byte[,] D = NmBin(G, A, mw, mh); //外插字元正規化
            ChInfo k = BestC(D); //辨識字元
            LP = k.Ch + LP; //外插字元加入車牌
            //繪圖顯示外插目標框線
            Bitmap bmp = new Bitmap(openFileDialog1.FileName);
            Rectangle rec = new Rectangle(G.xmn, G.ymn, mw, mh);
            Graphics Gr = Graphics.FromImage(bmp);
            Gr.DrawRectangle(Pens.Red, rec);
            pictureBox1.Image = bmp;
            MessageBox.Show(LP);
        }
        //點選字元
        private void PictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                int m = e.X / (Pw + 4);
                if (m < 0 || m > MC.Length - 1) return;
                ChInfo C = BestC((byte[,])MC[m]);
                MessageBox.Show(C.Ch.ToString() + "," + C.ft.ToString());
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
            ArrayList R = new ArrayList();
            int pmx = 0;
            for (int i = 0; i < C.Count; i++)
            {
                TgInfo T = (TgInfo)C[i];
                ArrayList D = new ArrayList(); int Dm = 0;
                D.Add(T); Dm = T.pm;
                int x1 = (int)(T.cx - T.height * 2.5);
                int x2 = (int)(T.cx + T.height * 2.5);
                int y1 = (int)(T.cy - T.height * 1.5);
                int y2 = (int)(T.cy + T.height * 1.5);
                for (int j = 0; j < C.Count; j++)
                {
                    if (i == j) continue;
                    TgInfo G = (TgInfo)C[j];
                    if (G.cx < x1) continue;
                    if (G.cx > x2) continue;
                    if (G.cy < y1) continue;
                    if (G.cy > y2) continue;
                    if (G.width > T.height) continue;
                    if (G.height > T.height * 1.5) continue;
                    D.Add(G); Dm += G.pm;
                    if (D.Count >= 7) break;
                }
                if (Dm > pmx)
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
