﻿Public Class ProgressForm
    ''' <summary>Sets the taskbar progress bar's value.</summary>
    Public Sub setTaskbarProgressBarValue(value As Integer)
        TaskBarProgressBarState = ThumbnailProgressState.Normal
        TaskBarProgressBarValue = value
        TaskBarProgressBarMaximumValue = 100
    End Sub
End Class