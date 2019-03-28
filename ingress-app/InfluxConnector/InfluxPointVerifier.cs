using System;
using System.Text.RegularExpressions;

namespace webapi.Controllers
{
    /// <summary>
    /// The Class for Influx data points verification.
    /// </summary>
    public static class InfluxPointVerifier
    {

        /// <summary>
        /// this function verifies Influx Point by checking Influx data point syntax and data types.
        /// Valid Point format is |measurement|,tag_set| |field_set| |timestamp|
        /// </summary>
        /// <param name="point">The Influx Point to be checked.</param>
        /// <returns>returns true if provided point is a valid Influx data point and false if it is invalid</returns>
        /// <exception cref="System.Exception">Thrown when provided point have invalid measurement or tag or field or timestamp.</exception>
        public static bool VerifyPoint(string point)
        {
            string[] tokens = SplitString(point,' ');

            // invalid Point there must be at least measurement and fieldset seperated by a space, or there must not be tokens other then (measurementtagset fieldset timestamp)
            if (tokens.Length < 2 || tokens.Length > 3)
            {
                throw new Exception("Invalid Point there must be at least measurement and fieldset seperated by a space, or there must not be tokens other then (measurementtagset fieldset timestamp) Point:" + point);
            }

            // getting measurement and Tags Set
            string[] measurementAndTagSet = SplitString(tokens[0], ',');

            //check for unescaped special chars
            //measurement name should be valid string
            if (string.IsNullOrWhiteSpace(measurementAndTagSet[0]))
            {
                throw new Exception("Measurement name should be valid string  Point:" + point);
            }

            //tag set validation, tag sets are optional
            if (measurementAndTagSet.Length > 1) //means point have tag set, so lets validate each tag set, tag set are optional
            {
                for (int i = 1; i < measurementAndTagSet.Length; i++)
                {

                    string[] tagKeyValue = SplitString(measurementAndTagSet[i], '=');

                    //verifying tags individually
                    if (tagKeyValue.Length != 2 ||
                    string.IsNullOrWhiteSpace(tagKeyValue[0]) || string.IsNullOrWhiteSpace(tagKeyValue[1]))
                    { //check for invalid tag key or value
                        throw new Exception("Invalid Tag (key or value) " + measurementAndTagSet[i] + "in Point:" + point);
                    }
                }
            }

            //Field set validation, at least 1 field set is mandatory
            string[] fieldSet = SplitString(tokens[1], ',');
            foreach (string field in fieldSet)
            {

                string[] fieldKeyValue = SplitString(field, '=');

                //Validation of fields individually
                if (fieldKeyValue.Length != 2 ||
                    string.IsNullOrWhiteSpace(fieldKeyValue[0]) || string.IsNullOrWhiteSpace(fieldKeyValue[1]))
                { //check for invalid tag
                    throw new Exception("Invalid Field (key or value) " + field + " in Point:" + point);
                }
            }

            //Time stamp validation, time stamp is optional
            if (tokens.Length == 3)
            {
                //Check for invalid timestamp
                bool result = long.TryParse(tokens[2], out _);
                if (!result)
                {
                    throw new Exception("Invalid timestamp " + tokens[2] + " in Point:" + point);
                }

            }

            return true;
        }

        /// <summary>
        /// Method for spliting string based on provided char while ignoring if it is escaped or if it is in sub string with ""
        /// </summary>
        /// <param name="data">String to be split.</param>
        /// <param name="splitChar">The spliting char.</param>
        /// <returns>returns true if provided point is a valid Influx data point and false if it is invalid</returns>
        /// <exception cref="System.Exception">Thrown when provided point have invalid measurement or tag or field or timestamp.</exception>
        private static string[] SplitString(string data, char splitChar)
        {
            string tmpData = data;
            string strRegex = @"[" + splitChar + @"](?=(?:[^""]*""[^""]*"")*[^""]*$)";
            Regex myRegex = new Regex(strRegex, RegexOptions.Multiline);

            string[] res = myRegex.Split(data);
            string randomStr = "QWE345AC!@#$XSQWE345AC";

            foreach (string s in res)
            {
                if (s.Contains(splitChar))
                {
                    string ns = s.Replace("" + splitChar, randomStr);
                    tmpData = tmpData.Replace(s, ns);
                }
            }
            string[] results = Regex.Split(tmpData, @"(?<!($|[^\\])(\\\\)*?\\)" + splitChar);

            for (int i = 0; i < results.Length; i++)
            {
                if (results[i].Contains(randomStr))
                {
                    results[i] = results[i].Replace(randomStr, "" + splitChar);
                }
                
            }
            return results;
        }
    }
}