using Baracuda.Utilities.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Baracuda.Utilities.Inspector.PropertyDrawer
{
    [CustomPropertyDrawer(typeof(Map<,>))]
    public class MapPropertyDrawer : UnityEditor.PropertyDrawer
    {
        private const string KeysFieldName = "keys";
        private const string ValuesFieldName = "values";
        private const float IndentWidth = 15f;

        private static readonly GUIContent iconPlus = IconContent("Toolbar Plus", "Add entry");
        private static readonly GUIContent iconMinus = IconContent("Toolbar Minus", "Remove entry");

        private static readonly GUIContent warningIconConflict =
            IconContent("console.warnicon.sml", "Conflicting key, this entry will be lost");

        private static readonly GUIContent warningIconOther = IconContent("console.infoicon.sml", "Conflicting key");

        private static readonly GUIContent warningIconNull =
            IconContent("console.warnicon.sml", "Null key, this entry will be lost");

        private static readonly GUIStyle buttonStyle = GUIStyle.none;

        private class ConflictState
        {
            public object ConflictKey = null;
            public object ConflictValue = null;
            public int ConflictIndex = -1;
            public int ConflictOtherIndex = -1;
            public bool ConflictKeyPropertyExpanded = false;
            public bool ConflictValuePropertyExpanded = false;
            public float ConflictLineHeight = 0f;
        }

        private struct PropertyIdentity
        {
            public PropertyIdentity(SerializedProperty property)
            {
                _instance = property.serializedObject.targetObject;
                _propertyPath = property.propertyPath;
            }

            private UnityEngine.Object _instance;
            private string _propertyPath;
        }

        private static readonly Dictionary<PropertyIdentity, ConflictState> sConflictStateDict = new();

        private enum Action
        {
            None,
            Add,
            Remove
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            label = EditorGUI.BeginProperty(position, label, property);

            var buttonAction = Action.None;
            var buttonActionIndex = 0;

            var keyArrayProperty = property.FindPropertyRelative(KeysFieldName);
            var valueArrayProperty = property.FindPropertyRelative(ValuesFieldName);

            var conflictState = GetConflictState(property);

            if (conflictState.ConflictIndex != -1)
            {
                keyArrayProperty.InsertArrayElementAtIndex(conflictState.ConflictIndex);
                var keyProperty = keyArrayProperty.GetArrayElementAtIndex(conflictState.ConflictIndex);
                SetPropertyValue(keyProperty, conflictState.ConflictKey);
                keyProperty.isExpanded = conflictState.ConflictKeyPropertyExpanded;

                if (valueArrayProperty != null)
                {
                    valueArrayProperty.InsertArrayElementAtIndex(conflictState.ConflictIndex);
                    var valueProperty = valueArrayProperty.GetArrayElementAtIndex(conflictState.ConflictIndex);
                    SetPropertyValue(valueProperty, conflictState.ConflictValue);
                    valueProperty.isExpanded = conflictState.ConflictValuePropertyExpanded;
                }
            }

            var buttonWidth = buttonStyle.CalcSize(iconPlus).x;

            var labelPosition = position;
            labelPosition.height = EditorGUIUtility.singleLineHeight;
            labelPosition.xMax -= buttonStyle.CalcSize(iconPlus).x;

            EditorGUI.PropertyField(labelPosition, property, label, false);
            if (property.isExpanded)
            {
                var buttonPosition = position;
                buttonPosition.xMin = buttonPosition.xMax - buttonWidth;
                buttonPosition.height = EditorGUIUtility.singleLineHeight;
                EditorGUI.BeginDisabledGroup(conflictState.ConflictIndex != -1);
                if (GUI.Button(buttonPosition, iconPlus, buttonStyle))
                {
                    buttonAction = Action.Add;
                    buttonActionIndex = keyArrayProperty.arraySize;
                }

                EditorGUI.EndDisabledGroup();

                EditorGUI.indentLevel++;
                var linePosition = position;
                linePosition.y += EditorGUIUtility.singleLineHeight;
                linePosition.xMax -= buttonWidth;

                foreach (var entry in EnumerateEntries(keyArrayProperty, valueArrayProperty))
                {
                    var keyProperty = entry.KeyProperty;
                    var valueProperty = entry.ValueProperty;
                    var i = entry.Index;

                    var lineHeight = DrawKeyValueLine(keyProperty, valueProperty, linePosition);

                    buttonPosition = linePosition;
                    buttonPosition.x = linePosition.xMax;
                    buttonPosition.height = EditorGUIUtility.singleLineHeight;
                    if (GUI.Button(buttonPosition, iconMinus, buttonStyle))
                    {
                        buttonAction = Action.Remove;
                        buttonActionIndex = i;
                    }

                    if (i == conflictState.ConflictIndex && conflictState.ConflictOtherIndex == -1)
                    {
                        var iconPosition = linePosition;
                        iconPosition.size = buttonStyle.CalcSize(warningIconNull);
                        GUI.Label(iconPosition, warningIconNull);
                    }
                    else if (i == conflictState.ConflictIndex)
                    {
                        var iconPosition = linePosition;
                        iconPosition.size = buttonStyle.CalcSize(warningIconConflict);
                        GUI.Label(iconPosition, warningIconConflict);
                    }
                    else if (i == conflictState.ConflictOtherIndex)
                    {
                        var iconPosition = linePosition;
                        iconPosition.size = buttonStyle.CalcSize(warningIconOther);
                        GUI.Label(iconPosition, warningIconOther);
                    }


                    linePosition.y += lineHeight;
                }

                EditorGUI.indentLevel--;
            }
            else if (keyArrayProperty.arraySize == 0)
            {
                var buttonPosition = position;
                buttonPosition.xMin = buttonPosition.xMax - buttonWidth;
                buttonPosition.height = EditorGUIUtility.singleLineHeight;
                if (GUI.Button(buttonPosition, iconPlus, buttonStyle))
                {
                    buttonAction = Action.Add;
                    buttonActionIndex = keyArrayProperty.arraySize;
                }
            }

            switch (buttonAction)
            {
                case Action.Add:
                {
                    // add new entry
                    keyArrayProperty.InsertArrayElementAtIndex(buttonActionIndex);
                    if (valueArrayProperty != null)
                    {
                        valueArrayProperty.InsertArrayElementAtIndex(buttonActionIndex);
                    }

                    // auto increment key
                    var newEntry = keyArrayProperty.GetArrayElementAtIndex(buttonActionIndex);
                    if (IsIntValue(newEntry.propertyType))
                    {
                        newEntry.intValue++;
                    }

                    // automatically expand upon adding first entry
                    if (buttonActionIndex == 0)
                    {
                        property.isExpanded = true;
                    }

                    break;
                }
                case Action.Remove:
                {
                    DeleteArrayElementAtIndex(keyArrayProperty, buttonActionIndex);
                    if (valueArrayProperty != null)
                    {
                        DeleteArrayElementAtIndex(valueArrayProperty, buttonActionIndex);
                    }

                    break;
                }
            }

            conflictState.ConflictKey = null;
            conflictState.ConflictValue = null;
            conflictState.ConflictIndex = -1;
            conflictState.ConflictOtherIndex = -1;
            conflictState.ConflictLineHeight = 0f;
            conflictState.ConflictKeyPropertyExpanded = false;
            conflictState.ConflictValuePropertyExpanded = false;

            foreach (var entry1 in EnumerateEntries(keyArrayProperty, valueArrayProperty))
            {
                var keyProperty1 = entry1.KeyProperty;
                var i = entry1.Index;
                var keyProperty1Value = GetPropertyValue(keyProperty1);

                if (keyProperty1Value == null)
                {
                    var valueProperty1 = entry1.ValueProperty;
                    SaveProperty(keyProperty1, valueProperty1, i, -1, conflictState);
                    DeleteArrayElementAtIndex(keyArrayProperty, i);
                    if (valueArrayProperty != null)
                    {
                        DeleteArrayElementAtIndex(valueArrayProperty, i);
                    }

                    break;
                }


                foreach (var entry2 in EnumerateEntries(keyArrayProperty, valueArrayProperty, i + 1))
                {
                    var keyProperty2 = entry2.KeyProperty;
                    var j = entry2.Index;
                    var keyProperty2Value = GetPropertyValue(keyProperty2);

                    if (ComparePropertyValues(keyProperty1Value, keyProperty2Value))
                    {
                        var valueProperty2 = entry2.ValueProperty;
                        SaveProperty(keyProperty2, valueProperty2, j, i, conflictState);
                        DeleteArrayElementAtIndex(keyArrayProperty, j);
                        if (valueArrayProperty != null)
                        {
                            DeleteArrayElementAtIndex(valueArrayProperty, j);
                        }

                        goto breakLoops;
                    }
                }
            }

            breakLoops:

            EditorGUI.EndProperty();
        }

        private static float DrawKeyValueLine(SerializedProperty keyProperty, SerializedProperty valueProperty,
            Rect linePosition)
        {
            var labelWidth = EditorGUIUtility.labelWidth;
            var labelWidthRelative = labelWidth / linePosition.width;

            var keyPropertyHeight = EditorGUI.GetPropertyHeight(keyProperty);
            var keyPosition = linePosition;
            keyPosition.height = keyPropertyHeight;
            keyPosition.width = labelWidth - IndentWidth;
            EditorGUIUtility.labelWidth = keyPosition.width * labelWidthRelative;
            EditorGUI.PropertyField(keyPosition, keyProperty, GUIContent.none, true);

            var valuePropertyHeight = EditorGUI.GetPropertyHeight(valueProperty);
            var valuePosition = linePosition;
            valuePosition.height = valuePropertyHeight;
            valuePosition.xMin += labelWidth;
            EditorGUIUtility.labelWidth = valuePosition.width * labelWidthRelative;
            EditorGUI.indentLevel--;
            EditorGUI.PropertyField(valuePosition, valueProperty, GUIContent.none, true);
            EditorGUI.indentLevel++;

            EditorGUIUtility.labelWidth = labelWidth;

            return Mathf.Max(keyPropertyHeight, valuePropertyHeight);
        }

        private static void SaveProperty(SerializedProperty keyProperty, SerializedProperty valueProperty, int index,
            int otherIndex, ConflictState conflictState)
        {
            conflictState.ConflictKey = GetPropertyValue(keyProperty);
            conflictState.ConflictValue = valueProperty != null ? GetPropertyValue(valueProperty) : null;
            var keyPropertyHeight = EditorGUI.GetPropertyHeight(keyProperty);
            var valuePropertyHeight = valueProperty != null ? EditorGUI.GetPropertyHeight(valueProperty) : 0f;
            var lineHeight = Mathf.Max(keyPropertyHeight, valuePropertyHeight);
            conflictState.ConflictLineHeight = lineHeight;
            conflictState.ConflictIndex = index;
            conflictState.ConflictOtherIndex = otherIndex;
            conflictState.ConflictKeyPropertyExpanded = keyProperty.isExpanded;
            conflictState.ConflictValuePropertyExpanded = valueProperty != null && valueProperty.isExpanded;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var propertyHeight = EditorGUIUtility.singleLineHeight;

            if (property.isExpanded)
            {
                var keysProperty = property.FindPropertyRelative("keys");
                var valuesProperty = property.FindPropertyRelative("values");

                foreach (var entry in EnumerateEntries(keysProperty, valuesProperty))
                {
                    var keyProperty = entry.KeyProperty;
                    var valueProperty = entry.ValueProperty;
                    var keyPropertyHeight = EditorGUI.GetPropertyHeight(keyProperty);
                    var valuePropertyHeight = valueProperty != null ? EditorGUI.GetPropertyHeight(valueProperty) : 0f;
                    var lineHeight = Mathf.Max(keyPropertyHeight, valuePropertyHeight);
                    propertyHeight += lineHeight;
                }

                var conflictState = GetConflictState(property);

                if (conflictState.ConflictIndex != -1)
                {
                    propertyHeight += conflictState.ConflictLineHeight;
                }
            }

            return propertyHeight;
        }

        private static ConflictState GetConflictState(SerializedProperty property)
        {
            PropertyIdentity propId = new(property);
            if (!sConflictStateDict.TryGetValue(propId, out var conflictState))
            {
                conflictState = new ConflictState();
                sConflictStateDict.Add(propId, conflictState);
            }

            return conflictState;
        }

        private static readonly Dictionary<SerializedPropertyType, PropertyInfo> sSerializedPropertyValueAccessorsDict;

        static MapPropertyDrawer()
        {
            Dictionary<SerializedPropertyType, string> serializedPropertyValueAccessorsNameDict = new()
            {
                {SerializedPropertyType.Integer, "intValue"},
                {SerializedPropertyType.Boolean, "boolValue"},
                {SerializedPropertyType.Float, "floatValue"},
                {SerializedPropertyType.String, "stringValue"},
                {SerializedPropertyType.Color, "colorValue"},
                {SerializedPropertyType.ObjectReference, "objectReferenceValue"},
                {SerializedPropertyType.LayerMask, "intValue"},
                {SerializedPropertyType.Enum, "intValue"},
                {SerializedPropertyType.Vector2, "vector2Value"},
                {SerializedPropertyType.Vector3, "vector3Value"},
                {SerializedPropertyType.Vector4, "vector4Value"},
                {SerializedPropertyType.Rect, "rectValue"},
                {SerializedPropertyType.ArraySize, "intValue"},
                {SerializedPropertyType.Character, "intValue"},
                {SerializedPropertyType.AnimationCurve, "animationCurveValue"},
                {SerializedPropertyType.Bounds, "boundsValue"},
                {SerializedPropertyType.Quaternion, "quaternionValue"},
            };
            var serializedPropertyType = typeof(SerializedProperty);

            sSerializedPropertyValueAccessorsDict = new Dictionary<SerializedPropertyType, PropertyInfo>();
            const BindingFlags Flags = BindingFlags.Instance | BindingFlags.Public;

            foreach (var kvp in serializedPropertyValueAccessorsNameDict)
            {
                var propertyInfo = serializedPropertyType.GetProperty(kvp.Value, Flags);
                sSerializedPropertyValueAccessorsDict.Add(kvp.Key, propertyInfo);
            }
        }

        private static bool IsIntValue(SerializedPropertyType type) => type switch
        {
            SerializedPropertyType.Enum => true,
            SerializedPropertyType.Integer => true,
            _ => false,
        };

        private static GUIContent IconContent(string name, string tooltip)
        {
            var builtinIcon = EditorGUIUtility.IconContent(name);
            return new GUIContent(builtinIcon.image, tooltip);
        }

        private static void DeleteArrayElementAtIndex(SerializedProperty arrayProperty, int index)
        {
            var property = arrayProperty.GetArrayElementAtIndex(index);
            if (property.propertyType == SerializedPropertyType.ObjectReference)
            {
                property.objectReferenceValue = null;
            }

            arrayProperty.DeleteArrayElementAtIndex(index);
        }

        public static object GetPropertyValue(SerializedProperty p)
        {
            if (sSerializedPropertyValueAccessorsDict.TryGetValue(p.propertyType, out var propertyInfo))
            {
                return propertyInfo.GetValue(p, null);
            }
            else
            {
                if (p.isArray)
                {
                    return GetPropertyValueArray(p);
                }
                else
                {
                    return GetPropertyValueGeneric(p);
                }
            }
        }

        private static void SetPropertyValue(SerializedProperty p, object v)
        {
            if (sSerializedPropertyValueAccessorsDict.TryGetValue(p.propertyType, out var propertyInfo))
            {
                propertyInfo.SetValue(p, v, null);
            }
            else
            {
                if (p.isArray)
                {
                    SetPropertyValueArray(p, v);
                }
                else
                {
                    SetPropertyValueGeneric(p, v);
                }
            }
        }

        private static object GetPropertyValueArray(SerializedProperty property)
        {
            var array = new object[property.arraySize];
            for (var i = 0; i < property.arraySize; i++)
            {
                var item = property.GetArrayElementAtIndex(i);
                array[i] = GetPropertyValue(item);
            }

            return array;
        }

        private static object GetPropertyValueGeneric(SerializedProperty property)
        {
            Dictionary<string, object> dict = new();
            var iterator = property.Copy();
            if (iterator.Next(true))
            {
                var end = property.GetEndProperty();
                do
                {
                    var name = iterator.name;
                    var value = GetPropertyValue(iterator);
                    dict.Add(name, value);
                } while (iterator.Next(false) && iterator.propertyPath != end.propertyPath);
            }

            return dict;
        }

        private static void SetPropertyValueArray(SerializedProperty property, object v)
        {
            var array = (object[]) v;
            property.arraySize = array.Length;
            for (var i = 0; i < property.arraySize; i++)
            {
                var item = property.GetArrayElementAtIndex(i);
                SetPropertyValue(item, array[i]);
            }
        }

        private static void SetPropertyValueGeneric(SerializedProperty property, object v)
        {
            var dict = (Dictionary<string, object>) v;
            var iterator = property.Copy();
            if (iterator.Next(true))
            {
                var end = property.GetEndProperty();
                do
                {
                    var name = iterator.name;
                    SetPropertyValue(iterator, dict[name]);
                } while (iterator.Next(false) && iterator.propertyPath != end.propertyPath);
            }
        }

        private static bool ComparePropertyValues(object value1, object value2)
        {
            if (value1 is Dictionary<string, object> dictionary1 && value2 is Dictionary<string, object> dictionary2)
            {
                return CompareDictionaries(dictionary1, dictionary2);
            }
            else
            {
                return Equals(value1, value2);
            }
        }

        private static bool CompareDictionaries(Dictionary<string, object> dict1, Dictionary<string, object> dict2)
        {
            if (dict1.Count != dict2.Count)
            {
                return false;
            }

            foreach (var kvp1 in dict1)
            {
                var key1 = kvp1.Key;
                var value1 = kvp1.Value;

                if (!dict2.TryGetValue(key1, out var value2))
                {
                    return false;
                }

                if (!ComparePropertyValues(value1, value2))
                {
                    return false;
                }
            }

            return true;
        }

        private struct EnumerationEntry
        {
            public readonly SerializedProperty KeyProperty;
            public readonly SerializedProperty ValueProperty;
            public readonly int Index;

            public EnumerationEntry(SerializedProperty keyProperty, SerializedProperty valueProperty, int index)
            {
                KeyProperty = keyProperty;
                ValueProperty = valueProperty;
                Index = index;
            }
        }

        private static IEnumerable<EnumerationEntry> EnumerateEntries(SerializedProperty keyArrayProperty,
            SerializedProperty valueArrayProperty, int startIndex = 0)
        {
            if (keyArrayProperty.arraySize > startIndex)
            {
                var index = startIndex;
                var keyProperty = keyArrayProperty.GetArrayElementAtIndex(startIndex);
                var valueProperty = valueArrayProperty?.GetArrayElementAtIndex(startIndex);
                var endProperty = keyArrayProperty.GetEndProperty();

                do
                {
                    yield return new EnumerationEntry(keyProperty, valueProperty, index);
                    index++;
                } while (keyProperty.Next(false)
                         && (valueProperty == null || valueProperty.Next(false))
                         && !SerializedProperty.EqualContents(keyProperty, endProperty));
            }
        }
    }
}