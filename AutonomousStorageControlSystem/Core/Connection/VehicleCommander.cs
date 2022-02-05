using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Team.HobbyRobot.ASCS.Core.Logging;
using Team.HobbyRobot.ASCS.Core.Physical;
using Team.HobbyRobot.TDN.Base;
using Team.HobbyRobot.TDN.Core;

namespace Team.HobbyRobot.ASCS.Core.Connection
{
    public class VehicleCommander : TDNAPICommander
    {
        public const string MOVEMENT_SERVICE_NAME = "MovementService";

        public VehicleCommander(VehicleConnection connection, Logger logger = null) : base(connection, logger)
        {
        }

        public TDNRoot RequestTravel(float distance) => Request(MOVEMENT_SERVICE_NAME, "travel", new TDNRoot().InsertValue("distance", (ConvertibleTDNValue)distance));
        public async Task<TDNRoot> RequestTravelAsync(float distance) => await RequestAsync(MOVEMENT_SERVICE_NAME, "travel", new TDNRoot().InsertValue("distance", (ConvertibleTDNValue)distance));
    }
}