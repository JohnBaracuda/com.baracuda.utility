using System;
using UnityEngine;

namespace Baracuda.Utilities.Reflection
{
    /// <summary>
    /// Set the <see cref="HideFlags"/> fot the target <see cref="GameObject"/> and/or <see cref="MonoBehaviour"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class HideFlagsAttribute : Attribute
    {
        /*
         *  Properties   
         */

        /// <summary>
        /// Set the <see cref="HideFlags"/> for the targets <see cref="GameObject"/>
        /// </summary>
        public HideFlags ObjectFlags
        {
            get => InternalObjectFlags ?? HideFlags.None; 
            set => InternalObjectFlags = value;
        }
        
        /// <summary>
        /// Set the <see cref="HideFlags"/> for the target <see cref="MonoBehaviour"/> inspector script.
        /// </summary>
        public HideFlags ScriptFlags
        {
            get => InternalScriptFlags ?? HideFlags.None; 
            set => InternalScriptFlags = value;
        }

        /*
         *  Internal   
         */

        public HideFlags? InternalObjectFlags { get; private set; } = null;
        public HideFlags? InternalScriptFlags { get; private set; }= null;

        /*
         *  Ctor   
         */

        public HideFlagsAttribute(HideFlags objectFlags)
        {
            InternalObjectFlags = objectFlags;
        }
        
        public HideFlagsAttribute()
        {
        }
    }
    
    /// <summary>
    /// Set the <see cref="HideFlags"/> fot the target <see cref="GameObject"/> to <see cref="HideFlags.HideInHierarchy"/>
    /// </summary>
    public class HideGameObjectAttribute : HideFlagsAttribute
    {
        /// <summary>
        /// Set the <see cref="HideFlags"/> fot the target <see cref="GameObject"/> to <see cref="HideFlags.HideInHierarchy"/>
        /// </summary>
        public HideGameObjectAttribute() : base(HideFlags.HideInHierarchy)
        {
        }
    }
    
    /// <summary>
    /// Set the <see cref="HideFlags"/> fot the target <see cref="MonoBehaviour"/> (inspector) to <see cref="HideFlags.HideInInspector"/>
    /// </summary>
    public class HideScriptAttribute : HideFlagsAttribute
    {
        /// <summary>
        /// Set the <see cref="HideFlags"/> fot the target <see cref="MonoBehaviour"/> (inspector) to <see cref="HideFlags.HideInInspector"/>
        /// </summary>
        public HideScriptAttribute()
        {
            ScriptFlags = HideFlags.HideInInspector;
        }
    }
}