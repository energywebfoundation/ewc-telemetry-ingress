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
                return Forbid();
            }

            // Verify Signature
            string signedPayload = string.Join("", telemetryPackage.Payload);
            bool signatureValid = SignatureVerifier.IsSignatureValid(signedPayload, telemetryPackage.Signature, nodeKey);

            if (!signatureValid)
            {
                return Forbid();
            }

            try
            {
  
                // Signature valid - record to db
                await Task.Run(() => _influx.Enqueue(telemetryPackage.Payload));

            }
            catch (Exception ex)
            {
               return BadRequest(ex.ToString());
            }

            return Accepted();
        }

    }
}
