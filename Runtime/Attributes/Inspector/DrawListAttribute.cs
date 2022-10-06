using System;
using System.Diagnostics;
using UnityEngine;

namespace Baracuda.Utilities.Inspector
{
    [AttributeUsage(AttributeTargets.Field)]
    [Conditional("UNITY_EDITOR")]
    public class DrawListAttribute : PropertyAttribute
    {
        public bool Draggable { get; set; } = true;
        public bool DisplayHeader { get; set; } = true;
        public bool AddButton { get; set; } = true;
        public bool RemoveButton { get; set; } = true;
    }
}