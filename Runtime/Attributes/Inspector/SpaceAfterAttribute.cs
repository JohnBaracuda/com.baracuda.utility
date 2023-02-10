using System;
using UnityEngine;

namespace Baracuda.Utilities.Inspector
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class SpaceAfterAttribute : PropertyAttribute
    {
        /// <summary>
        ///   <para>The spacing in pixels.</para>
        /// </summary>
        public readonly float Height;

        public SpaceAfterAttribute() => this.Height = 8f;

        /// <summary>
        ///   <para>Use this DecoratorDrawer to add some spacing in the Inspector.</para>
        /// </summary>
        public SpaceAfterAttribute(float height) => Height = height;
    }


    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method)]
    public class BeginBoxAttribute : PropertyAttribute
    {

    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method)]
    public class EndBoxAttribute : PropertyAttribute
    {

    }
}