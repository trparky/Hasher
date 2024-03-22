<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Checksum_Viewer
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
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

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.checksums = New System.Windows.Forms.ListView()
        Me.colType = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.colChecksum = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.lblFileName = New System.Windows.Forms.Label()
        Me.btnClose = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'checksums
        '
        Me.checksums.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.colType, Me.colChecksum})
        Me.checksums.HideSelection = False
        Me.checksums.Location = New System.Drawing.Point(12, 25)
        Me.checksums.Name = "checksums"
        Me.checksums.Size = New System.Drawing.Size(956, 119)
        Me.checksums.TabIndex = 0
        Me.checksums.UseCompatibleStateImageBehavior = False
        Me.checksums.View = System.Windows.Forms.View.Details
        '
        'colType
        '
        Me.colType.Text = "Type"
        '
        'colChecksum
        '
        Me.colChecksum.Text = "Checksum"
        Me.colChecksum.Width = 888
        '
        'lblFileName
        '
        Me.lblFileName.AutoSize = True
        Me.lblFileName.Location = New System.Drawing.Point(12, 9)
        Me.lblFileName.Name = "lblFileName"
        Me.lblFileName.Size = New System.Drawing.Size(57, 13)
        Me.lblFileName.TabIndex = 1
        Me.lblFileName.Text = "File Name:"
        '
        'btnClose
        '
        Me.btnClose.Location = New System.Drawing.Point(893, 150)
        Me.btnClose.Name = "btnClose"
        Me.btnClose.Size = New System.Drawing.Size(75, 23)
        Me.btnClose.TabIndex = 2
        Me.btnClose.Text = "Close"
        Me.btnClose.UseVisualStyleBackColor = True
        '
        'Checksum_Viewer
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(980, 181)
        Me.Controls.Add(Me.btnClose)
        Me.Controls.Add(Me.lblFileName)
        Me.Controls.Add(Me.checksums)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.KeyPreview = True
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "Checksum_Viewer"
        Me.Text = "Checksum Viewer"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents checksums As ListView
    Friend WithEvents colType As ColumnHeader
    Friend WithEvents colChecksum As ColumnHeader
    Friend WithEvents lblFileName As Label
    Friend WithEvents btnClose As Button
End Class
