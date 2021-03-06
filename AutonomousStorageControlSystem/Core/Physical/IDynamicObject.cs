using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Team.HobbyRobot.ASCS.Core.Physical
{
    /// <summary>
    /// Moving Object
    /// </summary>
    interface IDynamicObject : IStaticObject
    {
        /// <summary>
        /// The speed at which the object moves
        /// </summary>
        public float Speed { get; set; }

        /// <summary>
        /// The speed and the direction at which the object moves
        /// </summary>
        public Vector2 Velocity { get; }
    }
}
