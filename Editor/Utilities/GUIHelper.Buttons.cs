using System;
using UnityEngine;

namespace Baracuda.Utility.Editor.Utilities
{
    public static partial class GUIUtility
    {
        public static Func<bool> RefreshButton => refreshButton ??= CreateButton("↻", "Refresh", 3, 2, 3, 0);
        public static Func<bool> OptionsButton => optionsButton ??= CreateButton("≡", "Show more options", 3, 2, 1, 2);
        public static Func<bool> ClearButton => clearButton ??= CreateButton("Ͼ", "Clear", 3, 2, 1, 2);
        public static Func<bool> AddButton => addButton ??= CreateButton("+", "Add a new object", 3, 2, 1, 2);

        public static Func<bool> RemoveButton =>
            removeButton ??= CreateButton("-", "Remove the selected object", 3, 2, 1, 2);

        public static Func<bool> SelectButton =>
            selectButton ??= CreateButton("⊙", "Select the current object", 3, 2, 1, 2);

        public static Func<bool> DownloadButton =>
            downloadButton ??= CreateButton("↓", "Download selected option", 3, 2, 1, 2);

        public static Func<bool> DeleteButton =>
            deleteButton ??= CreateButton("X", "Delete the selected object", 3, 2, 1, 2);

        private static Func<bool> refreshButton;
        private static Func<bool> optionsButton;
        private static Func<bool> clearButton;
        private static Func<bool> addButton;
        private static Func<bool> removeButton;
        private static Func<bool> selectButton;
        private static Func<bool> downloadButton;
        private static Func<bool> deleteButton;

        private static Func<bool> CreateButton(string character, string tooltip, int left, int right, int top,
            int bottom, float size = 32)
        {
            var content = new GUIContent(character, tooltip);
            var style = new GUIStyle(GUI.skin.button)
            {
                fontSize = 22,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
                padding = new RectOffset(left, right, top, bottom)
            };
            GUILayoutOption[] options =
            {
                GUILayout.Width(size)
            };
            return () => GUILayout.Button(content, style, options);
        }

        public static bool ButtonRight(GUIContent label, int size = 150)
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (GUILayout.Button(label, GUILayout.Width(size)))
            {
                GUILayout.EndHorizontal();
                return true;
            }

            GUILayout.EndHorizontal();
            return false;
        }

        public static bool ButtonCenter(GUIContent label, int width = 150, int height = 30)
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (GUILayout.Button(label, GUILayout.Width(width), GUILayout.Height(30)))
            {
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                return true;
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            return false;
        }

        public static bool ButtonCenter(string label, int width = 150, int height = 30)
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (GUILayout.Button(label, GUILayout.Width(width), GUILayout.Height(30)))
            {
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                return true;
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            return false;
        }
    }
}