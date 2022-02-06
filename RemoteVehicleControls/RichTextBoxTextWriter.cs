using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Team.HobbyRobot.ASCS.RemoteVehicleControls
{
    internal class RichTextBoxTextWriter : TextWriter
    {

        #region Fields

        private Int32 _bufferSize = 10000;
        private List<RichTextBox> _rtbOutputs = new List<RichTextBox>(1);

        #endregion

        #region Public Property: Encoding

        public override Encoding Encoding
        {
            get
            {
                return System.Text.Encoding.Default;
            }
        }

        #endregion

        #region Public Properties

        public Int32 BufferSize
        {
            get
            {

                return this._bufferSize;
            }
            set
            {
                this._bufferSize = value;
            }
        }

        #endregion

        #region Public Method: AttachRtbOutput

        public void AttachRtbOutput(RichTextBox rtbOutput)
        {
            this._rtbOutputs.Add(rtbOutput);
        }

        #endregion

        #region Public Method: DetachRtbOutput

        public void DetachRtbOutput(RichTextBox rtbOutput)
        {
            this._rtbOutputs.Remove(rtbOutput);
        }

        #endregion

        #region Public Method: WriteLine, Write

        public override void WriteLine(string format, params object[] arg)
        {
            this.Write(String.Format(format, arg) + System.Environment.NewLine);
        }

        public override void WriteLine(string value)
        {
            this.Write(value + System.Environment.NewLine);
        }

        public override void Write(string format, params object[] arg)
        {
            this.Write(String.Format(format, arg));
        }

        public override void Write(string value)
        {
            foreach (RichTextBox rtbOutput in this._rtbOutputs)
            {
                if (rtbOutput.InvokeRequired)
                {
                    rtbOutput.Invoke(
                        new WriteMessageToRtfEventHandler(WriteMessageToRtb),
                        new Object[] { rtbOutput, value }
                    );
                }
                else
                {
                    this.WriteMessageToRtb(rtbOutput, value);
                }
            }
        }

        private delegate void WriteMessageToRtfEventHandler(RichTextBox rtbOutput, String message);

        private void WriteMessageToRtb(RichTextBox rtbOutput, String message)
        {
            if (rtbOutput.Text.Length > this._bufferSize)
                rtbOutput.Text = message;
            else
                rtbOutput.Text += message;
            rtbOutput.SelectionStart = rtbOutput.Text.Length;
        }

        #endregion

    }
}
