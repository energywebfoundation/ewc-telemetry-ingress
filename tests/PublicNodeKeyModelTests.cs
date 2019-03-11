using webapi;
using Xunit;

namespace tests
{
    public class PublicNodeKeyModelTests
    {
        [Fact]
        public void PublicNodeKeyModelTestShouldPass()
        {
            string nodeId = "0x00000000012";
            string pubKey = "SADDFAJIONJKASD234ASDASNK34234=";
            
            PublicNodeKey obj = new PublicNodeKey { NodeId = nodeId, PublicKey = pubKey };

            Assert.NotNull(obj);
            Assert.NotNull(obj.NodeId == nodeId);
            Assert.NotNull(obj.PublicKey == pubKey);

        }
    }
}