Public Class Form1
    '開啟檔案
    Private Sub OpenToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) _
        Handles OpenToolStripMenuItem.Click
        If OpenFileDialog1.ShowDialog = Windows.Forms.DialogResult.OK Then
            Dim bmp As New Bitmap(OpenFileDialog1.FileName)
            Bmp2RGB(bmp) '讀取RGB亮度陣列
            PictureBox1.Image = bmp '顯示
        End If
    End Sub
    '以紅光為灰階
    Private Sub RedToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) _
        Handles RedToolStripMenuItem.Click
        PictureBox1.Image = GrayImg(Rv) '建立灰階圖
    End Sub
    '以綠光為灰階
    Private Sub GreenToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) _
        Handles GreenToolStripMenuItem.Click
        PictureBox1.Image = GrayImg(Gv) '建立灰階圖
    End Sub
    '以藍光為灰階
    Private Sub BlueToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) _
        Handles BlueToolStripMenuItem.Click
        PictureBox1.Image = GrayImg(Bv) '建立灰階圖
    End Sub
    '以RGB整合亮度為灰階
    Private Sub RGBToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) _
        Handles RGBToolStripMenuItem.Click
        Dim A(nx - 1, ny - 1) As Byte
        For i As Integer = 0 To nx - 1
            For j As Integer = 0 To ny - 1
                Dim Gray As Integer = Rv(i, j) * 0.299 + Gv(i, j) * 0.587 + Bv(i, j) * 0.114
                A(i, j) = Gray
            Next
        Next
        PictureBox1.Image = GrayImg(A) '建立灰階圖
    End Sub
    '選擇紅綠光之較暗亮度為灰階
    Private Sub RGLowToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) _
        Handles RGLowToolStripMenuItem.Click
        Dim A(nx - 1, ny - 1) As Byte
        For i As Integer = 0 To nx - 1
            For j As Integer = 0 To ny - 1
                If Rv(i, j) > Gv(i, j) Then
                    A(i, j) = Gv(i, j)
                Else
                    A(i, j) = Rv(i, j)
                End If
            Next
        Next
        PictureBox1.Image = GrayImg(A) '建立灰階圖
    End Sub
    '二值化
    Private Sub BinaryToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) _
        Handles BinaryToolStripMenuItem.Click
        Dim A(nx - 1, ny - 1) As Byte
        For i As Integer = 1 To nx - 2
            For j As Integer = 1 To ny - 2
                If Gv(i, j) < 128 Then A(i, j) = 1
            Next
        Next
        PictureBox1.Image = BWImg(A) '建立二值化圖
    End Sub
    '負片
    Private Sub NegativeToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) _
        Handles NegativeToolStripMenuItem.Click
        Dim A(nx - 1, ny - 1) As Byte
        For i As Integer = 0 To nx - 1
            For j As Integer = 0 To ny - 1
                A(i, j) = 255 - Gv(i, j)
            Next
        Next
        PictureBox1.Image = GrayImg(A) '建立灰階圖
    End Sub
    '儲存影像
    Private Sub SaveImageToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) _
        Handles SaveImageToolStripMenuItem.Click
        If SaveFileDialog1.ShowDialog = Windows.Forms.DialogResult.OK Then
            PictureBox1.Image.Save(SaveFileDialog1.FileName)
        End If
    End Sub
End Class
