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
        private readonly IPublickeySource _pubkeySrc;

        public SignatureVerifier(IPublickeySource pubkeySrc) 
        {
            _pubkeySrc = pubkeySrc ?? throw new ArgumentNullException(nameof(pubkeySrc));
        }


        /// <summary>
        /// Verifies that a given payload was signed with the correct key
        /// </summary>
        /// <param name="payload">The payload to verify</param>
        /// <param name="signature">The Signature send along with the payload</param>
        /// <param name="nodeId">The Id of the node that signed this</param>
        /// <returns>true if signature is valid, otherwise false</returns>
        public bool IsSignatureValid(string payload, string signature, string nodeId)
        {

            string publicKey;
            try
            {
                publicKey = _pubkeySrc.GetKeyForNode(nodeId);
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to find key for nodeId",ex);
            }
            
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

                RSAParameters key = rsaCsp.ExportParameters(false);

                // Verify the data and display the result to the 
                // console.
                return VerifySignedHash(payloadBytes, signatureBytes, key);

            }
            catch(Exception ex)
            {
                throw new SignatureVerifyException("Unable to verify signature",ex);
            }
        }

        private static bool VerifySignedHash(byte[] dataToVerify, byte[] signatureBytes, RSAParameters key)
        {
            try
            {
                // Create a new instance of RSACryptoServiceProvider using the 
                // key from RSAParameters.
                RSACryptoServiceProvider rsAalg = new RSACryptoServiceProvider();

                rsAalg.ImportParameters(key);

                // Verify the data using the signature.  Pass a new instance of SHA1CryptoServiceProvider
                // to specify the use of SHA1 for hashing.
                return rsAalg.VerifyData(dataToVerify, new SHA256CryptoServiceProvider(), signatureBytes); 

            }
            catch(CryptographicException e)
            {
                Console.WriteLine(e.Message);

                return false;
            }
        }
        
    }
}