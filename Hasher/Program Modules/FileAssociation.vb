Imports Microsoft.Win32

Namespace FileAssociation
    Module FileAssociation
        Private Sub CreateAssociationSubRoutine(ByRef selectedKey As RegistryKey, description As String, application As String, icon As String)
            If selectedKey.OpenSubKey("Shell") Is Nothing Then
                selectedKey.CreateSubKey("Shell")
            End If

            If selectedKey.OpenSubKey("Shell\Verify with Hasher") Is Nothing Then
                If description IsNot Nothing Then selectedKey.SetValue(vbNullString, description)

                If icon IsNot Nothing Then
                    selectedKey.CreateSubKey("DefaultIcon").SetValue("", icon, RegistryValueKind.ExpandString)
                    selectedKey.CreateSubKey("Shell\Verify with Hasher").SetValue("icon", icon, RegistryValueKind.ExpandString)
                End If

                If application IsNot Nothing Then selectedKey.CreateSubKey("Shell\Verify with Hasher\command").SetValue("", String.Format("{0}{1}{0} --hashfile={0}%1{0}", Chr(34), application), RegistryValueKind.ExpandString)
            End If

            selectedKey = selectedKey.OpenSubKey("Shell\Verify with Hasher\command", True)

            If selectedKey IsNot Nothing Then
                If Not selectedKey.GetValue("", Nothing).ToString.CaseInsensitiveContains("hasher.exe") Then
                    selectedKey.SetValue("", String.Format("{0}{1}{0} --hashfile={0}%1{0}", Chr(34), application), RegistryValueKind.ExpandString)
                End If
            End If

            selectedKey.Dispose()
        End Sub

        ' Registry.CurrentUser.OpenSubKey("Software\Classes\" & 

        Public Sub CreateAssociation(extension As String, description As String, application As String, icon As String)
            Dim selectedKey As RegistryKey

            If Registry.CurrentUser.OpenSubKey("Software\Classes\" & extension) Is Nothing Then
                selectedKey = Registry.CurrentUser.OpenSubKey("Software\Classes\", True).CreateSubKey(extension)

                If selectedKey IsNot Nothing Then
                    CreateAssociationSubRoutine(selectedKey, description, application, icon)
                End If
            Else
                Dim strDefaultValue As String = Registry.CurrentUser.OpenSubKey("Software\Classes\" & extension).GetValue(vbNullString, Nothing)

                If String.IsNullOrWhiteSpace(strDefaultValue) Then
                    CreateAssociationSubRoutine(Registry.CurrentUser.OpenSubKey("Software\Classes\" & extension), description, application, icon)
                Else
                    selectedKey = Registry.CurrentUser.OpenSubKey("Software\Classes\" & extension & "\" & strDefaultValue, True)
                    If selectedKey Is Nothing Then
                        selectedKey = Registry.CurrentUser.OpenSubKey("Software\Classes", True).CreateSubKey(strDefaultValue)
                    End If
                    CreateAssociationSubRoutine(selectedKey, description, application, icon)
                End If
            End If
        End Sub

        Public Sub SelfCreateAssociation(extension As String, Optional description As String = "")
            Dim FileLocation As String = Reflection.Assembly.GetExecutingAssembly().Location
            CreateAssociation(extension, description, FileLocation, FileLocation & ",0")
        End Sub

        Public Sub AddAssociationWithAllFiles()
            If Registry.CurrentUser.OpenSubKey("Software\Classes\*\Shell") Is Nothing Then
                Registry.CurrentUser.OpenSubKey("Software\Classes\*", True).CreateSubKey("Shell")
            End If

            Dim selectedKey As RegistryKey = Registry.CurrentUser.OpenSubKey("Software\Classes", True).OpenSubKey("*\Shell", True)
            Dim FileLocation As String = Reflection.Assembly.GetExecutingAssembly().Location

            If selectedKey IsNot Nothing Then
                selectedKey = selectedKey.CreateSubKey("Hash with Hasher")
                selectedKey.SetValue("icon", FileLocation & ",0", RegistryValueKind.ExpandString)
                selectedKey.CreateSubKey("command").SetValue("", String.Format("{0}{1}{0} --addfile={0}%1{0}", Chr(34), FileLocation), RegistryValueKind.ExpandString)
            End If

            selectedKey = Registry.CurrentUser.OpenSubKey("Software\Classes", True).OpenSubKey("*\Shell", True)

            If selectedKey IsNot Nothing Then
                selectedKey = selectedKey.CreateSubKey("Verify against known hash with Hasher")
                selectedKey.SetValue("icon", FileLocation & ",0", RegistryValueKind.ExpandString)
                selectedKey.CreateSubKey("command").SetValue("", String.Format("{0}{1}{0} --knownhashfile={0}%1{0}", Chr(34), FileLocation), RegistryValueKind.ExpandString)
            End If

            selectedKey = Registry.CurrentUser.OpenSubKey("Software\Classes\*\Shell", True)

            If selectedKey IsNot Nothing Then
                selectedKey = selectedKey.CreateSubKey("Compare Two Files")
                selectedKey.SetValue("icon", FileLocation & ",0", RegistryValueKind.ExpandString)
                selectedKey.CreateSubKey("command").SetValue("", String.Format("{0}{1}{0} --comparefile={0}%1{0}", Chr(34), FileLocation), RegistryValueKind.ExpandString)
            End If

            selectedKey = Registry.CurrentUser.OpenSubKey("Software\Classes\Folder\Shell", True)

            If selectedKey IsNot Nothing Then
                selectedKey = selectedKey.CreateSubKey("Hash with Hasher")
                selectedKey.SetValue("icon", FileLocation & ",0", RegistryValueKind.ExpandString)
                selectedKey.CreateSubKey("command").SetValue("", String.Format("{0}{1}{0} --addfile={0}%1{0}", Chr(34), FileLocation), RegistryValueKind.ExpandString)
            End If
        End Sub

        Public Function DoesCompareFilesExist() As Boolean
            Return Registry.CurrentUser.OpenSubKey("Software\Classes\*\Shell\Compare Two Files", False) IsNot Nothing
        End Function
    End Module
End Namespace