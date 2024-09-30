using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using Baracuda.Utility.Utilities;
using UnityEngine;
using UnityEngine.Pool;
using Object = UnityEngine.Object;

namespace Baracuda.Utility.Reflection
{
    public static partial class ReflectionExtensions
    {
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string[] GetNamesOfGenericInterfaceSubtypesInObject(this Object target,
            Type interfaceType)
        {
            if (!interfaceType.IsGenericType)
            {
                return Array.Empty<string>();
            }

            if (target.IsNull())
            {
                return Array.Empty<string>();
            }

            var cache = ListPool<string>.Get();

            if (target is GameObject gameObject)
            {
                var components = gameObject.GetComponents<MonoBehaviour>();
                for (var i = 0; i < components.Length; i++)
                {
                    cache.AddRange(GetNamesOfGenericInterfaceSubtypesInObject(components[i], interfaceType));
                }
            }
            else
            {
                var interfaces = target.GetType().GetInterfaces();
                for (var i = 0; i < interfaces.Length; i++)
                {
                    if (interfaces[i].IsGenericType)
                    {
                        continue;
                    }

                    if (interfaces[i].HasInterfaceWithGenericTypeDefinition(interfaceType))
                    {
                        cache.Add(interfaces[i].HumanizedName());
                    }
                }
            }

            var result = cache.ToArray();
            ListPool<string>.Release(cache);
            return result;
        }
    }
}