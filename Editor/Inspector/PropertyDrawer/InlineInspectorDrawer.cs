using Baracuda.Utilities.Helper;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Baracuda.Utilities.Inspector.PropertyDrawer
{
    [CustomPropertyDrawer(typeof(InlineInspectorAttribute))]
    internal class InlineInspectorDrawer : UnityEditor.PropertyDrawer
    {
        private Editor _inspector;
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
            EditorGUI.PropertyField(position, property, label);

            if (property.propertyType != SerializedPropertyType.ObjectReference)
            {
                return;
            }

            if (property.objectReferenceValue == null)
            {
                _inspector = null;
                return;
            }

            if (ReferenceEquals(property.objectReferenceValue, property.serializedObject.targetObject))
            {
                return;
            }

            _inspectorAttribute ??= (InlineInspectorAttribute) attribute;
            FoldoutHandler.BeginStyleOverride(FoldoutStyle.DarkGradient);
            GetOrCreateInspector(property).OnInspectorGUI();
            FoldoutHandler.EndStyleOverride();
        }

        private Editor GetOrCreateInspector(SerializedProperty property)
        {
            _inspectorAttribute ??= (InlineInspectorAttribute) attribute;

            if (_inspector && _inspector.target != property.objectReferenceValue)
            {
                _inspector = null;
            }
            _inspector ??= Editor.CreateEditor(property.objectReferenceValue);

            return _inspector;
        }
    }
}