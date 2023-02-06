using Baracuda.Utilities.Callbacks;
using System.Collections.Generic;

namespace Baracuda.Gameloop.Jobs
{
    internal static class JobSystem
    {
        private static readonly List<JobBase> activeUpdateJobs = new(64);

        static JobSystem()
        {
            EngineCallbacks.Update += OnUpdate;
        }

        internal static void RegisterJob(JobBase job)
        {
            activeUpdateJobs.Add(job);
        }

        internal static void UnregisterJob(JobBase job)
        {
            activeUpdateJobs.Remove(job);
        }

        private static void OnUpdate(float deltaTime)
        {
            for (var i = activeUpdateJobs.Count - 1; i >= 0; i--)
            {
                activeUpdateJobs[i].Update(deltaTime);
            }
        }
    }
}