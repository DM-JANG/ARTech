namespace ProtocolTest
{
    partial class DBSetUp
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
            this.DbId = new System.Windows.Forms.Label();
            this.DbPass = new System.Windows.Forms.Label();
            this.ID = new System.Windows.Forms.TextBox();
            this.Pass = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.Server = new System.Windows.Forms.TextBox();
            this.Ok = new System.Windows.Forms.Button();
            this.Cancel = new System.Windows.Forms.Button();
            this.DBName = new System.Windows.Forms.TextBox();
            this.DBNames = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // DbId
            // 
            this.DbId.AutoSize = true;
            this.DbId.Location = new System.Drawing.Point(56, 44);
            this.DbId.Name = "DbId";
            this.DbId.Size = new System.Drawing.Size(47, 12);
            this.DbId.TabIndex = 0;
            this.DbId.Text = "DB Id : ";
            // 
            // DbPass
            // 
            this.DbPass.AutoSize = true;
            this.DbPass.Location = new System.Drawing.Point(9, 70);
            this.DbPass.Name = "DbPass";
            this.DbPass.Size = new System.Drawing.Size(94, 12);
            this.DbPass.TabIndex = 1;
            this.DbPass.Text = "DB Password : ";
            // 
            // ID
            // 
            this.ID.Location = new System.Drawing.Point(109, 39);
            this.ID.Name = "ID";
            this.ID.Size = new System.Drawing.Size(180, 21);
            this.ID.TabIndex = 2;
            // 
            // Pass
            // 
            this.Pass.Location = new System.Drawing.Point(109, 67);
            this.Pass.Name = "Pass";
            this.Pass.PasswordChar = '*';
            this.Pass.Size = new System.Drawing.Size(180, 21);
            this.Pass.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(91, 12);
            this.label1.TabIndex = 4;
            this.label1.Text = "Server Name : ";
            // 
            // Server
            // 
            this.Server.Location = new System.Drawing.Point(109, 12);
            this.Server.Name = "Server";
            this.Server.Size = new System.Drawing.Size(180, 21);
            this.Server.TabIndex = 5;
            // 
            // Ok
            // 
            this.Ok.Location = new System.Drawing.Point(135, 126);
            this.Ok.Name = "Ok";
            this.Ok.Size = new System.Drawing.Size(75, 23);
            this.Ok.TabIndex = 6;
            this.Ok.Text = "확인";
            this.Ok.UseVisualStyleBackColor = true;
            this.Ok.Click += new System.EventHandler(this.Ok_Click);
            // 
            // Cancel
            // 
            this.Cancel.Location = new System.Drawing.Point(215, 126);
            this.Cancel.Name = "Cancel";
            this.Cancel.Size = new System.Drawing.Size(75, 23);
            this.Cancel.TabIndex = 7;
            this.Cancel.Text = "취소";
            this.Cancel.UseVisualStyleBackColor = true;
            this.Cancel.Click += new System.EventHandler(this.Cancel_Click);
            // 
            // DBName
            // 
            this.DBName.Location = new System.Drawing.Point(109, 94);
            this.DBName.Name = "DBName";
            this.DBName.Size = new System.Drawing.Size(180, 21);
            this.DBName.TabIndex = 9;
            // 
            // DBNames
            // 
            this.DBNames.AutoSize = true;
            this.DBNames.Location = new System.Drawing.Point(32, 97);
            this.DBNames.Name = "DBNames";
            this.DBNames.Size = new System.Drawing.Size(71, 12);
            this.DBNames.TabIndex = 8;
            this.DBNames.Text = "DB Name : ";
            // 
            // DBSetUp
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(302, 161);
            this.Controls.Add(this.DBName);
            this.Controls.Add(this.DBNames);
            this.Controls.Add(this.Cancel);
            this.Controls.Add(this.Ok);
            this.Controls.Add(this.Server);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.Pass);
            this.Controls.Add(this.ID);
            this.Controls.Add(this.DbPass);
            this.Controls.Add(this.DbId);
            this.Name = "DBSetUp";
            this.Text = "DBSetUp";
            this.Load += new System.EventHandler(this.DBSetUp_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label DbId;
        private System.Windows.Forms.Label DbPass;
        private System.Windows.Forms.TextBox ID;
        private System.Windows.Forms.TextBox Pass;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox Server;
        private System.Windows.Forms.Button Ok;
        private System.Windows.Forms.Button Cancel;
        private System.Windows.Forms.TextBox DBName;
        private System.Windows.Forms.Label DBNames;
    }
}