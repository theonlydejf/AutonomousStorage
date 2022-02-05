using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Team.HobbyRobot.TDN.Base;

namespace Team.HobbyRobot.TDN.Core
{
    public class TDNRoot : IEnumerable<KeyValuePair<string, TDNValue>>
    {
        public TDNRoot()
        {
            rootData = new Dictionary<string, TDNValue>();
        }

        internal Dictionary<string, TDNValue> rootData;

        public TDNValue this[string key]
        {
            get
            {
                string[] path = key.Split('.');
                var root = GetRootData(path);
                return root[path.Last()];
            }
            set
            {
                string[] path = key.Split('.');
                var root = GetRootData(path, true);
                bool containsKey = root.ContainsKey(path.Last());
                if (containsKey && value == null)
                    root.Remove(path.Last());
                else if (!containsKey)
                    root.Add(path.Last(), value);
                else
                    root[path.Last()] = value;
            }
        }

        private Dictionary<string, TDNValue> GetRootData(string[] path, bool createNewRoots = false)
        {
            IEnumerable<string> rootPath = path.Take(path.Length - 1);

            Dictionary<string, TDNValue> currTable = rootData;
            foreach (string rootName in rootPath)
            {
                if (createNewRoots && !currTable.ContainsKey(rootName))
                    currTable.Add(rootName, new TDNValue(new TDNRoot(), new TDNRootParser()));
                TDNValue newRoot = currTable[rootName];
                if (newRoot.Parser.TypeKey != new TDNRootParser().TypeKey)
                    throw new ArgumentException($"Root \"{ rootName }\" in path \"{ string.Join(".", path) }\" is not a valid root!");
                currTable = ((TDNRoot)newRoot.Value).rootData;
            }

            return currTable;
        }

        public void WriteToStream(Stream s)
        {
            TDNStreamWriter sw = new TDNStreamWriter(new StreamWriter(s));

            new TDNRootParser().WriteToStream(sw, this);
        }

        public static TDNRoot ReadFromStream(Stream s)
        {
            TDNStreamReader reader = new TDNStreamReader(new StreamReader(s));
            TDNValue objVal = new TDNRootParser().ReadFromStream(reader);
            return objVal.Value as TDNRoot;
        }

        public TDNRoot InsertValue(string key, TDNValue value)
        {
            rootData.Add(key, value);
            return this;
        }

        public IEnumerator<KeyValuePair<string, TDNValue>> GetEnumerator()
        {
            return rootData.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)rootData).GetEnumerator();
        }
    }
}
