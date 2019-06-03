Imports System.ComponentModel
Imports System.Runtime.InteropServices

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class ProgressForm
    Inherits Hasher.Form1

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing AndAlso components IsNot Nothing Then
            components.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(ProgressForm))
        Me.SuspendLayout()
        '
        'ProgressForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.ClientSize = New System.Drawing.Size(1080, 413)
        Me.Name = "ProgressForm"
        Me.ResumeLayout(False)

    End Sub

    Private m_State As ThumbnailProgressState = ThumbnailProgressState.NoProgress
    Private m_Maximum As Integer = 100
    Private m_Value As Integer = 0

    <Browsable(True)>
    <DefaultValue(ThumbnailProgressState.NoProgress)>
    <EditorBrowsable(EditorBrowsableState.Always)>
    Public Property State As ThumbnailProgressState
        Get
            Return m_State
        End Get
        Set(ByVal value As ThumbnailProgressState)

            Select Case value
                Case ThumbnailProgressState.NoProgress, ThumbnailProgressState.Indeterminate, ThumbnailProgressState.Normal, ThumbnailProgressState.[Error], ThumbnailProgressState.Paused
                    m_State = value
                    OnStateChanged(New EventArgs())
                Case Else
                    Throw New InvalidEnumArgumentException("The value is not a member of the System.Windows.Forms.ThumbnailProgressState enumeration.")
            End Select
        End Set
    End Property

    <Browsable(True)>
    <DefaultValue(0)>
    <EditorBrowsable(EditorBrowsableState.Always)>
    Public Property Value As Integer
        Get
            Return m_Value
        End Get
        Set(ByVal value As Integer)

            If (value < 0) OrElse (value > m_Maximum) Then
                Throw New ArgumentException("The value specified is greater than the value of the System.Windows.Forms.ProgressForm.Maximum property. -or- The value specified is less than 0.")
            Else
                m_Value = value
                OnValueChanged(New EventArgs())
            End If
        End Set
    End Property

    <Browsable(True)>
    <DefaultValue(100)>
    <EditorBrowsable(EditorBrowsableState.Always)>
    Public Property Maximum As Integer
        Get
            Return m_Maximum
        End Get
        Set(ByVal value As Integer)

            If value < 0 Then
                Throw New ArgumentException("The value specified is less than 0.")
            Else
                m_Maximum = value
                If value < m_Value Then m_Value = value
                OnMaximumChanged(New EventArgs())
            End If
        End Set
    End Property

    Protected Overridable Sub OnStateChanged(ByVal e As EventArgs)
        If Windows7orGreater Then SetProgressState()
    End Sub

    Protected Overridable Sub OnValueChanged(ByVal e As EventArgs)
        If Windows7orGreater Then SetProgressValue()
    End Sub

    Protected Overridable Sub OnMaximumChanged(ByVal e As EventArgs)
        If Windows7orGreater Then SetProgressValue()
    End Sub

    Protected Overrides Sub WndProc(ByRef m As Message)
        Try
            If Windows7orGreater Then
                If m.Msg = WM_TaskbarButtonCreated Then SetProgressState()
            End If

            MyBase.WndProc(m)
        Catch ex As Exception
        End Try
    End Sub

    Private Sub SetProgressState()
        TaskbarList.SetProgressState(Handle, m_State)
        SetProgressValue()
    End Sub

    Private Sub SetProgressValue()
        Select Case m_State
            Case ThumbnailProgressState.Normal, ThumbnailProgressState.[Error], ThumbnailProgressState.Paused
                TaskbarList.SetProgressValue(Handle, CULng(m_Value), CULng(m_Maximum))
        End Select
    End Sub

    Private Shared WM_TaskbarButtonCreated As Integer = -1
    Private Shared _winVersion As Integer = -1

    Friend Shared ReadOnly Property Windows7orGreater As Boolean
        Get
            If _winVersion < 0 Then
                Dim osVersion As Version = Environment.OSVersion.Version

                If (osVersion.Major = 6 AndAlso osVersion.Minor > 0) OrElse (osVersion.Major > 6) Then
                    _winVersion = 1
                    WM_TaskbarButtonCreated = RegisterWindowMessage("TaskbarButtonCreated")
                Else
                    _winVersion = 0
                End If
            End If

            Return (_winVersion > 0)
        End Get
    End Property

    Private Shared _taskbarList As ITaskbarList3 = Nothing

    Friend Shared ReadOnly Property TaskbarList As ITaskbarList3
        Get

            If _taskbarList Is Nothing Then

                SyncLock GetType(ProgressForm)

                    If _taskbarList Is Nothing Then
                        _taskbarList = CType(New CTaskbarList(), ITaskbarList3)
                        _taskbarList.HrInit()
                    End If
                End SyncLock
            End If

            Return _taskbarList
        End Get
    End Property

    <DllImport("user32.dll")>
    Friend Shared Function RegisterWindowMessage(ByVal message As String) As Integer
    End Function

    <ComImportAttribute()>
    <GuidAttribute("ea1afb91-9e28-4b86-90e9-9e9f8a5eefaf")>
    <InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)>
    Friend Interface ITaskbarList3
        <PreserveSig>
        Sub HrInit()
        <PreserveSig>
        Sub AddTab(ByVal hwnd As IntPtr)
        <PreserveSig>
        Sub DeleteTab(ByVal hwnd As IntPtr)
        <PreserveSig>
        Sub ActivateTab(ByVal hwnd As IntPtr)
        <PreserveSig>
        Sub SetActiveAlt(ByVal hwnd As IntPtr)
        <PreserveSig>
        Sub MarkFullscreenWindow(ByVal hwnd As IntPtr,
        <MarshalAs(UnmanagedType.Bool)> ByVal fFullscreen As Boolean)
        Sub SetProgressValue(ByVal hwnd As IntPtr, ByVal ullCompleted As UInt64, ByVal ullTotal As UInt64)
        Sub SetProgressState(ByVal hwnd As IntPtr, ByVal tbpFlags As ThumbnailProgressState)
    End Interface

    <GuidAttribute("56FDF344-FD6D-11d0-958A-006097C9A090")>
    <ClassInterfaceAttribute(ClassInterfaceType.None)>
    <ComImportAttribute()>
    Friend Class CTaskbarList
    End Class
End Class

Public Enum ThumbnailProgressState
    NoProgress = 0
    Indeterminate = &H1
    Normal = &H2
    [Error] = &H4
    Paused = &H8
End Enum