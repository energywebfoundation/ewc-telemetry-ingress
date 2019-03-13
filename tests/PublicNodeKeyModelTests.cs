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
            
            JsonPublicNodeKey obj = new JsonPublicNodeKey { NodeId = nodeId, PublicKey = pubKey };

            Assert.NotNull(obj);
            Assert.NotNull(obj.NodeId == nodeId);
            Assert.NotNull(obj.PublicKey == pubKey);

        }
    }
}