using Baracuda.Utilities.Helper;
using System;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UIElements;

namespace Baracuda.Utilities.Inspector.InspectorFields
{
    public class SerializedPropertyInspectorMember : InspectorMember
    {
        private readonly SerializedObject _serializedObject;
        private readonly SerializedProperty _serializedProperty;
        private readonly bool _hideLabel;
        private readonly bool _textArea;
        private readonly bool _listInspector;
        private readonly bool _runtimeReadonly;
        private readonly bool _readonly;
        private readonly ReorderableList _reorderableList;

        public SerializedPropertyInspectorMember(SerializedProperty serializedProperty, MemberInfo memberInfo, object target) : base(memberInfo, target)
        {
            _serializedProperty = serializedProperty ?? throw new ArgumentNullException(nameof(serializedProperty));
            _serializedObject = serializedProperty.serializedObject;
            _hideLabel = memberInfo.HasAttribute<HideLabelAttribute>();
            _textArea = memberInfo.HasAttribute<TextAreaAttribute>();
            _runtimeReadonly = memberInfo.HasAttribute<RuntimeReadonlyAttribute>();
            _readonly = memberInfo.HasAttribute<ReadonlyAttribute>();

            var label = memberInfo.TryGetCustomAttribute<LabelAttribute>(out var labelAttribute)
                ? labelAttribute.Label
                : serializedProperty.name.Humanize(Prefixes);

            Label = new GUIContent(label, serializedProperty.tooltip);

            if (memberInfo.TryGetCustomAttribute<ListOptions>(out var listAttribute))
            {
                _listInspector = true;
                _reorderableList = new ReorderableList(serializedProperty.serializedObject, serializedProperty, listAttribute.Draggable, listAttribute.DisplayHeader, listAttribute.AddButton,listAttribute.RemoveButton);
                _reorderableList.drawHeaderCallback += rect => { EditorGUI.LabelField(rect, Label); };
                _reorderableList.elementHeight -= 4;
                _reorderableList.drawElementCallback += (rect, index, _, _) =>
                {
                    EditorGUI.PropertyField(rect, serializedProperty.GetArrayElementAtIndex(index), GUIContent.none);
                };
            }
        }

        protected override void DrawGUI()
        {
            _serializedObject.Update();
            var enabled = GUI.enabled;
            if (_readonly || (_runtimeReadonly && Application.isPlaying))
            {
                GUI.enabled = false;
            }

            if (_textArea)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(Label, GUILayout.Width(GUIHelper.GetLabelWidth()));
                GUIHelper.BeginIndentOverride(0);
                _serializedProperty.stringValue = EditorGUILayout.TextArea(_serializedProperty.stringValue);
                GUIHelper.EndIndentOverride();
                EditorGUILayout.EndHorizontal();
                GUI.enabled = enabled;
                _serializedObject.ApplyModifiedProperties();
                return;
            }

            if (_listInspector)
            {
                GUILayout.BeginHorizontal();
                GUIHelper.IndentSpace();
                _reorderableList.DoLayoutList();
                GUIHelper.IndentSpace();
                GUILayout.EndHorizontal();
                GUI.enabled = enabled;
                _serializedObject.ApplyModifiedProperties();
                return;
            }


            if (_hideLabel)
            {
                EditorGUILayout.PropertyField(_serializedProperty, GUIContent.none);
                GUI.enabled = enabled;
                _serializedObject.ApplyModifiedProperties();
                return;
            }

            try
            {
                EditorGUILayout.PropertyField(_serializedProperty, Label);
                _serializedObject.ApplyModifiedProperties();
            }
            catch (InvalidOperationException)
            {
            }

            GUI.enabled = enabled;
        }
    }
}