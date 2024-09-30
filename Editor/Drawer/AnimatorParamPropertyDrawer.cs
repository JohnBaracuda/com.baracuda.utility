using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Baracuda.Utility.Attributes;
using Baracuda.Utility.Reflection;
using UnityEngine;

namespace Baracuda.Utility.Editor.Drawer
{
    [UnityEditor.CustomPropertyDrawer(typeof(AnimatorParamAttribute))]
    public class AnimatorParamPropertyDrawer : UnityEditor.PropertyDrawer
    {
        private const string InvalidAnimatorControllerWarningMessage = "Target animator controller is null";
        private const string InvalidTypeWarningMessage = "{0} must be an int or a string";

        public override float GetPropertyHeight(UnityEditor.SerializedProperty property, GUIContent label)
        {
            var animatorParamAttribute = PropertyUtility.GetAttribute<AnimatorParamAttribute>(property);
            var validAnimatorController = GetAnimatorController(property, animatorParamAttribute.AnimatorName) != null;
            var validPropertyType = property.propertyType == UnityEditor.SerializedPropertyType.Integer ||
                                    property.propertyType == UnityEditor.SerializedPropertyType.String;

            return validAnimatorController && validPropertyType
                ? base.GetPropertyHeight(property, label)
                : 0;
        }

        public override void OnGUI(Rect rect, UnityEditor.SerializedProperty property, GUIContent label)
        {
            UnityEditor.EditorGUI.BeginProperty(rect, label, property);

            var animatorParamAttribute = PropertyUtility.GetAttribute<AnimatorParamAttribute>(property);

            var animatorController = GetAnimatorController(property, animatorParamAttribute.AnimatorName);
            if (animatorController == null)
            {
                UnityEditor.EditorGUILayout.HelpBox(InvalidAnimatorControllerWarningMessage,
                    UnityEditor.MessageType.Warning);
                return;
            }

            var parametersCount = animatorController.parameters.Length;
            var animatorParameters = new List<AnimatorControllerParameter>(parametersCount);
            for (var i = 0; i < parametersCount; i++)
            {
                var parameter = animatorController.parameters[i];
                if (animatorParamAttribute.ParameterType == null ||
                    parameter.type == animatorParamAttribute.ParameterType)
                {
                    animatorParameters.Add(parameter);
                }
            }

            switch (property.propertyType)
            {
                case UnityEditor.SerializedPropertyType.Integer:
                    DrawPropertyForInt(rect, property, label, animatorParameters);
                    break;

                case UnityEditor.SerializedPropertyType.String:
                    DrawPropertyForString(rect, property, label, animatorParameters);
                    break;

                default:
                    UnityEditor.EditorGUILayout.HelpBox(string.Format(InvalidTypeWarningMessage, property.name),
                        UnityEditor.MessageType.Warning);
                    break;
            }

            UnityEditor.EditorGUI.EndProperty();
        }

        private static void DrawPropertyForInt(Rect rect, UnityEditor.SerializedProperty property, GUIContent label,
            List<AnimatorControllerParameter> animatorParameters)
        {
            var paramNameHash = property.intValue;
            var index = 0;

            for (var i = 0; i < animatorParameters.Count; i++)
            {
                if (paramNameHash == animatorParameters[i].nameHash)
                {
                    index = i + 1; // +1 because the first option is reserved for (None)
                    break;
                }
            }

            var displayOptions = GetDisplayOptions(animatorParameters);

            var newIndex = UnityEditor.EditorGUI.Popup(rect, label.text, index, displayOptions);
            var newValue = newIndex == 0 ? 0 : animatorParameters[newIndex - 1].nameHash;

            if (property.intValue != newValue)
            {
                property.intValue = newValue;
            }
        }

        private static void DrawPropertyForString(Rect rect, UnityEditor.SerializedProperty property, GUIContent label,
            List<AnimatorControllerParameter> animatorParameters)
        {
            var paramName = property.stringValue;
            var index = 0;

            for (var i = 0; i < animatorParameters.Count; i++)
            {
                if (paramName.Equals(animatorParameters[i].name, StringComparison.Ordinal))
                {
                    index = i + 1; // +1 because the first option is reserved for (None)
                    break;
                }
            }

            var displayOptions = GetDisplayOptions(animatorParameters);

            var newIndex = UnityEditor.EditorGUI.Popup(rect, label.text, index, displayOptions);
            var newValue = newIndex == 0 ? null : animatorParameters[newIndex - 1].name;

            if (!property.stringValue.Equals(newValue, StringComparison.Ordinal))
            {
                property.stringValue = newValue;
            }
        }

        private static string[] GetDisplayOptions(List<AnimatorControllerParameter> animatorParams)
        {
            var displayOptions = new string[animatorParams.Count + 1];
            displayOptions[0] = "(None)";

            for (var i = 0; i < animatorParams.Count; i++)
            {
                displayOptions[i + 1] = animatorParams[i].name;
            }

            return displayOptions;
        }

        private static UnityEditor.Animations.AnimatorController GetAnimatorController(
            UnityEditor.SerializedProperty property, string animatorName)
        {
            var target = PropertyUtility.GetTargetObjectWithProperty(property);
            var targetType = target.GetType();
            var animatorFieldInfo = targetType.GetFieldIncludeBaseTypes(animatorName);

            if (animatorFieldInfo != null &&
                animatorFieldInfo.FieldType == typeof(UnityEditor.Animations.AnimatorController))
            {
                return animatorFieldInfo.GetValue(target) as UnityEditor.Animations.AnimatorController;
            }

            var animatorPropertyInfo = targetType.GetPropertyIncludeBaseTypes(animatorName);
            if (animatorPropertyInfo != null &&
                animatorPropertyInfo.PropertyType == typeof(UnityEditor.Animations.AnimatorController))
            {
                return animatorPropertyInfo.GetValue(target) as UnityEditor.Animations.AnimatorController;
            }

            var animatorGetterMethodInfo = targetType.GetMethodIncludeBaseTypes(animatorName);
            if (animatorGetterMethodInfo != null &&
                animatorGetterMethodInfo.ReturnType == typeof(UnityEditor.Animations.AnimatorController) &&
                animatorGetterMethodInfo.GetParameters().Length == 0)
            {
                return animatorGetterMethodInfo.Invoke(target, null) as UnityEditor.Animations.AnimatorController;
            }

            return null;
        }


        #region Nested

        private static class PropertyUtility
        {
            public static T GetAttribute<T>(UnityEditor.SerializedProperty property) where T : class
            {
                var attributes = GetAttributes<T>(property);
                return attributes.Length > 0 ? attributes[0] : null;
            }

            public static T[] GetAttributes<T>(UnityEditor.SerializedProperty property) where T : class
            {
                var targetType = GetTargetObjectWithProperty(property).GetType();
                var fieldInfo = targetType.GetFieldIncludeBaseTypes(property.name);
                if (fieldInfo == null)
                {
                    return new T[]
                    {
                    };
                }

                return (T[])fieldInfo.GetCustomAttributes(typeof(T), true);
            }

            /// <summary>
            ///     Gets the object that the property is a member of
            /// </summary>
            /// <param name="property"></param>
            /// <returns></returns>
            public static object GetTargetObjectWithProperty(UnityEditor.SerializedProperty property)
            {
                var path = property.propertyPath.Replace(".Array.data[", "[");
                object obj = property.serializedObject.targetObject;
                var elements = path.Split('.');

                for (var i = 0; i < elements.Length - 1; i++)
                {
                    var element = elements[i];
                    if (element.Contains("["))
                    {
                        var elementName = element[..element.IndexOf("[", StringComparison.Ordinal)];
                        var index = Convert.ToInt32(element[element.IndexOf("[", StringComparison.Ordinal)..]
                            .Replace("[", "").Replace("]", ""));
                        obj = GetValue_Imp(obj, elementName, index);
                    }
                    else
                    {
                        obj = GetValue_Imp(obj, element);
                    }
                }

                return obj;
            }

            private static object GetValue_Imp(object source, string name)
            {
                if (source == null)
                {
                    return null;
                }

                var type = source.GetType();

                while (type != null)
                {
                    var field = type.GetField(name,
                        BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                    if (field != null)
                    {
                        return field.GetValue(source);
                    }

                    var property = type.GetProperty(name,
                        BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                    if (property != null)
                    {
                        return property.GetValue(source, null);
                    }

                    type = type.BaseType;
                }

                return null;
            }

            private static object GetValue_Imp(object source, string name, int index)
            {
                if (GetValue_Imp(source, name) is not IEnumerable enumerable)
                {
                    return null;
                }

                var enumerator = enumerable.GetEnumerator();
                for (var i = 0; i <= index; i++)
                {
                    if (!enumerator.MoveNext())
                    {
                        return null;
                    }
                }

                return enumerator.Current;
            }
        }

        #endregion
    }
}