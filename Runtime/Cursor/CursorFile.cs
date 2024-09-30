using UnityEngine;

namespace Baracuda.Utility.Cursor
{
    /// <summary>
    ///     Class for storing custom cursor animation / texture data.
    /// </summary>
    public abstract class CursorFile : ScriptableObject
    {
        [SerializeField] private Vector2 hotSpot = Vector2.zero;
        [SerializeField] private CursorMode cursorMode = CursorMode.Auto;

        /// <summary>
        ///     The offset from the top left of the texture to use as the target point (must be within the bounds of the cursor).
        /// </summary>
        public Vector2 HotSpot => hotSpot;

        /// <summary>
        ///     Allow this cursor to render as a hardware cursor on supported platforms, or force software cursor.
        /// </summary>
        public CursorMode CursorMode => cursorMode;
    }
}