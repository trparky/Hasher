Imports System.Web.Script.Serialization

Public Module SavedAppSettingsModule
    Public Sub SaveApplicationSettingsToFile(strFileName As String)
        Using streamWriter As New IO.StreamWriter(strFileName)
            Dim json As JavaScriptSerializer = New JavaScriptSerializer()
            streamWriter.Write(json.Serialize(ExportApplicationSettings()))
        End Using
    End Sub

    Public Sub LoadApplicationSettingsFromFile(strFileName As String)
        Dim SavedAppSettingsInstance As New SavedAppSettings

        Using streamReader As New IO.StreamReader(strFileName)
            Dim json As JavaScriptSerializer = New JavaScriptSerializer()
            SavedAppSettingsInstance = json.Deserialize(Of SavedAppSettings)(streamReader.ReadToEnd.Trim)
        End Using

        ImportApplicationSettings(SavedAppSettingsInstance)
    End Sub

    Private Function ExportApplicationSettings() As SavedAppSettings
        Dim SavedAppSettings As New SavedAppSettings With {
            .boolAutoAddExtension = My.Settings.boolAutoAddExtension,
            .boolCheckForUpdates = My.Settings.boolCheckForUpdates,
            .boolComputeHashesOnCompareFilesTabEvenWithDifferentFileSizes = My.Settings.boolComputeHashesOnCompareFilesTabEvenWithDifferentFileSizes,
            .boolDisplayHashesInUpperCase = My.Settings.boolDisplayHashesInUpperCase,
            .boolDisplayValidChecksumString = My.Settings.boolDisplayValidChecksumString,
            .boolIncludeEntryCountInFileNameHeader = My.Settings.boolIncludeEntryCountInFileNameHeader,
            .boolOpenInExplorer = My.Settings.boolOpenInExplorer,
            .boolRecurrsiveDirectorySearch = My.Settings.boolRecurrsiveDirectorySearch,
            .boolSaveChecksumFilesWithRelativePaths = My.Settings.boolSaveChecksumFilesWithRelativePaths,
            .boolShowFileProgressInFileList = My.Settings.boolShowFileProgressInFileList,
            .boolShowPercentageInWindowTitleBar = My.Settings.boolShowPercentageInWindowTitleBar,
            .boolSortByFileSizeAfterLoadingHashFile = My.Settings.boolSortByFileSizeAfterLoadingHashFile,
            .boolSortFileListingAfterAddingFilesToHash = My.Settings.boolSortFileListingAfterAddingFilesToHash,
            .boolUseCommasInNumbers = My.Settings.boolUseCommasInNumbers,
            .boolUseMilliseconds = My.Settings.boolUseMilliseconds,
            .boolWindowMaximized = My.Settings.boolWindowMaximized,
            .defaultHash = My.Settings.defaultHash,
            .fileNotFoundColor = My.Settings.fileNotFoundColor.ToArgb,
            .hashIndividualFilesChecksumColumnSize = My.Settings.hashIndividualFilesChecksumColumnSize,
            .hashIndividualFilesComputeTimeColumnSize = My.Settings.hashIndividualFilesComputeTimeColumnSize,
            .hashIndividualFilesFileNameColumnSize = My.Settings.hashIndividualFilesFileNameColumnSize,
            .hashIndividualFilesFileSizeColumnSize = My.Settings.hashIndividualFilesFileSizeColumnSize,
            .newHashChecksumColumnSize = My.Settings.newHashChecksumColumnSize,
            .notValidColor = My.Settings.notValidColor.ToArgb,
            .roundFileSizes = My.Settings.roundFileSizes,
            .roundPercentages = My.Settings.roundPercentages,
            .shortBufferSize = My.Settings.shortBufferSize,
            .taskPriority = My.Settings.taskPriority,
            .validColor = My.Settings.validColor.ToArgb,
            .verifyHashComputeTimeColumnSize = My.Settings.verifyHashComputeTimeColumnSize,
            .verifyHashFileNameColumnSize = My.Settings.verifyHashFileNameColumnSize,
            .verifyHashFileResults = My.Settings.verifyHashFileResults,
            .verifyHashFileSizeColumnSize = My.Settings.verifyHashFileSizeColumnSize,
            .windowLocation = My.Settings.windowLocation,
            .windowSize = My.Settings.windowSize
        }
        Return SavedAppSettings
    End Function

    Private Sub ImportApplicationSettings(SavedAppSettings As SavedAppSettings)
        With My.Settings
            .boolAutoAddExtension = SavedAppSettings.boolAutoAddExtension
            .boolCheckForUpdates = SavedAppSettings.boolCheckForUpdates
            .boolComputeHashesOnCompareFilesTabEvenWithDifferentFileSizes = SavedAppSettings.boolComputeHashesOnCompareFilesTabEvenWithDifferentFileSizes
            .boolDisplayHashesInUpperCase = SavedAppSettings.boolDisplayHashesInUpperCase
            .boolDisplayValidChecksumString = SavedAppSettings.boolDisplayValidChecksumString
            .boolIncludeEntryCountInFileNameHeader = SavedAppSettings.boolIncludeEntryCountInFileNameHeader
            .boolOpenInExplorer = SavedAppSettings.boolOpenInExplorer
            .boolRecurrsiveDirectorySearch = SavedAppSettings.boolRecurrsiveDirectorySearch
            .boolSaveChecksumFilesWithRelativePaths = SavedAppSettings.boolSaveChecksumFilesWithRelativePaths
            .boolShowFileProgressInFileList = SavedAppSettings.boolShowFileProgressInFileList
            .boolShowPercentageInWindowTitleBar = SavedAppSettings.boolShowPercentageInWindowTitleBar
            .boolSortByFileSizeAfterLoadingHashFile = SavedAppSettings.boolSortByFileSizeAfterLoadingHashFile
            .boolSortFileListingAfterAddingFilesToHash = SavedAppSettings.boolSortFileListingAfterAddingFilesToHash
            .boolUseCommasInNumbers = SavedAppSettings.boolUseCommasInNumbers
            .boolUseMilliseconds = SavedAppSettings.boolUseMilliseconds
            .boolWindowMaximized = SavedAppSettings.boolWindowMaximized
            .defaultHash = SavedAppSettings.defaultHash
            .fileNotFoundColor = Color.FromArgb(SavedAppSettings.fileNotFoundColor)
            .hashIndividualFilesChecksumColumnSize = SavedAppSettings.hashIndividualFilesChecksumColumnSize
            .hashIndividualFilesComputeTimeColumnSize = SavedAppSettings.hashIndividualFilesComputeTimeColumnSize
            .hashIndividualFilesFileNameColumnSize = SavedAppSettings.hashIndividualFilesFileNameColumnSize
            .hashIndividualFilesFileSizeColumnSize = SavedAppSettings.hashIndividualFilesFileSizeColumnSize
            .newHashChecksumColumnSize = SavedAppSettings.newHashChecksumColumnSize
            .notValidColor = Color.FromArgb(SavedAppSettings.notValidColor)
            .roundFileSizes = SavedAppSettings.roundFileSizes
            .roundPercentages = SavedAppSettings.roundPercentages
            .shortBufferSize = SavedAppSettings.shortBufferSize
            .taskPriority = SavedAppSettings.taskPriority
            .validColor = Color.FromArgb(SavedAppSettings.validColor)
            .verifyHashComputeTimeColumnSize = SavedAppSettings.verifyHashComputeTimeColumnSize
            .verifyHashFileNameColumnSize = SavedAppSettings.verifyHashFileNameColumnSize
            .verifyHashFileResults = SavedAppSettings.verifyHashFileResults
            .verifyHashFileSizeColumnSize = SavedAppSettings.verifyHashFileSizeColumnSize
            .windowLocation = SavedAppSettings.windowLocation
            .windowSize = SavedAppSettings.windowSize
        End With
    End Sub

    Private Structure SavedAppSettings
        Public boolAutoAddExtension As Boolean
        Public boolCheckForUpdates As Boolean
        Public boolComputeHashesOnCompareFilesTabEvenWithDifferentFileSizes As Boolean
        Public boolDisplayHashesInUpperCase As Boolean
        Public boolDisplayValidChecksumString As Boolean
        Public boolIncludeEntryCountInFileNameHeader As Boolean
        Public boolOpenInExplorer As Boolean
        Public boolRecurrsiveDirectorySearch As Boolean
        Public boolSaveChecksumFilesWithRelativePaths As Boolean
        Public boolShowFileProgressInFileList As Boolean
        Public boolShowPercentageInWindowTitleBar As Boolean
        Public boolSortByFileSizeAfterLoadingHashFile As Boolean
        Public boolSortFileListingAfterAddingFilesToHash As Boolean
        Public boolUseCommasInNumbers As Boolean
        Public boolUseMilliseconds As Boolean
        Public boolWindowMaximized As Boolean
        Public defaultHash As Byte
        Public fileNotFoundColor As Integer
        Public hashIndividualFilesChecksumColumnSize As Short
        Public hashIndividualFilesComputeTimeColumnSize As Short
        Public hashIndividualFilesFileNameColumnSize As Short
        Public hashIndividualFilesFileSizeColumnSize As Short
        Public newHashChecksumColumnSize As Short
        Public notValidColor As Integer
        Public roundFileSizes As Byte
        Public roundPercentages As Byte
        Public shortBufferSize As Short
        Public taskPriority As Byte
        Public validColor As Integer
        Public verifyHashComputeTimeColumnSize As Short
        Public verifyHashFileNameColumnSize As Short
        Public verifyHashFileResults As Short
        Public verifyHashFileSizeColumnSize As Short
        Public windowLocation As Point
        Public windowSize As Size
    End Structure
End Module