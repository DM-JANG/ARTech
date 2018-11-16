namespace DAS_GUI
{
    partial class PointReview
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
            this.PointList = new System.Windows.Forms.ListBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.ZoneDataList = new System.Windows.Forms.DataGridView();
            this.Index = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Tag = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Direction = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Space = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Distince = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Date = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Print = new System.Windows.Forms.Button();
            this.StartDate = new System.Windows.Forms.DateTimePicker();
            this.EndDate = new System.Windows.Forms.DateTimePicker();
            this.label1 = new System.Windows.Forms.Label();
            this.Search = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ZoneDataList)).BeginInit();
            this.SuspendLayout();
            // 
            // PointList
            // 
            this.PointList.FormattingEnabled = true;
            this.PointList.ItemHeight = 12;
            this.PointList.Items.AddRange(new object[] {
            "1.지정천교(입)",
            "2.지정천교(출)",
            "3.소막골 터널(입)",
            "4.소막골 터널(출)",
            "5.선로상태",
            "6.보통고가(입)",
            "7.보통고가(출)",
            "8.서원주 변전소",
            "9.광터고가(입)",
            "10.광터고가(출)",
            "11.지장물1(입)",
            "12.지장물1(출)",
            "13.지장물2(입)",
            "14.지장물2(출)",
            "15.만종터널(입)",
            "16.만종터널(출)",
            "17.만종역(입)",
            "18.만종역(출)",
            "19.만종천1교(입)",
            "20.만종천교(출)",
            "21.호저터널(입)",
            "22.호저터널(출)",
            "23.가현교(입)",
            "24.가현교(출)",
            "25.점실교(입)",
            "26.점실교(출)",
            "27.원주천교(입)",
            "28.원주천교(출)"});
            this.PointList.Location = new System.Drawing.Point(18, 20);
            this.PointList.Name = "PointList";
            this.PointList.Size = new System.Drawing.Size(151, 364);
            this.PointList.TabIndex = 0;
            this.PointList.SelectedIndexChanged += new System.EventHandler(this.PointList_SelectedIndexChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.PointList);
            this.groupBox1.Location = new System.Drawing.Point(12, 22);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(188, 401);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Zone List";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.ZoneDataList);
            this.groupBox2.Location = new System.Drawing.Point(223, 29);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(781, 393);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Event List";
            // 
            // ZoneDataList
            // 
            this.ZoneDataList.AllowUserToAddRows = false;
            this.ZoneDataList.AllowUserToDeleteRows = false;
            this.ZoneDataList.BackgroundColor = System.Drawing.SystemColors.ControlLight;
            this.ZoneDataList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.ZoneDataList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Index,
            this.Tag,
            this.Direction,
            this.Space,
            this.Distince,
            this.Date});
            this.ZoneDataList.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.ZoneDataList.Location = new System.Drawing.Point(15, 20);
            this.ZoneDataList.MultiSelect = false;
            this.ZoneDataList.Name = "ZoneDataList";
            this.ZoneDataList.ReadOnly = true;
            this.ZoneDataList.RowHeadersVisible = false;
            this.ZoneDataList.RowTemplate.Height = 23;
            this.ZoneDataList.Size = new System.Drawing.Size(725, 357);
            this.ZoneDataList.TabIndex = 0;
            // 
            // Index
            // 
            this.Index.HeaderText = "Index";
            this.Index.Name = "Index";
            this.Index.ReadOnly = true;
            // 
            // Tag
            // 
            this.Tag.HeaderText = "Tag";
            this.Tag.Name = "Tag";
            this.Tag.ReadOnly = true;
            // 
            // Direction
            // 
            this.Direction.HeaderText = "Direction";
            this.Direction.Name = "Direction";
            this.Direction.ReadOnly = true;
            // 
            // Space
            // 
            this.Space.HeaderText = "Space";
            this.Space.Name = "Space";
            this.Space.ReadOnly = true;
            // 
            // Distince
            // 
            this.Distince.HeaderText = "Distince";
            this.Distince.Name = "Distince";
            this.Distince.ReadOnly = true;
            // 
            // Date
            // 
            this.Date.HeaderText = "Date";
            this.Date.Name = "Date";
            this.Date.ReadOnly = true;
            this.Date.Width = 200;
            // 
            // Print
            // 
            this.Print.Location = new System.Drawing.Point(831, 433);
            this.Print.Name = "Print";
            this.Print.Size = new System.Drawing.Size(131, 26);
            this.Print.TabIndex = 3;
            this.Print.Text = "Print";
            this.Print.UseVisualStyleBackColor = true;
            this.Print.Click += new System.EventHandler(this.Print_Click);
            // 
            // StartDate
            // 
            this.StartDate.Location = new System.Drawing.Point(93, 434);
            this.StartDate.Name = "StartDate";
            this.StartDate.Size = new System.Drawing.Size(267, 21);
            this.StartDate.TabIndex = 4;
            // 
            // EndDate
            // 
            this.EndDate.Location = new System.Drawing.Point(413, 435);
            this.EndDate.Name = "EndDate";
            this.EndDate.Size = new System.Drawing.Size(267, 21);
            this.EndDate.TabIndex = 5;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(379, 438);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(14, 12);
            this.label1.TabIndex = 6;
            this.label1.Text = "~";
            // 
            // Search
            // 
            this.Search.Location = new System.Drawing.Point(700, 433);
            this.Search.Name = "Search";
            this.Search.Size = new System.Drawing.Size(110, 26);
            this.Search.TabIndex = 7;
            this.Search.Text = "Search";
            this.Search.UseVisualStyleBackColor = true;
            this.Search.Click += new System.EventHandler(this.Search_Click);
            // 
            // PointReview
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1027, 470);
            this.Controls.Add(this.Search);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.EndDate);
            this.Controls.Add(this.StartDate);
            this.Controls.Add(this.Print);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "PointReview";
            this.Text = "PointReview";
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ZoneDataList)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox PointList;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.DataGridView ZoneDataList;
        private System.Windows.Forms.DataGridViewTextBoxColumn Index;
        private System.Windows.Forms.DataGridViewTextBoxColumn Tag;
        private System.Windows.Forms.DataGridViewTextBoxColumn Direction;
        private System.Windows.Forms.DataGridViewTextBoxColumn Space;
        private System.Windows.Forms.DataGridViewTextBoxColumn Distince;
        private System.Windows.Forms.DataGridViewTextBoxColumn Date;
        private System.Windows.Forms.Button Print;
        private System.Windows.Forms.DateTimePicker StartDate;
        private System.Windows.Forms.DateTimePicker EndDate;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button Search;
    }
}