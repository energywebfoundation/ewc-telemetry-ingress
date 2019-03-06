using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace stresstests
{
    public class RSACryptoManager
    {
        private RSACryptoServiceProvider RSAalg;
        public RSACryptoManager(string keyfile){
            byte[] csp = importCspParam(keyfile);
            RSAalg = new RSACryptoServiceProvider();
            RSAalg.ImportCspBlob(csp);
        }
        public void createAndExportRSAKeys(string filePath, Boolean includePrivateKey, string nodeId)
        {
            RSACryptoServiceProvider RSAalg = new RSACryptoServiceProvider();
            //export public key and private key
            byte[] cspBlob = RSAalg.ExportCspBlob(includePrivateKey);
            string keysData = Convert.ToBase64String(cspBlob);
            System.IO.File.WriteAllText(filePath, keysData);

            //export only public key from same RSAalg object keys pair
            byte[] cspPublicBlob = RSAalg.ExportCspBlob(false);
            string publicKeyData = Convert.ToBase64String(cspPublicBlob);
            System.IO.File.WriteAllText(filePath.Replace(".bin", ".json"), "[{\"nodeid\":\"" + nodeId + "\", \"key\":\"" + publicKeyData + "\"}]");
        }

        public  byte[] importCspParam(string filePath)
        {
            string contents = File.ReadAllText(filePath);
            return Convert.FromBase64String(contents);
        }
        public  string signData(string data)
        {
            try
            {
                ASCIIEncoding byteConverter = new ASCIIEncoding();
                byte[] dataToSign = byteConverter.GetBytes(data);
                byte[] sign = RSAalg.SignData(dataToSign, new SHA256CryptoServiceProvider());
                return Convert.ToBase64String(sign);
            }
            catch (CryptographicException e)
            {
                Console.WriteLine(e.Message);
                return "";
            }
        }

         public bool VerifySignedData(string payload, string signature){
            try
            {
                ASCIIEncoding byteConverter = new ASCIIEncoding();
                byte[] payloadBytes = byteConverter.GetBytes(payload);
                byte[] signatureBytes = Convert.FromBase64String(signature);
                return RSAalg.VerifyData(payloadBytes, new SHA256CryptoServiceProvider(), signatureBytes); 

            }
            catch(CryptographicException e)
            {
                Console.WriteLine(e.Message);

                return false;
            }
        }
    }
}