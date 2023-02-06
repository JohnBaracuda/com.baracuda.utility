using Baracuda.Utilities.Helper;
using UnityEditor;
using UnityEngine;

namespace Baracuda.Utilities.Inspector.PropertyDrawer
{
    [CustomPropertyDrawer(typeof(RichTextPreviewAttribute))]
    internal class RichTextPreviewDrawer : UnityEditor.PropertyDrawer
    {
        private bool _enabled;
        private GUIContent _label;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType == SerializedPropertyType.String)
            {
                property.serializedObject.Update();
                _label ??= new GUIContent(label)
                {
                    tooltip = $"{label.tooltip} (toggle to preview rich text formatting)",
                };
                _enabled = EditorGUI.ToggleLeft(position, _label, _enabled);
                property.stringValue = EditorGUILayout.TextArea(property.stringValue, _enabled ? GUIHelper.RichTextArea : GUIHelper.TextArea);
                property.serializedObject.ApplyModifiedProperties();
            }
            else
            {
                EditorGUI.LabelField(position, label.text, "Use RichText Preview with string.");
            }
        }
    }
}