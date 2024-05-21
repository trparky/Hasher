Public Class DataGridViewComparer
    Implements IComparer(Of MyDataGridViewRow)

    Private ReadOnly intColumnNumber As Integer
    Private ReadOnly soSortOrder As SortOrder

    Public Sub New(columnNumber As Integer, sortOrder As SortOrder)
        intColumnNumber = columnNumber
        soSortOrder = sortOrder
    End Sub

    Public Function Compare(row1 As MyDataGridViewRow, row2 As MyDataGridViewRow) As Integer Implements IComparer(Of MyDataGridViewRow).Compare
        If intColumnNumber = 1 Then
            Dim fileSize1 As Long = row1.FileSize
            Dim fileSize2 As Long = row2.FileSize

            Return If(soSortOrder = Global.System.Windows.Forms.SortOrder.Ascending, fileSize1.CompareTo(fileSize2), fileSize2.CompareTo(fileSize1))
        Else
            Dim strFirstString As String = If(row1.Cells.Count <= intColumnNumber, "", row1.Cells(intColumnNumber).Value?.ToString())
            Dim strSecondString As String = If(row2.Cells.Count <= intColumnNumber, "", row2.Cells(intColumnNumber).Value?.ToString())

            Return If(soSortOrder = SortOrder.Ascending, String.Compare(strFirstString, strSecondString), String.Compare(strSecondString, strFirstString))
        End If

        Return 0
    End Function
End Class