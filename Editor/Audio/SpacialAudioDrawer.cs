using Baracuda.Utility.Audio;
using UnityEngine;

namespace Baracuda.Utility.Editor.Audio
{
    [UnityEditor.CustomPropertyDrawer(typeof(SpacialAudio))]
    public class SpacialAudioDrawer : UnityEditor.PropertyDrawer
    {
        public override void OnGUI(Rect position, UnityEditor.SerializedProperty property, GUIContent label)
        {
            UnityEditor.EditorGUI.BeginProperty(position, label, property);

            // Indent for child properties
            UnityEditor.EditorGUI.indentLevel++;

            // Get references to the fields in the struct
            var eventReferenceProp = property.FindPropertyRelative("eventReference");
            var spacialSettingsProp = property.FindPropertyRelative("spacialSettings");

            // Calculate rects for each property
            var eventRefRect = new Rect(position.x, position.y, position.width, UnityEditor.EditorGUIUtility.singleLineHeight);
            var settingsRect = new Rect(position.x, position.y + UnityEditor.EditorGUIUtility.singleLineHeight + 4, position.width,
                UnityEditor.EditorGUIUtility.singleLineHeight);

            // Draw EventReference with children expanded
            UnityEditor.EditorGUI.PropertyField(eventRefRect, eventReferenceProp, new GUIContent("Event Reference"), true);

            // Check if SpacialAudioSettings is default and initialize if necessary
            if (IsDefaultSpacialAudioSettings(spacialSettingsProp))
            {
                spacialSettingsProp.FindPropertyRelative("volume").floatValue = 1f;
                spacialSettingsProp.FindPropertyRelative("radius").floatValue = 10f;
                spacialSettingsProp.FindPropertyRelative("falloff").animationCurveValue = AnimationCurve.Linear(0, 1, 1, 0);
            }

            // Draw SpacialAudioSettings with children expanded
            UnityEditor.EditorGUI.PropertyField(settingsRect, spacialSettingsProp, new GUIContent("Spacial Settings"), true);

            UnityEditor.EditorGUI.indentLevel--;

            UnityEditor.EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(UnityEditor.SerializedProperty property, GUIContent label)
        {
            // Adjust height to account for expandability and child properties
            var eventReferenceProp = property.FindPropertyRelative("eventReference");
            var spacialSettingsProp = property.FindPropertyRelative("spacialSettings");

            var height = UnityEditor.EditorGUIUtility.singleLineHeight + 4; // Base height
            if (eventReferenceProp.isExpanded)
            {
                height += UnityEditor.EditorGUI.GetPropertyHeight(eventReferenceProp, true);
            }
            if (spacialSettingsProp.isExpanded)
            {
                height += UnityEditor.EditorGUI.GetPropertyHeight(spacialSettingsProp, true);
            }

            return height;
        }

        private bool IsDefaultSpacialAudioSettings(UnityEditor.SerializedProperty spacialSettingsProp)
        {
            return spacialSettingsProp.FindPropertyRelative("volume").floatValue == 0f &&
                   spacialSettingsProp.FindPropertyRelative("radius").floatValue == 0f &&
                   spacialSettingsProp.FindPropertyRelative("falloff").animationCurveValue.length == 0;
        }
    }
}