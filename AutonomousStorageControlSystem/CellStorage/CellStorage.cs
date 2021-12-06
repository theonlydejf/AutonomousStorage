using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Team.HobbyRobot.ASCS.CellStorage.Physical;
using Team.HobbyRobot.ASCS.Core;

namespace Team.HobbyRobot.ASCS.CellStorage
{
    public class CellStorage : IStorage
    {
        public CellStorage(PhysicalCellStorageModel storageModel)
        {
            this.storageModel = storageModel;
            storageCells = new IStorageItem[storageModel.StorageCells.Length];
            for (int i = 0; i < Capacity; i++)
                storageCells[i] = null;
        }

        /// <summary>
        /// Physical model of the storage
        /// </summary>
        private readonly PhysicalCellStorageModel storageModel;

        /// <summary>
        /// Represents contents of the storage
        /// </summary>
        private readonly IStorageItem[] storageCells;

        public IStorageItem this[int index] { get => storageCells[index]; set => storageCells[index] = value; }

        public IList<IStorageItem> Contents => (from item in storageCells
                                               where item != null
                                               select item).ToList();

        public int Capacity => storageModel.StorageCells.Length;

        public int Count => storageCells.Count(x => x != null);

        public IEnumerator<IStorageItem> GetEnumerator() => Contents.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
