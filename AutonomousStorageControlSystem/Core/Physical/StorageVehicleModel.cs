using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Team.HobbyRobot.ASCS.Core.Physical
{
    public struct StorageVehicleModel : IDynamicObject
    {
        public int ID { get; }
        public double Orientation { get; set; }
        public float Speed { get; set; }
        public Vector2 Location { get; set; }
        public Vector2 Velocity => new Vector2
        (
            (float)(Math.Cos(Orientation) * Speed),
            (float)(Math.Sin(Orientation) * Speed)
        );


    }
}
