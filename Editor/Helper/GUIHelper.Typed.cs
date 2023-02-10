using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Baracuda.Utilities.Helper
{
    public static partial class GUIHelper
    {
        public static Action<object> CreateDrawer(GUIContent label, [NotNull] Type type, params GUILayoutOption[] options)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (type == typeof(bool))
            {
                return value => EditorGUILayout.Toggle(label, (bool) value, options);
            }

            if (type == typeof(byte))
            {
                return value => EditorGUILayout.IntField(label, (byte) value, options);
            }

            if (type == typeof(char))
            {
                return value => EditorGUILayout.TextField(label, ((char) value).ToString(), options);
            }

            if (type == typeof(decimal))
            {
                return value => EditorGUILayout.FloatField(label, (float) (decimal) value, options);
            }

            if (type == typeof(double))
            {
                return value => EditorGUILayout.DoubleField(label, (double) value, options);
            }

            if (type == typeof(short))
            {
                return value => EditorGUILayout.IntField(label, (short) value, options);
            }

            if (type == typeof(int))
            {
                return value => EditorGUILayout.IntField(label, (int) value, options);
            }

            if (type == typeof(long))
            {
                return value => EditorGUILayout.LongField(label, (long) value, options);
            }

            if (type == typeof(sbyte))
            {
                return value => EditorGUILayout.IntField(label, (sbyte) value, options);
            }

            if (type == typeof(float))
            {
                return value => EditorGUILayout.FloatField(label, (float) value, options);
            }

            if (type == typeof(string))
            {
                return value => EditorGUILayout.TextField(label, (string) value, options);
            }

            if (type == typeof(ushort))
            {
                return value => EditorGUILayout.IntField(label, (ushort) value, options);
            }

            if (type == typeof(uint))
            {
                return value => EditorGUILayout.IntField(label, (int) (uint) value, options);
            }

            if (type == typeof(ulong))
            {
                return value => EditorGUILayout.LongField(label, (long) (ulong) value, options);
            }

            if (type == typeof(Vector2))
            {
                return value => EditorGUILayout.Vector2Field(label, (Vector2) value, options);
            }

            if (type == typeof(Vector3))
            {
                return value => EditorGUILayout.Vector3Field(label, (Vector3) value, options);
            }

            if (type == typeof(Vector4))
            {
                return value => EditorGUILayout.Vector4Field(label, (Vector4) value, options);
            }

            if (type == typeof(Vector2Int))
            {
                return value => EditorGUILayout.Vector2IntField(label, (Vector2Int) value, options);
            }

            if (type == typeof(Vector3Int))
            {
                return value => EditorGUILayout.Vector3IntField(label, (Vector3Int) value, options);
            }

            if (type == typeof(Quaternion))
            {
                return value => EditorGUILayout.Vector4Field(label, ((Quaternion) value).ToVector4(), options);
            }

            if (type == typeof(Color))
            {
                return value => EditorGUILayout.ColorField(label, (Color) value, options);
            }

            if (type == typeof(Color32))
            {
                return value => EditorGUILayout.ColorField(label, (Color32) value, options);
            }

            if (type == typeof(Rect))
            {
                return value => EditorGUILayout.RectField(label, (Rect) value, options);
            }

            if (type == typeof(AnimationCurve))
            {
                return value => EditorGUILayout.CurveField(label, (AnimationCurve) value, options);
            }

            if (type.IsAssignableFrom(typeof(UnityEngine.Object)))
            {
                return value => EditorGUILayout.ObjectField(label, (UnityEngine.Object) value, type, true, options);
            }

            if (type.IsGenericIEnumerable())
            {
                var elementType = type.GetElementTypeDefinition();
                var elementDrawer = CreateDrawer(GUIContent.none, elementType);
                return value => DrawBoxedIEnumerable(label, type, value, elementDrawer);
            }

            return value => EditorGUILayout.TextField(label, $"{value.ToNullString()}", options);
        }

        private static void DrawBoxedIEnumerable(GUIContent label, Type type, object boxedValue, Action<object> elementDrawer)
        {
            BeginBox();
            try
            {
                // ReSharper disable PossibleMultipleEnumeration
                var enumerable = (IEnumerable) boxedValue;
                var isEmpty = !enumerable.GetEnumerator().MoveNext();

                if (!isEmpty)
                {
                    EditorGUILayout.LabelField(label);
                }
                foreach (var element in enumerable)
                {
                    elementDrawer(element);
                }
                if (isEmpty)
                {
                    EditorGUILayout.LabelField($"{type.HumanizedName()} is empty");
                }
            }
            catch (Exception exception)
            {
                DrawException(exception);
            }

            EndBox();
        }


        public static Func<object, object> CreateEditor(GUIContent label, [NotNull] Type type, params GUILayoutOption[] options)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (type == typeof(bool))
            {
                return value => EditorGUILayout.Toggle(label, (bool) value, options);
            }

            if (type == typeof(byte))
            {
                return value => EditorGUILayout.IntField(label, (byte) value, options);
            }

            if (type == typeof(char))
            {
                return value => EditorGUILayout.TextField(label, ((char) value).ToString(), options);
            }

            if (type == typeof(decimal))
            {
                return value => EditorGUILayout.FloatField(label, (float) (decimal) value, options);
            }

            if (type == typeof(double))
            {
                return value => EditorGUILayout.DoubleField(label, (double) value, options);
            }

            if (type == typeof(short))
            {
                return value => EditorGUILayout.IntField(label, (short) value, options);
            }

            if (type == typeof(int))
            {
                return value => EditorGUILayout.IntField(label, (int) value, options);
            }

            if (type == typeof(long))
            {
                return value => EditorGUILayout.LongField(label, (long) value, options);
            }

            if (type == typeof(sbyte))
            {
                return value => EditorGUILayout.IntField(label, (sbyte) value, options);
            }

            if (type == typeof(float))
            {
                return value => EditorGUILayout.FloatField(label, (float) value, options);
            }

            if (type == typeof(string))
            {
                return value => EditorGUILayout.TextField(label, (string) value, options);
            }

            if (type == typeof(ushort))
            {
                return value => EditorGUILayout.IntField(label, (ushort) value, options);
            }

            if (type == typeof(uint))
            {
                return value => EditorGUILayout.IntField(label, (int) (uint) value, options);
            }

            if (type == typeof(ulong))
            {
                return value => EditorGUILayout.LongField(label, (long) (ulong) value, options);
            }

            if (type == typeof(Vector2))
            {
                return value => EditorGUILayout.Vector2Field(label, (Vector2) value, options);
            }

            if (type == typeof(Vector3))
            {
                return value => EditorGUILayout.Vector3Field(label, (Vector3) value, options);
            }

            if (type == typeof(Vector4))
            {
                return value => EditorGUILayout.Vector4Field(label, (Vector4) value, options);
            }

            if (type == typeof(Vector2Int))
            {
                return value => EditorGUILayout.Vector2IntField(label, (Vector2Int) value, options);
            }

            if (type == typeof(Vector3Int))
            {
                return value => EditorGUILayout.Vector3IntField(label, (Vector3Int) value, options);
            }

            if (type == typeof(Quaternion))
            {
                return value => EditorGUILayout.Vector4Field(label, ((Quaternion) value).ToVector4(), options);
            }

            if (type == typeof(Color))
            {
                return value => EditorGUILayout.ColorField(label, (Color) value, options);
            }

            if (type == typeof(Color32))
            {
                return value => EditorGUILayout.ColorField(label, (Color32) value, options);
            }

            if (type == typeof(Rect))
            {
                return value => EditorGUILayout.RectField(label, (Rect) value, options);
            }

            if (type == typeof(AnimationCurve))
            {
                return value => EditorGUILayout.CurveField(label, (AnimationCurve) value, options);
            }

            if (type.IsAssignableFrom(typeof(UnityEngine.Object)))
            {
                return value => EditorGUILayout.ObjectField(label, (UnityEngine.Object) value, type, true, options);
            }

            return value =>
            {
                EditorGUILayout.HelpBox($"Warning no EditorGUI for {type.HumanizedName()} implemented!", MessageType.Warning);
                return value;
            };
        }

    }
}