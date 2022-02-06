using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Timers = System.Timers;

namespace Team.HobbyRobot.ASCS.RemoteVehicleControls
{
    public class ConsoleOutput : RichTextBox
    {
        public TextWriter TextWriter => writer;
        private RichTextBoxTextWriter writer;

        public ConsoleOutput()
        {
            ReadOnly = true;
            BackColor = Color.Black;
            ForeColor = Color.White;
            Font = new Font("Consolas", Font.Size);

            TextChanged += Console_TextChanged;
            writer = new RichTextBoxTextWriter();
            writer.AttachRtbOutput(this);

        }

        private void Console_TextChanged(object sender, EventArgs e)
        {
            SelectionStart = Text.Length;
            ScrollToCaret();
        }
    }
}
