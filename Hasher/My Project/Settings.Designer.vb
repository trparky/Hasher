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
     Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "15.9.0.0"),  _
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
        Public Property boolEnableServer() As Boolean
            Get
                Return CType(Me("boolEnableServer"),Boolean)
            End Get
            Set
                Me("boolEnableServer") = value
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
