using UnityEditor;
using UnityEngine;

namespace Baracuda.Utilities.Inspector.PropertyDrawer
{
    [CustomPropertyDrawer(typeof(OptionalValue<>))]
    public class OptionalValuePropertyDrawer : UnityEditor.PropertyDrawer
    {
        private readonly GUIContent whiteSpace = new GUIContent("   ");

        /// <summary>
        ///   <para>Override this method to make your own IMGUI based GUI for the property.</para>
        /// </summary>
        /// <param name="position">Rectangle on the screen to use for the property GUI.</param>
        /// <param name="property">The SerializedProperty to make the custom GUI for.</param>
        /// <param name="label">The label of this property.</param>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var enabledProperty = property.FindPropertyRelative("enabled");
            var valueProperty = property.FindPropertyRelative("value");
            var guiEnabled = GUI.enabled;
            const int Width = 60;
            GUI.enabled = enabledProperty.boolValue;
            var valueRect = position;
            valueRect.x += Width;
            valueRect.width -= Width;
            var labelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth -= Width;
            whiteSpace.tooltip = label.tooltip;
            EditorGUI.PropertyField(valueRect, valueProperty, whiteSpace);
            EditorGUIUtility.labelWidth = labelWidth;
            GUI.enabled = guiEnabled;
            var boolRect = position;
            boolRect.width = EditorGUIUtility.labelWidth - Width;
            enabledProperty.boolValue = EditorGUI.ToggleLeft(boolRect, label, enabledProperty.boolValue);
        }
    }
}