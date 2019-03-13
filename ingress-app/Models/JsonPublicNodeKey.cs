using Newtonsoft.Json;

namespace webapi
{
    /// <summary>
    /// The model class for Json Public Node Key
    /// </summary>
    public class JsonPublicNodeKey
    {

        /// <summary>
        /// The property for nodeid
        /// </summary>
        [JsonProperty("nodeid")]
        public string NodeId { get; set; }

        /// <summary>
        /// The property for public key
        /// </summary>
        [JsonProperty("key")]
        public string PublicKey { get; set; }
    }
}