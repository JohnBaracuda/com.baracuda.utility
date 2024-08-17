// Copyright (c) 2022 Jonathan Lang

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Baracuda.Bedrock.Reflection
{
    public static class AssemblyProfiler
    {
        private static readonly string[] bannedAssemblyPrefixes =
        {
            "Newtonsoft",
            "netstandard",
            "System",
            "Unity",
            "Microsoft",
            "Mono.",
            "mscorlib",
            "NSubstitute",
            "nunit.",
            "JetBrains",
            "GeNa."
        };

        private static readonly string[] bannedAssemblyNames =
        {
            "mcs",
            "AssetStoreTools",
            "PPv2URPConverters"
        };

        /// <summary>
        ///     Method will initialize and filter all available assemblies only leaving custom assemblies.
        ///     Precompiled unity and system assemblies as well as some other known assemblies will be excluded by default.
        /// </summary>
        /// <param name="excludeNames">Custom array of names of assemblies that should be excluded from the result</param>
        /// <param name="excludePrefixes">Custom array of prefixes for names of assemblies that should be excluded from the result</param>
        public static Assembly[] GetFilteredAssemblies(string[] excludeNames = null,
            string[] excludePrefixes = null)
        {
            return GetFilteredAssembliesInternal(excludeNames ?? Array.Empty<string>(),
                excludePrefixes ?? Array.Empty<string>());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Assembly[] GetFilteredAssembliesInternal(string[] excludeNames, string[] excludePrefixes)
        {
            if (excludeNames == null)
            {
                throw new ArgumentNullException(nameof(excludeNames));
            }

            if (excludePrefixes == null)
            {
                throw new ArgumentNullException(nameof(excludePrefixes));
            }

            var filteredAssemblies = new List<Assembly>(64);
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            for (var i = 0; i < assemblies.Length; i++)
            {
                var assembly = assemblies[i];

                if (assembly.IsAssemblyValid(excludeNames, excludePrefixes))
                {
                    filteredAssemblies.Add(assemblies[i]);
                }
            }

            return filteredAssemblies.ToArray();
        }

        private static bool IsAssemblyValid(this Assembly assembly, IReadOnlyList<string> excludeNames,
            IReadOnlyList<string> excludePrefixes)
        {
            var assemblyFullName = assembly.FullName;
            foreach (var prefix in bannedAssemblyPrefixes)
            {
                if (!string.IsNullOrWhiteSpace(prefix) && assemblyFullName.StartsWith(prefix))
                {
                    return false;
                }
            }

            foreach (var prefix in excludePrefixes)
            {
                if (!string.IsNullOrWhiteSpace(prefix) && assemblyFullName.StartsWith(prefix))
                {
                    return false;
                }
            }

            var assemblyShortName = assembly.GetName().Name;
            foreach (var name in bannedAssemblyNames)
            {
                if (assemblyShortName == name)
                {
                    return false;
                }
            }

            foreach (var name in excludeNames)
            {
                if (assemblyShortName == name)
                {
                    return false;
                }
            }

            return true;
        }
    }
}