using System;
using UnityEngine;

namespace Baracuda.Utilities.Inspector
{
    public enum BoxStyle
    {
        HelpBox,
        GrayBox,
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method)]
    public class BeginBoxAttribute : PropertyAttribute
    {
        public BoxStyle Style { get; } = BoxStyle.HelpBox;

        public BeginBoxAttribute()
        {

        }

        public BeginBoxAttribute(BoxStyle style)
        {
            Style = style;
        }
    }
}