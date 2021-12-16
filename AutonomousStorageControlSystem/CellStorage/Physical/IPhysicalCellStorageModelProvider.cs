using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Team.HobbyRobot.ASCS.CellStorage.Physical
{
    /// <summary>
    /// Provides a method, that can create an instance of PhysicalCellStorageModel
    /// </summary>
    public interface IPhysicalCellStorageModelProvider
    {
        PhysicalCellStorageModel CreateCellStorageModel();
    }
}
