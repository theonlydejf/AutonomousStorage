using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Team.HobbyRobot.ASCS.CellStorage.Physical;
using Team.HobbyRobot.ASCS.Vision;
using Team.HobbyRobot.ApriltagSharp;
using cv = OpenCvSharp;

namespace Team.HobbyRobot.ASCS.ApriltagModeling
{
    class ApriltagCellStorageProvider : IPhysicalCellStorageModelProvider
    {
        private IFrameProvider frameProvider;
        private ApriltagDetector apriltagDetector;
        private int frameSampleCnt;


        // TODO: implement CreateCellStorageModel
        public PhysicalCellStorageModel CreateCellStorageModel()
        {
            // Zkontrolovat jestli to funguje xd
            IList<Apriltag> foundTags = ApriltagUtils.DetectApriltagsFromSamples(frameSampleCnt, frameProvider, apriltagDetector);
            throw new NotImplementedException();
        }
    }
}
