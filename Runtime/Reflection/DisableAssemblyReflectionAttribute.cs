// Copyright (c) 2022 Jonathan Lang

using Baracuda.Utilities.Reflection;
using System;

[assembly: DisableAssemblyReflection]
namespace Baracuda.Utilities.Reflection
{
    /// <summary>
    /// Disable reflection for the target assembly or class.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct)]
    public class DisableAssemblyReflectionAttribute : Attribute
    {
    }
}
