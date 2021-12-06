using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Team.HobbyRobot.ASCS.Core.Physical;

namespace Team.HobbyRobot.ASCS.CellStorage.Physical
{
    /// <summary>
    /// Model of the whole storage
    /// </summary>
    public class PhysicalCellStorageModel
    {
        /// <summary>
        /// Array, which stores models of all storage cells in the current storage model
        /// </summary>
        public PhysicalStorageCellModel[] StorageCells { get; set; }
        /// <summary>
        /// Array, which stores models of all storage vehicles in the current storage model
        /// </summary>
        public StorageVehicleModel[] StorageVehicles { get; set; }
        /// <summary>
        /// Properties of the current storage model
        /// </summary>
        public PhysicalCellStorageProperties Properties { get; set; }
    }
}
