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
        TDNFactory tdnFactory;

        public Form1()
        {
            InitializeComponent();
            logger.RegisterEndpoint(LocalLoggerConsole.TextWriter);

            tdnFactory = new TDNFactory(TDNFactoryConsoleOutput.TextWriter, new TextBoxBasicReader(TDNFactoryConsoleInTxt, TDNFactoryConsoleOutput.TextWriter));
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

            TDNRoot pt1 = new TDNRoot();
            pt1["x"] = (ConvertibleTDNValue)2000f;
            pt1["y"] = (ConvertibleTDNValue)0f;

            TDNRoot pt2 = new TDNRoot();
            pt2["x"] = (ConvertibleTDNValue)0f;
            pt2["y"] = (ConvertibleTDNValue)0f;
            pt2["heading"] = (ConvertibleTDNValue)0f;

            TDNRoot root = new TDNRoot();
            root["service"] = (ConvertibleTDNValue)"MovementService";
            root["request"] = (ConvertibleTDNValue)"followPath";
            root["params.path"] = (ConvertibleTDNValue)new TDNArray(new object[] { pt1, pt2 }, TDNParsers.ROOT);

            CurrRootViewer.Root = root;
        }

        private async void ConnectBtn_Click(object sender, EventArgs e)
        {
            IPTxt.Enabled = false;
            ConnectBtn.Enabled = false;
            connection = new VehicleConnection(IPAddress.Parse(IPTxt.Text), logger);
            connection.LoggerConnected += Connection_LoggerConnected;
            await connection.ConnectAsync(-1, -1);
            UpdateConnectionStatus(VehicleConnection.IsValid(connection));
        }

        private void Connection_LoggerConnected(object sender, LoggerConnectedEventArgs e)
        {
            RemoteLoggerWatcher.Invoke((MethodInvoker)delegate ()
            {
                RemoteLoggerWatcher.SelectionColor = Color.LimeGreen;
                RemoteLoggerWatcher.AppendText("\n\nRemote logger connected\n\n");
                RemoteLoggerWatcher.SelectionColor = Color.White;
                return;
            });
            RemoteLoggerWatcher.Start(connection.LoggerStream);
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

            IPTxt.Enabled = !connected;
            ConnectBtn.Enabled = !connected;
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

            TDNFactoryConsoleOutput.Clear();
            CreateValueBtn.Enabled = true;
            PathTxt.Focus();
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
