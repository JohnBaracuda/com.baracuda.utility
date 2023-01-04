using System;
using System.Diagnostics;
using UnityEngine;

namespace Baracuda.Utilities.Inspector
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    [Conditional("UNITY_EDITOR")]
    public class ReadonlyAttribute : PropertyAttribute
    {
    }
}