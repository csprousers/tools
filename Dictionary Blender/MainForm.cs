using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using CSPro;

namespace Dictionary_Blender
{
    public partial class MainForm : Form
    {
        // the name of a dictionary -> the dictionary object
        private Dictionary<string,DataDictionary> _dictionaries = new Dictionary<string,DataDictionary>();

        // the name of a dictionary -> a dictionary containing all of the symbols in the dictionary -> the objects themselves
        private Dictionary<string,Dictionary<string,object>> _dictionarySymbols = new Dictionary<string,Dictionary<string,object>>();

        // the name of a symbol -> the dictionary object(s) containing the symbol
        private Dictionary<string,List<DataDictionary>> _symbolParents;

        private Dictionary<string,BlenderFunction> _functions = new Dictionary<string,BlenderFunction>();

        private void InitializeFunctions()
        {
            _functions.Add("EXIT",new BlenderFunction(ProcessExit,0));
            _functions.Add("OPEN",new BlenderFunction(ProcessOpen,1));
            _functions.Add("CLOSE",new BlenderFunction(ProcessClose,1));
            _functions.Add("SAVE",new BlenderFunction(ProcessSave,2));
            _functions.Add("LABEL",new BlenderFunction(ProcessLabel,2));
            _functions.Add("REMOVE",new BlenderFunction(ProcessRemove,1));
            _functions.Add("RENAME",new BlenderFunction(ProcessRename,2));
            _functions.Add("FLATTEN",new BlenderFunction(ProcessFlatten,1,2));
            _functions.Add("RUN",new BlenderFunction(ProcessRun,1));
            _functions.Add("UNSUBITEMIZE",new BlenderFunction(ProcessUnsubitemize,1));
            _functions.Add("UNCHECKBOXIZE",new BlenderFunction(ProcessUncheckboxize,1));
        }

        public MainForm()
        {
            InitializeComponent();

            InitializeFunctions();
            RefreshSymbolParents();
        }

        private void MainForm_Shown(object sender,EventArgs e)
        {
            Array commandArgs = Environment.GetCommandLineArgs();

            if( commandArgs.Length >= 2 )
                RunScript((string)commandArgs.GetValue(1));
        }

        private void buttonEnter_Click(object sender,EventArgs e)
        {
            labelCommand.Text = textBoxCommand.Text;
            ProcessCommand(textBoxCommand.Text);
        }

        private void SetOutputError(string message)
        {
            textBoxOutput.Text = message;
            textBoxOutput.ForeColor = Color.Red;
        }

        private void SetOutputSuccess(string message)
        {
            textBoxOutput.Text = message;
            textBoxOutput.ForeColor = Color.Black;
            textBoxCommand.Text = ""; // if successful, clear the command
        }

        private bool ProcessCommand(string command)
        {
            try
            {
                command = command.Trim();

                if( command.Length == 0 )
                    throw new Exception(Messages.ProcessorNoCommand);

                // regular expression from http://stackoverflow.com/questions/14655023/split-a-string-that-has-white-spaces-unless-they-are-enclosed-within-quotes
                string[] commands = Regex.Split(command,"(?<=^[^\"]*(?:\"[^\"]*\"[^\"]*)*) (?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");

                // remove the quotes from any quoted strings
                for( int i = 0; i < commands.Length; i++ )
                {
                    if( commands[i].Length >= 2 && commands[i][0] == '"' && commands[i][commands[i].Length - 1] == '"' )
                        commands[i] = commands[i].Substring(1,commands[i].Length - 2);
                }

                string verb = commands[0].ToUpper();

                if( _functions.ContainsKey(verb) )
                {
                    BlenderFunction function = _functions[verb];

                    int numberArguments = commands.Length - 1;

                    if( numberArguments < function.MinArguments || numberArguments > function.MaxArguments )
                    {
                        if( function.MinArguments == function.MaxArguments )
                            throw new Exception(String.Format(Messages.ProcessorInvalidNumberArguments,verb,function.MinArguments));

                        else
                            throw new Exception(String.Format(Messages.ProcessorInvalidNumberArgumentsRange,verb,function.MinArguments,function.MaxArguments));
                    }

                    function.Implementation(commands);
                }

                else
                    throw new Exception(Messages.ProcessorUnrecognizedCommand);
            }

            catch( Exception exception )
            {
                SetOutputError(exception.Message);
                return false;
            }

            return true;
        }


        private DataDictionary GetDataDictionaryFromName(string name)
        {
            name = name.ToUpper();

            if( _dictionaries.ContainsKey(name) )
                return _dictionaries[name];

            else
                throw new Exception(String.Format(Messages.DictionaryNotFound,name));
        }


        private object GetSymbolFromName(string name)
        {
            name = name.ToUpper();

            // first check the dictionary names
            if( _dictionaries.ContainsKey(name) )
                return _dictionaries[name];

            // handle dot notation
            string symbolName = name;
            string containingDictionaryName = null;
            int dotPos = name.IndexOf('.');            

            if( dotPos >= 0 )
            {
                containingDictionaryName = name.Substring(0,dotPos);
                symbolName = name.Substring(dotPos + 1);

                if( !_dictionaries.ContainsKey(containingDictionaryName) )
                    throw new Exception(String.Format(Messages.DictionaryNotFound,containingDictionaryName));
            }

            if( !_symbolParents.ContainsKey(symbolName) )
                throw new Exception(String.Format(Messages.SymbolNotFound,symbolName));
            
            if( containingDictionaryName == null ) // did not use dot notation
            {
                List<DataDictionary> possibleDictionaries = _symbolParents[name];

                if( possibleDictionaries.Count > 1 )
                    throw new Exception(String.Format(Messages.SymbolAmbiguous,symbolName));

                containingDictionaryName = possibleDictionaries[0].Name;
            }

            else
            {
                if( !_dictionarySymbols[containingDictionaryName].ContainsKey(symbolName) )
                    throw new Exception(String.Format(Messages.SymbolNotFoundInDictionary,symbolName,containingDictionaryName));
            }

            return _dictionarySymbols[containingDictionaryName][symbolName];
        }


        private class SymbolTypes
        {
            public const int DataDictionary = 0x01;
            public const int Level = 0x02;
            public const int Record = 0x04;
            public const int Item = 0x08;
            public const int ValueSet = 0x10;
            public const int AllSupported = DataDictionary | Level | Record | Item | ValueSet;
        }

        private DataDictionaryObject GetSymbolFromName(string name,int allowedTypes)
        {
            object symbol = GetSymbolFromName(name);

            if( ( symbol is DataDictionary && ( allowedTypes & SymbolTypes.DataDictionary ) != 0 ) ||
                ( symbol is Level && ( allowedTypes & SymbolTypes.Level ) != 0 ) ||
                ( symbol is Record && ( allowedTypes & SymbolTypes.Record ) != 0 ) ||
                ( symbol is Item && ( allowedTypes & SymbolTypes.Item ) != 0 ) ||
                ( symbol is ValueSet && ( allowedTypes & SymbolTypes.ValueSet ) != 0 ) )
            {
                return (DataDictionaryObject)symbol;
            }

            throw new Exception(String.Format(Messages.SymbolNotAllowedType,name.ToUpper()));
        }

        private DataDictionary GetDataDictionaryFromSymbol(DataDictionaryObject symbol)
        {
            return symbol is DataDictionary ? (DataDictionary)symbol :
                symbol is Level ? ( (Level)symbol ).ParentDictionary :
                symbol is Record ? ( (Record)symbol ).ParentDictionary :
                symbol is Item ? ( (Item)symbol ).ParentDictionary :
                symbol is ValueSet ? ( (ValueSet)symbol ).ParentDictionary :
                null;
        }


        // EXIT -- no parameters
        private void ProcessExit(string[] commands)
        {
            Close();
        }


        private void AddSymbol(Dictionary<string,object> symbols,string name,object symbol)
        {
            if( _dictionaries.ContainsKey(name) )
                throw new Exception(String.Format(Messages.SymbolNameInUse,name));

            symbols.Add(name,symbol);
        }

        private void LoadRecordSymbols(Dictionary<string,object> symbols,Record record)
        {
            AddSymbol(symbols,record.Name,record);

            foreach( Item item in record.Items )
            {
                AddSymbol(symbols,item.Name,item);

                foreach( ValueSet vs in item.ValueSets )
                    AddSymbol(symbols,vs.Name,vs);
            }
        }

        private void LoadDictionarySymbols(DataDictionary dictionary)
        {
            Dictionary<string,object> symbols = new Dictionary<string,object>();

            foreach( Level level in dictionary.Levels )
            {
                AddSymbol(symbols,level.Name,level);

                LoadRecordSymbols(symbols,level.IDs);

                foreach( Record record in level.Records )
                    LoadRecordSymbols(symbols,record);
            }

            foreach( Relation relation in dictionary.Relations )
                AddSymbol(symbols,relation.Name,relation);
            
            _dictionarySymbols[dictionary.Name] = symbols;
        }


        private void RefreshSymbolParents()
        {
            _symbolParents = new Dictionary<string,List<DataDictionary>>();

            foreach( var kp in _dictionarySymbols )
            {
                DataDictionary dictionary = GetDataDictionaryFromName(kp.Key);

                foreach( string name in kp.Value.Keys )
                {
                    if( !_symbolParents.ContainsKey(name) )
                        _symbolParents.Add(name,new List<DataDictionary>());

                    _symbolParents[name].Add(dictionary);
                }
            }
        }


        // OPEN -- dictionary-filename
        private void ProcessOpen(string[] commands)
        {
            string filename = new FileInfo(commands[1]).FullName;

            if( !File.Exists(filename) )
                throw new Exception(String.Format(Messages.FileNotExist,filename));

            DataDictionary dictionary = DataDictionaryReader.Load(filename); // Load can throw an exception

            if( dictionary.Levels.Count != 1 )
                throw new Exception(Messages.DictionaryNotOneLevel);

            if( dictionary.Relations.Count > 0 )
                throw new Exception(Messages.DictionaryHasRelations);

            if( _dictionaries.ContainsKey(dictionary.Name) )
                throw new Exception(String.Format(Messages.DictionaryAlreadyLoaded,dictionary.Name));

            // make sure that the dictionary name doesn't already exist as a name of an already loaded object
            if( _symbolParents.ContainsKey(dictionary.Name) )
                throw new Exception(String.Format(Messages.DictionaryNameInUse,dictionary.Name,_symbolParents[dictionary.Name][0].Name));

            LoadDictionarySymbols(dictionary);

            _dictionaries.Add(dictionary.Name,dictionary);

            RefreshSymbolParents();

            SetOutputSuccess(String.Format(Messages.DictionaryLoaded,dictionary.Name));
        }


        // CLOSE -- dictionary-name
        private void ProcessClose(string[] commands)
        {
            DataDictionary dictionary = GetDataDictionaryFromName(commands[1]);

            _dictionaries.Remove(dictionary.Name);
            _dictionarySymbols.Remove(dictionary.Name);

            RefreshSymbolParents();

            SetOutputSuccess(String.Format(Messages.DictionaryClosed,dictionary.Name));
        }


        // SAVE -- dictionary-name dictionary-filename
        private void ProcessSave(string[] commands)
        {
            DataDictionary dictionary = GetDataDictionaryFromName(commands[1]);
            string filename = new FileInfo(commands[2]).FullName;

            const string DictionaryExtension = ".dcf";

            if( !Path.GetExtension(filename).Equals(DictionaryExtension,StringComparison.InvariantCultureIgnoreCase) )
                filename = filename + DictionaryExtension;

            DataDictionaryWriter.Save(dictionary,filename); // Save can throw an exception

            SetOutputSuccess(String.Format(Messages.DictionarySaved,dictionary.Name,filename));
        }


        // LABEL -- symbol-name label
        private void ProcessLabel(string[] commands)
        {
            DataDictionaryObject symbol = GetSymbolFromName(commands[1],SymbolTypes.AllSupported);
            symbol.Label = commands[2];
            SetOutputSuccess(String.Format(Messages.LabelModified,symbol.Name,commands[2]));
        }


        // REMOVE -- symbol-name
        private void ProcessRemove(string[] commands)
        {
            DataDictionaryObject symbol = GetSymbolFromName(commands[1],SymbolTypes.Record | SymbolTypes.Item | SymbolTypes.ValueSet);
            DataDictionary dictionary = null;

            if( symbol is Record )
            {
                Record record = (Record)symbol;
                Level level = record.ParentLevel;
                dictionary = record.ParentDictionary;

                if( level.IDs == record )
                    throw new Exception(Messages.RemoveIDsDisabled);

                level.Records.Remove(record);

                SetOutputSuccess(String.Format(Messages.RemoveRecord,record.Name,record.Items.Count));
            }

            else if( symbol is Item )
            {
                Item item = (Item)symbol;
                Record record = item.ParentRecord;
                dictionary = item.ParentDictionary;

                record.Items.Remove(item);

                // delete any subitems associated with the item
                foreach( Item subitem in item.Subitems )
                    record.Items.Remove(subitem);

                DataDictionaryWorker.CalculateItemPositions(dictionary);
                DataDictionaryWorker.CalculateRecordLengths(dictionary);
                DataDictionaryWorker.RemoveBrokenValueSetLinks(dictionary);

                SetOutputSuccess(String.Format(Messages.RemoveItem,item.Name,record.Name));
            }

            else // ValueSet
            {
                ValueSet vs = (ValueSet)symbol;
                Item item = vs.ParentItem;
                dictionary = vs.ParentDictionary;

                item.ValueSets.Remove(vs);
                
                DataDictionaryWorker.RemoveBrokenValueSetLinks(dictionary);

                SetOutputSuccess(String.Format(Messages.RemoveValueSet,vs.Name,item.Name));
            }

            LoadDictionarySymbols(dictionary);
            RefreshSymbolParents();
        }


        public void CheckIfValidNewName(DataDictionary dictionary,string newName)
        {
            if( !Names.IsValid(newName) )
                throw new Exception(String.Format(Messages.NameCheckInvalid,newName));

            // check that a dictionary with the same name has not been loaded
            if( _dictionaries.ContainsKey(newName) )
                throw new Exception(String.Format(Messages.NameCheckUsedAsDictionary,newName));

            if( _dictionarySymbols[dictionary.Name].ContainsKey(newName) )
                throw new Exception(String.Format(Messages.NameCheckUsedWithinDictionary,newName,dictionary.Name));
        }


        // RENAME -- symbol-name new-symbol-name
        private void ProcessRename(string[] commands)
        {
            DataDictionaryObject symbol = GetSymbolFromName(commands[1],SymbolTypes.AllSupported);
            DataDictionary dictionary = GetDataDictionaryFromSymbol(symbol);

            string oldName = symbol.Name;
            string newName = commands[2].ToUpper();

            CheckIfValidNewName(dictionary,newName);

            // a couple things if changing the name of a dictionary
            if( symbol == dictionary )
            {
                _dictionaries.Remove(dictionary.Name);
                _dictionarySymbols.Remove(dictionary.Name);

                _dictionaries.Add(newName,dictionary);
            }
               
            // change the name
            symbol.Name = newName;

            LoadDictionarySymbols(dictionary);
            RefreshSymbolParents();

            SetOutputSuccess(String.Format(Messages.RenameSuccess,oldName,newName));
        }


        void CheckCanUnsubitemize(Item item)
        {
            if( item.Subitems.Count == 0 )
                throw new Exception(String.Format(Messages.UnsubitemizeNoSubitems,item.Name));

            int startingPos = item.Start;
            bool hasGap = false;

            foreach( Item subitem in item.Subitems )
            {
                if( subitem.Start < startingPos )
                    throw new Exception(String.Format(Messages.UnsubitemizeOverlappingSubitems,item.Name));

                else if( subitem.Start != startingPos )
                    hasGap = true;

                startingPos = subitem.Start + subitem.Length;
            }

            if( item.ParentDictionary.RelativePositioning && hasGap )
                throw new Exception(String.Format(Messages.UnsubitemizeSubitemGap,item.Name));
        }

        // FLATTEN -- [UNSUBITEMIZE] item-name
        public void ProcessFlatten(string[] commands)
        {
            const string FlattenNameFormat = "{0}_{1}";
            const string FlattenLabelFormat = "{0} ({1})";

            bool createItem = true;

            if( commands.Length == 3 )
            {
                if( !commands[1].Equals("UNSUBITEMIZE",StringComparison.InvariantCultureIgnoreCase) )
                    throw new Exception(String.Format(Messages.ProcessorUnrecognizedCommandSpecify,commands[1]));

                createItem = false;
            }

            Item item = (Item)GetSymbolFromName(commands[createItem ? 1: 2],SymbolTypes.Item);

            if( !createItem )
                CheckCanUnsubitemize(item);

            if( item.Occurrences == 1 )
                throw new Exception(String.Format(Messages.FlattenMustHaveOccurrences,item.Name));

            // make sure that all of the names (to be created) are valid and available
            for( int i = createItem ? - 1 : 0; i < item.Subitems.Count; i++ )
            {
                Item thisItem = ( i == -1 ) ? item : item.Subitems[i];

                for( int occ = 0; occ < item.Occurrences; occ++ )
                {
                    string newName = null;
                    
                    try
                    {
                        newName = String.Format(FlattenNameFormat,thisItem.Name,occ + 1);
                        CheckIfValidNewName(item.ParentDictionary,newName);

                        foreach( ValueSet vs in thisItem.ValueSets )
                        {
                            newName = String.Format(FlattenNameFormat,vs.Name,occ + 1);
                            CheckIfValidNewName(item.ParentDictionary,newName);
                        }
                    }

                    catch( Exception )
                    {
                        throw new Exception(String.Format(Messages.FlattenNameInvalid,item.Name,newName));
                    }
                }
            }

            List<Item> recordItems = item.ParentRecord.Items;            
            int itemInsertionIndex = recordItems.IndexOf(item);
            recordItems.RemoveAt(itemInsertionIndex);

            // remove any subitems associated with the just removed item
            for( int i = 0; i < item.Subitems.Count; i++ )
                recordItems.RemoveAt(itemInsertionIndex);

            int itemsAdded = 0;

            for( int occ = 0; occ < item.Occurrences; occ++ ) 
            {
                Item firstItem = null;

                for( int i = createItem ? -1 : 0; i < item.Subitems.Count; i++ )
                {
                    Item thisItem = ( i == -1 ) ? item : item.Subitems[i];
                    Item newItem = new Item(thisItem.ParentRecord);

                    if( createItem )
                    {
                        if( firstItem == null )
                            firstItem = newItem;

                        else
                            firstItem.Subitems.Add(newItem);
                    }

                    newItem.Name = String.Format(FlattenNameFormat,thisItem.Name,occ + 1); 

                    // add an occurrence number (or label if it exists)
                    newItem.Label = String.Format(FlattenLabelFormat,thisItem.Label,
                        ( occ < item.OccurrenceLabels.Count ) ? item.OccurrenceLabels[occ] : ( occ + 1 ).ToString());

                    newItem.Start = thisItem.Start + item.Length * occ;
                    newItem.Occurrences = 1;

                    // most of the item is a direct copy
                    newItem.Note = thisItem.Note;
                    newItem.Length = thisItem.Length;
                    newItem.Numeric = thisItem.Numeric;
                    newItem.Subitem = createItem ? thisItem.Subitem : false;
                    newItem.Decimal = thisItem.Decimal;
                    newItem.DecChar = thisItem.DecChar;
                    newItem.ZeroFill = thisItem.ZeroFill;

                    // create linked value sets for each of the value sets
                    foreach( ValueSet vs in thisItem.ValueSets )
                    {
                        ValueSet newValueSet = new ValueSet(newItem);
                        
                        newValueSet.Name = String.Format(FlattenNameFormat,vs.Name,occ + 1); 
                        newValueSet.Label = String.Format(FlattenLabelFormat,vs.Label,
                            ( occ < item.OccurrenceLabels.Count ) ? item.OccurrenceLabels[occ] : ( occ + 1 ).ToString());

                        // create a new linked value set if it is not already linked
                        if( vs.LinkID == Int32.MinValue )
                            vs.LinkID = DataDictionaryWorker.GetNewValueSetLinkID(item.ParentDictionary);
                        
                        newValueSet.EstablishValueSetLink(vs);
                        newItem.ValueSets.Add(newValueSet);
                    }

                    recordItems.Insert(itemInsertionIndex++,newItem);
                    itemsAdded++;
                }
            }

            LoadDictionarySymbols(item.ParentDictionary);
            RefreshSymbolParents();

            SetOutputSuccess(String.Format(Messages.FlattenSuccess,item.Name,item.Occurrences,itemsAdded));
        }


        public void RunScript(string filename)
        {
            int commandsProcessed = 0;
            bool ranToCompletion = false;
            string storedWorkingDirectory = Directory.GetCurrentDirectory();

            try
            {
                filename = new FileInfo(filename).FullName;
                
                if( !File.Exists(filename) )
                    throw new Exception(String.Format(Messages.FileNotExist,filename));

                Directory.SetCurrentDirectory(Path.GetDirectoryName(filename));

                string[] commands = File.ReadAllLines(filename);
                
                ranToCompletion = true;

                for( int i = 0; i < commands.Length; i++ )
                {
                    string command = commands[i].Trim();

                    if( command.Length == 0 )
                        continue; // skip blank lines

                    if( ProcessCommand(command) )
                        commandsProcessed++;

                    else
                    {
                        labelCommand.Text = command;
                        ranToCompletion = false;
                        break;
                    }
                }
            }

            catch( Exception exception )
            {
                SetOutputError(exception.Message);
            }

            if( ranToCompletion )
                SetOutputSuccess(String.Format(Messages.RunSuccess,commandsProcessed,filename));

            Directory.SetCurrentDirectory(storedWorkingDirectory);
        }

        // RUN -- script-filename
        private void ProcessRun(string[] commands)
        {
            RunScript(commands[1]);
        }


        // UNSUBITEMIZE -- item-name
        private void ProcessUnsubitemize(string[] commands)
        {
            Item item = (Item)GetSymbolFromName(commands[1],SymbolTypes.Item);

            if( item.Occurrences != 1 )
                throw new Exception(String.Format(Messages.UnsubitemizeHasOccurrences,item.Name));

            CheckCanUnsubitemize(item);

            item.ParentRecord.Items.Remove(item);

            foreach( Item subitem in item.Subitems )
                subitem.Subitem = false;

            LoadDictionarySymbols(item.ParentDictionary);
            RefreshSymbolParents();

            SetOutputSuccess(String.Format(Messages.UnsubitemizeSuccess,item.Name));
        }


        // UNCHECKBOXIZE -- item-name
        private void ProcessUncheckboxize(string[] commands)
        {
            const string UncheckboxizeNameFormat = "{0}_{1}";
            const string UncheckboxizeValueSetSuffix = "_VS1";

            Item item = (Item)GetSymbolFromName(commands[1],SymbolTypes.Item);

            if( !item.Alpha )
                throw new Exception(String.Format(Messages.UncheckboxizeNotAlpha,item.Name));

            if( item.ValueSets.Count == 0 )
                throw new Exception(String.Format(Messages.UncheckboxizedNoValueSet,item.Name));

            List<Value> checkboxValues = item.ValueSets[0].Values;
            int maxValueWidth = 0;

            foreach( Value value in checkboxValues )
                maxValueWidth = Math.Max(maxValueWidth,value.Pairs[0].From.Trim().Length);

            if( maxValueWidth == 0 || item.Length % maxValueWidth != 0 )
                throw new Exception(String.Format(Messages.UncheckboxizedNotValidCheckbox,item.Name,item.ValueSets[0].Name,maxValueWidth,item.Length));

            int numCheckboxes = item.Length / maxValueWidth;

            // make sure that all of the names (to be created) are valid and available
            for( int occ = 0; occ < numCheckboxes; occ++ )
            {
                string newName = null;

                try
                {
                    newName = String.Format(UncheckboxizeNameFormat,item.Name,occ + 1);
                    CheckIfValidNewName(item.ParentDictionary,newName);

                    newName = newName + UncheckboxizeValueSetSuffix;
                    CheckIfValidNewName(item.ParentDictionary,newName);
                }

                catch( Exception )
                {
                    throw new Exception(String.Format(Messages.UnsubitemizeNameInvalid,item.Name,newName));
                }
            }

            // remove the old item
            int itemIndex = item.ParentRecord.Items.IndexOf(item);
            item.ParentRecord.Items.RemoveAt(itemIndex);

            // create the new items
            ValueSet baseVS = null;

            for( int occ = 0; occ < numCheckboxes; occ++ )
            {
                Item newItem = new Item(item.ParentRecord);
                newItem.ParentRecord.Items.Insert(itemIndex + occ,newItem);
                
                newItem.Name = String.Format(UncheckboxizeNameFormat,item.Name,occ + 1);
                newItem.Label = occ < checkboxValues.Count ? String.Format("{0} ({1})",item.Label,checkboxValues[occ].Label) : item.Label;

                newItem.Start = item.Start + maxValueWidth * occ;
                newItem.Occurrences = 1;

                newItem.Note = item.Note;
                newItem.Length = maxValueWidth;
                newItem.Numeric = true;
                newItem.Subitem = item.Subitem;
                newItem.ZeroFill = item.ParentDictionary.ZeroFillDefault;

                ValueSet vs = null;

                // add the value set
                if( baseVS == null )
                {
                    Value uncheckedValue = new Value();
                    uncheckedValue.Label = "Unchecked";
                    uncheckedValue.Pairs.Add(new ValuePair("0"));

                    Value checkedValue = new Value();
                    checkedValue.Label = "Checked";
                    checkedValue.Pairs.Add(new ValuePair("1"));

                    baseVS = new ValueSet(newItem);
                    baseVS.AddValue(uncheckedValue);
                    baseVS.AddValue(checkedValue);

                    vs = baseVS;
                }

                else
                {
                    if( baseVS.LinkID == Int32.MinValue )
                        baseVS.LinkID = DataDictionaryWorker.GetNewValueSetLinkID(item.ParentDictionary);

                    vs = new ValueSet(newItem);
                    vs.EstablishValueSetLink(baseVS);
                }

                vs.Name = String.Format(UncheckboxizeNameFormat,item.Name,occ + 1) + UncheckboxizeValueSetSuffix;
                vs.Label = newItem.Label;

                newItem.AddValueSet(vs);
            }

            SetOutputSuccess(String.Format(Messages.UncheckboxizedSuccess,item.Name,numCheckboxes));
        }


    }
}
