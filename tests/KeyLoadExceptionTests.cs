using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using webapi;
using Xunit;

namespace tests
{
    public class KeyLoadExceptionTests
    {
        [Fact]
        public void KeyLoadExceptionConstructorShouldPass()
        {
            KeyLoadException ex = new KeyLoadException();
            Assert.NotNull(ex);

        }

        [Fact]
        public void KeyLoadExceptionSerializationDeserializationShouldPass()
        {

            var innerEx = new Exception("inner message");
            var originalException = new KeyLoadException("file exc message", innerEx);

            var buffer = new byte[4096];
            var memoryStream = new MemoryStream(buffer);
            var memoryStream2 = new MemoryStream(buffer);
            var formatterObj = new BinaryFormatter();

            // Act
            formatterObj.Serialize(memoryStream, originalException);
            var deserializedException = (KeyLoadException)formatterObj.Deserialize(memoryStream2);

            Assert.Equal(originalException.InnerException.Message, deserializedException.InnerException.Message);
            Assert.Equal(originalException.Message, deserializedException.Message);
        }
    }
}