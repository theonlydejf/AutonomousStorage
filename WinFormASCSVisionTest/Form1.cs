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
        public static readonly ApriltagDetector detector = new ApriltagDetector(new Tag25h9Family());

        private FilterInfoCollection filterInfoCollection;
        private VideoCaptureDevice videoCaptureDevice;

        private IFrameProvider frameProvider = null;
        IList<Apriltag> snapshotTags = new List<Apriltag>();

        IList<Apriltag> frameTags = new List<Apriltag>();

        Vector2 fieldSize = new Vector2(235, 114);

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

        private void InitVideoDeviceList()
        {
            videoDeviceCombo.Items.Clear();
            foreach (FilterInfo item in filterInfoCollection)
                videoDeviceCombo.Items.Add(item);
            videoDeviceCombo.SelectedIndex = 0;
        }

        private void startVideoBtn_Click(object sender, EventArgs e)
        {
            videoCaptureDevice = new VideoCaptureDevice((videoDeviceCombo.SelectedItem as FilterInfo).MonikerString);
            if(videoDeviceSettingsList.SelectedIndex >= 0 && videoDeviceSettingsList.SelectedIndex < videoCaptureDevice.VideoCapabilities.Length)
                videoCaptureDevice.VideoResolution = videoCaptureDevice.VideoCapabilities[videoDeviceSettingsList.SelectedIndex];

            frameProvider = new AForgeFrameProvider(videoCaptureDevice);
            frameProvider.FrameRecieved += FrameProvider_FrameRecieved;

            videoCaptureDevice.Start();
            videoSettingsGroup.Enabled = false;
        }

        CalibrationRectangle calibrationRectangle = null;
        private void FrameProvider_FrameRecieved(object sender, FrameRecievedEventArgs e)
        {
            frameTags = detector.Detect(cv::Extensions.BitmapConverter.ToMat(e.FrameImage));
            calibrationRectangle = CalibrationRectangle.Create(frameTags, fieldSize);
            videoBox.Invalidate();
        }

        private void videoDeviceCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            videoCaptureDevice = new VideoCaptureDevice((videoDeviceCombo.SelectedItem as FilterInfo).MonikerString);
            videoDeviceSettingsList.Items.Clear();
            foreach (var capabilities in videoCaptureDevice.VideoCapabilities)
                videoDeviceSettingsList.Items.Add($"{ capabilities.FrameSize.Width }x{ capabilities.FrameSize.Height }\t{ capabilities.AverageFrameRate }fps");
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            videoCaptureDevice.SignalToStop();
            videoCaptureDevice.WaitForStop();
        }

        private void videoBox_Paint(object sender, PaintEventArgs e)
        {
            Control box = sender as Control;
            Graphics g = e.Graphics;

            if (frameProvider == null)
            {
                PaintError(g, "No device!", box);
                return;
            }
            try
            {
                if(frameProvider.LatestFrame == null)
                {
                    PaintError(g, "Waiting for first frame...", box);
                    return;
                }

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
                    g.DrawImage(frame, 0, 0);
                    Apriltag? tag0 = null;
                    foreach (Apriltag tag in frameTags)
                    {
                        if (tag.ID == 0)
                            tag0 = tag;
                        Point[] pts = tag.Corners.Select(pt => new Point(pt.X, pt.Y)).ToArray();
                        g.FillPolygon(new SolidBrush(Color.FromArgb(128, Color.Magenta)), pts);
                        g.DrawLine(new Pen(Color.Lime, 5), tag.Center.X, tag.Center.Y, (pts[0].X + pts[1].X) / 2, (pts[0].Y + pts[1].Y) / 2);
                        g.DrawString(tag.ID.ToString(), new Font("Arail", 10, FontStyle.Bold), Brushes.Yellow, tag.Center.X, tag.Center.Y);
                    }

                    if (calibrationRectangle != null)
                    {
                        calibrationRectangle.Paint(g, tag0 == null ? null : tag0.Value.Center);
                        if(tag0.HasValue)
                        {
                            Vector2 map = calibrationRectangle.MapPoint(tag0.Value.Center);
                            float width = 300;
                            float height = width / fieldSize.X * fieldSize.Y;
                            g.FillRectangle(Brushes.White, 0, box.Height - height - 1, width, height);
                            g.DrawRectangle(Pens.Black, 0, box.Height - height - 1, width, height);
                            g.FillEllipse(Brushes.Black, width * map.X - 2.5f, box.Height - height + height * map.Y - 2.5f - 1, 5, 5);
                        }
                    }
                }     
            }
            catch (InvalidOperationException)
            {
                g.ResetTransform();
                PaintError(g, "Operation exception thrown", box);
                return;
            }
        }

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

    public class AForgeFrameProvider : IFrameProvider
    {
        private VideoCaptureDevice videoCaptureDevice;
        private Bitmap latestFrame;

        public AForgeFrameProvider(VideoCaptureDevice videoCaptureDevice)
        {
            this.videoCaptureDevice = videoCaptureDevice;
            videoCaptureDevice.NewFrame += VideoCaptureDevice_NewFrame;
            latestFrame = null;
        }

        private void VideoCaptureDevice_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            IDisposable latestTmp = latestFrame;
            latestFrame = eventArgs.Frame.Clone(new Rectangle(0, 0, eventArgs.Frame.Width, eventArgs.Frame.Height), PixelFormat.Format32bppArgb);
            if(latestTmp != null)
                latestTmp.Dispose();

            if (FrameRecieved != null)
                FrameRecieved(this, new FrameRecievedEventArgs() { FrameImage = latestFrame.Clone() as Bitmap, Timestamp = DateTime.Now });
        }

        public Bitmap LatestFrame => latestFrame;

        public event FrameRecievedEventHandler FrameRecieved;
    }
}
