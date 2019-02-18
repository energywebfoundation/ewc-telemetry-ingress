using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace webapi
{
    public class JsonPublicKeySource : IPublickeySource
    {
        private List<JsonPublicNodeKey> _loadedKeys;

        public void LoadFromFile(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentNullException(nameof(path),"path can't be null or empty");
            }

            if (!File.Exists(path))
            {
                throw new ArgumentException(nameof(path),"No file at path: " + path);
            }
            
            // Read the file from disk
            string fileContents = File.ReadAllText(path);
            if (string.IsNullOrWhiteSpace(fileContents))
            {
                throw new FileEmptyException($"File at path {path} is empty."); 
            }

            LoadFromJson(fileContents);
        }

        public void LoadFromJson(string json)
        {
            List<JsonPublicNodeKey> jsonKeys = null;
            try
            {
                jsonKeys = JsonConvert.DeserializeObject<List<JsonPublicNodeKey>>(json);
            }
            catch (Exception ex)
            {
                throw new KeyLoadException($"Unable to load keys from json",ex); 
            }

            if (jsonKeys.Count == 0)
            {
                throw new KeyLoadException($"JSON contains no keys");
            }

            _loadedKeys = jsonKeys;
        }

        public string GetKeyForNode(string nodeId)
        {
            string key = _loadedKeys.FirstOrDefault(x => x.NodeId == nodeId)?.PublicKey;
            if (key == null)
            {
                throw new KeyNotFoundException("Public key not available");
            }

            return key;
        }
    }
}