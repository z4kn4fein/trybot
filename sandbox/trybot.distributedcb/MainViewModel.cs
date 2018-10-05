using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using Trybot.CircuitBreaker;

namespace Trybot.DistributedCB
{
    public class MainViewModel
    {
        public ObservableCollection<CbViewModel> Cbs { get; set; } = new ObservableCollection<CbViewModel>();

        public MainViewModel()
        {
            var states = new ConcurrentDictionary<string, CircuitState>();
            for (var i = 0; i < 10; i++)
            {
                this.Cbs.Add(new CbViewModel("cb" + i, states));
            }
        }
    }
}
