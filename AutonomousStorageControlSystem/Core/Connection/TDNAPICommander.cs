using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Team.HobbyRobot.ASCS.Core.Logging;
using Team.HobbyRobot.TDN.Base;
using Team.HobbyRobot.TDN.Core;

namespace Team.HobbyRobot.ASCS.Core.Connection
{
    public class TDNAPICommander : IDisposable
    {
        // - Constants for parsing requests - //
        public const string SERVICE_KEYWORD = "service";
	    public const string REQUEST_KEYWORD = "request";
	    public const string PARAMS_KEYWORD = "params";

	    // - Constants for parsing responses - //
	    public const string ERROR_CODE_KEYWORD = "error-code";
	    public const string ERROR_DETAILS_KEYWORD = "details";
	    public const string DATA_KEYWORD = "data";

        public const string API_SERVICE_NAME = "API";
        public const string HEARTBEAT_REQUEST_NAME = "Heartbeat";

        public const int HEARTBEAT_INTERVAL = 9700;

        private Stream stream;
        private VehicleConnection connection;
        private Timer heartbeatTimer;

        protected Logger logger = null;

        public bool IsDisposed { get; private set; }

        public event EventHandler HeartbeatFailed;

        static TDNAPICommander()
        {
            heartbeatRequest = new TDNRoot();
            heartbeatRequest["service"] = (ConvertibleTDNValue)API_SERVICE_NAME;
            heartbeatRequest["request"] = (ConvertibleTDNValue)HEARTBEAT_REQUEST_NAME;
        }

        public TDNAPICommander(VehicleConnection connection, Logger logger = null)
        {
            this.connection = connection;
            stream = connection.APIStream;
            this.logger = logger.CreateSubLogger("API Commander");
            heartbeatTimer = new Timer(HEARTBEAT_INTERVAL);
            heartbeatTimer.Elapsed += HeartbeatTimer_Elapsed;
            heartbeatTimer.Start();
            Log("Heartbeats started...");

            Log("First heartbeat sent");
            heartbeatRequest.WriteToStream(stream);
        }

        private void HeartbeatTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                heartbeatRequest.WriteToStream(stream);
                Log("Heartbeat sent");
            }
            catch(IOException ex)
            {
                Log("Exception was thrown when sending heartbeat. Disposing...: " + ex.ToString());
                if (HeartbeatFailed != null)
                    HeartbeatFailed(this, null);
                Dispose();
            }
        }

        public TDNRoot RawRequest(TDNRoot request)
        {
            request.WriteToStream(stream);
            return TDNRoot.ReadFromStream(stream);
        }

        private static TDNRoot heartbeatRequest;
        public async Task<TDNRoot> RawRequestAsync(TDNRoot request)
        {
            return await Task.Run(() => RawRequest(request));
        }

        public static TDNRoot CreateAPIRequest(string service, string request, TDNRoot _params)
        {
            TDNRoot tdnRequest = new TDNRoot();
            tdnRequest[SERVICE_KEYWORD] = (ConvertibleTDNValue)service;
            tdnRequest[REQUEST_KEYWORD] = (ConvertibleTDNValue)request;
            tdnRequest[PARAMS_KEYWORD] = (ConvertibleTDNValue)_params;
            return tdnRequest;
        }

        public TDNRoot Request(string service, string request, TDNRoot _params)
        {

            return RawRequest(CreateAPIRequest(service, request, _params));
        }

        public async Task<TDNRoot> RequestAsync(string service, string request, TDNRoot _params)
        {
            return await RawRequestAsync(CreateAPIRequest(service, request, _params));
        }

        private void Log(string msg)
        {
            if (logger != null)
                logger.Log(msg);
        }

        public void Dispose()
        {
            heartbeatTimer.Stop();
            IsDisposed = true;
        }
    }
}
