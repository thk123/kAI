using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using kAI.Core;

namespace kAI.Editor.Core
{
    class ConsoleLogger : kAIILogger
    {
        private static ConsoleLogger mLogger;

        private ConsoleLogger()
        {
            if (mLogger != null)
            {
                throw new LoggerAlreadyInstantiatedException();
            }
        }

        private void WriteString(string lText, ConsoleColor lColour = ConsoleColor.Gray)
        {
            Console.ForegroundColor = lColour;
            Console.WriteLine(DateTime.Now.ToString("HH:mm:ss") + ": " + lText);
            Console.ResetColor();
        }

        private void LogKeys(KeyValuePair<string, object>[] lDetails, ConsoleColor lColor)
        {
            foreach (KeyValuePair<string, object> lDetail in lDetails)
            {
                WriteString("\t" + lDetail.Key + " - " + lDetail.Value.ToString());
            }
        }

        public static ConsoleLogger Get()
        {
            if (mLogger == null)
            {
                mLogger = new ConsoleLogger();
            }

            return mLogger;
        }

        public void LogMessage(string lMessage, params KeyValuePair<string, object>[] lDetails)
        {
            WriteString(lMessage);
            LogKeys(lDetails, ConsoleColor.White);
            
        }

        

        public void LogWarning(string lWarning, params KeyValuePair<string, object>[] lDetails)
        {
            WriteString(lWarning, ConsoleColor.Yellow);
            LogKeys(lDetails, ConsoleColor.Yellow);
        }

        public void LogError(string lError, params KeyValuePair<string, object>[] lDetails)
        {
            WriteString(lError, ConsoleColor.Red);
            LogKeys(lDetails, ConsoleColor.Red);
        }

        public void LogCriticalError(string lError, params KeyValuePair<string, object>[] lDetails)
        {
            LogError(lError, lDetails);
        }
    }
    class LoggerAlreadyInstantiatedException : Exception
    {

    }
}
