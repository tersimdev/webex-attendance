namespace WebExAttendance_Form
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.btnSelectImageFile = new System.Windows.Forms.Button();
            this.btnShowScreenshotWindow = new System.Windows.Forms.Button();
            this.picturePreview = new System.Windows.Forms.PictureBox();
            this.pictureCaption = new System.Windows.Forms.Label();
            this.btnSelectImageClipboard = new System.Windows.Forms.Button();
            this.btnSelectImageScreen = new System.Windows.Forms.Button();
            this.clipboardNoImgLabel = new System.Windows.Forms.Label();
            this.btnHideScreenshotWindow = new System.Windows.Forms.Button();
            this.btnDetect = new System.Windows.Forms.Button();
            this.btnReset = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.picturePreview)).BeginInit();
            this.SuspendLayout();
            // 
            // btnSelectImageFile
            // 
            this.btnSelectImageFile.BackColor = System.Drawing.SystemColors.Control;
            this.btnSelectImageFile.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSelectImageFile.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnSelectImageFile.Location = new System.Drawing.Point(12, 12);
            this.btnSelectImageFile.Name = "btnSelectImageFile";
            this.btnSelectImageFile.Size = new System.Drawing.Size(198, 40);
            this.btnSelectImageFile.TabIndex = 0;
            this.btnSelectImageFile.Text = "Select Image from File";
            this.btnSelectImageFile.UseVisualStyleBackColor = false;
            this.btnSelectImageFile.Click += new System.EventHandler(this.btnSelectImageFile_Click);
            // 
            // btnShowScreenshotWindow
            // 
            this.btnShowScreenshotWindow.BackColor = System.Drawing.SystemColors.Control;
            this.btnShowScreenshotWindow.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnShowScreenshotWindow.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnShowScreenshotWindow.Location = new System.Drawing.Point(12, 102);
            this.btnShowScreenshotWindow.Name = "btnShowScreenshotWindow";
            this.btnShowScreenshotWindow.Size = new System.Drawing.Size(198, 40);
            this.btnShowScreenshotWindow.TabIndex = 1;
            this.btnShowScreenshotWindow.Text = "Select Image from Screen";
            this.btnShowScreenshotWindow.UseVisualStyleBackColor = false;
            this.btnShowScreenshotWindow.Click += new System.EventHandler(this.btnShowScreenshotWindow_Click);
            // 
            // picturePreview
            // 
            this.picturePreview.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.picturePreview.Location = new System.Drawing.Point(12, 151);
            this.picturePreview.Name = "picturePreview";
            this.picturePreview.Size = new System.Drawing.Size(560, 545);
            this.picturePreview.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picturePreview.TabIndex = 2;
            this.picturePreview.TabStop = false;
            // 
            // pictureCaption
            // 
            this.pictureCaption.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pictureCaption.Location = new System.Drawing.Point(12, 699);
            this.pictureCaption.Name = "pictureCaption";
            this.pictureCaption.Size = new System.Drawing.Size(560, 53);
            this.pictureCaption.TabIndex = 4;
            this.pictureCaption.Text = "CAPTION HERE";
            // 
            // btnSelectImageClipboard
            // 
            this.btnSelectImageClipboard.BackColor = System.Drawing.SystemColors.Control;
            this.btnSelectImageClipboard.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSelectImageClipboard.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnSelectImageClipboard.Location = new System.Drawing.Point(12, 57);
            this.btnSelectImageClipboard.Name = "btnSelectImageClipboard";
            this.btnSelectImageClipboard.Size = new System.Drawing.Size(198, 40);
            this.btnSelectImageClipboard.TabIndex = 5;
            this.btnSelectImageClipboard.Text = "Select Image from Clipboard";
            this.btnSelectImageClipboard.UseVisualStyleBackColor = false;
            this.btnSelectImageClipboard.Click += new System.EventHandler(this.btnSelectImageClipboard_Click);
            // 
            // btnSelectImageScreen
            // 
            this.btnSelectImageScreen.BackColor = System.Drawing.SystemColors.Control;
            this.btnSelectImageScreen.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSelectImageScreen.ForeColor = System.Drawing.Color.Green;
            this.btnSelectImageScreen.Location = new System.Drawing.Point(219, 102);
            this.btnSelectImageScreen.Name = "btnSelectImageScreen";
            this.btnSelectImageScreen.Size = new System.Drawing.Size(40, 40);
            this.btnSelectImageScreen.TabIndex = 6;
            this.btnSelectImageScreen.Text = "✔️";
            this.btnSelectImageScreen.UseVisualStyleBackColor = false;
            this.btnSelectImageScreen.Click += new System.EventHandler(this.btnSelectImageScreen_Click);
            // 
            // clipboardNoImgLabel
            // 
            this.clipboardNoImgLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.clipboardNoImgLabel.Location = new System.Drawing.Point(216, 63);
            this.clipboardNoImgLabel.Name = "clipboardNoImgLabel";
            this.clipboardNoImgLabel.Size = new System.Drawing.Size(231, 30);
            this.clipboardNoImgLabel.TabIndex = 7;
            this.clipboardNoImgLabel.Text = "No image found in clipboard!";
            this.clipboardNoImgLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.clipboardNoImgLabel.Visible = false;
            // 
            // btnHideScreenshotWindow
            // 
            this.btnHideScreenshotWindow.BackColor = System.Drawing.SystemColors.Control;
            this.btnHideScreenshotWindow.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnHideScreenshotWindow.ForeColor = System.Drawing.Color.DarkRed;
            this.btnHideScreenshotWindow.Location = new System.Drawing.Point(265, 102);
            this.btnHideScreenshotWindow.Name = "btnHideScreenshotWindow";
            this.btnHideScreenshotWindow.Size = new System.Drawing.Size(40, 40);
            this.btnHideScreenshotWindow.TabIndex = 8;
            this.btnHideScreenshotWindow.Text = "❌";
            this.btnHideScreenshotWindow.UseVisualStyleBackColor = false;
            this.btnHideScreenshotWindow.Click += new System.EventHandler(this.btnHideScreenshotWindow_Click);
            // 
            // btnDetect
            // 
            this.btnDetect.BackColor = System.Drawing.SystemColors.Control;
            this.btnDetect.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDetect.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnDetect.Location = new System.Drawing.Point(458, 12);
            this.btnDetect.Name = "btnDetect";
            this.btnDetect.Size = new System.Drawing.Size(114, 40);
            this.btnDetect.TabIndex = 9;
            this.btnDetect.Text = "DETECT";
            this.btnDetect.UseVisualStyleBackColor = false;
            this.btnDetect.Click += new System.EventHandler(this.btnDetect_Click);
            // 
            // btnReset
            // 
            this.btnReset.BackColor = System.Drawing.SystemColors.Control;
            this.btnReset.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnReset.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnReset.Location = new System.Drawing.Point(482, 58);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(90, 40);
            this.btnReset.TabIndex = 10;
            this.btnReset.Text = "RESET";
            this.btnReset.UseVisualStyleBackColor = false;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ClientSize = new System.Drawing.Size(584, 761);
            this.Controls.Add(this.btnReset);
            this.Controls.Add(this.btnDetect);
            this.Controls.Add(this.btnHideScreenshotWindow);
            this.Controls.Add(this.clipboardNoImgLabel);
            this.Controls.Add(this.btnSelectImageScreen);
            this.Controls.Add(this.btnSelectImageClipboard);
            this.Controls.Add(this.pictureCaption);
            this.Controls.Add(this.picturePreview);
            this.Controls.Add(this.btnShowScreenshotWindow);
            this.Controls.Add(this.btnSelectImageFile);
            this.ForeColor = System.Drawing.Color.White;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.Text = "WebEx Attendance - Made by Terence";
            this.Load += new System.EventHandler(this.MainForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.picturePreview)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnSelectImageFile;
        private System.Windows.Forms.Button btnShowScreenshotWindow;
        private System.Windows.Forms.PictureBox picturePreview;
        private System.Windows.Forms.Label pictureCaption;
        private System.Windows.Forms.Button btnSelectImageClipboard;
        private System.Windows.Forms.Button btnSelectImageScreen;
        private System.Windows.Forms.Label clipboardNoImgLabel;
        private System.Windows.Forms.Button btnHideScreenshotWindow;
        private System.Windows.Forms.Button btnDetect;
        private System.Windows.Forms.Button btnReset;
    }
}

