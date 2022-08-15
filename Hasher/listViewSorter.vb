' Implements a comparer for ListView columns.
Class ListViewComparer
    Implements IComparer

    Private ReadOnly intColumnNumber As Integer
    Private ReadOnly soSortOrder As SortOrder

    Public Sub New(intInputColumnNumber As Integer, soInputSortOrder As SortOrder)
        intColumnNumber = intInputColumnNumber
        soSortOrder = soInputSortOrder
    End Sub

    ' Compare the items in the appropriate column
    ' for objects x and y.
    Public Function Compare(lvInputFirstListView As Object, lvInputSecondListView As Object) As Integer Implements IComparer.Compare
        Dim long1, long2 As Long
        Dim timespan1, timespan2 As TimeSpan
        Dim strFirstString, strSecondString As String
        Dim lvFirstListView As ListViewItem = lvInputFirstListView
        Dim lvSecondListView As ListViewItem = lvInputSecondListView

        ' Get the sub-item values.
        strFirstString = If(lvFirstListView.SubItems.Count <= intColumnNumber, "", lvFirstListView.SubItems(intColumnNumber).Text)
        strSecondString = If(lvSecondListView.SubItems.Count <= intColumnNumber, "", lvSecondListView.SubItems(intColumnNumber).Text)

        If lvFirstListView.ListView IsNot Nothing Then
            ' Compare them.
            If (lvFirstListView.ListView.Name = "verifyHashesListFiles" Or lvFirstListView.ListView.Name = "listFiles") And intColumnNumber = 1 Then
                long1 = DirectCast(lvFirstListView, MyListViewItem).FileSize
                long2 = DirectCast(lvSecondListView, MyListViewItem).FileSize

                Return If(soSortOrder = SortOrder.Ascending, long1.CompareTo(long2), long2.CompareTo(long1))
            ElseIf (lvFirstListView.ListView.Name = "verifyHashesListFiles" Or lvFirstListView.ListView.Name = "listFiles") And intColumnNumber = 3 Then
                timespan1 = DirectCast(lvFirstListView, MyListViewItem).ComputeTime
                timespan2 = DirectCast(lvSecondListView, MyListViewItem).ComputeTime

                Return If(soSortOrder = SortOrder.Ascending, timespan1.CompareTo(timespan2), timespan2.CompareTo(timespan1))
            Else
                If Long.TryParse(strFirstString, long1) And Long.TryParse(strSecondString, long2) Then
                    Return If(soSortOrder = SortOrder.Ascending, long1.CompareTo(long2), long2.CompareTo(long1))
                Else
                    Return If(soSortOrder = SortOrder.Ascending, String.Compare(strFirstString, strSecondString), String.Compare(strSecondString, strFirstString))
                End If
            End If
        Else
            Return 0
        End If
    End Function
End Class