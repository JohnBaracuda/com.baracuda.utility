using System;
using System.Runtime.CompilerServices;

namespace Baracuda.Gameloop.Jobs
{
    public delegate void UpdateJobDelegate(UpdateData handle);
    
    public class UpdateJob : JobBase
    {
        /*
         * Fields   
         */
        
        private UpdateJobDelegate _onUpdate;
        private Action _onStop;
        private float _runTime;

        /*
         * Factory   
         */
        
        public static UpdateJobHandle Run(UpdateJobDelegate job, Action onStop = null)
        {
            var updateJob = UpdateJobPool.Get();
            updateJob._onUpdate = job;
            updateJob._onStop = onStop;
            updateJob._runTime = 0;
            updateJob.IsValid = true;
            JobManager.RegisterJob(updateJob);
            return new UpdateJobHandle(updateJob);
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
            _onUpdate(new UpdateData(this, delta, _runTime += delta));
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
                UpdateJobPool.Release(this);
                IsValid = false;
            }
        }

        internal void SoftReset()
        {
            _runTime = 0;
        }
    }
}