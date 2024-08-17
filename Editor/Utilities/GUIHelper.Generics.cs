using UnityEngine;

namespace Baracuda.Utilities.Editor.Utilities
{
    public static partial class GUIUtility
    {
        public static T ObjectField<T>(string label, T obj, bool allowSceneObjects) where T : Object
        {
            return (T)UnityEditor.EditorGUILayout.ObjectField(label, obj, typeof(T), allowSceneObjects);
        }

        public static T ObjectField<T>(Rect rect, GUIContent label, T obj, bool allowSceneObjects) where T : Object
        {
            return (T)UnityEditor.EditorGUI.ObjectField(rect, label, obj, typeof(T), allowSceneObjects);
        }
    }
}