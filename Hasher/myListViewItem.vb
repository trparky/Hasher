' This class extends the ListViewItem so that I can add more properties to it for my purposes.
<Serializable>
Public Class myListViewItem
    Inherits ListViewItem
    Implements Runtime.Serialization.ISerializable
    Public Property fileSize As Long
    Public Property hash As String
    Public Property fileName As String
    Public Property color As Color
    Public Property boolFileExists As Boolean
    Public Property computeTime As TimeSpan

    Public Sub New(strInput As String)
        Me.Text = strInput
    End Sub

    Protected Sub New(serializationInfo As Runtime.Serialization.SerializationInfo, streamingContext As Runtime.Serialization.StreamingContext)
        Throw New NotImplementedException()
    End Sub
End Class