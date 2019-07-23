Public Class Benchmark
    Private workingThread As Threading.Thread
    Private boolBackgroundThreadWorking As Boolean = False
    Private boolClosingWindow As Boolean

    Private Sub Benchmark_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Icon = Icon.ExtractAssociatedIcon(Reflection.Assembly.GetExecutingAssembly().Location)
    End Sub

    Private Sub BtnOpenFile_Click(sender As Object, e As EventArgs) Handles btnOpenFile.Click
        If btnOpenFile.Text = "Abort Processing" Then
            If workingThread IsNot Nothing Then
                workingThread.Abort()
                boolBackgroundThreadWorking = False
            End If

            Exit Sub
        End If

        If OpenFileDialog.ShowDialog() <> DialogResult.OK Then Exit Sub

        btnOpenFile.Text = "Abort Processing"
        listResults.Items.Clear()

        workingThread = New Threading.Thread(Sub()
                                                 Try
                                                     boolBackgroundThreadWorking = True
                                                     Dim strFileNameLine As String = "Benchmarking with file " & New IO.FileInfo(OpenFileDialog.FileName).Name & vbCrLf
                                                     Dim intBufferSize As Integer
                                                     Dim percentage As Double
                                                     Dim strChecksum As String = Nothing
                                                     Dim index As Integer = 1
                                                     Dim subRoutine As [Delegate] = Sub(size As Long, totalBytesRead As Long, eta As TimeSpan)
                                                                                        Try
                                                                                            Me.Invoke(Sub()
                                                                                                          percentage = If(totalBytesRead <> 0 And size <> 0, totalBytesRead / size * 100, 0)
                                                                                                          ProgressBar.Value = percentage
                                                                                                          lblStatus.Text = strFileNameLine & fileSizeToHumanSize(totalBytesRead) & " of " & fileSizeToHumanSize(size) & " (" & Math.Round(percentage, 2) & "%) have been processed with a " & intBufferSize.ToString & " MB buffer size."
                                                                                                      End Sub)
                                                                                        Catch ex As Exception
                                                                                        End Try
                                                                                    End Sub

                                                     Dim stopWatch As Stopwatch = Stopwatch.StartNew
                                                     Dim computeStopwatch As Stopwatch
                                                     Dim intRealBufferSize As Integer
                                                     Dim itemToBeAdded As benchmarkListViewItem

                                                     For intBufferSize = 1 To 16
                                                         intRealBufferSize = intBufferSize * 1024 * 1024
                                                         computeStopwatch = Stopwatch.StartNew

                                                         If doChecksumWithAttachedSubRoutine(OpenFileDialog.FileName, checksumType.sha256, strChecksum, subRoutine, intRealBufferSize) Then
                                                             itemToBeAdded = New benchmarkListViewItem(intBufferSize & If(intBufferSize = 1, " MB", " MBs")) With {.bufferSize = intBufferSize}
                                                             itemToBeAdded.SubItems.Add(timespanToHMS(computeStopwatch.Elapsed))
                                                             listResults.Items.Add(itemToBeAdded)
                                                         End If
                                                     Next

                                                     Me.Text = "Hasher"
                                                     Me.Invoke(Sub() MsgBox("Benchmark completed in " & timespanToHMS(stopWatch.Elapsed) & ".", MsgBoxStyle.Information, "Hasher Benchmark"))
                                                 Catch ex As Threading.ThreadAbortException
                                                 Finally
                                                     If Not boolClosingWindow Then
                                                         lblStatus.Text = "(No Background Process Running)"
                                                         btnOpenFile.Text = "Open File for Benchmarking"
                                                         ProgressBar.Value = 0
                                                     End If

                                                     boolBackgroundThreadWorking = False
                                                     workingThread = Nothing
                                                 End Try
                                             End Sub) With {
            .Priority = Threading.ThreadPriority.Highest,
            .Name = "Hash Generation Thread",
            .IsBackground = True
        }
        workingThread.Start()
    End Sub

    Private Sub Benchmark_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        If boolBackgroundThreadWorking AndAlso MsgBox("Benchmarks are being processed, do you want to abort?", MsgBoxStyle.YesNo + MsgBoxStyle.Question, "Hasher Benchmark") = MsgBoxResult.No Then
            e.Cancel = True
            Exit Sub
        Else
            If workingThread IsNot Nothing Then
                boolClosingWindow = True
                workingThread.Abort()
            End If
            benchmarkWindowInstance = Nothing
        End If
    End Sub

    Private Shared Function doChecksumWithAttachedSubRoutine(strFile As String, checksumType As checksumType, ByRef strChecksum As String, subRoutine As [Delegate], intBufferSize As Integer) As Boolean
        Try
            If IO.File.Exists(strFile) Then
                Dim checksums As New checksums(subRoutine)
                strChecksum = checksums.performFileHash(strFile, intBufferSize, checksumType, My.Settings.boolShowEstimatedTime)
                Return True
            Else
                Return False
            End If

            Return False
        Catch ex As Exception
            Return False
        End Try
    End Function

    Private Sub ContextMenuStrip_Opening(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles ContextMenuStrip.Opening
        If listResults.SelectedItems.Count = 0 Then
            e.Cancel = True
        Else
            Dim selectedItem As benchmarkListViewItem = listResults.SelectedItems(0)
            shortBufferSize = selectedItem.bufferSize
            btnSetBufferSize.Text = "Set Buffer Size to " & If(selectedItem.bufferSize = 1, "1 MB", selectedItem.bufferSize & " MBs")
        End If
    End Sub

    Private Sub BtnSetBufferSize_Click(sender As Object, e As EventArgs) Handles btnSetBufferSize.Click
        boolSetBufferSize = True
        Me.Close()
    End Sub
End Class