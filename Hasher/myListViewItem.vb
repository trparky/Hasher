' This class extends the ListViewItem so that I can add more properties to it for my purposes.
Public Class myListViewItem
    Inherits ListViewItem
    Public Property fileSize As Long
    Public Property hash As String
    Public Property fileName As String
    Public Property color As Color
    Public Property boolFileExists As Boolean
    Public Property boolComputedHash As Boolean = False

    Public Sub New(strInput As String)
        Me.Text = strInput
    End Sub
End Class