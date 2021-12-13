using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Team.HobbyRobot.ASCS.CellStorage.Physical
{
    public interface IPhysicalCellStorageModelProvider
    {
        PhysicalCellStorageModel CreateCellStorageModel();
    }
}
