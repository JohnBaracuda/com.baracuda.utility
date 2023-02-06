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
        private Action PreprocessDelegate { get; }
        protected object Target { get; }

        private readonly bool _hasConditional;

        protected static string[] Prefixes { get; } = {"_", "m_"};

        /*
         * Ctor
         */

        protected InspectorMember(MemberInfo member, object target)
        {
            Target = target;
            Member = member;
            HasHeaderAttribute = Member.HasAttribute<HeaderAttribute>();

            var attributes = member.GetCustomAttributes<PropertyAttribute>().ToArray();

            for (var i = 0; i < attributes.Length; i++)
            {
                var propertyAttribute = attributes[i];

                switch (propertyAttribute)
                {
                    case DrawSpaceAttribute spaceBeforeAttribute:
                        PreprocessDelegate += () => GUIHelper.Space(spaceBeforeAttribute.height);
                        break;

                    case DrawLineAttribute drawLineAttribute:
                        if (drawLineAttribute.SpaceBefore > -1)
                        {
                            var space = drawLineAttribute.SpaceBefore;
                            PreprocessDelegate += () => GUIHelper.Space(space);
                        }

                        PreprocessDelegate += GUIHelper.DrawLine;

                        if (drawLineAttribute.SpaceAfter > -1)
                        {
                            var space = drawLineAttribute.SpaceBefore;
                            PreprocessDelegate += () => GUIHelper.Space(space);
                        }

                        break;

                    case AnnotationAttribute annotationAttribute:
                        PreprocessDelegate += () => EditorGUILayout.HelpBox(annotationAttribute.Annotation, (MessageType) annotationAttribute.MessageType);
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
                }
            }

            displayMode ??= () => DisplayMode.Show;
        }

        /*
         * Abstract
         */

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
                    PreprocessDelegate?.Invoke();
                    DrawGUI();
                    return;
                case DisplayMode.Hide:
                    return;
                case DisplayMode.Readonly:
                    var enabled = GUI.enabled;
                    GUI.enabled = false;
                    PreprocessDelegate?.Invoke();
                    GUI.enabled = false;
                    DrawGUI();
                    GUI.enabled = enabled;
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