using System.Net;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace webapi
{
    public class Program
    {
        public static void Main(string[] args)
        {

            IWebHost host = WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                 .UseKestrel(options =>
                    {
                        options.Listen(IPAddress.Loopback, 5000);
                        options.Listen(IPAddress.Loopback, 5001, listenOptions =>
                        {
                            // TODO: need to find better way to store the cert password.
                            listenOptions.UseHttps("telemetry-ingress.pfx", "EDKHDKxCEkiGGJd4kTRj7k6");
                        });
                    })

                .Build();

            host.Run();
        }
    }
}
