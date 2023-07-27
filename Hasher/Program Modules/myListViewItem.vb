' This class extends the ListViewItem so that I can add more properties to it for my purposes.
Public Class MyListViewItem
    Inherits ListViewItem
    Implements ICloneable
    Public Property FileSize As Long
    Public Property Hash As String
    Public Property FileName As String
    Public Property Color As Color
    Public Property BoolFileExists As Boolean
    Public Property ComputeTime As TimeSpan
    Public Property AllTheHashes As AllTheHashes
    Public Property BoolValidHash As Boolean
    Public Property ColorType As ColorType
    Public Property boolExceptionOccurred As Boolean = False
    Public Property strCrashData As String

    Public Sub New(strInput As String)
        Me.Text = strInput
    End Sub

    Public Overrides Function Clone() As Object Implements ICloneable.Clone
        Dim newListViewItem As New MyListViewItem(Me.Text)

        For index As Short = 1 To Me.SubItems.Count - 1
            newListViewItem.SubItems.Add(Me.SubItems(index))
        Next

        With newListViewItem
            .FileSize = Me.FileSize
            .Hash = Me.Hash
            .FileName = Me.FileName
            .Color = Me.Color
            .BoolFileExists = Me.BoolFileExists
            .ComputeTime = Me.ComputeTime
            .BackColor = Me.BackColor
            .AllTheHashes = Me.AllTheHashes
            .BoolValidHash = Me.BoolValidHash
            .ColorType = Me.ColorType
            .boolExceptionOccurred = Me.boolExceptionOccurred
            .strCrashData = Me.strCrashData
        End With

        Return newListViewItem
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