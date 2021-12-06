using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Team.HobbyRobot.ASCS.Core.Physical
{
    /// <summary>
    /// Model of the storage vehicle
    /// </summary>
    public struct StorageVehicleModel : IDynamicObject
    {
        /// <summary>
        /// ID of the storage vehicle
        /// </summary>
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
