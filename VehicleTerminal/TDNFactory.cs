using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Team.HobbyRobot.TDN.Base;
using Team.HobbyRobot.TDN.Core;

namespace VehicleTerminal
{
    public class TDNFactory
    {
        private readonly TextWriter writer;
        private readonly TextBoxBasicReader reader;
        private Dictionary<string, Func<Task<object>>> BaseParserLookup;

        public TDNFactory(TextWriter writer, TextBoxBasicReader reader)
        {
            this.writer = writer;
            this.reader = reader;
            BaseParserLookup = new Dictionary<string, Func<Task<object>>>()
            {
                { "bln", () => ReadBoolean() },
                { "flt", () => ReadFloat() },
                { "int", () => ReadInteger() },
                { "str", () => ReadString() }
            };
        }

        public async Task<object> ReadAnyObject(string valueKey, TDNParserSettings settings)
        {
            writer.WriteLine("Select type parser:");
            PrintParsers(settings);
            string typeKey = (await ReadFromUser("Enter type key: ")).ToLower();

            return await ReadAnyObject (typeKey, valueKey, settings);
        }

        public async Task<object> ReadAnyObject(string typeKey, string valueKey, TDNParserSettings settings)
        {
            writer.Write("Enter value: ");
            if (BaseParserLookup.ContainsKey(typeKey))
            {
                return await BaseParserLookup[typeKey]();
            }
            else if (typeKey == new TDNRootParser().TypeKey)
            {
                return await ReadRoot(valueKey, settings, new TDNRoot());
            }
            else if (typeKey == new ArrayParser().TypeKey)
            {
                return await ReadArray(valueKey, settings);
            }
            else
            {
                writer.WriteLine("Unknown type key!");
                return await ReadAnyObject(valueKey, settings);
            }
        }

        public async Task<string> ReadTypeKey(TDNParserSettings settings)
        {
            writer.WriteLine("Select type parser:");
            PrintParsers(settings);
            string typeKey = (await ReadFromUser("Enter type key: ")).ToLower();
            if (settings.Parsers.ContainsKey(typeKey))
                return typeKey;

            writer.WriteLine("Unknown type key!");
            return await ReadTypeKey(settings);
        }

        public async Task<TDNRoot> ReadRoot(string rootKey, TDNParserSettings settings, TDNRoot startRoot)
        {
            TDNRoot root = startRoot;
            while (true)
            {
                //TODO vytvoreni
                writer.WriteLine("\n=====================");
                writer.WriteLine($"\n{ rootKey } contents: ");
                PrintRoot(root);

                writer.Write($"Add another value to { rootKey }? (y/n): ");
                char c = char.ToLower(await reader.Read());
                if (c != 'y')
                    return root;

                string path = await ReadFromUser("\nEnter path: ");

                writer.WriteLine("Select type parser:");
                PrintParsers(settings);
                string typeKey = (await ReadFromUser("Enter type key: ")).ToLower();

                object obj = await ReadAnyObject(typeKey, path, settings);

                root[path] = new TDNValue(obj, settings.Parsers[typeKey]);
            }
        }

        private async Task<object> ReadArray(string valueKey, TDNParserSettings settings)
        {
            writer.WriteLine($"\n\nSelect array { valueKey } item type parser:");
            PrintParsers(settings);
            string typeKey = (await ReadFromUser("Enter type key: ")).ToLower();
            if (!settings.Parsers.ContainsKey(typeKey))
            {
                writer.WriteLine("Unknown type key!");
                return await ReadArray(valueKey, settings);
            }

            uint len = Convert.ToUInt32(await ReadFromUser("Enter length of an array: "));
            object[] arr = new object[len];
            for (int i = 0; i < len; i++)
            {
                writer.Write($"{ valueKey }[{ i }]: ");
                arr[i] = await ReadAnyObject(typeKey, valueKey + "[" + i + "]", settings);
            }

            return new TDNArray(arr, settings.Parsers[typeKey]);
        }

        private async Task<string> ReadFromUser(string msg)
        {
            writer.Write(msg);
            string s = await reader.ReadLine();
            return s;
        }

        private void PrintParsers(TDNParserSettings settings)
        {
            writer.WriteLine();
            writer.WriteLine("Type Key\tParser name");
            foreach (var item in settings.Parsers)
            {
                writer.WriteLine($"{ item.Key }:\t\t{ item.Value.GetType().Name }");
            }
        }

        public async Task<object> ReadBoolean() => (await ReadFromUser("")).ToLower().Contains('t');

        public async Task<object> ReadFloat() => Convert.ToSingle(await ReadFromUser(""));

        public async Task<object> ReadInteger() => Convert.ToInt32(await ReadFromUser(""));

        public async Task<object> ReadString() => await ReadFromUser("");

        public void PrintRoot(TDNRoot root)
        {
            writer.WriteLine("(");
            foreach (var val in root)
            {
                writer.Write(val.Key);
                writer.Write(": ");
                if (val.Value.Value is TDNRoot root1)
                {
                    PrintRoot(root1);
                    continue;
                }
                if (val.Value.Value is TDNArray arr)
                {
                    writer.WriteLine("[");
                    foreach (object item in arr)
                    {
                        if (arr.ItemParser.TypeKey.Equals(new TDNRootParser().TypeKey))
                            PrintRoot((TDNRoot)item);
                        else
                            writer.WriteLine(item);
                        writer.Write(",");
                        continue;
                    }
                    writer.WriteLine("]");
                    continue;
                }
                writer.WriteLine(val.Value.Value);
            }
            writer.WriteLine(")");
        }
    }
}
