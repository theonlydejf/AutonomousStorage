using System.Collections.Generic;
using System.IO;
using System.Text;
using Team.HobbyRobot.TDN.Base;

namespace Team.HobbyRobot.TDN.Core
{
    public class TDNStreamReader
    {
        public TDNControlCharacter LastReadControlCharacter { get; private set; }
        public bool IsLastReadCharacterEscaped { get; private set; }

        private readonly StreamReader sr;
        private Queue<char> charQueue = new Queue<char>();
        public readonly TDNParserSettings settings;

        public TDNStreamReader(StreamReader streamReader) : this(streamReader, new DefaultTDNParserSettings()) { }

        public TDNStreamReader(StreamReader streamReader, TDNParserSettings parserSettings)
        {
            sr = streamReader;
            settings = parserSettings;
        }

        /*public TDNCharStreamReader(StreamReader streamReader, DefaultTDNParserSettings defaultTDNParserSettings)
        {
            sr = streamReader;
            settings = defaultTDNParserSettings;
        }*/

        public int Read() => Read(true);

        private int Read(bool useEscapeChar)
        {
            int iChar = -1;
            if (charQueue.Count > 0)
                iChar = charQueue.Dequeue();
            else
                iChar = sr.Read();
            
            if (iChar < 0)
                return iChar;

            char c = (char)iChar;
            if (useEscapeChar)
                IsLastReadCharacterEscaped = c == settings.EscapeCharacter;

            if (c == settings.EscapeCharacter && useEscapeChar)
            {
                LastReadControlCharacter = TDNControlCharacter.None;
                return Read(false);
            }
            if(!IsLastReadCharacterEscaped)
                LastReadControlCharacter = settings.GetCharacterType(c);

            return c;
        }

        public void QueueCharacter(char c) => charQueue.Enqueue(c);

        public string ReadUntilControlCharacter(TDNControlCharacter controlChar)
        {
            StringBuilder sb = new StringBuilder();
            int currChar;
            while ((currChar = Read()) >= 0 && LastReadControlCharacter != controlChar)
                sb.Append((char)currChar);

            if (currChar < 0)
                throw new EndOfStreamException("End of stream was unexpectadly reached before " + controlChar.ToString());

            return sb.ToString();
        }

        public string ReadType() => ReadUntilControlCharacter(TDNControlCharacter.TypeSeparator);
        public string ReadKey() => ReadUntilControlCharacter(TDNControlCharacter.KeySeparator);
        public string ReadValue() => ReadUntilControlCharacter(TDNControlCharacter.ValueSeparator);
    }
}