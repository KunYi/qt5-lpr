Public Class Form1
    Dim B(,) As Byte '灰階陣列
    Dim Z(,) As Byte '全圖二值化陣列
    Dim C As ArrayList '目標物件集合
    Dim brt As Integer = 128 '全圖平均亮度
    Dim Ytop As Integer, Ybot As Integer '字元列上下切線之Y值
    '目標物件結構
    Public Structure TgInfo
        Dim np As Integer '目標點數
        Dim P As ArrayList '目標點的集合
        Dim xmn As Short, xmx As Short, ymn As Short, ymx As Short '四面座標極值
        Dim width As Integer, height As Integer '寬與高
    End Structure
    '開啟檔案
    Private Sub OpenToolStripMenuItem_Click(ByVal sender As Object, ByVal e As System.EventArgs) _
        Handles OpenToolStripMenuItem.Click
        If OpenFileDialog1.ShowDialog = Windows.Forms.DialogResult.OK Then
            Dim bmp As New Bitmap(OpenFileDialog1.FileName)
            Bmp2RGB(bmp) '擷取影像資訊
            PictureBox1.Image = bmp '顯示
        End If
    End Sub
    '蝕刻或浮雕字影像的影像前處理→空間亮度偏差值積分
    Private Sub IntegralToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) _
        Handles IntegralToolStripMenuItem.Click
        '計算全圖平均亮度
        brt = 0
        For i As Integer = 0 To nx - 1
            For j As Integer = 0 To ny - 1
                brt += Gv(i, j)
            Next
        Next
        brt /= nx * ny
        '計算空間亮度偏差值的局部積分
        Dim T(nx - 1, ny - 1) As Integer
        Dim mx As Integer = 0
        For i As Integer = 3 To nx - 4
            For j As Integer = 3 To ny - 4
                Dim d As Integer = 0
                For x As Integer = -3 To 3
                    For y As Integer = -3 To 3
                        d += Math.Abs(Gv(i + x, j + y) - brt) '與平均亮度的偏差值
                    Next
                Next
                T(i, j) = d '此點積分值
                If d > mx Then mx = d '最大積分值
            Next
        Next
        '積分陣列轉換為灰階
        ReDim B(nx - 1, ny - 1) '重設灰階陣列
        Dim f As Single = mx / 255 / 2 '積分值投射為灰階值之比例(X2倍)
        For i As Integer = 3 To nx - 4
            For j As Integer = 3 To ny - 4
                Dim u As Integer = T(i, j) / f
                If u > 255 Then u = 255
                B(i, j) = 255 - u '灰階，目標為深色
            Next
        Next
        PictureBox1.Image = GrayImg(B) '灰階圖
    End Sub
    '搜尋鎖定字元列的上下切線
    Private Sub YBoundToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) _
        Handles YBoundToolStripMenuItem.Click
        Dim Yb(ny - 1) As Integer '各Y值橫列的亮度總和
        For j As Integer = 0 To ny - 1
            For i As Integer = 0 To nx - 1
                Yb(j) += B(i, j)
            Next
        Next
        '用相鄰Y值的亮度差最大位置鎖定字元列上下邊界
        Dim tmx As Integer = 0, bmx As Integer = 0 '最大亮度差值
        Ytop = 0 : Ybot = ny - 1 '預設字元列上下邊界Y值
        For j As Integer = 0 To ny - 2
            If Yb(j) = 0 Or Yb(j + 1) = 0 Then Continue For '無資料略過
            If j < ny / 2 Then '上邊界搜尋
                Dim dy As Integer = Yb(j) - Yb(j + 1)
                If dy > tmx Then
                    tmx = dy : Ytop = j
                End If
            Else '下邊界搜尋
                Dim dy As Integer = Yb(j + 1) - Yb(j)
                If dy > bmx Then
                    bmx = dy : Ybot = j + 1
                End If
            End If
        Next
        '繪製上下邊界
        Dim bmp As Bitmap = GrayImg(B)
        Dim G As Graphics = Graphics.FromImage(bmp)
        G.DrawLine(Pens.Red, 0, Ytop, nx - 1, Ytop)
        G.DrawLine(Pens.Red, 0, Ytop + 1, nx - 1, Ytop)
        G.DrawLine(Pens.Red, 0, Ybot, nx - 1, Ybot)
        G.DrawLine(Pens.Red, 0, Ybot - 1, nx - 1, Ybot)
        PictureBox1.Image = bmp
    End Sub
    '二值化
    Private Sub BinaryToolStripMenuItem_Click(ByVal sender As Object, ByVal e As System.EventArgs) _
        Handles BinaryToolStripMenuItem.Click
        Dim his(255) As Integer, n As Integer = 0 '字元區的亮度分佈直方圖
        For j As Integer = Ytop To Ybot
            For i As Integer = 0 To nx - 1
                his(B(i, j)) += 1 : n += 1
            Next
        Next
        '計算二值化門檻值
        brt = 0
        Dim ac As Integer = his(brt)
        Do While ac < n * 0.4 '假設點強度前25%為目標
            brt += 1
            ac += his(brt)
        Loop
        ReDim Z(nx - 1, ny - 1) '建立二值化陣列
        For i As Integer = 0 To nx - 1
            For j As Integer = Ytop To Ybot
                If B(i, j) < brt Then Z(i, j) = 1 '低於亮度門檻設為目標點
            Next
        Next
        PictureBox1.Image = BWImg(Z) '建立二值化圖
    End Sub
    '建立目標物件
    Private Sub TargetsToolStripMenuItem_Click(ByVal sender As Object, ByVal e As System.EventArgs) _
        Handles TargetsToolStripMenuItem.Click
        C = getTargets(Z) '建立目標物件集合
        '繪製目標框線
        Dim bmp As Bitmap = BWImg(Z)
        Dim G As Graphics = Graphics.FromImage(bmp)
        For k As Integer = 0 To C.Count - 1
            Dim T As TgInfo = C(k)
            G.DrawRectangle(Pens.Red, T.xmn, T.ymn, T.width, T.height)
        Next
        PictureBox1.Image = bmp '顯示目標輪廓
    End Sub
    '建立目標
    Function getTargets(ByVal q(,) As Byte) As ArrayList
        Dim minwidth As Integer = 20, minHeight As Integer = 20  '最小有效目標寬度寬高度
        Dim A As New ArrayList
        Dim b(,) As Byte = q.Clone '建立輪廓點陣列副本
        For i As Integer = 1 To nx - 2
            For j As Integer = 1 To ny - 2
                If b(i, j) = 0 Then Continue For
                Dim G As New TgInfo
                G.xmn = i : G.xmx = i : G.ymn = j : G.ymx = j : G.np = 1 : G.P = New ArrayList
                Dim nc As New ArrayList '每一輪搜尋的起點集合
                nc.Add(New Point(i, j)) '搜尋起點
                G.P.Add(New Point(i, j))
                b(i, j) = 0 '清除此起點之輪廓點標記
                Do
                    Dim nb As ArrayList = nc.Clone '複製此輪之搜尋起點集合
                    nc = New ArrayList '清除準備蒐集下一輪搜尋起點之集合
                    For m As Integer = 0 To nb.Count - 1
                        Dim p As Point = nb(m) '搜尋起點
                        '在此點周邊3X3區域內找目標點
                        For ii As Integer = p.X - 1 To p.X + 1
                            If ii < 0 Or ii > nx - 1 Then Continue For '避免搜尋越界
                            For jj As Integer = p.Y - 1 To p.Y + 1
                                If jj < 0 Or jj > ny - 1 Then Continue For '避免搜尋越界
                                If b(ii, jj) = 0 Then Continue For '非目標點忽略
                                Dim k As New Point(ii, jj) '建立點物件
                                nc.Add(k) '本輪搜尋新增的目標點
                                G.P.Add(k)
                                G.np += 1 '點數累計
                                If ii < G.xmn Then G.xmn = ii
                                If ii > G.xmx Then G.xmx = ii
                                If jj < G.ymn Then G.ymn = jj
                                If jj > G.ymx Then G.ymx = jj
                                b(ii, jj) = 0 '清除輪廓點點標記
                            Next
                        Next
                    Next
                Loop While nc.Count > 0 '此輪搜尋有新發現輪廓點時繼續搜尋
                If Z(i - 1, j) = 1 Then Continue For '排除白色區塊的負目標，起點左邊是黑點
                G.width = G.xmx - G.xmn + 1 '寬度計算
                If G.width < minwidth Then Continue For
                G.height = G.ymx - G.ymn + 1 '高度計算
                If G.height < minHeight Then Continue For
                A.Add(G) '加入有效目標集合
            Next
        Next
        Return A '回傳目標物件集合
    End Function
    '儲存目前影像
    Private Sub SaveImageToolStripMenuItem_Click(ByVal sender As Object, ByVal e As System.EventArgs) _
        Handles SaveImageToolStripMenuItem.Click
        If SaveFileDialog1.ShowDialog = Windows.Forms.DialogResult.OK Then
            PictureBox1.Image.Save(SaveFileDialog1.FileName)
        End If
    End Sub
    '融合交疊目標
    Private Sub MergeTgsToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) _
        Handles MergeTgsToolStripMenuItem.Click
        Dim merged(C.Count - 1) As Boolean '是否已被她目標融合之標記陣列
        For k As Integer = 0 To C.Count - 2
            If merged(k) Then Continue For '已被融合目標略過
            Dim T As TgInfo = C(k)
            For m As Integer = k + 1 To C.Count - 1
                If merged(k) Then Continue For
                Dim G As TgInfo = C(m)
                If T.xmx > G.xmn Then '目標交疊進行融合
                    T.xmn = Math.Min(T.xmn, G.xmn)
                    T.xmx = Math.Max(T.xmx, G.xmx)
                    T.ymn = Math.Min(T.ymn, G.ymn)
                    T.ymx = Math.Max(T.ymx, G.ymx)
                    T.width = T.xmx - T.xmn + 1
                    T.height = T.ymx - T.ymn + 1
                    For i As Integer = 0 To G.P.Count - 1
                        T.P.Add(G.P(i))
                    Next
                    merged(m) = True '標記已被融合目標
                    C(k) = T '回置目標物件
                End If
            Next
        Next
        '排除已被融合目標，建立新目標物件集合
        Dim A As New ArrayList
        For k As Integer = 0 To C.Count - 1
            If merged(k) Then Continue For
            A.Add(C(k))
        Next
        C = A '回置目標集合
        '分割過大的沾連目標
        Dim bh As Integer = Ybot - Ytop + 1 '理想字元高度
        Dim bw As Integer = bh * 0.6 '理想字元寬度
        A = New ArrayList
        For k As Integer = 0 To C.Count - 1
            Dim T As TgInfo = C(k)
            If T.width > bw * 2 Then '兩字沾連進行等分切割
                Dim G As TgInfo = T
                Dim midx As Integer = (T.xmn + T.xmx) / 2
                T.xmx = midx : T.width = T.xmx - T.xmn + 1
                G.xmn = midx : G.width = G.xmx - G.xmn + 1
                A.Add(T) : A.Add(G)
            Else '無沾連
                A.Add(T)
            End If
        Next
        C = A '回置標物件集合
        '繪製已融合處理之目標範圍框架
        Dim bmp As Bitmap = BWImg(Z)
        Dim Gr As Graphics = Graphics.FromImage(bmp)
        For k As Integer = 0 To C.Count - 1
            Dim T As TgInfo = C(k)
            Gr.DrawRectangle(Pens.Red, T.xmn, T.ymn, T.width, T.height)
        Next
        PictureBox1.Image = bmp
    End Sub
    '點選目標顯示位置與屬性
    Private Sub PictureBox1_MouseDown(ByVal sender As Object, ByVal e As Windows.Forms.MouseEventArgs) _
        Handles PictureBox1.MouseDown
        If e.Button = MouseButtons.Left Then
            Dim m As Integer = -1
            For k As Integer = 0 To C.Count - 1
                Dim T As TgInfo = C(k)
                If e.X < T.xmn Then Continue For
                If e.X > T.xmx Then Continue For
                If e.Y < T.ymn Then Continue For
                If e.Y > T.ymx Then Continue For
                m = k '被點選目標
                Exit For
            Next
            If m >= 0 Then '有被選目標時
                Dim bmp As Bitmap = BWImg(Z)
                Dim T As TgInfo = C(m)
                For k As Integer = 0 To T.P.Count - 1
                    Dim p As Point = T.P(k)
                    bmp.SetPixel(p.X, p.Y, Color.Red)
                Next
                Dim G As Graphics = Graphics.FromImage(bmp)
                G.DrawRectangle(Pens.Lime, T.xmn, T.ymn, T.width, T.height)
                PictureBox1.Image = bmp
                '指定目標的資訊
                Dim S As String = "Width=" + T.width.ToString
                S += vbNewLine + "Height=" + T.height.ToString
                S += vbNewLine + "Points=" + T.np.ToString
                MsgBox(S)
            End If
        End If
    End Sub
    '鎖定最佳目標區塊
    Private Sub BestFitToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) _
        Handles BestFitToolStripMenuItem.Click
        Dim bh As Integer = Ybot - Ytop + 1 '理想字元高度
        Dim bw As Integer = bh * 0.6 '理想字元寬度
        '建立X方向的每行強度總和變化陣列
        Dim Xb(nx - 1) As Integer
        For i As Integer = 0 To nx - 1
            For j As Integer = Ytop To Ybot
                Xb(i) += Z(i, j)
            Next
        Next
        '假設目標為理想寬度，搜尋目標點密度最高之X位置
        For k As Integer = 0 To C.Count - 1
            Dim T As TgInfo = C(k)
            Dim dw As Integer = Math.Abs(bw - T.width) '目標寬度與理想寬度之差
            Dim x1 As Integer = T.xmn, x2 As Integer = T.xmn '左右搜尋之X範圍
            If T.width < bw Then x1 -= dw '目標窄於理想寬度時，搜尋起點左移
            If T.width > bw Then x2 += dw '目標寬於理想寬度時，搜尋終點右移
            Dim mx As Integer = 0, mi As Integer = T.xmn '假設目標內之最大目標點總量與X位置
            For i As Integer = x1 To x2
                Dim v As Integer = 0 '此估計目標內之目標點累計值
                For x As Integer = 0 To bw - 1
                    If i + x > nx - 1 Then Continue For
                    v += Xb(i + x)
                Next
                If v > mx Then '找到目前最高目標點總量位置
                    mx = v : mi = i
                End If
            Next
            T.width = bw '修改目標點寬度為理想寬度
            T.xmn = mi : T.xmx = mi + bw - 1 '修改目標左右邊界
            '重建目標物件內之目標點集合內容
            T.P = New ArrayList
            For i As Integer = T.xmn To T.xmx
                For j As Integer = T.ymn To T.ymx
                    If Z(i, j) = 1 Then T.P.Add(New Point(i, j))
                Next
            Next
            C(k) = T
        Next
        '繪製最佳目標位置與範圍
        Dim bmp As Bitmap = BWImg(Z)
        Dim Gr As Graphics = Graphics.FromImage(bmp)
        For k As Integer = 0 To C.Count - 1
            Dim T As TgInfo = C(k)
            Gr.DrawRectangle(Pens.Red, T.xmn, T.ymn, T.width, T.height)
        Next
        PictureBox1.Image = bmp
    End Sub
End Class
