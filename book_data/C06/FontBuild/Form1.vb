Public Class Form1
    Dim Pw As Integer = 25, Ph As Integer = 50 '字模的寬與高
    Dim P(1, 35, Pw - 1, Ph - 1) As Byte '六與七碼車牌所有英數字二值化陣列
    Dim P69(1, Pw - 1, Ph - 1) As Byte '變形的六碼車牌6與9字型
    Dim A(Pw - 1, Ph - 1) As Byte, B(Pw - 1, Ph - 1) As Byte '待比對的字模與目標二值化陣列
    '用影像載入字模
    Private Sub Button1_Click(ByVal sender As Object, ByVal e As System.EventArgs) _
        Handles Button1.Click
        If FolderBrowserDialog1.ShowDialog = Windows.Forms.DialogResult.OK Then
            Dim d As String = FolderBrowserDialog1.SelectedPath
            For k As Integer = 0 To 35
                '六碼字型
                Dim b6 As New Bitmap(d + "\" + Format(k, "00") + ".gif")
                For i As Integer = 0 To Pw - 1
                    For j As Integer = 0 To Ph - 1
                        Dim C As Color = b6.GetPixel(i, j)
                        If C.R < 128 Then P(0, k, i, j) = 1 '黑點
                    Next
                Next
                '七碼字型
                Dim b7 As New Bitmap(d + "\" + Format(k + 36, "00") + ".gif")
                For i As Integer = 0 To Pw - 1
                    For j As Integer = 0 To Ph - 1
                        Dim C As Color = b7.GetPixel(i, j)
                        If C.R < 128 Then P(1, k, i, j) = 1 '黑點
                    Next
                Next
            Next
            For k As Integer = 72 To 73
                '六碼變形的69字型
                Dim b69 As New Bitmap(d + "\" + Format(k, "00") + ".gif")
                For i As Integer = 0 To Pw - 1
                    For j As Integer = 0 To Ph - 1
                        Dim C As Color = b69.GetPixel(i, j)
                        If C.R < 128 Then P69(k - 72, i, j) = 1 '黑點
                    Next
                Next
            Next
        End If
    End Sub
    '選擇字模
    Private Sub ListBox1_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) _
        Handles ListBox1.SelectedIndexChanged
        Dim knd As Integer = ListBox2.SelectedIndex '六或七碼
        If knd < 0 Then Exit Sub '未選擇
        Dim sC As Integer = ListBox1.SelectedIndex '選擇的字元
        If sC < 0 Then Exit Sub '未選擇
        If knd = 1 And sC > 35 Then Exit Sub '七碼車牌字元無變形的69
        ReDim B(Pw - 1, Ph - 1) '待比對的字模二值化陣列
        For i As Integer = 0 To Pw - 1
            For j As Integer = 0 To Ph - 1
                If knd = 0 Then '六碼字型
                    If sC <= 35 Then
                        B(i, j) = P(0, sC, i, j)
                    Else
                        B(i, j) = P69(sC - 36, i, j)
                    End If
                Else '七碼字型
                    B(i, j) = P(1, sC, i, j)
                End If
            Next
        Next
        PictureBox1.Image = BWImg(B) '顯示字模影像
    End Sub
    '建立字模二進位檔案
    Private Sub Button2_Click(ByVal sender As Object, ByVal e As EventArgs) _
        Handles Button2.Click
        If SaveFileDialog1.ShowDialog = Windows.Forms.DialogResult.OK Then
            If My.Computer.FileSystem.FileExists(SaveFileDialog1.FileName) Then
                My.Computer.FileSystem.DeleteFile(SaveFileDialog1.FileName)
            End If
            '寫出六與七碼字模
            For m As Integer = 0 To 1
                For k As Integer = 0 To 35
                    For j As Integer = 0 To Ph - 1
                        Dim x(Pw - 1) As Byte
                        For i As Integer = 0 To Pw - 1
                            x(i) = P(m, k, i, j)
                        Next
                        My.Computer.FileSystem.WriteAllBytes(SaveFileDialog1.FileName, x, True)
                    Next
                Next
            Next
            '寫出六碼變形69字模
            For k As Integer = 0 To 1
                For j As Integer = 0 To Ph - 1
                    Dim x(Pw - 1) As Byte
                    For i As Integer = 0 To Pw - 1
                        x(i) = P69(k, i, j)
                    Next
                    My.Computer.FileSystem.WriteAllBytes(SaveFileDialog1.FileName, x, True)
                Next
            Next
        End If
    End Sub
    '啟動程式載入字模
    Private Sub Form1_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
        FontLoad()
    End Sub
    Private Sub FontLoad()
        Dim q() As Byte = My.Resources.font
        Dim n As Integer = 0
        '匯入六與七碼字模
        For m As Integer = 0 To 1
            For k As Integer = 0 To 35
                For j As Integer = 0 To Ph - 1
                    For i As Integer = 0 To Pw - 1
                        P(m, k, i, j) = q(n)
                        n += 1
                    Next
                Next
            Next
        Next
        '匯入六碼變形69字模
        For k As Integer = 0 To 1
            For j As Integer = 0 To Ph - 1
                For i As Integer = 0 To Pw - 1
                    P69(k, i, j) = q(n)
                    n += 1
                Next
            Next
        Next
    End Sub
    '載入比對目標
    Private Sub PictureBox2_Click(ByVal sender As Object, ByVal e As System.EventArgs) _
        Handles PictureBox2.Click
        If OpenFileDialog1.ShowDialog = Windows.Forms.DialogResult.OK Then
            Dim m As New Bitmap(OpenFileDialog1.FileName)
            PictureBox2.Image = m
            ReDim A(Pw - 1, Ph - 1) '待比對的目標二值化陣列
            For i As Integer = 0 To Pw - 1
                For j As Integer = 0 To Ph - 1
                    Dim C As Color = m.GetPixel(i, j)
                    If C.R < 127 Then A(i, j) = 1 '黑點
                Next
            Next
        End If
    End Sub
    '執行字模比對
    Private Sub Button3_Click(ByVal sender As Object, ByVal e As System.EventArgs) _
        Handles Button3.Click
        Dim n0 As Integer = 0 '字模黑點數
        Dim nf As Integer = 0 '符合的黑點數
        For i As Integer = 0 To Pw - 1
            For j As Integer = 0 To Ph - 1
                If B(i, j) = 0 Then
                    If A(i, j) = 1 Then nf -= 1 '目標與字模不符合扣分
                Else
                    n0 += 1 '字模黑點數累計
                    If A(i, j) = 1 Then nf += 1 '目標與字模符合加分
                End If
            Next
        Next
        Dim pc As Integer = nf * 1000 / n0 '符合點數千分比
        Label1.Text = pc.ToString
    End Sub
End Class

