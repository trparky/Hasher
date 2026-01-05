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

    Public Function PerformFileHash(strFileName As String, intBufferSize As Integer, hashType As HashAlgorithmName) As String
        ' Get file size and adjust buffer size accordingly
        Dim longFileSize As Long = New IO.FileInfo(strFileName).Length
        intBufferSize = If(longFileSize = 0, 1, Math.Min(intBufferSize, longFileSize))

        ' Initialize byte buffer and variables for tracking progress
        Dim byteDataBuffer As Byte() = localPool.Rent(intBufferSize)
        Dim longTotalBytesRead As Long = 0
        Dim intBytesRead As Integer
        Dim lastUpdate As Long = Environment.TickCount
        Dim hashEngine As HashAlgorithm

        If hashType = HashAlgorithmName.MD5 Then
            hashEngine = New MD5Cng()
        ElseIf hashType = HashAlgorithmName.SHA1 Then
            hashEngine = New SHA1Cng()
        ElseIf hashType = HashAlgorithmName.SHA256 Then
            hashEngine = New SHA256Cng()
        ElseIf hashType = HashAlgorithmName.SHA384 Then
            hashEngine = New SHA384Cng()
        ElseIf hashType = HashAlgorithmName.SHA512 Then
            hashEngine = New SHA512Cng()
        Else
            hashEngine = New SHA256Cng()
        End If

        ' Open file for reading
        Using stream As New IO.FileStream(strFileName, IO.FileMode.Open, IO.FileAccess.Read, IO.FileShare.Read, intBufferSize, IO.FileOptions.SequentialScan)
            ' Read data from file in chunks and update hash engines
            While True
                ' Check for thread abort
                If boolAbortThread Then Throw New MyThreadAbortException()

                intBytesRead = stream.Read(byteDataBuffer, 0, byteDataBuffer.Length)

                If intBytesRead <= 0 Then Exit While

                hashEngine.TransformBlock(byteDataBuffer, 0, intBytesRead, Nothing, 0)

                longTotalBytesRead += intBytesRead
                Threading.Interlocked.Add(longAllReadBytes, intBytesRead)

                If Environment.TickCount - lastUpdate >= 500 Then ' every 500 ms
                    lastUpdate = Environment.TickCount
                    checksumStatusUpdater(longFileSize, longTotalBytesRead)
                End If
            End While

            checksumStatusUpdater(longFileSize, longTotalBytesRead)

            localPool.Return(byteDataBuffer, clearArray:=False)

            hashEngine.TransformFinalBlock(Array.Empty(Of Byte)(), 0, 0)
            Dim strHash As String = BytesToHex(hashEngine.Hash)
            hashEngine.dispose()

            Return strHash
        End Using
    End Function

    Public Shared Function BytesToHex(bytes As Byte()) As String
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

' Strongly typed delegate
Public Delegate Sub ChecksumStatusUpdaterDelegate(fileSize As Long, bytesRead As Long)