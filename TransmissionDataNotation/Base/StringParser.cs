using System;
using System.IO;
using Team.HobbyRobot.TDN.Core;

namespace Team.HobbyRobot.TDN.Base
{
    public class StringParser : ITDNTypeParser
    {
        public string TypeKey => "str";

        public readonly char BeginStringCharacter;

        public StringParser() : this('"')
        {
        }

        public StringParser(char beginStringCharacter)
        {
            BeginStringCharacter = beginStringCharacter;
        }

        public TDNValue ReadFromStream(TDNStreamReader reader)
        {
            string value = reader.ReadValue();
            if (value.Length < 1 || value[0] != BeginStringCharacter)
                throw new FormatException();

            return new TDNValue(value.Substring(1, value.Length - 1), this);
        }

        public void WriteToStream(TDNStreamWriter writer, object value)
        {
            if (!(value is string))
                throw new ArgumentException("Value is not type of string!");

            writer.Write(BeginStringCharacter);
            writer.WriteValue((string)value);
        }
    }
}
