using System;
using System.IO;
using Team.HobbyRobot.TDN.Base;

namespace Team.HobbyRobot.TDN.Core
{
    public class TDNStreamWriter
    {
        private readonly StreamWriter sw;
        private readonly TDNParserSettings settings;

        public TDNStreamWriter(StreamWriter streamWriter) : this(streamWriter, new DefaultTDNParserSettings()) { }

        public TDNStreamWriter(StreamWriter streamWriter, TDNParserSettings parserSettings)
        {
            sw = streamWriter;
            settings = parserSettings;
        }

        public void Write(char c, bool flushStream = true)
        {
            if (Array.IndexOf(settings.GetInvalidChars(), c) >= 0)
                sw.Write(settings.EscapeCharacter);
            sw.Write(c);
            if (flushStream)
                sw.Flush();
        }

        public void Write(string s, bool flushStream = true)
        {
            foreach (char c in s)
                Write(c, false);
            if(flushStream)
                sw.Flush();
        }

        public void WriteType(string value)
        {
            Write(value, false);
            sw.Write(settings.TypeSeparator);
            sw.Flush();
        }

        public void WriteKey(string value)
        {
            Write(value, false);
            sw.Write(settings.KeySeparator);
            sw.Flush();
        }

        public void WriteValue(string value)
        {
            Write(value, false);
            sw.Write(settings.ValueSeparator);
            sw.Flush();
        }
    }
}