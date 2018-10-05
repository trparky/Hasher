Public Class checksums
    Private checksumStatusUpdater As [Delegate]

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

    Public Function performFileHash(strFileName As String, intBufferSize As Integer, hashType As Form1.checksumType) As String
        Dim HashAlgorithm As Security.Cryptography.HashAlgorithm

        If hashType = Form1.checksumType.md5 Then
            HashAlgorithm = New Security.Cryptography.MD5CryptoServiceProvider
        ElseIf hashType = Form1.checksumType.sha160 Then
            HashAlgorithm = New Security.Cryptography.SHA1CryptoServiceProvider
        ElseIf hashType = Form1.checksumType.sha256 Then
            HashAlgorithm = New Security.Cryptography.SHA256CryptoServiceProvider
        ElseIf hashType = Form1.checksumType.sha384 Then
            HashAlgorithm = New Security.Cryptography.SHA384CryptoServiceProvider
        ElseIf hashType = Form1.checksumType.sha512 Then
            HashAlgorithm = New Security.Cryptography.SHA512CryptoServiceProvider
        End If

        Dim readAheadBuffer As Byte(), buffer As Byte()
        Dim readAheadBytesRead As Integer, bytesRead As Integer
        Dim size As Long, totalBytesRead As Long = 0

        Using stream As New IO.FileStream(strFileName, IO.FileMode.Open, IO.FileAccess.Read, IO.FileShare.Read, intBufferSize, IO.FileOptions.SequentialScan)
            size = stream.Length
            readAheadBuffer = New Byte(intBufferSize - 1) {}
            readAheadBytesRead = stream.Read(readAheadBuffer, 0, readAheadBuffer.Length)

            totalBytesRead += readAheadBytesRead

            Do
                bytesRead = readAheadBytesRead
                buffer = readAheadBuffer

                readAheadBuffer = New Byte(intBufferSize - 1) {}
                readAheadBytesRead = stream.Read(readAheadBuffer, 0, readAheadBuffer.Length)

                totalBytesRead += readAheadBytesRead

                If readAheadBytesRead = 0 Then
                    HashAlgorithm.TransformFinalBlock(buffer, 0, bytesRead)
                Else
                    HashAlgorithm.TransformBlock(buffer, 0, bytesRead, buffer, 0)
                End If

                checksumStatusUpdater.DynamicInvoke(size, totalBytesRead)
            Loop While readAheadBytesRead <> 0
        End Using

        Return BitConverter.ToString(HashAlgorithm.Hash).ToLower().Replace("-", "")
    End Function
End Class