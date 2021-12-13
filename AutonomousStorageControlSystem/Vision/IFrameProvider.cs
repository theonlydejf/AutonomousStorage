using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Team.HobbyRobot.ASCS.Vision
{
    /// <summary>
    /// Represents the method that handles ASCS.Vision.IFrameProvider.FrameRecieved event
    /// </summary>
    /// <param name="sender">The source of the event</param>
    /// <param name="e">An instance of ASCS.Vision.FrameRecievedEventArgs that contains the event data</param>
    public delegate void FrameRecievedEventHandler(object sender, FrameRecievedEventArgs e);

    /// <summary>
    /// Provides data for ASCS.Vision.IFrameProvider.FrameRecieved event
    /// </summary>
    public class FrameRecievedEventArgs : EventArgs
    {
        /// <summary>
        /// Image that the recieved frame contains
        /// </summary>
        public Bitmap FrameImage { get; set; }

        /// <summary>
        /// Time and date of when the frame was recieved
        /// </summary>
        public DateTime Timestamp { get; set; }
    }

    /// <summary>
    /// Provides a interface between any camera and ASCS
    /// </summary>
    public interface IFrameProvider
    {
        /// <summary>
        /// The latest recieved frame
        /// </summary>
        Bitmap LatestFrame { get; }
        /// <summary>
        /// Event that occurs when a new frame is recieved from a device
        /// </summary>
        event FrameRecievedEventHandler FrameRecieved;
    }
}
