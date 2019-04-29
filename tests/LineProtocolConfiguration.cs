using System;
using Microsoft.Extensions.Configuration;
using webapi;

namespace tests
{
    public static class LineProtocolConfiguration
    {
        public static LineProtocolConnectionParameters InitConfiguration()
        {
            var confFileobj = new LineProtocolConnectionParameters
            { 
                Address = new Uri("http://influxdb:8086"), 
                DBName = "telemetry", 
                User = "root", 
                Password = "root", 
                FlushBufferItemsSize = 2, 
                FlushBufferSeconds = 2, 
                FlushSecondBufferItemsSize = 2, 
                FlushSecondBufferSeconds = 3, 
                UseGzipCompression = true };


            return confFileobj;
        }
    }
}
