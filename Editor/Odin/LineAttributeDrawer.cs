using Baracuda.Bedrock.Odin;
using Baracuda.Bedrock.Utilities;
using Sirenix.OdinInspector.Editor;
using UnityEngine;

namespace Baracuda.Utilities.Editor.Odin
{
    public class LineAttributeDrawer : OdinAttributeDrawer<LineAttribute>
    {
        private static Color LightColor { get; } = new(.1f, .1f, .1f, .9f);
        private static Color DarkColor { get; } = new(.2f, .2f, .2f, .5f);

        protected override void DrawPropertyLayout(GUIContent label)
        {
            if (Attribute.DrawTiming.HasFlagFast(DrawTiming.Before))
            {
                DrawLine();
            }

            CallNextDrawer(label);

            if (Attribute.DrawTiming.HasFlagFast(DrawTiming.After))
            {
                DrawLine();
            }
        }

        private void DrawLine()
        {
            GUILayout.Space(Attribute.SpaceBefore);
            var rect = UnityEditor.EditorGUILayout.GetControlRect(GUILayout.Height(2));
            rect.height = 1;
            rect.y += .5f;
            rect.x = 0;
            rect.width = UnityEditor.EditorGUIUtility.currentViewWidth;
            UnityEditor.EditorGUI.DrawRect(rect, UnityEditor.EditorGUIUtility.isProSkin ? LightColor : DarkColor);
            GUILayout.Space(Attribute.SpaceAfter);
        }
    }
}