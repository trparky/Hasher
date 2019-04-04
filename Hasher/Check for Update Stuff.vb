Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions
Imports System.Xml
Imports System.Security.AccessControl
Imports System.Security.Principal

Class Check_for_Update_Stuff
    Private Const programZipFileURL = "www.toms-world.org/download/Hasher.zip"
    Private Const programZipFileSHA256URL = "www.toms-world.org/download/Hasher.zip.sha2"
    Private Const programFileNameInZIP As String = "Hasher.exe"
    Private Const programUpdateCheckerXMLFile As String = "www.toms-world.org/updates/hasher_update.xml"

    Public windowObject As Form1

    Public Sub New(inputWindowObject As Form1)
        windowObject = inputWindowObject
    End Sub

    Private Sub extractFileFromZIPFile(ByRef memoryStream As MemoryStream, fileToExtract As String, fileToWriteExtractedFileTo As String)
        Using zipFileObject As New Compression.ZipArchive(memoryStream, Compression.ZipArchiveMode.Read)
            Using fileStream As New FileStream(fileToWriteExtractedFileTo, FileMode.Create)
                zipFileObject.GetEntry(fileToExtract).Open().CopyTo(fileStream)
            End Using
        End Using
    End Sub

    Public Shared versionInfo As String() = Application.ProductVersion.Split(".")
    Private shortBuild As Short = Short.Parse(versionInfo(versionPieces.build).Trim)
    Public Shared versionString As String = String.Format("{0}.{1} Build {2}", versionInfo(0), versionInfo(1), versionInfo(2))

    Private versionStringWithoutBuild As String = String.Format("{0}.{1}", versionInfo(versionPieces.major), versionInfo(versionPieces.minor))

    ''' <summary>This parses the XML update data and determines if an update is needed.</summary>
    ''' <param name="xmlData">The XML data from the web site.</param>
    ''' <returns>A Boolean value indicating if the program has been updated or not.</returns>
    Private Function processUpdateXMLData(ByVal xmlData As String, ByRef remoteVersion As String, ByRef remoteBuild As String) As Boolean
        Try
            Dim xmlDocument As New XmlDocument() ' First we create an XML Document Object.
            xmlDocument.Load(New StringReader(xmlData)) ' Now we try and parse the XML data.

            Dim xmlNode As XmlNode = xmlDocument.SelectSingleNode("/xmlroot")

            remoteVersion = xmlNode.SelectSingleNode("version").InnerText.Trim
            remoteBuild = xmlNode.SelectSingleNode("build").InnerText.Trim
            Dim shortRemoteBuild As Short

            ' This checks to see if current version and the current build matches that of the remote values in the XML document.
            If remoteVersion.Equals(versionStringWithoutBuild) And remoteBuild.Equals(shortBuild.ToString) Then
                ' OK, they match so there's no update to download and update to therefore we return a False value.
                Return False
            Else
                If Short.TryParse(remoteBuild, shortRemoteBuild) And remoteVersion.Equals(versionStringWithoutBuild) Then
                    If shortRemoteBuild < shortBuild Then
                        ' This is weird, the remote build is less than the current build. Something went wrong. So to be safe we're going to return a False value indicating that there is no update to download. Better to be safe.
                        Return False
                    End If
                End If

                ' We return a True value indicating that there is a new version to download and install.
                Return True
            End If
        Catch ex As XPath.XPathException
            ' Something went wrong so we return a False value.
            Return False
        Catch ex As XmlException
            ' Something went wrong so we return a False value.
            Return False
        Catch ex As Exception
            ' Something went wrong so we return a False value.
            Return False
        End Try
    End Function

    Private Function canIWriteToTheCurrentDirectory() As Boolean
        Return canIWriteThere(New FileInfo(Application.ExecutablePath).DirectoryName)
    End Function

    Private Function canIWriteThere(folderPath As String) As Boolean
        ' We make sure we get valid folder path by taking off the leading slash.
        If folderPath.EndsWith("\") Then folderPath = folderPath.Substring(0, folderPath.Length - 1)

        If String.IsNullOrEmpty(folderPath) Or Not Directory.Exists(folderPath) Then Return False

        If checkByFolderACLs(folderPath) Then
            Try
                File.Create(Path.Combine(folderPath, "test.txt"), 1, FileOptions.DeleteOnClose).Close()
                If File.Exists(Path.Combine(folderPath, "test.txt")) Then File.Delete(Path.Combine(folderPath, "test.txt"))
                Return True
            Catch ex As Exception
                Return False
            End Try
        Else
            Return False
        End If
    End Function

    Private Function checkByFolderACLs(folderPath As String) As Boolean
        Try
            Dim directoryACLs As DirectorySecurity = Directory.GetAccessControl(folderPath)
            Dim directoryUsers As String = WindowsIdentity.GetCurrent.User.Value
            Dim directoryAccessRights As FileSystemAccessRule
            Dim fileSystemRightsVariable As FileSystemRights

            For Each rule As AuthorizationRule In directoryACLs.GetAccessRules(True, True, GetType(SecurityIdentifier))
                If rule.IdentityReference.Value = directoryUsers Then
                    directoryAccessRights = DirectCast(rule, FileSystemAccessRule)

                    If directoryAccessRights.AccessControlType = AccessControlType.Allow Then
                        fileSystemRightsVariable = directoryAccessRights.FileSystemRights

                        If fileSystemRightsVariable = (FileSystemRights.Read Or FileSystemRights.Modify Or FileSystemRights.Write Or FileSystemRights.FullControl) Then
                            Return True
                        End If
                    End If
                End If
            Next

            Return False
        Catch ex As Exception
            Return False
        End Try
    End Function

    Private Function createNewHTTPHelperObject() As httpHelper
        Dim httpHelper As New httpHelper With {
            .setUserAgent = createHTTPUserAgentHeaderString(),
            .useHTTPCompression = True,
            .setProxyMode = True
        }
        httpHelper.addHTTPHeader("PROGRAM_NAME", "Hasher")
        httpHelper.addHTTPHeader("PROGRAM_VERSION", versionString)
        httpHelper.addHTTPHeader("OPERATING_SYSTEM", getFullOSVersionString())

        httpHelper.setURLPreProcessor = Function(ByVal strURLInput As String) As String
                                            Try
                                                If Not strURLInput.Trim.ToLower.StartsWith("http") Then
                                                    Return If(My.Settings.boolSSL, "https://", "http://") & strURLInput
                                                Else
                                                    Return strURLInput
                                                End If
                                            Catch ex As Exception
                                                Return strURLInput
                                            End Try
                                        End Function

        Return httpHelper
    End Function

    Private Function SHA256ChecksumStream(ByRef stream As Stream) As String
        Dim SHA256Engine As New Security.Cryptography.SHA256CryptoServiceProvider
        Return BitConverter.ToString(SHA256Engine.ComputeHash(stream)).ToLower().Replace("-", "").Trim
    End Function

    Private Function verifyChecksum(urlOfChecksumFile As String, ByRef memStream As MemoryStream, ByRef httpHelper As httpHelper, boolGiveUserAnErrorMessage As Boolean) As Boolean
        Dim checksumFromWeb As String = Nothing
        memStream.Position = 0

        Try
            If httpHelper.getWebData(urlOfChecksumFile, checksumFromWeb) Then
                ' Checks to see if we have a valid SHA1 file.
                If Regex.IsMatch(checksumFromWeb, "([a-zA-Z0-9]{64})") Then
                    ' Now that we have a valid SHA256 file we need to parse out what we want.
                    checksumFromWeb = Regex.Match(checksumFromWeb, "([a-zA-Z0-9]{64})").Groups(1).Value.Trim

                    ' Now we do the actual checksum verification by passing the name of the file to the SHA256() function
                    ' which calculates the checksum of the file on disk. We then compare it to the checksum from the web.
                    If SHA256ChecksumStream(memStream).Equals(checksumFromWeb, StringComparison.OrdinalIgnoreCase) Then
                        Return True ' OK, things are good; the file passed checksum verification so we return True.
                    Else
                        ' The checksums don't match. Oops.
                        If boolGiveUserAnErrorMessage Then
                            MsgBox("There was an error in the download, checksums don't match. Update process aborted.", MsgBoxStyle.Critical, "Restore Point Creator")
                        End If

                        Return False
                    End If
                Else
                    If boolGiveUserAnErrorMessage Then
                        MsgBox("Invalid SHA2 file detected. Update process aborted.", MsgBoxStyle.Critical, "Restore Point Creator")
                    End If

                    Return False
                End If
            Else
                If boolGiveUserAnErrorMessage Then
                    MsgBox("There was an error downloading the checksum verification file. Update process aborted.", MsgBoxStyle.Critical, "Restore Point Creator")
                End If

                Return False
            End If
        Catch ex As Exception
            If boolGiveUserAnErrorMessage Then
                MsgBox("There was an error downloading the checksum verification file. Update process aborted.", MsgBoxStyle.Critical, "Restore Point Creator")
            End If

            Return False
        End Try
    End Function

    Private Sub downloadAndPerformUpdate()
        Dim fileInfo As New FileInfo(Application.ExecutablePath)
        Dim newExecutableName As String = fileInfo.Name & ".new.exe"

        ' We have to do this stuff on the thread that the form belongs to or we will get an error.
        windowObject.Invoke(Sub()
                                windowObject.lblDownloadNotification.Visible = True
                            End Sub)

        Dim httpHelper As httpHelper = createNewHTTPHelperObject()
        httpHelper.setDownloadStatusUpdateRoutine = Function(downloadStatusDetails As downloadStatusDetails)
                                                        windowObject.Invoke(Sub()
                                                                                windowObject.lblDownloadNotification.Text = String.Format("{0}% Downloaded.", downloadStatusDetails.percentageDownloaded.ToString)
                                                                            End Sub)
                                                        Return Nothing
                                                    End Function

        Dim memoryStream As New MemoryStream()
        If Not httpHelper.downloadFile(programZipFileURL, memoryStream, False) Then
            MsgBox("There was an error while downloading required files.", MsgBoxStyle.Critical, "Scheduled Task Scanner")
            Exit Sub
        End If

        If Not verifyChecksum(programZipFileSHA256URL, memoryStream, httpHelper, True) Then
            MsgBox("There was an error while downloading required files.", MsgBoxStyle.Critical, "Scheduled Task Scanner")
            Exit Sub
        End If

        fileInfo = Nothing
        memoryStream.Position = 0

        extractFileFromZIPFile(memoryStream, programFileNameInZIP, newExecutableName)

        Dim startInfo As New ProcessStartInfo With {
            .FileName = newExecutableName,
            .Arguments = "-update"
        }
        If Not canIWriteToTheCurrentDirectory() Then startInfo.Verb = "runas"
        Process.Start(startInfo)

        Process.GetCurrentProcess.Kill()
    End Sub

    ''' <summary>Creates a User Agent String for this program to be used in HTTP requests.</summary>
    ''' <returns>String type.</returns>
    Private Function createHTTPUserAgentHeaderString() As String
        Dim versionInfo As String() = Application.ProductVersion.Split(".")
        Dim versionString As String = String.Format("{0}.{1} Build {2}", versionInfo(0), versionInfo(1), versionInfo(2))
        Return String.Format("Hasher version {0} on {1}", versionString, getFullOSVersionString())
    End Function

    Private Function getFullOSVersionString() As String
        Try
            Dim intOSMajorVersion As Integer = Environment.OSVersion.Version.Major
            Dim intOSMinorVersion As Integer = Environment.OSVersion.Version.Minor
            Dim dblDOTNETVersion As Double = Double.Parse(Environment.Version.Major & "." & Environment.Version.Minor)
            Dim strOSName As String

            If intOSMajorVersion = 5 And intOSMinorVersion = 0 Then
                strOSName = "Windows 2000"
            ElseIf intOSMajorVersion = 5 And intOSMinorVersion = 1 Then
                strOSName = "Windows XP"
            ElseIf intOSMajorVersion = 6 And intOSMinorVersion = 0 Then
                strOSName = "Windows Vista"
            ElseIf intOSMajorVersion = 6 And intOSMinorVersion = 1 Then
                strOSName = "Windows 7"
            ElseIf intOSMajorVersion = 6 And intOSMinorVersion = 2 Then
                strOSName = "Windows 8"
            ElseIf intOSMajorVersion = 6 And intOSMinorVersion = 3 Then
                strOSName = "Windows 8.1"
            ElseIf intOSMajorVersion = 10 Then
                strOSName = "Windows 10"
            Else
                strOSName = String.Format("Windows NT {0}.{1}", intOSMajorVersion, intOSMinorVersion)
            End If

            Return String.Format("{0} {2}-bit (Microsoft .NET {1})", strOSName, dblDOTNETVersion, If(Environment.Is64BitOperatingSystem, "64", "32"))
        Catch ex As Exception
            Try
                Return "Unknown Windows Operating System (" & Environment.OSVersion.VersionString & ")"
            Catch ex2 As Exception
                Return "Unknown Windows Operating System"
            End Try
        End Try
    End Function

    Public Sub checkForUpdates()
        windowObject.Invoke(Sub()
                                windowObject.btnCheckForUpdates.Enabled = False
                            End Sub)

        If Not checkForInternetConnection() Then
            MsgBox("No Internet connection detected.", MsgBoxStyle.Information, windowObject.Text)
        Else
            Try
                Dim xmlData As String = Nothing
                Dim httpHelper As httpHelper = createNewHTTPHelperObject()

                If httpHelper.getWebData(programUpdateCheckerXMLFile, xmlData, False) Then
                    Dim remoteVersion As String = Nothing
                    Dim remoteBuild As String = Nothing

                    If processUpdateXMLData(xmlData, remoteVersion, remoteBuild) Then
                        If MsgBox(String.Format("An update to Hasher (version {0} Build {1}) is available to be downloaded, do you want to download and update to this new version?", remoteVersion, remoteBuild), MsgBoxStyle.Question + MsgBoxStyle.YesNo, windowObject.Text) = MsgBoxResult.Yes Then
                            downloadAndPerformUpdate()
                        Else
                            MsgBox("The update will not be downloaded.", MsgBoxStyle.Information, windowObject.Text)
                        End If
                    Else
                        MsgBox("You already have the latest version.", MsgBoxStyle.Information, windowObject.Text)
                    End If
                Else
                    MsgBox("There was an error checking for updates.", MsgBoxStyle.Information, windowObject.Text)
                End If
            Catch ex As Exception
                ' Ok, we crashed but who cares.
            Finally
                windowObject.Invoke(Sub()
                                        windowObject.btnCheckForUpdates.Enabled = True
                                    End Sub)
                windowObject = Nothing
            End Try
        End If
    End Sub

    Private Function checkForInternetConnection() As Boolean
        Return My.Computer.Network.IsAvailable
    End Function
End Class

Module StringExtensions
    ' PHP like addSlashes and stripSlashes.  Call using String.addSlashes() and String.stripSlashes().

    <Extension()>
    Public Function addSlashes(unsafeString As String) As String
        Return Regex.Replace(unsafeString, "([\000\010\011\012\015\032\042\047\134\140])", "\$1")
    End Function

    ' Un-quote string quoted with addslashes()
    <Extension()>
    Public Function stripSlashes(safeString As String) As String
        Return Regex.Replace(safeString, "\\([\000\010\011\012\015\032\042\047\134\140])", "$1")
    End Function

    ''' <summary>This function uses a RegEx search to do a case-insensitive search. This function operates a lot like Contains().</summary>
    ''' <param name="needle">The String containing what you want to search for.</param>
    ''' <param name="boolDoEscaping">This tells the function if it should add slashes where appropriate to the "needle" String.</param>
    ''' <return>Returns a Boolean value.</return>
    <Extension()>
    Public Function caseInsensitiveContains(haystack As String, needle As String, Optional boolDoEscaping As Boolean = False) As Boolean
        Try
            If boolDoEscaping Then needle = Regex.Escape(needle)
            Return Regex.IsMatch(haystack, needle, RegexOptions.IgnoreCase)
        Catch ex As Exception
            Return False
        End Try
    End Function

    ''' <summary>This function operates a lot like Replace() but is case-InSeNsItIvE.</summary>
    ''' <param name="source">The source String, aka the String where the data will be replaced in.</param>
    ''' <param name="replace">What you want to replace in the String.</param>
    ''' <param name="replaceWith">What you want to replace with in the String.</param>
    ''' <param name="boolEscape">This is an optional parameter, the default is True. This parameter gives you far more control over how the function works. With this parameter set to True the function automatically properly escapes the "replace" parameter for use in the RegEx replace function that operates inside this function. If this parameter is set to False it is up to you, the programmer, to properly escape the value of the "replace" parameter or this function will throw an exception.</param>
    ''' <return>Returns a String value.</return>
    <Extension()>
    Public Function caseInsensitiveReplace(source As String, replace As String, replaceWith As String, Optional boolEscape As Boolean = True) As String
        If boolEscape Then replace = Regex.Escape(replace)
        Return Regex.Replace(source, replace, replaceWith, RegexOptions.IgnoreCase)
    End Function
End Module

Public Enum versionPieces As Short
    major = 0
    minor = 1
    build = 2
    revision = 3
End Enum