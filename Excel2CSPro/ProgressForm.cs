using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Excel2CSPro
{
    public partial class ProgressForm : Form
    {
        private BackgroundWorker _backgroundWorker = new BackgroundWorker();

        public ProgressForm(string dialogTitle,object runTask)
        {
            InitializeComponent();

            this.Text = dialogTitle;
            
            _backgroundWorker.WorkerReportsProgress = true;
            _backgroundWorker.WorkerSupportsCancellation = true;

            _backgroundWorker.DoWork += new DoWorkEventHandler(
                delegate(object o,DoWorkEventArgs args)
                {
                    if( runTask is CreateDictionaryControl )
                        ( (CreateDictionaryControl)runTask ).RunTask(_backgroundWorker);
                });

            _backgroundWorker.ProgressChanged += new ProgressChangedEventHandler(
                delegate(object o,ProgressChangedEventArgs args)
                {
                    progressBar.Value = Math.Min(args.ProgressPercentage,100);
                });

            _backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(
                delegate(object o,RunWorkerCompletedEventArgs args)
                {
                    this.DialogResult = args.Cancelled ? DialogResult.Cancel : DialogResult.OK;
                    Close();
                });
        }

        private void ProgressForm_Load(object sender,EventArgs e)
        {
            _backgroundWorker.RunWorkerAsync();
        }

        private bool ConfirmExit()
        {
            if( MessageBox.Show(Messages.ConfirmCancelTask,this.Text,MessageBoxButtons.YesNo) == DialogResult.Yes )
            {
                _backgroundWorker.CancelAsync();
                this.DialogResult = DialogResult.Cancel;
                return true;
            }

            else
                return false;
        }

        private void ProgressForm_FormClosing(object sender,FormClosingEventArgs e)
        {
            if( _backgroundWorker.IsBusy && !_backgroundWorker.CancellationPending )
            {
                if( !ConfirmExit() )
                    e.Cancel = true;
            }
        }

        private void buttonCancel_Click(object sender,EventArgs e)
        {
            if( ConfirmExit() )
                Close();
        }

    }
}
