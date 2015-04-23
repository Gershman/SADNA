using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.Remoting.Lifetime;

namespace ConsoleApplication1.Logging
{
    public class Logger : MarshalByRefObject, ILoggerListener
    {
        private static object getInstanceLock = new object();
        private static Logger instance;

        private Logger parent;
        private List<ILoggerListener> listeners;

        public override object InitializeLifetimeService()
        {
            ILease lease = (ILease)base.InitializeLifetimeService();
            lease.InitialLeaseTime = TimeSpan.FromDays(365);
            return lease;
        }

        internal static Logger GetInstance()
        {
            if (instance != null)
            {
                return instance;
            }

            lock (getInstanceLock)
            {
                if (instance == null)
                {
                    instance = new Logger();
                }
            }

            return instance;
        }

        public static Logger Instance
        {
            get
            {
                return GetInstance();
            }
        }

        public Logger()
            : this(null)
        {
        }

        public Logger(Logger parent)
        {
            this.parent = parent;
            this.listeners = new List<ILoggerListener>();
        }

        public void LogError()
        {
            LogError("");
        }

        private ILoggerListener[] GetRegistedListeners()
        {
            lock (this.listeners)
            {
                ILoggerListener[] l = this.listeners.ToArray();
                return l;
            }
        }

        public void LogError(string message)
        {
            DateTime now = DateTime.Now;
            LogError(now, message, null);
        }

        public void LogError(DateTime now, string message, int? ident)
        {
            foreach (ILoggerListener listener in GetRegistedListeners())
            {
                try
                {
                    listener.LogError(now, message, ident);
                }
                catch
                {
                    this.listeners.Remove(listener);
                }
            }

            if (this.parent != null)
            {
                this.parent.LogError(now, message, ident);
            }
        }

        public void LogWarning(string message)
        {
            DateTime now = DateTime.Now;
            LogWarning(now, message, null);
        }

        public void LogWarning(DateTime now, string message, int? ident)
        {
            foreach (ILoggerListener listener in GetRegistedListeners())
            {
                try
                {
                    listener.LogWarning(now, message, ident);
                }
                catch
                {
                    this.listeners.Remove(listener);
                }
            }

            if (this.parent != null)
            {
                this.parent.LogWarning(now, message, ident);
            }
        }

        public void LogMessage()
        {
            // LogMessage("");
        }

        public void LogMessage(string message, int? ident)
        {
            DateTime now = DateTime.Now;
            LogMessage(now, message, ident);
        }

        public void LogMessage(DateTime now, string message, int? ident)
        {
            foreach (ILoggerListener listener in GetRegistedListeners())
            {
                try
                {
                    listener.LogMessage(now, message, ident);
                }
                catch
                {
                    this.listeners.Remove(listener);
                }
            }

            if (this.parent != null)
            {
                this.parent.LogMessage(now, message, null);
            }
        }

        public virtual void RegisterListener(ILoggerListener listener)
        {
            lock (this.listeners)
            {
                this.listeners.Add(listener);
            }
        }

        public virtual void UnregisterListener(ILoggerListener listener)
        {
            lock (this.listeners)
            {
                this.listeners.Remove(listener);
            }
        }

        public LoggerHandlerFile GetFileListener()
        {
            foreach (ILoggerListener listener in GetRegistedListeners())
            {
                if (listener is LoggerHandlerFile)
                {
                    LoggerHandlerFile loggerFile = (LoggerHandlerFile)listener;
                    return loggerFile;
                }
            }

            throw new ArgumentException("No registered logger listener of type: LoggerListenerFile");
        }

        public string FilePath
        {
            get
            {
                return GetFileListener().FilePath;
            }
            set
            {
                GetFileListener().FilePath = value;
            }
        }
    }
}