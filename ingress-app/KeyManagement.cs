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
            string validator = _config.GetValue<string>("validator", null);
            string pubkeyAsBase64 = _config.GetValue<string>("publickey", null);

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

            _keystore.AddKey(validator, pubkeyAsBase64);
            Console.WriteLine($"Key for {validator} added.");

        }
        
        private void RemoveKey()
        {
            string validator = _config.GetValue<string>("validator", null);

            if (string.IsNullOrWhiteSpace(validator))
            {
                Console.WriteLine("ERR: Please provide --validator");
                return;
            }

            _keystore.RemoveKey(validator);
            Console.WriteLine($"Key for {validator} removed.");

        }
        
        public void ProcessKeyCommand(string keyCommandMode)
        {
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