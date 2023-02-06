using UnityEditor;
using UnityEngine;

namespace Baracuda.Utilities.Inspector.PropertyDrawer
{
    [CustomPropertyDrawer(typeof(StringSelectionAttribute))]
    internal class StringSelectionDrawer : UnityEditor.PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var selectionAttribute = (StringSelectionAttribute) attribute;
            var strings = selectionAttribute.Strings;

            if (property.propertyType == SerializedPropertyType.String)
            {
                var selectedIndex = 0;
                for (var i = 0; i < strings.Length; i++)
                {
                    if (property.stringValue == strings[i])
                    {
                        selectedIndex = i;
                        break;
                    }
                }

                selectedIndex = EditorGUI.Popup(position, label.text, selectedIndex, selectionAttribute.Strings);


                property.stringValue = strings[selectedIndex];
                property.serializedObject.ApplyModifiedProperties();
            }
            else
            {
                EditorGUI.LabelField(position, label.text, "Use StringSelectionAttribute with string.");
            }
        }
    }
}