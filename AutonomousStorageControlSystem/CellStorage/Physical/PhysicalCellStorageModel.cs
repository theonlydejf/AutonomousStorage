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

        /// <summary>
        /// Map between vahicle ID and its corresponding index in StorageVehicles array
        /// </summary>
        private Dictionary<int, int> vehicle_IDtoIdx = new Dictionary<int, int>();

        /// <summary>
        /// Map between cell ID and its corresponding index in StorageCells array
        /// </summary>
        private Dictionary<int, int> cell_IDtoIdx = new Dictionary<int, int>();

        /// <summary>
        /// Creates an innstance of PhysicalCellStorageModel
        /// </summary>
        /// <param name="storageCells">Physical models of all storage cells present in the storage</param>
        /// <param name="storageVehicles">Physical models of all available storage vehicles</param>
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

        /// <summary>
        /// Gets cells index in the StorageCells array by its ID
        /// </summary>
        /// <param name="id">Id of the cell</param>
        /// <returns>Cells index in the StorageCells array</returns>
        public int IndexOfCell(int id) => cell_IDtoIdx[id];

        /// <summary>
        /// Gets vehicles index in the StorageVehicles array by its ID
        /// </summary>
        /// <param name="id">Id of the cell</param>
        /// <returns>Cells index in the StorageVehicles array</returns>
        public int IndexOfVehicle(int id) => vehicle_IDtoIdx[id];

        /// <summary>
        /// Gets cell model, that is presnnt in the storage, by its ID
        /// </summary>
        /// <param name="id">ID of the cell</param>
        /// <returns>PhysicalCellModel that corresponds to the desired ID</returns>
        public PhysicalStorageCellModel GetCellByID(int id) => StorageCells[IndexOfCell(id)];

        /// <summary>
        /// Gets vehicle model, that is presnnt in the storage, by its ID
        /// </summary>
        /// <param name="id">ID of the vehicle</param>
        /// <returns>PhysicalVehicleModel that corresponds to the desired ID</returns>
        public PhysicalStorageVehicleModel GetVehicleByID(int id) => StorageVehicles[IndexOfVehicle(id)];

        /// <summary>
        /// Checks if the storage contains a cell with a specific ID
        /// </summary>
        /// <param name="id">ID of the cell</param>
        /// <returns>True, if the storage contains a cell with with the desired ID</returns>
        public bool ContainsCell(int id) => cell_IDtoIdx.ContainsKey(id);

        /// <summary>
        /// Checks if the storage contains a vehicle with a specific ID
        /// </summary>
        /// <param name="id">ID of the vehicle</param>
        /// <returns>True, if the storage contains a vehicle with with the desired ID</returns>
        public bool ContainsVehicle(int id) => vehicle_IDtoIdx.ContainsKey(id);
    }
}
