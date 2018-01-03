using System;
using System.Net;
using Botwin;
using Grains;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Orleans;
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
            var gateways = Configuration["silo_gateways"].Split("-");
            
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
                            // docker : https://dotnet.github.io/orleans/Documentation/Deployment-and-Operations/Docker-Deployment.html
                            var config = new ClientConfiguration();
                            config.ClientName = "orleans-client";
                            config.ClusterId = "orleans-docker";
                            config.PropagateActivityId = true;

                            foreach (var gateway in gateways) {
                                var gatewayParts = gateway.Split(":");
                                config.Gateways.Add(new IPEndPoint(IPAddress.Parse(gatewayParts[0]), int.Parse(gatewayParts[1])));
                            }
                            
                            config.PreferedGatewayIndex = 0;
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
