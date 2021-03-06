using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Team.HobbyRobot.ASCS.Core.Physical
{
    /// <summary>
    /// Stationary object
    /// </summary>
    interface IStaticObject
    {
        /// <summary>
        /// Orientation of the object
        /// </summary>
        double Orientation { get; set; }
        /// <summary>
        /// Location of the object
        /// </summary>
        Vector2 Location { get; set; }
    }
}
