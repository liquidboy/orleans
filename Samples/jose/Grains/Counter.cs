using Orleans;
// using Orleans.ApplicationParts;
using Orleans.Providers;
using System.Net;
using System.Threading.Tasks;

namespace Grains
{
    public class CounterState
    {
        public int Count { get; set; }
    }

    public interface ICounterGrain : IGrainWithStringKey
    {
        Task Increment(int increment);

        Task<int> GetCount();

        Task<string> GetFingerPrintedCount();
    }



    [StorageProvider(ProviderName = "OrleansStorage")]
    public class Counter : Grain<CounterState>, ICounterGrain
    {

        public Task Increment(int increment)
        {
            State.Count += increment;
            return base.WriteStateAsync();

            //_counter++;
            //return Task.CompletedTask;
        }
        
        public async Task<int> GetCount()
        {
            //return Task.FromResult(_counter);
            await base.ReadStateAsync();
            return State.Count;
        }

        public async Task<string> GetFingerPrintedCount()
        {
            // return Task.FromResult(_counter);
            // await base.ReadStateAsync();
            var fp = await GetFingerPrint();
            return $"{fp}";
        }


        public async Task<string> GetFingerPrint()
        {
            var html = $"[{Dns.GetHostName()}";
            var ips = await Dns.GetHostAddressesAsync(Dns.GetHostName());
            foreach (var ip in ips)
            {
                html += $"|{ip}";
            }
            html += "]";
            return html;
        }

    }
}
