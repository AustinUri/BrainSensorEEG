using ServerF;
using static ServerF.Server;

namespace WinFormsApp1
{
    partial class Form1 : Form
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
            label_Battery = new Label();
            label_Welcome = new Label();
            label_Height = new Label();
            lable_Drone = new Label();
            label_Dbattery = new Label();
            label_Dheight = new Label();
            label_Dstatus = new Label();
            SuspendLayout();
            // 
            // label_Battery
            // 
            label_Battery.Anchor = AnchorStyles.None;
            label_Battery.AutoSize = true;
            label_Battery.Location = new Point(12, 106);
            label_Battery.Name = "label_Battery";
            label_Battery.Size = new Size(217, 25);
            label_Battery.TabIndex = 0;
            label_Battery.Text = "Drone Battery Precentage:";
            label_Battery.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label_Welcome
            // 
            label_Welcome.Anchor = AnchorStyles.None;
            label_Welcome.AutoSize = true;
            label_Welcome.Location = new Point(339, 9);
            label_Welcome.Name = "label_Welcome";
            label_Welcome.Size = new Size(95, 25);
            label_Welcome.TabIndex = 2;
            label_Welcome.Text = "Welcome!!";
            // 
            // label_Height
            // 
            label_Height.Anchor = AnchorStyles.None;
            label_Height.AutoSize = true;
            label_Height.Location = new Point(12, 183);
            label_Height.Name = "label_Height";
            label_Height.Size = new Size(157, 25);
            label_Height.TabIndex = 3;
            label_Height.Text = "Drone Height(cm):";
            // 
            // lable_Drone
            // 
            lable_Drone.Anchor = AnchorStyles.None;
            lable_Drone.AutoSize = true;
            lable_Drone.Location = new Point(12, 292);
            lable_Drone.Name = "lable_Drone";
            lable_Drone.Size = new Size(118, 25);
            lable_Drone.TabIndex = 5;
            lable_Drone.Text = "Drone Status:";
            // 
            // label_Dbattery
            // 
            label_Dbattery.AutoSize = true;
            label_Dbattery.Location = new Point(451, 103);
            label_Dbattery.Name = "label_Dbattery";
            label_Dbattery.Size = new Size(125, 25);
            label_Dbattery.TabIndex = 6;
            label_Dbattery.Text = "label_Dbattery";
            // 
            // label_Dheight
            // 
            label_Dheight.AutoSize = true;
            label_Dheight.Location = new Point(456, 184);
            label_Dheight.Name = "label_Dheight";
            label_Dheight.Size = new Size(119, 25);
            label_Dheight.TabIndex = 7;
            label_Dheight.Text = "label_Dheight";
            // 
            // label_Dstatus
            // 
            label_Dstatus.AutoSize = true;
            label_Dstatus.Location = new Point(451, 292);
            label_Dstatus.Name = "label_Dstatus";
            label_Dstatus.Size = new Size(116, 25);
            label_Dstatus.TabIndex = 9;
            label_Dstatus.Text = "label_Dstatus";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(label_Dstatus);
            Controls.Add(label_Dheight);
            Controls.Add(label_Dbattery);
            Controls.Add(lable_Drone);
            Controls.Add(label_Height);
            Controls.Add(label_Welcome);
            Controls.Add(label_Battery);
            Name = "Form1";
            Text = "GUI";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label_Battery;
        private Label label_Welcome;
        private Label label_Height;
        private Label lable_Drone;
        private Label label_Dbattery;
        private Label label_Dheight;
        private Label label_Dstatus;



        public void ShowAlert(string message, string title)
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            label_Dstatus.Invoke((MethodInvoker)(() => label_Dstatus.Text = "Status: Disconnected"));
        }

    }
}
