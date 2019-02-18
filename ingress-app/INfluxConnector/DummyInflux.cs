using System;
using System.Collections.Generic;

namespace webapi.Controllers
{
    public class DummyInflux : IInfluxConnector
    {
        public void Record(List<string> influxLines)
        {
            Console.WriteLine($"Would have recorded {influxLines.Count} records.");
            LastInsertCount = influxLines.Count;
        }

        public int LastInsertCount { get; private set; } = 0;
    }
}