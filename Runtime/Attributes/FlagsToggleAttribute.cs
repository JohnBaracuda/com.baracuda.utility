using System;
using System.Diagnostics;
using UnityEngine;

namespace Baracuda.Utility.Attributes
{
    [Conditional("UNITY_EDITOR")]
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method)]
    public class FlagsToggleAttribute : PropertyAttribute
    {
    }
}