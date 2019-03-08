using System;
using System.Runtime.Serialization;
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
            FileEmptyException ex = new FileEmptyException("Message", new Exception("inner exception"));
            Assert.NotNull(ex);

        }

    }

}