using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using IPUMS2CSPro;

// this contains a bare bones class, Codebook, that reads the aspects of an IPUMS codebook needed to create a CSPro dictionary

namespace IPUMS
{
    public enum FileType { Rectangular, Hierarchical, Household };

    public class Variable
    {
        public string Name { get; set; }
        public int StartPosition { get; set; }
        public int Length { get; set; }

        public bool RecordType { get; set; }
        public bool ID { get; set; }
    };

    public class Value
    {
        public string Code { get; set; }
        public string Label { get; set; }
    }

    public class ValueSet
    {
        private List<Value> _values;

        public string Name { get; set; }
        public string Label  { get; set; }
        public List<Value> Values { get { return _values; } }

        public ValueSet()
        {
            _values = new List<Value>();
        }
    }

    public class Codebook
    {
        private Dictionary<string,List<Variable>> _records;
        private Dictionary<string,ValueSet> _valuesets;

        public string Description { get; set; }
        public FileType FileType { get; set; }
        public bool HasCaseSelection  { get; set; }
        public Dictionary<string,List<Variable>> Records { get { return _records; } }
        public Dictionary<string,ValueSet> ValueSets { get { return _valuesets; } }

        public Codebook(string filename)
        {
            string[] lines = File.ReadAllLines(filename,Encoding.GetEncoding(1252));

            int lineCtr = 0;

            ReadDescription(lines,ref lineCtr);
            ReadUpToFileType(lines,ref lineCtr);
            ReadFileType(lines,ref lineCtr);
            ReadCaseSelectionHeading(lines,ref lineCtr);

            _records = new Dictionary<string,List<Variable>>();
            ReadVariables(lines,ref lineCtr);
            ProcessVariables();

            ReadPostVariables(lines,ref lineCtr);

            if( HasCaseSelection )
                ReadCaseSelections(lines,ref lineCtr);

            _valuesets = new Dictionary<string,ValueSet>();
            ReadValueSets(lines,ref lineCtr);
        }

        void SkipBlankLines(string[] lines,ref int lineCtr,bool checkEOF = true)
        {
            while( lineCtr < lines.Length && String.IsNullOrWhiteSpace(lines[lineCtr]) )
                lineCtr++;

            if( checkEOF && lineCtr == lines.Length )
                throw new Exception(Messages.UnexpectedEOF);
        }

        string ReadColonSeparatedValue(string line)
        {
            int colonPos = line.IndexOf(':');

            if( colonPos < 0 )
                throw new Exception(String.Format(Messages.MissingColon,line));

            return line.Substring(colonPos + 1).Trim();
        }

        void ReadDescription(string[] lines,ref int lineCtr)
        {
            SkipBlankLines(lines,ref lineCtr);

            if( lines[lineCtr].IndexOf("Description:",StringComparison.InvariantCultureIgnoreCase) != 0 )
                throw new Exception(Messages.MissingDescription);

            Description = ReadColonSeparatedValue(lines[lineCtr]);

            lineCtr++;
        }

        // for now, ignore the description and samples selected, so read up to the file type
        void ReadUpToFileType(string[] lines,ref int lineCtr)
        {
            for( ; lineCtr < lines.Length; lineCtr++ )
            {
                if( lines[lineCtr].IndexOf("File Type:",StringComparison.InvariantCultureIgnoreCase) == 0 )
                    return;
            }

            throw new Exception(Messages.MissingFileType);
        }

        void ReadFileType(string[] lines,ref int lineCtr)
        {
            string type = ReadColonSeparatedValue(lines[lineCtr]);

            if( type.Equals("rectangular",StringComparison.InvariantCultureIgnoreCase) )
                FileType = FileType.Rectangular;

            else if( type.Equals("hierarchical",StringComparison.InvariantCultureIgnoreCase) )
                FileType = FileType.Hierarchical;

            else if( type.Equals("household",StringComparison.InvariantCultureIgnoreCase) )
                FileType = FileType.Household;

            else
                throw new Exception(String.Format(Messages.InvalidFileType,type));

            lineCtr++;
        }

        void ReadCaseSelectionHeading(string[] lines,ref int lineCtr)
        {
            SkipBlankLines(lines,ref lineCtr);

            if( lines[lineCtr].IndexOf("Case Selection:",StringComparison.InvariantCultureIgnoreCase) != 0 )
                throw new Exception(Messages.MissingCaseSelection);

            HasCaseSelection = ReadColonSeparatedValue(lines[lineCtr]).Equals("yes",StringComparison.InvariantCultureIgnoreCase);

            lineCtr++;
        }

        void ReadVariables(string[] lines,ref int lineCtr)
        {
            SkipBlankLines(lines,ref lineCtr);

            if( lines[lineCtr].IndexOf("Variable",StringComparison.InvariantCultureIgnoreCase) < 0 ||
                 lines[lineCtr].IndexOf("Columns",StringComparison.InvariantCultureIgnoreCase) < 0 ||
                 lines[lineCtr].IndexOf("Len",StringComparison.InvariantCultureIgnoreCase) < 0 )
                throw new Exception(Messages.MissingVariablesDeclaration);

            lineCtr++;

            while( lineCtr < lines.Length && !String.IsNullOrWhiteSpace(lines[lineCtr]) )
            {
                string line = lines[lineCtr];
                int nonSpacePos = line.TakeWhile(c => char.IsWhiteSpace(c)).Count();
                int spacePos = line.Substring(nonSpacePos + 1).TakeWhile(c => !char.IsWhiteSpace(c)).Count();
                string variableName = line.Substring(nonSpacePos,spacePos + 1);

                line = line.Substring(nonSpacePos + spacePos + 1);
                nonSpacePos = line.TakeWhile(c => char.IsWhiteSpace(c)).Count();
                spacePos = line.Substring(nonSpacePos + 1).TakeWhile(c => !char.IsWhiteSpace(c)).Count();
                string recordType = line.Substring(nonSpacePos,spacePos + 1);

                line = line.Substring(nonSpacePos + spacePos + 1);
                nonSpacePos = line.TakeWhile(c => char.IsWhiteSpace(c)).Count();
                spacePos = line.Substring(nonSpacePos + 1).TakeWhile(c => !char.IsWhiteSpace(c)).Count();
                string columns = line.Substring(nonSpacePos,spacePos + 1);

                line = line.Substring(nonSpacePos + spacePos + 1);
                nonSpacePos = line.TakeWhile(c => char.IsWhiteSpace(c)).Count();
                spacePos = line.Substring(nonSpacePos + 1).TakeWhile(c => !char.IsWhiteSpace(c)).Count();
                string len = line.Substring(nonSpacePos,spacePos + 1);

                // ignore the second value of the columns string
                int dashPos = columns.IndexOf("-");

                if( dashPos > 0 )
                    columns = columns.Substring(0,dashPos);

                Variable variable = new Variable();
                variable.Name = variableName;
                variable.StartPosition = Int32.Parse(columns,CultureInfo.InvariantCulture);
                variable.Length = Int32.Parse(len,CultureInfo.InvariantCulture);

                if( !Records.ContainsKey(recordType) )
                    Records.Add(recordType,new List<Variable>());

                Records[recordType].Add(variable);

                lineCtr++;
            }
        }

        void ProcessVariables()
        {
            for( int i = 0; i < Records.Count; i++ )
            {
                List<Variable> record = Records.ElementAt(i).Value;

                bool processedPastIds = false;

                if( FileType == FileType.Rectangular && i > 0 )
                    processedPastIds = true;

                for( int j = 0; j < record.Count; j++ )
                {
                    Variable variable = record[j];

                    if( variable.Name.IndexOf("rectype",StringComparison.InvariantCultureIgnoreCase) == 0 )
                        variable.RecordType = true;

                    else if( !processedPastIds ) // check if the item is an ID
                    {
                        variable.ID = true;

                        if( variable.Name.IndexOf("serial",StringComparison.InvariantCultureIgnoreCase) == 0 )
                            processedPastIds = true;
                    }

                    record[j] = variable;
                }
            }
        }

        void ReadPostVariables(string[] lines,ref int lineCtr)
        {
            while( lineCtr < lines.Length )
            {
                if( lines[lineCtr++].IndexOf("All Years .",StringComparison.InvariantCultureIgnoreCase) == 0 )
                    return;
            }

            throw new Exception(Messages.InvalidVariableAvailability);
        }

        void ReadCaseSelections(string[] lines,ref int lineCtr)
        {
            SkipBlankLines(lines,ref lineCtr,false);

            // these are not needed to create the CSPro dictionary
            while( lineCtr < lines.Length && !String.IsNullOrWhiteSpace(lines[lineCtr]) )
                lineCtr++;
        }

        void ReadValueSets(string[] lines,ref int lineCtr)
        {
            ValueSet thisValueSet = null;

            SkipBlankLines(lines,ref lineCtr,false);

            while( lineCtr < lines.Length )
            {
                string line = lines[lineCtr];

                int tabPos = line.IndexOf('\t');

                if( tabPos != 0 ) // (when tabPos == 0 we will ignore the descriptive labels of a series of value labels)
                {
                    string leftWord = line.Substring(0,tabPos).Trim();
                    string rightWord = line.Substring(tabPos + 1).Trim();

                    if( line[0] == ' ' ) // this is a new value set
                    {
                        thisValueSet = new ValueSet();
                        thisValueSet.Name = leftWord;
                        thisValueSet.Label = rightWord;

                        ValueSets.Add(thisValueSet.Name,thisValueSet);
                    }

                    else // a value label
                    {
                        Value value = new Value();
                        value.Code = leftWord;
                        value.Label = rightWord;
                        thisValueSet.Values.Add(value);
                    }
                }

                lineCtr++;

                SkipBlankLines(lines,ref lineCtr,false);
            }
        }
    }
}
