using System;
using System.Collections;
using System.Drawing;

namespace CsRGBshow
{
    //Fast RGB Processing Class
    class FastPixel
    {
        public int nx, ny; //影像寬與高
        public byte[,] Rv, Gv, Bv; //Red, Green & Blue 陣列
        byte[] rgb; //影像的可存取副本資料陣列
        System.Drawing.Imaging.BitmapData D; //影像資料
        IntPtr ptr; //影像資料所在的記憶體指標(位置)
        int n, L, nB; //影像總位元組數，單行位元組數，單點位元組數

        //鎖定點陣圖(Bitmap)物件的記憶體位置，建立一個可操作的為元組陣列副本
        private void LockBMP(Bitmap bmp)
        {
            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height); //矩形物件，定義影像範圍
            //鎖定影像區記憶體(暫時不接受作業系統的移動)
            D = bmp.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, bmp.PixelFormat);
            ptr = D.Scan0; //影像區塊的記憶體指標
            L = D.Stride; //每一影像列的長度(bytes)
            nB = (int)Math.Floor((double)L / (double)bmp.Width); //每一像素的位元組數(3或4)
            n = L * bmp.Height; //影像總位元組數
            rgb = new byte[n]; //宣告影像副本資料陣列
            System.Runtime.InteropServices.Marshal.Copy(ptr, rgb, 0, n); //拷貝點陣圖資料到副本陣列
        }
        //複製位元組陣列副本的處理結果到Bitmap物件，並解除其記憶體鎖定
        private void UnLockBMP(Bitmap bmp)
        {
            System.Runtime.InteropServices.Marshal.Copy(rgb, 0, ptr, n); //拷貝副本陣列到點陣圖位置
            bmp.UnlockBits(D); //解除鎖定
        }
        //取得RGB陣列
        public void Bmp2RGB(Bitmap bmp)
        {
            nx = bmp.Width; ny = bmp.Height; //影像寬高
            Rv = new byte[nx, ny]; Gv = new byte[nx, ny]; Bv = new byte[nx, ny]; //RGB
            LockBMP(bmp);
            for (int j = 0; j < ny; j++)
            {
                int Lj = j * D.Stride;
                for (int i = 0; i < nx; i++)
                {
                    int k = Lj + i * nB;
                    Rv[i, j] = rgb[k + 2]; //Red
                    Gv[i, j] = rgb[k + 1]; //Green
                    Bv[i, j] = rgb[k]; //Blue
                }
            }
            UnLockBMP(bmp);
        }

        //灰階圖
        public Bitmap GrayImg(byte[,] b)
        {
            Bitmap bmp = new Bitmap(b.GetLength(0), b.GetLength(1));
            LockBMP(bmp);
            for (int j = 0; j < b.GetLength(1); j++)
            {
                for (int i = 0; i < b.GetLength(0); i++)
                {
                    int k = j * L + i * nB;
                    byte c = b[i, j];
                    rgb[k] = c; rgb[k + 1] = c; rgb[k + 2] = c; //RGB一致
                    rgb[k + 3] = 255; //實心不透明
                }
            }
            UnLockBMP(bmp);
            return bmp;
        }
        //黑白圖
        public Bitmap BWImg(byte[,] b)
        {
            Bitmap bmp = new Bitmap(b.GetLength(0), b.GetLength(1));
            LockBMP(bmp);
            for (int j = 0; j < b.GetLength(1); j++)
            {
                for (int i = 0; i < b.GetLength(0); i++)
                {
                    int k = j * L + i * nB;
                    if (b[i, j] == 1)
                    {
                        rgb[k] = 0; rgb[k + 1] = 0; rgb[k + 2] = 0; //黑
                    }
                    else
                    {
                        rgb[k] = 255; rgb[k + 1] = 255; rgb[k + 2] = 255; //白
                    }
                    rgb[k + 3] = 255;
                }
            }
            UnLockBMP(bmp);
            return bmp;

        }
    }
    //目標物件類別
    class TgInfo
    {
        public int np = 0; //目標點數
        public ArrayList P = null; //目標點的集合
        public int xmn = 0, xmx = 0, ymn = 0, ymx = 0; //四面座標極值
        public int width = 0, height = 0; //寬與高
        public int pm = 0; //目標與背景的對比強度
        public int cx = 0, cy = 0; //目標中心點座標
        public int ID = 0; //依對比度排序的序號
    }
    //最佳字元結構
    class ChInfo {
        public char Ch; //最符合字元
        public int ft; //符合度評分
        public int kind; //六或七碼字元，0→六碼，1→七碼
    }
    //車牌資料結構
    class LPInfo {
        public string A; //車牌號碼
        public int Sc; //符合度
        public int N; //車牌字元目標個數
        public int kind; //六或七碼字型
        public int xmn, xmx, ymn, ymx; //車牌四邊極值
        public int width, height; //車牌寬與高
        public int cx, cy; //車牌中心點座標
    }
}
