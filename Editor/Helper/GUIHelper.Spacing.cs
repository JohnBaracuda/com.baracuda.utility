using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

namespace Baracuda.Utilities.Helper
{
    public static partial class GUIHelper
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IndentSpace()
        {
            var value = 10 * EditorGUI.indentLevel;
            GUILayout.Label("", GUILayout.Width(value));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Space()
        {
            EditorGUILayout.Space();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Space(bool condition)
        {
            if (condition)
            {
                EditorGUILayout.Space();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Space(float value)
        {
            EditorGUILayout.Space(value);
        }
    }
}