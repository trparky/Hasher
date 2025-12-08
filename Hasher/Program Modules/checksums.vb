Imports System.Buffers
Imports System.Security.Cryptography

Public Class Checksums
    Private ReadOnly checksumStatusUpdater As ChecksumStatusUpdaterDelegate
    Public localPool As ArrayPool(Of Byte)
    Private Const updateStep As Long = 1024L * 1024L ' 1 MB

    ''' <summary>
    ''' This allows you to set up a function to be run while your checksum is being processed. This function can be used to update things on the GUI during a checksum.
    ''' </summary>
    ''' <example>
    ''' A VB.NET Example...
    ''' Dim checksums As New Checksums(Sub(ByVal checksumStatusDetails As checksumStatusDetails)
    ''' End Sub)
    ''' OR A C# Example...
    ''' Checksums checksums = new Checksums((checksumStatusDetails checksumStatusDetails) => { });
    ''' </example>
    Public Sub New(inputDelegate As ChecksumStatusUpdaterDelegate, ByRef pool As ArrayPool(Of Byte))
        localPool = pool
        checksumStatusUpdater = inputDelegate
    End Sub

    Public Function PerformFileHash(strFileName As String, intBufferSize As Integer) As AllTheHashes
        ' Get file size and adjust buffer size accordingly
        Dim longFileSize As Long = New IO.FileInfo(strFileName).Length
        intBufferSize = If(longFileSize = 0, 1, Math.Min(intBufferSize, longFileSize))

        ' Initialize byte buffer and variables for tracking progress
        Dim byteDataBuffer As Byte() = localPool.Rent(intBufferSize)
        Dim longTotalBytesRead As Long = 0
        Dim intBytesRead As Integer
        Dim lastUpdate As Long = Environment.TickCount

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
                    Threading.Interlocked.Add(longTotalBytesRead, intBytesRead)
                    Threading.Interlocked.Add(longAllReadBytes, intBytesRead)

                    If Environment.TickCount - lastUpdate >= 500 Then ' every 500 ms
                        lastUpdate = Environment.TickCount
                        checksumStatusUpdater(longFileSize, longTotalBytesRead)
                    End If

                    ' Check for thread abort
                    If boolAbortThread Then Throw New MyThreadAbortException()
                Loop

                checksumStatusUpdater(longFileSize, longTotalBytesRead)

                ' Finalize hash calculations
                md5Engine.TransformFinalBlock(byteDataBuffer, 0, 0)
                sha160Engine.TransformFinalBlock(byteDataBuffer, 0, 0)
                sha256Engine.TransformFinalBlock(byteDataBuffer, 0, 0)
                sha384Engine.TransformFinalBlock(byteDataBuffer, 0, 0)
                sha512Engine.TransformFinalBlock(byteDataBuffer, 0, 0)

                localPool.Return(byteDataBuffer, clearArray:=True)

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

' Strongly typed delegate
Public Delegate Sub ChecksumStatusUpdaterDelegate(fileSize As Long, bytesRead As Long)