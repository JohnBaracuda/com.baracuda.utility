using System;
using UnityEditor;
using UnityEngine;

namespace Baracuda.Utilities.Helper
{
    public static partial class GUIHelper
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
            return (T) EditorGUILayout.EnumPopup(label, current);
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
                    if (GetAttribute<ObsoleteAttribute>(type, name) != null || !value.ConvertUnsafe<TEnum, int>().IsBinarySequenceExcludeZero())
                    {
                        continue;
                    }

                    if (EditorGUILayout.Toggle(Enum.GetName(type, value).HumanizeIf(humanizeLabels),
                            current.HasFlagUnsafe(value)))
                    {
                        result |= value.ConvertUnsafe<TEnum, long>();
                    }
                }

                return result.ConvertUnsafe<long, TEnum>();
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
                return attributes.Length > 0 ? (TAttribute) attributes[0] : null;
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

                    if (EditorGUILayout.Toggle(Enum.GetName(enumType, value).HumanizeIf(humanizeLabels),
                            current.HasFlagInt(value)))
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
                return attributes.Length > 0 ? (TAttribute) attributes[0] : null;
            }
        }
    }
}