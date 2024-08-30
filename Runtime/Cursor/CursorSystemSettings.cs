using NaughtyAttributes;
using UnityEngine;

namespace Baracuda.Bedrock.Cursor
{
    public class CursorSystemSettings : ScriptableObject
    {
        [SerializeField] private CursorType startCursor;
        [SerializeField] [Required] private CursorSet startCursorSet;

        public CursorType StartCursor => startCursor;
        public CursorSet StartCursorSet => startCursorSet;
    }
}