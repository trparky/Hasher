Public Class DataGridViewComparer
    Implements IComparer(Of DataGridViewRow)

    Private ReadOnly intColumnNumber As Integer
    Private ReadOnly soSortOrder As SortOrder

    Public Sub New(columnNumber As Integer, sortOrder As SortOrder)
        intColumnNumber = columnNumber
        soSortOrder = sortOrder
    End Sub

    Public Function Compare(row1 As DataGridViewRow, row2 As DataGridViewRow) As Integer Implements IComparer(Of DataGridViewRow).Compare
        Dim strFirstString, strSecondString As String
        Dim fileSize1, fileSize2 As Long

        ' Get the cell values.
        strFirstString = If(row1.Cells.Count <= intColumnNumber, "", row1.Cells(intColumnNumber).Value?.ToString())
        strSecondString = If(row2.Cells.Count <= intColumnNumber, "", row2.Cells(intColumnNumber).Value?.ToString())

        ' Compare them.
        If intColumnNumber = 1 Then
            If TypeOf row1 Is MyDataGridViewRow AndAlso TypeOf row2 Is MyDataGridViewRow Then
                fileSize1 = DirectCast(row1, MyDataGridViewRow).FileSize
                fileSize2 = DirectCast(row2, MyDataGridViewRow).FileSize
                Return If(soSortOrder = SortOrder.Ascending, fileSize1.CompareTo(fileSize2), fileSize2.CompareTo(fileSize1))
            End If
        Else
            Return If(soSortOrder = SortOrder.Ascending, String.Compare(strFirstString, strSecondString), String.Compare(strSecondString, strFirstString))
        End If

        Return 0
    End Function
End Class