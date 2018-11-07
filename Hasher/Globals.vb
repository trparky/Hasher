Imports System.Security.Principal

Public Class communicationChannelClass
    Public strFileName As String
End Class

Public Module Globals
    Public Enum checksumType As Short
        md5
        sha160
        sha256
        sha384
        sha512
    End Enum

    ''' <summary>Checks to see if a Process ID or PID exists on the system.</summary>
    ''' <param name="PID">The PID of the process you are checking the existance of.</param>
    ''' <param name="processObject">If the PID does exist, the function writes back to this argument in a ByRef way a Process Object that can be interacted with outside of this function.</param>
    ''' <returns>Return a Boolean value. If the PID exists, it return a True value. If the PID doesn't exist, it returns a False value.</returns>
    Private Function doesProcessIDExist(ByVal PID As Integer, ByRef processObject As Process) As Boolean
        Try
            processObject = Process.GetProcessById(PID)
            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function

    Private Sub killProcess(processID As Integer, Optional boolLogToEventLog As Boolean = False)
        Dim processObject As Process = Nothing

        ' First we are going to check if the Process ID exists.
        If doesProcessIDExist(processID, processObject) = True Then
            Try
                processObject.Kill() ' Yes, it does so let's kill it.
            Catch ex As Exception
                ' Wow, it seems that even with double-checking if a process exists by it's PID number things can still go wrong.
                ' So this Try-Catch block is here to trap any possible errors when trying to kill a process by it's PID number.
            End Try
        End If

        processObject = Nothing
        Threading.Thread.Sleep(250) ' We're going to sleep to give the system some time to kill the process.

        '' Now we are going to check again if the Process ID exists and if it does, we're going to attempt to kill it again.
        If doesProcessIDExist(processID, processObject) = True Then
            Try
                processObject.Kill()
            Catch ex As Exception
                ' Wow, it seems that even with double-checking if a process exists by it's PID number things can still go wrong.
                ' So this Try-Catch block is here to trap any possible errors when trying to kill a process by it's PID number.
            End Try
        End If

        processObject = Nothing
        Threading.Thread.Sleep(250) ' We're going to sleep (again) to give the system some time to kill the process.
    End Sub

    Private Function getProcessExecutablePath(processID As Integer) As String
        Dim memoryBuffer = New Text.StringBuilder(1024)
        Dim processHandle As IntPtr = NativeMethod.NativeMethods.OpenProcess(NativeMethod.ProcessAccessFlags.PROCESS_QUERY_LIMITED_INFORMATION, False, processID)

        If processHandle <> IntPtr.Zero Then
            Try
                Dim memoryBufferSize As Integer = memoryBuffer.Capacity

                If NativeMethod.NativeMethods.QueryFullProcessImageName(processHandle, 0, memoryBuffer, memoryBufferSize) Then
                    Return memoryBuffer.ToString()
                End If
            Finally
                NativeMethod.NativeMethods.CloseHandle(processHandle)
            End Try
        End If

        NativeMethod.NativeMethods.CloseHandle(processHandle)
        Return Nothing
    End Function

    Public Sub searchForProcessAndKillIt(strFileName As String, boolFullFilePathPassed As Boolean)
        Dim processExecutablePath As String
        Dim processExecutablePathFileInfo As IO.FileInfo

        For Each process As Process In Process.GetProcesses()
            processExecutablePath = getProcessExecutablePath(process.Id)

            If processExecutablePath IsNot Nothing Then
                Try
                    processExecutablePathFileInfo = New IO.FileInfo(processExecutablePath)

                    If boolFullFilePathPassed = True Then
                        If strFileName.Equals(processExecutablePathFileInfo.FullName, StringComparison.OrdinalIgnoreCase) = True Then
                            killProcess(process.Id, True)
                        End If
                    ElseIf boolFullFilePathPassed = False Then
                        If strFileName.Equals(processExecutablePathFileInfo.Name, StringComparison.OrdinalIgnoreCase) = True Then
                            killProcess(process.Id, True)
                        End If
                    End If

                    processExecutablePathFileInfo = Nothing
                Catch ex As ArgumentException
                End Try
            End If

            processExecutablePath = Nothing
        Next
    End Sub

    Public Function areWeAnAdministrator() As Boolean
        Try
            Dim principal As WindowsPrincipal = New WindowsPrincipal(WindowsIdentity.GetCurrent())
            Return principal.IsInRole(WindowsBuiltInRole.Administrator)
        Catch ex As Exception
            Return False
        End Try
    End Function

    Public Function getHashEngine(hashType As checksumType) As Security.Cryptography.HashAlgorithm
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
End Module