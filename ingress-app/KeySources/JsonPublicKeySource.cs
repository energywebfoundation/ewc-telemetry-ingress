using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json;

namespace webapi
{
    /// <summary>
    /// The class having Json Public Key source functions for adding new key, removing key, loading keys from json file, loading from JSON string, saving to file and getting key for node.
    /// </summary>
    public class JsonPublicKeySource : IPublickeySource
    {
        private List<JsonPublicNodeKey> _loadedKeys;
        private string _sourceFile;

        /// <summary>
        /// This function loads keys and values from provided file path
        /// </summary>
        /// <param name="path">The Path of key source file.</param>
        /// <param name="createIfEmpty">The flag for file creation if it does not exist on provided path.</param>
        /// <exception cref="ArgumentNullException">Thrown when provided path string is null or empty.</exception>
        /// <exception cref="ArgumentException">Thrown when key source file does not exist on provided path.</exception>
        /// <exception cref="FileEmptyException">Thrown when key source file is empty.</exception>
        public void LoadFromFile(string path, bool createIfEmpty = false)
        {
            //Path validation
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentNullException(nameof(path), "path can't be null or empty");
            }

            //File existence check
            if (!createIfEmpty && !File.Exists(path))
            {
                throw new ArgumentException(nameof(path), "No file at path: " + path);
            }

            //Check if file creation is requested
            if (createIfEmpty && !File.Exists(path))
            {
                // Create empty keyfile
                File.WriteAllText(path, "[]");
            }

            // Read the file from disk
            string fileContents = File.ReadAllText(path);
            if (string.IsNullOrWhiteSpace(fileContents))
            {
                throw new FileEmptyException($"File at path {path} is empty.");
            }

            _sourceFile = path;
            LoadFromJson(fileContents, createIfEmpty);
        }

        /// <summary>
        /// This function loads keys and values from provided JSON string
        /// </summary>
        /// <param name="json">The JSON string having keys data.</param>
        /// <param name="emptyOk">The flag which check for JSON 0 number of keys.</param>
        /// <exception cref="KeyLoadException">Thrown when invalid keys are provided or 0 number of keys are provided with emptyOk flag false.</exception>
        public void LoadFromJson(string json, bool emptyOk = false)
        {
            //Loading JSON from string
            List<JsonPublicNodeKey> jsonKeys = null;
            try
            {
                jsonKeys = JsonConvert.DeserializeObject<List<JsonPublicNodeKey>>(json);
            }
            catch (Exception ex)
            {
                throw new KeyLoadException($"Unable to load keys from json", ex);
            }

            //Check for JSON 0 keys
            if (!emptyOk && jsonKeys.Count == 0)
            {
                throw new KeyLoadException($"JSON contains no keys");
            }

            _loadedKeys = jsonKeys;
        }

        /// <summary>
        /// Function for saving loaded keys into file.
        /// </summary>
        /// <exception cref="Exception">Thrown when keys are not loaded from file.</exception>
        /// <exception cref="FileNotFoundException">Thrown when source file doesn't exist.</exception>
        public void SaveToFile()
        {
            //check for invalid sourcefile
            if (string.IsNullOrWhiteSpace(_sourceFile))
            {
                throw new Exception("Not loaded from file.");
            }

            //check for file existence
            if (!File.Exists(_sourceFile))
            {
                throw new FileNotFoundException("Source file no longer exists.");
            }

            //serialization and writing data to file
            string json = JsonConvert.SerializeObject(_loadedKeys, Formatting.Indented);
            File.WriteAllText(_sourceFile, json);

        }

        /// <summary>
        /// This function gets key for provided node Id from key source.
        /// </summary>
        /// <param name="nodeId">The node Id for which key is required.</param>
        /// <returns>returns Public key from key source.</returns>
        /// <exception cref="KeyNotFoundException">Thrown when key does not exist in key source.</exception>
        public string GetKeyForNode(string nodeId)
        {
            string key = (_loadedKeys.Where(x => x.NodeId == nodeId)).FirstOrDefault()?.PublicKey;
            if (key == null)
            {
                throw new KeyNotFoundException("Public key not available");
            }

            return key;
        }

        /// <summary>
        /// This function adds key and node Id to key source.
        /// </summary>
        /// <param name="nodeId">The node Id to be registered.</param>
        /// <param name="pubkeyAsBase64">The Base64 encoded public key to be registered.</param>
        public void AddKey(string nodeId, string pubkeyAsBase64)
        {
            //adding new key and saving to file
            _loadedKeys.Add(new JsonPublicNodeKey
            {
                NodeId = nodeId,
                PublicKey = pubkeyAsBase64
            });
            SaveToFile();
        }

        /// <summary>
        /// This function removes key for provided node Id from key source.
        /// </summary>
        /// <param name="nodeId">The node Id for which key removal is performed.</param>
        /// <exception cref="KeyNotFoundException">Thrown when key does not exist in key source.</exception>
        public void RemoveKey(string nodeId)
        {
            //removing key and saving to fil
            var key = _loadedKeys.FirstOrDefault(x => x.NodeId == nodeId);
            if (key == null)
            {
                throw new KeyNotFoundException("Node key is not known");
            }

            _loadedKeys.Remove(key);
            SaveToFile();
        }

        /// <summary>
        /// Function for loading keys from file
        /// </summary>
        /// <param name="keyfileJson">The path of key source file.</param>
        /// <param name="createIfEmpty">The flag for file creation if it does not exist on provided path.</param>
        /// <returns>returns JsonPublicKeySource object.</returns>
        /// <exception cref="ArgumentNullException">Thrown when provided path string is null or empty.</exception>
        /// <exception cref="ArgumentException">Thrown when key source file does not exist on provided path.</exception>
        /// <exception cref="FileEmptyException">Thrown when key source file is empty.</exception>
        public static IPublickeySource FromFile(string keyfileJson, bool createIfEmpty = false)
        {
            //directly load from file
            JsonPublicKeySource source = new JsonPublicKeySource();
            source.LoadFromFile(keyfileJson, createIfEmpty);
            return source;
        }
    }
}