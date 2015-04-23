using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace CSPro
{
    public class DataDictionaryWriter
    {
        private string _filename = null;
        private TextWriter _tw = null;
        private HashSet<int> _vsLinks = new HashSet<int>();

        private DataDictionaryWriter()
        {
            // a DataDictionaryWriter object can only be created from within the Save function
        }

        public static void Save(DataDictionary dictionary,string filename)
        {
            DataDictionaryWriter ddw = new DataDictionaryWriter();
            ddw._filename = filename;

            using( ddw._tw = IO.CreateStreamWriterForCSProFile(filename,dictionary.Version) )
            {
                // save the header
                ddw._tw.WriteLine(DataDictionaryElements.DICT_HEADER);
                ddw.SaveOption(DataDictionaryElements.HEADER_VERSION,String.Format(CultureInfo.InvariantCulture,"{0} {1:F1}",
                    DataDictionaryElements.HEADER_VERSION_CSPRO,dictionary.Version));

                ddw.SaveSharedComponents(dictionary);

                ddw.SaveOption(DataDictionaryElements.HEADER_RECSTART,dictionary.RecTypeStart);
                ddw.SaveOption(DataDictionaryElements.HEADER_RECLEN,dictionary.RecTypeLength);

                ddw.SaveOption(DataDictionaryElements.HEADER_POSITIONS,
                    dictionary.RelativePositioning ? DataDictionaryElements.HEADER_RELATIVE : DataDictionaryElements.HEADER_ABSOLUTE);

                ddw.SaveOption(DataDictionaryElements.HEADER_ZEROFILL,dictionary.ZeroFillDefault);
                ddw.SaveOption(DataDictionaryElements.HEADER_DECCHAR,dictionary.DecimalCharDefault);

                if( dictionary.UsingValueSetImages )
                    ddw.SaveOption(DataDictionaryElements.HEADER_VALUESETIMAGES,dictionary.UsingValueSetImages);

                foreach( Level level in dictionary.Levels )
                    ddw.SaveLevel(level);

                foreach( Relation relation in dictionary.Relations )
                    ddw.SaveRelation(relation);
            }
        }

        private void SaveSharedComponents(DataDictionaryObject dictObject)
        {
            SaveOption(DataDictionaryElements.DICT_LABEL,dictObject.Label);
            SaveOption(DataDictionaryElements.DICT_NAME,dictObject.Name);
            SaveNote(dictObject.Note);
        }

        private void SaveWrapNote(string note)
        {
            const int WrapLength = 120;

            int lastSpace = -1;

            if( note.Length > WrapLength )
            {
                bool keepSearching = true;

                for( int i = 0; keepSearching && i < note.Length; i++ )
                {
                    if( note[i] == ' ' )
                        lastSpace = i;

                    if( i >= WrapLength && lastSpace >= 0 )
                        keepSearching = false;
                }

                if( lastSpace >= 0 )
                    SaveWrapNote(note.Substring(0,lastSpace));
            }

            SaveOption(DataDictionaryElements.DICT_NOTE,note.Substring(lastSpace + 1));
        }

        private void SaveNote(string note)
        {
            if( !String.IsNullOrWhiteSpace(note) )
            {
                const string MemoryNewLineChars = "\r\n";

                int startPos = 0;
                int endPos;

                while( ( endPos = note.IndexOf(MemoryNewLineChars,startPos) ) >= 0 )
                {
                    SaveWrapNote(note.Substring(startPos,endPos - startPos) + DataDictionaryElements.DICT_NOTENEWLINE);
                    startPos = endPos + MemoryNewLineChars.Length;
                }

                SaveWrapNote(note.Substring(startPos));
            }
        }

        private void SaveOccurrenceLabels(List<string> occurrenceLabels)
        {
            for( int i = 0; i < occurrenceLabels.Count; i++ )
            {
                if( !String.IsNullOrWhiteSpace(occurrenceLabels[i]) )
                    SaveOption(DataDictionaryElements.DICT_OCCLABEL,String.Format("{0},{1}",i + 1,occurrenceLabels[i]));
            }
        }

        private void SaveOption(string argument,string value)
        {
            _tw.WriteLine("{0}={1}",argument,value);
        }

        private void SaveOption(string argument,int value)
        {
            _tw.WriteLine("{0}={1}",argument,value);
        }

        private void SaveOption(string argument,bool value)
        {
            _tw.WriteLine("{0}={1}",argument,value ? DataDictionaryElements.DICT_YES : DataDictionaryElements.DICT_NO);
        }

        private void SaveLevel(Level level)
        {
            _tw.WriteLine();
            _tw.WriteLine(DataDictionaryElements.DICT_LEVEL);
            SaveSharedComponents(level);

            _tw.WriteLine();
            _tw.WriteLine(DataDictionaryElements.DICT_IDITEMS);

            foreach( Item item in level.IDs.Items )
                SaveItem(item);

            foreach( Record record in level.Records )
                SaveRecord(record);
        }

        private void SaveRecord(Record record)
        {
            _tw.WriteLine();

            _tw.WriteLine(DataDictionaryElements.DICT_RECORD);
            SaveSharedComponents(record);

            char quoteChar = record.RecordType.IndexOf('\'') < 0 ? '\'' : '"';
            SaveOption(DataDictionaryElements.RECORD_TYPE,quoteChar + record.RecordType + quoteChar);

            if( !record.Required )
                SaveOption(DataDictionaryElements.RECORD_REQUIRED,record.Required);

            if( record.MaxOccs != 1 )
                SaveOption(DataDictionaryElements.RECORD_MAX,record.MaxOccs);

            SaveOption(DataDictionaryElements.RECORD_LEN,record.Length);

            SaveOccurrenceLabels(record.OccurrenceLabels);

            foreach( Item item in record.Items )
                SaveItem(item);
        }

        private void SaveItem(Item item)
        {
            _tw.WriteLine();
            _tw.WriteLine(DataDictionaryElements.DICT_ITEM);
            SaveSharedComponents(item);

            SaveOption(DataDictionaryElements.ITEM_START,item.Start);
            SaveOption(DataDictionaryElements.ITEM_LEN,item.Length);

            if( item.Subitem )
                SaveOption(DataDictionaryElements.ITEM_ITEMTYPE,DataDictionaryElements.ITEM_SUBITEM);

            if( !item.Numeric )
                SaveOption(DataDictionaryElements.ITEM_DATATYPE,DataDictionaryElements.ITEM_ALPHA);

            if( item.Occurrences != 1 )
                SaveOption(DataDictionaryElements.ITEM_OCCS,item.Occurrences);

            if( item.Decimal > 0 )
                SaveOption(DataDictionaryElements.ITEM_DECIMAL,item.Decimal);

            if( item.DecChar )
                SaveOption(DataDictionaryElements.ITEM_DECCHAR,item.DecChar);

            if( item.ZeroFill )
                SaveOption(DataDictionaryElements.ITEM_ZEROFILL,item.ZeroFill);

            SaveOccurrenceLabels(item.OccurrenceLabels);

            foreach( ValueSet vs in item.ValueSets )
                SaveValueSet(vs);
        }

        private void SaveValueSet(ValueSet vs)
        {
            _tw.WriteLine();
            _tw.WriteLine(DataDictionaryElements.DICT_VALUESET);
            SaveSharedComponents(vs);

            if( vs.LinkID != Int32.MinValue )
            {
                SaveOption(DataDictionaryElements.VALUE_LINK,vs.LinkID);

                if( _vsLinks.Contains(vs.LinkID) ) // the values have already been written
                    return;

                _vsLinks.Add(vs.LinkID);
            }

            foreach( Value value in vs.Values )
                SaveValue(value,vs.Parent);
        }

        private string GetNumericSaveFormat(string stringVal,int numDecimals)
        {
            if( numDecimals == 0 )
                return stringVal;

            string formatString = String.Format("{{0:F{0}}}",numDecimals);
            return String.Format(formatString,Double.Parse(stringVal));
        }

        private void SaveValue(Value value,Item item)
        {
            StringBuilder sb = new StringBuilder();

            for( int i = 0; i < value.Pairs.Count; i++ )
            {
                ValuePair vp = value.Pairs[i];

                // space out multiple pairs
                if( i != 0 )
                    sb.Append(' ');

                // blank values get written out with spaces
                if( String.IsNullOrWhiteSpace(vp.From) )
                    sb.AppendFormat("'{0}'",new String(' ',item.Length));

                else if( item.Alpha )
                {
                    char separator = vp.From.IndexOf('\'') < 0 ? '\'' : '"';
                    sb.AppendFormat(separator + vp.From + separator);
                }

                else // numeric values
                {
                    sb.Append(GetNumericSaveFormat(vp.From,item.Decimal));

                    if( !String.IsNullOrWhiteSpace(vp.To) )
                        sb.AppendFormat(":{0}",GetNumericSaveFormat(vp.To,item.Decimal));
                }
            }

            if( !String.IsNullOrWhiteSpace(value.Label) )
                sb.AppendFormat(";{0}",value.Label);

            SaveOption(DataDictionaryElements.VALUE_VALUE,sb.ToString());

            if( value.SpecialValue != Value.Special.None )
            {
                string specialValueText =
                    value.SpecialValue == Value.Special.Missing ? DataDictionaryElements.VALUE_MISSING :
                    value.SpecialValue == Value.Special.NotApplicable ? DataDictionaryElements.VALUE_NOTAPPL :
                    DataDictionaryElements.VALUE_DEFAULT;

                SaveOption(DataDictionaryElements.DICT_NAME,String.Format("{0},{1}",specialValueText,DataDictionaryElements.VALUE_SPECIAL));
            }

            SaveNote(value.Note);

            if( !String.IsNullOrWhiteSpace(value.ImageFilename) )
                SaveOption(DataDictionaryElements.VALUE_IMAGE,Paths.MakeRelativePath(Path.GetDirectoryName(_filename),value.ImageFilename));
        }

        private void SaveRelation(Relation relation)
        {
            _tw.WriteLine();
            _tw.WriteLine(DataDictionaryElements.DICT_RELATION);

            SaveOption(DataDictionaryElements.DICT_NAME,relation.Name);
            SaveOption(DataDictionaryElements.RELATION_PRIMARY,relation.Primary);
            SaveOption(DataDictionaryElements.RELATION_PRIMARYLINK,relation.PrimaryLink);
            SaveOption(DataDictionaryElements.RELATION_SECONDARY,relation.Secondary);
            SaveOption(DataDictionaryElements.RELATION_SECONDARYLINK,relation.SecondaryLink);
        }

    }
}
