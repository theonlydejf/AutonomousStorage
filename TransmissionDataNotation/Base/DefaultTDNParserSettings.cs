using System;
using Team.HobbyRobot.TDN.Core;

namespace Team.HobbyRobot.TDN.Base
{
    public class DefaultTDNParserSettings : TDNParserSettings
    {
        public DefaultTDNParserSettings() : base
            (
                '|',
                ':',
                ';',
                '\\',
                new TDNParserCollection
                (
                    new ArrayParser(),
                    new BooleanParser(),
                    new FloatParser(),
                    new IntegerParser(),
                    new StringParser(),
                    new TDNRootParser()
                )
            ) { }
    }
}
