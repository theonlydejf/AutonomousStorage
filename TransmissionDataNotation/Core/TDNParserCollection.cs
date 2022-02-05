using System;
using System.Collections;
using System.Collections.Generic;

namespace Team.HobbyRobot.TDN.Core
{
    public class TDNParserCollection : IEnumerable<KeyValuePair<string, ITDNTypeParser>>, IEnumerable, IReadOnlyCollection<KeyValuePair<string, ITDNTypeParser>>, IReadOnlyDictionary<string, ITDNTypeParser>
    {
        public Dictionary<string, ITDNTypeParser> TDNParserLookupTable;

        public TDNParserCollection(params ITDNTypeParser[] parsers)
        {
            TDNParserLookupTable = new Dictionary<string, ITDNTypeParser>();
            foreach (var parser in parsers)
            {
                TDNParserLookupTable.Add(parser.TypeKey, parser);
            }
        }

        public ITDNTypeParser this[string typeKey] => TDNParserLookupTable[typeKey];

        public int Count => TDNParserLookupTable.Count;

        public IEnumerable<string> Keys => TDNParserLookupTable.Keys;

        public IEnumerable<ITDNTypeParser> Values => TDNParserLookupTable.Values;

        public bool ContainsKey(string typeKey) => TDNParserLookupTable.ContainsKey(typeKey);

        public IEnumerator<KeyValuePair<string, ITDNTypeParser>> GetEnumerator() => TDNParserLookupTable.GetEnumerator();

        public bool TryGetValue(string typeKey, out ITDNTypeParser value) => TDNParserLookupTable.TryGetValue(typeKey, out value);

        IEnumerator IEnumerable.GetEnumerator() => TDNParserLookupTable.GetEnumerator();
    }
}
