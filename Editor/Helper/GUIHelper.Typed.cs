using System;
using UnityEditor;
using UnityEngine;

namespace Baracuda.Utilities.Helper
{
    public static partial class GUIHelper
    {
        public static object DynamicField(string label, object value, Type type, params GUILayoutOption[] options)
        {
            return DynamicField(new GUIContent(label), value, type, options);
        }

        public static object DynamicField(GUIContent label, object value, Type type, params GUILayoutOption[] options)
        {
            switch (value)
            {
                case int parsedValue:
                    return EditorGUILayout.IntField(label, parsedValue, options);
                case float parsedValue:
                    return EditorGUILayout.FloatField(label, parsedValue, options);
                case bool parsedValue:
                    return EditorGUILayout.Toggle(label, parsedValue, options);
                case double parsedValue:
                    return EditorGUILayout.DoubleField(label, parsedValue, options);
                case Vector2 parsedValue:
                    return EditorGUILayout.Vector2Field(label, parsedValue, options);
                case Vector3 parsedValue:
                    return EditorGUILayout.Vector3Field(label, parsedValue, options);
                case Vector4 parsedValue:
                    return EditorGUILayout.Vector4Field(label, parsedValue, options);
                case Vector2Int parsedValue:
                    return EditorGUILayout.Vector2IntField(label, parsedValue, options);
                case Vector3Int parsedValue:
                    return EditorGUILayout.Vector3IntField(label, parsedValue, options);
                case Color parsedValue:
                    return EditorGUILayout.ColorField(label, parsedValue, options);
                case Color32 parsedValue:
                    return EditorGUILayout.ColorField(label, parsedValue, options);
                case string parsedValue:
                    return EditorGUILayout.TextField(label, parsedValue, options);
                case AnimationCurve parsedValue:
                    return EditorGUILayout.CurveField(label, parsedValue, options);
                case UnityEngine.Object parsedValue:
                    return EditorGUILayout.ObjectField(label, parsedValue, type, true, options);
                case null:
                    return EditorGUILayout.ObjectField(label, null, type, true, options);
                //TODO: add list/array drawer
                //TODO: remove null
            }

            Debug.LogWarning($"Warning no EditorGUI for {type.Name} implemented!");
            return null;
        }
    }
}