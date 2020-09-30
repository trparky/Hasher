<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Form1))
        Me.TabControl1 = New System.Windows.Forms.TabControl()
        Me.tabWelcome = New System.Windows.Forms.TabPage()
        Me.btnDonate = New System.Windows.Forms.Button()
        Me.lblDownloadNotification = New System.Windows.Forms.Label()
        Me.btnCheckForUpdates = New System.Windows.Forms.Button()
        Me.lblWelcomeText = New System.Windows.Forms.Label()
        Me.tabHashText = New System.Windows.Forms.TabPage()
        Me.btnCopyTextHashResultsToClipboard = New System.Windows.Forms.Button()
        Me.txtHashResults = New System.Windows.Forms.TextBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.btnPasteTextFromWindowsClipboard = New System.Windows.Forms.Button()
        Me.btnComputeTextHash = New System.Windows.Forms.Button()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.textRadioMD5 = New System.Windows.Forms.RadioButton()
        Me.textRadioSHA512 = New System.Windows.Forms.RadioButton()
        Me.textRadioSHA384 = New System.Windows.Forms.RadioButton()
        Me.textRadioSHA256 = New System.Windows.Forms.RadioButton()
        Me.textRadioSHA1 = New System.Windows.Forms.RadioButton()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.lblHashTextStep1 = New System.Windows.Forms.Label()
        Me.txtTextToHash = New System.Windows.Forms.TextBox()
        Me.lblTextToHash = New System.Windows.Forms.Label()
        Me.tabHashIndividualFiles = New System.Windows.Forms.TabPage()
        Me.hashIndividualFilesTableLayoutControl = New System.Windows.Forms.TableLayoutPanel()
        Me.lblIndividualFilesStatusProcessingFile = New System.Windows.Forms.Label()
        Me.lblProcessingFile = New System.Windows.Forms.Label()
        Me.IndividualFilesProgressBar = New System.Windows.Forms.ProgressBar()
        Me.hashIndividualFilesAllFilesProgressBar = New System.Windows.Forms.ProgressBar()
        Me.lblHashIndividualFilesTotalStatus = New System.Windows.Forms.Label()
        Me.lblIndividualFilesStatus = New System.Windows.Forms.Label()
        Me.lblLine = New System.Windows.Forms.Label()
        Me.lblHashIndividualFilesStep3 = New System.Windows.Forms.Label()
        Me.lblHashIndividualFilesStep2 = New System.Windows.Forms.Label()
        Me.btnIndividualFilesSaveResultsToDisk = New System.Windows.Forms.Button()
        Me.btnIndividualFilesCopyToClipboard = New System.Windows.Forms.Button()
        Me.btnComputeHash = New System.Windows.Forms.Button()
        Me.radioMD5 = New System.Windows.Forms.RadioButton()
        Me.radioSHA512 = New System.Windows.Forms.RadioButton()
        Me.radioSHA384 = New System.Windows.Forms.RadioButton()
        Me.radioSHA256 = New System.Windows.Forms.RadioButton()
        Me.radioSHA1 = New System.Windows.Forms.RadioButton()
        Me.lblHashIndividualFilesStep1 = New System.Windows.Forms.Label()
        Me.listFiles = New System.Windows.Forms.ListView()
        Me.colFileName = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.colFileSize = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.colChecksum = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.colComputeTime = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.listFilesContextMenu = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.CopyHashToClipboardToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.btnRemoveSelectedFiles = New System.Windows.Forms.Button()
        Me.btnRemoveAllFiles = New System.Windows.Forms.Button()
        Me.btnAddFilesInFolder = New System.Windows.Forms.Button()
        Me.btnAddIndividualFiles = New System.Windows.Forms.Button()
        Me.tabVerifySavedHashes = New System.Windows.Forms.TabPage()
        Me.verifySavedHashesTableLayoutControl = New System.Windows.Forms.TableLayoutPanel()
        Me.VerifyHashProgressBar = New System.Windows.Forms.ProgressBar()
        Me.lblVerifyHashStatusProcessingFile = New System.Windows.Forms.Label()
        Me.verifyIndividualFilesAllFilesProgressBar = New System.Windows.Forms.ProgressBar()
        Me.lblVerifyHashesTotalStatus = New System.Windows.Forms.Label()
        Me.lblVerifyHashStatus = New System.Windows.Forms.Label()
        Me.lblProcessingFileVerify = New System.Windows.Forms.Label()
        Me.lblVerifyFileNameLabel = New System.Windows.Forms.Label()
        Me.verifyHashesListFiles = New System.Windows.Forms.ListView()
        Me.colFile = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.colFileSize2 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.colResults = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.colComputeTime2 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.colNewHash = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.verifyListFilesContextMenu = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.ViewChecksumDifferenceToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.btnOpenExistingHashFile = New System.Windows.Forms.Button()
        Me.btnTransferToHashIndividualFilesTab = New System.Windows.Forms.Button()
        Me.tabCompareFiles = New System.Windows.Forms.TabPage()
        Me.compareFilesTableLayoutControl = New System.Windows.Forms.TableLayoutPanel()
        Me.lblCompareFilesStatus = New System.Windows.Forms.Label()
        Me.compareFilesProgressBar = New System.Windows.Forms.ProgressBar()
        Me.CompareFilesAllFilesProgress = New System.Windows.Forms.ProgressBar()
        Me.lblCompareFilesAllFilesStatus = New System.Windows.Forms.Label()
        Me.lblFile2Hash = New System.Windows.Forms.Label()
        Me.lblFile1Hash = New System.Windows.Forms.Label()
        Me.btnCompareFiles = New System.Windows.Forms.Button()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.compareRadioMD5 = New System.Windows.Forms.RadioButton()
        Me.compareRadioSHA512 = New System.Windows.Forms.RadioButton()
        Me.compareRadioSHA384 = New System.Windows.Forms.RadioButton()
        Me.compareRadioSHA256 = New System.Windows.Forms.RadioButton()
        Me.compareRadioSHA1 = New System.Windows.Forms.RadioButton()
        Me.btnCompareFilesBrowseFile2 = New System.Windows.Forms.Button()
        Me.btnCompareFilesBrowseFile1 = New System.Windows.Forms.Button()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.txtFile2 = New System.Windows.Forms.TextBox()
        Me.txtFile1 = New System.Windows.Forms.TextBox()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.tabCompareAgainstKnownHash = New System.Windows.Forms.TabPage()
        Me.pictureBoxVerifyAgainstResults = New System.Windows.Forms.PictureBox()
        Me.lblCompareFileAgainstKnownHashType = New System.Windows.Forms.Label()
        Me.compareAgainstKnownHashProgressBar = New System.Windows.Forms.ProgressBar()
        Me.lblCompareAgainstKnownHashStatus = New System.Windows.Forms.Label()
        Me.btnCompareAgainstKnownHash = New System.Windows.Forms.Button()
        Me.txtKnownHash = New System.Windows.Forms.TextBox()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.btnBrowseFileForCompareKnownHash = New System.Windows.Forms.Button()
        Me.txtFileForKnownHash = New System.Windows.Forms.TextBox()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.tabSettings = New System.Windows.Forms.TabPage()
        Me.btnSetBufferSize = New System.Windows.Forms.Button()
        Me.bufferSize = New System.Windows.Forms.NumericUpDown()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.chkUseMilliseconds = New System.Windows.Forms.CheckBox()
        Me.chkSortFileListingAfterAddingFilesToHash = New System.Windows.Forms.CheckBox()
        Me.chkSaveChecksumFilesWithRelativePaths = New System.Windows.Forms.CheckBox()
        Me.chkSortByFileSizeAfterLoadingHashFile = New System.Windows.Forms.CheckBox()
        Me.btnAddHasherToAllFiles = New System.Windows.Forms.Button()
        Me.btnAssociate = New System.Windows.Forms.Button()
        Me.chkSSL = New System.Windows.Forms.CheckBox()
        Me.chkRecurrsiveDirectorySearch = New System.Windows.Forms.CheckBox()
        Me.chkDisplayHashesInUpperCase = New System.Windows.Forms.CheckBox()
        Me.btnFileNotFoundColor = New System.Windows.Forms.Button()
        Me.btnSetColorsBackToDefaults = New System.Windows.Forms.Button()
        Me.btnSetNotValidColor = New System.Windows.Forms.Button()
        Me.btnSetValidColor = New System.Windows.Forms.Button()
        Me.lblFileNotFoundColor = New System.Windows.Forms.Label()
        Me.lblNotValidColor = New System.Windows.Forms.Label()
        Me.lblValidColor = New System.Windows.Forms.Label()
        Me.btnPerformBenchmark = New System.Windows.Forms.Button()
        Me.chkUseCommasInNumbers = New System.Windows.Forms.CheckBox()
        Me.taskPriority = New System.Windows.Forms.ComboBox()
        Me.lblTaskPriorityLabel = New System.Windows.Forms.Label()
        Me.chkCheckForUpdates = New System.Windows.Forms.CheckBox()
        Me.chkAutoAddExtension = New System.Windows.Forms.CheckBox()
        Me.btnSetRoundPercentages = New System.Windows.Forms.Button()
        Me.roundPercentages = New System.Windows.Forms.NumericUpDown()
        Me.lblRoundPercentagesLabel = New System.Windows.Forms.Label()
        Me.btnSetRoundFileSizes = New System.Windows.Forms.Button()
        Me.roundFileSizes = New System.Windows.Forms.NumericUpDown()
        Me.lblRoundFileSizesLabel = New System.Windows.Forms.Label()
        Me.OpenFileDialog = New System.Windows.Forms.OpenFileDialog()
        Me.SaveFileDialog = New System.Windows.Forms.SaveFileDialog()
        Me.FolderBrowserDialog = New System.Windows.Forms.FolderBrowserDialog()
        Me.ToolTip = New System.Windows.Forms.ToolTip(Me.components)
        Me.pictureBoxCompareFiles = New System.Windows.Forms.PictureBox()
        Me.chkShowPercentageInWindowTitleBar = New System.Windows.Forms.CheckBox()
        Me.btnRetestFailedFiles = New System.Windows.Forms.Button()
        Me.lblDefaultHashLabel = New System.Windows.Forms.Label()
        Me.defaultHashType = New System.Windows.Forms.ComboBox()
        Me.TabControl1.SuspendLayout()
        Me.tabWelcome.SuspendLayout()
        Me.tabHashText.SuspendLayout()
        Me.tabHashIndividualFiles.SuspendLayout()
        Me.hashIndividualFilesTableLayoutControl.SuspendLayout()
        Me.listFilesContextMenu.SuspendLayout()
        Me.tabVerifySavedHashes.SuspendLayout()
        Me.chkOpenInExplorer = New System.Windows.Forms.CheckBox()
        Me.verifySavedHashesTableLayoutControl.SuspendLayout()
        Me.chkDisplayValidChecksumString = New System.Windows.Forms.CheckBox()
        Me.verifyListFilesContextMenu.SuspendLayout()
        Me.tabCompareFiles.SuspendLayout()
        CType(Me.pictureBoxCompareFiles, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.compareFilesTableLayoutControl.SuspendLayout()
        Me.tabCompareAgainstKnownHash.SuspendLayout()
        CType(Me.pictureBoxVerifyAgainstResults, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.tabSettings.SuspendLayout()
        CType(Me.bufferSize, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.roundPercentages, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.roundFileSizes, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'TabControl1
        '
        Me.TabControl1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TabControl1.Controls.Add(Me.tabWelcome)
        Me.TabControl1.Controls.Add(Me.tabHashText)
        Me.TabControl1.Controls.Add(Me.tabHashIndividualFiles)
        Me.TabControl1.Controls.Add(Me.tabVerifySavedHashes)
        Me.TabControl1.Controls.Add(Me.tabCompareFiles)
        Me.TabControl1.Controls.Add(Me.tabCompareAgainstKnownHash)
        Me.TabControl1.Controls.Add(Me.tabSettings)
        Me.TabControl1.Location = New System.Drawing.Point(12, 12)
        Me.TabControl1.Name = "TabControl1"
        Me.TabControl1.SelectedIndex = 0
        Me.TabControl1.Size = New System.Drawing.Size(1048, 389)
        Me.TabControl1.TabIndex = 0
        '
        'tabWelcome
        '
        Me.tabWelcome.BackColor = System.Drawing.SystemColors.Control
        Me.tabWelcome.Controls.Add(Me.btnDonate)
        Me.tabWelcome.Controls.Add(Me.lblDownloadNotification)
        Me.tabWelcome.Controls.Add(Me.btnCheckForUpdates)
        Me.tabWelcome.Controls.Add(Me.lblWelcomeText)
        Me.tabWelcome.Location = New System.Drawing.Point(4, 22)
        Me.tabWelcome.Name = "tabWelcome"
        Me.tabWelcome.Padding = New System.Windows.Forms.Padding(3)
        Me.tabWelcome.Size = New System.Drawing.Size(1040, 363)
        Me.tabWelcome.TabIndex = 0
        Me.tabWelcome.Text = "Welcome"
        '
        'btnDonate
        '
        Me.btnDonate.Image = Global.Hasher.My.Resources.Resources.green_dollar
        Me.btnDonate.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnDonate.Location = New System.Drawing.Point(19, 119)
        Me.btnDonate.Name = "btnDonate"
        Me.btnDonate.Size = New System.Drawing.Size(163, 39)
        Me.btnDonate.TabIndex = 9
        Me.btnDonate.Text = "Donate Money to Developer"
        Me.btnDonate.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.btnDonate.UseVisualStyleBackColor = True
        '
        'lblDownloadNotification
        '
        Me.lblDownloadNotification.AutoSize = True
        Me.lblDownloadNotification.Location = New System.Drawing.Point(188, 74)
        Me.lblDownloadNotification.Name = "lblDownloadNotification"
        Me.lblDownloadNotification.Size = New System.Drawing.Size(133, 13)
        Me.lblDownloadNotification.TabIndex = 8
        Me.lblDownloadNotification.Text = "(No Download in Progress)"
        Me.lblDownloadNotification.Visible = False
        '
        'btnCheckForUpdates
        '
        Me.btnCheckForUpdates.Location = New System.Drawing.Point(19, 74)
        Me.btnCheckForUpdates.Name = "btnCheckForUpdates"
        Me.btnCheckForUpdates.Size = New System.Drawing.Size(163, 39)
        Me.btnCheckForUpdates.TabIndex = 7
        Me.btnCheckForUpdates.Text = "Check for Updates"
        Me.btnCheckForUpdates.UseVisualStyleBackColor = True
        '
        'lblWelcomeText
        '
        Me.lblWelcomeText.AutoSize = True
        Me.lblWelcomeText.Location = New System.Drawing.Point(16, 19)
        Me.lblWelcomeText.Name = "lblWelcomeText"
        Me.lblWelcomeText.Size = New System.Drawing.Size(373, 52)
        Me.lblWelcomeText.TabIndex = 0
        Me.lblWelcomeText.Text = "Welcome to Hasher, the only hash program you need." & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Hasher version {0}        (Ru" &
    "nning in {1}-bit mode on a {2}-bit operating system)" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Written by Tom Parkison." &
    ""
        '
        'tabHashText
        '
        Me.tabHashText.BackColor = System.Drawing.SystemColors.Control
        Me.tabHashText.Controls.Add(Me.btnCopyTextHashResultsToClipboard)
        Me.tabHashText.Controls.Add(Me.txtHashResults)
        Me.tabHashText.Controls.Add(Me.Label3)
        Me.tabHashText.Controls.Add(Me.btnPasteTextFromWindowsClipboard)
        Me.tabHashText.Controls.Add(Me.btnComputeTextHash)
        Me.tabHashText.Controls.Add(Me.Label2)
        Me.tabHashText.Controls.Add(Me.textRadioMD5)
        Me.tabHashText.Controls.Add(Me.textRadioSHA512)
        Me.tabHashText.Controls.Add(Me.textRadioSHA384)
        Me.tabHashText.Controls.Add(Me.textRadioSHA256)
        Me.tabHashText.Controls.Add(Me.textRadioSHA1)
        Me.tabHashText.Controls.Add(Me.Label1)
        Me.tabHashText.Controls.Add(Me.lblHashTextStep1)
        Me.tabHashText.Controls.Add(Me.txtTextToHash)
        Me.tabHashText.Controls.Add(Me.lblTextToHash)
        Me.tabHashText.Location = New System.Drawing.Point(4, 22)
        Me.tabHashText.Name = "tabHashText"
        Me.tabHashText.Padding = New System.Windows.Forms.Padding(3)
        Me.tabHashText.Size = New System.Drawing.Size(1040, 363)
        Me.tabHashText.TabIndex = 1
        Me.tabHashText.Text = "Hash Text"
        '
        'btnCopyTextHashResultsToClipboard
        '
        Me.btnCopyTextHashResultsToClipboard.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.btnCopyTextHashResultsToClipboard.Enabled = False
        Me.btnCopyTextHashResultsToClipboard.Location = New System.Drawing.Point(240, 285)
        Me.btnCopyTextHashResultsToClipboard.Name = "btnCopyTextHashResultsToClipboard"
        Me.btnCopyTextHashResultsToClipboard.Size = New System.Drawing.Size(156, 23)
        Me.btnCopyTextHashResultsToClipboard.TabIndex = 31
        Me.btnCopyTextHashResultsToClipboard.Text = "&Copy Results to Clipboard"
        Me.btnCopyTextHashResultsToClipboard.UseVisualStyleBackColor = True
        '
        'txtHashResults
        '
        Me.txtHashResults.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtHashResults.BackColor = System.Drawing.SystemColors.Window
        Me.txtHashResults.Location = New System.Drawing.Point(240, 259)
        Me.txtHashResults.Name = "txtHashResults"
        Me.txtHashResults.ReadOnly = True
        Me.txtHashResults.Size = New System.Drawing.Size(791, 20)
        Me.txtHashResults.TabIndex = 29
        '
        'Label3
        '
        Me.Label3.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.Label3.AutoSize = True
        Me.Label3.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label3.Location = New System.Drawing.Point(238, 243)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(112, 13)
        Me.Label3.TabIndex = 28
        Me.Label3.Text = "Your Hash Results"
        '
        'btnPasteTextFromWindowsClipboard
        '
        Me.btnPasteTextFromWindowsClipboard.Location = New System.Drawing.Point(18, 107)
        Me.btnPasteTextFromWindowsClipboard.Name = "btnPasteTextFromWindowsClipboard"
        Me.btnPasteTextFromWindowsClipboard.Size = New System.Drawing.Size(156, 23)
        Me.btnPasteTextFromWindowsClipboard.TabIndex = 27
        Me.btnPasteTextFromWindowsClipboard.Text = "Paste Text"
        Me.btnPasteTextFromWindowsClipboard.UseVisualStyleBackColor = True
        '
        'btnComputeTextHash
        '
        Me.btnComputeTextHash.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.btnComputeTextHash.Enabled = False
        Me.btnComputeTextHash.Location = New System.Drawing.Point(18, 237)
        Me.btnComputeTextHash.Name = "btnComputeTextHash"
        Me.btnComputeTextHash.Size = New System.Drawing.Size(216, 71)
        Me.btnComputeTextHash.TabIndex = 26
        Me.btnComputeTextHash.Text = "Compute Hash"
        Me.btnComputeTextHash.UseVisualStyleBackColor = True
        '
        'Label2
        '
        Me.Label2.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.Label2.AutoSize = True
        Me.Label2.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label2.Location = New System.Drawing.Point(16, 221)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(459, 13)
        Me.Label2.TabIndex = 25
        Me.Label2.Text = "Step 2: Compute the hashes. (Now computes all five hash types simultaneously)"
        '
        'textRadioMD5
        '
        Me.textRadioMD5.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.textRadioMD5.AutoSize = True
        Me.textRadioMD5.Location = New System.Drawing.Point(506, 340)
        Me.textRadioMD5.Name = "textRadioMD5"
        Me.textRadioMD5.Size = New System.Drawing.Size(296, 17)
        Me.textRadioMD5.TabIndex = 24
        Me.textRadioMD5.Text = "MD5 (Seriously Not Recommended, Insecure Hash Type)"
        Me.textRadioMD5.UseVisualStyleBackColor = True
        '
        'textRadioSHA512
        '
        Me.textRadioSHA512.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.textRadioSHA512.AutoSize = True
        Me.textRadioSHA512.Location = New System.Drawing.Point(167, 340)
        Me.textRadioSHA512.Name = "textRadioSHA512"
        Me.textRadioSHA512.Size = New System.Drawing.Size(68, 17)
        Me.textRadioSHA512.TabIndex = 23
        Me.textRadioSHA512.Text = "SHA-512"
        Me.textRadioSHA512.UseVisualStyleBackColor = True
        '
        'textRadioSHA384
        '
        Me.textRadioSHA384.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.textRadioSHA384.AutoSize = True
        Me.textRadioSHA384.Location = New System.Drawing.Point(93, 340)
        Me.textRadioSHA384.Name = "textRadioSHA384"
        Me.textRadioSHA384.Size = New System.Drawing.Size(68, 17)
        Me.textRadioSHA384.TabIndex = 22
        Me.textRadioSHA384.Text = "SHA-384"
        Me.textRadioSHA384.UseVisualStyleBackColor = True
        '
        'textRadioSHA256
        '
        Me.textRadioSHA256.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.textRadioSHA256.AutoSize = True
        Me.textRadioSHA256.Checked = True
        Me.textRadioSHA256.Location = New System.Drawing.Point(19, 340)
        Me.textRadioSHA256.Name = "textRadioSHA256"
        Me.textRadioSHA256.Size = New System.Drawing.Size(68, 17)
        Me.textRadioSHA256.TabIndex = 21
        Me.textRadioSHA256.TabStop = True
        Me.textRadioSHA256.Text = "SHA-256"
        Me.textRadioSHA256.UseVisualStyleBackColor = True
        '
        'textRadioSHA1
        '
        Me.textRadioSHA1.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.textRadioSHA1.AutoSize = True
        Me.textRadioSHA1.Location = New System.Drawing.Point(241, 340)
        Me.textRadioSHA1.Name = "textRadioSHA1"
        Me.textRadioSHA1.Size = New System.Drawing.Size(259, 17)
        Me.textRadioSHA1.TabIndex = 20
        Me.textRadioSHA1.Text = "SHA-1 (Not Recommended, Insecure Hash Type)"
        Me.textRadioSHA1.UseVisualStyleBackColor = True
        '
        'Label1
        '
        Me.Label1.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.Location = New System.Drawing.Point(15, 324)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(459, 13)
        Me.Label1.TabIndex = 19
        Me.Label1.Text = "Step 3: Select your hash type for display and copying to the Windows Clipboard"
        '
        'lblHashTextStep1
        '
        Me.lblHashTextStep1.AutoSize = True
        Me.lblHashTextStep1.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblHashTextStep1.Location = New System.Drawing.Point(15, 13)
        Me.lblHashTextStep1.Name = "lblHashTextStep1"
        Me.lblHashTextStep1.Size = New System.Drawing.Size(219, 13)
        Me.lblHashTextStep1.TabIndex = 7
        Me.lblHashTextStep1.Text = "Step 1: Input some text: 0 Characters"
        '
        'txtTextToHash
        '
        Me.txtTextToHash.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtTextToHash.Location = New System.Drawing.Point(180, 29)
        Me.txtTextToHash.Multiline = True
        Me.txtTextToHash.Name = "txtTextToHash"
        Me.txtTextToHash.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.txtTextToHash.Size = New System.Drawing.Size(854, 176)
        Me.txtTextToHash.TabIndex = 1
        '
        'lblTextToHash
        '
        Me.lblTextToHash.AutoSize = True
        Me.lblTextToHash.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblTextToHash.Location = New System.Drawing.Point(15, 32)
        Me.lblTextToHash.Name = "lblTextToHash"
        Me.lblTextToHash.Size = New System.Drawing.Size(159, 13)
        Me.lblTextToHash.TabIndex = 0
        Me.lblTextToHash.Text = "Text that you want to hash"
        '
        'tabHashIndividualFiles
        '
        Me.tabHashIndividualFiles.BackColor = System.Drawing.SystemColors.Control
        Me.tabHashIndividualFiles.Controls.Add(Me.hashIndividualFilesTableLayoutControl)
        Me.tabHashIndividualFiles.Controls.Add(Me.lblLine)
        Me.tabHashIndividualFiles.Controls.Add(Me.lblHashIndividualFilesStep3)
        Me.tabHashIndividualFiles.Controls.Add(Me.lblHashIndividualFilesStep2)
        Me.tabHashIndividualFiles.Controls.Add(Me.btnIndividualFilesSaveResultsToDisk)
        Me.tabHashIndividualFiles.Controls.Add(Me.btnIndividualFilesCopyToClipboard)
        Me.tabHashIndividualFiles.Controls.Add(Me.btnComputeHash)
        Me.tabHashIndividualFiles.Controls.Add(Me.radioMD5)
        Me.tabHashIndividualFiles.Controls.Add(Me.radioSHA512)
        Me.tabHashIndividualFiles.Controls.Add(Me.radioSHA384)
        Me.tabHashIndividualFiles.Controls.Add(Me.radioSHA256)
        Me.tabHashIndividualFiles.Controls.Add(Me.radioSHA1)
        Me.tabHashIndividualFiles.Controls.Add(Me.lblHashIndividualFilesStep1)
        Me.tabHashIndividualFiles.Controls.Add(Me.listFiles)
        Me.tabHashIndividualFiles.Controls.Add(Me.btnRemoveSelectedFiles)
        Me.tabHashIndividualFiles.Controls.Add(Me.btnRemoveAllFiles)
        Me.tabHashIndividualFiles.Controls.Add(Me.btnAddFilesInFolder)
        Me.tabHashIndividualFiles.Controls.Add(Me.btnAddIndividualFiles)
        Me.tabHashIndividualFiles.Location = New System.Drawing.Point(4, 22)
        Me.tabHashIndividualFiles.Name = "tabHashIndividualFiles"
        Me.tabHashIndividualFiles.Size = New System.Drawing.Size(1040, 363)
        Me.tabHashIndividualFiles.TabIndex = 2
        Me.tabHashIndividualFiles.Text = "Hash Individual Files"
        '
        'hashIndividualFilesTableLayoutControl
        '
        Me.hashIndividualFilesTableLayoutControl.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.hashIndividualFilesTableLayoutControl.ColumnCount = 2
        Me.hashIndividualFilesTableLayoutControl.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.hashIndividualFilesTableLayoutControl.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.hashIndividualFilesTableLayoutControl.Controls.Add(Me.lblIndividualFilesStatusProcessingFile, 1, 0)
        Me.hashIndividualFilesTableLayoutControl.Controls.Add(Me.lblProcessingFile, 0, 0)
        Me.hashIndividualFilesTableLayoutControl.Controls.Add(Me.IndividualFilesProgressBar, 0, 1)
        Me.hashIndividualFilesTableLayoutControl.Controls.Add(Me.hashIndividualFilesAllFilesProgressBar, 1, 1)
        Me.hashIndividualFilesTableLayoutControl.Controls.Add(Me.lblHashIndividualFilesTotalStatus, 1, 2)
        Me.hashIndividualFilesTableLayoutControl.Controls.Add(Me.lblIndividualFilesStatus, 0, 2)
        Me.hashIndividualFilesTableLayoutControl.Location = New System.Drawing.Point(237, 202)
        Me.hashIndividualFilesTableLayoutControl.Name = "hashIndividualFilesTableLayoutControl"
        Me.hashIndividualFilesTableLayoutControl.RowCount = 3
        Me.hashIndividualFilesTableLayoutControl.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25.0!))
        Me.hashIndividualFilesTableLayoutControl.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.hashIndividualFilesTableLayoutControl.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25.0!))
        Me.hashIndividualFilesTableLayoutControl.Size = New System.Drawing.Size(800, 66)
        Me.hashIndividualFilesTableLayoutControl.TabIndex = 24
        '
        'lblIndividualFilesStatusProcessingFile
        '
        Me.lblIndividualFilesStatusProcessingFile.AutoSize = True
        Me.lblIndividualFilesStatusProcessingFile.Location = New System.Drawing.Point(403, 0)
        Me.lblIndividualFilesStatusProcessingFile.Name = "lblIndividualFilesStatusProcessingFile"
        Me.lblIndividualFilesStatusProcessingFile.Size = New System.Drawing.Size(181, 13)
        Me.lblIndividualFilesStatusProcessingFile.TabIndex = 17
        Me.lblIndividualFilesStatusProcessingFile.Text = "lblIndividualFilesStatusProcessingFile"
        '
        'lblProcessingFile
        '
        Me.lblProcessingFile.AutoSize = True
        Me.lblProcessingFile.Location = New System.Drawing.Point(3, 0)
        Me.lblProcessingFile.Name = "lblProcessingFile"
        Me.lblProcessingFile.Size = New System.Drawing.Size(85, 13)
        Me.lblProcessingFile.TabIndex = 22
        Me.lblProcessingFile.Text = "lblProcessingFile"
        '
        'IndividualFilesProgressBar
        '
        Me.IndividualFilesProgressBar.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.IndividualFilesProgressBar.Location = New System.Drawing.Point(3, 19)
        Me.IndividualFilesProgressBar.Name = "IndividualFilesProgressBar"
        Me.IndividualFilesProgressBar.Size = New System.Drawing.Size(394, 27)
        Me.IndividualFilesProgressBar.TabIndex = 14
        Me.IndividualFilesProgressBar.Visible = False
        '
        'hashIndividualFilesAllFilesProgressBar
        '
        Me.hashIndividualFilesAllFilesProgressBar.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.hashIndividualFilesAllFilesProgressBar.Location = New System.Drawing.Point(403, 19)
        Me.hashIndividualFilesAllFilesProgressBar.Name = "hashIndividualFilesAllFilesProgressBar"
        Me.hashIndividualFilesAllFilesProgressBar.Size = New System.Drawing.Size(394, 27)
        Me.hashIndividualFilesAllFilesProgressBar.TabIndex = 23
        '
        'lblHashIndividualFilesTotalStatus
        '
        Me.lblHashIndividualFilesTotalStatus.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.lblHashIndividualFilesTotalStatus.AutoSize = True
        Me.lblHashIndividualFilesTotalStatus.Location = New System.Drawing.Point(403, 53)
        Me.lblHashIndividualFilesTotalStatus.Name = "lblHashIndividualFilesTotalStatus"
        Me.lblHashIndividualFilesTotalStatus.Size = New System.Drawing.Size(162, 13)
        Me.lblHashIndividualFilesTotalStatus.TabIndex = 24
        Me.lblHashIndividualFilesTotalStatus.Text = "lblHashIndividualFilesTotalStatus"
        Me.lblHashIndividualFilesTotalStatus.Visible = False
        '
        'lblIndividualFilesStatus
        '
        Me.lblIndividualFilesStatus.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.lblIndividualFilesStatus.AutoSize = True
        Me.lblIndividualFilesStatus.Location = New System.Drawing.Point(3, 53)
        Me.lblIndividualFilesStatus.Name = "lblIndividualFilesStatus"
        Me.lblIndividualFilesStatus.Size = New System.Drawing.Size(140, 13)
        Me.lblIndividualFilesStatus.TabIndex = 13
        Me.lblIndividualFilesStatus.Text = "(No Background Processes)"
        Me.lblIndividualFilesStatus.Visible = False
        '
        'lblLine
        '
        Me.lblLine.BackColor = System.Drawing.Color.Black
        Me.lblLine.Location = New System.Drawing.Point(15, 80)
        Me.lblLine.Name = "lblLine"
        Me.lblLine.Size = New System.Drawing.Size(139, 1)
        Me.lblLine.TabIndex = 20
        '
        'lblHashIndividualFilesStep3
        '
        Me.lblHashIndividualFilesStep3.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.lblHashIndividualFilesStep3.AutoSize = True
        Me.lblHashIndividualFilesStep3.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblHashIndividualFilesStep3.Location = New System.Drawing.Point(15, 181)
        Me.lblHashIndividualFilesStep3.Name = "lblHashIndividualFilesStep3"
        Me.lblHashIndividualFilesStep3.Size = New System.Drawing.Size(459, 13)
        Me.lblHashIndividualFilesStep3.TabIndex = 19
        Me.lblHashIndividualFilesStep3.Text = "Step 2: Compute the hashes. (Now computes all five hash types simultaneously)"
        '
        'lblHashIndividualFilesStep2
        '
        Me.lblHashIndividualFilesStep2.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.lblHashIndividualFilesStep2.AutoSize = True
        Me.lblHashIndividualFilesStep2.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblHashIndividualFilesStep2.Location = New System.Drawing.Point(15, 283)
        Me.lblHashIndividualFilesStep2.Name = "lblHashIndividualFilesStep2"
        Me.lblHashIndividualFilesStep2.Size = New System.Drawing.Size(345, 13)
        Me.lblHashIndividualFilesStep2.TabIndex = 18
        Me.lblHashIndividualFilesStep2.Text = "Step 3: Select your hash type for display and copying to the Windows Clipboard"
        '
        'btnIndividualFilesSaveResultsToDisk
        '
        Me.btnIndividualFilesSaveResultsToDisk.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.btnIndividualFilesSaveResultsToDisk.Enabled = False
        Me.btnIndividualFilesSaveResultsToDisk.Location = New System.Drawing.Point(237, 334)
        Me.btnIndividualFilesSaveResultsToDisk.Name = "btnIndividualFilesSaveResultsToDisk"
        Me.btnIndividualFilesSaveResultsToDisk.Size = New System.Drawing.Size(216, 23)
        Me.btnIndividualFilesSaveResultsToDisk.TabIndex = 16
        Me.btnIndividualFilesSaveResultsToDisk.Text = "&Save Results to Disk ..."
        Me.btnIndividualFilesSaveResultsToDisk.UseVisualStyleBackColor = True
        '
        'btnIndividualFilesCopyToClipboard
        '
        Me.btnIndividualFilesCopyToClipboard.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.btnIndividualFilesCopyToClipboard.Enabled = False
        Me.btnIndividualFilesCopyToClipboard.Location = New System.Drawing.Point(15, 334)
        Me.btnIndividualFilesCopyToClipboard.Name = "btnIndividualFilesCopyToClipboard"
        Me.btnIndividualFilesCopyToClipboard.Size = New System.Drawing.Size(216, 23)
        Me.btnIndividualFilesCopyToClipboard.TabIndex = 15
        Me.btnIndividualFilesCopyToClipboard.Text = "Copy Results to Clipboard"
        Me.btnIndividualFilesCopyToClipboard.UseVisualStyleBackColor = True
        '
        'btnComputeHash
        '
        Me.btnComputeHash.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.btnComputeHash.Enabled = False
        Me.btnComputeHash.Location = New System.Drawing.Point(15, 197)
        Me.btnComputeHash.Name = "btnComputeHash"
        Me.btnComputeHash.Size = New System.Drawing.Size(216, 71)
        Me.btnComputeHash.TabIndex = 12
        Me.btnComputeHash.Text = "Compute &Hash"
        Me.btnComputeHash.UseVisualStyleBackColor = True
        '
        'radioMD5
        '
        Me.radioMD5.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.radioMD5.AutoSize = True
        Me.radioMD5.Location = New System.Drawing.Point(503, 299)
        Me.radioMD5.Name = "radioMD5"
        Me.radioMD5.Size = New System.Drawing.Size(296, 17)
        Me.radioMD5.TabIndex = 11
        Me.radioMD5.Text = "MD5 (Seriously Not Recommended, Insecure Hash Type)"
        Me.radioMD5.UseVisualStyleBackColor = True
        '
        'radioSHA512
        '
        Me.radioSHA512.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.radioSHA512.AutoSize = True
        Me.radioSHA512.Location = New System.Drawing.Point(164, 299)
        Me.radioSHA512.Name = "radioSHA512"
        Me.radioSHA512.Size = New System.Drawing.Size(68, 17)
        Me.radioSHA512.TabIndex = 10
        Me.radioSHA512.Text = "SHA-512"
        Me.radioSHA512.UseVisualStyleBackColor = True
        '
        'radioSHA384
        '
        Me.radioSHA384.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.radioSHA384.AutoSize = True
        Me.radioSHA384.Location = New System.Drawing.Point(90, 299)
        Me.radioSHA384.Name = "radioSHA384"
        Me.radioSHA384.Size = New System.Drawing.Size(68, 17)
        Me.radioSHA384.TabIndex = 9
        Me.radioSHA384.Text = "SHA-384"
        Me.radioSHA384.UseVisualStyleBackColor = True
        '
        'radioSHA256
        '
        Me.radioSHA256.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.radioSHA256.AutoSize = True
        Me.radioSHA256.Checked = True
        Me.radioSHA256.Location = New System.Drawing.Point(16, 299)
        Me.radioSHA256.Name = "radioSHA256"
        Me.radioSHA256.Size = New System.Drawing.Size(68, 17)
        Me.radioSHA256.TabIndex = 8
        Me.radioSHA256.TabStop = True
        Me.radioSHA256.Text = "SHA-256"
        Me.radioSHA256.UseVisualStyleBackColor = True
        '
        'radioSHA1
        '
        Me.radioSHA1.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.radioSHA1.AutoSize = True
        Me.radioSHA1.Location = New System.Drawing.Point(238, 299)
        Me.radioSHA1.Name = "radioSHA1"
        Me.radioSHA1.Size = New System.Drawing.Size(259, 17)
        Me.radioSHA1.TabIndex = 7
        Me.radioSHA1.Text = "SHA-1 (Not Recommended, Insecure Hash Type)"
        Me.radioSHA1.UseVisualStyleBackColor = True
        '
        'lblHashIndividualFilesStep1
        '
        Me.lblHashIndividualFilesStep1.AutoSize = True
        Me.lblHashIndividualFilesStep1.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblHashIndividualFilesStep1.Location = New System.Drawing.Point(15, 4)
        Me.lblHashIndividualFilesStep1.Name = "lblHashIndividualFilesStep1"
        Me.lblHashIndividualFilesStep1.Size = New System.Drawing.Size(302, 13)
        Me.lblHashIndividualFilesStep1.TabIndex = 6
        Me.lblHashIndividualFilesStep1.Text = "Step 1: Select Individual Files to be Hashed: 0 Files"
        '
        'listFiles
        '
        Me.listFiles.AllowDrop = True
        Me.listFiles.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.listFiles.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.colFileName, Me.colFileSize, Me.colChecksum, Me.colComputeTime})
        Me.listFiles.ContextMenuStrip = Me.listFilesContextMenu
        Me.listFiles.FullRowSelect = True
        Me.listFiles.HideSelection = False
        Me.listFiles.Location = New System.Drawing.Point(160, 20)
        Me.listFiles.Name = "listFiles"
        Me.listFiles.Size = New System.Drawing.Size(877, 146)
        Me.listFiles.TabIndex = 5
        Me.listFiles.UseCompatibleStateImageBehavior = False
        Me.listFiles.View = System.Windows.Forms.View.Details
        '
        'colFileName
        '
        Me.colFileName.Text = "File Name"
        Me.colFileName.Width = 528
        '
        'colFileSize
        '
        Me.colFileSize.Text = "File Size"
        Me.colFileSize.Width = 70
        '
        'colChecksum
        '
        Me.colChecksum.Text = "Hash/Checksum (SHA256)"
        Me.colChecksum.Width = 241
        '
        'colComputeTime
        '
        Me.colComputeTime.Text = "Compute Time"
        Me.colComputeTime.Width = 150
        '
        'listFilesContextMenu
        '
        Me.listFilesContextMenu.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.CopyHashToClipboardToolStripMenuItem})
        Me.listFilesContextMenu.Name = "ContextMenuStrip1"
        Me.listFilesContextMenu.Size = New System.Drawing.Size(249, 26)
        '
        'CopyHashToClipboardToolStripMenuItem
        '
        Me.CopyHashToClipboardToolStripMenuItem.Name = "CopyHashToClipboardToolStripMenuItem"
        Me.CopyHashToClipboardToolStripMenuItem.Size = New System.Drawing.Size(248, 22)
        Me.CopyHashToClipboardToolStripMenuItem.Text = "Copy Selected Hash to Clipboard"
        '
        'btnRemoveSelectedFiles
        '
        Me.btnRemoveSelectedFiles.Location = New System.Drawing.Point(15, 90)
        Me.btnRemoveSelectedFiles.Name = "btnRemoveSelectedFiles"
        Me.btnRemoveSelectedFiles.Size = New System.Drawing.Size(139, 23)
        Me.btnRemoveSelectedFiles.TabIndex = 4
        Me.btnRemoveSelectedFiles.Text = "Remove &Selected Files"
        Me.btnRemoveSelectedFiles.UseVisualStyleBackColor = True
        '
        'btnRemoveAllFiles
        '
        Me.btnRemoveAllFiles.Location = New System.Drawing.Point(15, 119)
        Me.btnRemoveAllFiles.Name = "btnRemoveAllFiles"
        Me.btnRemoveAllFiles.Size = New System.Drawing.Size(139, 23)
        Me.btnRemoveAllFiles.TabIndex = 3
        Me.btnRemoveAllFiles.Text = "&Remove All Files"
        Me.btnRemoveAllFiles.UseVisualStyleBackColor = True
        '
        'btnAddFilesInFolder
        '
        Me.btnAddFilesInFolder.Location = New System.Drawing.Point(15, 49)
        Me.btnAddFilesInFolder.Name = "btnAddFilesInFolder"
        Me.btnAddFilesInFolder.Size = New System.Drawing.Size(139, 23)
        Me.btnAddFilesInFolder.TabIndex = 1
        Me.btnAddFilesInFolder.Text = "&Add File(s) in Folder ..."
        Me.btnAddFilesInFolder.UseVisualStyleBackColor = True
        '
        'btnAddIndividualFiles
        '
        Me.btnAddIndividualFiles.Location = New System.Drawing.Point(15, 20)
        Me.btnAddIndividualFiles.Name = "btnAddIndividualFiles"
        Me.btnAddIndividualFiles.Size = New System.Drawing.Size(139, 23)
        Me.btnAddIndividualFiles.TabIndex = 0
        Me.btnAddIndividualFiles.Text = "Add &File(s) ..."
        Me.btnAddIndividualFiles.UseVisualStyleBackColor = True
        '
        'tabVerifySavedHashes
        '
        Me.tabVerifySavedHashes.BackColor = System.Drawing.SystemColors.Control
        Me.tabVerifySavedHashes.Controls.Add(Me.btnRetestFailedFiles)
        Me.tabVerifySavedHashes.Controls.Add(Me.verifyHashesListFiles)
        Me.tabVerifySavedHashes.Controls.Add(Me.verifySavedHashesTableLayoutControl)
        Me.tabVerifySavedHashes.Controls.Add(Me.lblVerifyFileNameLabel)
        Me.tabVerifySavedHashes.Controls.Add(Me.btnOpenExistingHashFile)
        Me.tabVerifySavedHashes.Controls.Add(Me.btnTransferToHashIndividualFilesTab)
        Me.tabVerifySavedHashes.Location = New System.Drawing.Point(4, 22)
        Me.tabVerifySavedHashes.Name = "tabVerifySavedHashes"
        Me.tabVerifySavedHashes.Size = New System.Drawing.Size(1040, 363)
        Me.tabVerifySavedHashes.TabIndex = 3
        Me.tabVerifySavedHashes.Text = "Verify Saved Hashes"
        '
        'verifySavedHashesTableLayoutControl
        '
        Me.verifySavedHashesTableLayoutControl.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.verifySavedHashesTableLayoutControl.ColumnCount = 2
        Me.verifySavedHashesTableLayoutControl.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.verifySavedHashesTableLayoutControl.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.verifySavedHashesTableLayoutControl.Controls.Add(Me.VerifyHashProgressBar, 0, 1)
        Me.verifySavedHashesTableLayoutControl.Controls.Add(Me.lblVerifyHashStatusProcessingFile, 1, 0)
        Me.verifySavedHashesTableLayoutControl.Controls.Add(Me.verifyIndividualFilesAllFilesProgressBar, 1, 1)
        Me.verifySavedHashesTableLayoutControl.Controls.Add(Me.lblVerifyHashesTotalStatus, 1, 2)
        Me.verifySavedHashesTableLayoutControl.Controls.Add(Me.lblVerifyHashStatus, 0, 0)
        Me.verifySavedHashesTableLayoutControl.Controls.Add(Me.lblProcessingFileVerify, 0, 2)
        Me.verifySavedHashesTableLayoutControl.Location = New System.Drawing.Point(160, 294)
        Me.verifySavedHashesTableLayoutControl.Name = "verifySavedHashesTableLayoutControl"
        Me.verifySavedHashesTableLayoutControl.RowCount = 3
        Me.verifySavedHashesTableLayoutControl.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25.0!))
        Me.verifySavedHashesTableLayoutControl.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.verifySavedHashesTableLayoutControl.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25.0!))
        Me.verifySavedHashesTableLayoutControl.Size = New System.Drawing.Size(877, 66)
        Me.verifySavedHashesTableLayoutControl.TabIndex = 25
        '
        'VerifyHashProgressBar
        '
        Me.VerifyHashProgressBar.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.VerifyHashProgressBar.Location = New System.Drawing.Point(3, 19)
        Me.VerifyHashProgressBar.Name = "VerifyHashProgressBar"
        Me.VerifyHashProgressBar.Size = New System.Drawing.Size(432, 27)
        Me.VerifyHashProgressBar.TabIndex = 16
        Me.VerifyHashProgressBar.Visible = False
        '
        'lblVerifyHashStatusProcessingFile
        '
        Me.lblVerifyHashStatusProcessingFile.AutoSize = True
        Me.lblVerifyHashStatusProcessingFile.Location = New System.Drawing.Point(441, 0)
        Me.lblVerifyHashStatusProcessingFile.Name = "lblVerifyHashStatusProcessingFile"
        Me.lblVerifyHashStatusProcessingFile.Size = New System.Drawing.Size(166, 13)
        Me.lblVerifyHashStatusProcessingFile.TabIndex = 18
        Me.lblVerifyHashStatusProcessingFile.Text = "lblVerifyHashStatusProcessingFile"
        '
        'verifyIndividualFilesAllFilesProgressBar
        '
        Me.verifyIndividualFilesAllFilesProgressBar.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.verifyIndividualFilesAllFilesProgressBar.Location = New System.Drawing.Point(441, 19)
        Me.verifyIndividualFilesAllFilesProgressBar.Name = "verifyIndividualFilesAllFilesProgressBar"
        Me.verifyIndividualFilesAllFilesProgressBar.Size = New System.Drawing.Size(433, 27)
        Me.verifyIndividualFilesAllFilesProgressBar.TabIndex = 24
        '
        'lblVerifyHashesTotalStatus
        '
        Me.lblVerifyHashesTotalStatus.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.lblVerifyHashesTotalStatus.AutoSize = True
        Me.lblVerifyHashesTotalStatus.Location = New System.Drawing.Point(441, 53)
        Me.lblVerifyHashesTotalStatus.Name = "lblVerifyHashesTotalStatus"
        Me.lblVerifyHashesTotalStatus.Size = New System.Drawing.Size(133, 13)
        Me.lblVerifyHashesTotalStatus.TabIndex = 25
        Me.lblVerifyHashesTotalStatus.Text = "lblVerifyHashesTotalStatus"
        Me.lblVerifyHashesTotalStatus.Visible = False
        '
        'lblVerifyHashStatus
        '
        Me.lblVerifyHashStatus.AutoSize = True
        Me.lblVerifyHashStatus.Location = New System.Drawing.Point(3, 0)
        Me.lblVerifyHashStatus.Name = "lblVerifyHashStatus"
        Me.lblVerifyHashStatus.Size = New System.Drawing.Size(140, 13)
        Me.lblVerifyHashStatus.TabIndex = 15
        Me.lblVerifyHashStatus.Text = "(No Background Processes)"
        Me.lblVerifyHashStatus.Visible = False
        '
        'lblProcessingFileVerify
        '
        Me.lblProcessingFileVerify.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.lblProcessingFileVerify.AutoSize = True
        Me.lblProcessingFileVerify.Location = New System.Drawing.Point(3, 53)
        Me.lblProcessingFileVerify.Name = "lblProcessingFileVerify"
        Me.lblProcessingFileVerify.Size = New System.Drawing.Size(111, 13)
        Me.lblProcessingFileVerify.TabIndex = 20
        Me.lblProcessingFileVerify.Text = "lblProcessingFileVerify"
        '
        'lblVerifyFileNameLabel
        '
        Me.lblVerifyFileNameLabel.AutoSize = True
        Me.lblVerifyFileNameLabel.Location = New System.Drawing.Point(160, 12)
        Me.lblVerifyFileNameLabel.Name = "lblVerifyFileNameLabel"
        Me.lblVerifyFileNameLabel.Size = New System.Drawing.Size(207, 13)
        Me.lblVerifyFileNameLabel.TabIndex = 19
        Me.lblVerifyFileNameLabel.Text = "File Name: (None Selected for Processing)"
        '
        'verifyHashesListFiles
        '
        Me.verifyHashesListFiles.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.verifyHashesListFiles.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.colFile, Me.colFileSize2, Me.colResults, Me.colComputeTime2, Me.colNewHash})
        Me.verifyHashesListFiles.ContextMenuStrip = Me.verifyListFilesContextMenu
        Me.verifyHashesListFiles.FullRowSelect = True
        Me.verifyHashesListFiles.HideSelection = False
        Me.verifyHashesListFiles.Location = New System.Drawing.Point(160, 28)
        Me.verifyHashesListFiles.MultiSelect = False
        Me.verifyHashesListFiles.Name = "verifyHashesListFiles"
        Me.verifyHashesListFiles.Size = New System.Drawing.Size(877, 332)
        Me.verifyHashesListFiles.TabIndex = 6
        Me.verifyHashesListFiles.UseCompatibleStateImageBehavior = False
        Me.verifyHashesListFiles.View = System.Windows.Forms.View.Details
        '
        'colFile
        '
        Me.colFile.Text = "File Name"
        Me.colFile.Width = 557
        '
        'colFileSize2
        '
        Me.colFileSize2.Text = "File Size"
        Me.colFileSize2.Width = 87
        '
        'colResults
        '
        Me.colResults.Text = "Results"
        Me.colResults.Width = 72
        '
        'colComputeTime2
        '
        Me.colComputeTime2.Text = "Compute Time"
        Me.colComputeTime2.Width = 150
        '
        'colNewHash
        '
        Me.colNewHash.Text = "Computed Hash/Checksum (Displays the hash that was computed only if not valid)"
        Me.colNewHash.Width = 120
        '
        'verifyListFilesContextMenu
        '
        Me.verifyListFilesContextMenu.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ViewChecksumDifferenceToolStripMenuItem})
        Me.verifyListFilesContextMenu.Name = "verifyListFilesContextMenu"
        Me.verifyListFilesContextMenu.Size = New System.Drawing.Size(216, 26)
        '
        'ViewChecksumDifferenceToolStripMenuItem
        '
        Me.ViewChecksumDifferenceToolStripMenuItem.Name = "ViewChecksumDifferenceToolStripMenuItem"
        Me.ViewChecksumDifferenceToolStripMenuItem.Size = New System.Drawing.Size(215, 22)
        Me.ViewChecksumDifferenceToolStripMenuItem.Text = "&View Checksum Difference"
        '
        'btnOpenExistingHashFile
        '
        Me.btnOpenExistingHashFile.AllowDrop = True
        Me.btnOpenExistingHashFile.Location = New System.Drawing.Point(12, 12)
        Me.btnOpenExistingHashFile.Name = "btnOpenExistingHashFile"
        Me.btnOpenExistingHashFile.Size = New System.Drawing.Size(142, 67)
        Me.btnOpenExistingHashFile.TabIndex = 0
        Me.btnOpenExistingHashFile.Text = "Open Hash File"
        Me.btnOpenExistingHashFile.UseVisualStyleBackColor = True
        '
        'btnTransferToHashIndividualFilesTab
        '
        Me.btnTransferToHashIndividualFilesTab.Enabled = False
        Me.btnTransferToHashIndividualFilesTab.Location = New System.Drawing.Point(12, 85)
        Me.btnTransferToHashIndividualFilesTab.Name = "btnTransferToHashIndividualFilesTab"
        Me.btnTransferToHashIndividualFilesTab.Size = New System.Drawing.Size(142, 67)
        Me.btnTransferToHashIndividualFilesTab.TabIndex = 26
        Me.btnTransferToHashIndividualFilesTab.Text = "Transfer to" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & """Hash Individual Files""" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "tab"
        Me.btnTransferToHashIndividualFilesTab.UseVisualStyleBackColor = True
        '
        'tabCompareFiles
        '
        Me.tabCompareFiles.BackColor = System.Drawing.SystemColors.Control
        Me.tabCompareFiles.Controls.Add(Me.pictureBoxCompareFiles)
        Me.tabCompareFiles.Controls.Add(Me.compareFilesTableLayoutControl)
        Me.tabCompareFiles.Controls.Add(Me.lblFile2Hash)
        Me.tabCompareFiles.Controls.Add(Me.lblFile1Hash)
        Me.tabCompareFiles.Controls.Add(Me.btnCompareFiles)
        Me.tabCompareFiles.Controls.Add(Me.Label6)
        Me.tabCompareFiles.Controls.Add(Me.compareRadioMD5)
        Me.tabCompareFiles.Controls.Add(Me.compareRadioSHA512)
        Me.tabCompareFiles.Controls.Add(Me.compareRadioSHA384)
        Me.tabCompareFiles.Controls.Add(Me.compareRadioSHA256)
        Me.tabCompareFiles.Controls.Add(Me.compareRadioSHA1)
        Me.tabCompareFiles.Controls.Add(Me.btnCompareFilesBrowseFile2)
        Me.tabCompareFiles.Controls.Add(Me.btnCompareFilesBrowseFile1)
        Me.tabCompareFiles.Controls.Add(Me.Label5)
        Me.tabCompareFiles.Controls.Add(Me.txtFile2)
        Me.tabCompareFiles.Controls.Add(Me.txtFile1)
        Me.tabCompareFiles.Controls.Add(Me.Label4)
        Me.tabCompareFiles.Location = New System.Drawing.Point(4, 22)
        Me.tabCompareFiles.Name = "tabCompareFiles"
        Me.tabCompareFiles.Size = New System.Drawing.Size(1040, 363)
        Me.tabCompareFiles.TabIndex = 5
        Me.tabCompareFiles.Text = "Compare Files"
        '
        'compareFilesTableLayoutControl
        '
        Me.compareFilesTableLayoutControl.ColumnCount = 2
        Me.compareFilesTableLayoutControl.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.compareFilesTableLayoutControl.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.compareFilesTableLayoutControl.Controls.Add(Me.lblCompareFilesStatus, 0, 0)
        Me.compareFilesTableLayoutControl.Controls.Add(Me.compareFilesProgressBar, 0, 1)
        Me.compareFilesTableLayoutControl.Controls.Add(Me.CompareFilesAllFilesProgress, 1, 1)
        Me.compareFilesTableLayoutControl.Controls.Add(Me.lblCompareFilesAllFilesStatus, 1, 0)
        Me.compareFilesTableLayoutControl.Location = New System.Drawing.Point(166, 154)
        Me.compareFilesTableLayoutControl.Name = "compareFilesTableLayoutControl"
        Me.compareFilesTableLayoutControl.RowCount = 2
        Me.compareFilesTableLayoutControl.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 31.37255!))
        Me.compareFilesTableLayoutControl.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 68.62745!))
        Me.compareFilesTableLayoutControl.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.compareFilesTableLayoutControl.Size = New System.Drawing.Size(871, 51)
        Me.compareFilesTableLayoutControl.TabIndex = 35
        '
        'lblCompareFilesStatus
        '
        Me.lblCompareFilesStatus.AutoSize = True
        Me.lblCompareFilesStatus.Location = New System.Drawing.Point(3, 0)
        Me.lblCompareFilesStatus.Name = "lblCompareFilesStatus"
        Me.lblCompareFilesStatus.Size = New System.Drawing.Size(140, 13)
        Me.lblCompareFilesStatus.TabIndex = 30
        Me.lblCompareFilesStatus.Text = "(No Background Processes)"
        '
        'compareFilesProgressBar
        '
        Me.compareFilesProgressBar.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.compareFilesProgressBar.Location = New System.Drawing.Point(3, 19)
        Me.compareFilesProgressBar.Name = "compareFilesProgressBar"
        Me.compareFilesProgressBar.Size = New System.Drawing.Size(429, 29)
        Me.compareFilesProgressBar.TabIndex = 31
        Me.compareFilesProgressBar.Visible = False
        '
        'CompareFilesAllFilesProgress
        '
        Me.CompareFilesAllFilesProgress.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.CompareFilesAllFilesProgress.Location = New System.Drawing.Point(438, 19)
        Me.CompareFilesAllFilesProgress.Name = "CompareFilesAllFilesProgress"
        Me.CompareFilesAllFilesProgress.Size = New System.Drawing.Size(430, 29)
        Me.CompareFilesAllFilesProgress.TabIndex = 34
        '
        'lblCompareFilesAllFilesStatus
        '
        Me.lblCompareFilesAllFilesStatus.AutoSize = True
        Me.lblCompareFilesAllFilesStatus.Location = New System.Drawing.Point(438, 0)
        Me.lblCompareFilesAllFilesStatus.Name = "lblCompareFilesAllFilesStatus"
        Me.lblCompareFilesAllFilesStatus.Size = New System.Drawing.Size(142, 13)
        Me.lblCompareFilesAllFilesStatus.TabIndex = 35
        Me.lblCompareFilesAllFilesStatus.Text = "lblCompareFilesAllFilesStatus"
        '
        'lblFile2Hash
        '
        Me.lblFile2Hash.AutoSize = True
        Me.lblFile2Hash.Location = New System.Drawing.Point(15, 87)
        Me.lblFile2Hash.Name = "lblFile2Hash"
        Me.lblFile2Hash.Size = New System.Drawing.Size(179, 13)
        Me.lblFile2Hash.TabIndex = 33
        Me.lblFile2Hash.Text = "Hash/Checksum: (To Be Computed)"
        '
        'lblFile1Hash
        '
        Me.lblFile1Hash.AutoSize = True
        Me.lblFile1Hash.Location = New System.Drawing.Point(15, 37)
        Me.lblFile1Hash.Name = "lblFile1Hash"
        Me.lblFile1Hash.Size = New System.Drawing.Size(179, 13)
        Me.lblFile1Hash.TabIndex = 32
        Me.lblFile1Hash.Text = "Hash/Checksum: (To Be Computed)"
        '
        'btnCompareFiles
        '
        Me.btnCompareFiles.Enabled = False
        Me.btnCompareFiles.Location = New System.Drawing.Point(18, 154)
        Me.btnCompareFiles.Name = "btnCompareFiles"
        Me.btnCompareFiles.Size = New System.Drawing.Size(142, 48)
        Me.btnCompareFiles.TabIndex = 29
        Me.btnCompareFiles.Text = "Compare Files"
        Me.btnCompareFiles.UseVisualStyleBackColor = True
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label6.Location = New System.Drawing.Point(15, 115)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(236, 13)
        Me.Label6.TabIndex = 28
        Me.Label6.Text = "Select your hash type to compare with..."
        '
        'compareRadioMD5
        '
        Me.compareRadioMD5.AutoSize = True
        Me.compareRadioMD5.Location = New System.Drawing.Point(548, 131)
        Me.compareRadioMD5.Name = "compareRadioMD5"
        Me.compareRadioMD5.Size = New System.Drawing.Size(296, 17)
        Me.compareRadioMD5.TabIndex = 26
        Me.compareRadioMD5.Text = "MD5 (Seriously Not Recommended, Insecure Hash Type)"
        Me.compareRadioMD5.UseVisualStyleBackColor = True
        '
        'compareRadioSHA512
        '
        Me.compareRadioSHA512.AutoSize = True
        Me.compareRadioSHA512.Checked = True
        Me.compareRadioSHA512.Location = New System.Drawing.Point(166, 131)
        Me.compareRadioSHA512.Name = "compareRadioSHA512"
        Me.compareRadioSHA512.Size = New System.Drawing.Size(111, 17)
        Me.compareRadioSHA512.TabIndex = 25
        Me.compareRadioSHA512.TabStop = True
        Me.compareRadioSHA512.Text = "SHA-512 (Default)"
        Me.compareRadioSHA512.UseVisualStyleBackColor = True
        '
        'compareRadioSHA384
        '
        Me.compareRadioSHA384.AutoSize = True
        Me.compareRadioSHA384.Location = New System.Drawing.Point(92, 131)
        Me.compareRadioSHA384.Name = "compareRadioSHA384"
        Me.compareRadioSHA384.Size = New System.Drawing.Size(68, 17)
        Me.compareRadioSHA384.TabIndex = 24
        Me.compareRadioSHA384.Text = "SHA-384"
        Me.compareRadioSHA384.UseVisualStyleBackColor = True
        '
        'compareRadioSHA256
        '
        Me.compareRadioSHA256.AutoSize = True
        Me.compareRadioSHA256.Location = New System.Drawing.Point(18, 131)
        Me.compareRadioSHA256.Name = "compareRadioSHA256"
        Me.compareRadioSHA256.Size = New System.Drawing.Size(68, 17)
        Me.compareRadioSHA256.TabIndex = 23
        Me.compareRadioSHA256.Text = "SHA-256"
        Me.compareRadioSHA256.UseVisualStyleBackColor = True
        '
        'compareRadioSHA1
        '
        Me.compareRadioSHA1.AutoSize = True
        Me.compareRadioSHA1.Location = New System.Drawing.Point(283, 131)
        Me.compareRadioSHA1.Name = "compareRadioSHA1"
        Me.compareRadioSHA1.Size = New System.Drawing.Size(259, 17)
        Me.compareRadioSHA1.TabIndex = 22
        Me.compareRadioSHA1.Text = "SHA-1 (Not Recommended, Insecure Hash Type)"
        Me.compareRadioSHA1.UseVisualStyleBackColor = True
        '
        'btnCompareFilesBrowseFile2
        '
        Me.btnCompareFilesBrowseFile2.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnCompareFilesBrowseFile2.Image = Global.Hasher.My.Resources.Resources.folder_explore
        Me.btnCompareFilesBrowseFile2.Location = New System.Drawing.Point(938, 60)
        Me.btnCompareFilesBrowseFile2.Name = "btnCompareFilesBrowseFile2"
        Me.btnCompareFilesBrowseFile2.Size = New System.Drawing.Size(25, 23)
        Me.btnCompareFilesBrowseFile2.TabIndex = 5
        Me.ToolTip.SetToolTip(Me.btnCompareFilesBrowseFile2, "Browse for File #2")
        Me.btnCompareFilesBrowseFile2.UseVisualStyleBackColor = True
        '
        'btnCompareFilesBrowseFile1
        '
        Me.btnCompareFilesBrowseFile1.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnCompareFilesBrowseFile1.Image = Global.Hasher.My.Resources.Resources.folder_explore
        Me.btnCompareFilesBrowseFile1.Location = New System.Drawing.Point(938, 9)
        Me.btnCompareFilesBrowseFile1.Name = "btnCompareFilesBrowseFile1"
        Me.btnCompareFilesBrowseFile1.Size = New System.Drawing.Size(25, 23)
        Me.btnCompareFilesBrowseFile1.TabIndex = 4
        Me.ToolTip.SetToolTip(Me.btnCompareFilesBrowseFile1, "Browse for File #1")
        Me.btnCompareFilesBrowseFile1.UseVisualStyleBackColor = True
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label5.Location = New System.Drawing.Point(15, 65)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(46, 13)
        Me.Label5.TabIndex = 3
        Me.Label5.Text = "File #2"
        '
        'txtFile2
        '
        Me.txtFile2.AllowDrop = True
        Me.txtFile2.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtFile2.BackColor = System.Drawing.SystemColors.Window
        Me.txtFile2.Location = New System.Drawing.Point(67, 62)
        Me.txtFile2.Name = "txtFile2"
        Me.txtFile2.ReadOnly = True
        Me.txtFile2.Size = New System.Drawing.Size(865, 20)
        Me.txtFile2.TabIndex = 2
        '
        'txtFile1
        '
        Me.txtFile1.AllowDrop = True
        Me.txtFile1.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtFile1.BackColor = System.Drawing.SystemColors.Window
        Me.txtFile1.Location = New System.Drawing.Point(67, 11)
        Me.txtFile1.Name = "txtFile1"
        Me.txtFile1.ReadOnly = True
        Me.txtFile1.Size = New System.Drawing.Size(865, 20)
        Me.txtFile1.TabIndex = 1
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label4.Location = New System.Drawing.Point(15, 14)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(46, 13)
        Me.Label4.TabIndex = 0
        Me.Label4.Text = "File #1"
        '
        'tabCompareAgainstKnownHash
        '
        Me.tabCompareAgainstKnownHash.BackColor = System.Drawing.SystemColors.Control
        Me.tabCompareAgainstKnownHash.Controls.Add(Me.pictureBoxVerifyAgainstResults)
        Me.tabCompareAgainstKnownHash.Controls.Add(Me.lblCompareFileAgainstKnownHashType)
        Me.tabCompareAgainstKnownHash.Controls.Add(Me.compareAgainstKnownHashProgressBar)
        Me.tabCompareAgainstKnownHash.Controls.Add(Me.lblCompareAgainstKnownHashStatus)
        Me.tabCompareAgainstKnownHash.Controls.Add(Me.btnCompareAgainstKnownHash)
        Me.tabCompareAgainstKnownHash.Controls.Add(Me.txtKnownHash)
        Me.tabCompareAgainstKnownHash.Controls.Add(Me.Label8)
        Me.tabCompareAgainstKnownHash.Controls.Add(Me.btnBrowseFileForCompareKnownHash)
        Me.tabCompareAgainstKnownHash.Controls.Add(Me.txtFileForKnownHash)
        Me.tabCompareAgainstKnownHash.Controls.Add(Me.Label7)
        Me.tabCompareAgainstKnownHash.Location = New System.Drawing.Point(4, 22)
        Me.tabCompareAgainstKnownHash.Name = "tabCompareAgainstKnownHash"
        Me.tabCompareAgainstKnownHash.Size = New System.Drawing.Size(1040, 363)
        Me.tabCompareAgainstKnownHash.TabIndex = 6
        Me.tabCompareAgainstKnownHash.Text = "Compare file against known hash"
        '
        'pictureBoxVerifyAgainstResults
        '
        Me.pictureBoxVerifyAgainstResults.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.pictureBoxVerifyAgainstResults.Location = New System.Drawing.Point(955, 10)
        Me.pictureBoxVerifyAgainstResults.Name = "pictureBoxVerifyAgainstResults"
        Me.pictureBoxVerifyAgainstResults.Size = New System.Drawing.Size(64, 64)
        Me.pictureBoxVerifyAgainstResults.TabIndex = 35
        Me.pictureBoxVerifyAgainstResults.TabStop = False
        '
        'lblCompareFileAgainstKnownHashType
        '
        Me.lblCompareFileAgainstKnownHashType.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblCompareFileAgainstKnownHashType.AutoSize = True
        Me.lblCompareFileAgainstKnownHashType.Location = New System.Drawing.Point(830, 42)
        Me.lblCompareFileAgainstKnownHashType.Name = "lblCompareFileAgainstKnownHashType"
        Me.lblCompareFileAgainstKnownHashType.Size = New System.Drawing.Size(192, 13)
        Me.lblCompareFileAgainstKnownHashType.TabIndex = 34
        Me.lblCompareFileAgainstKnownHashType.Text = "lblCompareFileAgainstKnownHashType"
        '
        'compareAgainstKnownHashProgressBar
        '
        Me.compareAgainstKnownHashProgressBar.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.compareAgainstKnownHashProgressBar.Location = New System.Drawing.Point(158, 83)
        Me.compareAgainstKnownHashProgressBar.Name = "compareAgainstKnownHashProgressBar"
        Me.compareAgainstKnownHashProgressBar.Size = New System.Drawing.Size(861, 23)
        Me.compareAgainstKnownHashProgressBar.TabIndex = 33
        Me.compareAgainstKnownHashProgressBar.Visible = False
        '
        'lblCompareAgainstKnownHashStatus
        '
        Me.lblCompareAgainstKnownHashStatus.AutoSize = True
        Me.lblCompareAgainstKnownHashStatus.Location = New System.Drawing.Point(158, 67)
        Me.lblCompareAgainstKnownHashStatus.Name = "lblCompareAgainstKnownHashStatus"
        Me.lblCompareAgainstKnownHashStatus.Size = New System.Drawing.Size(140, 13)
        Me.lblCompareAgainstKnownHashStatus.TabIndex = 32
        Me.lblCompareAgainstKnownHashStatus.Text = "(No Background Processes)"
        '
        'btnCompareAgainstKnownHash
        '
        Me.btnCompareAgainstKnownHash.Enabled = False
        Me.btnCompareAgainstKnownHash.Location = New System.Drawing.Point(18, 67)
        Me.btnCompareAgainstKnownHash.Name = "btnCompareAgainstKnownHash"
        Me.btnCompareAgainstKnownHash.Size = New System.Drawing.Size(134, 39)
        Me.btnCompareAgainstKnownHash.TabIndex = 10
        Me.btnCompareAgainstKnownHash.Text = "Compare File Against Known Hash"
        Me.btnCompareAgainstKnownHash.UseVisualStyleBackColor = True
        '
        'txtKnownHash
        '
        Me.txtKnownHash.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtKnownHash.BackColor = System.Drawing.SystemColors.Window
        Me.txtKnownHash.Location = New System.Drawing.Point(99, 38)
        Me.txtKnownHash.MaxLength = 128
        Me.txtKnownHash.Name = "txtKnownHash"
        Me.txtKnownHash.Size = New System.Drawing.Size(724, 20)
        Me.txtKnownHash.TabIndex = 9
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label8.Location = New System.Drawing.Point(15, 41)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(78, 13)
        Me.Label8.TabIndex = 8
        Me.Label8.Text = "Known Hash"
        '
        'btnBrowseFileForCompareKnownHash
        '
        Me.btnBrowseFileForCompareKnownHash.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnBrowseFileForCompareKnownHash.Image = Global.Hasher.My.Resources.Resources.folder_explore
        Me.btnBrowseFileForCompareKnownHash.Location = New System.Drawing.Point(829, 10)
        Me.btnBrowseFileForCompareKnownHash.Name = "btnBrowseFileForCompareKnownHash"
        Me.btnBrowseFileForCompareKnownHash.Size = New System.Drawing.Size(25, 23)
        Me.btnBrowseFileForCompareKnownHash.TabIndex = 7
        Me.ToolTip.SetToolTip(Me.btnBrowseFileForCompareKnownHash, "Browse for File #1")
        Me.btnBrowseFileForCompareKnownHash.UseVisualStyleBackColor = True
        '
        'txtFileForKnownHash
        '
        Me.txtFileForKnownHash.AllowDrop = True
        Me.txtFileForKnownHash.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtFileForKnownHash.BackColor = System.Drawing.SystemColors.Window
        Me.txtFileForKnownHash.Location = New System.Drawing.Point(48, 12)
        Me.txtFileForKnownHash.Name = "txtFileForKnownHash"
        Me.txtFileForKnownHash.ReadOnly = True
        Me.txtFileForKnownHash.Size = New System.Drawing.Size(775, 20)
        Me.txtFileForKnownHash.TabIndex = 6
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label7.Location = New System.Drawing.Point(15, 15)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(27, 13)
        Me.Label7.TabIndex = 5
        Me.Label7.Text = "File"
        '
        'tabSettings
        '
        Me.tabSettings.BackColor = System.Drawing.SystemColors.Control
        Me.tabSettings.Controls.Add(Me.defaultHashType)
        Me.tabSettings.Controls.Add(Me.lblDefaultHashLabel)
        Me.tabSettings.Controls.Add(Me.chkShowPercentageInWindowTitleBar)
        Me.tabSettings.Controls.Add(Me.chkOpenInExplorer)
        Me.tabSettings.Controls.Add(Me.chkDisplayValidChecksumString)
        Me.tabSettings.Controls.Add(Me.btnSetBufferSize)
        Me.tabSettings.Controls.Add(Me.bufferSize)
        Me.tabSettings.Controls.Add(Me.Label9)
        Me.tabSettings.Controls.Add(Me.chkUseMilliseconds)
        Me.tabSettings.Controls.Add(Me.chkSortFileListingAfterAddingFilesToHash)
        Me.tabSettings.Controls.Add(Me.chkSaveChecksumFilesWithRelativePaths)
        Me.tabSettings.Controls.Add(Me.chkSortByFileSizeAfterLoadingHashFile)
        Me.tabSettings.Controls.Add(Me.btnAddHasherToAllFiles)
        Me.tabSettings.Controls.Add(Me.btnAssociate)
        Me.tabSettings.Controls.Add(Me.chkSSL)
        Me.tabSettings.Controls.Add(Me.chkRecurrsiveDirectorySearch)
        Me.tabSettings.Controls.Add(Me.chkDisplayHashesInUpperCase)
        Me.tabSettings.Controls.Add(Me.btnFileNotFoundColor)
        Me.tabSettings.Controls.Add(Me.btnSetColorsBackToDefaults)
        Me.tabSettings.Controls.Add(Me.btnSetNotValidColor)
        Me.tabSettings.Controls.Add(Me.btnSetValidColor)
        Me.tabSettings.Controls.Add(Me.lblFileNotFoundColor)
        Me.tabSettings.Controls.Add(Me.lblNotValidColor)
        Me.tabSettings.Controls.Add(Me.lblValidColor)
        Me.tabSettings.Controls.Add(Me.btnPerformBenchmark)
        Me.tabSettings.Controls.Add(Me.chkUseCommasInNumbers)
        Me.tabSettings.Controls.Add(Me.taskPriority)
        Me.tabSettings.Controls.Add(Me.lblTaskPriorityLabel)
        Me.tabSettings.Controls.Add(Me.chkCheckForUpdates)
        Me.tabSettings.Controls.Add(Me.chkAutoAddExtension)
        Me.tabSettings.Controls.Add(Me.btnSetRoundPercentages)
        Me.tabSettings.Controls.Add(Me.roundPercentages)
        Me.tabSettings.Controls.Add(Me.lblRoundPercentagesLabel)
        Me.tabSettings.Controls.Add(Me.btnSetRoundFileSizes)
        Me.tabSettings.Controls.Add(Me.roundFileSizes)
        Me.tabSettings.Controls.Add(Me.lblRoundFileSizesLabel)
        Me.tabSettings.Location = New System.Drawing.Point(4, 22)
        Me.tabSettings.Name = "tabSettings"
        Me.tabSettings.Size = New System.Drawing.Size(1040, 363)
        Me.tabSettings.TabIndex = 4
        Me.tabSettings.Text = "Settings"
        '
        'btnSetBufferSize
        '
        Me.btnSetBufferSize.Location = New System.Drawing.Point(868, 129)
        Me.btnSetBufferSize.Name = "btnSetBufferSize"
        Me.btnSetBufferSize.Size = New System.Drawing.Size(31, 23)
        Me.btnSetBufferSize.TabIndex = 27
        Me.btnSetBufferSize.Text = "Set"
        Me.btnSetBufferSize.UseVisualStyleBackColor = True
        '
        'bufferSize
        '
        Me.bufferSize.Location = New System.Drawing.Point(812, 132)
        Me.bufferSize.Maximum = New Decimal(New Integer() {16, 0, 0, 0})
        Me.bufferSize.Minimum = New Decimal(New Integer() {1, 0, 0, 0})
        Me.bufferSize.Name = "bufferSize"
        Me.bufferSize.Size = New System.Drawing.Size(50, 20)
        Me.bufferSize.TabIndex = 26
        Me.bufferSize.Value = New Decimal(New Integer() {16, 0, 0, 0})
        '
        'Label9
        '
        Me.Label9.AutoSize = True
        Me.Label9.Location = New System.Drawing.Point(618, 134)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(188, 13)
        Me.Label9.TabIndex = 23
        Me.Label9.Text = "Data Buffer Size (In MBs, Default is 2):"
        Me.ToolTip.SetToolTip(Me.Label9, resources.GetString("Label9.ToolTip"))
        '
        'chkUseMilliseconds
        '
        Me.chkUseMilliseconds.AutoSize = True
        Me.chkUseMilliseconds.Location = New System.Drawing.Point(15, 128)
        Me.chkUseMilliseconds.Name = "chkUseMilliseconds"
        Me.chkUseMilliseconds.Size = New System.Drawing.Size(332, 17)
        Me.chkUseMilliseconds.TabIndex = 7
        Me.chkUseMilliseconds.Text = "Display compute times that are less than a second in milliseconds"
        Me.chkUseMilliseconds.UseVisualStyleBackColor = True
        '
        'chkSortFileListingAfterAddingFilesToHash
        '
        Me.chkSortFileListingAfterAddingFilesToHash.AutoSize = True
        Me.chkSortFileListingAfterAddingFilesToHash.Location = New System.Drawing.Point(15, 82)
        Me.chkSortFileListingAfterAddingFilesToHash.Name = "chkSortFileListingAfterAddingFilesToHash"
        Me.chkSortFileListingAfterAddingFilesToHash.Size = New System.Drawing.Size(262, 17)
        Me.chkSortFileListingAfterAddingFilesToHash.TabIndex = 6
        Me.chkSortFileListingAfterAddingFilesToHash.Text = "Sort file listing after adding files to hash/checksum"
        Me.chkSortFileListingAfterAddingFilesToHash.UseVisualStyleBackColor = True
        '
        'chkSaveChecksumFilesWithRelativePaths
        '
        Me.chkSaveChecksumFilesWithRelativePaths.AutoSize = True
        Me.chkSaveChecksumFilesWithRelativePaths.Location = New System.Drawing.Point(15, 105)
        Me.chkSaveChecksumFilesWithRelativePaths.Name = "chkSaveChecksumFilesWithRelativePaths"
        Me.chkSaveChecksumFilesWithRelativePaths.Size = New System.Drawing.Size(212, 17)
        Me.chkSaveChecksumFilesWithRelativePaths.TabIndex = 5
        Me.chkSaveChecksumFilesWithRelativePaths.Text = "Save checksum files with relative paths"
        Me.chkSaveChecksumFilesWithRelativePaths.UseVisualStyleBackColor = True
        '
        'chkSortByFileSizeAfterLoadingHashFile
        '
        Me.chkSortByFileSizeAfterLoadingHashFile.AutoSize = True
        Me.chkSortByFileSizeAfterLoadingHashFile.Location = New System.Drawing.Point(15, 221)
        Me.chkSortByFileSizeAfterLoadingHashFile.Name = "chkSortByFileSizeAfterLoadingHashFile"
        Me.chkSortByFileSizeAfterLoadingHashFile.Size = New System.Drawing.Size(244, 17)
        Me.chkSortByFileSizeAfterLoadingHashFile.TabIndex = 4
        Me.chkSortByFileSizeAfterLoadingHashFile.Text = "Sort file listing by file size after loading hash file"
        Me.chkSortByFileSizeAfterLoadingHashFile.UseVisualStyleBackColor = True
        '
        'btnAddHasherToAllFiles
        '
        Me.btnAddHasherToAllFiles.Location = New System.Drawing.Point(258, 314)
        Me.btnAddHasherToAllFiles.Name = "btnAddHasherToAllFiles"
        Me.btnAddHasherToAllFiles.Size = New System.Drawing.Size(175, 27)
        Me.btnAddHasherToAllFiles.TabIndex = 3
        Me.btnAddHasherToAllFiles.Text = "Add Hasher to All Files"
        Me.btnAddHasherToAllFiles.UseVisualStyleBackColor = True
        '
        'btnAssociate
        '
        Me.btnAssociate.Location = New System.Drawing.Point(15, 314)
        Me.btnAssociate.Name = "btnAssociate"
        Me.btnAssociate.Size = New System.Drawing.Size(237, 27)
        Me.btnAssociate.TabIndex = 2
        Me.btnAssociate.Text = "Associate File Hash Files with Hasher"
        Me.btnAssociate.UseVisualStyleBackColor = True
        '
        'chkSSL
        '
        Me.chkSSL.AutoSize = True
        Me.chkSSL.Location = New System.Drawing.Point(15, 36)
        Me.chkSSL.Name = "chkSSL"
        Me.chkSSL.Size = New System.Drawing.Size(361, 17)
        Me.chkSSL.TabIndex = 1
        Me.chkSSL.Text = "Use SSL to check for and download program updates (Recommended)"
        Me.chkSSL.UseVisualStyleBackColor = True
        '
        'chkRecurrsiveDirectorySearch
        '
        Me.chkRecurrsiveDirectorySearch.AutoSize = True
        Me.chkRecurrsiveDirectorySearch.Location = New System.Drawing.Point(15, 198)
        Me.chkRecurrsiveDirectorySearch.Name = "chkRecurrsiveDirectorySearch"
        Me.chkRecurrsiveDirectorySearch.Size = New System.Drawing.Size(434, 17)
        Me.chkRecurrsiveDirectorySearch.TabIndex = 0
        Me.chkRecurrsiveDirectorySearch.Text = "When processing directories, search recursively for all files in path including s" &
    "ub-folders"
        Me.chkRecurrsiveDirectorySearch.UseVisualStyleBackColor = True
        '
        'chkDisplayHashesInUpperCase
        '
        Me.chkDisplayHashesInUpperCase.AutoSize = True
        Me.chkDisplayHashesInUpperCase.Location = New System.Drawing.Point(15, 152)
        Me.chkDisplayHashesInUpperCase.Name = "chkDisplayHashesInUpperCase"
        Me.chkDisplayHashesInUpperCase.Size = New System.Drawing.Size(207, 17)
        Me.chkDisplayHashesInUpperCase.TabIndex = 8
        Me.chkDisplayHashesInUpperCase.Text = "Display hashes in UPPERCASE letters"
        Me.chkDisplayHashesInUpperCase.UseVisualStyleBackColor = True
        '
        'btnFileNotFoundColor
        '
        Me.btnFileNotFoundColor.Location = New System.Drawing.Point(718, 71)
        Me.btnFileNotFoundColor.Name = "btnFileNotFoundColor"
        Me.btnFileNotFoundColor.Size = New System.Drawing.Size(181, 23)
        Me.btnFileNotFoundColor.TabIndex = 15
        Me.btnFileNotFoundColor.Text = "Set File Not Found Color"
        Me.btnFileNotFoundColor.UseVisualStyleBackColor = True
        '
        'btnSetColorsBackToDefaults
        '
        Me.btnSetColorsBackToDefaults.Location = New System.Drawing.Point(718, 100)
        Me.btnSetColorsBackToDefaults.Name = "btnSetColorsBackToDefaults"
        Me.btnSetColorsBackToDefaults.Size = New System.Drawing.Size(181, 23)
        Me.btnSetColorsBackToDefaults.TabIndex = 15
        Me.btnSetColorsBackToDefaults.Text = "Set Colors Back to Defaults"
        Me.btnSetColorsBackToDefaults.UseVisualStyleBackColor = True
        '
        'btnSetNotValidColor
        '
        Me.btnSetNotValidColor.Location = New System.Drawing.Point(718, 42)
        Me.btnSetNotValidColor.Name = "btnSetNotValidColor"
        Me.btnSetNotValidColor.Size = New System.Drawing.Size(181, 23)
        Me.btnSetNotValidColor.TabIndex = 14
        Me.btnSetNotValidColor.Text = "Set Not Valid Checksum Color"
        Me.btnSetNotValidColor.UseVisualStyleBackColor = True
        '
        'btnSetValidColor
        '
        Me.btnSetValidColor.Location = New System.Drawing.Point(718, 13)
        Me.btnSetValidColor.Name = "btnSetValidColor"
        Me.btnSetValidColor.Size = New System.Drawing.Size(181, 23)
        Me.btnSetValidColor.TabIndex = 13
        Me.btnSetValidColor.Text = "Set Valid Checksum Color"
        Me.btnSetValidColor.UseVisualStyleBackColor = True
        '
        'lblFileNotFoundColor
        '
        Me.lblFileNotFoundColor.BackColor = System.Drawing.Color.LightGray
        Me.lblFileNotFoundColor.Location = New System.Drawing.Point(621, 71)
        Me.lblFileNotFoundColor.Name = "lblFileNotFoundColor"
        Me.lblFileNotFoundColor.Size = New System.Drawing.Size(91, 23)
        Me.lblFileNotFoundColor.TabIndex = 12
        '
        'lblNotValidColor
        '
        Me.lblNotValidColor.BackColor = System.Drawing.Color.Pink
        Me.lblNotValidColor.Location = New System.Drawing.Point(621, 42)
        Me.lblNotValidColor.Name = "lblNotValidColor"
        Me.lblNotValidColor.Size = New System.Drawing.Size(91, 23)
        Me.lblNotValidColor.TabIndex = 11
        '
        'lblValidColor
        '
        Me.lblValidColor.BackColor = System.Drawing.Color.LightGreen
        Me.lblValidColor.Location = New System.Drawing.Point(621, 13)
        Me.lblValidColor.Name = "lblValidColor"
        Me.lblValidColor.Size = New System.Drawing.Size(91, 23)
        Me.lblValidColor.TabIndex = 10
        '
        'btnPerformBenchmark
        '
        Me.btnPerformBenchmark.Location = New System.Drawing.Point(621, 158)
        Me.btnPerformBenchmark.Name = "btnPerformBenchmark"
        Me.btnPerformBenchmark.Size = New System.Drawing.Size(278, 23)
        Me.btnPerformBenchmark.TabIndex = 28
        Me.btnPerformBenchmark.Text = "Perform Benchmark to Determine Optimal Buffer Size"
        Me.btnPerformBenchmark.UseVisualStyleBackColor = True
        '
        'chkUseCommasInNumbers
        '
        Me.chkUseCommasInNumbers.AutoSize = True
        Me.chkUseCommasInNumbers.Location = New System.Drawing.Point(15, 175)
        Me.chkUseCommasInNumbers.Name = "chkUseCommasInNumbers"
        Me.chkUseCommasInNumbers.Size = New System.Drawing.Size(194, 17)
        Me.chkUseCommasInNumbers.TabIndex = 29
        Me.chkUseCommasInNumbers.Text = "Use commas in numbers (ex. 2,000)"
        Me.chkUseCommasInNumbers.UseVisualStyleBackColor = True
        '
        'taskPriority
        '
        Me.taskPriority.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.taskPriority.FormattingEnabled = True
        Me.taskPriority.Items.AddRange(New Object() {"Lowest", "Below Normal", "Normal", "Above Normal", "Highest"})
        Me.taskPriority.Location = New System.Drawing.Point(774, 187)
        Me.taskPriority.Name = "taskPriority"
        Me.taskPriority.Size = New System.Drawing.Size(125, 21)
        Me.taskPriority.TabIndex = 31
        Me.ToolTip.SetToolTip(Me.taskPriority, "This sets the priority of the task relative to everything else that's running on " &
        "your computer.")
        '
        'lblTaskPriorityLabel
        '
        Me.lblTaskPriorityLabel.AutoSize = True
        Me.lblTaskPriorityLabel.Location = New System.Drawing.Point(618, 190)
        Me.lblTaskPriorityLabel.Name = "lblTaskPriorityLabel"
        Me.lblTaskPriorityLabel.Size = New System.Drawing.Size(150, 13)
        Me.lblTaskPriorityLabel.TabIndex = 30
        Me.lblTaskPriorityLabel.Text = "Task Priority (Default: Highest)"
        Me.ToolTip.SetToolTip(Me.lblTaskPriorityLabel, "This sets the priority of the task relative to everything else that's running on " &
        "your computer.")
        '
        'chkCheckForUpdates
        '
        Me.chkCheckForUpdates.AutoSize = True
        Me.chkCheckForUpdates.Location = New System.Drawing.Point(15, 13)
        Me.chkCheckForUpdates.Name = "chkCheckForUpdates"
        Me.chkCheckForUpdates.Size = New System.Drawing.Size(261, 17)
        Me.chkCheckForUpdates.TabIndex = 32
        Me.chkCheckForUpdates.Text = "Automatically Check for Updates (Recommended)"
        Me.chkCheckForUpdates.UseVisualStyleBackColor = True
        '
        'chkAutoAddExtension
        '
        Me.chkAutoAddExtension.AutoSize = True
        Me.chkAutoAddExtension.Location = New System.Drawing.Point(15, 59)
        Me.chkAutoAddExtension.Name = "chkAutoAddExtension"
        Me.chkAutoAddExtension.Size = New System.Drawing.Size(520, 17)
        Me.chkAutoAddExtension.TabIndex = 33
        Me.chkAutoAddExtension.Text = "Enable adding appropriate file extension to filename when saving results to disk (Highly Recommended!!!)"
        Me.ToolTip.SetToolTip(Me.chkAutoAddExtension, "Enabling this option automatically tacks on the appropriate file extension when s" &
        "aving checksum data to disk.")
        Me.chkAutoAddExtension.UseVisualStyleBackColor = True
        '
        'btnSetRoundPercentages
        '
        Me.btnSetRoundPercentages.Location = New System.Drawing.Point(818, 251)
        Me.btnSetRoundPercentages.Name = "btnSetRoundPercentages"
        Me.btnSetRoundPercentages.Size = New System.Drawing.Size(31, 23)
        Me.btnSetRoundPercentages.TabIndex = 36
        Me.btnSetRoundPercentages.Text = "Set"
        Me.btnSetRoundPercentages.UseVisualStyleBackColor = True
        '
        'roundPercentages
        '
        Me.roundPercentages.Location = New System.Drawing.Point(762, 254)
        Me.roundPercentages.Maximum = New Decimal(New Integer() {4, 0, 0, 0})
        Me.roundPercentages.Name = "roundPercentages"
        Me.roundPercentages.Size = New System.Drawing.Size(50, 20)
        Me.roundPercentages.TabIndex = 35
        Me.roundPercentages.Value = New Decimal(New Integer() {2, 0, 0, 0})
        '
        'lblRoundPercentagesLabel
        '
        Me.lblRoundPercentagesLabel.AutoSize = True
        Me.lblRoundPercentagesLabel.Location = New System.Drawing.Point(618, 245)
        Me.lblRoundPercentagesLabel.Name = "lblRoundPercentagesLabel"
        Me.lblRoundPercentagesLabel.Size = New System.Drawing.Size(138, 39)
        Me.lblRoundPercentagesLabel.TabIndex = 34
        Me.lblRoundPercentagesLabel.Text = "Round percentages to how" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "many numbers after decimal" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "point (Default: 2)"
        '
        'btnSetRoundFileSizes
        '
        Me.btnSetRoundFileSizes.Location = New System.Drawing.Point(868, 217)
        Me.btnSetRoundFileSizes.Name = "btnSetRoundFileSizes"
        Me.btnSetRoundFileSizes.Size = New System.Drawing.Size(31, 23)
        Me.btnSetRoundFileSizes.TabIndex = 36
        Me.btnSetRoundFileSizes.Text = "Set"
        Me.btnSetRoundFileSizes.UseVisualStyleBackColor = True
        '
        'roundFileSizes
        '
        Me.roundFileSizes.Location = New System.Drawing.Point(811, 217)
        Me.roundFileSizes.Maximum = New Decimal(New Integer() {4, 0, 0, 0})
        Me.roundFileSizes.Name = "roundFileSizes"
        Me.roundFileSizes.Size = New System.Drawing.Size(50, 20)
        Me.roundFileSizes.TabIndex = 35
        Me.roundFileSizes.Value = New Decimal(New Integer() {2, 0, 0, 0})
        '
        'lblRoundFileSizesLabel
        '
        Me.lblRoundFileSizesLabel.AutoSize = True
        Me.lblRoundFileSizesLabel.Location = New System.Drawing.Point(618, 211)
        Me.lblRoundFileSizesLabel.Name = "lblRoundFileSizesLabel"
        Me.lblRoundFileSizesLabel.Size = New System.Drawing.Size(187, 26)
        Me.lblRoundFileSizesLabel.TabIndex = 34
        Me.lblRoundFileSizesLabel.Text = "Round file sizes to how many numbers" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "after decimal point (Default: 2)"
        '
        'OpenFileDialog
        '
        Me.OpenFileDialog.Multiselect = True
        Me.OpenFileDialog.Title = "Add Files to List..."
        '
        'pictureBoxCompareFiles
        '
        Me.pictureBoxCompareFiles.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.pictureBoxCompareFiles.Location = New System.Drawing.Point(969, 14)
        Me.pictureBoxCompareFiles.Name = "pictureBoxCompareFiles"
        Me.pictureBoxCompareFiles.Size = New System.Drawing.Size(64, 64)
        Me.pictureBoxCompareFiles.TabIndex = 36
        Me.pictureBoxCompareFiles.TabStop = False
        '
        'chkDisplayValidChecksumString
        '
        Me.chkDisplayValidChecksumString.AutoSize = True
        Me.chkDisplayValidChecksumString.Location = New System.Drawing.Point(15, 245)
        Me.chkDisplayValidChecksumString.Name = "chkDisplayValidChecksumString"
        Me.chkDisplayValidChecksumString.Size = New System.Drawing.Size(485, 17)
        Me.chkDisplayValidChecksumString.TabIndex = 37
        Me.chkDisplayValidChecksumString.Text = "Display ""Valid Checksum"" instead of leaving it blank in fifth column on ""Verify Saved Hashes"" tab"
        Me.chkDisplayValidChecksumString.UseVisualStyleBackColor = True
        '
        'chkOpenInExplorer
        '
        Me.chkOpenInExplorer.AutoSize = True
        Me.chkOpenInExplorer.Location = New System.Drawing.Point(15, 268)
        Me.chkOpenInExplorer.Name = "chkOpenInExplorer"
        Me.chkOpenInExplorer.Size = New System.Drawing.Size(482, 17)
        Me.chkOpenInExplorer.TabIndex = 38
        Me.chkOpenInExplorer.Text = "Open Windows Explorer with checksum file selected after saving checksum file to disk by default"
        Me.chkOpenInExplorer.UseVisualStyleBackColor = True
        '
        'chkShowPercentageInWindowTitleBar
        '
        Me.chkShowPercentageInWindowTitleBar.AutoSize = True
        Me.chkShowPercentageInWindowTitleBar.Location = New System.Drawing.Point(15, 291)
        Me.chkShowPercentageInWindowTitleBar.Name = "chkShowPercentageInWindowTitleBar"
        Me.chkShowPercentageInWindowTitleBar.Size = New System.Drawing.Size(206, 17)
        Me.chkShowPercentageInWindowTitleBar.TabIndex = 39
        Me.chkShowPercentageInWindowTitleBar.Text = "Show Percentage in Window Title Bar"
        Me.chkShowPercentageInWindowTitleBar.UseVisualStyleBackColor = True
        '
        'btnRetestFailedFiles
        '
        Me.btnRetestFailedFiles.Location = New System.Drawing.Point(12, 158)
        Me.btnRetestFailedFiles.Name = "btnRetestFailedFiles"
        Me.btnRetestFailedFiles.Size = New System.Drawing.Size(142, 67)
        Me.btnRetestFailedFiles.TabIndex = 27
        Me.btnRetestFailedFiles.Text = "Retest Failed Files"
        Me.btnRetestFailedFiles.UseVisualStyleBackColor = True
        Me.btnRetestFailedFiles.Visible = False
        '
        'lblDefaultHashLabel
        '
        Me.lblDefaultHashLabel.AutoSize = True
        Me.lblDefaultHashLabel.Location = New System.Drawing.Point(618, 295)
        Me.lblDefaultHashLabel.Name = "lblDefaultHashLabel"
        Me.lblDefaultHashLabel.Size = New System.Drawing.Size(72, 13)
        Me.lblDefaultHashLabel.TabIndex = 40
        Me.lblDefaultHashLabel.Text = "Default Hash:"
        '
        'defaultHashType
        '
        Me.defaultHashType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.defaultHashType.FormattingEnabled = True
        Me.defaultHashType.Items.AddRange(New Object() {"MD5 (Seriously Not Recommended)", "SHA1 (Not Recommended)", "SHA256", "SHA384", "SHA512"})
        Me.defaultHashType.Location = New System.Drawing.Point(696, 291)
        Me.defaultHashType.Name = "defaultHashType"
        Me.defaultHashType.Size = New System.Drawing.Size(203, 21)
        Me.defaultHashType.TabIndex = 41
        Me.ToolTip.SetToolTip(Me.defaultHashType, "This sets the default hash type at program load.")
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1072, 414)
        Me.Controls.Add(Me.TabControl1)
        Me.KeyPreview = True
        Me.MinimumSize = New System.Drawing.Size(1088, 446)
        Me.Name = "Form1"
        Me.Text = "Hasher"
        Me.TabControl1.ResumeLayout(False)
        Me.tabWelcome.ResumeLayout(False)
        Me.tabWelcome.PerformLayout()
        Me.tabHashText.ResumeLayout(False)
        Me.tabHashText.PerformLayout()
        Me.tabHashIndividualFiles.ResumeLayout(False)
        Me.tabHashIndividualFiles.PerformLayout()
        Me.hashIndividualFilesTableLayoutControl.ResumeLayout(False)
        Me.hashIndividualFilesTableLayoutControl.PerformLayout()
        Me.listFilesContextMenu.ResumeLayout(False)
        Me.tabVerifySavedHashes.ResumeLayout(False)
        Me.tabVerifySavedHashes.PerformLayout()
        Me.verifySavedHashesTableLayoutControl.ResumeLayout(False)
        Me.verifySavedHashesTableLayoutControl.PerformLayout()
        Me.verifyListFilesContextMenu.ResumeLayout(False)
        Me.tabCompareFiles.ResumeLayout(False)
        Me.tabCompareFiles.PerformLayout()
        Me.compareFilesTableLayoutControl.ResumeLayout(False)
        Me.compareFilesTableLayoutControl.PerformLayout()
        Me.tabCompareAgainstKnownHash.ResumeLayout(False)
        Me.tabCompareAgainstKnownHash.PerformLayout()
        CType(Me.pictureBoxVerifyAgainstResults, System.ComponentModel.ISupportInitialize).EndInit()
        Me.tabSettings.ResumeLayout(False)
        Me.tabSettings.PerformLayout()
        CType(Me.bufferSize, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.roundPercentages, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.roundFileSizes, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.pictureBoxCompareFiles, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents TabControl1 As TabControl
    Friend WithEvents tabWelcome As TabPage
    Friend WithEvents tabHashText As TabPage
    Friend WithEvents tabHashIndividualFiles As TabPage
    Friend WithEvents btnRemoveSelectedFiles As Button
    Friend WithEvents btnRemoveAllFiles As Button
    Friend WithEvents btnAddFilesInFolder As Button
    Friend WithEvents btnAddIndividualFiles As Button
    Friend WithEvents OpenFileDialog As OpenFileDialog
    Friend WithEvents listFiles As ListView
    Friend WithEvents colFileName As ColumnHeader
    Friend WithEvents lblHashIndividualFilesStep1 As Label
    Friend WithEvents radioSHA1 As RadioButton
    Friend WithEvents radioMD5 As RadioButton
    Friend WithEvents radioSHA512 As RadioButton
    Friend WithEvents radioSHA384 As RadioButton
    Friend WithEvents radioSHA256 As RadioButton
    Friend WithEvents btnComputeHash As Button
    Friend WithEvents lblIndividualFilesStatus As Label
    Friend WithEvents IndividualFilesProgressBar As ProgressBar
    Friend WithEvents btnIndividualFilesCopyToClipboard As Button
    Friend WithEvents btnIndividualFilesSaveResultsToDisk As Button
    Friend WithEvents SaveFileDialog As SaveFileDialog
    Friend WithEvents lblIndividualFilesStatusProcessingFile As Label
    Friend WithEvents FolderBrowserDialog As FolderBrowserDialog
    Friend WithEvents lblHashIndividualFilesStep3 As Label
    Friend WithEvents lblHashIndividualFilesStep2 As Label
    Friend WithEvents lblWelcomeText As Label
    Friend WithEvents tabVerifySavedHashes As TabPage
    Friend WithEvents verifyHashesListFiles As ListView
    Friend WithEvents colFile As ColumnHeader
    Friend WithEvents colResults As ColumnHeader
    Friend WithEvents btnOpenExistingHashFile As Button
    Friend WithEvents VerifyHashProgressBar As ProgressBar
    Friend WithEvents lblVerifyHashStatus As Label
    Friend WithEvents lblVerifyHashStatusProcessingFile As Label
    Friend WithEvents tabSettings As TabPage
    Friend WithEvents chkRecurrsiveDirectorySearch As CheckBox
    Friend WithEvents lblLine As Label
    Friend WithEvents txtTextToHash As TextBox
    Friend WithEvents lblTextToHash As Label
    Friend WithEvents lblHashTextStep1 As Label
    Friend WithEvents Label1 As Label
    Friend WithEvents btnComputeTextHash As Button
    Friend WithEvents Label2 As Label
    Friend WithEvents textRadioMD5 As RadioButton
    Friend WithEvents textRadioSHA512 As RadioButton
    Friend WithEvents textRadioSHA384 As RadioButton
    Friend WithEvents textRadioSHA256 As RadioButton
    Friend WithEvents textRadioSHA1 As RadioButton
    Friend WithEvents btnPasteTextFromWindowsClipboard As Button
    Friend WithEvents txtHashResults As TextBox
    Friend WithEvents Label3 As Label
    Friend WithEvents btnCopyTextHashResultsToClipboard As Button
    Friend WithEvents colChecksum As ColumnHeader
    Friend WithEvents colFileSize As ColumnHeader
    Friend WithEvents colFileSize2 As ColumnHeader
    Friend WithEvents listFilesContextMenu As ContextMenuStrip
    Friend WithEvents CopyHashToClipboardToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents chkSSL As CheckBox
    Friend WithEvents lblDownloadNotification As Label
    Friend WithEvents btnCheckForUpdates As Button
    Friend WithEvents tabCompareFiles As TabPage
    Friend WithEvents btnCompareFilesBrowseFile1 As Button
    Friend WithEvents Label5 As Label
    Friend WithEvents txtFile2 As TextBox
    Friend WithEvents txtFile1 As TextBox
    Friend WithEvents Label4 As Label
    Friend WithEvents btnCompareFilesBrowseFile2 As Button
    Friend WithEvents ToolTip As ToolTip
    Friend WithEvents compareRadioMD5 As RadioButton
    Friend WithEvents compareRadioSHA512 As RadioButton
    Friend WithEvents compareRadioSHA384 As RadioButton
    Friend WithEvents compareRadioSHA256 As RadioButton
    Friend WithEvents compareRadioSHA1 As RadioButton
    Friend WithEvents Label6 As Label
    Friend WithEvents btnCompareFiles As Button
    Friend WithEvents compareFilesProgressBar As ProgressBar
    Friend WithEvents lblCompareFilesStatus As Label
    Friend WithEvents lblFile2Hash As Label
    Friend WithEvents lblFile1Hash As Label
    Friend WithEvents tabCompareAgainstKnownHash As TabPage
    Friend WithEvents txtKnownHash As TextBox
    Friend WithEvents Label8 As Label
    Friend WithEvents btnBrowseFileForCompareKnownHash As Button
    Friend WithEvents txtFileForKnownHash As TextBox
    Friend WithEvents Label7 As Label
    Friend WithEvents compareAgainstKnownHashProgressBar As ProgressBar
    Friend WithEvents lblCompareAgainstKnownHashStatus As Label
    Friend WithEvents btnCompareAgainstKnownHash As Button
    Friend WithEvents lblCompareFileAgainstKnownHashType As Label
    Friend WithEvents lblVerifyFileNameLabel As Label
    Friend WithEvents btnAssociate As Button
    Friend WithEvents btnAddHasherToAllFiles As Button
    Friend WithEvents lblProcessingFile As Label
    Friend WithEvents lblProcessingFileVerify As Label
    Friend WithEvents chkSortByFileSizeAfterLoadingHashFile As CheckBox
    Friend WithEvents chkSaveChecksumFilesWithRelativePaths As CheckBox
    Friend WithEvents chkSortFileListingAfterAddingFilesToHash As CheckBox
    Friend WithEvents btnDonate As Button
    Friend WithEvents colComputeTime As ColumnHeader
    Friend WithEvents colComputeTime2 As ColumnHeader
    Friend WithEvents chkUseMilliseconds As CheckBox
    Friend WithEvents chkDisplayHashesInUpperCase As CheckBox
    Friend WithEvents pictureBoxVerifyAgainstResults As PictureBox
    Friend WithEvents lblValidColor As Label
    Friend WithEvents lblNotValidColor As Label
    Friend WithEvents lblFileNotFoundColor As Label
    Friend WithEvents btnSetValidColor As Button
    Friend WithEvents btnFileNotFoundColor As Button
    Friend WithEvents btnSetNotValidColor As Button
    Friend WithEvents btnSetColorsBackToDefaults As Button
    Friend WithEvents Label9 As Label
    Friend WithEvents bufferSize As NumericUpDown
    Friend WithEvents btnSetBufferSize As Button
    Friend WithEvents btnPerformBenchmark As Button
    Friend WithEvents chkUseCommasInNumbers As CheckBox
    Friend WithEvents hashIndividualFilesAllFilesProgressBar As ProgressBar
    Friend WithEvents verifyIndividualFilesAllFilesProgressBar As ProgressBar
    Friend WithEvents CompareFilesAllFilesProgress As ProgressBar
    Friend WithEvents taskPriority As ComboBox
    Friend WithEvents lblTaskPriorityLabel As Label
    Friend WithEvents chkCheckForUpdates As CheckBox
    Friend WithEvents chkAutoAddExtension As CheckBox
    Friend WithEvents hashIndividualFilesTableLayoutControl As TableLayoutPanel
    Friend WithEvents verifySavedHashesTableLayoutControl As TableLayoutPanel
    Friend WithEvents compareFilesTableLayoutControl As TableLayoutPanel
    Friend WithEvents lblHashIndividualFilesTotalStatus As Label
    Friend WithEvents lblVerifyHashesTotalStatus As Label
    Friend WithEvents btnSetRoundPercentages As Button
    Friend WithEvents roundPercentages As NumericUpDown
    Friend WithEvents lblRoundPercentagesLabel As Label
    Friend WithEvents btnSetRoundFileSizes As Button
    Friend WithEvents roundFileSizes As NumericUpDown
    Friend WithEvents lblRoundFileSizesLabel As Label
    Friend WithEvents btnTransferToHashIndividualFilesTab As Button
    Friend WithEvents colNewHash As ColumnHeader
    Friend WithEvents verifyListFilesContextMenu As ContextMenuStrip
    Friend WithEvents ViewChecksumDifferenceToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents lblCompareFilesAllFilesStatus As Label
    Friend WithEvents pictureBoxCompareFiles As PictureBox
    Friend WithEvents chkDisplayValidChecksumString As CheckBox
    Friend WithEvents chkOpenInExplorer As CheckBox
    Friend WithEvents chkShowPercentageInWindowTitleBar As CheckBox
    Friend WithEvents btnRetestFailedFiles As Button
    Friend WithEvents defaultHashType As ComboBox
    Friend WithEvents lblDefaultHashLabel As Label
End Class
