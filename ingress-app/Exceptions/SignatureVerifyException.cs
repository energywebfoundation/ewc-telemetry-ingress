using System;
using System.Runtime.Serialization;

namespace webapi
{
    /// <summary>
    /// Custom Exception class for Signature Verify Exception
    /// </summary>
    [Serializable]
    public class SignatureVerifyException : Exception
    {

        /// <summary>
        /// Deafult Constructor of SignatureVerifyException
        /// </summary>
        /// <returns>returns instance of SignatureVerifyException</returns>
        public SignatureVerifyException()
        {
        }

        /// <summary>
        ///  Parameterized Constructor of SignatureVerifyException with message
        /// </summary>
        /// <param name="message">The message string for SignatureVerifyException</param>
        /// <returns>returns instance of SignatureVerifyException with custom message</returns>
        public SignatureVerifyException(string message) : base(message)
        {
        }

        /// <summary>
        ///  Parameterized Constructor of SignatureVerifyException with message and Inner Exception
        /// </summary>
        /// <param name="message">The message string for SignatureVerifyException</param>
        /// <param name="inner">The inner Exception reference for SignatureVerifyException</param>
        /// <returns>returns instance of SignatureVerifyException with custom message and inner exception</returns>
        public SignatureVerifyException(string message, Exception inner) : base(message, inner)
        {
        }

        /// <summary>
        ///  Parameterized Constructor of SignatureVerifyException with SerializationInfo and StreamingContext
        /// </summary>
        /// <param name="info">The serialization info for SignatureVerifyException</param>
        /// <param name="context">The streaming context for SignatureVerifyException</param>
        /// <returns>returns instance of SignatureVerifyException with custom serialization and streaming context</returns>
        protected SignatureVerifyException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}