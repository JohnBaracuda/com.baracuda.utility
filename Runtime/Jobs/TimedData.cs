using Baracuda.Utilities.Jobs;

namespace Baracuda.Gameloop.Jobs
{
    public readonly ref struct TimedData
    {
        public readonly TimedJob Job;
        public readonly float DeltaTime;
        public readonly float Progress;

        public TimedData(TimedJob job, float deltaTime, float progress)
        {
            Job = job;
            DeltaTime = deltaTime;
            Progress = progress;
        }
        
        public void SetCompleted()
        {
            Job.Stop();
        }
        
        public void SetCompletedIf(bool condition)
        {
            if (condition)
            {
                Job.Stop();
            }
        }
        
        public void SoftReset()
        {
            Job.SoftReset();
        }
        
        public void SoftResetIf(bool condition)
        {
            if (condition)
            {
                Job.SoftReset();
            }
        }
    }
}