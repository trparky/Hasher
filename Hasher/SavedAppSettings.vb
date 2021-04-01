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
        Dim ExportedSettingsObject As ExportedSettings
        Dim boolResult As Boolean, byteResult As Byte, intResult As Integer, longResult As Long, shortResult As Short, splitArray As String()

        Using streamReader As New IO.StreamReader(strFileName)
            Dim json As JavaScriptSerializer = New JavaScriptSerializer()
            exportedSettingsArray = json.Deserialize(Of List(Of ExportedSettings))(streamReader.ReadToEnd.Trim)
        End Using

        For Each settingProperty As Configuration.SettingsPropertyValue In My.Settings.PropertyValues
            ExportedSettingsObject = exportedSettingsArray.ToList.Find(Function(item As ExportedSettings) item.strName.Equals(settingProperty.Name, StringComparison.OrdinalIgnoreCase))

            If ExportedSettingsObject IsNot Nothing Then
                If ExportedSettingsObject.type.CaseInsensitiveContains("Drawing.Color") Then
                    My.Settings(ExportedSettingsObject.strName) = Color.FromArgb(ExportedSettingsObject.value)
                ElseIf ExportedSettingsObject.type.CaseInsensitiveContains("Drawing.Point") Then
                    splitArray = ExportedSettingsObject.value.split("|")
                    My.Settings(ExportedSettingsObject.strName) = New Point() With {.X = splitArray(0), .Y = splitArray(1)}
                    splitArray = Nothing
                ElseIf ExportedSettingsObject.type.CaseInsensitiveContains("Drawing.Size") Then
                    splitArray = ExportedSettingsObject.value.split("|")
                    My.Settings(ExportedSettingsObject.strName) = New Size() With {.Height = splitArray(0), .Width = splitArray(1)}
                    splitArray = Nothing
                ElseIf ExportedSettingsObject.type.CaseInsensitiveContains("Boolean") Then
                    If Boolean.TryParse(ExportedSettingsObject.value, boolResult) Then My.Settings(ExportedSettingsObject.strName) = boolResult
                ElseIf ExportedSettingsObject.type.CaseInsensitiveContains("Byte") Then
                    If Byte.TryParse(ExportedSettingsObject.value, byteResult) Then My.Settings(ExportedSettingsObject.strName) = byteResult
                ElseIf ExportedSettingsObject.type.CaseInsensitiveContains("Int16") Then
                    If Short.TryParse(ExportedSettingsObject.value, shortResult) Then My.Settings(ExportedSettingsObject.strName) = shortResult
                ElseIf ExportedSettingsObject.type.CaseInsensitiveContains("Int32") Then
                    If Integer.TryParse(ExportedSettingsObject.value, intResult) Then My.Settings(ExportedSettingsObject.strName) = intResult
                ElseIf ExportedSettingsObject.type.CaseInsensitiveContains("Int64") Then
                    If Long.TryParse(ExportedSettingsObject.value, longResult) Then My.Settings(ExportedSettingsObject.strName) = longResult
                Else
                    My.Settings(ExportedSettingsObject.strName) = ExportedSettingsObject.value
                End If
            End If
        Next
    End Sub
End Module