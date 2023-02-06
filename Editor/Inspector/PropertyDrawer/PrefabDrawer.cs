using UnityEditor;
using UnityEngine;

namespace Baracuda.Utilities.Inspector.PropertyDrawer
{
    [CustomPropertyDrawer(typeof(Prefab))]
    internal class PrefabDrawer : UnityEditor.PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            property.serializedObject.Update();

            var enabledProperty = property.FindPropertyRelative("enabled");
            var prefabProperty = property.FindPropertyRelative("gameObject");

            if (prefabProperty.objectReferenceValue is GameObject obj && !obj.IsPrefab())
            {
                Debug.LogWarning("Only prefabs are allowed!");
                prefabProperty.objectReferenceValue = null;
            }
            EditorGUI.PropertyField(position.WithWidth(20), enabledProperty, GUIContent.none);
            EditorGUI.PropertyField(position.WithOffset(20, 0, -20), prefabProperty, GUIContent.none);

            property.serializedObject.ApplyModifiedProperties();
        }
    }
}