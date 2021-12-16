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
            points = corners;
            if (corners != null)
                Homography = ComputeHomographyOpencv(corners);
            else
                Homography = null;
            if (corners != null)
                center = FindCentroid(corners);
            else
                center = new Point(float.NaN, float.NaN);
            if(rotation != 0)
            {
                int firstIdx = 4 - rotation;
                Point[] pts = new Point[4];
                for (int i = 0; i < 4; i++)
                {
                    int idx = firstIdx + i;
                    if (idx >= 4)
                        idx -= 4;
                    pts[i] = corners[idx];
                }
                points = pts;
            }
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
        private Point[] points;
        public Mat Homography { get; private set; }
        public Point[] Corners { get => points; set { points = value; Homography = ComputeHomographyOpencv(); } }
        private Point center;
        public Point Center => center;
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

        private Point FindCentroid() => FindCentroid(points);

        private static Point FindCentroid(Point[] points)
        {
            // Add the first point at the end of the array.
            int num_points = points.Length;
            Point[] pts = new Point[num_points + 1];
            for (int i = 0; i < points.Length; i++)
            {
                pts[i] = new Point(points[i].X, points[i].Y);
            }
            pts[num_points] = new Point(points[0].X, points[0].Y);

            // Find the centroid.
            float X = 0;
            float Y = 0;
            float second_factor;
            for (int i = 0; i < num_points; i++)
            {
                second_factor =
                    pts[i].X * pts[i + 1].Y -
                    pts[i + 1].X * pts[i].Y;
                X += (pts[i].X + pts[i + 1].X) * second_factor;
                Y += (pts[i].Y + pts[i + 1].Y) * second_factor;
            }

            float area = 0;
            for (int i = 0; i < num_points; i++)
            {
                area +=
                    (pts[i + 1].X - pts[i].X) *
                    (pts[i + 1].Y + pts[i].Y) / 2;
            }

            // Divide by 6 times the polygon's area.
            X /= (6 * area);
            Y /= (6 * area);

            // If the values are negative, the polygon is
            // oriented counterclockwise so reverse the signs.
            if (X < 0)
            {
                X = -X;
                Y = -Y;
            }

            return new Point(X, Y);
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