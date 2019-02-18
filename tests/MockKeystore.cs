using System;
using System.Collections.Generic;
using webapi;

namespace tests
{
    public class MockKeystore : IPublickeySource
    {
        private readonly Dictionary<string, string> _keys;

        public MockKeystore()
        {
            _keys = new Dictionary<string, string>();
        }
                
        public string GetKeyForNode(string nodeId)
        {
            if (!_keys.ContainsKey(nodeId))
            {
                throw new KeyNotFoundException();
            }
            return _keys[nodeId];
        }

        public void AddKey(string node, string pubKey)
        {
            _keys.Add(node,pubKey);
        }
    }
}
