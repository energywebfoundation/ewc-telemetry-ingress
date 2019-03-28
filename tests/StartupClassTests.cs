using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Moq;
using webapi;
using Xunit;

namespace tests
{
    public class StartupClassTests
    {
        [Fact]
        public void ConfigGetShouldPass()
        {

            ConfigurationBuilder cb = new ConfigurationBuilder();
            IConfigurationRoot configObj = cb.AddInfluxConfigFromEnvironment().Build();

            Startup su = new Startup(configObj);
            Assert.Equal(su.Configuration, configObj);
        }

/* 
        [Fact]
        public void IConfigGetShouldPass2()
        {

            ConfigurationBuilder cb = new ConfigurationBuilder();
            IConfigurationRoot configObj = cb.AddInfluxConfigFromEnvironment().Build();

            var dummyHostingEnvObj =  new Mock<IHostingEnvironment>(MockBehavior.Strict);
            dummyHostingEnvObj
                .Setup(p => p.IsDevelopment())
                .Returns(true);

            var dummyAppBuilderObj = new Mock<IApplicationBuilder>();
            dummyAppBuilderObj.Setup(p => p.UseDeveloperExceptionPage());
            dummyAppBuilderObj.Setup(p => p.UseMvc());


            Startup su = new Startup(configObj);
            su.Configure(dummyAppBuilderObj.Object, 
            
            dummyHostingEnvObj.Object);

            //no assert just for coverage
        }*/
    }
}
