' This class extends the DataGridViewRow so that I can add more properties to it for my purposes.
Public Class MyDataGridViewRow
    Inherits DataGridViewRow
    Implements ICloneable
    Public Property FileSize As Long
    Public Property Hash As String
    Public Property FileName As String
    Public Property MyColor As Color
    Public Property BoolFileExists As Boolean
    Public Property ComputeTime As TimeSpan
    Public Property AllTheHashes As AllTheHashes
    Public Property BoolValidHash As Boolean
    Public Property ColorType As ColorType
    Public Property BoolExceptionOccurred As Boolean = False
    Public Property StrCrashData As String

    Public Overrides Function Clone() As Object Implements ICloneable.Clone
        Dim newMyDataGridViewRow As New MyDataGridViewRow()
        newMyDataGridViewRow.CreateCells(Form1.listFiles)

        For index As Short = 1 To Me.Cells.Count - 1
            newMyDataGridViewRow.Cells(index).Value = Me.Cells(index).Value
        Next

        With newMyDataGridViewRow
            .FileSize = Me.FileSize
            .Hash = Me.Hash
            .FileName = Me.FileName
            .MyColor = Me.MyColor
            .BoolFileExists = Me.BoolFileExists
            .ComputeTime = Me.ComputeTime
            .AllTheHashes = Me.AllTheHashes
            .BoolValidHash = Me.BoolValidHash
            .ColorType = Me.ColorType
            .BoolExceptionOccurred = Me.BoolExceptionOccurred
            .StrCrashData = Me.StrCrashData
        End With

        Return newMyDataGridViewRow
    End Function
End Class

Public Class BenchmarkListViewItem
    Inherits ListViewItem
    Public Property BufferSize As Short

    Public Sub New(strInput As String)
        Me.Text = strInput
    End Sub
End Class

Public Enum ColorType As Byte
    Valid
    NotValid
    NotFound
End Enum