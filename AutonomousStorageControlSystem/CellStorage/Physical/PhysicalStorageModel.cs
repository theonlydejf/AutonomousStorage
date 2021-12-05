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
        public PhysicalStorageCellModel[] StorageCells { get; set; }
        public StorageVehicleModel[] StorageVehicles { get; set; }
    }
}
