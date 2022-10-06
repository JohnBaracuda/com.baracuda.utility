using System;
using System.Diagnostics;
using UnityEngine;

namespace Baracuda.Utilities.Inspector
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = false)]
    [Conditional("UNITY_EDITOR")]
    public abstract class ConditionalDrawerAttribute : PropertyAttribute
    {
        public string Member { get; }

        /// <summary>
        /// The condition that must be met. (If the two values are equal)
        /// </summary>
        public object Condition { get; }

        /// <summary>
        /// When enabled, the condition will be negated.
        /// </summary>
        public bool NegateCondition { get; }

        /// <summary>
        /// When enabled, the property will be displayed as readonly if the condition is not met.
        /// </summary>
        public bool ReadOnly { get; set; } = false;

        private protected ConditionalDrawerAttribute(string member, object condition, bool negateCondition)
        {
            Member = member;
            Condition = condition;
            NegateCondition = negateCondition;
        }
    }
}
