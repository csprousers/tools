using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

// this data dictionary reader does minimal validation checks

namespace CSPro
{
    public class DataDictionaryReader
    {
        private DataDictionary _dictionary = null;
        private Level _level = null;
        private Record _record = null;
        private Item _item = null;
        private Item _lastNonSubitem = null;
        private ValueSet _valueset = null;
        private Value _vsValue = null;

        private string _filename = null;
        private TextReader _tr = null;
        private Dictionary<int,ValueSet> _vsLinks = new Dictionary<int,ValueSet>();

        private DataDictionaryReader()
        {
            // a DataDictionaryReader object can only be created from within the Load function
        }

        private static void ThrowDictionaryException(string line = null)
        {
            string message;

            if( line == null )
                message = Errors.DataDictionary_ReadError;

            else
                message = String.Format(Errors.DataDictionary_ReadErrorLine,line);

            throw new Exception(message);
        }

        private string ReadTrimmedLine()
        {
            string line;

            while( ( line = _tr.ReadLine() ) != null && ( line = line.Trim() ).Length == 0 )
                ;

            return line;
        }

        private static bool IsHeader(string line)
        {
            return line.StartsWith("[") && line.EndsWith("]");
        }

        private static void ParseLine(string line,out string argument,out string value)
        {
            int equalsPos = line.IndexOf('=');

            if( equalsPos < 0 )
                ThrowDictionaryException(line);

            argument = line.Substring(0,equalsPos).Trim();
            value = line.Substring(equalsPos + 1);
        }

        private static string QuotedStringToString(string value)
        {
            if( !value.StartsWith("'") || !value.EndsWith("'") )
                ThrowDictionaryException();

            return value.Substring(1,value.Length - 2);
        }

        private static string AppendNote(string note,string value)
        {
            const string MemoryNewLineChars = "\r\n";

            int newLineIndex = value.IndexOf(DataDictionaryElements.DICT_NOTENEWLINE);

            // add a newline if the \r\n characters are found
            if( newLineIndex >= 0 && newLineIndex == ( value.Length - DataDictionaryElements.DICT_NOTENEWLINE.Length ) )
                value = value.Substring(0,newLineIndex) + MemoryNewLineChars;

            // add a space if this note continues the previous line (because long lines are split) and if the previous line wasn't a newline
            else if( note.Length >= MemoryNewLineChars.Length && !note.Substring(note.Length - MemoryNewLineChars.Length).Equals(MemoryNewLineChars) )
                value = ' ' + value;

            return note + value;
        }

        private static bool AssignSharedComponents(DataDictionaryObject dictObject,string argument,string value)
        {
            if( argument.Equals(DataDictionaryElements.DICT_LABEL,StringComparison.InvariantCultureIgnoreCase) )
            {
                dictObject.Label = value;
                return true;
            }

            else if( argument.Equals(DataDictionaryElements.DICT_NAME,StringComparison.InvariantCultureIgnoreCase) )
            {
                dictObject.Name = value;
                return true;
            }

            else if( argument.Equals(DataDictionaryElements.DICT_NOTE,StringComparison.InvariantCultureIgnoreCase) )
            {
                dictObject.Note = AppendNote(dictObject.Note,value);
                return true;
            }

            return false;
        }

        public static DataDictionary Load(string filename)
        {
            DataDictionaryReader ddr = new DataDictionaryReader();
            ddr._filename = filename;

            using( ddr._tr = IO.CreateStreamReaderFromCSProFile(filename) )
            {
                string line = null;
                string prevLineToHandle = null;

                while( ( prevLineToHandle != null ) || ( ( line = ddr.ReadTrimmedLine() ) != null ) )
                {
                    if( prevLineToHandle != null )
                    {
                        line = prevLineToHandle;
                        prevLineToHandle = null;
                    }

                    if( IsHeader(line) )
                    {
                        // the loading functions return the last line read, which should be either null, a blank string, or a new section

                        if( line.Equals(DataDictionaryElements.DICT_HEADER,StringComparison.InvariantCultureIgnoreCase) )
                            prevLineToHandle = ddr.LoadDictionary();

                        else if( line.Equals(DataDictionaryElements.DICT_LEVEL,StringComparison.InvariantCultureIgnoreCase) )
                            prevLineToHandle = ddr.LoadLevel();

                        else if( line.Equals(DataDictionaryElements.DICT_IDITEMS,StringComparison.InvariantCultureIgnoreCase) )
                            ddr._record = ddr._level.IDs;

                        else if( line.Equals(DataDictionaryElements.DICT_RECORD,StringComparison.InvariantCultureIgnoreCase) )
                            prevLineToHandle = ddr.LoadRecord();

                        else if( line.Equals(DataDictionaryElements.DICT_ITEM,StringComparison.InvariantCultureIgnoreCase) )
                            prevLineToHandle = ddr.LoadItem();

                        else if( line.Equals(DataDictionaryElements.DICT_VALUESET,StringComparison.InvariantCultureIgnoreCase) )
                            prevLineToHandle = ddr.LoadValueSet();

                        else if( line.Equals(DataDictionaryElements.DICT_RELATION,StringComparison.InvariantCultureIgnoreCase) )
                            prevLineToHandle = ddr.LoadRelation();

                        else
                            ThrowDictionaryException(line);
                    }

                    else // an unrecognized command
                        ThrowDictionaryException(line);
                }

                return ddr._dictionary;
            }
        }

        private string LoadDictionary()
        {
            _dictionary = new DataDictionary();

            string line;

            while( ( line = ReadTrimmedLine() ) != null && !IsHeader(line) )
            {
                string argument,value;

                ParseLine(line,out argument,out value);

                if( AssignSharedComponents(_dictionary,argument,value) )
                {
                }

                else if( argument.Equals(DataDictionaryElements.HEADER_VERSION,StringComparison.InvariantCultureIgnoreCase) )
                {
                    if( value.IndexOf(DataDictionaryElements.HEADER_VERSION_CSPRO,StringComparison.InvariantCultureIgnoreCase) != 0 )
                        ThrowDictionaryException(line);

                    _dictionary.Version = Double.Parse(value.Substring(DataDictionaryElements.HEADER_VERSION_CSPRO.Length),CultureInfo.InvariantCulture);
                }

                else if( argument.Equals(DataDictionaryElements.HEADER_RECSTART,StringComparison.InvariantCultureIgnoreCase) )
                    _dictionary.RecTypeStart = Int32.Parse(value,CultureInfo.InvariantCulture);

                else if( argument.Equals(DataDictionaryElements.HEADER_RECLEN,StringComparison.InvariantCultureIgnoreCase) )
                    _dictionary.RecTypeLength = Int32.Parse(value,CultureInfo.InvariantCulture);

                else if( argument.Equals(DataDictionaryElements.HEADER_POSITIONS,StringComparison.InvariantCultureIgnoreCase) )
                    _dictionary.RelativePositioning = value.Equals(DataDictionaryElements.HEADER_RELATIVE,StringComparison.InvariantCultureIgnoreCase);

                else if( argument.Equals(DataDictionaryElements.HEADER_ZEROFILL,StringComparison.InvariantCultureIgnoreCase) )
                    _dictionary.ZeroFillDefault = value.Equals(DataDictionaryElements.DICT_YES,StringComparison.InvariantCultureIgnoreCase);

                else if( argument.Equals(DataDictionaryElements.HEADER_DECCHAR,StringComparison.InvariantCultureIgnoreCase) )
                    _dictionary.DecimalCharDefault = value.Equals(DataDictionaryElements.DICT_YES,StringComparison.InvariantCultureIgnoreCase);

                else if( argument.Equals(DataDictionaryElements.HEADER_VALUESETIMAGES,StringComparison.InvariantCultureIgnoreCase) )
                    _dictionary.UsingValueSetImages = value.Equals(DataDictionaryElements.DICT_YES,StringComparison.InvariantCultureIgnoreCase);

                else
                    ThrowDictionaryException(line);
            }

            return line;
        }

        private string LoadLevel()
        {
            int newLevelNum = _level == null ? 1 : _level.LevelNum + 1;

            _level = new Level(_dictionary,newLevelNum);

            string line;

            while( ( line = ReadTrimmedLine() ) != null && !IsHeader(line) )
            {
                string argument,value;

                ParseLine(line,out argument,out value);

                if( AssignSharedComponents(_level,argument,value) )
                {
                }

                else
                    ThrowDictionaryException(line);
            }

            _dictionary.AddLevel(_level);

            return line;
        }

        private string LoadRecord()
        {
            _record = new Record(_level);

            string line;

            while( ( line = ReadTrimmedLine() ) != null && !IsHeader(line) )
            {
                string argument,value;

                ParseLine(line,out argument,out value);

                if( AssignSharedComponents(_record,argument,value) )
                {
                }

                else if( argument.Equals(DataDictionaryElements.RECORD_TYPE,StringComparison.InvariantCultureIgnoreCase) )
                    _record.RecordType = QuotedStringToString(value);

                else if( argument.Equals(DataDictionaryElements.RECORD_REQUIRED,StringComparison.InvariantCultureIgnoreCase) )
                    _record.Required = value.Equals(DataDictionaryElements.DICT_YES,StringComparison.InvariantCultureIgnoreCase);

                else if( argument.Equals(DataDictionaryElements.RECORD_MAX,StringComparison.InvariantCultureIgnoreCase) )
                    _record.MaxOccs = Int32.Parse(value,CultureInfo.InvariantCulture);

                else if( argument.Equals(DataDictionaryElements.RECORD_LEN,StringComparison.InvariantCultureIgnoreCase) )
                    _record.Length = Int32.Parse(value,CultureInfo.InvariantCulture);

                else if( argument.Equals(DataDictionaryElements.DICT_OCCLABEL,StringComparison.InvariantCultureIgnoreCase) )
                    ProcessOccurrenceLabel(_record.OccurrenceLabels,_record.MaxOccs,value);

                else
                    ThrowDictionaryException(line);
            }

            _level.AddRecord(_record);

            return line;
        }

        private string LoadItem()
        {
            _item = new Item(_record);

            string line;

            while( ( line = ReadTrimmedLine() ) != null && !IsHeader(line) )
            {
                if( IsHeader(line) )
                    return line;

                string argument,value;

                ParseLine(line,out argument,out value);

                if( AssignSharedComponents(_item,argument,value) )
                {
                }

                else if( argument.Equals(DataDictionaryElements.ITEM_START,StringComparison.InvariantCultureIgnoreCase) )
                    _item.Start = Int32.Parse(value,CultureInfo.InvariantCulture);

                else if( argument.Equals(DataDictionaryElements.ITEM_LEN,StringComparison.InvariantCultureIgnoreCase) )
                    _item.Length = Int32.Parse(value,CultureInfo.InvariantCulture);

                else if( argument.Equals(DataDictionaryElements.ITEM_DATATYPE,StringComparison.InvariantCultureIgnoreCase) )
                    _item.Numeric = !value.Equals(DataDictionaryElements.ITEM_ALPHA,StringComparison.InvariantCultureIgnoreCase);

                else if( argument.Equals(DataDictionaryElements.ITEM_ITEMTYPE,StringComparison.InvariantCultureIgnoreCase) )
                {
                    _item.Subitem = value.Equals(DataDictionaryElements.ITEM_SUBITEM,StringComparison.InvariantCultureIgnoreCase);

                    if( _item.Subitem )
                        _lastNonSubitem.Subitems.Add(_item);
                }

                else if( argument.Equals(DataDictionaryElements.ITEM_OCCS,StringComparison.InvariantCultureIgnoreCase) )
                    _item.Occurrences = Int32.Parse(value,CultureInfo.InvariantCulture);

                else if( argument.Equals(DataDictionaryElements.ITEM_DECIMAL,StringComparison.InvariantCultureIgnoreCase) )
                    _item.Decimal = Int32.Parse(value,CultureInfo.InvariantCulture);

                else if( argument.Equals(DataDictionaryElements.ITEM_DECCHAR,StringComparison.InvariantCultureIgnoreCase) )
                    _item.DecChar = value.Equals(DataDictionaryElements.DICT_YES,StringComparison.InvariantCultureIgnoreCase);

                else if( argument.Equals(DataDictionaryElements.ITEM_ZEROFILL,StringComparison.InvariantCultureIgnoreCase) )
                    _item.ZeroFill = value.Equals(DataDictionaryElements.DICT_YES,StringComparison.InvariantCultureIgnoreCase);

                else if( argument.Equals(DataDictionaryElements.DICT_OCCLABEL,StringComparison.InvariantCultureIgnoreCase) )
                    ProcessOccurrenceLabel(_item.OccurrenceLabels,_item.Occurrences,value);

                else
                    ThrowDictionaryException(line);
            }

            if( !_item.Subitem )
                _lastNonSubitem = _item;

            _record.AddItem(_item);

            return line;
        }

        private string LoadValueSet()
        {
            _valueset = new ValueSet(_item);
            _vsValue = null;

            string line;
            bool firstName = true;

            while( ( line = ReadTrimmedLine() ) != null && !IsHeader(line) )
            {
                string argument,value;

                ParseLine(line,out argument,out value);

                // special parsing for the name
                if( argument.Equals(DataDictionaryElements.DICT_NAME,StringComparison.InvariantCultureIgnoreCase) )
                {
                    if( firstName )
                    {
                        _valueset.Name = value;
                        firstName = false;
                    }

                    else
                    {
                        int commaPos = value.IndexOf(',');

                        if( commaPos < 0 )
                            ThrowDictionaryException(line);

                        string specialVal,description;

                        specialVal = value.Substring(0,commaPos);
                        description = value.Substring(commaPos + 1);

                        if( !description.Equals(DataDictionaryElements.VALUE_SPECIAL,StringComparison.InvariantCultureIgnoreCase) )
                            ThrowDictionaryException(line);

                        else if( specialVal.Equals(DataDictionaryElements.VALUE_MISSING,StringComparison.InvariantCultureIgnoreCase) )
                            _vsValue.SpecialValue = Value.Special.Missing;

                        else if( specialVal.Equals(DataDictionaryElements.VALUE_NOTAPPL,StringComparison.InvariantCultureIgnoreCase) )
                            _vsValue.SpecialValue = Value.Special.NotApplicable;

                        else if( specialVal.Equals(DataDictionaryElements.VALUE_DEFAULT,StringComparison.InvariantCultureIgnoreCase) )
                            _vsValue.SpecialValue = Value.Special.Default;

                        else
                            ThrowDictionaryException(line);
                    }
                }

                // a value (not value set) note
                else if( _vsValue != null && argument.Equals(DataDictionaryElements.DICT_NOTE,StringComparison.InvariantCultureIgnoreCase) )
                    _vsValue.Note = AppendNote(_vsValue.Note,value);

                else if( AssignSharedComponents(_valueset,argument,value) ) // the value set label and note will be handled here
                {
                }

                else if( argument.Equals(DataDictionaryElements.VALUE_LINK,StringComparison.InvariantCultureIgnoreCase) )
                {
                    int linkID = Int32.Parse(value,CultureInfo.InvariantCulture);

                    if( _vsLinks.ContainsKey(linkID) )
                        _valueset.EstablishValueSetLink(_vsLinks[linkID],linkID);

                    else
                    {
                        _valueset.LinkID = linkID;
                        _vsLinks.Add(linkID,_valueset);
                    }
                }

                else if( argument.Equals(DataDictionaryElements.VALUE_VALUE,StringComparison.InvariantCultureIgnoreCase) )
                {
                    _vsValue = new Value();
                    AddValues(value);
                    _valueset.AddValue(_vsValue);
                }

                else if( argument.Equals(DataDictionaryElements.VALUE_IMAGE,StringComparison.InvariantCultureIgnoreCase) )
                    _vsValue.ImageFilename = Paths.MakeAbsolutePath(Path.GetDirectoryName(_filename),value);

                else
                    ThrowDictionaryException(line);
            }

            _item.AddValueSet(_valueset);

            return line;
        }

        private void AddValues(string line)
        {
            line = line.Trim();

            if( line.Length == 0 )
                return;

            ValuePair vp = new ValuePair();

            int semicolonPos = line.IndexOf(';');

            if( semicolonPos == 0 )
            {
                _vsValue.Label = line.Substring(1);
                return;
            }

            else if( line.IndexOf('\'') == 0 )
            {
                int nextPos = line.IndexOf('\'',1);

                if( nextPos >= 1 )
                {
                    vp.From = line.Substring(1,nextPos - 1);
                    _vsValue.AddValuePair(vp);
                    AddValues(line.Substring(nextPos + 1));
                    return;
                }

                else
                    ThrowDictionaryException(line);
            }

            else if( line.IndexOf('"') == 0 )
            {
                int nextPos = line.IndexOf('"',1);

                if( nextPos >= 1 )
                {
                    vp.From = line.Substring(1,nextPos - 1);
                    _vsValue.AddValuePair(vp);
                    AddValues(line.Substring(nextPos + 1));
                    return;
                }

                else
                    ThrowDictionaryException(line);
            }

            string thisValue;
            int spacePos = line.IndexOf(' ');

            if( spacePos > 0 && ( semicolonPos < 0 || spacePos < semicolonPos ) )
                thisValue = line.Substring(0,spacePos);

            else if( semicolonPos >= 0 )
                thisValue = line.Substring(0,semicolonPos);

            else
                thisValue = line;

            int colonPos = line.IndexOf(':');

            if( colonPos >= 0 && ( spacePos < 0 || colonPos < spacePos ) && ( semicolonPos < 0 || colonPos < semicolonPos ) )
            {
                vp.From = Double.Parse(thisValue.Substring(0,colonPos),CultureInfo.InvariantCulture).ToString();
                vp.To = Double.Parse(thisValue.Substring(colonPos + 1),CultureInfo.InvariantCulture).ToString();
            }

            else
                vp.From = Double.Parse(thisValue,CultureInfo.InvariantCulture).ToString();

            _vsValue.AddValuePair(vp);

            if( spacePos > 0 && ( semicolonPos < 0 || spacePos < semicolonPos ) )
                AddValues(line.Substring(spacePos + 1));

            else if( semicolonPos >= 0 )
                AddValues(line.Substring(semicolonPos));

            else
                _vsValue.Label = "";
        }

        private string LoadRelation()
        {
            Relation relation = new Relation();

            string line;

            while( ( line = ReadTrimmedLine() ) != null && !IsHeader(line) )
            {
                string argument,value;

                ParseLine(line,out argument,out value);

                if( argument.Equals(DataDictionaryElements.DICT_NAME,StringComparison.InvariantCultureIgnoreCase) )
                    relation.Name = value;

                else if( argument.Equals(DataDictionaryElements.RELATION_PRIMARY,StringComparison.InvariantCultureIgnoreCase) )
                    relation.Primary = value;

                else if( argument.Equals(DataDictionaryElements.RELATION_PRIMARYLINK,StringComparison.InvariantCultureIgnoreCase) )
                    relation.PrimaryLink = value;

                else if( argument.Equals(DataDictionaryElements.RELATION_SECONDARY,StringComparison.InvariantCultureIgnoreCase) )
                    relation.Secondary = value;

                else if( argument.Equals(DataDictionaryElements.RELATION_SECONDARYLINK,StringComparison.InvariantCultureIgnoreCase) )
                    relation.SecondaryLink = value;

                else
                    ThrowDictionaryException(line);
            }

            _dictionary.Relations.Add(relation);

            return line;
        }

        private void ProcessOccurrenceLabel(List<string> occurrenceLabels,int maxOccs,string line)
        {
            int commaPos = line.IndexOf(',');

            if( commaPos < 0 )
                ThrowDictionaryException(line);

            int occ = Int32.Parse(line.Substring(0,commaPos),CultureInfo.InvariantCulture);

            if( occ <= maxOccs )
            {
                string label = line.Substring(commaPos + 1);

                // insert blank occurrence labels into unfilled occurrences
                for( int i = occurrenceLabels.Count; i < occ; i++ )
                    occurrenceLabels.Add("");

                occurrenceLabels[occ - 1] = label;
            }
        }

    }

}
