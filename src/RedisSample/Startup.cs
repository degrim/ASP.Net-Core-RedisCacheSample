using System;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RedisSample.Models;
using RedisSample.Services;
using Newtonsoft.Json;

namespace RedisSample
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            // Set up configuration sources.
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddJsonFile("config.json", optional: true)
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
            var creds = new RedisCredentials
                {
                    Hostname = Configuration["rediscloud:0:credentials:hostname"],
                    Password = Configuration["rediscloud:0:credentials:password"],
                    Port = Configuration["rediscloud:0:credentials:port"]
                };
            var database = 0; // specify which redis database to use for storing session data
            var idleTimeout = TimeSpan.FromMinutes(30);

            services.AddRedisCaching(creds, database, idleTimeout);

            services.AddSession(options =>
            {
                options.CookieHttpOnly = true;
                options.CookieName = "RedisSampleSession";
                options.IdleTimeout = idleTimeout;
            });

            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseIISPlatformHandler();

            app.UseStaticFiles();

            app.UseSession();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        // Entry point for the application.
        public static void Main(string[] args) => WebApplication.Run<Startup>(args);
    }
}
