' This class extends the ListViewItem so that I can add more properties to it for my purposes.
Public Class myListViewItem
    Inherits ListViewItem
    Implements ICloneable
    Public Property fileSize As Long
    Public Property hash As String
    Public Property fileName As String
    Public Property color As Color
    Public Property boolFileExists As Boolean
    Public Property computeTime As TimeSpan

    Public Sub New(strInput As String)
        Me.Text = strInput
    End Sub

    Public Sub New()
    End Sub

    Public Overrides Function Clone() As Object Implements ICloneable.Clone
        Dim listViewSubItem(Me.SubItems.Count - 1) As ListViewItem.ListViewSubItem
        Dim newListViewItem As New myListViewItem(Me.Text)

        For i As Short = 1 To Me.SubItems.Count - 1
            newListViewItem.SubItems.Add(Me.SubItems(i))
        Next

        newListViewItem.fileSize = Me.fileSize
        newListViewItem.hash = Me.hash
        newListViewItem.fileName = Me.fileName
        newListViewItem.color = Me.color
        newListViewItem.boolFileExists = Me.boolFileExists
        newListViewItem.computeTime = Me.computeTime
        newListViewItem.BackColor = Me.BackColor

        Return newListViewItem
    End Function
End Class

Public Class benchmarkListViewItem
    Inherits ListViewItem
    Public Property bufferSize As Short

    Public Sub New(strInput As String)
        Me.Text = strInput
    End Sub
End Class