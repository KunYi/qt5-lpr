Imports System.Drawing.Drawing2D '匯入2D繪圖模組
Public Class Form2
    '開啟視窗，建立一個鏤空半透明外觀的視窗
    Private Sub Form2_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ClipWindow() '鏤空視窗
    End Sub
    '鏤空視窗
    Private Sub ClipWindow()
        '影像辨識區透空處理
        Dim path As New GraphicsPath
        Dim pt(9) As Point
        pt(0).X = 0 : pt(0).Y = 0 '左上角
        pt(1).X = Me.Width : pt(1).Y = 0 '右上角
        pt(2).X = Me.Width : pt(2).Y = Me.Height '右下角
        pt(3).X = 0 : pt(3).Y = Me.Height '左下角
        pt(4).X = 0 : pt(4).Y = Me.Height - 10
        pt(5).X = Me.Width - 10 : pt(5).Y = Me.Height - 10
        pt(6).X = Me.Width - 10 : pt(6).Y = 10
        pt(7).X = 10 : pt(7).Y = 10
        pt(8).X = 10 : pt(8).Y = Me.Height - 10
        pt(9).X = 0 : pt(9).Y = Me.Height - 10
        path.AddPolygon(pt) '以多邊形的方式加入path
        Me.Region = New Region(path) 'Region視窗區域
    End Sub
    '拖曳視窗功能，定義視窗位置
    Dim mdp As Point
    Private Sub Form2_MouseDown(sender As Object, e As MouseEventArgs) _
        Handles Me.MouseDown
        mdp = e.Location
    End Sub
    Private Sub Form2_MouseMove(sender As Object, e As MouseEventArgs) _
        Handles Me.MouseMove
        If e.Button = Windows.Forms.MouseButtons.Left Then
            Me.Location += e.Location - mdp
        End If
    End Sub
    '拖曳右下角功能，定義視窗寬高大小
    Private Sub Label1_MouseDown(sender As Object, e As MouseEventArgs) _
        Handles Label1.MouseDown
        mdp = e.Location
    End Sub
    Private Sub Label1_MouseMove(sender As Object, e As MouseEventArgs) _
        Handles Label1.MouseMove
        If e.Button = MouseButtons.Left Then
            Label1.Location += e.Location - mdp
            Me.Width = Label1.Right : Me.Height = Label1.Bottom
            Me.Refresh()
            ClipWindow()
        End If
    End Sub
End Class