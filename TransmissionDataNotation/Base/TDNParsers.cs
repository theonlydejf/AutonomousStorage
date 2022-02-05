using System;
using System.Collections.Generic;
using System.Text;
using Team.HobbyRobot.TDN.Core;

namespace Team.HobbyRobot.TDN.Base
{
    public static class TDNParsers
    {
		public static readonly ITDNTypeParser ARRAY = new ArrayParser();
		public static readonly ITDNTypeParser BOOLEAN = new BooleanParser();
		public static readonly ITDNTypeParser FLOAT = new FloatParser();
		public static readonly ITDNTypeParser INTEGER = new IntegerParser();
		public static readonly ITDNTypeParser STRING = new StringParser();
		public static readonly ITDNTypeParser ROOT = new TDNRootParser();
	}
}
