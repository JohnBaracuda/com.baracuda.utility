using System.Collections.Generic;
using Baracuda.Bedrock.Odin;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEngine;

namespace Baracuda.Utilities.Editor.Odin
{
    public class FoldoutAttributeDrawer : OdinGroupDrawer<FoldoutAttribute>
    {
        private LocalPersistentContext<bool> _isExpanded;

        private static readonly Dictionary<string, FoldoutAttributeDrawer> instances = new();

        protected override void Initialize()
        {
            const string IsExpandedKey = "FoldoutAttributeDrawer.isExpanded";
            var defaultExpandedState = GeneralDrawerConfig.Instance.ExpandFoldoutByDefault;

            _isExpanded = this.GetPersistentValue(IsExpandedKey, defaultExpandedState);

            instances[Property.Name] = this;
        }

        protected override void DrawPropertyLayout(GUIContent label)
        {
            var eventType = Event.current.type;
            var isAltPressed = Event.current.alt;

            var viewWidth = UnityEditor.EditorGUIUtility.currentViewWidth;
            var controlRect =
                SirenixEditorGUI.GetFeatureRichControlRect(GUIContent.none, out var _, out var _, out var labelRect);
            const int LineHeight = 1;
            var backgroundRect = new Rect(0, controlRect.y, viewWidth, controlRect.height + LineHeight * 2);
            var lineRect = new Rect(0, controlRect.y, viewWidth, LineHeight);
            var lineColor = new Color(0f, 0f, 0f, Attribute.LineAlpha);
            var rectColor = new Color(0f, 0f, 0f, Attribute.BackgroundAlpha);
            labelRect.y += LineHeight;

            UnityEditor.EditorGUI.DrawRect(lineRect, lineColor);
            UnityEditor.EditorGUI.DrawRect(backgroundRect, rectColor);

            var isExpanded = UnityEditor.EditorGUI.Foldout(labelRect, _isExpanded.Value, label, true);
            var hasChanged = isExpanded != _isExpanded.Value;
            _isExpanded.Value = isExpanded;

            if (hasChanged && isAltPressed && eventType == EventType.MouseUp)
            {
                SetOtherFoldoutsTo(isExpanded);
            }

            if (isExpanded)
            {
                DrawChildMember();
            }
        }

        private void DrawChildMember()
        {
            UnityEditor.EditorGUILayout.Space();
            for (var index = 0; index < Property.Children.Count; index++)
            {
                Property.Children[index].Draw();
            }
            UnityEditor.EditorGUILayout.Space();
        }

        private void SetOtherFoldoutsTo(bool isExpanded)
        {
            foreach (var foldoutAttributeDrawer in instances.Values)
            {
                if (foldoutAttributeDrawer != this)
                {
                    foldoutAttributeDrawer._isExpanded.Value = isExpanded;
                }
            }
        }
    }
}