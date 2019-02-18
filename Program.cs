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
                .UseUrls("http://localhost:5000")
                .Build();

            host.Run();
        }
    }
}
