using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using webapi;
using Xunit;

namespace tests
{
    public class SignatureVerifyExceptionTests
    {

        [Fact]
        public void SignatureVerifyExceptionConstructorShouldPass()
        {
            SignatureVerifyException ex = new SignatureVerifyException();
            Assert.NotNull(ex);

        }

        [Fact]
        public void SignatureVerifyExceptionParamConstructorShouldPass()
        {
            SignatureVerifyException ex = new SignatureVerifyException("Exception message");
            Assert.NotNull(ex);
            Assert.True(ex.ToString().Contains("Exception message"));

        }

        [Fact]
        public void SignatureVerifyExceptionSecondParamConstructorShouldPass()
        {
            SignatureVerifyException ex = new SignatureVerifyException("Message", new Exception("Inner Exception"));
            Assert.NotNull(ex);

        }

        [Fact]
        public void SignatureVerifyExceptionSerializationDeserializationShouldPass()
        {

            var innerEx = new Exception("inner message");
            var originalException = new SignatureVerifyException("file exc message", innerEx);

            var buffer = new byte[4096];
            var memoryStream = new MemoryStream(buffer);
            var memoryStream2 = new MemoryStream(buffer);
            var formatterObj = new BinaryFormatter();

            // Act
            formatterObj.Serialize(memoryStream, originalException);
            var deserializedException = (SignatureVerifyException)formatterObj.Deserialize(memoryStream2);

            Assert.Equal(originalException.InnerException.Message, deserializedException.InnerException.Message);
            Assert.Equal(originalException.Message, deserializedException.Message);
        }

    }
}