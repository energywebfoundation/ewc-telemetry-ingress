using Newtonsoft.Json;

namespace webapi
{
    public class JsonPublicNodeKey
    {
        [JsonProperty("nodeid")]
        public string NodeId { get; set; }
        [JsonProperty("key")]
        public string PublicKey { get; set; }
    }
}