using System.Runtime.CompilerServices;

namespace Baracuda.Gameloop.Jobs
{
    public readonly struct WaitJobHandle
    {
        private readonly WaitJob _job;
        private readonly nint _jobId;

        internal WaitJobHandle(WaitJob job)
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Complete()
        {
            if (IsValid())
            {
                _job.Complete();
            }
        }

        private bool IsValid()
        {
            return _job is not null && _jobId == _job.CurrentId;
        }
    }
}