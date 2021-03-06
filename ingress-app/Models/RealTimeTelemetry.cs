using Newtonsoft.Json;

namespace webapi
{
    /// <summary>
    /// The model class for Real Time Telemetry
    /// </summary>
    public class RealTimeTelemetry
    {
        /// <summary>
        /// The property for nodeid
        /// </summary>
        [JsonProperty("nodeid")]
        public string NodeId { get; set; }

        /// <summary>
        /// The property for payload
        /// </summary>
        [JsonProperty("payload")]
        public RealTimeTelemetryPayload Payload { get; set; }

        /// <summary>
        /// The property for signature
        /// </summary>
        [JsonProperty("signature")]
        public string Signature { get; set; }
    }

    /// <summary>
    /// The model class for Real Time Telemetry Payload
    /// </summary>
    public class RealTimeTelemetryPayload
    {
        /// <summary>
        /// The property for client
        /// </summary>
        [JsonProperty("client")]
        public string Client { get; set; }

        /// <summary>
        /// The property for block num
        /// </summary>
        [JsonProperty("blocknum")]
        public ulong BlockNum { get; set; }

        /// <summary>
        /// The property for block hash
        /// </summary>
        [JsonProperty("blockhash")]
        public string BlockHash { get; set; }

        /// <summary>
        /// The property for block Time Stamp
        /// </summary>
        [JsonProperty("blockts")]
        public ulong BlockTS { get; set; }

        /// <summary>
        /// The property for block received Time Stamp
        /// </summary>
        [JsonProperty("blockreceived")]
        public ulong BlockReceived { get; set; }

        /// <summary>
        /// The property for Num of Peers
        /// </summary>
        [JsonProperty("numpeers")]
        public ushort NumPeers { get; set; }

        /// <summary>
        /// The property for Num of Transactions in block
        /// </summary>
        [JsonProperty("numtxinblock")]
        public ushort NumTxInBlock { get; set; }
        
        /// <summary>
        /// Gaslimit of the block
        /// </summary>
        [JsonProperty("gaslimit")]
        public long GasLimit { get; set; }

        /// <summary>
        /// Amount of gas used inside the block
        /// </summary>
        [JsonProperty("gasused")]
        public long GasUsed { get; set; }
    }
}