Public Class Form1
    Dim B(,) As Byte '灰階陣列
    Dim Z(,) As Byte '全圖二值化陣列
    Dim Q(,) As Byte '輪廓線陣列
    Dim Gdim As Integer = 40 '計算區域亮度區塊的寬與高
    Dim Th(,) As Integer '每一區塊的平均亮度，二值化門檻值
    Dim C As ArrayList '目標物件集合
    Dim Mb As Bitmap '顯示目標的影像
    '目標物件結構
    Public Structure TgInfo
        Dim np As Integer '目標點數
        Dim P As ArrayList '目標點的集合
        Dim xmn As Short, xmx As Short, ymn As Short, ymx As Short '四面座標極值
        Dim cx As Integer, cy As Integer '目標中心點座標
        Dim width As Integer, height As Integer '寬與高
        Dim pm As Integer '目標與背景的對比強度
        Dim ID As Integer '目標依據對比度的排序
    End Structure
    '開啟檔案
    Private Sub OpenToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) _
        Handles OpenToolStripMenuItem.Click
        If OpenFileDialog1.ShowDialog = Windows.Forms.DialogResult.OK Then
            Dim bmp As New Bitmap(OpenFileDialog1.FileName)
            Bmp2RGB(bmp) '擷取影像資訊
            B = Gv '以綠光為灰階
            PictureBox1.Image = bmp '顯示
        End If
    End Sub
    '二值化
    Private Sub BinaryToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) _
        Handles BinaryToolStripMenuItem.Click
        Th = ThresholdBuild(B) '建立二值化使用之門檻值陣列
        ReDim Z(nx - 1, ny - 1) '建立二值化陣列
        For i As Integer = 1 To nx - 2
            Dim x As Integer = i \ Gdim 'x座標換算
            For j As Integer = 1 To ny - 2
                Dim y As Integer = j \ Gdim 'y座標換算
                If B(i, j) < Th(x, y) Then
                    Z(i, j) = 1 '低於亮度門檻設為目標點
                End If
            Next
        Next
        Dim bmp As Bitmap = BWImg(Z) '建立二值化圖
        PictureBox1.Image = bmp '顯示
    End Sub
    '門檻值陣列建立
    Private Function ThresholdBuild(ByVal b(,) As Byte) As Integer(,)
        Dim kx As Integer = nx \ Gdim, ky As Integer = ny \ Gdim
        Dim T(kx, ky) As Integer
        '累計各區塊亮度值總和
        For i As Integer = 0 To nx - 1
            Dim x As Integer = i \ Gdim
            For j As Integer = 0 To ny - 1
                Dim y As Integer = j \ Gdim
                T(x, y) += b(i, j) '亮度值累加
            Next
        Next
        '區塊亮度平均值計算
        For i As Integer = 0 To kx - 1
            For j As Integer = 0 To ky - 1
                T(i, j) /= Gdim * Gdim
            Next
        Next
        Return T
    End Function
    '輪廓線
    Private Sub OutlineToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) _
        Handles OutlineToolStripMenuItem.Click
        If IsNothing(Z) Then Exit Sub '無二值化圖忽略
        Q = Outline(Z) '建立輪廓點陣列
        Dim bmp As Bitmap = BWImg(Q) '建立輪廓圖
        PictureBox1.Image = bmp '顯示輪廓圖
    End Sub
    '建立輪廓點陣列
    Private Function Outline(ByVal b(,) As Byte) As Byte(,)
        Dim Q(nx - 1, ny - 1) As Byte '輪廓點陣列
        For i As Integer = 1 To nx - 2
            For j As Integer = 1 + 1 To ny - 2
                If b(i, j) = 0 Then Continue For '非輪廓點忽略
                If b(i, j - 1) = 0 Then Q(i, j) = 1 : Continue For '確認為輪廓點
                If b(i - 1, j) = 0 Then Q(i, j) = 1 : Continue For '確認為輪廓點
                If b(i + 1, j) = 0 Then Q(i, j) = 1 : Continue For '確認為輪廓點
                If b(i, j + 1) = 0 Then Q(i, j) = 1 '確認為輪廓點
            Next
        Next
        Return Q
    End Function
    '建立目標物件
    Private Sub TargetsToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) _
        Handles TargetsToolStripMenuItem.Click
        C = getTargets(Q) '建立目標物件集合
        '繪製目標輪廓點
        Dim bmp As New Bitmap(nx, ny)
        For k As Integer = 0 To C.Count - 1
            Dim T As TgInfo = C(k)
            For m As Integer = 0 To T.P.Count - 1
                Dim p As Point = T.P(m)
                bmp.SetPixel(p.X, p.Y, Color.Black)
            Next
        Next
        PictureBox1.Image = bmp '顯示目標輪廓
        Mb = bmp.Clone
    End Sub
    '以輪廓點建立目標陣列，排除負目標
    Function getTargets(ByVal q(,) As Byte) As ArrayList
        Dim A As New ArrayList
        Dim b(,) As Byte = q.Clone '建立輪廓點陣列副本
        For i As Integer = 1 To nx - 2
            For j As Integer = 1 To ny - 2
                If b(i, j) = 0 Then Continue For
                Dim G As New TgInfo
                G.xmn = i : G.xmx = i : G.ymn = j : G.ymx = j : G.np = 1 : G.P = New ArrayList
                Dim nc As New ArrayList '每一輪搜尋的起點集合
                nc.Add(New Point(i, j)) '輸入之搜尋起點
                G.P.Add(New Point(i, j))
                b(i, j) = 0 '清除此起點之輪廓點標記
                Do
                    Dim nb As ArrayList = nc.Clone '複製此輪之搜尋起點集合
                    nc = New ArrayList '清除準備蒐集下一輪搜尋起點之集合
                    For m As Integer = 0 To nb.Count - 1
                        Dim p As Point = nb(m) '搜尋起點
                        '在此點周邊3X3區域內找輪廓點
                        For ii As Integer = p.X - 1 To p.X + 1
                            For jj As Integer = p.Y - 1 To p.Y + 1
                                If b(ii, jj) = 0 Then Continue For '非輪廓點忽略
                                Dim k As New Point(ii, jj) '建立點物件
                                nc.Add(k) '本輪搜尋新增的輪廓點
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
                G.height = G.ymx - G.ymn + 1 '高度計算
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
    '依據目標大小篩選目標
    Dim minHeight As Integer = 10, maxHeight As Integer = 80 '有效目標高度範圍
    Dim minwidth As Integer = 2, maxWidth As Integer = 80 '有效目標寬度範圍
    Private Sub FilterToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) _
        Handles FilterToolStripMenuItem.Click
        Dim D As New ArrayList
        For k As Integer = 0 To C.Count - 1
            Dim T As TgInfo = C(k)
            If T.height < minHeight Then Continue For
            If T.height > maxHeight Then Continue For
            If T.width < minwidth Then Continue For
            If T.width > maxWidth Then Continue For
            D.Add(T) '有效目標
        Next
        C = D '回置目標集合
        '繪製有效目標
        Dim bmp As New Bitmap(nx, ny)
        For k As Integer = 0 To C.Count - 1
            Dim T As TgInfo = C(k)
            For m As Integer = 0 To T.P.Count - 1
                Dim p As Point = T.P(m)
                bmp.SetPixel(p.X, p.Y, Color.Black)
            Next
        Next
        PictureBox1.Image = bmp '顯示目標輪廓
        Mb = bmp.Clone
    End Sub
    '依據對比度排序前X大目標
    Private Sub SortToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) _
        Handles SortToolStripMenuItem.Click
        '建立目標物件的對比度屬性
        For k As Integer = 0 To C.Count - 1
            Dim T As TgInfo = C(k)
            For m As Integer = 0 To T.P.Count - 1
                Dim pm As Integer = PointPm(T.P(m))
                If pm > T.pm Then T.pm = pm '最高對比度的輪廓點
            Next
            C(k) = T '回置目標到原屬集合
        Next
        For i As Integer = 0 To 9
            For j As Integer = i + 1 To C.Count - 1
                Dim T As TgInfo = C(i), G As TgInfo = C(j)
                If T.pm < G.pm Then
                    C(i) = G : C(j) = T '互換位置，高對比目標在前
                End If
            Next
        Next
        '繪製有效目標
        Dim bmp As New Bitmap(nx, ny)
        Dim D As New ArrayList
        For k As Integer = 0 To 9
            If k > C.Count - 1 Then Continue For
            Dim T As TgInfo = C(k)
            D.Add(T)
            For m As Integer = 0 To T.P.Count - 1
                Dim p As Point = T.P(m)
                bmp.SetPixel(p.X, p.Y, Color.Black)
            Next
        Next
        C = D
        PictureBox1.Image = bmp '顯示目標輪廓
        Mb = bmp.Clone
    End Sub
    '輪廓點與背景的對比度
    Private Function PointPm(ByVal p As Point) As Integer
        Dim x As Integer = p.X, y As Integer = p.Y
        Dim mx As Integer = B(x, y) '周邊最亮點，依據灰階陣列B
        If mx < B(x - 1, y) Then mx = B(x - 1, y)
        If mx < B(x + 1, y) Then mx = B(x + 1, y)
        If mx < B(x, y + 1) Then mx = B(x, y + 1)
        If mx < B(x, y - 1) Then mx = B(x, y - 1)
        Return mx - B(x, y) '最亮點與輪廓點的差值
    End Function
    '點選目標顯示位置與屬性
    Private Sub PictureBox1_MouseDown(ByVal sender As Object, ByVal e As MouseEventArgs) _
        Handles PictureBox1.MouseDown
        If IsNothing(Mb) Then Exit Sub
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
                Dim bmp As Bitmap = Mb.Clone
                Dim T As TgInfo = C(m)
                For k As Integer = 0 To T.P.Count - 1
                    Dim p As Point = T.P(k)
                    bmp.SetPixel(p.X, p.Y, Color.Red)
                Next
                PictureBox1.Image = bmp
                '指定目標的資訊
                Dim S As String = "Width=" + T.width.ToString
                S += vbNewLine + "Height=" + T.height.ToString
                S += vbNewLine + "Contrast=" + T.pm.ToString
                S += vbNewLine + "Points=" + T.np.ToString
                MsgBox(S)
            End If
        End If
    End Sub
End Class
