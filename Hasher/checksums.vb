Public Class checksums
    Private checksumStatusUpdater As [Delegate]

    ''' <summary>This allows you to set up a function to be run while your checksum is being processed. This function can be used to update things on the GUI during a checksum.</summary>
    ''' <example>
    ''' A VB.NET Example...
    ''' Dim checksums As New checksum(Function(ByVal checksumStatusDetails As checksumStatusDetails)
    ''' End Function)
    ''' OR A C# Example...
    ''' checksum checksums = new checksum((checksumStatusDetails checksumStatusDetails) => { });
    ''' </example>
    Public Sub New(ByRef inputDelegate As [Delegate])
        checksumStatusUpdater = inputDelegate
    End Sub

    Public Enum checksumType As Short
        md5
        sha160
        sha256
        sha384
        sha512
    End Enum

    Public Shared Function getHashEngine(hashType As checksumType) As Security.Cryptography.HashAlgorithm
        If hashType = checksumType.md5 Then
            Return New Security.Cryptography.MD5CryptoServiceProvider
        ElseIf hashType = checksumType.sha160 Then
            Return New Security.Cryptography.SHA1CryptoServiceProvider
        ElseIf hashType = checksumType.sha256 Then
            Return New Security.Cryptography.SHA256CryptoServiceProvider
        ElseIf hashType = checksumType.sha384 Then
            Return New Security.Cryptography.SHA384CryptoServiceProvider
        ElseIf hashType = checksumType.sha512 Then
            Return New Security.Cryptography.SHA512CryptoServiceProvider
        Else
            Return New Security.Cryptography.SHA256CryptoServiceProvider
        End If
    End Function

    Private Function getETATime(ByVal sw As Stopwatch, ByVal counter As Long, ByVal counterGoal As Long) As TimeSpan
        If Not My.Settings.boolShowEstimatedTime Then Return TimeSpan.Zero
        If counter = 0 Then Return TimeSpan.Zero
        Dim elapsedMin As Single = CSng(sw.ElapsedMilliseconds) / 1000 / 60
        Dim minLeft As Single = (elapsedMin / counter) * (counterGoal - counter)
        Return TimeSpan.FromMinutes(minLeft)
    End Function

    Public Function performFileHash(strFileName As String, intBufferSize As Integer, hashType As checksumType) As String
        Dim HashAlgorithm As Security.Cryptography.HashAlgorithm = getHashEngine(hashType)
        Dim byteReadAheadDataBuffer As Byte(), byteDataBuffer As Byte()
        Dim intReadAheadBytesRead As Integer, intBytesRead As Integer
        Dim longFileSize As Long, longTotalBytesRead As Long = 0
        Dim stopwatch As Stopwatch = Stopwatch.StartNew

        Using fileStream As New IO.FileStream(strFileName, IO.FileMode.Open, IO.FileAccess.Read, IO.FileShare.Read, intBufferSize, IO.FileOptions.SequentialScan)
            longFileSize = fileStream.Length
            byteReadAheadDataBuffer = New Byte(intBufferSize - 1) {}
            intReadAheadBytesRead = fileStream.Read(byteReadAheadDataBuffer, 0, byteReadAheadDataBuffer.Length)
            longTotalBytesRead += intReadAheadBytesRead

            Do
                intBytesRead = intReadAheadBytesRead
                byteDataBuffer = byteReadAheadDataBuffer

                byteReadAheadDataBuffer = New Byte(intBufferSize - 1) {}
                intReadAheadBytesRead = fileStream.Read(byteReadAheadDataBuffer, 0, byteReadAheadDataBuffer.Length)
                longTotalBytesRead += intReadAheadBytesRead

                If intReadAheadBytesRead = 0 Then
                    HashAlgorithm.TransformFinalBlock(byteDataBuffer, 0, intBytesRead)
                Else
                    HashAlgorithm.TransformBlock(byteDataBuffer, 0, intBytesRead, byteDataBuffer, 0)
                End If

                checksumStatusUpdater.DynamicInvoke(longFileSize, longTotalBytesRead, getETATime(stopwatch, longTotalBytesRead, longFileSize))
            Loop While intReadAheadBytesRead <> 0
        End Using

        Return BitConverter.ToString(HashAlgorithm.Hash).ToLower().Replace("-", "")
    End Function
End Class