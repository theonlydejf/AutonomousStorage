using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VehicleTerminal
{
    public class TextBoxBasicReader
    {
        private bool reading = false;
        public TextBoxBasicReader(TextBox txt, TextWriter writer)
        {
            this.writer = writer;
            this.txt = txt;
            txt.KeyPress += Txt_KeyPress;
            txt.KeyDown += Txt_KeyDown;
            txt.LostFocus += Txt_LostFocus;
        }

        private void Txt_LostFocus(object sender, EventArgs e)
        {
            if (reading)
                txt.Focus();
        }

        private void Txt_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                enterPressed.Set();
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private readonly TextWriter writer;
        private readonly ManualResetEvent enterPressed = new ManualResetEvent(false);
        private readonly ManualResetEvent charEntered = new ManualResetEvent(false);

        private TextBox txt;
        private char lastChar;
        private void Txt_KeyPress(object sender, KeyPressEventArgs e)
        {
            lastChar = e.KeyChar;
            charEntered.Set();
        }

        public async Task<char> Read()
        {
            if (reading)
                await Task.Run(() => { while (reading) {} });

            txt.Focus();
            reading = true;
            charEntered.Reset();
            await Task.Factory.StartNew(() => charEntered.WaitOne());
            txt.Clear();
            writer.Write(lastChar);
            reading = false;
            return lastChar;
        }

        public async Task<string> ReadLine()
        {
            if (reading)
                await Task.Run(() => { while (reading) { } });

            txt.Focus();
            reading = true;
            enterPressed.Reset();
            await Task.Factory.StartNew(() => enterPressed.WaitOne());
            string text = txt.Text;
            writer.WriteLine(text);

            txt.Clear();
            txt.Focus();
            reading = false;
            return text;
        }
    }
}