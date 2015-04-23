using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using CSPro;

namespace RecordCleaner
{
    public partial class MainForm : Form
    {
        private class RecordType
        {
            public string Label;
            public string Type;
            public bool Required;
            public int Max;
            public bool Valid;

            public RecordType(string label,string type,bool required,int max,bool valid)
            {
                Label = label;
                Type = type;
                Required = required;
                Max = max;
                Valid = valid;
            }
            
            public int CurrentCount = 0;
            
            public int Read = 0;
            public int Deleted = 0;
            public int Inserted = 0;
        }

        private DataDictionary dictionary = null;
        private string dataFilename = null;
        private string cleanedFilename = null;
        private int casesRead,recordsRead,recordsWritten;
        private Dictionary<string,RecordType> recordTypes = null;


        public MainForm()
        {
            InitializeComponent();
        }

        private void buttonOpenDictionary_Click(object sender,EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "CSPro Dictionary (*.dcf)|*.dcf";

            if( ofd.ShowDialog() == DialogResult.OK )
            {
                try
                {
                    dictionary = DataDictionaryReader.Load(ofd.FileName);

                    if( dictionary.Levels.Count != 1 )
                        throw new Exception(Messages.Dictionary_TwoLevelError);

                    else if( dictionary.RecTypeLength == 0 )
                        throw new Exception(Messages.Dictionary_NoRecordType);
                    
                    labelDictionary.Text = dictionary.Label;

                    SetupInitialRecordTypes();
                    casesRead = 0;
                    recordsRead = 0;
                    recordsWritten = 0;
                        
                    UpdateStats(false);
                }

                catch( Exception exception )
                {
                    dictionary = null;
                    labelDictionary.Text = "";
                    MessageBox.Show(String.Format(Messages.Dictionary_LoadError,exception.Message));
                }
            }

            EnableCleanButton();
        }

        private void buttonSelectDataFile_Click(object sender,EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Data Files (*.*)|*.*";

            if( ofd.ShowDialog() == DialogResult.OK )
            {
                dataFilename = ofd.FileName;
                cleanedFilename = ofd.FileName + ".cleaned";

                labelDataFile.Text = Path.GetFileName(dataFilename);
                labelCleanedFile.Text = Path.GetFileName(cleanedFilename);
                labelCleanedFile.Visible = true;

                EnableCleanButton();
            }            
        }

        private void EnableCleanButton()
        {
            buttonClean.Enabled = ( dictionary != null && dataFilename != null );
        }

        private void buttonClean_Click(object sender,EventArgs e)
        {
            try
            {
                const int StatusUpdateFrequency = 10000;
                int statusUpdater = 0;

                SetupInitialRecordTypes();
                casesRead = 0;
                recordsRead = 0;
                recordsWritten = 0;

                int minRecLen = dictionary.RecTypeStart + dictionary.RecTypeLength - 1;

                Record idsRecord = dictionary.Levels[0].IDs;

                foreach( Item item in idsRecord.Items )
                    minRecLen = Math.Max(minRecLen,item.Start + item.Length - 1);

                bool dataFileIsUtf8 = IO.IsFileUtf8(dataFilename);

                using( TextReader tr = IO.CreateStreamReaderFromCSProFile(dataFilename) )
                {
                    using( TextWriter tw = IO.CreateStreamWriterForCSProFile(cleanedFilename,dataFileIsUtf8) )
                    {
                        string line;
                        string prevCaseID = null;

                        while( ( line = tr.ReadLine() ) != null )
                        {
                            if( line.Length < minRecLen )
                                line = line + new String(' ',minRecLen - line.Length);

                            string recType = line.Substring(dictionary.RecTypeStart - 1,dictionary.RecTypeLength);

                            StringBuilder ids = new StringBuilder();

                            foreach( Item item in idsRecord.Items )
                                ids.Append(line.Substring(item.Start - 1,item.Length));

                            string idsString = ids.ToString();

                            if( prevCaseID == null || !prevCaseID.Equals(idsString) )
                            {
                                WriteMissingRecords(tw,prevCaseID,minRecLen);
                                ResetRecordCurrentCounts();
                                casesRead++;
                                prevCaseID = idsString;
                            }

                            recordsRead++;

                            bool writeLine = true;

                            if( !recordTypes.ContainsKey(recType) )
                                recordTypes.Add(recType,new RecordType("<invalid>",recType,false,1,false));

                            RecordType rt = recordTypes[recType];
                            rt.Read++;

                            if( rt.Valid )
                            {
                                rt.CurrentCount++;

                                if( rt.CurrentCount > rt.Max )
                                {
                                    rt.Deleted++;
                                    writeLine = false;
                                }
                            }

                            else
                            {
                                rt.Deleted++;
                                writeLine = false;
                            }

                            if( writeLine )
                            {
                                recordsWritten++;
                                tw.WriteLine(line);
                            }

                            if( --statusUpdater < 0 )
                            {
                                UpdateStats(true);
                                statusUpdater = StatusUpdateFrequency;
                            }
                        }

                        WriteMissingRecords(tw,prevCaseID,minRecLen);
                        UpdateStats(true);
                    }
                }

                MessageBox.Show(Messages.CleanSuccess);
            }

            catch( Exception exception )
            {
                MessageBox.Show(String.Format(Messages.CleanError,exception.Message));
            }
        }

        private void SetupInitialRecordTypes()
        {
            recordTypes = new Dictionary<string,RecordType>();

            foreach( Record record in dictionary.Levels[0].Records )
                recordTypes.Add(record.RecordType,new RecordType(record.Label,record.RecordType,record.Required,record.MaxOccs,true));
        }

        private void ResetRecordCurrentCounts()
        {
            foreach( var kp in recordTypes )
                kp.Value.CurrentCount = 0;
        }

        private void WriteMissingRecords(TextWriter tw,string prevCaseID,int minRecLen)
        {
            if( prevCaseID == null )
                return;

            StringBuilder line = null;

            foreach( var kp in recordTypes )
            {
                RecordType rt = kp.Value;

                if( rt.Valid && rt.Required && rt.CurrentCount == 0 )
                {
                    // set up the IDs, if necessary
                    if( line == null )
                    {
                        line = new StringBuilder(new String(' ',minRecLen));

                        int idPos = 0;

                        foreach( Item item in dictionary.Levels[0].IDs.Items )
                        {
                            line.Remove(item.Start - 1,item.Length);
                            line.Insert(item.Start - 1,prevCaseID.Substring(idPos,item.Length));
                            idPos += item.Length;
                        }
                    }

                    line.Remove(dictionary.RecTypeStart - 1,dictionary.RecTypeLength);
                    line.Insert(dictionary.RecTypeStart - 1,kp.Value.Type);

                    rt.Inserted++;
                    recordsWritten++;
                    tw.WriteLine(line.ToString());
                }
            }
        }

        private void UpdateStats(bool inCleaning)
        {
            labelCases.Text = String.Format("{0:N0}",casesRead);
            labelRecordsRead.Text = String.Format("{0:N0}",recordsRead);
            labelRecordsWritten.Text = String.Format("{0:N0}",recordsWritten);

            listViewStats.Items.Clear();

            foreach( var kp in recordTypes )
            {
                RecordType rt = kp.Value;

                ListViewItem lvi = listViewStats.Items.Add(rt.Label);
                lvi.SubItems.Add(rt.Type);

                if( inCleaning )
                {
                    lvi.SubItems.Add(String.Format("{0:N0}",rt.Read));
                    lvi.SubItems.Add(String.Format("{0:N0}",rt.Inserted));
                    lvi.SubItems.Add(String.Format("{0:N0}",rt.Deleted));
                }
            }
        }

    }
}
