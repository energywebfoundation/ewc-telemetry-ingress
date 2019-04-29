using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using Newtonsoft.Json.Linq;
using webapi;
using webapi.Controllers;
using Xunit;

namespace tests
{
    public class InfluxClientTests
    {
        public static string InfluxCon(LineProtocolConnectionParameters conobj, string query)
        {
            var client = new WebClient();
            var queryString = Uri.EscapeUriString("db=" + Uri.EscapeDataString(conobj.DBName) + "&q=" + query);//SELECT * FROM \"weather\"");
            var queryUrl = conobj.Address + "query?" + queryString;
            return client.DownloadString(queryUrl);
        }


        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void FirstBufferShouldFlushOnItemSize(bool useListEnqueue)
        {
            var conobj = LineProtocolConfiguration.InitConfiguration();
            conobj.FlushBufferItemsSize = 2;
            conobj.FlushBufferSeconds = 30000;
            InfluxCon(conobj, "DELETE FROM \"datameasurement\"");

            List<string> data = new List<string>();
            data.Add("datameasurement,location=us-midwest temperature=82 1465839830100400200");
            data.Add("datameasurement,location=us-east temperature=75 1465839830100400200");

            var influxLib = new InfluxClient(conobj);
            if (useListEnqueue)
            {
                influxLib.Enqueue(data, true);
            }
            else
            {
                foreach (string point in data)
                {
                    influxLib.Enqueue(point, true);
                }

            }

            Thread.Sleep(1000); //wait for NW latency to Influx


            JObject pobj = JObject.Parse(InfluxCon(conobj, "SELECT * FROM \"datameasurement\""));
            var rows = pobj.SelectTokens("['results'][0].['series'][0].['values']");
            var inCount = rows.Children().Count();
            Assert.Equal(2, inCount);//Check if row count is now 2

        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void FirstBufferShouldNotFlushBeforeItemSize(bool useListEnqueue)
        {
            var conobj = LineProtocolConfiguration.InitConfiguration();
            conobj.FlushBufferItemsSize = 2;
            conobj.FlushBufferSeconds = 300;
            InfluxCon(conobj, "DELETE FROM \"datameasurementA\"");

            List<string> data = new List<string>();
            data.Add("datameasurementA,location=us-midwest temperature=82 1465839830100400200");

            var influxLib = new InfluxClient(conobj);
            if (useListEnqueue)
            {
                influxLib.Enqueue(data, true);
            }
            else
            {
                influxLib.Enqueue(data[0], true);
            }


            JObject pobj = JObject.Parse(InfluxCon(conobj, "SELECT * FROM \"datameasurementA\""));
            var rows = pobj.SelectTokens("['results'][0].['series'][0].['values']");
            var inCount = rows.Children().Count();
            Assert.Equal(0, inCount);//Check if row count is now 0

        }

        [Fact]
        public void FirstBufferShouldFlushAfterTimeInterval()
        {
            var conobj = LineProtocolConfiguration.InitConfiguration();
            conobj.FlushBufferItemsSize = 100;
            conobj.FlushBufferSeconds = 1;
            InfluxCon(conobj, "DELETE FROM \"datameasurementB\"");

            List<string> data = new List<string>();
            data.Add("datameasurementB,location=us-midwest temperature=82 1465839830100400200");

            var influxLib = new InfluxClient(conobj);
            influxLib.Enqueue(data, true);
            Thread.Sleep(2000); //wait for TIME flush & NW latency to Influx


            JObject pobj = JObject.Parse(InfluxCon(conobj, "SELECT * FROM \"datameasurementB\""));
            var rows = pobj.SelectTokens("['results'][0].['series'][0].['values']");
            var inCount = rows.Children().Count();
            Assert.Equal(1, inCount);//Check if row count is now 1

        }

        [Fact]
        public void FirstBufferShouldNotFlushBeforeTimeInterval()
        {
            var conobj = LineProtocolConfiguration.InitConfiguration();
            conobj.FlushBufferItemsSize = 2000;
            conobj.FlushBufferSeconds = 300;
            InfluxCon(conobj, "DELETE FROM \"datameasurementC\"");

            List<string> data = new List<string>();
            data.Add("datameasurementC,location=us-midwest temperature=82 1465839830100400200");

            var influxLib = new InfluxClient(conobj);
            influxLib.Enqueue(data, true);

            JObject pobj = JObject.Parse(InfluxCon(conobj, "SELECT * FROM \"datameasurementC\""));
            var rows = pobj.SelectTokens("['results'][0].['series'][0].['values']");
            var inCount = rows.Children().Count();
            Assert.Equal(0, inCount);//Check if row count is now 0

        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void SecondBufferFlushTimeInterval(bool notFlushBeforeTimeInterval)  //first buffer failure test
        {
            var conobjBufferA = LineProtocolConfiguration.InitConfiguration();
            conobjBufferA.FlushBufferItemsSize = 2;
            conobjBufferA.FlushBufferSeconds = 300;
            conobjBufferA.FlushSecondBufferSeconds = 5;
            conobjBufferA.FlushSecondBufferItemsSize = 5;
            InfluxCon(conobjBufferA, "DELETE FROM \"datameasurementD\"");
            InfluxCon(conobjBufferA, "DROP DATABASE \"" + conobjBufferA.DBName + "\""); // dropping db so first buffer fail

            List<string> data = new List<string>();
            data.Add("datameasurementD,location=us-midwest temperature=82 1465839830100400200");
            data.Add("datameasurementD,location=us-east temperature=75 1465839830100400200");

            var influxLib = new InfluxClient(conobjBufferA);
            influxLib.Enqueue(data, true);
            Thread.Sleep(2000);
            InfluxCon(conobjBufferA, "CREATE DATABASE \"" + conobjBufferA.DBName + "\"");

            if (!notFlushBeforeTimeInterval)
            {
                Thread.Sleep((conobjBufferA.FlushSecondBufferSeconds + 1) * 1000); //wait for 2nd buffer to flush
            }

            JObject pobj = JObject.Parse(InfluxCon(conobjBufferA, "SELECT * FROM \"datameasurementD\""));
            var rows = pobj.SelectTokens("['results'][0].['series'][0].['values']");
            var inCount = rows.Children().Count();
            Assert.Equal((notFlushBeforeTimeInterval ? 0 : 2), inCount);//Check if row count is now 2

        }

        [Fact]
        public void SecondBufferShouldFlushOnItemSize()  //first buffer failure test
        {
            var conobjBufferA = LineProtocolConfiguration.InitConfiguration();
            conobjBufferA.FlushBufferItemsSize = 2;
            conobjBufferA.FlushBufferSeconds = 300;
            conobjBufferA.FlushSecondBufferSeconds = 50;
            conobjBufferA.FlushSecondBufferItemsSize = 3;
            InfluxCon(conobjBufferA, "DELETE FROM \"datameasurementE\"");
            InfluxCon(conobjBufferA, "DROP DATABASE \"" + conobjBufferA.DBName + "\"");  // dropping db so first buffer fail

            List<string> data = new List<string>();
            data.Add("datameasurementE,location=us-midwest temperature=32 1465839830100400200");
            data.Add("datameasurementE,location=us-east temperature=25 1465839830100400201");

            var influxLib = new InfluxClient(conobjBufferA);
            influxLib.Enqueue(data, true);
            Thread.Sleep(1000); //wait before flush call and db creation so flush dnt pass to influx but to 2nd buffer
            InfluxCon(conobjBufferA, "CREATE DATABASE \"" + conobjBufferA.DBName + "\"");
            Thread.Sleep(2000); //wait for NW latency to Influx

            //directly enqueue item in 2nd buffer to trigger flush
            influxLib.Enqueue("datameasurementE,location=us-east temperature=15 1465839830100400203", false);


            Thread.Sleep(3000); //wait for 2nd buffer to flush

            JObject pobj = JObject.Parse(InfluxCon(conobjBufferA, "SELECT * FROM \"datameasurementE\""));
            var rows = pobj.SelectTokens("['results'][0].['series'][0].['values']");
            var inCount = rows.Children().Count();
            Assert.Equal(3, inCount);//Check if row count is now 4
        }

        [Fact]
        public void SecondBufferShouldNotFlushBeforeItemSize()  //first buffer failure test
        {
            var conobjBufferA = LineProtocolConfiguration.InitConfiguration();
            conobjBufferA.FlushBufferItemsSize = 1;
            conobjBufferA.FlushBufferSeconds = 300;
            conobjBufferA.FlushSecondBufferSeconds = 50;
            conobjBufferA.FlushSecondBufferItemsSize = 42;
            InfluxCon(conobjBufferA, "DELETE FROM \"datameasurementE\"");
            InfluxCon(conobjBufferA, "DROP DATABASE " + conobjBufferA.DBName);  // dropping db so first buffer fail

            List<string> data = new List<string>();
            data.Add("datameasurementE,location=us-midwest temperature=32 1465839830100400200");

            var influxLib = new InfluxClient(conobjBufferA);
            influxLib.Enqueue(data, true);
            Thread.Sleep(1000); //wait before flush call and db creation so flush dnt pass to influx but to 2nd buffer
            InfluxCon(conobjBufferA, "CREATE DATABASE \"" + conobjBufferA.DBName + "\"");
            Thread.Sleep(2000); //wait for NW latency to Influx

            influxLib.Enqueue("datameasurementE,location=us-east temperature=15 1465839830100400203", false);


            Thread.Sleep(3000); //wait for 2nd buffer to flush

            JObject pobj = JObject.Parse(InfluxCon(conobjBufferA, "SELECT * FROM \"datameasurementE\""));
            var rows = pobj.SelectTokens("['results'][0].['series'][0].['values']");
            var inCount = rows.Children().Count();
            Assert.Equal(0, inCount);//Check if row count is now 4
        }

        [Fact]
        public void InfluxClientDisposeTestShouldPass()  //first buffer failure test
        {
            //for coverage
            InfluxClient ic = new InfluxClient(LineProtocolConfiguration.InitConfiguration());
            ic.Dispose();

        }

    }
}