using System;
using webapi;

namespace tests
{
    public class MockKeystore : IPublickeySource
    {
        
        public string NodeKey { get; set; }
        
        public string GetKeyForNode(string nodeId)
        {
            return NodeKey;
        }
    }
}
