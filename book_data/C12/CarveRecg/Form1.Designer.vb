<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Form 覆寫 Dispose 以清除元件清單。
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    '為 Windows Form 設計工具的必要項
    Private components As System.ComponentModel.IContainer

    '注意: 以下為 Windows Form 設計工具所需的程序
    '可以使用 Windows Form 設計工具進行修改。
    '請勿使用程式碼編輯器進行修改。
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Form1))
        Me.PictureBox1 = New System.Windows.Forms.PictureBox()
        Me.ContextMenuStrip1 = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.SaveImageToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuStrip1 = New System.Windows.Forms.MenuStrip()
        Me.OpenToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.IntegralToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.YBoundToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.BinaryToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.TargetsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.MergeTgsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.BestFitToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.OpenFileDialog1 = New System.Windows.Forms.OpenFileDialog()
        Me.SaveFileDialog1 = New System.Windows.Forms.SaveFileDialog()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.ContextMenuStrip1.SuspendLayout()
        Me.MenuStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'PictureBox1
        '
        Me.PictureBox1.ContextMenuStrip = Me.ContextMenuStrip1
        Me.PictureBox1.Location = New System.Drawing.Point(0, 28)
        Me.PictureBox1.Name = "PictureBox1"
        Me.PictureBox1.Size = New System.Drawing.Size(640, 400)
        Me.PictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize
        Me.PictureBox1.TabIndex = 3
        Me.PictureBox1.TabStop = False
        '
        'ContextMenuStrip1
        '
        Me.ContextMenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.SaveImageToolStripMenuItem})
        Me.ContextMenuStrip1.Name = "ContextMenuStrip1"
        Me.ContextMenuStrip1.Size = New System.Drawing.Size(141, 26)
        '
        'SaveImageToolStripMenuItem
        '
        Me.SaveImageToolStripMenuItem.Name = "SaveImageToolStripMenuItem"
        Me.SaveImageToolStripMenuItem.Size = New System.Drawing.Size(140, 22)
        Me.SaveImageToolStripMenuItem.Text = "Save Image"
        '
        'MenuStrip1
        '
        Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.OpenToolStripMenuItem, Me.IntegralToolStripMenuItem, Me.YBoundToolStripMenuItem, Me.BinaryToolStripMenuItem, Me.TargetsToolStripMenuItem, Me.MergeTgsToolStripMenuItem, Me.BestFitToolStripMenuItem})
        Me.MenuStrip1.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip1.Name = "MenuStrip1"
        Me.MenuStrip1.Size = New System.Drawing.Size(640, 24)
        Me.MenuStrip1.TabIndex = 2
        Me.MenuStrip1.Text = "MenuStrip1"
        '
        'OpenToolStripMenuItem
        '
        Me.OpenToolStripMenuItem.Name = "OpenToolStripMenuItem"
        Me.OpenToolStripMenuItem.Size = New System.Drawing.Size(51, 20)
        Me.OpenToolStripMenuItem.Text = "Open"
        '
        'IntegralToolStripMenuItem
        '
        Me.IntegralToolStripMenuItem.Name = "IntegralToolStripMenuItem"
        Me.IntegralToolStripMenuItem.Size = New System.Drawing.Size(62, 20)
        Me.IntegralToolStripMenuItem.Text = "Integral"
        '
        'YBoundToolStripMenuItem
        '
        Me.YBoundToolStripMenuItem.Name = "YBoundToolStripMenuItem"
        Me.YBoundToolStripMenuItem.Size = New System.Drawing.Size(67, 20)
        Me.YBoundToolStripMenuItem.Text = "Y bound"
        '
        'BinaryToolStripMenuItem
        '
        Me.BinaryToolStripMenuItem.Name = "BinaryToolStripMenuItem"
        Me.BinaryToolStripMenuItem.Size = New System.Drawing.Size(53, 20)
        Me.BinaryToolStripMenuItem.Text = "Binary"
        '
        'TargetsToolStripMenuItem
        '
        Me.TargetsToolStripMenuItem.Name = "TargetsToolStripMenuItem"
        Me.TargetsToolStripMenuItem.Size = New System.Drawing.Size(61, 20)
        Me.TargetsToolStripMenuItem.Text = "Targets"
        '
        'MergeTgsToolStripMenuItem
        '
        Me.MergeTgsToolStripMenuItem.Name = "MergeTgsToolStripMenuItem"
        Me.MergeTgsToolStripMenuItem.Size = New System.Drawing.Size(80, 20)
        Me.MergeTgsToolStripMenuItem.Text = "Merge Tgs"
        '
        'BestFitToolStripMenuItem
        '
        Me.BestFitToolStripMenuItem.Name = "BestFitToolStripMenuItem"
        Me.BestFitToolStripMenuItem.Size = New System.Drawing.Size(58, 20)
        Me.BestFitToolStripMenuItem.Text = "Best Fit"
        '
        'SaveFileDialog1
        '
        Me.SaveFileDialog1.Filter = "*.png|*.png"
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(640, 428)
        Me.Controls.Add(Me.PictureBox1)
        Me.Controls.Add(Me.MenuStrip1)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "Form1"
        Me.Text = "Carved Character Recognization"
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ContextMenuStrip1.ResumeLayout(False)
        Me.MenuStrip1.ResumeLayout(False)
        Me.MenuStrip1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents PictureBox1 As PictureBox
    Friend WithEvents ContextMenuStrip1 As ContextMenuStrip
    Friend WithEvents SaveImageToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents MenuStrip1 As MenuStrip
    Friend WithEvents OpenToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents IntegralToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents YBoundToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents BinaryToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents TargetsToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents MergeTgsToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents BestFitToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents OpenFileDialog1 As OpenFileDialog
    Friend WithEvents SaveFileDialog1 As SaveFileDialog
End Class
