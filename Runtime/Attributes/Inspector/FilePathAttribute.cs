using System.Diagnostics;
using UnityEngine;

namespace Baracuda.Utilities.Inspector
{
    [Conditional("UNITY_EDITOR")]
    public class DrawFilePathAttribute : PropertyAttribute
    {
        /// <summary>
        /// The width of the select button.
        /// </summary>
        public int ButtonWidth { get; set; } = 23;

        /// <summary>
        /// When enabled the file path string can be edited like a string.
        /// </summary>
        public bool Readonly { get; set; } = false;

        /// <summary>
        /// Optional valid file extension.
        /// </summary>
        public string FileExtension { get; } = string.Empty;

        public DrawFilePathAttribute()
        {
        }

        public DrawFilePathAttribute(string fileExtension)
        {
            FileExtension = fileExtension;
        }
    }
}