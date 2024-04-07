using System.Runtime.CompilerServices;
using UnityEngine;

namespace Baracuda.Utilities.Editor.Helper
{
    public static partial class GUIUtility
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DrawRect(Rect rect)
        {
            UnityEditor.EditorGUI.DrawRect(rect,
                UnityEditor.EditorGUIUtility.isProSkin
                    ? LightColor
                    : DarkColor);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DrawRect(Rect rect, Color color)
        {
            UnityEditor.EditorGUI.DrawRect(rect, color);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DrawLine()
        {
            DrawLine(UnityEditor.EditorGUIUtility.isProSkin
                ? LightColor
                : DarkColor);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DrawLine(Color color, int thickness = 1, int padding = 1, bool space = false)
        {
            var rect = UnityEditor.EditorGUILayout.GetControlRect(GUILayout.Height(padding + thickness));
            rect.height = thickness;
            rect.y += padding * .5f;
            rect.x -= 3;
            rect.width += 6;
            UnityEditor.EditorGUI.DrawRect(rect, color);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DrawFullLine(Color? color = null, int thickness = 1, int padding = 1)
        {
            var rect = UnityEditor.EditorGUILayout.GetControlRect(GUILayout.Height(padding + thickness));
            rect.height = thickness;
            rect.y += padding * .5f;
            rect.x = 0;
            rect.width = GetViewWidth();
            UnityEditor.EditorGUI.DrawRect(rect,
                color ?? (UnityEditor.EditorGUIUtility.isProSkin
                    ? LightColor
                    : DarkColor));
        }

        /*
         * Textures
         */

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DrawTexture(Texture2D texture, int scale = 70)
        {
            var controlRect = UnityEditor.EditorGUILayout.GetControlRect(false, scale);
            var rect = new Rect(controlRect.x, controlRect.y, scale, scale);
            UnityEditor.EditorGUI.DrawTextureTransparent(rect, texture ? texture : Texture2D.linearGrayTexture);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DrawTextures(Texture2D[] textures, int scale = 70)
        {
            UnityEditor.EditorGUILayout.BeginHorizontal();
            var controlRect = UnityEditor.EditorGUILayout.GetControlRect(false, scale);
            for (var i = 0; i < textures.Length; i++)
            {
                var x = controlRect.x + scale * i;
                var rect = new Rect(x, controlRect.y, scale, scale);

                UnityEditor.EditorGUI.DrawTextureTransparent(rect, textures[i]);
            }

            UnityEditor.EditorGUILayout.EndHorizontal();
        }

        public static void DrawGradient(Rect rect, Color from, Color to, int steps = 10, int stepWidth = 2)
        {
            for (var i = 0; i < steps; i++)
            {
                var stepRect = new Rect(rect.x + i * stepWidth, rect.y, stepWidth, rect.height);
                var delta = (float) i / steps;

                var color = Color.Lerp(from, to, delta);
                UnityEditor.EditorGUI.DrawRect(stepRect, color);
            }

            UnityEditor.EditorGUI.DrawRect(
                new Rect(rect.x + steps * stepWidth, rect.y, rect.width - steps * stepWidth, rect.height), to);
        }
    }
}