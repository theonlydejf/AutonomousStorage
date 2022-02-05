using System;
using System.IO;

namespace Team.HobbyRobot.TDN.Core
{
    public interface ITDNTypeParser
    {
        string TypeKey { get; }
        TDNValue ReadFromStream(TDNStreamReader reader);
        void WriteToStream(TDNStreamWriter writer, object value);
    }
}
