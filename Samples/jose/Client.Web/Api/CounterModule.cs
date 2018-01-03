using Botwin;
using Grains;
using Microsoft.AspNetCore.Http;
using Orleans;

namespace Web.Api
{
    // https://github.com/jchannon/Botwin
    public class CounterModule : BotwinModule
    {
        
        public CounterModule(IClusterClient clusterClient) : base("api")
        {
            Get("/counter/", async (request, response, _) =>
            {
                var counter = clusterClient.GetGrain<ICounterGrain>("Demo");
                var currentCount = await counter.GetCount();
                var fingerPrint = await counter.GetFingerPrintedCount();
                await response.WriteAsync($"{fingerPrint} {currentCount.ToString()}");
            });

            Post("/counter/", async (request, response, _) =>
            {
                var counter = clusterClient.GetGrain<ICounterGrain>("Demo");
                await counter.Increment(1);
                response.StatusCode = 204;
            });

        }

    }
}



