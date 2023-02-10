using Baracuda.Utilities.Helper;
using System;
using System.Reflection;
using UnityEngine;

namespace Baracuda.Utilities.Inspector.InspectorFields
{
    public class NonSerializedMemberInspectorMember : InspectorMember
    {
        private readonly FieldInfo _fieldInfo;
        private readonly object _target;
        private readonly Action<object> _drawer;


        public NonSerializedMemberInspectorMember(FieldInfo fieldInfo, object target) : base(fieldInfo, target)
        {
            _fieldInfo = fieldInfo;
            _target = target;
            var label = fieldInfo.TryGetCustomAttribute<LabelAttribute>(out var labelAttribute)
                ? labelAttribute.Label
                : fieldInfo.Name.Humanize(Prefixes);

            var tooltip = fieldInfo.TryGetCustomAttribute<TooltipAttribute>(out var tooltipAttribute) ? tooltipAttribute.tooltip : null;

            Label = new GUIContent(label, tooltip);

            _drawer = GUIHelper.CreateDrawer(Label, _fieldInfo.FieldType);
        }

        protected override void DrawGUI()
        {
            var enabled = GUI.enabled;
            GUI.enabled = false;
            _drawer(_fieldInfo.GetValue(_target));
            GUI.enabled = enabled;
        }
    }
}