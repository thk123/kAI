using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace kAI.Core
{
    public interface kAIILogger
    {
        void LogMessage(string lMessage);
        void LogWarning(string lWarning, params object[] lDetails);
        void LogError(string lError, params object[] lDetails);
        void LogCriticalError(string lError, params object[] lDetails);
    }
}
