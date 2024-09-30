using System;
using System.Diagnostics;
using UnityEngine;

namespace Baracuda.Utility.Attributes
{
    public enum SelectionPreset
    {
        LogicalDrives
    }

    [Conditional("UNITY_EDITOR")]
    public class StringSelectionAttribute : PropertyAttribute
    {
        public string[] Strings { get; }

        public StringSelectionAttribute(params string[] strings)
        {
            Strings = strings;
        }

        public StringSelectionAttribute(SelectionPreset preset)
        {
            switch (preset)
            {
                case SelectionPreset.LogicalDrives:
                    Strings = Environment.GetLogicalDrives();
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(preset), preset, null);
            }
        }

        private StringSelectionAttribute()
        {
        }
    }
}