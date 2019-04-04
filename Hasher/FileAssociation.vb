﻿Imports Microsoft.Win32

Class FileAssociation
    Private Shared Sub CreateAssociationSubRoutine(ByRef selectedKey As RegistryKey, ByVal description As String, ByVal application As String, ByVal icon As String)
        If selectedKey.OpenSubKey("Shell\Verify with Hasher") Is Nothing Then
            If description IsNot Nothing Then selectedKey.SetValue(vbNullString, description)

            If icon IsNot Nothing Then
                selectedKey.CreateSubKey("DefaultIcon").SetValue("", icon, RegistryValueKind.ExpandString)
                selectedKey.CreateSubKey("Shell\Verify with Hasher").SetValue("icon", icon, RegistryValueKind.ExpandString)
            End If

            If application IsNot Nothing Then selectedKey.CreateSubKey("Shell\Verify with Hasher\command").SetValue("", """" & application & """" & " --hashfile=""%1""", RegistryValueKind.ExpandString)
        End If

        selectedKey = selectedKey.OpenSubKey("Shell\Verify with Hasher\command", True)

        If selectedKey IsNot Nothing Then
            If Not selectedKey.GetValue("", Nothing).ToString.caseInsensitiveContains("hasher.exe") Then
                selectedKey.SetValue("", """" & application & """" & " --hashfile=""%1""", RegistryValueKind.ExpandString)
            End If
        End If

        selectedKey.Flush()
        selectedKey.Close()
    End Sub

    Public Shared Sub CreateAssociation(ByVal extension As String, ByVal description As String, ByVal application As String, ByVal icon As String)
        Dim selectedKey As RegistryKey

        If Registry.ClassesRoot.OpenSubKey(extension) Is Nothing Then
            selectedKey = Registry.ClassesRoot.CreateSubKey(extension)

            If selectedKey IsNot Nothing Then
                CreateAssociationSubRoutine(selectedKey, description, application, icon)
            End If
        Else
            Dim strDefaultValue As String = Registry.ClassesRoot.OpenSubKey(extension).GetValue(vbNullString, Nothing)

            If String.IsNullOrWhiteSpace(strDefaultValue) Then
                CreateAssociationSubRoutine(Registry.ClassesRoot.OpenSubKey(extension), description, application, icon)
            Else
                selectedKey = Registry.ClassesRoot.OpenSubKey(strDefaultValue, True)
                If selectedKey Is Nothing Then
                    selectedKey = Registry.ClassesRoot.CreateSubKey(strDefaultValue)
                End If
                CreateAssociationSubRoutine(selectedKey, description, application, icon)
            End If
        End If
    End Sub

    Public Shared Sub SelfCreateAssociation(ByVal extension As String, ByVal Optional description As String = "")
        Dim FileLocation As String = Reflection.Assembly.GetExecutingAssembly().Location
        CreateAssociation(extension, description, FileLocation, FileLocation & ",0")
    End Sub

    Public Shared Sub addAssociationWithAllFiles()
        Dim selectedKey As RegistryKey = Registry.ClassesRoot.OpenSubKey("*\Shell", True)
        Dim FileLocation As String = Reflection.Assembly.GetExecutingAssembly().Location

        If selectedKey IsNot Nothing Then
            selectedKey = selectedKey.CreateSubKey("Hash with Hasher")
            selectedKey.SetValue("icon", FileLocation & ",0", RegistryValueKind.ExpandString)
            selectedKey.CreateSubKey("command").SetValue("", """" & FileLocation & """" & " --addfile=""%1""", RegistryValueKind.ExpandString)
        End If

        selectedKey = Registry.ClassesRoot.OpenSubKey("*\Shell", True)

        If selectedKey IsNot Nothing Then
            selectedKey = selectedKey.CreateSubKey("Verify against known hash with Hasher")
            selectedKey.SetValue("icon", FileLocation & ",0", RegistryValueKind.ExpandString)
            selectedKey.CreateSubKey("command").SetValue("", """" & FileLocation & """" & " --knownhashfile=""%1""", RegistryValueKind.ExpandString)
        End If

        selectedKey = Registry.ClassesRoot.OpenSubKey("Folder\Shell", True)

        If selectedKey IsNot Nothing Then
            selectedKey = selectedKey.CreateSubKey("Hash with Hasher")
            selectedKey.SetValue("icon", FileLocation & ",0", RegistryValueKind.ExpandString)
            selectedKey.CreateSubKey("command").SetValue("", """" & FileLocation & """" & " --addfile=""%1""", RegistryValueKind.ExpandString)
        End If
    End Sub
End Class