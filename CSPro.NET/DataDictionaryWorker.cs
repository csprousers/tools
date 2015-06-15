using System;
using System.Collections.Generic;

// some useful routines

namespace CSPro
{
    public class DataDictionaryWorker
    {
        public static void CalculateRecordLengths(DataDictionary dictionary)
        {
            foreach( Level level in dictionary.Levels )
            {
                int maxLengthIDs = CalculateMaxLengthFromItems(level.IDs.Items);

                foreach( Record record in level.Records )
                    record.Length = Math.Max(maxLengthIDs,CalculateMaxLengthFromItems(record.Items));
            }
        }

        private static int CalculateMaxLengthFromItems(List<Item> items)
        {
            int maxLength = 0;

            foreach( Item item in items )
            {
                if( !item.Subitem )
                    maxLength = Math.Max(maxLength,item.Start + item.Length * item.Occurrences - 1);
            }

            return maxLength;
        }

        public static void CalculateItemPositions(DataDictionary dictionary)
        {
            if( dictionary.AbsolutePositioning )
                return;

            int lengthIDs = 0;

            foreach( Level level in dictionary.Levels )
            {
                int position = dictionary.RecTypeLength + 1;

                foreach( Item item in level.IDs.Items )
                {
                    item.Start = position;
                    position += item.Length;
                    lengthIDs += item.Length;
                }

                foreach( Record record in level.Records )
                {
                    int lastItemOffset = 0;

                    position = dictionary.RecTypeLength + 1 + lengthIDs;                    

                    foreach( Item item in record.Items )
                    {
                        if( item.Subitem )
                            item.Start += lastItemOffset;

                        else
                        {
                            lastItemOffset = position - item.Start;
                            item.Start = position;
                            position += item.Length * item.Occurrences;
                        }
                    }
                }
            }
        }


        public static List<Item> GetAllItems(DataDictionary dictionary)
        {
            List<Item> items = new List<Item>();

            foreach( Level level in dictionary.Levels )
            {
                for( int i = -1; i < level.Records.Count; i++ )
                {
                    Record record = ( i == -1 ) ? level.IDs : level.Records[i];
                    items.AddRange(record.Items);
                }
            }

            return items;
        }

        public static List<ValueSet> GetAllValueSets(DataDictionary dictionary)
        {
            List<ValueSet> valueSets = new List<ValueSet>();

            foreach( Item item in GetAllItems(dictionary) )
                valueSets.AddRange(item.ValueSets);

            return valueSets;
        }


        public static int GetNewValueSetLinkID(DataDictionary dictionary)
        {
            // create a listing of used IDs
            HashSet<int> usedIDs = new HashSet<int>();
            
            foreach( ValueSet vs in GetAllValueSets(dictionary) )
            {
                if( vs.LinkID != Int32.MinValue && !usedIDs.Contains(vs.LinkID) )
                    usedIDs.Add(vs.LinkID);
            }
            
            // return a linked value over 10000000
            for( int linkID = 10000001; ; linkID++ )
            {
                if( !usedIDs.Contains(linkID) )
                    return linkID;
            }
        }

    }
}
