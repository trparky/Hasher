Public Class Form1
    Private Const strToBeComputed As String = "To Be Computed"
    Private Const strNoBackgroundProcesses As String = "(No Background Processes)"
    Private Const intBufferSize As Integer = 16 * 1024 * 1024

    Private hashResultArray As New Dictionary(Of String, String)
    Private hashLineParser As New Text.RegularExpressions.Regex("([a-zA-Z0-9]*) \*(.*)", System.Text.RegularExpressions.RegexOptions.Compiled)
    Private hashLineFilePathChecker As New Text.RegularExpressions.Regex("\A[A-Za-z]{1}:.*\Z", System.Text.RegularExpressions.RegexOptions.Compiled)
    Private boolBackgroundThreadWorking As Boolean = False
    Private workingThread As Threading.Thread
    Private boolClosingWindow As Boolean = False
    Private m_SortingColumn1, m_SortingColumn2 As ColumnHeader

    Enum checksumType As Short
        md5
        sha160
        sha256
        sha384
        sha512
        RIPEMD160
    End Enum

    Function fileSizeToHumanSize(ByVal size As Long, Optional roundToNearestWholeNumber As Boolean = False) As String
        Dim result As String
        Dim shortRoundNumber As Short = If(roundToNearestWholeNumber, 0, 2)

        If size <= (2 ^ 10) Then
            result = size & " Bytes"
        ElseIf size > (2 ^ 10) And size <= (2 ^ 20) Then
            result = Math.Round(size / (2 ^ 10), shortRoundNumber) & " KBs"
        ElseIf size > (2 ^ 20) And size <= (2 ^ 30) Then
            result = Math.Round(size / (2 ^ 20), shortRoundNumber) & " MBs"
        ElseIf size > (2 ^ 30) And size <= (2 ^ 40) Then
            result = Math.Round(size / (2 ^ 30), shortRoundNumber) & " GBs"
        ElseIf size > (2 ^ 40) And size <= (2 ^ 50) Then
            result = Math.Round(size / (2 ^ 40), shortRoundNumber) & " TBs"
        ElseIf size > (2 ^ 50) And size <= (2 ^ 60) Then
            result = Math.Round(size / (2 ^ 50), shortRoundNumber) & " PBs"
        Else
            result = "(None)"
        End If

        Return result
    End Function

    Function verifyChecksum(strFile As String, checksumType As checksumType) As String
        Dim strChecksum As String = Nothing

        If IO.File.Exists(strFile) Then
            Dim oldLocationInFile As ULong = 0

            Dim checksums As New checksums With {
                .setFileStream = New IO.FileStream(strFile, IO.FileMode.Open, IO.FileAccess.Read, IO.FileShare.Read, intBufferSize, IO.FileOptions.SequentialScan),
                .setChecksumStatusUpdateRoutine = Sub(checksumStatusDetails As checksumStatusDetails)
                                                      Try
                                                          Me.Invoke(Sub()
                                                                        lblVerifyHashStatus.Text = "Estimated " & fileSizeToHumanSize(checksumStatusDetails.currentLocationInFile - oldLocationInFile) & "/second"
                                                                        oldLocationInFile = checksumStatusDetails.currentLocationInFile

                                                                        If checksumStatusDetails.currentLocationInFile <> 0 And checksumStatusDetails.lengthOfFile <> 0 Then
                                                                            VerifyHashProgressBar.Value = checksumStatusDetails.currentLocationInFile / checksumStatusDetails.lengthOfFile * 100
                                                                        Else
                                                                            VerifyHashProgressBar.Value = 0
                                                                        End If

                                                                        lblVerifyHashStatus.Text &= ", " & String.Format("{0} of {1} have been processed.", fileSizeToHumanSize(checksumStatusDetails.currentLocationInFile), fileSizeToHumanSize(checksumStatusDetails.lengthOfFile))
                                                                        oldLocationInFile = checksumStatusDetails.currentLocationInFile
                                                                    End Sub)
                                                      Catch ex As Exception
                                                      End Try
                                                  End Sub
            }

            If checksumType = checksumType.md5 Then
                strChecksum = checksums.MD5()
            ElseIf checksumType = checksumType.sha160 Then
                strChecksum = checksums.SHA160()
            ElseIf checksumType = checksumType.sha256 Then
                strChecksum = checksums.SHA256()
            ElseIf checksumType = checksumType.sha384 Then
                strChecksum = checksums.SHA384()
            ElseIf checksumType = checksumType.sha512 Then
                strChecksum = checksums.SHA512()
            ElseIf checksumType = checksumType.RIPEMD160 Then
                strChecksum = checksums.RIPEMD160()
            Else
                strChecksum = Nothing
            End If

            checksums.dispose()
        Else
            strChecksum = Nothing
        End If

        Return strChecksum
    End Function

    Function performIndividualFilesChecksum(index As Short, strFile As String, checksumType As checksumType) As String
        Dim strChecksum As String = Nothing

        If IO.File.Exists(strFile) Then
            Dim oldLocationInFile As ULong = 0
            Dim checksums As New checksums With {
                .setFileStream = New IO.FileStream(strFile, IO.FileMode.Open, IO.FileAccess.Read, IO.FileShare.Read, intBufferSize, IO.FileOptions.SequentialScan),
                .setChecksumStatusUpdateRoutine = Sub(checksumStatusDetails As checksumStatusDetails)
                                                      Try
                                                          Me.Invoke(Sub()
                                                                        lblIndividualFilesStatus.Text = "Estimated " & fileSizeToHumanSize(checksumStatusDetails.currentLocationInFile - oldLocationInFile) & "/second"
                                                                        oldLocationInFile = checksumStatusDetails.currentLocationInFile

                                                                        If checksumStatusDetails.currentLocationInFile <> 0 And checksumStatusDetails.lengthOfFile <> 0 Then
                                                                            IndividualFilesProgressBar.Value = checksumStatusDetails.currentLocationInFile / checksumStatusDetails.lengthOfFile * 100
                                                                        Else
                                                                            IndividualFilesProgressBar.Value = 0
                                                                        End If

                                                                        lblIndividualFilesStatus.Text &= ", " & String.Format("{0} of {1} have been processed.", fileSizeToHumanSize(checksumStatusDetails.currentLocationInFile), fileSizeToHumanSize(checksumStatusDetails.lengthOfFile))
                                                                        lblIndividualFilesStatusProcessingFile.Text = String.Format("Processing {0} of {1} file(s).", index, listFiles.Items.Count())
                                                                        oldLocationInFile = checksumStatusDetails.currentLocationInFile
                                                                    End Sub)
                                                      Catch ex As Exception
                                                      End Try
                                                  End Sub
            }

            If checksumType = checksumType.md5 Then
                strChecksum = checksums.MD5()
            ElseIf checksumType = checksumType.sha160 Then
                strChecksum = checksums.SHA160()
            ElseIf checksumType = checksumType.sha256 Then
                strChecksum = checksums.SHA256()
            ElseIf checksumType = checksumType.sha384 Then
                strChecksum = checksums.SHA384()
            ElseIf checksumType = checksumType.sha512 Then
                strChecksum = checksums.SHA512()
            ElseIf checksumType = checksumType.RIPEMD160 Then
                strChecksum = checksums.RIPEMD160()
            Else
                strChecksum = Nothing
            End If

            checksums.dispose()
        Else
            strChecksum = Nothing
        End If

        Return strChecksum
    End Function

    Private Sub updateFilesListCountHeader()
        lblHashIndividualFilesStep1.Text = String.Format("Step 1: Select Individual Files to be Hashed: {0} Files", listFiles.Items.Count().ToString())
        btnComputeHash.Enabled = If(listFiles.Items.Count() = 0, False, True)
    End Sub

    Private Sub btnRemoveAllFiles_Click(sender As Object, e As EventArgs) Handles btnRemoveAllFiles.Click
        listFiles.Items.Clear()
        updateFilesListCountHeader()
    End Sub

    Private Sub btnRemoveSelectedFiles_Click(sender As Object, e As EventArgs) Handles btnRemoveSelectedFiles.Click
        For Each item In listFiles.SelectedItems
            listFiles.Items.Remove(item)
        Next
        updateFilesListCountHeader()
    End Sub

    Private Sub btnAddIndividualFiles_Click(sender As Object, e As EventArgs) Handles btnAddIndividualFiles.Click
        Dim itemToBeAdded As myListViewItem

        OpenFileDialog.Title = "Select Files to be Hashed..."
        OpenFileDialog.Multiselect = True
        OpenFileDialog.Filter = "Show All Files|*.*"

        If OpenFileDialog.ShowDialog() = DialogResult.OK Then
            If OpenFileDialog.FileNames.Count() = 0 Then
                MsgBox("You must select some files.", MsgBoxStyle.Critical, Me.Text)
            ElseIf OpenFileDialog.FileNames.Count() = 1 Then
                If Not isFileInListView(OpenFileDialog.FileName) Then
                    itemToBeAdded = New myListViewItem(OpenFileDialog.FileName) With {.fileSize = New IO.FileInfo(OpenFileDialog.FileName).Length}
                    itemToBeAdded.SubItems.Add(fileSizeToHumanSize(itemToBeAdded.fileSize))
                    itemToBeAdded.SubItems.Add(strToBeComputed)
                    listFiles.Items.Add(itemToBeAdded)
                    itemToBeAdded = Nothing
                End If
            Else
                listFiles.BeginUpdate()
                For Each strFileName As String In OpenFileDialog.FileNames
                    If Not isFileInListView(strFileName) Then
                        itemToBeAdded = New myListViewItem(strFileName) With {.fileSize = New IO.FileInfo(strFileName).Length}
                        itemToBeAdded.SubItems.Add(fileSizeToHumanSize(itemToBeAdded.fileSize))
                        itemToBeAdded.SubItems.Add(strToBeComputed)
                        listFiles.Items.Add(itemToBeAdded)
                        itemToBeAdded = Nothing
                    End If
                Next
                listFiles.EndUpdate()
            End If
        End If
        updateFilesListCountHeader()
    End Sub

    Private Sub btnComputeHash_Click(sender As Object, e As EventArgs) Handles btnComputeHash.Click
        btnComputeHash.Enabled = False
        btnAddFilesInFolder.Enabled = False
        btnAddIndividualFiles.Enabled = False
        btnRemoveAllFiles.Enabled = False
        btnRemoveSelectedFiles.Enabled = False
        listFiles.Enabled = False

        workingThread = New Threading.Thread(Sub()
                                                 Try
                                                     boolBackgroundThreadWorking = True
                                                     Dim strFileName, strChecksum As String
                                                     Dim checksumType As checksumType
                                                     Dim index As Short = 1
                                                     hashResultArray.Clear()

                                                     radioMD5.Enabled = False
                                                     radioSHA1.Enabled = False
                                                     radioSHA256.Enabled = False
                                                     radioSHA384.Enabled = False
                                                     radioSHA512.Enabled = False
                                                     radioRIPEMD160.Enabled = False

                                                     If radioMD5.Checked Then
                                                         checksumType = checksumType.md5
                                                     ElseIf radioSHA1.Checked Then
                                                         checksumType = checksumType.sha160
                                                     ElseIf radioSHA256.Checked Then
                                                         checksumType = checksumType.sha256
                                                     ElseIf radioSHA384.Checked Then
                                                         checksumType = checksumType.sha384
                                                     ElseIf radioSHA512.Checked Then
                                                         checksumType = checksumType.sha512
                                                     ElseIf radioRIPEMD160.Checked Then
                                                         checksumType = checksumType.RIPEMD160
                                                     End If

                                                     listFiles.BeginUpdate()

                                                     For Each item As ListViewItem In listFiles.Items
                                                         strFileName = item.SubItems(0).Text

                                                         If Not hashResultArray.ContainsKey(strFileName) Then
                                                             strChecksum = performIndividualFilesChecksum(index, strFileName, checksumType)
                                                             item.SubItems(2).Text = strChecksum
                                                             hashResultArray.Add(strFileName, strChecksum)
                                                         End If

                                                         index += 1
                                                     Next

                                                     listFiles.EndUpdate()

                                                     btnIndividualFilesCopyToClipboard.Enabled = True
                                                     btnIndividualFilesSaveResultsToDisk.Enabled = True
                                                     radioMD5.Enabled = True
                                                     radioSHA1.Enabled = True
                                                     radioSHA256.Enabled = True
                                                     radioSHA384.Enabled = True
                                                     radioSHA512.Enabled = True
                                                     radioRIPEMD160.Enabled = True

                                                     Me.Invoke(Sub() MsgBox("Complete.", MsgBoxStyle.Information + MsgBoxStyle.ApplicationModal, Me.Text))
                                                     resetHashIndividualFilesProgress()
                                                     boolBackgroundThreadWorking = False
                                                     workingThread = Nothing
                                                 Catch ex As Threading.ThreadAbortException
                                                     If Not boolClosingWindow Then
                                                         lblIndividualFilesStatus.Text = strNoBackgroundProcesses
                                                         lblIndividualFilesStatusProcessingFile.Text = ""
                                                         IndividualFilesProgressBar.Value = 0
                                                         resetHashIndividualFilesProgress()
                                                     End If

                                                     boolBackgroundThreadWorking = False
                                                     workingThread = Nothing
                                                     If Not boolClosingWindow Then Me.Invoke(Sub() MsgBox("Processing aborted.", MsgBoxStyle.Information + MsgBoxStyle.ApplicationModal, Me.Text))
                                                 Finally
                                                     btnComputeHash.Enabled = True
                                                 End Try
                                             End Sub) With {
            .Priority = Threading.ThreadPriority.Highest,
            .Name = "Hash Generation Thread",
            .IsBackground = True
        }
        workingThread.Start()
    End Sub

    Sub resetHashIndividualFilesProgress()
        btnAddFilesInFolder.Enabled = True
        btnAddIndividualFiles.Enabled = True
        btnRemoveAllFiles.Enabled = True
        btnRemoveSelectedFiles.Enabled = True
        listFiles.Enabled = True

        lblIndividualFilesStatus.Text = strNoBackgroundProcesses
        lblIndividualFilesStatusProcessingFile.Text = ""
        IndividualFilesProgressBar.Value = 0
    End Sub

    Private Function strGetIndividualHashesInStringFormat() As String
        Dim stringBuilder As New Text.StringBuilder()
        For Each item As KeyValuePair(Of String, String) In hashResultArray
            stringBuilder.AppendLine(item.Value.ToString() & " *" & item.Key)
        Next
        Return stringBuilder.ToString()
    End Function

    Private Sub btnIndividualFilesCopyToClipboard_Click(sender As Object, e As EventArgs) Handles btnIndividualFilesCopyToClipboard.Click
        If copyTextToWindowsClipboard(strGetIndividualHashesInStringFormat().Trim) Then MsgBox("Your hash results have been copied to the Windows Clipboard.", MsgBoxStyle.Information, Me.Text)
    End Sub

    Private Function copyTextToWindowsClipboard(strTextToBeCopiedToClipboard As String) As Boolean
        Try
            Clipboard.SetDataObject(strTextToBeCopiedToClipboard, True, 5, 200)
            Return True
        Catch ex As Exception
            MsgBox("Unable to open Windows Clipboard to copy text to it.", MsgBoxStyle.Critical, Me.Text)
            Return False
        End Try
    End Function

    Private Sub btnIndividualFilesSaveResultsToDisk_Click(sender As Object, e As EventArgs) Handles btnIndividualFilesSaveResultsToDisk.Click
        If Me.radioMD5.Checked Then
            SaveFileDialog.Filter = "MD5 File|*.md5"
        ElseIf radioSHA1.Checked Then
            SaveFileDialog.Filter = "SHA1 File|*.sha1"
        ElseIf radioSHA256.Checked Then
            SaveFileDialog.Filter = "SHA256 File|*.sha256"
        ElseIf radioSHA384.Checked Then
            SaveFileDialog.Filter = "SHA384 File|*.sha384"
        ElseIf radioSHA512.Checked Then
            SaveFileDialog.Filter = "SHA512 File|*.sha512"
        ElseIf radioRIPEMD160.Checked Then
            SaveFileDialog.Filter = "RIPEMD160 File|*.ripemd160"
        End If

        SaveFileDialog.Title = "Save Hash Results to Disk"

        If SaveFileDialog.ShowDialog() = DialogResult.OK Then
            IO.File.WriteAllText(SaveFileDialog.FileName, strGetIndividualHashesInStringFormat(), System.Text.Encoding.UTF8)
            MsgBox("Your hash results have been written to disk.", MsgBoxStyle.Information, Me.Text)
        End If
    End Sub

    Private Sub disableIndividualFilesResultsButtonsAndClearResults()
        btnIndividualFilesCopyToClipboard.Enabled = False
        btnIndividualFilesSaveResultsToDisk.Enabled = False
        hashResultArray.Clear()
    End Sub

    Private Sub radioMD5_Click(sender As Object, e As EventArgs) Handles radioMD5.Click
        disableIndividualFilesResultsButtonsAndClearResults()
    End Sub

    Private Sub radioSHA1_Click(sender As Object, e As EventArgs) Handles radioSHA1.Click
        disableIndividualFilesResultsButtonsAndClearResults()
    End Sub

    Private Sub radioSHA256_Click(sender As Object, e As EventArgs) Handles radioSHA256.Click
        disableIndividualFilesResultsButtonsAndClearResults()
    End Sub

    Private Sub radioSHA384_Click(sender As Object, e As EventArgs) Handles radioSHA384.Click
        disableIndividualFilesResultsButtonsAndClearResults()
    End Sub

    Private Sub radioSHA512_Click(sender As Object, e As EventArgs) Handles radioSHA512.Click
        disableIndividualFilesResultsButtonsAndClearResults()
    End Sub

    Private Sub textRadioRIPEMD160_Click(sender As Object, e As EventArgs) Handles textRadioRIPEMD160.Click
        disableIndividualFilesResultsButtonsAndClearResults()
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Control.CheckForIllegalCrossThreadCalls = False
        lblIndividualFilesStatusProcessingFile.Text = ""
        lblVerifyHashStatusProcessingFile.Text = ""
        chkRecurrsiveDirectorySearch.Checked = My.Settings.boolRecurrsiveDirectorySearch
    End Sub

    Private Sub addFilesFromDirectory(directoryPath As String)
        Dim listOfFiles As New List(Of ListViewItem)
        Dim listViewItem As myListViewItem
        Dim directorySearchOptions As IO.SearchOption = If(My.Settings.boolRecurrsiveDirectorySearch, IO.SearchOption.AllDirectories, IO.SearchOption.TopDirectoryOnly)

        For Each strFileName As String In IO.Directory.GetFiles(directoryPath, "*.*", directorySearchOptions)
            If Not isFileInListOfListViewItems(strFileName, listOfFiles) Then
                listViewItem = New myListViewItem(strFileName) With {.fileSize = New IO.FileInfo(strFileName).Length}
                listViewItem.SubItems.Add(fileSizeToHumanSize(listViewItem.fileSize))
                listViewItem.SubItems.Add(strToBeComputed)
                listOfFiles.Add(listViewItem)
                listViewItem = Nothing
            End If
        Next

        listFiles.BeginUpdate()
        listFiles.Items.AddRange(listOfFiles.ToArray())
        listFiles.EndUpdate()
    End Sub

    Private Sub btnAddFilesInFolder_Click(sender As Object, e As EventArgs) Handles btnAddFilesInFolder.Click
        If FolderBrowserDialog.ShowDialog = DialogResult.OK Then
            addFilesFromDirectory(FolderBrowserDialog.SelectedPath)
        End If
        updateFilesListCountHeader()
    End Sub

    Private Sub btnOpenExistingHashFile_Click(sender As Object, e As EventArgs) Handles btnOpenExistingHashFile.Click
        btnOpenExistingHashFile.Enabled = False
        verifyHashesListFiles.Items.Clear()

        Dim oldMultiValue As Boolean = OpenFileDialog.Multiselect
        Dim checksumType As checksumType
        Dim checksumFileInfo As IO.FileInfo
        Dim strFileExtension, strPathOfChecksumFile As String
        Dim listOfFiles As New List(Of ListViewItem)

        OpenFileDialog.Title = "Select a hash file to verify..."
        OpenFileDialog.Multiselect = False
        OpenFileDialog.Filter = "Checksum File|*.md5;*.sha1;*.sha256;*.sha384;*.sha512;*.ripemd160"

        If OpenFileDialog.ShowDialog() = DialogResult.OK Then
            checksumFileInfo = New IO.FileInfo(OpenFileDialog.FileName)
            strFileExtension = checksumFileInfo.Extension
            strPathOfChecksumFile = checksumFileInfo.DirectoryName
            checksumFileInfo = Nothing

            If strFileExtension.Equals(".md5", StringComparison.OrdinalIgnoreCase) Then
                checksumType = checksumType.md5
            ElseIf strFileExtension.Equals(".sha1", StringComparison.OrdinalIgnoreCase) Then
                checksumType = checksumType.sha160
            ElseIf strFileExtension.Equals(".sha256", StringComparison.OrdinalIgnoreCase) Then
                checksumType = checksumType.sha256
            ElseIf strFileExtension.Equals(".sha384", StringComparison.OrdinalIgnoreCase) Then
                checksumType = checksumType.sha384
            ElseIf strFileExtension.Equals(".sha512", StringComparison.OrdinalIgnoreCase) Then
                checksumType = checksumType.sha512
            ElseIf strFileExtension.Equals(".ripemd160", StringComparison.OrdinalIgnoreCase) Then
                checksumType = checksumType.RIPEMD160
            Else
                MsgBox("Invalid Hash File Type.", MsgBoxStyle.Critical, Me.Text)
                Exit Sub
            End If

            workingThread = New Threading.Thread(Sub()
                                                     Try
                                                         verifyHashesListFiles.BeginUpdate()
                                                         boolBackgroundThreadWorking = True
                                                         Dim linesInFile As New Specialized.StringCollection()
                                                         Dim strLineInFile As String
                                                         Dim index As Integer = 1
                                                         Dim longFilesThatPassedVerification As Long = 0

                                                         Using fileStream As New IO.StreamReader(OpenFileDialog.FileName, System.Text.Encoding.UTF8)
                                                             strLineInFile = fileStream.ReadLine()

                                                             While Not String.IsNullOrEmpty(strLineInFile)
                                                                 linesInFile.Add(strLineInFile)
                                                                 strLineInFile = fileStream.ReadLine()
                                                             End While
                                                         End Using

                                                         For Each strLineInCollection As String In linesInFile
                                                             lblVerifyHashStatusProcessingFile.Text = String.Format("Processing {0} of {1} file(s)", index, linesInFile.Count())
                                                             processLineInHashFile(strPathOfChecksumFile, strLineInCollection, checksumType, listOfFiles, longFilesThatPassedVerification)
                                                             index += 1
                                                         Next

                                                         lblVerifyHashStatusProcessingFile.Text = ""
                                                         lblVerifyHashStatus.Text = strNoBackgroundProcesses
                                                         VerifyHashProgressBar.Value = 0

                                                         verifyHashesListFiles.Items.AddRange(listOfFiles.ToArray())
                                                         verifyHashesListFiles.EndUpdate()
                                                         Me.Invoke(Sub() MsgBox(String.Format("Processing of hash file complete. {0} out of {1} file(s) passed verification.", longFilesThatPassedVerification, linesInFile.Count), MsgBoxStyle.Information + MsgBoxStyle.ApplicationModal, Me.Text))
                                                         boolBackgroundThreadWorking = False
                                                         workingThread = Nothing
                                                     Catch ex As Threading.ThreadAbortException
                                                         If Not boolClosingWindow Then
                                                             lblVerifyHashStatusProcessingFile.Text = ""
                                                             lblVerifyHashStatus.Text = strNoBackgroundProcesses
                                                             VerifyHashProgressBar.Value = 0
                                                             verifyHashesListFiles.Items.Clear()
                                                         End If

                                                         boolBackgroundThreadWorking = False
                                                         workingThread = Nothing
                                                         If Not boolClosingWindow Then Me.Invoke(Sub() MsgBox("Processing aborted.", MsgBoxStyle.Information + MsgBoxStyle.ApplicationModal, Me.Text))
                                                     Finally
                                                         btnOpenExistingHashFile.Enabled = True
                                                     End Try
                                                 End Sub) With {
                .Priority = Threading.ThreadPriority.Highest,
                .Name = "Verify Hash File Working Thread",
                .IsBackground = True
            }
            workingThread.Start()
        End If

        OpenFileDialog.Multiselect = oldMultiValue
    End Sub

    Private Sub processLineInHashFile(strPathOfChecksumFile As String, strLineInFile As String, hashFileType As checksumType, ByRef listOfFiles As List(Of ListViewItem), ByRef longFilesThatPassedVerification As Long)
        Dim strChecksum As String = hashLineParser.Match(strLineInFile).Groups(1).Value
        Dim strFileName As String = hashLineParser.Match(strLineInFile).Groups(2).Value

        If Not hashLineFilePathChecker.IsMatch(strFileName) Then
            strFileName = IO.Path.Combine(strPathOfChecksumFile, strFileName)
        End If

        Dim listViewItem As myListViewItem
        listViewItem = New myListViewItem(strFileName)

        If IO.File.Exists(strFileName) Then
            Dim strChecksumInFile As String = verifyChecksum(strFileName, hashFileType)
            listViewItem.fileSize = New IO.FileInfo(strFileName).Length

            If strChecksum.Equals(strChecksumInFile, StringComparison.OrdinalIgnoreCase) Then
                listViewItem.BackColor = Color.LightGreen
                listViewItem.SubItems.Add(fileSizeToHumanSize(listViewItem.fileSize))
                listViewItem.SubItems.Add("Valid")
                longFilesThatPassedVerification += 1
            Else
                listViewItem.BackColor = Color.Pink
                listViewItem.SubItems.Add(fileSizeToHumanSize(listViewItem.fileSize))
                listViewItem.SubItems.Add("NOT Valid")
            End If
        Else
            listViewItem.BackColor = Color.LightGray
            listViewItem.SubItems.Add("")
            listViewItem.SubItems.Add("Doesn't Exist")
        End If

        verifyHashesListFiles.Items.Add(listViewItem)
        listViewItem = Nothing
    End Sub

    Private Sub listFiles_DragDrop(sender As Object, e As DragEventArgs) Handles listFiles.DragDrop
        Dim listViewItem As myListViewItem

        For Each strItem As String In e.Data.GetData(DataFormats.FileDrop)
            If IO.File.GetAttributes(strItem).HasFlag(IO.FileAttributes.Directory) Then
                addFilesFromDirectory(strItem)
            Else
                If Not isFileInListView(strItem) Then
                    listViewItem = New myListViewItem(strItem) With {.fileSize = New IO.FileInfo(strItem).Length}
                    listViewItem.SubItems.Add(fileSizeToHumanSize(listViewItem.fileSize))
                    listViewItem.SubItems.Add(strToBeComputed)
                    listFiles.Items.Add(listViewItem)
                    listViewItem = Nothing
                End If
            End If
        Next
        updateFilesListCountHeader()
    End Sub

    Private Sub listFiles_DragEnter(sender As Object, e As DragEventArgs) Handles listFiles.DragEnter
        e.Effect = If(e.Data.GetDataPresent(DataFormats.FileDrop), DragDropEffects.All, DragDropEffects.None)
    End Sub

    Private Function isFileInListView(strFile As String) As Boolean
        Dim itemCountWithMatch As Integer = listFiles.Items.Cast(Of myListViewItem).Where(Function(item As myListViewItem) item.Text.Equals(strFile, StringComparison.OrdinalIgnoreCase)).Count() <> 0
        'Dim itemCountWithMatch As Integer = (From item As myListViewItem In listFiles.Items Where item.Text.Equals(strFile, StringComparison.OrdinalIgnoreCase) Select item).Count
        Return If(itemCountWithMatch = 0, False, True)
    End Function

    Private Function isFileInListOfListViewItems(strFile As String, ByRef listOfListViewItems As List(Of ListViewItem)) As Boolean
        Dim itemCountWithMatch As Integer = listOfListViewItems.Cast(Of ListViewItem).Where(Function(item As ListViewItem) item.Text.Equals(strFile, StringComparison.OrdinalIgnoreCase)).Count() <> 0
        'Dim itemCountWithMatch As Integer = (From item As ListViewItem In listFiles.Items Where item.Text.Equals(strFile, StringComparison.OrdinalIgnoreCase) Select item).Count
        Return If(itemCountWithMatch = 0, False, True)
    End Function

    Private Sub chkRecurrsiveDirectorySearch_Click(sender As Object, e As EventArgs) Handles chkRecurrsiveDirectorySearch.Click
        My.Settings.boolRecurrsiveDirectorySearch = chkRecurrsiveDirectorySearch.Checked
    End Sub

    Private Sub txtTextToHash_TextChanged(sender As Object, e As EventArgs) Handles txtTextToHash.TextChanged
        lblHashTextStep1.Text = String.Format("Step 1: Input some text: {0} Characters", txtTextToHash.Text.Length)
        btnComputeTextHash.Enabled = If(txtTextToHash.Text.Length = 0, False, True)
        clearTextHashResults()
    End Sub

    Private Sub clearTextHashResults()
        btnCopyTextHashResultsToClipboard.Enabled = False
        txtHashResults.Text = Nothing
    End Sub

    Private Sub btnComputeTextHash_Click(sender As Object, e As EventArgs) Handles btnComputeTextHash.Click
        If textRadioMD5.Checked Then
            txtHashResults.Text = checksums.MD5String(txtTextToHash.Text)
        ElseIf textRadioSHA1.Checked Then
            txtHashResults.Text = checksums.SHA160String(txtTextToHash.Text)
        ElseIf textRadioSHA256.Checked Then
            txtHashResults.Text = checksums.SHA256String(txtTextToHash.Text)
        ElseIf textRadioSHA384.Checked Then
            txtHashResults.Text = checksums.SHA384String(txtTextToHash.Text)
        ElseIf textRadioSHA512.Checked Then
            txtHashResults.Text = checksums.SHA512String(txtTextToHash.Text)
        ElseIf textRadioRIPEMD160.Checked Then
            txtHashResults.Text = checksums.RIPEMD160String(txtTextToHash.Text)
        End If

        btnCopyTextHashResultsToClipboard.Enabled = True
    End Sub

    Private Sub btnPasteTextFromWindowsClipboard_Click(sender As Object, e As EventArgs) Handles btnPasteTextFromWindowsClipboard.Click
        txtTextToHash.Text = Clipboard.GetText()
    End Sub

    Private Sub btnCopyTextHashResultsToClipboard_Click(sender As Object, e As EventArgs) Handles btnCopyTextHashResultsToClipboard.Click
        If copyTextToWindowsClipboard(txtHashResults.Text) Then MsgBox("Your hash results have been copied to the Windows Clipboard.", MsgBoxStyle.Information, Me.Text)
    End Sub

    Private Sub textRadioSHA256_CheckedChanged(sender As Object, e As EventArgs) Handles textRadioSHA256.CheckedChanged
        clearTextHashResults()
    End Sub

    Private Sub textRadioSHA384_CheckedChanged(sender As Object, e As EventArgs) Handles textRadioSHA384.CheckedChanged
        clearTextHashResults()
    End Sub

    Private Sub textRadioSHA512_CheckedChanged(sender As Object, e As EventArgs) Handles textRadioSHA512.CheckedChanged
        clearTextHashResults()
    End Sub

    Private Sub textRadioRIPEMD160_CheckedChanged(sender As Object, e As EventArgs) Handles textRadioRIPEMD160.CheckedChanged
        clearTextHashResults()
    End Sub

    Private Sub textRadioSHA1_CheckedChanged(sender As Object, e As EventArgs) Handles textRadioSHA1.CheckedChanged
        clearTextHashResults()
    End Sub

    Private Sub textRadioMD5_CheckedChanged(sender As Object, e As EventArgs) Handles textRadioMD5.CheckedChanged
        clearTextHashResults()
    End Sub

    Private Sub TabControl1_Selecting(sender As Object, e As TabControlCancelEventArgs) Handles TabControl1.Selecting
        If boolBackgroundThreadWorking AndAlso MsgBox("Checksum hashes are being computed, do you want to abort?", MsgBoxStyle.YesNo + MsgBoxStyle.Question, Me.Text) = MsgBoxResult.No Then
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
        If boolBackgroundThreadWorking AndAlso MsgBox("Checksum hashes are being computed, do you want to abort?", MsgBoxStyle.YesNo + MsgBoxStyle.Question, Me.Text) = MsgBoxResult.No Then
            e.Cancel = True
            Exit Sub
        Else
            If workingThread IsNot Nothing Then
                boolClosingWindow = True
                workingThread.Abort()
            End If
        End If
    End Sub

    Private Sub listFiles_ColumnClick(sender As Object, e As ColumnClickEventArgs) Handles listFiles.ColumnClick
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

    Private Sub verifyHashesListFiles_ColumnClick(sender As Object, e As ColumnClickEventArgs) Handles verifyHashesListFiles.ColumnClick
        ' Get the new sorting column.
        Dim new_sorting_column As ColumnHeader = verifyHashesListFiles.Columns(e.Column)

        ' Figure out the new sorting order.
        Dim sort_order As SortOrder
        If (m_SortingColumn1 Is Nothing) Then
            ' New column. Sort ascending.
            sort_order = SortOrder.Ascending
        Else
            ' See if this is the same column.
            If new_sorting_column.Equals(m_SortingColumn1) Then
                ' Same column. Switch the sort order.
                If m_SortingColumn1.Text.StartsWith("> ") Then
                    sort_order = SortOrder.Descending
                Else
                    sort_order = SortOrder.Ascending
                End If
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
End Class