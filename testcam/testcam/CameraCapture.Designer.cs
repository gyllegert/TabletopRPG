﻿namespace testcam
{
    partial class CameraCapture
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
            this.components = new System.ComponentModel.Container();
            this.captureBoxOrg = new Emgu.CV.UI.ImageBox();
            this.btnStart = new System.Windows.Forms.Button();
            this.captureBoxBlue = new Emgu.CV.UI.ImageBox();
            this.captureBoxMagenta = new Emgu.CV.UI.ImageBox();
            this.captureBoxGreen = new Emgu.CV.UI.ImageBox();
            this.captureBoxTeal = new Emgu.CV.UI.ImageBox();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            ((System.ComponentModel.ISupportInitialize)(this.captureBoxOrg)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.captureBoxBlue)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.captureBoxMagenta)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.captureBoxGreen)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.captureBoxTeal)).BeginInit();
            this.SuspendLayout();
            // 
            // captureBoxOrg
            // 
            this.captureBoxOrg.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.captureBoxOrg.Location = new System.Drawing.Point(12, 12);
            this.captureBoxOrg.Name = "captureBoxOrg";
            this.captureBoxOrg.Size = new System.Drawing.Size(640, 480);
            this.captureBoxOrg.TabIndex = 2;
            this.captureBoxOrg.TabStop = false;
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(1333, 23);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(75, 23);
            this.btnStart.TabIndex = 3;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // captureBoxBlue
            // 
            this.captureBoxBlue.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.captureBoxBlue.Location = new System.Drawing.Point(658, 12);
            this.captureBoxBlue.Name = "captureBoxBlue";
            this.captureBoxBlue.Size = new System.Drawing.Size(640, 480);
            this.captureBoxBlue.TabIndex = 4;
            this.captureBoxBlue.TabStop = false;
            // 
            // captureBoxMagenta
            // 
            this.captureBoxMagenta.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.captureBoxMagenta.Location = new System.Drawing.Point(542, 514);
            this.captureBoxMagenta.Name = "captureBoxMagenta";
            this.captureBoxMagenta.Size = new System.Drawing.Size(512, 384);
            this.captureBoxMagenta.TabIndex = 5;
            this.captureBoxMagenta.TabStop = false;
            // 
            // captureBoxGreen
            // 
            this.captureBoxGreen.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.captureBoxGreen.Location = new System.Drawing.Point(24, 514);
            this.captureBoxGreen.Name = "captureBoxGreen";
            this.captureBoxGreen.Size = new System.Drawing.Size(512, 384);
            this.captureBoxGreen.TabIndex = 6;
            this.captureBoxGreen.TabStop = false;
            // 
            // captureBoxTeal
            // 
            this.captureBoxTeal.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.captureBoxTeal.Location = new System.Drawing.Point(1072, 514);
            this.captureBoxTeal.Name = "captureBoxTeal";
            this.captureBoxTeal.Size = new System.Drawing.Size(512, 384);
            this.captureBoxTeal.TabIndex = 7;
            this.captureBoxTeal.TabStop = false;
            // 
            // richTextBox1
            // 
            this.richTextBox1.Location = new System.Drawing.Point(1322, 87);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(100, 96);
            this.richTextBox1.TabIndex = 8;
            this.richTextBox1.Text = "";
            // 
            // CameraCapture
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1584, 985);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.captureBoxTeal);
            this.Controls.Add(this.captureBoxGreen);
            this.Controls.Add(this.captureBoxMagenta);
            this.Controls.Add(this.captureBoxBlue);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.captureBoxOrg);
            this.Name = "CameraCapture";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.CameraCapture_Load);
            ((System.ComponentModel.ISupportInitialize)(this.captureBoxOrg)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.captureBoxBlue)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.captureBoxMagenta)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.captureBoxGreen)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.captureBoxTeal)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Emgu.CV.UI.ImageBox captureBoxOrg;
        private System.Windows.Forms.Button btnStart;
        private Emgu.CV.UI.ImageBox captureBoxBlue;
        private Emgu.CV.UI.ImageBox captureBoxMagenta;
        private Emgu.CV.UI.ImageBox captureBoxGreen;
        private Emgu.CV.UI.ImageBox captureBoxTeal;
        private System.Windows.Forms.RichTextBox richTextBox1;
    }
}

