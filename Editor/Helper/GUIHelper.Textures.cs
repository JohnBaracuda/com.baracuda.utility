using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

namespace Baracuda.Utilities.Helper
{
    public static partial class GUIHelper
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
            var result = (Texture2D) EditorGUILayout.ObjectField(texture, typeof(Texture2D), false,
                GUILayout.Width(scale), GUILayout.Height(scale));
            GUILayout.EndVertical();
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Texture2D TextureField(Texture2D texture, int scale = 70)
        {
            var result = (Texture2D) EditorGUILayout.ObjectField(texture, typeof(Texture2D), false,
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