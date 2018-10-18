Imports System.Runtime.InteropServices

Namespace NativeMethod
    Friend Class NativeMethods
        Public Const BCM_FIRST As Integer = &H1600
        Public Const BCM_SETSHIELD As Integer = (BCM_FIRST + &HC)

        <DllImport("user32.dll", EntryPoint:="SendMessageW")>
        Public Shared Function SendMessage(ByVal hWnd As IntPtr, ByVal Msg As UInteger, ByVal wParam As IntPtr, ByVal lParam As IntPtr) As Integer
        End Function
    End Class
End Namespace