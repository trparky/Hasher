Imports System.ComponentModel

<ToolboxItem(True)>
<ToolboxBitmap(GetType(ListView))>
Public Class ListViewDoubleBuffered
    Inherits ListView

    Public Sub New()
        Me.DoubleBuffered = True
    End Sub
End Class