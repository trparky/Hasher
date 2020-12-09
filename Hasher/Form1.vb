Imports System.IO.Pipes

Public Class Form1
    Private Const strWaitingToBeProcessed As String = "Waiting to be processed..."
    Private Const strCurrentlyBeingProcessed As String = "Currently being processed... Please wait."
    Private Const strNoBackgroundProcesses As String = "(No Background Processes)"
#If DEBUG Then
    Private Const strWindowTitle As String = "Hasher (Debug Build)"
#Else
    Private Const strWindowTitle As String = "Hasher"
#End If

    Private Const strMessageBoxTitleText As String = "Hasher"
    Private intBufferSize As Integer = My.Settings.shortBufferSize * 1024 * 1024
    Private strLastDirectoryWorkedOn As String
    Private filesInListFiles As New Specialized.StringCollection
    Private boolBackgroundThreadWorking As Boolean = False
    Private workingThread As Threading.Thread
    Private boolClosingWindow As Boolean = False
    Private m_SortingColumn1, m_SortingColumn2 As ColumnHeader
    Private boolDoneLoading As Boolean = False
    Private Property PipeServer As NamedPipeServerStream = Nothing
    Private ReadOnly strNamedPipeServerName As String = "hasher_" & GetHashOfString(Environment.UserName, ChecksumType.sha256).Substring(0, 10)
    Private Const strPayPal As String = "https://www.paypal.com/cgi-bin/webscr?cmd=_donations&business=HQL3AC96XKM42&lc=US&no_note=1&no_shipping=1&rm=1&return=http%3a%2f%2fwww%2etoms%2dworld%2eorg%2fblog%2fthank%2dyou%2dfor%2dyour%2ddonation&currency_code=USD&bn=PP%2dDonationsBF%3abtn_donateCC_LG%2egif%3aNonHosted"
    Private boolDidWePerformAPreviousHash As Boolean = False
    Private validColor, notValidColor, fileNotFoundColor As Color
    Private intCurrentlyActiveTab As Integer = -1
    Private compareFilesAllTheHashes1 As AllTheHashes = Nothing
    Private compareFilesAllTheHashes2 As AllTheHashes = Nothing
    Private hashTextAllTheHashes As AllTheHashes = Nothing
    Private checksumTypeForChecksumCompareWindow As ChecksumType
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

    Private ReadOnly hashLineParser As New Text.RegularExpressions.Regex("([a-zA-Z0-9]*) \*(.*)", System.Text.RegularExpressions.RegexOptions.Compiled)
    Private ReadOnly hashLineFilePathChecker As New Text.RegularExpressions.Regex("\A[A-Za-z]{1}:.*\Z", System.Text.RegularExpressions.RegexOptions.Compiled)

    Private Function GenerateProcessingFileString(intCurrentFile As Integer, intTotalFiles As Integer) As String
        Return String.Format("Processing file {0} of {1} {2}.",
                             MyToString(intCurrentFile),
                             MyToString(intTotalFiles),
                             If(intTotalFiles = 1, "file", "files")
               )
    End Function

    ''' <summary>
    ''' This function works very similar to the Invoke function that's already built into .NET. The only difference
    ''' is that this function checks to see if an invoke is required and only invokes the passed routine on the
    ''' main thread if it's required. If not, the passed routine is executed on the thread that the call
    ''' originated from. Also, if the program is closing the function aborts itself so as to prevent
    ''' System.InvalidOperationException upon program close.
    ''' </summary>
    ''' <param name="input"></param>
    Private Sub MyInvoke(input As [Delegate])
        If boolClosingWindow Then Exit Sub
        If InvokeRequired() Then
            Invoke(input)
        Else
            input.DynamicInvoke()
        End If
    End Sub

    Private Function GetListViewItems(ByVal lstview As ListView) As ListView.ListViewItemCollection
        Dim tempListViewItemCollection As ListView.ListViewItemCollection = New ListView.ListViewItemCollection(New ListView())

        If Not lstview.InvokeRequired() Then
            For Each item As MyListViewItem In lstview.Items
                tempListViewItemCollection.Add(CType(item.Clone(), MyListViewItem))
            Next

            Return tempListViewItemCollection
        Else
            Return CType(Invoke(New Func(Of ListView.ListViewItemCollection)(Function() GetListViewItems(lstview))), ListView.ListViewItemCollection)
        End If
    End Function

    Private Sub UpdateListViewItem(ByRef itemOnGUI As MyListViewItem, ByRef item As MyListViewItem, Optional boolForceUpdateColor As Boolean = False)
        With itemOnGUI
            For i As Short = 1 To item.SubItems.Count - 1
                .SubItems(i) = item.SubItems(i)
            Next

            .FileSize = item.FileSize
            .Hash = item.Hash
            .FileName = item.FileName
            .Color = item.Color
            .BoolFileExists = item.BoolFileExists
            .ComputeTime = item.ComputeTime
            .AllTheHashes = item.AllTheHashes
            .BoolValidHash = item.BoolValidHash
            If boolForceUpdateColor Then .BackColor = item.Color
        End With
    End Sub

    Private Function MyToString(input As Integer) As String
        Return If(chkUseCommasInNumbers.Checked, input.ToString("N0"), input.ToString)
    End Function

    Private Function MyToString(input As Long) As String
        Return If(chkUseCommasInNumbers.Checked, input.ToString("N0"), input.ToString)
    End Function

    Function DoChecksumWithAttachedSubRoutine(strFile As String, ByRef allTheHashes As AllTheHashes, subRoutine As [Delegate]) As Boolean
        Try
            If IO.File.Exists(strFile) Then
                Dim checksums As New Checksums(subRoutine)
                allTheHashes = checksums.PerformFileHash(strFile, intBufferSize)
                Return True
            Else
                Return False
            End If

            Return False
        Catch ex As Exception
            Return False
        End Try
    End Function

    Private Sub UpdateFilesListCountHeader(Optional boolIncludeSelectedItemCount As Boolean = False)
        MyInvoke(Sub()
                     If boolIncludeSelectedItemCount Then
                         lblHashIndividualFilesStep1.Text = String.Format("Step 1: Select Individual Files to be Hashed: {0} Files ({1} {2} are selected)",
                                                                          MyToString(listFiles.Items.Count),
                                                                          MyToString(listFiles.SelectedItems.Count),
                                                                          If(listFiles.SelectedItems.Count = 1, "item", "items")
                                                                         )
                     Else
                         lblHashIndividualFilesStep1.Text = String.Format("Step 1: Select Individual Files to be Hashed: {0} Files",
                                                                          MyToString(listFiles.Items.Count)
                                                                         )
                     End If

                     If listFiles.Items.Count = 0 Then
                         btnComputeHash.Enabled = False
                         btnIndividualFilesCopyToClipboard.Enabled = False
                         btnIndividualFilesSaveResultsToDisk.Enabled = False
                     Else
                         Dim intNumberOfItemsWithoutHash As Integer = listFiles.Items.Cast(Of MyListViewItem).Where(Function(item As MyListViewItem) String.IsNullOrWhiteSpace(item.AllTheHashes.Sha160)).Count

                         btnComputeHash.Enabled = intNumberOfItemsWithoutHash > 0

                         If intNumberOfItemsWithoutHash <> listFiles.Items.Count Then
                             btnIndividualFilesCopyToClipboard.Enabled = True
                             btnIndividualFilesSaveResultsToDisk.Enabled = True
                         End If
                     End If
                 End Sub)
    End Sub

    Private Sub BtnRemoveAllFiles_Click(sender As Object, e As EventArgs) Handles btnRemoveAllFiles.Click
        listFiles.Items.Clear()
        filesInListFiles.Clear()
        UpdateFilesListCountHeader()
        strLastHashFileLoaded = Nothing
    End Sub

    Private Sub BtnRemoveSelectedFiles_Click(sender As Object, e As EventArgs) Handles btnRemoveSelectedFiles.Click
        If listFiles.SelectedItems.Count > 500 AndAlso MsgBox("It would be recommended to use the ""Remove All Files"" button instead, removing this many items (" & MyToString(listFiles.SelectedItems.Count) & " items) from the list is a slow process and will make the program appear locked up." & vbCrLf & vbCrLf & "Are you sure you want to remove the items this way?", MsgBoxStyle.Question + MsgBoxStyle.YesNo, strMessageBoxTitleText) = MsgBoxResult.No Then
            Exit Sub
        End If

        listFiles.BeginUpdate()
        For Each item As MyListViewItem In listFiles.SelectedItems
            filesInListFiles.Remove(item.Text.Trim.ToLower)
            listFiles.Items.Remove(item)
        Next
        listFiles.EndUpdate()

        UpdateFilesListCountHeader()
    End Sub

    Private Function CreateListFilesObject(strFileName As String) As MyListViewItem
        filesInListFiles.Add(strFileName.Trim.ToLower)

        Dim itemToBeAdded As New MyListViewItem(strFileName)
        With itemToBeAdded
            .FileSize = New IO.FileInfo(strFileName).Length
            .FileName = strFileName
            .SubItems.Add(FileSizeToHumanSize(itemToBeAdded.FileSize))
            .SubItems.Add(strWaitingToBeProcessed)
            .SubItems.Add("")
        End With

        Return itemToBeAdded
    End Function

    Private Sub BtnAddIndividualFiles_Click(sender As Object, e As EventArgs) Handles btnAddIndividualFiles.Click
        OpenFileDialog.Title = "Select Files to be Hashed..."
        OpenFileDialog.Multiselect = True
        OpenFileDialog.Filter = "Show All Files|*.*"

        If OpenFileDialog.ShowDialog() = DialogResult.OK Then
            If OpenFileDialog.FileNames.Count = 0 Then
                MsgBox("You must select some files.", MsgBoxStyle.Critical, strMessageBoxTitleText)
            ElseIf OpenFileDialog.FileNames.Count = 1 Then
                strLastDirectoryWorkedOn = New IO.FileInfo(OpenFileDialog.FileName).DirectoryName

                If Not filesInListFiles.Contains(OpenFileDialog.FileName.Trim.ToLower) Then
                    If IO.File.Exists(OpenFileDialog.FileName) Then listFiles.Items.Add(CreateListFilesObject(OpenFileDialog.FileName))
                End If
            Else
                strLastDirectoryWorkedOn = New IO.FileInfo(OpenFileDialog.FileNames(0)).DirectoryName

                listFiles.BeginUpdate()
                For Each strFileName As String In OpenFileDialog.FileNames
                    If Not filesInListFiles.Contains(strFileName.Trim.ToLower) Then
                        If IO.File.Exists(strFileName) Then listFiles.Items.Add(CreateListFilesObject(strFileName))
                    End If
                Next
                listFiles.EndUpdate()
            End If
        End If

        UpdateFilesListCountHeader()
        If chkSortFileListingAfterAddingFilesToHash.Checked Then ApplyFileSizeSortingToHashList()
    End Sub

    Private Function GetDataFromAllTheHashes(checksum As ChecksumType, allTheHashes As AllTheHashes) As String
        Select Case checksum
            Case ChecksumType.md5
                Return allTheHashes.Md5
            Case ChecksumType.sha160
                Return allTheHashes.Sha160
            Case ChecksumType.sha256
                Return allTheHashes.Sha256
            Case ChecksumType.sha384
                Return allTheHashes.Sha384
            Case ChecksumType.sha512
                Return allTheHashes.Sha512
            Case Else
                Return Nothing
        End Select
    End Function

    Private Sub BtnComputeHash_Click(sender As Object, e As EventArgs) Handles btnComputeHash.Click
        If btnComputeHash.Text = "Abort Processing" Then
            If MsgBox("Are you sure you want to abort processing?", MsgBoxStyle.Question + MsgBoxStyle.YesNo + MsgBoxStyle.DefaultButton2, strMessageBoxTitleText) = MsgBoxResult.Yes Then
                If workingThread IsNot Nothing Then
                    workingThread.Abort()
                    boolBackgroundThreadWorking = False
                End If

                Exit Sub
            Else
                Exit Sub
            End If
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
        Dim itemOnGUI As MyListViewItem = Nothing
        Dim currentItem As MyListViewItem = Nothing
        Dim intIndexBeingWorkedOn As Integer

        workingThread = New Threading.Thread(Sub()
                                                 Try
                                                     boolBackgroundThreadWorking = True
                                                     Dim percentage, allBytesPercentage As Double
                                                     Dim strChecksum As String = Nothing
                                                     Dim checksumType As ChecksumType
                                                     Dim index As Integer = 1
                                                     Dim stopWatch As Stopwatch = Stopwatch.StartNew
                                                     Dim computeStopwatch As Stopwatch
                                                     Dim items As ListView.ListViewItemCollection = GetListViewItems(listFiles)
                                                     Dim allTheHashes As AllTheHashes = Nothing
                                                     Dim fileCountPercentage As Double

                                                     SyncLock threadLockingObject
                                                         longAllReadBytes = 0
                                                         longAllBytes = 0
                                                     End SyncLock

                                                     Dim subRoutine As [Delegate] = Sub(size As Long, totalBytesRead As Long)
                                                                                        Try
                                                                                            MyInvoke(Sub()
                                                                                                         percentage = If(totalBytesRead = 0 Or size = 0, 0, totalBytesRead / size * 100) ' This fixes a possible divide by zero exception.
                                                                                                         IndividualFilesProgressBar.Value = percentage
                                                                                                         SyncLock threadLockingObject
                                                                                                             allBytesPercentage = longAllReadBytes / longAllBytes * 100
                                                                                                             lblHashIndividualFilesTotalStatus.Text = FileSizeToHumanSize(longAllReadBytes) & " of " & FileSizeToHumanSize(longAllBytes) & " (" & MyRoundingFunction(allBytesPercentage, byteRoundPercentages) & "%) has been processed."
                                                                                                             If chkShowPercentageInWindowTitleBar.Checked Then Text = strWindowTitle & " (" & MyRoundingFunction(allBytesPercentage, byteRoundPercentages) & "% Completed)"
                                                                                                         End SyncLock
                                                                                                         ProgressForm.SetTaskbarProgressBarValue(allBytesPercentage)
                                                                                                         hashIndividualFilesAllFilesProgressBar.Value = allBytesPercentage
                                                                                                         lblIndividualFilesStatus.Text = FileSizeToHumanSize(totalBytesRead) & " of " & FileSizeToHumanSize(size) & " (" & MyRoundingFunction(percentage, byteRoundPercentages) & "%) have been processed."

                                                                                                         If chkShowFileProgressInFileList.Checked Then
                                                                                                             currentItem.SubItems(2).Text = lblIndividualFilesStatus.Text
                                                                                                             itemOnGUI.SubItems(2) = currentItem.SubItems(2)
                                                                                                         End If
                                                                                                     End Sub)
                                                                                        Catch ex As Exception
                                                                                        End Try
                                                                                    End Sub

                                                     MyInvoke(Sub()
                                                                  radioMD5.Enabled = False
                                                                  radioSHA1.Enabled = False
                                                                  radioSHA256.Enabled = False
                                                                  radioSHA384.Enabled = False
                                                                  radioSHA512.Enabled = False

                                                                  If radioMD5.Checked Then
                                                                      checksumType = ChecksumType.md5
                                                                  ElseIf radioSHA1.Checked Then
                                                                      checksumType = ChecksumType.sha160
                                                                  ElseIf radioSHA256.Checked Then
                                                                      checksumType = ChecksumType.sha256
                                                                  ElseIf radioSHA384.Checked Then
                                                                      checksumType = ChecksumType.sha384
                                                                  ElseIf radioSHA512.Checked Then
                                                                      checksumType = ChecksumType.sha512
                                                                  End If
                                                              End Sub)

                                                     SyncLock threadLockingObject
                                                         For Each item As MyListViewItem In items
                                                             If String.IsNullOrWhiteSpace(item.Hash) And IO.File.Exists(item.FileName) Then longAllBytes += item.FileSize
                                                         Next
                                                     End SyncLock

                                                     For Each item As MyListViewItem In items
                                                         currentItem = item
                                                         intIndexBeingWorkedOn = item.Index
                                                         itemOnGUI = Nothing
                                                         MyInvoke(Sub() itemOnGUI = listFiles.Items(item.Index))

                                                         SyncLock threadLockingObject
                                                             If Not IO.File.Exists(item.FileName) Then longAllBytes -= item.FileSize
                                                         End SyncLock

                                                         If String.IsNullOrWhiteSpace(item.Hash) And IO.File.Exists(item.FileName) Then
                                                             item.SubItems(2).Text = strCurrentlyBeingProcessed

                                                             MyInvoke(Sub()
                                                                          fileCountPercentage = index / listFiles.Items.Count * 100
                                                                          lblProcessingFile.Text = "Now processing file " & New IO.FileInfo(item.FileName).Name & "."
                                                                          lblIndividualFilesStatusProcessingFile.Text = GenerateProcessingFileString(index, listFiles.Items.Count)

                                                                          UpdateListViewItem(itemOnGUI, item)
                                                                      End Sub)

                                                             computeStopwatch = Stopwatch.StartNew

                                                             If DoChecksumWithAttachedSubRoutine(item.FileName, allTheHashes, subRoutine) Then
                                                                 item.AllTheHashes = allTheHashes
                                                                 strChecksum = GetDataFromAllTheHashes(checksumType, allTheHashes)
                                                                 item.SubItems(2).Text = If(chkDisplayHashesInUpperCase.Checked, strChecksum.ToUpper, strChecksum.ToLower)
                                                                 item.ComputeTime = computeStopwatch.Elapsed
                                                                 item.SubItems(3).Text = TimespanToHMS(item.ComputeTime)
                                                                 item.Hash = strChecksum
                                                             Else
                                                                 item.SubItems(2).Text = "(Error while calculating checksum)"
                                                                 item.SubItems(3).Text = ""
                                                                 item.ComputeTime = Nothing
                                                                 longErroredFiles += 1
                                                             End If

                                                             MyInvoke(Sub() UpdateListViewItem(itemOnGUI, item))
                                                         End If

                                                         index += 1
                                                     Next

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
                                                                      MsgBox("Completed in " & TimespanToHMS(stopWatch.Elapsed) & ".", MsgBoxStyle.Information, strMessageBoxTitleText)
                                                                  Else
                                                                      MsgBox("Completed in " & TimespanToHMS(stopWatch.Elapsed) & "." & vbCrLf & vbCrLf & longErroredFiles.ToString & If(longErroredFiles = 1, " file", " files") & " experienced a general I/O error while processing.", MsgBoxStyle.Information, strMessageBoxTitleText)
                                                                  End If
                                                              End Sub)
                                                 Catch ex As Threading.ThreadAbortException
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

                                                                      currentItem.SubItems(2).Text = strWaitingToBeProcessed
                                                                      UpdateListViewItem(itemOnGUI, currentItem)

                                                                      Dim intNumberOfItemsWithoutHash As Integer = listFiles.Items.Cast(Of MyListViewItem).Where(Function(item As MyListViewItem) String.IsNullOrWhiteSpace(item.AllTheHashes.Sha160)).Count
                                                                      btnComputeHash.Enabled = intNumberOfItemsWithoutHash > 0
                                                                  End If

                                                                  boolBackgroundThreadWorking = False
                                                                  workingThread = Nothing
                                                                  If Not boolClosingWindow Then MsgBox("Processing aborted.", MsgBoxStyle.Information, strMessageBoxTitleText)
                                                              End Sub)
                                                 Finally
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
                                                              End Sub)
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

    Private Function StrGetIndividualHashesInStringFormat(strPathOfChecksumFile As String, checksumType As ChecksumType) As String
        Dim strDirectoryName As String = New IO.FileInfo(strPathOfChecksumFile).DirectoryName
        Dim folderOfChecksumFile As String = If(strDirectoryName.Length = 3, strDirectoryName, strDirectoryName & "\")
        Dim stringBuilder As New Text.StringBuilder()
        Dim strFile As String

        AddHashFileHeader(stringBuilder, listFiles.Items.Count)

        For Each item As MyListViewItem In listFiles.Items
            If Not String.IsNullOrWhiteSpace(item.Hash) Then
                strFile = item.FileName
                If chkSaveChecksumFilesWithRelativePaths.Checked Then strFile = strFile.CaseInsensitiveReplace(folderOfChecksumFile, "")
                stringBuilder.AppendLine(GetDataFromAllTheHashes(checksumType, item.AllTheHashes) & " *" & strFile)
            End If
        Next

        AddEndOfHashLines(stringBuilder)
        Return stringBuilder.ToString()
    End Function

    Private Function StrGetIndividualHashesInStringFormat() As String
        Dim stringBuilder As New Text.StringBuilder()

        AddHashFileHeader(stringBuilder, listFiles.Items.Count)

        For Each item As MyListViewItem In listFiles.Items
            If Not String.IsNullOrWhiteSpace(item.Hash) Then
                stringBuilder.AppendLine(item.Hash & " *" & item.FileName)
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
        SaveFileDialog.Filter = "MD5 File|*.md5|SHA1 File|*.sha1|SHA256 File|*.sha256|SHA384 File|*.sha384|SHA512 File|*.sha512"
        SaveFileDialog.InitialDirectory = strLastDirectoryWorkedOn
        SaveFileDialog.Title = "Save Hash Results to Disk"
        SaveFileDialog.FilterIndex = ChecksumFilterIndexSHA256
        If chkAutoAddExtension.Checked Then SaveFileDialog.OverwritePrompt = False ' We handle this in our own code below.

        Dim strFileExtension As String
        Dim checksumType As ChecksumType

        If Not String.IsNullOrWhiteSpace(strLastHashFileLoaded) Then
            strFileExtension = New IO.FileInfo(strLastHashFileLoaded).Extension

            If strFileExtension.Equals(".md5", StringComparison.OrdinalIgnoreCase) Then
                SaveFileDialog.FilterIndex = ChecksumFilterIndexMD5
            ElseIf strFileExtension.Equals(".sha1", StringComparison.OrdinalIgnoreCase) Then
                SaveFileDialog.FilterIndex = ChecksumFilterIndexSHA160
            ElseIf strFileExtension.Equals(".sha256", StringComparison.OrdinalIgnoreCase) Then
                SaveFileDialog.FilterIndex = ChecksumFilterIndexSHA256
            ElseIf strFileExtension.Equals(".sha384", StringComparison.OrdinalIgnoreCase) Then
                SaveFileDialog.FilterIndex = ChecksumFilterIndexSHA384
            ElseIf strFileExtension.Equals(".sha512", StringComparison.OrdinalIgnoreCase) Then
                SaveFileDialog.FilterIndex = ChecksumFilterIndexSHA512
            End If

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
                Dim msgBoxResult As MsgBoxResult

                If SaveFileDialog.FilterIndex = ChecksumFilterIndexMD5 Then
                    msgBoxResult = MsgBox("MD5 is not recommended for hashing files." & vbCrLf & vbCrLf & "Are you sure you want to use this hash type?", MsgBoxStyle.Question + MsgBoxStyle.YesNo, strMessageBoxTitleText)
                ElseIf SaveFileDialog.FilterIndex = ChecksumFilterIndexSHA160 Then
                    msgBoxResult = MsgBox("SHA1 is not recommended for hashing files." & vbCrLf & vbCrLf & "Are you sure you want to use this hash type?", MsgBoxStyle.Question + MsgBoxStyle.YesNo, strMessageBoxTitleText)
                End If

                If msgBoxResult = MsgBoxResult.No Then
                    MsgBox("Your hash data has not been saved to disk.", MsgBoxStyle.Information, strMessageBoxTitleText)
                    Exit Sub
                End If
            End If

            If chkAutoAddExtension.Checked Then
                strFileExtension = New IO.FileInfo(SaveFileDialog.FileName).Extension

                If Not strFileExtension.Equals(".md5", StringComparison.OrdinalIgnoreCase) And Not strFileExtension.Equals(".sha1", StringComparison.OrdinalIgnoreCase) And Not strFileExtension.Equals(".sha256", StringComparison.OrdinalIgnoreCase) And Not strFileExtension.Equals(".sha384", StringComparison.OrdinalIgnoreCase) And Not strFileExtension.Equals(".sha512", StringComparison.OrdinalIgnoreCase) Then
                    If SaveFileDialog.FilterIndex = ChecksumFilterIndexMD5 Then
                        SaveFileDialog.FileName &= ".md5"
                    ElseIf SaveFileDialog.FilterIndex = ChecksumFilterIndexSHA160 Then
                        SaveFileDialog.FileName &= ".sha1"
                    ElseIf SaveFileDialog.FilterIndex = ChecksumFilterIndexSHA256 Then
                        SaveFileDialog.FileName &= ".sha256"
                    ElseIf SaveFileDialog.FilterIndex = ChecksumFilterIndexSHA384 Then
                        SaveFileDialog.FileName &= ".sha384"
                    ElseIf SaveFileDialog.FilterIndex = ChecksumFilterIndexSHA512 Then
                        SaveFileDialog.FileName &= ".sha512"
                    End If
                End If

                If IO.File.Exists(SaveFileDialog.FileName) AndAlso MsgBox("The file named """ & New IO.FileInfo(SaveFileDialog.FileName).Name & """ already exists." & vbCrLf & vbCrLf & "Are you absolutely sure you want to replace it?", MsgBoxStyle.Question + MsgBoxStyle.YesNo + MsgBoxStyle.DefaultButton2, "Overwrite?") = MsgBoxResult.No Then
                    MsgBox("Save Results to Disk Aborted.", MsgBoxStyle.Information, strMessageBoxTitleText)
                    Exit Sub
                End If
            End If

            Dim fileInfo As New IO.FileInfo(SaveFileDialog.FileName)
            strFileExtension = fileInfo.Extension

            If strFileExtension.Equals(".md5", StringComparison.OrdinalIgnoreCase) Then
                checksumType = ChecksumType.md5
            ElseIf strFileExtension.Equals(".sha1", StringComparison.OrdinalIgnoreCase) Then
                checksumType = ChecksumType.sha160
            ElseIf strFileExtension.Equals(".sha256", StringComparison.OrdinalIgnoreCase) Then
                checksumType = ChecksumType.sha256
            ElseIf strFileExtension.Equals(".sha384", StringComparison.OrdinalIgnoreCase) Then
                checksumType = ChecksumType.sha384
            ElseIf strFileExtension.Equals(".sha512", StringComparison.OrdinalIgnoreCase) Then
                checksumType = ChecksumType.sha512
            End If

            Using streamWriter As New IO.StreamWriter(SaveFileDialog.FileName, False, System.Text.Encoding.UTF8)
                streamWriter.Write(StrGetIndividualHashesInStringFormat(SaveFileDialog.FileName, checksumType))
            End Using

            Dim openInExplorerMsgBoxResult As MsgBoxResult

            If chkOpenInExplorer.Checked Then
                openInExplorerMsgBoxResult = MsgBoxResult.Yes
                MsgBox("Your hash results have been written to disk." & vbCrLf & vbCrLf & "File Name: " & SaveFileDialog.FileName & vbCrLf & "Checksum Type: " & ConvertChecksumTypeToString(checksumType), MsgBoxStyle.Information, strMessageBoxTitleText)
            Else
                openInExplorerMsgBoxResult = MsgBox("Your hash results have been written to disk." & vbCrLf & vbCrLf & "File Name: " & SaveFileDialog.FileName & vbCrLf & "Checksum Type: " & ConvertChecksumTypeToString(checksumType) & vbCrLf & vbCrLf & "Do you want to open Windows Explorer to the location of the checksum file?", MsgBoxStyle.Information + MsgBoxStyle.YesNo, strMessageBoxTitleText)
            End If

            If openInExplorerMsgBoxResult = MsgBoxResult.Yes Then SelectFileInWindowsExplorer(fileInfo.FullName)
        End If
    End Sub

    Private Sub SelectFileInWindowsExplorer(ByVal strFullPath As String)
        If Not String.IsNullOrEmpty(strFullPath) AndAlso IO.File.Exists(strFullPath) Then
            Dim pidlList As IntPtr = NativeMethod.NativeMethods.ILCreateFromPathW(strFullPath)

            If pidlList <> IntPtr.Zero Then
                Try
                    NativeMethod.NativeMethods.SHOpenFolderAndSelectItems(pidlList, 0, IntPtr.Zero, 0)
                Finally
                    NativeMethod.NativeMethods.ILFree(pidlList)
                End Try
            End If
        End If
    End Sub

    Private Function ConvertChecksumTypeToString(checksumType As ChecksumType) As String
        If checksumType = ChecksumType.md5 Then
            Return "MD5"
        ElseIf checksumType = ChecksumType.sha160 Then
            Return "SHA1/SHA160"
        ElseIf checksumType = ChecksumType.sha256 Then
            Return "SHA256"
        ElseIf checksumType = ChecksumType.sha384 Then
            Return "SHA384"
        ElseIf checksumType = ChecksumType.sha512 Then
            Return "SHA512"
        Else
            Return ""
        End If
    End Function

    Private Sub UpdateChecksumsInListFiles(checksumType As ChecksumType)
        If listFiles.Items.Count <> 0 Then
            Dim selectedItemsIntegerArray As New List(Of Integer)
            If listFiles.SelectedIndices.Count <> 0 Then
                For Each item As Integer In listFiles.SelectedIndices()
                    selectedItemsIntegerArray.Add(item)
                Next
            End If

            listFiles.BeginUpdate()

            Dim strChecksum As String
            Dim tempListViewItemCollection As New List(Of MyListViewItem)
            Dim listViewItem As MyListViewItem

            For Each item As MyListViewItem In listFiles.Items
                listViewItem = item.Clone()
                strChecksum = GetDataFromAllTheHashes(checksumType, listViewItem.AllTheHashes)

                If Not String.IsNullOrWhiteSpace(strChecksum) Then
                    listViewItem.Hash = strChecksum
                    listViewItem.SubItems(2).Text = If(chkDisplayHashesInUpperCase.Checked, strChecksum.ToUpper, strChecksum.ToLower)
                End If

                tempListViewItemCollection.Add(listViewItem)
                listViewItem = Nothing
            Next

            listFiles.Items.Clear()
            listFiles.Items.AddRange(tempListViewItemCollection.ToArray)

            If selectedItemsIntegerArray.Count <> 0 Then
                For Each item As Integer In selectedItemsIntegerArray
                    listFiles.Items(item).Selected = True
                Next
                listFiles.Select()
                listFiles.EnsureVisible(selectedItemsIntegerArray(selectedItemsIntegerArray.Count - 1))
            End If

            listFiles.EndUpdate()
            listFiles.Refresh()
        End If
    End Sub

    Private Sub RadioMD5_Click(sender As Object, e As EventArgs) Handles radioMD5.Click
        UpdateChecksumsInListFiles(ChecksumType.md5)
        colChecksum.Text = strColumnTitleChecksumMD5
    End Sub

    Private Sub RadioSHA1_Click(sender As Object, e As EventArgs) Handles radioSHA1.Click
        UpdateChecksumsInListFiles(ChecksumType.sha160)
        colChecksum.Text = strColumnTitleChecksumSHA160
    End Sub

    Private Sub RadioSHA256_Click(sender As Object, e As EventArgs) Handles radioSHA256.Click
        UpdateChecksumsInListFiles(ChecksumType.sha256)
        colChecksum.Text = strColumnTitleChecksumSHA256
    End Sub

    Private Sub RadioSHA384_Click(sender As Object, e As EventArgs) Handles radioSHA384.Click
        UpdateChecksumsInListFiles(ChecksumType.sha384)
        colChecksum.Text = strColumnTitleChecksumSHA384
    End Sub

    Private Sub RadioSHA512_Click(sender As Object, e As EventArgs) Handles radioSHA512.Click
        UpdateChecksumsInListFiles(ChecksumType.sha512)
        colChecksum.Text = strColumnTitleChecksumSHA512
    End Sub

    Private Sub LaunchURLInWebBrowser(url As String, Optional errorMessage As String = "An error occurred when trying the URL In your Default browser. The URL has been copied to your Windows Clipboard for you to paste into the address bar in the web browser of your choice.")
        If Not url.Trim.StartsWith("http", StringComparison.OrdinalIgnoreCase) Then url = If(chkSSL.Checked, "https://" & url, "http://" & url)

        Try
            Process.Start(url)
        Catch ex As Exception
            CopyTextToWindowsClipboard(url)
            MsgBox(errorMessage, MsgBoxStyle.Information, strMessageBoxTitleText)
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

    ''' <summary>Creates a named pipe server. Returns a Boolean value indicating if the function was able to create a named pipe server.</summary>
    ''' <returns>Returns a Boolean value indicating if the function was able to create a named pipe server.</returns>
    Private Function StartNamedPipeServer() As Boolean
        Try
            Dim pipeServer As NamedPipeServerStream = New NamedPipeServerStream(strNamedPipeServerName, PipeDirection.In, 1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous)
            pipeServer.BeginWaitForConnection(New AsyncCallback(AddressOf WaitForConnectionCallBack), pipeServer)
            Return True ' We were able to create a named pipe server. Yay!
        Catch oEX As Exception
            Return False ' OK, there's already a named pipe server in operation already so we return a False value.
        End Try
    End Function

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
                MyInvoke(Sub()
                             TabControl1.SelectTab(TabNumberHashIndividualFilesTab)
                             NativeMethod.NativeMethods.SetForegroundWindow(Handle.ToInt32())
                         End Sub)

                If Not IO.File.GetAttributes(strReceivedFileName).HasFlag(IO.FileAttributes.Directory) AndAlso Not filesInListFiles.Contains(strReceivedFileName.Trim.ToLower) Then
                    strLastDirectoryWorkedOn = New IO.FileInfo(strReceivedFileName).DirectoryName
                    MyInvoke(Sub() If IO.File.Exists(strReceivedFileName) Then listFiles.Items.Add(CreateListFilesObject(strReceivedFileName)))
                Else
                    AddFilesFromDirectory(strReceivedFileName)
                End If

                UpdateFilesListCountHeader()
                If chkSortFileListingAfterAddingFilesToHash.Checked Then ApplyFileSizeSortingToHashList()
            End If
        Catch ex As Exception
        End Try
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' This function returns a Boolean value indicating if the name pipe server was started or not.
        Dim boolNamedPipeServerStarted As Boolean = StartNamedPipeServer()
        Dim commandLineArgument As String

        If My.Application.CommandLineArgs.Count = 1 Then
            commandLineArgument = My.Application.CommandLineArgs(0).Trim

            If commandLineArgument.StartsWith("--addfile=", StringComparison.OrdinalIgnoreCase) Or commandLineArgument.StartsWith("--comparefile=", StringComparison.OrdinalIgnoreCase) Then
                If boolNamedPipeServerStarted Then
                    ' This instance of the program is the first executed instance so it's the host of the named pipe server.
                    ' We still need to process the first incoming file passed to it via command line arguments. After doing
                    ' so, this instance of the program will continue operating as the host of the named pipe server.
                    If commandLineArgument.StartsWith("--addfile=", StringComparison.OrdinalIgnoreCase) Then
                        ' We now have to strip off what we don't need.
                        commandLineArgument = commandLineArgument.CaseInsensitiveReplace("--addfile=", "").Replace(Chr(34), "")
                        AddFileOrDirectoryToHashFileList(commandLineArgument)
                    ElseIf commandLineArgument.StartsWith("--comparefile=", StringComparison.OrdinalIgnoreCase) Then
                        ' We now have to strip off what we don't need.
                        commandLineArgument = commandLineArgument.CaseInsensitiveReplace("--comparefile=", "").Replace(Chr(34), "")
                        txtFile1.Text = commandLineArgument
                    End If
                Else
                    ' This instance of the program isn't the first running instance, this instance is a secondary instance
                    ' for the lack of a better word. However, this instance has received data from Windows Explorer so we
                    ' need to do something with it, namely pass that data to the first running instance via the named
                    ' pipe and then terminate itself.
                    SendToIPCNamedPipeServer(commandLineArgument) ' This passes the data to the named pipe server.
                    Process.GetCurrentProcess.Kill() ' This terminates the process.
                End If
            ElseIf commandLineArgument.StartsWith("--hashfile=", StringComparison.OrdinalIgnoreCase) Then
                commandLineArgument = commandLineArgument.CaseInsensitiveReplace("--hashfile=", "").Replace(Chr(34), "")

                If IO.File.Exists(commandLineArgument) Then
                    TabControl1.SelectTab(TabNumberVerifySavedHashesTab)
                    btnOpenExistingHashFile.Text = "Abort Processing"
                    verifyHashesListFiles.Items.Clear()
                    ProcessExistingHashFile(commandLineArgument)
                End If
            ElseIf commandLineArgument.StartsWith("--knownhashfile=", StringComparison.OrdinalIgnoreCase) Then
                commandLineArgument = commandLineArgument.CaseInsensitiveReplace("--knownhashfile=", "").Replace(Chr(34), "")
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
            btnAssociate.ImageAlign = ContentAlignment.MiddleLeft
            btnAssociate.Image = My.Resources.UAC
            btnAddHasherToAllFiles.ImageAlign = ContentAlignment.MiddleLeft
            btnAddHasherToAllFiles.Image = My.Resources.UAC
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
        chkSSL.Checked = My.Settings.boolSSL
        chkSortByFileSizeAfterLoadingHashFile.Checked = My.Settings.boolSortByFileSizeAfterLoadingHashFile
        chkSortFileListingAfterAddingFilesToHash.Checked = My.Settings.boolSortFileListingAfterAddingFilesToHash
        chkSaveChecksumFilesWithRelativePaths.Checked = My.Settings.boolSaveChecksumFilesWithRelativePaths
        chkUseMilliseconds.Checked = My.Settings.boolUseMilliseconds
        chkDisplayHashesInUpperCase.Checked = My.Settings.boolDisplayHashesInUpperCase
        chkUseCommasInNumbers.Checked = My.Settings.boolUseCommasInNumbers
        chkCheckForUpdates.Checked = My.Settings.boolCheckForUpdates
        chkAutoAddExtension.Checked = My.Settings.boolAutoAddExtension
        chkDisplayValidChecksumString.Checked = My.Settings.boolDisplayValidChecksumString
        chkOpenInExplorer.Checked = My.Settings.boolOpenInExplorer
        chkShowPercentageInWindowTitleBar.Checked = My.Settings.boolShowPercentageInWindowTitleBar
        chkShowFileProgressInFileList.Checked = My.Settings.boolShowFileProgressInFileList
        lblWelcomeText.Text = String.Format(lblWelcomeText.Text,
                                            Check_for_Update_Stuff.versionString,
                                            If(Environment.Is64BitProcess, "64", "32"),
                                            If(Environment.Is64BitOperatingSystem, "64", "32")
                                           )
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
        byteRoundFileSizes = My.Settings.roundFileSizes
        byteRoundPercentages = My.Settings.roundPercentages
        btnSetRoundFileSizes.Enabled = False
        btnSetRoundPercentages.Enabled = False
        Location = My.Settings.windowLocation

        If My.Settings.defaultHash < 0 Or My.Settings.defaultHash > 4 Then My.Settings.defaultHash = 2
        defaultHashType.SelectedIndex = My.Settings.defaultHash
        SetDefaultHashTypeGUIElementOptions()

        If Debugger.IsAttached Then
            Text &= " (Debugger Attached)"
            btnAddHasherToAllFiles.Visible = False
            btnAssociate.Visible = False
        End If

        DeleteTemporaryNewEXEFile()

        colFileName.Width = My.Settings.hashIndividualFilesFileNameColumnSize
        colFileSize.Width = My.Settings.hashIndividualFilesFileSizeColumnSize
        colChecksum.Width = My.Settings.hashIndividualFilesChecksumColumnSize
        colComputeTime.Width = My.Settings.hashIndividualFilesComputeTimeColumnSize
        colNewHash.Width = My.Settings.newHashChecksumColumnSize

        colFile.Width = My.Settings.verifyHashFileNameColumnSize
        colFileSize2.Width = My.Settings.verifyHashFileSizeColumnSize
        colResults.Width = My.Settings.verifyHashFileResults
        colComputeTime2.Width = My.Settings.verifyHashComputeTimeColumnSize
        If My.Settings.taskPriority > 4 Then My.Settings.taskPriority = Byte.Parse(4)
        taskPriority.SelectedIndex = My.Settings.taskPriority

        If My.Settings.boolCheckForUpdates Then
            Threading.ThreadPool.QueueUserWorkItem(Sub()
                                                       Dim checkForUpdatesClassObject As New Check_for_Update_Stuff(Me)
                                                       checkForUpdatesClassObject.CheckForUpdates(False)
                                                   End Sub)
        End If

        boolDoneLoading = True

        If Not FileAssociation.DoesCompareFilesExist() Then
            MsgBox("Hasher has a new function! The ability to compare two files from Windows Explorer." & vbCrLf & vbCrLf & "Please go to the Setting tab and click on the ""Add Hasher to All Files"" button to add support to Windows Explorer for this new feature.", MsgBoxStyle.Information, strMessageBoxTitleText)
        End If
    End Sub

    Private Sub DeleteTemporaryNewEXEFile()
        Try
            Dim newExecutableName As String = New IO.FileInfo(Application.ExecutablePath).Name & ".new.exe"
            If IO.File.Exists(newExecutableName) Then
                SearchForProcessAndKillIt(newExecutableName, False)
                IO.File.Delete(newExecutableName)
            End If
        Catch ex As Exception
        End Try
    End Sub

    Private Sub AddFilesFromDirectory(directoryPath As String)
        workingThread = New Threading.Thread(Sub()
                                                 Dim oldFilesInListFiles As Specialized.StringCollection = filesInListFiles

                                                 Try
                                                     strLastDirectoryWorkedOn = directoryPath
                                                     Dim collectionOfListViewItems As New List(Of ListViewItem)
                                                     Dim index As Integer = 0
                                                     boolBackgroundThreadWorking = True

                                                     MyInvoke(Sub()
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
                                                              End Sub)

                                                     Dim filesInDirectory As IEnumerable(Of FastDirectoryEnumerator.FileData) = FastDirectoryEnumerator.FastDirectoryEnumerator.EnumerateFiles(directoryPath, "*.*", If(chkRecurrsiveDirectorySearch.Checked, IO.SearchOption.AllDirectories, IO.SearchOption.TopDirectoryOnly))
                                                     Dim intFileIndexNumber As Integer = 0
                                                     Dim intTotalNumberOfFiles As Integer = filesInDirectory.Count
                                                     Dim percentage As Double

                                                     For Each filedata As FastDirectoryEnumerator.FileData In filesInDirectory
                                                         intFileIndexNumber += 1
                                                         MyInvoke(Sub()
                                                                      percentage = intFileIndexNumber / intTotalNumberOfFiles * 100
                                                                      IndividualFilesProgressBar.Value = percentage
                                                                      ProgressForm.SetTaskbarProgressBarValue(percentage)
                                                                      lblIndividualFilesStatus.Text = GenerateProcessingFileString(intFileIndexNumber, intTotalNumberOfFiles)
                                                                  End Sub)
                                                         If Not filesInListFiles.Contains(filedata.Path.Trim.ToLower) Then
                                                             If IO.File.Exists(filedata.Path) Then collectionOfListViewItems.Add(CreateListFilesObject(filedata.Path))
                                                         End If
                                                     Next

                                                     filesInDirectory = Nothing

                                                     MyInvoke(Sub()
                                                                  lblProcessingFile.Text = "Adding files to list... Please wait."
                                                              End Sub)

                                                     Threading.Thread.Sleep(250)

                                                     MyInvoke(Sub()
                                                                  listFiles.BeginUpdate()
                                                                  listFiles.Items.AddRange(collectionOfListViewItems.ToArray())
                                                                  collectionOfListViewItems.Clear()
                                                                  collectionOfListViewItems = Nothing
                                                                  If chkSortFileListingAfterAddingFilesToHash.Checked Then ApplyFileSizeSortingToHashList()
                                                                  listFiles.EndUpdate()

                                                                  UpdateFilesListCountHeader()

                                                                  btnComputeHash.Enabled = True
                                                                  btnIndividualFilesCopyToClipboard.Enabled = True
                                                                  btnIndividualFilesSaveResultsToDisk.Enabled = True
                                                              End Sub)
                                                 Catch ex As Threading.ThreadAbortException
                                                     filesInListFiles.Clear()
                                                     filesInListFiles = oldFilesInListFiles

                                                     MyInvoke(Sub()
                                                                  UpdateFilesListCountHeader()

                                                                  If listFiles.Items.Count <> 0 Then
                                                                      btnComputeHash.Enabled = True
                                                                      btnIndividualFilesCopyToClipboard.Enabled = True
                                                                      btnIndividualFilesSaveResultsToDisk.Enabled = True
                                                                  End If
                                                              End Sub)
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

                                                                                                If listFiles.Items.Count <> 0 Then
                                                                                                    radioSHA256.Enabled = True
                                                                                                    radioSHA384.Enabled = True
                                                                                                    radioSHA512.Enabled = True
                                                                                                    radioSHA1.Enabled = True
                                                                                                    radioMD5.Enabled = True
                                                                                                End If
                                                                                            End Sub)

                                                     boolBackgroundThreadWorking = False
                                                 End Try
                                             End Sub) With {
            .Priority = GetThreadPriority(),
            .Name = "Directory Scanning Thread",
            .IsBackground = True
        }
        workingThread.Start()
    End Sub

    Private Sub BtnAddFilesInFolder_Click(sender As Object, e As EventArgs) Handles btnAddFilesInFolder.Click
        If btnAddFilesInFolder.Text = "Abort Adding Files" AndAlso workingThread IsNot Nothing AndAlso MsgBox("Are you sure you want to abort adding files?", MsgBoxStyle.Question + vbYesNo, strMessageBoxTitleText) = MsgBoxResult.Yes Then
            workingThread.Abort()
            boolBackgroundThreadWorking = False
            Exit Sub
        End If

        If FolderBrowserDialog.ShowDialog = DialogResult.OK Then AddFilesFromDirectory(FolderBrowserDialog.SelectedPath)
    End Sub

    Private Shared Function CreateListViewItemForHashFileEntry(strFileName As String, strChecksum As String, ByRef intFilesNotFound As Integer, ByRef boolFileExists As Boolean) As MyListViewItem
        Dim listViewItem As New MyListViewItem(strFileName) With {.Hash = strChecksum, .FileName = strFileName}

        With listViewItem
            If IO.File.Exists(strFileName) Then
                .FileSize = New IO.FileInfo(strFileName).Length
                SyncLock threadLockingObject
                    longAllBytes += .FileSize
                End SyncLock
                .SubItems.Add(FileSizeToHumanSize(listViewItem.FileSize))
                .SubItems.Add("")
                .SubItems.Add("")
                .SubItems.Add(strWaitingToBeProcessed)
                .BoolFileExists = True
                boolFileExists = True
            Else
                .FileSize = -1
                .ComputeTime = Nothing
                .SubItems.Add("")
                .SubItems.Add("Doesn't Exist")
                .SubItems.Add("")
                .SubItems.Add("")
                .BoolFileExists = False
                .BackColor = Color.LightGray
                intFilesNotFound += 1
                boolFileExists = True
            End If
        End With

        Return listViewItem
    End Function

    Private Sub ProcessExistingHashFile(strPathToChecksumFile As String)
        strLastHashFileLoaded = strPathToChecksumFile
        lblVerifyFileNameLabel.Text = "File Name: " & strPathToChecksumFile
        verifyHashesListFiles.Size = New Size(verifyHashesListFiles.Size.Width, verifyHashesListFiles.Size.Height - 72)

        Dim checksumType As ChecksumType
        Dim checksumFileInfo As New IO.FileInfo(strPathToChecksumFile)
        Dim strChecksumFileExtension, strDirectoryThatContainsTheChecksumFile As String

        strLastDirectoryWorkedOn = checksumFileInfo.DirectoryName
        strChecksumFileExtension = checksumFileInfo.Extension
        strDirectoryThatContainsTheChecksumFile = checksumFileInfo.DirectoryName
        checksumFileInfo = Nothing
        intCurrentlyActiveTab = TabNumberVerifySavedHashesTab

        If strChecksumFileExtension.Equals(".md5", StringComparison.OrdinalIgnoreCase) Then
            checksumType = ChecksumType.md5
        ElseIf strChecksumFileExtension.Equals(".sha1", StringComparison.OrdinalIgnoreCase) Then
            checksumType = ChecksumType.sha160
        ElseIf strChecksumFileExtension.Equals(".sha256", StringComparison.OrdinalIgnoreCase) Then
            checksumType = ChecksumType.sha256
        ElseIf strChecksumFileExtension.Equals(".sha384", StringComparison.OrdinalIgnoreCase) Then
            checksumType = ChecksumType.sha384
        ElseIf strChecksumFileExtension.Equals(".sha512", StringComparison.OrdinalIgnoreCase) Then
            checksumType = ChecksumType.sha512
        Else
            MsgBox("Invalid Hash File Type.", MsgBoxStyle.Critical, strMessageBoxTitleText)
            Exit Sub
        End If

        checksumTypeForChecksumCompareWindow = checksumType

        workingThread = New Threading.Thread(Sub()
                                                 Dim itemOnGUI As MyListViewItem = Nothing

                                                 Try
                                                     boolBackgroundThreadWorking = True
                                                     Dim strChecksum, strFileName As String
                                                     Dim index As Integer = 1
                                                     Dim longFilesThatPassedVerification As Long = 0
                                                     Dim intFilesNotFound As Integer = 0
                                                     Dim regExMatchObject As Text.RegularExpressions.Match
                                                     Dim dataInFileArray As String() = IO.File.ReadAllLines(strPathToChecksumFile)
                                                     Dim intLineCounter As Integer = 0
                                                     Dim stopWatch As Stopwatch = Stopwatch.StartNew
                                                     Dim strReadingHashFileMessage As String = "Reading hash file and creating ListView item objects... Please wait."
                                                     Dim boolFileExists As Boolean
                                                     Dim intFileCount As Integer = 0
                                                     Dim strLineInFile As String
                                                     Dim listOfListViewItems As New List(Of MyListViewItem)
                                                     Dim intIndexBeingWorkedOn As Integer
                                                     Dim currentItem As MyListViewItem = Nothing

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
                                                                  verifyHashesListFiles.BeginUpdate()
                                                              End Sub)

                                                     SyncLock threadLockingObject
                                                         longAllReadBytes = 0
                                                         longAllBytes = 0
                                                     End SyncLock

                                                     Dim newDataInFileArray As New List(Of String)
                                                     For Each strLineInFile In dataInFileArray
                                                         If Not strLineInFile.StartsWith("'") Then newDataInFileArray.Add(strLineInFile)
                                                     Next
                                                     strLineInFile = Nothing
                                                     dataInFileArray = Nothing

                                                     For Each strLineInFile In newDataInFileArray
                                                         intLineCounter += 1
                                                         MyInvoke(Sub()
                                                                      VerifyHashProgressBar.Value = intLineCounter / newDataInFileArray.LongCount * 100
                                                                      ProgressForm.SetTaskbarProgressBarValue(VerifyHashProgressBar.Value)
                                                                      lblVerifyHashStatus.Text = strReadingHashFileMessage & " Processing item " & MyToString(intLineCounter) & " of " & MyToString(newDataInFileArray.LongCount) & " (" & MyRoundingFunction(VerifyHashProgressBar.Value, byteRoundPercentages) & "%)."
                                                                  End Sub)

                                                         If Not String.IsNullOrEmpty(strLineInFile) Then
                                                             regExMatchObject = hashLineParser.Match(strLineInFile)

                                                             If regExMatchObject.Success Then
                                                                 strChecksum = regExMatchObject.Groups(1).Value
                                                                 strFileName = regExMatchObject.Groups(2).Value

                                                                 If Not hashLineFilePathChecker.IsMatch(strFileName) Then
                                                                     strFileName = IO.Path.Combine(strDirectoryThatContainsTheChecksumFile, strFileName)
                                                                 End If

                                                                 listOfListViewItems.Add(CreateListViewItemForHashFileEntry(strFileName, strChecksum, intFilesNotFound, boolFileExists))
                                                                 If boolFileExists Then intFileCount += 1
                                                             End If

                                                             regExMatchObject = Nothing
                                                         End If
                                                     Next

                                                     MyInvoke(Sub()
                                                                  verifyHashesListFiles.Items.AddRange(listOfListViewItems.ToArray)
                                                                  verifyHashesListFiles.EndUpdate()
                                                                  Text = strWindowTitle
                                                                  If chkSortByFileSizeAfterLoadingHashFile.Checked Then ApplyFileSizeSortingToVerifyList()
                                                                  VerifyHashProgressBar.Value = 0
                                                                  ProgressForm.SetTaskbarProgressBarValue(0)
                                                                  lblVerifyHashStatusProcessingFile.Visible = True
                                                              End Sub)

                                                     newDataInFileArray = Nothing
                                                     strLineInFile = Nothing
                                                     listOfListViewItems = Nothing

                                                     Dim items As ListView.ListViewItemCollection = GetListViewItems(verifyHashesListFiles)
                                                     Dim strChecksumInFile As String = Nothing
                                                     Dim percentage, allBytesPercentage As Double
                                                     Dim computeStopwatch As Stopwatch
                                                     Dim allTheHashes As AllTheHashes = Nothing
                                                     Dim strDisplayValidChecksumString As String = If(chkDisplayValidChecksumString.Checked, "Valid Checksum", "")
                                                     Dim fileCountPercentage As Double
                                                     Dim subRoutine As [Delegate] = Sub(size As Long, totalBytesRead As Long)
                                                                                        Try
                                                                                            MyInvoke(Sub()
                                                                                                         percentage = If(totalBytesRead = 0 Or size = 0, 0, totalBytesRead / size * 100) ' This fixes a possible divide by zero exception.
                                                                                                         VerifyHashProgressBar.Value = percentage
                                                                                                         SyncLock threadLockingObject
                                                                                                             allBytesPercentage = longAllReadBytes / longAllBytes * 100
                                                                                                             lblVerifyHashesTotalStatus.Text = FileSizeToHumanSize(longAllReadBytes) & " of " & FileSizeToHumanSize(longAllBytes) & " (" & MyRoundingFunction(allBytesPercentage, byteRoundPercentages) & "%) have been processed."
                                                                                                             If chkShowPercentageInWindowTitleBar.Checked Then Text = strWindowTitle & " (" & MyRoundingFunction(allBytesPercentage, byteRoundPercentages) & "% Completed)"
                                                                                                         End SyncLock
                                                                                                         lblProcessingFileVerify.Text = FileSizeToHumanSize(totalBytesRead) & " of " & FileSizeToHumanSize(size) & " (" & MyRoundingFunction(percentage, byteRoundPercentages) & "%) have been processed."
                                                                                                         If chkShowFileProgressInFileList.Checked Then
                                                                                                             currentItem.SubItems(4).Text = lblProcessingFileVerify.Text
                                                                                                             itemOnGUI.SubItems(4) = currentItem.SubItems(4)
                                                                                                         End If
                                                                                                         ProgressForm.SetTaskbarProgressBarValue(allBytesPercentage)
                                                                                                         verifyIndividualFilesAllFilesProgressBar.Value = allBytesPercentage
                                                                                                     End Sub)
                                                                                        Catch ex As Exception
                                                                                        End Try
                                                                                    End Sub

                                                     For Each item As MyListViewItem In items
                                                         currentItem = item
                                                         intIndexBeingWorkedOn = item.Index
                                                         fileCountPercentage = index / intFileCount * 100
                                                         MyInvoke(Sub()
                                                                      lblVerifyHashStatusProcessingFile.Text = GenerateProcessingFileString(index, intFileCount)
                                                                      itemOnGUI = Nothing
                                                                      itemOnGUI = verifyHashesListFiles.Items(item.Index)
                                                                  End Sub)

                                                         If item.BoolFileExists Then
                                                             strChecksum = item.Hash
                                                             strFileName = item.FileName

                                                             item.SubItems(4).Text = strCurrentlyBeingProcessed

                                                             MyInvoke(Sub()
                                                                          lblVerifyHashStatus.Text = "Now processing file " & New IO.FileInfo(strFileName).Name & "."
                                                                          UpdateListViewItem(itemOnGUI, item)
                                                                      End Sub)

                                                             computeStopwatch = Stopwatch.StartNew

                                                             If DoChecksumWithAttachedSubRoutine(strFileName, allTheHashes, subRoutine) Then
                                                                 strChecksum = GetDataFromAllTheHashes(checksumType, allTheHashes)
                                                                 item.AllTheHashes = allTheHashes

                                                                 If strChecksum.Equals(item.Hash, StringComparison.OrdinalIgnoreCase) Then
                                                                     item.Color = validColor
                                                                     item.SubItems(2).Text = "Valid Checksum"
                                                                     item.ComputeTime = computeStopwatch.Elapsed
                                                                     item.SubItems(3).Text = TimespanToHMS(item.ComputeTime)
                                                                     item.SubItems(4).Text = strDisplayValidChecksumString
                                                                     longFilesThatPassedVerification += 1
                                                                     item.BoolValidHash = True
                                                                 Else
                                                                     item.Color = notValidColor
                                                                     item.SubItems(2).Text = "Incorrect Checksum"
                                                                     item.ComputeTime = computeStopwatch.Elapsed
                                                                     item.SubItems(3).Text = TimespanToHMS(item.ComputeTime)
                                                                     item.SubItems(4).Text = If(chkDisplayHashesInUpperCase.Checked, GetDataFromAllTheHashes(checksumType, allTheHashes).ToUpper, GetDataFromAllTheHashes(checksumType, allTheHashes).ToLower)
                                                                     item.BoolValidHash = False
                                                                 End If
                                                             Else
                                                                 item.Color = fileNotFoundColor
                                                                 item.SubItems(2).Text = "(Error while calculating checksum)"
                                                             End If

                                                             MyInvoke(Sub() UpdateListViewItem(itemOnGUI, item))

                                                             index += 1
                                                         Else
                                                             item.BoolValidHash = False
                                                         End If
                                                     Next

                                                     subRoutine = Nothing

                                                     MyInvoke(Sub()
                                                                  For Each item As MyListViewItem In verifyHashesListFiles.Items
                                                                      If item.BoolFileExists Then item.BackColor = item.Color
                                                                  Next

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
                                                                          sbMessageBoxText.AppendLine(String.Format("Processing of hash file complete. {0} out of {1} file(s) passed verification, {2} {3} didn't pass verification.",
                                                                                                                    MyToString(longFilesThatPassedVerification),
                                                                                                                    MyToString(intFileCount),
                                                                                                                    MyToString(intFilesThatDidNotPassVerification),
                                                                                                                    If(intFilesThatDidNotPassVerification = 1, "file", "files")
                                                                                                                   )
                                                                           )
                                                                      End If
                                                                  Else
                                                                      sbMessageBoxText.AppendLine("Processing of hash file complete.")
                                                                      sbMessageBoxText.AppendLine()
                                                                      btnRetestFailedFiles.Visible = True

                                                                      Dim intTotalFiles As Integer = intFileCount - intFilesNotFound
                                                                      If longFilesThatPassedVerification = intTotalFiles Then
                                                                          sbMessageBoxText.AppendLine(String.Format("All files have passed verification. Unfortunately, {0} {1} were not found.",
                                                                                                                    MyToString(intFilesNotFound),
                                                                                                                    If(intFilesNotFound = 1, "file", "files")
                                                                                                                   )
                                                                           )
                                                                      Else
                                                                          intFilesThatDidNotPassVerification = intTotalFiles - longFilesThatPassedVerification
                                                                          If intFilesThatDidNotPassVerification <> 0 Then btnRetestFailedFiles.Visible = True
                                                                          sbMessageBoxText.AppendLine(String.Format("Not all of the files passed verification, only {0} out of {1} {2} passed verification, Unfortunately, {3} {4} didn't pass verification and {5} {6} were not found.",
                                                                                                                    MyToString(longFilesThatPassedVerification),
                                                                                                                    MyToString(intTotalFiles),
                                                                                                                    If(intTotalFiles = 1, "file", "files"),
                                                                                                                    MyToString(intFilesThatDidNotPassVerification),
                                                                                                                    If(intFilesThatDidNotPassVerification = 1, "file", "files"),
                                                                                                                    MyToString(intFilesNotFound),
                                                                                                                    If(intFilesNotFound = 1, "file", "files")
                                                                                                                   )
                                                                           )
                                                                      End If
                                                                  End If

                                                                  sbMessageBoxText.AppendLine()
                                                                  sbMessageBoxText.AppendLine("Processing completed in " & TimespanToHMS(stopWatch.Elapsed) & ".")

                                                                  MsgBox(sbMessageBoxText.ToString.Trim, MsgBoxStyle.Information, strMessageBoxTitleText)
                                                              End Sub)

                                                     boolBackgroundThreadWorking = False
                                                     workingThread = Nothing
                                                 Catch ex As Threading.ThreadAbortException
                                                     MyInvoke(Sub()
                                                                  If Not boolClosingWindow Then
                                                                      verifyHashesListFiles.EndUpdate()
                                                                      lblVerifyHashStatusProcessingFile.Visible = False
                                                                      verifyIndividualFilesAllFilesProgressBar.Visible = False
                                                                      lblVerifyHashStatus.Visible = False
                                                                      lblVerifyHashesTotalStatus.Visible = False
                                                                      lblProcessingFileVerify.Visible = False
                                                                      VerifyHashProgressBar.Value = 0
                                                                      VerifyHashProgressBar.Visible = False
                                                                      ProgressForm.SetTaskbarProgressBarValue(0)
                                                                      verifyHashesListFiles.Items.Clear()
                                                                      Text = strWindowTitle
                                                                      verifyHashesListFiles.Size = New Size(verifyHashesListFiles.Size.Width, verifyHashesListFiles.Size.Height + 72)
                                                                      lblVerifyFileNameLabel.Text = "File Name: (None Selected for Processing)"
                                                                  End If

                                                                  boolBackgroundThreadWorking = False
                                                                  workingThread = Nothing
                                                                  If Not boolClosingWindow Then MsgBox("Processing aborted.", MsgBoxStyle.Information, strMessageBoxTitleText)
                                                              End Sub)
                                                 Finally
                                                     itemOnGUI = Nothing
                                                     intCurrentlyActiveTab = TabNumberNull
                                                     SyncLock threadLockingObject
                                                         longAllReadBytes = 0
                                                         longAllBytes = 0
                                                     End SyncLock

                                                     MyInvoke(Sub()
                                                                  If Not boolClosingWindow Then
                                                                      btnTransferToHashIndividualFilesTab.Enabled = verifyHashesListFiles.Items.Count <> 0
                                                                      btnOpenExistingHashFile.Text = "Open Hash File"
                                                                      ProgressForm.SetTaskbarProgressBarValue(0)
                                                                      verifyIndividualFilesAllFilesProgressBar.Value = 0
                                                                  End If
                                                              End Sub)
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
            If MsgBox("Are you sure you want to abort processing?", MsgBoxStyle.Question + MsgBoxStyle.YesNo + MsgBoxStyle.DefaultButton2, strMessageBoxTitleText) = MsgBoxResult.Yes Then
                If workingThread IsNot Nothing Then
                    workingThread.Abort()
                    boolBackgroundThreadWorking = False
                End If

                Exit Sub
            Else
                Exit Sub
            End If
        End If

        btnTransferToHashIndividualFilesTab.Enabled = False
        btnOpenExistingHashFile.Text = "Abort Processing"
        verifyHashesListFiles.Items.Clear()

        Dim oldMultiValue As Boolean = OpenFileDialog.Multiselect

        OpenFileDialog.Title = "Select a hash file to verify..."
        OpenFileDialog.Multiselect = False
        OpenFileDialog.Filter = "Checksum File|*.md5;*.sha1;*.sha256;*.sha384;*.sha512;*.ripemd160"

        If OpenFileDialog.ShowDialog() = DialogResult.OK Then
            ProcessExistingHashFile(OpenFileDialog.FileName)
        Else
            btnOpenExistingHashFile.Text = "Open Hash File"
        End If

        OpenFileDialog.Multiselect = oldMultiValue
    End Sub

    Private Sub ListFiles_DragDrop(sender As Object, e As DragEventArgs) Handles listFiles.DragDrop
        If boolBackgroundThreadWorking Then Exit Sub
        For Each strItem As String In e.Data.GetData(DataFormats.FileDrop)
            If IO.File.Exists(strItem) Or IO.Directory.Exists(strItem) Then
                If Not IO.File.GetAttributes(strItem).HasFlag(IO.FileAttributes.Directory) AndAlso Not filesInListFiles.Contains(strItem.Trim.ToLower) Then
                    If IO.File.Exists(strItem) Then listFiles.Items.Add(CreateListFilesObject(strItem))
                Else
                    AddFilesFromDirectory(strItem)
                End If
            End If
        Next

        UpdateFilesListCountHeader()
        If chkSortFileListingAfterAddingFilesToHash.Checked Then ApplyFileSizeSortingToHashList()
    End Sub

    Private Sub ListFiles_DragEnter(sender As Object, e As DragEventArgs) Handles listFiles.DragEnter
        e.Effect = If(e.Data.GetDataPresent(DataFormats.FileDrop), DragDropEffects.All, DragDropEffects.None)
    End Sub

    Private Sub ChkRecurrsiveDirectorySearch_Click(sender As Object, e As EventArgs) Handles chkRecurrsiveDirectorySearch.Click
        My.Settings.boolRecurrsiveDirectorySearch = chkRecurrsiveDirectorySearch.Checked
    End Sub

    Private Sub TxtTextToHash_TextChanged(sender As Object, e As EventArgs) Handles txtTextToHash.TextChanged
        lblHashTextStep1.Text = "Step 1: Input some text: " & MyToString(txtTextToHash.Text.Length) & " " & If(txtTextToHash.Text.Length = 1, "Character", "Characters")
        btnComputeTextHash.Enabled = txtTextToHash.Text.Length <> 0
        btnCopyTextHashResultsToClipboard.Enabled = False
        txtHashResults.Text = Nothing
        hashTextAllTheHashes = Nothing
    End Sub

    Private Sub FillInNewHash(checksumType As ChecksumType)
        Dim strHash As String = GetDataFromAllTheHashes(checksumType, hashTextAllTheHashes)
        If Not String.IsNullOrEmpty(strHash) Then txtHashResults.Text = If(chkDisplayHashesInUpperCase.Checked, strHash.ToUpper, strHash.ToLower)
    End Sub

    Private Sub BtnComputeTextHash_Click(sender As Object, e As EventArgs) Handles btnComputeTextHash.Click
        hashTextAllTheHashes = GetHashOfString(txtTextToHash.Text)
        Dim strHash As String = Nothing

        If textRadioMD5.Checked Then
            strHash = GetDataFromAllTheHashes(ChecksumType.md5, hashTextAllTheHashes)
        ElseIf textRadioSHA1.Checked Then
            strHash = GetDataFromAllTheHashes(ChecksumType.sha160, hashTextAllTheHashes)
        ElseIf textRadioSHA256.Checked Then
            strHash = GetDataFromAllTheHashes(ChecksumType.sha256, hashTextAllTheHashes)
        ElseIf textRadioSHA384.Checked Then
            strHash = GetDataFromAllTheHashes(ChecksumType.sha384, hashTextAllTheHashes)
        ElseIf textRadioSHA512.Checked Then
            strHash = GetDataFromAllTheHashes(ChecksumType.sha512, hashTextAllTheHashes)
        End If

        txtHashResults.Text = If(chkDisplayHashesInUpperCase.Checked, strHash.ToUpper, strHash.ToLower)
        btnCopyTextHashResultsToClipboard.Enabled = True
        btnComputeTextHash.Enabled = False
    End Sub

    Private Sub BtnPasteTextFromWindowsClipboard_Click(sender As Object, e As EventArgs) Handles btnPasteTextFromWindowsClipboard.Click
        txtTextToHash.Text = Clipboard.GetText()
    End Sub

    Private Sub BtnCopyTextHashResultsToClipboard_Click(sender As Object, e As EventArgs) Handles btnCopyTextHashResultsToClipboard.Click
        If CopyTextToWindowsClipboard(txtHashResults.Text) Then MsgBox("Your hash results have been copied to the Windows Clipboard.", MsgBoxStyle.Information, strMessageBoxTitleText)
    End Sub

    Private Sub TextRadioSHA256_CheckedChanged(sender As Object, e As EventArgs) Handles textRadioSHA256.CheckedChanged
        FillInNewHash(ChecksumType.sha256)
    End Sub

    Private Sub TextRadioSHA384_CheckedChanged(sender As Object, e As EventArgs) Handles textRadioSHA384.CheckedChanged
        FillInNewHash(ChecksumType.sha384)
    End Sub

    Private Sub TextRadioSHA512_CheckedChanged(sender As Object, e As EventArgs) Handles textRadioSHA512.CheckedChanged
        FillInNewHash(ChecksumType.sha512)
    End Sub

    Private Sub TextRadioSHA1_CheckedChanged(sender As Object, e As EventArgs) Handles textRadioSHA1.CheckedChanged
        FillInNewHash(ChecksumType.sha160)
    End Sub

    Private Sub TextRadioMD5_CheckedChanged(sender As Object, e As EventArgs) Handles textRadioMD5.CheckedChanged
        FillInNewHash(ChecksumType.md5)
    End Sub

    Private Sub TabControl1_Selecting(sender As Object, e As TabControlCancelEventArgs) Handles TabControl1.Selecting
        If e.TabPageIndex = TabNumberSettingsTab AndAlso intCurrentlyActiveTab <> TabNumberNull AndAlso Not TabControl1.TabPages(intCurrentlyActiveTab).Text.Contains("Currently Active") Then
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

        If boolBackgroundThreadWorking AndAlso MsgBox("Checksum hashes are being computed, are you sure you want to abort?", MsgBoxStyle.YesNo + MsgBoxStyle.Question + MsgBoxStyle.DefaultButton2, strMessageBoxTitleText) = MsgBoxResult.No Then
            e.Cancel = True
            Exit Sub
        Else
            If workingThread IsNot Nothing Then
                workingThread.Abort()
                boolBackgroundThreadWorking = False
            End If
        End If
    End Sub

    Private Sub Form1_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        If boolBackgroundThreadWorking AndAlso MsgBox("Background tasks are being processed, are you sure you want to abort and exit the program?", MsgBoxStyle.YesNo + MsgBoxStyle.Question + MsgBoxStyle.DefaultButton2, strMessageBoxTitleText) = MsgBoxResult.No Then
            e.Cancel = True
            Exit Sub
        Else
            boolClosingWindow = True

            If PipeServer IsNot Nothing Then
                PipeServer.Disconnect()
                PipeServer.Close()
            End If

            If workingThread IsNot Nothing Then workingThread.Abort()

            My.Settings.windowLocation = Location
        End If
    End Sub

    Private Sub ListFiles_ColumnClick(sender As Object, e As ColumnClickEventArgs) Handles listFiles.ColumnClick
        If boolBackgroundThreadWorking Then Exit Sub ' Disable resorting the list while the program is working in the background.

        ' Get the new sorting column.
        Dim new_sorting_column As ColumnHeader = listFiles.Columns(e.Column)

        ' Figure out the new sorting order.
        Dim sort_order As SortOrder

        If m_SortingColumn2 Is Nothing Then
            ' New column. Sort ascending.
            sort_order = SortOrder.Ascending
        Else
            ' See if this is the same column.
            If new_sorting_column.Equals(m_SortingColumn2) Then
                ' Same column. Switch the sort order.
                sort_order = If(m_SortingColumn2.Text.StartsWith("> "), SortOrder.Descending, SortOrder.Ascending)
            Else
                ' New column. Sort ascending.
                sort_order = SortOrder.Ascending
            End If

            ' Remove the old sort indicator.
            m_SortingColumn2.Text = m_SortingColumn2.Text.Substring(2)
        End If

        ' Display the new sort order.
        m_SortingColumn2 = new_sorting_column
        m_SortingColumn2.Text = If(sort_order = SortOrder.Ascending, "> " & m_SortingColumn2.Text, "< " & m_SortingColumn2.Text)

        ' Create a comparer.
        listFiles.ListViewItemSorter = New ListViewComparer(e.Column, sort_order)

        ' Sort.
        listFiles.Sort()
    End Sub

    Private Sub ContextMenuStrip1_Opening(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles listFilesContextMenu.Opening
        If listFiles.SelectedItems.Count = 0 Then
            e.Cancel = True
        Else
            If String.IsNullOrWhiteSpace(DirectCast(listFiles.SelectedItems(0), MyListViewItem).Hash) Then e.Cancel = True

            If listFiles.SelectedItems.Count = 1 Then
                CopyHashToClipboardToolStripMenuItem.Text = " Copy Selected Hash to Clipboard"
            ElseIf listFiles.SelectedItems.Count > 1 Then
                CopyHashToClipboardToolStripMenuItem.Text = " Copy Selected Hashes to Clipboard"
            End If
        End If
    End Sub

    Private Sub AddHashFileHeader(ByRef stringBuilder As Text.StringBuilder, intFileCount As Integer)
        stringBuilder.AppendLine("'")
        stringBuilder.AppendLine("' Hash file generated by Hasher, version " & Check_for_Update_Stuff.versionString & ". Written by Tom Parkison.")
        stringBuilder.AppendLine("' Web Site: https://www.toms-world.org/blog/hasher")
        stringBuilder.AppendLine("' Source Code Available At: https://bitbucket.org/trparky/hasher")
        stringBuilder.AppendLine("'")
        stringBuilder.AppendLine("' Number of files in hash data: " & MyToString(intFileCount))
        stringBuilder.AppendLine("'")
    End Sub

    Private Sub CopyHashToClipboardToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CopyHashToClipboardToolStripMenuItem.Click
        If listFiles.SelectedItems.Count = 1 Then
            Dim selectedItem As MyListViewItem = listFiles.SelectedItems(0)
            If CopyTextToWindowsClipboard(If(chkDisplayHashesInUpperCase.Checked, selectedItem.Hash.ToUpper, selectedItem.Hash.ToLower)) Then MsgBox("The hash result has been copied to the Windows Clipboard.", MsgBoxStyle.Information, strMessageBoxTitleText)
        Else
            Dim stringBuilder As New Text.StringBuilder
            AddHashFileHeader(stringBuilder, listFiles.SelectedItems.Count)

            For Each item As MyListViewItem In listFiles.SelectedItems
                stringBuilder.AppendLine(If(chkDisplayHashesInUpperCase.Checked, item.Hash.ToUpper, item.Hash.ToLower) & " *" & item.FileName)
            Next

            If CopyTextToWindowsClipboard(stringBuilder.ToString.Trim) Then MsgBox("The hash result has been copied to the Windows Clipboard.", MsgBoxStyle.Information, strMessageBoxTitleText)
        End If
    End Sub

    Private Sub ApplyFileSizeSortingToHashList()
        MyInvoke(Sub()
                     colFileName.Text = "File Name"
                     colFileSize.Text = "File Size"
                     colComputeTime.Text = "Compute Time"

                     If radioMD5.Checked Then
                         colChecksum.Text = strColumnTitleChecksumMD5
                     ElseIf radioSHA1.Checked Then
                         colChecksum.Text = strColumnTitleChecksumSHA160
                     ElseIf radioSHA256.Checked Then
                         colChecksum.Text = strColumnTitleChecksumSHA256
                     ElseIf radioSHA384.Checked Then
                         colChecksum.Text = strColumnTitleChecksumSHA384
                     ElseIf radioSHA512.Checked Then
                         colChecksum.Text = strColumnTitleChecksumSHA512
                     End If

                     Dim new_sorting_column As ColumnHeader = listFiles.Columns(1)
                     Dim sort_order As SortOrder = SortOrder.Ascending

                     m_SortingColumn2 = new_sorting_column
                     m_SortingColumn2.Text = "> File Size"

                     listFiles.ListViewItemSorter = New ListViewComparer(1, sort_order)
                     listFiles.Sort()
                 End Sub)
    End Sub

    Private Sub ApplyFileSizeSortingToVerifyList()
        colFile.Text = "File Name"
        colFileSize2.Text = "File Size"
        colResults.Text = "Results"
        colComputeTime2.Text = "Compute Time"

        Dim new_sorting_column As ColumnHeader = verifyHashesListFiles.Columns(1)
        Dim sort_order As SortOrder = SortOrder.Ascending

        m_SortingColumn1 = new_sorting_column
        m_SortingColumn1.Text = "> File Size"

        verifyHashesListFiles.ListViewItemSorter = New ListViewComparer(1, sort_order)
        verifyHashesListFiles.Sort()
    End Sub

    Private Sub VerifyHashesListFiles_ColumnClick(sender As Object, e As ColumnClickEventArgs) Handles verifyHashesListFiles.ColumnClick
        If boolBackgroundThreadWorking Then Exit Sub ' Disable resorting the list while the program is working in the background.

        ' Get the new sorting column.
        Dim new_sorting_column As ColumnHeader = verifyHashesListFiles.Columns(e.Column)

        ' Figure out the new sorting order.
        Dim sort_order As SortOrder

        If m_SortingColumn1 Is Nothing Then
            ' New column. Sort ascending.
            sort_order = SortOrder.Ascending
        Else
            ' See if this is the same column.
            If new_sorting_column.Equals(m_SortingColumn1) Then
                ' Same column. Switch the sort order.
                sort_order = If(m_SortingColumn1.Text.StartsWith("> "), SortOrder.Descending, SortOrder.Ascending)
            Else
                ' New column. Sort ascending.
                sort_order = SortOrder.Ascending
            End If

            ' Remove the old sort indicator.
            m_SortingColumn1.Text = m_SortingColumn1.Text.Substring(2)
        End If

        ' Display the new sort order.
        m_SortingColumn1 = new_sorting_column
        m_SortingColumn1.Text = If(sort_order = SortOrder.Ascending, "> " & m_SortingColumn1.Text, "< " & m_SortingColumn1.Text)

        ' Create a comparer.
        verifyHashesListFiles.ListViewItemSorter = New ListViewComparer(e.Column, sort_order)

        ' Sort.
        verifyHashesListFiles.Sort()
    End Sub

    Private Sub BtnCheckForUpdates_Click(sender As Object, e As EventArgs) Handles btnCheckForUpdates.Click
        Threading.ThreadPool.QueueUserWorkItem(Sub()
                                                   Dim checkForUpdatesClassObject As New Check_for_Update_Stuff(Me)
                                                   checkForUpdatesClassObject.CheckForUpdates()
                                               End Sub)
    End Sub

    Private Sub BtnCompareFiles_Click(sender As Object, e As EventArgs) Handles btnCompareFiles.Click
        compareRadioSHA512.Checked = True
        If btnCompareFiles.Text = "Abort Processing" Then
            If MsgBox("Are you sure you want to abort processing?", MsgBoxStyle.Question + MsgBoxStyle.YesNo + MsgBoxStyle.DefaultButton2, strMessageBoxTitleText) = MsgBoxResult.Yes Then
                If workingThread IsNot Nothing Then
                    workingThread.Abort()
                    boolBackgroundThreadWorking = False
                End If

                Exit Sub
            Else
                Exit Sub
            End If
        End If

        If txtFile1.Text.Equals(txtFile2.Text, StringComparison.OrdinalIgnoreCase) Then
            MsgBox("Please select two different files.", MsgBoxStyle.Information, strMessageBoxTitleText)
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

        SyncLock threadLockingObject
            longAllBytes = 0
            longAllReadBytes = 0

            longAllBytes += New IO.FileInfo(txtFile1.Text).Length
            longAllBytes += New IO.FileInfo(txtFile2.Text).Length
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
                                                     Dim checksumType As ChecksumType

                                                     MyInvoke(Sub()
                                                                  compareRadioMD5.Enabled = False
                                                                  compareRadioSHA1.Enabled = False
                                                                  compareRadioSHA256.Enabled = False
                                                                  compareRadioSHA384.Enabled = False
                                                                  compareRadioSHA512.Enabled = False
                                                                  CompareFilesAllFilesProgress.Visible = True

                                                                  If compareRadioMD5.Checked Then
                                                                      checksumType = ChecksumType.md5
                                                                  ElseIf compareRadioSHA1.Checked Then
                                                                      checksumType = ChecksumType.sha160
                                                                  ElseIf compareRadioSHA256.Checked Then
                                                                      checksumType = ChecksumType.sha256
                                                                  ElseIf compareRadioSHA384.Checked Then
                                                                      checksumType = ChecksumType.sha384
                                                                  ElseIf compareRadioSHA512.Checked Then
                                                                      checksumType = ChecksumType.sha512
                                                                  End If
                                                              End Sub)

                                                     Dim strChecksum1 As String = Nothing
                                                     Dim strChecksum2 As String = Nothing
                                                     Dim boolSuccessful As Boolean = False
                                                     Dim percentage, allBytesPercentage As Double
                                                     Dim subRoutine As [Delegate] = Sub(size As Long, totalBytesRead As Long)
                                                                                        Try
                                                                                            MyInvoke(Sub()
                                                                                                         percentage = If(totalBytesRead = 0 Or size = 0, 0, totalBytesRead / size * 100) ' This fixes a possible divide by zero exception.
                                                                                                         compareFilesProgressBar.Value = percentage
                                                                                                         SyncLock threadLockingObject
                                                                                                             allBytesPercentage = longAllReadBytes / longAllBytes * 100
                                                                                                         End SyncLock
                                                                                                         ProgressForm.SetTaskbarProgressBarValue(allBytesPercentage)
                                                                                                         CompareFilesAllFilesProgress.Value = allBytesPercentage
                                                                                                         lblCompareFilesStatus.Text = FileSizeToHumanSize(totalBytesRead) & " of " & FileSizeToHumanSize(size) & " (" & MyRoundingFunction(percentage, byteRoundPercentages) & "%) have been processed."
                                                                                                         lblCompareFilesAllFilesStatus.Text = FileSizeToHumanSize(longAllReadBytes) & " of " & FileSizeToHumanSize(longAllBytes) & " (" & MyRoundingFunction(allBytesPercentage, byteRoundPercentages) & "%) have been processed."
                                                                                                         If chkShowPercentageInWindowTitleBar.Checked Then Text = strWindowTitle & " (" & MyRoundingFunction(allBytesPercentage, byteRoundPercentages) & "% Completed)"
                                                                                                     End Sub)
                                                                                        Catch ex As Exception
                                                                                        End Try
                                                                                    End Sub

                                                     Dim stopWatch As Stopwatch = Stopwatch.StartNew

                                                     If DoChecksumWithAttachedSubRoutine(txtFile1.Text, compareFilesAllTheHashes1, subRoutine) AndAlso DoChecksumWithAttachedSubRoutine(txtFile2.Text, compareFilesAllTheHashes2, subRoutine) Then
                                                         boolSuccessful = True

                                                         If checksumType = ChecksumType.md5 Then
                                                             strChecksum1 = compareFilesAllTheHashes1.Md5
                                                             strChecksum2 = compareFilesAllTheHashes2.Md5
                                                         ElseIf checksumType = ChecksumType.sha160 Then
                                                             strChecksum1 = compareFilesAllTheHashes1.Sha160
                                                             strChecksum2 = compareFilesAllTheHashes2.Sha160
                                                         ElseIf checksumType = ChecksumType.sha256 Then
                                                             strChecksum1 = compareFilesAllTheHashes1.Sha256
                                                             strChecksum2 = compareFilesAllTheHashes2.Sha256
                                                         ElseIf checksumType = ChecksumType.sha384 Then
                                                             strChecksum1 = compareFilesAllTheHashes1.Sha384
                                                             strChecksum2 = compareFilesAllTheHashes2.Sha384
                                                         ElseIf checksumType = ChecksumType.sha512 Then
                                                             strChecksum1 = compareFilesAllTheHashes1.Sha512
                                                             strChecksum2 = compareFilesAllTheHashes2.Sha512
                                                         End If

                                                         MyInvoke(Sub()
                                                                      lblFile1Hash.Text = "Hash/Checksum: " & If(chkDisplayHashesInUpperCase.Checked, strChecksum1.ToUpper, strChecksum1.ToLower)
                                                                      ToolTip.SetToolTip(lblFile1Hash, strChecksum1)

                                                                      lblFile2Hash.Text = "Hash/Checksum: " & If(chkDisplayHashesInUpperCase.Checked, strChecksum2.ToUpper, strChecksum2.ToLower)
                                                                      ToolTip.SetToolTip(lblFile2Hash, strChecksum2)
                                                                  End Sub)
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
                                                                          MsgBox("Both files are the same." & vbCrLf & vbCrLf & "Processing completed in " & TimespanToHMS(stopWatch.Elapsed) & ".", MsgBoxStyle.Information, strMessageBoxTitleText)
                                                                      Else
                                                                          pictureBoxCompareFiles.Image = My.Resources.bad_check
                                                                          ToolTip.SetToolTip(pictureBoxCompareFiles, "The two files don't match.")
                                                                          MsgBox("The two files don't match." & vbCrLf & vbCrLf & "Processing completed in " & TimespanToHMS(stopWatch.Elapsed) & ".", MsgBoxStyle.Critical, strMessageBoxTitleText)
                                                                      End If
                                                                  Else
                                                                      MsgBox("There was an error while calculating the checksum.", MsgBoxStyle.Critical, strMessageBoxTitleText)
                                                                  End If

                                                                  boolBackgroundThreadWorking = False
                                                                  workingThread = Nothing
                                                              End Sub)
                                                 Catch ex As Threading.ThreadAbortException
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
                                                              End Sub)
                                                 Finally
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

        OpenFileDialog.Title = "Select file #1 to be compared..."
        OpenFileDialog.Multiselect = False
        OpenFileDialog.Filter = "Show All Files|*.*"

        If OpenFileDialog.ShowDialog() = DialogResult.OK Then txtFile1.Text = OpenFileDialog.FileName
    End Sub

    Private Sub BtnCompareFilesBrowseFile2_Click(sender As Object, e As EventArgs) Handles btnCompareFilesBrowseFile2.Click
        lblFile2Hash.Text = strHashChecksumToBeComputed
        pictureBoxCompareFiles.Image = Nothing
        ToolTip.SetToolTip(pictureBoxCompareFiles, "")
        ToolTip.SetToolTip(lblFile2Hash, "")

        OpenFileDialog.Title = "Select file #2 to be compared..."
        OpenFileDialog.Multiselect = False
        OpenFileDialog.Filter = "Show All Files|*.*"

        If OpenFileDialog.ShowDialog() = DialogResult.OK Then txtFile2.Text = OpenFileDialog.FileName
    End Sub

    Private Sub ChkSSL_Click(sender As Object, e As EventArgs) Handles chkSSL.Click
        My.Settings.boolSSL = chkSSL.Checked
    End Sub

    Private Sub BtnBrowseFileForCompareKnownHash_Click(sender As Object, e As EventArgs) Handles btnBrowseFileForCompareKnownHash.Click
        If boolDidWePerformAPreviousHash Then txtKnownHash.Text = Nothing
        boolDidWePerformAPreviousHash = False
        OpenFileDialog.Title = "Select file for known hash comparison..."
        OpenFileDialog.Multiselect = False
        OpenFileDialog.Filter = "Show All Files|*.*"

        If OpenFileDialog.ShowDialog() = DialogResult.OK Then
            txtFileForKnownHash.Text = OpenFileDialog.FileName
            If Not String.IsNullOrWhiteSpace(txtKnownHash.Text) Then btnCompareAgainstKnownHash.Enabled = True
            txtKnownHash.Select()
        End If
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

                If txtKnownHash.Text.Length = 32 Then
                    lblCompareFileAgainstKnownHashType.Text = "Hash Type: MD5"
                ElseIf txtKnownHash.Text.Length = 40 Then
                    lblCompareFileAgainstKnownHashType.Text = "Hash Type: SHA1"
                ElseIf txtKnownHash.Text.Length = 64 Then
                    lblCompareFileAgainstKnownHashType.Text = "Hash Type: SHA256"
                ElseIf txtKnownHash.Text.Length = 96 Then
                    lblCompareFileAgainstKnownHashType.Text = "Hash Type: SHA384"
                ElseIf txtKnownHash.Text.Length = 128 Then
                    lblCompareFileAgainstKnownHashType.Text = "Hash Type: SHA512"
                End If
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
            If MsgBox("Are you sure you want to abort processing?", MsgBoxStyle.Question + MsgBoxStyle.YesNo + MsgBoxStyle.DefaultButton2, strMessageBoxTitleText) = MsgBoxResult.Yes Then
                If workingThread IsNot Nothing Then
                    workingThread.Abort()
                    boolBackgroundThreadWorking = False
                End If

                Exit Sub
            Else
                Exit Sub
            End If
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
                                                     boolBackgroundThreadWorking = True
                                                     Dim checksumType As ChecksumType

                                                     If txtKnownHash.Text.Length = 32 Then
                                                         checksumType = ChecksumType.md5
                                                     ElseIf txtKnownHash.Text.Length = 40 Then
                                                         checksumType = ChecksumType.sha160
                                                     ElseIf txtKnownHash.Text.Length = 64 Then
                                                         checksumType = ChecksumType.sha256
                                                     ElseIf txtKnownHash.Text.Length = 96 Then
                                                         checksumType = ChecksumType.sha384
                                                     ElseIf txtKnownHash.Text.Length = 128 Then
                                                         checksumType = ChecksumType.sha512
                                                     End If

                                                     Dim strChecksum As String = Nothing
                                                     Dim percentage As Double
                                                     Dim subRoutine As [Delegate] = Sub(size As Long, totalBytesRead As Long)
                                                                                        Try
                                                                                            MyInvoke(Sub()
                                                                                                         percentage = If(totalBytesRead = 0 Or size = 0, 0, totalBytesRead / size * 100) ' This fixes a possible divide by zero exception.
                                                                                                         compareAgainstKnownHashProgressBar.Value = percentage
                                                                                                         ProgressForm.SetTaskbarProgressBarValue(compareAgainstKnownHashProgressBar.Value)
                                                                                                         lblCompareAgainstKnownHashStatus.Text = FileSizeToHumanSize(totalBytesRead) & " of " & FileSizeToHumanSize(size) & " (" & MyRoundingFunction(percentage, byteRoundPercentages) & "%) have been processed."
                                                                                                         If chkShowPercentageInWindowTitleBar.Checked Then Text = strWindowTitle & " (" & MyRoundingFunction(percentage, byteRoundPercentages) & "% Completed)"
                                                                                                     End Sub)
                                                                                        Catch ex As Exception
                                                                                        End Try
                                                                                    End Sub

                                                     Dim stopWatch As Stopwatch = Stopwatch.StartNew
                                                     Dim allTheHashes As AllTheHashes = Nothing
                                                     Dim boolSuccessful As Boolean = DoChecksumWithAttachedSubRoutine(txtFileForKnownHash.Text, allTheHashes, subRoutine)
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
                                                                          MsgBox("The checksums match!" & vbCrLf & vbCrLf & "Processing completed in " & TimespanToHMS(stopWatch.Elapsed) & ".", MsgBoxStyle.Information, strMessageBoxTitleText)
                                                                      Else
                                                                          pictureBoxVerifyAgainstResults.Image = Global.Hasher.My.Resources.Resources.bad_check
                                                                          ToolTip.SetToolTip(pictureBoxVerifyAgainstResults, "Checksum verification failed, checksum didn't match!")
                                                                          MsgBox("The checksums DON'T match!" & vbCrLf & vbCrLf & "Processing completed in " & TimespanToHMS(stopWatch.Elapsed) & ".", MsgBoxStyle.Critical, strMessageBoxTitleText)
                                                                      End If
                                                                  Else
                                                                      pictureBoxVerifyAgainstResults.Image = Global.Hasher.My.Resources.Resources.bad_check
                                                                      MsgBox("There was an error while calculating the checksum.", MsgBoxStyle.Critical, strMessageBoxTitleText)
                                                                  End If

                                                                  boolBackgroundThreadWorking = False
                                                                  workingThread = Nothing
                                                              End Sub)
                                                 Catch ex As Threading.ThreadAbortException
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
                                                              End Sub)
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
    End Sub

    Private Sub Form1_Resize(sender As Object, e As EventArgs) Handles Me.Resize
        If boolDoneLoading Then My.Settings.boolWindowMaximized = WindowState = FormWindowState.Maximized
    End Sub

    Private Function GetHashOfString(inputString As String, hashType As ChecksumType) As String
        Using HashAlgorithm As Security.Cryptography.HashAlgorithm = Checksums.GetHashEngine(hashType)
            Dim byteOutput As Byte() = HashAlgorithm.ComputeHash(System.Text.Encoding.UTF8.GetBytes(inputString))
            Return BitConverter.ToString(byteOutput).ToLower().Replace("-", "")
        End Using
    End Function

    Private Function GetHashOfString(inputString As String) As AllTheHashes
        Dim md5Engine As Security.Cryptography.HashAlgorithm = Checksums.GetHashEngine(ChecksumType.md5)
        Dim sha160Engine As Security.Cryptography.HashAlgorithm = Checksums.GetHashEngine(ChecksumType.sha160)
        Dim sha256Engine As Security.Cryptography.HashAlgorithm = Checksums.GetHashEngine(ChecksumType.sha256)
        Dim sha384Engine As Security.Cryptography.HashAlgorithm = Checksums.GetHashEngine(ChecksumType.sha384)
        Dim sha512Engine As Security.Cryptography.HashAlgorithm = Checksums.GetHashEngine(ChecksumType.sha512)
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

        md5Engine.Dispose()
        sha160Engine.Dispose()
        sha256Engine.Dispose()
        sha384Engine.Dispose()
        sha512Engine.Dispose()

        Return allTheHashes
    End Function

    Private Sub ListFiles_ColumnWidthChanged(sender As Object, e As ColumnWidthChangedEventArgs) Handles listFiles.ColumnWidthChanged
        If Not boolDoneLoading Then Exit Sub
        My.Settings.hashIndividualFilesFileNameColumnSize = colFileName.Width
        My.Settings.hashIndividualFilesFileSizeColumnSize = colFileSize.Width
        My.Settings.hashIndividualFilesChecksumColumnSize = colChecksum.Width
        My.Settings.hashIndividualFilesComputeTimeColumnSize = colComputeTime.Width
    End Sub

    Private Sub VerifyHashesListFiles_ColumnWidthChanged(sender As Object, e As ColumnWidthChangedEventArgs) Handles verifyHashesListFiles.ColumnWidthChanged
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
        If AreWeAnAdministrator() Then
            FileAssociation.SelfCreateAssociation(".md5", "Checksum File")
            FileAssociation.SelfCreateAssociation(".sha1", "Checksum File")
            FileAssociation.SelfCreateAssociation(".sha256", "Checksum File")
            FileAssociation.SelfCreateAssociation(".sha384", "Checksum File")
            FileAssociation.SelfCreateAssociation(".sha512", "Checksum File")
        Else
            Dim startInfo As New ProcessStartInfo With {
                .FileName = Application.ExecutablePath,
                .Arguments = "-associatefiletype",
                .Verb = "runas"
            }
            Dim process As Process = Process.Start(startInfo)
            process.WaitForExit()
        End If

        MsgBox("File association complete.", MsgBoxStyle.Information, strMessageBoxTitleText)
    End Sub

    Private Sub BtnAddHasherToAllFiles_Click(sender As Object, e As EventArgs) Handles btnAddHasherToAllFiles.Click
        If AreWeAnAdministrator() Then
            FileAssociation.AddAssociationWithAllFiles()
        Else
            Dim startInfo As New ProcessStartInfo With {
                .FileName = Application.ExecutablePath,
                .Arguments = "-associateallfiles",
                .Verb = "runas"
            }
            Dim process As Process = Process.Start(startInfo)
            process.WaitForExit()
        End If

        MsgBox("File association complete.", MsgBoxStyle.Information, strMessageBoxTitleText)
    End Sub

    Private Sub BtnOpenExistingHashFile_DragDrop(sender As Object, e As DragEventArgs) Handles btnOpenExistingHashFile.DragDrop
        Dim receivedData As String() = DirectCast(e.Data.GetData(DataFormats.FileDrop), String())
        If receivedData.Count = 1 Then
            Dim strReceivedFileName As String = receivedData(0)
            Dim fileInfo As New IO.FileInfo(strReceivedFileName)

            If fileInfo.Extension.Equals(".md5", StringComparison.OrdinalIgnoreCase) Or fileInfo.Extension.Equals(".sha1", StringComparison.OrdinalIgnoreCase) Or fileInfo.Extension.Equals(".sha256", StringComparison.OrdinalIgnoreCase) Or fileInfo.Extension.Equals(".sha384", StringComparison.OrdinalIgnoreCase) Or fileInfo.Extension.Equals(".sha512", StringComparison.OrdinalIgnoreCase) Then
                btnOpenExistingHashFile.Text = "Abort Processing"
                verifyHashesListFiles.Items.Clear()
                ProcessExistingHashFile(strReceivedFileName)
            Else
                MsgBox("Invalid file type.", MsgBoxStyle.Critical, strMessageBoxTitleText)
            End If
        End If
    End Sub

    Private Sub ListFiles_ItemSelectionChanged(sender As Object, e As ListViewItemSelectionChangedEventArgs) Handles listFiles.ItemSelectionChanged
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

    Private Sub WaitForConnectionCallBack(ByVal iar As IAsyncResult)
        Try
            Dim namedPipeServer As NamedPipeServerStream = CType(iar.AsyncState, NamedPipeServerStream)
            namedPipeServer.EndWaitForConnection(iar)
            Dim buffer As Byte() = New Byte(499) {}
            namedPipeServer.Read(buffer, 0, 500)

            Dim strReceivedMessage As String = System.Text.Encoding.UTF8.GetString(buffer, 0, buffer.Length).Replace(vbNullChar, "").Trim

            If strReceivedMessage.StartsWith("--comparefile=", StringComparison.OrdinalIgnoreCase) Then
                MyInvoke(Sub()
                             Dim strFilePathToBeCompared As String = strReceivedMessage.CaseInsensitiveReplace("--comparefile=", "")

                             If String.IsNullOrWhiteSpace(txtFile1.Text) And String.IsNullOrWhiteSpace(txtFile2.Text) Then
                                 txtFile1.Text = strFilePathToBeCompared
                             ElseIf String.IsNullOrWhiteSpace(txtFile1.Text) And Not String.IsNullOrWhiteSpace(txtFile2.Text) Then
                                 txtFile1.Text = strFilePathToBeCompared
                             ElseIf Not String.IsNullOrWhiteSpace(txtFile1.Text) And String.IsNullOrWhiteSpace(txtFile2.Text) Then
                                 txtFile2.Text = strFilePathToBeCompared
                             End If

                             TabControl1.SelectedIndex = TabNumberCompareFilesTab
                             If Not String.IsNullOrWhiteSpace(txtFile1.Text) AndAlso Not String.IsNullOrWhiteSpace(txtFile2.Text) Then btnCompareFiles.PerformClick()
                         End Sub)
            ElseIf strReceivedMessage.StartsWith("--addfile=", StringComparison.OrdinalIgnoreCase) Then
                AddFileOrDirectoryToHashFileList(strReceivedMessage.CaseInsensitiveReplace("--addfile=", ""))
            End If

            namedPipeServer.Dispose()
            namedPipeServer = New NamedPipeServerStream(strNamedPipeServerName, PipeDirection.In, 1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous)
            namedPipeServer.BeginWaitForConnection(New AsyncCallback(AddressOf WaitForConnectionCallBack), namedPipeServer)
        Catch
            Return
        End Try
    End Sub

    Private Sub ChkUseMilliseconds_Click(sender As Object, e As EventArgs) Handles chkUseMilliseconds.Click
        My.Settings.boolUseMilliseconds = chkUseMilliseconds.Checked
    End Sub

    Private Sub ChkDisplayHashesInUpperCase_Click(sender As Object, e As EventArgs) Handles chkDisplayHashesInUpperCase.Click
        Dim boolDisplayHashesInUpperCase As Boolean = chkDisplayHashesInUpperCase.Checked
        My.Settings.boolDisplayHashesInUpperCase = chkDisplayHashesInUpperCase.Checked

        If listFiles.Items.Count <> 0 Then
            For Each item As MyListViewItem In listFiles.Items
                item.SubItems(2).Text = If(boolDisplayHashesInUpperCase, item.Hash.ToUpper, item.Hash.ToLower)
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
                MsgBox("Color preferences will not be used until the next time a checksum file is processed in the ""Verify Saved Hashes"" tab.", MsgBoxStyle.Information, strMessageBoxTitleText)
            End If
        End Using
    End Sub

    Private Sub BtnSetNotValidColor_Click(sender As Object, e As EventArgs) Handles btnSetNotValidColor.Click
        Using colorDialog As New ColorDialog() With {.Color = notValidColor}
            If colorDialog.ShowDialog() = DialogResult.OK Then
                My.Settings.notValidColor = colorDialog.Color
                lblNotValidColor.BackColor = colorDialog.Color
                notValidColor = colorDialog.Color
                MsgBox("Color preferences will not be used until the next time a checksum file is processed in the ""Verify Saved Hashes"" tab.", MsgBoxStyle.Information, strMessageBoxTitleText)
            End If
        End Using
    End Sub

    Private Sub BtnFileNotFoundColor_Click(sender As Object, e As EventArgs) Handles btnFileNotFoundColor.Click
        Using colorDialog As New ColorDialog() With {.Color = fileNotFoundColor}
            If colorDialog.ShowDialog() = DialogResult.OK Then
                My.Settings.fileNotFoundColor = colorDialog.Color
                lblFileNotFoundColor.BackColor = colorDialog.Color
                fileNotFoundColor = colorDialog.Color
                MsgBox("Color preferences will not be used until the next time a checksum file is processed in the ""Verify Saved Hashes"" tab.", MsgBoxStyle.Information, strMessageBoxTitleText)
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

        MsgBox("Color preferences will not be used until the next time a checksum file is processed in the ""Verify Saved Hashes"" tab.", MsgBoxStyle.Information, strMessageBoxTitleText)
    End Sub

    Private Sub BtnSetBufferSize_Click(sender As Object, e As EventArgs) Handles btnSetBufferSize.Click
        Dim shortBufferSize As Short = Decimal.ToInt16(bufferSize.Value)
        intBufferSize = shortBufferSize * 1024 * 1024
        My.Settings.shortBufferSize = shortBufferSize
        btnSetBufferSize.Enabled = False
        MsgBox("Data buffer size set successfully to " & shortBufferSize & If(shortBufferSize = 1, " MB.", " MBs."), MsgBoxStyle.Information, strMessageBoxTitleText)
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
                MsgBox("Data buffer size set successfully to " & benchmark.shortBufferSize & If(benchmark.shortBufferSize = 1, " MB.", " MBs."), MsgBoxStyle.Information, strMessageBoxTitleText)
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
    End Sub

    Private Sub ChkAutoAddExtension_Click(sender As Object, e As EventArgs) Handles chkAutoAddExtension.Click
        If Not chkAutoAddExtension.Checked AndAlso MsgBox("You are disabling a highly recommended option, it is HIGHLY recommended that you re-enable this option to prevent accidental data loss." & vbCrLf & vbCrLf & "Are you sure you want to do this?", MsgBoxStyle.Critical + MsgBoxStyle.YesNo + MsgBoxStyle.DefaultButton2, strMessageBoxTitleText) = MsgBoxResult.No Then
            chkAutoAddExtension.Checked = True
        End If
        My.Settings.boolAutoAddExtension = chkAutoAddExtension.Checked
    End Sub

    Private Sub FillInChecksumLabelsOnCompareFilesTab(checksumType As ChecksumType)
        Dim strChecksum1 As String = GetDataFromAllTheHashes(checksumType, compareFilesAllTheHashes1)
        Dim strChecksum2 As String = GetDataFromAllTheHashes(checksumType, compareFilesAllTheHashes2)

        If Not String.IsNullOrWhiteSpace(strChecksum1) AndAlso Not String.IsNullOrWhiteSpace(strChecksum2) Then
            lblFile1Hash.Text = "Hash/Checksum: " & If(chkDisplayHashesInUpperCase.Checked, strChecksum1.ToUpper, strChecksum1.ToLower)
            lblFile2Hash.Text = "Hash/Checksum: " & If(chkDisplayHashesInUpperCase.Checked, strChecksum2.ToUpper, strChecksum2.ToLower)
            ToolTip.SetToolTip(lblFile1Hash, strChecksum1)
            ToolTip.SetToolTip(lblFile2Hash, strChecksum2)
        End If
    End Sub

    Private Sub CompareRadioMD5_Click(sender As Object, e As EventArgs) Handles compareRadioMD5.Click
        FillInChecksumLabelsOnCompareFilesTab(ChecksumType.md5)
    End Sub

    Private Sub CompareRadioSHA1_Click(sender As Object, e As EventArgs) Handles compareRadioSHA1.Click
        FillInChecksumLabelsOnCompareFilesTab(ChecksumType.sha160)
    End Sub

    Private Sub CompareRadioSHA256_Click(sender As Object, e As EventArgs) Handles compareRadioSHA256.Click
        FillInChecksumLabelsOnCompareFilesTab(ChecksumType.sha256)
    End Sub

    Private Sub CompareRadioSHA384_Click(sender As Object, e As EventArgs) Handles compareRadioSHA384.Click
        FillInChecksumLabelsOnCompareFilesTab(ChecksumType.sha384)
    End Sub

    Private Sub BtnTransferToHashIndividualFilesTab_Click(sender As Object, e As EventArgs) Handles btnTransferToHashIndividualFilesTab.Click
        Threading.ThreadPool.QueueUserWorkItem(Sub()
                                                   MyInvoke(Sub()
                                                                btnTransferToHashIndividualFilesTab.Enabled = False
                                                                VerifyHashProgressBar.Visible = True
                                                                lblVerifyHashStatus.Visible = True
                                                                verifyHashesListFiles.Size = New Size(verifyHashesListFiles.Size.Width, verifyHashesListFiles.Size.Height - 72)

                                                                listFiles.Items.Clear()
                                                                filesInListFiles.Clear()
                                                                listFiles.BeginUpdate()
                                                            End Sub)

                                                   boolBackgroundThreadWorking = True
                                                   Dim listOfListViewItems As New List(Of MyListViewItem)
                                                   Dim intLineCounter As Integer = 0
                                                   Dim listViewItemCollection As ListView.ListViewItemCollection = GetListViewItems(verifyHashesListFiles)
                                                   Dim strHashString As String
                                                   Dim checksumType As ChecksumType

                                                   If My.Settings.defaultHash = ChecksumType.md5 Then
                                                       checksumType = ChecksumType.md5
                                                   ElseIf My.Settings.defaultHash = ChecksumType.sha160 Then
                                                       checksumType = ChecksumType.sha160
                                                   ElseIf My.Settings.defaultHash = ChecksumType.sha256 Then
                                                       checksumType = ChecksumType.sha256
                                                   ElseIf My.Settings.defaultHash = ChecksumType.sha384 Then
                                                       checksumType = ChecksumType.sha384
                                                   ElseIf My.Settings.defaultHash = ChecksumType.sha512 Then
                                                       checksumType = ChecksumType.sha512
                                                   End If

                                                   For Each item As MyListViewItem In listViewItemCollection
                                                       intLineCounter += 1
                                                       MyInvoke(Sub()
                                                                    VerifyHashProgressBar.Value = intLineCounter / listViewItemCollection.Count * 100
                                                                    ProgressForm.SetTaskbarProgressBarValue(VerifyHashProgressBar.Value)
                                                                    lblVerifyHashStatus.Text = "Processing item " & MyToString(intLineCounter) & " of " & MyToString(listViewItemCollection.Count) & " (" & VerifyHashProgressBar.Value & "%)."
                                                                End Sub)

                                                       If Not filesInListFiles.Contains(item.FileName.ToLower) And IO.File.Exists(item.FileName) Then
                                                           filesInListFiles.Add(item.FileName.ToLower)

                                                           Dim itemToBeAdded As New MyListViewItem(item.FileName) With {
                                                               .FileSize = New IO.FileInfo(item.FileName).Length,
                                                               .FileName = item.FileName
                                                           }
                                                           With itemToBeAdded
                                                               strHashString = GetDataFromAllTheHashes(checksumType, item.AllTheHashes)
                                                               .SubItems.Add(FileSizeToHumanSize(itemToBeAdded.FileSize))
                                                               .SubItems.Add(If(chkDisplayHashesInUpperCase.Checked, strHashString.ToUpper, strHashString.ToLower))
                                                               .SubItems.Add(TimespanToHMS(item.ComputeTime))
                                                               .AllTheHashes = item.AllTheHashes
                                                               .Hash = strHashString
                                                           End With

                                                           listOfListViewItems.Add(itemToBeAdded)
                                                       End If
                                                   Next

                                                   MyInvoke(Sub()
                                                                boolBackgroundThreadWorking = False
                                                                listFiles.Items.AddRange(listOfListViewItems.ToArray)
                                                                listOfListViewItems = Nothing

                                                                If chkSortFileListingAfterAddingFilesToHash.Checked Then ApplyFileSizeSortingToHashList()
                                                                listFiles.EndUpdate()
                                                                colChecksum.Text = strColumnTitleChecksumSHA256
                                                                TabControl1.SelectedIndex = TabNumberHashIndividualFilesTab
                                                                btnIndividualFilesCopyToClipboard.Enabled = True
                                                                btnIndividualFilesSaveResultsToDisk.Enabled = True

                                                                ProgressForm.SetTaskbarProgressBarValue(0)
                                                                VerifyHashProgressBar.Value = 0
                                                                lblVerifyHashStatus.Text = Nothing
                                                                listViewItemCollection = Nothing
                                                                btnTransferToHashIndividualFilesTab.Enabled = True
                                                                VerifyHashProgressBar.Visible = False
                                                                lblVerifyHashStatus.Visible = False
                                                                verifyHashesListFiles.Size = New Size(verifyHashesListFiles.Size.Width, verifyHashesListFiles.Size.Height + 72)
                                                            End Sub)
                                               End Sub)
    End Sub

    Private Sub CompareRadioSHA512_Click(sender As Object, e As EventArgs) Handles compareRadioSHA512.Click
        FillInChecksumLabelsOnCompareFilesTab(ChecksumType.sha512)
    End Sub

    Private Sub TxtFile1_TextChanged(sender As Object, e As EventArgs) Handles txtFile1.TextChanged
        If txtFile1.Text.Equals(txtFile2.Text, StringComparison.OrdinalIgnoreCase) Then
            MsgBox("You have selected the same file. Oops.", MsgBoxStyle.Critical, strMessageBoxTitleText)
            txtFile1.Text = Nothing
        End If
        btnCompareFiles.Enabled = Not String.IsNullOrEmpty(txtFile1.Text) And Not String.IsNullOrEmpty(txtFile2.Text)
        lblFile1Hash.Text = strHashChecksumToBeComputed
        pictureBoxCompareFiles.Image = Nothing
        ToolTip.SetToolTip(pictureBoxCompareFiles, "")
    End Sub

    Private Sub TxtFile2_TextChanged(sender As Object, e As EventArgs) Handles txtFile2.TextChanged
        If txtFile1.Text.Equals(txtFile2.Text, StringComparison.OrdinalIgnoreCase) Then
            MsgBox("You have selected the same file. Oops.", MsgBoxStyle.Critical, strMessageBoxTitleText)
            txtFile2.Text = Nothing
        End If
        btnCompareFiles.Enabled = Not String.IsNullOrEmpty(txtFile1.Text) And Not String.IsNullOrEmpty(txtFile2.Text)
        lblFile2Hash.Text = strHashChecksumToBeComputed
        pictureBoxCompareFiles.Image = Nothing
        ToolTip.SetToolTip(pictureBoxCompareFiles, "")
    End Sub

    Private Sub VerifyListFilesContextMenu_Opening(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles verifyListFilesContextMenu.Opening
        If verifyHashesListFiles.Items.Count = 0 Or verifyHashesListFiles.SelectedItems.Count = 0 Then
            e.Cancel = True
            Exit Sub
        Else
            If String.IsNullOrEmpty(verifyHashesListFiles.SelectedItems(0).SubItems(4).Text) Or workingThread IsNot Nothing Then
                e.Cancel = True
                Exit Sub
            ElseIf Not DirectCast(verifyHashesListFiles.SelectedItems(0), MyListViewItem).BoolFileExists Or DirectCast(verifyHashesListFiles.SelectedItems(0), MyListViewItem).BoolValidHash Then
                e.Cancel = True
                Exit Sub
            End If
        End If
    End Sub

    Private Sub ViewChecksumDifferenceToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ViewChecksumDifferenceToolStripMenuItem.Click
        Dim selectedItem As MyListViewItem = verifyHashesListFiles.SelectedItems(0)
        Dim stringBuilder As New System.Text.StringBuilder()

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
    End Sub

    Private Sub BtnRetestFailedFiles_Click(sender As Object, e As EventArgs) Handles btnRetestFailedFiles.Click
        verifyHashesListFiles.Size = New Size(verifyHashesListFiles.Size.Width, verifyHashesListFiles.Size.Height - 72)

        workingThread = New Threading.Thread(Sub()
                                                 Dim itemOnGUI As MyListViewItem = Nothing
                                                 Dim currentItem As MyListViewItem = Nothing

                                                 Try
                                                     boolBackgroundThreadWorking = True
                                                     Dim strChecksum, strFileName As String
                                                     Dim index As Integer = 1
                                                     Dim longFilesThatPassedVerification As Long = 0
                                                     Dim intFilesNotFound As Integer = 0
                                                     Dim intLineCounter As Integer = 0
                                                     Dim stopWatch As Stopwatch = Stopwatch.StartNew

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
                                                              End Sub)

                                                     SyncLock threadLockingObject
                                                         longAllReadBytes = 0
                                                         longAllBytes = 0
                                                     End SyncLock

                                                     MyInvoke(Sub()
                                                                  Text = strWindowTitle
                                                                  If chkSortByFileSizeAfterLoadingHashFile.Checked Then ApplyFileSizeSortingToVerifyList()
                                                                  VerifyHashProgressBar.Value = 0
                                                                  ProgressForm.SetTaskbarProgressBarValue(0)
                                                                  lblVerifyHashStatusProcessingFile.Visible = True
                                                              End Sub)

                                                     Dim items As ListView.ListViewItemCollection = GetListViewItems(verifyHashesListFiles)
                                                     Dim strChecksumInFile As String = Nothing
                                                     Dim percentage, allBytesPercentage As Double
                                                     Dim subRoutine As [Delegate]
                                                     Dim computeStopwatch As Stopwatch
                                                     Dim allTheHashes As AllTheHashes = Nothing
                                                     Dim strDisplayValidChecksumString As String = If(chkDisplayValidChecksumString.Checked, "Valid Checksum", "")
                                                     Dim intFileCount As Integer = 0

                                                     For Each item As MyListViewItem In items
                                                         MyInvoke(Sub() itemOnGUI = verifyHashesListFiles.Items(item.Index))
                                                         If Not item.BoolValidHash Then
                                                             If IO.File.Exists(item.FileName) Then
                                                                 item.BoolFileExists = True
                                                                 item.FileSize = New IO.FileInfo(item.FileName).Length
                                                                 item.SubItems(1).Text = FileSizeToHumanSize(item.FileSize)
                                                                 item.SubItems(2).Text = ""
                                                                 item.SubItems(3).Text = ""
                                                                 item.SubItems(4).Text = strWaitingToBeProcessed
                                                                 item.Color = Color.FromKnownColor(KnownColor.Window)

                                                                 MyInvoke(Sub() UpdateListViewItem(itemOnGUI, item, True))

                                                                 longAllBytes += item.FileSize
                                                                 intFileCount += 1
                                                             Else
                                                                 item.BoolFileExists = False
                                                             End If
                                                         End If
                                                     Next

                                                     If chkSortByFileSizeAfterLoadingHashFile.Checked Then MyInvoke(Sub() ApplyFileSizeSortingToVerifyList())
                                                     items.Clear()
                                                     items = GetListViewItems(verifyHashesListFiles)
                                                     index = 1

                                                     For Each item As MyListViewItem In items
                                                         currentItem = item
                                                         MyInvoke(Sub()
                                                                      itemOnGUI = verifyHashesListFiles.Items(item.Index)
                                                                      lblVerifyHashStatusProcessingFile.Text = GenerateProcessingFileString(index, intFileCount)
                                                                  End Sub)

                                                         If Not item.BoolValidHash Then
                                                             strChecksum = item.Hash
                                                             strFileName = item.FileName

                                                             If IO.File.Exists(strFileName) Then
                                                                 subRoutine = Sub(size As Long, totalBytesRead As Long)
                                                                                  Try
                                                                                      MyInvoke(Sub()
                                                                                                   percentage = If(totalBytesRead = 0 Or size = 0, 0, totalBytesRead / size * 100) ' This fixes a possible divide by zero exception.
                                                                                                   VerifyHashProgressBar.Value = percentage
                                                                                                   SyncLock threadLockingObject
                                                                                                       allBytesPercentage = longAllReadBytes / longAllBytes * 100
                                                                                                       lblVerifyHashesTotalStatus.Text = FileSizeToHumanSize(longAllReadBytes) & " of " & FileSizeToHumanSize(longAllBytes) & " (" & MyRoundingFunction(allBytesPercentage, byteRoundPercentages) & "%) have been processed."
                                                                                                       If chkShowPercentageInWindowTitleBar.Checked Then Text = strWindowTitle & " (" & MyRoundingFunction(allBytesPercentage, byteRoundPercentages) & "% Completed)"
                                                                                                   End SyncLock
                                                                                                   lblProcessingFileVerify.Text = FileSizeToHumanSize(totalBytesRead) & " of " & FileSizeToHumanSize(size) & " (" & MyRoundingFunction(percentage, byteRoundPercentages) & "%) have been processed."
                                                                                                   If chkShowFileProgressInFileList.Checked Then
                                                                                                       currentItem.SubItems(4).Text = lblProcessingFileVerify.Text
                                                                                                       itemOnGUI.SubItems(4) = currentItem.SubItems(4)
                                                                                                   End If
                                                                                                   ProgressForm.SetTaskbarProgressBarValue(allBytesPercentage)
                                                                                                   verifyIndividualFilesAllFilesProgressBar.Value = allBytesPercentage
                                                                                               End Sub)
                                                                                  Catch ex As Exception
                                                                                  End Try
                                                                              End Sub

                                                                 item.SubItems(4).Text = strCurrentlyBeingProcessed

                                                                 MyInvoke(Sub()
                                                                              lblVerifyHashStatus.Text = "Now processing file " & New IO.FileInfo(strFileName).Name & "."
                                                                              UpdateListViewItem(itemOnGUI, item)
                                                                          End Sub)

                                                                 computeStopwatch = Stopwatch.StartNew

                                                                 If DoChecksumWithAttachedSubRoutine(strFileName, allTheHashes, subRoutine) Then
                                                                     strChecksum = GetDataFromAllTheHashes(checksumTypeForChecksumCompareWindow, allTheHashes)
                                                                     item.AllTheHashes = allTheHashes

                                                                     If strChecksum.Equals(item.Hash, StringComparison.OrdinalIgnoreCase) Then
                                                                         item.Color = validColor
                                                                         item.SubItems(2).Text = "Valid Checksum"
                                                                         item.ComputeTime = computeStopwatch.Elapsed
                                                                         item.SubItems(3).Text = TimespanToHMS(item.ComputeTime)
                                                                         item.SubItems(4).Text = strDisplayValidChecksumString
                                                                         longFilesThatPassedVerification += 1
                                                                         item.BoolValidHash = True
                                                                     Else
                                                                         item.Color = notValidColor
                                                                         item.SubItems(2).Text = "Incorrect Checksum"
                                                                         item.ComputeTime = computeStopwatch.Elapsed
                                                                         item.SubItems(3).Text = TimespanToHMS(item.ComputeTime)
                                                                         item.SubItems(4).Text = If(chkDisplayHashesInUpperCase.Checked, GetDataFromAllTheHashes(checksumTypeForChecksumCompareWindow, allTheHashes).ToUpper, GetDataFromAllTheHashes(checksumTypeForChecksumCompareWindow, allTheHashes).ToLower)
                                                                         item.BoolValidHash = False
                                                                     End If
                                                                 Else
                                                                     item.Color = fileNotFoundColor
                                                                     item.SubItems(2).Text = "(Error while calculating checksum)"
                                                                     item.SubItems(4).Text = "(Error while calculating checksum)"
                                                                     item.BoolValidHash = False

                                                                     SyncLock threadLockingObject
                                                                         longAllBytes -= item.FileSize
                                                                     End SyncLock
                                                                 End If

                                                                 subRoutine = Nothing

                                                                 MyInvoke(Sub() UpdateListViewItem(itemOnGUI, item))

                                                                 index += 1
                                                             End If
                                                         Else
                                                             item.BoolValidHash = False
                                                         End If
                                                     Next

                                                     MyInvoke(Sub()
                                                                  For Each item As MyListViewItem In verifyHashesListFiles.Items
                                                                      If item.BoolFileExists Then item.BackColor = item.Color
                                                                  Next

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
                                                                          sbMessageBoxText.AppendLine(String.Format("Processing of hash file complete. {0} out of {1} file(s) passed verification, {2} {3} didn't pass verification.",
                                                                                                                    MyToString(longFilesThatPassedVerification),
                                                                                                                    MyToString(intFileCount),
                                                                                                                    MyToString(intFilesThatDidNotPassVerification),
                                                                                                                    If(intFilesThatDidNotPassVerification = 1, "file", "files")
                                                                                                                   )
                                                                           )
                                                                      End If
                                                                  Else
                                                                      sbMessageBoxText.AppendLine("Processing of hash file complete.")
                                                                      sbMessageBoxText.AppendLine()
                                                                      btnRetestFailedFiles.Visible = True

                                                                      Dim intTotalFiles As Integer = intFileCount - intFilesNotFound
                                                                      If longFilesThatPassedVerification = intTotalFiles Then
                                                                          sbMessageBoxText.AppendLine(String.Format("All files have passed verification. Unfortunately, {0} {1} were not found.",
                                                                                                                    MyToString(intFilesNotFound),
                                                                                                                    If(intFilesNotFound = 1, "file", "files")
                                                                                                                   )
                                                                           )
                                                                      Else
                                                                          intFilesThatDidNotPassVerification = intTotalFiles - longFilesThatPassedVerification
                                                                          If intFilesThatDidNotPassVerification <> 0 Then btnRetestFailedFiles.Visible = True
                                                                          sbMessageBoxText.AppendLine(String.Format("Not all of the files passed verification, only {0} out of {1} {2} passed verification, Unfortunately, {3} {4} didn't pass verification and {5} {6} were not found.",
                                                                                                                    MyToString(longFilesThatPassedVerification),
                                                                                                                    MyToString(intTotalFiles),
                                                                                                                    If(intTotalFiles = 1, "file", "files"),
                                                                                                                    MyToString(intFilesThatDidNotPassVerification),
                                                                                                                    If(intFilesThatDidNotPassVerification = 1, "file", "files"),
                                                                                                                    MyToString(intFilesNotFound),
                                                                                                                    If(intFilesNotFound = 1, "file", "files")
                                                                                                                   )
                                                                           )
                                                                      End If
                                                                  End If

                                                                  sbMessageBoxText.AppendLine()
                                                                  sbMessageBoxText.AppendLine("Processing completed in " & TimespanToHMS(stopWatch.Elapsed) & ".")

                                                                  MsgBox(sbMessageBoxText.ToString.Trim, MsgBoxStyle.Information, strMessageBoxTitleText)
                                                              End Sub)

                                                     boolBackgroundThreadWorking = False
                                                     workingThread = Nothing
                                                 Catch ex As Threading.ThreadAbortException
                                                     MyInvoke(Sub()
                                                                  If Not boolClosingWindow Then
                                                                      verifyHashesListFiles.EndUpdate()
                                                                      lblVerifyHashStatusProcessingFile.Visible = False
                                                                      verifyIndividualFilesAllFilesProgressBar.Visible = False
                                                                      lblVerifyHashStatus.Visible = False
                                                                      lblVerifyHashesTotalStatus.Visible = False
                                                                      lblProcessingFileVerify.Visible = False
                                                                      VerifyHashProgressBar.Value = 0
                                                                      VerifyHashProgressBar.Visible = False
                                                                      ProgressForm.SetTaskbarProgressBarValue(0)
                                                                      verifyHashesListFiles.Items.Clear()
                                                                      Text = strWindowTitle
                                                                      verifyHashesListFiles.Size = New Size(verifyHashesListFiles.Size.Width, verifyHashesListFiles.Size.Height + 72)
                                                                      lblVerifyFileNameLabel.Text = "File Name: (None Selected for Processing)"
                                                                  End If

                                                                  boolBackgroundThreadWorking = False
                                                                  workingThread = Nothing
                                                                  If Not boolClosingWindow Then MsgBox("Processing aborted.", MsgBoxStyle.Information, strMessageBoxTitleText)
                                                              End Sub)
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
                                                              End Sub)
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
            My.Settings.defaultHash = defaultHashType.SelectedIndex
        End If
    End Sub

    Private Sub SetDefaultHashTypeGUIElementOptions()
        If defaultHashType.SelectedIndex = 0 Then
            radioMD5.Checked = True
            textRadioMD5.Checked = True
            colChecksum.Text = strColumnTitleChecksumMD5
        ElseIf defaultHashType.SelectedIndex = 1 Then
            radioSHA1.Checked = True
            textRadioSHA1.Checked = True
            colChecksum.Text = strColumnTitleChecksumSHA160
        ElseIf defaultHashType.SelectedIndex = 2 Then
            radioSHA256.Checked = True
            textRadioSHA256.Checked = True
            colChecksum.Text = strColumnTitleChecksumSHA256
        ElseIf defaultHashType.SelectedIndex = 3 Then
            radioSHA384.Checked = True
            textRadioSHA384.Checked = True
            colChecksum.Text = strColumnTitleChecksumSHA384
        ElseIf defaultHashType.SelectedIndex = 4 Then
            radioSHA512.Checked = True
            textRadioSHA512.Checked = True
            colChecksum.Text = strColumnTitleChecksumSHA512
        End If
    End Sub

    Private Sub ChkShowFileProgressInFileList_Click(sender As Object, e As EventArgs) Handles chkShowFileProgressInFileList.Click
        My.Settings.boolShowFileProgressInFileList = chkShowFileProgressInFileList.Checked
    End Sub
End Class