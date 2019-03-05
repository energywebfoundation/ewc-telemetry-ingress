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
        public async Task<ActionResult> PostInfluxTelemetry([FromBody] InfluxTelemetry telemetryPackage)
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

         [HttpPost("influx")]
        public async Task<ActionResult> PostRealTimeTelemetry([FromBody] RealTimeTelemetry realTimePackage)
        {
            // verify
            if (realTimePackage?.NodeId == null ||
                string.IsNullOrWhiteSpace(realTimePackage.Signature) ||
                realTimePackage.Payload == null ||
                string.IsNullOrWhiteSpace(realTimePackage.Payload.Client) ||
                realTimePackage.Payload?.BlockNum==null || realTimePackage.Payload?.BlockNum<=0 ||
                string.IsNullOrWhiteSpace(realTimePackage.Payload.BlockHash) ||
                realTimePackage.Payload?.BlockTS==null || realTimePackage.Payload?.BlockTS<=0 ||
                realTimePackage.Payload?.BlockReceived==null || realTimePackage.Payload?.BlockReceived<=0 ||
                realTimePackage.Payload?.NumPeers==null || realTimePackage.Payload?.NumPeers<0 
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

                Console.WriteLine($"Accepted telemetry from {realTimePackage.NodeId} ]");
                string influxPoint = string.Format("{0},numpeers={1},blockreceived={2} blocknum={3},blockhash={4} {5}",
                        realTimePackage.Payload.Client,
                        realTimePackage.Payload.NumPeers,
                        realTimePackage.Payload.BlockReceived,
                        realTimePackage.Payload.BlockNum,
                        realTimePackage.Payload.BlockHash,
                        realTimePackage.Payload.BlockTS);
                // Signature valid - record to db
                List<string> pointList = new List<string>();
                pointList.Add(influxPoint);
                _influx.Enqueue(pointList , true);
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
