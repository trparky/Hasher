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
        ' Get file size and adjust buffer size accordingly
        Dim longFileSize As Long = New IO.FileInfo(strFileName).Length
        intBufferSize = If(longFileSize = 0, 1, Math.Min(intBufferSize, longFileSize))

        ' Initialize byte buffer and variables for tracking progress
        Dim byteDataBuffer As Byte() = New Byte(intBufferSize - 1) {}
        Dim longTotalBytesRead As Long = 0
        Dim intBytesRead As Integer

        ' Open file for reading
        Using stream As New IO.FileStream(strFileName, IO.FileMode.Open, IO.FileAccess.Read, IO.FileShare.Read, intBufferSize, IO.FileOptions.SequentialScan)
            ' Using statements automatically handle disposal of hash engines
            Using md5Engine As New MD5CryptoServiceProvider(), sha160Engine As New SHA1CryptoServiceProvider(), sha256Engine As New SHA256CryptoServiceProvider(), sha384Engine As New SHA384CryptoServiceProvider(), sha512Engine As New SHA512CryptoServiceProvider()
                ' Read data from file in chunks and update hash engines
                Do
                    If boolAbortThread Then Throw New MyThreadAbortException
                    intBytesRead = stream.Read(byteDataBuffer, 0, byteDataBuffer.Length)
                    If intBytesRead <= 0 Then Exit Do

                    ' Update hash algorithms with read data
                    md5Engine.TransformBlock(byteDataBuffer, 0, intBytesRead, byteDataBuffer, 0)
                    sha160Engine.TransformBlock(byteDataBuffer, 0, intBytesRead, byteDataBuffer, 0)
                    sha256Engine.TransformBlock(byteDataBuffer, 0, intBytesRead, byteDataBuffer, 0)
                    sha384Engine.TransformBlock(byteDataBuffer, 0, intBytesRead, byteDataBuffer, 0)
                    sha512Engine.TransformBlock(byteDataBuffer, 0, intBytesRead, byteDataBuffer, 0)

                    ' Update progress
                    longTotalBytesRead += intBytesRead
                    SyncLock threadLockingObject
                        longAllReadBytes += intBytesRead
                    End SyncLock

                    ' Call the status updating delegate
                    checksumStatusUpdater.DynamicInvoke(longFileSize, longTotalBytesRead)

                    ' Check for thread abort
                    If boolAbortThread Then Throw New MyThreadAbortException()
                Loop

                ' Finalize hash calculations
                md5Engine.TransformFinalBlock(byteDataBuffer, 0, 0)
                sha160Engine.TransformFinalBlock(byteDataBuffer, 0, 0)
                sha256Engine.TransformFinalBlock(byteDataBuffer, 0, 0)
                sha384Engine.TransformFinalBlock(byteDataBuffer, 0, 0)
                sha512Engine.TransformFinalBlock(byteDataBuffer, 0, 0)

                ' Return the results in the AllTheHashes object
                Return New AllTheHashes With {
                    .Md5 = BitConverter.ToString(md5Engine.Hash).ToLower().Replace("-", ""),
                    .Sha160 = BitConverter.ToString(sha160Engine.Hash).ToLower().Replace("-", ""),
                    .Sha256 = BitConverter.ToString(sha256Engine.Hash).ToLower().Replace("-", ""),
                    .Sha384 = BitConverter.ToString(sha384Engine.Hash).ToLower().Replace("-", ""),
                    .Sha512 = BitConverter.ToString(sha512Engine.Hash).ToLower().Replace("-", "")
                }
            End Using
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