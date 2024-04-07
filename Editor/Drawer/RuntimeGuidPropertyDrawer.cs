using Baracuda.Utilities.Types;
using UnityEngine;
using GUIUtility = Baracuda.Utilities.Editor.Helper.GUIUtility;

namespace Baracuda.Utilities.Editor.Drawer
{
    [UnityEditor.CustomPropertyDrawer(typeof(RuntimeGUID))]
    public class RuntimeGUIDPropertyDrawer : UnityEditor.PropertyDrawer
    {
        private UnityEditor.SerializedProperty _stringProperty;
        private const int ToggleButtonWidth = 40;
        private const int UpdateButtonWidth = 60;
        private bool _enableEdit;

        public override void OnGUI(Rect position, UnityEditor.SerializedProperty property, GUIContent label)
        {
            _stringProperty ??= property.FindPropertyRelative("value");
            GUIUtility.BeginEnabledOverride(_enableEdit);
            const int CombinedWidth = ToggleButtonWidth + UpdateButtonWidth;
            var textPos = new Rect(position.x, position.y, position.width - CombinedWidth, position.height);
            UnityEditor.EditorGUI.PropertyField(textPos, _stringProperty, label);
            GUIUtility.EndEnabledOverride();

            var updateButtonPosition = new Rect(position.x + position.width - UpdateButtonWidth, position.y,
                UpdateButtonWidth,
                position.height);
            var toggleButtonPosition = new Rect(position.x + position.width - CombinedWidth, position.y,
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
                _stringProperty.serializedObject.Update();
                _stringProperty.stringValue = prefabGuid;
                _stringProperty.serializedObject.ApplyModifiedProperties();
                return;
            }
            var path = UnityEditor.AssetDatabase.GetAssetPath(property.serializedObject.targetObject);
            var guid = UnityEditor.AssetDatabase.AssetPathToGUID(path);
            _stringProperty.stringValue = guid;
        }
    }
}