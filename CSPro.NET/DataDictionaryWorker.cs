using System;
using System.Collections.Generic;

// some useful routines

namespace CSPro
{
    public class DataDictionaryWorker
    {
        public static void CalculateRecordLengths(DataDictionary dict)
        {
            foreach( Level level in dict.Levels )
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
                maxLength = Math.Max(maxLength,item.Start + item.Length - 1);

            return maxLength;
        }

    }
}
