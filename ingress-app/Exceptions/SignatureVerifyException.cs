using System;
using System.Runtime.Serialization;

namespace webapi
{
    [Serializable]
    public class SignatureVerifyException : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public SignatureVerifyException()
        {
        }

        public SignatureVerifyException(string message) : base(message)
        {
        }

        public SignatureVerifyException(string message, Exception inner) : base(message, inner)
        {
        }

        protected SignatureVerifyException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}