using System;
using System.Windows.Forms;

namespace Excel2CSPro
{
    public partial class MainForm : Form
    {
        private string _specFilename = null;

        public MainForm()
        {
            InitializeComponent();
        }

        private void tabControlMain_SelectedIndexChanged(object sender,EventArgs e)
        {
            bool menuOptionsEnabled = ( tabControlMain.SelectedTab == tabPageExcel2CSPro );
            menuItemNew.Enabled = menuOptionsEnabled;
            menuItemOpen.Enabled = menuOptionsEnabled;
            menuItemSave.Enabled = menuOptionsEnabled;
            menuItemSaveAs.Enabled = menuOptionsEnabled;
            menuItemCommandLine.Enabled = menuOptionsEnabled;
        }

        private bool ConfirmExit()
        {
            // TODO
            WorkbookController.Close();
            return true;
        }

        private void MainForm_FormClosing(object sender,FormClosingEventArgs e)
        {
            if( !ConfirmExit() )
                e.Cancel = true;
        }

        private void menuItemExit_Click(object sender,EventArgs e)
        {
            if( ConfirmExit() )
                this.Close();
        }

        private void menuItemCommandLine_Click(object sender,EventArgs e)
        {
            if( _specFilename == null )
                MessageBox.Show(Messages.CommandLineRequiresSpec);

            else
                new CommandLineHelpForm(_specFilename).ShowDialog();
        }

        private void menuItemAbout_Click(object sender,EventArgs e)
        {
            new AboutForm().ShowDialog();
        }

    }
}
