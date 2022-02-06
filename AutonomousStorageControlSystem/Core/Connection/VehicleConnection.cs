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
        /// <summary>
        /// Port on which the vehicle runs the logger
        /// </summary>
        public const int LOGGER_PORT = 1111;
        /// <summary>
        /// Port on which the vehicle runs the TDN API
        /// </summary>
        public const int API_PORT = 2222;

        /// <summary>
        /// Event which will be invoked right after the logger was succesfully conencted
        /// </summary>
        public event EventHandler<LoggerConnectedEventArgs> LoggerConnected;
        /// <summary>
        /// Event which will be invoked right after the TDN API was succesfully connected
        /// </summary>
        public event EventHandler<TDNAPIConnectedEventArgs> APIConnected;

        /// <summary>
        /// IP address of the vehicle
        /// </summary>
        public IPAddress VehicleIP { get; }
        /// <summary>
        /// Socket on which the TDN API is listening
        /// </summary>
        public Socket APISocket => apiSocket;
        /// <summary>
        /// Socket on which the logger runs
        /// </summary>
        public Socket LoggerSocket => loggerSocket;
        /// <summary>
        /// Stream used to communicate with TDN API
        /// </summary>
        public Stream APIStream => apiStream;
        /// <summary>
        /// Stream used to communicate with vehicle logger
        /// </summary>
        public Stream LoggerStream => loggerStream;

        /// <summary>
        /// Commander, which ensures communication with the vehicles TDN API
        /// </summary>
        public TDNAPICommander apiCommander { get; private set; }

        private Socket apiSocket;
        private Socket loggerSocket;
        private NetworkStream apiStream;
        private NetworkStream loggerStream;
        private readonly ManualResetEvent loggerConnectionTried = new ManualResetEvent(false);
        private readonly ManualResetEvent apiConnectionTried = new ManualResetEvent(false);

        private readonly Logger logger;

        /// <summary>
        /// True, if the current instance of VehicleConnection is disposed, otherwise false
        /// </summary>
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// Creates disconnected instance of <see cref="VehicleConnection"/>
        /// </summary>
        /// <param name="ip">IP address of the targeted vehicle</param>
        /// <param name="logger">Global logger, onto which connection events will be logged</param>
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

        /// <summary>
        /// Begins an attempt to connect to the vehicle's TDN API. If the connection was succesful, it will invoke the <see cref="APIConnected"/> event
        /// </summary>
        public void BeginAPIConnect()
        {
            IPEndPoint apiEP = new IPEndPoint(VehicleIP, API_PORT);

            Log("Started connecting to api");
            apiConnectionTried.Reset();
            apiSocket.BeginConnect(apiEP, new AsyncCallback(APIConnectCallback), apiSocket);
        }

        /// <summary>
        /// Begins an attempt to connect to the vehicle's logger. If the connection was succesful, it will invoke the <see cref="LoggerConnected"/> event
        /// </summary>
        public void BeginLoggerConnect()
        {
            IPEndPoint loggerEP = new IPEndPoint(VehicleIP, LOGGER_PORT);

            Log("Started connecting to logger");
            loggerConnectionTried.Reset();
            loggerSocket.BeginConnect(loggerEP, new AsyncCallback(LoggerConnectCallback), loggerSocket);
        }

        private void LoggerConnectCallback(IAsyncResult ar)
        {
            try
            {
                Socket socket = (Socket)ar.AsyncState;
                if(socket.Connected)
                {
                    socket.EndConnect(ar);
                    loggerStream = new NetworkStream(socket);
                    Log("Logger connected");
                    if (LoggerConnected != null)
                        LoggerConnected.Invoke(this, new LoggerConnectedEventArgs(loggerStream));
                }
            }
            catch (Exception e)
            {
                Log("Exception was thrown when finishing connection to logger: " + e.ToString());
                Dispose();
            }
            loggerConnectionTried.Set();
        }

        private void APIConnectCallback(IAsyncResult ar)
        {
            try
            {
                Socket socket = (Socket)ar.AsyncState;
                if(socket.Connected)
                { 
                    socket.EndConnect(ar);
                    apiStream = new NetworkStream(socket);
                    apiCommander = new TDNAPICommander(this, logger);
                    apiCommander.HeartbeatFailed += ApiCommander_HeartbeatFailed;
                    Log("API connected");
                    if (APIConnected != null)
                        APIConnected.Invoke(this, new TDNAPIConnectedEventArgs(apiStream));
                }
            }
            catch (Exception e)
            {
                Log("Exception was thrown when finishing connection to API: " + e.ToString());
                Dispose();
            }
            apiConnectionTried.Set();
        }

        private void ApiCommander_HeartbeatFailed(object sender, EventArgs e)
        {
            Log("Disposing API resources...");
            apiSocket.Dispose();
            apiSocket = null;
            apiStream = null;
        }

        /// <summary>
        /// Creates a connection to a vehicle
        /// </summary>
        /// <param name="ip">IP address of the vehicle</param>
        /// <param name="logger">Global logger, onto which connection events will be logged</param>
        /// <param name="timeout">Timout, after which the connection attempt will be considered unsuccessful (use -1 for no timeout)</param>
        /// <param name="retryCnt">Amount of attempts, before considering the vehicle as unreachable (use -1 for infinite attempts)</param>
        /// <returns>Instance of <see cref="VehicleConnection"/> describing the connection to the vehicle</returns>
        public static VehicleConnection CreateConnection(IPAddress ip, Logger logger = null, int timeout = -1, int retryCnt = 1)
        {
            VehicleConnection connection = new VehicleConnection(ip, logger);

            connection.Connect(timeout, retryCnt);

            return connection;
        }

        /// <summary>
        /// Creates a connection to a vehicle asynchronously
        /// </summary>
        /// <param name="ip">IP address of the vehicle</param>
        /// <param name="logger">Global logger, onto which connection events will be logged</param>
        /// <param name="timeout">Timout, after which the connection attempt will be considered unsuccessful (use -1 for no timeout)</param>
        /// <param name="retryCnt">Amount of attempts, before considering the vehicle as unreachable (use -1 for infinite attempts)</param>
        /// <returns>Instance of <see cref="VehicleConnection"/> describing the connection to the vehicle</returns>
        public static async Task<VehicleConnection> CreateConnectionAsync(IPAddress ip, Logger logger = null, int timeout = -1, int retryCnt = 1)
        {
            VehicleConnection connection = new VehicleConnection(ip, logger);

            await connection.ConnectAsync(timeout, retryCnt);

            return connection;
        }

        /// <summary>
        /// Tries to establish a connection to the vehicle
        /// </summary>
        /// <param name="timeout">Timout, after which the connection attempt will be considered unsuccessful (use -1 for no timeout)</param>
        /// <param name="retryCnt">Amount of attempts, before considering the vehicle as unreachable (use -1 for infinite attempts)</param>
        public void Connect(int timeout = -1, int retryCnt = 1)
        {
            Task connectLogger = Task.Run(() => ConnectLogger(timeout, retryCnt));
            Task connectAPI = Task.Run(() => ConnectAPI(timeout, retryCnt));

            Task.WaitAll(connectLogger, connectAPI);
        }

        /// <summary>
        /// Tries to establish a connection to the vehicle asynchronously
        /// </summary>
        /// <param name="timeout">Timout, after which the connection attempt will be considered unsuccessful (use -1 for no timeout)</param>
        /// <param name="retryCnt">Amount of attempts, before considering the vehicle as unreachable (use -1 for infinite attempts)</param>
        public async Task ConnectAsync(int timeout = -1, int retryCnt = 1)
        {
            Task connectLogger = Task.Run(() => ConnectLogger(timeout, retryCnt));
            Task connectAPI = Task.Run(() => ConnectAPI(timeout, retryCnt));

            await Task.WhenAll(connectLogger, connectAPI);
        }

        private void ConnectLogger(int timeout, int retryCnt)
        {
            while (true)
            {
                try
                {
                    BeginLoggerConnect();
                }
                catch (Exception ex)
                {
                    Log("Exception was thrown when connecting to logger: " + ex.ToString());
                }

                if (!loggerConnectionTried.WaitOne(timeout) || loggerStream == null)
                {
                    retryCnt--;
                    Log("Connecting logger failed, trying again..." + (retryCnt > 0 ? $" ({retryCnt} attempts remaining)" : ""));
                    if (retryCnt == 0)
                        break;
                }
                else
                {
                    break;
                }
            }
            Log("Connecting logger done");
        }

        private void ConnectAPI(int timeout, int retryCnt)
        {
            while (true)
            {
                try
                {
                    BeginAPIConnect();
                }
                catch (Exception ex)
                {
                    Log("Exception was thrown when connecting to API: " + ex.ToString());
                }

                if (!apiConnectionTried.WaitOne(timeout) || apiStream == null)
                {
                    retryCnt--;
                    Log("Connecting API failed, trying again..." + (retryCnt > 0 ? $" ({retryCnt} attempts remaining)" : ""));
                    if (retryCnt == 0)
                        break;
                }
                else
                {
                    break;
                }
            }
            Log("Connecting API done");
        }

        private void Log(string msg)
        {
            if (logger != null)
                logger.Log(msg);
        }

        /// <summary>
        /// Checks, whether the <see cref="VehicleConnection"/> is valid
        /// </summary>
        /// <param name="connection">Instance of <see cref="VehicleConnection"/> to check validity for</param>
        /// <returns>True, if the connection is valid, otherwise false</returns>
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

    public class TDNAPIConnectedEventArgs : EventArgs
    {
        public TDNAPIConnectedEventArgs(Stream stream)
        {
            Stream = stream;
        }

        public Stream Stream { get; }
    }

    public class LoggerConnectedEventArgs : EventArgs
    {
        public Stream Stream { get; }

        public LoggerConnectedEventArgs(Stream stream)
        {
            Stream = stream;
        }
    }
}
