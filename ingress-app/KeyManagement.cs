using System;
using Microsoft.Extensions.Configuration;

namespace webapi
{
    /// <summary>
    /// The class for Key Management having functionality like adding or removing keys based on provided command
    /// </summary>
    public class KeyManagement
    {
        private readonly IConfigurationRoot _config;
        private readonly IPublickeySource _keystore;

        /// <summary>
        /// Constructor of KeyManagement
        /// </summary>
        /// <param name="config">Expects ConfigurationRoot as input</param>
        /// <param name="keystore">Expects PublickeySource as input</param>
        /// <returns>Instance of KeyManagement</returns>
        public KeyManagement(IConfigurationRoot config, IPublickeySource keystore)
        {
            _config = config;
            _keystore = keystore;
        }

        /// <summary>
        /// This function adds key and node Id to key source.
        /// </summary>
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

        /// <summary>
        /// This function removes key and node Id from key source.
        /// </summary>
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

            //remove validator from key store
            _keystore.RemoveKey(validator);
            Console.WriteLine($"Key for {validator} removed.");

        }

        /// <summary>
        /// Processes provided command for adding or removing key
        /// </summary>
        /// <param name="keyCommandMode">Input command 'add' for adding key, and 'remove' for removing key</param>
        public void ProcessKeyCommand(string keyCommandMode)
        {
            //Process incoming command for key adding or removing from keystore
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