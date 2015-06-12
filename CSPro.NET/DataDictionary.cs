using System;
using System.Collections.Generic;

// this dictionary class is, for now, intended to read valid CSPro dictionaries;
// you can make modifications to the dictionary, but you have the responsibility of
// ensuring that the modifications are valid


namespace CSPro
{
    public class DataDictionaryObject
    {
        public string Name { get; set; }
        public string Label { get; set; }
        public string Note { get; set; }

        public DataDictionaryObject()
        {
            Name = "";
            Label = "";
            Note = "";
        }
    }

    public class DataDictionary : DataDictionaryObject
    {
        private List<Level> _levels;
        private List<Relation> _relations;

        public List<Level> Levels { get { return _levels; } }
        public List<Relation> Relations { get { return _relations; } }

        public double Version { get; set; }
        public int RecTypeStart { get; set; }
        public int RecTypeLength { get; set; }
        public bool RelativePositioning { get; set; }
        public bool AbsolutePositioning { get { return !RelativePositioning; } set { RelativePositioning = !value; } }
        public bool ZeroFillDefault { get; set; }
        public bool DecimalCharDefault { get; set; }
        public bool UsingValueSetImages { get; set; }

        public DataDictionary()
        {
            _levels = new List<Level>();
            _relations = new List<Relation>();

            Version = CurrentVersion.Version;
            RecTypeStart = 0;
            RecTypeLength = 0;
            RelativePositioning = true;
            ZeroFillDefault = false;
            DecimalCharDefault = true;
            UsingValueSetImages = false;
        }

        public void AddLevel(Level level)
        {
            _levels.Add(level);
        }
    }

    public class Level : DataDictionaryObject
    {
        private DataDictionary _parent;
        private Record _ids;
        private List<Record> _records;

        public DataDictionary Parent { get { return _parent; } }
        public DataDictionary ParentDictionary { get { return _parent; } }

        public Record IDs { get { return _ids; } }
        public List<Record> Records { get { return _records; } }

        public int LevelNum { get; set; }

        public Level(DataDictionary parent,int levelNum)
        {
            _parent = parent;
            _records = new List<Record>();
            LevelNum = levelNum;

            _ids = new Record(this);
            _ids.Name = String.Format("_IDS{0}",LevelNum - 1);
            _ids.Label = "(Id Items)";
        }

        public void AddRecord(Record record)
        {
            _records.Add(record);
        }
    }

    public class Record : DataDictionaryObject
    {
        private Level _parent;
        private List<Item> _items;
        private List<string> _occurrenceLabels;

        public Level Parent { get { return _parent; } }
        public Level ParentLevel { get { return _parent; } }
        public DataDictionary ParentDictionary { get { return _parent.ParentDictionary; } }

        public List<Item> Items { get { return _items; } }
        public List<string> OccurrenceLabels { get { return _occurrenceLabels; } }

        public bool Required { get; set; }
        public int MaxOccs { get; set; }
        public int Length { get; set; }
        public string RecordType { get; set; }

        public Record(Level level)
        {
            _parent = level;
            _items = new List<Item>();
            _occurrenceLabels = new List<string>();
            Required = true;
            MaxOccs = 1;
            Length = 0;
            RecordType = "";
        }

        public void AddItem(Item item)
        {
            _items.Add(item);
        }
    }

    public class Item : DataDictionaryObject
    {
        private Record _parent;
        private List<ValueSet> _valuesets;
        private List<Item> _subitems;
        private List<string> _occurrenceLabels;

        public Record Parent { get { return _parent; } }
        public Record ParentRecord { get { return _parent; } }
        public Level ParentLevel { get { return _parent.ParentLevel; } }
        public DataDictionary ParentDictionary { get { return _parent.ParentDictionary; } }

        public List<ValueSet> ValueSets { get { return _valuesets; } }
        public List<Item> Subitems { get { return _subitems; } }
        public List<string> OccurrenceLabels { get { return _occurrenceLabels; } }

        public int Start { get; set; }
        public int Length { get; set; }
        public bool Numeric { get; set; }
        public bool Alpha { get { return !Numeric; } set { Numeric = !value; } }
        public bool Subitem { get; set; }
        public int Occurrences { get; set; }
        public int Decimal { get; set; }
        public bool DecChar { get; set; }
        public int NonDecimalLength { get { return Length - ( DecChar ? 1 : 0 ) - Decimal; } }
        public bool ZeroFill { get; set; }

        public Item(Record record)
        {
            _parent = record;
            _valuesets = new List<ValueSet>();
            _subitems = new List<Item>();
            _occurrenceLabels = new List<string>();
            Start = 1;
            Length = 1;
            Numeric = true;
            Subitem = false;
            Occurrences = 1;
            Decimal = 0;
            DecChar = false;
            ZeroFill = false;
        }

        public void AddValueSet(ValueSet vs)
        {
            _valuesets.Add(vs);
        }
    }

    public class ValueSet : DataDictionaryObject
    {
        private Item _parent;
        private List<Value> _values;

        public Item Parent { get { return _parent; } }
        public Item ParentItem { get { return _parent; } }
        public Record ParentRecord { get { return _parent.ParentRecord; } }
        public Level ParentLevel { get { return _parent.ParentLevel; } }
        public DataDictionary ParentDictionary { get { return _parent.ParentDictionary; } }

        public List<Value> Values { get { return _values; } }

        public int LinkID { get; set; }

        public ValueSet(Item item)
        {
            _parent = item;
            _values = new List<Value>();
            LinkID = Int32.MinValue;
        }

        public void EstablishValueSetLink(ValueSet vs,int linkID)
        {
            _values = vs._values;
            LinkID = linkID;
        }

        public void AddValue(Value value)
        {
            _values.Add(value);
        }
    }

    public class Value
    {
        public enum Special { None, NotApplicable, Missing, Default }

        private List<ValuePair> _pairs;

        public List<ValuePair> Pairs { get { return _pairs; } }

        public string Label { get; set; }
        public string Note { get; set; }
        public string ImageFilename { get; set; }
        public Special SpecialValue { get; set; }

        public Value()
        {
            _pairs = new List<ValuePair>();
            Label = "";
            Note = "";
            ImageFilename = "";
            SpecialValue = Special.None;
        }

        public Value(Value value)
        {
            _pairs = new List<ValuePair>();
            Label = value.Label;
            SpecialValue = value.SpecialValue;

            for( int i = 0; i < value.Pairs.Count; i++ )
                AddValuePair(new ValuePair(value.Pairs[i]));
        }

        public void AddValuePair(ValuePair vp)
        {
            _pairs.Add(vp);
        }
    }

    public class ValuePair
    {
        public string From { get; set; }
        public string To { get; set; }

        public ValuePair()
        {
            From = "";
            To = "";
        }

        public ValuePair(ValuePair vp)
        {
            From = vp.From;
            To = vp.To;
        }
    }

    public class Relation
    {
        public string Name { get; set; }
        public string Primary { get; set; }
        public string PrimaryLink { get; set; }
        public string Secondary { get; set; }
        public string SecondaryLink { get; set; }

        public Relation()
        {
            Name = "";
            Primary = "";
            PrimaryLink = "";
            Secondary = "";
            SecondaryLink = "";
        }

        public Relation(Relation relation)
        {
            Name = relation.Name;
            Primary = relation.Primary;
            PrimaryLink = relation.PrimaryLink;
            Secondary = relation.Secondary;
            SecondaryLink = relation.SecondaryLink;
        }
    }

}
