using Baracuda.Utilities.Helper;
using System;
using System.Reflection;
using UnityEngine;

namespace Baracuda.Utilities.Inspector.InspectorFields
{
    public class MethodButtonInspectorMember : InspectorMember
    {
        private readonly Action _methodCall;
        private readonly ButtonType _buttonType;
        private readonly float _spaceBefore;
        private readonly float _spaceAfter;
        private readonly bool _conditional;


        public MethodButtonInspectorMember(MethodInfo methodInfo, ButtonAttribute attribute, object target) : base(methodInfo, target)
        {
            _methodCall = methodInfo.IsStatic?
                (Action) methodInfo.CreateMatchingDelegate() :
                (Action) methodInfo.CreateDelegate(typeof(Action), target);

            _buttonType = attribute.ButtonType;
            _spaceBefore = attribute.SpaceBefore;
            _spaceAfter = attribute.SpaceAfter;
            var label = attribute.Label ?? methodInfo.Name.Humanize();
            var tooltip = methodInfo.TryGetCustomAttribute<TooltipAttribute>(out var tooltipAttribute) ? tooltipAttribute.tooltip : null;
            Label = new GUIContent(label, tooltip);
        }

        protected override void DrawGUI()
        {
            GUIHelper.Space(_spaceBefore);
            switch (_buttonType)
            {
                case ButtonType.Default:
                    if (GUILayout.Button(Label))
                    {
                        _methodCall?.Invoke();
                    }
                    break;
                case ButtonType.Center:
                    if (GUIHelper.ButtonCenter(Label))
                    {
                        _methodCall?.Invoke();
                    }
                    break;
                case ButtonType.Right:
                    if (GUIHelper.ButtonRight(Label))
                    {
                        _methodCall?.Invoke();
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            GUIHelper.Space(_spaceAfter);
        }
    }
}