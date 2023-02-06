using UnityEditor;
using UnityEngine;

namespace Baracuda.Utilities.Inspector.PropertyDrawer
{
    [CustomPropertyDrawer(typeof(LabelAttribute), true)]
    public class LabelDrawer : UnityEditor.PropertyDrawer
    {
        private string _label;
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            _label ??= ((LabelAttribute) attribute).Label;
            label.text = _label;
            EditorGUI.PropertyField(position, property, label);
        }
    }
}