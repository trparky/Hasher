Imports System.Web.Script.Serialization

Public Class ExportedSettings
    Public strName As String, value As Object
    Public type As String
End Class

Public Module SavedAppSettingsModule
    Public Sub SaveApplicationSettingsToFile(strFileName As String)
        Dim exportedSettingsArray As New List(Of ExportedSettings)
        Dim exportedSettingsObject As ExportedSettings
        Dim settingType As Type
        Dim point As Point, size As Size

        For Each settingProperty As Configuration.SettingsPropertyValue In My.Settings.PropertyValues
            If settingProperty.PropertyValue IsNot Nothing Then
                settingType = settingProperty.PropertyValue.GetType
                exportedSettingsObject = New ExportedSettings With {.strName = settingProperty.Name, .type = settingProperty.PropertyValue.GetType.ToString}

                If settingType = GetType(Point) Then
                    point = DirectCast(settingProperty.PropertyValue, Point)
                    exportedSettingsObject.value = point.X & "|" & point.Y
                    point = Nothing
                ElseIf settingType = GetType(Color) Then
                    exportedSettingsObject.value = DirectCast(settingProperty.PropertyValue, Color).ToArgb
                ElseIf settingType = GetType(Size) Then
                    size = DirectCast(settingProperty.PropertyValue, Size)
                    exportedSettingsObject.value = size.Height & "|" & size.Width
                    size = Nothing
                Else
                    exportedSettingsObject.value = settingProperty.PropertyValue
                End If

                exportedSettingsArray.Add(exportedSettingsObject)
                exportedSettingsObject = Nothing
            End If
        Next

        Using streamWriter As New IO.StreamWriter(strFileName)
            Dim json As JavaScriptSerializer = New JavaScriptSerializer()
            streamWriter.Write(json.Serialize(exportedSettingsArray))
        End Using
    End Sub

    Public Sub LoadApplicationSettingsFromFile(strFileName As String)
        Dim exportedSettingsArray As New List(Of ExportedSettings)
        Dim exportedSettingsObject As ExportedSettings
        Dim boolResult As Boolean, byteResult As Byte, intResult As Integer, longResult As Long, shortResult As Short, splitArray As String()

        Using streamReader As New IO.StreamReader(strFileName)
            Dim json As JavaScriptSerializer = New JavaScriptSerializer()
            exportedSettingsArray = json.Deserialize(Of List(Of ExportedSettings))(streamReader.ReadToEnd.Trim)
        End Using

        For Each settingProperty As Configuration.SettingsPropertyValue In My.Settings.PropertyValues
            exportedSettingsObject = exportedSettingsArray.ToList.Find(Function(item As ExportedSettings) item.strName.Equals(settingProperty.Name, StringComparison.OrdinalIgnoreCase))

            If exportedSettingsObject IsNot Nothing Then
                If exportedSettingsObject.type.CaseInsensitiveContains("Drawing.Color") Then
                    My.Settings(exportedSettingsObject.strName) = Color.FromArgb(exportedSettingsObject.value)
                ElseIf exportedSettingsObject.type.CaseInsensitiveContains("Drawing.Point") Then
                    splitArray = exportedSettingsObject.value.split("|")
                    My.Settings(exportedSettingsObject.strName) = New Point() With {.X = splitArray(0), .Y = splitArray(1)}
                    splitArray = Nothing
                ElseIf exportedSettingsObject.type.CaseInsensitiveContains("Drawing.Size") Then
                    splitArray = exportedSettingsObject.value.split("|")
                    My.Settings(exportedSettingsObject.strName) = New Size() With {.Height = splitArray(0), .Width = splitArray(1)}
                    splitArray = Nothing
                ElseIf exportedSettingsObject.type.CaseInsensitiveContains("Boolean") Then
                    If Boolean.TryParse(exportedSettingsObject.value, boolResult) Then My.Settings(exportedSettingsObject.strName) = boolResult
                ElseIf exportedSettingsObject.type.CaseInsensitiveContains("Byte") Then
                    If Byte.TryParse(exportedSettingsObject.value, byteResult) Then My.Settings(exportedSettingsObject.strName) = byteResult
                ElseIf exportedSettingsObject.type.CaseInsensitiveContains("Int16") Then
                    If Short.TryParse(exportedSettingsObject.value, shortResult) Then My.Settings(exportedSettingsObject.strName) = shortResult
                ElseIf exportedSettingsObject.type.CaseInsensitiveContains("Int32") Then
                    If Integer.TryParse(exportedSettingsObject.value, intResult) Then My.Settings(exportedSettingsObject.strName) = intResult
                ElseIf exportedSettingsObject.type.CaseInsensitiveContains("Int64") Then
                    If Long.TryParse(exportedSettingsObject.value, longResult) Then My.Settings(exportedSettingsObject.strName) = longResult
                Else
                    My.Settings(exportedSettingsObject.strName) = exportedSettingsObject.value
                End If
            End If
        Next
    End Sub
End Module