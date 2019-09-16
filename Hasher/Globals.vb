Imports System.Security.Principal

Public Module Globals
    ''' <summary>These two variables, ulongAllReadBytes and ulongAllBytes, and used to track overall hashing progress of all files to be processed.</summary>
    Public ulongAllReadBytes, ulongAllBytes As ULong
    ''' <summary>Protects ulongAllReadBytes and ulongAllBytes from being accessed by two threads at the same time. Use this with a SyncLock code block.</summary>
    Public ReadOnly threadLockingObject As New Object()

    Public Function fileSizeToHumanSize(ByVal size As Long, Optional roundToNearestWholeNumber As Boolean = False) As String
        Dim result As String
        Dim shortRoundNumber As Short = If(roundToNearestWholeNumber, 0, 2)

        If size <= (2 ^ 10) Then
            result = size & " Bytes"
        ElseIf size > (2 ^ 10) And size <= (2 ^ 20) Then
            result = Math.Round(size / (2 ^ 10), shortRoundNumber) & " KBs"
        ElseIf size > (2 ^ 20) And size <= (2 ^ 30) Then
            result = Math.Round(size / (2 ^ 20), shortRoundNumber) & " MBs"
        ElseIf size > (2 ^ 30) And size <= (2 ^ 40) Then
            result = Math.Round(size / (2 ^ 30), shortRoundNumber) & " GBs"
        ElseIf size > (2 ^ 40) And size <= (2 ^ 50) Then
            result = Math.Round(size / (2 ^ 40), shortRoundNumber) & " TBs"
        ElseIf size > (2 ^ 50) And size <= (2 ^ 60) Then
            result = Math.Round(size / (2 ^ 50), shortRoundNumber) & " PBs"
        ElseIf size > (2 ^ 60) And size <= (2 ^ 70) Then
            result = Math.Round(size / (2 ^ 50), shortRoundNumber) & " EBs"
        Else
            result = "(None)"
        End If

        Return result
    End Function

    Public Function timespanToHMS(timeSpan As TimeSpan) As String
        If timeSpan.TotalMilliseconds < 1000 Or timeSpan.Seconds = 0 Then
            If My.Settings.boolUseMilliseconds Then
                Return Math.Round(timeSpan.TotalMilliseconds, 2) & "ms (less than one second)"
            Else
                Return Math.Round(timeSpan.TotalMilliseconds / 1000, 2) & " seconds"
            End If
        End If

        Dim strReturnedString As String = Nothing

        If timeSpan.Hours <> 0 Then strReturnedString = timeSpan.Hours & If(timeSpan.Hours = 1, " Hour", " Hours")
        If timeSpan.Minutes <> 0 Then
            If String.IsNullOrWhiteSpace(strReturnedString) Then
                strReturnedString = timeSpan.Minutes & If(timeSpan.Minutes = 1, " Minute", " Minutes")
            Else
                strReturnedString &= ", " & timeSpan.Minutes & If(timeSpan.Minutes = 1, " Minute", " Minutes")
            End If
        End If
        If timeSpan.Seconds <> 0 Then
            If String.IsNullOrWhiteSpace(strReturnedString) Then
                strReturnedString = timeSpan.Seconds & If(timeSpan.Seconds = 1, " Second", " Seconds")
            Else
                strReturnedString &= " and " & timeSpan.Seconds & If(timeSpan.Seconds = 1, " Second", " Seconds")
            End If
        End If

        Return strReturnedString
    End Function

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

    Private Sub killProcess(processID As Integer)
        Dim processObject As Process = Nothing

        ' First we are going to check if the Process ID exists.
        If doesProcessIDExist(processID, processObject) Then
            Try
                processObject.Kill() ' Yes, it does so let's kill it.
            Catch ex As Exception
                ' Wow, it seems that even with double-checking if a process exists by it's PID number things can still go wrong.
                ' So this Try-Catch block is here to trap any possible errors when trying to kill a process by it's PID number.
            End Try
        End If

        Threading.Thread.Sleep(250) ' We're going to sleep to give the system some time to kill the process.

        '' Now we are going to check again if the Process ID exists and if it does, we're going to attempt to kill it again.
        If doesProcessIDExist(processID, processObject) Then
            Try
                processObject.Kill()
            Catch ex As Exception
                ' Wow, it seems that even with double-checking if a process exists by it's PID number things can still go wrong.
                ' So this Try-Catch block is here to trap any possible errors when trying to kill a process by it's PID number.
            End Try
        End If

        Threading.Thread.Sleep(250) ' We're going to sleep (again) to give the system some time to kill the process.
    End Sub

    Private Function getProcessExecutablePath(processID As Integer) As String
        Dim memoryBuffer As New Text.StringBuilder(1024)
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

                    If boolFullFilePathPassed Then
                        If strFileName.Equals(processExecutablePathFileInfo.FullName, StringComparison.OrdinalIgnoreCase) Then
                            killProcess(process.Id)
                        End If
                    Else
                        If strFileName.Equals(processExecutablePathFileInfo.Name, StringComparison.OrdinalIgnoreCase) Then
                            killProcess(process.Id)
                        End If
                    End If
                Catch ex As ArgumentException
                End Try
            End If
        Next
    End Sub

    Public Function areWeAnAdministrator() As Boolean
        Try
            Return New WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator)
        Catch ex As Exception
            Return False
        End Try
    End Function
End Module