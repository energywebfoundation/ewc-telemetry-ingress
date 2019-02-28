using System;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
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
                .Build();
            
            // print config
            foreach (var c in config.AsEnumerable())
            {
                Console.WriteLine($"{c.Key} ==> {c.Value}");
            }
            
            string certificatePath = Path.Combine(config.GetValue("INTERNAL_DIR","./"),"telemetry-ingress.pfx");
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

            IWebHost host = WebHost.CreateDefaultBuilder(args)
                .UseConfiguration(config)
                .UseStartup<Startup>()
                .UseKestrel(options =>
                {
                    options.Listen(IPAddress.Loopback, 5000);
                    options.Listen(IPAddress.Loopback, 5001,
                        listenOptions => { listenOptions.UseHttps(certificatePath, keyPassword); });
                })
                .Build();

            host.Run();
        }
    }
}