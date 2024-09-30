using System.Collections.Generic;
using System.Reflection;
using Baracuda.Utility.Collections;
using UnityEngine;

namespace Baracuda.Utility.Editor.Drawer
{
    [UnityEditor.CustomPropertyDrawer(typeof(Map<,>))]
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
            public object ConflictKey;
            public object ConflictValue;
            public int ConflictIndex = -1;
            public int ConflictOtherIndex = -1;
            public bool ConflictKeyPropertyExpanded;
            public bool ConflictValuePropertyExpanded;
            public float ConflictLineHeight;
        }

        private struct PropertyIdentity
        {
            public PropertyIdentity(UnityEditor.SerializedProperty property)
            {
                _instance = property.serializedObject.targetObject;
                _propertyPath = property.propertyPath;
            }

            private Object _instance;
            private string _propertyPath;
        }

        private static readonly Dictionary<PropertyIdentity, ConflictState> sConflictStateDict = new();

        private enum Action
        {
            None,
            Add,
            Remove
        }

        public override void OnGUI(Rect position, UnityEditor.SerializedProperty property, GUIContent label)
        {
            label = UnityEditor.EditorGUI.BeginProperty(position, label, property);

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
            labelPosition.height = UnityEditor.EditorGUIUtility.singleLineHeight;
            labelPosition.xMax -= buttonStyle.CalcSize(iconPlus).x;

            UnityEditor.EditorGUI.PropertyField(labelPosition, property, label, false);
            if (property.isExpanded)
            {
                var buttonPosition = position;
                buttonPosition.xMin = buttonPosition.xMax - buttonWidth;
                buttonPosition.height = UnityEditor.EditorGUIUtility.singleLineHeight;
                UnityEditor.EditorGUI.BeginDisabledGroup(conflictState.ConflictIndex != -1);
                if (GUI.Button(buttonPosition, iconPlus, buttonStyle))
                {
                    buttonAction = Action.Add;
                    buttonActionIndex = keyArrayProperty.arraySize;
                }

                UnityEditor.EditorGUI.EndDisabledGroup();

                UnityEditor.EditorGUI.indentLevel++;
                var linePosition = position;
                linePosition.y += UnityEditor.EditorGUIUtility.singleLineHeight;
                linePosition.xMax -= buttonWidth;

                foreach (var entry in EnumerateEntries(keyArrayProperty, valueArrayProperty))
                {
                    var keyProperty = entry.KeyProperty;
                    var valueProperty = entry.ValueProperty;
                    var i = entry.Index;

                    var lineHeight = DrawKeyValueLine(keyProperty, valueProperty, linePosition);

                    buttonPosition = linePosition;
                    buttonPosition.x = linePosition.xMax;
                    buttonPosition.height = UnityEditor.EditorGUIUtility.singleLineHeight;
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

                UnityEditor.EditorGUI.indentLevel--;
            }
            else if (keyArrayProperty.arraySize == 0)
            {
                var buttonPosition = position;
                buttonPosition.xMin = buttonPosition.xMax - buttonWidth;
                buttonPosition.height = UnityEditor.EditorGUIUtility.singleLineHeight;
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

            UnityEditor.EditorGUI.EndProperty();
        }

        private static float DrawKeyValueLine(UnityEditor.SerializedProperty keyProperty,
            UnityEditor.SerializedProperty valueProperty,
            Rect linePosition)
        {
            var labelWidth = UnityEditor.EditorGUIUtility.labelWidth;
            var labelWidthRelative = labelWidth / linePosition.width;

            var keyPropertyHeight = UnityEditor.EditorGUI.GetPropertyHeight(keyProperty);
            var keyPosition = linePosition;
            keyPosition.height = keyPropertyHeight;
            keyPosition.width = labelWidth - IndentWidth;
            UnityEditor.EditorGUIUtility.labelWidth = keyPosition.width * labelWidthRelative;
            UnityEditor.EditorGUI.PropertyField(keyPosition, keyProperty, GUIContent.none, true);

            var valuePropertyHeight = UnityEditor.EditorGUI.GetPropertyHeight(valueProperty);
            var valuePosition = linePosition;
            valuePosition.height = valuePropertyHeight;
            valuePosition.xMin += labelWidth;
            UnityEditor.EditorGUIUtility.labelWidth = valuePosition.width * labelWidthRelative;
            UnityEditor.EditorGUI.indentLevel--;
            UnityEditor.EditorGUI.PropertyField(valuePosition, valueProperty, GUIContent.none, true);
            UnityEditor.EditorGUI.indentLevel++;

            UnityEditor.EditorGUIUtility.labelWidth = labelWidth;

            return Mathf.Max(keyPropertyHeight, valuePropertyHeight);
        }

        private static void SaveProperty(UnityEditor.SerializedProperty keyProperty,
            UnityEditor.SerializedProperty valueProperty, int index,
            int otherIndex, ConflictState conflictState)
        {
            conflictState.ConflictKey = GetPropertyValue(keyProperty);
            conflictState.ConflictValue = valueProperty != null ? GetPropertyValue(valueProperty) : null;
            var keyPropertyHeight = UnityEditor.EditorGUI.GetPropertyHeight(keyProperty);
            var valuePropertyHeight =
                valueProperty != null ? UnityEditor.EditorGUI.GetPropertyHeight(valueProperty) : 0f;
            var lineHeight = Mathf.Max(keyPropertyHeight, valuePropertyHeight);
            conflictState.ConflictLineHeight = lineHeight;
            conflictState.ConflictIndex = index;
            conflictState.ConflictOtherIndex = otherIndex;
            conflictState.ConflictKeyPropertyExpanded = keyProperty.isExpanded;
            conflictState.ConflictValuePropertyExpanded = valueProperty != null && valueProperty.isExpanded;
        }

        public override float GetPropertyHeight(UnityEditor.SerializedProperty property, GUIContent label)
        {
            var propertyHeight = UnityEditor.EditorGUIUtility.singleLineHeight;

            if (property.isExpanded)
            {
                var keysProperty = property.FindPropertyRelative("keys");
                var valuesProperty = property.FindPropertyRelative("values");

                foreach (var entry in EnumerateEntries(keysProperty, valuesProperty))
                {
                    var keyProperty = entry.KeyProperty;
                    var valueProperty = entry.ValueProperty;
                    var keyPropertyHeight = UnityEditor.EditorGUI.GetPropertyHeight(keyProperty);
                    var valuePropertyHeight = valueProperty != null
                        ? UnityEditor.EditorGUI.GetPropertyHeight(valueProperty)
                        : 0f;
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

        private static ConflictState GetConflictState(UnityEditor.SerializedProperty property)
        {
            PropertyIdentity propId = new(property);
            if (!sConflictStateDict.TryGetValue(propId, out var conflictState))
            {
                conflictState = new ConflictState();
                sConflictStateDict.Add(propId, conflictState);
            }

            return conflictState;
        }

        private static readonly Dictionary<UnityEditor.SerializedPropertyType, PropertyInfo>
            sSerializedPropertyValueAccessorsDict;

        static MapPropertyDrawer()
        {
            Dictionary<UnityEditor.SerializedPropertyType, string> serializedPropertyValueAccessorsNameDict = new()
            {
                {
                    UnityEditor.SerializedPropertyType.Integer, "intValue"
                },
                {
                    UnityEditor.SerializedPropertyType.Boolean, "boolValue"
                },
                {
                    UnityEditor.SerializedPropertyType.Float, "floatValue"
                },
                {
                    UnityEditor.SerializedPropertyType.String, "stringValue"
                },
                {
                    UnityEditor.SerializedPropertyType.Color, "colorValue"
                },
                {
                    UnityEditor.SerializedPropertyType.ObjectReference, "objectReferenceValue"
                },
                {
                    UnityEditor.SerializedPropertyType.LayerMask, "intValue"
                },
                {
                    UnityEditor.SerializedPropertyType.Enum, "intValue"
                },
                {
                    UnityEditor.SerializedPropertyType.Vector2, "vector2Value"
                },
                {
                    UnityEditor.SerializedPropertyType.Vector3, "vector3Value"
                },
                {
                    UnityEditor.SerializedPropertyType.Vector4, "vector4Value"
                },
                {
                    UnityEditor.SerializedPropertyType.Rect, "rectValue"
                },
                {
                    UnityEditor.SerializedPropertyType.ArraySize, "intValue"
                },
                {
                    UnityEditor.SerializedPropertyType.Character, "intValue"
                },
                {
                    UnityEditor.SerializedPropertyType.AnimationCurve, "animationCurveValue"
                },
                {
                    UnityEditor.SerializedPropertyType.Bounds, "boundsValue"
                },
                {
                    UnityEditor.SerializedPropertyType.Quaternion, "quaternionValue"
                }
            };
            var serializedPropertyType = typeof(UnityEditor.SerializedProperty);

            sSerializedPropertyValueAccessorsDict = new Dictionary<UnityEditor.SerializedPropertyType, PropertyInfo>();
            const BindingFlags Flags = BindingFlags.Instance | BindingFlags.Public;

            foreach (var kvp in serializedPropertyValueAccessorsNameDict)
            {
                var propertyInfo = serializedPropertyType.GetProperty(kvp.Value, Flags);
                sSerializedPropertyValueAccessorsDict.Add(kvp.Key, propertyInfo);
            }
        }

        private static bool IsIntValue(UnityEditor.SerializedPropertyType type)
        {
            return type switch
            {
                UnityEditor.SerializedPropertyType.Enum => true,
                UnityEditor.SerializedPropertyType.Integer => true,
                _ => false
            };
        }

        private static GUIContent IconContent(string name, string tooltip)
        {
            var builtinIcon = UnityEditor.EditorGUIUtility.IconContent(name);
            return new GUIContent(builtinIcon.image, tooltip);
        }

        private static void DeleteArrayElementAtIndex(UnityEditor.SerializedProperty arrayProperty, int index)
        {
            var property = arrayProperty.GetArrayElementAtIndex(index);
            if (property.propertyType == UnityEditor.SerializedPropertyType.ObjectReference)
            {
                property.objectReferenceValue = null;
            }

            arrayProperty.DeleteArrayElementAtIndex(index);
        }

        public static object GetPropertyValue(UnityEditor.SerializedProperty p)
        {
            if (sSerializedPropertyValueAccessorsDict.TryGetValue(p.propertyType, out var propertyInfo))
            {
                return propertyInfo.GetValue(p, null);
            }
            if (p.isArray)
            {
                return GetPropertyValueArray(p);
            }
            return GetPropertyValueGeneric(p);
        }

        private static void SetPropertyValue(UnityEditor.SerializedProperty p, object v)
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

        private static object GetPropertyValueArray(UnityEditor.SerializedProperty property)
        {
            var array = new object[property.arraySize];
            for (var i = 0; i < property.arraySize; i++)
            {
                var item = property.GetArrayElementAtIndex(i);
                array[i] = GetPropertyValue(item);
            }

            return array;
        }

        private static object GetPropertyValueGeneric(UnityEditor.SerializedProperty property)
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

        private static void SetPropertyValueArray(UnityEditor.SerializedProperty property, object v)
        {
            var array = (object[])v;
            property.arraySize = array.Length;
            for (var i = 0; i < property.arraySize; i++)
            {
                var item = property.GetArrayElementAtIndex(i);
                SetPropertyValue(item, array[i]);
            }
        }

        private static void SetPropertyValueGeneric(UnityEditor.SerializedProperty property, object v)
        {
            var dict = (Dictionary<string, object>)v;
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
            return Equals(value1, value2);
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
            public readonly UnityEditor.SerializedProperty KeyProperty;
            public readonly UnityEditor.SerializedProperty ValueProperty;
            public readonly int Index;

            public EnumerationEntry(UnityEditor.SerializedProperty keyProperty,
                UnityEditor.SerializedProperty valueProperty, int index)
            {
                KeyProperty = keyProperty;
                ValueProperty = valueProperty;
                Index = index;
            }
        }

        private static IEnumerable<EnumerationEntry> EnumerateEntries(UnityEditor.SerializedProperty keyArrayProperty,
            UnityEditor.SerializedProperty valueArrayProperty, int startIndex = 0)
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
                         && !UnityEditor.SerializedProperty.EqualContents(keyProperty, endProperty));
            }
        }
    }
}