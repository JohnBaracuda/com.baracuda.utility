namespace Baracuda.Gameloop.Jobs
{
    public abstract class JobBase
    {
        internal bool IsValid { get; set; }
        internal abstract void Update(float delta);
        internal nint CurrentId { get; set; }

        private static nint nextJobId;

        internal void IncrementId()
        {
            CurrentId = nextJobId++;
        }

        public override string ToString()
        {
            return $"Valid: [{IsValid.ToString()}] | ID: [{CurrentId.ToString()}] | Type: [{GetType().Name}]";
        }
    }
}