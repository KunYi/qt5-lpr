Public Class Form1
    Dim c(,) As Byte '監視區圓周標記點
    Dim d(,) As Byte '浮萍目標點
    Dim RTg As TgInfo '邊界標記之目標物件
    '目標物件結構
    Public Structure TgInfo
        Dim np As Integer '目標點數
        Dim P As ArrayList '目標點的集合
        Dim xmn As Short, xmx As Short, ymn As Short, ymx As Short '四面座標極值
        Dim width As Integer, height As Integer '寬與高
        Dim cx As Integer, cy As Integer '中心點座標
    End Structure
    '開啟檔案
    Private Sub OpenToolStripMenuItem_Click(ByVal sender As Object, ByVal e As System.EventArgs) _
        Handles OpenToolStripMenuItem.Click
        If OpenFileDialog1.ShowDialog = Windows.Forms.DialogResult.OK Then
            Dim bmp As New Bitmap(OpenFileDialog1.FileName)
            Bmp2RGB(bmp) '讀取RGB亮度陣列
            PictureBox1.Image = bmp '顯示
        End If
    End Sub
    '浮萍二值化
    Private Sub LeafToolStripMenuItem_Click(ByVal sender As Object, ByVal e As System.EventArgs) _
        Handles LeafToolStripMenuItem.Click
        ReDim d(nx - 1, ny - 1) '浮萍目標點陣列
        For i As Integer = 1 To nx - 2
            For j As Integer = 1 To ny - 2
                If Gv(i, j) > 70 And CInt(Gv(i, j)) - Bv(i, j) > 50 Then
                    d(i, j) = 1  '浮萍(偏綠色點)
                End If
            Next
        Next
        PictureBox1.Image = BWImg(d) '建立浮萍二值化圖
    End Sub
    '水面圓周邊界二值化
    Private Sub RimToolStripMenuItem_Click(ByVal sender As Object, ByVal e As System.EventArgs) _
        Handles RimToolStripMenuItem.Click
        ReDim c(nx - 1, ny - 1) '圓周標記目標點陣列
        For i As Integer = 1 To nx - 2
            For j As Integer = 1 To ny - 2
                If Rv(i, j) > 70 And Rv(i, j) > Gv(i, j) And Rv(i, j) > Bv(i, j) Then '偏紅色點
                    c(i, j) = 1 '邊界
                End If
            Next
        Next
        Dim bmp As Bitmap = BWImg(c) '建立邊界二值化圖
        PictureBox1.Image = bmp '顯示
    End Sub
    '取得邊界(紅色圓圈)目標
    Private Sub RimTgToolStripMenuItem_Click(ByVal sender As Object, ByVal e As System.EventArgs) _
        Handles RimTgToolStripMenuItem.Click
        Dim T As TgInfo = getMaxTg(c) '取得最大紅色區塊目標
        '繪製圓周目標
        Dim bmp As New Bitmap(nx - 1, ny - 1)
        For k As Integer = 0 To T.P.Count - 1
            Dim pt As Point = T.P(k)
            bmp.SetPixel(pt.X, pt.Y, Color.Black)
        Next
        PictureBox1.Image = bmp '顯示
    End Sub
    '取得最大目標
    Function getMaxTg(ByVal q(,) As Byte) As TgInfo
        Dim mx As Integer = 0 '最多的目標點數
        RTg = New TgInfo '最大的圓周目標
        Dim b(,) As Byte = q.Clone '建立目標點陣列副本
        For i As Integer = 1 To nx - 2
            For j As Integer = 1 To ny - 2
                If b(i, j) = 0 Then Continue For
                Dim G As New TgInfo
                G.xmn = i : G.xmx = i : G.ymn = j : G.ymx = j : G.np = 1 : G.P = New ArrayList
                Dim nc As New ArrayList '每一輪搜尋的起點集合
                nc.Add(New Point(i, j)) '輸入之搜尋起點
                b(i, j) = 0 '清除此起點之標記
                Do
                    Dim nb As ArrayList = nc.Clone '複製此輪之搜尋起點集合
                    nc = New ArrayList '清除準備蒐集下一輪搜尋起點之集合
                    For m As Integer = 0 To nb.Count - 1
                        Dim p As Point = nb(m) '搜尋起點
                        '在此點周邊3X3區域內找目標點
                        For ii As Integer = p.X - 1 To p.X + 1
                            For jj As Integer = p.Y - 1 To p.Y + 1
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
                Loop While nc.Count > 0 '此輪搜尋有新發現目標點時繼續搜尋
                If q(i - 1, j) = 1 Then Continue For '排除白色區塊的負目標，起點左邊是黑點
                If G.np > mx Then '發現更大目標時
                    mx = G.np
                    G.width = G.xmx - G.xmn + 1 : G.height = G.ymx - G.ymn + 1
                    G.cx = (G.xmn + G.xmx) / 2 : G.cy = (G.ymn + G.ymx) / 2
                    RTg = G
                End If
            Next
        Next
        Return RTg '回傳最大目標
    End Function
    '計算與繪製結果→圓周內之浮萍面積
    Private Sub LeafRimToolStripMenuItem_Click(ByVal sender As Object, ByVal e As System.EventArgs) _
        Handles LeafRimToolStripMenuItem.Click
        Dim r As Double = 0 '邊界圓圈之半徑
        For k As Integer = 0 To RTg.P.Count - 1
            Dim p As Point = RTg.P(k)
            Dim rr As Double = Math.Sqrt((p.X - RTg.cx) ^ 2 + (p.Y - RTg.cy) ^ 2)
            r += rr
        Next
        r /= RTg.P.Count '平均半徑
        Dim A(nx - 1, ny - 1) As Byte, nLf As Integer = 0
        For i As Integer = RTg.xmn To RTg.xmx
            For j As Integer = RTg.ymn To RTg.ymx
                If d(i, j) = 0 Then Continue For
                Dim rr As Double = Math.Sqrt((i - RTg.cx) ^ 2 + (j - RTg.cy) ^ 2) '浮萍目標到中心距離
                If rr > r Then Continue For '目標點在監視區外
                A(i, j) = 1 : nLf += 1 '監視範圍內→計量
            Next
        Next
        Dim bmp As Bitmap = BWImg(A) '建立監視區內浮萍二值化圖
        Dim G As Graphics = Graphics.FromImage(bmp)
        G.DrawEllipse(Pens.Red, RTg.xmn, RTg.ymn, RTg.width, RTg.height) '繪製監視區邊界圓周
        PictureBox1.Image = bmp '顯示
        Dim Area As Double = Math.PI * r ^ 2 '監視總面積→畫素單位
        Dim pc As Double = nLf * 100 / Area '浮萍面積百分比
        MsgBox(Format(pc, "##.###") + "%")
    End Sub
    '儲存影像
    Private Sub SaveImageToolStripMenuItem_Click(ByVal sender As Object, ByVal e As System.EventArgs) _
        Handles SaveImageToolStripMenuItem.Click
        If SaveFileDialog1.ShowDialog = Windows.Forms.DialogResult.OK Then
            PictureBox1.Image.Save(SaveFileDialog1.FileName)
        End If
    End Sub
End Class
