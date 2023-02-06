using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

namespace Baracuda.Utilities.Helper
{
    public static class SerializedPropertyExtensions
    {
        /*
         * Backing Field
         */

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SerializedProperty FindPropertyRelativeBacking(this SerializedProperty serializedProperty, string path)
        {
            return serializedProperty.FindPropertyRelative($"<{path}>k__BackingField");
        }

        private const BindingFlags Flags = BindingFlags.Static |
                                           BindingFlags.NonPublic |
                                           BindingFlags.Instance |
                                           BindingFlags.Public |
                                           BindingFlags.FlattenHierarchy;

        public static TAttribute GetAttribute<TAttribute>(this SerializedProperty serializedProperty)
            where TAttribute : Attribute
        {
            return serializedProperty.GetUnderlyingFieldInfo()?.GetCustomAttribute<TAttribute>();
        }

        public static bool TryGetAttribute<TAttribute>(this SerializedProperty serializedProperty,
            out TAttribute attribute) where TAttribute : Attribute
        {
            attribute = GetAttribute<TAttribute>(serializedProperty);
            return attribute != null;
        }

        private static readonly Dictionary<SerializedProperty, Type> spTypeCache = new Dictionary<SerializedProperty, Type>();

        public static Type GetUnderlyingType(this SerializedProperty serializedProperty)
        {
            if (spTypeCache.TryGetValue(serializedProperty, out var type))
            {
                return type;
            }

            var targetType = serializedProperty.serializedObject.targetObject.GetType();

            var fieldInfo = targetType.GetFieldIncludeBaseTypes(serializedProperty.name);
            type = fieldInfo?.FieldType ?? Type.GetType(serializedProperty.type.ToFullTypeName());

            if (type != null)
            {
                spTypeCache.Add(serializedProperty, type);
                return type;
            }

            var propertyInfo = targetType.GetPropertyIncludeBaseTypes(serializedProperty.name);
            type = propertyInfo?.PropertyType;

            if (type != null)
            {
                spTypeCache.Add(serializedProperty, type);
                return type;
            }

            throw new NullReferenceException($"{serializedProperty.name} || {targetType.Name}");
        }

        public static FieldInfo GetUnderlyingFieldInfo(this SerializedProperty serializedProperty)
        {
            var targetType = serializedProperty.serializedObject.targetObject.GetType();
            return targetType.GetFieldIncludeBaseTypes(serializedProperty.name, Flags);
        }


        public static SerializedProperty[] GetSerializedProperties(this SerializedObject target)
        {
            var type = target.targetObject.GetType();
            var list = new List<SerializedProperty>();
            foreach (var fieldInfo in type.GetFieldsIncludeBaseTypes(BindingFlags.Instance | BindingFlags.NonPublic))
            {
                if (fieldInfo.HasAttribute<SerializeField>())
                {
                    list.Add(target.FindProperty(fieldInfo.Name));
                }
            }

            foreach (var fieldInfo in type.GetFieldsIncludeBaseTypes(BindingFlags.Instance | BindingFlags.Public))
            {
                if (!fieldInfo.HasAttribute<HideInInspector>())
                {
                    list.Add(target.FindProperty(fieldInfo.Name));
                }
            }

            return list.ToArray();
        }

        /*
         * Array
         */

        public static string[] GetStringArray(this SerializedProperty serializedProperty)
        {
            var array = new string[serializedProperty.arraySize];
            for (var i = 0; i < array.Length; i++)
            {
                array[i] = serializedProperty.GetArrayElementAtIndex(i).stringValue;
            }
            return array;
        }

        public static void SetStringArray(this SerializedProperty serializedProperty, string[] array)
        {
            serializedProperty.ClearArray();

            for (var i = 0; i < array.Length; i++)
            {
                serializedProperty.InsertArrayElementAtIndex(i);
                serializedProperty.GetArrayElementAtIndex(i).stringValue = array[i];
            }
        }
    }
}