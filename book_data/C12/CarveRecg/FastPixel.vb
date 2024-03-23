'Fast RGB Processing Module
Module FastPixel
    Public nx As Integer, ny As Integer, Rv(,) As Byte, Gv(,) As Byte, Bv(,) As Byte
    Dim rgb() As Byte '影像的可存取副本資料陣列
    Dim D As System.Drawing.Imaging.BitmapData '影像資料
    Dim ptr As IntPtr '影像資料所在的記憶體指標(位置)
    Dim n As Integer '影像總共佔據的位元組數
    Dim L As Integer '一個影像列的記憶體位元組數，對於32位元電腦必須是32bit(或4byte)的倍數
    Dim nB As Integer '每一像素點是幾個位元組？通常為3(24bits)或4(32bits)  '取出RGB陣列(含縮放功能)
    '鎖定點陣圖(Bitmap)物件的記憶體位置，建立一個可操作的為元組陣列副本
    Sub LockBMP(ByVal bmp As Bitmap)
        Dim rect As New Rectangle(0, 0, bmp.Width, bmp.Height) '矩形物件，定義影像範圍
        D = bmp.LockBits(rect, Drawing.Imaging.ImageLockMode.ReadWrite, bmp.PixelFormat) '鎖定影像區記憶體(暫時不接受作業系統的移動)
        ptr = D.Scan0 '影像區塊的記憶體指標
        L = D.Stride '每一影像列的長度(bytes)
        nB = L \ bmp.Width '每一像素的位元組數(3或4)
        n = L * bmp.Height '影像總位元組數
        ReDim rgb(n - 1) '宣告影像副本資料陣列
        System.Runtime.InteropServices.Marshal.Copy(ptr, rgb, 0, n) '拷貝點陣圖資料到副本陣列
    End Sub
    '複製位元組陣列副本的處理結果到Bitmap物件，並解除其記憶體鎖定
    Sub UnLockBMP(ByVal bmp As Bitmap)
        System.Runtime.InteropServices.Marshal.Copy(rgb, 0, ptr, n) '拷貝副本陣列資料到點陣圖位置
        bmp.UnlockBits(D) '解除鎖定
    End Sub
    '取得RGB陣列
    Public Sub Bmp2RGB(ByVal bmp As Bitmap)
        nx = bmp.Width : ny = bmp.Height
        ReDim Rv(nx - 1, ny - 1), Gv(nx - 1, ny - 1), Bv(nx - 1, ny - 1) '輸出用灰階陣列
        LockBMP(bmp)
        For j As Integer = 0 To ny - 1
            Dim Lj As Integer = j * D.Stride
            For i As Integer = 0 To nx - 1
                Dim k As Integer = Lj + i * nB
                Rv(i, j) = rgb(k + 2) '紅光
                Gv(i, j) = rgb(k + 1) '綠光
                Bv(i, j) = rgb(k) '藍光
            Next
        Next
        bmp.UnlockBits(D) '解除鎖定
    End Sub
    '灰階圖
    Function GrayImg(ByVal b(,) As Byte) As Bitmap
        Dim bmp As New Bitmap(b.GetLength(0), b.GetLength(1))
        LockBMP(bmp)
        For i As Integer = 0 To b.GetLength(0) - 1
            For j As Integer = 0 To b.GetLength(1) - 1
                Dim k As Integer = j * L + i * nB '索引位置計算
                Dim c As Byte = b(i, j)
                rgb(k) = c : rgb(k + 1) = c : rgb(k + 2) = c 'RGB一致的灰色
                rgb(k + 3) = 255 '實心不透明
            Next
        Next
        UnLockBMP(bmp)
        Return bmp
    End Function
    '黑白圖
    Function BWImg(ByVal b(,) As Byte) As Bitmap
        Dim bmp As New Bitmap(b.GetLength(0), b.GetLength(1))
        LockBMP(bmp)
        For i As Integer = 0 To b.GetLength(0) - 1
            For j As Integer = 0 To b.GetLength(1) - 1
                Dim k As Integer = j * L + i * nB '索引位置計算
                If b(i, j) = 1 Then
                    rgb(k) = 0 : rgb(k + 1) = 0 : rgb(k + 2) = 0 '黑色
                Else
                    rgb(k) = 255 : rgb(k + 1) = 255 : rgb(k + 2) = 255 '白色
                End If
                rgb(k + 3) = 255 '實心不透明
            Next
        Next
        UnLockBMP(bmp)
        Return bmp
    End Function

End Module
