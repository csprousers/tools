namespace IPUMS2CSPro
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.buttonIPUMS = new System.Windows.Forms.Button();
            this.buttonCSPro = new System.Windows.Forms.Button();
            this.buttonConvert = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.labelDescription = new System.Windows.Forms.Label();
            this.labelRecords = new System.Windows.Forms.Label();
            this.labelIDItems = new System.Windows.Forms.Label();
            this.labelValueSets = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.pictureBoxIcon = new System.Windows.Forms.PictureBox();
            this.labelIPUMS = new System.Windows.Forms.Label();
            this.labelCSPro = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.labelOtherItems = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxIcon)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 44);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(96, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "IPUMS Codebook:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 74);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(90, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "CSPro Dictionary:";
            // 
            // buttonIPUMS
            // 
            this.buttonIPUMS.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonIPUMS.Location = new System.Drawing.Point(662, 39);
            this.buttonIPUMS.Name = "buttonIPUMS";
            this.buttonIPUMS.Size = new System.Drawing.Size(91, 23);
            this.buttonIPUMS.TabIndex = 0;
            this.buttonIPUMS.Text = "Open";
            this.buttonIPUMS.UseVisualStyleBackColor = true;
            this.buttonIPUMS.Click += new System.EventHandler(this.buttonIPUMS_Click);
            // 
            // buttonCSPro
            // 
            this.buttonCSPro.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCSPro.Location = new System.Drawing.Point(662, 70);
            this.buttonCSPro.Name = "buttonCSPro";
            this.buttonCSPro.Size = new System.Drawing.Size(91, 23);
            this.buttonCSPro.TabIndex = 1;
            this.buttonCSPro.Text = "Select";
            this.buttonCSPro.UseVisualStyleBackColor = true;
            this.buttonCSPro.Click += new System.EventHandler(this.buttonCSPro_Click);
            // 
            // buttonConvert
            // 
            this.buttonConvert.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonConvert.Location = new System.Drawing.Point(662, 100);
            this.buttonConvert.Name = "buttonConvert";
            this.buttonConvert.Size = new System.Drawing.Size(91, 23);
            this.buttonConvert.TabIndex = 2;
            this.buttonConvert.Text = "Convert";
            this.buttonConvert.UseVisualStyleBackColor = true;
            this.buttonConvert.Click += new System.EventHandler(this.buttonConvert_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(108, 104);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(63, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Description:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(108, 120);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(50, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "Records:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(108, 136);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(49, 13);
            this.label5.TabIndex = 9;
            this.label5.Text = "ID Items:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(108, 168);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(61, 13);
            this.label6.TabIndex = 10;
            this.label6.Text = "Value Sets:";
            // 
            // labelDescription
            // 
            this.labelDescription.AutoSize = true;
            this.labelDescription.Location = new System.Drawing.Point(181, 104);
            this.labelDescription.Name = "labelDescription";
            this.labelDescription.Size = new System.Drawing.Size(16, 13);
            this.labelDescription.TabIndex = 11;
            this.labelDescription.Text = "---";
            // 
            // labelRecords
            // 
            this.labelRecords.AutoSize = true;
            this.labelRecords.Location = new System.Drawing.Point(181, 120);
            this.labelRecords.Name = "labelRecords";
            this.labelRecords.Size = new System.Drawing.Size(16, 13);
            this.labelRecords.TabIndex = 12;
            this.labelRecords.Text = "---";
            // 
            // labelIDItems
            // 
            this.labelIDItems.AutoSize = true;
            this.labelIDItems.Location = new System.Drawing.Point(181, 136);
            this.labelIDItems.Name = "labelIDItems";
            this.labelIDItems.Size = new System.Drawing.Size(16, 13);
            this.labelIDItems.TabIndex = 13;
            this.labelIDItems.Text = "---";
            // 
            // labelValueSets
            // 
            this.labelValueSets.AutoSize = true;
            this.labelValueSets.Location = new System.Drawing.Point(181, 168);
            this.labelValueSets.Name = "labelValueSets";
            this.labelValueSets.Size = new System.Drawing.Size(16, 13);
            this.labelValueSets.TabIndex = 14;
            this.labelValueSets.Text = "---";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(12, 9);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(737, 13);
            this.label7.TabIndex = 15;
            this.label7.Text = "The data file that IPUMS generates can be ready directly by CSPro. The codebook, " +
    "however, must be converted to a CSPro dictionary. This tool does that.";
            // 
            // pictureBoxIcon
            // 
            this.pictureBoxIcon.Image = global::IPUMS2CSPro.Properties.Resources.IPUMS_Logo;
            this.pictureBoxIcon.Location = new System.Drawing.Point(15, 103);
            this.pictureBoxIcon.Name = "pictureBoxIcon";
            this.pictureBoxIcon.Size = new System.Drawing.Size(80, 80);
            this.pictureBoxIcon.TabIndex = 16;
            this.pictureBoxIcon.TabStop = false;
            this.pictureBoxIcon.Click += new System.EventHandler(this.pictureBoxIcon_Click);
            // 
            // labelIPUMS
            // 
            this.labelIPUMS.AutoSize = true;
            this.labelIPUMS.Location = new System.Drawing.Point(108, 44);
            this.labelIPUMS.Name = "labelIPUMS";
            this.labelIPUMS.Size = new System.Drawing.Size(176, 13);
            this.labelIPUMS.TabIndex = 17;
            this.labelIPUMS.Text = "<select an input IPUMS codebook>";
            // 
            // labelCSPro
            // 
            this.labelCSPro.AutoSize = true;
            this.labelCSPro.Location = new System.Drawing.Point(108, 74);
            this.labelCSPro.Name = "labelCSPro";
            this.labelCSPro.Size = new System.Drawing.Size(235, 13);
            this.labelCSPro.TabIndex = 18;
            this.labelCSPro.Text = "<select the name of an output CSPro dictionary>";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(108, 152);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(64, 13);
            this.label8.TabIndex = 19;
            this.label8.Text = "Other Items:";
            // 
            // labelOtherItems
            // 
            this.labelOtherItems.AutoSize = true;
            this.labelOtherItems.Location = new System.Drawing.Point(181, 152);
            this.labelOtherItems.Name = "labelOtherItems";
            this.labelOtherItems.Size = new System.Drawing.Size(16, 13);
            this.labelOtherItems.TabIndex = 20;
            this.labelOtherItems.Text = "---";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(765, 197);
            this.Controls.Add(this.labelOtherItems);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.labelCSPro);
            this.Controls.Add(this.labelIPUMS);
            this.Controls.Add(this.pictureBoxIcon);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.labelValueSets);
            this.Controls.Add(this.labelIDItems);
            this.Controls.Add(this.labelRecords);
            this.Controls.Add(this.labelDescription);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.buttonConvert);
            this.Controls.Add(this.buttonCSPro);
            this.Controls.Add(this.buttonIPUMS);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(781, 236);
            this.Name = "MainForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "IPUMS Codebook to CSPro Dictionary";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxIcon)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button buttonIPUMS;
        private System.Windows.Forms.Button buttonCSPro;
        private System.Windows.Forms.Button buttonConvert;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label labelDescription;
        private System.Windows.Forms.Label labelRecords;
        private System.Windows.Forms.Label labelIDItems;
        private System.Windows.Forms.Label labelValueSets;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.PictureBox pictureBoxIcon;
        private System.Windows.Forms.Label labelIPUMS;
        private System.Windows.Forms.Label labelCSPro;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label labelOtherItems;
    }
}

