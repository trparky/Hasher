﻿Imports Microsoft.Win32

Namespace FileAssociation
    Module FileAssociation
        Private Sub CreateAssociationSubRoutine(ByRef selectedKey As RegistryKey, description As String, application As String, icon As String)
            If selectedKey.OpenSubKey("Shell") Is Nothing Then
                selectedKey.CreateSubKey("Shell")
            End If

            If selectedKey.OpenSubKey("Shell\Verify with Hasher") Is Nothing Then
                If description IsNot Nothing Then selectedKey.SetValue(vbNullString, description)

                If Not String.IsNullOrWhiteSpace(icon) Then
                    selectedKey.CreateSubKey("DefaultIcon").SetValue("", icon, RegistryValueKind.ExpandString)
                    selectedKey.CreateSubKey("Shell\Verify with Hasher").SetValue("icon", icon, RegistryValueKind.ExpandString)
                End If

                If Not String.IsNullOrWhiteSpace(application) Then selectedKey.CreateSubKey("Shell\Verify with Hasher\command").SetValue("", $"""{application}"" --hashfile=""%1""", RegistryValueKind.ExpandString)
            End If

            selectedKey = selectedKey.OpenSubKey("Shell\Verify with Hasher\command", True)

            If selectedKey IsNot Nothing Then
                If Not selectedKey.GetValue("", Nothing).ToString.CaseInsensitiveContains("hasher.exe") Then
                    selectedKey.SetValue("", $"""{application}"" --hashfile=""%1""", RegistryValueKind.ExpandString)
                End If
            End If

            selectedKey.Dispose()
        End Sub

        Public Sub CreateAssociation(extension As String, description As String, application As String, icon As String)
            Dim selectedKey As RegistryKey

            If Registry.CurrentUser.OpenSubKey($"Software\Classes\{extension}") Is Nothing Then
                selectedKey = Registry.CurrentUser.OpenSubKey("Software\Classes", True).CreateSubKey(extension)

                If selectedKey IsNot Nothing Then
                    CreateAssociationSubRoutine(selectedKey, description, application, icon)
                End If
            Else
                Dim strDefaultValue As String = Registry.CurrentUser.OpenSubKey($"Software\Classes\{extension}").GetValue(vbNullString, Nothing)

                If String.IsNullOrWhiteSpace(strDefaultValue) Then
                    CreateAssociationSubRoutine(Registry.CurrentUser.OpenSubKey($"Software\Classes\{extension}"), description, application, icon)
                Else
                    selectedKey = If(Registry.CurrentUser.OpenSubKey($"Software\Classes\{extension}", True), Registry.CurrentUser.OpenSubKey("Software\Classes", True).CreateSubKey(strDefaultValue))
                    CreateAssociationSubRoutine(selectedKey, description, application, icon)
                End If
            End If
        End Sub

        Public Sub SelfCreateAssociation(extension As String, Optional description As String = "")
            Dim FileLocation As String = Reflection.Assembly.GetExecutingAssembly().Location
            CreateAssociation(extension, description, FileLocation, $"{FileLocation},0")
        End Sub

        Public Sub DeleteFileAssociation()
            Using selectedKey As RegistryKey = Registry.CurrentUser.OpenSubKey("Software\Classes\.md5\Shell", True)
                If selectedKey IsNot Nothing Then selectedKey.DeleteSubKeyTree("Verify with Hasher", False)
            End Using
            Using selectedKey As RegistryKey = Registry.CurrentUser.OpenSubKey("Software\Classes\.sha1\Shell", True)
                If selectedKey IsNot Nothing Then selectedKey.DeleteSubKeyTree("Verify with Hasher", False)
            End Using
            Using selectedKey As RegistryKey = Registry.CurrentUser.OpenSubKey("Software\Classes\.sha2\Shell", True)
                If selectedKey IsNot Nothing Then selectedKey.DeleteSubKeyTree("Verify with Hasher", False)
            End Using
            Using selectedKey As RegistryKey = Registry.CurrentUser.OpenSubKey("Software\Classes\.sha256\Shell", True)
                If selectedKey IsNot Nothing Then selectedKey.DeleteSubKeyTree("Verify with Hasher", False)
            End Using
            Using selectedKey As RegistryKey = Registry.CurrentUser.OpenSubKey("Software\Classes\.sha384\Shell", True)
                If selectedKey IsNot Nothing Then selectedKey.DeleteSubKeyTree("Verify with Hasher", False)
            End Using
            Using selectedKey As RegistryKey = Registry.CurrentUser.OpenSubKey("Software\Classes\.sha512\Shell", True)
                If selectedKey IsNot Nothing Then selectedKey.DeleteSubKeyTree("Verify with Hasher", False)
            End Using
        End Sub

        Public Sub DeleteAssociationWithAllFiles()
            Using selectedKey As RegistryKey = Registry.CurrentUser.OpenSubKey("Software\Classes\*\Shell", True)
                If selectedKey.OpenSubKey("Compare Two Files") IsNot Nothing Then selectedKey.DeleteSubKeyTree("Compare Two Files")
                If selectedKey.OpenSubKey("Hash with Hasher") IsNot Nothing Then selectedKey.DeleteSubKeyTree("Hash with Hasher")
                If selectedKey.OpenSubKey("Verify against known hash with Hasher") IsNot Nothing Then selectedKey.DeleteSubKeyTree("Verify against known hash with Hasher")
            End Using

            Using selectedKey As RegistryKey = Registry.CurrentUser.OpenSubKey("Software\Classes\Folder\Shell", True)
                If selectedKey.OpenSubKey("Hash with Hasher") IsNot Nothing Then selectedKey.DeleteSubKeyTree("Hash with Hasher")
            End Using
        End Sub

        Public Sub AddAssociationWithAllFiles()
            If Registry.CurrentUser.OpenSubKey("Software\Classes\*\Shell") Is Nothing Then
                Registry.CurrentUser.OpenSubKey("Software\Classes\*", True).CreateSubKey("Shell")
            End If

            Dim selectedKey As RegistryKey = Registry.CurrentUser.OpenSubKey("Software\Classes\*\Shell", True)
            Dim FileLocation As String = Reflection.Assembly.GetExecutingAssembly().Location

            If selectedKey IsNot Nothing Then
                selectedKey = selectedKey.CreateSubKey("Hash with Hasher")
                selectedKey.SetValue("icon", $"{FileLocation},0", RegistryValueKind.ExpandString)
                selectedKey.CreateSubKey("command").SetValue("", $"""{FileLocation}"" --addfile=""%1""", RegistryValueKind.ExpandString)
            End If

            selectedKey = Registry.CurrentUser.OpenSubKey("Software\Classes\*\Shell", True)

            If selectedKey IsNot Nothing Then
                selectedKey = selectedKey.CreateSubKey("Verify against known hash with Hasher")
                selectedKey.SetValue("icon", $"{FileLocation},0", RegistryValueKind.ExpandString)
                selectedKey.CreateSubKey("command").SetValue("", $"""{FileLocation}"" --knownhashfile=""%1""", RegistryValueKind.ExpandString)
            End If

            selectedKey = Registry.CurrentUser.OpenSubKey("Software\Classes\*\Shell", True)

            If selectedKey IsNot Nothing Then
                selectedKey = selectedKey.CreateSubKey("Compare Two Files")
                selectedKey.SetValue("icon", $"{FileLocation},0", RegistryValueKind.ExpandString)
                selectedKey.CreateSubKey("command").SetValue("", $"""{FileLocation}"" --comparefile=""%1""", RegistryValueKind.ExpandString)
            End If

            selectedKey = Registry.CurrentUser.OpenSubKey("Software\Classes\Folder\Shell", True)

            If selectedKey IsNot Nothing Then
                selectedKey = selectedKey.CreateSubKey("Hash with Hasher")
                selectedKey.SetValue("icon", $"{FileLocation},0", RegistryValueKind.ExpandString)
                selectedKey.CreateSubKey("command").SetValue("", $"""{FileLocation}"" --addfile=""%1""", RegistryValueKind.ExpandString)
            End If
        End Sub

        Public Function DoesCompareFilesExist() As Boolean
            Return Registry.CurrentUser.OpenSubKey("Software\Classes\*\Shell\Compare Two Files", False) IsNot Nothing
        End Function

        Public Sub DeleteSystemLevelFileAssociation()
            Using selectedKey As RegistryKey = Registry.LocalMachine.OpenSubKey("Software\Classes\.md5\Shell", True)
                selectedKey.DeleteSubKeyTree("Verify with Hasher")
            End Using
            Using selectedKey As RegistryKey = Registry.LocalMachine.OpenSubKey("Software\Classes\.sha1\Shell", True)
                selectedKey.DeleteSubKeyTree("Verify with Hasher")
            End Using
            Using selectedKey As RegistryKey = Registry.LocalMachine.OpenSubKey("Software\Classes\.sha2\Shell", True)
                selectedKey.DeleteSubKeyTree("Verify with Hasher")
            End Using
            Using selectedKey As RegistryKey = Registry.LocalMachine.OpenSubKey("Software\Classes\.sha256\Shell", True)
                selectedKey.DeleteSubKeyTree("Verify with Hasher")
            End Using
            Using selectedKey As RegistryKey = Registry.LocalMachine.OpenSubKey("Software\Classes\.sha384\Shell", True)
                selectedKey.DeleteSubKeyTree("Verify with Hasher")
            End Using
            Using selectedKey As RegistryKey = Registry.LocalMachine.OpenSubKey("Software\Classes\.sha512\Shell", True)
                selectedKey.DeleteSubKeyTree("Verify with Hasher")
            End Using
        End Sub

        Public Sub DeleteSystemLevelAssociationWithAllFiles()
            Using selectedKey As RegistryKey = Registry.LocalMachine.OpenSubKey("Software\Classes\*\Shell", True)
                selectedKey.DeleteSubKeyTree("Compare Two Files")
                selectedKey.DeleteSubKeyTree("Hash with Hasher")
                selectedKey.DeleteSubKeyTree("Verify against known hash with Hasher")
            End Using

            Using selectedKey As RegistryKey = Registry.LocalMachine.OpenSubKey("Software\Classes\Folder\Shell", True)
                selectedKey.DeleteSubKeyTree("Hash with Hasher")
            End Using
        End Sub
    End Module
End Namespace