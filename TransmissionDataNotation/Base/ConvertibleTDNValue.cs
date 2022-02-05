using System;
using Team.HobbyRobot.TDN.Core;

namespace Team.HobbyRobot.TDN.Base
{
    public class ConvertibleTDNValue : TDNValue
    {
        public ConvertibleTDNValue(object value, ITDNTypeParser parser) : base(value, parser) { }

        public static implicit operator ConvertibleTDNValue(TDNArray arr) => new ConvertibleTDNValue(arr, new ArrayParser());
        public static implicit operator ConvertibleTDNValue(bool b) => new ConvertibleTDNValue(b, new BooleanParser());
        public static implicit operator ConvertibleTDNValue(float f) => new ConvertibleTDNValue(f, new FloatParser());
        public static implicit operator ConvertibleTDNValue(int i) => new ConvertibleTDNValue(i, new IntegerParser());
        public static implicit operator ConvertibleTDNValue(string s) => new ConvertibleTDNValue(s, new StringParser());
        public static implicit operator ConvertibleTDNValue(TDNRoot obj) => new ConvertibleTDNValue(obj, new TDNRootParser());
    }
}
