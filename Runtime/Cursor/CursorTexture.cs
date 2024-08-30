using NaughtyAttributes;
using UnityEngine;

namespace Baracuda.Bedrock.Cursor
{
    public class CursorTexture : CursorFile
    {
        [Tooltip("Requires RGBA32 format, have alphaIsTransparency enabled, and have no mip chain!")]
        [ShowAssetPreview]
        [SerializeField] private Texture2D texture;

        public Texture2D Texture => texture;

        public static implicit operator Texture2D(CursorTexture file)
        {
            return file ? file.texture : null;
        }
    }
}