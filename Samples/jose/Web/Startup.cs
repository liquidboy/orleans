using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Botwin;
using Grains;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Providers.Streams.Common;
using Orleans.Runtime;
using Orleans.Runtime.Configuration;
using Polly;

namespace Web
{
    public class Startup
    {

        public Startup(IConfiguration configuration)
        { 
            Configuration = configuration;
        }
         
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            IPAddress gatewayIp = System.Net.IPAddress.Parse(Configuration["proxyip"]);
            var proxyPort = int.Parse(Configuration["proxyport"]);
            //try
            //{
            //    var hostEntry = Dns.GetHostEntry("orleans-silo");
            //    gatewayIp = hostEntry.AddressList[0];
            //}
            //catch {
            //    // not running in docker or swarm/networked environment 
            //}

            //var dataConnection = $"User ID=postgres;Password=Password123;Server={Configuration["postgresServer"]};Port=5432;Database=orleans;Pooling=true;Timeout=15;CommandTimeout=15";



            // https://github.com/jchannon/Botwin
            services.AddBotwin();
            services.AddMvc();
            services.AddSingleton( provider =>
            {

                // https://github.com/App-vNext/Polly
                return Policy<IClusterClient>
                    .Handle<Exception>()
                    .WaitAndRetry(new[]
                    {
                        TimeSpan.FromSeconds(30),
                        TimeSpan.FromSeconds(60),
                        TimeSpan.FromSeconds(90)

                    })
                    .Execute(() =>
                    {
                        IClusterClient client;

                        while (true)
                        {
                            // https://github.com/dotnet/orleans/blob/master/Samples/HelloWorld.NetCore/src/OrleansClient/Program.cs
                            // https://github.com/RayTale/Ray/tree/master/Example/Ray.IGrains
                            // var config = ClientConfiguration.LocalhostSilo(proxyPort);
                            var config = new ClientConfiguration();
                            config.ClientName = "orleans-client";
                            // config.GatewayProvider = ClientConfiguration.GatewayProviderType.Custom;
                            config.ClusterId = "orleans-client";
                            config.PropagateActivityId = true;


                            // Some top level features
                            //config.GatewayProvider = ClientConfiguration.GatewayProviderType.None;
                            //config.ResponseTimeout = TimeSpan.FromSeconds(30);
                            //// config.DataConnectionString = dataConnection;
                            //config.AdoInvariant = "Npgsql";
                            config.Gateways.Add(new IPEndPoint(gatewayIp, proxyPort));


                            // config.PreferedGatewayIndex = 0;
                            client = new ClientBuilder()
                                .UseConfiguration(config)
                                .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(ICounterGrain).Assembly).WithReferences())
                                .ConfigureLogging(logging => logging.AddConsole())
                                .Build();

                            client.Connect().ConfigureAwait(true);
                            
                            break;
                        }

                       
                        return client;

                    });


            });


        }
        
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseBotwin();
            app.UseStaticFiles();

            //app.UseMvc(routes =>
            //{
            //    routes.MapRoute(
            //        name: "default",
            //        template: "{controller}/{action=Index}/{id?}");
            //});
        }
    }
}
