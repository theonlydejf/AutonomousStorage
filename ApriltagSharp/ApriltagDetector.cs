using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using OpenCvSharp;

namespace Team.HobbyRobot.ApriltagSharp
{
    public class ApriltagDetector
    {
        public enum ThresholdMethod
        {
            Adaptive,
            Canny
        }

        /// <summary>
        /// Method to threshold the picture
        /// </summary>
        public ThresholdMethod Threshold { get; }
        /// <summary>
        /// Sigma used to blur the picture (0.8 recommened)
        /// </summary>
        public double Sigma { get; }
        /// <summary>
        /// Tag family that is being used
        /// </summary>
        public TagFamily TagFamily { get; }

        private readonly bool Debug;
        private readonly int MinArea;

        /// <summary>
        /// Create instance of apriltag detector 
        /// </summary>
        /// <param name="tagFamily">Tag family which is gonna be detected</param>
        /// <param name="threshold">Edge detection method</param>
        /// <param name="sigma">Image Gaussian blur value sigma</param>
        /// <param name="minarea">Smallest area for a tag to be considered valid</param>
        /// <param name="debug">Is the detector in debug mode?</param>
        public ApriltagDetector(TagFamily tagFamily, ThresholdMethod threshold = ThresholdMethod.Canny, double sigma = 0.8, int minarea = 400, bool debug = false)
        {
            Threshold = threshold;
            Debug = debug;
            Sigma = sigma;
            MinArea = minarea;
            TagFamily = tagFamily;
        }
        /// <summary>
        /// Carry out point mapping, according to the four points in p to map the relativeeePoint into it
        /// </summary>
        /// <param name="quad">quad</param>
        /// <param name="relativePoint">Point to be mapped</param>
        /// <returns>Mapped point</returns>
        private Point Interpolate(Point[] quad, Point2d relativePoint)
        {
            double tmp0 = quad[1].X * quad[2].Y;
            double tmp1 = quad[2].X * quad[1].Y;
            double tmp2 = tmp0 - tmp1;
            double tmp3 = quad[1].X * quad[3].Y;
            double tmp4 = tmp2 - tmp3;
            double tmp5 = quad[3].X * quad[1].Y;
            double tmp6 = quad[2].X * quad[3].Y;
            double tmp7 = quad[3].X * quad[2].Y;
            double tmp8 = tmp4 + tmp5 + tmp6 - tmp7;
            double tmp9 = quad[0].X * quad[2].X;
            double tmp10 = tmp9 * quad[1].Y;
            double tmp11 = quad[1].X * quad[2].X;
            double tmp12 = quad[0].X * quad[3].X;
            double tmp13 = quad[1].X * quad[3].X;
            double tmp14 = tmp13 * quad[0].Y;
            double tmp15 = tmp9 * quad[3].Y;
            double tmp16 = tmp13 * quad[2].Y;
            double tmp17 = tmp10 - tmp11 * quad[0].Y - tmp12 * quad[1].Y + tmp14 - tmp15 + tmp12 * quad[2].Y + tmp11 * quad[3].Y - tmp16;
            double tmp18 = quad[0].X * quad[1].X;
            double tmp19 = quad[2].X * quad[3].X;
            double tmp20 = tmp18 * quad[2].Y - tmp10 - tmp18 * quad[3].Y + tmp14 + tmp15 - tmp19 * quad[0].Y - tmp16 + tmp19 * quad[1].Y;
            double tmp21 = quad[0].X * quad[1].Y;
            double tmp22 = quad[1].X * quad[0].Y;
            double tmp23 = tmp22 * quad[2].Y;
            double tmp24 = tmp21 * quad[3].Y;
            double tmp25 = quad[2].X * quad[0].Y;
            double tmp26 = quad[3].X * quad[0].Y;
            double tmp27 = tmp26 * quad[2].Y;
            double tmp28 = tmp1 * quad[3].Y;
            double tmp29 = tmp21 * quad[2].Y - tmp23 - tmp24 + tmp22 * quad[3].Y - tmp25 * quad[3].Y + tmp27 + tmp28 - tmp5 * quad[2].Y;
            double tmp30 = quad[0].X * quad[2].Y;
            double tmp31 = tmp23 - tmp25 * quad[1].Y - tmp24 + tmp26 * quad[1].Y + tmp30 * quad[3].Y - tmp27 - tmp0 * quad[3].Y + tmp28;
            double tmp32 = quad[0].X * quad[3].Y;
            double tmp33 = tmp30 - tmp25 - tmp32 - tmp0 + tmp1 + tmp26 + tmp3 - tmp5;
            double tmp34 = tmp21 - tmp22;
            double tmp35 = tmp34 - tmp30 + tmp25 + tmp3 - tmp5 - tmp6 + tmp7;
            double hx = (tmp17 / tmp8) * relativePoint.X - (tmp20 / tmp8) * relativePoint.Y + quad[0].X;
            double hy = (tmp29 / tmp8) * relativePoint.X - (tmp31 / tmp8) * relativePoint.Y + quad[0].Y;
            double hw = (tmp33 / tmp8) * relativePoint.X + (tmp35 / tmp8) * relativePoint.Y + 1;
            return new Point(hy / hw, hx / hw);
        }
        /// <summary>
        /// Get average of byte list
        /// </summary>
        private double Average(IList<byte> list)
        {
            int sum = 0;
            foreach (byte item in list)
                sum += item;
            return sum / list.Count;
        }

        public static int bc = 9;
        public static double c = 5;

        /// <summary>
        /// Detect tags in one frame
        /// </summary>
        /// <param name="frame">RGB frame</param>
        /// <returns>List of instances of Apriltag found in the frame</returns>
        public IList<Apriltag> Detect(Mat frame)
        {
            Mat gray = new Mat();
            Cv2.CvtColor(frame, gray, ColorConversionCodes.RGB2GRAY);
            Mat dst = new Mat();
            Mat gauss = new Mat();
            Cv2.GaussianBlur(gray, gauss, new Size(3, 3), this.Sigma);
            switch (Threshold)
            {
                case ThresholdMethod.Canny:
                    Cv2.Canny(gauss, dst, 150, 400, 3);
                    break;
                case ThresholdMethod.Adaptive:
                    Cv2.AdaptiveThreshold(gauss, dst, 255, AdaptiveThresholdTypes.GaussianC, ThresholdTypes.BinaryInv, bc, c);
                    break;
            }

            Cv2.FindContours(dst, out Point[][] contours, out HierarchyIndex[] hierarchy, OpenCvSharp.RetrievalModes.CComp, ContourApproximationModes.ApproxSimple, null);

            if (this.Debug == true)
            {
                Mat copyimg = new Mat();
                frame.CopyTo(copyimg);
                copyimg.DrawContours(contours, -1, new Scalar(0, 255, 0));
                using (new Window("contours image", copyimg))
                {
                    Cv2.WaitKey();
                }
            }
            List<Point[]> hulls = new List<Point[]>();
            List<Point[]> quads = new List<Point[]>();
            for (int i = 0; i < contours.Length; i++)
            {
                Point[] contour = contours[i];//取出多边形 get polygon
                if (contour.Length >= 4 && hierarchy[i].Previous < 0)
                {
                    double area = Cv2.ContourArea(contour);//求多边形面积 get contour`s area
                    if (area > this.MinArea)
                    {
                        Point[] hull = Cv2.ConvexHull(contour);//求出凸包 get hull
                        if ((area / Cv2.ContourArea(hull)) > 0.8)
                        {
                            hulls.Add(hull);
                            Point[] quad = Cv2.ApproxPolyDP(hull, 9, true);//根据凸包计算出四边形 get quad
                            if (quad.Length == 4)
                            {
                                double areaqued = Cv2.ContourArea(quad);
                                double areahull = Cv2.ContourArea(hull);
                                if (areaqued / areahull > 0.8 && areahull >= areaqued)
                                {
                                    quads.Add(quad);
                                }
                            }
                        }
                    }
                }
            }
            if (this.Debug == true)
            {
                Mat copyimg = new Mat();
                frame.CopyTo(copyimg);
                foreach (Point[] item in quads)
                {
                    Point[][] temp = new Point[1][];
                    temp[0] = item;
                    copyimg.DrawContours(temp, -1, new Scalar(0, 255, 0));
                }
                using (new Window("contours image", copyimg))
                {
                    Cv2.WaitKey();
                }
                Console.WriteLine("quads count" + quads.Count);
            }

            List<Apriltag> detections = new List<Apriltag>();
            List<Point> points = new List<Point>();
            List<Point> whitepoints = new List<Point>();

            //进行点quad点的提取
            foreach (Point[] quad in quads)
            {
                int dd = this.TagFamily.BlackBorderThickness * 2 + TagFamily.EdgeLength;
                List<byte> blackvalue = new List<byte>();
                List<byte> whitevalue = new List<byte>();

                for (int iy = 0; iy < dd; iy++)
                {
                    for (int ix = 0; ix < dd; ix++)
                    {
                        double x = (ix + 0.5) / (dd * 1.0);
                        double y = (iy + 0.5) / (dd * 1.0);
                        Point polatepoint = Interpolate(quad, new Point2d(x, y));
                        points.Add(polatepoint);
                        byte value = gray.At<byte>(polatepoint.X, polatepoint.Y);
                        if ((iy == 0 || iy == dd - 1) || (ix == 0 || ix == dd - 1))
                        {
                            blackvalue.Add(value);
                        }
                        else if ((iy == 1 || iy == dd - 2) || (ix == 1 || ix == dd - 2))
                        {
                            whitevalue.Add(value);
                        }
                        else
                        {
                            continue;
                        }
                    }
                }
                long tagcode = 0;
                double threshold = 0.5 * (Average(blackvalue) + Average(whitevalue));
                for (int iy = 0; iy < dd; iy++)
                {
                    for (int ix = 0; ix < dd; ix++)
                    {
                        if ((iy == 0 || iy == dd - 1) || (ix == 0 || ix == dd - 1))
                        {
                            continue;
                        }
                        double newx = (ix + 0.5) / dd * 1.0;
                        double newy = (iy + 0.5) / dd * 1.0;
                        Point point = Interpolate(quad, new Point2d(newx, newy));
                        int grayvalue = gray.At<byte>(point.X, point.Y);
                        tagcode <<= 1;
                        if (grayvalue > threshold)
                        {
                            tagcode |= 1;
                            whitepoints.Add(point);
                        }
                    }
                }

                Apriltag decoderesult = TagFamily.Decode(tagcode, quad);
                if (decoderesult.IsGood)
                    detections.Add(decoderesult);
            }
            if (this.Debug == true)
            {
                Mat copyimg = new Mat();
                frame.CopyTo(copyimg);
                foreach (Point item in points)
                {
                    Point tpoint = new Point(item.Y, item.X);
                    copyimg.Circle(tpoint, 1, new Scalar(0, 0, 255));
                }
                using (new Window("quad", copyimg))
                {
                    Cv2.WaitKey();
                }

                Mat copyimg2 = new Mat();
                frame.CopyTo(copyimg2);
                foreach (Point item in whitepoints)
                {
                    Point tpoint = new Point(item.Y, item.X);
                    copyimg2.Circle(tpoint, 1, new Scalar(0, 0, 255));
                }
                using (new Window("quad", copyimg2))
                {
                    Cv2.WaitKey();
                }
            }
            return detections;
        }
    }
}