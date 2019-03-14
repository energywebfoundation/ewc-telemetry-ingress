using System;
using webapi.Controllers;
using Xunit;

namespace tests
{
    public class InfluxPointVerificationTests
    {
        [Fact]
        public void ShouldPassValidPoint(){
            string point= "parity,nodeid=0x00000000000000000000000,client=Parityv123 blocknum=123456,numpeers=12,blockts=1465839830100400200,numtxinblock=23,propagationtime=0 1465839830100400200";
            bool result = InfluxPointVerifier.verifyPoint(point);
            Assert.Equal(result,true);
        }

        [Fact]
        public void ShouldNotPassInvalidMeasurement(){
            string point= ",nodeid=0x00000000000000000000000,client=Parityv123 blocknum=123456,numpeers=12,blockts=1465839830100400200,numtxinblock=23,propagationtime=0 1465839830100400200";

            //Assert.Throws<Exception>(() => InfluxPointVerifier.verifyPoint(point));
            var exception = Assert.Throws<Exception>(() => InfluxPointVerifier.verifyPoint(point));
            Assert.NotNull(exception);
            Assert.Equal("Measurement name should be valid string", exception.Message);

        }

        [Fact]
        public void ShouldNotPassInvalidTag(){
            string point= "parity,nodeid=0x00000000000000000000000,clientParityv123 blocknum=123456,numpeers=12,blockts=1465839830100400200,numtxinblock=23,propagationtime=0 1465839830100400200";

            //Assert.Throws<Exception>(() => InfluxPointVerifier.verifyPoint(point));
            var exception = Assert.Throws<Exception>(() => InfluxPointVerifier.verifyPoint(point));
            Assert.NotNull(exception);
            Assert.True(exception.Message.Contains("Invalid Tag (key or value)"));

        }

        [Fact]
        public void ShouldNotPassInvalidField(){
            string point= "parity,nodeid=0x00000000000000000000000,client=Parityv123 blocknum=123456numpeers=12,blockts=1465839830100400200,numtxinblock=23,propagationtime=0 1465839830100400200";

            //Assert.Throws<Exception>(() => InfluxPointVerifier.verifyPoint(point));
            var exception = Assert.Throws<Exception>(() => InfluxPointVerifier.verifyPoint(point));
            Assert.NotNull(exception);

            Assert.True(exception.Message.Contains("Invalid Field (key or value)"));

        }

        [Fact]
        public void ShouldNotPassInvalidTimeStamp(){
            string point= "parity,nodeid=0x00000000000000000000000,client=Parityv123 blocknum=123456,numpeers=12,blockts=1465839830100400200,numtxinblock=23,propagationtime=0 1465839ert830100400200";

            //Assert.Throws<Exception>(() => InfluxPointVerifier.verifyPoint(point));
            var exception = Assert.Throws<Exception>(() => InfluxPointVerifier.verifyPoint(point));
            Assert.NotNull(exception);
            Assert.True(exception.Message.Contains("Invalid timestamp"));

        }

        [Fact]
        public void ShouldPassOptionalTagSet(){
            string point= "parity blocknum=123456,numpeers=12,blockts=1465839830100400200,numtxinblock=23,propagationtime=0 1465839830100400200";

            bool result = InfluxPointVerifier.verifyPoint(point);
            Assert.Equal(result,true);

        }

        [Fact]
        public void ShouldPassOptionalTimeStamp(){
            string point= "parity,nodeid=0x000000000000000000 blocknum=123456,numpeers=12,blockts=1465839830100400200,numtxinblock=23,propagationtime=0";

            bool result = InfluxPointVerifier.verifyPoint(point);
            Assert.Equal(result,true);

        }

        [Fact]
        public void ShouldPassMeasurementAndFieldSetOnly(){
            string point= "parity blocknum=123456,numpeers=12,blockts=1465839830100400200,numtxinblock=23,propagationtime=0";

            bool result = InfluxPointVerifier.verifyPoint(point);
            Assert.Equal(result,true);

        }

        [Fact]
        public void ShouldNotAllowEmptyInput(){
            string point= "";

            var exception = Assert.Throws<Exception>(() => InfluxPointVerifier.verifyPoint(point));
            Assert.NotNull(exception);
            Assert.True(exception.Message.Contains("Invalid Point there must be at least measurement and fieldset seperated by a space, or there must not be tokens other then (measurementtagset fieldset timestamp)"));

        }
    }
}