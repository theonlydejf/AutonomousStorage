using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Team.HobbyRobot.ASCS.Core.Physical
{
    interface IDynamicObject
    {
        public double Orientation { get; set; }
        public float Speed { get; set; }
        public PointF Location { get; set; }
        public SizeF Velocity { get; }
    }
}
