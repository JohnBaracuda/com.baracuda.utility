using System;
using System.Linq;
using UnityEngine;

namespace Baracuda.Utilities.Types
{
    [Serializable]
    public class SerializableType : ISerializationCallbackReceiver
    {
        [SerializeField] private string assemblyQualifiedName = string.Empty;

        public Type Type { get; private set; }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            assemblyQualifiedName = Type?.AssemblyQualifiedName ?? assemblyQualifiedName;
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            if (!TryGetType(assemblyQualifiedName, out var type))
            {
                Debug.LogError($"Type {assemblyQualifiedName} not found");
                return;
            }
            Type = type;
        }

        private static bool TryGetType(string typeString, out Type type)
        {
            type = Type.GetType(typeString);
            return type != null || !string.IsNullOrEmpty(typeString);
        }

        // Implicit conversion from SerializableType to Type
        public static implicit operator Type(SerializableType serializableType)
        {
            return serializableType.Type;
        }

        // Implicit conversion from Type to SerializableType
        public static implicit operator SerializableType(Type type)
        {
            return new SerializableType {Type = type};
        }
    }

    public static class TypeExtensions
    {
        /// <summary>
        ///     Checks if a given type inherits or implements a specified base type.
        /// </summary>
        /// <param name="type">The type which needs to be checked.</param>
        /// <param name="baseType">The base type/interface which is expected to be inherited or implemented by the 'type'</param>
        /// <returns>Return true if 'type' inherits or implements 'baseType'. False otherwise</returns>
        public static bool InheritsOrImplements(this Type type, Type baseType)
        {
            type = ResolveGenericType(type);
            baseType = ResolveGenericType(baseType);

            while (type != typeof(object))
            {
                if (baseType == type || HasAnyInterfaces(type, baseType))
                {
                    return true;
                }

                type = ResolveGenericType(type.BaseType);
                if (type == null)
                {
                    return false;
                }
            }

            return false;
        }

        private static Type ResolveGenericType(Type type)
        {
            if (type is not {IsGenericType: true})
            {
                return type;
            }

            var genericType = type.GetGenericTypeDefinition();
            return genericType != type ? genericType : type;
        }

        private static bool HasAnyInterfaces(Type type, Type intefaceType)
        {
            return type.GetInterfaces().Any(i => ResolveGenericType(i) == intefaceType);
        }
    }
}