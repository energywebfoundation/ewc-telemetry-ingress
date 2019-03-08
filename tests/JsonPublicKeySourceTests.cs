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

            string randFileName= RandomString(6)+".json";
            obj.LoadFromFile(randFileName, true);
            Assert.True(File.Exists(randFileName));
        }

        [Fact]
        public void EmptyFileShouldFail()
        {
            JsonPublicKeySource obj = new JsonPublicKeySource();

            string randFileName= RandomString(6)+".json";
            File.WriteAllText(randFileName,"");
            
            var exception = Assert.Throws<FileEmptyException>(() => obj.LoadFromFile(randFileName));
            Assert.NotNull(exception);

            Assert.True(exception.Message.Contains("is empty"));
        }

        [Fact]
        public void InvalidKeysShouldFail()
        {
            JsonPublicKeySource obj = new JsonPublicKeySource();

            string randFileName= RandomString(6)+".json";
            File.WriteAllText(randFileName,"{nodewe=w1");
            
            var exception = Assert.Throws<KeyLoadException>(() => obj.LoadFromFile(randFileName));
            Assert.NotNull(exception);

            Assert.True(exception.Message.Contains("Unable to load keys from json"));
        }

        [Fact]
        public void ZeroKeysShouldFail()
        {
            JsonPublicKeySource obj = new JsonPublicKeySource();

            string randFileName= RandomString(6)+".json";
            File.WriteAllText(randFileName,"[]");
            
            var exception = Assert.Throws<KeyLoadException>(() => obj.LoadFromFile(randFileName));
            Assert.NotNull(exception);

            Assert.True(exception.Message.Contains("JSON contains no keys"));
        }


        [Fact]
        public void SaveToFileNullPathShouldFail()
        {
            JsonPublicKeySource obj = new JsonPublicKeySource();

            //var exception = Assert.Throws<ArgumentException>(() => obj.LoadFromFile(""));
            //Assert.NotNull(exception);

            var exceptionSTF = Assert.Throws<Exception>(() => obj.SaveToFile());
            Assert.NotNull(exceptionSTF);
            Assert.True(exceptionSTF.Message.Contains("Not loaded from file."));
        }

        [Fact]
        public void SaveToFileNoFileExistFail()
        {
            JsonPublicKeySource obj = new JsonPublicKeySource();

            string randFileName= RandomString(6)+".json";
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

            string randFileName= RandomString(6)+".json";
            File.WriteAllText(randFileName,"[ {\"nodeid\": \"node-3\",\"key\": \"BgIA345nlikrwegfSDFG=\"}]");

            obj.LoadFromFile(randFileName, true);

            var exception = Assert.Throws<KeyNotFoundException>(() => obj.GetKeyForNode("Node-12"));
            Assert.NotNull(exception);
            Assert.True(exception.Message.Contains("Public key not available"));
        }

        [Fact]
        public void GetKeyForNodeShouldPasslForKeyFound()
        {
            JsonPublicKeySource obj = new JsonPublicKeySource();

            string randFileName= RandomString(6)+".json";
            File.WriteAllText(randFileName,"[ {\"nodeid\": \"node-3\",\"key\": \"BdrwegfSDFG=\"},{\"nodeid\": \"node-12\",\"key\": \"BgIA345nlikrwegfSDFG=\"}]");

            obj.LoadFromFile(randFileName, true);

            string key = obj.GetKeyForNode("node-12");
            Assert.Equal(key , "BgIA345nlikrwegfSDFG=");
        }


    }
}