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
        Dim newListViewItem As New myListViewItem(Me.Text)

        For index As Short = 1 To Me.SubItems.Count - 1
            newListViewItem.SubItems.Add(Me.SubItems(index))
        Next

        With newListViewItem
            .fileSize = Me.fileSize
            .hash = Me.hash
            .fileName = Me.fileName
            .color = Me.color
            .boolFileExists = Me.boolFileExists
            .computeTime = Me.computeTime
            .BackColor = Me.BackColor
        End With

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