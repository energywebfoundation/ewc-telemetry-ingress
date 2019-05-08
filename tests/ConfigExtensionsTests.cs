using System;
using FluentAssertions;
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
            usrVal.Should().Be(usr);

            string passVal = configObj.GetValue<string>("Influx:Password");
            passVal.Should().Be(pass);

            string dbVal = configObj.GetValue<string>("Influx:DBName");
            dbVal.Should().Be(db);

            string hostVal = configObj.GetValue<string>("Influx:Address");
            hostVal.Should().Be(host);

        }

    }
}