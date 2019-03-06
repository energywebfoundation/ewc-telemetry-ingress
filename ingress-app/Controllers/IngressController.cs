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
    [Route("api/ingress")]
    [ApiController]
    public class IngressController : ControllerBase
    {
        private readonly IPublickeySource _keyStore;
        private readonly IInfluxClient _influx;


        public IngressController(IPublickeySource keystore, IInfluxClient influx)
        {
            _keyStore = keystore;
            _influx = influx;
        }

        // POST api/values
        [HttpPost("influx")]
        public ActionResult PostInfluxTelemetry([FromBody] InfluxTelemetry telemetryPackage)
        {
            // verify
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
                _influx.Enqueue(telemetryPackage.Payload , true);
            }
            catch (Exception ex)
            {
                //return BadRequest(ex.ToString());
                //TODO Logging error instead of sending back to client
                Console.WriteLine("ERROR: unable to enqueue: {0}" , ex.ToString());
               return StatusCode(400);
            }

            return Accepted();
        }

         [HttpPost("realtime")]
        public ActionResult PostRealTimeTelemetry([FromBody] RealTimeTelemetry realTimePackage)
        {
            // verify
            if (realTimePackage?.NodeId == null ||
                string.IsNullOrWhiteSpace(realTimePackage.Signature) ||
                realTimePackage.Payload == null ||
                string.IsNullOrWhiteSpace(realTimePackage.Payload.Client) ||
                realTimePackage.Payload?.BlockNum==null || realTimePackage.Payload?.BlockNum<=0 ||
                string.IsNullOrWhiteSpace(realTimePackage.Payload.BlockHash) ||
                realTimePackage.Payload?.BlockTS==null ||
                realTimePackage.Payload?.BlockReceived==null ||
                realTimePackage.Payload?.NumPeers==null || realTimePackage.Payload?.NumPeers<0 ||
                realTimePackage.Payload?.NumTxInBlock==null || realTimePackage.Payload?.NumTxInBlock<0
                )
            {
                Console.WriteLine("bad request");
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

                Console.WriteLine($"Accepted telemetry from {realTimePackage.NodeId} ]");
                string influxPoint = string.Format("parity,nodeid={0},client={1} blocknum={2},numpeers={3},blockts={4},numtxinblock={4},propagationtime={5} {6}",
                        realTimePackage.NodeId,
                        realTimePackage.Payload.Client,
                        realTimePackage.Payload.BlockNum,
                        realTimePackage.Payload.NumPeers,
                        realTimePackage.Payload.BlockTS,
                        realTimePackage.Payload.NumTxInBlock,
                        (realTimePackage.Payload.BlockReceived - realTimePackage.Payload.BlockTS),
                        realTimePackage.Payload.BlockReceived);
                // Signature valid - record to db
                
                _influx.Enqueue(influxPoint , true);
            }
            catch (Exception ex)
            {
                //return BadRequest(ex.ToString());
                //TODO Logging error instead of sending back to client
                Console.WriteLine("ERROR: unable to enqueue: {0}" , ex.ToString());
               return StatusCode(400);
            }

            return Accepted();
        }

    }
}
