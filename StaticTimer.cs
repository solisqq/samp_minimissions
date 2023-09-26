using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace partymode
{
    public static class StaticTimer
    {
        public static Task RunAsync(TimeSpan delay, Action action)
        {
            return Task.Delay(delay).ContinueWith(t => action(), TaskScheduler.FromCurrentSynchronizationContext());
        }
    }
}
