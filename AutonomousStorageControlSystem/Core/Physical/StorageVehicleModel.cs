using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Team.HobbyRobot.ASCS.Core.Physical
{
    public struct StorageVehicleModel : IDynamicObject
    {
        public int ID { get; }
        public double Orientation { get; set; }
        public float Speed { get; set; }
        public PointF Location { get; set; }
        public SizeF Velocity => new SizeF
        (
            (float)(Math.Cos(Orientation) * Speed),
            (float)(Math.Sin(Orientation) * Speed)
        );


    }
}
