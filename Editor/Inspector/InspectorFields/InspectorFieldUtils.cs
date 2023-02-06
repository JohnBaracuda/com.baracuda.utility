using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

namespace Baracuda.Utilities.Inspector.InspectorFields
{
    public static class InspectorFieldUtils
    {
        public static InspectorMember[] GetInspectorMembers(SerializedObject target)
        {
            var type = target.targetObject.GetType();
            var list = new List<InspectorMember>();

            const BindingFlags FLAGS = BindingFlags.Instance | BindingFlags.NonPublic |
                                       BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Static;

            //TODO: Buttons last
            var memberInfos = type.GetMembersIncludeBaseTypes(FLAGS).OrderBy(data => data.MetadataToken).ToArray();

            for (var i = 0; i < memberInfos.Length; i++)
            {
                switch (memberInfos[i])
                {
                    case FieldInfo fieldInfo:
                        HandleFieldInfo(target, fieldInfo, ref list);
                        break;
                    case PropertyInfo propertyInfo:
                        HandlePropertyInfo(target, propertyInfo, ref list);
                        break;
                    case MethodInfo methodInfo:
                        HandleMethodInfo(target, methodInfo, ref list);
                        break;
                }
            }

            return list.ToArray();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void HandleMethodInfo(SerializedObject target, MethodInfo methodInfo, ref List<InspectorMember> list)
        {
            if (methodInfo.TryGetCustomAttribute<ButtonAttribute>(out var buttonAttribute))
            {
                try
                {
                    list.Add(new MethodButtonInspectorMember(methodInfo, buttonAttribute, target.targetObject));
                }
                catch (Exception exception)
                {
                    list.Add(new HelpBoxInspectorMember(
                        message: exception.Message,
                        messageType: MessageType.Error,
                        memberInfo: methodInfo,
                        target: target.targetObject));
                }
                return;
            }

            // if (methodInfo.TryGetCustomAttribute<ToggleAttribute>(out var toggleAttribute))
            // {
            //     try
            //     {
            //         list.Add(new MethodToggleInspectorMember(methodInfo, toggleAttribute, target.targetObject));
            //     }
            //     catch (Exception exception)
            //     {
            //         list.Add(new HelpBoxInspectorMember(
            //             message: exception.Message,
            //             messageType: MessageType.Error,
            //             memberInfo: methodInfo,
            //             target: target.targetObject));
            //     }
            //     return;
            // }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void HandlePropertyInfo(SerializedObject target, PropertyInfo propertyInfo, ref List<InspectorMember> list)
        {
            if (propertyInfo.TryGetCustomAttribute<ConditionalDrawerAttribute>(out var conditional))
            {
                list.Add(new PropertyInspectorMember(propertyInfo, target.targetObject, conditional));
                return;
            }

            if (propertyInfo.TryGetCustomAttribute<ShowInInspectorAttribute>(out var showInInspector))
            {
                list.Add(new PropertyInspectorMember(propertyInfo, target.targetObject, showInInspector));
                return;
            }

            if (propertyInfo.TryGetCustomAttribute<ReadonlyAttribute>(out var readonlyInspector))
            {
                list.Add(new PropertyInspectorMember(propertyInfo, target.targetObject, readonlyInspector));
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void HandleFieldInfo(SerializedObject target, FieldInfo fieldInfo, ref List<InspectorMember> list)
        {
            if (fieldInfo.FieldType.IsList())
            {
                if (fieldInfo.HasAttribute<ShowInInspectorAttribute>())
                {
                    list.Add(new NonSerializedListInspectorMember(fieldInfo, target.targetObject));
                    return;
                }

                if (fieldInfo.HasAttribute<ReadonlyAttribute>())
                {
                    list.Add(new NonSerializedListInspectorMember(fieldInfo, target.targetObject));
                    return;
                }
            }
            else
            {
                if (fieldInfo.HasAttribute<ShowInInspectorAttribute>())
                {
                    list.Add(new NonSerializedMemberInspectorMember(fieldInfo, target.targetObject));
                    return;
                }

                if (fieldInfo.HasAttribute<ReadonlyAttribute>())
                {
                    list.Add(new NonSerializedMemberInspectorMember(fieldInfo, target.targetObject));
                    return;
                }
            }

            if (fieldInfo.IsStatic)
            {
                return;
            }

            if (fieldInfo.HasAttribute<HideInInspector>())
            {
                return;
            }

            if (fieldInfo.IsPublic)
            {
                var prop = target.FindProperty(fieldInfo.Name);
                if (prop.IsNotNull())
                {
                    list.Add(new SerializedPropertyInspectorMember(prop, fieldInfo, target.targetObject));
                }
                return;
            }

            if (fieldInfo.LacksAttribute<SerializeField>() && fieldInfo.LacksAttribute<SerializeReference>())
            {
                return;
            }

            var serializedProperty = target.FindProperty(fieldInfo.Name);
            if (serializedProperty.IsNull())
            {
                list.Add(new HelpBoxInspectorMember(
                    message: $"Warning {fieldInfo.Name} cannot be accessed! Did you create a readonly get; accessor?",
                    messageType: MessageType.Warning,
                    memberInfo: fieldInfo,
                    target: target.targetObject));

                return;
            }

            list.Add(new SerializedPropertyInspectorMember(serializedProperty, fieldInfo, target.targetObject));
        }
    }
}