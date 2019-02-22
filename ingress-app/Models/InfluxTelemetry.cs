using System.Collections.Generic;
using Newtonsoft.Json;

namespace webapi
{
    public class InfluxTelemetry
    {
        [JsonProperty("nodeid")]
        public string NodeId { get; set; } 
        [JsonProperty("payload")]
        public List<string> Payload { get; set; }
        [JsonProperty("signature")]
        public string Signature { get; set; }    
    }
}