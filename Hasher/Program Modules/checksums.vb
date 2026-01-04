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
            Using md5Engine As New MD5Cng(), sha160Engine As New SHA1Cng(), sha256Engine As New SHA256Cng(), sha384Engine As New SHA384Cng(), sha512Engine As New SHA512Cng()
                ' Read data from file in chunks and update hash engines
                While True
                    ' Check for thread abort
                    If boolAbortThread Then Throw New MyThreadAbortException()

                    intBytesRead = stream.Read(byteDataBuffer, 0, byteDataBuffer.Length)

                    If intBytesRead <= 0 Then Exit While

                    md5Engine.TransformBlock(byteDataBuffer, 0, intBytesRead, Nothing, 0)
                    sha160Engine.TransformBlock(byteDataBuffer, 0, intBytesRead, Nothing, 0)
                    sha256Engine.TransformBlock(byteDataBuffer, 0, intBytesRead, Nothing, 0)
                    sha384Engine.TransformBlock(byteDataBuffer, 0, intBytesRead, Nothing, 0)
                    sha512Engine.TransformBlock(byteDataBuffer, 0, intBytesRead, Nothing, 0)

                    longTotalBytesRead += intBytesRead
                    Threading.Interlocked.Add(longAllReadBytes, intBytesRead)

                    If Environment.TickCount - lastUpdate >= 500 Then ' every 500 ms
                        lastUpdate = Environment.TickCount
                        checksumStatusUpdater(longFileSize, longTotalBytesRead)
                    End If
                End While

                checksumStatusUpdater(longFileSize, longTotalBytesRead)

                localPool.Return(byteDataBuffer, clearArray:=False)

                md5Engine.TransformFinalBlock(Array.Empty(Of Byte)(), 0, 0)
                sha160Engine.TransformFinalBlock(Array.Empty(Of Byte)(), 0, 0)
                sha256Engine.TransformFinalBlock(Array.Empty(Of Byte)(), 0, 0)
                sha384Engine.TransformFinalBlock(Array.Empty(Of Byte)(), 0, 0)
                sha512Engine.TransformFinalBlock(Array.Empty(Of Byte)(), 0, 0)

                Return New AllTheHashes With {
                    .Md5 = BytesToHex(md5Engine.Hash),
                    .Sha160 = BytesToHex(sha160Engine.Hash),
                    .Sha256 = BytesToHex(sha256Engine.Hash),
                    .Sha384 = BytesToHex(sha384Engine.Hash),
                    .Sha512 = BytesToHex(sha512Engine.Hash)
                }
            End Using
        End Using
    End Function

    Private Function BytesToHex(bytes As Byte()) As String
        Dim chars(bytes.Length * 2 - 1) As Char
        Dim idx As Integer = 0

        For Each b In bytes
            Dim hi = b >> 4
            Dim lo = b And &HF

            chars(idx) = ChrW(If(hi < 10, 48 + hi, 87 + hi))
            idx += 1
            chars(idx) = ChrW(If(lo < 10, 48 + lo, 87 + lo))
            idx += 1
        Next

        Return New String(chars)
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