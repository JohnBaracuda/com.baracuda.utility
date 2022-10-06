using System;
using System.Diagnostics;

namespace Baracuda.Utilities.Inspector
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method)]
    [Conditional("UNITY_EDITOR")]
    public class ConditionalHideAttribute : ConditionalDrawerAttribute
    {
        public ConditionalHideAttribute(string member, object condition) : base(member, condition, true)
        {
        }

        public ConditionalHideAttribute(string member, bool condition = true) : base(member, condition, true)
        {
        }
    }
}