using Baracuda.Gameloop.Update;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Baracuda.Gameloop.Jobs
{
    internal class JobManager : ScriptableObject, ITickReceiver, IDisposable
    {
        private static readonly List<JobBase> activeUpdateJobs = new(64);

        internal static void RegisterJob(JobBase job)
        {
            activeUpdateJobs.Add(job);
        }

        internal static void UnregisterJob(JobBase job)
        {
            activeUpdateJobs.Remove(job);
        }


        public void Tick(float deltaTime)
        {
            for (var i = activeUpdateJobs.Count - 1; i >= 0; i--)
            {
                activeUpdateJobs[i].Update(deltaTime);
            }
        }

        public void Dispose()
        {
            activeUpdateJobs.Clear();
        }
    }
}