using System;
using Microsoft.Extensions.Configuration;

namespace webapi
{
    public class KeyManagement
    {
        private readonly IConfigurationRoot _config;
        private readonly IPublickeySource _keystore;

        public KeyManagement(IConfigurationRoot config, IPublickeySource keystore)
        {
            _config = config;
            _keystore = keystore;
        }

        private void AddKey()
        {
            //Adding key into key store

            //first getting validator from provided config
            string validator = _config.GetValue<string>("validator", null);

            //getting public key from provided config
            string pubkeyAsBase64 = _config.GetValue<string>("publickey", null);

            //validation checks for validator and public keys
            if (string.IsNullOrWhiteSpace(validator))
            {
                Console.WriteLine("ERR: Please provide --validator");
                return;
            }
            
            if (string.IsNullOrWhiteSpace(pubkeyAsBase64))
            {
                Console.WriteLine("ERR: Please provide --publickey");
                return;
            }

            // if key is valid then add into key store
            _keystore.AddKey(validator, pubkeyAsBase64);
            Console.WriteLine($"Key for {validator} added.");

        }
        
        private void RemoveKey()
        {
            // Removing key from key store

            //getting validator to be removed
            string validator = _config.GetValue<string>("validator", null);

            //validation check
            if (string.IsNullOrWhiteSpace(validator))
            {
                Console.WriteLine("ERR: Please provide --validator");
                return;
            }

            //remvove validator from key store
            _keystore.RemoveKey(validator);
            Console.WriteLine($"Key for {validator} removed.");

        }
        
        public void ProcessKeyCommand(string keyCommandMode)
        {
            //Pocess incoming command for key adding or removing from keystore
            Console.WriteLine("Telemetry Ingress - Key Management Mode");
            switch (keyCommandMode)
            {
                case "add":
                    // check
                    AddKey();
                    break;

                case "remove":
                    // check
                    RemoveKey();
                    break;
                    
                default:
                    Console.WriteLine("ERR: Unknown key command");
                    return;
            }
        }
    }
}