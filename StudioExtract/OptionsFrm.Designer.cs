namespace StudioExtract
{
    partial class OptionsFrm
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
            this.grpCard = new System.Windows.Forms.GroupBox();
            this.chkScene = new System.Windows.Forms.CheckBox();
            this.btnBkgColorChange = new System.Windows.Forms.Button();
            this.chkFrame = new System.Windows.Forms.CheckBox();
            this.chkBkgImage = new System.Windows.Forms.CheckBox();
            this.boxBkgColor = new System.Windows.Forms.PictureBox();
            this.lblBkgColor = new System.Windows.Forms.Label();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.grpCard.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.boxBkgColor)).BeginInit();
            this.SuspendLayout();
            // 
            // grpCard
            // 
            this.grpCard.Controls.Add(this.chkScene);
            this.grpCard.Controls.Add(this.btnBkgColorChange);
            this.grpCard.Controls.Add(this.chkFrame);
            this.grpCard.Controls.Add(this.chkBkgImage);
            this.grpCard.Controls.Add(this.boxBkgColor);
            this.grpCard.Controls.Add(this.lblBkgColor);
            this.grpCard.Location = new System.Drawing.Point(12, 12);
            this.grpCard.Name = "grpCard";
            this.grpCard.Size = new System.Drawing.Size(460, 178);
            this.grpCard.TabIndex = 0;
            this.grpCard.TabStop = false;
            this.grpCard.Text = "Card image";
            // 
            // chkScene
            // 
            this.chkScene.AutoSize = true;
            this.chkScene.Location = new System.Drawing.Point(35, 142);
            this.chkScene.Name = "chkScene";
            this.chkScene.Size = new System.Drawing.Size(118, 17);
            this.chkScene.TabIndex = 4;
            this.chkScene.Text = "Draw scene picture";
            this.chkScene.UseVisualStyleBackColor = true;
            // 
            // btnBkgColorChange
            // 
            this.btnBkgColorChange.Location = new System.Drawing.Point(187, 28);
            this.btnBkgColorChange.Name = "btnBkgColorChange";
            this.btnBkgColorChange.Size = new System.Drawing.Size(75, 23);
            this.btnBkgColorChange.TabIndex = 1;
            this.btnBkgColorChange.Text = "&Change";
            this.btnBkgColorChange.UseVisualStyleBackColor = true;
            this.btnBkgColorChange.Click += new System.EventHandler(this.btnBkgColorChange_Click);
            // 
            // chkFrame
            // 
            this.chkFrame.AutoSize = true;
            this.chkFrame.Location = new System.Drawing.Point(35, 109);
            this.chkFrame.Name = "chkFrame";
            this.chkFrame.Size = new System.Drawing.Size(80, 17);
            this.chkFrame.TabIndex = 3;
            this.chkFrame.Text = "Draw frame";
            this.chkFrame.UseVisualStyleBackColor = true;
            // 
            // chkBkgImage
            // 
            this.chkBkgImage.AutoSize = true;
            this.chkBkgImage.Location = new System.Drawing.Point(35, 76);
            this.chkBkgImage.Name = "chkBkgImage";
            this.chkBkgImage.Size = new System.Drawing.Size(142, 17);
            this.chkBkgImage.TabIndex = 2;
            this.chkBkgImage.Text = "Draw background image";
            this.chkBkgImage.UseVisualStyleBackColor = true;
            // 
            // boxBkgColor
            // 
            this.boxBkgColor.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(126)))), ((int)(((byte)(116)))), ((int)(((byte)(146)))));
            this.boxBkgColor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.boxBkgColor.Location = new System.Drawing.Point(139, 19);
            this.boxBkgColor.Name = "boxBkgColor";
            this.boxBkgColor.Size = new System.Drawing.Size(42, 42);
            this.boxBkgColor.TabIndex = 1;
            this.boxBkgColor.TabStop = false;
            // 
            // lblBkgColor
            // 
            this.lblBkgColor.AutoSize = true;
            this.lblBkgColor.Location = new System.Drawing.Point(32, 33);
            this.lblBkgColor.Name = "lblBkgColor";
            this.lblBkgColor.Size = new System.Drawing.Size(91, 13);
            this.lblBkgColor.TabIndex = 0;
            this.lblBkgColor.Text = "&Background color";
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.Location = new System.Drawing.Point(284, 211);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 2;
            this.btnSave.Text = "&Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(382, 211);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "&Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // OptionsFrm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(484, 250);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.grpCard);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "OptionsFrm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Options";
            this.grpCard.ResumeLayout(false);
            this.grpCard.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.boxBkgColor)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox grpCard;
        private System.Windows.Forms.PictureBox boxBkgColor;
        private System.Windows.Forms.Label lblBkgColor;
        private System.Windows.Forms.CheckBox chkFrame;
        private System.Windows.Forms.CheckBox chkBkgImage;
        private System.Windows.Forms.Button btnBkgColorChange;
        private System.Windows.Forms.CheckBox chkScene;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;
    }
}