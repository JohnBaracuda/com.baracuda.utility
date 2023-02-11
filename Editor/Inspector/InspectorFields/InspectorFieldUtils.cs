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

            const BindingFlags Flags = BindingFlags.Instance | BindingFlags.NonPublic |
                                       BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Static;

            var fieldInfos = type.GetFieldsIncludeBaseTypes(Flags);
            for (var i = 0; i < fieldInfos.Length; i++)
            {
                HandleFieldInfo(target, fieldInfos[i], ref list);
            }

            var propertyInfos = type.GetPropertiesIncludeBaseTypes(Flags);
            for (var i = 0; i < propertyInfos.Length; i++)
            {
                HandlePropertyInfo(target, propertyInfos[i], ref list);
            }

            var methodInfos = type.GetMethodsIncludeBaseTypes(Flags);
            for (var i = 0; i < methodInfos.Length; i++)
            {
                HandleMethodInfo(target, methodInfos[i], ref list);
            }

            return list.ToArray();
        }

        #region Methods

        private static void HandleMethodInfo(SerializedObject target, MethodInfo methodInfo,
            ref List<InspectorMember> list)
        {
            if (methodInfo.TryGetCustomAttribute<ButtonAttribute>(out var buttonAttribute))
            {
                try
                {
                    list.Add(new MethodButtonInspectorMember(methodInfo, buttonAttribute, target.targetObject));
                }
                catch (Exception exception)
                {
                    list.Add(new ExceptionInspectorMember(
                        exception: exception,
                        memberInfo: methodInfo,
                        target: target.targetObject));
                }
            }
        }

        #endregion


        #region Properties

        private static void HandlePropertyInfo(SerializedObject target, PropertyInfo propertyInfo, ref List<InspectorMember> list)
        {
            try
            {
                var hasSetAccess = propertyInfo.SetMethod != null;

                if (propertyInfo.HasAttribute<ReadonlyAttribute>())
                {
                    list.Add(new ReadonlyPropertyInspector(propertyInfo, target.targetObject));
                    return;
                }

                if (propertyInfo.HasAttribute<ConditionalDrawerAttribute>())
                {
                    InspectorMember inspector = hasSetAccess
                        ? new PropertyInspector(propertyInfo, target.targetObject)
                        : new ReadonlyPropertyInspector(propertyInfo, target.targetObject);

                    list.Add(inspector);
                    return;
                }

                if (propertyInfo.HasAttribute<ShowInInspectorAttribute>())
                {
                    InspectorMember inspector = hasSetAccess
                        ? new PropertyInspector(propertyInfo, target.targetObject)
                        : new ReadonlyPropertyInspector(propertyInfo, target.targetObject);

                    list.Add(inspector);
                    return;
                }

                if (propertyInfo.HasAttribute<InlineInspectorAttribute>())
                {
                    InspectorMember inspector = hasSetAccess
                        ? new PropertyInspector(propertyInfo, target.targetObject)
                        : new ReadonlyPropertyInspector(propertyInfo, target.targetObject);

                    list.Add(inspector);
                    return;
                }
            }
            catch (Exception exception)
            {
                list.Add(new ExceptionInspectorMember(
                    exception: exception,
                    memberInfo: propertyInfo,
                    target: target.targetObject));
            }
        }

        #endregion


        #region Fields

        private static void HandleFieldInfo(SerializedObject target, FieldInfo fieldInfo, ref List<InspectorMember> list)
        {
            try
            {
                var isStatic = fieldInfo.IsStatic;
                var hideInInspector = fieldInfo.HasAttribute<HideInInspector>();
                var hasSerializeField = fieldInfo.HasAttribute<SerializeField>();
                var hasSerializeReference = fieldInfo.HasAttribute<SerializeField>();
                var isPublicField = fieldInfo.IsPublic && !fieldInfo.IsInitOnly;

                if (!hideInInspector && !isStatic && (hasSerializeField || hasSerializeReference || isPublicField))
                {
                    HandleSerializedField(target, fieldInfo, ref list);
                    return;
                }

                HandleNonSerializedField(target, fieldInfo, ref list);
            }
            catch (Exception exception)
            {
                list.Add(new ExceptionInspectorMember(
                    exception: exception,
                    memberInfo: fieldInfo,
                    target: target.targetObject));
            }
        }

        private static void HandleSerializedField(SerializedObject target, FieldInfo fieldInfo, ref List<InspectorMember> list)
        {
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

        private static void HandleNonSerializedField(SerializedObject target, FieldInfo fieldInfo, ref List<InspectorMember> list)
        {
            var showInInspector = fieldInfo.HasAttribute<ShowInInspectorAttribute>();
            var isReadonly = fieldInfo.HasAttribute<ReadonlyAttribute>();
            var conditionalDraw = fieldInfo.HasAttribute<ConditionalDrawerAttribute>();

            if (!showInInspector && !isReadonly && !conditionalDraw)
            {
                return;
            }

            if (isReadonly)
            {
                list.Add(new NonSerializedReadonlyFieldInspector(fieldInfo, target.targetObject));
                return;
            }

            list.Add(new NonSerializedFieldInspector(fieldInfo, target.targetObject));
        }

        #endregion
    }
}