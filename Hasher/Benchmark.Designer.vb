<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Benchmark
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Benchmark))
        Me.listResults = New System.Windows.Forms.ListView()
        Me.colBufferSize = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.colComputeTime = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.OpenFileDialog = New System.Windows.Forms.OpenFileDialog()
        Me.btnOpenFile = New System.Windows.Forms.Button()
        Me.ProgressBar = New System.Windows.Forms.ProgressBar()
        Me.lblStatus = New System.Windows.Forms.Label()
        Me.lblHeader = New System.Windows.Forms.Label()
        Me.SuspendLayout()
        '
        'listResults
        '
        Me.listResults.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.colBufferSize, Me.colComputeTime})
        Me.listResults.FullRowSelect = True
        Me.listResults.HideSelection = False
        Me.listResults.Location = New System.Drawing.Point(13, 200)
        Me.listResults.Name = "listResults"
        Me.listResults.Size = New System.Drawing.Size(437, 303)
        Me.listResults.TabIndex = 0
        Me.listResults.UseCompatibleStateImageBehavior = False
        Me.listResults.View = System.Windows.Forms.View.Details
        '
        'colBufferSize
        '
        Me.colBufferSize.Text = "Buffer Size"
        Me.colBufferSize.Width = 77
        '
        'colComputeTime
        '
        Me.colComputeTime.Text = "Compute Time"
        Me.colComputeTime.Width = 185
        '
        'OpenFileDialog
        '
        Me.OpenFileDialog.FileName = "OpenFileDialog"
        Me.OpenFileDialog.Filter = "All Files|*.*"
        Me.OpenFileDialog.Title = "Open File for Benchmarking"
        '
        'btnOpenFile
        '
        Me.btnOpenFile.Location = New System.Drawing.Point(12, 116)
        Me.btnOpenFile.Name = "btnOpenFile"
        Me.btnOpenFile.Size = New System.Drawing.Size(438, 23)
        Me.btnOpenFile.TabIndex = 1
        Me.btnOpenFile.Text = "Open File for Benchmarking"
        Me.btnOpenFile.UseVisualStyleBackColor = True
        '
        'ProgressBar
        '
        Me.ProgressBar.Location = New System.Drawing.Point(12, 145)
        Me.ProgressBar.Name = "ProgressBar"
        Me.ProgressBar.Size = New System.Drawing.Size(437, 23)
        Me.ProgressBar.TabIndex = 15
        '
        'lblStatus
        '
        Me.lblStatus.AutoSize = True
        Me.lblStatus.Location = New System.Drawing.Point(12, 171)
        Me.lblStatus.Name = "lblStatus"
        Me.lblStatus.Size = New System.Drawing.Size(172, 26)
        Me.lblStatus.TabIndex = 16
        Me.lblStatus.Text = vbCrLf & "(No Background Process Running)"
        '
        'lblHeader
        '
        Me.lblHeader.AutoSize = True
        Me.lblHeader.Location = New System.Drawing.Point(9, 9)
        Me.lblHeader.Name = "lblHeader"
        Me.lblHeader.Size = New System.Drawing.Size(441, 104)
        Me.lblHeader.TabIndex = 17
        Me.lblHeader.Text = resources.GetString("lblHeader.Text")
        '
        'Benchmark
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(462, 515)
        Me.Controls.Add(Me.lblHeader)
        Me.Controls.Add(Me.lblStatus)
        Me.Controls.Add(Me.ProgressBar)
        Me.Controls.Add(Me.btnOpenFile)
        Me.Controls.Add(Me.listResults)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.MaximizeBox = False
        Me.Name = "Benchmark"
        Me.Text = "Hasher Benchmark"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents listResults As ListView
    Friend WithEvents colBufferSize As ColumnHeader
    Friend WithEvents colComputeTime As ColumnHeader
    Friend WithEvents OpenFileDialog As OpenFileDialog
    Friend WithEvents btnOpenFile As Button
    Friend WithEvents ProgressBar As ProgressBar
    Friend WithEvents lblStatus As Label
    Friend WithEvents lblHeader As Label
End Class
