using System.Runtime.CompilerServices;
using UnityEngine;

namespace Baracuda.Utility.Editor.Utilities
{
    public static partial class GUIUtility
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IndentSpace()
        {
            var value = 10 * UnityEditor.EditorGUI.indentLevel;
            GUILayout.Label("", GUILayout.Width(value));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Space()
        {
            UnityEditor.EditorGUILayout.Space();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Space(bool condition)
        {
            if (condition)
            {
                UnityEditor.EditorGUILayout.Space();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Space(float value)
        {
            UnityEditor.EditorGUILayout.Space(value);
        }
    }
}