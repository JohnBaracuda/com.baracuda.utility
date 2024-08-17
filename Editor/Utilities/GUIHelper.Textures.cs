using System.Runtime.CompilerServices;
using UnityEngine;

namespace Baracuda.Utilities.Editor.Utilities
{
    public static partial class GUIUtility
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Texture2D TextureField(string name, Texture2D texture, int scale = 70)
        {
            GUILayout.BeginVertical();
            var style = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.UpperCenter,
                fixedWidth = scale
            };
            GUILayout.Label(name, style);
            var result = (Texture2D)UnityEditor.EditorGUILayout.ObjectField(texture, typeof(Texture2D), false,
                GUILayout.Width(scale), GUILayout.Height(scale));
            GUILayout.EndVertical();
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Texture TextureField(string name, Texture texture)
        {
            var result = (Texture)UnityEditor.EditorGUILayout.ObjectField(name, texture, typeof(Texture), false);
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Texture2D TextureField(Texture2D texture, int scale = 70)
        {
            var result = (Texture2D)UnityEditor.EditorGUILayout.ObjectField(texture, typeof(Texture2D), false,
                GUILayout.Width(scale), GUILayout.Height(scale));
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Texture2D CreateTexture2D(int width, int height, Color col)
        {
            var pix = new Color[width * height];

            for (var i = 0; i < pix.Length; i++)
            {
                pix[i] = col;
            }

            var result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();

            return result;
        }
    }
}