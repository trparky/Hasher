Public Class FrmHashFileWritten
    Public Enum UserResponse
        ok
        yes
        no
    End Enum

    Public OutUserResponse As UserResponse

    Private Sub BtnOK_Click(sender As Object, e As EventArgs) Handles BtnOK.Click
        OutUserResponse = UserResponse.ok
        Me.Close()
    End Sub

    Private Sub BtnNo_Click(sender As Object, e As EventArgs) Handles BtnNo.Click
        OutUserResponse = UserResponse.no
        Me.Close()
    End Sub

    Private Sub BtnYes_Click(sender As Object, e As EventArgs) Handles BtnYes.Click
        OutUserResponse = UserResponse.yes
        Me.Close()
    End Sub
End Class