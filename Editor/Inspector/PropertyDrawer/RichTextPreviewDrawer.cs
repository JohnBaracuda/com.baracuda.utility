using Baracuda.Utilities.Helper;
using UnityEditor;
using UnityEngine;

namespace Baracuda.Utilities.Inspector.PropertyDrawer
{
    [CustomPropertyDrawer(typeof(RichTextPreviewAttribute))]
    internal class RichTextPreviewDrawer : UnityEditor.PropertyDrawer
    {
        private bool _enabled;
        private GUIContent _richTextLabel;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType == SerializedPropertyType.String)
            {
                property.serializedObject.Update();

                EditorGUI.LabelField(position, label);
                property.stringValue = EditorGUILayout.TextArea(property.stringValue, _enabled ? GUIHelper.RichTextArea : GUIHelper.TextArea);
                property.serializedObject.ApplyModifiedProperties();

                _richTextLabel ??= new GUIContent("Enable Rich Text")
                {
                    tooltip = $"{label.tooltip} (toggle to preview rich text formatting)",
                };
                if (GUILayout.Button(_enabled ? "Stop Rich Text Preview" : "Start Rich Text Preview"))
                {
                    _enabled = !_enabled;
                }
            }
            else
            {
                EditorGUI.LabelField(position, label.text, "Use RichText Preview with string.");
            }
        }
    }
}