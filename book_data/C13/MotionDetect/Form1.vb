Public Class Form1
    Dim B1(,) As Byte, B2(,) As Byte '連續影像的灰階陣列
    Dim Type As Integer = 0 '顯示模式：0→原圖，1→灰階，2→二值化
    Dim F2 As New Form2 '監看範圍設定框
    '啟動程式顯示監看範圍設定框
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        F2.Show()
    End Sub
    '啟動監看
    Private Sub CopyScreenToolStripMenuItem_Click(sender As Object, e As EventArgs) _
        Handles CopyScreentToolStripMenuItem.Click
        F2.Hide() '隱藏設定框
        Timer1.Start() '啟動螢幕拷貝
    End Sub
    '動態監看迴圈
    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        '拷貝螢幕指定範圍
        Dim bmp As New Bitmap(F2.Width, F2.Height)
        Dim G As Graphics = Graphics.FromImage(bmp)
        G.CopyFromScreen(F2.Location, New Point(0, 0), F2.Size)
        Bmp2RGB(bmp) '取得影像陣列
        If IsNothing(B1) Then
            B1 = Gv : B2 = Gv '首張影像
        Else
            B1 = B2.Clone : B2 = Gv.Clone  '舊影像推移
        End If
        Select Case Type
            Case 0
                PictureBox1.Image = bmp'原圖顯示
            Case 1
                PictureBox1.Image = GrayDiff()'差異灰階顯示
            Case 2
                PictureBox1.Image = BinDiff() '差異二值化顯示
        End Select
    End Sub
    '灰階差異
    Private Sub DifferenceToolStripMenuItem_Click(sender As Object, e As EventArgs) _
        Handles DifferenceToolStripMenuItem.Click
        Type = 1
    End Sub
    '灰階動態圖
    Private Function GrayDiff() As Bitmap
        Dim D(nx - 1, ny - 1) As Byte
        For i As Integer = 0 To nx - 1
            For j As Integer = 0 To ny - 1
                D(i, j) = 255 - Math.Abs(CInt(B1(i, j)) - B2(i, j))
            Next
        Next
        Return GrayImg(D)
    End Function
    '高差異點二值化
    Private Sub BinaryToolStripMenuItem_Click(sender As Object, e As EventArgs) _
        Handles BinaryToolStripMenuItem.Click
        Type = 2
    End Sub
    '二值化動態圖
    Private Function BinDiff() As Bitmap
        Dim Th As Integer = 10
        Dim D(nx - 1, ny - 1) As Byte
        For i As Integer = 0 To nx - 1
            For j As Integer = 0 To ny - 1
                Dim dd As Integer = Math.Abs(CInt(B1(i, j)) - B2(i, j))
                If dd > Th Then D(i, j) = 1
            Next
        Next
        Return BWImg(D)
    End Function
End Class
