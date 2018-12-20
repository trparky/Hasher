Public Class Form1
    Private Const strToBeComputed As String = "To Be Computed"
    Private Const strNoBackgroundProcesses As String = "(No Background Processes)"
    Private Const intBufferSize As Integer = 16 * 1024 * 1024
    Private Const strWindowTitle As String = "Hasher"
    Private Const shortUDPServerPort As Short = 32589

    Private filesInListFiles As New Specialized.StringCollection
    Private hashResultArray As New Dictionary(Of String, String)
    Private ReadOnly hashLineParser As New Text.RegularExpressions.Regex("([a-zA-Z0-9]*) \*(.*)", System.Text.RegularExpressions.RegexOptions.Compiled)
    Private ReadOnly hashLineFilePathChecker As New Text.RegularExpressions.Regex("\A[A-Za-z]{1}:.*\Z", System.Text.RegularExpressions.RegexOptions.Compiled)
    Private boolBackgroundThreadWorking As Boolean = False
    Private workingThread As Threading.Thread
    Private boolClosingWindow As Boolean = False
    Private m_SortingColumn1, m_SortingColumn2 As ColumnHeader
    Private boolDoneLoading As Boolean = False
    Private udpClient As Net.Sockets.UdpClient = Nothing
    Private ReadOnly communicationChannelClassSerializer As New Xml.Serialization.XmlSerializer((New communicationChannelClass).GetType)

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

    Function doChecksumWithAttachedSubRoutine(strFile As String, checksumType As checksumType, ByRef strChecksum As String, subRoutine As [Delegate]) As Boolean
        Try
            If IO.File.Exists(strFile) Then
                Dim checksums As New checksums With {.setChecksumStatusUpdateRoutine = subRoutine}
                strChecksum = checksums.performFileHash(strFile, intBufferSize, checksumType)
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
            lblHashIndividualFilesStep1.Text = String.Format("Step 1: Select Individual Files to be Hashed: {0} Files ({1} {2} are selected)", listFiles.Items.Count().ToString("N0"), listFiles.SelectedItems.Count().ToString("N0"), If(listFiles.SelectedItems.Count() = 1, "item", "items"))
        Else
            lblHashIndividualFilesStep1.Text = String.Format("Step 1: Select Individual Files to be Hashed: {0} Files", listFiles.Items.Count().ToString("N0"))
        End If

        btnComputeHash.Enabled = If(listFiles.Items.Count() = 0, False, True)
    End Sub

    Private Sub btnRemoveAllFiles_Click(sender As Object, e As EventArgs) Handles btnRemoveAllFiles.Click
        listFiles.Items.Clear()
        filesInListFiles.Clear()
        updateFilesListCountHeader()
    End Sub

    Private Sub btnRemoveSelectedFiles_Click(sender As Object, e As EventArgs) Handles btnRemoveSelectedFiles.Click
        If listFiles.SelectedItems.Count > 500 AndAlso MsgBox(String.Format("It would be recommended to use the ""Remove All Files"" button instead, removing this many items ({0} items) from the list is a slow process and will make the program appear locked up." & vbCrLf & vbCrLf & "Are you sure you want to remove the items this way?", listFiles.SelectedItems.Count.ToString("N0")), MsgBoxStyle.Question + MsgBoxStyle.YesNo, Me.Text) = MsgBoxResult.No Then
            Exit Sub
        End If

        listFiles.BeginUpdate()
        For Each item As myListViewItem In listFiles.SelectedItems
            filesInListFiles.Remove(item.Text)
            listFiles.Items.Remove(item)
        Next
        listFiles.EndUpdate()

        updateFilesListCountHeader()
    End Sub

    Private Sub btnAddIndividualFiles_Click(sender As Object, e As EventArgs) Handles btnAddIndividualFiles.Click
        Dim itemToBeAdded As myListViewItem

        OpenFileDialog.Title = "Select Files to be Hashed..."
        OpenFileDialog.Multiselect = True
        OpenFileDialog.Filter = "Show All Files|*.*"

        If OpenFileDialog.ShowDialog() = DialogResult.OK Then
            If OpenFileDialog.FileNames.Count() = 0 Then
                MsgBox("You must select some files.", MsgBoxStyle.Critical, strWindowTitle)
            ElseIf OpenFileDialog.FileNames.Count() = 1 Then
                If Not filesInListFiles.Contains(OpenFileDialog.FileName) Then
                    filesInListFiles.Add(OpenFileDialog.FileName)
                    itemToBeAdded = New myListViewItem(OpenFileDialog.FileName) With {.fileSize = New IO.FileInfo(OpenFileDialog.FileName).Length}
                    itemToBeAdded.SubItems.Add(fileSizeToHumanSize(itemToBeAdded.fileSize))
                    itemToBeAdded.SubItems.Add(strToBeComputed)
                    itemToBeAdded.fileName = OpenFileDialog.FileName
                    listFiles.Items.Add(itemToBeAdded)
                    itemToBeAdded = Nothing
                End If
            Else
                listFiles.BeginUpdate()
                For Each strFileName As String In OpenFileDialog.FileNames
                    If Not filesInListFiles.Contains(strFileName) Then
                        filesInListFiles.Add(strFileName)
                        itemToBeAdded = New myListViewItem(strFileName) With {.fileSize = New IO.FileInfo(strFileName).Length}
                        itemToBeAdded.SubItems.Add(fileSizeToHumanSize(itemToBeAdded.fileSize))
                        itemToBeAdded.SubItems.Add(strToBeComputed)
                        itemToBeAdded.fileName = strFileName
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

        workingThread = New Threading.Thread(Sub()
                                                 Try
                                                     boolBackgroundThreadWorking = True
                                                     Dim strFileName As String
                                                     Dim strChecksum As String = Nothing
                                                     Dim checksumType As checksumType
                                                     Dim index As Short = 1
                                                     Dim oldLocationInFile As ULong = 0
                                                     Dim subRoutine As [Delegate] = Sub(size As Long, totalBytesRead As Long)
                                                                                        Try
                                                                                            Me.Invoke(Sub()
                                                                                                          oldLocationInFile = totalBytesRead

                                                                                                          If totalBytesRead <> 0 And size <> 0 Then
                                                                                                              IndividualFilesProgressBar.Value = totalBytesRead / size * 100
                                                                                                          Else
                                                                                                              IndividualFilesProgressBar.Value = 0
                                                                                                          End If

                                                                                                          lblIndividualFilesStatus.Text = String.Format("{0} of {1} have been processed.", fileSizeToHumanSize(totalBytesRead), fileSizeToHumanSize(size))
                                                                                                          lblIndividualFilesStatusProcessingFile.Text = String.Format("Processing {0} of {1} file(s).", index.ToString("N0"), listFiles.Items.Count().ToString("N0"))
                                                                                                          oldLocationInFile = totalBytesRead
                                                                                                      End Sub)
                                                                                        Catch ex As Exception
                                                                                        End Try
                                                                                    End Sub

                                                     hashResultArray.Clear()

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

                                                     For Each item As myListViewItem In listFiles.Items
                                                         strFileName = item.SubItems(0).Text

                                                         If Not hashResultArray.ContainsKey(strFileName) Then
                                                             If doChecksumWithAttachedSubRoutine(strFileName, checksumType, strChecksum, subRoutine) Then
                                                                 item.SubItems(2).Text = strChecksum
                                                                 item.hash = strChecksum
                                                                 hashResultArray.Add(strFileName, strChecksum)
                                                             Else
                                                                 item.SubItems(2).Text = "(Error while calculating checksum)"
                                                             End If
                                                         End If

                                                         index += 1
                                                     Next

                                                     btnIndividualFilesCopyToClipboard.Enabled = True
                                                     btnIndividualFilesSaveResultsToDisk.Enabled = True
                                                     radioMD5.Enabled = True
                                                     radioSHA1.Enabled = True
                                                     radioSHA256.Enabled = True
                                                     radioSHA384.Enabled = True
                                                     radioSHA512.Enabled = True

                                                     Me.Invoke(Sub() MsgBox("Complete.", MsgBoxStyle.Information + MsgBoxStyle.ApplicationModal, strWindowTitle))
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
                                                     If Not boolClosingWindow Then Me.Invoke(Sub() MsgBox("Processing aborted.", MsgBoxStyle.Information + MsgBoxStyle.ApplicationModal, strWindowTitle))
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

        lblIndividualFilesStatus.Text = strNoBackgroundProcesses
        lblIndividualFilesStatusProcessingFile.Text = ""
        lblProcessingFile.Text = ""
        IndividualFilesProgressBar.Value = 0
    End Sub

    Private Function strGetIndividualHashesInStringFormat() As String
        Dim stringBuilder As New Text.StringBuilder()
        addHashFileHeader(stringBuilder)

        For Each item As KeyValuePair(Of String, String) In hashResultArray
            stringBuilder.AppendLine(item.Value.ToString() & " *" & item.Key)
        Next
        Return stringBuilder.ToString()
    End Function

    Private Sub btnIndividualFilesCopyToClipboard_Click(sender As Object, e As EventArgs) Handles btnIndividualFilesCopyToClipboard.Click
        If copyTextToWindowsClipboard(strGetIndividualHashesInStringFormat().Trim) Then MsgBox("Your hash results have been copied to the Windows Clipboard.", MsgBoxStyle.Information, strWindowTitle)
    End Sub

    Private Function copyTextToWindowsClipboard(strTextToBeCopiedToClipboard As String) As Boolean
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

        SaveFileDialog.Title = "Save Hash Results to Disk"

        If SaveFileDialog.ShowDialog() = DialogResult.OK Then
            Using streamWriter As New IO.StreamWriter(SaveFileDialog.FileName, False, System.Text.Encoding.UTF8)
                streamWriter.Write(strGetIndividualHashesInStringFormat())
            End Using
            MsgBox("Your hash results have been written to disk.", MsgBoxStyle.Information, strWindowTitle)
        End If
    End Sub

    Private Sub disableIndividualFilesResultsButtonsAndClearResults()
        btnIndividualFilesCopyToClipboard.Enabled = False
        btnIndividualFilesSaveResultsToDisk.Enabled = False
        hashResultArray.Clear()

        listFiles.BeginUpdate()
        For Each item As myListViewItem In listFiles.Items
            item.SubItems(2).Text = strToBeComputed
            item.hash = Nothing
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

    Private Sub textRadioRIPEMD160_Click(sender As Object, e As EventArgs)
        disableIndividualFilesResultsButtonsAndClearResults()
    End Sub

    Private Sub sendToServer(strFileName As String)
        Using memStream As New IO.MemoryStream
            communicationChannelClassSerializer.Serialize(memStream, New communicationChannelClass With {.strFileName = strFileName})

            Dim udpClient As New Net.Sockets.UdpClient()
            udpClient.Connect("localhost", shortUDPServerPort)
            Dim senddata As Byte() = memStream.ToArray()
            udpClient.Send(senddata, senddata.Length)
        End Using
    End Sub

    Private Sub udpServer()
        Try
            udpClient = New Net.Sockets.UdpClient(shortUDPServerPort)
            My.Settings.Save()

            While True
                Dim remoteIPEndPoint As New Net.IPEndPoint(Net.IPAddress.Any, 0)
                Dim byteArray As Byte() = udpClient.Receive(remoteIPEndPoint)

                TabControl1.Invoke(Sub() TabControl1.SelectTab(2))
                Me.Invoke(Sub() NativeMethod.NativeMethods.SetForegroundWindow(Handle.ToInt32()))

                Using memStream As New IO.MemoryStream(byteArray)
                    processIncomingDataFromServer(memStream)
                End Using
            End While
        Catch ex As Net.Sockets.SocketException
            If My.Application.CommandLineArgs.Count = 1 AndAlso My.Application.CommandLineArgs(0).Trim.StartsWith("--addfile=", StringComparison.OrdinalIgnoreCase) Then
                ' Since there is already a server, let's kill this instance of the program.
                Application.Exit()
            End If
        Catch ex As Exception
            Debug.WriteLine(ex.GetType.ToString & " -- " & ex.Message & ex.StackTrace)
        End Try
    End Sub

    Private Sub processIncomingDataFromServer(ByRef memStream As IO.MemoryStream)
        Try
            Dim receivedClassObject As communicationChannelClass = communicationChannelClassSerializer.Deserialize(memStream)
            Dim isDirectory As Boolean = (IO.File.GetAttributes(receivedClassObject.strFileName) And IO.FileAttributes.Directory) = IO.FileAttributes.Directory

            If isDirectory Then
                addFilesFromDirectory(receivedClassObject.strFileName)
            Else
                If IO.File.Exists(receivedClassObject.strFileName) AndAlso Not filesInListFiles.Contains(receivedClassObject.strFileName) Then
                    filesInListFiles.Add(receivedClassObject.strFileName)
                    Dim itemToBeAdded As myListViewItem
                    itemToBeAdded = New myListViewItem(receivedClassObject.strFileName) With {.fileSize = New IO.FileInfo(receivedClassObject.strFileName).Length}
                    itemToBeAdded.SubItems.Add(fileSizeToHumanSize(itemToBeAdded.fileSize))
                    itemToBeAdded.SubItems.Add(strToBeComputed)
                    itemToBeAdded.fileName = receivedClassObject.strFileName
                    listFiles.Items.Add(itemToBeAdded)
                    itemToBeAdded = Nothing
                    updateFilesListCountHeader()
                End If
            End If
        Catch ex As Exception
        End Try
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        If My.Settings.boolEnableServer Then Threading.ThreadPool.QueueUserWorkItem(AddressOf udpServer)
        Me.Icon = Icon.ExtractAssociatedIcon(Reflection.Assembly.GetExecutingAssembly().Location)

        If areWeAnAdministrator() Then
            Me.Text &= " (WARNING!!! Running as Administrator.)"
        Else
            btnAssociate.FlatStyle = FlatStyle.System
            btnAddHasherToAllFiles.FlatStyle = FlatStyle.System
            NativeMethod.NativeMethods.SendMessage(btnAssociate.Handle, NativeMethod.NativeMethods.BCM_SETSHIELD, 0, &HFFFFFFFF)
            NativeMethod.NativeMethods.SendMessage(btnAddHasherToAllFiles.Handle, NativeMethod.NativeMethods.BCM_SETSHIELD, 0, &HFFFFFFFF)
        End If

        If Not My.Settings.boolEnableServer Then btnAddHasherToAllFiles.Visible = False

        Control.CheckForIllegalCrossThreadCalls = False
        lblIndividualFilesStatusProcessingFile.Text = ""
        lblVerifyHashStatusProcessingFile.Text = ""
        lblFile1Hash.Text = ""
        lblFile2Hash.Text = ""
        lblProcessingFile.Text = ""
        lblProcessingFileVerify.Text = ""
        lblCompareFileAgainstKnownHashType.Text = ""
        chkRecurrsiveDirectorySearch.Checked = My.Settings.boolRecurrsiveDirectorySearch
        chkSSL.Checked = My.Settings.boolSSL
        chkEnableInterprocessCommunicationServer.Checked = My.Settings.boolEnableServer
        chkSortByFileSizeAfterLoadingHashFile.Checked = My.Settings.boolSortByFileSizeAfterLoadingHashFile
        lblWelcomeText.Text = String.Format(lblWelcomeText.Text, Check_for_Update_Stuff.versionString, If(Environment.Is64BitProcess, "64", "32"), If(Environment.Is64BitOperatingSystem, "64", "32"))
        Me.Size = My.Settings.windowSize

        deleteTemporaryNewEXEFile()

        If My.Application.CommandLineArgs.Count = 1 Then
            Dim commandLineArgument As String = My.Application.CommandLineArgs(0).Trim

            If commandLineArgument.StartsWith("--hashfile=", StringComparison.OrdinalIgnoreCase) Then
                commandLineArgument = commandLineArgument.caseInsensitiveReplace("--hashfile=", "")
                commandLineArgument = commandLineArgument.Replace(Chr(34), "")

                If IO.File.Exists(commandLineArgument) Then
                    TabControl1.SelectTab(3)
                    btnOpenExistingHashFile.Enabled = False
                    verifyHashesListFiles.Items.Clear()
                    processExistingHashFile(commandLineArgument)
                End If
            ElseIf commandLineArgument.StartsWith("--addfile=", StringComparison.OrdinalIgnoreCase) Then
                commandLineArgument = commandLineArgument.caseInsensitiveReplace("--addfile=", "")
                commandLineArgument = commandLineArgument.Replace(Chr(34), "")
                sendToServer(commandLineArgument) ' This is the file name.
            ElseIf commandLineArgument.StartsWith("--knownhashfile=", StringComparison.OrdinalIgnoreCase) Then
                commandLineArgument = commandLineArgument.caseInsensitiveReplace("--knownhashfile=", "")
                commandLineArgument = commandLineArgument.Replace(Chr(34), "")
                TabControl1.SelectTab(5)
                txtFileForKnownHash.Text = commandLineArgument
            End If
        End If

        colFileName.Width = My.Settings.hashIndividualFilesFileNameColumnSize
        colFileSize.Width = My.Settings.hashIndividualFilesFileSizeColumnSize
        colChecksum.Width = My.Settings.hashIndividualFilesChecksumColumnSize

        colFile.Width = My.Settings.verifyHashFileNameColumnSize
        colFileSize2.Width = My.Settings.verifyHashFileSizeColumnSize
        colResults.Width = My.Settings.verifyHashFileResults

        boolDoneLoading = True
    End Sub

    Private Sub deleteTemporaryNewEXEFile()
        Try
            Dim newExecutableName As String = New IO.FileInfo(Application.ExecutablePath).Name & ".new.exe"
            If IO.File.Exists(newExecutableName) Then IO.File.Delete(newExecutableName)
        Catch ex As Exception
        End Try
    End Sub

    Private Sub recursiveDirectorySearch(ByVal strDirectory As String, ByRef files As List(Of String))
        Try
            files.AddRange(IO.Directory.EnumerateFiles(strDirectory))
        Catch ex As Exception
        End Try

        Try
            For Each directory As String In IO.Directory.EnumerateDirectories(strDirectory)
                recursiveDirectorySearch(directory, files)
            Next
        Catch ex As Exception
        End Try
    End Sub

    Private Sub addFilesFromDirectory(directoryPath As String)
        Threading.ThreadPool.QueueUserWorkItem(Sub()
                                                   Dim listOfFiles As New List(Of ListViewItem)
                                                   Dim listViewItem As myListViewItem
                                                   Dim index As Integer = 0
                                                   Dim dblPercentage As Double

                                                   Me.Invoke(Sub()
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

                                                   Dim filesInFolder As New List(Of String)
                                                   If My.Settings.boolRecurrsiveDirectorySearch Then
                                                       recursiveDirectorySearch(directoryPath, filesInFolder)
                                                   Else
                                                       filesInFolder.AddRange(IO.Directory.EnumerateFiles(directoryPath))
                                                   End If

                                                   For Each strFileName As String In filesInFolder
                                                       index += 1

                                                       Me.Invoke(Sub()
                                                                     dblPercentage = Math.Round(index / filesInFolder.Count * 100, 2)
                                                                     lblIndividualFilesStatusProcessingFile.Text = String.Format("Scanning directory... processing file {0} of {1} ({2}%). Please Wait.", index.ToString("N0"), filesInFolder.Count.ToString("N0"), dblPercentage.ToString)
                                                                     IndividualFilesProgressBar.Value = dblPercentage
                                                                 End Sub)

                                                       If Not filesInListFiles.Contains(strFileName) Then
                                                           filesInListFiles.Add(strFileName)
                                                           listViewItem = New myListViewItem(strFileName) With {.fileSize = New IO.FileInfo(strFileName).Length}
                                                           listViewItem.SubItems.Add(fileSizeToHumanSize(listViewItem.fileSize))
                                                           listViewItem.SubItems.Add(strToBeComputed)
                                                           listViewItem.fileName = strFileName
                                                           listOfFiles.Add(listViewItem)
                                                           listViewItem = Nothing
                                                       End If
                                                   Next

                                                   Me.Invoke(Sub()
                                                                 lblIndividualFilesStatusProcessingFile.Text = "Adding files to list... Please Wait."

                                                                 listFiles.BeginUpdate()
                                                                 listFiles.Items.AddRange(listOfFiles.ToArray())
                                                                 listFiles.EndUpdate()

                                                                 lblIndividualFilesStatusProcessingFile.Text = Nothing
                                                                 lblIndividualFilesStatus.Text = strNoBackgroundProcesses
                                                                 IndividualFilesProgressBar.Value = 0
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
                                                     Dim listViewItem As myListViewItem
                                                     Dim regExMatchObject As Text.RegularExpressions.Match
                                                     Dim dataInFileArray As String() = IO.File.ReadAllLines(strPathToChecksumFile)
                                                     Dim longLineCounter As Long = 0
                                                     Dim listOfItemsToAddToListView As New List(Of myListViewItem)

                                                     lblVerifyHashStatus.Text = "Reading hash file into memory... Please Wait."

                                                     For Each strLineInFile As String In dataInFileArray
                                                         longLineCounter += 1
                                                         VerifyHashProgressBar.Value = longLineCounter / dataInFileArray.LongLength * 100

                                                         If Not String.IsNullOrEmpty(strLineInFile) Then
                                                             regExMatchObject = hashLineParser.Match(strLineInFile)

                                                             If regExMatchObject.Success Then
                                                                 strChecksum = regExMatchObject.Groups(1).Value
                                                                 strFileName = regExMatchObject.Groups(2).Value

                                                                 If Not hashLineFilePathChecker.IsMatch(strFileName) Then
                                                                     strFileName = IO.Path.Combine(strDirectoryThatContainsTheChecksumFile, strFileName)
                                                                 End If

                                                                 listViewItem = New myListViewItem(strFileName) With {
                                                                    .hash = strChecksum,
                                                                    .fileName = strFileName
                                                                 }

                                                                 If IO.File.Exists(strFileName) Then
                                                                     listViewItem.fileSize = New IO.FileInfo(strFileName).Length
                                                                     listViewItem.SubItems.Add(fileSizeToHumanSize(listViewItem.fileSize))
                                                                     listViewItem.SubItems.Add("To Be Tested")
                                                                     listViewItem.boolFileExists = True
                                                                 Else
                                                                     listViewItem.fileSize = 0
                                                                     listViewItem.SubItems.Add("")
                                                                     listViewItem.SubItems.Add("Doesn't Exist")
                                                                     listViewItem.boolFileExists = False
                                                                     listViewItem.BackColor = Color.LightGray
                                                                 End If

                                                                 listOfItemsToAddToListView.Add(listViewItem)
                                                                 listViewItem = Nothing
                                                             End If

                                                             regExMatchObject = Nothing
                                                         End If
                                                     Next

                                                     lblVerifyHashStatus.Text = "Creating ListView Item Objects... Please Wait."

                                                     verifyHashesListFiles.Items.AddRange(listOfItemsToAddToListView.ToArray())
                                                     listOfItemsToAddToListView = Nothing

                                                     If My.Settings.boolSortByFileSizeAfterLoadingHashFile Then applyFileSizeSortingToVerifyList()

                                                     lblVerifyHashStatus.Text = strNoBackgroundProcesses
                                                     VerifyHashProgressBar.Value = 0

                                                     dataInFileArray = Nothing

                                                     For Each item As myListViewItem In verifyHashesListFiles.Items
                                                         lblVerifyHashStatusProcessingFile.Text = String.Format("Processing file {0} of {1} file(s)", index.ToString("N0"), verifyHashesListFiles.Items.Count().ToString("N0"))
                                                         If item.boolFileExists Then processFileInVerifyFileList(item, checksumType, longFilesThatPassedVerification)
                                                         index += 1
                                                     Next

                                                     For Each item As myListViewItem In verifyHashesListFiles.Items
                                                         If item.boolFileExists Then item.BackColor = item.color
                                                     Next

                                                     lblVerifyHashStatusProcessingFile.Text = ""
                                                     lblVerifyHashStatus.Text = strNoBackgroundProcesses
                                                     lblProcessingFileVerify.Text = ""
                                                     VerifyHashProgressBar.Value = 0

                                                     Me.Invoke(Sub()
                                                                   Dim strMessageBoxText As String

                                                                   If longFilesThatPassedVerification = verifyHashesListFiles.Items.Count Then
                                                                       strMessageBoxText = "Processing of hash file complete. All files have passed verification."
                                                                   Else
                                                                       strMessageBoxText = String.Format("Processing of hash file complete. {0} out of {1} file(s) passed verification, {2} files didn't pass verification.", longFilesThatPassedVerification, verifyHashesListFiles.Items.Count, verifyHashesListFiles.Items.Count - longFilesThatPassedVerification)
                                                                   End If

                                                                   MsgBox(strMessageBoxText, MsgBoxStyle.Information + MsgBoxStyle.ApplicationModal, strWindowTitle)
                                                               End Sub)

                                                     boolBackgroundThreadWorking = False
                                                     workingThread = Nothing
                                                 Catch ex As Threading.ThreadAbortException
                                                     If Not boolClosingWindow Then
                                                         lblVerifyHashStatusProcessingFile.Text = ""
                                                         lblVerifyHashStatus.Text = strNoBackgroundProcesses
                                                         lblProcessingFileVerify.Text = ""
                                                         VerifyHashProgressBar.Value = 0
                                                         verifyHashesListFiles.Items.Clear()
                                                     End If

                                                     boolBackgroundThreadWorking = False
                                                     workingThread = Nothing
                                                     If Not boolClosingWindow Then Me.Invoke(Sub() MsgBox("Processing aborted.", MsgBoxStyle.Information + MsgBoxStyle.ApplicationModal, strWindowTitle))
                                                 Finally
                                                     btnOpenExistingHashFile.Enabled = True
                                                 End Try
                                             End Sub) With {
            .Priority = Threading.ThreadPriority.Highest,
            .Name = "Verify Hash File Working Thread",
            .IsBackground = True
        }
        workingThread.Start()
    End Sub

    Private Sub btnOpenExistingHashFile_Click(sender As Object, e As EventArgs) Handles btnOpenExistingHashFile.Click
        btnOpenExistingHashFile.Enabled = False
        verifyHashesListFiles.Items.Clear()

        Dim oldMultiValue As Boolean = OpenFileDialog.Multiselect

        OpenFileDialog.Title = "Select a hash file to verify..."
        OpenFileDialog.Multiselect = False
        OpenFileDialog.Filter = "Checksum File|*.md5;*.sha1;*.sha256;*.sha384;*.sha512;*.ripemd160"

        If OpenFileDialog.ShowDialog() = DialogResult.OK Then
            processExistingHashFile(OpenFileDialog.FileName)
        Else
            btnOpenExistingHashFile.Enabled = True
        End If

        OpenFileDialog.Multiselect = oldMultiValue
    End Sub

    Private Sub processFileInVerifyFileList(ByRef item As myListViewItem, hashFileType As checksumType, ByRef longFilesThatPassedVerification As Long)
        Dim strChecksum As String = item.hash
        Dim strFileName As String = item.fileName

        If IO.File.Exists(strFileName) Then
            Dim fileInfo As New IO.FileInfo(strFileName)
            Dim strChecksumInFile As String = Nothing
            Dim oldLocationInFile As ULong = 0
            Dim subRoutine As [Delegate] = Sub(size As Long, totalBytesRead As Long)
                                               Try
                                                   Me.Invoke(Sub()
                                                                 oldLocationInFile = totalBytesRead

                                                                 If totalBytesRead <> 0 And size <> 0 Then
                                                                     VerifyHashProgressBar.Value = totalBytesRead / size * 100
                                                                 Else
                                                                     VerifyHashProgressBar.Value = 0
                                                                 End If

                                                                 lblVerifyHashStatus.Text = String.Format("{0} of {1} have been processed.", fileSizeToHumanSize(totalBytesRead), fileSizeToHumanSize(size))
                                                                 oldLocationInFile = totalBytesRead
                                                             End Sub)
                                               Catch ex As Exception
                                               End Try
                                           End Sub

            lblProcessingFileVerify.Text = String.Format("Now processing file {0}.", fileInfo.Name)

            If doChecksumWithAttachedSubRoutine(strFileName, hashFileType, strChecksumInFile, subRoutine) Then
                If strChecksum.Equals(item.hash, StringComparison.OrdinalIgnoreCase) Then
                    item.color = Color.LightGreen
                    item.SubItems(2).Text = "Valid"
                    longFilesThatPassedVerification += 1
                Else
                    item.color = Color.Pink
                    item.SubItems(2).Text = "NOT Valid"
                End If
            Else
                item.color = Color.LightGray
                item.SubItems(2).Text = "(Error while calculating checksum)"
            End If

            fileInfo = Nothing
        End If
    End Sub

    Private Sub listFiles_DragDrop(sender As Object, e As DragEventArgs) Handles listFiles.DragDrop
        Dim listViewItem As myListViewItem

        For Each strItem As String In e.Data.GetData(DataFormats.FileDrop)
            If IO.File.GetAttributes(strItem).HasFlag(IO.FileAttributes.Directory) Then
                addFilesFromDirectory(strItem)
            Else
                If Not filesInListFiles.Contains(strItem) Then
                    filesInListFiles.Add(strItem)
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

    Private Sub chkRecurrsiveDirectorySearch_Click(sender As Object, e As EventArgs) Handles chkRecurrsiveDirectorySearch.Click
        My.Settings.boolRecurrsiveDirectorySearch = chkRecurrsiveDirectorySearch.Checked
    End Sub

    Private Sub txtTextToHash_TextChanged(sender As Object, e As EventArgs) Handles txtTextToHash.TextChanged
        lblHashTextStep1.Text = String.Format("Step 1: Input some text: {0} Characters", txtTextToHash.Text.Length.ToString("N0"))
        btnComputeTextHash.Enabled = If(txtTextToHash.Text.Length = 0, False, True)
        clearTextHashResults()
    End Sub

    Private Sub clearTextHashResults()
        btnCopyTextHashResultsToClipboard.Enabled = False
        txtHashResults.Text = Nothing
    End Sub

    Private Sub btnComputeTextHash_Click(sender As Object, e As EventArgs) Handles btnComputeTextHash.Click
        If textRadioMD5.Checked Then
            txtHashResults.Text = getHashOfString(txtTextToHash.Text, checksumType.md5)
        ElseIf textRadioSHA1.Checked Then
            txtHashResults.Text = getHashOfString(txtTextToHash.Text, checksumType.sha160)
        ElseIf textRadioSHA256.Checked Then
            txtHashResults.Text = getHashOfString(txtTextToHash.Text, checksumType.sha256)
        ElseIf textRadioSHA384.Checked Then
            txtHashResults.Text = getHashOfString(txtTextToHash.Text, checksumType.sha384)
        ElseIf textRadioSHA512.Checked Then
            txtHashResults.Text = getHashOfString(txtTextToHash.Text, checksumType.sha512)
        End If

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
                If udpClient IsNot Nothing Then udpClient.Close()
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

    Private Sub addHashFileHeader(ByRef stringBuilder As Text.StringBuilder)
        stringBuilder.AppendLine("'")
        stringBuilder.AppendLine(String.Format("' Hash file generated by Hasher, version {0}. Written by Tom Parkison.", Check_for_Update_Stuff.versionString))
        stringBuilder.AppendLine("' Web Site: https://www.toms-world.org/blog/hasher")
        stringBuilder.AppendLine("' Source Code Available At: https://bitbucket.org/trparky/hasher")
        stringBuilder.AppendLine("'")
    End Sub

    Private Sub CopyHashToClipboardToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CopyHashToClipboardToolStripMenuItem.Click
        Dim stringBuilder As New Text.StringBuilder
        addHashFileHeader(stringBuilder)

        If listFiles.SelectedItems.Count = 1 Then
            Dim selectedItem As myListViewItem = listFiles.SelectedItems(0)
            stringBuilder.AppendLine(selectedItem.hash & " *" & selectedItem.fileName)
        Else
            For Each item As myListViewItem In listFiles.SelectedItems
                stringBuilder.AppendLine(item.hash & " *" & item.fileName)
            Next
        End If

        If copyTextToWindowsClipboard(stringBuilder.ToString.Trim) Then MsgBox("The hash result has been copied to the Windows Clipboard.", MsgBoxStyle.Information, strWindowTitle)
    End Sub

    Private Sub applyFileSizeSortingToVerifyList()
        Dim new_sorting_column As ColumnHeader = verifyHashesListFiles.Columns(1)
        Dim sort_order As SortOrder = SortOrder.Ascending

        m_SortingColumn1 = new_sorting_column
        m_SortingColumn1.Text = "> File Size"

        verifyHashesListFiles.ListViewItemSorter = New ListViewComparer(1, sort_order)
        verifyHashesListFiles.Sort()
    End Sub

    Private Sub verifyHashesListFiles_ColumnClick(sender As Object, e As ColumnClickEventArgs) Handles verifyHashesListFiles.ColumnClick
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
        Dim checkForUpdatesClassObject As New Check_for_Update_Stuff(Me)
        checkForUpdatesClassObject.checkForUpdates()
    End Sub

    Private Sub btnCompareFiles_Click(sender As Object, e As EventArgs) Handles btnCompareFiles.Click
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

        btnCompareFilesBrowseFile1.Enabled = False
        btnCompareFilesBrowseFile2.Enabled = False
        txtFile1.Enabled = False
        txtFile2.Enabled = False
        btnCompareFiles.Enabled = False

        workingThread = New Threading.Thread(Sub()
                                                 Try
                                                     boolBackgroundThreadWorking = True
                                                     Dim checksumType As checksumType

                                                     compareRadioMD5.Enabled = False
                                                     compareRadioSHA1.Enabled = False
                                                     compareRadioSHA256.Enabled = False
                                                     compareRadioSHA384.Enabled = False
                                                     compareRadioSHA512.Enabled = False

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

                                                     Dim strChecksum1 As String = Nothing
                                                     Dim strChecksum2 As String = Nothing
                                                     Dim boolSuccessful As Boolean = False
                                                     Dim oldLocationInFile As ULong = 0
                                                     Dim subRoutine As [Delegate] = Sub(size As Long, totalBytesRead As Long)
                                                                                        Try
                                                                                            Me.Invoke(Sub()
                                                                                                          oldLocationInFile = totalBytesRead

                                                                                                          If totalBytesRead <> 0 And size <> 0 Then
                                                                                                              compareFilesProgressBar.Value = totalBytesRead / size * 100
                                                                                                          Else
                                                                                                              compareFilesProgressBar.Value = 0
                                                                                                          End If

                                                                                                          lblCompareFilesStatus.Text = String.Format("{0} of {1} have been processed.", fileSizeToHumanSize(totalBytesRead), fileSizeToHumanSize(size))
                                                                                                          oldLocationInFile = totalBytesRead
                                                                                                      End Sub)
                                                                                        Catch ex As Exception
                                                                                        End Try
                                                                                    End Sub

                                                     If doChecksumWithAttachedSubRoutine(txtFile1.Text, checksumType, strChecksum1, subRoutine) AndAlso doChecksumWithAttachedSubRoutine(txtFile2.Text, checksumType, strChecksum2, subRoutine) Then
                                                         lblFile1Hash.Text = "Hash/Checksum: " & strChecksum1
                                                         lblFile2Hash.Text = "Hash/Checksum: " & strChecksum2
                                                         ToolTip.SetToolTip(lblFile1Hash, strChecksum1)
                                                         ToolTip.SetToolTip(lblFile2Hash, strChecksum2)
                                                         boolSuccessful = True
                                                     End If

                                                     btnCompareFilesBrowseFile1.Enabled = True
                                                     btnCompareFilesBrowseFile2.Enabled = True
                                                     txtFile1.Enabled = True
                                                     txtFile2.Enabled = True
                                                     btnCompareFiles.Enabled = True
                                                     compareRadioMD5.Enabled = True
                                                     compareRadioSHA1.Enabled = True
                                                     compareRadioSHA256.Enabled = True
                                                     compareRadioSHA384.Enabled = True
                                                     compareRadioSHA512.Enabled = True
                                                     lblCompareFilesStatus.Text = strNoBackgroundProcesses
                                                     compareFilesProgressBar.Value = 0

                                                     If boolSuccessful Then
                                                         If strChecksum1.Equals(strChecksum2, StringComparison.OrdinalIgnoreCase) Then
                                                             MsgBox("Both files are the same.", MsgBoxStyle.Information, strWindowTitle)
                                                         Else
                                                             MsgBox("The two files don't match.", MsgBoxStyle.Critical, strWindowTitle)
                                                         End If
                                                     Else
                                                         MsgBox("There was an error while calculating the checksum.", MsgBoxStyle.Critical, strWindowTitle)
                                                     End If

                                                     boolBackgroundThreadWorking = False
                                                     workingThread = Nothing
                                                 Catch ex As Threading.ThreadAbortException
                                                     If Not boolClosingWindow Then
                                                         btnCompareFilesBrowseFile1.Enabled = True
                                                         btnCompareFilesBrowseFile1.Enabled = True
                                                         txtFile1.Enabled = True
                                                         txtFile2.Enabled = True
                                                         btnCompareFiles.Enabled = True
                                                         compareRadioMD5.Enabled = True
                                                         compareRadioSHA1.Enabled = True
                                                         compareRadioSHA256.Enabled = True
                                                         compareRadioSHA384.Enabled = True
                                                         compareRadioSHA512.Enabled = True
                                                         lblCompareFilesStatus.Text = strNoBackgroundProcesses
                                                     End If

                                                     boolBackgroundThreadWorking = False
                                                     workingThread = Nothing
                                                     If Not boolClosingWindow Then Me.Invoke(Sub() MsgBox("Processing aborted.", MsgBoxStyle.Information + MsgBoxStyle.ApplicationModal, strWindowTitle))
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

    Private Sub btnCompareFilesBrowseFile1_Click(sender As Object, e As EventArgs) Handles btnCompareFilesBrowseFile1.Click
        lblFile1Hash.Text = ""
        ToolTip.SetToolTip(lblFile1Hash, "")

        OpenFileDialog.Title = "Select file #1 to be compared..."
        OpenFileDialog.Multiselect = False
        OpenFileDialog.Filter = "Show All Files|*.*"

        If OpenFileDialog.ShowDialog() = DialogResult.OK Then txtFile1.Text = OpenFileDialog.FileName
    End Sub

    Private Sub btnCompareFilesBrowseFile2_Click(sender As Object, e As EventArgs) Handles btnCompareFilesBrowseFile2.Click
        lblFile2Hash.Text = ""
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
        OpenFileDialog.Title = "Select file for known hash comparison..."
        OpenFileDialog.Multiselect = False
        OpenFileDialog.Filter = "Show All Files|*.*"

        If OpenFileDialog.ShowDialog() = DialogResult.OK Then txtFileForKnownHash.Text = OpenFileDialog.FileName
    End Sub

    Private Sub txtKnownHash_TextChanged(sender As Object, e As EventArgs) Handles txtKnownHash.TextChanged
        If String.IsNullOrWhiteSpace(txtKnownHash.Text) Then
            lblCompareFileAgainstKnownHashType.Text = ""
            btnCompareAgainstKnownHash.Enabled = False
        Else
            If txtKnownHash.Text.Length = 128 Or txtKnownHash.Text.Length = 96 Or txtKnownHash.Text.Length = 64 Or txtKnownHash.Text.Length = 40 Or txtKnownHash.Text.Length = 32 Then
                btnCompareAgainstKnownHash.Enabled = True

                'lblCompareFileAgainstKnownHashType
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

    Private Sub btnCompareAgainstKnownHash_Click(sender As Object, e As EventArgs) Handles btnCompareAgainstKnownHash.Click
        txtFileForKnownHash.Text = txtFileForKnownHash.Text.Trim

        If Not IO.File.Exists(txtFileForKnownHash.Text) Then
            MsgBox("File doesn't exist.", MsgBoxStyle.Critical, strWindowTitle)
            Exit Sub
        End If

        txtFileForKnownHash.Enabled = False
        btnBrowseFileForCompareKnownHash.Enabled = False
        txtKnownHash.Enabled = False
        btnCompareAgainstKnownHash.Enabled = False

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
                                                     Dim oldLocationInFile As ULong = 0
                                                     Dim subRoutine As [Delegate] = Sub(size As Long, totalBytesRead As Long)
                                                                                        Try
                                                                                            Me.Invoke(Sub()
                                                                                                          oldLocationInFile = totalBytesRead

                                                                                                          If totalBytesRead <> 0 And size <> 0 Then
                                                                                                              compareAgainstKnownHashProgressBar.Value = totalBytesRead / size * 100
                                                                                                          Else
                                                                                                              compareAgainstKnownHashProgressBar.Value = 0
                                                                                                          End If

                                                                                                          lblCompareAgainstKnownHashStatus.Text = String.Format("{0} of {1} have been processed.", fileSizeToHumanSize(totalBytesRead), fileSizeToHumanSize(size))
                                                                                                          oldLocationInFile = totalBytesRead
                                                                                                      End Sub)
                                                                                        Catch ex As Exception
                                                                                        End Try
                                                                                    End Sub

                                                     Dim boolSuccessful As Boolean = doChecksumWithAttachedSubRoutine(txtFileForKnownHash.Text, checksumType, strChecksum, subRoutine)

                                                     txtFileForKnownHash.Enabled = True
                                                     btnBrowseFileForCompareKnownHash.Enabled = True
                                                     txtKnownHash.Enabled = True
                                                     btnCompareAgainstKnownHash.Enabled = True
                                                     lblCompareAgainstKnownHashStatus.Text = strNoBackgroundProcesses
                                                     compareAgainstKnownHashProgressBar.Value = 0

                                                     If boolSuccessful Then
                                                         If strChecksum.Equals(txtKnownHash.Text.Trim, StringComparison.OrdinalIgnoreCase) Then
                                                             MsgBox("The checksums match!", MsgBoxStyle.Information, strWindowTitle)
                                                         Else
                                                             MsgBox("The checksums DON'T match!", MsgBoxStyle.Critical, strWindowTitle)
                                                         End If
                                                     Else
                                                         MsgBox("There was an error while calculating the checksum.", MsgBoxStyle.Critical, strWindowTitle)
                                                     End If

                                                     boolBackgroundThreadWorking = False
                                                     workingThread = Nothing
                                                 Catch ex As Threading.ThreadAbortException
                                                     If Not boolClosingWindow Then
                                                         txtFileForKnownHash.Enabled = True
                                                         btnBrowseFileForCompareKnownHash.Enabled = True
                                                         txtKnownHash.Enabled = True
                                                         btnCompareAgainstKnownHash.Enabled = True
                                                         lblCompareFilesStatus.Text = strNoBackgroundProcesses
                                                     End If

                                                     boolBackgroundThreadWorking = False
                                                     workingThread = Nothing
                                                     If Not boolClosingWindow Then Me.Invoke(Sub() MsgBox("Processing aborted.", MsgBoxStyle.Information + MsgBoxStyle.ApplicationModal, strWindowTitle))
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

    Private Sub Form1_ResizeEnd(sender As Object, e As EventArgs) Handles Me.ResizeEnd
        My.Settings.windowSize = Me.Size
    End Sub

    Private Function getHashOfString(inputString As String, hashType As checksumType) As String
        Dim HashAlgorithm As Security.Cryptography.HashAlgorithm = getHashEngine(hashType)
        Dim Output As Byte() = HashAlgorithm.ComputeHash(System.Text.Encoding.UTF8.GetBytes(inputString))
        Return BitConverter.ToString(Output).ToLower().Replace("-", "")
    End Function

    Private Sub listFiles_ColumnWidthChanged(sender As Object, e As ColumnWidthChangedEventArgs) Handles listFiles.ColumnWidthChanged
        If Not boolDoneLoading Then Exit Sub
        My.Settings.hashIndividualFilesFileNameColumnSize = colFileName.Width
        My.Settings.hashIndividualFilesFileSizeColumnSize = colFileSize.Width
        My.Settings.hashIndividualFilesChecksumColumnSize = colChecksum.Width
    End Sub

    Private Sub verifyHashesListFiles_ColumnWidthChanged(sender As Object, e As ColumnWidthChangedEventArgs) Handles verifyHashesListFiles.ColumnWidthChanged
        If Not boolDoneLoading Then Exit Sub
        My.Settings.verifyHashFileNameColumnSize = colFile.Width
        My.Settings.verifyHashFileSizeColumnSize = colFileSize2.Width
        My.Settings.verifyHashFileResults = colResults.Width
    End Sub

    Private Sub txtFile1_DragEnter(sender As Object, e As DragEventArgs) Handles txtFile1.DragEnter
        e.Effect = If(e.Data.GetDataPresent(DataFormats.FileDrop), DragDropEffects.All, DragDropEffects.None)
    End Sub

    Private Sub txtFile1_DragDrop(sender As Object, e As DragEventArgs) Handles txtFile1.DragDrop
        Dim receivedData As String() = DirectCast(e.Data.GetData(DataFormats.FileDrop), String())
        If receivedData.Count = 1 Then txtFile1.Text = receivedData(0)
    End Sub

    Private Sub txtFile2_DragEnter(sender As Object, e As DragEventArgs) Handles txtFile2.DragEnter
        e.Effect = If(e.Data.GetDataPresent(DataFormats.FileDrop), DragDropEffects.All, DragDropEffects.None)
    End Sub

    Private Sub txtFile2_DragDrop(sender As Object, e As DragEventArgs) Handles txtFile2.DragDrop
        Dim receivedData As String() = DirectCast(e.Data.GetData(DataFormats.FileDrop), String())
        If receivedData.Count = 1 Then txtFile2.Text = receivedData(0)
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
                verifyHashesListFiles.Items.Clear()
                btnOpenExistingHashFile.Enabled = False
                processExistingHashFile(strReceivedFileName)
            Else
                MsgBox("Invalid file type.", MsgBoxStyle.Critical, strWindowTitle)
            End If
        End If
    End Sub

    Private Sub chkEnableInterprocessCommunicationServer_Click(sender As Object, e As EventArgs) Handles chkEnableInterprocessCommunicationServer.Click
        My.Settings.boolEnableServer = chkEnableInterprocessCommunicationServer.Checked
        My.Settings.Save()
        MsgBox("Hasher needs to restart, the application will now close and restart.", MsgBoxStyle.Information, strWindowTitle)
        Process.Start(Application.ExecutablePath)
        Process.GetCurrentProcess.Kill()
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
End Class