using Baracuda.Utilities.Helper;
using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Baracuda.Utilities.Inspector.InspectorFields
{
    public class PropertyInspectorMember : InspectorMember
    {
        private readonly PropertyInfo _propertyInfo;
        private readonly object _target;
        private readonly bool _drawSpace;
        private readonly float _space;
        private readonly bool _drawHeader;
        private readonly string _header;
        private readonly bool _drawList;
        private readonly bool _inline;
        private readonly bool _readonly;
        private readonly MessageType _messageType;
        private readonly ReorderableList _list;
        private Editor _editor;

        public PropertyInspectorMember(PropertyInfo propertyInfo, object target, Attribute attribute) : base(propertyInfo, target)
        {
            _propertyInfo = propertyInfo;
            _target = target;

            _inline = propertyInfo.HasAttribute<InlinePropertyAttribute>();
            _readonly = attribute is ReadonlyAttribute;

            var label = propertyInfo.TryGetCustomAttribute<LabelAttribute>(out var labelAttribute)
                ? labelAttribute.Label
                : _propertyInfo.Name.Humanize(Prefixes);
            var tooltip = propertyInfo.TryGetCustomAttribute<TooltipAttribute>(out var tooltipAttribute) ? tooltipAttribute.tooltip : null;

            Label = new GUIContent(label, tooltip);

            if (_propertyInfo.TryGetCustomAttribute<SpaceAttribute>(out var spaceAttribute))
            {
                _drawSpace = true;
                _space = spaceAttribute.height;
            }
            if (_propertyInfo.TryGetCustomAttribute<HeaderAttribute>(out var headerAttribute))
            {
                _drawHeader = true;
                _header = headerAttribute.header;
            }

            _list = new ReorderableList(Array.Empty<string>(), typeof(string), false, true, false,false);
            _list.drawHeaderCallback += rect => EditorGUI.LabelField(rect, Label);
        }

        protected override void DrawGUI()
        {
            if (_drawSpace)
            {
                GUIHelper.Space(_space);
            }
            if (_drawHeader)
            {
                GUIHelper.Space();
                GUIHelper.BoldLabel(_header);
            }

            var value = _propertyInfo.GetValue(_target);
            if (_inline && value is UnityEngine.Object uObject)
            {
                _editor ??= Editor.CreateEditor(uObject);
                _editor.OnInspectorGUI();
                _editor.serializedObject.ApplyModifiedProperties();
                return;
            }
            if (value?.GetType().IsIEnumerable(true) ?? false)
            {
                var enumerable = (IEnumerable) value;
                var items = enumerable as object[] ?? enumerable.Cast<object>().ToArray();
                _list.list = items;
                _list.DoLayoutList();
            }
            else if (_readonly)
            {
                var guiEnabled = GUI.enabled;
                GUI.enabled = false;
                GUIHelper.DynamicField(Label, value, _propertyInfo.PropertyType);
                GUI.enabled = guiEnabled;
            }
            else
            {
                GUIHelper.RichTextLabel(Label, new GUIContent(value?.ToString() ?? "null"));
            }
        }
    }
}