using System;
using Microsoft.Extensions.Configuration;
using webapi;

namespace tests
{
    public class LineProtocolConfiguration
    {
        public static LineProtocolConnectionParameters InitConfiguration()
        {
            bool fromFile = false;
            LineProtocolConnectionParameters confFileobj = null;

            if (fromFile)
            {
                ConfigurationBuilder cb = new ConfigurationBuilder();
                cb.SetBasePath(System.AppContext.BaseDirectory);
                cb.AddJsonFile("appsettings.test.json");
                IConfigurationRoot cr = cb.Build();
                confFileobj = cr.GetSection("Influx").Get<LineProtocolConnectionParameters>();

            }else{

                confFileobj = new LineProtocolConnectionParameters() { 
                    Address = new Uri("http://influxdb:8086"), 
                    DBName = "telemetry", 
                    User = "root", 
                    Password = "root", 
                    FlushBufferItemsSize = 2, 
                    FlushBufferSeconds = 2, 
                    FlushSecondBufferItemsSize = 2, 
                    FlushSecondBufferSeconds = 3, 
                    UseGzipCompression = true };
            }

            return confFileobj;
        }
    }
}
