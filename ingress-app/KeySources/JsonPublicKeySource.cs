using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json;

namespace webapi
{
    public class JsonPublicKeySource : IPublickeySource
    {
        private List<JsonPublicNodeKey> _loadedKeys;
        private string _sourceFile;
        
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

            _sourceFile = path;
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

        public void SaveToFile()
        {
            if (string.IsNullOrWhiteSpace(_sourceFile))
            {
                throw new Exception("Not loaded from file.");
            }

            if (!File.Exists(_sourceFile))
            {
                throw new FileNotFoundException("Source file no longer exists.");
            }

            string json = JsonConvert.SerializeObject(_loadedKeys, Formatting.Indented);
            File.WriteAllText(_sourceFile,json);

        }
        
        public string GetKeyForNode(string nodeId)
        {
            string key = (_loadedKeys.Where(x=> x.NodeId == nodeId)).FirstOrDefault().PublicKey;
            if (key == null)
            {
                throw new KeyNotFoundException("Public key not available");
            }

            return key;
        }

        public void AddKey(string nodeId, string pubkeyAsBase64)
        {
            _loadedKeys.Add(new JsonPublicNodeKey
            {
                NodeId = nodeId,
                PublicKey = pubkeyAsBase64
            });
            SaveToFile();
        }

        public void RemoveKey(string nodeId)
        {
            var key = _loadedKeys.FirstOrDefault(x => x.NodeId == nodeId);
            if (key == null)
            {
                throw new KeyNotFoundException("Node key is not known");
            }

            _loadedKeys.Remove(key);
            SaveToFile();
        }

        public static IPublickeySource FromFile(string keyfileJson)
        {
            JsonPublicKeySource source = new JsonPublicKeySource();
            source.LoadFromFile(keyfileJson);
            return source;
        }
    }
}