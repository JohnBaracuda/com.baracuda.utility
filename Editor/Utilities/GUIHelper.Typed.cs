using System;
using System.Collections;
using Baracuda.Bedrock.Reflection;
using Baracuda.Bedrock.Utilities;
using JetBrains.Annotations;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Baracuda.Utilities.Editor.Utilities
{
    public static partial class GUIUtility
    {
        public static Action<object> CreateDrawer(GUIContent label, [NotNull] Type type,
            params GUILayoutOption[] options)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (type == typeof(bool))
            {
                return value => UnityEditor.EditorGUILayout.Toggle(label, (bool)value, options);
            }

            if (type == typeof(byte))
            {
                return value => UnityEditor.EditorGUILayout.IntField(label, (byte)value, options);
            }

            if (type == typeof(char))
            {
                return value => UnityEditor.EditorGUILayout.TextField(label, ((char)value).ToString(), options);
            }

            if (type == typeof(decimal))
            {
                return value => UnityEditor.EditorGUILayout.FloatField(label, (float)(decimal)value, options);
            }

            if (type == typeof(double))
            {
                return value => UnityEditor.EditorGUILayout.DoubleField(label, (double)value, options);
            }

            if (type == typeof(short))
            {
                return value => UnityEditor.EditorGUILayout.IntField(label, (short)value, options);
            }

            if (type == typeof(int))
            {
                return value => UnityEditor.EditorGUILayout.IntField(label, (int)value, options);
            }

            if (type == typeof(long))
            {
                return value => UnityEditor.EditorGUILayout.LongField(label, (long)value, options);
            }

            if (type == typeof(sbyte))
            {
                return value => UnityEditor.EditorGUILayout.IntField(label, (sbyte)value, options);
            }

            if (type == typeof(float))
            {
                return value => UnityEditor.EditorGUILayout.FloatField(label, (float)value, options);
            }

            if (type == typeof(string))
            {
                return value => UnityEditor.EditorGUILayout.TextField(label, (string)value, options);
            }

            if (type == typeof(ushort))
            {
                return value => UnityEditor.EditorGUILayout.IntField(label, (ushort)value, options);
            }

            if (type == typeof(uint))
            {
                return value => UnityEditor.EditorGUILayout.IntField(label, (int)(uint)value, options);
            }

            if (type == typeof(ulong))
            {
                return value => UnityEditor.EditorGUILayout.LongField(label, (long)(ulong)value, options);
            }

            if (type == typeof(Vector2))
            {
                return value => UnityEditor.EditorGUILayout.Vector2Field(label, (Vector2)value, options);
            }

            if (type == typeof(Vector3))
            {
                return value => UnityEditor.EditorGUILayout.Vector3Field(label, (Vector3)value, options);
            }

            if (type == typeof(Vector4))
            {
                return value => UnityEditor.EditorGUILayout.Vector4Field(label, (Vector4)value, options);
            }

            if (type == typeof(Vector2Int))
            {
                return value => UnityEditor.EditorGUILayout.Vector2IntField(label, (Vector2Int)value, options);
            }

            if (type == typeof(Vector3Int))
            {
                return value => UnityEditor.EditorGUILayout.Vector3IntField(label, (Vector3Int)value, options);
            }

            if (type == typeof(Quaternion))
            {
                return value =>
                    UnityEditor.EditorGUILayout.Vector4Field(label, ((Quaternion)value).ToVector4(), options);
            }

            if (type == typeof(Color))
            {
                return value => UnityEditor.EditorGUILayout.ColorField(label, (Color)value, options);
            }

            if (type == typeof(Color32))
            {
                return value => UnityEditor.EditorGUILayout.ColorField(label, (Color32)value, options);
            }

            if (type == typeof(Rect))
            {
                return value => UnityEditor.EditorGUILayout.RectField(label, (Rect)value, options);
            }

            if (type == typeof(AnimationCurve))
            {
                return value => UnityEditor.EditorGUILayout.CurveField(label, (AnimationCurve)value, options);
            }

            if (type.IsAssignableFrom(typeof(Object)))
            {
                return value => UnityEditor.EditorGUILayout.ObjectField(label, (Object)value, type, true, options);
            }

            if (type.IsGenericIEnumerable())
            {
                var elementType = type.GetElementTypeDefinition();
                var elementDrawer = CreateDrawer(GUIContent.none, elementType);
                return value => DrawBoxedIEnumerable(label, type, value, elementDrawer);
            }

            return value => UnityEditor.EditorGUILayout.TextField(label, $"{value.ToNullString()}", options);
        }

        private static void DrawBoxedIEnumerable(GUIContent label, Type type, object boxedValue,
            Action<object> elementDrawer)
        {
            BeginBox();
            try
            {
                if (boxedValue != null)
                {
                    // ReSharper disable PossibleMultipleEnumeration
                    var enumerable = (IEnumerable)boxedValue;
                    var isEmpty = !enumerable.GetEnumerator().MoveNext();

                    if (!isEmpty)
                    {
                        UnityEditor.EditorGUILayout.LabelField(label);
                    }

                    foreach (var element in enumerable)
                    {
                        elementDrawer(element);
                    }

                    if (isEmpty)
                    {
                        UnityEditor.EditorGUILayout.LabelField($"{type.HumanizedName()} is empty");
                    }
                }
            }
            catch (Exception exception)
            {
                DrawException(exception);
            }

            EndBox();
        }

        public static Func<object, object> CreateEditor(GUIContent label, [NotNull] Type type,
            params GUILayoutOption[] options)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (type == typeof(bool))
            {
                return value => UnityEditor.EditorGUILayout.Toggle(label, (bool)value, options);
            }

            if (type == typeof(byte))
            {
                return value => UnityEditor.EditorGUILayout.IntField(label, (byte)value, options);
            }

            if (type == typeof(char))
            {
                return value => UnityEditor.EditorGUILayout.TextField(label, ((char)value).ToString(), options);
            }

            if (type == typeof(decimal))
            {
                return value => UnityEditor.EditorGUILayout.FloatField(label, (float)(decimal)value, options);
            }

            if (type == typeof(double))
            {
                return value => UnityEditor.EditorGUILayout.DoubleField(label, (double)value, options);
            }

            if (type == typeof(short))
            {
                return value => UnityEditor.EditorGUILayout.IntField(label, (short)value, options);
            }

            if (type == typeof(int))
            {
                return value => UnityEditor.EditorGUILayout.IntField(label, (int)value, options);
            }

            if (type == typeof(long))
            {
                return value => UnityEditor.EditorGUILayout.LongField(label, (long)value, options);
            }

            if (type == typeof(sbyte))
            {
                return value => UnityEditor.EditorGUILayout.IntField(label, (sbyte)value, options);
            }

            if (type == typeof(float))
            {
                return value => UnityEditor.EditorGUILayout.FloatField(label, (float)value, options);
            }

            if (type == typeof(string))
            {
                return value => UnityEditor.EditorGUILayout.TextField(label, (string)value, options);
            }

            if (type == typeof(ushort))
            {
                return value => UnityEditor.EditorGUILayout.IntField(label, (ushort)value, options);
            }

            if (type == typeof(uint))
            {
                return value => UnityEditor.EditorGUILayout.IntField(label, (int)(uint)value, options);
            }

            if (type == typeof(ulong))
            {
                return value => UnityEditor.EditorGUILayout.LongField(label, long.Parse(value.ToString()), options);
            }

            if (type == typeof(Vector2))
            {
                return value => UnityEditor.EditorGUILayout.Vector2Field(label, (Vector2)value, options);
            }

            if (type == typeof(Vector3))
            {
                return value => UnityEditor.EditorGUILayout.Vector3Field(label, (Vector3)value, options);
            }

            if (type == typeof(Vector4))
            {
                return value => UnityEditor.EditorGUILayout.Vector4Field(label, (Vector4)value, options);
            }

            if (type == typeof(Vector2Int))
            {
                return value => UnityEditor.EditorGUILayout.Vector2IntField(label, (Vector2Int)value, options);
            }

            if (type == typeof(Vector3Int))
            {
                return value => UnityEditor.EditorGUILayout.Vector3IntField(label, (Vector3Int)value, options);
            }

            if (type == typeof(Quaternion))
            {
                return value =>
                    UnityEditor.EditorGUILayout.Vector4Field(label, ((Quaternion)value).ToVector4(), options);
            }

            if (type == typeof(Color))
            {
                return value => UnityEditor.EditorGUILayout.ColorField(label, (Color)value, options);
            }

            if (type == typeof(Color32))
            {
                return value => UnityEditor.EditorGUILayout.ColorField(label, (Color32)value, options);
            }

            if (type == typeof(Rect))
            {
                return value => UnityEditor.EditorGUILayout.RectField(label, (Rect)value, options);
            }

            if (type == typeof(AnimationCurve))
            {
                return value => UnityEditor.EditorGUILayout.CurveField(label, (AnimationCurve)value, options);
            }

            if (type.IsAssignableFrom(typeof(Object)))
            {
                return value => UnityEditor.EditorGUILayout.ObjectField(label, (Object)value, type, true, options);
            }

            if (type.IsEnum)
            {
                return value => UnityEditor.EditorGUILayout.EnumPopup(label, (Enum)value, options);
            }

            var drawer = CreateDrawer(label, type, options);

            return value =>
            {
                BeginEnabledOverride(false);
                drawer(value);
                EndEnabledOverride();
                return value;
            };
        }
    }
}