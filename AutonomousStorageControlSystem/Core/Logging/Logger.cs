using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Team.HobbyRobot.ASCS.Core.Logging
{
    public class Logger : IDisposable
    {
        private LinkedList<TextWriter> writers = new LinkedList<TextWriter>();
        public IEnumerable<string> LocalLog => localLog;
        private LinkedList<string> localLog = new LinkedList<string>();

		public string Name { get; protected set; }
        public bool IncludeName { get; set; }

        public Logger() : this(null)
		{
		}

        public Logger(string name)
        {
            Name = name;
            AppDomain.CurrentDomain.ProcessExit += (s, e) => { Log("Shutting down..."); Dispose(); };
        }

        public void RegisterEndpoint(TextWriter writer)
        {
            writers.AddLast(writer);
        }

        public void UnregisterEndpoint(TextWriter writer)
        {
            writers.Remove(writer);
        }

		public virtual void Log(string message)
		{
			string msg = message;
			if (IncludeName && Name != null)
				msg = Name + ": " + msg;

			localLog.AddLast(msg);

			LinkedList<TextWriter> badWriters = new LinkedList<TextWriter>();
			foreach(TextWriter tw in writers)
			{
				try
				{
					tw.WriteLine(msg);
					tw.Flush();
				}
				catch (Exception)
				{
					badWriters.AddLast(tw);
				}
			}

			if (badWriters.Count() > 0)
			{
				foreach(TextWriter tw in badWriters)
					writers.Remove(tw);
				for (int i = 0; i < badWriters.Count(); i++)
					Log("Writer for logger threw an exception");
			}
		}

		public Logger CreateSubLogger(String name)
		{
			return new SubLogger(name, this);
		}

		public void Dispose()
        {
            foreach (TextWriter writer in writers)
                writer.Dispose();
        }

		public class SubLogger : Logger
		{
			public Logger parent { get; }
			internal SubLogger(string name, Logger parent) : base(name)
			{
				this.parent = parent;
				IncludeName = false;
			}

            public override void Log(string message)
            {
                base.Log(message);
				parent.Log(Name + ": " + message);
			}
	}
}
}
