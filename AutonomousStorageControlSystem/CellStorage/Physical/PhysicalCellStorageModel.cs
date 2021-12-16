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
        public PhysicalStorageVehicleModel[] StorageVehicles { get; set; }

        private Dictionary<int, int> vehicle_IDtoIdx = new Dictionary<int, int>();
        private Dictionary<int, int> cell_IDtoIdx = new Dictionary<int, int>();

        public PhysicalCellStorageModel(PhysicalStorageCellModel[] storageCells, PhysicalStorageVehicleModel[] storageVehicles)
        {
            StorageCells = storageCells;
            StorageVehicles = storageVehicles;
            int i = 0;
            foreach (var cell in storageCells)
                cell_IDtoIdx.Add(cell.ID, i++);
            i = 0;
            foreach (var vehicle in StorageVehicles)
                vehicle_IDtoIdx.Add(vehicle.ID, i++);
        }

        public int IndexOfCell(int id) => cell_IDtoIdx[id];
        public int IndexOfVehicle(int id) => vehicle_IDtoIdx[id];
        public PhysicalStorageCellModel GetCellByID(int id) => StorageCells[IndexOfCell(id)];
        public PhysicalStorageVehicleModel GetVehicleByID(int id) => StorageVehicles[IndexOfVehicle(id)];
        public bool ContainsCell(int id) => cell_IDtoIdx.ContainsKey(id);
        public bool ContainsVehicle(int id) => vehicle_IDtoIdx.ContainsKey(id);
    }
}
