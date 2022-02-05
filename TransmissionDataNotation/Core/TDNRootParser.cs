using System;
using System.Collections.Generic;
using Team.HobbyRobot.TDN.Core;

namespace Team.HobbyRobot.TDN.Core
{
    public class TDNRootParser : ITDNTypeParser
    {
        public string TypeKey => "obj";

        public TDNValue ReadFromStream(TDNStreamReader reader)
        {
            TDNRoot obj = new TDNRoot();
            KeyValuePair<string, TDNValue>? readData = ReadKeyValuePair(reader);

            while (readData.HasValue)
            {
                obj.InsertValue(readData.Value.Key, readData.Value.Value);
                readData = ReadKeyValuePair(reader);
            }

            return new TDNValue(obj, this);
        }

        private KeyValuePair<string, TDNValue>? ReadKeyValuePair(TDNStreamReader reader)
        {
            int firstChar = reader.Read();
            if (firstChar < 0 || reader.LastReadControlCharacter == TDNControlCharacter.ValueSeparator)
                return null;

            reader.QueueCharacter((char)firstChar);
            string type = reader.ReadType();
            string key = reader.ReadKey();
            TDNValue value = reader.settings.Parsers[type].ReadFromStream(reader);

            return new KeyValuePair<string, TDNValue>(key, value);
        }

        public void WriteToStream(TDNStreamWriter writer, object value)
        {
            if(!(value is TDNRoot))
                throw new ArgumentException("Value is not type of TDNObject!");

            TDNRoot obj = value as TDNRoot;

            foreach (var notatedValue in obj.rootData)
            {
                writer.WriteType(notatedValue.Value.Parser.TypeKey);
                writer.WriteKey(notatedValue.Key);
                notatedValue.Value.Parser.WriteToStream(writer, notatedValue.Value.Value);
            }
            writer.WriteValue("");
        }
    }
}
