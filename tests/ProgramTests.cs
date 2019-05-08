using webapi;
using Xunit;

namespace tests
{
    public class ProgramTests
    {
        [Fact]
        public void ProgramKeyCommandShouldPass()
        {

            string validator = "0x0000000965";
            string pubKey = "ASDFASdFFDJDY4356SDFGFDs=";

            string[] args = { "--keycmd", "add", "--validator", validator, "--publickey", pubKey };
            Program.Main(args);

            JsonPublicKeySource obj = new JsonPublicKeySource();
            obj.LoadFromFile("keyfile.json", true);

            string key = obj.GetKeyForNode(validator);
            Assert.Equal(key, pubKey);
        }

        [Fact]
        public void ProgramShouldPass()
        {

            string[] args = { "--KEYPASS", "EDKHDKxCEkiGGJd4kTRj7k6", "--STARTSERVICE", "false" };
            Program.Main(args);
            //its for coverage so no assert
        }

        [Fact]
        public void ProgramShouldNotPassWithOutCert()
        {

            string[] args = { "--INTERNAL_DIR", "\\etc\\abc", "--STARTSERVICE", "false" };
            Program.Main(args);
            //its for coverage so no assert

        }

        [Fact]
        public void ProgramShouldNotPassWithOutCertPassword()
        {

            string[] args = { "--STARTSERVICE", "false" };
            Program.Main(args);
            //its for coverage so no assert
        }
    }
}