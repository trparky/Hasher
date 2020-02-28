Imports System.IO.Pipes

Public Class Form1
#If DEBUG Then
    Private Const boolDebugMode As Boolean = True
#Else
    Private Const boolDebugMode As Boolean = False
#End If
    Private Const strToBeComputed As String = "To Be Computed"
    Private Const strNoBackgroundProcesses As String = "(No Background Processes)"
    Private Const strWindowTitle As String = "Hasher"

    Private intBufferSize As Integer = My.Settings.shortBufferSize * 1024 * 1024
    Private strLastDirectoryWorkedOn As String
    Private filesInListFiles As New Specialized.StringCollection
    Private ReadOnly hashLineParser As New Text.RegularExpressions.Regex("([a-zA-Z0-9]*) \*(.*)", System.Text.RegularExpressions.RegexOptions.Compiled)
    Private ReadOnly hashLineFilePathChecker As New Text.RegularExpressions.Regex("\A[A-Za-z]{1}:.*\Z", System.Text.RegularExpressions.RegexOptions.Compiled)
    Private boolBackgroundThreadWorking As Boolean = False
    Private workingThread As Threading.Thread
    Private boolClosingWindow As Boolean = False
    Private m_SortingColumn1, m_SortingColumn2 As ColumnHeader
    Private boolDoneLoading As Boolean = False
    Private Property pipeServer As NamedPipeServerStream = Nothing
    Private ReadOnly strNamedPipeServerName As String = "hasher_" & getHashOfString(Environment.UserName, checksumType.sha256).Substring(0, 10)
    Private Const strPayPal As String = "https://www.paypal.com/cgi-bin/webscr?cmd=_donations&business=HQL3AC96XKM42&lc=US&no_note=1&no_shipping=1&rm=1&return=http%3a%2f%2fwww%2etoms%2dworld%2eorg%2fblog%2fthank%2dyou%2dfor%2dyour%2ddonation&currency_code=USD&bn=PP%2dDonationsBF%3abtn_donateCC_LG%2egif%3aNonHosted"
    Private boolShowEstimatedTime As Boolean
    Private boolDidWePerformAPreviousHash As Boolean = False
    Private validColor, notValidColor, fileNotFoundColor As Color

    ''' <summary>
    ''' This function works very similar to the Invoke function that's already built into .NET. The only difference
    ''' is that this function checks to see if an invoke is required and only invokes the passed routine on the
    ''' main thread if it's required. If not, the passed routine is executed on the thread that the call
    ''' originated from. Also, if the program is closing the function aborts itself so as to prevent
    ''' System.InvalidOperationException upon program close.
    ''' </summary>
    ''' <param name="input"></param>
    Private Sub myInvoke(input As [Delegate])
        If boolClosingWindow Then Exit Sub
        If InvokeRequired() Then
            Invoke(input)
        Else
            input.DynamicInvoke()
        End If
    End Sub

    Private Function getListViewItems(ByVal lstview As ListView) As ListView.ListViewItemCollection
        Dim tempListViewItemCollection As ListView.ListViewItemCollection = New ListView.ListViewItemCollection(New ListView())

        If Not lstview.InvokeRequired() Then
            For Each item As myListViewItem In lstview.Items
                tempListViewItemCollection.Add(CType(item.Clone(), myListViewItem))
            Next

            Return tempListViewItemCollection
        Else
            Return CType(Invoke(New Func(Of ListView.ListViewItemCollection)(Function() getListViewItems(lstview))), ListView.ListViewItemCollection)
        End If
    End Function

    Private Sub updateListViewItem(ByRef itemOnGUI As myListViewItem, ByRef item As myListViewItem)
        With itemOnGUI
            For i As Short = 1 To item.SubItems.Count - 1
                .SubItems(i) = item.SubItems(i)
            Next

            .fileSize = item.fileSize
            .hash = item.hash
            .fileName = item.fileName
            .color = item.color
            .boolFileExists = item.boolFileExists
            .computeTime = item.computeTime
        End With
    End Sub

    Private Function myToString(input As Integer) As String
        Return If(chkUseCommasInNumbers.Checked, input.ToString("N0"), input.ToString)
    End Function

    Private Function myToString(input As Long) As String
        Return If(chkUseCommasInNumbers.Checked, input.ToString("N0"), input.ToString)
    End Function

    Function doChecksumWithAttachedSubRoutine(strFile As String, checksumType As checksumType, ByRef strChecksum As String, subRoutine As [Delegate]) As Boolean
        Try
            If IO.File.Exists(strFile) Then
                Dim checksums As New checksums(subRoutine)
                strChecksum = checksums.performFileHash(strFile, intBufferSize, checksumType, boolShowEstimatedTime)
                Return True
            Else
                Return False
            End If

            Return False
        Catch ex As Exception
            Return False
        End Try
    End Function

    Function doChecksumWithAttachedSubRoutine(strFile As String, checksumType As checksumType, ByRef strChecksum As String, checksumSubRoutine As [Delegate], finishedChecksumSubRoutine As [Delegate]) As Boolean
        Try
            If IO.File.Exists(strFile) Then
                Dim checksums As New checksums(checksumSubRoutine)
                strChecksum = checksums.performFileHash(strFile, intBufferSize, checksumType, boolShowEstimatedTime)
                finishedChecksumSubRoutine.DynamicInvoke()
                Return True
            Else
                Return False
            End If

            Return False
        Catch ex As Exception
            Return False
        End Try
    End Function

    Private Sub updateFilesListCountHeader(Optional boolIncludeSelectedItemCount As Boolean = False)
        If boolIncludeSelectedItemCount Then
            lblHashIndividualFilesStep1.Text = String.Format("Step 1: Select Individual Files to be Hashed: {0} Files ({1} {2} are selected)",
                                                             myToString(listFiles.Items.Count),
                                                             myToString(listFiles.SelectedItems.Count),
                                                             If(listFiles.SelectedItems.Count = 1, "item", "items")
                                                            )
        Else
            lblHashIndividualFilesStep1.Text = String.Format("Step 1: Select Individual Files to be Hashed: {0} Files",
                                                             myToString(listFiles.Items.Count)
                                                            )
        End If

        If listFiles.Items.Count = 0 Then
            btnComputeHash.Enabled = False
            btnIndividualFilesCopyToClipboard.Enabled = False
            btnIndividualFilesSaveResultsToDisk.Enabled = False
        Else
            btnComputeHash.Enabled = True

            If listFiles.Items.Cast(Of myListViewItem).Where(Function(item As myListViewItem) String.IsNullOrWhiteSpace(item.hash)).Count <> listFiles.Items.Count Then
                btnIndividualFilesCopyToClipboard.Enabled = True
                btnIndividualFilesSaveResultsToDisk.Enabled = True
            End If
        End If
    End Sub

    Private Sub btnRemoveAllFiles_Click(sender As Object, e As EventArgs) Handles btnRemoveAllFiles.Click
        listFiles.Items.Clear()
        filesInListFiles.Clear()
        updateFilesListCountHeader()
    End Sub

    Private Sub btnRemoveSelectedFiles_Click(sender As Object, e As EventArgs) Handles btnRemoveSelectedFiles.Click
        If listFiles.SelectedItems.Count > 500 AndAlso MsgBox("It would be recommended to use the ""Remove All Files"" button instead, removing this many items (" & myToString(listFiles.SelectedItems.Count) & " items) from the list is a slow process and will make the program appear locked up." & vbCrLf & vbCrLf & "Are you sure you want to remove the items this way?", MsgBoxStyle.Question + MsgBoxStyle.YesNo, Me.Text) = MsgBoxResult.No Then
            Exit Sub
        End If

        listFiles.BeginUpdate()
        For Each item As myListViewItem In listFiles.SelectedItems
            filesInListFiles.Remove(item.Text.Trim.ToLower)
            listFiles.Items.Remove(item)
        Next
        listFiles.EndUpdate()

        updateFilesListCountHeader()
    End Sub

    Private Function createListFilesObject(strFileName As String) As myListViewItem
        filesInListFiles.Add(strFileName.Trim.ToLower)

        Dim itemToBeAdded As New myListViewItem(strFileName) With {
            .fileSize = New IO.FileInfo(strFileName).Length,
            .fileName = strFileName
        }
        With itemToBeAdded
            .SubItems.Add(fileSizeToHumanSize(itemToBeAdded.fileSize))
            .SubItems.Add(strToBeComputed)
            .SubItems.Add("")
        End With

        Return itemToBeAdded
    End Function

    Private Sub btnAddIndividualFiles_Click(sender As Object, e As EventArgs) Handles btnAddIndividualFiles.Click
        OpenFileDialog.Title = "Select Files to be Hashed..."
        OpenFileDialog.Multiselect = True
        OpenFileDialog.Filter = "Show All Files|*.*"

        If OpenFileDialog.ShowDialog() = DialogResult.OK Then
            If OpenFileDialog.FileNames.Count = 0 Then
                MsgBox("You must select some files.", MsgBoxStyle.Critical, strWindowTitle)
            ElseIf OpenFileDialog.FileNames.Count = 1 Then
                strLastDirectoryWorkedOn = New IO.FileInfo(OpenFileDialog.FileName).DirectoryName

                If Not filesInListFiles.Contains(OpenFileDialog.FileName.Trim.ToLower) Then
                    listFiles.Items.Add(createListFilesObject(OpenFileDialog.FileName))
                End If
            Else
                strLastDirectoryWorkedOn = New IO.FileInfo(OpenFileDialog.FileNames(0)).DirectoryName

                listFiles.BeginUpdate()
                For Each strFileName As String In OpenFileDialog.FileNames
                    If Not filesInListFiles.Contains(strFileName.Trim.ToLower) Then
                        listFiles.Items.Add(createListFilesObject(strFileName))
                    End If
                Next
                listFiles.EndUpdate()
            End If
        End If

        updateFilesListCountHeader()
        If chkSortFileListingAfterAddingFilesToHash.Checked Then applyFileSizeSortingToHashList()
    End Sub

    Private Sub btnComputeHash_Click(sender As Object, e As EventArgs) Handles btnComputeHash.Click
        If btnComputeHash.Text = "Abort Processing" Then
            If workingThread IsNot Nothing Then
                workingThread.Abort()
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

        workingThread = New Threading.Thread(Sub()
                                                 Try
                                                     boolBackgroundThreadWorking = True
                                                     Dim percentage, allBytesPercentage As Double
                                                     Dim strChecksum As String = Nothing
                                                     Dim checksumType As checksumType
                                                     Dim index As Integer = 1

                                                     SyncLock threadLockingObject
                                                         ulongAllReadBytes = 0
                                                         ulongAllBytes = 0
                                                     End SyncLock

                                                     Dim subRoutine As [Delegate] = Sub(size As Long, totalBytesRead As Long, eta As TimeSpan)
                                                                                        Try
                                                                                            myInvoke(Sub()
                                                                                                         percentage = If(totalBytesRead <> 0 And size <> 0, totalBytesRead / size * 100, 0)
                                                                                                         IndividualFilesProgressBar.Value = percentage
                                                                                                         SyncLock threadLockingObject
                                                                                                             allBytesPercentage = ulongAllReadBytes / ulongAllBytes * 100
                                                                                                         End SyncLock
                                                                                                         ProgressForm.setTaskbarProgressBarValue(allBytesPercentage)
                                                                                                         hashIndividualFilesAllFilesProgressBar.Value = allBytesPercentage
                                                                                                         lblIndividualFilesStatus.Text = fileSizeToHumanSize(totalBytesRead) & " of " & fileSizeToHumanSize(size) & " (" & Math.Round(percentage, 2) & "%) have been processed."
                                                                                                         If boolShowEstimatedTime AndAlso eta <> TimeSpan.Zero Then lblIndividualFilesStatus.Text &= " Estimated " & timespanToHMS(eta) & " remaining."
                                                                                                     End Sub)
                                                                                        Catch ex As Exception
                                                                                        End Try
                                                                                    End Sub

                                                     myInvoke(Sub()
                                                                  radioMD5.Enabled = False
                                                                  radioSHA1.Enabled = False
                                                                  radioSHA256.Enabled = False
                                                                  radioSHA384.Enabled = False
                                                                  radioSHA512.Enabled = False

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
                                                                  End If
                                                              End Sub)


                                                     Dim stopWatch As Stopwatch = Stopwatch.StartNew
                                                     Dim computeStopwatch As Stopwatch
                                                     Dim itemOnGUI As myListViewItem
                                                     Dim items As ListView.ListViewItemCollection = getListViewItems(listFiles)

                                                     SyncLock threadLockingObject
                                                         For Each item As myListViewItem In items
                                                             If String.IsNullOrWhiteSpace(item.hash) Then ulongAllBytes += item.fileSize
                                                         Next
                                                     End SyncLock

                                                     For Each item As myListViewItem In items
                                                         If String.IsNullOrWhiteSpace(item.hash) Then
                                                             myInvoke(Sub()
                                                                          lblProcessingFile.Text = "Now processing file " & New IO.FileInfo(item.fileName).Name & "."
                                                                          lblIndividualFilesStatusProcessingFile.Text = "Processing " & myToString(index) & " of " & myToString(listFiles.Items.Count) & If(listFiles.Items.Count = 1, " file", " files") & "."
                                                                      End Sub)

                                                             computeStopwatch = Stopwatch.StartNew

                                                             If doChecksumWithAttachedSubRoutine(item.fileName, checksumType, strChecksum, subRoutine) Then
                                                                 item.SubItems(2).Text = If(chkDisplayHashesInUpperCase.Checked, strChecksum.ToUpper, strChecksum.ToLower)
                                                                 item.computeTime = computeStopwatch.Elapsed
                                                                 item.SubItems(3).Text = timespanToHMS(item.computeTime)
                                                                 item.hash = strChecksum
                                                             Else
                                                                 item.SubItems(2).Text = "(Error while calculating checksum)"
                                                                 item.SubItems(3).Text = ""
                                                                 item.computeTime = Nothing
                                                             End If

                                                             myInvoke(Sub()
                                                                          itemOnGUI = listFiles.Items(item.Index)
                                                                          If itemOnGUI IsNot Nothing Then updateListViewItem(itemOnGUI, item)
                                                                          itemOnGUI = Nothing
                                                                      End Sub)
                                                         End If

                                                         index += 1
                                                     Next

                                                     myInvoke(Sub()
                                                                  btnIndividualFilesCopyToClipboard.Enabled = True
                                                                  btnIndividualFilesSaveResultsToDisk.Enabled = True
                                                                  radioMD5.Enabled = True
                                                                  radioSHA1.Enabled = True
                                                                  radioSHA256.Enabled = True
                                                                  radioSHA384.Enabled = True
                                                                  radioSHA512.Enabled = True

                                                                  Me.Text = "Hasher"
                                                                  resetHashIndividualFilesProgress()
                                                                  boolBackgroundThreadWorking = False
                                                                  workingThread = Nothing

                                                                  MsgBox("Completed in " & timespanToHMS(stopWatch.Elapsed) & ".", MsgBoxStyle.Information, strWindowTitle)
                                                              End Sub)
                                                 Catch ex As Threading.ThreadAbortException
                                                     myInvoke(Sub()
                                                                  If Not boolClosingWindow Then
                                                                      lblProcessingFile.Text = Nothing
                                                                      lblIndividualFilesStatus.Text = strNoBackgroundProcesses
                                                                      lblIndividualFilesStatusProcessingFile.Visible = False
                                                                      hashIndividualFilesAllFilesProgressBar.Visible = False
                                                                      IndividualFilesProgressBar.Value = 0
                                                                      IndividualFilesProgressBar.Visible = False
                                                                      ProgressForm.setTaskbarProgressBarValue(0)
                                                                      resetHashIndividualFilesProgress()
                                                                      Me.Text = "Hasher"
                                                                  End If

                                                                  boolBackgroundThreadWorking = False
                                                                  workingThread = Nothing
                                                                  If Not boolClosingWindow Then MsgBox("Processing aborted.", MsgBoxStyle.Information, strWindowTitle)
                                                              End Sub)
                                                 Finally
                                                     SyncLock threadLockingObject
                                                         ulongAllReadBytes = 0
                                                         ulongAllBytes = 0
                                                     End SyncLock

                                                     myInvoke(Sub()
                                                                  If Not boolClosingWindow Then
                                                                      btnComputeHash.Text = "Compute Hash"
                                                                      ProgressForm.setTaskbarProgressBarValue(0)
                                                                      hashIndividualFilesAllFilesProgressBar.Value = 0
                                                                  End If
                                                              End Sub)
                                                 End Try
                                             End Sub) With {
            .Priority = getThreadPriority(),
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

        lblIndividualFilesStatus.Text = strNoBackgroundProcesses
        lblIndividualFilesStatusProcessingFile.Visible = False
        hashIndividualFilesAllFilesProgressBar.Visible = False
        lblProcessingFile.Text = ""
        IndividualFilesProgressBar.Value = 0
        IndividualFilesProgressBar.Visible = False
        myInvoke(Sub() ProgressForm.setTaskbarProgressBarValue(0))
    End Sub

    Private Function strGetIndividualHashesInStringFormat(strPathOfChecksumFile As String) As String
        Dim fileInfo As New IO.FileInfo(strPathOfChecksumFile)
        Dim folderOfChecksumFile As String = If(fileInfo.DirectoryName.Length = 3, fileInfo.DirectoryName, fileInfo.DirectoryName & "\")
        Dim stringBuilder As New Text.StringBuilder()
        Dim strFile As String

        addHashFileHeader(stringBuilder)

        For Each item As myListViewItem In listFiles.Items
            If Not String.IsNullOrWhiteSpace(item.hash) Then
                strFile = item.fileName
                If chkSaveChecksumFilesWithRelativePaths.Checked Then strFile = strFile.caseInsensitiveReplace(folderOfChecksumFile, "")
                stringBuilder.AppendLine(item.hash & " *" & strFile)
            End If
        Next

        Return stringBuilder.ToString()
    End Function

    Private Function strGetIndividualHashesInStringFormat() As String
        Dim stringBuilder As New Text.StringBuilder()

        addHashFileHeader(stringBuilder)

        For Each item As myListViewItem In listFiles.Items
            If Not String.IsNullOrWhiteSpace(item.hash) Then
                stringBuilder.AppendLine(item.hash & " *" & item.fileName)
            End If
        Next

        Return stringBuilder.ToString()
    End Function

    Private Sub btnIndividualFilesCopyToClipboard_Click(sender As Object, e As EventArgs) Handles btnIndividualFilesCopyToClipboard.Click
        If copyTextToWindowsClipboard(strGetIndividualHashesInStringFormat().Trim) Then MsgBox("Your hash results have been copied to the Windows Clipboard.", MsgBoxStyle.Information, strWindowTitle)
    End Sub

    Private Shared Function copyTextToWindowsClipboard(strTextToBeCopiedToClipboard As String) As Boolean
        Try
            Clipboard.SetDataObject(strTextToBeCopiedToClipboard, True, 5, 200)
            Return True
        Catch ex As Exception
            MsgBox("Unable to open Windows Clipboard to copy text to it.", MsgBoxStyle.Critical, strWindowTitle)
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
        End If

        SaveFileDialog.InitialDirectory = strLastDirectoryWorkedOn
        SaveFileDialog.Title = "Save Hash Results to Disk"
        If My.Settings.boolAutoAddExtension Then SaveFileDialog.OverwritePrompt = False ' We handle this in our own code below.

        If SaveFileDialog.ShowDialog() = DialogResult.OK Then
            If My.Settings.boolAutoAddExtension Then
                Dim strFileExtension As String = New IO.FileInfo(SaveFileDialog.FileName).Extension

                If Not strFileExtension.Equals(".md5", StringComparison.OrdinalIgnoreCase) And Not strFileExtension.Equals(".sha1", StringComparison.OrdinalIgnoreCase) And Not strFileExtension.Equals(".sha256", StringComparison.OrdinalIgnoreCase) And Not strFileExtension.Equals(".sha384", StringComparison.OrdinalIgnoreCase) And Not strFileExtension.Equals(".sha512", StringComparison.OrdinalIgnoreCase) Then
                    If Me.radioMD5.Checked Then
                        SaveFileDialog.FileName &= ".md5"
                    ElseIf radioSHA1.Checked Then
                        SaveFileDialog.FileName &= ".sha1"
                    ElseIf radioSHA256.Checked Then
                        SaveFileDialog.FileName &= ".sha256"
                    ElseIf radioSHA384.Checked Then
                        SaveFileDialog.FileName &= ".sha384"
                    ElseIf radioSHA512.Checked Then
                        SaveFileDialog.FileName &= ".sha512"
                    End If
                End If

                If IO.File.Exists(SaveFileDialog.FileName) AndAlso MsgBox("The file named """ & New IO.FileInfo(SaveFileDialog.FileName).Name & """ already exists." & vbCrLf & vbCrLf & "Are you absolutely sure you want to replace it?", MsgBoxStyle.Question + MsgBoxStyle.YesNo + MsgBoxStyle.DefaultButton2, "Overwrite?") = MsgBoxResult.No Then
                    MsgBox("Save Results to Disk Aborted.", MsgBoxStyle.Information, strMessageBoxTitleText)
                    Exit Sub
                End If
            End If

            Using streamWriter As New IO.StreamWriter(SaveFileDialog.FileName, False, System.Text.Encoding.UTF8)
                streamWriter.Write(strGetIndividualHashesInStringFormat(SaveFileDialog.FileName))
            End Using
            MsgBox("Your hash results have been written to disk.", MsgBoxStyle.Information, strWindowTitle)
        End If
    End Sub

    Private Sub disableIndividualFilesResultsButtonsAndClearResults()
        btnComputeHash.Enabled = True
        btnIndividualFilesCopyToClipboard.Enabled = False
        btnIndividualFilesSaveResultsToDisk.Enabled = False

        listFiles.BeginUpdate()
        For Each item As myListViewItem In listFiles.Items
            item.SubItems(2).Text = strToBeComputed
            item.SubItems(3).Text = Nothing
            item.hash = Nothing
            item.computeTime = Nothing
        Next
        listFiles.EndUpdate()
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

    Private Shared Function getFileAssociation(ByVal fileExtension As String, ByRef associatedApplication As String) As Boolean
        Try
            fileExtension = fileExtension.ToLower.Trim
            If Not fileExtension.StartsWith(".") Then
                fileExtension = "." & fileExtension
            End If

            Dim subPath As String = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(fileExtension, False).GetValue(vbNullString)
            Dim rawExecutablePath As String = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(subPath & "\shell\open\command", False).GetValue(vbNullString)

            ' We use this to parse out the executable path out of the regular junk in the string.
            Dim matches As Text.RegularExpressions.Match = System.Text.RegularExpressions.Regex.Match(rawExecutablePath, "(""{0, 1}[A-Za-z]:  \\.*\.(?:bat|bin|cmd|com|cpl|exe|gadget|inf1|ins|inx|isu|job|jse|lnk|msc|msi|msp|mst|paf|pif|ps1|reg|rgs|sct|shb|shs|u3p|vb|vbe|vbs|vbscript|ws|wsf)""{0,1})", System.Text.RegularExpressions.RegexOptions.IgnoreCase)

            associatedApplication = matches.Groups(1).Value.Trim ' And return the value.
            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function

    Private Sub launchURLInWebBrowser(url As String, Optional errorMessage As String = "An error occurred when trying the URL In your Default browser. The URL has been copied to your Windows Clipboard for you to paste into the address bar in the web browser of your choice.")
        If Not url.Trim.StartsWith("http", StringComparison.OrdinalIgnoreCase) Then url = If(chkSSL.Checked, "https://" & url, "http://" & url)

        Try
            Dim associatedApplication As String = Nothing

            If Not getFileAssociation(".html", associatedApplication) Then
                Process.Start(url)
            Else
                If IO.File.Exists(associatedApplication) Then
                    Process.Start(associatedApplication, Chr(34) & url & Chr(34))
                Else
                    Process.Start(url)
                End If
            End If
        Catch ex2 As ComponentModel.Win32Exception
            MsgBox("There was an error attempting to launch your web browser. Perhaps rebooting your system will correct this issue.", MsgBoxStyle.Information, strWindowTitle)
        Catch ex As Exception
            copyTextToWindowsClipboard(url)
            MsgBox(errorMessage, MsgBoxStyle.Information, strWindowTitle)
        End Try
    End Sub

    Private Sub btnDonate_Click(sender As Object, e As EventArgs) Handles btnDonate.Click
        launchURLInWebBrowser(strPayPal)
    End Sub

    Private Sub sendToIPCNamedPipeServer(strFileName As String)
        Try
            Using memoryStream As New IO.MemoryStream(System.Text.Encoding.UTF8.GetBytes(strFileName))
                Dim namedPipeDataStream As New NamedPipeClientStream(".", strNamedPipeServerName, PipeDirection.Out, PipeOptions.Asynchronous)
                namedPipeDataStream.Connect(5000)
                memoryStream.CopyTo(namedPipeDataStream)
            End Using
        Catch ex As IO.IOException
            MsgBox("There was an error sending data to the named pipe server used for interprocess communication, please close all Hasher instances and try again.", MsgBoxStyle.Critical, strWindowTitle)
        End Try
    End Sub

    ''' <summary>Creates a named pipe server. Returns a Boolean value indicating if the function was able to create a named pipe server.</summary>
    ''' <returns>Returns a Boolean value indicating if the function was able to create a named pipe server.</returns>
    Private Function startNamedPipeServer() As Boolean
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
    Private Sub addFileOrDirectoryToHashFileList(strReceivedFileName As String)
        Try
            If IO.File.Exists(strReceivedFileName) Or IO.Directory.Exists(strReceivedFileName) Then
                If IO.File.GetAttributes(strReceivedFileName).HasFlag(IO.FileAttributes.Directory) Then
                    myInvoke(Sub()
                                 TabControl1.SelectTab(2)
                                 NativeMethod.NativeMethods.SetForegroundWindow(Handle.ToInt32())
                             End Sub)

                    addFilesFromDirectory(strReceivedFileName)
                Else
                    If Not filesInListFiles.Contains(strReceivedFileName.Trim.ToLower) Then
                        myInvoke(Sub()
                                     TabControl1.SelectTab(2)
                                     NativeMethod.NativeMethods.SetForegroundWindow(Handle.ToInt32())
                                 End Sub)

                        strLastDirectoryWorkedOn = New IO.FileInfo(strReceivedFileName).DirectoryName
                        listFiles.Items.Add(createListFilesObject(strReceivedFileName))
                    End If
                End If

                updateFilesListCountHeader()
                If chkSortFileListingAfterAddingFilesToHash.Checked Then applyFileSizeSortingToHashList()
            End If
        Catch ex As Exception
        End Try
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim boolNamedPipeServerStarted As Boolean = startNamedPipeServer()
        Dim commandLineArgument As String

        With My.Application
            If .CommandLineArgs.Count = 1 Then
                commandLineArgument = .CommandLineArgs(0).Trim

                If commandLineArgument.StartsWith("--addfile=", StringComparison.OrdinalIgnoreCase) Then
                    commandLineArgument = commandLineArgument.caseInsensitiveReplace("--addfile=", "").Replace(Chr(34), "")

                    If boolNamedPipeServerStarted Then
                        ' In this case this instance of the program is the first executed instance so it has a named pipe server running
                        ' in it, but we still need to process the first incoming file passed to it via command line arguments.
                        addFileOrDirectoryToHashFileList(commandLineArgument)
                    Else
                        ' OK, there's already a named pipe server running so we send the file that's been passed to this
                        ' instance via the command line argument to the first instance via the IPC named pipe server
                        ' and then exit out of this instance in a very quick way by killing this current process.
                        sendToIPCNamedPipeServer(commandLineArgument)
                        Process.GetCurrentProcess.Kill()
                    End If
                End If
            End If
        End With

        Me.Icon = Icon.ExtractAssociatedIcon(Reflection.Assembly.GetExecutingAssembly().Location)

        If areWeAnAdministrator() Then
            Me.Text &= " (WARNING!!! Running as Administrator)"
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
        lblFile1Hash.Text = Nothing
        lblFile2Hash.Text = Nothing
        lblProcessingFile.Text = ""
        lblProcessingFileVerify.Text = ""
        lblCompareFileAgainstKnownHashType.Text = ""
        chkRecurrsiveDirectorySearch.Checked = My.Settings.boolRecurrsiveDirectorySearch
        chkSSL.Checked = My.Settings.boolSSL
        chkSortByFileSizeAfterLoadingHashFile.Checked = My.Settings.boolSortByFileSizeAfterLoadingHashFile
        chkSortFileListingAfterAddingFilesToHash.Checked = My.Settings.boolSortFileListingAfterAddingFilesToHash
        chkSaveChecksumFilesWithRelativePaths.Checked = My.Settings.boolSaveChecksumFilesWithRelativePaths
        chkUseMilliseconds.Checked = My.Settings.boolUseMilliseconds
        chkDisplayHashesInUpperCase.Checked = My.Settings.boolDisplayHashesInUpperCase
        chkUseCommasInNumbers.Checked = My.Settings.boolUseCommasInNumbers
        boolShowEstimatedTime = My.Settings.boolShowEstimatedTime
        chkShowEstimatedTimeRemaining.Checked = boolShowEstimatedTime
        chkCheckForUpdates.Checked = My.Settings.boolCheckForUpdates
        chkAutoAddExtension.Checked = My.Settings.boolAutoAddExtension
        lblWelcomeText.Text = String.Format(lblWelcomeText.Text,
                                            Check_for_Update_Stuff.versionString,
                                            If(Environment.Is64BitProcess, "64", "32"),
                                            If(Environment.Is64BitOperatingSystem, "64", "32")
                                           )
        Me.Size = My.Settings.windowSize
        validColor = My.Settings.validColor
        lblValidColor.BackColor = validColor
        notValidColor = My.Settings.notValidColor
        lblNotValidColor.BackColor = notValidColor
        fileNotFoundColor = My.Settings.fileNotFoundColor
        lblFileNotFoundColor.BackColor = fileNotFoundColor
        bufferSize.Value = My.Settings.shortBufferSize
        btnSetBufferSize.Enabled = False

        If boolDebugMode Then
            btnAddHasherToAllFiles.Visible = False
            btnAssociate.Visible = False
        End If

        deleteTemporaryNewEXEFile()

        With My.Application
            If .CommandLineArgs.Count = 1 Then
                commandLineArgument = .CommandLineArgs(0).Trim

                If commandLineArgument.StartsWith("--hashfile=", StringComparison.OrdinalIgnoreCase) Then
                    commandLineArgument = commandLineArgument.caseInsensitiveReplace("--hashfile=", "")
                    commandLineArgument = commandLineArgument.Replace(Chr(34), "")

                    If IO.File.Exists(commandLineArgument) Then
                        TabControl1.SelectTab(3)
                        btnOpenExistingHashFile.Text = "Abort Processing"
                        verifyHashesListFiles.Items.Clear()
                        processExistingHashFile(commandLineArgument)
                    End If
                ElseIf commandLineArgument.StartsWith("--knownhashfile=", StringComparison.OrdinalIgnoreCase) Then
                    commandLineArgument = commandLineArgument.caseInsensitiveReplace("--knownhashfile=", "")
                    commandLineArgument = commandLineArgument.Replace(Chr(34), "")
                    TabControl1.SelectTab(5)
                    txtFileForKnownHash.Text = commandLineArgument
                    txtKnownHash.Select()
                End If
            End If
        End With

        colFileName.Width = My.Settings.hashIndividualFilesFileNameColumnSize
        colFileSize.Width = My.Settings.hashIndividualFilesFileSizeColumnSize
        colChecksum.Width = My.Settings.hashIndividualFilesChecksumColumnSize
        colComputeTime.Width = My.Settings.hashIndividualFilesComputeTimeColumnSize

        colFile.Width = My.Settings.verifyHashFileNameColumnSize
        colFileSize2.Width = My.Settings.verifyHashFileSizeColumnSize
        colResults.Width = My.Settings.verifyHashFileResults
        colComputeTime2.Width = My.Settings.verifyHashComputeTimeColumnSize
        If My.Settings.taskPriority > 4 Then My.Settings.taskPriority = Byte.Parse(4)
        taskPriority.SelectedIndex = My.Settings.taskPriority

        If My.Settings.boolCheckForUpdates Then
            Threading.ThreadPool.QueueUserWorkItem(Sub()
                                                       Dim checkForUpdatesClassObject As New Check_for_Update_Stuff(Me)
                                                       checkForUpdatesClassObject.checkForUpdates(False)
                                                   End Sub)
        End If

        boolDoneLoading = True
    End Sub

    Private Sub deleteTemporaryNewEXEFile()
        Try
            Dim newExecutableName As String = New IO.FileInfo(Application.ExecutablePath).Name & ".new.exe"
            If IO.File.Exists(newExecutableName) Then
                searchForProcessAndKillIt(newExecutableName, False)
                IO.File.Delete(newExecutableName)
            End If
        Catch ex As Exception
        End Try
    End Sub

    Private Sub recursiveDirectorySearch(ByVal strDirectory As String, ByRef collectionOfListViewItems As List(Of ListViewItem))
        Try
            For Each strFileName As String In IO.Directory.EnumerateFiles(strDirectory)
                addFileToList(strFileName, collectionOfListViewItems)
            Next
        Catch ex As Exception
        End Try

        Try
            For Each directory As String In IO.Directory.EnumerateDirectories(strDirectory)
                recursiveDirectorySearch(directory, collectionOfListViewItems)
            Next
        Catch ex As Exception
        End Try
    End Sub

    Sub addFileToList(strFileName As String, ByRef collectionOfListViewItems As List(Of ListViewItem))
        If Not filesInListFiles.Contains(strFileName.Trim.ToLower) Then
            collectionOfListViewItems.Add(createListFilesObject(strFileName))
        End If
    End Sub

    Private Sub addFilesFromDirectory(directoryPath As String)
        Threading.ThreadPool.QueueUserWorkItem(Sub()
                                                   strLastDirectoryWorkedOn = directoryPath
                                                   Dim collectionOfListViewItems As New List(Of ListViewItem)
                                                   Dim index As Integer = 0

                                                   myInvoke(Sub()
                                                                btnAddIndividualFiles.Enabled = False
                                                                btnAddFilesInFolder.Enabled = False
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

                                                                lblIndividualFilesStatus.Text = "Enumerating files in directory... Please Wait."
                                                                btnAddFilesInFolder.Enabled = False
                                                            End Sub)

                                                   If chkRecurrsiveDirectorySearch.Checked Then
                                                       recursiveDirectorySearch(directoryPath, collectionOfListViewItems)
                                                   Else
                                                       For Each strFileName As String In IO.Directory.EnumerateFiles(directoryPath)
                                                           addFileToList(strFileName, collectionOfListViewItems)
                                                       Next
                                                   End If

                                                   myInvoke(Sub()
                                                                lblIndividualFilesStatusProcessingFile.Visible = True
                                                                lblIndividualFilesStatusProcessingFile.Text = "Adding files to list... Please Wait."

                                                                listFiles.BeginUpdate()
                                                                listFiles.Items.AddRange(collectionOfListViewItems.ToArray())
                                                                If chkSortFileListingAfterAddingFilesToHash.Checked Then applyFileSizeSortingToHashList()
                                                                listFiles.EndUpdate()

                                                                lblIndividualFilesStatusProcessingFile.Text = Nothing
                                                                lblIndividualFilesStatus.Text = strNoBackgroundProcesses
                                                                IndividualFilesProgressBar.Value = 0
                                                                IndividualFilesProgressBar.Visible = False
                                                                ProgressForm.setTaskbarProgressBarValue(0)
                                                                btnAddFilesInFolder.Enabled = True

                                                                updateFilesListCountHeader()

                                                                btnAddIndividualFiles.Enabled = True
                                                                btnAddFilesInFolder.Enabled = True
                                                                btnRemoveSelectedFiles.Enabled = True
                                                                btnRemoveAllFiles.Enabled = True
                                                                radioSHA256.Enabled = True
                                                                radioSHA384.Enabled = True
                                                                radioSHA512.Enabled = True
                                                                radioSHA1.Enabled = True
                                                                radioMD5.Enabled = True
                                                                btnComputeHash.Enabled = True
                                                                btnIndividualFilesCopyToClipboard.Enabled = True
                                                                btnIndividualFilesSaveResultsToDisk.Enabled = True
                                                            End Sub)
                                               End Sub)
    End Sub

    Private Sub btnAddFilesInFolder_Click(sender As Object, e As EventArgs) Handles btnAddFilesInFolder.Click
        If FolderBrowserDialog.ShowDialog = DialogResult.OK Then addFilesFromDirectory(FolderBrowserDialog.SelectedPath)
    End Sub

    Private Shared Function createListViewItemForHashFileEntry(strFileName As String, strChecksum As String, ByRef intFilesNotFound As Integer) As myListViewItem
        Dim listViewItem As New myListViewItem(strFileName) With {.hash = strChecksum, .fileName = strFileName}

        With listViewItem
            If IO.File.Exists(strFileName) Then
                .fileSize = New IO.FileInfo(strFileName).Length
                SyncLock threadLockingObject
                    ulongAllBytes += .fileSize
                End SyncLock
                .SubItems.Add(fileSizeToHumanSize(listViewItem.fileSize))
                .SubItems.Add("To Be Tested")
                .SubItems.Add("To Be Tested")
                .boolFileExists = True
            Else
                .fileSize = -1
                .computeTime = Nothing
                .SubItems.Add("")
                .SubItems.Add("Doesn't Exist")
                .SubItems.Add("")
                .boolFileExists = False
                .BackColor = Color.LightGray
                intFilesNotFound += 1
            End If
        End With

        Return listViewItem
    End Function

    Private Sub processExistingHashFile(strPathToChecksumFile As String)
        lblVerifyFileNameLabel.Text = "File Name: " & strPathToChecksumFile

        Dim checksumType As checksumType
        Dim checksumFileInfo As New IO.FileInfo(strPathToChecksumFile)
        Dim strChecksumFileExtension, strDirectoryThatContainsTheChecksumFile As String

        strChecksumFileExtension = checksumFileInfo.Extension
        strDirectoryThatContainsTheChecksumFile = checksumFileInfo.DirectoryName
        checksumFileInfo = Nothing

        If strChecksumFileExtension.Equals(".md5", StringComparison.OrdinalIgnoreCase) Then
            checksumType = checksumType.md5
        ElseIf strChecksumFileExtension.Equals(".sha1", StringComparison.OrdinalIgnoreCase) Then
            checksumType = checksumType.sha160
        ElseIf strChecksumFileExtension.Equals(".sha256", StringComparison.OrdinalIgnoreCase) Then
            checksumType = checksumType.sha256
        ElseIf strChecksumFileExtension.Equals(".sha384", StringComparison.OrdinalIgnoreCase) Then
            checksumType = checksumType.sha384
        ElseIf strChecksumFileExtension.Equals(".sha512", StringComparison.OrdinalIgnoreCase) Then
            checksumType = checksumType.sha512
        Else
            MsgBox("Invalid Hash File Type.", MsgBoxStyle.Critical, strWindowTitle)
            Exit Sub
        End If

        workingThread = New Threading.Thread(Sub()
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
                                                     Dim strReadingHashFileMessage As String = "Reading hash file into memory and creating ListView item objects... Please Wait."

                                                     myInvoke(Sub()
                                                                  lblVerifyHashStatus.Text = strReadingHashFileMessage
                                                                  verifyIndividualFilesAllFilesProgressBar.Visible = True
                                                                  VerifyHashProgressBar.Visible = True
                                                                  verifyHashesListFiles.BeginUpdate()
                                                              End Sub)

                                                     SyncLock threadLockingObject
                                                         ulongAllReadBytes = 0
                                                         ulongAllBytes = 0
                                                     End SyncLock

                                                     For Each strLineInFile As String In dataInFileArray
                                                         intLineCounter += 1
                                                         myInvoke(Sub()
                                                                      VerifyHashProgressBar.Value = intLineCounter / dataInFileArray.LongLength * 100
                                                                      ProgressForm.setTaskbarProgressBarValue(VerifyHashProgressBar.Value)
                                                                      lblVerifyHashStatus.Text = strReadingHashFileMessage & " Processing item " & myToString(intLineCounter) & " of " & myToString(dataInFileArray.LongLength) & " (" & VerifyHashProgressBar.Value & "%)."
                                                                  End Sub)

                                                         If Not String.IsNullOrEmpty(strLineInFile) Then
                                                             regExMatchObject = hashLineParser.Match(strLineInFile)

                                                             If regExMatchObject.Success Then
                                                                 strChecksum = regExMatchObject.Groups(1).Value
                                                                 strFileName = regExMatchObject.Groups(2).Value

                                                                 If Not hashLineFilePathChecker.IsMatch(strFileName) Then
                                                                     strFileName = IO.Path.Combine(strDirectoryThatContainsTheChecksumFile, strFileName)
                                                                 End If

                                                                 myInvoke(Sub() verifyHashesListFiles.Items.Add(createListViewItemForHashFileEntry(strFileName, strChecksum, intFilesNotFound)))
                                                             End If

                                                             regExMatchObject = Nothing
                                                         End If
                                                     Next

                                                     myInvoke(Sub()
                                                                  verifyHashesListFiles.EndUpdate()
                                                                  Me.Text = "Hasher"
                                                                  If chkSortByFileSizeAfterLoadingHashFile.Checked Then applyFileSizeSortingToVerifyList()
                                                                  lblVerifyHashStatus.Text = strNoBackgroundProcesses
                                                                  VerifyHashProgressBar.Value = 0
                                                                  ProgressForm.setTaskbarProgressBarValue(0)
                                                                  lblVerifyHashStatusProcessingFile.Visible = True
                                                              End Sub)

                                                     dataInFileArray = Nothing

                                                     Dim items As ListView.ListViewItemCollection = getListViewItems(verifyHashesListFiles)
                                                     Dim strChecksumInFile As String = Nothing
                                                     Dim percentage, allBytesPercentage As Double
                                                     Dim subRoutine As [Delegate]
                                                     Dim computeStopwatch As Stopwatch
                                                     Dim itemOnGUI As myListViewItem

                                                     For Each item As myListViewItem In items
                                                         myInvoke(Sub() lblVerifyHashStatusProcessingFile.Text = String.Format("Processing file {0} of {1} {2}",
                                                                                                                               myToString(index),
                                                                                                                               myToString(verifyHashesListFiles.Items.Count),
                                                                                                                               If(verifyHashesListFiles.Items.Count = 1, "file", "files"))
                                                                                                                              )

                                                         If item.boolFileExists Then
                                                             strChecksum = item.hash
                                                             strFileName = item.fileName

                                                             If IO.File.Exists(strFileName) Then
                                                                 subRoutine = Sub(size As Long, totalBytesRead As Long, eta As TimeSpan)
                                                                                  Try
                                                                                      myInvoke(Sub()
                                                                                                   percentage = If(totalBytesRead <> 0 And size <> 0, totalBytesRead / size * 100, 0)
                                                                                                   VerifyHashProgressBar.Value = percentage
                                                                                                   SyncLock threadLockingObject
                                                                                                       allBytesPercentage = ulongAllReadBytes / ulongAllBytes * 100
                                                                                                   End SyncLock
                                                                                                   ProgressForm.setTaskbarProgressBarValue(allBytesPercentage)
                                                                                                   verifyIndividualFilesAllFilesProgressBar.Value = allBytesPercentage
                                                                                                   lblVerifyHashStatus.Text = fileSizeToHumanSize(totalBytesRead) & " of " & fileSizeToHumanSize(size) & " (" & Math.Round(percentage, 2) & "%) have been processed."
                                                                                                   If boolShowEstimatedTime AndAlso eta <> TimeSpan.Zero Then lblVerifyHashStatus.Text &= " Estimated " & timespanToHMS(eta) & " remaining."
                                                                                               End Sub)
                                                                                  Catch ex As Exception
                                                                                  End Try
                                                                              End Sub

                                                                 myInvoke(Sub() lblProcessingFileVerify.Text = "Now processing file " & New IO.FileInfo(strFileName).Name & ".")
                                                                 computeStopwatch = Stopwatch.StartNew

                                                                 If doChecksumWithAttachedSubRoutine(strFileName, checksumType, strChecksumInFile, subRoutine) Then
                                                                     If strChecksum.Equals(item.hash, StringComparison.OrdinalIgnoreCase) Then
                                                                         item.color = validColor
                                                                         item.SubItems(2).Text = "Valid"
                                                                         item.computeTime = computeStopwatch.Elapsed
                                                                         item.SubItems(3).Text = timespanToHMS(item.computeTime)
                                                                         longFilesThatPassedVerification += 1
                                                                     Else
                                                                         item.color = notValidColor
                                                                         item.SubItems(2).Text = "NOT Valid"
                                                                     End If
                                                                 Else
                                                                     item.color = fileNotFoundColor
                                                                     item.SubItems(2).Text = "(Error while calculating checksum)"
                                                                 End If

                                                                 subRoutine = Nothing

                                                                 myInvoke(Sub()
                                                                              itemOnGUI = verifyHashesListFiles.Items(item.Index)
                                                                              If itemOnGUI IsNot Nothing Then updateListViewItem(itemOnGUI, item)
                                                                              itemOnGUI = Nothing
                                                                          End Sub)
                                                             End If
                                                         End If

                                                         index += 1
                                                     Next

                                                     myInvoke(Sub()
                                                                  For Each item As myListViewItem In verifyHashesListFiles.Items
                                                                      If item.boolFileExists Then item.BackColor = item.color
                                                                  Next

                                                                  lblVerifyHashStatusProcessingFile.Visible = False
                                                                  verifyIndividualFilesAllFilesProgressBar.Visible = False
                                                                  lblVerifyHashStatus.Text = strNoBackgroundProcesses
                                                                  lblProcessingFileVerify.Text = ""
                                                                  VerifyHashProgressBar.Value = 0
                                                                  VerifyHashProgressBar.Visible = False
                                                                  ProgressForm.setTaskbarProgressBarValue(0)
                                                                  Me.Text = "Hasher"

                                                                  Dim sbMessageBoxText As New Text.StringBuilder

                                                                  If intFilesNotFound = 0 Then
                                                                      If longFilesThatPassedVerification = verifyHashesListFiles.Items.Count Then
                                                                          sbMessageBoxText.AppendLine("Processing of hash file complete. All files have passed verification.")
                                                                      Else
                                                                          Dim intFilesThatDidNotPassVerification As Integer = verifyHashesListFiles.Items.Count - longFilesThatPassedVerification
                                                                          sbMessageBoxText.AppendLine(String.Format("Processing of hash file complete. {0} out of {1} file(s) passed verification, {2} {3} didn't pass verification.",
                                                                                                                    myToString(longFilesThatPassedVerification),
                                                                                                                    myToString(verifyHashesListFiles.Items.Count),
                                                                                                                    myToString(intFilesThatDidNotPassVerification),
                                                                                                                    If(intFilesThatDidNotPassVerification = 1, "file", "files")
                                                                                                                   )
                                                                           )
                                                                      End If
                                                                  Else
                                                                      sbMessageBoxText.AppendLine("Processing of hash file complete.")
                                                                      sbMessageBoxText.AppendLine()

                                                                      Dim intTotalFiles As Integer = verifyHashesListFiles.Items.Count - intFilesNotFound
                                                                      If longFilesThatPassedVerification = intTotalFiles Then
                                                                          sbMessageBoxText.AppendLine(String.Format("All files have passed verification. Unfortunately, {0} {1} were not found.",
                                                                                                                    myToString(intFilesNotFound),
                                                                                                                    If(intFilesNotFound = 1, "file", "files")
                                                                                                                   )
                                                                           )
                                                                      Else
                                                                          Dim intFilesThatDidNotPassVerification As Integer = intTotalFiles - longFilesThatPassedVerification
                                                                          sbMessageBoxText.AppendLine(String.Format("Not all of the files passed verification, only {0} out of {1} {2} passed verification, Unfortunately, {3} {4} didn't pass verification and {5} {6} were not found.",
                                                                                                                    myToString(longFilesThatPassedVerification),
                                                                                                                    myToString(intTotalFiles),
                                                                                                                    If(intTotalFiles = 1, "file", "files"),
                                                                                                                    myToString(intFilesThatDidNotPassVerification),
                                                                                                                    If(intFilesThatDidNotPassVerification = 1, "file", "files"),
                                                                                                                    myToString(intFilesNotFound),
                                                                                                                    If(intFilesNotFound = 1, "file", "files")
                                                                                                                   )
                                                                           )
                                                                      End If
                                                                  End If

                                                                  sbMessageBoxText.AppendLine()
                                                                  sbMessageBoxText.AppendLine("Processing completed in " & timespanToHMS(stopWatch.Elapsed) & ".")

                                                                  MsgBox(sbMessageBoxText.ToString.Trim, MsgBoxStyle.Information, strWindowTitle)
                                                              End Sub)

                                                     boolBackgroundThreadWorking = False
                                                     workingThread = Nothing
                                                 Catch ex As Threading.ThreadAbortException
                                                     myInvoke(Sub()
                                                                  If Not boolClosingWindow Then
                                                                      verifyHashesListFiles.EndUpdate()
                                                                      lblVerifyHashStatusProcessingFile.Visible = False
                                                                      verifyIndividualFilesAllFilesProgressBar.Visible = False
                                                                      lblVerifyHashStatus.Text = strNoBackgroundProcesses
                                                                      lblProcessingFileVerify.Text = ""
                                                                      VerifyHashProgressBar.Value = 0
                                                                      VerifyHashProgressBar.Visible = False
                                                                      ProgressForm.setTaskbarProgressBarValue(0)
                                                                      verifyHashesListFiles.Items.Clear()
                                                                      Me.Text = "Hasher"
                                                                      lblVerifyFileNameLabel.Text = "File Name: (None Selected for Processing)"
                                                                  End If

                                                                  boolBackgroundThreadWorking = False
                                                                  workingThread = Nothing
                                                                  If Not boolClosingWindow Then MsgBox("Processing aborted.", MsgBoxStyle.Information, strWindowTitle)
                                                              End Sub)
                                                 Finally
                                                     SyncLock threadLockingObject
                                                         ulongAllReadBytes = 0
                                                         ulongAllBytes = 0
                                                     End SyncLock

                                                     myInvoke(Sub()
                                                                  If Not boolClosingWindow Then
                                                                      btnOpenExistingHashFile.Text = "Open Hash File"
                                                                      ProgressForm.setTaskbarProgressBarValue(0)
                                                                      verifyIndividualFilesAllFilesProgressBar.Value = 0
                                                                  End If
                                                              End Sub)
                                                 End Try
                                             End Sub) With {
            .Priority = getThreadPriority(),
            .Name = "Verify Hash File Working Thread",
            .IsBackground = True
        }
        workingThread.Start()
    End Sub

    Private Sub btnOpenExistingHashFile_Click(sender As Object, e As EventArgs) Handles btnOpenExistingHashFile.Click
        If btnOpenExistingHashFile.Text = "Abort Processing" Then
            If workingThread IsNot Nothing Then
                workingThread.Abort()
                boolBackgroundThreadWorking = False
            End If

            Exit Sub
        End If

        btnOpenExistingHashFile.Text = "Abort Processing"
        verifyHashesListFiles.Items.Clear()

        Dim oldMultiValue As Boolean = OpenFileDialog.Multiselect

        OpenFileDialog.Title = "Select a hash file to verify..."
        OpenFileDialog.Multiselect = False
        OpenFileDialog.Filter = "Checksum File|*.md5;*.sha1;*.sha256;*.sha384;*.sha512;*.ripemd160"

        If OpenFileDialog.ShowDialog() = DialogResult.OK Then
            processExistingHashFile(OpenFileDialog.FileName)
        Else
            btnOpenExistingHashFile.Text = "Open Hash File"
        End If

        OpenFileDialog.Multiselect = oldMultiValue
    End Sub

    Private Sub listFiles_DragDrop(sender As Object, e As DragEventArgs) Handles listFiles.DragDrop
        For Each strItem As String In e.Data.GetData(DataFormats.FileDrop)
            If IO.File.Exists(strItem) Or IO.Directory.Exists(strItem) Then
                If IO.File.GetAttributes(strItem).HasFlag(IO.FileAttributes.Directory) Then
                    addFilesFromDirectory(strItem)
                Else
                    If Not filesInListFiles.Contains(strItem.Trim.ToLower) Then
                        listFiles.Items.Add(createListFilesObject(strItem))
                    End If
                End If
            End If
        Next

        updateFilesListCountHeader()
        If chkSortFileListingAfterAddingFilesToHash.Checked Then applyFileSizeSortingToHashList()
    End Sub

    Private Sub listFiles_DragEnter(sender As Object, e As DragEventArgs) Handles listFiles.DragEnter
        e.Effect = If(e.Data.GetDataPresent(DataFormats.FileDrop), DragDropEffects.All, DragDropEffects.None)
    End Sub

    Private Sub chkRecurrsiveDirectorySearch_Click(sender As Object, e As EventArgs) Handles chkRecurrsiveDirectorySearch.Click
        My.Settings.boolRecurrsiveDirectorySearch = chkRecurrsiveDirectorySearch.Checked
    End Sub

    Private Sub txtTextToHash_TextChanged(sender As Object, e As EventArgs) Handles txtTextToHash.TextChanged
        lblHashTextStep1.Text = "Step 1: Input some text: " & myToString(txtTextToHash.Text.Length) & " " & If(txtTextToHash.Text.Length = 1, "Character", "Characters")
        btnComputeTextHash.Enabled = If(txtTextToHash.Text.Length = 0, False, True)
        clearTextHashResults()
    End Sub

    Private Sub clearTextHashResults()
        btnCopyTextHashResultsToClipboard.Enabled = False
        txtHashResults.Text = Nothing
    End Sub

    Private Sub btnComputeTextHash_Click(sender As Object, e As EventArgs) Handles btnComputeTextHash.Click
        Dim strHash As String = Nothing

        If textRadioMD5.Checked Then
            strHash = getHashOfString(txtTextToHash.Text, checksumType.md5)
        ElseIf textRadioSHA1.Checked Then
            strHash = getHashOfString(txtTextToHash.Text, checksumType.sha160)
        ElseIf textRadioSHA256.Checked Then
            strHash = getHashOfString(txtTextToHash.Text, checksumType.sha256)
        ElseIf textRadioSHA384.Checked Then
            strHash = getHashOfString(txtTextToHash.Text, checksumType.sha384)
        ElseIf textRadioSHA512.Checked Then
            strHash = getHashOfString(txtTextToHash.Text, checksumType.sha512)
        End If

        txtHashResults.Text = If(chkDisplayHashesInUpperCase.Checked, strHash.ToUpper, strHash.ToLower)
        btnCopyTextHashResultsToClipboard.Enabled = True
    End Sub

    Private Sub btnPasteTextFromWindowsClipboard_Click(sender As Object, e As EventArgs) Handles btnPasteTextFromWindowsClipboard.Click
        txtTextToHash.Text = Clipboard.GetText()
    End Sub

    Private Sub btnCopyTextHashResultsToClipboard_Click(sender As Object, e As EventArgs) Handles btnCopyTextHashResultsToClipboard.Click
        If copyTextToWindowsClipboard(txtHashResults.Text) Then MsgBox("Your hash results have been copied to the Windows Clipboard.", MsgBoxStyle.Information, strWindowTitle)
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

    Private Sub textRadioSHA1_CheckedChanged(sender As Object, e As EventArgs) Handles textRadioSHA1.CheckedChanged
        clearTextHashResults()
    End Sub

    Private Sub textRadioMD5_CheckedChanged(sender As Object, e As EventArgs) Handles textRadioMD5.CheckedChanged
        clearTextHashResults()
    End Sub

    Private Sub TabControl1_Selecting(sender As Object, e As TabControlCancelEventArgs) Handles TabControl1.Selecting
        If e.TabPageIndex = 5 Then
            pictureBoxVerifyAgainstResults.Image = Nothing
            ToolTip.SetToolTip(pictureBoxVerifyAgainstResults, "")
            txtFileForKnownHash.Text = Nothing
            txtKnownHash.Text = Nothing
            lblCompareFileAgainstKnownHashType.Text = Nothing
        End If

        If boolBackgroundThreadWorking AndAlso MsgBox("Checksum hashes are being computed, do you want to abort?", MsgBoxStyle.YesNo + MsgBoxStyle.Question, strWindowTitle) = MsgBoxResult.No Then
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
        If boolBackgroundThreadWorking AndAlso MsgBox("Checksum hashes are being computed, do you want to abort?", MsgBoxStyle.YesNo + MsgBoxStyle.Question, strWindowTitle) = MsgBoxResult.No Then
            e.Cancel = True
            Exit Sub
        Else
            If workingThread IsNot Nothing Then
                boolClosingWindow = True

                If pipeServer IsNot Nothing Then
                    pipeServer.Disconnect()
                    pipeServer.Close()
                End If

                workingThread.Abort()
            End If
        End If
    End Sub

    Private Sub listFiles_ColumnClick(sender As Object, e As ColumnClickEventArgs) Handles listFiles.ColumnClick
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
            If String.IsNullOrWhiteSpace(DirectCast(listFiles.SelectedItems(0), myListViewItem).hash) Then e.Cancel = True

            If listFiles.SelectedItems.Count = 1 Then
                CopyHashToClipboardToolStripMenuItem.Text = "Copy Selected Hash to Clipboard"
            ElseIf listFiles.SelectedItems.Count > 1 Then
                CopyHashToClipboardToolStripMenuItem.Text = "Copy Selected Hashes to Clipboard"
            End If
        End If
    End Sub

    Private Shared Sub addHashFileHeader(ByRef stringBuilder As Text.StringBuilder)
        stringBuilder.AppendLine("'")
        stringBuilder.AppendLine("' Hash file generated by Hasher, version " & Check_for_Update_Stuff.versionString & ". Written by Tom Parkison.")
        stringBuilder.AppendLine("' Web Site: https://www.toms-world.org/blog/hasher")
        stringBuilder.AppendLine("' Source Code Available At: https://bitbucket.org/trparky/hasher")
        stringBuilder.AppendLine("'")
    End Sub

    Private Sub CopyHashToClipboardToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CopyHashToClipboardToolStripMenuItem.Click
        If listFiles.SelectedItems.Count = 1 Then
            Dim selectedItem As myListViewItem = listFiles.SelectedItems(0)
            If copyTextToWindowsClipboard(If(chkDisplayHashesInUpperCase.Checked, selectedItem.hash.ToUpper, selectedItem.hash.ToLower)) Then MsgBox("The hash result has been copied to the Windows Clipboard.", MsgBoxStyle.Information, strWindowTitle)
        Else
            Dim stringBuilder As New Text.StringBuilder
            addHashFileHeader(stringBuilder)

            For Each item As myListViewItem In listFiles.SelectedItems
                stringBuilder.AppendLine(If(chkDisplayHashesInUpperCase.Checked, item.hash.ToUpper, item.hash.ToLower) & " *" & item.fileName)
            Next

            If copyTextToWindowsClipboard(stringBuilder.ToString.Trim) Then MsgBox("The hash result has been copied to the Windows Clipboard.", MsgBoxStyle.Information, strWindowTitle)
        End If
    End Sub

    Private Sub applyFileSizeSortingToHashList()
        colFileName.Text = "File Name"
        colFileSize.Text = "File Size"
        colChecksum.Text = "Hash/Checksum"
        colComputeTime.Text = "Compute Time"

        Dim new_sorting_column As ColumnHeader = listFiles.Columns(1)
        Dim sort_order As SortOrder = SortOrder.Ascending

        m_SortingColumn2 = new_sorting_column
        m_SortingColumn2.Text = "> File Size"

        listFiles.ListViewItemSorter = New ListViewComparer(1, sort_order)
        listFiles.Sort()
    End Sub

    Private Sub applyFileSizeSortingToVerifyList()
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

    Private Sub verifyHashesListFiles_ColumnClick(sender As Object, e As ColumnClickEventArgs) Handles verifyHashesListFiles.ColumnClick
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

    Private Sub btnCheckForUpdates_Click_1(sender As Object, e As EventArgs) Handles btnCheckForUpdates.Click
        Threading.ThreadPool.QueueUserWorkItem(Sub()
                                                   Dim checkForUpdatesClassObject As New Check_for_Update_Stuff(Me)
                                                   checkForUpdatesClassObject.checkForUpdates()
                                               End Sub)
    End Sub

    Private Sub btnCompareFiles_Click(sender As Object, e As EventArgs) Handles btnCompareFiles.Click
        If btnCompareFiles.Text = "Abort Processing" Then
            If workingThread IsNot Nothing Then
                workingThread.Abort()
                boolBackgroundThreadWorking = False
            End If

            Exit Sub
        End If

        If txtFile1.Text.Equals(txtFile2.Text, StringComparison.OrdinalIgnoreCase) Then
            MsgBox("Please select two different files.", MsgBoxStyle.Information, strWindowTitle)
            Exit Sub
        End If
        If Not IO.File.Exists(txtFile1.Text) Then
            MsgBox("File #1 doesn't exist.", MsgBoxStyle.Critical, strWindowTitle)
            Exit Sub
        End If
        If Not IO.File.Exists(txtFile2.Text) Then
            MsgBox("File #2 doesn't exist.", MsgBoxStyle.Critical, strWindowTitle)
            Exit Sub
        End If

        SyncLock threadLockingObject
            ulongAllBytes = 0
            ulongAllReadBytes = 0

            ulongAllBytes += New IO.FileInfo(txtFile1.Text).Length
            ulongAllBytes += New IO.FileInfo(txtFile2.Text).Length
        End SyncLock

        btnCompareFilesBrowseFile1.Enabled = False
        btnCompareFilesBrowseFile2.Enabled = False
        txtFile1.Enabled = False
        txtFile2.Enabled = False
        btnCompareFiles.Text = "Abort Processing"
        compareFilesProgressBar.Visible = True

        workingThread = New Threading.Thread(Sub()
                                                 Try
                                                     boolBackgroundThreadWorking = True
                                                     Dim checksumType As checksumType

                                                     myInvoke(Sub()
                                                                  compareRadioMD5.Enabled = False
                                                                  compareRadioSHA1.Enabled = False
                                                                  compareRadioSHA256.Enabled = False
                                                                  compareRadioSHA384.Enabled = False
                                                                  compareRadioSHA512.Enabled = False
                                                                  CompareFilesAllFilesProgress.Visible = True

                                                                  If compareRadioMD5.Checked Then
                                                                      checksumType = checksumType.md5
                                                                  ElseIf compareRadioSHA1.Checked Then
                                                                      checksumType = checksumType.sha160
                                                                  ElseIf compareRadioSHA256.Checked Then
                                                                      checksumType = checksumType.sha256
                                                                  ElseIf compareRadioSHA384.Checked Then
                                                                      checksumType = checksumType.sha384
                                                                  ElseIf compareRadioSHA512.Checked Then
                                                                      checksumType = checksumType.sha512
                                                                  End If
                                                              End Sub)

                                                     Dim strChecksum1 As String = Nothing
                                                     Dim strChecksum2 As String = Nothing
                                                     Dim boolSuccessful As Boolean = False
                                                     Dim percentage, allBytesPercentage As Double
                                                     Dim subRoutine As [Delegate] = Sub(size As Long, totalBytesRead As Long, eta As TimeSpan)
                                                                                        Try
                                                                                            myInvoke(Sub()
                                                                                                         percentage = If(totalBytesRead <> 0 And size <> 0, totalBytesRead / size * 100, 0)
                                                                                                         compareFilesProgressBar.Value = percentage
                                                                                                         SyncLock threadLockingObject
                                                                                                             allBytesPercentage = ulongAllReadBytes / ulongAllBytes * 100
                                                                                                         End SyncLock
                                                                                                         ProgressForm.setTaskbarProgressBarValue(allBytesPercentage)
                                                                                                         CompareFilesAllFilesProgress.Value = allBytesPercentage
                                                                                                         lblCompareFilesStatus.Text = fileSizeToHumanSize(totalBytesRead) & " of " & fileSizeToHumanSize(size) & " (" & Math.Round(percentage, 2) & "%) have been processed."
                                                                                                         If boolShowEstimatedTime AndAlso eta <> TimeSpan.Zero Then lblCompareFilesStatus.Text &= " Estimated " & timespanToHMS(eta) & " remaining."
                                                                                                     End Sub)
                                                                                        Catch ex As Exception
                                                                                        End Try
                                                                                    End Sub

                                                     Dim stopWatch As Stopwatch = Stopwatch.StartNew

                                                     Dim checksum1FinishCode As [Delegate] = Sub()
                                                                                                 myInvoke(Sub()
                                                                                                              lblFile1Hash.Text = "Hash/Checksum: " & If(chkDisplayHashesInUpperCase.Checked, strChecksum1.ToUpper, strChecksum1.ToLower)
                                                                                                              ToolTip.SetToolTip(lblFile1Hash, strChecksum1)
                                                                                                          End Sub)

                                                                                             End Sub
                                                     Dim checksum2FinishCode As [Delegate] = Sub()
                                                                                                 myInvoke(Sub()
                                                                                                              lblFile2Hash.Text = "Hash/Checksum: " & If(chkDisplayHashesInUpperCase.Checked, strChecksum2.ToUpper, strChecksum2.ToLower)
                                                                                                              ToolTip.SetToolTip(lblFile2Hash, strChecksum2)
                                                                                                          End Sub)

                                                                                             End Sub

                                                     If doChecksumWithAttachedSubRoutine(txtFile1.Text, checksumType, strChecksum1, subRoutine, checksum1FinishCode) AndAlso doChecksumWithAttachedSubRoutine(txtFile2.Text, checksumType, strChecksum2, subRoutine, checksum2FinishCode) Then
                                                         boolSuccessful = True
                                                     End If


                                                     myInvoke(Sub()
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
                                                                  compareFilesProgressBar.Value = 0
                                                                  compareFilesProgressBar.Visible = False
                                                                  CompareFilesAllFilesProgress.Value = 0
                                                                  CompareFilesAllFilesProgress.Visible = False
                                                                  ProgressForm.setTaskbarProgressBarValue(0)
                                                                  Me.Text = "Hasher"

                                                                  If boolSuccessful Then
                                                                      If strChecksum1.Equals(strChecksum2, StringComparison.OrdinalIgnoreCase) Then
                                                                          MsgBox("Both files are the same." & vbCrLf & vbCrLf & "Processing completed in " & timespanToHMS(stopWatch.Elapsed) & ".", MsgBoxStyle.Information, strWindowTitle)
                                                                      Else
                                                                          MsgBox("The two files don't match." & vbCrLf & vbCrLf & "Processing completed in " & timespanToHMS(stopWatch.Elapsed) & ".", MsgBoxStyle.Critical, strWindowTitle)
                                                                      End If
                                                                  Else
                                                                      MsgBox("There was an error while calculating the checksum.", MsgBoxStyle.Critical, strWindowTitle)
                                                                  End If

                                                                  boolBackgroundThreadWorking = False
                                                                  workingThread = Nothing
                                                              End Sub)
                                                 Catch ex As Threading.ThreadAbortException
                                                     myInvoke(Sub()
                                                                  If Not boolClosingWindow Then
                                                                      btnCompareFilesBrowseFile1.Enabled = True
                                                                      btnCompareFilesBrowseFile1.Enabled = True
                                                                      txtFile1.Enabled = True
                                                                      txtFile2.Enabled = True
                                                                      compareFilesProgressBar.Value = 0
                                                                      compareFilesProgressBar.Visible = False
                                                                      CompareFilesAllFilesProgress.Value = 0
                                                                      CompareFilesAllFilesProgress.Visible = False
                                                                      ProgressForm.setTaskbarProgressBarValue(0)
                                                                      btnCompareFiles.Text = "Compare Files"
                                                                      compareRadioMD5.Enabled = True
                                                                      compareRadioSHA1.Enabled = True
                                                                      compareRadioSHA256.Enabled = True
                                                                      compareRadioSHA384.Enabled = True
                                                                      compareRadioSHA512.Enabled = True
                                                                      lblCompareFilesStatus.Text = strNoBackgroundProcesses
                                                                      Me.Text = "Hasher"
                                                                  End If

                                                                  boolBackgroundThreadWorking = False
                                                                  workingThread = Nothing
                                                                  If Not boolClosingWindow Then MsgBox("Processing aborted.", MsgBoxStyle.Information, strWindowTitle)
                                                              End Sub)
                                                 Finally
                                                     SyncLock threadLockingObject
                                                         ulongAllReadBytes = 0
                                                         ulongAllBytes = 0
                                                     End SyncLock
                                                 End Try
                                             End Sub) With {
            .Priority = getThreadPriority(),
            .Name = "Hash Generation Thread",
            .IsBackground = True
        }
        workingThread.Start()
    End Sub

    Private Sub btnCompareFilesBrowseFile1_Click(sender As Object, e As EventArgs) Handles btnCompareFilesBrowseFile1.Click
        lblFile1Hash.Text = Nothing
        ToolTip.SetToolTip(lblFile1Hash, "")

        OpenFileDialog.Title = "Select file #1 to be compared..."
        OpenFileDialog.Multiselect = False
        OpenFileDialog.Filter = "Show All Files|*.*"

        If OpenFileDialog.ShowDialog() = DialogResult.OK Then txtFile1.Text = OpenFileDialog.FileName
    End Sub

    Private Sub btnCompareFilesBrowseFile2_Click(sender As Object, e As EventArgs) Handles btnCompareFilesBrowseFile2.Click
        lblFile2Hash.Text = Nothing
        ToolTip.SetToolTip(lblFile2Hash, "")

        OpenFileDialog.Title = "Select file #2 to be compared..."
        OpenFileDialog.Multiselect = False
        OpenFileDialog.Filter = "Show All Files|*.*"

        If OpenFileDialog.ShowDialog() = DialogResult.OK Then txtFile2.Text = OpenFileDialog.FileName
    End Sub

    Private Sub chkSSL_Click(sender As Object, e As EventArgs) Handles chkSSL.Click
        My.Settings.boolSSL = chkSSL.Checked
    End Sub

    Private Sub btnBrowseFileForCompareKnownHash_Click(sender As Object, e As EventArgs) Handles btnBrowseFileForCompareKnownHash.Click
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

    Private Sub txtKnownHash_TextChanged(sender As Object, e As EventArgs) Handles txtKnownHash.TextChanged
        pictureBoxVerifyAgainstResults.Image = Nothing
        ToolTip.SetToolTip(pictureBoxVerifyAgainstResults, "")

        If String.IsNullOrWhiteSpace(txtKnownHash.Text) Then
            lblCompareFileAgainstKnownHashType.Text = ""
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
                lblCompareFileAgainstKnownHashType.Text = ""
                btnCompareAgainstKnownHash.Enabled = False
            End If
        End If
    End Sub

    Private Sub txtKnownHash_KeyUp(sender As Object, e As KeyEventArgs) Handles txtKnownHash.KeyUp
        If e.KeyCode = Keys.Enter And (txtKnownHash.Text.Length = 128 Or txtKnownHash.Text.Length = 96 Or txtKnownHash.Text.Length = 64 Or txtKnownHash.Text.Length = 40 Or txtKnownHash.Text.Length = 32) Then
            btnCompareAgainstKnownHash.PerformClick()
        End If
    End Sub

    Private Sub btnCompareAgainstKnownHash_Click(sender As Object, e As EventArgs) Handles btnCompareAgainstKnownHash.Click
        If btnCompareAgainstKnownHash.Text = "Abort Processing" Then
            If workingThread IsNot Nothing Then
                workingThread.Abort()
                boolBackgroundThreadWorking = False
            End If

            Exit Sub
        End If

        txtFileForKnownHash.Text = txtFileForKnownHash.Text.Trim

        If Not IO.File.Exists(txtFileForKnownHash.Text) Then
            MsgBox("File doesn't exist.", MsgBoxStyle.Critical, strWindowTitle)
            Exit Sub
        End If

        txtFileForKnownHash.Enabled = False
        btnBrowseFileForCompareKnownHash.Enabled = False
        txtKnownHash.Enabled = False
        btnCompareAgainstKnownHash.Text = "Abort Processing"
        boolDidWePerformAPreviousHash = True

        workingThread = New Threading.Thread(Sub()
                                                 Try
                                                     boolBackgroundThreadWorking = True
                                                     Dim checksumType As checksumType

                                                     If txtKnownHash.Text.Length = 32 Then
                                                         checksumType = checksumType.md5
                                                     ElseIf txtKnownHash.Text.Length = 40 Then
                                                         checksumType = checksumType.sha160
                                                     ElseIf txtKnownHash.Text.Length = 64 Then
                                                         checksumType = checksumType.sha256
                                                     ElseIf txtKnownHash.Text.Length = 96 Then
                                                         checksumType = checksumType.sha384
                                                     ElseIf txtKnownHash.Text.Length = 128 Then
                                                         checksumType = checksumType.sha512
                                                     End If

                                                     Dim strChecksum As String = Nothing
                                                     Dim percentage As Double
                                                     Dim subRoutine As [Delegate] = Sub(size As Long, totalBytesRead As Long, eta As TimeSpan)
                                                                                        Try
                                                                                            myInvoke(Sub()
                                                                                                         percentage = If(totalBytesRead <> 0 And size <> 0, totalBytesRead / size * 100, 0)
                                                                                                         compareAgainstKnownHashProgressBar.Value = percentage
                                                                                                         ProgressForm.setTaskbarProgressBarValue(compareAgainstKnownHashProgressBar.Value)
                                                                                                         lblCompareAgainstKnownHashStatus.Text = fileSizeToHumanSize(totalBytesRead) & " of " & fileSizeToHumanSize(size) & " (" & Math.Round(percentage, 2) & "%) have been processed."
                                                                                                         If boolShowEstimatedTime AndAlso eta <> TimeSpan.Zero Then lblCompareAgainstKnownHashStatus.Text &= " Estimated " & timespanToHMS(eta) & " remaining."
                                                                                                     End Sub)
                                                                                        Catch ex As Exception
                                                                                        End Try
                                                                                    End Sub

                                                     Dim stopWatch As Stopwatch = Stopwatch.StartNew
                                                     Dim boolSuccessful As Boolean = doChecksumWithAttachedSubRoutine(txtFileForKnownHash.Text, checksumType, strChecksum, subRoutine)

                                                     myInvoke(Sub()
                                                                  txtFileForKnownHash.Enabled = True
                                                                  btnBrowseFileForCompareKnownHash.Enabled = True
                                                                  txtKnownHash.Enabled = True
                                                                  btnCompareAgainstKnownHash.Text = "Compare File Against Known Hash"
                                                                  btnCompareAgainstKnownHash.Enabled = False
                                                                  lblCompareAgainstKnownHashStatus.Text = strNoBackgroundProcesses
                                                                  compareAgainstKnownHashProgressBar.Value = 0
                                                                  ProgressForm.setTaskbarProgressBarValue(0)
                                                                  Me.Text = "Hasher"

                                                                  If boolSuccessful Then
                                                                      If strChecksum.Equals(txtKnownHash.Text.Trim, StringComparison.OrdinalIgnoreCase) Then
                                                                          pictureBoxVerifyAgainstResults.Image = Global.Hasher.My.Resources.Resources.good_check
                                                                          ToolTip.SetToolTip(pictureBoxVerifyAgainstResults, "Checksum Verified!")
                                                                          MsgBox("The checksums match!" & vbCrLf & vbCrLf & "Processing completed in " & timespanToHMS(stopWatch.Elapsed) & ".", MsgBoxStyle.Information, strWindowTitle)
                                                                      Else
                                                                          pictureBoxVerifyAgainstResults.Image = Global.Hasher.My.Resources.Resources.bad_check
                                                                          ToolTip.SetToolTip(pictureBoxVerifyAgainstResults, "Checksum verification failed, checksum didn't match!")
                                                                          MsgBox("The checksums DON'T match!" & vbCrLf & vbCrLf & "Processing completed in " & timespanToHMS(stopWatch.Elapsed) & ".", MsgBoxStyle.Critical, strWindowTitle)
                                                                      End If
                                                                  Else
                                                                      pictureBoxVerifyAgainstResults.Image = Global.Hasher.My.Resources.Resources.bad_check
                                                                      MsgBox("There was an error while calculating the checksum.", MsgBoxStyle.Critical, strWindowTitle)
                                                                  End If

                                                                  boolBackgroundThreadWorking = False
                                                                  workingThread = Nothing
                                                              End Sub)
                                                 Catch ex As Threading.ThreadAbortException
                                                     myInvoke(Sub()
                                                                  If Not boolClosingWindow Then
                                                                      txtFileForKnownHash.Enabled = True
                                                                      btnBrowseFileForCompareKnownHash.Enabled = True
                                                                      txtKnownHash.Enabled = True
                                                                      btnCompareAgainstKnownHash.Text = "Compare File Against Known Hash"
                                                                      compareAgainstKnownHashProgressBar.Value = 0
                                                                      ProgressForm.setTaskbarProgressBarValue(0)
                                                                      lblCompareFilesStatus.Text = strNoBackgroundProcesses
                                                                      Me.Text = "Hasher"
                                                                  End If

                                                                  boolBackgroundThreadWorking = False
                                                                  workingThread = Nothing
                                                                  If Not boolClosingWindow Then MsgBox("Processing aborted.", MsgBoxStyle.Information, strWindowTitle)
                                                              End Sub)
                                                 End Try
                                             End Sub) With {
            .Priority = getThreadPriority(),
            .Name = "Hash Generation Thread",
            .IsBackground = True
        }
        workingThread.Start()
    End Sub

    Private Sub Form1_ResizeEnd(sender As Object, e As EventArgs) Handles Me.ResizeEnd
        My.Settings.windowSize = Me.Size
    End Sub

    Private Function getHashOfString(inputString As String, hashType As checksumType) As String
        Using HashAlgorithm As Security.Cryptography.HashAlgorithm = checksums.getHashEngine(hashType)
            Dim Output As Byte() = HashAlgorithm.ComputeHash(System.Text.Encoding.UTF8.GetBytes(inputString))
            Return BitConverter.ToString(Output).ToLower().Replace("-", "")
        End Using
    End Function

    Private Sub listFiles_ColumnWidthChanged(sender As Object, e As ColumnWidthChangedEventArgs) Handles listFiles.ColumnWidthChanged
        If Not boolDoneLoading Then Exit Sub
        My.Settings.hashIndividualFilesFileNameColumnSize = colFileName.Width
        My.Settings.hashIndividualFilesFileSizeColumnSize = colFileSize.Width
        My.Settings.hashIndividualFilesChecksumColumnSize = colChecksum.Width
        My.Settings.hashIndividualFilesComputeTimeColumnSize = colComputeTime.Width
    End Sub

    Private Sub verifyHashesListFiles_ColumnWidthChanged(sender As Object, e As ColumnWidthChangedEventArgs) Handles verifyHashesListFiles.ColumnWidthChanged
        If Not boolDoneLoading Then Exit Sub
        My.Settings.verifyHashFileNameColumnSize = colFile.Width
        My.Settings.verifyHashFileSizeColumnSize = colFileSize2.Width
        My.Settings.verifyHashFileResults = colResults.Width
        My.Settings.verifyHashComputeTimeColumnSize = colComputeTime2.Width
    End Sub

    Private Sub txtFile1_DragEnter(sender As Object, e As DragEventArgs) Handles txtFile1.DragEnter
        e.Effect = If(e.Data.GetDataPresent(DataFormats.FileDrop), DragDropEffects.All, DragDropEffects.None)
    End Sub

    Private Sub txtFile1_DragDrop(sender As Object, e As DragEventArgs) Handles txtFile1.DragDrop
        Dim receivedData As String() = DirectCast(e.Data.GetData(DataFormats.FileDrop), String())
        If receivedData.Count = 1 Then
            txtFile1.Text = receivedData(0)
            lblFile1Hash.Text = Nothing
        End If
    End Sub

    Private Sub txtFile2_DragEnter(sender As Object, e As DragEventArgs) Handles txtFile2.DragEnter
        e.Effect = If(e.Data.GetDataPresent(DataFormats.FileDrop), DragDropEffects.All, DragDropEffects.None)
    End Sub

    Private Sub txtFile2_DragDrop(sender As Object, e As DragEventArgs) Handles txtFile2.DragDrop
        Dim receivedData As String() = DirectCast(e.Data.GetData(DataFormats.FileDrop), String())
        If receivedData.Count = 1 Then
            txtFile2.Text = receivedData(0)
            lblFile2Hash.Text = Nothing
        End If
    End Sub

    Private Sub txtFileForKnownHash_DragEnter(sender As Object, e As DragEventArgs) Handles txtFileForKnownHash.DragEnter
        e.Effect = If(e.Data.GetDataPresent(DataFormats.FileDrop), DragDropEffects.All, DragDropEffects.None)
    End Sub

    Private Sub txtFileForKnownHash_DragDrop(sender As Object, e As DragEventArgs) Handles txtFileForKnownHash.DragDrop
        Dim receivedData As String() = DirectCast(e.Data.GetData(DataFormats.FileDrop), String())
        If receivedData.Count = 1 Then txtFileForKnownHash.Text = receivedData(0)
    End Sub

    Private Sub btnOpenExistingHashFile_DragEnter(sender As Object, e As DragEventArgs) Handles btnOpenExistingHashFile.DragEnter
        e.Effect = If(e.Data.GetDataPresent(DataFormats.FileDrop), DragDropEffects.All, DragDropEffects.None)
        Dim receivedData As String() = DirectCast(e.Data.GetData(DataFormats.FileDrop), String())

        If receivedData.Count = 1 Then
            Dim fileInfo As New IO.FileInfo(receivedData(0))

            If Not fileInfo.Extension.Equals(".md5", StringComparison.OrdinalIgnoreCase) And Not fileInfo.Extension.Equals(".sha1", StringComparison.OrdinalIgnoreCase) And Not fileInfo.Extension.Equals(".sha256", StringComparison.OrdinalIgnoreCase) And Not fileInfo.Extension.Equals(".sha384", StringComparison.OrdinalIgnoreCase) And Not fileInfo.Extension.Equals(".sha512", StringComparison.OrdinalIgnoreCase) Then
                e.Effect = DragDropEffects.None
            End If
        End If
    End Sub

    Private Sub btnAssociate_Click(sender As Object, e As EventArgs) Handles btnAssociate.Click
        If areWeAnAdministrator() Then
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

        MsgBox("File association complete.", MsgBoxStyle.Information, strWindowTitle)
    End Sub

    Private Sub btnAddHasherToAllFiles_Click(sender As Object, e As EventArgs) Handles btnAddHasherToAllFiles.Click
        If areWeAnAdministrator() Then
            FileAssociation.addAssociationWithAllFiles()
        Else
            Dim startInfo As New ProcessStartInfo With {
                .FileName = Application.ExecutablePath,
                .Arguments = "-associateallfiles",
                .Verb = "runas"
            }
            Dim process As Process = Process.Start(startInfo)
            process.WaitForExit()
        End If

        MsgBox("File association complete.", MsgBoxStyle.Information, strWindowTitle)
    End Sub

    Private Sub btnOpenExistingHashFile_DragDrop(sender As Object, e As DragEventArgs) Handles btnOpenExistingHashFile.DragDrop
        Dim receivedData As String() = DirectCast(e.Data.GetData(DataFormats.FileDrop), String())
        If receivedData.Count = 1 Then
            Dim strReceivedFileName As String = receivedData(0)
            Dim fileInfo As New IO.FileInfo(strReceivedFileName)

            If fileInfo.Extension.Equals(".md5", StringComparison.OrdinalIgnoreCase) Or fileInfo.Extension.Equals(".sha1", StringComparison.OrdinalIgnoreCase) Or fileInfo.Extension.Equals(".sha256", StringComparison.OrdinalIgnoreCase) Or fileInfo.Extension.Equals(".sha384", StringComparison.OrdinalIgnoreCase) Or fileInfo.Extension.Equals(".sha512", StringComparison.OrdinalIgnoreCase) Then
                btnOpenExistingHashFile.Text = "Abort Processing"
                verifyHashesListFiles.Items.Clear()
                processExistingHashFile(strReceivedFileName)
            Else
                MsgBox("Invalid file type.", MsgBoxStyle.Critical, strWindowTitle)
            End If
        End If
    End Sub

    Private Sub listFiles_ItemSelectionChanged(sender As Object, e As ListViewItemSelectionChangedEventArgs) Handles listFiles.ItemSelectionChanged
        updateFilesListCountHeader(True)
    End Sub

    Private Sub listFiles_KeyUp(sender As Object, e As KeyEventArgs) Handles listFiles.KeyUp
        If e.KeyCode = Keys.Delete Then btnRemoveSelectedFiles.PerformClick()
    End Sub

    Private Sub chkSortByFileSizeAfterLoadingHashFile_Click(sender As Object, e As EventArgs) Handles chkSortByFileSizeAfterLoadingHashFile.Click
        My.Settings.boolSortByFileSizeAfterLoadingHashFile = chkSortByFileSizeAfterLoadingHashFile.Checked
    End Sub

    Private Sub chkSaveChecksumFilesWithRelativePaths_Click(sender As Object, e As EventArgs) Handles chkSaveChecksumFilesWithRelativePaths.Click
        My.Settings.boolSaveChecksumFilesWithRelativePaths = chkSaveChecksumFilesWithRelativePaths.Checked
    End Sub

    Private Sub chkSortFileListingAfterAddingFilesToHash_Click(sender As Object, e As EventArgs) Handles chkSortFileListingAfterAddingFilesToHash.Click
        My.Settings.boolSortFileListingAfterAddingFilesToHash = chkSortFileListingAfterAddingFilesToHash.Checked
    End Sub

    Private Sub WaitForConnectionCallBack(ByVal iar As IAsyncResult)
        Try
            Dim namedPipeServer As NamedPipeServerStream = CType(iar.AsyncState, NamedPipeServerStream)
            namedPipeServer.EndWaitForConnection(iar)
            Dim buffer As Byte() = New Byte(499) {}
            namedPipeServer.Read(buffer, 0, 500)

            Dim strReceivedFileName As String = System.Text.Encoding.UTF8.GetString(buffer, 0, buffer.Length).Replace(vbNullChar, "").Trim
            addFileOrDirectoryToHashFileList(strReceivedFileName)

            namedPipeServer.Dispose()
            namedPipeServer = New NamedPipeServerStream(strNamedPipeServerName, PipeDirection.In, 1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous)
            namedPipeServer.BeginWaitForConnection(New AsyncCallback(AddressOf WaitForConnectionCallBack), namedPipeServer)
        Catch
            Return
        End Try
    End Sub

    Private Sub chkUseMilliseconds_Click(sender As Object, e As EventArgs) Handles chkUseMilliseconds.Click
        My.Settings.boolUseMilliseconds = chkUseMilliseconds.Checked
    End Sub

    Private Sub chkDisplayHashesInUpperCase_Click(sender As Object, e As EventArgs) Handles chkDisplayHashesInUpperCase.Click
        My.Settings.boolDisplayHashesInUpperCase = chkDisplayHashesInUpperCase.Checked

        If listFiles.Items.Count <> 0 Then
            For Each item As myListViewItem In listFiles.Items
                item.SubItems(2).Text = If(My.Settings.boolDisplayHashesInUpperCase, item.hash.ToUpper, item.hash.ToLower)
            Next
        End If

        If Not String.IsNullOrWhiteSpace(txtHashResults.Text) Then
            txtHashResults.Text = If(My.Settings.boolDisplayHashesInUpperCase, txtHashResults.Text.ToUpper, txtHashResults.Text.ToLower)
        End If
        If Not String.IsNullOrWhiteSpace(lblFile1Hash.Text) Then
            lblFile1Hash.Text = If(My.Settings.boolDisplayHashesInUpperCase, lblFile1Hash.Text.ToUpper, lblFile1Hash.Text.ToLower)
        End If
        If Not String.IsNullOrWhiteSpace(lblFile2Hash.Text) Then
            lblFile2Hash.Text = If(My.Settings.boolDisplayHashesInUpperCase, lblFile2Hash.Text.ToUpper, lblFile2Hash.Text.ToLower)
        End If
    End Sub

    Private Sub BtnSetValidColor_Click(sender As Object, e As EventArgs) Handles btnSetValidColor.Click
        Using colorDialog As New ColorDialog() With {.Color = My.Settings.validColor}
            If colorDialog.ShowDialog() = DialogResult.OK Then
                My.Settings.validColor = colorDialog.Color
                lblValidColor.BackColor = colorDialog.Color
                validColor = colorDialog.Color
                MsgBox("Color preferences will not be used until the next time a checksum file is processed in the ""Verify Saved Hashes"" tab.", MsgBoxStyle.Information, Me.Text)
            End If
        End Using
    End Sub

    Private Sub BtnSetNotValidColor_Click(sender As Object, e As EventArgs) Handles btnSetNotValidColor.Click
        Using colorDialog As New ColorDialog() With {.Color = My.Settings.notValidColor}
            If colorDialog.ShowDialog() = DialogResult.OK Then
                My.Settings.notValidColor = colorDialog.Color
                lblNotValidColor.BackColor = colorDialog.Color
                notValidColor = colorDialog.Color
                MsgBox("Color preferences will not be used until the next time a checksum file is processed in the ""Verify Saved Hashes"" tab.", MsgBoxStyle.Information, Me.Text)
            End If
        End Using
    End Sub

    Private Sub BtnFileNotFoundColor_Click(sender As Object, e As EventArgs) Handles btnFileNotFoundColor.Click
        Using colorDialog As New ColorDialog() With {.Color = My.Settings.fileNotFoundColor}
            If colorDialog.ShowDialog() = DialogResult.OK Then
                My.Settings.fileNotFoundColor = colorDialog.Color
                lblFileNotFoundColor.BackColor = colorDialog.Color
                fileNotFoundColor = colorDialog.Color
                MsgBox("Color preferences will not be used until the next time a checksum file is processed in the ""Verify Saved Hashes"" tab.", MsgBoxStyle.Information, Me.Text)
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

        MsgBox("Color preferences will not be used until the next time a checksum file is processed in the ""Verify Saved Hashes"" tab.", MsgBoxStyle.Information, Me.Text)
    End Sub

    Private Sub chkShowEstimatedTimeRemaining_Click(sender As Object, e As EventArgs) Handles chkShowEstimatedTimeRemaining.Click
        boolShowEstimatedTime = chkShowEstimatedTimeRemaining.Checked
        My.Settings.boolShowEstimatedTime = chkShowEstimatedTimeRemaining.Checked
    End Sub

    Private Sub btnSetBufferSize_Click(sender As Object, e As EventArgs) Handles btnSetBufferSize.Click
        Dim shortBufferSize As Short = Decimal.ToInt16(bufferSize.Value)
        intBufferSize = shortBufferSize * 1024 * 1024
        My.Settings.shortBufferSize = shortBufferSize
        btnSetBufferSize.Enabled = False
        MsgBox("Data buffer size set successfully to " & shortBufferSize & If(shortBufferSize = 1, " MB.", " MBs."), MsgBoxStyle.Information, strWindowTitle)
    End Sub

    Private Sub BufferSize_ValueChanged(sender As Object, e As EventArgs) Handles bufferSize.ValueChanged
        btnSetBufferSize.Enabled = True
    End Sub

    Private Sub BtnPerformBenchmark_Click(sender As Object, e As EventArgs) Handles btnPerformBenchmark.Click
        Using benchmark As New Benchmark With {.StartPosition = FormStartPosition.CenterScreen}
            benchmark.ShowDialog(Me)

            If benchmark.boolSetBufferSize Then
                bufferSize.Value = Convert.ToDecimal(benchmark.shortBufferSize)
                intBufferSize = benchmark.shortBufferSize * 1024 * 1024
                My.Settings.shortBufferSize = benchmark.shortBufferSize
                btnSetBufferSize.Enabled = False
                MsgBox("Data buffer size set successfully to " & benchmark.shortBufferSize & If(benchmark.shortBufferSize = 1, " MB.", " MBs."), MsgBoxStyle.Information, strWindowTitle)
            End If
        End Using
    End Sub

    Private Sub chkUseCommasInNumbers_Click(sender As Object, e As EventArgs) Handles chkUseCommasInNumbers.Click
        My.Settings.boolUseCommasInNumbers = chkUseCommasInNumbers.Checked
    End Sub

    Private Function getThreadPriority() As Threading.ThreadPriority
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

    Private Sub taskPriority_SelectedIndexChanged(sender As Object, e As EventArgs) Handles taskPriority.SelectedIndexChanged
        If boolDoneLoading Then My.Settings.taskPriority = taskPriority.SelectedIndex
    End Sub

    Private Sub chkCheckForUpdates_Click(sender As Object, e As EventArgs) Handles chkCheckForUpdates.Click
        My.Settings.boolCheckForUpdates = chkCheckForUpdates.Checked
    End Sub

    Private Sub chkAutoAddExtension_Click(sender As Object, e As EventArgs) Handles chkAutoAddExtension.Click
        If Not chkAutoAddExtension.Checked AndAlso MsgBox("You are disabling a highly recommended option, it is HIGHLY recommended that you re-enable this option to prevent accidental data loss." & vbCrLf & vbCrLf & "Are you sure you want to do this?", MsgBoxStyle.Critical + MsgBoxStyle.YesNo + MsgBoxStyle.DefaultButton2, strMessageBoxTitleText) = MsgBoxResult.No Then
            chkAutoAddExtension.Checked = True
        End If
        My.Settings.boolAutoAddExtension = chkAutoAddExtension.Checked
    End Sub
End Class