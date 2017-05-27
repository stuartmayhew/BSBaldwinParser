<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class fmMain
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing AndAlso components IsNot Nothing Then
            components.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(fmMain))
        Me.ListBox1 = New System.Windows.Forms.ListBox()
        Me.btnBaldwin = New System.Windows.Forms.Button()
        Me.tbEndInst = New System.Windows.Forms.TextBox()
        Me.tbStartInst = New System.Windows.Forms.TextBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.dpTBSDate = New System.Windows.Forms.DateTimePicker()
        Me.btnClose = New System.Windows.Forms.Button()
        Me.Button2 = New System.Windows.Forms.Button()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.WebBrowser1 = New System.Windows.Forms.WebBrowser()
        Me.cbScanText = New System.Windows.Forms.CheckBox()
        Me.Panel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'ListBox1
        '
        Me.ListBox1.FormattingEnabled = True
        Me.ListBox1.Location = New System.Drawing.Point(26, 106)
        Me.ListBox1.Name = "ListBox1"
        Me.ListBox1.Size = New System.Drawing.Size(366, 368)
        Me.ListBox1.TabIndex = 0
        '
        'btnBaldwin
        '
        Me.btnBaldwin.Location = New System.Drawing.Point(327, 67)
        Me.btnBaldwin.Name = "btnBaldwin"
        Me.btnBaldwin.Size = New System.Drawing.Size(75, 21)
        Me.btnBaldwin.TabIndex = 6
        Me.btnBaldwin.Text = "Get Baldwin"
        Me.btnBaldwin.UseVisualStyleBackColor = True
        '
        'tbEndInst
        '
        Me.tbEndInst.Location = New System.Drawing.Point(192, 68)
        Me.tbEndInst.Name = "tbEndInst"
        Me.tbEndInst.Size = New System.Drawing.Size(100, 20)
        Me.tbEndInst.TabIndex = 5
        Me.tbEndInst.Text = "1631551"
        '
        'tbStartInst
        '
        Me.tbStartInst.Location = New System.Drawing.Point(59, 71)
        Me.tbStartInst.Name = "tbStartInst"
        Me.tbStartInst.Size = New System.Drawing.Size(100, 20)
        Me.tbStartInst.TabIndex = 4
        Me.tbStartInst.Text = "1631549"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(14, 71)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(27, 13)
        Me.Label2.TabIndex = 9
        Me.Label2.Text = "Instr"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(171, 71)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(16, 13)
        Me.Label3.TabIndex = 10
        Me.Label3.Text = "to"
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(209, 17)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(54, 13)
        Me.Label6.TabIndex = 14
        Me.Label6.Text = "TBS Date"
        '
        'dpTBSDate
        '
        Me.dpTBSDate.Format = System.Windows.Forms.DateTimePickerFormat.[Short]
        Me.dpTBSDate.Location = New System.Drawing.Point(269, 12)
        Me.dpTBSDate.Name = "dpTBSDate"
        Me.dpTBSDate.Size = New System.Drawing.Size(97, 20)
        Me.dpTBSDate.TabIndex = 15
        Me.dpTBSDate.Value = New Date(2017, 5, 29, 0, 0, 0, 0)
        '
        'btnClose
        '
        Me.btnClose.Location = New System.Drawing.Point(308, 499)
        Me.btnClose.Name = "btnClose"
        Me.btnClose.Size = New System.Drawing.Size(75, 23)
        Me.btnClose.TabIndex = 17
        Me.btnClose.Text = "Close"
        Me.btnClose.UseVisualStyleBackColor = True
        '
        'Button2
        '
        Me.Button2.BackColor = System.Drawing.Color.Red
        Me.Button2.Location = New System.Drawing.Point(17, 13)
        Me.Button2.Name = "Button2"
        Me.Button2.Size = New System.Drawing.Size(75, 23)
        Me.Button2.TabIndex = 18
        Me.Button2.Text = "Abort"
        Me.Button2.UseVisualStyleBackColor = False
        '
        'Panel1
        '
        Me.Panel1.Controls.Add(Me.WebBrowser1)
        Me.Panel1.Location = New System.Drawing.Point(439, 0)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(410, 542)
        Me.Panel1.TabIndex = 19
        '
        'WebBrowser1
        '
        Me.WebBrowser1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.WebBrowser1.Location = New System.Drawing.Point(0, 0)
        Me.WebBrowser1.MinimumSize = New System.Drawing.Size(20, 20)
        Me.WebBrowser1.Name = "WebBrowser1"
        Me.WebBrowser1.Size = New System.Drawing.Size(410, 542)
        Me.WebBrowser1.TabIndex = 0
        '
        'cbScanText
        '
        Me.cbScanText.AutoSize = True
        Me.cbScanText.Checked = True
        Me.cbScanText.CheckState = System.Windows.Forms.CheckState.Checked
        Me.cbScanText.Location = New System.Drawing.Point(26, 499)
        Me.cbScanText.Name = "cbScanText"
        Me.cbScanText.Size = New System.Drawing.Size(98, 17)
        Me.cbScanText.TabIndex = 20
        Me.cbScanText.Text = "Scan Doc Text"
        Me.cbScanText.UseVisualStyleBackColor = True
        '
        'fmMain
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(438, 538)
        Me.ControlBox = False
        Me.Controls.Add(Me.cbScanText)
        Me.Controls.Add(Me.Panel1)
        Me.Controls.Add(Me.Button2)
        Me.Controls.Add(Me.btnClose)
        Me.Controls.Add(Me.dpTBSDate)
        Me.Controls.Add(Me.Label6)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.btnBaldwin)
        Me.Controls.Add(Me.tbEndInst)
        Me.Controls.Add(Me.tbStartInst)
        Me.Controls.Add(Me.ListBox1)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.Name = "fmMain"
        Me.Text = "Parser"
        Me.Panel1.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents ListBox1 As System.Windows.Forms.ListBox
    Friend WithEvents btnBaldwin As System.Windows.Forms.Button
    Friend WithEvents tbEndInst As System.Windows.Forms.TextBox
    Friend WithEvents tbStartInst As System.Windows.Forms.TextBox
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents btnClose As System.Windows.Forms.Button
    Friend WithEvents Button2 As System.Windows.Forms.Button
    Protected WithEvents dpTBSDate As System.Windows.Forms.DateTimePicker
    Friend WithEvents Panel1 As System.Windows.Forms.Panel
    Friend WithEvents WebBrowser1 As System.Windows.Forms.WebBrowser
    Friend WithEvents cbScanText As System.Windows.Forms.CheckBox
End Class
