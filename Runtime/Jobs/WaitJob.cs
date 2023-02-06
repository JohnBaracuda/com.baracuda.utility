using Baracuda.Utilities;
using System;
using System.Runtime.CompilerServices;

namespace Baracuda.Gameloop.Jobs
{
    public class WaitJob : JobBase
    {
        /*
         * Fields
         */

        private float _duration;
        private float _waitSeconds;
        private Action _action;

        /*
         * Factory
         */

        public static WaitJobHandle Run(Action action, float waitSeconds)
        {
            var waitJob = WaitJobPool.Get();
            waitJob._duration = 0;
            waitJob._action = action;
            waitJob._waitSeconds = waitSeconds;
            waitJob.IsValid = true;
            JobSystem.RegisterJob(waitJob);
            return new WaitJobHandle(waitJob);
        }

        public static WaitJobHandle Run(Action action, int waitMilliseconds)
        {
            var waitJob = WaitJobPool.Get();
            waitJob._duration = 0;
            waitJob._action = action;
            waitJob._waitSeconds = waitMilliseconds.MillisecondsToSeconds();
            waitJob.IsValid = true;
            JobSystem.RegisterJob(waitJob);
            return new WaitJobHandle(waitJob);
        }

        /*
         * Public
         */

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void Cancel()
        {
            Reset();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void Complete()
        {
            _action?.Invoke();
            Reset();
        }

        //--------------------------------------------------------------------------------------------------------------

        /*
         * Internal
         */

        internal override void Update(float delta)
        {
            _duration += delta;
            if (_duration > _waitSeconds)
            {
                Complete();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Reset()
        {
            if (IsValid)
            {
                _action = null;
                _waitSeconds = 0;
                _duration = 0;
                JobSystem.UnregisterJob(this);
                WaitJobPool.Release(this);
                IsValid = false;
            }
        }

        //--------------------------------------------------------------------------------------------------------------

        public override string ToString()
        {
            return base.ToString() + $" | Duration: [{_duration:0.000}]";
        }
    }
}