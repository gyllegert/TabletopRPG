namespace testcam
{
    partial class Battlemat
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
            this.bluePieceBox = new System.Windows.Forms.PictureBox();
            this.greenPieceBox = new System.Windows.Forms.PictureBox();
            this.tealPieceBox = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.bluePieceBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.greenPieceBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tealPieceBox)).BeginInit();
            this.SuspendLayout();
            // 
            // bluePieceBox
            // 
            this.bluePieceBox.Image = global::testcam.Properties.Resources.BlueF;
            this.bluePieceBox.Location = new System.Drawing.Point(0, 0);
            this.bluePieceBox.Name = "bluePieceBox";
            this.bluePieceBox.Size = new System.Drawing.Size(128, 128);
            this.bluePieceBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.bluePieceBox.TabIndex = 0;
            this.bluePieceBox.TabStop = false;
            this.bluePieceBox.Click += new System.EventHandler(this.bluePieceBox_Click);
            // 
            // greenPieceBox
            // 
            this.greenPieceBox.Image = global::testcam.Properties.Resources.GreenF;
            this.greenPieceBox.Location = new System.Drawing.Point(0, 25);
            this.greenPieceBox.Name = "greenPieceBox";
            this.greenPieceBox.Size = new System.Drawing.Size(128, 128);
            this.greenPieceBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.greenPieceBox.TabIndex = 1;
            this.greenPieceBox.TabStop = false;
            // 
            // tealPieceBox
            // 
            this.tealPieceBox.Image = global::testcam.Properties.Resources.TealF;
            this.tealPieceBox.Location = new System.Drawing.Point(0, 12);
            this.tealPieceBox.Name = "tealPieceBox";
            this.tealPieceBox.Size = new System.Drawing.Size(128, 128);
            this.tealPieceBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.tealPieceBox.TabIndex = 2;
            this.tealPieceBox.TabStop = false;
            // 
            // Battlemat
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::testcam.Properties.Resources.GameBoardTemplateWgrid;
            this.ClientSize = new System.Drawing.Size(1285, 1053);
            this.Controls.Add(this.tealPieceBox);
            this.Controls.Add(this.greenPieceBox);
            this.Controls.Add(this.bluePieceBox);
            this.Name = "Battlemat";
            this.Text = "Battlemat";
            this.Load += new System.EventHandler(this.Battlemat_Load);
            ((System.ComponentModel.ISupportInitialize)(this.bluePieceBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.greenPieceBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tealPieceBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox bluePieceBox;
        private System.Windows.Forms.PictureBox greenPieceBox;
        private System.Windows.Forms.PictureBox tealPieceBox;
    }
}