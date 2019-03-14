using System.Collections.Generic;
using Newtonsoft.Json;

namespace webapi
{
    /// <summary>
    /// The model class for Influx telemetry
    /// </summary>
    public class InfluxTelemetry
    {
        /// <summary>
        /// The property for nodeid
        /// </summary>
        [JsonProperty("nodeid")]
        public string NodeId { get; set; }

        /// <summary>
        ///  The property for payload
        /// </summary>
        [JsonProperty("payload")]
        public IList<string> Payload { get; set; }

        /// <summary>
        ///  The property for signature
        /// </summary>
        [JsonProperty("signature")]
        public string Signature { get; set; }
    }
}