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
    }
}