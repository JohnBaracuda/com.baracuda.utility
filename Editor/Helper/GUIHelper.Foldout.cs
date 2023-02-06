using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

namespace Baracuda.Utilities.Helper
{
    public static partial class GUIHelper
    {
        public static bool FoldoutArea(bool value, string label)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("", GUILayout.Width(10));
            value = EditorGUILayout.Foldout(value, label, true);
            EditorGUILayout.EndHorizontal();
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Foldout(bool value, string label, string tooltip = "", Color? color = null)
        {
            EditorGUILayout.LabelField("");
            var lastRect = GetLastRect();
            var widthRect = new Rect(0, lastRect.y, GetViewWidth(), lastRect.height + 2);
            var foldoutRect = new Rect(20, lastRect.y + 1, GetViewWidth() - 10, lastRect.height);
            EditorGUI.DrawRect(new Rect(0, lastRect.y, GetViewWidth(), 1), new Color(0f, 0f, 0f, 0.3f));
            EditorGUI.DrawRect(widthRect, color.GetValueOrDefault(new Color(0f, 0f, 0f, 0.15f)));
            EmptyContent.text = label;
            EmptyContent.tooltip = tooltip;
            var result = EditorGUI.Foldout(foldoutRect, value, EmptyContent, true);
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool LineFoldout(bool value, string label, string tooltip = "", Color? color = null)
        {
            EditorGUILayout.LabelField("");
            var lastRect = GetLastRect();
            var foldoutRect = new Rect(20, lastRect.y + 1, GetViewWidth() - 10, lastRect.height);
            EditorGUI.DrawRect(new Rect(0, lastRect.y, GetViewWidth(), 1), new Color(0f, 0f, 0f, 0.3f));
            EmptyContent.text = label;
            EmptyContent.tooltip = tooltip;
            var result = EditorGUI.Foldout(foldoutRect, value, EmptyContent, true);
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool DarkFoldout(bool value, string label, string tooltip = "")
        {
            EditorGUILayout.LabelField("");
            var lastRect = GetLastRect();
            var widthRect = new Rect(0, lastRect.y, GetViewWidth(), lastRect.height + 2);
            var foldoutRect = new Rect(20, lastRect.y + 1, GetViewWidth() - 10, lastRect.height);
            EditorGUI.DrawRect(new Rect(0, lastRect.y, GetViewWidth(), 1), new Color(0f, 0f, 0f, 0.4f));
            EditorGUI.DrawRect(widthRect, new Color(0f, 0f, 0f, 0.25f));
            EmptyContent.text = label;
            EmptyContent.tooltip = tooltip;
            var result = EditorGUI.Foldout(foldoutRect, value, EmptyContent, true);
            return result;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TitleFoldout(bool value, string label, string tooltip = "")
        {
            EditorGUILayout.LabelField("");
            var lastRect = GetLastRect();
            var widthRect = new Rect(0, lastRect.y, GetViewWidth(), lastRect.height + 6);
            var foldoutRect = new Rect(20, lastRect.y - 2, GetViewWidth() - 10, lastRect.height + 8);
            EditorGUI.DrawRect(new Rect(0, lastRect.y, GetViewWidth(), 1), new Color(0f, 0f, 0f, 0.4f));
            EditorGUI.DrawRect(widthRect, new Color(0f, 0f, 0f, 0.25f));
            EmptyContent.text = label;
            EmptyContent.tooltip = tooltip;
            var result = EditorGUI.Foldout(foldoutRect, value, EmptyContent, true, BoldFoldoutStyle);
            return result;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool DynamicFoldout(bool value, string label, string tooltip = "")
        {
            EditorGUILayout.LabelField("");
            var lastRect = GetLastRect();
            var widthRect = new Rect(0, lastRect.y, lastRect.width + 5, lastRect.height + 2);
            var foldoutRect = new Rect(10, lastRect.y + 1, lastRect.width, lastRect.height);
            EditorGUI.DrawRect(new Rect(0, lastRect.y, lastRect.width + 5, 1), new Color(0f, 0f, 0f, 0.3f));
            EditorGUI.DrawRect(widthRect, new Color(0f, 0f, 0f, 0.15f));
            EmptyContent.text = label;
            EmptyContent.tooltip = tooltip;
            return EditorGUI.Foldout(foldoutRect, value, EmptyContent, true);
        }
    }
}