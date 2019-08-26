<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class SEOLinksFrm
    Inherits System.Windows.Forms.Form

    'Form 重写 Dispose，以清理组件列表。
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

    'Windows 窗体设计器所必需的
    Private components As System.ComponentModel.IContainer

    '注意: 以下过程是 Windows 窗体设计器所必需的
    '可以使用 Windows 窗体设计器修改它。  
    '不要使用代码编辑器修改它。
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.btn_choose = New System.Windows.Forms.Button()
        Me.txt_folderPath = New System.Windows.Forms.TextBox()
        Me.txt_replace = New System.Windows.Forms.TextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.btn_select = New System.Windows.Forms.Button()
        Me.btn_update = New System.Windows.Forms.Button()
        Me.FolderBrowserDialog1 = New System.Windows.Forms.FolderBrowserDialog()
        Me.rdo_current = New System.Windows.Forms.RadioButton()
        Me.rdo_allFolder = New System.Windows.Forms.RadioButton()
        Me.btn_addTag = New System.Windows.Forms.Button()
        Me.rdo_index = New System.Windows.Forms.RadioButton()
        Me.btn_CreateFolder = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'btn_choose
        '
        Me.btn_choose.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        Me.btn_choose.Location = New System.Drawing.Point(185, 12)
        Me.btn_choose.Name = "btn_choose"
        Me.btn_choose.Size = New System.Drawing.Size(75, 23)
        Me.btn_choose.TabIndex = 0
        Me.btn_choose.Text = "choose"
        Me.btn_choose.UseVisualStyleBackColor = True
        '
        'txt_folderPath
        '
        Me.txt_folderPath.Location = New System.Drawing.Point(33, 50)
        Me.txt_folderPath.Name = "txt_folderPath"
        Me.txt_folderPath.Size = New System.Drawing.Size(227, 20)
        Me.txt_folderPath.TabIndex = 1
        '
        'txt_replace
        '
        Me.txt_replace.Location = New System.Drawing.Point(33, 95)
        Me.txt_replace.Multiline = True
        Me.txt_replace.Name = "txt_replace"
        Me.txt_replace.ScrollBars = System.Windows.Forms.ScrollBars.Both
        Me.txt_replace.Size = New System.Drawing.Size(736, 343)
        Me.txt_replace.TabIndex = 2
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.Location = New System.Drawing.Point(33, 13)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(146, 20)
        Me.Label1.TabIndex = 3
        Me.Label1.Text = "Choose your folder:"
        '
        'btn_select
        '
        Me.btn_select.Location = New System.Drawing.Point(602, 50)
        Me.btn_select.Name = "btn_select"
        Me.btn_select.Size = New System.Drawing.Size(75, 23)
        Me.btn_select.TabIndex = 4
        Me.btn_select.Text = "Select"
        Me.btn_select.UseVisualStyleBackColor = True
        '
        'btn_update
        '
        Me.btn_update.Location = New System.Drawing.Point(694, 50)
        Me.btn_update.Name = "btn_update"
        Me.btn_update.Size = New System.Drawing.Size(75, 23)
        Me.btn_update.TabIndex = 5
        Me.btn_update.Text = "Update"
        Me.btn_update.UseVisualStyleBackColor = True
        '
        'rdo_current
        '
        Me.rdo_current.AutoSize = True
        Me.rdo_current.Location = New System.Drawing.Point(266, 56)
        Me.rdo_current.Name = "rdo_current"
        Me.rdo_current.Size = New System.Drawing.Size(91, 17)
        Me.rdo_current.TabIndex = 8
        Me.rdo_current.Text = "Current Folder"
        Me.rdo_current.UseVisualStyleBackColor = True
        '
        'rdo_allFolder
        '
        Me.rdo_allFolder.AutoSize = True
        Me.rdo_allFolder.Location = New System.Drawing.Point(363, 56)
        Me.rdo_allFolder.Name = "rdo_allFolder"
        Me.rdo_allFolder.Size = New System.Drawing.Size(134, 17)
        Me.rdo_allFolder.TabIndex = 9
        Me.rdo_allFolder.Text = "Current and Sub Folder"
        Me.rdo_allFolder.UseVisualStyleBackColor = True
        '
        'btn_addTag
        '
        Me.btn_addTag.Location = New System.Drawing.Point(650, 21)
        Me.btn_addTag.Name = "btn_addTag"
        Me.btn_addTag.Size = New System.Drawing.Size(75, 23)
        Me.btn_addTag.TabIndex = 10
        Me.btn_addTag.Text = "AddTag"
        Me.btn_addTag.UseVisualStyleBackColor = True
        '
        'rdo_index
        '
        Me.rdo_index.AutoSize = True
        Me.rdo_index.Checked = True
        Me.rdo_index.Location = New System.Drawing.Point(503, 56)
        Me.rdo_index.Name = "rdo_index"
        Me.rdo_index.Size = New System.Drawing.Size(76, 17)
        Me.rdo_index.TabIndex = 11
        Me.rdo_index.TabStop = True
        Me.rdo_index.Text = "IndexPage"
        Me.rdo_index.UseVisualStyleBackColor = True
        '
        'btn_CreateFolder
        '
        Me.btn_CreateFolder.Location = New System.Drawing.Point(320, 13)
        Me.btn_CreateFolder.Name = "btn_CreateFolder"
        Me.btn_CreateFolder.Size = New System.Drawing.Size(75, 23)
        Me.btn_CreateFolder.TabIndex = 12
        Me.btn_CreateFolder.Text = "CreateFolder"
        Me.btn_CreateFolder.UseVisualStyleBackColor = True
        '
        'SEOLinksFrm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(800, 450)
        Me.Controls.Add(Me.btn_CreateFolder)
        Me.Controls.Add(Me.rdo_index)
        Me.Controls.Add(Me.btn_addTag)
        Me.Controls.Add(Me.rdo_allFolder)
        Me.Controls.Add(Me.rdo_current)
        Me.Controls.Add(Me.btn_update)
        Me.Controls.Add(Me.btn_select)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.txt_replace)
        Me.Controls.Add(Me.txt_folderPath)
        Me.Controls.Add(Me.btn_choose)
        Me.Name = "SEOLinksFrm"
        Me.Text = "SEOLinksFrm"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents btn_choose As Button
    Friend WithEvents txt_folderPath As TextBox
    Friend WithEvents txt_replace As TextBox
    Friend WithEvents Label1 As Label
    Friend WithEvents btn_select As Button
    Friend WithEvents btn_update As Button
    Friend WithEvents FolderBrowserDialog1 As FolderBrowserDialog
    Friend WithEvents rdo_current As RadioButton
    Friend WithEvents rdo_allFolder As RadioButton
    Friend WithEvents btn_addTag As Button
    Friend WithEvents rdo_index As RadioButton
    Friend WithEvents btn_CreateFolder As Button
End Class
