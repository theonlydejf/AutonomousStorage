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
        private Vector2 fieldSize;

        public static CalibrationRectangle Create(IList<Apriltag> detectedTags, Vector2 fieldSize)
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
            return new CalibrationRectangle() { corners = corners, cornerTags = cornerTags, fieldSize = fieldSize };
        }

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

        Point FindIntersection(Point s1, Point e1, Point s2, Point e2)
        {
            float a1 = e1.Y - s1.Y;
            float b1 = s1.X - e1.X;
            float c1 = a1 * s1.X + b1 * s1.Y;

            float a2 = e2.Y - s2.Y;
            float b2 = s2.X - e2.X;
            float c2 = a2 * s2.X + b2 * s2.Y;

            float delta = a1 * b2 - a2 * b1;
            //If lines are parallel, the result will be (NaN, NaN).
            return delta == 0 ? new Point(float.NaN, float.NaN)
                : new Point((b2 * c1 - b1 * c2) / delta, (a1 * c2 - a2 * c1) / delta);
        }
    }

    public struct CalibrationRectangleSettings
    {
        public int TopLeftID { get; set; }
        public int TopRightID { get; set; }
        public int BottomRightID { get; set; }
        public int BottomLeftID { get; set; }
    }
}
