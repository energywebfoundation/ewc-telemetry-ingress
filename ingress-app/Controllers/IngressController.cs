using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace webapi.Controllers
{
    /// <summary>
    /// IngressController exposes Restful endpoints having features as Telemetry verification, validation, security and persistance to Influx
    /// </summary>

    [Route("api/ingress")]
    [ApiController]
    public class IngressController : ControllerBase
    {
        private readonly IPublickeySource _keyStore;
        private readonly IInfluxClient _influx;


        /// <summary>
        /// IngressController constructor for Controller initialization
        /// </summary>
        /// <param name="keystore">PublickeySource instance reference</param>
        /// <param name="influx">InfluxClient instance reference, that will be used for data persistance</param>
        /// <returns>returns instance of IngressController</returns>
        public IngressController(IPublickeySource keystore, IInfluxClient influx)
        {
            _keyStore = keystore;
            _influx = influx;
        }

        /// <summary>
        /// Restful endpoint - POST api/ingress/influx for telemetry persistance to influx after validation and signature verification.
        /// The method only enqueues incoming data into worker buffer, so once buffer flush trigger is called then actual data is sent to Influx.
        /// In case worker buffer fails then data is flushed to failure handler buffer. Buffers flushing settings can be configured using appsettings.json
        /// </summary>
        /// <param name="telemetryPackage">Expects InfluxTelemetry JSON object in form body</param>
        /// <returns>returns instance of ActionResult HTTP status code (400: Bad Request, 403: Unauthorized, 202: Accepted)</returns>
        [HttpPost("influx")]
        public ActionResult PostInfluxTelemetry([FromBody] InfluxTelemetry telemetryPackage)
        {
            // verify in coming data
            if (telemetryPackage?.NodeId == null ||
                string.IsNullOrWhiteSpace(telemetryPackage.Signature) ||
                telemetryPackage.Payload == null ||
                telemetryPackage.Payload.Count == 0)
            {
                Console.WriteLine("bad request");
                return BadRequest();
            }

            // Get Node key from keystore
            string nodeKey;
            try
            {
                nodeKey = _keyStore.GetKeyForNode(telemetryPackage.NodeId);
            }
            catch (KeyNotFoundException)
            {
                Console.WriteLine($"Node Unknown: {telemetryPackage.NodeId}");
                return StatusCode(403);
            }

            // Verify Signature
            string signedPayload = string.Join("", telemetryPackage.Payload);
            bool signatureValid = SignatureVerifier.IsSignatureValid(signedPayload, telemetryPackage.Signature, nodeKey);

            if (!signatureValid)
            {
                Console.WriteLine($"Bad signature from node: {telemetryPackage.NodeId}");
                return StatusCode(403);
            }

            try
            {

                Console.WriteLine($"Accepted telemetry from {telemetryPackage.NodeId} [{telemetryPackage.Payload.Count} metrics]");
                // Signature valid - record to db
                _influx.Enqueue(telemetryPackage.Payload, true);
            }
            catch (Exception ex)
            {

                Console.WriteLine("ERROR: Unable to enqueue: {0}", ex.ToString());
                return StatusCode(400);
            }

            return Accepted();
        }


        /// <summary>
        /// Restful endpoint - POST api/ingress/realtime for realtime telemetry persistance to influx after validation and signature verification.
        /// The method only enqueues incoming data into worker buffer, so once buffer flush trigger is called then actual data is sent to Influx.
        /// In case worker buffer fails then data is flushed to failure handler buffer. Buffers flushing settings can be configured using appsettings.json
        /// </summary>
        /// <param name="realTimePackage">Expects RealTimeTelemetry JSON object in form body</param>
        /// <returns>returns instance of ActionResult HTTP status code (400: Bad Request, 403: Unauthorized, 202: Accepted)</returns>
        [HttpPost("realtime")]
        public ActionResult PostRealTimeTelemetry([FromBody] RealTimeTelemetry realTimePackage)
        {
            // verify incoming data
            if (realTimePackage?.NodeId == null ||
                string.IsNullOrWhiteSpace(realTimePackage.Signature) ||
                realTimePackage.Payload == null ||
                string.IsNullOrWhiteSpace(realTimePackage.Payload.Client) ||
                realTimePackage.Payload?.BlockNum == null || realTimePackage.Payload?.BlockNum < 0 ||
                string.IsNullOrWhiteSpace(realTimePackage.Payload.BlockHash) ||
                realTimePackage.Payload?.BlockTS == null ||
                realTimePackage.Payload?.BlockReceived == null ||
                realTimePackage.Payload?.NumPeers == null || realTimePackage.Payload?.NumPeers < 0 ||
                realTimePackage.Payload?.NumTxInBlock == null || realTimePackage.Payload?.NumTxInBlock < 0
                )
            {
                Console.WriteLine("Bad Request");
                return BadRequest();
            }

            // Get Node key from keystore
            string nodeKey;
            try
            {
                nodeKey = _keyStore.GetKeyForNode(realTimePackage.NodeId);
            }
            catch (KeyNotFoundException)
            {
                Console.WriteLine($"Node Unknown: {realTimePackage.NodeId}");
                return StatusCode(403);
            }

            // Verify Signature
            string payload = JsonConvert.SerializeObject(realTimePackage.Payload);
            bool signatureValid = SignatureVerifier.IsSignatureValid(payload, realTimePackage.Signature, nodeKey);

            if (!signatureValid)
            {
                Console.WriteLine($"Bad signature from node: {realTimePackage.NodeId}");
                return StatusCode(403);
            }

            try
            {
                //Point format |measurement|,tag_set| |field_set| |timestamp|

                //create a point from incoming JSON
                Console.WriteLine($"Accepted RT telemetry from {realTimePackage.NodeId} ");
                string influxPoint = string.Format("parity,nodeid={0},client={1} blocknum={2},numpeers={3},blockts={4},numtxinblock={5},propagationtime={6} {7}",
                        realTimePackage.NodeId,
                        realTimePackage.Payload.Client,
                        realTimePackage.Payload.BlockNum,
                        realTimePackage.Payload.NumPeers,
                        realTimePackage.Payload.BlockTS,
                        realTimePackage.Payload.NumTxInBlock,
                        (realTimePackage.Payload.BlockReceived - realTimePackage.Payload.BlockTS),
                        realTimePackage.Payload.BlockReceived);

                // Signature valid - record to db
                _influx.Enqueue(influxPoint, true);
            }
            catch (Exception ex)
            {

                Console.WriteLine("ERROR: unable to enqueue RTT: {0}", ex.ToString());
                return StatusCode(400);
            }

            return Accepted();
        }

    }
}
