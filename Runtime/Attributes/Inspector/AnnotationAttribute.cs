using System;
using System.Diagnostics;
using UnityEngine;

namespace Baracuda.Utilities.Inspector
{
    public enum MessageTypeValue
    {
        None,
        Info,
        Warning,
        Error,
    }
    
    [Conditional("UNITY_EDITOR")]
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method)]
    public class AnnotationAttribute : PropertyAttribute
    {
        public string Annotation { get; }
        public MessageTypeValue MessageType { get; }

        public AnnotationAttribute(string annotation, MessageTypeValue messageType = MessageTypeValue.Info)
        {
            Annotation = annotation;
            MessageType = messageType;
        }
    }
}