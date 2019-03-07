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
 public class InfluxIngressTests
    {
        public static LineProtocolConnectionParameters InitConfiguration()
        {
            bool fromFile = false;
            LineProtocolConnectionParameters confFileobj = null;

            if (fromFile)
            {
                ConfigurationBuilder cb = new ConfigurationBuilder();
                cb.SetBasePath(System.AppContext.BaseDirectory);
                cb.AddJsonFile("appsettings.test.json");
                IConfigurationRoot cr = cb.Build();
                confFileobj = cr.GetSection("Influx").Get<LineProtocolConnectionParameters>();

            }else{

                confFileobj = new LineProtocolConnectionParameters() { 
                    Address = new Uri("http://influxdb:8086"), 
                    DBName = "telemetry", 
                    User = "root", 
                    Password = "root", 
                    FlushBufferItemsSize = 4, 
                    FlushBufferSeconds = 2, 
                    FlushSecondBufferItemsSize = 1000, 
                    FlushSecondBufferSeconds = 30, 
                    UseGzipCompression = true };
            }

            return confFileobj;
        }

        public static string InfluxCon(LineProtocolConnectionParameters conobj)
        {
            var client = new WebClient();
            var queryString = Uri.EscapeUriString("db=" + Uri.EscapeDataString(conobj.DBName) + "&q=SELECT * FROM \"weather\"");
            var queryUrl = conobj.Address + "query?" + queryString;
            return client.DownloadString(queryUrl);
        }

        [Fact]
        public void EnqueueShouldFailForInvalidPoint()
        {

            var conobj = InitConfiguration();
            var influxLib = new InfluxClient(conobj);
            var keystore = new MockKeystore();

            keystore.AddKey("node-3", "BgIAAACkAABSU0ExAAQAAAEAAQBXZXt7QOileknWzBH2Sg+Yk4INDTbKA5XUUfUe23zUmr6eM1USCNHX3lidZfjk5Emuui1m8k0KnghxcJfOau8iPRpLg/lubMNojpLGe2MXn5GsyjgEpVdE+Cf0pLBAYHcBuBYHj99muMsJrJW1/InbKFa24JuVnBr+MybPuMXqtQ==");

            IngressController tc = new IngressController(keystore, influxLib);
            ActionResult webResponse = tc.PostInfluxTelemetry(new InfluxTelemetry
            {
                NodeId = "node-3",
                Signature = "fHYfz0RKGI9xpGBX0v4nbBBoZvyryZbrRTp08wPFcMw9y5VUS6lrNrKaXNfaW0V3vb88iD4as/cBbvNKMDxrq0Hq1wcx9ceIkH3abuCCCBJWwHaLCIqJ1ZBgVcLP8VDjpxzvJ9m6fiTTYjQleGTGzaoGziH/buvW9NOIPHzqrNg=",
                Payload = new List<string>
                {
                    "invalid pointw"
                }
            });
            
            Assert.NotNull(webResponse);
            var result = Assert.IsType<StatusCodeResult>(webResponse);
            Assert.Equal((int)HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Fact]
        public void InvalidPublicKeyOfNodeShouldFail()
        {

            var conobj = InitConfiguration();
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
        public void ValidMetricsShouldRecordAsync()
        {

            var conobj = InitConfiguration();
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
           
            System.Threading.Thread.Sleep(conobj.FlushBufferSeconds * 1000 * 2); //wait for Queue to flush
            Assert.Equal(2, influxLib.LastInsertCount);

            /*JObject pobj = JObject.Parse(InfluxCon(conobj));
            var rows = pobj.SelectTokens("['results'][0].['series'][0].['values']");
            var in_count = rows.Children().Count();
            Assert.Equal(2, in_count);//Check if row count is now 2
            */
        }
 
        [Fact]
        public void InvalidSignatureShouldNotRecordAsync()
        {

            var conobj = InitConfiguration();
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
            var conobj = InitConfiguration();
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
            var conobj = InitConfiguration();
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