using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace kAI.Core
{
    public abstract class kAIObject
    {
        static kAIILogger mGlobalLogger;
        kAIILogger mLogger;

        public static void SetLogger(kAIILogger lLogger);

        public kAIObject()
        {
            mLogger = null;
        }

        public kAIObject(kAIILogger lLogger)
        {
            mLogger = lLogger;
        }

        public void LogMessage(string lMessage)
        {
            if (mLogger != null)
            {
                mLogger.LogMessage(GetType().ToString() + ": " + lMessage);
            }

            if (mGlobalLogger != null)
            {
                mLogger.LogMessage(GetType().ToString() + ": " + lMessage);
            }
        }

        public void LogWarning(string lWarning, params object[] lDetails)
        {
            if (mLogger != null)
            {
                mLogger.LogWarning(GetType().ToString() + ": " + lWarning, lDetails);
            }

            if (mGlobalLogger != null)
            {
                mLogger.LogWarning(GetType().ToString() + ": " + lWarning, lDetails);
            }
        }

        public void LogError(string lError, params object[] lDetails)
        {
            if (mLogger != null)
            {
                mLogger.LogError(GetType().ToString() + ": " + lError, lDetails);
            }

            if (mGlobalLogger != null)
            {
                mLogger.LogError(GetType().ToString() + ": " + lError, lDetails);
            }
        }

        public void LogCriticalError(string lError, params object[] lDetails)
        {
            if (mLogger != null)
            {
                mLogger.LogCriticalError(GetType().ToString() + ": " + lError, lDetails);
            }

            if (mGlobalLogger != null)
            {
                mLogger.LogCriticalError(GetType().ToString() + ": " + lError, lDetails);
            }
        }
    }
}
