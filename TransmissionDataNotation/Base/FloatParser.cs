using System;
using System.Globalization;
using Team.HobbyRobot.TDN.Core;

namespace Team.HobbyRobot.TDN.Base
{
    public class FloatParser : ITDNTypeParser
    {
        public string TypeKey => "flt";

        public TDNValue ReadFromStream(TDNStreamReader reader)
            => new TDNValue(Convert.ToSingle(reader.ReadValue(), CultureInfo.InvariantCulture), this);

        public void WriteToStream(TDNStreamWriter writer, object value)
        {
            if (!(value is float))
                throw new ArgumentException("Value is not type of float!");

            writer.WriteValue(((float)value).ToString(System.Globalization.CultureInfo.InvariantCulture));
        }
    }
}
