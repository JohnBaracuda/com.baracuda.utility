using System.Runtime.CompilerServices;

namespace Baracuda.Gameloop.Jobs
{
    public readonly struct FrameJobHandle
    {
        private readonly FrameJob _job;
        private readonly nint _jobId;

        internal FrameJobHandle(FrameJob job)
        {
            _job = job;
            _jobId = job.CurrentId;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Cancel()
        {
            if (IsValid())
            {
                _job.Cancel();
            }
        }

        private bool IsValid()
        {
            return _job is not null && _jobId == _job.CurrentId;
        }
    }
}