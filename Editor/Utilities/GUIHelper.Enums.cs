using System;
using Baracuda.Utility.Reflection;
using Baracuda.Utility.Utilities;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace Baracuda.Utility.Editor.Utilities
{
    public static partial class GUIUtility
    {
        public static T EnumButtons<T>(T current) where T : unmanaged, Enum
        {
            var color = GUI.backgroundColor;
            BeginBox();
            GUILayout.BeginHorizontal();

            foreach (T eValue in Enum.GetValues(typeof(T)))
            {
                GUI.backgroundColor = eValue.Equals(current) ? ActiveButtonColor : color;
                if (!GUILayout.Button(new GUIContent($"{eValue.ToString().Humanize()}",
                        $"{eValue.GetAttribute<TooltipAttribute>()?.tooltip}")))
                {
                    continue;
                }

                current = eValue;
                break;
            }

            GUILayout.EndHorizontal();
            EndBox();
            GUI.backgroundColor = color;
            return current;
        }

        public static T EnumButtons<T>(GUIContent label, T current) where T : unmanaged, Enum
        {
            var color = GUI.backgroundColor;
            BeginBox();
            GUILayout.BeginHorizontal();
            GUILayout.Label(label, RichTextStyle);
            foreach (T eValue in Enum.GetValues(typeof(T)))
            {
                GUI.backgroundColor = eValue.Equals(current) ? ActiveButtonColor : color;
                if (GUILayout.Button(new GUIContent($"{eValue.ToString()}",
                        $"{eValue.GetAttribute<TooltipAttribute>()?.tooltip}")))
                {
                    return eValue;
                }
            }

            GUILayout.EndHorizontal();
            EndBox();
            GUI.backgroundColor = color;
            return current;
        }

        public static T EnumLabel<T>(string label, T current) where T : unmanaged, Enum
        {
            return (T)UnityEditor.EditorGUILayout.EnumPopup(label, current);
        }

        /*
         *  Flags Enum Toggle
         */

        public static TEnum DrawFlagsEnumAsToggle<TEnum>(TEnum current, bool humanizeLabels)
            where TEnum : unmanaged, Enum
        {
            try
            {
                var values = Enum.GetValues(typeof(TEnum));
                long result = 0;
                var type = typeof(TEnum);

                foreach (TEnum value in values)
                {
                    var name = Enum.GetName(type, value);
                    if (GetAttribute<ObsoleteAttribute>(type, name) != null ||
                        !ConvertUnsafe<TEnum, int>(value).IsBinarySequenceExcludeZero())
                    {
                        continue;
                    }

                    if (UnityEditor.EditorGUILayout.Toggle(Enum.GetName(type, value).HumanizeIf(humanizeLabels),
                            current.HasFlagFast(value)))
                    {
                        result |= ConvertUnsafe<TEnum, long>(value);
                    }
                }

                return ConvertUnsafe<long, TEnum>(result);
            }
            catch (Exception exception)
            {
                Debug.LogError(exception);
                return current;
            }

            TAttribute GetAttribute<TAttribute>(Type enumType, string name) where TAttribute : Attribute
            {
                var memInfo = enumType.GetMember(name);
                var attributes = memInfo[0].GetCustomAttributes(typeof(TAttribute), false);
                return attributes.Length > 0 ? (TAttribute)attributes[0] : null;
            }

            static T2 ConvertUnsafe<T1, T2>(T1 value)
            {
                return UnsafeUtility.As<T1, T2>(ref value);
            }
        }

        public static int DrawFlagsEnumAsToggle(int current, Type enumType, bool humanizeLabels)
        {
            try
            {
                var values = Enum.GetValues(enumType);
                var result = 0;

                foreach (int value in values)
                {
                    var name = Enum.GetName(enumType, value);
                    if (GetAttribute<ObsoleteAttribute>(enumType, name) != null || !value.IsBinarySequenceExcludeZero())
                    {
                        continue;
                    }

                    if (UnityEditor.EditorGUILayout.Toggle(Enum.GetName(enumType, value).HumanizeIf(humanizeLabels), EnumUtility.HasFlagFast(current, value)))
                    {
                        result |= value;
                    }
                }

                return result;
            }
            catch (Exception exception)
            {
                Debug.LogError(exception);
                return current;
            }

            TAttribute GetAttribute<TAttribute>(Type enumType, string name) where TAttribute : Attribute
            {
                var memInfo = enumType.GetMember(name);
                var attributes = memInfo[0].GetCustomAttributes(typeof(TAttribute), false);
                return attributes.Length > 0 ? (TAttribute)attributes[0] : null;
            }
        }
    }
}