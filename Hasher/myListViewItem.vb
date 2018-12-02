' This class extends the ListViewItem so that I can add more properties to it for my purposes.
Public Class myListViewItem
    Inherits ListViewItem
    Public fileSize As Long
    Public hash, fileName As String
    Public color As Color
    Public boolFileExists As Boolean

    Public Sub New(strInput As String)
        Me.Text = strInput
    End Sub
End Class