using Baracuda.Utilities.Pooling;
using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Baracuda.Utilities
{
    public static partial class ReflectionExtensions
    {
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string[] GetNamesOfGenericInterfaceSubtypesInObject(this UnityEngine.Object target, Type interfaceType)
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
                    //TODO: make this an option in settings
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