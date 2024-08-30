using Baracuda.Bedrock.Types;
using UnityEngine;

namespace Baracuda.Utilities.Editor.Drawer
{
    [UnityEditor.CustomPropertyDrawer(typeof(Optional<>))]
    public class OptionalValuePropertyDrawer : UnityEditor.PropertyDrawer
    {
        public override float GetPropertyHeight(UnityEditor.SerializedProperty property, GUIContent label)
        {
            return 0;
        }

        public override void OnGUI(Rect position, UnityEditor.SerializedProperty property, GUIContent label)
        {
            var enabledProperty = property.FindPropertyRelative("enabled");
            var valueProperty = property.FindPropertyRelative("value");

            UnityEditor.EditorGUILayout.BeginHorizontal();
            enabledProperty.boolValue =
                UnityEditor.EditorGUILayout.ToggleLeft(label, enabledProperty.boolValue, GUILayout.Width(UnityEditor.EditorGUIUtility.labelWidth));

            using (new UnityEditor.EditorGUI.DisabledGroupScope(!enabledProperty.boolValue))
            {
                if (valueProperty.isArray && valueProperty.type != "string")
                {
                    UnityEditor.EditorGUILayout.EndHorizontal();
                    UnityEditor.EditorGUILayout.PropertyField(valueProperty, GUIContent.none, true);
                }
                else
                {
                    UnityEditor.EditorGUILayout.PropertyField(valueProperty, GUIContent.none);
                    UnityEditor.EditorGUILayout.EndHorizontal();
                }
            }
        }
    }
}