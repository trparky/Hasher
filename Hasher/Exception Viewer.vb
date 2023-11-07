Public Class Exception_Viewer
    Private Sub BtnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
        Close()
    End Sub

    Private Sub Exception_Viewer_KeyUp(sender As Object, e As KeyEventArgs) Handles Me.KeyUp
        If e.KeyCode = Keys.Escape Then Close()
    End Sub

    Private Sub Exception_Viewer_ResizeEnd(sender As Object, e As EventArgs) Handles Me.ResizeEnd
        My.Settings.exceptionViewerWindowSize = Size
    End Sub

    Private Sub Exception_Viewer_Load(sender As Object, e As EventArgs) Handles Me.Load
        TxtExceptionData.Select(0, 0)
    End Sub
End Class