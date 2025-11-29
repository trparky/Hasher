Imports System.Buffers
Imports System.ComponentModel
Imports System.IO.Pipes
Imports System.Reflection
Imports System.Security.Cryptography

Public Class Form1
    Private Const strWaitingToBeProcessed As String = "Waiting to be processed..."
    Private Const strCurrentlyBeingProcessed As String = "Currently being processed... Please wait."
    Private Const strNoBackgroundProcesses As String = "(No Background Processes)"
#If DEBUG Then
    Private Const strWindowTitle As String = "Hasher (Debug Build)"
#Else
    Private Const strWindowTitle As String = "Hasher"
#End If

    Private pool As ArrayPool(Of Byte) = ArrayPool(Of Byte).Shared

    Private Const strMessageBoxTitleText As String = "Hasher"
    Private intBufferSize As Integer = My.Settings.shortBufferSize * 1024 * 1024
    Private strLastDirectoryWorkedOn As String
    Private filesInListFiles As New List(Of String)
    Private boolBackgroundThreadWorking As Boolean = False
    Private workingThread As Threading.Thread
    Private boolClosingWindow As Boolean = False
    Private sortOrderForListFiles As SortOrder = SortOrder.Descending ' Define soSortOrder at class level
    Private sortOrderForVerifyHashesListFiles As SortOrder = SortOrder.Descending ' Define soSortOrder at class level
    Private boolDoneLoading As Boolean = False
    Private Property PipeServer As NamedPipeServerStream = Nothing
    Private ReadOnly strNamedPipeServerName As String = $"hasher_{GetSHA256HashOfString(Environment.UserName).Substring(0, 10)}"
    Private Const strPayPal As String = "https://www.paypal.com/cgi-bin/webscr?cmd=_donations&business=HQL3AC96XKM42&lc=US&no_note=1&no_shipping=1&rm=1&return=http%3a%2f%2fwww%2etoms%2dworld%2eorg%2fblog%2fthank%2dyou%2dfor%2dyour%2ddonation&currency_code=USD&bn=PP%2dDonationsBF%3abtn_donateCC_LG%2egif%3aNonHosted"
    Private boolDidWePerformAPreviousHash As Boolean = False
    Private validColor, notValidColor, fileNotFoundColor As Color
    Private intCurrentlyActiveTab As Integer = -1
    Private compareFilesAllTheHashes1 As AllTheHashes = Nothing
    Private compareFilesAllTheHashes2 As AllTheHashes = Nothing
    Private hashTextAllTheHashes As AllTheHashes = Nothing
    Private globalAllTheHashes As AllTheHashes = Nothing
    Private checksumTypeForChecksumCompareWindow As HashAlgorithmName
    Private strLastHashFileLoaded As String = Nothing

    Private Const strColumnTitleChecksumMD5 As String = "Hash/Checksum (MD5)"
    Private Const strColumnTitleChecksumSHA160 As String = "Hash/Checksum (SHA1/SHA160)"
    Private Const strColumnTitleChecksumSHA256 As String = "Hash/Checksum (SHA256)"
    Private Const strColumnTitleChecksumSHA384 As String = "Hash/Checksum (SHA384)"
    Private Const strColumnTitleChecksumSHA512 As String = "Hash/Checksum (SHA512)"
    Private Const strHashChecksumToBeComputed As String = "Hash/Checksum: (To Be Computed)"

    Private Const ChecksumFilterIndexMD5 As Integer = 1
    Private Const ChecksumFilterIndexSHA160 As Integer = 2
    Private Const ChecksumFilterIndexSHA256 As Integer = 3
    Private Const ChecksumFilterIndexSHA384 As Integer = 4
    Private Const ChecksumFilterIndexSHA512 As Integer = 5

    Private Const TabNumberNull As Integer = -1
    Private Const TabNumberWelcomeTab As Integer = 0
    Private Const TabNumberHashTextTab As Integer = 1
    Private Const TabNumberHashIndividualFilesTab As Integer = 2
    Private Const TabNumberVerifySavedHashesTab As Integer = 3
    Private Const TabNumberCompareFilesTab As Integer = 4
    Private Const TabNumberCompareFileAgainstKnownHashTab As Integer = 5
    Private Const TabNumberSettingsTab As Integer = 6

    Private ReadOnly hashLineParser As New Text.RegularExpressions.Regex("(?<checksum>[0-9a-f]{32,128}) \*?(?<filename>.+)", System.Text.RegularExpressions.RegexOptions.Compiled + System.Text.RegularExpressions.RegexOptions.IgnoreCase)
    Private ReadOnly PFSenseHashLineParser As New Text.RegularExpressions.Regex("SHA256 \((?<filename>.+)\) = (?<checksum>.+)", System.Text.RegularExpressions.RegexOptions.Compiled + System.Text.RegularExpressions.RegexOptions.IgnoreCase)
    Private ReadOnly HashFileWithNoFilename As New Text.RegularExpressions.Regex("(?<checksum>[0-9a-f]{32,128})", System.Text.RegularExpressions.RegexOptions.Compiled + System.Text.RegularExpressions.RegexOptions.IgnoreCase)

    Private Function GenerateProcessingFileString(intCurrentFile As Integer, intTotalFiles As Integer) As String
        Return $"Processing file {MyToString(intCurrentFile)} of {MyToString(intTotalFiles)} {If(intTotalFiles = 1, "file", "files")}."
    End Function

    ''' <summary>
    ''' This function works similarly to the Invoke function that's already built into .NET.
    ''' It checks if an invoke is required, and only invokes the passed routine on the main thread if necessary.
    ''' </summary>
    ''' <param name="input">A strongly-typed delegate.</param>
    ''' <param name="invokeOn">The control that the Delegate will be invoked on.</param>
    Private Sub MyInvoke(input As Action, invokeOn As Control)
        If boolClosingWindow Then Exit Sub
        If invokeOn.InvokeRequired() Then
            ' If InvokeRequired is true, use Invoke to call the action on the UI thread.
            invokeOn.Invoke(input)
        Else
            ' If not required, just execute the action directly.
            input()
        End If
    End Sub

    Private Sub UpdateGridViewRowColor(ByRef itemOnGUI As MyDataGridViewRow, ByRef item As MyDataGridViewRow)
        With itemOnGUI
            If item IsNot Nothing Then
                Dim currentStyle As DataGridViewCellStyle = item.DefaultCellStyle
                currentStyle.BackColor = item.MyColor
                currentStyle.ForeColor = GetGoodTextColorBasedUponBackgroundColor(item.MyColor)
                .DefaultCellStyle = currentStyle
            End If
        End With
    End Sub

    Private Sub UpdateDataGridViewRow(ByRef itemOnGUI As MyDataGridViewRow, ByRef item As MyDataGridViewRow, Optional boolUpdateColor As Boolean = True)
        With itemOnGUI
            If item IsNot Nothing Then
                For i As Short = 1 To item.Cells.Count - 1
                    .Cells(i).Value = item.Cells(i).Value
                Next

                .FileSize = item.FileSize
                .Hash = item.Hash
                .FileName = item.FileName
                .MyColor = item.MyColor
                .BoolFileExists = item.BoolFileExists
                .ComputeTime = item.ComputeTime
                .AllTheHashes = item.AllTheHashes
                .BoolValidHash = item.BoolValidHash
                .StrCrashData = item.StrCrashData
                .BoolExceptionOccurred = item.BoolExceptionOccurred
                .DefaultCellStyle.Padding = item.DefaultCellStyle.Padding

                If boolUpdateColor Then
                    Dim currentStyle As DataGridViewCellStyle = item.DefaultCellStyle

                    currentStyle.BackColor = item.MyColor
                    currentStyle.ForeColor = GetGoodTextColorBasedUponBackgroundColor(item.MyColor)
                    currentStyle.WrapMode = DataGridViewTriState.True

                    .DefaultCellStyle = currentStyle
                End If
            End If
        End With
    End Sub

    Private Function MyToString(input As Integer) As String
        Return If(chkUseCommasInNumbers.Checked, input.ToString("N0"), input.ToString)
    End Function

    Private Function MyToString(input As Long) As String
        Return If(chkUseCommasInNumbers.Checked, input.ToString("N0"), input.ToString)
    End Function

    Private Function DoChecksumWithAttachedSubRoutine(strFile As String, ByRef allTheHashes As AllTheHashes, subRoutine As ChecksumStatusUpdaterDelegate, ByRef exceptionObject As Exception) As Boolean
        Try
            If IO.File.Exists(strFile) Then
                Dim checksums As New Checksums(subRoutine, pool)
                allTheHashes = checksums.PerformFileHash(strFile, intBufferSize)
                Return True
            Else
                Return False
            End If

            Return False
        Catch ex As Exception
            exceptionObject = ex
            Return False
        End Try
    End Function

    Private Sub UpdateFilesListCountHeader(Optional boolIncludeSelectedItemCount As Boolean = False)
        MyInvoke(Sub()
                     If boolIncludeSelectedItemCount Then
                         lblFileCountOnHashIndividualFilesTab.Text = $"({MyToString(listFiles.Rows.Count)} {If(listFiles.Rows.Count = 1, "file", "files")}, {MyToString(listFiles.SelectedRows.Count)} {If(listFiles.SelectedRows.Count = 1, "file", "files")} are selected)"
                     Else
                         lblFileCountOnHashIndividualFilesTab.Text = $"({MyToString(listFiles.Rows.Count)} {If(listFiles.Rows.Count = 1, "file", "files")})"
                     End If

                     If listFiles.Rows.Count = 0 Then
                         btnComputeHash.Enabled = False
                         btnIndividualFilesCopyToClipboard.Enabled = False
                         btnIndividualFilesSaveResultsToDisk.Enabled = False
                     Else
                         Dim intNumberOfItemsWithoutHash As Integer = listFiles.Rows.Cast(Of DataGridViewRow).Where(Function(item As DataGridViewRow) item.Cells(2).Value.Equals(strWaitingToBeProcessed)).Count

                         btnComputeHash.Enabled = intNumberOfItemsWithoutHash > 0

                         If intNumberOfItemsWithoutHash <> listFiles.Rows.Count Then
                             btnIndividualFilesCopyToClipboard.Enabled = True
                             btnIndividualFilesSaveResultsToDisk.Enabled = True
                         End If
                     End If
                 End Sub, Me)
    End Sub

    Private Sub BtnRemoveAllFiles_Click(sender As Object, e As EventArgs) Handles btnRemoveAllFiles.Click
        listFiles.Rows.Clear()
        filesInListFiles.Clear()
        UpdateFilesListCountHeader()
        strLastHashFileLoaded = Nothing
    End Sub

    Private Sub BtnRemoveSelectedFiles_Click(sender As Object, e As EventArgs) Handles btnRemoveSelectedFiles.Click
        If listFiles.SelectedRows.Count > 500 AndAlso MsgBox($"It would be recommended to use the ""Remove All Files"" button instead, removing this many items ({MyToString(listFiles.SelectedRows.Count)} items) from the list is a slow process and will make the program appear locked up.{DoubleCRLF}Are you sure you want to remove the items this way?", MsgBoxStyle.Question + MsgBoxStyle.YesNo, strMessageBoxTitleText) = MsgBoxResult.No Then
            Exit Sub
        End If

        For Each item As MyDataGridViewRow In listFiles.SelectedRows
            filesInListFiles.Remove(item.Cells(0).Value)
            listFiles.Rows.Remove(item)
        Next

        UpdateFilesListCountHeader()
    End Sub

    Private Function CreateListFilesObject(strFileName As String, ByRef dataGrid As DataGridView) As MyDataGridViewRow
        filesInListFiles.Add(strFileName.Trim.ToLower)

        Dim itemToBeAdded As New MyDataGridViewRow()
        With itemToBeAdded
            .CreateCells(dataGrid)
            .FileSize = New IO.FileInfo(strFileName).Length
            .FileName = strFileName
            .Cells(0).Value = strFileName
            .Cells(1).Value = FileSizeToHumanSize(itemToBeAdded.FileSize)
            .Cells(2).Value = strWaitingToBeProcessed
            .Cells(3).Value = ""
            .DefaultCellStyle.Padding = New Padding(0, 2, 0, 2)
            .DefaultCellStyle.WrapMode = DataGridViewTriState.True
        End With

        Return itemToBeAdded
    End Function

    Private Function CreateFilesDataGridObject(strFileName As String, ByRef dataGrid As DataGridView) As MyDataGridViewRow
        Dim itemToBeAdded As New MyDataGridViewRow

        With itemToBeAdded
            .CreateCells(dataGrid)
            .FileSize = New IO.FileInfo(strFileName).Length
            .FileName = strFileName
            .Cells(0).Value = strFileName
            .Cells(1).Value = FileSizeToHumanSize(itemToBeAdded.FileSize)
            .Cells(2).Value = strWaitingToBeProcessed
            .Cells(3).Value = ""
            .DefaultCellStyle.Padding = New Padding(0, 2, 0, 2)
        End With

        Return itemToBeAdded
    End Function

    Private Function CreateFilesDataGridObject(strFileName As String, longFileSize As Long, ByRef dataGrid As DataGridView) As MyDataGridViewRow
        Dim itemToBeAdded As New MyDataGridViewRow

        With itemToBeAdded
            .CreateCells(dataGrid)
            .FileSize = longFileSize
            .FileName = strFileName
            .Cells(0).Value = strFileName
            .Cells(1).Value = FileSizeToHumanSize(itemToBeAdded.FileSize)
            .Cells(2).Value = strWaitingToBeProcessed
            .Cells(3).Value = ""
            .DefaultCellStyle.Padding = New Padding(0, 2, 0, 2)
        End With

        Return itemToBeAdded
    End Function

    Private Sub BtnAddIndividualFiles_Click(sender As Object, e As EventArgs) Handles btnAddIndividualFiles.Click
        Using OpenFileDialog As New OpenFileDialog
            OpenFileDialog.Title = "Select Files to be Hashed..."
            OpenFileDialog.Multiselect = True
            OpenFileDialog.Filter = "Show All Files|*.*"
            OpenFileDialog.Multiselect = True

            If OpenFileDialog.ShowDialog() = DialogResult.OK Then
                If OpenFileDialog.FileNames.Count = 0 Then
                    MsgBox("You must select some files.", MsgBoxStyle.Critical, strMessageBoxTitleText)
                ElseIf OpenFileDialog.FileNames.Count = 1 Then
                    strLastDirectoryWorkedOn = New IO.FileInfo(OpenFileDialog.FileName).DirectoryName

                    If Not filesInListFiles.Contains(OpenFileDialog.FileName.Trim.ToLower) AndAlso IO.File.Exists(OpenFileDialog.FileName) Then
                        listFiles.Rows.Add(CreateListFilesObject(OpenFileDialog.FileName, listFiles))
                    End If
                Else
                    strLastDirectoryWorkedOn = New IO.FileInfo(OpenFileDialog.FileNames(0)).DirectoryName

                    For Each strFileName As String In OpenFileDialog.FileNames
                        If Not filesInListFiles.Contains(strFileName.Trim.ToLower) AndAlso IO.File.Exists(strFileName) Then
                            listFiles.Rows.Add(CreateListFilesObject(strFileName, listFiles))
                        End If
                    Next
                End If
            End If

            UpdateFilesListCountHeader()
            If chkSortFileListingAfterAddingFilesToHash.Checked Then SortLogsByFileSize(1, sortOrderForListFiles, listFiles)
        End Using
    End Sub

    Private Function GetDataFromAllTheHashes(checksum As HashAlgorithmName, allTheHashes As AllTheHashes) As String
        Select Case checksum
            Case HashAlgorithmName.MD5
                Return allTheHashes.Md5
            Case HashAlgorithmName.SHA1
                Return allTheHashes.Sha160
            Case HashAlgorithmName.SHA256
                Return allTheHashes.Sha256
            Case HashAlgorithmName.SHA384
                Return allTheHashes.Sha384
            Case HashAlgorithmName.SHA512
                Return allTheHashes.Sha512
            Case Else
                Return Nothing
        End Select
    End Function

    Private Sub ChkAutoScroll_Click(sender As Object, e As EventArgs) Handles ChkAutoScroll.Click
        My.Settings.boolAutoScroll = ChkAutoScroll.Checked
    End Sub

    Private Sub BtnComputeHash_Click(sender As Object, e As EventArgs) Handles btnComputeHash.Click
        If btnComputeHash.Text = "Abort Processing" Then
            Dim result As MsgBoxResult = MsgBox("Are you sure you want to abort processing?", MsgBoxStyle.Question Or MsgBoxStyle.YesNo, strMessageBoxTitleText)

            If result = MsgBoxResult.Yes AndAlso workingThread IsNot Nothing Then
                boolAbortThread = True
                boolBackgroundThreadWorking = False
            End If

            Exit Sub
        End If

        btnComputeHash.Text = "Abort Processing"
        btnAddFilesInFolder.Enabled = False
        btnAddIndividualFiles.Enabled = False
        btnRemoveAllFiles.Enabled = False
        btnRemoveSelectedFiles.Enabled = False
        btnIndividualFilesCopyToClipboard.Enabled = False
        btnIndividualFilesSaveResultsToDisk.Enabled = False
        hashIndividualFilesAllFilesProgressBar.Visible = True
        IndividualFilesProgressBar.Visible = True
        lblHashIndividualFilesTotalStatus.Visible = True
        lblIndividualFilesStatus.Visible = True
        lblIndividualFilesStatusProcessingFile.Visible = True
        intCurrentlyActiveTab = TabNumberHashIndividualFilesTab

        Dim longErroredFiles As Long = 0
        Dim itemOnGUI As MyDataGridViewRow = Nothing
        Dim currentItem As MyDataGridViewRow = Nothing
        Dim intIndexBeingWorkedOn As Integer

        workingThread = New Threading.Thread(Sub()
                                                 Try
                                                     boolBackgroundThreadWorking = True
                                                     Dim percentage, allBytesPercentage As Double
                                                     Dim strChecksum As String = Nothing
                                                     Dim checksumType As HashAlgorithmName
                                                     Dim index As Integer = 1
                                                     Dim myStopWatch As Stopwatch = Stopwatch.StartNew
                                                     Dim computeStopwatch As Stopwatch
                                                     Dim allTheHashes As AllTheHashes = Nothing
                                                     Dim fileCountPercentage As Double
                                                     Dim exceptionObject As Exception = Nothing

                                                     SyncLock threadLockingObject
                                                         longAllReadBytes = 0
                                                         longAllBytes = 0
                                                     End SyncLock

                                                     Dim subRoutine As New ChecksumStatusUpdaterDelegate(Sub(size As Long, totalBytesRead As Long)
                                                                                                             Try
                                                                                                                 MyInvoke(Sub()
                                                                                                                              percentage = If(totalBytesRead = 0 Or size = 0, 0, totalBytesRead / size * 100) ' This fixes a possible divide by zero exception.
                                                                                                                              IndividualFilesProgressBar.Value = percentage

                                                                                                                              SyncLock threadLockingObject
                                                                                                                                  allBytesPercentage = If(longAllReadBytes = 0 Or longAllBytes = 0, 100, longAllReadBytes / longAllBytes * 100)
                                                                                                                                  lblHashIndividualFilesTotalStatus.Text = $"{FileSizeToHumanSize(longAllReadBytes)} of {FileSizeToHumanSize(longAllBytes)} ({MyRoundingFunction(allBytesPercentage, byteRoundPercentages)}%) has been processed."
                                                                                                                                  If chkShowPercentageInWindowTitleBar.Checked Then Text = $"{strWindowTitle} ({MyRoundingFunction(allBytesPercentage, byteRoundPercentages)}% Completed)"
                                                                                                                              End SyncLock

                                                                                                                              ProgressForm.SetTaskbarProgressBarValue(allBytesPercentage)
                                                                                                                              hashIndividualFilesAllFilesProgressBar.Value = allBytesPercentage
                                                                                                                              lblIndividualFilesStatus.Text = $"{FileSizeToHumanSize(totalBytesRead)} of {FileSizeToHumanSize(size)} ({MyRoundingFunction(percentage, byteRoundPercentages)}%) has been processed."
                                                                                                                          End Sub, Me)

                                                                                                                 MyInvoke(Sub()
                                                                                                                              If chkShowFileProgressInFileList.Checked Then
                                                                                                                                  currentItem.Cells(2).Value = lblIndividualFilesStatus.Text
                                                                                                                                  itemOnGUI.Cells(2).Value = currentItem.Cells(2).Value
                                                                                                                              End If
                                                                                                                          End Sub, listFiles)
                                                                                                             Catch ex As Exception
                                                                                                             End Try
                                                                                                         End Sub)

                                                     MyInvoke(Sub()
                                                                  radioMD5.Enabled = False
                                                                  radioSHA1.Enabled = False
                                                                  radioSHA256.Enabled = False
                                                                  radioSHA384.Enabled = False
                                                                  radioSHA512.Enabled = False

                                                                  If radioMD5.Checked Then
                                                                      checksumType = HashAlgorithmName.MD5
                                                                  ElseIf radioSHA1.Checked Then
                                                                      checksumType = HashAlgorithmName.SHA1
                                                                  ElseIf radioSHA256.Checked Then
                                                                      checksumType = HashAlgorithmName.SHA256
                                                                  ElseIf radioSHA384.Checked Then
                                                                      checksumType = HashAlgorithmName.SHA384
                                                                  ElseIf radioSHA512.Checked Then
                                                                      checksumType = HashAlgorithmName.SHA512
                                                                  End If
                                                              End Sub, Me)

                                                     Dim myItem As MyDataGridViewRow

                                                     SyncLock threadLockingObject
                                                         For Each item As DataGridViewRow In listFiles.Rows
                                                             If boolAbortThread Then Throw New MyThreadAbortException()
                                                             If Not String.IsNullOrWhiteSpace(item.Cells(0).Value) Then
                                                                 myItem = DirectCast(item, MyDataGridViewRow)
                                                                 If String.IsNullOrWhiteSpace(myItem.Hash) And IO.File.Exists(myItem.FileName) Then longAllBytes += myItem.FileSize
                                                             End If
                                                         Next
                                                     End SyncLock

                                                     For Each item As DataGridViewRow In listFiles.Rows
                                                         If boolAbortThread Then Throw New MyThreadAbortException()

                                                         If Not String.IsNullOrWhiteSpace(item.Cells(0).Value) Then
                                                             myItem = DirectCast(item, MyDataGridViewRow)
                                                             currentItem = item
                                                             intIndexBeingWorkedOn = item.Index
                                                             itemOnGUI = Nothing
                                                             MyInvoke(Sub() itemOnGUI = listFiles.Rows(item.Index), Me)

                                                             SyncLock threadLockingObject
                                                                 If Not IO.File.Exists(myItem.FileName) Then longAllBytes -= myItem.FileSize
                                                             End SyncLock

                                                             If String.IsNullOrWhiteSpace(myItem.Hash) And IO.File.Exists(myItem.FileName) Then
                                                                 MyInvoke(Sub()
                                                                              item.Cells(3).Value = strCurrentlyBeingProcessed
                                                                              fileCountPercentage = index / listFiles.Rows.Count * 100
                                                                              lblProcessingFile.Text = $"Now processing file ""{New IO.FileInfo(myItem.FileName).Name}""."
                                                                              lblIndividualFilesStatusProcessingFile.Text = GenerateProcessingFileString(index, listFiles.Rows.Count)

                                                                              UpdateDataGridViewRow(itemOnGUI, item, False)
                                                                          End Sub, listFiles)

                                                                 computeStopwatch = Stopwatch.StartNew

                                                                 If DoChecksumWithAttachedSubRoutine(myItem.FileName, allTheHashes, subRoutine, exceptionObject) Then
                                                                     MyInvoke(Sub()
                                                                                  myItem.AllTheHashes = allTheHashes
                                                                                  strChecksum = GetDataFromAllTheHashes(checksumType, allTheHashes)
                                                                                  myItem.Cells(2).Value = If(chkDisplayHashesInUpperCase.Checked, strChecksum.ToUpper, strChecksum.ToLower)
                                                                                  myItem.ComputeTime = computeStopwatch.Elapsed
                                                                                  myItem.Cells(3).Value = TimespanToHMS(myItem.ComputeTime)
                                                                                  myItem.Hash = strChecksum
                                                                                  myItem.BoolExceptionOccurred = False
                                                                                  myItem.StrCrashData = Nothing
                                                                              End Sub, listFiles)
                                                                 Else
                                                                     MyInvoke(Sub()
                                                                                  myItem.Cells(2).Value = If(exceptionObject.GetType IsNot Nothing, $"(An error occurred while calculating checksum, {exceptionObject.GetType})", "(An error occurred while calculating checksum, unknown exception type)")
                                                                                  myItem.Cells(3).Value = ""
                                                                                  myItem.ComputeTime = Nothing
                                                                                  myItem.BoolExceptionOccurred = True
                                                                                  myItem.StrCrashData = $"{exceptionObject.Message}{vbCrLf}{exceptionObject.StackTrace}"
                                                                                  longErroredFiles += 1
                                                                              End Sub, listFiles)
                                                                 End If

                                                                 MyInvoke(Sub()
                                                                              If ChkAutoScroll.Checked Then listFiles.FirstDisplayedScrollingRowIndex = myItem.Index
                                                                              UpdateDataGridViewRow(itemOnGUI, item, False)
                                                                          End Sub, listFiles)
                                                             End If
                                                         End If

                                                         index += 1
                                                     Next

                                                     GC.Collect()

                                                     MyInvoke(Sub()
                                                                  btnIndividualFilesCopyToClipboard.Enabled = True
                                                                  btnIndividualFilesSaveResultsToDisk.Enabled = True
                                                                  radioMD5.Enabled = True
                                                                  radioSHA1.Enabled = True
                                                                  radioSHA256.Enabled = True
                                                                  radioSHA384.Enabled = True
                                                                  radioSHA512.Enabled = True

                                                                  Text = strWindowTitle
                                                                  ResetHashIndividualFilesProgress()
                                                                  boolBackgroundThreadWorking = False
                                                                  workingThread = Nothing

                                                                  If longErroredFiles = 0 Then
                                                                      MsgBox($"Completed in {TimespanToHMS(myStopWatch.Elapsed)}.", MsgBoxStyle.Information, strMessageBoxTitleText)
                                                                  Else
                                                                      MsgBox($"Completed in {TimespanToHMS(myStopWatch.Elapsed)}.{DoubleCRLF}{MyToString(longErroredFiles)} {If(longErroredFiles = 1, "file", "files")} experienced a general I/O error while processing.", MsgBoxStyle.Information, strMessageBoxTitleText)
                                                                  End If
                                                              End Sub, Me)
                                                 Catch ex As MyThreadAbortException
                                                     MyInvoke(Sub()
                                                                  If Not boolClosingWindow Then
                                                                      lblProcessingFile.Text = Nothing
                                                                      lblIndividualFilesStatus.Text = strNoBackgroundProcesses
                                                                      lblIndividualFilesStatusProcessingFile.Visible = False
                                                                      hashIndividualFilesAllFilesProgressBar.Visible = False
                                                                      IndividualFilesProgressBar.Value = 0
                                                                      IndividualFilesProgressBar.Visible = False
                                                                      ProgressForm.SetTaskbarProgressBarValue(0)
                                                                      ResetHashIndividualFilesProgress()
                                                                      Text = strWindowTitle

                                                                      If currentItem IsNot Nothing Then currentItem.Cells(2).Value = strWaitingToBeProcessed
                                                                      UpdateDataGridViewRow(itemOnGUI, currentItem, False)

                                                                      Dim intNumberOfItemsWithoutHash As Integer = listFiles.Rows.Cast(Of MyDataGridViewRow).Where(Function(item As MyDataGridViewRow) String.IsNullOrWhiteSpace(item.AllTheHashes.Sha160)).Count
                                                                      btnComputeHash.Enabled = intNumberOfItemsWithoutHash > 0
                                                                  End If

                                                                  boolBackgroundThreadWorking = False
                                                                  workingThread = Nothing
                                                                  If Not boolClosingWindow Then MsgBox("Processing aborted.", MsgBoxStyle.Information, strMessageBoxTitleText)
                                                              End Sub, Me)
                                                 Finally
                                                     boolAbortThread = False
                                                     itemOnGUI = Nothing
                                                     intCurrentlyActiveTab = TabNumberNull
                                                     SyncLock threadLockingObject
                                                         longAllReadBytes = 0
                                                         longAllBytes = 0
                                                     End SyncLock

                                                     MyInvoke(Sub()
                                                                  If Not boolClosingWindow Then
                                                                      btnComputeHash.Text = "Compute Hash"
                                                                      ProgressForm.SetTaskbarProgressBarValue(0)
                                                                      hashIndividualFilesAllFilesProgressBar.Value = 0
                                                                  End If
                                                              End Sub, Me)
                                                 End Try
                                             End Sub) With {
            .Priority = GetThreadPriority(),
            .Name = "Hash Generation Thread",
            .IsBackground = True
        }
        workingThread.Start()
    End Sub

    Sub ResetHashIndividualFilesProgress()
        btnAddFilesInFolder.Enabled = True
        btnAddIndividualFiles.Enabled = True
        btnRemoveAllFiles.Enabled = True
        btnRemoveSelectedFiles.Enabled = True

        lblIndividualFilesStatus.Visible = False
        lblIndividualFilesStatusProcessingFile.Visible = False
        hashIndividualFilesAllFilesProgressBar.Visible = False
        lblProcessingFile.Text = Nothing
        IndividualFilesProgressBar.Value = 0
        IndividualFilesProgressBar.Visible = False
        ProgressForm.SetTaskbarProgressBarValue(0)
        lblHashIndividualFilesTotalStatus.Visible = False
    End Sub

    Private Function GetMyHashArray(strPathOfChecksumFile As String) As List(Of MyHash)
        Dim strDirectoryName As String = New IO.FileInfo(strPathOfChecksumFile).DirectoryName
        Dim folderOfChecksumFile As String = If(strDirectoryName.Length = 3, strDirectoryName, $"{strDirectoryName}\")

        Dim myHashList As New List(Of MyHash)

        For Each item As MyDataGridViewRow In listFiles.Rows
            If chkSaveChecksumFilesWithRelativePaths.Checked Then
                myHashList.Add(New MyHash With {
                    .FileName = item.FileName.Replace(folderOfChecksumFile, "", StringComparison.OrdinalIgnoreCase),
                    .FileHash = item.AllTheHashes.Sha512
                })
            Else
                myHashList.Add(New MyHash With {
                    .FileName = item.FileName,
                    .FileHash = item.AllTheHashes.Sha512
                })
            End If
        Next

        Return myHashList
    End Function

    Private Function StrGetIndividualHashesInStringFormat(strPathOfChecksumFile As String, checksumType As HashAlgorithmName) As String
        Dim strDirectoryName As String = New IO.FileInfo(strPathOfChecksumFile).DirectoryName
        Dim folderOfChecksumFile As String = If(strDirectoryName.Length = 3, strDirectoryName, $"{strDirectoryName}\")
        Dim stringBuilder As New Text.StringBuilder()
        Dim strFile As String

        AddHashFileHeader(stringBuilder, listFiles.Rows.Count)

        For Each item As MyDataGridViewRow In listFiles.Rows
            If Not String.IsNullOrWhiteSpace(item.Hash) Then
                strFile = item.FileName
                If chkSaveChecksumFilesWithRelativePaths.Checked Then strFile = strFile.Replace(folderOfChecksumFile, "", StringComparison.OrdinalIgnoreCase)
                stringBuilder.AppendLine($"{GetDataFromAllTheHashes(checksumType, item.AllTheHashes)} *{strFile}")
            End If
        Next

        AddEndOfHashLines(stringBuilder)
        Return stringBuilder.ToString()
    End Function

    Private Function StrGetIndividualHashesInStringFormat() As String
        Dim stringBuilder As New Text.StringBuilder()

        AddHashFileHeader(stringBuilder, listFiles.Rows.Count)

        For Each item As MyDataGridViewRow In listFiles.Rows
            If Not String.IsNullOrWhiteSpace(item.Hash) Then
                stringBuilder.AppendLine($"{item.Hash} *{item.FileName}")
            End If
        Next

        AddEndOfHashLines(stringBuilder)
        Return stringBuilder.ToString()
    End Function

    Private Sub AddEndOfHashLines(ByRef stringBuilder As Text.StringBuilder)
        stringBuilder.AppendLine("'")
        stringBuilder.AppendLine("' End of hash data.")
        stringBuilder.AppendLine("'")
    End Sub

    Private Sub BtnIndividualFilesCopyToClipboard_Click(sender As Object, e As EventArgs) Handles btnIndividualFilesCopyToClipboard.Click
        If CopyTextToWindowsClipboard(StrGetIndividualHashesInStringFormat().Trim) Then MsgBox("Your hash results have been copied to the Windows Clipboard.", MsgBoxStyle.Information, strMessageBoxTitleText)
    End Sub

    Private Shared Function CopyTextToWindowsClipboard(strTextToBeCopiedToClipboard As String) As Boolean
        Try
            Clipboard.SetDataObject(strTextToBeCopiedToClipboard, True, 5, 200)
            Return True
        Catch ex As Exception
            MsgBox("Unable to open Windows Clipboard to copy text to it.", MsgBoxStyle.Critical, strMessageBoxTitleText)
            Return False
        End Try
    End Function

    Private Sub BtnIndividualFilesSaveResultsToDisk_Click(sender As Object, e As EventArgs) Handles btnIndividualFilesSaveResultsToDisk.Click
        Using SaveFileDialog As New SaveFileDialog
            SaveFileDialog.Filter = "MD5 File|*.md5|SHA1 File|*.sha1|SHA256 File|*.sha256|SHA384 File|*.sha384|SHA512 File|*.sha512|Hasher File|*.hasher"
            SaveFileDialog.InitialDirectory = strLastDirectoryWorkedOn
            SaveFileDialog.Title = "Save Hash Results to Disk"
            SaveFileDialog.FilterIndex = ChecksumFilterIndexSHA256
            If chkAutoAddExtension.Checked Then SaveFileDialog.OverwritePrompt = False ' We handle this in our own code below.

            Dim strFileExtension As String
            Dim checksumType As HashAlgorithmName

            If Not String.IsNullOrWhiteSpace(strLastHashFileLoaded) Then
                strFileExtension = New IO.FileInfo(strLastHashFileLoaded).Extension

                Select Case strFileExtension.Trim().ToLower()
                    Case ".md5"
                        SaveFileDialog.FilterIndex = ChecksumFilterIndexMD5
                    Case ".sha1"
                        SaveFileDialog.FilterIndex = ChecksumFilterIndexSHA160
                    Case ".sha256"
                        SaveFileDialog.FilterIndex = ChecksumFilterIndexSHA256
                    Case ".sha384"
                        SaveFileDialog.FilterIndex = ChecksumFilterIndexSHA384
                    Case ".sha512"
                        SaveFileDialog.FilterIndex = ChecksumFilterIndexSHA512
                    Case ".hasher"
                        SaveFileDialog.FilterIndex = ChecksumFilterIndexSHA512
                End Select

                SaveFileDialog.FileName = strLastHashFileLoaded
            Else
                If radioMD5.Checked Then
                    SaveFileDialog.FilterIndex = ChecksumFilterIndexMD5
                ElseIf radioSHA1.Checked Then
                    SaveFileDialog.FilterIndex = ChecksumFilterIndexSHA160
                ElseIf radioSHA256.Checked Then
                    SaveFileDialog.FilterIndex = ChecksumFilterIndexSHA256
                ElseIf radioSHA384.Checked Then
                    SaveFileDialog.FilterIndex = ChecksumFilterIndexSHA384
                ElseIf radioSHA512.Checked Then
                    SaveFileDialog.FilterIndex = ChecksumFilterIndexSHA512
                End If
            End If

            If SaveFileDialog.ShowDialog() = DialogResult.OK Then
                If SaveFileDialog.FilterIndex = ChecksumFilterIndexMD5 Or SaveFileDialog.FilterIndex = ChecksumFilterIndexSHA160 Then
                    Dim MsgBoxQuestionResult As MsgBoxResult

                    If SaveFileDialog.FilterIndex = ChecksumFilterIndexMD5 Then
                        MsgBoxQuestionResult = MsgBox($"MD5 is not recommended for hashing files.{DoubleCRLF}Are you sure you want to use this hash type?", MsgBoxStyle.Question + MsgBoxStyle.YesNo, strMessageBoxTitleText)
                    ElseIf SaveFileDialog.FilterIndex = ChecksumFilterIndexSHA160 Then
                        MsgBoxQuestionResult = MsgBox($"SHA1 is not recommended for hashing files.{DoubleCRLF}Are you sure you want to use this hash type?", MsgBoxStyle.Question + MsgBoxStyle.YesNo, strMessageBoxTitleText)
                    End If

                    If MsgBoxQuestionResult = MsgBoxResult.No Then
                        MsgBox("Your hash data has not been saved to disk.", MsgBoxStyle.Information, strMessageBoxTitleText)
                        Exit Sub
                    End If
                End If

                If chkAutoAddExtension.Checked Then
                    strFileExtension = New IO.FileInfo(SaveFileDialog.FileName).Extension

                    If Not strFileExtension.Equals(".md5", StringComparison.OrdinalIgnoreCase) And Not strFileExtension.Equals(".sha1", StringComparison.OrdinalIgnoreCase) And Not strFileExtension.Equals(".sha256", StringComparison.OrdinalIgnoreCase) And Not strFileExtension.Equals(".sha384", StringComparison.OrdinalIgnoreCase) And Not strFileExtension.Equals(".sha512", StringComparison.OrdinalIgnoreCase) And Not strFileExtension.Equals(".hasher", StringComparison.OrdinalIgnoreCase) Then
                        Select Case SaveFileDialog.FilterIndex
                            Case ChecksumFilterIndexMD5
                                SaveFileDialog.FileName &= ".md5"
                            Case ChecksumFilterIndexSHA160
                                SaveFileDialog.FileName &= ".sha1"
                            Case ChecksumFilterIndexSHA256
                                SaveFileDialog.FileName &= ".sha256"
                            Case ChecksumFilterIndexSHA384
                                SaveFileDialog.FileName &= ".sha384"
                            Case ChecksumFilterIndexSHA512
                                SaveFileDialog.FileName &= ".sha512"
                            Case ChecksumFilterIndexSHA512
                                SaveFileDialog.FileName &= ".hasher"
                        End Select
                    End If

                    Media.SystemSounds.Exclamation.Play()

                    If IO.File.Exists(SaveFileDialog.FileName) AndAlso MsgBox($"The file named ""{New IO.FileInfo(SaveFileDialog.FileName).Name}"" already exists.{DoubleCRLF}Are you absolutely sure you want to replace it?", MsgBoxStyle.Question + MsgBoxStyle.YesNo, "Overwrite?") = MsgBoxResult.No Then
                        MsgBox("Save Results to Disk Aborted.", MsgBoxStyle.Information, strMessageBoxTitleText)
                        Exit Sub
                    End If
                End If

                Dim fileInfo As New IO.FileInfo(SaveFileDialog.FileName)
                strFileExtension = fileInfo.Extension

                Select Case strFileExtension.Trim().ToLower()
                    Case ".md5"
                        checksumType = HashAlgorithmName.MD5
                    Case ".sha1"
                        checksumType = HashAlgorithmName.SHA1
                    Case ".sha256"
                        checksumType = HashAlgorithmName.SHA256
                    Case ".sha384"
                        checksumType = HashAlgorithmName.SHA384
                    Case ".sha512"
                        checksumType = HashAlgorithmName.SHA512
                    Case ".hasher"
                        checksumType = HashAlgorithmName.SHA512
                End Select

                If strFileExtension.Equals(".hasher", StringComparison.OrdinalIgnoreCase) Then
                    Using fileStream As New IO.StreamWriter(SaveFileDialog.FileName)
                        fileStream.Write(Newtonsoft.Json.JsonConvert.SerializeObject(GetMyHashArray(SaveFileDialog.FileName), Newtonsoft.Json.Formatting.Indented))
                    End Using
                Else
                    Using streamWriter As New IO.StreamWriter(SaveFileDialog.FileName, False, System.Text.Encoding.UTF8)
                        streamWriter.Write(StrGetIndividualHashesInStringFormat(SaveFileDialog.FileName, checksumType))
                    End Using
                End If

                Dim openInExplorerMsgBoxResult As MsgBoxResult

                If chkOpenInExplorer.Checked Then
                    openInExplorerMsgBoxResult = MsgBoxResult.Yes
                    ShowHashFileWrittenWindow(False, SaveFileDialog.FileName, checksumType)
                Else
                    openInExplorerMsgBoxResult = ShowHashFileWrittenWindow(True, SaveFileDialog.FileName, checksumType)
                End If

                If openInExplorerMsgBoxResult = MsgBoxResult.Yes Then SelectFileInWindowsExplorer(fileInfo.FullName)
            End If
        End Using
    End Sub

    Private Function ShowHashFileWrittenWindow(BoolAskUserOpenInExplorer As Boolean, fileName As String, checksumtype As HashAlgorithmName) As MsgBoxResult
        Dim StringBuilder As New Text.StringBuilder()
        StringBuilder.AppendLine("Your hash results have been written to disk.")
        StringBuilder.AppendLine()
        StringBuilder.AppendLine($"File Name: {fileName}")
        StringBuilder.AppendLine($"Checksum Type: {ConvertChecksumTypeToString(checksumtype)}")
        StringBuilder.AppendLine()

        If BoolAskUserOpenInExplorer Then
            StringBuilder.AppendLine("Do you want to open Windows Explorer to the location of the checksum file?")
            Return MsgBox(StringBuilder.ToString.Trim, MsgBoxStyle.Question + MsgBoxStyle.YesNo + MsgBoxStyle.DefaultButton2, strMessageBoxTitleText)
        Else
            StringBuilder.AppendLine("Windows Explorer will now open to the location of the checksum file.")
            Return MsgBox(StringBuilder.ToString.Trim, MsgBoxStyle.Information, strMessageBoxTitleText)
        End If

        Return MsgBoxResult.Yes
    End Function

    Private Function ConvertChecksumTypeToString(checksumType As HashAlgorithmName) As String
        Select Case checksumType
            Case HashAlgorithmName.MD5
                Return "MD5"
            Case HashAlgorithmName.SHA1
                Return "SHA1/SHA160"
            Case HashAlgorithmName.SHA256
                Return "SHA256"
            Case HashAlgorithmName.SHA384
                Return "SHA384"
            Case HashAlgorithmName.SHA512
                Return "SHA512"
            Case Else
                Return ""
        End Select
    End Function

    Private Sub UpdateChecksumsInListFiles(checksumType As HashAlgorithmName)
        If listFiles.Rows.Count <> 0 Then
            Dim strChecksum As String
            Dim dataGridRow As MyDataGridViewRow

            For Each item As DataGridViewRow In listFiles.Rows
                If Not String.IsNullOrWhiteSpace(item.Cells(0).Value) Then
                    dataGridRow = DirectCast(item, MyDataGridViewRow)
                    strChecksum = GetDataFromAllTheHashes(checksumType, dataGridRow.AllTheHashes)

                    If Not String.IsNullOrWhiteSpace(strChecksum) Then
                        dataGridRow.Hash = strChecksum
                        dataGridRow.Cells(2).Value = If(chkDisplayHashesInUpperCase.Checked, strChecksum.ToUpper, strChecksum.ToLower)
                    End If

                    dataGridRow = Nothing
                End If
            Next
        End If
    End Sub

    Private Sub RadioMD5_Click(sender As Object, e As EventArgs) Handles radioMD5.Click
        UpdateChecksumsInListFiles(HashAlgorithmName.MD5)
        colChecksum.HeaderText = strColumnTitleChecksumMD5
    End Sub

    Private Sub RadioSHA1_Click(sender As Object, e As EventArgs) Handles radioSHA1.Click
        UpdateChecksumsInListFiles(HashAlgorithmName.SHA1)
        colChecksum.HeaderText = strColumnTitleChecksumSHA160
    End Sub

    Private Sub RadioSHA256_Click(sender As Object, e As EventArgs) Handles radioSHA256.Click
        UpdateChecksumsInListFiles(HashAlgorithmName.SHA256)
        colChecksum.HeaderText = strColumnTitleChecksumSHA256
    End Sub

    Private Sub RadioSHA384_Click(sender As Object, e As EventArgs) Handles radioSHA384.Click
        UpdateChecksumsInListFiles(HashAlgorithmName.SHA384)
        colChecksum.HeaderText = strColumnTitleChecksumSHA384
    End Sub

    Private Sub RadioSHA512_Click(sender As Object, e As EventArgs) Handles radioSHA512.Click
        UpdateChecksumsInListFiles(HashAlgorithmName.SHA512)
        colChecksum.HeaderText = strColumnTitleChecksumSHA512
    End Sub

    Private Sub LaunchURLInWebBrowser(url As String, Optional errorMessage As String = "An error occurred when trying the URL In your Default browser. The URL has been copied to your Windows Clipboard for you to paste into the address bar in the web browser of your choice.")
        If Not url.Trim.StartsWith("http", StringComparison.OrdinalIgnoreCase) Then url = $"https://{url}"

        Try
            Process.Start(url)
        Catch ex As Exception
            CopyTextToWindowsClipboard(url)
            MsgBox(errorMessage, MsgBoxStyle.Critical, strMessageBoxTitleText)
        End Try
    End Sub

    Private Sub BtnDonate_Click(sender As Object, e As EventArgs) Handles btnDonate.Click
        LaunchURLInWebBrowser(strPayPal)
    End Sub

    Private Sub SendToIPCNamedPipeServer(strMessageToSend As String)
        Try
            Using memoryStream As New IO.MemoryStream(System.Text.Encoding.UTF8.GetBytes(strMessageToSend))
                Dim namedPipeDataStream As New NamedPipeClientStream(".", strNamedPipeServerName, PipeDirection.Out, PipeOptions.Asynchronous)
                namedPipeDataStream.Connect(5000)
                memoryStream.CopyTo(namedPipeDataStream)
            End Using
        Catch ex As IO.IOException
            MsgBox("There was an error sending data to the named pipe server used for interprocess communication, please close all Hasher instances and try again.", MsgBoxStyle.Critical, strMessageBoxTitleText)
        End Try
    End Sub

    ''' <summary>
    ''' This function will act upon either a file or a directory path.
    ''' If it's passed a directory path it will call the addFilesFromDirectory() function.
    ''' If it's passed a file path it will process and add the file to the list by itself
    ''' with an included call to the createListFilesObject() function.
    ''' </summary>
    ''' <param name="strReceivedFileName">This parameter contains the path of either a file or a directory.</param>
    Private Sub AddFileOrDirectoryToHashFileList(strReceivedFileName As String)
        Try
            If boolBackgroundThreadWorking Then Exit Sub
            If IO.File.Exists(strReceivedFileName) Or IO.Directory.Exists(strReceivedFileName) Then
                TabControl1.SelectTab(TabNumberHashIndividualFilesTab)
                NativeMethod.NativeMethods.SetForegroundWindow(Handle.ToInt32())

                If Not IO.File.GetAttributes(strReceivedFileName).HasFlag(IO.FileAttributes.Directory) AndAlso Not filesInListFiles.Contains(strReceivedFileName.Trim.ToLower) Then
                    strLastDirectoryWorkedOn = New IO.FileInfo(strReceivedFileName).DirectoryName
                    If IO.File.Exists(strReceivedFileName) Then listFiles.Rows.Add(CreateListFilesObject(strReceivedFileName, listFiles))
                Else
                    AddFilesFromDirectory(strReceivedFileName)
                End If

                UpdateFilesListCountHeader()
                If chkSortFileListingAfterAddingFilesToHash.Checked Then SortLogsByFileSize(1, sortOrderForListFiles, listFiles)
            End If
        Catch ex As Exception
        End Try
    End Sub

    Private Sub CallSaveColumnOrders()
        My.Settings.listFilesColumnOrder = SaveColumnOrders(listFiles.Columns)
        My.Settings.verifyListFilesColumnOrder = SaveColumnOrders(verifyHashesListFiles.Columns)
    End Sub

    Private Sub Form1_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        CallSaveColumnOrders()
    End Sub

    ''' <summary>Creates a named pipe server. Returns a Boolean value indicating if the function was able to create a named pipe server.</summary>
    ''' <returns>Returns a Boolean value indicating if the function was able to create a named pipe server.</returns>
    Private Function StartNamedPipeServer() As Boolean
        Try
            Dim pipeServer As New NamedPipeServerStream(strNamedPipeServerName, PipeDirection.In, 1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous)
            pipeServer.BeginWaitForConnection(New AsyncCallback(AddressOf WaitForConnectionCallBack), pipeServer)
            Return True ' We were able to create a named pipe server. Yay!
        Catch oEX As Exception
            Return False ' OK, there's already a named pipe server in operation already so we return a False value.
        End Try
    End Function

    Private Sub WaitForConnectionCallBack(iar As IAsyncResult)
        Try
            Dim namedPipeServer As NamedPipeServerStream = CType(iar.AsyncState, NamedPipeServerStream)
            namedPipeServer.EndWaitForConnection(iar)
            Dim buffer As Byte() = New Byte(499) {}
            namedPipeServer.Read(buffer, 0, 500)

            Dim strReceivedMessage As String = System.Text.Encoding.UTF8.GetString(buffer, 0, buffer.Length).Replace(vbNullChar, "").Trim
            Dim parsedArguments As New Dictionary(Of String, Object)(StringComparer.OrdinalIgnoreCase)

            parsedArguments = Newtonsoft.Json.JsonConvert.DeserializeObject(Of Dictionary(Of String, Object))(strReceivedMessage)

            If parsedArguments.ContainsKey("comparefile") Then
                MyInvoke(Sub()
                             Dim strFilePathToBeCompared As String = parsedArguments("comparefile")

                             If String.IsNullOrWhiteSpace(txtFile1.Text) And String.IsNullOrWhiteSpace(txtFile2.Text) Then
                                 txtFile1.Text = strFilePathToBeCompared
                             ElseIf String.IsNullOrWhiteSpace(txtFile1.Text) And Not String.IsNullOrWhiteSpace(txtFile2.Text) Then
                                 txtFile1.Text = strFilePathToBeCompared
                             ElseIf Not String.IsNullOrWhiteSpace(txtFile1.Text) And String.IsNullOrWhiteSpace(txtFile2.Text) Then
                                 txtFile2.Text = strFilePathToBeCompared
                             End If

                             TabControl1.SelectedIndex = TabNumberCompareFilesTab
                             If Not String.IsNullOrWhiteSpace(txtFile1.Text) AndAlso Not String.IsNullOrWhiteSpace(txtFile2.Text) Then btnCompareFiles.PerformClick()
                         End Sub, Me)
            ElseIf parsedArguments.ContainsKey("addfile") Then
                AddFileOrDirectoryToHashFileList(parsedArguments("addfile"))
            End If

            namedPipeServer.Dispose()
            namedPipeServer = New NamedPipeServerStream(strNamedPipeServerName, PipeDirection.In, 1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous)
            namedPipeServer.BeginWaitForConnection(New AsyncCallback(AddressOf WaitForConnectionCallBack), namedPipeServer)
        Catch
            Return
        End Try
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim parsedArguments As Dictionary(Of String, Object) = ParseArguments(My.Application.CommandLineArgs)

        ' This function returns a Boolean value indicating if the name pipe server was started or not.
        Dim boolNamedPipeServerStarted As Boolean = StartNamedPipeServer()
        Dim commandLineArgument As String

        Dim flags As BindingFlags = BindingFlags.NonPublic Or BindingFlags.Instance Or BindingFlags.SetProperty
        Dim propInfo As PropertyInfo = GetType(DataGridView).GetProperty("DoubleBuffered", flags)
        propInfo?.SetValue(verifyHashesListFiles, True, Nothing)
        propInfo?.SetValue(listFiles, True, Nothing)

        If parsedArguments.Any() Then
            If parsedArguments.ContainsKey("addfile") Or parsedArguments.ContainsKey("comparefile") Then
                If boolNamedPipeServerStarted Then
                    ' This instance of the program is the first executed instance so it's the host of the named pipe server.
                    ' We still need to process the first incoming file passed to it via command line arguments. After doing
                    ' so, this instance of the program will continue operating as the host of the named pipe server.
                    If parsedArguments.ContainsKey("addfile") Then
                        ' We now have to strip off what we don't need.
                        commandLineArgument = parsedArguments("addfile")
                        MyInvoke(Sub() AddFileOrDirectoryToHashFileList(commandLineArgument), Me)
                    ElseIf parsedArguments.ContainsKey("comparefile") Then
                        ' We now have to strip off what we don't need.
                        commandLineArgument = parsedArguments("comparefile")
                        txtFile1.Text = commandLineArgument
                    End If
                Else
                    ' This instance of the program isn't the first running instance, this instance is a secondary instance
                    ' for the lack of a better word. However, this instance has received data from Windows Explorer so we
                    ' need to do something with it, namely pass that data to the first running instance via the named
                    ' pipe and then terminate itself.
                    SendToIPCNamedPipeServer(Newtonsoft.Json.JsonConvert.SerializeObject(parsedArguments)) ' This passes the data to the named pipe server.
                    Process.GetCurrentProcess.Kill() ' This terminates the process.
                End If
            ElseIf parsedArguments.ContainsKey("hashfile") Then
                commandLineArgument = parsedArguments("hashfile")

                If IO.File.Exists(commandLineArgument) Then
                    TabControl1.SelectTab(TabNumberVerifySavedHashesTab)
                    btnOpenExistingHashFile.Text = "Abort Processing"
                    verifyHashesListFiles.Rows.Clear()
                    ProcessExistingHashFile(commandLineArgument)
                End If
            ElseIf parsedArguments.ContainsKey("knownhashfile") Then
                commandLineArgument = parsedArguments("knownhashfile")
                TabControl1.SelectTab(TabNumberCompareFileAgainstKnownHashTab)
                txtFileForKnownHash.Text = commandLineArgument
                txtKnownHash.Select()
            End If
        End If

        Icon = Icon.ExtractAssociatedIcon(Reflection.Assembly.GetExecutingAssembly().Location)
        Text = strWindowTitle

        If AreWeAnAdministrator() Then
            Text &= " (WARNING!!! Running as Administrator)"
        Else
            btnRemoveSystemLevelFileAssociations.Image = My.Resources.UAC
            btnRemoveSystemLevelFileAssociations.ImageAlign = ContentAlignment.MiddleLeft
            btnRemoveSystemLevelFileAssociations.TextAlign = ContentAlignment.MiddleRight
        End If

        lblIndividualFilesStatusProcessingFile.Visible = False
        hashIndividualFilesAllFilesProgressBar.Visible = False
        verifyIndividualFilesAllFilesProgressBar.Visible = False
        CompareFilesAllFilesProgress.Visible = False
        lblVerifyHashStatusProcessingFile.Visible = False
        lblProcessingFile.Text = Nothing
        lblProcessingFileVerify.Text = Nothing
        lblCompareFileAgainstKnownHashType.Text = Nothing
        lblCompareFilesAllFilesStatus.Text = Nothing
        lblIndividualFilesStatusProcessingFile.Text = Nothing
        lblHashIndividualFilesTotalStatus.Text = Nothing
        chkRecurrsiveDirectorySearch.Checked = My.Settings.boolRecurrsiveDirectorySearch
        chkSortByFileSizeAfterLoadingHashFile.Checked = My.Settings.boolSortByFileSizeAfterLoadingHashFile
        chkSortFileListingAfterAddingFilesToHash.Checked = My.Settings.boolSortFileListingAfterAddingFilesToHash
        chkSaveChecksumFilesWithRelativePaths.Checked = My.Settings.boolSaveChecksumFilesWithRelativePaths
        chkUseMilliseconds.Checked = My.Settings.boolUseMilliseconds
        chkDisplayHashesInUpperCase.Checked = My.Settings.boolDisplayHashesInUpperCase
        chkUseCommasInNumbers.Checked = My.Settings.boolUseCommasInNumbers
        chkCheckForUpdates.Checked = My.Settings.boolCheckForUpdates
        chkHideCheckForUpdatesButton.Checked = My.Settings.boolHideCheckForUpdatesButton
        chkHideCheckForUpdatesButton.Visible = chkCheckForUpdates.Checked
        btnCheckForUpdates.Visible = Not chkHideCheckForUpdatesButton.Checked Or Not chkCheckForUpdates.Checked
        chkAutoAddExtension.Checked = My.Settings.boolAutoAddExtension
        chkDisplayValidChecksumString.Checked = My.Settings.boolDisplayValidChecksumString
        chkOpenInExplorer.Checked = My.Settings.boolOpenInExplorer
        chkShowPercentageInWindowTitleBar.Checked = My.Settings.boolShowPercentageInWindowTitleBar
        chkShowFileProgressInFileList.Checked = My.Settings.boolShowFileProgressInFileList
        ChkIncludeEntryCountInFileNameHeader.Checked = My.Settings.boolIncludeEntryCountInFileNameHeader
        ChkComputeHashesOnCompareFilesTabEvenWithDifferentFileSizes.Checked = My.Settings.boolComputeHashesOnCompareFilesTabEvenWithDifferentFileSizes
        chkClearBeforeTransferringFromVerifyToHash.Checked = My.Settings.boolClearBeforeTransferringFromVerifyToHash
        lblWelcomeText.Text = String.Format(lblWelcomeText.Text, checkForUpdates.strDisplayVersionString)
        Size = My.Settings.windowSize
        If My.Settings.boolWindowMaximized Then WindowState = FormWindowState.Maximized
        validColor = My.Settings.validColor
        lblValidColor.BackColor = validColor
        notValidColor = My.Settings.notValidColor
        lblNotValidColor.BackColor = notValidColor
        fileNotFoundColor = My.Settings.fileNotFoundColor
        lblFileNotFoundColor.BackColor = fileNotFoundColor
        bufferSize.Value = My.Settings.shortBufferSize
        btnSetBufferSize.Enabled = False
        roundFileSizes.Value = My.Settings.roundFileSizes
        roundPercentages.Value = My.Settings.roundPercentages
        btnSetRoundFileSizes.Enabled = False
        btnSetRoundPercentages.Enabled = False
        ChkAutoScroll.Checked = My.Settings.boolAutoScroll
        Location = VerifyWindowLocation(My.Settings.windowLocation, Me)

        listFiles.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCellsExceptHeaders
        verifyHashesListFiles.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCellsExceptHeaders

        If Microsoft.Win32.Registry.LocalMachine.OpenSubKey("Software\Classes\.hasher\Shell\Verify with Hasher", False) Is Nothing Then btnRemoveSystemLevelFileAssociations.Visible = False
        If Microsoft.Win32.Registry.CurrentUser.OpenSubKey("Software\Classes\.hasher\Shell\Verify with Hasher", False) IsNot Nothing Then btnAssociate.Enabled = False
        If Microsoft.Win32.Registry.CurrentUser.OpenSubKey("Software\Classes\*\Shell\Compare Two Files", False) IsNot Nothing Then btnAddHasherToAllFiles.Enabled = False
        If Microsoft.Win32.Registry.CurrentUser.OpenSubKey("Software\Classes\.hasher\Shell\Verify with Hasher", False) Is Nothing Then btnRemoveFileAssociations.Enabled = False

        If My.Settings.defaultHash < 0 Or My.Settings.defaultHash > 4 Then My.Settings.defaultHash = Byte.Parse(2)
        defaultHashType.SelectedIndex = My.Settings.defaultHash
        SetDefaultHashTypeGUIElementOptions()

        If Debugger.IsAttached Then
            Text &= " (Debugger Attached)"
            btnAddHasherToAllFiles.Visible = False
            btnAssociate.Visible = False
            btnRemoveFileAssociations.Visible = False
            btnRemoveSystemLevelFileAssociations.Visible = False
        End If

        LoadColumnOrders(listFiles.Columns, My.Settings.listFilesColumnOrder)
        LoadColumnOrders(verifyHashesListFiles.Columns, My.Settings.verifyListFilesColumnOrder)

        colFileName.Width = My.Settings.hashIndividualFilesFileNameColumnSize
        colFileSize.Width = My.Settings.hashIndividualFilesFileSizeColumnSize
        colChecksum.Width = My.Settings.hashIndividualFilesChecksumColumnSize
        colComputeTime.Width = My.Settings.hashIndividualFilesComputeTimeColumnSize
        colNewHash.Width = My.Settings.newHashChecksumColumnSize

        colFile.Width = My.Settings.verifyHashFileNameColumnSize
        colFileSize2.Width = My.Settings.verifyHashFileSizeColumnSize
        colResults.Width = My.Settings.verifyHashFileResults
        colComputeTime2.Width = My.Settings.verifyHashComputeTimeColumnSize
        If My.Settings.taskPriority < 0 Or My.Settings.taskPriority > 4 Then My.Settings.taskPriority = Byte.Parse(4)
        taskPriority.SelectedIndex = My.Settings.taskPriority

        If My.Settings.boolCheckForUpdates Then
            Threading.ThreadPool.QueueUserWorkItem(Sub()
                                                       Dim checkForUpdatesClassObject As New checkForUpdates.CheckForUpdatesClass(Me)
                                                       checkForUpdatesClassObject.CheckForUpdates(False)
                                                   End Sub)
        End If

        boolDoneLoading = True

        If Not FileAssociation.DoesCompareFilesExist() Then
            MsgBox($"Hasher has a new function! The ability to compare two files from Windows Explorer.{DoubleCRLF}Please go to the Setting tab and click on the ""Add Hasher to All Files"" button to add support to Windows Explorer for this new feature.", MsgBoxStyle.Information, strMessageBoxTitleText)
        End If
    End Sub

    Private Sub AddFilesFromDirectory(directoryPath As String)
        workingThread = New Threading.Thread(Sub()
                                                 Dim oldFilesInListFiles As List(Of String) = filesInListFiles

                                                 Try
                                                     strLastDirectoryWorkedOn = directoryPath
                                                     Dim collectionOfDataGridRows As New List(Of MyDataGridViewRow)
                                                     Dim index As Integer = 0
                                                     boolBackgroundThreadWorking = True

                                                     MyInvoke(Sub()
                                                                  listFiles.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None
                                                                  verifyHashesListFiles.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None
                                                                  btnAddFilesInFolder.Text = "Abort Adding Files"
                                                                  btnAddIndividualFiles.Enabled = False
                                                                  btnRemoveSelectedFiles.Enabled = False
                                                                  btnRemoveAllFiles.Enabled = False
                                                                  radioSHA256.Enabled = False
                                                                  radioSHA384.Enabled = False
                                                                  radioSHA512.Enabled = False
                                                                  radioSHA1.Enabled = False
                                                                  radioMD5.Enabled = False
                                                                  btnComputeHash.Enabled = False
                                                                  btnIndividualFilesCopyToClipboard.Enabled = False
                                                                  btnIndividualFilesSaveResultsToDisk.Enabled = False

                                                                  IndividualFilesProgressBar.Visible = True
                                                                  lblIndividualFilesStatus.Visible = True
                                                                  lblProcessingFile.Text = "Enumerating files in directory... Please wait."
                                                                  lblIndividualFilesStatus.Text = Nothing
                                                              End Sub, Me)

                                                     Dim rawFilesInDirectory As IEnumerable(Of FastDirectoryEnumerator.FileData) = FastDirectoryEnumerator.EnumerateFiles(directoryPath, "*.*", If(chkRecurrsiveDirectorySearch.Checked, IO.SearchOption.AllDirectories, IO.SearchOption.TopDirectoryOnly))
                                                     Dim filesInDirectory As List(Of FastDirectoryEnumerator.FileData) = rawFilesInDirectory.Where(Function(filedata As FastDirectoryEnumerator.FileData) Not IO.Directory.Exists(filedata.Path) AndAlso Not (filedata.Name = "." OrElse filedata.Name = "..")).ToList()

                                                     Dim intFileIndexNumber As Integer = 0
                                                     Dim intTotalNumberOfFiles As Integer = filesInDirectory.Count
                                                     Dim percentage As Double

                                                     Parallel.ForEach(filesInDirectory, Sub(filedata As FastDirectoryEnumerator.FileData)
                                                                                            SyncLock collectionOfDataGridRows
                                                                                                If boolAbortThread Then Throw New MyThreadAbortException()
                                                                                                Threading.Interlocked.Increment(intFileIndexNumber)

                                                                                                MyInvoke(Sub()
                                                                                                             percentage = intFileIndexNumber / intTotalNumberOfFiles * 100
                                                                                                             IndividualFilesProgressBar.Value = percentage
                                                                                                             ProgressForm.SetTaskbarProgressBarValue(percentage)
                                                                                                             lblIndividualFilesStatus.Text = GenerateProcessingFileString(intFileIndexNumber, intTotalNumberOfFiles)
                                                                                                         End Sub, Me)

                                                                                                If Not filesInListFiles.Contains(filedata.Path.Trim.ToLower) AndAlso (filedata.Attributes And IO.FileAttributes.Hidden) <> IO.FileAttributes.Hidden Then
                                                                                                    If IO.File.Exists(filedata.Path) Then collectionOfDataGridRows.Add(CreateFilesDataGridObject(filedata.Path, filedata.Size, listFiles))
                                                                                                End If
                                                                                            End SyncLock
                                                                                        End Sub)

                                                     filesInDirectory = Nothing

                                                     MyInvoke(Sub()
                                                                  lblProcessingFile.Text = "Adding files to list... Please wait."
                                                              End Sub, Me)

                                                     Threading.Thread.Sleep(250)

                                                     collectionOfDataGridRows = collectionOfDataGridRows.Where(Function(item As MyDataGridViewRow)
                                                                                                                   Return item IsNot Nothing
                                                                                                               End Function).ToList()

                                                     MyInvoke(Sub()
                                                                  listFiles.Rows.AddRange(collectionOfDataGridRows.ToArray())
                                                                  collectionOfDataGridRows.Clear()
                                                                  collectionOfDataGridRows = Nothing
                                                                  If chkSortFileListingAfterAddingFilesToHash.Checked Then SortLogsByFileSize(1, sortOrderForListFiles, listFiles)

                                                                  UpdateFilesListCountHeader()

                                                                  btnComputeHash.Enabled = True
                                                                  btnIndividualFilesCopyToClipboard.Enabled = False
                                                                  btnIndividualFilesSaveResultsToDisk.Enabled = False
                                                              End Sub, Me)
                                                 Catch ex As MyThreadAbortException
                                                     filesInListFiles.Clear()
                                                     filesInListFiles = oldFilesInListFiles

                                                     MyInvoke(Sub()
                                                                  UpdateFilesListCountHeader()

                                                                  If listFiles.Rows.Count <> 0 Then
                                                                      btnComputeHash.Enabled = True
                                                                      btnIndividualFilesCopyToClipboard.Enabled = False
                                                                      btnIndividualFilesSaveResultsToDisk.Enabled = False
                                                                  End If
                                                              End Sub, Me)
                                                 Finally
                                                     If Not boolClosingWindow Then MyInvoke(Sub()
                                                                                                lblIndividualFilesStatus.Text = Nothing
                                                                                                lblProcessingFile.Text = Nothing
                                                                                                btnAddFilesInFolder.Text = "&Add File(s) in Folder ..."
                                                                                                IndividualFilesProgressBar.Value = 0
                                                                                                IndividualFilesProgressBar.Visible = False
                                                                                                ProgressForm.SetTaskbarProgressBarValue(0)
                                                                                                btnAddIndividualFiles.Enabled = True
                                                                                                btnRemoveSelectedFiles.Enabled = True
                                                                                                btnRemoveAllFiles.Enabled = True
                                                                                                listFiles.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCellsExceptHeaders
                                                                                                verifyHashesListFiles.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCellsExceptHeaders

                                                                                                If listFiles.Rows.Count <> 0 Then
                                                                                                    radioSHA256.Enabled = True
                                                                                                    radioSHA384.Enabled = True
                                                                                                    radioSHA512.Enabled = True
                                                                                                    radioSHA1.Enabled = True
                                                                                                    radioMD5.Enabled = True
                                                                                                End If
                                                                                            End Sub, Me)

                                                     boolBackgroundThreadWorking = False
                                                     boolAbortThread = False
                                                 End Try
                                             End Sub) With {
            .Priority = GetThreadPriority(),
            .Name = "Directory Scanning Thread",
            .IsBackground = True
        }
        workingThread.Start()
    End Sub

    Private Sub BtnAddFilesInFolder_Click(sender As Object, e As EventArgs) Handles btnAddFilesInFolder.Click
        If btnAddFilesInFolder.Text = "Abort Adding Files" AndAlso workingThread IsNot Nothing AndAlso MsgBox("Are you sure you want to abort adding files?", MsgBoxStyle.Question + MsgBoxStyle.YesNo, strMessageBoxTitleText) = MsgBoxResult.Yes Then
            boolAbortThread = True
            boolBackgroundThreadWorking = False
            Exit Sub
        End If

        Using OpenFileDialog As New OpenFileDialog
            OpenFileDialog.Title = "Browse for folder location..."
            OpenFileDialog.Filter = "Show All Files|*.*"
            OpenFileDialog.ValidateNames = False
            OpenFileDialog.CheckFileExists = False
            OpenFileDialog.FileName = "Select Folder"

            If OpenFileDialog.ShowDialog = DialogResult.OK Then AddFilesFromDirectory(IO.Path.GetDirectoryName(OpenFileDialog.FileName))
        End Using
    End Sub

    Private Shared Function CreateMyDataGridRowForHashFileEntry(strFileName As String, strChecksum As String, ByRef longFilesThatWereNotFound As Long, ByRef boolFileExists As Boolean, ByRef dataGrid As DataGridView) As MyDataGridViewRow
        Dim MyDataGridRow As New MyDataGridViewRow() With {.Hash = strChecksum, .FileName = strFileName}
        MyDataGridRow.CreateCells(dataGrid)

        With MyDataGridRow
            If IO.File.Exists(strFileName) Then
                .FileSize = New IO.FileInfo(strFileName).Length
                SyncLock threadLockingObject
                    longAllBytes += .FileSize
                End SyncLock
                .Cells(0).Value = strFileName
                .Cells(1).Value = FileSizeToHumanSize(MyDataGridRow.FileSize)
                .Cells(2).Value = ""
                .Cells(3).Value = strWaitingToBeProcessed
                .BoolFileExists = True
                boolFileExists = True
            Else
                .ColorType = ColorType.NotFound
                .FileSize = -1
                .ComputeTime = Nothing
                .Cells(0).Value = strFileName
                .Cells(1).Value = ""
                .Cells(2).Value = "Doesn't Exist"
                .Cells(3).Value = ""
                .DefaultCellStyle = New DataGridViewCellStyle() With {.BackColor = My.Settings.fileNotFoundColor, .WrapMode = DataGridViewTriState.True}
                .BoolFileExists = False
                .MyColor = Color.LightGray
                longFilesThatWereNotFound += 1
                boolFileExists = True
            End If

            .DefaultCellStyle.Padding = New Padding(0, 2, 0, 2)
        End With

        Return MyDataGridRow
    End Function

    Private Function IsRegexMatch(regex As Text.RegularExpressions.Regex, strHashLine As String, ByRef match As Text.RegularExpressions.Match) As Boolean
        match = regex.Match(strHashLine)
        Return match.Success
    End Function

    Private Function IsRegexMatch(regex As Text.RegularExpressions.Regex, strHashLine As String) As Boolean
        Return regex.Match(strHashLine).Success
    End Function

    Private Sub ProcessExistingHashFile(strPathToChecksumFile As String)
        strLastHashFileLoaded = strPathToChecksumFile
        lblVerifyFileNameLabel.Text = $"File Name: {strPathToChecksumFile}"
        verifyHashesListFiles.Size = New Size(verifyHashesListFiles.Size.Width, verifyHashesListFiles.Size.Height - 72)

        Dim checksumType As HashAlgorithmName
        Dim checksumFileInfo As New IO.FileInfo(strPathToChecksumFile)
        Dim strChecksumFileExtension, strDirectoryThatContainsTheChecksumFile As String
        Dim strHashFileNameWithoutExtension As String = IO.Path.GetFileNameWithoutExtension(strPathToChecksumFile)
        Dim boolHasherFileType As Boolean = False

        strLastDirectoryWorkedOn = checksumFileInfo.DirectoryName
        strChecksumFileExtension = checksumFileInfo.Extension
        strDirectoryThatContainsTheChecksumFile = checksumFileInfo.DirectoryName
        checksumFileInfo = Nothing
        intCurrentlyActiveTab = TabNumberVerifySavedHashesTab

        Select Case strChecksumFileExtension.ToLower()
            Case ".md5"
                checksumType = HashAlgorithmName.MD5
            Case ".sha1"
                checksumType = HashAlgorithmName.SHA1
            Case ".sha2", ".sha256"
                checksumType = HashAlgorithmName.SHA256
            Case ".sha384"
                checksumType = HashAlgorithmName.SHA384
            Case ".sha512"
                checksumType = HashAlgorithmName.SHA512
            Case ".hasher"
                boolHasherFileType = True
                checksumType = HashAlgorithmName.SHA512
            Case Else
                MsgBox("Invalid Hash File Type.", MsgBoxStyle.Critical, strMessageBoxTitleText)
                Exit Sub
        End Select

        checksumTypeForChecksumCompareWindow = checksumType

        workingThread = New Threading.Thread(Sub()
                                                 Dim itemOnGUI As MyDataGridViewRow = Nothing

                                                 Try
                                                     boolBackgroundThreadWorking = True
                                                     Dim strChecksum, strFileName As String
                                                     Dim index As Integer = 1
                                                     Dim longFilesThatPassedVerification As Long = 0
                                                     Dim longFilesThatDidNotPassVerification As Long = 0
                                                     Dim longFilesThatWereNotFound As Long = 0
                                                     Dim longTotalFiles As Long
                                                     Dim dataInFileArray As String() = Nothing
                                                     Dim intLineCounter As Integer = 0
                                                     Dim myStopWatch As Stopwatch = Stopwatch.StartNew
                                                     Dim strReadingHashFileMessage As String = "Reading hash file and creating DataGridView item objects... Please wait."
                                                     Dim intFileCount As Integer = 0
                                                     Dim strLineInFile As String
                                                     Dim listOfDataGridRows As New List(Of MyDataGridViewRow)
                                                     Dim intIndexBeingWorkedOn As Integer
                                                     Dim currentItem As MyDataGridViewRow = Nothing
                                                     Dim exType As Type = Nothing
                                                     Dim collectionOfHashes As New List(Of MyHash)

                                                     If Not boolHasherFileType Then
                                                         dataInFileArray = IO.File.ReadAllLines(strPathToChecksumFile)
                                                     Else
                                                         Try
                                                             Using fileStream As New IO.StreamReader(strPathToChecksumFile)
                                                                 collectionOfHashes = Newtonsoft.Json.JsonConvert.DeserializeObject(Of List(Of MyHash))(fileStream.ReadToEnd.Trim, New Newtonsoft.Json.JsonSerializerSettings With {.MissingMemberHandling = Newtonsoft.Json.MissingMemberHandling.Error})
                                                             End Using
                                                         Catch ex As Exception
                                                             Throw New MyThreadAbortException
                                                         End Try
                                                     End If

                                                     MyInvoke(Sub()
                                                                  btnRetestFailedFiles.Visible = False
                                                                  lblVerifyHashStatus.Visible = True
                                                                  lblVerifyHashStatus.Text = strReadingHashFileMessage
                                                                  verifyIndividualFilesAllFilesProgressBar.Visible = True
                                                                  VerifyHashProgressBar.Visible = True
                                                                  lblProcessingFileVerify.Visible = True
                                                                  lblVerifyHashStatus.Visible = True
                                                                  lblVerifyHashesTotalStatus.Visible = True
                                                                  lblVerifyHashesTotalStatus.Text = Nothing
                                                                  lblVerifyHashStatusProcessingFile.Text = Nothing
                                                              End Sub, Me)

                                                     SyncLock threadLockingObject
                                                         longAllReadBytes = 0
                                                         longAllBytes = 0
                                                     End SyncLock

                                                     If boolHasherFileType Then
                                                         checksumType = HashAlgorithmName.SHA512

                                                         Parallel.ForEach(collectionOfHashes, Sub(MyHash As MyHash, state As ParallelLoopState)
                                                                                                  SyncLock listOfDataGridRows
                                                                                                      If boolAbortThread Then state.Break()

                                                                                                      Dim strChecksum2, strFileName2 As String
                                                                                                      Dim boolFileExists As Boolean

                                                                                                      If boolAbortThread Then Throw New MyThreadAbortException
                                                                                                      Threading.Interlocked.Increment(intLineCounter)

                                                                                                      MyInvoke(Sub()
                                                                                                                   VerifyHashProgressBar.Value = intLineCounter / collectionOfHashes.LongCount * 100
                                                                                                                   ProgressForm.SetTaskbarProgressBarValue(VerifyHashProgressBar.Value)
                                                                                                                   lblVerifyHashStatus.Text = $"{strReadingHashFileMessage} Processing item {MyToString(intLineCounter)} of {MyToString(collectionOfHashes.LongCount)} ({MyRoundingFunction(VerifyHashProgressBar.Value, byteRoundPercentages)}%)."
                                                                                                               End Sub, lblVerifyHashStatus)

                                                                                                      strChecksum2 = MyHash.FileHash
                                                                                                      strFileName2 = MyHash.FileName

                                                                                                      If Not IO.Path.IsPathRooted(strFileName2) Then
                                                                                                          strFileName2 = IO.Path.Combine(strDirectoryThatContainsTheChecksumFile, strFileName2)
                                                                                                      End If

                                                                                                      listOfDataGridRows.Add(CreateMyDataGridRowForHashFileEntry(strFileName2, strChecksum2, longFilesThatWereNotFound, boolFileExists, verifyHashesListFiles))
                                                                                                      If boolFileExists Then intFileCount += 1
                                                                                                  End SyncLock
                                                                                              End Sub)
                                                     Else
                                                         Dim newDataInFileArray As List(Of String) = dataInFileArray.ToList.Where(Function(strLineInFile2 As String)
                                                                                                                                      Return Not strLineInFile2.Trim.StartsWith("'")
                                                                                                                                  End Function).ToList

                                                         If ChkIncludeEntryCountInFileNameHeader.Checked Then MyInvoke(Sub() lblVerifyFileNameLabel.Text &= $" ({MyToString(newDataInFileArray.Count)} {If(newDataInFileArray.Count = 1, "entry", "entries")} in hash file)", Me)

                                                         Parallel.ForEach(newDataInFileArray, Sub(strLineInFile2 As String, state As ParallelLoopState)
                                                                                                  SyncLock listOfDataGridRows
                                                                                                      If boolAbortThread Then state.Break()

                                                                                                      Dim strChecksum2, strFileName2 As String
                                                                                                      Dim boolFileExists As Boolean

                                                                                                      If boolAbortThread Then Throw New MyThreadAbortException
                                                                                                      Threading.Interlocked.Increment(intLineCounter)

                                                                                                      MyInvoke(Sub()
                                                                                                                   VerifyHashProgressBar.Value = intLineCounter / newDataInFileArray.LongCount * 100
                                                                                                                   ProgressForm.SetTaskbarProgressBarValue(VerifyHashProgressBar.Value)
                                                                                                                   lblVerifyHashStatus.Text = $"{strReadingHashFileMessage} Processing item {MyToString(intLineCounter)} of {MyToString(newDataInFileArray.LongCount)} ({MyRoundingFunction(VerifyHashProgressBar.Value, byteRoundPercentages)}%)."
                                                                                                               End Sub, lblVerifyHashStatus)

                                                                                                      If Not String.IsNullOrEmpty(strLineInFile2) Then
                                                                                                          Dim regExMatchObject As Text.RegularExpressions.Match = Nothing

                                                                                                          If IsRegexMatch(hashLineParser, strLineInFile2, regExMatchObject) OrElse IsRegexMatch(PFSenseHashLineParser, strLineInFile2, regExMatchObject) OrElse IsRegexMatch(HashFileWithNoFilename, strLineInFile2, regExMatchObject) Then
                                                                                                              If IsRegexMatch(PFSenseHashLineParser, strLineInFile2) Then
                                                                                                                  checksumType = HashAlgorithmName.SHA256
                                                                                                              End If

                                                                                                              strChecksum2 = regExMatchObject.Groups("checksum").Value.Trim
                                                                                                              strFileName2 = regExMatchObject.Groups("filename").Value.Trim

                                                                                                              If String.IsNullOrWhiteSpace(strFileName2) Then
                                                                                                                  strFileName2 = strHashFileNameWithoutExtension
                                                                                                              End If

                                                                                                              If Not IO.Path.IsPathRooted(strFileName2) Then
                                                                                                                  strFileName2 = IO.Path.Combine(strDirectoryThatContainsTheChecksumFile, strFileName2)
                                                                                                              End If

                                                                                                              listOfDataGridRows.Add(CreateMyDataGridRowForHashFileEntry(strFileName2, strChecksum2, longFilesThatWereNotFound, boolFileExists, verifyHashesListFiles))
                                                                                                              If boolFileExists Then intFileCount += 1
                                                                                                          End If
                                                                                                      End If
                                                                                                  End SyncLock
                                                                                              End Sub)

                                                         newDataInFileArray = Nothing
                                                     End If

                                                     If boolAbortThread Then Throw New MyThreadAbortException

                                                     MyInvoke(Sub()
                                                                  verifyHashesListFiles.Rows.AddRange(listOfDataGridRows.ToArray)
                                                                  Text = strWindowTitle
                                                                  If chkSortByFileSizeAfterLoadingHashFile.Checked Then SortLogsByFileSize(1, sortOrderForVerifyHashesListFiles, verifyHashesListFiles)
                                                                  VerifyHashProgressBar.Value = 0
                                                                  ProgressForm.SetTaskbarProgressBarValue(0)
                                                                  lblVerifyHashStatusProcessingFile.Visible = True
                                                              End Sub, Me)

                                                     strLineInFile = Nothing
                                                     listOfDataGridRows = Nothing

                                                     Dim strChecksumInFile As String = Nothing
                                                     Dim percentage, allBytesPercentage As Double
                                                     Dim computeStopwatch As Stopwatch
                                                     Dim allTheHashes As AllTheHashes = Nothing
                                                     Dim strDisplayValidChecksumString As String = If(chkDisplayValidChecksumString.Checked, "Valid Checksum", "")
                                                     Dim fileCountPercentage As Double
                                                     Dim exceptionObject As Exception = Nothing

                                                     Dim subRoutine As New ChecksumStatusUpdaterDelegate(Sub(size As Long, totalBytesRead As Long)
                                                                                                             Try
                                                                                                                 MyInvoke(Sub()
                                                                                                                              percentage = If(totalBytesRead = 0 Or size = 0, 0, totalBytesRead / size * 100) ' This fixes a possible divide by zero exception.
                                                                                                                              VerifyHashProgressBar.Value = percentage

                                                                                                                              SyncLock threadLockingObject
                                                                                                                                  allBytesPercentage = If(longAllReadBytes = 0 Or longAllBytes = 0, 100, longAllReadBytes / longAllBytes * 100)
                                                                                                                                  lblVerifyHashesTotalStatus.Text = $"{FileSizeToHumanSize(longAllReadBytes)} of {FileSizeToHumanSize(longAllBytes)} ({MyRoundingFunction(allBytesPercentage, byteRoundPercentages)}%) has been processed."
                                                                                                                                  If chkShowPercentageInWindowTitleBar.Checked Then Text = $"{strWindowTitle} ({MyRoundingFunction(allBytesPercentage, byteRoundPercentages)}% Completed)"
                                                                                                                              End SyncLock

                                                                                                                              lblProcessingFileVerify.Text = $"{FileSizeToHumanSize(totalBytesRead)} of {FileSizeToHumanSize(size)} ({MyRoundingFunction(percentage, byteRoundPercentages)}%) has been processed."
                                                                                                                              ProgressForm.SetTaskbarProgressBarValue(allBytesPercentage)
                                                                                                                              verifyIndividualFilesAllFilesProgressBar.Value = allBytesPercentage
                                                                                                                          End Sub, lblProcessingFileVerify)

                                                                                                                 MyInvoke(Sub()
                                                                                                                              If chkShowFileProgressInFileList.Checked Then
                                                                                                                                  currentItem.Cells(4).Value = lblProcessingFileVerify.Text
                                                                                                                                  itemOnGUI.Cells(4).Value = currentItem.Cells(4).Value
                                                                                                                              End If
                                                                                                                          End Sub, verifyHashesListFiles)
                                                                                                             Catch ex As Exception
                                                                                                             End Try
                                                                                                         End Sub)

                                                     longTotalFiles = verifyHashesListFiles.Rows.Count

                                                     MyInvoke(Sub()
                                                                  verifyHashesListFiles.BeginEdit(True)
                                                                  verifyHashesListFiles.SuspendLayout()
                                                              End Sub, verifyHashesListFiles)

                                                     For Each item As MyDataGridViewRow In verifyHashesListFiles.Rows
                                                         If boolAbortThread Then Throw New MyThreadAbortException
                                                         currentItem = item
                                                         intIndexBeingWorkedOn = item.Index
                                                         fileCountPercentage = index / intFileCount * 100

                                                         MyInvoke(Sub()
                                                                      lblVerifyHashStatusProcessingFile.Text = GenerateProcessingFileString(index, intFileCount)
                                                                      itemOnGUI = Nothing
                                                                      itemOnGUI = verifyHashesListFiles.Rows(item.Index)
                                                                  End Sub, verifyHashesListFiles)

                                                         If item.BoolFileExists Then
                                                             strChecksum = item.Hash
                                                             strFileName = item.FileName

                                                             MyInvoke(Sub() lblVerifyHashStatus.Text = $"Now processing file ""{New IO.FileInfo(strFileName).Name}"".", lblVerifyHashStatus)

                                                             MyInvoke(Sub()
                                                                          item.Cells(3).Value = strCurrentlyBeingProcessed
                                                                          UpdateDataGridViewRow(itemOnGUI, item, False)
                                                                      End Sub, verifyHashesListFiles)

                                                             computeStopwatch = Stopwatch.StartNew

                                                             If DoChecksumWithAttachedSubRoutine(strFileName, allTheHashes, subRoutine, exceptionObject) Then
                                                                 strChecksum = GetDataFromAllTheHashes(checksumType, allTheHashes)
                                                                 item.AllTheHashes = allTheHashes

                                                                 If strChecksum.Equals(item.Hash, StringComparison.OrdinalIgnoreCase) Then
                                                                     MyInvoke(Sub()
                                                                                  item.ColorType = ColorType.Valid
                                                                                  item.MyColor = validColor
                                                                                  item.Cells(2).Value = "Valid Checksum"
                                                                                  item.ComputeTime = computeStopwatch.Elapsed
                                                                                  item.Cells(3).Value = TimespanToHMS(item.ComputeTime)
                                                                                  item.Cells(4).Value = strDisplayValidChecksumString
                                                                                  longFilesThatPassedVerification += 1
                                                                                  item.BoolValidHash = True
                                                                              End Sub, verifyHashesListFiles)
                                                                 Else
                                                                     MyInvoke(Sub()
                                                                                  item.ColorType = ColorType.NotValid
                                                                                  item.MyColor = notValidColor
                                                                                  item.Cells(2).Value = "Incorrect Checksum"
                                                                                  item.ComputeTime = computeStopwatch.Elapsed
                                                                                  item.Cells(3).Value = TimespanToHMS(item.ComputeTime)
                                                                                  item.Cells(4).Value = If(chkDisplayHashesInUpperCase.Checked, GetDataFromAllTheHashes(checksumType, allTheHashes).ToUpper, GetDataFromAllTheHashes(checksumType, allTheHashes).ToLower)
                                                                                  longFilesThatDidNotPassVerification += 1
                                                                                  item.BoolValidHash = False
                                                                              End Sub, verifyHashesListFiles)
                                                                 End If

                                                                 item.BoolExceptionOccurred = False
                                                                 item.StrCrashData = Nothing
                                                             Else
                                                                 MyInvoke(Sub()
                                                                              item.ColorType = ColorType.NotFound
                                                                              item.MyColor = fileNotFoundColor
                                                                              item.Cells(2).Value = If(exType IsNot Nothing, $"(An error occurred while calculating checksum, {exType})", "(An error occurred while calculating checksum, unknown exception type)")
                                                                              item.BoolExceptionOccurred = True
                                                                              item.StrCrashData = $"{exceptionObject.Message}{vbCrLf}{exceptionObject.StackTrace}"
                                                                              longFilesThatWereNotFound += 1
                                                                          End Sub, verifyHashesListFiles)
                                                             End If

                                                             MyInvoke(Sub()
                                                                          If ChkAutoScroll.Checked Then verifyHashesListFiles.FirstDisplayedScrollingRowIndex = item.Index
                                                                          UpdateDataGridViewRow(itemOnGUI, item, False)
                                                                      End Sub, verifyHashesListFiles)

                                                             index += 1
                                                         Else
                                                             item.BoolValidHash = False
                                                         End If
                                                     Next

                                                     For Each item As MyDataGridViewRow In verifyHashesListFiles.Rows
                                                         MyInvoke(Sub()
                                                                      itemOnGUI = Nothing
                                                                      itemOnGUI = verifyHashesListFiles.Rows(item.Index)
                                                                      UpdateGridViewRowColor(itemOnGUI, item)
                                                                  End Sub, verifyHashesListFiles)
                                                     Next

                                                     MyInvoke(Sub()
                                                                  verifyHashesListFiles.EndEdit()
                                                                  verifyHashesListFiles.ResumeLayout()
                                                              End Sub, verifyHashesListFiles)

                                                     GC.Collect()

                                                     subRoutine = Nothing

                                                     MyInvoke(Sub()
                                                                  lblVerifyHashStatusProcessingFile.Visible = False
                                                                  lblVerifyHashesTotalStatus.Visible = False
                                                                  verifyIndividualFilesAllFilesProgressBar.Visible = False
                                                                  lblVerifyHashStatus.Visible = False
                                                                  lblProcessingFileVerify.Visible = False
                                                                  VerifyHashProgressBar.Value = 0
                                                                  VerifyHashProgressBar.Visible = False
                                                                  ProgressForm.SetTaskbarProgressBarValue(0)
                                                                  Text = strWindowTitle
                                                                  verifyHashesListFiles.Size = New Size(verifyHashesListFiles.Size.Width, verifyHashesListFiles.Size.Height + 72)

                                                                  Dim sbMessageBoxText As New Text.StringBuilder

                                                                  If longFilesThatWereNotFound = 0 And longFilesThatDidNotPassVerification = 0 Then
                                                                      If longFilesThatPassedVerification = longTotalFiles Then
                                                                          If longTotalFiles = 1 Then
                                                                              sbMessageBoxText.AppendLine("Processing of hash file complete. The one file in the hash file has passed verification.")
                                                                          Else
                                                                              sbMessageBoxText.AppendLine($"Processing of hash file complete. All {MyToString(longTotalFiles)} files have passed verification.")
                                                                          End If
                                                                      Else
                                                                          If longFilesThatDidNotPassVerification <> 0 Then btnRetestFailedFiles.Visible = True
                                                                          sbMessageBoxText.AppendLine($"Processing of hash file complete. {MyToString(longFilesThatPassedVerification)} out of {MyToString(longTotalFiles)} {If(longTotalFiles = 1, "file", "files")} passed verification, {MyToString(longFilesThatDidNotPassVerification)} {If(longFilesThatDidNotPassVerification = 1, "file", "files")} didn't pass verification.")
                                                                      End If
                                                                  Else
                                                                      sbMessageBoxText.AppendLine("Processing of hash file complete.")
                                                                      sbMessageBoxText.AppendLine()
                                                                      btnRetestFailedFiles.Visible = True

                                                                      If longFilesThatPassedVerification = longTotalFiles Then
                                                                          sbMessageBoxText.AppendLine($"All files have passed verification. Unfortunately, {MyToString(longFilesThatWereNotFound)} {If(longFilesThatWereNotFound = 1, "file", "files")} were not found.")
                                                                      Else
                                                                          If longFilesThatDidNotPassVerification <> 0 Then btnRetestFailedFiles.Visible = True

                                                                          If longFilesThatPassedVerification = 0 Then
                                                                              sbMessageBoxText.Append($"None of the files out of {MyToString(longTotalFiles)} {If(longTotalFiles = 1, "file", "files")} passed verification. ")
                                                                          Else
                                                                              longTotalFiles -= longFilesThatWereNotFound
                                                                              sbMessageBoxText.Append($"Not all of the files passed verification, only {MyToString(longFilesThatPassedVerification)} out of {MyToString(longTotalFiles)} {If(longTotalFiles = 1, "file", "files")} passed verification.")
                                                                          End If
                                                                          sbMessageBoxText.AppendLine($" Unfortunately, {MyToString(longFilesThatWereNotFound)} {If(longFilesThatWereNotFound = 1, "file was", "files were")} not found.")
                                                                      End If
                                                                  End If

                                                                  sbMessageBoxText.AppendLine()
                                                                  sbMessageBoxText.AppendLine($"Processing completed in {TimespanToHMS(myStopWatch.Elapsed)}.")

                                                                  MsgBox(sbMessageBoxText.ToString.Trim, MsgBoxStyle.Information, strMessageBoxTitleText)
                                                              End Sub, Me)

                                                     boolBackgroundThreadWorking = False
                                                     workingThread = Nothing
                                                 Catch ex As MyThreadAbortException
                                                     MyInvoke(Sub()
                                                                  If Not boolClosingWindow Then
                                                                      lblVerifyHashStatusProcessingFile.Visible = False
                                                                      verifyIndividualFilesAllFilesProgressBar.Visible = False
                                                                      lblVerifyHashStatus.Visible = False
                                                                      lblVerifyHashesTotalStatus.Visible = False
                                                                      lblProcessingFileVerify.Visible = False
                                                                      VerifyHashProgressBar.Value = 0
                                                                      VerifyHashProgressBar.Visible = False
                                                                      ProgressForm.SetTaskbarProgressBarValue(0)
                                                                      verifyHashesListFiles.Rows.Clear()
                                                                      Text = strWindowTitle
                                                                      verifyHashesListFiles.Size = New Size(verifyHashesListFiles.Size.Width, verifyHashesListFiles.Size.Height + 72)
                                                                      lblVerifyFileNameLabel.Text = "File Name: (None Selected for Processing)"
                                                                  End If

                                                                  boolBackgroundThreadWorking = False
                                                                  workingThread = Nothing

                                                                  If ex.GetType = GetType(Threading.ThreadAbortException) And Not boolClosingWindow Then
                                                                      MsgBox("Processing aborted.", MsgBoxStyle.Information, strMessageBoxTitleText)
                                                                  ElseIf ex.GetType = GetType(UnauthorizedAccessException) Then
                                                                      MsgBox($"Unable to access data, an Unauthorized Access Exception has occurred.{DoubleCRLF}Check to see if you have access to the data in question.{vbCrLf}{vbCrLf}{ex.Message}", MsgBoxStyle.Critical, strMessageBoxTitleText)
                                                                  End If
                                                              End Sub, Me)
                                                 Finally
                                                     boolAbortThread = False
                                                     itemOnGUI = Nothing
                                                     intCurrentlyActiveTab = TabNumberNull
                                                     SyncLock threadLockingObject
                                                         longAllReadBytes = 0
                                                         longAllBytes = 0
                                                     End SyncLock

                                                     MyInvoke(Sub()
                                                                  If Not boolClosingWindow Then
                                                                      btnTransferToHashIndividualFilesTab.Enabled = verifyHashesListFiles.Rows.Count <> 0
                                                                      btnOpenExistingHashFile.Text = "Open Hash File"
                                                                      ProgressForm.SetTaskbarProgressBarValue(0)
                                                                      verifyIndividualFilesAllFilesProgressBar.Value = 0
                                                                  End If
                                                              End Sub, Me)
                                                 End Try
                                             End Sub) With {
            .Priority = GetThreadPriority(),
            .Name = "Verify Hash File Working Thread",
            .IsBackground = True
        }
        workingThread.Start()
    End Sub

    Private Sub BtnOpenExistingHashFile_Click(sender As Object, e As EventArgs) Handles btnOpenExistingHashFile.Click
        If btnOpenExistingHashFile.Text = "Abort Processing" Then
            Dim result As MsgBoxResult = MsgBox("Are you sure you want to abort processing?", MsgBoxStyle.Question Or MsgBoxStyle.YesNo, strMessageBoxTitleText)

            If result = MsgBoxResult.Yes AndAlso workingThread IsNot Nothing Then
                boolAbortThread = True
                boolBackgroundThreadWorking = False
            End If

            Exit Sub
        End If

        btnTransferToHashIndividualFilesTab.Enabled = False
        btnOpenExistingHashFile.Text = "Abort Processing"
        verifyHashesListFiles.Rows.Clear()

        Using OpenFileDialog As New OpenFileDialog
            lblVerifyFileNameLabel.Text = "File Name: (None Selected for Processing)"

            OpenFileDialog.Title = "Select a hash file to verify..."
            OpenFileDialog.Filter = "Checksum File|*.md5;*.sha1;*.sha256;*.sha384;*.sha512;*.ripemd160;*.hasher|Hasher File|*.hasher"
            OpenFileDialog.Multiselect = True

            If OpenFileDialog.ShowDialog() = DialogResult.OK Then
                ProcessExistingHashFile(OpenFileDialog.FileName)
            Else
                btnOpenExistingHashFile.Text = "Open Hash File"
            End If
        End Using
    End Sub

    Private Sub ListFiles_DragDrop(sender As Object, e As DragEventArgs) Handles listFiles.DragDrop
        If boolBackgroundThreadWorking Then Exit Sub
        For Each strItem As String In e.Data.GetData(DataFormats.FileDrop)
            If IO.File.Exists(strItem) Or IO.Directory.Exists(strItem) Then
                If Not IO.File.GetAttributes(strItem).HasFlag(IO.FileAttributes.Directory) AndAlso Not filesInListFiles.Contains(strItem.Trim.ToLower) Then
                    If IO.File.Exists(strItem) Then listFiles.Rows.Add(CreateFilesDataGridObject(strItem, listFiles))
                Else
                    AddFilesFromDirectory(strItem)
                End If
            End If
        Next

        UpdateFilesListCountHeader()
        If chkSortFileListingAfterAddingFilesToHash.Checked Then SortLogsByFileSize(1, sortOrderForListFiles, listFiles)
    End Sub

    Private Sub ListFiles_DragEnter(sender As Object, e As DragEventArgs) Handles listFiles.DragEnter
        e.Effect = If(e.Data.GetDataPresent(DataFormats.FileDrop), DragDropEffects.All, DragDropEffects.None)
    End Sub

    Private Sub ChkRecurrsiveDirectorySearch_Click(sender As Object, e As EventArgs) Handles chkRecurrsiveDirectorySearch.Click
        My.Settings.boolRecurrsiveDirectorySearch = chkRecurrsiveDirectorySearch.Checked
    End Sub

    Private Sub TxtTextToHash_TextChanged(sender As Object, e As EventArgs) Handles txtTextToHash.TextChanged
        lblHashTextStep1.Text = $"Step 1: Input some text: {MyToString(txtTextToHash.Text.Length)} {If(txtTextToHash.Text.Length = 1, "Character", "Characters")}"
        Dim boolEnableButtons As Boolean = txtTextToHash.Text.Length <> 0
        btnComputeTextHash.Enabled = boolEnableButtons
        btnCheckHaveIBeenPwned.Enabled = boolEnableButtons
        btnCopyTextHashResultsToClipboard.Enabled = False
        txtHashResults.Rows.Clear()
        hashTextAllTheHashes = Nothing
    End Sub

    Private Function MakeTextHashListViewItem(strHashType As String, strHash As String) As DataGridViewRow
        Dim itemToBeAdded As New DataGridViewRow()
        With itemToBeAdded
            .CreateCells(txtHashResults)
            .Cells(0).Value = strHashType
            .Cells(1).Value = If(chkDisplayHashesInUpperCase.Checked, strHash.ToUpper, strHash.ToLower)
        End With
        Return itemToBeAdded
    End Function

    Private Sub BtnComputeTextHash_Click(sender As Object, e As EventArgs) Handles btnComputeTextHash.Click
        hashTextAllTheHashes = GetHashOfString(txtTextToHash.Text)

        txtHashResults.Rows.Clear()

        txtHashResults.Rows.Add(MakeTextHashListViewItem("MD5", GetDataFromAllTheHashes(HashAlgorithmName.MD5, hashTextAllTheHashes)))
        txtHashResults.Rows.Add(MakeTextHashListViewItem("SHA1/SHA160", GetDataFromAllTheHashes(HashAlgorithmName.SHA1, hashTextAllTheHashes)))
        txtHashResults.Rows.Add(MakeTextHashListViewItem("SHA256", GetDataFromAllTheHashes(HashAlgorithmName.SHA256, hashTextAllTheHashes)))
        txtHashResults.Rows.Add(MakeTextHashListViewItem("SHA384", GetDataFromAllTheHashes(HashAlgorithmName.SHA384, hashTextAllTheHashes)))
        txtHashResults.Rows.Add(MakeTextHashListViewItem("SHA512", GetDataFromAllTheHashes(HashAlgorithmName.SHA512, hashTextAllTheHashes)))

        btnCopyTextHashResultsToClipboard.Enabled = True
        btnComputeTextHash.Enabled = False
    End Sub

    Private Sub CopyHashToWindowsClipboardToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CopyHashToWindowsClipboardToolStripMenuItem.Click
        Dim listViewItem As DataGridViewRow = txtHashResults.SelectedRows(0)
        If CopyTextToWindowsClipboard(listViewItem.Cells(1).Value) Then MsgBox("Your hash results have been copied to the Windows Clipboard.", MsgBoxStyle.Information, strMessageBoxTitleText)
    End Sub

    Private Sub BtnPasteTextFromWindowsClipboard_Click(sender As Object, e As EventArgs) Handles btnPasteTextFromWindowsClipboard.Click
        txtTextToHash.Text = Clipboard.GetText()
    End Sub

    Private Sub BtnCopyTextHashResultsToClipboard_Click(sender As Object, e As EventArgs) Handles btnCopyTextHashResultsToClipboard.Click
        Dim strHash As New Text.StringBuilder
        strHash.AppendLine($"MD5: {If(chkDisplayHashesInUpperCase.Checked, GetDataFromAllTheHashes(HashAlgorithmName.MD5, hashTextAllTheHashes).ToUpper, GetDataFromAllTheHashes(HashAlgorithmName.MD5, hashTextAllTheHashes).ToLower)}")
        strHash.AppendLine($"SHA1/SHA160: {If(chkDisplayHashesInUpperCase.Checked, GetDataFromAllTheHashes(HashAlgorithmName.SHA1, hashTextAllTheHashes).ToUpper, GetDataFromAllTheHashes(HashAlgorithmName.SHA1, hashTextAllTheHashes).ToLower)}")
        strHash.AppendLine($"SHA256: {If(chkDisplayHashesInUpperCase.Checked, GetDataFromAllTheHashes(HashAlgorithmName.SHA256, hashTextAllTheHashes).ToUpper, GetDataFromAllTheHashes(HashAlgorithmName.SHA256, hashTextAllTheHashes).ToLower)}")
        strHash.AppendLine($"SHA384: {If(chkDisplayHashesInUpperCase.Checked, GetDataFromAllTheHashes(HashAlgorithmName.SHA384, hashTextAllTheHashes).ToUpper, GetDataFromAllTheHashes(HashAlgorithmName.SHA384, hashTextAllTheHashes).ToLower)}")
        strHash.AppendLine($"SHA512: {If(chkDisplayHashesInUpperCase.Checked, GetDataFromAllTheHashes(HashAlgorithmName.SHA512, hashTextAllTheHashes).ToUpper, GetDataFromAllTheHashes(HashAlgorithmName.SHA512, hashTextAllTheHashes).ToLower)}")

        If CopyTextToWindowsClipboard(strHash.ToString.Trim) Then MsgBox("Your hash results have been copied to the Windows Clipboard.", MsgBoxStyle.Information, strMessageBoxTitleText)
    End Sub

    Private Sub TabControl1_Selecting(sender As Object, e As TabControlCancelEventArgs) Handles TabControl1.Selecting
        If (e.TabPageIndex = TabNumberSettingsTab Or e.TabPageIndex = TabNumberWelcomeTab) AndAlso intCurrentlyActiveTab <> TabNumberNull AndAlso Not TabControl1.TabPages(intCurrentlyActiveTab).Text.Contains("Currently Active") Then
            TabControl1.TabPages(intCurrentlyActiveTab).Text &= " (Currently Active)"
        ElseIf e.TabPageIndex = intCurrentlyActiveTab AndAlso TabControl1.TabPages(intCurrentlyActiveTab).Text.Contains("Currently Active") Then
            Dim strNewTabText As String = TabControl1.TabPages(intCurrentlyActiveTab).Text.Replace(" (Currently Active)", "")
            TabControl1.TabPages(intCurrentlyActiveTab).Text = strNewTabText
        End If

        If e.TabPageIndex = TabNumberCompareFileAgainstKnownHashTab Then
            pictureBoxVerifyAgainstResults.Image = Nothing
            ToolTip.SetToolTip(pictureBoxVerifyAgainstResults, "")
            txtFileForKnownHash.Text = Nothing
            txtKnownHash.Text = Nothing
            lblCompareFileAgainstKnownHashType.Text = Nothing
        ElseIf e.TabPageIndex = TabNumberWelcomeTab Or e.TabPageIndex = TabNumberSettingsTab Or e.TabPageIndex = intCurrentlyActiveTab Then
            Exit Sub
        End If

        If boolBackgroundThreadWorking AndAlso MsgBox("Checksum hashes are being computed, are you sure you want to abort?", MsgBoxStyle.Question + MsgBoxStyle.YesNo, strMessageBoxTitleText) = MsgBoxResult.No Then
            e.Cancel = True
            Exit Sub
        Else
            If workingThread IsNot Nothing Then
                boolAbortThread = True
                boolBackgroundThreadWorking = False
            End If
        End If
    End Sub

    Private Sub Form1_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        If boolBackgroundThreadWorking AndAlso MsgBox("Background tasks are being processed, are you sure you want to abort and exit the program?", MsgBoxStyle.Question + MsgBoxStyle.YesNo, strMessageBoxTitleText) = MsgBoxResult.No Then
            e.Cancel = True
            Exit Sub
        Else
            boolClosingWindow = True

            If PipeServer IsNot Nothing Then
                PipeServer.Disconnect()
                PipeServer.Close()
            End If

            If workingThread IsNot Nothing Then boolAbortThread = True

            My.Settings.windowLocation = Location
        End If
    End Sub

    Private Sub ListFilesContextMenu_Opening(sender As Object, e As CancelEventArgs) Handles listFilesContextMenu.Opening
        If listFiles.SelectedRows.Count = 0 Or listFiles.SelectedRows.Count > 1 Then
            e.Cancel = True
        ElseIf listFiles.SelectedRows.Count = 1 Then
            If String.IsNullOrWhiteSpace(DirectCast(listFiles.SelectedRows(0), MyDataGridViewRow).Hash) Then
                e.Cancel = True
                Exit Sub
            End If

            Dim MyDataGridRow As MyDataGridViewRow = DirectCast(listFiles.SelectedRows(0), MyDataGridViewRow)

            globalAllTheHashes = MyDataGridRow.AllTheHashes
            listFilesContextMenuFileName.Text = $"File Name: {MyDataGridRow.FileName}"

            With MyDataGridRow.AllTheHashes
                listFilesContextMenuMD5.Text = $"MD5: {If(chkDisplayHashesInUpperCase.Checked, .Md5.ToUpper, .Md5.ToLower)}"
                listFilesContextMenuSHA160.Text = $"SHA160: {If(chkDisplayHashesInUpperCase.Checked, .Sha160.ToUpper, .Sha160.ToLower)}"
                listFilesContextMenuSHA256.Text = $"SHA256: {If(chkDisplayHashesInUpperCase.Checked, .Sha256.ToUpper, .Sha256.ToLower)}"
                listFilesContextMenuSHA384.Text = $"SHA384: {If(chkDisplayHashesInUpperCase.Checked, .Sha384.ToUpper, .Sha384.ToLower)}"
                listFilesContextMenuSHA512.Text = $"SHA512: {If(chkDisplayHashesInUpperCase.Checked, .Sha512.ToUpper, .Sha512.ToLower)}"
            End With
        End If
    End Sub

    Private Sub AddHashFileHeader(ByRef stringBuilder As Text.StringBuilder, intFileCount As Integer)
        stringBuilder.AppendLine("'")
        stringBuilder.AppendLine($"' Hash file generated by Hasher, version {checkForUpdates.versionString}. Written by Tom Parkison.")
        stringBuilder.AppendLine("' Web Site: https://www.toms-world.org/blog/hasher")
        stringBuilder.AppendLine("' Source Code Available At: https://github.com/trparky/Hasher")
        stringBuilder.AppendLine("'")
        stringBuilder.AppendLine($"' Number of files in hash data: {MyToString(intFileCount)}")
        stringBuilder.AppendLine("'")
    End Sub

    Private Sub BtnCheckForUpdates_Click(sender As Object, e As EventArgs) Handles btnCheckForUpdates.Click
        Threading.ThreadPool.QueueUserWorkItem(Sub()
                                                   Dim checkForUpdatesClassObject As New checkForUpdates.CheckForUpdatesClass(Me)
                                                   checkForUpdatesClassObject.CheckForUpdates()
                                               End Sub)
    End Sub

    Private Sub BtnCompareFiles_Click(sender As Object, e As EventArgs) Handles btnCompareFiles.Click
        compareRadioSHA512.Checked = True
        If btnCompareFiles.Text = "Abort Processing" Then
            Dim result As MsgBoxResult = MsgBox("Are you sure you want to abort processing?", MsgBoxStyle.Question Or MsgBoxStyle.YesNo, strMessageBoxTitleText)

            If result = MsgBoxResult.Yes AndAlso workingThread IsNot Nothing Then
                boolAbortThread = True
                boolBackgroundThreadWorking = False
            End If

            Exit Sub
        End If

        If txtFile1.Text.Equals(txtFile2.Text, StringComparison.OrdinalIgnoreCase) Then
            MsgBox("Please select two different files.", MsgBoxStyle.Critical, strMessageBoxTitleText)
            Exit Sub
        End If
        If Not IO.File.Exists(txtFile1.Text) Then
            MsgBox("File #1 doesn't exist.", MsgBoxStyle.Critical, strMessageBoxTitleText)
            Exit Sub
        End If
        If Not IO.File.Exists(txtFile2.Text) Then
            MsgBox("File #2 doesn't exist.", MsgBoxStyle.Critical, strMessageBoxTitleText)
            Exit Sub
        End If

        Dim File1FileInfo As New IO.FileInfo(txtFile1.Text)
        Dim File2FileInfo As New IO.FileInfo(txtFile2.Text)

        If File1FileInfo.Length <> File2FileInfo.Length And Not ChkComputeHashesOnCompareFilesTabEvenWithDifferentFileSizes.Checked Then
            Dim stringBuilder As New Text.StringBuilder
            stringBuilder.AppendLine("Both files are different file sizes, so we're going to assume that they're different. OK?")
            stringBuilder.AppendLine()
            stringBuilder.AppendLine($"File #1 Size: {FileSizeToHumanSize(File1FileInfo.Length)}")
            stringBuilder.AppendLine($"File #2 Size: {FileSizeToHumanSize(File2FileInfo.Length)}")

            MsgBox(stringBuilder.ToString.Trim, MsgBoxStyle.Information, strMessageBoxTitleText)
            Exit Sub
        End If

        SyncLock threadLockingObject
            longAllBytes = 0
            longAllReadBytes = 0

            longAllBytes += File1FileInfo.Length
            longAllBytes += File2FileInfo.Length
        End SyncLock

        btnCompareFilesBrowseFile1.Enabled = False
        btnCompareFilesBrowseFile2.Enabled = False
        txtFile1.Enabled = False
        txtFile2.Enabled = False
        btnCompareFiles.Text = "Abort Processing"
        compareFilesProgressBar.Visible = True
        intCurrentlyActiveTab = TabNumberCompareFilesTab

        workingThread = New Threading.Thread(Sub()
                                                 Try
                                                     boolBackgroundThreadWorking = True
                                                     If boolAbortThread Then Throw New MyThreadAbortException()
                                                     Dim checksumType As HashAlgorithmName

                                                     MyInvoke(Sub()
                                                                  compareRadioMD5.Enabled = False
                                                                  compareRadioSHA1.Enabled = False
                                                                  compareRadioSHA256.Enabled = False
                                                                  compareRadioSHA384.Enabled = False
                                                                  compareRadioSHA512.Enabled = False
                                                                  CompareFilesAllFilesProgress.Visible = True

                                                                  If compareRadioMD5.Checked Then
                                                                      checksumType = HashAlgorithmName.MD5
                                                                  ElseIf compareRadioSHA1.Checked Then
                                                                      checksumType = HashAlgorithmName.SHA1
                                                                  ElseIf compareRadioSHA256.Checked Then
                                                                      checksumType = HashAlgorithmName.SHA256
                                                                  ElseIf compareRadioSHA384.Checked Then
                                                                      checksumType = HashAlgorithmName.SHA384
                                                                  ElseIf compareRadioSHA512.Checked Then
                                                                      checksumType = HashAlgorithmName.SHA512
                                                                  End If
                                                              End Sub, Me)

                                                     Dim strChecksum1 As String = Nothing
                                                     Dim strChecksum2 As String = Nothing
                                                     Dim boolSuccessful As Boolean = False
                                                     Dim percentage, allBytesPercentage As Double
                                                     Dim exceptionObject1 As Exception = Nothing
                                                     Dim exceptionObject2 As Exception = Nothing

                                                     Dim subRoutine As New ChecksumStatusUpdaterDelegate(Sub(size As Long, totalBytesRead As Long)
                                                                                                             Try
                                                                                                                 MyInvoke(Sub()
                                                                                                                              percentage = If(totalBytesRead = 0 Or size = 0, 0, totalBytesRead / size * 100) ' This fixes a possible divide by zero exception.
                                                                                                                              compareFilesProgressBar.Value = percentage

                                                                                                                              SyncLock threadLockingObject
                                                                                                                                  allBytesPercentage = If(longAllReadBytes = 0 Or longAllBytes = 0, 100, longAllReadBytes / longAllBytes * 100)
                                                                                                                              End SyncLock

                                                                                                                              ProgressForm.SetTaskbarProgressBarValue(allBytesPercentage)
                                                                                                                              CompareFilesAllFilesProgress.Value = allBytesPercentage
                                                                                                                              lblCompareFilesStatus.Text = $"{FileSizeToHumanSize(totalBytesRead)} of {FileSizeToHumanSize(size)} ({MyRoundingFunction(percentage, byteRoundPercentages)}%) has been processed."
                                                                                                                              lblCompareFilesAllFilesStatus.Text = $"{FileSizeToHumanSize(longAllReadBytes)} of {FileSizeToHumanSize(longAllBytes)} ({MyRoundingFunction(allBytesPercentage, byteRoundPercentages)}%) has been processed."

                                                                                                                              If chkShowPercentageInWindowTitleBar.Checked Then Text = $"{strWindowTitle} ({MyRoundingFunction(allBytesPercentage, byteRoundPercentages)}% Completed)"
                                                                                                                          End Sub, Me)
                                                                                                             Catch ex As Exception
                                                                                                             End Try
                                                                                                         End Sub)

                                                     Dim myStopWatch As Stopwatch = Stopwatch.StartNew

                                                     If boolAbortThread Then Throw New MyThreadAbortException()
                                                     If DoChecksumWithAttachedSubRoutine(txtFile1.Text, compareFilesAllTheHashes1, subRoutine, exceptionObject1) AndAlso DoChecksumWithAttachedSubRoutine(txtFile2.Text, compareFilesAllTheHashes2, subRoutine, exceptionObject2) Then
                                                         boolSuccessful = True

                                                         Select Case checksumType
                                                             Case HashAlgorithmName.MD5
                                                                 strChecksum1 = compareFilesAllTheHashes1.Md5
                                                                 strChecksum2 = compareFilesAllTheHashes2.Md5
                                                             Case HashAlgorithmName.SHA1
                                                                 strChecksum1 = compareFilesAllTheHashes1.Sha160
                                                                 strChecksum2 = compareFilesAllTheHashes2.Sha160
                                                             Case HashAlgorithmName.SHA256
                                                                 strChecksum1 = compareFilesAllTheHashes1.Sha256
                                                                 strChecksum2 = compareFilesAllTheHashes2.Sha256
                                                             Case HashAlgorithmName.SHA384
                                                                 strChecksum1 = compareFilesAllTheHashes1.Sha384
                                                                 strChecksum2 = compareFilesAllTheHashes2.Sha384
                                                             Case HashAlgorithmName.SHA512
                                                                 strChecksum1 = compareFilesAllTheHashes1.Sha512
                                                                 strChecksum2 = compareFilesAllTheHashes2.Sha512
                                                         End Select

                                                         MyInvoke(Sub()
                                                                      lblFile1Hash.Text = $"Hash/Checksum: {If(chkDisplayHashesInUpperCase.Checked, strChecksum1.ToUpper, strChecksum1.ToLower)}"
                                                                      ToolTip.SetToolTip(lblFile1Hash, strChecksum1)

                                                                      lblFile2Hash.Text = $"Hash/Checksum: {If(chkDisplayHashesInUpperCase.Checked, strChecksum2.ToUpper, strChecksum2.ToLower)}"
                                                                      ToolTip.SetToolTip(lblFile2Hash, strChecksum2)
                                                                  End Sub, Me)
                                                     End If


                                                     MyInvoke(Sub()
                                                                  btnCompareFilesBrowseFile1.Enabled = True
                                                                  btnCompareFilesBrowseFile2.Enabled = True
                                                                  txtFile1.Enabled = True
                                                                  txtFile2.Enabled = True
                                                                  btnCompareFiles.Text = "Compare Files"
                                                                  compareRadioMD5.Enabled = True
                                                                  compareRadioSHA1.Enabled = True
                                                                  compareRadioSHA256.Enabled = True
                                                                  compareRadioSHA384.Enabled = True
                                                                  compareRadioSHA512.Enabled = True
                                                                  lblCompareFilesStatus.Text = strNoBackgroundProcesses
                                                                  lblCompareFilesAllFilesStatus.Text = Nothing
                                                                  compareFilesProgressBar.Value = 0
                                                                  compareFilesProgressBar.Visible = False
                                                                  CompareFilesAllFilesProgress.Value = 0
                                                                  CompareFilesAllFilesProgress.Visible = False
                                                                  ProgressForm.SetTaskbarProgressBarValue(0)
                                                                  Text = strWindowTitle

                                                                  If boolSuccessful Then
                                                                      If strChecksum1.Equals(strChecksum2, StringComparison.OrdinalIgnoreCase) Then
                                                                          pictureBoxCompareFiles.Image = My.Resources.good_check
                                                                          ToolTip.SetToolTip(pictureBoxCompareFiles, "Both files are the same.")
                                                                          MsgBox($"Both files are the same.{DoubleCRLF}Processing completed in {TimespanToHMS(myStopWatch.Elapsed)}.", MsgBoxStyle.Information, strMessageBoxTitleText)
                                                                      Else
                                                                          pictureBoxCompareFiles.Image = My.Resources.bad_check
                                                                          ToolTip.SetToolTip(pictureBoxCompareFiles, "The two files don't match.")
                                                                          MsgBox($"The two files don't match.{DoubleCRLF}Processing completed in {TimespanToHMS(myStopWatch.Elapsed)}.", MsgBoxStyle.Critical, strMessageBoxTitleText)
                                                                      End If
                                                                  Else
                                                                      If boolAbortThread AndAlso Not boolClosingWindow Then
                                                                          MsgBox("Processing aborted.", MsgBoxStyle.Information, strMessageBoxTitleText)
                                                                      Else
                                                                          MsgBox("There was an error while calculating the checksum.", MsgBoxStyle.Critical, strMessageBoxTitleText)
                                                                      End If
                                                                  End If

                                                                  boolBackgroundThreadWorking = False
                                                                  workingThread = Nothing
                                                              End Sub, Me)
                                                 Catch ex As MyThreadAbortException
                                                     MyInvoke(Sub()
                                                                  If Not boolClosingWindow Then
                                                                      btnCompareFilesBrowseFile1.Enabled = True
                                                                      btnCompareFilesBrowseFile1.Enabled = True
                                                                      txtFile1.Enabled = True
                                                                      txtFile2.Enabled = True
                                                                      compareFilesProgressBar.Value = 0
                                                                      compareFilesProgressBar.Visible = False
                                                                      CompareFilesAllFilesProgress.Value = 0
                                                                      CompareFilesAllFilesProgress.Visible = False
                                                                      ProgressForm.SetTaskbarProgressBarValue(0)
                                                                      btnCompareFiles.Text = "Compare Files"
                                                                      compareRadioMD5.Enabled = True
                                                                      compareRadioSHA1.Enabled = True
                                                                      compareRadioSHA256.Enabled = True
                                                                      compareRadioSHA384.Enabled = True
                                                                      compareRadioSHA512.Enabled = True
                                                                      lblCompareFilesStatus.Text = strNoBackgroundProcesses
                                                                      lblCompareFilesAllFilesStatus.Text = Nothing
                                                                      Text = strWindowTitle
                                                                  End If

                                                                  boolBackgroundThreadWorking = False
                                                                  workingThread = Nothing
                                                                  If Not boolClosingWindow Then MsgBox("Processing aborted.", MsgBoxStyle.Information, strMessageBoxTitleText)
                                                              End Sub, Me)
                                                 Finally
                                                     boolAbortThread = False
                                                     intCurrentlyActiveTab = TabNumberNull
                                                     SyncLock threadLockingObject
                                                         longAllReadBytes = 0
                                                         longAllBytes = 0
                                                     End SyncLock
                                                 End Try
                                             End Sub) With {
            .Priority = GetThreadPriority(),
            .Name = "Hash Generation Thread",
            .IsBackground = True
        }
        workingThread.Start()
    End Sub

    Private Sub BtnCompareFilesBrowseFile1_Click(sender As Object, e As EventArgs) Handles btnCompareFilesBrowseFile1.Click
        lblFile1Hash.Text = strHashChecksumToBeComputed
        pictureBoxCompareFiles.Image = Nothing
        ToolTip.SetToolTip(pictureBoxCompareFiles, "")
        ToolTip.SetToolTip(lblFile1Hash, "")

        Using OpenFileDialog As New OpenFileDialog
            OpenFileDialog.Title = "Select file #1 to be compared..."
            OpenFileDialog.Filter = "Show All Files|*.*"

            If OpenFileDialog.ShowDialog() = DialogResult.OK Then txtFile1.Text = OpenFileDialog.FileName
        End Using
    End Sub

    Private Sub BtnCompareFilesBrowseFile2_Click(sender As Object, e As EventArgs) Handles btnCompareFilesBrowseFile2.Click
        lblFile2Hash.Text = strHashChecksumToBeComputed
        pictureBoxCompareFiles.Image = Nothing
        ToolTip.SetToolTip(pictureBoxCompareFiles, "")
        ToolTip.SetToolTip(lblFile2Hash, "")

        Using OpenFileDialog As New OpenFileDialog
            OpenFileDialog.Title = "Select file #2 to be compared..."
            OpenFileDialog.Filter = "Show All Files|*.*"

            If OpenFileDialog.ShowDialog() = DialogResult.OK Then txtFile2.Text = OpenFileDialog.FileName
        End Using
    End Sub

    Private Sub BtnBrowseFileForCompareKnownHash_Click(sender As Object, e As EventArgs) Handles btnBrowseFileForCompareKnownHash.Click
        If boolDidWePerformAPreviousHash Then txtKnownHash.Text = Nothing
        boolDidWePerformAPreviousHash = False

        Using OpenFileDialog As New OpenFileDialog
            OpenFileDialog.Title = "Select file for known hash comparison..."
            OpenFileDialog.Filter = "Show All Files|*.*"

            If OpenFileDialog.ShowDialog() = DialogResult.OK Then
                txtFileForKnownHash.Text = OpenFileDialog.FileName
                If Not String.IsNullOrWhiteSpace(txtKnownHash.Text) Then btnCompareAgainstKnownHash.Enabled = True
                txtKnownHash.Select()
            End If
        End Using
    End Sub

    Private Sub TxtKnownHash_TextChanged(sender As Object, e As EventArgs) Handles txtKnownHash.TextChanged
        pictureBoxVerifyAgainstResults.Image = Nothing
        ToolTip.SetToolTip(pictureBoxVerifyAgainstResults, "")

        If String.IsNullOrWhiteSpace(txtKnownHash.Text) Then
            lblCompareFileAgainstKnownHashType.Text = Nothing
            btnCompareAgainstKnownHash.Enabled = False
        Else
            txtKnownHash.Text = txtKnownHash.Text.Trim

            If txtKnownHash.Text.Length = 128 Or txtKnownHash.Text.Length = 96 Or txtKnownHash.Text.Length = 64 Or txtKnownHash.Text.Length = 40 Or txtKnownHash.Text.Length = 32 Then
                If Not String.IsNullOrWhiteSpace(txtFileForKnownHash.Text) Then btnCompareAgainstKnownHash.Enabled = True

                Select Case txtKnownHash.Text.Length
                    Case 32
                        lblCompareFileAgainstKnownHashType.Text = "Hash Type: MD5"
                    Case 40
                        lblCompareFileAgainstKnownHashType.Text = "Hash Type: SHA1"
                    Case 64
                        lblCompareFileAgainstKnownHashType.Text = "Hash Type: SHA256"
                    Case 96
                        lblCompareFileAgainstKnownHashType.Text = "Hash Type: SHA384"
                    Case 128
                        lblCompareFileAgainstKnownHashType.Text = "Hash Type: SHA512"
                End Select
            Else
                lblCompareFileAgainstKnownHashType.Text = Nothing
                btnCompareAgainstKnownHash.Enabled = False
            End If
        End If
    End Sub

    Private Sub TxtKnownHash_KeyUp(sender As Object, e As KeyEventArgs) Handles txtKnownHash.KeyUp
        If e.KeyCode = Keys.Enter And (txtKnownHash.Text.Length = 128 Or txtKnownHash.Text.Length = 96 Or txtKnownHash.Text.Length = 64 Or txtKnownHash.Text.Length = 40 Or txtKnownHash.Text.Length = 32) Then
            btnCompareAgainstKnownHash.PerformClick()
        End If
    End Sub

    Private Sub BtnCompareAgainstKnownHash_Click(sender As Object, e As EventArgs) Handles btnCompareAgainstKnownHash.Click
        If btnCompareAgainstKnownHash.Text = "Abort Processing" Then
            Dim result As MsgBoxResult = MsgBox("Are you sure you want to abort processing?", MsgBoxStyle.Question Or MsgBoxStyle.YesNo, strMessageBoxTitleText)

            If result = MsgBoxResult.Yes AndAlso workingThread IsNot Nothing Then
                boolAbortThread = True
                boolBackgroundThreadWorking = False
            End If

            Exit Sub
        End If

        txtFileForKnownHash.Text = txtFileForKnownHash.Text.Trim

        If Not IO.File.Exists(txtFileForKnownHash.Text) Then
            MsgBox("File doesn't exist.", MsgBoxStyle.Critical, strMessageBoxTitleText)
            Exit Sub
        End If

        txtFileForKnownHash.Enabled = False
        btnBrowseFileForCompareKnownHash.Enabled = False
        txtKnownHash.Enabled = False
        btnCompareAgainstKnownHash.Text = "Abort Processing"
        boolDidWePerformAPreviousHash = True
        compareAgainstKnownHashProgressBar.Visible = True
        intCurrentlyActiveTab = TabNumberCompareFileAgainstKnownHashTab

        workingThread = New Threading.Thread(Sub()
                                                 Try
                                                     If boolAbortThread Then Throw New MyThreadAbortException()
                                                     boolBackgroundThreadWorking = True
                                                     Dim checksumType As HashAlgorithmName

                                                     Select Case txtKnownHash.Text.Length
                                                         Case 32
                                                             checksumType = HashAlgorithmName.MD5
                                                         Case 40
                                                             checksumType = HashAlgorithmName.SHA1
                                                         Case 64
                                                             checksumType = HashAlgorithmName.SHA256
                                                         Case 96
                                                             checksumType = HashAlgorithmName.SHA384
                                                         Case 128
                                                             checksumType = HashAlgorithmName.SHA512
                                                     End Select

                                                     Dim strChecksum As String = Nothing
                                                     Dim percentage As Double
                                                     Dim subRoutine As New ChecksumStatusUpdaterDelegate(Sub(size As Long, totalBytesRead As Long)
                                                                                                             Try
                                                                                                                 MyInvoke(Sub()
                                                                                                                              percentage = If(totalBytesRead = 0 Or size = 0, 0, totalBytesRead / size * 100) ' This fixes a possible divide by zero exception.
                                                                                                                              compareAgainstKnownHashProgressBar.Value = percentage
                                                                                                                              ProgressForm.SetTaskbarProgressBarValue(compareAgainstKnownHashProgressBar.Value)
                                                                                                                              lblCompareAgainstKnownHashStatus.Text = $"{FileSizeToHumanSize(totalBytesRead)} of {FileSizeToHumanSize(size)} ({MyRoundingFunction(percentage, byteRoundPercentages)}%) has been processed."
                                                                                                                              If chkShowPercentageInWindowTitleBar.Checked Then Text = $"{strWindowTitle} ({MyRoundingFunction(percentage, byteRoundPercentages)}% Completed)"
                                                                                                                          End Sub, Me)
                                                                                                             Catch ex As Exception
                                                                                                             End Try
                                                                                                         End Sub)

                                                     Dim myStopWatch As Stopwatch = Stopwatch.StartNew
                                                     Dim allTheHashes As AllTheHashes = Nothing
                                                     Dim exceptionObject As Exception = Nothing
                                                     Dim boolSuccessful As Boolean = DoChecksumWithAttachedSubRoutine(txtFileForKnownHash.Text, allTheHashes, subRoutine, exceptionObject)
                                                     strChecksum = GetDataFromAllTheHashes(checksumType, allTheHashes)

                                                     MyInvoke(Sub()
                                                                  txtFileForKnownHash.Enabled = True
                                                                  btnBrowseFileForCompareKnownHash.Enabled = True
                                                                  txtKnownHash.Enabled = True
                                                                  btnCompareAgainstKnownHash.Text = "Compare File Against Known Hash"
                                                                  btnCompareAgainstKnownHash.Enabled = False
                                                                  lblCompareAgainstKnownHashStatus.Text = strNoBackgroundProcesses
                                                                  compareAgainstKnownHashProgressBar.Value = 0
                                                                  compareAgainstKnownHashProgressBar.Visible = False
                                                                  ProgressForm.SetTaskbarProgressBarValue(0)
                                                                  Text = strWindowTitle

                                                                  If boolSuccessful Then
                                                                      If strChecksum.Equals(txtKnownHash.Text.Trim, StringComparison.OrdinalIgnoreCase) Then
                                                                          pictureBoxVerifyAgainstResults.Image = Global.Hasher.My.Resources.Resources.good_check
                                                                          ToolTip.SetToolTip(pictureBoxVerifyAgainstResults, "Checksum Verified!")
                                                                          MsgBox($"The checksums match!{DoubleCRLF}Processing completed in {TimespanToHMS(myStopWatch.Elapsed)}.", MsgBoxStyle.Information, strMessageBoxTitleText)
                                                                      Else
                                                                          pictureBoxVerifyAgainstResults.Image = Global.Hasher.My.Resources.Resources.bad_check
                                                                          ToolTip.SetToolTip(pictureBoxVerifyAgainstResults, "Checksum verification failed, checksum didn't match!")
                                                                          MsgBox($"The checksums DON'T match!{DoubleCRLF}Processing completed in {TimespanToHMS(myStopWatch.Elapsed)}.", MsgBoxStyle.Critical, strMessageBoxTitleText)
                                                                      End If
                                                                  Else
                                                                      pictureBoxVerifyAgainstResults.Image = Global.Hasher.My.Resources.Resources.bad_check
                                                                      MsgBox("There was an error while calculating the checksum.", MsgBoxStyle.Critical, strMessageBoxTitleText)
                                                                  End If

                                                                  boolBackgroundThreadWorking = False
                                                                  workingThread = Nothing
                                                              End Sub, Me)
                                                 Catch ex As MyThreadAbortException
                                                     MyInvoke(Sub()
                                                                  If Not boolClosingWindow Then
                                                                      txtFileForKnownHash.Enabled = True
                                                                      btnBrowseFileForCompareKnownHash.Enabled = True
                                                                      txtKnownHash.Enabled = True
                                                                      btnCompareAgainstKnownHash.Text = "Compare File Against Known Hash"
                                                                      compareAgainstKnownHashProgressBar.Value = 0
                                                                      compareAgainstKnownHashProgressBar.Visible = False
                                                                      ProgressForm.SetTaskbarProgressBarValue(0)
                                                                      lblCompareFilesStatus.Text = strNoBackgroundProcesses
                                                                      Text = strWindowTitle
                                                                  End If

                                                                  boolBackgroundThreadWorking = False
                                                                  workingThread = Nothing
                                                                  If Not boolClosingWindow Then MsgBox("Processing aborted.", MsgBoxStyle.Information, strMessageBoxTitleText)
                                                              End Sub, Me)
                                                 Finally
                                                     intCurrentlyActiveTab = TabNumberNull
                                                 End Try
                                             End Sub) With {
            .Priority = GetThreadPriority(),
            .Name = "Hash Generation Thread",
            .IsBackground = True
        }
        workingThread.Start()
    End Sub

    Private Sub Form1_ResizeEnd(sender As Object, e As EventArgs) Handles Me.ResizeEnd
        My.Settings.windowSize = Size
        listFiles.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCellsExceptHeaders
        verifyHashesListFiles.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCellsExceptHeaders
    End Sub

    Private Sub Form1_ResizeBegin(sender As Object, e As EventArgs) Handles Me.ResizeBegin
        listFiles.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None
        verifyHashesListFiles.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None
    End Sub

    Private Sub Form1_Resize(sender As Object, e As EventArgs) Handles Me.Resize
        If boolDoneLoading Then My.Settings.boolWindowMaximized = WindowState = FormWindowState.Maximized
    End Sub

    Private Function GetSHA160HashOfString(inputString As String) As String
        Using sha256Engine As HashAlgorithm = New SHA1CryptoServiceProvider
            Dim byteOutput As Byte() = sha256Engine.ComputeHash(System.Text.Encoding.UTF8.GetBytes(inputString))
            Return BitConverter.ToString(byteOutput).ToLower().Replace("-", "")
        End Using
    End Function

    Private Function GetSHA256HashOfString(inputString As String) As String
        Using sha256Engine As HashAlgorithm = New SHA256CryptoServiceProvider
            Dim byteOutput As Byte() = sha256Engine.ComputeHash(System.Text.Encoding.UTF8.GetBytes(inputString))
            Return BitConverter.ToString(byteOutput).ToLower().Replace("-", "")
        End Using
    End Function

    Private Function GetHashOfString(inputString As String) As AllTheHashes
        Using md5Engine As HashAlgorithm = New MD5CryptoServiceProvider, sha160Engine As HashAlgorithm = New SHA1CryptoServiceProvider, sha256Engine As HashAlgorithm = New SHA256CryptoServiceProvider, sha384Engine As HashAlgorithm = New SHA384CryptoServiceProvider, sha512Engine As HashAlgorithm = New SHA512CryptoServiceProvider
            Dim byteArray As Byte() = System.Text.Encoding.UTF8.GetBytes(inputString)

            md5Engine.ComputeHash(byteArray)
            sha160Engine.ComputeHash(byteArray)
            sha256Engine.ComputeHash(byteArray)
            sha384Engine.ComputeHash(byteArray)
            sha512Engine.ComputeHash(byteArray)

            Dim allTheHashes As New AllTheHashes With {
                .Md5 = BitConverter.ToString(md5Engine.Hash).ToLower().Replace("-", ""),
                .Sha160 = BitConverter.ToString(sha160Engine.Hash).ToLower().Replace("-", ""),
                .Sha256 = BitConverter.ToString(sha256Engine.Hash).ToLower().Replace("-", ""),
                .Sha384 = BitConverter.ToString(sha384Engine.Hash).ToLower().Replace("-", ""),
                .Sha512 = BitConverter.ToString(sha512Engine.Hash).ToLower().Replace("-", "")
            }

            Return allTheHashes
        End Using
    End Function

    Private Sub ListFiles_ColumnWidthChanged(sender As Object, e As DataGridViewColumnEventArgs) Handles listFiles.ColumnWidthChanged
        If Not boolDoneLoading Then Exit Sub
        My.Settings.hashIndividualFilesFileNameColumnSize = colFileName.Width
        My.Settings.hashIndividualFilesFileSizeColumnSize = colFileSize.Width
        My.Settings.hashIndividualFilesChecksumColumnSize = colChecksum.Width
        My.Settings.hashIndividualFilesComputeTimeColumnSize = colComputeTime.Width
    End Sub

    Private Sub VerifyHashesListFiles_ColumnWidthChanged(sender As Object, e As DataGridViewColumnEventArgs) Handles verifyHashesListFiles.ColumnWidthChanged
        If Not boolDoneLoading Then Exit Sub
        My.Settings.verifyHashFileNameColumnSize = colFile.Width
        My.Settings.verifyHashFileSizeColumnSize = colFileSize2.Width
        My.Settings.verifyHashFileResults = colResults.Width
        My.Settings.verifyHashComputeTimeColumnSize = colComputeTime2.Width
        My.Settings.newHashChecksumColumnSize = colNewHash.Width
    End Sub

    Private Sub TxtFile1_DragEnter(sender As Object, e As DragEventArgs) Handles txtFile1.DragEnter
        e.Effect = If(e.Data.GetDataPresent(DataFormats.FileDrop), DragDropEffects.All, DragDropEffects.None)
    End Sub

    Private Sub TxtFile1_DragDrop(sender As Object, e As DragEventArgs) Handles txtFile1.DragDrop
        Dim receivedData As String() = DirectCast(e.Data.GetData(DataFormats.FileDrop), String())
        If receivedData.Count = 1 Then
            txtFile1.Text = receivedData(0)
            lblFile1Hash.Text = strHashChecksumToBeComputed
            pictureBoxCompareFiles.Image = Nothing
            ToolTip.SetToolTip(pictureBoxCompareFiles, "")
        End If
    End Sub

    Private Sub TxtFile2_DragEnter(sender As Object, e As DragEventArgs) Handles txtFile2.DragEnter
        e.Effect = If(e.Data.GetDataPresent(DataFormats.FileDrop), DragDropEffects.All, DragDropEffects.None)
    End Sub

    Private Sub TxtFile2_DragDrop(sender As Object, e As DragEventArgs) Handles txtFile2.DragDrop
        Dim receivedData As String() = DirectCast(e.Data.GetData(DataFormats.FileDrop), String())
        If receivedData.Count = 1 Then
            txtFile2.Text = receivedData(0)
            lblFile2Hash.Text = strHashChecksumToBeComputed
            pictureBoxCompareFiles.Image = Nothing
            ToolTip.SetToolTip(pictureBoxCompareFiles, "")
        End If
    End Sub

    Private Sub TxtFileForKnownHash_DragEnter(sender As Object, e As DragEventArgs) Handles txtFileForKnownHash.DragEnter
        e.Effect = If(e.Data.GetDataPresent(DataFormats.FileDrop), DragDropEffects.All, DragDropEffects.None)
    End Sub

    Private Sub TxtFileForKnownHash_DragDrop(sender As Object, e As DragEventArgs) Handles txtFileForKnownHash.DragDrop
        Dim receivedData As String() = DirectCast(e.Data.GetData(DataFormats.FileDrop), String())
        If receivedData.Count = 1 Then txtFileForKnownHash.Text = receivedData(0)
    End Sub

    Private Sub BtnOpenExistingHashFile_DragEnter(sender As Object, e As DragEventArgs) Handles btnOpenExistingHashFile.DragEnter
        e.Effect = If(e.Data.GetDataPresent(DataFormats.FileDrop), DragDropEffects.All, DragDropEffects.None)
        Dim receivedData As String() = DirectCast(e.Data.GetData(DataFormats.FileDrop), String())

        If receivedData.Count = 1 Then
            Dim fileInfo As New IO.FileInfo(receivedData(0))

            If Not fileInfo.Extension.Equals(".md5", StringComparison.OrdinalIgnoreCase) And Not fileInfo.Extension.Equals(".sha1", StringComparison.OrdinalIgnoreCase) And Not fileInfo.Extension.Equals(".sha256", StringComparison.OrdinalIgnoreCase) And Not fileInfo.Extension.Equals(".sha384", StringComparison.OrdinalIgnoreCase) And Not fileInfo.Extension.Equals(".sha512", StringComparison.OrdinalIgnoreCase) Then
                e.Effect = DragDropEffects.None
            End If
        End If
    End Sub

    Private Sub BtnAssociate_Click(sender As Object, e As EventArgs) Handles btnAssociate.Click
        FileAssociation.SelfCreateAssociation(".md5", "Checksum File")
        FileAssociation.SelfCreateAssociation(".sha1", "Checksum File")
        FileAssociation.SelfCreateAssociation(".sha2", "Checksum File")
        FileAssociation.SelfCreateAssociation(".sha256", "Checksum File")
        FileAssociation.SelfCreateAssociation(".sha384", "Checksum File")
        FileAssociation.SelfCreateAssociation(".sha512", "Checksum File")
        FileAssociation.SelfCreateAssociation(".hasher", "Checksum File")

        MsgBox("File association complete.", MsgBoxStyle.Information, strMessageBoxTitleText)
    End Sub

    Private Sub BtnAddHasherToAllFiles_Click(sender As Object, e As EventArgs) Handles btnAddHasherToAllFiles.Click
        FileAssociation.AddAssociationWithAllFiles()
        MsgBox("File association complete.", MsgBoxStyle.Information, strMessageBoxTitleText)
    End Sub

    Private Sub BtnOpenExistingHashFile_DragDrop(sender As Object, e As DragEventArgs) Handles btnOpenExistingHashFile.DragDrop
        Dim receivedData As String() = DirectCast(e.Data.GetData(DataFormats.FileDrop), String())
        If receivedData.Count = 1 Then
            Dim strReceivedFileName As String = receivedData(0)
            Dim fileInfo As New IO.FileInfo(strReceivedFileName)

            If fileInfo.Extension.Equals(".md5", StringComparison.OrdinalIgnoreCase) Or fileInfo.Extension.Equals(".sha1", StringComparison.OrdinalIgnoreCase) Or fileInfo.Extension.Equals(".sha256", StringComparison.OrdinalIgnoreCase) Or fileInfo.Extension.Equals(".sha384", StringComparison.OrdinalIgnoreCase) Or fileInfo.Extension.Equals(".sha512", StringComparison.OrdinalIgnoreCase) Or fileInfo.Extension.Equals(".hasher", StringComparison.OrdinalIgnoreCase) Then
                btnTransferToHashIndividualFilesTab.Enabled = False
                btnOpenExistingHashFile.Text = "Abort Processing"
                verifyHashesListFiles.Rows.Clear()
                ProcessExistingHashFile(strReceivedFileName)
            Else
                MsgBox("Invalid file type.", MsgBoxStyle.Critical, strMessageBoxTitleText)
            End If
        End If
    End Sub

    Private Sub ListFiles_SelectionChanged(sender As Object, e As EventArgs) Handles listFiles.SelectionChanged
        UpdateFilesListCountHeader(True)
    End Sub

    Private Sub ListFiles_KeyUp(sender As Object, e As KeyEventArgs) Handles listFiles.KeyUp
        If e.KeyCode = Keys.Delete Then btnRemoveSelectedFiles.PerformClick()
    End Sub

    Private Sub ChkSortByFileSizeAfterLoadingHashFile_Click(sender As Object, e As EventArgs) Handles chkSortByFileSizeAfterLoadingHashFile.Click
        My.Settings.boolSortByFileSizeAfterLoadingHashFile = chkSortByFileSizeAfterLoadingHashFile.Checked
    End Sub

    Private Sub ChkSaveChecksumFilesWithRelativePaths_Click(sender As Object, e As EventArgs) Handles chkSaveChecksumFilesWithRelativePaths.Click
        My.Settings.boolSaveChecksumFilesWithRelativePaths = chkSaveChecksumFilesWithRelativePaths.Checked
    End Sub

    Private Sub ChkSortFileListingAfterAddingFilesToHash_Click(sender As Object, e As EventArgs) Handles chkSortFileListingAfterAddingFilesToHash.Click
        My.Settings.boolSortFileListingAfterAddingFilesToHash = chkSortFileListingAfterAddingFilesToHash.Checked
    End Sub

    Private Sub ChkUseMilliseconds_Click(sender As Object, e As EventArgs) Handles chkUseMilliseconds.Click
        My.Settings.boolUseMilliseconds = chkUseMilliseconds.Checked
    End Sub

    Private Sub ChkDisplayHashesInUpperCase_Click(sender As Object, e As EventArgs) Handles chkDisplayHashesInUpperCase.Click
        Dim boolDisplayHashesInUpperCase As Boolean = chkDisplayHashesInUpperCase.Checked
        My.Settings.boolDisplayHashesInUpperCase = chkDisplayHashesInUpperCase.Checked

        If listFiles.Rows.Count <> 0 Then
            For Each item As MyDataGridViewRow In listFiles.Rows
                item.Cells(2).Value = If(boolDisplayHashesInUpperCase, item.Hash.ToUpper, item.Hash.ToLower)
            Next
        End If
        If txtHashResults.Rows.Count <> 0 Then
            For Each item As DataGridViewRow In txtHashResults.Rows
                item.Cells(1).Value = If(boolDisplayHashesInUpperCase, item.Cells(1).Value.ToUpper, item.Cells(1).Value.ToLower)
            Next
        End If

        If Not String.IsNullOrWhiteSpace(txtHashResults.Text) Then
            txtHashResults.Text = If(boolDisplayHashesInUpperCase, txtHashResults.Text.ToUpper, txtHashResults.Text.ToLower)
        End If
        If Not String.IsNullOrWhiteSpace(lblFile1Hash.Text) Then
            lblFile1Hash.Text = If(boolDisplayHashesInUpperCase, lblFile1Hash.Text.ToUpper, lblFile1Hash.Text.ToLower)
        End If
        If Not String.IsNullOrWhiteSpace(lblFile2Hash.Text) Then
            lblFile2Hash.Text = If(boolDisplayHashesInUpperCase, lblFile2Hash.Text.ToUpper, lblFile2Hash.Text.ToLower)
        End If
    End Sub

    Private Sub BtnSetValidColor_Click(sender As Object, e As EventArgs) Handles btnSetValidColor.Click
        Using colorDialog As New ColorDialog() With {.Color = validColor}
            If colorDialog.ShowDialog() = DialogResult.OK Then
                My.Settings.validColor = colorDialog.Color
                lblValidColor.BackColor = colorDialog.Color
                validColor = colorDialog.Color
                If verifyHashesListFiles.Rows.Count <> 0 Then UpdateColorsInList(ColorType.Valid, colorDialog.Color)
            End If
        End Using
    End Sub

    Private Sub BtnSetNotValidColor_Click(sender As Object, e As EventArgs) Handles btnSetNotValidColor.Click
        Using colorDialog As New ColorDialog() With {.Color = notValidColor}
            If colorDialog.ShowDialog() = DialogResult.OK Then
                My.Settings.notValidColor = colorDialog.Color
                lblNotValidColor.BackColor = colorDialog.Color
                notValidColor = colorDialog.Color
                If verifyHashesListFiles.Rows.Count <> 0 Then UpdateColorsInList(ColorType.NotValid, colorDialog.Color)
            End If
        End Using
    End Sub

    Private Sub BtnFileNotFoundColor_Click(sender As Object, e As EventArgs) Handles btnFileNotFoundColor.Click
        Using colorDialog As New ColorDialog() With {.Color = fileNotFoundColor}
            If colorDialog.ShowDialog() = DialogResult.OK Then
                My.Settings.fileNotFoundColor = colorDialog.Color
                lblFileNotFoundColor.BackColor = colorDialog.Color
                fileNotFoundColor = colorDialog.Color
                If verifyHashesListFiles.Rows.Count <> 0 Then UpdateColorsInList(ColorType.NotFound, colorDialog.Color)
            End If
        End Using
    End Sub

    Private Sub BtnSetColorsBackToDefaults_Click(sender As Object, e As EventArgs) Handles btnSetColorsBackToDefaults.Click
        My.Settings.validColor = Color.LightGreen
        lblValidColor.BackColor = Color.LightGreen
        validColor = Color.LightGreen

        My.Settings.notValidColor = Color.Pink
        lblNotValidColor.BackColor = Color.Pink
        notValidColor = Color.Pink

        My.Settings.fileNotFoundColor = Color.LightGray
        lblFileNotFoundColor.BackColor = Color.LightGray
        fileNotFoundColor = Color.LightGray

        If verifyHashesListFiles.Rows.Count <> 0 Then
            For Each item As MyDataGridViewRow In verifyHashesListFiles.Rows
                If item.ColorType = ColorType.NotFound Then
                    item.MyColor = Color.LightGray
                ElseIf item.ColorType = ColorType.NotValid Then
                    item.MyColor = Color.Pink
                ElseIf item.ColorType = ColorType.Valid Then
                    item.MyColor = Color.LightGreen
                End If
            Next
        End If
    End Sub

    Private Sub UpdateColorsInList(ColorType As ColorType, NewColor As Color)
        For Each item As MyDataGridViewRow In verifyHashesListFiles.Rows
            If item.ColorType = ColorType Then item.MyColor = NewColor
        Next
    End Sub

    Private Sub BtnSetBufferSize_Click(sender As Object, e As EventArgs) Handles btnSetBufferSize.Click
        Dim shortBufferSize As Short = Decimal.ToInt16(bufferSize.Value)
        intBufferSize = shortBufferSize * 1024 * 1024
        My.Settings.shortBufferSize = shortBufferSize
        btnSetBufferSize.Enabled = False
        MsgBox($"Data buffer size set successfully to {shortBufferSize} {If(shortBufferSize = 1, "MB", "MBs")}.", MsgBoxStyle.Information, strMessageBoxTitleText)
    End Sub

    Private Sub BufferSize_ValueChanged(sender As Object, e As EventArgs) Handles bufferSize.ValueChanged
        btnSetBufferSize.Enabled = My.Settings.shortBufferSize <> Decimal.ToInt16(bufferSize.Value)
    End Sub

    Private Sub BtnPerformBenchmark_Click(sender As Object, e As EventArgs) Handles btnPerformBenchmark.Click
        Using benchmark As New Benchmark With {.StartPosition = FormStartPosition.CenterScreen}
            benchmark.ShowDialog(Me)

            If benchmark.boolSetBufferSize Then
                bufferSize.Value = Convert.ToDecimal(benchmark.shortBufferSize)
                intBufferSize = benchmark.shortBufferSize * 1024 * 1024
                My.Settings.shortBufferSize = benchmark.shortBufferSize
                btnSetBufferSize.Enabled = False
                MsgBox($"Data buffer size set successfully to {benchmark.shortBufferSize} {If(benchmark.shortBufferSize = 1, "MB", "MBs")}.", MsgBoxStyle.Information, strMessageBoxTitleText)
            End If
        End Using
    End Sub

    Private Sub ChkUseCommasInNumbers_Click(sender As Object, e As EventArgs) Handles chkUseCommasInNumbers.Click
        My.Settings.boolUseCommasInNumbers = chkUseCommasInNumbers.Checked
    End Sub

    Private Function GetThreadPriority() As Threading.ThreadPriority
        Select Case My.Settings.taskPriority
            Case 0
                Return Threading.ThreadPriority.Lowest
            Case 1
                Return Threading.ThreadPriority.BelowNormal
            Case 2
                Return Threading.ThreadPriority.Normal
            Case 3
                Return Threading.ThreadPriority.AboveNormal
            Case 4
                Return Threading.ThreadPriority.Highest
            Case Else
                Return Threading.ThreadPriority.Highest
        End Select
    End Function

    Private Sub BtnSetRoundFileSizes_Click(sender As Object, e As EventArgs) Handles btnSetRoundFileSizes.Click
        My.Settings.roundFileSizes = roundFileSizes.Value
        byteRoundFileSizes = roundFileSizes.Value
        btnSetRoundFileSizes.Enabled = False
        MsgBox("Preference saved.", MsgBoxStyle.Information, strMessageBoxTitleText)
    End Sub

    Private Sub BtnSetRoundPercentages_Click(sender As Object, e As EventArgs) Handles btnSetRoundPercentages.Click
        My.Settings.roundPercentages = roundPercentages.Value
        byteRoundPercentages = roundPercentages.Value
        btnSetRoundPercentages.Enabled = False
        MsgBox("Preference saved.", MsgBoxStyle.Information, strMessageBoxTitleText)
    End Sub

    Private Sub RoundFileSizes_ValueChanged(sender As Object, e As EventArgs) Handles roundFileSizes.ValueChanged
        btnSetRoundFileSizes.Enabled = My.Settings.roundFileSizes <> Decimal.ToByte(roundFileSizes.Value)
    End Sub

    Private Sub RoundPercentages_ValueChanged(sender As Object, e As EventArgs) Handles roundPercentages.ValueChanged
        btnSetRoundPercentages.Enabled = My.Settings.roundPercentages <> Decimal.ToByte(roundPercentages.Value)
    End Sub

    Private Sub TaskPriority_SelectedIndexChanged(sender As Object, e As EventArgs) Handles taskPriority.SelectedIndexChanged
        If boolDoneLoading Then My.Settings.taskPriority = taskPriority.SelectedIndex
    End Sub

    Private Sub ChkCheckForUpdates_Click(sender As Object, e As EventArgs) Handles chkCheckForUpdates.Click
        My.Settings.boolCheckForUpdates = chkCheckForUpdates.Checked
        chkHideCheckForUpdatesButton.Visible = chkCheckForUpdates.Checked
        btnCheckForUpdates.Visible = Not chkHideCheckForUpdatesButton.Checked Or Not chkCheckForUpdates.Checked
    End Sub

    Private Sub ChkAutoAddExtension_Click(sender As Object, e As EventArgs) Handles chkAutoAddExtension.Click
        If Not chkAutoAddExtension.Checked AndAlso MsgBox($"You are disabling a highly recommended option, it is HIGHLY recommended that you re-enable this option to prevent accidental data loss.{DoubleCRLF}Are you sure you want to do this?", MsgBoxStyle.Question + MsgBoxStyle.YesNo, strMessageBoxTitleText) = MsgBoxResult.No Then
            chkAutoAddExtension.Checked = True
        End If
        My.Settings.boolAutoAddExtension = chkAutoAddExtension.Checked
    End Sub

    Private Sub FillInChecksumLabelsOnCompareFilesTab(checksumType As HashAlgorithmName)
        Dim strChecksum1 As String = GetDataFromAllTheHashes(checksumType, compareFilesAllTheHashes1)
        Dim strChecksum2 As String = GetDataFromAllTheHashes(checksumType, compareFilesAllTheHashes2)

        If Not String.IsNullOrWhiteSpace(strChecksum1) AndAlso Not String.IsNullOrWhiteSpace(strChecksum2) Then
            lblFile1Hash.Text = $"Hash/Checksum: {If(chkDisplayHashesInUpperCase.Checked, strChecksum1.ToUpper, strChecksum1.ToLower)}"
            lblFile2Hash.Text = $"Hash/Checksum: {If(chkDisplayHashesInUpperCase.Checked, strChecksum2.ToUpper, strChecksum2.ToLower)}"
            ToolTip.SetToolTip(lblFile1Hash, strChecksum1)
            ToolTip.SetToolTip(lblFile2Hash, strChecksum2)
        End If
    End Sub

    Private Sub CompareRadioMD5_Click(sender As Object, e As EventArgs) Handles compareRadioMD5.Click
        FillInChecksumLabelsOnCompareFilesTab(HashAlgorithmName.MD5)
    End Sub

    Private Sub CompareRadioSHA1_Click(sender As Object, e As EventArgs) Handles compareRadioSHA1.Click
        FillInChecksumLabelsOnCompareFilesTab(HashAlgorithmName.SHA1)
    End Sub

    Private Sub CompareRadioSHA256_Click(sender As Object, e As EventArgs) Handles compareRadioSHA256.Click
        FillInChecksumLabelsOnCompareFilesTab(HashAlgorithmName.SHA256)
    End Sub

    Private Sub CompareRadioSHA384_Click(sender As Object, e As EventArgs) Handles compareRadioSHA384.Click
        FillInChecksumLabelsOnCompareFilesTab(HashAlgorithmName.SHA384)
    End Sub

    Private Sub CompareRadioSHA512_Click(sender As Object, e As EventArgs) Handles compareRadioSHA512.Click
        FillInChecksumLabelsOnCompareFilesTab(HashAlgorithmName.SHA512)
    End Sub

    Private Sub BtnTransferToHashIndividualFilesTab_Click(sender As Object, e As EventArgs) Handles btnTransferToHashIndividualFilesTab.Click
        Threading.ThreadPool.QueueUserWorkItem(Sub()
                                                   MyInvoke(Sub()
                                                                btnTransferToHashIndividualFilesTab.Enabled = False
                                                                VerifyHashProgressBar.Visible = True
                                                                lblVerifyHashStatus.Visible = True
                                                                verifyHashesListFiles.Size = New Size(verifyHashesListFiles.Size.Width, verifyHashesListFiles.Size.Height - 72)

                                                                If chkClearBeforeTransferringFromVerifyToHash.Checked Then
                                                                    listFiles.Rows.Clear()
                                                                    filesInListFiles.Clear()
                                                                End If
                                                            End Sub, Me)

                                                   boolBackgroundThreadWorking = True
                                                   Dim listOfDataGridRows As New List(Of MyDataGridViewRow)
                                                   Dim intLineCounter As Integer = 0
                                                   Dim strHashString As String
                                                   Dim checksumType As HashAlgorithmName

                                                   Select Case My.Settings.defaultHash
                                                       Case Byte.Parse(0)
                                                           checksumType = HashAlgorithmName.MD5
                                                       Case Byte.Parse(1)
                                                           checksumType = HashAlgorithmName.SHA1
                                                       Case Byte.Parse(2)
                                                           checksumType = HashAlgorithmName.SHA256
                                                       Case Byte.Parse(3)
                                                           checksumType = HashAlgorithmName.SHA384
                                                       Case Byte.Parse(4)
                                                           checksumType = HashAlgorithmName.SHA512
                                                   End Select

                                                   Dim itemToBeAdded As MyDataGridViewRow

                                                   For Each item As MyDataGridViewRow In verifyHashesListFiles.Rows
                                                       intLineCounter += 1
                                                       MyInvoke(Sub()
                                                                    VerifyHashProgressBar.Value = intLineCounter / verifyHashesListFiles.Rows.Count * 100
                                                                    ProgressForm.SetTaskbarProgressBarValue(VerifyHashProgressBar.Value)
                                                                    lblVerifyHashStatus.Text = $"Processing item {MyToString(intLineCounter)} of {MyToString(verifyHashesListFiles.Rows.Count)} ({VerifyHashProgressBar.Value}%)."
                                                                End Sub, Me)

                                                       If Not filesInListFiles.Contains(item.FileName.Trim.ToLower) And IO.File.Exists(item.FileName) Then
                                                           filesInListFiles.Add(item.FileName.Trim.ToLower)

                                                           itemToBeAdded = New MyDataGridViewRow() With {
                                                               .FileSize = New IO.FileInfo(item.FileName).Length,
                                                               .FileName = item.FileName
                                                           }
                                                           With itemToBeAdded
                                                               .CreateCells(listFiles)
                                                               strHashString = GetDataFromAllTheHashes(checksumType, item.AllTheHashes)
                                                               .Cells(0).Value = item.FileName
                                                               .Cells(1).Value = FileSizeToHumanSize(itemToBeAdded.FileSize)
                                                               .Cells(2).Value = If(chkDisplayHashesInUpperCase.Checked, strHashString.ToUpper, strHashString.ToLower)
                                                               .Cells(3).Value = TimespanToHMS(item.ComputeTime)
                                                               .AllTheHashes = item.AllTheHashes
                                                               .Hash = strHashString
                                                               .DefaultCellStyle.Padding = New Padding(0, 2, 0, 2)
                                                           End With

                                                           listOfDataGridRows.Add(itemToBeAdded)
                                                       End If
                                                   Next

                                                   MyInvoke(Sub()
                                                                boolBackgroundThreadWorking = False
                                                                listFiles.Rows.AddRange(listOfDataGridRows.ToArray)
                                                                listOfDataGridRows = Nothing

                                                                If chkSortFileListingAfterAddingFilesToHash.Checked Then SortLogsByFileSize(1, sortOrderForListFiles, listFiles)
                                                                colChecksum.HeaderText = strColumnTitleChecksumSHA256
                                                                TabControl1.SelectedIndex = TabNumberHashIndividualFilesTab
                                                                btnIndividualFilesCopyToClipboard.Enabled = True
                                                                btnIndividualFilesSaveResultsToDisk.Enabled = True

                                                                ProgressForm.SetTaskbarProgressBarValue(0)
                                                                VerifyHashProgressBar.Value = 0
                                                                lblVerifyHashStatus.Text = Nothing
                                                                btnTransferToHashIndividualFilesTab.Enabled = True
                                                                VerifyHashProgressBar.Visible = False
                                                                lblVerifyHashStatus.Visible = False
                                                                verifyHashesListFiles.Size = New Size(verifyHashesListFiles.Size.Width, verifyHashesListFiles.Size.Height + 72)
                                                            End Sub, Me)
                                               End Sub)
    End Sub

    Private Sub TxtFile1_TextChanged(sender As Object, e As EventArgs) Handles txtFile1.TextChanged
        If txtFile1.Text.Equals(txtFile2.Text, StringComparison.OrdinalIgnoreCase) Then
            MsgBox("You have selected the same file. Oops.", MsgBoxStyle.Information, strMessageBoxTitleText)
            txtFile1.Text = Nothing
        End If
        btnCompareFiles.Enabled = Not String.IsNullOrEmpty(txtFile1.Text) And Not String.IsNullOrEmpty(txtFile2.Text)
        lblFile1Hash.Text = strHashChecksumToBeComputed
        pictureBoxCompareFiles.Image = Nothing
        ToolTip.SetToolTip(pictureBoxCompareFiles, "")
    End Sub

    Private Sub TxtFile2_TextChanged(sender As Object, e As EventArgs) Handles txtFile2.TextChanged
        If txtFile1.Text.Equals(txtFile2.Text, StringComparison.OrdinalIgnoreCase) Then
            MsgBox("You have selected the same file. Oops.", MsgBoxStyle.Information, strMessageBoxTitleText)
            txtFile2.Text = Nothing
        End If
        btnCompareFiles.Enabled = Not String.IsNullOrEmpty(txtFile1.Text) And Not String.IsNullOrEmpty(txtFile2.Text)
        lblFile2Hash.Text = strHashChecksumToBeComputed
        pictureBoxCompareFiles.Image = Nothing
        ToolTip.SetToolTip(pictureBoxCompareFiles, "")
    End Sub

    Private Sub VerifyListFilesContextMenu_Opening(sender As Object, e As CancelEventArgs) Handles verifyListFilesContextMenu.Opening
        ViewChecksumDifferenceToolStripMenuItem.Visible = True
        verifyListFilesContextMenuLine1.Visible = True

        If verifyHashesListFiles.Rows.Count = 0 Or verifyHashesListFiles.SelectedRows.Count = 0 Then
            e.Cancel = True
            Exit Sub
        Else
            If String.IsNullOrEmpty(verifyHashesListFiles.SelectedRows(0).Cells(4).Value) Or workingThread IsNot Nothing Then
                ViewChecksumDifferenceToolStripMenuItem.Visible = False
                verifyListFilesContextMenuLine1.Visible = False
            ElseIf Not DirectCast(verifyHashesListFiles.SelectedRows(0), MyDataGridViewRow).BoolFileExists Or DirectCast(verifyHashesListFiles.SelectedRows(0), MyDataGridViewRow).BoolValidHash Then
                ViewChecksumDifferenceToolStripMenuItem.Visible = False
                verifyListFilesContextMenuLine1.Visible = False
            End If

            If verifyHashesListFiles.SelectedRows.Count = 1 Then
                Dim MyDataGridRow As MyDataGridViewRow = DirectCast(verifyHashesListFiles.SelectedRows(0), MyDataGridViewRow)

                globalAllTheHashes = MyDataGridRow.AllTheHashes
                verifyListFilesContextMenuFileName.Text = $"File Name: {MyDataGridRow.FileName}"

                With MyDataGridRow.AllTheHashes
                    ' We should only need to check this to avoid a NullReferenceException.
                    If String.IsNullOrWhiteSpace(.Sha512) Then
                        If Not MyDataGridRow.BoolFileExists Then
                            ' Shows file not found error
                            verifyListFilesContextMenuMD5.Text = "MD5: (Error, File Doesn't Exist)"
                            verifyListFilesContextMenuSHA160.Text = "SHA160: (Error, File Doesn't Exist)"
                            verifyListFilesContextMenuSHA256.Text = "SHA256: (Error, File Doesn't Exist)"
                            verifyListFilesContextMenuSHA384.Text = "SHA384: (Error, File Doesn't Exist)"
                            verifyListFilesContextMenuSHA512.Text = "SHA512: (Error, File Doesn't Exist)"
                        Else
                            ' Shows general error
                            verifyListFilesContextMenuMD5.Text = "MD5: (Error)"
                            verifyListFilesContextMenuSHA160.Text = "SHA160: (Error)"
                            verifyListFilesContextMenuSHA256.Text = "SHA256: (Error)"
                            verifyListFilesContextMenuSHA384.Text = "SHA384: (Error)"
                            verifyListFilesContextMenuSHA512.Text = "SHA512: (Error)"
                        End If
                    Else
                        ' Everything is good.
                        verifyListFilesContextMenuMD5.Text = $"MD5: {If(chkDisplayHashesInUpperCase.Checked, .Md5.ToUpper, .Md5.ToLower)}"
                        verifyListFilesContextMenuSHA160.Text = $"SHA160: {If(chkDisplayHashesInUpperCase.Checked, .Sha160.ToUpper, .Sha160.ToLower)}"
                        verifyListFilesContextMenuSHA256.Text = $"SHA256: {If(chkDisplayHashesInUpperCase.Checked, .Sha256.ToUpper, .Sha256.ToLower)}"
                        verifyListFilesContextMenuSHA384.Text = $"SHA384: {If(chkDisplayHashesInUpperCase.Checked, .Sha384.ToUpper, .Sha384.ToLower)}"
                        verifyListFilesContextMenuSHA512.Text = $"SHA512: {If(chkDisplayHashesInUpperCase.Checked, .Sha512.ToUpper, .Sha512.ToLower)}"
                    End If
                End With
            End If
        End If
    End Sub

    Private Sub ViewChecksumDifferenceToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ViewChecksumDifferenceToolStripMenuItem.Click
        Dim selectedItem As MyDataGridViewRow = verifyHashesListFiles.SelectedRows(0)
        Dim stringBuilder As New Text.StringBuilder()

        stringBuilder.AppendLine("Hash/Checksum Contained in Checksum File")
        With selectedItem.Hash
            stringBuilder.AppendLine(If(chkDisplayHashesInUpperCase.Checked, .ToUpper, .ToLower))
        End With
        stringBuilder.AppendLine()
        stringBuilder.AppendLine("Newly Computed Hash/Checksum")
        With GetDataFromAllTheHashes(checksumTypeForChecksumCompareWindow, selectedItem.AllTheHashes)
            stringBuilder.AppendLine(If(chkDisplayHashesInUpperCase.Checked, .ToUpper, .ToLower))
        End With

        Using frmChecksumDifference As New FrmChecksumDifference With {.Icon = Icon, .StartPosition = FormStartPosition.CenterParent}
            frmChecksumDifference.lblMainLabel.Text = stringBuilder.ToString.Trim

            Dim size As New Size(frmChecksumDifference.lblMainLabel.Size.Width + 40, frmChecksumDifference.Size.Height)

            With frmChecksumDifference
                .Size = size
                .MinimumSize = size
                .MaximumSize = size
            End With

            frmChecksumDifference.ShowDialog(Me)
        End Using

        stringBuilder = Nothing
    End Sub

    Private Sub TxtTextToHash_KeyUp(sender As Object, e As KeyEventArgs) Handles txtTextToHash.KeyUp
        If e.KeyCode = Keys.Back And String.IsNullOrWhiteSpace(txtTextToHash.Text) Then Media.SystemSounds.Exclamation.Play()
    End Sub

    Private Sub ChkDisplayValidChecksumString_Click(sender As Object, e As EventArgs) Handles chkDisplayValidChecksumString.Click
        My.Settings.boolDisplayValidChecksumString = chkDisplayValidChecksumString.Checked
    End Sub

    Private Sub ChkOpenInExplorer_Click(sender As Object, e As EventArgs) Handles chkOpenInExplorer.Click
        My.Settings.boolOpenInExplorer = chkOpenInExplorer.Checked
    End Sub

    Private Sub ChkShowPercentageInWindowTitleBar_Click(sender As Object, e As EventArgs) Handles chkShowPercentageInWindowTitleBar.Click
        My.Settings.boolShowPercentageInWindowTitleBar = chkShowPercentageInWindowTitleBar.Checked
        If Not chkShowPercentageInWindowTitleBar.Checked Then Text = strWindowTitle
    End Sub

    Private Sub BtnRetestFailedFiles_Click(sender As Object, e As EventArgs) Handles btnRetestFailedFiles.Click
        verifyHashesListFiles.Size = New Size(verifyHashesListFiles.Size.Width, verifyHashesListFiles.Size.Height - 72)
        btnTransferToHashIndividualFilesTab.Enabled = False

        workingThread = New Threading.Thread(Sub()
                                                 Dim itemOnGUI As MyDataGridViewRow = Nothing
                                                 Dim currentItem As MyDataGridViewRow = Nothing

                                                 Try
                                                     boolBackgroundThreadWorking = True
                                                     Dim strChecksum, strFileName As String
                                                     Dim index As Integer = 1
                                                     Dim longFilesThatPassedVerification As Long = 0
                                                     Dim intFilesNotFound As Integer = 0
                                                     Dim intLineCounter As Integer = 0
                                                     Dim myStopWatch As Stopwatch = Stopwatch.StartNew

                                                     MyInvoke(Sub()
                                                                  btnRetestFailedFiles.Visible = False
                                                                  lblVerifyHashStatus.Visible = True
                                                                  verifyIndividualFilesAllFilesProgressBar.Visible = True
                                                                  VerifyHashProgressBar.Visible = True
                                                                  lblProcessingFileVerify.Visible = True
                                                                  lblVerifyHashStatus.Visible = True
                                                                  lblVerifyHashesTotalStatus.Visible = True
                                                                  lblVerifyHashesTotalStatus.Text = Nothing
                                                                  lblVerifyHashStatusProcessingFile.Text = Nothing
                                                              End Sub, Me)

                                                     SyncLock threadLockingObject
                                                         longAllReadBytes = 0
                                                         longAllBytes = 0
                                                     End SyncLock

                                                     MyInvoke(Sub()
                                                                  Text = strWindowTitle
                                                                  'If chkSortByFileSizeAfterLoadingHashFile.Checked Then ApplyFileSizeSortingToVerifyList()
                                                                  VerifyHashProgressBar.Value = 0
                                                                  ProgressForm.SetTaskbarProgressBarValue(0)
                                                                  lblVerifyHashStatusProcessingFile.Visible = True
                                                              End Sub, Me)

                                                     Dim strChecksumInFile As String = Nothing
                                                     Dim percentage, allBytesPercentage As Double
                                                     Dim subRoutine As ChecksumStatusUpdaterDelegate
                                                     Dim computeStopwatch As Stopwatch
                                                     Dim allTheHashes As AllTheHashes = Nothing
                                                     Dim strDisplayValidChecksumString As String = If(chkDisplayValidChecksumString.Checked, "Valid Checksum", "")
                                                     Dim intFileCount As Integer = 0
                                                     Dim exceptionObject As Exception = Nothing

                                                     For Each item As MyDataGridViewRow In verifyHashesListFiles.Rows
                                                         MyInvoke(Sub() itemOnGUI = verifyHashesListFiles.Rows(item.Index), Me)
                                                         If Not item.BoolValidHash Then
                                                             If IO.File.Exists(item.FileName) Then
                                                                 item.BoolFileExists = True
                                                                 item.FileSize = New IO.FileInfo(item.FileName).Length
                                                                 item.Cells(1).Value = FileSizeToHumanSize(item.FileSize)
                                                                 item.Cells(2).Value = ""
                                                                 item.Cells(3).Value = ""
                                                                 item.Cells(4).Value = strWaitingToBeProcessed
                                                                 item.MyColor = Color.FromKnownColor(KnownColor.Window)
                                                                 item.DefaultCellStyle.Padding = New Padding(0, 2, 0, 2)

                                                                 MyInvoke(Sub() UpdateDataGridViewRow(itemOnGUI, item), Me)

                                                                 longAllBytes += item.FileSize
                                                                 intFileCount += 1
                                                             Else
                                                                 item.BoolFileExists = False
                                                             End If
                                                         End If
                                                     Next

                                                     If chkSortByFileSizeAfterLoadingHashFile.Checked Then MyInvoke(Sub() SortLogsByFileSize(1, sortOrderForListFiles, listFiles), Me)
                                                     index = 1

                                                     For Each item As MyDataGridViewRow In verifyHashesListFiles.Rows
                                                         currentItem = item

                                                         MyInvoke(Sub()
                                                                      itemOnGUI = verifyHashesListFiles.Rows(item.Index)
                                                                      lblVerifyHashStatusProcessingFile.Text = GenerateProcessingFileString(index, intFileCount)
                                                                  End Sub, Me)

                                                         If Not item.BoolValidHash Then
                                                             strChecksum = item.Hash
                                                             strFileName = item.FileName

                                                             If IO.File.Exists(strFileName) Then
                                                                 subRoutine = New ChecksumStatusUpdaterDelegate(Sub(size As Long, totalBytesRead As Long)
                                                                                                                    Try
                                                                                                                        MyInvoke(Sub()
                                                                                                                                     percentage = If(totalBytesRead = 0 Or size = 0, 0, totalBytesRead / size * 100) ' This fixes a possible divide by zero exception.
                                                                                                                                     VerifyHashProgressBar.Value = percentage
                                                                                                                                     SyncLock threadLockingObject
                                                                                                                                         allBytesPercentage = If(longAllReadBytes = 0 Or longAllBytes = 0, 100, longAllReadBytes / longAllBytes * 100)
                                                                                                                                         lblVerifyHashesTotalStatus.Text = $"{FileSizeToHumanSize(longAllReadBytes)} of {FileSizeToHumanSize(longAllBytes)} ({MyRoundingFunction(allBytesPercentage, byteRoundPercentages)}%) has been processed."
                                                                                                                                         If chkShowPercentageInWindowTitleBar.Checked Then Text = $"{strWindowTitle} ({MyRoundingFunction(allBytesPercentage, byteRoundPercentages)}% Completed)"
                                                                                                                                     End SyncLock
                                                                                                                                     lblProcessingFileVerify.Text = $"{FileSizeToHumanSize(totalBytesRead)} of {FileSizeToHumanSize(size)} ({MyRoundingFunction(percentage, byteRoundPercentages)}%) has been processed."
                                                                                                                                     If chkShowFileProgressInFileList.Checked Then
                                                                                                                                         currentItem.Cells(4).Value = lblProcessingFileVerify.Text
                                                                                                                                         itemOnGUI.Cells(4).Value = currentItem.Cells(4).Value
                                                                                                                                     End If
                                                                                                                                     ProgressForm.SetTaskbarProgressBarValue(allBytesPercentage)
                                                                                                                                     verifyIndividualFilesAllFilesProgressBar.Value = allBytesPercentage
                                                                                                                                 End Sub, Me)
                                                                                                                    Catch ex As Exception
                                                                                                                    End Try
                                                                                                                End Sub)

                                                                 item.Cells(3).Value = strCurrentlyBeingProcessed

                                                                 MyInvoke(Sub() lblVerifyHashStatus.Text = $"Now processing file ""{New IO.FileInfo(strFileName).Name}"".", Me)
                                                                 MyInvoke(Sub() UpdateDataGridViewRow(itemOnGUI, item), verifyHashesListFiles)

                                                                 computeStopwatch = Stopwatch.StartNew

                                                                 If DoChecksumWithAttachedSubRoutine(strFileName, allTheHashes, subRoutine, exceptionObject) Then
                                                                     strChecksum = GetDataFromAllTheHashes(checksumTypeForChecksumCompareWindow, allTheHashes)
                                                                     item.AllTheHashes = allTheHashes

                                                                     If strChecksum.Equals(item.Hash, StringComparison.OrdinalIgnoreCase) Then
                                                                         item.MyColor = validColor
                                                                         item.Cells(2).Value = "Valid Checksum"
                                                                         item.ComputeTime = computeStopwatch.Elapsed
                                                                         item.Cells(3).Value = TimespanToHMS(item.ComputeTime)
                                                                         item.Cells(4).Value = strDisplayValidChecksumString
                                                                         longFilesThatPassedVerification += 1
                                                                         item.BoolValidHash = True
                                                                     Else
                                                                         item.MyColor = notValidColor
                                                                         item.Cells(2).Value = "Incorrect Checksum"
                                                                         item.ComputeTime = computeStopwatch.Elapsed
                                                                         item.Cells(3).Value = TimespanToHMS(item.ComputeTime)
                                                                         item.Cells(4).Value = If(chkDisplayHashesInUpperCase.Checked, GetDataFromAllTheHashes(checksumTypeForChecksumCompareWindow, allTheHashes).ToUpper, GetDataFromAllTheHashes(checksumTypeForChecksumCompareWindow, allTheHashes).ToLower)
                                                                         item.BoolValidHash = False
                                                                     End If

                                                                     item.BoolExceptionOccurred = False
                                                                     item.StrCrashData = Nothing
                                                                 Else
                                                                     item.MyColor = fileNotFoundColor
                                                                     item.Cells(2).Value = If(exceptionObject.GetType IsNot Nothing, $"(An error occurred while calculating checksum, {exceptionObject.GetType})", "(An error occurred while calculating checksum, unknown exception type)")
                                                                     item.Cells(4).Value = If(exceptionObject.GetType IsNot Nothing, $"(An error occurred while calculating checksum, {exceptionObject.GetType})", "(An error occurred while calculating checksum, unknown exception type)")
                                                                     item.BoolExceptionOccurred = True
                                                                     item.StrCrashData = $"{exceptionObject.Message}{vbCrLf}{exceptionObject.StackTrace}"
                                                                     item.BoolValidHash = False

                                                                     SyncLock threadLockingObject
                                                                         longAllBytes -= item.FileSize
                                                                     End SyncLock
                                                                 End If

                                                                 subRoutine = Nothing

                                                                 MyInvoke(Sub() UpdateDataGridViewRow(itemOnGUI, item), verifyHashesListFiles)

                                                                 index += 1
                                                             End If
                                                         Else
                                                             item.BoolValidHash = False
                                                         End If
                                                     Next

                                                     MyInvoke(Sub()
                                                                  For Each item As MyDataGridViewRow In verifyHashesListFiles.Rows
                                                                      If item.BoolFileExists Then item.MyColor = item.MyColor
                                                                  Next

                                                                  btnTransferToHashIndividualFilesTab.Enabled = True
                                                                  lblVerifyHashStatusProcessingFile.Visible = False
                                                                  lblVerifyHashesTotalStatus.Visible = False
                                                                  verifyIndividualFilesAllFilesProgressBar.Visible = False
                                                                  lblVerifyHashStatus.Visible = False
                                                                  lblProcessingFileVerify.Visible = False
                                                                  VerifyHashProgressBar.Value = 0
                                                                  VerifyHashProgressBar.Visible = False
                                                                  ProgressForm.SetTaskbarProgressBarValue(0)
                                                                  Text = strWindowTitle
                                                                  verifyHashesListFiles.Size = New Size(verifyHashesListFiles.Size.Width, verifyHashesListFiles.Size.Height + 72)

                                                                  Dim sbMessageBoxText As New Text.StringBuilder
                                                                  Dim intFilesThatDidNotPassVerification As Integer = 0

                                                                  If intFilesNotFound = 0 Then
                                                                      If longFilesThatPassedVerification = intFileCount Then
                                                                          sbMessageBoxText.AppendLine("Processing of hash file complete. All files have passed verification.")
                                                                      Else
                                                                          intFilesThatDidNotPassVerification = intFileCount - longFilesThatPassedVerification
                                                                          If intFilesThatDidNotPassVerification <> 0 Then btnRetestFailedFiles.Visible = True
                                                                          sbMessageBoxText.AppendLine($"Processing of hash file complete. {MyToString(longFilesThatPassedVerification) } out of {MyToString(intFileCount) } {If(intFileCount = 1, "file", "files") } passed verification, {MyToString(intFilesThatDidNotPassVerification) } {If(intFilesThatDidNotPassVerification = 1, "file", "files") } didn't pass verification.")
                                                                      End If
                                                                  Else
                                                                      sbMessageBoxText.AppendLine("Processing of hash file complete.")
                                                                      sbMessageBoxText.AppendLine()
                                                                      btnRetestFailedFiles.Visible = True

                                                                      Dim intTotalFiles As Integer = intFileCount - intFilesNotFound
                                                                      If longFilesThatPassedVerification = intTotalFiles Then
                                                                          sbMessageBoxText.AppendLine($"All files have passed verification. Unfortunately, {MyToString(intFilesNotFound)} {If(intFilesNotFound = 1, "file", "files")} were not found.")
                                                                      Else
                                                                          intFilesThatDidNotPassVerification = intTotalFiles - longFilesThatPassedVerification
                                                                          If intFilesThatDidNotPassVerification <> 0 Then btnRetestFailedFiles.Visible = True
                                                                          sbMessageBoxText.AppendLine($"Not all of the files passed verification, only {MyToString(longFilesThatPassedVerification)} out of {MyToString(intTotalFiles)} {If(intTotalFiles = 1, "file", "files")} passed verification, Unfortunately, {MyToString(intFilesThatDidNotPassVerification)} {If(intFilesThatDidNotPassVerification = 1, "file", "files")} didn't pass verification and {MyToString(intFilesNotFound)} {If(intFilesNotFound = 1, "file", "files")} were not found.")
                                                                      End If
                                                                  End If

                                                                  sbMessageBoxText.AppendLine()
                                                                  sbMessageBoxText.AppendLine($"Processing completed in {TimespanToHMS(myStopWatch.Elapsed)}.")

                                                                  MsgBox(sbMessageBoxText.ToString.Trim, MsgBoxStyle.Information, strMessageBoxTitleText)
                                                              End Sub, Me)

                                                     boolBackgroundThreadWorking = False
                                                     workingThread = Nothing
                                                 Catch ex As Threading.ThreadAbortException
                                                     MyInvoke(Sub()
                                                                  If Not boolClosingWindow Then
                                                                      lblVerifyHashStatusProcessingFile.Visible = False
                                                                      verifyIndividualFilesAllFilesProgressBar.Visible = False
                                                                      lblVerifyHashStatus.Visible = False
                                                                      lblVerifyHashesTotalStatus.Visible = False
                                                                      lblProcessingFileVerify.Visible = False
                                                                      VerifyHashProgressBar.Value = 0
                                                                      VerifyHashProgressBar.Visible = False
                                                                      ProgressForm.SetTaskbarProgressBarValue(0)
                                                                      verifyHashesListFiles.Rows.Clear()
                                                                      Text = strWindowTitle
                                                                      verifyHashesListFiles.Size = New Size(verifyHashesListFiles.Size.Width, verifyHashesListFiles.Size.Height + 72)
                                                                      lblVerifyFileNameLabel.Text = "File Name: (None Selected for Processing)"
                                                                  End If

                                                                  boolBackgroundThreadWorking = False
                                                                  workingThread = Nothing
                                                                  If Not boolClosingWindow Then MsgBox("Processing aborted.", MsgBoxStyle.Information, strMessageBoxTitleText)
                                                              End Sub, Me)
                                                 Finally
                                                     itemOnGUI = Nothing
                                                     intCurrentlyActiveTab = TabNumberNull
                                                     SyncLock threadLockingObject
                                                         longAllReadBytes = 0
                                                         longAllBytes = 0
                                                     End SyncLock

                                                     MyInvoke(Sub()
                                                                  If Not boolClosingWindow Then
                                                                      btnTransferToHashIndividualFilesTab.Enabled = True
                                                                      btnOpenExistingHashFile.Text = "Open Hash File"
                                                                      ProgressForm.SetTaskbarProgressBarValue(0)
                                                                      verifyIndividualFilesAllFilesProgressBar.Value = 0
                                                                  End If
                                                              End Sub, Me)
                                                 End Try
                                             End Sub) With {
            .Priority = GetThreadPriority(),
            .Name = "Verify Hash File Working Thread",
            .IsBackground = True
        }
        workingThread.Start()
    End Sub

    Private Sub DefaultHashType_SelectedIndexChanged(sender As Object, e As EventArgs) Handles defaultHashType.SelectedIndexChanged
        If boolDoneLoading Then
            SetDefaultHashTypeGUIElementOptions()
            My.Settings.defaultHash = Byte.Parse(defaultHashType.SelectedIndex)
        End If
    End Sub

    Private Sub SetDefaultHashTypeGUIElementOptions()
        Select Case defaultHashType.SelectedIndex
            Case 0
                radioMD5.Checked = True
                colChecksum.HeaderText = strColumnTitleChecksumMD5
            Case 1
                radioSHA1.Checked = True
                colChecksum.HeaderText = strColumnTitleChecksumSHA160
            Case 2
                radioSHA256.Checked = True
                colChecksum.HeaderText = strColumnTitleChecksumSHA256
            Case 3
                radioSHA384.Checked = True
                colChecksum.HeaderText = strColumnTitleChecksumSHA384
            Case 4
                radioSHA512.Checked = True
                colChecksum.HeaderText = strColumnTitleChecksumSHA512
        End Select
    End Sub

    Private Sub ChkShowFileProgressInFileList_Click(sender As Object, e As EventArgs) Handles chkShowFileProgressInFileList.Click
        My.Settings.boolShowFileProgressInFileList = chkShowFileProgressInFileList.Checked
        If Not chkShowFileProgressInFileList.Checked Then Text = strWindowTitle
    End Sub

    Private Sub ChkIncludeEntryCountInFileNameHeader_Click(sender As Object, e As EventArgs) Handles ChkIncludeEntryCountInFileNameHeader.Click
        My.Settings.boolIncludeEntryCountInFileNameHeader = ChkIncludeEntryCountInFileNameHeader.Checked
    End Sub

    Private Sub ChkComputeHashesOnCompareFilesTabEvenWithDifferentFileSizes_Click(sender As Object, e As EventArgs) Handles ChkComputeHashesOnCompareFilesTabEvenWithDifferentFileSizes.Click
        My.Settings.boolComputeHashesOnCompareFilesTabEvenWithDifferentFileSizes = ChkComputeHashesOnCompareFilesTabEvenWithDifferentFileSizes.Checked
    End Sub

    Private Sub BtnSaveSettingsToFile_Click(sender As Object, e As EventArgs) Handles BtnSaveSettingsToFile.Click
        Using SaveFileDialogBox As New SaveFileDialog()
            CallSaveColumnOrders()

            SaveFileDialogBox.Title = "Save Settings to JSON File"
            SaveFileDialogBox.Filter = "JSON File|*.json"

            If SaveFileDialogBox.ShowDialog = DialogResult.OK Then
                Try
                    SaveApplicationSettingsToFile(SaveFileDialogBox.FileName)
                    If MsgBox("Application settings have been saved to disk. Do you want to open Windows Explorer to the location of the file?", MsgBoxStyle.Question + MsgBoxStyle.YesNo + MsgBoxStyle.DefaultButton2, strMessageBoxTitleText) = MsgBoxResult.Yes Then SelectFileInWindowsExplorer(SaveFileDialogBox.FileName)
                Catch ex As Exception
                    MsgBox("There was an issue saving your exported settings to disk, export failed.", MsgBoxStyle.Critical, strMessageBoxTitleText)
                End Try
            End If
        End Using
    End Sub

    Private Sub BtnLoadSettingsFromFile_Click(sender As Object, e As EventArgs) Handles BtnLoadSettingsFromFile.Click
        Using OpenFileDialogBox As New OpenFileDialog()
            OpenFileDialogBox.Title = "Open Settings JSON File"
            OpenFileDialogBox.Filter = "JSON File|*.json"

            If OpenFileDialogBox.ShowDialog = DialogResult.OK AndAlso LoadApplicationSettingsFromFile(OpenFileDialogBox.FileName, "Hasher") Then
                My.Settings.Save()
                MsgBox("Hasher will now close and restart itself for the imported settings to take effect.", MsgBoxStyle.Information, strMessageBoxTitleText)
                Process.Start(strEXEPath)
                Process.GetCurrentProcess.Kill()
            End If
        End Using
    End Sub

    Private Sub ChkClearBeforeTransferringFromVerifyToHash_Click(sender As Object, e As EventArgs) Handles chkClearBeforeTransferringFromVerifyToHash.Click
        My.Settings.boolClearBeforeTransferringFromVerifyToHash = chkClearBeforeTransferringFromVerifyToHash.Checked
    End Sub

    Private Sub SetClipboardDataFromGlobalAllTheHashes(checksum As HashAlgorithmName)
        Try
            Dim boolResult As Boolean = False

            Select Case checksum
                Case HashAlgorithmName.MD5
                    boolResult = CopyTextToWindowsClipboard(If(chkDisplayHashesInUpperCase.Checked, globalAllTheHashes.Md5.ToUpper, globalAllTheHashes.Md5.ToLower))
                Case HashAlgorithmName.SHA1
                    boolResult = CopyTextToWindowsClipboard(If(chkDisplayHashesInUpperCase.Checked, globalAllTheHashes.Sha160.ToUpper, globalAllTheHashes.Sha160.ToLower))
                Case HashAlgorithmName.SHA256
                    boolResult = CopyTextToWindowsClipboard(If(chkDisplayHashesInUpperCase.Checked, globalAllTheHashes.Sha256.ToUpper, globalAllTheHashes.Sha256.ToLower))
                Case HashAlgorithmName.SHA384
                    boolResult = CopyTextToWindowsClipboard(If(chkDisplayHashesInUpperCase.Checked, globalAllTheHashes.Sha384.ToUpper, globalAllTheHashes.Sha384.ToLower))
                Case HashAlgorithmName.SHA512
                    boolResult = CopyTextToWindowsClipboard(If(chkDisplayHashesInUpperCase.Checked, globalAllTheHashes.Sha512.ToUpper, globalAllTheHashes.Sha512.ToLower))
            End Select

            globalAllTheHashes = Nothing
            If boolResult Then MsgBox("Checksum copied to Windows Clipboard.", MsgBoxStyle.Information, strMessageBoxTitleText)
        Catch ex As Exception
            MsgBox("Unable to copy the checksum to the Windows Clipboard.", MsgBoxStyle.Critical, strMessageBoxTitleText)
        End Try
    End Sub

    Private Sub ListFilesContextMenuMD5_Click(sender As Object, e As EventArgs) Handles listFilesContextMenuMD5.Click
        SetClipboardDataFromGlobalAllTheHashes(HashAlgorithmName.MD5)
    End Sub

    Private Sub ListFilesContextMenuSHA160_Click(sender As Object, e As EventArgs) Handles listFilesContextMenuSHA160.Click
        SetClipboardDataFromGlobalAllTheHashes(HashAlgorithmName.SHA1)
    End Sub

    Private Sub ListFilesContextMenuSHA256_Click(sender As Object, e As EventArgs) Handles listFilesContextMenuSHA256.Click
        SetClipboardDataFromGlobalAllTheHashes(HashAlgorithmName.SHA256)
    End Sub

    Private Sub ListFilesContextMenuSHA384_Click(sender As Object, e As EventArgs) Handles listFilesContextMenuSHA384.Click
        SetClipboardDataFromGlobalAllTheHashes(HashAlgorithmName.SHA384)
    End Sub

    Private Sub ListFilesContextMenuSHA512_Click(sender As Object, e As EventArgs) Handles listFilesContextMenuSHA512.Click
        SetClipboardDataFromGlobalAllTheHashes(HashAlgorithmName.SHA512)
    End Sub

    Private Sub VerifyListFilesContextMenuMD5_Click(sender As Object, e As EventArgs) Handles verifyListFilesContextMenuMD5.Click
        SetClipboardDataFromGlobalAllTheHashes(HashAlgorithmName.MD5)
    End Sub

    Private Sub VerifyListFilesContextMenuSHA160_Click(sender As Object, e As EventArgs) Handles verifyListFilesContextMenuSHA160.Click
        SetClipboardDataFromGlobalAllTheHashes(HashAlgorithmName.SHA1)
    End Sub

    Private Sub VerifyListFilesContextMenuSHA256_Click(sender As Object, e As EventArgs) Handles verifyListFilesContextMenuSHA256.Click
        SetClipboardDataFromGlobalAllTheHashes(HashAlgorithmName.SHA256)
    End Sub

    Private Sub VerifyListFilesContextMenuSHA384_Click(sender As Object, e As EventArgs) Handles verifyListFilesContextMenuSHA384.Click
        SetClipboardDataFromGlobalAllTheHashes(HashAlgorithmName.SHA384)
    End Sub

    Private Sub VerifyListFilesContextMenuSHA512_Click(sender As Object, e As EventArgs) Handles verifyListFilesContextMenuSHA512.Click
        SetClipboardDataFromGlobalAllTheHashes(HashAlgorithmName.SHA512)
    End Sub

    Private Function SaveColumnOrders(columns As DataGridViewColumnCollection) As Specialized.StringCollection
        Try
            Dim SpecializedStringCollection As New Specialized.StringCollection

            For Each column As DataGridViewTextBoxColumn In columns
                SpecializedStringCollection.Add(Newtonsoft.Json.JsonConvert.SerializeObject(New ColumnOrder With {.ColumnName = column.Name, .ColumnIndex = column.DisplayIndex}))
            Next

            Return SpecializedStringCollection
        Catch ex As Exception
            Return New Specialized.StringCollection
        End Try
    End Function

    Private Sub BtnCheckHaveIBeenPwned_Click(sender As Object, e As EventArgs) Handles btnCheckHaveIBeenPwned.Click
        ' This whole routine has been documented so that users who aren't even programers can see that there's nothing nefarious going on in this routine.

        btnCheckHaveIBeenPwned.Enabled = False ' Disable the button on the GUI.

        ' Do all of this work in a background thread so as to keep the GUI active even while work is being done in this routine.
        Threading.ThreadPool.QueueUserWorkItem(Sub()
                                                   Dim strFullSHA1String As String = GetSHA160HashOfString(txtTextToHash.Text).ToUpper ' First hash the String.
                                                   Dim strSHA1ToSearchWith As String = strFullSHA1String.Substring(0, 5) ' Take out only what we want, the first five characters. That's all we send to the server.
                                                   Dim strWebData As String = Nothing ' Prepare a variable to get data from the web.

                                                   Dim httpHelper As HttpHelper = checkForUpdates.CheckForUpdatesClass.CreateNewHTTPHelperObject() ' Create an HTTPHelper Class instance.

                                                   ' Call HaveIBeenPwned.com's Password Checking API. Note how we only send the first five characters
                                                   ' to the server, you can see that by our use of the strSHA1ToSearchWith variable created above.
                                                   If httpHelper.GetWebData($"https://api.pwnedpasswords.com/range/{strSHA1ToSearchWith}", strWebData, False) Then
                                                       ' OK, we have a valid HTTP response, now let's do something with it.

                                                       ' We do some parsing of the incoming data, we do that by searching for the full SHA1 String in the web
                                                       ' data but this is all being done client-side; again none of this is happening on the server side.
                                                       Dim regexObject As New Text.RegularExpressions.Regex("(?:" & strFullSHA1String.Substring(5) & ")+:([0-9]+)")
                                                       Dim regexMatchResults As Text.RegularExpressions.MatchCollection = regexObject.Matches(strWebData) ' Use the above Regex object to do some searching.

                                                       ' Do we have some results?
                                                       If regexMatchResults.Count > 0 Then
                                                           ' Yes, that's not good.

                                                           Dim intTimes As Integer = 0 ' Create a variable to use to store the parsed Integer.

                                                           ' Try to parse the Integer that's a String into an actual Integer.
                                                           If Integer.TryParse(regexMatchResults.Item(0).Groups(1).ToString, intTimes) Then
                                                               ' Parsing worked, let's plug it into the message box text.
                                                               MyInvoke(Sub() MsgBox($"OH NO!{DoubleCRLF}Your password has been found {MyToString(intTimes)} {If(intTimes = 1, "time", "times")} in the HaveIBeenPwned.com database, consider changing your password on any accounts that use this password.", MsgBoxStyle.Critical, strMessageBoxTitleText), Me)
                                                           Else
                                                               ' Oops, parsing failed; let's just use a generic message instead of a custom one.
                                                               MyInvoke(Sub() MsgBox($"OH NO!{DoubleCRLF}Your password has been found in the HaveIBeenPwned.com database, consider changing your password on any accounts that use this password.", MsgBoxStyle.Critical, strMessageBoxTitleText), Me)
                                                           End If
                                                       Else
                                                           ' Nope, the password hasn't been breached.
                                                           MyInvoke(Sub() MsgBox($"Your password has not been found in the HaveIBeenPwned.com database.{DoubleCRLF}Congratulations!", MsgBoxStyle.Information, strMessageBoxTitleText), Me)
                                                       End If
                                                   Else
                                                       ' Something happened during our API call, give the user an error message.
                                                       MyInvoke(Sub()
                                                                    btnCheckHaveIBeenPwned.Enabled = True ' Re-enable the button on the GUI.
                                                                    MsgBox("There was an error calling the HaveIBeenPwned.com API. Please try again later.", MsgBoxStyle.Critical, strMessageBoxTitleText)
                                                                End Sub, Me)
                                                   End If
                                               End Sub)

        ' End of routine.
    End Sub

    Private Sub BtnRemoveFileAssociations_Click(sender As Object, e As EventArgs) Handles btnRemoveFileAssociations.Click
        Try
            FileAssociation.DeleteFileAssociation()
            FileAssociation.DeleteAssociationWithAllFiles()
            btnAssociate.Enabled = True
            btnAddHasherToAllFiles.Enabled = True
            btnRemoveFileAssociations.Enabled = False
            MsgBox("File associations have been removed successfully.", MsgBoxStyle.Information, strMessageBoxTitleText)
        Catch ex As Exception
            MsgBox($"Something went wrong while removing file associations.{DoubleCRLF}{ex.Message} -- {ex.StackTrace}", MsgBoxStyle.Critical, strMessageBoxTitleText)
        End Try
    End Sub

    Private Sub BtnRemoveSystemLevelFileAssociations_Click(sender As Object, e As EventArgs) Handles btnRemoveSystemLevelFileAssociations.Click
        Dim boolSuccessful As Boolean = False

        If AreWeAnAdministrator() Then
            FileAssociation.DeleteSystemLevelFileAssociation()
            FileAssociation.DeleteSystemLevelAssociationWithAllFiles()
            boolSuccessful = True
        Else
            Try
                Dim startInfo As New ProcessStartInfo With {
                    .FileName = strEXEPath,
                    .Arguments = "-removesystemlevelassociations",
                    .Verb = "runas"
                }
                Dim process As Process = Process.Start(startInfo)
                process.WaitForExit()
                boolSuccessful = True
            Catch ex As Win32Exception
                MsgBox($"Failed to elevate process.{DoubleCRLF}Please try again but make sure you respond with a ""Yes"" to the UAC Prompt.", MsgBoxStyle.Critical, strMessageBoxTitleText)
            End Try
        End If

        If boolSuccessful Then MsgBox("System-level file associations have been removed successfully.", MsgBoxStyle.Information, strMessageBoxTitleText)
    End Sub

    Private Sub LoadColumnOrders(ByRef columns As DataGridViewColumnCollection, ByRef specializedStringCollection As Specialized.StringCollection)
        Dim columnOrder As ColumnOrder
        Dim jsonSerializerSettings As New Newtonsoft.Json.JsonSerializerSettings With {.MissingMemberHandling = Newtonsoft.Json.MissingMemberHandling.Error}

        If specializedStringCollection IsNot Nothing AndAlso specializedStringCollection.Count <> 0 Then
            Try
                For Each item As String In specializedStringCollection
                    columnOrder = Newtonsoft.Json.JsonConvert.DeserializeObject(Of ColumnOrder)(item, jsonSerializerSettings)
                    columns(columnOrder.ColumnName).DisplayIndex = columnOrder.ColumnIndex
                Next
            Catch ex As Newtonsoft.Json.JsonSerializationException
                specializedStringCollection = Nothing
            End Try
        End If
    End Sub

    Private Sub Form1_Shown(sender As Object, e As EventArgs) Handles Me.Shown
        ' This is to work around a bug in Windows 10 in which the Start Menu doesn't close when launching a .NET program from the Start Menu.
        Try
            If Environment.OSVersion.Version.Major = 10 Or Environment.OSVersion.Version.Major = 11 Then SendKeys.Send("{ESC}")
        Catch ex As Exception
        End Try
    End Sub

    Private Sub ListFiles_DoubleClick(sender As Object, e As EventArgs)
        ShowExceptionOrChecksumViewerWindow(DirectCast(listFiles.SelectedRows(0), MyDataGridViewRow))
    End Sub

    Private Sub VerifyHashesListFiles_DoubleClick(sender As Object, e As EventArgs)
        ShowExceptionOrChecksumViewerWindow(DirectCast(verifyHashesListFiles.SelectedRows(0), MyDataGridViewRow))
    End Sub

    Private Sub ShowExceptionOrChecksumViewerWindow(selectedItem As MyDataGridViewRow)
        If selectedItem.BoolExceptionOccurred Then
            Using exceptionViewerWindow As New Exception_Viewer
                With exceptionViewerWindow
                    .TxtExceptionData.Text = selectedItem.StrCrashData
                    .StartPosition = FormStartPosition.CenterParent
                    .Size = My.Settings.exceptionViewerWindowSize
                    .Icon = Icon
                End With

                exceptionViewerWindow.ShowDialog()
            End Using
        Else
            Using checksumViewerWindowInstance As New Checksum_Viewer
                With checksumViewerWindowInstance
                    .lblFileName.Text = $"File Name: {selectedItem.FileName}"
                    .Icon = Icon
                    .StartPosition = FormStartPosition.CenterParent

                    Dim checksumsToDisplay As New List(Of ListViewItem) From {
                        CreateChecksumViewerListViewItem("MD5", If(chkDisplayHashesInUpperCase.Checked, selectedItem.AllTheHashes.Md5.ToUpper, selectedItem.AllTheHashes.Md5.ToLower)),
                        CreateChecksumViewerListViewItem("SHA1", If(chkDisplayHashesInUpperCase.Checked, selectedItem.AllTheHashes.Sha160.ToUpper, selectedItem.AllTheHashes.Sha160.ToLower)),
                        CreateChecksumViewerListViewItem("SHA256", If(chkDisplayHashesInUpperCase.Checked, selectedItem.AllTheHashes.Sha256.ToUpper, selectedItem.AllTheHashes.Sha256.ToLower)),
                        CreateChecksumViewerListViewItem("SHA384", If(chkDisplayHashesInUpperCase.Checked, selectedItem.AllTheHashes.Sha384.ToUpper, selectedItem.AllTheHashes.Sha384.ToLower)),
                        CreateChecksumViewerListViewItem("SHA512", If(chkDisplayHashesInUpperCase.Checked, selectedItem.AllTheHashes.Sha512.ToUpper, selectedItem.AllTheHashes.Sha512.ToLower))
                    }

                    .checksums.Items.AddRange(checksumsToDisplay.ToArray)
                End With

                checksumViewerWindowInstance.ShowDialog()
            End Using
        End If
    End Sub

    Private Function CreateChecksumViewerListViewItem(strType As String, strChecksum As String) As ListViewItem
        Dim itemToBeAdded As New ListViewItem(strType)
        With itemToBeAdded
            .SubItems.Add(strChecksum)
        End With
        Return itemToBeAdded
    End Function

    Private Sub ChkHideCheckForUpdatesButton_Click(sender As Object, e As EventArgs) Handles chkHideCheckForUpdatesButton.Click
        My.Settings.boolHideCheckForUpdatesButton = chkHideCheckForUpdatesButton.Checked
        btnCheckForUpdates.Visible = Not chkHideCheckForUpdatesButton.Checked Or Not chkCheckForUpdates.Checked
    End Sub

    Private Sub ListFiles_ColumnHeaderMouseClick(sender As Object, e As DataGridViewCellMouseEventArgs) Handles listFiles.ColumnHeaderMouseClick
        ' Disable user sorting
        listFiles.AllowUserToOrderColumns = False

        Dim column As DataGridViewColumn = listFiles.Columns(e.ColumnIndex)

        If sortOrderForListFiles = SortOrder.Descending Then
            sortOrderForListFiles = SortOrder.Ascending
        ElseIf sortOrderForListFiles = SortOrder.Ascending Then
            sortOrderForListFiles = SortOrder.Descending
        Else
            sortOrderForListFiles = SortOrder.Ascending
        End If

        colFileName.HeaderCell.SortGlyphDirection = SortOrder.None
        colFileSize.HeaderCell.SortGlyphDirection = SortOrder.None
        colChecksum.HeaderCell.SortGlyphDirection = SortOrder.None
        colComputeTime.HeaderCell.SortGlyphDirection = SortOrder.None

        listFiles.Columns(e.ColumnIndex).HeaderCell.SortGlyphDirection = sortOrderForListFiles

        SortLogsByFileSize(column.Index, sortOrderForListFiles, listFiles)
    End Sub

    Private Sub VerifyHashesListFiles_ColumnHeaderMouseClick(sender As Object, e As DataGridViewCellMouseEventArgs) Handles verifyHashesListFiles.ColumnHeaderMouseClick
        ' Disable user sorting
        verifyHashesListFiles.AllowUserToOrderColumns = False

        Dim column As DataGridViewColumn = verifyHashesListFiles.Columns(e.ColumnIndex)

        If sortOrderForVerifyHashesListFiles = SortOrder.Descending Then
            sortOrderForVerifyHashesListFiles = SortOrder.Ascending
        ElseIf sortOrderForVerifyHashesListFiles = SortOrder.Ascending Then
            sortOrderForVerifyHashesListFiles = SortOrder.Descending
        Else
            sortOrderForVerifyHashesListFiles = SortOrder.Ascending
        End If

        colFile.HeaderCell.SortGlyphDirection = SortOrder.None
        colFileSize2.HeaderCell.SortGlyphDirection = SortOrder.None
        colResults.HeaderCell.SortGlyphDirection = SortOrder.None
        colComputeTime2.HeaderCell.SortGlyphDirection = SortOrder.None
        colNewHash.HeaderCell.SortGlyphDirection = SortOrder.None

        verifyHashesListFiles.Columns(e.ColumnIndex).HeaderCell.SortGlyphDirection = sortOrderForVerifyHashesListFiles

        SortLogsByFileSize(column.Index, sortOrderForVerifyHashesListFiles, verifyHashesListFiles)
    End Sub

    Private Sub SortLogsByFileSize(columnIndex As Integer, order As SortOrder, ByRef DataGridObject As DataGridView)
        DataGridObject.AllowUserToOrderColumns = False
        DataGridObject.Enabled = False

        Dim comparer As New DataGridViewComparer(columnIndex, order)
        Dim rows As MyDataGridViewRow() = DataGridObject.Rows.Cast(Of DataGridViewRow)().OfType(Of MyDataGridViewRow)().ToArray()

        Array.Sort(rows, Function(row1 As MyDataGridViewRow, row2 As MyDataGridViewRow) comparer.Compare(row1, row2))

        DataGridObject.Rows.Clear()
        DataGridObject.Rows.AddRange(rows)

        DataGridObject.Enabled = True
        DataGridObject.AllowUserToOrderColumns = True
    End Sub

    Private Sub TxtHashResults_MouseDown(sender As Object, e As MouseEventArgs) Handles txtHashResults.MouseDown
        If e.Button = MouseButtons.Right Then
            Dim currentMouseOverRow As Integer = txtHashResults.HitTest(e.X, e.Y).RowIndex

            If currentMouseOverRow >= 0 Then
                If txtHashResults.SelectedRows.Count <= 1 Then
                    txtHashResults.ClearSelection()
                    txtHashResults.Rows(currentMouseOverRow).Selected = True
                End If
            End If
        End If
    End Sub
End Class