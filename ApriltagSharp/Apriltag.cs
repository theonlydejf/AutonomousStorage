using System;
using System.Collections;
using System.Linq;
using System.Text.RegularExpressions;
using OpenCvSharp;
namespace Team.HobbyRobot.ApriltagSharp
{
    public struct Apriltag
    {
        public Apriltag(bool good, long obsCode, long matchCode, int id, int hammingDistance, int rotation, Point[] corners)
        {
            IsGood = good;
            ObservedCode = obsCode;
            MatchCode = matchCode;
            ID = id;
            HammingDistance = hammingDistance;
            Orientation = rotation;
            points = corners;
            if (corners != null)
                Homography = ComputeHomographyOpencv(corners);
            else
                Homography = null;
        }

        /// <summary>
        /// Whether the hamming distance is within the margin of error
        /// </summary>
        public bool IsGood { get; }
        /// <summary>
        /// Observed Code
        /// </summary>
        public long ObservedCode { get; }
        /// <summary>
        /// Coresponding code to the observed code
        /// </summary>
        public long MatchCode { get; }
        /// <summary>
        /// ID of the tag
        /// </summary>
        public int ID { get; }
        /// <summary>
        /// Hamming distance between Observed code and matched code
        /// </summary>
        public int HammingDistance { get; }
        public int Orientation { get; }
        private Point[] points;
        public Mat Homography { get; private set; }
        public Point[] Corners { get => points; set { points = value; Homography = ComputeHomographyOpencv(); } }

        /// <summary>
        /// 得到旋转矩阵，需要使用opencv
        /// </summary>
        private static Mat ComputeHomographyOpencv(Point[] corners)
        {
            Point2d[] src = {
                new Point2d (-1, -1),
                new Point2d (1, -1),
                new Point2d (1, 1),
                new Point2d (-1, 1)
            };

            Point2d[] dst = corners.Select(x => new Point2d(x.X, x.Y)).ToArray(); //ConvertPoint2Point2D();
            return Cv2.FindHomography(src, dst);
        }

        private Mat ComputeHomographyOpencv() => ComputeHomographyOpencv(Corners);

        public override bool Equals(object obj)
        {
            return obj is Apriltag apriltag &&
                   ID == apriltag.ID;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ID);
        }
    }
}