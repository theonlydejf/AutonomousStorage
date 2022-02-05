using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AForge.Video;
using AForge.Video.DirectShow;
using Team.HobbyRobot.ASCS.Vision;
using Team.HobbyRobot.ASCS.ApriltagModeling;
using Team.HobbyRobot.ApriltagSharp;
using cv = OpenCvSharp;
using System.Numerics;

namespace WinFormASCSVisionTest
{
    public partial class Form1 : Form
    {
        /// <summary>
        /// A detector that is used to detect apriltags in the frame
        /// </summary>
        public static readonly ApriltagDetector detector = new ApriltagDetector(new Tag25h9Family(), ApriltagDetector.ThresholdMethod.Canny, 0.9, 100);

        /// <summary>
        /// Filter used for filtering video devices
        /// </summary>
        private FilterInfoCollection filterInfoCollection;

        /// <summary>
        /// Video device that is used to capture frames
        /// </summary>
        private VideoCaptureDevice videoCaptureDevice;

        /// <summary>
        /// A interface between videoCaptureDevice and ASCS
        /// </summary>
        private IFrameProvider frameProvider = null;

        /// <summary>
        /// All apriltags detected in the current frame
        /// </summary>
        private IList<Apriltag> frameTags = new List<Apriltag>();

        /// <summary>
        /// Size of the field
        /// </summary>
        private Vector2 fieldSize = new Vector2(2.35f, 1.14f);

        /// <summary>
        /// Calibration rectangle used to translate points from a frame to field coordinates
        /// </summary>
        private CalibrationRectangle calibrationRectangle = null;

        public Form1()
        {
            InitializeComponent();

            filterInfoCollection = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            videoCaptureDevice = new VideoCaptureDevice();

            videoDeviceCombo.DisplayMember = "Name";
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            InitVideoDeviceList();
        }

        /// <summary>
        /// Updates list of available video devices
        /// </summary>
        private void InitVideoDeviceList()
        {
            videoDeviceCombo.Items.Clear();
            foreach (FilterInfo item in filterInfoCollection)
                videoDeviceCombo.Items.Add(item);
            videoDeviceCombo.SelectedIndex = 0;
        }

        /// <summary>
        /// Starts receiving frames from the selected video device
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void startVideoBtn_Click(object sender, EventArgs e)
        {
            videoCaptureDevice = new VideoCaptureDevice((videoDeviceCombo.SelectedItem as FilterInfo).MonikerString);

            // If any selected, change video parametrs
            if(videoDeviceSettingsList.SelectedIndex >= 0 && videoDeviceSettingsList.SelectedIndex < videoCaptureDevice.VideoCapabilities.Length)
                videoCaptureDevice.VideoResolution = videoCaptureDevice.VideoCapabilities[videoDeviceSettingsList.SelectedIndex];

            // Create interface between videoCaptureDevice and ASCS
            frameProvider = new AForgeFrameProvider(videoCaptureDevice);
            frameProvider.FrameRecieved += FrameProvider_FrameRecieved;

            // Start recieving frames
            videoCaptureDevice.Start();
            videoSettingsGroup.Enabled = false;
        }

        private void FrameProvider_FrameRecieved(object sender, FrameRecievedEventArgs e)
        {
            frameTags = detector.Detect(cv::Extensions.BitmapConverter.ToMat(e.FrameImage));
            calibrationRectangle = CalibrationRectangle.Create(frameTags);
            videoBox.Invalidate();
        }

        /// <summary>
        /// When selected video device is changed, updtae video settings list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void videoDeviceCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            videoCaptureDevice = new VideoCaptureDevice((videoDeviceCombo.SelectedItem as FilterInfo).MonikerString);
            videoDeviceSettingsList.Items.Clear();
            foreach (var capabilities in videoCaptureDevice.VideoCapabilities)
                videoDeviceSettingsList.Items.Add($"{ capabilities.FrameSize.Width }x{ capabilities.FrameSize.Height }\t{ capabilities.AverageFrameRate }fps");
        }

        /// <summary>
        /// When the winform is closed, stop receiving frames from the video device
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            videoCaptureDevice.SignalToStop();
            videoCaptureDevice.WaitForStop();
        }

        private void videoBox_Paint(object sender, PaintEventArgs e)
        {
            // Variables used often
            Control box = sender as Control;
            Graphics g = e.Graphics;

            // If no divec is selected yet
            if (frameProvider == null)
            {
                PaintError(g, "No device!", box);
                return;
            }
            try
            {
                // If no frame has been received yet
                if (frameProvider.LatestFrame == null)
                {
                    PaintError(g, "Waiting for first frame...", box);
                    return;
                }

                // Clone the current frame before it is desposed
                using (Bitmap frame = frameProvider.LatestFrame.Clone() as Bitmap)
                {
                    float sx = box.Width / (float)frame.Width;
                    float sy = box.Height / (float)frame.Height;
                    if (sx < sy)
                        sy = sx;
                    else
                        sx = sy;
                    //g.TranslateTransform(box.Width / 2 - frame.Width / 2 * sx, box.Height / 2 - frame.Height / 2 * sy);
                    //g.ScaleTransform(sx, sy);

                    // Draw the frame tothe background
                    g.DrawImage(frame, 0, 0);

                    Apriltag? tag0 = null;
                    // Draw all the detected apriltags
                    foreach (Apriltag tag in frameTags)
                    {
                        if (tag.ID == 0)
                            tag0 = tag;
                        // Convert OpenCV Points to System.Drawing.Point
                        Point[] pts = tag.Corners.Select(pt => new Point(pt.X, pt.Y)).ToArray();

                        // Draw the face of the apriltag
                        g.FillPolygon(new SolidBrush(Color.FromArgb(128, Color.Magenta)), pts);
                        // Draw the line pointg to the top of the apriltag (shows the orientation)
                        g.DrawLine(new Pen(Color.Lime, 5), tag.Center.X, tag.Center.Y, (pts[0].X + pts[1].X) / 2, (pts[0].Y + pts[1].Y) / 2);
                        // Draw the ID of the apriltag
                        g.DrawString(tag.ID.ToString(), new Font("Arail", 10, FontStyle.Bold), Brushes.Yellow, tag.Center.X, tag.Center.Y);
                    }

                    // If a calibration rectangle can be created
                    if (calibrationRectangle != null)
                    {
                        // Draw the outline of the rectangle
                        calibrationRectangle.Paint(g, tag0 == null ? null : tag0.Value.Center);
                        // If a tag with ID=0 is present in the frame
                        if(tag0.HasValue)
                        {
                            // Map a point to the field
                            Vector2 map = calibrationRectangle.MapPoint(tag0.Value.Center);
                            // Calculate the dimension of the scaled field
                            float width = 300;
                            float height = width / fieldSize.X * fieldSize.Y;

                            // Background of the field in the bottom left
                            g.FillRectangle(Brushes.White, 0, box.Height - height - 1, width, height);
                            g.DrawRectangle(Pens.Black, 0, box.Height - height - 1, width, height);

                            // Draws the point representing the tag0
                            g.FillEllipse(Brushes.Black, width * map.X - 2.5f, box.Height - height + height * map.Y - 2.5f - 1, 5, 5);
                        }
                    }
                }     
            }
            // When the frame is already disposed or when any other multithread exception occures
            catch (InvalidOperationException)
            {
                g.ResetTransform();
                PaintError(g, "Operation exception thrown", box);
                return;
            }
        }

        /// <summary>
        /// Draw a error message onto any Graphics object
        /// </summary>
        /// <param name="g">Graphics the error will be drawn to</param>
        /// <param name="msg">Message that the error shows</param>
        /// <param name="control">Control onto which the error will be drawn</param>
        private void PaintError(Graphics g, string msg, Control control)
        {
            using (Font f = new Font("Consolas", 30))
            {
                string errMsg = "No Device!";
                SizeF txtSize = g.MeasureString(errMsg, f);
                g.DrawString(errMsg, f, Brushes.Red, control.Width / 2 - txtSize.Width / 2, control.Height / 2 - txtSize.Height / 2);
            }
        }

        private void refreshVideDevicesBtn_Click(object sender, EventArgs e)
        {
            InitVideoDeviceList();
        }
    }

    /// <summary>
    /// Interface between AForge.Video.DirectShow.VideoCaptureDevice and ASCS
    /// </summary>
    public class AForgeFrameProvider : IFrameProvider
    {
        /// <summary>
        /// VideoCaptureDevice that supplies frames
        /// </summary>
        private VideoCaptureDevice videoCaptureDevice;
        /// <summary>
        /// Latest frame recieved from the videoCaptureDevice
        /// </summary>
        private Bitmap latestFrame;

        public Bitmap LatestFrame => latestFrame;

        public event FrameRecievedEventHandler FrameRecieved;

        public AForgeFrameProvider(VideoCaptureDevice videoCaptureDevice)
        {
            this.videoCaptureDevice = videoCaptureDevice;
            videoCaptureDevice.NewFrame += VideoCaptureDevice_NewFrame;
            latestFrame = null;
        }

        /// <summary>
        /// Forwards the recieved frame to FrameRecieved event and corrects the PixelFormat
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void VideoCaptureDevice_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            // Updates the LatestFrame and disposes the old frame
            IDisposable latestTmp = latestFrame;
            latestFrame = eventArgs.Frame.Clone(new Rectangle(0, 0, eventArgs.Frame.Width, eventArgs.Frame.Height), PixelFormat.Format32bppArgb);
            if(latestTmp != null)
                latestTmp.Dispose();

            if (FrameRecieved != null)
                FrameRecieved(this, new FrameRecievedEventArgs() { FrameImage = latestFrame.Clone() as Bitmap, Timestamp = DateTime.Now });
        }
    }
}
