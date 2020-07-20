namespace StudioGet
{
    partial class MainFrm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainFrm));
            this.menuStripMain = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.exportSelectedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selectAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selectNoneToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStripMain = new System.Windows.Forms.StatusStrip();
            this.statusLabelStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusProgressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.panelRight = new System.Windows.Forms.Panel();
            this.groupBoxCharater = new System.Windows.Forms.GroupBox();
            this.buttonChangeImage = new System.Windows.Forms.Button();
            this.textSex = new System.Windows.Forms.Label();
            this.labelSex = new System.Windows.Forms.Label();
            this.textName = new System.Windows.Forms.Label();
            this.labelName = new System.Windows.Forms.Label();
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.panelBottom = new System.Windows.Forms.Panel();
            this.buttonExport = new System.Windows.Forms.Button();
            this.checkBoxCoord = new System.Windows.Forms.CheckBox();
            this.buttonBrowse = new System.Windows.Forms.Button();
            this.textBoxOutput = new System.Windows.Forms.TextBox();
            this.labelFolder = new System.Windows.Forms.Label();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.listView = new System.Windows.Forms.ListView();
            this.colIndex = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colSex = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colFileName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.backgroundWorkerLoad = new System.ComponentModel.BackgroundWorker();
            this.backgroundWorkerExtract = new System.ComponentModel.BackgroundWorker();
            this.openFileDialogImage = new System.Windows.Forms.OpenFileDialog();
            this.menuStripMain.SuspendLayout();
            this.statusStripMain.SuspendLayout();
            this.panelRight.SuspendLayout();
            this.groupBoxCharater.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            this.panelBottom.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStripMain
            // 
            this.menuStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStripMain.Location = new System.Drawing.Point(0, 0);
            this.menuStripMain.Name = "menuStripMain";
            this.menuStripMain.Size = new System.Drawing.Size(834, 24);
            this.menuStripMain.TabIndex = 0;
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.toolStripSeparator,
            this.exportSelectedToolStripMenuItem,
            this.exportAllToolStripMenuItem,
            this.toolStripSeparator2,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("openToolStripMenuItem.Image")));
            this.openToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.openToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
            this.openToolStripMenuItem.Text = "&Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // toolStripSeparator
            // 
            this.toolStripSeparator.Name = "toolStripSeparator";
            this.toolStripSeparator.Size = new System.Drawing.Size(152, 6);
            // 
            // exportSelectedToolStripMenuItem
            // 
            this.exportSelectedToolStripMenuItem.Name = "exportSelectedToolStripMenuItem";
            this.exportSelectedToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
            this.exportSelectedToolStripMenuItem.Text = "E&xport Selected";
            this.exportSelectedToolStripMenuItem.Click += new System.EventHandler(this.exportSelectedToolStripMenuItem_Click);
            // 
            // exportAllToolStripMenuItem
            // 
            this.exportAllToolStripMenuItem.Name = "exportAllToolStripMenuItem";
            this.exportAllToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
            this.exportAllToolStripMenuItem.Text = "&Export all";
            this.exportAllToolStripMenuItem.Click += new System.EventHandler(this.exportAllToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(152, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
            this.exitToolStripMenuItem.Text = "E&xit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.clearToolStripMenuItem,
            this.selectAllToolStripMenuItem,
            this.selectNoneToolStripMenuItem,
            this.toolStripMenuItem1,
            this.optionsToolStripMenuItem});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
            this.editToolStripMenuItem.Text = "&Edit";
            // 
            // clearToolStripMenuItem
            // 
            this.clearToolStripMenuItem.Name = "clearToolStripMenuItem";
            this.clearToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.clearToolStripMenuItem.Text = "&Clear";
            this.clearToolStripMenuItem.Click += new System.EventHandler(this.clearToolStripMenuItem_Click);
            // 
            // selectAllToolStripMenuItem
            // 
            this.selectAllToolStripMenuItem.Name = "selectAllToolStripMenuItem";
            this.selectAllToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.A)));
            this.selectAllToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.selectAllToolStripMenuItem.Text = "Select &All";
            this.selectAllToolStripMenuItem.Click += new System.EventHandler(this.selectAllToolStripMenuItem_Click);
            // 
            // selectNoneToolStripMenuItem
            // 
            this.selectNoneToolStripMenuItem.Name = "selectNoneToolStripMenuItem";
            this.selectNoneToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.selectNoneToolStripMenuItem.Text = "Select &None";
            this.selectNoneToolStripMenuItem.Click += new System.EventHandler(this.selectNoneToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(161, 6);
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.optionsToolStripMenuItem.Text = "&Options ...";
            this.optionsToolStripMenuItem.Click += new System.EventHandler(this.optionsToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "&Help";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F1;
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.aboutToolStripMenuItem.Text = "&About...";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // statusStripMain
            // 
            this.statusStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusLabelStatus,
            this.statusProgressBar});
            this.statusStripMain.Location = new System.Drawing.Point(0, 539);
            this.statusStripMain.Name = "statusStripMain";
            this.statusStripMain.Size = new System.Drawing.Size(834, 22);
            this.statusStripMain.TabIndex = 4;
            // 
            // statusLabelStatus
            // 
            this.statusLabelStatus.Name = "statusLabelStatus";
            this.statusLabelStatus.Size = new System.Drawing.Size(717, 17);
            this.statusLabelStatus.Spring = true;
            this.statusLabelStatus.Text = "Ready";
            this.statusLabelStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // statusProgressBar
            // 
            this.statusProgressBar.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.statusProgressBar.Name = "statusProgressBar";
            this.statusProgressBar.Size = new System.Drawing.Size(100, 16);
            // 
            // panelRight
            // 
            this.panelRight.Controls.Add(this.groupBoxCharater);
            this.panelRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.panelRight.Location = new System.Drawing.Point(550, 24);
            this.panelRight.Name = "panelRight";
            this.panelRight.Padding = new System.Windows.Forms.Padding(8);
            this.panelRight.Size = new System.Drawing.Size(284, 515);
            this.panelRight.TabIndex = 2;
            // 
            // groupBoxCharater
            // 
            this.groupBoxCharater.Controls.Add(this.buttonChangeImage);
            this.groupBoxCharater.Controls.Add(this.textSex);
            this.groupBoxCharater.Controls.Add(this.labelSex);
            this.groupBoxCharater.Controls.Add(this.textName);
            this.groupBoxCharater.Controls.Add(this.labelName);
            this.groupBoxCharater.Controls.Add(this.pictureBox);
            this.groupBoxCharater.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxCharater.Location = new System.Drawing.Point(8, 8);
            this.groupBoxCharater.Name = "groupBoxCharater";
            this.groupBoxCharater.Size = new System.Drawing.Size(268, 499);
            this.groupBoxCharater.TabIndex = 0;
            this.groupBoxCharater.TabStop = false;
            this.groupBoxCharater.Text = "Character info";
            // 
            // buttonChangeImage
            // 
            this.buttonChangeImage.Location = new System.Drawing.Point(9, 456);
            this.buttonChangeImage.Name = "buttonChangeImage";
            this.buttonChangeImage.Size = new System.Drawing.Size(99, 34);
            this.buttonChangeImage.TabIndex = 0;
            this.buttonChangeImage.Text = "Change Image";
            this.buttonChangeImage.UseVisualStyleBackColor = true;
            this.buttonChangeImage.Click += new System.EventHandler(this.buttonChangeImage_Click);
            // 
            // textSex
            // 
            this.textSex.Location = new System.Drawing.Point(56, 412);
            this.textSex.Name = "textSex";
            this.textSex.Size = new System.Drawing.Size(204, 17);
            this.textSex.TabIndex = 4;
            this.textSex.Text = "-";
            // 
            // labelSex
            // 
            this.labelSex.AutoSize = true;
            this.labelSex.Location = new System.Drawing.Point(6, 412);
            this.labelSex.Name = "labelSex";
            this.labelSex.Size = new System.Drawing.Size(25, 13);
            this.labelSex.TabIndex = 3;
            this.labelSex.Text = "Sex";
            // 
            // textName
            // 
            this.textName.Location = new System.Drawing.Point(56, 387);
            this.textName.Name = "textName";
            this.textName.Size = new System.Drawing.Size(204, 13);
            this.textName.TabIndex = 2;
            this.textName.Text = "-";
            // 
            // labelName
            // 
            this.labelName.AutoSize = true;
            this.labelName.Location = new System.Drawing.Point(6, 387);
            this.labelName.Name = "labelName";
            this.labelName.Size = new System.Drawing.Size(35, 13);
            this.labelName.TabIndex = 1;
            this.labelName.Text = "Name";
            // 
            // pictureBox
            // 
            this.pictureBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(126)))), ((int)(((byte)(116)))), ((int)(((byte)(146)))));
            this.pictureBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox.Location = new System.Drawing.Point(8, 20);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(252, 352);
            this.pictureBox.TabIndex = 0;
            this.pictureBox.TabStop = false;
            // 
            // panelBottom
            // 
            this.panelBottom.Controls.Add(this.buttonExport);
            this.panelBottom.Controls.Add(this.checkBoxCoord);
            this.panelBottom.Controls.Add(this.buttonBrowse);
            this.panelBottom.Controls.Add(this.textBoxOutput);
            this.panelBottom.Controls.Add(this.labelFolder);
            this.panelBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelBottom.Location = new System.Drawing.Point(0, 421);
            this.panelBottom.Name = "panelBottom";
            this.panelBottom.Size = new System.Drawing.Size(550, 118);
            this.panelBottom.TabIndex = 3;
            // 
            // buttonExport
            // 
            this.buttonExport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonExport.Location = new System.Drawing.Point(397, 56);
            this.buttonExport.Name = "buttonExport";
            this.buttonExport.Size = new System.Drawing.Size(140, 45);
            this.buttonExport.TabIndex = 4;
            this.buttonExport.Text = "Export All";
            this.buttonExport.UseVisualStyleBackColor = true;
            this.buttonExport.Click += new System.EventHandler(this.buttonExport_Click);
            // 
            // checkBoxCoord
            // 
            this.checkBoxCoord.AutoSize = true;
            this.checkBoxCoord.Location = new System.Drawing.Point(86, 56);
            this.checkBoxCoord.Name = "checkBoxCoord";
            this.checkBoxCoord.Size = new System.Drawing.Size(109, 17);
            this.checkBoxCoord.TabIndex = 3;
            this.checkBoxCoord.Text = "Export coordinate";
            this.checkBoxCoord.UseVisualStyleBackColor = true;
            // 
            // buttonBrowse
            // 
            this.buttonBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonBrowse.Location = new System.Drawing.Point(501, 18);
            this.buttonBrowse.Name = "buttonBrowse";
            this.buttonBrowse.Size = new System.Drawing.Size(34, 23);
            this.buttonBrowse.TabIndex = 2;
            this.buttonBrowse.Text = "...";
            this.buttonBrowse.UseVisualStyleBackColor = true;
            this.buttonBrowse.Click += new System.EventHandler(this.buttonBrowse_Click);
            // 
            // textBoxOutput
            // 
            this.textBoxOutput.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxOutput.BackColor = System.Drawing.SystemColors.Window;
            this.textBoxOutput.Location = new System.Drawing.Point(86, 20);
            this.textBoxOutput.Name = "textBoxOutput";
            this.textBoxOutput.ReadOnly = true;
            this.textBoxOutput.Size = new System.Drawing.Size(409, 20);
            this.textBoxOutput.TabIndex = 1;
            // 
            // labelFolder
            // 
            this.labelFolder.AutoSize = true;
            this.labelFolder.Location = new System.Drawing.Point(12, 23);
            this.labelFolder.Name = "labelFolder";
            this.labelFolder.Size = new System.Drawing.Size(68, 13);
            this.labelFolder.TabIndex = 0;
            this.labelFolder.Text = "&Output folder";
            // 
            // openFileDialog
            // 
            this.openFileDialog.Multiselect = true;
            // 
            // listView
            // 
            this.listView.AllowDrop = true;
            this.listView.CheckBoxes = true;
            this.listView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colIndex,
            this.colName,
            this.colSex,
            this.colFileName});
            this.listView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView.FullRowSelect = true;
            this.listView.HideSelection = false;
            this.listView.Location = new System.Drawing.Point(0, 24);
            this.listView.MultiSelect = false;
            this.listView.Name = "listView";
            this.listView.Size = new System.Drawing.Size(550, 397);
            this.listView.TabIndex = 1;
            this.listView.UseCompatibleStateImageBehavior = false;
            this.listView.View = System.Windows.Forms.View.Details;
            this.listView.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.listView_ItemChecked);
            this.listView.SelectedIndexChanged += new System.EventHandler(this.listView_SelectedIndexChanged);
            this.listView.DragDrop += new System.Windows.Forms.DragEventHandler(this.listView_DragDrop);
            this.listView.DragEnter += new System.Windows.Forms.DragEventHandler(this.listView_DragEnter);
            // 
            // colIndex
            // 
            this.colIndex.Text = "#";
            // 
            // colName
            // 
            this.colName.Text = "Name";
            this.colName.Width = 240;
            // 
            // colSex
            // 
            this.colSex.Text = "Sex";
            // 
            // colFileName
            // 
            this.colFileName.Text = "Source File Name";
            this.colFileName.Width = 160;
            // 
            // backgroundWorkerLoad
            // 
            this.backgroundWorkerLoad.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerLoad_DoWork);
            this.backgroundWorkerLoad.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorkerLoad_RunWorkerCompleted);
            // 
            // backgroundWorkerExtract
            // 
            this.backgroundWorkerExtract.WorkerReportsProgress = true;
            this.backgroundWorkerExtract.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerExtract_DoWork);
            this.backgroundWorkerExtract.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorkerExtract_ProgressChanged);
            this.backgroundWorkerExtract.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorkerExtract_RunWorkerCompleted);
            // 
            // MainFrm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(834, 561);
            this.Controls.Add(this.listView);
            this.Controls.Add(this.panelBottom);
            this.Controls.Add(this.panelRight);
            this.Controls.Add(this.statusStripMain);
            this.Controls.Add(this.menuStripMain);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStripMain;
            this.MinimumSize = new System.Drawing.Size(700, 600);
            this.Name = "MainFrm";
            this.Text = "Studio Get";
            this.menuStripMain.ResumeLayout(false);
            this.menuStripMain.PerformLayout();
            this.statusStripMain.ResumeLayout(false);
            this.statusStripMain.PerformLayout();
            this.panelRight.ResumeLayout(false);
            this.groupBoxCharater.ResumeLayout(false);
            this.groupBoxCharater.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            this.panelBottom.ResumeLayout(false);
            this.panelBottom.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStripMain;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator;
        private System.Windows.Forms.ToolStripMenuItem exportAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem selectAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.StatusStrip statusStripMain;
        private System.Windows.Forms.ToolStripProgressBar statusProgressBar;
        private System.Windows.Forms.ToolStripStatusLabel statusLabelStatus;
        private System.Windows.Forms.Panel panelRight;
        private System.Windows.Forms.PictureBox pictureBox;
        private System.Windows.Forms.Panel panelBottom;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
        private System.Windows.Forms.TextBox textBoxOutput;
        private System.Windows.Forms.Label labelFolder;
        private System.Windows.Forms.Button buttonBrowse;
        private System.Windows.Forms.ListView listView;
        private System.Windows.Forms.ColumnHeader colIndex;
        private System.Windows.Forms.ColumnHeader colName;
        private System.Windows.Forms.ColumnHeader colSex;
        private System.Windows.Forms.CheckBox checkBoxCoord;
        private System.Windows.Forms.Button buttonExport;
        private System.Windows.Forms.GroupBox groupBoxCharater;
        private System.Windows.Forms.Label textSex;
        private System.Windows.Forms.Label labelSex;
        private System.Windows.Forms.Label textName;
        private System.Windows.Forms.Label labelName;
        private System.Windows.Forms.ToolStripMenuItem exportSelectedToolStripMenuItem;
        private System.ComponentModel.BackgroundWorker backgroundWorkerLoad;
        private System.ComponentModel.BackgroundWorker backgroundWorkerExtract;
        private System.Windows.Forms.Button buttonChangeImage;
        private System.Windows.Forms.ColumnHeader colFileName;
        private System.Windows.Forms.OpenFileDialog openFileDialogImage;
        private System.Windows.Forms.ToolStripMenuItem selectNoneToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem clearToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
    }
}

