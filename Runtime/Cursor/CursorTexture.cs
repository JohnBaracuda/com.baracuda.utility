using Baracuda.Bedrock.Odin;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Baracuda.Bedrock.Cursor
{
    public class CursorTexture : CursorFile
    {
        [Line(SpaceAfter = 0)]
        [Tooltip("Requires RGBA32 format, have alphaIsTransparency enabled, and have no mip chain!")]
        [InlineEditor(InlineEditorModes.LargePreview)]
        [SerializeField] private Texture2D texture;

        public Texture2D Texture => texture;

        public static implicit operator Texture2D(CursorTexture file)
        {
            return file ? file.texture : null;
        }
    }
}