using UnityEngine;

namespace Baracuda.Bedrock.Cursor
{
    public class CursorType : ScriptableObject
    {
        public static CursorType None => none ??= CreateInstance<CursorType>();

        private static CursorType none;
    }
}