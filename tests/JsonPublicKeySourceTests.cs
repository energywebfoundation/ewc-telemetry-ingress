using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using webapi;
using Xunit;

namespace tests
{
    public class JsonPublicKeySourceTests
    {

        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        [Fact]
        public void PublicKeySourceNullPathShouldFail()
        {
            JsonPublicKeySource obj = new JsonPublicKeySource();

            var exception = Assert.Throws<ArgumentNullException>(() => obj.LoadFromFile(""));
            Assert.NotNull(exception);

            Assert.True(exception.Message.Contains("path can't be null or empty"));
        }

        [Fact]
        public void PublicKeySourceNoFileShouldFail()
        {
            JsonPublicKeySource obj = new JsonPublicKeySource();

            var exception = Assert.Throws<ArgumentException>(() => obj.LoadFromFile("\\etc\\file.json"));
            Assert.NotNull(exception);

            Assert.True(exception.Message.Contains("No file at path: "));
        }

        [Fact]
        public void CreateFileFileShouldPass()
        {
            JsonPublicKeySource obj = new JsonPublicKeySource();

            string randFileName = RandomString(6) + ".json";
            obj.LoadFromFile(randFileName, true);
            Assert.True(File.Exists(randFileName));

            //removing temp file
            File.Delete(randFileName);
        }

        [Fact]
        public void EmptyFileShouldFail()
        {
            JsonPublicKeySource obj = new JsonPublicKeySource();

            string randFileName = RandomString(6) + ".json";
            File.WriteAllText(randFileName, "");

            var exception = Assert.Throws<FileEmptyException>(() => obj.LoadFromFile(randFileName));
            Assert.NotNull(exception);

            Assert.True(exception.Message.Contains("is empty"));

            //removing temp file
            File.Delete(randFileName);
        }

        [Fact]
        public void InvalidKeysShouldFail()
        {
            JsonPublicKeySource obj = new JsonPublicKeySource();

            string randFileName = RandomString(6) + ".json";
            File.WriteAllText(randFileName, "{nodewe=w1");

            var exception = Assert.Throws<KeyLoadException>(() => obj.LoadFromFile(randFileName));
            Assert.NotNull(exception);

            Assert.True(exception.Message.Contains("Unable to load keys from json"));

            //removing temp file
            File.Delete(randFileName);
        }

        [Fact]
        public void ZeroKeysShouldFail()
        {
            JsonPublicKeySource obj = new JsonPublicKeySource();

            string randFileName = RandomString(6) + ".json";
            File.WriteAllText(randFileName, "[]");

            var exception = Assert.Throws<KeyLoadException>(() => obj.LoadFromFile(randFileName));
            Assert.NotNull(exception);

            Assert.True(exception.Message.Contains("JSON contains no keys"));

            //removing temp file
            File.Delete(randFileName);
        }


        [Fact]
        public void SaveToFileNullPathShouldFail()
        {
            JsonPublicKeySource obj = new JsonPublicKeySource();

            var exceptionSTF = Assert.Throws<Exception>(() => obj.SaveToFile());
            Assert.NotNull(exceptionSTF);
            Assert.True(exceptionSTF.Message.Contains("Not loaded from file."));
        }

        [Fact]
        public void SaveToFileNoFileExistFail()
        {
            JsonPublicKeySource obj = new JsonPublicKeySource();

            string randFileName = RandomString(6) + ".json";
            obj.LoadFromFile(randFileName, true);
            File.Delete(randFileName);


            var exceptionSTF = Assert.Throws<FileNotFoundException>(() => obj.SaveToFile());
            Assert.NotNull(exceptionSTF);
            Assert.True(exceptionSTF.Message.Contains("Source file no longer exists."));
        }

        [Fact]
        public void GetKeyForNodeShouldFailForKeyNotFound()
        {
            JsonPublicKeySource obj = new JsonPublicKeySource();

            string randFileName = RandomString(6) + ".json";
            File.WriteAllText(randFileName, "[ {\"nodeid\": \"node-3\",\"key\": \"BgIA345nlikrwegfSDFG=\"}]");

            obj.LoadFromFile(randFileName, true);

            var exception = Assert.Throws<KeyNotFoundException>(() => obj.GetKeyForNode("Node-12"));
            Assert.NotNull(exception);
            Assert.True(exception.Message.Contains("Public key not available"));

            //removing temp file
            File.Delete(randFileName);
        }

        [Fact]
        public void GetKeyForNodeShouldPasslForKeyFound()
        {
            JsonPublicKeySource obj = new JsonPublicKeySource();

            string randFileName = RandomString(6) + ".json";
            File.WriteAllText(randFileName, "[ {\"nodeid\": \"node-3\",\"key\": \"BdrwegfSDFG=\"},{\"nodeid\": \"node-12\",\"key\": \"BgIA345nlikrwegfSDFG=\"}]");

            obj.LoadFromFile(randFileName, true);

            string key = obj.GetKeyForNode("node-12");
            Assert.Equal(key, "BgIA345nlikrwegfSDFG=");

            //removing temp file
            File.Delete(randFileName);
        }

        [Fact]
        public void AddRemoveKeyShouldPassAndNonExistintKeyShouldFail()
        {
            JsonPublicKeySource obj = new JsonPublicKeySource();

            string randFileName = RandomString(6) + ".json";
            obj.LoadFromFile(randFileName, true);

            string nodeId = "0x00000000012";
            string pubKey = "SADDFAJIONJKASD234ASDASNK34234=";

            obj.AddKey(nodeId, pubKey);

            string key = obj.GetKeyForNode(nodeId);
            Assert.Equal(key, pubKey);

            string nodeId2 = "0x00000000003";
            string pubKey2 = "SASADDFAJIONJKASD234ASDASNK34234DDFAJIONJKASD234ASDASNK34234=";
            obj.AddKey(nodeId2, pubKey2);

            string key2 = obj.GetKeyForNode(nodeId2);
            Assert.Equal(key2, pubKey2);

            //now removing key
            obj.RemoveKey(nodeId2);

            //verifying that key is removed
            var exception = Assert.Throws<KeyNotFoundException>(() => obj.RemoveKey(nodeId2));
            Assert.NotNull(exception);
            Assert.True(exception.Message.Contains("Node key is not known"));

            //removing temp file
            File.Delete(randFileName);
        }

        [Fact]
        public void LoadFromFileShouldPass()
        {

            string randFileName = RandomString(6) + ".json";
            File.WriteAllText(randFileName, "[ {\"nodeid\": \"node-3\",\"key\": \"BdrwegfSDFG=\"},{\"nodeid\": \"node-12\",\"key\": \"BgIA345nlikrwegfSDFG=\"}]");
            IPublickeySource obj = JsonPublicKeySource.FromFile(randFileName, false);

            string key = obj.GetKeyForNode("node-3");
            Assert.Equal(key, "BdrwegfSDFG=");

            //removing temp file
            File.Delete(randFileName);

        }



    }
}