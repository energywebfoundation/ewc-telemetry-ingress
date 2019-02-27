using System;
using System.IO;
using System.Net;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace webapi
{
    public class Program
    {
        private static string GetConfigFromEnvironment(string name, string defaultValue)
        {
            string value = Environment.GetEnvironmentVariable(name);
            return String.IsNullOrWhiteSpace(value) ? defaultValue : value;
        }

        
        public static void Main(string[] args)
        {

            string certificatePath = Path.Combine(GetConfigFromEnvironment("TELEMETRY_INTERNAL_DIR", "./"),"telemetry-ingress.pfx");
            if (!File.Exists(certificatePath))
            {
                Console.WriteLine($"Error: Unable to read certificate from {certificatePath}. File not found");
                return;
            }

            string keyPassword = GetConfigFromEnvironment("TELEMETRY_KEYPASS", String.Empty);
            if (String.IsNullOrWhiteSpace(keyPassword))
            {
                Console.WriteLine($"Error: Certificate password not provided.");
                return;
            }
            
            IWebHost host = WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                 .UseKestrel(options =>
                    {
                        options.Listen(IPAddress.Loopback, 5000);
                        options.Listen(IPAddress.Loopback, 5001, listenOptions =>
                        {
                            listenOptions.UseHttps(certificatePath, keyPassword );
                        });
                    })

                .Build();

            host.Run();
        }
    }
}
