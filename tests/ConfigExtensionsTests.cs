using System;
using Microsoft.Extensions.Configuration;
using webapi;
using Xunit;

namespace tests
{
    public class ConfigExtensionsTests
    {
        [Fact]
        public void ConfigExtensionsSetShouldPass()
        {
            //Random selected data for testing env variables
            string usr = "usrname";
            string pass = "abc123";
            string db = "testdb";
            string host = "192.168.21.41";

            Environment.SetEnvironmentVariable("TELEMETRY_INFLUXDB_USER", usr);
            Environment.SetEnvironmentVariable("TELEMETRY_INFLUXDB_USER_PASSWORD", pass);
            Environment.SetEnvironmentVariable("TELEMETRY_INFLUXDB_DB", db);
            Environment.SetEnvironmentVariable("TELEMETRY_INFLUXDB_HOST", host);

            ConfigurationBuilder cb = new ConfigurationBuilder();
            IConfigurationRoot configObj = cb.AddInfluxConfigFromEnvironment().Build();

            string usrVal = configObj.GetValue<string>("Influx:User");
            Assert.True(usr.Equals(usrVal));

            string passVal = configObj.GetValue<string>("Influx:Password");
            Assert.True(pass.Equals(passVal));

            string dbVal = configObj.GetValue<string>("Influx:DBName");
            Assert.True(db.Equals(dbVal));

            string hostVal = configObj.GetValue<string>("Influx:Address");
            Assert.True(host.Equals(hostVal));


        }

    }
}