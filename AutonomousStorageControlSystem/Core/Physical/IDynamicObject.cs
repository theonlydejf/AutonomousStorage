using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Team.HobbyRobot.ASCS.Core.Physical
{
    interface IDynamicObject
    {
        public double Orientation { get; set; }
        public float Speed { get; set; }
        public Vector2 Location { get; set; }
        public Vector2 Velocity { get; }
    }
}
