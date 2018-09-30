Public Class checksums

    ' How to use this VB.NET Class

    ' CRC
    'Dim CRC As New CRC32()
    'Dim fsFileStream As FileStream = New FileStream("file", FileMode.Open, FileAccess.Read, FileShare.Read, 1024)
    'Dim intCRCResult As Integer = CRC.GetCrc32(fsFileStream)
    'Dim strCRCHexResult As String = Hex(intCRCResult)

    ' MD5
    'Dim CRC As New CRC32()
    'Dim fsFileStream As FileStream = New FileStream("file", FileMode.Open, FileAccess.Read, FileShare.Read, 1024)
    'Dim strMD5Result As String = CRC.MD5(fsFileStream)

    ' This is v2 of the VB CRC32 algorithm provided by Paul
    ' (wpsjr1@succeed.net) - much quicker than the nasty
    ' original version I posted.  Excellent work!

    Private crc32Table() As Integer
    Private Const BUFFER_SIZE As Integer = 1024
    Private checksumStatusUpdater As [Delegate]
    Private checksumStatusUpdaterThread As Threading.Thread = Nothing
    Private fileStream As IO.Stream

    ''' <summary>This allows you to set up a function to be run while your checksum is being processed. This function can be used to update things on the GUI during a checksum.</summary>
    ''' <value>A Lambda</value>
    ''' <example>
    ''' A VB.NET Example...
    ''' checksum.setChecksumStatusUpdateRoutine(Function(ByVal checksumStatusDetails As checksumStatusDetails)
    ''' End Function)
    ''' OR A C# Example...
    ''' checksum.setChecksumStatusUpdateRoutine((checksumStatusDetails checksumStatusDetails) => { })
    ''' </example>
    Public WriteOnly Property setChecksumStatusUpdateRoutine As [Delegate]
        Set(value As [Delegate])
            checksumStatusUpdater = value
        End Set
    End Property

    Public WriteOnly Property setFileStream As IO.Stream
        Set(value As IO.Stream)
            fileStream = value
        End Set
    End Property

    Public Function GetCrc32() As Integer
        Dim crc32Result As Integer
        crc32Result = &HFFFFFFFF

        Dim buffer(BUFFER_SIZE) As Byte
        Dim readSize As Integer = BUFFER_SIZE

        checksumStatusUpdateInvoker()
        Dim count As Integer = fileStream.Read(buffer, 0, readSize)
        Dim i As Integer
        Dim iLookup As Integer
        Dim tot As Integer = 0
        Do While count > 0
            For i = 0 To count - 1
                iLookup = (crc32Result And &HFF) Xor buffer(i)
                crc32Result = ((crc32Result And &HFFFFFF00) \ &H100) And 16777215 ' nasty shr 8 with vb :/
                crc32Result = crc32Result Xor crc32Table(iLookup)
            Next i
            count = fileStream.Read(buffer, 0, readSize)
        Loop

        checksumStatusUpdaterThread.Abort()
        Return Not crc32Result
    End Function

    Public Sub dispose()
        fileStream.Close()
        fileStream.Dispose()
    End Sub

    Private Sub checksumStatusUpdaterThreadSubroutine()
        Try
beginAgain:
            checksumStatusUpdater.DynamicInvoke(fileStream.Length, fileStream.Position)
            If My.Settings.boolEnablePerSecondStatusUpdates Then Threading.Thread.Sleep(1000)
            GoTo beginAgain
        Catch ex As Threading.ThreadAbortException
            ' Does nothing
        Catch ex2 As Reflection.TargetInvocationException
            ' Does nothing
        End Try
    End Sub

    Public Sub New()
        ' This is the official polynomial used by CRC32 in PKZip.
        ' Often the polynomial is shown reversed (04C11DB7).
        Dim dwPolynomial As Integer = &HEDB88320
        Dim i As Integer, j As Integer

        ReDim crc32Table(256)
        Dim dwCrc As Integer

        For i = 0 To 255
            dwCrc = i
            For j = 8 To 1 Step -1
                If (dwCrc And 1) Then
                    dwCrc = ((dwCrc And &HFFFFFFFE) \ 2&) And &H7FFFFFFF
                    dwCrc = dwCrc Xor dwPolynomial
                Else
                    dwCrc = ((dwCrc And &HFFFFFFFE) \ 2&) And &H7FFFFFFF
                End If
            Next j
            crc32Table(i) = dwCrc
        Next i
    End Sub

    Private Sub checksumStatusUpdateInvoker()
        ' Checks to see if we have a status update routine to invoke.
        If checksumStatusUpdater IsNot Nothing Then
            ' We invoke the status update routine if we have one to invoke. This is usually injected
            ' into the class instance by the programmer who's using this class in his/her program.
            If checksumStatusUpdaterThread Is Nothing Then
                checksumStatusUpdaterThread = New Threading.Thread(AddressOf checksumStatusUpdaterThreadSubroutine) With {
                    .IsBackground = True,
                    .Priority = Threading.ThreadPriority.Lowest,
                    .Name = "Checksum Class Status Updating Thread"
                }
                checksumStatusUpdaterThread.Start()
            End If
        End If
    End Sub

    Public Function MD5() As String
        checksumStatusUpdateInvoker()
        Dim MD5Engine As New Security.Cryptography.MD5CryptoServiceProvider
        Dim Output As Byte() = MD5Engine.ComputeHash(fileStream)
        checksumStatusUpdaterThread.Abort()
        Return BitConverter.ToString(Output).ToLower().Replace("-", "")
    End Function

    Public Function SHA160() As String
        checksumStatusUpdateInvoker()
        Dim SHA1Engine As New Security.Cryptography.SHA1CryptoServiceProvider
        Dim Output As Byte() = SHA1Engine.ComputeHash(fileStream)
        checksumStatusUpdaterThread.Abort()
        Return BitConverter.ToString(Output).ToLower().Replace("-", "")
    End Function

    Public Function SHA256() As String
        checksumStatusUpdateInvoker()
        Dim SHA1Engine As New Security.Cryptography.SHA256Managed
        Dim Output As Byte() = SHA1Engine.ComputeHash(fileStream)
        checksumStatusUpdaterThread.Abort()
        Return BitConverter.ToString(Output).ToLower().Replace("-", "")
    End Function

    Public Function SHA384() As String
        checksumStatusUpdateInvoker()
        Dim SHA1Engine As New Security.Cryptography.SHA384Managed
        Dim Output As Byte() = SHA1Engine.ComputeHash(fileStream)
        checksumStatusUpdaterThread.Abort()
        Return BitConverter.ToString(Output).ToLower().Replace("-", "")
    End Function

    Public Function SHA512() As String
        checksumStatusUpdateInvoker()
        Dim SHA1Engine As New Security.Cryptography.SHA512Managed
        Dim Output As Byte() = SHA1Engine.ComputeHash(fileStream)
        checksumStatusUpdaterThread.Abort()
        Return BitConverter.ToString(Output).ToLower().Replace("-", "")
    End Function

    Public Function RIPEMD160() As String
        checksumStatusUpdateInvoker()
        Dim SHA1Engine As New Security.Cryptography.RIPEMD160Managed
        Dim Output As Byte() = SHA1Engine.ComputeHash(fileStream)
        checksumStatusUpdaterThread.Abort()
        Return BitConverter.ToString(Output).ToLower().Replace("-", "")
    End Function

    Public Shared Function MD5String(inputString As String) As String
        Dim MD5Engine As New Security.Cryptography.MD5CryptoServiceProvider
        Dim Output As Byte() = MD5Engine.ComputeHash(Text.Encoding.UTF8.GetBytes(inputString))
        Return BitConverter.ToString(Output).ToLower().Replace("-", "")
    End Function

    Public Shared Function SHA160String(inputString As String) As String
        Dim SHA1Engine As New Security.Cryptography.SHA1CryptoServiceProvider
        Dim Output As Byte() = SHA1Engine.ComputeHash(Text.Encoding.UTF8.GetBytes(inputString))
        Return BitConverter.ToString(Output).ToLower().Replace("-", "")
    End Function

    Public Shared Function SHA256String(inputString As String) As String
        Dim SHA1Engine As New Security.Cryptography.SHA256Managed
        Dim Output As Byte() = SHA1Engine.ComputeHash(Text.Encoding.UTF8.GetBytes(inputString))
        Return BitConverter.ToString(Output).ToLower().Replace("-", "")
    End Function

    Public Shared Function SHA384String(inputString As String) As String
        Dim SHA1Engine As New Security.Cryptography.SHA384Managed
        Dim Output As Byte() = SHA1Engine.ComputeHash(Text.Encoding.UTF8.GetBytes(inputString))
        Return BitConverter.ToString(Output).ToLower().Replace("-", "")
    End Function

    Public Shared Function SHA512String(inputString As String) As String
        Dim SHA1Engine As New Security.Cryptography.SHA512Managed
        Dim Output As Byte() = SHA1Engine.ComputeHash(Text.Encoding.UTF8.GetBytes(inputString))
        Return BitConverter.ToString(Output).ToLower().Replace("-", "")
    End Function
End Class