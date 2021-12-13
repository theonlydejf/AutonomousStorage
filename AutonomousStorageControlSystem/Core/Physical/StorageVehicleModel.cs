using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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
        public Vector2 Location { get; set; }
        public Vector2 Velocity => new Vector2
        (
            (float)(Math.Cos(Orientation) * Speed),
            (float)(Math.Sin(Orientation) * Speed)
        );


    }
}
