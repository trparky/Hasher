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
     Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "17.6.0.0"),  _
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
        Public Property hashIndividualFilesFileNameColumnSize() As Short
            Get
                Return CType(Me("hashIndividualFilesFileNameColumnSize"),Short)
            End Get
            Set
                Me("hashIndividualFilesFileNameColumnSize") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("70")>  _
        Public Property hashIndividualFilesFileSizeColumnSize() As Short
            Get
                Return CType(Me("hashIndividualFilesFileSizeColumnSize"),Short)
            End Get
            Set
                Me("hashIndividualFilesFileSizeColumnSize") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("241")>  _
        Public Property hashIndividualFilesChecksumColumnSize() As Short
            Get
                Return CType(Me("hashIndividualFilesChecksumColumnSize"),Short)
            End Get
            Set
                Me("hashIndividualFilesChecksumColumnSize") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("557")>  _
        Public Property verifyHashFileNameColumnSize() As Short
            Get
                Return CType(Me("verifyHashFileNameColumnSize"),Short)
            End Get
            Set
                Me("verifyHashFileNameColumnSize") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("87")>  _
        Public Property verifyHashFileSizeColumnSize() As Short
            Get
                Return CType(Me("verifyHashFileSizeColumnSize"),Short)
            End Get
            Set
                Me("verifyHashFileSizeColumnSize") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("72")>  _
        Public Property verifyHashFileResults() As Short
            Get
                Return CType(Me("verifyHashFileResults"),Short)
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
        Public Property hashIndividualFilesComputeTimeColumnSize() As Short
            Get
                Return CType(Me("hashIndividualFilesComputeTimeColumnSize"),Short)
            End Get
            Set
                Me("hashIndividualFilesComputeTimeColumnSize") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("150")>  _
        Public Property verifyHashComputeTimeColumnSize() As Short
            Get
                Return CType(Me("verifyHashComputeTimeColumnSize"),Short)
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
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("4")>  _
        Public Property taskPriority() As Byte
            Get
                Return CType(Me("taskPriority"),Byte)
            End Get
            Set
                Me("taskPriority") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("True")>  _
        Public Property boolCheckForUpdates() As Boolean
            Get
                Return CType(Me("boolCheckForUpdates"),Boolean)
            End Get
            Set
                Me("boolCheckForUpdates") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("True")>  _
        Public Property boolAutoAddExtension() As Boolean
            Get
                Return CType(Me("boolAutoAddExtension"),Boolean)
            End Get
            Set
                Me("boolAutoAddExtension") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("2")>  _
        Public Property roundFileSizes() As Byte
            Get
                Return CType(Me("roundFileSizes"),Byte)
            End Get
            Set
                Me("roundFileSizes") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("2")>  _
        Public Property roundPercentages() As Byte
            Get
                Return CType(Me("roundPercentages"),Byte)
            End Get
            Set
                Me("roundPercentages") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("0, 0")>  _
        Public Property windowLocation() As Global.System.Drawing.Point
            Get
                Return CType(Me("windowLocation"),Global.System.Drawing.Point)
            End Get
            Set
                Me("windowLocation") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("120")>  _
        Public Property newHashChecksumColumnSize() As Short
            Get
                Return CType(Me("newHashChecksumColumnSize"),Short)
            End Get
            Set
                Me("newHashChecksumColumnSize") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("False")>  _
        Public Property boolDisplayValidChecksumString() As Boolean
            Get
                Return CType(Me("boolDisplayValidChecksumString"),Boolean)
            End Get
            Set
                Me("boolDisplayValidChecksumString") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("True")>  _
        Public Property boolOpenInExplorer() As Boolean
            Get
                Return CType(Me("boolOpenInExplorer"),Boolean)
            End Get
            Set
                Me("boolOpenInExplorer") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("True")>  _
        Public Property boolShowPercentageInWindowTitleBar() As Boolean
            Get
                Return CType(Me("boolShowPercentageInWindowTitleBar"),Boolean)
            End Get
            Set
                Me("boolShowPercentageInWindowTitleBar") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("2")>  _
        Public Property defaultHash() As Byte
            Get
                Return CType(Me("defaultHash"),Byte)
            End Get
            Set
                Me("defaultHash") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("False")>  _
        Public Property boolWindowMaximized() As Boolean
            Get
                Return CType(Me("boolWindowMaximized"),Boolean)
            End Get
            Set
                Me("boolWindowMaximized") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("True")>  _
        Public Property boolShowFileProgressInFileList() As Boolean
            Get
                Return CType(Me("boolShowFileProgressInFileList"),Boolean)
            End Get
            Set
                Me("boolShowFileProgressInFileList") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("True")>  _
        Public Property boolIncludeEntryCountInFileNameHeader() As Boolean
            Get
                Return CType(Me("boolIncludeEntryCountInFileNameHeader"),Boolean)
            End Get
            Set
                Me("boolIncludeEntryCountInFileNameHeader") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("True")>  _
        Public Property boolComputeHashesOnCompareFilesTabEvenWithDifferentFileSizes() As Boolean
            Get
                Return CType(Me("boolComputeHashesOnCompareFilesTabEvenWithDifferentFileSizes"),Boolean)
            End Get
            Set
                Me("boolComputeHashesOnCompareFilesTabEvenWithDifferentFileSizes") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("True")>  _
        Public Property boolClearBeforeTransferringFromVerifyToHash() As Boolean
            Get
                Return CType(Me("boolClearBeforeTransferringFromVerifyToHash"),Boolean)
            End Get
            Set
                Me("boolClearBeforeTransferringFromVerifyToHash") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute()>  _
        Public Property listFilesColumnOrder() As Global.System.Collections.Specialized.StringCollection
            Get
                Return CType(Me("listFilesColumnOrder"),Global.System.Collections.Specialized.StringCollection)
            End Get
            Set
                Me("listFilesColumnOrder") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute()>  _
        Public Property verifyListFilesColumnOrder() As Global.System.Collections.Specialized.StringCollection
            Get
                Return CType(Me("verifyListFilesColumnOrder"),Global.System.Collections.Specialized.StringCollection)
            End Get
            Set
                Me("verifyListFilesColumnOrder") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("False")>  _
        Public Property boolUpdateColorInRealTime() As Boolean
            Get
                Return CType(Me("boolUpdateColorInRealTime"),Boolean)
            End Get
            Set
                Me("boolUpdateColorInRealTime") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("549, 236")>  _
        Public Property exceptionViewerWindowSize() As Global.System.Drawing.Size
            Get
                Return CType(Me("exceptionViewerWindowSize"),Global.System.Drawing.Size)
            End Get
            Set
                Me("exceptionViewerWindowSize") = value
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
