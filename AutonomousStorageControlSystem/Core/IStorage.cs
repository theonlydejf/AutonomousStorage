using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Team.HobbyRobot.ASCS.Core
{
    public interface IStorage : IEnumerable<IStorageItem>, IEnumerable
    {
        IStorageItem this[int ID] { get; set; }

        /// <summary>
        /// All contents currently in the storage
        /// </summary>
        IList<IStorageItem> Contents { get; }

        /// <summary>
        /// Capacity of this storage
        /// </summary>
        int Capacity { get; }

        /// <summary>
        /// Amount of item currently stored in this storage
        /// </summary>
        int Count { get; }
    }
}
