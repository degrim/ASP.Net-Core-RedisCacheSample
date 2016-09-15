using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Net;
using System.Threading.Tasks;

namespace RedisSample
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            // Set up configuration sources.
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile("vcap-local.json", optional: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();

            string vcapServices = Environment.GetEnvironmentVariable("VCAP_SERVICES");
            if (vcapServices != null)
            {
                dynamic json = JsonConvert.DeserializeObject(vcapServices);
                if (json.rediscloud != null)
                {
                    string hostname = json.rediscloud[0].credentials.hostname;
                    string password = json.rediscloud[0].credentials.password;
                    string port = json.rediscloud[0].credentials.port;
                    Configuration["rediscloud:0:credentials:hostname"] = hostname;
                    Configuration["rediscloud:0:credentials:password"] = password;
                    Configuration["rediscloud:0:credentials:port"] = port;
                }
            }
        }

        public IConfigurationRoot Configuration { get; set; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            IPAddress ip = GetIPAddress(Configuration["rediscloud:0:credentials:hostname"]).Result;
            var connectionString = $"{ip}:{Configuration["rediscloud:0:credentials:port"]},password={Configuration["rediscloud:0:credentials:password"]}";
            services.AddDistributedRedisCache(options =>
            {
                options.Configuration = connectionString;
            });
            var redis = ConnectionMultiplexer.Connect(connectionString);
            services.AddDataProtection()
                .PersistKeysToRedis(redis, "DataProtection-Keys");
            services.AddSession();
        }

        public async Task<IPAddress> GetIPAddress(string hostname)
        {
            return (await Dns.GetHostEntryAsync(hostname)).AddressList[0];
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseDeveloperExceptionPage();

            app.UseStaticFiles();

            app.UseSession();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
