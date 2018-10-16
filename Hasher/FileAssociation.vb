Imports Microsoft.Win32

Class FileAssociation
    Public Enum KeyHiveSmall
        ClassesRoot
        CurrentUser
        LocalMachine
    End Enum

    Public Shared Sub CreateAssociation(ByVal ProgID As String, ByVal extension As String, ByVal description As String, ByVal application As String, ByVal icon As String, ByVal Optional hive As KeyHiveSmall = KeyHiveSmall.CurrentUser)
        Dim selectedKey As RegistryKey = Nothing

        Select Case hive
            Case KeyHiveSmall.ClassesRoot
                Registry.ClassesRoot.CreateSubKey(extension).SetValue("", ProgID)
                selectedKey = Registry.ClassesRoot.CreateSubKey(ProgID)
            Case KeyHiveSmall.CurrentUser
                Registry.CurrentUser.CreateSubKey("Software\Classes\" & extension).SetValue("", ProgID)
                selectedKey = Registry.CurrentUser.CreateSubKey("Software\Classes\" & ProgID)
            Case KeyHiveSmall.LocalMachine
                Registry.LocalMachine.CreateSubKey("Software\Classes\" & extension).SetValue("", ProgID)
                selectedKey = Registry.LocalMachine.CreateSubKey("Software\Classes\" & ProgID)
        End Select

        If selectedKey IsNot Nothing Then
            If description IsNot Nothing Then selectedKey.SetValue("", description)

            If icon IsNot Nothing Then
                selectedKey.CreateSubKey("DefaultIcon").SetValue("", icon, RegistryValueKind.ExpandString)
                selectedKey.CreateSubKey("Shell\Open").SetValue("icon", icon, RegistryValueKind.ExpandString)
            End If

            If application IsNot Nothing Then selectedKey.CreateSubKey("Shell\Open\command").SetValue("", """" & application & """" & " --hashfile=""%1""", RegistryValueKind.ExpandString)

            selectedKey.Flush()
            selectedKey.Close()
        End If
    End Sub

    Public Shared Sub SelfCreateAssociation(ByVal extension As String, ByVal Optional hive As KeyHiveSmall = KeyHiveSmall.CurrentUser, ByVal Optional description As String = "")
        Dim ProgID As String = Reflection.Assembly.GetExecutingAssembly().EntryPoint.DeclaringType.FullName
        Dim FileLocation As String = Reflection.Assembly.GetExecutingAssembly().Location
        CreateAssociation(ProgID, extension, description, FileLocation, FileLocation & ",0", hive)
    End Sub
End Class