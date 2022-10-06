using System;
using System.Diagnostics;
using UnityEngine;

namespace Baracuda.Utilities.Inspector
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    [Conditional("UNITY_EDITOR")]
    public class LabelAttribute : PropertyAttribute
    {
        public string Label { get; }
        
        public LabelAttribute(string label)
        {
            Label = label ?? throw new ArgumentNullException(nameof(label));
        }
    }
}