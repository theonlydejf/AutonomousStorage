using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Team.HobbyRobot.ASCS.Core.Physical;

namespace Team.HobbyRobot.ASCS.CellStorage.Physical
{
    public class PhysicalCellStorageModel
    {
        /// <summary>
        /// Physical models of all storage cells present in the storage
        /// </summary>
        public PhysicalStorageCellModel[] StorageCells { get; set; }

        /// <summary>
        /// Physical models of all available storage vehicles
        /// </summary>
        public StorageVehicleModel[] StorageVehicles { get; set; }
    }
}
