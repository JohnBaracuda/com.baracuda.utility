using Baracuda.Utilities.Helper;
using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Baracuda.Utilities.Inspector.InspectorFields
{
    public class PropertyInspectorMember : InspectorMember
    {
        private readonly PropertyInfo _propertyInfo;
        private readonly object _target;
        private readonly bool _drawList;
        private readonly bool _inline;
        private readonly MessageType _messageType;
        private readonly Action<object> _drawer;
        private Editor _editor;

        public PropertyInspectorMember(PropertyInfo propertyInfo, object target) : base(propertyInfo, target)
        {
            _propertyInfo = propertyInfo;
            _target = target;

            _inline = propertyInfo.HasAttribute<InlineInspectorAttribute>();

            var label = propertyInfo.TryGetCustomAttribute<LabelAttribute>(out var labelAttribute)
                ? labelAttribute.Label
                : _propertyInfo.Name.Humanize(Prefixes);
            var tooltip = propertyInfo.TryGetCustomAttribute<TooltipAttribute>(out var tooltipAttribute) ? tooltipAttribute.tooltip : null;

            Label = new GUIContent(label, tooltip);

            _drawer = GUIHelper.CreateDrawer(Label, propertyInfo.PropertyType);
        }

        protected override void DrawGUI()
        {
            var value = _propertyInfo.GetValue(_target);
            if (_inline && value is Object targetObject)
            {
                DrawInline(targetObject);
                return;
            }

            var enabled = GUI.enabled;
            GUI.enabled = false;
            _drawer(value);
            GUI.enabled = enabled;
        }

        private void DrawInline(Object targetObject)
        {
            if (targetObject == null)
            {
                _editor = null;
                return;
            }

            if (ReferenceEquals(targetObject, _target))
            {
                return;
            }

            if (_editor != null && _editor.target != targetObject)
            {
                _editor = null;
            }

            _editor ??= Editor.CreateEditor(targetObject);
            _editor.OnInspectorGUI();
            _editor.serializedObject.ApplyModifiedProperties();
        }
    }
}