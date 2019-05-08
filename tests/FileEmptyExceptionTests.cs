using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using webapi;
using Xunit;

namespace tests
{
    public class FileEmptyExceptionTests : FileEmptyException
    {
        [Fact]
        public void FileEmptyExceptionConstructorShouldPass()
        {
            FileEmptyException ex = new FileEmptyException();
            Assert.NotNull(ex);

        }

        [Fact]
        public void FileEmptyExceptionParamConstructorShouldPass()
        {
            string msg1 = "outer message 1";
            string msg2 = "innter message 2";
            FileEmptyException ex = new FileEmptyException(msg1, new Exception(msg2));
            Assert.NotNull(ex);
            Assert.Equal(ex.Message, msg1);
            Assert.Equal(ex.InnerException.Message, msg2);

        }

        [Fact]
        public void FileEmptyExceptionSerializationDeserializationShouldPass()
        {

            var innerEx = new Exception("inner message");
            var originalException = new FileEmptyException("file exc message", innerEx);
            
            var buffer = new byte[4096];
            var memoryStream = new MemoryStream(buffer);
            var memoryStream2 = new MemoryStream(buffer);
            var formatterObj = new BinaryFormatter();

            // Act
            formatterObj.Serialize(memoryStream, originalException);
            var deserializedException = (FileEmptyException)formatterObj.Deserialize(memoryStream2);

            Assert.Equal(originalException.InnerException.Message, deserializedException.InnerException.Message);
            Assert.Equal(originalException.Message, deserializedException.Message);
        }

    }

}