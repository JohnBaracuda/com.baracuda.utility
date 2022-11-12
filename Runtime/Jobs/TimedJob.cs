using Baracuda.Gameloop.Jobs;
using System;
using System.Runtime.CompilerServices;
using UnityEngine.Assertions;

namespace Baracuda.Utilities.Jobs
{
    public delegate void TimedJobDelegate(in TimedData handle);

    public class TimedJob : JobBase
    {
        /*
         * Fields
         */

        private TimedJobDelegate _onUpdate;
        private Action _onStop;
        private float _runTime;
        private float _progress;
        private float _secondsDuration;

        /*
         * Factory
         */

        public static TimedJobHandle Run(TimedJobDelegate job, float secondsDuration, Action onStop = null)
        {
            var timedJob = TimedJobPool.Get();
            timedJob._onUpdate = job;
            timedJob._onStop = onStop;
            timedJob._runTime = 0;
            timedJob._progress = 0;
            timedJob._secondsDuration = secondsDuration;
            timedJob.IsValid = true;
            JobManager.RegisterJob(timedJob);
            return new TimedJobHandle(timedJob);
        }

        /*
         * Public
         */

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Stop()
        {
            Reset();
        }

        //--------------------------------------------------------------------------------------------------------------

        /*
         * Internal
         */

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal override void Update(float delta)
        {
            _runTime += delta;
            _progress = _runTime / _secondsDuration;
            Assert.IsNotNull(_onUpdate);
            _onUpdate(new TimedData(this, delta, _progress));
            if (_progress >= 1)
            {
                Stop();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Reset()
        {
            if (IsValid)
            {
                _onStop?.Invoke();
                _onStop = null;
                _onUpdate = null;
                JobManager.UnregisterJob(this);
                TimedJobPool.Release(this);
                IsValid = false;
            }
        }

        internal void SoftReset()
        {
            _runTime = 0;
        }


        //--------------------------------------------------------------------------------------------------------------

        public override string ToString()
        {
            return base.ToString() + $" | Completed: [{_progress:0.000}]";
        }
    }
}