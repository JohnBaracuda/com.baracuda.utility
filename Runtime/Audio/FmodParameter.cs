using FMOD.Studio;

namespace Baracuda.Utility.Audio
{
    public readonly struct FmodParameter
    {
        public readonly PARAMETER_ID Id;
        public readonly float Value;

        public FmodParameter(PARAMETER_ID name, float value)
        {
            Id = name;
            Value = value;
        }

        public FmodParameter WithValue(float value)
        {
            return new FmodParameter(Id, value);
        }
    }
}