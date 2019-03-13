using System;
using System.Runtime.Serialization;

namespace webapi
{
    /// <summary>
    /// Custom Exception class for Empty File Exception
    /// </summary>
    [Serializable]
    public class FileEmptyException : Exception
    {

        /// <summary>
        /// Deafult Constructor of FileEmptyException
        /// </summary>
        /// <returns>returns instance of FileEmptyException</returns>
        public FileEmptyException()
        {
        }

        /// <summary>
        ///  Parameterized Constructor of FileEmptyException with message
        /// </summary>
        /// <param name="message">The message string for FileEmptyException</param>
        /// <returns>returns instance of FileEmptyException with custom message</returns>
        public FileEmptyException(string message) : base(message)
        {
        }

        /// <summary>
        ///  Parameterized Constructor of FileEmptyException with message and Inner Exception
        /// </summary>
        /// <param name="message">The message string for FileEmptyException</param>
        /// <param name="inner">The inner Exception reference for FileEmptyException</param>
        /// <returns>returns instance of FileEmptyException with custom message and inner exception</returns>
        public FileEmptyException(string message, Exception inner) : base(message, inner)
        {
        }

        /// <summary>
        ///  Parameterized Constructor of FileEmptyException with SerializationInfo and StreamingContext
        /// </summary>
        /// <param name="info">The serialization info for FileEmptyException</param>
        /// <param name="context">The streaming context for FileEmptyException</param>
        /// <returns>returns instance of FileEmptyException with custom serialization and streaming context</returns>
        protected FileEmptyException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}