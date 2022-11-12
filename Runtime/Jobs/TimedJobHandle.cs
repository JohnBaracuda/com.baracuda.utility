using Baracuda.Utilities.Jobs;
using System.Runtime.CompilerServices;

namespace Baracuda.Gameloop.Jobs
{
    public readonly struct TimedJobHandle
    {
        private readonly TimedJob _job;
        private readonly nint _jobId;

        internal TimedJobHandle(TimedJob job)
        {
            _job = job;
            _jobId = job.CurrentId;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Stop()
        {
            if (IsValid())
            {
                _job.Stop();
            }
        }

        public bool SoftReset()
        {
            if (!IsValid())
            {
                return false;
            }

            _job.SoftReset();
            return true;
        }

        private bool IsValid()
        {
            return _job is not null && _job.IsValid && _jobId == _job.CurrentId;
        }
    }
}