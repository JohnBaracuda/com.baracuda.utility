using System;

namespace Baracuda.Bedrock.PlayerLoop
{
    /// <summary>
    ///     Apply this attribute to a MonoScript to ensure that it is executed after the passed type.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ExecuteAfterAttribute : Attribute
    {
        public Type TargetType { get; }
        public uint OrderIncrease { get; set; } = 10;

        public ExecuteAfterAttribute(Type targetType)
        {
            TargetType = targetType;
        }
    }
}