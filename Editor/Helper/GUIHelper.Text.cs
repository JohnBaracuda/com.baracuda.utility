using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

namespace Baracuda.Utilities.Helper
{
    public static partial class GUIHelper
    {
        /*
         * Message
         */

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DrawMessageBox(string message)
        {
            EditorGUILayout.TextArea(message, HelpBoxStyle);
        }

        /*
         * Search
         */

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string SearchBar(string text)
        {
            return EditorGUILayout.TextField(text, SearchBarStyle);
        }

        /*
         * Title
         */

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DrawTitle(GUIContent title)
        {
            var rect = EditorGUILayout.GetControlRect(GUILayout.Height(32));
            EditorGUI.LabelField(rect, title, BoldTitleStyle);
        }

        /*
         * RichText
         */

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RichTextLabel(string content)
        {
            EditorGUILayout.LabelField(content, RichTextStyle);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RichTextLabel(string label, string content)
        {
            EditorGUILayout.LabelField(label, content, RichTextStyle);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RichTextLabel(GUIContent label, GUIContent content)
        {
            EditorGUILayout.LabelField(label, content, RichTextStyle);
        }

        /*
         * Bold
         */

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void BoldLabel(string content)
        {
            EditorGUILayout.LabelField(content, BoldLabelStyle);
        }
    }
}