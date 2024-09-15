Imports System.Collections.ObjectModel
Imports System.Security.Principal

Public Module Globals
    ''' <summary>These two variables, longAllReadBytes and longAllBytes, and used to track overall hashing progress of all files to be processed.</summary>
    Public longAllReadBytes, longAllBytes As Long
    ''' <summary>Protects longAllReadBytes and longAllBytes from being accessed by two threads at the same time. Use this with a SyncLock code block.</summary>
    Public ReadOnly threadLockingObject As New Object()
    ''' <summary>Stores the value of the setting for roundNumbers locally.</summary>
    Public byteRoundFileSizes As Byte = My.Settings.roundFileSizes
    Public byteRoundPercentages As Byte = My.Settings.roundPercentages
    Public Const DoubleCRLF As String = vbCrLf & vbCrLf
    Public boolAbortThread As Boolean = False
    Public strEXEPath As String = Process.GetCurrentProcess.MainModule.FileName

    Public Function VerifyWindowLocation(point As Point, ByRef window As Form) As Point
        Dim screen As Screen = Screen.FromPoint(point) ' Get the screen based on the new window location

        Dim windowBounds As New Rectangle(point.X, point.Y, window.Width, window.Height)
        Dim screenBounds As Rectangle = screen.WorkingArea

        ' Ensure the window is at least partially on the screen
        If windowBounds.IntersectsWith(screenBounds) Then
            Return point
        Else
            ' Adjust the window to a default location if it is completely off-screen
            Return New Point(screenBounds.Left, screenBounds.Top)
        End If
    End Function

    Public Function GetGoodTextColorBasedUponBackgroundColor(input As Color) As Color
        Dim intCombinedTotal As Short = Integer.Parse(input.R.ToString) + Integer.Parse(input.G.ToString) + Integer.Parse(input.B.ToString)
        Return If((intCombinedTotal / 3) < 128, Color.White, Color.Black)
    End Function

    Public Function ParseArguments(args As ReadOnlyCollection(Of String)) As Dictionary(Of String, Object)
        Dim parsedArguments As New Dictionary(Of String, Object)(StringComparer.OrdinalIgnoreCase)
        Dim strValue As String

        For Each strArgument As String In args
            If strArgument.StartsWith("--") Then
                Dim splitArg As String() = strArgument.Substring(2).Split(New Char() {"="c}, 2)
                Dim key As String = splitArg(0)

                If splitArg.Length = 2 Then
                    ' Argument with a value
                    strValue = splitArg(1)
                    parsedArguments(key) = strValue
                Else
                    ' Boolean flag
                    parsedArguments(key) = True
                End If
            Else
                Console.WriteLine($"Unrecognized argument format: {strArgument}")
            End If
        Next

        Return parsedArguments
    End Function

    Public Sub SelectFileInWindowsExplorer(strFullPath As String)
        If Not String.IsNullOrEmpty(strFullPath) AndAlso IO.File.Exists(strFullPath) Then
            Dim pidlList As IntPtr = NativeMethod.NativeMethods.ILCreateFromPathW(strFullPath)

            If Not pidlList.Equals(IntPtr.Zero) Then
                Try
                    NativeMethod.NativeMethods.SHOpenFolderAndSelectItems(pidlList, 0, IntPtr.Zero, 0)
                Finally
                    NativeMethod.NativeMethods.ILFree(pidlList)
                End Try
            End If
        End If
    End Sub

    Public Function MyRoundingFunction(value As Double, digits As Integer) As String
        If digits < 0 Then Throw New ArgumentException("The number of digits must be non-negative.", NameOf(digits))

        If digits = 0 Then
            Return Math.Round(value, digits).ToString
        Else
            Return Math.Round(value, digits).ToString("0." & New String("0", digits))
        End If
    End Function

    Public Function FileSizeToHumanSize(size As Long, Optional roundToNearestWholeNumber As Boolean = False) As String
        Dim result As String
        Dim shortRoundNumber As Short = If(roundToNearestWholeNumber, 0, byteRoundFileSizes)

        If size <= (2 ^ 10) Then
            result = $"{size} Bytes"
        ElseIf size > (2 ^ 10) And size <= (2 ^ 20) Then
            result = $"{MyRoundingFunction(size / (2 ^ 10), shortRoundNumber)} KBs"
        ElseIf size > (2 ^ 20) And size <= (2 ^ 30) Then
            result = $"{MyRoundingFunction(size / (2 ^ 20), shortRoundNumber)} MBs"
        ElseIf size > (2 ^ 30) And size <= (2 ^ 40) Then
            result = $"{MyRoundingFunction(size / (2 ^ 30), shortRoundNumber)} GBs"
        ElseIf size > (2 ^ 40) And size <= (2 ^ 50) Then
            result = $"{MyRoundingFunction(size / (2 ^ 40), shortRoundNumber)} TBs"
        ElseIf size > (2 ^ 50) And size <= (2 ^ 60) Then
            result = $"{MyRoundingFunction(size / (2 ^ 50), shortRoundNumber)} PBs"
        ElseIf size > (2 ^ 60) And size <= (2 ^ 70) Then
            result = $"{MyRoundingFunction(size / (2 ^ 50), shortRoundNumber)} EBs"
        Else
            result = "(None)"
        End If

        Return result
    End Function

    Public Function TimespanToHMS(timeSpan As TimeSpan) As String
        If timeSpan.TotalMilliseconds < 1000 Then
            If My.Settings.boolUseMilliseconds Then
                Return $"{Math.Round(timeSpan.TotalMilliseconds, 2)}ms (less than one second)"
            Else
                Return $"{Math.Round(timeSpan.TotalMilliseconds / 1000, 2)} seconds"
            End If
        End If

        Dim strReturnedString As String = Nothing

        If timeSpan.Hours <> 0 Then strReturnedString = $"{timeSpan.Hours} {If(timeSpan.Hours = 1, "Hour", "Hours")}"
        If timeSpan.Minutes <> 0 Then
            If String.IsNullOrWhiteSpace(strReturnedString) Then
                strReturnedString = $"{timeSpan.Minutes} {If(timeSpan.Minutes = 1, "Minute", "Minutes")}"
            Else
                strReturnedString &= $", {timeSpan.Minutes} {If(timeSpan.Minutes = 1, "Minute", "Minutes")}"
            End If
        End If
        If timeSpan.Seconds <> 0 Then
            If String.IsNullOrWhiteSpace(strReturnedString) Then
                strReturnedString = $"{timeSpan.Seconds} {If(timeSpan.Seconds = 1, "Second", "Seconds")}"
            Else
                strReturnedString &= $" and {timeSpan.Seconds} {If(timeSpan.Seconds = 1, "Second", "Seconds")}"
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

Public Class MyThreadAbortException
    Inherits Exception

    Public Sub New()
    End Sub

    Public Sub New(message As String)
        MyBase.New(message)
    End Sub

    Public Sub New(message As String, inner As Exception)
        MyBase.New(message, inner)
    End Sub
End Class

Public Class ColumnOrder
    Public ColumnName As String
    Public ColumnIndex As Integer
End Class