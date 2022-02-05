using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
namespace Team.HobbyRobot.TDN.Core
{
    public class TDNParserSettings
    {
        public char TypeSeparator { get; }
        public char KeySeparator { get; }
        public char ValueSeparator { get; }
        public char EscapeCharacter { get; }
        public TDNParserCollection Parsers { get; }

        public TDNParserSettings(char typeSeparator, char keySeparator, char valueSeparator, char escapeCharacter, TDNParserCollection parsers)
        {
            TypeSeparator = typeSeparator;
            KeySeparator = keySeparator;
            ValueSeparator = valueSeparator;
            EscapeCharacter = escapeCharacter;
            Parsers = parsers;
        }

        internal char[] GetControlChars() => new[] { TypeSeparator, KeySeparator, ValueSeparator };
        internal char[] GetInvalidChars() => new[] { TypeSeparator, KeySeparator, ValueSeparator, EscapeCharacter };

        public TDNControlCharacter GetCharacterType(char character)
            => (TDNControlCharacter)(Array.IndexOf(GetControlChars(), character) + 1);
    }

    public enum TDNControlCharacter
    {
        None,
        TypeSeparator,
        KeySeparator,
        ValueSeparator
    }
}
