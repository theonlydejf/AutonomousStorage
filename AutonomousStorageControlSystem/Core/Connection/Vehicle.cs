using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Team.HobbyRobot.ASCS.Core.Physical;

namespace Team.HobbyRobot.ASCS.Core.Connection
{
    public class Vehicle
    {
        public VehicleConnection Connection { get; private set; }
        public PhysicalStorageVehicleModel VehicleModel { get; set; }
    }
}
