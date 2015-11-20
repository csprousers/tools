using System;
using System.Windows.Forms;

namespace Excel2CSPro
{
    public partial class AboutForm : Form
    {
        public AboutForm()
        {
            InitializeComponent();

            labelVersion.Text = String.Format(labelVersion.Text,CSPro.CurrentVersion.VersionString);
        }
    }
}
