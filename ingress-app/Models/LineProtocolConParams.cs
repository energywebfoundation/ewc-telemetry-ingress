using System;

namespace webapi
{
    public class LineProtocolConnectionParameters
    {
        public Uri Address { get; set; }

        public string DBName { get; set; }

        public string User { get; set; }

        public string Password { get; set; }

        public int FlushBufferItemsSize {get; set;}

        public int FlushBufferSeconds {get; set;}

        public int FlushSecondBufferItemsSize {get; set;}

        public int FlushSecondBufferSeconds {get; set;}

        public bool UseGzipCompression { get; set; }
    }
}