using System;
using webapi.Controllers;
using Xunit;

namespace tests
{
    public class InfluxPointVerificationTests
    {
        [Theory]
        [InlineData("parity,nodeid=0x00000000000000000000000,client=Parityv123 blocknum=123456,numpeers=12,blockts=1465839830100400200,numtxinblock=23,propagationtime=0 1465839830100400200")]
        [InlineData(@"parity\ num2,node\ id=0x00000000000000000000000,client=Parityv123 blocknum=123456,numpeers=12,blockts=1465839830100400200,numtxinblock=23,propagationtime=0 1465839830100400200")]
        [InlineData(@"parity\ num2,node\ id=0x00000000000000000000000,client\ name=Parityv123 block\ num=123456,numpeers=12,block\ ts=1465839830100400200,numtxinblock=23,propagationtime=0 1465839830100400200")]
        [InlineData("paritynum2,nodeid=0x00000000000000000000000,clientname=\"Parity\'v123\" blocknum=123456,numpeers=12,blockts=1465839830100400200,numtxinblock=23,propagationtime=0 1465839830100400200")]
        [InlineData(@"paritynum2,nodeid=0x00000000000000000000000,clientname=Parity\=v123 blocknum=123456,numpeers=12,blockts=1465839830100400200,region=side\=inn\ top\=bottom,numtxinblock=23,propagationtime=0 1465839830100400200")]
        [InlineData("parity,nodeid=0x00000000000000000000000,client=\"Parity 123\" blocknum=123456,numpeers=12,blockts=1465839830100400200,numtxinblock=23,propagationtime=0 1465839830100400200")]
        [InlineData("parity,nodeid=0x00000000000000000000000,client=\"Parity 123\" blocknum=123456,address=\"city=abc,country=def\",numpeers=12,blockts=1465839830100400200,numtxinblock=23,propagationtime=0 1465839830100400200")]
        public void ShouldPassValidPoint(string point)
        {
            bool result = InfluxPointVerifier.VerifyPoint(point);
            Assert.Equal(true, result);
        }


        [Fact]
        public void ShouldNotPassInvalidMeasurement()
        {
            string point = ",nodeid=0x00000000000000000000000,client=Parityv123 blocknum=123456,numpeers=12,blockts=1465839830100400200,numtxinblock=23,propagationtime=0 1465839830100400200";

            //Assert.Throws<Exception>(() => InfluxPointVerifier.verifyPoint(point));
            var exception = Assert.Throws<Exception>(() => InfluxPointVerifier.VerifyPoint(point));
            Assert.NotNull(exception);
            Assert.Contains("Measurement name should be valid string", exception.Message.ToString());

        }

        [Fact]
        public void ShouldNotPassInvalidTag()
        {
            string point = "parity,nodeid=0x00000000000000000000000,clientParityv123 blocknum=123456,numpeers=12,blockts=1465839830100400200,numtxinblock=23,propagationtime=0 1465839830100400200";

            //Assert.Throws<Exception>(() => InfluxPointVerifier.verifyPoint(point));
            var exception = Assert.Throws<Exception>(() => InfluxPointVerifier.VerifyPoint(point));
            Assert.NotNull(exception);
            Assert.Contains("Invalid Tag (key or value)", exception.Message);

        }

        [Fact]
        public void ShouldNotPassInvalidField()
        {
            string point = "parity,nodeid=0x00000000000000000000000,client=Parityv123 blocknum=123456numpeers=12,blockts=1465839830100400200,numtxinblock=23,propagationtime=0 1465839830100400200";

            //Assert.Throws<Exception>(() => InfluxPointVerifier.verifyPoint(point));
            var exception = Assert.Throws<Exception>(() => InfluxPointVerifier.VerifyPoint(point));
            Assert.NotNull(exception);

            Assert.Contains("Invalid Field (key or value)", exception.Message);

        }

        [Fact]
        public void ShouldNotPassInvalidTimeStamp()
        {
            string point = "parity,nodeid=0x00000000000000000000000,client=Parityv123 blocknum=123456,numpeers=12,blockts=1465839830100400200,numtxinblock=23,propagationtime=0 1465839ert830100400200";

            //Assert.Throws<Exception>(() => InfluxPointVerifier.verifyPoint(point));
            var exception = Assert.Throws<Exception>(() => InfluxPointVerifier.VerifyPoint(point));
            Assert.NotNull(exception);
            Assert.Contains("Invalid timestamp", exception.Message);

        }

        [Fact]
        public void ShouldPassOptionalTagSet()
        {
            string point = "parity blocknum=123456,numpeers=12,blockts=1465839830100400200,numtxinblock=23,propagationtime=0 1465839830100400200";

            bool result = InfluxPointVerifier.VerifyPoint(point);
            Assert.Equal(true, result);

        }

        [Fact]
        public void ShouldPassOptionalTimeStamp()
        {
            string point = "parity,nodeid=0x000000000000000000 blocknum=123456,numpeers=12,blockts=1465839830100400200,numtxinblock=23,propagationtime=0";

            bool result = InfluxPointVerifier.VerifyPoint(point);
            Assert.Equal(true, result);

        }

        [Fact]
        public void ShouldPassMeasurementAndFieldSetOnly()
        {
            string point = "parity blocknum=123456,numpeers=12,blockts=1465839830100400200,numtxinblock=23,propagationtime=0";

            bool result = InfluxPointVerifier.VerifyPoint(point);
            Assert.Equal(true, result);

        }

        [Fact]
        public void ShouldNotAllowEmptyInput()
        {
            string point = "";

            var exception = Assert.Throws<Exception>(() => InfluxPointVerifier.VerifyPoint(point));
            Assert.NotNull(exception);
            Assert.Contains("Invalid Point there must be at least measurement and fieldset seperated by a space, or there must not be tokens other then (measurementtagset fieldset timestamp)", exception.Message);

        }

    }
}