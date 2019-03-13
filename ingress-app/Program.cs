using System;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.Extensions.Configuration;

namespace webapi
{
    /// <summary>
    /// Class with entry point of project
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Program entry point
        /// </summary>
        /// <param name="args">Command Line arguments, 
        /// INTERNAL_DIR: for setting internal dir path.
        /// KEYPASS: for passwod for decryption of SSL Certificate
        /// STARTSERVICE: flag for indication of starting WebHost, Added for unit testing Program class, as we donot want to actually invoke host.Run in unit tests
        /// validator: to be used with add or remove command line arg for adding into or removing from key source
        /// publickey: to be used with add or remove command line arg for adding into or removing from key source
        /// add: adding into key source
        /// remove: removing into key source
        /// </param>
        public static void Main(string[] args)
        {
            // build config
            string envName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            IConfigurationRoot config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{envName}.json", true)
                .AddEnvironmentVariables("TELEMETRY_")
                .AddInfluxConfigFromEnvironment()
                .AddCommandLine(args)
                .Build();


            // If we run a key management function we don't go into daemon mode
            string keyCommandMode = config.GetValue<string>("keycmd", String.Empty);
            if (!String.IsNullOrWhiteSpace(keyCommandMode))
            {
                // TODO: Keystore not taken from singleton as this happens later on
                var keystore = JsonPublicKeySource.FromFile(
                    Path.Combine(config.GetValue<string>("INTERNAL_DIR", "./"), "keyfile.json"), true);

                var keymgr = new KeyManagement(config, keystore);
                keymgr.ProcessKeyCommand(keyCommandMode);
                return;
            }


            // print config
            // TODO: remove after done
            foreach (var c in config.AsEnumerable())
            {
                Console.WriteLine($"{c.Key} ==> {c.Value}");
            }

            //Getting certificate path for SSL
            string certificatePath = Path.GetFullPath(Path.Combine(config.GetValue("INTERNAL_DIR", "./"), "telemetry-ingress.pfx"));

            //verify if certificate exists on given path
            if (!File.Exists(certificatePath))
            {
                Console.WriteLine($"Error: Unable to read certificate from {certificatePath}. File not found");
                return;
            }

            //get certificate password
            string keyPassword = config.GetValue("KEYPASS", String.Empty);

            //certificate key password validation
            if (String.IsNullOrWhiteSpace(keyPassword))
            {
                Console.WriteLine($"Error: Certificate password not provided.");
                return;
            }

            // Configure and instantiate Web Host 
            IWebHost host = new WebHostBuilder()
                .UseConfiguration(config)
                .UseStartup<Startup>()
                .UseKestrel(options =>
                {
                    options.Listen(IPAddress.Any, 5000);
                    options.Listen(IPAddress.Any, 5010,
                        listenOptions => { listenOptions.UseHttps(certificatePath, keyPassword); });
                })
                .Build();

            //Added for unit testing Program class, as we donot want to actually invoke host.Run in unit tests
            string startSignal = config.GetValue("STARTSERVICE", String.Empty);
            if (String.IsNullOrWhiteSpace(startSignal))
            {
                host.Run();
            }
        }

    }
}