using System;

namespace Baracuda.Utilities.Inspector
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class SpaceBeforeAttribute : UnityEngine.SpaceAttribute
    {
        public SpaceBeforeAttribute() : base(8)
        {
        }

        public SpaceBeforeAttribute(float height) : base(height)
        {
        }
    }
}