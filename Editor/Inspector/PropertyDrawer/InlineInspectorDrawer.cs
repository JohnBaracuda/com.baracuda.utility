using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Baracuda.Utilities.Inspector.PropertyDrawer
{
    [CustomPropertyDrawer(typeof(InlineInspectorAttribute))]
    internal class InlineInspectorDrawer : UnityEditor.PropertyDrawer
    {
        private UnityEditor.Editor _inspector;
        private InlineInspectorAttribute _inspectorAttribute;
        private string _key;

        private static readonly Dictionary<Object, FoldoutHandler> foldoutHandlers = new();

        private FoldoutHandler Foldout(Object target)
        {
            if (foldoutHandlers.TryGetValue(target, out var handler))
            {
                return handler;
            }

            handler = new FoldoutHandler(target.name);
            foldoutHandlers.Add(target, handler);
            return handler;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.ObjectReference)
            {
                return;
            }

            if (property.objectReferenceValue == null)
            {
                EditorGUI.PropertyField(position, property, label);
                _inspector = null;
                return;
            }

            _inspectorAttribute ??= (InlineInspectorAttribute) attribute;

            if (!_inspectorAttribute.Simple)
            {
                EditorGUI.indentLevel--;
                EditorGUI.PropertyField(position, property, new GUIContent(" "));
                EditorGUI.indentLevel++;

                FoldoutHandler.Style = FoldoutStyle.Title;
                if (Foldout(property.serializedObject.targetObject)[property.displayName])
                {
                    EditorGUI.indentLevel++;
                    FoldoutHandler.Style = FoldoutStyle.Default;
                    _inspector ??= UnityEditor.Editor.CreateEditor(property.objectReferenceValue);
                    _inspector.OnInspectorGUI();
                    EditorGUI.indentLevel--;
                }
                FoldoutHandler.Style = FoldoutStyle.Default;
                EditorGUILayout.Space();
            }
            else
            {
                EditorGUILayout.LabelField(label);
                _inspector ??= Editor.CreateEditor(property.objectReferenceValue);
                _inspector.OnInspectorGUI();
            }
        }
    }
}