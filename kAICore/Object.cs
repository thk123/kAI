using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace kAI.Core
{
    /// <summary>
    /// The root class all kAI.Core Objects inherit from. Provides functions for logging messages and error
    /// </summary>
    [DataContract()]    
    public abstract class kAIObject
    {
        /// <summary>
        /// A method for handling exceptions from within kAI. 
        /// </summary>
        /// <param name="lException">The exception that was thrown. </param>
        /// <param name="lSender">The object that threw the exception. </param>
        public delegate void ExceptionHandle(Exception lException, kAIObject lSender);

        /// <summary>
        /// The global logger to be used by all classes.
        /// </summary>
        static kAIILogger mGlobalLogger;

        static ExceptionHandle ExceptionHandler
        {
            get;
            set;
        }

        public static bool ExceptionOnAssert = false;
        

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

        /// <summary>
        /// Throw an exception. An specific handler can be specified using kAIObject.ExceptionHandler.
        /// </summary>
        /// <seealso cref="ExceptionHandler"/>
        /// <param name="lException">The exception that has been thrown. </param>
        protected void ThrowException(Exception lException)
        {
            if (kAIObject.ExceptionHandler != null)
            {
                ExceptionHandler(lException, this);
            }
            else
            {
                throw lException;
            }
        }

        /// <summary>
        /// Assert that something should be true. Optionally can throw an exception if it isn't. 
        /// </summary>
        /// <param name="lCondition">The condition to check. </param>
        /// <param name="lMessage">Optionally, the message to display. </param>
        protected void Assert(bool lCondition, string lMessage = null)
        {
            kAIObject.Assert(this, lCondition, lMessage);
        }

        protected static void Assert(kAIObject lObject, bool lCondition, string lMessage = null)
        {
            if (!lCondition)
            {
                //LogCriticalError(lMessage);
                System.Diagnostics.Debugger.Break();

                if (ExceptionOnAssert)
                {
                    //ThrowException(new AssertFailedException(lMessage));
                }
            }
        }
    }

    class AssertFailedException : Exception
    {
        public AssertFailedException(string lMessage = null)
            :base(lMessage)
        {}
    }
}
