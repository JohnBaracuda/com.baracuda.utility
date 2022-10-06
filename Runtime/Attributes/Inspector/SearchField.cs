using System;
using System.Diagnostics;

namespace Baracuda.Utilities.Inspector
{
    [AttributeUsage(AttributeTargets.Class)]
    [Conditional("UNITY_EDITOR")]
    public class SearchField : Attribute
    {
        public bool Enabled { get; set; } = true;
    }
}