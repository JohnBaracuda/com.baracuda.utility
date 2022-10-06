using System;
using System.Diagnostics;

namespace Baracuda.Utilities.Inspector
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method)]
    [Conditional("UNITY_EDITOR")]
    public class ConditionalShowAttribute : ConditionalDrawerAttribute
    {
        public ConditionalShowAttribute(string member, object condition) : base(member, condition, false)
        {
        }

        public ConditionalShowAttribute(string member, bool condition = true) : base(member, condition, false)
        {
        }
    }
}