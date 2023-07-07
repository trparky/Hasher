Imports System.Security.Cryptography

Public Class Checksums
    Private ReadOnly checksumStatusUpdater As [Delegate]

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

    Public Function PerformFileHash(strFileName As String, intBufferSize As Integer) As AllTheHashes
        ' Declare some variables.
        Dim byteDataBuffer As Byte()
        Dim intBytesRead As Integer
        Dim longTotalBytesRead As Long = 0
        Dim longFileSize As Long = New IO.FileInfo(strFileName).Length ' Get the size of the file.

        If longFileSize = 0 Then
            intBufferSize = 1 ' Even though the size of the file is 0 bytes (empty), we still need a data buffer size of 1 byte.
        ElseIf longFileSize < intBufferSize Then
            ' In this case, why have a data buffer size that's larger than the file itself. It doesn't make
            ' sense. So here we set the data buffer size to the size of the file that's being read to make
            ' it so that we don't have to read any more data from the drive than we have to.
            intBufferSize = longFileSize
        End If

        ' Open the file for reading.
        Using stream As New IO.FileStream(strFileName, IO.FileMode.Open, IO.FileAccess.Read, IO.FileShare.Read, intBufferSize, IO.FileOptions.SequentialScan)
            If boolAbortThread Then Throw New MyThreadAbortException
            byteDataBuffer = New Byte(intBufferSize - 1) {} ' Create a data buffer in system memory to store some data.
            intBytesRead = stream.Read(byteDataBuffer, 0, byteDataBuffer.Length) ' Read some data from disk into the above data buffer.
            SyncLock threadLockingObject
                longAllReadBytes += intBytesRead
            End SyncLock
            longTotalBytesRead += intBytesRead ' Increment the amount of data that we've read by the amount we read above.

            ' Call the status updating delegate.
            checksumStatusUpdater.DynamicInvoke(longFileSize, longTotalBytesRead)

            Dim md5Engine As HashAlgorithm = New MD5CryptoServiceProvider
            Dim sha160Engine As HashAlgorithm = New SHA1CryptoServiceProvider
            Dim sha256Engine As HashAlgorithm = New SHA256CryptoServiceProvider
            Dim sha384Engine As HashAlgorithm = New SHA384CryptoServiceProvider
            Dim sha512Engine As HashAlgorithm = New SHA512CryptoServiceProvider

            ' We're going to loop until all the data for the file we're processing has been read from disk.
            Do While longTotalBytesRead < longFileSize
                If boolAbortThread Then
                    stream.Close()
                    stream.Dispose()
                    Throw New MyThreadAbortException()
                End If

                ' Add the data that we've read from disk into the hasher function.
                md5Engine.TransformBlock(byteDataBuffer, 0, intBytesRead, byteDataBuffer, 0)
                sha160Engine.TransformBlock(byteDataBuffer, 0, intBytesRead, byteDataBuffer, 0)
                sha256Engine.TransformBlock(byteDataBuffer, 0, intBytesRead, byteDataBuffer, 0)
                sha384Engine.TransformBlock(byteDataBuffer, 0, intBytesRead, byteDataBuffer, 0)
                sha512Engine.TransformBlock(byteDataBuffer, 0, intBytesRead, byteDataBuffer, 0)

                Array.Clear(byteDataBuffer, 0, byteDataBuffer.Length) ' Clear the Byte Array.
                intBytesRead = stream.Read(byteDataBuffer, 0, byteDataBuffer.Length) ' Read some data from disk into the data buffer that was created above.
                SyncLock threadLockingObject
                    longAllReadBytes += intBytesRead
                End SyncLock
                longTotalBytesRead += intBytesRead ' Increment the amount of data that we've read by the amount we read above.

                ' This sub-routine call is to help de-duplicate code.
                ' Call the status updating delegate.
                checksumStatusUpdater.DynamicInvoke(longFileSize, longTotalBytesRead)
            Loop

            ' We're done reading the file, we now need to finalize the data in the hasher function.
            md5Engine.TransformFinalBlock(byteDataBuffer, 0, intBytesRead)
            sha160Engine.TransformFinalBlock(byteDataBuffer, 0, intBytesRead)
            sha256Engine.TransformFinalBlock(byteDataBuffer, 0, intBytesRead)
            sha384Engine.TransformFinalBlock(byteDataBuffer, 0, intBytesRead)
            sha512Engine.TransformFinalBlock(byteDataBuffer, 0, intBytesRead)

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

            Array.Clear(byteDataBuffer, 0, byteDataBuffer.Length) ' Clear the Byte Array.
            Return allTheHashes
        End Using
    End Function
End Class

Public Structure AllTheHashes
    Public Property Md5 As String
    Public Property Sha160 As String
    Public Property Sha256 As String
    Public Property Sha384 As String
    Public Property Sha512 As String
End Structure