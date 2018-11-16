namespace DAS_GUI
{
    partial class Report
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
            this.Kindlb = new System.Windows.Forms.Label();
            this.Datelb = new System.Windows.Forms.Label();
            this.Datelb2 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.EndDate = new System.Windows.Forms.DateTimePicker();
            this.StartDate = new System.Windows.Forms.DateTimePicker();
            this.KindCombo = new System.Windows.Forms.ComboBox();
            this.Print = new System.Windows.Forms.Button();
            this.Cancle = new System.Windows.Forms.Button();
            this.directoryEntry1 = new System.DirectoryServices.DirectoryEntry();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // Kindlb
            // 
            this.Kindlb.AutoSize = true;
            this.Kindlb.Location = new System.Drawing.Point(7, 29);
            this.Kindlb.Name = "Kindlb";
            this.Kindlb.Size = new System.Drawing.Size(69, 12);
            this.Kindlb.TabIndex = 0;
            this.Kindlb.Text = "열차 종류 : ";
            // 
            // Datelb
            // 
            this.Datelb.AutoSize = true;
            this.Datelb.Location = new System.Drawing.Point(35, 61);
            this.Datelb.Name = "Datelb";
            this.Datelb.Size = new System.Drawing.Size(41, 12);
            this.Datelb.TabIndex = 1;
            this.Datelb.Text = "날짜 : ";
            // 
            // Datelb2
            // 
            this.Datelb2.AutoSize = true;
            this.Datelb2.Location = new System.Drawing.Point(273, 58);
            this.Datelb2.Name = "Datelb2";
            this.Datelb2.Size = new System.Drawing.Size(26, 12);
            this.Datelb2.TabIndex = 2;
            this.Datelb2.Text = " ~  ";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.EndDate);
            this.groupBox1.Controls.Add(this.StartDate);
            this.groupBox1.Controls.Add(this.KindCombo);
            this.groupBox1.Controls.Add(this.Datelb2);
            this.groupBox1.Controls.Add(this.Datelb);
            this.groupBox1.Controls.Add(this.Kindlb);
            this.groupBox1.Location = new System.Drawing.Point(13, 16);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(496, 98);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "출력 옵션";
            // 
            // EndDate
            // 
            this.EndDate.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            this.EndDate.Location = new System.Drawing.Point(305, 52);
            this.EndDate.Name = "EndDate";
            this.EndDate.Size = new System.Drawing.Size(176, 21);
            this.EndDate.TabIndex = 7;
            // 
            // StartDate
            // 
            this.StartDate.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            this.StartDate.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.StartDate.Location = new System.Drawing.Point(91, 52);
            this.StartDate.Name = "StartDate";
            this.StartDate.Size = new System.Drawing.Size(176, 21);
            this.StartDate.TabIndex = 6;
            // 
            // KindCombo
            // 
            this.KindCombo.FormattingEnabled = true;
            this.KindCombo.Items.AddRange(new object[] {
            "All",
            "KTX",
            "Others"});
            this.KindCombo.Location = new System.Drawing.Point(91, 21);
            this.KindCombo.Name = "KindCombo";
            this.KindCombo.Size = new System.Drawing.Size(176, 20);
            this.KindCombo.TabIndex = 3;
            // 
            // Print
            // 
            this.Print.Location = new System.Drawing.Point(344, 120);
            this.Print.Name = "Print";
            this.Print.Size = new System.Drawing.Size(75, 23);
            this.Print.TabIndex = 4;
            this.Print.Text = "출력";
            this.Print.UseVisualStyleBackColor = true;
            this.Print.Click += new System.EventHandler(this.Print_Click);
            // 
            // Cancle
            // 
            this.Cancle.Location = new System.Drawing.Point(434, 119);
            this.Cancle.Name = "Cancle";
            this.Cancle.Size = new System.Drawing.Size(75, 23);
            this.Cancle.TabIndex = 5;
            this.Cancle.Text = "취소";
            this.Cancle.UseVisualStyleBackColor = true;
            this.Cancle.Click += new System.EventHandler(this.Cancle_Click);
            // 
            // Report
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(522, 154);
            this.Controls.Add(this.Cancle);
            this.Controls.Add(this.Print);
            this.Controls.Add(this.groupBox1);
            this.Name = "Report";
            this.Text = "Report";
            this.Load += new System.EventHandler(this.Report_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label Kindlb;
        private System.Windows.Forms.Label Datelb;
        private System.Windows.Forms.Label Datelb2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.DateTimePicker EndDate;
        private System.Windows.Forms.DateTimePicker StartDate;
        private System.Windows.Forms.ComboBox KindCombo;
        private System.Windows.Forms.Button Print;
        private System.Windows.Forms.Button Cancle;
        private System.DirectoryServices.DirectoryEntry directoryEntry1;
    }
}