using System;

namespace webapi
{
    public class LineProtocolConParams
    {
        public Uri Address { get; set; }

        public string DBName { get; set; }

        public string User { get; set; }

        public string Password { get; set; }

        public bool UseGzipCompression { get; set; }
    }
}