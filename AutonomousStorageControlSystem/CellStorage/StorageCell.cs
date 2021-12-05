using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Team.HobbyRobot.ASCS.Core;

namespace Team.HobbyRobot.ASCS.CellStorage
{
    public struct StorageCell
    {
        public bool IsOccupied { get; set; }
        public IStorageItem Content { get; set; }
    }
}
