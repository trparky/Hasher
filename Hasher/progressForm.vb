Public Class ProgressForm
    ''' <summary>Sets the taskbar progress bar's value.</summary>
    Public Sub setTaskbarProgressBarValue(value As Integer)
        Me.TaskBarProgressBarState = ThumbnailProgressState.Normal
        Me.TaskBarProgressBarValue = value
        Me.TaskBarProgressBarMaximumValue = 100
    End Sub
End Class