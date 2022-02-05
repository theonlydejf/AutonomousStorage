using System;
using Team.HobbyRobot.TDN.Core;

namespace Team.HobbyRobot.TDN.Base
{
    public class IntegerParser : ITDNTypeParser
    {
        public string TypeKey => "int";

        public TDNValue ReadFromStream(TDNStreamReader reader)
            => new TDNValue(Convert.ToInt32(reader.ReadValue()), this);

        public void WriteToStream(TDNStreamWriter writer, object value)
        {
            if (!(value is int))
                throw new ArgumentException("Value is not type of int!");

            writer.WriteValue(((int)value).ToString());
        }
    }
}
