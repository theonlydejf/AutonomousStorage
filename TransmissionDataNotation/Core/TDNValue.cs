using System;
using System.Collections;
using System.Linq;

namespace Team.HobbyRobot.TDN.Core
{
    /// <summary>
    /// Object that holds a parsable value with its corresponding parser
    /// </summary>
    public class TDNValue
    {
        public TDNValue(object value, ITDNTypeParser parser)
        {
            Parser = parser;
            Value = value;
        }

        /// <summary>
        /// Parser, which is used to parse value to or from stream
        /// </summary>
        public ITDNTypeParser Parser { get; }
        /// <summary>
        /// Parsable value
        /// </summary>
        public object Value { get; set; }

        public override string ToString()
        {
            return Parser.TypeKey + ": " + Value.ToString();
        }

        public T As<T>()
        {
            Type type = typeof(T);
            if (type.IsArray)
                throw new InvalidCastException("This method does not support casting arrays. Use different method!");
            return (T)Value;
        }
    }
}
