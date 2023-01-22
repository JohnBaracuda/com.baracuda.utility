using System.Runtime.CompilerServices;

namespace Baracuda.Gameloop.Jobs
{
    public readonly struct UpdateJobHandle
    {
        private readonly UpdateJob _job;
        private readonly nint _jobId;

        internal UpdateJobHandle(UpdateJob job)
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

        private bool IsValid()
        {
            return _job is not null && _jobId == _job.CurrentId;
        }
    }
}