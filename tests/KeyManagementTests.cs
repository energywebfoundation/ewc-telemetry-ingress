using System.Collections.Generic;
using System.IO;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using webapi;
using Xunit;

namespace tests
{
    public class KeyManagementTests
    {

        [Fact]
        public void AddKeyShouldFailWithoutValidator()
        {
            JsonPublicKeySource pubKeySourceObj = new JsonPublicKeySource();
            string randFileName = JsonPublicKeySourceTests.RandomString(6) + ".json";
            pubKeySourceObj.LoadFromFile(randFileName, true);

            IConfigurationRoot configObj = new ConfigurationBuilder().Build();

            KeyManagement km = new KeyManagement(configObj, pubKeySourceObj);
            km.ProcessKeyCommand("add");
            string fileData = File.ReadAllText(randFileName);

            fileData.Should().Contain("[]");
            //removing temp file
            File.Delete(randFileName);
        }

        [Fact]
        public void AddKeyShouldFailWithoutPublickey()
        {
            JsonPublicKeySource pubKeySourceObj = new JsonPublicKeySource();
            string randFileName = JsonPublicKeySourceTests.RandomString(6) + ".json";
            pubKeySourceObj.LoadFromFile(randFileName, true);

            ConfigurationBuilder cb = new ConfigurationBuilder();
            string[] args = { "--validator", "0x0000000965" };
            IConfigurationRoot configObj = cb.AddCommandLine(args).Build();

            KeyManagement km = new KeyManagement(configObj, pubKeySourceObj);
            km.ProcessKeyCommand("add");
            string fileData = File.ReadAllText(randFileName);

            fileData.Should().Contain("[]");

            //removing temp file
            File.Delete(randFileName);
        }

        [Fact]
        public void AddKeyShouldPassWithValidArgs()
        {
            string validator = "0x0000000965";
            string pubKey = "ASDFASdFFDJDY4356SDFGFDs=";

            JsonPublicKeySource pubKeySourceObj = new JsonPublicKeySource();
            string randFileName = JsonPublicKeySourceTests.RandomString(6) + ".json";
            pubKeySourceObj.LoadFromFile(randFileName, true);

            ConfigurationBuilder cb = new ConfigurationBuilder();
            string[] args = { "--validator", validator, "--publickey", pubKey };
            IConfigurationRoot configObj = cb.AddCommandLine(args).Build();

            KeyManagement km = new KeyManagement(configObj, pubKeySourceObj);
            km.ProcessKeyCommand("add");

            //Now check file independently that key Registration was success

            JsonPublicKeySource obj = new JsonPublicKeySource();
            obj.LoadFromFile(randFileName, true);

            string key = obj.GetKeyForNode(validator);
            Assert.Equal(key, pubKey);

            //removing temp file
            File.Delete(randFileName);
        }

        [Fact]
        public void RemveKeyWithInvalidValidatorShouldFail()
        {
            string validator = "0x00000005476534";
            string pubKey = "TREHXCAVSOYJULSDVGFDs=";
            JsonPublicKeySource pubKeySourceObj = new JsonPublicKeySource();
            string randFileName = JsonPublicKeySourceTests.RandomString(6) + ".json";
            pubKeySourceObj.LoadFromFile(randFileName, true);

            //Independently adding key for removal test
            pubKeySourceObj.AddKey(validator, pubKey);

            ConfigurationBuilder cb = new ConfigurationBuilder();
            string[] args = { "test", validator };
            IConfigurationRoot configObj = cb.AddCommandLine(args).Build();


            KeyManagement km = new KeyManagement(configObj, pubKeySourceObj);
            km.ProcessKeyCommand("remove");

            //check the keys should not be removed
            pubKeySourceObj.GetKeyForNode(validator).Should().Be(pubKey);

            //removing temp file
            File.Delete(randFileName);
        }

        [Fact]
        public void RemveKeyWithValidValidatorShouldPass()
        {
            string validator = "0x00000005476534";
            string pubKey = "TREHXCAVSOYJULSDVGFDs=";
            JsonPublicKeySource pubKeySourceObj = new JsonPublicKeySource();
            string randFileName = JsonPublicKeySourceTests.RandomString(6) + ".json";
            pubKeySourceObj.LoadFromFile(randFileName, true);

            //Independently adding key for removal test
            pubKeySourceObj.AddKey(validator, pubKey);

            ConfigurationBuilder cb = new ConfigurationBuilder();
            string[] args = { "--validator", validator };
            IConfigurationRoot configObj = cb.AddCommandLine(args).Build();


            KeyManagement km = new KeyManagement(configObj, pubKeySourceObj);
            km.ProcessKeyCommand("remove");

            //check the keys should be removed
            var exception = Assert.Throws<KeyNotFoundException>(() => pubKeySourceObj.GetKeyForNode(validator));
            Assert.NotNull(exception);

            exception.Message.Should().Contain("Public key not available");

            //removing temp file
            File.Delete(randFileName);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("asd")]
        [InlineData("---")]
        public void InvalidArgsShouldFail(string command)
        {
            JsonPublicKeySource pubKeySourceObj = new JsonPublicKeySource();
            string randFileName = JsonPublicKeySourceTests.RandomString(6) + ".json";
            pubKeySourceObj.LoadFromFile(randFileName, true);

            KeyManagement km = new KeyManagement(null, null);
            km.ProcessKeyCommand(command);

            string fileData = File.ReadAllText(randFileName);
            //there is no change in pub key data file
            fileData.Should().Contain("[]");
            //removing temp file
            File.Delete(randFileName);

        }

    }
}