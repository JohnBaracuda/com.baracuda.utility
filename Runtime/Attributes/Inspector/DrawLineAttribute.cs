using System;
using System.Diagnostics;
using UnityEngine;

namespace Baracuda.Utilities.Inspector
{
    [Conditional("UNITY_EDITOR")]
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method)]
    public class DrawLineAttribute : PropertyAttribute
    {
        public int SpaceBefore { get; set; } = 3;
        public int SpaceAfter { get; set; } = 3;
    }
}