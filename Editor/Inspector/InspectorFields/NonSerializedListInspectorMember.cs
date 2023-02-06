using Baracuda.Utilities.Helper;
using System.Collections;
using System.Reflection;
using UnityEditorInternal;
using UnityEngine;

namespace Baracuda.Utilities.Inspector.InspectorFields
{
    public class NonSerializedListInspectorMember : InspectorMember
    {
        private readonly bool _drawSpace;
        private readonly float _space;
        private readonly bool _drawHeader;
        private readonly string _header;
        private readonly ReorderableList _list;

        public NonSerializedListInspectorMember(FieldInfo fieldInfo, object target) : base(fieldInfo, target)
        {
            _list = new ReorderableList(fieldInfo.GetValue(target) as IList, fieldInfo.FieldType.GetElementType(), false, false, false, false);
            var label = fieldInfo.TryGetCustomAttribute<LabelAttribute>(out var labelAttribute)
                ? labelAttribute.Label
                : fieldInfo.Name.Humanize(Prefixes);

            var tooltip = fieldInfo.TryGetCustomAttribute<TooltipAttribute>(out var tooltipAttribute) ? tooltipAttribute.tooltip : null;
            Label = new GUIContent(label, tooltip);

            if (fieldInfo.TryGetCustomAttribute<SpaceAttribute>(out var spaceAttribute))
            {
                _drawSpace = true;
                _space = spaceAttribute.height;
            }
            if (fieldInfo.TryGetCustomAttribute<HeaderAttribute>(out var headerAttribute))
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

            _list.DoLayoutList();

            GUI.enabled = enabled;
        }
    }
}