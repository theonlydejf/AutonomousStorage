using OpenCvSharp;
using System;
using System.Collections.Generic;
using draw = System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Team.HobbyRobot.ApriltagSharp;

namespace Team.HobbyRobot.ASCS.ApriltagModeling
{
    /// <summary>
    /// A class used for calculating position on a plane thats defined by 4 points representing a rectangle
    /// </summary>
    public class CalibrationRectangle
    {
        public static CalibrationRectangleSettings Settings = new CalibrationRectangleSettings()
        {
            TopLeftID = 1,
            TopRightID = 2,
            BottomRightID = 3,
            BottomLeftID = 4
        };

        private Apriltag[] cornerTags;
        private Point[] corners;

        /// <summary>
        /// Creates an instance of CalibrationRectangle from list of tags
        /// </summary>
        /// <param name="detectedTags">List of any tags, that should contain all of the corner tags</param>
        /// <returns>An instance of CalibrationRectangle if detectedTags contain all four corners tags, or null</returns>
        public static CalibrationRectangle Create(IList<Apriltag> detectedTags)
        {
            Apriltag[] cornerTags;
            Point[] corners;
            cornerTags = new Apriltag[4];
            int found = 0;
            foreach (Apriltag tag in detectedTags)
            {
                if (found >= 4)
                    break;

                if (tag.ID == Settings.TopLeftID)
                {
                    cornerTags[0] = tag;
                    found++;
                    continue;
                }
                if (tag.ID == Settings.TopRightID)
                {
                    cornerTags[1] = tag;
                    found++;
                    continue;
                }
                if (tag.ID == Settings.BottomRightID)
                {
                    cornerTags[2] = tag;
                    found++;
                    continue;
                }
                if (tag.ID == Settings.BottomLeftID)
                {
                    cornerTags[3] = tag;
                    found++;
                    continue;
                }
            }

            if (found < 4)
                return null;

            corners = new Point[4];
            for (int i = 0; i < corners.Length; i++)
                corners[i] = cornerTags[i].Corners[i];
            return new CalibrationRectangle() { corners = corners, cornerTags = cornerTags };
        }

        /// <summary>
        /// Maps a point inside of the quadrilateral to a rectangle with sides of length 1
        /// </summary>
        /// <param name="pt">A point, that will be mapped</param>
        /// <returns>A Vector2 with mapped coordinates (ranging from 0 to 1)</returns>
        public Vector2 MapPoint(Point pt)
        {
            double topDist = FindDistanceToSegment(pt, corners[0], corners[1], out Point xTop);
            double bottomDist = FindDistanceToSegment(pt, corners[2], corners[3], out Point xBottom);
            double leftDist = FindDistanceToSegment(pt, corners[3], corners[0], out Point yLeft);
            double rightDist = FindDistanceToSegment(pt, corners[1], corners[2], out Point yRight);

            double sx = (leftDist / (leftDist + rightDist));
            double sy = (topDist / (topDist + bottomDist));
            return new Vector2((float)sx, (float)sy);
        }

        /// <summary>
        /// ONLY A TEST FUNCTION FOR VISUALISING (works only on windows!)
        /// </summary>
        /// <param name="g"></param>
        /// <param name="pos"></param>
        public void Paint(draw::Graphics g, Point? pos)
        {
            g.DrawPolygon(draw.Pens.Red, corners.Select(x => new draw.PointF(x.X, x.Y)).ToArray());
            if(pos.HasValue)
            {
                Point pt = pos.Value;
                double topDist = FindDistanceToSegment(pt, corners[0], corners[1], out Point xTop);
                double bottomDist = FindDistanceToSegment(pt, corners[2], corners[3], out Point xBottom);
                double leftDist = FindDistanceToSegment(pt, corners[3], corners[0], out Point yLeft);
                double rightDist = FindDistanceToSegment(pt, corners[1], corners[2], out Point yRight);

                g.DrawLine(draw.Pens.Red, xTop.X, xTop.Y, xBottom.X, xBottom.Y);
                g.DrawLine(draw.Pens.Lime, pt.X, pt.Y, xTop.X, xTop.Y);
                g.DrawLine(draw.Pens.ForestGreen, pt.X, pt.Y, xBottom.X, xBottom.Y);

                g.DrawLine(draw.Pens.Red, yLeft.X, yLeft.Y, yRight.X, yRight.Y);
                g.DrawLine(draw.Pens.Lime, pt.X, pt.Y, yLeft.X, yLeft.Y);
                g.DrawLine(draw.Pens.ForestGreen, pt.X, pt.Y, yRight.X, yRight.Y);
            }
        }

        // Calculate the distance between
        // point pt and the segment p1 --> p2.
        private double FindDistanceToSegment(Point pt, Point p1, Point p2, out Point closest)
        {
            float dx = p2.X - p1.X;
            float dy = p2.Y - p1.Y;
            if ((dx == 0) && (dy == 0))
            {
                // It's a point not a line segment.
                closest = p1;
                dx = pt.X - p1.X;
                dy = pt.Y - p1.Y;
                return Math.Sqrt(dx * dx + dy * dy);
            }

            // Calculate the t that minimizes the distance.
            float t = ((pt.X - p1.X) * dx + (pt.Y - p1.Y) * dy) /
                (dx * dx + dy * dy);

            closest = new Point(p1.X + t * dx, p1.Y + t * dy);

            dx = pt.X - closest.X;
            dy = pt.Y - closest.Y;

            return Math.Sqrt(dx * dx + dy * dy);
        }
    }

    /// <summary>
    /// Settings that are used to define CalibrationRectangle class
    /// </summary>
    public struct CalibrationRectangleSettings
    {
        /// <summary>
        /// ID of a tag in the top left corner of the rectangle
        /// </summary>
        public int TopLeftID { get; set; }

        /// <summary>
        /// ID of a tag in the top left corner of the rectangle
        /// </summary>
        public int TopRightID { get; set; }

        /// <summary>
        /// ID of a tag in the top left corner of the rectangle
        /// </summary>
        public int BottomRightID { get; set; }

        /// <summary>
        /// ID of a tag in the top left corner of the rectangle
        /// </summary>
        public int BottomLeftID { get; set; }
    }
}
