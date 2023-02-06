using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

namespace Baracuda.Utilities.Helper
{
    public static partial class GUIHelper
    {
        /*
         * Lines
         */

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DrawRect(Rect rect)
        {
            EditorGUI.DrawRect(rect, EditorGUIUtility.isProSkin ? LightColor : DarkColor);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DrawRect(Rect rect, Color color)
        {
            EditorGUI.DrawRect(rect, color);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DrawLine()
        {
            DrawLine(EditorGUIUtility.isProSkin ? LightColor : DarkColor);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DrawLine(Color color, int thickness = 1, int padding = 1, bool space = false)
        {
            var rect = EditorGUILayout.GetControlRect(GUILayout.Height(padding + thickness));
            rect.height = thickness;
            rect.y += padding * .5f;
            rect.x -= 3;
            rect.width += 6;
            EditorGUI.DrawRect(rect, color);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DrawFullLine(Color? color = null, int thickness = 1, int padding = 1)
        {
            var rect = EditorGUILayout.GetControlRect(GUILayout.Height(padding + thickness));
            rect.height = thickness;
            rect.y += padding * .5f;
            rect.x = 0;
            rect.width = GetViewWidth();
            EditorGUI.DrawRect(rect, color ?? (EditorGUIUtility.isProSkin ? LightColor : DarkColor));
        }

        /*
         * Textures
         */

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DrawTexture(Texture2D texture, int scale = 70)
        {
            var controlRect = EditorGUILayout.GetControlRect(false, scale);
            var rect = new Rect(controlRect.x, controlRect.y, scale, scale);
            EditorGUI.DrawTextureTransparent(rect, texture ? texture : Texture2D.linearGrayTexture);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DrawTextures(Texture2D[] textures, int scale = 70)
        {
            EditorGUILayout.BeginHorizontal();
            var controlRect = EditorGUILayout.GetControlRect(false, scale);
            for (var i = 0; i < textures.Length; i++)
            {
                var x = controlRect.x + scale * i;
                var rect = new Rect(x, controlRect.y, scale, scale);

                EditorGUI.DrawTextureTransparent(rect, textures[i]);
            }

            EditorGUILayout.EndHorizontal();
        }

        public static void DrawGradient(Rect rect, Color from, Color to, int steps = 10, int stepWidth = 2)
        {
            for (var i = 0; i < steps; i++)
            {
                var stepRect = new Rect(rect.x + (i * stepWidth), rect.y, stepWidth, rect.height);
                var delta = (float) i / steps;

                var color = Color.Lerp(from, to, delta);
                EditorGUI.DrawRect(stepRect, color);
            }

            EditorGUI.DrawRect(new Rect(rect.x + (steps * stepWidth), rect.y, rect.width - (steps * stepWidth), rect.height), to);
        }
    }
}