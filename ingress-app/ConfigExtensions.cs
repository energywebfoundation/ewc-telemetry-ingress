using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace webapi
{
    /// <summary>
    /// The class for Configuration Extensions
    /// </summary>
    public static class ConfigExtensions
    {

        /// <summary>
        /// Adds provided TELEMETRY configuration params from Environment variables into Dictionary and adds that to Configuration Builder Object
        /// </summary>
        /// <param name="configBuilder">Expects ConfigurationBuilder as input</param>
        /// <returns>ConfigurationBuilder reference</returns>
        public static IConfigurationBuilder AddInfluxConfigFromEnvironment(this IConfigurationBuilder configBuilder)
        {
            //Setting ENV variables into dictionary with following keys
            Dictionary<string, string> dict = new Dictionary<string, string>();
            string user = Environment.GetEnvironmentVariable("TELEMETRY_INFLUXDB_USER"); if (!string.IsNullOrEmpty(user))
            {
                dict.Add("Influx:User", user);
            }

            string pw = Environment.GetEnvironmentVariable("TELEMETRY_INFLUXDB_USER_PASSWORD");
            if (!string.IsNullOrEmpty(pw))
            {
                dict.Add("Influx:Password", pw);
            }

            string db = Environment.GetEnvironmentVariable("TELEMETRY_INFLUXDB_DB");
            if (!string.IsNullOrEmpty(db))
            {
                dict.Add("Influx:DBName", db);
            }

            string host = Environment.GetEnvironmentVariable("TELEMETRY_INFLUXDB_HOST");
            if (!string.IsNullOrEmpty(host))
            {
                dict.Add("Influx:Address", host);
            }

            configBuilder.AddInMemoryCollection(dict);
            return configBuilder;
        }
    }
}