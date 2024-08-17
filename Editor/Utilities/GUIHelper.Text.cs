using System.Runtime.CompilerServices;
using UnityEngine;

namespace Baracuda.Utilities.Editor.Utilities
{
    public static partial class GUIUtility
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DrawMessageBox(string message)
        {
            UnityEditor.EditorGUILayout.TextArea(message, HelpBoxStyle);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string SearchBar(string text)
        {
            return UnityEditor.EditorGUILayout.TextField(text, SearchBarStyle);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DrawTitle(GUIContent title)
        {
            var rect = UnityEditor.EditorGUILayout.GetControlRect(GUILayout.Height(32));
            UnityEditor.EditorGUI.LabelField(rect, title, BoldTitleStyle);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RichTextLabel(string content)
        {
            UnityEditor.EditorGUILayout.LabelField(content, RichTextStyle);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RichTextLabel(string label, string content)
        {
            UnityEditor.EditorGUILayout.LabelField(label, content, RichTextStyle);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RichTextLabel(GUIContent label, GUIContent content)
        {
            UnityEditor.EditorGUILayout.LabelField(label, content, RichTextStyle);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void BoldLabel(string content)
        {
            UnityEditor.EditorGUILayout.LabelField(content, BoldLabelStyle);
        }
    }
}