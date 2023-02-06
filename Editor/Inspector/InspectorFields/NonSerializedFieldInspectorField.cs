using Baracuda.Utilities.Helper;
using System.Reflection;
using UnityEngine;

namespace Baracuda.Utilities.Inspector.InspectorFields
{
    public class NonSerializedMemberInspectorMember : InspectorMember
    {
        private readonly FieldInfo _fieldInfo;
        private readonly object _target;
        private readonly bool _drawSpace;
        private readonly float _space;
        private readonly bool _drawHeader;
        private readonly string _header;

        public NonSerializedMemberInspectorMember(FieldInfo fieldInfo, object target) : base(fieldInfo, target)
        {
            _fieldInfo = fieldInfo;
            _target = target;
            var label = fieldInfo.TryGetCustomAttribute<LabelAttribute>(out var labelAttribute)
                ? labelAttribute.Label
                : fieldInfo.Name.Humanize(Prefixes);

            var tooltip = fieldInfo.TryGetCustomAttribute<TooltipAttribute>(out var tooltipAttribute) ? tooltipAttribute.tooltip : null;

            Label = new GUIContent(label, tooltip);

            if (_fieldInfo.TryGetCustomAttribute<SpaceAttribute>(out var spaceAttribute))
            {
                _drawSpace = true;
                _space = spaceAttribute.height;
            }
            if (_fieldInfo.TryGetCustomAttribute<HeaderAttribute>(out var headerAttribute))
            {
                _drawHeader = true;
                _header = headerAttribute.header;
            }
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
            var enabled = GUI.enabled;
            GUI.enabled = false;
            GUIHelper.DynamicField(Label, _fieldInfo.GetValue(_target), _fieldInfo.FieldType);
            GUI.enabled = enabled;
        }
    }
}