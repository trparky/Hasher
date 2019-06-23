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

    Private Shared Function getETATime(ByVal sw As Stopwatch, ByVal counter As Long, ByVal counterGoal As Long) As TimeSpan
        If counter = 0 Then Return TimeSpan.Zero
        Dim elapsedMin As Single = CSng(sw.ElapsedMilliseconds) / 1000 / 60
        Dim minLeft As Single = (elapsedMin / counter) * (counterGoal - counter)
        Return TimeSpan.FromMinutes(minLeft)
    End Function

    Public Function performFileHash(strFileName As String, intBufferSize As Integer, hashType As checksumType, boolShowEstimatedTime As Boolean) As String
        Dim HashAlgorithm As Security.Cryptography.HashAlgorithm = getHashEngine(hashType) ' Get our hashing engine.

        ' Declare some variables.
        Dim byteDataBuffer As Byte()
        Dim intBytesRead As Integer
        Dim longFileSize As Long, longTotalBytesRead As Long = 0

        Dim stopwatch As Stopwatch = Stopwatch.StartNew ' Create a stopwatch for compute timing.

        ' Open the file for reading.
        Using stream As New IO.FileStream(strFileName, IO.FileMode.Open, IO.FileAccess.Read, IO.FileShare.Read, intBufferSize, IO.FileOptions.SequentialScan)
            longFileSize = stream.Length ' Get the size of the file.
            byteDataBuffer = New Byte(intBufferSize - 1) {} ' Create a data buffer in system memory to store some data.
            intBytesRead = stream.Read(byteDataBuffer, 0, byteDataBuffer.Length) ' Read some data from disk into the above data buffer.
            longTotalBytesRead += intBytesRead ' Increment the amount of data that we've read by the amount we read above.

            ' This sub-routine call is to help de-duplicate code.
            callChecksumStatusUpdatedPluginCode(boolShowEstimatedTime, longFileSize, longTotalBytesRead, stopwatch)

            ' We're going to loop until all the data for the file we're processing has been read from disk.
            Do While longTotalBytesRead < longFileSize
                ' Add the data that we've read from disk into the hasher function.
                HashAlgorithm.TransformBlock(byteDataBuffer, 0, intBytesRead, byteDataBuffer, 0)

                Array.Clear(byteDataBuffer, 0, byteDataBuffer.Length) ' Clear the Byte Array.
                intBytesRead = stream.Read(byteDataBuffer, 0, byteDataBuffer.Length) ' Read some data from disk into the data buffer that was created above.
                longTotalBytesRead += intBytesRead ' Increment the amount of data that we've read by the amount we read above.

                ' This sub-routine call is to help de-duplicate code.
                callChecksumStatusUpdatedPluginCode(boolShowEstimatedTime, longFileSize, longTotalBytesRead, stopwatch)
            Loop

            ' We're done reading the file, we now need to finalize the data in the hasher function.
            HashAlgorithm.TransformFinalBlock(byteDataBuffer, 0, intBytesRead)
        End Using

        Return BitConverter.ToString(HashAlgorithm.Hash).ToLower().Replace("-", "") ' Return the hash string.
    End Function

    ' This sub-routine call is to help de-duplicate code. And yes, we use ByRef variables here to reduce data copying performance issues.
    Private Sub callChecksumStatusUpdatedPluginCode(ByRef boolShowEstimatedTime As Boolean, ByRef longFileSize As Long, ByRef longTotalBytesRead As Long, ByRef stopwatch As Stopwatch)
        ' We do a check to see if the user wants the estimated time to be shown, if not we don't call the getETATime()
        ' function at all and pass a TimeSpan.Zero as part of the plugged in function call below.
        If boolShowEstimatedTime Then
            ' Update the status on the GUI.
            checksumStatusUpdater.DynamicInvoke(longFileSize, longTotalBytesRead, getETATime(stopwatch, longTotalBytesRead, longFileSize))
        Else
            ' Update the status on the GUI.
            checksumStatusUpdater.DynamicInvoke(longFileSize, longTotalBytesRead, TimeSpan.Zero)
        End If
    End Sub
End Class

Public Enum checksumType As Short
    md5
    sha160
    sha256
    sha384
    sha512
End Enum