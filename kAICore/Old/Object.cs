using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace kAI.Core
{
    /// <summary>
    /// The root class all kAI.Core Objects inherit from. Provides functions for logging messages and error
    /// </summary>
    public abstract class kAIObject
    {
        /// <summary>
        /// The global logger to be used by all classes.
        /// </summary>
        static kAIILogger mGlobalLogger;

        /// <summary>
        /// The logger of this instance, only used by this object.
        /// </summary>
        kAIILogger mLogger;

        /// <summary>
        /// Set the global logger used by all kAIObjects.
        /// </summary>
        /// <param name="lLogger">The instance of the logger for all objects to use.</param>
        public static void SetGlobalLogger(kAIILogger lLogger)
        {
            mGlobalLogger = lLogger;
        }

        /// <summary>
        /// Set an instance based logger for this object (use SetLogger to set for all objects).
        /// </summary>
        /// <param name="lLogger">The logger for this instance to use.</param>
        /// <seealso cref="kAIObject.SetGlobalLogger"/>
        public kAIObject(kAIILogger lLogger = null)
        {
            mLogger = lLogger;
        }

        /// <summary>
        /// Sets the logger to be used by this instance.
        /// </summary>
        /// <param name="lLogger">The logger for this instance to use from now on. </param>
        public void SetInstanceLogger(kAIILogger lLogger)
        {
            mLogger = lLogger;
        }

        /// <summary>
        /// Log a message using the global logger and/or instance logger marked as a message.
        /// </summary>
        /// <param name="lMessage">The message to log.</param>
        /// <param name="lDetails">Any additional information.</param>
        public void LogMessage(string lMessage, params object[] lDetails)
        {
            if (mLogger != null)
            {
                mLogger.LogMessage(GetType().ToString() + ": " + lMessage, lDetails);
            }

            if (mGlobalLogger != null)
            {
                mLogger.LogMessage(GetType().ToString() + ": " + lMessage, lDetails);
            }
        }

        /// <summary>
        /// Log a message using the global logger and/or instance logger marked as a warning.
        /// </summary>
        /// <param name="lWarning">The warning string.</param>
        /// <param name="lDetails">Any additional information. </param>
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

        /// <summary>
        /// Log a message using the global logger and/or instance logger marked as an error.
        /// </summary>
        /// <param name="lError">The error string.</param>
        /// <param name="lDetails">Any additional information.</param>
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

        /// <summary>
        /// Log a message using the global logger and/or instance logger marked as an critical error (eg no recovery).
        /// </summary>
        /// <param name="lError">The error string.</param>
        /// <param name="lDetails">Any additional information.</param>
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
