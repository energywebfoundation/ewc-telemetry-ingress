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
    public class Program
    {
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
                
                var keymgr = new KeyManagement(config,keystore);
                keymgr.ProcessKeyCommand(keyCommandMode);
                return;
            }
            
            
            // print config
            // TODO: remove after done
            foreach (var c in config.AsEnumerable())
            {
                Console.WriteLine($"{c.Key} ==> {c.Value}");
            }

            string certificatePath = Path.GetFullPath(Path.Combine(config.GetValue("INTERNAL_DIR","./"),"telemetry-ingress.pfx"));
            
            if (!File.Exists(certificatePath))
            {
                Console.WriteLine($"Error: Unable to read certificate from {certificatePath}. File not found");
                return;
            }

            string keyPassword = config.GetValue("KEYPASS", String.Empty);
            if (String.IsNullOrWhiteSpace(keyPassword))
            {
                Console.WriteLine($"Error: Certificate password not provided.");
                return;
            }

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

            host.Run();
        }
       
    }
}