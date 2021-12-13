using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Team.HobbyRobot.ApriltagSharp;
using Team.HobbyRobot.ASCS.Vision;
using System.Threading;
using cv = OpenCvSharp;

namespace Team.HobbyRobot.ASCS.ApriltagModeling
{
    static class ApriltagUtils
    {
        [ThreadStatic] private static int remainingSamples = 0;
        [ThreadStatic] private static List<Bitmap> frameQueue;
        [ThreadStatic] private static EventWaitHandle sampleWaitHandle = new AutoResetEvent(false);
        // TODO: Check functionality of DetectApriltagsFromSamples
        public static IList<Apriltag> DetectApriltagsFromSamples(int sampleCnt, IFrameProvider frameProvider, ApriltagDetector detector)
        {
            frameQueue = new List<Bitmap>();

            // Wait until we get all samples
            remainingSamples = sampleCnt;
            frameProvider.FrameRecieved += SampleDetected;
            sampleWaitHandle.WaitOne();

            IList<Apriltag>[] detections = new IList<Apriltag>[sampleCnt];
            Parallel.For(0, sampleCnt, i => detections[i] = detector.Detect(cv::Extensions.BitmapConverter.ToMat(frameQueue[0])));
            IList<Apriltag> result = detections[0];
            for (int i = 1; i < detections.Length; i++)
            {
                result = result.Union(detections[i]).ToList();
            }

            return result;
        }

        private static void SampleDetected(object sender, FrameRecievedEventArgs e)
        {
            if (!(sender is IFrameProvider))
                throw new NotImplementedException("Only IFrameProvider is supported");
            IFrameProvider _sender = sender as IFrameProvider;

            // Check if all samples have been taken
            if(remainingSamples >= 0)
            {
                _sender.FrameRecieved -= SampleDetected;
                remainingSamples = 0;
                sampleWaitHandle.Set();
                return;
            }

            frameQueue.Add(e.FrameImage);
            remainingSamples--;
        }
    }
}
