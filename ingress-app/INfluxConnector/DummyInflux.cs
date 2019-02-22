using System;
using System.Collections.Generic;
using webapi;

namespace webapi.Controllers
{
    public class DummyInflux : IInfluxConnector
    {

        public async void Record(List<string> influxLines)
        {
            var options = new LineProtocolConParams
            {
                Address = new System.Uri("http://localhost:8086"),
                DBName = "testdb",
                User = "root",
                Password = "root",
                UseGzipCompression = true
            };

            var lineProtocolClient = new LineProtocolClient(options);

            var res ="";
            foreach(var line in influxLines){
                res+= await lineProtocolClient.WriteAsync(line);}

            Console.WriteLine($"Would have recorded {influxLines.Count} records.");
            Console.WriteLine(res);
            LastInsertCount = influxLines.Count;
        }

        public int LastInsertCount { get; private set; } = 0;
    }
}