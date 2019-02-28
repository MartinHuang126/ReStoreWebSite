<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class GatherWebFrm
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.btn_retry = New System.Windows.Forms.Button()
        Me.txt_Detail = New System.Windows.Forms.TextBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.NumericUpDown1 = New System.Windows.Forms.NumericUpDown()
        Me.txt_WaitList = New System.Windows.Forms.TextBox()
        Me.txt_DoingList = New System.Windows.Forms.TextBox()
        Me.txt_Complete = New System.Windows.Forms.TextBox()
        Me.btn_Add = New System.Windows.Forms.Button()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.NumericUpDown2 = New System.Windows.Forms.NumericUpDown()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.txt_Add = New System.Windows.Forms.TextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.cbo_encoding = New System.Windows.Forms.ComboBox()
        CType(Me.NumericUpDown1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.NumericUpDown2, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'btn_retry
        '
        Me.btn_retry.Location = New System.Drawing.Point(793, 175)
        Me.btn_retry.Name = "btn_retry"
        Me.btn_retry.Size = New System.Drawing.Size(75, 23)
        Me.btn_retry.TabIndex = 2
        Me.btn_retry.Text = "ReTry"
        Me.btn_retry.UseVisualStyleBackColor = True
        '
        'txt_Detail
        '
        Me.txt_Detail.Location = New System.Drawing.Point(28, 407)
        Me.txt_Detail.Multiline = True
        Me.txt_Detail.Name = "txt_Detail"
        Me.txt_Detail.ReadOnly = True
        Me.txt_Detail.ScrollBars = System.Windows.Forms.ScrollBars.Both
        Me.txt_Detail.Size = New System.Drawing.Size(861, 196)
        Me.txt_Detail.TabIndex = 4
        Me.txt_Detail.WordWrap = False
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(669, 83)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(36, 13)
        Me.Label3.TabIndex = 5
        Me.Label3.Text = "Level:"
        '
        'NumericUpDown1
        '
        Me.NumericUpDown1.Location = New System.Drawing.Point(727, 83)
        Me.NumericUpDown1.Maximum = New Decimal(New Integer() {8, 0, 0, 0})
        Me.NumericUpDown1.Name = "NumericUpDown1"
        Me.NumericUpDown1.Size = New System.Drawing.Size(45, 20)
        Me.NumericUpDown1.TabIndex = 6
        Me.NumericUpDown1.Value = New Decimal(New Integer() {5, 0, 0, 0})
        '
        'txt_WaitList
        '
        Me.txt_WaitList.Location = New System.Drawing.Point(28, 204)
        Me.txt_WaitList.Multiline = True
        Me.txt_WaitList.Name = "txt_WaitList"
        Me.txt_WaitList.ReadOnly = True
        Me.txt_WaitList.ScrollBars = System.Windows.Forms.ScrollBars.Both
        Me.txt_WaitList.Size = New System.Drawing.Size(275, 201)
        Me.txt_WaitList.TabIndex = 7
        Me.txt_WaitList.WordWrap = False
        '
        'txt_DoingList
        '
        Me.txt_DoingList.Location = New System.Drawing.Point(309, 204)
        Me.txt_DoingList.Multiline = True
        Me.txt_DoingList.Name = "txt_DoingList"
        Me.txt_DoingList.ReadOnly = True
        Me.txt_DoingList.ScrollBars = System.Windows.Forms.ScrollBars.Both
        Me.txt_DoingList.Size = New System.Drawing.Size(300, 201)
        Me.txt_DoingList.TabIndex = 8
        Me.txt_DoingList.WordWrap = False
        '
        'txt_Complete
        '
        Me.txt_Complete.Location = New System.Drawing.Point(615, 204)
        Me.txt_Complete.Multiline = True
        Me.txt_Complete.Name = "txt_Complete"
        Me.txt_Complete.ReadOnly = True
        Me.txt_Complete.ScrollBars = System.Windows.Forms.ScrollBars.Both
        Me.txt_Complete.Size = New System.Drawing.Size(274, 201)
        Me.txt_Complete.TabIndex = 9
        Me.txt_Complete.WordWrap = False
        '
        'btn_Add
        '
        Me.btn_Add.Location = New System.Drawing.Point(228, 63)
        Me.btn_Add.Name = "btn_Add"
        Me.btn_Add.Size = New System.Drawing.Size(75, 23)
        Me.btn_Add.TabIndex = 10
        Me.btn_Add.Text = "ADD"
        Me.btn_Add.UseVisualStyleBackColor = True
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(647, 30)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(58, 13)
        Me.Label4.TabIndex = 11
        Me.Label4.Text = "MaxDoing:"
        '
        'NumericUpDown2
        '
        Me.NumericUpDown2.Location = New System.Drawing.Point(727, 30)
        Me.NumericUpDown2.Maximum = New Decimal(New Integer() {8, 0, 0, 0})
        Me.NumericUpDown2.Minimum = New Decimal(New Integer() {1, 0, 0, 0})
        Me.NumericUpDown2.Name = "NumericUpDown2"
        Me.NumericUpDown2.Size = New System.Drawing.Size(45, 20)
        Me.NumericUpDown2.TabIndex = 12
        Me.NumericUpDown2.Value = New Decimal(New Integer() {5, 0, 0, 0})
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(91, 188)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(52, 13)
        Me.Label5.TabIndex = 13
        Me.Label5.Tag = ""
        Me.Label5.Text = "Waiting..."
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(417, 188)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(44, 13)
        Me.Label6.TabIndex = 14
        Me.Label6.Text = "Doing..." & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10)
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Location = New System.Drawing.Point(724, 188)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(54, 13)
        Me.Label7.TabIndex = 15
        Me.Label7.Text = "Complete!" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10)
        '
        'txt_Add
        '
        Me.txt_Add.Location = New System.Drawing.Point(309, 12)
        Me.txt_Add.Multiline = True
        Me.txt_Add.Name = "txt_Add"
        Me.txt_Add.ScrollBars = System.Windows.Forms.ScrollBars.Both
        Me.txt_Add.Size = New System.Drawing.Size(300, 173)
        Me.txt_Add.TabIndex = 16
        Me.txt_Add.WordWrap = False
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(650, 132)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(55, 13)
        Me.Label1.TabIndex = 17
        Me.Label1.Text = "Encoding:"
        '
        'cbo_encoding
        '
        Me.cbo_encoding.DisplayMember = "UTF-8"
        Me.cbo_encoding.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cbo_encoding.FormattingEnabled = True
        Me.cbo_encoding.Items.AddRange(New Object() {"UTF-8", "Default"})
        Me.cbo_encoding.Location = New System.Drawing.Point(727, 132)
        Me.cbo_encoding.Name = "cbo_encoding"
        Me.cbo_encoding.Size = New System.Drawing.Size(121, 21)
        Me.cbo_encoding.TabIndex = 18
        Me.cbo_encoding.ValueMember = "1"
        '
        'GatherWebFrm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(927, 615)
        Me.Controls.Add(Me.cbo_encoding)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.txt_Add)
        Me.Controls.Add(Me.Label7)
        Me.Controls.Add(Me.Label6)
        Me.Controls.Add(Me.Label5)
        Me.Controls.Add(Me.NumericUpDown2)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.btn_Add)
        Me.Controls.Add(Me.txt_Complete)
        Me.Controls.Add(Me.txt_DoingList)
        Me.Controls.Add(Me.txt_WaitList)
        Me.Controls.Add(Me.NumericUpDown1)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.txt_Detail)
        Me.Controls.Add(Me.btn_retry)
        Me.Name = "GatherWebFrm"
        Me.Text = "GatherWebTool"
        CType(Me.NumericUpDown1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.NumericUpDown2, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents btn_retry As Button
    Friend WithEvents txt_Detail As TextBox
    Friend WithEvents Label3 As Label
    Friend WithEvents NumericUpDown1 As NumericUpDown
    Friend WithEvents txt_WaitList As TextBox
    Friend WithEvents txt_DoingList As TextBox
    Friend WithEvents txt_Complete As TextBox
    Friend WithEvents Label4 As Label
    Friend WithEvents NumericUpDown2 As NumericUpDown
    Friend WithEvents Label5 As Label
    Friend WithEvents Label6 As Label
    Friend WithEvents Label7 As Label
    Friend WithEvents btn_Add As Button
    Friend WithEvents txt_Add As TextBox
    Friend WithEvents Label1 As Label
    Friend WithEvents cbo_encoding As ComboBox
End Class
