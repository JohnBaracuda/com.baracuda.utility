using System;

namespace Baracuda.Utility.PlayerLoop
{
    /// <summary>
    ///     Apply this attribute to a MonoScript to ensure that it is executed before the passed type.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ExecuteBeforeAttribute : Attribute
    {
        public Type TargetType { get; }
        public int OrderDecrease { get; set; } = 10;

        public ExecuteBeforeAttribute(Type targetType)
        {
            TargetType = targetType;
        }
    }
}