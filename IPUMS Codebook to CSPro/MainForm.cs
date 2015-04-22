using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using CSPro;

namespace IPUMS2CSPro
{
    public partial class MainForm : Form
    {
        private IPUMS.Codebook _codebook = null;
        private string _dictFilename;

        public MainForm()
        {
            InitializeComponent();
        }


        private void buttonIPUMS_Click(object sender,EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "IPUMS Codebook (*.cbk)|*.cbk";

            if( ofd.ShowDialog() == DialogResult.OK )
            {
                try
                {
                    _codebook = new IPUMS.Codebook(ofd.FileName);

                    labelIPUMS.Text = ofd.FileName;

                    labelDescription.Text = _codebook.Description;

                    int records = _codebook.FileType == IPUMS.FileType.Rectangular ? 1 : _codebook.Records.Count;
                    labelRecords.Text = String.Format("{0} ({1})",records,_codebook.FileType.ToString());

                    // count the items
                    bool onFirstRecord = true;
                    int idItems = 0;
                    int otherItems = 0;

                    foreach( var kp in _codebook.Records )
                    {
                        foreach( IPUMS.Variable variable in kp.Value )
                        {
                            if( variable.ID )
                            {
                                if( onFirstRecord )
                                    idItems++;
                            }

                            else if( !variable.RecordType )
                                otherItems++;
                        }

                        onFirstRecord = false;
                    }

                    labelIDItems.Text = idItems.ToString();
                    labelOtherItems.Text = otherItems.ToString();
                    labelValueSets.Text = _codebook.ValueSets.Count.ToString();
                }

                catch( Exception exception )
                {
                    MessageBox.Show(String.Format(Messages.Codebook_ReadError,exception.Message));
                }
            }
        }

        private void buttonCSPro_Click(object sender,EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "CSPro Dictionary (*.dcf)|*.dcf";

            if( sfd.ShowDialog() == DialogResult.OK )
            {
                _dictFilename = sfd.FileName;
                labelCSPro.Text = sfd.FileName;
            }
        }


        private void buttonConvert_Click(object sender,EventArgs e)
        {
            if( _codebook == null )
            {
                MessageBox.Show(Messages.Convert_OpenCodebook);
                return;
            }

            if( _dictFilename == null )
            {
                MessageBox.Show(Messages.Convert_SelectDictionary);
                return;
            }

            if( !_codebook.Records.ContainsKey("H") )
            {
                MessageBox.Show(Messages.Convert_NoHousingRecord);
                return;
            }

            // create a CSPro dictionary from the IPUMS codebook
            DataDictionary dict = new DataDictionary();

            dict.Name = "IPUMS_DICT";
            dict.Label = _codebook.Description;

            List<IPUMS.Variable> housingRecord = _codebook.Records["H"];

            // go through the housing record, see if there is a record type, and set up all the ID fields

            Level level = new Level(dict,1); // IPUMS dictionaries will always be one-level
            dict.AddLevel(level);

            level.Name = "IPUMS_LEVEL";
            level.Label = _codebook.Description;

            foreach( IPUMS.Variable variable in housingRecord )
            {
                if( variable.RecordType )
                {
                    dict.RecTypeStart = variable.StartPosition;
                    dict.RecTypeLength = variable.Length;
                }

                else if( variable.ID )
                    level.IDs.AddItem(CreateItem(variable,level));
            }

            // now go through and create each record
            Record thisRecord = null;

            foreach( var kp in _codebook.Records )
            {
                string recLabel = kp.Key == "H" ? "Housing" : kp.Key == "P" ? "Population" : kp.Key;

                if( thisRecord == null || dict.RecTypeLength > 0 ) // if there is a record type, we will create a new CSPro record for every IPUMS record
                {
                    thisRecord = new Record(level);
                    level.Records.Add(thisRecord);

                    thisRecord.Name = recLabel.ToUpper() + "_REC";
                    thisRecord.Label = recLabel;

                    bool isHousingRecord = ( kp.Key == "H" );

                    thisRecord.Required = isHousingRecord; // only require housing records

                    if( dict.RecTypeLength > 0 )
                        thisRecord.RecordType = kp.Key;

                    // 100 is assumed to be the maximum number of population occurrences
                    thisRecord.MaxOccs = ( isHousingRecord && _codebook.FileType != IPUMS.FileType.Rectangular) ? 1 : 100;
                }

                else // this is a record that has multiple types on one record (e.g., housing and population)
                    thisRecord.Label = thisRecord.Label + " + " + recLabel;

                foreach( IPUMS.Variable variable in kp.Value )
                {
                    if( !variable.RecordType && !variable.ID )
                        thisRecord.AddItem(CreateItem(variable,level));
                }
            }

            try
            {
                DataDictionaryWorker.CalculateRecordLengths(dict);
                DataDictionaryWriter.Save(dict,_dictFilename);
                MessageBox.Show(Messages.Convert_Success);
            }

            catch( Exception exception )
            {
                MessageBox.Show(String.Format(Messages.Convert_Failure,exception.Message));
            }
        }


        private Item CreateItem(IPUMS.Variable variable,Level level)
        {
            Item item = new Item(level.IDs);

            item.Name = variable.Name;

            if( _codebook.ValueSets.ContainsKey(variable.Name) )
                item.Label = _codebook.ValueSets[variable.Name].Label;

            else
                item.Label = variable.Name;

            item.Start = variable.StartPosition;
            item.Length = variable.Length;
            item.ZeroFill = true;

            // see if there is a value set for this item
            if( _codebook.ValueSets.ContainsKey(variable.Name) )
            {
                IPUMS.ValueSet ipumsVS = _codebook.ValueSets[variable.Name];

                ValueSet vs = new ValueSet(item);
                item.AddValueSet(vs);

                vs.Name = ipumsVS.Name + "_VS";
                vs.Label = ipumsVS.Label;

                foreach( IPUMS.Value ipumsValue in ipumsVS.Values )
                {
                    Value value = new Value();
                    vs.AddValue(value);

                    value.Label = ipumsValue.Label;

                    ValuePair valuePair = new ValuePair();
                    value.AddValuePair(valuePair);

                    valuePair.From = ipumsValue.Code;
                }
            }

            return item;
        }


        private void pictureBoxIcon_Click(object sender,EventArgs e)
        {
            Process.Start("http://www.ipums.org/");
        }

    }
}
