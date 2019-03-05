using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

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
                Console.WriteLine("ERROR: unable to enqueue: " + ex);
               return StatusCode(400);
            }

            return Accepted();
        }

    }
}
