using UnityEditor;
using UnityEngine;

namespace Baracuda.Utilities.Inspector.PropertyDrawer
{
    [CustomPropertyDrawer(typeof(LockableFieldAttribute))]
    internal class LockableFieldDrawer : UnityEditor.PropertyDrawer
    {
        private bool _writable = false;
        private const int ButtonWidth = 50;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var enabled = GUI.enabled;
            GUI.enabled = _writable;
            var textPos = new Rect(position.x, position.y, position.width - ButtonWidth, position.height);
            EditorGUI.PropertyField(textPos, property, label);
            GUI.enabled = enabled;

            var buttonPos = new Rect(position.x + position.width - ButtonWidth, position.y, ButtonWidth, position.height);
            if (GUI.Button(buttonPos , _writable? "Lock" : "Edit"))
            {
                _writable = !_writable;
            }
        }
    }
}