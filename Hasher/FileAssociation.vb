Imports Microsoft.Win32

Class FileAssociation
    Public Enum KeyHiveSmall
        ClassesRoot
        CurrentUser
        LocalMachine
    End Enum

    Public Shared Sub CreateAssociation(ByVal extension As String, ByVal description As String, ByVal application As String, ByVal icon As String, ByVal Optional hive As KeyHiveSmall = KeyHiveSmall.CurrentUser)
        Dim selectedKey As RegistryKey = Nothing

        Select Case hive
            Case KeyHiveSmall.ClassesRoot
                selectedKey = Registry.ClassesRoot.CreateSubKey(extension)
            Case KeyHiveSmall.CurrentUser
                selectedKey = Registry.CurrentUser.CreateSubKey("Software\Classes\" & extension)
            Case KeyHiveSmall.LocalMachine
                selectedKey = Registry.LocalMachine.CreateSubKey("Software\Classes\" & extension)
        End Select

        If selectedKey IsNot Nothing Then
            If description IsNot Nothing Then selectedKey.SetValue(vbNullString, description)

            If icon IsNot Nothing Then
                selectedKey.CreateSubKey("DefaultIcon").SetValue("", icon, RegistryValueKind.ExpandString)
                selectedKey.CreateSubKey("Shell\Verify with Hasher").SetValue("icon", icon, RegistryValueKind.ExpandString)
            End If

            If application IsNot Nothing Then selectedKey.CreateSubKey("Shell\Verify with Hasher\command").SetValue("", """" & application & """" & " --hashfile=""%1""", RegistryValueKind.ExpandString)

            selectedKey.Flush()
            selectedKey.Close()
        End If
    End Sub

    Public Shared Sub SelfCreateAssociation(ByVal extension As String, ByVal Optional hive As KeyHiveSmall = KeyHiveSmall.CurrentUser, ByVal Optional description As String = "")
        Dim FileLocation As String = Reflection.Assembly.GetExecutingAssembly().Location
        CreateAssociation(extension, description, FileLocation, FileLocation & ",0", hive)
    End Sub
End Class