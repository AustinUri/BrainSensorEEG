namespace Screen_Capture
{
    partial class frmPrintScreen
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.butCapture = new System.Windows.Forms.Button();
            this.picScreen = new System.Windows.Forms.PictureBox();
            this.txtR = new System.Windows.Forms.TextBox();
            this.txtG = new System.Windows.Forms.TextBox();
            this.txtB = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txtX = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtY = new System.Windows.Forms.TextBox();
            this.butCleanMemory = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.picScreen)).BeginInit();
            this.SuspendLayout();
            // 
            // butCapture
            // 
            this.butCapture.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.butCapture.Location = new System.Drawing.Point(200, 441);
            this.butCapture.Name = "butCapture";
            this.butCapture.Size = new System.Drawing.Size(221, 54);
            this.butCapture.TabIndex = 0;
            this.butCapture.Text = "לכידת חלק מהמסך";
            this.butCapture.UseVisualStyleBackColor = false;
            this.butCapture.Click += new System.EventHandler(this.butCapture_Click);
            // 
            // picScreen
            // 
            this.picScreen.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picScreen.Location = new System.Drawing.Point(45, 97);
            this.picScreen.Name = "picScreen";
            this.picScreen.Size = new System.Drawing.Size(376, 338);
            this.picScreen.TabIndex = 1;
            this.picScreen.TabStop = false;
            this.picScreen.MouseDown += new System.Windows.Forms.MouseEventHandler(this.picScreen_MouseDown);
            // 
            // txtR
            // 
            this.txtR.Location = new System.Drawing.Point(56, 54);
            this.txtR.Name = "txtR";
            this.txtR.Size = new System.Drawing.Size(88, 27);
            this.txtR.TabIndex = 2;
            // 
            // txtG
            // 
            this.txtG.Location = new System.Drawing.Point(181, 54);
            this.txtG.Name = "txtG";
            this.txtG.Size = new System.Drawing.Size(88, 27);
            this.txtG.TabIndex = 3;
            // 
            // txtB
            // 
            this.txtB.Location = new System.Drawing.Point(303, 54);
            this.txtB.Name = "txtB";
            this.txtB.Size = new System.Drawing.Size(88, 27);
            this.txtB.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(32, 57);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(18, 20);
            this.label1.TabIndex = 5;
            this.label1.Text = "R";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(157, 57);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(19, 20);
            this.label2.TabIndex = 6;
            this.label2.Text = "G";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(279, 57);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(18, 20);
            this.label3.TabIndex = 7;
            this.label3.Text = "B";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(32, 15);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(18, 20);
            this.label4.TabIndex = 9;
            this.label4.Text = "X";
            // 
            // txtX
            // 
            this.txtX.Location = new System.Drawing.Point(56, 12);
            this.txtX.Name = "txtX";
            this.txtX.Size = new System.Drawing.Size(58, 27);
            this.txtX.TabIndex = 8;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(126, 15);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(17, 20);
            this.label5.TabIndex = 11;
            this.label5.Text = "Y";
            // 
            // txtY
            // 
            this.txtY.Location = new System.Drawing.Point(157, 12);
            this.txtY.Name = "txtY";
            this.txtY.Size = new System.Drawing.Size(70, 27);
            this.txtY.TabIndex = 10;
            // 
            // butCleanMemory
            // 
            this.butCleanMemory.BackColor = System.Drawing.Color.Red;
            this.butCleanMemory.Location = new System.Drawing.Point(45, 441);
            this.butCleanMemory.Name = "butCleanMemory";
            this.butCleanMemory.Size = new System.Drawing.Size(149, 54);
            this.butCleanMemory.TabIndex = 12;
            this.butCleanMemory.Text = "ניקוי הזיכרון";
            this.butCleanMemory.UseVisualStyleBackColor = false;
            // 
            // frmPrintScreen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(460, 507);
            this.Controls.Add(this.butCleanMemory);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txtY);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtX);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtB);
            this.Controls.Add(this.txtG);
            this.Controls.Add(this.txtR);
            this.Controls.Add(this.picScreen);
            this.Controls.Add(this.butCapture);
            this.Name = "frmPrintScreen";
            this.Text = "לכידת חלק מהמסך";
            this.Load += new System.EventHandler(this.frmPrintScreen_Load);
            ((System.ComponentModel.ISupportInitialize)(this.picScreen)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Button butCapture;
        private PictureBox picScreen;
        private TextBox txtR;
        private TextBox txtG;
        private TextBox txtB;
        private Label label1;
        private Label label2;
        private Label label3;
        private Label label4;
        private TextBox txtX;
        private Label label5;
        private TextBox txtY;
        private Button butCleanMemory;
    }
}