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
        }

        public MainForm()
        {
            InitializeComponent();

            InitializeFunctions();
            RefreshSymbolParents();
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

        private void ProcessCommand(string command)
        {
            try
            {
                command = command.Trim();

                if( command.Length == 0 )
                    throw new Exception(Messages.ProcessorNoCommand);

                // regular expression from http://stackoverflow.com/questions/14655023/split-a-string-that-has-white-spaces-unless-they-are-enclosed-within-quotes
                string[] commands = Regex.Split(command,"(?<=^[^\"]*(?:\"[^\"]*\"[^\"]*)*) (?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");

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
            }
        }


        private string GetFilenameFromArgument(string argument)
        {
            // if the file name was surrounded in quotes, remove the quotes
            if( argument.Length >= 2 && argument[0] == '"' && argument[argument.Length - 1] == '"' )
                argument = argument.Substring(1,argument.Length - 2);

            return new FileInfo(argument).FullName;
        }

        private DataDictionary GetDataDictionaryFromName(string name)
        {
            name = name.ToUpper();

            if( _dictionaries.ContainsKey(name) )
                return _dictionaries[name];

            else
                throw new Exception(String.Format(Messages.DictionaryNotFound,name));
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
            
            _dictionarySymbols.Add(dictionary.Name,symbols);
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
            string filename = GetFilenameFromArgument(commands[1]);

            if( !File.Exists(filename) )
                throw new Exception(String.Format(Messages.FileNotExist,filename));

            DataDictionary dictionary = DataDictionaryReader.Load(filename); // Load can throw an exception

            if( dictionary.Levels.Count != 1 )
                throw new Exception(Messages.DictionaryNotOneLevel);

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
            string filename = GetFilenameFromArgument(commands[2]);

            const string DictionaryExtension = ".dcf";

            if( !Path.GetExtension(filename).Equals(DictionaryExtension,StringComparison.InvariantCultureIgnoreCase) )
                filename = filename + DictionaryExtension;

            DataDictionaryWriter.Save(dictionary,filename); // Save can throw an exception

            SetOutputSuccess(String.Format(Messages.DictionarySaved,dictionary.Name,filename));
        }

    }
}
