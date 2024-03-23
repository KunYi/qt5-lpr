Public Class Form1
    '開啟檔案
    Private Sub OpenToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) _
        Handles OpenToolStripMenuItem.Click
        If OpenFileDialog1.ShowDialog = Windows.Forms.DialogResult.OK Then
            Dim bmp As New Bitmap(OpenFileDialog1.FileName)
            Bmp2RGB(bmp)
            PictureBox1.Image = bmp
        End If
    End Sub
    '灰階
    Private Sub GrayToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) _
        Handles GrayToolStripMenuItem.Click
        PictureBox1.Image = GrayImg(Gv)
    End Sub
    '平均亮度方塊圖
    Dim Gdim As Integer = 40 '計算區域亮度區塊的寬與高
    Dim Th(,) As Integer '每一區塊的平均亮度，二值化門檻值
    Private Sub AveToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) _
         Handles AveToolStripMenuItem.Click
        Dim kx As Integer = nx \ Gdim, ky As Integer = ny \ Gdim
        ReDim Th(kx, ky)
        '累計各區塊亮度值總和
        For i As Integer = 0 To nx - 1
            Dim x As Integer = i \ Gdim
            For j As Integer = 0 To ny - 1
                Dim y As Integer = j \ Gdim
                Th(x, y) += Gv(i, j)
            Next
        Next
        '建立亮度塊狀圖
        Dim A(nx - 1, ny - 1) As Byte
        For i As Integer = 0 To kx - 1
            For j As Integer = 0 To ky - 1
                Th(i, j) /= Gdim * Gdim '區塊亮度平均值計算
                For ii As Integer = 0 To Gdim - 1
                    For jj As Integer = 0 To Gdim - 1
                        A(i * Gdim + ii, j * Gdim + jj) = Th(i, j)
                    Next
                Next
            Next
        Next
        PictureBox1.Image = GrayImg(A) '建立灰階圖
    End Sub
    '二值化
    Dim Z(,) As Byte '全圖二值化陣列
    Private Sub BinaryToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) _
        Handles BinaryToolStripMenuItem.Click
        ReDim Z(nx - 1, ny - 1)
        For i As Integer = 1 To nx - 2
            Dim x As Integer = i \ Gdim 'x座標換算
            For j As Integer = 1 To ny - 2
                Dim y As Integer = j \ Gdim 'y座標換算
                If Gv(i, j) < Th(x, y) Then
                    Z(i, j) = 1 '低於亮度門檻設為目標點
                End If
            Next
        Next
        PictureBox1.Image = BWImg(Z) '建立二值化圖
    End Sub
    '輪廓線
    Dim Q(,) As Byte '輪廓線陣列
    Private Sub OutlineToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) _
        Handles OutlineToolStripMenuItem.Click
        If IsNothing(Z) Then Exit Sub '無二值化圖忽略
        Q = Outline(Z) '建立輪廓點陣列
        PictureBox1.Image = BWImg(Q) '建立輪廓圖
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
    '選擇顯示某目標之輪廓線
    Private Sub PictureBox1_MouseDown(ByVal sender As Object, ByVal e As MouseEventArgs) _
        Handles PictureBox1.MouseDown
        If e.Button = MouseButtons.Left Then
            If IsNothing(Q) Then Exit Sub
            Dim x As Integer = e.X, y As Integer = e.Y
            '尋找左方最近之輪廓點
            Do While Q(x, y) = 0 And x > 0
                x -= 1
            Loop
            Dim A As ArrayList = getGrp(Q, x, y) '搜尋此目標所有輪廓點
            Dim bmp As Bitmap = BWImg(Q) '建立輪廓圖
            For k As Integer = 0 To A.Count - 1
                Dim p As Point = A(k)
                bmp.SetPixel(p.X, p.Y, Color.Red)
            Next
            PictureBox1.Image = bmp
        End If
    End Sub
    '氾濫式演算法取得某目標之輪廓點
    Function getGrp(ByVal q(,) As Byte, ByVal i As Integer, ByVal j As Integer) As ArrayList
        If q(i, j) = 0 Then Return New ArrayList
        Dim b(,) As Byte = q.Clone '建立輪廓點陣列副本
        Dim nc As New ArrayList '每一輪搜尋的起點集合
        nc.Add(New Point(i, j)) '輸入之搜尋起點
        b(i, j) = 0 '清除此起點之輪廓點標記
        Dim A As ArrayList = nc '此目標中所有目標點的集合
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
                        A.Add(k) '加入所有已蒐集到的目標點集合
                        b(ii, jj) = 0 '清除輪廓點點標記
                    Next
                Next
            Next
        Loop While nc.Count > 0 '此輪搜尋有新發現輪廓點時繼續搜尋
        Return A '回傳所有目標點的集合
    End Function

    Private Sub SaveImageToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) _
        Handles SaveImageToolStripMenuItem.Click
        If SaveFileDialog1.ShowDialog = Windows.Forms.DialogResult.OK Then
            PictureBox1.Image.Save(SaveFileDialog1.FileName)
        End If
    End Sub
End Class
