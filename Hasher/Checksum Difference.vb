Public Class frmChecksumDifference
    Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
        Close()
    End Sub

    Private Sub frmChecksumDifference_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Threading.ThreadPool.QueueUserWorkItem(Sub() Media.SystemSounds.Exclamation.Play())
    End Sub
End Class