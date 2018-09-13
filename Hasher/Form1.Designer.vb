<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
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
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.TabControl1 = New System.Windows.Forms.TabControl()
        Me.tabWelcome = New System.Windows.Forms.TabPage()
        Me.lblWelcomeText = New System.Windows.Forms.Label()
        Me.tabHashText = New System.Windows.Forms.TabPage()
        Me.btnCopyTextHashResultsToClipboard = New System.Windows.Forms.Button()
        Me.textRadioRIPEMD160 = New System.Windows.Forms.RadioButton()
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
        Me.radioRIPEMD160 = New System.Windows.Forms.RadioButton()
        Me.lblLine = New System.Windows.Forms.Label()
        Me.lblHashIndividualFilesStep3 = New System.Windows.Forms.Label()
        Me.lblHashIndividualFilesStep2 = New System.Windows.Forms.Label()
        Me.lblIndividualFilesStatusProcessingFile = New System.Windows.Forms.Label()
        Me.btnIndividualFilesSaveResultsToDisk = New System.Windows.Forms.Button()
        Me.btnIndividualFilesCopyToClipboard = New System.Windows.Forms.Button()
        Me.IndividualFilesProgressBar = New System.Windows.Forms.ProgressBar()
        Me.lblIndividualFilesStatus = New System.Windows.Forms.Label()
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
        Me.btnRemoveSelectedFiles = New System.Windows.Forms.Button()
        Me.btnRemoveAllFiles = New System.Windows.Forms.Button()
        Me.btnAddFilesInFolder = New System.Windows.Forms.Button()
        Me.btnAddIndividualFiles = New System.Windows.Forms.Button()
        Me.tabVerifySavedHashes = New System.Windows.Forms.TabPage()
        Me.lblVerifyHashStatusProcessingFile = New System.Windows.Forms.Label()
        Me.VerifyHashProgressBar = New System.Windows.Forms.ProgressBar()
        Me.lblVerifyHashStatus = New System.Windows.Forms.Label()
        Me.verifyHashesListFiles = New System.Windows.Forms.ListView()
        Me.colFile = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.colResults = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.btnOpenExistingHashFile = New System.Windows.Forms.Button()
        Me.tabSettings = New System.Windows.Forms.TabPage()
        Me.chkRecurrsiveDirectorySearch = New System.Windows.Forms.CheckBox()
        Me.OpenFileDialog = New System.Windows.Forms.OpenFileDialog()
        Me.SaveFileDialog = New System.Windows.Forms.SaveFileDialog()
        Me.FolderBrowserDialog = New System.Windows.Forms.FolderBrowserDialog()
        Me.colChecksum = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.colFileSize2 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.TabControl1.SuspendLayout()
        Me.tabWelcome.SuspendLayout()
        Me.tabHashText.SuspendLayout()
        Me.tabHashIndividualFiles.SuspendLayout()
        Me.tabVerifySavedHashes.SuspendLayout()
        Me.tabSettings.SuspendLayout()
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
        Me.tabWelcome.Controls.Add(Me.lblWelcomeText)
        Me.tabWelcome.Location = New System.Drawing.Point(4, 22)
        Me.tabWelcome.Name = "tabWelcome"
        Me.tabWelcome.Padding = New System.Windows.Forms.Padding(3)
        Me.tabWelcome.Size = New System.Drawing.Size(1040, 363)
        Me.tabWelcome.TabIndex = 0
        Me.tabWelcome.Text = "Welcome"
        '
        'lblWelcomeText
        '
        Me.lblWelcomeText.AutoSize = True
        Me.lblWelcomeText.Location = New System.Drawing.Point(16, 19)
        Me.lblWelcomeText.Name = "lblWelcomeText"
        Me.lblWelcomeText.Size = New System.Drawing.Size(261, 39)
        Me.lblWelcomeText.TabIndex = 0
        Me.lblWelcomeText.Text = "Welcome to Hasher, the only hash program you need." & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Written by Tom Parkison."
        '
        'tabHashText
        '
        Me.tabHashText.BackColor = System.Drawing.SystemColors.Control
        Me.tabHashText.Controls.Add(Me.btnCopyTextHashResultsToClipboard)
        Me.tabHashText.Controls.Add(Me.textRadioRIPEMD160)
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
        Me.btnCopyTextHashResultsToClipboard.Enabled = False
        Me.btnCopyTextHashResultsToClipboard.Location = New System.Drawing.Point(243, 328)
        Me.btnCopyTextHashResultsToClipboard.Name = "btnCopyTextHashResultsToClipboard"
        Me.btnCopyTextHashResultsToClipboard.Size = New System.Drawing.Size(156, 23)
        Me.btnCopyTextHashResultsToClipboard.TabIndex = 31
        Me.btnCopyTextHashResultsToClipboard.Text = "Copy Results to Clipboard"
        Me.btnCopyTextHashResultsToClipboard.UseVisualStyleBackColor = True
        '
        'textRadioRIPEMD160
        '
        Me.textRadioRIPEMD160.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.textRadioRIPEMD160.AutoSize = True
        Me.textRadioRIPEMD160.Location = New System.Drawing.Point(243, 250)
        Me.textRadioRIPEMD160.Name = "textRadioRIPEMD160"
        Me.textRadioRIPEMD160.Size = New System.Drawing.Size(85, 17)
        Me.textRadioRIPEMD160.TabIndex = 30
        Me.textRadioRIPEMD160.Text = "RIPEMD160"
        Me.textRadioRIPEMD160.UseVisualStyleBackColor = True
        '
        'txtHashResults
        '
        Me.txtHashResults.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtHashResults.BackColor = System.Drawing.SystemColors.Window
        Me.txtHashResults.Location = New System.Drawing.Point(243, 302)
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
        Me.Label3.Location = New System.Drawing.Point(240, 286)
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
        Me.btnComputeTextHash.Location = New System.Drawing.Point(18, 286)
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
        Me.Label2.Location = New System.Drawing.Point(16, 270)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(158, 13)
        Me.Label2.TabIndex = 25
        Me.Label2.Text = "Step 3: Compute the hash."
        '
        'textRadioMD5
        '
        Me.textRadioMD5.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.textRadioMD5.AutoSize = True
        Me.textRadioMD5.Location = New System.Drawing.Point(599, 250)
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
        Me.textRadioSHA512.Location = New System.Drawing.Point(167, 250)
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
        Me.textRadioSHA384.Location = New System.Drawing.Point(93, 250)
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
        Me.textRadioSHA256.Location = New System.Drawing.Point(19, 250)
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
        Me.textRadioSHA1.Location = New System.Drawing.Point(334, 250)
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
        Me.Label1.Location = New System.Drawing.Point(15, 234)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(179, 13)
        Me.Label1.TabIndex = 19
        Me.Label1.Text = "Step 2: Select your hash type."
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
        Me.txtTextToHash.Size = New System.Drawing.Size(854, 202)
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
        Me.tabHashIndividualFiles.Controls.Add(Me.radioRIPEMD160)
        Me.tabHashIndividualFiles.Controls.Add(Me.lblLine)
        Me.tabHashIndividualFiles.Controls.Add(Me.lblHashIndividualFilesStep3)
        Me.tabHashIndividualFiles.Controls.Add(Me.lblHashIndividualFilesStep2)
        Me.tabHashIndividualFiles.Controls.Add(Me.lblIndividualFilesStatusProcessingFile)
        Me.tabHashIndividualFiles.Controls.Add(Me.btnIndividualFilesSaveResultsToDisk)
        Me.tabHashIndividualFiles.Controls.Add(Me.btnIndividualFilesCopyToClipboard)
        Me.tabHashIndividualFiles.Controls.Add(Me.IndividualFilesProgressBar)
        Me.tabHashIndividualFiles.Controls.Add(Me.lblIndividualFilesStatus)
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
        'radioRIPEMD160
        '
        Me.radioRIPEMD160.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.radioRIPEMD160.AutoSize = True
        Me.radioRIPEMD160.Location = New System.Drawing.Point(240, 224)
        Me.radioRIPEMD160.Name = "radioRIPEMD160"
        Me.radioRIPEMD160.Size = New System.Drawing.Size(85, 17)
        Me.radioRIPEMD160.TabIndex = 21
        Me.radioRIPEMD160.Text = "RIPEMD160"
        Me.radioRIPEMD160.UseVisualStyleBackColor = True
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
        Me.lblHashIndividualFilesStep3.Location = New System.Drawing.Point(15, 244)
        Me.lblHashIndividualFilesStep3.Name = "lblHashIndividualFilesStep3"
        Me.lblHashIndividualFilesStep3.Size = New System.Drawing.Size(171, 13)
        Me.lblHashIndividualFilesStep3.TabIndex = 19
        Me.lblHashIndividualFilesStep3.Text = "Step 3: Compute the hashes."
        '
        'lblHashIndividualFilesStep2
        '
        Me.lblHashIndividualFilesStep2.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.lblHashIndividualFilesStep2.AutoSize = True
        Me.lblHashIndividualFilesStep2.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblHashIndividualFilesStep2.Location = New System.Drawing.Point(15, 208)
        Me.lblHashIndividualFilesStep2.Name = "lblHashIndividualFilesStep2"
        Me.lblHashIndividualFilesStep2.Size = New System.Drawing.Size(179, 13)
        Me.lblHashIndividualFilesStep2.TabIndex = 18
        Me.lblHashIndividualFilesStep2.Text = "Step 2: Select your hash type."
        '
        'lblIndividualFilesStatusProcessingFile
        '
        Me.lblIndividualFilesStatusProcessingFile.AutoSize = True
        Me.lblIndividualFilesStatusProcessingFile.Location = New System.Drawing.Point(687, 260)
        Me.lblIndividualFilesStatusProcessingFile.Name = "lblIndividualFilesStatusProcessingFile"
        Me.lblIndividualFilesStatusProcessingFile.Size = New System.Drawing.Size(37, 13)
        Me.lblIndividualFilesStatusProcessingFile.TabIndex = 17
        Me.lblIndividualFilesStatusProcessingFile.Text = "fghfgff"
        '
        'btnIndividualFilesSaveResultsToDisk
        '
        Me.btnIndividualFilesSaveResultsToDisk.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.btnIndividualFilesSaveResultsToDisk.Enabled = False
        Me.btnIndividualFilesSaveResultsToDisk.Location = New System.Drawing.Point(237, 337)
        Me.btnIndividualFilesSaveResultsToDisk.Name = "btnIndividualFilesSaveResultsToDisk"
        Me.btnIndividualFilesSaveResultsToDisk.Size = New System.Drawing.Size(216, 23)
        Me.btnIndividualFilesSaveResultsToDisk.TabIndex = 16
        Me.btnIndividualFilesSaveResultsToDisk.Text = "Save Results to Disk ..."
        Me.btnIndividualFilesSaveResultsToDisk.UseVisualStyleBackColor = True
        '
        'btnIndividualFilesCopyToClipboard
        '
        Me.btnIndividualFilesCopyToClipboard.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.btnIndividualFilesCopyToClipboard.Enabled = False
        Me.btnIndividualFilesCopyToClipboard.Location = New System.Drawing.Point(15, 337)
        Me.btnIndividualFilesCopyToClipboard.Name = "btnIndividualFilesCopyToClipboard"
        Me.btnIndividualFilesCopyToClipboard.Size = New System.Drawing.Size(216, 23)
        Me.btnIndividualFilesCopyToClipboard.TabIndex = 15
        Me.btnIndividualFilesCopyToClipboard.Text = "Copy Results to Clipboard"
        Me.btnIndividualFilesCopyToClipboard.UseVisualStyleBackColor = True
        '
        'IndividualFilesProgressBar
        '
        Me.IndividualFilesProgressBar.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.IndividualFilesProgressBar.Location = New System.Drawing.Point(237, 276)
        Me.IndividualFilesProgressBar.Name = "IndividualFilesProgressBar"
        Me.IndividualFilesProgressBar.Size = New System.Drawing.Size(800, 23)
        Me.IndividualFilesProgressBar.TabIndex = 14
        '
        'lblIndividualFilesStatus
        '
        Me.lblIndividualFilesStatus.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.lblIndividualFilesStatus.AutoSize = True
        Me.lblIndividualFilesStatus.Location = New System.Drawing.Point(237, 260)
        Me.lblIndividualFilesStatus.Name = "lblIndividualFilesStatus"
        Me.lblIndividualFilesStatus.Size = New System.Drawing.Size(140, 13)
        Me.lblIndividualFilesStatus.TabIndex = 13
        Me.lblIndividualFilesStatus.Text = "(No Background Processes)"
        '
        'btnComputeHash
        '
        Me.btnComputeHash.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.btnComputeHash.Enabled = False
        Me.btnComputeHash.Location = New System.Drawing.Point(15, 260)
        Me.btnComputeHash.Name = "btnComputeHash"
        Me.btnComputeHash.Size = New System.Drawing.Size(216, 71)
        Me.btnComputeHash.TabIndex = 12
        Me.btnComputeHash.Text = "Compute Hash"
        Me.btnComputeHash.UseVisualStyleBackColor = True
        '
        'radioMD5
        '
        Me.radioMD5.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.radioMD5.AutoSize = True
        Me.radioMD5.Location = New System.Drawing.Point(596, 224)
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
        Me.radioSHA512.Location = New System.Drawing.Point(166, 224)
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
        Me.radioSHA384.Location = New System.Drawing.Point(92, 224)
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
        Me.radioSHA256.Location = New System.Drawing.Point(18, 224)
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
        Me.radioSHA1.Location = New System.Drawing.Point(331, 224)
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
        Me.listFiles.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.colFileName, Me.colFileSize, Me.colChecksum})
        Me.listFiles.FullRowSelect = True
        Me.listFiles.Location = New System.Drawing.Point(160, 20)
        Me.listFiles.Name = "listFiles"
        Me.listFiles.Size = New System.Drawing.Size(877, 185)
        Me.listFiles.TabIndex = 5
        Me.listFiles.UseCompatibleStateImageBehavior = False
        Me.listFiles.View = System.Windows.Forms.View.Details
        '
        'colFileName
        '
        Me.colFileName.Text = "File Name"
        Me.colFileName.Width = 528
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
        Me.btnRemoveAllFiles.Text = "Remove &All Files"
        Me.btnRemoveAllFiles.UseVisualStyleBackColor = True
        '
        'btnAddFilesInFolder
        '
        Me.btnAddFilesInFolder.Location = New System.Drawing.Point(15, 49)
        Me.btnAddFilesInFolder.Name = "btnAddFilesInFolder"
        Me.btnAddFilesInFolder.Size = New System.Drawing.Size(139, 23)
        Me.btnAddFilesInFolder.TabIndex = 1
        Me.btnAddFilesInFolder.Text = "Add File(s) in Folder ..."
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
        Me.tabVerifySavedHashes.Controls.Add(Me.lblVerifyHashStatusProcessingFile)
        Me.tabVerifySavedHashes.Controls.Add(Me.VerifyHashProgressBar)
        Me.tabVerifySavedHashes.Controls.Add(Me.lblVerifyHashStatus)
        Me.tabVerifySavedHashes.Controls.Add(Me.verifyHashesListFiles)
        Me.tabVerifySavedHashes.Controls.Add(Me.btnOpenExistingHashFile)
        Me.tabVerifySavedHashes.Location = New System.Drawing.Point(4, 22)
        Me.tabVerifySavedHashes.Name = "tabVerifySavedHashes"
        Me.tabVerifySavedHashes.Size = New System.Drawing.Size(1040, 363)
        Me.tabVerifySavedHashes.TabIndex = 3
        Me.tabVerifySavedHashes.Text = "Verify Saved Hashes"
        '
        'lblVerifyHashStatusProcessingFile
        '
        Me.lblVerifyHashStatusProcessingFile.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.lblVerifyHashStatusProcessingFile.AutoSize = True
        Me.lblVerifyHashStatusProcessingFile.Location = New System.Drawing.Point(677, 321)
        Me.lblVerifyHashStatusProcessingFile.Name = "lblVerifyHashStatusProcessingFile"
        Me.lblVerifyHashStatusProcessingFile.Size = New System.Drawing.Size(37, 13)
        Me.lblVerifyHashStatusProcessingFile.TabIndex = 18
        Me.lblVerifyHashStatusProcessingFile.Text = "dfgdfd"
        '
        'VerifyHashProgressBar
        '
        Me.VerifyHashProgressBar.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.VerifyHashProgressBar.Location = New System.Drawing.Point(157, 337)
        Me.VerifyHashProgressBar.Name = "VerifyHashProgressBar"
        Me.VerifyHashProgressBar.Size = New System.Drawing.Size(880, 23)
        Me.VerifyHashProgressBar.TabIndex = 16
        '
        'lblVerifyHashStatus
        '
        Me.lblVerifyHashStatus.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.lblVerifyHashStatus.AutoSize = True
        Me.lblVerifyHashStatus.Location = New System.Drawing.Point(157, 321)
        Me.lblVerifyHashStatus.Name = "lblVerifyHashStatus"
        Me.lblVerifyHashStatus.Size = New System.Drawing.Size(140, 13)
        Me.lblVerifyHashStatus.TabIndex = 15
        Me.lblVerifyHashStatus.Text = "(No Background Processes)"
        '
        'verifyHashesListFiles
        '
        Me.verifyHashesListFiles.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.verifyHashesListFiles.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.colFile, Me.colFileSize2, Me.colResults})
        Me.verifyHashesListFiles.FullRowSelect = True
        Me.verifyHashesListFiles.Location = New System.Drawing.Point(160, 12)
        Me.verifyHashesListFiles.Name = "verifyHashesListFiles"
        Me.verifyHashesListFiles.Size = New System.Drawing.Size(877, 306)
        Me.verifyHashesListFiles.TabIndex = 6
        Me.verifyHashesListFiles.UseCompatibleStateImageBehavior = False
        Me.verifyHashesListFiles.View = System.Windows.Forms.View.Details
        '
        'colFile
        '
        Me.colFile.Text = "File Name"
        Me.colFile.Width = 557
        '
        'colResults
        '
        Me.colResults.DisplayIndex = 2
        Me.colResults.Text = "Results"
        Me.colResults.Width = 72
        '
        'btnOpenExistingHashFile
        '
        Me.btnOpenExistingHashFile.Location = New System.Drawing.Point(12, 12)
        Me.btnOpenExistingHashFile.Name = "btnOpenExistingHashFile"
        Me.btnOpenExistingHashFile.Size = New System.Drawing.Size(142, 67)
        Me.btnOpenExistingHashFile.TabIndex = 0
        Me.btnOpenExistingHashFile.Text = "Open Hash File"
        Me.btnOpenExistingHashFile.UseVisualStyleBackColor = True
        '
        'tabSettings
        '
        Me.tabSettings.BackColor = System.Drawing.SystemColors.Control
        Me.tabSettings.Controls.Add(Me.chkRecurrsiveDirectorySearch)
        Me.tabSettings.Location = New System.Drawing.Point(4, 22)
        Me.tabSettings.Name = "tabSettings"
        Me.tabSettings.Size = New System.Drawing.Size(1040, 363)
        Me.tabSettings.TabIndex = 4
        Me.tabSettings.Text = "Settings"
        '
        'chkRecurrsiveDirectorySearch
        '
        Me.chkRecurrsiveDirectorySearch.AutoSize = True
        Me.chkRecurrsiveDirectorySearch.Location = New System.Drawing.Point(15, 13)
        Me.chkRecurrsiveDirectorySearch.Name = "chkRecurrsiveDirectorySearch"
        Me.chkRecurrsiveDirectorySearch.Size = New System.Drawing.Size(434, 17)
        Me.chkRecurrsiveDirectorySearch.TabIndex = 0
        Me.chkRecurrsiveDirectorySearch.Text = "When processing directories, search recursively for all files in path including s" &
    "ub-folders"
        Me.chkRecurrsiveDirectorySearch.UseVisualStyleBackColor = True
        '
        'OpenFileDialog
        '
        Me.OpenFileDialog.Multiselect = True
        Me.OpenFileDialog.Title = "Add Files to List..."
        '
        'colChecksum
        '
        Me.colChecksum.Text = "Hash/Checksum"
        Me.colChecksum.Width = 241
        '
        'colFileSize
        '
        Me.colFileSize.Text = "File Size"
        Me.colFileSize.Width = 70
        '
        'colFileSize2
        '
        Me.colFileSize2.DisplayIndex = 1
        Me.colFileSize2.Text = "File Size"
        Me.colFileSize2.Width = 87
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1072, 407)
        Me.Controls.Add(Me.TabControl1)
        Me.MinimumSize = New System.Drawing.Size(904, 446)
        Me.Name = "Form1"
        Me.Text = "Hasher"
        Me.TabControl1.ResumeLayout(False)
        Me.tabWelcome.ResumeLayout(False)
        Me.tabWelcome.PerformLayout()
        Me.tabHashText.ResumeLayout(False)
        Me.tabHashText.PerformLayout()
        Me.tabHashIndividualFiles.ResumeLayout(False)
        Me.tabHashIndividualFiles.PerformLayout()
        Me.tabVerifySavedHashes.ResumeLayout(False)
        Me.tabVerifySavedHashes.PerformLayout()
        Me.tabSettings.ResumeLayout(False)
        Me.tabSettings.PerformLayout()
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
    Friend WithEvents textRadioRIPEMD160 As RadioButton
    Friend WithEvents radioRIPEMD160 As RadioButton
    Friend WithEvents btnCopyTextHashResultsToClipboard As Button
    Friend WithEvents colChecksum As ColumnHeader
    Friend WithEvents colFileSize As ColumnHeader
    Friend WithEvents colFileSize2 As ColumnHeader
End Class
