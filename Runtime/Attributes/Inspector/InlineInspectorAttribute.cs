using System;
using System.Diagnostics;
using UnityEngine;

namespace Baracuda.Utilities.Inspector
{
    [Conditional("UNITY_EDITOR")]
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class InlineInspectorAttribute : PropertyAttribute
    {
    }
}