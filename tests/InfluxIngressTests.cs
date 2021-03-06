using System.Collections.Generic;
using System.Net;
using System.Threading;
using Microsoft.AspNetCore.Mvc;
using webapi;
using webapi.Controllers;
using Xunit;

namespace tests
{
    [Collection("InfluxTestCollection")]
    public class InfluxIngressTests
    {
        [Fact]
        public void EnqueueShouldFailForInvalidPoint()
        {

            var conobj = LineProtocolConfiguration.InitConfiguration();
            var influxLib = new InfluxClient(conobj);
            var keystore = new MockKeystore();

            keystore.AddKey("node-3", "BgIAAACkAABSU0ExAAQAAAEAAQBXZXt7QOileknWzBH2Sg+Yk4INDTbKA5XUUfUe23zUmr6eM1USCNHX3lidZfjk5Emuui1m8k0KnghxcJfOau8iPRpLg/lubMNojpLGe2MXn5GsyjgEpVdE+Cf0pLBAYHcBuBYHj99muMsJrJW1/InbKFa24JuVnBr+MybPuMXqtQ==");

            IngressController tc = new IngressController(keystore, influxLib);
            ActionResult webResponse = tc.PostInfluxTelemetry(new InfluxTelemetry
            {
                NodeId = "node-3",
                Signature = "h9tYfyqrRjq5yrx/7G9zyFHA5YHxr3gUondveKxLhNJevbyj5ozVOoKU2U7VCdHnm2WwO+Jr0fl2xuVqAt1DItIqbZX+7phDufMLown0DgqnzY0YLfW8pEt7xVop9CkGuqXTRZoGzpUiFidqtonwRB9uJXR/x9O3YQheoZ/Wz00=",
                Payload = new List<string>
                {
                    "weather,location=us-midwest another key=another value temperature=82 1465839830100400200"
                }
            });
            
            Assert.NotNull(webResponse);
            var result = Assert.IsType<StatusCodeResult>(webResponse);
            Assert.Equal((int)HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Fact]
        public void PostCallShouldFailForNullPoint()
        {

            var conobj = LineProtocolConfiguration.InitConfiguration();
            var influxLib = new InfluxClient(conobj);
            var keystore = new MockKeystore();

            keystore.AddKey("node-3", "BgIAAACkAABSU0ExAAQAAAEAAQBXZXt7QOileknWzBH2Sg+Yk4INDTbKA5XUUfUe23zUmr6eM1USCNHX3lidZfjk5Emuui1m8k0KnghxcJfOau8iPRpLg/lubMNojpLGe2MXn5GsyjgEpVdE+Cf0pLBAYHcBuBYHj99muMsJrJW1/InbKFa24JuVnBr+MybPuMXqtQ==");

            IngressController tc = new IngressController(keystore, influxLib);
            ActionResult webResponse = tc.PostInfluxTelemetry(null);

            Assert.NotNull(webResponse);
            var result = Assert.IsType<BadRequestResult>(webResponse);
            Assert.Equal((int)HttpStatusCode.BadRequest, result.StatusCode);
            Assert.Equal(0, influxLib.LastInsertCount);
        }

        [Fact]
        public void InvalidPublicKeyOfNodeShouldFail()
        {

            var conobj = LineProtocolConfiguration.InitConfiguration();
            var influxLib = new InfluxClient(conobj);
            var keystore = new MockKeystore();

            keystore.AddKey("0x000000000000001", "BgIAAACkAABSU0ExAAQAAAEAAQBdUkRrF0SA3a+QtGv6y97DFa79Z/IDHtCHehoj/LADUJxXsI1k6GBqdyE7MkF9uX2j8FbAMlxpmIKrMcRTWj9wZ5gIhbntiCF61IFsQJ5af23WsTg82u9A7mepxSXrfgfu6Bzq1nB+pUGeWlATaLiOT+wm5uCYjYH8MiTMfDLu4g==");

            IngressController tc = new IngressController(keystore, influxLib);
            ActionResult webResponse = tc.PostInfluxTelemetry(new InfluxTelemetry
            {
                NodeId = "node-1",
                Signature = "GT+8qiTNx2X2jtE0YQOBH6EE6Pu+a6DUFMK//LU+wiIwp/OPvaO7h2SDlU40/MAt83R4ZzVT2IBrl37phKUhbiBN0sMmvgxGJdJAOkAjKtgtacqUUxuVGim4PE6pAIAEIRoETQMe7ZlsALcoyA1p5M8Y1481bM1ykNcKQ23QPuM=",
                Payload = new List<string>
                {
                    "weather,location=us-midwest temperature=82 1465839830100400200"
                }
            });
            
            Assert.NotNull(webResponse);
            var result = Assert.IsType<StatusCodeResult>(webResponse);
            Assert.Equal((int)HttpStatusCode.Forbidden, result.StatusCode);
        }

        [Fact]
        public void ValidMetricsShouldRecord()
        {

            var conobj = LineProtocolConfiguration.InitConfiguration();
            conobj.FlushBufferItemsSize = 2;
            conobj.FlushBufferSeconds = 1;
            var influxLib = new InfluxClient(conobj);
            var keystore = new MockKeystore();

            keystore.AddKey("node-1", "BgIAAACkAABSU0ExAAQAAAEAAQBdUkRrF0SA3a+QtGv6y97DFa79Z/IDHtCHehoj/LADUJxXsI1k6GBqdyE7MkF9uX2j8FbAMlxpmIKrMcRTWj9wZ5gIhbntiCF61IFsQJ5af23WsTg82u9A7mepxSXrfgfu6Bzq1nB+pUGeWlATaLiOT+wm5uCYjYH8MiTMfDLu4g==");

            IngressController tc = new IngressController(keystore, influxLib);
            ActionResult webResponse = tc.PostInfluxTelemetry(new InfluxTelemetry
            {
                NodeId = "node-1",
                Signature = "GT+8qiTNx2X2jtE0YQOBH6EE6Pu+a6DUFMK//LU+wiIwp/OPvaO7h2SDlU40/MAt83R4ZzVT2IBrl37phKUhbiBN0sMmvgxGJdJAOkAjKtgtacqUUxuVGim4PE6pAIAEIRoETQMe7ZlsALcoyA1p5M8Y1481bM1ykNcKQ23QPuM=",
                Payload = new List<string>
                {
                    "weather,location=us-midwest temperature=82 1465839830100400200",
                    "weather,location=us-east temperature=75 1465839830100400200"
                }
            });
            
            Assert.NotNull(webResponse);
            var result = Assert.IsType<AcceptedResult>(webResponse);
            Assert.Equal((int)HttpStatusCode.Accepted, result.StatusCode);
           
            Thread.Sleep(conobj.FlushBufferSeconds * 1000 * 4); //wait for Queue to flush
            Assert.Equal(2, influxLib.LastInsertCount);

            /*JObject pobj = JObject.Parse(InfluxCon(conobj));
            var rows = pobj.SelectTokens("['results'][0].['series'][0].['values']");
            var in_count = rows.Children().Count();
            Assert.Equal(2, in_count);//Check if row count is now 2
            */
        }
 
        [Fact]
        public void InvalidSignatureShouldNotRecord()
        {

            var conobj = LineProtocolConfiguration.InitConfiguration();
            var influxLib = new InfluxClient(conobj);
            var keystore = new MockKeystore();
            keystore.AddKey("node-1", "BgIAAACkAABSU0ExAAQAAAEAAQBdUkRrF0SA3a+QtGv6y97DFa79Z/IDHtCHehoj/LADUJxXsI1k6GBqdyE7MkF9uX2j8FbAMlxpmIKrMcRTWj9wZ5gIhbntiCF61IFsQJ5af23WsTg82u9A7mepxSXrfgfu6Bzq1nB+pUGeWlATaLiOT+wm5uCYjYH8MiTMfDLu4g==");


            IngressController tc = new IngressController(keystore, influxLib);
            ActionResult webResponse = tc.PostInfluxTelemetry(new InfluxTelemetry
            {
                NodeId = "node-1",
                Signature = "GT+8qiTNx2X2jtE0YQOBH6EE6Pu+a6DUFMK//LU+wiIwp/OPvaO7h2SDlU40/MAt83R4ZzVT2IBrl37phKUhbiBN0sMmvgxGJdJAOkAjKtgtacqUUxuVGim4PE6pAIAEIRoETQMe7ZlsALcoyA1p5M8Y1481bM1ykNcKQ23QPuM=",
                Payload = new List<string>
                {
                    "weather,location=us-midwest temperature=82 1465839830100400200"
                }
            });

            //Assert.IsType<ForbidResult>(webResponse);
            Assert.NotNull(webResponse);
            var result = Assert.IsType<StatusCodeResult>(webResponse);
            Assert.Equal((int)HttpStatusCode.Forbidden, result.StatusCode);
            Assert.Equal(0, influxLib.LastInsertCount);
            /* JObject pobj = JObject.Parse(InfluxCon(conobj));
            var rows = pobj.SelectTokens("['results'][0].['series'][0].['values']");
            var in_count = rows.Children().Count();
            Assert.Equal(2, in_count);//Check if row count have not increased after first test case
        */
        }

        [Fact]
        public void NullTelemetryShouldNotRecord()
        {
            var conobj = LineProtocolConfiguration.InitConfiguration();
            var influxLib = new InfluxClient(conobj);
            var keystore = new MockKeystore();

            keystore.AddKey("node-1", "BgIAAACkAABSU0ExAAQAAAEAAQBdUkRrF0SA3a+QtGv6y97DFa79Z/IDHtCHehoj/LADUJxXsI1k6GBqdyE7MkF9uX2j8FbAMlxpmIKrMcRTWj9wZ5gIhbntiCF61IFsQJ5af23WsTg82u9A7mepxSXrfgfu6Bzq1nB+pUGeWlATaLiOT+wm5uCYjYH8MiTMfDLu4g==");


            IngressController tc = new IngressController(keystore, influxLib);
            ActionResult webResponse = tc.PostInfluxTelemetry(new InfluxTelemetry
            {
                NodeId = null,
                Signature = null,
                Payload = null
            });

            //Assert.IsType<BadRequestResult>(webResponse);
            Assert.NotNull(webResponse);
            var result = Assert.IsType<BadRequestResult>(webResponse);
            Assert.Equal((int)HttpStatusCode.BadRequest, result.StatusCode);
            Assert.Equal(0, influxLib.LastInsertCount);
            /* JObject pobj = JObject.Parse(InfluxCon(conobj));
            var rows = pobj.SelectTokens("['results'][0].['series'][0].['values']");
            var in_count = rows.Children().Count();
            //Assert.Equal(2, in_count);//Check if row count have not increased after first test case
            */
        }

        [Fact]
        public void EmptyTelemetryShouldNotRecord()
        {
            var conobj = LineProtocolConfiguration.InitConfiguration();
            var influxLib = new InfluxClient(conobj);
            var keystore = new MockKeystore();
            keystore.AddKey("node-1", "BgIAAACkAABSU0ExAAQAAAEAAQBdUkRrF0SA3a+QtGv6y97DFa79Z/IDHtCHehoj/LADUJxXsI1k6GBqdyE7MkF9uX2j8FbAMlxpmIKrMcRTWj9wZ5gIhbntiCF61IFsQJ5af23WsTg82u9A7mepxSXrfgfu6Bzq1nB+pUGeWlATaLiOT+wm5uCYjYH8MiTMfDLu4g==");


            IngressController tc = new IngressController(keystore, influxLib);
            ActionResult webResponse = tc.PostInfluxTelemetry(new InfluxTelemetry
            {
                NodeId = "",
                Signature = "",
                Payload = new List<string>()
            });

            //Assert.IsType<BadRequestResult>(webResponse);
            Assert.NotNull(webResponse);
            var result = Assert.IsType<BadRequestResult>(webResponse);
            Assert.Equal((int)HttpStatusCode.BadRequest, result.StatusCode);
            Assert.Equal(0, influxLib.LastInsertCount);
            /* JObject pobj = JObject.Parse(InfluxCon(conobj));
            var rows = pobj.SelectTokens("['results'][0].['series'][0].['values']");
            var in_count = rows.Children().Count();
            //Assert.Equal(2, in_count);//Check if row count have not increased after first test case
            */
        }

    }
}