Imports System.Web.Script.Serialization

Public Module SaveAppSettings
    Public Sub SaveApplicationSettingsToFile(strFileName As String)
        Dim exportedSettingsArray As New Dictionary(Of String, Object)
        Dim settingType As Type
        Dim point As Point, size As Size
        Dim rawValue As Object

        For Each settingProperty As Configuration.SettingsPropertyValue In My.Settings.PropertyValues
            If settingProperty.PropertyValue IsNot Nothing Then
                settingType = settingProperty.PropertyValue.GetType

                If settingType = GetType(Point) Then
                    point = DirectCast(settingProperty.PropertyValue, Point)
                    rawValue = point.X & "|" & point.Y
                    point = Nothing
                ElseIf settingType = GetType(Color) Then
                    rawValue = DirectCast(settingProperty.PropertyValue, Color).ToArgb
                ElseIf settingType = GetType(Size) Then
                    size = DirectCast(settingProperty.PropertyValue, Size)
                    rawValue = size.Height & "|" & size.Width
                    size = Nothing
                Else
                    rawValue = settingProperty.PropertyValue
                End If

                exportedSettingsArray.Add(settingProperty.Name.Trim.ToLower, rawValue)
            End If
        Next

        Using streamWriter As New IO.StreamWriter(strFileName)
            Dim json As New JavaScriptSerializer()
            streamWriter.Write(json.Serialize(exportedSettingsArray))
        End Using
    End Sub

    Public Function LoadApplicationSettingsFromFile(strFileName As String) As Boolean
        Try
            Dim exportedSettingsArray As New Dictionary(Of String, Object)
            Dim boolResult As Boolean, byteResult As Byte, intResult As Integer, longResult As Long, settingType As Type, shortResult As Short, splitArray As String()
            Dim rawValue As Object = Nothing

            Using streamReader As New IO.StreamReader(strFileName)
                Dim json As New JavaScriptSerializer()
                exportedSettingsArray = json.Deserialize(Of Dictionary(Of String, Object))(streamReader.ReadToEnd.Trim)
            End Using

            For Each settingProperty As Configuration.SettingsPropertyValue In My.Settings.PropertyValues
                ' We use the TryGetValue() function to get the save setting from the dictionary because there might be a possibility that the user will
                ' be loading an older version of a saved settings file that may be missing new settings. Using the TryGetValue() True if the setting
                ' exists in the loaded function will return settings file thus making the program not crash while loading an older settings file.
                If exportedSettingsArray.TryGetValue(settingProperty.Name.Trim.ToLower, rawValue) Then
                    If settingProperty.PropertyValue IsNot Nothing Then
                        settingType = settingProperty.PropertyValue.GetType

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
                End If
            Next

            Return True
        Catch ex As Exception
            MsgBox("There was an issue decoding your chosen JSON settings file, import failed." & DoubleCRLF & ex.Message & ex.StackTrace.Trim, MsgBoxStyle.Critical, checkForUpdates.strMessageBoxTitleText)
            Return False
        End Try
    End Function

    Private Function ConvertArrayListToSpecializedStringCollection(input As ArrayList) As Specialized.StringCollection
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
End Module