using System;
using Team.HobbyRobot.TDN.Core;

namespace Team.HobbyRobot.TDN.Base
{
    public class ArrayParser : ITDNTypeParser
    {
        public string TypeKey => "arr";

        private static readonly IntegerParser intParser = new IntegerParser();

        // LENGTH;TYPE;VAL;...
        public TDNValue ReadFromStream(TDNStreamReader reader)
        {
            int itemCnt = (int)intParser.ReadFromStream(reader).Value;
            string itemType = reader.ReadValue();
            ITDNTypeParser itemParser = reader.settings.Parsers[itemType];

            object[] array = new object[itemCnt];

            for (int i = 0; i < itemCnt; i++)
                array[i] = itemParser.ReadFromStream(reader).Value;

            return new TDNValue(new TDNArray(array, itemParser), this);
        }

        public void WriteToStream(TDNStreamWriter writer, object value)
        {
            if(!(value is TDNArray))
                throw new ArgumentException("Value is not type of TDNArray!");

            TDNArray arr = value as TDNArray;
            intParser.WriteToStream(writer, arr.Value.Length);
            writer.WriteValue(arr.ItemParser.TypeKey);
            foreach (var item in arr.Value)
                arr.ItemParser.WriteToStream(writer, item);
        }
    }
}
