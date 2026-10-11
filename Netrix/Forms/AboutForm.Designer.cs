namespace Netrix.Forms
{
    partial class AboutForm
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
            this.RepoLabel = new System.Windows.Forms.LinkLabel();
            this.NetrixPictureBox = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.NetrixPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // RepoLabel
            // 
            this.RepoLabel.AutoSize = true;
            this.RepoLabel.Location = new System.Drawing.Point(94, 215);
            this.RepoLabel.Name = "RepoLabel";
            this.RepoLabel.Size = new System.Drawing.Size(150, 17);
            this.RepoLabel.TabIndex = 5;
            this.RepoLabel.TabStop = true;
            this.RepoLabel.Text = "github.com/psnwd/netrix";
            this.RepoLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.RepoLabel_LinkClicked);
            // 
            // NetrixPictureBox
            // 
            this.NetrixPictureBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this.NetrixPictureBox.Image = global::Netrix.Properties.Resources.Netrix;
            this.NetrixPictureBox.Location = new System.Drawing.Point(72, 12);
            this.NetrixPictureBox.Name = "NetrixPictureBox";
            this.NetrixPictureBox.Size = new System.Drawing.Size(200, 200);
            this.NetrixPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.NetrixPictureBox.TabIndex = 0;
            this.NetrixPictureBox.TabStop = false;
            this.NetrixPictureBox.Click += new System.EventHandler(this.NetrixPictureBox_Click);
            // 
            // AboutForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(338, 244);
            this.Controls.Add(this.RepoLabel);
            this.Controls.Add(this.NetrixPictureBox);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.Name = "AboutForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "About";
            this.Load += new System.EventHandler(this.AboutForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.NetrixPictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox NetrixPictureBox;
        private System.Windows.Forms.LinkLabel RepoLabel;
    }
}