Imports System.Runtime.CompilerServices

Public Module SaveAppSettings
    Public Sub SaveApplicationSettingsToFile(strFileName As String)
        Dim exportedSettingsArray As New Dictionary(Of String, Object)(StringComparison.OrdinalIgnoreCase)
        Dim settingType As Type
        Dim point As Point, size As Size
        Dim rawValue As Object

        For Each settingProperty As Configuration.SettingsPropertyValue In My.Settings.PropertyValues
            If settingProperty.PropertyValue IsNot Nothing Then
                settingType = settingProperty.PropertyValue.GetType

                If settingType = GetType(Point) Then
                    point = DirectCast(settingProperty.PropertyValue, Point)
                    rawValue = $"{point.X}|{point.Y}"
                    point = Nothing
                ElseIf settingType = GetType(Color) Then
                    rawValue = DirectCast(settingProperty.PropertyValue, Color).ToArgb
                ElseIf settingType = GetType(Size) Then
                    size = DirectCast(settingProperty.PropertyValue, Size)
                    rawValue = $"{size.Height}|{size.Width}"
                    size = Nothing
                Else
                    rawValue = settingProperty.PropertyValue
                End If

                exportedSettingsArray.Add(settingProperty.Name.Trim, rawValue)
            End If
        Next

        WriteFileAtomically(strFileName, Newtonsoft.Json.JsonConvert.SerializeObject(exportedSettingsArray))
    End Sub

    Public Function LoadApplicationSettingsFromFile(strFileName As String, strMessageBoxTitle As String) As Boolean
        Try
            Dim exportedSettingsArray As New Dictionary(Of String, Object)(StringComparison.OrdinalIgnoreCase)
            Dim boolResult As Boolean, byteResult As Byte, intResult As Integer, longResult As Long, settingType As Type, shortResult As Short, splitArray As String()
            Dim rawValue As Object = Nothing

            Using streamReader As New IO.StreamReader(strFileName)
                exportedSettingsArray = Newtonsoft.Json.JsonConvert.DeserializeObject(Of Dictionary(Of String, Object))(streamReader.ReadToEnd.Trim)
            End Using

            For Each settingProperty As Configuration.SettingsPropertyValue In My.Settings.PropertyValues
                If exportedSettingsArray.FindKeyInDictionaryAndReturnIt(settingProperty.Name, rawValue) And settingProperty.PropertyValue IsNot Nothing Then
                    If settingProperty.Name.Equals("listFilesColumnOrder", StringComparison.OrdinalIgnoreCase) Or settingProperty.Name.Equals("verifyListFilesColumnOrder", StringComparison.OrdinalIgnoreCase) Then
                        settingType = GetType(Specialized.StringCollection)
                    Else
                        settingType = settingProperty.PropertyValue.GetType
                    End If

                    If settingType = GetType(Color) AndAlso Integer.TryParse(rawValue, intResult) Then
                        My.Settings(settingProperty.Name) = Color.FromArgb(intResult)
                    ElseIf settingType = GetType(Point) Then
                        splitArray = rawValue.split("|")
                        My.Settings(settingProperty.Name) = New Point() With {.X = splitArray(0), .Y = splitArray(1)}
                        splitArray = Nothing
                    ElseIf settingType = GetType(Size) Then
                        splitArray = rawValue.split("|")
                        My.Settings(settingProperty.Name) = New Size() With {.Height = splitArray(0), .Width = splitArray(1)}
                        splitArray = Nothing
                    ElseIf settingType = GetType(Boolean) AndAlso Boolean.TryParse(rawValue, boolResult) Then
                        My.Settings(settingProperty.Name) = boolResult
                    ElseIf settingType = GetType(Byte) AndAlso Byte.TryParse(rawValue, byteResult) Then
                        My.Settings(settingProperty.Name) = byteResult
                    ElseIf settingType = GetType(Short) AndAlso Short.TryParse(rawValue, shortResult) Then
                        My.Settings(settingProperty.Name) = shortResult
                    ElseIf settingType = GetType(Integer) AndAlso Integer.TryParse(rawValue, intResult) Then
                        My.Settings(settingProperty.Name) = intResult
                    ElseIf settingType = GetType(Long) AndAlso Long.TryParse(rawValue, longResult) Then
                        My.Settings(settingProperty.Name) = longResult
                    ElseIf settingType = GetType(Specialized.StringCollection) Then
                        My.Settings(settingProperty.Name) = ConvertArrayListToSpecializedStringCollection(rawValue)
                    Else
                        My.Settings(settingProperty.Name) = rawValue
                    End If
                End If
            Next

            Return True
        Catch ex As Exception
            MsgBox($"There was an issue decoding your chosen JSON settings file, import failed.{DoubleCRLF}{ex.Message}{ex.StackTrace.Trim}", MsgBoxStyle.Critical, strMessageBoxTitle)
            Return False
        End Try
    End Function

    Private Function ConvertArrayListToSpecializedStringCollection(input As Newtonsoft.Json.Linq.JArray) As Specialized.StringCollection
        Try
            Dim stringCollection As New Specialized.StringCollection
            For Each item As String In input
                stringCollection.Add(item)
            Next
            Return stringCollection
        Catch ex As Exception
            Return New Specialized.StringCollection
        End Try
    End Function

    ''' <summary>This function operates a lot like ContainsKey() but is case-InSeNsItIvE and it returns the value of the key/value pair if the function returns True.</summary>
    ''' <param name="haystack">The dictionary that's being searched.</param>
    ''' <param name="needle">The key that you're looking for.</param>
    ''' <param name="value">The value of the key you're looking for, passed as a ByRef so that you can access the value if the function returns True.</param>
    ''' <return>Returns a String value.</return>
    <Extension()>
    Private Function FindKeyInDictionaryAndReturnIt(haystack As Dictionary(Of String, Object), needle As String, ByRef value As Object) As Boolean
        If String.IsNullOrEmpty(needle) Then
            Throw New ArgumentException($"'{NameOf(needle)}' cannot be null or empty.", NameOf(needle))
        End If
        If haystack Is Nothing Then
            Throw New ArgumentNullException(NameOf(haystack))
        End If

        Dim KeyValuePair As KeyValuePair(Of String, Object) = haystack.FirstOrDefault(Function(item As KeyValuePair(Of String, Object)) item.Key.Trim.Equals(needle, StringComparison.OrdinalIgnoreCase))
        If KeyValuePair.Value IsNot Nothing Then value = KeyValuePair.Value
        Return KeyValuePair.Value IsNot Nothing
    End Function
End Module