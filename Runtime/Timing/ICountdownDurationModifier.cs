namespace Baracuda.Utility.Timing
{
    public interface ICountdownDurationModifier
    {
        public void ModifyCooldownDuration(ref float totalDurationInSeconds, float unmodifiedTotalDurationInSeconds);
    }
}