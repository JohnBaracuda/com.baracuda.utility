using System.Diagnostics;
using UnityEngine;

namespace Baracuda.Utilities.Inspector
{
    [Conditional("UNITY_EDITOR")]
    public class DrawFolderPathAttribute : PropertyAttribute
    {
        /// <summary>
        /// The width of the select button.
        /// </summary>
        public int ButtonWidth { get; set; } = 23;

        /// <summary>
        /// When enabled the file path string can be edited like a string.
        /// </summary>
        public bool EnableDirectEditing { get; set; } = true;

        public DrawFolderPathAttribute()
        {

        }
    }
}