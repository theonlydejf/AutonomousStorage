using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Team.HobbyRobot.ASCS.Core.Connection;
using Team.HobbyRobot.ASCS.Core.Logging;
using Team.HobbyRobot.TDN.Base;
using Team.HobbyRobot.TDN.Core;

namespace VehicleTerminal
{
    public partial class Form1 : Form
    {
        private VehicleConnection connection = null;
        private Logger logger = new Logger();

        public Form1()
        {
            InitializeComponent();
            var localLoggerEndpoint = new RichTextBoxTextWriter();
            localLoggerEndpoint.AttachRtbOutput(LocalLoggerTxt);
            logger.RegisterEndpoint(localLoggerEndpoint);

            var tdnFactoryConsole = new RichTextBoxTextWriter();
            tdnFactoryConsole.AttachRtbOutput(TDNFactoryConsoleOutTxt);

            tdnFactory = new TDNFactory(tdnFactoryConsole, new TextBoxBasicReader(TDNFactoryConsoleInTxt, tdnFactoryConsole));

            TDNFactoryConsoleOutTxt.GotFocus += TDNFactoryConsoleOutTxt_GotFocus;

            lastFocused = PathTxt;
            PathTxt.GotFocus += Focusable_GotFocus;
            TDNFactoryConsoleInTxt.GotFocus += Focusable_GotFocus;

            LocalLoggerTxt.GotFocus += Unfocusable_GotFocus;
            RemoteLoggerTxt.GotFocus += Unfocusable_GotFocus;
        }

        Control lastFocused;
        private void Focusable_GotFocus(object sender, EventArgs e)
        {
            lastFocused = sender as Control;
        }

        private void Unfocusable_GotFocus(object sender, EventArgs e)
        {
            lastFocused.Focus();
        }

        private void TDNFactoryConsoleOutTxt_GotFocus(object sender, EventArgs e)
        {
            TDNFactoryConsoleInTxt.Focus();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ParserCombo.Items.Add("(Select in console)");
            foreach (string parser in new DefaultTDNParserSettings().Parsers.Keys)
                ParserCombo.Items.Add(parser);
            ParserCombo.SelectedIndex = 0;
        }

        private async void ConnectBtn_Click(object sender, EventArgs e)
        {
            IPTxt.Enabled = false;
            ConnectBtn.Enabled = false;
            connection = await VehicleConnection.CreateConnectionAsync(IPAddress.Parse(IPTxt.Text), logger);
            UpdateConnectionStatus(VehicleConnection.IsValid(connection));

            IPTxt.Enabled = !VehicleConnection.IsValid(connection);
            ConnectBtn.Enabled = !VehicleConnection.IsValid(connection);

            if (VehicleConnection.IsValid(connection))
            {
                _ = Task.Run(() => WatchVehicleLogger());
            }
        }

        private void WatchVehicleLogger()
        {
            StringBuilder block = new StringBuilder();
            while(true)
            {
                try
                {
                    int c = connection.LoggerStream.ReadByte();
                    if (c > -1)
                        RemoteLoggerTxt.Invoke((MethodInvoker) delegate() { RemoteLoggerTxt.AppendText(((char)c).ToString()); });
                    else
                    {
                        RemoteLoggerTxt.Invoke((MethodInvoker)delegate ()
                        {
                            RemoteLoggerTxt.SelectionColor = Color.Red;
                            RemoteLoggerTxt.AppendText("\n\nRemote logger disconnected\n\n");
                            RemoteLoggerTxt.SelectionColor = Color.White;
                            return;
                        });
                        return;
                    }
                }
                catch(Exception ex)
                {
                    RemoteLoggerTxt.Invoke((MethodInvoker)delegate () 
                    { 
                        RemoteLoggerTxt.SelectionColor = Color.Red;
                        RemoteLoggerTxt.AppendText("\n\nException was thrown when reading logger: " + ex.ToString() + "\n\n");
                        RemoteLoggerTxt.SelectionColor = Color.White;
                    });
                    return;
                }
            }
        }

        private void UpdateConnectionStatus(bool connected)
        {
            if(connected)
            {
                ConnectionStatusLbl.ForeColor = Color.Green;
                ConnectionStatusLbl.Text = "Connected";
            }
            else
            {
                ConnectionStatusLbl.ForeColor = Color.Red;
                ConnectionStatusLbl.Text = "Not connected";
            }
        }

        private void AutoScrollRichTextbox_TextChanged(object sender, EventArgs e)
        {
            RichTextBox richTextBox = sender as RichTextBox;
            richTextBox.SelectionStart = richTextBox.Text.Length;
            richTextBox.ScrollToCaret();
        }

        private void ClearRootBtn_Click(object sender, EventArgs e)
        {
            CurrRootViewer.Root = new TDNRoot();
        }

        private void DeleteValueBtn_Click(object sender, EventArgs e)
        {
            CurrRootViewer.SetRootValue(PathTxt.Text, null);
        }

        TDNFactory tdnFactory;
        private async void CreateValueBtn_Click(object sender, EventArgs e)
        {
            CreateValueBtn.Enabled = false;
            TDNParserSettings settings = new DefaultTDNParserSettings();

            string typeKey = (string)ParserCombo.SelectedItem;
            if (ParserCombo.SelectedIndex == 0)
                typeKey = await tdnFactory.ReadTypeKey(settings);
            ITDNTypeParser parser = settings.Parsers[typeKey];

            object val = await tdnFactory.ReadAnyObject(typeKey, "root", new DefaultTDNParserSettings());
            CurrRootViewer.SetRootValue(PathTxt.Text, new TDNValue(val, parser));

            TDNFactoryConsoleOutTxt.Clear();
            CreateValueBtn.Enabled = true;
        }

        private void PathTxt_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                CreateValueBtn.PerformClick();
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private async void SendBtn_Click(object sender, EventArgs e)
        {
            RecievedRootViewer.Root = await connection.apiCommander.RawRequestAsync(CurrRootViewer.Root);
        }
    }
}
