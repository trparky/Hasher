﻿'------------------------------------------------------------------------------
' <auto-generated>
'     This code was generated by a tool.
'     Runtime Version:4.0.30319.42000
'
'     Changes to this file may cause incorrect behavior and will be lost if
'     the code is regenerated.
' </auto-generated>
'------------------------------------------------------------------------------

Option Strict On
Option Explicit On


Namespace My
    
    <Global.System.Runtime.CompilerServices.CompilerGeneratedAttribute(),  _
     Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "16.2.0.0"),  _
     Global.System.ComponentModel.EditorBrowsableAttribute(Global.System.ComponentModel.EditorBrowsableState.Advanced)>  _
    Partial Friend NotInheritable Class MySettings
        Inherits Global.System.Configuration.ApplicationSettingsBase
        
        Private Shared defaultInstance As MySettings = CType(Global.System.Configuration.ApplicationSettingsBase.Synchronized(New MySettings()),MySettings)
        
#Region "My.Settings Auto-Save Functionality"
#If _MyType = "WindowsForms" Then
    Private Shared addedHandler As Boolean

    Private Shared addedHandlerLockObject As New Object

    <Global.System.Diagnostics.DebuggerNonUserCodeAttribute(), Global.System.ComponentModel.EditorBrowsableAttribute(Global.System.ComponentModel.EditorBrowsableState.Advanced)> _
    Private Shared Sub AutoSaveSettings(sender As Global.System.Object, e As Global.System.EventArgs)
        If My.Application.SaveMySettingsOnExit Then
            My.Settings.Save()
        End If
    End Sub
#End If
#End Region
        
        Public Shared ReadOnly Property [Default]() As MySettings
            Get
                
#If _MyType = "WindowsForms" Then
               If Not addedHandler Then
                    SyncLock addedHandlerLockObject
                        If Not addedHandler Then
                            AddHandler My.Application.Shutdown, AddressOf AutoSaveSettings
                            addedHandler = True
                        End If
                    End SyncLock
                End If
#End If
                Return defaultInstance
            End Get
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("True")>  _
        Public Property boolRecurrsiveDirectorySearch() As Boolean
            Get
                Return CType(Me("boolRecurrsiveDirectorySearch"),Boolean)
            End Get
            Set
                Me("boolRecurrsiveDirectorySearch") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("True")>  _
        Public Property boolSSL() As Boolean
            Get
                Return CType(Me("boolSSL"),Boolean)
            End Get
            Set
                Me("boolSSL") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("1088, 446")>  _
        Public Property windowSize() As Global.System.Drawing.Size
            Get
                Return CType(Me("windowSize"),Global.System.Drawing.Size)
            End Get
            Set
                Me("windowSize") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("528")>  _
        Public Property hashIndividualFilesFileNameColumnSize() As Integer
            Get
                Return CType(Me("hashIndividualFilesFileNameColumnSize"),Integer)
            End Get
            Set
                Me("hashIndividualFilesFileNameColumnSize") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("70")>  _
        Public Property hashIndividualFilesFileSizeColumnSize() As Integer
            Get
                Return CType(Me("hashIndividualFilesFileSizeColumnSize"),Integer)
            End Get
            Set
                Me("hashIndividualFilesFileSizeColumnSize") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("241")>  _
        Public Property hashIndividualFilesChecksumColumnSize() As Integer
            Get
                Return CType(Me("hashIndividualFilesChecksumColumnSize"),Integer)
            End Get
            Set
                Me("hashIndividualFilesChecksumColumnSize") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("557")>  _
        Public Property verifyHashFileNameColumnSize() As Integer
            Get
                Return CType(Me("verifyHashFileNameColumnSize"),Integer)
            End Get
            Set
                Me("verifyHashFileNameColumnSize") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("87")>  _
        Public Property verifyHashFileSizeColumnSize() As Integer
            Get
                Return CType(Me("verifyHashFileSizeColumnSize"),Integer)
            End Get
            Set
                Me("verifyHashFileSizeColumnSize") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("72")>  _
        Public Property verifyHashFileResults() As Integer
            Get
                Return CType(Me("verifyHashFileResults"),Integer)
            End Get
            Set
                Me("verifyHashFileResults") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("True")>  _
        Public Property boolSortByFileSizeAfterLoadingHashFile() As Boolean
            Get
                Return CType(Me("boolSortByFileSizeAfterLoadingHashFile"),Boolean)
            End Get
            Set
                Me("boolSortByFileSizeAfterLoadingHashFile") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("True")>  _
        Public Property boolSaveChecksumFilesWithRelativePaths() As Boolean
            Get
                Return CType(Me("boolSaveChecksumFilesWithRelativePaths"),Boolean)
            End Get
            Set
                Me("boolSaveChecksumFilesWithRelativePaths") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("True")>  _
        Public Property boolSortFileListingAfterAddingFilesToHash() As Boolean
            Get
                Return CType(Me("boolSortFileListingAfterAddingFilesToHash"),Boolean)
            End Get
            Set
                Me("boolSortFileListingAfterAddingFilesToHash") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("150")>  _
        Public Property hashIndividualFilesComputeTimeColumnSize() As Integer
            Get
                Return CType(Me("hashIndividualFilesComputeTimeColumnSize"),Integer)
            End Get
            Set
                Me("hashIndividualFilesComputeTimeColumnSize") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("150")>  _
        Public Property verifyHashComputeTimeColumnSize() As String
            Get
                Return CType(Me("verifyHashComputeTimeColumnSize"),String)
            End Get
            Set
                Me("verifyHashComputeTimeColumnSize") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("True")>  _
        Public Property boolUseMilliseconds() As Boolean
            Get
                Return CType(Me("boolUseMilliseconds"),Boolean)
            End Get
            Set
                Me("boolUseMilliseconds") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("True")>  _
        Public Property boolDisplayHashesInUpperCase() As Boolean
            Get
                Return CType(Me("boolDisplayHashesInUpperCase"),Boolean)
            End Get
            Set
                Me("boolDisplayHashesInUpperCase") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("LightGreen")>  _
        Public Property validColor() As Global.System.Drawing.Color
            Get
                Return CType(Me("validColor"),Global.System.Drawing.Color)
            End Get
            Set
                Me("validColor") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("Pink")>  _
        Public Property notValidColor() As Global.System.Drawing.Color
            Get
                Return CType(Me("notValidColor"),Global.System.Drawing.Color)
            End Get
            Set
                Me("notValidColor") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("LightGray")>  _
        Public Property fileNotFoundColor() As Global.System.Drawing.Color
            Get
                Return CType(Me("fileNotFoundColor"),Global.System.Drawing.Color)
            End Get
            Set
                Me("fileNotFoundColor") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("False")>  _
        Public Property boolShowEstimatedTime() As Boolean
            Get
                Return CType(Me("boolShowEstimatedTime"),Boolean)
            End Get
            Set
                Me("boolShowEstimatedTime") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("False")>  _
        Public Property boolUseTaskBarProgressBarForOverallStatus() As Boolean
            Get
                Return CType(Me("boolUseTaskBarProgressBarForOverallStatus"),Boolean)
            End Get
            Set
                Me("boolUseTaskBarProgressBarForOverallStatus") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("2")>  _
        Public Property shortBufferSize() As Short
            Get
                Return CType(Me("shortBufferSize"),Short)
            End Get
            Set
                Me("shortBufferSize") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("True")>  _
        Public Property boolUseCommasInNumbers() As Boolean
            Get
                Return CType(Me("boolUseCommasInNumbers"),Boolean)
            End Get
            Set
                Me("boolUseCommasInNumbers") = value
            End Set
        End Property
    End Class
End Namespace

Namespace My
    
    <Global.Microsoft.VisualBasic.HideModuleNameAttribute(),  _
     Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
     Global.System.Runtime.CompilerServices.CompilerGeneratedAttribute()>  _
    Friend Module MySettingsProperty
        
        <Global.System.ComponentModel.Design.HelpKeywordAttribute("My.Settings")>  _
        Friend ReadOnly Property Settings() As Global.Hasher.My.MySettings
            Get
                Return Global.Hasher.My.MySettings.Default
            End Get
        End Property
    End Module
End Namespace
