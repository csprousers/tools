using System;
using System.IO;
using System.Windows.Forms;

namespace Excel2CSPro
{
    public partial class CommandLineHelpForm : Form
    {
        public CommandLineHelpForm(string specFilename)
        {
            InitializeComponent();

            string exeFilename = System.Reflection.Assembly.GetEntryAssembly().Location;

            textBoxCommandLine.Text = String.Format("\"{0}\" \"{1}\"",exeFilename,specFilename);
            labelExcelOverride.Text = String.Format(labelExcelOverride.Text,Path.GetFileName(specFilename));
            labelCSProOverride.Text = String.Format(labelCSProOverride.Text,Path.GetFileName(specFilename));
        }
    }
}
