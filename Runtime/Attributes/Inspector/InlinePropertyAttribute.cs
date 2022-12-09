using System;
using System.Diagnostics;

namespace Baracuda.Utilities.Inspector
{
    [Conditional("UNITY_EDITOR")]
    [AttributeUsage(AttributeTargets.Property)]
    public class InlinePropertyAttribute : Attribute
    {

    }
}