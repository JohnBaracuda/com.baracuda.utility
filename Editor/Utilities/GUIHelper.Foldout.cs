using System.Runtime.CompilerServices;
using UnityEngine;

namespace Baracuda.Utility.Editor.Utilities
{
    public static partial class GUIUtility
    {
        public static bool FoldoutArea(bool value, string label)
        {
            UnityEditor.EditorGUILayout.BeginHorizontal();
            UnityEditor.EditorGUILayout.LabelField("", GUILayout.Width(10));
            value = UnityEditor.EditorGUILayout.Foldout(value, label, true);
            UnityEditor.EditorGUILayout.EndHorizontal();
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Foldout(bool value, string label, string tooltip = "", Color? color = null)
        {
            UnityEditor.EditorGUILayout.LabelField("");
            var lastRect = GetLastRect();
            var widthRect = new Rect(0, lastRect.y, GetViewWidth(), lastRect.height + 2);
            var foldoutRect = new Rect(20, lastRect.y + 1, GetViewWidth() - 10, lastRect.height);
            UnityEditor.EditorGUI.DrawRect(new Rect(0, lastRect.y, GetViewWidth(), 1), new Color(0f, 0f, 0f, 0.3f));
            UnityEditor.EditorGUI.DrawRect(widthRect, color.GetValueOrDefault(new Color(0f, 0f, 0f, 0.15f)));
            EmptyContent.text = label;
            EmptyContent.tooltip = tooltip;
            var result = UnityEditor.EditorGUI.Foldout(foldoutRect, value, EmptyContent, true);
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool LineFoldout(bool value, string label, string tooltip = "", Color? color = null)
        {
            UnityEditor.EditorGUILayout.LabelField("");
            var lastRect = GetLastRect();
            var foldoutRect = new Rect(20, lastRect.y + 1, GetViewWidth() - 10, lastRect.height);
            UnityEditor.EditorGUI.DrawRect(new Rect(0, lastRect.y, GetViewWidth(), 1), new Color(0f, 0f, 0f, 0.3f));
            EmptyContent.text = label;
            EmptyContent.tooltip = tooltip;
            var result = UnityEditor.EditorGUI.Foldout(foldoutRect, value, EmptyContent, true);
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool DarkFoldout(bool value, string label, string tooltip = "")
        {
            UnityEditor.EditorGUILayout.LabelField("");
            var lastRect = GetLastRect();
            var widthRect = new Rect(0, lastRect.y, GetViewWidth(), lastRect.height + 2);
            var foldoutRect = new Rect(20, lastRect.y + 1, GetViewWidth() - 10, lastRect.height);
            UnityEditor.EditorGUI.DrawRect(new Rect(0, lastRect.y, GetViewWidth(), 1), new Color(0f, 0f, 0f, 0.4f));
            UnityEditor.EditorGUI.DrawRect(widthRect, new Color(0f, 0f, 0f, 0.25f));
            EmptyContent.text = label;
            EmptyContent.tooltip = tooltip;
            var result = UnityEditor.EditorGUI.Foldout(foldoutRect, value, EmptyContent, true);
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TitleFoldout(bool value, string label, string tooltip = "")
        {
            UnityEditor.EditorGUILayout.LabelField("");
            var lastRect = GetLastRect();
            var widthRect = new Rect(0, lastRect.y, GetViewWidth(), lastRect.height + 6);
            var foldoutRect = new Rect(20, lastRect.y - 2, GetViewWidth() - 10, lastRect.height + 8);
            UnityEditor.EditorGUI.DrawRect(new Rect(0, lastRect.y, GetViewWidth(), 1), new Color(0f, 0f, 0f, 0.4f));
            UnityEditor.EditorGUI.DrawRect(widthRect, new Color(0f, 0f, 0f, 0.25f));
            EmptyContent.text = label;
            EmptyContent.tooltip = tooltip;
            var result = UnityEditor.EditorGUI.Foldout(foldoutRect, value, EmptyContent, true, BoldFoldoutStyle);
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool DynamicFoldout(bool value, string label, string tooltip = "")
        {
            UnityEditor.EditorGUILayout.LabelField("");
            var lastRect = GetLastRect();
            var widthRect = new Rect(0, lastRect.y, lastRect.width + 5, lastRect.height + 2);
            var foldoutRect = new Rect(10, lastRect.y + 1, lastRect.width, lastRect.height);
            UnityEditor.EditorGUI.DrawRect(new Rect(0, lastRect.y, lastRect.width + 5, 1), new Color(0f, 0f, 0f, 0.3f));
            UnityEditor.EditorGUI.DrawRect(widthRect, new Color(0f, 0f, 0f, 0.15f));
            EmptyContent.text = label;
            EmptyContent.tooltip = tooltip;
            return UnityEditor.EditorGUI.Foldout(foldoutRect, value, EmptyContent, true);
        }
    }
}