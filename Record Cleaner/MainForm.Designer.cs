namespace RecordCleaner
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
            if( disposing && ( components != null ) )
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
            this.buttonOpenDictionary = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.labelDictionary = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.labelDataFile = new System.Windows.Forms.Label();
            this.labelCleanedFile = new System.Windows.Forms.Label();
            this.buttonSelectDataFile = new System.Windows.Forms.Button();
            this.buttonClean = new System.Windows.Forms.Button();
            this.listViewStats = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.labelCases = new System.Windows.Forms.Label();
            this.labelRecordsRead = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.labelRecordsWritten = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonOpenDictionary
            // 
            this.buttonOpenDictionary.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOpenDictionary.Location = new System.Drawing.Point(503, 12);
            this.buttonOpenDictionary.Name = "buttonOpenDictionary";
            this.buttonOpenDictionary.Size = new System.Drawing.Size(116, 23);
            this.buttonOpenDictionary.TabIndex = 0;
            this.buttonOpenDictionary.Text = "Select Dictionary";
            this.buttonOpenDictionary.UseVisualStyleBackColor = true;
            this.buttonOpenDictionary.Click += new System.EventHandler(this.buttonOpenDictionary_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(57, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Dictionary:";
            // 
            // labelDictionary
            // 
            this.labelDictionary.AutoSize = true;
            this.labelDictionary.Location = new System.Drawing.Point(84, 17);
            this.labelDictionary.Name = "labelDictionary";
            this.labelDictionary.Size = new System.Drawing.Size(95, 13);
            this.labelDictionary.TabIndex = 2;
            this.labelDictionary.Text = "<select dictionary>";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 51);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(49, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Data file:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 85);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Cleaned file:";
            // 
            // labelDataFile
            // 
            this.labelDataFile.AutoSize = true;
            this.labelDataFile.Location = new System.Drawing.Point(84, 51);
            this.labelDataFile.Name = "labelDataFile";
            this.labelDataFile.Size = new System.Drawing.Size(87, 13);
            this.labelDataFile.TabIndex = 5;
            this.labelDataFile.Text = "<select data file>";
            // 
            // labelCleanedFile
            // 
            this.labelCleanedFile.AutoSize = true;
            this.labelCleanedFile.Location = new System.Drawing.Point(84, 85);
            this.labelCleanedFile.Name = "labelCleanedFile";
            this.labelCleanedFile.Size = new System.Drawing.Size(16, 13);
            this.labelCleanedFile.TabIndex = 6;
            this.labelCleanedFile.Text = "...";
            this.labelCleanedFile.Visible = false;
            // 
            // buttonSelectDataFile
            // 
            this.buttonSelectDataFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonSelectDataFile.Location = new System.Drawing.Point(503, 46);
            this.buttonSelectDataFile.Name = "buttonSelectDataFile";
            this.buttonSelectDataFile.Size = new System.Drawing.Size(116, 23);
            this.buttonSelectDataFile.TabIndex = 7;
            this.buttonSelectDataFile.Text = "Select Data File";
            this.buttonSelectDataFile.UseVisualStyleBackColor = true;
            this.buttonSelectDataFile.Click += new System.EventHandler(this.buttonSelectDataFile_Click);
            // 
            // buttonClean
            // 
            this.buttonClean.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonClean.Enabled = false;
            this.buttonClean.Location = new System.Drawing.Point(503, 80);
            this.buttonClean.Name = "buttonClean";
            this.buttonClean.Size = new System.Drawing.Size(116, 23);
            this.buttonClean.TabIndex = 8;
            this.buttonClean.Text = "Clean!";
            this.buttonClean.UseVisualStyleBackColor = true;
            this.buttonClean.Click += new System.EventHandler(this.buttonClean_Click);
            // 
            // listViewStats
            // 
            this.listViewStats.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listViewStats.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader5,
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4});
            this.listViewStats.Location = new System.Drawing.Point(18, 60);
            this.listViewStats.Name = "listViewStats";
            this.listViewStats.Size = new System.Drawing.Size(569, 309);
            this.listViewStats.TabIndex = 9;
            this.listViewStats.UseCompatibleStateImageBehavior = false;
            this.listViewStats.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Type";
            this.columnHeader1.Width = 100;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Read";
            this.columnHeader2.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnHeader2.Width = 75;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Inserted";
            this.columnHeader3.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnHeader3.Width = 75;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Deleted";
            this.columnHeader4.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnHeader4.Width = 75;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.labelRecordsWritten);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.labelRecordsRead);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.labelCases);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.listViewStats);
            this.groupBox1.Location = new System.Drawing.Point(15, 118);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(604, 381);
            this.groupBox1.TabIndex = 10;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Cleaning Statistics";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(17, 29);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(68, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "Cases Read:";
            // 
            // labelCases
            // 
            this.labelCases.AutoSize = true;
            this.labelCases.Location = new System.Drawing.Point(91, 29);
            this.labelCases.Name = "labelCases";
            this.labelCases.Size = new System.Drawing.Size(13, 13);
            this.labelCases.TabIndex = 11;
            this.labelCases.Text = "0";
            // 
            // labelRecordsRead
            // 
            this.labelRecordsRead.AutoSize = true;
            this.labelRecordsRead.Location = new System.Drawing.Point(231, 29);
            this.labelRecordsRead.Name = "labelRecordsRead";
            this.labelRecordsRead.Size = new System.Drawing.Size(13, 13);
            this.labelRecordsRead.TabIndex = 13;
            this.labelRecordsRead.Text = "0";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(146, 29);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(79, 13);
            this.label7.TabIndex = 12;
            this.label7.Text = "Records Read:";
            // 
            // labelRecordsWritten
            // 
            this.labelRecordsWritten.AutoSize = true;
            this.labelRecordsWritten.Location = new System.Drawing.Point(394, 29);
            this.labelRecordsWritten.Name = "labelRecordsWritten";
            this.labelRecordsWritten.Size = new System.Drawing.Size(13, 13);
            this.labelRecordsWritten.TabIndex = 15;
            this.labelRecordsWritten.Text = "0";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(302, 29);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(87, 13);
            this.label6.TabIndex = 14;
            this.label6.Text = "Records Written:";
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "Record";
            this.columnHeader5.Width = 200;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(634, 511);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.buttonClean);
            this.Controls.Add(this.buttonSelectDataFile);
            this.Controls.Add(this.labelCleanedFile);
            this.Controls.Add(this.labelDataFile);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.labelDictionary);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonOpenDictionary);
            this.MinimumSize = new System.Drawing.Size(650, 550);
            this.Name = "MainForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "CSPro Record Cleaner";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonOpenDictionary;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label labelDictionary;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label labelDataFile;
        private System.Windows.Forms.Label labelCleanedFile;
        private System.Windows.Forms.Button buttonSelectDataFile;
        private System.Windows.Forms.Button buttonClean;
        private System.Windows.Forms.ListView listViewStats;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label labelRecordsRead;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label labelCases;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label labelRecordsWritten;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ColumnHeader columnHeader5;
    }
}

