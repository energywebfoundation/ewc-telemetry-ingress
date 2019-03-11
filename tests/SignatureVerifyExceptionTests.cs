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
            string msg1 = "Exception message";
            SignatureVerifyException ex = new SignatureVerifyException(msg1);
            Assert.NotNull(ex);
            Assert.Equal(ex.Message,msg1);

        }

        [Fact]
        public void SignatureVerifyExceptionSecondParamConstructorShouldPass()
        {
            string msg1 = "outer message 1";
            string msg2 = "innter message 2";
            SignatureVerifyException ex = new SignatureVerifyException(msg1, new Exception(msg2));
            Assert.NotNull(ex);
            Assert.Equal(ex.Message, msg1);
            Assert.Equal(ex.InnerException.Message, msg2);

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