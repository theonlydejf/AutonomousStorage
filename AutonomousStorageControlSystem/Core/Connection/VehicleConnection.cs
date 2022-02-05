using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Team.HobbyRobot.ASCS.Core.Logging;

namespace Team.HobbyRobot.ASCS.Core.Connection
{
    public class VehicleConnection : IDisposable
    {
        public const int LOGGER_PORT = 1111;
        public const int API_PORT = 2222;
        public const int HEARTBEAT_INTERVAL = 9700;

        public IPAddress VehicleIP { get; }

        private Socket apiSocket;
        public Socket APISocket => apiSocket;
        private Socket loggerSocket;
        public Socket LoggerSocket => loggerSocket;

        public Stream APIStream => apiStream;
        public Stream LoggerStream => loggerStream;

        public TDNAPICommander apiCommander { get; private set; }

        private NetworkStream apiStream;
        private NetworkStream loggerStream;
        private readonly ManualResetEvent loggerConnected = new ManualResetEvent(false);
        private readonly ManualResetEvent apiConnected = new ManualResetEvent(false);

        private readonly Logger logger;

        public bool IsDisposed { get; private set; }

        public VehicleConnection(IPAddress ip, Logger logger = null)
        {
            this.logger = logger.CreateSubLogger("Vehicle connection " + ip);

            VehicleIP = ip;

            apiSocket = new Socket(ip.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            loggerSocket = new Socket(ip.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            apiCommander = null;
            apiStream = null;
            loggerStream = null;
        }

        public void BeginAPIConnect()
        {
            IPEndPoint apiEP = new IPEndPoint(VehicleIP, API_PORT);

            Log("Started connecting to api");
            apiConnected.Reset();
            apiSocket.BeginConnect(apiEP, new AsyncCallback(APIConnectCallback), apiSocket);
        }

        public void BeginLoggerConnect()
        {
            IPEndPoint loggerEP = new IPEndPoint(VehicleIP, LOGGER_PORT);

            Log("Started connecting to logger");
            loggerConnected.Reset();
            loggerSocket.BeginConnect(loggerEP, new AsyncCallback(LoggerConnectCallback), loggerSocket);
        }

        private void LoggerConnectCallback(IAsyncResult ar)
        {
            try
            {
                Socket socket = (Socket)ar.AsyncState;
                socket.EndConnect(ar);
                loggerStream = new NetworkStream(socket);
                Log("Logger connected");
            }
            catch (Exception e)
            {
                Log("Exception was thrown when finishing connection to logger: " + e.ToString());
                Dispose();
            }
            loggerConnected.Set();
        }
        private void APIConnectCallback(IAsyncResult ar)
        {
            try
            {
                Socket socket = (Socket)ar.AsyncState;
                socket.EndConnect(ar);
                Log("API connected");
                apiStream = new NetworkStream(socket);
                apiCommander = new TDNAPICommander(this, logger);
                apiCommander.HeartbeatFailed += ApiCommander_HeartbeatFailed;
            }
            catch (Exception e)
            {
                Log("Exception was thrown when finishing connection to API: " + e.ToString());
                Dispose();
            }
            apiConnected.Set();
        }

        private void ApiCommander_HeartbeatFailed(object sender, EventArgs e)
        {
            Log("Disposing API resources...");
            apiSocket.Dispose();
            apiSocket = null;
            apiStream = null;
        }

        public static VehicleConnection CreateConnection(IPAddress ip, Logger logger = null, int timeout = -1)
        {
            VehicleConnection connection = new VehicleConnection(ip, logger);
            connection.BeginLoggerConnect();
            connection.BeginAPIConnect();
            if (WaitHandle.WaitAll(new WaitHandle[] { connection.loggerConnected, connection.apiConnected }, timeout))
                return connection;

            return null;
        }

        public static async Task<VehicleConnection> CreateConnectionAsync(IPAddress ip, Logger logger = null, int timeout = -1, int retryCnt = 1)
        {
            VehicleConnection connection = new VehicleConnection(ip, logger);


            Task connectLogger = Task.Run(() => ConnectLogger(connection, timeout, retryCnt));
            Task connectAPI = Task.Run(() => ConnectAPI(connection, timeout, retryCnt));

            await Task.WhenAll(connectLogger, connectAPI);

            return connection;
        }

        private static void ConnectLogger(VehicleConnection connection, int timeout, int retryCnt)
        {
            try
            {
                while (true)
                {
                    connection.BeginLoggerConnect();
                    if (!connection.loggerConnected.WaitOne(timeout) || connection.loggerStream == null)
                    {
                        retryCnt--;
                        if (retryCnt == 0)
                            break;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                connection.Log("Exception was thrown when connecting to logger: " + ex.ToString());
            }
        }

        private static void ConnectAPI(VehicleConnection connection, int timeout, int retryCnt)
        {
            try
            {
                while (true)
                {
                    connection.BeginAPIConnect();
                    if (!connection.apiConnected.WaitOne(timeout) || connection.apiStream == null)
                    {
                        retryCnt--;
                        if (retryCnt == 0)
                            break;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                connection.Log("Exception was thrown when connecting to API: " + ex.ToString());
            }
        }

        private void Log(string msg)
        {
            if (logger != null)
                logger.Log(msg);
        }

        public static bool IsValid(VehicleConnection connection)
        {
            return connection != null && !connection.IsDisposed;
        }

        public void Dispose()
        {
            if(apiCommander != null)
                apiCommander.Dispose();
            if(loggerSocket != null)
            {
                if(loggerSocket.Connected)
                    loggerSocket.Disconnect(false);
                loggerSocket.Dispose();
            }
            if(apiSocket != null)
            {
                if (apiSocket.Connected)
                    apiSocket.Disconnect(false);
                apiSocket.Dispose();
            }
            IsDisposed = true;
        }
    }
}
