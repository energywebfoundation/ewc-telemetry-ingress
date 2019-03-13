using System;

namespace webapi.Controllers
{
    /// <summary>
    /// The Class for Influx data points verification.
    /// </summary>
    public class InfluxPointVerifier
    {

        /// <summary>
        /// this funciton verifies Influx Point by checking Influx data point syntax and data types.
        /// Valid Point format is |measurement|,tag_set| |field_set| |timestamp|
        /// </summary>
        /// <param name="point">The Influx Point to be checked.</param>
        /// <returns>returns true if provided point is a valid Influx data point and false if it is invalid</returns>
        /// <exception cref="System.Exception">Thrown when provided point have invalid measurement or tag or field or timestamp.</exception>
        public static bool verifyPoint(string point)
        {
            string[] tokens = point.Split(' ');

            // invalid Point there must be at least measurement and fieldset seperated by a space, or there must not be tokens other then (measurementtagset fieldset timestamp)
            if (tokens.Length < 2 || tokens.Length > 3)
            {
                throw new Exception("Invalid Point there must be at least measurement and fieldset seperated by a space, or there must not be tokens other then (measurementtagset fieldset timestamp) " + tokens.Length);
            }

            // getting measurement and Tags Set
            string[] measurementAndTagSet = tokens[0].Split(',');

            //measurement name should be valid string
            if (string.IsNullOrWhiteSpace(measurementAndTagSet[0]) || measurementAndTagSet[0].Contains('\'')
                || measurementAndTagSet[0].Contains('"'))
            {
                throw new Exception("Measurement name should be valid string");
            }

            //tag set validation, tag sets are optional
            if (measurementAndTagSet.Length > 1) //means point have tag set, so lets validate each tag set, tag set are optional
            {
                for (int i = 1; i < measurementAndTagSet.Length; i++)
                {

                    string[] tagKeyValue = (measurementAndTagSet[i]).Split('=');

                    //verifying tags individually
                    if (tagKeyValue.Length != 2 ||
                    string.IsNullOrWhiteSpace(tagKeyValue[0]) || string.IsNullOrWhiteSpace(tagKeyValue[1]) ||
                    tagKeyValue[0].Contains('\'') || tagKeyValue[0].Contains('"') ||
                    tagKeyValue[1].Contains('\'') || tagKeyValue[1].Contains('"'))
                    { //check for invalid tag key or value
                        throw new Exception("Invalid Tag (key or value) " + measurementAndTagSet[i]);
                    }
                }
            }

            //Field set validation, at least 1 field set is mandatory
            string[] fieldSet = tokens[1].Split(',');
            foreach (string field in fieldSet)
            {

                string[] fieldKeyValue = field.Split('=');

                //Validation of fields individually
                if (fieldKeyValue.Length != 2 ||
                    string.IsNullOrWhiteSpace(fieldKeyValue[0]) || string.IsNullOrWhiteSpace(fieldKeyValue[1]) ||
                    fieldKeyValue[0].Contains('\'') || fieldKeyValue[0].Contains('"') ||
                    fieldKeyValue[1].Contains('\'') || fieldKeyValue[1].Contains('"'))
                { //check for invalid tag
                    throw new Exception("Invalid Field (key or value) " + field);
                }
            }

            //Time stamp validation, time stamp is optional
            if (tokens.Length == 3)
            {
                long holder = 0;
                //Check for invalid timestamp
                bool result = System.Int64.TryParse(tokens[2], out holder);
                if (!result)
                {
                    throw new Exception("Invalid timestamp " + tokens[2]);
                }

            }

            return true;
        }
    }
}