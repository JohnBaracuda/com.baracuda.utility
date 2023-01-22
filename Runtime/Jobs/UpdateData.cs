namespace Baracuda.Gameloop.Jobs
{
    public readonly ref struct UpdateData
    {
        public readonly UpdateJob Job;
        public readonly float DeltaTime;
        public readonly float Runtime;

        public UpdateData(UpdateJob job, float deltaTime, float runtime)
        {
            Job = job;
            DeltaTime = deltaTime;
            Runtime = runtime;
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