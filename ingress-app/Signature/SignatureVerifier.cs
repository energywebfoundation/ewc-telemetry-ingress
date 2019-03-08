using System;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;

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
    
    public class SignatureVerifier
    {

        /// <summary>
        /// Verifies that a given payload was signed with the correct key
        /// </summary>
        /// <param name="payload">The payload to verify</param>
        /// <param name="signature">The Signature send along with the payload</param>
        /// <param name="publicKey">The Public key of the keypair that signed the payload</param>
        /// <returns>true if signature is valid, otherwise false</returns>
        public static bool IsSignatureValid(string payload, string signature, string publicKey)
        {
           
            try
            {
                // Decode payload and signature to bytes
                ASCIIEncoding byteConverter = new ASCIIEncoding();
                byte[] payloadBytes = byteConverter.GetBytes(payload);
                byte[] signatureBytes = Convert.FromBase64String(signature);

                
                // Get csp and load public key
                byte[] publicKeyBytes = Convert.FromBase64String(publicKey);
                RSACryptoServiceProvider rsaCsp = new RSACryptoServiceProvider();
                rsaCsp.ImportCspBlob(publicKeyBytes);
                
                // Make sure we've loaded a public key
                if (!rsaCsp.PublicOnly)
                {
                    throw new KeyLoadException("Was given a private key. Won't continue.");
                }

                return rsaCsp.VerifyData(payloadBytes, new SHA256CryptoServiceProvider(), signatureBytes);

            }
            catch(Exception ex)
            {
                //throw new SignatureVerifyException("Unable to verify signature",ex);
                //TO DO we can log this afterMVP but verify sig method must return false
                Console.WriteLine("Signature Verification Failed {0}", ex.ToString());
                return false;
            }
        }
        
    }
}