using System;

namespace Baracuda.Utilities.Inspector
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class DrawSpaceAttribute : UnityEngine.SpaceAttribute
    {
        public DrawSpaceAttribute() : base(8)
        {
        }

        public DrawSpaceAttribute(float height) : base(height)
        {
        }
    }
}