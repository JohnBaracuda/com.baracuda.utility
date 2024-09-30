using Baracuda.Utility.Attributes;
using UnityEngine;
using GUIUtility = Baracuda.Utility.Editor.Utilities.GUIUtility;

namespace Baracuda.Utility.Editor.Drawer
{
    [UnityEditor.CustomPropertyDrawer(typeof(AssetGUIDAttribute))]
    public class AssetGUIDPropertyDrawer : UnityEditor.PropertyDrawer
    {
        private const int ToggleButtonWidth = 40;
        private const int UpdateButtonWidth = 60;
        private bool _enableEdit;

        public override void OnGUI(Rect position, UnityEditor.SerializedProperty property, GUIContent label)
        {
            GUIUtility.BeginEnabledOverride(_enableEdit);
            const int combinedWidth = ToggleButtonWidth + UpdateButtonWidth;
            var textPos = new Rect(position.x, position.y, position.width - combinedWidth, position.height);
            UnityEditor.EditorGUI.PropertyField(textPos, property, label);
            GUIUtility.EndEnabledOverride();

            var updateButtonPosition = new Rect(position.x + position.width - UpdateButtonWidth, position.y,
                UpdateButtonWidth,
                position.height);
            var toggleButtonPosition = new Rect(position.x + position.width - combinedWidth, position.y,
                ToggleButtonWidth,
                position.height);
            if (GUI.Button(toggleButtonPosition, _enableEdit ? "Lock" : "Edit"))
            {
                _enableEdit = !_enableEdit;
            }
            if (GUI.Button(updateButtonPosition, "Update"))
            {
                UpdateGuid(property);
            }
        }

        private void UpdateGuid(UnityEditor.SerializedProperty property)
        {
            if (UnityEditor.PrefabUtility.IsPartOfAnyPrefab(property.serializedObject.targetObject))
            {
                var prefabPath =
                    UnityEditor.PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(property.serializedObject
                        .targetObject);
                var prefabGuid = UnityEditor.AssetDatabase.AssetPathToGUID(prefabPath);
                property.serializedObject.Update();
                property.stringValue = prefabGuid;
                property.serializedObject.ApplyModifiedProperties();
                return;
            }
            var path = UnityEditor.AssetDatabase.GetAssetPath(property.serializedObject.targetObject);
            var guid = UnityEditor.AssetDatabase.AssetPathToGUID(path);
            property.stringValue = guid;
        }
    }
}