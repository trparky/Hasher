Imports Microsoft.VisualBasic.ApplicationServices

Namespace My
    ' The following events are available for MyApplication:
    ' Startup: Raised when the application starts, before the startup form is created.
    ' Shutdown: Raised after all application forms are closed.  This event is not raised if the application terminates abnormally.
    ' UnhandledException: Raised if the application encounters an unhandled exception.
    ' StartupNextInstance: Raised when launching a single-instance application and the application is already active. 
    ' NetworkAvailabilityChanged: Raised when the network connection is connected or disconnected.
    Partial Friend Class MyApplication
        Private Sub MyApplication_Startup(sender As Object, e As StartupEventArgs) Handles Me.Startup
            If IO.File.Exists("updater.exe") Then
                SearchForProcessAndKillIt("updater.exe", False)
                IO.File.Delete("updater.exe")
            End If

            If Application.CommandLineArgs.Count = 1 Then
                Dim commandLineArgument As String = Application.CommandLineArgs(0).Trim

                If commandLineArgument.Equals("-removesystemlevelassociations", StringComparison.OrdinalIgnoreCase) Then
                    FileAssociation.DeleteSystemLevelFileAssociation()
                    FileAssociation.DeleteSystemLevelAssociationWithAllFiles()
                    Process.GetCurrentProcess.Kill()
                End If
            End If
        End Sub
    End Class
End Namespace