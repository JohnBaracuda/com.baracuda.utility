using System;
using System.Diagnostics;

namespace Baracuda.Utilities.Inspector
{
    public enum FoldoutName
    {
        ObjectName = 1,
        HumanizedObjectName = 2
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Method | AttributeTargets.Property)]
    [Conditional("UNITY_EDITOR")]
    public class FoldoutAttribute : Attribute
    {
        public string Title { get; }
        public bool Unfold { get; } = true;
        public FoldoutName FoldoutName { get; } = 0;
        public string ToolTip { get; }

        public FoldoutAttribute(string title, bool unfold = true, string tooltip = "")
        {
            Title = title;
            Unfold = unfold;
            ToolTip = tooltip;
        }

        public FoldoutAttribute(string title, string tooltip)
        {
            Title = title;
            ToolTip = tooltip;
        }

        public FoldoutAttribute(FoldoutName title, bool unfold = true, string tooltip = "")
        {
            FoldoutName = title;
            Unfold = unfold;
            ToolTip = tooltip;
        }

        public FoldoutAttribute(FoldoutName title, string tooltip)
        {
            FoldoutName = title;
            ToolTip = tooltip;
        }
    }
}