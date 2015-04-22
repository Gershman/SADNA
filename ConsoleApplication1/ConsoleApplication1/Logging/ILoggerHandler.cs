using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApplication1.Logging
{
    public interface ILoggerListener
    {
        void LogError(DateTime now, string message, int? ident);
        void LogMessage(DateTime now, string message, int? ident);
        void LogWarning(DateTime now, string message, int? ident);
    }
}
