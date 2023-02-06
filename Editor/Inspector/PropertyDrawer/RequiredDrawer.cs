using UnityEditor;
using UnityEngine;

namespace Baracuda.Utilities.Inspector.PropertyDrawer
{
    [CustomPropertyDrawer(typeof(RequiredAttribute), true)]
    public class RequiredDrawer : UnityEditor.PropertyDrawer
    {
        private float _propertyHeight;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return _propertyHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var requiredAttribute = (RequiredAttribute) attribute;
            if (property.objectReferenceValue == null)
            {
                var baseHeight = base.GetPropertyHeight(property, label);
                var helpRect = new Rect(position.x + EditorGUI.indentLevel * 12, position.y, position.width, baseHeight * 2 - 2);
                var propRect = new Rect(position.x, position.y + baseHeight * 2, position.width, baseHeight);

                var message = requiredAttribute.Message ?? $"{property.displayName} is Required!";
                var messageType = requiredAttribute.MessageLevel;

                EditorGUI.HelpBox(helpRect, message, (MessageType) messageType);
                EditorGUI.PropertyField(propRect, property, label);

                _propertyHeight = baseHeight * 3;
            }
            else
            {
                _propertyHeight = base.GetPropertyHeight(property, label);
                EditorGUI.PropertyField(position, property, label);
            }
        }
    }
}