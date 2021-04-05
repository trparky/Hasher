﻿Imports System.Web.Script.Serialization

Public Module SavedAppSettingsModule
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
            Dim json As JavaScriptSerializer = New JavaScriptSerializer()
            streamWriter.Write(json.Serialize(exportedSettingsArray))
        End Using
    End Sub

    Public Sub LoadApplicationSettingsFromFile(strFileName As String)
        Dim exportedSettingsArray As New Dictionary(Of String, Object)
        Dim boolResult As Boolean, byteResult As Byte, intResult As Integer, longResult As Long, settingType As Type, shortResult As Short, splitArray As String()
        Dim rawValue As Object = Nothing

        Using streamReader As New IO.StreamReader(strFileName)
            Dim json As JavaScriptSerializer = New JavaScriptSerializer()
            exportedSettingsArray = json.Deserialize(Of Dictionary(Of String, Object))(streamReader.ReadToEnd.Trim)
        End Using

        For Each settingProperty As Configuration.SettingsPropertyValue In My.Settings.PropertyValues
            If exportedSettingsArray.TryGetValue(settingProperty.Name.Trim.ToLower, rawValue) Then
                settingType = settingProperty.PropertyValue.GetType

                If settingType = GetType(Color) Then
                    If Integer.TryParse(rawValue, intResult) Then My.Settings(settingProperty.Name) = Color.FromArgb(intResult)
                ElseIf settingType = GetType(Point) Then
                    splitArray = rawValue.split("|")
                    My.Settings(settingProperty.Name) = New Point() With {.X = splitArray(0), .Y = splitArray(1)}
                    splitArray = Nothing
                ElseIf settingType = GetType(Size) Then
                    splitArray = rawValue.split("|")
                    My.Settings(settingProperty.Name) = New Size() With {.Height = splitArray(0), .Width = splitArray(1)}
                    splitArray = Nothing
                ElseIf settingType = GetType(Boolean) Then
                    If Boolean.TryParse(rawValue, boolResult) Then My.Settings(settingProperty.Name) = boolResult
                ElseIf settingType = GetType(Byte) Then
                    If Byte.TryParse(rawValue, byteResult) Then My.Settings(settingProperty.Name) = byteResult
                ElseIf settingType = GetType(Short) Then
                    If Short.TryParse(rawValue, shortResult) Then My.Settings(settingProperty.Name) = shortResult
                ElseIf settingType = GetType(Integer) Then
                    If Integer.TryParse(rawValue, intResult) Then My.Settings(settingProperty.Name) = intResult
                ElseIf settingType = GetType(Long) Then
                    If Long.TryParse(rawValue, longResult) Then My.Settings(settingProperty.Name) = longResult
                Else
                    My.Settings(settingProperty.Name) = rawValue
                End If
            End If
        Next
    End Sub
End Module