using System;
using System.Diagnostics;
using JetBrains.Annotations;

namespace Baracuda.Utilities.Inspector
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    [Conditional("UNITY_EDITOR")]
    [MeansImplicitUse]
    public class ShowInInspectorAttribute : Attribute
    {
        public bool Box { get; set; } = false;
    }
}