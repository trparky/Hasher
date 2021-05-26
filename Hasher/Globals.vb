Imports System.Security.Principal

Public Module Globals
    ''' <summary>These two variables, longAllReadBytes and longAllBytes, and used to track overall hashing progress of all files to be processed.</summary>
    Public longAllReadBytes, longAllBytes As Long
    ''' <summary>Protects longAllReadBytes and longAllBytes from being accessed by two threads at the same time. Use this with a SyncLock code block.</summary>
    Public ReadOnly threadLockingObject As New Object()
    ''' <summary>Stores the value of the setting for roundNumbers locally.</summary>
    Public byteRoundFileSizes, byteRoundPercentages As Byte
    Public Const DoubleCRLF As String = vbCrLf & vbCrLf

    Public Function MyRoundingFunction(value As Double, digits As Integer) As String
        If digits = 0 Then
            Return Math.Round(value, digits).ToString
        Else
            Dim strFormatString As String = "{0:0." & New String("0", digits) & "}"
            Return String.Format(strFormatString, Math.Round(value, digits))
        End If
    End Function

    Public Function FileSizeToHumanSize(ByVal size As Long, Optional roundToNearestWholeNumber As Boolean = False) As String
        Dim result As String
        Dim shortRoundNumber As Short = If(roundToNearestWholeNumber, 0, byteRoundFileSizes)

        If size <= (2 ^ 10) Then
            result = size & " Bytes"
        ElseIf size > (2 ^ 10) And size <= (2 ^ 20) Then
            result = MyRoundingFunction(size / (2 ^ 10), shortRoundNumber) & " KBs"
        ElseIf size > (2 ^ 20) And size <= (2 ^ 30) Then
            result = MyRoundingFunction(size / (2 ^ 20), shortRoundNumber) & " MBs"
        ElseIf size > (2 ^ 30) And size <= (2 ^ 40) Then
            result = MyRoundingFunction(size / (2 ^ 30), shortRoundNumber) & " GBs"
        ElseIf size > (2 ^ 40) And size <= (2 ^ 50) Then
            result = MyRoundingFunction(size / (2 ^ 40), shortRoundNumber) & " TBs"
        ElseIf size > (2 ^ 50) And size <= (2 ^ 60) Then
            result = MyRoundingFunction(size / (2 ^ 50), shortRoundNumber) & " PBs"
        ElseIf size > (2 ^ 60) And size <= (2 ^ 70) Then
            result = MyRoundingFunction(size / (2 ^ 50), shortRoundNumber) & " EBs"
        Else
            result = "(None)"
        End If

        Return result
    End Function

    Public Function TimespanToHMS(timeSpan As TimeSpan) As String
        If timeSpan.TotalMilliseconds < 1000 Then
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

    Public Function AreWeAnAdministrator() As Boolean
        Try
            Return New WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator)
        Catch ex As Exception
            Return False
        End Try
    End Function
End Module