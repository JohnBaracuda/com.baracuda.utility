using System.Diagnostics;
using UnityEngine;

namespace Baracuda.Utilities.Inspector
{
    [Conditional("UNITY_EDITOR")]
    public class InlineInspectorAttribute : PropertyAttribute
    {
        public bool Simple { get; set; } = true;
    }
}