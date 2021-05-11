Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Runtime.ConstrainedExecution
Imports System.Runtime.InteropServices
Imports System.Security
Imports System.Security.Permissions
Imports Microsoft.Win32.SafeHandles

Namespace FastDirectoryEnumerator
    <Serializable>
    Public Structure FileData
        Public ReadOnly Attributes As FileAttributes
        Public ReadOnly LastWriteTimeUtc As Date
        Public ReadOnly Size As Long
        Public ReadOnly Name As String
        Public ReadOnly Path As String
        Public ReadOnly LastAccessTimeUtc As Date
        Public ReadOnly CreationTimeUtc As Date

        Public ReadOnly Property CreationTime As Date
            Get
                Return CreationTimeUtc.ToLocalTime()
            End Get
        End Property

        Public ReadOnly Property LastAccesTime As Date
            Get
                Return LastAccessTimeUtc.ToLocalTime()
            End Get
        End Property

        Public ReadOnly Property LastWriteTime As Date
            Get
                Return LastWriteTimeUtc.ToLocalTime()
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return Name
        End Function

        Friend Sub New(ByVal dir As String, ByVal findData As WIN32_FIND_DATA)
            Attributes = findData.dwFileAttributes
            CreationTimeUtc = ConvertDateTime(findData.ftCreationTime_dwHighDateTime, findData.ftCreationTime_dwLowDateTime)
            LastAccessTimeUtc = ConvertDateTime(findData.ftLastAccessTime_dwHighDateTime, findData.ftLastAccessTime_dwLowDateTime)
            LastWriteTimeUtc = ConvertDateTime(findData.ftLastWriteTime_dwHighDateTime, findData.ftLastWriteTime_dwLowDateTime)
            Size = CombineHighLowInts(findData.nFileSizeHigh, findData.nFileSizeLow)
            Name = findData.cFileName
            Path = System.IO.Path.Combine(dir, findData.cFileName)
        End Sub

        Private Shared Function CombineHighLowInts(ByVal high As UInteger, ByVal low As UInteger) As Long
            Return CULng(high) << 32 Or low
        End Function

        Private Shared Function ConvertDateTime(ByVal high As UInteger, ByVal low As UInteger) As Date
            Dim fileTime As Long = CombineHighLowInts(high, low)
            Return Date.FromFileTimeUtc(fileTime)
        End Function
    End Structure

    <Serializable, StructLayout(LayoutKind.Sequential, CharSet:=CharSet.Auto), BestFitMapping(False)>
    Friend Class WIN32_FIND_DATA
        Public dwFileAttributes As FileAttributes
        Public ftCreationTime_dwLowDateTime As UInteger
        Public ftCreationTime_dwHighDateTime As UInteger
        Public ftLastAccessTime_dwLowDateTime As UInteger
        Public ftLastAccessTime_dwHighDateTime As UInteger
        Public ftLastWriteTime_dwLowDateTime As UInteger
        Public ftLastWriteTime_dwHighDateTime As UInteger
        Public nFileSizeHigh As UInteger
        Public nFileSizeLow As UInteger
        Public dwReserved0 As Integer
        Public dwReserved1 As Integer
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=260)>
        Public cFileName As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=14)>
        Public cAlternateFileName As String

        Public Overrides Function ToString() As String
            Return "File name=" & cFileName
        End Function
    End Class

    Module FastDirectoryEnumerator
        Function EnumerateFiles(ByVal path As String) As IEnumerable(Of FileData)
            Return EnumerateFiles(path, "*")
        End Function

        Function EnumerateFiles(ByVal path As String, ByVal searchPattern As String) As IEnumerable(Of FileData)
            Return EnumerateFiles(path, searchPattern, SearchOption.TopDirectoryOnly)
        End Function

        Function EnumerateFiles(ByVal path As String, ByVal searchPattern As String, ByVal searchOption As SearchOption) As IEnumerable(Of FileData)
            If path Is Nothing Then Throw New ArgumentNullException("path")
            If searchPattern Is Nothing Then Throw New ArgumentNullException("searchPattern")
            If searchOption <> SearchOption.TopDirectoryOnly AndAlso searchOption <> SearchOption.AllDirectories Then Throw New ArgumentOutOfRangeException("searchOption")

            Dim fullPath As String = IO.Path.GetFullPath(path)
            Return New FileEnumerable(fullPath, searchPattern, searchOption)
        End Function

        Function GetFiles(ByVal path As String, ByVal searchPattern As String, ByVal searchOption As SearchOption) As FileData()
            Dim e As IEnumerable(Of FileData) = EnumerateFiles(path, searchPattern, searchOption)
            Dim list As List(Of FileData) = New List(Of FileData)(e)
            Dim retval As FileData() = New FileData(list.Count - 1) {}
            list.CopyTo(retval)
            Return retval
        End Function

        Private Class FileEnumerable
            Implements IEnumerable(Of FileData), IEnumerable
            Private ReadOnly m_path As String
            Private ReadOnly m_filter As String
            Private ReadOnly m_searchOption As SearchOption

            Public Sub New(ByVal path As String, ByVal filter As String, ByVal searchOption As System.IO.SearchOption)
                MyBase.New()
                m_path = path
                m_filter = filter
                m_searchOption = searchOption
            End Sub

            Public Function GetEnumerator() As IEnumerator(Of FileData) Implements IEnumerable(Of FileData).GetEnumerator
                Return New FileEnumerator(m_path, m_filter, m_searchOption)
            End Function

            Private Function ExplicitGetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
                Return New FileEnumerator(m_path, m_filter, m_searchOption)
            End Function
        End Class

        Private NotInheritable Class SafeFindHandle
            Inherits SafeHandleZeroOrMinusOneIsInvalid
            <SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode:=True)>
            Friend Sub New()
                MyBase.New(True)
            End Sub

            <DllImport("kernel32.dll", CharSet:=CharSet.None, ExactSpelling:=False)>
            <ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)>
            Private Shared Function FindClose(ByVal handle As IntPtr) As Boolean
            End Function

            Protected Overrides Function ReleaseHandle() As Boolean
                Return FindClose(handle)
            End Function
        End Class

        <SuppressUnmanagedCodeSecurity>
        Private Class FileEnumerator
            Implements IEnumerator(Of FileData), IDisposable, IEnumerator
            Private m_path As String
            Private ReadOnly m_filter As String
            Private ReadOnly m_searchOption As SearchOption
            Private ReadOnly m_contextStack As Stack(Of SearchContext)
            Private m_currentContext As SearchContext
            Private m_hndFindFile As SafeFindHandle
            Private ReadOnly m_win_find_data As WIN32_FIND_DATA = New WIN32_FIND_DATA()

            Public ReadOnly Property Current As FileData Implements IEnumerator(Of FileData).Current
                Get
                    Return New FileData(m_path, m_win_find_data)
                End Get
            End Property

            ReadOnly Property ExplicitCurrent As Object Implements IEnumerator.Current
                Get
                    Return New FileData(m_path, m_win_find_data)
                End Get
            End Property

            Public Sub New(ByVal path As String, ByVal filter As String, ByVal searchOption As System.IO.SearchOption)
                MyBase.New()
                m_path = path
                m_filter = filter
                m_searchOption = searchOption
                m_currentContext = New SearchContext(path)
                If Me.m_searchOption = System.IO.SearchOption.AllDirectories Then m_contextStack = New Stack(Of SearchContext)()
            End Sub

            Public Sub Dispose() Implements IDisposable.Dispose
                If m_hndFindFile IsNot Nothing Then m_hndFindFile.Dispose()
            End Sub

            <DllImport("kernel32.dll", CharSet:=CharSet.Auto, ExactSpelling:=False, SetLastError:=True)>
            Private Shared Function FindFirstFile(ByVal fileName As String, <InAttribute> <Out> ByVal data As WIN32_FIND_DATA) As SafeFindHandle
            End Function

            <DllImport("kernel32.dll", CharSet:=CharSet.Auto, ExactSpelling:=False, SetLastError:=True)>
            Private Shared Function FindNextFile(ByVal hndFindFile As SafeFindHandle, <InAttribute> <Out> ByVal lpFindFileData As WIN32_FIND_DATA) As Boolean
            End Function

            Public Function MoveNext() As Boolean Implements IEnumerator.MoveNext
                Dim flag As Boolean
                Dim retval As Boolean = False
                If m_currentContext.SubdirectoriesToProcess Is Nothing Then
                    If m_hndFindFile IsNot Nothing Then
                        retval = FindNextFile(m_hndFindFile, m_win_find_data)
                    Else
                        Dim fileIOPermission As FileIOPermission = New FileIOPermission(FileIOPermissionAccess.PathDiscovery, m_path)
                        fileIOPermission.Demand()
                        Dim searchPath As String = Path.Combine(m_path, m_filter)
                        m_hndFindFile = FindFirstFile(searchPath, m_win_find_data)
                        retval = Not m_hndFindFile.IsInvalid
                    End If
                End If
                If retval Then
                    If (Me.m_win_find_data.dwFileAttributes And FileAttributes.Directory) = FileAttributes.Directory Then
                        flag = MoveNext()
                        Return flag
                    End If
                ElseIf Me.m_searchOption = SearchOption.AllDirectories Then
                    If m_currentContext.SubdirectoriesToProcess Is Nothing Then
                        Try
                            Dim subDirectories As String() = Directory.GetDirectories(m_path)
                            m_currentContext.SubdirectoriesToProcess = New Stack(Of String)(subDirectories)
                        Catch
                        End Try
                    End If
                    If m_currentContext.SubdirectoriesToProcess IsNot Nothing AndAlso m_currentContext.SubdirectoriesToProcess.Count > 0 Then
                        Dim subDir As String = m_currentContext.SubdirectoriesToProcess.Pop()
                        m_contextStack.Push(m_currentContext)
                        m_path = subDir
                        m_hndFindFile = Nothing
                        m_currentContext = New SearchContext(m_path)
                        flag = MoveNext()
                        Return flag
                    ElseIf m_contextStack.Count > 0 Then
                        m_currentContext = m_contextStack.Pop()
                        m_path = m_currentContext.Path
                        If m_hndFindFile IsNot Nothing Then
                            m_hndFindFile.Close()
                            m_hndFindFile = Nothing
                        End If
                        flag = MoveNext()
                        Return flag
                    End If
                End If
                flag = retval
                Return flag
            End Function

            Public Sub Reset() Implements IEnumerator.Reset
                m_hndFindFile = Nothing
            End Sub

            Private Class SearchContext
                Public ReadOnly Path As String
                Public SubdirectoriesToProcess As Stack(Of String)

                Public Sub New(ByVal path As String)
                    MyBase.New()
                    Me.Path = path
                End Sub
            End Class
        End Class
    End Module
End Namespace