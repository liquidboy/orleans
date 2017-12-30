using Orleans;
// using Orleans.ApplicationParts;
using Orleans.Providers;
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
    }
}
