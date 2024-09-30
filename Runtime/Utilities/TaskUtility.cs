using System;
using System.Threading;
using System.Threading.Tasks;

namespace Baracuda.Utility.Utilities
{
    public static class TaskUtility
    {
        public static async Task WaitWhile(Func<bool> condition,
            CancellationToken cancellationToken = new())
        {
            while (condition())
            {
                await Task.Delay(25, cancellationToken);
            }
        }

        public static async Task WaitUntil(Func<bool> condition,
            CancellationToken cancellationToken = new())
        {
            while (!condition())
            {
                await Task.Delay(25, cancellationToken);
            }
        }
    }
}