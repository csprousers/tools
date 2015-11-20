namespace Excel2CSPro
{
    partial class CommandLineHelpForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CommandLineHelpForm));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxCommandLine = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.labelExcelOverride = new System.Windows.Forms.Label();
            this.labelCSProOverride = new System.Windows.Forms.Label();
            this.buttonClose = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // textBoxCommandLine
            // 
            resources.ApplyResources(this.textBoxCommandLine, "textBoxCommandLine");
            this.textBoxCommandLine.Name = "textBoxCommandLine";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // labelExcelOverride
            // 
            resources.ApplyResources(this.labelExcelOverride, "labelExcelOverride");
            this.labelExcelOverride.Name = "labelExcelOverride";
            // 
            // labelCSProOverride
            // 
            resources.ApplyResources(this.labelCSProOverride, "labelCSProOverride");
            this.labelCSProOverride.Name = "labelCSProOverride";
            // 
            // buttonClose
            // 
            resources.ApplyResources(this.buttonClose, "buttonClose");
            this.buttonClose.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.UseVisualStyleBackColor = true;
            // 
            // CommandLineHelpForm
            // 
            this.AcceptButton = this.buttonClose;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonClose;
            this.Controls.Add(this.buttonClose);
            this.Controls.Add(this.labelCSProOverride);
            this.Controls.Add(this.labelExcelOverride);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textBoxCommandLine);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "CommandLineHelpForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxCommandLine;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label labelExcelOverride;
        private System.Windows.Forms.Label labelCSProOverride;
        private System.Windows.Forms.Button buttonClose;
    }
}