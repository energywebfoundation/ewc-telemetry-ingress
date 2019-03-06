using Newtonsoft.Json;

namespace webapi
{
    public class RealTimeTelemetry
    {
        [JsonProperty("nodeid")]
        public string NodeId { get; set; }

        [JsonProperty("payload")]
        public RealTimeTelemetryPayload Payload { get; set; }

        [JsonProperty("signature")]
        public string Signature { get; set; }
    }

    public class RealTimeTelemetryPayload
    {
        [JsonProperty("client")]
        public string Client { get; set; }

        [JsonProperty("blocknum")]
        public ulong BlockNum { get; set; }

        [JsonProperty("blockhash")]
        public string BlockHash { get; set; }

        [JsonProperty("blockts")]
        public ulong BlockTS { get; set; }

        [JsonProperty("blockreceived")]
        public ulong BlockReceived { get; set; }

        [JsonProperty("numpeers")]
        public ushort NumPeers { get; set; }

        [JsonProperty("numtxinblock")]
        public ushort NumTxInBlock { get; set; }
    }
}