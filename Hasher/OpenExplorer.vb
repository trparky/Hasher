Public Class OpenExplorer
    Public Property _MyParentForm As Form1
    Public Property _boolYesNo As Boolean

    Public Sub New(strFilePath As String, strChecksumType As String, strMessage As String, boolYesNo As Boolean, MyParentForm As Form1)
        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Debug.WriteLine(Label1.Width)
        Label1.Text = String.Format(Label1.Text, strFilePath, strChecksumType, strMessage)
        Size = New Size(Label1.Width + 80, Size.Height)

        _boolYesNo = boolYesNo
        _MyParentForm = MyParentForm

        ChkAskEveryTime.Checked = Not My.Settings.boolOpenInExplorer

        If Not boolYesNo Then
            ChkAskEveryTime.Visible = False
            BtnYes.Visible = False
            BtnNo.Text = "OK"
        End If
    End Sub

    Private Sub OpenExplorer_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Media.SystemSounds.Asterisk.Play()
        PictureBox1.Image = SystemIcons.Question.ToBitmap()
    End Sub

    Private Sub BtnYes_Click(sender As Object, e As EventArgs) Handles BtnYes.Click
        DialogResult = DialogResult.Yes
        Close()
    End Sub

    Private Sub BtnNo_Click(sender As Object, e As EventArgs) Handles BtnNo.Click
        DialogResult = If(_boolYesNo, DialogResult.No, DialogResult.Yes)
        Close()
    End Sub

    Private Sub CloseFreeSysLogDialog_KeyUp(sender As Object, e As KeyEventArgs) Handles Me.KeyUp
        If e.KeyCode = Keys.Y Then
            BtnYes.PerformClick()
        ElseIf e.KeyCode = Keys.N Then
            BtnNo.PerformClick()
        End If
    End Sub

    Private Sub ChkAskEveryTime_Click(sender As Object, e As EventArgs) Handles ChkAskEveryTime.Click
        _MyParentForm.chkOpenInExplorer.Checked = Not ChkAskEveryTime.Checked
        My.Settings.boolOpenInExplorer = Not ChkAskEveryTime.Checked
    End Sub
End Class