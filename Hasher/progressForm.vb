Public Class ProgressForm
    ''' <summary>Sets the taskbar progress bar's value.</summary>
    Public Sub setTaskbarProgressBarValue(value As Integer, maxValue As Integer)
        Me.Maximum = maxValue
        Me.State = ThumbnailProgressState.Normal
        Me.Value = value
    End Sub

    ''' <summary>Sets the taskbar progress bar's value.</summary>
    Public Sub setTaskbarProgressBarValue(value As Integer)
        Me.State = ThumbnailProgressState.Normal
        Me.Value = value
        Me.Maximum = 100
    End Sub
End Class