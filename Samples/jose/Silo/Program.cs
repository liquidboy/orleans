
using Orleans.Runtime.Configuration;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Hosting;
using System.Collections.Generic;
using Grains;
using System.Net;
using System.Linq;
using Orleans.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace Silo
{
    class Program
    {
        static string postgresServer;
        static bool isDevMode;
        static int proxyGatewayPort;
        static bool isPrimary;
        static string siloName;

        public static int Main(string[] args)
        {
            postgresServer = args[0];
            proxyGatewayPort = int.Parse(args[1]);
            isDevMode = (args[2].ToLower().Equals("true")) ? true : false;
            isPrimary = (args[3].ToLower().Equals("true")) ? true : false;
            siloName = args[4];

            return RunMainAsync().Result;
        }

        private static async Task<int> RunMainAsync()
        {
            try
            {
                var host = await StartSilo();
                Console.WriteLine("Press Enter to terminate...");
                Console.ReadLine();
                await host.StopAsync();
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return 1;
            }
        }

        // note: system store - https://github.com/dotnet/orleans/issues/3316


        private static async Task<ISiloHost> StartSilo()
        {
            var dataConnection = $"User ID=postgres;Password=Password123;Server={postgresServer};Port=5432;Database=orleans;Pooling=true;Timeout=15;CommandTimeout=15";

            // define the cluster configuration
            var config = new ClusterConfiguration();
            config.AddMemoryStorageProvider();
            // var config = ClusterConfiguration.LocalhostPrimarySilo();
            config.LoadFromFile("OrleansConfiguration.xml");


            var properties = new Dictionary<string, string> {
                { "DataConnectionString", dataConnection},
                { "ConnectionString", $"User ID=postgres;Password=Password123;Server={postgresServer};Port=5432;Database=orleans;Pooling=true;Timeout=15;CommandTimeout=15"},
                { "UseJsonFormat", "True" },
                { "DeleteStateOnClear", "True" },
                { "AdoInvariant", "Npgsql" },
                { "TraceToConsole", "True" },
                { "DefaultTraceLevel", "Verbose"}
            };

            config.Globals.RegisterStorageProvider<MemoryStorage>("MemoryStore");
            config.Globals.RegisterStorageProvider<AdoNetStorageProvider>("Default", properties);
            config.Globals.RegisterStorageProvider<AdoNetStorageProvider>("OrleansStorage", properties);
            config.Globals.RegisterStorageProvider<AdoNetStorageProvider>("PubSubStore", properties);
            config.Globals.RegisterStorageProvider<AdoNetStorageProvider>("AzureTable", properties);
            config.Globals.RegisterStorageProvider<AdoNetStorageProvider>("DataStorage", properties);
            config.Globals.RegisterStorageProvider<AdoNetStorageProvider>("BlobStorage", properties);

            
            config.Globals.ClusterId = "orleans-silo";

            config.Globals.DataConnectionString = $"User ID=postgres;Password=Password123;Server={postgresServer};Port=5432;Database=orleans;Pooling=true;Timeout=15;CommandTimeout=15";
            config.Globals.AdoInvariant = "Npgsql";

            config.Globals.LivenessEnabled = true;
            config.Globals.LivenessType = GlobalConfiguration.LivenessProviderType.SqlServer;

            config.Globals.ReminderServiceType = GlobalConfiguration.ReminderServiceProviderType.SqlServer;

            //config.Globals.FastKillOnCancelKeyPress = true;
            // config.Defaults.Port = 11111;
            config.Defaults.PropagateActivityId = true;
                        
            if (isDevMode )
            {
                config.Defaults.Port = proxyGatewayPort - 100;
                config.Defaults.HostNameOrIPAddress = "localhost";
                config.Defaults.ProxyGatewayEndpoint = new IPEndPoint(IPAddress.Any, proxyGatewayPort);
            }
            else {
                config.Defaults.Port = proxyGatewayPort - 100;

                var ips = await Dns.GetHostAddressesAsync(Dns.GetHostName());
                
                Console.WriteLine($"hostname : {Dns.GetHostName()}");
                Console.WriteLine("ip addresses on silo :");
                foreach (var ip in ips) {
                    Console.WriteLine($"  {ip}");
                }

                config.Defaults.HostNameOrIPAddress = ips.LastOrDefault()?.ToString();
                config.Defaults.ProxyGatewayEndpoint = new IPEndPoint(IPAddress.Any, proxyGatewayPort);

                Console.WriteLine(config.Defaults.HostNameOrIPAddress);
            }


            // Primary Silo's is defined by the seend nodes
            // config.Globals.SeedNodes.Add(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 11111));
            
            var builder = new SiloHostBuilder()
                .UseConfiguration(config)
                // .ConfigureSiloName(siloName)
                .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(Counter).Assembly).WithReferences())
                .ConfigureLogging(logging => logging.AddFilter(  "orleans", LogLevel.Trace));
            
            var host = builder.Build();
            await host.StartAsync();

            return host;
        }
        

    }
}
