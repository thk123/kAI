using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace kAI.Core
{
    /// <summary>
    /// Inherit from this to create a logger that can be used by kAIObjects
    /// </summary>
    /// <seealso cref="kAIObject"/>
    public interface kAIILogger
    {
        /// <summary>
        /// Log a standard message. This is not an error, just some information.
        /// </summary>
        /// <param name="lMessage">The message.</param>
        /// <param name="lDetails">Any additional information. </param>
        void LogMessage(string lMessage, params object[] lDetails);

        /// <summary>
        /// Log a warning. Something unexpected has happened, but can carry on. 
        /// </summary>
        /// <param name="lWarning">The warning message. </param>
        /// <param name="lDetails"></param>
        void LogWarning(string lWarning, params object[] lDetails);

        /// <summary>
        /// An error has occurred, normally should stop, AI will behave unexpectedly
        /// </summary>
        /// <param name="lError">The error message. </param>
        /// <param name="lDetails">Any additional information.</param>
        void LogError(string lError, params object[] lDetails);

        /// <summary>
        /// Something has gone catastrophically wrong, it is impossible for kAI to continue. 
        /// </summary>
        /// <param name="lError">The error message.</param>
        /// <param name="lDetails">Any additional information.</param>
        void LogCriticalError(string lError, params object[] lDetails);
    }
}
