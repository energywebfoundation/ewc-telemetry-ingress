using System;

namespace webapi
{
    /// <summary>
    /// The model class for Influx Connection Parameters
    /// </summary>
    public class LineProtocolConnectionParameters
    {
        /// <summary>
        /// The property for Influx URI
        /// </summary>
        public Uri Address { get; set; }

        /// <summary>
        /// The property for Influx database name
        /// </summary>
        public string DBName { get; set; }

        /// <summary>
        /// The property for Influx User
        /// </summary>
        public string User { get; set; }

        /// <summary>
        /// The property for Influx User password
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// The property for Worker buffer Item length trigger point
        /// </summary>
        public int FlushBufferItemsSize { get; set; }

        /// <summary>
        /// The property for Worker buffer Item length trigger point
        /// </summary>
        public int FlushBufferSeconds { get; set; }

        /// <summary>
        /// The property for Failure Handler buffer Item length trigger point
        /// </summary>
        public int FlushSecondBufferItemsSize { get; set; }

        /// <summary>
        /// The property for Failure Handler buffer Item length trigger point
        /// </summary>
        public int FlushSecondBufferSeconds { get; set; }

        /// <summary>
        /// The property for using compression flag
        /// </summary>
        public bool UseGzipCompression { get; set; }
    }
}