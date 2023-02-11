using Baracuda.Utilities.Helper;
using Baracuda.Utilities.Inspector.PropertyDrawer;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

namespace Baracuda.Utilities.Inspector.InspectorFields
{
    public abstract class InspectorMember
    {
        /*
         * Fields & Properties
         */

        public GUIContent Label { get; protected set; }
        public MemberInfo Member { get;}
        public bool HasHeaderAttribute { get;}
        protected object Target { get; }

        private readonly bool _hasConditional;
        private readonly Action _preDraw;
        private readonly Action _postDraw;

        protected static string[] Prefixes { get; } = {"_", "m_"};

        /*
         * Ctor
         */

        protected InspectorMember(MemberInfo member, object target)
        {
            Target = target;
            Member = member;
            HasHeaderAttribute = Member.HasAttribute<HeaderAttribute>();

            var isSerialized = member.HasAttribute<SerializeField>() || member is FieldInfo {IsPublic: true};

            var attributes = member.GetCustomAttributes<PropertyAttribute>().ToArray();

            for (var i = 0; i < attributes.Length; i++)
            {
                var propertyAttribute = attributes[i];

                switch (propertyAttribute)
                {
                    case SpaceBeforeAttribute spaceBeforeAttribute:
                        _preDraw += () => GUIHelper.Space(spaceBeforeAttribute.height);
                        break;

                    case SpaceAfterAttribute spaceAfterAttribute:
                        _postDraw += () => GUIHelper.Space(spaceAfterAttribute.Height);
                        break;

                    case SpaceAttribute spaceAttribute when !isSerialized:
                        _preDraw += () => GUIHelper.Space(spaceAttribute.height);
                        break;

                    case HeaderAttribute headerAttribute when !isSerialized:
                        _preDraw += () =>
                        {
                            GUIHelper.Space();
                            GUIHelper.BoldLabel(headerAttribute.header);
                        };
                        break;

                    case DrawLineAttribute drawLineAttribute:
                        if (drawLineAttribute.SpaceBefore > -1)
                        {
                            var space = drawLineAttribute.SpaceBefore;
                            _preDraw += () => GUIHelper.Space(space);
                        }

                        _preDraw += GUIHelper.DrawLine;

                        if (drawLineAttribute.SpaceAfter > -1)
                        {
                            var space = drawLineAttribute.SpaceBefore;
                            _preDraw += () => GUIHelper.Space(space);
                        }

                        break;

                    case AnnotationAttribute annotationAttribute:
                        _preDraw += () => EditorGUILayout.HelpBox(annotationAttribute.Annotation, (MessageType) annotationAttribute.MessageType);
                        break;

                    case ConditionalShowAttribute conditionalAttribute:
                        displayMode = () =>
                        {
                            var result = ConditionalShowValidator.ValidateComparison(conditionalAttribute.Condition,
                                Target.CreateGetDelegateForMember(conditionalAttribute.Member), false);

                            return result ? DisplayMode.Show :
                                conditionalAttribute.ReadOnly ? DisplayMode.Readonly : DisplayMode.Hide;
                        };
                        break;

                    case ConditionalHideAttribute conditionalAttribute:
                        displayMode = () =>
                        {
                            var result = ConditionalShowValidator.ValidateComparison(conditionalAttribute.Condition,
                                Target.CreateGetDelegateForMember(conditionalAttribute.Member), true);

                            return result ? DisplayMode.Show :
                                conditionalAttribute.ReadOnly ? DisplayMode.Readonly : DisplayMode.Hide;
                        };
                        break;

                    case BeginBoxAttribute beginBoxAttribute:
                        _preDraw += () => GUIHelper.BeginBox(beginBoxAttribute.Style);
                        break;

                    case EndBoxAttribute:
                        _postDraw += GUIHelper.EndBox;
                        break;
                }
            }

            displayMode ??= () => DisplayMode.Show;
        }

        private enum DisplayMode
        {
            Show,
            Hide,
            Readonly
        }

        private readonly Func<DisplayMode> displayMode;

        public void ProcessGUI()
        {
            switch (displayMode())
            {
                case DisplayMode.Show:
                    _preDraw?.Invoke();
                    DrawGUI();
                    _postDraw?.Invoke();
                    return;
                case DisplayMode.Hide:
                    return;
                case DisplayMode.Readonly:
                    var enabled = GUI.enabled;
                    GUI.enabled = false;
                    _preDraw?.Invoke();
                    GUI.enabled = false;
                    DrawGUI();
                    GUI.enabled = enabled;
                    _postDraw?.Invoke();
                    return;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        [DebuggerHidden]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected abstract void DrawGUI();
    }
}