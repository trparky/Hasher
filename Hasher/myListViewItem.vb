' This class extends the ListViewItem so that I can add more properties to it for my purposes.
Public Class myListViewItem
    Inherits ListViewItem
    Private longFileSize As Long
    Private strHash As String

    Public Sub New(strInput As String)
        Me.Text = strInput
    End Sub

    Public Property hash() As String
        Get
            Return strHash
        End Get
        Set(value As String)
            strHash = value
        End Set
    End Property

    Public Property fileSize() As Long
        Get
            Return longFileSize
        End Get
        Set(value As Long)
            longFileSize = value
        End Set
    End Property
End Class