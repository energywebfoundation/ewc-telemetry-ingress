using System;
using System.Runtime.Serialization;

namespace webapi
{
    /// <summary>
    /// Custom Exception class for Key Load Exception
    /// </summary>
    [Serializable]
    public class KeyLoadException : Exception
    {

        /// <summary>
        /// Deafult Constructor of KeyLoadException
        /// </summary>
        /// <returns>returns instance of KeyLoadException</returns>
        public KeyLoadException()
        {
        }

        /// <summary>
        ///  Parameterized Constructor of KeyLoadException with message
        /// </summary>
        /// <param name="message">The message string for KeyLoadException</param>
        /// <returns>returns instance of KeyLoadException with custom message</returns>
        public KeyLoadException(string message) : base(message)
        {
        }

        /// <summary>
        ///  Parameterized Constructor of KeyLoadException with message and Inner Exception
        /// </summary>
        /// <param name="message">The message string for KeyLoadException</param>
        /// <param name="inner">The inner Exception reference for KeyLoadException</param>
        /// <returns>returns instance of KeyLoadException with custom message and inner exception</returns>
        public KeyLoadException(string message, Exception inner) : base(message, inner)
        {
        }

        /// <summary>
        ///  Parameterized Constructor of KeyLoadException with SerializationInfo and StreamingContext
        /// </summary>
        /// <param name="info">The serialization info for KeyLoadException</param>
        /// <param name="context">The streaming context for KeyLoadException</param>
        /// <returns>returns instance of KeyLoadException with custom serialization and streaming context</returns>
        protected KeyLoadException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}