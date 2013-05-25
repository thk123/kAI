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
        /// The global logger to be used by all classes. If the instance logger is set, 
        /// this will be used instead. 
        /// </summary>
        public static kAIILogger GlobalLogger
        {
            get;
            set;
        }

        /// <summary>
        /// A method for handling exceptions
        /// </summary>
        public static ExceptionHandle ExceptionHandler
        {
            get;
            set;
        }

        /// <summary>
        /// Should an exception be thrown if an assert fails. 
        /// </summary>
        public static bool ExceptionOnAssert = false;

        /// <summary>
        /// The logger of this instance, only used by this object.
        /// If this object is null, then the global logger will be used. 
        /// </summary>
        public kAIILogger Logger
        {
            get;
            set;
        }

        /// <summary>
        /// Set an instance based logger for this object (use SetLogger to set for all objects).
        /// </summary>
        /// <param name="lLogger">The logger for this instance to use.</param>
        /// <seealso cref="kAIObject.GlobalLogger"/>
        public kAIObject(kAIILogger lLogger = null)
        {
            Logger = lLogger;
        }

        /// <summary>
        /// Log a message using the relevant logger. 
        /// </summary>
        /// <param name="lMessage">The message to log.</param>
        /// <param name="lDetails">Any additional information.</param>
        protected void LogMessage(string lMessage, params KeyValuePair<string, object>[] lDetails)
        {
            LogMessage(this, lMessage, lDetails);
        }

        /// <summary>
        /// Log a message using the relevant logger. 
        /// </summary>
        /// <param name="lCaller">The object that called this log, null if global</param>
        /// <param name="lMessage">The message to log.</param>
        /// <param name="lDetails">Any additional information.</param>
        public static void LogMessage(kAIObject lCaller, string lMessage, params KeyValuePair<string, object>[] lDetails)
        {
            kAIILogger lLogger = GetLoggerForInstance(lCaller);
            if (lLogger != null)
            {
                lLogger.LogMessage(lMessage, lDetails);
            }
        }

        /// <summary>
        /// Log a message using the global logger and/or instance logger marked as a warning.
        /// </summary>
        /// <param name="lWarning">The warning string.</param>
        /// <param name="lDetails">Any additional information. </param>
        protected void LogWarning(string lWarning, params KeyValuePair<string, object>[] lDetails)
        {
            LogWarning(this, lWarning, lDetails);
        }

        /// <summary>
        /// Log a message using the global logger and/or instance logger marked as a warning.
        /// </summary>
        /// <param name="lCaller">The object that called this log, null if global</param>
        /// <param name="lWarning">The warning string.</param>
        /// <param name="lDetails">Any additional information. </param>
        public static void LogWarning(kAIObject lCaller, string lWarning, params KeyValuePair<string, object>[] lDetails)
        {
            kAIILogger lLogger = GetLoggerForInstance(lCaller);
            if (lLogger != null)
            {
                lLogger.LogWarning(lWarning, lDetails);
            }
        }

        /// <summary>
        /// Assert that something should be true. Optionally can throw an exception if it isn't. 
        /// </summary>
        /// <param name="lCondition">The condition to check. </param>
        /// <param name="lMessage">Optionally, the message to display. </param>
        protected void Assert(bool lCondition, string lMessage = null)
        {
            Assert(this, lCondition, lMessage);
        }

        /// <summary>
        /// Assert that something should be true. Optionally can throw an exception if it isn't. 
        /// </summary>
        /// <param name="lCaller">The object that called this log, null if global</param>
        /// <param name="lCondition">The condition to check. </param>
        /// <param name="lMessage">Optionally, the message to display. </param>
        public static void Assert(kAIObject lCaller, bool lCondition, string lMessage = null)
        {
            if (!lCondition)
            {
                AssertFailed(lCaller, lMessage);
            }
        }

        /// <summary>
        /// Assert that an object should be not null. 
        /// </summary>
        /// <param name="lObjectToCheck">Checks this object is not null. </param>
        /// <param name="lMessage">Optionally, the message to display. </param>
        protected void Assert(object lObjectToCheck, string lMessage = null)
        {
            Assert(this, lObjectToCheck, lMessage);
        }

        /// <summary>
        /// Assert that an object should be not null. 
        /// </summary>
        /// <param name="lCaller">The object that called this log, null if global</param>
        /// <param name="lObjectToCheck">Checks this object is not null. </param>
        /// <param name="lMessage">Optionally, the message to display. </param>
        public static void Assert(kAIObject lCaller, object lObjectToCheck, string lMessage = null)
        {
            if (lObjectToCheck == null)
            {
                AssertFailed(lCaller, lMessage);
            }
        }

        /// <summary>
        /// Throw an exception. An specific handler can be specified using kAIObject.ExceptionHandler.
        /// </summary>
        /// <seealso cref="ExceptionHandler"/>
        /// <param name="lException">The exception that has been thrown. </param>
        protected void ThrowException(Exception lException)
        {
            ThrowException(this, lException);
        }

        /// <summary>
        /// Throw an exception. An specific handler can be specified using kAIObject.ExceptionHandler.
        /// </summary>
        /// <seealso cref="ExceptionHandler"/>
        /// <param name="lCaller">The object that called this log, null if global</param>
        /// <param name="lException">The exception that has been thrown. </param>
        public static void ThrowException(kAIObject lCaller, Exception lException)
        {
            kAIILogger lLogger = GetLoggerForInstance(lCaller);
            if (lLogger != null)
            {
                lLogger.LogCriticalError("Exception thrown: " + lException.GetType().Name,
                    new KeyValuePair<string, object>("Error Message", lException.Message),
                    new KeyValuePair<string, object>("Stack", lException.StackTrace),
                    new KeyValuePair<string, object>("Exception", lException));
            }

            if (ExceptionHandler != null)
            {
                ExceptionHandler(lException, lCaller);
            }
            else
            {
                throw lException;
            }
        }

        /// <summary>
        /// An assert failed so we call this to decide what to do.
        /// </summary>
        /// <param name="lCaller">The object that asserted the fact. </param>
        /// <param name="lMessage">The message associated with the assert. </param>
        private static void AssertFailed(kAIObject lCaller, string lMessage = null)
        {
            kAIILogger lLogger = GetLoggerForInstance(lCaller);
            if (lLogger != null)
            {
                lLogger.LogError("Assert failed: " + lMessage.GetValueOrDefault(""));
            }


            if (ExceptionOnAssert)
            {
                ThrowException(lCaller, new AssertFailedException(lMessage));
            }
            else
            {
                System.Diagnostics.Debugger.Break();
            }
        }

        /// <summary>
        /// Gets the logger to be used for the given instance. 
        /// </summary>
        /// <param name="lObject">The instance to get the logger for, can be null. </param>
        /// <returns>An instance of the logger to use, or null if there is no applicable logger. </returns>
        private static kAIILogger GetLoggerForInstance(kAIObject lObject)
        {
            if (lObject != null)
            {
                return lObject.Logger.GetValueOrDefault(GlobalLogger);
            }
            else
            {
                return GlobalLogger;
            }
        }
    }

    /// <summary>
    /// The exception that gets triggered when an assert fails (and assert exceptions on). 
    /// </summary>
    class AssertFailedException : Exception
    {
        /// <summary>
        /// Create the exception based on the assert message. 
        /// </summary>
        /// <param name="lMessage"></param>
        public AssertFailedException(string lMessage = null)
            :base(lMessage)
        {}
    }
}
