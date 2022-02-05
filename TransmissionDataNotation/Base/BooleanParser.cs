using System;
using Team.HobbyRobot.TDN.Core;

namespace Team.HobbyRobot.TDN.Base
{
    public class BooleanParser : ITDNTypeParser
    {
        public string TypeKey => "bln";

        public TDNValue ReadFromStream(TDNStreamReader reader)
            => new TDNValue(reader.ReadValue().ToLower().Equals("true"), this);

        public void WriteToStream(TDNStreamWriter writer, object value)
        {
            if (!(value is bool))
                throw new ArgumentException("Value is not type of bool!");

            writer.WriteValue(((bool)value).ToString().ToLower());
        }
    }
}
