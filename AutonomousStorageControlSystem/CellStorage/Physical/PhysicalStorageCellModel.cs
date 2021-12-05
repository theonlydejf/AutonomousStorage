using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Team.HobbyRobot.ASCS.Core;
using Team.HobbyRobot.ASCS.Core.Physical;

namespace Team.HobbyRobot.ASCS.CellStorage.Physical
{
    /// <summary>
    /// Model of the storage cell
    /// </summary>
    public struct PhysicalStorageCellModel : IStaticObject
    {
        /// <summary>
        /// Properties of the storage model
        /// </summary>
        public PhysicalCellStorageProperties Properties { get; set; }

        public double Orientation { get; set; }
        public PointF Location { get; set; }
        /// <summary>
        /// Approach point associated with the storage cell
        /// </summary>
        public PointF ApproachPoint => Location - new SizeF
        (
            (float)(Math.Cos(Orientation) * Properties.ApproachPointDistance),
            (float)(Math.Sin(Orientation) * Properties.ApproachPointDistance)
        );
    }
}
