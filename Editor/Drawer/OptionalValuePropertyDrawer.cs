using System;
using Baracuda.Utility.Types;
using UnityEngine;

namespace Baracuda.Utility.Editor.Drawer
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
            try
            {
                UnityEditor.EditorGUILayout.BeginHorizontal();

                var enabledProperty = property.FindPropertyRelative("enabled");
                var valueProperty = property.FindPropertyRelative("value");

                enabledProperty.boolValue =
                    UnityEditor.EditorGUILayout.ToggleLeft(label, enabledProperty.boolValue, GUILayout.Width(UnityEditor.EditorGUIUtility.labelWidth));

                using (new UnityEditor.EditorGUI.DisabledGroupScope(!enabledProperty.boolValue))
                {
                    if (valueProperty.isArray && valueProperty.type != "string")
                    {
                        UnityEditor.EditorGUILayout.PropertyField(valueProperty, GUIContent.none, true);
                    }
                    else
                    {
                        UnityEditor.EditorGUILayout.PropertyField(valueProperty, GUIContent.none);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            finally
            {
                UnityEditor.EditorGUILayout.EndHorizontal();
            }
        }
    }
}