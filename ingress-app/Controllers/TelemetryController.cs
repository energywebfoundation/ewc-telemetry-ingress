using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace webapi.Controllers
{
    [Route("api/telemetry")]
    [ApiController]
    public class TelemetryController : ControllerBase
    {
        private readonly IPublickeySource _keyStore;
        private readonly IInfluxConnector _influx;

        public TelemetryController(IPublickeySource keystore, IInfluxConnector influx)
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
                telemetryPackage.InfluxLines == null || 
                telemetryPackage.InfluxLines.Count == 0)
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
            string signedPayload =  string.Join("", telemetryPackage.InfluxLines);
            bool signatureValid = SignatureVerifier.IsSignatureValid(signedPayload, telemetryPackage.Signature, nodeKey);

            if (!signatureValid)
            {
                return Forbid();
            }
            
            // Signature valid - record to db
            _influx.Record(telemetryPackage.InfluxLines);

            return Accepted();
        }

     
    }
}
