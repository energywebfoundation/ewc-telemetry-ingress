using System.Net;
using Microsoft.AspNetCore.Mvc;
using webapi;
using webapi.Controllers;
using Xunit;

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
                        Client = "Parity v123",
                        BlockNum = 123456,
                        BlockHash = "123efd",
                        BlockTS = 1465839830100400200,
                        BlockReceived = 1465839830100400200,
                        NumPeers = 12,
                        NumTxInBlock = 23,
                        GasLimit = 20,
                        GasUsed = 12

                    },
                    Signature = "B8udYvuWJ4hioMt6SJ/WxOMS7hY75QrUEKgQ+vlrt2+AblTpJwVba7TB43GvyHjimwbvvZPenJLWsS+r6j48ySGv7rM7DTYsA7SGMfwhx58gQxV2SxOx8klsiDjRPRC1e5AdrL0e1Lko/9o7OWIOdS1CUeuzfa8QhB2o41UmyuU="
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
                        Client = "ClieParity v123",
                        BlockNum = 12342256,
                        BlockHash = "123efd",
                        BlockTS = 1465839830100400200,
                        BlockReceived = 1465839830100400200,
                        NumPeers = 12,
                        NumTxInBlock = 23,
                        GasLimit = 20,
                        GasUsed = 12
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
                        NumTxInBlock = 23,
                        GasLimit = 20,
                        GasUsed = 12
                    },
                    Signature = "SDZL5noFP5XQK9rj9SqHPqCm9uEPq9rrQi0lU1SUIhjCI+UU3J3CdYIVG/XnUAf8NFFrHHwA52njhGbnxb+76q4vCruaoAFmmFqigL1MvVHj4XdENhJd5yq3URQMU+8tN9FXosOzNxmqaINKJIfvhfjN4CND4bOXzNdiQ5QrdEU="
                });

            Assert.NotNull(webResponse);
            var result = Assert.IsType<AcceptedResult>(webResponse);
            Assert.Equal((int)HttpStatusCode.Accepted, result.StatusCode);

            //System.Threading.Thread.Sleep(conobj.FlushBufferSeconds * 1000 * 4); //wait for Queue to flush with some extra time for NW latency
            //Assert.Equal(1, influxLib.LastInsertCount);

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
                        Client = "Parity v123",
                        BlockNum = 123456,
                        BlockHash = "123efd",
                        BlockTS = 1465839830100400200,
                        BlockReceived = 1465839830100400200,
                        NumPeers = 12,
                        NumTxInBlock = 23,
                        GasLimit = 20,
                        GasUsed =12
                    },
                    Signature = "B8udYvuWJ4hioMt6SJ/WxOMS7hY75QrUEKgQ+vlrt2+AblTpJwVba7TB43GvyHjimwbvvZPenJLWsS+r6j48ySGv7rM7DTYsA7SGMfwhx58gQxV2SxOx8klsiDjRPRC1e5AdrL0e1Lko/9o7OWIOdS1CUeuzfa8QhB2o41UmyuU="
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