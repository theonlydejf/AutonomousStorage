using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Timers =System.Timers;

namespace Team.HobbyRobot.ASCS.RemoteVehicleControls
{
    public class RemoteLoggerWatcher : RichTextBox
    {
        private StringBuilder block;
        private Stream loggerStream;
        private Timers::Timer updateTextTimer;
        public double RefreshInterval { get => updateTextTimer.Interval; set => updateTextTimer.Interval = value; }
        private Task watchLoggerTask = null;

        public RemoteLoggerWatcher()
        {
            ReadOnly = true;
            BackColor = Color.Black;
            ForeColor = Color.White;
            Font = new Font("Consolas", Font.Size);

            block = new StringBuilder();
            updateTextTimer = new Timers.Timer(100);
            updateTextTimer.Elapsed += UpdateTextTimer_Elapsed;
            TextChanged += RemoteLoggerWatcher_TextChanged;
        }

        private void RemoteLoggerWatcher_TextChanged(object sender, EventArgs e)
        {
            SelectionStart = Text.Length;
            ScrollToCaret();
        }

        private void UpdateTextTimer_Elapsed(object sender, Timers.ElapsedEventArgs e) => UpdateText();

        private void UpdateText()
        {
            Invoke((MethodInvoker)delegate ()
            {
                lock (block)
                {
                    if (block.Length <= 0)
                        return;
                    Text += block.ToString();
                    block.Clear();
                }
            });
        }

        public override void Refresh()
        {
            UpdateText();
            base.Refresh();
        }

        public void Start(Stream loggerStream)
        {
            if (watchLoggerTask != null && !watchLoggerTask.IsCompleted)
                return;

            this.loggerStream = loggerStream;
            watchLoggerTask = Task.Run(() => WatchLogger());
            updateTextTimer.Start();
        }

        private void WatchLogger()
        {
            while (true)
            {
                try
                {
                    int c = loggerStream.ReadByte();
                    if (c > -1)
                        lock (block)
                        {
                            block.Append((char)c);
                        }
                    else
                        break;
                }
                catch (Exception ex)
                {
                    Invoke((MethodInvoker)delegate ()
                    {
                        SelectionColor = Color.Red;
                        AppendText("\n\nException was thrown when reading logger: " + ex.ToString() + "\n\n");
                        SelectionColor = Color.White;
                    });
                    break;
                }
            }
            UpdateText();
            Invoke((MethodInvoker)delegate ()
            {
                SelectionColor = Color.Red;
                AppendText("\n\nRemote logger disconnected\n\n");
                SelectionColor = Color.White;
                return;
            });
        }
    }
}
