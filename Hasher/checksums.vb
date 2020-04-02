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

    Public Function performFileHash(strFileName As String, intBufferSize As Integer, hashType As checksumType) As String
        Using HashAlgorithm As Security.Cryptography.HashAlgorithm = getHashEngine(hashType) ' Get our hashing engine.
            ' Declare some variables.
            Dim byteDataBuffer As Byte()
            Dim intBytesRead As Integer
            Dim longFileSize As Long, longTotalBytesRead As ULong = 0

            ' Open the file for reading.
            Using stream As New IO.FileStream(strFileName, IO.FileMode.Open, IO.FileAccess.Read, IO.FileShare.Read, intBufferSize, IO.FileOptions.SequentialScan)
                longFileSize = stream.Length ' Get the size of the file.
                byteDataBuffer = New Byte(intBufferSize - 1) {} ' Create a data buffer in system memory to store some data.
                intBytesRead = stream.Read(byteDataBuffer, 0, byteDataBuffer.Length) ' Read some data from disk into the above data buffer.
                SyncLock threadLockingObject
                    ulongAllReadBytes += intBytesRead
                End SyncLock
                longTotalBytesRead += intBytesRead ' Increment the amount of data that we've read by the amount we read above.

                ' Call the status updating delegate.
                checksumStatusUpdater.DynamicInvoke(longFileSize, longTotalBytesRead)

                ' We're going to loop until all the data for the file we're processing has been read from disk.
                Do While longTotalBytesRead < longFileSize
                    ' Add the data that we've read from disk into the hasher function.
                    HashAlgorithm.TransformBlock(byteDataBuffer, 0, intBytesRead, byteDataBuffer, 0)

                    Array.Clear(byteDataBuffer, 0, byteDataBuffer.Length) ' Clear the Byte Array.
                    intBytesRead = stream.Read(byteDataBuffer, 0, byteDataBuffer.Length) ' Read some data from disk into the data buffer that was created above.
                    SyncLock threadLockingObject
                        ulongAllReadBytes += intBytesRead
                    End SyncLock
                    longTotalBytesRead += intBytesRead ' Increment the amount of data that we've read by the amount we read above.

                    ' This sub-routine call is to help de-duplicate code.
                    ' Call the status updating delegate.
                    checksumStatusUpdater.DynamicInvoke(longFileSize, longTotalBytesRead)
                Loop

                ' We're done reading the file, we now need to finalize the data in the hasher function.
                HashAlgorithm.TransformFinalBlock(byteDataBuffer, 0, intBytesRead)
            End Using

            Array.Clear(byteDataBuffer, 0, byteDataBuffer.Length) ' Clear the Byte Array.
            Return BitConverter.ToString(HashAlgorithm.Hash).ToLower().Replace("-", "") ' Return the hash string.
        End Using
    End Function
End Class

Public Enum checksumType As Short
    md5
    sha160
    sha256
    sha384
    sha512
End Enum