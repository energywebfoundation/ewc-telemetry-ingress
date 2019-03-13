using System;
using System.Collections.Generic;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using webapi;
using webapi.Controllers;
using Xunit;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Linq;

namespace tests
{
    public class IngressRealTimeTelemetryTests
    {

        [Fact]
        public void NullTelemetryShouldNotRecord()
        {
            var conobj = LineProtocolConfiguration.InitConfiguration();
            var influxLib = new InfluxClient(conobj);
            var keystore = new MockKeystore();

            keystore.AddKey("node-3", "BgIAAACkAABSU0ExAAQAAAEAAQBXZXt7QOileknWzBH2Sg+Yk4INDTbKA5XUUfUe23zUmr6eM1USCNHX3lidZfjk5Emuui1m8k0KnghxcJfOau8iPRpLg/lubMNojpLGe2MXn5GsyjgEpVdE+Cf0pLBAYHcBuBYHj99muMsJrJW1/InbKFa24JuVnBr+MybPuMXqtQ==");

            IngressController tc = new IngressController(keystore, influxLib);
            ActionResult webResponse = tc.PostRealTimeTelemetry(
                new RealTimeTelemetry
                {
                    NodeId = null,
                    Payload = new RealTimeTelemetryPayload
                    {
                        Client = null,
                        BlockNum = 9,
                        BlockHash = "",
                        BlockTS = 123,
                        BlockReceived = 123,
                        NumPeers = 123,
                        NumTxInBlock = 12
                    },
                    Signature = ""
                });

            Assert.NotNull(webResponse);
            var result = Assert.IsType<BadRequestResult>(webResponse);
            Assert.Equal((int)HttpStatusCode.BadRequest, result.StatusCode);
            Assert.Equal(0, influxLib.LastInsertCount);

        }

        [Fact]
        public void NullDataShouldNotRecord()
        {
            var conobj = LineProtocolConfiguration.InitConfiguration();
            var influxLib = new InfluxClient(conobj);
            var keystore = new MockKeystore();

            keystore.AddKey("node-3", "BgIAAACkAABSU0ExAAQAAAEAAQBXZXt7QOileknWzBH2Sg+Yk4INDTbKA5XUUfUe23zUmr6eM1USCNHX3lidZfjk5Emuui1m8k0KnghxcJfOau8iPRpLg/lubMNojpLGe2MXn5GsyjgEpVdE+Cf0pLBAYHcBuBYHj99muMsJrJW1/InbKFa24JuVnBr+MybPuMXqtQ==");

            IngressController tc = new IngressController(keystore, influxLib);
            ActionResult webResponse = tc.PostRealTimeTelemetry(
                null);

            Assert.NotNull(webResponse);
            var result = Assert.IsType<BadRequestResult>(webResponse);
            Assert.Equal((int)HttpStatusCode.BadRequest, result.StatusCode);
            Assert.Equal(0, influxLib.LastInsertCount);
        }

        [Fact]
        public void NullPayloadShouldNotRecord()
        {
            var conobj = LineProtocolConfiguration.InitConfiguration();
            var influxLib = new InfluxClient(conobj);
            var keystore = new MockKeystore();

            keystore.AddKey("node-3", "BgIAAACkAABSU0ExAAQAAAEAAQBXZXt7QOileknWzBH2Sg+Yk4INDTbKA5XUUfUe23zUmr6eM1USCNHX3lidZfjk5Emuui1m8k0KnghxcJfOau8iPRpLg/lubMNojpLGe2MXn5GsyjgEpVdE+Cf0pLBAYHcBuBYHj99muMsJrJW1/InbKFa24JuVnBr+MybPuMXqtQ==");

            IngressController tc = new IngressController(keystore, influxLib);
            ActionResult webResponse = tc.PostRealTimeTelemetry(
                new RealTimeTelemetry
                {
                    NodeId = null,
                    Payload = null,
                    Signature = null
                });

            Assert.NotNull(webResponse);
            var result = Assert.IsType<BadRequestResult>(webResponse);
            Assert.Equal((int)HttpStatusCode.BadRequest, result.StatusCode);
            Assert.Equal(0, influxLib.LastInsertCount);

        }

        [Fact]
        public void UnRegisteredPublicKeyShouldNotRecord()
        {

            var conobj = LineProtocolConfiguration.InitConfiguration();
            var influxLib = new InfluxClient(conobj);
            var keystore = new MockKeystore();

            keystore.AddKey("node-3", "BgIAAACkAABSU0ExAAQAAAEAAQBXZXt7QOileknWzBH2Sg+Yk4INDTbKA5XUUfUe23zUmr6eM1USCNHX3lidZfjk5Emuui1m8k0KnghxcJfOau8iPRpLg/lubMNojpLGe2MXn5GsyjgEpVdE+Cf0pLBAYHcBuBYHj99muMsJrJW1/InbKFa24JuVnBr+MybPuMXqtQ==");

            IngressController tc = new IngressController(keystore, influxLib);
            ActionResult webResponse = tc.PostRealTimeTelemetry(
                new RealTimeTelemetry
                {
                    NodeId = "0x00000",
                    Payload = new RealTimeTelemetryPayload
                    {
                        Client = "Parity123",
                        BlockNum = 9,
                        BlockHash = "467234DSFG2345SDF",
                        BlockTS = 12365785,
                        BlockReceived = 1457645623,
                        NumPeers = 123,
                        NumTxInBlock = 126
                    },
                    Signature = "KI8EGFW4acwQc5V7C3MPzmGjHSguXCkE3YxRrQFlZ+G2PrFWkhno9g9NzPSq7H8wxmZMmcZcGdwWNcX0vxLM+vunp3EboHhKxe3p3FFeij6z6GcX0GCJpjyGtaS+VqIGnGKFdzPA3bNZ9QLJwROS/SQA1hjTVm8lhpWR1CQ55oc="
                });

            Assert.NotNull(webResponse);
            var result = Assert.IsType<StatusCodeResult>(webResponse);
            Assert.Equal((int)HttpStatusCode.Forbidden, result.StatusCode);
        }

        [Fact]
        public void InvalidSignatureShouldNotRecord()
        {

            var conobj = LineProtocolConfiguration.InitConfiguration();
            var influxLib = new InfluxClient(conobj);
            var keystore = new MockKeystore();
            keystore.AddKey("node-3", "BgIAAACkAABSU0ExAAQAAAEAAQBXZXt7QOileknWzBH2Sg+Yk4INDTbKA5XUUfUe23zUmr6eM1USCNHX3lidZfjk5Emuui1m8k0KnghxcJfOau8iPRpLg/lubMNojpLGe2MXn5GsyjgEpVdE+Cf0pLBAYHcBuBYHj99muMsJrJW1/InbKFa24JuVnBr+MybPuMXqtQ==");

            IngressController tc = new IngressController(keystore, influxLib);
            ActionResult webResponse = tc.PostRealTimeTelemetry(
                new RealTimeTelemetry
                {
                    NodeId = "node-3",
                    Payload = new RealTimeTelemetryPayload
                    {
                        Client = "Parity123",
                        BlockNum = 9,
                        BlockHash = "467234DSFG2345SDF",
                        BlockTS = 12365785,
                        BlockReceived = 1457645623,
                        NumPeers = 123,
                        NumTxInBlock = 126
                    },
                    Signature = "KI8EGFW4acwQc5V7C3MPzmGjHSguXCkE3YxRrQFlZ+G2PrFWkhno9g9NzPSq7H8wxmZMmcZcGdwWNcX0vxLM+vunp3EboHhKxe3p3FFeij6z6GcX0GCJpjyGtaS+VqIGnGKFdzPA3bNZ9QLJwROS/SQA1hjTVm8lhpWR1CQ55oc="
                });


            Assert.NotNull(webResponse);
            var result = Assert.IsType<StatusCodeResult>(webResponse);
            Assert.Equal((int)HttpStatusCode.Forbidden, result.StatusCode);
            Assert.Equal(0, influxLib.LastInsertCount);

        }

        [Fact]
        public void ValidTelemetryShouldRecord()
        {

            var conobj = LineProtocolConfiguration.InitConfiguration();
            conobj.FlushBufferItemsSize = 1;
            conobj.FlushBufferSeconds = 1;
            var influxLib = new InfluxClient(conobj);
            var keystore = new MockKeystore();

            keystore.AddKey("node-3", "BgIAAACkAABSU0ExAAQAAAEAAQBXZXt7QOileknWzBH2Sg+Yk4INDTbKA5XUUfUe23zUmr6eM1USCNHX3lidZfjk5Emuui1m8k0KnghxcJfOau8iPRpLg/lubMNojpLGe2MXn5GsyjgEpVdE+Cf0pLBAYHcBuBYHj99muMsJrJW1/InbKFa24JuVnBr+MybPuMXqtQ==");

            IngressController tc = new IngressController(keystore, influxLib);
            ActionResult webResponse = tc.PostRealTimeTelemetry(
                new RealTimeTelemetry
                {
                    NodeId = "node-3",
                    Payload = new RealTimeTelemetryPayload
                    {
                        Client = "Parityv123",
                        BlockNum = 123456,
                        BlockHash = "123efd",
                        BlockTS = 1465839830100400200,
                        BlockReceived = 1465839830100400200,
                        NumPeers = 12,
                        NumTxInBlock = 23
                    },
                    Signature = "RPoRe9Yfg4q5hdf/6l7YKDUBA4fT7oevQKIjbLKVjubyUmNtwoxLLmtCguoe/S0n1b6V38E551pcYKkSotZ731cSpNEkHTXyk2+okv0deRqJpFRUWZsfkVJO7iHOxfvPzYR2ybw1UUxg7/nC8R87ZkFI+mipR1mzjF+S3MZ1kEI="
                });

            Assert.NotNull(webResponse);
            var result = Assert.IsType<AcceptedResult>(webResponse);
            Assert.Equal((int)HttpStatusCode.Accepted, result.StatusCode);

            System.Threading.Thread.Sleep(conobj.FlushBufferSeconds * 1000 * 4); //wait for Queue to flush with some extra time for NW latency
            Assert.Equal(1, influxLib.LastInsertCount);

        }

        [Fact]
        public void EnqueueShouldFailForInvalidPointData()
        {

            var conobj = LineProtocolConfiguration.InitConfiguration();
            var influxLib = new InfluxClient(conobj);
            var keystore = new MockKeystore();

            keystore.AddKey("node-3", "BgIAAACkAABSU0ExAAQAAAEAAQBXZXt7QOileknWzBH2Sg+Yk4INDTbKA5XUUfUe23zUmr6eM1USCNHX3lidZfjk5Emuui1m8k0KnghxcJfOau8iPRpLg/lubMNojpLGe2MXn5GsyjgEpVdE+Cf0pLBAYHcBuBYHj99muMsJrJW1/InbKFa24JuVnBr+MybPuMXqtQ==");

            IngressController tc = new IngressController(keystore, influxLib);
            ActionResult webResponse = tc.PostRealTimeTelemetry(
                new RealTimeTelemetry
                {
                    NodeId = "node-3",
                    Payload = new RealTimeTelemetryPayload
                    {
                        Client = "'Parityv123'",
                        BlockNum = 123456,
                        BlockHash = "123efd",
                        BlockTS = 1465839830100400200,
                        BlockReceived = 1465839830100400200,
                        NumPeers = 12,
                        NumTxInBlock = 23
                    },
                    Signature = "kVfVyaGWkGyVMVyQCpX4knbjz2p6JRADNAi0lI8lvhUhS9ESfn7jm3a35ul1sjIRg1K9lOTEVL+HonfBjjqebJHdwAHvjzBiO6JARpgi6f0iLUvcCGflVZIb5fX7CfwT6Eb5kP+NThKXH3SV0zSYbHzhxX8EjyCi0fkRFpCkQmo="
                });

            Assert.NotNull(webResponse);
            var result = Assert.IsType<StatusCodeResult>(webResponse);
            Assert.Equal((int)HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Fact]
        public void PostCallShouldFailForNullData()
        {

            var conobj = LineProtocolConfiguration.InitConfiguration();
            var influxLib = new InfluxClient(conobj);
            var keystore = new MockKeystore();

            keystore.AddKey("node-3", "BgIAAACkAABSU0ExAAQAAAEAAQBXZXt7QOileknWzBH2Sg+Yk4INDTbKA5XUUfUe23zUmr6eM1USCNHX3lidZfjk5Emuui1m8k0KnghxcJfOau8iPRpLg/lubMNojpLGe2MXn5GsyjgEpVdE+Cf0pLBAYHcBuBYHj99muMsJrJW1/InbKFa24JuVnBr+MybPuMXqtQ==");

            IngressController tc = new IngressController(keystore, influxLib);
            ActionResult webResponse = tc.PostRealTimeTelemetry(null);

            Assert.NotNull(webResponse);
            var result = Assert.IsType<BadRequestResult>(webResponse);
            Assert.Equal((int)HttpStatusCode.BadRequest, result.StatusCode);
            Assert.Equal(0, influxLib.LastInsertCount);
        }
    }
}