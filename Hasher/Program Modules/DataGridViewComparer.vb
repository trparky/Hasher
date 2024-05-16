Public Class DataGridViewComparer
    Implements IComparer(Of DataGridViewRow)

    Private ReadOnly intColumnNumber As Integer
    Private ReadOnly soSortOrder As SortOrder

    Public Sub New(columnNumber As Integer, sortOrder As SortOrder)
        intColumnNumber = columnNumber
        soSortOrder = sortOrder
    End Sub

    Public Function Compare(row1 As DataGridViewRow, row2 As DataGridViewRow) As Integer Implements IComparer(Of DataGridViewRow).Compare
        Dim fileSize1, fileSize2 As Long

        If intColumnNumber = 1 AndAlso TypeOf row1 Is MyDataGridViewRow AndAlso TypeOf row2 Is MyDataGridViewRow Then
            fileSize1 = DirectCast(row1, MyDataGridViewRow).FileSize
            fileSize2 = DirectCast(row2, MyDataGridViewRow).FileSize
            Return If(soSortOrder = SortOrder.Ascending, fileSize1.CompareTo(fileSize2), fileSize2.CompareTo(fileSize1))
        End If

        Return 0
    End Function
End Class