using System.Collections.Generic;
using Newtonsoft.Json;

namespace webapi.Controllers
{
    public class InfluxTelemetry
    {
        [JsonProperty("nid")]
        public string NodeId { get; set; } 
        [JsonProperty("lines")]
        public List<string> InfluxLines { get; set; }
        [JsonProperty("sig")]
        public string Signature { get; set; }    
    }
}