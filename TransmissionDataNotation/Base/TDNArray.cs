using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Team.HobbyRobot.TDN.Core;

namespace Team.HobbyRobot.TDN.Base
{
    public class TDNArray : IEnumerable
    {
        public TDNArray(object[] value, ITDNTypeParser itemParser)
        {
            Value = value;
            ItemParser = itemParser;
        }

        public object[] Value { get; set; }
        public ITDNTypeParser ItemParser { get; set; }

        public object this[int index]
        {
            get => Value[index];
            set => Value[index] = value;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Value.GetEnumerator();
        }
    }

    public static class TDNArrayExtensions
    {
        public static T[] AsArray<T>(this TDNValue value) => ((TDNArray)value.Value).Value.Select(x => (T)x).ToArray();
    }

}
