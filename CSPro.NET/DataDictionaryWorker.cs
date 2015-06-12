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

    }
}
