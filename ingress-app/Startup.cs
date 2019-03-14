using System;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using webapi.Controllers;

namespace webapi
{
    /// <summary>
    /// Class with functions for startup and configurations
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Startup Constructor.
        /// </summary>
        /// <param name="configuration">Constructor expects Configuration reference.</param>
        /// <returns>returns instance of Startup</returns>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// Configuration property
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services">The method expects ServiceCollection reference.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddSingleton<IPublickeySource>(serviceProvider =>
                JsonPublicKeySource.FromFile(
                Path.Combine(Configuration.GetValue<string>("INTERNAL_DIR", "./"), "keyfile.json"))
            );


            services.AddSingleton<IInfluxClient, InfluxClient>(serviceProvider =>
               {
                   var lpcp = Configuration.GetSection("Influx").Get<LineProtocolConnectionParameters>();
                   return new InfluxClient(lpcp);

               });

        }


        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app">The method expects ApplicationBuilder reference.</param>
        /// <param name="env">The method expects HostingEnvironment reference.</param>
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
                app.UseHttpsRedirection();
            }

            app.UseMvc();
        }
    }
}
