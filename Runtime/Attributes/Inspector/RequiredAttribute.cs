using System;
using System.Diagnostics;
using UnityEngine;

namespace Baracuda.Utilities.Inspector
{
    [AttributeUsage(AttributeTargets.Field)]
    [Conditional("UNITY_EDITOR")]
    public class RequiredAttribute : PropertyAttribute
    {
        /// <summary>
        /// Custom message that will be displayed if the target is null
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// The message type of the displayed message
        /// </summary>
        public MessageLevel MessageLevel { get; set; }

        public RequiredAttribute(string message, MessageLevel messageLevel = MessageLevel.Error)
        {
            Message = message;
            MessageLevel = messageLevel;
        }

        public RequiredAttribute()
        {
            Message = null;
            MessageLevel = MessageLevel.Error;
        }
    }
}