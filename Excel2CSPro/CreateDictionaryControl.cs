using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;

namespace Excel2CSPro
{
    public partial class CreateDictionaryControl : UserControl
    {
        private WorkbookController _workbookController = new WorkbookController();
        private Excel.Worksheet _worksheet = null;
        private List<ColumnAnalysis> _columnAnalysis = null;
        private List<CreateDictionaryItemControl> _itemSettings = null;
        private string _lastSaveFilename = null;

        public CreateDictionaryControl()
        {
            InitializeComponent();

            textBoxStartingRow.Text = Messages.ExcelDefaultStartingRow;
        }

        private void buttonSelectExcelFile_Click(object sender,EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = Messages.ExcelOpenTitle;
            ofd.Filter = Messages.ExcelOpenFilter;

            if( ofd.ShowDialog() == DialogResult.OK )
            {
                try
                {
                    _workbookController.OpenWorksheet(ofd.FileName);

                    List<string> worksheetNames = _workbookController.WorksheetNames;

                    comboBoxWorksheet.Items.Clear();

                    foreach( string name in worksheetNames )
                        comboBoxWorksheet.Items.Add(name);

                    comboBoxWorksheet.SelectedIndex = 0;

                    groupBoxExcelOptions.Text = String.Format(Messages.ExcelGroupTitle,ofd.FileName);
                    buttonAnalyzeWorksheet.Enabled = true;
                }

                catch( Exception exception )
                {
                    MessageBox.Show(String.Format(Messages.ExcelIOError,exception.Message));
                }
            }
        }

        public class ColumnAnalysis
        {
            public string ColumnAddress = "";
            public string HeaderText = "";
            public bool ContainsAlpha = false;
            public int DigitsBeforeDecimal = 0;
            public int DigitsAfterDecimal = 0;
            public int MaximumLength = 0;
            public int ValidValues = 0;

            public const int MaxValueSetValues = 500;
            public HashSet<double> Values = new HashSet<double>();
        }

        private void buttonAnalyzeWorksheet_Click(object sender,EventArgs e)
        {
            try
            {
                _worksheet = _workbookController.GetWorksheet(comboBoxWorksheet.SelectedIndex);

                List<ColumnAnalysis> previousColumnAnalysis = _columnAnalysis;

                ProgressForm progressDlg = new ProgressForm(String.Format(Messages.TitleReadingWorksheet,comboBoxWorksheet.SelectedItem.ToString()),this);

                if( progressDlg.ShowDialog() != DialogResult.OK || previousColumnAnalysis == _columnAnalysis )
                    return;

                // draw the data about the read items
                _itemSettings = new List<CreateDictionaryItemControl>();
                panelDictionaryContents.Controls.Clear();

                int yPos = 0;

                foreach( ColumnAnalysis ca in _columnAnalysis )
                {
                    CreateDictionaryItemControl cdic = new CreateDictionaryItemControl(ca);
                    cdic.Location = new Point(cdic.Margin.Left,cdic.Margin.Top + yPos);

                    panelDictionaryContents.Controls.Add(cdic);
                    _itemSettings.Add(cdic);

                    yPos += cdic.Height;
                }

                groupBoxDictionaryContents.Visible = true;
                groupBoxCreateDictionary.Visible = true;
                textBoxNamePrefix.Text = CSPro.Names.CreateFromLabel(comboBoxWorksheet.SelectedItem.ToString());
                buttonCreateDictionary.Enabled = true;
                _lastSaveFilename = null;
            }

            catch( Exception exception )
            {
                MessageBox.Show(String.Format(Messages.ExcelIOError,exception.Message));
            }
        }

        public void RunTask(BackgroundWorker backgroundWorker)
        {
            try
            {
                int startingRow = _workbookController.ParseStartingRow(textBoxStartingRow.Text);

                int numRows = _worksheet.UsedRange.Rows.Count;
                int numCols = _worksheet.UsedRange.Columns.Count;
                int numDataRows = _worksheet.UsedRange.Rows.Count - startingRow + 1;

                double progress = 0;
                double progressRowIncrementer = 100.0 / numDataRows;

                List<ColumnAnalysis> columnsAnalysis = new List<ColumnAnalysis>();

                // read the Excel file
                for( int col = 1; !backgroundWorker.CancellationPending && col <= numCols; col++ )
                {
                    ColumnAnalysis ca = new ColumnAnalysis();

                    // the column address will appear in a format like $A:$A
                    ca.ColumnAddress = (string)_worksheet.Columns[col].Address;
                    ca.ColumnAddress = ca.ColumnAddress.Substring(1,ca.ColumnAddress.IndexOf(':') - 1);

                    // read the first row, which may be header text
                    object header = _worksheet.Cells[1,col].Value2;

                    if( header != null )
                        ca.HeaderText = header.ToString().Trim();

                    else
                        ca.HeaderText = String.Format("Column {0}",ca.ColumnAddress);                    

                    Excel.Range dataRange = _worksheet.Range[_worksheet.Cells[startingRow,col],_worksheet.Cells[numRows,col]];
                    object[,] data = dataRange.Value2;

                    if( data == null )
                        throw new Exception(Messages.ExcelNoData);

                    for( int row = 1; !backgroundWorker.CancellationPending && row <= numDataRows; row++ )
                    {
                        object cell = data[row,1];

                        if( cell != null )
                        {
                            ca.ValidValues++;

                            bool isNumeric = false;
                            double doubleValue = 0;

                            if( !ca.ContainsAlpha ) // see if this value is a valid numeric
                            {
                                if( cell is double )
                                {
                                    doubleValue = (double)cell;
                                    isNumeric = true;
                                }

                                else if( cell is string )
                                    isNumeric = Double.TryParse((string)cell,out doubleValue);
                            }

                            if( isNumeric )
                            {
                                if( ca.Values.Count < ColumnAnalysis.MaxValueSetValues && !ca.Values.Contains(doubleValue) )
                                    ca.Values.Add(doubleValue);

                                // assume a maximum of six digits after the decimal point
                                string formattedDouble = doubleValue.ToString("0.000000",CultureInfo.InvariantCulture);
                                formattedDouble = formattedDouble.TrimEnd('0');

                                int digitsBeforeDecimal = formattedDouble.IndexOf('.');
                                int digitsAfterDecimal = formattedDouble.Length - digitsBeforeDecimal - 1;

                                ca.DigitsBeforeDecimal = Math.Max(ca.DigitsBeforeDecimal,digitsBeforeDecimal);
                                ca.DigitsAfterDecimal = Math.Max(ca.DigitsAfterDecimal,digitsAfterDecimal);

                                ca.MaximumLength = Math.Max(ca.MaximumLength,digitsBeforeDecimal + digitsAfterDecimal + ( ( digitsAfterDecimal > 0 ) ? 1 : 0 ));
                            }

                            else
                            {
                                string sValue = (cell.ToString()).Trim();
                                ca.ContainsAlpha = true;
                                ca.MaximumLength = Math.Max(ca.MaximumLength,sValue.Length);
                            }
                        }
                
                        progress += progressRowIncrementer;
                        backgroundWorker.ReportProgress((int)progress);
                    }

                    if( ca.ValidValues != 0 )
                        columnsAnalysis.Add(ca);
                }

                if( !backgroundWorker.CancellationPending )
                    _columnAnalysis = columnsAnalysis;
            }

            catch( Exception exception )
            {
                MessageBox.Show(String.Format(Messages.ExcelIOError,exception.Message));
            }
        }

        
        private void buttonCreateDictionary_Click(object sender,EventArgs e)
        {
            string namePrefix = textBoxNamePrefix.Text.ToUpper();

            if( !CSPro.Names.IsValid(namePrefix) )
            {
                string suggestedNamePrefix = CSPro.Names.CreateFromLabel(namePrefix);

                if( MessageBox.Show(String.Format(Messages.InvalidNameSuggestOther,namePrefix,suggestedNamePrefix),this.Text,MessageBoxButtons.YesNo) == DialogResult.No )
                    return;

                namePrefix = suggestedNamePrefix;
            }

            HashSet<string> usedNames = new HashSet<string>();

            string dictionaryName = CSPro.Names.CreateFromLabel(namePrefix + "_DICT");
            string levelName = CSPro.Names.CreateFromLabel(namePrefix + "_LEVEL");
            string recordName = CSPro.Names.CreateFromLabel(namePrefix + "_REC");

            usedNames.Add(dictionaryName);
            usedNames.Add(levelName);
            usedNames.Add(recordName);

            List<CreateDictionaryItemControl.ItemSelections> ids = new List<CreateDictionaryItemControl.ItemSelections>();
            List<CreateDictionaryItemControl.ItemSelections> items = new List<CreateDictionaryItemControl.ItemSelections>();

            CreateDictionaryItemControl cdicForMessages = null;

            try
            {
                foreach( CreateDictionaryItemControl cdic in _itemSettings )
                {
                    cdicForMessages = cdic;

                    CreateDictionaryItemControl.ItemSelections selections = cdic.Selections;

                    if( !CSPro.Names.IsValid(selections.Name) )
                        throw new Exception(String.Format(Messages.InvalidName,selections.Name));

                    if( usedNames.Contains(selections.Name) )
                        throw new Exception(String.Format(Messages.AlreadyUsedName,selections.Name));

                    usedNames.Add(selections.Name);

                    int length = 0;

                    if( selections.IsNumeric )
                    {
                        length = selections.BeforeDecLength;

                        if( selections.AfterDecLength > 0 )
                        {
                            if( selections.IsId )
                                throw new Exception(Messages.InvalidIdNumericDecimal);

                            if( selections.AfterDecLength > CSPro.Defines.MaxLengthDecimal )
                                throw new Exception(String.Format(Messages.InvalidLengthNumericDecimal,CSPro.Defines.MaxLengthDecimal));

                            length += selections.AfterDecLength + ( checkBoxDecChar.Checked ? 1 : 0 );
                        }

                        if( length > CSPro.Defines.MaxLengthNumeric )
                            throw new Exception(String.Format(Messages.InvalidLengthNumeric,CSPro.Defines.MaxLengthNumeric));
                    }

                    else
                    {
                        length = selections.AlphaLength;

                        if( length > CSPro.Defines.MaxLengthAlpha )
                            throw new Exception(String.Format(Messages.InvalidLengthAlpha,CSPro.Defines.MaxLengthAlpha));
                    }

                    if( length == 0 )
                        throw new Exception(Messages.InvalidLengthZero);

                    if( selections.IsId )
                        ids.Add(selections);

                    else
                        items.Add(selections);
                }

                cdicForMessages = null;

                if( ids.Count == 0 )
                    throw new Exception(Messages.InvalidNumberIds);

                // save the dictionary
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Title = Messages.CSProSaveTitle;
                sfd.Filter = Messages.CSProFileFilter;

                if( _lastSaveFilename != null )
                {
                    sfd.InitialDirectory = Path.GetDirectoryName(_lastSaveFilename);
                    sfd.FileName = Path.GetFileName(_lastSaveFilename);
                }

                if( sfd.ShowDialog() != DialogResult.OK )
                    return;

                CSPro.DataDictionary dictionary = new CSPro.DataDictionary();
                dictionary.DecimalCharDefault = checkBoxDecChar.Checked;
                dictionary.ZeroFillDefault = checkBoxZeroFill.Checked;

                dictionary.Name = dictionaryName;
                dictionary.Label = textBoxNamePrefix.Text + " Dictionary";

                CSPro.Level level = new CSPro.Level(dictionary,0);
                dictionary.AddLevel(level);
                level.Name = levelName;
                level.Label = textBoxNamePrefix.Text + " Level";

                CSPro.Record record = new CSPro.Record(level);
                level.AddRecord(record);
                record.Name = recordName;
                record.Label = textBoxNamePrefix.Text + " Record";

                int startingPos = 1;

                foreach( CreateDictionaryItemControl.ItemSelections selections in ids )
                    AddItem(level.IDs,selections,ref startingPos);

                foreach( CreateDictionaryItemControl.ItemSelections selections in items )
                    AddItem(record,selections,ref startingPos);

                record.Length = startingPos;

                CSPro.DataDictionaryWriter.Save(dictionary,sfd.FileName);
                _lastSaveFilename = sfd.FileName;
            }

            catch( Exception exception )
            {
                if( cdicForMessages != null )
                    MessageBox.Show(String.Format("{0}: {1}",cdicForMessages.ColumnText,exception.Message));

                else
                    MessageBox.Show(exception.Message);

                return;
            }
        }

        private void AddItem(CSPro.Record record,CreateDictionaryItemControl.ItemSelections selections,ref int startingPos)
        {
            CSPro.Item item = new CSPro.Item(record);
            record.AddItem(item);
            item.Name = CSPro.Names.CreateFromLabel(selections.Name);
            item.Label = selections.Name;

            int length = 0;

            item.Start = startingPos;
            item.Numeric = selections.IsNumeric;

            if( item.Numeric )
            {
                item.ZeroFill = checkBoxZeroFill.Checked;

                length = selections.BeforeDecLength;

                if( selections.AfterDecLength > 0 )
                {
                    item.DecChar = checkBoxDecChar.Checked;
                    item.Decimal = selections.AfterDecLength;
                    length += selections.AfterDecLength + ( checkBoxDecChar.Checked ? 1 : 0 );
                }

                if( selections.CreateValueSet )
                {
                    CSPro.ValueSet vs = new CSPro.ValueSet(item);
                    item.AddValueSet(vs);
                    vs.Name = CSPro.Names.CreateFromLabel(item.Name + "_VS1");
                    vs.Label = item.Label;
                    
                    foreach( double doubleValue in selections.Values )
                    {
                        CSPro.Value value = new CSPro.Value();
                        vs.AddValue(value);
                        value.Label = String.Format("Value {0}",doubleValue.ToString());

                        CSPro.ValuePair vp = new CSPro.ValuePair();
                        vp.From = doubleValue.ToString();

                        value.AddValuePair(vp);
                    }
                }
            }

            else
                length = selections.AlphaLength;

            item.Length = length;

            startingPos += length;
        }

    }
}
